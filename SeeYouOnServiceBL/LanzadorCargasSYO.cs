/*
Nombre:		    Ruben Zavala
Fecha:		    20131029
Descripción:	Clase principal de monitoreo de cargas en SYO
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.IO.Compression;
using KeytiaServiceBL;

namespace SeeYouOnServiceBL
{
    public class LanzadorCargasSYO
    {
        #region Campos

        protected bool pbSigueCorriendo;
        protected KDBAccess kdb;

        #endregion


        #region Metodos

        /// <summary>
        /// Método inicial
        /// </summary>
        public void Start()
        {
            pbSigueCorriendo = true;

            if (kdb == null)
                kdb = new KDBAccess();



            while (pbSigueCorriendo)
            {
                //RZ.20131014 Iniciar un ping para el proceso KeytiaService
                //Pinger.StartPing("KeytiaService", 20);

                //Espera N seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < Util.TiempoPausa("Cargas") / 2 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);

                //Si recibió la señal de terminar sale del ciclo
                if (!pbSigueCorriendo)
                    break;

                //Adopta el esquema Keytia
                DSODataContext.SetContext(0);
                kdb.FechaVigencia = DateTime.Today;



                //Util.LogMessage("Inicia barrido de esquemas.");
                try
                {
                    if (kdb == null)
                        kdb = new KDBAccess();


                    //Obtiene todos los Usuarios DB que se encuentren en el sistema
                    string lsEntidad = "UsuarDB";
                    string lsMaestro = "Usuarios DB";
                    //RZ.20131029 Solo para esquema SeeYouOn
                    string lsEsquema = "SeeYouOn";

                    //RZ.20130820 Filtrar solo aquellos esquemas en donde la ip del servicio configurada coincida
                    DataTable ldtUsuarDB = kdb.GetHisRegByEnt(lsEntidad, lsMaestro, new string[] { "iCodCatalogo" }, "{Esquema} = '" + lsEsquema + "'");

                    if (ldtUsuarDB != null && ldtUsuarDB.Rows.Count > 0)
                    {

                        //Recorre uno a uno cada Usuario DB encontrado
                        foreach (DataRow ldrUsuarDB in ldtUsuarDB.Rows)
                        {

                            //Crea una instancia de la clase LanzadorCargasEsquema
                            //que es la que se encarga de buscar las cargas pendientes
                            LanzadorCargasEsquemaSYO loLC =
                                new LanzadorCargasEsquemaSYO((int)ldrUsuarDB["iCodCatalogo"]);

                            loLC.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.LogException(ex);
                }
                //Util.LogMessage("Barrido de esquemas.Fin\r\n\r\nEsquemas lanzados:\r\n" + lsbEsquemas.ToString());



                //Espera N seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < Util.TiempoPausa("Cargas") / 2 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);


            }

            //Pinger.StopPing();
        }

        public void Stop()
        {
            pbSigueCorriendo = false;
        }

        #endregion
    }



    public class LanzadorCargasEsquemaSYO
    {
        #region Campos

        //protected int piEstatusFinal = -1;
        protected int piEstatusInicial = -1;
        protected int piEstatusEsperaProceso = -1;
        protected int piEstatusErrorInesperado = -1;
        //protected int piEstatusArchEnSis1 = -1;
        //protected int piEntidadCargas = -1;
        //RZ.20130401 Se agrega variable para estatus Arch1NoFrmt
        protected int piEstatusArch1NoFrmt = -1;
        //RZ.20130426 Se agregan variables para estatus ErrEtiqueta y ErrElimPteDet
        //protected int piEstatusErrEtiqueta = -1;
        //protected int piEstatusErrElimPteDet = -1;
        protected int piUsuarioDB = -1;

        protected Hashtable phtMaestrosCargas;
        protected Hashtable phtMaestrosCargasInv;
        //protected Hashtable phtMaestrosCargasA;
        protected List<string> psMaestros = new List<string>();

        protected KDBAccess kdb;

        #endregion


        #region Constructores

        public LanzadorCargasEsquemaSYO(int liUsuarioDB)
        {
            piUsuarioDB = liUsuarioDB;
        }

        #endregion


        #region Metodos

        /// <summary>
        /// Método inicial. 
        /// Este es el método invocado inmediatamente despues de que se instancia la clase
        /// </summary>
        public void Start()
        {
            //Util.LogMessage("Inicia búsqueda y ejecución en '" + DSODataContext.Schema + "'.");

            try
            {
                DSODataContext.SetContext(piUsuarioDB);

                if (kdb == null)
                    kdb = new KDBAccess();


                //Obtiene el listado de Estatus de las cargas
                //y el listado de Maestros de la Entidad CargasA
                InitMaestros_Estatus();


                // Recorre una a una cada carga automática del esquema
                // Busca en cada directorio configurado en la carga si hay archivos pendientes de procesar
                // Inserta un registro en Historicos de Cargas, por cada archivo pendiente de procesar
                //RZ.20131029 Se retira la busqueda de cargas automaticas para SeeYouOnService
                //BuscarCargas();


                EjecutarCargas();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }

        /// <summary>
        /// Obtiene el listado de Estatus de las cargas
        /// Obtiene el listado de Maestros de la entidad CargasA
        /// </summary>
        public void InitMaestros_Estatus()
        {
            try
            {
                InitEstatus();  //Obtiene los icodcatalogos de los diferentes estatus de las cargas
                InitMaestros(); //Obtiene los maestros de las entidades Cargas y CargasA
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Obtiene los icodcatalogos de los diferentes estatus de las cargas
        /// </summary>
        public void InitEstatus()
        {
            object loAuxiliar = new object();

            //Establece el icodCatalogo del Estatus "Carga Inicializada"
            if ((loAuxiliar = DSODataContext.GetObject("piEstatusInicial")) != null)
            {
                piEstatusInicial = (int)loAuxiliar;
            }
            else
            {
                piEstatusInicial = (int)DSODataAccess.ExecuteScalar(
                    "select cat.iCodRegistro\r\n" +
                    "from   catalogos ent\r\n" +
                    "       inner join catalogos cat\r\n" +
                    "           on cat.iCodCatalogo = ent.iCodRegistro\r\n" +
                    "           and cat.vchCodigo = 'CarInicial'\r\n" +
                    "where  ent.vchCodigo = 'EstCarga'\r\n" +
                    "and    ent.dtIniVigencia <> ent.dtFinVigencia\r\n" +
                    "and    ent.iCodCatalogo is null\r\n",
                -1);
                DSODataContext.SetObject("piEstatusInicial", piEstatusInicial);
            }

            //Establece el icodCatalogo del Estatus "Error Inesperado"
            if ((loAuxiliar = DSODataContext.GetObject("piEstatusErrorInesperado")) != null)
            {
                piEstatusErrorInesperado = (int)loAuxiliar;
            }
            else
            {
                piEstatusErrorInesperado = (int)DSODataAccess.ExecuteScalar(
                    "select cat.iCodRegistro\r\n" +
                    "from   catalogos ent\r\n" +
                    "       inner join catalogos cat\r\n" +
                    "           on cat.iCodCatalogo = ent.iCodRegistro\r\n" +
                    "           and cat.vchCodigo = 'ErrInesp'\r\n" +
                    "where  ent.vchCodigo = 'EstCarga'\r\n" +
                    "and    ent.dtIniVigencia <> ent.dtFinVigencia\r\n" +
                    "and    ent.iCodCatalogo is null\r\n",
                -1);
                DSODataContext.SetObject("piEstatusErrorInesperado", piEstatusErrorInesperado);
            }

            //Establece el icodCatalogo del Estatus "Carga en Espera de Servicio"
            if ((loAuxiliar = DSODataContext.GetObject("piEstatusEsperaProceso")) != null)
            {
                piEstatusEsperaProceso = (int)loAuxiliar;
            }
            else
            {
                piEstatusEsperaProceso = (int)DSODataAccess.ExecuteScalar(
                    "select cat.iCodRegistro\r\n" +
                    "from   catalogos ent\r\n" +
                    "       inner join catalogos cat\r\n" +
                    "           on cat.iCodCatalogo = ent.iCodRegistro\r\n" +
                    "           and cat.vchCodigo = 'CarEspera'\r\n" +
                    "where  ent.vchCodigo = 'EstCarga'\r\n" +
                    "and    ent.dtIniVigencia <> ent.dtFinVigencia\r\n" +
                    "and    ent.iCodCatalogo is null\r\n",
                -1);
                DSODataContext.SetObject("piEstatusEsperaProceso", piEstatusEsperaProceso);
            }

            //Establece el icodCatalogo del Estatus "Carga Finalizada"
            //if ((loAuxiliar = DSODataContext.GetObject("piEstatusFinal")) != null)
            //{
            //    piEstatusFinal = (int)loAuxiliar;
            //}
            //else
            //{
            //    piEstatusFinal = (int)DSODataAccess.ExecuteScalar(
            //        "select cat.iCodRegistro\r\n" +
            //        "from   catalogos ent\r\n" +
            //        "       inner join catalogos cat\r\n" +
            //        "           on cat.iCodCatalogo = ent.iCodRegistro\r\n" +
            //        "           and cat.vchCodigo = 'CarFinal'\r\n" +
            //        "where  ent.vchCodigo = 'EstCarga'\r\n" +
            //        "and    ent.dtIniVigencia <> ent.dtFinVigencia\r\n" +
            //        "and    ent.iCodCatalogo is null\r\n",
            //        -1);
            //    DSODataContext.SetObject("piEstatusFinal", piEstatusFinal);
            //}

            //Establece el icodCatalogo del Estatus "Archivo 1 previamente cargado en sistema"
            //if ((loAuxiliar = DSODataContext.GetObject("piEstatusArchEnSis1")) != null)
            //{
            //    piEstatusArchEnSis1 = (int)loAuxiliar;
            //}
            //else
            //{
            //    piEstatusArchEnSis1 = (int)DSODataAccess.ExecuteScalar(
            //        "select cat.iCodRegistro\r\n" +
            //        "from   catalogos ent\r\n" +
            //        "       inner join catalogos cat\r\n" +
            //        "           on cat.iCodCatalogo = ent.iCodRegistro\r\n" +
            //        "           and cat.vchCodigo = 'ArchEnSis1'\r\n" +
            //        "where  ent.vchCodigo = 'EstCarga'\r\n" +
            //        "and    ent.dtIniVigencia <> ent.dtFinVigencia\r\n" +
            //        "and    ent.iCodCatalogo is null\r\n",
            //    -1);
            //    DSODataContext.SetObject("piEstatusArchEnSis1", piEstatusArchEnSis1);
            //}

            //if ((loAuxiliar = DSODataContext.GetObject("piEntidadCargas")) != null)
            //{
            //    piEntidadCargas = (int)loAuxiliar;
            //}
            //else
            //{
            //    piEntidadCargas = (int)DSODataAccess.ExecuteScalar(
            //        "select iCodRegistro\r\n" +
            //        "from Catalogos\r\n" +
            //        "where vchCodigo = 'Cargas'\r\n" +
            //        "and iCodCatalogo is null\r\n" +
            //        "and dtIniVigencia <> dtFinVigencia\r\n",
            //        -1);
            //    DSODataContext.SetObject("piEntidadCargas", piEntidadCargas);
            //}

            /*RZ.20130401 Se agrega estatus ErrElimPteDet para validar en las cargas y mover a backup*/
            //Establece el icodCatalogo del Estatus "Carga Finalizada. Errores en proceso de etiquetación. Eliminación de Detallados y Pendientes Fallida"
            //if ((loAuxiliar = DSODataContext.GetObject("piEstatusErrElimPteDet")) != null)
            //{
            //    piEstatusErrElimPteDet = (int)loAuxiliar;
            //}
            //else
            //{
            //    piEstatusErrElimPteDet = (int)DSODataAccess.ExecuteScalar(
            //        "select cat.iCodRegistro\r\n" +
            //        "from   catalogos ent\r\n" +
            //        "       inner join catalogos cat\r\n" +
            //        "           on cat.iCodCatalogo = ent.iCodRegistro\r\n" +
            //        "           and cat.vchCodigo = 'ErrElimPteDet'\r\n" +
            //        "where  ent.vchCodigo = 'EstCarga'\r\n" +
            //        "and    ent.dtIniVigencia <> ent.dtFinVigencia\r\n" +
            //        "and    ent.iCodCatalogo is null\r\n",
            //    -1);
            //    DSODataContext.SetObject("piEstatusErrElimPteDet", piEstatusErrElimPteDet);
            //}


            /*RZ.20130426 Se agrega estatus ErrEtiqueta para validar en las cargas y mover a backup*/
            //Establece el icodCatalogo del Estatus "Carga Finalizada. Se detectaron errores en el proceso de etiquetación"
            //if ((loAuxiliar = DSODataContext.GetObject("piEstatusErrEtiqueta")) != null)
            //{
            //    piEstatusErrEtiqueta = (int)loAuxiliar;
            //}
            //else
            //{
            //    piEstatusErrEtiqueta = (int)DSODataAccess.ExecuteScalar(
            //        "select cat.iCodRegistro\r\n" +
            //        "from   catalogos ent\r\n" +
            //        "       inner join catalogos cat\r\n" +
            //        "           on cat.iCodCatalogo = ent.iCodRegistro\r\n" +
            //        "           and cat.vchCodigo = 'ErrEtiqueta'\r\n" +
            //        "where  ent.vchCodigo = 'EstCarga'\r\n" +
            //        "and    ent.dtIniVigencia <> ent.dtFinVigencia\r\n" +
            //        "and    ent.iCodCatalogo is null\r\n",
            //    -1);
            //    DSODataContext.SetObject("piEstatusErrEtiqueta", piEstatusErrEtiqueta);
            //}
        }

        /// <summary>
        /// Obtiene los maestros de las entidades Cargas y CargasA
        /// </summary>
        public void InitMaestros()
        {
            //KDBAccess kdb = new KDBAccess();
            DataTable ldtMaestrosCargas;
            //DataTable ldtMaestrosCargasA;
            DataTable ldtMaestrosCargasInv;

            object loAuxiliar = new object();


            //Obtiene los maestros de la entidad Cargas
            if ((loAuxiliar = DSODataContext.GetObject("phtMaestrosCargas")) != null)
            {
                phtMaestrosCargas = (Hashtable)loAuxiliar;
            }
            else
            {
                phtMaestrosCargas = new Hashtable();
                ldtMaestrosCargas = kdb.GetMaeRegByEnt("Cargas");
                if (ldtMaestrosCargas != null)
                    foreach (DataRow ldr in ldtMaestrosCargas.Rows)
                        phtMaestrosCargas.Add(ldr["iCodRegistro"], ldr["vchDescripcion"]);
                DSODataContext.SetObject("phtMaestrosCargas", phtMaestrosCargas);
            }


            //Obtiene los maestros de la entidad CargasA
            //if ((loAuxiliar = DSODataContext.GetObject("phtMaestrosCargasA")) != null)
            //{
            //    phtMaestrosCargasA = (Hashtable)loAuxiliar;
            //}
            //else
            //{
            //    phtMaestrosCargasA = new Hashtable();
            //    ldtMaestrosCargasA = kdb.GetMaeRegByEnt("CargasA");
            //    if (ldtMaestrosCargasA != null)
            //        foreach (DataRow ldr in ldtMaestrosCargasA.Rows)
            //            phtMaestrosCargasA.Add(ldr["iCodRegistro"], ldr["vchDescripcion"]);
            //    DSODataContext.SetObject("phtMaestrosCargasA", phtMaestrosCargasA);
            //}


            //Obtiene los maestros de la entidad Cargas
            if ((loAuxiliar = DSODataContext.GetObject("phtMaestrosCargasInv")) != null)
            {
                phtMaestrosCargasInv = (Hashtable)loAuxiliar;
            }
            else
            {
                phtMaestrosCargasInv = new Hashtable();
                ldtMaestrosCargasInv = kdb.GetMaeRegByEnt("Cargas");
                if (ldtMaestrosCargasInv != null)
                    foreach (DataRow ldr in ldtMaestrosCargasInv.Rows)
                        phtMaestrosCargasInv.Add(ldr["vchDescripcion"], ldr["iCodRegistro"]);
                DSODataContext.SetObject("phtMaestrosCargasInv", phtMaestrosCargasInv);
            }



            //Agrega a la lista psMaestros cada Maestro encontrado
            //if (phtMaestrosCargasA != null)
            //    foreach (string lsMae in phtMaestrosCargasA.Values)
            //        psMaestros.Add(lsMae);

        }

        /// <summary>
        /// Revisa que exista el maestro en el esquema
        /// </summary>
        /// <param name="iCodMaestro"></param>
        public void AsegurarExisteMaestro(int iCodMaestro)
        {
            if (!phtMaestrosCargas.ContainsKey(iCodMaestro) && !phtMaestrosCargasInv.ContainsValue(iCodMaestro))
                InitMaestros();

            if (!phtMaestrosCargas.ContainsKey(iCodMaestro) && !phtMaestrosCargasInv.ContainsValue(iCodMaestro))
                throw new Exception("No se encontró el maestro con iCodRegistro " + iCodMaestro);
        }

        /// <summary>
        /// Obtiene las cargas que se encuentren en el estatus recibido como parametro
        /// </summary>
        /// <param name="estatusCarga">iCodCatalogo del estatus de la carga</param>
        /// <returns>DataTable con el listado de cargas obtenidas en la consulta</returns>
        public DataTable ObtenerCargas(int estatusCarga)
        {
            DataTable listadoCargas = null;

            listadoCargas = kdb.GetHisRegByEnt("Cargas", "",
                    new string[] { "iCodMaestro", "iCodCatalogo", "iCodRegistro", "{Clase}" },
                    "{EstCarga} = " + estatusCarga.ToString());


            return listadoCargas;
        }

        /// <summary>
        /// Obtiene las cargas que se encuentren en el estatus "En espera de servicio"
        /// 
        /// </summary>
        public void EjecutarCargas()
        {
            DataTable ldtCargasPorProcesar = null;
            Thread ltThread = null;
            //RZ.20131030 Se retira instancia de CargaServicio por CargaServicioSYO
            CargaServicioSYO loCarga = null;
            //CargasSYO.CargaConferenciasProgramadas loCarga;
            bool lbHayError = false;

            try
            {
                //Obtiene las cargas que se encuentren con el estatus "En espera de servicio"
                ldtCargasPorProcesar = ObtenerCargas(piEstatusEsperaProceso);
            }
            catch (Exception ex)
            {
                Util.LogException("Error al obtener los registros de carga.", ex);
                return;
            }

            if (ldtCargasPorProcesar != null)
            {
                foreach (DataRow ldrCarga in ldtCargasPorProcesar.Rows)
                {
                    lbHayError = false;
                    loCarga = null;

                    AsegurarExisteMaestro((int)ldrCarga["iCodMaestro"]);

                    if (ValidaDisponibilidadArchivos(
                        (int)ldrCarga["iCodCatalogo"],
                        (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]]))
                    {
                        Util.LogMessage("Trabajando con la carga (Cargas:" +
                            (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]] + ":" +
                                (int)ldrCarga["iCodCatalogo"] + ":" +
                                (int)ldrCarga["iCodRegistro"] + ")\r\n" +
                                ldrCarga["{Clase}"]);
                        try
                        {
                            //RZ.20131112 Solo si la carga contiene SeeYouOnServiceBL se instanciara
                            if (ldrCarga["{Clase}"].ToString().Contains("SeeYouOnServiceBL"))
                            {
                                //RZ.20131030 Se retira instancia de CargaServicio por CargaServicioSYO
                                loCarga = (CargaServicioSYO)System.Activator.CreateInstanceFrom(
                                    System.Reflection.Assembly.GetExecutingAssembly().CodeBase,
                                    (string)ldrCarga["{Clase}"]).Unwrap();
                            }

                            //loCarga = new SeeYouOnServiceBL.CargasSYO.CargaConferenciasProgramadas();
                        }
                        catch (Exception ex)
                        {
                            lbHayError = true;

                            Util.LogException("Error al instanciar la clase de la carga (Cargas:" +
                                (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]] + ":" +
                                (int)ldrCarga["iCodCatalogo"] + ":" +
                                (int)ldrCarga["iCodRegistro"] + ")\r\n" +
                                ldrCarga["{Clase}"],
                                ex);
                        }
                    }
                    if (loCarga != null)
                    {
                        Util.LogMessage("Inicio de carga (Cargas:" +
                            (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]] + ":" +
                            (int)ldrCarga["iCodCatalogo"] + ":" +
                            (int)ldrCarga["iCodRegistro"] + ")");

                        Hashtable lhtVal = new Hashtable();
                        lhtVal.Add("{EstCarga}", piEstatusInicial);
                        kdb.Update("Historicos", "Cargas", (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]], lhtVal, (int)ldrCarga["iCodRegistro"]);

                        loCarga.CodCarga = (int)ldrCarga["iCodCatalogo"];
                        loCarga.CodUsuarioDB = piUsuarioDB;
                        loCarga.Maestro = (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]];

                        try
                        {
                            ltThread = new Thread(loCarga.Main);
                            ltThread.Start();
                            Util.LogMessage("Thread de la carga inicializado.");
                        }
                        catch (Exception ex)
                        {
                            lbHayError = true;

                            Util.LogException("Error al iniciar la carga (Cargas:" +
                                (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]] + ":" +
                                (int)ldrCarga["iCodCatalogo"] + ":" +
                                (int)ldrCarga["iCodRegistro"] + ")",
                                ex);
                        }
                    }

                    if (lbHayError)
                    {
                        Hashtable lhtVal = new Hashtable();
                        lhtVal.Add("{EstCarga}", piEstatusErrorInesperado);
                        lhtVal.Add("dtFecUltAct", DateTime.Now);

                        kdb.Update("Historicos", "Cargas", (string)phtMaestrosCargas[ldrCarga["iCodMaestro"]], lhtVal, (int)ldrCarga["iCodRegistro"]);
                    }
                }
            }
        }

        public bool ValidaDisponibilidadArchivos(int liCodCarga, string lsMaestro)
        {
            bool lbRet = true;
            Hashtable lhtCampos = kdb.CamposHis("Cargas", lsMaestro);
            ArrayList laArchivos = new ArrayList();

            laArchivos.Add("iCodCatalogo");
            laArchivos.Add("{Archivo01}");
            if (((Hashtable)lhtCampos["Todos"]).ContainsKey("{Archivo02}")) laArchivos.Add("{Archivo02}");
            if (((Hashtable)lhtCampos["Todos"]).ContainsKey("{Archivo03}")) laArchivos.Add("{Archivo03}");
            if (((Hashtable)lhtCampos["Todos"]).ContainsKey("{Archivo04}")) laArchivos.Add("{Archivo04}");
            if (((Hashtable)lhtCampos["Todos"]).ContainsKey("{Archivo05}")) laArchivos.Add("{Archivo05}");

            DataTable ldtConf = DSODataAccess.Execute(
                "select *\r\n" +
                "from   (" + kdb.GetQueryHis(lhtCampos, (string[])laArchivos.ToArray(Type.GetType("System.String")), "", "", "") + ") a\r\n" +
                "where  iCodCatalogo = " + liCodCarga);

            if (ldtConf != null && ldtConf.Rows.Count > 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (ldtConf.Columns.Contains("{Archivo" + i.ToString().PadLeft(2, '0') + "}") &&
                        (string)Util.IsDBNull(ldtConf.Rows[0]["{Archivo" + i.ToString().PadLeft(2, '0') + "}"], "") != "" &&
                        !ValidaDisponibilidadArchivo((string)ldtConf.Rows[0]["{Archivo" + i.ToString().PadLeft(2, '0') + "}"]))
                    {
                        lbRet = false;
                        break;
                    }
                }
            }

            return lbRet;
        }

        public bool ValidaDisponibilidadArchivo(string lsArchivo)
        {
            bool lbRet = true;
            StreamReader lsrFileTest = null;

            try
            {
                lsrFileTest = new StreamReader(lsArchivo);
            }
            catch (Exception ex)
            {
                lbRet = false;
                Util.LogException("El archivo '" + lsArchivo + "' aún no está disponible para procesar.", ex);
            }
            finally
            {
                if (lsrFileTest != null)
                    lsrFileTest.Close();
            }

            return lbRet;
        }
        #endregion
    }
}
