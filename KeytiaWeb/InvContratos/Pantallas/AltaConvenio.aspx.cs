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
    public partial class Pantallas_AltaConvenio : System.Web.UI.Page
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

            if (!IsPostBack == true)
            {
                DpdCategoria_SelectedIndexChanged();
                dpdCategoriaServicio_SelectedIndexChanged();
                DpdEstatus_SelectedIndexChanged();
                DpdArea_SelectedIndexChanged();
                DpdRegion_SelectedIndexChanged();
                DpdPuesto_SelectedIndexChanged();
                CListSociedades_SelectedIndexChanged();
                DpdMoneda_SelectedIndexChanged();
                DpdProveedor_SelectedIndexChanged();
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        /// <summary>
        /// Drop down list de ConvenioTipo
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
        /// Drop down list de TipoServicio
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
        /// Drop down list de ConvenioEstatus
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
        /// Drop down list de Area
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
        /// Drop down list de region
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
        /// Drop down list de puestos
        /// </summary>
        protected void DpdPuesto_SelectedIndexChanged()
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<Puestos> puestos = new List<Puestos>();
                string JSON = service.GetPuestos(Util.Encrypt(DSODataContext.ConnectionString));

                puestos = (new JavaScriptSerializer()).Deserialize<List<Puestos>>(JSON);
                DpdPuesto.DataSource = puestos.ToList();
                DpdPuesto.DataBind();
            }
        }

        /// <summary>
        /// Checkbox list de sociedades
        /// </summary>
        protected void CListSociedades_SelectedIndexChanged()
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<InvContSociedad> sociedades = new List<InvContSociedad>();
                string JSON = service.GetSociedades(Util.Encrypt(DSODataContext.ConnectionString));

                sociedades = (new JavaScriptSerializer()).Deserialize<List<InvContSociedad>>(JSON);
                CListSociedades.DataSource = sociedades.ToList();
                CListSociedades.DataBind();
            }
        }

        /// <summary>
        /// Checkbox list de moneda
        /// </summary>
        protected void DpdMoneda_SelectedIndexChanged()
        {
            WebServiceSoapClient servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<Monedas> moneda = new List<Monedas>();
                string JSON = servicio.DevuelveDropDownMonedasJSON(Util.Encrypt(DSODataContext.ConnectionString));

                moneda = (new JavaScriptSerializer()).Deserialize<List<Monedas>>(JSON);
                DpdMoneda.DataSource = moneda.ToList();
                DpdMoneda.DataBind();
            }

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


        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            DateTime fecha;
            if (DateTime.TryParseExact(txtFechaEmision.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
            {
                if (DateTime.TryParseExact(txtFechaInicioV.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
                {
                    if (DateTime.TryParseExact(txtFechaFinV.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
                    {
                        AltaConvenio();
                    }
                    else
                    {
                        Type cstype = this.GetType();
                        ClientScriptManager cs = Page.ClientScript;
                        String cstext = "alert('Favor de insertar un formato de fecha válido para la Fecha Fin Vigencia. Ejemplo: 2018/12/31');";
                        cs.RegisterStartupScript(cstype, "", cstext, true);
                    }
                }
                else
                {
                    Type cstype = this.GetType();
                    ClientScriptManager cs = Page.ClientScript;
                    String cstext = "alert('Favor de insertar un formato de fecha válido en la Fecha Inicio Vigencia. Ejemplo: 2018/12/31');";
                    cs.RegisterStartupScript(cstype, "", cstext, true);
                }
            }
            else
            {
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('Favor de insertar un formato de fecha válido en la Fecha Emisión. Ejemplo: 2018/12/31');";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }

        }

        protected void AltaConvenio()
        {
            InvContConvModificatorio modificatorio = new InvContConvModificatorio();
            String result;
            modificatorio.Folio = txtFolio.Text;
            modificatorio.FolioContrato = txtFolioRelacionado.Text;
            modificatorio.FolioAnexo = txtFolioAnexo.Text;
            modificatorio.NumeroConvenio = txtNumeroConvenio.Text;
            modificatorio.ProveedorId = Int32.Parse(dpdProveedor.SelectedValue);

            InvContConvenioTipo convenioTipo = new InvContConvenioTipo();
            convenioTipo.Id = Int32.Parse(dpdCategoria.SelectedValue);
            modificatorio.InvContConvenioTipo = convenioTipo;

            InvContTipoServicio tipoServicio = new InvContTipoServicio();
            tipoServicio.Id = Int32.Parse(dpdCategoriaServicio.SelectedValue);
            modificatorio.InvContTipoServicio = tipoServicio;

            InvContConvenioEstatus convenioEstatus = new InvContConvenioEstatus();
            convenioEstatus.Id = Int32.Parse(DpdEstatus.SelectedValue);
            modificatorio.InvContConvenioEstatus = convenioEstatus;

            InvContArea area = new InvContArea();
            area.Id = Int32.Parse(DpdArea.SelectedValue);
            modificatorio.InvContArea = area;

            InvContRegion region = new InvContRegion();
            region.Id = Int32.Parse(DpdRegion.SelectedValue);
            modificatorio.InvContRegion = region;

            int sociedadId;
            Int32.TryParse(CListSociedades.SelectedValue, out sociedadId);
            InvContSociedad sociedad = new InvContSociedad();
            sociedadId = 1;
            sociedad.Id = sociedadId;
            modificatorio.InvContSociedad = sociedad;
            modificatorio.RequiereRFP = false;
            modificatorio.FechaEmision = DateTime.Parse(txtFechaEmision.Text);
            modificatorio.FechaInicioVigencia = DateTime.Parse(txtFechaInicioV.Text);
            modificatorio.FechaFinVigencia = DateTime.Parse(txtFechaFinV.Text);
            int mesDuracionConvenio;
            Int32.TryParse(txtMesDuracion.Text, out mesDuracionConvenio);
            modificatorio.MesesDuracionConvenio = mesDuracionConvenio;
            modificatorio.CompradorNombre = txtNombreComprador.Text;
            modificatorio.CompradorTelExt = txtTelComprador.Text;
            int puesto;
            Int32.TryParse(DpdPuesto.SelectedValue, out puesto);
            modificatorio.CompradorPuesto = puesto;
            modificatorio.CompradorEmail = txtEmailComprador.Text;
            modificatorio.CompradorArea = txtAreaComprador.Text;
            modificatorio.CuentaContable = txtCuentaContable.Text;
            decimal montoTotalMonedaOriginal;
            Decimal.TryParse(txtMontoTotalMO.Text, out montoTotalMonedaOriginal);
            modificatorio.MontoTotalMonedaOriginal = montoTotalMonedaOriginal;
            decimal montoMXN;
            Decimal.TryParse(txtMontoMXN.Text, out montoMXN);
            modificatorio.MontoTotalMXN = montoMXN;
            modificatorio.MonedaOriginal = Int32.Parse(DpdMoneda.SelectedValue);
            decimal tipodeCambio;
            Decimal.TryParse(txtTipoCambio.Text, out tipodeCambio);
            modificatorio.TipoDeCambio = tipodeCambio;
            modificatorio.Descripcion = txtDescripcion.Text;
            modificatorio.Comentarios = txtComentarios.Text;
            modificatorio.UsuarioUltAct = 0;
            bool RFP;
            if (rbSi.Checked)
            {
                RFP = true;
            }
            else
            {
                RFP = false;
            }
            RFP = false;
            modificatorio.RequiereRFP = RFP;
            int cantidadPagos;
            Int32.TryParse(txtCantidadPagos.Text, out cantidadPagos);
            modificatorio.CantidadPagosProgramados = cantidadPagos;

            WebServiceSoapClient webService = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    result = webService.InsertarConvenio(modificatorio, Util.Encrypt(DSODataContext.ConnectionString));
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext;
            if (result == "Convenio dado de alta correctamente.")
            {
                cstext = "alert('" + result + "'); location.href = 'AltaConvenio.aspx';";
            }
            else
            {
                cstext = "alert('" + result + "');";
            }

            cs.RegisterStartupScript(cstype, "", cstext, true);

        }

        //Master
        protected void lnkBuscar_Click(object sender, EventArgs e)
        {
            //string parametro = txtBuscar.Text;
            //Response.Redirect("~/invContratos/Pantallas/ResultadoEspecifico.aspx?parametro=" + parametro);
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