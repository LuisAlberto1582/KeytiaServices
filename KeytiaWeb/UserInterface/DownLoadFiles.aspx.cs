using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace KeytiaWeb.UserInterface
{
    public partial class DownLoadFiles : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        StringBuilder query = new StringBuilder();
        int iCodUsuario;
        protected void Page_Load(object sender, EventArgs e)
        {
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            if (!Page.IsPostBack)
            {
                IniciaProceso();
            }
        }
        private void IniciaProceso()
        {
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
            cboCategoria.DataSource = ObtieneCategoria();
            cboCategoria.DataBind();
        }

        private DataTable GetDataDropDownList(string clave)
        {
            lblMensaje.Text = "";
            bool isEstatus = false;
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");

            #region Filtro
            switch (clave.ToUpper())
            {

                case "ANIO":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Anio','Años','Español')]");
                    query.AppendLine(" AND CONVERT(INT, vchDescripcion) >= 2016 AND CONVERT(INT, vchDescripcion) <= YEAR(GETDATE())");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                case "MES":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Español AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                default:
                    return new DataTable();
            }

            #endregion
        }
        private DataTable ObtieneCategoria()
        {
            System.Text.StringBuilder query = new StringBuilder();
            DataTable dt = new DataTable();

            query.AppendLine(" SELECT ");
            query.AppendLine(" iCodCatalogo,");
            query.AppendLine(" NombreCategoria ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisRelaciones('CategoriaArchivoDesc - Usuario','Español')] AS RELCAT  ");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('CategoriaArchivoDescargable','Categorías Archivo Descargable','Español')] AS CATEGORIA ");
            query.AppendLine(" ON RELCAT.CategoriaArchivoDescargable = CATEGORIA.iCodCatalogo ");
            query.AppendLine(" AND CATEGORIA.dtinivigencia <> CATEGORIA.dtFinVigencia AND CATEGORIA.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE RELCAT.dtIniVigencia<> RELCAT.dtFinVigencia ");
            query.AppendLine(" AND RELCAT.dtFinVigencia >= GETDATE() ");
            query.AppendLine(" AND RELCAT.Usuar = " + iCodUsuario + " ");


            dt = DSODataAccess.Execute(query.ToString(), connStr);

            return dt;
        }
        private DataTable AddRowDefault(DataTable dt, bool estatus)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow rowExtra = dt.NewRow();
                rowExtra["iCodCatalogo"] = 0;
                rowExtra["Descripcion"] = !estatus ? "TODOS" : "Seleccionar";
                dt.Rows.InsertAt(rowExtra, 0);
            }
            return dt;
        }
        private void ObtieneArchivos(int categoria, int anio, int mes)
        {
            DataTable dt = new DataTable();
            StringBuilder query = new StringBuilder();
            gvDetails.DataSource = null;
            gvDetails.DataBind();

            query.AppendLine(" SELECT");
            query.AppendLine(" vchDescripcion,");
            query.AppendLine(" Descripcion,");
            query.AppendLine(" FechaCarga,");
            query.AppendLine(" RutaArchivo");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[Vishistoricos('ArchivoDescargable','Archivos Descargables','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() ");
            query.AppendLine(" AND CategoriaArchivoDescargable = " + categoria + " ");
            if(anio > 0)
            {
                query.AppendLine(" AND Anio = " + anio + " ");

                if(mes > 0)
                {
                    query.AppendLine(" AND Mes = " + mes + " ");
                }
            }

            query.AppendLine(" ORDER BY Anio DESC,Mes DESC ");

            dt = DSODataAccess.Execute(query.ToString(), connStr);

            if(dt!= null && dt.Rows.Count > 0)
            {
                gvDetails.DataSource = dt;
                gvDetails.DataBind();
                gvDetails.UseAccessibleHeader = true;
                gvDetails.HeaderRow.TableSection = TableRowSection.TableHeader;

                ValidaExisteArchivo();

                lblMensaje.Visible = false;
                lblMensaje.Text = "";
                rowBusqueda.Visible = true;
            }
            else
            {
                rowBusqueda.Visible = false;
                lblMensaje.Visible = true;
                lblMensaje.Text = "No hay Información para Mostrar";
                return;
            }

        }

        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnkbtn = sender as LinkButton;
                GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;
                string filePath = gvDetails.DataKeys[gvrow.RowIndex].Value.ToString();
                if(File.Exists(filePath))
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(filePath) + "\"");
                    Response.TransmitFile(filePath);
                    Response.End();
                }
                else
                {
                    lblMensaje.Visible = true;
                    lblMensaje.Text = "Ocurrio un error al Descargar el archivo, puede que el Archivo no se encuentre disponible, Favor de contactar al Administrador.";
                }
               
            }
            catch (Exception ex)
            {

                lblMensaje.Visible = true;
                lblMensaje.Text = "Ocurrio un error al Descargar el archivo, puede que el Archivo no se encuentre disponible Favor de contactar al Administrador";
                /*Ocurrio un error al Descargar el archivo, puede que el Archivo no se encuentre disponible
                 Favor de contactar al Administrador*/
                return;
            }

            gvDetails.UseAccessibleHeader = true;
            gvDetails.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            int categoria = Convert.ToInt32(cboCategoria.SelectedValue);
            int mes = Convert.ToInt32(cboMes.SelectedValue);
            int anio = Convert.ToInt32(cboAnio.SelectedValue);
            if (categoria == 0)
            {
                lblMensaje.Visible = true;
                lblMensaje.Text = "Debe Seleccionar una Categoria";
                return;
            }
            else {
                lblMensaje.Visible = false;
                lblMensaje.Text = "";
            }
            ObtieneArchivos(categoria, anio, mes);
        }

        protected void gvDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string desc = e.Row.Cells[0].Text;
                string pathFile = gvDetails.DataKeys[e.Row.RowIndex].Values[0].ToString();
                if (!File.Exists(pathFile))
                {
                    e.Row.BackColor = Color.LightCoral;
                    e.Row.ForeColor = Color.Black;
                }

            }
        }
        private void ValidaExisteArchivo()
        {
            foreach (GridViewRow row in gvDetails.Rows)
            {
                string pathFile = gvDetails.DataKeys[row.RowIndex].Values[0].ToString();
                if (File.Exists(pathFile))
                {

                }
            }

            //for (int i = 0; i <= gvDetails.Rows.Count; i++)
            //{
            //    string pathFile = gvDetails.DataKeys[i].Values[0].ToString();
            //    if(File.Exists(pathFile))
            //    {
            //        gvDetails.Rows[i].Cells
            //    }
            //}
        }
    }
}