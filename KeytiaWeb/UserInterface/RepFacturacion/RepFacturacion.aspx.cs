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

namespace KeytiaWeb.UserInterface.RepFacturacion
{
    public partial class RepFacturacion : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        string fechaI = string.Empty;
        string fechaF = string.Empty;
        string carrier = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                IniciaProceso();
                ObtieneFechaFact();
                GetCarrier();
            }

            ObtieneDatos();
        }
        private void ObtieneDatos()
        {
            ObtieneFechas();

            if (rbtnResumen.Checked ==true)
            {
                Tab1Rep2.Visible = false;
                Tab1Rep1.Visible = true;
                ObtieneResumen(fechaI, fechaF, carrier);

            }
            else
            {
                Tab1Rep2.Visible = true;
                Tab1Rep1.Visible = false;
                ObtieneInfGeneral(fechaI, fechaF, carrier);
            }                        
        }
        private void ObtieneFechas()
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
            carrier = cboCarrier.SelectedValue.ToString();

            var mes1 = (mes.Length == 1) ? "0" + mes : mes;
            string fecValida = anio + "-" + mes1 + "-" + "01";

            DateTime feFin = Convert.ToDateTime(fecValida);
            DateTime fecIni = feFin.AddMonths(-11);
            int mesIni = fecIni.Month + 12;
            int mesFin = feFin.Month + 12;
            fechaI = fecIni.Year.ToString() + mesIni;
            fechaF = feFin.Year.ToString() + mesFin;
        }
        private void ObtieneResumen(string fecIni, string fecFin,string carrier)
        {
            DataTable dt = DSODataAccess.Execute(ObtieneResumenGen(fecIni, fecFin, carrier), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                Tab1Rep1.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabResumenGen_T", dt, true, "Totales",
                   new string[] { "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }),
                   "RepTabResumenGen_T", "")
                            );
            }
        }
        private void ObtieneInfGeneral(string fecIni, string fecFin, string carrier)
        {
            DataTable dt = DSODataAccess.Execute(ObtieneServicios(fecIni, fecFin, carrier), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                Tab1Rep2.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTServicios_T", dt, true, "Totales",
                   new string[] { "", "", "", "", "{0:c}" }),
                   "RepTabTServicios_T", "")
                            ); ;
            }
        }
        private void ObtieneFechaFact()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @Fecha INT,@MES VARCHAR(5),@FechaBusqueda VARCHAR(10)");
            query.AppendLine(" SELECT");
            query.AppendLine(" @Fecha = MAX(FechaInt)");
            query.AppendLine(" FROM " + esquema + ".TIMConsolidadoPorClaveCargo WITH(NOLOCK)");
            query.AppendLine(" SET @MES = CONVERT(VARCHAR, RIGHT(@Fecha, 2) - 12)");
            query.AppendLine(" SET @FechaBusqueda = LEFT(@Fecha, 4) + '-' + CASE WHEN LEN(@MES)= 1 THEN '0' + @MES ELSE @MES END + '-01'");
            query.AppendLine(" SELECT @FechaBusqueda AS FEC,ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaBusqueda), N'MMMM', 'es-MX')),'') AS Mes,ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaBusqueda), N'yyyy', 'es-MX')),'') AS Anio");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                ValidaSelectCombo(dr["Mes"].ToString(), cboMes);
            }
        }
        private void IniciaProceso()
        {
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
        private void GetCarrier()
        {
            DataTable dt = DSODataAccess.Execute(ObtieneCarriers(),connStr);
            if(dt != null && dt.Rows.Count >0)
            {
                cboCarrier.DataSource = null;
                cboCarrier.DataSource = dt;
                cboCarrier.DataValueField = "iCodCatalogo";
                cboCarrier.DataTextField = "vchDescripcion";
                cboCarrier.DataBind();
            }

        }
        private string ObtieneCarriers()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" CREATE TABLE #Carriers");
            query.AppendLine(" (");
            query.AppendLine(" iCodCatalogo INT,");
            query.AppendLine(" vchDescripcion VARCHAR(100)");
            query.AppendLine(" )");
            query.AppendLine(" INSERT INTO #Carriers VALUES(0,'TODOS')");
            query.AppendLine(" INSERT INTO #Carriers");
            query.AppendLine(" SELECT");
            query.AppendLine(" DISTINCT");
            query.AppendLine(" C.iCodCatalogo,");
            query.AppendLine(" UPPER(C.vchDescripcion)");
            query.AppendLine(" FROM ["+DSODataContext.Schema+"].TIMConsolidadoPorClaveCargo AS TIM WITH(NOLOCK)");
            query.AppendLine(" JOIN [" + DSODataContext.Schema + "].HistCarrier AS C WITH(NOLOCK)");
            query.AppendLine(" ON TIM.iCodCatCarrier = C.iCodCatalogo");
            query.AppendLine(" AND dtIniVigencia<> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" SELECT* FROM #Carriers");
            return query.ToString();
        }
        private DataTable GetDataDropDownList(string clave)
        {
            ObtieneAnio();
            StringBuilder query = new StringBuilder();
            bool isEstatus = false;
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");
            query = query.Replace("[CAMPOS]", "CASE WHEN LEN(VCHCODIGO) = 1 THEN '0' + VCHCODIGO ELSE VCHCODIGO END AS vchCodigo, UPPER(Español) AS Descripcion");
            query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
            return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
        }
        private DataTable AddRowDefault(DataTable dt, bool estatus)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow rowExtra = dt.NewRow();
                rowExtra["vchCodigo"] = 0;
                rowExtra["Descripcion"] = !estatus ? "TODOS" : "Seleccionar";
                dt.Rows.InsertAt(rowExtra, 0);
            }
            return dt;
        }
        public void ObtieneAnio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo AS vchCodigo, vchDescripcion AS Descripcion FROM " + esquema + ".[VisHistoricos('Anio','Años','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion IN(DATEPART(YEAR, GETDATE()),DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())), DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())))");
            query.AppendLine(" ORDER BY vchDescripcion DESC");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                cboAnio.DataSource = dt;
                cboAnio.DataBind();
            }
        }
        private void ValidaSelectCombo(string valor, DropDownList cbo)
        {
            string itemToCompare = string.Empty;
            string itemOrigin = valor.ToUpper();
            foreach (ListItem item in cbo.Items)
            {
                itemToCompare = item.Text.ToUpper();
                if (itemOrigin == itemToCompare)
                {
                    cbo.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        private string ObtieneResumenGen(string fecIni, string fecFin, string carrier)
        {
            string sp = "EXEC [dbo].ObtieneResumenGeneralTIM @Esquema = '{0}',@FechaIni = {1},@FechaFin = {2},@Carrier = {3}";
            string query = string.Format(sp, DSODataContext.Schema, fecIni, fecFin, carrier);
            return query.ToString();
        }
        private string ObtieneServicios(string fecIni, string fecFin, string carrier)
        {
            string sp = "EXEC [dbo].ObtieneDetalleServicioTIM @Esquema = '{0}',@FechaIni = {1},@FechaFin = {2},@Carrier = {3}";
            string query = string.Format(sp, DSODataContext.Schema, fecIni, fecFin, carrier);
            return query.ToString();
        }
        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {

        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ObtieneFechas();
            DataTable dt = DSODataAccess.Execute(ObtieneResumenGen(fechaI, fechaF, carrier), connStr);
            DataTable dt1 = DSODataAccess.Execute(ObtieneServicios(fechaI, fechaF, carrier), connStr);
            ExcelAccess lExcel = new ExcelAccess();
            string fileName = cboMes.SelectedItem.ToString() + "-" + cboAnio.SelectedItem.ToString();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteFacturacion" + ".xlsx");
                lExcel.Abrir();
                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Resumen General", "Resumen");
                if(dt != null && dt.Rows.Count > 0)
                {
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Resumen", "Tabla");
                }

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Informacion General", "InformacionGeneral");
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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "ReporteFacturacion_"+ fileName+"_");
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