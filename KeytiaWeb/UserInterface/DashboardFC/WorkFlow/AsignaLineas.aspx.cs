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
    public partial class AsignaLineas : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                esquema = DSODataContext.Schema;
                connStr = DSODataContext.ConnectionString;
                if (!Page.IsPostBack)
                {
                    IniciaProceso(esquema, connStr);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path
                      + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }
        #region METODOS
        public void IniciaProceso(string esquema, string con)
        {
            int iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            txtLineaModal.Text = "";
            List<Solicitudes> list = new List<Solicitudes>();
            ObtieneSolicitudes(esquema, con, ref list,0);
            string icodCat = ObtieneEmpleSerGenerales(iCodUsuario, esquema, connStr);
            /*validar si la consulta trae datos, si no trae datos mostrar un mensaje*/
            if (list.Count > 0)
            {
                rowLineas.Visible = true;
                gridLineas.DataSource = list;
                gridLineas.DataBind();

                if(icodCat == "235816")
                {
                    gridLineas.Columns[6].Visible = true;
                }
                else
                {
                    gridLineas.Columns[6].Visible = false;
                }
            }
            else
            {
                rowLineas.Visible = false;
                pnlInfo.Visible = true;
                lblMensajeInfo.Text = "No Existen Solicitudes Pendientes!";
            }
        }
        #endregion METODOS
        #region CONSULTAS
        public void ObtieneSolicitudes(string esquema, string con, ref List<Solicitudes> list,int numSol)
        {
            DataTable dt = new DataTable();
            string sp = "EXEC WorkFlowLineasObtieneSolATMTPVBAM @Esquema = '{0}', @Solicitud = {1}";
            string query = string.Format(sp, esquema, numSol);
            dt = DSODataAccess.Execute(query, con);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Solicitudes sol = new Solicitudes();
                    sol.Solicitud = Convert.ToInt32(dr["Solicitud"]);
                    sol.EmpleId = Convert.ToInt32(dr["EmpleId"]);
                    sol.Nombre = dr["NOMBRE"].ToString();
                    sol.PlanT = dr["PLANT"].ToString();
                    sol.Monto = dr["MONTO"].ToString();
                    sol.Carrier = dr["CARRIER"].ToString();
                    sol.Recurso = dr["RECURSO"].ToString();
                    list.Add(sol);
                }
            }

        }
        public void ObtienePlanTarifarios()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo, vchDescripcion FROM "+ esquema + ".[VisHistoricos('PlanTarif','Plan Tarifario','Español')]");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion <> 'Sin Asignar' ");
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                cboPlanModal.DataSource = dt;
                cboPlanModal.DataBind();
            }
        }

        public void RegistraLinea(string con, string linea, int solicitud, int plan)
        {
            try
            {
                string sp = "EXEC WorkFlowLineasAltaNuevaLinea @esquema ='{0}',@Linea = '{1}',@Solicitud = {2},@PlanId = {3} ";
                string query = string.Format(sp, DSODataContext.Schema, linea, solicitud, plan);
                DSODataAccess.ExecuteNonQuery(query, con);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion CONSULTAS

        protected void gridLineas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            txtSolicitudModal.Text = "";
            string currentCommand = e.CommandName;
            if (currentCommand.ToUpper() == "RECHAZAR")
            {
                int currentRowIndex = Convert.ToInt32(e.CommandArgument.ToString());
                string ProductID = gridLineas.DataKeys[currentRowIndex].Values["Solicitud"].ToString();
                string recurso = gridLineas.DataKeys[currentRowIndex].Values["RECURSO"].ToString();
                int solicitud = Convert.ToInt32(ProductID);
                int bitacoraid = ObtieneBitacoraId(solicitud);

                string respuesta = "Rechazada";
                int bitacoraId = ObtieneBitacoraId(solicitud);
                int nivel = 0;

                if(recurso.ToUpper() == "BAM")
                {
                    nivel = 2;
                }
                else
                {
                    nivel = 3;
                }

                int resultado = RechazaSolicitud(respuesta, bitacoraId, solicitud, nivel);
                if (resultado == 1)
                {
                    MuestraMensaje("Asignación de Lineas!", "La solicitud se Rechazo Correctamente!");
                }
                else if (resultado != 1)
                {
                    MuestraMensaje("Asignación de Lineas!", "Ocurrior un Error al Rechazar la solicitud!");                    
                }

                IniciaProceso(esquema, connStr);
            }
            else
            {
                int currentRowIndex = Convert.ToInt32(e.CommandArgument.ToString());
                string ProductID = gridLineas.DataKeys[currentRowIndex].Values["Solicitud"].ToString();
                txtSolicitudModal.Text = ProductID;
                BuscaPlan();
            }
        }
        public void MuestraMensaje(string mensajeHeader, string mensajeBody)
        {
            lblTituloModalMsn.Text = mensajeHeader;
            lblBodyModalMsn.Text = mensajeBody;
            mpeEtqMsn.Show();
        }
        public void BuscaPlan()
        {
            int folio = Convert.ToInt32(txtSolicitudModal.Text);
            List<Solicitudes> list = new List<Solicitudes>();
            list.Clear();
            ObtieneSolicitudes(esquema, connStr, ref list, folio);
            var lista = list.FirstOrDefault();
            string recurso = lista.Recurso;
            txtLineaModal.Focus();
            if (recurso.ToUpper() == "BAM")
            {
                cboPlanModal.Visible = true;
                txtPlanModal.Visible = false;
                txtPlanModal.Text = "";
                txtMontoModal.Text = "";
                txtMontoModal.Visible = false;
                lblMontoModal.Visible = false;
                ObtienePlanTarifarios();
            }
            else
            {
                txtMontoModal.Text = lista.Monto;
                cboPlanModal.Visible = false;
                txtPlanModal.Visible = true;
                txtMontoModal.Visible = true;
                lblMontoModal.Visible = true;
            }
            txtPlanModal.Text = lista.PlanT;
            lblTituloAsgnaLinea.Text = "Asignar Línea";
            mpeEditHallazo.Show();
        }
        protected void btnGuardarModal_Click(object sender, EventArgs e)
        {
            string linea = txtLineaModal.Text;
            if (linea != "" && linea != null)
            {
                int solicitud = Convert.ToInt32(txtSolicitudModal.Text);
            int planTarifId = 0;
            if (cboPlanModal.Visible ==  true)
            {
                planTarifId = Convert.ToInt32(cboPlanModal.SelectedValue);
            }
            else
            {
                planTarifId = 0;
            }

                try
                {
                    RegistraLinea(connStr, linea, solicitud, planTarifId);
                    IniciaProceso(esquema, connStr);

                    lblTituloModalMsn.Text = "Asigancíon de Linea";
                    lblBodyModalMsn.Text = "La Línea se Asigno Correctamente.";
                    mpeEtqMsn.Show();
                }
                catch(Exception ex)
                {
                    lblTituloModalMsn.Text = "Error";
                    lblBodyModalMsn.Text = "Ocurrio un error al asignar la Línea.";
                    mpeEtqMsn.Show();
                    throw ex;
                }
            }
            else
            {
                lblTituloModalMsn.Text = "Mensaje";
                lblBodyModalMsn.Text = "Debe de Ingresar una Línea.";
                mpeEtqMsn.Show();
            }
        }
        protected void btnCancelarModal_Click(object sender, EventArgs e)
        {
            mpeEditHallazo.DropShadow = false;
            txtLineaModal.Text = "";
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
        private int ObtieneBitacoraId(int solicitud)
        {
            int bitacoraId = 0;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 1 Id FROM K5Afirme.WorkflowLineasBitacoraEnvios");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND SolicitudLineaMovilId = " + solicitud + "");
            query.AppendLine(" AND(EventoDesc LIKE '%N1%' OR EventoDesc LIKE '%N2%' OR EventoDesc LIKE '%N3%')");
            query.AppendLine(" ORDER BY ID DESC ");

            DataTable dt = DSODataAccess.Execute(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                bitacoraId = (int)dr["Id"];
            }

            return bitacoraId;
        }
        private int RechazaSolicitud(string respuesta, int bitacoraId, int solicitudId, int nivel)
        {
            int resultado = 0;
            string motivo = "Solicitud Rechazada por Servicios Generales";
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [WorkflowLineasRespondeSolicitud]");
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
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }
    }

    public class Solicitudes
    {
        public int Solicitud { get; set; }
        public int EmpleId { get; set; }
        public string Nombre { get; set; }
        public string PlanT { get; set; }
        public string Monto { get; set; }
        public string Carrier { get; set; }
        public string Recurso { get; set; }
    }
}