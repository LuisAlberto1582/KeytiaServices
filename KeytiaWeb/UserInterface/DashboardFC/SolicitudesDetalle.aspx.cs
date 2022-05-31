using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class SolicitudesDetalle : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int iCodUsuario;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
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
            DataTable dt = new DataTable();
            StringBuilder query = new StringBuilder();
            gvDetails.DataSource = null;
            gvDetails.DataBind();

            query.AppendLine(" SELECT");
            query.AppendLine(" Att.NombreReporte,");
            query.AppendLine(" convert(varchar,Att.FechaReg, 120) as FechaReg,");
            query.AppendLine(" Car.iCodCatalogo,");
            query.AppendLine(" REPLACE(Car.EstCargaDesc, 'Carga', 'Solicitud') as Estatus,");
            query.AppendLine(" Att.Email,");
            query.AppendLine(" convert(varchar,Att.FechaEnvio, 120) as FechaEnvio");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[vishistoricos('AtribCargaExportaDetalleCDR','Atributos de carga exporta detalleCDR','Español')] Att");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('Cargas','Cargas Genericas','Español')]  Car");
            query.AppendLine(" ON Car.iCodCatalogo = Att.Cargas");
            query.AppendLine("WHERE Att.dtIniVigencia <> Att.dtFinVigencia");
            query.AppendLine("	AND Att.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Car.iCodUsuario = " + iCodUsuario);
            query.AppendLine(" ORDER BY FechaReg DESC");

            dt = DSODataAccess.Execute(query.ToString(), connStr);

            if (dt.Rows.Count == 0)
            {
                lblMensaje.Visible = true;
                lblMensaje.Text = "No hay Información para Mostrar";
                return;
            }
            else
            {
                lblMensaje.Visible = false;
                lblMensaje.Text = "";
            }

            gvDetails.DataSource = dt;
            gvDetails.DataBind();
        }

        protected void lnkVerDetalle_Click(object sender, EventArgs e)
        {
            LinkButton lnkbtn = sender as LinkButton;
            GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;
            string iCodCatalogo = gvDetails.DataKeys[gvrow.RowIndex].Value.ToString();
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT");
            query.AppendLine(" FechaInicioPeriodo,");
            query.AppendLine(" FechaFinPeriodo,");
            query.AppendLine(" SitioDesc,");
            query.AppendLine(" EmpleDesc,");
            query.AppendLine(" CenCosDesc,");
            query.AppendLine(" GpoEtiquetaDesc,");
            query.AppendLine(" CarrierDesc,");
            query.AppendLine(" TDestDesc,");
            query.AppendLine(" LocaliDesc,");
            query.AppendLine(" NumeroTelf,");
            query.AppendLine(" Extension,");
            query.AppendLine(" CodAut,");
            query.AppendLine(" FiltroCostoParaDetalle,");
            query.AppendLine(" FiltroDuracionParaDetalle");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[vishistoricos('AtribCargaExportaDetalleCDR','Atributos de carga exporta detalleCDR','Español')]");
            query.AppendLine(" WHERE Cargas = " + iCodCatalogo);
            DataRow dr = DSODataAccess.ExecuteDataRow(query.ToString(), connStr);
            DateTime fechaInicial = DateTime.Parse(dr["FechaInicioPeriodo"].ToString());
            DateTime fechaFinal = DateTime.Parse(dr["FechaFinPeriodo"].ToString());
            string Ubicacion = dr["SitioDesc"].ToString();
            string Empleado = dr["EmpleDesc"].ToString();
            string CenCos = dr["CenCosDesc"].ToString();
            string TipoLlam = dr["GpoEtiquetaDesc"].ToString();
            string Carrier = dr["CarrierDesc"].ToString();
            string TipoDest = dr["TDestDesc"].ToString();
            string Localidad = dr["LocaliDesc"].ToString();
            string Telefono = dr["NumeroTelf"].ToString();
            string Extension = dr["Extension"].ToString();
            string Codigo = dr["CodAut"].ToString();
            string Costo = dr["FiltroCostoParaDetalle"].ToString();
            string Duracion = dr["FiltroDuracionParaDetalle"].ToString();
            lblDetalle.Text = "<table Class=\"Detalle\"><tr><td Style=\"background-color: #EEEEEE;\">Período</td><td>" + fechaInicial.ToString("MMMM dd, yyyy") + " - " + fechaFinal.ToString("MMMM dd, yyyy") + "</td></tr>";
            if (!String.IsNullOrEmpty(Ubicacion))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Ubicación</td><td>" + Ubicacion + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Empleado))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Empleado</td><td>" + Empleado + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(CenCos))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Centro de Costos</td><td>" + CenCos + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(TipoLlam))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Categoría Llamada</td><td>" + TipoLlam + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Carrier))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Carrier</td><td>" + Carrier + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(TipoDest))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Tipo Destino</td><td>" + TipoDest + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Localidad))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Localidad</td><td>" + Localidad + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Telefono))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Núm. Marcado</td><td>" + Telefono + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Extension))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Extensión</td><td>" + Extension + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Codigo))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Código</td><td>" + Codigo + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Costo))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Costo</td><td>" + Costo + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(Duracion))
            {
                lblDetalle.Text = lblDetalle.Text + "<tr><td Style=\"background-color: #EEEEEE;\">Duración</td><td>" + Duracion + "</td></tr>";
            }
            lblDetalle.Text = lblDetalle.Text + "</table>";
            mpeEtqDetalle.Show();
        }

        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnkbtn = sender as LinkButton;
                GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;
                string iCodCatalogo = gvDetails.DataKeys[gvrow.RowIndex].Value.ToString();
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT LinkParaDescarga");
                query.AppendLine(" FROM " + DSODataContext.Schema + ".[vishistoricos('AtribCargaExportaDetalleCDR','Atributos de carga exporta detalleCDR','Español')]");
                query.AppendLine(" WHERE Cargas = " + iCodCatalogo);
                DataRow dr = DSODataAccess.ExecuteDataRow(query.ToString(), connStr);
                if(!String.IsNullOrEmpty(dr["LinkParaDescarga"].ToString()))
                {
                    string filePath = dr["LinkParaDescarga"].ToString();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(filePath) + "\"");
                    Response.TransmitFile(filePath);
                    Response.End();
                }
                
            }
            catch
            {

                lblMensaje.Visible = true;
                lblMensaje.Text = "Ocurrio un error al Descargar el archivo, puede que el Archivo no se encuentre disponible Favor de contactar al Administrador";
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }
    }
}