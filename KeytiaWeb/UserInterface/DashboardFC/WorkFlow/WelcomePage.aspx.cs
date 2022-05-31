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
    public partial class WelcomePage : System.Web.UI.Page
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
            int iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            ObtieneNomEmpleado(iCodUsuario, esquema, connStr);

        }
        private void ObtieneNomEmpleado(int usuario,string esquema,string con)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" E.NomCompleto AS NOMBRE");
            query.AppendLine(" FROM "+ esquema + ".[VisHistoricos('Usuar','Usuarios','Español')] AS U");
            query.AppendLine(" JOIN "+ esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E");
            query.AppendLine(" ON U.iCodCatalogo = E.Usuar");
            query.AppendLine(" AND E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE U.dtIniVigencia <> U.dtFinVigencia");
            query.AppendLine(" AND U.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND U.iCodCatalogo ="+ usuario +" ");

            DataTable dt = DSODataAccess.Execute(query.ToString(), con);
            if(dt!= null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string nombre = dr["NOMBRE"].ToString();
                lblNomEmple.InnerText = nombre.ToUpper();
            }

        }
    }
}