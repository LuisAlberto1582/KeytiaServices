using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardFC.Reportes;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    public partial class ConsumoGeneralMoviles
    {
        string maxFechaAnio = DateTime.Now.Year.ToString();
        public void RepAcumuladoCarrier(Control Content, GroupBy agrupado)
        {
            DataTable dtConsumoCarrier = DSODataAccess.Execute(GetConsumoPorXAnioActual());

            string tituloGrafica = "Por " + agrupado.ToString() + " acumulado " + maxFechaAnio;

            DataView dvldt = new DataView(dtConsumoCarrier);
            dtConsumoCarrier = DTIChartsAndControls.ordenaTabla(dtConsumoCarrier, "IMPORTE Desc");
            dtConsumoCarrier.AcceptChanges();

            #region Grafica Carrier Acumulado
            dtConsumoCarrier = dvldt.ToTable(false, new string[] { "Nom Carrier", "Costo" });
            if (dtConsumoCarrier.Rows.Count > 0)
            {
                if (dtConsumoCarrier.Rows[dtConsumoCarrier.Rows.Count - 1]["Nom Carrier"].ToString() == "Totales")
                {
                    dtConsumoCarrier.Rows[dtConsumoCarrier.Rows.Count - 1].Delete();
                }
            }
            

            dtConsumoCarrier.Columns["Nom Carrier"].ColumnName = "Nombre Carrier";
            dtConsumoCarrier.Columns["Costo"].ColumnName = "Costo";
            TabPanel3Dos.Controls.Add(
             DTIChartsAndControls.TituloYPestañasRep1Nvl(
                             DTIChartsAndControls.GridView("IdCatalogoFour", dtConsumoCarrier, false, "",
                             new string[] { "", "{0:c}", "" }),
                              "graficaCarrrier", "", Request.Path + "?Nav=HistoricoN1", 0, FCGpoGraf.Doughnut2d)
             );
            #endregion

            #region Grafica Carrier Acumulado
            dtConsumoCarrier.Columns["Nombre Carrier"].ColumnName = "label";
            dtConsumoCarrier.Columns["Costo"].ColumnName = "value";
            dtConsumoCarrier.AcceptChanges();

            string idContener = "RepCarrrierTabPanel3Uno";

            TabPanel3Uno.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica(idContener, tituloGrafica));
            Page.ClientScript.RegisterStartupScript(this.GetType(), idContener,
                     FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtConsumoCarrier), idContener,
                                "", "", "label", "value", "doughnut2d", "", "", "dti", "100%", "300", true), false);
            #endregion

        }

        private void RepConsumoHistorico(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = ReportesTelFijaMovil.ConsultaRepHistoricoAnioActualVsAnteriorMoviles(carrierSelected, Session["iCodUsuario"].ToString());
            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "Total", "CantidadLineas" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["CantidadLineas"].ColumnName = "Cantidad líneas";
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepHistGrid", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "" }),
                               "RepTabHistMov1Pnl", tituloGrid, Request.Path + "?Nav=HistoricoN1", pestaniaActiva, FCGpoGraf.ColsCombinada)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Total", "Cantidad líneas" });
                if(ldt.Rows.Count > 0)
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";

                ldt.AcceptChanges();
                DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(ldt);
                string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(arrColumns);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHistMov1Pnl",
                    FCAndControls.GraficaCombinada(DataSourceArrayJSon,
                    new string[] { "Mes", "Total", "Cantidad líneas" }, "RepTabHistMov1Pnl", "", "", "Mes", "Importe", "Cant. líneas",
                    pestaniaActiva, FCTipoEjeSecundario.line, FCGpoGraf.ColsCombinada, "", "", "dti", "100%", "300"), false);
            }
            #endregion Grafica
        }

        private void RepLCarriersMoviles(Control contenedor)
        {
            List<InfoCarrierGlobal> dtCarriers = new List<InfoCarrierGlobal>();
            List<InfoCarrierGlobal> dtEmpres = new List<InfoCarrierGlobal>();
            dtCarriers = GetCarriersTIM(0, 0, "ConsumoGeneral");
            dtCarriers.GroupBy(g => new { g.Empre, g.EmpreDesc }).Select(x => x.First()).ToList().ForEach(n => dtEmpres.Add(new InfoCarrierGlobal() { Empre = n.Empre, EmpreDesc = n.EmpreDesc }));
            if (empreSelected == "-1" && dtEmpres.Count == 1)
            { empreSelected = dtEmpres.First().Empre.ToString(); }
            DataTable dtResult = ConvertToDataTable(dtCarriers.Where(x => x.Empre.ToString().Equals(empreSelected)).ToList());
            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtResult);
                dtResult = dvldt.ToTable(false, new string[] { "Empre", "iCodCatalogo", "vchdescripcion" });
                dtResult.Columns["vchDescripcion"].ColumnName = "Carriers";
                dtResult.AcceptChanges();

                DataRow rowGlobal = dtResult.NewRow();
                if (carrierSelected == "-1" && dtEmpres.Count > 1)
                {
                    rowGlobal["Empre"] = -1;
                    rowGlobal["iCodCatalogo"] = -1;
                    rowGlobal["Carriers"] = "Volver";
                    dtResult.Rows.InsertAt(rowGlobal, 0);
                }
                else if (carrierSelected != "-1" && dtCarriers.Where(x => x.Empre.ToString() == empreSelected).Count() > 1) //tiene mas de un carrier en esa empresa.
                {
                    rowGlobal["Empre"] = empreSelected;
                    rowGlobal["iCodCatalogo"] = -1;
                    rowGlobal["Carriers"] = "Volver";
                    dtResult.Rows.InsertAt(rowGlobal, 0);
                }
                else if (carrierSelected != "-1" && dtCarriers.Where(x => x.Empre.ToString() == empreSelected).Count() == 1)
                {
                    rowGlobal["Empre"] = -1;
                    rowGlobal["iCodCatalogo"] = -1;
                    rowGlobal["Carriers"] = "Volver";
                    dtResult.Rows.InsertAt(rowGlobal, 0);
                }

                GridView gvResultCarrier = DTIChartsAndControls.GridView("RepCarriers", dtResult, false, "",
                    new string[] { "", "", "" }, Request.Path + "?Empre={0}&Carrier={1}", new string[] { "Empre", "iCodCatalogo" },
                    0, new int[] { 0, 1 }, new int[] { }, new int[] { 2 }, 2, false);

                gvResultCarrier.RowDataBound += (sender, e) => AgregarHiperlink_RowDataBound(sender, e, "~/UserInterface/DashboardFC/TIM/images/TIM", 2);
                gvResultCarrier.DataBind();
                gvResultCarrier.UseAccessibleHeader = true;
                gvResultCarrier.HeaderRow.TableSection = TableRowSection.TableHeader;

                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvResultCarrier, "RepCarriers", "Carriers", ""));
            }
        }
        public void RepHistoricoGastoInternet(Control contenedor2)
        {
            maxFechaAnio = DateTime.Now.Year.ToString();
            DataTable GrafPieConsPorConceptoTot = ReportesTelFijaMovil.Reportehistoricogastointernet(Session["iCodUsuario"].ToString());
            DataView dvGrafPieConsPorConceptoTot = new DataView(GrafPieConsPorConceptoTot);

            GrafPieConsPorConceptoTot = dvGrafPieConsPorConceptoTot.ToTable(false,
                    new string[] { "Fecha", "GastoNacional", "GastoInternac", "GastoPaquetes" });
            GrafPieConsPorConceptoTot.Columns["GastoNacional"].ColumnName = "Gasto Nacional";
            GrafPieConsPorConceptoTot.Columns["GastoInternac"].ColumnName = "Gasto Internacional";
            GrafPieConsPorConceptoTot.Columns["GastoPaquetes"].ColumnName = "Gasto Paquetes";
            GrafPieConsPorConceptoTot.AcceptChanges();

             string[] ConsumoGlobalGrafDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(GrafPieConsPorConceptoTot));

            contenedor2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabBolsaDiariaN2Graf_G", "Consumo historico", 2, FCGpoGraf.Matricial));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabBolsaDiariaN2Graf_G",
               FCAndControls.GraficaMultiSeries(
                   ConsumoGlobalGrafDataSource,
                   FCAndControls.extraeNombreColumnas(GrafPieConsPorConceptoTot),
                   "RepTabBolsaDiariaN2Graf_G", "", "", "Fecha", "Gasto Nacional", 2, FCGpoGraf.Matricial),
                   false);
        }

        public void UsoInternetPorTabs(string tituloGrafica)
        {
            try
            {
                AjaxControlToolkit.TabPanel Tabp3;
                AjaxControlToolkit.TabPanel Tabp2;
                AjaxControlToolkit.TabPanel Tabp1;
                DataTable dtRecord;
                Panel Tab3;
                Panel Tab2;
                Panel Tab1;
                AjaxControlToolkit.TabContainer TabContainer;
                if (tituloGrafica == "Datos Nacionales")
                {
                    dtRecord = ReportesTelFijaMovil.ReporteInternetNacIntl(Session["iCodUsuario"].ToString(),"Nac");
                    TabContainer = TabContainerPrincipal;
                    Tab1 = Tab1Rep1;
                    Tab2 = Tab2Rep1;
                    Tab3 = Tab3Rep1;
                    Tabp1 = TabPanelUno;
                    Tabp2 = TabPanelDos;
                    Tabp3 = TabPanelTres;
                }
                else
                {
                    dtRecord = ReportesTelFijaMovil.ReporteInternetNacIntl(Session["iCodUsuario"].ToString(),"Int");
                    TabContainer = TabContainerPrincipal2;
                    Tab1 = Tab1Rep2;
                    Tab2 = Tab2Rep2;
                    Tab3 = Tab3Rep2;
                    Tabp1 = TabPanel2Uno;
                    Tabp2 = TabPanel2Dos;
                    Tabp3 = TabPanel2Tres;
                }
                if (dtRecord != null && dtRecord.Rows.Count > 0)
                {
                    TabInternetImporte(dtRecord, Tab1, Tabp1, tituloGrafica);
                    TabInternetGB(dtRecord, Tab2, Tabp2, tituloGrafica);
                    TabInternetImporteGB(dtRecord, Tab3, Tabp3, tituloGrafica);

                    TabContainer.ActiveTabIndex = 0;
                    TabContainer.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReporteConsumoPlanesFacturados(Control Content)
        {
            string nomColID = "iCodCatCarrier";

            DataTable GrafPieConsPorConceptoTot = ReportesTelFijaMovil.TIMMovilesConsumoPorCategoriaYPlanTarifario(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, incluirInfoTelMovil: 1);

            if (GrafPieConsPorConceptoTot.Rows.Count > 0 && GrafPieConsPorConceptoTot.Columns.Count > 0)
            {
                string IdGrafica = "GraftCons";
                Content.Controls.Add(
                  DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                  DTIChartsAndControls.GridView("IdCatalogoFour", GrafPieConsPorConceptoTot, true, "Totales",
                                  new string[] { "", "{0:c}", "", "", "", "", "", "", "", "", "" }),
                                   IdGrafica, "Planes facturados " + maxFechaAnio, Request.Path + "?Nav=HistoricoN1", 0, FCGpoGraf.Doughnut2d)
                  );

                Page.ClientScript.RegisterStartupScript(this.GetType(), IdGrafica,
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafPieConsPorConceptoTot), IdGrafica,
                                   "", "", "", "", "Doughnut2d", "", "%", "dti", "100%", "280", true), false);

            }
        }

        public void ReporteConsumoPlanesFacturadosExcedentes (Control Content)
        {
            string nomColID = "iCodCatCarrier";

            DataTable GrafPieConsPorConceptoTot = ReportesTelFijaMovil.TIMMovilesConsumoPorCategoriaYPlanTarifarioPromedio(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, incluirInfoTelMovil: 1);

            if (GrafPieConsPorConceptoTot.Rows.Count > 0 && GrafPieConsPorConceptoTot.Columns.Count > 0)
            {
                Control[] tablasDatos = new Control[1];
                string[] titulosRep = new string[1];
                titulosRep[0] ="Planes";
                string IdGrafica = "RepConsumoPlanesFact";
        
                GridView gvRepAhorroVsfact = DTIChartsAndControls.GridView("IdCatalogoFourFactura", GrafPieConsPorConceptoTot, true, "Totales", 
                                             new string[] { "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" });
                
                gvRepAhorroVsfact.RowDataBound += (sender, e) => CambiaColorLetra_RowDataBoundN(sender, e, "Excedentes", "Plan");
                gvRepAhorroVsfact.DataBind();
                gvRepAhorroVsfact.UseAccessibleHeader = true;
                gvRepAhorroVsfact.HeaderRow.TableSection = TableRowSection.TableHeader;
               

                tablasDatos[0] = gvRepAhorroVsfact;
                //Content.Controls.Add(DTIChartsAndControls.TituloYPestañasRepTablasTIM(tablasDatos, titulosRep, IdGrafica, "Planes facturados " + maxFechaAnio, 0));
                GridViewRow row = gvRepAhorroVsfact.Rows[gvRepAhorroVsfact.Rows.Count - 1];
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }
                Content.Controls.Add(
                 DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                 gvRepAhorroVsfact, IdGrafica, "Planes facturados " + maxFechaAnio, Request.Path + "?Nav=HistoricoN1", 0, FCGpoGraf.Doughnut2d)
                 );

                Page.ClientScript.RegisterStartupScript(this.GetType(), IdGrafica,
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafPieConsPorConceptoTot), IdGrafica,
                                   "", "", "", "", "Doughnut2d", "", "%", "dti", "100%", "280", true), false);
            }
        }



        public void TabInternetImporte(DataTable dt,Panel Tab, AjaxControlToolkit.TabPanel Tabplanel, string tituloGrafica)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                Tab.Visible = true;
                DataTable dt3 = dt;
                string RandomId = (new Random().Next()).ToString();
                DataView dvldt = new DataView(dt3);
                dt3 = DTIChartsAndControls.ordenaTabla(dt3, "IMPORTE Desc");
                dt3.AcceptChanges();

                Control Controles = DTIChartsAndControls.GridView("RepConsumoConceptoTab1Rep1_T" + RandomId, dt3, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}" });

                dt3 = dvldt.ToTable(false, new string[] { "APP", "IMPORTE" });
                if (dt3.Rows[dt3.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt3.Rows[dt3.Rows.Count - 1].Delete();
                }

                dt3.Columns["APP"].ColumnName = "label";
                dt3.Columns["IMPORTE"].ColumnName = "value";
                dt3.AcceptChanges();

                string idContener = "RepConsumoConceptoTab1Rep1_Graf" + RandomId;

                Tabplanel.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica(idContener, tituloGrafica));

                Page.ClientScript.RegisterStartupScript(this.GetType(), idContener,
                         FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt3), idContener,
                                    "", "", "label", "value", "doughnut2d", "$", "", "dti", "100%", "350", true), false);
            }
        }
        public void TabInternetGB(DataTable dt, Panel Tab, AjaxControlToolkit.TabPanel Tabplanel,string tituloGrafica)
        {
            try
            {
                Tab.Visible = true;
                DataTable dt2 = dt;
                string RandomId = (new Random().Next()).ToString();
                DataView dvldt = new DataView(dt2);
                dt2 = DTIChartsAndControls.ordenaTabla(dt2, "GB Desc");
                dt2.AcceptChanges();

                Control Controles = DTIChartsAndControls.GridView("RepConsumoConceptoTab2Rep1_T" + RandomId, dt2, true, "Totales",
                new string[] { "", "{0:c}", "{0:0,0}" });

                dt2 = dvldt.ToTable(false, new string[] { "APP", "GB" });
                if (dt2.Rows[dt2.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt2.Rows[dt2.Rows.Count - 1].Delete();
                }

                dt2.Columns["APP"].ColumnName = "label";
                dt2.Columns["GB"].ColumnName = "value";
                dt2.AcceptChanges();
               string idContener = "RepConsumoConceptoTab2Rep3_Graf" + RandomId;

                Tabplanel.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica(idContener, tituloGrafica));

                Page.ClientScript.RegisterStartupScript(this.GetType(), idContener,
                         FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt2), idContener,
                                    "", "", "label", "value", "doughnut2d", "$", "", "dti", "100%", "350", true), false);
                           }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void TabInternetImporteGB(DataTable dt, Panel Tab, AjaxControlToolkit.TabPanel Tabplanel, string tituloGrafica)
        {
            try
            {
                Tab.Visible = true;
                string RandomId = (new Random().Next()).ToString();

                DataTable dt1 = dt;
                DataView dvldt = new DataView(dt1);
                dt1 = DTIChartsAndControls.ordenaTabla(dt1, "IMPORTE Desc");
                dt1.AcceptChanges();

                Control Controles = DTIChartsAndControls.GridView("RepConsumoConceptoTab3Rep1_T" + RandomId, dt1, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}" });

                dt1 = dvldt.ToTable(false, new string[] { "APP", "IMPORTE", "GB" });
                if (dt1.Rows[dt1.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt1.Rows[dt1.Rows.Count - 1].Delete();
                }

                dt1.Columns["APP"].ColumnName = "label";
                dt1.Columns["IMPORTE"].ColumnName = "value";
                dt1.AcceptChanges();

                DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(dt1);
                string idContener = "RepConsumoConceptoTab3Rep1_Graf" + RandomId;

                Tabplanel.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica(idContener, tituloGrafica));

                Page.ClientScript.RegisterStartupScript(this.GetType(), idContener,
                         FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt1), idContener,
                                    "", "", "label", "value", "doughnut2d", "$", "", "dti", "100%", "350", true), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReportePorConcepGrafTotalizado(Control contenedor1, GroupBy agrupado, string tipoGraf,  Control contenedor2)
        {
            string nomColID = "CategoriaCod";
            int concepto = (agrupado.ToString() == "Concepto") ? 1 : 0;
            DataTable GrafPieConsPorConceptoTot = ReportesTelFijaMovil.ReporteConceptoAcumulado(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, incluirInfoTelMovil: 1, PorConcepto: concepto);

            if (GrafPieConsPorConceptoTot.Rows.Count > 0 && GrafPieConsPorConceptoTot.Columns.Count > 0)
            {
                if (GrafPieConsPorConceptoTot.Columns.Contains("Categoria"))
                    GrafPieConsPorConceptoTot.Columns["Categoria"].ColumnName = agrupado.ToString();

                var primerID = Convert.ToInt32(GrafPieConsPorConceptoTot.Rows[0][0]);

                DataView dvGrafPieConsPorConceptoTot = new DataView(GrafPieConsPorConceptoTot);

                GrafPieConsPorConceptoTot = dvGrafPieConsPorConceptoTot.ToTable(false, new string[] { nomColID, agrupado.ToString(), "Total Anual" });
                GrafPieConsPorConceptoTot.Columns[agrupado.ToString()].ColumnName = "label";
                GrafPieConsPorConceptoTot.Columns["Total anual"].ColumnName = "value";
                if (contenedor2 != null)
                {
                    GrafPieConsPorConceptoTot.Columns.Add("link");
                    foreach (DataRow ldr in GrafPieConsPorConceptoTot.Rows)
                    {
                        ldr["link"] = "j-DrillDown" + agrupado.ToString() + "s-" + ldr[nomColID];
                    }

                    contenedor2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("DetalleConsumo" + agrupado.ToString(), "Detalle consumo " + maxFechaAnio));

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "CambiaGraficaDrillDown" + agrupado.ToString(),
                                            JavaScriptDrillDown(GrafPieConsPorConceptoTot, agrupado.ToString(), nomColID), false);
                }

                //NZ 20160210. Se solicito cambio de se grafiquen porcentajes.
                GrafPieConsPorConceptoTot.Columns.Remove(nomColID);
                if (GrafPieConsPorConceptoTot.Columns["value"].DataType != System.Type.GetType("System.String"))
                {
                    double total = Convert.ToDouble(GrafPieConsPorConceptoTot.Compute("SUM([value])", string.Empty));
                    if (total != 0)
                    {
                        foreach (DataRow row in GrafPieConsPorConceptoTot.Rows)
                        {
                            if ((Convert.ToDouble(row["value"]) > 0))
                            {
                                row["value"] = (Convert.ToDouble(row["value"]) * 100) / total;
                            }
                            else { row["value"] = 0; }
                        }
                    }
                }
                string idControl = "GrafPieConsPor2" + agrupado.ToString() + "Tot";

                contenedor1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica(idControl, "Por " + agrupado.ToString() + " acumulado " + maxFechaAnio));

                Page.ClientScript.RegisterStartupScript(this.GetType(), idControl,
                         FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafPieConsPorConceptoTot), idControl,
                                    "", "", "", "", tipoGraf, "", "%", "dti", "100%", "280", true), false);

                //Seleccionar la primera
                ScriptManager.RegisterStartupScript(this, typeof(Page), "CallFunction", "$(document).ready(function(){DrillDown" + agrupado.ToString() + "s('" + primerID + "');});", true);
            }
        }

        public void RepTabPanelPorXAnioActual(GroupBy agrupado, ref Control[] tablasDatos, int index)
        {
            #region Consumo por conceptos o Carrier

            string ColumnaGrupo = agrupado.ToString() == "Concepto" ? "Categoria" : agrupado.ToString();
            int concepto = (agrupado.ToString() == "Concepto") ? 1 : 0;
            DataTable RepConsumo = ReportesTelFijaMovil.ConsumoGeneral(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, incluirInfoTelMovil: 1, PorConcepto: concepto);

            //string QueryGrupo = agrupado.ToString() == "Carrier" ? ConsumoGeneralCarrier() : reporteporconcepto();
            //DataTable RepConsumo = DSODataAccess.Execute(QueryGrupo);
            if (RepConsumo.Rows.Count > 0 && RepConsumo.Columns.Count > 0)
            {
                DataView dvRepConsumo = new DataView(RepConsumo);
                RepConsumo.Columns[ColumnaGrupo].ColumnName = agrupado.ToString();

                RepConsumo = dvRepConsumo.ToTable(false, new string[] { agrupado.ToString(), "Total Anual", "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" });
                RepConsumo.AcceptChanges();
            }

            tablasDatos[index] = DTIChartsAndControls.GridView("RepConsumoPorConceptoOCarrier" + agrupado.ToString(), RepConsumo, true, "Totales",
                           new string[] { "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }, 2, false);

            #endregion Consumo por conceptos o Carrier
        }

        public void ReporteConsumoGlobalPorXTabsComparativos(Control contenedor, GroupBy agrupado)
        {
            if(agrupado == GroupBy.Carrier)
            {
                Control[] tablasDatos = new Control[4];
                string[] titulosRep = new string[4];
                var dtConsumoGlobal = ReporteConsumoGlobalGrafica();
                RepTabPanelConsumoGlobal(dtConsumoGlobal, ref tablasDatos, ref titulosRep, 0);

                titulosRep[1] = "Por " + agrupado.ToString();
                RepTabPanelPorXAnioActual(agrupado, ref tablasDatos, 1);

                RepTabPanelPorXAhorroMesActVsMesAnt(agrupado, ref tablasDatos, ref titulosRep, 2);

                RepTabPanelPorXAhorroMismoMesAnioActVsMesAnioAnt(agrupado, ref tablasDatos, ref titulosRep, 3);

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepTablasTIM(
                    tablasDatos, 
                    titulosRep,
                    "RepTabsGeneral" + agrupado.ToString(),
                    "Consumos " + maxFechaAnio,
                    0,
                    false, 
                    "btnExportarXLS_Click", 
                    "btnConsumoPor" + agrupado.ToString(), 
                    this, false));
            }
            else
            {
                Control[] tablasDatos = new Control[1];
                string[] titulosRep = new string[1];
                titulosRep[0] = "Por " + agrupado.ToString();
                RepTabPanelPorXAnioActual(agrupado, ref tablasDatos, 0);

                //RepTabPanelPorXAhorroMesActVsMesAnt(agrupado, ref tablasDatos, ref titulosRep, 1);

                //RepTabPanelPorXAhorroMismoMesAnioActVsMesAnioAnt(agrupado, ref tablasDatos, ref titulosRep, 2);

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepTablasTIM(
                        tablasDatos,
                        titulosRep, 
                        "RepTabsGeneral" + agrupado.ToString(), 
                        "Consumos " + maxFechaAnio, 0,
                        false, 
                        "btnExportarXLS_Click", 
                        "btnConsumoPor" + agrupado.ToString(), 
                        this, false));
            }
        }


       
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        

        public void AgregarHiperlink_RowDataBound(object sender, GridViewRowEventArgs e, string pathImg, int indexHyperLink)
        {
            //if it is not DataRow return.
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            TableCell td = e.Row.Cells[indexHyperLink];
            if (td.Controls.Count > 0 && td.Controls[0] is HyperLink)
            {
                HyperLink hyperLink = td.Controls[0] as HyperLink;
                hyperLink.ImageUrl = pathImg + hyperLink.Text.ToString().Replace(" ", "") + ".png";
                hyperLink.ToolTip = hyperLink.Text;
                hyperLink.Text = string.Empty;
                hyperLink.Width = 110;
                hyperLink.Height = 16;
                hyperLink.CssClass = "customerLogoImgTIM";
            }
            td.HorizontalAlign = HorizontalAlign.Center;
        }

    

        private void RepTabPanelConsumoGlobal(DataTable dtConsumoGlobal, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            #region Consumo global

            DataTable RepConsumoGlobal = dtConsumoGlobal.Select().CopyToDataTable();
            if (RepConsumoGlobal.Rows.Count > 0 && RepConsumoGlobal.Columns.Count > 0)
            {
                DataView dvRepConsumoGlobal = new DataView(RepConsumoGlobal);
                RepConsumoGlobal = dvRepConsumoGlobal.ToTable(false,
                    new string[] { "Mes", "AÑO ANTERIOR", "AÑO ACTUAL" });
                RepConsumoGlobal.Columns["AÑO ANTERIOR"].ColumnName = "Año " + (Convert.ToInt32(maxFechaAnio) - 1).ToString();
                RepConsumoGlobal.Columns["AÑO ACTUAL"].ColumnName = "Año " + maxFechaAnio;
                RepConsumoGlobal.AcceptChanges();
            }

            titulosRep[index] = "Global";
            tablasDatos[index] = DTIChartsAndControls.GridView("RepConsumoGlobal", RepConsumoGlobal, false, "", new string[] { "", "{0:c}", "{0:c}" }, 2, false);

            #endregion Consumo global
        }
        private DataTable ReporteConsumoGlobalGrafica()
        {
            string Carrier = string.IsNullOrEmpty(param["Carrier"]) ? "-1" : param["Carrier"];
            DataTable ConsumoGlobalGraf = ReportesTelFijaMovil.TIMConsumoGeneralHistorico2Anios(Carrier, Session["iCodUsuario"].ToString(),empreSelected,incluirInfoTelMovil:1);

            if (ConsumoGlobalGraf.Rows.Count > 0 && ConsumoGlobalGraf.Columns.Count > 0)
            {
                DataView dvConsumoGlobalGraf = new DataView(ConsumoGlobalGraf);
                ConsumoGlobalGraf = dvConsumoGlobalGraf.ToTable(false,
                    new string[] { "Mes", "AÑO ANTERIOR", "AÑO ACTUAL" });
                ConsumoGlobalGraf.AcceptChanges();

                //contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("ConsumoGlobalGraf", "Consumo global " + maxFechaAnio));

                //string[] ConsumoGlobalGrafDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ConsumoGlobalGraf));
                /*
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsumoGlobalGraf",
                    FCAndControls.GraficaMultiSeries(ConsumoGlobalGrafDataSource, FCAndControls.extraeNombreColumnas(ConsumoGlobalGraf),
                    "ConsumoGlobalGraf", "", "", "MES", "IMPORTE", "msline", "$", "", "dti", "98%", "280", true), false);
                */
            }

            return ConsumoGlobalGraf;
        }

        public string GetConsumoGlobalGrafica()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralHistorico2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected);
            return query.ToString();
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            LinkButton boton = sender as LinkButton;
            ExportXLS(".xlsx", boton.ID.Replace("btn", ""));
        }
        public void RepTabPanelPorXAhorroMesActVsMesAnt(GroupBy agrupado, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            #region Ahorros mes Actual vs mes anterior

            int concepto = (agrupado.ToString() == "Concepto") ? 1 : 0;
            DataTable RepAhorroVsMesAnterior = ReportesTelFijaMovil.TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, "MesActualvsMesAnterior", incluirInfoTelMovil: 1, PorConcepto: concepto);
            if (RepAhorroVsMesAnterior.Rows.Count > 0 && RepAhorroVsMesAnterior.Columns.Count > 0)
            {
                string columnas = "CategoriaCod|CarrierCod|EmpreCod|ImporteMoneda";
                var array = columnas.Split('|');
                for (int i = 0; i < array.Length; i++)
                {
                    if (RepAhorroVsMesAnterior.Columns.Contains(array[i]))
                        RepAhorroVsMesAnterior.Columns.Remove(array[i]);
                }

                RepAhorroVsMesAnterior.Columns[agrupado.ToString() == "Concepto" ? "Categoria" : agrupado.ToString()].ColumnName = agrupado.ToString();

                titulosRep[index] = "Ahorros vs mes anterior";

                GridView gvRepAhorroVsMesAnterior = DTIChartsAndControls.GridView("RepAhorroVsMesAnterior" + agrupado.ToString(),
                    RepAhorroVsMesAnterior, true, "Totales", new string[] { "", "{0:c}", "{0:c}", "{0:c}" }, 2, false);

                gvRepAhorroVsMesAnterior.RowDataBound += (sender, e) => CambiaColorLetra_RowDataBound(sender, e, "Ahorro", agrupado.ToString());
                gvRepAhorroVsMesAnterior.DataBind();
                gvRepAhorroVsMesAnterior.UseAccessibleHeader = true;
                gvRepAhorroVsMesAnterior.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepAhorroVsMesAnterior.Rows[gvRepAhorroVsMesAnterior.Rows.Count - 1];
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                tablasDatos[index] = gvRepAhorroVsMesAnterior;
            }

            #endregion Ahorros vs mes anterior
        }

        public void RepTabPanelPorXAhorroMismoMesAnioActVsMesAnioAnt(GroupBy agrupado, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            #region Ahorros vs mismo mes anio anterior

            int concepto = (agrupado.ToString() == "Concepto") ? 1 : 0;
            DataTable RepAhorroVsMismoMesAnioAnterior = ReportesTelFijaMovil.AhorrosvsmismomesañoanteriorCarrier(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, "MesActualvsMismoMesAnioAnterior", incluirInfoTelMovil: 1, PorConcepto: concepto);

            if (RepAhorroVsMismoMesAnioAnterior.Rows.Count > 0 && RepAhorroVsMismoMesAnioAnterior.Columns.Count > 0)
            {
                string columnas = "CategoriaCod|CarrierCod|EmpreCod|ImporteMoneda";
                var array = columnas.Split('|');
                for (int i = 0; i < array.Length; i++)
                {
                    if (RepAhorroVsMismoMesAnioAnterior.Columns.Contains(array[i]))
                        RepAhorroVsMismoMesAnioAnterior.Columns.Remove(array[i]);
                }

                RepAhorroVsMismoMesAnioAnterior.Columns[agrupado.ToString() == "Concepto" ? "Categoria" : agrupado.ToString()].ColumnName = agrupado.ToString();

                titulosRep[index] = "Ahorros vs mismo mes año anterior";

                GridView gvRepAhorroVsMismoMesAnioAnterior = DTIChartsAndControls.GridView("RepAhorroVsMismoMesAnioAnterior" + agrupado.ToString(),
                    RepAhorroVsMismoMesAnioAnterior, true, "Totales", new string[] { "", "{0:c}", "{0:c}", "{0:c}" }, 2, false);
                gvRepAhorroVsMismoMesAnioAnterior.RowDataBound += (sender, e) => CambiaColorLetra_RowDataBound(sender, e, "Ahorro", agrupado.ToString());
                gvRepAhorroVsMismoMesAnioAnterior.DataBind();
                gvRepAhorroVsMismoMesAnioAnterior.UseAccessibleHeader = true;
                gvRepAhorroVsMismoMesAnioAnterior.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepAhorroVsMismoMesAnioAnterior.Rows[gvRepAhorroVsMismoMesAnioAnterior.Rows.Count - 1];
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                tablasDatos[index] = gvRepAhorroVsMismoMesAnioAnterior;

            }
            #endregion Ahorros vs mismo mes anio anterior
        }

        public void CambiaColorLetra_RowDataBound(object sender, GridViewRowEventArgs e, string nombreColumnaValor, string nomColTotales)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int totalAnual = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, nombreColumnaValor));
                string isTotales = DataBinder.Eval(e.Row.DataItem, nomColTotales).ToString();

                if (isTotales != "Totales")
                {
                    if (totalAnual < 0)
                    {
                        e.Row.CssClass = "letraRoja";
                    }
                    if (totalAnual > 0)
                    {
                        e.Row.CssClass = "letraVerde";
                    }
                    if (totalAnual == 0)
                    {
                        e.Row.BackColor = Color.Gainsboro;
                    }
                }
            }
        }

        public void CambiaColorLetra_RowDataBoundN(object sender, GridViewRowEventArgs e, string nombreColumnaValor, string nomColTotales)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int totalAnual = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, nombreColumnaValor));
                string isTotales = DataBinder.Eval(e.Row.DataItem, nomColTotales).ToString();

                if (isTotales != "Totales")
                {
                    if (totalAnual > 0)
                    {
                        e.Row.CssClass = "letraRoja";
                    }
                    if (totalAnual == 0)
                    {
                        e.Row.CssClass = "letraVerde";
                    }
                }
            }
        }


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
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por Concepto " + maxFechaAnio);

                        DataTable RepConsumoPorConcepto = null; // DSODataAccess.Execute(GetConsumoPorXAnioActual(GroupBy.Concepto));
                        if (RepConsumoPorConcepto.Rows.Count > 0 && RepConsumoPorConcepto.Columns.Count > 0)
                        {
                            DataView dvRepConsumoPorConcepto = new DataView(RepConsumoPorConcepto);
                            RepConsumoPorConcepto = dvRepConsumoPorConcepto.ToTable(false,
                                new string[] { "Categoria", "Total Anual", "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" });

                            RepConsumoPorConcepto.Columns["Categoria"].ColumnName = "Concepto";
                            RepConsumoPorConcepto.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsumoPorConcepto, 0, "Totales"), "Reporte", "Tabla");
                        }
                    }
                }
                else if (reportExportar == "ConsumoPorCarrier")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashboardSiana\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por Carrier " + maxFechaAnio);

                    DataTable RepConsumoPorConcepto = null;// DSODataAccess.Execute(GetConsumoPorXAnioActual(GroupBy.Carrier));
                    if (RepConsumoPorConcepto.Rows.Count > 0 && RepConsumoPorConcepto.Columns.Count > 0)
                    {
                        DataView dvRepConsumoPorConcepto = new DataView(RepConsumoPorConcepto);
                        RepConsumoPorConcepto = dvRepConsumoPorConcepto.ToTable(false,
                            new string[] { "Carrier", "Total Anual", "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" });

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsumoPorConcepto, 0, "Totales"), "Reporte", "Tabla");
                    }
                }
                else if (reportExportar == "ConsumoPorEmpresa")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashboardSiana\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por Empresa " + maxFechaAnio);

                    DataTable RepConsumoPorConcepto = null;// DSODataAccess.Execute(GetConsumoPorXAnioActual(GroupBy.Empresa));
                    if (RepConsumoPorConcepto.Rows.Count > 0 && RepConsumoPorConcepto.Columns.Count > 0)
                    {
                        DataView dvRepConsumoPorConcepto = new DataView(RepConsumoPorConcepto);
                        RepConsumoPorConcepto = dvRepConsumoPorConcepto.ToTable(false,
                            new string[] { "Empresa", "Total Anual", "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" });

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsumoPorConcepto, 0, "Totales"), "Reporte", "Tabla");
                    }
                }

                #endregion Exportar Reportes solo con tabla

                #region Exportar Reportes pequeños

                else if (reportExportar == "Volumetria")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashboardSiana\ReporteTablasVolumetria" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (reportExportar == "Volumetria")
                    {
                        //ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Volumetría " + maxFechaMesLetra);

                        #region Tarifas
                        DataTable dtResult = null; //DSODataAccess.Execute(GetComparacionTarifasUltMes());
                        if (dtResult.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dtResult);
                            dtResult = dvldt.ToTable(false, new string[] { "Destino", "Tarifa" });
                            dtResult.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtResult, "Reporte", "Tabla1");
                        }
                        #endregion
                        dtResult = null;

                        #region Rentas
                        //dtResult = DSODataAccess.Execute(GetRepRentasUltMes());
                        if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtResult);
                            dtResult = dvldt.ToTable(false, new string[] { "Concepto", "Renta" });
                            dtResult.AcceptChanges();

                            foreach (DataRow row in dtResult.Rows)
                            {
                                if (row["Concepto"].ToString().ToLower().Contains("troncal"))
                                {
                                    row["Concepto"] = "TRONCAL DIGITAL";
                                }
                            }

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtResult, "Reporte", "Tabla2");
                        }
                        #endregion
                        dtResult = null;

                        #region Recursos
                        //dtResult = DSODataAccess.Execute(GetRepRecursosUltMes());
                        if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtResult);
                            dtResult = dvldt.ToTable(false, new string[] { "Tipo Recurso", "Cantidad" });
                            dtResult.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtResult, 0, "Totales"), "Reporte", "Tabla3");
                        }
                        #endregion
                        dtResult = null;

                        #region Conusmos
                        //dtResult = DSODataAccess.Execute(GetConsumoLlamsMinUltMes());
                        if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtResult);
                            dtResult = dvldt.ToTable(false, new string[] { "Destino", "Llamadas", "Minutos", "Total" });
                            dtResult.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtResult, 0, "Totales"), "Reporte", "Tabla4");
                        }
                        #endregion
                    }
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
    }
}