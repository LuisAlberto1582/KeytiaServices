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
    public partial class Autorizadores1 : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
        }
        [WebMethod]
        public static object GetCencos(string texto)
        {
            DataTable CenCosto = new DataTable();
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo, vchCodigo+'-'+Descripcion AS vchDescripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".HistCencos WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchcodigo + vchDescripcion + Descripcion LIKE '%" + texto + "%'");

            CenCosto = DSODataAccess.Execute(query.ToString(), connStr);
            DataView dvldt = new DataView(CenCosto);
            CenCosto = dvldt.ToTable(false, new string[] { "iCodCatalogo", "vchDescripcion" });
            CenCosto.Columns["iCodCatalogo"].ColumnName = "idCencos";
            CenCosto.Columns["vchDescripcion"].ColumnName = "Descripcion";
            return FCAndControls.ConvertDataTabletoJSONString(CenCosto);
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string cencos = (txtCencosId.Text != "") ? txtCencosId.Text.ToString() : "";
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            txtAut1.Enabled = true;
            txtAut2.Enabled = true;
            txtAut3.Enabled = true;
            btnEditar.Visible = false;
            btnGuardar.Visible = true;
            
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            btnEditar.Visible = true;
            btnGuardar.Visible = false;
        }
    }
}