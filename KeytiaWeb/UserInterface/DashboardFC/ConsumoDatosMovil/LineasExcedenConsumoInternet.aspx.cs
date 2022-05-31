using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class LineasExcedenConsumoInternet : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        string icodLinea = "";
        string tipoConsumo = "";
        string linea = "";
        string tit = "";
        string col = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            tipoConsumo = Request.QueryString["TipoExc"];
            icodLinea = Request.QueryString["Linea"];
            linea = Request.QueryString["NumLinea"];

            try
            {
                //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
                (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();
                (Master as KeytiaOH).ChangeCalendar(false);   //Hace que la pagina tenga un solo calendario.
                if (!Page.IsPostBack && ((Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")))
                {
                    if (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")
                    {
                        #region Inicia los valores default de los controles de fecha
                        try
                        {
                            CalculaFechasDeDashboard();

                        }
                        catch (Exception ex)
                        {
                            throw new KeytiaWebException(
                                "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                                + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                        }
                        #endregion Inicia los valores default de los controles de fecha
                    }
                }
                else
                {
                    //En este caso que es Moviles, aquí se ven meses completos, así que si se selecciona una fecha 
                    //diferente el primer mes, se mueven internamente las fechas para que siempre sea mes completo.
                    #region Fecha mes completo
                    if (Session["FechaInicio"] != null && Session["FechaInicio"] != "")
                    {
                        DateTime fechaAux = Convert.ToDateTime(Session["FechaInicio"].ToString());

                        fechaInicio = new DateTime(fechaAux.Year, fechaAux.Month, 1);
                        fechaFinal = fechaInicio.AddMonths(-5);

                        Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                        Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
                    }
                    #endregion
                }

                if (Page.IsPostBack)
                {
                    Session["LineasExcedentes"] = null;
                }

                if (!ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
                {
                    tit = (tipoConsumo == "Nac") ? "Líneas exceden consumo de internet nacional en los últimos 6 meses" : "Líneas con consumo de internet internacional en los últimos 6 meses";
                    col = (tipoConsumo == "Nac") ? "Cantidad de veces que excedio su limite" : "Cantidad de veces que presento consumo internacional";
                    IniciaProceso();
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);

            }
        }
        private void CalculaFechasDeDashboard()
        {
            DateTime ldtFechaInicio = new DateTime();
            string lsfechaInicio = DSODataAccess.ExecuteScalar(ConsultaFechaMaximaDeDetallFactCDR()).ToString();
            if (DateTime.TryParse(lsfechaInicio, out ldtFechaInicio))
            {
                //NZ 20150319 Se establecio que siempre se mostrara el primer dia de cada mes.
                fechaInicio = new DateTime(ldtFechaInicio.Year, ldtFechaInicio.Month, 1);
                fechaFinal = fechaInicio.AddMonths(-5);
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
        }
        public string ConsultaFechaMaximaDeDetallFactCDR()
        {
            StringBuilder lsb = new StringBuilder();
            //RM 20161214 Se modifico la consulta para que regrese una fecha por default, el primero del mes actual en dado caso de no encontrar una 
            lsb.Append("select isNull(Max(FechaInicio),'" + DateTime.Now.ToString("yyyy-MM-01 00:00:00") + "') as FechaInicio \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleFacturaCDR','Español')]  \n ");
            lsb.Append("where Carrier = 373 \n ");
            return lsb.ToString();
        }
        public void IniciaProceso()
        {
            ObtieneExcedenteLineas();
            if (icodLinea != null)
            {
                ObtieneConsumoConceptos();
                ObtieneConsumoConceptoHistorico();
            }

        }
        private void ObtieneExcedenteLineas()
        {
            DataTable dt = new DataTable();
            if (Session["LineasExcedentes"] != null && icodLinea == null && linea == null) { Session["LineasExcedentes"] = null; }

            if (Session["LineasExcedentes"] != null)
            {
                dt = (DataTable)Session["LineasExcedentes"];
            }
            else
            {
                dt = (tipoConsumo == "Nac") ? DSODataAccess.Execute(GetMatrizNacional()) : DSODataAccess.Execute(GetMatrizInterNacional());
                Session["LineasExcedentes"] = null;
                Session["LineasExcedentes"] = dt;
            }


            if (dt != null && dt.Rows.Count > 0)
            {
                dt = DTIChartsAndControls.ordenaTabla(dt, "CantidadExcedido Desc");
                dt.Columns["Linea"].ColumnName = "Línea";
                dt.Columns["CantidadExcedido"].ColumnName = col;
                dt.AcceptChanges();

                Control tabla = DTIChartsAndControls.GridView(
                          "RepTabExcedeNac_T", dt, true, "Totales",
                          new string[] { "", "", "", "{0:0,0}", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "" }, Request.Path + "?Nav=LineasExcedenConsumoInternet.aspx&TipoExc=" + tipoConsumo + "&Linea={0}&NumLinea={1}",
                          new string[] { "icodCatLinea", "Línea" }, 1,
                          new int[] { 0 }, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, new int[] { 1 });

                Tab1Rep1.Controls.Add(DTIChartsAndControls.TituloY2RowsTablaGrafica(tabla, "RepTabExcedeNac_Tabla", tit, "", true));

                //Tab1Rep1.Controls.Add(
                //  DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                //      DTIChartsAndControls.GridView(
                //          "RepTabExcedeNac_T", dt, true, "Totales",
                //          new string[] { "", "", "", "{0:0,0}", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "", "{0:c}", "" }, Request.Path + "?Nav=LineasExcedenConsumoInternet.aspx&TipoExc=" + tipoConsumo + "&Linea={0}&NumLinea={1}",
                //          new string[] { "icodCatLinea", "Línea" }, 1,
                //          new int[] { 0 }, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, new int[] { 1 }),
                //          "RepTabExcedeNac_T", tit, "")
                //);


            }
        }

        private void ObtieneConsumoConceptos()
        {
            DataTable dt = DSODataAccess.Execute(GetMatrizConsumoConceptoLinea());
            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt.Columns["Servicio"].ColumnName = "APP";
                dt.Columns["GBinternet"].ColumnName = "GB Consumidos";
                dt.Columns["GBConsumidos"].ColumnName = "% Consumo";
                //dt = dvldt.ToTable(false, new string[] { "Servicio", "GBConsumidos" });
                //if (dt.Rows[dt.Rows.Count - 1]["Servicio"].ToString() == "Totales")
                //{
                //    dt.Rows[dt.Rows.Count - 1].Delete();
                //}
                //dt.Columns["Servicio"].ColumnName = "label";
                //dt.Columns["GBConsumidos"].ColumnName = "value";
                dt.AcceptChanges();
                Tab1Rep3.Controls.Add(DTIChartsAndControls.TituloYPestañasRep1Nvl(
                          DTIChartsAndControls.GridView("RepConsumoConceptoLineatGrid", dt, true, "Totales",
                         new string[] { "", "", "" }), "RepConsumoConceptoLineatGrid", "GB Consumidos " + linea, "", 2, FCGpoGraf.TabularBaCoDoTa)
                          );

                dt = dvldt.ToTable(false, new string[] { "APP", "% Consumo" });
                if (dt.Rows[dt.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt.Rows[dt.Rows.Count - 1].Delete();
                }
                dt.Columns["APP"].ColumnName = "label";
                dt.Columns["% Consumo"].ColumnName = "value";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoLineatGrid",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dt, "value desc", 10)), "RepConsumoConceptoLineatGrid", "", "",
                    "APP", "% Consumo", 2, FCGpoGraf.TabularBaCoDoTa, " % "), false);

                //Tab1Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("RepConsumoConceptoLineaGraf_G", "GB Consumidos"));

                //ScriptManager.RegisterStartupScript(this, typeof(Page), "RepConsumoConceptoLineaGraf_G",
                //    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt),
                //    "RepConsumoConceptoLineaGraf_G", linea, "", "Servicio", "GB Consumidos", "doughnut2d"), false);
            }
            else
            {
                Tab1Rep3.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepConsumoConceptoHistLineaGraf_G", "GB Consumidos " + linea, "", false));
            }


        }
        private void ObtieneConsumoConceptoHistorico()
        {

            DataTable ldt1 = DSODataAccess.Execute(GetMatrizConsumoHistoricoLinea(1));
            DataTable ldt2 = DSODataAccess.Execute(GetMatrizConsumoHistoricoLinea());
            if (ldt1 != null && ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
            {

                //Tab1Rep4.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("RepConsumoConceptoHistLineaGraf_G", "Historico GB Consumidos"));
                //string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ldt1));
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoHistLineaGraf_G",
                //    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt1), "RepConsumoConceptoHistLineaGraf_G",
                //    linea, "", "Servicio", "GB Consumidos", "stackedcolumn2d","","%"), false);

                Tab1Rep4.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                    DTIChartsAndControls.GridView("RepConsumoConceptoHistLineaGraf_G", ldt1, true, "Totales",
                    new string[] { "", "", "", "", "", "", "", "", "", "", "", "" }),
                    "RepConsumoConceptoHistLineaGraf_G", "Historico GB Consumidos " + linea, "", 3, FCGpoGraf.MatricialConStack)
                    );



                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ldt2));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoHistLineaGraf_G",
                    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt1), "RepConsumoConceptoHistLineaGraf_G",
                    "", "", "APP", "GB Consumidos", 3, FCGpoGraf.MatricialConStack,"","%"), false);
            }
            else
            {
                Tab1Rep4.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepConsumoConceptoHistLineaGraf_G", "Historico GB Consumidos " + linea, "", false));
            }

        }
        private string GetMatrizNacional(int TipoDatoPre = 0)
        {
            string sp = "EXEC dbo.ObtieneGastoExcedenteNacional @Esquema = '{0}',@FechaIni ='{1}',@FechaFin='{2}',@iCodUsuario = {3},@iCodPerfil={4},@Moneda= '{5}',@TipoDatoPre={6}";
            string query = string.Format(sp, DSODataContext.Schema, HttpContext.Current.Session["FechaFin"].ToString(), HttpContext.Current.Session["FechaInicio"].ToString(), iCodUsuario, iCodPerfil, HttpContext.Current.Session["Currency"].ToString(), TipoDatoPre);
            return query;
        }
        private string GetMatrizInterNacional(int TipoDatoPre = 0)
        {

            string sp = "EXEC dbo.ObtieneGastoExcedenteInternacional @Esquema = '{0}',@FechaIni ='{1}',@FechaFin='{2}',@iCodUsuario = {3},@iCodPerfil={4},@Moneda= '{5}',@TipoDatoPre={6}";
            string query = string.Format(sp, DSODataContext.Schema, HttpContext.Current.Session["FechaFin"].ToString(), HttpContext.Current.Session["FechaInicio"].ToString(), iCodUsuario, iCodPerfil, HttpContext.Current.Session["Currency"].ToString(), TipoDatoPre);
            return query;
        }
        private string GetMatrizConsumoConceptoLinea()
        {
            string sp = "EXEC dbo.DesgloceConceptoInternet @Esquema = '{0}',@FechaIni ='{1}',@FechaFin='{2}',@icodLinea={3},@TipoConsumo ='{4}'";
            string query = string.Format(sp, DSODataContext.Schema, HttpContext.Current.Session["FechaFin"].ToString(), HttpContext.Current.Session["FechaInicio"].ToString(), icodLinea, tipoConsumo);
            return query;
        }
        private string GetMatrizConsumoHistoricoLinea(int t = 0)
        {
            string sp = "EXEC dbo.DesgloceConceptoHistorico @Esquema = '{0}',@FechaIni ='{1}',@FechaFin='{2}',@icodLinea={3},@TipoConsumo ='{4}',@TipoDesglose = {5}";
            string query = string.Format(sp, DSODataContext.Schema, HttpContext.Current.Session["FechaFin"].ToString(), HttpContext.Current.Session["FechaInicio"].ToString(), icodLinea, tipoConsumo,t);
            return query;
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + ".xlsx");
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");
                ExportacionExcelRep.ProcesarTituloExcel(lExcel, tit);
                DataTable dt = (tipoConsumo == "Nac") ? DSODataAccess.Execute(GetMatrizNacional(1)) : DSODataAccess.Execute(GetMatrizInterNacional(1));
                if (dt != null && dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("icodCatLinea");
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");
                }

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + ".xlsx");
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;
                lExcel.SalvarComo();

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reportes" + "_" + tit + "_");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrExportTo", ex, ".xlsx");
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