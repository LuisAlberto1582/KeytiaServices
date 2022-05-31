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
    public partial class SeguimientoMiSolicitud : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int iCodUsuario;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            if (!Page.IsPostBack)
            {
                IniciProceso(esquema, connStr, iCodUsuario);
            }
        }
        #region METODOS
        private void IniciProceso(string esquema, string con,int icodUsuario)
        {
            int emple = ObtieneClaveEmple(esquema, con, icodUsuario);
            if (emple > 0)
            {
                List<Solicitud> lista = new List<Solicitud>();
                ObtieneSol(esquema, connStr, ref lista, emple);
                if (lista.Count > 0)
                {
                    lblMensajeInfo.Text = "!";
                    pnlInfo.Visible = false;
                    cboSolicitudes.DataSource = lista;
                    cboSolicitudes.DataBind();
                }
                else
                {
                    cboSolicitudes.Enabled = false;
                    lblMensajeInfo.Text = "No Existen Solicitudes en Proceso!";
                    pnlInfo.Visible = true;
                }
            }

        }
        private int ObtieneClaveEmple(string esquema, string conn,int icodUsuario)
        {
            int iCodEmple = 0;
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT E.iCodCatalogo FROM "+ esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E");
            query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('Usuar','Usuarios','Español')] AS U");
            query.AppendLine(" ON E.Usuar = U.iCodCatalogo");
            query.AppendLine(" AND U.dtIniVigencia <> U.dtFinVigencia");
            query.AppendLine(" AND U.dtFinVigencia >=GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND U.iCodCatalogo = "+icodUsuario+" ");
            DataTable dt = DSODataAccess.Execute(query.ToString(),conn);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                iCodEmple = Convert.ToInt32(dr["iCodCatalogo"]);
            }
            return iCodEmple;
        }
        private void ObtieneSol(string esquema, string con,ref List<Solicitud> lista,int emple)
        {
            string sp = "EXEC [WorkflowLineasGetDatosSolicitudes] @esquema = '{0}', @Opcion = 5,@IcodCatEmple = {1}";
            string query = string.Format(sp, esquema, emple);
            DataTable dt = DSODataAccess.Execute(query, con);
            if(dt != null && dt.Rows.Count > 0)
            {
                lista.Clear();

                foreach (DataRow dr in dt.Rows)
                {
                    Solicitud sol = new Solicitud();
                    sol.IdSol = Convert.ToInt32(dr["SolicitudId"]);
                    sol.NomSol = dr["SolicitudId"].ToString() + " - " + dr["TipoRecursoDesc"].ToString();
                    lista.Add(sol);
                }
            }
        }
        private void GetDatosSolicitud(string esquema, string con, int solicitud)
        {
            string sp = "EXEC [WorkflowLineasGetDatosSolicitudes] @esquema = '{0}', @Opcion = 1, @IdSolicitud = {1}";
            string query = string.Format(sp, esquema, solicitud);
            DataTable dt = DSODataAccess.Execute(query, con);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                txtSolicitud.Text = dr["SolicitudId"].ToString();
                txtPerfil.Text = dr["PerfilDesc"].ToString();
                txtPlan.Text = dr["PlanTarifarioDesc"].ToString();
                txtTipoRecurso.Text = dr["TipoRecursoDesc"].ToString().ToUpper();
                row1.Visible = true;
            }
        }
        private void ObtieneEstatusSolicitud(string esquema, string con, int solicitud)
        {
            rowLineas.Visible = true;
            int estatus = ObtieneEstatus(esquema, con, solicitud);
            if (estatus > 0)
            {
                rowIconos.Visible = true;
                CreaTablaEstatus(estatus);
            }
        }
        private int ObtieneEstatus(string esquema, string con, int solicitud)
        {
            int estatus = 0;

            string sp = "EXEC WorkFlowLineasObtieneEstatusSol @Esquema = '{0}', @Solicitud = {1}";
            string query = string.Format(sp, esquema, solicitud);
            DataTable dt = DSODataAccess.Execute(query, con);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string e = string.IsNullOrEmpty(dr["Estatus"].ToString()) ? "0" : dr["Estatus"].ToString();
                estatus = Convert.ToInt32(e);
            }

            return estatus;
        }
        private void CreaTablaEstatus(int estatus)
        {
            var lista = ListaSolicitudes();
            int contador = 0;
            StringBuilder html = new StringBuilder();
            html.AppendLine(" <table class= 'table table-bordered tableDashboard'>");
            html.AppendLine(" <thead class= 'tableHeaderStyle'>");
            html.AppendLine(" <tr>");
            html.AppendLine(" <th style='font-size:14px;'>Estatus Solicitud</th>");
            html.AppendLine(" <th style='font-size:14px;'>Estado</th>");
            html.AppendLine(" </tr>");
            html.AppendLine(" </thead>");
            html.AppendLine(" <tbody>");

            foreach (var item in lista)
            {
                int e = item.NumEstatus;
                string est = item.Estatus;
                html.AppendLine(" <tr>");
                html.AppendLine(" <td style='font-size:13px;'>" + est + "</td>");

                if (e == estatus)
                {
                    html.AppendLine(" <td style='width: 20px; text-align:center; font-size:20px;'><i class='fas fa-arrow-alt-circle-right'></i></td>");
                }
                else
                {
                    if( e < estatus)
                    {
                        html.AppendLine("<td style='width: 20px; text-align:center;font-size:20px;'><i class='fas fa-check-circle'></i></td>");
                    }
                    else
                    {
                        html.AppendLine("<td style='width: 20px; text-align:center;font-size:20px;'>&nbsp;</td>");
                    }                    
                }

                html.AppendLine(" </tr>");
                contador++;
            }
            html.AppendLine(" </tbody>");
            html.AppendLine(" </table>");

            divLineas.InnerHtml = html.ToString();
        }
        private List<EstatusSolicitud> ListaSolicitudes()
        {
            List<EstatusSolicitud> list = new List<EstatusSolicitud>();
            list.Add(new EstatusSolicitud() { Estatus = "Solicitud Registrada", NumEstatus = 0 });
            list.Add(new EstatusSolicitud() { Estatus = "En espera de Autorización por el Director", NumEstatus = 1});
            list.Add(new EstatusSolicitud() { Estatus = "Autorizada por Director", NumEstatus = 2 });
            list.Add(new EstatusSolicitud() { Estatus = "En espera de Autorización por RH", NumEstatus = 3 });
            list.Add(new EstatusSolicitud() { Estatus = "Autorizada por RH", NumEstatus = 4 });
            list.Add(new EstatusSolicitud() { Estatus = "En espera de Autorización por Servicios Generales", NumEstatus = 5 });
            list.Add(new EstatusSolicitud() { Estatus = "En espera de asignacíon de Recurso", NumEstatus = 6 });
            list.Add(new EstatusSolicitud() { Estatus = "Recurso Asignado", NumEstatus = 7 });
            list.Add(new EstatusSolicitud() { Estatus = "Documentos cargados", NumEstatus = 8 });
            list.Add(new EstatusSolicitud() { Estatus = "En espera de envio carta original", NumEstatus = 9 });

            var lista = list.OrderBy(x => x.NumEstatus).ToList();
            return lista;
        }
        #endregion METODOS

        protected void cboSolicitudes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idSolicitud = Convert.ToInt32(cboSolicitudes.SelectedValue);
            if (idSolicitud > 0)
            {
                GetDatosSolicitud(esquema, connStr, idSolicitud);
                ObtieneEstatusSolicitud(esquema, connStr, idSolicitud);
            }
            else
            {
                rowLineas.Visible = false;
                rowIconos.Visible = false;
                row1.Visible = false;
            }
        }
    }
    public class Solicitud
    {
       public int IdSol { get; set; }
       public string  NomSol {set; get;}
    }
    public class EstatusSolicitud
    {
        public string Estatus { get; set; }
        public int NumEstatus { get; set; }
    }
}