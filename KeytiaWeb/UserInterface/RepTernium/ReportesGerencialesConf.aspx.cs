using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;
using System.Data;
using System.Text;

namespace KeytiaWeb.UserInterface.RepTernium
{
    public partial class ReportesGerencialesConf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            //Combos
            if (!IsPostBack)
            {
                LLenarDdlAnio();
                LLenarDdlMes();
                LlenarDdlPais();
            }
        }

        private void ObtenerDirecciones(string strPais)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT iCodCatalogo, vchDescripcion + '(' + EmpreCod + ')' as vchDescripcion");
            sb.AppendLine("FROM Ternium.[vishistoricos('Usuar','Usuarios','Español')]");
            sb.AppendLine("WHERE dtfinvigencia>=GETDATE()");
            sb.AppendLine("AND Perfil = 369");
            sb.AppendLine($"AND Empre = ( SELECT Empre.iCodCatalogo FROM Ternium.[VisHistoricos('Paises','Paises','Español')] Pais JOIN Ternium.[VisHistoricos('Empre','Empresas','Español')] Empre ON Empre.dtFinVigencia >= GETDATE() AND Empre.Paises = Pais.iCodCatalogo WHERE Pais.dtFinVigencia >= GETDATE() AND Pais.vchDescripcion = '{strPais}')");
            sb.AppendLine("AND CategoUsuar = 'DIRECCION'");

            DataTable res = DSODataAccess.Execute(sb.ToString());
            if (res.Rows.Count > 0)
            {
                grdDirecciones.DataSource = res;
                grdDirecciones.DataBind();
            }
        }

        private void LLenarDdlAnio()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT iCodCatalogo, vchDescripcion from Ternium.[VisHistoricos('Anio','Años','Español')]");
            sb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            sb.AppendLine("AND CONVERT(int, vchDescripcion) BETWEEN DATEPART(yyyy, DATEADD(yyyy, -1, GETDATE())) AND DATEPART(yyyy, GETDATE())");
            sb.AppendLine("ORDER BY convert(int, vchDescripcion) desc");

            DataTable res = DSODataAccess.Execute(sb.ToString());
            if (res.Rows.Count > 0)
            {
                ddlAnio.DataSource = res;
                ddlAnio.DataTextField = "vchDescripcion";
                ddlAnio.DataValueField = "iCodCatalogo";
                ddlAnio.DataBind();
                ddlAnio.Items.Insert(0, new ListItem("Seleccionar año", string.Empty));
            }
        }

        private void LLenarDdlMes()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT iCodCatalogo, vchDescripcion from Ternium.[VisHistoricos('Mes','Meses','Español')]");
            sb.AppendLine("WHERE dtFinVigencia >= GETDATE()");
            sb.AppendLine("order by convert(int, vchCodigo)");

            DataTable res = DSODataAccess.Execute(sb.ToString());
            if (res.Rows.Count > 0)
            {
                ddlMes.DataSource = res;
                ddlMes.DataTextField = "vchDescripcion";
                ddlMes.DataValueField = "iCodCatalogo";
                ddlMes.DataBind();
                ddlMes.Items.Insert(0, new ListItem("Seleccionar mes", string.Empty));
            }
        }

        private void LlenarDdlPais()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT Pais.vchDescripcion, Pais.iCodCatalogo");
            sb.AppendLine("FROM Ternium.[VisHistoricos('Paises','Paises','Español')] Pais");
            sb.AppendLine("JOIN Ternium.[VisHistoricos('Empre','Empresas','Español')] Empre");
            sb.AppendLine("ON Empre.dtFinVigencia>=GETDATE()");
            sb.AppendLine("AND Empre.Paises = Pais.iCodCatalogo");
            //string strQuery = "select Pais.vchDescripcion, Pais.iCodCatalogo from Ternium.[VisHistoricos('Paises','Paises','Español')] Pais JOIN Ternium.[VisHistoricos('Empre','Empresas','Español')] Empre on Empre.dtFinVigencia>=GETDATE() and Empre.Paises = Pais.iCodCatalogo";
            DataTable res = DSODataAccess.Execute(sb.ToString());
            if (res.Rows.Count > 0)
            {
                ddlPais.DataSource = res;
                ddlPais.DataTextField = "vchDescripcion";
                ddlPais.DataValueField = "iCodCatalogo";
                ddlPais.DataBind();
                ddlPais.Items.Insert(0, new ListItem("Seleccionar pais", string.Empty));
            }
        }

        private void GuardarSolicitud()
        {
            //Obtener valores
            string strIds = ObtenerIdsDirecciones();
            string strClave = txtClave.Text.Trim();
            string strAnio = ddlAnio.SelectedValue;
            string strMes = ddlMes.SelectedValue;
            string strPais = ddlPais.SelectedValue;
            int intIdioma = ddlPais.SelectedIndex != 630 ? 438 : 439; //Si es diferente de EUA es español sino es español
            string strDestinatario = txtDestinatario.Text.Trim();
            int intChkGeneraConsolidado = chkGenerarConsolidado.Checked ? 1 : 0;
            int intRadDolares = radDolares.Checked ? 2 : 0;
            int intBanderaRep = intChkGeneraConsolidado + intRadDolares;

            //Ejecutar SP para guardar la solicitud
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("EXEC TerniumReporteGerencialNuevaSolicitud");
            sb.AppendLine($"@vchCodigo = '{strClave}',");
            sb.AppendLine($"@CadenaTextoParaIDs = '{strIds}',");
            sb.AppendLine($"@iCodCatPais = {strPais},");
            sb.AppendLine($"@iCodCatAnio = {strAnio},");
            sb.AppendLine($"@iCodCatMes = {strMes},");
            sb.AppendLine($"@iCodCatIdioma = {intIdioma},");
            sb.AppendLine($"@banderasRepGerenciales = {intBanderaRep},");
            sb.AppendLine($"@destinatarios = '{strDestinatario}'");

            DataTable res = DSODataAccess.Execute(sb.ToString());

            //Redireccionar a la pantalla de consulta
            HttpContext.Current.Response.Redirect("~/UserInterface/RepTernium/RepGerencialSolicitudes.aspx", false);

        }

        protected string ObtenerIdsDirecciones()
        {
            List<string> lstIds = new List<string>();
            foreach (GridViewRow row in grdDirecciones.Rows)
            {
                int chkDireccion = ((row.FindControl("chkDireccion") as CheckBox).Checked) == true ? 1 : 0;
                if (chkDireccion == 1)
                {
                    string strId = row.Cells[1].Text;
                    lstIds.Add(strId);
                }
            }

            string strIds = String.Join(",", lstIds);
            return strIds;
        }
        
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            bool flgValidacion = ValidarClaveSolicitud();
            if (flgValidacion)
            {
                GuardarSolicitud();
            }
            else {
                divMensaje.Visible = true;
                lblMensaje.Text = "La clave ingresada ya existe.";
            }
        }

        //Verifica que no exista alguna solicitud con la misma clave
        private bool ValidarClaveSolicitud()
        {
            bool flg = false;
            string strClave = txtClave.Text.Trim();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"IF EXISTS(SELECT * FROM Ternium.[vishistoricos('ProcesoAutomatico','Ternium reportes gerenciales','Español')] WHERE vchCodigo = '{strClave}')");
            sb.AppendLine("BEGIN");
            sb.AppendLine("SELECT 1");
            sb.AppendLine("END");
            sb.AppendLine("ELSE");
            sb.AppendLine("BEGIN");
            sb.AppendLine("SELECT 0");
            sb.AppendLine("END");

            DataTable res = DSODataAccess.Execute(sb.ToString());
            if (res.Rows.Count > 0)
            {
                if (res.Rows[0][0].ToString() == "0")
                {
                    flg = true;
                }
            }
            return flg;
        }

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strPais = ddlPais.SelectedItem.ToString();
            ObtenerDirecciones(strPais);
        }
    }
}