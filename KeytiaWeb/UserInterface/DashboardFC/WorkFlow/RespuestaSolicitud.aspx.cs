using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class RespuestaSolicitud : System.Web.UI.Page
    {
        int solicitudId = 0;
        int nivel = 0;
        string respuesta = "";
        int bitacoraId = 0;
        DataTable dtSolicitud = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            try
            {
                GetParametrosQueryString();
                if (solicitudId == 0 || nivel == 0 || string.IsNullOrEmpty(respuesta) || bitacoraId == 0)
                {
                    throw new ArgumentException("Error en los datos de la solicitud.");
                }

                dtSolicitud = GetDatosSolicitud();

                if (!IsPostBack)
                {
                    
                    if (dtSolicitud != null)
                    {
                        ProcesarInfoAVisualizar();
                    }
                    else { throw new ArgumentException("No se encontró la solicitud."); }
                }
            }
            catch (ArgumentException ex)
            {
                lblMensajeDanger.Text = ex.Message;
                pnlError.Visible = true;
                pnlDatosSolicitud.Visible = false;
            }
            catch (Exception)
            {
                lblMensajeDanger.Text = "Error al procesar la solicitud.";
                pnlError.Visible = true;
                pnlDatosSolicitud.Visible = false;
            }
        }

        private void GetParametrosQueryString()
        {
            try
            {
                solicitudId = Request.QueryString["s"] != null ? Convert.ToInt32(KeytiaServiceBL.Util.Decrypt(Request.QueryString["s"].ToString(), false)) : 0;
                respuesta = Request.QueryString["r"] != null ? KeytiaServiceBL.Util.Decrypt(Request.QueryString["r"].ToString(), false) : string.Empty;
                nivel = Request.QueryString["n"] != null ? Convert.ToInt32(KeytiaServiceBL.Util.Decrypt(Request.QueryString["n"].ToString(), false)) : 0;
                bitacoraId = Request.QueryString["b"] != null ? Convert.ToInt32(KeytiaServiceBL.Util.Decrypt(Request.QueryString["b"].ToString(), false)) : 0;
            }
            catch (Exception)
            {
            }
        }

        private void ProcesarInfoAVisualizar()
        {
            try
            {
                DataRow estatusBitacora = null;

                DataRow solicitud = dtSolicitud.Rows[0];
                //LA SOLICITUD YA FUE ACEPTADA O RECHAZADA, POR LO TANTO YA NO PROCEDE.
                if (solicitud["EstatusSolicitudCod"].ToString().ToLower() == "aceptada" || solicitud["EstatusSolicitudCod"].ToString().ToLower() == "rechazada")
                {
                    lblMensajeInfo.Text = "La solicitud ha sido previamente " + solicitud["EstatusSolicitudCod"].ToString();
                    pnlInfo.Visible = true;
                    pnlDatosSolicitud.Visible = false;
                    return;
                }
                else if (Convert.ToInt32(solicitud["SolicitudFinalizada"]) == 1)
                {
                    lblMensajeInfo.Text = "La solicitud ha sido previamente procesada.";
                    pnlInfo.Visible = true;
                    pnlDatosSolicitud.Visible = false;
                    return;
                }
                else if (solicitud["EstatusSolicitudCod"].ToString().ToLower() == "enproceso" &&
                    (estatusBitacora = GetEstatusBitacora()) != null && 
                    estatusBitacora["EstatusEnvioCod"].ToString().ToLower() != "enviado")  //Validar si a solicitud ya fue aceptada por el usuario sin estar totalmente aceptada.
                {       
                    //NZ: Este camino revisa que la solicitud no vaya a ser aceptada,rechazada o se que quiera cambiar la respuesta emitada la primera vez.             
                    if (estatusBitacora != null )
                    {
                        lblMensajeInfo.Text = "Ya se ha emitido previamente una respuesta para esta solicitud.";
                        pnlInfo.Visible = true;
                        pnlDatosSolicitud.Visible = false;
                        return;
                    }
                }
                else if (solicitud["EstatusSolicitudCod"].ToString().ToLower() == "expirada")
                {
                    lblMensajeInfo.Text = "La solicitud ha expirado";
                    pnlInfo.Visible = true;
                    pnlDatosSolicitud.Visible = false;
                    return;
                }
                else if (solicitud["EstatusSolicitudCod"].ToString().ToLower() == "enproceso" && !string.IsNullOrEmpty(respuesta))   //SI ESTE PARAMETRO VIENE LLENO, SE ESTA ACEPTANDO O RECHAZANDO UNA SOLICITUD.
                {
                    if (respuesta.ToLower() == "rechazada")
                    {
                        MostrarDatosSolicitud();
                    }
                    else if (respuesta.ToLower() == "aceptada")
                    {
                        AutorizarSolicitud();
                    }
                    else { throw new ArgumentException("Error en los datos de la solicitud."); }
                }
                else { throw new ArgumentException("Error en los datos de la solicitud."); }
            }
            catch (ArgumentException ex)
            {
                lblMensajeDanger.Text = ex.Message;
                pnlError.Visible = true;
                pnlDatosSolicitud.Visible = false;
            }
            catch (Exception)
            {
                lblMensajeDanger.Text = "Error en los datos de la solicitud.";
                pnlError.Visible = true;
                pnlDatosSolicitud.Visible = false;

            }

            
        }

        private void RechazarSolicitud()
        {
            try
            {
                UpdateBitacora("[" + txtMotivoRechazo.Text.ToUpper().Replace("DROP", "").Replace("DELETE", "").Replace("INSERT", "").Replace("UPDATE", "") + "]");
                lblMensajeSuccess.Text = "La solicitud de " + dtSolicitud.Rows[0]["SolicitanteNomCompleto"].ToString() + " ha sido " + respuesta.ToUpper();
                InfoPanelSucces.Visible = true;
                pnlDatosSolicitud.Visible = false;
            }
            catch (Exception)
            {
                throw new ArgumentException("No fue posible procesar el rechazo de la solicitud.");
            }
        }

        private void AutorizarSolicitud()
        {
            try
            {
                UpdateBitacora("");
                lblMensajeSuccess.Text = "La solicitud de " + dtSolicitud.Rows[0]["SolicitanteNomCompleto"].ToString() + " ha sido " + respuesta.ToUpper();
                InfoPanelSucces.Visible = true;
                pnlDatosSolicitud.Visible = false;
            }
            catch (Exception)
            {
                throw new ArgumentException("No fue posible procesar la autorización de la solicitud.");
            }

        }

        private void MostrarDatosSolicitud()
        {
            pnlDatosSolicitud.Visible = true;
            lblFolio.Text = dtSolicitud.Rows[0]["SolicitudId"].ToString();
            lblNominaEmple.Text = dtSolicitud.Rows[0]["SolicitanteNomina"].ToString();
            lblNombre.Text = dtSolicitud.Rows[0]["SolicitanteNomCompleto"].ToString();
            lblTipoRecurso.Text = dtSolicitud.Rows[0]["TipoRecursoDesc"].ToString();
        }

        protected void btnRechazar_Click(object sender, EventArgs e)
        {
            if (solicitudId != 0 && bitacoraId != 0)
            {
                if (!string.IsNullOrEmpty(txtMotivoRechazo.Text))
                {
                    RechazarSolicitud();
                }
                else
                {
                    lblMensajeDanger.Text = "Favor de especificar un motivo de rechazo.";
                    pnlError.Visible = true;
                    pnlDatosSolicitud.Visible = true;
                }
            }
            else
            {
                lblMensajeDanger.Text = "Error en los datos de la solicitud.";
                pnlError.Visible = true;
                pnlDatosSolicitud.Visible = false;
                Session["HomePage"] = null;
            }

            
        }

        private DataTable GetDatosSolicitud()
        {
            return DSODataAccess.Execute("EXEC [WorkflowLineasGetDatosSolicitudes] @esquema='" + DSODataContext.Schema + "', @Opcion = 1, @IdSolicitud = " + solicitudId);
        }

        private DataRow GetEstatusBitacora()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT B.Id AS BitacoraId, B.EstatusEnviosId AS EstatusId, EE.Clave AS EstatusEnvioCod, EE.Descripcion AS EstatusEnvioDesc");
            query.AppendLine("FROM " + DSODataContext.Schema + ".WorkflowLineasBitacoraEnvios B");
            query.AppendLine("	JOIN " + DSODataContext.Schema + ".WorkflowLineasEstatusEnvios EE");
            query.AppendLine("		ON EE.Id = B.EstatusEnviosId");
            query.AppendLine("		AND EE.dtIniVigencia <> EE.dtFinVigencia");
            query.AppendLine("		AND EE.dtFinVigencia >= GETDATE()");
            query.AppendLine("WHERE B.dtIniVigencia <> B.dtFinVigencia");
            query.AppendLine("  AND B.dtFinVigencia >= GETDATE()");
            query.AppendLine("  AND B.Id = " + bitacoraId);
            query.AppendLine("  AND SolicitudLineaMovilId = " + solicitudId);

            var dtResult = DSODataAccess.Execute(query.ToString());
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                return dtResult.Rows[0];
            }
            else { return null; }
        }

        private void UpdateBitacora(string motivo)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [WorkflowLineasRespondeSolicitud]");
            query.AppendLine("  @Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @respuesta = '" + respuesta + "',");
            query.AppendLine("  @regBitacora = " + bitacoraId + ",");
            query.AppendLine("  @SolicitudId = " + solicitudId + ",");
            query.AppendLine("  @lvlResp = " + nivel + ",");
            query.AppendLine("  @comentarios = '" + motivo + "'");

            DSODataAccess.Execute(query.ToString());
        }
      
    }
}