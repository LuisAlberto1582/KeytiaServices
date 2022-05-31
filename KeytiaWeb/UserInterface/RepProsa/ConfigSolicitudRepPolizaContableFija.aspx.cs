using System;
using System.Collections.Generic;
using System.Linq;
using KeytiaServiceBL;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepProsa
{
    public partial class ConfigSolicitudRepPolizaContableFija : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            if (!Page.IsPostBack)
            {
                ObtenerFacturas();
            }
        }

        private void ObtenerFacturas() {
            string strQuery = "select * from Prosa.[vishistoricos('FacturaPolizaContableTelFija','Facturas Poliza Contable Tel Fija','Español')]";
            DataTable res = DSODataAccess.Execute(strQuery);
            if (res.Rows.Count > 0)
            {
                rowGrv.Visible = true;
                InfoPanelSucces.Visible = false;
                grvFacturas.DataSource = res;
                grvFacturas.DataBind();
            }
            else
            {
                lblMensajeSuccess.Text = "No existen facturas Actualmente";
                InfoPanelSucces.Visible = true;
                rowGrv.Visible = false;
            }

        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            GuardarCarga();
        }

        protected void GuardarCarga() {
            //Obtener valores
            string strPeriodo = Request.Form.Get("txtPeriodo");
            string[] lstPeriodo = strPeriodo.Split('-');
            string strAnio = lstPeriodo[0];
            string strMes = lstPeriodo[1];
            string strDestinatario = Request.Form.Get("txtDestinatario");

            //Guardar registro
            string strQuery = $"exec ProcesoAutomaticoProsaPolizaContFija @anio = {strAnio}, @mes = {strMes}, @destinatario = '{strDestinatario}'";
            DataTable res = DSODataAccess.Execute(strQuery);
        }

        protected void grvFacturas_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grvFacturas.EditIndex = e.NewEditIndex;
            ObtenerFacturas();
        }

        protected void grvFacturas_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grvFacturas.EditIndex = -1;
            ObtenerFacturas();
        }

        protected void grvFacturas_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string strICodRegistro = grvFacturas.DataKeys[e.RowIndex].Value.ToString();
            //string strDescripcion = (grvFacturas.Rows[e.RowIndex].FindControl("txtDescripcion") as TextBox).Text.Trim();
            int intProrrateo = ((grvFacturas.Rows[e.RowIndex].FindControl("chkProrrateo") as CheckBox).Checked) == true ? 1 : 0;
            string strDebe = (grvFacturas.Rows[e.RowIndex].FindControl("txtDebe") as TextBox).Text.Trim();
            string strHaber = (grvFacturas.Rows[e.RowIndex].FindControl("txtHaber") as TextBox).Text.Trim();
            string strCC = (grvFacturas.Rows[e.RowIndex].FindControl("txtCC") as TextBox).Text.Trim();
            string strCia = (grvFacturas.Rows[e.RowIndex].FindControl("txtCia") as TextBox).Text.Trim();
            string strCuenta = (grvFacturas.Rows[e.RowIndex].FindControl("txtCuenta") as TextBox).Text.Trim();
            string strProducto = (grvFacturas.Rows[e.RowIndex].FindControl("txtProducto") as TextBox).Text.Trim();
            string strTemp = (grvFacturas.Rows[e.RowIndex].FindControl("txtTemp") as TextBox).Text.Trim();

            string strQuery = $"UPDATE Prosa.[vishistoricos('FacturaPolizaContableTelFija','Facturas Poliza Contable Tel Fija','Español')] SET BanderasFacturaPolizaContableTelFija = {intProrrateo}, ImpDebe = {(strDebe != "" ? strDebe : "Null")}, ImpHaber = {(strHaber != "" ? strHaber : "Null")}, ProsaClaveCCOtrosGastos = '{strCC}', Cia = '{strCia}', CuentaContable = '{strCuenta}', Producto = '{strProducto}', Temp = '{strTemp}' WHERE iCodRegistro = {strICodRegistro}";
            DataTable res = DSODataAccess.Execute(strQuery);
            grvFacturas.EditIndex = -1;
            ObtenerFacturas();
        }
    }
}