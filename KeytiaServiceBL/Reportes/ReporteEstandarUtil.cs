using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using KeytiaServiceBL.Alarmas;

namespace KeytiaServiceBL.Reportes
{
    public enum TipoReporte
    {
        Tabular,
        Resumido,
        Matricial
    }

    public enum OrdenZonasReporte
    {
        PrimeroGraficas,
        PrimeroReportes
    }

    public enum TipoGrafica
    {
        Pastel = 1,
        Barras = 2,
        Lineas = 4,
        Area = 8
    }

    public class ParametrosGrafica
    {
        protected DataTable pdtDatos;
        protected string psTitle = "";
        protected string[] psDataColumns;
        protected string[] psSeriesNames;
        protected string[] psSeriesIds;
        protected string psXColumn;
        protected string psXIdsColumn;
        protected string psXTitle = "";
        protected string psXFormat;
        protected string psYTitle = "";
        protected string psYFormat;
        protected bool pbShowLegend = true;
        protected TipoGrafica pTipoGrafica = TipoGrafica.Pastel;

        public DataTable Datos
        {
            get
            {
                return pdtDatos;
            }
            set
            {
                pdtDatos = value;
            }
        }

        public string Title
        {
            get
            {
                return psTitle;
            }
            set
            {
                psTitle = value;
            }
        }

        public string[] DataColumns
        {
            get
            {
                return psDataColumns;
            }
            set
            {
                psDataColumns = value;
            }
        }

        public string[] SeriesNames
        {
            get
            {
                return psSeriesNames;
            }
            set
            {
                psSeriesNames = value;
            }
        }

        public string[] SeriesIds
        {
            get
            {
                return psSeriesIds;
            }
            set
            {
                psSeriesIds = value;
            }
        }

        public string XColumn
        {
            get
            {
                return psXColumn;
            }
            set
            {
                psXColumn = value;
            }
        }

        public string XIdsColumn
        {
            get
            {
                return psXIdsColumn;
            }
            set
            {
                psXIdsColumn = value;
            }
        }

        public string XTitle
        {
            get
            {
                return psXTitle;
            }
            set
            {
                psXTitle = value;
            }
        }

        public string XFormat
        {
            get
            {
                return psXFormat;
            }
            set
            {
                psXFormat = value;
            }
        }

        public string YTitle
        {
            get
            {
                return psYTitle;
            }
            set
            {
                psYTitle = value;
            }
        }

        public string YFormat
        {
            get
            {
                return psYFormat;
            }
            set
            {
                psYFormat = value;
            }
        }

        public bool ShowLegend
        {
            get
            {
                return pbShowLegend;
            }
            set
            {
                pbShowLegend = value;
            }
        }

        public TipoGrafica TipoGrafica
        {
            get
            {
                return pTipoGrafica;
            }
            set
            {
                pTipoGrafica = value;
            }
        }
    }

    public class ReporteEstandarUtil
    {
        protected int piCodReporte;
        protected Hashtable pHTParam;
        protected Hashtable pHTParamDesc;

        protected DataSet pDSCampos;
        protected DataTable pKDBTypes;
        protected DataTable pVisParametros;
        protected DataTable pVisIdiomaCampos;

        protected DataRow pKDBRowReporte;
        protected DataRow pKDBRowDataSource;

        protected DataTable pTablaHeader;
        protected DataTable pTablaHeaderExp;
        protected DataTable pTablaParametros;
        protected DataTable pTablaReporte;  //La tabla justo como la regresa el DataSource
        protected DataTable pTablaGrid;     //La tabla para el grid ya con los datos procesados para la exportación
        protected DataTable pTablaTotales = null;
        protected DataTable pTablaTotalesGrid = null;
        protected DataRow pRowCliente;

        protected ParametrosGrafica pParametrosGrafica;
        protected ParametrosGrafica pParametrosGraficaHis;

        protected bool pbAreaGrafica;
        protected bool pbAreaGraficaHis;
        protected bool pbAreaReporte;
        protected bool pbAreaParametros;
        protected bool pbOrdenCol;
        protected bool pbParamFecIniFin;
        protected bool pbReajustarParam;
        protected bool pbModoDebug;
        protected bool pbParamNumReg;

        protected bool pbAutoFiltro = true;

        protected int piNumRegistros;
        protected string psMostrandoRegistros = null;

        protected TipoReporte pTipoReporte = TipoReporte.Tabular;
        protected Orientation pOrientacionZonas = Orientation.Horizontal;
        protected OrdenZonasReporte pOrdenZonas = OrdenZonasReporte.PrimeroGraficas;

        protected TipoGrafica pTipoGrafica = TipoGrafica.Pastel;
        protected TipoGrafica pTipoGraficaHis = TipoGrafica.Pastel;

        protected string psOperAgrupacion;

        protected StringBuilder psbQuery = new StringBuilder();
        protected KDBAccess pKDB = new KDBAccess();
        protected DateTime pVigencia = DateTime.Now;

        protected int piCodUsuario;
        protected int piCodPerfil;
        protected string pvchCodIdioma;
        public string Idioma
        {
            get { return pvchCodIdioma; }
        }
        protected string pvchCodMoneda;

        protected string psKeytiaWebFPath;
        protected string psStylePath;

        protected int piRenHeader;
        protected int piColHeader;

        protected int piRenParam;
        protected int piColParam;

        //Variables para dar formato
        protected string psPrefijoString = ""; //En el caso de exportar a Excel se ocupa agregar un ' al inicio de los strings para que los numeros marcados los reconozca bien

        protected KeytiaNumberFormats pIntegerField = new KeytiaNumberFormats();
        protected KeytiaNumberFormats pIntegerFormatField = new KeytiaNumberFormats();
        protected KeytiaNumberFormats pNumericField = new KeytiaNumberFormats();
        protected KeytiaNumberFormats pCurrencyField = new KeytiaNumberFormats();

        protected string psDateFormat;
        protected string psDateTimeFormat;


        protected EstiloTablaExcel pEstiloTablaExcel;
        protected EstiloTablaWord pEstiloTablaWord;

        //Variables para exportar
        protected WordAccess pWord;
        protected ExcelAccess pExcel;
        protected TxtFileAccess pTxt;

        public ReporteEstandarUtil(int liCodReporte, Hashtable lHTParam, Hashtable lHTParamDesc, string lsKeytiaWebFPath, string lsStylePath)
        {
            piCodReporte = liCodReporte;
            pHTParam = lHTParam;
            pHTParamDesc = lHTParamDesc;
            psKeytiaWebFPath = lsKeytiaWebFPath;
            psStylePath = lsStylePath;

            pKDB.FechaVigencia = pVigencia;

            InitConfig();
            InitHTParam();
            InitIdiomaCampos();
            InitNumRegistros();

            pIntegerField.SetFormatInfo("Enteros");
            pIntegerFormatField.SetFormatInfo("EnterosFormat");
            pNumericField.SetFormatInfo("Flotantes");
            pCurrencyField.SetFormatInfo(pvchCodMoneda);

            psDateFormat = GetMsgWeb("NetDateFormat");
            psDateTimeFormat = GetMsgWeb("NetDateTimeFormat");

        }

        public int iCodReporte
        {
            get
            {
                return piCodReporte;
            }
        }

        public bool bAutoFiltro
        {
            get
            {
                return pbAutoFiltro;
            }
            set
            {
                pbAutoFiltro = value;
            }
        }

        public DataSet DSCampos
        {
            get
            {
                return pDSCampos;
            }
        }

        public TipoReporte TipoReporte
        {
            get
            {
                return pTipoReporte;
            }
        }

        public DataTable TablaReporte
        {
            get
            {
                return pTablaReporte;
            }
        }

        public DataTable TablaGrid
        {
            get
            {
                return pTablaGrid;
            }
        }

        protected void InitConfig()
        {
            pKDBTypes = pKDB.GetHisRegByEnt("Types", "Tipos de Datos");
            pKDBRowReporte = pKDB.GetHisRegByEnt("RepEst", "", "iCodCatalogo = " + iCodReporte).Rows[0];
            pTipoReporte = (TipoReporte)Enum.Parse(typeof(TipoReporte), DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + pKDBRowReporte["iCodMaestro"]).ToString());

            int liBanderas = (int)pKDBRowReporte["{BanderasRepEstandar}"];

            pbAreaGrafica = (liBanderas & 1) == 1;
            pbAreaGraficaHis = (liBanderas & 2) == 2;
            pbAreaReporte = (liBanderas & 4) == 4;
            pbAreaParametros = (liBanderas & 8) == 8;
            pbOrdenCol = (liBanderas & 16) == 16;
            pbParamFecIniFin = (liBanderas & 32) == 32;
            pbReajustarParam = (liBanderas & 64) == 64;
            pbModoDebug = (liBanderas & 128) == 128;
            pbParamNumReg = (liBanderas & 256) == 256;

            pOrientacionZonas = (Orientation)pKDBRowReporte["{RepEstOrientacionZonas}"];
            pOrdenZonas = (OrdenZonasReporte)pKDBRowReporte["{RepEstOrdenZonas}"];

            if (pTipoReporte == TipoReporte.Tabular)
            {
                pKDBRowDataSource = pKDB.GetHisRegByEnt("DataSourceRep", "DataSource Reportes", "iCodCatalogo = " + pKDBRowReporte["{DataSourceRep}"]).Rows[0];
                pDSCampos = GetCamposTabular(iCodReporte);
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                if (pKDBRowReporte["{RepTipoResumen}"] == DBNull.Value
                    || int.Parse(pKDBRowReporte["{RepTipoResumen}"].ToString()) == 1)
                {
                    psOperAgrupacion = "with rollup";
                }
                else
                {
                    psOperAgrupacion = "with cube";
                }
                pKDBRowDataSource = pKDB.GetHisRegByEnt("DataSourceRep", "DataSource Reportes", "iCodCatalogo = " + pKDBRowReporte["{DataSourceRep}"]).Rows[0];
                pDSCampos = GetCamposResumido(iCodReporte);
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                pKDBRowDataSource = pKDB.GetHisRegByEnt("DataSourceRepMat", "DataSource Reportes", "iCodCatalogo = " + pKDBRowReporte["{DataSourceRepMat}"]).Rows[0];
                pDSCampos = GetCamposMatricial(iCodReporte);
            }
            GetCamposGr(pDSCampos, iCodReporte);
        }

        protected void InitHTParam()
        {
            //Metodo para complementar los valores de pHTParam
            //Parametros que no se agregaran automaticamente
            //lHTParam.Add("iCodUsuario", Session["iCodUsuario"]);
            //lHTParam.Add("iCodPerfil", Session["iCodPerfil"]);
            //lHTParam.Add("vchCodIdioma", pvchCodIdioma);
            //lHTParam.Add("Schema", DSODataContext.Schema);
            //lHTParam.Add("vchCodMoneda", Globals.GetCurrentCurrency());
            //lHTParam.Add("FechaIniRep", pdtInicio.DataValue);
            //lHTParam.Add("FechaFinRep", pdtFin.DataValue);
            if (!pHTParam.ContainsKey("iCodUsuario"))
            {
                throw new ArgumentException("No se encontró el parametro de iCodUsuario");
            }
            piCodUsuario = (int)pHTParam["iCodUsuario"];

            if (!pHTParam.ContainsKey("iCodPerfil"))
            {
                throw new ArgumentException("No se encontró el parametro de iCodPerfil");
            }
            piCodPerfil = (int)pHTParam["iCodPerfil"];

            if (!pHTParam.ContainsKey("vchCodIdioma"))
            {
                throw new ArgumentException("No se encontró el parametro de vchCodIdioma");
            }
            pvchCodIdioma = pHTParam["vchCodIdioma"].ToString();

            if (!pHTParam.ContainsKey("Schema"))
            {
                pHTParam.Add("Schema", DSODataContext.Schema);
            }
            if (!pHTParam.ContainsKey("vchCodMoneda"))
            {
                throw new ArgumentException("No se encontró el parametro de vchCodMoneda");
            }
            pvchCodMoneda = pHTParam["vchCodMoneda"].ToString();

            if (pbParamFecIniFin && !pHTParam.ContainsKey("FechaIniRep"))
            {
                throw new ArgumentException("No se encontró el parametro de FechaIniRep");
            }
            if (pbParamFecIniFin && !pHTParam.ContainsKey("FechaFinRep"))
            {
                throw new ArgumentException("No se encontró el parametro de FechaFinRep");
            }
            if (pbParamNumReg && !pHTParam.ContainsKey("NumRegReporte"))
            {
                pHTParam.Add("NumRegReporte", 10);
                pHTParamDesc.Add("NumRegReporte", 10);
            }

            //Los tipos de graficas son opcionales ya que si no se envian y se requieren entonces se tomaran los valores 
            //predeterminados del reporte
            //lHTParam.Add("TipoGrafica", (int)pOpcTipoGrafica.DataValue);
            //lHTParam.Add("TipoGraficaHis", (int)pOpcTipoGraficaHis.DataValue);
            if (!pHTParam.ContainsKey("TipoGrafica") && pKDBRowReporte["{RepTipoGrafica}"] != DBNull.Value)
            {
                pHTParam.Add("TipoGrafica", (int)pKDBRowReporte["{RepTipoGrafica}"]);
            }
            else if (!pHTParam.ContainsKey("TipoGrafica") && pKDBRowReporte["{RepTipoGrafica}"] == DBNull.Value)
            {
                pHTParam.Add("TipoGrafica", (int)pTipoGrafica);
            }

            if (!pHTParam.ContainsKey("TipoGraficaHis") && pKDBRowReporte["{RepTipoGraficaHis}"] != DBNull.Value)
            {
                pHTParam.Add("TipoGraficaHis", (int)pKDBRowReporte["{RepTipoGraficaHis}"]);
            }
            else if (!pHTParam.ContainsKey("TipoGraficaHis") && pKDBRowReporte["{RepTipoGraficaHis}"] == DBNull.Value)
            {
                pHTParam.Add("TipoGraficaHis", (int)pTipoGraficaHis);
            }

            pTipoGrafica = (TipoGrafica)pHTParam["TipoGrafica"];
            pTipoGraficaHis = (TipoGrafica)pHTParam["TipoGraficaHis"];

            ComplementaParametrosReporte();
            AgregarParamDataFields();
        }

        protected void ComplementaParametrosReporte()
        {
            //Los parametros del reporte que no se envien se reemplazaran con null
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('RepEstCampo','Parametros Reporte','" + pvchCodIdioma + "')]");
            psbQuery.AppendLine("where RepEst = " + piCodReporte);
            psbQuery.AppendLine("and dtIniVigencia <= '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and dtFinVigencia > '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("order by ParamRenglon, ParamColumna");

            pVisParametros = DSODataAccess.Execute(psbQuery.ToString());

            foreach (DataRow lRow in pVisParametros.Rows)
            {
                if (!pHTParam.ContainsKey(lRow["AtribCod"].ToString()))
                {
                    if (lRow["ControlesCod"] == "CompDateTime"
                        || lRow["ControlesCod"] == "CompDate"
                        || lRow["ControlesCod"] == "CompMonth"
                        || lRow["ControlesCod"] == "CompTime"
                        || lRow["ControlesCod"] == "CompNumber"
                        || lRow["ControlesCod"] == "CompInteger"
                        || lRow["ControlesCod"] == "CompIntegerFormat"
                        || lRow["ControlesCod"] == "CompCurrency")
                    {
                        pHTParam.Add(lRow["AtribCod"].ToString(), "= null");
                    }
                    else
                    {
                        pHTParam.Add(lRow["AtribCod"].ToString(), "null");
                    }
                }
                if (!pHTParamDesc.ContainsKey(lRow["AtribCod"].ToString()))
                {
                    pHTParamDesc.Add(lRow["AtribCod"].ToString(), "");
                }
            }
        }

        protected virtual void AgregarParamDataFields()
        {
            if (pTipoReporte == TipoReporte.Tabular)
            {
                AgregarParamDataFieldsTabular(pHTParam, pDSCampos);
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                AgregarParamDataFieldsResumido(pHTParam, pDSCampos, psOperAgrupacion);
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                AgregarParamDataFieldsMatricial(pHTParam, pDSCampos);
                GetValoresEjeX(pvchCodIdioma, pKDBRowReporte, pKDBRowDataSource, pDSCampos, pHTParam, pbReajustarParam);
            }
        }

        protected void AgregarParamGrid()
        {
            if (pTipoReporte == TipoReporte.Tabular)
            {
                AgregarParamGridTabular(pDSCampos, pHTParam);
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                AgregarParamGridResumido(pDSCampos, pHTParam);
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                AgregarParamGridMatricial(pDSCampos, pHTParam);
            }
        }

        public static void AgregarParamGridTabular(DataSet lDSCampos, Hashtable lHTParam)
        {
            //Obtengo orden default
            string lsOrderCol;
            string lsSortDir = null;
            string lsSortDirInv = null;
            Hashtable lHTClientCols = new Hashtable();
            List<string> lstOrdenColumnas = new List<string>();
            DataView lDataView = lDSCampos.Tables["Campos"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
                lHTClientCols.Add(lsOrderCol, lViewRow.Row);
                if (lsSortDir == null && int.Parse(lViewRow.Row["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsSortDir = "Asc";
                    lsSortDirInv = "Desc";
                }
                else if (lsSortDir == null)
                {
                    lsSortDir = "Desc";
                    lsSortDirInv = "Asc";
                }
            }

            StringBuilder lsbSortCol = new StringBuilder();
            StringBuilder lsbSortColInv = new StringBuilder();
            DataRow lRowCampo;
            foreach (string lsCol in lstOrdenColumnas)
            {
                if (lsbSortCol.Length > 0)
                {
                    lsbSortCol.Append(",");
                    lsbSortColInv.Append(",");
                }
                lRowCampo = lHTClientCols[lsCol] as DataRow;
                if (lRowCampo["{RepEstDirDatos}"] == DBNull.Value
                    || int.Parse(lRowCampo["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsbSortCol.Append("[" + lsCol + "] Asc");
                    lsbSortColInv.Append("[" + lsCol + "] Desc");
                }
                else
                {
                    lsbSortCol.Append("[" + lsCol + "] Desc");
                    lsbSortColInv.Append("[" + lsCol + "] Asc");
                }
            }

            if (lHTParam.ContainsKey("SortCol"))
            {
                lHTParam["SortCol"] = lsbSortCol.ToString();
            }
            else
            {
                lHTParam.Add("SortCol", lsbSortCol.ToString());
            }

            if (lHTParam.ContainsKey("SortColInv"))
            {
                lHTParam["SortColInv"] = lsbSortColInv.ToString();
            }
            else
            {
                lHTParam.Add("SortColInv", lsbSortColInv.ToString());
            }

            if (lsSortDir == null)
            {
                lsSortDir = "Asc";
                lsSortDirInv = "Desc";
            }

            if (lHTParam.ContainsKey("SortDir"))
            {
                lHTParam["SortDir"] = lsSortDir;
            }
            else
            {
                lHTParam.Add("SortDir", lsSortDir);
            }

            if (lHTParam.ContainsKey("SortDirInv"))
            {
                lHTParam["SortDirInv"] = lsSortDirInv;
            }
            else
            {
                lHTParam.Add("SortDirInv", lsSortDirInv);
            }
        }

        public static void AgregarParamGridResumido(DataSet lDSCampos, Hashtable lHTParam)
        {
            //Obtengo orden default
            string lsOrderCol;
            string lsSortDir = null;
            string lsSortDirInv = null;
            Hashtable lHTClientCols = new Hashtable();
            List<string> lstOrdenColumnas = new List<string>();
            DataView lDataView = lDSCampos.Tables["Campos"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
                lHTClientCols.Add(lsOrderCol, lViewRow.Row);
                if (lsSortDir == null && int.Parse(lViewRow.Row["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsSortDir = "Asc";
                    lsSortDirInv = "Desc";
                }
                else if (lsSortDir == null)
                {
                    lsSortDir = "Desc";
                    lsSortDirInv = "Asc";
                }
            }

            StringBuilder lsbSortCol = new StringBuilder();
            StringBuilder lsbSortColInv = new StringBuilder();
            DataRow lRowCampo;

            foreach (string lsCol in lstOrdenColumnas)
            {
                if (lsbSortCol.Length > 0)
                {
                    lsbSortCol.Append(",");
                    lsbSortColInv.Append(",");
                }
                lRowCampo = lHTClientCols[lsCol] as DataRow;
                if (lDSCampos.Tables["Agrupadores"].Select("iCodRegistro = " + lRowCampo["iCodRegistro"]).Length > 0)
                {
                    if (ReporteEstandarUtil.IsValidGroupByField(lRowCampo))
                    {
                        lsbSortCol.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, false) + " Asc,");
                        lsbSortColInv.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, false) + " Desc,");
                    }
                    if (lRowCampo["{DataFieldRuta}"] != DBNull.Value
                        && ReporteEstandarUtil.IsValidGroupByField(lRowCampo, "{DataFieldRuta}"))
                    {
                        lsbSortCol.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, "{DataFieldRuta}", false) + " Asc,");
                        lsbSortColInv.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, "{DataFieldRuta}", false) + " Desc,");
                    }
                }
                if (lRowCampo["{RepEstDirDatos}"] == DBNull.Value
                    || int.Parse(lRowCampo["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsbSortCol.Append("[" + lsCol + "] Asc");
                    lsbSortColInv.Append("[" + lsCol + "] Desc");
                }
                else
                {
                    lsbSortCol.Append("[" + lsCol + "] Desc");
                    lsbSortColInv.Append("[" + lsCol + "] Asc");
                }
            }

            if (lHTParam.ContainsKey("SortCol"))
            {
                lHTParam["SortCol"] = lsbSortCol.ToString();
            }
            else
            {
                lHTParam.Add("SortCol", lsbSortCol.ToString());
            }

            if (lHTParam.ContainsKey("SortColInv"))
            {
                lHTParam["SortColInv"] = lsbSortColInv.ToString();
            }
            else
            {
                lHTParam.Add("SortColInv", lsbSortColInv.ToString());
            }

            if (lsSortDir == null)
            {
                lsSortDir = "Asc";
                lsSortDirInv = "Desc";
            }

            if (lHTParam.ContainsKey("SortDir"))
            {
                lHTParam["SortDir"] = lsSortDir;
            }
            else
            {
                lHTParam.Add("SortDir", lsSortDir);
            }

            if (lHTParam.ContainsKey("SortDirInv"))
            {
                lHTParam["SortDirInv"] = lsSortDirInv;
            }
            else
            {
                lHTParam.Add("SortDirInv", lsSortDirInv);
            }
        }

        public static void AgregarParamGridMatricial(DataSet lDSCampos, Hashtable lHTParam)
        {
            //Obtengo orden default
            string lsOrderCol;
            string lsSortDir = null;
            string lsSortDirInv = null;
            Hashtable lHTClientCols = new Hashtable();
            List<string> lstOrdenColumnas = new List<string>();
            DataView lDataView = lDSCampos.Tables["CamposY"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
                lHTClientCols.Add(lsOrderCol, lViewRow.Row);
                if (lsSortDir == null && int.Parse(lViewRow.Row["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsSortDir = "Asc";
                    lsSortDirInv = "Desc";
                }
                else if (lsSortDir == null)
                {
                    lsSortDir = "Desc";
                    lsSortDirInv = "Asc";
                }
            }
            lDataView = lDSCampos.Tables["TotalizadosXYTotX"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
                lHTClientCols.Add(lsOrderCol, lViewRow.Row);
            }

            StringBuilder lsbSortCol = new StringBuilder();
            StringBuilder lsbSortColInv = new StringBuilder();
            DataRow lRowCampo;
            foreach (string lsCol in lstOrdenColumnas)
            {
                if (lsbSortCol.Length > 0)
                {
                    lsbSortCol.Append(",");
                    lsbSortColInv.Append(",");
                }
                lRowCampo = lHTClientCols[lsCol] as DataRow;
                if (lRowCampo["{RepEstDirDatos}"] == DBNull.Value
                    || int.Parse(lRowCampo["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsbSortCol.Append("[" + lsCol + "] Asc");
                    lsbSortColInv.Append("[" + lsCol + "] Desc");
                }
                else
                {
                    lsbSortCol.Append("[" + lsCol + "] Desc");
                    lsbSortColInv.Append("[" + lsCol + "] Asc");
                }
            }

            if (lHTParam.ContainsKey("SortCol"))
            {
                lHTParam["SortCol"] = lsbSortCol.ToString();
            }
            else
            {
                lHTParam.Add("SortCol", lsbSortCol.ToString());
            }

            if (lHTParam.ContainsKey("SortColInv"))
            {
                lHTParam["SortColInv"] = lsbSortColInv.ToString();
            }
            else
            {
                lHTParam.Add("SortColInv", lsbSortColInv.ToString());
            }

            if (lsSortDir == null)
            {
                lsSortDir = "Asc";
                lsSortDirInv = "Desc";
            }

            if (lHTParam.ContainsKey("SortDir"))
            {
                lHTParam["SortDir"] = lsSortDir;
            }
            else
            {
                lHTParam.Add("SortDir", lsSortDir);
            }

            if (lHTParam.ContainsKey("SortDirInv"))
            {
                lHTParam["SortDirInv"] = lsSortDirInv;
            }
            else
            {
                lHTParam.Add("SortDirInv", lsSortDirInv);
            }
        }

        protected void InitIdiomaCampos()
        {
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('RepEstIdiomaCmp','Idioma Campos','" + pvchCodIdioma + "')] a");
            psbQuery.AppendLine("where a.iCodCatalogo in(select RepEstIdiomaCmp from " + DSODataContext.Schema + ".[VisHistoricos('RepEstCampo','" + pvchCodIdioma + "')] b");
            psbQuery.AppendLine("                   where b.RepEst = " + piCodReporte);
            psbQuery.AppendLine("                   and b.dtIniVigencia <= '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("                   and b.dtFinVigencia > '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            psbQuery.AppendLine("and a.dtIniVigencia <= '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and a.dtFinVigencia > '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            pVisIdiomaCampos = DSODataAccess.Execute(psbQuery.ToString());
        }

        protected void InitNumRegistros()
        {
            pHTParam.Add("iDisplayStart", 0);
            pHTParam.Add("iDisplayLength", 100);

            string lsRepEstDataSourceNumReg = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceNumReg}"].ToString(), pHTParam, pbReajustarParam);
            if (pbModoDebug)
            {
                ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceNumReg", lsRepEstDataSourceNumReg);
            }
            piNumRegistros = int.Parse(Util.IsDBNull(DSODataAccess.ExecuteScalar(lsRepEstDataSourceNumReg), 0).ToString());

            pHTParam["iDisplayLength"] = piNumRegistros;

        }

        protected void InitDatos()
        {
            if (pbAreaReporte)
            {
                InitReporte();
            }
            if (pbAreaGrafica)
            {
                InitGrafica();
            }
            if (pbAreaGraficaHis)
            {
                InitGraficaHis();
            }
            InitHeader();
            InitDatosCliente();
        }

        protected void InitReporte()
        {
            AgregarParamGrid();

            string lsRepEstDataSource = ParseDataSource(pKDBRowDataSource["{RepEstDataSource}"].ToString(), pHTParam, pbReajustarParam);
            if (pbModoDebug)
            {
                ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSource", lsRepEstDataSource);
            }
            pTablaReporte = DSODataAccess.Execute(lsRepEstDataSource);

            if (((pTipoReporte == TipoReporte.Tabular || pTipoReporte == TipoReporte.Resumido)
                && pDSCampos.Tables["Totalizados"].Rows.Count > 0)
                || (pTipoReporte == TipoReporte.Matricial
                && (pDSCampos.Tables["TotalizadosY"].Rows.Count > 0
                || pDSCampos.Tables["TotalizadosXYTotX"].Rows.Count > 0
                || pDSCampos.Tables["TotalizadosXYTotY"].Rows.Count > 0)))
            {
                string lsRepEstDataSourceTot = ParseDataSource(pKDBRowDataSource["{RepEstDataSourceTot}"].ToString(), pHTParam, pbReajustarParam);
                if (pbModoDebug)
                {
                    ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceTot", lsRepEstDataSourceTot);
                }
                pTablaTotales = DSODataAccess.Execute(lsRepEstDataSourceTot);
            }
        }

        protected void InitGrafica()
        {
            pParametrosGrafica = InitChart(pTipoGrafica, "");
        }

        protected void InitGraficaHis()
        {
            pParametrosGraficaHis = InitChart(pTipoGraficaHis, "His");
        }

        protected ParametrosGrafica InitChart(TipoGrafica lTipoGrafica, string lsHis)
        {
            ParametrosGrafica lParametrosGrafica = new ParametrosGrafica();

            string lsTitle = "";
            StringBuilder lsbDataFields = new StringBuilder();
            string lsGroupByField = "";

            List<string> lstDataColumns = new List<string>();
            List<string> lstSeriesNames = new List<string>();
            List<string> lstSeriesIds = new List<string>();
            string lsXColumn = null;
            string lsXIdsColumn = null;
            string lsXTitle = "";
            string lsXFormat = null;
            string lsYTitle = "";
            string lsYFormat = null;
            bool lbShowLegend = true;
            DataRow lKDBRowTitulos;
            string lsSerieName;

            if (pKDBRowReporte["{RepEstTitGr" + lsHis + "}"] != DBNull.Value)
            {
                lKDBRowTitulos = pKDB.GetHisRegByEnt("RepEstTitGr" + lsHis, "Idioma", "iCodCatalogo = " + pKDBRowReporte["{RepEstTitGr" + lsHis + "}"]).Rows[0];
                if (lKDBRowTitulos["{" + pvchCodIdioma + "}"] != DBNull.Value)
                {
                    lsTitle = lKDBRowTitulos["{" + pvchCodIdioma + "}"].ToString();
                }
                if (lKDBRowTitulos["{RepEstTitEjeX" + lsHis + "}"] != DBNull.Value)
                {
                    lsXTitle = pKDB.GetHisRegByEnt("RepEstTitEjeX" + lsHis, "Idioma", "iCodCatalogo = " + lKDBRowTitulos["{RepEstTitEjeX" + lsHis + "}"]).Rows[0]["{" + pvchCodIdioma + "}"].ToString();
                }
                if (lKDBRowTitulos["{RepEstTitEjeY" + lsHis + "}"] != DBNull.Value)
                {
                    lsYTitle = pKDB.GetHisRegByEnt("RepEstTitEjeY" + lsHis, "Idioma", "iCodCatalogo = " + lKDBRowTitulos["{RepEstTitEjeY" + lsHis + "}"]).Rows[0]["{" + pvchCodIdioma + "}"].ToString();
                }
                if (lKDBRowTitulos["{LeyendaGrafica" + lsHis + "}"] != DBNull.Value)
                {
                    lbShowLegend = ((int)lKDBRowTitulos["{LeyendaGrafica" + lsHis + "}"] & (int)lTipoGrafica) == (int)lTipoGrafica;
                }
            }

            DataTable lDataOrder = pDSCampos.Tables["CamposGr" + lsHis].Clone();
            foreach (DataRow lRow in pDSCampos.Tables["CamposGr" + lsHis].Rows)
            {
                //Si el campo aplica para el tipo de grafica seleccionada
                if (((int)lRow["{BanderasCamposGr" + lsHis + "}"] & (int)lTipoGrafica) == (int)lTipoGrafica)
                {
                    lDataOrder.ImportRow(lRow);
                    if (lsbDataFields.Length > 0)
                    {
                        lsbDataFields.AppendLine(",");
                    }
                    lsbDataFields.Append(lRow["{DataField}"].ToString());
                    if (lRow["{DataFieldRuta}"] != DBNull.Value)
                    {
                        lsbDataFields.AppendLine(",");
                        lsbDataFields.Append(lRow["{DataFieldRuta}"].ToString());
                    }

                    if ((int)lRow["{TipoCampoGr" + lsHis + "}"] == 1) //Si es de tipo serie de datos
                    {
                        lstSeriesIds.Add(lRow["{Atrib}"].ToString());

                        lsSerieName = pKDB.GetHisRegByEnt("RepEstIdiomaCmp", "Idioma Campos", "iCodCatalogo = " + lRow["{RepEstIdiomaCmp}"]).Rows[0]["{" + pvchCodIdioma + "}"].ToString();
                        lstSeriesNames.Add(lsSerieName);

                        lstDataColumns.Add(ReporteEstandarUtil.GetDataFieldName(lRow));

                        //Se estable el formato de la primer serie de datos que tenga un tipo de dato definido
                        if (String.IsNullOrEmpty(lsYFormat) && lRow["{Types}"] != DBNull.Value)
                        {
                            lsYFormat = GetFormatString((int)lRow["{Types}"]);
                        }
                    }
                    else if ((int)lRow["{TipoCampoGr" + lsHis + "}"] == 2) //Si es de tipo valores eje x
                    {
                        lsXColumn = ReporteEstandarUtil.GetDataFieldName(lRow);
                        lsXIdsColumn = lsXColumn;

                        //Se establece el formato del eje X si se tiene un tipo de datos definido
                        if (String.IsNullOrEmpty(lsXFormat) && lRow["{Types}"] != DBNull.Value)
                        {
                            lsXFormat = GetFormatString((int)lRow["{Types}"]);
                        }

                        lsGroupByField = "";
                        if (ReporteEstandarUtil.IsValidGroupByField(lRow))
                        {
                            lsGroupByField = ReporteEstandarUtil.GetGroupField(lRow);
                        }

                        if (lRow["{DataFieldRuta}"] != DBNull.Value
                            && ReporteEstandarUtil.IsValidGroupByField(lRow, "{DataFieldRuta}"))
                        {
                            lsXIdsColumn = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRuta}");
                            if (!String.IsNullOrEmpty(lsGroupByField))
                            {
                                lsGroupByField = lsGroupByField + "," + ReporteEstandarUtil.GetGroupField(lRow, "{DataFieldRuta}");
                            }
                            else
                            {
                                lsGroupByField = ReporteEstandarUtil.GetGroupField(lRow, "{DataFieldRuta}");
                            }
                        }
                    }
                }
            }
            string lsSortDir;
            string lsSortDirInv;
            pHTParam.Add("DataFieldsGr" + lsHis, lsbDataFields.ToString());
            pHTParam.Add("GroupByFieldGr" + lsHis, lsGroupByField);
            pHTParam.Add("OrderByFieldsGr" + lsHis, ReporteEstandarUtil.GetOrderByFields(lDataOrder, 1, out lsSortDir));
            pHTParam.Add("OrderByFieldsGr" + lsHis + "Inv", ReporteEstandarUtil.GetOrderByFields(lDataOrder, 2, out lsSortDirInv));
            pHTParam.Add("SortDirGr" + lsHis, lsSortDir);
            pHTParam.Add("SortDirGr" + lsHis + "Inv", lsSortDirInv);

            string lsDataSource = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceGr" + lsHis + "}"].ToString(), pHTParam, pbReajustarParam);
            if (pbModoDebug)
            {
                ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceGr" + lsHis, lsDataSource);
            }
            lParametrosGrafica.Datos = DSODataAccess.Execute(lsDataSource);
            lParametrosGrafica.Title = lsTitle;
            lParametrosGrafica.DataColumns = lstDataColumns.ToArray();
            lParametrosGrafica.SeriesNames = lstSeriesNames.ToArray();
            lParametrosGrafica.SeriesIds = lstSeriesIds.ToArray();
            lParametrosGrafica.XColumn = lsXColumn;
            lParametrosGrafica.XIdsColumn = lsXIdsColumn;
            lParametrosGrafica.XTitle = lsXTitle;
            lParametrosGrafica.XFormat = lsXFormat;
            lParametrosGrafica.YTitle = lsYTitle;
            lParametrosGrafica.YFormat = lsYFormat;
            lParametrosGrafica.ShowLegend = lbShowLegend;
            lParametrosGrafica.TipoGrafica = lTipoGrafica;

            return lParametrosGrafica;
        }

        protected void InitHeader()
        {
            if (pKDBRowDataSource["{RepEstDataSourceHeader}"] != DBNull.Value)
            {
                string lsDataSource = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceHeader}"].ToString(), pHTParam, pbReajustarParam);
                if (pbModoDebug)
                {
                    ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceHeader", lsDataSource);
                }
                pTablaHeader = DSODataAccess.Execute(lsDataSource);
            }

            pTablaHeaderExp = new DataTable();
            DataRow lHeaderRow;
            bool lbInsertarFila = true;
            if (pbParamFecIniFin)
            {
                if (pVisParametros.Select("AtribCod = 'FechaIniRep'").Length == 0)
                {
                    lbInsertarFila = false;
                    pTablaHeaderExp.Columns.Add(new DataColumn("Col" + pTablaHeaderExp.Columns.Count, typeof(string)));
                    pTablaHeaderExp.Columns.Add(new DataColumn("Col" + pTablaHeaderExp.Columns.Count, typeof(string)));
                    pTablaHeaderExp.Rows.InsertAt(pTablaHeaderExp.NewRow(), 0);
                    pTablaHeaderExp.Rows[0][0] = Alarma.GetLangItem(pvchCodIdioma, "Atrib", "Atributos", "FechaIniRep");
                    pTablaHeaderExp.Rows[0][1] = pHTParamDesc["FechaIniRep"].ToString();
                }
                if (pVisParametros.Select("AtribCod = 'FechaFinRep'").Length == 0)
                {
                    pTablaHeaderExp.Columns.Add(new DataColumn("Col" + pTablaHeaderExp.Columns.Count, typeof(string)));
                    pTablaHeaderExp.Columns.Add(new DataColumn("Col" + pTablaHeaderExp.Columns.Count, typeof(string)));
                    if (lbInsertarFila)
                    {
                        pTablaHeaderExp.Rows.InsertAt(pTablaHeaderExp.NewRow(), 0);
                    }
                    pTablaHeaderExp.Rows[0][2] = Alarma.GetLangItem(pvchCodIdioma, "Atrib", "Atributos", "FechaFinRep");
                    pTablaHeaderExp.Rows[0][3] = pHTParamDesc["FechaFinRep"].ToString();
                }
            }
            if (pbParamNumReg)
            {
                if (pVisParametros.Select("AtribCod = 'NumRegReporte'").Length == 0)
                {
                    pTablaHeaderExp.Columns.Add(new DataColumn("Col" + pTablaHeaderExp.Columns.Count, typeof(string)));
                    pTablaHeaderExp.Columns.Add(new DataColumn("Col" + pTablaHeaderExp.Columns.Count, typeof(string)));
                    if (lbInsertarFila)
                    {
                        pTablaHeaderExp.Rows.InsertAt(pTablaHeaderExp.NewRow(), 0);
                    }
                    pTablaHeaderExp.Rows[0][4] = Alarma.GetLangItem(pvchCodIdioma, "Atrib", "Atributos", "NumRegReporte");
                    pTablaHeaderExp.Rows[0][5] = pHTParamDesc["NumRegReporte"].ToString();
                }
            }
            if (pTablaHeader != null && pTablaHeader.Rows.Count > 0)
            {
                while (pTablaHeaderExp.Columns.Count < pTablaHeader.Columns.Count)
                {
                    pTablaHeaderExp.Columns.Add(new DataColumn("Col" + pTablaHeaderExp.Columns.Count, typeof(string)));
                }

                foreach (DataRow lRow in pTablaHeader.Rows)
                {
                    lHeaderRow = pTablaHeaderExp.NewRow();
                    for (int lidx = 0; lidx < pTablaHeader.Columns.Count; lidx++)
                    {
                        lHeaderRow[lidx] = lRow[lidx].ToString();
                    }
                    pTablaHeaderExp.Rows.Add(lHeaderRow);
                }
            }
        }

        protected void InitDatosCliente()
        {
            int liCodEmpresa;
            int liCodCliente;
            DataTable ldt;

            pRowCliente = null;

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','" + pvchCodIdioma + "')] Usuar");
            psbQuery.AppendLine("where Usuar.iCodCatalogo = " + piCodUsuario);
            psbQuery.AppendLine("and Usuar.dtIniVigencia <= '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Usuar.dtFinVigencia > '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            ldt = DSODataAccess.Execute(psbQuery.ToString());
            if (ldt.Rows.Count == 0)
            {
                return;
            }

            liCodEmpresa = (int)ldt.Rows[0]["Empre"];

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + pvchCodIdioma + "')] Empre");
            psbQuery.AppendLine("where Empre.iCodCatalogo = " + liCodEmpresa);
            psbQuery.AppendLine("and Empre.dtIniVigencia <= '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Empre.dtFinVigencia > '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt.Rows.Count == 0)
            {
                return;
            }

            liCodCliente = (int)ldt.Rows[0]["Client"];

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','" + pvchCodIdioma + "')] Client");
            psbQuery.AppendLine("where Client.iCodCatalogo = " + liCodCliente);
            psbQuery.AppendLine("and Client.dtIniVigencia <= '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Client.dtFinVigencia > '" + pVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt.Rows.Count == 0)
            {
                return;
            }

            pRowCliente = ldt.Rows[0];
        }

        protected void InitParametros()
        {
            pTablaParametros = new DataTable();
            if (!pbAreaParametros)
            {
                return;
            }
            DataRow lRowParam;
            int liParamRenglon;
            int liParamColumna;
            int liParamColSpan;
            int liColDesc = 0;
            string lsParamCod;

            foreach (DataRow ldataRow in pVisParametros.Rows)
            {
                lsParamCod = ldataRow["AtribCod"].ToString();
                if (!String.IsNullOrEmpty(pHTParamDesc[lsParamCod].ToString())
                    && pHTParam[lsParamCod].ToString() != "null")
                {
                    liParamRenglon = (int)ldataRow["ParamRenglon"];
                    liParamColumna = (int)ldataRow["ParamColumna"];
                    liParamColSpan = (int)ldataRow["ParamColSpan"];
                    if (pTablaParametros.Rows.Count < liParamRenglon)
                    {
                        liColDesc = 0;
                        lRowParam = pTablaParametros.NewRow();
                        pTablaParametros.Rows.Add(lRowParam);
                    }
                    else
                    {
                        lRowParam = pTablaParametros.Rows[liParamRenglon - 1];
                    }

                    while (pTablaParametros.Columns.Count < liColDesc + liParamColSpan + 1)
                    {
                        pTablaParametros.Columns.Add(new DataColumn("Col" + pTablaParametros.Columns.Count, typeof(string)));
                    }

                    lRowParam[liColDesc] = ldataRow["RepEstIdiomaCmpDesc"].ToString();
                    lRowParam[liColDesc + 1] = pHTParamDesc[lsParamCod];

                    liColDesc = liColDesc + liParamColSpan + 1;
                }
            }
        }

        protected string GetMsgWeb(string lsElemento, params object[] lsParam)
        {
            return GetLangItem(pvchCodIdioma, "MsgWeb", "Mensajes Web", lsElemento, lsParam);
        }

        protected virtual string GetFormatString(int iCodType)
        {
            DataRow lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + iCodType)[0];
            string lsFormatString = null;

            if (lKDBTypeRow["vchCodigo"].ToString() == "Int")
            {
                lsFormatString = pIntegerField.StringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "IntFormat")
            {
                lsFormatString = pIntegerFormatField.StringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "Float")
            {
                lsFormatString = pNumericField.StringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "Currency")
            {
                lsFormatString = pCurrencyField.OfficeStringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "Date")
            {
                lsFormatString = psDateFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "DateTime")
            {
                lsFormatString = psDateTimeFormat;
            }

            return lsFormatString;
        }

        protected void ProcesarTablaGridTabular()
        {
            pTablaGrid = new DataTable();
            string lsEncabezado;
            string lsDataField;

            while (pTablaGrid.Rows.Count < pTablaReporte.Rows.Count)
            {
                pTablaGrid.Rows.Add(pTablaGrid.NewRow());
            }

            foreach (DataRow lRowCampo in pDSCampos.Tables["Campos"].Rows)
            {
                lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                pTablaGrid.Columns.Add(new DataColumn(lsEncabezado, typeof(string)));

                lsDataField = GetDataFieldName(lRowCampo);
                for (int li = 0; li < pTablaReporte.Rows.Count; li++)
                {
                    pTablaGrid.Rows[li][lsEncabezado] = FormatearValor(lRowCampo, pTablaReporte.Rows[li][lsDataField]);
                }
            }
        }

        protected void ProcesarTablaGridResumido()
        {
            ProcesarTablaGridTabular();

            //Procesar subtotales de resumen
            string lsEncabezado;
            string lsDataField;
            bool lbProcesarRuta;
            string lsGroupingField;
            string lsSubTotalResumen = GetMsgWeb("SubTotalResumen");
            foreach (DataRow lRowCampo in pDSCampos.Tables["Agrupadores"].Rows)
            {
                lbProcesarRuta = true;
                lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                lsDataField = GetDataFieldName(lRowCampo);
                lsGroupingField = GetGroupingFieldName(lRowCampo);
                if (pTablaReporte.Columns.Contains(lsGroupingField))
                {
                    lbProcesarRuta = false;
                    for (int li = 0; li < pTablaReporte.Rows.Count; li++)
                    {
                        if (int.Parse(pTablaReporte.Rows[li][lsGroupingField].ToString()) == 1)
                        {
                            pTablaGrid.Rows[li][lsEncabezado] = lsSubTotalResumen;
                        }
                    }
                }
                if (lbProcesarRuta && lRowCampo["{DataFieldRuta}"] != DBNull.Value)
                {
                    lsGroupingField = GetGroupingFieldName(lRowCampo, "{DataFieldRuta}");
                    if (pTablaReporte.Columns.Contains(lsGroupingField))
                    {
                        lbProcesarRuta = false;
                        for (int li = 0; li < pTablaReporte.Rows.Count; li++)
                        {
                            if (int.Parse(pTablaReporte.Rows[li][lsGroupingField].ToString()) == 1)
                            {
                                pTablaGrid.Rows[li][lsEncabezado] = lsSubTotalResumen;
                            }
                        }
                    }
                }
            }
        }

        protected void ProcesarTablaGridMatricial()
        {
            pTablaGrid = new DataTable();
            int lidx;
            string lsEncabezado;
            string lsDataField;

            while (pTablaGrid.Rows.Count < pTablaReporte.Rows.Count)
            {
                pTablaGrid.Rows.Add(pTablaGrid.NewRow());
            }

            foreach (DataRow lRowCampo in pDSCampos.Tables["CamposY"].Rows)
            {
                lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                pTablaGrid.Columns.Add(new DataColumn(lsEncabezado, typeof(string)));

                lsDataField = GetDataFieldName(lRowCampo);
                for (int li = 0; li < pTablaReporte.Rows.Count; li++)
                {
                    pTablaGrid.Rows[li][lsEncabezado] = FormatearValor(lRowCampo, pTablaReporte.Rows[li][lsDataField]);
                }
            }
            for (lidx = 0; lidx < pDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
            {
                foreach (DataRow lRowCampo in pDSCampos.Tables["CamposXY"].Rows)
                {
                    lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                    lsEncabezado = lsEncabezado + (lidx + 1);
                    pTablaGrid.Columns.Add(new DataColumn(lsEncabezado, typeof(string)));

                    lsDataField = "ColX" + lidx + "_" + GetDataFieldName(lRowCampo);
                    for (int li = 0; li < pTablaReporte.Rows.Count; li++)
                    {
                        pTablaGrid.Rows[li][lsEncabezado] = FormatearValor(lRowCampo, pTablaReporte.Rows[li][lsDataField]);
                    }
                }
            }
            foreach (DataRow lRowCampo in pDSCampos.Tables["TotalizadosXYTotX"].Rows)
            {
                lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                pTablaGrid.Columns.Add(new DataColumn(lsEncabezado, typeof(string)));

                lsDataField = GetDataFieldName(lRowCampo);
                for (int li = 0; li < pTablaReporte.Rows.Count; li++)
                {
                    pTablaGrid.Rows[li][lsEncabezado] = FormatearValor(lRowCampo, pTablaReporte.Rows[li][lsDataField]);
                }
            }
        }

        protected bool ProcesarTotalesGridTabular()
        {
            return ProcesarTotalesGridTabular(true);
        }

        protected bool ProcesarTotalesGridTabular(bool lbInsertInTablaGrid)
        {
            bool lbRet = false;
            string lsEncabezado;
            string lsDataField;
            if (pTablaTotales != null && pTablaTotales.Rows.Count > 0)
            {
                pTablaTotalesGrid = pTablaGrid.Clone();
                lbRet = true;
                DataRow lRowGrid = pTablaTotalesGrid.NewRow();
                DataRow lRowTotales = pTablaTotales.Rows[0];
                pTablaTotalesGrid.Rows.Add(lRowGrid);
                foreach (DataRow lRowCampo in pDSCampos.Tables["Campos"].Rows)
                {
                    lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                    lsDataField = GetDataFieldName(lRowCampo);
                    if (pTablaTotales.Columns.Contains(lsDataField))
                    {
                        lRowGrid[lsEncabezado] = FormatearValor(lRowCampo, lRowTotales[lsDataField]);
                    }
                }
                foreach (DataColumn lDataCol in pTablaTotalesGrid.Columns)
                {
                    if (lRowGrid[lDataCol] == DBNull.Value || String.IsNullOrEmpty(lRowGrid[lDataCol].ToString()))
                    {
                        lRowGrid[lDataCol] = GetMsgWeb("Total");
                        break;
                    }
                }
                if (lbInsertInTablaGrid)
                {
                    pTablaGrid.ImportRow(lRowGrid);
                }
            }

            return lbRet;
        }

        protected bool ProcesarTotalesGridMatricial()
        {
            return ProcesarTotalesGridMatricial(true);
        }

        protected bool ProcesarTotalesGridMatricial(bool lbInsertInTablaGrid)
        {
            bool lbRet = false;
            string lsEncabezado;
            string lsDataField;
            if (pTablaTotales != null && pTablaTotales.Rows.Count > 0)
            {
                pTablaTotalesGrid = pTablaGrid.Clone();
                lbRet = true;
                DataRow lRowGrid = pTablaTotalesGrid.NewRow();
                DataRow lRowTotales = pTablaTotales.Rows[0];
                pTablaTotalesGrid.Rows.Add(lRowGrid);
                foreach (DataRow lRowCampo in pDSCampos.Tables["TotalizadosY"].Rows)
                {
                    lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                    lsDataField = GetDataFieldName(lRowCampo);
                    if (pTablaTotales.Columns.Contains(lsDataField))
                    {
                        lRowGrid[lsEncabezado] = FormatearValor(lRowCampo, lRowTotales[lsDataField]);
                    }
                }
                int lidx;
                for (lidx = 0; lidx < pDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
                {
                    foreach (DataRow lRowCampo in pDSCampos.Tables["CamposXY"].Rows)
                    {
                        lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                        lsEncabezado = lsEncabezado + (lidx + 1);

                        lsDataField = "ColX" + lidx + "_" + GetDataFieldName(lRowCampo);
                        if (pTablaTotales.Columns.Contains(lsDataField))
                        {
                            lRowGrid[lsEncabezado] = FormatearValor(lRowCampo, lRowTotales[lsDataField]);
                        }
                    }
                }
                foreach (DataRow lRowCampo in pDSCampos.Tables["TotalizadosXYTotX"].Rows)
                {
                    lsEncabezado = pVisIdiomaCampos.Select("iCodCatalogo = " + lRowCampo["{RepEstIdiomaCmp}"])[0][pvchCodIdioma].ToString();
                    lsDataField = GetDataFieldName(lRowCampo);
                    if (pTablaTotales.Columns.Contains(lsDataField))
                    {
                        lRowGrid[lsEncabezado] = FormatearValor(lRowCampo, lRowTotales[lsDataField]);
                    }
                }

                foreach (DataColumn lDataCol in pTablaTotalesGrid.Columns)
                {
                    if (lRowGrid[lDataCol] == DBNull.Value || String.IsNullOrEmpty(lRowGrid[lDataCol].ToString()))
                    {
                        lRowGrid[lDataCol] = GetMsgWeb("Total");
                        break;
                    }
                }
                if (lbInsertInTablaGrid)
                {
                    pTablaGrid.ImportRow(lRowGrid);
                }
            }

            return lbRet;
        }

        public string FormatearValor(DataRow lKDBRowCampo, object lValor)
        {
            string lsRet = "";
            DataRow lKDBTypeRow;

            if (lKDBRowCampo["{Types}"] != DBNull.Value)
            {
                lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lKDBRowCampo["{Types}"])[0];
            }
            else
            {
                lKDBTypeRow = null;
            }

            if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Int")
            {
                if (lValor == DBNull.Value)
                {
                    lValor = 0;
                }
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pIntegerField.StringFormat, pIntegerField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "IntFormat")
            {
                if (lValor == DBNull.Value)
                {
                    lValor = 0;
                }
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pIntegerFormatField.StringFormat, pIntegerFormatField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Float")
            {
                if (lValor == DBNull.Value)
                {
                    lValor = 0.0;
                }
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pNumericField.StringFormat, pNumericField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Currency")
            {
                if (lValor == DBNull.Value)
                {
                    lValor = 0.0;
                }
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pCurrencyField.StringFormat, pCurrencyField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Date")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, psDateFormat);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "DateTime")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, psDateTimeFormat);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "TimeSeg")
            {
                if (lValor == DBNull.Value)
                {
                    lValor = 0;
                }
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, "TimeSeg");
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Time")
            {
                if (lValor == DBNull.Value)
                {
                    lValor = 0;
                }
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, "Time");
            }
            else
            {
                lsRet = psPrefijoString + lValor.ToString();
            }

            return lsRet;
        }

        #region Excel

        public ExcelAccess ExportXLS()
        {
            ExcelAccess lExcel = null;
            try
            {
                lExcel = new ExcelAccess();
                lExcel.FilePath = System.IO.Path.Combine(psStylePath, @"plantillas\reportes\Plantilla Reporte Estandar.xlsx");
                lExcel.Abrir();
                ExportXLS(lExcel);
            }
            catch (Exception e)
            {
                if (lExcel != null)
                {
                    lExcel.Cerrar(true);
                    lExcel.Dispose();
                    lExcel = null;
                }
                throw new Exception("", e);
            }

            return lExcel;
        }

        public void ExportXLS(ExcelAccess lExcel)
        {
            pExcel = lExcel;
            pExcel.XmlPalettePath = System.IO.Path.Combine(psStylePath, @"chart.xml");

            psPrefijoString = "'";
            InitMaxRegExcel();
            InitDatos();
            InitParametrosExcel();

            ProcesarHeaderExcel();
            ProcesarParametrosExcel();

            Hashtable lhtMetaTitulo = BuscarTituloExcel();

            AjustarHeaderExcel();
            AjustarParametrosExcel();
            ProcesarDatosReporteExcel();

            AjustarHeaderExcel();
            AjustarParametrosExcel();
            ProcesarTituloExcel(lhtMetaTitulo);
            ProcesarHojasExcel();
        }

        protected void InitMaxRegExcel()
        {
            int liMaxRegExcel = int.MaxValue; //20000;
            pHTParam["iDisplayLength"] = Math.Min(piNumRegistros, liMaxRegExcel);
            if (piNumRegistros > (int)pHTParam["iDisplayLength"])
            {
                psMostrandoRegistros = GetMsgWeb("GridInfo");
                psMostrandoRegistros = psMostrandoRegistros.Replace("_START_", "1");
                psMostrandoRegistros = psMostrandoRegistros.Replace("_END_", pHTParam["iDisplayLength"].ToString());
                psMostrandoRegistros = psMostrandoRegistros.Replace("_TOTAL_", piNumRegistros.ToString());
            }
        }

        protected void InitParametrosExcel()
        {
            pTablaParametros = new DataTable();
            if (!pbAreaParametros)
            {
                return;
            }
            DataRow lRowParam;
            int liParamRenglon;
            int liParamColumna;
            int liParamColSpan;
            int liColDesc = 0;
            string lsParamCod;

            foreach (DataRow ldataRow in pVisParametros.Rows)
            {
                lsParamCod = ldataRow["AtribCod"].ToString();
                if (!String.IsNullOrEmpty(pHTParamDesc[lsParamCod].ToString())
                    && pHTParam[lsParamCod].ToString() != "null")
                {
                    liParamRenglon = (int)ldataRow["ParamRenglon"];
                    liParamColumna = (int)ldataRow["ParamColumna"];
                    liParamColSpan = (int)ldataRow["ParamColSpan"];
                    if (pTablaParametros.Rows.Count < liParamRenglon)
                    {
                        liColDesc = 0;
                        lRowParam = pTablaParametros.NewRow();
                        pTablaParametros.Rows.Add(lRowParam);
                    }
                    else
                    {
                        lRowParam = pTablaParametros.Rows[liParamRenglon - 1];
                    }

                    while (pTablaParametros.Columns.Count < liColDesc + liParamColSpan + 1)
                    {
                        pTablaParametros.Columns.Add(new DataColumn("Col" + pTablaParametros.Columns.Count, typeof(string)));
                    }

                    lRowParam[liColDesc] = ldataRow["RepEstIdiomaCmpDesc"].ToString();
                    if (pHTParam[lsParamCod] is string)
                    {
                        lRowParam[liColDesc + 1] = "'" + pHTParamDesc[lsParamCod];
                    }
                    else
                    {
                        lRowParam[liColDesc + 1] = pHTParamDesc[lsParamCod];
                    }

                    liColDesc = liColDesc + liParamColSpan + 1;
                }
            }
        }

        protected Hashtable BuscarTituloExcel()
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto("Reporte", "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto("Reporte", "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto("Reporte", "{TituloReporte}", true));

            return lhtRet;
        }

        protected void ProcesarTituloExcel(Hashtable lhtMeta)
        {
            string lsImg;
            int liRenLogoCliente = -1;
            int liRenLogoKeytia = -1;
            int liRenTitulo;
            int liRenglon;
            int liColumna;
            Hashtable lHTDatosImg = null;
            string lsTopLeft = null;
            string lsBottomLeft = null;
            float lfImgOffset = 0;

            //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
            if (pRowCliente != null && pRowCliente["LogoExportacion"] != DBNull.Value && lhtMeta["{LogoCliente}"] != null)
            {
                lsImg = pRowCliente["LogoExportacion"].ToString().Replace("~", "").Replace("/", "\\");
                if (lsImg.StartsWith("\\"))
                {
                    lsImg = lsImg.Substring(1);
                }
                lsImg = System.IO.Path.Combine(psKeytiaWebFPath, lsImg);
                if (System.IO.File.Exists(lsImg))
                {
                    lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"], lsImg, false, false, 0, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating, 70);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lfImgOffset = (float)lHTDatosImg["Ancho"] + 10;
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoCliente = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
            }

            //20141015 AM Cambio de header de keytia en exportacion (Se comento porque solo se hizo una prueba del requerimiento)
            //if (DSODataContext.Schema == "AlfaCorporativo")
            //{
            //    lsImg = System.IO.Path.Combine(psStylePath, @"images\Cancel.png");
            //}
            //else
            //{
            //    lsImg = System.IO.Path.Combine(psStylePath, @"images\KeytiaHeader.png");
            //}
            lsImg = System.IO.Path.Combine(psKeytiaWebFPath, @"images\KeytiaReportes.png");
            if (System.IO.File.Exists(lsImg) && lhtMeta["{LogoKeytia}"] != null)
            {
                lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"], lsImg, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating, 70);
                if ((bool)lHTDatosImg["Inserto"])
                {
                    lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                    lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                    liRenLogoKeytia = int.Parse(lsBottomLeft.Split(',')[0]);
                }
            }

            if (lhtMeta["{LogoCliente}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"]).set_Value(System.Type.Missing, String.Empty);

            if (lhtMeta["{LogoKeytia}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"]).set_Value(System.Type.Missing, String.Empty);


            if (lhtMeta["{TituloReporte}"] != null)
            {
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).set_Value(System.Type.Missing, pKDBRowReporte["{" + pvchCodIdioma + "}"].ToString());

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }
        }

        protected void ProcesarHeaderExcel()
        {
            object[,] larr;
            pExcel.BuscarTexto("Reporte", "{HeaderReporte}", true, out piRenHeader, out piColHeader);
            if (piRenHeader > 0 && piColHeader > 0 && pTablaHeaderExp.Rows.Count > 0)
            {
                pExcel.InsertarFilas("Reporte", piRenHeader + 1, pTablaHeaderExp.Rows.Count);

                larr = pExcel.DataTableToArray(pTablaHeaderExp, false);
                EstiloTablaExcel lEstiloTablaExcel = new EstiloTablaExcel();
                lEstiloTablaExcel.Estilo = "KeytiaHeaderRep";
                lEstiloTablaExcel.FilaEncabezado = false;
                lEstiloTablaExcel.FilaTotales = false;
                lEstiloTablaExcel.FilasBandas = false;
                lEstiloTablaExcel.PrimeraColumna = false;
                lEstiloTablaExcel.UltimaColumna = false;
                lEstiloTablaExcel.ColumnasBandas = true;
                lEstiloTablaExcel.AutoFiltro = false;

                pExcel.Actualizar("Reporte", piColHeader, piRenHeader, larr, lEstiloTablaExcel);
                pExcel.EliminarFila("Reporte", piRenHeader);

                //CombinarCeldasNulosExcel(pTablaHeaderExp, "Reporte", piRenHeader, piColHeader);
            }
            else if (piRenHeader > 0 && piColHeader > 0)
            {
                pExcel.EliminarFila("Reporte", piRenHeader);
                piRenHeader = -1;
                //pExcel.Actualizar("Reporte", liRenHeader, liColHeader, String.Empty);
            }
        }

        protected void AjustarHeaderExcel()
        {
            if (piRenHeader > 0 && piColHeader > 0)
            {
                CombinarCeldasNulosExcel(pTablaHeaderExp, "Reporte", piRenHeader, piColHeader);
            }
        }

        protected void ProcesarParametrosExcel()
        {
            pExcel.BuscarTexto("Reporte", "{ParametrosReporte}", true, out piRenParam, out piColParam);
            if (!pbAreaParametros)
            {
                pExcel.Actualizar("Reporte", piRenParam, piColParam, String.Empty);
            }

            object[,] larr;
            if (piRenParam > 0 && piColParam > 0 && pTablaParametros.Rows.Count > 0)
            {
                pExcel.InsertarFilas("Reporte", piRenParam + 1, pTablaParametros.Rows.Count);

                larr = pExcel.DataTableToArray(pTablaParametros, false);
                EstiloTablaExcel lEstiloTablaExcel = new EstiloTablaExcel();
                lEstiloTablaExcel.Estilo = "KeytiaParamRep";
                lEstiloTablaExcel.FilaEncabezado = false;
                lEstiloTablaExcel.FilaTotales = false;
                lEstiloTablaExcel.FilasBandas = false;
                lEstiloTablaExcel.PrimeraColumna = false;
                lEstiloTablaExcel.UltimaColumna = false;
                lEstiloTablaExcel.ColumnasBandas = true;
                lEstiloTablaExcel.AutoFiltro = false;

                pExcel.Actualizar("Reporte", piColParam, piRenParam, larr, lEstiloTablaExcel);
                pExcel.EliminarFila("Reporte", piRenParam);

                //CombinarCeldasNulosExcel(pTablaParametros, "Reporte", piRenParam, piColParam);
            }
            else if (piRenParam > 0 && piColParam > 0)
            {
                pExcel.EliminarFila("Reporte", piRenParam);
                piRenParam = -1;
                //pExcel.Actualizar("Reporte", liRenParam, liColParam, String.Empty);
            }
        }

        protected void AjustarParametrosExcel()
        {
            if (piRenParam > 0 && piColParam > 0)
            {
                CombinarCeldasNulosExcel(pTablaParametros, "Reporte", piRenParam, piColParam);
            }
        }

        protected void ProcesarDatosReporteExcel()
        {
            Hashtable lHTInfoPosicion = null;

            int liRenDatos;
            int liColDatos;

            int liRenGrid;
            int liColGrid;

            pExcel.BuscarTexto("Reporte", "{DatosReporte}", true, out liRenDatos, out liColDatos);
            if (!(liRenDatos > 0 && liColDatos > 0))
            {
                return;
            }
            liRenGrid = liRenDatos;
            liColGrid = liColDatos;

            if ((pbAreaGrafica || pbAreaGraficaHis) && pbAreaReporte)
            {
                if (pOrdenZonas == OrdenZonasReporte.PrimeroGraficas)
                {
                    if (pOrientacionZonas == Orientation.Vertical)
                    {
                        lHTInfoPosicion = ProcesarGraficasExcelVertical();
                        liRenGrid = int.Parse(lHTInfoPosicion["BottomLeft"].ToString().Split(',')[0]);
                    }
                    else if (pOrientacionZonas == Orientation.Horizontal)
                    {
                        lHTInfoPosicion = ProcesarGraficasExcelHorizontal();
                        liColGrid = int.Parse(lHTInfoPosicion["TopRight"].ToString().Split(',')[1]);
                    }
                    ProcesarGridExcel(liRenGrid, liColGrid);
                }
                else if (pOrdenZonas == OrdenZonasReporte.PrimeroReportes)
                {
                    ProcesarGridExcel(liRenGrid, liColGrid);
                    if (pOrientacionZonas == Orientation.Vertical)
                    {
                        liRenDatos = liRenDatos + pTablaGrid.Rows.Count + 3;
                        pExcel.Actualizar("Reporte", liRenDatos, liColDatos, "{DatosReporte}");

                        lHTInfoPosicion = ProcesarGraficasExcelVertical();
                    }
                    else if (pOrientacionZonas == Orientation.Horizontal)
                    {
                        liColDatos = liColDatos + pTablaGrid.Columns.Count + 1;
                        pExcel.Actualizar("Reporte", liRenDatos, liColDatos, "{DatosReporte}");
                        pExcel.EliminarColumna("Reporte", liColDatos - 1);
                        liColDatos = liColDatos - 1;

                        lHTInfoPosicion = ProcesarGraficasExcelHorizontal();
                    }
                }
            }
            else if ((pbAreaGrafica || pbAreaGraficaHis) && !pbAreaReporte)
            {
                if (pOrientacionZonas == Orientation.Vertical)
                {
                    lHTInfoPosicion = ProcesarGraficasExcelVertical();
                }
                else if (pOrientacionZonas == Orientation.Horizontal)
                {
                    lHTInfoPosicion = ProcesarGraficasExcelHorizontal();
                }
            }
            else
            {
                ProcesarGridExcel(liRenGrid, liColGrid);
            }
            if (!(liRenDatos == liRenGrid && liColDatos == liColGrid))
            {
                pExcel.Actualizar("Reporte", liRenDatos, liColDatos, String.Empty);
            }
        }

        protected Hashtable ProcesarGraficasExcelVertical()
        {
            Hashtable lHTInfoPosicion = null;
            float lfWidth1GrVertical = 600;
            float lfWidth2GrVertical = 300;
            float lfHeightGrVertical = 300;

            if (pbAreaGrafica && pbAreaGraficaHis)
            {
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGr", lfWidth2GrVertical, lfHeightGrVertical, 0, 0, pParametrosGrafica);
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGrHis", lfWidth2GrVertical, lfHeightGrVertical, lfWidth2GrVertical, 0, pParametrosGraficaHis);
            }
            else if (pbAreaGrafica && !pbAreaGraficaHis)
            {
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGr", lfWidth1GrVertical, lfHeightGrVertical, 0, 0, pParametrosGrafica);
            }
            else if (pbAreaGraficaHis && !pbAreaGrafica)
            {
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGrHis", lfWidth1GrVertical, lfHeightGrVertical, 0, 0, pParametrosGraficaHis);
            }
            return lHTInfoPosicion;
        }

        protected Hashtable ProcesarGraficasExcelHorizontal()
        {
            Hashtable lHTInfoPosicion = null;
            float lfWidthGrHorizontal = 400;
            float lfHeight1GrHorizontal = 400;
            float lfHeight2GrHorizontal = 300;

            if (pbAreaGrafica && pbAreaGraficaHis)
            {
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGr", lfWidthGrHorizontal, lfHeight2GrHorizontal, 0, 0, pParametrosGrafica);
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGrHis", lfWidthGrHorizontal, lfHeight2GrHorizontal, 0, lfHeight2GrHorizontal, pParametrosGraficaHis);
            }
            else if (pbAreaGrafica && !pbAreaGraficaHis)
            {
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGr", lfWidthGrHorizontal, lfHeight1GrHorizontal, 0, 0, pParametrosGrafica);
            }
            else if (pbAreaGraficaHis && !pbAreaGrafica)
            {
                lHTInfoPosicion = ProcesarGraficaExcel("Reporte", "DatosGrHis", lfWidthGrHorizontal, lfHeight1GrHorizontal, 0, 0, pParametrosGraficaHis);
            }
            return lHTInfoPosicion;
        }

        protected void ProcesarGridExcel(int liRenDatos, int liColDatos)
        {
            pEstiloTablaExcel = new EstiloTablaExcel();
            pEstiloTablaExcel.Estilo = "KeytiaGrid";
            pEstiloTablaExcel.FilaEncabezado = true;
            pEstiloTablaExcel.FilasBandas = true;
            pEstiloTablaExcel.FilaTotales = false;
            pEstiloTablaExcel.PrimeraColumna = false;
            pEstiloTablaExcel.UltimaColumna = false;
            pEstiloTablaExcel.ColumnasBandas = false;
            pEstiloTablaExcel.AutoFiltro = pbAutoFiltro;
            pEstiloTablaExcel.AutoAjustarColumnas = true;

            int liRenGrid = liRenDatos;
            if (!String.IsNullOrEmpty(psMostrandoRegistros))
            {
                liRenGrid = liRenGrid + 1;
                pExcel.Actualizar("Reporte", liRenDatos, liColDatos, psMostrandoRegistros);
            }

            if (pTipoReporte == TipoReporte.Tabular)
            {
                ProcesarGridTabularExcel(liRenGrid, liColDatos);
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                ProcesarGridResumidoExcel(liRenGrid, liColDatos);
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                ProcesarGridMatricialExcel(liRenGrid, liColDatos);
            }

            if (pExcel.Consultar("Reporte", liRenDatos, liColDatos) == "{DatosReporte}")
                pExcel.Actualizar("Reporte", liRenDatos, liColDatos, String.Empty);
        }

        protected void ProcesarGridTabularExcel(int liRenDatos, int liColDatos)
        {
            ProcesarTablaGridTabular();
            ProcesarTotalesGridTabular(false);

            object[,] larr = pExcel.DataTableToArray(pTablaGrid, true);

            pExcel.Actualizar("Reporte", liColDatos, liRenDatos, larr, pEstiloTablaExcel);

            ProcesarTotalesGridExcel(liRenDatos, liColDatos);
        }

        protected void ProcesarGridResumidoExcel(int liRenDatos, int liColDatos)
        {
            ProcesarTablaGridResumido();
            ProcesarTotalesGridTabular(false);

            object[,] larr = pExcel.DataTableToArray(pTablaGrid, true);

            pExcel.Actualizar("Reporte", liColDatos, liRenDatos, larr, pEstiloTablaExcel);

            ProcesarTotalesGridExcel(liRenDatos, liColDatos);
        }

        protected void ProcesarGridMatricialExcel(int liRenDatos, int liColDatos)
        {
            int liRenMatriz = liRenDatos + pDSCampos.Tables["CamposX"].Rows.Count;
            if (pDSCampos.Tables["CamposXY"].Rows.Count == 1)
            {
                liRenDatos = liRenDatos + 1;
            }

            ProcesarTablaGridMatricial();
            ProcesarTotalesGridMatricial(false);
            DataTable lTablaTotales = pTablaGrid.Clone();

            object[,] larr = pExcel.DataTableToArray(pTablaGrid, true);

            pExcel.Actualizar("Reporte", liColDatos, liRenMatriz, larr, pEstiloTablaExcel);

            ProcesarTotalesGridExcel(liRenMatriz, liColDatos);

            int liColDatosFin = liColDatos + pTablaGrid.Columns.Count - 1;
            string lsCell1Origen = pExcel.ObtenerNombreCelda("Reporte", liRenMatriz, liColDatos);
            string lsCell2Origen = pExcel.ObtenerNombreCelda("Reporte", liRenMatriz, liColDatosFin);
            string lsCell1Destino;
            string lsCell2Destino;
            for (int li = liRenDatos; li < liRenMatriz; li++)
            {
                lsCell1Destino = pExcel.ObtenerNombreCelda("Reporte", li, liColDatos);
                lsCell2Destino = pExcel.ObtenerNombreCelda("Reporte", li, liColDatosFin);
                pExcel.CopiarFormato("Reporte", lsCell1Origen, lsCell2Origen, lsCell1Destino, lsCell2Destino);
            }

            int lidx;
            string lsDataField;
            string lsEncabezadoX;
            string lsEncabezadoXAnt;
            int liColIni = 1;
            int liColSpan = 1;
            int liFila;

            for (lidx = 0; lidx < pDSCampos.Tables["CamposX"].Rows.Count; lidx++)
            {
                DataRow lRowCampo = pDSCampos.Tables["CamposX"].Rows[lidx];
                lsDataField = GetDataFieldName(lRowCampo);
                liFila = lidx + liRenDatos;
                liColIni = pDSCampos.Tables["CamposY"].Rows.Count + liColDatos;
                lsEncabezadoX = null;
                lsEncabezadoXAnt = null;
                foreach (DataRow ldataRow in pDSCampos.Tables["ValoresEjeX"].Rows)
                {
                    lsEncabezadoX = FormatearValor(lRowCampo, ldataRow[lsDataField]);

                    if (lsEncabezadoXAnt == null)
                    {
                        lsEncabezadoXAnt = lsEncabezadoX;
                        liColSpan = 1;
                    }
                    else if (lsEncabezadoX == lsEncabezadoXAnt)
                    {
                        liColSpan = liColSpan + 1;
                    }
                    else
                    {
                        pExcel.Actualizar("Reporte", liFila, liColIni, lsEncabezadoXAnt);
                        liColSpan = liColSpan * pDSCampos.Tables["CamposXY"].Rows.Count;
                        pExcel.CombinarCeldas("Reporte", liFila, liColIni, liFila, liColIni + liColSpan - 1, true, true);

                        lsEncabezadoXAnt = lsEncabezadoX;
                        liColIni = liColIni + liColSpan;
                        liColSpan = 1;
                    }
                }
                if (lsEncabezadoX != null)
                {
                    pExcel.Actualizar("Reporte", liFila, liColIni, lsEncabezadoX);
                    liColSpan = liColSpan * pDSCampos.Tables["CamposXY"].Rows.Count;
                    pExcel.CombinarCeldas("Reporte", liFila, liColIni, liFila, liColIni + liColSpan - 1, true, true);
                }
            }
        }

        protected void ProcesarTotalesGridExcel(int liRenDatos, int liColDatos)
        {
            if (pTablaTotalesGrid != null && pTablaTotalesGrid.Rows.Count > 0)
            {
                int liColDatosFin = liColDatos + pTablaGrid.Columns.Count - 1;
                int liRenTotales = liRenDatos + pTablaGrid.Rows.Count + 1;
                pExcel.InsertarFilas("Reporte", liRenTotales, 1);

                string lsCell1Origen = pExcel.ObtenerNombreCelda("Reporte", liRenDatos, liColDatos);
                string lsCell2Origen = pExcel.ObtenerNombreCelda("Reporte", liRenDatos, liColDatosFin);
                string lsCell1Destino = pExcel.ObtenerNombreCelda("Reporte", liRenTotales + 1, liColDatos);
                string lsCell2Destino = pExcel.ObtenerNombreCelda("Reporte", liRenTotales + 1, liColDatosFin);
                pExcel.CopiarFormato("Reporte", lsCell1Origen, lsCell2Origen, lsCell1Destino, lsCell2Destino);

                object[,] larr = pExcel.DataTableToArray(pTablaTotalesGrid, false);
                pExcel.Actualizar("Reporte", liRenTotales + 1, liColDatos, liRenTotales + 1, liColDatos + pTablaGrid.Columns.Count - 1, larr);
                pExcel.EliminarFila("Reporte", liRenTotales);
            }
        }

        protected void ProcesarHojasExcel()
        {
            int liNombreHoja = 1;
            string lsAreaReporte = GetMsgWeb("AreaReporte");
            string lsAreaGrafica = GetMsgWeb("AreaGrafica");
            string lsAreaGraficaHis = GetMsgWeb("AreaGraficaHis");

            if (String.IsNullOrEmpty(lsAreaReporte))
            {
                lsAreaReporte = liNombreHoja.ToString();
                liNombreHoja = liNombreHoja + 1;
            }
            if (String.IsNullOrEmpty(lsAreaGrafica))
            {
                lsAreaGrafica = liNombreHoja.ToString();
                liNombreHoja = liNombreHoja + 1;
            }
            if (String.IsNullOrEmpty(lsAreaGraficaHis))
            {
                lsAreaGraficaHis = liNombreHoja.ToString();
            }

            lsAreaReporte = lsAreaReporte.Substring(0, Math.Min(31, lsAreaReporte.Length));
            lsAreaGrafica = lsAreaGrafica.Substring(0, Math.Min(31, lsAreaGrafica.Length));
            lsAreaGraficaHis = lsAreaGraficaHis.Substring(0, Math.Min(31, lsAreaGraficaHis.Length));

            if (pbAreaGrafica)
            {
                if (lsAreaGrafica != lsAreaReporte)
                {
                    pExcel.Renombrar("DatosGr", lsAreaGrafica);
                }
            }
            else
            {
                pExcel.Remover("DatosGr");
            }
            if (pbAreaGraficaHis)
            {
                if (pbAreaGrafica
                    && lsAreaGraficaHis != lsAreaReporte
                    && lsAreaGraficaHis != lsAreaGrafica)
                {
                    pExcel.Renombrar("DatosGrHis", lsAreaGraficaHis);
                }
                else if (!pbAreaGrafica
                    && lsAreaGraficaHis != lsAreaReporte)
                {
                    pExcel.Renombrar("DatosGrHis", lsAreaGraficaHis);
                }
            }
            else
            {
                pExcel.Remover("DatosGrHis");
            }
            pExcel.Renombrar("Reporte", lsAreaReporte);
        }

        protected Hashtable ProcesarGraficaExcel(string lsHojaGrafico, string lsHojaDatos, float lfWidth, float lfHeight, float lfOffsetX, float lfOffsetY, ParametrosGrafica lParametrosGrafica)
        {
            FormatoGrafica lFormatoGrafica = new FormatoGrafica();
            lFormatoGrafica.Titulo = lParametrosGrafica.Title;
            lFormatoGrafica.Leyendas = lParametrosGrafica.ShowLegend;
            lFormatoGrafica.XFormat = lParametrosGrafica.XFormat;
            lFormatoGrafica.YFormat = lParametrosGrafica.YFormat;
            Microsoft.Office.Interop.Excel.XlChartType lCharType = GetTipoGraficaExcel(lParametrosGrafica.TipoGrafica);
            if (lParametrosGrafica.TipoGrafica == TipoGrafica.Pastel && lParametrosGrafica.DataColumns.Length > 1)
            {
                lCharType = Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked100;
                return pExcel.InsertarGraficoSR(lsHojaGrafico, lsHojaDatos, "{DatosReporte}", lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lParametrosGrafica.DataColumns, lParametrosGrafica.SeriesNames, lParametrosGrafica.Datos, lCharType, lfWidth, lfHeight, lfOffsetX, lfOffsetY, lFormatoGrafica);
            }
            else
            {
                return pExcel.InsertarGrafico(lsHojaGrafico, lsHojaDatos, "{DatosReporte}", lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lParametrosGrafica.DataColumns, lParametrosGrafica.SeriesNames, lParametrosGrafica.Datos, lCharType, lfWidth, lfHeight, lfOffsetX, lfOffsetY, lFormatoGrafica);
            }
        }

        protected Microsoft.Office.Interop.Excel.XlChartType GetTipoGraficaExcel(TipoGrafica lTipoGrafica)
        {
            if (lTipoGrafica == TipoGrafica.Pastel)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlPie;
            }
            else if (lTipoGrafica == TipoGrafica.Barras)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered;
            }
            else if (lTipoGrafica == TipoGrafica.Lineas)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlLine;
            }
            else
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlArea;
            }
        }

        protected void CombinarCeldasNulosExcel(DataTable lTabla, string lsHoja, int liRenHeaderIni, int liColHeaderIni)
        {
            pExcel.InsertarFilas("Reporte", liRenHeaderIni + lTabla.Rows.Count, lTabla.Rows.Count + 1);
            int liRenHeaderFin = liRenHeaderIni + lTabla.Rows.Count - 1;
            int liColHeaderFin = liColHeaderIni + lTabla.Columns.Count - 1;

            int liRenDatosIni = liRenHeaderFin + 2;
            int liRenDatosFin = liRenDatosIni + lTabla.Rows.Count - 1;

            string lsCell1Origen = pExcel.ObtenerNombreCelda("Reporte", liRenHeaderIni, liColHeaderIni);
            string lsCell2Origen = pExcel.ObtenerNombreCelda("Reporte", liRenHeaderFin, liColHeaderFin);
            string lsCell1Destino = pExcel.ObtenerNombreCelda("Reporte", liRenDatosIni, liColHeaderIni);
            string lsCell2Destino = pExcel.ObtenerNombreCelda("Reporte", liRenDatosFin, liColHeaderFin);
            object[,] larr = pExcel.DataTableToArray(lTabla, false);
            pExcel.CopiarFormato("Reporte", lsCell1Origen, lsCell2Origen, lsCell1Destino, lsCell2Destino);
            pExcel.Actualizar("Reporte", liRenDatosIni, liColHeaderIni, liRenDatosFin, liColHeaderFin, larr);
            pExcel.EliminarFilas("Reporte", liRenHeaderIni, liRenHeaderFin + 1);

            int liColIni;
            int liColFin;
            DataRow ldataRow;
            for (int li = 0; li < lTabla.Rows.Count; li++)
            {
                ldataRow = lTabla.Rows[li];
                liColIni = 1;
                liColFin = 1;
                for (int lj = 1; lj < lTabla.Columns.Count; lj++)
                {
                    liColFin = lj;
                    if (ldataRow[lj] != DBNull.Value)
                    {
                        if (ldataRow[lj - 1] == DBNull.Value)
                        {
                            pExcel.CombinarCeldas(lsHoja, li + liRenHeaderIni, liColIni + liColHeaderIni - 1, li + liRenHeaderIni, liColFin + liColHeaderIni - 1, true);
                        }
                        liColIni = lj + 1;
                    }
                }
                liColFin = lTabla.Columns.Count;
                pExcel.CombinarCeldas(lsHoja, li + liRenHeaderIni, liColIni + liColHeaderIni - 1, li + liRenHeaderIni, liColFin + liColHeaderIni - 1, true);
            }
        }

        #endregion

        #region Word

        public WordAccess ExportDOC()
        {
            WordAccess lWord = null; ;
            try
            {
                lWord = new WordAccess();
                lWord.FilePath = System.IO.Path.Combine(psStylePath, @"plantillas\reportes\Plantilla Reporte Estandar.docx");
                lWord.Abrir();
                ExportDOC(lWord);
            }
            catch (Exception e)
            {
                if (lWord != null)
                {
                    lWord.Cerrar(true);
                    lWord = null;
                }
                throw new Exception("", e);
            }

            return lWord;
        }

        public void ExportDOC(WordAccess lWord)
        {
            pWord = lWord;
            pWord.XmlPalettePath = System.IO.Path.Combine(psStylePath, @"chart.xml");

            InitMaxRegWord();
            InitDatos();
            InitParametros();
            ProcesarTituloWord();
            ProcesarHeaderWord();
            ProcesarParametrosWord();
            ProcesarDatosReporteWord();
        }

        protected void InitMaxRegWord()
        {
            int liMaxRegWord = int.MaxValue; //1000;
            pHTParam["iDisplayLength"] = Math.Min(piNumRegistros, liMaxRegWord);
            if (piNumRegistros > (int)pHTParam["iDisplayLength"])
            {
                psMostrandoRegistros = GetMsgWeb("GridInfo");
                psMostrandoRegistros = psMostrandoRegistros.Replace("_START_", "1");
                psMostrandoRegistros = psMostrandoRegistros.Replace("_END_", pHTParam["iDisplayLength"].ToString());
                psMostrandoRegistros = psMostrandoRegistros.Replace("_TOTAL_", piNumRegistros.ToString());
            }
        }

        protected void ProcesarTituloWord()
        {
            string lsImg;
            //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
            if (pRowCliente != null && pRowCliente["LogoExportacion"] != DBNull.Value)
            {
                lsImg = System.IO.Path.Combine(psKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                if (System.IO.File.Exists(lsImg))
                {
                    pWord.ReemplazarTextoPorImagen("{LogoCliente}", lsImg);
                }
                else
                {
                    pWord.ReemplazarTexto("{LogoCliente}", "");
                }
            }
            else
            {
                pWord.ReemplazarTexto("{LogoCliente}", "");
            }

            lsImg = System.IO.Path.Combine(psStylePath, @"images\KeytiaHeader.png");
            if (System.IO.File.Exists(lsImg))
            {
                pWord.ReemplazarTextoPorImagen("{LogoKeytia}", lsImg);
            }
            else
            {
                pWord.ReemplazarTexto("{LogoKeytia}", "");
            }

            pWord.ReemplazarTexto("{TituloReporte}", pKDBRowReporte["{" + pvchCodIdioma + "}"].ToString());
        }

        protected void ProcesarHeaderWord()
        {
            pWord.PosicionaCursor("{HeaderReporte}");
            if (pTablaHeaderExp.Rows.Count > 0)
            {
                pWord.ReemplazarTexto("{HeaderReporte}", "");
                EstiloTablaWord lEstiloTablaWord = new EstiloTablaWord();
                lEstiloTablaWord.Estilo = "KeytiaHeaderRep";
                lEstiloTablaWord.FilaEncabezado = false;
                lEstiloTablaWord.FilaTotales = false;
                lEstiloTablaWord.FilasBandas = false;
                lEstiloTablaWord.PrimeraColumna = false;
                lEstiloTablaWord.UltimaColumna = false;
                lEstiloTablaWord.ColumnasBandas = true;
                pWord.InsertarTabla(pTablaHeaderExp, lEstiloTablaWord.FilaEncabezado, lEstiloTablaWord.Estilo, lEstiloTablaWord);
                CombinarCeldasNulosWord(pTablaHeaderExp);
            }
            else
            {
                pWord.ReemplazarTexto(@"\{HeaderReporte\}^13", "", true);
            }
        }

        protected void ProcesarParametrosWord()
        {
            if (!pbAreaParametros)
            {
                pWord.ReemplazarTexto(@"\{ParametrosReporte\}^13", "", true);
                return;
            }

            pWord.PosicionaCursor("{ParametrosReporte}");
            if (pTablaParametros.Rows.Count > 0)
            {
                pWord.ReemplazarTexto("{ParametrosReporte}", "");
                EstiloTablaWord lEstiloTablaWord = new EstiloTablaWord();
                lEstiloTablaWord.Estilo = "KeytiaParamRep";
                lEstiloTablaWord.FilaEncabezado = false;
                lEstiloTablaWord.FilaTotales = false;
                lEstiloTablaWord.FilasBandas = false;
                lEstiloTablaWord.PrimeraColumna = false;
                lEstiloTablaWord.UltimaColumna = false;
                lEstiloTablaWord.ColumnasBandas = true;
                pWord.InsertarTabla(pTablaParametros, lEstiloTablaWord.FilaEncabezado, lEstiloTablaWord.Estilo, lEstiloTablaWord);
                CombinarCeldasNulosWord(pTablaParametros);
            }
            else
            {
                pWord.ReemplazarTexto(@"\{ParametrosReporte\}^13", "", true);
            }

        }

        protected void ProcesarDatosReporteWord()
        {
            if (pOrdenZonas == OrdenZonasReporte.PrimeroGraficas)
            {
                if (pbAreaGrafica)
                {
                    ProcesarGraficaWord(pParametrosGrafica);
                }
                if (pbAreaGraficaHis)
                {
                    ProcesarGraficaWord(pParametrosGraficaHis);
                }
                if (pbAreaReporte)
                {
                    ProcesarGridReporteWord();
                }
            }
            else
            {
                if (pbAreaReporte)
                {
                    ProcesarGridReporteWord();
                }
                if (pbAreaGrafica)
                {
                    ProcesarGraficaWord(pParametrosGrafica);
                }
                if (pbAreaGraficaHis)
                {
                    ProcesarGraficaWord(pParametrosGraficaHis);
                }
            }
            pWord.PosicionaCursor("{DatosReporte}", true);
            pWord.ReemplazarTexto(@"\{DatosReporte\}^13", "", true);
        }

        protected void ProcesarGraficaWord(ParametrosGrafica lParametrosGrafica)
        {
            FormatoGrafica lFormatoGrafica = new FormatoGrafica();
            lFormatoGrafica.Titulo = lParametrosGrafica.Title;
            lFormatoGrafica.Leyendas = lParametrosGrafica.ShowLegend;
            lFormatoGrafica.XFormat = lParametrosGrafica.XFormat;
            lFormatoGrafica.YFormat = lParametrosGrafica.YFormat;
            Microsoft.Office.Core.XlChartType lCharType = GetTipoGrafica(lParametrosGrafica.TipoGrafica);

            pWord.PosicionaCursor("{DatosReporte}", true);
            if (lParametrosGrafica.TipoGrafica == TipoGrafica.Pastel && lParametrosGrafica.DataColumns.Length > 1)
            {
                lCharType = Microsoft.Office.Core.XlChartType.xlColumnStacked100;
                pWord.InsertarGraficoSR(lParametrosGrafica.Datos, lParametrosGrafica.DataColumns, lParametrosGrafica.SeriesNames, lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lCharType, lFormatoGrafica);
            }
            else
            {
                pWord.InsertarGrafico(lParametrosGrafica.Datos, lParametrosGrafica.DataColumns, lParametrosGrafica.SeriesNames, lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lCharType, lFormatoGrafica);
            }
        }

        protected void ProcesarGridReporteWord()
        {
            pWord.PosicionaCursor("{DatosReporte}", true);
            if (!String.IsNullOrEmpty(psMostrandoRegistros))
            {
                pWord.ActualizaTexto(psMostrandoRegistros);
                pWord.PosicionaCursor("{DatosReporte}", true);
                pWord.PosicionaCursor("{DatosReporte}", true);
            }

            pEstiloTablaWord = new EstiloTablaWord();
            pEstiloTablaWord.Estilo = "KeytiaGrid";
            pEstiloTablaWord.FilaEncabezado = true;
            pEstiloTablaWord.FilasBandas = true;
            pEstiloTablaWord.PrimeraColumna = false;
            pEstiloTablaWord.UltimaColumna = false;
            pEstiloTablaWord.ColumnasBandas = false;

            if (pTipoReporte == TipoReporte.Tabular)
            {
                ProcesarGridTabularWord();
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                ProcesarGridResumidoWord();
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                ProcesarGridMatricialWord();
            }
        }

        protected void ProcesarGridTabularWord()
        {
            ProcesarTablaGridTabular();
            pEstiloTablaWord.FilaTotales = ProcesarTotalesGridTabular();

            pWord.InsertarTabla(pTablaGrid, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);

            pWord.Tabla.Columns.AutoFit();
        }

        protected void ProcesarGridResumidoWord()
        {
            ProcesarTablaGridResumido();
            pEstiloTablaWord.FilaTotales = ProcesarTotalesGridTabular();

            pWord.InsertarTabla(pTablaGrid, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);

            pWord.Tabla.Columns.AutoFit();
        }

        protected void ProcesarGridMatricialWord()
        {
            ProcesarTablaGridMatricial();
            pEstiloTablaWord.FilaTotales = ProcesarTotalesGridMatricial();

            pWord.InsertarTabla(pTablaGrid, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);


            int lidx;
            string lsDataField;
            string lsEncabezadoX;
            string lsEncabezadoXAnt;
            int liColIni = 1;
            int liColSpan = 1;
            int liFila;

            if (pDSCampos.Tables["CamposXY"].Rows.Count == 1)
            {
                if (pDSCampos.Tables["CamposX"].Rows.Count - 1 > 0)
                {
                    pWord.InsertarFilasArriba(1, pDSCampos.Tables["CamposX"].Rows.Count - 1);
                }
                liFila = 0;
            }
            else
            {
                pWord.InsertarFilasArriba(1, pDSCampos.Tables["CamposX"].Rows.Count);
                liFila = 1;
            }

            for (lidx = 0; lidx < pDSCampos.Tables["CamposX"].Rows.Count; lidx++)
            {
                DataRow lRowCampo = pDSCampos.Tables["CamposX"].Rows[lidx];
                lsDataField = GetDataFieldName(lRowCampo);
                liFila = lidx + 1;
                liColIni = pDSCampos.Tables["CamposY"].Rows.Count + 1;
                lsEncabezadoX = null;
                lsEncabezadoXAnt = null;
                foreach (DataRow ldataRow in pDSCampos.Tables["ValoresEjeX"].Rows)
                {
                    lsEncabezadoX = FormatearValor(lRowCampo, ldataRow[lsDataField]);

                    if (lsEncabezadoXAnt == null)
                    {
                        lsEncabezadoXAnt = lsEncabezadoX;
                        liColSpan = 1;
                    }
                    else if (lsEncabezadoX == lsEncabezadoXAnt)
                    {
                        liColSpan = liColSpan + 1;
                    }
                    else
                    {
                        pWord.ActualizaTextoCelda(liFila, liColIni, lsEncabezadoXAnt);
                        liColSpan = liColSpan * pDSCampos.Tables["CamposXY"].Rows.Count;
                        pWord.CombinarCeldas(liFila, liColIni, liFila, liColIni + liColSpan - 1);

                        lsEncabezadoXAnt = lsEncabezadoX;
                        liColSpan = 1;
                        liColIni = liColIni + liColSpan;
                    }
                }
                if (lsEncabezadoX != null)
                {
                    pWord.ActualizaTextoCelda(liFila, liColIni, lsEncabezadoX);
                    liColSpan = liColSpan * pDSCampos.Tables["CamposXY"].Rows.Count;
                    pWord.CombinarCeldas(liFila, liColIni, liFila, liColIni + liColSpan - 1);
                }
            }

            if (pDSCampos.Tables["CamposXY"].Rows.Count == 1)
            {
                lidx = pDSCampos.Tables["CamposX"].Rows.Count - 1;
            }
            else
            {
                lidx = pDSCampos.Tables["CamposX"].Rows.Count;
            }
            for (int li = 1; li <= lidx; li++)
            {
                pWord.CopiarFormatoFila(1, li + 1);
            }
        }

        protected Microsoft.Office.Core.XlChartType GetTipoGrafica(TipoGrafica lTipoGrafica)
        {
            if (lTipoGrafica == TipoGrafica.Pastel)
            {
                return Microsoft.Office.Core.XlChartType.xlPie;
            }
            else if (lTipoGrafica == TipoGrafica.Barras)
            {
                return Microsoft.Office.Core.XlChartType.xlColumnClustered;
            }
            else if (lTipoGrafica == TipoGrafica.Lineas)
            {
                return Microsoft.Office.Core.XlChartType.xlLine;
            }
            else
            {
                return Microsoft.Office.Core.XlChartType.xlArea;
            }
        }

        protected void CombinarCeldasNulosWord(DataTable lTabla)
        {
            int liColIni;
            for (int li = 1; li <= pWord.Tabla.Rows.Count; li++)
            {
                int lj = 1;
                while (lj <= pWord.Tabla.Rows[li].Cells.Count &&
                    !string.IsNullOrEmpty(pWord.Tabla.Cell(li, lj).Range.Text.Trim().Replace("\n", "").Replace("\a", "")))
                {
                    lj++;
                }
                while (lj <= pWord.Tabla.Rows[li].Cells.Count)
                {
                    liColIni = lj;
                    while (lj < pWord.Tabla.Rows[li].Cells.Count &&
                        string.IsNullOrEmpty(pWord.Tabla.Cell(li, lj + 1).Range.Text.Trim().Replace("\n", "").Replace("\a", "")))
                    {
                        lj++;
                    }
                    if (lj <= pWord.Tabla.Rows[li].Cells.Count)
                    {
                        pWord.CombinarCeldas(li, liColIni, li, lj);
                        lj = liColIni + 1;
                        while (lj <= pWord.Tabla.Rows[li].Cells.Count &&
                            !string.IsNullOrEmpty(pWord.Tabla.Cell(li, lj).Range.Text.Trim().Replace("\n", "").Replace("\a", "")))
                        {
                            lj++;
                        }

                    }
                }
            }
        }

        #endregion

        #region CSV

        public void ExportCSV(TxtFileAccess lTxt)
        {
            pTxt = lTxt;
            InitMaxRegCSV();
            InitDatos();
            ProcesarDatosReporteTxt();
        }

        protected void InitMaxRegCSV()
        {
            int liMaxRegCSV = int.MaxValue; //20000;
            pHTParam["iDisplayLength"] = Math.Min(piNumRegistros, liMaxRegCSV);
        }

        protected void ProcesarDatosReporteTxt()
        {
            int li;
            int lj;
            List<string> lstValores = new List<string>();

            if (pTipoReporte == TipoReporte.Tabular)
            {
                ProcesarTablaGridTabular();
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                ProcesarTablaGridResumido();
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                ProcesarTablaGridMatricial();
            }


            for (li = 0; li < pTablaGrid.Columns.Count; li++)
            {
                lstValores.Add(pTablaGrid.Columns[li].ColumnName);
            }
            pTxt.Escribir("\"" + string.Join("\",\"", lstValores.ToArray()) + "\"");

            for (li = 0; li < pTablaGrid.Rows.Count; li++)
            {
                lstValores = new List<string>();
                for (lj = 0; lj < pTablaGrid.Columns.Count; lj++)
                {
                    lstValores.Add(pTablaGrid.Rows[li][lj].ToString());

                }
                pTxt.Escribir("\"" + string.Join("\",\"", lstValores.ToArray()) + "\"");
            }
        }

        #endregion

        #region Metodos estaticos

        public static DataSet GetCamposTabular(int liCodReporte)
        {
            KDBAccess lKDB = new KDBAccess();
            DataSet lDSCampos = new DataSet();
            DataTable lKDBCampos = lKDB.GetHisRegByEnt("RepEstCampo", "Campos", "{RepEst} = " + liCodReporte, "{RepEstOrdenCampo}");
            DataTable lKDBTotalizados = lKDBCampos.Clone();
            DataTable lKDBAgrupadores = lKDBCampos.Clone();

            foreach (DataRow lRow in lKDBCampos.Rows)
            {
                if (((int)lRow["{BanderasCamposRep}"] & 1) == 1)
                {
                    lKDBAgrupadores.ImportRow(lRow);
                }
                if (((int)lRow["{BanderasCamposRep}"] & 2) == 2 && lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lKDBTotalizados.ImportRow(lRow);
                }
            }

            lKDBCampos.TableName = "Campos";
            lKDBTotalizados.TableName = "Totalizados";
            lKDBAgrupadores.TableName = "Agrupadores";

            lDSCampos.Tables.Add(lKDBCampos);
            lDSCampos.Tables.Add(lKDBTotalizados);
            lDSCampos.Tables.Add(lKDBAgrupadores);

            return lDSCampos;
        }

        public static DataSet GetCamposResumido(int liCodReporte)
        {
            return GetCamposTabular(liCodReporte);
        }

        public static DataSet GetCamposMatricial(int liCodReporte)
        {
            KDBAccess lKDB = new KDBAccess();
            DataSet lDSCampos = new DataSet();
            DataTable lKDBCamposX = lKDB.GetHisRegByEnt("RepEstCampo", "Campos Eje X", "{RepEst} = " + liCodReporte, "{RepEstOrdenCampo}");
            DataTable lKDBAgrupadorX = lKDBCamposX.Clone();

            foreach (DataRow lRow in lKDBCamposX.Rows)
            {
                if (((int)lRow["{BanderasCamposX}"] & 1) == 1)
                {
                    lKDBAgrupadorX.ImportRow(lRow);
                }
            }

            DataTable lKDBCamposY = lKDB.GetHisRegByEnt("RepEstCampo", "Campos Eje Y", "{RepEst} = " + liCodReporte, "{RepEstOrdenCampo}");
            DataTable lKDBAgrupadorY = lKDBCamposY.Clone();
            DataTable lKDBTotalizadosY = lKDBCamposY.Clone();

            foreach (DataRow lRow in lKDBCamposY.Rows)
            {
                if (((int)lRow["{BanderasCamposY}"] & 1) == 1)
                {
                    lKDBAgrupadorY.ImportRow(lRow);
                }
                if (((int)lRow["{BanderasCamposY}"] & 2) == 2 && lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lKDBTotalizadosY.ImportRow(lRow);
                }
            }

            DataTable lKDBCamposXY = lKDB.GetHisRegByEnt("RepEstCampo", "Campos XY", "{RepEst} = " + liCodReporte, "{RepEstOrdenCampo}");
            DataTable lKDBAgrupadorXY = lKDBCamposXY.Clone();
            DataTable lKDBTotalizadosXYTotX = lKDBCamposXY.Clone();
            DataTable lKDBTotalizadosXYTotY = lKDBCamposXY.Clone();

            foreach (DataRow lRow in lKDBCamposXY.Rows)
            {
                if (((int)lRow["{BanderasCamposXY}"] & 1) == 1)
                {
                    lKDBAgrupadorXY.ImportRow(lRow);
                }
                if (((int)lRow["{BanderasCamposXY}"] & 2) == 2 && lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lKDBTotalizadosXYTotX.ImportRow(lRow);
                }
                if (((int)lRow["{BanderasCamposXY}"] & 4) == 4 && lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lKDBTotalizadosXYTotY.ImportRow(lRow);
                }
            }

            lKDBCamposX.TableName = "CamposX";
            lKDBAgrupadorX.TableName = "AgrupadorX";

            lKDBCamposY.TableName = "CamposY";
            lKDBAgrupadorY.TableName = "AgrupadorY";
            lKDBTotalizadosY.TableName = "TotalizadosY";

            lKDBCamposXY.TableName = "CamposXY";
            lKDBAgrupadorXY.TableName = "AgrupadorXY";
            lKDBTotalizadosXYTotX.TableName = "TotalizadosXYTotX";
            lKDBTotalizadosXYTotY.TableName = "TotalizadosXYTotY";

            lDSCampos.Tables.Add(lKDBCamposX);
            lDSCampos.Tables.Add(lKDBAgrupadorX);

            lDSCampos.Tables.Add(lKDBCamposY);
            lDSCampos.Tables.Add(lKDBAgrupadorY);
            lDSCampos.Tables.Add(lKDBTotalizadosY);

            lDSCampos.Tables.Add(lKDBCamposXY);
            lDSCampos.Tables.Add(lKDBAgrupadorXY);
            lDSCampos.Tables.Add(lKDBTotalizadosXYTotX);
            lDSCampos.Tables.Add(lKDBTotalizadosXYTotY);

            return lDSCampos;
        }

        public static void GetCamposGr(DataSet lDSCampos, int liCodReporte)
        {
            KDBAccess lKDB = new KDBAccess();
            DataTable lKDBCamposGr = lKDB.GetHisRegByEnt("RepEstCampo", "Campos Grafica", "{RepEst} = " + liCodReporte, "{RepEstOrdenCampo}");
            DataTable lKDBCamposGrHis = lKDB.GetHisRegByEnt("RepEstCampo", "Campos Grafica Historica", "{RepEst} = " + liCodReporte, "{RepEstOrdenCampo}");

            lKDBCamposGr.TableName = "CamposGr";
            lKDBCamposGrHis.TableName = "CamposGrHis";

            lDSCampos.Tables.Add(lKDBCamposGr);
            lDSCampos.Tables.Add(lKDBCamposGrHis);
        }

        public static void AgregarParamDataFieldsTabular(Hashtable lHTParam, DataSet lDSCampos)
        {
            if (!lHTParam.ContainsKey("DataFields"))
            {
                lHTParam.Add("DataFields", ReporteEstandarUtil.GetDataFields(lDSCampos.Tables["Campos"]));
            }
            if (!lHTParam.ContainsKey("TopFields"))
            {
                lHTParam.Add("TopFields", ReporteEstandarUtil.GetTopFields(lDSCampos.Tables["Totalizados"]));
            }
            if (!lHTParam.ContainsKey("GroupByFields"))
            {
                lHTParam.Add("GroupByFields", ReporteEstandarUtil.GetGroupByFields(lDSCampos.Tables["Agrupadores"]));
            }
        }

        public static void AgregarParamDataFieldsResumido(Hashtable lHTParam, DataSet lDSCampos, string lsOperAgrupacion)
        {
            if (!lHTParam.ContainsKey("DataFields"))
            {
                lHTParam.Add("DataFields", ReporteEstandarUtil.GetDataFieldsResumido(lDSCampos));
            }
            if (!lHTParam.ContainsKey("TopFields"))
            {
                lHTParam.Add("TopFields", ReporteEstandarUtil.GetTopFields(lDSCampos.Tables["Totalizados"]));
            }
            if (!lHTParam.ContainsKey("TopFieldsGroupBy"))
            {
                lHTParam.Add("TopFieldsGroupBy", ReporteEstandarUtil.GetTopFieldsGroupBy(lDSCampos.Tables["Agrupadores"], lsOperAgrupacion));
            }
            if (!lHTParam.ContainsKey("GroupByFields"))
            {
                lHTParam.Add("GroupByFields", ReporteEstandarUtil.GetGroupByFieldsResumido(lDSCampos.Tables["Agrupadores"], lsOperAgrupacion));
            }
        }

        public static void AgregarParamDataFieldsMatricial(Hashtable lHTParam, DataSet lDSCampos)
        {
            string lsFieldsY;
            string lsFieldsX;
            string lsFieldsXY;
            StringBuilder lsbFields = new StringBuilder();

            //Agregar DataFields
            if (!lHTParam.ContainsKey("DataFieldsY"))
            {
                lsFieldsY = ReporteEstandarUtil.GetDataFields(lDSCampos.Tables["CamposY"]);
                lHTParam.Add("DataFieldsY", lsFieldsY);
            }
            if (!lHTParam.ContainsKey("DataFieldsX"))
            {
                lsFieldsX = ReporteEstandarUtil.GetDataFields(lDSCampos.Tables["CamposX"]);
                lHTParam.Add("DataFieldsX", lsFieldsX);
            }
            if (!lHTParam.ContainsKey("DataFieldsXY"))
            {
                lsFieldsXY = ReporteEstandarUtil.GetDataFields(lDSCampos.Tables["CamposXY"]);
                lHTParam.Add("DataFieldsXY", lsFieldsXY);
            }
            if (!lHTParam.ContainsKey("DataFields"))
            {
                lsFieldsY = lHTParam["DataFieldsY"].ToString();
                lsFieldsX = lHTParam["DataFieldsX"].ToString();
                lsFieldsXY = lHTParam["DataFieldsXY"].ToString();
                lsbFields.Append(lsFieldsY);
                if (!String.IsNullOrEmpty(lsFieldsY)
                    && !String.IsNullOrEmpty(lsFieldsX))
                {
                    lsbFields.AppendLine(",");
                }
                lsbFields.Append(lsFieldsX);
                if (!String.IsNullOrEmpty(lsFieldsX)
                    && !String.IsNullOrEmpty(lsFieldsXY))
                {
                    lsbFields.AppendLine(",");
                }
                lsbFields.Append(lsFieldsXY);
                lHTParam.Add("DataFields", lsbFields.ToString());
            }

            //Agregar TopFields
            if (!lHTParam.ContainsKey("TopFieldsY"))
            {
                lsFieldsY = ReporteEstandarUtil.GetTopFields(lDSCampos.Tables["TotalizadosY"]);
                lHTParam.Add("TopFieldsY", lsFieldsY);
            }
            if (!lHTParam.ContainsKey("TopFieldsXYTotX"))
            {
                lsFieldsX = ReporteEstandarUtil.GetTopFields(lDSCampos.Tables["TotalizadosXYTotX"]);
                lHTParam.Add("TopFieldsXYTotX", lsFieldsX);
            }

            //Agregar GroupByFields
            if (!lHTParam.ContainsKey("GroupByFieldsY"))
            {
                lsFieldsY = ReporteEstandarUtil.GetGroupByFields(lDSCampos.Tables["AgrupadorY"]);
                lHTParam.Add("GroupByFieldsY", lsFieldsY);
            }
            if (!lHTParam.ContainsKey("GroupByFieldsX"))
            {
                lsFieldsX = ReporteEstandarUtil.GetGroupByFields(lDSCampos.Tables["AgrupadorX"]);
                lHTParam.Add("GroupByFieldsX", lsFieldsX);
            }
            if (!lHTParam.ContainsKey("GroupByFieldsXY"))
            {
                lsFieldsXY = ReporteEstandarUtil.GetGroupByFields(lDSCampos.Tables["AgrupadorXY"]);
                lHTParam.Add("GroupByFieldsXY", lsFieldsXY);
            }
            if (!lHTParam.ContainsKey("GroupByFields"))
            {
                lsFieldsY = lHTParam["GroupByFieldsY"].ToString();
                lsFieldsX = lHTParam["GroupByFieldsX"].ToString();
                lsFieldsXY = lHTParam["GroupByFieldsXY"].ToString();
                lsbFields.Length = 0;
                lsbFields.Append(lsFieldsY);
                if (!String.IsNullOrEmpty(lsFieldsY)
                    && !String.IsNullOrEmpty(lsFieldsX))
                {
                    lsbFields.AppendLine(",");
                }
                lsbFields.Append(lsFieldsX);
                if (!String.IsNullOrEmpty(lsFieldsX)
                    && !String.IsNullOrEmpty(lsFieldsXY))
                {
                    lsbFields.AppendLine(",");
                }
                lsbFields.Append(lsFieldsXY);
                lHTParam.Add("GroupByFields", lsbFields.ToString());
            }

            //Agregar GroupByMatrizFields
            if (!lHTParam.ContainsKey("GroupByMatrizFields"))
            {
                lsFieldsY = ReporteEstandarUtil.GetGroupByMatrizFields(lDSCampos.Tables["CamposY"]);
                lHTParam.Add("GroupByMatrizFields", lsFieldsY);
            }

            //Agregar OrderByFieldsX
            string lsSortDir;
            string lsSortDirInv;
            if (!lHTParam.ContainsKey("OrderByFieldsX"))
            {
                lsFieldsX = ReporteEstandarUtil.GetOrderByFields(lDSCampos.Tables["CamposX"], 1, out lsSortDir);
                lHTParam.Add("OrderByFieldsX", lsFieldsX);
                lHTParam.Add("SortDirX", lsSortDir);
            }

            if (!lHTParam.ContainsKey("OrderByFieldsXInv"))
            {
                lsFieldsX = ReporteEstandarUtil.GetOrderByFields(lDSCampos.Tables["CamposX"], 2, out lsSortDirInv);
                lHTParam.Add("OrderByFieldsXInv", lsFieldsX);
                lHTParam.Add("SortDirXInv", lsSortDirInv);
            }
        }

        public static void GetValoresEjeX(string lsIdioma, DataRow lKDBRowReporte, DataRow lKDBRowDataSource, DataSet lDSCampos, Hashtable lHTParam, bool lbReajustarParam)
        {
            //Obtener las columnas para el eje X
            int liBanderas = (int)lKDBRowReporte["{BanderasRepEstandar}"];
            bool lbModoDebug = (liBanderas & 128) == 128;

            //string lsDataSource = ReporteEstandarUtil.ParseDataSource(lKDBRowDataSource["{RepEstDataSourceEjeX}"].ToString(), lHTParam, lbReajustarParam);
            //if (lbModoDebug)
            //{
            //    LogDataSource(lsIdioma, TipoReporte.Matricial, lKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceEjeX", lsDataSource);
            //}

            //DataTable lValoresEjeX = DSODataAccess.Execute(lsDataSource);

            //lValoresEjeX.TableName = "ValoresEjeX";
            //if (lDSCampos.Tables.Contains("ValoresEjeX"))
            //{
            //    lDSCampos.Tables.Remove("ValoresEjeX");
            //}
            //lDSCampos.Tables.Add(lValoresEjeX);

            DataTable lValoresEjeX;
            if (lDSCampos.Tables.Contains("ValoresEjeX"))
            {
                lValoresEjeX = lDSCampos.Tables["ValoresEjeX"];
            }
            else
            {
                string lsDataSource = ReporteEstandarUtil.ParseDataSource(lKDBRowDataSource["{RepEstDataSourceEjeX}"].ToString(), lHTParam, lbReajustarParam);
                if (lbModoDebug)
                {
                    LogDataSource(lsIdioma, TipoReporte.Matricial, lKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceEjeX", lsDataSource);
                }
                lValoresEjeX = DSODataAccess.Execute(lsDataSource);
                lValoresEjeX.TableName = "ValoresEjeX";
                lDSCampos.Tables.Add(lValoresEjeX);
            }

            //Agrego los parametros que dependen de los Valores del Eje X
            string lsFieldsY;
            string lsFieldsX;
            string lsFieldsXY;
            StringBuilder lsbFields = new StringBuilder();

            lsFieldsXY = ReporteEstandarUtil.GetMatrizFields(lDSCampos);

            if (lHTParam.ContainsKey("MatrizFields"))
            {
                lHTParam["MatrizFields"] = lsFieldsXY;
            }
            else
            {
                lHTParam.Add("MatrizFields", lsFieldsXY);
            }

            lsFieldsY = lHTParam["TopFieldsY"].ToString();
            lsFieldsXY = ReporteEstandarUtil.GetTopFieldsMatriz(lDSCampos.Tables["TotalizadosXYTotY"], lValoresEjeX.Rows.Count);
            lsFieldsX = lHTParam["TopFieldsXYTotX"].ToString();

            lsbFields.Append(lsFieldsY);
            if (!String.IsNullOrEmpty(lsFieldsY)
                && !String.IsNullOrEmpty(lsFieldsXY))
            {
                lsbFields.AppendLine(",");
            }
            lsbFields.Append(lsFieldsXY);
            if (!String.IsNullOrEmpty(lsFieldsXY)
                && !String.IsNullOrEmpty(lsFieldsX))
            {
                lsbFields.AppendLine(",");
            }
            lsbFields.Append(lsFieldsX);


            if (lHTParam.ContainsKey("TopFieldsXYTotY"))
            {
                lHTParam["TopFieldsXYTotY"] = lsFieldsXY;
            }
            else
            {
                lHTParam.Add("TopFieldsXYTotY", lsFieldsXY);
            }

            if (lHTParam.ContainsKey("TopFields"))
            {
                lHTParam["TopFields"] = lsbFields.ToString();
            }
            else
            {
                lHTParam.Add("TopFields", lsbFields.ToString());
            }

        }

        public static string ParseDataSource(string lsDataSource, Hashtable lHTParam, bool lbReajustarParametros)
        {
            string lsRet = lsDataSource;

            foreach (string lsKey in lHTParam.Keys)
            {
                if (lbReajustarParametros)
                {
                    lsRet = lsRet.Replace("Param(" + lsKey + ")", lHTParam[lsKey].ToString().Replace("'", "''"));
                }
                else
                {
                    lsRet = lsRet.Replace("Param(" + lsKey + ")", lHTParam[lsKey].ToString());
                }
            }

            return lsRet;
        }

        public static void LogDataSource(string lsIdioma, TipoReporte lTipoReporte, string lsCodReporte, string lsCodDataSource, string lsDataSource)
        {
            try
            {
                StringBuilder lsb = new StringBuilder();
                lsb.Append("--");
                if (lTipoReporte == TipoReporte.Tabular)
                {
                    lsb.AppendLine(GetLangItem(lsIdioma, "RepEst", "Tabular", lsCodReporte));
                }
                else if (lTipoReporte == TipoReporte.Resumido)
                {
                    lsb.AppendLine(GetLangItem(lsIdioma, "RepEst", "Resumido", lsCodReporte));
                }
                else if (lTipoReporte == TipoReporte.Matricial)
                {
                    lsb.AppendLine(GetLangItem(lsIdioma, "RepEst", "Matricial", lsCodReporte));
                }
                lsb.Append("--");
                lsb.AppendLine(GetLangItem(lsIdioma, "Atrib", "Atributos", lsCodDataSource));

                lsb.AppendLine(lsDataSource);
                Util.LogMessage(lsb.ToString());
            }
            catch
            {
            }
        }

        public static string GetDataFields(DataTable lKDBCampos)
        {
            StringBuilder lsb = new StringBuilder();

            foreach (DataRow lRow in lKDBCampos.Rows)
            {
                if (lsb.Length > 0)
                {
                    lsb.AppendLine(",");
                }
                lsb.Append(lRow["{DataField}"].ToString());
                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    lsb.AppendLine(",");
                    lsb.Append(lRow["{DataFieldRuta}"].ToString());
                }
                if (lKDBCampos.Columns.Contains("{DataFieldRutaAd}")
                    && lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    lsb.AppendLine(",");
                    lsb.Append(lRow["{DataFieldRutaAd}"].ToString());
                }
            }

            return lsb.ToString();
        }

        public static string GetDataFieldsResumido(DataSet lDSCampos)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(GetDataFields(lDSCampos.Tables["Campos"]));
            if (lsb.Length > 0)
            {
                lsb.AppendLine(",");
            }
            lsb.Append(GetGroupingFields(lDSCampos.Tables["Agrupadores"]));

            return lsb.ToString();
        }

        public static string GetMatrizFields(DataSet lDSCampos)
        {
            StringBuilder lsb = new StringBuilder();
            KDBAccess lKDB = new KDBAccess();
            DataTable lKDBMatrizFunc = lKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib} = (select iCodRegistro from Catalogos where vchCodigo = 'MatrizFunc' and iCodCatalogo = (select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'))");
            string lsMatrizFunc;
            string lsDataField;
            string lsDataFieldRuta;
            string lsDataFieldRutaAd;

            DataRow lRowXVal;
            int lidx;

            //Agrego los campos del eje Y
            foreach (DataRow lRow in lDSCampos.Tables["CamposY"].Rows)
            {
                if (lsb.Length > 0)
                {
                    lsb.AppendLine(",");
                }
                lsDataField = GetDataFieldName(lRow, false);
                lsb.Append(lsDataField);
                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    lsDataField = GetDataFieldName(lRow, "{DataFieldRuta}", false);
                    lsb.AppendLine(",");
                    lsb.Append(lRow["{DataFieldRuta}"].ToString());
                }
                if (lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    lsDataField = GetDataFieldName(lRow, "{DataFieldRutaAd}", false);
                    lsb.AppendLine(",");
                    lsb.Append(lRow["{DataFieldRutaAd}"].ToString());
                }
            }

            //Agrego los campos del eje X
            foreach (DataRow lRow in lDSCampos.Tables["CamposXY"].Rows)
            {
                lsMatrizFunc = lKDBMatrizFunc.Select("[{Value}] = " + lRow["{MatrizFunc}"])[0]["vchDescripcion"].ToString() + "(";
                lsDataField = GetDataFieldName(lRow);
                lsDataFieldRuta = null;
                lsDataFieldRutaAd = null;
                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    lsDataFieldRuta = GetDataFieldName(lRow, "{DataFieldRuta}");
                }
                if (lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    lsDataFieldRutaAd = GetDataFieldName(lRow, "{DataFieldRutaAd}");
                }


                for (lidx = 0; lidx < lDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
                {
                    lRowXVal = lDSCampos.Tables["ValoresEjeX"].Rows[lidx];
                    if (lsb.Length > 0)
                    {
                        lsb.AppendLine(",");
                    }

                    lsb.Append("[ColX" + lidx + "_" + lsDataField + "] = " + lsMatrizFunc + "case when ");
                    lsb.Append(GetCondicionesValoresX(lDSCampos.Tables["CamposX"], lRowXVal));
                    lsb.Append(" then [" + lsDataField + "] else null end)");

                    if (!String.IsNullOrEmpty(lsDataFieldRuta))
                    {
                        lsb.AppendLine(",");
                        lsb.Append("[ColX" + lidx + "_" + lsDataFieldRuta + "] = " + lsMatrizFunc + "case when ");
                        lsb.Append(GetCondicionesValoresX(lDSCampos.Tables["CamposX"], lRowXVal));
                        lsb.Append(" then [" + lsDataFieldRuta + "] else null end)");
                    }
                    if (!String.IsNullOrEmpty(lsDataFieldRutaAd))
                    {
                        lsb.AppendLine(",");
                        lsb.Append("[ColX" + lidx + "_" + lsDataFieldRutaAd + "] = " + lsMatrizFunc + "case when ");
                        lsb.Append(GetCondicionesValoresX(lDSCampos.Tables["CamposX"], lRowXVal));
                        lsb.Append(" then [" + lsDataFieldRutaAd + "] else null end)");
                    }
                }

                //Revisar si es totalizado para los valores del eje X
                if (((int)lRow["{BanderasCamposXY}"] & 2) == 2)
                {
                    lsb.AppendLine(",");
                    lsb.Append("[" + lsDataField + "] = " + lsMatrizFunc + "[" + lsDataField + "])");

                    if (!String.IsNullOrEmpty(lsDataFieldRuta))
                    {
                        lsb.AppendLine(",");
                        lsb.Append("[" + lsDataFieldRuta + "] = " + lsMatrizFunc + "[" + lsDataFieldRuta + "])");
                    }
                    if (!String.IsNullOrEmpty(lsDataFieldRutaAd))
                    {
                        lsb.AppendLine(",");
                        lsb.Append("[" + lsDataFieldRutaAd + "] = " + lsMatrizFunc + "[" + lsDataFieldRutaAd + "])");
                    }
                }
            }

            return lsb.ToString();
        }

        private static string GetCondicionesValoresX(DataTable lKDBCamposX, DataRow lRowXVal)
        {
            StringBuilder lsb = new StringBuilder();
            string lsDataFieldX;
            string lsDataFieldXRuta;
            string lsCondicion = "";

            foreach (DataRow lRowX in lKDBCamposX.Rows)
            {
                lsDataFieldX = GetDataFieldName(lRowX);
                lsb.Append(lsCondicion);
                lsb.Append("[" + lsDataFieldX + "] = ");
                lsb.Append(GetValorX(lRowXVal[lsDataFieldX]));

                if (String.IsNullOrEmpty(lsCondicion))
                {
                    lsCondicion = " AND ";
                }

                if (lRowX["{DataFieldRuta}"] != DBNull.Value)
                {
                    lsDataFieldXRuta = GetDataFieldName(lRowX, "{DataFieldRuta}");
                    lsb.Append(lsCondicion);
                    lsb.Append("[" + lsDataFieldXRuta + "] = ");
                    lsb.Append(GetValorX(lRowXVal[lsDataFieldXRuta]));
                }
            }
            return lsb.ToString();
        }

        private static string GetValorX(object lValor)
        {
            string lsValorX;

            if (lValor == DBNull.Value)
            {
                lsValorX = "null";
            }
            else if (lValor is string)
            {
                lsValorX = "'" + lValor.ToString().Replace("'", "''") + "'";
            }
            else if (lValor is DateTime)
            {
                lsValorX = "'" + ((DateTime)lValor).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            }
            else
            {
                lsValorX = lValor.ToString();
            }
            return lsValorX;
        }

        public static string GetTopFields(DataTable lKDBCamposTot)
        {
            KDBAccess lKDB = new KDBAccess();
            StringBuilder lsb = new StringBuilder();
            DataTable lKDBAggregates = lKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib} = (select iCodRegistro from Catalogos where vchCodigo = 'AggregateFunc' and iCodCatalogo = (select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'))");
            string lsTopField;
            string lsAggregate;

            foreach (DataRow lRow in lKDBCamposTot.Rows)
            {
                if (lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lsAggregate = lKDBAggregates.Select("[{Value}] = " + lRow["{AggregateFunc}"])[0]["vchCodigo"].ToString() + "(";

                    lsTopField = GetDataFieldName(lRow, false);
                    lsTopField += " = " + lsAggregate + lsTopField + ")";
                    if (lsb.Length > 0)
                    {
                        lsb.AppendLine(",");
                    }
                    lsb.Append(lsTopField);
                }
            }

            return lsb.ToString();
        }

        public static string GetTopFieldsMatriz(DataTable lKDBCamposTot, int liColXNum)
        {
            KDBAccess lKDB = new KDBAccess();
            StringBuilder lsb = new StringBuilder();
            DataTable lKDBAggregates = lKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib} = (select iCodRegistro from Catalogos where vchCodigo = 'AggregateFunc' and iCodCatalogo = (select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'))");
            string lsTopField;
            string lsTopFieldColX;
            string lsAggregate;
            int lidx;

            foreach (DataRow lRow in lKDBCamposTot.Rows)
            {
                if (lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lsAggregate = lKDBAggregates.Select("[{Value}] = " + lRow["{AggregateFunc}"])[0]["vchCodigo"].ToString() + "(";

                    lsTopField = GetDataFieldName(lRow);

                    for (lidx = 0; lidx < liColXNum; lidx++)
                    {
                        lsTopFieldColX = "[ColX" + lidx + "_" + lsTopField + "]";
                        lsTopFieldColX += " = " + lsAggregate + lsTopFieldColX + ")";

                        if (lsb.Length > 0)
                        {
                            lsb.AppendLine(",");
                        }
                        lsb.Append(lsTopFieldColX);
                    }
                }
            }

            return lsb.ToString();
        }

        public static string GetGroupByFields(DataTable lKDBAgrupadores)
        {
            return GetGroupByFields(lKDBAgrupadores, null);
        }

        public static string GetGroupByFields(DataTable lKDBAgrupadores, string lsOperAgrupacion)
        {
            StringBuilder lsb = new StringBuilder();

            foreach (DataRow lRow in lKDBAgrupadores.Rows)
            {
                if (IsValidGroupByField(lRow))
                {
                    if (lsb.Length > 0)
                    {
                        lsb.AppendLine(",");
                    }
                    lsb.Append(GetGroupField(lRow));
                }

                if (lRow["{DataFieldRuta}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRuta}"))
                {
                    if (lsb.Length > 0)
                    {
                        lsb.AppendLine(",");
                    }
                    lsb.Append(GetGroupField(lRow, "{DataFieldRuta}"));
                }

                if (lKDBAgrupadores.Columns.Contains("{DataFieldRutaAd}")
                    && lRow["{DataFieldRutaAd}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRutaAd}"))
                {
                    if (lsb.Length > 0)
                    {
                        lsb.AppendLine(",");
                    }
                    lsb.Append(GetGroupField(lRow, "{DataFieldRutaAd}"));
                }
            }

            if (lsb.Length > 0 && !String.IsNullOrEmpty(lsOperAgrupacion))
            {
                lsb.AppendLine();
                lsb.AppendLine(lsOperAgrupacion);
            }

            return lsb.ToString();
        }

        public static string GetGroupByFieldsResumido(DataTable lKDBAgrupadores, string lsOperAgrupacion)
        {
            StringBuilder lsbCondiciones = new StringBuilder();
            StringBuilder lsbGroupingAll = new StringBuilder(); //Se utiliza para quitar el registro que tiene todos los grouping = 1
            StringBuilder lsbRet = new StringBuilder();
            bool lbAppendCondicionDataField;
            bool lbAppendCondicionRuta;
            bool lbAppendCondicionRutaAd;

            foreach (DataRow lRow in lKDBAgrupadores.Rows)
            {
                lbAppendCondicionDataField = false;
                lbAppendCondicionRuta = false;
                lbAppendCondicionRutaAd = false;
                if (IsValidGroupByField(lRow))
                {
                    if (lsbGroupingAll.Length > 0)
                    {
                        lsbGroupingAll.Append("And ");
                    }
                    lsbGroupingAll.Append(GetGroupingField(lRow));
                    lsbGroupingAll.AppendLine(" = 1");
                    lbAppendCondicionDataField = true;
                }

                if (lRow["{DataFieldRuta}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRuta}"))
                {
                    if (lsbGroupingAll.Length > 0)
                    {
                        lsbGroupingAll.Append("And ");
                    }
                    lsbGroupingAll.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbGroupingAll.AppendLine(" = 1");
                    lbAppendCondicionRuta = true;
                }

                if (lRow["{DataFieldRutaAd}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRutaAd}"))
                {
                    if (lsbGroupingAll.Length > 0)
                    {
                        lsbGroupingAll.Append("And ");
                    }
                    lsbGroupingAll.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbGroupingAll.AppendLine(" = 1");
                    lbAppendCondicionRutaAd = true;
                }

                if (lbAppendCondicionDataField && lbAppendCondicionRuta && lbAppendCondicionRutaAd)
                {
                    if (lsbCondiciones.Length > 0)
                    {
                        lsbCondiciones.Append("AND ");
                    }

                    lsbCondiciones.Append("((");
                    lsbCondiciones.Append(GetGroupingField(lRow));
                    lsbCondiciones.Append(" = 1 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbCondiciones.AppendLine(" = 1 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbCondiciones.AppendLine(" = 1)");

                    lsbCondiciones.Append("or (");
                    lsbCondiciones.Append(GetGroupingField(lRow));
                    lsbCondiciones.Append(" = 0 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbCondiciones.AppendLine(" = 0 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbCondiciones.AppendLine(" = 0))");
                }
                else if (lbAppendCondicionDataField && lbAppendCondicionRuta && !lbAppendCondicionRutaAd)
                {
                    if (lsbCondiciones.Length > 0)
                    {
                        lsbCondiciones.Append("AND ");
                    }

                    lsbCondiciones.Append("((");
                    lsbCondiciones.Append(GetGroupingField(lRow));
                    lsbCondiciones.Append(" = 1 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbCondiciones.AppendLine(" = 1)");

                    lsbCondiciones.Append("or (");
                    lsbCondiciones.Append(GetGroupingField(lRow));
                    lsbCondiciones.Append(" = 0 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbCondiciones.AppendLine(" = 0))");
                }
                else if (lbAppendCondicionDataField && !lbAppendCondicionRuta && lbAppendCondicionRutaAd)
                {
                    if (lsbCondiciones.Length > 0)
                    {
                        lsbCondiciones.Append("AND ");
                    }

                    lsbCondiciones.Append("((");
                    lsbCondiciones.Append(GetGroupingField(lRow));
                    lsbCondiciones.Append(" = 1 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbCondiciones.AppendLine(" = 1)");

                    lsbCondiciones.Append("or (");
                    lsbCondiciones.Append(GetGroupingField(lRow));
                    lsbCondiciones.Append(" = 0 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbCondiciones.AppendLine(" = 0))");
                }
                else if (!lbAppendCondicionDataField && lbAppendCondicionRuta && lbAppendCondicionRutaAd)
                {
                    if (lsbCondiciones.Length > 0)
                    {
                        lsbCondiciones.Append("AND ");
                    }

                    lsbCondiciones.Append("((");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbCondiciones.AppendLine(" = 1 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbCondiciones.AppendLine(" = 1)");

                    lsbCondiciones.Append("or (");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbCondiciones.AppendLine(" = 0 And ");
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbCondiciones.AppendLine(" = 0))");
                }
            }
            lsbRet.Append(GetGroupByFields(lKDBAgrupadores, lsOperAgrupacion));
            if (lsbGroupingAll.Length > 0)
            {
                lsbRet.Append("Having Not(");
                lsbRet.Append(lsbGroupingAll.ToString());
                lsbRet.AppendLine(")");
            }
            if (lsbCondiciones.Length > 0)
            {
                lsbRet.Append("And (");
                lsbRet.Append(lsbCondiciones.ToString());
                lsbRet.Append(")");
            }

            return lsbRet.ToString();
        }

        public static string GetTopFieldsGroupBy(DataTable lKDBAgrupadores, string lsOperAgrupacion)
        {
            StringBuilder lsbCondiciones = new StringBuilder();
            StringBuilder lsbRet = new StringBuilder();

            foreach (DataRow lRow in lKDBAgrupadores.Rows)
            {
                if (IsValidGroupByField(lRow))
                {
                    if (lsbCondiciones.Length > 0)
                    {
                        lsbCondiciones.Append("AND ");
                    }
                    lsbCondiciones.Append(GetGroupingField(lRow));
                    lsbCondiciones.AppendLine(" = 0");
                }

                if (lRow["{DataFieldRuta}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRuta}"))
                {
                    if (lsbCondiciones.Length > 0)
                    {
                        lsbCondiciones.Append("AND ");
                    }
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRuta}"));
                    lsbCondiciones.AppendLine(" = 0");
                }

                if (lRow["{DataFieldRutaAd}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRutaAd}"))
                {
                    if (lsbCondiciones.Length > 0)
                    {
                        lsbCondiciones.Append("AND ");
                    }
                    lsbCondiciones.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                    lsbCondiciones.AppendLine(" = 0");
                }
            }

            lsbRet.AppendLine(GetGroupByFields(lKDBAgrupadores, lsOperAgrupacion));
            if (lsbCondiciones.Length > 0)
            {
                lsbRet.AppendLine("Having");
                lsbRet.AppendLine(lsbCondiciones.ToString());
            }
            return lsbRet.ToString();
        }

        public static string GetGroupField(DataRow lKDBRowCampo)
        {
            return GetGroupField(lKDBRowCampo, "{DataField}");
        }

        public static string GetGroupField(DataRow lKDBRowCampo, string lsColumna)
        {
            if (lKDBRowCampo[lsColumna].ToString().Split('=').Length > 1)
            {
                return lKDBRowCampo[lsColumna].ToString().Substring(lKDBRowCampo[lsColumna].ToString().IndexOf('=') + 1);
            }

            return lKDBRowCampo[lsColumna].ToString();
        }

        public static string GetGroupingFieldName(DataRow lKDBRowCampo)
        {
            return GetGroupingFieldName(lKDBRowCampo, "{DataField}", true);
        }

        public static string GetGroupingFieldName(DataRow lKDBRowCampo, bool lbEscapeBrackets)
        {
            return GetGroupingFieldName(lKDBRowCampo, "{DataField}", lbEscapeBrackets);
        }

        public static string GetGroupingFieldName(DataRow lKDBRowCampo, string lsColumna)
        {
            return GetGroupingFieldName(lKDBRowCampo, lsColumna, true);
        }

        public static string GetGroupingFieldName(DataRow lKDBRowCampo, string lsColumna, bool lbEscapeBrackets)
        {
            if (lbEscapeBrackets)
            {
                return "Grouping" + GetDataFieldName(lKDBRowCampo, lsColumna);
            }
            else
            {
                return "[Grouping" + GetDataFieldName(lKDBRowCampo, lsColumna) + "]";
            }
        }

        public static string GetGroupingField(DataRow lKDBRowCampo)
        {
            return GetGroupingField(lKDBRowCampo, "{DataField}");
        }

        public static string GetGroupingField(DataRow lKDBRowCampo, string lsColumna)
        {
            return "Grouping(" + GetGroupField(lKDBRowCampo, lsColumna) + ")";
        }

        public static string GetGroupingFields(DataTable lKDBAgrupadores)
        {
            StringBuilder lsb = new StringBuilder();
            bool lbAppendCommaRuta;

            foreach (DataRow lRow in lKDBAgrupadores.Rows)
            {
                if (lsb.Length > 0)
                {
                    lsb.AppendLine(",");
                }
                lbAppendCommaRuta = false;
                if (IsValidGroupByField(lRow))
                {
                    lsb.Append(GetGroupingFieldName(lRow, false));
                    lsb.Append(" = ");
                    lsb.Append(GetGroupingField(lRow));

                    lbAppendCommaRuta = true;
                }

                if (lRow["{DataFieldRuta}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRuta}"))
                {
                    if (lbAppendCommaRuta)
                    {
                        lsb.AppendLine(",");
                    }
                    lsb.Append(GetGroupingFieldName(lRow, "{DataFieldRuta}", false));
                    lsb.Append(" = ");
                    lsb.Append(GetGroupingField(lRow, "{DataFieldRuta}"));

                    lbAppendCommaRuta = true;
                }
                if (lRow["{DataFieldRutaAd}"] != DBNull.Value
                    && IsValidGroupByField(lRow, "{DataFieldRutaAd}"))
                {
                    if (lbAppendCommaRuta)
                    {
                        lsb.AppendLine(",");
                    }
                    lsb.Append(GetGroupingFieldName(lRow, "{DataFieldRutaAd}", false));
                    lsb.Append(" = ");
                    lsb.Append(GetGroupingField(lRow, "{DataFieldRutaAd}"));
                }
            }

            return lsb.ToString();
        }

        public static bool IsValidGroupByField(DataRow lKDBRowCampo)
        {
            return IsValidGroupByField(lKDBRowCampo, "{DataField}");
        }

        public static bool IsValidGroupByField(DataRow lKDBRowCampo, string lsColumna)
        {
            return !Regex.IsMatch(GetGroupField(lKDBRowCampo, lsColumna), @"(sum|avg|count|min|max|grouping)\s*\(", RegexOptions.IgnoreCase);
        }

        public static string GetGroupByMatrizFields(DataTable lKDBCamposY)
        {
            StringBuilder lsb = new StringBuilder();

            foreach (DataRow lRow in lKDBCamposY.Rows)
            {
                if (lsb.Length > 0)
                {
                    lsb.AppendLine(",");
                }
                lsb.Append(GetDataFieldName(lRow, false));

                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    lsb.AppendLine(",");
                    lsb.Append(GetDataFieldName(lRow, "{DataFieldRuta}", false));
                }

                if (lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    lsb.AppendLine(",");
                    lsb.Append(GetDataFieldName(lRow, "{DataFieldRutaAd}", false));
                }
            }

            return lsb.ToString();
        }

        public static string GetOrderByFields(DataTable lKDBCampos, int liDir, out string lsSort)
        {
            StringBuilder lsb = new StringBuilder();
            DataView lDataView = lKDBCampos.DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            lsSort = "";
            string lsDir;
            string lsDirInv;
            if (liDir == 1)
            {
                lsDir = " Asc";
                lsDirInv = " Desc";
            }
            else
            {
                lsDir = " Desc";
                lsDirInv = " Asc";
            }
            foreach (DataRowView lViewRow in lDataView)
            {
                if (lsb.Length > 0)
                {
                    lsb.AppendLine(",");
                }
                lsb.Append(GetDataFieldName(lViewRow.Row, false));

                if (lViewRow["{RepEstDirDatos}"] == DBNull.Value
                    || int.Parse(lViewRow["{RepEstDirDatos}"].ToString()) == 1)
                {
                    if (String.IsNullOrEmpty(lsSort))
                    {
                        lsSort = lsDir;
                    }
                    lsb.Append(lsDir);
                }
                else
                {
                    if (String.IsNullOrEmpty(lsSort))
                    {
                        lsSort = lsDirInv;
                    }
                    lsb.Append(lsDirInv);
                }

            }

            return lsb.ToString();
        }

        public static string FormatearValor(object lValor, string lsIdioma, string lStringFormat)
        {
            return FormatearValor(lValor, lsIdioma, lStringFormat, null);
        }

        public static string FormatearValor(object lValor, string lsIdioma, string lStringFormat, IFormatProvider lFormat)
        {
            string lsRet = "";
            if (lStringFormat == "TimeSeg")
            {
                lsRet = TimeSegToString(lValor, lsIdioma);
            }
            else if (lStringFormat == "Time")
            {
                lsRet = TimeToString(lValor, lsIdioma);
            }
            else if (lValor is DateTime)
            {
                if (lFormat != null)
                {
                    lsRet = ((DateTime)lValor).ToString(lStringFormat, lFormat);
                }
                else
                {
                    lsRet = ((DateTime)lValor).ToString(lStringFormat);
                }
            }
            else if (lValor is byte)
            {
                if (lFormat != null)
                {
                    lsRet = ((byte)lValor).ToString(lStringFormat, lFormat);
                }
                else
                {
                    lsRet = ((byte)lValor).ToString(lStringFormat);
                }
            }
            else if (lValor is int)
            {
                if (lFormat != null)
                {
                    lsRet = ((int)lValor).ToString(lStringFormat, lFormat);
                }
                else
                {
                    lsRet = ((int)lValor).ToString(lStringFormat);
                }
            }
            else if (lValor is double)
            {
                if (lFormat != null)
                {
                    lsRet = ((double)lValor).ToString(lStringFormat, lFormat);
                }
                else
                {
                    lsRet = ((double)lValor).ToString(lStringFormat);
                }
            }
            else if (lValor is decimal)
            {
                if (lFormat != null)
                {
                    lsRet = ((decimal)lValor).ToString(lStringFormat, lFormat);
                }
                else
                {
                    lsRet = ((decimal)lValor).ToString(lStringFormat);
                }
            }
            else
            {
                lsRet = lValor.ToString();
            }
            return lsRet;
        }

        public static DataRow GetCampoByDataField(string lsDataField, DataTable lKDBCampos)
        {
            return GetCampoByDataField(lsDataField, "{DataField}", lKDBCampos);
        }

        public static DataRow GetCampoByDataField(string lsDataField, string lsColumna, DataTable lKDBCampos)
        {
            DataRow lRowCampo = null;
            if (lKDBCampos.Select("[" + lsColumna + "] like '" + lsDataField + " =%'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] like '" + lsDataField + " =%'")[0];
            }
            else if (lKDBCampos.Select("[" + lsColumna + "] like '" + lsDataField + "=%'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] like '" + lsDataField + "=%'")[0];
            }
            else if (lKDBCampos.Select("[" + lsColumna + "] = '" + lsDataField + "'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] = '" + lsDataField + "'")[0];
            }

            else if (lKDBCampos.Select("[" + lsColumna + "] like '[[]" + lsDataField + "[]] =%'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] like '[[]" + lsDataField + "[]] =%'")[0];
            }
            else if (lKDBCampos.Select("[" + lsColumna + "] like '[[]" + lsDataField + "[]]=%'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] like '[[]" + lsDataField + "[]]=%'")[0];
            }
            else if (lKDBCampos.Select("[" + lsColumna + "] = '[" + lsDataField + "]'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] = '[" + lsDataField + "]'")[0];
            }

            else if (lKDBCampos.Select("[" + lsColumna + "] like '\"" + lsDataField + "\" =%'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] like '\"" + lsDataField + "\" =%'")[0];
            }
            else if (lKDBCampos.Select("[" + lsColumna + "] like '\"" + lsDataField + "\"=%'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] like '\"" + lsDataField + "\"=%'")[0];
            }
            else if (lKDBCampos.Select("[" + lsColumna + "] = '\"" + lsDataField + "\"'").Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[" + lsColumna + "] = '\"" + lsDataField + "\"'")[0];
            }
            else
            {
                string lsDataFieldAux;
                foreach (DataRow lRow in lKDBCampos.Rows)
                {
                    lsDataFieldAux = EscapeColumnName(GetDataFieldName(lRow, lsColumna));
                    if (lsDataFieldAux == lsDataField)
                    {
                        lRowCampo = lRow;
                        break;
                    }
                }
            }
            return lRowCampo;
        }

        public static string EscapeColumnName(string lsName)
        {
            lsName = lsName.Replace("á", "a");
            lsName = lsName.Replace("é", "e");
            lsName = lsName.Replace("í", "i");
            lsName = lsName.Replace("ó", "o");
            lsName = lsName.Replace("ú", "u");
            lsName = lsName.Replace("Á", "A");
            lsName = lsName.Replace("É", "E");
            lsName = lsName.Replace("Í", "I");
            lsName = lsName.Replace("Ó", "O");
            lsName = lsName.Replace("Ú", "U");
            lsName = lsName.Replace("ñ", "n");
            lsName = lsName.Replace("Ñ", "N");
            return lsName;
        }

        public static DataRow GetCampoByMatrizField(string lsMatrizField, DataSet lDSCampos, int liColXNum)
        {
            return GetCampoByMatrizField(lsMatrizField, "{DataField}", lDSCampos, liColXNum);
        }

        public static DataRow GetCampoByMatrizField(string lsMatrizField, string lsColumna, DataSet lDSCampos, int liColXNum)
        {
            DataRow lRowCampo = null;

            //Primero reviso si es uno de los campos del eje Y
            lRowCampo = GetCampoByDataField(lsMatrizField, lsColumna, lDSCampos.Tables["CamposY"]);
            if (lRowCampo != null)
                return lRowCampo;

            //Reviso si es uno de los totalizados XY en el eje X
            lRowCampo = GetCampoByDataField(lsMatrizField, lsColumna, lDSCampos.Tables["CamposXY"]);
            if (lRowCampo != null)
                return lRowCampo;

            //Si llegue aqui entonces tiene que ser una columna XY por lo que debe de iniciar con "ColX" + lidx + "_"
            int lidx;
            string lsPrefijo;
            for (lidx = 0; lidx < liColXNum; lidx++)
            {
                lsPrefijo = "ColX" + lidx + "_";
                if (lsMatrizField.StartsWith(lsPrefijo))
                {
                    lRowCampo = GetCampoByDataField(lsMatrizField.Substring(lsPrefijo.Length), lsColumna, lDSCampos.Tables["CamposXY"]);
                    break;
                }
            }

            return lRowCampo;
        }

        public static string GetDataFieldName(DataRow lKDBRowCampo)
        {
            return GetDataFieldName(lKDBRowCampo, "{DataField}", true);
        }

        public static string GetDataFieldName(DataRow lKDBRowCampo, bool bEscape)
        {
            return GetDataFieldName(lKDBRowCampo, "{DataField}", bEscape);
        }

        public static string GetDataFieldName(DataRow lKDBRowCampo, string lsColumna)
        {
            return GetDataFieldName(lKDBRowCampo, lsColumna, true);
        }

        public static string GetDataFieldName(DataRow lKDBRowCampo, string lsColumna, bool lbEscapeBrackets)
        {
            string lsDataField = lKDBRowCampo[lsColumna].ToString().Split('=')[0].Trim();
            if (lbEscapeBrackets && lsDataField.StartsWith("[")
                && lsDataField.EndsWith("]"))
            {
                lsDataField = lsDataField.Remove(0, 1);
                lsDataField = lsDataField.Remove(lsDataField.LastIndexOf("]"), 1);
            }
            if (lbEscapeBrackets && lsDataField.StartsWith("\"")
                && lsDataField.EndsWith("\""))
            {
                lsDataField = lsDataField.Remove(0, 1);
                lsDataField = lsDataField.Remove(lsDataField.LastIndexOf("\""), 1);
            }
            return lsDataField;
        }

        protected static string GetMsgWeb(string lsIdioma, string lsElemento, params object[] lsParam)
        {
            return GetLangItem(lsIdioma, "MsgWeb", "Mensajes Web", lsElemento, lsParam);
        }

        public static string GetLangItem(string lsLang, string lsEntidad, string lsMaestro, string lsElemento, params object[] lsParam)
        {
            KDBAccess kdb = new KDBAccess();
            string lsRet = "#undefined-" + lsElemento + "#";
            string lsElem = null;

            lsElem = DSODataContext.GetObject(lsLang + "-" + lsEntidad + "-" + lsMaestro + "-" + lsElemento) as string;

            if (lsElem == null)
            {
                DataTable ldt = kdb.GetHisRegByEnt(lsEntidad, lsMaestro, "vchCodigo = '" + lsElemento + "'");

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    if (ldt.Columns.Contains("{" + lsLang + "}"))
                        lsElem = ldt.Rows[0]["{" + lsLang + "}"].ToString();
                    else
                        lsElem = ldt.Rows[0]["vchDescripcion"].ToString();

                    DSODataContext.SetObject(lsLang + "-" + lsEntidad + "-" + lsMaestro + "-" + lsElemento, lsElem);
                }
            }

            if (lsElem != null)
                lsRet = lsElem;

            return (lsParam == null ? lsRet : string.Format(lsRet, lsParam));
        }

        public static string TimeSegToString(object lValor, string lsIdioma)
        {
            StringBuilder lsbRet = new StringBuilder();
            string lsTimeSegD = GetMsgWeb(lsIdioma, "TimeSegD");
            string lsTimeSegHH = GetMsgWeb(lsIdioma, "TimeSegHH");
            string lsTimeSegmm = GetMsgWeb(lsIdioma, "TimeSegmm");
            string lsTimeSegss = GetMsgWeb(lsIdioma, "TimeSegss");

            int liDias;
            int liHoras;
            int liMinutos;
            int liSegundos;

            int liValor;
            if (int.TryParse(lValor.ToString(), out liValor))
            {
                liDias = liValor / 86400;
                liHoras = ((liValor / 3600) % 24);
                liMinutos = ((liValor / 60) % 60);
                liSegundos = (liValor % 60);

                if (liDias > 0)
                {
                    lsbRet.Append(liDias);
                    lsbRet.Append(lsTimeSegD);
                }

                lsbRet.Append(liHoras.ToString("00"));
                lsbRet.Append(lsTimeSegHH);
                lsbRet.Append(liMinutos.ToString("00"));
                lsbRet.Append(lsTimeSegmm);
                lsbRet.Append(liSegundos.ToString("00"));
                lsbRet.Append(lsTimeSegss);
            }
            return lsbRet.ToString();
        }

        public static string TimeToString(object lValor, string lsIdioma)
        {
            StringBuilder lsbRet = new StringBuilder();
            string lsTimeSegHH = GetMsgWeb(lsIdioma, "TimeSegHH");
            string lsTimeSegmm = GetMsgWeb(lsIdioma, "TimeSegmm");
            string lsTimeSegss = GetMsgWeb(lsIdioma, "TimeSegss");


            int liValor;
            float lfValor;
            if (int.TryParse(lValor.ToString(), out liValor))
            {
                int liHoras = (liValor / 3600);
                int liMinutos = ((liValor / 60) % 60);
                int liSegundos = (liValor % 60);

                lsbRet.Append(liHoras.ToString("00"));
                lsbRet.Append(lsTimeSegHH);
                lsbRet.Append(liMinutos.ToString("00"));
                lsbRet.Append(lsTimeSegmm);
                lsbRet.Append(liSegundos.ToString("00"));
                lsbRet.Append(lsTimeSegss);
            }
            else if (float.TryParse(lValor.ToString(), out lfValor))
            {
                float liHoras = (lfValor / 3600);
                float liMinutos = ((lfValor / 60) % 60);
                float liSegundos = (lfValor % 60);

                lsbRet.Append(liHoras.ToString("00"));
                lsbRet.Append(lsTimeSegHH);
                lsbRet.Append(liMinutos.ToString("00"));
                lsbRet.Append(lsTimeSegmm);
                lsbRet.Append(liSegundos.ToString("00"));
                lsbRet.Append(lsTimeSegss);
            }
            return lsbRet.ToString();
        }
        #endregion
    }
}
