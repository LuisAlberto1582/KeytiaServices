using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Text;
using System.Data;
using KeytiaServiceBL;
using AjaxControlToolkit;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    public partial class PresupuestoPorCategoria : System.Web.UI.Page
    {
        DataTable dtCatMesConsVsPptoSinCarrier = new DataTable();
        DataTable dtCatConsVsPptoConCarrier = new DataTable();
        DataTable dtCatMesConsVsPptoConCarrier = new DataTable();

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                Session["FechaInicio"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
                Session["FechaFin"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).ToString("yyyy-MM-dd");

                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                if (!Page.IsPostBack)
                {
                    DashboardTIMDefault();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void DashboardTIMDefault()
        {
            RepPptoVsConsumoCategoriasSinCarrier();

            RePptoVsConsumoCategoriaConCarrier();
        }

        #region Reportes

        private void RepPptoVsConsumoCategoriasSinCarrier()
        {
            //Tablas InfoEstaticaPorCarrier
            var dtResultCategorias = DSODataAccess.Execute("EXEC [TIMPptoVsConsumoCategoria] @Esquema = '" + DSODataContext.Schema + "'");
            dtCatMesConsVsPptoSinCarrier = DSODataAccess.Execute("EXEC [TIMPptoVsConsumoCategoriaMes] @Esquema = '" + DSODataContext.Schema + "'");

            #region Grid

            if (dtResultCategorias != null && dtResultCategorias.Columns.Count > 0)
            {
                DataTable dtCloned = dtResultCategorias.Clone();
                dtCloned.Columns[5].DataType = typeof(string);
                foreach (DataRow row in dtResultCategorias.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                dtResultCategorias = dtCloned;
                DataView dvldt = new DataView(dtResultCategorias);
                dtResultCategorias = dvldt.ToTable(false, new string[] { "TDest", "NombreTDest", "Presupuesto", "Consumo", "Variacion",
                    "AnioPresupuesto" });
                dtResultCategorias.AcceptChanges();
            }

            if (dtCatMesConsVsPptoSinCarrier != null && dtCatMesConsVsPptoSinCarrier.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtCatMesConsVsPptoSinCarrier);
                dtCatMesConsVsPptoSinCarrier = dvldt.ToTable(false, new string[] { "TDest", "NombreTDest", "NombreMesNatural", "Presupuesto", "Consumo",
                    "Variacion", "AnioPresupuesto", "FechaIntPublica" });
                dtCatMesConsVsPptoSinCarrier.AcceptChanges();

                dtCatMesConsVsPptoSinCarrier.Columns["NombreMesNatural"].ColumnName = "Mes";
                dtCatMesConsVsPptoSinCarrier.Columns["Variacion"].ColumnName = "% Variación";

            }

            gridRepCategoriasSinCarrier.DataSource = dtResultCategorias;
            gridRepCategoriasSinCarrier.DataBind();
            gridRepCategoriasSinCarrier.UseAccessibleHeader = true;
            gridRepCategoriasSinCarrier.HeaderRow.TableSection = TableRowSection.TableHeader;
            ContenedorPorCategoriaSinCarrier.Visible = true;

            Rep1.Controls.Add(DTIChartsAndControls.TituloYPestañasRep1Nvl(ContenedorPorCategoriaSinCarrier, "RepCategoriasSinCarrier", "Presupuestos por Concepto",
                string.Empty, 4, FCGpoGraf.Matricial, "" ,false));

            #endregion Grid

            #region Grafica

            if (dtResultCategorias.Rows.Count > 0 && dtResultCategorias.Columns.Count > 0)
            {
                DataView dvResultCategorias = new DataView(dtResultCategorias);
                dtResultCategorias = dvResultCategorias.ToTable(false,
                    new string[] { "NombreTDest", "Presupuesto", "Consumo" });
                dtResultCategorias.Columns["NombreTDest"].ColumnName = "Concepto";
                dtResultCategorias.AcceptChanges();

                string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dtResultCategorias));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepCategoriasSinCarrier",
                    FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(dtResultCategorias), "RepCategoriasSinCarrier",
                    "", "", "", "IMPORTE", 4, FCGpoGraf.Matricial), false);
            }
            #endregion Grafica

            #region Grafica dinamica por concepto y mes.  --> Presupuesto vs Consumo

            if (dtResultCategorias.Rows.Count > 0 && dtResultCategorias.Columns.Count > 0)
            {
                DataView dvDtResult = new DataView(dtResultCategorias);
                var dtResult = dvDtResult.ToTable(false, new string[] { "Concepto" });

                Rep2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafSinCarrierNombreTDestMeses", "Gráfica desglose por mes"));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "CambiaGraficaDrillDownSinCarrierTDestMeses",
                    javaScriptDrillDownSinCarrierCategoriaMeses(dtResult, "Concepto", "IMPORTE", "Concepto", "SinCarrierNombreTDestMeses", "CONTROL PPTO.", "GrafSinCarrierNombreTDestMeses"), false);
            }

            #endregion Grafica dinamica por concepto y mes.
        }
        
        protected void OnRowDataBoundCatSinCarrier(object sender, GridViewRowEventArgs e)
        {
            if (dtCatMesConsVsPptoSinCarrier != null && dtCatMesConsVsPptoSinCarrier.Rows.Count > 0)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var mesesCategoria = from row in dtCatMesConsVsPptoSinCarrier.AsEnumerable()
                                         where row.Field<int>("TDest") == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "TDest"))
                                         select row;

                    Label lbl = ((Label)e.Row.FindControl("lblVariacion"));
                    if (lbl != null)
                    {
                        lbl.CssClass = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Variacion")) > 100 ? "letraRoja" : "letraVerde";
                    }

                    if (mesesCategoria != null && mesesCategoria.Count() > 0)
                    {
                        var dtResultCategoria = mesesCategoria.CopyToDataTable();
                        var panelResumen = (e.Row.FindControl("pnlDetalleMes") as Panel);

                        GridView grid = DTIChartsAndControls.GridView("MesesCategoria", dtResultCategoria, false, "",
                                new string[] { "", "", "", "{0:c}", "{0:c}", "{0:n}", "", "" }, "", new string[] { }, 1,
                                new int[] { 0, 1, 6, 7 }, new int[] { 2, 3, 4, 5 }, new int[] { }, 2, false);

                        grid.RowDataBound += (sender2, e2) => CambiaColorLetra_RowDataBound(sender2, e2, "% Variación");
                        grid.DataBind();
                        grid.UseAccessibleHeader = true;
                        grid.HeaderRow.TableSection = TableRowSection.TableHeader;
                        panelResumen.Controls.Add(grid);
                    }
                }
            }
        }

        public string javaScriptDrillDownSinCarrierCategoriaMeses(DataTable ldt, string nomColId, string tituloGrafEje, string nomColMostrar, string nomFuncionJS, string tituloGraf, string idContenedorGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\">");
            lsb.AppendLine("function DrillDown" + nomFuncionJS + "(Concepto){");
            string anioPpto = string.Empty;
            foreach (DataRow ldr in ldt.Rows)
            {
                if (ldr[nomColId].ToString().ToLower() != "totales")
                {
                    lsb.Append("if(Concepto == '" + ldr[nomColId].ToString() + "'){");

                    if (dtCatMesConsVsPptoSinCarrier != null)
                    {
                        var consulta = from row in dtCatMesConsVsPptoSinCarrier.AsEnumerable()
                                       where row.Field<string>("NombreTDest") == ldr[nomColId].ToString()
                                       select row;

                        if (consulta != null && consulta.Count() > 0)
                        {
                            var dtConsulta = consulta.CopyToDataTable();
                            anioPpto = dtConsulta.Rows[0]["AnioPresupuesto"].ToString();
                            DataView dvDataSource = new DataView(dtConsulta);
                            dtConsulta = dvDataSource.ToTable(false, new string[] { "Mes", "Presupuesto", "Consumo" });
                            dtConsulta.AcceptChanges();

                            string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(
                                                          FCAndControls.ConvertDataTabletoDataTableArray(dtConsulta));

                            lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(dtConsulta), idContenedorGrafica,
                                          tituloGraf + " " + ldr[nomColMostrar].ToString().Trim() + " " + anioPpto, "",
                                          "MESES", tituloGrafEje, "mscolumn2d", "$", "", "dti", "98%", "385", false));
                        }
                    }
                    lsb.AppendLine("}");
                }
            }
            lsb.AppendLine("}");
            lsb.AppendLine("</script>");
            return lsb.ToString();
        }





        private void RePptoVsConsumoCategoriaConCarrier()
        {
            //Tablas InfoEstaticaPorCarrier
            dtCatConsVsPptoConCarrier = DSODataAccess.Execute("EXEC [TIMPptoVsConsumoCarrierCategoria] @Esquema = '" + DSODataContext.Schema + "'");
            dtCatMesConsVsPptoConCarrier = DSODataAccess.Execute("EXEC [TIMPptoVsConsumoCarrierCategoriaMes] @Esquema = '" + DSODataContext.Schema + "'");

            if (dtCatConsVsPptoConCarrier.Rows.Count > 0 && dtCatConsVsPptoConCarrier.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtCatConsVsPptoConCarrier);
                dtCatConsVsPptoConCarrier = dvldt.ToTable(false, new string[] { "Carrier", "NombreCarrier", "TDest", "NombreTDest", "Presupuesto", 
                    "Consumo", "Variacion", "AnioPresupuesto", "UltimoMesCargado", "NombreUltimoMesCargado" });
                dtCatConsVsPptoConCarrier.AcceptChanges();
            }

            if (dtCatMesConsVsPptoConCarrier != null && dtCatMesConsVsPptoConCarrier.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtCatMesConsVsPptoConCarrier);
                dtCatMesConsVsPptoConCarrier = dvldt.ToTable(false, new string[] { "Carrier", "NombreCarrier", "TDest", "NombreTDest", "NombreMesNatural", 
                    "Presupuesto", "Consumo", "Variacion", "AnioPresupuesto", "FechaIntPublica" });
                dtCatMesConsVsPptoConCarrier.AcceptChanges();

                dtCatMesConsVsPptoConCarrier.Columns["NombreMesNatural"].ColumnName = "Mes";
                dtCatMesConsVsPptoConCarrier.Columns["Variacion"].ColumnName = "% Variación";
            }

            //Primer agrupado por Carrier
            var dtGpoPorCarrier = (from row in dtCatConsVsPptoConCarrier.AsEnumerable()
                                   select new { Carrier = row.Field<Int32>("Carrier"), NombreCarrier = row.Field<string>("NombreCarrier") }).Distinct();

            gridRepCategoriaConCarrier.DataSource = dtGpoPorCarrier;
            gridRepCategoriaConCarrier.DataBind();
            gridRepCategoriaConCarrier.UseAccessibleHeader = true;
            gridRepCategoriaConCarrier.HeaderRow.TableSection = TableRowSection.TableHeader;
            ContenedorPorCategoriaConCarrier.Visible = true;
            Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(ContenedorPorCategoriaConCarrier, "RepCategoriaConCarrier", "Presupuestos por Carrier - Concepto", "", false));
            
            //Graficas dinamicas
            #region Grafica dinamica por Carrier - Concepto.  --> Presupuesto vs Consumo //DrillDownConCarrierNombreTDest

            if (dtCatConsVsPptoConCarrier.Rows.Count > 0 && dtCatConsVsPptoConCarrier.Columns.Count > 0)
            {
                DataView dvDtResult = new DataView(dtCatConsVsPptoConCarrier);
                var dtResult = dvDtResult.ToTable(true, new string[] { "NombreCarrier" });

                Rep4.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafConCarrierNombreTDest", "Gráfica desglose por Concepto"));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "CambiaGraficaDrillDownConCarrierTDest",
                    javaScriptDrillDownConCarrierCategoria(dtResult, "NombreCarrier", "IMPORTE", "ConCarrierNombreTDest", "CONTROL PPTO. CONCEPTOS", "GrafConCarrierNombreTDest"), false);
            }

            if (dtCatMesConsVsPptoConCarrier.Rows.Count > 0 && dtCatMesConsVsPptoConCarrier.Columns.Count > 0)
            {
                DataView dvDtResult = new DataView(dtCatConsVsPptoConCarrier);
                var dtResult = dvDtResult.ToTable(true, new string[] { "NombreCarrier", "NombreTDest" });

                Rep5.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafConCarrierNombreTDestMes", "Gráfica desglose por mes"));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "CambiaGraficaDrillDownConCarrierTDestMes",
                    javaScriptDrillDownConCarrierCategoriaMeses(dtResult, "IMPORTE", "NombreTDest", "ConCarrierNombreTDestMeses", "CONTROL PPTO.", "GrafConCarrierNombreTDestMes"), false);
            }
            #endregion

        }

        protected void OnRowDataBoundCatConCarrier(object sender, GridViewRowEventArgs e)
        {
            if (dtCatConsVsPptoConCarrier != null && dtCatConsVsPptoConCarrier.Rows.Count > 0)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var categoriasCarrier = from row in dtCatConsVsPptoConCarrier.AsEnumerable()
                                            where row.Field<int>("Carrier") == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Carrier"))
                                            select row;

                    if (categoriasCarrier != null && categoriasCarrier.Count() > 0)
                    {
                        DataTable dtResultCategoria = categoriasCarrier.CopyToDataTable();
                        var panelCategorias = (e.Row.FindControl("pnlDetallCategorias") as Panel);
                        var gridCategorias = panelCategorias.FindControl("gridRepCategoriasConCarrier") as GridView;
                        gridCategorias.DataSource = dtResultCategoria;
                        gridCategorias.DataBind();
                        gridCategorias.UseAccessibleHeader = true;
                        gridCategorias.HeaderRow.TableSection = TableRowSection.TableHeader;
                    }
                }
            }
        }

        protected void OnRowDataBoundCatConCarrierMes(object sender, GridViewRowEventArgs e)
        {
            if (dtCatConsVsPptoConCarrier != null && dtCatConsVsPptoConCarrier.Rows.Count > 0)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var mesesCategoriasCarrier = from row in dtCatMesConsVsPptoConCarrier.AsEnumerable()
                                                 where row.Field<int>("Carrier") == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Carrier")) //Posicion del Carrier (0)
                                                     && row.Field<int>("TDest") == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "TDest"))
                                                 select row;

                    Label lbl = ((Label)e.Row.FindControl("lblVariacion"));
                    if (lbl != null)
                    {
                        lbl.CssClass = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Variacion")) > 100 ? "letraRoja" : "letraVerde";
                    }

                    if (mesesCategoriasCarrier != null && mesesCategoriasCarrier.Count() > 0)
                    {
                        var dtResultMesCategoria = mesesCategoriasCarrier.CopyToDataTable();
                        var panelDetalleMes = (e.Row.FindControl("pnlDetalleMes") as Panel);

                        GridView grid = DTIChartsAndControls.GridView("MesesCategoria", dtResultMesCategoria, false, "",
                                new string[] { "", "", "", "", "", "{0:c}", "{0:c}", "{0:n}", "", "" }, "",
                                new string[] { }, 1, new int[] { 0, 1, 2, 3, 8, 9 }, new int[] { 4, 5, 6, 7 }, new int[] { }, 2, false);

                        grid.RowDataBound += (sender2, e2) => CambiaColorLetra_RowDataBound(sender2, e2, "% Variación");
                        grid.DataBind();
                        grid.UseAccessibleHeader = true;
                        grid.HeaderRow.TableSection = TableRowSection.TableHeader;
                        panelDetalleMes.Controls.Add(grid);
                    }
                }
            }
        }

        public string javaScriptDrillDownConCarrierCategoria(DataTable ldt, string nomColId, string tituloGrafEje, string nomFuncionJS, string tituloGraf, string idContenedorGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\">");
            lsb.AppendLine("function DrillDown" + nomFuncionJS + "(Concepto){");
            foreach (DataRow ldr in ldt.Rows)
            {
                if (ldr[nomColId].ToString().ToLower() != "totales")
                {
                    lsb.Append("if(Concepto == '" + ldr[nomColId].ToString() + "'){");

                    if (dtCatConsVsPptoConCarrier != null)
                    {
                        var consulta = from row in dtCatConsVsPptoConCarrier.AsEnumerable()
                                       where row.Field<string>("NombreCarrier") == ldr[nomColId].ToString()
                                       select row;

                        if (consulta != null && consulta.Count() > 0)
                        {
                            var dtConsulta = consulta.CopyToDataTable();
                            DataView dvDataSource = new DataView(dtConsulta);
                            dtConsulta = dvDataSource.ToTable(false, new string[] { "NombreTDest", "Presupuesto", "Consumo" });
                            dtConsulta.AcceptChanges();

                            string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(
                                                          FCAndControls.ConvertDataTabletoDataTableArray(dtConsulta));

                            lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(dtConsulta), idContenedorGrafica,
                                         tituloGraf + " " + ldr[nomColId].ToString().Trim(), "", "", tituloGrafEje, "msbar2d", "$", "", "dti", "98%", "385", false));                           
                        }
                    }
                    lsb.Append("}");
                }
            }
            lsb.AppendLine("}");
            lsb.AppendLine("</script>");
            return lsb.ToString();
        }

        public string javaScriptDrillDownConCarrierCategoriaMeses(DataTable ldt, string tituloGrafEje, string nomColMostrar, string nomFuncionJS, string tituloGraf, string idContenedorGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\">");
            lsb.AppendLine("function DrillDown" + nomFuncionJS + "(Concepto){");
            foreach (DataRow ldr in ldt.Rows)
            {
                if (ldr["NombreCarrier"].ToString().ToLower() != "totales")
                {
                    lsb.Append("if(Concepto == '" + ldr["NombreCarrier"].ToString() + "-" + ldr["NombreTDest"].ToString() + "'){");

                    if (dtCatMesConsVsPptoSinCarrier != null)
                    {
                        var consulta = from row in dtCatMesConsVsPptoConCarrier.AsEnumerable()
                                       where row.Field<string>("NombreCarrier") == ldr["NombreCarrier"].ToString() &&
                                             row.Field<string>("NombreTDest") == ldr["NombreTDest"].ToString()
                                       select row;

                        if (consulta != null && consulta.Count() > 0)
                        {
                            var dtConsulta = consulta.CopyToDataTable();
                            DataView dvDataSource = new DataView(dtConsulta);
                            dtConsulta = dvDataSource.ToTable(false, new string[] { "Mes", "Presupuesto", "Consumo" });
                            dtConsulta.AcceptChanges();

                            string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(
                                                          FCAndControls.ConvertDataTabletoDataTableArray(dtConsulta));

                            lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(dtConsulta), idContenedorGrafica,
                                          tituloGraf + " " + ldr["NombreCarrier"].ToString().Trim() + ": " + ldr["NombreTDest"].ToString().Trim(),
                                          "", "", tituloGrafEje, "mscolumn2d", "$", "", "dti", "98%", "385", false));
                        }
                    }
                    lsb.Append("}");
                }
            }
            lsb.AppendLine("}");
            lsb.AppendLine("</script>");
            return lsb.ToString();
        }

        public void CambiaColorLetra_RowDataBound(object sender, GridViewRowEventArgs e, string nombreColumnaValor)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var data = e.Row.DataItem as DataRowView;

                if (data.DataView.Table.Columns[nombreColumnaValor] != null)
                {
                    int idx = data.Row.Table.Columns[nombreColumnaValor].Ordinal;
                    e.Row.Cells[idx].CssClass = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, nombreColumnaValor)) > 100 ? "letraRoja" : "letraVerde";
                }
            }
        }

        #endregion
    }
}
