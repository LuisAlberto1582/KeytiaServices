using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Collections;
using KeytiaServiceBL.Reportes;
using AjaxControlToolkit;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using KeytiaWeb.UserInterface.DashboardFC.Reportes;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    enum GroupBy { Empresa = 1, Carrier = 2, Concepto = 3 };
    public partial class ConsumoGeneral : System.Web.UI.Page
    {
        #region Variables

        StringBuilder query = new StringBuilder();
        string maxFecha = "";
        string maxFechaMesLetra = "";
        string maxFechaAnio = "";
        string carrierSelected = "-1";
        string empreSelected = "-1";
        bool nvlGlobal = false;
        List<InfoCarrierGlobal> dtCarriers = new List<InfoCarrierGlobal>();
        List<InfoCarrierGlobal> dtEmpres = new List<InfoCarrierGlobal>();

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

                LeeQueryString();

                dtCarriers = GetCarriersTIM(0, 0, "ConsumoGeneral");

                if (dtCarriers.Count > 0) //El Cliente tiene carriers del TIM
                {
                    ddlCarrier.Visible = btnAplicar.Visible = false;

                    //Distinct de Empresas
                    dtCarriers.GroupBy(g => new { g.Empre, g.EmpreDesc }).Select(x => x.First()).ToList().ForEach(n => dtEmpres.Add(new InfoCarrierGlobal() { Empre = n.Empre, EmpreDesc = n.EmpreDesc }));

                    if (empreSelected == "-1" && dtEmpres.Count > 1)
                    {
                        DashboardGlobalALLEmpre();
                    }
                    else
                    {
                        //En este punto debemos tener una empresa seleccionada
                        if (empreSelected == "-1" && dtEmpres.Count == 1)
                        { empreSelected = dtEmpres.First().Empre.ToString(); }

                        if (carrierSelected == "-1" && dtCarriers.Where(x => x.Empre.ToString() == empreSelected).GroupBy(gpo => gpo.iCodCatalogo).Select(k => k.Key).Count() > 1)
                        {
                            DashboardGlobalAllCarriersPorEmpre();
                        }
                        else
                        {
                            if (carrierSelected == "-1" && dtCarriers.Count == 1) { carrierSelected = "1"; }
                            DashboardGeneralPorCarrierPorEmpre(); //Contiene un ID de Carrier valido
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void LeeQueryString()
        {
            #region Revisar si el querystring Empre contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["Empre"]))
            {
                try
                {
                    empreSelected = Request.QueryString["Empre"];
                }
                catch (Exception) { carrierSelected = "-1"; }
            }
            else { empreSelected = "-1"; }

            #endregion Revisar si el querystring Empre contiene un valor

            #region Revisar si el querystring Carrier contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["Carrier"]))
            {
                try
                {
                    carrierSelected = Request.QueryString["Carrier"];
                }
                catch (Exception) { carrierSelected = "-1"; }
            }
            else { carrierSelected = "-1"; }

            #endregion Revisar si el querystring Carrier contiene un valor
        }

        public void DashboardGlobalALLEmpre()
        {
            ddlCarrier.Visible = btnAplicar.Visible = true;
            if (!Page.IsPostBack)
            {
                var carriers = dtCarriers.
                                GroupBy(g => new { g.iCodCatalogo, g.vchDescripcion }).
                                Select(x => x.First()).
                                Select(x => new { x.iCodCatalogo, EmpreDesc = x.vchDescripcion + " - " + x.EmpreDesc }).ToList();

                ddlCarrier.DataSource = carriers;

                ddlCarrier.DataValueField = "iCodCatalogo";
                ddlCarrier.DataTextField = "EmpreDesc";
                ddlCarrier.DataBind();
            }

            //Datos de año y mes
            var rowCarrier = dtCarriers.OrderByDescending(x => x.MaxFecha).First();

            ///RM 20190529 Se modifica la forma en base a a que se calcula la maxFecha para mostrar en Dsh
            var carrier = 0;
            int.TryParse(ddlCarrier.SelectedValue, out carrier);

            if (carrier == 0 && int.Parse(carrierSelected) > 0)
            {
                int.TryParse(carrierSelected, out carrier);
            }

            if (carrier > 0)
            {
                rowCarrier = dtCarriers.Where(x => (x.IdsCarrierEmpre.ToString().Split('-'))[0] == carrier.ToString()).OrderByDescending(x => x.MaxFecha).First();
            }


            carrierSelected = ddlCarrier.SelectedValue;
            empreSelected = "-1";
            maxFecha = rowCarrier.MaxFecha.ToString();
            maxFechaMesLetra = rowCarrier.MesNombre.ToString();
            maxFechaAnio = rowCarrier.Año.ToString();
            lblCarrierConsumo.Text = "Consumo Global por Empresa";
            nvlGlobal = true;

            //Grafica Consumo global
            var dtResultados = ReporteConsumoGlobalGrafica();

            ////Reportes en tabs
            ReporteConsumoGlobalPorXTabsComparativos(dtResultados, GroupBy.Empresa);

            ////Reporte por concepto grafica pie totalizado
            ReportePorConcepGrafTotalizado("EmpreCod", GroupBy.Empresa, "doughnut2d", Rep2, Rep4);

            ///Reporte Ultimo mes de Inventario activo
            ReporteRecursoUltMes(GroupBy.Empresa);

            //Reporte del Uso/Consumo de llamadas y minutos del ultimo mes cargado en el sistema.
            ReporteConsumoLlamsMinUltMes();

            ////Reporte por concepto. barras totalizado.
            ReportePorConcepGrafTotalizado("CategoriaCod", GroupBy.Concepto, "bar2d", Rep6, null);

            ///Por por concepto y comparativos
            ReporteConsumoGlobalPorXTabsComparativos(GroupBy.Concepto, Rep5);

            ////Reporte de Empresas
            ReporteEmpresas();
        }

        public void DashboardGlobalAllCarriersPorEmpre()
        {
            //Datos de año y mes
            var rowCarrier = dtCarriers.OrderByDescending(x => x.MaxFecha).First();

            var empre = dtCarriers.Where(x => x.Empre.ToString() == empreSelected).FirstOrDefault();
            var empreDesc = (empre != null && empre.EmpreDesc.Length > 0) ? empre.EmpreDesc : "";

            var lblCarrierDesc = empreDesc.Length > 0 ? "Consumo Global (Carriers)  Empresa: " + empreDesc : "Consumo Global (Carriers)";
            carrierSelected = "-1";
            maxFecha = rowCarrier.MaxFecha.ToString();
            maxFechaMesLetra = rowCarrier.MesNombre;
            maxFechaAnio = rowCarrier.Año.ToString();
            lblCarrierConsumo.Text = lblCarrierDesc;
            nvlGlobal = true;

            //Grafica Consumo global
            var dtResultados = ReporteConsumoGlobalGrafica();

            ////Reportes en tabs
            ReporteConsumoGlobalPorXTabsComparativos(dtResultados, GroupBy.Carrier);

            ////Reporte por concepto grafica pie totalizado
            ReportePorConcepGrafTotalizado("CarrierCod", GroupBy.Carrier, "doughnut2d", Rep2, Rep4);//"pie3d");

            ///Reporte Ultimo mes de Inventario activo
            ReporteRecursoUltMes(GroupBy.Carrier);

            //Reporte del Uso/Consumo de llamadas y minutos del ultimo mes cargado en el sistema.
            ReporteConsumoLlamsMinUltMes();

            ////Reporte por concepto. barras totalizado.
            ReportePorConcepGrafTotalizado("CategoriaCod", GroupBy.Concepto, "bar2d", Rep6, null);

            ///Por por concepto y comparativos
            ReporteConsumoGlobalPorXTabsComparativos(GroupBy.Concepto, Rep5);

            //Reporte de Carriers
            ReporteCarriers();
        }

        public void DashboardGeneralPorCarrierPorEmpre()
        {
            InfoCarrierGlobal rowCarrier = null;
            if (carrierSelected == "1" || carrierSelected == "-1") // Indica que debe escoger el primero que tenga el cliente.
            {
                rowCarrier = dtCarriers.FirstOrDefault(x => x.Empre.ToString() == empreSelected);
            }
            else if (Convert.ToInt32(carrierSelected) > 1)
            {
                rowCarrier = dtCarriers.FirstOrDefault(x => x.iCodCatalogo.ToString() == carrierSelected && x.Empre.ToString() == empreSelected);
            }

            if (rowCarrier != null)
            {



                var empre = dtCarriers.Where(x => x.Empre.ToString() == empreSelected).FirstOrDefault();
                var empreDesc = (empre != null && empre.EmpreDesc.Length > 0) ? empre.EmpreDesc : "";

                var lblCarrierDesc = empreDesc.Length > 0 ? "Consumo " + rowCarrier.vchDescripcion + " Empresa: " + empreDesc : "Consumo " + rowCarrier.vchDescripcion;

                carrierSelected = rowCarrier.iCodCatalogo.ToString(); //Toma el valor del primer carrier del cliente
                maxFecha = rowCarrier.MaxFecha.ToString();
                maxFechaMesLetra = rowCarrier.MesNombre;
                maxFechaAnio = rowCarrier.Año.ToString();
                lblCarrierConsumo.Text = lblCarrierDesc;

                //Grafica Consumo global
                var dtResultados = ReporteConsumoGlobalGrafica();

                //Reportes en tabs
                ReporteConsumoGlobalPorXTabsComparativos(dtResultados, GroupBy.Concepto);

                //Reporte por concepto grafica pie totalizado
                ReportePorConcepGrafTotalizado("CategoriaCod", GroupBy.Concepto, "bar2d", Rep2, Rep4);

                if (dtCarriers.Count > 1)
                {
                    //Reporte de Carriers
                    ReporteCarriers();
                }

                //Reporte de las Tarifas Globales del ultimo mes cargado en el sistema del que se tengan tarifas calculadas.
                ReporteTarifaGlobalUltMes();

                //Reporte de las Rentas Globales del ultimo mes cargado en el sistema del que se tengan rentas Calculadas.
                ReporteRentaGlobalUltMes();

                //Reporte del inventario de recursos hasta el ultimo mes cargado en el sistema.
                ReporteRecursoUltMes(GroupBy.Concepto);

                //Reporte del Uso/Consumo de llamadas y minutos del ultimo mes cargado en el sistema.
                ReporteConsumoLlamsMinUltMes();
            }
        }


        #region Consultas a BD

        public List<InfoCarrierGlobal> GetCarriersTIM(int iCodCarrier, int iCodCatEmpre, string dashboard)
        {
            List<InfoCarrierGlobal> lista = new List<InfoCarrierGlobal>();
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralGetCarriersTIM] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("      @iCodCatEmpre = " + iCodCatEmpre + ",");
            query.AppendLine("      @iCodCatCarrier = " + iCodCarrier + ",");
            query.AppendLine("      @nomDashboard = '" + dashboard + "'");

            var dt = DSODataAccess.Execute(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    InfoCarrierGlobal r = new InfoCarrierGlobal();
                    r.iCodCatalogo = Convert.ToInt32(item["iCodCatalogo"]);
                    r.vchDescripcion = item["vchDescripcion"].ToString();
                    r.MaxFecha = Convert.ToInt32(item["MaxFecha"]);
                    r.Año = Convert.ToInt32(item["Anio"]);
                    r.Mes = Convert.ToInt32(item["Mes"]);
                    r.MesNombre = item["MesNombre"].ToString();
                    r.Empre = Convert.ToInt32(item["Empre"]);
                    r.EmpreDesc = item["EmpreDesc"].ToString();
                    r.IdsCarrierEmpre = r.iCodCatalogo + "-" + r.Empre;
                    r.NomCarrierEmpre = r.vchDescripcion + " (" + r.EmpreDesc + ")";
                    lista.Add(r);
                }
            }
            return lista;
        }

        private string GetConsumoGlobalGrafica()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralHistorico2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected);
            return query.ToString();
        }

        private string GetConsumoPorXAnioActual(GroupBy agrupado)
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorConceptoOCarrierAnioActual] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected);
            if (nvlGlobal && GroupBy.Concepto == agrupado)
            {
                query.AppendLine("     , @NvlGlobalPorConcepto = 1");
            }

            return query.ToString();
        }

        private string GetConsumoPorXAhorro2Meses(string opcReporte, GroupBy agrupado)
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @OpcionReporte = '" + opcReporte + "'");
            if (nvlGlobal && GroupBy.Concepto == agrupado)
            {
                query.AppendLine("     , @NvlGlobalPorConcepto = 1");
            }
            return query.ToString();
        }

        private string GetConsumoPorX2Anios(int iCodCatTDestCarrierEmpre)
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorConceptoOCarrier2Anios] @Esquema = '" + DSODataContext.Schema + "', ");

            //20190523 RM Busca si hay un carrier Seleccionado en el ddlCarrier de se asi  saca el alor y lo manda al query
            int ddlCarrierValue = 0;
            int.TryParse(ddlCarrier.SelectedValue.ToString(), out ddlCarrierValue);


            int carrier = ddlCarrierValue > 0 ? ddlCarrierValue : -1;
            if (empreSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatCarrier = " + carrier + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Empre'");

            }
            else if (carrierSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Carrier'");
            }
            else
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + carrierSelected + ",");
                query.AppendLine("      @iCodCatTDest = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @Nvl = 'Categoria'");
            }



            /*Version anterior que dejo NZ
             *
            if (empreSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatCarrier = " + -1 + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Empre'");

            }
            else if (carrierSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Carrier'");
            }
            else
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + carrierSelected + ",");
                query.AppendLine("      @iCodCatTDest = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @Nvl = 'Categoria'");
            }
             * 
             */
            return query.ToString();
        }

        private string GetComparacionTarifasUltMes()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralComparacionTarifasUltMes] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @urlImgSemaforoVerde = '~/images/CirculoVerde.png',");
            query.AppendLine("     @urlImgSemaforoRojo = '~/images/CirculoRojo.png',");
            query.AppendLine("     @urlImgSemaforoAzul = '~/images/CirculoAzul.png'");
            return query.ToString();
        }

        private string GetRepRentasUltMes()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralRentasUltMes] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("      @iCodCatCarrier = " + carrierSelected);
            return query.ToString();
        }

        private string GetRepRecursosUltMes()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralRecursosUltMes] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("      @iCodCatCarrier = " + carrierSelected);
            return query.ToString();
        }

        private string GetConsumoLlamsMinUltMes()
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralLlamsMinUltMes] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("      @iCodCatCarrier = " + carrierSelected);
            return query.ToString();
        }

        #endregion Consultas a BD


        #region Reportes

        private DataTable ReporteConsumoGlobalGrafica()
        {
            DataTable ConsumoGlobalGraf = ReportesTelFijaMovil.TIMConsumoGeneralHistorico2Anios(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, incluirInfoTelFija: 1);

            if (ConsumoGlobalGraf.Rows.Count > 0 && ConsumoGlobalGraf.Columns.Count > 0)
            {
                DataView dvConsumoGlobalGraf = new DataView(ConsumoGlobalGraf);
                ConsumoGlobalGraf = dvConsumoGlobalGraf.ToTable(false,
                    new string[] { "Mes", "AÑO ANTERIOR", "AÑO ACTUAL" });
                ConsumoGlobalGraf.AcceptChanges();

                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("ConsumoGlobalGraf", "Consumo global " + maxFechaAnio));

                string[] ConsumoGlobalGrafDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ConsumoGlobalGraf));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsumoGlobalGraf",
                    FCAndControls.GraficaMultiSeries(ConsumoGlobalGrafDataSource, FCAndControls.extraeNombreColumnas(ConsumoGlobalGraf),
                    "ConsumoGlobalGraf", "", "", "MES", "IMPORTE", "msline", "$", "", "dti", "98%", "280", true), false);
            }

            return ConsumoGlobalGraf;
        }

        private void ReporteConsumoGlobalPorXTabsComparativos(DataTable dtConsumoGlobal, GroupBy agrupado)
        {
            Control[] tablasDatos = new Control[4];
            string[] titulosRep = new string[4];

            RepTabPanelConsumoGlobal(dtConsumoGlobal, ref tablasDatos, ref titulosRep, 0);

            titulosRep[1] = "Por " + agrupado.ToString();
            RepTabPanelPorXAnioActual(agrupado, ref tablasDatos, 1);

            RepTabPanelPorXAhorroMesActVsMesAnt(agrupado, ref tablasDatos, ref titulosRep, 2);

            RepTabPanelPorXAhorroMismoMesAnioActVsMesAnioAnt(agrupado, ref tablasDatos, ref titulosRep, 3);

            Rep3.Controls.Add(DTIChartsAndControls.TituloYPestañasRepTablasTIM(tablasDatos, titulosRep, "RepTabsGeneral" + agrupado.ToString(), "Consumos " + maxFechaAnio, 1,
                true, "btnExportarXLS_Click", "btnConsumoPor" + agrupado.ToString(), this, false));
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

        private void RepTabPanelPorXAnioActual(GroupBy agrupado, ref Control[] tablasDatos, int index)
        {
            #region Consumo por conceptos o Carrier
            int concepto = (agrupado.ToString() == "Concepto")? 1:0;

            DataTable RepConsumo = ReportesTelFijaMovil.ConsumoGeneral(carrierSelected, Session["iCodUsuario"].ToString(),empreSelected, incluirInfoTelFija:1,PorConcepto:concepto);
            if (RepConsumo.Rows.Count > 0 && RepConsumo.Columns.Count > 0)
            {
                DataView dvRepConsumo = new DataView(RepConsumo);
                RepConsumo.Columns[agrupado.ToString() == "Concepto" ? "Categoria" : agrupado.ToString()].ColumnName = agrupado.ToString();

                RepConsumo = dvRepConsumo.ToTable(false, new string[] { agrupado.ToString(), "Total Anual", "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" });
                RepConsumo.AcceptChanges();
            }

            tablasDatos[index] = DTIChartsAndControls.GridView("RepConsumoPorConceptoOCarrier" + agrupado.ToString(), RepConsumo, true, "Totales",
                           new string[] { "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }, 2, false);

            #endregion Consumo por conceptos o Carrier
        }

        private void RepTabPanelPorXAhorroMesActVsMesAnt(GroupBy agrupado, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            #region Ahorros mes Actual vs mes anterior

            //DataTable RepAhorroVsMesAnterior = DSODataAccess.Execute(GetConsumoPorXAhorro2Meses("MesActualvsMesAnterior", agrupado));


            int concepto = (agrupado.ToString() == "Concepto") ? 1 : 0;
            DataTable RepAhorroVsMesAnterior = ReportesTelFijaMovil.TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, "MesActualvsMesAnterior", incluirInfoTelFija: 1, PorConcepto: concepto);

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

        private void RepTabPanelPorXAhorroMismoMesAnioActVsMesAnioAnt(GroupBy agrupado, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            #region Ahorros vs mismo mes anio anterior
            int concepto = (agrupado.ToString() == "Concepto") ? 1 : 0;
            DataTable RepAhorroVsMismoMesAnioAnterior = ReportesTelFijaMovil.AhorrosvsmismomesañoanteriorCarrier(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, "MesActualvsMismoMesAnioAnterior", incluirInfoTelFija: 1, PorConcepto: concepto);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomColID"></param>
        /// <param name="tipo">Especificar si es: "Concepto" o "Carrier"</param>
        private void ReportePorConcepGrafTotalizado(string nomColID, GroupBy agrupado, string tipoGraf, Control contenedor1, Control contenedor2)
        {
            //DataTable GrafPieConsPorConceptoTot = DSODataAccess.Execute(GetConsumoPorXAnioActual(agrupado));

            int concepto = (agrupado.ToString() == "Concepto") ? 1 : 0;
            DataTable GrafPieConsPorConceptoTot = ReportesTelFijaMovil.ReporteConceptoAcumulado(carrierSelected, Session["iCodUsuario"].ToString(), empreSelected, incluirInfoTelFija: 1, PorConcepto: concepto);

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

                contenedor1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafPieConsPor" + agrupado.ToString() + "Tot", "Por " + agrupado.ToString() + " acumulado " + maxFechaAnio));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafPieConsPor" + agrupado.ToString() + "Tot",
                         FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafPieConsPorConceptoTot), "GrafPieConsPor" + agrupado.ToString() + "Tot",
                                    "", "", "", "", tipoGraf, "", "%", "dti", "100%", "280", true), false);

                //Seleccionar la primera
                ScriptManager.RegisterStartupScript(this, typeof(Page), "CallFunction", "$(document).ready(function(){DrillDown" + agrupado.ToString() + "s('" + primerID + "');});", true);
            }
        }

        private void ReporteCarriers()
        {
            DataTable dtResult = ConvertToDataTable(dtCarriers.Where(x => x.Empre == Convert.ToUInt32(empreSelected)).ToList());
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
                else if (carrierSelected != "-1" && dtCarriers.Where(x=> x.Empre.ToString() == empreSelected).Count() > 1) //tiene mas de un carrier en esa empresa.
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

                Rep8.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvResultCarrier, "RepCarriers", "Carriers", ""));
            }
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

        private void ReporteTarifaGlobalUltMes()
        {
            DataTable dtResult = DSODataAccess.Execute(GetComparacionTarifasUltMes());
            string maxFechaTarifas = string.Empty;

            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                maxFechaTarifas = dtResult.Rows[0]["MaxFechaTarifas"].ToString();
                maxFechaTarifas = MonthName(Convert.ToInt32(maxFechaTarifas.Substring(4, 2)) - 12) + " " + maxFechaTarifas.Substring(0, 4) + " ";

                DataView dvldt = new DataView(dtResult);
                dtResult = dvldt.ToTable(false, new string[] { "Destino", "Tarifa", "Estatus", "urlImage" });
                dtResult.AcceptChanges();

                dtResult.Columns.Add("ToolTip");
                string aux = string.Empty;
                foreach (DataRow item in dtResult.Rows)
                {
                    aux = item["urlImage"].ToString().ToLower();

                    if (aux.Contains("rojo"))
                        item["ToolTip"] = "Por encima del costo acordado.";
                    else if (aux.Contains("verde"))
                        item["ToolTip"] = "Igual que el costo acordado.";
                    else if (aux.Contains("azul"))
                        item["ToolTip"] = "Por debajo del costo acordado.";
                    else { item["ToolTip"] = string.Empty; }
                }

                Rep9.Controls.Add(
                    DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                            DTIChartsAndControls.GridViewWithImage("RepTarifaGlobal", dtResult, false, "", new string[] { "", "{0:c}", "", "" },
                            new int[] { 2 }, new int[] { 3 }, new int[] { 4 }, 2, false), "ReporteTarifaGlobalUltMes", "Tarifas " + maxFechaTarifas,
                            true, "btnExportarXLS_Click", "btnVolumetria", this, false));
            }
        }

        private void ReporteRentaGlobalUltMes()
        {
            DataTable dtResult = DSODataAccess.Execute(GetRepRentasUltMes());
            string maxFechaRentas = string.Empty;

            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                maxFechaRentas = dtResult.Rows[0]["MaxFechaRentas"].ToString();
                maxFechaRentas = MonthName(Convert.ToInt32(maxFechaRentas.Substring(4, 2)) - 12) + " " + maxFechaRentas.Substring(0, 4) + " ";

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

                Rep10.Controls.Add(
                    DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                            DTIChartsAndControls.GridView("RepRentaGlobal", dtResult, false, "",
                            new string[] { "", "{0:c}" }, 2, false), "ReporteRentaGlobalUltMes", "Rentas " + maxFechaRentas, "", false));
            }
        }

        private void ReporteRecursoUltMes(GroupBy agrupado)
        {
            DataTable dtResult = DSODataAccess.Execute(GetRepRecursosUltMes());
            string maxFechaRecursos = string.Empty;

            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                maxFechaRecursos = dtResult.Rows[0]["MaxFechaRecursos"].ToString();
                maxFechaRecursos = MonthName(Convert.ToInt32(maxFechaRecursos.Substring(4, 2)) - 12) + " " + maxFechaRecursos.Substring(0, 4) + " ";

                if (DSODataContext.Schema.ToLower() != "sperto" || agrupado != GroupBy.Concepto)
                {
                    DataView dvldt = new DataView(dtResult);
                    dtResult = dvldt.ToTable(false, new string[] { "Tipo Recurso", "Cantidad" });
                    dtResult.AcceptChanges();

                    Rep11.Controls.Add(
                        DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                                DTIChartsAndControls.GridView("RepTotalRecursos", dtResult, true, "Totales",
                                new string[] { "", "{0:0,0}" }, 2, false), "ReporteRecursoUltMes", "Recursos " + maxFechaRecursos, "", false));
                }
                else if(agrupado == GroupBy.Concepto)
                {
                    //NZ 20180227 Se solicito que en la DEMO este reporte mostrará mes actual y el anterior.
                    //Dado que esta información no cambia y la prisa con la que solicita se quedaran fijos los datos.

                    dtResult.Columns.Add("NOV 2016", Type.GetType("System.Int32"));
                    if (carrierSelected == "374")
                    {
                        dtResult.Rows[0]["NOV 2016"] = 512;
                        dtResult.Rows[1]["NOV 2016"] = 536;
                        dtResult.Rows[2]["NOV 2016"] = 312;
                        dtResult.Rows[3]["NOV 2016"] = 303;
                    }
                    else
                    {
                        dtResult.Rows[0]["NOV 2016"] = 3130;
                        dtResult.Rows[1]["NOV 2016"] = 131;
                        dtResult.Rows[2]["NOV 2016"] = 10;
                        dtResult.Rows[3]["NOV 2016"] = 1740;
                    }
                    dtResult.Columns["Cantidad"].ColumnName = "DIC 2016";

                    DataView dvldt = new DataView(dtResult);
                    dtResult = dvldt.ToTable(false, new string[] { "Tipo Recurso", "NOV 2016", "DIC 2016" });
                    dtResult.AcceptChanges();

                    Rep11.Controls.Add(
                        DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                                DTIChartsAndControls.GridView("RepTotalRecursos", dtResult, true, "Totales",
                                new string[] { "", "{0:0,0}", "{0:0,0}" }, 2, false), "ReporteRecursoUltMes", "Recursos " + maxFechaRecursos, "", false));
                }
            }
        }

        private void ReporteConsumoLlamsMinUltMes()
        {
            DataTable dtResult = DSODataAccess.Execute(GetConsumoLlamsMinUltMes());
            string maxFechaLlamsMinUltMes = string.Empty;

            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                maxFechaLlamsMinUltMes = dtResult.Rows[0]["MaxFechaCarga"].ToString();
                maxFechaLlamsMinUltMes = MonthName(Convert.ToInt32(maxFechaLlamsMinUltMes.Substring(4, 2))) + " " + maxFechaLlamsMinUltMes.Substring(0, 4) + " ";

                DataView dvldt = new DataView(dtResult);
                dtResult = dvldt.ToTable(false, new string[] { "Destino", "Llamadas", "Minutos", "Total" });
                dtResult.AcceptChanges();

                Rep12.Controls.Add(
                    DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                            DTIChartsAndControls.GridView("RepConsumoLlamMin", dtResult, true, "Totales",
                            new string[] { "", "{0:0,0}", "{0:0,0}", "{0:c}" }, 2, false), "ReporteConsumoLlamsMinUltMes", "Consumos " + maxFechaLlamsMinUltMes, "", false));
            }
        }

        private void ReporteEmpresas()
        {
            DataTable dtResult = ConvertToDataTable(dtEmpres);
            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtResult);
                dtResult = dvldt.ToTable(false, new string[] { "Empre", "EmpreDesc" });
                dtResult.Columns["EmpreDesc"].ColumnName = "Empresas";
                dtResult.AcceptChanges();

                GridView gvResultEmpre = DTIChartsAndControls.GridView("RepEmpresas", dtResult, false, "",
                    new string[] { "", "", "" }, Request.Path + "?Empre={0}", new string[] { "Empre" },
                    0, new int[] { 0 }, new int[] { }, new int[] { 1 }, 2, false);

                gvResultEmpre.RowDataBound += (sender, e) => AgregarHiperlink_RowDataBound(sender, e, "~/images/TIMEmpre", 1);
                gvResultEmpre.DataBind();
                gvResultEmpre.UseAccessibleHeader = true;
                gvResultEmpre.HeaderRow.TableSection = TableRowSection.TableHeader;

                Rep8.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvResultEmpre, "RepEmpresas", "Empresas", "", false));
            }
        }

        private void ReporteConsumoGlobalPorXTabsComparativos(GroupBy agrupado, Control contenedor)
        {
            Control[] tablasDatos = new Control[3];
            string[] titulosRep = new string[3];
            
            titulosRep[0] = "Por " + agrupado.ToString();
            RepTabPanelPorXAnioActual(agrupado, ref tablasDatos, 0);

            RepTabPanelPorXAhorroMesActVsMesAnt(agrupado, ref tablasDatos, ref titulosRep, 1);

            RepTabPanelPorXAhorroMismoMesAnioActVsMesAnioAnt(agrupado, ref tablasDatos, ref titulosRep, 2);

            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepTablasTIM(tablasDatos, titulosRep, "RepTabsGeneral" + agrupado.ToString(), "Consumos " + maxFechaAnio, 0,
                true, "btnExportarXLS_Click", "btnConsumoPor" + agrupado.ToString(), this, false));
        }

        #endregion Reportes


        #region Botones y funcionalidades de apoyo

        private string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper();
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            LinkButton boton = sender as LinkButton;
            ExportXLS(".xlsx", boton.ID.Replace("btn", ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldt"></param>
        /// <param name="tipo">Especificar si es: "Concepto" o "Carrier"</param>
        /// <param name="nomColumna"></param>
        /// <returns></returns>
        public string JavaScriptDrillDown(DataTable ldt, string tipo, string nomColumna)
        {
            tipo = tipo.Replace(" ", "");
            StringBuilder lsb = new StringBuilder();

            lsb.Append("<script type=\"text/javascript\">\n ");
            lsb.Append("function DrillDown" + tipo + "s(" + tipo + "){\n ");
            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("if(" + tipo + " == '" + ldr[nomColumna].ToString() + "'){");
                DataTable DataSource = DSODataAccess.Execute(GetConsumoPorX2Anios(Convert.ToInt32(ldr[nomColumna])));
                if (DataSource.Rows.Count > 0 && DataSource.Columns.Count > 0)
                {
                    DataView dvDataSource = new DataView(DataSource);
                    DataSource = dvDataSource.ToTable(false,
                        new string[] { "Mes", "AÑO ANTERIOR", "AÑO ACTUAL" });
                    DataSource.AcceptChanges();
                }
                string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(DataSource));

                lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(DataSource),
                                  "DetalleConsumo" + tipo, "Detalle consumo " + ldr["label"].ToString(), "",
                                  "MES", "IMPORTE", "msline", "$", "", "dti", "98%", "280", false));
                lsb.Append("}");
            }
            lsb.Append("}\n ");
            lsb.Append("</script>\n ");
            return lsb.ToString();
        }

        /// <summary>
        /// Cambia propiedades del color de la fila del gridview, [(Valor < 0) = rojo], [(Valor > 0) = verde], [(Valor = 0) = gris]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="columnaValor">Nombre de columna que contiene valor a validar</param>
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

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            //carrierSelected = ddlCarrier.SelectedValue;
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
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por Concepto " + maxFechaAnio);

                        DataTable RepConsumoPorConcepto = DSODataAccess.Execute(GetConsumoPorXAnioActual(GroupBy.Concepto));
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

                    DataTable RepConsumoPorConcepto = DSODataAccess.Execute(GetConsumoPorXAnioActual(GroupBy.Carrier));
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

                    DataTable RepConsumoPorConcepto = DSODataAccess.Execute(GetConsumoPorXAnioActual(GroupBy.Empresa));
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
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Volumetría " + maxFechaMesLetra);

                        #region Tarifas
                        DataTable dtResult = DSODataAccess.Execute(GetComparacionTarifasUltMes());
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
                        dtResult = DSODataAccess.Execute(GetRepRentasUltMes());
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
                        dtResult = DSODataAccess.Execute(GetRepRecursosUltMes());
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
                        dtResult = DSODataAccess.Execute(GetConsumoLlamsMinUltMes());
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

        #endregion

    }

    public class InfoCarrierGlobal
    {
        public int iCodCatalogo { get; set; }
        public string vchDescripcion { get; set; }
        public int MaxFecha { get; set; }
        public int Año { get; set; }
        public int Mes { get; set; }
        public string MesNombre { get; set; }
        public int Empre { get; set; }
        public string EmpreDesc { get; set; }

        public string IdsCarrierEmpre { get; set; }
        public string NomCarrierEmpre { get; set; }
    }
}