using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace KeytiaWeb.UserInterface.Inventario
{
    public partial class DispositivoMovil : System.Web.UI.Page
    {

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CargarEmpleados();
                    CargarMarcas();
                    HidePanels();

                    MostrarSeccion();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dateTimeFechaIng = new DateTime();
                DateTime dateTimeFechaAsignacion = new DateTime();

                DateTime.TryParse(dtbFechaIng.TextValue.Text, out dateTimeFechaIng);
                DateTime.TryParse(dtbFechaAsignacion.TextValue.Text, out dateTimeFechaAsignacion);

                EquipoMovil objEquipoMovil = new EquipoMovil();
                objEquipoMovil.IMEI = txtIMEI.Text;
                objEquipoMovil.NoSerie = txtNoSerie.Text;
                objEquipoMovil.Modelo = txtModelo.Text;
                objEquipoMovil.Marca = Convert.ToInt32(ddlMarca.SelectedItem.Value);
                objEquipoMovil.Color = txtColor.Text;
                objEquipoMovil.FechaIngreso = dateTimeFechaIng.ToString("dd/MM/yyyy");
                objEquipoMovil.FechaAsignacion = dateTimeFechaAsignacion > new DateTime() ? dateTimeFechaAsignacion.ToString("dd/MM/yyyy") : "";
                objEquipoMovil.ICodCatEmple = Convert.ToInt32(ddlEmpleado.SelectedItem.Value);

                if (objEquipoMovil != null)
                {
                    Dictionary<bool, string> resValidacion = ValidacoinInicial(objEquipoMovil);
                    bool res = resValidacion.Keys.FirstOrDefault();
                    string msgValidacion = resValidacion.Values.FirstOrDefault();

                    if (res)
                    {
                        if (CreateEquipoMovil(objEquipoMovil))
                        {
                            //Modal insert exitoso
                            lblTituloModalMsn.Text = "Insert Exitoso";
                            lblBodyModalMsn.Text = "Registro creado con exito";

                            LimpiarCamposSeccion("SectionCrate");
                        }
                        else
                        {
                            //Modal insert no exitoso
                            lblTituloModalMsn.Text = "Insert Fallido";
                            lblBodyModalMsn.Text = "No se pudo dar de alta el quipo en base de datos";

                        }
                    }
                    else
                    {
                        string[] listaMensajes = msgValidacion.Trim('|').Split('|');

                        StringBuilder mensajeIncorrecto = new StringBuilder();
                        lblTituloModalMsn.Text = "Validacion de campos fallida";
                        mensajeIncorrecto.AppendLine("Errores en validacion: ");
                        mensajeIncorrecto.AppendLine("");
                        foreach (string mensaje in listaMensajes)
                        {
                            mensajeIncorrecto.AppendLine(mensaje);
                        }
                        lblBodyModalMsn.Text = mensajeIncorrecto.ToString();
                    }

                    mpeEquipoMsn.Show();

                }
                //cpeBusquedaEquipoImg.Collapsed = true;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        protected void btnBuscaEquipos_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtEquipos = new DataTable();

                DateTime dateTimeFechaIng = new DateTime(1900, 01, 01);
                DateTime dateTimeFechaAsignacion = new DateTime(1900, 01, 01);

                //DateTime.TryParse(dtbFechaIngresoBuscar.TextValue.Text, out dateTimeFechaIng);
                //DateTime.TryParse(dtBFechaAsignacionBuscar.TextValue.Text, out dateTimeFechaAsignacion);

                EquipoMovil objEquipoMovil = new EquipoMovil();
                objEquipoMovil.IMEI = txtIMEIBuscar.Text;
                objEquipoMovil.NoSerie = txtNoSerieBuscar.Text;
                objEquipoMovil.Modelo = txtModeloBuscar.Text;
                objEquipoMovil.Marca = Convert.ToInt32(ddlMarcaBuscar.SelectedItem.Value);
                objEquipoMovil.Color = txtColorBuscar.Text;
                objEquipoMovil.FechaIngreso = dateTimeFechaIng > new DateTime(1900, 01, 01) ? dateTimeFechaIng.ToString("yyyy-MM-dd 00:00:00") : "";
                objEquipoMovil.FechaAsignacion = dateTimeFechaAsignacion > new DateTime(1900, 01, 01) ? dateTimeFechaAsignacion.ToString("yyyy-MM-dd 00:00:00") : "";
                objEquipoMovil.ICodCatEmple = Convert.ToInt32(ddlEmpleBuscar.SelectedItem.Value);

                if (objEquipoMovil != null)
                {
                    dtEquipos = BuscarDTEquiposFiltros(objEquipoMovil);

                    if (dtEquipos != null && dtEquipos.Rows.Count > 0 && dtEquipos.Columns.Count > 0)
                    {
                        if (dtEquipos.Columns.Contains("MarcaEquipoMovilDesc"))
                        {
                            dtEquipos.Columns["MarcaEquipoMovilDesc"].ColumnName = "Marca";
                        }
                        if (dtEquipos.Columns.Contains("ModeloCel"))
                        {
                            dtEquipos.Columns["ModeloCel"].ColumnName = "Modelo";
                        }
                        if (dtEquipos.Columns.Contains("NSerie"))
                        {
                            dtEquipos.Columns["NSerie"].ColumnName = "No. Serie";
                        }
                        if (dtEquipos.Columns.Contains("FechaReg"))
                        {
                            dtEquipos.Columns["FechaReg"].ColumnName = "Fecha Registro";
                        }
                        if (dtEquipos.Columns.Contains("FechaAsignacion"))
                        {
                            dtEquipos.Columns["FechaAsignacion"].ColumnName = "Fecha Asignación";
                        }
                        if (dtEquipos.Columns.Contains("Emple"))
                        {
                            dtEquipos.Columns["Emple"].ColumnName = "Colaborador";
                        }


                        InsertarGridView(dtEquipos);

                        //cpeRegistroEquipoImg.Collapsed = true;
                    }
                    else
                    {
                        grvEquipo.DataSource = new DataTable();
                        grvEquipo.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void HideSections_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                string command = btn.CommandArgument;

                //if ((command == "SectionList"))
                //{
                //    SectionCreate.Style.Add("display", "none");
                //    SectionList.Style.Add("display", "block");
                //}

                //if ((command == "SectionCreate"))
                //{
                //    SectionCreate.Style.Add("display", "block");
                //    SectionList.Style.Add("display", "none");
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnEditarEquipoMovilRow_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);
            string accion = ibtn1.Attributes["Accion"];

            GridViewRow rowGridView = (GridViewRow)grvEquipo.Rows[rowIndex];

            int iCodCatEquipo = (int)grvEquipo.DataKeys[rowIndex].Values[0];

            if (iCodCatEquipo > 0)
            {
                //Response.Redirect(string.Format("~/UserInterface/Inventario/RetiroDispositivoMovil.aspx?iCodCatEquipo={0}&Accion={1}", iCodCatEquipo, accion));
                //Response.Redirect("~/UserInterface/Inventario/RetiroDispositivoMovil.aspx",true);
                //string url = string.Format("~/UserInterface/Inventario/RetiroDispositivoMovil.aspx?iCodCatEquipo={0}&Accion={1}", iCodCatEquipo, accion); ;
                //HttpContext.Current.Response.Redirect(url);

                CargarModalUpdateEquipoMovil(iCodCatEquipo, accion);

            }


        }

        protected void btnBorrarEquipoMovilRow_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);
            string accion = ibtn1.Attributes["Accion"];

            GridViewRow rowGridView = (GridViewRow)grvEquipo.Rows[rowIndex];

            int iCodCatEquipo = (int)grvEquipo.DataKeys[rowIndex].Values[0];

            if (iCodCatEquipo > 0)
            {
                //Response.Redirect(string.Format("~/UserInterface/Inventario/RetiroDispositivoMovil.aspx?iCodCatEquipo={0}&Accion={1}", iCodCatEquipo, accion));
                //Response.Redirect("~/UserInterface/Inventario/RetiroDispositivoMovil.aspx",true);
                //string url = string.Format("~/UserInterface/Inventario/RetiroDispositivoMovil.aspx?iCodCatEquipo={0}&Accion={1}", iCodCatEquipo, accion); ;
                //HttpContext.Current.Response.Redirect(url);

                CargarModalDeleteEquipoMovil(iCodCatEquipo, accion);

            }
        }

        protected void btnAccionUpdateModal_Click(object sender, EventArgs e)
        {
            try
            {

                EquipoMovil objEquipoMovil = new EquipoMovil();
                objEquipoMovil.IMEI = txtIMEIModal.Text;
                objEquipoMovil.Marca = Convert.ToInt32(ddlMarcaModal.SelectedItem.Value);
                objEquipoMovil.Modelo = txtModeloModal.Text;
                objEquipoMovil.NoSerie = txtNoSerieModal.Text;
                objEquipoMovil.Color = txtColorModal.Text;
                objEquipoMovil.FechaIngreso = txtFechaInicioModal.Text;
                objEquipoMovil.FechaAsignacion = txtFechaAsignacionModal.Text;
                objEquipoMovil.ICodCatEmple = Convert.ToInt32(ddlEmpleadoModal.SelectedItem.Value);
                objEquipoMovil.iCodCatHistEquipo = Convert.ToInt32(lblICodCatEquipoModal.Text);


                Dictionary<bool, string> dictRes = ValidacoinInicial(objEquipoMovil);
                if (dictRes.Keys.FirstOrDefault())
                {

                    if (UpdateEquipoMovil(objEquipoMovil))
                    {
                        //Modal insert exitoso
                        lblTituloModalMsn.Text = "Update Exitoso";
                        lblBodyModalMsn.Text = "Se modifico la informacion del equipo en la base de datos";
                    }
                    else
                    {
                        //Modal insert no exitoso
                        lblTituloModalMsn.Text = "Update Fallido";
                        lblBodyModalMsn.Text = "Sucedio un error al tratar de modificar las caracteristicas del equipo";
                    }
                }
                else
                {
                    //Mostrar Mensaje de Error con Msg de Dict
                    string msgValidacion = dictRes.Values.FirstOrDefault();
                    string[] listaMensajes = msgValidacion.Trim('|').Split('|');

                    StringBuilder mensajeIncorrecto = new StringBuilder();
                    lblTituloModalMsn.Text = "Validacion de campos fallida";
                    mensajeIncorrecto.AppendLine("Errores en validacion: ");
                    mensajeIncorrecto.AppendLine("");
                    foreach (string mensaje in listaMensajes)
                    {
                        mensajeIncorrecto.AppendLine(mensaje);
                    }
                    lblBodyModalMsn.Text = mensajeIncorrecto.ToString();

                }

                mpeEquipoMsn.Show();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Métodos

        public void MostrarSeccion()
        {
            try
            {
                string Seccion = "create";
                if (Request.QueryString["Action"] != null)
                {
                    Seccion = Request.QueryString["Action"].ToString();
                }

                if (Seccion != null)
                {
                    if (Seccion.ToLower() == "create")
                    {
                        seccionAltaEquipos.Visible = true; 
                        seccionBusquedaEuipos.Visible= false;

                    }
                    if (Seccion.ToLower() == "search")
                    {
                        seccionAltaEquipos.Visible = false;
                        seccionBusquedaEuipos.Visible= true;
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarModalUpdateEquipoMovil(int iCodCatEquipo, string accion)
        {
            try
            {
                bool resultado = false;
                EquipoMovil objEquipoMovil = BuscarEquipoMovil(iCodCatEquipo);


                lblTituloPopUpAccion.Text = "Cambio de atributos de equipo";
                txtIMEIModal.Text = objEquipoMovil.IMEI;
                txtModeloModal.Text = objEquipoMovil.Modelo;
                txtNoSerieModal.Text = objEquipoMovil.NoSerie;
                txtColorModal.Text = objEquipoMovil.Color;
                lblICodCatEquipoModal.Text = objEquipoMovil.iCodCatHistEquipo.ToString();

                DateTime dtFechaIngresoModal = new DateTime();
                DateTime dtFechaAsignacionModal = new DateTime();

                if (DateTime.TryParse(objEquipoMovil.FechaIngreso, out dtFechaIngresoModal))
                {
                    txtFechaInicioModal.Text = dtFechaIngresoModal.ToString("dd/MM/yyyy");
                }

                if (DateTime.TryParse(objEquipoMovil.FechaAsignacion, out dtFechaAsignacionModal))
                {
                    txtFechaAsignacionModal.Text = dtFechaAsignacionModal.ToString("dd/MM/yyyy");
                }


                ddlEmpleadoModal.SelectedIndex = ddlEmpleadoModal.Items.IndexOf(ddlEmpleado.Items.FindByValue(objEquipoMovil.ICodCatEmple.ToString()));
                ddlMarcaModal.SelectedIndex = ddlMarcaModal.Items.IndexOf(ddlMarca.Items.FindByValue(objEquipoMovil.Marca.ToString()));

                mpeAccionEquipo.Show();



                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CargarModalDeleteEquipoMovil(int iCodCatEquipo, string accion)
        {
            try
            {
                bool resultado = false;

                EquipoMovil objEquipoMovil = BuscarEquipoMovil(iCodCatEquipo);
                DataTable dtMotivos = new DataTable();

                dtMotivos = BuscarMotivos();

                lblTituloModalDelete.Text = "Retiro de equipo";

                ddlEmpleModalDelete.SelectedIndex = ddlEmpleadoModal.Items.IndexOf(ddlEmpleado.Items.FindByValue(objEquipoMovil.ICodCatEmple.ToString()));
                ddlEmpleModalDelete.Enabled = false;

                txtIMEIModalDelete.Text = objEquipoMovil.IMEI;
                txtIMEIModalDelete.Enabled = false;

                txtFechaRetiroModalDelete.Text = DateTime.Now.ToString("dd/MM/yy");
                txtFechaRetiroModalDelete.Enabled = false;

                ddlMotivoModalDelete.DataSource = dtMotivos;
                ddlMotivoModalDelete.DataValueField = "iCodCatalogo";
                ddlMotivoModalDelete.DataTextField = "vchDescripcion";
                ddlMotivoModalDelete.DataBind();


                mpeModalDelete.Show();
                lblICodCatEquipoModalDelete.Text = objEquipoMovil.iCodCatHistEquipo.ToString();


                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable BuscarMotivos()
        {
            try
            {
                return DSODataAccess.Execute(ConsultaBuscarMotivos());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaBuscarMotivos()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("Select ");
                query.AppendLine("    iCodCatalogo,");
                query.AppendLine("    vchCodigo,");
                query.AppendLine("    vchDescripcion");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('MotivoRetiro','Motivos de Retiro','Español')]");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia");
                query.AppendLine("And dtFinVigencia >= GETDATE()");

                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public EquipoMovil BuscarEquipoMovil(int iCodCatEquipo)
        {
            try
            {
                EquipoMovil objEquipoMovil = new EquipoMovil();
                DataTable dtEquipoMovil = BuscarDTEquiposFiltros(new EquipoMovil { iCodCatHistEquipo = iCodCatEquipo });
                DataRow row = dtEquipoMovil.Rows[0];

                DateTime dtFechaIng = new DateTime();
                DateTime dtFechaAsignacion = new DateTime();
                int iCodCatEmple = 0;

                DateTime.TryParse(row["FechaReg"].ToString(), out dtFechaIng);
                DateTime.TryParse(row["FechaAsignacion"].ToString(), out dtFechaAsignacion);
                int.TryParse(row["iCodCatEmple"].ToString(), out iCodCatEmple);


                objEquipoMovil.IMEI = row["IMEI"].ToString();
                objEquipoMovil.Marca = Convert.ToInt32(row["iCodCatMarca"].ToString());
                objEquipoMovil.Modelo = row["ModeloCel"].ToString();
                objEquipoMovil.NoSerie = row["NSerie"].ToString();
                objEquipoMovil.Color = row["Color"].ToString();
                objEquipoMovil.FechaIngreso = dtFechaIng > new DateTime() ? dtFechaIng.ToString("yyyy-MM-dd 00:00:00") : "";
                objEquipoMovil.FechaAsignacion = dtFechaAsignacion > new DateTime() ? dtFechaAsignacion.ToString("yyyy-MM-dd 00:00:00") : "";
                objEquipoMovil.ICodCatEmple = iCodCatEmple;
                objEquipoMovil.iCodCatHistEquipo = Convert.ToInt32(row["iCodCatEquipo"].ToString());

                return objEquipoMovil;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Dictionary<bool, string> ValidacoinInicial(EquipoMovil objequipoMovil)
        {
            try
            {
                bool resultado = true;
                string msg = "";

                //Validar que el IMEI se componga de solo Numero y que sean 17 Digitos
                string patternIMEI = @"^(\d{15}|\d{17})$";
                string patterFecha = @"^\d{2}[\/]\d{2}[\/]\d{4}$";

                if (!Regex.IsMatch(objequipoMovil.IMEI, patternIMEI))
                {
                    resultado = false;

                    msg += "El campo IMEI debe de contener solo digitos.|";
                }

                if (objequipoMovil.Marca <= 0)
                {
                    resultado = false;
                    msg += "El campo  de Marca es obligatorio. |";
                }

                if (objequipoMovil.NoSerie.Length == 0)
                {
                    resultado = false;
                    msg += "El campo de No Serie es obligatoio. |";
                }

                if (objequipoMovil.FechaIngreso.Length == 0)
                {
                    resultado = false;
                    msg += "Se debe seleccionar una fecha |";
                }
                else if (!(Regex.IsMatch(objequipoMovil.FechaIngreso, patterFecha)))
                {
                    resultado = false;
                    msg += "El fomato de la fecha es incorrecto |";
                }

                DateTime dtFechaIngreso = new DateTime();
                DateTime dtFechaAsignacion = new DateTime();

                if (objequipoMovil.FechaAsignacion.Length > 0 && Regex.IsMatch(objequipoMovil.FechaAsignacion, patterFecha))
                {
                    if (DateTime.TryParse(objequipoMovil.FechaIngreso, out dtFechaIngreso) && DateTime.TryParse(objequipoMovil.FechaAsignacion, out dtFechaAsignacion))
                    {
                        if (dtFechaIngreso > dtFechaAsignacion)
                        {
                            resultado = false;
                            msg += "La fecha de asignacion debe de ser mayor  o igual a la fecha de ingreso del equipo";
                        }
                    }
                }


                // Validar que la fecha de Ingeso existe y que cumpla con el formato de fecha 
                DateTime dtFechaIng = new DateTime();

                if (!(DateTime.TryParse(objequipoMovil.FechaIngreso, out dtFechaIng) && dtFechaIng > new DateTime(2011, 01, 01)))
                {
                    resultado = false;
                }


                Dictionary<bool, string> resultadoValidacion = new Dictionary<bool, string>();
                resultadoValidacion.Add(resultado, msg);

                return resultadoValidacion;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void LimpiarCamposSeccion(string Secction)
        {
            try
            {
                switch (Secction)
                {
                    case "SectionCreate":

                        txtColor.Text = "";
                        txtIMEI.Text = "";
                        txtModelo.Text = "";
                        txtNoSerie.Text = "";

                        ddlEmpleado.SelectedIndex = ddlEmpleado.Items.IndexOf(ddlEmpleado.Items.FindByValue("0"));
                        ddlMarca.SelectedIndex = ddlMarca.Items.IndexOf(ddlMarca.Items.FindByValue("0"));

                        TextBox txt1 = new TextBox();
                        txt1.Text = "";

                        dtbFechaIng.TextValue = txt1;
                        dtbFechaAsignacion.TextValue = txt1;


                        break;
                    case "SeccionList":

                        txtColorBuscar.Text = "";
                        txtIMEIBuscar.Text = "";
                        txtModeloBuscar.Text = "";
                        txtNoSerieBuscar.Text = "";

                        ddlEmpleBuscar.SelectedIndex = ddlEmpleado.Items.IndexOf(ddlEmpleado.Items.FindByValue("0"));
                        ddlMarcaBuscar.SelectedIndex = ddlMarca.Items.IndexOf(ddlMarca.Items.FindByValue("0"));

                        TextBox txt2 = new TextBox();
                        txt2.Text = "";

                        //Esto se comento porque se quitaron las busquedas por fecha 
                        //dtbFechaIngBuscar.TextValue = txt2;
                        //dtbFechaAsignacionBuscar.TextValue = txt2;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool InsertarGridView(DataTable dtEquipos)
        {
            try
            {
                bool respuesta = false;

                if (dtEquipos.Rows.Count > 0)
                {
                    if (dtEquipos.Columns.Contains("iCodCatMarca"))
                    {
                        dtEquipos.Columns.Remove("iCodCatMarca");
                    }

                    if (dtEquipos.Columns.Contains("iCodCatEmple"))
                    {
                        dtEquipos.Columns.Remove("iCodCatEmple");
                    }

                    grvEquipo.DataSource = dtEquipos;
                    grvEquipo.DataBind();

                    //string[] formatos = new string[]
                    //{
                    //    "","","","","","","","","",""
                    //};


                    //int[] columnasVisibles = new int[]
                    //{
                    //    0,1,2,3,4,5,6,7,8
                    //};

                    //int[] columnasNoVisibles = new int[] {9};

                    //pnlGRID.Controls.Add(
                    //     DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    //                     DTIChartsAndControls.GridView("GrafConsHistGrid", dtEquipos, false, "",
                    //                     formatos, "",new string[] { "" }, 1, columnasNoVisibles, columnasVisibles, new int[] { }),
                    //                     "RepTabHist2PnlsPrs_T", "EquiposMoviles")
                    //     );
                }
                else {
                    grvEquipo.DataSource = new DataTable();
                    grvEquipo.DataBind();
                }



                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void HidePanels()
        {
            try
            {
                //SectionCreate.Style.Add("display", "none");
                // SectionList.Style.Add("display", "none");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CargarEmpleados()
        {
            try
            {
                DataTable dtEmpleados = new DataTable();
                dtEmpleados = BuscarDTEmpleados();

                ddlEmpleado.DataSource = dtEmpleados;
                ddlEmpleado.DataTextField = "Emple";
                ddlEmpleado.DataValueField = "iCodCatEmple";
                ddlEmpleado.DataBind();

                ddlEmpleBuscar.DataSource = dtEmpleados;
                ddlEmpleBuscar.DataTextField = "Emple";
                ddlEmpleBuscar.DataValueField = "iCodCatEmple";
                ddlEmpleBuscar.DataBind();

                ddlEmpleadoModal.DataSource = dtEmpleados;
                ddlEmpleadoModal.DataTextField = "Emple";
                ddlEmpleadoModal.DataValueField = "iCodCatEmple";
                ddlEmpleadoModal.DataBind();

                ddlEmpleModalDelete.DataSource = dtEmpleados;
                ddlEmpleModalDelete.DataTextField = "Emple";
                ddlEmpleModalDelete.DataValueField = "iCodCatEmple";
                ddlEmpleModalDelete.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CargarMarcas()
        {
            try
            {
                DataTable dtMarcas = new DataTable();
                dtMarcas = BuscarDTMarcas();

                ddlMarca.DataSource = dtMarcas;
                ddlMarca.DataTextField = "Marca";
                ddlMarca.DataValueField = "iCodCatMarca";
                ddlMarca.DataBind();

                ddlMarcaBuscar.DataSource = dtMarcas;
                ddlMarcaBuscar.DataTextField = "Marca";
                ddlMarcaBuscar.DataValueField = "iCodCatMarca";
                ddlMarcaBuscar.DataBind();

                ddlMarcaModal.DataSource = dtMarcas;
                ddlMarcaModal.DataTextField = "Marca";
                ddlMarcaModal.DataValueField = "iCodCatMarca";
                ddlMarcaModal.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public DataTable BuscarDTEmpleados()
        {
            try
            {
                return DSODataAccess.Execute(ConsultaBuscarDTEmpleados());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaBuscarDTEmpleados()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("Select ");
            query.AppendLine("iCodCatEmple,");
            query.AppendLine("Emple");
            query.AppendLine("From");
            query.AppendLine("(");
            query.AppendLine("	Select ");
            query.AppendLine("		Nomina = '0',");
            query.AppendLine("		ID =0,");
            query.AppendLine("		iCodCatEmple = 0,");
            query.AppendLine("		Emple = ''");
            query.AppendLine("");
            query.AppendLine("	Union");
            query.AppendLine("");
            query.AppendLine("	Select");
            query.AppendLine("		Nomina = Nominaa,");
            query.AppendLine("		ID = ROW_NUMBER() over(order by Nominaa) ,");
            query.AppendLine("		iCodCatEmple = iCodCatalogo,");
            query.AppendLine("		Emple = NominaA +' '+NomCompleto");
            query.AppendLine("	From [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')]");
            query.AppendLine("	where dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	And dtFinVigencia >= GETDATE()");
            query.AppendLine(") as Cons");
            query.AppendLine("order by Nomina");

            return query.ToString();
        }

        public DataTable BuscarDTMarcas()
        {
            try
            {
                return DSODataAccess.Execute(ConsultaBuscarDTMarcas());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaBuscarDTMarcas()
        {
            try
            {
                StringBuilder query = new StringBuilder();


                query.AppendLine("Select *");
                query.AppendLine("From ");
                query.AppendLine("(");
                query.AppendLine("	Select ");
                query.AppendLine("	iCodcatMarca =0,");
                query.AppendLine("	Marca = ''");
                query.AppendLine("");
                query.AppendLine("	Union all");
                query.AppendLine("");
                query.AppendLine("	Select");
                query.AppendLine("		iCodCatMarca = iCodCatalogo,");
                query.AppendLine("		Marca = vchDescripcion");
                query.AppendLine("	From [" + DSODataContext.Schema + "].[visHistoricos('MarcaEquipoMovil','Marcas Equipo Movil','español')] marcEq");
                query.AppendLine("	where dtinivigencia <>dtFinVigencia");
                query.AppendLine("	And dtFinVigencia >= GETDATE()");
                query.AppendLine(") as Cons");
                query.AppendLine("order by ");
                query.AppendLine("Marca");

                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CreateEquipoMovil(EquipoMovil objEquipoMovil)
        {
            try
            {
                bool respuesta = false;

                int res = 0;
                string mensajes = "";

                DateTime dateTimeFechaIng = new DateTime();
                DateTime dateTimeFechaAsignacion = new DateTime();

                DateTime.TryParse(objEquipoMovil.FechaIngreso, out dateTimeFechaIng);
                DateTime.TryParse(objEquipoMovil.FechaAsignacion, out dateTimeFechaAsignacion);


                //Validar que la fecha de ingreso sea mayor a 1900 y que la fecha de ingreso sea menor que la fecha de asignacion
                if (dateTimeFechaIng > new DateTime(2011, 01, 01) && dateTimeFechaAsignacion >= dateTimeFechaIng)
                {

                    objEquipoMovil.FechaIngreso = dateTimeFechaIng.ToString("yyyy-MM-dd 00:00:00");
                    objEquipoMovil.FechaAsignacion = dateTimeFechaAsignacion.ToString("yyyy-MM-dd 00:00:00");
                    DataTable dtRes = new DataTable();

                    dtRes = DSODataAccess.Execute(ConsultaCreateEquipoMovil(objEquipoMovil, "C"));
                    res = Convert.ToInt32(dtRes.Rows[0][0].ToString());
                    mensajes = dtRes.Rows[0][1].ToString();

                }

                respuesta = Convert.ToBoolean(res);

                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool UpdateEquipoMovil(EquipoMovil objEquipoMovil)
        {
            try
            {
                bool respuesta = false;


                int res = 0;
                string mensajes = "";

                DateTime dateTimeFechaIng = new DateTime();
                DateTime dateTimeFechaAsignacion = new DateTime();

                DateTime.TryParse(objEquipoMovil.FechaIngreso, out dateTimeFechaIng);
                DateTime.TryParse(objEquipoMovil.FechaAsignacion, out dateTimeFechaAsignacion);


                //Validar que la fecha de ingreso sea mayor a 1900 y que la fecha de ingreso sea menor que la fecha de asignacion
                if (dateTimeFechaIng > new DateTime(2011, 01, 01))
                {
                    objEquipoMovil.FechaIngreso = dateTimeFechaIng.ToString("yyyy-MM-dd 00:00:00");
                    objEquipoMovil.FechaAsignacion = dateTimeFechaAsignacion >= dateTimeFechaIng ? dateTimeFechaAsignacion.ToString("yyyy-MM-dd 00:00:00") : "";

                    DataTable dtRes = new DataTable();

                    dtRes = DSODataAccess.Execute(ConsultaCreateEquipoMovil(objEquipoMovil, "U"));
                    res = Convert.ToInt32(dtRes.Rows[0][0].ToString());
                    mensajes = dtRes.Rows[0][1].ToString();
                }

                return respuesta = Convert.ToBoolean(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool BajaEquipoMovil(EquipoMovil objEquipoMovil)
        {
            try
            {
                bool resultado = false;

                DateTime datetimeFechRetiro = new DateTime();
                DataTable dtRes = new DataTable();
                int res = 0;
                string mensajes = "";

                if (DateTime.TryParse(objEquipoMovil.FechaRetiro, out datetimeFechRetiro))
                {
                    objEquipoMovil.FechaRetiro = datetimeFechRetiro >= Convert.ToDateTime(objEquipoMovil.FechaIngreso) ? datetimeFechRetiro.ToString("yyyy-MM-dd 00:00:00") : "";

                    dtRes = DSODataAccess.Execute(ConsultaCreateEquipoMovil(objEquipoMovil, "D"));

                    res = Convert.ToInt32(dtRes.Rows[0][0].ToString());
                    mensajes = dtRes.Rows[0][1].ToString();

                }


                resultado = Convert.ToBoolean(res);


                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaCreateEquipoMovil(EquipoMovil objEquipoMovil, string Accion)
        {
            try
            {
                int motivoRetiro = 0;
                int.TryParse(objEquipoMovil.MotivoRetiro, out motivoRetiro);

                StringBuilder query = new StringBuilder();
                query.AppendLine("Exec EquipoMovilCRUD");
                query.AppendLine("	@Esquema = '" + DSODataContext.Schema + "',");
                query.AppendLine("	@IMEI			= '" + objEquipoMovil.IMEI + "',");
                query.AppendLine("	@NoSerie			= '" + objEquipoMovil.NoSerie + "', ");
                query.AppendLine("	@Marca			=	" + objEquipoMovil.Marca + ",");
                query.AppendLine("	@Modelo			= '" + objEquipoMovil.Modelo + "',");
                query.AppendLine("	@FechaIngreso	= '" + objEquipoMovil.FechaIngreso + "',");
                query.AppendLine("	@FechaRetiro		= '" + objEquipoMovil.FechaRetiro + "',");
                query.AppendLine("	@AlmacenResguardo= '" + objEquipoMovil.AlmacenResguardo + "',");
                query.AppendLine("	@MotivoRetiro	=	" + motivoRetiro + ",");
                query.AppendLine("	@Color			= '" + objEquipoMovil.Color + "',");
                query.AppendLine("  @ICodCatEmple = '" + objEquipoMovil.ICodCatEmple + "',");
                query.AppendLine("  @FechaAsignacion = '" + objEquipoMovil.FechaAsignacion + "',");
                query.AppendLine("  @razonRetiroEquipo = " + objEquipoMovil.ValorRetiroEquipo + ",");
                query.AppendLine("	@MovimientoCRUD	= '" + Accion + "'  ");



                query.AppendLine("");

                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable BuscarDTEquiposFiltros(EquipoMovil objEquipoMovil)
        {
            try
            {
                return DSODataAccess.Execute(ConsultaBuscarDTEquiposFiltros(objEquipoMovil));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaBuscarDTEquiposFiltros(EquipoMovil objEquipoMovil)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("Select ");
                query.AppendLine("    IMEI,");
                query.AppendLine("    iCodCatMarca = MarcaEquipoMovil,");
                query.AppendLine("    MarcaEquipoMovilDesc,");
                query.AppendLine("    ModeloCel,");
                query.AppendLine("    NSerie,");
                query.AppendLine("    Color,");
                query.AppendLine("    FechaReg,");
                query.AppendLine("    FechaAsignacion = relEquipoEmple.dtIniVigencia,");
                query.AppendLine("    iCodCatEmple =  emple.iCodCatalogo,");
                query.AppendLine("    Nomina = emple.NominaA,");
                query.AppendLine("    Emple = emple.NomCompleto,");
                query.AppendLine("    iCodCatEquipo = HistEquipoMovil.iCodCatalogo   ");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('EquipoMovil','Equipos Moviles','Español')] HistEquipoMovil");
                query.AppendLine("");
                query.AppendLine("Left Join [" + DSODataContext.Schema + "].[VisRelaciones('Equipo Movil -Emple','Español')] relEquipoEmple");
                query.AppendLine("On  relEquipoEmple.EquipoMovil = HistEquipoMovil.iCodCatalogo");
                query.AppendLine("And relEquipoEmple.dtIniVigencia <> relEquipoEmple.dtFinVigencia");
                query.AppendLine("And relEquipoEmple.dtFinVigencia >= GETDATE()");
                query.AppendLine("");
                query.AppendLine("Left Join [" + DSODataContext.Schema + "].[visHistoricos('emple','empleados','español')] emple");
                query.AppendLine("On relEquipoEmple.emple = emple.icodcatalogo");
                query.AppendLine("And emple.dtIniVigencia <> emple.dtFinVigencia");
                query.AppendLine("And emple.dtFinVigencia >= GETDATE()");
                query.AppendLine("");
                query.AppendLine("Where HistEquipoMovil.dtIniVigencia <> HistEquipoMovil.dtFinVigencia");
                query.AppendLine("And HistEquipoMovil.dtFinVigencia >= GETDATE()");

                if (objEquipoMovil.iCodCatHistEquipo != null && objEquipoMovil.iCodCatHistEquipo > 0)
                {
                    query.AppendLine("And HistEquipoMovil.iCodcatalogo = " + objEquipoMovil.iCodCatHistEquipo + "");
                }

                if (objEquipoMovil.Marca != null && objEquipoMovil.Marca > 0)
                {
                    query.AppendLine("And HistEquipoMovil.MarcaEquipoMovil = " + objEquipoMovil.Marca + "");
                }

                if (objEquipoMovil.IMEI != null && objEquipoMovil.IMEI.Length > 0)
                {
                    query.AppendLine("And HistEquipoMovil.IMEI like'%" + objEquipoMovil.IMEI + "%'");
                }

                if (objEquipoMovil.Modelo != null && objEquipoMovil.Modelo.Length > 0)
                {
                    query.AppendLine("And HistEquipoMovil.ModeloCel like' %" + objEquipoMovil.Modelo + "%'");
                }


                if (objEquipoMovil.NoSerie != null && objEquipoMovil.NoSerie.Length > 0)
                {
                    query.AppendLine("And HistEquipoMovil.NSerie like'%" + objEquipoMovil.NoSerie + "%'");
                }

                if (objEquipoMovil.Color != null && objEquipoMovil.Color.Length > 0)
                {
                    query.AppendLine("And HistEquipoMovil.Color like'%" + objEquipoMovil.Color + "%'");
                }

                if (objEquipoMovil.FechaIngreso != null && objEquipoMovil.FechaIngreso.Length > 0)
                {
                    query.AppendLine("And HistEquipoMovil.fechaReg like ' %" + objEquipoMovil.FechaIngreso + "%'");
                }

                if (objEquipoMovil.ICodCatEmple != null && objEquipoMovil.ICodCatEmple > 0)
                {
                    query.AppendLine("And relEquipoEmple.emple =" + objEquipoMovil.ICodCatEmple + "");
                }

                query.AppendLine("Order by HistEquipoMovil.IMEI");



                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        #endregion

        protected void btnAccionModalDelete_Click(object sender, EventArgs e)
        {

        }

        protected void btnAceptarModalDelete_Click(object sender, EventArgs e)
        {

            EquipoMovil objEquipoMovil = new EquipoMovil();

            objEquipoMovil.iCodCatHistEquipo = Convert.ToInt32(lblICodCatEquipoModalDelete.Text);
            objEquipoMovil.IMEI = txtIMEIModalDelete.Text;
            objEquipoMovil.FechaRetiro = txtFechaRetiroModalDelete.Text;
            objEquipoMovil.MotivoRetiro = ddlMotivoModalDelete.SelectedItem.Value;
            objEquipoMovil.AlmacenResguardo = txtAlmacenResguardoModalDelete.Text;
            int ValorRetiroEquipo = 0;

            if (rbtnDevueltoModalDelete.Checked)
            {
                ValorRetiroEquipo = 1;
            }

            if (rbtnBasuraModelDelete.Checked)
            {
                ValorRetiroEquipo = 2;
            }
            objEquipoMovil.ValorRetiroEquipo = ValorRetiroEquipo;

            if (BajaEquipoMovil(objEquipoMovil))
            {
                lblTituloModalMsn.Text = "Delete Exitoso";
                lblBodyModalMsn.Text = "El equipo se dio dio de baja de la base de datos";
            }
            else
            {
                lblTituloModalMsn.Text = "Delete Fallido";
                lblBodyModalMsn.Text = "Sucedio un error al tratar  de dar de baja el equipo de la base de datos";
            }

            mpeEquipoMsn.Show();
        }




    }
}
