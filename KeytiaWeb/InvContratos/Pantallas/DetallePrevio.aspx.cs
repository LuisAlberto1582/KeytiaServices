using KeytiaWeb.InvContratos.App_Code.Models;
using KeytiaWeb.WSSS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using KeytiaServiceBL;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class DetallePrevio : System.Web.UI.Page
    {
        int i = 0;
        List<KeytiaWeb.InvContratos.App_Code.Models.DetallePrevio> previo = new List<KeytiaWeb.InvContratos.App_Code.Models.DetallePrevio>();
        List<EncabezadoPrevio> encabezado = new List<EncabezadoPrevio>();
        List<DocumentosRenovacionP> documentos = new List<DocumentosRenovacionP>();

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
            if (
                    !list.Exists(element => element == lsURL) &&    // Busca que el elemanto actual no esta ya en la lista
                    list.Last() != lsURL &&  // Busca que el elemanto actual sea distinto del ultimo elemanto
                    !(String.Equals(list.Last(), lsURL, StringComparison.InvariantCultureIgnoreCase)) // Busca que el elemento actual sea distinto del ultimo elemento de la lista
                )
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


            //Se toman los ID de cada uno para la carga de archivos renovación.
            int idContrato = Int32.Parse(String.IsNullOrEmpty(Request.QueryString["IdContrato"]) ? "" : Request.QueryString["IdContrato"].ToString());
            Session["idContrato"] = idContrato;
            int idAnexo = Int32.Parse(String.IsNullOrEmpty(Request.QueryString["IdAnexo"]) ? "" : Request.QueryString["IdAnexo"].ToString());
            Session["idAnexo"] = idAnexo;
            int idConv = Int32.Parse(String.IsNullOrEmpty(Request.QueryString["IdConv"]) ? "" : Request.QueryString["IdConv"].ToString());
            Session["idConv"] = idConv;
            string procedimiento = String.IsNullOrEmpty(Request.QueryString["Procedimiento"]) ? "" : Request.QueryString["Procedimiento"].ToString();
            Session["Procedimiento"] = procedimiento;
            //Parametro que recibe al seleccionar el folio 
            string folio = String.IsNullOrEmpty(Request.QueryString["Folio"]) ? "" : Request.QueryString["Folio"].ToString();
            Session["folio"] = folio;

            string estatus = String.IsNullOrEmpty(Request.QueryString["Estatus"]) ? "" : Request.QueryString["Estatus"].ToString();
            estatus = estatus.Replace('_', ' ');
            Session["Estatus"] = estatus;
            string contrato = String.IsNullOrEmpty(Request.QueryString["folioContrato"]) ? "" : Request.QueryString["folioContrato"].ToString();
            Session["folioContrato"] = contrato;

            string cadena = HttpContext.Current.Request.Url.AbsoluteUri;
            string[] Separado = cadena.Split('/');
            string Final = Separado[Separado.Length - 1];
            url.Text = Server.UrlEncode(Final);


            if (contrato == "")
            {
                txtFolio.Text = folio;
                txtContrato.Text = folio;
                Session["folioContrato"] = folio;
            }
            else
            {
                txtFolio.Text = contrato;
                txtContrato.Text = contrato;
            }

            txtEstatus.Text = estatus;
            if (!Page.IsPostBack)
            {
                ContratoData(folio);
                AnexoData(folio);
                ConvenioData(folio);
                RenovacionData();
                DpdFasesRenovacion_SelectedIndexChanged();
                CargarCheckBox();
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        /// <summary>
        /// Carga el grid de contratos.
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
            i = 0;

            //Asigna los valores a los textbox que corresponden al detalle previo.
            foreach (GridViewRow row in gvPrevioDetalle.Rows)
            {
                string JSON =
                    service.DevuelvePrevioContratoJSON(encabezado[i].Folio, Util.Encrypt(DSODataContext.ConnectionString));
                if (JSON != "[]")
                {
                    previo = (new JavaScriptSerializer()).Deserialize<List<KeytiaWeb.InvContratos.App_Code.Models.DetallePrevio>>(JSON);
                    TextBox txtFolio = (TextBox)row.FindControl("txtFolio");
                    txtFolio.Text = previo[0].Folio;
                    Session["FolioContrato"] = txtFolio.Text;
                    TextBox txtClave = (TextBox)row.FindControl("txtClave");
                    txtClave.Text = previo[0].Clave;
                    TextBox txtFolioRelacionado = (TextBox)row.FindControl("txtFolioRelacionado");
                    txtFolioRelacionado.Text = previo[i].RelacionFolio;
                    TextBox txtProveedor = (TextBox)row.FindControl("txtProveedor");
                    txtProveedor.Text = previo[0].Proveedor;
                    TextBox txtVigencia = (TextBox)row.FindControl("txtVigencia");
                    txtVigencia.Text = previo[i].Vigente;
                    if (txtVigencia.Text == "True")
                    {
                        txtVigencia.Text = "S";
                    }
                    else
                    {
                        txtVigencia.Text = "N";
                    }

                    TextBox txtTipoContrato = (TextBox)row.FindControl("txtTipoContrato");
                    txtTipoContrato.Text = previo[0].CategoriaConvenio;
                    TextBox txtTipoServicio = (TextBox)row.FindControl("txtTipoServicio");
                    txtTipoServicio.Text = previo[0].CategoriaServicio;
                    TextBox txtSolicitante = (TextBox)row.FindControl("txtSolicitante");
                    txtSolicitante.Text = previo[0].SolicitanteNombre;
                    TextBox txtComprador = (TextBox)row.FindControl("txtComprador");
                    txtComprador.Text = previo[i].CompradorNombre;
                    DateTime Fechasolicitud = DateTime.Parse(previo[0].FechaSolicitud);
                    TextBox txtFechaSolicitud = (TextBox)row.FindControl("txtFechaSolicitud");
                    txtFechaSolicitud.Text = Fechasolicitud.ToString("yy/MM/dd");
                    TextBox txtInicio = (TextBox)row.FindControl("txtInicio");
                    txtInicio.Text = previo[0].FechaInicioVigencia;
                    TextBox txtFin = (TextBox)row.FindControl("txtFin");
                    txtFin.Text = previo[0].FechaFinVigencia;
                    TextBox txtDescripcion = (TextBox)row.FindControl("txtDescripcion");
                    txtDescripcion.Text = previo[0].Descripcion;

                }


                i += 1;
            }
        }

        /// <summary>
        /// Carga el grid de anexos.
        /// </summary>
        /// <param name="folio"></param>
        public void AnexoData(string folio)
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonEncabezado = service.DevuelveEncabezadoAnexoJSON(folio, Util.Encrypt(DSODataContext.ConnectionString));
                encabezado = (new JavaScriptSerializer()).Deserialize<List<EncabezadoPrevio>>(jsonEncabezado);
                gvAnexo.DataSource = encabezado;
                gvAnexo.DataBind();

            }

            i = 0;
            //Asigna los valores a los textbox que corresponden al detalle previo.
            foreach (GridViewRow row in gvAnexo.Rows)
            {
                string JSON =
                    service.DevuelvePrevioAnexoJSON(encabezado[i].Folio, Util.Encrypt(DSODataContext.ConnectionString));
                if (JSON != "[]")
                {
                    previo = (new JavaScriptSerializer()).Deserialize<List<KeytiaWeb.InvContratos.App_Code.Models.DetallePrevio>>(JSON);
                    TextBox txtFolio = (TextBox)row.FindControl("txtFolioA");
                    txtFolio.Text = previo[0].Folio;
                    Session["FolioAnexo"] = txtFolio.Text;
                    TextBox txtClave = (TextBox)row.FindControl("txtClaveA");
                    txtClave.Text = previo[0].Clave;
                    TextBox txtFolioRelacionado = (TextBox)row.FindControl("txtFolioRelacionadoA");
                    txtFolioRelacionado.Text = previo[0].RelacionFolio;
                    TextBox txtProveedor = (TextBox)row.FindControl("txtProveedorA");
                    txtProveedor.Text = previo[0].Proveedor;
                    TextBox txtVigencia = (TextBox)row.FindControl("txtVigenciaA");
                    txtVigencia.Text = previo[0].Vigente;
                    if (txtVigencia.Text == "True")
                    {
                        txtVigencia.Text = "S";
                    }
                    else
                    {
                        txtVigencia.Text = "N";
                    }
                    TextBox txtTipoContrato = (TextBox)row.FindControl("txtTipoContratoA");
                    txtTipoContrato.Text = previo[0].CategoriaConvenio;
                    TextBox txtTipoServicio = (TextBox)row.FindControl("txtTipoServicioA");
                    txtTipoServicio.Text = previo[0].CategoriaServicio;
                    TextBox txtSolicitante = (TextBox)row.FindControl("txtSolicitanteA");
                    txtSolicitante.Text = previo[0].SolicitanteNombre;
                    TextBox txtComprador = (TextBox)row.FindControl("txtCompradorA");
                    txtComprador.Text = previo[0].CompradorNombre;
                    TextBox txtInicio = (TextBox)row.FindControl("txtInicioA");
                    txtInicio.Text = previo[0].FechaInicioVigencia;
                    TextBox txtFin = (TextBox)row.FindControl("txtFinA");
                    txtFin.Text = previo[0].FechaFinVigencia;
                    TextBox txtDescripcion = (TextBox)row.FindControl("txtDescripcionA");
                    txtDescripcion.Text = previo[0].Descripcion;

                }


                i += 1;
            }
        }

        /// <summary>
        /// Carga el grid de convenios.
        /// </summary>
        /// <param name="folio"></param>
        public void ConvenioData(string folio)
        {
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                String jsonEncabezado = service.DevuelveEncabezadoConvenioJSON(folio, Util.Encrypt(DSODataContext.ConnectionString));
                encabezado = (new JavaScriptSerializer()).Deserialize<List<EncabezadoPrevio>>(jsonEncabezado);
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
                    TextBox txtFolio = (TextBox)row.FindControl("txtFolioC");
                    txtFolio.Text = previo[0].Folio;
                    Session["FolioConvenio"] = txtFolio.Text;
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
                    txtInicio.Text = previo[0].FechaInicioVigencia;
                    TextBox txtFin = (TextBox)row.FindControl("txtFinC");
                    txtFin.Text = previo[0].FechaFinVigencia;
                    TextBox txtDescripcion = (TextBox)row.FindControl("txtDescripcionC");
                    txtDescripcion.Text = previo[0].Descripcion;
                }


                i += 1;
            }
        }

        /// <summary>
        /// Carga el grid de los documentos en renovación.
        /// </summary>
        /// <param name="folio"></param>
        public void RenovacionData()
        {
            string folio = (String)Session["folioContrato"];
            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                string jsonDocumentos = service.DevuelveDocumentosRenovacionJSON(folio, Util.Encrypt(DSODataContext.ConnectionString));
                documentos = (new JavaScriptSerializer()).Deserialize<List<DocumentosRenovacionP>>(jsonDocumentos);
                gvRenovacion.DataSource = documentos;
                gvRenovacion.DataBind();

            }
        }

        /// <summary>
        /// Cambia el color al row con la fecha más reciente del grid de contratos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPrevioDetalle_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string _estado = DataBinder.Eval(e.Row.DataItem, "Activo").ToString();

                if (_estado == "True")
                {
                    e.Row.BackColor = System.Drawing.Color.MediumSeaGreen;
                }
                else
                {
                }



            }
        }

        /// <summary>
        /// Cambia el color al row con la fecha más reciente del grid de anexos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvAnexo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string _estado = DataBinder.Eval(e.Row.DataItem, "Activo").ToString();

                if (_estado == "True")
                {
                    e.Row.BackColor = System.Drawing.Color.MediumSeaGreen;
                }
                else
                {
                }



            }
        }

        /// <summary>
        /// Cambia el color al row con la fecha más reciente del grid de convenios.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvConvenio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string _estado = DataBinder.Eval(e.Row.DataItem, "Activo").ToString();

                if (_estado == "True")
                {
                    e.Row.BackColor = System.Drawing.Color.MediumSeaGreen;
                }
                else
                {
                }
            }
        }

        /// <summary>
        /// Cambia el color al row correspondiente a los documentos vigentes en el grid de documentos en renovacion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRenovacion_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string _estado = DataBinder.Eval(e.Row.DataItem, "Comentarios").ToString();

                if (_estado == "S")
                {
                    e.Row.BackColor = System.Drawing.Color.MediumSeaGreen;
                }
                else
                {
                }
            }
        }

        protected void gvAnexo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string cadena = HttpContext.Current.Request.Url.AbsoluteUri;
            string[] Separado = cadena.Split('/');
            string Final = Separado[Separado.Length - 1];
            if (e.CommandName == "Cargar")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                string Folio = gvAnexo.DataKeys[RowIndex].Values["Folio"].ToString();
                int Id = Int32.Parse(gvAnexo.DataKeys[RowIndex].Values["Id"].ToString());
                string FolioContrato = gvAnexo.DataKeys[RowIndex].Values["FolioContrato"].ToString();


                //mpeCargaArchivo.Show();
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "var left = ((screen.width)/2)-250; " +
                    "window.open('CargaArchivos.aspx?folio=" + FolioContrato + "&folioAnexo=" + Folio + "&Anexo=" + Id + "&folioConvenio=&Convenio=0&Id=0&Url=" + Server.UrlEncode(Final) + "', " +
                                    "null,  'height = 330,width = 700,status = yes,toolbar = no,menubar = no,location = no,left='+left);";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            if (e.CommandName == "VerDetalle")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                string Folio = gvAnexo.DataKeys[RowIndex].Values["Folio"].ToString();
                int Id = Int32.Parse(gvAnexo.DataKeys[RowIndex].Values["Id"].ToString());
                string FolioContrato = gvAnexo.DataKeys[RowIndex].Values["FolioContrato"].ToString();
                string estatus = gvAnexo.DataKeys[RowIndex].Values["Activo"].ToString().Replace(' ', '_');

                Response.Redirect("~/InvContratos/Pantallas/DetalleAnexo.aspx?Estatus=" + estatus + "&Folio=" + FolioContrato + "&folioAnexo=" + Folio + "&Id=" + Id + "&Url=" + Server.UrlEncode(Final));
            }

        }

        protected void gvPrevioDetalle_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string cadena = HttpContext.Current.Request.Url.AbsoluteUri;
            string[] Separado = cadena.Split('/');
            string Final = Separado[Separado.Length - 1];
            if (e.CommandName == "Cargar")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                string Folio = gvPrevioDetalle.DataKeys[RowIndex].Values["Folio"].ToString();
                int Id = Int32.Parse(gvPrevioDetalle.DataKeys[RowIndex].Values["Id"].ToString());
                //string FolioContrato = gvAnexo.DataKeys[RowIndex].Values["FolioContrato"].ToString();

                //mpeCargaArchivo.Show();
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "var left = ((screen.width)/2)-250; " +
                    "window.open('CargaArchivos.aspx?folio=" + Folio + "&Id=" + Id + "&folioAnexo=&Anexo=0&folioConvenio=&Convenio=0&Url=" + Server.UrlEncode(Final) + "', " +
                    "null,  'height = 330,width = 700,status = yes,toolbar = no,menubar = no,location = no,left='+left);";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            if (e.CommandName == "VerDetalle")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                string Folio = gvPrevioDetalle.DataKeys[RowIndex].Values["Folio"].ToString();
                int Id = Int32.Parse(gvPrevioDetalle.DataKeys[RowIndex].Values["Id"].ToString());
                string estatus = gvPrevioDetalle.DataKeys[RowIndex].Values["Activo"].ToString().Replace(' ', '_');

                Response.Redirect("~/invContratos/Pantallas/DetalleContrato.aspx?Estatus=" + estatus + "&Folio=" + Folio + "&Id=" + Id);
            }

        }

        protected void gvConvenio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string cadena = HttpContext.Current.Request.Url.AbsoluteUri;
            string[] Separado = cadena.Split('/');
            string Final = Separado[Separado.Length - 1];

            if (e.CommandName == "Cargar")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                string Folio = gvConvenio.DataKeys[RowIndex].Values["Folio"].ToString();
                int Id = Int32.Parse(gvConvenio.DataKeys[RowIndex].Values["Id"].ToString());
                string FolioContrato = gvConvenio.DataKeys[RowIndex].Values["FolioContrato"].ToString();

                //mpeCargaArchivo.Show();
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                //String cstext = "var left = ((screen.width)/2)-250; " +
                //    "window.open('CargaArchivos.aspx?folio=" + FolioContrato + "&folioConvenio=" + Folio + "&Convenio=" + Id + "&folioAnexo=&Anexo=0&Id=0&Url=" + Server.UrlEncode(Final) + "', " +
                //    "null,  height = 450,width = 700,status = yes,toolbar = no,menubar = no,location = no,left='+left);";
                //cs.RegisterStartupScript(cstype, "", cstext, true);
                String cstext = "var left = ((screen.width)/2)-250; " +
                "window.open('CargaArchivos.aspx?folio=" + FolioContrato + "&folioConvenio=" + Folio + "&Convenio=" + Id + "&folioAnexo=&Anexo=0&Id=0&Url=" + Server.UrlEncode(Final) + "', " +
                "null,  'height = 330,width = 700,status = yes,toolbar = no,menubar = no,location = no,left='+left);";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            if (e.CommandName == "VerDetalle")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                string Folio = gvConvenio.DataKeys[RowIndex].Values["Folio"].ToString();
                int Id = Int32.Parse(gvConvenio.DataKeys[RowIndex].Values["Id"].ToString());
                string FolioContrato = gvConvenio.DataKeys[RowIndex].Values["FolioContrato"].ToString();
                string estatus = gvConvenio.DataKeys[RowIndex].Values["Activo"].ToString().Replace(' ', '_');

                Response.Redirect("~/InvContratos/Pantallas/DetalleConvenio.aspx?Estatus=" + estatus + "&Folio=" + FolioContrato + "&FolioConvenio=" + Folio + "&Id=" + Id + "&Url=" + Server.UrlEncode(Final));
            }
        }

        protected void gvArchivos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Cargar" | e.CommandName == "Descargar")
            {
                GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int RowIndex = gvr.RowIndex;
                string Folio = gvPrevioDetalle.DataKeys[RowIndex].Value.ToString();
            }

        }

        public void DpdFasesRenovacion_SelectedIndexChanged()
        {
            WebServiceSoapClient Servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<InvContFaseRenovacionContrato> List = new List<InvContFaseRenovacionContrato>();
                string JSON = Servicio.DevuelveFasesRenovacionContratoJSON(Util.Encrypt(DSODataContext.ConnectionString));
                List = (new JavaScriptSerializer()).Deserialize<List<InvContFaseRenovacionContrato>>(JSON);
                dpdFase.DataSource = List.ToList();
                dpdFase.DataBind();
            }

        }

        public void CargarArchivos(object sender, EventArgs e)
        {

            try
            {
                int idAnexo = (Int32)Session["idAnexo"];
                int idconvenio = (Int32)Session["idConv"];
                int idContrato = (Int32)Session["idContrato"];
                string proceso = (String)Session["Procedimiento"];
                string estatus = (String)Session["Estatus"];

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String result;

                string folio = (String)Session["folioContrato"];

                String path = @"D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";
                //String path = @"C:/K6/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";
                if (Directory.Exists(path))
                {
                    Directory.CreateDirectory(path + "/Documento Renovacion Contrato " + folio + "/");
                    String path2 = "Documento Renovacion Contrato " + folio + "/" + FileUploadControl.FileName;

                    FileUploadControl.PostedFile.SaveAs(path + path2);

                    //Para tomar la ruta de archivo y guardarlo en la tabla
                    string path4 = path + path2;
                    Session["Archivo"] = path4;
                }
                else
                {

                    Directory.CreateDirectory("D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Documento Renovacion Contrato " + folio + "/");
                    String path3 = "D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Documento Renovacion Contrato " + folio + "/";

                    //Directory.CreateDirectory("C:/K6/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Documento Renovacion Contrato " + folio + "/");
                    //String path3 = "C:/K6/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Documento Renovacion Contrato " + folio + "/";

                    FileUploadControl.PostedFile.SaveAs(path3 + FileUploadControl.FileName);


                    path3 = path3 + FileUploadControl.FileName;
                    Session["Archivo2"] = path3;

                }

                string Archivo = (String)Session["Archivo"];
                string Archivo2 = (String)Session["Archivo2"];
                bool EsVigente;

                InvContAnexo anexo = new InvContAnexo();
                anexo.Id = idAnexo;
                InvContContrato contrato = new InvContContrato();
                contrato.Id = idContrato;
                InvContConvModificatorio conv = new InvContConvModificatorio();
                conv.Id = idconvenio;
                InvContFaseRenovacionContrato fase = new InvContFaseRenovacionContrato();
                fase.Id = int.Parse(dpdFase.SelectedValue);

                WSSS.InvContRenovacionContrato file = new WSSS.InvContRenovacionContrato();
                file.InvContAnexo = anexo;
                file.InvContContrato = contrato;
                file.InvContConvModificatorio = conv;
                file.InvContFaseRenovacionContrato = fase;
                file.NombreArchivo = FileUploadControl.FileName.ToString();
                file.Comentarios = txtComentario.Text;
                if (rbSi.Checked)
                {
                    EsVigente = true;
                }
                else
                {
                    EsVigente = false;
                }
                file.EsVigente = EsVigente;
                file.FrecuenciaEnDiasParaWarning = Int32.Parse(txtDias.Text);
                if (Archivo != null)
                {
                    file.RutaArchivo = Archivo;
                }
                else
                {
                    file.RutaArchivo = Archivo2;
                }
                file.UsuarioCarga = 0;
                file.UsuarioUltAct = 0;


                file.DeshabilitarNoif = chbxDeshabilitarNotificaciones.Checked ? true : false;
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        result = service.InsertarDocumentosRenovacion(file, proceso, Util.Encrypt(DSODataContext.ConnectionString));
                        result = "Archivo cargado correctamente.";
                    }
                    catch (Exception ex)
                    {
                        mpeCargaArchivo.Hide();
                        result = ex.Message;
                    }
                }

                string folioContrato = Session["folioContrato"].ToString();
                string procedimiento = Session["Procedimiento"].ToString().ToLower();
                string urlredirect = string.Empty;

                if (procedimiento == "ConvModificatorio")
                {
                    urlredirect = "DetallePrevio.aspx?Folio=" + (String)Session["folio"] + "&Estatus=" + estatus + "&folioContrato=" + folioContrato + " & IdConv=" + conv.Id + "&IdContrato=" + idContrato + "&IdAnexo=0&Procedimiento=" + proceso;
                }
                else if (procedimiento == "anexo")
                {
                    urlredirect = "DetallePrevio.aspx?Folio=" + (String)Session["folio"] + "&Estatus=" + estatus + "&folioContrato=" + folioContrato + "&IdAnexo=" + anexo.Id + "&IdContrato=" + idContrato + "&IdConv=0&Procedimiento=" + proceso;
                }
                else
                {
                    urlredirect = "DetallePrevio.aspx?Folio=" + (String)Session["folio"] + "&Estatus=" + estatus + "&IdContrato=" + idContrato + "&IdAnexo=0&IdConv=0&Procedimiento=" + proceso;
                }

                // DetallePrevio.aspx?Folio=" + (String)Session["folio"]+ "&folioContrato="+folioContrato + "&Estatus=" + estatus + "&IdContrato=" + idContrato + "&IdAnexo=" + 0 + "&IdConv=" + 0 + "& Procedimiento=" + proceso + "
                String cstext = "alert('" + result + "'); location.href= '" + urlredirect + "';";
                cs.RegisterStartupScript(cstype, "", cstext, true);


                //Response.Redirect("~/Pantallas/DetallePrevio.aspx?Folio=" + (String)Session["folio"] + "&Estatus=" + estatus + "&IdContrato=" + idContrato + "&IdAnexo=" + 0 + "&IdConv=" + 0 + "& Procedimiento=" + proceso);

                //ESTE PROCESO SE COMENTA PARA FUTURAS POSIBILILDADES SE GUARDEN ARCHIVOS POR ANEXO Y CONVENIO
                //else if (proceso == "Anexo")
                //{
                //    string folioAnexo = (String)Session["folio"];
                //    string folioContrato = (String)Session["folioContrato"];

                //    String path = @"C:/Contrato/Contrato " + folioContrato + "/Anexo " + folioAnexo + "/";
                //    if (Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path + "Documento Renovacion Anexo " + folioAnexo + "/");
                //        String path2 = "Documento Renovacion Anexo " + folioAnexo + " / " + FileUploadControl.FileName;

                //        FileUploadControl.PostedFile.SaveAs(path + path2);

                //        //Para tomar la ruta de archivo y guardarlo en la tabla
                //        string path4 = path + path2;
                //        Session["Archivo"] = path4;
                //    }
                //    else
                //    {
                //        Directory.CreateDirectory("c:/Contrato/Contrato " + folioContrato + "/Anexo " + folioAnexo + "/Documento Renovacion Anexo  " + folioAnexo + "/");
                //        String path3 = "c:/Contrato /Contrato " + folioContrato + " /Anexo " + folioAnexo + " /Documento Renovacion Anexo " + folioAnexo + "/" + FileUploadControl.FileName;

                //        FileUploadControl.PostedFile.SaveAs(path3);

                //        Session["Archivo2"] = path3;

                //    }

                //    string Archivo = (String)Session["Archivo"];
                //    string Archivo2 = (String)Session["Archivo2"];
                //    bool EsVigente;

                //    InvContAnexo anexo = new InvContAnexo();
                //    anexo.Id = idAnexo;
                //    InvContContrato contrato = new InvContContrato();
                //    contrato.Id = idContrato;
                //    InvContConvModificatorio conv = new InvContConvModificatorio();
                //    conv.Id = idConv;
                //    InvContFaseRenovacionContrato fase = new InvContFaseRenovacionContrato();
                //    fase.Id = int.Parse(dpdFase.SelectedValue);

                //    WSSS.InvContRenovacionContrato file = new WSSS.InvContRenovacionContrato();
                //    file.InvContAnexo = anexo;
                //    file.InvContContrato = contrato;
                //    file.InvContConvModificatorio = conv;
                //    file.InvContFaseRenovacionContrato = fase;
                //    file.NombreArchivo = FileUploadControl.FileName.ToString();
                //    file.Comentarios = txtComentario.Text;
                //    if (rbSi.Checked)
                //    {
                //        EsVigente = true;
                //    }
                //    else
                //    {
                //        EsVigente = false;
                //    }
                //    file.EsVigente = EsVigente;
                //    file.FrecuenciaEnDiasParaWarning = Int32.Parse(txtDias.Text);
                //    if (Archivo != null)
                //    {
                //        file.RutaArchivo = Archivo;
                //    }
                //    else
                //    {
                //        file.RutaArchivo = Archivo2;
                //    }
                //    file.UsuarioCarga = 0;
                //    file.UsuarioUltAct = 0;

                //    WebServiceSoapClient service = new WebServiceSoapClient();
                //    using (WebClient client = new WebClient())
                //    {
                //        try
                //        {
                //            result = service.InsertarDocumentosRenovacion(file, proceso);
                //        }
                //        catch (Exception ex)
                //        {
                //            result = ex.Message;
                //        }
                //    }

                //    String cstext = "alert('" + result + "');";
                //    cs.RegisterStartupScript(cstype, "", cstext, true);
                //    Response.Redirect("~/Pantallas/DetallePrevio.aspx?Folio=" + (String)Session["folio"] + "&Estatus=" + estatus + "&folioContrato=" + folioContrato + "&IdAnexo=" + idAnexo + "&IdContrato=" + idContrato + "&IdConv=" + idConv + "&Procedimiento=" + proceso);

                //}
                //else if (proceso == "ConvModificatorio")
                //{
                //    string folioConv = (String)Session["folio"];
                //    string folioContrato = (String)Session["folioContrato"];

                //    String path = @"C:/Contrato/Contrato " + folioContrato + "/Convenio " + folioConv + "/";
                //    if (Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path + "Documento Renovacion Convenio " + folioConv + "/");
                //        String path2 = "Documento Renovacion Convenio " + folioConv + "/" + FileUploadControl.FileName;

                //        FileUploadControl.PostedFile.SaveAs(path + path2);

                //        //Para tomar la ruta de archivo y guardarlo en la tabla
                //        string path4 = path + path2;
                //        Session["Archivo"] = path4;
                //    }
                //    else
                //    {
                //        Directory.CreateDirectory("c:/Contrato/Contrato " + folioContrato + "/Convenio " + folioConv + "/Documento Renovacion Convenio " + folioConv + "/");
                //        String path3 = "c:/Contrato/Contrato " + folioContrato + " /Convenio " + folioConv + " /Documento Renovacion Convenio " + folioConv + "/" + FileUploadControl.FileName;

                //        FileUploadControl.PostedFile.SaveAs(path3);

                //        Session["Archivo2"] = path3;

                //    }

                //    string Archivo = (String)Session["Archivo"];
                //    string Archivo2 = (String)Session["Archivo2"];
                //    bool EsVigente;

                //    InvContAnexo anexo = new InvContAnexo();
                //    anexo.Id = idAnexo;
                //    InvContContrato contrato = new InvContContrato();
                //    contrato.Id = idContrato;
                //    InvContConvModificatorio conv = new InvContConvModificatorio();
                //    conv.Id = idConv;
                //    InvContFaseRenovacionContrato fase = new InvContFaseRenovacionContrato();
                //    fase.Id = int.Parse(dpdFase.SelectedValue);

                //    WSSS.InvContRenovacionContrato file = new WSSS.InvContRenovacionContrato();
                //    file.InvContAnexo = anexo;
                //    file.InvContContrato = contrato;
                //    file.InvContConvModificatorio = conv;
                //    file.InvContFaseRenovacionContrato = fase;
                //    file.NombreArchivo = FileUploadControl.FileName.ToString();
                //    file.Comentarios = txtComentario.Text;
                //    if (rbSi.Checked)
                //    {
                //        EsVigente = true;
                //    }
                //    else
                //    {
                //        EsVigente = false;
                //    }
                //    file.EsVigente = EsVigente;
                //    file.FrecuenciaEnDiasParaWarning = Int32.Parse(txtDias.Text);
                //    if (Archivo != null)
                //    {
                //        file.RutaArchivo = Archivo;
                //    }
                //    else
                //    {
                //        file.RutaArchivo = Archivo2;
                //    }
                //    file.UsuarioCarga = 0;
                //    file.UsuarioUltAct = 0;

                //    WebServiceSoapClient service = new WebServiceSoapClient();
                //    using (WebClient client = new WebClient())
                //    {
                //        try
                //        {
                //            result = service.InsertarDocumentosRenovacion(file, proceso, Util.Encrypt(DSODataContext.ConnectionString));
                //        }
                //        catch (Exception ex)
                //        {
                //            result = ex.Message;
                //        }
                //    }

                //    String cstext = "alert('" + result + "');";
                //    cs.RegisterStartupScript(cstype, "", cstext, true);
                //    Response.Redirect("~/Pantallas/DetallePrevio.aspx?Folio=" + (String)Session["folio"] + "&Estatus=" + estatus + "&folioContrato=" + folioContrato + "&IdConv=" + idConv + "&IdContrato=" + idContrato + "&IdAnexo=" + idAnexo + "&Procedimiento=" + proceso);

                //}
            }
            catch (Exception ex)
            {

            }
        }

        public void CboxNotificacion_CheckedChanged(object sender, EventArgs e)
        {
            string folio = (String)Session["folioContrato"];
            bool proceso;
            if (CboxNotificacion.Checked)
            {
                proceso = true;
            }
            else
            {
                proceso = false;
            }
            String result;

            WebServiceSoapClient service = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                try
                {
                    service.EstatusWarning(folio, proceso, Util.Encrypt(DSODataContext.ConnectionString));
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
        }

        public void CargarCheckBox()
        {
            string folio = (String)Session["folioContrato"];

            WebServiceSoapClient Servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<InvContContrato> List = new List<InvContContrato>();
                string JSON = Servicio.DevuelveEstatusWarningJSON(folio, Util.Encrypt(DSODataContext.ConnectionString));
                List = (new JavaScriptSerializer()).Deserialize<List<InvContContrato>>(JSON);

                bool proceso = List[0].EnvioWarningsActivo;
                if (proceso == false)
                {
                    CboxNotificacion.Checked = true;
                }
                else
                {
                    CboxNotificacion.Checked = false;
                }
            }
        }

        protected void gvRenovacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RutaArchivo")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string RutaArchivo = gvRenovacion.DataKeys[index].Value.ToString();


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

        //Master C
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

        protected void lnkAgregar_Click(object sender, EventArgs e)
        {
            mpeCargaArchivo.Show();
        }
    }
}