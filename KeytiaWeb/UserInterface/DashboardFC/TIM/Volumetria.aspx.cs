using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using AjaxControlToolkit;
using System.Text;
using KeytiaServiceBL;
using System.Globalization;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Drawing;
using System.Web.Services;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    public partial class Volumetria : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();
        string maxFechaAnio = "";
        bool hayConsumo = false;

        string carrierSelected = "-1";
        string empreSelected = "-1";
        List<InfoCarrierGlobal> dtCarriers = new List<InfoCarrierGlobal>();
        List<InfoCarrierGlobal> dtEmpres = new List<InfoCarrierGlobal>();

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
                    //carriers.Insert(0, new InfoCarrierGlobal() { iCodCatalogo = -1, vchDescripcion = "TODOS" });
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
                if (!ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
                {
                    if (ddlCarrier.Items.Count > 0)
                    {
                        TabPanelUno.Visible= false;
                        TabPanelDos.Visible = false;

                        Tab1Rep1.Controls.Clear();
                        Tab1Rep2.Controls.Clear();
                        Tab1Rep3.Controls.Clear();
                        Tab1Rep4.Controls.Clear();

                        Tab2Rep1.Controls.Clear();
                        Tab2Rep2.Controls.Clear();
                        Tab2Rep3.Controls.Clear();
                        Tab2Rep4.Controls.Clear();


                        DashboardPrincipal();
                    }
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
                lblCarrierConsumo.Text = "Volumetría";


                string nomCarrier = ddlCarrier.SelectedItem.Text;

                if (nomCarrier.ToUpper() != "CONCEPTO MOVIL" && nomCarrier.ToUpper() != "MARCATEL" )
                {
                    //Reporte del total de llamadas del año actual y el año anterior.
                    ReporteTotalLlams();

                    //Reporte del total de minutos del año actual y el año anterior.
                    ReporteTotalMin();

                    //Reporte del total de recurso del año actual y el año anterior.
                    ReporteTotalRecursos();
                }
                else
                {
                    //Reporte del total de llamadas del año actual y el año anterior.
                    ReporteTotalMensajes();
                    
                }
            }
        }

        #region Consultas

        public void GetCarriersTIM(int iCodCarrier, int iCodCatEmpre)
        {
            dtCarriers = new ConsumoGeneral().GetCarriersTIM(iCodCarrier, iCodCatEmpre, "Volumetria");

            List<InfoCarrierGlobal> listaCarriersMensajes = new ConsumoGeneral().GetCarriersTIM(iCodCarrier, iCodCatEmpre, "Volumetria-Mensajes");

            dtCarriers.AddRange(listaCarriersMensajes);
        }

        private string GetMatrizTotalLlamMin(string campoAConsiderar)
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMVolumetriaMatLlamsMin2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @CampoAConsiderar = '" + campoAConsiderar + "'");
            return query.ToString();
        }


        private string GetMatrizTotalMensajes(string campoAConsiderar)
        {
            query.Length = 0;
            query.AppendLine("EXEC [TIMVolumetriaMatMensajes2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @CampoAConsiderar = '" + campoAConsiderar + "'");
            return query.ToString();
        }

        private string GetMatrizRecursos()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [TIMVolumetriaMatInventarioRecursos2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected);
            return query.ToString();
        }

        private string GetLlamsMinPorTDest2Años(string tdest, string colAGarf)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [TIMVolumetriaLlamsMinPorDestino2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @TDestDesc = '" + tdest + "',");
            query.AppendLine("     @CampoASum = '" + colAGarf + "'");
            return query.ToString();
        }

        private string GetMensajesPorTDest2Años(string tdest, string colAGarf)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [TIMVolumetriaMensajesPorDestino2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @TDestDesc = '" + tdest + "',");
            query.AppendLine("     @CampoASum = '" + colAGarf + "'");
            return query.ToString();
        }

        private string GetInvPorRecurso2Años(string recurso)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [TIMVolumetriaInventarioPorRecurso2Anios] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("     @iCodCatEmpre = " + empreSelected + ",");
            query.AppendLine("     @iCodCatCarrier = " + carrierSelected + ",");
            query.AppendLine("     @iCodCatRecursoContratado = " + recurso + "");
            return query.ToString();
        }

        #endregion

        #region Reportes

        public void ReporteTotalLlams()
        {
            DataTable dtResult = new DataTable();

            dtResult = DSODataAccess.Execute(GetMatrizTotalLlamMin("TotalLlamadas"));

            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                hayConsumo = true;
                TabPanelUno.Visible = true;

                List<string> formatCol = new List<string>();
                List<int> colMostrar = new List<int>();

                formatCol.Add("");

                for (int i = 1; i < dtResult.Columns.Count; i++)
                {
                    formatCol.Add("{0:0,0}");
                    colMostrar.Add(i);
                }

                GridView gvRepLlamsTDest = DTIChartsAndControls.GridView(
                                                                            "RepLlamsTDest",
                                                                            dtResult,
                                                                            true,
                                                                            "Totales",
                                                                            formatCol.ToArray(),
                                                                            "javascript.DrillDownDestinoTotalLlamadas('{0}');",
                                                                            new string[] { "Destino" },
                                                                            0,
                                                                            new int[] { },
                                                                            colMostrar.ToArray(),
                                                                            new int[] { 0 },
                                                                            2,
                                                                            false
                                                                         );

                gvRepLlamsTDest.RowDataBound += (sender, e) => CambiaURLFunctionJava_RowDataBound(sender, e);
                gvRepLlamsTDest.DataBind();
                gvRepLlamsTDest.UseAccessibleHeader = true;
                gvRepLlamsTDest.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepLlamsTDest.Rows[gvRepLlamsTDest.Rows.Count - 1];
                row.Cells[0].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                Tab1Rep1.Controls.Add(
                                        DTIChartsAndControls.TituloYBordeRepSencilloTabla
                                                    (
                                                        gvRepLlamsTDest,
                                                        "RepLlamsTDest",
                                                        "Reporte de Llamadas " + maxFechaAnio,
                                                        "",
                                                        false
                                                    )
                                     );

                //--------------------------------------------------------------------------------------------------------
                DataView dvDtResult = new DataView(dtResult);
                dtResult = dvDtResult.ToTable(false, new string[] { "Destino" });

                Tab1Rep3.Controls.Add
                                    (
                                        DTIChartsAndControls.TituloYBordeRepSencilloGrafica
                                                            (
                                                                "GrafPorDestinoTotalLlamadas",
                                                                "Gráfica de Llamadas " + maxFechaAnio
                                                            )
                                    );

                Page.ClientScript.RegisterStartupScript
                            (
                                this.GetType(),
                                "CambiaGraficaDrillDownDestinoLlams",
                                javaScriptDrillDownLlamsMin
                                        (
                                            dtResult,
                                            "Destino",
                                            "TotalLlamadas",
                                            "TOTAL LLAMADAS",
                                            "Destino",
                                            false
                                         ),
                                false
                           );

                //Selecciona el Primer Elemento
                ScriptManager.RegisterStartupScript(
                                                        this,
                                                        typeof(Page),
                                                        "funcionLlamada",
                                                        "javascript:DrillDownDestinoTotalLlamadas('" +
                                                                                                    dtResult.Rows[0][0].ToString() +
                                                                                                "'); document.getElementById('" + TabPanelUno.ClientID + "') .style.display='block'; ",
                                                        true);


                TabContainerPrincipal.ActiveTabIndex = 0;
                TabContainerPrincipal.Visible = true;
                TabPanelUno.Visible = true;
            }
            //else
            //{
            //    Tab1Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepLlamsTDest", "Reporte de Llamadas " + maxFechaAnio, "", false));
            //    Tab1Rep2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "GrafPorDestinoTotalLlamadas", "Gráfica de Llamadas " + maxFechaAnio, "", false));
            //}
        }

        public void ReporteTotalMensajes()
        {
            DataTable dtResult = new DataTable();

            dtResult = DSODataAccess.Execute(GetMatrizTotalMensajes("TotalMensajes"));



            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                hayConsumo = true;
                TabPanelUno.Visible = true;

                List<string> formatCol = new List<string>();
                List<int> colMostrar = new List<int>();

                formatCol.Add("");

                for (int i = 1; i < dtResult.Columns.Count; i++)
                {
                    formatCol.Add("{0:0,0}");
                    colMostrar.Add(i);
                }

                GridView gvRepLlamsTDest = DTIChartsAndControls.GridView(
                                                                            "RepMnesajesTDest",
                                                                            dtResult,
                                                                            true,
                                                                            "Totales",
                                                                            formatCol.ToArray(),
                                                                            "javascript.DrillDownDestinoTotalMensajes('{0}');",
                                                                            new string[] { "Destino" },
                                                                            0,
                                                                            new int[] { },
                                                                            colMostrar.ToArray(),
                                                                            new int[] { 0 },
                                                                            2,
                                                                            false
                                                                         );

                gvRepLlamsTDest.RowDataBound += (sender, e) => CambiaURLFunctionJava_RowDataBound(sender, e);
                gvRepLlamsTDest.DataBind();
                gvRepLlamsTDest.UseAccessibleHeader = true;
                gvRepLlamsTDest.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepLlamsTDest.Rows[gvRepLlamsTDest.Rows.Count - 1];
                row.Cells[0].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                Tab1Rep1.Controls.Add(
                                        DTIChartsAndControls.TituloYBordeRepSencilloTabla
                                                    (
                                                        gvRepLlamsTDest,
                                                        "RepMensajesTDest",
                                                        "Reporte de Mensajes " + maxFechaAnio,
                                                        "",
                                                        false
                                                    )
                                     );

                //--------------------------------------------------------------------------------------------------------
                DataView dvDtResult = new DataView(dtResult);
                dtResult = dvDtResult.ToTable(false, new string[] { "Destino" });


                Tab1Rep2.Controls.Clear();
                Tab1Rep2.Controls.Add
                                    (
                                        DTIChartsAndControls.TituloYBordeRepSencilloGrafica
                                                            (
                                                                "GrafPorDestinoTotalMensajes",
                                                                "Gráfica de Mensajes " + maxFechaAnio
                                                            )
                                    );

                Page.ClientScript.RegisterStartupScript
                            (
                                this.GetType(),
                                "CambiaGraficaDrillDownDestinoMensajes",
                                javaScriptDrillDownLlamsMin
                                        (
                                            dtResult,
                                            "Destino",
                                            "TotalMensajes",
                                            "TOTAL MENSAJES",
                                            "Destino",
                                            false
                                         ),
                                false
                           );

                //Selecciona el Primer Elemento
                ScriptManager.RegisterStartupScript
                                                    (
                                                        this,
                                                        typeof(Page),
                                                        "funcionMensaje",
                                                        "javascript:DrillDownDestinoTotalMensajes('" + dtResult.Rows[0][0].ToString() + "');",
                                                        true
                                                    );
            }
            //else
            //{
            //    Tab1Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepLlamsTDest", "Reporte de Llamadas " + maxFechaAnio, "", false));
            //    Tab1Rep2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "GrafPorDestinoTotalLlamadas", "Gráfica de Llamadas " + maxFechaAnio, "", false));
            //}
        }

        public void ReporteTotalMin()
        {
            DataTable dtResult = DSODataAccess.Execute(GetMatrizTotalLlamMin("Minutos"));
            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                hayConsumo = true;
                TabPanelUno.Visible = true;
                List<string> formatCol = new List<string>();
                List<int> colMostrar = new List<int>();
                formatCol.Add("");
                for (int i = 1; i < dtResult.Columns.Count; i++)
                {
                    formatCol.Add("{0:0,0}");
                    colMostrar.Add(i);
                }

                GridView gvRepMinutosTDest = DTIChartsAndControls.GridView("RepMinutosTDest", dtResult, true, "Totales", formatCol.ToArray(),
                    "javascript.DrillDownDestinoMinutos('{0}');", new string[] { "Destino" }, 0, new int[] { }, colMostrar.ToArray(), new int[] { 0 }, 2, false);

                gvRepMinutosTDest.RowDataBound += (sender, e) => CambiaURLFunctionJava_RowDataBound(sender, e);
                gvRepMinutosTDest.DataBind();
                gvRepMinutosTDest.UseAccessibleHeader = true;
                gvRepMinutosTDest.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepMinutosTDest.Rows[gvRepMinutosTDest.Rows.Count - 1];
                row.Cells[0].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }
                Tab1Rep2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvRepMinutosTDest, "RepMinutosTDest", "Reporte de Minutos " + maxFechaAnio, "", false));

                //--------------------------------------------------------------------------------------------------------
                DataView dvDtResult = new DataView(dtResult);
                dtResult = dvDtResult.ToTable(false, new string[] { "Destino" });

                Tab1Rep4.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafPorDestinoMinutos", "Gráfica de Minutos " + maxFechaAnio));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CambiaGraficaDrillDownDestinoMinutos",
                    javaScriptDrillDownLlamsMin(dtResult, "Destino", "Minutos", "TOTAL MINUTOS", "Destino", false), false);

                //Selecciona el Primer Elemento
                ScriptManager.RegisterStartupScript(
                                                    this,
                                                    typeof(Page),
                                                    "funcionMinutos",
                                                    "javascript:DrillDownDestinoMinutos('" + 
                                                                                            dtResult.Rows[0][0].ToString() +
                                                                                        "'); document.getElementById('" + TabPanelUno.ClientID + "') .style.display='block';",
                                                    true);

                TabContainerPrincipal.ActiveTabIndex = 0;
                TabContainerPrincipal.Visible = true;
                TabPanelUno.Visible = true;
            }
            //else
            //{
            //    Tab1Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepMinutosTDest", "Reporte de Minutos " + maxFechaAnio, "", false));
            //    Tab1Rep4.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "GrafPorDestinoMinutos", "Gráfica de Minutos " + maxFechaAnio, "", false));
            //}
        }

        public string javaScriptDrillDownLlamsMin(DataTable ldt, string nomColId, string nomColumGraficar, string tituloGrafEje, string nomColMostrar, bool isRecurso)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("<script type=\"text/javascript\">\n ");
            lsb.Append("function DrillDown" + nomColId + nomColumGraficar + "(Concepto){\n ");
            foreach (DataRow ldr in ldt.Rows)
            {
                if (ldr[nomColId].ToString().ToLower() != "totales")
                {
                    lsb.Append("if(Concepto == '" + ldr[nomColId].ToString() + "'){");

                    DataTable dtConsulta = null;
                    if (!isRecurso)
                    {
                        if (nomColumGraficar.ToUpper() != "TOTALMENSAJES")
                        {
                            dtConsulta = DSODataAccess.Execute(GetLlamsMinPorTDest2Años(ldr[nomColId].ToString(), nomColumGraficar));
                        }
                        else
                        {
                            dtConsulta = DSODataAccess.Execute(GetMensajesPorTDest2Años(ldr[nomColId].ToString(), nomColumGraficar));
                        }
                         
                    }
                    else
                    {
                        dtConsulta = DSODataAccess.Execute(GetInvPorRecurso2Años(ldr[nomColId].ToString()));
                    }

                    if (dtConsulta.Rows.Count > 0 && dtConsulta.Columns.Count > 0)
                    {
                        DataView dvDataSource = new DataView(dtConsulta);
                        dtConsulta = dvDataSource.ToTable(false,
                            new string[] { "Mes", "AÑO ANTERIOR", "AÑO ACTUAL" });
                        dtConsulta.AcceptChanges();
                    }
                    string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dtConsulta));

                    lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(dtConsulta),
                                       "GrafPor" + nomColId + nomColumGraficar, ldr[nomColMostrar].ToString(), "",
                                       "MESES", tituloGrafEje, "msline", "", "", "dti", "98%", "385", false));
                    lsb.Append("}");
                }
            }
            lsb.Append("}\n ");
            lsb.Append("</script>\n ");
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

        public void ReporteTotalRecursos()
        {
            DataTable dtResult = DSODataAccess.Execute(GetMatrizRecursos());
            if (dtResult.Rows.Count > 0 && dtResult.Columns.Count > 0)
            {
                TabPanelDos.Visible = true;
                List<string> formatCol = new List<string>();
                List<int> colMostrar = new List<int>();
                formatCol.Add("");
                formatCol.Add("");
                formatCol.Add("");
                for (int i = 3; i < dtResult.Columns.Count; i++)
                {
                    formatCol.Add("{0:0,0}");
                    colMostrar.Add(i);
                }

                GridView gvRepTotRecursos = DTIChartsAndControls.GridView("RepRecurso", dtResult, true, "Totales", formatCol.ToArray(),
                    "javascript.DrillDowniCodCatRecursoContratadoRecurso('{0}');", new string[] { "iCodCatRecursoContratado" }, 2, new int[] { 0, 1 }, colMostrar.ToArray(), new int[] { 2 }, 2, false);

                gvRepTotRecursos.RowDataBound += (sender, e) => CambiaURLFunctionJava_RowDataBound(sender, e);
                gvRepTotRecursos.DataBind();
                gvRepTotRecursos.UseAccessibleHeader = true;
                gvRepTotRecursos.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = gvRepTotRecursos.Rows[gvRepTotRecursos.Rows.Count - 1];
                row.Cells[2].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }
                Tab2Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gvRepTotRecursos, "RepRecurso", "Reporte de Recursos " + maxFechaAnio, "", false));

                //--------------------------------------------------------------------------------------------------------
                DataView dvDtResult = new DataView(dtResult);
                dtResult = dvDtResult.ToTable(false, new string[] { "iCodCatRecursoContratado", "Recurso" });

                Tab2Rep2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("GrafPoriCodCatRecursoContratadoRecurso", "Gráfica de Recursos " + maxFechaAnio));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "CambiaGraficaDrillDowniCodCatRecursoContratadoRecurso",
                    javaScriptDrillDownLlamsMin(dtResult, "iCodCatRecursoContratado", "Recurso", "TOTAL RECURSOS", "Recurso", true), false);

                if (!hayConsumo) { TabContainerPrincipal.ActiveTabIndex = 1; }

            }
            else
            {
                Tab2Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepMinutosTDest", "Reporte de Recursos " + maxFechaAnio, "", false));
                Tab2Rep2.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "GrafPoriCodCatRecursoContratadoRecurso", "Gráfica de Recursos " + maxFechaAnio, "", false));
            }
        }

        #endregion

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            carrierSelected = ddlCarrier.SelectedValue;
        }

        private string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper();
        }

        protected void ddlEmpre_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetCarriersTIM(-1, int.Parse(ddlEmpre.SelectedItem.Value));
            //Distinct de Carriers
            var carriers = dtCarriers.GroupBy(g => new { g.iCodCatalogo, g.vchDescripcion }).Select(x => x.First()).ToList();
            //carriers.Insert(0, new InfoCarrierGlobal() { iCodCatalogo = -1, vchDescripcion = "TODOS" });

            ddlCarrier.DataSource = null;
            ddlCarrier.DataSource = carriers;
            ddlCarrier.DataValueField = "iCodCatalogo";
            ddlCarrier.DataTextField = "vchDescripcion";
            ddlCarrier.DataBind();
        }

    }
}
