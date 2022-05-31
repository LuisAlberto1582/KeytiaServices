using KeytiaWeb.InvContratos.App_Code.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaWeb.WSSS;
using KeytiaServiceBL;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class BusquedaEspecifica : System.Web.UI.Page
    {
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
                DpdProveedor_SelectedIndexChanged();
                DpdCategoria_SelectedIndexChanged();
                DpdArea_SelectedIndexChanged();
                DpdElemento_SelectedIndexChanged();
                dpdCategoriaServicio_SelectedIndexChanged();
                DpdEstatus_SelectedIndexChanged();
                DpdRegion_SelectedIndexChanged();
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        /// <summary>
        /// Carga el combo del proveedor.
        /// </summary>
        public void DpdProveedor_SelectedIndexChanged()
        {
            WebServiceSoapClient Servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<InvContProveedor> List = new List<InvContProveedor>();
                string JSON = Servicio.DevuelveDropDownProveedorJSON(Util.Encrypt(DSODataContext.ConnectionString));

                List = (new JavaScriptSerializer()).Deserialize<List<InvContProveedor>>(JSON);
                dpdProveedor.DataSource = List.ToList();
                dpdProveedor.DataBind();
            }

        }

        /// <summary>
        /// Carga el compo del tipo de convenio.
        /// </summary>
        public void DpdCategoria_SelectedIndexChanged()
        {
            WebServiceSoapClient servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<WSSS.InvContConvenioTipo> convenioTipos = new List<WSSS.InvContConvenioTipo>();
                string JSON = servicio.DevuelveDropDownConvenioJSON(Util.Encrypt(DSODataContext.ConnectionString));

                convenioTipos = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContConvenioTipo>>(JSON);
                dpdCategoria.DataSource = convenioTipos.ToList();
                dpdCategoria.DataBind();
            }
        }

        /// <summary>
        /// Carga el combo del area.
        /// </summary>
        public void DpdArea_SelectedIndexChanged()
        {
            WebServiceSoapClient servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<WSSS.InvContArea> area = new List<WSSS.InvContArea>();
                string JSON = servicio.DevuelveAreaJSON(Util.Encrypt(DSODataContext.ConnectionString));

                area = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContArea>>(JSON);
                DpdArea.DataSource = area.ToList();
                DpdArea.DataBind();
            }
        }

        /// <summary>
        /// Carga el combo del elemento.
        /// </summary>
        public void DpdElemento_SelectedIndexChanged()
        {
            WebServiceSoapClient servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<WSSS.InvContElemento> elemento = new List<WSSS.InvContElemento>();
                string JSON = servicio.DevuelveElementosJSON(Util.Encrypt(DSODataContext.ConnectionString));

                elemento = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContElemento>>(JSON);
                DpdElemento.DataSource = elemento.ToList();
                DpdElemento.DataBind();
            }
        }

        /// <summary>
        /// Carga el combo del tipo de servicio.
        /// </summary>
        protected void dpdCategoriaServicio_SelectedIndexChanged()
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<WSSS.InvContTipoServicio> servicio = new List<WSSS.InvContTipoServicio>();
                string JSON = service.DevuelveDropDownTipoServicioJSON(Util.Encrypt(DSODataContext.ConnectionString));

                servicio = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContTipoServicio>>(JSON);
                dpdCategoriaServicio.DataSource = servicio.ToList();
                dpdCategoriaServicio.DataBind();
            }
        }

        /// <summary>
        /// Carga el combo de la región.
        /// </summary>
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

        /// <summary>
        /// Carga el combo del estatus del convenio.
        /// </summary>
        protected void DpdEstatus_SelectedIndexChanged()
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<WSSS.InvContConvenioEstatus> convenio = new List<WSSS.InvContConvenioEstatus>();
                string JSON = service.DevuelveDropDownConvenioEstatusJSON(Util.Encrypt(DSODataContext.ConnectionString));

                convenio = (new JavaScriptSerializer()).Deserialize<List<WSSS.InvContConvenioEstatus>>(JSON);
                DpdEstatus.DataSource = convenio.ToList();
                DpdEstatus.DataBind();
            }
        }

        /// <summary>
        /// Es el evento del clic en aceptar de la búsqueda Específica.
        /// Toma los parámetros y los manda a la página de los grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAceptar_Click1(object sender, EventArgs e)
        {
            if (txtFechaInicio.Text == "")
            {
                Busqueda();
            }
            else
            {
                DateTime fecha;
                if (DateTime.TryParseExact(txtFechaInicio.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
                {
                    if (DateTime.TryParseExact(txtFechaFin.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
                    {
                        Busqueda();
                    }
                    else
                    {
                        Type cstype = this.GetType();
                        ClientScriptManager cs = Page.ClientScript;
                        String cstext = "alert('Favor de insertar un formato de fecha válido para la Fecha Inicio Vigencia. Ejemplo: 2018/12/31');";
                        cs.RegisterStartupScript(cstype, "", cstext, true);
                    }
                }
                else
                {
                    Type cstype = this.GetType();
                    ClientScriptManager cs = Page.ClientScript;
                    String cstext = "alert('Favor de insertar un formato de fecha válido. Ejemplo: 2018/12/31');";
                    cs.RegisterStartupScript(cstype, "", cstext, true);
                }
            }
        }

        public void Busqueda()
        {
            string folio = txtFolio.Text;
            string clave = txtClave.Text;
            string cuentaContable = txtCuentaContable.Text;
            int proveedor = int.Parse(dpdProveedor.SelectedValue);
            int convenioTipo = int.Parse(dpdCategoria.SelectedValue);
            int tipoServicio = int.Parse(dpdCategoriaServicio.SelectedValue);
            int estatusConvenio = int.Parse(DpdEstatus.SelectedValue);
            string descripcion = txtDescripcion.Text;
            string nombreContacto = txtContacto.Text;
            string mailContacto = txtMail.Text.Trim();
            string vigencia = "true";
            if (rbNo.Checked)
            {
                vigencia = "false";
            }
            string nombreArchivo = txtArchivo.Text;
            string compradorNombre = txtComprador.Text;
            string nombreSolicitante = txtSolicitante.Text;
            string fechaInicio = txtFechaInicio.Text;
            string fechaFin = txtFechaFin.Text;
            int area = int.Parse(DpdArea.SelectedValue);
            int region = int.Parse(DpdRegion.SelectedValue);
            int elemento = int.Parse(DpdElemento.SelectedValue);
            string metodo = "BusquedaEspecifica";
            string mailComprador = txtMailComprador.Text;
            Response.Redirect("~/InvContratos/Pantallas/ResultadoEspecifico.aspx?folio=" + Server.UrlEncode(folio) + "&clave=" + Server.UrlEncode(clave) + "&cuentaContable=" + Server.UrlEncode(cuentaContable) + "&proveedor=" + proveedor + "&convenioTipo=" + convenioTipo + "&tipoServicio=" + tipoServicio + "&estatusConvenio=" + estatusConvenio + "&descripcion=" + Server.UrlEncode(descripcion) + "&nombreContacto=" + Server.UrlEncode(nombreContacto) + "&mailContacto=" + Server.UrlEncode(mailContacto) + "&vigencia=" + vigencia + "&nombreArchivo=" + Server.UrlEncode(nombreArchivo) + "&compradorNombre=" + Server.UrlEncode(compradorNombre) + "&nombreSolicitante=" + Server.UrlEncode(nombreSolicitante) + "&fechaInicio=" + Server.UrlEncode(fechaInicio) + "&fechaFin=" + Server.UrlEncode(fechaFin) + "&area=" + area + "&region=" + region + "&elemento=" + elemento + "&Metodo=" + metodo + "&mailComprador=" + Server.UrlEncode(mailComprador));
        }

        /// <summary>
        /// Valida el formato de fecha.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtFechaInicio_TextChanged(object sender, EventArgs e)
        {
            DateTime fecha;
            if (!DateTime.TryParseExact(txtFechaInicio.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
            {
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('Favor de insertar un formato de fecha válido. Ejemplo: 2018/12/31');";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
        }

        /// <summary>
        /// Valida el formato de fecha.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtFechaFin_TextChanged(object sender, EventArgs e)
        {
            DateTime fecha;
            if (!DateTime.TryParseExact(txtFechaFin.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
            {
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('Favor de insertar un formato de fecha válido para la Fecha Inicio Vigencia. Ejemplo: 2018/12/31');";
                cs.RegisterStartupScript(cstype, "", cstext, true);
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