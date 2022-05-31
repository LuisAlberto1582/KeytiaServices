using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepSevenEleven
{
    public partial class RepConsolidado : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string fechaInicio = "";
        static string fechaFinal = "";
        static string fechaFac = "";
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            if (!Page.IsPostBack)
            {
                IniciaProceso();
                ObtieneFechaFact();
            }     
            ObtieneInfoSiana();
        }
        private void ObtieneInfoSiana()
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
           
            if (rbtLineas.Checked == true)
            {
                fechaFac = anio + mes;
                line.Visible = true;
                ctaMaestra.Visible = false;
                tipoDest.Visible = false;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionConsumoSianaConsolidado", "ConsumoSianaConsolidado('JSONConsumoSianaConsolidado');", true);
            }
            else if (rbtnCtaMaestra.Checked == true)
            {
                fechaFac = anio+"-"+mes+"-"+"01";
                line.Visible = false;
                tipoDest.Visible = false;
                ctaMaestra.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionConsumoCtaMaestra", "ConsumoCtaMaestra('JSONConsumoCtaMaestra');", true);
            }
            else if(rbtnTipoServicio.Checked == true)
            {
                line.Visible = false;
                ctaMaestra.Visible = false;
                tipoDest.Visible = true;
                //ObtieneTipoDest();
                DateTime F = Convert.ToDateTime((anio + "-" + mes + "-" + "01"));
                DateTime fecha2 = F.AddMonths(1).AddDays(-1);
                fechaInicio = F.ToString("yyyy-MM-dd 00:00:00");
                fechaFinal = fecha2.ToString("yyyy-MM-dd 23:59:59");

                Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionConsumoTipoDestino", "ConsumoTipoDestino('JSONConsumoTipoDestino');", true);
            }
        }
        private void IniciaProceso()
        {
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
        private void ObtieneFechaFact()
        {
            string query = "EXEC ObtieneFechaMaximaFactura @Schema = '" + esquema + "',@Carrier = 'TELMEX'";
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                //fechaInicio = dr["FechaInicio"].ToString() + " 00:00:00";
                //fechaFinal = dr["FechaFin"].ToString() + " 23:59:59";
                //lblFechaFact.Text = dr["PeriodoFac"].ToString();
                ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                ValidaSelectCombo(dr["Mes"].ToString(), cboMes);
                //Session["FechaInicioCen"] = dr["FechaInicio"].ToString() + " 00:00:00";
                //Session["FechaFinCen"] = dr["FechaFin"].ToString() + " 23:59:59";
                //Session["PeridoFacturado"] = dr["PeriodoFac"].ToString();
                //Session["AnioSelect"] = dr["Anio"].ToString();
                //Session["MesSelect"] = dr["Mes"].ToString();
                ObtieneInfoSiana();
            }
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

        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
            if (Convert.ToInt32(mes) > 0)
            {
                DateTime F = Convert.ToDateTime((anio + "-" + mes + "-" + "01"));
                DateTime fecha2 = F.AddMonths(1).AddDays(-1);
                fechaInicio = F.ToString("yyyy-MM-dd 00:00:00");
                fechaFinal = fecha2.ToString("yyyy-MM-dd 23:59:59");
                //Session["FechaInicioCen"] = fechaInicio;
                //Session["FechaFinCen"] = fechaFinal;
                DateTime periodoFac = Convert.ToDateTime(fechaFinal);
                //Session["PeridoFacturado"] = periodoFac.ToString("MMMM").ToUpper() + " DE " + periodoFac.ToString("yyyy");
                //lblFechaFact.Text = periodoFac.ToString("MMMM").ToUpper() + " DE " + periodoFac.ToString("yyyy");
                //Session["AnioSelect"] = anio;
                //Session["MesSelect"] = cboMes.SelectedItem.ToString();
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionConsumoCencosN3", "ConsumoCencosN3('JSONConsumoCencosN3');", true);
                if (rbtLineas.Checked == true)
                {

                }
                else if (rbtnCtaMaestra.Checked == true)
                {

                }
            }
        }

        #region WeMethod Json
        [WebMethod]
        public static string JSONConsumoSianaConsolidado()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                string sp = "EXEC [dbo].ObtieneConsumoSianaConsolidado @Schema = '" + esquema + "', @FechaFac = '{0}',@Usuario={1},@Perfil = {2}";
                
                string query = string.Format(sp, fechaFac, iCodUsuario, iCodPerfil);

                dt = DSODataAccess.Execute(query, connStr);

                #region elimina Nulos
                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (dataRow[column] == DBNull.Value)
                        {
                            dataRow[column] = "";
                        }
                    }
                }
                #endregion
                #region Cambia Nombre Columnas

                if (dt.Columns.Contains("RowNumber"))
                {
                    dt.Columns["RowNumber"].ColumnName = "recid";
                }
                #endregion

                List<string> customers = new List<string>();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);

                }

                return "{\"records\":" + serializer.Serialize(rows) + "}";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [WebMethod]
        public static string JSONConsumoCtaMaestra()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                string sp = "EXEC [dbo].ObtieneConsumoCtaMestra @Schema='" + esquema + "', @FechaFac='{0}', @Usuario={1}, @Perfil={2}"; 
                string query = string.Format(sp, fechaFac, iCodUsuario, iCodPerfil);

                dt = DSODataAccess.Execute(query, connStr);

                #region elimina Nulos
                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (dataRow[column] == DBNull.Value)
                        {
                            dataRow[column] = "";
                        }
                    }
                }
                #endregion
                #region Cambia Nombre Columnas

                if (dt.Columns.Contains("CtaMaestra"))
                {
                    dt.Columns["CtaMaestra"].ColumnName = "recid";
                }
                #endregion

                List<string> customers = new List<string>();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);

                }

                return "{\"records\":" + serializer.Serialize(rows) + "}";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [WebMethod]
        public static string JSONConsumoTipoDestino()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;
                DataTable dt = ResFinalTdest();

                #region elimina Nulos
                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (dataRow[column] == DBNull.Value)
                        {
                            dataRow[column] = "";
                        }

                    }
                }
                #endregion
                #region Cambia Nombre Columnas

                if (dt.Columns.Contains("Id"))
                {
                    dt.Columns["Id"].ColumnName = "recid";
                }
                #endregion

                List<string> customers = new List<string>();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);

                }

                return "{\"records\":" + serializer.Serialize(rows) + "}";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        private  static DataTable ResFinalTdest()
        {
            DataTable dt = ObtieneTipoDest();
            return dt;
        }
        private static DataTable ObtieneTipoDest()
        {               
            string sp = "EXEC [dbo].RepTipoDestinoSiana @Schema = '" + esquema + "', @FechaInicio='{0}',@FechaFin='{1}',@Usuario={2},@Perfil={3}";
            string query = string.Format(sp, fechaInicio, fechaFinal, iCodUsuario, iCodPerfil);
            DataTable dt = DSODataAccess.Execute(query, connStr);
            DataTable dtF = TiposDest(dt);

            return dtF;
        }
        private static DataTable TiposDest(DataTable dt)
        {
            List<ConsumoTdest> listTdest = new List<ConsumoTdest>();
            listTdest.Clear();
            DataTable dtFinal = new DataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoTdest dest = new ConsumoTdest();
                    dest.Empleado = dr["EMPLEADO"].ToString();
                    dest.CencosAbuelo = dr["CENCOSABUELO"].ToString();
                    dest.CencosPadre = dr["CENCOSPADRE"].ToString();
                    dest.CencosEmple = dr["CENCOSEMPLE"].ToString();
                    dest.CtaMaestra = dr["CtaM"].ToString();
                    dest.Destino = dr["DESTINO"].ToString();
                    dest.Importe = Convert.ToDouble(dr["IMPORTE"]);
                    listTdest.Add(dest);
                }
                /*creamos las columnas del DataTable*/
                dtFinal.Columns.Add("Id");
                dtFinal.Columns.Add("Tienda-Linea");
                dtFinal.Columns.Add("Division");
                dtFinal.Columns.Add("Mercado");
                dtFinal.Columns.Add("Tienda-Mercado");
                dtFinal.Columns.Add("Cuenta Maestra");

                /*Obtenemos un agrupado de los tipos destino para generar las columnas*/
                IEnumerable<ConsumoTdest> listFinal = listTdest.GroupBy(c => c.Destino).Select(group => group.First());
                foreach (var item in listFinal)
                {
                    string nomColumn = item.Destino.ToString().TrimEnd('.');
                    if(nomColumn != "" && nomColumn!= string.Empty)
                    {
                        dtFinal.Columns.Add(nomColumn);
                    }                  
                }
                dtFinal.Columns.Add("Total");
                
                /*Obtenemos un agrupado de los empleados*/
                IEnumerable<ConsumoTdest>listEmple= listTdest.GroupBy(c => c.Empleado).Select(group => group.First());
                int id = 1;
                foreach (var item in listEmple)
                {
                    DataRow row = dtFinal.NewRow();
                    var emple = item.Empleado;
                    double totGral = 0;
                    row["Id"] = id;
                    row["Tienda-Linea"] = item.Empleado;
                    row["Division"] = item.CencosAbuelo;
                    row["Mercado"] = item.CencosPadre;
                    row["Tienda-Mercado"] = item.CencosEmple;
                    row["Cuenta Maestra"] = item.CtaMaestra;

                    /*Obtiene los tipos destino del emplado */
                    var lt = listTdest.Where(x => x.Empleado == emple);
                    foreach (var item1 in lt)
                    {
                        double importe = 0;
                        var tpdest = item1.Destino.TrimEnd('.');
                        if(tpdest != null && tpdest != "")
                        {
                            importe = item1.Importe;
                            row[tpdest] = importe;
                        }

                        totGral += importe;
                    }
                    
                    row["Total"] = totGral;
                    dtFinal.Rows.Add(row);

                    id++;
                }                
            }
            return dtFinal;
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportFiles excelFile = new ExportFiles();
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
            string sp = "";
            string query="";
            string nomFile = "";
            DateTime F = Convert.ToDateTime((anio + "-" + mes + "-" + "01"));
            DateTime fecha2 = F.AddMonths(1).AddDays(-1);
            fechaInicio = F.ToString("yyyy-MM-dd 00:00:00");
            DataTable dt = new DataTable();

            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + ".xlsx");
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                if (rbtLineas.Checked == true)
                {
                    nomFile = "ConsumoLineas_";
                    fechaFac = anio + mes;
                    line.Visible = true;
                    ctaMaestra.Visible = false;
                    tipoDest.Visible = false;
                    sp = "EXEC [dbo].ObtieneConsumoSianaConsolidado @Schema = '" + esquema + "', @FechaFac = '{0}',@Usuario={1},@Perfil = {2}";
                    query = string.Format(sp, fechaFac, iCodUsuario, iCodPerfil);
 
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo de Lineas");
                    DataTable ldt = DSODataAccess.Execute(query);
                    if (ldt.Rows.Count > 0)
                    {
                        ldt.Columns.Remove("RowNumber");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                    }
                }
                else if (rbtnCtaMaestra.Checked == true)
                {
                    nomFile = "ConsumoPorCuentaMaestra_";
                    fechaFac = anio + "-" + mes + "-" + "01";
                    line.Visible = false;
                    tipoDest.Visible = false;
                    ctaMaestra.Visible = true;
                    sp = "EXEC [dbo].ObtieneConsumoCtaMestra @Schema = '" + esquema + "', @FechaFac = '{0}'";
                    query = string.Format(sp, fechaFac);
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Cuenta Maestra");
                    DataTable ldt = DSODataAccess.Execute(query);
                    if (ldt.Rows.Count > 0)
                    {
                        ldt.Columns.Remove("CtaMaestra");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                    }
                }
                else if (rbtnTipoServicio.Checked == true)
                {
                    nomFile = "ConsumoPorTipoDestino_";
                    line.Visible = false;
                    ctaMaestra.Visible = false;
                    tipoDest.Visible = true;

                    fechaFinal = fecha2.ToString("yyyy-MM-dd 23:59:59");

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Tipo Destino");
                    DataTable ldt = ResFinalTdest();
                    if (ldt.Rows.Count > 0)
                    {
                        ldt.Columns.Remove("Id");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                    }
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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reportes" + "_" + nomFile);
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

            //string file = excelFile.GeneraArchivoExcel(nomFile + DateTime.Now.ToString("hhmmss"), fechaInicio, fechaFinal, dt, dt, 5, 5);
            //ExportFiles(lsFileName);
        }
        public void ExportFiles(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var buffer = File.ReadAllBytes(filePath);
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("Content-Type", "application/octet-stream");
                    Response.AddHeader("Content-disposition", "attachment; filename=\"" + Path.GetFileName(filePath) + "\"");
                    Response.BinaryWrite(buffer);
                    Response.ContentType = "application/octet-stream";
                    Response.Flush();
                    buffer = null;

                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                throw new KeytiaWebException("Ocurrio un error en : " + ex);

            }
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }
    }
    public class ConsumoTdest
    {
        public string Empleado { get; set; }
        public string CencosEmple { get; set; }
        public string CencosPadre { get; set; }
        public string CencosAbuelo { get; set; }
        public string Destino { get; set; }
        public string CtaMaestra { get; set; }
        public double Importe { get; set; }
    }
}