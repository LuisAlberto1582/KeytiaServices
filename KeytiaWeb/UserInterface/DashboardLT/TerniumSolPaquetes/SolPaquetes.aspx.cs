using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.TerniumSolPaquetes
{
    public partial class SolPaquetes : System.Web.UI.Page
    {
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;

            if (!Page.IsPostBack)
            {

                iCodUsuario = Session["iCodUsuario"].ToString();
                iCodPerfil = Session["iCodPerfil"].ToString();
                ObtieneDatosEmple(Convert.ToInt32(iCodUsuario));
                #region Inicia los valores default de los controles de fecha y banderas de clientes y perfiles
                try
                {
                    if (Session["Language"].ToString() == "Español")
                    {
                        pdtInicio.setRegion("es");
                        pdtFin.setRegion("es");
                    }

                    pdtInicio.MaxDateTime = DateTime.Today;
                    pdtInicio.MinDateTime = new DateTime((DateTime.Now.Year), 1, 1);

                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }

                #endregion Inicia los valores default de los controles de fecha
            }
        }

        #region CONSULTAS
        public void ObtieneDatosEmple(int icodEmple)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" E.iCodCatalogo,");
            query.AppendLine(" E.NomCompleto,");
            query.AppendLine(" ISNULL(E.Email, '')  AS Email,");
            query.AppendLine(" C.Descripcion");
            query.AppendLine(" FROM "+ esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E");
            query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('Usuar','Usuarios','Español')]");
            query.AppendLine(" AS U");
            query.AppendLine(" ON E.Usuar = U.iCodCatalogo");
            query.AppendLine(" AND U.dtIniVigencia<> U.dtFinVigencia");
            query.AppendLine(" AND U.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN Ternium.[vishistoricos('CenCos','Centro de costos','Español')] AS C");
            query.AppendLine(" ON E.CenCos = C.iCodCatalogo");
            query.AppendLine(" AND C.dtIniVigencia <> C.dtFinVigencia");
            query.AppendLine(" AND C.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND U.iCodCatalogo = "+ icodEmple + "");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                hfIcodEmple.Value = dr["iCodCatalogo"].ToString();
                txtNombre.Text = dr["NomCompleto"].ToString();
                txtEmail.Text = dr["Email"].ToString();
                txtCencos.Text = dr["Descripcion"].ToString();
                int claveEmple = Convert.ToInt32(dr["iCodCatalogo"]);
                ObtieneLineasEmple(claveEmple);
            }

        }
        public void ObtieneLineasEmple(int icodEmpleado)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" L.iCodCatalogo,");
            query.AppendLine(" L.Tel + ' - ' + ISNULL(L.EqCelularDesc,'')  AS Linea");
            query.AppendLine(" FROM " + esquema + ".[VisHistoricos('Linea','Lineas','Español')] AS L");
            query.AppendLine(" JOIN " + esquema + ".[VisRelaciones('Empleado - Linea','Español')] AS R");
            query.AppendLine(" ON L.iCodCatalogo = R.Linea");
            query.AppendLine(" AND R.dtIniVigencia <> R.dtFinVigencia");
            query.AppendLine(" AND R.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE L.dtIniVigencia <> L.dtFinVigencia");
            query.AppendLine(" AND L.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND R.Emple = "+ icodEmpleado + "");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                cboLineas.DataSource = dt;
                cboLineas.DataBind();
            }
        }
    #endregion CONSULTAS

}
}