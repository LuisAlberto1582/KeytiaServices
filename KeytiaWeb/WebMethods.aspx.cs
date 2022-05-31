using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using KeytiaWeb.UserInterface;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeytiaWeb
{
    [DataContract]
    public class Parametro
    {
        private string psName;

        private object poValue;

        [DataMember(Name = "name")]
        public string Name
        {
            get
            {
                return psName;
            }
            set
            {
                psName = value;
            }
        }

        [DataMember(Name = "value")]
        public object Value
        {
            get
            {
                return poValue;
            }
            set
            {
                poValue = value;
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    poValue = "null";
                }
                else if (value is string && value.ToString().StartsWith("/Date(")
                    && value.ToString().EndsWith(")/"))
                {
                    poValue = DSOControl.DeserializeJSON<DateTime>("\"" + value.ToString().Replace("/", "\\/") + "\"");
                }
            }
        }
    }

    public partial class WebMethods : KeytiaPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod()]
        public static string ChangePassword(string lsCurrentPassword, string lsNewPassword, string lsNewPasswordConf, string wnd)
        {
            return Usuarios.ChangePassword(lsCurrentPassword, lsNewPassword, lsNewPasswordConf, wnd);
            //return null;
        }

        [WebMethod()]

        //++**HD 
        public static string UsrComment(string lsCommentName, string lsCommentCorreo, string lscboAsunto, string lsCommentComentario, string wnd)
        {
            return Usuarios.UsrComment(lsCommentName, lsCommentCorreo, lscboAsunto, lsCommentComentario, wnd);
            //return null;
        }

        #region Catalogos
        [WebMethod(EnableSession = true)]
        public static string GetCatReg(int iCodRegistro)
        {
            return CatalogEdit.GetCatReg(iCodRegistro);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetCatData(DSOGridServerRequest gsRequest)
        {
            return CatalogEdit.GetCatData(gsRequest);
        }
        #endregion

        #region Maestros y Relaciones

        [WebMethod(EnableSession = true)]
        public static string GetMaestros(string term, int iCodEntidad)
        {
            return RegEdit.GetMaestros(term, iCodEntidad);
        }

        [WebMethod(EnableSession = true)]
        public static string GetRelaciones(string term)
        {
            return RegEdit.GetRelaciones(term);
        }

        [WebMethod(EnableSession = true)]
        public static string GetDataSource(string lsNombreTabla, string tipoCampo, string iCodEntidad, string Excluir)
        {
            return RegEdit.GetDataSource(lsNombreTabla, tipoCampo, iCodEntidad, Excluir);
        }

        #endregion

        #region Historicos

        [WebMethod(EnableSession = true)]
        public static string GetHisMaestros(string iCodCatalogo)
        {
            return HistoricEdit.GetHisMaestros(iCodCatalogo);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchCatReg(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return HistoricEdit.SearchCatReg(term, iCodEntidad, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchRecursoRest(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return EmpleRelationFieldCollection.SearchRecursoRest(term, iCodEntidad, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchDataSourceRep(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return HistoricEdit.SearchDataSourceRep(term, iCodEntidad, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static bool ValidaVigenciasAutoComplete(int iCodCatalogo, int iCodEntidad, string restrictedValues, object iniVigencia, object finVigencia)
        {
            return KeytiaAutoCompleteField.ValidaVigenciasAutoComplete(iCodCatalogo, iCodEntidad, (string.IsNullOrEmpty(restrictedValues) ? 0 : 1), iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static bool ValidaVigenciasAtribAutoComplete(int iCodCatalogo, object iniVigencia, object finVigencia)
        {
            return KeytiaAutoCompleteField.ValidaVigenciasAtribAutoComplete(iCodCatalogo, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchEntity(string term)
        {
            return HistoricEdit.SearchEntity(term);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchAttribute(string term, int iCodEntidad)
        {
            return HistoricEdit.SearchAttribute(term, iCodEntidad);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchConsulta(string term, int iCodAtributo)
        {
            return CnfgValorConsultasEdit.SearchConsulta(term, iCodAtributo);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchAtribControl(string term, int iCodType)
        {
            return CnfgAtributos.SearchAtribControl(term, iCodType);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchEmpreEmple(string term, int iCodEmpre, object iniVigencia, object finVigencia)
        {
            return CnfgNotifPrepEmpre.SearchEmpreEmple(term, iCodEmpre, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetValorConsultasData(DSOGridServerRequest gsRequest, int iCodAtributo, int iCodEntidad, int iCodMaestro)
        {
            return CnfgValorConsultasEdit.GetValorConsultasData(gsRequest, iCodAtributo, iCodEntidad, iCodMaestro);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchCatRestricted(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return HistoricEdit.SearchCatRestricted(term, iCodEntidad, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchCatFiltered(string term, int iCodEntidad, string keytiaFilter, object iniVigencia, object finVigencia)
        {
            return HistoricEdit.SearchCatFiltered(term, iCodEntidad, keytiaFilter, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetMultiSelectData(DSOGridServerRequest gsRequest, int iCodEntidad, int bSelTodos, string jsonSeleccionados, int enableField, object iniVigencia, object finVigencia)
        {
            return HistoricEdit.GetMultiSelectData(gsRequest, iCodEntidad, bSelTodos, jsonSeleccionados, enableField, false, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetMultiSelectRestData(DSOGridServerRequest gsRequest, int iCodEntidad, int bSelTodos, string jsonSeleccionados, int enableField, object iniVigencia, object finVigencia)
        {
            return HistoricEdit.GetMultiSelectData(gsRequest, iCodEntidad, bSelTodos, jsonSeleccionados, enableField, true, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetRelData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodRelacion, int iCodCatalogo, string jsonData, string vchDescripcion)
        {
            return HistoricEdit.GetRelData(gsRequest, iCodEntidad, iCodRelacion, iCodCatalogo, jsonData, vchDescripcion);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetEmpleRelData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodRelacion, int iCodCatalogo, string jsonData, string vchDescripcion)
        {
            return CnfgEmpleadosFieldCollection.GetEmpleRelData(gsRequest, iCodEntidad, iCodRelacion, iCodCatalogo, jsonData, vchDescripcion);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return HistoricEdit.GetHisData(gsRequest, iCodEntidad, iCodMaestro);
        }
        //PT.20140410 Cambio para el configurador de empleados
        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisDataEmple(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return HistoricEdit.GetHisDataEmple(gsRequest, iCodEntidad, iCodMaestro);
        }
        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisRestData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return HistoricEdit.GetHisRestData(gsRequest, iCodEntidad, iCodMaestro);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetVisHistorico(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return HistoricEdit.GetVisHistorico(gsRequest, iCodEntidad, iCodMaestro);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetVisHistoricoParam(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return HistoricEdit.GetVisHistorico(gsRequest, iCodEntidad, iCodMaestro, DateTime.Now, parametros);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetVisHistoricoVigParam(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, object dtVigencia, List<Parametro> parametros)
        {
            return HistoricEdit.GetVisHistorico(gsRequest, iCodEntidad, iCodMaestro, dtVigencia, parametros);
        }


        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetSubHisData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return HistoricEdit.GetSubHisData(gsRequest, iCodEntidad, iCodMaestro, parametros);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetSubFullHisData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return HistoricEdit.GetSubHisData(gsRequest, iCodEntidad, iCodMaestro, null, parametros);
        }

        #endregion

        #region CargasWeb

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisDetallados(DSOGridServerRequest gsRequest, int liTipoDeta, int liCodCarga, int liCodMaestro)
        {
            return CargasWebEdit.GetHisDetallados(gsRequest, liTipoDeta, liCodCarga, liCodMaestro);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisPendientes(DSOGridServerRequest gsRequest, int iCodCarga, int iCodMaestroPend)
        {
            return CargasWebEdit.GetHisPendientes(gsRequest, iCodCarga, iCodMaestroPend);
        }

        #endregion

        #region Busquedas

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse BusquedaRecursos(DSOGridServerRequest gsRequest, string SessionID)
        {
            return BusquedaGenerica.BusquedaRecursos(gsRequest, SessionID);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse BusquedaConsumos(DSOGridServerRequest gsRequest, string SessionID)
        {
            return BusquedaGenerica.BusquedaConsumos(gsRequest, SessionID);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse BusquedaEmpleados(DSOGridServerRequest gsRequest, string SessionID)
        {
            return BusquedaGenerica.BusquedaEmpleados(gsRequest, SessionID);
        }

        [WebMethod(EnableSession = true)]
        public static void Busquedas_Unload(string SessionID)
        {
            BusquedaGenerica.Busquedas_Unload(SessionID);
        }

        #endregion

        #region Etiqueta

        [WebMethod(EnableSession = true)]
        public static string GetConsumoResumen(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, DateTime ldtCorte, DateTime ldtLimite)
        {
            return EtiquetaEdit.GetConsumoResumen(liCodEmpleado, ldtIniPeriodo, ldtFinPeriodo, ldtCorte, ldtLimite);
        }

        [WebMethod(EnableSession = true)]
        public static int IsPeriodoValido(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, DateTime ldtCorte, DateTime ldtLimite)
        {
            return EtiquetaEdit.IsPeriodoValido(liCodEmpleado, ldtIniPeriodo, ldtFinPeriodo, ldtCorte, ldtLimite);
        }

        [WebMethod(EnableSession = true)]
        public static void SetResumen(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, int liHisPrevEtq, int liCodMaestroResum, int liCodEntidadResum)
        {
            EtiquetaEdit.SetResumen(liCodEmpleado, ldtIniPeriodo, ldtFinPeriodo, liHisPrevEtq, liCodMaestroResum, liCodEntidadResum);
        }

        [WebMethod(EnableSession = true)]
        public static void SetAllGrupo(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, int liCodMaestroResum, int liCodEntidadResum, int liCheck, int liValor)
        {
            EtiquetaEdit.SetAllGrupo(liCodEmpleado, ldtIniPeriodo, ldtFinPeriodo, liCodMaestroResum, liCodEntidadResum, liCheck, liValor);
        }

        [WebMethod(EnableSession = true)]
        public static void UpdateRowResumen(int liCodEmpleado, int liCodMaestroResum, int liCodEntidadResum, int liRegistro, string lsCampo, string lsValor)
        {
            EtiquetaEdit.UpdateRowResumen(liCodEmpleado, liCodMaestroResum, liCodEntidadResum, liRegistro, lsCampo, lsValor);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetResumen(DSOGridServerRequest gsRequest, int liCodEmpleado, int liCodMaestroResum, int liCodEntidad, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, DateTime ldtCorte, DateTime ldtLimite, int liHisPrevEtq, int liSetResumen)
        {
            return EtiquetaEdit.GetResumen(gsRequest, liCodEmpleado, liCodMaestroResum, liCodEntidad, ldtIniPeriodo, ldtFinPeriodo, ldtCorte, ldtLimite, liHisPrevEtq, liSetResumen);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetEtiquetaDet(DSOGridServerRequest gsRequest, int liCodEmpleado, int liCodMaestro, int liCodEntidad, string lsLinea, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo)
        {
            return EtiquetaDetalle.GetEtiquetaDet(gsRequest, liCodEmpleado, liCodMaestro, liCodEntidad, lsLinea, ldtIniPeriodo, ldtFinPeriodo);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisEmple(DSOGridServerRequest gsRequest, int liCodEntidad, int liCodMaestro, int liCodEmpleado)
        {
            return EtiquetaDetalle.GetHisEmple(gsRequest, liCodEntidad, liCodMaestro, liCodEmpleado);
        }

        #endregion

        #region Reporte Estandar

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetRepTabular(DSOGridServerRequest gsRequest, int iCodReporte, int iNumeroRegistros, List<Parametro> parametros)
        {
            return ReporteEstandar.GetRepTabular(gsRequest, iCodReporte, iNumeroRegistros, parametros);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetRepResumido(DSOGridServerRequest gsRequest, int iCodReporte, int iNumeroRegistros, List<Parametro> parametros)
        {
            return ReporteEstandar.GetRepResumido(gsRequest, iCodReporte, iNumeroRegistros, parametros);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetRepMatricial(DSOGridServerRequest gsRequest, int iCodReporte, int iNumeroRegistros, List<Parametro> parametros)
        {
            return ReporteEstandar.GetRepMatricial(gsRequest, iCodReporte, iNumeroRegistros, parametros);
        }

        #endregion

        #region Directorios

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisDataDirEmple(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return CnfgDirectoriosEdit.GetHisDataDirEmple(gsRequest, iCodEntidad, iCodMaestro);
        }

        #endregion

        #region Cartas Custodia

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisRecAceptados(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return CartasCustodia.GetHisRecAceptados(gsRequest, iCodEntidad, iCodMaestro);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisRecPend(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return CartasCustodia.GetHisRecPend(gsRequest, iCodEntidad, iCodMaestro, parametros);
        }

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetHisRecPendLiberar(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return CartasCustodia.GetHisRecPendLiberar(gsRequest, iCodEntidad, iCodMaestro, parametros);
        }

        #endregion

        #region Dashboard

        [WebMethod(EnableSession = true)]
        public static string SearchConsultaDashboard(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return CnfgBloqueDashboard.SearchConsultaDashboard(term, iCodEntidad, iniVigencia, finVigencia);
        }

        #endregion

        #region Recursos

        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetSubRecursosData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return CnfgRecursosFieldCollection.GetSubRecursosData(gsRequest, iCodEntidad, iCodMaestro, parametros);
        }

        [WebMethod(EnableSession = true)]
        public static string GetSitiosConmutadores(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return CnfgSubRecursosFieldCollection.GetSitiosConmutadores(term, iCodEntidad, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static string GetSitiosNoConmutadores(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return CnfgSubRecursosFieldCollection.GetSitiosNoConmutadores(term, iCodEntidad, iniVigencia, finVigencia);
        }

        #endregion

        #region Reporte Usuario

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetReportesBase(string term)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess kdb = new KDBAccess();
                DataTable dt;

                dt = kdb.GetHisRegByEnt("RepUsu", "Reportes Base",
                    new string[] { "iCodCatalogo", "vchDescripcion" },
                    "vchDescripcion like '%" + term + "%'");

                if (dt != null)
                {
                    dt.Columns["iCodCatalogo"].ColumnName = "id";
                    dt.Columns["vchDescripcion"].ColumnName = "value";
                }

                return DSOControl.SerializeJSON<DataTable>(dt);
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetReportesUsuario(string term)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess kdb = new KDBAccess();
                DataTable dt = null;
                DataRow dr;
                string lsRet = "";

                if (System.Web.HttpContext.Current.Request.Params["rb"] != null &&
                    System.Web.HttpContext.Current.Request.Params["rb"] != "")
                {
                    dt = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario",
                        new string[] { "iCodRegistro", "vchDescripcion" },
                        "vchDescripcion like '%" + term + "%' and {RepUsu} = " +
                        System.Web.HttpContext.Current.Request.Params["rb"]);

                    if (dt != null)
                    {
                        dt.Columns["iCodRegistro"].ColumnName = "id";
                        dt.Columns["vchDescripcion"].ColumnName = "value";

                        dr = dt.NewRow();
                        dr["id"] = -1;
                        dr["value"] = Globals.GetLangItem("MsgWeb", "Mensajes Reporte Usuario", "RUNuevo");

                        dt.Rows.InsertAt(dr, 0);
                    }
                }

                if (dt != null)
                    lsRet = DSOControl.SerializeJSON<DataTable>(dt);

                return lsRet;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string GetReports()
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess kdb = new KDBAccess();
                DataTable dt = null;

                dt = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario",
                    new string[] { "iCodRegistro", "iCodCatalogo", "vchDescripcion", "{Descripcion}", "{RepUsuAcceso}" },
                    "{RepUsuAcceso} = '" + KDBUtil.SearchICodCatalogo("RepUsuAcceso", "publico", true) + "' or " +
                        "({RepUsuAcceso} = '" + KDBUtil.SearchICodCatalogo("RepUsuAcceso", "privado", true) + "' " +
                        "and iCodUsuario = " + System.Web.HttpContext.Current.Session["iCodUsuario"] + ")",
                    "{RepUsuAcceso},{Descripcion}");

                KDBUtil.AddEntityFields(dt, "RepUsuAcceso", new string[] { "{" + Globals.GetLanguage() + "}" });
                dt.Columns["{RepUsuAcceso}.{" + Globals.GetLanguage() + "}"].ColumnName = "{RepUsuAcceso}.vchDescripcion";

                return DSOControl.SerializeJSON<DataTable>(dt);
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        #endregion

        #region Alarmas
        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetSubHisDataAlarmas(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return CnfgAlarmasEdit.GetSubHisDataAlarmas(gsRequest, iCodEntidad, iCodMaestro, parametros);
        }
        #endregion

        #region EtiquetaVCS
        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetSubHisDataEtiquetaVCS(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return CnfgEtiquetaVCS.GetSubHisDataEtiquetaVCS(gsRequest, iCodEntidad, iCodMaestro, parametros);
        }
        #endregion
        #region See You On
        [WebMethod(EnableSession = true)]
        public static string SearchSeeYouOnServicioConferencia(string term, int iCodCliente, object iniVigencia, object finVigencia)
        {
            return CnfgConferencias.SearchSeeYouOnServicioConferencia(term, iCodCliente, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchSeeYouOnProyectoConferencia(string term, int iCodCliente, object iniVigencia, object finVigencia)
        {
            return CnfgConferencias.SearchSeeYouOnProyectoConferencia(term, iCodCliente, iniVigencia, finVigencia);
        }

        [WebMethod(EnableSession = true)]
        public static string SearchSeeYouOnPhoneBookContact(string term, int iCodConferencia)
        {
            return CnfgConferencias.SearchSeeYouOnPhoneBookContact(term, iCodConferencia);
        }
        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetSeeYouOnConferencias(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return CnfgConferencias.GetSeeYouOnConferencias(gsRequest, iCodEntidad, iCodMaestro);
        }
        [WebMethod(EnableSession = true)]
        public static DSOGridServerResponse GetSeeYouOnProyectos(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return CnfgProyectos.GetSeeYouOnProyectos(gsRequest, iCodEntidad, iCodMaestro);
        }
        [WebMethod(EnableSession = true)]
        public static string SearchEmpleByClient(string term, string iCodCliente, object iniVigencia, object finVigencia)
        {
            return CnfgAsistentes.SearchEmpleByClient(term, iCodCliente, iniVigencia, finVigencia);
        }

        #endregion
    }
}
