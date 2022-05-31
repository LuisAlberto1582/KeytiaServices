using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class FinalizaSolicitudes : System.Web.UI.Page
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
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                ObtieneSolcitidudes(esquema, connStr);
            }
        }
        #region METODOS
        public void ObtieneSolcitidudes(string esquema, string con)
        {
            DataTable dt = new DataTable();
            dt = ObtieneSolicitud(esquema,con);
            if (dt != null && dt.Rows.Count > 0)
            {
                gridSolicitudes.DataSource = dt;
                gridSolicitudes.DataBind();
            }
            else
            {
                //MuestraMensaje("Mensaje!","No Existen Solicitudes por Finalizar!");
                rowGuardar.Visible = false;
                pnlInfo.Visible = true;
                lblMensajeInfo.Text = "No Existen Solicitudes por Finalizar!";
            }

        }
        public void MuestraMensaje(string mensajeHeader, string mensajeBody)
        {
            lblTituloModalMsn.Text = mensajeHeader;
            lblBodyModalMsn.Text = mensajeBody;
            mpeEtqMsn.Show();
        }
        public void ValidaSelectedCheckBox()
        {
            int result = 0;
            try
            {
                int contador = 0;
                foreach (GridViewRow gvRow in gridSolicitudes.Rows)
                {
                    try
                    {
                        var checkbox = gvRow.FindControl("chkRow") as CheckBox;
                        if (checkbox.Checked)
                        {
                            var lblID = gvRow.FindControl("lblIdSolitud") as Label;
                            int solicitud = Convert.ToInt32(lblID.Text);
                            ActualizaSolicitud(esquema, solicitud, connStr);

                            contador++;
                        }

                        result ++;
                    }
                    catch(Exception ex)
                    {
                        result = 0;
                        throw ex;
                    }
                }
                if (contador == 0)
                {
                    MuestraMensaje("Mensaje!", "Debe de Seleccionar al Menos Una Solicitud!");
                }
                else if(contador > 0 && result > 0)
                {
                    MuestraMensaje("Mensaje!", "Se han Finalizado Correctamente las Solicitudes Seleccionadas!");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        #endregion METODOS
        #region Querys
        public DataTable ObtieneSolicitud(string esquema,string con)
        {
            DataTable dt = new DataTable();
            //string sp = "EXEC [WorkflowLineasGetDatosSolicitudes] @Esquema = '{0}', @Opcion = 4,@EstatusSolicitud = 'Aceptada'";
            string sp = "EXEC WorkFlowLineasGetFinalizaSol @Esquema = '{0}'";
            string query = string.Format(sp, esquema);
            dt = DSODataAccess.Execute(query, con);
            return dt;
        }
        public void ActualizaSolicitud(string esquema,int solicitud,string con)
        {
            string sp = "EXEC WorkFlowLineasActualizaSolCartaFisica @Esquema = '{0}', @IdSolicitud = {1}";
            string query = string.Format(sp, esquema, solicitud);
            DSODataAccess.ExecuteNonQuery(query, con);
        }
        #endregion Querys

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            ValidaSelectedCheckBox();
            gridSolicitudes.DataSource = null;
            gridSolicitudes.DataBind();
            ObtieneSolcitidudes(esquema, connStr);
        }

        protected void lnkDownloadINE_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Clear();
                LinkButton lnkbtn = sender as LinkButton;
                GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;
                string filePath = gridSolicitudes.DataKeys[gvrow.RowIndex].Values[0].ToString();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(filePath) + "\"");
                Response.TransmitFile(filePath);
                Response.End();
            }
            catch(ThreadAbortException ex)
            {
                throw ex;
            }
            
        }

        protected void lnkDownloadCarta_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Clear();
                LinkButton lnkbtn1 = sender as LinkButton;
                GridViewRow gvrow1 = lnkbtn1.NamingContainer as GridViewRow;
                string filePath = gridSolicitudes.DataKeys[gvrow1.RowIndex].Values[1].ToString();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(filePath) + "\"");
                Response.TransmitFile(filePath);
                Response.End();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}