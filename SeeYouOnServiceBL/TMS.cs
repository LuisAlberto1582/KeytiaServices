using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Data;
using System.Text;

using KeytiaServiceBL;

using SeeYouOnServiceBL;
using SeeYouOnServiceBL.TMSBooking;
using SeeYouOnServiceBL.TMSRemoteSetup;
using SeeYouOnServiceBL.TMSManagement;
using SeeYouOnServiceBL.TMSPhoneBook;
using SeeYouOnServiceBL.XmlRpc;

namespace SeeYouOnServiceBL
{
    public class TMS
    {
        protected KDBAccess kdb = new KDBAccess();

        protected List<string> plSystem = new List<string>();
        protected List<string> plSystemFolder = new List<string>();
        protected List<string> plSystemAddress = new List<string>();

        protected List<string> plContact = new List<string>();
        protected List<string> plContactFolder = new List<string>();
        protected List<string> plContactAddress = new List<string>();

        protected List<string> plProvGroup = new List<string>();

        protected string psDominioCtaMovi = "";

        protected DataTable pdtDeletedSystems = new DataTable();
        protected DataTable pdtDeletedPhoneBooks = new DataTable();
        protected DataTable pdtDeletedProvGroups = new DataTable();
        protected DataTable pdtDeletedMovi = new DataTable();


        #region Sync Systems
        public void SyncProvGroups(int liCodTMS)
        {
            try
            {
                SyncProvGroupsTMS2SYO(liCodTMS);
                //SyncProvGroupsTMS2SYODel(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de grupos de provisioning.", ex);
            }
        }

        public void SyncProvGroupsTMS2SYO(int liCodTMS)
        {
            try
            {
                plProvGroup.Clear();

                string lsUrlOpenDS = (string)KDBUtil.SearchScalar("ServidorTMS", liCodTMS, "{URLOpenDS}");

                DirectoryEntry de = new DirectoryEntry(lsUrlOpenDS + "/ou=groups,dc=provisioning",
                    "cn=Directory Manager", "3v0xCONFsyo", AuthenticationTypes.ServerBind);

                foreach (DirectoryEntry loGroup in de.Children)
                    SyncProvGroupsTMS2SYOGroup(liCodTMS, loGroup);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de conferencias TMS->SYO.", ex);
                throw ex;
            }
        }

        public void SyncProvGroupsTMS2SYOGroup(int liCodTMS, DirectoryEntry loGroup)
        {
            try
            {
                //Guarda el Folder
                int liCodFolder = SYOProvGroupSave(liCodTMS, loGroup);

                if (liCodFolder != -1 && liCodFolder != int.MinValue)
                    plProvGroup.Add(liCodFolder.ToString());

                //Guarda los subfolders
                if (loGroup.Children != null)
                    foreach (DirectoryEntry loGrp in loGroup.Children)
                        SyncProvGroupsTMS2SYOGroup(liCodTMS, loGrp);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de grupos TMS->SYO.", ex);
            }
        }

        public void SyncProvGroupsTMS2SYODel(int liCodTMS)
        {
            try
            {
                //Borra folders
                DataTable ldtFol = kdb.GetHisRegByEnt("TMSGroup", "Grupo Provisioning",
                    new string[] { "iCodRegistro", "iCodCatalogo" },
                    "{ServidorTMS} = " + liCodTMS +
                    (plProvGroup.Count > 0 ? " and iCodCatalogo not in (" + string.Join(",", plProvGroup.ToArray()) + ")" : ""));

                if (ldtFol != null)
                {
                    foreach (DataRow ldrFol in ldtFol.Rows)
                    {
                        KDBUtil.DeleteHistoric((int)ldrFol["iCodRegistro"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la baja de grupos de provisioning TMS->SYO.", ex);
            }
        }
        #endregion

        #region Sync Systems
        public void SyncSystems(int liCodTMS)
        {
            try
            {
                ManagementService loRemoteSetup = TMSGetManagementWS(liCodTMS);
                SyncSystemsTMS2SYO(liCodTMS, loRemoteSetup);
                //SyncSystemsTMS2SYODel(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de usuarios.", ex);
            }
        }

        public void SyncSystemsTMS2SYO(int liCodTMS, ManagementService loManagement)
        {
            try
            {
                plSystem.Clear();
                plSystemFolder.Clear();

                FolderTree laTMSSys = loManagement.GetFolderTree(true);

                if (laTMSSys.Root != null)
                    SyncSystemsTMS2SYOFolder(liCodTMS, laTMSSys.Root);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de conferencias TMS->SYO.", ex);
                throw ex;
            }
        }

        public void SyncSystemsTMS2SYOFolder(int liCodTMS, Folder loTMSFolder)
        {
            int liCodSystem;

            try
            {
                //Guarda el Folder
                int liCodFolder = SYOFolderSave(liCodTMS, loTMSFolder);

                if (liCodFolder != -1 && liCodFolder != int.MinValue)
                    plSystemFolder.Add(liCodFolder.ToString());

                //Guarda los sistemas del Folder
                if (loTMSFolder.SystemsInFolder != null)
                    foreach (ManagedSystem loTMSSys in loTMSFolder.SystemsInFolder)
                    {
                        liCodSystem = SYOSystemSave(liCodTMS, loTMSSys, liCodFolder);

                        if (liCodSystem != -1 && liCodSystem != int.MinValue)
                            plSystem.Add(liCodSystem.ToString());
                    }

                //Guarda los subfolders
                if (loTMSFolder.Children != null)
                    foreach (Folder loTMSFol in loTMSFolder.Children)
                        SyncSystemsTMS2SYOFolder(liCodTMS, loTMSFol);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de grupos TMS->SYO.", ex);
            }
        }

        public void SyncSystemsTMS2SYODel(int liCodTMS)
        {
            try
            {
                //Borra sistemas
                string[] laMae = { "Gateway", "Gatekeeper", "MCU", "Room", "Equipment", "Endpoint" };

                foreach (string lsMae in laMae)
                {
                    DataTable ldtSys = kdb.GetHisRegByEnt("TMSSystems", lsMae,
                        new string[] { "iCodRegistro", "iCodCatalogo" },
                        "{ServidorTMS} = " + liCodTMS +
                        (plSystem.Count > 0 ? " and iCodCatalogo not in (" + string.Join(",", plSystem.ToArray()) + ")" : ""));

                    if (ldtSys != null)
                        foreach (DataRow ldrSys in ldtSys.Rows)
                            KDBUtil.DeleteHistoric((int)ldrSys["iCodRegistro"]);
                }

                //Borra folders
                DataTable ldtFol = kdb.GetHisRegByEnt("TMSGroup", "Grupo TMS",
                    new string[] { "iCodRegistro", "iCodCatalogo" },
                    "{ServidorTMS} = " + liCodTMS +
                    (plSystemFolder.Count > 0 ? " and iCodCatalogo not in (" + string.Join(",", plSystemFolder.ToArray()) + ")" : ""));

                if (ldtFol != null)
                    foreach (DataRow ldrFol in ldtFol.Rows)
                        KDBUtil.DeleteHistoric((int)ldrFol["iCodRegistro"]);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la baja de sistemas TMS->SYO.", ex);
            }
        }
        #endregion

        #region SYO Grupos de Provisioning
        public int SYOProvGroupSave(int liCodTMS, DirectoryEntry loGrp)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodGrp = int.MinValue;

            try
            {
                if (loGrp.Name.ToUpper().Substring(0, 3) == "OU=")
                {
                    SearchResult s = LDAPGetEntry(liCodTMS, loGrp.Path.Substring(loGrp.Path.LastIndexOf("/") + 1), new string[] { "ou", "displayName" });
                    UTF8Encoding enc = new UTF8Encoding();

                    //Busca si existe la carpeta
                    DataRow ldrFol = KDBUtil.SearchHistoricRow("TMSGroup", "Grupo Provisioning",
                        new string[] { "iCodRegistro", "iCodCatalogo", "vchDescripcion", "{ServidorTMS}", 
                        "{TMSGroup}", "{Client}", "{Emple}", "{DireccionLDAP}" },
                        "{ServidorTMS} = " + liCodTMS + " and {DireccionLDAP} = '" + loGrp.Path.Substring(loGrp.Path.LastIndexOf("/") + 1) + "'");

                    //Busca si existe la carpeta padre
                    DataRow ldrFolP = KDBUtil.SearchHistoricRow("TMSGroup", "Grupo Provisioning",
                        new string[] { "iCodCatalogo", "{Client}", "{Emple}" },
                        "{ServidorTMS} = " + liCodTMS + " and {DireccionLDAP} = '" + loGrp.Parent.Path.Substring(loGrp.Parent.Path.LastIndexOf("/") + 1) + "'");

                    //Graba la conferencia
                    lhtVal = new Hashtable();
                    if (ldrFol == null)
                        lsCodigo = "ProvGrp " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    else
                    {
                        lhtVal.Add("iCodRegistro", (int)ldrFol["iCodRegistro"]);
                        liCodGrp = (int)ldrFol["iCodCatalogo"];
                        lsCodigo = (string)ldrFol["vchCodigo"];
                    }

                    HTAddIfDiferent(ldrFol, lhtVal, "vchDescripcion", Util.IsDBNull(enc.GetString((byte[])s.Properties["displayName"][0]), loGrp.InvokeGet("ou")));
                    HTAddIfDiferent(ldrFol, lhtVal, "{DireccionLDAP}", loGrp.Path.Substring(loGrp.Path.LastIndexOf("/") + 1));
                    HTAddIfDiferent(ldrFol, lhtVal, "{ServidorTMS}", liCodTMS);

                    //Grupo padre
                    if (ldrFolP != null)
                        HTAddIfDiferent(ldrFol, lhtVal, "{TMSGroup}", ldrFolP["iCodCatalogo"]);

                    //Si no tiene cliente, toma el del padre
                    if (ldrFol != null && (int)Util.IsDBNull(ldrFol["{Client}"], int.MinValue) == int.MinValue &&
                        ldrFolP != null && (int)Util.IsDBNull(ldrFolP["{Client}"], int.MinValue) != int.MinValue)
                        HTAddIfDiferent(ldrFol, lhtVal, "{Client}", ldrFolP["{Client}"]);

                    //Si no tiene empleado, toma el del padre
                    if (ldrFol != null && (int)Util.IsDBNull(ldrFol["{Emple}"], int.MinValue) == int.MinValue &&
                        ldrFolP != null && (int)Util.IsDBNull(ldrFolP["{Emple}"], int.MinValue) != int.MinValue)
                        HTAddIfDiferent(ldrFol, lhtVal, "{Emple}", ldrFolP["{Emple}"]);

                    if ((!lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 0) ||
                        (lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 1))
                        liCodGrp = KDBUtil.SaveHistoric("TMSGroup", "Grupo Provisioning", lsCodigo, null, lhtVal);
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar el grupo '" + Util.IsDBNull(loGrp.InvokeGet("displayName"), loGrp.InvokeGet("ou")) + " " + loGrp.Path + "' en SYO.", ex);
            }

            return liCodGrp;
        }
        #endregion

        #region SYO Systems
        /// <summary>
        /// Método para guardar un Sistema del TMS en SYO
        /// </summary>
        /// <param name="liCodTMS">iCodCatalogo del Servidor TMS que se está procesando</param>
        /// <param name="loTMSSys">Sistema TMS</param>
        /// <param name="liCodFolder">iCodCatalogo del folder TMS donde se guardará el sistema</param>
        /// <returns></returns>
        public int SYOSystemSave(int liCodTMS, ManagedSystem loTMSSys, int liCodFolder)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodSys = int.MinValue;
            string lsMae = "";
            DataRow ldrGroup = null;

            try
            {
                // Se obtiene el tipo de sistema con el que se está trabajando para inicializar
                // correctamente el maestro del histórico que se generará
                if (loTMSSys.SystemCategory == SeeYouOnServiceBL.TMSManagement.SystemCategory.Gateway ||
                    loTMSSys.SystemCategory == SeeYouOnServiceBL.TMSManagement.SystemCategory.Gatekeeper ||
                    loTMSSys.SystemCategory == SeeYouOnServiceBL.TMSManagement.SystemCategory.MCU ||
                    loTMSSys.SystemCategory == SeeYouOnServiceBL.TMSManagement.SystemCategory.Room ||
                    loTMSSys.SystemCategory == SeeYouOnServiceBL.TMSManagement.SystemCategory.Equipment)
                    lsMae = loTMSSys.SystemCategory.ToString();
                else if (loTMSSys.SystemCategory == SeeYouOnServiceBL.TMSManagement.SystemCategory.EndPoint)
                    lsMae = "Endpoint";

                // Busca si existe el sistema mediante el Id del sistema
                DataRow ldrSys = KDBUtil.SearchHistoricRow("TMSSystems", lsMae,
                    new string[] { "iCodRegistro", "iCodCatalogo", "iCodMaestro", "vchDescripcion",
                        "{ServidorTMS}", "{TMSSysId}", "{TMSGroup}", "{Emple}" },
                    "{ServidorTMS} = " + liCodTMS + " and {TMSSysId} = " + loTMSSys.Id);

                // Si no encuentra un sistema en base al ID, lo buscamos en base a su descripción y grupo TMS
                if (ldrSys == null)
                {
                    ldrSys = KDBUtil.SearchHistoricRow("TMSSystems", lsMae,
                        new string[] { "iCodRegistro", "iCodCatalogo", "iCodMaestro", "vchDescripcion",
                        "{ServidorTMS}", "{TMSSysId}", "{TMSGroup}", "{Emple}" },
                        "{ServidorTMS} = " + liCodTMS + " and vchDescripcion = '" + loTMSSys.Name + "' " +
                        "and {TMSGroup} = " + liCodFolder);
                }

                //Graba el sistema
                lhtVal = new Hashtable();

                if (ldrSys == null)
                    lsCodigo = "Sys " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                else
                {
                    liCodSys = (int)ldrSys["iCodCatalogo"];
                    lsCodigo = (string)ldrSys["vchCodigo"];
                    lhtVal.Add("iCodRegistro", (int)ldrSys["iCodRegistro"]);
                    //lsMae = (string)Util.IsDBNull(DSODataAccess.ExecuteScalar("select vchDescripcion from maestros where iCodRegistro = " + ldrSys["iCodMaestro"]), ""); //TODO: optimizar

                    //busca el empleado del del grupo
                    ldrGroup = KDBUtil.SearchHistoricRow("TMSGroup", liCodFolder, new string[] { "{Emple}" });
                }

                if (!String.IsNullOrEmpty(lsMae))
                {
                    // Agregamos la información en el hash que se grabará
                    HTAddIfDiferent(ldrSys, lhtVal, "vchDescripcion", loTMSSys.Name);
                    HTAddIfDiferent(ldrSys, lhtVal, "{ServidorTMS}", liCodTMS);
                    HTAddIfDiferent(ldrSys, lhtVal, "{TMSSysId}", loTMSSys.Id, Type.GetType("System.Int32"));
                    HTAddIfDiferent(ldrSys, lhtVal, "{TMSGroup}", liCodFolder);

                    if (ldrSys != null && ((int)Util.IsDBNull(ldrSys["{Emple}"], int.MinValue)) == int.MinValue &&
                        ldrGroup != null && ((int)Util.IsDBNull(ldrGroup["{Emple}"], int.MinValue)) != int.MinValue)
                        HTAddIfDiferent(ldrSys, lhtVal, "{Emple}", ldrGroup["{Emple}"]);

                    if ((!lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 0) ||
                        (lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 1))
                        liCodSys = KDBUtil.SaveHistoric("TMSSystems", lsMae, lsCodigo, null, lhtVal);

                    // Si el registro se grabó, grabamos también sus direcciones
                    if (liCodSys != -1 && liCodSys != int.MinValue)
                    {
                        int liCodSysAddr;
                        plSystemAddress.Clear();

                        // Se graba la dirección con protocolo SIP
                        liCodSysAddr = SYOSystemAddressSave(liCodSys, loTMSSys.SIPUri, "SIP");
                        if (liCodSysAddr != -1 && liCodSysAddr != int.MinValue)
                            plSystemAddress.Add(liCodSysAddr.ToString());

                        // Se graba la dirección con protocolo H.323
                        liCodSysAddr = SYOSystemAddressSave(liCodSys, loTMSSys.H323Id, "H.323");
                        if (liCodSysAddr != -1 && liCodSysAddr != int.MinValue)
                            plSystemAddress.Add(liCodSysAddr.ToString());

                        //SYOSystemAddressSave(liCodTMS, liCodSys, loTMSSys.E164Alias);

                        //Borra system addres
                        DataTable ldtFol = kdb.GetHisRegByEnt("TMSSystemAddress", "TMSSystemAddress",
                            new string[] { "iCodRegistro", "iCodCatalogo" },
                            "{TMSSystems} = " + liCodSys +
                            (plSystemAddress.Count > 0 ? " and iCodCatalogo not in (" + string.Join(",", plSystemAddress.ToArray()) + ")" : ""));

                        if (ldtFol != null)
                            foreach (DataRow ldrFol in ldtFol.Rows)
                                KDBUtil.DeleteHistoric((int)ldrFol["iCodRegistro"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar el sistema TMS '" + loTMSSys.Id + " " + loTMSSys.Name + "' en SYO.", ex);
            }

            return liCodSys;
        }

        /// <summary>
        /// Método para guardar una dirección en SYO y ligarla al Sistema que fue creado
        /// </summary>
        /// <param name="liCodSys">iCodCatalogo del Sistema cuya dirección será grabada</param>
        /// <param name="lsSysAddr">Dirección del sistema</param>
        /// <param name="lsCodProtocol">Protocolo de la dirección</param>
        /// <returns>iCodCatalogo del registro que liga una Dirección con un Sistema.</returns>
        public int SYOSystemAddressSave(int liCodSys, string lsSysAddr, string lsCodProtocol)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodAddr = int.MinValue;
            int liCodSysAddr = int.MinValue;

            // Validación para la dirección
            if (!string.IsNullOrEmpty(lsSysAddr))
            {
                try
                {
                    // Grabamos la dirección
                    liCodAddr = SYOAddressSave(lsSysAddr, lsCodProtocol);

                    // Busca si ya existe un registro que ligue a la dirección con el sistema
                    DataRow ldrSysAddr = KDBUtil.SearchHistoricRow("TMSSystemAddress", "TMSSystemAddress",
                        new string[] { "iCodRegistro", "iCodCatalogo", "{TMSSystems}", "{Address}" },
                        "{TMSSystems} = " + liCodSys + " and {Address} = " + liCodAddr);

                    //Graba el system address 
                    lhtVal = new Hashtable();
                    // Si no se encontró ya un registro System-Address, se creará uno nuevo
                    if (ldrSysAddr == null)
                        lsCodigo = "SysAddr " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    else
                    {
                        liCodSysAddr = (int)ldrSysAddr["iCodCatalogo"];
                        lhtVal.Add("iCodRegistro", (int)ldrSysAddr["iCodRegistro"]);
                        lsCodigo = (string)ldrSysAddr["vchCodigo"];
                    }

                    HTAddIfDiferent(ldrSysAddr, lhtVal, "{TMSSystems}", liCodSys);
                    HTAddIfDiferent(ldrSysAddr, lhtVal, "{Address}", liCodAddr);

                    if ((!lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 0) ||
                        (lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 1))
                        liCodSysAddr = KDBUtil.SaveHistoric("TMSSystemAddress", "TMSSystemAddress", lsCodigo, null, lhtVal);
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al grabar el sistem address TMS '" + liCodSys + " " + lsCodProtocol + " " + lsSysAddr + "' en SYO.", ex);
                }
            }

            return liCodSysAddr;
        }

        public int SYOFolderSave(int liCodTMS, Folder loTMSFol)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodGrp = int.MinValue;

            try
            {
                //Busca si existe la carpeta
                DataRow ldrFol = KDBUtil.SearchHistoricRow("TMSGroup", "Grupo TMS",
                    new string[] { "iCodRegistro", "iCodCatalogo", "vchDescripcion", "{ServidorTMS}",
                        "{TMSFolderId}", "{TMSParentFolderId}", "{TMSGroup}", "{Client}", "{Emple}" },
                    "{ServidorTMS} = " + liCodTMS + " and {TMSFolderId} = " + loTMSFol.Id);

                //Busca si existe la carpeta padre
                DataRow ldrFolP = KDBUtil.SearchHistoricRow("TMSGroup", "Grupo TMS",
                    new string[] { "iCodCatalogo", "{Client}", "{Emple}" },
                    "{ServidorTMS} = " + liCodTMS + " and {TMSFolderId} = " + loTMSFol.ParentId);

                //Graba la conferencia
                lhtVal = new Hashtable();
                if (ldrFol == null)
                    lsCodigo = "Fol " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                else
                {
                    lhtVal.Add("iCodRegistro", (int)ldrFol["iCodRegistro"]);
                    liCodGrp = (int)ldrFol["iCodCatalogo"];
                    lsCodigo = (string)ldrFol["vchCodigo"];
                }

                HTAddIfDiferent(ldrFol, lhtVal, "vchDescripcion", loTMSFol.Name);
                HTAddIfDiferent(ldrFol, lhtVal, "{ServidorTMS}", liCodTMS);
                HTAddIfDiferent(ldrFol, lhtVal, "{TMSFolderId}", loTMSFol.Id, Type.GetType("System.Int32"));
                HTAddIfDiferent(ldrFol, lhtVal, "{TMSParentFolderId}", loTMSFol.ParentId, Type.GetType("System.Int32"));

                //Grupo padre
                if (ldrFolP != null)
                    HTAddIfDiferent(ldrFol, lhtVal, "{TMSGroup}", ldrFolP["iCodCatalogo"]);

                //Si no tiene cliente, toma el del padre
                if (ldrFol != null && (int)Util.IsDBNull(ldrFol["{Client}"], int.MinValue) == int.MinValue &&
                    ldrFolP != null && (int)Util.IsDBNull(ldrFolP["{Client}"], int.MinValue) != int.MinValue)
                    HTAddIfDiferent(ldrFol, lhtVal, "{Client}", ldrFolP["{Client}"]);

                //Si no tiene empleado, toma el del padre
                if (ldrFol != null && (int)Util.IsDBNull(ldrFol["{Emple}"], int.MinValue) == int.MinValue &&
                    ldrFolP != null && (int)Util.IsDBNull(ldrFolP["{Emple}"], int.MinValue) != int.MinValue)
                    HTAddIfDiferent(ldrFol, lhtVal, "{Emple}", ldrFolP["{Emple}"]);

                if ((!lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 0) ||
                    (lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 1))
                    liCodGrp = KDBUtil.SaveHistoric("TMSGroup", "Grupo TMS", lsCodigo, null, lhtVal);
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar el grupo '" + loTMSFol.Id + " " + loTMSFol.Name + "' en SYO.", ex);
            }

            return liCodGrp;
        }
        #endregion

        #region Sync PhoneBooks
        public void SyncPhoneBooks(int liCodTMS)
        {
            try
            {
                PhoneBookService loPhoneBook = TMSGetPhoneBookWS(liCodTMS);
                SyncPhoneBookTMS2SYO(liCodTMS, loPhoneBook);
                //SyncPhoneBookTMS2SYODel(liCodTMS);

            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de usuarios.", ex);
            }
        }

        public void SyncPhoneBookTMS2SYO(int liCodTMS, PhoneBookService loPhoneBook)
        {
            try
            {
                plContact.Clear();
                plContactFolder.Clear();

                PhoneBook[] laTMSPhBk = loPhoneBook.GetPhoneBooks();

                if (laTMSPhBk != null)

                    foreach (PhoneBook loTMSPhBk in laTMSPhBk)
                        SyncPhoneBookTMS2SYO(liCodTMS, loPhoneBook, loTMSPhBk);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de PhoneBooks TMS->SYO.", ex);
                throw ex;
            }
        }

        public void SyncPhoneBookTMS2SYO(int liCodTMS, PhoneBookService loPhoneBook, PhoneBook loTMSPhBk)
        {
            int liCodContact;

            try
            {
                int liCodPhoneBook = SYOPhoneBookSave(liCodTMS, loTMSPhBk);

                if (liCodPhoneBook != -1 && liCodPhoneBook != int.MinValue)
                    plContactFolder.Add(liCodPhoneBook.ToString());

                PhoneBookToSourceBinding[] laTMSPhBkSrcs = loPhoneBook.GetSourcesForPhoneBook(loTMSPhBk.Id);

                if (laTMSPhBkSrcs != null)
                    foreach (PhoneBookToSourceBinding loTMSPhBkSrc in laTMSPhBkSrcs)
                    {
                        PhoneBookContact[] laTMSContacts = loPhoneBook.GetPhoneBookSourceContacts(loTMSPhBkSrc.PhoneBookSourceId, 10000, 0); //TODO: revisar maxresults

                        if (laTMSContacts != null)
                            foreach (PhoneBookContact loTMSContact in laTMSContacts)
                            {
                                liCodContact = SYOContactSave(liCodTMS, loTMSContact, liCodPhoneBook);

                                if (liCodContact != -1 && liCodContact != int.MinValue)
                                    plContact.Add(liCodContact.ToString());
                            }
                    }
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de grupos TMS->SYO.", ex);
            }
        }

        public void SyncPhoneBookTMS2SYODel(int liCodTMS)
        {
            try
            {
                //Borra contactos
                DataTable ldtPhCt = kdb.GetHisRegByEnt("TMSPhoneBookContact", "PhoneBook",
                    new string[] { "iCodRegistro", "iCodCatalogo" },
                    "{ServidorTMS} = " + liCodTMS +
                    (plContact.Count > 0 ? " and iCodCatalogo not in (" + string.Join(",", plContact.ToArray()) + ")" : ""));

                if (ldtPhCt != null)
                    foreach (DataRow ldrPhCt in ldtPhCt.Rows)
                        KDBUtil.DeleteHistoric((int)ldrPhCt["iCodRegistro"]);

                //Borra folders
                DataTable ldtFol = kdb.GetHisRegByEnt("TMSPhoneBookFolder", "TMS Phone Book Folder",
                    new string[] { "iCodRegistro", "iCodCatalogo" },
                    "{ServidorTMS} = " + liCodTMS +
                    (plContactFolder.Count > 0 ? " and iCodCatalogo not in (" + string.Join(",", plContactFolder.ToArray()) + ")" : ""));

                if (ldtFol != null)
                    foreach (DataRow ldrFol in ldtFol.Rows)
                        KDBUtil.DeleteHistoric((int)ldrFol["iCodRegistro"]);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la baja de contactos TMS->SYO.", ex);
            }
        }
        #endregion

        #region SYO PhoneBooks
        public int SYOPhoneBookSave(int liCodTMS, PhoneBook loTMSPhBk)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodPhBk = int.MinValue;

            try
            {
                //Busca si existe la carpeta
                DataRow ldrFol = KDBUtil.SearchHistoricRow("TMSPhoneBookFolder", "TMS Phone Book Folder",
                    new string[] { "iCodRegistro", "iCodCatalogo", "vchDescripcion", "{ServidorTMS}", 
                        "{TMSFolderId}", "{TMSParentFolderId}", "{Client}", "{Emple}" },
                    "{ServidorTMS} = " + liCodTMS + " and {TMSFolderId} = " + loTMSPhBk.Id);

                //Busca si existe la carpeta padre
                DataRow ldrFolP = KDBUtil.SearchHistoricRow("TMSPhoneBookFolder", "TMS Phone Book Folder",
                    new string[] { "iCodCatalogo", "{Client}", "{Emple}" },
                    "{ServidorTMS} = " + liCodTMS + " and {TMSFolderId} = " + loTMSPhBk.ParentId);

                //Graba la conferencia
                lhtVal = new Hashtable();

                if (ldrFol == null)
                    lsCodigo = "PhBk " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                else
                {
                    liCodPhBk = (int)ldrFol["iCodCatalogo"];
                    lsCodigo = (string)ldrFol["vchCodigo"];
                    lhtVal.Add("iCodRegistro", (int)ldrFol["iCodRegistro"]);
                }

                HTAddIfDiferent(ldrFol, lhtVal, "vchDescripcion", loTMSPhBk.Name);
                HTAddIfDiferent(ldrFol, lhtVal, "{ServidorTMS}", liCodTMS);
                HTAddIfDiferent(ldrFol, lhtVal, "{TMSFolderId}", loTMSPhBk.Id);
                HTAddIfDiferent(ldrFol, lhtVal, "{TMSParentFolderId}", loTMSPhBk.ParentId);

                //Grupo padre
                if (ldrFolP != null)
                    lhtVal.Add("{TMSPhoneBookFolder}", ldrFolP["iCodCatalogo"]);

                //Si no tiene cliente, toma el del padre
                if (ldrFol != null && (int)Util.IsDBNull(ldrFol["{Client}"], int.MinValue) == int.MinValue &&
                    ldrFolP != null && (int)Util.IsDBNull(ldrFolP["{Client}"], int.MinValue) != int.MinValue)
                    lhtVal.Add("{Client}", ldrFolP["{Client}"]);

                //Si no tiene empleado, toma el del padre
                if (ldrFol != null && (int)Util.IsDBNull(ldrFol["{Emple}"], int.MinValue) == int.MinValue &&
                    ldrFolP != null && (int)Util.IsDBNull(ldrFolP["{Emple}"], int.MinValue) != int.MinValue)
                    lhtVal.Add("{Emple}", ldrFolP["{Emple}"]);

                if ((!lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 0) ||
                    (lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 1))
                    liCodPhBk = KDBUtil.SaveHistoric("TMSPhoneBookFolder", "TMS Phone Book Folder", lsCodigo, null, lhtVal);
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar el Phone Book Folder '" + loTMSPhBk.Id + " " + loTMSPhBk.Name + "' en SYO.", ex);
            }

            return liCodPhBk;
        }

        public int SYOContactSave(int liCodTMS, PhoneBookContact loTMSContact, int liCodPhoneBook)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodPhCt = int.MinValue;

            try
            {
                string lsDatosContacto = "[" + loTMSContact.Name + ", SysId: " + loTMSContact.Id + "]";

                // Busca si existe el PhoneBook
                DataRow ldrSys = KDBUtil.SearchHistoricRow("TMSPhoneBookContact", "PhoneBook",
                    new string[] { "iCodRegistro", "iCodCatalogo", "vchDescripcion", "iCodMaestro",
                        "{ServidorTMS}", "{TMSSysId}", "{TMSPhoneBookFolder}" },
                    "{ServidorTMS} = " + liCodTMS + " and {TMSSysId} = " + loTMSContact.Id);


                // Si no encuentra un PhoneBook en base al ID, lo buscamos en base a su descripción y 
                // PhoneBookFolder
                if (ldrSys == null)
                {
                    ldrSys = KDBUtil.SearchHistoricRow("TMSPhoneBookContact", "PhoneBook",
                        new string[] { "iCodRegistro", "iCodCatalogo", "iCodMaestro", "vchDescripcion",
                        "{ServidorTMS}", "{TMSSysId}", "{TMSPhoneBookFolder}" },
                        "{ServidorTMS} = " + liCodTMS + " and vchDescripcion = '" + loTMSContact.Name + "' " +
                        "and {TMSPhoneBookFolder} = " + liCodPhoneBook);
                }

                // Graba el PhoneBook Contact
                lhtVal = new Hashtable();

                if (ldrSys == null)
                    lsCodigo = "PhCt " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                else
                {
                    liCodPhCt = (int)ldrSys["iCodCatalogo"];
                    lhtVal.Add("iCodRegistro", (int)ldrSys["iCodRegistro"]);
                    lsCodigo = (string)ldrSys["vchCodigo"];
                }

                HTAddIfDiferent(ldrSys, lhtVal, "vchDescripcion", loTMSContact.Name);
                HTAddIfDiferent(ldrSys, lhtVal, "{ServidorTMS}", liCodTMS);
                HTAddIfDiferent(ldrSys, lhtVal, "{TMSSysId}", loTMSContact.Id);
                HTAddIfDiferent(ldrSys, lhtVal, "{TMSPhoneBookFolder}", liCodPhoneBook);

                if ((!lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 0) ||
                    (lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 1))
                    liCodPhCt = KDBUtil.SaveHistoric("TMSPhoneBookContact", "PhoneBook", lsCodigo, null, lhtVal);

                if (liCodPhCt != -1)
                {
                    int liCodPhBkAddr;
                    plContactAddress.Clear();

                    foreach (PhoneBookContactMethod loMethod in loTMSContact.ContactMethods)
                        if (loMethod.Type == PhoneBookContactMethodType.SIP || loMethod.Type == PhoneBookContactMethodType.H323)
                        {
                            liCodPhBkAddr = SYOPhoneBookAddressSave(liCodPhCt, loMethod.FQDialString,
                                loMethod.Type == PhoneBookContactMethodType.SIP ? "SIP" :
                                loMethod.Type == PhoneBookContactMethodType.H323 ? "H.323" : "");

                            if (liCodPhBkAddr != -1 && liCodPhBkAddr != int.MinValue)
                                plContactAddress.Add(liCodPhBkAddr.ToString());
                        }

                    //Borra system addres
                    DataTable ldtFol = kdb.GetHisRegByEnt("PhoneBookAddress", "PhoneBookAddress",
                        new string[] { "iCodRegistro", "iCodCatalogo" },
                        "{TMSPhoneBookContact} = " + liCodPhCt +
                        (plContactAddress.Count > 0 ? " and iCodCatalogo not in (" + string.Join(",", plContactAddress.ToArray()) + ")" : ""));

                    if (ldtFol != null)
                        foreach (DataRow ldrFol in ldtFol.Rows)
                            KDBUtil.DeleteHistoric((int)ldrFol["iCodRegistro"]);
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar el Phone Book Contact '" + loTMSContact.Id + " " + loTMSContact.Name + "' en SYO.", ex);
            }

            return liCodPhCt;
        }

        public int SYOPhoneBookAddressSave(int liCodPhCt, string lsPhBkAddr, string lsCodProtocol)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodAddr = int.MinValue;
            int liCodPhBkAddr = int.MinValue;

            if (!string.IsNullOrEmpty(lsPhBkAddr))
            {
                try
                {
                    liCodAddr = SYOAddressSave(lsPhBkAddr, lsCodProtocol);

                    //Busca si existe el system address
                    DataRow ldrSysAddr = KDBUtil.SearchHistoricRow("PhoneBookAddress", "PhoneBookAddress",
                        new string[] { "iCodRegistro", "iCodCatalogo", "{TMSPhoneBookContact}", "{Address}" },
                        "{TMSPhoneBookContact} = " + liCodPhCt + " and {Address} = " + liCodAddr);

                    //Graba el system address
                    lhtVal = new Hashtable();

                    if (ldrSysAddr == null)
                        lsCodigo = "PhBkAddr " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    else
                    {
                        liCodPhBkAddr = (int)ldrSysAddr["iCodCatalogo"];
                        lhtVal.Add("iCodRegistro", (int)ldrSysAddr["iCodRegistro"]);
                        lsCodigo = (string)ldrSysAddr["vchCodigo"];
                    }

                    HTAddIfDiferent(ldrSysAddr, lhtVal, "{TMSPhoneBookContact}", liCodPhCt);
                    HTAddIfDiferent(ldrSysAddr, lhtVal, "{Address}", liCodAddr);

                    if ((!lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 0) ||
                        (lhtVal.ContainsKey("iCodRegistro") && lhtVal.Count > 1))
                        liCodPhBkAddr = KDBUtil.SaveHistoric("PhoneBookAddress", "PhoneBookAddress", lsCodigo, null, lhtVal);
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al grabar el contact address TMS '" + liCodPhBkAddr + " " + lsCodProtocol + " " + lsPhBkAddr + "' en SYO.", ex);
                }
            }

            return liCodPhBkAddr;
        }
        #endregion

        /// <summary>
        /// Método para guardar una dirección en SeeYouOn
        /// </summary>
        /// <param name="lsSysAddr">Dirección</param>
        /// <param name="lsCodProtocol">Protocolo de la dirección</param>
        /// <returns>iCodCatalogo de la dirección creada</returns>
        protected int SYOAddressSave(string lsSysAddr, string lsCodProtocol)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodAddr = int.MinValue;

            // Validación a la dirección
            if (!string.IsNullOrEmpty(lsSysAddr))
            {
                try
                {
                    // Obtener iCodCatalogo de la dirección que se guardará
                    int liCodProtocol = KDBUtil.SearchICodCatalogo("MCUCallProtocol", lsCodProtocol, true);

                    //Busca si existe el address
                    DataRow ldrAddr = KDBUtil.SearchHistoricRow("Address", "Address",
                        new string[] { "iCodRegistro", "iCodCatalogo" },
                        "{SystemAddress} = '" + lsSysAddr + "' and {MCUCallProtocol} = " + liCodProtocol);

                    // Graba la dirección si no se encontró
                    lhtVal = new Hashtable();
                    if (ldrAddr == null)
                    {
                        lsCodigo = "Addr " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        lhtVal.Clear();
                        lhtVal.Add("vchDescripcion", lsCodProtocol + ": " + lsSysAddr);
                        lhtVal.Add("{MCUCallProtocol}", liCodProtocol);
                        lhtVal.Add("{SystemAddress}", lsSysAddr);

                        liCodAddr = KDBUtil.SaveHistoric("Address", "Address", lsCodigo, null, lhtVal);
                    }
                    else
                        liCodAddr = (int)ldrAddr["iCodCatalogo"];
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al grabar el address TMS " + lsCodProtocol + " " + lsSysAddr + "' en SYO.", ex);
                }
            }

            return liCodAddr;
        }

        #region Sync Conferences
        public void SyncConferences(int liCodTMS)
        {
            try
            {
                BookingService loBooking = TMSGetBookingWS(liCodTMS);

                SyncConferencesTMS2SYO(liCodTMS, loBooking);
                //SyncConferencesSYO2TMS(liCodTMS, loBooking);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de conferencias.", ex);
            }
        }

        public void SyncConferencesTMS2SYO(int liCodTMS, BookingService loBooking)
        {
            try
            {
                int liLastTrId = SYOGetLastTrId();

                Transaction[] laTMSTrs = loBooking.GetTransactionsSince(liLastTrId);
                Conference loTMSConf = null;

                foreach (Transaction loTr in laTMSTrs)
                {
                    loTMSConf = loBooking.GetConferenceById(loTr.ConferenceId);

                    if (loTr.TransType == TransactionType.New || loTr.TransType == TransactionType.Updated)
                        SYOConfSave(liCodTMS, loTMSConf, loTr.TransactionId);
                    else if (loTr.TransType == TransactionType.Deleted)
                        SYOConfDelete(liCodTMS, loTMSConf, loTr.TransactionId);
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de conferencias TMS->SYO.", ex);
            }
        }

        public void SyncConferencesSYO2TMS(int liCodTMS, BookingService loBooking)
        {
            try
            {
                DataTable ldt = kdb.GetHisRegByEnt("TMSConf", "Conferencia",
                new string[] { "iCodRegistro", "iCodCatalogo", "vchDescripcion", "{FechaInicioReservacion}", "{FechaFinReservacion}" },
                "{ServidorTMS} = " + liCodTMS + " and {ConferenceId} is null");

                if (ldt != null)
                    foreach (DataRow ldr in ldt.Rows)
                        TMSConfSave(loBooking, ldr);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de conferencias SYO->TMS.", ex);
            }
        }
        #endregion

        #region SYO Conferences
        public int SYOConfSave(int liCodTMS, Conference loTMSConf, int liTrId)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liCodConf = int.MinValue;
            int liCodConfPart = int.MinValue;

            DataRow[] laRows = null;

            try
            {
                //Busca si existe la conferencia
                DataRow ldrConf = KDBUtil.SearchHistoricRow("TMSConf", "Conferencia",
                    new string[] { "iCodRegistro", "iCodCatalogo" },
                    "{ServidorTMS} = " + liCodTMS + " and {ConferenceId} = " + loTMSConf.ConferenceId);

                //Graba la conferencia
                lhtVal = new Hashtable();

                if (ldrConf == null)
                {
                    lsCodigo = "Conf " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    lhtVal.Add("{EstConferencia}", KDBUtil.SearchScalar("EstConferencia", "Programada", "iCodCatalogo", true));
                }
                else
                {
                    lhtVal.Add("iCodRegistro", (int)ldrConf["iCodRegistro"]);
                    lsCodigo = (string)ldrConf["vchCodigo"];
                    lhtVal.Add("{EstConferencia}", KDBUtil.SearchScalar("EstConferencia", "ReProgramada", "iCodCatalogo", true));
                }

                lhtVal.Add("vchDescripcion", loTMSConf.Title);
                lhtVal.Add("{ServidorTMS}", liCodTMS);
                lhtVal.Add("{ConferenceId}", loTMSConf.ConferenceId);
                lhtVal.Add("{TMSTrId}", liTrId);
                lhtVal.Add("{FechaInicioReservacion}", DateTime.Parse(loTMSConf.StartTimeUTC)); //TODO: TimeZone
                lhtVal.Add("{FechaFinReservacion}", DateTime.Parse(loTMSConf.EndTimeUTC)); //TODO: TimeZone

                liCodConf = KDBUtil.SaveHistoric("TMSConf", "Conferencia", lsCodigo, null, lhtVal);

                //Busca participantes previamente grabados
                DataTable ldtConfPart = kdb.GetHisRegByEnt("Participante", "Participante",
                    new string[] { "iCodCatalogo", "iCodRegistro" },
                    "{TMSConf} = " + liCodConf);

                if (ldtConfPart == null)
                    throw new Exception("Error al buscar los participantes de la conferencia previamente guardados.");

                ldtConfPart.Columns.Add("saved", System.Type.GetType("System.Boolean"));

                //Graba los participantes
                if (loTMSConf.Participants != null)
                {
                    foreach (Participant loTMSPart in loTMSConf.Participants)
                    {
                        liCodConfPart = SYOConfPartSave(liCodTMS, liCodConf, loTMSPart);

                        if ((laRows = ldtConfPart.Select("iCodCatalogo = " + liCodConfPart)).Length > 0)
                            laRows[0]["saved"] = true;
                    }
                }

                //Borra los participantes previamente salvados que ya no están
                lhtVal = new Hashtable();
                lhtVal.Add("{EstParticipante}", KDBUtil.SearchScalar("EstParticipante", "Eliminado", "iCodCatalogo", true));

                foreach (DataRow ldr in ldtConfPart.Rows)
                {
                    if (!(bool)Util.IsDBNull(ldr["saved"], false))
                        KDBUtil.DeleteHistoric((int)ldr["iCodRegistro"], lhtVal);
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar la conferencia TMS '" + loTMSConf.ConferenceId + " " + loTMSConf.Title + "' en SYO.", ex);
            }

            return liCodConf;
        }

        public void SYOConfDelete(int liCodTMS, Conference loTMSConf, int liTrId)
        {
            try
            {
                DataTable ldt = kdb.GetHisRegByEnt("TMSConf", "Conferencia",
                    new string[] { "iCodRegistro" },
                    "{ServidorTMS} = " + liCodTMS + " and {ConferenceId} = " + loTMSConf.ConferenceId);

                if (ldt == null)
                    throw new Exception("Error al buscar la conferencia para borrarla.");

                foreach (DataRow ldr in ldt.Rows)
                    SYOConfDelete((int)ldr["iCodRegistro"]);
            }
            catch (Exception ex)
            {
                Util.LogException("Error al borrar la conferencia TMS '" + loTMSConf.ConferenceId + " " + loTMSConf.Title + "' en SYO.", ex);
            }
        }

        public void SYOConfDelete(int liCodRegistro)
        {
            SYOConfDelete(liCodRegistro, int.MinValue);
        }

        public void SYOConfDelete(int liCodRegistro, int liTrId)
        {
            Hashtable lhtVal = new Hashtable();
            lhtVal.Add("{TMSTrId}", liTrId);
            lhtVal.Add("dtFinVigencia", DateTime.Today);

            KDBUtil.DeleteHistoric(liCodRegistro, lhtVal);
        }

        public int SYOConfPartSave(int liCodTMS, int liCodConf, Participant loTMSPart)
        {
            Hashtable lhtVal;
            string lsCodigo = "";
            int liRet = int.MinValue;
            int liCodSys = int.MinValue;
            int liEstParticipante = int.MaxValue;
            try
            {
                if (loTMSPart.ParticipantCallType == ParticipantType.TMS)
                {
                    DataRow ldrSys = KDBUtil.SearchHistoricRow("TMSSystems", "",
                        new string[] { "iCodRegistro", "iCodCatalogo", "iCodMaestro", "{Emple}" },
                        "{ServidorTMS} = " + liCodTMS + " and {TMSSysId} = " + loTMSPart.ParticipantId);

                    if (ldrSys != null)
                        liCodSys = (int)ldrSys["iCodCatalogo"];
                }
                else
                {
                    DataRow ldrSys = KDBUtil.SearchHistoricRow("TMSSystemAddress", "TMSSystemAddress",
                        new string[] { "iCodRegistro", "iCodCatalogo", "{TMSSystems}" },
                        "{ServidorTMS} = " + liCodTMS + " and {SystemAddress} = '" + loTMSPart.NameOrNumber + "'");

                    if (ldrSys != null)
                        liCodSys = (int)ldrSys["{TMSSystems}"];
                }

                if (liCodSys == int.MinValue)
                    Util.LogMessage("No se encontró el sistema '" + loTMSPart.ParticipantId + ":" + loTMSPart.NameOrNumber + ":" + loTMSPart.ParticipantCallType + "' para agregarlo a la conferencia '" + liCodConf + "'.");
                else
                {
                    DataRow ldrConfPart = KDBUtil.SearchHistoricRow("Participante", "Participante",
                        new string[] { "iCodRegistro", "iCodCatalogo" },
                        "{TMSConf} = " + liCodConf + " and {TMSSystems} = " + liCodSys);

                    //Graba el participante
                    lhtVal = new Hashtable();
                    liEstParticipante = (int)Util.IsDBNull(KDBUtil.SearchScalar("EstParticipante", "Agregado",
                        "iCodCatalogo", true), 0);

                    if (ldrConfPart == null)
                    {
                        lsCodigo = "Conf " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else
                    {
                        lhtVal.Add("iCodRegistro", (int)ldrConfPart["iCodRegistro"]);
                        lsCodigo = (string)ldrConfPart["vchCodigo"];
                    }

                    lhtVal.Add("vchDescripcion", loTMSPart.ParticipantCallType.ToString());
                    lhtVal.Add("{TMSConf}", liCodConf);
                    lhtVal.Add("{TMSSystems}", liCodSys);
                    lhtVal.Add("{EstParticipante}", liEstParticipante);

                    liRet = KDBUtil.SaveHistoric("Participante", "Participante", lsCodigo, null, lhtVal);
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar el participante '" + loTMSPart.NameOrNumber + "' en la conferencia '" + liCodConf + "'.", ex);
            }

            return liRet;
        }

        public int SYOGetLastTrId()
        {
            Hashtable lhtCamposHis = kdb.CamposHis("TMSConf", "Conferencia");
            string lsQueryHis = kdb.GetQueryHis(lhtCamposHis, new string[] { "{TMSTrId}" }, "", "", "");

            return (int)DSODataAccess.ExecuteScalar("select isnull(max([{TMSTrId}]), 0) from (" + lsQueryHis + ") A");
        }
        #endregion

        #region TMS WebServices
        protected RemoteSetupService TMSGetRemoteSetupWS(int liCodTMS)
        {
            DataRow ldrTMS = KDBUtil.SearchHistoricRow("ServidorTMS", liCodTMS,
                new string[] { "{URLWSRemoteSetup}", "{UsuarioTMS}", "{Password}", "{Dominio}" });

            if (ldrTMS == null)
                throw new Exception("No se encontraron datos de configuración para el Servidor TMS '" + liCodTMS + "'");

            RemoteSetupService loRet = new RemoteSetupService();
            loRet.ExternalAPIVersionSoapHeaderValue = new SeeYouOnServiceBL.TMSRemoteSetup.ExternalAPIVersionSoapHeader();
            loRet.ExternalAPIVersionSoapHeaderValue.ClientVersionIn = 5;

            loRet.Url = (string)Util.IsDBNull(ldrTMS["{URLWSRemoteSetup}"], "");
            loRet.Credentials = new System.Net.NetworkCredential(
                (string)Util.IsDBNull(ldrTMS["{UsuarioTMS}"], ""),
                Util.Decrypt((string)Util.IsDBNull(ldrTMS["{Password}"], "")),
                (string)Util.IsDBNull(ldrTMS["{Dominio}"], ""));

            return loRet;
        }

        protected BookingService TMSGetBookingWS(int liCodTMS)
        {
            DataRow ldrTMS = KDBUtil.SearchHistoricRow("ServidorTMS", liCodTMS,
                new string[] { "{URLWSBooking}", "{UsuarioTMS}", "{Password}", "{Dominio}" });

            if (ldrTMS == null)
                throw new Exception("No se encontraron datos de configuración para el Servidor TMS '" + liCodTMS + "'");

            BookingService loRet = new BookingService();
            loRet.ExternalAPIVersionSoapHeaderValue = new SeeYouOnServiceBL.TMSBooking.ExternalAPIVersionSoapHeader();
            loRet.ExternalAPIVersionSoapHeaderValue.ClientVersionIn = 5;

            loRet.Url = (string)Util.IsDBNull(ldrTMS["{URLWSBooking}"], "");
            loRet.Credentials = new System.Net.NetworkCredential(
                (string)Util.IsDBNull(ldrTMS["{UsuarioTMS}"], ""),
                Util.Decrypt((string)Util.IsDBNull(ldrTMS["{Password}"], "")),
                (string)Util.IsDBNull(ldrTMS["{Dominio}"], ""));

            return loRet;
        }

        protected ManagementService TMSGetManagementWS(int liCodTMS)
        {
            DataRow ldrTMS = KDBUtil.SearchHistoricRow("ServidorTMS", liCodTMS,
                new string[] { "{URLWSManagement}", "{UsuarioTMS}", "{Password}", "{Dominio}" });

            if (ldrTMS == null)
                throw new Exception("No se encontraron datos de configuración para el Servidor TMS '" + liCodTMS + "'");

            ManagementService loRet = new ManagementService();

            loRet.Url = (string)Util.IsDBNull(ldrTMS["{URLWSManagement}"], "");
            loRet.Credentials = new System.Net.NetworkCredential(
                (string)Util.IsDBNull(ldrTMS["{UsuarioTMS}"], ""),
                Util.Decrypt((string)Util.IsDBNull(ldrTMS["{Password}"], "")),
                (string)Util.IsDBNull(ldrTMS["{Dominio}"], ""));

            return loRet;
        }

        protected PhoneBookService TMSGetPhoneBookWS(int liCodTMS)
        {
            DataRow ldrTMS = KDBUtil.SearchHistoricRow("ServidorTMS", liCodTMS,
                new string[] { "{URLWSPhoneBook}", "{UsuarioTMS}", "{Password}", "{Dominio}" });

            if (ldrTMS == null)
                throw new Exception("No se encontraron datos de configuración para el Servidor TMS '" + liCodTMS + "'");

            PhoneBookService loRet = new PhoneBookService();

            loRet.Url = (string)Util.IsDBNull(ldrTMS["{URLWSPhoneBook}"], "");
            loRet.Credentials = new System.Net.NetworkCredential(
                (string)Util.IsDBNull(ldrTMS["{UsuarioTMS}"], ""),
                Util.Decrypt((string)Util.IsDBNull(ldrTMS["{Password}"], "")),
                (string)Util.IsDBNull(ldrTMS["{Dominio}"], ""));

            //Util.LogMessage("Se usarán las credenciales:\r\nUsuario: " + (string)Util.IsDBNull(ldrTMS["{UsuarioTMS}"], "") +
            //    "\r\nPass: " + Util.Decrypt((string)Util.IsDBNull(ldrTMS["{Password}"], "")) +
            //    "\r\nDominio: " + (string)Util.IsDBNull(ldrTMS["{Dominio}"], ""));

            return loRet;
        }
        #endregion

        #region TMS Movi Accounts
        public void SyncMoviAccounts(int liCodTMS)
        {
            try
            {
                // 1.- Eliminar del ldap las cuentas movi que estén dadas de baja
                RemoveCtasMOVI();
                // 2.- Cargar las cuentas movi desde ldap
                GetCtasMOVI(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de usuarios.", ex);
            }
        }

        public void GetCtasMOVI(int liCodTMS)
        {
            string lsUrlOpenDS = (string)KDBUtil.SearchScalar("ServidorTMS", liCodTMS, "{URLOpenDS}");
            // Obtener el nodo de usuarios
            DirectoryEntry de = new DirectoryEntry(lsUrlOpenDS + "/ou=users,dc=provisioning",
                    "cn=Directory Manager", "3v0xCONFsyo", AuthenticationTypes.ServerBind);

            // Barrer los usuarios
            foreach (DirectoryEntry children in de.Children)
            {
                // Revisar si la cuenta existe en seeyouon
                string lsDePath = children.Path.Substring(children.Path.LastIndexOf("/") + 1);
                SearchResult leResult = LDAPGetEntry(liCodTMS, lsDePath,
                    new string[] { "memberOf", "mail", "displayName", "cn" });
                if (MOVIExists(leResult, liCodTMS, lsDePath))
                    continue;
                else
                {
                    //string lsGroupPath = de.Path.Substring(de.Path.LastIndexOf("/") + 1);
                    CrearCtaMOVISYO(leResult, liCodTMS, lsDePath);
                }
            }
        }

        public bool MOVIExists(SearchResult ldeLDAP, int liCodTMS, string lsDireccionLDAP)
        {

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Append("select iCodCatalogo from ");
            lsbQuery.Append(DSODataContext.Schema);
            lsbQuery.Append(".[VisHistoricos('TMSSystems','Cuenta Movi','Español')] where dtIniVigencia <> dtFinVigencia And DireccionLDAP = '");
            lsbQuery.Append(lsDireccionLDAP);
            lsbQuery.Append("'");
            lsbQuery.AppendLine("  and ServidorTMS = " + liCodTMS);

            DataTable ldtCuentasMovi = DSODataAccess.Execute(lsbQuery.ToString());

            return (ldtCuentasMovi != null && ldtCuentasMovi.Rows.Count > 0);
        }

        public bool CrearCtaMOVISYO(SearchResult ldMovi, int liCodTMS, string lsDireccionLDAP)
        {
            #region Crear la cuenta movi como sistema
            string lsDominio = ObtenerDominioMOVI(ldMovi, liCodTMS);

            string lsCodMoviAccount = "TMSCM " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string lsEmail = ldMovi.Properties["mail"][0].ToString();
            string lsUsuarioTMS = ldMovi.Properties["cn"][0].ToString() + lsDominio;

            UTF8Encoding leEncoding = new UTF8Encoding();
            string lsGroupPath = leEncoding.GetString((byte[])ldMovi.Properties["memberOf"][0]).Replace(", ", ",") + ",dc=provisioning";
            string lsNombre = leEncoding.GetString((byte[])ldMovi.Properties["displayName"][0]);
            //lsGroupPath = lsGroupPath.Replace(", ", ",") + ",dc=provisioning";

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Append("select iCodCatalogo, Emple  from ");
            lsbQuery.Append(DSODataContext.Schema);
            lsbQuery.AppendLine(".[VisHistoricos('TMSGroup','Grupo Provisioning','Español')] where dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("  and ServidorTMS = " + liCodTMS);
            lsbQuery.Append("  and DireccionLDAP = '");
            lsbQuery.Append(lsGroupPath);
            lsbQuery.AppendLine("'");

            DataRow ldrGrupoTMS = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            if (ldrGrupoTMS == null)
            {
                return false;
            }

            int liTipoCamara = 1;

            lsbQuery.Length = 0;
            lsbQuery.Append("select iCodCatalogo from ");
            lsbQuery.Append(DSODataContext.Schema);
            lsbQuery.AppendLine(".[VisHistoricos('TMSSystemTypes','TMS System Type','Español')] where dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("  and vchCodigo = 'TMSSTCuentaMovi'");

            int liTipoSistema = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString(), -1);
            if (liTipoSistema <= 0)
                return false;

            Hashtable lhtMovi = new Hashtable();
            lhtMovi.Add("{Emple}", (int)ldrGrupoTMS["Emple"]);
            lhtMovi.Add("{TMSSystemTypes}", liTipoSistema);
            lhtMovi.Add("{ServidorTMS}", liCodTMS);
            lhtMovi.Add("{TMSGroup}", (int)ldrGrupoTMS["iCodCatalogo"]);
            lhtMovi.Add("{TipoCamara}", liTipoCamara);
            lhtMovi.Add("{CuentaMovi}", lsEmail);
            lhtMovi.Add("{Nombre}", lsNombre);
            lhtMovi.Add("{Email}", lsEmail);
            lhtMovi.Add("{UsuarioTMS}", lsUsuarioTMS);
            lhtMovi.Add("{DireccionLDAP}", lsDireccionLDAP);

            int liCodCatalogoMoviAcc = KDBUtil.SaveHistoric("TMSSystems", "Cuenta Movi", lsCodMoviAccount, lsUsuarioTMS, lhtMovi);

            if (liCodCatalogoMoviAcc <= 0)
                return false;

            #endregion

            #region Crear la dirección de la cuenta movi si se necesita

            int liCodCatalogoDireccion = CrearDireccionSYO(lsUsuarioTMS);

            //lsbQuery.Length = 0;
            //lsbQuery.Append("select iCodCatalogo from ");
            //lsbQuery.Append(DSODataContext.Schema);
            //lsbQuery.AppendLine(".[VisHistoricos('Address','Address','Español')] where dtIniVigencia <> dtFinVigencia");
            //lsbQuery.AppendLine("  and SystemAddress = '" + lsUsuarioTMS + "'");

            ////int liCodDireccion = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString(), -1);
            //liCodDireccion = CrearDireccionCtaMovi(lsUsuarioTMS);

            //if (liCodDireccion <= 0)
            //{
            //    lsbQuery.Length = 0;
            //    lsbQuery.Append("select iCodCatalogo from ");
            //    lsbQuery.Append(DSODataContext.Schema);
            //    lsbQuery.AppendLine(".[VisHistoricos('MCUCallProtocol','MCU Call Protocol','Español')] where dtIniVigencia <> dtFinVigencia");
            //    lsbQuery.AppendLine("  and vchCodigo = 'SIP'");
            //    int liCodProtocolo = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString(), -1);
            //    Hashtable lhtDireccion = new Hashtable();
            //    lhtDireccion.Add("{SystemAddress}", lsUsuarioTMS);
            //    lhtDireccion.Add("{MCUCallProtocol}", liCodProtocolo);
            //    liCodDireccion = KDBUtil.SaveHistoric("Address", "Address", "Addr " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "SIP:" + lsEmail, lhtDireccion);
            //}

            if (liCodCatalogoDireccion <= 0)
                return false;

            #endregion

            #region Asociar la dirección con el sistema
            int liCodSystemAddress = CrearDireccionDeSistema(liCodCatalogoMoviAcc, liCodCatalogoDireccion);
            //Hashtable lhtSystemAddress = new Hashtable();
            //lhtSystemAddress.Add("{Address}", liCodDireccion);
            //lhtSystemAddress.Add("{TMSSystems}", liCatalogoMovi);
            //int liCodSystemAddress = KDBUtil.SaveHistoric("TMSSystemAddress", "TMSSystemAddress", "SysAddr " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), lhtSystemAddress);
            #endregion

            if (liCodSystemAddress > 0)
                return true;
            else
                return false;
        }

        protected int CrearDireccionSYO(string lsSystemAddress)
        {
            int liCodDireccion = -1;
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Append("select iCodCatalogo from ");
            lsbQuery.Append(DSODataContext.Schema);
            lsbQuery.AppendLine(".[VisHistoricos('Address','Address','Español')] where dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("  and SystemAddress = '" + lsSystemAddress + "'");

            liCodDireccion = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString(), -1);

            if (liCodDireccion <= 0)
            {
                lsbQuery.Length = 0;
                lsbQuery.Append("select iCodCatalogo from ");
                lsbQuery.Append(DSODataContext.Schema);
                lsbQuery.AppendLine(".[VisHistoricos('MCUCallProtocol','MCU Call Protocol','Español')] where dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("  and vchCodigo = 'SIP'");
                int liCodProtocolo = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString(), -1);
                Hashtable lhtDireccion = new Hashtable();
                lhtDireccion.Add("{SystemAddress}", lsSystemAddress);
                lhtDireccion.Add("{MCUCallProtocol}", liCodProtocolo);
                liCodDireccion = KDBUtil.SaveHistoric("Address", "Address", "Addr " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "SIP: " + lsSystemAddress, lhtDireccion);
            }
            return liCodDireccion;
        }

        protected int CrearDireccionDeSistema(int liCodCatalogoSistema, int liCodCatalogoDireccion)
        {
            Hashtable lhtSystemAddress = new Hashtable();
            lhtSystemAddress.Add("{Address}", liCodCatalogoDireccion);
            lhtSystemAddress.Add("{TMSSystems}", liCodCatalogoSistema);
            int liCodSystemAddress = KDBUtil.SaveHistoric("TMSSystemAddress", "TMSSystemAddress", "SysAddr " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), lhtSystemAddress);
            return liCodSystemAddress;
        }

        private string ObtenerDominioMOVI(SearchResult ldMovi, int liCodTMS)
        {
            string dominio = "";
            UTF8Encoding leEncoding = new UTF8Encoding();
            if (ldMovi.Properties.Contains("tt-deviceUriPattern"))
            {
                dominio = ldMovi.Properties["tt-deviceUriPattern"][0].ToString();
            }
            else
            {
                string lsPath = leEncoding.GetString((byte[])ldMovi.Properties["memberOf"][0]).Replace(", ", ",") + ",dc=provisioning";
                SearchResult leResult = LDAPGetEntry(liCodTMS, lsPath, new string[] { "memberOf", "tt-deviceUriPattern" });
                if (leResult == null)
                {
                    throw new Exception("Error encontrando el dominio de la cuenta movi.");
                }
                if (leResult.Properties.Contains("tt-deviceUriPattern"))
                {
                    dominio = leEncoding.GetString((byte[])leResult.Properties["tt-deviceUriPattern"][0]);
                    dominio = dominio.Substring(dominio.IndexOf("@"));
                }
                else
                {
                    dominio = ObtenerDominioMOVI(leResult, liCodTMS);
                }
            }
            return dominio;
        }

        public void RemoveCtasMOVI()
        {
            StringBuilder lsQuery = new StringBuilder();
            string lsVsita = "[VisHistoricos('TMSSystems','Cuenta Movi','Español')]";

            lsQuery.Length = 0;
            lsQuery.AppendLine("Select iCodCatalogo ,CuentaMovi, ServidorTMS, DireccionLDAP ");
            lsQuery.AppendLine("from [" + DSODataContext.Schema + "]." + lsVsita);
            lsQuery.AppendLine("where dtIniVigencia <> dtFinVigencia ");
            lsQuery.AppendLine("and dtFinVigencia < getDate() ");

            DataTable ldtCtasMovi = DSODataAccess.Execute(lsQuery.ToString());

            //Borrar la cuenta MOVI 
            int liCodServidorTMS = 0;
            string lsDireccionLDAP = "";
            string lsCuentaMovi = "";
            if (ldtCtasMovi != null)
                foreach (DataRow ldr in ldtCtasMovi.Rows)
                {
                    liCodServidorTMS = (int)Util.IsDBNull(ldr["ServidorTMS"], 0);
                    lsDireccionLDAP = (string)Util.IsDBNull(ldr["DireccionLDAP"], "");
                    lsCuentaMovi = (string)Util.IsDBNull(ldr["CuentaMovi"], "");
                    if (!string.IsNullOrEmpty(lsDireccionLDAP) && liCodServidorTMS > 0)
                        SYODeleteMOVI(liCodServidorTMS, lsDireccionLDAP, lsCuentaMovi);

                }

        }

        public void SYODeleteMOVI(int iCodServidorTMS, string lsDireccionLDAP, string lsCuentaMovi)
        {
            try
            {
                LDAPUserDelete(iCodServidorTMS, "cn=config," + lsDireccionLDAP);
                LDAPUserDelete(iCodServidorTMS, lsDireccionLDAP);

            }
            catch (Exception ex)
            {
                Util.LogException("Error al Remover la Cuenta MOVI SYO '" + lsCuentaMovi + "'  en TMS.", ex);
            }

        }

        public void TMSMoviSave(int liCodMoviAccount)
        {
            int liCodTMS = int.MinValue;

            try
            {
                DataRow ldrMA = KDBUtil.SearchHistoricRow("TMSSystems", "Cuenta Movi",
                    new string[] { "iCodRegistro", "{ServidorTMS}", "{Nombre}", "{Email}", "{UsuarioTMS}",
                        "{TMSGroup}", "{DireccionLDAP}" },
                    "iCodCatalogo = " + liCodMoviAccount);

                if (ldrMA == null)
                    throw new Exception("No se encontró la cuenta movi '" + liCodMoviAccount + "'");

                DataRow ldrGP = KDBUtil.SearchHistoricRow("TMSGroup", "Grupo Provisioning",
                    new string[] { "{vchDescripcion}", "{DireccionLDAP}" },
                    "iCodCatalogo = " + (int)Util.IsDBNull(ldrMA["{TMSGroup}"], int.MinValue));

                if (ldrGP == null)
                    throw new Exception("No se encontró el grupo de provisioning '" + (int)Util.IsDBNull(ldrMA["{TMSGroup}"], int.MinValue) + "'");

                liCodTMS = (int)ldrMA["{ServidorTMS}"];

                string lsCodMoviAccount = (string)ldrMA["vchCodigo"];
                string lsName = (string)ldrMA["{Nombre}"];
                string lsEmail = (string)ldrMA["{Email}"];
                string lsUser = (string)ldrMA["{UsuarioTMS}"];
                string lsGroupPath = (string)ldrGP["{DireccionLDAP}"];
                string lsUserLDAP = (string)Util.IsDBNull(ldrMA["{DireccionLDAP}"], "");

                if (string.IsNullOrEmpty(lsUserLDAP))
                {
                    psDominioCtaMovi = "";
                    LDAPUserAdd(liCodTMS, lsCodMoviAccount, lsUser, lsName, lsEmail, lsGroupPath);
                    if (!string.IsNullOrEmpty(psDominioCtaMovi))
                    {
                        int liCodCatalogoDireccion = CrearDireccionSYO(lsUser + psDominioCtaMovi);
                        int liCodSystemAddress = CrearDireccionDeSistema(liCodMoviAccount, liCodCatalogoDireccion);
                    }
                }
                else
                    LDAPUserUpdate(liCodTMS, lsCodMoviAccount, lsUser, lsName, lsEmail, lsGroupPath, lsUserLDAP);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al crear la cuenta movi '" + liCodMoviAccount + "' el servidor LDAP del TMS '" + liCodTMS + "'", ex);
                throw ex;
            }
        }

        public void TMSMoviDelete(int liCodMoviAccount)
        {
            DataRow ldrMA = KDBUtil.SearchHistoricRow("TMSSystems", "Cuenta Movi",
                new string[] { "iCodRegistro", "{ServidorTMS}", "{DireccionLDAP}" },
                "iCodCatalogo = " + liCodMoviAccount);

            if (ldrMA == null)
                throw new Exception("No se encontró la cuenta movi '" + liCodMoviAccount + "'");

            if (!string.IsNullOrEmpty((string)Util.IsDBNull(ldrMA["{DireccionLDAP}"], "")))
            {
                LDAPUserDelete((int)ldrMA["{ServidorTMS}"], "cn=config," + (string)ldrMA["{DireccionLDAP}"]);
                LDAPUserDelete((int)ldrMA["{ServidorTMS}"], (string)ldrMA["{DireccionLDAP}"]);
            }
        }

        /// <summary>
        /// Método para obtener el siguiente password para cuentas movi generadas desde la web de SYO.
        /// </summary>
        /// <returns>DataRow con la información del siguiente password</returns>
        public DataRow SiguientePasswordMovi()
        {
            // DataRow para regresar resultados
            DataRow ldrResult = null;

            try
            {
                // Variable para indicar qué registro se tomará como el siguiente password
                int liRow = 0;

                // Variable para guardar el número total de passwords
                int liNumPasswords = 0;

                // Variable para indicar el siguiente índice del password que se usará
                int liSiguientePass = 0;

                // Variable para guardar el iCodRegistro del registro configurado con el "Siguiente Password"
                int liCodSiguientePass = 0;

                // vchCodigo y vchDescripcion del registro configurado con el "Siguiente Password"
                string lsCodigoSP = "";
                string lsDescSP = "";

                // Obtenemos todos los passwords configurados que existen. Los ordenamos por iCodCatalogo.
                DataTable ldtPass = kdb.GetHisRegByEnt("TMSMoviPass", "Password Movi",
                    new string[] { "{Password}", "{PassTMS}", "iCodCatalogo" }, "", "iCodCatalogo");

                if (ldtPass != null && ldtPass.Rows.Count > 0)
                {
                    liNumPasswords = ldtPass.Rows.Count;
                }

                // Obtenemos los registros que guardan el siguiente password. 
                // Se ordena por iCodCatalogo y se toma siempre el primer elemento.
                DataTable ldtNextPass = kdb.GetHisRegByEnt("TMSMoviPass", "Siguiente Password",
                    new string[] { "iCodRegistro", "{Siguiente}", "vchDescripcion", "iCodCatalogo" },
                    "", "iCodCatalogo");

                // Inicializamos el siguiente pass en 1, vchCodigo y vchDescripcion.
                // Por si no se encuentra nada configurado
                liSiguientePass = 1;
                lsCodigoSP = "NextMoviPass_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                lsDescSP = "Siguiente Password Movi - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Si se encontó configurado por lo menos un registro del maestro 'Siguiente Password'
                if (ldtNextPass != null && ldtNextPass.Rows.Count > 0)
                {
                    // Se usará este como el bueno
                    liSiguientePass = (int)Util.IsDBNull(ldtNextPass.Rows[0]["{Siguiente}"], 1);
                    // Almacenamos la información del registro que hay que actualizar
                    liCodSiguientePass = (int)ldtNextPass.Rows[0]["iCodRegistro"];
                    lsCodigoSP = ldtNextPass.Rows[0]["vchCodigo"].ToString();
                    lsDescSP = ldtNextPass.Rows[0]["vchDescripcion"].ToString();
                }

                // Ajustamos el siguiente pass, que está con índice en base 1 a un índice en base 0
                liRow = liSiguientePass - 1;

                // Ajustamos el índice que se usará para un password futuro, tomando 
                // en cuenta que el índice mostrado al usuario está en base a 1.
                if (liSiguientePass < liNumPasswords)
                {
                    liSiguientePass = liSiguientePass + 1;
                }
                else
                {
                    liSiguientePass = 1;
                }

                // Guardamos la información del siguiente password que se usará
                Hashtable lhtSiguientePass = new Hashtable();
                if (liCodSiguientePass != 0)
                {
                    lhtSiguientePass["iCodRegistro"] = liCodSiguientePass;
                }
                lhtSiguientePass["{Siguiente}"] = liSiguientePass;
                // Actualiza o crea un histórico nuevo, según sea el caso.
                KDBUtil.SaveHistoric("TMSMoviPass", "Siguiente Password", lsCodigoSP, lsDescSP, lhtSiguientePass);

                // Asignamos el resultado y lo regresamos
                ldrResult = ldtPass.Rows[liRow];
            }
            catch (Exception ex)
            {
                Util.LogException("Ocurrió un error obteniendo el password de la cuenta movi.", ex);
                throw ex;
            }
            return ldrResult;
        }

        public void LDAPUserAdd(int liCodTMS, string lsCodMoviAccount, string lsUser, string lsName, string lsEmail, string lsGroupPath)
        {
            string lsUrlOpenDS = (string)KDBUtil.SearchScalar("ServidorTMS", liCodTMS, "{URLOpenDS}");
            string lsUUID = Guid.NewGuid().ToString();
            string lsCUID = Guid.NewGuid().ToString();
            string lsPassword = "";
            string lsPasswordVCS = "";
            string lsPathUsuario = "";
            SearchResult leResult = null;

            Random loRnd = new Random(DateTime.Now.Millisecond);

            DataRow ldrPass = SiguientePasswordMovi();

            lsPassword = Util.Decrypt((string)ldrPass["{Password}"]);
            lsPasswordVCS = Util.Decrypt((string)ldrPass["{PassTMS}"]);

            try
            {
                DirectoryEntry de = new DirectoryEntry(lsUrlOpenDS + "/ou=users,dc=provisioning",
                    "cn=Directory Manager", "3v0xCONFsyo", AuthenticationTypes.ServerBind);

                DirectoryEntry nu = de.Children.Add("uid=" + lsUUID, "user");
                nu.Properties["objectClass"].Add("user");
                nu.Properties["objectClass"].Add("commURIObject");
                nu.Properties["objectClass"].Add("tt-findmeAwareUser");
                nu.Properties["cn"].Add(lsUser);
                nu.Properties["mail"].Add(lsEmail);
                nu.Properties["displayName"].Add(lsName);
                nu.Properties["sn"].Add(lsName);
                nu.Properties["memberOf"].Add(lsGroupPath.Replace(",dc=provisioning", "").Replace(",", ", "));
                nu.Properties["userPassword"].Add(lsPassword);
                nu.Properties["commURI"].Add("ldap:///ou=profiles,dc=provisioning??one?(commUniqueId=" + lsCUID + ") deprecated.default.profile");
                nu.Properties["numericUserId"].Add(loRnd.Next(1000000, 1999999)); //TODO: cambiar
                nu.Properties["initials"].Add(lsPasswordVCS);
                nu.Properties["tt-findmeUriSticky"].Add("FALSE");
                nu.Properties["tt-searchField"].Add(lsName);
                nu.CommitChanges();

                lsPathUsuario = nu.Path.Substring(nu.Path.LastIndexOf("/") + 1);

                DirectoryEntry nuc = nu.Children.Add("cn=config", "inetResource");
                nuc.Properties["objectClass"].Add("inetResource");
                nuc.Properties["objectClass"].Add("configObject");
                nuc.Properties["lastUpdated"].Add(DateTime.Now.ToString("yyyyMMddHHmmss.fffZ"));
                nuc.CommitChanges();

                Hashtable lhtVal = new Hashtable();
                lhtVal.Add("{DireccionLDAP}", nu.Path.Substring(nu.Path.LastIndexOf("/") + 1));
                lhtVal.Add("{Password}", Util.Encrypt(lsPassword));
                KDBUtil.SaveHistoric("TMSSystems", "Cuenta Movi", lsCodMoviAccount, null, lhtVal);

                nu.Dispose(); nu = null;
                nuc.Dispose(); nuc = null;
                de.Dispose(); de = null;


                de = new DirectoryEntry(lsUrlOpenDS + "/ou=profiles,dc=provisioning",
                    "cn=Directory Manager", "3v0xCONFsyo", AuthenticationTypes.ServerBind);

                DirectoryEntry nc = de.Children.Add("commUniqueId=" + lsCUID, "profile");
                nc.Properties["objectClass"].Add("profile");
                nc.Properties["objectClass"].Add("SIPIdentity");
                nc.Properties["objectClass"].Add("h235Identity");
                nc.Properties["objectClass"].Add("h323Identity");
                nc.Properties["commOwner"].Add("uid=" + lsUUID + ", ou=users");
                nc.Properties["displayName"].Add("deprecated.default.profile");
                nc.Properties["SIPIdentityPassword"].Add(lsPassword);
                nc.Properties["SIPIdentitySIPURI"].Add("sip:deprecated." + lsUser + "@tandberg.com");
                nc.Properties["SIPIdentityUserName"].Add(lsUser);
                nc.Properties["isDefault"].Add("FALSE");
                nc.CommitChanges();

                DirectoryEntry ncc = nc.Children.Add("cn=config", "inetResource");
                ncc.Properties["objectClass"].Add("inetResource");
                ncc.Properties["objectClass"].Add("configObject");
                ncc.Properties["lastUpdated"].Add(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss.fffZ"));
                ncc.CommitChanges();

                nc.Dispose(); nc = null;
                ncc.Dispose(); ncc = null;
                de.Dispose(); de = null;

            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al crear la cuenta movi '" + lsUser + "' el servidor LDAP '" + lsUrlOpenDS + "'", ex);
                throw ex;
            }
            try
            {
                leResult = LDAPGetEntry(liCodTMS, lsPathUsuario,
                    new string[] { "memberOf", "mail", "displayName", "cn" });
                if (leResult != null)
                {
                    psDominioCtaMovi = ObtenerDominioMOVI(leResult, liCodTMS);
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al obtener el dominio la cuenta movi '" + lsUser + "' el servidor LDAP '" + lsUrlOpenDS + "'.\r\nLa cuenta movi si se generó.", ex);
                throw ex;
            }
        }

        public void LDAPUserUpdate(int liCodTMS, string lsCodMoviAccount, string lsUser, string lsName, string lsEmail, string lsGroupPath, string lsUserLDAP)
        {
            string lsUrlOpenDS = "";
            SearchResult sr = null;

            try
            {
                lsUrlOpenDS = (string)KDBUtil.SearchScalar("ServidorTMS", liCodTMS, "{URLOpenDS}");

                try
                {
                    sr = LDAPGetEntry(liCodTMS, lsUserLDAP, new string[] { "cn", "memberOf", "mail", "displayName", "commURI" });
                }
                catch (Exception ex)
                {
                    sr = null;
                }

                if (sr != null && (string)sr.Properties["cn"][0] != lsUser)
                    throw new Exception("No se puede modificar el nombre de usuario.");

                if (sr != null)
                {
                    LDAPUserDelete(liCodTMS, "cn=config," + lsUserLDAP);
                    LDAPUserDelete(liCodTMS, lsUserLDAP);
                }

                LDAPUserAdd(liCodTMS, lsCodMoviAccount, lsUser, lsName, lsEmail, lsGroupPath);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al modificar la cuenta movi '" + lsUser + "' el servidor LDAP '" + lsUrlOpenDS + "'", ex);
                throw ex;
            }
        }

        public void LDAPUserDelete(int liCodTMS, string lsUserLDAP)
        {
            string lsUrlOpenDS = "";

            try
            {
                lsUrlOpenDS = (string)KDBUtil.SearchScalar("ServidorTMS", liCodTMS, "{URLOpenDS}");

                try
                {
                    DirectoryEntry de = new DirectoryEntry(lsUrlOpenDS + "/" + lsUserLDAP,
                         "cn=Directory Manager", "3v0xCONFsyo", AuthenticationTypes.ServerBind);

                    if (de != null && de.Parent != null)
                    {

                        DirectoryEntry deParent = de.Parent;

                        deParent.Children.Remove(de);
                        deParent.CommitChanges();
                    }
                }
                catch (Exception ex)
                {
                    //Util.LogMessage("No se encontró la cuenta movi '" + lsUserLDAP + "' el servidor LDAP '" + lsUrlOpenDS + "'.");
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al eliminar la cuenta movi '" + lsUserLDAP + "' el servidor LDAP '" + lsUrlOpenDS + "'", ex);
                throw ex;
            }
        }

        public SearchResult LDAPGetEntry(int liCodTMS, string lsPath, string[] lsProperties)
        {
            string lsUrlOpenDS = (string)KDBUtil.SearchScalar("ServidorTMS", liCodTMS, "{URLOpenDS}");

            DirectoryEntry de = new DirectoryEntry(lsUrlOpenDS + "/" + lsPath,
                 "cn=Directory Manager", "3v0xCONFsyo", AuthenticationTypes.ServerBind);

            DirectorySearcher ds = new DirectorySearcher(de);
            ds.SearchScope = SearchScope.Base;

            if (lsProperties != null)
                foreach (string lsProperty in lsProperties)
                    ds.PropertiesToLoad.Add(lsProperty);

            SearchResult loSR = ds.FindOne();

            return loSR;
        }

        public SearchResult LDAPSearchEntry(int liCodTMS, string lsBasePath, string lsQuery, string[] lsProperties)
        {
            string lsUrlOpenDS = (string)KDBUtil.SearchScalar("ServidorTMS", liCodTMS, "{URLOpenDS}");

            DirectoryEntry de = new DirectoryEntry(lsUrlOpenDS + "/" + lsBasePath,
                 "cn=Directory Manager", "3v0xCONFsyo", AuthenticationTypes.ServerBind);

            DirectorySearcher ds = new DirectorySearcher(de);
            ds.SearchScope = SearchScope.Subtree;
            ds.Filter = lsQuery;

            if (lsProperties != null)
                foreach (string lsProperty in lsProperties)
                    ds.PropertiesToLoad.Add(lsProperty);

            SearchResult loSR = ds.FindOne();

            return loSR;
        }
        #endregion

        #region TMS Conferences
        //Incompleto... Ya no va
        protected int TMSConfSave(BookingService loBooking, DataRow ldrConf)
        {
            int liRet = int.MinValue;
            Hashtable lhtVal;

            try
            {
                //Graba la conferencia en TMS
                Conference loTMSConf = loBooking.GetDefaultConference();
                loTMSConf.StartTimeUTC = ((DateTime)ldrConf["{FechaInicioReservacion}"]).ToString("yyyy-MM-ddTHH:mm:ssZ"); //TODO: Cual TZ es la origen, actualmente tomo la del servidor K5
                loTMSConf.EndTimeUTC = ((DateTime)ldrConf["{FechaFinReservacion}"]).ToString("yyyy-MM-ddTHH:mm:ssZ"); //TODO: Cual TZ es la origen, actualmente tomo la del servidor K5

                //TODO: participantes de la conferencia
                loTMSConf.Participants = new Participant[] { new Participant() };
                loTMSConf.Participants[0].ParticipantId = 2;

                loTMSConf = loBooking.SaveConference(loTMSConf);
                liRet = loTMSConf.ConferenceId;

                //Actualiza la conferencia en SYO
                lhtVal = new Hashtable();
                lhtVal.Add("{ConferenceId}", loTMSConf.ConferenceId);

                KDBUtil.SaveHistoric("TMSConf", "Conferencia", (string)ldrConf["vchCodigo"], null, lhtVal);
            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar la conferencia SYO '" + ldrConf["iCodCatalogo"] + " " + ldrConf["vchCodigo"] + " " + ldrConf["vchDescripcion"] + "' en TMS.", ex);
            }

            return liRet;
        }
        #endregion

        #region util
        protected void HTAddIfDiferent(DataRow ldr, Hashtable lht, string lsField, object loValue)
        {
            HTAddIfDiferent(ldr, lht, lsField, loValue, null);
        }

        protected void HTAddIfDiferent(DataRow ldr, Hashtable lht, string lsField, object loValue, Type ltConvert)
        {
            if (!lht.ContainsKey(lsField) &&
                (ldr == null ||
                (ldr != null && !ldr.Table.Columns.Contains(lsField)) ||
                (ldr != null && ldr.Table.Columns.Contains(lsField) && ltConvert == null && !ldr[lsField].Equals(loValue)) ||
                (ldr != null && ldr.Table.Columns.Contains(lsField) && ltConvert != null && !Convert.ChangeType(ldr[lsField], ltConvert).Equals(Convert.ChangeType(loValue, ltConvert)))))
            {
                lht.Add(lsField, loValue);
            }
        }

        #endregion
    }
}
