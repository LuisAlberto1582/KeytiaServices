/*
Nombre:		    PGS
Fecha:		    20110714
Descripción:	Configuración específica para almacenamiento de Sitios.
Modificación:	
*/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgSitiosEdit : HistoricEdit
    {
        protected HtmlButton pbtnNotifPrepSitio;

        public CnfgSitiosEdit()
        {
            Init += new EventHandler(CnfgSitiosEdit_Init);
        }

        void CnfgSitiosEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgSitiosEdit";
            pbtnNotifPrepSitio = new HtmlButton();

            pToolBar.Controls.Add(pbtnNotifPrepSitio);
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pbtnNotifPrepSitio.Visible = false;

            if (s == HistoricState.Consulta)
            {
                InitConfigPresupuestos();
            }

            if (pFields != null && pFields.ContainsConfigName("TipoPr"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("TipoPr").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PeriodoPr"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PeriodoPr").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PresupFijo"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PresupFijo").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PresupProv"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PresupProv").Row - 1].Visible = false;
            }
        }

        protected virtual void InitConfigPresupuestos()
        {
            //Revisa si la empresa a la que pertenece el sitio tiene prendida la bandera de Activar notificaciones de presupuestos
            //y la bandera de generacion de archivos de altas y bajas de codigos para mostrar el boton de pbtnNotifPrepSitio
            pPanelSubHistoricos.Visible = false;

            if (!pFields.ContainsConfigName("Empre") && !pFields.GetByConfigName("Empre").DSOControlDB.HasValue)
            {
                return;
            }

            DateTime ldtAhora = DateTime.Now;

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select BanderasEmpre = isnull(Empre.BanderasEmpre,0)");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre");
            lsbQuery.AppendLine("where Empre.iCodCatalogo = " + pFields.GetByConfigName("Empre").DataValue);
            lsbQuery.AppendLine("and Empre.dtIniVigencia <> Empre.dtFinVigencia");
            lsbQuery.AppendLine("and Empre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Empre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

            if (lDataTable.Rows.Count > 0
                && ((int)lDataTable.Rows[0]["BanderasEmpre"] & 1) == 1
                && ((int)lDataTable.Rows[0]["BanderasEmpre"] & 2) == 2)
            {
                pbtnNotifPrepSitio.Visible = true;
            }
        }

        protected override void InitAcciones()
        {
            base.InitAcciones();

            pbtnNotifPrepSitio.ID = "btnNotifPrepSitio";
            pbtnNotifPrepSitio.Attributes["class"] = "buttonEdit";
            pbtnNotifPrepSitio.Style["display"] = "none";

            pbtnNotifPrepSitio.ServerClick += new EventHandler(pbtnNotifPrepSitio_ServerClick);
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            if (pFields != null)
            {
                if (pFields.ContainsConfigName("Emple"))
                {
                    KeytiaBaseField lField = pFields.GetByConfigName("Emple");
                    lField.DSOControlDB.TcLbl.Text = Globals.GetMsgWeb(false, "MsgEmpleResp");
                }

                if (pFields.ContainsConfigName("Sitio - Empleados"))
                {
                    KeytiaBaseField lField = pFields.GetByConfigName("Sitio - Empleados");
                    lField.DSOControlDB.TcLbl.Text = lField.Descripcion + " " + Globals.GetMsgWeb(false, "SubHistorialField");
                }
            }
        }

        protected virtual void pbtnNotifPrepSitio_ServerClick(object sender, EventArgs e)
        {
            FillAjaxControls();

            PrevState = State;
            SubHistoricClass = "KeytiaWeb.UserInterface.CnfgNotifPrepSitio";
            SubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";

            InitSubHistorico(this.ID + "CnfgNotifPrepSitio");

            pSubHistorico.SetEntidad("NotifPrepSitio");
            pSubHistorico.SetMaestro("Notificaciones de Presupuestos para Sitios");

            pSubHistorico.EsSubHistorico = true;
            pSubHistorico.FillControls();

            SetHistoricState(HistoricState.CnfgSubHistoricField);
            pSubHistorico.InitMaestro();

            DateTime ldtAhora = DateTime.Now;
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select iCodRegistro");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepSitio','Notificaciones de Presupuestos para Sitios','" + Globals.GetCurrentLanguage() + "')] NotifPrepSitio");
            lsbQuery.AppendLine("where NotifPrepSitio.Sitio = " + iCodCatalogo);
            lsbQuery.AppendLine("and NotifPrepSitio.dtIniVigencia <> NotifPrepSitio.dtFinVigencia");
            lsbQuery.AppendLine("and NotifPrepSitio.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and NotifPrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                pSubHistorico.iCodRegistro = (lDataTable.Rows[0]["iCodRegistro"]).ToString();
                pSubHistorico.ConsultarRegistro();
            }


            pSubHistorico.Fields.EnableFields();

            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DataValue = iCodCatalogo;
            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DisableField();

            pSubHistorico.SetHistoricState(HistoricState.Edicion);
        }

        public override void RaisePostBackEvent(string eventArgument)
        {
            base.RaisePostBackEvent(eventArgument);
            if (eventArgument == "btnBaja")
            {
                SetHistoricState(HistoricState.Baja);
                pbtnGrabar_ServerClick(pbtnBaja, new EventArgs());
            }
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgSitiosEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgSitiosEdit.js") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected override void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            base.pbtnBaja_ServerClick(sender, e);
            string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "btnBaja");
            pbtnGrabar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = "Confirmar(function(){" + lsdoPostBack + "}," + ContieneRecursos() + ",'" + Globals.GetMsgWeb(false, "RecSitAsig") + "','" + Globals.GetMsgWeb(false, "ConfirmarTitulo") + "');return false;";
        }

        protected override void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            if (State != HistoricState.Baja)
            {
                base.pbtnGrabar_ServerClick(sender, e);
            }
            else
            {
                if (ValidarRecursos() && ValidarDeta_Pend())
                {
                    base.pbtnGrabar_ServerClick(sender, e);
                }
            }
            ActualizaRestricciones();
        }

        protected void ActualizaRestricciones()
        {
            KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");

            int liCodUsuario = (int)Session["iCodUsuarioDB"];
            pCargaCom.ActualizaRestriccionesSitio(iCodCatalogo, liCodUsuario);
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

        protected override bool ValidarCampos()
        {
            //si el registro se esta eliminando entonces no es necesaria la validacion
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            bool lbret = base.ValidarCampos();

            if (lbret)
            {
                string[] lsFieldRequeridos = new string[] { "Empre", "Locali" }; ;
                string lsError;
                StringBuilder lsbErrores = new StringBuilder();
                string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloSitios"));
                try
                {
                    if (pFields.GetByConfigName("TipoSitio").DSOControlDB.HasValue)
                    {
                        string lsTipoSitioDesc = pKDB.GetHisRegByEnt("TipoSitio", "Tipo de Sitio", "iCodCatalogo = " + pFields.GetByConfigName("TipoSitio").DataValue).Rows[0]["vchDescripcion"].ToString();
                        if (lsTipoSitioDesc == "PBX")
                        {
                            lsFieldRequeridos = new string[] { "Empre", "ExtIni", "ExtFin", "Locali", "LongExt" };
                        }
                    }

                    //Valida Campos obligatorios para Maestro de Sitio
                    for (int liFReq = 0; liFReq < lsFieldRequeridos.Length; liFReq++)
                    {
                        if (!pFields.ContainsConfigName(lsFieldRequeridos[liFReq]))
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequeridoMae", lsFieldRequeridos[liFReq]));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                        }
                    }

                    //Valida valor no nulo de campos obligatorios
                    foreach (KeytiaBaseField lField in pFields)
                    {
                        if (lsFieldRequeridos.Contains(lField.ConfigName) && (String.IsNullOrEmpty(lField.DataValue.ToString()) ||
                            lField.DataValue.ToString() == "null"))
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", lField.Descripcion));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                        }
                    }

                    if (lsbErrores.Length > 0)
                    {
                        lbret = false;
                        lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    }
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException("ErrValidateRecord", ex);
                }
            }

            return lbret;
        }

        protected override bool ValidarClaves()
        {
            if (!base.ValidarClaves())
            {
                return false;
            }

            bool lbret = true;
            int liCount;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloSitios"));

            try
            {
                Regex regexp = new Regex("^[a-zA-Z0-9 _-]+$");
                if (regexp.IsMatch(pvchCodigo.ToString()))
                {
                    pvchDescripcion.DataValue = pvchDescripcion.ToString().Trim();
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("Select Sitios = isnull(count(S.iCodRegistro),0)");
                    psbQuery.AppendLine("from [VisHistoricos('Sitio','" + Globals.GetCurrentLanguage() + "')] S");
                    psbQuery.AppendLine("Where S.vchDescripcion = '" + pvchDescripcion.ToString() + "'");
                    psbQuery.AppendLine("and S.dtIniVigencia <> S.dtFinVigencia");
                    psbQuery.AppendLine("and ((S.dtInivigencia <= " + pdtIniVigencia.DataValue + " and");
                    psbQuery.AppendLine("      S.dtFinvigencia > " + pdtIniVigencia.DataValue + ")");
                    psbQuery.AppendLine(" or  (S.dtInivigencia <= " + pdtFinVigencia.DataValue + " and");
                    psbQuery.AppendLine("      S.dtFinvigencia > " + pdtFinVigencia.DataValue + ")");
                    psbQuery.AppendLine(" or  (S.dtInivigencia >= " + pdtIniVigencia.DataValue + " and");
                    psbQuery.AppendLine("      S.dtFinvigencia <= " + pdtFinVigencia.DataValue + "))");

                    if (iCodCatalogo != "null")
                    {
                        psbQuery.AppendLine("and S.iCodCatalogo <> " + iCodCatalogo);
                    }

                    liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                    if (liCount > 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "SitioYaExiste", pvchDescripcion.ToString()));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
                else
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "SitioNombre", pvchCodigo.ToString()));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected override bool ValidarDatos()
        {
            //si el registro se esta eliminando entonces no es necesaria la validacion
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }
            return ValidarExtensiones() && ValidarEmpleado();
        }

        protected bool ValidarExtensiones()
        {
            bool lbret = true;
            string lsError;
            Regex regexp = new Regex("^[0-9]+(([,]{1}|[-]{1})*[0-9]+)*$");
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloSitios"));

            try
            {
                //KeytiaBaseField lFieldExtensiones = pFields.GetByConfigName("RangosExt");
                //KeytiaBaseField lFieldEmpresa = pFields.GetByConfigName("Empre");

                if (pFields.GetByConfigName("RangosExt").DSOControlDB.HasValue &&
                    pFields.GetByConfigName("Empre").DSOControlDB.HasValue)
                {
                    string lsExtensiones = pFields.GetByConfigName("RangosExt").DataValue.ToString();
                    lsExtensiones = lsExtensiones.Replace("\t", "");
                    lsExtensiones = lsExtensiones.Replace("\r\n", ",");
                    lsExtensiones = lsExtensiones.Replace(",'", "");
                    lsExtensiones = lsExtensiones.Replace(",,", ",");
                    lsExtensiones = lsExtensiones.Replace(" ", "");
                    lsExtensiones = lsExtensiones.Replace("'", "");
                    string[] lsaExtensiones = lsExtensiones.Split(',');

                    foreach (string lsExt in lsaExtensiones)
                    {
                        if (lsExt == "")
                        {
                            continue;
                        }

                        if (!regexp.IsMatch(lsExt))
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoExtInc", pFields.GetByConfigName("RangosExt").Descripcion));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                            break;
                        }

                        string[] lsaExtVal = lsExt.Replace(",", "").Split('-');
                        int RanIni;
                        int RanFin;
                        if (lsaExtVal.Length == 1)
                        {
                            RanIni = int.Parse(lsaExtVal[0]);
                            RanFin = int.Parse(lsaExtVal[0]);
                        }
                        else
                        {
                            RanIni = int.Parse(lsaExtVal[0]);
                            RanFin = int.Parse(lsaExtVal[1]);
                            if (RanFin < RanIni)
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoExtInc", pFields.GetByConfigName("RangosExt").Descripcion));
                                lsbErrores.Append("<li>" + lsError + "</li>");
                                break;
                            }
                        }

                        DataTable dtExtensiones;

                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select RTRIM(LTRIM(RangosExt)) AS RangosExt, vchCodigo");
                        psbQuery.AppendLine("from [VisHistoricos('Sitio','" + vchDesMaestro + "','" + Globals.GetCurrentLanguage() + "')] S");
                        psbQuery.AppendLine("where S.Empre = " + pFields.GetByConfigName("Empre").DataValue);

                        if (iCodCatalogo != "null")
                        {
                            psbQuery.AppendLine("and S.iCodCatalogo <> " + iCodCatalogo);
                        }
                        psbQuery.AppendLine("AND ISNULL(RangosExt,'') <> '' ");

                        dtExtensiones = DSODataAccess.Execute(psbQuery.ToString());
                        if (dtExtensiones == null || dtExtensiones.Rows.Count == 0)
                        {
                            continue;
                        }

                        foreach (DataRow drRanExt in dtExtensiones.Rows)
                        {
                            string RanExt = drRanExt["RangosExt"].ToString().Replace(" ", "").Trim();
                            RanExt = RanExt.Replace("\t", "");
                            RanExt = RanExt.Replace("\r\n", ",");
                            RanExt = RanExt.Replace(",,", ",");
                            if (RanExt == "")
                            {
                                continue;
                            }
                            string[] ExtEmpresa = RanExt.Split(',');
                            foreach (string lsExtEmpresa in ExtEmpresa)
                            {
                                string lsExtEmpre = lsExtEmpresa.Replace(" ", "").Trim();
                                if (lsExtEmpre == "99999999" || lsExtEmpre == "")
                                {
                                    continue;
                                }
                                string[] lsaExtEmpreVal = lsExtEmpre.Replace(",", "").Trim().Split('-');
                                int RanIniEmpre;
                                int RanFinEmpre;
                                if (lsaExtEmpreVal.Length == 1)
                                {
                                    RanIniEmpre = int.Parse(lsaExtEmpreVal[0]);
                                    RanFinEmpre = int.Parse(lsaExtEmpreVal[0]);
                                }
                                else
                                {
                                    RanIniEmpre = int.Parse(lsaExtEmpreVal[0]);
                                    RanFinEmpre = int.Parse(lsaExtEmpreVal[1]);
                                }
                                if ((RanIni >= RanIniEmpre && RanIni <= RanFinEmpre)
                                   || (RanFin >= RanIniEmpre && RanFin <= RanFinEmpre)
                                   || (RanIni <= RanIniEmpre && RanFin >= RanFinEmpre))
                                {
                                    string lsEmpalme = lsExtEmpre + " (" + drRanExt["vchCodigo"].ToString() + ")";
                                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoExt", lsExt, lsEmpalme));
                                    lsbErrores.Append("<li>" + lsError + "</li>");
                                }
                            }
                        }
                    }
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
                if (lbret)
                {
                    string lsDataValue = pFields.GetByConfigName("RangosExt").DataValue.ToString().Replace("'", "");
                    //if (iCodRegistro == "null")
                    //{
                    //    lsDataValue += ",99999999";
                    //}
                    //if (lsDataValue == "null")
                    //{
                    //    lsDataValue = "99999999";
                    //}
                    //lsDataValue = lsDataValue.Replace(" ", "");
                    lsDataValue = lsDataValue.Replace("\r\n", ",\r\n");
                    lsDataValue = lsDataValue.Replace(",,", ",");
                    lsDataValue = lsDataValue.Replace("null", "");
                    pFields.GetByConfigName("RangosExt").DataValue = lsDataValue;
                    phtValues = pFields.GetValues();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected bool ValidarEmpleado()
        {
            bool lbRet = true;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloSitios"));

            try
            {
                if (pFields.GetByConfigName("Emple").DSOControlDB.HasValue)
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select Registro = isnull(count(iCodRegistro), 0)");
                    psbQuery.AppendLine("from [VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')]");
                    psbQuery.AppendLine("where iCodCatalogo = " + pFields.GetByConfigName("Emple").DataValue);
                    psbQuery.AppendLine("and dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("and dtFinVigencia >= " + pdtFinVigencia.DataValue);

                    int liCodRegistro = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                    if (liCodRegistro == 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "VigEmpResp"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
                if (lsbErrores.Length > 0)
                {
                    lbRet = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbRet;
        }

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
            if (iCodCatalogo != "null")
            {
                GrabarPermisos("Epmpl");
                GrabarPermisos("Replicador");
                GrabarPermisos("Admin");
                GrabarPermisos("Config");

                GrabarPlanesDeMarcacion();
            }
        }

        protected override void GrabarRelaciones()
        {
            base.GrabarRelaciones();
            KeytiaBaseField lField = pFields.GetByConfigName("Emple");
            KeytiaBaseField lFieldRel = pFields.GetByConfigName("Sitio - Empleados");

            if (lField.DataValue.ToString().Replace("'", "") != "null")
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                Hashtable lhtFields = new Hashtable();
                lhtFields.Add("{Sitio}", iCodCatalogo);
                lhtFields.Add("{Emple}", lField.DataValue.ToString().Replace("'", ""));

                Hashtable lhtValues = Util.TraducirRelacion("Sitio - Empleados", lhtFields);
                lhtValues.Add("dtIniVigencia", pdtIniVigencia.DataValue);
                lhtValues.Add("dtFinVigencia", pdtFinVigencia.DataValue);
                lhtValues.Add("iCodRelacion", lFieldRel.ConfigValue);
                lhtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                psbQuery.Length = 0;
                psbQuery.AppendLine("select iCodRegistro = isnull (Max(icodregistro),0)");
                psbQuery.AppendLine("from [VisRelaciones('Sitio - Empleados','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where Emple = " + lField.DataValue.ToString().Replace("'", ""));
                psbQuery.AppendLine("And Sitio = " + iCodCatalogo);
                psbQuery.AppendLine("and dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("and dtFinVigencia <= " + pdtFinVigencia.DataValue);

                int liCodRegistroRel = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                if (liCodRegistroRel == 0)
                {
                    //Mandar llamar al COM para grabar los datos de la relacion
                    liCodRegistroRel = lCargasCOM.GuardaRelacion(lhtValues, "Sitio - Empleados", false, (int)Session["iCodUsuarioDB"]);
                    if (liCodRegistroRel < 0)
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                }
            }
        }

        protected void GrabarPermisos(string lsVchCodigoPerfil)
        {
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            int liCodRegistro = -1;
            int liCodMaestro;
            Hashtable phtValuesPermisos = new Hashtable();
            string lsiCodCatalogo;
            //pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select icodcatalogo");
                psbQuery.AppendLine("from [VisHistoricos('Perfil','Perfiles','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where vchCodigo = '" + lsVchCodigoPerfil + "'");

                lsiCodCatalogo = (DSODataAccess.ExecuteScalar(psbQuery.ToString())).ToString();

                psbQuery.Length = 0;
                psbQuery.AppendLine("select icodregistro ");
                psbQuery.AppendLine("from maestros ");
                psbQuery.AppendLine("where icodentidad = ");
                psbQuery.AppendLine("(select icodregistro from catalogos where vchcodigo = 'RestriccionesPerfil' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia)");
                psbQuery.AppendLine("and vchdescripcion = 'Sitios'");
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");

                liCodMaestro = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                #region Obtener la fecha de inicio de vigencia más antigua, y la fecha de fin de vigencia más lejana
                string lsQueryVigencias = @"select 
                    (select MIN(dtIniVigencia) from Historicos where iCodCatalogo = @iCodCatalogo and dtIniVigencia <> dtFinVigencia ) dtIniVigencia, 
                    (select MAX(dtFinVigencia) from Historicos where iCodCatalogo = @iCodCatalogo and dtIniVigencia <> dtFinVigencia ) dtFinVigencia";
                DataTable ldtVigencias = DSODataAccess.Execute(lsQueryVigencias.Replace("@iCodCatalogo", iCodCatalogo));
                DateTime ldtIniVigencia = DateTime.Today;
                DateTime ldtFinVigencia = DateTime.Today;
                if (ldtVigencias != null && ldtVigencias.Rows.Count > 0)
                {
                    ldtIniVigencia = (DateTime)Util.IsDBNull(ldtVigencias.Rows[0]["dtIniVigencia"], DateTime.Today);
                    ldtFinVigencia = (DateTime)Util.IsDBNull(ldtVigencias.Rows[0]["dtFinVigencia"], DateTime.Today);
                }
                #endregion

                #region Armar el hash de la restricción
                //phtValuesPermisos.Add("iCodCatalogo01", lsiCodCatalogo);
                phtValuesPermisos.Add("{Perfil}", lsiCodCatalogo);
                //phtValuesPermisos.Add("iCodCatalogo02", iCodCatalogo);
                phtValuesPermisos.Add("{Sitio}", iCodCatalogo);
                //phtValuesPermisos.Add("Integer01", 0);
                phtValuesPermisos.Add("{PermisosEntidad}", 0);
                phtValuesPermisos.Add("iCodMaestro", liCodMaestro);
                phtValuesPermisos.Add("vchCodigo", "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");// pvchCodigo + "RestPerf" + lsPerfil + "'");
                phtValuesPermisos.Add("vchDescripcion", "'" + lsVchCodigoPerfil + " - " + pvchCodigo + "'");
                //phtValuesPermisos.Add("dtIniVigencia", pdtIniVigencia.Date);
                //phtValuesPermisos.Add("dtFinVigencia", pdtFinVigencia.Date);
                phtValuesPermisos.Add("dtIniVigencia", ldtIniVigencia);
                phtValuesPermisos.Add("dtFinVigencia", ldtFinVigencia);
                phtValuesPermisos.Add("iCodUsuario", Session["iCodUsuario"]);
                #endregion

                #region Buscar restricciones creadas para este sitio
                StringBuilder lsbWhere = new StringBuilder();
                lsbWhere.AppendLine("where iCodCatalogo01 = " + int.Parse(lsiCodCatalogo));
                lsbWhere.AppendLine("and iCodCatalogo02 = " + iCodCatalogo);
                lsbWhere.AppendLine("and iCodMaestro = " + liCodMaestro);
                lsbWhere.AppendLine("and dtIniVigencia <> dtFinVigencia");
                DataTable ldtRestricciones = DSODataAccess.Execute("select * from Historicos\r\n" + lsbWhere.ToString());
                #endregion

                #region Insertar o actualizar la restricción según el resultado de la búsqueda
                if (ldtRestricciones != null && ldtRestricciones.Rows.Count == 0)
                {
                    // Insertar restricción
                    liCodRegistro = lCargasCOM.InsertaRegistro(phtValuesPermisos, "Historicos", "RestriccionesPerfil", "Sitios", false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                }
                else if (ldtRestricciones != null 
                    && ldtRestricciones.Rows.Count == 1 
                    && ((int)Util.IsDBNull(ldtRestricciones.Rows[0]["Integer01"],0) & 2) != 2)
                {
                    // Actualizar restricción
                    liCodRegistro = 0;
                    int liCodRegistroRestriccion = (int)ldtRestricciones.Rows[0]["iCodRegistro"];
                    if (!lCargasCOM.ActualizaRegistro("Historicos", "RestriccionesPerfil", "Sitios", phtValuesPermisos, liCodRegistroRestriccion, false, (int)Session["iCodUsuarioDB"]))
                        throw new KeytiaWebException("ErrSaveRecord");
                }
                #endregion

                if (liCodRegistro < 0)
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        private void GrabarPlanesDeMarcacion()
        {
            KDBAccess kdb = new KDBAccess();
            System.Data.DataTable pdtAuxiliar = new System.Data.DataTable();
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            string lsCatLoclaidad;
            string lsCatEstado;
            string lsCatPais;
            System.Data.DataTable ldtPMarcacion;
            System.Data.DataTable ldtRelSitioPMarcacion;
            KeytiaRelationField lRelField;
            KeytiaBaseField lFieldEntidad;
            KeytiaBaseField lFieldPlanM;
            Hashtable lhtRel;
            int liCodRegistroRel;
            System.Data.DataRow[] pdrRelSitioPlanM;

            lsCatLoclaidad = pFields.GetByConfigName("Locali").DataValue.ToString();
            pdtAuxiliar = kdb.GetHisRegByEnt("Locali", "Localidades", "iCodCatalogo =" + lsCatLoclaidad);
            if (pdtAuxiliar == null || pdtAuxiliar.Rows.Count == 0)
            {
                return;
            }
            lsCatEstado = Util.IsDBNull(pdtAuxiliar.Rows[0]["{Estados}"], "").ToString();
            if (lsCatEstado == "")
            {
                return;
            }
            pdtAuxiliar = kdb.GetHisRegByEnt("Estados", "Estados", "iCodCatalogo =" + lsCatEstado);
            if (pdtAuxiliar == null || pdtAuxiliar.Rows.Count == 0)
            {
                return;
            }
            lsCatPais = Util.IsDBNull(pdtAuxiliar.Rows[0]["{Paises}"], "").ToString();
            if (lsCatPais == "")
            {
                return;
            }

            ldtPMarcacion = kdb.GetHisRegByEnt("PlanM", "Plan de Marcacion", "{Paises}=" + lsCatPais);
            if (ldtPMarcacion == null || ldtPMarcacion.Rows.Count == 0)
            {
                return;
            }
            ldtRelSitioPMarcacion = kdb.GetRelRegByDes("Sitio - Plan de Marcacion", "{Sitio}=" + iCodCatalogo.ToString());

            lRelField = ((KeytiaRelationField)pFields.GetByConfigName("Sitio - Plan de Marcacion"));
            lFieldEntidad = lRelField.Fields.GetByConfigValue(int.Parse(iCodEntidad));
            lFieldPlanM = lRelField.Fields.GetByConfigName("PlanM");

            foreach (System.Data.DataRow ldrPlanM in ldtPMarcacion.Rows)
            {
                pdrRelSitioPlanM = ldtRelSitioPMarcacion.Select("[{" + lFieldPlanM.ConfigName + "}] = " + ldrPlanM["iCodCatalogo"].ToString());
                if (pdrRelSitioPlanM != null && pdrRelSitioPlanM.Length > 0)
                {
                    //Ya existe una relación para éste Plan de Marcación
                    continue;
                }
               
                lhtRel = new Hashtable();
                lhtRel.Add("iCodRelacion", lRelField.ConfigValue);
                lhtRel.Add("iCodUsuario", Session["iCodUsuario"]);
                //lhtRel.Add("dtIniVigencia", DateTime.Today);
                lhtRel.Add("dtIniVigencia", pdtIniVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss")); //RJ.Cambio para que dé de alta la relación con los planes de marcación con la fecha inicio del sitio en lugar de la fecha actual.
                lhtRel.Add("dtFinVigencia", new DateTime(2079, 1, 1));
                lhtRel.Add(lFieldEntidad.Column, iCodCatalogo);
                lhtRel.Add(lFieldPlanM.Column, ldrPlanM["iCodCatalogo"]);

                //Mandar llamar al COM para grabar los datos de la relacion
                liCodRegistroRel = lCargasCOM.GuardaRelacion(lhtRel, lRelField.ConfigName, (int)Session["iCodUsuarioDB"]);
                if (liCodRegistroRel < 0)
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
            }
        }

        protected bool ValidarRecursos()
        {
            bool lbret = true;
            int liCount;
            int liCodRecurso;
            string lsError;
            string[] lsRecursos = { "Exten", "Linea", "CodAcc", "CodAuto" };
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloSitios"));

            try
            {
                foreach (string lsCodRecurso in lsRecursos)
                {
                    liCodRecurso = (int)DSODataAccess.ExecuteScalar("select icodregistro from catalogos where icodcatalogo is null and dtIniVigencia <> dtFinVigencia and vchcodigo = '" + lsCodRecurso + "'");
                    DataTable liCodMaestro = DSODataAccess.Execute("select vchdescripcion from maestros where icodentidad = " + liCodRecurso + " and dtIniVigencia <> dtFinVigencia");

                    foreach (DataRow drMaRec in liCodMaestro.Rows)
                    {
                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select Recursos = isnull(count(H.iCodRegistro),0)");
                        psbQuery.AppendLine("from [VisHistoricos('" + lsCodRecurso + "','" + drMaRec["vchdescripcion"].ToString() + "','" + Globals.GetCurrentLanguage() + "')] H");
                        psbQuery.AppendLine("where Sitio = " + iCodCatalogo);
                        psbQuery.AppendLine("and (H.dtIniVigencia < " + pdtIniVigencia.DataValue);
                        psbQuery.AppendLine("       or H.dtFinVigencia > " + pdtFinVigencia.DataValue + ")");

                        liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                        if (liCount > 0)
                        {
                            string[] param = { vchDescripcion.ToString(), drMaRec["vchdescripcion"].ToString() };
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RecAsoc", param));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                        }
                    }
                }
                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected bool ValidarDeta_Pend()
        {
            bool lbret = true;
            int liCount;
            string lsError;
            //string[] lsVistasVal = { "'Detalle Codigo Autorizacion'", "'Detalle Codigo de Acceso'", 
            //                         "'Detalle Extensiones'", "'Detalle Lineas'", "'DetalleCDR'",
            //                         "'Codigo AutorizacionPendiente'", "'Codigo de AccesoPendiente'",
            //                         "'ExtensionesPendiente'", "'LineasPendiente'" };

            string[] lsVistasVal = { "[VisDetallados('Detall','DetalleCDR','" + Globals.GetCurrentLanguage() + "')]",
                                     "[VisPendientes('Detall','DetalleCDR','" + Globals.GetCurrentLanguage() + "')]"};

            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloSitios"));

            try
            {
                foreach (string lsVistas in lsVistasVal)
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select Registros = isnull(count(H.iCodRegistro),0)");
                    psbQuery.AppendLine("from " + lsVistas + " H");
                    psbQuery.AppendLine("where H.Sitio = " + iCodCatalogo);
                    psbQuery.AppendLine("and H.FechaFin > " + pdtFinVigencia.DataValue);

                    liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                    if (liCount > 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "DetAsoc", vchDescripcion.ToString()));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected int ContieneRecursos()
        {
            int liCount = 0;
            int liCodRecurso;
            string[] lsRecursos = { "Exten", "Linea", "CodAcc", "CodAuto" };
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloSitios"));

            try
            {
                foreach (string lsCodRecurso in lsRecursos)
                {
                    liCodRecurso = (int)DSODataAccess.ExecuteScalar("select icodregistro from catalogos where icodcatalogo is null and dtIniVigencia <> dtFinVigencia and vchcodigo = '" + lsCodRecurso + "'");
                    DataTable liCodMaestro = DSODataAccess.Execute("select vchdescripcion from maestros where icodentidad = " + liCodRecurso + " and dtIniVigencia <> dtFinVigencia");

                    foreach (DataRow drMaRec in liCodMaestro.Rows)
                    {
                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select Recursos = isnull(count(H.iCodRegistro),0)");
                        psbQuery.AppendLine("from [VisHistoricos('" + lsCodRecurso + "','" + drMaRec["vchdescripcion"].ToString() + "','" + Globals.GetCurrentLanguage() + "')] H");
                        psbQuery.AppendLine("where H.Sitio = " + iCodCatalogo);
                        psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                        psbQuery.AppendLine("and H.dtFinVigencia > " + pdtFinVigencia.DataValue);

                        liCount += (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return liCount;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            pbtnNotifPrepSitio.InnerText = Globals.GetMsgWeb(false, "btnNotifPrepSitio");
            if (pFields != null)
            {
                if (pFields.ContainsConfigName("ExtIni"))
                {
                    DSONumberEdit ctlNumber = (DSONumberEdit)pFields.GetByConfigName("ExtIni").DSOControlDB;
                    ctlNumber.NumberBox.MaxLength = 9;
                }
                if (pFields.ContainsConfigName("ExtFin"))
                {
                    DSONumberEdit ctlNumber = (DSONumberEdit)pFields.GetByConfigName("ExtFin").DSOControlDB;
                    ctlNumber.NumberBox.MaxLength = 9;
                }
                if (pFields.ContainsConfigName("LongExt"))
                {
                    DSONumberEdit ctlNumber = (DSONumberEdit)pFields.GetByConfigName("LongExt").DSOControlDB;
                    ctlNumber.NumberBox.MaxLength = 9;
                }
                if (pFields.ContainsConfigName("LongCasilla"))
                {
                    DSONumberEdit ctlNumber = (DSONumberEdit)pFields.GetByConfigName("LongCasilla").DSOControlDB;
                    ctlNumber.NumberBox.MaxLength = 9;
                }
            }


        }

        protected override void CnfgSubHistoricField_PostGrabarClick(object sender, EventArgs e)
        {
            if (pSubHistorico is CnfgNotifPrepSitio
                && pSubHistorico.PrevState == HistoricState.Edicion
                && pSubHistorico.iCodCatalogo != "null"
                && (int.Parse(pSubHistorico.Fields.GetByConfigName("BanderasPrepSitios").DataValue.ToString()) & 2) == 2)
            {
                DateTime ldtAhora = DateTime.Now;
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("select iCodRegistro");
                lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('PrepSitio','Notificación por Presupuesto Temporal a Empleado','" + Globals.GetCurrentLanguage() + "')] PrepSitio");
                lsbQuery.AppendLine("where PrepSitio.NotifPrepSitio = " + pSubHistorico.iCodCatalogo);
                lsbQuery.AppendLine("and PrepSitio.dtIniVigencia <> PrepSitio.dtFinVigencia");
                lsbQuery.AppendLine("and PrepSitio.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("and PrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());
                if (lDataTable.Rows.Count == 0)
                {
                    pSubHistorico.SetHistoricState(pSubHistorico.PrevState);
                    ((CnfgNotifPrepSitio)pSubHistorico).ConfigPrepSitio();
                }
                else
                {
                    base.CnfgSubHistoricField_PostGrabarClick(sender, e);
                }
            }
            else
            {
                base.CnfgSubHistoricField_PostGrabarClick(sender, e);
            }
        }

        protected override void CnfgSubHistorico_PostCancelarClick(object sender, EventArgs e)
        {
            if (pSubHistorico is CnfgNotifPrepSitio
                && pSubHistorico.PrevState == HistoricState.Edicion
                && pSubHistorico.iCodCatalogo != "null"
                && (int.Parse(pSubHistorico.Fields.GetByConfigName("BanderasPrepSitios").DataValue.ToString()) & 2) == 2)
            {
                DateTime ldtAhora = DateTime.Now;
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("select iCodRegistro");
                lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('PrepSitio','Notificación por Presupuesto Temporal a Empleado','" + Globals.GetCurrentLanguage() + "')] PrepSitio");
                lsbQuery.AppendLine("where PrepSitio.NotifPrepSitio = " + pSubHistorico.iCodCatalogo);
                lsbQuery.AppendLine("and PrepSitio.dtIniVigencia <> PrepSitio.dtFinVigencia");
                lsbQuery.AppendLine("and PrepSitio.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("and PrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());
                if (lDataTable.Rows.Count == 0)
                {
                    pSubHistorico.SetHistoricState(pSubHistorico.PrevState);
                    ((CnfgNotifPrepSitio)pSubHistorico).ConfigPrepSitio();
                }
                else
                {
                    base.CnfgSubHistorico_PostCancelarClick(sender, e);
                }
            }
            else
            {
                base.CnfgSubHistorico_PostCancelarClick(sender, e);
            }
        }
    }

    public class CnfgSitiosRestEdit : CnfgSitiosEdit
    {
        protected override void InitGrid()
        {
            base.InitGrid();
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisRestData");
        }
    }
}