using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;
using System.Text;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class DashboardTop10CCustodia : System.Web.UI.Page
    {
        DataTable ldtReporte = new DataTable(null);
        StringBuilder sbQuery = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            string lsStyleSheet = (string)HttpContext.Current.Session["StyleSheet"];

            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/CCustodia.css") + "\" />"));

            generaReportesDeDashboard();

        }

        /// <summary>
        /// En este metodo se van agregando los reportes que se necesitan mostrar en el dashboard.
        /// </summary>
        protected void generaReportesDeDashboard()
        {
            grvCCustAceptadas.DataSource = FillDTReporte(Top10CCustAceptadas());
            grvCCustAceptadas.DataBind();

            grvCCustRechazadas.DataSource = FillDTReporte(Top10CCustRechazadas());
            grvCCustRechazadas.DataBind();

            grvCCustPendientes.DataSource = FillDTReporte(Top10CCustPendientes());
            grvCCustPendientes.DataBind();
        }

        /// <summary>
        /// Regresa un DataTable con la información del Reporte.
        /// </summary>
        /// <param name="lsQuery"></param>
        protected DataTable FillDTReporte(string lsQuery)
        {
            ldtReporte.Clear();
            ldtReporte = DSODataAccess.Execute(lsQuery);

            return ldtReporte;
        }

        /// <summary>
        /// Consulta de top 10 de cartas custodia aceptadas
        /// </summary>
        /// <returns>Regresa la consulta en un string</returns>
        protected string Top10CCustAceptadas()
        {
            sbQuery.Length = 0;

            sbQuery.AppendLine("SELECT TOP 10   ");
            sbQuery.AppendLine("    [Folio]				= CCustodia.[FolioCCustodia], ");
            
            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            sbQuery.AppendLine("    [Estatus]	       = EstCCust.vchDescripcion, ");
            //sbQuery.AppendLine("[Estatus]	       = CCustodia.[EstCCustodiaDesc], ");

            sbQuery.AppendLine("    [No. Nomina]		= Emple.[NominaA], ");
            sbQuery.AppendLine("    [Empleado]			= IsNull(left(Emple.NomCompleto,400),''), ");
            sbQuery.AppendLine("    [Ubicación]			= Emple.[Ubica], ");
            //20140613 AM. Se cambia campo de fecha
            //sbQuery.AppendLine("[Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.[FechaCreacion], 103), ");
            sbQuery.AppendLine("    [Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.dtFecUltAct, 103), ");
            sbQuery.AppendLine("    [icodregistro]      = ccustodia.icodregistro, ");
            sbQuery.AppendLine("    [Codigo Empleado]	= CCustodia.[Emple] ");
            sbQuery.AppendLine("FROM [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia ");
            sbQuery.AppendLine("join [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] Estatus ");
            sbQuery.AppendLine("    on CCustodia.EstCCustodia = Estatus.icodcatalogo ");
            sbQuery.AppendLine("    AND CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia ");
            sbQuery.AppendLine("    AND CCustodia.dtFinVigencia >= GETDATE() ");
            sbQuery.AppendLine("    AND Estatus.dtIniVigencia <> Estatus.dtFinVigencia  ");
            sbQuery.AppendLine("    AND Estatus.dtFinVigencia >= GETDATE() ");
            sbQuery.AppendLine("    join [VisHistoricos('Emple','Empleados','Español')] Emple  ");
            sbQuery.AppendLine("    ON CCustodia.[Emple] = Emple.[iCodCatalogo]  ");
            sbQuery.AppendLine("    AND Emple.dtIniVigencia <> Emple.dtFinVigencia  ");
            sbQuery.AppendLine("    AND Emple.dtFinVigencia >= GETDATE()  ");
            sbQuery.AppendLine("    AND Estatus.[value] = 2  --ACEPTADAS ");


            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            sbQuery.AppendLine("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust ");
            sbQuery.AppendLine("    on EstCCust.iCodCatalogo = CCustodia.EstCCustodia ");
            sbQuery.AppendLine("    and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE() ");

            //20140613 AM. Se cambia ordenamiento de consulta
            //sbQuery.AppendLine("ORDER BY [Fecha] desc ");
            sbQuery.AppendLine("ORDER BY CCustodia.dtFecUltAct desc ");

            return sbQuery.ToString();
        }

        /// <summary>
        /// Consulta de top 10 de cartas custodia rechazadas
        /// </summary>
        /// <returns>Regresa la consulta en un string</returns>
        protected string Top10CCustRechazadas()
        {
            sbQuery.Length = 0;

            sbQuery.AppendLine("SELECT TOP 10  ");
            sbQuery.AppendLine("    [Folio]				= CCustodia.[FolioCCustodia], ");

            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            sbQuery.AppendLine("    [Estatus]	       = EstCCust.vchDescripcion, ");
            //sbQuery.AppendLine("[Estatus]	       = CCustodia.[EstCCustodiaDesc], ");

            sbQuery.AppendLine("    [No. Nomina]		= Emple.[NominaA], ");
            sbQuery.AppendLine("    [Empleado]			= IsNull(left(Emple.NomCompleto,400),''), ");
            sbQuery.AppendLine("    [Ubicación]			= Emple.[Ubica], ");
            //20140613 AM. Se cambia campo de fecha
            //sbQuery.AppendLine("[Fecha]				= CONVERT(VARCHAR(10),CCustodia.[FechaCancelacion], 103), ");
            sbQuery.AppendLine("    [Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.dtFecUltAct, 103), ");
            sbQuery.AppendLine("    [icodregistro]      = ccustodia.icodregistro, ");
            sbQuery.AppendLine("    [Codigo Empleado]	= CCustodia.[Emple] ");
            sbQuery.AppendLine("FROM [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia ");
            sbQuery.AppendLine("    join [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] Estatus ");
            sbQuery.AppendLine("    on CCustodia.EstCCustodia = Estatus.icodcatalogo ");
            sbQuery.AppendLine("    AND CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia ");
            sbQuery.AppendLine("    AND CCustodia.dtFinVigencia >= GETDATE() ");
            sbQuery.AppendLine("    AND Estatus.dtIniVigencia <> Estatus.dtFinVigencia  ");
            sbQuery.AppendLine("    AND Estatus.dtFinVigencia >= GETDATE() ");
            sbQuery.AppendLine("join [VisHistoricos('Emple','Empleados','Español')] Emple  ");
            sbQuery.AppendLine("    ON CCustodia.[Emple] = Emple.[iCodCatalogo]  ");
            sbQuery.AppendLine("    AND Emple.dtIniVigencia <> Emple.dtFinVigencia  ");
            sbQuery.AppendLine("    AND Emple.dtFinVigencia >= GETDATE()  ");
            sbQuery.AppendLine("    AND Estatus.[value] = 3 --RECHAZADAS ");

            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            sbQuery.AppendLine("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust ");
            sbQuery.AppendLine("on EstCCust.iCodCatalogo = CCustodia.EstCCustodia ");
            sbQuery.AppendLine("and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE() ");

            //20140613 AM. Se cambia ordenamiento de consulta
            //sbQuery.AppendLine("ORDER BY [Fecha] desc ");
            sbQuery.AppendLine("ORDER BY CCustodia.dtFecUltAct desc ");

            return sbQuery.ToString();
        }

        /// <summary>
        /// Consulta de top 10 de cartas custodia pendientes
        /// </summary>
        /// <returns>Regresa la consulta en un string</returns>
        protected string Top10CCustPendientes()
        {
            sbQuery.Length = 0;

            sbQuery.AppendLine("SELECT TOP 10  ");
            sbQuery.AppendLine("    [Folio]			   = CCustodia.[FolioCCustodia], ");

            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            sbQuery.AppendLine("    [Estatus]	       = EstCCust.vchDescripcion, ");
            //sbQuery.AppendLine("[Estatus]	       = CCustodia.[EstCCustodiaDesc], ");
            
            sbQuery.AppendLine("    [No. Nomina]	   = Emple.[NominaA], ");
            sbQuery.AppendLine("    [Empleado]		   = IsNull(left(Emple.NomCompleto,400),''), ");
            sbQuery.AppendLine("    [Ubicación]	   	   = Emple.[Ubica], ");
            //20140613 AM. Se cambia campo de fecha
            //sbQuery.AppendLine("[Fecha]			   = CONVERT(VARCHAR(10),CCustodia.[FechaResp], 103), ");
            sbQuery.AppendLine("    [Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.dtFecUltAct, 103), ");
            sbQuery.AppendLine("    [icodregistro]     = ccustodia.icodregistro, ");
            sbQuery.AppendLine("    [Codigo Empleado]  = CCustodia.[Emple] ");
            sbQuery.AppendLine("FROM [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia ");
            sbQuery.AppendLine("join [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] Estatus ");
            sbQuery.AppendLine("    on CCustodia.EstCCustodia = Estatus.icodcatalogo ");
            sbQuery.AppendLine("    AND CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia ");
            sbQuery.AppendLine("    AND CCustodia.dtFinVigencia >= GETDATE() ");
            sbQuery.AppendLine("    AND Estatus.dtIniVigencia <> Estatus.dtFinVigencia  ");
            sbQuery.AppendLine("    AND Estatus.dtFinVigencia >= GETDATE() ");
            sbQuery.AppendLine("join [VisHistoricos('Emple','Empleados','Español')] Emple  ");
            sbQuery.AppendLine("    ON CCustodia.[Emple] = Emple.[iCodCatalogo]  ");
            sbQuery.AppendLine("    AND Emple.dtIniVigencia <> Emple.dtFinVigencia  ");
            sbQuery.AppendLine("    AND Emple.dtFinVigencia >= GETDATE()  ");
            sbQuery.AppendLine("    AND Estatus.[value] = 1  --PENDIENTES ");

            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            sbQuery.AppendLine("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust ");
            sbQuery.AppendLine("on EstCCust.iCodCatalogo = CCustodia.EstCCustodia ");
            sbQuery.AppendLine("and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE() ");

            //20140613 AM. Se cambia ordenamiento de consulta
            //sbQuery.AppendLine("ORDER BY [Fecha] desc ");
            sbQuery.AppendLine("ORDER BY CCustodia.dtFecUltAct desc ");

            return sbQuery.ToString();
        }
    }
}
