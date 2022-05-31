using System;
using System.Data;
using System.Collections;
using KeytiaServiceBL;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepProsa
{
    public partial class ConfigSolicitudRepPolizaContable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GenerarSolicitudReporte()
        {
            try
            {
                //Obtener valores
                string strDescripcion = Request.Form.Get("txtDescripcion");
                string strPeriodo = Request.Form.Get("txtPeriodo");
                string[] lstPeriodo = strPeriodo.Split('-');
                string strAnio = lstPeriodo[0];
                string strMes = lstPeriodo[1];
                string strDestinatario = Request.Form.Get("txtDestinatario");
                int intOpcionesBandera = 0;
                if (chkRepPolizaContable.Checked)
                {
                    intOpcionesBandera += 1;
                }
                if (chkRepExcedentes.Checked)
                {
                    intOpcionesBandera += 2;
                }
                if (chkRepGastosBAM.Checked)
                {
                    intOpcionesBandera += 4;
                }

                //Guardar registro
                string strQuery = $"exec ProcesoAutomaticoProsaPolizaContMovil @descripcion = '{strDescripcion}', @anio = {strAnio}, @mes = {strMes}, @opcionesBandera = {intOpcionesBandera}, @destinatario = '{strDestinatario}'";
                DataTable res = DSODataAccess.Execute(strQuery);

                //Redireccionar a la pantalla de consulta
                HttpContext.Current.Response.Redirect("~/UserInterface/RepProsa/ConsultaSolicitudRepPolizaContable.aspx", false);
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            GenerarSolicitudReporte();
        }
    }
}