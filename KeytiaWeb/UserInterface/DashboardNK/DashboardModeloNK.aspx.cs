using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DSOControls2008;
using System.Web.UI.DataVisualization.Charting;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.DashboardNK
{
    public partial class DashboardModeloNK : System.Web.UI.Page
    {
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        DataTable ldtReporte = new DataTable(null);
        StringBuilder sbQuery = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            pToolBar.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";
            btnAplicar.Attributes["class"] = "buttonPlay";
            btnAplicar.Style["display"] = "none";

            lblFechaInicio.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#4297D7");
            lblFechaInicio.Style.Add(HtmlTextWriterStyle.Color, "#FFFFFF");
            lblFechaFin.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#4297D7");
            lblFechaFin.Style.Add(HtmlTextWriterStyle.Color, "#FFFFFF");

            pdtInicio.Row = 1;
            pdtInicio.ShowHour = false;
            pdtInicio.ShowMinute = false;
            pdtInicio.ShowSecond = false;

            pdtFin.Row = 1;
            pdtFin.ShowHour = false;
            pdtFin.ShowMinute = false;
            pdtFin.ShowSecond = false;

            ldtReporte.Clear();
            ldtReporte = DSODataAccess.Execute("exec zzPTObtieneConsumoPorRango");
            CrearGraficaPie(ldtReporte, "Consumo por Rangos", "c2");
            ldtReporte.Columns.Remove("Consumo");
            CrearGraficaPie(ldtReporte, "Empleados por Rango", "");

            if (!Page.IsPostBack)
            {
                pdtInicio.CreateControls();
                pdtInicio.DataValue = (object)fechaInicio;
                pdtFin.CreateControls();
                pdtFin.DataValue = (object)fechaFinal;
                generaReportesDeDashboard();
            }
        }

        protected void CrearGraficaPie(DataTable reporte, string titulo, string labelformat)
        {

            Chart graficaSeries1 = new Chart();
            graficaSeries1.Width = Unit.Pixel(600);
            graficaSeries1.Height = Unit.Pixel(600);
            graficaSeries1.ChartAreas.Add("Grafica");
            graficaSeries1.ChartAreas["Grafica"].Area3DStyle.Enable3D = true;
            graficaSeries1.Titles.Add(titulo);
            graficaSeries1.Titles[0].Font = new Font("Verdana", 14, FontStyle.Bold);
            

            graficaSeries1.Series.Add("Series1");
            graficaSeries1.Series["Series1"].ChartType = SeriesChartType.Pie;
            graficaSeries1.Legends.Add("Legend");
            graficaSeries1.Legends["Legend"].LegendStyle = LegendStyle.Table;
            graficaSeries1.Legends["Legend"].Docking = Docking.Bottom;
            graficaSeries1.Legends["Legend"].IsDockedInsideChartArea = false;
            graficaSeries1.Legends["Legend"].Alignment = StringAlignment.Center;
            graficaSeries1.Legends["Legend"].Title = "Rangos de Consumo";



            graficaSeries1.DataSource = reporte;

            graficaSeries1.Series["Series1"].XValueMember = reporte.Columns[0].ColumnName.ToString();
            graficaSeries1.Series["Series1"].YValueMembers = reporte.Columns[1].ColumnName.ToString();
            graficaSeries1.Series["Series1"].IsVisibleInLegend = true;
            graficaSeries1.Series["Series1"].IsValueShownAsLabel = true;
            graficaSeries1.Series["Series1"]["PieLabelStyle"] = "Outside";
            graficaSeries1.Series["Series1"]["PieLineColor"] = "black";
            graficaSeries1.Series["Series1"].LabelFormat = labelformat;
            //graficaSeries1.Series["Series1"].Label = "#VALY{$0.00} \n #VALX empleados";

            graficaSeries1.DataBind();

            divGraficasPie.Controls.Add(graficaSeries1);


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
        /// Crea reporte en grafica de columna.
        /// </summary>
        /// <param name="lsQuery"></param>

        protected void creaGrafHist(string lsQuery, string lsTitulo)
        {
            lblGrafCol.Text = lsTitulo;
            grafRepCol.Legends.Add("Legends");
            grafRepCol.Legends["Legends"].LegendStyle = LegendStyle.Column;


            grafRepCol.Series["Actual"].ChartType = SeriesChartType.Line;
            grafRepCol.Series["Actual"].IsValueShownAsLabel = true;
            grafRepCol.Series["Actual"].LabelFormat = "c2";
            //grafRepCol.Series["Actual"].Legend = "Año Actual";
            grafRepCol.Series["Actual"].BorderColor = Color.Red;
            grafRepCol.Series["Actual"].BorderWidth = 4;
            grafRepCol.Series["Actual"].Legend = "Legends";
            grafRepCol.Series["Actual"].LegendText = DateTime.Today.Year.ToString();


            grafRepCol.Series["Anterior"].ChartType = SeriesChartType.Line;
            grafRepCol.Series["Anterior"].IsValueShownAsLabel = true;
            grafRepCol.Series["Anterior"].LabelFormat = "c2";
            grafRepCol.Series["Anterior"].BorderWidth = 4;
            grafRepCol.Series["Anterior"].BorderColor = Color.Green;
            grafRepCol.Series["Anterior"].Legend = "Legends";
            grafRepCol.Series["Anterior"].LegendText = (DateTime.Today.Year - 1).ToString();


            grafRepCol.Width = 1100;


            ldtReporte.Clear();
            ldtReporte = DSODataAccess.Execute(lsQuery);

            /*for (int i = 0; i < ldtReporte.Rows.Count; i++)
            {
                ldtReporte.Rows[i][2] = float.Parse(ldtReporte.Rows[i][1].ToString()) * 1.5;


            }*/

            grafRepCol.DataSource = ldtReporte;
            //grafRepCol.Series.Add(
            grafRepCol.Series["Actual"].XValueMember = ldtReporte.Columns[0].ColumnName.ToString();
            grafRepCol.Series["Actual"].YValueMembers = ldtReporte.Columns[1].ColumnName.ToString();
            grafRepCol.Series["Anterior"].XValueMember = ldtReporte.Columns[0].ColumnName.ToString();
            grafRepCol.Series["Anterior"].YValueMembers = ldtReporte.Columns[2].ColumnName.ToString();
            grafRepCol.DataBind();
        }

        /// <summary>
        /// Query del reporteTop10EmpleadosMasCaros.
        /// </summary>
        /// <returns></returns>
        protected string reporteTop10EmpleadosMasCaros()
        {
            sbQuery.Length = 0;

            sbQuery.Append("create table #temp(ID int identity not null,Nombre varchar(max) not null, \r");
            sbQuery.Append("Total float not null) insert into #temp \r");
            sbQuery.Append("SELECT TOP 10 UPPER(Emple.NomCompleto) AS Nombre, \r");
            sbQuery.Append("Total = CAST(ROUND(SUM(ConsCDR.Costo+ConsCDR.CostoSM),2) AS FLOAT)  \r");
            sbQuery.Append("FROM Modelo.ConsolidadoCDR ConsCDR, \r");
            sbQuery.Append("Modelo.[VisHistoricos('Emple','Empleados','Español')] Emple  \r");
            sbQuery.Append("WHERE Emple.iCodCatalogo = ConsCDR.iCodCatalogoEmple  \r");
            sbQuery.Append("AND ConsCDR.FechaInicio >= '" + pdtInicio.DataValue.ToString().Substring(1, 10) + "' \r");
            sbQuery.Append("AND ConsCDR.FechaInicio <=  '" + pdtFin.DataValue.ToString().Substring(1, 10) + "' \r");
            sbQuery.Append("AND ConsCDR.iCodMaestro = 7   \r");
            sbQuery.Append("AND Emple.dtIniVigencia <> Emple.dtFinVigencia  \r");
            sbQuery.Append("AND Emple.dtFinVigencia >= GETDATE()  \r");
            sbQuery.Append("AND Emple.NomCompleto not like '%POR%IDENTIFICAR%'   \r");
            sbQuery.Append("GROUP BY Emple.NomCompleto ORDER BY Total DESC   \r");
            sbQuery.Append("select Nombre,Total = '$' + Convert(varchar,Total)  \r");
            sbQuery.Append("from #temp order by ID drop table #temp  \r");

            return sbQuery.ToString();
        }

        /// <summary>
        /// Query del reporteTop10SitiosMasCaros.
        /// </summary>
        /// <returns></returns>
        protected string reporteTop10SitiosMasCaros()
        {
            sbQuery.Length = 0;

            sbQuery.Append("create table #temp(ID int identity not null,Sitio varchar(max) not null, \r");
            sbQuery.Append("Total float not null) insert into #temp \r");
            sbQuery.Append("SELECT TOP 10 UPPER(Sitio.vchDESCripcion) AS Sitio,   \r");
            sbQuery.Append("Total = CAST(ROUND(SUM(ConsCDR.Costo+ConsCDR.CostoSM),2) AS FLOAT)   \r");
            sbQuery.Append("FROM Modelo.ConsolidadoCDR ConsCDR,    \r");
            sbQuery.Append(" \t \t  \t \t (select Historicos.icodcatalogo, Historicos.vchdescripcion  \r");
            sbQuery.Append(" \t \t  \t \t from modelo.historicos Historicos,  \r");
            sbQuery.Append(" \t \t  \t \t  \t \t  \t \t (select icodregistro  \r");
            sbQuery.Append("\t \t  \t \t  \t \t  \t \t from modelo.catalogos  \r");
            sbQuery.Append("\t \t  \t \t  \t \t  \t \t where icodcatalogo =(select icodregistro  \r");
            sbQuery.Append("\t \t  \t \t  \t \t  \t \t  \t \t  \t \t  \t \t from Modelo.Catalogos  \r");
            sbQuery.Append("\t \t  \t \t  \t \t  \t \t  \t \t  \t \t  \t \t where vchcodigo like 'sitio'  \r");
            sbQuery.Append("\t \t  \t \t  \t \t  \t \t  \t \t  \t \t  \t \t and icodcatalogo is null)) as CatSitios  \r");
            sbQuery.Append("\t \t  \t \t where Historicos.icodcatalogo = CatSitios.icodregistro  \r");
            sbQuery.Append("\t \t  \t \t and Historicos.dtinivigencia<>Historicos.dtfinvigencia  \r");
            sbQuery.Append("\t \t  \t \t and Historicos.dtfinvigencia>=getdate()) As Sitio  \r");
            sbQuery.Append("WHERE Sitio.iCodCatalogo = ConsCDR.iCodCatalogoSitio    \r");
            sbQuery.Append("AND ConsCDR.FechaInicio >= '" + pdtInicio.DataValue.ToString().Substring(1, 10) + "' \r");
            sbQuery.Append("AND ConsCDR.FechaInicio <=  '" + pdtFin.DataValue.ToString().Substring(1, 10) + "' \r");
            sbQuery.Append("AND ConsCDR.iCodMaestro = 27    \r");
            sbQuery.Append("GROUP BY Sitio.vchDESCripcion  \r");
            sbQuery.Append("ORDER BY Total DESC   \r");
            sbQuery.Append("select Sitio,Total = '$' + Convert(varchar,Total)  \r");
            sbQuery.Append("from #temp order by ID drop table #temp  \r");

            return sbQuery.ToString();

        }

        /// <summary>
        /// Query del reporteTop10SitiosMasCaros.
        /// </summary>
        /// <returns></returns>
        protected string reporteTop10CenCosMasCaros()
        {
            sbQuery.Length = 0;
            sbQuery.Append("create table #temp(ID int identity not null,[Centro de Costos] varchar(max) not null, \r");
            sbQuery.Append("Total float not null) insert into #temp \r");
            sbQuery.Append("SELECT TOP 10 UPPER(CenCos.CenCosDESC) AS [Centro de Costos],   \r");
            sbQuery.Append("Total = CAST(ROUND(SUM(ConsCDR.Costo+ConsCDR.CostoSM),2) AS FLOAT)   \r");
            sbQuery.Append("FROM Modelo.ConsolidadoCDR ConsCDR,    \r");
            sbQuery.Append("Modelo.[VisRelaciones('CentroCosto-Empleado','Español')] CenCos   \r");
            sbQuery.Append("WHERE CenCos.Emple = ConsCDR.iCodCatalogoEmple   \r");
            sbQuery.Append("AND ConsCDR.FechaInicio >= '" + pdtInicio.DataValue.ToString().Substring(1, 10) + "' \r");
            sbQuery.Append("AND ConsCDR.FechaInicio <=  '" + pdtFin.DataValue.ToString().Substring(1, 10) + "' \r");
            sbQuery.Append("AND ConsCDR.iCodMaestro = 7    \r");
            sbQuery.Append("AND CenCos.CenCosDESC not like '%POR%IDENTIFICAR%' \r");
            sbQuery.Append("GROUP BY CenCos.CenCosDESC ORDER BY Total DESC   \r");
            sbQuery.Append("select [Centro de Costos],Total = '$' + Convert(varchar,Total)  \r");
            sbQuery.Append("from #temp order by ID drop table #temp  \r");

            return sbQuery.ToString();

        }

        /// <summary>
        /// En este metodo se van agregando los reportes que se necesitan mostrar en el dashboard.
        /// </summary>
        protected void generaReportesDeDashboard()
        {
            grvReporteTop10EmpMasCaros.DataSource = FillDTReporte(reporteTop10EmpleadosMasCaros());
            grvReporteTop10EmpMasCaros.DataBind();
            lblReporteTop10EmpMasCaros.Text = "Top 10 Empleados Mas Caros";

            grvReporteTop10SitMasCaros.DataSource = FillDTReporte(reporteTop10SitiosMasCaros());
            grvReporteTop10SitMasCaros.DataBind();
            lblReporteTop10SitMasCaros.Text = "Top 10 Sitios Mas Caros";

            grvReporteTop10CenCosMasCaros.DataSource = FillDTReporte(reporteTop10CenCosMasCaros());
            grvReporteTop10CenCosMasCaros.DataBind();
            lblReporteTop10CenCosMasCaros.Text = "Top 10 CC Mas Caros";

            creaGrafHist("exec zzptConsumoHistoricoActualVsAnterior  @Schema='Modelo'", "Año Actual vs Anterior");

        }

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            generaReportesDeDashboard();
        }
    }
}
