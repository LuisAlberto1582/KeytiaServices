using System;
using System.Web.UI.WebControls;
using KeytiaServiceBL;
using System.Data;
using System.Web;

namespace KeytiaWeb.UserInterface.RepTernium
{
    public partial class ConfigSolicitudRepTernium : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            //DDL Empresa
            if (!Page.IsPostBack)
            {
                ObtenerEmpresas();
            }
        }

        protected void ObtenerEmpresas()
        {
            string strQuery = "select iCodCatalogo, vchDescripcion + ' ' + '(' + vchCodigo + ')' as vchDescripcion from [Ternium].[vishistoricos('Empre','Español')] where  dtFinVigencia >= GETDATE()";
            DataTable res = DSODataAccess.Execute(strQuery);
            if (res.Rows.Count > 0)
            {
                ddlEmpresa.DataSource = res;
                ddlEmpresa.DataTextField = "vchDescripcion";
                ddlEmpresa.DataValueField = "iCodCatalogo";
                ddlEmpresa.DataBind();
            }
        }
        
        private void GenerarCarga()
        {
            //Variables
            DateTime hoy = DateTime.Today;
            string strPath = $@"\\DTIMTYKEYTIA01\Archivos\Cargas\Ternium\Cargas\Carga Dicomtec\{hoy.Year}\{hoy.Month.ToString("d2")}\";
            string strSavePathFileArchivo = strPath + fileArchivo.FileName;

            try
            {
                //Guardar archivo
                fileArchivo.SaveAs(strSavePathFileArchivo);

                //Guardar carga
                string strClave = Request.Form.Get("txtClave");
                string strDescripcion = Request.Form.Get("txtDescripcion");
                string strPeriodo = Request.Form.Get("txtPeriodo");
                string[] lstPeriodo = strPeriodo.Split('-');
                string strAnio = lstPeriodo[0];
                string strMes = lstPeriodo[1];
                string strEmpresa = ddlEmpresa.SelectedValue;

                string strQuery = $"exec [dbo].[TerniumCargaDicomtec] @Clave = '{strClave}', @descripcion = '{strDescripcion}', @Anio = {strAnio}, @Mes = {strMes}, @Empresa = {strEmpresa}, @ArchivoUnidad = '{strSavePathFileArchivo}'";
                DataTable res = DSODataAccess.Execute(strQuery);
                if (Convert.ToInt32(res.Rows[0][0]) != 1)
                {
                    divMensaje.Visible = true;
                    lblMensaje.Text = res.Rows[0][1].ToString();
                }

                //Redireccionar a la pantalla de consulta
                HttpContext.Current.Response.Redirect("~/UserInterface/RepTernium/ConsultaCargaRepTernium.aspx", false);
            }
            catch (Exception ex)
            {
                divMensaje.Visible = true;
                lblMensaje.Text = ex.ToString();
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            GenerarCarga();
        }
    }
}