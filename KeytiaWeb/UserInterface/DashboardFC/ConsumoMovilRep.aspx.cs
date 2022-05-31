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
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class ConsumoMovilRep : System.Web.UI.Page
    {
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        StringBuilder query = new StringBuilder();
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        static string kpiFilter = string.Empty;
        Parametros param;

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Al entrar por indicador en dashboard moviles se agrega parametro redirect
            // Al entrar por el menu lineas por plan base se agrega el parametro Plan
            if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("redirect") || Request.QueryString["Plan"] != null)
            {
                pnlLeft.Visible = false;
                pnlRight.Style.Add("width", "100%");
            }

            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                if (!Page.IsPostBack || !ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
                {
                    #region Inicia los valores default de los controles de fecha y banderas de clientes y perfiles
                    try
                    {
                        if (Session["Language"].ToString() == "Español")
                        {
                            pdtInicio.setRegion("es");
                            pdtFin.setRegion("es");
                        }

                        pdtInicio.MaxDateTime = DateTime.Today;
                        pdtInicio.MinDateTime = new DateTime((DateTime.Now.Year - 4), 1, 1);
                        CalculaFechasDeDashboard();

                        iCodUsuario = Session["iCodUsuario"].ToString();
                        iCodPerfil = Session["iCodPerfil"].ToString();
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                    #endregion Inicia los valores default de los controles de fecha

                    LlenarDropDownList();

                    //Parametros Iniciales
                    param = new Parametros();
                    param.ClaveNivelReporte = "ConsumoMovil";
                    param.TituloNav = "Consumo por Línea";
                    param.PeriodoInfo = Convert.ToDateTime(Session["FechaInicio"]).ToString("yyyy-MM-dd") + " 00:00:00";

                    // Al entrar por indicador en dashboard moviles se agrega parametro kpiFilter
                    // De igual forma se asegura de que la string kpiFilter este vacia en caso de que no se contenga en el link.
                    if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("kpiFilter"))
                    {
                        kpiFilter = Request.QueryString["kpiFilter"].ToString();
                    }
                    else if (!String.IsNullOrEmpty(kpiFilter))
                    {
                        kpiFilter = string.Empty;
                    }

                    DashboardPrincipal();
                }
                else
                {
                    if (Request["__EVENTARGUMENT"] != null)
                    {
                        param = (new System.Web.Script.Serialization.JavaScriptSerializer()).Deserialize<Parametros>(Request["__EVENTARGUMENT"]);
                    }
                    if (param == null)
                    {
                        List<Parametros> lista = new List<Parametros>();
                        if (Session["Navegacion"] != null) //Entonces ya tiene navegacion almacenada
                        {
                            lista = (List<Parametros>)Session["Navegacion"];
                            param = lista[lista.Count - 1];
                        }
                        else
                        {
                            param = new Parametros();
                            param.ClaveNivelReporte = "ConsumoMovil";
                            param.TituloNav = "Consumo por Línea";
                        }
                    }

                    if (string.IsNullOrEmpty(param.PeriodoInfo) || Convert.ToDateTime(param.PeriodoInfo) == DateTime.MinValue)
                    {
                        param.PeriodoInfo = Convert.ToDateTime(Session["FechaInicio"]).ToString("yyyy-MM-dd") + " 00:00:00";
                    }

                    fechaInicio = Convert.ToDateTime(param.PeriodoInfo);
                    Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                    Session["FechaFin"] = fechaInicio.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

                    DashboardPrincipal();
                }

                GetNavegacion(); //Una vez que ya estan llenos los parametos.
            }
            catch (Exception ex)
            {
                if (Session["Navegacion"] != null) //Entonces ya tiene navegacion almacenada
                {
                    List<Parametros> lista = new List<Parametros>();
                    lista = (List<Parametros>)Session["Navegacion"];
                    param = lista[lista.Count - 1];
                }

                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        #region Configuración

        private void GetNavegacion()
        {
            List<Parametros> lista = new List<Parametros>();
            bool navDesdeMenu = false;  // Si proviene desde menu o indicador 

            if (Session["Navegacion"] != null) //Entonces ya tiene navegacion almacenada
            {
                lista = (List<Parametros>)Session["Navegacion"];

                if (lista.Exists(x => x.ClaveNivelReporte == param.ClaveNivelReporte))
                {
                    int index = lista.LastIndexOf(lista.First(x => x.ClaveNivelReporte == param.ClaveNivelReporte));
                    if (index == lista.Count - 1)
                    {
                        lista.RemoveAt(index);
                        lista.Add(param);
                    }
                    else { lista.RemoveRange(index + 1, (lista.Count - 1) - index); }
                }
                else { lista.Add(param); }
            }
            else
            {
                param = new Parametros();
                param.ClaveNivelReporte = "ConsumoMovil";
                param.TituloNav = "Consumo por Línea";

                lista.Add(param);
            }

            //Arma Navegacion en Panatalla
            pnlNavegacion.Controls.Clear();
            foreach (var item in lista)
            {
                // En caso de provenir desde el menu por plan base se agrega esta navegación al inicio
                if (Request.QueryString["Plan"] != null && navDesdeMenu != true)
                {
                    HyperLink linkMenu = new HyperLink();
                    linkMenu.Text = "Inicio";
                    linkMenu.CssClass = "navegacionStyle";
                    linkMenu.NavigateUrl = "DashboardMoviles.aspx?Opc=OpcDashFCMoviles&Nav=PlanBaseLinea";
                    pnlNavegacion.Controls.Add(linkMenu);
                    navDesdeMenu = true;
                }

                if (pnlNavegacion.Controls.Count > 0)
                {
                    Label l = new Label();
                    l.Text = "&nbsp;&nbsp;>&nbsp;&nbsp;";
                    l.CssClass = "navegacionStyle";
                    pnlNavegacion.Controls.Add(l);
                }

                HyperLink hl = new HyperLink();
                hl.Text = item.TituloNav;
                hl.CssClass = "navegacionStyle";
                hl.NavigateUrl = string.Format("javascript:Navegar('{0}','{1}','{2}','{3}','{4}');",
                     item.TituloNav, item.ClaveNivelReporte,
                     !string.IsNullOrEmpty(item.ICodCatLinea) ? item.ICodCatLinea : "0",
                     !string.IsNullOrEmpty(item.ICodCatCarrier) ? item.ICodCatCarrier : "0",
                     !string.IsNullOrEmpty(item.Categoria) ? item.Categoria : "0");

                pnlNavegacion.Controls.Add(hl);
            }

            Session["Navegacion"] = lista;
        }

        private void LlenarDropDownList()
        {
            DataTable carrier = DSODataAccess.Execute(ConsultaObtenerCarrierFacturas());
            carrier = DTIChartsAndControls.ordenaTabla(carrier, "Carrier ASC");
            DataRow rowCarrier = carrier.NewRow();
            rowCarrier["iCodCatalogo"] = 0;
            rowCarrier["Carrier"] = "   - TODOS -   ";

            carrier.Rows.InsertAt(rowCarrier, 0);
            cboCarrier.DataSource = carrier.DefaultView;
            cboCarrier.DataValueField = "iCodCatalogo";
            cboCarrier.DataTextField = "Carrier";
            cboCarrier.DataBind();
        }

        private void CalculaFechasDeDashboard()
        {
            #region Calcula Fechas

            DataTable fechaMaxFact = DSODataAccess.Execute(ConsultaFechaMaximaFactura());
            // Se valida que el datatable contenga resultados.
            if (fechaMaxFact.Rows.Count > 0)
            {
                fechaInicio = Convert.ToDateTime(fechaMaxFact.Rows[0][0]);
            }
            else { fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); }

            pdtInicio.CreateControls();
            pdtInicio.DataValue = (object)fechaInicio;

            // Se agrega esta condicional para evitar el cambio de la variable de sesion por el control de busqueda.
            // Cuando el reporte es el segundo nivel de otro reporte desde DashBoard moviles es necesario conservar 
            // el valor original.
            if(!(Request.QueryString["Plan"] != null))
            {
                Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                Session["FechaFin"] = fechaInicio.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            }
            #endregion Calcula Fechas
        }

        public void cambiaURLFunctionJava_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if it is not DataRow return.
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            //Loop thru the cells changing the "." to ":" in hyperlink navigate URLs
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                TableCell td = e.Row.Cells[i];
                if (td.Controls.Count > 0 && td.Controls[0] is HyperLink)
                {
                    HyperLink hyperLink = td.Controls[0] as HyperLink;
                    if (!hyperLink.NavigateUrl.ToLower().Contains("totales"))
                    {
                        string navigateUrl = hyperLink.NavigateUrl.ToLower();
                        hyperLink.NavigateUrl = hyperLink.NavigateUrl.Replace(
                           hyperLink.NavigateUrl.Substring(
                           navigateUrl.IndexOf("javascript."), "javascript.".Length),
                           "javascript:");
                    }
                }
            }
        }

        #endregion

        #region WebMethod

        [WebMethod]
        public static object ConsultaAutoComplateEmple(string texto)
        {
            DataTable Emple = DSODataAccess.Execute(BuscarElemento("", "BusquedaPorEmpleado", texto));
            DataView dvldt = new DataView(Emple);
            Emple = dvldt.ToTable(false, new string[] { "Codigo Empleado", "Nomina", "Nombre Completo" });
            Emple.Columns["Codigo Empleado"].ColumnName = "Id";
            Emple.Columns["Nombre Completo"].ColumnName = "Nombre";
            return FCAndControls.ConvertDataTabletoJSONString(Emple);
        }

        [WebMethod]
        public static object ConsultaAutoComplateCenCos(string texto)
        {
            DataTable CenCos = DSODataAccess.Execute(BuscarElemento("", "BusquedaPorCenCos", texto));
            DataView dvldt = new DataView(CenCos);
            CenCos = dvldt.ToTable(false, new string[] { "Codigo CenCos", "Clave CenCos", "Centro de Costos" });

            CenCos.Columns["Codigo CenCos"].ColumnName = "Id";
            CenCos.Columns["Clave CenCos"].ColumnName = "Clave";
            CenCos.Columns["Centro de Costos"].ColumnName = "Descripcion";
            return FCAndControls.ConvertDataTabletoJSONString(CenCos);
        }

        [WebMethod]
        public static object ConsultaAutoComplateLinea(string texto)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("SELECT Descripcion = vchDescripcion, Id = iCodCatalogo");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')]");
            lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsb.AppendLine("    AND dtFinVigencia >= GETDATE()");
            lsb.AppendLine("    AND Tel LIKE '%" + texto.Replace("'", "") + "%'");

            DataTable linea = DSODataAccess.Execute(lsb.ToString());
            return FCAndControls.ConvertDataTabletoJSONString(linea);
        }

        [WebMethod]
        public static object ConsultaAutoComplatePlan(string texto)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("SELECT [Descripcion] = vchDescripcion, Id = iCodCatalogo");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('PlanTarif','Plan Tarifario','Español')] [PlanT]");
            lsb.AppendLine("");
            lsb.AppendLine("	JOIN (SELECT Carrier");
            lsb.AppendLine("          FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles");
            lsb.AppendLine("          GROUP BY Carrier");
            lsb.AppendLine("        ) AS CDR");
            lsb.AppendLine("		ON [PlanT].Carrier = CDR.Carrier");
            lsb.AppendLine("");
            lsb.AppendLine("WHERE [PlanT].dtIniVigencia <> [PlanT].dtFinVigencia");
            lsb.AppendLine("    AND [PlanT].dtFinVigencia >= GETDATE()");
            lsb.AppendLine("    AND vchDescripcion LIKE '%" + texto.Replace("'", "") + "%'");
            lsb.AppendLine(" GROUP BY vchDescripcion, iCodCatalogo");

            DataTable plan = DSODataAccess.Execute(lsb.ToString());
            return FCAndControls.ConvertDataTabletoJSONString(plan);
        }

        #endregion

        #region Consultas

        private static string BuscarElemento(string link, string nombreSP, string texto)
        {
            StringBuilder querySP = new StringBuilder();
            querySP.AppendLine("EXEC " + nombreSP);
            querySP.AppendLine("  	@Schema = '" + DSODataContext.Schema + "',");
            querySP.AppendLine("      @Texto = '''%" + texto.Trim().Replace(" ", "%").Replace("'", "") + "%''',");
            querySP.AppendLine("      @Link = '''''',");
            querySP.AppendLine("      @Usuario = " + iCodUsuario + ",");
            querySP.AppendLine("      @Perfil = " + iCodPerfil);

            return querySP.ToString();
        }

        private string ConsultaObtenerCarrierFacturas()
        {
            query.Length = 0;
            query.AppendLine("SELECT iCodCatalogo, Carrier=UPPER(vchDescripcion)");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Carrier','Carriers','Español')] Carrier");
            query.AppendLine("");
            query.AppendLine("	JOIN (SELECT Carrier");
            query.AppendLine("          FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles");
            query.AppendLine("          GROUP BY Carrier");
            query.AppendLine("        ) AS CDR");
            query.AppendLine("		ON Carrier.iCodCatalogo = CDR.Carrier");
            query.AppendLine("");
            query.AppendLine("WHERE Carrier.dtIniVigencia <> Carrier.dtFinVigencia");
            query.AppendLine("    AND Carrier.dtFinVigencia >= GETDATE()");
            return query.ToString();
        }

        private string ConsultaFechaMaximaFactura()
        {
            query.Length = 0;
            query.AppendLine("SELECT isnull(MAX(FechaPub),GETDATE())");
            query.AppendLine("FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles");
            return query.ToString();
        }

        private string ConsultaRepConsumoMovilLineas()
        {
            StringBuilder consulta = new StringBuilder();
            DateTime periodoInfo = Convert.ToDateTime(param.PeriodoInfo);

            string fieldsStoredProcedure = "";
            string groupStoredProcedure = "";

            // Agregamos cambio para los parametros que tomara el Stored Procedure ya que para Komatsu requerira un campo extra (IVA) 
            if (DSODataContext.Schema.ToUpper() != "ROADMACHINERY")
            {
                fieldsStoredProcedure = "[Codigo Linea],[Linea],[Modelo],[Codigo Carrier],[Carrier],[Nombre Completo] = MIN(UPPER([Nombre Completo]))," +
                                        "[Nombre Centro de Costos] = MIN(UPPER([Nombre Centro de Costos])),[Renta] = ROUND(SUM([Renta]/[TipoCambio]),2)," +
                                        "[Excedente] = ROUND((SUM([Costo]/[TipoCambio])) - SUM([Renta]/[TipoCambio]),2),[Total] = ROUND(SUM([Costo]/[TipoCambio]),2)," +
                                        "[Plan] = MIN(UPPER([Plan])),[Fecha Fin Plan] = MIN(UPPER([Fecha Fin Plan]))";

                groupStoredProcedure = "[Codigo Linea],[Linea],[Modelo],[Codigo Carrier],[Carrier]";
            }
            else
            {
                fieldsStoredProcedure = "[Codigo Linea],[Linea],[Modelo],[Codigo Carrier],[Carrier],[Nombre Completo] = MIN(UPPER([Nombre Completo]))," +
                                        "[Nombre Centro de Costos] = MIN(UPPER([Nombre Centro de Costos])),[Renta] = ROUND(SUM([Renta]/[TipoCambio]),2)," +
                                        "[Excedente] = ROUND((SUM([Costo]/[TipoCambio])) - SUM([Renta]/[TipoCambio]),2),[Total] = ROUND(SUM([Costo]/[TipoCambio]),2)," +
                                        "[IVA], [Plan] = MIN(UPPER([Plan])),[Fecha Fin Plan] = MIN(UPPER([Fecha Fin Plan]))";

                groupStoredProcedure = "[Codigo Linea],[Linea],[Modelo],[Codigo Carrier],[Carrier],[IVA]";
            }

            consulta.AppendLine("DECLARE @Where VARCHAR(MAX)= ''   ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.

            #region Filtro por Linea
            if (!string.IsNullOrEmpty(param.ICodCatLinea) && param.ICodCatLinea != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Linea] = " + param.ICodCatLinea + " ' ");
            }
            #endregion Filtro por Linea

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(param.ICodCatEmple) && param.ICodCatEmple != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Empleado] = " + param.ICodCatEmple + " ' ");
            }
            #endregion Filtro por Empleado

            #region Filtro por CenCos
            if (!string.IsNullOrEmpty(param.ICodCatCenCos) && param.ICodCatCenCos != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Centro de Costos] = " + param.ICodCatCenCos + " ' ");
            }
            #endregion Filtro por CenCos

            #region Filtro por Plan

            // Prueba para filtrar por plan proveniendo desde otro reporte

            if (Request.QueryString["Plan"] != null)
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Plan] = " + Request.QueryString["Plan"] + " ' ");
            }

            if (!string.IsNullOrEmpty(param.ICodCatPlan) && param.ICodCatPlan != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Plan] = " + param.ICodCatPlan + " ' ");
            }
            #endregion Filtro por Plan

            #region Filtro por FechaFinPlan
            if (!string.IsNullOrEmpty(param.FechaFinPlan))
            {
                var fecha = Convert.ToDateTime(param.FechaFinPlan);
                string fechaInicioFin = fecha.ToString("yyyy-MM-dd") + " 00:00:00";
                string fechaFinFin = (fecha.AddMonths(1)).AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:29";

                consulta.AppendLine("SELECT @Where = @Where + ' AND [Fecha Fin Plan] >= ''" + fechaInicioFin + "'' ' ");
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Fecha Fin Plan] <= ''" + fechaFinFin + "'' ' ");
            }
            #endregion Filtro por FechaFinPlan

            consulta.AppendLine("EXEC [RepTabConsumoMovilPorLinea]   ");
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine(fieldsStoredProcedure);
            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @Group = '" + groupStoredProcedure + "',");
            consulta.AppendLine("   @Order = '[Total] Desc', ");
            consulta.AppendLine("   @OrderInv = '[Total] Asc', ");
            consulta.AppendLine("   @OrderDir = 'Asc',");
            consulta.AppendLine("   @Carrier = " + (!string.IsNullOrEmpty(param.ICodCatCarrier) ? param.ICodCatCarrier : "0") + ",");
            consulta.AppendLine("   @Usuario = " + Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Moneda = '" + Session["Currency"] + "', ");
            consulta.AppendLine("   @Idioma = '" + Session["Language"] + "', ");

            if (!string.IsNullOrWhiteSpace(kpiFilter))
            {
                consulta.AppendLine($"   @kpiFilter = '{kpiFilter}', ");
            }

            consulta.AppendLine("   @FechaIniRep = '" + new DateTime(periodoInfo.Year, periodoInfo.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "',");
            consulta.AppendLine("   @FechaFinRep = '" + new DateTime(periodoInfo.Year, periodoInfo.Month, DateTime.DaysInMonth(periodoInfo.Year, periodoInfo.Month), 23, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "' ");

            return consulta.ToString();
        }

        private string ConsultaRepPorCategoria()
        {
            StringBuilder consulta = new StringBuilder();
            DateTime periodoInfo = Convert.ToDateTime(param.PeriodoInfo);

            consulta.AppendLine("DECLARE @Where VARCHAR(MAX)= ''   ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.

            #region Filtro por Linea
            if (!string.IsNullOrEmpty(param.ICodCatLinea) && param.ICodCatLinea != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Linea] = " + param.ICodCatLinea + " ' ");
            }
            #endregion Filtro por Linea

            consulta.AppendLine("EXEC [ConsumoMovilPorCategoria]  ");
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("   [id] = MAX([id]),");
            consulta.AppendLine("   [Concepto],");
            consulta.AppendLine("   [Detalle],");
            consulta.AppendLine("   [idConcepto],");
            consulta.AppendLine("   [Total] = ROUND(SUM([Total]),2),");
            consulta.AppendLine("   [Codigo Carrier],");
            consulta.AppendLine("   [Carrier],");
            consulta.AppendLine("   [Codigo Linea],");
            consulta.AppendLine("   [Linea]");
            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @Group = '[Concepto],[idConcepto],[Codigo Carrier],[Carrier],[Detalle],[Codigo Linea],[Linea]',");
            consulta.AppendLine("   @Order = '[Total] Desc', ");
            consulta.AppendLine("   @OrderInv = '[Total] Asc', ");
            consulta.AppendLine("   @OrderDir = 'Asc',");
            consulta.AppendLine("   @Carrier = " + (!string.IsNullOrEmpty(param.ICodCatCarrier) ? param.ICodCatCarrier : "0") + ",");
            consulta.AppendLine("   @Usuario = " + Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Moneda = '" + Session["Currency"] + "', ");
            consulta.AppendLine("   @Idioma = '" + Session["Language"] + "', ");
            consulta.AppendLine("   @FechaIniRep = '" + new DateTime(periodoInfo.Year, periodoInfo.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "',");
            consulta.AppendLine("   @FechaFinRep = '" + new DateTime(periodoInfo.Year, periodoInfo.Month, DateTime.DaysInMonth(periodoInfo.Year, periodoInfo.Month), 23, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "' ");

            return consulta.ToString();
        }

        private string ConsultaRepPorConceptoDesglose()
        {
            StringBuilder consulta = new StringBuilder();
            DateTime periodoInfo = Convert.ToDateTime(param.PeriodoInfo);

            var fechaIni = new DateTime(periodoInfo.Year, periodoInfo.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            var fechaFin = new DateTime(periodoInfo.Year, periodoInfo.Month, DateTime.DaysInMonth(periodoInfo.Year, periodoInfo.Month), 23, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");

            consulta.AppendLine("DECLARE @Where VARCHAR(MAX)= ''   ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.
            consulta.AppendLine("SELECT @Where = 'FechaInicio BETWEEN ''" + fechaIni + "'' AND ''" + fechaFin + "'' '");

            #region Filtro por Linea
            if (!string.IsNullOrEmpty(param.ICodCatLinea) && param.ICodCatLinea != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Linea] = " + param.ICodCatLinea + " ' ");
            }
            #endregion Filtro por Linea

            #region Filtro por Concepto
            if (!string.IsNullOrEmpty(param.Categoria) && param.Categoria != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [idConcepto] = " + param.Categoria + " ' ");
            }
            #endregion Filtro por Concepto

            consulta.AppendLine("EXEC [spConsolidadoFacturasDeMovilesRest]  ");  //Este es el que se usa en el Dashboard de moviles
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("   [idConcepto],");
            consulta.AppendLine("   [Concepto] = UPPER([Concepto]),");
            consulta.AppendLine("   [Descripcion],");
            consulta.AppendLine("   [Telefono],");
            consulta.AppendLine("   [Total] = ROUND(SUM([Total]),2)");
            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @Group = 'UPPER([Concepto]),[Descripcion],[idConcepto],[Telefono]',");
            consulta.AppendLine("   @Order = '[Total] Desc,[Concepto] Asc', ");
            consulta.AppendLine("   @OrderInv = '[Total] Asc,[Concepto] Desc', ");
            consulta.AppendLine("   @OrderDir = 'Desc',");
            consulta.AppendLine("   @Carrier = " + (!string.IsNullOrEmpty(param.ICodCatCarrier) ? param.ICodCatCarrier : "0") + ",");
            consulta.AppendLine("   @Usuario = " + Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Moneda = '" + Session["Currency"] + "', ");
            consulta.AppendLine("   @Idioma = '" + Session["Language"] + "', ");
            consulta.AppendLine("   @FechaIniRep = '''" + new DateTime(periodoInfo.Year, periodoInfo.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "''',");
            consulta.AppendLine("   @FechaFinRep = '''" + new DateTime(periodoInfo.Year, periodoInfo.Month, DateTime.DaysInMonth(periodoInfo.Year, periodoInfo.Month), 23, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "''' ");

            return consulta.ToString();
        }

        private string ConsultaRepPorConceptoDetalleLamadas()
        {
            StringBuilder consulta = new StringBuilder();
            DateTime periodoInfo = Convert.ToDateTime(param.PeriodoInfo);

            var fechaIni = new DateTime(periodoInfo.Year, periodoInfo.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            var fechaFin = new DateTime(periodoInfo.Year, periodoInfo.Month, DateTime.DaysInMonth(periodoInfo.Year, periodoInfo.Month), 23, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");

            consulta.AppendLine("DECLARE @Where VARCHAR(MAX)= '' ");
            consulta.AppendLine("DECLARE @CampoTotal VARCHAR(40) = '' ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.
            consulta.AppendLine("SELECT @Where = 'FechaInicio BETWEEN ''" + fechaIni + "'' AND ''" + fechaFin + "'' '");

            #region Filtro por Linea
            if (!string.IsNullOrEmpty(param.ICodCatLinea) && param.ICodCatLinea != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Linea] = " + param.ICodCatLinea + " ' ");
            }
            #endregion Filtro por Linea

            #region Filtro por Carrier
            if (!string.IsNullOrEmpty(param.ICodCatCarrier) && param.ICodCatCarrier != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Carrier] = " + param.ICodCatCarrier + " ' ");
            }
            #endregion Filtro por Carrier

            #region Filtro por Concepto
            if (!string.IsNullOrEmpty(param.Categoria) && param.Categoria != "0")
            {
                consulta.AppendLine("");
                consulta.AppendLine("SELECT @CampoTotal = (SELECT vchCodigo FROM " + DSODataContext.Schema + ".Catalogos WHERE iCodRegistro = " + param.Categoria + ")");
            }
            #endregion Filtro por Concepto

            consulta.AppendLine("");
            consulta.AppendLine("EXEC [ConsumoTelcelDetalleLlamRestSinClaveCar]  ");  //Este es el que se usa en el Dashboard de moviles
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("   [Fecha Llamada],");
            consulta.AppendLine("   [Hora Llamada],");
            consulta.AppendLine("   [Numero Marcado] = '''''''' + [Numero Marcado],");
            consulta.AppendLine("   [Duracion Minutos],");
            consulta.AppendLine("   [Costo]=([Costo]/[TipoCambio]),");
            consulta.AppendLine("   [Punta A],");
            consulta.AppendLine("   [Dir Llamada],");
            consulta.AppendLine("   [Punta B]");
            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,");
            consulta.AppendLine("   @CampoTotal = @CampoTotal,");
            consulta.AppendLine("   @Group = '',");
            consulta.AppendLine("   @Usuario = " + Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Moneda = '" + Session["Currency"] + "', ");
            consulta.AppendLine("   @Idioma = '" + Session["Language"] + "', ");
            consulta.AppendLine("   @FechaIniRep = '''" + new DateTime(periodoInfo.Year, periodoInfo.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "''',");
            consulta.AppendLine("   @FechaFinRep = '''" + new DateTime(periodoInfo.Year, periodoInfo.Month, DateTime.DaysInMonth(periodoInfo.Year, periodoInfo.Month), 23, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "''' ");

            return consulta.ToString();
        }

        private string ConsultaRepConsumoIntermetHistorico()
        {
            StringBuilder consulta = new StringBuilder();
            DateTime periodoInfo = Convert.ToDateTime(param.PeriodoInfo);

            consulta.Append("EXEC [ConsumoTelcelInternetHistorico]");
            consulta.Append("   @Schema ='" + DSODataContext.Schema + "',");
            consulta.Append("   @Lineas ='" + param.ICodCatLinea + "', ");
            consulta.Append("   @Carrier = " + (!string.IsNullOrEmpty(param.ICodCatCarrier) ? param.ICodCatCarrier : "0") + ",");
            consulta.Append("   @Fields = '[Fecha],[Número Telcel],[Responsable],[Internet Disponible (MB)],");
            consulta.Append("       [Consumo (MB)],[Gasto Local],[Gasto Internacional],[Anio],[FechaK5],[PromedioAnual]',");
            consulta.Append("   @FechaInicio='" + new DateTime(periodoInfo.Year, periodoInfo.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "',");
            consulta.Append("   @FechaFin='" + new DateTime(periodoInfo.Year, periodoInfo.Month, DateTime.DaysInMonth(periodoInfo.Year, periodoInfo.Month), 23, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") + "'");

            return consulta.ToString();
        }

        #endregion

        #region Reportes

        public void DashboardPrincipal()
        {
            switch (param.ClaveNivelReporte)
            {
                case "ConsumoMovil":
                    ReportePrincipal(Rep2, "javascript.Navegar('Por Concepto [{0}]','Categoria','{1}','{2}','0');");
                    break;
                case "Categoria":
                    RepPorCategoria(Rep2_1, "javascript.Navegar('Desglose de Concepto [{0}]','Concepto','{1}','{2}','{3}');", Rep2_2, Rep4);
                    break;
                case "Concepto":
                    RepPorConcepto(Rep2, "");
                    break;
                default:
                    break;
            }
        }

        private void ReportePrincipal(Control contenedor, string link)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaRepConsumoMovilLineas());
            string tituloReporte = "Consumo por Linea";

            switch (kpiFilter) 
            {
                case "nuevas":
                    tituloReporte = "Líneas nuevas";
                    break;
                case "bajas":
                    tituloReporte = "Líneas dadas de baja";
                    break;
                case "planabierto":
                    tituloReporte = "Líneas con Plan abierto";
                    break;
                case "paquetescont":
                    tituloReporte = "Líneas con paquetes contratados";
                    break;
                default:
                    break;
            }

            // Se agrega cambio a datatable solo para cliente Komatsu
            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                string[] campos;
                int[] camposBoundField;
                int[] camposNavegacion;
                string[] formatoColumnas;
                if (DSODataContext.Schema.ToUpper() != "ROADMACHINERY")
                {
                    campos = new string[] { "Codigo Linea", "Linea", "Codigo Carrier", "Carrier", "Nombre Completo",
                    "Nombre Centro de Costos", "Renta", "Excedente", "Total", "Plan", "Fecha Fin Plan"};

                    camposBoundField = new int[] { 3, 4, 5, 6, 7, 8, 9, 10 };

                    camposNavegacion = new int[] { 0, 2 };

                    formatoColumnas = new string[] { "", "", "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "", "" };
                }
                else
                {
                    dt.Columns["Total"].ColumnName = "Subtotal";
                    dt.Columns.Add("Total", typeof(System.Double));

                    // Agregamos los valores a la columna total los cuales serán la suma de Renta, Excedente, IVA
                    foreach (DataRow fila in dt.Rows)
                    {
                        var totalCosto = Convert.ToDouble(fila["Renta"].ToString()) + Convert.ToDouble(fila["Excedente"].ToString()) + +Convert.ToDouble(fila["IVA"].ToString());
                        fila["Total"] = totalCosto;
                    }

                    campos = new string[] { "Codigo Linea", "Linea", "Modelo", "Codigo Carrier", "Carrier", "Nombre Completo",
                    "Nombre Centro de Costos", "Renta", "Excedente", "Subtotal", "IVA", "Total", "Plan", "Fecha Fin Plan" };

                    // La columna o campo 16 corresponde a la última columa agregada Total. La cual solo se mostrara para Komatsu
                    camposBoundField = new int[] { 2, 4, 5, 6, 7, 8, 9, 10, 16, 11, 12, 13 };

                    camposNavegacion = new int[] { 0, 3 };

                    formatoColumnas = new string[] { "", "", "", "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "", "" };
                }

                dt = dvldt.ToTable(false, campos);

                GridView grid = DTIChartsAndControls.GridView("RepConsumoMovil", dt, true, "Totales",
                      formatoColumnas,
                      link, new string[] { "Linea", "Codigo Linea", "Codigo Carrier" },
                      1, camposNavegacion, camposBoundField, new int[] { 1 });
                dt.AcceptChanges();

                grid.RowDataBound += (sender, e) => cambiaURLFunctionJava_RowDataBound(sender, e);
                grid.DataBind();
                grid.UseAccessibleHeader = true;
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = grid.Rows[grid.Rows.Count - 1];
                row.Cells[1].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));

            }
        }

        private void RepPorCategoria(Control contenedor, string link, Control contenedorGrafica, Control contenedorRepInternet)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaRepPorCategoria());
            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                #region Reporte Concepto
                param.Linea = dt.Rows[0]["Linea"].ToString();

                DataView dvdt = new DataView(dt);
                dt = dvdt.ToTable(false, new string[] { "idConcepto", "Concepto", "Total", "Codigo Linea", "Codigo Carrier", "Linea" });
                dt.AcceptChanges();

                GridView grid = DTIChartsAndControls.GridView("RepPorCategoria", dt, true, "Totales",
                        new string[] { "", "", "{0:c}", "", "", "" }, link, new string[] { "Concepto", "Codigo Linea", "Codigo Carrier", "idConcepto" },
                        1, new int[] { 0, 3, 4 }, new int[] { 2 }, new int[] { 1 });

                grid.RowDataBound += (sender, e) => cambiaURLFunctionJava_RowDataBound(sender, e);
                grid.DataBind();
                grid.UseAccessibleHeader = true;
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = grid.Rows[grid.Rows.Count - 1];
                row.Cells[1].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "RepPorCategoria", "Por categoría", string.Empty));

                #endregion

                #region Grafica

                dt.Columns.Add("link");
                foreach (DataRow ldr in dt.Rows)
                {
                    ldr["link"] = string.Format(link, ldr["Linea"].ToString(), ldr["Codigo Linea"].ToString(), ldr["Codigo Carrier"].ToString(), ldr["idConcepto"].ToString())
                        .Replace("javascript.", "javascript:").Replace(";", "");
                }

                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Concepto", "Total", "link" });
                if (dt.Rows[dt.Rows.Count - 1]["Concepto"].ToString() == "Totales")
                {
                    dt.Rows[dt.Rows.Count - 1].Delete();
                }
                dt.Columns["Concepto"].ColumnName = "label";
                dt.Columns["Total"].ColumnName = "value";
                dt.AcceptChanges();

                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("RepPorCategoriaGraf", "Por categoría"));

                ScriptManager.RegisterStartupScript(this, typeof(Page), "RepPorCategoriaGraf",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt),
                    "RepPorCategoriaGraf", "", "", "", "Importe", "bar2d"), false);

                #endregion

                #region Reporte Internet

                DataTable dtInt = DSODataAccess.Execute(ConsultaRepConsumoIntermetHistorico());
                if (dtInt.Rows.Count > 0 && dtInt.Columns.Count > 0)
                {
                    bool banderaInternetIlimitado = CalculoDeColumnasRepHist(dtInt);

                    //Se especifica que columnas se mostraran en el segun nivel dependiendo de esta bandera.
                    int[] mostrar;
                    int[] noMostrar;
                    if (banderaInternetIlimitado)
                    {
                        mostrar = new int[] { 3, 4, 7, 8 };
                        noMostrar = new int[] { 0, 1, 2, 5, 6, 9 };
                    }
                    else
                    {
                        mostrar = new int[] { 0, 2, 3, 7, 5, 6, 8, 10 };
                        noMostrar = new int[] { 1, 4, 9, 11, 12, 13 };
                    }

                    contenedorRepInternet.Controls.Clear();
                    contenedorRepInternet.Controls.Add(
                         DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                                          DTIChartsAndControls.GridView("RepConHistoricoIntColNvl2", DTIChartsAndControls.ordenaTabla(dtInt, "[FechaK5] Asc"),
                                         false, "", new string[] { "", "", "{0:p}", "{0:n}", "{0:n}", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "", "{0:c}", "{0:n}", "", "{0:c}" },
                                         "", new string[] { }, 0, noMostrar, mostrar, new int[] { }),
                                         "RepConsumoInternet", "Consumo Histórico de Internet", string.Empty)
                         );
                }

                #endregion
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepConsumoPorCategoria", "Consumo por categoría", string.Empty));
            }
        }

        private static bool CalculoDeColumnasRepHist(DataTable RepConHistoricoIntCol)
        {
            bool banderaInternetIlimitado = false;
            #region Columnas Calculadas
            if (RepConHistoricoIntCol.Rows.Count > 0 && RepConHistoricoIntCol.Columns.Count > 0)
            {
                RepConHistoricoIntCol.Columns.Add("% Utilizado", System.Type.GetType("System.String")).SetOrdinal(2);
                RepConHistoricoIntCol.Columns.Add("Excedio (MB)", System.Type.GetType("System.Decimal")).SetOrdinal(5);
                RepConHistoricoIntCol.Columns.Add("% Excedido", System.Type.GetType("System.Decimal")).SetOrdinal(6);
                RepConHistoricoIntCol.Columns.Add("Internet Disponible (MB) ", System.Type.GetType("System.String")).SetOrdinal(9);

                for (int row = 0; row < RepConHistoricoIntCol.Rows.Count; row++)
                {
                    // concepto Internet
                    if (RepConHistoricoIntCol.Rows[row][4] == DBNull.Value || Convert.ToInt32(RepConHistoricoIntCol.Rows[row][4]) == 0)
                    {
                        RepConHistoricoIntCol.Rows.Remove(RepConHistoricoIntCol.Rows[row]);
                    }
                    else
                    {
                        decimal variable;
                        //concepto internet
                        if (Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]) == (9999 * 1024))
                        {
                            //% utilizado
                            RepConHistoricoIntCol.Rows[row][2] = "Internet Ilimitado";
                            banderaInternetIlimitado = true;
                        }
                        else
                        {
                            variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][7]) / Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                            RepConHistoricoIntCol.Rows[row][2] = (variable > 1M) ? "100 %" : variable.ToString("0.## %");
                        }

                        //consumoMB - internet
                        variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][7]) - Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                        RepConHistoricoIntCol.Rows[row][5] = (variable < 0M) ? 0M : variable;  //excedio MB


                        //excedioMB - internet
                        variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][5]) / Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                        RepConHistoricoIntCol.Rows[row][6] = (variable < 0M) ? 0M : variable;  // % excedido

                        //internet
                        variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                        RepConHistoricoIntCol.Rows[row][9] = (variable == (9999 * 1024)) ? "Internet Ilimitado" : variable.ToString("##,0.00"); //internet

                    }
                }

                RepConHistoricoIntCol.Columns[7].SetOrdinal(5);
                RepConHistoricoIntCol.Columns[9].SetOrdinal(3);
                RepConHistoricoIntCol.Columns[4].SetOrdinal(9);
            }
            #endregion
            return banderaInternetIlimitado;
        }

        private void RepPorConcepto(Control Contenedor, string link)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaRepPorConceptoDesglose());
            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                DataView dvRepDesgloseFacturaPorConcepto = new DataView(dt);
                dt = dvRepDesgloseFacturaPorConcepto.ToTable(false, new string[] { "Concepto", "Descripcion", "Total" });
                dt.Columns["Descripcion"].ColumnName = "Descripción";
                dt.AcceptChanges();

                #region Seleccion de reporte

                if (dt.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                {
                    Contenedor.Controls.Add(
                        DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                                DTIChartsAndControls.GridView("RepPorConcepto", DTIChartsAndControls.ordenaTabla(dt, "Total desc"),
                                true, "Totales", new string[] { "", "", "{0:c}" }), "RepPorConcepto", "Por Concepto", string.Empty));
                }
                else
                {
                    DataTable dtLlams = DSODataAccess.Execute(ConsultaRepPorConceptoDetalleLamadas());

                    if (dtLlams.Rows.Count > 0 && dtLlams.Columns.Count > 0)
                    {
                        DataView dvRepDesgloseFacturaPorLlamada = new DataView(dtLlams);
                        dtLlams = dvRepDesgloseFacturaPorLlamada.ToTable(false,
                            new string[] { "Fecha Llamada", "Hora Llamada", "Numero Marcado", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                        dtLlams.Columns["Fecha Llamada"].ColumnName = "Fecha";
                        dtLlams.Columns["Hora Llamada"].ColumnName = "Hora";
                        dtLlams.Columns["Numero Marcado"].ColumnName = "Número marcado";
                        dtLlams.Columns["Duracion Minutos"].ColumnName = "Minutos";
                        dtLlams.Columns["Costo"].ColumnName = "Total";
                        dtLlams.Columns["Punta A"].ColumnName = "Localidad Origen";
                        dtLlams.Columns["Dir Llamada"].ColumnName = "Tipo";
                        dtLlams.Columns["Punta B"].ColumnName = "Localidad Destino";
                        dtLlams.AcceptChanges();
                    }

                    Contenedor.Controls.Clear();
                    Contenedor.Controls.Add(
                     DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                           DTIChartsAndControls.GridView("RepDesgloseFacturaPorLlamada", DTIChartsAndControls.ordenaTabla(dtLlams, "Total desc"),
                           true, "Totales", new string[] { "", "", "", "", "{0:c}", "", "", "" }), "RepDesgloseLlamadas", "Desglose de llamadas", string.Empty));
                }

                #endregion Seleccion de reporte
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                Contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepPorConcepto", "Por Concepto", string.Empty));

            }
        }

        #endregion

        #region Exportación

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            List<Parametros> lista = new List<Parametros>();

            if (Session["Navegacion"] != null) //Entonces ya tiene navegacion almacenada
            {
                lista = (List<Parametros>)Session["Navegacion"];
                param = lista[lista.Count - 1];
            }
            else
            {
                param = new Parametros();
                param.ClaveNivelReporte = "ConsumoMovil";
                param.TituloNav = "Consumo por Línea";
            }

            ExportXLS();
        }

        public void ExportXLS()
        {
            CrearXLS(".xlsx");
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                #region Exportar reportes con tabla y grafica

                if (param.ClaveNivelReporte == "Ejemplo1")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGraficoMovil" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param.ClaveNivelReporte == "Ejemplo1")
                    {
                        //ProcesarTituloExcel(lExcel, "Consumo histórico");

                        //#region Reporte 

                        ////Se obtiene el DataSource del reporte
                        //DataTable GrafConsHist = DSODataAccess.Execute(consultaConsumoHistoricoMov());

                        //if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
                        //{
                        //    DataView dvGrafConsHist = new DataView(GrafConsHist);
                        //    GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                        //    GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                        //    GrafConsHist.Columns["Total"].ColumnName = "Total";
                        //    GrafConsHist.AcceptChanges();
                        //}

                        //crearGrafico(GrafConsHist, "Consumo histórico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                        //                 true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        //creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsHist, 0, "Totales"), "Reporte", "Tabla");

                        //#endregion Reporte 
                    }
                }

                #endregion Exportar reportes con tabla y grafica

                #region Exportar Reportes solo con tabla

                else if (param.ClaveNivelReporte == "ConsumoMovil" || param.ClaveNivelReporte == "Concepto")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaMovil" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                    if (param.ClaveNivelReporte == "ConsumoMovil")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, param.TituloNav);

                        #region Reporte

                        DataTable dt = DSODataAccess.Execute(ConsultaRepConsumoMovilLineas());
                        string[] campos = null;

                        if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                        {
                            if (DSODataContext.Schema.ToUpper() != "ROADMACHINERY")
                            {
                                campos = new string[] { "Linea", "Carrier", "Nombre Completo", "Nombre Centro de Costos",
                                "Renta", "Excedente", "Total", "Plan", "Fecha Fin Plan" };
                            }
                            else
                            {
                                dt.Columns["Total"].ColumnName = "Subtotal";
                                dt.Columns.Add("Total", typeof(System.Double));

                                foreach (DataRow fila in dt.Rows)
                                {
                                    var totalCosto = Convert.ToDouble(fila["Renta"].ToString()) + Convert.ToDouble(fila["Excedente"].ToString()) + +Convert.ToDouble(fila["IVA"].ToString());
                                    fila["Total"] = totalCosto;
                                }

                                campos = new string[] { "Linea", "Modelo", "Codigo Carrier", "Carrier", "Nombre Completo",
                                "Nombre Centro de Costos", "Renta", "Excedente", "Subtotal", "IVA", "Total", "Plan", "Fecha Fin Plan" };
                            }

                            DataView dvldt = new DataView(dt);
                            dt = dvldt.ToTable(false, campos);

                            dt.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion
                    }
                    else if (param.ClaveNivelReporte == "Concepto")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, param.TituloNav);

                        #region Reporte

                        DataTable dt = DSODataAccess.Execute(ConsultaRepPorConceptoDesglose());
                        if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                        {
                            DataView dvRepDesgloseFacturaPorConcepto = new DataView(dt);
                            dt = dvRepDesgloseFacturaPorConcepto.ToTable(false, new string[] { "Concepto", "Descripcion", "Total" });
                            dt.Columns["Descripcion"].ColumnName = "Descripción";
                            dt.AcceptChanges();

                            #region Seleccion de reporte

                            if (dt.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(DTIChartsAndControls.ordenaTabla(dt, "Total desc"), 0, "Totales"), "Reporte", "Tabla");
                            }
                            else
                            {
                                DataTable dtLlams = DSODataAccess.Execute(ConsultaRepPorConceptoDetalleLamadas());

                                if (dtLlams.Rows.Count > 0 && dtLlams.Columns.Count > 0)
                                {
                                    DataView dvRepDesgloseFacturaPorLlamada = new DataView(dtLlams);
                                    dtLlams = dvRepDesgloseFacturaPorLlamada.ToTable(false,
                                        new string[] { "Fecha Llamada", "Hora Llamada", "Numero Marcado", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                                    dtLlams.Columns["Fecha Llamada"].ColumnName = "Fecha";
                                    dtLlams.Columns["Hora Llamada"].ColumnName = "Hora";
                                    dtLlams.Columns["Numero Marcado"].ColumnName = "Número marcado";
                                    dtLlams.Columns["Duracion Minutos"].ColumnName = "Minutos";
                                    dtLlams.Columns["Costo"].ColumnName = "Total";
                                    dtLlams.Columns["Punta A"].ColumnName = "Localidad Origen";
                                    dtLlams.Columns["Dir Llamada"].ColumnName = "Tipo";
                                    dtLlams.Columns["Punta B"].ColumnName = "Localidad Destino";
                                    dtLlams.AcceptChanges();
                                }

                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(DTIChartsAndControls.ordenaTabla(dtLlams, "Total desc"), 0, "Totales"), "Reporte", "Tabla");
                            }

                            #endregion Seleccion de reporte
                        }

                        #endregion
                    }
                }

                #endregion Exportar Reportes solo con tabla

                #region Exportar Reportes con 2 Tablas

                else if (param.ClaveNivelReporte == "Ejemplo3")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteDosTablas" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param.ClaveNivelReporte == "Ejemplo3")   //NZ 20150713   
                    {
                        //ProcesarTituloExcel(lExcel, "Consumo Histórico por Área");

                        //#region Reporte Consumo Histórico por Área
                        //DataTable dtRepAñoActual = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMatAnioAct());

                        //if (dtRepAñoActual.Rows.Count > 0)
                        //{
                        //    #region Elimina columnas no necesarias en el gridview
                        //    if (dtRepAñoActual.Columns.Contains("RID"))
                        //        dtRepAñoActual.Columns.Remove("RID");
                        //    if (dtRepAñoActual.Columns.Contains("RowNumber"))
                        //        dtRepAñoActual.Columns.Remove("RowNumber");
                        //    if (dtRepAñoActual.Columns.Contains("TopRID"))
                        //        dtRepAñoActual.Columns.Remove("TopRID");
                        //    if (dtRepAñoActual.Columns.Contains("Codigo Centro de Costos"))
                        //        dtRepAñoActual.Columns.Remove("Codigo Centro de Costos");
                        //    #endregion // Elimina columnas no necesarias en el gridview

                        //    creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAñoActual, 0, "Totales"), "Reporte", "Tabla1");
                        //}

                        //DataTable dtRepAñoAnterior = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMatAnioAnt());

                        //if (dtRepAñoAnterior.Rows.Count > 0)
                        //{
                        //    #region Elimina columnas no necesarias en el gridview
                        //    if (dtRepAñoAnterior.Columns.Contains("RID"))
                        //        dtRepAñoAnterior.Columns.Remove("RID");
                        //    if (dtRepAñoAnterior.Columns.Contains("RowNumber"))
                        //        dtRepAñoAnterior.Columns.Remove("RowNumber");
                        //    if (dtRepAñoAnterior.Columns.Contains("TopRID"))
                        //        dtRepAñoAnterior.Columns.Remove("TopRID");
                        //    if (dtRepAñoAnterior.Columns.Contains("Codigo Centro de Costos"))
                        //        dtRepAñoAnterior.Columns.Remove("Codigo Centro de Costos");
                        //    #endregion // Elimina columnas no necesarias en el gridview

                        //    creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAñoAnterior, 0, "Totales"), "Reporte", "Tabla2");
                        //}

                        //#endregion Reporte Consumo Histórico por Área
                    }

                }

                #endregion Exportar Reportes con 2 Tablas

                #region Exportar Reportes con 2 Tablas y una grafica

                else if (param.ClaveNivelReporte == "Categoria")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte2TablaYGraficoMovil" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param.ClaveNivelReporte == "Categoria")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, param.TituloNav);

                        #region Reporte
                        DataTable dt = DSODataAccess.Execute(ConsultaRepPorCategoria());
                        if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                        {
                            #region Reporte Internet

                            DataTable dtInt = DSODataAccess.Execute(ConsultaRepConsumoIntermetHistorico());
                            if (dtInt.Rows.Count > 0 && dtInt.Columns.Count > 0)
                            {
                                bool banderaInternetIlimitado = CalculoDeColumnasRepHist(dtInt);

                                //Se especifica que columnas se mostraran en el segun nivel dependiendo de esta bandera.
                                int[] mostrar;
                                int[] noMostrar;
                                if (banderaInternetIlimitado)
                                {
                                    mostrar = new int[] { 3, 4, 7, 8 };
                                    noMostrar = new int[] { 0, 1, 2, 5, 6, 9 };
                                }
                                else
                                {
                                    mostrar = new int[] { 0, 2, 3, 7, 5, 6, 8, 10 };
                                    noMostrar = new int[] { 1, 4, 9, 11, 12, 13 };
                                }

                                for (int i = noMostrar.Length - 1; i >= 0; i--)
                                {
                                    dtInt.Columns.RemoveAt(noMostrar[i]);
                                }
                            }

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtInt, 0, "Totales"), "Reporte", "Tabla2");

                            #endregion

                            #region Reporte Concepto

                            DataView dvdt = new DataView(dt);
                            dt = dvdt.ToTable(false, new string[] { "Concepto", "Total" });
                            dt.AcceptChanges();

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dt, "Total desc", 10), "Consumo Por Concepto", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Concepto", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla1");

                            #endregion


                        }

                        #endregion Reporte
                    }

                }

                #endregion Exportar Reportes con 2 Tablas

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;
                lExcel.SalvarComo();

                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes" + "_" + param.ClaveNivelReporte + "_");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, lsExt);
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

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            string script = "location.href='../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt + "';";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "script", script, true);
        }

        #endregion

    }

    public class Parametros
    {
        public string PeriodoInfo { get; set; }
        public string ICodCatCarrier { get; set; }
        public string ICodCatLinea { get; set; }
        public string ICodCatEmple { get; set; }
        public string ICodCatCenCos { get; set; }
        public string ICodCatPlan { get; set; }
        public string FechaFinPlan { get; set; }

        public string Categoria { get; set; }

        public string ClaveNivelReporte { get; set; }
        public string TituloNav { get; set; }

        public string Linea { get; set; }
    }
}