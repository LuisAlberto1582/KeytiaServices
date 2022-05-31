using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.Templates
{
    public partial class AsistenciaUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string query = "SELECT [Español] as TextoHtml " +
"from bimbo.[visHistoricos('MsgWeb','Mensajes Web','Español')] " +
"where dtfinvigencia>=getdate() " + " and vchcodigo = 'BimboAsistReporteConsumoPorDepto'";
            string resultado = KeytiaServiceBL.DSODataAccess.ExecuteScalar(query).ToString();
            divCenter.InnerHtml = resultado;
        }

    }
}