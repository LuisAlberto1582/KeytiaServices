using KeytiaWeb.WSSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class CategoriaElemento : System.Web.UI.Page
    {
        #region Eventos
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
            if (list.Count <= 1)
            {
                btnRegresar.Visible = false;
            }
            else
            {
                btnRegresar.Visible = true;
            }
            #endregion

            if (!IsPostBack)
            {
                CargarGridCategoria();
            }

            int id;
            Int32.TryParse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? null : Request.QueryString["Id"].ToString(), out id);
            if (id > 0)
            {
                EliminarCategoria(id);
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected void gvCategoria_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            string commandName = e.CommandName.ToString().ToLower();
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int RowIndex = gvr.RowIndex;

            WSSS.InvContCategoriaElemento objCatElem = new WSSS.InvContCategoriaElemento();
            objCatElem.Id = Convert.ToInt32(gvCategoria.DataKeys[RowIndex].Values["Id"].ToString());
            objCatElem.Nombre = gvCategoria.DataKeys[RowIndex].Values["Nombre"].ToString();
            objCatElem.Activo = Convert.ToBoolean(gvCategoria.DataKeys[RowIndex].Values["Activo"].ToString().ToString());
            objCatElem.sActivo = gvCategoria.DataKeys[RowIndex].Values["sActivo"].ToString();


            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "";
            if (commandName == "editar")
            {
                if (CargarModal(objCatElem))
                {
                    MostrarModal(objCatElem);
                }

            }
            else if (commandName == "eliminar")
            {
                cstext = "if(confirm('¿Está seguro que desea eliminar el registro?')){ location.assign('CategoriaElemento.aspx?Id=" + objCatElem.Id.ToString() + "'); }";
            }

            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {

            try
            {
                WSSS.InvContCategoriaElemento objCatElem = new InvContCategoriaElemento();
                objCatElem.Id = Convert.ToInt32(txtIdCategoriaElementoModal.Text.ToString());
                objCatElem.Nombre = txtNombreCategoriaElementoIdModal.Text.ToUpper();
                objCatElem.Activo = dpdEstatusModal.SelectedValue == "1" ? true : false;
                objCatElem.sActivo = objCatElem.Activo ? "ACTIVO" : "INACTIVO";
                UpdateCatElem(objCatElem);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void ImgInsertaElementoContratado_Click(object sender, EventArgs e)
        {
            try
            {
                InsertarCategoria();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Métodos
        public void CargarGridCategoria()
        {
            try
            {
                gvCategoria.DataSource = BuscarCategorias();
                gvCategoria.DataBind();
            }
            catch (Exception)
            {
            }
        }

        public List<WSSS.InvContCategoriaElemento> BuscarCategorias()
        {
            List<WSSS.InvContCategoriaElemento> listaCategorias = new List<WSSS.InvContCategoriaElemento>();

            try
            {
                WebServiceSoapClient service = new WebServiceSoapClient();

                string JSON = service.DevuelveDropDownCategoriaElementoJSON(Util.Encrypt(DSODataContext.ConnectionString));

                listaCategorias = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContCategoriaElemento>>(JSON);
            }
            catch (Exception ex)
            {
            }

            return listaCategorias;
        }

        public void EliminarCategoria(int id)
        {
            WSSS.InvContCategoriaElemento objCategoriaElem = new InvContCategoriaElemento();
            string res = string.Empty;
            try
            {
                objCategoriaElem.Id = id;

                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        res = service.DeleteCategoriaElemento(objCategoriaElem, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                }

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "');  location.href = 'CategoriaElemento.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);

            }
            catch (Exception ex)
            {
                res = "Ocurrio un error al eliminar categoria. ";
            }


        }

        public bool MostrarModal(WSSS.InvContCategoriaElemento objCatElem)
        {
            bool res = false;
            try
            {
                mpeCategoriaModal.Show();
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }

        public bool CargarModal(WSSS.InvContCategoriaElemento objCatElem)
        {
            bool res = false;
            try
            {
                txtIdCategoriaElementoModal.Text = objCatElem.Id.ToString();
                txtNombreCategoriaElementoIdModal.Text = objCatElem.Nombre.ToString();
                dpdEstatusModal.SelectedValue = objCatElem.Activo ? "1" : "2";

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }

        public bool InsertarCategoria()
        {
            bool res = false;
            string respuesta = "";
            try
            {
                WSSS.InvContCategoriaElemento objCatElem = new InvContCategoriaElemento();

                objCatElem.Nombre = txtNombreCategoria.Text.ToString().ToUpper();
                objCatElem.Activo = dpdEstatus.SelectedValue == "1" ? true : false;

                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        respuesta = service.InsertCategoriaElemento(objCatElem, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        respuesta = ex.Message.ToString();
                    }
                }

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + respuesta + "');  location.href = 'CategoriaElemento.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }

        public bool UpdateCatElem(WSSS.InvContCategoriaElemento objCatElem)
        {
            bool resultado = false;
            string res = string.Empty;

            try
            {
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        res = service.UpdateCategoriaElemento(objCatElem, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message.ToString();
                    }
                }

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "');  location.href = 'CategoriaElemento.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            catch (Exception ex)
            {
                resultado = false;
            }

            return resultado;
        }
        #endregion




        #region Eventos Todas páginas
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



        #endregion


    }
}