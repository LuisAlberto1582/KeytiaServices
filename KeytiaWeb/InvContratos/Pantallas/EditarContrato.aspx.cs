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
    public partial class Pantallas_EditarContrato : System.Web.UI.Page
    {
        List<EncabezadoPrevio> encabezado = new List<EncabezadoPrevio>();
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
            if (!IsPostBack == true)
            {
                string folioContrato = String.IsNullOrEmpty(Request.QueryString["folio"]) ? "" : Request.QueryString["folio"].ToString();
                txtFolioHead.Text = folioContrato;
                DpdPuesto_SelectedIndexChanged();

                ContratoData(folioContrato);
                DpdCategoria_SelectedIndexChanged();
                dpdCategoriaServicio_SelectedIndexChanged();
                DpdEstatus_SelectedIndexChanged();
                DpdArea_SelectedIndexChanged();
                DpdRegion_SelectedIndexChanged();
                CListSociedades_SelectedIndexChanged();
                DpdMoneda_SelectedIndexChanged();
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        /// <summary>
        /// Regresa información del contrato.
        /// </summary>
        /// <param name="folio"></param>
        public void ContratoData(string folio)
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonEncabezado = service.DevuelveEncabezadoContratoJSON(folio, Util.Encrypt(DSODataContext.ConnectionString));
                encabezado = (new JavaScriptSerializer()).Deserialize<List<EncabezadoPrevio>>(jsonEncabezado);
                gvPrevioDetalle.DataSource = encabezado;
                gvPrevioDetalle.DataBind();

            }

            DetalleData(Int32.Parse(encabezado[0].Id.ToString()));
        }

        /// <summary>
        /// Llena los textbox y dropdownlist de la pantalla con la información del contrato.
        /// </summary>
        /// <param name="folio"></param>
        public void DetalleData(int id)
        {
            List<InvContContrato> contratos = new List<InvContContrato>();
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                //Comienza a llenar los campos con la información del contrato.
                string jsonDetalle = service.GetContrato_Id(id, Util.Encrypt(DSODataContext.ConnectionString));
                contratos = (new JavaScriptSerializer()).Deserialize<List<InvContContrato>>(jsonDetalle);
                txtIdContrato.Text = contratos[0].Id.ToString();
                txtSolicitanteNombre.Text = contratos[0].SolicitanteNombre;
                txtMontoMXN.Text = contratos[0].MontoTotalMXN.ToString();
                txtTelSolicitante.Text = contratos[0].SolicitanteTelExt;
                DpdMoneda.SelectedValue = contratos[0].MonedaOriginal.ToString();
                DpdPuestos.SelectedValue = contratos[0].SolicitantePuesto.ToString();
                txtTipoCambio.Text = contratos[0].TipoDeCambio.ToString();
                txtNombreComprador.Text = contratos[0].CompradorNombre;
                dpdCategoria.SelectedValue = contratos[0].InvContConvenioTipo.Id.ToString();
                txtTelComprador.Text = contratos[0].CompradorTelExt;
                dpdCategoriaServicio.SelectedValue = contratos[0].InvContTipoServicio.Id.ToString();
                txtEmailComprador.Text = contratos[0].CompradorEmail;
                DpdEstatus.SelectedValue = contratos[0].InvContConvenioEstatus.Id.ToString();
                DpdPuesto.SelectedValue = contratos[0].CompradorPuesto.ToString();
                DpdArea.SelectedValue = contratos[0].InvContArea.Id.ToString();
                txtAreaComprador.Text = contratos[0].CompradorArea;
                DpdRegion.SelectedValue = contratos[0].InvContRegion.Id.ToString();
                txtFechaSolicitud.Text = DateTime.Parse(contratos[0].FechaSolicitud.ToString()).ToString("yyyy/MM/dd");
                if (contratos[0].RequiereRFP == true)
                {
                    rbSi.Checked = true;
                }
                else
                {
                    rbNo.Checked = true;
                }
                txtFechaEmision.Text = DateTime.Parse(contratos[0].FechaEmision.ToString()).ToString("yyyy/MM/dd");
                txtDescripcion.Text = contratos[0].Descripcion;
                txtFechaInicioV.Text = DateTime.Parse(contratos[0].FechaInicioVigencia.ToString()).ToString("yyyy/MM/dd");
                txtFechaFinV.Text = DateTime.Parse(contratos[0].FechaFinVigencia.ToString()).ToString("yyyy/MM/dd");
                txtCuentaContable.Text = contratos[0].CuentaContable;
                txtMesDuracion.Text = contratos[0].MesesDuracionConvenio.ToString();
                txtClave.Text = contratos[0].Clave;
                txtComentarios.Text = contratos[0].Comentarios;
                txtFolioRelacionado.Text = contratos[0].FolioRelacionado;
                txtMontoTotalMO.Text = contratos[0].MontoTotalMonedaOriginal.ToString();
                if (contratos[0].InvContSociedad.Id > 0)
                {
                    CListSociedades.SelectedValue = contratos[0].InvContSociedad.Id.ToString();
                }
            }
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
        /// Guarda los cambios
        /// </summary>
        protected void GuardarCambios()
        {
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;

            string result;
            //Rellena el objeto que se va a mandar al webservice.
            InvContContrato contrato = new InvContContrato();
            contrato.SolicitanteNombre = txtSolicitanteNombre.Text;
            decimal montoMXN;
            Decimal.TryParse(txtMontoMXN.Text, out montoMXN);
            contrato.MontoTotalMXN = montoMXN;
            contrato.SolicitanteTelExt = txtTelSolicitante.Text;
            contrato.MonedaOriginal = Int32.Parse(DpdMoneda.SelectedValue);
            contrato.SolicitantePuesto = Int32.Parse(DpdPuestos.SelectedValue);
            decimal tipodeCambio;
            Decimal.TryParse(txtTipoCambio.Text, out tipodeCambio);
            contrato.TipoDeCambio = tipodeCambio;
            contrato.CompradorNombre = txtNombreComprador.Text;
            contrato.CompradorTelExt = txtTelComprador.Text;
            WSSS.InvContConvenioTipo convenioTipo = new WSSS.InvContConvenioTipo();
            convenioTipo.Id = Int32.Parse(dpdCategoria.SelectedValue);
            contrato.InvContConvenioTipo = convenioTipo;

            contrato.CompradorEmail = txtEmailComprador.Text;

            WSSS.InvContConvenioEstatus convenioEstatus = new WSSS.InvContConvenioEstatus();
            convenioEstatus.Id = Int32.Parse(DpdEstatus.SelectedValue);
            contrato.InvContConvenioEstatus = convenioEstatus;

            contrato.CompradorPuesto = Int32.Parse(DpdPuesto.SelectedValue);

            WSSS.InvContArea area = new WSSS.InvContArea();
            area.Id = Int32.Parse(DpdArea.SelectedValue);
            contrato.InvContArea = area;

            contrato.CompradorArea = txtAreaComprador.Text;

            WSSS.InvContRegion region = new WSSS.InvContRegion();
            region.Id = Int32.Parse(DpdRegion.SelectedValue);
            contrato.InvContRegion = region;

            int sociedadId;
            Int32.TryParse(CListSociedades.SelectedValue, out sociedadId);
            InvContSociedad sociedad = new InvContSociedad();
            sociedadId = 1;
            sociedad.Id = sociedadId;
            contrato.InvContSociedad = sociedad;

            WSSS.InvContTipoServicio tipoServicio = new WSSS.InvContTipoServicio();
            tipoServicio.Id = Int32.Parse(dpdCategoriaServicio.SelectedValue);
            contrato.InvContTipoServicio = tipoServicio;

            contrato.FechaSolicitud = DateTime.Parse(txtFechaSolicitud.Text);
            bool requiereRFP;
            if (rbSi.Checked)
            {
                requiereRFP = true;
            }
            else
            {
                requiereRFP = false;
            }
            requiereRFP = true;
            contrato.RequiereRFP = requiereRFP;
            contrato.FechaEmision = DateTime.Parse(txtFechaEmision.Text);
            contrato.Descripcion = txtDescripcion.Text;
            contrato.FechaInicioVigencia = DateTime.Parse(txtFechaInicioV.Text);
            contrato.FechaFinVigencia = DateTime.Parse(txtFechaFinV.Text);
            contrato.CuentaContable = txtCuentaContable.Text;
            int mesDuracionConvenio;
            Int32.TryParse(txtMesDuracion.Text, out mesDuracionConvenio);
            contrato.MesesDuracionConvenio = mesDuracionConvenio;
            contrato.Clave = txtClave.Text;
            contrato.Comentarios = txtComentarios.Text;
            contrato.FolioRelacionado = txtFolioRelacionado.Text;
            decimal montoTotalMonedaOriginal;
            Decimal.TryParse(txtMontoTotalMO.Text, out montoTotalMonedaOriginal);
            contrato.MontoTotalMonedaOriginal = montoTotalMonedaOriginal;
            contrato.UsuarioUltAct = 0;
            contrato.Id = Int32.Parse(txtIdContrato.Text);

            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    result = service.ModificaContrato(contrato, Util.Encrypt(DSODataContext.ConnectionString));
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            String cstext = "var estatus = '" + result + "'; " +
                            "alert(estatus); " +
                            "if (estatus=='El registro fue cambiado correctamente.')" +
                            "{" +
                            " location.href = 'DetalleContrato.aspx?Folio=" + txtFolioHead.Text.ToString() + "&Id=" + txtIdContrato.Text + "&Estatus=" + DpdEstatus.SelectedItem + "';" +
                            "}";
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        /// <summary>
        /// Validación de campos de fecha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            DateTime fecha;
            if (DateTime.TryParseExact(txtFechaEmision.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
            {
                if (DateTime.TryParseExact(txtFechaInicioV.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
                {
                    if (DateTime.TryParseExact(txtFechaFinV.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
                    {
                        if (DateTime.TryParseExact(txtFechaSolicitud.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
                        {
                            GuardarCambios();
                        }
                        else
                        {
                            Type cstype = this.GetType();
                            ClientScriptManager cs = Page.ClientScript;
                            String cstext = "alert('Favor de insertar un formato de fecha válido para la Fecha de Solicitud. Ejemplo: 2018/12/31');";
                            cs.RegisterStartupScript(cstype, "", cstext, true);
                        }
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
                DpdPuestos.DataSource = puestos.ToList();
                DpdPuestos.DataBind();
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