using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class AsignaciondeRecurso : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;

            if (!Page.IsPostBack)
            {
                ObtieneSol(1);
            }
        }

        protected void cboSolicitud_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            int clave = 0;
            hfEstatus.Value = "";
            hfEvento.Value = "";
            clave = Convert.ToInt32(cboSolicitud.SelectedValue.Replace("Seleccione Una Solicitud", "0"));
            if (clave > 0)
            {
                gridLineas.DataSource = null;
                gridLineas.DataBind();
                /*validar si la solicitud ya se encuentra en estatus de aceptada*/
                ObtieneEstatusSolicitud(clave);
                string estatus = hfEstatus.Value.ToString();
                string evento = hfEvento.Value.ToString();

                if (estatus.ToUpper() == "ACEPTADA")
                {
                    chkMostrar.Checked = false;
                    lblMensajeInfo.Text = "";
                    pnlInfo.Visible = false;
                    MuestraDatosEmpleSol(clave);
                    string icodCat = ObtieneEmpleSerGenerales(iCodUsuario, esquema, connStr);
                    
                    if (icodCat == "235816")/*el boton para rechazar solo se muestra cuando ingresa marcela gomez*/
                    {
                        rowRechazo.Visible = true;
                    }
                    else
                    {
                        rowRechazo.Visible = false;
                    }
                }
                else
                {
                    lblMensajeInfo.Text = "Solicitud en Estatus de : " + evento;
                    pnlInfo.Visible = true;
                    rowRechazo.Visible = false;
                    rowDatosEmple.Visible = false;
                    rowAceptar.Visible = false;
                    divLineas.Style.Value = "height:0px;overflow-y:auto;overflow-x:auto;";
                }

            }
            else
            {
                MuestraMensaje("Mensaje", "Debe de Seleccionar Una Solicitud!");
                rowDatosEmple.Visible = false;
                rowLineas.Visible = false;
                lblMensajeInfo.Text = "";
                pnlInfo.Visible = false;
                rowRechazo.Visible = false;
                grdLineasDisp.DataSource = null;
                grdLineasDisp.DataBind();

            }
        }

        #region METODOS
        public void ObtieneSol(int origen)
        {
            List<Solicitudes> list = new List<Solicitudes>();
            list.Clear();
            //ObtieneSolicitudes(ref list);
            ObtieneSolRegistradas(ref list);
            if (list.Count > 0)
            {
                cboSolicitud.DataSource = list;
                cboSolicitud.DataBind();
            }
            else
            {
                    //MuestraMensaje("Asignación de Recurso!", "No Existen Solicitudes Pendientes por Asignar Recurso!");
                    cboSolicitud.Enabled = false;
                    lblMensajeInfo.Text = "No Existen Solicitudes en Proceso!";
                    pnlInfo.Visible = true;
                    rowRechazo.Visible = false;
            }
        }
        public void MuestraMensaje(string mensajeHeader, string mensajeBody)
        {
            lblTituloModalMsn.Text = mensajeHeader;
            lblBodyModalMsn.Text = mensajeBody;
            mpeEtqMsn.Show();
        }
        public void MuestraDatosEmpleSol(int claveSol)
        {

            DatosEmpleSol listSol = new DatosEmpleSol();

            ObtieneDatosSolEmple(claveSol, listSol);
            TextBox2.Text = "";
            txtNomina.Text = listSol.NominaSolicitante;
            hfNomina.Value = listSol.ClaveSolicitante.ToString();
            txtNombre.Text = listSol.NombreSolicitante;
            txtDepartamento.Text = listSol.SolicitanteCenCos;
            txtEmail.Text = listSol.SolicitanteEmail;
            txtPuesto.Text = listSol.SolicitantePuesto;
            txtPerfil.Text = listSol.PerfilDesc;
            txtPlan.Text = listSol.PlanTarifDesc;
            hfPerilId.Value = listSol.PerfilId.ToString();
            rowDatosEmple.Visible = true;
            /*Obtiene La Empresa del Empleado*/
            int claveEmple = Convert.ToInt32(hfNomina.Value);
            ObtieneEmpresaEmple(claveEmple);
            /*Obtiene La lineas de Acuerdo al Plan Tarifario*/
            int clavePlanTarif = listSol.PlanTarifarioId;
            int clavePerfilId = listSol.PerfilId;
            int clavesociedad = Convert.ToInt32(hfEmpresa.Value);
            ListaLineasDisponiblesPlanTarif(clavePerfilId, clavesociedad,1, "0");

        }
        public void ListaLineasDisponiblesPlanTarif(int clavePerfil,int clavesociedad,int opcion,string linea)
        {
            List<DatosDispositivos> lista = new List<DatosDispositivos>();
            lista.Clear();
            ObtieneLineasPlanTarif(clavePerfil, ref lista, clavesociedad, opcion, linea);
            if (lista.Count > 0)
            {
                rowLinDisp.Visible = false;
                rowLineas.Visible = true;
                rowAceptar.Visible = true;
                var listLineas = lista.OrderBy(x => x.Empresa);
                gridLineas.DataSource = listLineas;
                gridLineas.DataBind();
                int rows = lista.Count;
                if (rows > 7)
                {
                    divLineas.Style.Value = "height:250px;overflow-y:auto;overflow-x:auto;";
                }
                else
                {
                    divLineas.Style.Value = "height:auto;overflow-y:auto;overflow-x:auto;";
                }
            }
            else
            {
                rowLineas.Visible = false;
                pnlInfo.Visible = true;
                
                lblMensajeInfo.Text = "No Existen Lineas Disponibles del Plan Solicitado en la Empresa del Empleado!";
                //lblMnesajeInfo2.Text = "Cantidad de Líneas Disponibles por Sociedad";
                //ObtieneLinDispSociedad();
            }
            TextBox2.Text = "";
        }
        public void ReloadPage()
        {
            cboSolicitud.Items.Clear();
            gridLineas.DataSource = null;
            gridLineas.DataBind();
            txtNomina.Text = "";
            hfNomina.Value = "";
            txtNombre.Text = "";
            txtDepartamento.Text = "";
            txtPuesto.Text = "";
            txtEmail.Text = "";
            rowLineas.Visible = false;
            rowDatosEmple.Visible = false;
            ObtieneSol(2);
            cboSolicitud.Items.Insert(0, "Seleccione Una Solicitud");
            rowRechazo.Visible = false;
        }
        #endregion METODOS
        #region CONSULTAS
        private void ObtieneSolRegistradas(ref List<Solicitudes> list)
        {
            DataTable dt = new DataTable();
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT ");
            query.AppendLine(" SOL.Id AS SolicitudId, ");
            query.AppendLine(" CONVERT(VARCHAR, SOL.Id) + ' - ' + EMPLE.NomCompleto AS SolicitanteNomCompleto ");
            query.AppendLine(" FROM "+ DSODataContext.Schema + ".WorkflowLineasSolicitudLineaMovil AS SOL ");
            query.AppendLine(" JOIN  " + DSODataContext.Schema + ".WorkflowLineasTipoRecurso AS TR ");
            query.AppendLine(" ON SOL.TipoRecursoId = TR.Id ");
            query.AppendLine(" AND TR.dtIniVigencia <> TR.dtFinVigencia ");
            query.AppendLine(" AND TR.dtFinVigencia >= GETDATE() ");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[vishistoricos('Emple','Empleados','Español')] AS EMPLE ");
            query.AppendLine(" ON SOL.EmpleId = EMPLE.iCodCatalogo ");
            query.AppendLine(" AND EMPLE.dtIniVigencia <> EMPLE.dtFinVigencia ");
            query.AppendLine(" AND EMPLE.dtFinVigencia >= GETDATE() ");
            query.AppendLine(" WHERE SOL.dtIniVigencia <> SOL.dtFinVigencia ");
            query.AppendLine(" AND SOL.dtFinVigencia >= GETDATE() ");
            query.AppendLine(" AND ISNULL(BanderaEquipoAsignadoDesdeKeytia,0) = 0 ");
            query.AppendLine(" AND ISNULL(BanderaSolicitudFinalizada,0) = 0 ");
            query.AppendLine(" AND TR.Clave = 'Celular' ");
            query.AppendLine(" AND ISNULL(EventoId,0) > 0 ");

            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Solicitudes sol = new Solicitudes();
                    int idSolicitud = Convert.ToInt32(dr["SolicitudId"]);
                    string nombreSol = dr["SolicitanteNomCompleto"].ToString();
                    sol.Idsolicitud = idSolicitud;
                    sol.NomSolicitante = nombreSol;
                    list.Add(sol);
                }
            }
        }
        public void ObtieneSolicitudes(ref List<Solicitudes> list)
        {
            DataTable dt = new DataTable();
            string sp_ObtieneSol = "EXEC WorkflowLineasGetDatosSolicitudes @esquema='{0}', @Opcion = 3, @EstatusSolicitud = '{1}'";
            string query = string.Format(sp_ObtieneSol, esquema, "Aceptada");

            dt = DSODataAccess.Execute(query.ToString(), connStr);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Solicitudes sol = new Solicitudes();
                    int idSolicitud = Convert.ToInt32(dr["SolicitudId"]);
                    string nombreSol = dr["SolicitanteNomCompleto"].ToString();
                    sol.Idsolicitud = idSolicitud;
                    sol.NomSolicitante = idSolicitud + " - " + nombreSol;
                    list.Add(sol);
                }
            }
        }
        public void ObtieneDatosSolEmple(int solicitud, DatosEmpleSol empleSol)
        {
            DataTable dt = new DataTable();
            string sp_ObtieneSol = "EXEC [WorkflowLineasGetDatosSolicitudes] @esquema = '{0}', @Opcion = 1, @IdSolicitud = {1}";
            string query = string.Format(sp_ObtieneSol, esquema, solicitud);
            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                empleSol.ClaveSolicitud = Convert.ToInt32(dr["SolicitudId"]);
                empleSol.ClaveSolicitante = Convert.ToInt32(dr["SolicitanteId"]);
                empleSol.NominaSolicitante = dr["SolicitanteNomina"].ToString().Substring(3, (dr["SolicitanteNomina"].ToString().Length)-3);
                empleSol.NombreSolicitante = dr["SolicitanteNomCompleto"].ToString();
                empleSol.SolicitanteCenCos = dr["SolicitanteCenCos"].ToString();
                empleSol.SolicitanteEmail = dr["SolicitanteEmail"].ToString();
                //empleSol.PlanTarifarioId = Convert.ToInt32(dr["PlanTarifarioId"]);
                empleSol.PlanTarifDesc = dr["PlanTarifarioDesc"].ToString();
                empleSol.SolicitanteClavePuesto = dr["SolicitanteClavePuesto"].ToString();
                empleSol.SolicitantePuesto = dr["SolicitantePuesto"].ToString();
                empleSol.PerfilDesc = dr["PerfilDesc"].ToString();
                empleSol.PerfilId = Convert.ToInt32(dr["PerfilId"]);
            }

        }
        public void ObtieneEstatusSolicitud(int solicitud)
        {
            DataTable dt = new DataTable();

            string sp = "EXEC WorkFlowLineasObtineEstatusSol @Esquema = '{0}',@Solicitud = {1}";
            string query = string.Format(sp, DSODataContext.Schema, solicitud);
            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string evento = dr["Evento"].ToString();
                string estatus = dr["Estatus"].ToString();

                hfEstatus.Value = estatus;
                hfEvento.Value = evento;
            }
        }
        public void ObtieneLineasPlanTarif(int clavePerfil, ref List<DatosDispositivos> dispositivos,int iCodCatSociedad,int opcion,string linea)
        {
            DataTable dt = new DataTable();
            string sp = "EXEC WorkFlowLineasGetDispositivosV2 @Esquema = '{0}', @ClavePerfilId = {1},@iCodCatSociedad = {2},@Opcion = {3},@Linea = '{4}'";
            string query = string.Format(sp, DSODataContext.Schema, clavePerfil, iCodCatSociedad, opcion, linea);
            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DatosDispositivos datosDis = new DatosDispositivos();
                    datosDis.ClaveLinea = Convert.ToInt32(dr["ClaveLinea"]);
                    datosDis.Linea = dr["Tel"].ToString();
                    datosDis.PlanTarif = Convert.ToInt32(dr["PlanTarif"]);
                    datosDis.Plan = dr["PlanTarifDesc"].ToString();
                    datosDis.ClaveDispositivo = Convert.ToInt32(dr["Dispositivo"]);
                    datosDis.Marca = dr["Marca"].ToString();
                    datosDis.Modelo = dr["Modelo"].ToString();
                    datosDis.IMEI = dr["IMEI"].ToString();
                    datosDis.SimCard = dr["SIMCard"].ToString();
                    datosDis.Empresa = dr["Empresa"].ToString();
                    dispositivos.Add(datosDis);
                }
            }
        #endregion CONSULTAS

        }
        private DataTable ObtieneDatosLinea(int claveLinea)
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" Cencosto.Descripcion AS Sociedad,");
            query.AppendLine(" LINEAS.Tel,");
            query.AppendLine(" LINEAS.PlanTarif,");
            query.AppendLine(" PlanT.vchDescripcion AS PlanTarifDesc,");
            query.AppendLine(" Cencosto.Descripcion AS Empresa");
            query.AppendLine(" FROM "+ DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] AS LINEAS");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisRelaciones('CentroCosto-Lineas','Español')]");
            query.AppendLine(" AS RELLINEA");
            query.AppendLine(" ON LINEAS.iCodCatalogo = RELLINEA.Linea");
            query.AppendLine(" AND RELLINEA.dtIniVigencia<> RELLINEA.dtFinVigencia");
            query.AppendLine(" AND RELLINEA.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[vishistoricos('CenCos','Centro de costos','Español')] AS Cencosto");
            query.AppendLine(" ON RELLINEA.Cencos = Cencosto.iCodCatalogo");
            query.AppendLine(" AND Cencosto.dtIniVigencia<> Cencosto.dtFinVigencia");
            query.AppendLine(" AND Cencosto.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('PlanTarif','Plan Tarifario','Español')] AS PlanT");
            query.AppendLine(" ON LINEAS.PlanTarif = PlanT.iCodCatalogo");
            query.AppendLine(" AND PlanT.dtIniVigencia<> PlanT.dtFinVigencia");
            query.AppendLine(" AND PlanT.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE LINEAS.dtIniVigencia<> LINEAS.dtFinVigencia");
            query.AppendLine(" AND LINEAS.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND LINEAS.iCodCatalogo = "+ claveLinea + "");

            return dt = DSODataAccess.Execute(query.ToString(), connStr);          
        }
        public int AsignaRecursoEmpleado(int solicitud, int nomina, int claveLinea)
        {
            DataTable dt = new DataTable();
            int valor = 0;
            try
            {
                string sp = "EXEC WorkFlowLineasAsignaRecursos @Esquema = '{0}', @Solicitud = {1} , @NominaId = {2}, @LineaId = {3} ,@Opcion = 1";
                string query = string.Format(sp, DSODataContext.Schema, solicitud, nomina, claveLinea);
                dt = DSODataAccess.Execute(query, connStr);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    valor = Convert.ToInt32(dr["Correcto"]);
                }
            }
            catch(Exception ex)
            {
                valor = 0;
                throw ex;
            }

            return valor;
        }
        public void ObtieneEmpresaEmple(int emple)
        {
            hfEmpresa.Value = "0";
            string sp = "EXEC WorkflowLineasObtieneDireccionEmple @esquema = '{0}', @icodCatEmple = {1}";
            string query = string.Format(sp, DSODataContext.Schema, emple);
            DataTable dt  = DSODataAccess.Execute(query, connStr);
            if (dt != null && dt.Rows.Count > 0 )
            {
                DataRow dr = dt.Rows[0];
                txtEmpresa.Text = dr["Sociedad"].ToString();
                hfEmpresa.Value = dr["iCodCatSociedad"].ToString();
            }
        }
        private void ObtieneLinDispSociedad()
        {
            string sp = "EXEC WorkFlowLineasGetCantLineasDispSociedad @Esquema = '{0}'";
            string query = string.Format(sp, DSODataContext.Schema);
            DataTable dt = DSODataAccess.Execute(query, connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                rowLinDisp.Visible = true;
                grdLineasDisp.DataSource = dt;
                grdLineasDisp.DataBind();
                int rows = dt.Rows.Count;
                if (rows > 10)
                {
                    divLineas.Style.Value = "height:250px;overflow-y:auto;overflow-x:auto;";
                }
                else
                {
                    divLineas.Style.Value = "height:auto;overflow-y:auto;overflow-x:auto;";
                }
            }
            else
            {
                rowLinDisp.Visible = false;
                grdLineasDisp.DataSource = null;
                grdLineasDisp.DataBind();
            }
        }
        public class Solicitudes
        {
            public int Idsolicitud { get; set; }
            public string NomSolicitante { get; set; }
        }
        public class DatosEmpleSol
        {
            public int ClaveSolicitud { get; set; }
            public int ClaveSolicitante { get; set; }
            public string NominaSolicitante { get; set; }
            public string NombreSolicitante { get; set; }
            public string SolicitanteCenCos { get; set; }
            public string SolicitanteEmail { get; set; }
            public string SolicitanteClavePuesto { get; set; }
            public string SolicitantePuesto { get; set; }
            public int PlanTarifarioId { get; set; }
            public string PlanTarifDesc { get; set; }
            public int PerfilId { get; set; }
            public string PerfilDesc { get; set; }
        }
        public class DatosDispositivos
        {
            public int ClaveLinea { get; set; }
            public string Linea { get; set; }
            public int PlanTarif { get; set; }
            public string Plan { get; set; }
            public int ClaveDispositivo { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string IMEI { get; set; }
            public string SimCard { get; set; }
            public string Empresa { get; set; }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {            
            int claveLinea = Convert.ToInt32(Request.Form["rbtnPlanTarif"]);
            
            if (claveLinea > 0)
            {
                DataTable dt = ObtieneDatosLinea(claveLinea);
                DataRow dr = dt.Rows[0];
                txtLineaAsignar.Text = dr["Tel"].ToString();
                txtLineaPlan.Text = dr["PlanTarif"].ToString();
                txtLineaEmpre.Text = dr["Sociedad"].ToString();

                lblTituloAsgnaLinea2.Text = "¿Está seguro de asignar el siguiente recurso?";
                mpeEditHallazo.Show();
                claveLineaAsignar.Value = claveLinea.ToString();
            }
            else
            {
                MuestraMensaje("Asignación de Recurso!", "Debe de Seleccionar un Recurso!");
            }
        }

        protected void btnRechazar_Click(object sender, EventArgs e)
        {
            int solicitud = Convert.ToInt32(cboSolicitud.SelectedValue);
            if (solicitud > 0)
            {
               
                string respuesta = "Rechazada";
                int bitacoraId = ObtieneBitacoraId(solicitud);
                int nivel = 3;
                int resultado = RechazaSolicitud(respuesta, bitacoraId, solicitud, nivel);

                if (resultado == 1)
                {
                    MuestraMensaje("Asignación de Recurso!", "La solicitud se Rechazo Correctamente!");
                    rowLinDisp.Visible = false;
                    lblMnesajeInfo2.Text = "";
                }
                else if (resultado != 1)
                {
                    MuestraMensaje("Asignación de Recurso!", "Ocurrior un Error al Rechazar la solicitud!");
                    rowLinDisp.Visible = false;
                    lblMnesajeInfo2.Text = "";
                }

                ReloadPage();
            }
        }
        private int  ObtieneBitacoraId(int solicitud)
        {
            int bitacoraId = 0;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 1 Id FROM K5Afirme.WorkflowLineasBitacoraEnvios");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND SolicitudLineaMovilId = "+ solicitud + "");
            query.AppendLine(" AND(EventoDesc LIKE '%N1%' OR EventoDesc LIKE '%N2%' OR EventoDesc LIKE '%N3%')");
            query.AppendLine(" ORDER BY ID DESC ");

           DataTable dt = DSODataAccess.Execute(query.ToString());
            if( dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                bitacoraId = (int)dr["Id"];
            }

            return bitacoraId;
        }
        private int RechazaSolicitud(string respuesta,int bitacoraId,int solicitudId,int nivel)
        {
            int resultado = 0;
            string motivo = "Solicitud Rechazada por Servicios Generales";
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [WorkflowLineasRespondeSolicitud]");
            query.AppendLine("  @Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @respuesta = '" + respuesta + "',");
            query.AppendLine("  @regBitacora = " + bitacoraId + ",");
            query.AppendLine("  @SolicitudId = " + solicitudId + ",");
            query.AppendLine("  @lvlResp = " + nivel + ",");
            query.AppendLine("  @comentarios = '" + motivo + "'");
            try
            {
                DSODataAccess.Execute(query.ToString());
                resultado = 1;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return resultado;
        }
        private string ObtieneEmpleSerGenerales(int usuario, string esquema, string con)
        {
            string iCodCatalogo = "";
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" U.iCodCatalogo");
            query.AppendLine(" FROM " + esquema + ".[VisHistoricos('Usuar','Usuarios','Español')] AS U");
            query.AppendLine(" JOIN " + esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E");
            query.AppendLine(" ON U.iCodCatalogo = E.Usuar");
            query.AppendLine(" AND E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE U.dtIniVigencia <> U.dtFinVigencia");
            query.AppendLine(" AND U.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND U.iCodCatalogo =" + usuario + " ");

            DataTable dt = DSODataAccess.Execute(query.ToString(), con);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                iCodCatalogo = dr["iCodCatalogo"].ToString();
            }

            return iCodCatalogo;
        }
        protected void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {

            if(chkMostrar.Checked == true)
            {
                gridLineas.DataSource = null;
                gridLineas.DataBind();
                /*mostrar todas las lineas disponibles*/
                int clavePerfilId = Convert.ToInt32(hfPerilId.Value);
                int clavesociedad = Convert.ToInt32(hfEmpresa.Value);
                ListaLineasDisponiblesPlanTarif(clavePerfilId, clavesociedad,2, "0");
            }
            else
            {
                gridLineas.DataSource = null;
                gridLineas.DataBind();
                /*Obtiene La lineas de Acuerdo al Plan Tarifario*/
                int clavePerfilId = Convert.ToInt32(hfPerilId.Value);
                int clavesociedad = Convert.ToInt32(hfEmpresa.Value);
                ListaLineasDisponiblesPlanTarif(clavePerfilId, clavesociedad,1, "0");
            }

        }
        protected void btn1_Click(object sender, EventArgs e)
        {
            gridLineas.DataSource = null;
            gridLineas.DataBind();
            /*BUSCA LOS REGISTROS EN BASE A SOCIEDAD, PLAN O NUMERO DE LINEA*/
            int clavePerfilId = Convert.ToInt32(hfPerilId.Value);
            int clavesociedad = Convert.ToInt32(hfEmpresa.Value);
            string numLinea = TextBox2.Text;
            if (numLinea != "")
            {
                string busqueda = numLinea.Replace("INSERT", "").Replace("UPDATE", "").Replace("DELETE", "").Replace("DROP", "").Replace("TRUNCATE", "");
                ListaLineasDisponiblesPlanTarif(clavePerfilId, clavesociedad, 3, busqueda.ToUpper());
            }
        }

        protected void btnGuardarModal_Click(object sender, EventArgs e)
        {
            int claveLinea = Convert.ToInt32(claveLineaAsignar.Value);
            int nomina = Convert.ToInt32(hfNomina.Value);
            int solicitud = Convert.ToInt32(cboSolicitud.SelectedValue);

            var estatus = AsignaRecursoEmpleado(solicitud, nomina, claveLinea);
            if (estatus == 1)
            {
                MuestraMensaje("Asignación de Recurso!", "El Recurso Fue Asignado Correctamente!");

            }
            else if (estatus != 1)
            {
                MuestraMensaje("Asignación de Recurso!", "Ocurrior un Error al Asignar el Recurso!");
            }

            ReloadPage();
        }
    }
}