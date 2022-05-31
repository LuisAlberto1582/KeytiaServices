using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DSOControls2008;
using KeytiaServiceBL;
using KeytiaCOM;

namespace KeytiaWeb.UserInterface
{
    public partial class ReporteUsuario : KeytiaPage
    {
        #region Campos
        protected Button btnAbrir;
        protected Button btnNuevo;
        protected HtmlButton btnGrabar;
        protected HtmlButton btnBaja;
        protected HtmlButton btnGenerar;

        protected DSOTabs poTabs;
        protected DSOAutocomplete poBaseReport;
        protected DSOAutocomplete poReportesUsuario;
        protected DSODropDownList cboReportType;
        protected DSORadioButtonList rblAccessType;
        protected DSOTextBox txtNombre;
        protected DSOTextBox txtDescripcion;

        protected Panel pnlFields;
        protected Panel pnlSummary;
        protected Panel pnlFieldOrder;
        protected Panel pnlCriteria;
        protected Panel pnlDataOrder;
        protected Panel pnlMail;
        protected Panel pnlGroup;
        protected Panel pnlGroupMat;
        protected Panel pnlGraph;
        protected Panel pnlOutput;

        protected Table tabStart;
        protected Table tabReport;
        protected Table tabFields;
        protected Table tabSummary;
        protected Table tabCriteria;
        protected Table tabCriteriaGrp;
        protected Table tabDataOrder;
        protected Table tabGroup;
        protected Table tabGroupMat;
        protected Table tabGraph;
        protected Table tabMail;
        protected Table tabOutput;
        protected TableCell tdOutput;

        protected bool pbRebuildFieldsDependents = false;

        protected ReporteUsuarioLang poLang = new ReporteUsuarioLang();
        protected ReporteUsuarioData poRepUsrData = new ReporteUsuarioData();
        protected ReporteUsuarioData poRepUsrDataClient;
        protected PrevData poPrevData = new PrevData();

        private List<ReporteUsuarioTab> plstTabs = new List<ReporteUsuarioTab>();
        private List<ReporteUsuarioItem> plstOrderTypes = new List<ReporteUsuarioItem>();
        private List<ReporteUsuarioItem> plstOperators = new List<ReporteUsuarioItem>();
        private List<ReporteUsuarioItem> plstGraphTypes = new List<ReporteUsuarioItem>();

        protected ReporteEstandar Reporte;
        protected bool lbRepGen = false;
        protected bool pbExport = false;
        protected KeytiaExportFormat poExport;

        protected int piCodRepStd = -1;
        #endregion


        #region Constructores
        public ReporteUsuario()
        {
            Init += new EventHandler(ReporteUsuario_Init);
        }
        #endregion




        #region Eventos

        void ReporteUsuario_Init(object sender, EventArgs e)
        {
            lblTitulo.Text =
                (string)Util.IsDBNull(KDBUtil.SearchScalar("OpcMnu", (string)Request.Params["Opc"], "{" + Globals.GetLanguage() + "}", true), "Reporte Usuario");

            EnsureChildControls();

            if (Request.Params[txtReportData.UniqueID] != null)
            {
                poRepUsrDataClient = DSOControl.DeserializeJSON<ReporteUsuarioData>(Request.Params[txtReportData.UniqueID]);
                poPrevData = DSOControl.DeserializeJSON<PrevData>(Request.Params[txtPrevData.UniqueID]);

                poRepUsrDataClient.ShowGraph = poPrevData.ShowGraph;

                SetReporte();

                poRepUsrDataClient = DSOControl.DeserializeJSON<ReporteUsuarioData>(Request.Params[txtReportData.UniqueID]);
            }
        }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (ViewState["ReportFields"] != null && (string)ViewState["ReportFields"] != "")
                poRepUsrData = DSOControl.DeserializeJSON<ReporteUsuarioData>((string)ViewState["ReportFields"]);
            else if (poRepUsrData == null)
                poRepUsrData = new ReporteUsuarioData();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;

            int liAux = -1;


            GetFields((string)ViewState["ReportType"], (string)ViewState["BaseReport"]);

            InitFields();

            if (IsPostBack &&
                ((ViewState["BaseReport"] != null && (string)ViewState["BaseReport"] != poBaseReport.TextValue.Text) ||
                (poRepUsrDataClient != null && int.TryParse(poReportesUsuario.TextValue.Text, out liAux) && poRepUsrDataClient.Id != liAux) ||
                (ViewState["ReportType"] != null && (string)ViewState["ReportType"] != cboReportType.DropDownList.SelectedValue)))
            {
                plstTabs.Clear();
                poRepUsrData.FieldsList.Clear();
                poRepUsrData.CategoriesList.Clear();
                poRepUsrData.CriteriasList.Clear();
                poRepUsrData.CriteriasGrpList.Clear();

                GetFields(cboReportType.DropDownList.SelectedValue, poBaseReport.TextValue.Text);

                //Si el reporte base cambia
                if ((string)ViewState["BaseReport"] != poBaseReport.TextValue.Text)
                {
                    poRepUsrDataClient = null;
                    poRepUsrData.Id = liAux;

                    txtDescripcion.DataValue = poRepUsrData.Description;

                    if (Reporte != null)
                        Reporte.Controls.Clear();

                    Reporte = null;
                    lbRepGen = false;
                    poRepUsrData.Generated = 0;
                }

                //Si el reporte de usuario cambia
                else if (poRepUsrDataClient != null && int.TryParse(poReportesUsuario.TextValue.Text, out liAux) && poRepUsrDataClient.Id != liAux)
                {
                    if (Reporte != null)
                        Reporte.Controls.Clear();

                    Reporte = null;
                    lbRepGen = false;

                    string lsOpen = poRepUsrDataClient.VchCodigoOpen;
                    poRepUsrDataClient = poRepUsrData.Clone();
                    poRepUsrDataClient.Id = liAux;
                    poRepUsrDataClient.Generated = 0;
                    poRepUsrDataClient.VchCodigoOpen = lsOpen;

                    poRepUsrDataClient.LoadUserReport();

                    if (poRepUsrDataClient.AccessType != -1)
                        rblAccessType.DataValue = poRepUsrDataClient.AccessType;
                    else
                        poRepUsrDataClient.AccessType = int.Parse(rblAccessType.DataValue.ToString());

                    if (poRepUsrDataClient.ReportType != -1)
                        cboReportType.DataValue = poRepUsrDataClient.ReportType;

                    lblTitulo.Text = poRepUsrDataClient.Name;
                    txtDescripcion.DataValue = poRepUsrDataClient.Description;
                    txtNombre.TextBox.Text = "";
                }

                InitFields();
            }

            poReportesUsuario.Source = "../../WebMethods.aspx/GetReportesUsuario?rb=" + poBaseReport.TextValue.Text;
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {

            if (poRepUsrDataClient != null && poRepUsrDataClient.Action == "save")
                btnGrabar_Click(null, null);
            else if (poRepUsrDataClient != null && poRepUsrDataClient.Action == "delete")
                btnBaja_Click(null, null);

            if (poRepUsrDataClient != null && poRepUsrDataClient.ShowGraph != poPrevData.ShowGraph)
            {
                ClearChildViewState();

                if (Reporte != null)
                    Reporte.Controls.Clear();

                Reporte = null;
                lbRepGen = false;
            }

            try
            {
                SetReporte();
            }
            catch (Exception ex)
            {
                Util.LogException("Error al generar el reporte usuario.", ex);
            }

            SetTabs();
            SetClientData();
        }

        void CheckBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbRebuildFieldsDependents = true;
        }

        void btnAbrir_Click(object sender, EventArgs e)
        {
            InitBaseReport();
            poBaseReport.DataValue = KDBUtil.SearchScalar("RepUsu", poRepUsrDataClient.VchCodigoOpen, "{RepUsu}");

            InitUserReport();
            poReportesUsuario.DataValue = KDBUtil.SearchScalar("RepUsu", poRepUsrDataClient.VchCodigoOpen, "iCodRegistro");

            plstTabs.Clear();
            poRepUsrData.FieldsList.Clear();
            poRepUsrData.CategoriesList.Clear();
            poRepUsrData.CriteriasList.Clear();
            poRepUsrData.CriteriasGrpList.Clear();

            GetFields(cboReportType.DropDownList.SelectedValue, poBaseReport.TextValue.Text);

            if (Reporte != null)
                Reporte.Controls.Clear();

            Reporte = null;
            lbRepGen = false;

            if ((string)poReportesUsuario.DataValue == "null")
            {
                Alert(GetLangItem("RUMsgNoRepSel"));
                return;
            }

            poRepUsrDataClient = poRepUsrData.Clone();
            poRepUsrDataClient.Id = int.Parse((string)poReportesUsuario.DataValue);
            poRepUsrDataClient.Generated = 0;

            poRepUsrDataClient.LoadUserReport();

            if (poRepUsrDataClient.AccessType != -1)
                rblAccessType.DataValue = poRepUsrDataClient.AccessType;
            else
                poRepUsrDataClient.AccessType = int.Parse(rblAccessType.DataValue.ToString());

            if (poRepUsrDataClient.ReportType != -1)
                cboReportType.DataValue = poRepUsrDataClient.ReportType;

            lblTitulo.Text = poRepUsrDataClient.Name;
            txtDescripcion.DataValue = poRepUsrDataClient.Description;
            txtNombre.TextBox.Text = "";

            InitFields();

            poReportesUsuario.Source = "../../WebMethods.aspx/GetReportesUsuario?rb=" + poBaseReport.TextValue.Text;

            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioBtnAbrir",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { oRepUsr.nextStep(); });" +
                "</script>\r\n",
                true, false);

            if (poRepUsrDataClient != null)
                poRepUsrDataClient.Modified = false;
        }

        void btnNuevo_Click(object sender, EventArgs e)
        {
            InitBaseReport();
            poBaseReport.DataValue = "";

            //InitUserReport();
            //poReportesUsuario.DataValue = KDBUtil.SearchScalar("RepUsu", poRepUsrDataClient.VchCodigoOpen, "iCodRegistro");

            //Llena el combo de reportes usuario
            DataTable ldtRU;
            DataRow ldr;

            ldtRU = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario", new string[] { "iCodRegistro", "vchDescripcion" });

            if (ldtRU != null)
            {
                ldtRU.Columns["iCodRegistro"].ColumnName = "id";
                ldtRU.Columns["vchDescripcion"].ColumnName = "value";

                ldr = ldtRU.NewRow();
                ldr["id"] = -1;
                ldr["value"] = GetLangItem("RUNuevo");

                ldtRU.Rows.InsertAt(ldr, 0);
            }

            poReportesUsuario.Source = "../../WebMethods.aspx/GetReportesUsuario?rb=" + poBaseReport.TextValue.Text;
            poReportesUsuario.DataSource = ldtRU;
            poReportesUsuario.Fill();
            ((TextBox)poReportesUsuario.Control).Text = " ";
            poReportesUsuario.DataValue = System.DBNull.Value;



            plstTabs.Clear();
            poRepUsrData.FieldsList.Clear();
            poRepUsrData.CategoriesList.Clear();
            poRepUsrData.CriteriasList.Clear();
            poRepUsrData.CriteriasGrpList.Clear();
            poRepUsrData.Name = "";
            poRepUsrData.LastCriteriaRow = "";
            poRepUsrData.LastCriteriaGrpRow = "";
            poRepUsrData.TopN = 0;
            poRepUsrData.TopDir = 0;
            poRepUsrData.Generated = 0;

            InitFields();

            poRepUsrDataClient = null;

            if (Reporte != null)
                Reporte.Controls.Clear();

            Reporte = null;
            lbRepGen = false;

            lblTitulo.Text = (string)Util.IsDBNull(KDBUtil.SearchScalar("OpcMnu", (string)Request.Params["Opc"], "{" + Globals.GetLanguage() + "}", true), "Reporte Usuario"); ;
            txtDescripcion.DataValue = "";
            txtNombre.TextBox.Text = "";


            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioBtnAbrir",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { oRepUsr.nextStep(); });" +
                "</script>\r\n",
                true, false);
            if (poRepUsrDataClient != null)
                poRepUsrDataClient.Modified = false;
        }

        void btnGrabar_Click(object sender, EventArgs e)
        {
            if (RepUsuValidate() && RepUsuExecValidate())
            {
                DataTable ldtRU;
                DataRow ldr;

                SaveReport();
                int liCodCatRepEst = SaveReportStd();
                SaveAlarm(liCodCatRepEst);
                SaveOpcMenu(liCodCatRepEst);


                //Llena el combo de reportes usuario
                ldtRU = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario", new string[] { "iCodRegistro", "vchDescripcion" });

                if (ldtRU != null)
                {
                    ldtRU.Columns["iCodRegistro"].ColumnName = "id";
                    ldtRU.Columns["vchDescripcion"].ColumnName = "value";

                    ldr = ldtRU.NewRow();
                    ldr["id"] = -1;
                    ldr["value"] = GetLangItem("RUNuevo");

                    ldtRU.Rows.InsertAt(ldr, 0);
                }

                poReportesUsuario.DataSource = ldtRU;
                poReportesUsuario.Fill();
                poReportesUsuario.DataValue = poRepUsrDataClient.Id;

                //poRepUsrDataClient.Name = txtNombre.TextBox.Text;
                //lblTitulo.Text = txtNombre.TextBox.Text;
                txtNombre.TextBox.Text = "";
            }
        }

        void btnBaja_Click(object sender, EventArgs e)
        {
            if (poRepUsrDataClient.Id == -1)
            {
                Alert(GetLangItem("RUMsgNoRepSelBaja"));
                return;
            }

            BajaOpcMenu();

            CargasCOM loCom = new CargasCOM();
            loCom.EliminarRegistro("Historicos", poRepUsrDataClient.Id, DSODataContext.GetContext());


            //Llena el combo de reportes usuario
            DataTable ldtRU;
            DataRow ldr;

            ldtRU = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario", new string[] { "iCodRegistro", "vchDescripcion" });

            if (ldtRU != null)
            {
                ldtRU.Columns["iCodRegistro"].ColumnName = "id";
                ldtRU.Columns["vchDescripcion"].ColumnName = "value";

                ldr = ldtRU.NewRow();
                ldr["id"] = -1;
                ldr["value"] = GetLangItem("RUNuevo");

                ldtRU.Rows.InsertAt(ldr, 0);
            }

            poReportesUsuario.Source = "../../WebMethods.aspx/GetReportesUsuario?rb=" + poBaseReport.TextValue.Text;
            poReportesUsuario.DataSource = ldtRU;
            poReportesUsuario.Fill();
            ((TextBox)poReportesUsuario.Control).Text = "";
            poReportesUsuario.DataValue = System.DBNull.Value;

            plstTabs.Clear();
            poRepUsrData.FieldsList.Clear();
            poRepUsrData.CategoriesList.Clear();
            poRepUsrData.CriteriasList.Clear();
            poRepUsrData.CriteriasGrpList.Clear();
            poRepUsrData.Name = "";
            poRepUsrData.LastCriteriaRow = "";
            poRepUsrData.LastCriteriaGrpRow = "";
            poRepUsrData.TopN = 0;
            poRepUsrData.TopDir = 0;
            poRepUsrData.Generated = 0;

            InitFields();

            poRepUsrDataClient = null;

            if (Reporte != null)
                Reporte.Controls.Clear();

            Reporte = null;
            lbRepGen = false;

            lblTitulo.Text = (string)Util.IsDBNull(KDBUtil.SearchScalar("OpcMnu", (string)Request.Params["Opc"], "{" + Globals.GetLanguage() + "}", true), "Reporte Usuario"); ;
            txtDescripcion.DataValue = "";
            txtNombre.TextBox.Text = "";
        }

        void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            poRepUsrDataClient = null;

            if (Reporte != null)
                Reporte.Controls.Clear();

            Reporte = null;
            lbRepGen = false;
        }

        #endregion


        #region Métodos

        protected bool RepUsuValidate()
        {
            bool lbRet = true;
            DataTable ldtVal;

            if ((string)poBaseReport.DataValue == "null")
            {
                Alert(GetLangItem("RUMsgNoRepBSel"));
                return false;
            }

            if ((string)poReportesUsuario.DataValue == "null")
            {
                Alert(GetLangItem("RUMsgNoRepSel"));
                return false;
            }

            if ((string)poReportesUsuario.DataValue == "-1" && txtNombre.TextBox.Text == "")
            {
                Alert(GetLangItem("RUMsgNoRepNom"));
                return false;
            }

            if (txtNombre.TextBox.Text == "" && poRepUsrDataClient.CreatorUser != (int)Session["iCodUsuario"])
            {
                Alert(GetLangItem("RUMsgNoSvOtPer"));
                return false;
            }

            if (int.Parse((string)rblAccessType.DataValue) == KDBUtil.SearchICodCatalogo("RepUsuAcceso", "publico", true) &&
                !DSONavegador.getPermiso((string)Request.Params["Opc"], Permiso.Agregar))
            {
                Alert(GetLangItem("RUMsgNoAgrPub"));
                return false;
            }

            if (txtNombre.TextBox.Text != "")
            {
                ldtVal = null;
                ldtVal = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario", "vchDescripcion = '" + txtNombre.TextBox.Text + "'");

                if (ldtVal != null && ldtVal.Rows.Count > 0)
                {
                    Alert(GetLangItem("RUMsgRepEx", txtNombre.TextBox.Text));
                    return false;
                }
            }

            //Alertas
            if (poRepUsrDataClient.MailFreq == 1 && poRepUsrDataClient.MailFechaUnaVez == "")
            {
                Alert(GetLangItem("RUMsgDatEnvInc", txtNombre.TextBox.Text));
                return false;
            }

            if (poRepUsrDataClient.MailFreq == 5 && (poRepUsrDataClient.MailDiaMes < 1 || poRepUsrDataClient.MailDiaMes > 31))
            {
                Alert(GetLangItem("RUMsgDiaEnvInc", txtNombre.TextBox.Text));
                return false;
            }

            if (poRepUsrDataClient.MailFreq >= 1 && poRepUsrDataClient.MailTo.Trim() == "" &&
                poRepUsrDataClient.MailCC.Trim() == "" && poRepUsrDataClient.MailBCC.Trim() == "")
            {
                Alert(GetLangItem("RUMsgNoMailDir", txtNombre.TextBox.Text));
                return false;
            }

            string lsPattern = "^[a-zA-Z]{1}[a-zA-Z0-9_-]{2,}([.][a-zA-Z0-9_-]+)*[@][a-zA-Z0-9_-]{3,}([.][a-zA-Z0-9_-]+)*[.][a-zA-Z]{2,4}$";

            if (poRepUsrDataClient.MailTo.Trim() != "")
                foreach (string lsAddr in poRepUsrDataClient.MailTo.Replace(',', ';').Split(';'))
                    if (!Regex.IsMatch(lsAddr.Trim(), lsPattern))
                    {
                        Alert(GetLangItem("RUMsgDirMailInc"));
                        return false;
                    }

            if (poRepUsrDataClient.MailCC.Trim() != "")
                foreach (string lsAddr in poRepUsrDataClient.MailCC.Replace(',', ';').Split(';'))
                    if (!Regex.IsMatch(lsAddr.Trim(), lsPattern))
                    {
                        Alert(GetLangItem("RUMsgDirMailInc"));
                        return false;
                    }

            if (poRepUsrDataClient.MailBCC.Trim() != "")
                foreach (string lsAddr in poRepUsrDataClient.MailBCC.Replace(',', ';').Split(';'))
                    if (!Regex.IsMatch(lsAddr.Trim(), lsPattern))
                    {
                        Alert(GetLangItem("RUMsgDirMailInc"));
                        return false;
                    }

            return lbRet;
        }

        protected bool RepUsuExecValidate()
        {
            if (poRepUsrDataClient == null)
                return true;

            bool lbFlds = false;
            StringBuilder lsbCrits = new StringBuilder();
            string lsCrits;

            MatchCollection loMatches;
            MatchCollection loMatches2;

            string lsTipoRep = (string)Util.IsDBNull(KDBUtil.SearchScalar("RepUsuTpRep", poRepUsrDataClient.ReportType, "vchCodigo", true), "");

            //Reporte base seleccionado
            if (poRepUsrDataClient.BaseReport == -1)
            {
                Alert(GetLangItem("RUMsgNoRepBSel"));
                return false;
            }

            //Algun campo seleccionado
            foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
            {
                if (loFld.IsSelected)
                {
                    lbFlds = true;
                    break;
                }
            }

            if (!lbFlds)
            {
                Alert(GetLangItem("RUMsgNoFldSel"));
                return false;
            }

            //Criterios
            foreach (ReporteUsuarioCriteria loCrit in poRepUsrDataClient.Criterias)
            {
                if (loCrit.Value.Trim() == "" &&
                    (loCrit.Field.Type == "Date" || loCrit.Field.Type == "DateTime" ||
                    loCrit.Field.Type == "Int" || loCrit.Field.Type == "IntFormat" ||
                    loCrit.Field.Type == "Float" || loCrit.Field.Type == "Currency"))
                {
                    Alert(GetLangItem("RUMsgCritInc", loCrit.Field.Name));
                    return false;
                }

                lsbCrits.Append("{" + loCrit.Row + "}");
            }

            //Condiciones
            lsCrits = lsbCrits.ToString();
            loMatches = Regex.Matches(poRepUsrDataClient.Conditions, @"\{.\}");

            foreach (Match loMatch in loMatches)
                if (lsCrits.IndexOf(loMatch.Value) < 0)
                {
                    Alert(GetLangItem("RUMsgCritInex", loMatch.Value));
                    return false;
                }

            //Numero de paréntesis
            loMatches = Regex.Matches(poRepUsrDataClient.Conditions, @"\(");
            loMatches2 = Regex.Matches(poRepUsrDataClient.Conditions, @"\)");

            if (loMatches.Count != loMatches2.Count)
            {
                Alert(GetLangItem("RUMsgParNoCoin"));
                return false;
            }


            //Criterios Agrupados
            lsbCrits = new StringBuilder();

            foreach (ReporteUsuarioCriteria loCrit in poRepUsrDataClient.CriteriasGrp)
            {
                if (loCrit.Value.Trim() == "" &&
                    (loCrit.Field.Type == "Date" || loCrit.Field.Type == "DateTime" ||
                    loCrit.Field.Type == "Int" || loCrit.Field.Type == "IntFormat" ||
                    loCrit.Field.Type == "Float" || loCrit.Field.Type == "Currency"))
                {
                    Alert(GetLangItem("RUMsgCritInc", loCrit.Field.Name));
                    return false;
                }

                lsbCrits.Append("{" + loCrit.Row + "}");
            }

            //Condiciones
            lsCrits = lsbCrits.ToString();
            loMatches = Regex.Matches(poRepUsrDataClient.ConditionsGrp, @"\{.\}");

            foreach (Match loMatch in loMatches)
                if (lsCrits.IndexOf(loMatch.Value) < 0)
                {
                    Alert(GetLangItem("RUMsgCritInex", loMatch.Value));
                    return false;
                }

            //Numero de paréntesis
            loMatches = Regex.Matches(poRepUsrDataClient.ConditionsGrp, @"\(");
            loMatches2 = Regex.Matches(poRepUsrDataClient.ConditionsGrp, @"\)");

            if (loMatches.Count != loMatches2.Count)
            {
                Alert(GetLangItem("RUMsgParNoCoin"));
                return false;
            }

            //gráfica
            if (poRepUsrDataClient.ShowGraph != 0)
            {
                if (poRepUsrDataClient.GraphX == -1)
                {
                    Alert(GetLangItem("RUMsgNoGrSelX"));
                    return false;
                }

                if (poRepUsrDataClient.GraphY == -1)
                {
                    Alert(GetLangItem("RUMsgNoGrSelY"));
                    return false;
                }
            }

            //Matricial
            if (lsTipoRep == "RepUsuTpRes")
            {
                bool lbFldSum = false;

                foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                {
                    if (loFld.AggregateFn != "")
                        lbFldSum = true;
                }

                //Datos sumarizados
                if (!lbFldSum)
                {
                    Alert(GetLangItem("RUMsgNoFldSumSel"));
                    return false;
                }
            }

            //Matricial
            if (lsTipoRep == "RepUsuTpMat")
            {
                bool lbFldX = false;
                bool lbFldY = false;
                bool lbFldXY = false;

                foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                {
                    if (loFld.GroupMatX != -1)
                        lbFldX = true;

                    if (loFld.GroupMatY != -1)
                        lbFldY = true;

                    if (loFld.AggregateFn != "")
                        lbFldXY = true;
                }

                //Encabezado de columnas
                if (!lbFldX)
                {
                    Alert(GetLangItem("RUMsgNoFldXSel"));
                    return false;
                }

                //Encabezado de directorios
                if (!lbFldY)
                {
                    Alert(GetLangItem("RUMsgNoFldYSel"));
                    return false;
                }

                //Datos sumarizados
                if (!lbFldXY)
                {
                    Alert(GetLangItem("RUMsgNoFldXYSel"));
                    return false;
                }
            }

            return true;
        }

        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            pbExport = true;
            poExport = lkeFormat;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            btnGrabar = AddClientButton(Buttons, "btnGrabar", GetLangItem("RUGrabar"), "buttonSave", "oRepUsr.save();");
            poRepUsrData.SaveButtonId = btnGrabar.ClientID;

            btnBaja = AddClientButton(Buttons, "btnBaja", GetLangItem("RUBaja"), "buttonDelete", "oRepUsr.del();");
            poRepUsrData.DeleteButtonId = btnBaja.ClientID;

            btnGenerar = AddClientButton(Buttons, "poBtnGen", GetLangItem("RUGenerar"), "buttonPlay", "oRepUsr.generate();");
            poRepUsrData.GenerateButtonId = btnGenerar.ClientID;

            DSONumberEdit lne = new DSONumberEdit();
            Content.Controls.Add(lne);
            lne.CreateControls();
            lne.NumberBox.Visible = false;

            DSODateTimeBox lde = new DSODateTimeBox();
            Content.Controls.Add(lde);
            lde.CreateControls();
            lde.DateTimeBox.Visible = false;

            poTabs = new DSOTabs();
            Content.Controls.Add(poTabs);
            poTabs.IsSortable = false;
            poTabs.CreateControls();

            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");
            poTabs.AddTab("");

            CreateTabStart();
            CreateTabReport();
            CreateTabType();
            CreateTabFields();
            CreateTabSummary();
            CreateTabFieldOrder();
            CreateTabCriteria();
            CreateTabDataOrder();
            CreateTabMail();
            CreateTabGroup();
            CreateTabGroupMat();
            CreateTabGraph();
            CreateTabOutput();

            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioJs", "<script language=\"javascript\" src=\"ReporteUsuario.js?v=2\"></script>\r\n", true, false);
            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuario", "<script language=\"javascript\">var oRepUsr; $(document).ready(function() {oRepUsr = new ReporteUsuario(\"" + poTabs.ClientID + "\", 'oRepUsr', function() {" + Page.ClientScript.GetPostBackEventReference(cboReportType, "") + "})})</script>\r\n", true, false);

            ChildControlsCreated = true;
        }

        protected override void InitLanguage()
        {
            base.InitLanguage();

            DataTable ldtAcc;
            HtmlButton poBtn;

            string lsLangCol = "{" + Globals.GetLanguage() + "}";


            //Pestañas
            poTabs[0].Title = GetLangItem("RUPestInicio");
            poTabs[1].Title = "1. " + GetLangItem("RUPestSelReporte");
            poTabs[2].Title = "2. " + GetLangItem("RUPestTipoRep");

            //poBaseReport.Label.Text = GetLangItem("RURepBase");
            poBaseReport.Visible = false;

            poReportesUsuario.Label.Text = GetLangItem("RURepUsu");
            txtNombre.Label.Text = GetLangItem("RUGuardarComo");
            txtDescripcion.Label.Text = GetLangItem("RUDescr");
            cboReportType.Label.Text = GetLangItem("RUTipoRep");
            rblAccessType.Label.Text = GetLangItem("RUTipoAcceso");

            btnNuevo.Text = GetLangItem("RUNuevoRep");
            btnAbrir.Text = GetLangItem("RUAbrir");

            for (int i = 0; i < poTabs.Count; i++)
            {
                if ((poBtn = (HtmlButton)poTabs[i].Panel.FindControl("poBtnAnt" + i)) != null)
                    poBtn.InnerText = GetLangItem("RUAnterior");

                if ((poBtn = (HtmlButton)poTabs[i].Panel.FindControl("poBtnSig" + i)) != null)
                    poBtn.InnerText = GetLangItem("RUSiguiente");
            }


            //Tipo de reporte
            foreach (ListItem liItm in cboReportType.DropDownList.Items)
                liItm.Text = (string)KDBUtil.SearchScalar("RepUsuTpRep", int.Parse(liItm.Value), lsLangCol);


            //Tipo de acceso
            ldtAcc = kdb.GetHisRegByEnt("RepUsuAcceso", "Idioma", new string[] { "iCodCatalogo", "{" + Globals.GetLanguage() + "}" });

            if (ldtAcc != null)
            {
                ldtAcc.Columns["iCodCatalogo"].ColumnName = "id";
                ldtAcc.Columns["{" + Globals.GetLanguage() + "}"].ColumnName = "value";
            }

            rblAccessType.DataSource = ldtAcc;
            rblAccessType.Fill();



            //Tipo de orden
            if (plstOrderTypes.Count == 0)
            {
                DataTable ldt = kdb.GetHisRegByEnt("Valores", "Valores", new string[] { "{Value}", lsLangCol },
                    "{Atrib} = " + KDBUtil.SearchICodCatalogo("Atrib", "RepEstDirDatos", true));

                if (ldt != null)
                    foreach (DataRow ldr in ldt.Rows)
                        plstOrderTypes.Add(new ReporteUsuarioItem((int)ldr["{Value}"], (string)ldr[lsLangCol]));
            }

            //Operadores
            if (plstOperators.Count == 0)
            {
                DataTable ldt = kdb.GetHisRegByEnt("RepUsuOper", "Idioma", new string[] { "iCodCatalogo", lsLangCol });

                if (ldt != null)
                    foreach (DataRow ldr in ldt.Rows)
                        plstOperators.Add(new ReporteUsuarioItem((int)ldr["iCodCatalogo"], (string)ldr[lsLangCol]));
            }

            //Tipo de graficas
            if (plstGraphTypes.Count == 0)
            {
                DataTable ldt = kdb.GetHisRegByEnt("Valores", "Valores", new string[] { "{Value}", lsLangCol },
                    "{Atrib} = " + KDBUtil.SearchICodCatalogo("Atrib", "RepTipoGrafica", true));

                if (ldt != null)
                    foreach (DataRow ldr in ldt.Rows)
                        plstGraphTypes.Add(new ReporteUsuarioItem((int)ldr["{Value}"], (string)ldr[lsLangCol]));
            }


            //Reporte
            lblTitulo.Text = (poRepUsrData.Name.Trim() != "" ? poRepUsrData.Name : GetLangItem("RUTitRepUsu"));

            if (lbRepGen)
            {
                Reporte.iCodReporte = piCodRepStd;
                Reporte.Title = lblTitulo.Text;
                Reporte.bConfiguraGrid = true;
                Reporte.InitLanguage();
            }

            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgConfSave", GetLangItem("RUMsgConfSave")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgConfDel", GetLangItem("RUMsgConfDel")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgRepDisp", GetLangItem("RUMsgRepDisp")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgDesc", GetLangItem("RUMsgDesc")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgFldOrd", GetLangItem("RUMsgFldOrd")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgNumRecs", GetLangItem("RUMsgNumRecs")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgTotales", GetLangItem("RUMsgTotales")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgAggFn", GetLangItem("RUMsgAggFn")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgDatOrd", GetLangItem("RUMsgDatOrd")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgThen", GetLangItem("RUMsgThen")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgGrpBy", GetLangItem("RUMsgGrpBy")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgSubTot", GetLangItem("RUMsgSubTot")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgRowHdr", GetLangItem("RUMsgRowHdr")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgColHdr", GetLangItem("RUMsgColHdr")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgGraphDsg", GetLangItem("RUMsgGraphDsg")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgX", GetLangItem("RUMsgX")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgVals", GetLangItem("RUMsgVals")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgShw", GetLangItem("RUMsgShw")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgDiario", GetLangItem("RUMsgDiario")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgUnaVez", GetLangItem("RUMsgUnaVez")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgDiaHab", GetLangItem("RUMsgDiaHab")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgSem", GetLangItem("RUMsgSem")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgQuin", GetLangItem("RUMsgQuin")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgMens", GetLangItem("RUMsgMens")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgSemana", GetLangItem("RUMsgSemana")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgDest", GetLangItem("RUMsgDest")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgTo", GetLangItem("RUMsgTo")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgCC", GetLangItem("RUMsgCC")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgBCC", GetLangItem("RUMsgBCC")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgSubject", GetLangItem("RUMsgSubject")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgBody", GetLangItem("RUMsgBody")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgDia", GetLangItem("RUMsgDia")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgNoEnv", GetLangItem("RUMsgNoEnv")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgSelTodo", GetLangItem("RUMsgSelTodo")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgIncGrf", GetLangItem("RUMsgIncGrf")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgFreq", GetLangItem("RUMsgFreq")));
            poLang.ItemsList.Add(new ReporteUsuarioItem("RUMsgRegs", GetLangItem("RUMsgRegs")));


            StringBuilder lsbLang = new StringBuilder();
            foreach (ReporteUsuarioItem loLangItem in poLang.ItemsList)
                lsbLang.Append((lsbLang.Length > 0 ? ", " : "") +
                    "\"" + loLangItem.Code + "\": \"" + loLangItem.Name + "\"");


            txtPrevData.Text = DSOControl.SerializeJSON<PrevData>(poPrevData, Encoding.UTF8);
            txtReportData.Text = DSOControl.SerializeJSON<ReporteUsuarioData>(poRepUsrData, Encoding.UTF8);
            txtLangData.Text = "{" + lsbLang.ToString() + ", " +
                "\"tabs\": " + DSOControl.SerializeJSON<ReporteUsuarioTab[]>(plstTabs.ToArray(), Encoding.UTF8) + ", " +
                "\"orderTypes\": " + DSOControl.SerializeJSON<ReporteUsuarioItem[]>(plstOrderTypes.ToArray(), Encoding.UTF8) + ", " +
                "\"operators\": " + DSOControl.SerializeJSON<ReporteUsuarioItem[]>(plstOperators.ToArray(), Encoding.UTF8) + ", " +
                "\"graphTypes\": " + DSOControl.SerializeJSON<ReporteUsuarioItem[]>(plstGraphTypes.ToArray(), Encoding.UTF8) + "}";


            ViewState["BaseReport"] = poBaseReport.TextValue.Text;
            ViewState["ReportType"] = cboReportType.DropDownList.SelectedValue;
            ViewState["ReportFields"] = DSOControl.SerializeJSON<ReporteUsuarioData>(poRepUsrData, Encoding.UTF8);

            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioShow",
                "<script language=\"javascript\">$(document).ready(function() { " +
                "$(\"#" + txtNombre.ClientID + "\").change(function() { oRepUsr.setModified(true); });" +
                "$(\"#" + txtDescripcion.ClientID + "\").change(function() { oRepUsr.setModified(true); });" +
                "$(\"#" + cboReportType.ClientID + "\").change(function() { oRepUsr.setModified(true); });" +
                "$(\"#" + Content.ClientID + "\").attr(\"style\", \"block\");" +
                "}); </script>\r\n",
                true, false);
        }

        protected void CreateTabStart()
        {
            btnAbrir = new Button();
            btnAbrir.Text = "Abrir";
            btnAbrir.Click += new EventHandler(btnAbrir_Click);
            btnAbrir.CssClass = "buttonEdit";
            btnAbrir.Style.Add("display", "none");
            poTabs[0].Panel.Controls.Add(btnAbrir);

            btnNuevo = new Button();
            btnNuevo.Text = "Nuevo";
            btnNuevo.Click += new EventHandler(btnNuevo_Click);
            btnNuevo.CssClass = "buttonEdit";
            btnNuevo.Style.Add("display", "none");
            poTabs[0].Panel.Controls.Add(btnNuevo);

            tabStart = new Table();
            poTabs[0].Panel.Controls.Add(tabStart);
            tabStart.ID = "tabStart";
            //tabStart.Width = Unit.Percentage(100);

            poRepUsrData.StartTableId = tabStart.ClientID;
        }

        protected void CreateTabReport()
        {
            AddClientButton(poTabs[1].Panel, "poBtnAnt1", GetLangItem("RUAnterior"), "button", "oRepUsr.prevStep();");
            AddClientButton(poTabs[1].Panel, "poBtnSig1", GetLangItem("RUSiguiente"), "button", "oRepUsr.nextStep();");

            tabReport = new Table();
            poTabs[1].Panel.Controls.Add(tabReport);
            tabReport.Width = Unit.Percentage(100);


            poBaseReport = new DSOAutocomplete();
            poBaseReport.ID = "ReporteBase";
            poBaseReport.Table = tabReport;
            poBaseReport.Row = 1;
            poBaseReport.Delay = 0;
            poBaseReport.MinLength = 0;
            poBaseReport.Source = "../../WebMethods.aspx/GetReportesBase";
            poBaseReport.IsDropDown = false;
            poBaseReport.AutoPostBack = false;
            poBaseReport.ColumnSpan = 3;
            poBaseReport.Visible = false;
            poBaseReport.CreateControls();

            InitBaseReport();

            poBaseReport.TextValue.Text = "83211"; //Consumos
            ViewState["BaseReport"] = poBaseReport.TextValue.Text;

            poReportesUsuario = new DSOAutocomplete();
            poReportesUsuario.ID = "ReporteUsuario";
            poReportesUsuario.Table = tabReport;
            poReportesUsuario.Row = 2;
            poReportesUsuario.Delay = 0;
            poReportesUsuario.MinLength = 0;
            poReportesUsuario.Source = "../../WebMethods.aspx/GetReportesUsuario?rb=-1";
            poReportesUsuario.IsDropDown = false;
            poReportesUsuario.AutoPostBack = true;
            poReportesUsuario.CreateControls();

            InitUserReport();

            txtNombre = new DSOTextBox();
            txtNombre.ID = "Nombre";
            txtNombre.Table = tabReport;
            txtNombre.Row = 2;
            txtNombre.CreateControls();


            txtDescripcion = new DSOTextBox();
            txtDescripcion.ID = "txtDescripcion";
            txtDescripcion.Table = tabReport;
            txtDescripcion.Row = 3;
            txtDescripcion.ColumnSpan = 3;
            txtDescripcion.CreateControls();
            txtDescripcion.TextBox.TextMode = TextBoxMode.MultiLine;
            txtDescripcion.TextBox.MaxLength = 1000;
            txtDescripcion.TextBox.Rows = 10;
            txtDescripcion.TextBox.Style["height"] = "150px";


            rblAccessType = new DSORadioButtonList();
            rblAccessType.ID = "Acceso";
            rblAccessType.Table = tabReport;
            rblAccessType.Row = 4;
            rblAccessType.CreateControls();
            rblAccessType.DataValueDelimiter = "";
            rblAccessType.DataValue = KDBUtil.SearchICodCatalogo("RepUsuAcceso", "privado", true);
            rblAccessType.RadioButtonList.SelectedIndexChanged += new EventHandler(rblAccessType_SelectedIndexChanged);

            poRepUsrData.AccessTypeRblId = rblAccessType.ClientID;
        }

        protected void rblAccessType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (poRepUsrDataClient != null)
            {
                poRepUsrDataClient.AccessType = int.Parse(rblAccessType.RadioButtonList.SelectedValue);

                DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioTpAcc",
                    "<script language=\"javascript\">function() { oRepUsr.setModified(true); };</script>\r\n",
                    false, false);
            }
        }

        protected void CreateTabType()
        {
            AddClientButton(poTabs[2].Panel, "poBtnAnt2", GetLangItem("RUAnterior"), "button", "oRepUsr.prevStep();");
            AddClientButton(poTabs[2].Panel, "poBtnSig2", GetLangItem("RUSiguiente"), "button", "oRepUsr.nextStep();");

            Table t = new Table();
            poTabs[2].Panel.Controls.Add(t);
            t.Width = Unit.Percentage(100);

            cboReportType = new DSODropDownList();
            cboReportType.ID = "cboTipoReporte";
            cboReportType.Table = t;
            cboReportType.Row = 1;
            cboReportType.AutoPostBack = false;
            cboReportType.CreateControls();
            cboReportType.DataValueDelimiter = "";
            cboReportType.DropDownList.SelectedIndexChanged += new EventHandler(DropDownList_SelectedIndexChanged);

            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioTipoReporte",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { $(\"#" + cboReportType.DropDownList.ClientID + "\").change(function () { oRepUsr.conf.reportType = $(\"#" + cboReportType.DropDownList.ClientID + "\").val(); oRepUsr.doPostBack(); }); });" +
                "</script>\r\n",
                true, false);

            if (!IsPostBack)
            {
                DataTable ldt = kdb.GetHisRegByEnt("RepUsuTpRep", "Idioma", new string[] { "iCodCatalogo", "vchDescripcion" });

                if (ldt != null)
                {
                    ldt.Columns["iCodCatalogo"].ColumnName = "id";
                    ldt.Columns["vchDescripcion"].ColumnName = "value";

                    cboReportType.DataSource = ldt;
                    cboReportType.Fill();
                }
            }
        }

        protected void CreateTabFields()
        {
            pnlFields = new Panel();
            Content.Controls.Add(pnlFields);
            pnlFields.Visible = false;

            tabFields = new Table();
            pnlFields.Controls.Add(tabFields);
            tabFields.Width = Unit.Percentage(100);
        }

        protected void CreateTabSummary()
        {
            pnlSummary = new Panel();
            Content.Controls.Add(pnlSummary);
            pnlSummary.Visible = false;

            tabSummary = new Table();
            tabSummary.ID = "tabSummary";
            pnlSummary.Controls.Add(tabSummary);
            //tabSummary.Width = Unit.Percentage(100);

            poRepUsrData.SummaryTableId = tabSummary.ClientID;
        }

        protected void CreateTabFieldOrder()
        {
            pnlFieldOrder = new Panel();
            Content.Controls.Add(pnlFieldOrder);
            pnlFieldOrder.Visible = false;

            Table t = new Table();
            t.ID = "tabFieldOrder";
            pnlFieldOrder.Controls.Add(t);
            t.Width = Unit.Percentage(100);

            poRepUsrData.FieldOrderTableId = t.ClientID;
        }

        protected void CreateTabCriteria()
        {
            pnlCriteria = new Panel();
            pnlCriteria.ID = "pnlCriteria";
            Content.Controls.Add(pnlCriteria);
            pnlCriteria.Visible = false;

            Table lt;
            TableRow ltr;
            TableCell ltd;


            lt = new Table();
            lt.Width = Unit.Percentage(100);
            lt.CellPadding = 0;
            lt.CellSpacing = 0;
            pnlCriteria.Controls.Add(lt);


            ltr = new TableRow();
            lt.Rows.Add(ltr);


            ltd = new TableCell();
            ltd.Width = Unit.Percentage(50);
            ltd.ID = "ButtonsLeft";
            ltr.Cells.Add(ltd);

            ltd = new TableCell();
            ltd.Width = Unit.Percentage(50);
            ltd.ID = "ButtonsRight";
            ltd.VerticalAlign = VerticalAlign.Top;
            ltr.Cells.Add(ltd);


            ltr = new TableRow();
            lt.Rows.Add(ltr);


            ltd = new TableCell();
            ltd.VerticalAlign = VerticalAlign.Top;
            ltr.Cells.Add(ltd);

            tabCriteria = new Table();
            tabCriteria.ID = "tabCriteria";
            ltd.Controls.Add(tabCriteria);
            //tabCriteria.Width = Unit.Percentage(100);


            ltd = new TableCell();
            ltd.VerticalAlign = VerticalAlign.Top;
            ltr.Cells.Add(ltd);

            tabCriteriaGrp = new Table();
            tabCriteriaGrp.ID = "tabCriteriaGrp";
            ltd.Controls.Add(tabCriteriaGrp);


            poRepUsrData.CriteriaTableId = tabCriteria.ClientID;
            //poRepUsrData.CriteriaGrpTableId = tabCriteriaGrp.ClientID;
        }

        protected void CreateTabDataOrder()
        {
            pnlDataOrder = new Panel();
            Content.Controls.Add(pnlDataOrder);
            pnlDataOrder.Visible = false;

            tabDataOrder = new Table();
            tabDataOrder.ID = "tabDataOrder";
            pnlDataOrder.Controls.Add(tabDataOrder);
            //tabSummary.Width = Unit.Percentage(100);

            poRepUsrData.DataOrderTableId = tabDataOrder.ClientID;
        }

        protected void CreateTabMail()
        {
            pnlMail = new Panel();
            Content.Controls.Add(pnlMail);
            pnlMail.Visible = false;

            tabMail = new Table();
            tabMail.ID = "tabMail";
            pnlMail.Controls.Add(tabMail);

            poRepUsrData.MailTableId = tabMail.ClientID;
        }

        protected void CreateTabGroup()
        {
            pnlGroup = new Panel();
            Content.Controls.Add(pnlGroup);
            pnlGroup.Visible = false;

            tabGroup = new Table();
            tabGroup.ID = "tabGroup";
            pnlGroup.Controls.Add(tabGroup);
        }

        protected void CreateTabGroupMat()
        {
            pnlGroupMat = new Panel();
            Content.Controls.Add(pnlGroupMat);
            pnlGroupMat.Visible = false;

            tabGroupMat = new Table();
            tabGroupMat.ID = "tabGroupMat";
            pnlGroupMat.Controls.Add(tabGroupMat);
        }

        protected void CreateTabGraph()
        {
            pnlGraph = new Panel();
            Content.Controls.Add(pnlGraph);
            pnlGraph.Visible = false;

            tabGraph = new Table();
            tabGraph.ID = "tabGraph";
            pnlGraph.Controls.Add(tabGraph);
        }

        protected void CreateTabOutput()
        {
            pnlOutput = new Panel();
            Content.Controls.Add(pnlOutput);
            pnlOutput.Visible = false;

            tabOutput = new Table();
            tabOutput.ID = "tabOutput";
            tabOutput.Width = Unit.Percentage(100);
            pnlOutput.Controls.Add(tabOutput);

            TableRow tr = new TableRow();
            tabOutput.Rows.Add(tr);

            tdOutput = new TableCell();
            tr.Cells.Add(tdOutput);
        }

        protected void InitFields()
        {
            DSOCheckBoxList lcblCat = null;

            int liCat = -1;
            int liRow = 1;

            pnlFields.Controls.Remove(tabFields);

            tabFields = new Table();
            pnlFields.Controls.Add(tabFields);
            tabFields.Width = Unit.Percentage(100);

            poRepUsrData.FieldTableId = tabFields.ClientID;

            foreach (ReporteUsuarioField loFld in poRepUsrData.FieldsList)
            {
                if (liCat != loFld.CategoryId)
                {
                    liCat = loFld.CategoryId;

                    lcblCat = new DSOCheckBoxList();
                    lcblCat.ID = "cblCat" + liCat;
                    lcblCat.Table = tabFields;
                    lcblCat.Row = liRow++;
                    lcblCat.CreateControls();
                    lcblCat.Label.Text = (string)KDBUtil.SearchScalar("RepUsuCatCampo", liCat, "{" + Globals.GetLanguage() + "}"); //(string)kdb.GetHisRegByEnt("RepUsuCatCampo", "Idioma", new string[] { "vchDescripcion" }, "iCodCatalogo = " + liCat).Rows[0]["vchDescripcion"];
                    lcblCat.AddClientEvent("onclick", "javascript:oRepUsr.fieldClicked(event, this);");
                    lcblCat.CheckBoxList.SelectedIndexChanged += new EventHandler(CheckBoxList_SelectedIndexChanged);
                    lcblCat.CheckBoxList.EnableViewState = false;

                    loFld.Category.ControId = lcblCat.ClientID;
                }

                ListItem lliCampo = new ListItem(loFld.Name, loFld.Id.ToString());
                lcblCat.CheckBoxList.Items.Add(lliCampo);

                if (loFld.IsSelected)
                    lliCampo.Selected = true;

                loFld.Item = lliCampo;
            }
        }

        public void InitBaseReport()
        {
            DataTable ldtRB = kdb.GetHisRegByEnt("RepUsu", "Reportes Base", new string[] { "iCodCatalogo", "vchDescripcion" });

            if (ldtRB != null)
            {
                ldtRB.Columns["iCodCatalogo"].ColumnName = "id";
                ldtRB.Columns["vchDescripcion"].ColumnName = "value";
            }

            poBaseReport.DataSource = ldtRB;
            poBaseReport.Fill();
        }

        public void InitUserReport()
        {
            DataTable ldtRU = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario", new string[] { "iCodRegistro", "vchDescripcion" });
            DataRow ldr;

            if (ldtRU != null)
            {
                ldtRU.Columns["iCodRegistro"].ColumnName = "id";
                ldtRU.Columns["vchDescripcion"].ColumnName = "value";

                ldr = ldtRU.NewRow();
                ldr["id"] = -1;
                ldr["value"] = GetLangItem("RUNuevo");

                ldtRU.Rows.InsertAt(ldr, 0);
            }

            poReportesUsuario.DataSource = ldtRU;
            poReportesUsuario.Fill();
        }

        protected void SetTabs()
        {
            Trace.Write("ShowTabs");

            plstTabs.Clear();

            //Primero deja como no visibles las tabs "variables"
            //poTabs[1].Visible = false;
            poTabs[3].Visible = false;
            poTabs[4].Visible = false;
            poTabs[5].Visible = false;
            poTabs[6].Visible = false;
            poTabs[7].Visible = false;
            poTabs[8].Visible = false;
            poTabs[9].Visible = false;
            poTabs[10].Visible = false;
            poTabs[11].Visible = false;

            //Obtiene el Historico del tipo de reporte, según la selección del usuario
            DataRow ldr = KDBUtil.SearchHistoricRow("RepUsuTpRep", int.Parse((string)cboReportType.DataValue), new string[] { });

            //Dependiendo del tipo de reportes seleccionado, configurará los tabs se muestran en pantalla.
            if (ldr != null && (string)ldr["vchCodigo"] == "RepUsuTpTab")
            {
                //Opción Reporte Tabular
                SetTab(3, "3. " + GetLangItem("RUPestCampos"), pnlFields);
                //SetTab(4, "4. " + GetLangItem("RUPestResumen"), pnlSummary);
                SetTab(4, "4. " + GetLangItem("RUPestOrdCampos"), pnlFieldOrder);
                SetTab(5, "5. " + GetLangItem("RUPestCriterios"), pnlCriteria,
                    new HtmlButton[] {
                        this.CreateClientButton("btnAddCriteria", "+", "button", "oRepUsr.addCriteria();") });
                //SetTab(5, "5. " + GetLangItem("RUPestCriterios"), pnlCriteria,
                //    new HtmlButton[] {
                //        this.CreateClientButton("btnAddCriteria", "+", "button", "oRepUsr.addCriteria();"),
                //        this.CreateClientButton("btnToggleConds", GetLangItem("RUMsgCondCompl"), "button", "if ($(\"#txtConditions\").css(\"display\") == \"none\") $(\"#txtConditions\").show(); else $(\"#txtConditions\").val(\"\").hide();") });
                SetTab(6, "6. " + GetLangItem("RUPestOrdDatos"), pnlDataOrder);
                //SetTab(8, "8. " + GetLangItem("RUPestCorreo"), pnlMail);
                SetTab(7, "7. " + GetLangItem("RUPestReporte"), pnlOutput, false);

                poRepUsrData.GroupTableId = "";
                poRepUsrData.GroupMatTableId = "";
                poRepUsrData.GraphTableId = "";
                poRepUsrData.CriteriaGrpTableId = "";
            }
            else if (ldr != null && (string)ldr["vchCodigo"] == "RepUsuTpRes")
            {
                //Opción Reporte de Resumen
                SetTab(3, "3. " + GetLangItem("RUPestResumen"), pnlSummary);
                SetTab(4, "4. " + GetLangItem("RUPestAgrupamiento"), pnlGroup);
                SetTab(5, "5. " + GetLangItem("RUPestCampos"), pnlFields);
                SetTab(6, "6. " + GetLangItem("RUPestOrdCampos"), pnlFieldOrder);
                SetTab(7, "7. " + GetLangItem("RUPestCriterios"), pnlCriteria,
                    new HtmlButton[] { this.CreateClientButton("btnAddCriteria", "+ " + GetLangItem("RUMsgTitCrit"), "button", "oRepUsr.addCriteria();") });
                //SetTab(7, "7. " + GetLangItem("RUPestCriterios"), pnlCriteria,
                //    new HtmlButton[] { this.CreateClientButton("btnAddCriteria", "+ " + GetLangItem("RUMsgTitCrit"), "button", "oRepUsr.addCriteria();"),
                //        this.CreateClientButton("btnToggleConds", GetLangItem("RUMsgCondCompl"), "button", " if ($(\"#txtConditions\").css(\"display\") == \"none\") $(\"#txtConditions\").show(); else $(\"#txtConditions\").val(\"\").hide();") },
                //    new HtmlButton[] { this.CreateClientButton("btnAddCriteriaGrp", "+ " + GetLangItem("RUMsgTitCritGrp"), "button", "oRepUsr.addCriteriaGrp();"),
                //        this.CreateClientButton("btnToggleCondsGrp", GetLangItem("RUMsgCondCompl"), "button", " if ($(\"#txtConditionsGrp\").css(\"display\") == \"none\") $(\"#txtConditionsGrp\").show(); else $(\"#txtConditionsGrp\").val(\"\").hide();") });
                SetTab(8, "8. " + GetLangItem("RUPestOrdDatos"), pnlDataOrder);
                SetTab(9, "9. " + GetLangItem("RUPestGrafica"), pnlGraph);
                //SetTab(10, "10. " + GetLangItem("RUPestCorreo"), pnlMail);
                SetTab(10, "10. " + GetLangItem("RUPestReporte"), pnlOutput, false);

                poRepUsrData.GroupTableId = tabGroup.ClientID;
                poRepUsrData.GroupMatTableId = "";
                poRepUsrData.GraphTableId = tabGraph.ClientID;
                poRepUsrData.CriteriaGrpTableId = tabCriteriaGrp.ClientID;
            }
            else if (ldr != null && (string)ldr["vchCodigo"] == "RepUsuTpMat")
            {
                //Opción Reporte Matricial
                SetTab(3, "3. " + GetLangItem("RUPestAgrupamiento"), pnlGroupMat);
                SetTab(4, "4. " + GetLangItem("RUPestResumen"), pnlSummary);
                SetTab(5, "5. " + GetLangItem("RUPestCampos"), pnlFields);
                SetTab(6, "6. " + GetLangItem("RUPestOrdCampos"), pnlFieldOrder);
                SetTab(7, "7. " + GetLangItem("RUPestCriterios"), pnlCriteria,
                    new HtmlButton[] { this.CreateClientButton("btnAddCriteria", "+ " + GetLangItem("RUMsgTitCrit"), "button", "oRepUsr.addCriteria();") });
                //SetTab(7, "7. " + GetLangItem("RUPestCriterios"), pnlCriteria,
                //    new HtmlButton[] { this.CreateClientButton("btnAddCriteria", "+ " + GetLangItem("RUMsgTitCrit"), "button", "oRepUsr.addCriteria();"),
                //        this.CreateClientButton("btnToggleConds", GetLangItem("RUMsgCondCompl"), "button", " if ($(\"#txtConditions\").css(\"display\") == \"none\") $(\"#txtConditions\").show(); else $(\"#txtConditions\").val(\"\").hide();") },
                //    new HtmlButton[] { this.CreateClientButton("btnAddCriteriaGrp", "+ " + GetLangItem("RUMsgTitCritGrp"), "button", "oRepUsr.addCriteriaGrp();"),
                //        this.CreateClientButton("btnToggleCondsGrp", GetLangItem("RUMsgCondCompl"), "button", " if ($(\"#txtConditionsGrp\").css(\"display\") == \"none\") $(\"#txtConditionsGrp\").show(); else $(\"#txtConditionsGrp\").val(\"\").hide();") });
                SetTab(8, "8. " + GetLangItem("RUPestOrdDatos"), pnlDataOrder);
                //SetTab(9, "9. " + GetLangItem("RUPestGrafica"), pnlGraph);
                //SetTab(10, "10. " + GetLangItem("RUPestCorreo"), pnlMail);
                SetTab(10, "10. " + GetLangItem("RUPestReporte"), pnlOutput, false);

                poRepUsrData.GroupTableId = "";
                poRepUsrData.GroupMatTableId = tabGroupMat.ClientID;
                poRepUsrData.GraphTableId = tabGraph.ClientID;
                poRepUsrData.CriteriaGrpTableId = tabCriteriaGrp.ClientID;
            }
        }

        protected void SetTab(int liIndexTab, string lsTitle, Panel pnlPanel)
        {
            SetTab(liIndexTab, lsTitle, pnlPanel, null, true);
        }

        protected void SetTab(int liIndexTab, string lsTitle, Panel pnlPanel, bool lbBtnNext)
        {
            SetTab(liIndexTab, lsTitle, pnlPanel, null, lbBtnNext);
        }

        protected void SetTab(int liIndexTab, string lsTitle, Panel pnlPanel, HtmlButton[] laButtons)
        {
            SetTab(liIndexTab, lsTitle, pnlPanel, laButtons, true);
        }

        protected void SetTab(int liIndexTab, string lsTitle, Panel pnlPanel, HtmlButton[] laButtons, bool lbBtnNext)
        {
            AddClientButton(poTabs[liIndexTab].Panel, "poBtnAnt" + liIndexTab, GetLangItem("RUAnterior"), "button", "oRepUsr.prevStep();");

            if (lbBtnNext)
                AddClientButton(poTabs[liIndexTab].Panel, "poBtnSig" + liIndexTab, GetLangItem("RUSiguiente"), "button", "oRepUsr.nextStep();");

            if (laButtons != null)
                foreach (HtmlButton loBtn in laButtons)
                    AddClientButton(poTabs[liIndexTab].Panel, loBtn);

            poTabs[liIndexTab].Visible = true;
            pnlPanel.Visible = true;
            plstTabs.Add(new ReporteUsuarioTab(liIndexTab, lsTitle, poTabs[liIndexTab].Panel.ClientID, pnlPanel.ClientID));
        }

        protected void SetTab(int liIndexTab, string lsTitle, Panel pnlPanel, HtmlButton[] laButtonsIzq, HtmlButton[] laButtonsDer)
        {
            Control loControlLeft = pnlPanel.FindControl("ButtonsLeft");
            Control loControlRight = pnlPanel.FindControl("ButtonsRight");

            AddClientButton(loControlLeft, "poBtnAnt" + liIndexTab, GetLangItem("RUAnterior"), "button", "oRepUsr.prevStep();");
            AddClientButton(loControlLeft, "poBtnSig" + liIndexTab, GetLangItem("RUSiguiente"), "button", "oRepUsr.nextStep();");

            if (laButtonsIzq != null)
                foreach (HtmlButton loBtn in laButtonsIzq)
                    AddClientButton(loControlLeft, loBtn);

            if (laButtonsDer != null)
                foreach (HtmlButton loBtn in laButtonsDer)
                    AddClientButton(loControlRight, loBtn);

            poTabs[liIndexTab].Visible = true;
            pnlPanel.Visible = true;
            plstTabs.Add(new ReporteUsuarioTab(liIndexTab, lsTitle, poTabs[liIndexTab].Panel.ClientID, pnlPanel.ClientID));
        }

        public void SetClientData()
        {
            poRepUsrData.CurrentUser = (int)Session["iCodUsuario"];
            poRepUsrData.Action = "";
            poRepUsrData.ReportType = int.Parse((string)cboReportType.DataValue);

            if (poRepUsrDataClient != null)
            {
                poRepUsrData.Id = poRepUsrDataClient.Id;
                poRepUsrData.VchCodigo = poRepUsrDataClient.VchCodigo;
                poRepUsrData.Name = poRepUsrDataClient.Name;
                poRepUsrData.AccessType = poRepUsrDataClient.AccessType;
                poRepUsrData.ReportType = poRepUsrDataClient.ReportType;
                poRepUsrData.ExportExt = poRepUsrDataClient.ExportExt;
                poRepUsrData.Modified = poRepUsrDataClient.Modified;

                poRepUsrData.LastCriteriaRow = poRepUsrDataClient.LastCriteriaRow;
                poRepUsrData.LastCriteriaGrpRow = poRepUsrDataClient.LastCriteriaGrpRow;
                poRepUsrData.CriteriasList = poRepUsrDataClient.CriteriasList;
                poRepUsrData.CriteriasGrpList = poRepUsrDataClient.CriteriasGrpList;
                poRepUsrData.Generated = poRepUsrDataClient.Generated;
                poRepUsrData.Conditions = poRepUsrDataClient.Conditions;
                poRepUsrData.ConditionsGrp = poRepUsrDataClient.ConditionsGrp;

                poRepUsrData.ShowGraph = poRepUsrDataClient.ShowGraph;
                poRepUsrData.GraphType = poRepUsrDataClient.GraphType;
                poRepUsrData.GraphX = poRepUsrDataClient.GraphX;
                poRepUsrData.GraphY = poRepUsrDataClient.GraphY;

                poRepUsrData.CreatorUser = poRepUsrDataClient.CreatorUser;

                poRepUsrData.ShowNumRecs = poRepUsrDataClient.ShowNumRecs;
                poRepUsrData.TopDir = poRepUsrDataClient.TopDir;
                poRepUsrData.TopN = poRepUsrDataClient.TopN;


                poRepUsrData.MailFreq = poRepUsrDataClient.MailFreq;
                poRepUsrData.MailFechaUnaVez = poRepUsrDataClient.MailFechaUnaVez;
                poRepUsrData.MailDiasHabiles = poRepUsrDataClient.MailDiasHabiles;
                poRepUsrData.MailDiaSemana = poRepUsrDataClient.MailDiaSemana;
                poRepUsrData.MailDiaMes = poRepUsrDataClient.MailDiaMes;
                poRepUsrData.MailSemanaMes = poRepUsrDataClient.MailSemanaMes;
                poRepUsrData.MailDiaSemanaMes = poRepUsrDataClient.MailDiaSemanaMes;

                poRepUsrData.MailTo = poRepUsrDataClient.MailTo;
                poRepUsrData.MailCC = poRepUsrDataClient.MailCC;
                poRepUsrData.MailBCC = poRepUsrDataClient.MailBCC;
                poRepUsrData.MailSubject = poRepUsrDataClient.MailSubject;
                poRepUsrData.MailMessage = poRepUsrDataClient.MailMessage;


                foreach (ReporteUsuarioField loFld in poRepUsrData.FieldsList)
                    foreach (ReporteUsuarioField loFldC in poRepUsrDataClient.FieldsList)
                        if (loFldC.Id == loFld.Id)
                        {
                            loFld.IsSelected = loFldC.IsSelected;
                            loFld.FieldOrder = loFldC.FieldOrder;
                            loFld.AggregateFn = loFldC.AggregateFn;
                            loFld.DataOrder = loFldC.DataOrder;
                            loFld.DataOrderType = loFldC.DataOrderType;
                            loFld.Group = loFldC.Group;
                            loFld.GroupType = loFldC.GroupType;
                            loFld.GroupMatX = loFldC.GroupMatX;
                            loFld.GroupMatY = loFldC.GroupMatY;
                            loFld.TipoPeriodoGrp = loFldC.TipoPeriodoGrp;
                            loFld.TipoPeriodoGrpX = loFldC.TipoPeriodoGrpX;
                            loFld.TipoPeriodoGrpY = loFldC.TipoPeriodoGrpY;
                            break;
                        }
            }

            poRepUsrData.TextBoxDataId = txtReportData.ClientID;
            poRepUsrData.TextBoxNameId = txtNombre.TextBox.ClientID;
            //poRepUsrData.CalendarImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(DSODateTimeBox), "DSOControls2008.images.calendar.gif");

            poLang.ItemsList.Add(new ReporteUsuarioItem("CalImg", Page.ClientScript.GetWebResourceUrl(typeof(DSODateTimeBox), "DSOControls2008.images.calendar.gif")));

            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioInitReport",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { oRepUsr.initReport($('#" + txtReportData.ClientID + "').val(), $('#" + txtLangData.ClientID + "').val()); });" +
                "</script>\r\n",
                true, false);

            poPrevData.ShowGraph = poRepUsrData.ShowGraph;
        }

        public void SetReporte()
        {
            if (poRepUsrDataClient != null && poRepUsrDataClient.Generated == 0 && Reporte != null)
            {
                ClearChildViewState();
                Reporte.Controls.Clear();
                Reporte = null;
            }

            //Validaciones
            if (poRepUsrDataClient != null && poRepUsrDataClient.Generated != 0 && !RepUsuExecValidate())
            {
                poRepUsrDataClient.Generated = 0;
                return;
            }

            if (poRepUsrDataClient != null && poRepUsrDataClient.BaseReport != -1 && poRepUsrDataClient.Generated != 0 && Reporte == null)
            {
                int liCodRepStd = piCodRepStd;

                if (Reporte == null)
                    liCodRepStd = SaveReportStd();

                if (liCodRepStd != -1)
                {
                    piCodRepStd = liCodRepStd;

                    if (Reporte == null)
                        Reporte = new ReporteEstandar();

                    Reporte.ID = "Reporte" + liCodRepStd + poRepUsrDataClient.ShowGraph;
                    Reporte.lblTitle = lblTitulo;
                    tdOutput.Controls.Add(Reporte);

                    Reporte.OpcMnu = Request.Params["Opc"];
                    Reporte.iCodConsulta = (int)Util.IsDBNull(KDBUtil.SearchScalar("OpcMnu", (string)Request.Params["Opc"], "{Aplic}", true), -1);
                    Reporte.iCodReporte = liCodRepStd;
                    Reporte.iCodPerfil = (int)Session["iCodPerfil"];
                    Reporte.iEstadoConsulta = 0;

                    Reporte.ExportExt = poRepUsrDataClient.ExportExt;
                    Reporte.LoadScripts();

                    try
                    {
                        Reporte.CreateControls();
                        Reporte.iCodReporte = liCodRepStd;
                    }
                    catch (Exception ex)
                    {
                        Util.LogException("Error al crear controles del reporte estandar desde reporte usuario.", ex);

                        try
                        {
                            Reporte.Controls.Clear();
                            Reporte.CreateControls();
                            Reporte.iCodReporte = liCodRepStd;
                        }
                        catch (Exception ex2)
                        {
                            Util.LogException("Error al crear controles del reporte estandar desde reporte usuario.(2)", ex2);
                        }
                    }

                    lbRepGen = true;
                }
            }

            if (poRepUsrDataClient != null && poRepUsrDataClient.BaseReport != -1 && poRepUsrDataClient.Generated != 0 && Reporte != null && pbExport)
                switch (poExport)
                {
                    case KeytiaExportFormat.xlsx:
                        Reporte.ExportXLS();
                        break;
                    case KeytiaExportFormat.docx:
                        Reporte.ExportDOC();
                        break;
                    case KeytiaExportFormat.pdf:
                        Reporte.ExportPDF();
                        break;
                    case KeytiaExportFormat.csv:
                        Reporte.ExportCSV();
                        break;
                }

            if (poRepUsrDataClient != null && Reporte != null)
                poRepUsrDataClient.ExportExt = Reporte.ExportExt;
        }

        protected void GetFields(string lsReport, string liCodReporte)
        {
            if (poRepUsrData.FieldsList.Count == 0 && poRepUsrData.CategoriesList.Count == 0 && liCodReporte != "")
                poRepUsrData.LoadBaseReport(int.Parse(liCodReporte));
        }

        protected HtmlButton CreateClientButton(string lsId, string lsText, string lsCssClass, string lsScript)
        {
            HtmlButton lbtnRet = new HtmlButton();

            lbtnRet.ID = lsId;
            lbtnRet.InnerText = lsText;

            if (lsCssClass != "")
                lbtnRet.Attributes.Add("class", lsCssClass);

            lbtnRet.Attributes.Add("onclick", lsScript);

            return lbtnRet;
        }

        protected HtmlButton AddClientButton(Control loParent, string lsId, string lsText, string lsCssClass, string lsScript)
        {
            return AddClientButton(loParent, CreateClientButton(lsId, lsText, lsCssClass, lsScript));
        }

        protected HtmlButton AddClientButton(Control loParent, HtmlButton lbtnRet)
        {
            string lsScript = lbtnRet.Attributes["onclick"];

            lbtnRet.Attributes.Remove("onclick");

            loParent.Controls.Add(lbtnRet);

            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioBtn" + lbtnRet.ID,
                "<script language=\"javascript\">" +
                "$(document).ready(function() { $('#" + lbtnRet.ClientID + "').click(function(event) { event.preventDefault(); " + lsScript + "; return false; }) });" +
                "</script>\r\n",
                true, false);

            return lbtnRet;
        }

        public int SaveReport()
        {
            int liCodCatRU = -1;

            if (poRepUsrDataClient != null)
            {
                CargasCOM loCom = new CargasCOM();
                Hashtable lht = new Hashtable();

                DataTable ldtFlds;
                DataTable ldtCrits;
                DataRow[] laRows;

                string lsRUDescr = "";

                //Tipo de reporte
                string lsTipoReporte = (string)KDBUtil.SearchScalar("RepUsuTpRep", poRepUsrDataClient.ReportType, "vchCodigo");
                poRepUsrDataClient.AccessType = int.Parse(rblAccessType.DataValue.ToString());

                //Encabezado
                lht.Clear();
                lht.Add("{RepUsu}", poBaseReport.DataValue);
                lht.Add("{RepUsuAcceso}", rblAccessType.DataValue);
                lht.Add("{RepUsuTpRep}", cboReportType.DataValue);
                lht.Add("{RepUsuUltCritId}", poRepUsrDataClient.LastCriteriaRow);
                lht.Add("{RepUsuUltCritGrpId}", poRepUsrDataClient.LastCriteriaGrpRow);
                lht.Add("{RepUsuConds}", poRepUsrDataClient.Conditions);
                lht.Add("{RepUsuCondsGrp}", poRepUsrDataClient.ConditionsGrp);
                lht.Add("{Descripcion}", txtDescripcion.TextBox.Text);
                lht.Add("{RepTipoGrafica}", (poRepUsrDataClient.GraphType != -1 ? (object)poRepUsrDataClient.GraphType : null));
                lht.Add("{RepUsuGraficaX}", (poRepUsrDataClient.GraphX != -1 ? (object)poRepUsrDataClient.GraphX : null));
                lht.Add("{RepUsuGraficaY}", (poRepUsrDataClient.GraphY != -1 ? (object)poRepUsrDataClient.GraphY : null));
                lht.Add("{RepUsuBan2}", 0 +
                    (poRepUsrDataClient.ShowGraph != 0 ? (int)KDBUtil.SearchScalar("Valores", "RepUsuBanAreaGrafica", "{Value}", true) : 0) +
                    (poRepUsrDataClient.ShowNumRecs != 0 ? (int)KDBUtil.SearchScalar("Valores", "RepUsuBanNumRegs", "{Value}", true) : 0) +
                    (poRepUsrDataClient.TopDir > 0 ? (int)KDBUtil.SearchScalar("Valores", "RepUsuBanTop", "{Value}", true) : 0) +
                    (poRepUsrDataClient.TopDir < 0 ? (int)KDBUtil.SearchScalar("Valores", "RepUsuBanBottom", "{Value}", true) : 0));

                lht.Add("{TopRegs}", poRepUsrDataClient.TopN);
                lht.Add("{Msg}", poRepUsrDataClient.MailMessage);

                poRepUsrDataClient.CreatorUser = (int)Session["iCodUsuario"];

                if (poRepUsrDataClient.Id == -1 || txtNombre.TextBox.Text != "")
                {
                    poRepUsrDataClient.VchCodigo = "RU." + DateTime.Now.ToString("yyyyMMdd.HHmmss.fff");
                    lsRUDescr = txtNombre.TextBox.Text;
                }
                else
                    lsRUDescr = ((TextBox)(poReportesUsuario.Control)).Text;

                poRepUsrDataClient.Id = KDBUtil.SaveHistoric("RepUsu", "Reportes Usuario", poRepUsrDataClient.VchCodigo, lsRUDescr, lht, ReturnOnSaveEnum.iCodRegistro);
                poRepUsrDataClient.Name = lsRUDescr;
                liCodCatRU = KDBUtil.SearchICodCatalogo("RepUsu", poRepUsrDataClient.VchCodigo);


                //Detalles
                if (liCodCatRU != -1)
                {
                    //Campos
                    ldtFlds = kdb.GetHisRegByEnt("RepUsu", "Detalle Campos",
                        new string[] { "iCodRegistro", "{RepUsuCampo}" },
                        "{RepUsu} = " + liCodCatRU);

                    ldtFlds.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                    foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                    {
                        if (loFld.IsSelected)
                        {
                            lht.Clear();
                            lht.Add("{RepUsu}", liCodCatRU);
                            lht.Add("{RepUsuCampo}", loFld.Id);
                            lht.Add("{RepEstOrdenCampo}", loFld.FieldOrder);
                            lht.Add("{RepUsuAggFn}", loFld.AggregateFn);

                            lht.Add("{RepUsuOrdDatos}", loFld.DataOrder);
                            lht.Add("{RepUsuTpOrdDatos}", (loFld.DataOrderType != -1 ? (object)loFld.DataOrderType : null));

                            if (lsTipoReporte == "RepUsuTpRes" || lsTipoReporte == "RepUsuTpMat")
                            {
                                lht.Add("{RepUsuOrdDatosGrp}", loFld.Group);
                                lht.Add("{RepUsuTpOrdDatosGrp}", (loFld.GroupType != -1 ? (object)loFld.GroupType : null));

                                lht.Add("{RepUsuOrdDatosGrpMatX}", loFld.GroupMatX);
                                lht.Add("{RepUsuOrdDatosGrpMatY}", loFld.GroupMatY);

                                lht.Add("{TpPeriodo}", loFld.TipoPeriodoGrp +
                                    (loFld.TipoPeriodoGrpX * 8) +
                                    (loFld.TipoPeriodoGrpY * 64));
                            }
                            else
                            {
                                lht.Add("{RepUsuOrdDatosGrp}", -1);
                                lht.Add("{RepUsuTpOrdDatosGrp}", null);

                                lht.Add("{RepUsuOrdDatosGrpMatX}", -1);
                                lht.Add("{RepUsuOrdDatosGrpMatY}", -1);

                                lht.Add("{TpPeriodo}", 0);
                            }

                            if ((laRows = ldtFlds.Select("[{RepUsuCampo}] = " + loFld.Id)).Length > 0)
                                laRows[0]["isSelected"] = true;

                            KDBUtil.SaveHistoric("RepUsu", "Detalle Campos", poRepUsrDataClient.VchCodigo + " Fld." + loFld.Id, poRepUsrDataClient.VchCodigo + " Fld." + loFld.Id, lht);
                        }
                    }

                    foreach (DataRow ldr in ldtFlds.Rows)
                    {
                        if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                            loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                    }


                    //Criterios
                    ldtCrits = kdb.GetHisRegByEnt("RepUsu", "Detalle Criterios",
                        new string[] { "iCodRegistro", "{RepUsuFila}" },
                        "{RepUsu} = " + liCodCatRU);

                    ldtCrits.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                    foreach (ReporteUsuarioCriteria loCrit in poRepUsrDataClient.Criterias)
                    {
                        lht.Clear();
                        lht.Add("{RepUsu}", liCodCatRU);
                        lht.Add("{RepUsuFila}", loCrit.Row);
                        lht.Add("{RepUsuCampo}", loCrit.FieldId);
                        lht.Add("{RepUsuOper}", loCrit.Operator);
                        lht.Add("{RepUsuVal}", loCrit.Value);

                        if ((laRows = ldtCrits.Select("[{RepUsuFila}] = '" + loCrit.Row + "'")).Length > 0)
                            laRows[0]["isSelected"] = true;

                        KDBUtil.SaveHistoric("RepUsu", "Detalle Criterios", poRepUsrDataClient.VchCodigo + " Crit." + loCrit.Row, poRepUsrDataClient.VchCodigo + " Crit." + loCrit.Row, lht);
                    }

                    foreach (DataRow ldr in ldtCrits.Rows)
                    {
                        if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                            loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                    }


                    //Criterios Datos Agrupados
                    ldtCrits = kdb.GetHisRegByEnt("RepUsu", "Detalle Criterios Datos Agrupados",
                        new string[] { "iCodRegistro", "{RepUsuFila}" },
                        "{RepUsu} = " + liCodCatRU);

                    ldtCrits.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                    foreach (ReporteUsuarioCriteria loCrit in poRepUsrDataClient.CriteriasGrp)
                    {
                        lht.Clear();
                        lht.Add("{RepUsu}", liCodCatRU);
                        lht.Add("{RepUsuFila}", loCrit.Row);
                        lht.Add("{RepUsuCampo}", loCrit.FieldId);
                        lht.Add("{RepUsuOper}", loCrit.Operator);
                        lht.Add("{RepUsuVal}", loCrit.Value);

                        if ((laRows = ldtCrits.Select("[{RepUsuFila}] = '" + loCrit.Row + "'")).Length > 0)
                            laRows[0]["isSelected"] = true;

                        KDBUtil.SaveHistoric("RepUsu", "Detalle Criterios Datos Agrupados", poRepUsrDataClient.VchCodigo + " CritG." + loCrit.Row, poRepUsrDataClient.VchCodigo + " CritG." + loCrit.Row, lht);
                    }

                    foreach (DataRow ldr in ldtCrits.Rows)
                    {
                        if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                            loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                    }
                }
            }

            if (liCodCatRU != -1)
                poRepUsrDataClient.Modified = false;

            return liCodCatRU;
        }

        public int SaveReportStd()
        {
            CargasCOM loCom = new CargasCOM();
            Hashtable lht = new Hashtable();

            DataTable ldtFlds;
            DataRow ldrRB;
            DataRow ldrDSRB = null;
            DataRow[] laRows;

            int liCodCatDSRB = -1;
            int liCodCatDSMRB = -1;
            int liCodCatDSRE = -1;
            int liCodCatRE = -1;

            string lsTipoReporte = "";
            string lsCodFld = "";

            bool lbUseConds;
            bool lbAjustar;
            bool lbOrdenar;
            bool lbDebug;
            bool lbGraph;

            StringBuilder lsbWhere;
            StringBuilder lsbHaving;


            //Tipo de reporte
            lsTipoReporte = (string)Util.IsDBNull(KDBUtil.SearchScalar("RepUsuTpRep", poRepUsrDataClient.ReportType, "vchCodigo", true), "");


            string lsCodRE = (poRepUsrDataClient.VchCodigo.Length > 3 ? poRepUsrDataClient.VchCodigo.Substring(3, 15) : "unsaved") +
                "." + Session["iCodUsuario"] +
                "." + poRepUsrDataClient.ReportType;
            string lsCodDS;


            //Datos del reporte base
            ldrRB = KDBUtil.SearchHistoricRow("RepUsu", poRepUsrDataClient.BaseReport, new string[] { "{DataSourceRep}", "{DataSourceRepMat}", "{RepUsuBan}" });
            liCodCatDSRB = (int)Util.IsDBNull(ldrRB["{DataSourceRep}"], -1);
            liCodCatDSMRB = (int)Util.IsDBNull(ldrRB["{DataSourceRepMat}"], -1);


            lbAjustar = (((int)Util.IsDBNull(ldrRB["{RepUsuBan}"], 0) & (int)KDBUtil.SearchScalar("Valores", "RepUsuBanReajustar", "{Value}", true)) != 0);
            lbDebug = (((int)Util.IsDBNull(ldrRB["{RepUsuBan}"], 0) & (int)KDBUtil.SearchScalar("Valores", "RepUsuBanDebug", "{Value}", true)) != 0);
            lbOrdenar = (((int)Util.IsDBNull(ldrRB["{RepUsuBan}"], 0) & (int)KDBUtil.SearchScalar("Valores", "RepUsuBanOrdenar", "{Value}", true)) != 0);
            lbGraph = (lsTipoReporte == "RepUsuTpRes" || lsTipoReporte == "RepUsuTpMat") && poRepUsrDataClient.ShowGraph != 0;


            //Where
            lbUseConds = (poRepUsrDataClient.Conditions.Trim() != "");
            lsbWhere = new StringBuilder("(");

            if (lbUseConds)
                lsbWhere.Append(poRepUsrDataClient.Conditions);

            foreach (ReporteUsuarioCriteria loCrit in poRepUsrDataClient.Criterias)
            {
                if (lbUseConds)
                    lsbWhere.Replace("{" + loCrit.Row + "}", ConvertCriteria(loCrit, lbAjustar));
                else
                {
                    if (lsbWhere.Length > 1)
                        lsbWhere.Append(" and ");

                    lsbWhere.Append(ConvertCriteria(loCrit, lbAjustar));
                }
            }

            if (lsbWhere.Length == 1)
                lsbWhere.Append("1 = 1");

            lsbWhere.Append(")");





            //Having
            lbUseConds = (poRepUsrDataClient.ConditionsGrp.Trim() != "");
            lsbHaving = new StringBuilder("(");

            if (lbUseConds)
                lsbHaving.Append(poRepUsrDataClient.ConditionsGrp);

            foreach (ReporteUsuarioCriteria loCrit in poRepUsrDataClient.CriteriasGrp)
            {
                if (lbUseConds)
                    lsbHaving.Replace("{" + loCrit.Row + "}", ConvertCriteriaGrp(loCrit, lbAjustar, lsTipoReporte));
                else
                {
                    if (lsbHaving.Length > 1)
                        lsbHaving.Append(" and ");

                    lsbHaving.Append(ConvertCriteriaGrp(loCrit, lbAjustar, lsTipoReporte));
                }
            }

            if (lsbHaving.Length == 1)
                lsbHaving.Append("1 = 1");

            lsbHaving.Append(")");




            //Reporte estandar - DataSource
            if (lsTipoReporte == "RepUsuTpTab" || lsTipoReporte == "RepUsuTpRes")
                ldrDSRB = KDBUtil.SearchHistoricRow("DataSourceRep", liCodCatDSRB,
                    new string[] { "iCodCatalogo", "{RepEstDataSource}", "{RepEstDataSourceNumReg}", "{RepEstDataSourceTot}", "{RepEstDataSourceGr}" });
            else if (lsTipoReporte == "RepUsuTpMat")
                ldrDSRB = KDBUtil.SearchHistoricRow("DataSourceRepMat", liCodCatDSMRB,
                    new string[] { "iCodCatalogo", "{RepEstDataSource}", "{RepEstDataSourceNumReg}", "{RepEstDataSourceTot}", "{RepEstDataSourceGr}", "{RepEstDataSourceEjeX}" });

            if (ldrDSRB != null)
            {
                lsCodDS = lsCodRE + ".S" + ldrDSRB["iCodCatalogo"];

                lht.Clear();

                lht.Add("{Usuar}", Session["iCodUsuario"]);
                lht.Add("{RepEstDataSource}", ((string)ldrDSRB["{RepEstDataSource}"])
                    .Replace("Param(Where)", lsbWhere.ToString())
                    .Replace("Param(Having)", lsbHaving.ToString())
                    .Replace("Param(Top)", poRepUsrDataClient.TopDir != 0 ? "top " + poRepUsrDataClient.TopN : "")
                    .Replace("Param(TopDir)", poRepUsrDataClient.TopDir != 0 ? poRepUsrDataClient.TopDir.ToString() : ""));

                if ((string)Util.IsDBNull(ldrDSRB["{RepEstDataSourceNumReg}"], "") != "")
                    lht.Add("{RepEstDataSourceNumReg}", ((string)ldrDSRB["{RepEstDataSourceNumReg}"])
                        .Replace("Param(Where)", lsbWhere.ToString())
                        .Replace("Param(Having)", lsbHaving.ToString())
                        .Replace("Param(Top)", poRepUsrDataClient.TopDir != 0 ? "top " + poRepUsrDataClient.TopN : "")
                        .Replace("Param(TopDir)", poRepUsrDataClient.TopDir != 0 ? poRepUsrDataClient.TopDir.ToString() : ""));

                if ((string)Util.IsDBNull(ldrDSRB["{RepEstDataSourceTot}"], "") != "")
                    lht.Add("{RepEstDataSourceTot}", ((string)ldrDSRB["{RepEstDataSourceTot}"])
                        .Replace("Param(Where)", lsbWhere.ToString())
                        .Replace("Param(Having)", lsbHaving.ToString())
                        .Replace("Param(Top)", poRepUsrDataClient.TopDir != 0 ? "top " + poRepUsrDataClient.TopN : "")
                        .Replace("Param(TopDir)", poRepUsrDataClient.TopDir != 0 ? poRepUsrDataClient.TopDir.ToString() : ""));

                if ((string)Util.IsDBNull(ldrDSRB["{RepEstDataSourceGr}"], "") != "")
                    lht.Add("{RepEstDataSourceGr}", ((string)ldrDSRB["{RepEstDataSourceGr}"])
                        .Replace("Param(Where)", lsbWhere.ToString())
                        .Replace("Param(Having)", lsbHaving.ToString())
                        .Replace("Param(Top)", poRepUsrDataClient.TopDir != 0 ? "top " + poRepUsrDataClient.TopN : "")
                        .Replace("Param(TopDir)", poRepUsrDataClient.TopDir != 0 ? poRepUsrDataClient.TopDir.ToString() : ""));
                else
                    lht.Add("{RepEstDataSourceGr}", ((string)ldrDSRB["{RepEstDataSource}"])
                        .Replace("Param(Where)", lsbWhere.ToString())
                        .Replace("Param(Having)", lsbHaving.ToString())
                        .Replace("Param(Top)", poRepUsrDataClient.TopDir != 0 ? "top " + poRepUsrDataClient.TopN : "")
                        .Replace("Param(TopDir)", poRepUsrDataClient.TopDir != 0 ? poRepUsrDataClient.TopDir.ToString() : ""));

                if (lsTipoReporte == "RepUsuTpTab" || lsTipoReporte == "RepUsuTpRes")
                    liCodCatDSRE = KDBUtil.SaveHistoric("DataSourceRep", "DataSource Reportes", lsCodDS, poRepUsrDataClient.Name + " - " + lsCodDS, lht);
                else if (lsTipoReporte == "RepUsuTpMat")
                {
                    lht.Add("{RepEstDataSourceEjeX}", ((string)ldrDSRB["{RepEstDataSourceEjeX}"])
                        .Replace("Param(Where)", lsbWhere.ToString())
                        .Replace("Param(Having)", lsbHaving.ToString())
                        .Replace("Param(Top)", poRepUsrDataClient.TopDir != 0 ? "top " + poRepUsrDataClient.TopN : "")
                        .Replace("Param(TopDir)", poRepUsrDataClient.TopDir != 0 ? poRepUsrDataClient.TopDir.ToString() : ""));

                    liCodCatDSRE = KDBUtil.SaveHistoric("DataSourceRepMat", "DataSource Reportes", lsCodDS, poRepUsrDataClient.Name + " - " + lsCodDS, lht);
                }
            }



            //Reporte estandar
            lht.Clear();
            lht.Add("{RepEstOrientacionZonas}", 1);
            lht.Add("{RepEstOrdenZonas}", 0);
            lht.Add("{Usuar}", Session["iCodUsuario"]);
            lht.Add("{" + Globals.GetLanguage() + "}", poRepUsrDataClient.Name);

            if (lsTipoReporte == "RepUsuTpTab")
            {
                lht.Add("{DataSourceRep}", liCodCatDSRE);
                lht.Add("{BanderasRepEstandar}",
                    (int)KDBUtil.SearchScalar("Valores", "RepEstandarAreaReporte", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "RepEstandarParamIniFin", "{Value}", true) +
                    (lbAjustar ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarParamAjustar", "{Value}", true) : 0) +
                    (lbDebug ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarModoDebug", "{Value}", true) : 0) +
                    (lbOrdenar ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarOrden", "{Value}", true) : 0));

                liCodCatRE = KDBUtil.SaveHistoric("RepEst", "Tabular", lsCodRE, poRepUsrDataClient.Name + " - " + lsCodRE, lht);
            }
            else if (lsTipoReporte == "RepUsuTpRes")
            {
                lht.Add("{DataSourceRep}", liCodCatDSRE);
                lht.Add("{BanderasRepEstandar}",
                    (int)KDBUtil.SearchScalar("Valores", "RepEstandarAreaReporte", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "RepEstandarParamIniFin", "{Value}", true) +
                    (lbGraph ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarAreaGrafica", "{Value}", true) : 0) +
                    (lbAjustar ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarParamAjustar", "{Value}", true) : 0) +
                    (lbDebug ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarModoDebug", "{Value}", true) : 0) +
                    (lbOrdenar ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarOrden", "{Value}", true) : 0));

                lht.Add("{RepTipoResumen}", (int)KDBUtil.SearchScalar("Valores", "Rollup", "{Value}", true));
                lht.Add("{RepTipoGrafica}", (poRepUsrDataClient.GraphType != -1 ? (object)poRepUsrDataClient.GraphType : null));

                liCodCatRE = KDBUtil.SaveHistoric("RepEst", "Resumido", lsCodRE, poRepUsrDataClient.Name + " - " + lsCodRE, lht);
            }
            else if (lsTipoReporte == "RepUsuTpMat")
            {
                lht.Add("{DataSourceRepMat}", liCodCatDSRE);
                lht.Add("{BanderasRepEstandar}",
                    (int)KDBUtil.SearchScalar("Valores", "RepEstandarAreaReporte", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "RepEstandarParamIniFin", "{Value}", true) +
                    (lbGraph ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarAreaGrafica", "{Value}", true) : 0) +
                    (lbAjustar ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarParamAjustar", "{Value}", true) : 0) +
                    (lbDebug ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarModoDebug", "{Value}", true) : 0) +
                    (lbOrdenar ? (int)KDBUtil.SearchScalar("Valores", "RepEstandarOrden", "{Value}", true) : 0));

                lht.Add("{RepTipoGrafica}", (poRepUsrDataClient.GraphType != -1 ? (object)poRepUsrDataClient.GraphType : null));

                liCodCatRE = KDBUtil.SaveHistoric("RepEst", "Matricial", lsCodRE, poRepUsrDataClient.Name + " - " + lsCodRE, lht);
            }


            if (lsTipoReporte == "RepUsuTpTab" || lsTipoReporte == "RepUsuTpRes")
            {
                //Reporte estandar - Campos
                ldtFlds = kdb.GetHisRegByEnt("RepEstCampo", "Campos",
                    new string[] { "iCodRegistro" },
                    "{RepEst} = " + liCodCatRE);

                ldtFlds.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                {
                    if (loFld.IsSelected)
                    {
                        lsCodFld = lsCodRE + "." + loFld.Id;

                        lht.Clear();
                        lht.Add("{RepEst}", liCodCatRE);
                        lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", loFld.Type, true));
                        lht.Add("{RepEstOrdenCampo}", loFld.FieldOrder);
                        lht.Add("{AggregateFunc}", KDBUtil.SearchScalar("Valores", loFld.AggregateFn, "{Value}", true));
                        lht.Add("{BanderasCamposRep}", 0);


                        if (loFld.AggregateFn != "")
                            lht["{BanderasCamposRep}"] = (int)lht["{BanderasCamposRep}"] + (int)KDBUtil.SearchScalar("Valores", "Totalizado", "{Value}", true);

                        if (loFld.Group != -1)
                            lht["{BanderasCamposRep}"] = (int)lht["{BanderasCamposRep}"] + (int)KDBUtil.SearchScalar("Valores", "Agrupado", "{Value}", true);


                        if (loFld.DataOrder != -1)
                        {
                            lht.Add("{RepEstOrdenDatos}", loFld.DataOrder);
                            lht.Add("{RepEstDirDatos}", loFld.DataOrderType);
                        }
                        else
                        {
                            lht.Add("{RepEstOrdenDatos}", 9999);
                            lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));
                        }

                        lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchScalar("RepUsuCampo", loFld.Id, "{RepEstIdiomaCmp}"));
                        lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy

                        if (lsTipoReporte == "RepUsuTpTab")
                        {
                            lht.Add("{DataField}", loFld.DataField);
                        }
                        else if (lsTipoReporte == "RepUsuTpRes")
                        {
                            if (loFld.Group != -1) //Campo por el que se agrupa
                            {
                                if (loFld.DataField.Contains("="))
                                    lht.Add("{DataField}",
                                        loFld.DataField.Split('=')[0].Trim() + " = " +
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrp == 0 || loFld.TipoPeriodoGrp == 1)) ? "convert(varchar(10), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 2) ? "convert(varchar(7), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 3) ? "convert(varchar(4), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                        loFld.DataField.Split('=')[1].Trim()))));
                                else
                                    lht.Add("{DataField}", loFld.DataField + " = " +
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrp == 0 || loFld.TipoPeriodoGrp == 1)) ? "convert(varchar(10), " + loFld.DataField + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 2) ? "convert(varchar(7), " + loFld.DataField + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 3) ? "convert(varchar(4), " + loFld.DataField + ", 120)" :
                                        loFld.DataField))));
                            }
                            else if (loFld.AggregateFn != "")
                            {
                                if (loFld.DataField.Contains("="))
                                    lht.Add("{DataField}",
                                        loFld.DataField.Split('=')[0].Trim() + " = " +
                                        loFld.AggregateFn + "(" + loFld.DataField.Split('=')[1].Trim() + ")");
                                else
                                    lht.Add("{DataField}", loFld.DataField + " = " +
                                        loFld.AggregateFn + "(" + loFld.DataField + ")");
                            }
                            else
                            {
                                if (loFld.DataField.Contains("="))
                                    lht.Add("{DataField}",
                                        loFld.DataField.Split('=')[0].Trim() + " = " +
                                        "min(" + loFld.DataField.Split('=')[1].Trim() + ")");
                                else
                                    lht.Add("{DataField}", loFld.DataField + " = " +
                                        "min(" + loFld.DataField + ")");
                            }
                        }

                        KDBUtil.SaveHistoric("RepEstCampo", "Campos", lsCodFld, loFld.Name + " - " + lsCodFld, lht);

                        if ((laRows = ldtFlds.Select("vchCodigo = '" + lsCodFld + "'")).Length > 0)
                            laRows[0]["isSelected"] = true;
                    }
                }

                foreach (DataRow ldr in ldtFlds.Rows)
                {
                    if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                        loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                }

                //Campo de número de registros
                if (lsTipoReporte == "RepUsuTpRes")
                {
                    lsCodFld = lsCodRE + ".NRG";

                    if (poRepUsrDataClient.ShowNumRecs != 0)
                    {
                        lht.Clear();
                        lht.Add("{RepEst}", liCodCatRE);
                        lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", "Int", true));
                        lht.Add("{RepEstOrdenCampo}", 9999);
                        lht.Add("{AggregateFunc}", null);
                        lht.Add("{BanderasCamposRep}", 0);

                        lht.Add("{RepEstOrdenDatos}", 9999);
                        lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));

                        lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchICodCatalogo("RepEstIdiomaCmp", "NumRegs", true));
                        lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy

                        lht.Add("{DataField}", "NumRegsGrp = count(*)");

                        KDBUtil.SaveHistoric("RepEstCampo", "Campos", lsCodFld, "NumRegs - " + lsCodFld, lht);
                    }
                    else
                    {
                        int liCodRegNumRegs = KDBUtil.SearchICodRegistro("RepEstCampo", lsCodFld);

                        if (liCodRegNumRegs != -1)
                            loCom.EliminarRegistro("Historicos", liCodRegNumRegs, DSODataContext.GetContext());
                    }
                }


                //Reporte estandar - Campos Gráfica
                if (lsTipoReporte == "RepUsuTpRes")
                {
                    ldtFlds = kdb.GetHisRegByEnt("RepEstCampo", "Campos Grafica",
                        new string[] { "iCodRegistro" },
                        "{RepEst} = " + liCodCatRE);

                    ldtFlds.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                    foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                    {
                        if (loFld.IsSelected &&
                            (loFld.Id == poRepUsrDataClient.GraphX || loFld.Id == poRepUsrDataClient.GraphY) &&
                            (loFld.Group != -1 || loFld.AggregateFn != ""))
                        {
                            lsCodFld = lsCodRE + ".G" + loFld.Id;

                            lht.Clear();
                            lht.Add("{RepEst}", liCodCatRE);
                            lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", loFld.Type, true));
                            lht.Add("{RepEstOrdenCampo}", loFld.FieldOrder);

                            lht.Add("{TipoCampoGr}",
                                (loFld.Id == poRepUsrDataClient.GraphX ?
                                KDBUtil.SearchScalar("Valores", "TipoCampoGrEjeX", "{Value}", true) :
                                KDBUtil.SearchScalar("Valores", "TipoCampoGrSerie", "{Value}", true)));

                            lht.Add("{BanderasCamposGr}",
                                (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaPastel", "{Value}", true) +
                                (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaBarras", "{Value}", true) +
                                (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaLineas", "{Value}", true) +
                                (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaArea", "{Value}", true));


                            if (loFld.DataOrder != -1)
                            {
                                lht.Add("{RepEstOrdenDatos}", loFld.DataOrder);
                                lht.Add("{RepEstDirDatos}", (loFld.DataOrderType != -1 ? loFld.DataOrderType : Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null)));
                            }
                            else
                            {
                                lht.Add("{RepEstOrdenDatos}", 9999);
                                lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));
                            }

                            lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchScalar("RepUsuCampo", loFld.Id, "{RepEstIdiomaCmp}"));
                            lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy

                            if ((laRows = ldtFlds.Select("vchCodigo = '" + lsCodFld + "'")).Length > 0)
                                laRows[0]["isSelected"] = true;

                            if (loFld.Group != -1) //Campo por el que se agrupa
                            {
                                if (loFld.DataField.Contains("="))
                                    lht.Add("{DataField}",
                                        loFld.DataField.Split('=')[0].Trim() + " = " +
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrp == 0 || loFld.TipoPeriodoGrp == 1)) ? "convert(varchar(10), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 2) ? "convert(varchar(7), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 3) ? "convert(varchar(4), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                        loFld.DataField.Split('=')[1].Trim()))));
                                else
                                    lht.Add("{DataField}", loFld.DataField + " = " +
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrp == 0 || loFld.TipoPeriodoGrp == 1)) ? "convert(varchar(10), " + loFld.DataField + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 2) ? "convert(varchar(7), " + loFld.DataField + ", 120)" :
                                        (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 3) ? "convert(varchar(4), " + loFld.DataField + ", 120)" :
                                        loFld.DataField))));
                            }
                            else if (loFld.AggregateFn != "")
                            {
                                if (loFld.DataField.Contains("="))
                                    lht.Add("{DataField}",
                                        loFld.DataField.Split('=')[0].Trim() + " = " +
                                        loFld.AggregateFn + "(" + loFld.DataField.Split('=')[1].Trim() + ")");
                                else
                                    lht.Add("{DataField}", loFld.DataField + " = " +
                                        loFld.AggregateFn + "(" + loFld.DataField + ")");
                            }

                            KDBUtil.SaveHistoric("RepEstCampo", "Campos Grafica", lsCodFld, loFld.Name + " - " + lsCodFld, lht);
                        }
                    }

                    foreach (DataRow ldr in ldtFlds.Rows)
                    {
                        if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                            loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                    }
                }
            }
            else if (lsTipoReporte == "RepUsuTpMat")
            {
                //Reporte estandar - Campos X
                ldtFlds = kdb.GetHisRegByEnt("RepEstCampo", "Campos Eje X",
                    new string[] { "iCodRegistro" },
                    "{RepEst} = " + liCodCatRE);

                ldtFlds.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                {
                    if (loFld.IsSelected && loFld.GroupMatX != -1)
                    {
                        lsCodFld = lsCodRE + ".X" + loFld.Id;

                        lht.Clear();
                        lht.Add("{RepEst}", liCodCatRE);
                        lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", loFld.Type, true));
                        lht.Add("{RepEstOrdenCampo}", loFld.GroupMatX);
                        lht.Add("{BanderasCamposX}", KDBUtil.SearchScalar("Valores", "Agrupado", "{Value}", true));
                        lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchScalar("RepUsuCampo", loFld.Id, "{RepEstIdiomaCmp}"));
                        lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy

                        if (loFld.DataOrder != -1)
                        {
                            lht.Add("{RepEstOrdenDatos}", loFld.DataOrder);
                            lht.Add("{RepEstDirDatos}", (loFld.DataOrderType != -1 ? loFld.DataOrderType : Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null)));
                        }
                        else
                        {
                            lht.Add("{RepEstOrdenDatos}", 9999);
                            lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));
                        }

                        if (loFld.DataField.Contains("="))
                            lht.Add("{DataField}",
                                loFld.DataField.Split('=')[0].Trim() + " = " +
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrpX == 0 || loFld.TipoPeriodoGrpX == 1)) ? "convert(varchar(10), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 2) ? "convert(varchar(7), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 3) ? "convert(varchar(4), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                loFld.DataField.Split('=')[1].Trim()))));
                        else
                            lht.Add("{DataField}", loFld.DataField + " = " +
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrpX == 0 || loFld.TipoPeriodoGrpX == 1)) ? "convert(varchar(10), " + loFld.DataField + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 2) ? "convert(varchar(7), " + loFld.DataField + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 3) ? "convert(varchar(4), " + loFld.DataField + ", 120)" :
                                loFld.DataField))));

                        if ((laRows = ldtFlds.Select("vchCodigo = '" + lsCodFld + "'")).Length > 0)
                            laRows[0]["isSelected"] = true;

                        KDBUtil.SaveHistoric("RepEstCampo", "Campos Eje X", lsCodFld, loFld.Name + " - " + lsCodFld, lht);
                    }
                }

                foreach (DataRow ldr in ldtFlds.Rows)
                {
                    if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                        loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                }


                //Reporte estandar - Campos Y
                ldtFlds = kdb.GetHisRegByEnt("RepEstCampo", "Campos Eje Y",
                    new string[] { "iCodRegistro" },
                    "{RepEst} = " + liCodCatRE);

                ldtFlds.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                {
                    if (loFld.IsSelected && loFld.GroupMatY != -1)
                    {
                        lsCodFld = lsCodRE + ".Y" + loFld.Id;

                        lht.Clear();
                        lht.Add("{RepEst}", liCodCatRE);
                        lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", loFld.Type, true));
                        lht.Add("{RepEstOrdenCampo}", loFld.GroupMatY);
                        lht.Add("{BanderasCamposY}", KDBUtil.SearchScalar("Valores", "Agrupado", "{Value}", true));
                        lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchScalar("RepUsuCampo", loFld.Id, "{RepEstIdiomaCmp}"));
                        lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy

                        if (loFld.DataOrder != -1)
                        {
                            lht.Add("{RepEstOrdenDatos}", loFld.DataOrder);
                            lht.Add("{RepEstDirDatos}", (loFld.DataOrderType != -1 ? loFld.DataOrderType : Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null)));
                        }
                        else
                        {
                            lht.Add("{RepEstOrdenDatos}", 9999);
                            lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));
                        }

                        if (loFld.DataField.Contains("="))
                            lht.Add("{DataField}",
                                loFld.DataField.Split('=')[0].Trim() + " = " +
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrpY == 0 || loFld.TipoPeriodoGrpY == 1)) ? "convert(varchar(10), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpY == 2) ? "convert(varchar(7), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpY == 3) ? "convert(varchar(4), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                loFld.DataField.Split('=')[1].Trim()))));
                        else
                            lht.Add("{DataField}", loFld.DataField + " = " +
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrpY == 0 || loFld.TipoPeriodoGrpY == 1)) ? "convert(varchar(10), " + loFld.DataField + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpY == 2) ? "convert(varchar(7), " + loFld.DataField + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpY == 3) ? "convert(varchar(4), " + loFld.DataField + ", 120)" :
                                loFld.DataField))));

                        if ((laRows = ldtFlds.Select("vchCodigo = '" + lsCodFld + "'")).Length > 0)
                            laRows[0]["isSelected"] = true;

                        KDBUtil.SaveHistoric("RepEstCampo", "Campos Eje Y", lsCodFld, loFld.Name + " - " + lsCodFld, lht);
                    }
                }

                foreach (DataRow ldr in ldtFlds.Rows)
                {
                    if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                        loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                }


                //Reporte estandar - Campos XY
                ldtFlds = kdb.GetHisRegByEnt("RepEstCampo", "Campos XY",
                    new string[] { "iCodRegistro" },
                    "{RepEst} = " + liCodCatRE);

                ldtFlds.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                {
                    if (loFld.IsSelected && loFld.AggregateFn != "")
                    {
                        lsCodFld = lsCodRE + ".XY" + loFld.Id;

                        lht.Clear();
                        lht.Add("{RepEst}", liCodCatRE);
                        lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", loFld.Type, true));
                        lht.Add("{RepEstOrdenCampo}", loFld.GroupMatX);
                        lht.Add("{AggregateFunc}", KDBUtil.SearchScalar("Valores", loFld.AggregateFn, "{Value}", true));

                        lht.Add("{BanderasCamposXY}",
                            (int)KDBUtil.SearchScalar("Valores", "TotalizadoXYTotX", "{Value}", true) +
                            (int)KDBUtil.SearchScalar("Valores", "TotalizadoXYTotY", "{Value}", true));

                        if (loFld.DataOrder != -1)
                        {
                            lht.Add("{RepEstOrdenDatos}", loFld.DataOrder);
                            lht.Add("{RepEstDirDatos}", (loFld.DataOrderType != -1 ? loFld.DataOrderType : Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null)));
                        }
                        else
                        {
                            lht.Add("{RepEstOrdenDatos}", 9999);
                            lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));
                        }

                        lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchScalar("RepUsuCampo", loFld.Id, "{RepEstIdiomaCmp}"));
                        lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy
                        lht.Add("{MatrizFunc}", KDBUtil.SearchScalar("Valores", "SUM_Matriz", "{Value}", true));

                        if (loFld.DataField.Contains("="))
                            lht.Add("{DataField}",
                                loFld.DataField.Split('=')[0].Trim() + " = " +
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrpX == 0 || loFld.TipoPeriodoGrpX == 1 || loFld.TipoPeriodoGrpY == 0 || loFld.TipoPeriodoGrpY == 1)) ? "convert(varchar(10), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 2 || loFld.TipoPeriodoGrpY == 2) ? "convert(varchar(7), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 3 || loFld.TipoPeriodoGrpY == 3) ? "convert(varchar(4), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                loFld.DataField.Split('=')[1].Trim()))));
                        else
                            lht.Add("{DataField}", loFld.DataField + " = " + loFld.AggregateFn + "(" +
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrpX == 0 || loFld.TipoPeriodoGrpX == 1 || loFld.TipoPeriodoGrpY == 0 || loFld.TipoPeriodoGrpY == 1)) ? "convert(varchar(10), " + loFld.DataField + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 2 || loFld.TipoPeriodoGrpY == 2) ? "convert(varchar(7), " + loFld.DataField + ", 120)" :
                                (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrpX == 3 || loFld.TipoPeriodoGrpY == 3) ? "convert(varchar(4), " + loFld.DataField + ", 120)" :
                                loFld.DataField))) + ")");

                        if ((laRows = ldtFlds.Select("vchCodigo = '" + lsCodFld + "'")).Length > 0)
                            laRows[0]["isSelected"] = true;

                        KDBUtil.SaveHistoric("RepEstCampo", "Campos XY", lsCodFld, loFld.Name + " - " + lsCodFld, lht);
                    }
                }

                foreach (DataRow ldr in ldtFlds.Rows)
                {
                    if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                        loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                }

                if (lsTipoReporte == "RepUsuTpMat")
                {
                    lsCodFld = lsCodRE + ".NRGXY";

                    if (poRepUsrDataClient.ShowNumRecs != 0)
                    {
                        lht.Clear();
                        lht.Add("{RepEst}", liCodCatRE);
                        lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", "Int", true));
                        lht.Add("{RepEstOrdenCampo}", 9999);
                        lht.Add("{AggregateFunc}", KDBUtil.SearchScalar("Valores", "sum", "{Value}", true));

                        lht.Add("{BanderasCamposXY}",
                            (int)KDBUtil.SearchScalar("Valores", "TotalizadoXYTotX", "{Value}", true) +
                            (int)KDBUtil.SearchScalar("Valores", "TotalizadoXYTotY", "{Value}", true));

                        lht.Add("{RepEstOrdenDatos}", 9999);
                        lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));

                        lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchICodCatalogo("RepEstIdiomaCmp", "NumRegs", true));
                        lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy
                        lht.Add("{MatrizFunc}", KDBUtil.SearchScalar("Valores", "SUM_Matriz", "{Value}", true));

                        lht.Add("{DataField}", "NumRegsGrp = count(*)");

                        KDBUtil.SaveHistoric("RepEstCampo", "Campos XY", lsCodFld, "NumRegs - " + lsCodFld, lht);
                    }
                    else
                    {
                        int liCodRegNumRegs = KDBUtil.SearchICodRegistro("RepEstCampo", lsCodFld);

                        if (liCodRegNumRegs != -1)
                            loCom.EliminarRegistro("Historicos", liCodRegNumRegs, DSODataContext.GetContext());
                    }
                }

                //Reporte estandar - Campos Grafica
                ldtFlds = kdb.GetHisRegByEnt("RepEstCampo", "Campos Grafica",
                    new string[] { "iCodRegistro" },
                    "{RepEst} = " + liCodCatRE);

                ldtFlds.Columns.Add("isSelected", System.Type.GetType("System.Boolean"));

                foreach (ReporteUsuarioField loFld in poRepUsrDataClient.Fields)
                {
                    if (loFld.IsSelected &&
                        (loFld.Id == poRepUsrDataClient.GraphX || loFld.Id == poRepUsrDataClient.GraphY) &&
                        (loFld.GroupMatY != -1 || loFld.AggregateFn != ""))
                    {
                        lsCodFld = lsCodRE + ".GM" + loFld.Id;

                        lht.Clear();
                        lht.Add("{RepEst}", liCodCatRE);
                        lht.Add("{Types}", KDBUtil.SearchICodCatalogo("Types", loFld.Type, true));
                        lht.Add("{RepEstOrdenCampo}", loFld.FieldOrder);

                        lht.Add("{TipoCampoGr}",
                            (loFld.Id == poRepUsrDataClient.GraphX ?
                            KDBUtil.SearchScalar("Valores", "TipoCampoGrEjeX", "{Value}", true) :
                            KDBUtil.SearchScalar("Valores", "TipoCampoGrSerie", "{Value}", true)));

                        lht.Add("{BanderasCamposGr}",
                            (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaPastel", "{Value}", true) +
                            (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaBarras", "{Value}", true) +
                            (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaLineas", "{Value}", true) +
                            (int)KDBUtil.SearchScalar("Valores", "RepTipoGraficaArea", "{Value}", true));


                        if (loFld.DataOrder != -1)
                        {
                            lht.Add("{RepEstOrdenDatos}", loFld.DataOrder);
                            lht.Add("{RepEstDirDatos}", (loFld.DataOrderType != -1 ? loFld.DataOrderType : Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null)));
                        }
                        else
                        {
                            lht.Add("{RepEstOrdenDatos}", 9999);
                            lht.Add("{RepEstDirDatos}", Util.IsDBNull(KDBUtil.SearchScalar("Valores", "RepEstDirAsc", "{Value}", true), null));
                        }

                        lht.Add("{RepEstIdiomaCmp}", KDBUtil.SearchScalar("RepUsuCampo", loFld.Id, "{RepEstIdiomaCmp}"));
                        lht.Add("{Atrib}", KDBUtil.SearchICodCatalogo("Atrib", "Value")); //Dummy

                        if ((laRows = ldtFlds.Select("vchCodigo = '" + lsCodFld + "'")).Length > 0)
                            laRows[0]["isSelected"] = true;

                        if (loFld.GroupMatY != -1) //Campo por el que se agrupa
                        {
                            if (loFld.DataField.Contains("="))
                                lht.Add("{DataField}",
                                    loFld.DataField.Split('=')[0].Trim() + " = " +
                                    (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrp == 0 || loFld.TipoPeriodoGrp == 1)) ? "convert(varchar(10), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                    (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 2) ? "convert(varchar(7), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                    (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 3) ? "convert(varchar(4), " + loFld.DataField.Split('=')[1].Trim() + ", 120)" :
                                    loFld.DataField.Split('=')[1].Trim()))));
                            else
                                lht.Add("{DataField}", loFld.DataField + " = " +
                                    (((loFld.Type == "Date" || loFld.Type == "DateTime") && (loFld.TipoPeriodoGrp == 0 || loFld.TipoPeriodoGrp == 1)) ? "convert(varchar(10), " + loFld.DataField + ", 120)" :
                                    (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 2) ? "convert(varchar(7), " + loFld.DataField + ", 120)" :
                                    (((loFld.Type == "Date" || loFld.Type == "DateTime") && loFld.TipoPeriodoGrp == 3) ? "convert(varchar(4), " + loFld.DataField + ", 120)" :
                                    loFld.DataField))));
                        }
                        else if (loFld.AggregateFn != "")
                        {
                            if (loFld.DataField.Contains("="))
                                lht.Add("{DataField}",
                                    loFld.DataField.Split('=')[0].Trim() + " = " +
                                    loFld.AggregateFn + "(" + loFld.DataField.Split('=')[1].Trim() + ")");
                            else
                                lht.Add("{DataField}", loFld.DataField + " = " +
                                    loFld.AggregateFn + "(" + loFld.DataField + ")");
                        }

                        KDBUtil.SaveHistoric("RepEstCampo", "Campos Grafica", lsCodFld, loFld.Name + " - " + lsCodFld, lht);
                    }
                }

                foreach (DataRow ldr in ldtFlds.Rows)
                {
                    if (!(bool)Util.IsDBNull(ldr["isSelected"], false))
                        loCom.EliminarRegistro("Historicos", (int)ldr["iCodRegistro"], DSODataContext.GetContext());
                }
            }

            return liCodCatRE;
        }

        public int SaveAlarm(int liCodCatRepEst)
        {
            CargasCOM loCom = new CargasCOM();
            Hashtable lht = new Hashtable();

            int liCodCatAlarma = -1;

            if (poRepUsrDataClient.MailFreq == 0)
            {
                DataRow ldrAlarma = KDBUtil.SearchHistoricRow("Alarm", poRepUsrDataClient.VchCodigo, new string[] { "iCodRegistro" });

                if (ldrAlarma != null)
                    loCom.EliminarRegistro("Historicos", (int)ldrAlarma["iCodRegistro"], DSODataContext.GetContext());

                return -1;
            }

            if (liCodCatRepEst == -1)
                return -1;

            string lsHora = "06:00:00";

            //Asunto
            lht.Clear();
            lht.Add("{" + Globals.GetLanguage() + "}", poRepUsrDataClient.MailSubject);
            int liCodCatAsunto = KDBUtil.SaveHistoric("Asunto", "Asunto de Correo Electrónico", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.MailSubject, lht);

            //Mensaje
            string lsFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), poRepUsrDataClient.VchCodigo + ".doc");
            WordAccess loWA = new WordAccess();
            loWA.Abrir(false);
            loWA.InsertarTexto(poRepUsrDataClient.MailMessage);
            loWA.FilePath = lsFile;
            loWA.SalvarComo();
            loWA.Cerrar(true);

            //Alarma
            lht.Clear();
            lht.Add("{Idioma}", KDBUtil.SearchICodCatalogo("Idioma", Globals.GetLanguage(), true));
            lht.Add("{RepEst}", liCodCatRepEst);
            lht.Add("{Usuar}", Session["iCodUsuario"]);
            lht.Add("{TipoAlarma}", KDBUtil.SearchScalar("Valores", "Default", "{Value}", true));
            lht.Add("{ExtArchivo}", KDBUtil.SearchScalar("Valores", "xlsx", "{Value}", true));
            lht.Add("{HoraAlarma}", "1900-01-01 " + lsHora);
            lht.Add("{SigAct}", DateTime.Today.ToString("yyyy-MM-dd") + " " + lsHora);
            lht.Add("{CtaDe}", Util.AppSettings("appeMailID"));
            lht.Add("{NomRemitente}", "Reporte Usuario");
            lht.Add("{CtaPara}", poRepUsrDataClient.MailTo);
            lht.Add("{CtaCC}", poRepUsrDataClient.MailCC);
            lht.Add("{CtaCCO}", poRepUsrDataClient.MailBCC);
            lht.Add("{Asunto}", liCodCatAsunto);
            lht.Add("{Plantilla}", lsFile);
            lht.Add("{CtaNoValidos}", null);
            //RZ.20130502 Se reemplaza atributo CtaSoporte por el nuevo atributo en el VarChar10 de los maestros de Alarmas
            lht.Add("{DSFiltroAlarm}", null);
            lht.Add("{EstCarga}", (int)KDBUtil.SearchICodCatalogo("EstCarga", "CarEspera", true));

            if (poRepUsrDataClient.MailFreq == 1 || poRepUsrDataClient.MailFreq == 2) //Una vez y Diaria
            {
                lht.Add("{BanderasAlarmaDiaria}", 0 +
                    (int)KDBUtil.SearchScalar("Valores", "ReporteAdjuntoD", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirEmpleadosD", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirCenCosD", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirSitiosD", "{Value}", true) +
                    (poRepUsrDataClient.MailFreq == 2 && poRepUsrDataClient.MailDiasHabiles != 0 ?
                        (int)KDBUtil.SearchScalar("Valores", "DiaHabil", "{Value}", true) : 0));

                if (poRepUsrDataClient.MailFreq == 1)
                {
                    lht["{SigAct}"] = DateTime.Parse(poRepUsrDataClient.MailFechaUnaVez).ToString("yyyy-MM-dd") + " " + lsHora;
                    lht.Add("dtFinVigencia", DateTime.Today.AddDays(1));

                    ReporteEstandar.InitValoresSession();
                    lht.Add("{ParamRepEst}", "{FechaIniRep:=" + ((DateTime)Session["FechaIniRep"]).ToString("yyyy-MM-dd") + ";FechaFinRep:=" + ((DateTime)Session["FechafinRep"]).ToString("yyyy-MM-dd") + " 23:59:59}");
                }

                liCodCatAlarma = KDBUtil.SaveHistoric("Alarm", "Alarma Diaria", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.Name, lht);
            }
            else if (poRepUsrDataClient.MailFreq == 3) //Semanal
            {
                lht.Add("{BanderasAlarmaSQ}", 0 +
                    (int)KDBUtil.SearchScalar("Valores", "ReporteAdjuntoSQ", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirEmpleadosSQ", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirCenCosSQ", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirSitiosSQ", "{Value}", true));
                lht.Add("{DiaSemana}", poRepUsrDataClient.MailDiaSemana);

                liCodCatAlarma = KDBUtil.SaveHistoric("Alarm", "Alarma Semanal", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.Name, lht);
            }
            else if (poRepUsrDataClient.MailFreq == 4) //Quincenal
            {
                lht.Add("{BanderasAlarmaSQ}", 0 +
                    (int)KDBUtil.SearchScalar("Valores", "ReporteAdjuntoSQ", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirEmpleadosSQ", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirCenCosSQ", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirSitiosSQ", "{Value}", true));

                liCodCatAlarma = KDBUtil.SaveHistoric("Alarm", "Alarma Quincenal", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.Name, lht);
            }
            else if (poRepUsrDataClient.MailFreq == 5 && poRepUsrDataClient.MailDiaMes != 0) //Mensual por día del mes
            {
                lht.Add("{BanderasAlarmas}", 0 +
                    (int)KDBUtil.SearchScalar("Valores", "ReporteAdjunto", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirEmpleados", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirCenCos", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirSitios", "{Value}", true));
                lht.Add("{DiaEnvio}", poRepUsrDataClient.MailDiaMes);

                liCodCatAlarma = KDBUtil.SaveHistoric("Alarm", "Alarma Mensual por Número de Día", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.Name, lht);
            }
            else if (poRepUsrDataClient.MailFreq == 5 && poRepUsrDataClient.MailDiaMes == 0) //Mensual por semana
            {
                lht.Add("{BanderasAlarmas}", 0 +
                    (int)KDBUtil.SearchScalar("Valores", "ReporteAdjunto", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirEmpleados", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirCenCos", "{Value}", true) +
                    (int)KDBUtil.SearchScalar("Valores", "IncluirSitios", "{Value}", true));
                lht.Add("{Semana}", poRepUsrDataClient.MailSemanaMes);
                lht.Add("{DiaSemana}", poRepUsrDataClient.MailDiaSemanaMes);

                liCodCatAlarma = KDBUtil.SaveHistoric("Alarm", "Alarma Mensual por Número de Semana", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.Name, lht);
            }


            return liCodCatAlarma;
        }

        public int SaveOpcMenu(int liCodCatRepEst)
        {
            CargasCOM loCom = new CargasCOM();
            Hashtable lht = new Hashtable();

            int liCodCatAplic = -1;
            int liCodCatOpcMnu = -1;
            int liCodCatOpcMnuParent = -1;

            DataTable ldtOpc = kdb.GetRelRegByDes("ReporteUsuario - TipoAcceso - Menu",
                "{RepUsu} = " + poRepUsrDataClient.BaseReport + " " +
                "and {RepUsuAcceso} = " + poRepUsrDataClient.AccessType);

            if (ldtOpc != null && ldtOpc.Rows.Count > 0)
                liCodCatOpcMnuParent = (int)Util.IsDBNull(ldtOpc.Rows[0]["{OpcMnu}"], -1);

            if (liCodCatOpcMnuParent != -1)
            {
                //aplicación
                lht.Clear();
                lht.Add("{URL}", "~/UserInterface/Consultas/Consultas.aspx");
                liCodCatAplic = KDBUtil.SaveHistoric("Aplic", "Consultas", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.Name, lht);


                //Opcion de menú
                lht.Clear();
                lht.Add("{OpcMnu}", liCodCatOpcMnuParent);
                lht.Add("{Aplic}", liCodCatAplic);
                lht.Add("{OrdenMenu}", 9999);
                lht.Add("{Español}", poRepUsrDataClient.Name);
                lht.Add("{Ingles}", poRepUsrDataClient.Name);
                lht.Add("{Frances}", poRepUsrDataClient.Name);
                lht.Add("{Portugues}", poRepUsrDataClient.Name);
                lht.Add("{Aleman}", poRepUsrDataClient.Name);
                liCodCatOpcMnu = KDBUtil.SaveHistoric("OpcMnu", "Opciones de Menu", poRepUsrDataClient.VchCodigo, poRepUsrDataClient.Name, lht);


                //Relacion Cliente - Opcion Menu - Permiso
                lht.Clear();
                lht.Add("iCodRelacion", (int)DSODataAccess.ExecuteScalar("select iCodRegistro from relaciones where vchDescripcion = 'Cliente - Opción - Permiso' and iCodRelacion is null"));
                lht.Add("{Client}", KDBUtil.SearchScalar("Empre", (int)KDBUtil.SearchScalar("Usuar", (int)Session["iCodUsuario"], "{Empre}"), "{Client}"));
                lht.Add("{OpcMnu}", liCodCatOpcMnu);
                lht.Add("{Permiso}", KDBUtil.SearchICodCatalogo("Permiso", "Administar", true));

                DataTable ldtRelCli = kdb.GetRelRegByDes("Cliente - Opción - Permiso",
                    "{Client} = " + lht["{Client}"] + " and " +
                    "{OpcMnu} = " + lht["{OpcMnu}"] + " and " +
                    "{Permiso} = " + lht["{Permiso}"]);

                if (ldtRelCli == null || ldtRelCli.Rows.Count == 0)
                    loCom.GuardaRelacion(lht, "Cliente - Opción - Permiso", DSODataContext.GetContext());


                DataTable ldtRelPerf;
                DataTable ldtRelUsr;
                DataTable ldtRelAplicPerf;

                DataTable ldtPerf = kdb.GetHisRegByEnt("Perfil", "Perfiles", new string[] { "iCodCatalogo" });

                int liCodRelAplicPerf = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from relaciones where vchDescripcion = 'Aplicación - Estado - Perfil - Atributo - Consulta - Reporte' and iCodRelacion is null");
                int liCodRelPerf = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from relaciones where vchDescripcion = 'Perfil - Opción - Permiso' and iCodRelacion is null");
                int liCodRelUsr = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from relaciones where vchDescripcion = 'Usuario - Opción - Permiso' and iCodRelacion is null");

                if (ldtPerf != null && poRepUsrDataClient.AccessType == KDBUtil.SearchICodCatalogo("RepUsuAcceso", "publico", true))
                {
                    //Relaciones con perfiles
                    foreach (DataRow ldr in ldtPerf.Rows)
                    {
                        ldtRelAplicPerf = null;
                        ldtRelPerf = null;

                        //Relación Aplicación - Estado - Perfil - Atributo - Consulta - Reporte
                        lht.Clear();
                        lht.Add("iCodRelacion", liCodRelAplicPerf);
                        lht.Add("{Aplic}", liCodCatAplic);
                        lht.Add("{Perfil}", ldr["iCodCatalogo"]);
                        lht.Add("{RepEst}", liCodCatRepEst);

                        ldtRelAplicPerf = kdb.GetRelRegByDes("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte",
                            "{Aplic} = " + lht["{Aplic}"] + " and " +
                            "{Perfil} = " + lht["{Perfil}"]);

                        if (ldtRelAplicPerf == null || ldtRelAplicPerf.Rows.Count == 0)
                            loCom.GuardaRelacion(lht, "Aplicación - Estado - Perfil - Atributo - Consulta - Reporte", DSODataContext.GetContext());

                        //Relacion Perfil - Opción - Permiso
                        lht.Clear();
                        lht.Add("iCodRelacion", liCodRelPerf);
                        lht.Add("{Perfil}", ldr["iCodCatalogo"]);
                        lht.Add("{OpcMnu}", liCodCatOpcMnu);
                        lht.Add("{Permiso}", KDBUtil.SearchICodCatalogo("Permiso", "Administar", true));

                        ldtRelPerf = kdb.GetRelRegByDes("Perfil - Opción - Permiso",
                            "{Perfil} = " + lht["{Perfil}"] + " and " +
                            "{OpcMnu} = " + lht["{OpcMnu}"]);

                        if (ldtRelPerf == null || ldtRelPerf.Rows.Count == 0)
                            loCom.GuardaRelacion(lht, "Perfil - Opción - Permiso", DSODataContext.GetContext());
                    }

                    //Relacion Usuario - Opción - Permiso
                    ldtRelUsr = kdb.GetRelRegByDes("Usuario - Opción - Permiso", "{OpcMnu} = " + liCodCatOpcMnu);

                    if (ldtRelUsr != null)
                        foreach (DataRow ldrRel in ldtRelUsr.Rows)
                            DSODataAccess.Execute("update relaciones set dtFinVigencia = '" + DateTime.Today.ToString("yyyy-MM-dd") + "' where iCodRegistro = " + ldrRel["iCodRegistro"]);
                }
                else if (ldtPerf != null && poRepUsrDataClient.AccessType == KDBUtil.SearchICodCatalogo("RepUsuAcceso", "privado", true))
                {
                    ldtRelAplicPerf = null;

                    //Relación Aplicación - Estado - Perfil - Atributo - Consulta - Reporte
                    lht.Clear();
                    lht.Add("iCodRelacion", liCodRelAplicPerf);
                    lht.Add("{Aplic}", liCodCatAplic);
                    lht.Add("{Perfil}", Session["iCodPerfil"]);
                    lht.Add("{RepEst}", liCodCatRepEst);

                    ldtRelAplicPerf = kdb.GetRelRegByDes("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte",
                        "{Aplic} = " + lht["{Aplic}"] + " and " +
                        "{Perfil} = " + lht["{Perfil}"]);

                    if (ldtRelAplicPerf == null || ldtRelAplicPerf.Rows.Count == 0)
                        loCom.GuardaRelacion(lht, "Aplicación - Estado - Perfil - Atributo - Consulta - Reporte", DSODataContext.GetContext());

                    //Relacion Usuario - Opción - Permiso
                    lht.Clear();
                    lht.Add("iCodRelacion", liCodRelUsr);
                    lht.Add("{Usuar}", Session["iCodUsuario"]);
                    lht.Add("{OpcMnu}", liCodCatOpcMnu);
                    lht.Add("{Permiso}", KDBUtil.SearchICodCatalogo("Permiso", "Administar", true));

                    ldtRelUsr = kdb.GetRelRegByDes("Usuario - Opción - Permiso",
                        "{Usuar} = " + lht["{Usuar}"] + " and " +
                        "{OpcMnu} = " + lht["{OpcMnu}"]);

                    if (ldtRelUsr == null || ldtRelUsr.Rows.Count == 0)
                        loCom.GuardaRelacion(lht, "Usuario - Opción - Permiso", DSODataContext.GetContext());

                    //Relacion Perfil - Opción - Permiso
                    ldtRelPerf = kdb.GetRelRegByDes("Perfil - Opción - Permiso", "{OpcMnu} = " + liCodCatOpcMnu);

                    if (ldtRelPerf != null)
                        foreach (DataRow ldr in ldtRelPerf.Rows)
                            DSODataAccess.Execute("update relaciones set dtFinVigencia = '" + DateTime.Today.ToString("yyyy-MM-dd") + "' where iCodRegistro = " + ldr["iCodRegistro"]);
                }

                DSODataContext.ClearSchemaCache();

                Session.Remove("OpcionesUsuario");
                DSONavegador poNav = new DSONavegador();
                poNav.LoadUserInfo();
                poNav.getOpciones();
            }

            return liCodCatOpcMnu;
        }

        public void BajaOpcMenu()
        {
            CargasCOM loCom = new CargasCOM();

            int liCodCatAplic = (int)KDBUtil.SearchICodCatalogo("Aplic", poRepUsrDataClient.VchCodigo);
            int liCodCatOpcMnu = (int)KDBUtil.SearchICodCatalogo("OpcMnu", poRepUsrDataClient.VchCodigo);

            if (liCodCatAplic != -1)
            {
                DataTable ldtRelPerf = kdb.GetRelRegByDes("Perfil - Opción - Permiso", "{OpcMnu} = " + liCodCatOpcMnu);

                if (ldtRelPerf != null)
                    foreach (DataRow ldr in ldtRelPerf.Rows)
                        DSODataAccess.Execute("update relaciones set dtFinVigencia = '" + DateTime.Today.ToString("yyyy-MM-dd") + "' where iCodRegistro = " + ldr["iCodRegistro"]);


                DataTable ldtRelUsr = kdb.GetRelRegByDes("Usuario - Opción - Permiso", "{OpcMnu} = " + liCodCatOpcMnu);

                if (ldtRelUsr != null)
                    foreach (DataRow ldr in ldtRelUsr.Rows)
                        DSODataAccess.Execute("update relaciones set dtFinVigencia = '" + DateTime.Today.ToString("yyyy-MM-dd") + "' where iCodRegistro = " + ldr["iCodRegistro"]);


                DataTable ldtRelCli = kdb.GetRelRegByDes("Cliente - Opción - Permiso", "{OpcMnu} = " + liCodCatOpcMnu);

                if (ldtRelCli != null)
                    foreach (DataRow ldr in ldtRelCli.Rows)
                        DSODataAccess.Execute("update relaciones set dtFinVigencia = '" + DateTime.Today.ToString("yyyy-MM-dd") + "' where iCodRegistro = " + ldr["iCodRegistro"]);

                loCom.EliminarRegistro("historicos", KDBUtil.SearchICodRegistro("OpcMnu", poRepUsrDataClient.VchCodigo), DSODataContext.GetContext());
            }

            if (liCodCatAplic != -1)
            {
                DataTable ldtRelAplicPerf = kdb.GetRelRegByDes("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte", "{Aplic} = " + liCodCatAplic);

                if (ldtRelAplicPerf != null)
                    foreach (DataRow ldr in ldtRelAplicPerf.Rows)
                        DSODataAccess.Execute("update relaciones set dtFinVigencia = '" + DateTime.Today.ToString("yyyy-MM-dd") + "' where iCodRegistro = " + ldr["iCodRegistro"]);


                loCom.EliminarRegistro("historicos", KDBUtil.SearchICodRegistro("Aplic", poRepUsrDataClient.VchCodigo), DSODataContext.GetContext());
            }

            DSODataContext.ClearSchemaCache();

            Session.Remove("OpcionesUsuario");
            DSONavegador poNav = new DSONavegador();
            poNav.LoadUserInfo();
            poNav.getOpciones();
        }

        public string ConvertCriteria(ReporteUsuarioCriteria loCriteria, bool lbAjustarValores)
        {
            string lsOperator = (string)Util.IsDBNull(KDBUtil.SearchScalar("RepUsuOper", loCriteria.Operator, "vchCodigo", true), ""); ;
            StringBuilder lsbRet = new StringBuilder();
            string lsDlm = (lbAjustarValores ? "''" : "'");

            if (loCriteria.Field.DataField.Contains("="))
                lsbRet.Append(loCriteria.Field.DataField.Split('=')[1].Trim() + " ");
            else
                lsbRet.Append(loCriteria.Field.DataField + " ");


            if (lsOperator == "contains")
                lsbRet.Append("like " + lsDlm + "%" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "nocontains")
                lsbRet.Append("not like " + lsDlm + "%" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "starts")
                lsbRet.Append("like " + lsDlm + "" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "nostarts")
                lsbRet.Append("not like " + lsDlm + "" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "ends")
                lsbRet.Append("like " + lsDlm + "%" + loCriteria.Value + lsDlm);
            else if (lsOperator == "noends")
                lsbRet.Append("not like " + lsDlm + "%" + loCriteria.Value + lsDlm);
            else
                lsbRet.Append(lsOperator + " " +
                    (loCriteria.Field.Type == "VChar" ? lsDlm : "") +
                    loCriteria.Value.Replace("'", "''") +
                    (loCriteria.Field.Type == "VChar" ? lsDlm : "") + "");

            return lsbRet.ToString();
        }

        public string ConvertCriteriaGrp(ReporteUsuarioCriteria loCriteria, bool lbAjustarValores, string lsTipoReporte)
        {
            string lsOperator = (string)Util.IsDBNull(KDBUtil.SearchScalar("RepUsuOper", loCriteria.Operator, "vchCodigo", true), ""); ;
            StringBuilder lsbRet = new StringBuilder();
            string lsDlm = (lbAjustarValores ? "''" : "'");

            if (((lsTipoReporte == "RepUsuTpTab" || lsTipoReporte == "RepUsuTpRes") && loCriteria.Field.Group != -1) ||
                (lsTipoReporte == "RepUsuTpMat" && (loCriteria.Field.GroupMatX != -1 || loCriteria.Field.GroupMatY != -1))) //Campo por el que se agrupa
                lsbRet.Append(loCriteria.Field.DataField);
            else if (loCriteria.Field.AggregateFn != "")
            {
                if (loCriteria.Field.DataField.Contains("="))
                    lsbRet.Append(loCriteria.Field.AggregateFn + "(" + loCriteria.Field.DataField.Split('=')[1].Trim() + ")");
                else
                    lsbRet.Append(loCriteria.Field.AggregateFn + "(" + loCriteria.Field.DataField + ")");
            }
            else
            {
                if (loCriteria.Field.DataField.Contains("="))
                    lsbRet.Append("min(" + loCriteria.Field.DataField.Split('=')[1].Trim() + ")");
                else
                    lsbRet.Append("min(" + loCriteria.Field.DataField + ")");
            }


            if (lsOperator == "contains")
                lsbRet.Append("like " + lsDlm + "%" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "nocontains")
                lsbRet.Append("not like " + lsDlm + "%" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "starts")
                lsbRet.Append("like " + lsDlm + "" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "nostarts")
                lsbRet.Append("not like " + lsDlm + "" + loCriteria.Value + "%" + lsDlm);
            else if (lsOperator == "ends")
                lsbRet.Append("like " + lsDlm + "%" + loCriteria.Value + lsDlm);
            else if (lsOperator == "noends")
                lsbRet.Append("not like " + lsDlm + "%" + loCriteria.Value + lsDlm);
            else
                lsbRet.Append(lsOperator + " " +
                    (loCriteria.Field.Type == "VChar" ? lsDlm : "") +
                    loCriteria.Value.Replace("'", "''") +
                    (loCriteria.Field.Type == "VChar" ? lsDlm : "") + "");

            return lsbRet.ToString();
        }

        public static string GetLangItem(string lsItem)
        {
            return Globals.GetLangItem("MsgWeb", "Mensajes Reporte Usuario", lsItem);
        }

        public static string GetLangItem(string lsItem, params string[] lsParam)
        {
            return Globals.GetLangItem("MsgWeb", "Mensajes Reporte Usuario", lsItem, lsParam);
        }

        public void Alert(string lsMsg)
        {
            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteUsuario), "ReporteUsuarioMsg-" + Guid.NewGuid().ToString(),
                "<script language=\"javascript\">" +
                "$(document).ready(function() { jAlert('" + lsMsg + "'); });" +
                "</script>\r\n",
                true, false);
        }

        #endregion
    }

}
