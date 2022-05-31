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
    public partial class LineasExcedenPlanBase : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        string fechaI = string.Empty;
        string fechaF = string.Empty;

        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
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
                    IniciaProcess();
                }

                if (Session["FechaInicio"].ToString() != fechaI && Session["FechaInicio"].ToString() != fechaF)
                {
                    IniciaProcess();

                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }
        private void IniciaProcess()
        {
            if (rbtnConsolidado.Checked == true)
            {
                ObtieneLineas();
            }
            else
            {
                ObtieneConceptos();
            }
        }
        private void ObtieneLineas()
        {
            Tab1Rep1.Visible = true;
            Tab1Rep2.Visible = false;
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT");
            query.AppendLine("iCodCatalogo,");
            query.AppendLine("REPLACE(vchDescripcion,' ','') AS vchDescripcion,");
            query.AppendLine("MesDesc,");
            query.AppendLine("AnioDesc,");
            query.AppendLine("EstCargaDesc,");
            query.AppendLine("Archivo01,");
            query.AppendLine("Archivo02,");
            query.AppendLine("Archivo03,");
            query.AppendLine("Archivo04");
            query.AppendLine("FROM " + esquema + ".[vishistoricos('Cargas','ETL Factura Telcel','Español')] WITH(NOLOCK)");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if( dt != null && dt.Rows.Count> 0)
            {
                grvLineasPlan.DataSource = dt;
                grvLineasPlan.DataBind();
                grvLineasPlan.UseAccessibleHeader = true;
                grvLineasPlan.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
        private void ObtieneConceptos()
        {
            Tab1Rep2.Visible = true;
            Tab1Rep1.Visible = false;

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

        protected void rbtnConsolidado_CheckedChanged(object sender, EventArgs e)
        {
            ObtieneLineas();
        }

        protected void rbtnConceptos_CheckedChanged(object sender, EventArgs e)
        {
            ObtieneConceptos();
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExcelAccess lExcel = new ExcelAccess();
            DataTable dt = null;
            DataTable dt1 = null;
            string fileName = "";
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteFacturacion" + ".xlsx");
                lExcel.Abrir();
                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consolidado por Linea", "Resumen");
                if (dt != null && dt.Rows.Count > 0)
                {
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Resumen", "Tabla");
                }

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desglose de Conceptos", "InformacionGeneral");
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt1, 0, "Totales"), "InformacionGeneral", "Tabla");
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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "ReporteFacturacion_" + fileName + "_");
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