using KeytiaServiceBL;
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
    public partial class RepCencosJer : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string fechaInicio = "";
        static string fechaFinal = "";
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();

            if (!Page.IsPostBack)
            {
                Response.Write("<script type='text/javascript'> sessionStorage.clear();</script>");
                Session["FechaInicioCen"] = null;
                Session["FechaFinCen"] = null;
                Session["PeridoFacturado"] = null;
                Session["AnioSelect"] = null;
                Session["MesSelect"] = null;
                IniciaProceso();
                ObtieneFechaFact();
            }

                Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionConsumoCencos", "ConsumoCencos('JSONConsumoCencos');", true);
        }
        private void IniciaProceso()
        {
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONConsumoCencos()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                string sp = "exec [ObtieneConsumoCencosJerV2] @Schema = '" + esquema + "', @CenCos = {0}, @Usuar = {1},@Perfil = {2}, @Fechainicio = '{3}', @Fechafin = '{4}'";
                string query = string.Format(sp, 200028, iCodUsuario, iCodPerfil, fechaInicio, fechaFinal);

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

                if (dt.Columns.Contains("iCodCatalogo"))
                {
                    dt.Columns["iCodCatalogo"].ColumnName = "recid";
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
        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
            if(Convert.ToInt32(mes) > 0)
            {
                DateTime F = Convert.ToDateTime((anio + "-" + mes + "-" + "01"));
                DateTime fecha2 = F.AddMonths(1).AddDays(-1);
                fechaInicio = F.ToString("yyyy-MM-dd 00:00:00");
                fechaFinal = fecha2.ToString("yyyy-MM-dd 23:59:59");
                Session["FechaInicioCen"] = fechaInicio;
                Session["FechaFinCen"] = fechaFinal;
                DateTime periodoFac = Convert.ToDateTime(fechaFinal);
                Session["PeridoFacturado"] = periodoFac.ToString("MMMM").ToUpper() + " DE " + periodoFac.ToString("yyyy");
                lblFechaFact.Text = periodoFac.ToString("MMMM").ToUpper() + " DE " + periodoFac.ToString("yyyy");
                Session["AnioSelect"] = anio;
                Session["MesSelect"] = cboMes.SelectedItem.ToString();
            }

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportFiles excelFile = new ExportFiles();

            string sp = "exec [ObtieneConsumoCencosJerV2] @Schema = '" + esquema + "', @CenCos = {0}, @Usuar = {1},@Perfil = {2}, @Fechainicio = '{3}', @Fechafin = '{4}'";
            string query = string.Format(sp, 200028, iCodUsuario, iCodPerfil, fechaInicio, fechaFinal);
            DataTable dt = DSODataAccess.Execute(query, connStr);
            string file = excelFile.GeneraArchivoExcel("ReCencosJerN1_" + DateTime.Now.ToString("hhmmss"), fechaInicio, fechaFinal, dt, dt, 1,1);
            ExportFiles(file);          
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
        private void ObtieneFechaFact()
        {
            string query = "EXEC ObtieneFechaMaximaFactura @Schema = '"+ esquema + "',@Carrier = 'TELMEX'";
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                fechaInicio = dr["FechaInicio"].ToString() + " 00:00:00";
                fechaFinal = dr["FechaFin"].ToString() + " 23:59:59";
                lblFechaFact.Text = dr["PeriodoFac"].ToString();
                ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                ValidaSelectCombo(dr["Mes"].ToString(),cboMes);                
                Session["FechaInicioCen"] = dr["FechaInicio"].ToString() + " 00:00:00";
                Session["FechaFinCen"] = dr["FechaFin"].ToString() + " 23:59:59";
                Session["PeridoFacturado"] = dr["PeriodoFac"].ToString();
                Session["AnioSelect"] = dr["Anio"].ToString();
                Session["MesSelect"] = dr["Mes"].ToString();
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
        private void ValidaSelectCombo(string valor,DropDownList cbo)
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
    }
}