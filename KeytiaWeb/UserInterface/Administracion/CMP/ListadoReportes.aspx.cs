using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Administracion.CMP
{
    public partial class ListadoReportes : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
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
                LlenarTablaDeReportes();
            }
        }

        private void LlenarTablaDeReportes()
        {
            DataTable dt = ObtenerCargasDeReportes();

            grvCargas.DataSource = null;
            grvCargas.DataSource = dt;
            grvCargas.DataBind();
        }

        private DataTable ObtenerCargasDeReportes()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT");
            query.AppendLine("vchCodigo AS [Fecha Solicitud], ");
            query.AppendLine("EstCargaDesc AS [Estatus Carga], ");
            query.AppendLine("MesDesc AS Mes, ");
            query.AppendLine("AnioDesc AS [Año], ");
            query.AppendLine("UsuarCod AS Usuario, ");
            query.AppendLine("ListaCenCos AS [Listado de Centros de Costos], ");
            query.AppendLine("ISNULL(CtaCC,'') AS [Correo(s) Copia  Rep. Ind.], ");
            query.AppendLine("ISNULL(CtaCCO,'') AS [Correo(s) Copia Oculta Rep. Ind. ], ");
            query.AppendLine("ISNULL(CtaPara,'') AS [Correo(s) Destinatario(s) Rep. Gral], ");
            query.AppendLine("ISNULL(CorreoElectronicoCC,'') AS [Correo(s) Copia Oculta Rep. Gral] ");

            query.AppendLine("FROM " + esquema + ".[vishistoricos('ReporteAutomatizado','Envío reportes para directivos v1','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia  ");
            query.AppendLine("AND dtFinVigencia >= GETDATE() ");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UserInterface/Administracion/CMP/AltaCMPReporte.aspx", false);
        }
    }
}