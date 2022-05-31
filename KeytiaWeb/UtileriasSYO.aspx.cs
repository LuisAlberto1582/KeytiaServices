using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;

namespace KeytiaWeb
{
    public partial class UtileriasSYO : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            // Deshabilitar siempre el botón
            btnLimpiarConferencias.Enabled = false;
            // Habilitarlo sólo si el usuario logueado es un administrador de SYO
            if (AdministradorSYO())
            {
                btnLimpiarConferencias.Enabled = true;
            }
        }

        /// <summary>
        /// Método que determina si el usuario que está logueado tiene perfil
        /// de administrador de SeeYouOn
        /// </summary>
        /// <returns>True si es un usuario administrador de SYO, false de otro modo</returns>
        private bool AdministradorSYO()
        {
            bool lbRes = false;
            object oPerfil = Session["vchCodPerfil"];
            if (oPerfil != null && oPerfil.ToString() == "SeeYouOnAdmin")
            {
                lbRes = true;
            }
            return lbRes;
        }

        protected void btnLimpiarConferencias_Click(object sender, EventArgs e)
        {
            // Volver a revisar si el usuario es administrador de SYO
            if (!AdministradorSYO())
            {
                return;
            }

            string lsResultado = "";
            try
            {
                DSODataAccess.Execute("exec spLimpiaConferenciasSYO");
                lsResultado = "Conferencias procesadas.";
            }
            catch (Exception ex)
            {
                lsResultado = "Error, consulte el log.";
                Util.LogException("Error limpiando las conferencias.", ex);
            }

            lblResultado.Text = lsResultado;
        }
    }
}
