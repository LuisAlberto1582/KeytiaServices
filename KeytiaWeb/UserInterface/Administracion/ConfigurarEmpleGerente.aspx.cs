using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Administracion
{
    public partial class ConfigurarEmpleGerente : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
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
                ObtieneEmplesGerentes();
            }
        }
        private void ObtieneEmplesGerentes()
        {
            DataTable dt = DSODataAccess.Execute(QueryEmplesGerentes());
            if (dt != null && dt.Rows.Count > 0)
            {
                rowBusqueda.Visible = true;
                rowGrid.Visible = true;
                rowBoton.Visible = true;

                grvEmples.DataSource = dt;
                grvEmples.DataBind();
                grvEmples.UseAccessibleHeader = true;
                grvEmples.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            else
            {
                rowBusqueda.Visible = false;
                rowGrid.Visible = false;
                rowBoton.Visible = false;
            }
        }
        private string QueryEmplesGerentes()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" E.iCodCatalogo AS icodEmple,");
            query.AppendLine(" NominaA,");
            query.AppendLine(" Nomcompleto,");
            query.AppendLine(" P.vchDescripcion");
            query.AppendLine(" FROM " + esquema + ".HISTEMPLE AS E WITH(NOLOCK)");
            query.AppendLine(" LEFT JOIN " + esquema + ".[VisHistoricos('Puesto','Puestos Empleado','Español')] AS P WITH(NOLOCK)");
            query.AppendLine(" ON E.Puesto = p.icodCatalogo");
            query.AppendLine(" AND P.dtIniVigencia <> P.dtFinVigencia");
            query.AppendLine(" AND P.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND(ISNULL(banderasEmple,0) & 2)/2 = 1");
            return query.ToString();

        }
        private void CambiaEmpleGerente()
        {

        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            int cantRows = grvEmples.Rows.Count;

            for (int i = 0; i <= cantRows; i++)
            {
                var t = i;

                var checkbox = grvEmples.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {

                }
            }
        }
    }
}