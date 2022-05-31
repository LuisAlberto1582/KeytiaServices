﻿using KeytiaWeb.InvContratos.App_Code.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaWeb.WSSS;
using System.IO;
using KeytiaServiceBL;
using System.Globalization;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class DetalleConvenio : System.Web.UI.Page
    {
        List<KeytiaWeb.InvContratos.App_Code.Models.DetallePrevio> previo = new List<KeytiaWeb.InvContratos.App_Code.Models.DetallePrevio>();
        List<EncabezadoPrevio> encabezado = new List<EncabezadoPrevio>();
        int i;

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

            string estatus = String.IsNullOrEmpty(Request.QueryString["Estatus"]) ? "" : Request.QueryString["Estatus"].ToString();
            Session["Estatus"] = estatus;
            string folioContrato = String.IsNullOrEmpty(Request.QueryString["Folio"]) ? "" : Request.QueryString["Folio"].ToString();
            Session["folio"] = folioContrato;
            string folioConvenio = String.IsNullOrEmpty(Request.QueryString["FolioConvenio"]) ? "" : Request.QueryString["FolioConvenio"].ToString();
            Session["folioConvenio"] = folioConvenio;
            txtFolioContrato.Text = folioContrato;
            txtFolioConvenio.Text = folioConvenio;
            txtFolioConvenio2.Text = folioConvenio;
            int idConvenio = int.Parse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? "" : Request.QueryString["Id"].ToString());
            Session["IdConvenio"] = idConvenio;
            string url = String.IsNullOrEmpty(Request.QueryString["Url"]) ? "" : Request.QueryString["Url"].ToString();
            Session["url"] = url;
            if (!Page.IsPostBack)
            {
                ConvenioData(idConvenio);
                ElementosData();
                RelacionPagosData();
                ArchivosData();
                DpdElemento_SelectedIndexChanged();
                DpdMoneda_SelectedIndexChanged();
                DpMonedaElemCont_SelectedIndexChanged();
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;

        }

        /// <summary>
        /// Carga el grid de convenios.
        /// </summary>
        /// <param name="folio"></param>
        public void ConvenioData(int idConvenio)
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                String jsonEncabezado = service.DevuelveEncabezadoConvenioJSON_id(idConvenio, Util.Encrypt(DSODataContext.ConnectionString));

                encabezado = (new JavaScriptSerializer()).Deserialize<List<EncabezadoPrevio>>(jsonEncabezado);
                DateTime fechaM = DateTime.Parse("01/01/1900");
                i = 0;
                gvConvenio.DataSource = encabezado;
                gvConvenio.DataBind();

            }

            i = 0;

            //Asigna los valores a los textbox que corresponden al detalle previo.
            foreach (GridViewRow row in gvConvenio.Rows)
            {

                string JSON =
                    service.DevuelvePrevioConvenioJSON(encabezado[i].Folio, Util.Encrypt(DSODataContext.ConnectionString));
                if (JSON != "[]")
                {

                    previo = (new JavaScriptSerializer()).Deserialize<List<KeytiaWeb.InvContratos.App_Code.Models.DetallePrevio>>(JSON);
                    TextBox idConveniotxt = (TextBox)row.FindControl("txtConvenioId");
                    idConveniotxt.Text = previo[0].Id;
                    Session["ConvenioId"] = previo[0].Id;
                    TextBox txtFolio = (TextBox)row.FindControl("txtFolioC");
                    txtFolio.Text = previo[0].Folio;
                    TextBox txtClave = (TextBox)row.FindControl("txtClaveC");
                    txtClave.Text = previo[0].Clave;
                    TextBox txtFolioRelacionado = (TextBox)row.FindControl("txtFolioRelacionadoC");
                    txtFolioRelacionado.Text = previo[0].RelacionFolio;
                    TextBox txtProveedor = (TextBox)row.FindControl("txtProveedorC");
                    txtProveedor.Text = previo[0].Proveedor;
                    TextBox txtVigencia = (TextBox)row.FindControl("txtVigenciaC");
                    txtVigencia.Text = previo[0].Vigente;
                    if (txtVigencia.Text == "True")
                    {
                        txtVigencia.Text = "S";
                    }
                    else
                    {
                        txtVigencia.Text = "N";
                    }
                    TextBox txtTipoContrato = (TextBox)row.FindControl("txtTipoContratoC");
                    txtTipoContrato.Text = previo[0].CategoriaConvenio;
                    TextBox txtTipoServicio = (TextBox)row.FindControl("txtTipoServicioC");
                    txtTipoServicio.Text = previo[0].CategoriaServicio;
                    TextBox txtSolicitante = (TextBox)row.FindControl("txtSolicitanteC");
                    txtSolicitante.Text = previo[0].SolicitanteNombre;
                    TextBox txtComprador = (TextBox)row.FindControl("txtCompradorC");
                    txtComprador.Text = previo[0].CompradorNombre;
                    TextBox txtInicio = (TextBox)row.FindControl("txtInicioC");
                    txtInicio.Text = DateTime.ParseExact(previo[0].FechaInicioVigencia,"yy/MM/dd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    //txtInicio.Text = DateTime.Parse(previo[0].FechaInicioVigencia).ToString("yyyy-MM-dd");
                    TextBox txtFin = (TextBox)row.FindControl("txtFinC");
                    txtFin.Text = DateTime.ParseExact(previo[0].FechaFinVigencia, "yy/MM/dd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    //txtFin.Text = DateTime.Parse(previo[0].FechaFinVigencia).ToString("yyyy-MM-dd");
                    TextBox txtDescripcion = (TextBox)row.FindControl("txtDescripcionC");
                    txtDescripcion.Text = previo[0].Descripcion;
                }


                i += 1;
            }
        }

        /// <summary>
        /// Carga el grid de elementos
        /// </summary>
        public void ElementosData()
        {
            int idConvenio = (Int32)Session["IdConvenio"];


            List<App_Code.Models.ElementoContratado> elemento = new List<App_Code.Models.ElementoContratado>();
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonEncabezado = service.DevuelveConvRelacionElementosJSON(idConvenio, Util.Encrypt(DSODataContext.ConnectionString));

                elemento = (new JavaScriptSerializer()).Deserialize<List<App_Code.Models.ElementoContratado>>(jsonEncabezado);

                //RM20190123
                foreach (App_Code.Models.ElementoContratado el in elemento)
                {
                    el.CostoUnitarioMXN = Convert.ToDouble(el.CostoUnitarioMXN).ToString("C");
                    el.CostoUnitarioMonedaOriginal = Convert.ToDouble(el.CostoUnitarioMonedaOriginal).ToString("C");
                    el.TipoDeCambio = Convert.ToDouble(el.TipoDeCambio).ToString("C");
                }
                gvElementos.DataSource = elemento;
                gvElementos.DataBind();

            }
        }

        /// <summary>
        /// Carga el grid de pagos
        /// </summary>
        public void RelacionPagosData()
        {
            int idConvenio = (Int32)Session["IdConvenio"];

            List<RelacionDePago> elemento = new List<RelacionDePago>();
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonEncabezado = service.DevuelveConvRelacionDePagosJSON(idConvenio, Util.Encrypt(DSODataContext.ConnectionString));

                elemento = (new JavaScriptSerializer()).Deserialize<List<RelacionDePago>>(jsonEncabezado);

                //RM20190123
                foreach (RelacionDePago rel in elemento)
                {
                    rel.FechaPago = rel.FechaPago.Substring(0, 10);
                    rel.ImporteMonedaOriginal = Convert.ToDouble(rel.ImporteMonedaOriginal).ToString("C");
                    rel.ImporteMXN = Convert.ToDouble(rel.ImporteMXN).ToString("C");
                    rel.TipoDeCambio = Convert.ToDouble(rel.TipoDeCambio).ToString("C");

                }

                gvRelacionPagos.DataSource = elemento;
                gvRelacionPagos.DataBind();

            }
        }

        /// <summary>
        /// Carga drop down list de elemento
        /// </summary>
        protected void DpdElemento_SelectedIndexChanged()
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
        /// Insertar nuevos elementos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InsertaElementos(object sender, EventArgs e)
        {
            int idConvenio = (Int32)Session["IdConvenio"];

            int Cantidad = 0;
            Int32.TryParse(txtCantidad.Text, out Cantidad);

            int idElemento = 0;
            int.TryParse(DpdElemento.SelectedValue, out idElemento);

            int Moneda = 0;
            Int32.TryParse(DpMonedaElemCont.Text, out Moneda);
            if (Moneda == 0 && DpdMoneda != null && DpdMoneda.Items.Count > 0)
            {
                Moneda = Convert.ToInt32(DpdMoneda.Items.FindByText("MXP").Value);
            }

            string res = string.Empty;

            if (idConvenio != null && idConvenio > 0)
            {
                if (Moneda > 0)
                {
                    if (Cantidad > 0)
                    {
                        if (idElemento > 0)
                        {
                            InvContConvModificatorio modificatorio = new InvContConvModificatorio();
                            WSSS.InvContElemento elementos = new WSSS.InvContElemento();
                            WSSS.InvContConvModificatorioRelacionElementos elemento = new WSSS.InvContConvModificatorioRelacionElementos();

                            modificatorio.Id = idConvenio;

                            elemento.InvContConvModificatorio = modificatorio;
                            elementos.Id = Int32.Parse(DpdElemento.SelectedValue);
                            elemento.InvContElemento = elementos;

                            elemento.Cantidad = Cantidad;

                            //RM20190123
                            Decimal PU = 0;
                            decimal.TryParse(txtCostoUnitarioMXN.Text, out PU);
                            elemento.PrecioUnitarioMXN = PU;

                            Decimal PrecioUnitarioMoneda = 0;
                            Decimal.TryParse(txtCostoUnitarioMonedaOriginal.Text, out PrecioUnitarioMoneda);
                            elemento.PrecioUnitarioMonedaOriginal = PrecioUnitarioMoneda;

                            Decimal TipoCambio = 0;
                            Decimal.TryParse(txtTipodeCambio.Text, out TipoCambio);
                            elemento.TipoDeCambio = TipoCambio;

                            elemento.Moneda = Moneda;
                            elemento.UsuarioUltAct = 0;

                            WebServiceSoapClient service = new WebServiceSoapClient();
                            using (WebClient client = new WebClient())
                            {
                                string jsonEncabezado = service.InsertarConvRelacionElementos(elemento, Util.Encrypt(DSODataContext.ConnectionString));

                                res = jsonEncabezado.ToLower() == "ok" ? "Elemento dado de alta correctamente. " : "Ocurrio un error al dar de alta el elemento.";
                            }

                            List<App_Code.Models.ElementoContratado> elementoG = new List<App_Code.Models.ElementoContratado>();
                            WebServiceSoapClient services = new WebServiceSoapClient();
                            using (WebClient client = new WebClient())
                            {
                                string jsonEncabezado = service.DevuelveConvRelacionDeElementosJSON(modificatorio.Id, Util.Encrypt(DSODataContext.ConnectionString));

                                elementoG = (new JavaScriptSerializer()).Deserialize<List<App_Code.Models.ElementoContratado>>(jsonEncabezado);

                                //RM20190123
                                foreach (App_Code.Models.ElementoContratado el in elementoG)
                                {
                                    el.CostoUnitarioMXN = Convert.ToDouble(el.CostoUnitarioMXN).ToString("C");
                                    el.CostoUnitarioMonedaOriginal = Convert.ToDouble(el.CostoUnitarioMonedaOriginal).ToString("C");
                                    el.TipoDeCambio = Convert.ToDouble(el.TipoDeCambio).ToString("C");
                                }

                                gvElementos.DataSource = elementoG;
                                gvElementos.DataBind();


                                txtCantidad.Text = "";
                                DpdElemento.Text = "0";
                                txtCostoUnitarioMXN.Text = "";
                                txtCostoUnitarioMonedaOriginal.Text = "";
                                txtTipodeCambio.Text = "";
                                DpMonedaElemCont.Text = "0";

                            }
                        }
                        else
                        {
                            res = "Favor de seleccionar un elemento contratado.";
                        }
                    }
                    else
                    {
                        res = "Favor de ingresar una cantidad mayor a 0.";
                    }
                }
                else
                {
                    res = "Error no especificado. ";
                }
            }

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "alert('" + res + "');";
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        /// <summary>
        /// Carga el drop down list de moneda
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
        /// Carga el grid de moneda
        /// </summary>
        protected void DpMonedaElemCont_SelectedIndexChanged()
        {
            WebServiceSoapClient servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<Monedas> moneda = new List<Monedas>();
                string JSON = servicio.DevuelveDropDownMonedasJSON(Util.Encrypt(DSODataContext.ConnectionString));

                moneda = (new JavaScriptSerializer()).Deserialize<List<Monedas>>(JSON);
                DpMonedaElemCont.DataSource = moneda.ToList();
                DpMonedaElemCont.DataBind();
            }

        }

        /// <summary>
        /// Insertar nuevos pagos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InsertaRelacionPagos(object sender, EventArgs e)
        {
            DateTime fecha;

            bool insertar = true;
            string res = string.Empty;

            if (!DateTime.TryParseExact(txtFechaPago.Text, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out fecha))
            {
                res += "Favor de insertar un formato de fecha válido.Ejemplo: 2018 / 12 / 31. \n";
                insertar = false;
            }


            int numPago = 0;
            int.TryParse(txtPagoNumero.Text, out numPago);
            if (numPago == 0)
            {
                res += "Favor de ingresar el numero de pago. \n";
                insertar = false;
            }

            double importeMx = 0;
            double.TryParse(txtImporteMXN.Text, out importeMx);
            if (importeMx == 0)
            {
                res += "Favor de ingresar el importe en moneda mexicana. \n";
                insertar = false;
            }

            double importeMoneda = 0;
            double.TryParse(txtImporteMoneda.Text, out importeMoneda);
            if (importeMoneda == 0)
            {
                res += "Favor de ingresar el importe en moneda.\n ";
                insertar = false;
            }

            double tipocambio = 0;
            double.TryParse(txtTipoCambio.Text, out tipocambio);
            if (tipocambio == 0)
            {
                res += "Favor de ingresar el tipo de cambio.\n ";
                insertar = false;
            }

            int moneda = 0;
            int.TryParse(DpdMoneda.SelectedValue, out moneda);
            if (tipocambio == 0)
            {
                res += "Favor de seleccionar la moneda.\n ";
                insertar = false;
            }

            

            if (insertar)
            {
                int idConvenio = (Int32)Session["IdConvenio"];
                InvContConvModificatorio modificatorio = new InvContConvModificatorio();
                modificatorio.Id = idConvenio;

                WSSS.InvContConvModificatorioRelacionDePagos relacion = new WSSS.InvContConvModificatorioRelacionDePagos();
                relacion.InvContConvModificatorio = modificatorio;
                int Pago = int.Parse(txtPagoNumero.Text);
                relacion.PagoNumero = Pago;
                DateTime FechaPago = DateTime.Parse(txtFechaPago.Text);
                relacion.FechaPago = FechaPago;
                decimal ImporteMXN = decimal.Parse(txtImporteMXN.Text);
                relacion.ImporteMXN = ImporteMXN;
                decimal ImporteMonedaOriginal = decimal.Parse(txtImporteMoneda.Text);
                relacion.ImporteMonedaOriginal = ImporteMonedaOriginal;
                decimal TipoDeCambio = decimal.Parse(txtTipoCambio.Text);
                relacion.TipoDeCambio = TipoDeCambio;
                int monedaOriginal = int.Parse(DpdMoneda.Text);
                relacion.MonedaOriginal = monedaOriginal;
                relacion.UsuarioUltAct = 0;


                WebServiceSoapClient service = new WebServiceSoapClient();
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        res = service.InsertarConvRelacionPagos(relacion, Util.Encrypt(DSODataContext.ConnectionString));
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }


                List<RelacionDePago> elemento = new List<RelacionDePago>();
                WebServiceSoapClient services = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    string jsonEncabezado = service.DevuelveConvRelacionPagosJSON(modificatorio.Id, Util.Encrypt(DSODataContext.ConnectionString));
                    elemento = (new JavaScriptSerializer()).Deserialize<List<RelacionDePago>>(jsonEncabezado);

                    //RM20190123
                    foreach (RelacionDePago rel in elemento)
                    {
                        rel.FechaPago = rel.FechaPago.Substring(0, 10);
                        rel.ImporteMonedaOriginal = Convert.ToDouble(rel.ImporteMonedaOriginal).ToString("C");
                        rel.ImporteMXN = Convert.ToDouble(rel.ImporteMXN).ToString("C");
                        rel.TipoDeCambio = Convert.ToDouble(rel.TipoDeCambio).ToString("C");

                    }

                    gvRelacionPagos.DataSource = elemento;
                    gvRelacionPagos.DataBind();


                    txtPagoNumero.Text = "";
                    txtFechaPago.Text = "";
                    txtImporteMXN.Text = "";
                    txtImporteMoneda.Text = "";
                    txtTipoCambio.Text = "";
                    DpdMoneda.Text = "0";


                }
            }

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "alert('" + res + "');";
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        /// <summary>
        /// Eliminar elementos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvElementos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = Convert.ToInt32(e.RowIndex);
            int id = Convert.ToInt32(gvElementos.DataKeys[index].Value);

            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                service.EliminarConvModificatorioRelacionElementos(id, Util.Encrypt(DSODataContext.ConnectionString));
            }
            ElementosData();
        }

        /// <summary>
        /// Eliminar pagos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRelacionPagos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = Convert.ToInt32(e.RowIndex);
            int id = Convert.ToInt32(gvRelacionPagos.DataKeys[index].Value);

            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                service.EliminarConvModificatorioRelacionDePagos(id, Util.Encrypt(DSODataContext.ConnectionString));
            }
            RelacionPagosData();
        }

        /// <summary>
        /// Redirige a pantalla editar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            string folioContrato = txtFolioContrato.Text;
            string folioConvenio = txtFolioConvenio.Text;
            int idConvenio = (Int32)Session["IdConvenio"];
            Response.Redirect("~/InvContratos/Pantallas/EditarConvenio.aspx?folio=" + folioContrato + "&folioConvenio=" + folioConvenio + "&id=" + idConvenio);
        }

        /// <summary>
        /// Elimina el convenio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            int convenioId = (Int32)Session["IdConvenio"];
            string result;
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;

            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    result = service.EliminarConvenio(convenioId, Util.Encrypt(DSODataContext.ConnectionString));
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            string url = "/BusquedaEspecifica.aspx";//(String)Session["url"];
            string folio = (String)Session["folio"];
            string estatus = (String)Session["Estatus"];
            string folioConvenio = (String)Session["folioConvenio"];
            String cstext;
            if (result == "Registro eliminado correctamente.")
            {
                cstext = "alert('" + result + "'); location.href ='BusquedaEspecifica.aspx';";
            }
            else
            {
                cstext = "alert('" + result + "');";
            }
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        /// <summary>
        /// Carga el grid de archivos
        /// </summary>
        public void ArchivosData()
        {
            List<UploadedFile> archivos = new List<UploadedFile>();
            int Id = (Int32)Session["IdConvenio"];

            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonDocumentos = service.DevuelveArchivosConvenioJSON(Id, Util.Encrypt(DSODataContext.ConnectionString));
                archivos = (new JavaScriptSerializer()).Deserialize<List<UploadedFile>>(jsonDocumentos);
                gvArchivos.DataSource = archivos;
                gvArchivos.DataBind();

            }
        }

        protected void btnCargar_Click(object sender, EventArgs e)
        {
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String result;
            string folio = (String)Session["folio"];
            string folioConvenio = (String)Session["folioConvenio"];
            int IdConvenio = (Int32)Session["IdConvenio"];

            string archivo = FileUploadControl.FileName.ToString();
            int tam_var = archivo.Length;
            string extension = archivo.Substring((tam_var - 4), 4);
            int IdTipoDocumento = 0;
            WebServiceSoapClient servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<InvContTipoDocumento> tipo = new List<InvContTipoDocumento>();
                string JSON = servicio.DevuelveTipoDocumentoJSON(extension, Util.Encrypt(DSODataContext.ConnectionString));
                tipo = (new JavaScriptSerializer()).Deserialize<List<InvContTipoDocumento>>(JSON);
                if (tipo.Count > 0)
                {
                    IdTipoDocumento = tipo[0].Id;
                }
            }
            if (IdTipoDocumento != 0)
            {
                String path = @"D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";

                //String path = @"C:/K6/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";
                if (Directory.Exists(path))
                {


                    Directory.CreateDirectory(path + "/Convenio " + folioConvenio + "/");
                    String path2 = "Convenio " + folioConvenio + "/" + FileUploadControl.FileName;

                    FileUploadControl.PostedFile.SaveAs(path + path2);

                    //Para tomar la ruta de archivo y guardarlo en la tabla
                    string path4 = path + path2;
                    Session["Archivo"] = path4;
                }
                else
                {
                    Directory.CreateDirectory("D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/");
                    String path3 = "D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/" + FileUploadControl.FileName;
                    //Directory.CreateDirectory("C:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/");
                    //String path3 = "C:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/" + FileUploadControl.FileName;

                    FileUploadControl.PostedFile.SaveAs(path3);

                    Session["Archivo2"] = path3;

                }

                int IdTipo = IdTipoDocumento;
                string Archivo = (String)Session["Archivo"];
                string Archivo2 = (String)Session["Archivo2"];
                bool EsVigente;

                InvContAnexo anexo = new InvContAnexo();
                anexo.Id = 0;
                InvContContrato contrato = new InvContContrato();
                contrato.Id = 0;
                InvContConvModificatorio conv = new InvContConvModificatorio();
                conv.Id = IdConvenio;

                WSSS.InvContUploadedFile file = new WSSS.InvContUploadedFile();
                file.TipoDocumentoId = IdTipo;
                file.InvContAnexo = anexo;
                file.InvContContrato = contrato;
                file.InvContConvModificatorio = conv;
                file.Nombre = FileUploadControl.FileName.ToString();
                file.Comentarios = txtComentario.Text;
                if (rbSi.Checked)
                {
                    EsVigente = true;
                }
                else
                {
                    EsVigente = false;
                }
                file.Vigente = EsVigente;
                if (Archivo != null)
                {
                    file.RutaArchivo = Archivo;
                }
                else
                {
                    file.RutaArchivo = Archivo2;
                }
                file.Usuar = 0;
                file.UsuarioUltAct = 0;

                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        result = service.InsertarArchivos(file,
                            Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
            }
            else
            {
                result = "El tipo de documento '." + extension + "' no es permitido.";
            }
            String cstext = "alert('" + result + "'); location.href='DetalleConvenio.aspx?Folio=" + folio + "&FolioConvenio=" + folioConvenio + "&Id=" + IdConvenio + "';";
            cs.RegisterStartupScript(cstype, "", cstext, true);
            //            Response.Redirect("~/Pantallas/);
        }

        protected void gvArchivos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RutaArchivo")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string RutaArchivo = gvArchivos.DataKeys[index].Value.ToString();


                System.IO.FileInfo file = new System.IO.FileInfo(RutaArchivo);
                if (file.Exists)
                {
                    int tam_var = RutaArchivo.Length;
                    string extension = RutaArchivo.Substring((tam_var - 4), 4);

                    WebClient client = new WebClient();
                    byte[] buffer = client.DownloadData(RutaArchivo);

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/" + extension.Replace(".", ""); // download […]
                    Response.BinaryWrite(buffer);
                    Response.Flush();
                    Response.Close();
                    Response.End();
                }

            }
        }

        protected void gvArchivos1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RutaArchivo")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string RutaArchivo = gvArchivos.DataKeys[index].Value.ToString();


                System.IO.FileInfo file = new System.IO.FileInfo(RutaArchivo);
                if (file.Exists)
                {
                    int tam_var = RutaArchivo.Length;
                    string extension = RutaArchivo.Substring((tam_var - 4), 4);

                    WebClient client = new WebClient();
                    byte[] buffer = client.DownloadData(RutaArchivo);

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/" + extension.Replace(".", ""); // download […]
                    Response.BinaryWrite(buffer);
                    Response.Flush();
                    Response.Close();
                    Response.End();
                }

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

        protected void gvConvenio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string cadena = HttpContext.Current.Request.Url.AbsoluteUri;
            string[] Separado = cadena.Split('/');
            string Final = Separado[Separado.Length - 1];
            if (e.CommandName == "Cargar")
            {
                mpeCargaArchivo.Show();
            }
        }
    }
}