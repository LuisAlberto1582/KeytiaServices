/*
 * Nombre:		    DMM
 * Fecha:		    20110729
 * Descripción:	    Clase para realizar búsquedas genéricas en el sistema
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using KeytiaServiceBL;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Collections;
using DSOControls2008;
using System.Text.RegularExpressions;

namespace KeytiaWeb.UserInterface
{
    public class BusquedaGenerica
    {
        public enum Seccion
        {
            SecRecurs,
            SecConsumos,
            SecEmple
        }

        protected int piCodEmpleado;
        protected KDBAccess kdb = new KDBAccess();
        protected DataTable pdtResultados;
        protected Seccion pSeccionBusqueda;
        protected string psCadenaBuscada;
        protected bool pbColaboradores = false;

        public string CadenaBuscada
        {
            get { return psCadenaBuscada; }
            set { psCadenaBuscada = value; }
        }

        public Seccion SeccionBusqueda
        {
            get { return pSeccionBusqueda; }
            set { pSeccionBusqueda = value; }
        }

        public DataTable Resultados
        {
            get { return pdtResultados; }
        }

        public BusquedaGenerica()
        {
            pSeccionBusqueda = Seccion.SecEmple;
        }

        protected void LoadUserInfo()
        {
            int liCodPerfil = (int)HttpContext.Current.Session["iCodPerfil"];
            DataTable ldt = null;

            try
            {
                ldt = kdb.GetHisRegByEnt("Perfil", "",
                    "iCodCatalogo = " + liCodPerfil.ToString());
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloBusquedas");
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
            if (ldt != null && ldt.Rows.Count > 0)
            {
                if (!(ldt.Rows[0]["{BanderasPerfil}"] is DBNull))
                {
                    int liBanderasPerfil = (int)Util.IsDBNull(ldt.Rows[0]["{BanderasPerfil}"], 0);
                    pbColaboradores = (liBanderasPerfil & 1) == 1;
                }
            }
        }

        #region Procesar

        public DataTable Procesar(string lsCadenaBusqueda, Seccion sSeccionBusqueda)
        {
            psCadenaBuscada = lsCadenaBusqueda;
            pSeccionBusqueda = sSeccionBusqueda;
            return Procesar();
        }

        public DataTable Procesar()
        {
            LoadUserInfo();
            switch (pSeccionBusqueda)
            {
                case Seccion.SecEmple:
                    ProcesaEmpleados();
                    break;
                case Seccion.SecConsumos:
                    ProcesaConsumos();
                    break;
                case Seccion.SecRecurs:
                    ProcesaRecursos();
                    break;
            }
            return Resultados;
        }

        protected void ProcesaEmpleados()
        {
            if (string.IsNullOrEmpty(psCadenaBuscada))
            {
                newDataTableSeccion();
                return;
            }

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("GetBusquedaEmpleados");
            lsbQuery.AppendLine("	@Schema = '{0}',");
            lsbQuery.AppendLine("	@Idioma = '{1}',");
            lsbQuery.AppendLine("	@iCodUsuarioDB = {2},");
            lsbQuery.AppendLine("	@iCodUsuario = {3},");
            lsbQuery.AppendLine("	@iCodPerfil = {4},");
            lsbQuery.AppendLine("	@bColaboradores = '{5}',");
            lsbQuery.AppendLine("	@CadenaBuscada = '{6}'");

            try
            {
                pdtResultados = DSODataAccess.Execute(String.Format(
                    lsbQuery.ToString(),
                    DSODataContext.Schema,
                    Globals.GetCurrentLanguage(),
                    System.Web.HttpContext.Current.Session["iCodUsuarioDB"].ToString(),
                    System.Web.HttpContext.Current.Session["iCodUsuario"].ToString(),
                    System.Web.HttpContext.Current.Session["iCodPerfil"].ToString(),
                    pbColaboradores ? '1' : '0',
                    psCadenaBuscada));
            }
            catch (Exception ex)
            {
                newDataTableSeccion();
            }
        }

        protected void ProcesaRecursos()
        {
            if (string.IsNullOrEmpty(psCadenaBuscada))
            {
                newDataTableSeccion();
                return;
            }

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("GetBusquedaRecursos");
            lsbQuery.AppendLine("	@Schema = '{0}',");
            lsbQuery.AppendLine("	@Idioma = '{1}',");
            lsbQuery.AppendLine("	@iCodUsuarioDB = {2},");
            lsbQuery.AppendLine("	@iCodUsuario = {3},");
            lsbQuery.AppendLine("	@iCodPerfil = {4},");
            lsbQuery.AppendLine("	@bColaboradores = '{5}',");
            lsbQuery.AppendLine("	@CadenaBuscada = '{6}'");

            try
            {
                pdtResultados = DSODataAccess.Execute(String.Format(
                    lsbQuery.ToString(),
                    DSODataContext.Schema,
                    Globals.GetCurrentLanguage(),
                    System.Web.HttpContext.Current.Session["iCodUsuarioDB"].ToString(),
                    System.Web.HttpContext.Current.Session["iCodUsuario"].ToString(),
                    System.Web.HttpContext.Current.Session["iCodPerfil"].ToString(),
                    pbColaboradores ? '1' : '0',
                    psCadenaBuscada));
            }
            catch (Exception ex)
            {
                newDataTableSeccion();
            }
        }

        protected void ProcesaConsumos()
        {
            if (string.IsNullOrEmpty(psCadenaBuscada))
            {
                newDataTableSeccion();
                return;
            }

            getFechas();

            DateTime ldtFechaInicio = (DateTime)HttpContext.Current.Session["FechaIniRep"];
            DateTime ldtFechaFin = (DateTime)HttpContext.Current.Session["FechaFinRep"];
            ldtFechaFin = ldtFechaFin.AddDays(1);

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("GetBusquedaConsumos");
            lsbQuery.AppendLine("	@Schema = '{0}',");
            lsbQuery.AppendLine("	@Idioma = '{1}',");
            lsbQuery.AppendLine("	@iCodUsuario = {2},");
            lsbQuery.AppendLine("	@iCodPerfil = {3},");
            lsbQuery.AppendLine("	@Moneda = '{4}',");
            lsbQuery.AppendLine("	@dtFechaInicio = '{5}',");
            lsbQuery.AppendLine("	@dtFechaFin = '{6}',");
            lsbQuery.AppendLine("	@CadenaBuscada = '{7}'");

            try
            {
                pdtResultados = DSODataAccess.Execute(String.Format(
                    lsbQuery.ToString(),
                    DSODataContext.Schema,
                    Globals.GetCurrentLanguage(),
                    System.Web.HttpContext.Current.Session["iCodUsuario"].ToString(),
                    System.Web.HttpContext.Current.Session["iCodPerfil"].ToString(),
                    Globals.GetCurrentCurrency(),
                    ldtFechaInicio.ToString("yyyy-MM-dd"),
                    ldtFechaFin.ToString("yyyy-MM-dd"),
                    psCadenaBuscada));
            }
            catch (Exception ex)
            {
                newDataTableSeccion();
            }
        }

        protected void getFechas()
        {
            if (HttpContext.Current.Session["FechaIniRep"] == null || HttpContext.Current.Session["FechaFinRep"] == null)
            {
                StringBuilder psbQuery = new StringBuilder();
                psbQuery.AppendLine("select Fecha = isnull(max(FechaInicio),GetDate()) from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','" + Globals.GetCurrentLanguage() + "')] Detall");
                DateTime ldtMaxDetalle = (DateTime)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                HttpContext.Current.Session["FechaIniRep"] = ldtMaxDetalle.AddMonths(-1);
                HttpContext.Current.Session["FechaFinRep"] = ldtMaxDetalle;

                //int liDiaCorte = (int)kdb.ExecuteScalar("Client", "Clientes", "Select IsNull({DiaEtiquetacion},1) from Historicos where iCodCatalogo = " + piCodCliente.ToString() + " and '" + DateTime.Today.ToString("yyyy-MM-dd") + "' between dtIniVigencia and dtFinVigencia");
                //ldtFecIni = new DateTime(DateTime.Today.Year, DateTime.Today.Month, liDiaCorte);
                //if (ldtFecIni.CompareTo(DateTime.Today) > 0)
                //{
                //    ldtFecIni = ldtFecIni.AddMonths(-1);
                //}
                //ldtFecFin = DateTime.Today;
            }
        }

        protected void newDataTableSeccion()
        {
            switch (pSeccionBusqueda)
            {
                case Seccion.SecEmple:
                    pdtResultados = new DataTable();
                    pdtResultados.Columns.Add("Entidad");
                    pdtResultados.Columns.Add("Maestro");
                    pdtResultados.Columns.Add("Registro");
                    pdtResultados.Columns.Add("Consultar");
                    pdtResultados.Columns.Add("Nomina");
                    pdtResultados.Columns.Add("Descripcion");
                    pdtResultados.Columns.Add("Email");
                    pdtResultados.Columns.Add("Recurs");
                    pdtResultados.Columns.Add("Sitio");
                    pdtResultados.Columns.Add("CenCos");
                    pdtResultados.Columns.Add("Ubica");
                    break;
                case Seccion.SecConsumos:
                    pdtResultados = new DataTable();
                    pdtResultados.Columns.Add("Entidad");
                    pdtResultados.Columns.Add("Maestro");
                    pdtResultados.Columns.Add("Registro");
                    pdtResultados.Columns.Add("Consultar");
                    pdtResultados.Columns.Add("NomEntidad");
                    pdtResultados.Columns.Add("Descripcion");
                    pdtResultados.Columns.Add("Costo", typeof(double));
                    pdtResultados.Columns.Add("CostoFac", typeof(double));
                    pdtResultados.Columns.Add("Duracion", typeof(int));
                    pdtResultados.Columns.Add("Llamadas", typeof(int));
                    break;
                case Seccion.SecRecurs:
                    pdtResultados = new DataTable();
                    pdtResultados.Columns.Add("Entidad");
                    pdtResultados.Columns.Add("Maestro");
                    pdtResultados.Columns.Add("Registro");
                    pdtResultados.Columns.Add("Consultar");
                    pdtResultados.Columns.Add("Recurs");
                    pdtResultados.Columns.Add("Descripcion");
                    pdtResultados.Columns.Add("Sitio");
                    pdtResultados.Columns.Add("Emple");
                    pdtResultados.Columns.Add("Responsable");
                    pdtResultados.Columns.Add("EmpleResp");
                    pdtResultados.Columns.Add("Email");
                    pdtResultados.Columns.Add("CenCos");
                    break;
            }
        }

        #endregion

        public static string getDesRelacion(params int[] lstEntidades)
        {
            string lsDescripcion = "";
            string lsFK_Relaciones = RegEdit.getColsTable("iCodCatalogo", "Relaciones");
            string[] lstFK_Relaciones = lsFK_Relaciones.Split(',');
            StringBuilder lsQuery = new StringBuilder();

            lsQuery.Length = 0;
            lsQuery.AppendLine("Select vchDescripcion from Relaciones");
            lsQuery.AppendLine("where iCodRelacion is null");
            foreach (int liEntidad in lstEntidades)
            {
                lsQuery.AppendLine("and " + liEntidad + " in (" + lsFK_Relaciones + ")");
            }
            for (int i = lstEntidades.Length; i < lstFK_Relaciones.Length; i++)
            {
                lsQuery.AppendLine("and " + lstFK_Relaciones[i] + " is null");
            }
            DataTable ldt = DSODataAccess.Execute(lsQuery.ToString());

            if (ldt.Rows.Count > 0)
            {
                lsDescripcion = ldt.Rows[0]["vchDescripcion"].ToString();
            }
            return lsDescripcion;
        }

        public static string getDesRelacion(params string[] lstEntidades)
        {
            string lsDescripcion = "";
            string lsFK_Relaciones = RegEdit.getColsTable("iCodCatalogo", "Relaciones");
            string[] lstFK_Relaciones = lsFK_Relaciones.Split(',');
            System.Text.StringBuilder lsQuery = new System.Text.StringBuilder();

            lsQuery.Length = 0;
            lsQuery.AppendLine("Select vchDescripcion from Relaciones");
            lsQuery.AppendLine("where iCodRelacion is null");
            lsQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
            foreach (string lsEntidad in lstEntidades)
            {
                lsQuery.AppendLine("and (Select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = '" + lsEntidad + "') in (" + lsFK_Relaciones + ")");
            }
            for (int i = lstEntidades.Length; i < lstFK_Relaciones.Length; i++)
            {
                lsQuery.AppendLine("and " + lstFK_Relaciones[i] + " is null");
            }
            DataTable ldt = DSODataAccess.Execute(lsQuery.ToString());

            if (ldt.Rows.Count > 0)
            {
                lsDescripcion = ldt.Rows[0]["vchDescripcion"].ToString();
            }
            return lsDescripcion;
        }

        #region WebMethods

        public static DSOGridServerResponse BusquedaRecursos(DSOGridServerRequest gsRequest, string SessionID)
        {
            return GetBusquedaGenerica(gsRequest, Seccion.SecRecurs, SessionID);
        }

        public static DSOGridServerResponse BusquedaConsumos(DSOGridServerRequest gsRequest, string SessionID)
        {
            return GetBusquedaGenerica(gsRequest, Seccion.SecConsumos, SessionID);
        }

        public static DSOGridServerResponse BusquedaEmpleados(DSOGridServerRequest gsRequest, string SessionID)
        {
            return GetBusquedaGenerica(gsRequest, Seccion.SecEmple, SessionID);
        }

        protected static DSOGridServerResponse GetBusquedaGenerica(DSOGridServerRequest gsRequest, Seccion Sec, string SessionID)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloBusquedas");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                Dictionary<string, string> lColNumericFormat = new Dictionary<string, string>();
                Dictionary<string, IFormatProvider> lColFormatter = new Dictionary<string, IFormatProvider>();

                string lsOrderCol = "";
                string lsOrderDir = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];
                    switch (Sec)
                    {
                        case Seccion.SecRecurs:
                            switch (lsOrderCol)
                            {
                                case "Recurs":
                                case "Sitio":
                                case "Emple":
                                case "Responsable":
                                case "EmpleResp":
                                case "Email":
                                case "CenCos":
                                    break;
                                default:
                                    lsOrderCol = "Descripcion";
                                    break;
                            }
                            break;
                        case Seccion.SecConsumos:
                            switch (lsOrderCol)
                            {
                                case "Entidad":
                                case "Costo":
                                case "CostoFac":
                                case "Duracion":
                                case "Llamadas":
                                    break;
                                default:
                                    lsOrderCol = "Descripcion";
                                    break;
                            }
                            KeytiaCurrencyField currency = new KeytiaCurrencyField();
                            KeytiaIntegerField integer = new KeytiaIntegerField();

                            lColNumericFormat.Add("Costo", currency.StringFormat);
                            lColNumericFormat.Add("CostoFac", currency.StringFormat);
                            lColNumericFormat.Add("Duracion", integer.StringFormat);
                            lColNumericFormat.Add("Llamadas", integer.StringFormat);

                            lColFormatter.Add("Costo", currency.FormatInfo);
                            lColFormatter.Add("CostoFac", currency.FormatInfo);
                            lColFormatter.Add("Duracion", integer.FormatInfo);
                            lColFormatter.Add("Llamadas", integer.FormatInfo);
                            break;
                        case Seccion.SecEmple:
                            switch (lsOrderCol)
                            {
                                case "Nomina":
                                case "Email":
                                case "Recurs":
                                case "Sitio":
                                case "CenCos":
                                case "Ubica":
                                    break;
                                default:
                                    lsOrderCol = "Descripcion";
                                    break;
                            }
                            break;
                    }

                    if (gsRequest.sSortDir[0].ToLower() == "desc")
                    {
                        lsOrderDir = " desc";
                    }
                    else
                    {
                        lsOrderDir = " asc";
                    }
                }

                BusquedaGenerica Busqueda;
                DataTable dtResultados;
                if ((HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString() + "__SearchGlobal"] == null ||
                    (string)HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString() + "__SearchGlobal"] != gsRequest.sSearchGlobal.Trim()) ||
                    HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString()] == null)
                {
                    Busqueda = new BusquedaGenerica();
                    Busqueda.SeccionBusqueda = Sec;
                    Busqueda.CadenaBuscada = gsRequest.sSearchGlobal.Trim();
                    dtResultados = Busqueda.Procesar();
                    HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString()] = dtResultados;
                    HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString() + "__SearchGlobal"] = gsRequest.sSearchGlobal.Trim();
                }
                else
                {
                    dtResultados = (DataTable)HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString()];
                }
                DataRow[] Resultados = dtResultados.Select("", lsOrderCol + lsOrderDir);
                DataTable DisplayRecords = dtResultados.Clone();

                for (int liRow = gsRequest.iDisplayStart; liRow < Math.Min(dtResultados.Rows.Count, gsRequest.iDisplayStart + gsRequest.iDisplayLength); liRow++)
                {
                    DataRow dr = DisplayRecords.NewRow();
                    dr.ItemArray = Resultados[liRow].ItemArray;
                    DisplayRecords.Rows.Add(dr);
                }

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = Resultados.Length;
                lgsrRet.iTotalDisplayRecords = Resultados.Length;
                lgsrRet.sColumns = gsRequest.sColumns;
                lgsrRet.SetDataFromDataTable(DisplayRecords, lColNumericFormat, lColFormatter);
                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloBusquedas");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static void Busquedas_Unload(string SessionID)
        {
            for (Seccion Sec = Seccion.SecRecurs; Sec <= Seccion.SecEmple; Sec++)
            {
                HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString()] = null;
                HttpContext.Current.Session[SessionID + "__BusquedaGenerica__" + Sec.ToString() + "__SearchGlobal"] = null;
            }
        }

        #endregion
    }
}
