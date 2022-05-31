using KeytiaWeb.WSSS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class Sociedad : System.Web.UI.Page
    {
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        //public static string Db = (ConfigurationManager.ConnectionStrings["PNetConnectionString"].ConnectionString);


        protected void Page_Load(object sender, EventArgs e)
        {
            //#region Buscar los controles de busqueda que heredan de la master page y ocultarlos

            //Label lblBusqueda = (Label)Form.FindControl("lblSearch");
            //ImageButton imgbtnBusqueda = (ImageButton)Form.FindControl("imgSearch");
            //TextBox txtBusqueda = (TextBox)Form.FindControl("txtSearch");

            ///* Instanciados los controles, se inahibiltan y ocultan */
            //lblBusqueda.Visible = false;
            //lblBusqueda.Enabled = false;
            //imgbtnBusqueda.Visible = false;
            //imgbtnBusqueda.Enabled = false;
            //txtBusqueda.Visible = false;
            //txtBusqueda.Enabled = false;

            //#endregion //Fin de bloque --Buscar los controles de busqueda que heredan de la master page y ocultarlos

            #region Almacenar en variable de sesion los urls de navegacion
            List<string> list = new List<string>();
            string lsURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            if (Session["pltNavegacionDashFC"] != null) //Entonces ya tiene navegacion almacenada
            {
                list = (List<string>)Session["pltNavegacionDashFC"];
            }

            //20141114 AM. Se agrega condicion para eliminar querystring ?Opc=XXXXX
            if (lsURL.Contains("?Opc="))
            {
                //Asegurarse eliminar navegacion previa
                list.Clear();

                //Le quita el parametro Opc=XXXX
                lsURL = lsURL.Substring(0, lsURL.IndexOf("?Opc="));
            }

            //Si el url no contiene querystring y la lista tiene urls hay que limpiar la lista
            if (!(lsURL.Contains("?")) && list.Count > 0)
            {
                //Asegurarse eliminar navegacion previa
                list.Clear();
            }

            //Si no existe entonces quiere decir que estoy en un nuevo nivel de navegacion
            if (!list.Exists(element => element == lsURL))
            {
                //Agregar el valor del url actual para almacenarlo en la lista de navegacion
                list.Add(lsURL);
            }

            //Guardar en sesion la nueva lista
            Session["pltNavegacionDashFC"] = list;

            //Ocultar boton de regresar cuando solo exista un elemento en la lista
            //if (list.Count <= 1)
            //{
            //    btnRegresar.Visible = false;
            //}
            //else
            //{
            //    btnRegresar.Visible = true;
            //}
            #endregion

            if (!IsPostBack)
            {
                CargarGridSociedad();
            }

            int id;
            Int32.TryParse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? null : Request.QueryString["Id"].ToString(), out id);
            if (id > 0)
            {
                EliminarSociedad(id);
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected void gvSociedad_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandName = e.CommandName.ToString().ToLower();
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int RowIndex = gvr.RowIndex;

            WSSS.InvContSociedad objSociedad = new InvContSociedad();
            objSociedad.Id = Convert.ToInt32(gvSociedad.DataKeys[RowIndex].Values["Id"].ToString());
            objSociedad.Nombre = gvSociedad.DataKeys[RowIndex].Values["Nombre"].ToString();
            objSociedad.Clave = gvSociedad.DataKeys[RowIndex].Values["Clave"].ToString();
            objSociedad.Activo = Convert.ToBoolean(gvSociedad.DataKeys[RowIndex].Values["Activo"].ToString());


            string mensaje = "";

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "";
            if (commandName == "editar")
            {
                CargarModal(objSociedad);
                MostrarModal(objSociedad);
            }
            else if (commandName == "eliminar")
            {
                cstext = "if(confirm('¿Está seguro que desea eliminar el registro?')){ location.assign('Sociedad.aspx?Id=" + objSociedad.Id.ToString() + "'); }";
            }

            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        public bool CargarModal(WSSS.InvContSociedad objSociedad)
        {
            bool res = false;
            try
            {
                txtIdSociedadModal.Text = objSociedad.Id.ToString();
                txtClaveSociedadModal.Text = objSociedad.Clave;
                txtNombreSociedadModal.Text = objSociedad.Nombre;
                DpdEstatusModal.SelectedValue = (objSociedad.Activo ? "1" : "2");

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }

        public void MostrarModal(WSSS.InvContSociedad objsociedad)
        {
            try
            {
                if (CargarModal(objsociedad))
                {
                    mpeInsertaSociedadModal.Show();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void CargarGridSociedad()
        {
            try
            {
                List<WSSS.InvContSociedad> listaSociedad = new List<WSSS.InvContSociedad>();
                listaSociedad = BuscaSociedades();

                gvSociedad.DataSource = listaSociedad;
                gvSociedad.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<WSSS.InvContSociedad> BuscaSociedades()
        {
            List<WSSS.InvContSociedad> listaSociedad = new List<WSSS.InvContSociedad>();
            try
            {
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    string jsonEncabezado = service.GetSociedades(Util.Encrypt(DSODataContext.ConnectionString));
                    listaSociedad = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContSociedad>>(jsonEncabezado);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return listaSociedad;
        }

        protected void InsertaSociedad_Click(object sender, EventArgs e)
        {
            try
            {

                WebServiceSoapClient service = new WebServiceSoapClient();

                WSSS.InvContSociedad objSociedad = new WSSS.InvContSociedad();

                objSociedad.Nombre = txtNombreSociedad.Text;
                objSociedad.Clave = txtClaveSociedad.Text;
                objSociedad.Activo = DpdEstatusSociedad.SelectedValue == "1" ? true : false;


                string mensaje = "";
                mensaje = service.InsertaSociedad(objSociedad, Util.Encrypt(DSODataContext.ConnectionString));

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + mensaje + "'); location.href = 'Sociedad.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);







            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                EditarSociedad();
            }
            catch (Exception ex)
            {
            }
        }

        public void EditarSociedad()
        {
            string res = string.Empty;

            WSSS.InvContSociedad objSociedad = new InvContSociedad();
            objSociedad.Id = Convert.ToInt32(txtIdSociedadModal.Text);
            objSociedad.Clave = txtClaveSociedadModal.Text;
            objSociedad.Nombre = txtNombreSociedadModal.Text;
            objSociedad.Activo = DpdEstatusModal.SelectedValue.ToString() == "1" ? true : false;

            try
            {
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        res = service.EditarSociedad(objSociedad, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                }

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "');  location.href = 'Sociedad.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            catch (Exception ex)
            {
            }
        }

        public void EliminarSociedad(int id)
        {
            string res = string.Empty;

            WSSS.InvContSociedad objSociedad = new InvContSociedad();
            objSociedad.Id = id;

            try
            {
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        res = service.EliminarSociedad(objSociedad, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }


                    Type cstype = this.GetType();
                    ClientScriptManager cs = Page.ClientScript;
                    String cstext = "alert('" + res + "');  location.href = 'Sociedad.aspx';";
                    cs.RegisterStartupScript(cstype, "", cstext, true);
                }
            }
            catch (Exception ex)
            {
            }
        }

        //Master
        protected void lnkBuscar_Click(object sender, EventArgs e)
        {
            string parametro = txtBuscar.Text;
            Response.Redirect("~/invContratos/Pantallas/ResultadoEspecifico.aspx?parametro=" + parametro);
        }

        //Keytia
        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            List<string> ltNavegacion = (List<string>)Session["pltNavegacionDashFC"];

            //obtener el numero actual de elementos de la lista
            string lsCantidadElem = ltNavegacion.Count.ToString();
            //eliminar el ultimos elemento de la lista
            ltNavegacion.RemoveAt(ltNavegacion.Count - 1);

            //obtener el ultimo elemento de la lista
            string lsLastElement = ltNavegacion[ltNavegacion.Count - 1];

            HttpContext.Current.Response.Redirect(lsLastElement);
        }
    }
}