using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using KeytiaServiceBL;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;
using AjaxControlToolkit;
using System.Globalization;
using KeytiaServiceBL.Reportes;
using System.Collections;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    public partial class ConsumoGeneralPorSitio : System.Web.UI.Page
    {
        #region Variables

        StringBuilder query = new StringBuilder();
        string maxFechaAnio = "";
        string carrierSelected = "";
        string monedaGlobal = "0";
        string empreSelected = "-1";
        List<InfoCarrierGlobal> dtCarriers = new List<InfoCarrierGlobal>();
        List<InfoCarrierGlobal> dtEmpres = new List<InfoCarrierGlobal>();

        SitioCveCar param = new SitioCveCar();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                //Session["FechaInicio"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
                //Session["FechaFin"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).ToString("yyyy-MM-dd");

                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                if (!Page.IsPostBack)
                {
                    //Llenar el combo de Carriers que tiene el TIM
                    GetCarriersTIM(0, 0);
                    //Distinct de Empresas
                    dtCarriers.GroupBy(g => new { g.Empre, g.EmpreDesc }).Select(x => x.First()).ToList().ForEach(n => dtEmpres.Add(new InfoCarrierGlobal() { Empre = n.Empre, EmpreDesc = n.EmpreDesc }));

                    var carriers = dtCarriers.GroupBy(g => new { g.iCodCatalogo, g.vchDescripcion }).Select(x => x.First()).ToList();
                    carriers.Insert(0, new InfoCarrierGlobal() { iCodCatalogo = -1, vchDescripcion = "TODOS" });
                    ddlCarrier.DataSource = carriers;
                    ddlCarrier.DataValueField = "iCodCatalogo";
                    ddlCarrier.DataTextField = "vchDescripcion";
                    ddlCarrier.DataBind();

                    ddlEmpre.DataSource = dtEmpres;
                    ddlEmpre.DataValueField = "Empre";
                    ddlEmpre.DataTextField = "EmpreDesc";
                    ddlEmpre.DataBind();

                    if (carriers.Count == 1) { ddlCarrier.Enabled = ddlCarrier.Visible = false; }
                    if (dtEmpres.Count == 1) { ddlEmpre.Enabled = ddlEmpre.Visible = false; }
                    if (dtEmpres.Count == 1 && carriers.Count == 1) { btnAplicar.Enabled = btnAplicar.Visible = false; }
                }

                if (ddlCarrier.Items.Count > 0)
                {
                    DashboardPrincipal();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void DashboardPrincipal()
        {
            GetCarriersTIM(int.Parse(ddlCarrier.SelectedItem.Value), int.Parse(ddlEmpre.SelectedItem.Value));
            if (dtCarriers.Count > 0)
            {
                carrierSelected = ddlCarrier.SelectedItem.Value;
                empreSelected = ddlEmpre.SelectedItem.Value;
                maxFechaAnio = dtCarriers.Max(x => x.Año).ToString();
                lblCarrierConsumo.Text = "Consumo por Sitio";

                //Reporte Matricial por Sitio / Categoria
                if (!Page.IsPostBack || !ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
                {
                    RepMatPorSitioTDest();

                    RepMatPorEnlace2Años();

                    RepEnlacesCapacidad();
                }
                else
                {
                    if (Request["__EVENTARGUMENT"] != null)
                    {
                        param = (new System.Web.Script.Serialization.JavaScriptSerializer()).Deserialize<SitioCveCar>(Request["__EVENTARGUMENT"]);
                        Session["iCodCatSitioNombre"] = param.ICodCatSitioNombre;  //NZ: Se usa para la exportación
                    }
                    if (param == null)
                    {
                        param = new SitioCveCar();
                        param.ICodCatSitioNombre = Session["iCodCatSitioNombre"].ToString();
                    }
                }

                //Grafica Por Sitio 2 Años
                RepMatPorSitio2Años();

                //Reporte Matricial por ClaveCar / Meses
                RepMatPorSitioClaveCarMeses();
            }
        }

        #region Consultas a BD

        private void GetCarriersTIM(int iCodCarrier, int iCodCatEmpre)
        {
            dtCarriers = new ConsumoGeneral().GetCarriersTIM(iCodCarrier, iCodCatEmpre, "ConsumoGeneralPorSitio");
        }

        private string GetConsumoPorSitioMatAnioActual()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorSitioMatTDestAnioActual] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @EnMonedaGlobal = " + monedaGlobal);

            return query.ToString();
        }

        private string GetConsumoPorSitioClaveCarMatAnioActual()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorSitioMatClaveCarAnioActual] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @iCodCatSitioNombre = " + param.ICodCatSitioNombre + ",");
            query.AppendLine("     @EnMonedaGlobal = " + monedaGlobal);

            return query.ToString();
        }

        private string GetConsumoPorSitio2Años(int iCodCatSitio, int iCodCatClaveCar)
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorSitio2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @iCodCatSitioNombre = " + iCodCatSitio + ",");
            query.AppendLine("     @iCodCatClaveCar = " + iCodCatClaveCar + ",");
            query.AppendLine("     @EnMonedaGlobal = " + monedaGlobal);

            return query.ToString();
        }

        private string GetInventarioEnlaces(int byAnchoBanda)
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorSitioMatInventarioEnlaces2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @ByAnchoBanda = " + byAnchoBanda);

            return query.ToString();
        }

        #endregion Consultas a BD


        #region Reportes

        private void RepMatPorSitioTDest()
        {
            DataTable dtResult = DSODataAccess.Execute(GetConsumoPorSitioMatAnioActual());

            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                if (dtResult.Columns.Contains("ImporteMoneda"))
                    dtResult.Columns.Remove("ImporteMoneda");

                //Establecemos la variable Sitio para que se llene con lo primero
                param.ICodCatSitioNombre = dtResult.Rows[0]["iCodCatSitioNombre"].ToString();
                param.SitioDescripcion = dtResult.Rows[0]["SitioDescripcion"].ToString();
                Session["iCodCatSitioNombre"] = param.ICodCatSitioNombre;
                Session["SitioDescripcion"] = param.SitioDescripcion;

                //Para Calcular los formatos y columnas visibles
                List<string> formatStrings = new List<string>();
                formatStrings.Add("");
                formatStrings.Add("");
                for (int i = 2; i <= dtResult.Columns.Count; i++)
                {
                    formatStrings.Add("{0:c}");
                }

                List<int> boundFields = new List<int>();
                for (int i = 2; i <= dtResult.Columns.Count; i++)
                {
                    boundFields.Add(i);
                }

                dtResult.Columns["SitioDescripcion"].ColumnName = "Sitio";

                //Reporte    
                GridView gvRepMatPorSitioTDest = DTIChartsAndControls.GridView("RepMatPorSitioTDest", dtResult, true, "Totales",
                     formatStrings.ToArray(), "javascript.ActualizarRepSitio('{0}');", new string[] { "iCodCatSitioNombre" }, 1, new int[] { 0 },
                     boundFields.ToArray(), new int[] { 1 }, 2, false);

                gvRepMatPorSitioTDest.RowDataBound += (sender, e) => CambiaURLFunctionJava_RowDataBound(sender, e);
                gvRepMatPorSitioTDest.DataBind();
                gvRepMatPorSitioTDest.UseAccessibleHeader = true;
                gvRepMatPorSitioTDest.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepMatPorSitioTDest.Rows[gvRepMatPorSitioTDest.Rows.Count - 1];
                row.Cells[1].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }
                //Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvRepMatPorSitioTDest, "RepMatPorSitioTDest", "Consumo por Sitio y Categoría " + maxFechaAnio, "", false));
                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvRepMatPorSitioTDest, "RepMatPorSitioTDest", "Consumo por Sitio y Categoría " + maxFechaAnio, true, "btnExportarXLS_Click",
                             "btnConsumoPorSitio", this, false));
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepMatPorSitioTDest", "Consumo por Sitio y Categoría " + maxFechaAnio, "", false));
            }
        }

        private void RepMatPorSitio2Años()
        {
            Rep2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafPoriCodCatSitioNombreSitioDescripcion", "Gráfica por Sitio " + maxFechaAnio));

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("iCodCatSitioNombre");
            dtResult.Columns.Add("SitioDescripcion");
            DataRow row = dtResult.NewRow();
            row["iCodCatSitioNombre"] = param.ICodCatSitioNombre;
            row["SitioDescripcion"] = param.SitioDescripcion;
            dtResult.Rows.Add(row);

            ScriptManager.RegisterStartupScript(this, typeof(Page), "CambiaGraficaDrillDowniCodCatSitioNombreSitioDescripcion",
                   javaScriptDrillDownSitioTDest(dtResult, "iCodCatSitioNombre", "SitioDescripcion", "IMPORTE", "SitioDescripcion", true, "375"), false);

            if (!string.IsNullOrEmpty(param.ICodCatSitioNombre))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "funcionSitio", "javascript:DrillDowniCodCatSitioNombreSitioDescripcion('" + param.ICodCatSitioNombre + "')", true);
            }
        }

        private void RepMatPorSitioClaveCarMeses()
        {
            //System.Threading.Thread.Sleep(5000);
            DataTable dtResult = DSODataAccess.Execute(GetConsumoPorSitioClaveCarMatAnioActual());

            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                if (dtResult.Columns.Contains("ImporteMoneda"))
                    dtResult.Columns.Remove("ImporteMoneda");

                //Establecemos la variable Sitio para que se llene con lo primero
                param.ICodCatClaveCar = dtResult.Rows[0]["iCodCatClaveCar"].ToString();
                param.NombreClaveCar = dtResult.Rows[0]["Clave Cargo"].ToString();
                param.SitioDescripcion = dtResult.Rows[0]["SitioDescripcion"].ToString();

                //Para Calcular los formatos y columnas visibles
                List<string> formatStrings = new List<string>();
                formatStrings.Add("");
                formatStrings.Add("");
                formatStrings.Add("");
                for (int i = 3; i <= dtResult.Columns.Count; i++)
                {
                    formatStrings.Add("{0:c}");
                }

                List<int> boundFields = new List<int>();
                for (int i = 3; i <= dtResult.Columns.Count - 1; i++)
                {
                    boundFields.Add(i);
                }

                //Reporte    
                GridView gvRepMatPorSitioClaveCar = DTIChartsAndControls.GridView("RepMatPorSitioClaveCar", dtResult, true, "Totales",
                     formatStrings.ToArray(), "javascript.DrillDowniCodCatClaveCarClaveCargo('{0}')", new string[] { "iCodCatClaveCar" },
                     2, new int[] { 0, 1 }, boundFields.ToArray(), new int[] { 2 }, 2, false);

                gvRepMatPorSitioClaveCar.RowDataBound += (sender, e) => CambiaURLFunctionJava_RowDataBound(sender, e);
                gvRepMatPorSitioClaveCar.DataBind();
                gvRepMatPorSitioClaveCar.UseAccessibleHeader = true;
                gvRepMatPorSitioClaveCar.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepMatPorSitioClaveCar.Rows[gvRepMatPorSitioClaveCar.Rows.Count - 1];
                row.Cells[2].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvRepMatPorSitioClaveCar, "RepMatPorSitioClaveCar",
                             "Detalle de consumo del sitio " + param.SitioDescripcion + "   " + maxFechaAnio, true, "btnExportarXLS_Click",
                             "btnConsumoPorConcepto", this, false));

                //AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                //trigger.ControlID = Rep3.FindControl("btnConsumoPorConcepto").UniqueID;
                //trigger.EventName = "Click";
                //UpdAux.Triggers.Add(trigger);   

                PostBackTrigger trigger = new PostBackTrigger();
                trigger.ControlID = Rep3.FindControl("btnConsumoPorConcepto").ID;
                UpdAux.Triggers.Add(trigger);

                ScriptManager.GetCurrent(Page).RegisterPostBackControl(Rep3.FindControl("btnConsumoPorConcepto"));

                /*Seccion de grafica: Grafica Por Sitio 2 Años*/
                DataView dvDtResult = new DataView(dtResult);
                dtResult = dvDtResult.ToTable(false, new string[] { "iCodCatClaveCar", "Clave Cargo" });

                if (dtResult.Rows[dtResult.Rows.Count - 1]["Clave Cargo"].ToString() == "Totales")
                {
                    dtResult.Rows[dtResult.Rows.Count - 1].Delete();
                }

                Rep4.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafPoriCodCatClaveCarClaveCargo", "Gráfica por clave cargo   " + maxFechaAnio));

                //Para cuando uso Ajax
                ScriptManager.RegisterStartupScript(this, typeof(Page), "CambiaGraficaDrillDowniCodCatClaveCarClaveCargo",
                    javaScriptDrillDownSitioTDest(dtResult, "iCodCatClaveCar", "Clave Cargo", "IMPORTE", "Clave Cargo", false, "375"), false);

                //Seleccionar la primera clave cargo
                ScriptManager.RegisterStartupScript(this, typeof(Page), "funcionClaveCar", "$(document).ready(function(){DrillDowniCodCatClaveCarClaveCargo('" + param.ICodCatClaveCar + "');});", true);
            }
            else
            {
                param.ICodCatClaveCar = string.Empty;
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "GrafPoriCodCatClaveCarClaveCargo", "Detalle de consumo " + maxFechaAnio, "", false));
            }
        }

        private void RepMatPorEnlace2Años() 
        {
            DataTable dtResult = DSODataAccess.Execute(GetInventarioEnlaces(0));
            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                List<string> formatCol = new List<string>();
                List<int> colMostrar = new List<int>();
                formatCol.Add("");
                formatCol.Add("");
                for (int i = 2; i < dtResult.Columns.Count; i++)
                {
                    formatCol.Add("{0:0,0}");
                    colMostrar.Add(i);
                }


                //Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvRepMatPorSitioClaveCar, "RepMatPorSitioClaveCar",
                //             "Detalle de consumo del sitio " + param.SitioDescripcion + "   " + maxFechaAnio, true, "btnExportarXLS_Click",
                //             "btnConsumoPorConcepto", this, false));


                Rep5.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                    DTIChartsAndControls.GridView("RepRecurso", dtResult, true, "Totales", formatCol.ToArray(),
                    2, false), "RepInventarioEnlace_T", "Volumetría Global Enlaces Uninet", true, "btnExportarXLS_Click",
                             "btnVolumetriaGlobalEnlacesUninet", this, false));
            }            
        }

        private void RepEnlacesCapacidad()
        {
            DataTable dtResult = DSODataAccess.Execute(GetInventarioEnlaces(1));
            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                DataView dvResult = new DataView(dtResult);
                dtResult.Columns[0].ColumnName = "label";
                dtResult.Columns[1].ColumnName = "value";
                dtResult.AcceptChanges();

                dtResult.DefaultView.Sort = "value desc";
                dtResult = dtResult.DefaultView.ToTable();

                Rep6.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafPieEnlaces", "Resumen por Ancho de Banda"));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafPieEnlaces",
                         FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtResult), "GrafPieEnlaces",
                                    "", "", "", "", "bar2d", "", "", "dti", "100%", "375", true), false);
            }
        }

        #endregion Reportes


        #region Botones y funcionalidades de apoyo

        private string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper();
        }

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            carrierSelected = ddlCarrier.SelectedValue;
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            LinkButton boton = sender as LinkButton;
            param.ICodCatSitioNombre = Session["iCodCatSitioNombre"].ToString();
            if (string.IsNullOrEmpty(param.SitioDescripcion) && Session["SitioDescripcion"] != null && !string.IsNullOrEmpty(Session["SitioDescripcion"].ToString()))
            {
                param.SitioDescripcion = Session["SitioDescripcion"].ToString();
            }
            ExportXLS(".xlsx", boton.ID.Replace("btn", ""));
        }

        public string javaScriptDrillDownSitioTDest(DataTable ldt, string nomColId, string nomColumGraficar, string tituloGrafEje, string nomColMostrar, bool isSitios, string grafAltura)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\"> ");
            lsb.AppendLine("function DrillDown" + nomColId + nomColumGraficar.Replace(" ", "") + "(ID){ ");

            string nombreTitulo = string.Empty;
            foreach (DataRow ldr in ldt.Rows)
            {
                if (ldr[nomColId].ToString().ToLower() != "totales")
                {
                    lsb.Append("if(ID == '" + ldr[nomColId].ToString() + "'){");

                    DataTable dtConsulta = null;
                    if (isSitios)
                    {
                        dtConsulta = DSODataAccess.Execute(GetConsumoPorSitio2Años(Convert.ToInt32(ldr[nomColId]), 0));
                    }
                    else
                    {
                        dtConsulta = DSODataAccess.Execute(GetConsumoPorSitio2Años(Convert.ToInt32(param.ICodCatSitioNombre), Convert.ToInt32(ldr[nomColId])));
                    }

                    if (dtConsulta.Rows.Count > 0 && dtConsulta.Columns.Count > 0)
                    {
                        nombreTitulo = ldr[nomColMostrar].ToString();
                        if (string.IsNullOrEmpty(nombreTitulo) && dtConsulta.Columns.Contains(nomColMostrar))
                        {
                            nombreTitulo = dtConsulta.Rows[0][nomColMostrar].ToString();
                        }

                        DataView dvDataSource = new DataView(dtConsulta);
                        dtConsulta = dvDataSource.ToTable(false,
                            new string[] { "Mes", "AÑO ANTERIOR", "AÑO ACTUAL" });
                        dtConsulta.AcceptChanges();
                    }
                    string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dtConsulta));
                    lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(dtConsulta),
                                   "GrafPor" + nomColId + nomColumGraficar.Replace(" ", ""), "", nombreTitulo, "",
                                   "MESES", "msline", "$", "", "dti", "98%", grafAltura, false));
                    lsb.Append("}");
                }
            }
            lsb.AppendLine("}");
            lsb.AppendLine("</script>");
            return lsb.ToString();
        }

        public void CambiaURLFunctionJava_RowDataBound(object sender, GridViewRowEventArgs e)
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

        #endregion Botones y funcionalidades de apoyo


        #region Exportacion

        public void ExportXLS(string tipoExtensionArchivo, string reportExportar)
        {
            CrearXLS(tipoExtensionArchivo, reportExportar);
        }

        protected void CrearXLS(string lsExt, string reportExportar)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                #region Exportar reportes con tabla y grafica

                if (reportExportar == "Aun sin definir ningun reporte")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashboardSiana\ReporteTablaYGrafico" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (reportExportar == "")
                    {
                    }
                    else if (true)
                    { }
                }

                #endregion Exportar reportes con tabla y grafica

                #region Exportar Reportes solo con tabla

                else if (reportExportar == "ConsumoPorConcepto")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashboardSiana\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (reportExportar == "ConsumoPorConcepto")
                    {
                        DataTable RepConsumoPorConcepto = DSODataAccess.Execute(GetConsumoPorSitioClaveCarMatAnioActual());
                        if (RepConsumoPorConcepto.Rows.Count > 0 && RepConsumoPorConcepto.Columns.Count > 0)
                        {
                            param.SitioDescripcion = RepConsumoPorConcepto.Rows[0][0].ToString();
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por Concepto " + maxFechaAnio + " del Sitio: " + param.SitioDescripcion);

                            DataView dvRepConsumoPorConcepto = new DataView(RepConsumoPorConcepto);
                            RepConsumoPorConcepto = dvRepConsumoPorConcepto.ToTable(false,
                                new string[] { "Clave Cargo", "Total Anual", "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" });

                            RepConsumoPorConcepto.Columns["Clave Cargo"].ColumnName = "Concepto";
                            RepConsumoPorConcepto.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsumoPorConcepto, 0, "Totales"), "Reporte", "Tabla");
                        }
                        else
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por Concepto " + maxFechaAnio);
                        }
                    }
                    else if (true)
                    { }
                }
                else if (reportExportar == "ConsumoPorSitio")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashboardSiana\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por Sitio ");
                    DataTable dtResult = DSODataAccess.Execute(GetConsumoPorSitioMatAnioActual());

                    if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
                    {
                        if (dtResult.Columns.Contains("ImporteMoneda"))
                            dtResult.Columns.Remove("ImporteMoneda");

                        //Establecemos la variable Sitio para que se llene con lo primero
                        param.ICodCatSitioNombre = dtResult.Rows[0]["iCodCatSitioNombre"].ToString();
                        param.SitioDescripcion = dtResult.Rows[0]["SitioDescripcion"].ToString();
                        Session["iCodCatSitioNombre"] = param.ICodCatSitioNombre;
                        Session["SitioDescripcion"] = param.SitioDescripcion;

                        //Para Calcular los formatos y columnas visibles
                        List<string> formatStrings = new List<string>();
                        formatStrings.Add("");
                        formatStrings.Add("");
                        for (int i = 2; i <= dtResult.Columns.Count; i++)
                        {
                            formatStrings.Add("{0:c}");
                        }

                        List<int> boundFields = new List<int>();
                        for (int i = 2; i <= dtResult.Columns.Count; i++)
                        {
                            boundFields.Add(i);
                        }

                        dtResult.Columns["SitioDescripcion"].ColumnName = "Sitio";

                        //Reporte    
                        GridView gvRepMatPorSitioTDest = DTIChartsAndControls.GridView("RepMatPorSitioTDest", dtResult, true, "Totales",
                             formatStrings.ToArray(), "javascript.ActualizarRepSitio('{0}');", new string[] { "iCodCatSitioNombre" }, 1, new int[] { 0 },
                             boundFields.ToArray(), new int[] { 1 }, 2, false);

                        gvRepMatPorSitioTDest.RowDataBound += (sender, e) => CambiaURLFunctionJava_RowDataBound(sender, e);
                        gvRepMatPorSitioTDest.DataBind();
                        gvRepMatPorSitioTDest.UseAccessibleHeader = true;
                        gvRepMatPorSitioTDest.HeaderRow.TableSection = TableRowSection.TableHeader;

                        GridViewRow row = gvRepMatPorSitioTDest.Rows[gvRepMatPorSitioTDest.Rows.Count - 1];
                        row.Cells[1].Text = "Totales";
                        for (int tot = 0; tot < row.Cells.Count; tot++)
                        {
                            row.Cells[tot].CssClass = "lastRowTable";
                        }

                        if (dtResult.Columns.Contains("iCodCatSitioNombre"))
                        {
                            dtResult.Columns.Remove("iCodCatSitioNombre");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtResult, "Reporte", "Tabla");
                    }
                }
                else if (reportExportar == "VolumetriaGlobalEnlacesUninet")
                {

                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashboardSiana\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Volumetría Global Enlaces Uninet");

                    DataTable dtResult = DSODataAccess.Execute(GetInventarioEnlaces(0));
                    if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
                    {
                        List<string> formatCol = new List<string>();
                        List<int> colMostrar = new List<int>();
                        formatCol.Add("");
                        formatCol.Add("");
                        for (int i = 2; i < dtResult.Columns.Count; i++)
                        {
                            formatCol.Add("{0:0,0}");
                            colMostrar.Add(i);
                        }

                    }
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtResult, "Reporte", "Tabla");
                }
                else if (reportExportar == "Consumo")
                {
                }
                else if (reportExportar == "OtraOpcion")
                {

                }

                #endregion Exportar Reportes solo con tabla

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;

                lExcel.SalvarComo();

                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes" + "_" + reportExportar + "_");
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
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion
    }

    public class SitioCveCar
    {
        public string ICodCatSitioNombre { get; set; }
        public string ICodCatClaveCar { get; set; }
        public string SitioDescripcion { get; set; }
        public string NombreClaveCar { get; set; }
    }
}
