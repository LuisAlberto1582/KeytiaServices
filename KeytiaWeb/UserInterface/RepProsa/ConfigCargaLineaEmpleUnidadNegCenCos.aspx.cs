using System;
using KeytiaServiceBL;
using System.Data;
using System.IO;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Collections.Generic;
using System.Web;

namespace KeytiaWeb.UserInterface.RepProsa
{
    public partial class ConfigCargaLineaEmpleUnidadNegCenCos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void GenerarCargaInvetarioLineas()
        {
            //Variables
            string strPath = @"\\DTIMTYKEYTIA01\Archivos\Cargas\Prosa\Cargas\CargaRelLineaEmpleCenCosUnidadNeg\";
            string strSavePathFileLineas = strPath + fileInventarioLineas.FileName;
            string strSavePathFileUnidad = strPath + fileInventarioUnidadNegocio.FileName;
            string strSavePathFileFTES = strPath + fileFTES.FileName;

            try
            {
                //Guardar archivos
                fileInventarioLineas.SaveAs(strSavePathFileLineas);
                fileInventarioUnidadNegocio.SaveAs(strSavePathFileUnidad);
                fileFTES.SaveAs(strSavePathFileFTES);

                //Obtener valores
                string strClave = Request.Form.Get("txtClave");
                string strDescripcion = Request.Form.Get("txtDescripcion");
                string strPeriodo = Request.Form.Get("txtPeriodo");
                string[] lstPeriodo = strPeriodo.Split('-');
                string strAnio = lstPeriodo[0];
                string strMes = lstPeriodo[1];

                string strQuery = $"exec dbo.ProsaCargaRelLineaEmpleUnidadNegocioCenCosIns @Clave = '{strClave}', @descripcion = '{strDescripcion}', @Anio = {strAnio}, @Mes = {strMes}, @ArchivoUnidad = '{strSavePathFileUnidad}', @ArchivoLineas = '{strSavePathFileLineas}', @ArchivoFTES = '{strSavePathFileFTES}' ";
                DataTable res = DSODataAccess.Execute(strQuery);
                if (Convert.ToInt32(res.Rows[0][0]) != 1)
                {
                    divMensaje.Visible = true;
                    lblMensaje.Text = res.Rows[0][1].ToString();
                }

                //Redireccionar a la pantalla de consulta
                HttpContext.Current.Response.Redirect("~/UserInterface/RepProsa/ConsultaCargaLineaEmpleUnidadNegCenCos.aspx", false);
            }
            catch (Exception ex)
            {
                divMensaje.Visible = true;
                lblMensaje.Text = ex.ToString();
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            GenerarCargaInvetarioLineas();
        }
    }
}