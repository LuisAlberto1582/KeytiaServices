using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Globalization;
using System.Web.SessionState;
using KeytiaServiceBL.Reportes;

namespace KeytiaWeb.UserInterface
{
    public enum AreaBloqueEnum
    {
        Grafica = 1,
        GraficaHis = 2,
        Reporte = 4
    }
    public class DashboardReportField : IKeytiaFillableField
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected int piCodBloque;
        protected string pvchCodBloque;
        protected DashboardControl pDashboard;
        protected ValidacionPermisos ValidarPermiso;

        protected DataTable pVisConfigPersonalizada;

        protected int piCodConsultaDefault;
        protected int piAreaBloqueDefault;

        protected int pRow;
        protected int pCol;
        protected int pBlockSize;
        protected int piBanderasBloque;

        protected Panel pPanelConfig;
        protected Panel pPanelReporte;

        protected Table pTablaReportes; //Tabla donde se agregara el bloque
        protected TableRow pTableRow;   //Fila de la tabla donde se agrego el bloque
        protected TableCell pTableCell; //Celda de la tabla donde se agrego el bloque

        protected StringBuilder psbQuery = new StringBuilder();
        protected KDBAccess pKDB = new KDBAccess();
        protected DateTime pIniVigencia = DateTime.Now;

        protected DSODropDownList pddlConsulta;
        protected DSORadioButtonList popcAreaBloque; //Area del reporte mostrada en el bloque
        protected HtmlButton pbtnIr;
        protected ReporteEstandar pReporte;

        protected int piCodConsulta = 0;
        protected int piCodReporte = 0;
        protected int piAreaBloque = 0;
        protected int piRepTipoGrafica = 0;
        protected int piRepTipoGraficaHis = 0;

        protected DashboardFieldCollection pCollection;

        public DashboardReportField(DashboardControl lDashboard, Table lTablaReportes, ValidacionPermisos lValidarPermiso)
        {
            pDashboard = lDashboard;
            pTablaReportes = lTablaReportes;
            ValidarPermiso = lValidarPermiso;
        }

        public int iCodBloque
        {
            get
            {
                return piCodBloque;
            }
            set
            {
                piCodBloque = value;
            }
        }

        public string vchCodBloque
        {
            get
            {
                return pvchCodBloque;
            }
            set
            {
                pvchCodBloque = value;
            }
        }

        public int Row
        {
            get
            {
                return pRow;
            }
            set
            {
                pRow = value;
            }
        }

        public int Col
        {
            get
            {
                return pCol;
            }
            set
            {
                pCol = value;
            }
        }

        public int BlockSize
        {
            get
            {
                return pBlockSize;
            }
            set
            {
                pBlockSize = value;
            }
        }

        public int BanderasBloque
        {
            get
            {
                return piBanderasBloque;
            }
            set
            {
                piBanderasBloque = value;
            }
        }

        public bool NoSelRep
        {
            get
            {
                return (piBanderasBloque & 1) == 1;
            }
        }

        public int ConsultaDefault
        {
            get
            {
                return piCodConsultaDefault;
            }
            set
            {
                piCodConsultaDefault = value;
            }
        }

        public int AreaBloqueDefault
        {
            get
            {
                return piAreaBloqueDefault;
            }
            set
            {
                piAreaBloqueDefault = value;
            }
        }

        public int Consulta
        {
            get
            {
                return piCodConsulta;
            }
            set
            {
                piCodConsulta = value;
                if (pddlConsulta != null
                    && pddlConsulta.DropDownList != null)
                {
                    pddlConsulta.DataValue = value;
                }
                piCodReporte = (int)Collection.VisConsultas.Select("Aplic = " + piCodConsulta)[0]["RepEst"];
            }
        }

        public int AreaBloque
        {
            get
            {
                return piAreaBloque;
            }
            set
            {
                piAreaBloque = value;
                if (popcAreaBloque != null
                    && popcAreaBloque.RadioButtonList != null)
                {
                    popcAreaBloque.DataValue = value;
                }
            }
        }

        public int RepTipoGrafica
        {
            get
            {
                return piRepTipoGrafica;
            }
            set
            {
                piRepTipoGrafica = value;
                if (pReporte != null
                    && pReporte.OpcTipoGrafica != null
                    && pReporte.OpcTipoGrafica.DSOControlDB != null)
                {
                    pReporte.OpcTipoGrafica.DataValue = piRepTipoGrafica;
                }
            }
        }

        public int RepTipoGraficaHis
        {
            get
            {
                return piRepTipoGraficaHis;
            }
            set
            {
                piRepTipoGraficaHis = value;
                if (pReporte != null
                    && pReporte.OpcTipoGraficaHis != null
                    && pReporte.OpcTipoGraficaHis.DSOControlDB != null)
                {
                    pReporte.OpcTipoGraficaHis.DataValue = piRepTipoGraficaHis;
                }
            }
        }

        public ReporteEstandar Reporte
        {
            get
            {
                return pReporte;
            }
        }

        public Table TablaReporte
        {
            get
            {
                return pTablaReportes;
            }
        }

        public TableRow TableRow
        {
            get
            {
                return pTableRow;
            }
        }

        public TableCell TableCell
        {
            get
            {
                return pTableCell;
            }
        }

        public DateTime IniVigencia
        {
            get
            {
                return pIniVigencia;
            }
            set
            {
                pIniVigencia = value;
                pKDB.FechaVigencia = value;
            }
        }

        public DashboardFieldCollection Collection
        {
            get
            {
                return pCollection;
            }
            set
            {
                pCollection = value;
            }
        }


        public void CreateField()
        {
            InitConfigPersonalizada();

            pPanelConfig = new Panel();
            pPanelConfig.CssClass = "DashboardPanelConfig";

            pPanelReporte = new Panel();
            pPanelReporte.CssClass = "DashboardPanelReporte";

            pddlConsulta = new DSODropDownList();
            pddlConsulta.ID = "Bq" + piCodBloque + "_ddlConsulta";
            pddlConsulta.DataField = "Consul";
            pddlConsulta.DataValueDelimiter = "";
            pPanelConfig.Controls.Add(pddlConsulta);

            pbtnIr = new HtmlButton();
            pbtnIr.ID = "Bq" + piCodBloque + "_btnIr";
            pbtnIr.Attributes["class"] = "buttonSearchImg";
            pbtnIr.Style["display"] = "none";
            pbtnIr.InnerText = "...";
            pbtnIr.ServerClick += new EventHandler(pbtnIr_ServerClick);
            pPanelConfig.Controls.Add(pbtnIr);

            popcAreaBloque = new DSORadioButtonList();
            popcAreaBloque.ID = "Bq" + piCodBloque + "_opcAreaBloque";
            popcAreaBloque.DataField = "AreaBloque";
            popcAreaBloque.DataValueDelimiter = "";
            pPanelConfig.Controls.Add(popcAreaBloque);

            InitTablaReportes();

            pddlConsulta.CreateControls();
            pddlConsulta.AutoPostBack = true;
            pddlConsulta.DropDownListChange += new EventHandler(pddlConsulta_DropDownListChange);

            popcAreaBloque.CreateControls();
            popcAreaBloque.RadioButtonList.AutoPostBack = true;
            popcAreaBloque.RadioButtonList.RepeatDirection = RepeatDirection.Horizontal;
            popcAreaBloque.RadioButtonList.RepeatColumns = 3;
            popcAreaBloque.RadioButtonList.SelectedIndexChanged += new EventHandler(popcAreaBloque_SelectedIndexChanged);

        }

        protected void InitConfigPersonalizada()
        {
            //psbQuery.Length = 0;
            //psbQuery.AppendLine("select * ");
            //psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('BloqueDashboard','Configuración Personalizada','" + Globals.GetCurrentLanguage() + "')] His");
            //psbQuery.AppendLine("where His.dtIniVigencia <= '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            //psbQuery.AppendLine("and His.dtFinVigencia > '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            //psbQuery.AppendLine("and His.Usuar = " + Session["iCodUsuario"]);
            //psbQuery.AppendLine("and His.Consul in(" + pCollection.GetVisConsultasQuery("Rel.Aplic") + ")");
            //psbQuery.AppendLine("and His.BloqueDashboard = " + piCodBloque);

            //pVisConfigPersonalizada = DSODataAccess.Execute(psbQuery.ToString());
            pVisConfigPersonalizada = pCollection.VisConfigPersonalizada.Clone();
            if (pCollection.VisConfigPersonalizada.Select("BloqueDashboard = " + piCodBloque).Length > 0)
            {
                pVisConfigPersonalizada.ImportRow(pCollection.VisConfigPersonalizada.Select("BloqueDashboard = " + piCodBloque)[0]);
 
            }

            Consulta = piCodConsultaDefault;
            piAreaBloque = piAreaBloqueDefault;

            if (pVisConfigPersonalizada.Rows.Count > 0)
            {
                if (pVisConfigPersonalizada.Rows[0]["Consul"] != DBNull.Value && !NoSelRep)
                {
                    Consulta = (int)pVisConfigPersonalizada.Rows[0]["Consul"];
                }
                if (pVisConfigPersonalizada.Rows[0]["AreaBloque"] != DBNull.Value)
                {
                    piAreaBloque = (int)pVisConfigPersonalizada.Rows[0]["AreaBloque"];
                }
                if (pVisConfigPersonalizada.Rows[0]["RepTipoGrafica"] != DBNull.Value)
                {
                    piRepTipoGrafica = (int)pVisConfigPersonalizada.Rows[0]["RepTipoGrafica"];
                }
                if (pVisConfigPersonalizada.Rows[0]["RepTipoGraficaHis"] != DBNull.Value)
                {
                    piRepTipoGraficaHis = (int)pVisConfigPersonalizada.Rows[0]["RepTipoGraficaHis"];
                }
            }
        }

        protected void InitTablaReportes()
        {
            pTableCell = new TableCell();
            pTableCell.CssClass = "DashboardCell" + pBlockSize;
            pTableCell.ColumnSpan = pBlockSize;

            pTableCell.Controls.Add(pPanelConfig);
            pTableCell.Controls.Add(pPanelReporte);

            if (pTablaReportes.Rows.Count < pRow)
            {
                pTableRow = new TableRow();
                pTablaReportes.Rows.Add(pTableRow);
                pRow = pTablaReportes.Rows.Count;
            }
            else
            {
                pTableRow = pTablaReportes.Rows[pRow - 1];
            }

            pTableRow.Controls.Add(pTableCell);
        }

        protected void InitConfigReporte()
        {
            pPanelReporte.Controls.Clear();

            pReporte = new ReporteEstandar();
            pReporte.ID = "Bq" + piCodBloque + "_Reporte_" + piCodReporte;
            pReporte.OpcMnu = pDashboard.OpcMnu;
            pReporte.lblTitle = pDashboard.lblTitle;
            pReporte.iCodConsulta = piCodConsulta;
            pReporte.iCodReporte = piCodReporte;
            //pReporte.iCodPerfil = pDashboard.iCodPerfil;
            pReporte.iEstadoConsulta = 0;
            pReporte.ParentContainer = pDashboard.Parent;

            pPanelReporte.Controls.Add(pReporte);
        }

        public void InitField()
        {
            InitConfigReporte();
            InitReporte();
            InitAreaBloque();

            pReporte.LoadScripts();
        }

        protected void InitReporte()
        {
            pReporte.CreateControls();
            pReporte.PostInitRuta += new EventHandler(pReporte_PostInitRuta);
            pReporte.PostSetSubReporteParams += new EventHandler(pReporte_PostSetSubReporteParams);

            if (pReporte.bAreaGrafica)
            {
                ((DSORadioButtonList)pReporte.OpcTipoGrafica.DSOControlDB).RadioButtonList.SelectedIndexChanged += new EventHandler(OpcTipoGrafica_SelectedIndexChanged);
                if (piRepTipoGrafica != 0)
                {
                    pReporte.OpcTipoGrafica.DataValue = piRepTipoGrafica;
                }
                else
                {
                    piRepTipoGrafica = int.Parse(pReporte.OpcTipoGrafica.DataValue.ToString());
                }
            }
            if (pReporte.bAreaGraficaHis)
            {
                ((DSORadioButtonList)pReporte.OpcTipoGraficaHis.DSOControlDB).RadioButtonList.SelectedIndexChanged += new EventHandler(OpcTipoGraficaHis_SelectedIndexChanged);
                if (piRepTipoGraficaHis != 0)
                {
                    pReporte.OpcTipoGraficaHis.DataValue = piRepTipoGraficaHis;
                }
                else
                {
                    piRepTipoGraficaHis = int.Parse(pReporte.OpcTipoGraficaHis.DataValue.ToString());
                }
            }
        }

        protected void InitAreaBloque()
        {
            ReportState lReportStateGrafica;
            ReportState lReportStateGraficaHis;
            ReportState lReportStateReporte;

            if (pBlockSize == 1)
            {
                lReportStateGrafica = ReportState.DashboardGrafica;
                lReportStateGraficaHis = ReportState.DashboardGraficaHis;
                lReportStateReporte = ReportState.DashboardReporte;
            }
            else
            {
                lReportStateGrafica = ReportState.DashboardGrafica2Bloques;
                lReportStateGraficaHis = ReportState.DashboardGraficaHis2Bloques;
                lReportStateReporte = ReportState.DashboardReporte2Bloques;
            }

            if (pReporte.bAreaGrafica
                && ((AreaBloqueEnum)piAreaBloque) == AreaBloqueEnum.Grafica)
            {
                pReporte.SetReportState(lReportStateGrafica);
            }
            else if (pReporte.bAreaGraficaHis
                && ((AreaBloqueEnum)piAreaBloque) == AreaBloqueEnum.GraficaHis)
            {
                pReporte.SetReportState(lReportStateGraficaHis);
            }
            else if (pReporte.bAreaReporte
                && ((AreaBloqueEnum)piAreaBloque) == AreaBloqueEnum.Reporte)
            {
                pReporte.SetReportState(lReportStateReporte);
            }
            else //esta configurada un area de bloque que el reporte no tiene
            {
                if (pReporte.OrdenZonas == OrdenZonasReporte.PrimeroGraficas)
                {
                    if (pReporte.bAreaGrafica)
                    {
                        piAreaBloque = (int)AreaBloqueEnum.Grafica;
                        pReporte.SetReportState(lReportStateGrafica);
                    }
                    else if (pReporte.bAreaGraficaHis)
                    {
                        piAreaBloque = (int)AreaBloqueEnum.GraficaHis;
                        pReporte.SetReportState(lReportStateGraficaHis);
                    }
                    else if (pReporte.bAreaReporte)
                    {
                        piAreaBloque = (int)AreaBloqueEnum.Reporte;
                        pReporte.SetReportState(lReportStateReporte);
                    }
                }
                else if (pReporte.OrdenZonas == OrdenZonasReporte.PrimeroReportes)
                {
                    if (pReporte.bAreaReporte)
                    {
                        piAreaBloque = (int)AreaBloqueEnum.Reporte;
                        pReporte.SetReportState(lReportStateReporte);
                    }
                    else if (pReporte.bAreaGrafica)
                    {
                        piAreaBloque = (int)AreaBloqueEnum.Grafica;
                        pReporte.SetReportState(lReportStateGrafica);
                    }
                    else if (pReporte.bAreaGraficaHis)
                    {
                        piAreaBloque = (int)AreaBloqueEnum.GraficaHis;
                        pReporte.SetReportState(lReportStateGraficaHis);
                    }
                }
            }
        }

        protected void pReporte_PostInitRuta(object sender, EventArgs e)
        {
            pDashboard.SubReporte = pReporte.SubReporte;
            if (pDashboard.SubReporte.Fields == null)
            {
                GuardarConfiguracion();
                pDashboard.SetDashboardState(DashboardState.SubConsulta);
            }
        }

        protected void pReporte_PostSetSubReporteParams(object sender, EventArgs e)
        {
            GuardarConfiguracion();
            pDashboard.SetDashboardState(DashboardState.SubConsulta);
        }

        protected void pbtnIr_ServerClick(object sender, EventArgs e)
        {
            string lsDataValueDelimiter;

            pDashboard.iCodSubConsulta = pReporte.iCodConsulta;
            pDashboard.iCodSubReporte = pReporte.iCodReporte;
            pDashboard.iSubEstadoConsulta = pReporte.iEstadoConsulta;
            pDashboard.InitSubReporte();

            if (pReporte.Fields != null)
            {
                pReporte.Fields.IniVigencia = (DateTime)Session["FechaIniRep"];
                pReporte.Fields.FinVigencia = (DateTime)Session["FechaFinRep"];
                pReporte.Fields.FillAjaxControls();

                foreach (KeytiaBaseField lField in pReporte.Fields)
                {
                    if (lField is KeytiaVarcharField && lField.DataValue == "null")
                    {
                        pDashboard.SubReporte.Fields.GetByConfigValue(lField.ConfigValue).DataValue = DBNull.Value;
                    }
                    else if (lField is KeytiaVarcharField || lField is KeytiaMultiSelectField)
                    {
                        lsDataValueDelimiter = lField.DSOControlDB.DataValueDelimiter;
                        lField.DSOControlDB.DataValueDelimiter = "";

                        pDashboard.SubReporte.Fields.GetByConfigValue(lField.ConfigValue).DataValue = lField.DataValue;

                        lField.DSOControlDB.DataValueDelimiter = lsDataValueDelimiter;
                    }
                    else
                    {
                        pDashboard.SubReporte.Fields.GetByConfigValue(lField.ConfigValue).DataValue = lField.DataValue;
                    }
                }
            }
            if (pReporte.bAreaGrafica)
            {
                pDashboard.SubReporte.OpcTipoGrafica.DataValue = pReporte.OpcTipoGrafica.DataValue;
            }
            if (pReporte.bAreaGraficaHis)
            {
                pDashboard.SubReporte.OpcTipoGraficaHis.DataValue = pReporte.OpcTipoGraficaHis.DataValue; 
            }
            if (pReporte.bAreaReporte)
            {
                pDashboard.SubReporte.GridReporte.TxtState.Text = pReporte.GridReporte.TxtState.Text;
            }

            GuardarConfiguracion();
            pDashboard.SetDashboardState(DashboardState.SubConsulta);
        }

        protected void pddlConsulta_DropDownListChange(object sender, EventArgs e)
        {
            Consulta = int.Parse(pddlConsulta.DataValue.ToString());
            InitConfigReporte();
            InitReporte();
            InitAreaBloque();
            GuardarConfiguracion();
        }

        protected void popcAreaBloque_SelectedIndexChanged(object sender, EventArgs e)
        {
            piAreaBloque = int.Parse(popcAreaBloque.DataValue.ToString());
            InitAreaBloque();
            GuardarConfiguracion();
        }

        protected void OpcTipoGrafica_SelectedIndexChanged(object sender, EventArgs e)
        {
            piRepTipoGrafica = int.Parse(pReporte.OpcTipoGrafica.DataValue.ToString());
            GuardarConfiguracion();
        }

        protected void OpcTipoGraficaHis_SelectedIndexChanged(object sender, EventArgs e)
        {
            piRepTipoGraficaHis = int.Parse(pReporte.OpcTipoGraficaHis.DataValue.ToString());
            GuardarConfiguracion();
        }

        protected void GuardarConfiguracion()
        {
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                Hashtable lHTValues = new Hashtable();
                int liCodRegistro = 0;

                if (pVisConfigPersonalizada.Rows.Count > 0)
                {
                    liCodRegistro = (int)pVisConfigPersonalizada.Rows[0]["iCodRegistro"];
                }

                lHTValues.Add("{BloqueDashboard}", piCodBloque);
                lHTValues.Add("{Usuar}", Session["iCodUsuario"]);
                lHTValues.Add("{Consul}", piCodConsulta);
                lHTValues.Add("{AreaBloque}", piAreaBloque);
                lHTValues.Add("{RepTipoGrafica}", piRepTipoGrafica);
                lHTValues.Add("{RepTipoGraficaHis}", piRepTipoGraficaHis);
                lHTValues.Add("dtIniVigencia", DateTime.Now);
                lHTValues.Add("dtFinVigencia", new DateTime(2079, 01, 01));
                lHTValues.Add("iCodUsuario", Session["iCodUsuario"]);

                if (liCodRegistro == 0)
                {
                    lCargasCOM.InsertaRegistro(lHTValues, "Historicos", "BloqueDashboard", "Configuración Personalizada", true, (int)Session["iCodUsuarioDB"], false);
                }
                else
                {
                    lCargasCOM.ActualizaRegistro("Historicos", "BloqueDashboard", "Configuración Personalizada", lHTValues, liCodRegistro, true, (int)Session["iCodUsuarioDB"], false);
                }
            }
            catch (Exception e)
            {
                Util.LogException(e);
            }
        }

        public void InitLanguage()
        {
            Fill();
            pReporte.InitLanguage();
        }

        public void Fill()
        {
            FilldllConsulta();
            FillopcAreaBloque();
        }

        protected void FilldllConsulta()
        {
            DataRow lDataRow;
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("value", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("text", typeof(string)));

            foreach (DataRow lVisRow in Collection.VisConsultas.Rows)
            {
                if ((int)lVisRow["Aplic"] == piCodConsulta
                    || (!NoSelRep && !Collection.ConsultasSeleccionadas.Contains((int)lVisRow["Aplic"])))
                {
                    lDataRow = lDataTable.NewRow();
                    lDataRow["value"] = lVisRow["Aplic"];
                    lDataRow["text"] = lVisRow["RepEstDesc"];
                    lDataTable.Rows.Add(lDataRow);
                }
            }

            DataView lViewData = lDataTable.DefaultView;
            DataTable lDataSource = lDataTable.Clone();
            lViewData.Sort = "text ASC, value ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                if (lViewRow["value"] != DBNull.Value
                    && lDataSource.Select("value = " + lViewRow["value"]).Length == 0)
                {
                    lDataSource.ImportRow(lViewRow.Row);
                }
            }

            pddlConsulta.DataSource = lDataSource;
            pddlConsulta.Fill();

            if (pddlConsulta.DropDownList.SelectedIndex > 0)
            {
                Consulta = int.Parse(pddlConsulta.DataValue.ToString());
            }
            else
            {
                pddlConsulta.DataValue = piCodConsulta;
            }
            pddlConsulta.DropDownList.Enabled = !NoSelRep;
        }

        protected void FillopcAreaBloque()
        {
            if (pReporte == null)
            {
                return;
            }

            DataRow lVisAreaBloqueRow;
            DataRow lDataRow;
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("value", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("text", typeof(string)));
            lDataTable.Columns.Add(new DataColumn("order", typeof(int)));

            if (pReporte.bAreaGrafica)
            {
                lVisAreaBloqueRow = Collection.VisAreaBloque.Select("Value = 1")[0];
                lDataRow = lDataTable.NewRow();
                lDataRow["value"] = lVisAreaBloqueRow["Value"];
                lDataRow["text"] = lVisAreaBloqueRow[Globals.GetCurrentLanguage()];
                lDataRow["order"] = lVisAreaBloqueRow["OrdenPre"];
                lDataTable.Rows.Add(lDataRow);
            }
            if (pReporte.bAreaGraficaHis)
            {
                lVisAreaBloqueRow = Collection.VisAreaBloque.Select("Value = 2")[0];
                lDataRow = lDataTable.NewRow();
                lDataRow["value"] = lVisAreaBloqueRow["Value"];
                lDataRow["text"] = lVisAreaBloqueRow[Globals.GetCurrentLanguage()];
                lDataRow["order"] = lVisAreaBloqueRow["OrdenPre"];
                lDataTable.Rows.Add(lDataRow);
            }
            if (pReporte.bAreaReporte)
            {
                lVisAreaBloqueRow = Collection.VisAreaBloque.Select("Value = 4")[0];
                lDataRow = lDataTable.NewRow();
                lDataRow["value"] = lVisAreaBloqueRow["Value"];
                lDataRow["text"] = lVisAreaBloqueRow[Globals.GetCurrentLanguage()];
                lDataRow["order"] = lVisAreaBloqueRow["OrdenPre"];
                lDataTable.Rows.Add(lDataRow);
            }

            DataView lViewData = lDataTable.DefaultView;
            DataTable lDataSource = lDataTable.Clone();
            lViewData.Sort = "order ASC, value ASC, text ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                if (lViewRow["value"] != DBNull.Value
                    && lDataSource.Select("value = " + lViewRow["value"]).Length == 0)
                {
                    lDataSource.ImportRow(lViewRow.Row);
                }
            }

            if (lDataSource.Rows.Count > 0)
            {
                popcAreaBloque.DataSource = lDataSource;
                popcAreaBloque.Fill();

                popcAreaBloque.DataValue = piAreaBloque;
            }
            if (lDataSource.Rows.Count == 1)
            {
                popcAreaBloque.Visible = false;
            }
            else
            {
                popcAreaBloque.Visible = true;
            }
        }
    }
}
