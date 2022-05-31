﻿using KeytiaServiceBL;
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
    public partial class RepCencosJerN5 : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string icoEmple = "";
        static string tdest = "";
        static string fechaInicio = "";
        static string fechaFinal = "";
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        static string anio = "";
        static string mes = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;

            fechaInicio = Session["FechaInicioCen"].ToString();
            fechaFinal = Session["FechaFinCen"].ToString();
            lblFechaFact.Text = Session["PeridoFacturado"].ToString();
            anio = Session["AnioSelect"].ToString();
            mes = Session["MesSelect"].ToString();
            if (!Page.IsPostBack)
            {
                IniciaProceso();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionConsumoCencosN5", "ConsumoCencosN5('JSONConsumoCencosN5');", true);
        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONConsumoCencosN5(string iCodEmple,string Tdest)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;
                 icoEmple = iCodEmple;
                 tdest = Tdest;
                string sp = "EXEC [ObtieneConsumoCencosJerN5V2] @Schema = '" + esquema + "', @Usuar = {0},@Perfil = {1}, @Fechainicio = '{2}', @Fechafin = '{3}',@iCodEmple = {4},@Tdest = {5}";
                string query = string.Format(sp, iCodUsuario, iCodPerfil, fechaInicio, fechaFinal, iCodEmple, Tdest);

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

                if (dt.Columns.Contains("Renglon"))
                {
                    dt.Columns["Renglon"].ColumnName = "recid";
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
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportFiles excelFile = new ExportFiles();
            string sp = "EXEC [ObtieneConsumoCencosJerN5V2] @Schema = '" + esquema + "', @Usuar = {0},@Perfil = {1}, @Fechainicio = '{2}', @Fechafin = '{3}',@iCodEmple = {4},@Tdest = {5}";
            string query = string.Format(sp, iCodUsuario, iCodPerfil, fechaInicio, fechaFinal, icoEmple, tdest);
            DataTable dt = DSODataAccess.Execute(query, connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                if (dt.Columns.Contains("Renglon"))
                {
                    dt.Columns.Remove("Renglon");
                }
                if (dt.Columns.Contains("TotalSimulado"))
                {
                    dt.Columns.Remove("TotalSimulado");
                }
                if (dt.Columns.Contains("CostoSimulado"))
                {
                    dt.Columns.Remove("CostoSimulado");
                }
                if (dt.Columns.Contains("CostoReal"))
                {
                    dt.Columns.Remove("CostoReal");
                }
                if (Convert.ToInt32(tdest) == 386)
                {
                    if (dt.Columns.Contains("Nomina")) { dt.Columns.Remove("Nomina"); }
                    if (dt.Columns.Contains("Hora Fin")) { dt.Columns.Remove("Hora Fin"); }
                    if (dt.Columns.Contains("SM")) { dt.Columns.Remove("SM"); }
                    if (dt.Columns.Contains("Tipo Llamada")) { dt.Columns.Remove("Tipo Llamada"); }
                    if (dt.Columns.Contains("Codigo Autorizacion")) { dt.Columns.Remove("Codigo Autorizacion"); }
                    if (dt.Columns.Contains("Localidad")) { dt.Columns.Remove("Localidad"); }
                    dt.Columns["Extensión"].ColumnName = "Línea";
                    dt.Columns["Fecha Fin"].ColumnName = "Periodo Facturacion";
                    dt.Columns["TotalReal"].ColumnName = "Importe";
                    dt.AcceptChanges();

                }
                else
                {
                    if (dt.Columns.Contains("Nomina")) { dt.Columns.Remove("Nomina"); }
                    if (dt.Columns.Contains("Numero Marcado")) { dt.Columns.Remove("Numero Marcado"); }
                    if (dt.Columns.Contains("Hora")) { dt.Columns.Remove("Hora"); }
                    if (dt.Columns.Contains("Hora Fin")) { dt.Columns.Remove("Hora Fin"); }
                    if (dt.Columns.Contains("Fecha")) { dt.Columns.Remove("Fecha"); }
                    if (dt.Columns.Contains("Duracion")) { dt.Columns.Remove("Duracion"); }
                    if (dt.Columns.Contains("Llamadas")) { dt.Columns.Remove("Llamadas"); }
                    if (dt.Columns.Contains("SM")) { dt.Columns.Remove("SM"); }
                    if (dt.Columns.Contains("Tipo Llamada")) { dt.Columns.Remove("Tipo Llamada"); }
                    if (dt.Columns.Contains("Codigo Autorizacion")) { dt.Columns.Remove("Codigo Autorizacion"); }
                    if (dt.Columns.Contains("Localidad")) { dt.Columns.Remove("Localidad"); }
                    dt.Columns["Extensión"].ColumnName = "Línea";
                    dt.Columns["Fecha Fin"].ColumnName = "Periodo de Fcatura";
                    dt.Columns["TotalReal"].ColumnName = "Importe";
                    dt.AcceptChanges();
                }
            }
            
            string file = excelFile.GeneraArchivoExcel("ReDetallCencosJerN5_" + DateTime.Now.ToString("hhmmss"), fechaInicio, fechaFinal, dt, dt, 4, 4);
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
                Session["FechaInicioCen"] = fechaInicio;
                Session["FechaFinCen"] = fechaFinal;
                DateTime periodoFac = Convert.ToDateTime(fechaFinal);
                Session["PeridoFacturado"] = periodoFac.ToString("MMMM").ToUpper() + " DE " + periodoFac.ToString("yyyy");
                lblFechaFact.Text = periodoFac.ToString("MMMM").ToUpper() + " DE " + periodoFac.ToString("yyyy");
                Session["AnioSelect"] = anio;
                Session["MesSelect"] = cboMes.SelectedItem.ToString();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionConsumoCencosN5", "ConsumoCencosN5('JSONConsumoCencosN5');", true);
            }

        }
        private void IniciaProceso()
        {
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
            cboAnio.Items.FindByText(anio).Selected = true;
            cboMes.Items.FindByText(mes).Selected = true;
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
            query.AppendLine(" SELECT iCodCatalogo AS vchCodigo, vchDescripcion AS Descripcion FROM " + esquema + ".[VisHistoricos('Anio','Años','Español')]");
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
    }
}