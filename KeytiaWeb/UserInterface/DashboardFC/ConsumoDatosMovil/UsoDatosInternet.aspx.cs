using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.ConsumoDatosMovil
{
    public partial class ConsumoDatosNacional : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        string fechaI = string.Empty;
        string fechaF = string.Empty;

        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        string tipoConsumo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            try
            {
                //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
                fechaI = HttpContext.Current.Session["FechaInicio"].ToString();
                fechaF = HttpContext.Current.Session["FechaFin"].ToString();
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
                        fechaFinal = fechaInicio.AddMonths(1).AddDays(-1);

                        Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                        Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
                    }
                    #endregion
                }

                if (!Page.IsPostBack)
                {
                    LlenaTipoConsumoList();
                    ObtieneLineas();
                }

                //if(pnl1.Visible==false)
                //{
                //    ObtieneLineas();
                //}
                if(Session["FechaInicio"].ToString() != fechaI && Session["FechaInicio"].ToString() != fechaF)
                {
                    ObtieneLineas();
                }


            }
            catch (Exception ex)
            { 
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
            
        }
        private void LlenaTipoConsumoList()
        {
            cboTipo.Items.Clear();
            cboTipo.Items.Insert(0, "Todos");
            cboTipo.Items.Insert(1, "Excedido");
            cboTipo.Items.Insert(2, "No excedido");
        }
        private void ObtieneLineas()
        {
            try
            {
                string tipoInternet = (rbtNcional.Checked == true) ? "Nac" : "Int";
                int tipoBusqueda = cboTipo.SelectedIndex;
                DataTable dt = DSODataAccess.Execute(ConsultaUsoDatos(tipoInternet, tipoBusqueda), connStr);
                if (dt != null && dt.Rows.Count > 0)
                {
                    dt = DTIChartsAndControls.ordenaTabla(dt, "GBExcedido Desc");
                    dt.AcceptChanges();

                    pnl1.Visible = true;
                    pnlInfo.Visible = false;
                    grvLineas.DataSource = dt;
                    grvLineas.DataBind();
                    grvLineas.UseAccessibleHeader = true;
                    grvLineas.HeaderRow.TableSection = TableRowSection.TableHeader;                    
                }
                else
                {
                    pnl1.Visible = false;
                    grvLineas.DataSource = null;
                    grvLineas.DataBind();
                    lblMensajeInfo.Text = "¡No se encontro información para mostrar!";
                    pnlInfo.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ConsultaUsoDatos(string tipoInternet,int tipoBusqueda,int tipoPre = 0)
        {
            string idLinea = (txtLinea.Text!= "" && txtLineaId.Text != "")?txtLineaId.Text.ToString():"0";
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC dbo.ObtieneUsoInternetLineas ");
            query.AppendLine("@Esquema = '"+ DSODataContext.Schema + "', ");
            query.AppendLine("@FechaIni = '"+ HttpContext.Current.Session["FechaInicio"].ToString() + "', ");
            query.AppendLine("@FechaFin = '" + HttpContext.Current.Session["FechaFin"].ToString() + "', ");
            query.AppendLine("@iCodUsuario = "+ iCodUsuario + ", ");
            query.AppendLine("@iCodPerfil = "+ iCodPerfil + ", ");
            query.AppendLine("@icodLinea = "+ idLinea + ", ");
            query.AppendLine("@TipoBusqueda = "+ tipoBusqueda + ", ");
            query.AppendLine("@TipoConsumo = '"+ tipoInternet + "', ");
            query.AppendLine("@Moneda='"+ HttpContext.Current.Session["Currency"].ToString() + "',");
            query.AppendLine("@TipoDatoPre = "+ tipoPre + "");

            return query.ToString();
        }
        private void CalculaFechasDeDashboard()
        {
            DateTime ldtFechaInicio = new DateTime();
            string lsfechaInicio = DSODataAccess.ExecuteScalar(ConsultaFechaMaximaDeDetallFactCDR()).ToString();
            if (DateTime.TryParse(lsfechaInicio, out ldtFechaInicio))
            {
                //NZ 20150319 Se establecio que siempre se mostrara el primer dia de cada mes.
                fechaInicio = new DateTime(ldtFechaInicio.Year, ldtFechaInicio.Month, 1);
                fechaFinal = fechaInicio.AddMonths(1).AddDays(-1);
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
        [WebMethod]
        public static object GetLinea(string texto)
        {
            DataTable dtLinea = new DataTable();
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo AS idLinea, vchDescripcion AS Descripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".HistLinea WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Tel LIKE '%" + texto + "%'");
            dtLinea = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtLinea);
        }

        protected void lnkVerConsumo_Click(object sender, EventArgs e)
        {
            tipoConsumo = (rbtNcional.Checked == true) ? "Nac" : "Int";
            LinkButton lnkbtn = sender as LinkButton;
            int rowIndex = Convert.ToInt32(lnkbtn.Attributes["RowIndex"]);
            GridViewRow selectedRow = (GridViewRow)grvLineas.Rows[rowIndex];
            int icodCatLinea = (int)grvLineas.DataKeys[rowIndex].Values[0];
            string linea= grvLineas.DataKeys[rowIndex].Values[1].ToString();
            ObtieneDatosLinea(icodCatLinea, linea);
        }
        public void ObtieneDatosLinea(int icodLinea,string linea)
        {
            ObtieneConsumoConceptos(icodLinea, linea);

        }
        private void ObtieneConsumoConceptos(int icodLinea,string linea)
        {
            DataTable dt = DSODataAccess.Execute(GetMatrizConsumoConceptoLinea(icodLinea));
            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt.Columns["Servicio"].ColumnName = "APP";
                dt.Columns["GBinternet"].ColumnName = "GB Consumidos";
                dt.Columns["GBConsumidos"].ColumnName = "% Consumo";
                dt.AcceptChanges();

                Rep2.Controls.Add(
                  DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                      DTIChartsAndControls.GridView("RepConsumoDatos_T", dt, true, "Totales",
                          new string[] { "", "", "" }),
                          "RepConsumoDatos_T", "GB Consumidos " + linea, "")
                  );

                dt = dvldt.ToTable(false, new string[] { "APP", "% Consumo" });
                if (dt.Rows[dt.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt.Rows[dt.Rows.Count - 1].Delete();
                }
                dt.Columns["APP"].ColumnName = "label";
                dt.Columns["% Consumo"].ColumnName = "value";

                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("RepConsumoConceptoLineaGraf_G", "GB Consumidos " + linea));
                ScriptManager.RegisterStartupScript(this, typeof(Page), "RepConsumoConceptoLineaGraf_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt),
                    "RepConsumoConceptoLineaGraf_G", "", "", "Servicio", "GB Consumidos", "doughnut2d"), false);
                
                /*esta seccion hace que no se pierda los encabezados fijos*/
                grvLineas.UseAccessibleHeader = true;
                grvLineas.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            else
            {
                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepConsumoConceptoHistLineaGraf_G", "GB Consumidos " + linea, "", false));
            }


        }
        private string GetMatrizConsumoConceptoLinea(int icodLinea)
        {
            string sp = "EXEC dbo.DesgloceConceptoInternet @Esquema = '{0}',@FechaIni ='{1}',@FechaFin='{2}',@icodLinea={3},@TipoConsumo ='{4}'";
            string query = string.Format(sp, DSODataContext.Schema, HttpContext.Current.Session["FechaInicio"].ToString(), HttpContext.Current.Session["FechaFin"].ToString(), icodLinea, tipoConsumo);
            return query;
        }

        protected void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObtieneLineas();
        }

        protected void rbtNcional_CheckedChanged(object sender, EventArgs e)
        {
            ObtieneLineas();
        }

        protected void rbtnInternacional_CheckedChanged(object sender, EventArgs e)
        {
            ObtieneLineas();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            ObtieneLineas();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LlenaTipoConsumoList();
            txtLinea.Text = "";
            txtLineaId.Text = "";
            ObtieneLineas();
        }


        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());

                string idLinea = (txtLinea.Text != "" && txtLineaId.Text != "") ? txtLineaId.Text.ToString() : "0";
                string plantilla = (txtLinea.Text != "" && txtLineaId.Text != "") ? "ReporteDosTablas" : "ReporteTabla";
                string tabla = (txtLinea.Text != "" && txtLineaId.Text != "") ? "Tabla1" : "Tabla";
                tipoConsumo = (rbtNcional.Checked == true) ? "Nac" : "Int";

                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\"+ plantilla + ".xlsx");
                lExcel.Abrir();
                string titulo = (rbtNcional.Checked == true) ? "Consumo datos Nacional" : "Consumo datos InterNacional";
                string tipoInternet = (rbtNcional.Checked == true) ? "Nac" : "Int";
                int tipoBusqueda = cboTipo.SelectedIndex;

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");
                ExportacionExcelRep.ProcesarTituloExcel(lExcel, titulo);
                DataTable dt = DSODataAccess.Execute(ConsultaUsoDatos(tipoInternet, tipoBusqueda,1), connStr);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if(dt.Columns.Contains("iCodCatalogo"))
                    {
                        dt.Columns.Remove("iCodCatalogo");
                    }
                    

                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", tabla);
                }
                if(idLinea != "0")
                {
                    DataTable dt1 = DSODataAccess.Execute(GetMatrizConsumoConceptoLinea(Convert.ToInt32(idLinea)), connStr);

                    DataView dvldt = new DataView(dt1);
                    dt1.Columns["Servicio"].ColumnName = "APP";
                    dt1.Columns["GBinternet"].ColumnName = "GB Consumidos";
                    dt1.Columns["GBConsumidos"].ColumnName = "% Consumo";
                    dt1.AcceptChanges();

                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt1, 0, "Totales"), "Reporte", "Tabla2");

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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reportes" + "_" + titulo + "_");
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