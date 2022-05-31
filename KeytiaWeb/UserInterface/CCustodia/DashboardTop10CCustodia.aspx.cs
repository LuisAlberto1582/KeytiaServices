using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;
using System.Text;

namespace KeytiaWeb.UserInterface.CCustodia
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

            sbQuery.Append("SELECT TOP 10   \r");
            sbQuery.Append("[Folio]				= CCustodia.[FolioCCustodia], \r");
            
            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            sbQuery.Append("[Estatus]	       = EstCCust.vchDescripcion, \r");
            //sbQuery.Append("[Estatus]	       = CCustodia.[EstCCustodiaDesc], \r");

            sbQuery.Append("[No. Nomina]		= Emple.[NominaA], \r");
            sbQuery.Append("[Empleado]			= IsNull(left(Emple.NomCompleto,400),''), \r");
            sbQuery.Append("[Ubicación]			= Emple.[Ubica], \r");
            //20140613 AM. Se cambia campo de fecha
            //sbQuery.Append("[Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.[FechaCreacion], 103), \r");
            sbQuery.Append("[Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.dtFecUltAct, 103), \r");
            sbQuery.Append("[icodregistro]      = ccustodia.icodregistro, \r");
            sbQuery.Append("[Codigo Empleado]	= CCustodia.[Emple] \r");
            sbQuery.Append("FROM [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia \r");
            sbQuery.Append("join [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] Estatus \r");
            sbQuery.Append("on CCustodia.EstCCustodia = Estatus.icodcatalogo \r");
            sbQuery.Append("AND CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia \r");
            sbQuery.Append("AND CCustodia.dtFinVigencia >= GETDATE() \r");
            sbQuery.Append("AND Estatus.dtIniVigencia <> Estatus.dtFinVigencia  \r");
            sbQuery.Append("AND Estatus.dtFinVigencia >= GETDATE() \r");
            sbQuery.Append("join [VisHistoricos('Emple','Empleados','Español')] Emple  \r");
            sbQuery.Append("ON CCustodia.[Emple] = Emple.[iCodCatalogo]  \r");
            sbQuery.Append("AND Emple.dtIniVigencia <> Emple.dtFinVigencia  \r");
            sbQuery.Append("AND Emple.dtFinVigencia >= GETDATE()  \r");
            sbQuery.Append("AND Estatus.[value] = 2  --ACEPTADAS \r");


            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            sbQuery.Append("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust \r");
            sbQuery.Append("on EstCCust.iCodCatalogo = CCustodia.EstCCustodia \r");
            sbQuery.Append("and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE() \r");

            //20140613 AM. Se cambia ordenamiento de consulta
            //sbQuery.Append("ORDER BY [Fecha] desc \r");
            sbQuery.Append("ORDER BY CCustodia.dtFecUltAct desc \r");

            return sbQuery.ToString();
        }

        /// <summary>
        /// Consulta de top 10 de cartas custodia rechazadas
        /// </summary>
        /// <returns>Regresa la consulta en un string</returns>
        protected string Top10CCustRechazadas()
        {
            sbQuery.Length = 0;

            sbQuery.Append("SELECT TOP 10  \r");
            sbQuery.Append("[Folio]				= CCustodia.[FolioCCustodia], \r");

            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            sbQuery.Append("[Estatus]	       = EstCCust.vchDescripcion, \r");
            //sbQuery.Append("[Estatus]	       = CCustodia.[EstCCustodiaDesc], \r");

            sbQuery.Append("[No. Nomina]		= Emple.[NominaA], \r");
            sbQuery.Append("[Empleado]			= IsNull(left(Emple.NomCompleto,400),''), \r");
            sbQuery.Append("[Ubicación]			= Emple.[Ubica], \r");
            //20140613 AM. Se cambia campo de fecha
            //sbQuery.Append("[Fecha]				= CONVERT(VARCHAR(10),CCustodia.[FechaCancelacion], 103), \r");
            sbQuery.Append("[Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.dtFecUltAct, 103), \r");
            sbQuery.Append("[icodregistro]      = ccustodia.icodregistro, \r");
            sbQuery.Append("[Codigo Empleado]	= CCustodia.[Emple] \r");
            sbQuery.Append("FROM [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia \r");
            sbQuery.Append("join [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] Estatus \r");
            sbQuery.Append("on CCustodia.EstCCustodia = Estatus.icodcatalogo \r");
            sbQuery.Append("AND CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia \r");
            sbQuery.Append("AND CCustodia.dtFinVigencia >= GETDATE() \r");
            sbQuery.Append("AND Estatus.dtIniVigencia <> Estatus.dtFinVigencia  \r");
            sbQuery.Append("AND Estatus.dtFinVigencia >= GETDATE() \r");
            sbQuery.Append("join [VisHistoricos('Emple','Empleados','Español')] Emple  \r");
            sbQuery.Append("ON CCustodia.[Emple] = Emple.[iCodCatalogo]  \r");
            sbQuery.Append("AND Emple.dtIniVigencia <> Emple.dtFinVigencia  \r");
            sbQuery.Append("AND Emple.dtFinVigencia >= GETDATE()  \r");
            sbQuery.Append("AND Estatus.[value] = 3 --RECHAZADAS \r");

            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            sbQuery.Append("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust \r");
            sbQuery.Append("on EstCCust.iCodCatalogo = CCustodia.EstCCustodia \r");
            sbQuery.Append("and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE() \r");

            //20140613 AM. Se cambia ordenamiento de consulta
            //sbQuery.Append("ORDER BY [Fecha] desc \r");
            sbQuery.Append("ORDER BY CCustodia.dtFecUltAct desc \r");

            return sbQuery.ToString();
        }

        /// <summary>
        /// Consulta de top 10 de cartas custodia pendientes
        /// </summary>
        /// <returns>Regresa la consulta en un string</returns>
        protected string Top10CCustPendientes()
        {
            sbQuery.Length = 0;

            sbQuery.Append("SELECT TOP 10  \r");
            sbQuery.Append("[Folio]			   = CCustodia.[FolioCCustodia], \r");

            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            sbQuery.Append("[Estatus]	       = EstCCust.vchDescripcion, \r");
            //sbQuery.Append("[Estatus]	       = CCustodia.[EstCCustodiaDesc], \r");
            
            sbQuery.Append("[No. Nomina]	   = Emple.[NominaA], \r");
            sbQuery.Append("[Empleado]		   = IsNull(left(Emple.NomCompleto,400),''), \r");
            sbQuery.Append("[Ubicación]	   	   = Emple.[Ubica], \r");
            //20140613 AM. Se cambia campo de fecha
            //sbQuery.Append("[Fecha]			   = CONVERT(VARCHAR(10),CCustodia.[FechaResp], 103), \r");
            sbQuery.Append("[Fecha]		     	=  CONVERT(VARCHAR(10), CCustodia.dtFecUltAct, 103), \r");
            sbQuery.Append("[icodregistro]     = ccustodia.icodregistro, \r");
            sbQuery.Append("[Codigo Empleado]  = CCustodia.[Emple] \r");
            sbQuery.Append("FROM [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia \r");
            sbQuery.Append("join [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] Estatus \r");
            sbQuery.Append("on CCustodia.EstCCustodia = Estatus.icodcatalogo \r");
            sbQuery.Append("AND CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia \r");
            sbQuery.Append("AND CCustodia.dtFinVigencia >= GETDATE() \r");
            sbQuery.Append("AND Estatus.dtIniVigencia <> Estatus.dtFinVigencia  \r");
            sbQuery.Append("AND Estatus.dtFinVigencia >= GETDATE() \r");
            sbQuery.Append("join [VisHistoricos('Emple','Empleados','Español')] Emple  \r");
            sbQuery.Append("ON CCustodia.[Emple] = Emple.[iCodCatalogo]  \r");
            sbQuery.Append("AND Emple.dtIniVigencia <> Emple.dtFinVigencia  \r");
            sbQuery.Append("AND Emple.dtFinVigencia >= GETDATE()  \r");
            sbQuery.Append("AND Estatus.[value] = 1  --PENDIENTES \r");

            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            sbQuery.Append("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust \r");
            sbQuery.Append("on EstCCust.iCodCatalogo = CCustodia.EstCCustodia \r");
            sbQuery.Append("and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE() \r");

            //20140613 AM. Se cambia ordenamiento de consulta
            //sbQuery.Append("ORDER BY [Fecha] desc \r");
            sbQuery.Append("ORDER BY CCustodia.dtFecUltAct desc \r");

            return sbQuery.ToString();
        }
    }
}
