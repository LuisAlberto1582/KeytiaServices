using KeytiaWeb.InvContratos.App_Code.Models;
using KeytiaWeb.WSSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class ElementoContratado : System.Web.UI.Page
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
                CargarGridElementos();
                cargarDdlCategorias();
            }

            int id;
            Int32.TryParse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? null : Request.QueryString["Id"].ToString(), out id);
            if (id > 0)
            {
                EleiminarElemCont(id);
            }
        }

        protected void ImgInsertaElementoContratado_Click(object sender, EventArgs e)
        {
            InsertarElementoContratado();
        }

        protected void gvElementos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandName = e.CommandName.ToString().ToLower();
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int RowIndex = gvr.RowIndex;

            WSSS.ElemContratado objElemContratado = new WSSS.ElemContratado();
            objElemContratado.IdElemento = Convert.ToInt32(gvElementos.DataKeys[RowIndex].Values["IdElemento"].ToString());
            objElemContratado.CategoriaElementoId = Convert.ToInt32(gvElementos.DataKeys[RowIndex].Values["CategoriaElementoId"].ToString());
            objElemContratado.Nombre = gvElementos.DataKeys[RowIndex].Values["Nombre"].ToString();
            objElemContratado.Descripcion = gvElementos.DataKeys[RowIndex].Values["Descripcion"].ToString();
            objElemContratado.ClaveCargo = 0;
            objElemContratado.SActivo = gvElementos.DataKeys[RowIndex].Values["sActivo"].ToString();
            objElemContratado.Activo = objElemContratado.SActivo.ToLower() == "activo" ? true : false;

            string mensaje = "";

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "";
            if (commandName == "editar")
            {
                MostrarModal(objElemContratado);
            }
            else if (commandName == "eliminar")
            {
                cstext = "if(confirm('¿Está seguro que desea eliminar el registro?')){ location.assign('ElementoContratado.aspx?Id=" + objElemContratado.IdElemento.ToString() + "'); }";
            }

            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        #endregion

        #region metodos
        public void CargarGridElementos()
        {
            gvElementos.DataSource = BuscarElementosContratados();
            gvElementos.DataBind();
        }

        public List<App_Code.Models.ElemContratado> BuscaElementosContratados()
        {
            List<App_Code.Models.ElemContratado> listaElemCont = new List<App_Code.Models.ElemContratado>();

            return listaElemCont;
        }

        public bool InsertarElementoContratado()
        {
            bool respuesta = false;

            WSSS.ElemContratado objElemContratado = new WSSS.ElemContratado();

            objElemContratado.Nombre = txtNombreElemento.Text.ToUpper();
            objElemContratado.Descripcion = txtDescripcionElemento.Text.ToUpper();
            objElemContratado.ClaveCargo = Convert.ToInt32(DpdClaveCargo.SelectedValue.ToString());
            objElemContratado.IActivo = Convert.ToInt32(DpdActivo.SelectedValue.ToString());
            objElemContratado.CategoriaElementoId = Convert.ToInt32(DpdCategoria.SelectedValue.ToString());
            objElemContratado.DtIniVigencia = "2018/01/01 00:00:00";//txtxFechaInicio.Text.ToUpper();
            objElemContratado.DtFinVigencia = "2079/01/01 00:00:00";//txtFechaFin.Text.ToUpper();

            if (true)
            {

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                string cstext = "";
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    string resp = service.InsertarElementoContratado(objElemContratado, Util.Encrypt(DSODataContext.ConnectionString));

                    cstext = "alert('" + resp + "');  location.href = 'ElementoContratado.aspx';";

                    cs.RegisterStartupScript(cstype, "", cstext, true);
                }
            }


            return respuesta;
        }

        public bool ValidacionElemContratado(WSSS.ElemContratado objelemContratado)
        {
            bool respuesta = false;

            if (objelemContratado.IActivo == 1 || objelemContratado.IActivo == 2)
            {
                objelemContratado.Activo = objelemContratado.IActivo == 1 ? true : false;

                respuesta = true;
            }
            else
            {
                respuesta = false;
            }


            string Dateformat = @"yyyy/mm/dd";
            string datePattern = @"^[0-9]{4}\/[0-9]{2}\/[0-9]{2}$";
            DateTime dtInicio = new DateTime();
            DateTime dtFin = new DateTime();

            if (
                Regex.Match(objelemContratado.DtIniVigencia, datePattern).Success &&
                Regex.Match(objelemContratado.DtFinVigencia, datePattern).Success
                )
            {

                DateTime.TryParse(objelemContratado.DtIniVigencia, out dtInicio);
                DateTime.TryParse(objelemContratado.DtFinVigencia, out dtFin);

                if
                    (
                        dtInicio > new DateTime() &&
                        dtFin > new DateTime() &&
                        dtInicio < dtFin
                    )
                {
                    respuesta = true;
                }
                else
                {
                    respuesta = false;
                }
            }
            else
            {
                respuesta = false;
            }
            return respuesta;
        }

        public List<WSSS.ElemContratado> BuscarElementosContratados()
        {
            WebServiceSoapClient service = new WebServiceSoapClient();

            List<WSSS.ElemContratado> listaElemContratados = new List<WSSS.ElemContratado>();
            string JSON = service.BuscaElementosContratados(Util.Encrypt(DSODataContext.ConnectionString));
            listaElemContratados = (new JavaScriptSerializer()).Deserialize<List<WSSS.ElemContratado>>(JSON);

            return listaElemContratados;
        }



        #endregion

        public bool CargarModal(WSSS.ElemContratado objElemContratado)
        {
            bool res = false;
            try
            {
                txtIdElementoModal.Text = objElemContratado.IdElemento.ToString();
                txtNombreModal.Text = objElemContratado.Nombre.ToString();
                txtDescripcionModal.Text = objElemContratado.Descripcion.ToString();
                DpdCategoriaModal.SelectedValue = objElemContratado.CategoriaElementoId.ToString();
                txtClaveCargoModal.Text = objElemContratado.ClaveCargo.ToString();
                dpdEstatusModal.SelectedValue = (objElemContratado.Activo ? "1" : "2");

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }

        public void MostrarModal(WSSS.ElemContratado objElemContratado)
        {
            try
            {
                if (CargarModal(objElemContratado))
                {
                    mpeInsertaSociedadModal.Show();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void cargarDdlCategorias()
        {
            List<WSSS.InvContCategoriaElemento> listaCategorias = new List<InvContCategoriaElemento>();
            listaCategorias = BuscarCategorias();

            DpdCategoria.DataSource = listaCategorias;
            DpdCategoria.DataBind();


            DpdCategoriaModal.DataSource = listaCategorias;
            DpdCategoriaModal.DataBind();

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
            try
            {
                EditarElementoContratado();
            }
            catch (Exception ex)
            {
            }
        }

        public void EditarElementoContratado()
        {
            string res = string.Empty;
            try
            {
                WSSS.ElemContratado objElemContratado = new WSSS.ElemContratado();
                objElemContratado.IdElemento = Convert.ToInt32(txtIdElementoModal.Text.ToString());
                objElemContratado.Nombre = txtNombreModal.Text.ToString();
                objElemContratado.Descripcion = txtDescripcionModal.Text.ToString();
                objElemContratado.SActivo = dpdEstatusModal.SelectedItem.Text.ToString();
                objElemContratado.Activo = dpdEstatusModal.SelectedValue.ToString() == "1" ? true : false;
                objElemContratado.CategoriaElementoId = Convert.ToInt32(DpdCategoriaModal.SelectedValue.ToString());

                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        res = service.EditarElemCont(objElemContratado, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                }

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "');  location.href = 'ElementoContratado.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);

            }
            catch (Exception ex)
            {
            }
        }

        public void EleiminarElemCont(int id)
        {
            try
            {
                string res = string.Empty;
                WSSS.ElemContratado objElemCont = new WSSS.ElemContratado();
                objElemCont.IdElemento = id;
                WebServiceSoapClient service = new WebServiceSoapClient();
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        res = service.EliminarElemCont(objElemCont, Util.Encrypt(DSODataContext.ConnectionString));
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                }

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "');  location.href = 'ElementoContratado.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            catch (Exception ex)
            {
            }
        }
    }
}