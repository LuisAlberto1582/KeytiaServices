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

namespace KeytiaWeb.UserInterface.Administracion.ETL
{
    public partial class ListadoCargasETLClaroBrasil : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static int icodCatReg;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;

            if (!Page.IsPostBack)
            {
                ObtieneCargas();
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Administracion/ETL/AltaETLFacturaClaroBrasil.aspx", false);
        }
        private void ObtieneCargas(string filtro = "")
        {          
            DataTable dt = new DataTable();
            dt.Columns.Add("iCodCatalogo");
            dt.Columns.Add("vchDescripcion");
            dt.Columns.Add("AnioDesc");
            dt.Columns.Add("MesDesc");
            dt.Columns.Add("EstCargaDesc");
            dt.Columns.Add("Archivo01");
            dt.Rows.Add();
            if (dt != null)
            {
                rowGrv.Visible = true;
                InfoPanelSucces.Visible = false;
                grvListadoCargas.DataSource = dt;
                grvListadoCargas.DataBind();
            }
            else
            {
                lblMensajeSuccess.Text = "No existen Cargas Actualmente";
                InfoPanelSucces.Visible = true;
                rowGrv.Visible = false;
            }
            txtBuscar.Text = filtro;
        }
        private void EliminaCargaETL(int icodCatReg)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @icodCarga INT");
            query.AppendLine(" SELECT @icodCarga = icodCatalogo FROM " + esquema + ".[VisHistoricos('EstCarga','Estatus Cargas','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchCodigo = 'ETLEsperandoEliminacion'");
            query.AppendLine(" UPDATE A");
            query.AppendLine(" SET EstCarga = @icodCarga, dtFecUltAct = GETDATE()");
            query.AppendLine(" FROM " + esquema + ".[vishistoricos('Cargas','ETL Factura ClaroBrasilCel v3','Español')] AS A");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND iCodCatalogo = " + icodCatReg + "");
            try
            {
                DSODataAccess.ExecuteNonQuery(query.ToString(), connStr);
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar Eliminar la Carga. '", ex);
            }
        }
        protected void btnSi_Click(object sender, EventArgs e)
        {
            icodCatReg = Convert.ToInt32(claveCarga.Value);
            EliminaCargaETL(icodCatReg);
        }

        [WebMethod]
        public static string GetCustomers(string searchTerm)
        {
            return GetData(searchTerm).GetXml();
        }

        private static DataSet GetData(string searchTerm)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT");
            query.AppendLine("iCodCatalogo,");
            query.AppendLine("REPLACE(vchDescripcion,' ','') AS vchDescripcion,");
            query.AppendLine("MesDesc,");
            query.AppendLine("AnioDesc,");
            query.AppendLine("EstCargaDesc,");
            query.AppendLine("Archivo01");
            query.AppendLine("FROM " + esquema + ".[vishistoricos('Cargas','ETL Factura ClaroBrasilCel v3','Español')] WITH(NOLOCK)");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            if (searchTerm != "")
            {
                query.AppendLine("AND (vchdescripcion + aniodesc + mesdesc +Archivo01) LIKE '%" + searchTerm + "%' ");
            }
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            using (DataSet ds = new DataSet())
            {
                ds.Tables.Add(dt);
                return ds;
            }

        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            icodCatReg = Convert.ToInt32(claveCarga.Value);
            string carga = descCarga.Value.ToString();
            lblTituloModalMsn.Text = "¡Atención!";
            lblBodyModalMsn.Text = "¿Esta Seguro que desea eliminar la carga: " + carga + "?";
            mpeEtqMsn.Show();
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {

        }
    }
}