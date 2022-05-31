using AjaxControlToolkit;
using DSOControls2008;
using KeytiaServiceBL;
using KeytiaServiceBL.Reportes;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DashboardRepCFiltros : System.Web.UI.Page
    {
        private string fechaIniRep = DateTime.Now.AddMonths(-1 * 5).ToString("yyyy-MM-01 00:00:00");
        private string fechaFinRep = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
        private string tipoCampaniaRep1 = "Todas";
        private string tipoCampaniaRep2 = "Todas";
        private string tipoCampaniaRep3 = "Todas";
        private int campania = 0;
        private int campaniaN2 = 0;
        private string codRep = "";
        private string formatoFecha = "Case When Month >= 10 Then Convert(varchar,Month) Else (''0''+ convert(varchar,Month)) End  + ''/'' + convert(varchar,year)";
        private string formatoFechaInt = "convert(varchar,year)+ Case When month >= 10 Then (convert(varchar,month)) Else (''0''+Convert(varchar,Month)) End ";
        private string linkGraf = "[link] =";

        private string fechaQS = "";
        private int mesQS = 0;
        private int anioQS = 0;
        private int diaQS = 0;
        private string filtroTipoCampaniaR1QS = "";
        private string filtroTipoCampaniaR2QS = "";
        private string campaniaQS = "";
        private string codRepQS = "";

        private RoutingSessionHelper objRoutingSessionHelper = new RoutingSessionHelper();

        #region eventos
        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            #region Almacenar en variable de sesion los urls de navegacion
            List<string> list = new List<string>();
            string lsURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            if (Session["pltNavegacionDashFCRepCFiltros"] != null) //Entonces ya tiene navegacion almacenada
            {
                list = (List<string>)Session["pltNavegacionDashFCRepCFiltros"];
            }

            //20141114 AM. Se agrega condicion para eliminar querystring ?Opc=XXXXX
            if (lsURL.Contains("?Opc="))
            {
                //Asegurarse eliminar navegacion previa
                list.Clear();

                //Le quita el parametro Opc=XXXX
                lsURL = lsURL.Substring(0, lsURL.IndexOf("?Opc="));
            }


            //20150226 RJ.Se agrega la validación para determinar si se elimina
            //o no el valor que contengan las variables de sesion 
            //ESV(Empty Session Variables) es igual a 1 sólo cuando proviene de la liga del menú
            if (lsURL.Contains("?esv=1"))
            {
                //Vacía las variables de sesion de fechas 
                //para que más adelante se llenen con la fecha de la última factura cargada
                Session["FechaIniRep"] = null;
                Session["FechaFinRep"] = null;
            }


            //Si el url no contiene querystring y la lista tiene urls hay que limpiar la lista
            if (!(lsURL.Contains("?")) && list.Count > 0)
            {
                //Asegurarse eliminar navegacion previa
                list.Clear();
            }

            //Si no existe entonces quiere decir que estoy en un nuevo nivel de navegacion
            if (!list.Exists(element => element == lsURL))
            {
                //Agregar el valor del url actual para almacenarlo en la lista de navegacion
                list.Add(lsURL);
            }

            //Guardar en sesion la nueva lista
            Session["pltNavegacionDashFCRepCFiltros"] = list;

            //Ocultar boton de regresar cuando solo exista un elemento en la lista
            if (list.Count <= 1)
            {
                btnRegresar.Visible = false;


            }
            else
            {
                btnRegresar.Visible = true;


            }
            #endregion

            LeerQueryString();

            if (codRepQS.Length == 0)
            {
                pnlMapaNav.Visible = false;
                if (!IsPostBack)
                {
                    CargarFiltros();
                    CargarFiltrosQS();
                }
                else
                {
                    //CargarFiltros();

                    objRoutingSessionHelper = CrearRoutingObject("", "");

                    if
                      (
                          (
                              (objRoutingSessionHelper.S1FiltroTipoCampaña != null) &&
                              (objRoutingSessionHelper.S2FiltroTipoCampaña != null) &&
                              (objRoutingSessionHelper.S2FiltroCampaña != null)
                          ) &&
                          (
                              (filtroTipoCampaniaR1QS.ToString().ToLower() != objRoutingSessionHelper.S1FiltroTipoCampaña.ToString().ToLower()) ||
                              (filtroTipoCampaniaR2QS.ToString().ToLower() != objRoutingSessionHelper.S2FiltroTipoCampaña.ToString().ToLower()) ||
                              (campaniaQS.ToString().ToLower() != objRoutingSessionHelper.S3FiltroCampaña.ToString().ToLower())
                          )
                      )
                    {
                        CargarFiltrosSessionObj(objRoutingSessionHelper);
                    }
                    else
                    {
                        CargarFiltrosQS();
                    }
                }


            }
            else
            {
                pnlMapaNav.Visible = true;
            }
            CargarReportes();


            LimpiarQS();

        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {

            //List<string> ltNavegacion = (List<string>)Session["pltNavegacionDashFCRepCFiltros"];

            ////obtener el numero actual de elementos de la lista
            //string lsCantidadElem = ltNavegacion.Count.ToString();
            ////eliminar el ultimos elemento de la lista
            //ltNavegacion.RemoveAt(ltNavegacion.Count - 1);
            ////ltNavegacion.RemoveAt(ltNavegacion.Count - 1);

            ////obtener el ultimo elemento de la lista
            //string lsLastElement = ltNavegacion[ltNavegacion.Count - 1];

            string lsLastElement = Request.Path;
            HttpContext.Current.Response.Redirect(lsLastElement);
        }


        protected void btnAplicarFiltrosRep1_Click(object sender, EventArgs e)
        {
            try
            {

                Rep7.Controls.Clear();
                Rep8.Controls.Clear();

                //string filtroTipoCampañaRep1 = "";
                //string Campaña = "";
                //string filtroTipoCampañaRep2 = "";


                //filtroTipoCampañaRep1 = ddlTiposCampaniaRep1.SelectedItem.Text.ToString();
                //filtroTipoCampañaRep2 = ddlTiposCampaniaRep2.SelectedItem.Text.ToString();
                //Campaña = ddlcampanias.SelectedItem.Value.ToString();

                //Response.Redirect(Request.Path + "?filtroTipoCampR1="+filtroTipoCampañaRep1+"&filtroTipoCampR2="+filtroTipoCampañaRep2+"T&camp="+Campaña+"");


                //  fecha=05/2018&filtroTipoCampR1=Todas&filtroTipoCampR2=Todas&camp=201942
                //CargarReportes();              
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void btnAplicarFiltorsRep2_Click(object sender, EventArgs e)
        {
            try
            {
                Rep7.Controls.Clear();
                Rep8.Controls.Clear();

                //string filtroTipoCampañaRep1 = "";
                //string Campaña = "";
                //string filtroTipoCampañaRep2 = "";


                //filtroTipoCampañaRep1 = ddlTiposCampaniaRep1.SelectedItem.Text.ToString();
                //filtroTipoCampañaRep2 = ddlTiposCampaniaRep2.SelectedItem.Text.ToString();
                //Campaña = ddlcampanias.SelectedItem.Value.ToString();

                //Response.Redirect(Request.Path + "?filtroTipoCampR1=" + filtroTipoCampañaRep1 + "&filtroTipoCampR2=" + filtroTipoCampañaRep2 + "T&camp=" + Campaña + "");

                //CargarReportes();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        public void CargarFiltrosQS()
        {
            try
            {
                if (filtroTipoCampaniaR1QS.Length > 0)
                {
                    ddlTiposCampaniaRep1.SelectedIndex = ddlTiposCampaniaRep1.Items.IndexOf(ddlTiposCampaniaRep1.Items.FindByText(filtroTipoCampaniaR1QS));
                }

                if (filtroTipoCampaniaR2QS.Length > 0)
                {
                    ddlTiposCampaniaRep2.SelectedIndex = ddlTiposCampaniaRep2.Items.IndexOf(ddlTiposCampaniaRep2.Items.FindByText(filtroTipoCampaniaR2QS));
                }

                if (Convert.ToInt32(campaniaN2) > 0)
                {
                    ddlcampanias.SelectedIndex = ddlcampanias.Items.IndexOf(ddlcampanias.Items.FindByValue(campaniaN2.ToString()));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void LimpiarQS()
        {
            try
            {
                anioQS = 0;
                mesQS = 0;
                diaQS = 0;
                fechaQS = "";
                filtroTipoCampaniaR1QS = "";
                filtroTipoCampaniaR2QS = "";
                campaniaQS = "";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void LeerQueryString()
        {


            if (!string.IsNullOrEmpty(Request.QueryString["fecha"]))
            {
                try
                {
                    fechaQS = Request.QueryString["fecha"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (fecha) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                fechaQS = string.Empty;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["filtroTipoCampR1"]))
            {
                try
                {
                    filtroTipoCampaniaR1QS = Request.QueryString["filtroTipoCampR1"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (filtroTipoCampR1) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                filtroTipoCampaniaR1QS = string.Empty;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["filtroTipoCampR2"]))
            {
                try
                {
                    filtroTipoCampaniaR2QS = Request.QueryString["filtroTipoCampR2"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (filtroTipoCampR2) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                filtroTipoCampaniaR2QS = string.Empty;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["camp"]))
            {
                try
                {
                    campaniaQS = Request.QueryString["camp"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (camp) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                campaniaQS = string.Empty;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["codRep"]))
            {
                try
                {
                    codRepQS = Request.QueryString["codRep"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (codRep) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                codRepQS = string.Empty;
            }



            if (fechaQS.Length > 0)
            {
                string[] camposFecha = fechaQS.Split('/');

                int.TryParse(camposFecha[0], out mesQS);
                int.TryParse(camposFecha[1], out  anioQS);
            }

            if (campaniaQS.Length > 0 && filtroTipoCampaniaR1QS.Length > 0 && filtroTipoCampaniaR2QS.Length > 0)
            {

                if (Convert.ToInt32(campaniaQS) > 0)
                {
                    campaniaN2 = Convert.ToInt32(campaniaQS);
                }
                else
                {
                    campaniaN2 = 0;
                }

                if (filtroTipoCampaniaR1QS.Length > 0)
                {
                    tipoCampaniaRep3 = filtroTipoCampaniaR1QS;
                }
                else
                {
                    tipoCampaniaRep3 = "Todas";
                }

                if (filtroTipoCampaniaR2QS.Length > 0)
                {
                    tipoCampaniaRep3 = filtroTipoCampaniaR2QS;
                }
                else
                {
                    tipoCampaniaRep3 = "Todas";
                }
            }

        }

        public bool CargarReportes()
        {
            try
            {
                bool respuesta = false;



                DropDownList ddlTC1 = FindControl("ddlTiposCampaniaRep1") as DropDownList;
                DropDownList ddlTCC2 = FindControl("ddlTiposCampaniaRep2") as DropDownList;
                if (codRepQS.Length == 0 || (ddlTCC2 != null && ddlTC1 != null))
                {

                    btnExportarXLS.Visible = false;
                    btnExportarXLS.Enabled = false;
                    //Rep1
                    tipoCampaniaRep1 = ddlTiposCampaniaRep1.Text;

                    //Rep 2
                    tipoCampaniaRep2 = ddlTiposCampaniaRep2.Text;
                    campania = Convert.ToInt32(ddlcampanias.SelectedValue.ToString());

                    CargarReportesSeccion(tipoCampaniaRep1, tipoCampaniaRep2, campania);
                }
                else
                {


                    btnExportarXLS.Visible = true;
                    btnExportarXLS.Enabled = true;
                    if (fechaQS.Length > 0)
                    {
                        string[] camposFecha = fechaQS.Split('/');

                        int.TryParse(camposFecha[0], out mesQS);
                        int.TryParse(camposFecha[1], out  anioQS);
                    }

                    tipoCampaniaRep1 = filtroTipoCampaniaR1QS;
                    tipoCampaniaRep2 = filtroTipoCampaniaR2QS;
                    campania = Convert.ToInt32(campaniaQS);
                    CargarRepSencillo2Pnl(codRepQS, tipoCampaniaRep1, tipoCampaniaRep2, campania);
                }


                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarRepSencillo2Pnl(string CodRep, string tipoCampaniaRep1, string tipoCampaniaRep2, int campania)
        {
            try
            {
                bool respuesta = false;


                string tituloGrid = "";
                string tituloGraf = "";
                string tipoGrafDefault = "";
                string idContenedorGraf = "";
                string tipoCampania = "";

                string link = "";
                string linkLupa = "";

                DataTable dtRep = null;

                Rep1.Controls.Clear();
                Rep2.Controls.Clear();
                Rep3.Controls.Clear();
                Rep4.Controls.Clear();

                panelFiltrosRep1.Controls.Clear();
                panelFiltrosRep2.Controls.Clear();

                List<string> listaSeries = new List<string>();

                string codRep = "[%PlaceHolderCodRep%]";
                bool showValue = false;
                bool showPercentage = false;

                switch (codRepQS)
                {
                    case "TC":
                        //CargarReporteTipoCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1, 0, formatoFecha, formatoFechaInt, link, linkLupa);

                        BuscarFormatosFechas(6, DateTime.Now);
                        dtRep = RepTipoCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1, 0, formatoFecha, formatoFechaInt));

                        DataTable dtRepConsumoEnlacesMensual = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtRep);
                        DataView dtvRepConsumoEnlacesMensual = new DataView(dtRepConsumoEnlacesMensual);

                        DataTable dtRepAcumulado = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtRepConsumoEnlacesMensual, dtRep, dtRep.Clone());

                        foreach (DataColumn column in dtRep.Columns)
                        {
                            if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                            {
                                listaSeries.Add(column.ColumnName);
                            }
                        }


                        tituloGrid = "Por tipo de campaña";
                        tituloGraf = "Por tipo de campaña ";
                        idContenedorGraf = "rep1GrafTipoCampania";
                        tipoGrafDefault = "msline";

                        Rep3.Controls.Clear();
                        Rep3.Controls.Add(CrearControlGridConsumoMensualEnlaces(dtRepConsumoEnlacesMensual, "Consumo mensual enlaces"));


                        string codRepTC = "[%PlaceHolderCodRep%]";
                        string linkEnlacesAumuladoTC = Request.Path + "?fecha={2}&CodRep=" + codRepTC + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                        linkEnlacesAumuladoTC = linkEnlacesAumuladoTC.Replace("[%PlaceHolderCodRep%]", "TCEAD");
                        link = linkEnlacesAumuladoTC;

                        string linkLupaEnlacesAcumuladoTC = Request.Path + "?CodRep=" + codRepTC + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                        linkLupaEnlacesAcumuladoTC = linkLupaEnlacesAcumuladoTC.Replace("[%PlaceHolderCodRep%]", "TCEA");
                        linkLupa = linkLupaEnlacesAcumuladoTC;

                        Rep4.Controls.Clear();
                        Rep4.Controls.Add(CrearRepTipoCampaniaTabContainer(dtRepAcumulado, "Consumo mensual enlaces acumulado", "Consumo mensual enlaces acumulado", 3, "rep4GrafTipoCampaniaElacesAcumulado", link, linkLupa, true, true));

                        link = "";
                        linkLupa = "";

                        break;
                    case "TCC":

                        BuscarFormatosFechas(6, DateTime.Now);
                        dtRep = RepCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep2, campania, formatoFecha, formatoFechaInt));

                        DataTable dtRepConsumoEnlacesPorCampaña = RepPorCampañaFormatoDataTableEnlacesPorCampaña(BuscarInfoEnlacesporCampaña(fechaIniRep, fechaFinRep, campania), dtRep);

                        DataView dtvClone = new DataView(dtRepConsumoEnlacesPorCampaña);
                        List<string> listaColumnasDeseadas = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesPorCampaña).ToList().Where(r => !r.Contains(" ")).Select(r => r).ToList();

                        DataTable dtRepAcumuladoPorCampaña = RepPorCampañaFormatoDataTableEnlacesPorCampañaAcumulado(dtRepConsumoEnlacesPorCampaña, dtRep, dtvClone.ToTable(false, listaColumnasDeseadas.ToArray()));

                        foreach (DataColumn column in dtRep.Columns)
                        {
                            if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                            {
                                listaSeries.Add(column.ColumnName);
                            }
                        }

                        tituloGrid = "Por campaña";
                        tituloGraf = "Por campaña";
                        idContenedorGraf = "Rep2GrafTipoCamp";
                        tipoGrafDefault = "msline";

                        if (dtRepConsumoEnlacesPorCampaña.Columns.Count <= 5)
                        {
                            dtRepConsumoEnlacesPorCampaña.Clear();
                        }

                        if (dtRepAcumuladoPorCampaña.Columns.Count <= 5)
                        {
                            dtRepAcumuladoPorCampaña.Clear();
                        }


                        Rep3.Controls.Clear();
                        Rep3.Controls.Add(CrearControlGridConsumoEnlacesPorCampaña(dtRepConsumoEnlacesPorCampaña, "Consumo enlaces por campaña"));

                        Rep4.Controls.Clear();
                        Rep4.Controls.Add(CrearControlGridConsumoEnlacesPorCampañaAcumulado(dtRepAcumuladoPorCampaña, "Consumo enlaces por campaña acumulado"));



                        break;
                    case "TCD":

                        DateTime dT = new DateTime(anioQS, mesQS, 1);
                        BuscarFormatosFechas(1, dT);
                        dtRep = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt);


                        string[] camposFecha = fechaIniRep.Split('-');
                        DateTime dtFechaIniRep = new DateTime(Convert.ToInt32(camposFecha[0]), Convert.ToInt32(camposFecha[1]), 1);

                         DataTable dtPresupuestosporMes = new DataTable();

                        dtPresupuestosporMes = BuscaInfoPptoPorCampaña(fechaIniRep, Convert.ToInt32(campaniaQS));


                        foreach (DataRow rowdtRepTipoCampaña in dtRep.Rows)
                        {
                            if (!dtRep.Columns.Contains("Presupuesto"))
                            {
                                dtRep.Columns.Add("Presupuesto", typeof(float));
                            }
                            dtRep.AcceptChanges();

                            foreach (DataRow rowPpto in dtPresupuestosporMes.Rows)
                            {
                                if (dtRep.Columns.Contains("FechaInt") && dtPresupuestosporMes.Columns.Contains("FechaInt"))
                                {
                                    if (rowdtRepTipoCampaña["FechaInt"].ToString() == rowPpto["FechaInt"].ToString())
                                    {
                                        rowdtRepTipoCampaña["Presupuesto"] = Convert.ToDouble(rowPpto["pptoDiario"]);
                                    }
                                }
                            }
                        }

                        tituloGrid = "Por tipo de campaña de " + dtFechaIniRep.ToString("MMMM") + "";
                        tituloGraf = "Por tipo de campaña";
                        idContenedorGraf = "rep4GrafTipoCampania";
                        tipoGrafDefault = "msstackedcolumn2dlinedy";
                        break;

                    case "TCDA":
                        DateTime dTAcumulado = new DateTime(anioQS, mesQS, 1);
                        BuscarFormatosFechas(1, dTAcumulado);
                        dtRep = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt);

                        string[] camposFechaAcumulado = fechaIniRep.Split('-');
                        DateTime dtFechaIniRepAcumulado = new DateTime(Convert.ToInt32(camposFechaAcumulado[0]), Convert.ToInt32(camposFechaAcumulado[1]), 1);
                        dtRep = FormatoAcumuladoDT(dtRep);

                        tituloGrid = "Por tipo de campaña acumulado de " + dtFechaIniRepAcumulado.ToString("MMMM") + "";
                        tituloGraf = "Por tipo de campaña acumulado de " + dtFechaIniRepAcumulado.ToString("MMMM") + "";
                        idContenedorGraf = "rep8GrafTipoCampania";
                        tipoGrafDefault = "msstackedcolumn2dlinedy";
                        break;

                    case "TCEA":
                        BuscarFormatosFechas(6, DateTime.Now);
                        DataTable dtRepEA = RepTipoCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1, 0, formatoFecha, formatoFechaInt));

                        DataTable dtRepConsumoEnlacesMensualEA = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtRepEA);
                        DataView dtvRepConsumoEnlacesMensualEA = new DataView(dtRepConsumoEnlacesMensualEA);

                        DataTable dtRepAcumuladoEA = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtRepConsumoEnlacesMensualEA, dtRepEA, dtRepEA.Clone());

                        dtRep = dtRepAcumuladoEA;

                        foreach (DataColumn column in dtRepEA.Columns)
                        {
                            if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                            {
                                listaSeries.Add(column.ColumnName);
                            }
                        }

                        string[] camposFechaAcumuladoEA = fechaIniRep.Split('-');
                        DateTime dtFechaIniRepAcumuladoEA = new DateTime(Convert.ToInt32(camposFechaAcumuladoEA[0]), Convert.ToInt32(camposFechaAcumuladoEA[1]), 1);
                        //dtRep = FormatoAcumuladoDT(dtRep);

                        tituloGrid = "Consumo mensual enlaces acumulado";
                        tituloGraf = "Consumo mensual enlaces acumulado";
                        idContenedorGraf = "rep8GrafTipoCampania";
                        tipoGrafDefault = "msstackedcolumn2d";


                        string linkEnlacesAumulado = Request.Path + "?fecha={2}&CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                        linkEnlacesAumulado = linkEnlacesAumulado.Replace("[%PlaceHolderCodRep%]", "TCEAD");
                        link = linkEnlacesAumulado;

                        showValue = true;
                        showPercentage = true;



                        break;

                    case "TCEAD":
                        DateTime dTEAD = new DateTime(anioQS, mesQS, 1);
                        BuscarFormatosFechas(1, dTEAD);
                        dtRep = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt);

                        string[] camposFechaEAD = fechaIniRep.Split('-');
                        DateTime dtFechaIniRepEAD = new DateTime(Convert.ToInt32(camposFechaEAD[0]), Convert.ToInt32(camposFechaEAD[1]), 1);

                        DataTable dtRepConsumoEnlacesMensualEAD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtRep);
                        DataView dtvRepConsumoEnlacesMensualEAD = new DataView(dtRepConsumoEnlacesMensualEAD);

                        int i = 0;

                        DataTable dtCloneEnlacesEAD = dtRepConsumoEnlacesMensualEAD.Clone();
                        DataRow rowEnlace = dtRepConsumoEnlacesMensualEAD.Rows[0];

                        foreach (DataColumn column in dtRepConsumoEnlacesMensualEAD.Columns)
                        {
                            if (
                                column.ColumnName.ToLower() != "fecha" &&
                                column.ColumnName.ToLower() != "fechaint" &&
                                column.ColumnName.ToLower() != "link"
                               )
                            {
                                rowEnlace[column] = Convert.ToDouble(rowEnlace[column]) / dtRep.Rows.Count;
                            }
                        }

                        foreach (DataRow row in dtRep.Rows)
                        {
                            string fechaintEnlaceAED = "";
                            string fechaEnlace = "";

                            fechaintEnlaceAED = row["FechaInt"].ToString();
                            fechaEnlace = row["Fecha"].ToString();

                            dtCloneEnlacesEAD.ImportRow(rowEnlace);
                            dtCloneEnlacesEAD.Rows[i]["FechaInt"] = fechaintEnlaceAED;
                            dtCloneEnlacesEAD.Rows[i]["Fecha"] = fechaintEnlaceAED;


                            i++;
                        }

                        DataTable dtRepAcumuladoEAD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtCloneEnlacesEAD, dtRep, dtRep.Clone());

                        tituloGrid = "Consumo mensual enlaces de " + dtFechaIniRepEAD.ToString("MMMM") + "";
                        tituloGraf = "Consumo mensual enlaces de " + dtFechaIniRepEAD.ToString("MMMM") + "";
                        idContenedorGraf = "rep1ReporteEnlacesPorDia";
                        tipoGrafDefault = "msstackedcolumn2d";

                        string tituloGridAcumulado = "Consumo mensual enlaces acumulado diario de " + dtFechaIniRepEAD.ToString("MMMM") + "";
                        string tituloGrafAcumulado = "Consumo mensual enlaces acumulado diario de " + dtFechaIniRepEAD.ToString("MMMM") + "";
                        string idContenedorGrafAcumulado = "rep3ReporteEnlacesPorDia";
                        string tipoGrafDefaultAcumulado = "msstackedcolumn2d";

                        dtRep = dtRepAcumuladoEAD;

                        //haer reporte acumulado 
                        DataTable dtRepAcumuladoDiarioEAD = FormatoAcumuladoDT(dtRep);
                        if (dtRepAcumuladoDiarioEAD.Columns.Contains("Presupuesto"))
                        {
                            dtRepAcumuladoDiarioEAD.Columns.Remove("Presupuesto");
                        }


                        showValue = true;
                        showPercentage = true;

                        Rep3.Controls.Add(CrearControlGrid(dtRepAcumuladoDiarioEAD, tituloGridAcumulado, "", ""));
                        Rep4.Controls.Add(CrearControlGraf(dtRepAcumuladoDiarioEAD, tituloGrafAcumulado, 3, idContenedorGrafAcumulado, "", "", showValue, showPercentage));

                        break;

                    case "TCRTIO":
                        #region Busca Info Totales
                        BuscarFormatosFechas(6, DateTime.Now);


                        DataTable dtTasadoIn = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "In", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtTasadoOut = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "Out", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtEnlaces = BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep);
                        DataTable dtTotalesInOut = CalcularDTReporteTotales(dtTasadoIn, dtTasadoOut, dtEnlaces);

                        tituloGrid = "Importe total por tipo de campaña";
                        tituloGraf = "Importe total por tipo campaña";
                        idContenedorGraf = "Rep2RepImporteTotTipoCampania";
                        tipoGrafDefault = "msstackedcolumn2d";
                        #endregion



                        string linkTotalINOUT = Request.Path + "?fecha={2}&CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                        linkTotalINOUT = linkTotalINOUT.Replace("[%PlaceHolderCodRep%]", "TCRTIOD");
                        link = linkTotalINOUT;


                        showValue = true;
                        showPercentage = true;
                        dtRep = dtTotalesInOut;

                        break;

                    case "TCRTIOD":
                        #region Busca Info Totales
                        DateTime dTRTIOD = new DateTime(anioQS, mesQS, 1);
                        BuscarFormatosFechas(1, dTRTIOD);

                        DataTable dtTasadoInD = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "In", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtTasadoOutD = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "Out", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtEnlacesD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtTasadoInD);

                        #region duplica cada registro en el datatable de Enlaces por cada uno de los registros de dtenlace
                        int j = 0;

                        DataTable dtCloneEnlacesIOD = dtEnlacesD.Clone();
                        DataRow rowEnlaceIO = dtEnlacesD.Rows[0];

                        foreach (DataColumn column in dtCloneEnlacesIOD.Columns)
                        {
                            if (
                                column.ColumnName.ToLower() != "fecha" &&
                                column.ColumnName.ToLower() != "fechaint" &&
                                column.ColumnName.ToLower() != "link"
                               )
                            {
                                rowEnlaceIO[column.ColumnName] = Convert.ToDouble(rowEnlaceIO[column.ColumnName]) / dtTasadoInD.Rows.Count;
                            }
                        }

                        foreach (DataRow row in dtTasadoInD.Rows)
                        {
                            string fechaintEnlaceIO = "";
                            string fechaEnlace = "";

                            fechaintEnlaceIO = row["FechaInt"].ToString();
                            fechaEnlace = row["Fecha"].ToString();

                            dtCloneEnlacesIOD.ImportRow(rowEnlaceIO);
                            dtCloneEnlacesIOD.Rows[j]["FechaInt"] = fechaintEnlaceIO;
                            dtCloneEnlacesIOD.Rows[j]["Fecha"] = fechaEnlace;


                            j++;
                        }

                        #endregion

                        DataTable dtTotalesInOutD = CalcularDTReporteTotales(dtTasadoInD, dtTasadoOutD, dtCloneEnlacesIOD);
                        //DataTable dtRepAcumuladoIOD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtCloneEnlacesIOD, dtTotalesInOutD, dtTotalesInOutD.Clone());

                        dtRep = dtTotalesInOutD;

                        List<string> listaColumnasATomarEnCuenta = new List<string>();
                        listaColumnasATomarEnCuenta = FCAndControls.extraeNombreColumnas(dtTotalesInOutD)
                                                                                                        .AsEnumerable()
                                                                                                         .Where(o =>
                                                                                                                    !(o.ToString() == "Fecha") &&
                                                                                                                    !(o.ToString() == "FechaInt") &&
                                                                                                                    !(o.ToString() == "link")
                                                                                                                   ).ToList();

                        DataTable dtRepAcumuladoIOD = ConvierteDTEnFormtoAcumulado(dtTotalesInOutD, listaColumnasATomarEnCuenta);

                        tituloGrid = "Importe total por tipo campaña diario";
                        tituloGraf = "Importe total por tipo campaña diario";
                        idContenedorGraf = "Rep2RepImporteTotTipoCampaniaN2";
                        tipoGrafDefault = "msstackedcolumn2d";


                        tituloGridAcumulado = "Importe total por tipo campaña diario acumulado";
                        tituloGrafAcumulado = "Importe total por tipo campaña diario acumulado";
                        idContenedorGrafAcumulado = "Rep4RepImporteTotTipoCampaniaN2";
                        tipoGrafDefaultAcumulado = "msstackedcolumn2d";
                        #endregion

                        showValue = true;
                        showPercentage = true;
                        dtRep = dtTotalesInOutD;

                        showValue = true;
                        showPercentage = true;

                        Rep3.Controls.Add(CrearControlGrid(dtRepAcumuladoIOD, tituloGridAcumulado, "", ""));
                        Rep4.Controls.Add(CrearControlGraf(dtRepAcumuladoIOD, tituloGrafAcumulado, 3, idContenedorGrafAcumulado, "", "", showValue, showPercentage));

                        break;

                    default:
                        break;

                }

                DataTable dtClone = dtRep.Clone();
                foreach (DataRow row in dtRep.Rows)
                {
                    dtClone.ImportRow(row);
                }

                Rep1.Controls.Add(CrearControlGrid(dtRep, tituloGrid, link, linkLupa));
                if (codRepQS != "TCD" && codRepQS != "TCDA")
                {
                    Rep2.Controls.Add(CrearControlGraf(dtClone, tituloGraf, 3, idContenedorGraf, link, linkLupa,showValue,showPercentage));
                }
                else
                {
                    Rep2.Controls.Add(CrearControlGrafLineSet(dtClone, tituloGraf, 0, idContenedorGraf, link, linkLupa));
                }

                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public DataTable ConvierteDTEnFormtoAcumulado(DataTable dtRep, List<string> listaColumnasATomarEnCuenta)
        {
            try
            {
                DataTable dtClone = ClonarDT(dtRep);

                DataTable dtAcumulado = dtClone.Clone();

                int numRegistros = 0;

                numRegistros = dtClone.Rows.Count;

                for (int i = 0; i < numRegistros; i++)
                {
                    DataRow rowTotalAcumulado = dtAcumulado.NewRow();

                    if (dtAcumulado.Columns.Contains("Fecha"))
                    {
                        rowTotalAcumulado["Fecha"] = dtClone.Rows[i]["Fecha"];
                    }

                    if (dtAcumulado.Columns.Contains("FechaInt"))
                    {
                        rowTotalAcumulado["FechaInt"] = dtClone.Rows[i]["FechaInt"];
                    }

                    dtAcumulado.Rows.Add(rowTotalAcumulado);
                }


                for (int registroActual = 0; registroActual < dtAcumulado.Rows.Count; registroActual++)
                {
                    foreach (DataColumn column in dtAcumulado.Columns)
                    {
                        float acumuladoPorColumna;
                        if (listaColumnasATomarEnCuenta.Contains(column.ColumnName))
                        {
                            for (int contador = 0; contador <= registroActual; contador++)
                            {
                                double val = 0;
                                dtAcumulado.Rows[registroActual][column.ColumnName] = ((double.TryParse(dtAcumulado.Rows[registroActual][column.ColumnName].ToString(), out val)) ? val : 0) + double.Parse(dtClone.Rows[contador][column.ColumnName].ToString());
                            }
                        }
                    }

                }





                return dtAcumulado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable ClonarDT(DataTable dt)
        {
            try
            {
                DataTable dtClone = dt.Clone();

                foreach (DataRow row in dt.Rows)
                {
                    dtClone.ImportRow(row);
                }

                return dtClone;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CargarReportesSeccion(string tipoCampaniaRep1, string tipoCampaniaRep2, int campania)
        {
            try
            {
                //CodRep
                //tipoCampania = TC
                //tipoCampaniaFiltroCamp = TCC
                //tipoCampporDia  = TCD

                BuscarFormatosFechas(6, DateTime.Now);

                string codRep = "[%PlaceHolderCodRep%]";
                string link = Request.Path + "?fecha={2}&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                string linkLupa = Request.Path + "?CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=" + campania + "";
                string linkFiltroCamp = Request.Path + "?fecha={2}&filtroTipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=" + campania + "";
                string linkEnlacesAumulado = Request.Path + "?fecha={2}&CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                string linkLupaEnlacesAcumulado = Request.Path + "?CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                string linkReporteTotalInOUT = Request.Path + "?fecha={2}&CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";
                string linkLupaReporteTotalInOut = Request.Path + "?CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=0";

                // seccion 1
                linkLupa = linkLupa.Replace("[%PlaceHolderCodRep%]", "TC");
                linkLupaEnlacesAcumulado = linkLupaEnlacesAcumulado.Replace("[%PlaceHolderCodRep%]", "TCEA");
                linkEnlacesAumulado = linkEnlacesAumulado.Replace("[%PlaceHolderCodRep%]", "TCEAD");
                //CargarReporteTipoCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1, 0, formatoFecha, formatoFechaInt, link, linkLupa, linkEnlacesAumulado,linkLupaEnlacesAcumulado);
                CargarSeccion1(fechaIniRep, fechaFinRep, tipoCampaniaRep1, 0, formatoFecha, formatoFechaInt, link, linkLupa, linkLupaEnlacesAcumulado, linkEnlacesAumulado);

                //Seccion 2
                linkLupa = Request.Path + "?CodRep=" + codRep + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=" + campania + "";
                linkLupa = linkLupa.Replace("[%PlaceHolderCodRep%]", "TCC");
                //CargarReporteTipoCampaniaFiltroCamp(fechaIniRep, fechaFinRep, tipoCampaniaRep2, campania, formatoFecha, formatoFechaInt, linkFiltroCamp, linkLupa, "TCC");
                CargarSeccion2(fechaIniRep, fechaFinRep, tipoCampaniaRep2, campania, formatoFecha, formatoFechaInt, linkFiltroCamp, linkLupa, "TCC");


                //Seccion 3 
                linkLupaReporteTotalInOut = linkLupaReporteTotalInOut.Replace("[%PlaceHolderCodRep%]", "TCRTIO");
                linkReporteTotalInOUT = linkReporteTotalInOUT.Replace("[%PlaceHolderCodRep%]", "TCRTIOD");
                CargarSeccion3(fechaIniRep, fechaFinRep, tipoCampaniaRep1, 0, formatoFecha, formatoFechaInt, linkReporteTotalInOUT, linkLupaReporteTotalInOut, "TCRIO");

                if (campaniaQS.Length > 0 && filtroTipoCampaniaR1QS.Length > 0 && filtroTipoCampaniaR2QS.Length > 0 && fechaQS.Length > 0 && anioQS > 0 && mesQS > 0)
                {
                    DateTime dT = new DateTime(anioQS, mesQS, 1);
                    BuscarFormatosFechas(1, dT);

                    string fechaLinkLupa = (mesQS >= 10 ? mesQS.ToString() : "0" + mesQS.ToString()) + "/" + (anioQS);
                    linkLupa = Request.Path + "?CodRep=" + codRep + "&fecha=" + fechaLinkLupa + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=" + campaniaQS + "";
                    linkLupa = linkLupa.Replace("[%PlaceHolderCodRep%]", "TCD");

                    string linkLupaAcumulado = Request.Path + "?CodRep=" + codRep + "&fecha=" + fechaLinkLupa + "&filtrotipoCampR1=" + tipoCampaniaRep1 + "&filtroTipoCampR2=" + tipoCampaniaRep2 + "&camp=" + campaniaQS + "";
                    linkLupaAcumulado = linkLupaAcumulado.Replace("[%PlaceHolderCodRep%]", "TCDA");

        
                    DataTable dtRepTipoCampania = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt);
                    DataTable dtRepTipoCampañaPordiaAcumulado = dtRepTipoCampania.Clone();

                    foreach (DataRow row in dtRepTipoCampania.Rows)
                    {
                        dtRepTipoCampañaPordiaAcumulado.ImportRow(row);
                    }

                    string tituloGrid = campaniaQS != "0" ? "Por campaña" : "Por tipo de campaña";
                    string tituloGraf = campaniaQS != "0" ? "Por campaña" : "Por tipo de campaña";
                    string titutloGridAcumulado = campaniaQS != "0" ? "Por campaña acumulado" : "Por tipo de campaña acumulado";
                    string tituloGrafAcumulado = campaniaQS != "0" ? "Por campaña acumulado" : "Por tipo de campaña acumulado";

                    CargarReporteTipoCampaniaporDia(dtRepTipoCampania, fechaIniRep, tituloGrid, tituloGraf, "", linkLupa, Convert.ToInt32(campaniaQS), "pptoDiario");
                    CargarReporteTipoCampañaPorDiaAcumulado(dtRepTipoCampañaPordiaAcumulado, fechaIniRep, tituloGrafAcumulado, tituloGrafAcumulado, "", linkLupaAcumulado, Convert.ToInt32(campaniaQS), "pptoDiarioAcumulado");
                    //CargarReporteTipoCampaniaporDiaGrid(dtRepTipoCampania, fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt, "", linkLupa);
                    //CargarReporteTipoCampaniaPorDiaGraf(dtRepTipoCampania, fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt, "", linkLupa);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarSeccion1(string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa, string linkLupaEnlacesAcumulado, string linkEnlacesAumulado)
        {
            bool resultado = false;

            try
            {
                CargarReporteTipoCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1, 0, formatoFecha, formatoFechaInt, link, linkLupa, linkEnlacesAumulado, linkLupaEnlacesAcumulado);
            }
            catch (Exception ex)
            {

                throw ex;
            }



            return resultado;
        }

        public bool CargarSeccion2(string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa, string codRep)
        {
            try
            {
                bool respuesta = false;
                Panel pnlRepTipoCampania = new Panel();

                DataTable dtRepTipoCampania = RepCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampania, campania, formatoFecha, formatoFechaInt));
                DataTable dtRepConsumoEnlacesPorCampaña = RepPorCampañaFormatoDataTableEnlacesPorCampaña(BuscarInfoEnlacesporCampaña(fechaIniRep, fechaFinRep, campania), dtRepTipoCampania);


                foreach (DataRow row in dtRepConsumoEnlacesPorCampaña.Rows)
                {
                    foreach (DataColumn col in dtRepConsumoEnlacesPorCampaña.Columns)
                    {
                        if (row[col] == DBNull.Value)
                        {
                            row[col] = 0;
                        }
                    }
                }
                DataView dvRepAcumulado = new DataView(dtRepConsumoEnlacesPorCampaña);

                List<string> listaColumnasDeseadas = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesPorCampaña).ToList().Where(r => !r.Contains(" ")).Select(r => r).ToList();

                DataTable dtRepAcumulado = RepPorCampañaFormatoDataTableEnlacesPorCampañaAcumulado(dtRepConsumoEnlacesPorCampaña, dtRepTipoCampania, dvRepAcumulado.ToTable(false, listaColumnasDeseadas.ToArray()));

                List<string> listaSeries = new List<string>();

                foreach (DataColumn column in dtRepTipoCampania.Columns)
                {
                    if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                    {
                        listaSeries.Add(column.ColumnName);
                    }
                }



                Rep2.Controls.Clear();
                Rep2.Controls.Add(CrearRepTipoCampaniaTabContainer(dtRepTipoCampania, "Por campaña", "Por campaña", 3, "Rep2GrafTipoCamp", link, linkLupa));


                //Revisa si la cantidad de columnas es mayor a 5 para decir si encontro informacion para la campaña  actual
                if (dtRepConsumoEnlacesPorCampaña.Columns.Count <= 5)
                {
                    dtRepConsumoEnlacesPorCampaña.Clear();
                    dtRepAcumulado.Clear();

                }
                Rep4.Controls.Clear();
                Rep4.Controls.Add(CrearControlGridConsumoEnlacesPorCampaña(dtRepConsumoEnlacesPorCampaña, "Consumo enlaces por campaña"));

                Rep6.Controls.Clear();
                Rep6.Controls.Add(CrearControlGridConsumoEnlacesPorCampañaAcumulado(dtRepAcumulado, "Consumo enlaces por campaña acumulado"));



                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CargarSeccion3(string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa, string codRep)
        {
            bool resultado = false;

            try
            {
                //BuscaInfo necesaria
                #region Busca Info Totales

                DataTable dtTasadoIn = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "In", 0, formatoFecha, formatoFechaInt, link, linkLupa, codRep);
                DataTable dtTasadoOut = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "Out", 0, formatoFecha, formatoFechaInt, link, linkLupa, codRep);
                DataTable dtEnlaces = BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep);
                DataTable dtTotalesInOut = CalcularDTReporteTotales(dtTasadoIn, dtTasadoOut, dtEnlaces);
                #endregion


                #region Reporte


                //TabContainer tcReportes = new TabContainer();
                //tcReportes.ID = Guid.NewGuid().ToString();
                //tcReportes.CssClass = "MyTabStyle";

                //TabPanel tpRep1 = new TabPanel();
                //tpRep1.ID = Guid.NewGuid().ToString();
                //tcReportes.Tabs.Add(tpRep1);
                //tpRep1.HeaderText = "Tabla";


                //tpRep1.Controls.Add(
                //          DTIChartsAndControls.tituloYBordesReporte(
                //                          DTIChartsAndControls.GridView("gridimporteTotalPorTipoCampaña", dtTotalesInOut, true, "Totales",
                //                          new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }, link,
                //                          new string[] { "Fecha" }, 2, new int[] { 0, 1 }, new int[] { 3, 4, 5, 6, 7 }, new int[] { 2 }),
                //                          "Reporte", "Reporte importe total por tipo de campaña", 0, linkLupa)
                //          );


                string idContenedorGrid = "gridimporteTotalPorTipoCampaña";
                string idContenedorGraf = "GrafImporteTotalRep7";
                string tituloGrid = "Importe total por tipo de campaña";
                int pestañaActiva = 3;
                List<string> nombresColumnas = FCAndControls.extraeNombreColumnas(dtTotalesInOut).ToList();
                List<string> formatoColumnas = new List<string>();


                nombresColumnas = FCAndControls.extraeNombreColumnas(dtTotalesInOut).ToList();

                foreach(string nombre in nombresColumnas)
                {
                    if (nombre.ToLower().Contains("fecha") || nombre.ToLower().Contains("link"))
                    {
                        formatoColumnas.Add("");
                    }
                    else
                    {
                        formatoColumnas.Add("{0:c}");
                    }
                }
                
                

                Rep9.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                    DTIChartsAndControls.GridView(idContenedorGrid, dtTotalesInOut, true, "Totales",
                    formatoColumnas.ToArray(), link,
                    nombresColumnas.ToArray(), 2, new int[] { 0, 1 }, new int[] { 3, 4, 5, 6, 7 }, new int[] { 2 }),
                    idContenedorGraf, tituloGrid, linkLupa, pestañaActiva, FCGpoGraf.MatricialConStack)
                );




                #endregion Grid

                #region Grafica
                if (dtTotalesInOut.Rows[dtTotalesInOut.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                {
                    dtTotalesInOut.Rows[dtTotalesInOut.Rows.Count - 1].Delete();
                }
                if (dtTotalesInOut.Columns.Contains("Total"))
                {
                    dtTotalesInOut.Columns.Remove("Total");
                }
                if (dtTotalesInOut.Columns.Contains("FechaInt"))
                {
                    dtTotalesInOut.Columns.Remove("FechaInt");
                }
                if (dtTotalesInOut.Columns.Contains("link"))
                {
                    dtTotalesInOut.Columns.Remove("link");
                }
                dtTotalesInOut.AcceptChanges();
                DataTable[] arrDT;
                arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtTotalesInOut);


                List<string> listaFechasLink = dtTotalesInOut.AsEnumerable().Select(r => r["Fecha"].ToString()).ToList<string>();
                foreach (DataTable dt in arrDT)
                {

                    if (dt.Columns.Contains("value"))
                    {
                        dt.Columns.Add("link", typeof(string));

                        int i = 0;
                        foreach (DataRow row in dt.Rows)
                        {

                            row["link"] = link.Replace("{0}", listaFechasLink[i]);
                            i++;
                        }
                    }
                    dt.AcceptChanges();
                }

                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);
                string[] series = FCAndControls.extraeNombreColumnas(dtTotalesInOut);


                if (lsaDataSource != null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), idContenedorGraf,
                                        FCAndControls.GraficaMultiSeries(lsaDataSource, series, idContenedorGraf,
                                        "", "", "Mes", "Importe", pestañaActiva, FCGpoGraf.MatricialConStack,"$","","dti","98%","385",true,true), false);
                }

                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }



            return resultado;
        }

        public DataTable BuscarInfoTasacion(string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa, string codRep)
        {
            try
            {
                DataTable dtRep = new DataTable();

                dtRep = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampania, campania, formatoFecha, formatoFechaInt);
                return dtRep;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CalcularDTReporteTotales(DataTable dtTasadoIn, DataTable dtTasadoOut, DataTable dtEnlaces)
        {
            try
            {
                DataTable dtRepTotales = new DataTable();

                //se agregan columnass al datatable de totales
                dtRepTotales.Columns.Add("link", typeof(string));
                dtRepTotales.Columns.Add("FechaInt", typeof(string));
                dtRepTotales.Columns.Add("Fecha", typeof(string));
                dtRepTotales.Columns.Add("In", typeof(float));
                dtRepTotales.Columns.Add("Out", typeof(float));
                dtRepTotales.Columns.Add("Enlace", typeof(float));
                dtRepTotales.Columns.Add("Otros", typeof(float));
                dtRepTotales.Columns.Add("Total", typeof(float));

                //Fecha
                //toma las fechas del dt Tasado In
                foreach (DataRow row in dtTasadoIn.Rows)
                {
                    DataRow rowTotales = dtRepTotales.NewRow();

                    rowTotales["Fecha"] = row["Fecha"].ToString();
                    rowTotales["FechaInt"] = row["FechaInt"].ToString();
                    dtRepTotales.Rows.Add(rowTotales);
                }

                //In
                #region calcula tasado IN
                string[] arrayCarriers = BuscarCarriers(dtTasadoIn).AsEnumerable().Where
                                                                    (i =>
                                                                        !(i.ToLower().Contains("total")) &&
                                                                        !(i.ToLower().Contains("link")) &&
                                                                        !(i.ToLower().Contains("fecha")) &&
                                                                        !(i.ToLower().Contains("fechaint")) &&
                                                                        !(i.ToLower().Contains("mes")) &&
                                                                        !(i.ToLower().Contains("año")) &&
                                                                        !(i.ToLower().Contains("comprometido")) &&
                                                                        !(i.ToLower().Contains("ajuste"))
                                                                    ).ToArray();

                foreach (DataRow rowTotales in dtRepTotales.Rows)
                {
                    string fecha = rowTotales["Fecha"].ToString();

                    foreach (DataRow rowTasado in dtTasadoIn.Rows)
                    {
                        float totalInRowTasadoIn = 0;
                        string FechaTasado = rowTasado["Fecha"].ToString();

                        if (FechaTasado == fecha)
                        {
                            foreach (DataColumn column in dtTasadoIn.Columns)
                            {
                                if (arrayCarriers.Contains(column.ColumnName))
                                {
                                    totalInRowTasadoIn += float.Parse(rowTasado[column].ToString());
                                }
                            }
                            rowTotales["In"] = totalInRowTasadoIn;
                            rowTotales["Total"] = totalInRowTasadoIn;
                        }

                    }
                }

                #endregion
                //Out
                #region calcula tasado OUT
                arrayCarriers = BuscarCarriers(dtTasadoOut).AsEnumerable().Where
                                                    (i =>
                                                        !(i.ToLower().Contains("total")) &&
                                                        !(i.ToLower().Contains("link")) &&
                                                        !(i.ToLower().Contains("fecha")) &&
                                                        !(i.ToLower().Contains("fechaint")) &&
                                                        !(i.ToLower().Contains("id")) &&
                                                        !(i.ToLower().Contains("mes")) &&
                                                        !(i.ToLower().Contains("año")) &&
                                                        !(i.ToLower().Contains("comprometido")) &&
                                                        !(i.ToLower().Contains("ajuste"))
                                                    ).ToArray();


                foreach (DataRow rowTotales in dtRepTotales.Rows)
                {
                    string fecha = rowTotales["Fecha"].ToString();

                    foreach (DataRow rowTasado in dtTasadoOut.Rows)
                    {
                        float totalInRowTasadoOut = 0;
                        string FechaTasado = rowTasado["Fecha"].ToString();

                        if (FechaTasado == fecha)
                        {
                            foreach (DataColumn column in dtTasadoOut.Columns)
                            {
                                if (arrayCarriers.Contains(column.ColumnName))
                                {
                                    totalInRowTasadoOut += float.Parse(rowTasado[column].ToString());
                                }
                            }
                            rowTotales["Out"] = totalInRowTasadoOut;
                            rowTotales["Total"] = float.Parse(rowTotales["Total"].ToString()) + totalInRowTasadoOut;
                        }

                    }
                }

                #endregion
                //Enlaces
                #region Calcula costo enlaces

                arrayCarriers = BuscarCarriers(dtEnlaces).AsEnumerable().Where
                                                   (i =>
                                                       !(i.ToLower().Contains("total")) &&
                                                       !(i.ToLower().Contains("link")) &&
                                                       !(i.ToLower().Contains("fecha")) &&
                                                       !(i.ToLower().Contains("fechaint")) &&
                                                       !(i.ToLower().Contains("ajuste")) &&
                                                       !(i.ToLower().Contains("presupuesto")) &&
                                                       !(i.ToLower().Contains("id")) &&
                                                       !(i.ToLower().Contains("mes")) &&
                                                       !(i.ToLower().Contains("año")) &&
                                                       !(i.ToLower().Contains("comprometido"))
                                                   ).ToArray();


                foreach (DataRow rowTotales in dtRepTotales.Rows)
                {
                    string fecha = rowTotales["Fecha"].ToString();

                    foreach (DataRow rowTasado in dtEnlaces.Rows)
                    {
                        float totalInRowTasadoEnlaces = 0;
                        string FechaTasado = rowTasado["Fecha"].ToString();

                        if (FechaTasado == fecha)
                        {
                            foreach (DataColumn column in dtEnlaces.Columns)
                            {
                                if (arrayCarriers.Contains(column.ColumnName))
                                {
                                    totalInRowTasadoEnlaces += float.Parse(rowTasado[column].ToString());
                                }
                            }
                            rowTotales["Enlace"] = totalInRowTasadoEnlaces;
                            rowTotales["Total"] = float.Parse(rowTotales["Total"].ToString()) + totalInRowTasadoEnlaces;
                        }

                    }
                }

                #endregion
                //Otro
                #region calcula otros
                arrayCarriers = BuscarCarriers(dtEnlaces).AsEnumerable().Where
                                                  (i =>
                                                      (i.ToLower().Contains("ajuste"))
                                                  ).ToArray();




                foreach (DataRow rowTotales in dtRepTotales.Rows)
                {
                    string fecha = rowTotales["Fecha"].ToString();

                    foreach (DataRow rowTasado in dtEnlaces.Rows)
                    {
                        float totalInRowTasadoOtro = 0;
                        string FechaTasado = rowTasado["Fecha"].ToString();

                        if (FechaTasado == fecha)
                        {
                            foreach (DataColumn column in dtEnlaces.Columns)
                            {
                                if (arrayCarriers.Contains(column.ColumnName))
                                {
                                    totalInRowTasadoOtro += float.Parse(rowTasado[column].ToString());
                                }
                            }

                            rowTotales["Otros"] = totalInRowTasadoOtro;
                            rowTotales["Total"] = float.Parse(rowTotales["Total"].ToString()) + totalInRowTasadoOtro;
                        }

                    }
                }



                //Altera tabla de totales para restar el tasado de bestel
                double TasadoBestel = 0;
                arrayCarriers = FCAndControls.extraeNombreColumnas(dtTasadoIn)
                                                            .Where(i => (i.ToLower().Contains("bestel")) &&
                                                                        !(i.ToLower().Contains("ajuste")) &&
                                                                        !(i.ToLower().Contains("comprometido"))
                                                                   ).ToArray();


                foreach (DataRow rowTotales in dtRepTotales.Rows)
                {
                    string fecha = rowTotales["Fecha"].ToString();

                    foreach (DataRow rowTasado in dtTasadoIn.Rows)
                    {
                        float totalInRowTasadoOtro = 0;
                        string FechaTasado = rowTasado["Fecha"].ToString();

                        if (FechaTasado == fecha)
                        {
                            foreach (DataColumn column in dtTasadoIn.Columns)
                            {
                                if (arrayCarriers.Contains(column.ColumnName))
                                {
                                    //totalInRowTasadoOtro += float.Parse(rowTasado[column].ToString());
                                }
                            }
                        }

                        double otro = 0;
                        double.TryParse(rowTotales["Otros"].ToString(), out otro);


                        if (otro > 0)
                        {
                            rowTotales["Otros"] = (double.TryParse(rowTotales["Otros"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;
                            rowTotales["Total"] = Convert.ToDouble(rowTotales["Total"]) + (double.TryParse(rowTotales["Total"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;
                        }
                        else
                        {
                            rowTotales["Otros"] = 0;
                            rowTotales["Total"] = Convert.ToDouble(rowTotales["Total"]) + totalInRowTasadoOtro;

                        }


                        //rowTotales["Otros"] = (double.TryParse(rowTotales["Otros"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;
                        //rowTotales["Total"] = (double.TryParse(rowTotales["Total"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;

                    }
                }

                arrayCarriers = FCAndControls.extraeNombreColumnas(dtTasadoOut)
                                            .Where(i => (i.ToLower().Contains("bestel")) &&
                                                        !(i.ToLower().Contains("ajuste")) &&
                                                        !(i.ToLower().Contains("comprometido"))
                                                   ).ToArray();


                foreach (DataRow rowTotales in dtRepTotales.Rows)
                {
                    string fecha = rowTotales["Fecha"].ToString();

                    foreach (DataRow rowTasado in dtTasadoOut.Rows)
                    {
                        float totalInRowTasadoOtro = 0;
                        string FechaTasado = rowTasado["Fecha"].ToString();

                        if (FechaTasado == fecha)
                        {
                            foreach (DataColumn column in dtTasadoOut.Columns)
                            {
                                if (arrayCarriers.Contains(column.ColumnName))
                                {
                                    //totalInRowTasadoOtro += float.Parse(rowTasado[column].ToString());
                                }
                            }
                        }
                        double otro = 0;
                        double.TryParse(rowTotales["Otros"].ToString(), out otro);


                        if (otro > 0)
                        {
                            rowTotales["Otros"] = (double.TryParse(rowTotales["Otros"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;
                            rowTotales["Total"] = Convert.ToDouble(rowTotales["Total"]) + (double.TryParse(rowTotales["Total"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;
                        }
                        else
                        {
                            rowTotales["Otros"] = 0;
                            rowTotales["Total"] = Convert.ToDouble(rowTotales["Total"]) + totalInRowTasadoOtro;
                        }

                        //rowTotales["Otros"] = (double.TryParse(rowTotales["Otros"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;
                        //rowTotales["Total"] = (double.TryParse(rowTotales["Total"].ToString(), out otro) ? otro : 0) - totalInRowTasadoOtro;

                    }
                }



                #endregion

                return dtRepTotales;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] BuscarCarriers(DataTable dtRep)
        {
            try
            {
                List<string> listaCarriers = new List<string>();
                foreach (DataColumn column in dtRep.Columns)
                {
                    if (column.ColumnName == "Categoria")
                    {
                        listaCarriers.Add(column.ColumnName);
                    }
                }

                foreach (DataColumn column in dtRep.Columns)
                {
                    if (column.ColumnName != "Fecha" && column.ColumnName != "FechaInt" && column.ColumnName != "Categoria")
                    {
                        listaCarriers.Add(column.ColumnName);
                    }
                }

                return listaCarriers.ToArray();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarFiltros()
        {
            try
            {
                bool respuesta = false;

                #region ddlTiposCampaniaRep1
                ddlTiposCampaniaRep1.DataSource = BuscarFiltrosTipoCampania();
                ddlTiposCampaniaRep1.DataBind();
                #endregion


                #region ddlTiposCampaniaRep2
                ddlTiposCampaniaRep2.DataSource = BuscarFiltrosTipoCampania();
                ddlTiposCampaniaRep2.DataBind();
                #endregion

                #region ddlCampanias
                DataTable dtCampanias = new DataTable();
                dtCampanias = BuscarCampanias();
                if (dtCampanias != null && dtCampanias.Rows.Count > 0 && dtCampanias.Columns.Count > 0)
                {
                    ddlcampanias.DataSource = dtCampanias;
                    ddlcampanias.DataTextField = "descripcion";
                    ddlcampanias.DataValueField = "iCodCatalogo";

                    ddlcampanias.DataBind();
                }
                #endregion

                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarReporteTipoCampañaPorDiaAcumulado(DataTable dtRepTipoCampania, string fechaIniRep, string tituloGrid, string tituloGraf, string link, string linkLupa, int campaña, string TipoPpto)
        {
            try
            {
                bool respuesta = false;

                Panel pnlRepTipoCampania = new Panel();
                List<string> listaSeries = new List<string>();
                DateTime fechaIni = new DateTime();
                DataTable dtPresupuestosporMes = new DataTable();

                dtRepTipoCampania = FormatoAcumuladoDT(dtRepTipoCampania);


                foreach (DataColumn column in dtRepTipoCampania.Columns)
                {
                    if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                    {
                        listaSeries.Add(column.ColumnName);
                    }
                }



                if (DateTime.TryParse(fechaIniRep, out fechaIni))
                {
                    tituloGrid = tituloGrid + " " + fechaIni.ToString("MMMM");
                    tituloGraf = tituloGraf + " " + fechaIni.ToString("MMMM");
                }

                Rep8.Controls.Clear();
                Rep8.Controls.Add(CrearRepTipoCampaniaTabContainerLineSet(dtRepTipoCampania, tituloGrid, tituloGraf, 0, "rep8GrafTipoCampania", link, linkLupa));


                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable FormatoAcumuladoDT(DataTable dtRep)
        {
            try
            {
                DataTable dtResultado = dtRep.Clone();
                foreach (DataRow row in dtRep.Rows)
                {
                    DataRow rowRes = dtResultado.NewRow();
                    rowRes["Fecha"] = row["Fecha"];
                    rowRes["FechaInt"] = row["FechaInt"];

                    dtResultado.Rows.Add(rowRes);
                }



                //Agregar columna decimal presupuestos codRep el presupuestos por dia
                dtResultado = IntegrarPresupuestosPordia(dtResultado, BuscaInfoPptoPorCampaña(fechaIniRep, campania));

                List<string> listaCarriers = new List<string>();
                List<float> listaTasado = new List<float>();
                listaCarriers = FCAndControls.extraeNombreColumnas(dtResultado).AsEnumerable()
                    .Where(r => (!r.ToLower().Contains("link")) &&
                                (!r.ToLower().Contains("fecha")) &&
                                (!r.ToLower().Contains("total")) &&
                                (!r.ToLower().Contains("Presupuesto"))
                           ).ToList();


                int countResultado = 0;
                int countRep = 0;
                int nReg = dtRep.Rows.Count;
                float importeRegistro = 0;


                while (countResultado < nReg)
                {
                    DataRow rowResultado = dtResultado.Rows[countResultado];
                    countRep = 0;
                    while (countRep <= countResultado)
                    {
                        DataRow rowRep = dtRep.Rows[countRep];

                        foreach (string carrier in listaCarriers)
                        {
                            if (dtRep.Columns.Contains(carrier) && float.TryParse(rowRep[carrier].ToString(), out importeRegistro))
                            {


                                rowResultado[carrier] = ((rowResultado[carrier] != DBNull.Value && Convert.ToDouble(rowResultado[carrier]) > 0) ? Convert.ToDouble(rowResultado[carrier]) : 0) + importeRegistro;


                            }

                        }

                        countRep += 1;
                    }

                    float presupuestoBase = 0;

                    int DiaResultado = 0;
                    if (!int.TryParse(rowResultado["Fecha"].ToString().Split('/')[0], out DiaResultado))
                    {
                        DiaResultado = 1;
                    }

                    if (float.TryParse(rowResultado["Presupuesto"].ToString(), out presupuestoBase))
                    {
                        rowResultado["Presupuesto"] = presupuestoBase * (DiaResultado);
                    }
                    else
                    {
                        rowResultado["Presupuesto"] = 0;
                    }
                    countResultado += 1;
                }



                return dtResultado;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable IntegrarPresupuestosPordia(DataTable dtRep, DataTable dtPpto)
        {
            try
            {
                if (!dtRep.Columns.Contains("Presupuesto"))
                {
                    dtRep.Columns.Add("Presupuesto", typeof(float));

                }

                foreach (DataRow rowRep in dtRep.Rows)
                {
                    foreach (DataRow rowPpto in dtPpto.Rows)
                    {
                        if (rowRep["Presupuesto"] == DBNull.Value)
                        {
                            rowRep["Presupuesto"] = 0;
                        }

                        if (rowRep["FechaInt"].ToString() == rowPpto["FechaInt"].ToString())
                        {
                            rowRep["Presupuesto"] = (float)Convert.ToDouble(rowPpto["PptoDiario"].ToString());
                        }
                    }
                }
                return dtRep;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CargarReporteTipoCampaniaporDia(DataTable dtRepTipoCampania, string fechaIniRep, string tituloGrid, string tituloGraf, string link, string linkLupa, int campaña, string TipoPpto)
        {
            try
            {

                bool resultado = false;

                Panel pnlRepTipoCampania = new Panel();

                List<string> listaSeries = new List<string>();

                foreach (DataColumn column in dtRepTipoCampania.Columns)
                {
                    if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                    {
                        listaSeries.Add(column.ColumnName);
                    }
                }

                DateTime fechaIni = new DateTime();

                if (DateTime.TryParse(fechaIniRep, out fechaIni))
                {
                    tituloGrid = tituloGrid + " " + fechaIni.ToString("MMMM");
                    tituloGraf = tituloGraf + " " + fechaIni.ToString("MMMM");
                }

                DataTable dtPresupuestosporMes = new DataTable();

                dtPresupuestosporMes = BuscaInfoPptoPorCampaña(fechaIniRep, campaña);


                foreach (DataRow rowdtRepTipoCampaña in dtRepTipoCampania.Rows)
                {
                    if (!dtRepTipoCampania.Columns.Contains("Presupuesto"))
                    {
                        dtRepTipoCampania.Columns.Add("Presupuesto", typeof(float));
                    }
                    dtRepTipoCampania.AcceptChanges();

                    foreach (DataRow rowPpto in dtPresupuestosporMes.Rows)
                    {
                        if (dtRepTipoCampania.Columns.Contains("FechaInt") && dtPresupuestosporMes.Columns.Contains("FechaInt"))
                        {
                            if (rowdtRepTipoCampaña["FechaInt"].ToString() == rowPpto["FechaInt"].ToString())
                            {
                                rowdtRepTipoCampaña["Presupuesto"] = Convert.ToDouble(rowPpto[TipoPpto]);
                            }
                        }
                    }
                }


                Rep7.Controls.Clear();
                Rep7.Controls.Add(CrearRepTipoCampaniaTabContainerLineSet(dtRepTipoCampania, tituloGrid, tituloGraf, 0, "rep7GrafTipoCampania", link, linkLupa));




                return resultado;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable BuscaInfoPptoPorCampaña(string FechaIniRep, int campaña)
        {
            try
            {
                DataTable dtPresupuesto = new DataTable();


                dtPresupuesto = DSODataAccess.Execute(ConsultaBuscaInfoPptoPorCampaña(fechaIniRep, campaña));


                //if (!dtPresupuesto.Columns.Contains("FechaInt"))
                //{
                //    dtPresupuesto.Columns.Add("FechaInt", typeof(string));
                //}

                //if (!dtPresupuesto.Columns.Contains("Presupuesto"))
                //{
                //    dtPresupuesto.Columns.Add("Presupuesto", typeof(string));
                //}

                //DateTime fechaIni = new DateTime();
                //DateTime fechaFin = new DateTime();

                //if (DateTime.TryParse(fechaIniRep, out fechaIni) && DateTime.TryParse(fechaFinRep, out fechaFin))
                //{
                //    while (fechaIni < fechaFin)
                //    {
                //        Random rnd = new Random();

                //        DataRow row = dtPresupuesto.NewRow();

                //        row["FechaInt"] = fechaIni.ToString("yyyyMMdd");
                //        row["Presupuesto"] = 500 * rnd.Next(0, 100);
                //        dtPresupuesto.Rows.Add(row);
                //        fechaIni = fechaIni.AddDays(1);
                //    }
                //}


                return dtPresupuesto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaBuscaInfoPptoPorCampaña(string fechaIniRep, int campaña)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("exec PptoPorCampañaPentafon             ");
                query.AppendLine("@esquema = '" + DSODataContext.Schema + "', ");
                query.AppendLine("@descReporte  ='Reporte por Campaña',   ");
                query.AppendLine("@campaña = " + campaña + ",                 ");
                query.AppendLine("@FechaIniRep  = '" + fechaIniRep + "'       ");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CargarReporteTipoCampaniaporDiaGrid(DataTable dtRepTipoCampania, string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa)
        {
            try
            {
                bool resultado = false;

                Panel pnlRepTipoCampania = new Panel();

                string[] camposFecha = fechaIniRep.Split('-');
                DateTime dtFechaIniRep = new DateTime(Convert.ToInt32(camposFecha[0]), Convert.ToInt32(camposFecha[1]), 1);


                Rep7.Controls.Clear();
                Rep7.Controls.Add(CrearControlGrid(dtRepTipoCampania, "Por tipo de campaña de " + dtFechaIniRep.ToString("MMMM") + " ", "", linkLupa));
                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarReporteTipoCampaniaPorDiaGraf(DataTable dtRepTipoCampania, string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa)
        {
            try
            {
                bool respuesta = false;

                Panel pnlRepTipoCampania = new Panel();

                Rep8.Controls.Clear();
                Rep8.Controls.Add(CrearControlGraf(dtRepTipoCampania, "Por tipo de campaña", 3, "rep4GrafTipoCampania", link, linkLupa));

                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarReporteTipoCampania(string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa, string linkEnlacesAcumulado, string linkLupaEnlacesAcumulado)
        {
            try
            {
                bool resultado = false;

                Panel pnlRepTipoCampania = new Panel();

                // la fecha se devuelve como 201802
                DataTable dtRepTipoCampania = RepTipoCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampania, campania, formatoFecha, formatoFechaInt));
                DataTable dtRepConsumoEnlacesMensual = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtRepTipoCampania);
                DataTable dtRepAcumulado = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtRepConsumoEnlacesMensual, dtRepTipoCampania, dtRepTipoCampania.Clone());



                List<string> listaSeries = new List<string>();

                foreach (DataColumn column in dtRepTipoCampania.Columns)
                {
                    if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                    {
                        listaSeries.Add(column.ColumnName);
                    }
                }

                Rep1.Controls.Clear();
                Rep1.Controls.Add(CrearRepTipoCampaniaTabContainer(dtRepTipoCampania, "Por tipo de campaña", "Por tipo de campaña", 3, "rep1GrafTipoCampania", link, linkLupa));

                Rep3.Controls.Clear();
                Rep3.Controls.Add(CrearControlGridConsumoMensualEnlaces(dtRepConsumoEnlacesMensual, "Consumo mensual enlaces"));

                Rep5.Controls.Clear();
                Rep5.Controls.Add(CrearRepTipoCampaniaTabContainer(dtRepAcumulado, "Consumo mensual enlaces acumulado", "Consumo mensual enlaces acumulado", 3, "rep5GrafTipoCampaniaElacesAcumulado", linkEnlacesAcumulado, linkLupaEnlacesAcumulado, true, true));

                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable RepTipoCampañaFormatoDatatableConsumo(DataTable dtRepTipoCampania)
        {
            try
            {

                //Inserta un columna extra para guardar el todal para todos los carriers 
                if (!dtRepTipoCampania.Columns.Contains("Total"))
                {
                    dtRepTipoCampania.Columns.Add("Total", typeof(double));
                }

                if (dtRepTipoCampania.Columns.Contains("Total"))
                {
                    foreach (DataRow row in dtRepTipoCampania.Rows)
                    {
                        double importeTotal = 0;

                        foreach (DataColumn column in dtRepTipoCampania.Columns)
                        {
                            double importePorCarrier = 0;
                            if (
                                !(column.ColumnName.ToLower().Contains("fecha")) &&
                                !(column.ColumnName.ToLower().Contains("link")) &&
                                !(column.ColumnName.ToLower().Contains("Total"))
                               )
                            {
                                if (double.TryParse(row[column].ToString(), out importePorCarrier))
                                {
                                    importeTotal += importePorCarrier;
                                }
                            }
                        }
                        row["Total"] = importeTotal;
                    }

                }

                return dtRepTipoCampania;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(DataTable dtRepConsumoEnlacesMensual, DataTable dtRepTipoCampania)
        {
            try
            {

                DataView dtvRepConsumoEnlacesMensual = new DataView(dtRepConsumoEnlacesMensual);
                dtRepConsumoEnlacesMensual = dtvRepConsumoEnlacesMensual.ToTable(false, new string[] { "fechaInt", "fecha", "protelInternet", "protelL2L", "marcatelInternet", "marcatelL2L", "bestelInternet", "bestelL2L", "bestelajuste", "bestelComprometido", "axtelInternet", "total" });

                /*Cambia el nombre de las columnas como las recibe de la base de datos */
                if (dtRepConsumoEnlacesMensual.Columns.Contains("protelInternet"))
                {
                    dtRepConsumoEnlacesMensual.Columns["protelInternet"].ColumnName = "Protel Internet";
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("protelL2L"))
                {
                    dtRepConsumoEnlacesMensual.Columns["protelL2L"].ColumnName = "Protel L2L";
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("marcatelInternet"))
                {
                    dtRepConsumoEnlacesMensual.Columns["marcatelInternet"].ColumnName = "Marcatel Internet";
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("marcatelL2L"))
                {
                    dtRepConsumoEnlacesMensual.Columns["marcatelL2L"].ColumnName = "Marcatel L2L";
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("bestelInternet"))
                {
                    dtRepConsumoEnlacesMensual.Columns["bestelInternet"].ColumnName = "Bestel Internet";
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("bestelL2L"))
                {
                    dtRepConsumoEnlacesMensual.Columns["bestelL2L"].ColumnName = "Bestel L2L";
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("bestelComprometido"))
                {
                    dtRepConsumoEnlacesMensual.Columns["bestelComprometido"].ColumnName = "Bestel Comprometido";
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("axtelInternet"))
                {
                    dtRepConsumoEnlacesMensual.Columns["axtelInternet"].ColumnName = "Axtel Internet";
                }



                /*Calcula la culumna de Bestel Comprometido a partir del tasado de bestel*/
                foreach (DataRow row in dtRepConsumoEnlacesMensual.Rows)
                {
                    string fechaEnlaces = row["fechaInt"].ToString();

                    string consumoBestel = dtRepTipoCampania.AsEnumerable().Where(r => r["fechaInt"].ToString() == fechaEnlaces).Select(r => r["Bestel"].ToString()).FirstOrDefault();
                    consumoBestel = consumoBestel != null ? consumoBestel : "0";


                    double Comprometido = row["bestelajuste"] == DBNull.Value ? 0 : Convert.ToInt32(row["bestelajuste"]);
                    if (Comprometido > 0)
                    {
                        Comprometido = Comprometido - Convert.ToDouble(consumoBestel);
                    }
                    else
                    {
                        Comprometido = 0;
                    }
                    //row["Bestel Comprometido"] = (Convert.ToDouble(row["bestelajuste"] == DBNull.Value ? "0" : row["bestelajuste"]) - Convert.ToDouble(consumoBestel)).ToString();
                    row["Bestel Comprometido"] = Comprometido.ToString();


                    dtRepConsumoEnlacesMensual.AcceptChanges();
                }


                return dtRepConsumoEnlacesMensual;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public DataTable RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(DataTable dtRepConsumoEnlacesMensual, DataTable dtRepTipoCampania, DataTable dtRepAcumulado)
        {
            try
            {


                //Importa Todos los registro del datatable de dtRepTipoCampania
                foreach (DataRow row in dtRepTipoCampania.Rows)
                {
                    dtRepAcumulado.ImportRow(row);
                }

                ///////////////////////////Contrute datatable para el reporte De acumulado//////////////////////////////////

                List<string> listaCarriers = new List<string>();
                List<string> listaFechasRep = new List<string>();

                listaCarriers = FCAndControls.extraeNombreColumnas(dtRepTipoCampania).ToList<string>().Where(r => !(r.ToLower().Contains("link")) && !(r.ToLower().Contains("fecha"))).ToList<string>();

                if (dtRepTipoCampania.Columns.Contains("FechaInt"))
                {
                    foreach (DataRow row in dtRepTipoCampania.Rows)
                    {
                        string fechaRep = row["FechaInt"].ToString();
                        listaFechasRep.Add(fechaRep);
                    }
                }

                double consumo = 0;

                foreach (string fecha in listaFechasRep)
                {
                    foreach (string carrier in listaCarriers)
                    {
                        consumo = 0;
                        foreach (DataRow row in dtRepConsumoEnlacesMensual.Rows)
                        {
                            if (row["FechaInt"].ToString() == fecha)
                            {
                                foreach (DataColumn column in dtRepConsumoEnlacesMensual.Columns)
                                {
                                    if ((column.ColumnName.ToUpper().Contains(carrier.ToUpper())) && (!(column.ColumnName.ToUpper().Contains("AJUSTE")) && !(column.ColumnName.ToUpper().Contains("COMPROMETIDO"))))
                                    {
                                        consumo += Convert.ToDouble(row[column].ToString());
                                    }
                                }
                            }
                        }

                        foreach (DataRow row in dtRepAcumulado.Rows)
                        {
                            if (row["FechaInt"].ToString() == fecha)
                            {
                                row[carrier] = Convert.ToDouble(row[carrier]) + consumo;
                                if (dtRepAcumulado.Columns.Contains("Total"))
                                {
                                    double total = row["Total"].ToString().Length > 0 ? Convert.ToDouble(row["Total"]) : 0;
                                    row["Total"] = total + consumo;
                                }
                            }

                        }

                        dtRepAcumulado.AcceptChanges();
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////////////////

                return dtRepAcumulado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public Control CrearControlGridAcumulado(DataTable dtRepTipoCampania, string tituloGrid, string tituloGraf, int  pestañaActiva, string idContenedorGraf, string link, string linkLupa, bool showValue = false, bool showPercentage = false)
        {
            GridView grid = new GridView();
            Control objControl = new Control();

            try
            {
                int nCarriers = dtRepTipoCampania.Columns.Count > 2 ? Convert.ToInt32(dtRepTipoCampania.Columns.Count - 2) : Convert.ToInt32(dtRepTipoCampania.Columns.Count);
                string[] carriers = new string[nCarriers];
                string[] nomCarriers = new string[nCarriers];
                int[] indicesCarriers = new int[nCarriers];
                string[] formatos = new string[dtRepTipoCampania.Columns.Count];

                int[] indicesSinNavegacion;

                nomCarriers = FCAndControls.extraeNombreColumnas(dtRepTipoCampania);

                for (int i = 0; i < nCarriers; i++)
                {
                    indicesCarriers[i] = i + 3;
                }

                for (int i = 0; i < dtRepTipoCampania.Columns.Count; i++)
                {
                    if (indicesCarriers.Contains(i))
                    {
                        formatos[i] = "{0:c}";
                    }
                    else
                    {
                        formatos[i] = "";
                    }
                }

                string[] columnas = FCAndControls.extraeNombreColumnas(dtRepTipoCampania);
                indicesSinNavegacion = new int[(columnas.Count() - 3) > 0 ? (columnas.Count() - 3) : columnas.Count()];

                for (int i = 0; i < columnas.Count() - 3; i++)
                {
                    indicesSinNavegacion[i] = i + 3;
                }

                if (dtRepTipoCampania.Rows.Count > 0 && dtRepTipoCampania.Columns.Count > 0 && nCarriers > 0)
                {



                    objControl.Controls.Add(
                   DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                   DTIChartsAndControls.GridView("RepEnlacesTipoCampañaAcumulado", dtRepTipoCampania, true, "Totales",
                                   formatos, link,
                                   new string[] {"Fecha"}, 2, new int[] { 0, 1 }, indicesSinNavegacion, new int[] {2 }),
                                   idContenedorGraf, tituloGrid, linkLupa, pestañaActiva, FCGpoGraf.MatricialConStack)
                   );


                   //objControl.Controls.Add(
                   // DTIChartsAndControls.TituloYPestañasRep1Nvl(
                   // DTIChartsAndControls.GridView("RepEnlacesTipoCampañaAcumulado", dtRepTipoCampania, true, "Totales",
                   // formatos, "",
                   // new string[] { }, 2, new int[] { 0, 1 }, indicesSinNavegacion, new int[] { }),
                   // "Reporte", tituloGrid)
                   // ); 


                }


                return objControl;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarReporteTipoCampaniaFiltroCamp(string fechaIniRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoFechaInt, string link, string linkLupa, string codRep)
        {
            try
            {
                bool respuesta = false;
                Panel pnlRepTipoCampania = new Panel();

                DataTable dtRepTipoCampania = RepCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampania, campania, formatoFecha, formatoFechaInt));
                DataTable dtRepConsumoEnlacesPorCampaña = RepPorCampañaFormatoDataTableEnlacesPorCampaña(BuscarInfoEnlacesporCampaña(fechaIniRep, fechaFinRep, campania), dtRepTipoCampania);

                DataView dvRepAcumulado = new DataView(dtRepConsumoEnlacesPorCampaña);

                List<string> listaColumnasDeseadas = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesPorCampaña).ToList().Where(r => !r.Contains(" ")).Select(r => r).ToList();

                DataTable dtRepAcumulado = RepPorCampañaFormatoDataTableEnlacesPorCampañaAcumulado(dtRepConsumoEnlacesPorCampaña, dtRepTipoCampania, dvRepAcumulado.ToTable(false, listaColumnasDeseadas.ToArray()));

                List<string> listaSeries = new List<string>();

                foreach (DataColumn column in dtRepTipoCampania.Columns)
                {
                    if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                    {
                        listaSeries.Add(column.ColumnName);
                    }
                }



                Rep2.Controls.Clear();
                Rep2.Controls.Add(CrearRepTipoCampaniaTabContainer(dtRepTipoCampania, "Por campaña", "Por campaña", 3, "Rep2GrafTipoCamp", link, linkLupa));


                //Revisa si la cantidad de columnas es mayor a 5 para decir si encontro informacion para la campaña  actual
                if (dtRepConsumoEnlacesPorCampaña.Columns.Count <= 5)
                {
                    dtRepConsumoEnlacesPorCampaña.Clear();
                    dtRepAcumulado.Clear();

                }
                Rep4.Controls.Clear();
                Rep4.Controls.Add(CrearControlGridConsumoEnlacesPorCampaña(dtRepConsumoEnlacesPorCampaña, "Consumo enlaces por campaña"));


                Rep6.Controls.Clear();
                Rep6.Controls.Add(CrearControlGridConsumoEnlacesPorCampañaAcumulado(dtRepAcumulado, "Consumo enlaces por campaña acumulado"));

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable RepCampañaFormatoDatatableConsumo(DataTable dtRepTipoCampania)
        {
            try
            {
                //agregar columna de totales para el reporte de por campaña
                double importePorMes = 0;
                double importePorCarrierPorMes = 0;


                if (!dtRepTipoCampania.Columns.Contains("Total"))
                {
                    dtRepTipoCampania.Columns.Add("Total", typeof(double));
                }

                List<string> listaCarriers = new List<string>();
                listaCarriers = FCAndControls.extraeNombreColumnas(dtRepTipoCampania).ToList()
                                                    .Where(r =>
                                                            !(r.ToLower().Contains("fecha")) &&
                                                            !(r.ToLower().Contains("link")) &&
                                                            !(r.ToLower().Contains("id")) &&
                                                            !(r.ToLower().Contains("mes")) &&
                                                            !(r.ToLower().Contains("año"))
                                                           )
                                                    .Select(r => r.Split(' ')[0])
                                                    .Distinct().ToList();


                foreach (DataRow row in dtRepTipoCampania.Rows)
                {
                    importePorMes = 0;
                    foreach (DataColumn column in dtRepTipoCampania.Columns)
                    {
                        importePorCarrierPorMes = 0;
                        if (listaCarriers.Contains(column.ColumnName) && double.TryParse(row[column.ColumnName].ToString(), out importePorCarrierPorMes))
                        {
                            importePorMes += importePorCarrierPorMes;
                        }
                    }

                    row["Total"] = importePorMes;
                }

                return dtRepTipoCampania;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable RepPorCampañaFormatoDataTableEnlacesPorCampaña(DataTable dtRepConsumoEnlacesPorCampaña, DataTable dtRepTipoCampania)
        {
            try
            {

                foreach (DataColumn column in dtRepConsumoEnlacesPorCampaña.Columns)
                {
                    column.ColumnName = column.ColumnName.Replace("_", " ");
                }

                return dtRepConsumoEnlacesPorCampaña;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable RepPorCampañaFormatoDataTableEnlacesPorCampañaAcumulado(DataTable dtRepConsumoEnlacesPorCampaña, DataTable dtRepTipoCampania, DataTable dtRepAcumulado)
        {
            try
            {

                List<string> listaCarriers = new List<string>();
                listaCarriers = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesPorCampaña).ToList()
                                                    .Where(r =>
                                                            !(r.ToLower().Contains("fecha")) &&
                                                            !(r.ToLower().Contains("link")) &&
                                                            !(r.ToLower().Contains("id")) &&
                                                            !(r.ToLower().Contains("mes")) &&
                                                            !(r.ToLower().Contains("año"))
                                                           )
                                                    .Select(r => r.Split(' ')[0])
                                                    .Distinct().ToList();


                List<string> listaCarriersCampaña = new List<string>();
                listaCarriersCampaña = FCAndControls.extraeNombreColumnas(dtRepTipoCampania).ToList()
                                                    .Where(r =>
                                                            !(r.ToLower().Contains("fecha")) &&
                                                            !(r.ToLower().Contains("link")) &&
                                                            !(r.ToLower().Contains("id")) &&
                                                            !(r.ToLower().Contains("mes")) &&
                                                            !(r.ToLower().Contains("año"))
                                                           )
                                                    .Select(r => r.Split(' ')[0])
                                                    .Distinct().ToList();


                if (listaCarriers.Count > 0)
                {
                    listaCarriers = listaCarriers.Union(listaCarriersCampaña).ToList();

                    listaCarriers = listaCarriers.Union(listaCarriersCampaña).OrderBy(r => r).ToList();
                    /*Recorre el datatable de dtReoConsumoEnlacesPorCampaña*/
                    //////////////////////////////////////////////////////////

                    //Busca distintos Carriers en el datatable de Enlaces

                    List<string> listCarriersAgregarAcum = new List<string>();
                    listCarriersAgregarAcum = listaCarriers;

                    //listCarriersAgregarAcum = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesPorCampaña).ToList()
                    //                                .Where(r => r.Contains("_")).Select(r => r.Split('_')[0]).Distinct().ToList();

                    foreach (string carrierAgregar in listCarriersAgregarAcum)
                    {
                        dtRepAcumulado.Columns.Add(carrierAgregar.Trim(), typeof(decimal));
                    }
                    dtRepAcumulado.AcceptChanges();



                    string fechaIntActual = "";
                    double importe = 0;

                    List<string> listaCarriersEnlaces = new List<string>();
                    if (listCarriersAgregarAcum.Contains("Total"))
                    {
                        listCarriersAgregarAcum.Remove("Total");
                    }


                    //Ciclo  por carrier 
                    foreach (string carrier in listCarriersAgregarAcum)
                    {

                        foreach (DataRow row in dtRepConsumoEnlacesPorCampaña.Rows)
                        {
                            fechaIntActual = row["fechaInt"].ToString();
                            importe = 0;

                            foreach (DataRow rowdtTipoCampaña in dtRepTipoCampania.Rows)
                            {
                                if (dtRepTipoCampania.Columns.Contains("FechaInt"))
                                {
                                    if (rowdtTipoCampaña["FechaInt"].ToString().ToUpper() == fechaIntActual.ToUpper())
                                    {
                                        if (dtRepTipoCampania.Columns.Contains(carrier))
                                        {
                                            importe += Convert.ToDouble(rowdtTipoCampaña[carrier]);


                                        }
                                    }
                                }
                            }


                            foreach (DataColumn column in dtRepConsumoEnlacesPorCampaña.Columns)
                            {
                                if (column.ColumnName.ToUpper().Contains(carrier.ToUpper()))
                                {
                                    importe += Convert.ToDouble(row[column]);
                                }
                            }


                            foreach (DataRow rowAcumulado in dtRepAcumulado.Rows)
                            {
                                if (rowAcumulado["fechaInt"].ToString().ToUpper() == fechaIntActual.ToUpper())
                                {
                                    if (dtRepAcumulado.Columns.Contains(carrier))
                                    {
                                        rowAcumulado[carrier] = importe;
                                        if (dtRepAcumulado.Columns.Contains("Total"))
                                        {
                                            double total = rowAcumulado["Total"].ToString().Length > 0 ? Convert.ToDouble(rowAcumulado["Total"]) : 0;
                                            rowAcumulado["Total"] = total + importe;
                                        }
                                    }


                                    dtRepAcumulado.AcceptChanges();
                                }
                            }
                        }
                    }


                    //////////////////////////////////////////////////////////
                }
                return dtRepAcumulado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Control CrearRepTipoCampaniaTabContainer(DataTable dtRepTipoCampania, string tituloGrid, string tituloGraf, int pestaniaActiva, string idContenedorGraf, string link, string linkLupa, bool showValue = false, bool showPercentage = false)
        {
            try
            {
                Control objControl = new Control();

                int nCarriers = dtRepTipoCampania.Columns.Count > 3 ? Convert.ToInt32(dtRepTipoCampania.Columns.Count - 3) : Convert.ToInt32(dtRepTipoCampania.Columns.Count);
                string[] carriers = new string[nCarriers];
                string[] nomCarriers = new string[nCarriers];
                int[] indicesCarriers = new int[nCarriers];
                string[] formatos = new string[dtRepTipoCampania.Columns.Count];

                int[] indicesSinNavegacion;

                nomCarriers = FCAndControls.extraeNombreColumnas(dtRepTipoCampania);

                for (int i = 0; i < nCarriers; i++)
                {
                    indicesCarriers[i] = i + 3;
                }

                for (int i = 0; i < dtRepTipoCampania.Columns.Count; i++)
                {
                    if (indicesCarriers.Contains(i))
                    {
                        formatos[i] = "{0:c}";
                    }
                    else
                    {
                        formatos[i] = "";
                    }
                }


                if (link.Length == 0)
                {
                    indicesSinNavegacion = indicesCarriers.Union(new int[] { 2 }).ToArray();
                }
                else
                {
                    indicesSinNavegacion = indicesCarriers;
                }

                // "RepTipoCampaña_T"
                objControl.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView(tituloGrid, dtRepTipoCampania, true, "Totales",
                                    formatos, link,
                                    nomCarriers, 2, new int[] { 0, 1 }, indicesSinNavegacion, (link.Length > 0 ? new int[] { 2 } : new int[] { })),
                                    idContenedorGraf, tituloGrid, linkLupa, pestaniaActiva, FCGpoGraf.MatricialConStack)
                    );




                #region grafica
                string[] arrCarriers;
                DataTable[] arrDT;
                DataTable dtClone = dtRepTipoCampania.Clone();

                foreach (DataRow row in dtRepTipoCampania.Rows)
                {
                    dtClone.ImportRow(row);
                }


                if (dtRepTipoCampania.Columns.Contains("link"))
                {
                    dtRepTipoCampania.Columns.Remove("link");
                }

                if (dtRepTipoCampania.Rows.Count > 0 && dtRepTipoCampania.Columns.Count > 0)
                {
                    if (dtRepTipoCampania.Rows[dtRepTipoCampania.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                    {
                        dtRepTipoCampania.Rows[dtRepTipoCampania.Rows.Count - 1].Delete();
                    }
                    dtRepTipoCampania.AcceptChanges();

                    List<string> listaCarriers = new List<string>();

                    dtRepTipoCampania.Columns.Add("Categoria", typeof(string));

                    foreach (DataRow row in dtRepTipoCampania.Rows)
                    {
                        string[] fechaSplit = row["Fecha"].ToString().Split('/');
                        DateTime dtCat;
                        int year = 1990;
                        int month = 1;
                        int day = 1;

                        string cat = "";
                        if (fechaSplit.Count() == 2)
                        {
                            year = Convert.ToInt32(fechaSplit[1]);
                            month = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, 1);

                            cat = dtCat != null ? dtCat.ToString("MMMM") : "";
                        }
                        else
                        {
                            year = Convert.ToInt32(fechaSplit[2]);
                            month = Convert.ToInt32(fechaSplit[1]);
                            day = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, day);

                            cat = row["Fecha"].ToString();
                        }

                        row["Categoria"] = cat;
                    }



                    if (dtRepTipoCampania.Columns.Contains("Fecha"))
                    {
                        dtRepTipoCampania.Columns.Remove("Fecha");
                    }

                    if (dtRepTipoCampania.Columns.Contains("FechaInt"))
                    {
                        dtRepTipoCampania.Columns.Remove("FechaInt");
                    }

                    if (dtRepTipoCampania.Columns.Contains("Total"))
                    {
                        dtRepTipoCampania.Columns.Remove("Total");
                    }



                    dtRepTipoCampania.AcceptChanges();

                    arrCarriers = BucarCarriers(dtRepTipoCampania);
                    DataView dtv = new DataView(dtRepTipoCampania);
                    DataTable dtCarriers = dtv.ToTable(false, arrCarriers);

                    arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtCarriers);

                    foreach (DataRow row in dtClone.Rows)
                    {
                        if (row["Fecha"].ToString() == "Totales")
                        {
                            row.Delete();
                        }
                    }
                    dtClone.AcceptChanges();
                    List<string> listaFechasLink = dtClone.AsEnumerable().Select(r => r["Fecha"].ToString()).ToList<string>();
                    foreach (DataTable dt in arrDT)
                    {

                        if (dt.Columns.Contains("value"))
                        {
                            dt.Columns.Add("link", typeof(string));

                            int i = 0;
                            foreach (DataRow row in dt.Rows)
                            {

                                row["link"] = link.Replace("{2}", listaFechasLink[i]);
                                i++;
                            }
                        }
                        dt.AcceptChanges();
                    }

                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);

                    //20180718 Grafica de stack NZ

                    if (lsaDataSource != null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), idContenedorGraf,
                                         FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtCarriers), idContenedorGraf,
                                         "", "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.MatricialConStack,"$","","dti","98%","385",showValue,showPercentage), false);
                    }

                }
                #endregion

                return objControl; ;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Control CrearRepTipoCampaniaTabContainerLineSet(DataTable dtRepTipoCampania, string tituloGrid, string tituloGraf, int pestañaActiva, string idContenedorGraf, string link, string linkLupa)
        {
            try
            {
                TabContainer tcReportes = new TabContainer();
                TabPanel tabGrid = new TabPanel();
                TabPanel tabGraf = new TabPanel();

                tcReportes.CssClass = "MyTabStyle";
                DataTable dtClone = dtRepTipoCampania.Clone();
                foreach (DataRow row in dtRepTipoCampania.Rows)
                {
                    dtClone.ImportRow(row);
                }


                Control objControl = new Control();
                /*reportes nuevo LF  */
                int nCarriers = dtRepTipoCampania.Columns.Count > 3 ? Convert.ToInt32(dtRepTipoCampania.Columns.Count - 3) : Convert.ToInt32(dtRepTipoCampania.Columns.Count);
                string[] carriers = new string[nCarriers];
                string[] nomCarriers = new string[nCarriers];
                int[] indicesCarriers = new int[nCarriers];
                string[] formatos = new string[dtRepTipoCampania.Columns.Count];

                int[] indicesSinNavegacion;

                nomCarriers = FCAndControls.extraeNombreColumnas(dtRepTipoCampania);

                for (int i = 0; i < nCarriers; i++)
                {
                    indicesCarriers[i] = i + 3;
                }

                for (int i = 0; i < dtRepTipoCampania.Columns.Count; i++)
                {
                    if (indicesCarriers.Contains(i))
                    {
                        formatos[i] = "{0:c}";
                    }
                    else
                    {
                        formatos[i] = "";
                    }
                }


                indicesSinNavegacion = indicesCarriers.Union(new int[] { 2 }).ToArray();


                if (dtRepTipoCampania.Rows.Count > 0 && dtRepTipoCampania.Columns.Count > 0 && nCarriers > 0)
                {

                    objControl.Controls.Add(
                 DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                 DTIChartsAndControls.GridView("CrearRepTipoCampaniaTabContainerLineSet_T", dtRepTipoCampania, true, "Totales",
                                 formatos, link,
                                 nomCarriers, 2, new int[] { 0, 1 }, indicesSinNavegacion,  new int[] { }),
                                 idContenedorGraf, tituloGrid, linkLupa, pestañaActiva, FCGpoGraf.MatricialConStackLineaBase)
                 );
                }

                #region grafica
                string[] arrCarriers;
                DataTable[] arrDT;

                //foreach (DataRow row in dtRepTipoCampania.Rows)
                //{
                //    dtClone.ImportRow(row);
                //}


                if (dtRepTipoCampania.Columns.Contains("link"))
                {
                    dtRepTipoCampania.Columns.Remove("link");
                }

                if (dtRepTipoCampania.Rows.Count > 0 && dtRepTipoCampania.Columns.Count > 0)
                {
                    if (dtRepTipoCampania.Rows[dtRepTipoCampania.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                    {
                        dtRepTipoCampania.Rows[dtRepTipoCampania.Rows.Count - 1].Delete();
                    }
                    dtRepTipoCampania.AcceptChanges();

                    List<string> listaCarriers = new List<string>();

                    dtRepTipoCampania.Columns.Add("Categoria", typeof(string));

                    foreach (DataRow row in dtRepTipoCampania.Rows)
                    {
                        string[] fechaSplit = row["Fecha"].ToString().Split('/');
                        DateTime dtCat;
                        int year = 1990;
                        int month = 1;
                        int day = 1;

                        string cat = "";
                        if (fechaSplit.Count() == 2)
                        {
                            year = Convert.ToInt32(fechaSplit[1]);
                            month = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, 1);

                            cat = dtCat != null ? dtCat.ToString("MMMM") : "";
                        }
                        else
                        {
                            year = Convert.ToInt32(fechaSplit[2]);
                            month = Convert.ToInt32(fechaSplit[1]);
                            day = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, day);

                            cat = row["Fecha"].ToString();
                        }

                        row["Categoria"] = cat;
                    }



                    if (dtRepTipoCampania.Columns.Contains("Fecha"))
                    {
                        dtRepTipoCampania.Columns.Remove("Fecha");
                    }

                    if (dtRepTipoCampania.Columns.Contains("FechaInt"))
                    {
                        dtRepTipoCampania.Columns.Remove("FechaInt");
                    }

                    if (dtRepTipoCampania.Columns.Contains("Total"))
                    {
                        dtRepTipoCampania.Columns.Remove("Total");
                    }



                    dtRepTipoCampania.AcceptChanges();

                    arrCarriers = BucarCarriers(dtRepTipoCampania);
                    DataView dtv = new DataView(dtRepTipoCampania);
                    DataTable dtCarriers = dtv.ToTable(false, arrCarriers);

                    arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtCarriers);

                    foreach (DataRow row in dtClone.Rows)
                    {
                        if (row["Fecha"].ToString() == "Totales")
                        {
                            row.Delete();
                        }
                    }
                    dtClone.AcceptChanges();
                    List<string> listaFechasLink = dtClone.AsEnumerable().Select(r => r["Fecha"].ToString()).ToList<string>();
                    foreach (DataTable dt in arrDT)
                    {

                        if (dt.Columns.Contains("value"))
                        {
                            dt.Columns.Add("link", typeof(string));

                            int i = 0;
                            foreach (DataRow row in dt.Rows)
                            {

                                row["link"] = link.Replace("{2}", listaFechasLink[i]);
                                i++;
                            }
                        }
                        dt.AcceptChanges();
                    }

                    if (arrDT[arrDT.Count() - 1].Columns.Contains("link"))
                    {
                        arrDT[arrDT.Count() - 1].Columns.Remove("link");
                    }
                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);
                    int yMaxVal = BuscarMax(dtClone);
                    pestañaActiva = 0;

                    idContenedorGraf = "graf_" + idContenedorGraf + "_msstackedcolumn2dlinedy";
                    if (lsaDataSource != null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), idContenedorGraf,
                                         FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtCarriers), idContenedorGraf,
                                         "", "", "Mes", "Importe", pestañaActiva, FCGpoGraf.MatricialConStackLineaBase, yMaxVal,"Presupuesto"), false);
                    }

                }
                #endregion

                //////////////////


                //tabGrid = CrearTabPanelGrid(dtClone, tituloGrid, link, linkLupa);



                //if (dtRepTipoCampania.Columns.Contains("Total"))
                //{
                //    dtRepTipoCampania.Columns.Remove("Total");
                //}

                //tabGraf = CrearTabPanelGrafLineSet(dtRepTipoCampania, tituloGraf, tipoGrafDefault, idContenedorGraf, link, linkLupa);


                //tcReportes.Controls.Add(tabGrid);
                //tcReportes.Controls.Add(tabGraf);

               

                return objControl; ;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public TabPanel CrearTabPanelGrid(DataTable dtRep, string tituloGrid, string link, string linkLupa)
        {
            TabPanel tabGrid = new TabPanel();
            Control objControl = new Control();
            try
            {
                tabGrid.HeaderText = "tabla";

                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0)
                {

                    objControl.Controls.Add(CrearControlGrid(dtRep, tituloGrid, link, linkLupa));

                    tabGrid.Controls.Clear();
                    tabGrid.Controls.Add(objControl);

                }
                else
                {
                    Label sinInfo = new Label();
                    sinInfo.Text = "No hay información por mostrar";
                    tabGrid.Controls.Add(TituloYBordesReporte(sinInfo, 20));

                }


                return tabGrid;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public TabPanel CrearTabPanelGraf(DataTable dtRep, string tituloGraf, int pestañaActiva, string idContenedorGraf, string link, string linkLupa)
        {
            try
            {



                TabPanel tabGraf = new TabPanel();

                tabGraf.HeaderText = "Gráfica";


                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0)
                {
                    tabGraf.Controls.Add(CrearControlGraf(dtRep, tituloGraf, pestañaActiva, idContenedorGraf, link, linkLupa));
                }

                return tabGraf;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public GridView CrearGridView
            (DataTable dtRep, string tituloGrid, string link, string[] urlFields,
            int indexCeldaTotales, int[] arregloInicesNoMostrarEnWeb, int[] arregloIndicesSinNavegacino, int[] arregloIndicesConNavegacion, string[] formatos)
        {
            try
            {
                GridView grid = new GridView();
                Control objControl = new Control();



                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0)
                {
                    grid = DTIChartsAndControls.GridView(
                           tituloGrid,
                           dtRep,
                           true,
                           "Totales",
                           formatos,
                           link,
                           urlFields,
                           indexCeldaTotales,
                           arregloInicesNoMostrarEnWeb,
                           arregloIndicesSinNavegacino,
                           arregloIndicesConNavegacion
                           );

                    grid.Rows[grid.Rows.Count - 1].CssClass = "titulosReportes totalesBnrt";


                }

                return grid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TabPanel CrearTabPanelGrafLineSet(DataTable dtRep, string tituloGraf, int pestañaActiva, string idContenedorGraf, string link, string linkLupa)
        {
            try
            {



                TabPanel tabGraf = new TabPanel();

                tabGraf.HeaderText = "Gráfica";


                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0)
                {
                    tabGraf.Controls.Add(CrearControlGrafLineSet(dtRep, tituloGraf, pestañaActiva, idContenedorGraf, link, linkLupa));
                }

                return tabGraf;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Control CrearControlGrafLineSet(DataTable dtRep, string tituloGraf, int  pestañaActiva, string idContenedorGraf, string link, string linkLupa)
        {
            try
            {
                Control objControl = new Control();


                string[] arrCarriers;
                DataTable[] arrDT;
                DataTable dtClone = dtRep.Clone();

                foreach (DataRow row in dtRep.Rows)
                {
                    dtClone.ImportRow(row);
                }


                if (dtRep.Columns.Contains("link"))
                {
                    dtRep.Columns.Remove("link");
                }

                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0)
                {
                    if (dtRep.Rows[dtRep.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                    {
                        dtRep.Rows[dtRep.Rows.Count - 1].Delete();
                    }
                    dtRep.AcceptChanges();

                    List<string> listaCarriers = new List<string>();

                    dtRep.Columns.Add("Categoria", typeof(string));

                    foreach (DataRow row in dtRep.Rows)
                    {
                        string[] fechaSplit = row["Fecha"].ToString().Split('/');
                        DateTime dtCat;
                        int year = 1990;
                        int month = 1;
                        int day = 1;

                        string cat = "";
                        if (fechaSplit.Count() == 2)
                        {
                            year = Convert.ToInt32(fechaSplit[1]);
                            month = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, 1);

                            cat = dtCat != null ? dtCat.ToString("MMMM") : "";
                        }
                        else
                        {
                            year = Convert.ToInt32(fechaSplit[2]);
                            month = Convert.ToInt32(fechaSplit[1]);
                            day = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, day);

                            cat = row["Fecha"].ToString();
                        }

                        row["Categoria"] = cat;
                    }



                    if (dtRep.Columns.Contains("Fecha"))
                    {
                        dtRep.Columns.Remove("Fecha");
                    }

                    if (dtRep.Columns.Contains("FechaInt"))
                    {
                        dtRep.Columns.Remove("FechaInt");
                    }

                    if (dtRep.Columns.Contains("Total"))
                    {
                        dtRep.Columns.Remove("Total");
                    }



                    dtRep.AcceptChanges();

                    arrCarriers = BucarCarriers(dtRep);
                    DataView dtv = new DataView(dtRep);
                    DataTable dtCarriers = dtv.ToTable(false, arrCarriers);

                    arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtCarriers);

                    foreach (DataRow row in dtClone.Rows)
                    {
                        if (row["Fecha"].ToString() == "Totales")
                        {
                            row.Delete();
                        }
                    }
                    dtClone.AcceptChanges();
                    List<string> listaFechasLink = dtClone.AsEnumerable().Select(r => r["Fecha"].ToString()).ToList<string>();
                    foreach (DataTable dt in arrDT)
                    {

                        if (dt.Columns.Contains("value"))
                        {
                            dt.Columns.Add("link", typeof(string));

                            int i = 0;
                            foreach (DataRow row in dt.Rows)
                            {

                                row["link"] = link.Replace("{2}", listaFechasLink[i]);
                                i++;
                            }
                        }
                        dt.AcceptChanges();
                    }

                    if (arrDT[arrDT.Count() - 1].Columns.Contains("link"))
                    {
                        arrDT[arrDT.Count() - 1].Columns.Remove("link");
                    }
                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);
                    int yMaxVal = BuscarMax(dtClone);
                    //20180718 Grafica de stack NZ

                    //objControl.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                    //    FCAndControls.CreaContenedorGraficaYRadioButtonsGrafMultiSerieExtenLineSet(idContenedorGraf, "ControlesAlCentro", tipoGrafDefault)),
                    //                                    "Gráfica", tituloGraf, 450, linkLupa));

                    //if (lsaDataSource != null)
                    //{
                    //    Page.ClientScript.RegisterStartupScript(this.GetType(), idContenedorGraf,
                    //        FCAndControls.graficaMultiSeriesLineSet(lsaDataSource, FCAndControls.extraeNombreColumnas(dtCarriers), tipoGrafDefault, idContenedorGraf,
                    //                     "", "", "Mes", "Importe", "Presupuesto", "ocean", "98%", "330", idContenedorGraf, true, yMaxVal), false);
                    //}

                    //20180718 Grafica de stack NZ
                    objControl.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(idContenedorGraf, tituloGraf, pestañaActiva, FCGpoGraf.MatricialConStackLineaBase));

                    if (lsaDataSource != null)
                    {
                        idContenedorGraf = "graf_" + idContenedorGraf + "_msstackedcolumn2dlinedy";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), idContenedorGraf,
                                         FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtCarriers), idContenedorGraf,
                                         "", "", "Mes", "Importe", pestañaActiva, FCGpoGraf.MatricialConStackLineaBase,yMaxVal,"Presupuesto"), false);
                    }

                }



                return objControl;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int BuscarMax(DataTable dtRep)
        {
            try
            {
                double yFinalMaxVal = 0;

                if (dtRep.Columns.Contains("link"))
                {
                    dtRep.Columns.Remove("link");
                }

                if (dtRep.Columns.Contains("fecha"))
                {
                    dtRep.Columns.Remove("fecha");
                }

                if (dtRep.Columns.Contains("Fecha"))
                {
                    dtRep.Columns.Remove("Fecha");
                }

                if (dtRep.Columns.Contains("FechaInt"))
                {
                    dtRep.Columns.Remove("FechaInt");
                }




                double yMaxfinal = 0;
                double yMaxRow = 0;
                double yMaxcarrier = 0;
                double yMaxPpto = 0;

                foreach (DataRow row in dtRep.Rows)
                {
                    yMaxRow = 0;
                    yMaxPpto = 0;
                    yMaxcarrier = 0;
                    foreach (DataColumn col in dtRep.Columns)
                    {
                        double ycol = 0;
                        if (col.ColumnName != "Presupuesto")
                        {
                            if (double.TryParse(row[col].ToString(), out ycol))
                            {
                                yMaxcarrier += ycol;
                            }
                        }
                        else
                        {
                            if (double.TryParse(row[col].ToString(), out ycol))
                            {
                                yMaxPpto += ycol;
                            }
                        }

                    }
                    yMaxRow = yMaxcarrier > yMaxPpto ? yMaxcarrier : yMaxPpto;
                    yMaxfinal = yMaxfinal > yMaxRow ? yMaxfinal : yMaxRow;
                }

                yFinalMaxVal = Math.Round(yMaxfinal);
                return Convert.ToInt32(yFinalMaxVal);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable[] crearArrDT(DataTable dtRep, List<string> listaSeries)
        {
            try
            {
                DataTable[] dta = new DataTable[listaSeries.Count];

                for (int i = 0; i < listaSeries.Count; i++)
                {
                    DataView dtv = new DataView(dtRep);

                    DataTable dt = dtv.ToTable(true, listaSeries[i]);

                    dta[i] = dt;
                }


                return dta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static Control TituloYBordesReporte(Control control, int height)
        {
            Panel panel = new Panel();
            panel.HorizontalAlign = HorizontalAlign.Center;
            if (height > 0)
            {
                panel.CssClass = "PanelTitulosYBordeReportes maxWidth100Perc";
                panel.Height = height;
            }
            else
            {
                panel.CssClass = "PanelTitulosYBordeReportes AutoHeight maxWidth100Perc";
            }
            panel.Controls.Add(control);

            return panel;
        }

        public DataTable BuscarCampanias()
        {
            try
            {
                DataTable dtCampanias = new DataTable();

                dtCampanias = DSODataAccess.Execute(ConsultaBuscarCampanias());

                return dtCampanias;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] BucarCarriers(DataTable dtRep)
        {
            try
            {
                List<string> listaCarriers = new List<string>();
                foreach (DataColumn column in dtRep.Columns)
                {
                    if (column.ColumnName == "Categoria")
                    {
                        listaCarriers.Add(column.ColumnName);
                    }
                }

                foreach (DataColumn column in dtRep.Columns)
                {
                    if (column.ColumnName != "Fecha" && column.ColumnName != "FechaInt" && column.ColumnName != "Categoria")
                    {
                        listaCarriers.Add(column.ColumnName);
                    }
                }

                return listaCarriers.ToArray();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void BuscarFormatosFechas(int cantMeses, DateTime fechaInicial)
        {
            try
            {


                if (cantMeses != 1)
                {
                    fechaIniRep = fechaInicial.AddMonths(-1 * (cantMeses - 1)).ToString("yyyy-MM-01 00:00:00");
                    fechaFinRep = Convert.ToDateTime(fechaInicial.AddMonths(1).ToString("yyyy-MM-01 00:00:00")).AddDays(-1).ToString("yyyy-MM-dd 23:59:29");

                    formatoFecha = "Case When Month >= 10 Then Convert(varchar,Month) Else (''0''+ convert(varchar,Month)) End  + ''/'' + convert(varchar,year)";
                    formatoFechaInt = "convert(varchar,year)+ Case When month >= 10 Then (convert(varchar,month)) Else (''0''+Convert(varchar,Month)) End ";
                }
                else
                {
                    fechaIniRep = fechaInicial.ToString("yyyy-MM-01 00:00:00");
                    fechaFinRep = Convert.ToDateTime(fechaInicial.AddMonths(1).ToString("yyyy-MM-01 00:00:00")).AddDays(-1).ToString("yyyy-MM-dd 23:59:29");

                    formatoFecha = "Case When day >= 10 Then Convert(varchar,day) Else (''0''+ convert(varchar,day)) End  + ''/'' + Case When Month >= 10 Then Convert(varchar,Month) Else (''0''+ convert(varchar,Month)) End  + ''/'' + convert(varchar,year)";
                    formatoFechaInt = "convert(varchar,year)+ Case When month >= 10 Then (convert(varchar,month)) Else (''0''+Convert(varchar,Month)) End +Case When day >= 10 Then Convert(varchar,day) Else (''0''+ convert(varchar,day)) End";
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable BuscarInfoRepCampania(string fechainiRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoechaInt)
        {
            try
            {
                DataTable dtRep = new DataTable();

                dtRep = DSODataAccess.Execute(ConsultaBuscaInfoRepCampania(fechainiRep, fechaFinRep, tipoCampania, campania, formatoFecha, formatoechaInt));

                return dtRep;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable BuscarInfoEnlacesFijos(string fechaIniRep, string fechaFinRep)
        {
            try
            {
                DataTable dtRep = new DataTable();

                dtRep = DSODataAccess.Execute(ConsultaBuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep));

                return dtRep;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable BuscarInfoEnlacesporCampaña(string fechaIniRep, string fechaFinRep, int IDCampaña)
        {
            try
            {
                DataTable dtRep = new DataTable();

                dtRep = DSODataAccess.Execute(ConsultaBuscarInfoEnlacesporCampaña(fechaIniRep, fechaFinRep, IDCampaña));

                return dtRep;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] BuscarFiltrosTipoCampania()
        {
            try
            {
                string[] tiposCampania = new string[] { "Todas", "IN", "Out" };

                return tiposCampania;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Control CrearControlGrid(DataTable dtRep, string tituloGrid, string link, string linkLupa)
        {
            try
            {
                GridView grid = new GridView();
                Control objControl = new Control();

                int nCarriers = dtRep.Columns.Count > 3 ? Convert.ToInt32(dtRep.Columns.Count - 3) : Convert.ToInt32(dtRep.Columns.Count);
                string[] carriers = new string[nCarriers];
                string[] nomCarriers = new string[nCarriers];
                int[] indicesCarriers = new int[nCarriers];
                string[] formatos = new string[dtRep.Columns.Count];

                int[] indicesSinNavegacion;

                nomCarriers = FCAndControls.extraeNombreColumnas(dtRep);

                for (int i = 0; i < nCarriers; i++)
                {
                    indicesCarriers[i] = i + 3;
                }

                for (int i = 0; i < dtRep.Columns.Count; i++)
                {
                    if (indicesCarriers.Contains(i))
                    {
                        formatos[i] = "{0:c}";
                    }
                    else
                    {
                        formatos[i] = "";
                    }
                }


                if (link.Length == 0)
                {
                    indicesSinNavegacion = indicesCarriers.Union(new int[] { 2 }).ToArray();
                }
                else
                {
                    indicesSinNavegacion = indicesCarriers;
                }

                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0 && nCarriers > 0)
                {

                    objControl.Controls.Add(
                              DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                              DTIChartsAndControls.GridView(tituloGrid, dtRep, true, "Totales",
                                              formatos, link,
                                              nomCarriers, 2, new int[] { 0, 1 }, indicesSinNavegacion, (link.Length > 0 ? new int[] { 2 } : new int[] { })),
                                              "CrearControlGrid_T", tituloGrid,linkLupa)
                                              );

                }


                return objControl;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Control CrearControlGridConsumoMensualEnlaces(DataTable dtRep, string tituloGrid)
        {
            try
            {
                GridView grid = new GridView();
                Control objControl = new Control();

                int nCarriers = dtRep.Columns.Count - 1;
                string[] carriers = new string[nCarriers];
                string[] nomCarriers = new string[nCarriers];
                int[] indicesCarriers = new int[nCarriers];
                string[] formatos = new string[dtRep.Columns.Count];

                int[] indicesSinNavegacion;

                nomCarriers = FCAndControls.extraeNombreColumnas(dtRep);


                formatos = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" };


                indicesSinNavegacion = new int[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 };

                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0 && nCarriers > 0)
                {

                    objControl.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepEnlacesTipoCampaña_T", dtRep, true, "Totales",
                    formatos, "",
                    new string[] { }, 1, new int[] { 0, 8, 11 }, indicesSinNavegacion, new int[] { }),
                    "CrearControlGridConsumoMensualEnlaces_T", tituloGrid)
                    );


                    //grid = CrearGridView(dtRep, tituloGrid, "", nomCarriers, 1, new int[] { 0, 8, 11 }, indicesSinNavegacion, new int[] { }, formatos);

                    //grid.Rows[grid.Rows.Count - 1].CssClass = "titulosReportes totalesBnrt";
                    //objControl = DTIChartsAndControls.vistasGridView(grid, true, 438);



                }

                return objControl;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Control CrearControlGridConsumoEnlacesPorCampañaAcumulado(DataTable dtRep, string tituloGrid)
        {
            try
            {
                GridView grid = new GridView();
                Control objControl = new Control();

                int nCarriers = dtRep.Columns.Count;
                string[] carriers = new string[nCarriers];
                string[] nomCarriers = new string[nCarriers];
                int[] indicesCarriers = new int[nCarriers];
                string[] formatos = new string[dtRep.Columns.Count];

                int[] indicesSinNavegacion = new int[nCarriers - 4];


                if (dtRep.Columns.Count > 5)
                {
                    nomCarriers = FCAndControls.extraeNombreColumnas(dtRep);



                    for (int i = 0; i < nCarriers; i++)
                    {

                        if (i < 5)
                        {
                            formatos[i] = "";
                        }
                        else
                        {
                            formatos[i] = "{0:c}";
                        }

                        if (i >= 4)
                        {
                            indicesSinNavegacion[i - 4] = i;
                        }


                    }
                }

                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0 && nCarriers > 0)
                {
                }

                objControl.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                DTIChartsAndControls.GridView("RepEnlacesCampañaAcumulado_T", dtRep, true, "Totales",
                formatos, "",
                new string[] { }, 4, new int[] { 0, 1, 2, 3 }, indicesSinNavegacion, new int[] { }),
                "CrearControlGridConsumoEnlacesPorCampañaAcumulado_T", tituloGrid)
                );

                //grid = CrearGridView(dtRep, tituloGrid, "", nomCarriers, 4, new int[] { 0, 1, 2, 3 }, indicesSinNavegacion, new int[] { }, formatos);

                //grid.Rows[grid.Rows.Count - 1].CssClass = "titulosReportes totalesBnrt";
                //objControl = DTIChartsAndControls.vistasGridView(grid, true, 438);





                return objControl;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Control CrearControlGridConsumoEnlacesPorCampaña(DataTable dtRep, string tituloGrid)
        {
            try
            {
                GridView grid = new GridView();
                Control objControl = new Control();


                int nCarriers = dtRep.Columns.Count;
                string[] carriers = new string[nCarriers];
                string[] nomCarriers = new string[nCarriers];
                int[] indicesCarriers = new int[nCarriers];
                string[] formatos = new string[dtRep.Columns.Count];

                int[] indicesSinNavegacion = new int[nCarriers - 4];

                if (dtRep.Columns.Count > 5)
                {


                    nomCarriers = FCAndControls.extraeNombreColumnas(dtRep);

                    for (int i = 0; i < nCarriers; i++)
                    {
                        if (i <= 4)
                        {
                            formatos[i] = "";
                        }
                        else
                        {
                            formatos[i] = "{0:c}";

                        }

                        if (i >= 4)
                        {
                            indicesSinNavegacion[i - 4] = i;
                        }
                    }

                }
                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0 && nCarriers > 0)
                {
                }


                objControl.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                DTIChartsAndControls.GridView("RepEnlacesCampaña_T", dtRep, true, "Totales",
                formatos, "",
                new string[] { }, 4, new int[] { 0, 1, 2, 3 }, indicesSinNavegacion, new int[] { }),
                "CrearControlGridConsumoEnlacesPorCampaña_T", tituloGrid)
                );

                //grid = CrearGridView(dtRep, tituloGrid, "", nomCarriers, 4, new int[] { 0, 1, 2, 3 }, indicesSinNavegacion, new int[] { }, formatos);

                //grid.Rows[grid.Rows.Count - 1].CssClass = "titulosReportes totalesBnrt";
                //objControl = DTIChartsAndControls.vistasGridView(grid, true, 438);





                return objControl;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Control CrearControlGraf(DataTable dtRep, string tituloGraf, int grafActiva, string idContenedorGraf, string link, string linkLupa, bool showValue = false, bool showPercentage = false)
        {
            try
            {
                Control objControl = new Control();


                string[] arrCarriers;
                DataTable[] arrDT;
                DataTable dtClone = dtRep.Clone();

                foreach (DataRow row in dtRep.Rows)
                {
                    dtClone.ImportRow(row);
                }


                if (dtRep.Columns.Contains("link"))
                {
                    dtRep.Columns.Remove("link");
                }

                if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0)
                {
                    if (dtRep.Rows[dtRep.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                    {
                        dtRep.Rows[dtRep.Rows.Count - 1].Delete();
                    }
                    dtRep.AcceptChanges();

                    List<string> listaCarriers = new List<string>();

                    dtRep.Columns.Add("Categoria", typeof(string));

                    foreach (DataRow row in dtRep.Rows)
                    {
                        string[] fechaSplit = row["Fecha"].ToString().Split('/');
                        DateTime dtCat;
                        int year = 1990;
                        int month = 1;
                        int day = 1;

                        string cat = "";
                        if (fechaSplit.Count() == 2)
                        {
                            year = Convert.ToInt32(fechaSplit[1]);
                            month = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, 1);

                            cat = dtCat != null ? dtCat.ToString("MMMM") : "";
                        }
                        else
                        {
                            year = Convert.ToInt32(fechaSplit[2]);
                            month = Convert.ToInt32(fechaSplit[1]);
                            day = Convert.ToInt32(fechaSplit[0]);

                            dtCat = new DateTime(year, month, day);

                            cat = row["Fecha"].ToString();
                        }

                        row["Categoria"] = cat;
                    }



                    if (dtRep.Columns.Contains("Fecha"))
                    {
                        dtRep.Columns.Remove("Fecha");
                    }

                    if (dtRep.Columns.Contains("FechaInt"))
                    {
                        dtRep.Columns.Remove("FechaInt");
                    }

                    if (dtRep.Columns.Contains("Total"))
                    {
                        dtRep.Columns.Remove("Total");
                    }



                    dtRep.AcceptChanges();

                    arrCarriers = BucarCarriers(dtRep);
                    DataView dtv = new DataView(dtRep);
                    DataTable dtCarriers = dtv.ToTable(false, arrCarriers);

                    arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtCarriers);

                    foreach (DataRow row in dtClone.Rows)
                    {
                        if (row["Fecha"].ToString() == "Totales")
                        {
                            row.Delete();
                        }
                    }
                    dtClone.AcceptChanges();
                    List<string> listaFechasLink = dtClone.AsEnumerable().Select(r => r["Fecha"].ToString()).ToList<string>();
                    foreach (DataTable dt in arrDT)
                    {

                        if (dt.Columns.Contains("value"))
                        {
                            dt.Columns.Add("link", typeof(string));

                            int i = 0;
                            foreach (DataRow row in dt.Rows)
                            {

                                row["link"] = link.Replace("{2}", listaFechasLink[i]);
                                i++;
                            }
                        }
                        dt.AcceptChanges();
                    }

                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);

                    //20180718 Grafica de stack NZ
                    objControl.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(idContenedorGraf, tituloGraf, grafActiva, FCGpoGraf.MatricialConStack));

                    if (lsaDataSource != null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), idContenedorGraf,
                                         FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtCarriers), idContenedorGraf,
                                         "", "", "Mes", "Importe", grafActiva, FCGpoGraf.MatricialConStack,"$","","dti","98%","385",showValue,showPercentage), false);
                    }

                }



                return objControl;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaBuscarCampanias()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("");

                query.AppendLine("use keytia5							");
                query.AppendLine("Select campania.iCodCatalogo, campania.Descripcion								");
                query.AppendLine("From Pentafon.[VisHistoricos('Campaña','Campaña Padre','Español')] campania		");
                query.AppendLine("inner Join Pentafon.[VisRelaciones('Campaña - Sitio','Español')] relCS			");
                query.AppendLine("	On campania.iCodCatalogo = relCS.Campaña										");
                query.AppendLine("Inner Join pentafon.RestSitio restSitio											");
                query.AppendLine("	On relCS.Sitio = restSitio.Sitio												");
                query.AppendLine("  And restSitio.fechaInicio <= '" + fechaIniRep + "'");
                query.AppendLine(" And restSitio.fechaFin > '" + fechaFinRep + "'");
                query.AppendLine("And restSitio.Usuar = " + Session["iCodUsuario"] + "    							");
                query.AppendLine("group by campania.iCodCatalogo, campania.Descripcion								");
                query.AppendLine("Order by campania.descripcion");

                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaBuscaInfoRepCampania(string fechainiRep, string fechaFinRep, string tipoCampania, int campania, string formatoFecha, string formatoechaInt)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                string link = Request.Path + "?feha=''+[fecha]+''&tipoCamp=" + tipoCampania + "&camp=" + campania + "";

                query.AppendLine("");
                query.AppendLine("Exec [ConsumoMatCampaniaPentafon]    ");
                query.AppendLine("@esquema			= '" + DSODataContext.Schema + "' ,     ");
                query.AppendLine("@fechaInicioReporte = '" + fechainiRep + "',  ");
                query.AppendLine("@fechaFinReporte	= '" + fechaFinRep + "' ,   ");
                query.AppendLine("@tipoCampania       = '" + tipoCampania + "' ,      ");
                query.AppendLine("@campaña            = " + campania + ",             ");
                query.AppendLine("@formatoFecha		= '" + formatoFecha + "',  ");
                query.AppendLine("@formatoFechaInt	= '" + formatoechaInt + "',  ");
                query.AppendLine("@link = '" + link + "',");
                query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
                query.AppendLine("@Perfil = " + Session["iCodPerfil"] + "");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaBuscarInfoEnlacesporCampaña(string fechaIniRep, string fechaFinRep, int IDCampaña)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("Exec ConsumoMensualEnlacesPorCampañaPentafon        ");
            query.AppendLine("@esquema		=	'" + DSODataContext.Schema + "'	, ");
            query.AppendLine("@fechaIniRep	=	'" + fechaIniRep + "',            ");
            query.AppendLine("@fechaFinRep	=	'" + fechaFinRep + "',            ");
            query.AppendLine("@campañaID		=   " + IDCampaña.ToString() + ",  ");
            query.AppendLine("@DescReporte	=	'Reporte por Campaña'              ");


            return query.ToString();
        }

        public string ConsultaBuscarInfoEnlacesFijos(string fechaIniRep, string fechaFinRep)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("Exec ConsumoMensualEnlacesPentafon			");
            query.AppendLine("@esquema  = '" + DSODataContext.Schema + "',	");
            query.AppendLine("@fechaInicioReporte = '" + fechaIniRep + "',		");
            query.AppendLine("@fechaFinReporte = '" + fechaFinRep + "'			");
            return query.ToString();
        }

        public void RemoveControls(Panel panel)
        {
            try
            {
                foreach (Control control in panel.Controls)
                {
                    panel.Controls.Remove(control);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ExportXLS()
        {
            CrearXLS(".xlsx");
        }

        public void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();

            string codRepExp = "";
            string tipoCampaniaRep1Exp = "";
            string tipoCampaniaRep2Exp = "";
            string tipoCampaniaRep3Exp = "";
            string campaniaExp = "";
            string fechaExp = "";
            int mesQSExp = Convert.ToInt32(DateTime.Now.ToString("MM"));
            int anioQSExp = Convert.ToInt32(DateTime.Now.ToString("yyyy"));


            if (!string.IsNullOrEmpty(Request.QueryString["codRep"]))
            {
                codRepExp = Request.QueryString["codRep"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["filtroTipoCampR1"]))
            {
                tipoCampaniaRep1Exp = Request.QueryString["filtroTipoCampR1"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["filtroTipoCampR2"]))
            {
                tipoCampaniaRep2Exp = Request.QueryString["filtroTipoCampR2"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["camp"]))
            {
                campaniaExp = Request.QueryString["camp"];
            }
            if (Convert.ToInt32(campaniaExp) == 0)
            {
                tipoCampaniaRep3Exp = tipoCampaniaRep1Exp;
            }
            else
            {
                tipoCampaniaRep3Exp = tipoCampaniaRep2Exp;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["fecha"]))
            {
                fechaExp = Request.QueryString["fecha"];
                string[] camposFechaExp = fechaExp.Split('/');

                if (camposFechaExp.Count() == 2)
                {
                    mesQSExp = Convert.ToInt32(camposFechaExp[0]);
                    anioQSExp = Convert.ToInt32(camposFechaExp[1]);
                }
            }




            try
            {

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                DataTable dtRepTipoCampania = new DataTable();
                List<string> listaSeries = new List<string>();

                DataTable dtRepConsumoEnlacesMensual = new DataTable();
                DataTable dtRepAcumulado = new DataTable();

                bool isCombChart = false;

                switch (codRepExp)
                {
                    case "TC":

                        BuscarFormatosFechas(6, DateTime.Now);
                        //dtRepTipoCampania = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1Exp, 0, formatoFecha, formatoFechaInt);

                        dtRepTipoCampania = RepTipoCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1Exp, 0, formatoFecha, formatoFechaInt));
                        dtRepConsumoEnlacesMensual = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtRepTipoCampania);
                        dtRepAcumulado = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtRepConsumoEnlacesMensual, dtRepTipoCampania, dtRepTipoCampania.Clone());
                        isCombChart = false;


                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte3TablasY2GraficosPentafon" + lsExt);
                        break;
                    case "TCC":
                        BuscarFormatosFechas(6, DateTime.Now);
                        //dtRepTipoCampania = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep2Exp, Convert.ToInt32(campaniaExp), formatoFecha, formatoFechaInt);

                        dtRepTipoCampania = RepCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep2Exp, campania, formatoFecha, formatoFechaInt));
                        dtRepConsumoEnlacesMensual = RepPorCampañaFormatoDataTableEnlacesPorCampaña(BuscarInfoEnlacesporCampaña(fechaIniRep, fechaFinRep, Convert.ToInt32(campaniaExp)), dtRepTipoCampania);
                        DataView dvRepAcumulado = new DataView(dtRepConsumoEnlacesMensual);
                        List<string> listaColumnasDeseadas = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesMensual).ToList().Where(r => !r.Contains(" ")).Select(r => r).ToList();
                        dtRepAcumulado = RepPorCampañaFormatoDataTableEnlacesPorCampañaAcumulado(dtRepConsumoEnlacesMensual, dtRepTipoCampania, dvRepAcumulado.ToTable(false, listaColumnasDeseadas.ToArray()));
                        isCombChart = false;



                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte3TablasYGraficoPentafon" + lsExt);
                        break;
                    case "TCD":
                        DateTime dT = new DateTime(anioQSExp, mesQSExp, 1);
                        BuscarFormatosFechas(1, dT);
                        dtRepTipoCampania = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep3Exp, Convert.ToInt32(campaniaExp), formatoFecha, formatoFechaInt);

                        DataTable dtPresupuestosporMes = new DataTable();

                        dtPresupuestosporMes = BuscaInfoPptoPorCampaña(fechaIniRep, Convert.ToInt32(campaniaExp));


                        foreach (DataRow rowdtRepTipoCampaña in dtRepTipoCampania.Rows)
                        {
                            if (!dtRepTipoCampania.Columns.Contains("Presupuesto"))
                            {
                                dtRepTipoCampania.Columns.Add("Presupuesto", typeof(float));
                            }
                            dtRepTipoCampania.AcceptChanges();

                            foreach (DataRow rowPpto in dtPresupuestosporMes.Rows)
                            {
                                if (dtRepTipoCampania.Columns.Contains("FechaInt") && dtPresupuestosporMes.Columns.Contains("FechaInt"))
                                {
                                    if (rowdtRepTipoCampaña["FechaInt"].ToString() == rowPpto["FechaInt"].ToString())
                                    {
                                        rowdtRepTipoCampaña["Presupuesto"] = Convert.ToDouble(rowPpto["pptoDiario"]);
                                    }
                                }
                            }
                        }

                        isCombChart = true;

                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGraficoMovil" + lsExt);
                        break;
                    case "TCDA":
                        DateTime dTAcumulado = new DateTime(anioQSExp, mesQSExp, 1);
                        BuscarFormatosFechas(1, dTAcumulado);
                        dtRepTipoCampania = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt);
                        isCombChart = true;

                        string[] camposFechaAcumulado = fechaIniRep.Split('-');

                        dtRepTipoCampania = FormatoAcumuladoDT(dtRepTipoCampania);

                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGraficoMovil" + lsExt);
                        break;
                    case "TCEA":

                        BuscarFormatosFechas(6, DateTime.Now);
                        //dtRepTipoCampania = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1Exp, 0, formatoFecha, formatoFechaInt);

                        dtRepTipoCampania = RepTipoCampañaFormatoDatatableConsumo(BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep1Exp, 0, formatoFecha, formatoFechaInt));
                        dtRepConsumoEnlacesMensual = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtRepTipoCampania);
                        dtRepAcumulado = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtRepConsumoEnlacesMensual, dtRepTipoCampania, dtRepTipoCampania.Clone());

                        dtRepTipoCampania = dtRepAcumulado;
                        dtRepConsumoEnlacesMensual = new DataTable();
                        dtRepAcumulado = new DataTable();



                        isCombChart = false;

                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGraficoMovil" + lsExt);
                        break;

                    case "TCEAD":

                        DateTime dTEAD = new DateTime(anioQSExp, mesQSExp, 1);
                        BuscarFormatosFechas(1, dTEAD);
                        DataTable dtRep = BuscarInfoRepCampania(fechaIniRep, fechaFinRep, tipoCampaniaRep3, campaniaN2, formatoFecha, formatoFechaInt);

                        string[] camposFechaEAD = fechaIniRep.Split('-');
                        DateTime dtFechaIniRepEAD = new DateTime(Convert.ToInt32(camposFechaEAD[0]), Convert.ToInt32(camposFechaEAD[1]), 1);

                        DataTable dtRepConsumoEnlacesMensualEAD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtRep);
                        DataView dtvRepConsumoEnlacesMensualEAD = new DataView(dtRepConsumoEnlacesMensualEAD);

                        int i = 0;

                        DataTable dtCloneEnlacesEAD = dtRepConsumoEnlacesMensualEAD.Clone();
                        DataRow rowEnlace = dtRepConsumoEnlacesMensualEAD.Rows[0];

                        foreach (DataColumn column in dtRepConsumoEnlacesMensualEAD.Columns)
                        {
                            if (
                                column.ColumnName.ToLower() != "fecha" &&
                                column.ColumnName.ToLower() != "fechaint" &&
                                column.ColumnName.ToLower() != "link"
                               )
                            {
                                rowEnlace[column] = Convert.ToDouble(rowEnlace[column]) / dtRep.Rows.Count;
                            }
                        }

                        foreach (DataRow row in dtRep.Rows)
                        {
                            string fechaintEnlaceAED = "";
                            string fechaEnlace = "";

                            fechaintEnlaceAED = row["FechaInt"].ToString();
                            fechaEnlace = row["Fecha"].ToString();

                            dtCloneEnlacesEAD.ImportRow(rowEnlace);
                            dtCloneEnlacesEAD.Rows[i]["FechaInt"] = fechaintEnlaceAED;
                            dtCloneEnlacesEAD.Rows[i]["Fecha"] = fechaintEnlaceAED;


                            i++;
                        }

                        DataTable dtRepAcumuladoEAD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtCloneEnlacesEAD, dtRep, dtRep.Clone());

                        dtRep = dtRepAcumuladoEAD;

                        //haer reporte acumulado 
                        DataTable dtRepAcumuladoDiarioEAD = FormatoAcumuladoDT(dtRep);
                        if (dtRepAcumuladoDiarioEAD.Columns.Contains("Presupuesto"))
                        {
                            dtRepAcumuladoDiarioEAD.Columns.Remove("Presupuesto");
                        }

                        #region reportes Acumulado diario
                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte2TablasY2GraficasPentafon" + lsExt);

                        dtRepTipoCampania = dtRep;

                        dtRepAcumulado = dtRepAcumuladoDiarioEAD;
                        dtRepConsumoEnlacesMensual = dtRepConsumoEnlacesMensualEAD;
                        #endregion
                        break;


                    case "TCRTIO":
                        #region Busca Info Totales
                        BuscarFormatosFechas(6, DateTime.Now);

                        DataTable dtTasadoIn = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "In", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtTasadoOut = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "Out", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtEnlaces = BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep);
                        DataTable dtTotalesInOut = CalcularDTReporteTotales(dtTasadoIn, dtTasadoOut, dtEnlaces);

                        #endregion

                        dtRep = dtTotalesInOut;



                        dtRepTipoCampania = dtRep;
                        isCombChart = false;

                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGraficoMovil" + lsExt);

                        break;

                    case "TCRTIOD":
                        DateTime dTRTIOD = new DateTime(anioQSExp, mesQSExp, 1);
                        BuscarFormatosFechas(1, dTRTIOD);

                        DataTable dtTasadoInD = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "In", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtTasadoOutD = BuscarInfoTasacion(fechaIniRep, fechaFinRep, "Out", 0, formatoFecha, formatoFechaInt, "", "", "");
                        DataTable dtEnlacesD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijos(BuscarInfoEnlacesFijos(fechaIniRep, fechaFinRep), dtTasadoInD);

                        #region duplica cada registro en el datatable de Enlaces por cada uno de los registros de dtenlace
                        int j = 0;

                        DataTable dtCloneEnlacesIOD = dtEnlacesD.Clone();
                        DataRow rowEnlaceIO = dtEnlacesD.Rows[0];

                        foreach (DataColumn column in dtCloneEnlacesIOD.Columns)
                        {
                            if (
                                column.ColumnName.ToLower() != "fecha" &&
                                column.ColumnName.ToLower() != "fechaint" &&
                                column.ColumnName.ToLower() != "link"
                               )
                            {
                                rowEnlaceIO[column.ColumnName] = Convert.ToDouble(rowEnlaceIO[column.ColumnName]) / dtTasadoInD.Rows.Count;
                            }
                        }

                        foreach (DataRow row in dtTasadoInD.Rows)
                        {
                            string fechaintEnlaceIO = "";
                            string fechaEnlace = "";

                            fechaintEnlaceIO = row["FechaInt"].ToString();
                            fechaEnlace = row["Fecha"].ToString();

                            dtCloneEnlacesIOD.ImportRow(rowEnlaceIO);
                            dtCloneEnlacesIOD.Rows[j]["FechaInt"] = fechaintEnlaceIO;
                            dtCloneEnlacesIOD.Rows[j]["Fecha"] = fechaEnlace;


                            j++;
                        }

                        #endregion

                        DataTable dtTotalesInOutD = CalcularDTReporteTotales(dtTasadoInD, dtTasadoOutD, dtCloneEnlacesIOD);
                        //DataTable dtRepAcumuladoIOD = RepTipoCampañaFormatoDatatableConsumoEnlacesFijosAcumulado(dtCloneEnlacesIOD, dtTotalesInOutD, dtTotalesInOutD.Clone());
                        List<string> listaColumnasATomarEnCuenta = new List<string>();
                        listaColumnasATomarEnCuenta = FCAndControls.extraeNombreColumnas(dtTotalesInOutD)
                                                                                                        .AsEnumerable()
                                                                                                         .Where(o =>
                                                                                                                    !(o.ToString() == "Fecha") &&
                                                                                                                    !(o.ToString() == "FechaInt") &&
                                                                                                                    !(o.ToString() == "link")
                                                                                                                   ).ToList();

                        DataTable dtRepAcumuladoIOD = ConvierteDTEnFormtoAcumulado(dtTotalesInOutD, listaColumnasATomarEnCuenta);

                        lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte2TablasY2GraficasPentafon" + lsExt);

                        dtRepTipoCampania = dtTotalesInOutD;
                        dtRepAcumulado = dtRepAcumuladoIOD;
                        dtRepConsumoEnlacesMensual = dtEnlacesD;


                        break;

                    default:

                        break;

                }


                #region Elimina columnas
                /*Elimina las columnas que no son necesarias en la exportacion */
                /////////////////

                //dtRepConsumoEnlacesMensual

                if (dtRepConsumoEnlacesMensual.Columns.Contains("mes"))
                {
                    dtRepConsumoEnlacesMensual.Columns.Remove("mes");
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("año"))
                {
                    dtRepConsumoEnlacesMensual.Columns.Remove("año");
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("link"))
                {
                    dtRepConsumoEnlacesMensual.Columns.Remove("link");
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("id"))
                {
                    dtRepConsumoEnlacesMensual.Columns.Remove("id");
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("fechaInt"))
                {
                    dtRepConsumoEnlacesMensual.Columns.Remove("fechaInt");
                }
                if (dtRepConsumoEnlacesMensual.Columns.Contains("FechaInt"))
                {
                    dtRepConsumoEnlacesMensual.Columns.Remove("FechaInt");
                }

                //dtRepAcumulado

                if (dtRepAcumulado.Columns.Contains("mes"))
                {
                    dtRepAcumulado.Columns.Remove("mes");
                }
                if (dtRepAcumulado.Columns.Contains("año"))
                {
                    dtRepAcumulado.Columns.Remove("año");
                }
                if (dtRepAcumulado.Columns.Contains("link"))
                {
                    dtRepAcumulado.Columns.Remove("link");
                }
                if (dtRepAcumulado.Columns.Contains("id"))
                {
                    dtRepAcumulado.Columns.Remove("id");
                }
                if (dtRepAcumulado.Columns.Contains("fechaInt"))
                {
                    dtRepAcumulado.Columns.Remove("fechaInt");
                }
                if (dtRepAcumulado.Columns.Contains("FechaInt"))
                {
                    dtRepAcumulado.Columns.Remove("FechaInt");
                }


                //dtRepTipoCampania

                if (dtRepTipoCampania.Columns.Contains("mes"))
                {
                    dtRepTipoCampania.Columns.Remove("mes");
                }
                if (dtRepTipoCampania.Columns.Contains("año"))
                {
                    dtRepTipoCampania.Columns.Remove("año");
                }
                if (dtRepTipoCampania.Columns.Contains("link"))
                {
                    dtRepTipoCampania.Columns.Remove("link");
                }
                if (dtRepTipoCampania.Columns.Contains("id"))
                {
                    dtRepTipoCampania.Columns.Remove("id");
                }
                if (dtRepTipoCampania.Columns.Contains("fechaInt"))
                {
                    dtRepTipoCampania.Columns.Remove("fechaInt");
                }
                if (dtRepTipoCampania.Columns.Contains("FechaInt"))
                {
                    dtRepTipoCampania.Columns.Remove("FechaInt");
                }

                /////////////////
                #endregion

                List<string> listColcarrdtRepConsumoEnlacesMensual = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesMensual).ToList().Where(r => (!r.ToLower().Contains("fecha"))).ToList();
                List<string> listaColcarrdtRepConsumoEnlacesMennsualAcum = FCAndControls.extraeNombreColumnas(dtRepAcumulado).ToList().Where(r => (!r.ToLower().Contains("fecha"))).ToList();
                //if (listColcarrdtRepConsumoEnlacesMensual.Count == 0 || listaColcarrdtRepConsumoEnlacesMennsualAcum.Count == 0)
                //{
                //    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGraficoMovil" + lsExt);
                //}
                //else
                //{
                //    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte3TablasYGraficoPentafon" + lsExt);
                //}

                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ProcesarTituloExcel(lExcel, "Por tipo de campaña");



                foreach (DataColumn column in dtRepTipoCampania.Columns)
                {
                    if (column.ColumnName.ToString().ToLower() != "fecha" && column.ColumnName.ToString().ToLower() != "fechaint")
                    {
                        listaSeries.Add(column.ColumnName);
                    }
                }


                if (dtRepTipoCampania.Rows.Count > 0 && dtRepTipoCampania.Columns.Count > 0)
                {
                    //xlLineStacked

                    creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepTipoCampania,
                            0, "Totales"), "Reporte", "Tabla");


                    if (dtRepTipoCampania.Columns.Contains("Total"))
                    {
                        dtRepTipoCampania.Columns.Remove("Total");

                    }
                    if (listaSeries.Contains("Total"))
                    {
                        listaSeries.Remove("Total");
                    }

                    DataRow rowTot = dtRepTipoCampania.NewRow();
                    rowTot = dtRepTipoCampania.AsEnumerable().LastOrDefault();

                    if (dtRepTipoCampania.Rows[dtRepTipoCampania.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                    {
                        dtRepTipoCampania.Rows[dtRepTipoCampania.Rows.Count - 1].Delete();
                    }
                    dtRepTipoCampania.AcceptChanges();

                    crearGrafico(dtRepTipoCampania, "", listaSeries.ToArray(), listaSeries.ToArray(),
                                      listaSeries.ToArray(), "Fecha", "", "", "Total", "$#,0.00",
                                      true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked, lExcel, "{Grafica}", "Reporte", "DatosGr1", 500, 300, isCombChart);
                }




                List<string> listaCarriersEnlaces = new List<string>();
                listaCarriersEnlaces = FCAndControls.extraeNombreColumnas(dtRepConsumoEnlacesMensual).ToList()
                                                    .Where(r =>
                                                            !(r.ToLower().Contains("fecha")) &&
                                                            !(r.ToLower().Contains("link")) &&
                                                            !(r.ToLower().Contains("id")) &&
                                                            !(r.ToLower().Contains("mes")) &&
                                                            !(r.ToLower().Contains("año"))
                                                           )
                                                    .Select(r => r)
                                                    .Distinct().ToList();


                List<string> listaCarriersEnlacesAcumulado = new List<string>();
                listaCarriersEnlacesAcumulado = FCAndControls.extraeNombreColumnas(dtRepAcumulado).ToList()
                                                    .Where(r =>
                                                            !(r.ToLower().Contains("fecha")) &&
                                                            !(r.ToLower().Contains("link")) &&
                                                            !(r.ToLower().Contains("id")) &&
                                                            !(r.ToLower().Contains("mes")) &&
                                                            !(r.ToLower().Contains("año"))
                                                           )
                                                    .Select(r => r)
                                                    .Distinct().ToList();





                if (
                       listaCarriersEnlaces.Count > 0 && listaCarriersEnlacesAcumulado.Count > 0 &&
                       dtRepConsumoEnlacesMensual.Rows.Count > 0 && dtRepAcumulado.Rows.Count > 0
                    )
                {
                    if (dtRepConsumoEnlacesMensual.Columns.Contains("bestelajuste"))
                    {
                        dtRepConsumoEnlacesMensual.Columns.Remove("bestelajuste");
                    }

                    if (dtRepConsumoEnlacesMensual.Columns.Contains("total"))
                    {
                        dtRepConsumoEnlacesMensual.Columns.Remove("total");
                    }



                    if (codRepExp == "TC" || codRepExp == "TCC")
                    {
                        creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepConsumoEnlacesMensual,
                            0, "Totales"), "Reporte", "Tabla2");

                        creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAcumulado,
                                    0, "Totales"), "Reporte", "Tabla3");
                    }
                    else
                    {
                        creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAcumulado,
                        0, "Totales"), "Reporte", "Tabla2");
                    }



                    if (dtRepAcumulado.Columns.Contains("Total"))
                    {
                        dtRepAcumulado.Columns.Remove("Total");

                    }
                    if (listaCarriersEnlacesAcumulado.Contains("Total"))
                    {
                        listaCarriersEnlacesAcumulado.Remove("Total");
                    }

                    DataRow rowTot = dtRepAcumulado.NewRow();
                    rowTot = dtRepAcumulado.AsEnumerable().LastOrDefault();

                    if (dtRepAcumulado.Rows[dtRepAcumulado.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                    {
                        dtRepAcumulado.Rows[dtRepAcumulado.Rows.Count - 1].Delete();
                    }
                    dtRepAcumulado.AcceptChanges();

                    if (codRepExp == "TC" || codRepExp == "TCEAD" || codRepExp == "TCRTIOD")
                    {
                        crearGrafico(dtRepAcumulado, "", listaCarriersEnlacesAcumulado.ToArray(), listaCarriersEnlacesAcumulado.ToArray(),
                                  listaCarriersEnlacesAcumulado.ToArray(), "Fecha", "", "", "Total", "$#,0.00",
                                  true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked, lExcel, "{Grafica2}", "Reporte", "DatosGr2", 500, 300, isCombChart);
                    }



                }




                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;


                lExcel.SalvarComo();


                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes tipo campaña");



            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (lExcel != null)
                {
                    lExcel.Cerrar(true);
                    lExcel.Dispose();

                }
            }
        }

        private static void creaTablaEnExcel(ExcelAccess lExcel, DataTable ldt, string hoja, string textoBusqueda)
        {
            object[,] datos = lExcel.DataTableToArray(FCAndControls.daFormatoACeldas(ldt), true);

            EstiloTablaExcel estilo = new EstiloTablaExcel();
            estilo.Estilo = "KeytiaGrid";
            estilo.FilaEncabezado = true;
            estilo.FilasBandas = true;
            estilo.FilaTotales = false;
            estilo.PrimeraColumna = false;
            estilo.UltimaColumna = true;
            estilo.ColumnasBandas = false;
            estilo.AutoFiltro = false;
            estilo.AutoAjustarColumnas = true;

            lExcel.Actualizar(hoja, textoBusqueda, false, datos, estilo);
        }

        protected void ProcesarTituloExcel(ExcelAccess pExcel, string titulo, string titulo2 = "Consumo por Enlace", string titulo3 = "Consumo por Enlace Acumulado")
        {
            Hashtable lhtMeta = BuscarTituloExcel(pExcel);
            string lsImg;
            int liRenLogoCliente = -1;
            int liRenLogoKeytia = -1;
            int liRenTitulo;
            int liRenglon;
            Hashtable lHTDatosImg = null;
            string lsTopLeft = null;
            string lsBottomLeft = null;
            float lfImgOffset = 0;

            string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
            string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());

            //NZ 20150707 Se cambio el campo con el que se compara el esquema en la tabla de Clientes. En ves de vchcodigo se comprara con Esquema.
            DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] " +
                                                " where Esquema = '" + DSODataContext.Schema + "'" +
                                                " and dtinivigencia <> dtfinVigencia " +
                                                " and dtfinVigencia>getdate()");

            //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
            if (pRowCliente != null && pRowCliente["LogoExportacion"] != DBNull.Value && lhtMeta["{LogoCliente}"] != null)
            {
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                lsImg = pRowCliente["LogoExportacion"].ToString().Replace("~", "").Replace("/", "\\");
                if (lsImg.StartsWith("\\"))
                {
                    lsImg = lsImg.Substring(1);
                }
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, lsImg);
                if (System.IO.File.Exists(lsImg))
                {
                    lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"], lsImg, false, false, 0, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lfImgOffset = (float)lHTDatosImg["Ancho"];
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoCliente = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
            }

            lsImg = System.IO.Path.Combine(lsStylePath, @"images\KeytiaHeader.png");
            if (System.IO.File.Exists(lsImg) && lhtMeta["{LogoKeytia}"] != null)
            {
                lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"], lsImg, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                if ((bool)lHTDatosImg["Inserto"])
                {
                    lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                    lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                    liRenLogoKeytia = int.Parse(lsBottomLeft.Split(',')[0]);
                }
            }

            if (lhtMeta["{LogoCliente}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"]).set_Value(System.Type.Missing, String.Empty);

            if (lhtMeta["{LogoKeytia}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"]).set_Value(System.Type.Missing, String.Empty);


            if (lhtMeta["{TituloReporte}"] != null)
            {
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).set_Value(System.Type.Missing, titulo);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }
            if (lhtMeta["{FechasReporte}"] != null || lhtMeta["{FechasReporteMovil}"] != null)
            {
                int piRenFechas;
                int piColFechas;

                /* BG.20180307 */
                int mesInicio = 0;
                int anioInicio = 0;

                if (!string.IsNullOrEmpty(Request.QueryString["fecha"]))
                {
                    string[] camposFecha = Request.QueryString["fecha"].ToString().Split('/');


                    //RM 20180623 Se crea campos de mes y año
                    if (camposFecha.Count() == 2)
                    {
                        mesInicio = Convert.ToInt32(camposFecha[0].ToString());
                        anioInicio = Convert.ToInt32(camposFecha[1].ToString());
                    }
                    else
                    {
                        mesInicio = Convert.ToInt32(DateTime.Now.ToString("MM"));
                        anioInicio = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                    }
                }
                else
                {
                    mesInicio = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    anioInicio = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                }


                string tag = "{FechasReporte}";
                if(lhtMeta["{FechasReporte}"] != null)
                {
                    tag = "{FechasReporte}";
                }
                else if (lhtMeta["{FechasReporteMovil}"] != null)
                {
                    tag = "{FechasReporteMovil}";
                }

                pExcel.BuscarTexto("Reporte", tag, true, out piRenFechas, out piColFechas);
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas, "Período:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + MonthName(mesInicio) + " " + anioInicio);

                //pExcel.BuscarTexto("Reporte", "{FechasReporte}", true, out piRenFechas, out piColFechas);
                //pExcel.Actualizar("Reporte", piRenFechas, piColFechas, "Inicio:");
                //pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + pdtInicio.Date.ToString("dd/MM/yyyy"));
                //pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 2, "Fin:");
                //pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 3, "'" + pdtFin.Date.ToString("dd/MM/yyyy"));
            }

            if (lhtMeta["{TituloReporte2}"] != null)
            {
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte2}"]).set_Value(System.Type.Missing, titulo2);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte2}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }

            if (lhtMeta["{TituloReporte3}"] != null)
            {
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte3}"]).set_Value(System.Type.Missing, titulo3);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte3}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }


        }


        protected Hashtable BuscarTituloExcel(ExcelAccess pExcel)
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto("Reporte", "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto("Reporte", "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto("Reporte", "{TituloReporte}", true));
            lhtRet.Add("{FechasReporte}", pExcel.BuscarTexto("Reporte", "{FechasReporte}", true));
            lhtRet.Add("{FechasReporteMovil}", pExcel.BuscarTexto("Reporte", "{FechasReporteMovil}", true));
            lhtRet.Add("{TituloReporte2}", pExcel.BuscarTexto("Reporte", "{TituloReporte2}", true));
            lhtRet.Add("{TituloReporte3}", pExcel.BuscarTexto("Reporte", "{TituloReporte3}", true));

            return lhtRet;
        }

        private string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper();
        }

        protected void crearGrafico(DataTable ldt, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                          string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda,
                                          Microsoft.Office.Interop.Excel.XlChartType tipoGraf, ExcelAccess lExcel, string textoPlantilla, string HojaGrafico,
                                          string HojaDatos, float anchoGraf, float alturaGraf,bool isCombChart = false)
        {
            ParametrosGrafica lparametrosGraf = parametrosDeGrafica(ldt, tituloGraf, columnaDatos, leyenda, serieId, EjeX, tituloEjeX, formatoEjeX, tituloEjeY,
                                                             formatoEjeY, mostrarLeyenda);

            ProcesarGraficaExcel(HojaGrafico, HojaDatos, anchoGraf, alturaGraf, 20, 0, lparametrosGraf, lExcel, textoPlantilla, tipoGraf, isCombChart);
        }

        protected ParametrosGrafica parametrosDeGrafica(DataTable lsDataSource, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                                                          string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda)
        {

            ParametrosGrafica lParametrosGrafica = new ParametrosGrafica();

            lParametrosGrafica.Datos = lsDataSource;
            lParametrosGrafica.Title = tituloGraf;
            lParametrosGrafica.DataColumns = columnaDatos;
            lParametrosGrafica.SeriesNames = leyenda;
            lParametrosGrafica.SeriesIds = serieId;
            lParametrosGrafica.XColumn = EjeX;
            lParametrosGrafica.XIdsColumn = EjeX;
            lParametrosGrafica.XTitle = tituloEjeX;
            lParametrosGrafica.XFormat = formatoEjeX;
            lParametrosGrafica.YTitle = tituloEjeY;
            lParametrosGrafica.YFormat = formatoEjeY;
            lParametrosGrafica.ShowLegend = mostrarLeyenda;
            //lParametrosGrafica.TipoGrafica = tipoGraf;

            return lParametrosGrafica;
        }

        protected Hashtable ProcesarGraficaExcel(string lsHojaGrafico, string lsHojaDatos, float lfWidth, float lfHeight, float lfOffsetX, float lfOffsetY,
                                                               ParametrosGrafica lParametrosGrafica, ExcelAccess lExcel, string cambiarTextoPorGraf, Microsoft.Office.Interop.Excel.XlChartType tipoGrafica, bool isCombChart = false)
        {
            FormatoGrafica lFormatoGrafica = new FormatoGrafica();
            lFormatoGrafica.Titulo = lParametrosGrafica.Title;
            lFormatoGrafica.Leyendas = lParametrosGrafica.ShowLegend;
            lFormatoGrafica.XFormat = lParametrosGrafica.XFormat;
            lFormatoGrafica.YFormat = lParametrosGrafica.YFormat;
            Microsoft.Office.Interop.Excel.XlChartType lCharType = tipoGrafica;//GetTipoGraficaExcel(lParametrosGrafica.TipoGrafica);

            return lExcel.InsertarGrafico(lsHojaGrafico, lsHojaDatos, cambiarTextoPorGraf, lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lParametrosGrafica.DataColumns,
                     lParametrosGrafica.SeriesNames, lParametrosGrafica.Datos, lCharType, lfWidth, lfHeight, lfOffsetX, lfOffsetY, lFormatoGrafica,isCombChart);
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        protected void ddlTiposCampaniaRep1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string val = "";
        }

        private RoutingSessionHelper CrearRoutingObject(string TipoReporteSeccion3, string fecha)
        {
            RoutingSessionHelper objRoutingSessionHelper = new RoutingSessionHelper();

            DropDownList ddlS1FiltoTipoCampaña = ddlTiposCampaniaRep1;
            DropDownList ddlS2FiltoTipoCampaña = ddlTiposCampaniaRep2;
            DropDownList ddlS2FiltoCampaña = ddlcampanias;


            //string S1FiltroTipoCampaña    = string.Empty;  
            //string S1Link                 = string.Empty;
            //string S1LinkLupa             = string.Empty;

            //string S2FiltroTipoCampaña    = string.Empty;
            //int    S2FiltroCampaña        = 0;
            //string S2FiltroCampañaNombre  = string.Empty;
            //string S2Link                 = string.Empty;
            //string S2LinkLupa             = string.Empty;

            //string S3FiltroTipoCampaña    = string.Empty;
            //int    S3FiltroCampaña        = 0;
            //string S3FiltroCampañaNombre  = string.Empty;
            //string S3Link                 = string.Empty;
            //string S3LinkLupa             = string.Empty;

            //string S4FiltroTipoCampaña    = string.Empty;
            //int    S4FiltroCampaña        = 0;
            //string S4FiltroCampañaNombre  = string.Empty;
            //string S4Link                 = string.Empty;
            //string S4LinkLupa             = string.Empty;

            if (ddlS1FiltoTipoCampaña != null && ddlS2FiltoTipoCampaña != null && ddlS2FiltoCampaña != null)
            {
                int campaña = 0;

                objRoutingSessionHelper.S1FiltroTipoCampaña = ddlS1FiltoTipoCampaña.SelectedItem.Text.ToString();

                objRoutingSessionHelper.S2FiltroTipoCampaña = ddlS2FiltoTipoCampaña.SelectedItem.Text.ToString();
                int.TryParse(ddlS2FiltoTipoCampaña.SelectedItem.Value, out campaña);
                objRoutingSessionHelper.S2FiltroCampaña = campaña > 0 ? campaña : 0;
                objRoutingSessionHelper.S2FiltroCampañaNombre = ddlS2FiltoCampaña.SelectedItem.Text.ToString();

            }

            return objRoutingSessionHelper;
        }

        public bool CargarFiltrosSessionObj(RoutingSessionHelper objRoutingSessionHelper)
        {
            bool respuesta = false;
            try
            {
                if (objRoutingSessionHelper.S1FiltroTipoCampaña.Length > 0)
                {
                    ddlTiposCampaniaRep1.SelectedIndex = ddlTiposCampaniaRep1.Items.IndexOf(ddlTiposCampaniaRep1.Items.FindByText(objRoutingSessionHelper.S1FiltroTipoCampaña));
                }

                if (objRoutingSessionHelper.S2FiltroTipoCampaña.Length > 0)
                {
                    ddlTiposCampaniaRep2.SelectedIndex = ddlTiposCampaniaRep2.Items.IndexOf(ddlTiposCampaniaRep2.Items.FindByText(objRoutingSessionHelper.S2FiltroTipoCampaña));
                }

                if (Convert.ToInt32(objRoutingSessionHelper.S2FiltroCampaña) > 0)
                {
                    ddlcampanias.SelectedIndex = ddlcampanias.Items.IndexOf(ddlcampanias.Items.FindByValue(objRoutingSessionHelper.S2FiltroCampaña.ToString()));
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return respuesta;
        }


    }


    public class RoutingSessionHelper
    {
        public string S1FiltroTipoCampaña { get; set; }
        public string S1Link { set; get; }
        public string S1LinkLupa { set; get; }

        public string S2FiltroTipoCampaña { get; set; }
        public int S2FiltroCampaña { get; set; }
        public string S2FiltroCampañaNombre { set; get; }
        public string S2Link { set; get; }
        public string S2LinkLupa { set; get; }

        public string S3FiltroTipoCampaña { get; set; }
        public int S3FiltroCampaña { get; set; }
        public string S3FiltroCampañaNombre { set; get; }
        public string S3Link { set; get; }
        public string S3LinkLupa { set; get; }

        public string S4FiltroTipoCampaña { get; set; }
        public int S4FiltroCampaña { get; set; }
        public string S4FiltroCampañaNombre { set; get; }
        public string S4Link { set; get; }
        public string S4LinkLupa { set; get; }

    }
}