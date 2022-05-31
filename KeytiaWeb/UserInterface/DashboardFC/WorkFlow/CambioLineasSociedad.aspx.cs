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
    public partial class CambioLineasSociedad : System.Web.UI.Page
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
                IniciaProceso();
            }
        }
        #region METODOS
        private void IniciaProceso()
        {
            LlenaEmpresa(1);
            LlenaPlan(1);
        }
        private void LlenaPlan(int opcion)
        {
            DataTable dt = ObtienePlan();
            if (dt != null && dt.Rows.Count > 0)
            {
                if(opcion == 1)
                {
                    cboPlan.DataSource = dt;
                    cboPlan.DataBind();
                }
                else if(opcion == 2)
                {
                    cboPlan2.Items.Clear();
                    cboPlan2.DataSource = null;
                    cboPlan2.DataBind();
                    cboPlan2.Items.Insert(0, "Seleccione un Plan");
                    cboPlan2.SelectedIndex = 0;

                    cboPlan2.DataSource = dt;
                    cboPlan2.DataBind();
                }
               
            }
        }
        private void LlenaEmpresa(int opcion)
        {
            DataTable dt = ObtieneSociedad();
            if(dt != null && dt.Rows.Count > 0)
            {
                if(opcion == 1)
                {
                    cboSociedad.DataSource = dt;
                    cboSociedad.DataBind();
                }
                else if(opcion == 2)
                {
                    cboSociedad2.Items.Clear();
                    cboSociedad2.DataSource = null;
                    cboSociedad2.DataBind();
                    cboSociedad2.Items.Insert(0, "Seleccione una Sociedad");
                    cboSociedad2.SelectedIndex = 0;

                    cboSociedad2.DataSource = dt;
                    cboSociedad2.DataBind();
                }
            }
        }
        private void ValidaSelectedCheckBox()
        {
            int result = 0;
            try
            {
                int contador = 0;
                foreach (GridViewRow gvRow in gvLineas.Rows)
                {
                    try
                    {
                        var checkbox = gvRow.FindControl("chkRow") as CheckBox;
                        if (checkbox.Checked)
                        {
                            var lblTel = gvRow.FindControl("lblTelefono") as Label;
                            string lblTelefono = lblTel.Text.ToString();
                            CambioLineaSociedad(lblTelefono, connStr, esquema);
                            contador++;
                        }

                        result++;
                    }
                    catch (Exception ex)
                    {
                        result = 0;
                        return;
                        throw ex;

                    }
                }
                if (contador == 0)
                {
                    lblTituloModalMsn.Text = "Mensaje!";
                    lblBodyModalMsn.Text = "Debe Seleccionar al Menos Una Línea!";
                    mpeEtqMsn.Show();
                    return;
                }
                else if (contador > 0 && result > 0)
                {
                    lblTituloModalMsn.Text = "Mensaje!";
                    lblBodyModalMsn.Text = "Las Líneas se Cambiaron Correctamente!";
                    mpeEtqMsn.Show();
                    /*Metodo que recarga la pagina despues de Cambiar las Lineas*/
                    ReloadPage();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void CambioLineaSociedad(string tel,string con, string esquema)
        {
            int empresa = Convert.ToInt32(cboSociedad2.SelectedValue);
            int plan = Convert.ToInt32(cboPlan2.SelectedValue);

            ActualizaSociedadLineas(tel, con, esquema, empresa, plan);
        }
        private void ReloadPage()
        {
            row3.Visible = false;
            gvLineas.DataSource = null;
            gvLineas.DataBind();
            cboSociedad.SelectedIndex = 0;
            cboPlan.SelectedIndex = 0;
        }

        #endregion METODOS
        #region CONSULTAS
        private DataTable ObtieneSociedad()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @IcodCatalogo INT");
            query.AppendLine(" SELECT @IcodCatalogo = iCodCatalogo FROM "+ DSODataContext.Schema + ".[vishistoricos('CenCos','Centro de costos','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND CenCos IS NULL");
            query.AppendLine(" SELECT iCodCatalogo,Descripcion FROM " + DSODataContext.Schema + ".[vishistoricos('CenCos','Centro de costos','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND CenCos = @IcodCatalogo AND Descripcion <> 'POR IDENTIFICAR' AND Descripcion <> 'DIR GRAL. SEGUROS'");
            DataTable dt = new DataTable();
            return dt = DSODataAccess.Execute(query.ToString(), connStr);
        }
        private DataTable ObtienePlan()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" PLANT.iCodCatalogo,");
            query.AppendLine(" PERFIL.Id,");
            query.AppendLine(" PLANT.vchDescripcion + ' - ' + PERFIL.Descripcion AS Descripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('PlanTarif','Plan Tarifario','Español')] AS PLANT");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".WorkflowLineasRelacionPerfilPlanTarif AS  REL");
            query.AppendLine(" ON PLANT.iCodCatalogo = REL.PlanTarifId");
            query.AppendLine(" AND REL.dtIniVigencia <> REL.dtFinVigencia");
            query.AppendLine(" AND REL.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".WorkflowLineasPerfil AS PERFIL");
            query.AppendLine(" ON REL.PerfilId = PERFIL.Id");
            query.AppendLine(" AND PERFIL.dtIniVigencia <> PERFIL.dtFinVigencia");
            query.AppendLine(" AND PERFIL.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".WorkflowLineasTipoRecurso AS TPR");
            query.AppendLine(" ON REL.TipoRecursoId = TPR.Id");
            query.AppendLine(" WHERE PLANT.dtIniVigencia <> PLANT.dtFinVigencia");
            query.AppendLine(" AND PLANT.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND TPR.Clave = 'Celular'");

            DataTable dt = new DataTable();
            return dt = DSODataAccess.Execute(query.ToString(), connStr);
        }
        private DataTable ObtieneLineasEmpresa()
        {
            string sp = " EXEC[WorkFlowLineasGetDispositivos] @Esquema = '{0}', @ClavePerfilId = {1}, @iCodCatSociedad = {2}";
            int empresa = Convert.ToInt32(cboSociedad.SelectedValue);
            int plan = Convert.ToInt32(cboPlan.SelectedValue);
            string query = string.Format(sp, DSODataContext.Schema, plan, empresa);

            DataTable dt = new DataTable();
            return dt = DSODataAccess.Execute(query.ToString(), connStr);
        }
        private void ActualizaSociedadLineas(string tel, string con, string esquema,int sociedad, int plan)
        {
            string sp = "EXEC [WorkFlowLineasCambiaPlanLinea]  @Esquema = '{0}',@Linea = {1},@Sociedad = {2},@PlanTarif = {3}";
            string query = string.Format(sp, esquema, tel, sociedad, plan);
            DSODataAccess.ExecuteNonQuery(query,con);
        }
        #endregion CONSULTAS
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            int empresa = Convert.ToInt32(cboSociedad.SelectedValue);
            if (empresa > 0)
            {
                DataTable dt = ObtieneLineasEmpresa();
                if (dt != null && dt.Rows.Count > 0)
                {
                    int rows = dt.Rows.Count;
                    if (rows > 7)
                    {
                        divGrid.Style.Value = "height:200px;overflow-y:auto;overflow-x:auto;";
                    }
                    else
                    {
                        divGrid.Style.Value = "height:auto;overflow-y:auto;overflow-x:auto;";
                    }
                    row3.Visible = true;
                    gvLineas.DataSource = dt;
                    gvLineas.DataBind();

                    LlenaEmpresa(2);
                    LlenaPlan(2);
                    pnlInfo.Visible = false;
                    lblMensajeInfo.Text = "";
                }
                else
                {
                    pnlInfo.Visible = true;
                    lblMensajeInfo.Text = "No Existen Líneas Disponibles!";
                    row3.Visible = false;
                    gvLineas.DataSource = null;
                    gvLineas.DataBind();
                }
            }
            else
            {
                lblTituloModalMsn.Text = "Mensaje!";
                lblBodyModalMsn.Text = "Debe Seleccionar una Empresa!";
                mpeEtqMsn.Show();
                return;
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int empresa = Convert.ToInt32(cboSociedad2.SelectedValue.Replace("Seleccione una Sociedad", "0"));
                int plan = Convert.ToInt32(cboPlan2.SelectedValue.Replace("Seleccione un Plan", "0"));
                if (empresa > 0)
                {
                    if (plan > 0)
                    {
                        ValidaSelectedCheckBox();
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
                    lblBodyModalMsn.Text = "Debe Seleccionar una Sociedad!";
                    mpeEtqMsn.Show();
                    return;
                }
            }
            catch(Exception ex)
            {

                return;
                throw ex;
            }
        }
    }
}