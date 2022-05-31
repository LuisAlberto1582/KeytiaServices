using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class AltaSolicitud : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int iCodUsuario;
        private int tipoSol = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            tipoSol = Convert.ToInt32(Request.QueryString["p"]);
            /*Agregar una Validacion cuando entre el administrdor
             para que pueda buscar la nomina del empledo
             y que valide si ese empleado ya tiene un recurso asignado
             una linea o una solicitud, buscando por nomina*/

            int perfil = (int)Session["iCodPerfil"];
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            if (!Page.IsPostBack)
            {


                if (perfil == 367 || perfil == 225504)
                {
                    pnlMapaNav.Visible = true;
                    row1.Visible = false;
                    row2.Visible = false;
                }
                else
                {

                    IniciProceso(iCodUsuario, "", 1, tipoSol);
                    pnlMapaNav.Visible = false;
                }

                var esDemo = DSODataContext.Schema.ToLower();
                if (esDemo.ToUpper() == "K5AFIRME")
                {
                    rowUbicacion.Visible = true;
                    ObtieneUbicacion();
                }
            }


        }

        private void ObtieneUbicacion()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT id,Nombre, Estatus FROM K5AFIRME.WorkFlowLineasUbicaciones");
            query.AppendLine(" WHERE Activo = 1");

            DataTable dt = DSODataAccess.Execute(query.ToString(), DSODataContext.ConnectionString);
            if (dt != null && dt.Rows.Count > 0)
            {
                cboUbicacion.Items.Insert(0, "Seleccione Una Ubicación");
                cboUbicacion.SelectedIndex = 0;
                cboUbicacion.DataSource = dt;
                cboUbicacion.DataBind();
            }
        }
        public void IniciProceso(int usuario, string nomina, int opcion, int tipoSolicitud)
        {
            try
            {
                if (Session["DatosEmple"] != null) Session["DatosEmple"] = null;
                if (Session["EmailsAut"] != null) Session["EmailsAut"] = null;

                ValidaSolicitud(usuario, nomina, opcion, tipoSolicitud);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void ValidaSolicitud(int usuario, string nomina, int opcion, int tiposol)
        {
            /*validar el tipo de solicitud CELULAR,BAM,ATM,BAM*/
            if (tiposol == 1)/*solitud de Linea de Voz*/
            {
                var telefono = ValidaEmpleLinea(usuario, nomina, opcion);
                var esDemo = DSODataContext.Schema.ToLower() == "sperto" ? true : false;

                if (telefono != "" && !esDemo)/*Valida si ya cuenta con una linea en un Equipo Celular*/
                {
                    row1.Visible = false;
                    row2.Visible = false;

                    lblMensajeInfo.Text = " Ya cuenta con un equipo celular asignado con la siguiente línea:  " + telefono;
                    pnlInfo.Visible = true;
                    return;
                }
                else if (telefono == "" || esDemo)
                {
                    /*validar la solicitud*/
                    int folio = ValidaSolicitudEmple(usuario, nomina, opcion);
                    if (folio > 0 && !esDemo)
                    {
                        row1.Visible = false;
                        row2.Visible = false;
                        lblMensajeInfo.Text = " Ya tiene una solicitud en progreso, para un equipo celular con número de folio:  " + folio;
                        pnlInfo.Visible = true;
                        return;
                    }
                    else
                    {/*cuando el empleado no cuenta con una linea asignda y no tiene una solicitud en proceso*/
                        if (esDemo)
                        {
                            rowCorreoDirector.Visible = true;
                            rowCorreoAdminMoviles.Visible = true;
                        }

                        row1.Visible = true;
                        row2.Visible = true;
                        ObtieneDatosUsuario(usuario, nomina, opcion);
                        ObtieneTipoRecurso(tiposol);
                    }

                }
            }
            else /*solicitud de BAM,ATM,TPV*/
            {
                row1.Visible = true;
                row2.Visible = true;
                ObtieneDatosUsuario(usuario, nomina, opcion);
                ObtieneTipoRecurso(tiposol);
            }

        }
        public void MuestraMensajeSolicitud(string mensajeHeader, string mensajeBody)
        {
            lblTituloModalMsn.Text = mensajeHeader;
            lblBodyModalMsn.Text = mensajeBody;
            mpeEtqMsn.Show();
        }
        [WebMethod]
        public static object GetEmploye(string texto)
        {
            DataTable Emple = new DataTable();
            string connStr = DSODataContext.ConnectionString;
            string sp = "EXEC WorkFlowLineasBuscaEmpleados @Esquema = '{0}', @Prefix = '{1}'";
            string query = string.Format(sp, DSODataContext.Schema, texto);
            Emple = DSODataAccess.Execute(query, connStr);
            DataView dvldt = new DataView(Emple);
            Emple = dvldt.ToTable(false, new string[] { "NomCompleto", "NominaA" });
            Emple.Columns["NomCompleto"].ColumnName = "Nombre";
            Emple.Columns["NominaA"].ColumnName = "Nomina";
            return FCAndControls.ConvertDataTabletoJSONString(Emple);
        }
        #region CosultasBD
        public string ValidaEmpleLinea(int usuario, string nomina, int opcion)
        {
            DataTable dt = new DataTable();
            string telefono = "";
            string sp = "EXEC WorkFlowLineasValidaRecursoEmple @Esquema = '{0}',@Usuario = {1}, @Nomina = '{2}', @Opcion = {3}";
            string query = string.Format(sp, DSODataContext.Schema, usuario, nomina, opcion);
            dt = DSODataAccess.Execute(query.ToString(), DSODataContext.ConnectionString);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                telefono = dr["DATO"].ToString();
            }

            return telefono;
        }
        public int ValidaSolicitudEmple(int usuario, string nomina, int opcion)
        {
            DataTable dt = new DataTable();
            int valor = 0;
            string sp = "EXEC WorkFlowLineasValidaSolEnProgreso @Esquema = '{0}',@Usuario = {1}, @Nomina = '{2}', @Opcion = {3}";
            string query = string.Format(sp, DSODataContext.Schema, usuario, nomina, opcion);

            dt = DSODataAccess.Execute(query.ToString(), DSODataContext.ConnectionString);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                valor = Convert.ToInt32(dr["DATO"]);
            }

            return valor;
        }

        public void ObtieneTipoRecurso(int opcion)
        {
            hfTipoRecurso.Value = "";
            int sociedad = Convert.ToInt32(hfSociedad.Value);
            string puesto = txtPuesto.Text.ToUpper();
            string sp = "EXEC WorkFlowLineasObtieneTipoRecurso @Esquema = '{0}',@IcodSociedad = {1},@Puesto = '{2}' ,@OpcionSolRecurso = {3}";
            string query = string.Format(sp, DSODataContext.Schema, sociedad, puesto, opcion);
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query, connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                txtTipoRecurso.Text = dr["Descripcion"].ToString();
                hfTipoRecurso.Value = dr["Id"].ToString();
                int recursoId = Convert.ToInt32(dr["Id"]);
                ValidaTipoRecurso(recursoId, opcion);
            }
            else
            {
                lblTituloModalMsn.Text = "Mensaje!";
                lblBodyModalMsn.Text = "NO Cuenta con Permisos para Solicitar un Recurso!";
                mpeEtqMsn.Show();
                row1.Visible = false;
                row2.Visible = false;
                return;
            }

        }
        public void ObtieneDatosUsuario(int usuario, string nomina, int opcion)
        {
            int perfil = (int)Session["iCodPerfil"];

            DataTable dt = new DataTable();
            string sp = "EXEC WorkFlowLineasDatosEmpleSol @Esquema = '{0}', @Usuario = {1},@Nomina = '{2}',@Opcion= {3}";
            string query = string.Format(sp, DSODataContext.Schema, usuario, nomina, opcion);


            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {

                DatosEmple emple = new DatosEmple();
                DataRow dr = dt.Rows[0];
                emple.icodCatalogo = Convert.ToInt32(dr["icodCatalogo"]);
                int claveEmple = Convert.ToInt32(dr["icodCatalogo"]);
                iCodCatEmple.Value = dr["icodCatalogo"].ToString();
                txtEmail.Focus();
                txtNomina.Text = dr["NominaA"].ToString();
                txtNombre.Text = dr["NomCompleto"].ToString();
                txtDireccion.Text = "";
                emple.CenCos = Convert.ToInt32(dr["CenCos"]);
                txtArea.Text = "";
                txtEmpresa.Text = "";
                txtPuesto.Text = dr["PuestoDesc"].ToString();

                Session["DatosEmple"] = emple;
                /*Metodo para Obtener los demas datos del empleado*/
                ObtieneDatosComplementariosEmple(claveEmple);

            }
        }
        public void ObtieneEmailAutorizadores(int claveEmple, int perfilId, int tipoRecurso, int sociedad, int direccion, int cencos, int area)
        {

            string sp = "EXEC WorkflowLineasObtieneDestinatariosV2 @esquema = {0}, @icodCatEmple = {1},@PerfilId = {2},@TipoRecursoId = {3}, @SociedadID = {4},@DireccionID = {5},@CencosId = {6},@Area = {7}";
            string query = string.Format(sp, DSODataContext.Schema, claveEmple, perfilId, tipoRecurso, sociedad, direccion, cencos, area);
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                Autorizadores emailAut = new Autorizadores();
                DataRow dr = dt.Rows[0];



                emailAut.AutorizadoresN1 = dr["AutorizadoresN1"].ToString();
                emailAut.AutorizadoresN2 = dr["AutorizadoresN2"].ToString();
                emailAut.AutorizadoresN3 = dr["AutorizadoresN3"].ToString();
                emailAut.AutorizadoresN4 = dr["AutorizadoresN4"].ToString();
                emailAut.AutorizadoresN5 = dr["AutorizadoresN5"].ToString();


                Session["EmailsAut"] = emailAut;
            }
            else
            {
                Session["EmailsAut"] = null;
            }


        }
        public void ObtieneDatosComplementariosEmple(int claveEmple)
        {
            hfSociedad.Value = "";
            hfDireccion.Value = "";
            hfCencos.Value = "";
            hfArea.Value = "";
            string sp = "EXEC WorkflowLineasObtieneDireccionEmple @esquema = {0}, @icodCatEmple = {1}";
            string query = string.Format(sp, DSODataContext.Schema, claveEmple);
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                hfSociedad.Value = dr["iCodCatSociedad"].ToString();
                hfDireccion.Value = dr["iCodCatDireccion"].ToString();
                hfCencos.Value = dr["iCodCatCencos"].ToString();
                hfArea.Value = dr["iCodCatArea"].ToString();
                txtEmpresa.Text = dr["Sociedad"].ToString();
                txtArea.Text = dr["Area"].ToString();
                txtDireccion.Text = dr["Direccion"].ToString();
            }
        }
        public DataTable ObtienePlanTarifDispositivo(int IcodSociedad, string Puesto, int TipoRecurso)
        {
            DataTable dt = new DataTable();
            //string sp = "EXEC WorkFlowLineasObtienePlanPerfil @Esquema = '{0}', @Perfil = {1}";
            string sp = "EXEC WorkFlowLineasObtienePlanPerfil @Esquema = '{0}', @IcodSociedad= {1},@Puesto='{2}',@TipoRecurso = {3}";
            string query = string.Format(sp, DSODataContext.Schema, IcodSociedad, Puesto, TipoRecurso);
            return dt = DSODataAccess.Execute(query.ToString(), connStr);

        }
        public int InsertaSolicitud()
        {
            /*quitar el  parametro de dispositivoID*/
            /*Obtener el tipo de movimiento de forma dinamica
             por lo pronto es estatico
             97895 ALTA
             */
            int puestoNuevaCreacion = 0;
            if (chkPuesto.Checked == true)
            {
                puestoNuevaCreacion = 1;
            }




            int resultado = 0;
            int perfil = Convert.ToInt32(hfPerfil.Value);//Convert.ToInt32(cboPerfil.SelectedValue);
            int tipoRecurso = Convert.ToInt32(hfTipoRecurso.Value);//Convert.ToInt32(cboTipoRecurso.SelectedValue);//Convert.ToInt32(hfTipoRecurso.Value);
            int planTarifario = Convert.ToInt32(hfPlanTarif.Value);
            string obje = objetivo.Text.ToUpper();
            string objetive = obje.Replace("INSERT", "").Replace("UPDATE", "").Replace("DELETE", "").Replace("DROP", "").Replace("TRUNCATE", "");
            string justif = justificacion.Text.ToUpper();
            string justify = justif.Replace("INSERT", "").Replace("UPDATE", "").Replace("DELETE", "").Replace("DROP", "").Replace("TRUNCATE", "");
            string emailCliente = txtEmail.Text;
            string direccionEnvio = txtDireccionEnvio.Text.ToUpper();
            string envioEquipo = direccionEnvio.Replace("INSERT", "").Replace("UPDATE", "").Replace("DELETE", "").Replace("DROP", "").Replace("TRUNCATE", "");
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC WorkFlowLineasInsertaSolicitud ");
            query.AppendLine(" @Esquema = '{0}',");
            query.AppendLine(" @MovimientoId = {1},");
            query.AppendLine(" @PerfilId = {2},");
            query.AppendLine(" @EmpleId = {3},");
            query.AppendLine(" @CencosId = {4},");
            query.AppendLine(" @TipoRecursoId = {5},");
            query.AppendLine(" @PlanTarifarioId = {6},");
            query.AppendLine(" @EstatusSolicitud = '{7}',");
            query.AppendLine(" @ObjetivoSol = '{8}',");
            query.AppendLine(" @JustificacionSol = '{9}',");
            query.AppendLine(" @AutorizadoresN1 = '{10}',");
            query.AppendLine(" @AutorizadoresN2 = '{11}',");
            query.AppendLine(" @AutorizadoresN3 = '{12}',");
            query.AppendLine(" @AutorizadoresN4 = '{13}',");
            query.AppendLine(" @AutorizadoresN5 = '{14}',");
            query.AppendLine(" @EmailDestinatario = '{15}',");
            query.AppendLine(" @DireccionEnvio = '{16}',");
            query.AppendLine(" @BanderaPuestoNuevaCreacion = {17},");
            query.AppendLine(" @EmailAdminMoviles = '{18}'");
            try
            {
                Autorizadores Autorizadores = (Autorizadores)(Session["EmailsAut"]);

                //RM20190315 variable guarda los correos de director y administrador  obtenidos del front DEMO
                string directorCorreo = txtCorreoDirector.Text;
                string AdminMovCorreo = txtCorreoAdminMoviles.Text;


                if (directorCorreo.Length > 0)
                {
                    Autorizadores.AutorizadoresN1 = directorCorreo;
                }

                if (AdminMovCorreo.Length <= 0)
                {
                    AdminMovCorreo = "";
                }

                //if (AdminMovCorreo.Length > 0)
                //{
                //    Autorizadores.AutorizadoresN5 = AdminMovCorreo;
                //}


                DatosEmple Empleado = (DatosEmple)(Session["DatosEmple"]);


                //No importa lo que se mande como se busca en tipo de movimiento de alta 
                string insertaSol = string.Format(query.ToString(), DSODataContext.Schema, 97895, perfil, Empleado.icodCatalogo, Empleado.CenCos,
                                                  tipoRecurso, planTarifario, "EnEsperaDeAtención", objetive, justify, Autorizadores.AutorizadoresN1, Autorizadores.AutorizadoresN2,
                                                  Autorizadores.AutorizadoresN3, Autorizadores.AutorizadoresN4, Autorizadores.AutorizadoresN5, emailCliente, envioEquipo, puestoNuevaCreacion, AdminMovCorreo);
                DSODataAccess.ExecuteNonQuery(insertaSol.ToString(), connStr);
                resultado = 1;
            }
            catch (Exception ex)
            {
                resultado = 0;
                throw ex;
            }

            return resultado;
        }
        private int ValidaEmpleEspecial(int emple)
        {
            int existe = 0;
            DataTable dt = new DataTable();
            string sp = "EXEC WorkFlowLineasEmpleEspeciales @Esquema = '{0}',@Usuario = {1}";
            string query = string.Format(sp, DSODataContext.Schema, emple);
            dt = DSODataAccess.Execute(query, connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                existe = Convert.ToInt32(dr["Existe"]);
            }

            return existe;
        }
        #endregion ConsultasBD
        private void ValidaTipoRecurso(int tipoRecurso, int tipoSol)
        {
            txtPlan.Text = "";
            hfPlanTarif.Value = "";
            txtMonto.Text = "";
            txtPerfil.Text = "";
            hfPerfil.Value = "";
            DataTable dt = new DataTable();
            if (tipoRecurso > 0)
            {
                int sociedad = Convert.ToInt32(hfSociedad.Value);
                string puesto = txtPuesto.Text.ToUpper();
                dt = ObtienePlanTarifDispositivo(sociedad, puesto, tipoRecurso);
                int perfil = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (tipoSol == 3 || tipoSol == 4)
                    {
                        //hfTipoRecurso.Value = "";
                        DataRow dr = dt.Rows[0];

                        txtPerfil.Text = dr["Descripcion"].ToString();
                        perfil = Convert.ToInt32(dr["PerfilId"]);
                        hfPerfil.Value = dr["PerfilId"].ToString();/*CLAVE DE PERFIL*/
                        cboPlan.Items.Clear();
                        cboPlan.DataSource = null;
                        cboPlan.DataBind();
                        cboPlan.Items.Insert(0, "Seleccione Un Plan");
                        cboPlan.SelectedIndex = 0;
                        rowMonto.Visible = false;
                        txtPlan.Visible = false;
                        /*mostrar un combo con los planes*/
                        cboPlan.Visible = true;
                        cboPlan.DataSource = dt;
                        cboPlan.DataBind();
                    }
                    else
                    {
                        txtPlan.Visible = true;
                        if (tipoSol == 2)
                        {
                            rowMonto.Visible = false;
                            cboPlan.Visible = false;
                        }
                        else
                        {
                            rowMonto.Visible = true;
                            cboPlan.Visible = false;
                        }

                        DataRow dr = dt.Rows[0];
                        hfPlanTarif.Value = dr["iCodCatalogo"].ToString();
                        txtPlan.Text = dr["vchDescripcion"].ToString();
                        txtMonto.Text = dr["RentaTelefonia"].ToString();
                        perfil = Convert.ToInt32(dr["PerfilId"]);
                        txtPerfil.Text = dr["Descripcion"].ToString();
                        hfPerfil.Value = dr["PerfilId"].ToString();/*CLAVE DE PERFIL*/

                    }

                    /*Obtiene Los Autorizadores*/
                    int ClaveEmple = Convert.ToInt32(iCodCatEmple.Value);
                    int direccion = Convert.ToInt32(hfDireccion.Value);
                    int cencos = Convert.ToInt32(hfCencos.Value);
                    int areaEmple = Convert.ToInt32(hfArea.Value);

                    ObtieneEmailAutorizadores(ClaveEmple, perfil, tipoRecurso, sociedad, direccion, cencos, areaEmple);
                }
            }
            else
            {
                rowMonto.Visible = true;
                cboPlan.Visible = false;
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                int perfilPlan = 0;
                if (cboPlan.Visible == false)
                {
                    perfilPlan = 1;
                }
                else if (cboPlan.Visible == true)
                {
                    perfilPlan = Convert.ToInt32(cboPlan.SelectedValue.Replace("Seleccione Un Plan", "0"));
                    if (perfilPlan > 0)
                    {
                        perfilPlan = 1;
                    }
                }

                string valido = cboUbicacion.SelectedItem.ToString();
                if (valido == "Seleccione Una Ubicación")
                {
                    lblTituloModalMsn.Text = "Mensaje!";
                    lblBodyModalMsn.Text = "Debe Seleccionar una Ubicación!";
                    mpeEtqMsn.Show();
                    return;
                }
                //int perfil = Convert.ToInt32(cboTipoRecurso.SelectedValue.Replace("Selecciona un Tipo de Recurso","0"));//Convert.ToInt32(cboPerfil.SelectedValue.Replace("Seleccione Un Perfil", "0"));
                //if (perfil > 0)
                //{
                if (txtEmail.Text != "")
                {
                    if (objetivo.Text != "")
                    {
                        if (justificacion.Text != "")
                        {
                            if (txtDireccionEnvio.Text != "")
                            {
                                if (Session["EmailsAut"] != null)
                                {
                                    if (perfilPlan > 0)
                                    {
                                        int resultado = InsertaSolicitud();
                                        if (resultado == 1)
                                        {
                                            /*la solicitud fue registrada correctamente*/
                                            //lblTituloModalMsn.Text = "Solicitud Exitosa!";
                                            //lblBodyModalMsn.Text = "La Solicitud Fue Registrada Correctamente.";
                                            //mpeEtqMsn.Show();
                                            lblMensajeSuccess.Text = "La solicitud Fue Registrada Correctamente ";
                                            InfoPanelSucces.Visible = true;
                                            row1.Visible = false;
                                            row2.Visible = false;
                                            //int empleEsp = Convert.ToInt32(hfEmpleEspecial.Value);
                                            //if (empleEsp == 1)
                                            //{
                                            //rowNuevaSol.Visible = true;
                                            //}

                                        }
                                        else
                                        {
                                            /*ocurrio un error al guardar su solcitud*/
                                            lblTituloModalMsn.Text = "Error!!";
                                            lblBodyModalMsn.Text = "Ocurrio Un Error al Generar la Solicitud.";
                                            //lblMensajeDanger.Text = "Ocurrio Un Error al Generar la Solicitud.";
                                            //pnlError.Visible = true;
                                            mpeEtqMsn.Show();
                                        }
                                    }
                                    else
                                    {
                                        lblTituloModalMsn.Text = "Mensaje!";
                                        lblBodyModalMsn.Text = "Debe Seleccionar un Plan!";
                                        mpeEtqMsn.Show();
                                        return;
                                    }
                                }
                                else
                                {
                                    lblTituloModalMsn.Text = "Mensaje!";
                                    lblBodyModalMsn.Text = "No se Puede dar de Alta la Solcitud, porque no tiene un Autorizador Definido!";
                                    mpeEtqMsn.Show();
                                    return;
                                }
                            }
                            else if (txtDireccionEnvio.Text == "")
                            {
                                lblTituloModalMsn.Text = "Mensaje!";
                                lblBodyModalMsn.Text = "Debe de Ingresar una Dirección de Envío.!";
                                mpeEtqMsn.Show();
                                txtDireccionEnvio.Focus();
                                return;
                            }
                        }

                        else if (justificacion.Text == "")
                        {
                            justificacion.Focus();
                            lblTituloModalMsn.Text = "Mensaje!";
                            lblBodyModalMsn.Text = "Debe de Ingresar una Justificación!";
                            mpeEtqMsn.Show();
                            return;
                        }
                    }
                    else if (objetivo.Text == "")
                    {
                        objetivo.Focus();
                        lblTituloModalMsn.Text = "Mensaje!";
                        lblBodyModalMsn.Text = "Debe de Ingresar el Objetivo!";
                        mpeEtqMsn.Show();
                        return;
                    }
                }
                else if (txtEmail.Text == "")
                {
                    lblTituloModalMsn.Text = "Mensaje!";
                    lblBodyModalMsn.Text = "Debe de Ingresar un Email!";
                    mpeEtqMsn.Show();
                    txtEmail.Focus();
                    return;
                }

                // }
                //else
                // {
                //     lblTituloModalMsn.Text = "Mensaje!";
                //     lblBodyModalMsn.Text = "Debe Seleccionar un Recurso!";
                //     mpeEtqMsn.Show();
                //     return;
                // }
            }
            catch (Exception ex)
            {
                return;
                throw ex;

            }

            txtEmpleNom.Text = "";
            txtEmpleId.Text = "";

        }
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaControles();

            try
            {
                string nomina = txtEmpleId.Text;///Request.Form[hfNomina.UniqueID];
                IniciProceso(iCodUsuario, nomina, 2, tipoSol);
            }
            catch (Exception ex)
            {
                lblTituloModalMsn.Text = "Error!!";
                lblBodyModalMsn.Text = "Ocurrio Un Error al Buscar el Empleado.";
                mpeEtqMsn.Show();
                return;
                throw ex;
            }
        }
        private void LimpiaControles()
        {
            txtPerfil.Text = "";
            txtDireccionEnvio.Text = "";
            lblMensajeInfo.Text = "";
            pnlInfo.Visible = false;
            lblMensajeSuccess.Text = "";
            InfoPanelSucces.Visible = false;
            justificacion.Text = "";
            objetivo.Text = "";
            txtEmail.Text = "";
            //txtTipoRecurso.Text = "";
            txtMonto.Text = "";
            txtPlan.Text = "";
            //cboPerfil.Items.Clear();
            //cboPerfil.DataSource = null;
            //cboPerfil.DataBind();
            //cboPerfil.Items.Insert(0, "Seleccione Un Perfil");
            //cboPerfil.SelectedIndex = 0;
            cboPlan.Items.Clear();
            cboPlan.DataSource = null;
            cboPlan.DataBind();
            cboPlan.Items.Insert(0, "Seleccione Un Plan");
            cboPlan.SelectedIndex = 0;
            rowMonto.Visible = false;
            txtPlan.Visible = false;
            cboPlan.Visible = false;
            txtPlan.Visible = true;
            rowMonto.Visible = true;
            //cboTipoRecurso.Items.Clear();
            //cboTipoRecurso.DataSource = null;
            //cboTipoRecurso.DataBind();
            //cboTipoRecurso.Items.Insert(0, "Selecciona un Tipo de Recurso");
            //cboTipoRecurso.SelectedIndex = 0;
        }
        protected void btnNuevaSolicitud_Click(object sender, EventArgs e)
        {
            LimpiaControles();
            IniciProceso(iCodUsuario, "", 1, tipoSol);
            rowNuevaSol.Visible = false;
        }
        protected void cboPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfPlanTarif.Value = "";
            string planTarifario = cboPlan.SelectedValue.ToString();
            hfPlanTarif.Value = planTarifario.ToString();
        }

        protected void cboUbicacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string valido = cboUbicacion.SelectedValue.ToString();
            string T = Valido(Convert.ToInt32(valido));
            if (valido != "0")
            {
                if (T.Trim().ToUpper() == "NO AUTORIZADA")
                {
                    lblTitulomensaje.Text = T + "!";
                    lblmensaje.Text = "Estimado Usuario, de momento no esta autorizada la asignación de nuevas líneas celulares y lineas de datos (BAM).";
                    mpeEditHallazo.Show();
                }
            }
        }
        private string Valido(int idUbica)
        {
            string aut = "";
            StringBuilder q = new StringBuilder();
            q.Append(" SELECT Estatus FROM K5AFIRME.WorkFlowLineasUbicaciones WHERE Activo = 1 AND Id = " + idUbica + "");

            DataTable dt = DSODataAccess.Execute(q.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                aut = dr["Estatus"].ToString();
            }
            return aut;
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            if (DSODataContext.Schema.Trim().ToUpper() == "K5AFIRME")
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/DashboardFC/WorkFlow/WelcomePage.aspx", false);
            }
        }
    }

    public class DatosEmple
    {
        public int icodCatalogo { get; set; }
        public string Nomina { get; set; }
        public string Nombre { get; set; }
        public int CenCos { get; set; }
        public string CencosDesc { get; set; }
        public string Direccion { get; set; }
        public int Organizacion { get; set; }
        public string Empresa { get; set; }
        public int Puesto { get; set; }
        public string PuestoDesc { get; set; }
        public string Email { get; set; }

    }
    public class Dispositivo
    {
        public int PlanTarifId { get; set; }
        public int DispositivoId { get; set; }
    }
    public class Autorizadores
    {
        public string AutorizadoresN1 { get; set; }
        public string AutorizadoresN2 { get; set; }
        public string AutorizadoresN3 { get; set; }
        public string AutorizadoresN4 { get; set; }
        public string AutorizadoresN5 { get; set; }
    }
}