using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DSOControls2008;
using KeytiaWeb.UserInterface.DashboardFC;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.InventarioTelecom
{
    public partial class ComentarioEdit : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                if (!Page.IsPostBack)
                {                   
                    Rep1.Visible = false;
                    Rep2.Visible = false;
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }       

        protected void btnBuscarIP_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNoSerie.Text))
            {
                var dtResult = DSODataAccess.Execute(BuscarNoSerie(txtNoSerie.Text));
                if (dtResult.Rows.Count > 0)
                {
                    lblTitulo.Text = string.Format("Datos: No. Serie  {0}", dtResult.Rows[0]["NoSerie"].ToString());
                    txtCR.Text = dtResult.Rows[0]["Cr"].ToString();
                    txtNombreCR.Text = dtResult.Rows[0]["Nombre CR"].ToString();
                    txtTipo.Text = dtResult.Rows[0]["Tipo"].ToString();
                    txtHostname.Text = dtResult.Rows[0]["Hostname"].ToString();
                    txtDireccionIPDatos.Text = dtResult.Rows[0]["Direccion IP"].ToString();
                    hfNoSerie.Value = dtResult.Rows[0]["NoSerie"].ToString();
                    txtAdministrador.Text = dtResult.Rows[0]["Administrador"].ToString();
                    txtComentarios.Text = dtResult.Rows[0]["Comentarios"].ToString();

                    Rep2.Visible = true;
                    Rep1.Visible = false;
                }
                else
                {
                    Rep1.Visible = true;
                    Rep2.Visible = false;
                }
            }
        }

        private string BuscarNoSerie(string noSerie)
        {
            query.Length = 0;
            query.AppendLine("SELECT TOP(1)");
            query.AppendLine("	[Cr]				= Equipo.CrTelecom");
            query.AppendLine("	, [Nombre CR]		= Equipo.Nombre");
            query.AppendLine("	, [Tipo]			= Equipo.TipoRecurso");
            query.AppendLine("	, [Hostname]		= Equipo.Hostname");
            query.AppendLine("	, [Direccion IP]	= Equipo.IPAdministracion");
            query.AppendLine("	, [NoSerie]	        = Equipo.NoSerie");
            query.AppendLine("	, [Administrador]	= Equipo.Administrador");
            query.AppendLine("	, [Comentarios]		= Equipo.Comentarios");
            query.AppendLine("FROM " + DSODataContext.Schema + ".EquiposTelecom Equipo");
            query.AppendLine("WHERE NoSerie = '" + noSerie + "'");

            return query.ToString();
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            ActualizarComentarios(txtComentarios.Text, hfNoSerie.Value);
            LimpiarControles();
            Rep1.Visible = false;
            Rep2.Visible = false;
            lblTituloModalMsn.Text = "Actualización de Comentarios";
            lblBodyModalMsn.Text = "Los comentarios se actualizaron correctamente.";
            mpeEtqMsn.Show();
        }

        private void ActualizarComentarios(string comentario, string noSerie)
        {
            query.Length = 0;
            query.AppendLine("UPDATE " + DSODataContext.Schema + ".EquiposTelecom");
            query.AppendLine("SET Comentarios = '" + comentario + "'");
            query.AppendLine("WHERE NoSerie = '" + noSerie + "'");
            DSODataAccess.ExecuteNonQuery(query.ToString());
        }

        private void LimpiarControles()
        {
            txtNoSerie.Text = string.Empty;
            txtDireccionIPDatos.Text = string.Empty;
            txtCR.Text = string.Empty;
            txtNombreCR.Text = string.Empty;
            txtTipo.Text = string.Empty;
            txtHostname.Text = string.Empty;
            txtAdministrador.Text = string.Empty;
            txtComentarios.Text = string.Empty;
            hfNoSerie.Value = string.Empty;
        }

    }
}
