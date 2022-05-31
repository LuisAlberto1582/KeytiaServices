using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Inventario
{
    public partial class RetiroDispositivoMovil : System.Web.UI.Page
    {
        private int iCodCatEquipo = 0;
        private string Accion = "";

        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LeerQueryString();
                HabilitarSeccion();
            }
            
        }
        #endregion

        #region Métodos
        public void LeerQueryString()
        {
            try
            {
                int iCodCatalogo = 0;
                string Action = "";

                if (Request.QueryString["iCodCatEquipo"] != null)
                {
                    iCodCatalogo = Convert.ToInt32(Request.QueryString["iCodCatEquipo"].ToString());

                }

                if (Request.QueryString["Accion"] != null)
                {
                    Action = Request.QueryString["Accion"].ToString();
                }

                if((Action.ToLower() == "update" || Action.ToLower() == "delete") && iCodCatalogo > 0)
                {
                    iCodCatEquipo = iCodCatalogo;
                    Accion = Action;
                }

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public void HabilitarSeccion()
        {
            try
            {
                if (Accion.ToLower() == "update")
                {
                    SeccionUpdate.Style.Add("display", "block");
                    //SeccionDelete.Style.Add("display", "none");
                }
                else if(Accion.ToLower() == "delete")
                {
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        #endregion

    }
}
