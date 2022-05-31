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
    public partial class Pantallas_EditarConvenio : System.Web.UI.Page
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
            if (list.Count <= 1)
            {
                btnRegresar.Visible = false;
            }
            else
            {
                btnRegresar.Visible = true;
            }
            #endregion

            string folioContrato = String.IsNullOrEmpty(Request.QueryString["folio"]) ? "" : Request.QueryString["folio"].ToString();
            string folioConvenio = String.IsNullOrEmpty(Request.QueryString["folioConvenio"]) ? "" : Request.QueryString["folioConvenio"].ToString();
            txtFolioHead.Text = folioContrato;
            txtFolioConv.Text = folioConvenio;
            if (!IsPostBack == true)
            {
                ConvenioData(folioConvenio);
                DpdCategoria_SelectedIndexChanged();
                dpdCategoriaServicio_SelectedIndexChanged();
                DpdEstatus_SelectedIndexChanged();
                DpdArea_SelectedIndexChanged();
                DpdRegion_SelectedIndexChanged();
                DpdPuesto_SelectedIndexChanged();
                CListSociedades_SelectedIndexChanged();
                DpdMoneda_SelectedIndexChanged();
            }
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected void ConvenioData(string folio)
        {
            List<EncabezadoPrevio> encabezados = new List<EncabezadoPrevio>();
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string json = service.DevuelveEncabezadoConvenioJSON(folio, Util.Encrypt(DSODataContext.ConnectionString));
                encabezados = (new JavaScriptSerializer()).Deserialize<List<EncabezadoPrevio>>(json);
                gvConvenio.DataSource = encabezados;
                gvConvenio.DataBind();
            }
            ConvenioDetalle(encabezados[0].Id);
        }

        protected void ConvenioDetalle(Int32 id)
        {
            List<InvContConvModificatorio> modificatorios = new List<InvContConvModificatorio>();
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string json = service.GetConvenio_Id(id, Util.Encrypt(DSODataContext.ConnectionString));
                modificatorios = (new JavaScriptSerializer()).Deserialize<List<InvContConvModificatorio>>(json);
                txtIdConvenio.Text = modificatorios[0].Id.ToString();
                txtMontoMXN.Text = modificatorios[0].MontoTotalMXN.ToString();
                DpdMoneda.SelectedValue = modificatorios[0].MonedaOriginal.ToString();
                txtTipoCambio.Text = modificatorios[0].TipoDeCambio.ToString();
                txtNombreComprador.Text = modificatorios[0].CompradorNombre;
                dpdCategoria.SelectedValue = modificatorios[0].InvContConvenioTipo.Id.ToString();
                txtTelComprador.Text = modificatorios[0].CompradorTelExt;
                dpdCategoriaServicio.SelectedValue = modificatorios[0].InvContTipoServicio.Id.ToString();
                txtEmailComprador.Text = modificatorios[0].CompradorEmail;
                DpdEstatus.SelectedValue = modificatorios[0].InvContConvenioEstatus.Id.ToString();
                DpdPuesto.SelectedValue = modificatorios[0].CompradorPuesto.ToString();
                DpdArea.SelectedValue = modificatorios[0].InvContArea.Id.ToString();
                txtAreaComprador.Text = modificatorios[0].CompradorArea;
                DpdRegion.SelectedValue = modificatorios[0].InvContRegion.Id.ToString();
                if (modificatorios[0].RequiereRFP == true)
                {
                    rbSi.Checked = true;
                }
                else
                {
                    rbNo.Checked = true;
                }
                txtFechaEmision.Text = DateTime.Parse(modificatorios[0].FechaEmision.ToString()).ToString("yyyy/MM/dd");
                txtDescripcion.Text = modificatorios[0].Descripcion;
                txtFechaInicioV.Text = DateTime.Parse(modificatorios[0].FechaInicioVigencia.ToString()).ToString("yyyy/MM/dd");
                txtFechaFinV.Text = DateTime.Parse(modificatorios[0].FechaFinVigencia.ToString()).ToString("yyyy/MM/dd");
                txtFolioContrato.Text = modificatorios[0].FolioContrato;
                txtFolioAnexo.Text = modificatorios[0].FolioAnexo;
                txtCuentaContable.Text = modificatorios[0].CuentaContable;
                txtMesDuracion.Text = modificatorios[0].MesesDuracionConvenio.ToString();
                txtComentarios.Text = modificatorios[0].Comentarios;
                txtMontoTotalMO.Text = modificatorios[0].MontoTotalMonedaOriginal.ToString();
                if (modificatorios[0].InvContSociedad.Id > 0)
                {
                    CListSociedades.SelectedValue = modificatorios[0].InvContSociedad.Id.ToString();
                }
            }
        }

        /// <summary>
        /// Drop down list de ConvenioTipo
        /// </summary>
        protected void DpdCategoria_SelectedIndexChanged()
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
        /// Evento para guardar los cambios
        /// </summary>
        protected void GuardarCambios()
        {
            string result;
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            InvContConvModificatorio modificatorio = new InvContConvModificatorio();
            decimal montoTotalMXN;
            Decimal.TryParse(txtMontoMXN.Text, out montoTotalMXN);
            modificatorio.MontoTotalMXN = montoTotalMXN;
            modificatorio.MonedaOriginal = Int32.Parse(DpdMoneda.SelectedValue);
            decimal tipodeCambio;
            Decimal.TryParse(txtTipoCambio.Text, out tipodeCambio);
            modificatorio.TipoDeCambio = tipodeCambio;
            modificatorio.CompradorNombre = txtNombreComprador.Text;
            modificatorio.CompradorTelExt = txtTelComprador.Text;
            WSSS.InvContConvenioTipo convenioTipo = new WSSS.InvContConvenioTipo();
            convenioTipo.Id = Int32.Parse(dpdCategoria.SelectedValue);
            modificatorio.InvContConvenioTipo = convenioTipo;

            modificatorio.CompradorEmail = txtEmailComprador.Text;

            WSSS.InvContConvenioEstatus convenioEstatus = new WSSS.InvContConvenioEstatus();
            convenioEstatus.Id = Int32.Parse(DpdEstatus.SelectedValue);
            modificatorio.InvContConvenioEstatus = convenioEstatus;

            modificatorio.CompradorPuesto = Int32.Parse(DpdPuesto.SelectedValue);

            WSSS.InvContArea area = new WSSS.InvContArea();
            area.Id = Int32.Parse(DpdArea.SelectedValue);
            modificatorio.InvContArea = area;

            modificatorio.CompradorArea = txtAreaComprador.Text;

            WSSS.InvContRegion region = new WSSS.InvContRegion();
            region.Id = Int32.Parse(DpdRegion.SelectedValue);
            modificatorio.InvContRegion = region;

            int sociedadId;
            Int32.TryParse(CListSociedades.SelectedValue, out sociedadId);
            InvContSociedad sociedad = new InvContSociedad();
            sociedadId = 1;
            sociedad.Id = sociedadId;
            modificatorio.InvContSociedad = sociedad;

            WSSS.InvContTipoServicio tipoServicio = new WSSS.InvContTipoServicio();
            tipoServicio.Id = Int32.Parse(dpdCategoriaServicio.SelectedValue);
            modificatorio.InvContTipoServicio = tipoServicio;

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
            modificatorio.RequiereRFP = requiereRFP;
            modificatorio.FechaEmision = DateTime.Parse(txtFechaEmision.Text);
            modificatorio.Descripcion = txtDescripcion.Text;
            modificatorio.FechaInicioVigencia = DateTime.Parse(txtFechaInicioV.Text);
            modificatorio.FechaFinVigencia = DateTime.Parse(txtFechaFinV.Text);
            modificatorio.FolioContrato = txtFolioContrato.Text;
            modificatorio.FolioAnexo = txtFolioAnexo.Text;
            modificatorio.CuentaContable = txtCuentaContable.Text;
            int mesesDuracionConvenio;
            Int32.TryParse(txtMesDuracion.Text, out mesesDuracionConvenio);
            modificatorio.MesesDuracionConvenio = mesesDuracionConvenio;
            modificatorio.Comentarios = txtComentarios.Text;
            decimal montoTotalMonedaOriginal;
            Decimal.TryParse(txtMontoTotalMO.Text, out montoTotalMonedaOriginal);
            modificatorio.MontoTotalMonedaOriginal = montoTotalMonedaOriginal;
            modificatorio.UsuarioUltAct = 0;
            modificatorio.Id = Int32.Parse(txtIdConvenio.Text);
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    result = service.ModificaConvenio(modificatorio, Util.Encrypt(DSODataContext.ConnectionString));
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            String cstext = "var estatus = '" + result + "'; " +
                            "alert(estatus); " +
                            "if (estatus=='Ok')" +
                            "{" +
                            " location.href = 'DetalleConvenio.aspx?Folio=" + txtFolioContrato.Text.ToString() + "&FolioConvenio=" + txtFolioConv.Text + "&Id=" + txtIdConvenio.Text + "';" +
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
                        GuardarCambios();
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
                    String cstext = "alert('Favor de insertar un formato de fecha válido para la Fecha Inicio Vigencia. Ejemplo: 2018/12/31');";
                    cs.RegisterStartupScript(cstype, "", cstext, true);
                }
            }
            else
            {
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('Favor de insertar un formato de fecha válido para la Fecha Emisión. Ejemplo: 2018/12/31');";
                cs.RegisterStartupScript(cstype, "", cstext, true);
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