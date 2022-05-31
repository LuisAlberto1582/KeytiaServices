using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;
using DSOControls2008;
using KeytiaWeb.UserInterface.DashboardLT;
using AjaxControlToolkit;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    public partial class PresupuestoGlobal : System.Web.UI.Page
    {
        DataTable dtCarrierMesConsVsPpto = new DataTable();

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
                //Session["FechaInicio"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
                //Session["FechaFin"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).ToString("yyyy-MM-dd");

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
            ContenedorPorCarrier.Visible = false;
            RepPptoVsConsumoCarries();
        }

        #region Reportes       

        private void RepPptoVsConsumoCarries()
        {
            //Tablas InfoEstaticaPorCarrier
            var dtResultCarries = DSODataAccess.Execute("EXEC [TIMPptoVsConsumoCarries] @Esquema = '" + DSODataContext.Schema + "'");
            dtCarrierMesConsVsPpto = DSODataAccess.Execute("EXEC [TIMPptoVsConsumoCarriesMes] @Esquema = '" + DSODataContext.Schema + "'");

            if (dtResultCarries.Rows.Count > 0 && dtResultCarries.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtResultCarries);
                dtResultCarries = dvldt.ToTable(false, new string[] { "Empresa", "Carrier", "NombreCarrier", "Presupuesto", "Consumo", "Variacion",
                    "AnioPresupuesto", "UltimoMesCargado", "NombreUltimoMesCargado" });
                dtResultCarries.AcceptChanges();
            }

            if (dtCarrierMesConsVsPpto != null && dtCarrierMesConsVsPpto.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtCarrierMesConsVsPpto);
                dtCarrierMesConsVsPpto = dvldt.ToTable(false, new string[] { "Empresa", "Carrier", "NombreCarrier", "NombreMesNatural", "Presupuesto", "Consumo", "Variacion",
                    "AnioPresupuesto", "FechaIntPublica", "MesNatural", "MesFiscal" });
                dtCarrierMesConsVsPpto.AcceptChanges();

                dtCarrierMesConsVsPpto.Columns["NombreMesNatural"].ColumnName = "Mes";
                dtCarrierMesConsVsPpto.Columns["Variacion"].ColumnName = "% Variación";
            }
            
            gridRepCarries.DataSource = dtResultCarries;
            gridRepCarries.DataBind();
            gridRepCarries.UseAccessibleHeader = true;
            gridRepCarries.HeaderRow.TableSection = TableRowSection.TableHeader;
            ContenedorPorCarrier.Visible = true;
            Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(ContenedorPorCarrier, "gvRepPorCarrier", "Presupuestos por Carrier", "", false));              

            //Grafica Dinamica por Carrier, Meses, --> Presupuesto vs Consumo
            DataView dvDtResult = new DataView(dtResultCarries);
            var dtResult = dvDtResult.ToTable(false, new string[] { "Empresa", "Carrier", "NombreCarrier"});

            Rep4.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafPorNombreCarrierMeses", "Gráfica desglose por mes"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "CambiaGraficaDrillDownCarrierMeses",
                javaScriptDrillDownCarrierMeses(dtResult, "NombreCarrier", "IMPORTE", "NombreCarrier", "CarrierMeses", "CONTROL PPTO."), false);

        }
        
        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (dtCarrierMesConsVsPpto != null && dtCarrierMesConsVsPpto.Rows.Count > 0)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var mesesCarrier = from row in dtCarrierMesConsVsPpto.AsEnumerable()
                                       where row.Field<int>("Carrier") == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Carrier")) &&
                                             row.Field<int>("Empresa") == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Empresa")) 
                                       select row;


                    Label lbl = ((Label)e.Row.FindControl("lblVariacion"));
                    if (lbl != null)
                    {
                        lbl.CssClass = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Variacion")) > 100 ? "letraRoja" : "letraVerde";
                    }

                    if (mesesCarrier != null && mesesCarrier.Count() > 0)
                    {
                        var dtResultCarrier = mesesCarrier.CopyToDataTable();
                        var panelResumen = (e.Row.FindControl("pnlDetalleMes") as Panel);
                        GridView grid = DTIChartsAndControls.GridView("Meses", dtResultCarrier, false, "",
                                new string[] { "", "", "", "", "{0:c}", "{0:c}", "{0:n}", "", "", "", "" }, "", new string[] { }, 2,
                                new int[] { 0, 1, 2, 7, 8, 9, 10 }, new int[] { 3, 4, 5, 6 }, new int[] { }, 2, false);

                        grid.RowDataBound += (sender2, e2) => CambiaColorLetra_RowDataBound(sender2, e2, "% Variación");
                        grid.DataBind();
                        grid.UseAccessibleHeader = true;
                        grid.HeaderRow.TableSection = TableRowSection.TableHeader;
                        panelResumen.Controls.Add(grid);
                    }
                }
            }
        }

        public string javaScriptDrillDownCarrierMeses(DataTable ldt, string nomColId, string tituloGrafEje, string nomColMostrar, string nomFuncionJS, string tituloGraf)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\"> ");
            lsb.AppendLine("function DrillDown" + nomFuncionJS + "(Concepto){");
            string anioPpto = string.Empty;
            foreach (DataRow ldr in ldt.Rows)
            {
                if (ldr[nomColId].ToString().ToLower() != "totales")
                {
                    lsb.Append("if(Concepto == '" + ldr[nomColId].ToString() + "'){");

                    if (dtCarrierMesConsVsPpto != null)
                    {
                        var consulta = from row in dtCarrierMesConsVsPpto.AsEnumerable()
                                       where row.Field<int>("Carrier") == Convert.ToInt32(ldr["Carrier"]) &&
                                             row.Field<int>("Empresa") == Convert.ToInt32(ldr["Empresa"])
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

                            lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(dtConsulta),
                                          "GrafPor" + nomColId + "Meses", tituloGraf + " " + ldr[nomColMostrar].ToString().Trim() + " " + anioPpto, "",
                                          "MES", tituloGrafEje, "mscolumn2d", "$", "", "dti", "98%", "385", false));
                        }
                    }
                    lsb.Append("}");
                }
            }
            lsb.Append("}");
            lsb.Append("</script>");
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
