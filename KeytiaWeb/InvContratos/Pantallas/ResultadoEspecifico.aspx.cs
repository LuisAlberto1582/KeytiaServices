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
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using KeytiaServiceBL;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class ResultadoEspecifico : System.Web.UI.Page
    {
        int msg;

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

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            //Se define variable metodo para diferenciar el método de la búsqueda específica con la búsqueda general.
            string metodo = String.IsNullOrEmpty(Request.QueryString["Metodo"]) ? "" : Request.QueryString["Metodo"].ToString();
            if (metodo == "BusquedaEspecifica")
            {
                //Recibe los parámetros cuando la búsqueda es específica.
                string folio = String.IsNullOrEmpty(Request.QueryString["folio"]) ? null : Request.QueryString["folio"].ToString();
                string clave = String.IsNullOrEmpty(Request.QueryString["clave"]) ? null : Request.QueryString["clave"].ToString();
                string cuentaContable = String.IsNullOrEmpty(Request.QueryString["cuentaContable"]) ? null : Request.QueryString["cuentaContable"].ToString();
                int proveedor = int.Parse(String.IsNullOrEmpty(Request.QueryString["proveedor"]) ? "0" : Request.QueryString["proveedor"].ToString());
                int convenioTipo = int.Parse(String.IsNullOrEmpty(Request.QueryString["convenioTipo"]) ? "0" : Request.QueryString["convenioTipo"].ToString());
                int tipoServicio = int.Parse(String.IsNullOrEmpty(Request.QueryString["tipoServicio"]) ? "0" : Request.QueryString["tipoServicio"].ToString());
                int estatusConvenio = int.Parse(String.IsNullOrEmpty(Request.QueryString["estatusConvenio"]) ? null : Request.QueryString["estatusConvenio"].ToString());
                string descripcion = String.IsNullOrEmpty(Request.QueryString["descripcion"]) ? null : Request.QueryString["descripcion"].ToString();
                string nombreContacto = String.IsNullOrEmpty(Request.QueryString["nombreContacto"]) ? null : Request.QueryString["nombreContacto"].ToString();
                string mailContacto = String.IsNullOrEmpty(Request.QueryString["mailContacto"]) ? null : Request.QueryString["mailContacto"].ToString();
                string vigencia = String.IsNullOrEmpty(Request.QueryString["vigencia"]) ? null : Request.QueryString["vigencia"].ToString();
                string nombreArchivo = String.IsNullOrEmpty(Request.QueryString["nombreArchivo"]) ? null : Request.QueryString["nombreArchivo"].ToString();
                string compradorNombre = String.IsNullOrEmpty(Request.QueryString["compradorNombre"]) ? null : Request.QueryString["compradorNombre"].ToString();
                string nombreSolicitante = String.IsNullOrEmpty(Request.QueryString["nombreSolicitante"]) ? null : Request.QueryString["nombreSolicitante"].ToString();
                string fechaInicio = String.IsNullOrEmpty(Request.QueryString["fechaInicio"]) ? "1900/01/01" : Request.QueryString["fechaInicio"].ToString();
                string fechaFin = String.IsNullOrEmpty(Request.QueryString["fechaFin"]) ? "1900/01/01" : Request.QueryString["fechaFin"].ToString();
                int area = int.Parse(String.IsNullOrEmpty(Request.QueryString["area"]) ? "0" : Request.QueryString["area"].ToString());
                int region = int.Parse(String.IsNullOrEmpty(Request.QueryString["region"]) ? "0" : Request.QueryString["region"].ToString());
                int elemento = int.Parse(String.IsNullOrEmpty(Request.QueryString["elemento"]) ? "0" : Request.QueryString["elemento"].ToString());
                string mailComprador = String.IsNullOrEmpty(Request.QueryString["mailComprador"]) ? null : Request.QueryString["mailComprador"].ToString();
                CargaGridContrato(folio, clave, cuentaContable, proveedor, convenioTipo, tipoServicio, estatusConvenio, descripcion, nombreContacto, mailContacto, vigencia, nombreArchivo, compradorNombre, nombreSolicitante, fechaInicio, fechaFin, area, region, elemento, mailComprador);
                CargaGridAnexo(folio, clave, cuentaContable, proveedor, convenioTipo, tipoServicio, estatusConvenio, descripcion, nombreContacto, mailContacto, vigencia, nombreArchivo, compradorNombre, nombreSolicitante, fechaInicio, fechaFin, area, region, elemento, mailComprador);
                CargaGridConvenio(folio, cuentaContable, proveedor, convenioTipo, tipoServicio, estatusConvenio, descripcion, nombreContacto, mailContacto, vigencia, nombreArchivo, compradorNombre, nombreSolicitante, fechaInicio, fechaFin, area, region, elemento, clave, mailComprador);

                if (msg == 3)
                {
                    String cstext = "if(confirm('No hay Información con ese parametro de búsqueda.')){location.href = 'BusquedaEspecifica.aspx';}";
                    cs.RegisterStartupScript(cstype, "", cstext, true);
                }
            }
            else
            {
                //Recibe el parámetro cuando la búsqueda es general.
                string parametro = String.IsNullOrEmpty(Request.QueryString["parametro"]) ? "" : Request.QueryString["parametro"].ToString();
                Session["Parametro"] = parametro;
                txtContrato.Text = parametro;
                CargarGrid(parametro);
            }

            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        /// <summary>
        /// Carga el grid del contrato con la búsqueda específica.
        /// </summary>
        /// <param name="folio"></param>
        /// <param name="clave"></param>
        /// <param name="cuentaContable"></param>
        /// <param name="proveedor"></param>
        /// <param name="convenioTipo"></param>
        /// <param name="tipoServicio"></param>
        /// <param name="estatusConvenio"></param>
        /// <param name="descripcion"></param>
        /// <param name="nombreContacto"></param>
        /// <param name="mailContacto"></param>
        /// <param name="vigencia"></param>
        /// <param name="nombreArchivo"></param>
        /// <param name="compradorNombre"></param>
        /// <param name="nombreSolicitante"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="area"></param>
        /// <param name="region"></param>
        /// <param name="elemento"></param>
        public void CargaGridContrato(string folio, string clave, string cuentaContable, int proveedor, int convenioTipo, int tipoServicio, int estatusConvenio, string descripcion, string nombreContacto, string mailContacto, string vigencia, string nombreArchivo, string compradorNombre, string nombreSolicitante, string fechaInicio, string fechaFin, int area, int region, int elemento, string mailComprador)
        {

            DateTime fechaInicio2 = DateTime.Parse(fechaInicio);
            DateTime fechaFin2 = DateTime.Parse(fechaFin);
            WebServiceSoapClient Servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<ResultadoContrato> List = new List<ResultadoContrato>();
                string JSON =
                    Servicio.DevuelveResultadoContratoJSON(folio, clave, cuentaContable, proveedor,
                    convenioTipo, estatusConvenio, descripcion, nombreContacto, mailContacto, vigencia,
                    nombreArchivo, compradorNombre, nombreSolicitante, area, region, elemento, fechaInicio2,
                    fechaFin2, tipoServicio, mailComprador, Util.Encrypt(DSODataContext.ConnectionString),
                    DSODataContext.Schema);
                List = (new JavaScriptSerializer()).Deserialize<List<ResultadoContrato>>(JSON);

                gvContrato.DataSource = List.ToList();
                gvContrato.DataBind();

                if (JSON == "[]")
                {
                    msg += 1;
                }
            }

        }

        /// <summary>
        /// Carga el grid del anexo con la búsqueda específica.
        /// </summary>
        /// <param name="folio"></param>
        /// <param name="clave"></param>
        /// <param name="cuentaContable"></param>
        /// <param name="proveedor"></param>
        /// <param name="convenioTipo"></param>
        /// <param name="tipoServicio"></param>
        /// <param name="estatusConvenio"></param>
        /// <param name="descripcion"></param>
        /// <param name="nombreContacto"></param>
        /// <param name="mailContacto"></param>
        /// <param name="vigencia"></param>
        /// <param name="nombreArchivo"></param>
        /// <param name="compradorNombre"></param>
        /// <param name="nombreSolicitante"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="area"></param>
        /// <param name="region"></param>
        /// <param name="elemento"></param>
        public void CargaGridAnexo(
            string folio, string clave, string cuentaContable, int proveedor, int convenioTipo,
            int tipoServicio, int estatusConvenio, string descripcion, string nombreContacto,
            string mailContacto, string vigencia, string nombreArchivo, string compradorNombre,
            string nombreSolicitante, string fechaInicio, string fechaFin, int area, int region,
            int elemento, string mailComprador)
        {
            DateTime fechaInicio2 = DateTime.Parse(fechaInicio);
            DateTime fechaFin2 = DateTime.Parse(fechaFin); WebServiceSoapClient Servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<ResultadoAnexoConvenio> List = new List<ResultadoAnexoConvenio>();
                string JSON = Servicio.DevuelveResultadoAnexoJSON(folio, clave, cuentaContable,
                    proveedor, convenioTipo, estatusConvenio, descripcion, nombreContacto,
                    mailContacto, vigencia, nombreArchivo, compradorNombre, nombreSolicitante,
                    area, region, elemento, fechaInicio2, fechaFin2, tipoServicio, mailComprador,
                    Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);
                List = (new JavaScriptSerializer()).Deserialize<List<ResultadoAnexoConvenio>>(JSON);
                gvAnexo.DataSource = List.ToList();
                gvAnexo.DataBind();

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;

                if (JSON == "[]")
                {
                    msg += 1;
                }
            }

        }

        /// <summary>
        /// Carga el grid del convenio con la búsqueda específica.
        /// </summary>
        /// <param name="folio"></param>
        /// <param name="cuentaContable"></param>
        /// <param name="proveedor"></param>
        /// <param name="convenioTipo"></param>
        /// <param name="tipoServicio"></param>
        /// <param name="estatusConvenio"></param>
        /// <param name="descripcion"></param>
        /// <param name="nombreContacto"></param>
        /// <param name="mailContacto"></param>
        /// <param name="vigencia"></param>
        /// <param name="nombreArchivo"></param>
        /// <param name="compradorNombre"></param>
        /// <param name="nombreSolicitante"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="area"></param>
        /// <param name="region"></param>
        /// <param name="elemento"></param>
        public void CargaGridConvenio(
            string folio, string cuentaContable, int proveedor, int convenioTipo,
            int tipoServicio, int estatusConvenio, string descripcion, string nombreContacto,
            string mailContacto, string vigencia, string nombreArchivo, string compradorNombre,
            string nombreSolicitante, string fechaInicio, string fechaFin, int area, int region,
            int elemento, string clave, string mailComprador)
        {
            DateTime fechaInicio2 = DateTime.Parse(fechaInicio);
            DateTime fechaFin2 = DateTime.Parse(fechaFin);
            WebServiceSoapClient Servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<ResultadoAnexoConvenio> List = new List<ResultadoAnexoConvenio>();
                string JSON =
                    Servicio.DevuelveResultadoConvenioJSON(folio, cuentaContable, proveedor,
                    convenioTipo, estatusConvenio, descripcion, nombreContacto, mailContacto,
                    vigencia, nombreArchivo, compradorNombre, nombreSolicitante, area, region,
                    elemento, fechaInicio2, fechaFin2, tipoServicio, clave, mailComprador,
                    Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);

                List = (new JavaScriptSerializer()).Deserialize<List<ResultadoAnexoConvenio>>(JSON);
                gvConvenio.DataSource = List.ToList();
                gvConvenio.DataBind();

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;

                if (JSON == "[]")
                {
                    msg += 1;
                }
            }

        }

        /// <summary>
        /// Carga los tres grid (Contrato, Anexo, Convenio) con la búsqueda general (1 parámetro).
        /// </summary>
        /// <param name="parametro"></param>
        public void CargarGrid(string parametro)
        {
            WebServiceSoapClient Servicio = new WebServiceSoapClient();
            using (WebClient client = new WebClient())
            {
                List<ResultadoContrato> listaContrato = new List<ResultadoContrato>();
                string JSON =
                    Servicio.DevuelveContratoParametroJSON(parametro,
                            Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);

                listaContrato = (new JavaScriptSerializer()).Deserialize<List<ResultadoContrato>>(JSON);
                gvContrato.DataSource = listaContrato.ToList();
                gvContrato.DataBind();

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;

                string msg = "";

                List<ResultadoAnexoConvenio> listaAnexo = new List<ResultadoAnexoConvenio>();
                string jsonAnexo =
                    Servicio.DevuelveAnexoParametroJSON(parametro,
                            Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);
                listaAnexo = (new JavaScriptSerializer()).Deserialize<List<ResultadoAnexoConvenio>>(jsonAnexo);
                gvAnexo.DataSource = listaAnexo.ToList();
                gvAnexo.DataBind();


                List<ResultadoAnexoConvenio> listaConvenio = new List<ResultadoAnexoConvenio>();
                string jsonConvenio =
                    Servicio.DevuelveConvenioParametroJSON(parametro,
                        Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);
                listaConvenio = (new JavaScriptSerializer()).Deserialize<List<ResultadoAnexoConvenio>>(jsonConvenio);
                gvConvenio.DataSource = listaConvenio.ToList();
                gvConvenio.DataBind();



                if (JSON == "[]" && jsonAnexo == "[]" && jsonConvenio == "[]")
                {
                    String cstext = "if(confirm('No hay Información con ese parametro de búsqueda.')){location.href = 'BusquedaEspecifica.aspx';}";
                    cs.RegisterStartupScript(cstype, msg, cstext, true);

                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect",
                    //"alert('No hay Información con ese parametro de búsqueda.'); window.location='" +
                    //Request.ApplicationPath + "~/Pantallas/BusquedaEspecifica.aspx';", true);

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

        protected void gv_Command(object sender, CommandEventArgs e)
        {
            string Folio = "";
            string Activo = "";
            string Contrato = "";
            string Id = "";
            string ContratoId = "";

            GridView grid = null;
            if (e.CommandName == "ToDetalleContrato")
            {
                grid = gvContrato;
            }
            else if (e.CommandName == "ToDetalleAnexo")
            {
                grid = gvAnexo;
            }
            else if (e.CommandName == "ToDetalleConvenio")
            {
                grid = gvConvenio;
            }


            foreach (GridViewRow row in grid.Rows)
            {

                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    if (grid.HeaderRow.Cells[i].Text.ToString().ToLower() == "folio")
                    {
                        LinkButton lbtn = (LinkButton)row.Cells[i].FindControl("lbtngvContrato");
                        if (lbtn != null)
                        {
                            Folio = lbtn.Text;
                        }

                    }

                    if (grid.HeaderRow.Cells[i].Text.ToString().ToLower() == "activo")
                    {
                        Activo = row.Cells[i].Text.ToString();
                    }

                    if (grid.HeaderRow.Cells[i].Text.ToString().ToLower() == "contrato")
                    {
                        Contrato = row.Cells[i].Text.ToString();
                    }

                    if (grid.HeaderRow.Cells[i].Text.ToString().ToLower() == "id")
                    {
                        Id = row.Cells[i].Text.ToString();
                    }

                    if (grid.HeaderRow.Cells[i].ToString().ToLower() == "contratoid")
                    {
                        ContratoId = row.Cells[i].ToString();
                    }
                }
            }
        }

        protected void lbtngvContrato_Click(object sender, EventArgs e)
        {

            LinkButton lkBtn = sender as LinkButton;

            if (lkBtn != null)
            {
                GridViewRow gridRow = lkBtn.NamingContainer as GridViewRow;
                string Folio = gvContrato.DataKeys[gridRow.RowIndex].Values[0].ToString();
                string Activo = HttpUtility.UrlPathEncode(gvContrato.DataKeys[gridRow.RowIndex].Values[1].ToString().Replace(' ', '_'));
                string Id = gvContrato.DataKeys[gridRow.RowIndex].Values[2].ToString();

                Response.Redirect
                    (
                    string.Format
                        (
                            "~/invContratos/Pantallas/DetallePrevio.aspx?Folio={0}&Estatus={1}&IdContrato={2}&IdAnexo=0&IdConv=0&Procedimiento=Contrato"
                            , Folio, Activo, Id
                        )
                    );
            }
        }

        protected void gridAnexolkBtn_Click(object sender, EventArgs e)
        {
            LinkButton lkBtn = sender as LinkButton;

            if (lkBtn != null)
            {
                GridViewRow gridRow = lkBtn.NamingContainer as GridViewRow;
                string Folio = gvAnexo.DataKeys[gridRow.RowIndex].Values[0].ToString();
                string Activo = HttpUtility.UrlPathEncode(gvAnexo.DataKeys[gridRow.RowIndex].Values[1].ToString().Replace(' ', '_'));
                string Contrato = gvAnexo.DataKeys[gridRow.RowIndex].Values[2].ToString();
                string Id = gvAnexo.DataKeys[gridRow.RowIndex].Values[3].ToString();
                string ContratoId = gvAnexo.DataKeys[gridRow.RowIndex].Values[4].ToString();

                Response.Redirect
                    (
                    string.Format
                        (
                            "~/invContratos/Pantallas/DetallePrevio.aspx?Folio={0}&Estatus={1}&folioContrato={2}&IdAnexo={3}&IdContrato={4}&IdConv=0&Procedimiento=Anexo"
                            , Folio, Activo, Contrato, Id, ContratoId
                        )
                    );
            }
        }

        protected void gridConveniolkBtn_Click(object sender, EventArgs e)
        {
            LinkButton lkBtn = sender as LinkButton;

            if (lkBtn != null)
            {
                GridViewRow gridRow = lkBtn.NamingContainer as GridViewRow;
                string Folio = gvConvenio.DataKeys[gridRow.RowIndex].Values[0].ToString();
                string Activo = HttpUtility.UrlPathEncode(gvConvenio.DataKeys[gridRow.RowIndex].Values[1].ToString().Replace(' ', '_'));
                string Contrato = gvConvenio.DataKeys[gridRow.RowIndex].Values[2].ToString();
                string Id = gvConvenio.DataKeys[gridRow.RowIndex].Values[3].ToString();
                string ContratoId = gvConvenio.DataKeys[gridRow.RowIndex].Values[4].ToString();

                Response.Redirect
                    (
                    string.Format
                        (
                            "~/invContratos/Pantallas/DetallePrevio.aspx?Folio={0}&Estatus={1}&folioContrato={2}&IdConv={3}&IdContrato={4}&IdAnexo=0&Procedimiento=ConvModificatorio"
                            , Folio, Activo, Contrato, Id, ContratoId
                        )
                    );
            }
        }
    }
}