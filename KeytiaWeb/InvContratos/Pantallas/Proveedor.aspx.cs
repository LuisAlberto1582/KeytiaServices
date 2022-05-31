using KeytiaWeb.InvContratos.App_Code.Models;
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
    public partial class Proveedor : System.Web.UI.Page
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



            if (!Page.IsPostBack)
            {
                GetProveedores();
                DpdRegion_SelectedIndexChanged();
            }
            int id;
            Int32.TryParse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? null : Request.QueryString["Id"].ToString(), out id);
            if (id > 0)
            {
                EliminaProveedor(id);
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        public void GetProveedores()
        {
            List<ProveedoresContacto> elemento = new List<ProveedoresContacto>();
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonEncabezado = service.DevuelveProveedorContactoJSON(Util.Encrypt(DSODataContext.ConnectionString));
                elemento = (new JavaScriptSerializer()).Deserialize<List<ProveedoresContacto>>(jsonEncabezado);
                gvProveedores.DataSource = elemento;
                gvProveedores.DataBind();

            }

        }

        protected void DpdRegion_SelectedIndexChanged()
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<WSSS.InvContRegion> region = new List<WSSS.InvContRegion>();
                string JSON = service.DevuelveDropDownRegionJSON(Util.Encrypt(DSODataContext.ConnectionString));
                region = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContRegion>>(JSON);
                DpdRegion.DataSource = region.ToList();
                DpdRegion.DataBind();
            }
        }

        public void InsertaProveedor(object sender, EventArgs e)
        {
            string result;
            WSSS.InvContProveedor proveedor = new WSSS.InvContProveedor();
            proveedor.RazonSocial = txtRazonSocial.Text;
            proveedor.Nombre = txtNombre.Text;
            int pais = Int32.Parse(DpdRegion.SelectedValue);
            proveedor.PaisId = pais;
            proveedor.NumeroProveedorSAP = txtNumeroProveedorSAP.Text;
            proveedor.UsuarioUltAct = 0;

            WSSS.InvContContacto contacto = new WSSS.InvContContacto();
            contacto.Nombre = txtContacto.Text;
            contacto.CorreoElectronico = txtCorreo.Text;
            contacto.TelefonoExtension = txtTelefono.Text;
            contacto.UsuarioUltAct = 0;

            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    result = service.InsertarProveedor(proveedor, contacto, Util.Encrypt(DSODataContext.ConnectionString));
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            List<ProveedoresContacto> elementoG = new List<ProveedoresContacto>();
            WebServiceSoapClient services = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonEncabezado =
                    service.DevuelveProveedorContactoJSON(Util.Encrypt(DSODataContext.ConnectionString));
                elementoG = (new JavaScriptSerializer()).Deserialize<List<ProveedoresContacto>>(jsonEncabezado);
                gvProveedores.DataSource = elementoG;
                gvProveedores.DataBind();


                txtRazonSocial.Text = "";
                txtNombre.Text = "";
                DpdRegion.Text = "0";
                txtNumeroProveedorSAP.Text = "";
                txtContacto.Text = "";
                txtCorreo.Text = "";
                txtTelefono.Text = "";
            }

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "alert('" + result + "'); location.href = 'Proveedor.aspx';";
            cs.RegisterStartupScript(cstype, "", cstext, true);

        }

        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditarProveedor")
            {

                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;

                WSSS.InvContProveedor objProveedor = new InvContProveedor();
                objProveedor.Id = Convert.ToInt32(gvProveedores.DataKeys[RowIndex].Values["IdProveedor"].ToString());
                objProveedor.Nombre = gvProveedores.DataKeys[RowIndex].Values["Nombre"].ToString();
                objProveedor.RazonSocial = gvProveedores.DataKeys[RowIndex].Values["RazonSocial"].ToString(); ;
                objProveedor.PaisId = Convert.ToInt32(gvProveedores.DataKeys[RowIndex].Values["IdPais"].ToString());
                objProveedor.NumeroProveedorSAP = gvProveedores.DataKeys[RowIndex].Values["NoProveedorSAP"].ToString(); ;

                WSSS.InvContContacto objcontacto = new InvContContacto();
                objcontacto.Id = Convert.ToInt32(gvProveedores.DataKeys[RowIndex].Values["IdContacto"].ToString());
                objcontacto.Nombre = gvProveedores.DataKeys[RowIndex].Values["NombreContacto"].ToString();
                objcontacto.CorreoElectronico = gvProveedores.DataKeys[RowIndex].Values["CorreoElectronico"].ToString();
                objcontacto.TelefonoExtension = gvProveedores.DataKeys[RowIndex].Values["TelefonoExtension"].ToString();

                if (cargarModal(objProveedor, objcontacto))
                {
                    mpeCargaArchivo.Show();
                }

            }
            if (e.CommandName == "EliminarProveedor")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                int IdProveedor = Int32.Parse(gvProveedores.DataKeys[RowIndex].Values["IdProveedor"].ToString());
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "if(confirm('¿Está seguro que desea eliminar el registro?')){ location.assign('Proveedor.aspx?Id=" + IdProveedor + "'); }";
                cs.RegisterStartupScript(cstype, "", cstext, true);

            }
        }

        public bool cargarModal(WSSS.InvContProveedor objProveedor, WSSS.InvContContacto objContacto)
        {
            bool Respuesta = false;

            //Proveedor
            txtIdProveedorModal.Text = objProveedor.Id.ToString();
            txtNombreModal.Text = objProveedor.Nombre;
            txtRazonSocialModal.Text = objProveedor.RazonSocial;
            DpdPaisModal.SelectedValue = objProveedor.PaisId.ToString();
            txtNumeroProveedorSAP.Text = objProveedor.NumeroProveedorSAP;

            //Contacto
            txtIdContactoModal.Text = objContacto.Id.ToString();
            txtNombrecontactoModal.Text = objContacto.Nombre;
            txtCorreoContactoModal.Text = objContacto.CorreoElectronico;
            txtTelefonoContactoModal.Text = objContacto.TelefonoExtension;

            Respuesta = true;
            return Respuesta;
        }

        public void EliminaProveedor(Int32 id)
        {
            String result = String.Empty;
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    result = service.EliminarProveedor(id, Util.Encrypt(DSODataContext.ConnectionString));
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "alert('" + result + "'); '";

            cs.RegisterStartupScript(cstype, "", cstext, true);
            Response.Redirect("~/InvContratos/Pantallas/Proveedor.aspx");
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

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            WSSS.InvContProveedor objProveedor = new InvContProveedor();

            objProveedor.Id = Convert.ToInt32(txtIdProveedorModal.Text);
            objProveedor.Nombre = txtNombreModal.Text;
            objProveedor.PaisId = Convert.ToInt32(DpdPaisModal.SelectedValue.ToString());
            objProveedor.NumeroProveedorSAP = TextBoxtxtNomProvSapModal.Text;
            objProveedor.RazonSocial = txtRazonSocialModal.Text;

            WSSS.InvContContacto objContacto = new InvContContacto();
            objContacto.Id = Convert.ToInt32(txtIdContactoModal.Text.ToString());
            objContacto.Nombre = txtNombrecontactoModal.Text.ToString();
            objContacto.CorreoElectronico = txtCorreoContactoModal.Text.ToString();
            objContacto.TelefonoExtension = txtTelefonoContactoModal.Text.ToString();


            string mensajeEdicion = "";

            mensajeEdicion += EditarProveedor(objProveedor);

            mensajeEdicion += EditarContacto(objContacto);

            if (mensajeEdicion.Length > 0 && mensajeEdicion.Trim() != "")
            {
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;

                String cstext = "alert('" + mensajeEdicion + "'); location.href = 'Proveedor.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }



        }

        public string EditarProveedor(WSSS.InvContProveedor objProveedor)
        {
            string Resultado = "";

            try
            {
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        Resultado = service.ModificaProveedor(objProveedor, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        Resultado = ex.Message;
                    }
                }

            }
            catch (Exception ex)
            {
                Resultado = "Error: " + ex.Message.ToString() + "";
            }

            return Resultado;
        }

        public string EditarContacto(WSSS.InvContContacto objContacto)
        {
            string resultado = "";


            try
            {
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        resultado = service.ModificaContacto(objContacto, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        resultado = ex.Message;
                    }
                }

            }
            catch (Exception ex)
            {
                resultado = "Error: " + ex.Message.ToString() + "";
            }

            return resultado;
        }
    }
}