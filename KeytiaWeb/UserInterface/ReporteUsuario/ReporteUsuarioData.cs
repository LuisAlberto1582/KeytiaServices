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
    [DataContract]
    public class ReporteUsuarioData
    {
        private int piId = -1;
        private bool pbModified = false;

        private string psTextBoxDataId = "";
        private string psTextBoxNameId = "";

        private List<ReporteUsuarioField> plstFields = new List<ReporteUsuarioField>();
        private List<ReporteUsuarioCategory> plstCategories = new List<ReporteUsuarioCategory>();
        private List<ReporteUsuarioCriteria> plstCriterias = new List<ReporteUsuarioCriteria>();
        private List<ReporteUsuarioCriteria> plstCriteriasGrp = new List<ReporteUsuarioCriteria>();

        private string psAction = "";

        private string psStartTableId = "";
        private string psFieldTableId = "";
        private string psSummaryTableId = "";
        private string psFieldOrderTableId = "";
        private string psDataOrderTableId = "";
        private string psCriteriaTableId = "";
        private string psCriteriaGrpTableId = "";
        private string psGroupTableId = "";
        private string psGroupMatTableId = "";
        private string psGraphTableId = "";
        private string psMailTableId = "";

        private string psAccessTypeRblId = "";
        private string psSaveButtonId = "";
        private string psDeleteButtonId = "";
        private string psGenerateButtonId = "";

        private string psLastCriteriaRow = "";
        private string psLastCriteriaGrpRow = "";
        private string psVchCodigo = "";
        private string psVchCodigoOpen = "";
        private string psConditions = "";
        private string psConditionsGrp = "";
        private string psDescription = "";
        private string psName = "";
        private string psExportExt = "";

        private int piBaseReport = -1;
        private int piGen = 0;
        private int piAccessType = -1;
        private int piReportType = -1;

        private int piGraphType = -1;
        private int piGraphX = -1;
        private int piGraphY = -1;

        private int piCRU = -1;
        private int piCU = -1;

        private int piShowGraph = 0;
        private int piShowNumRecs = 0;
        private int piTopDir = 0;
        private int piTopN = 0;

        private int piMailFreq = 0;
        private string psMailFechaUnaVez;
        private int piMailDiasHabiles = 0;
        private int piMailDiaSemana = 0;
        private int piMailDiaMes = 0;
        private int piMailSemanaMes = 0;
        private int piMailDiaSemanaMes = 0;

        private string psMailTo = "";
        private string psMailCC = "";
        private string psMailBCC = "";
        private string psMailSubject = "";
        private string psMailMessage = "";

        [DataMember(Name = "id")]
        public int Id
        {
            get { return piId; }
            set { piId = value; }
        }

        [DataMember(Name = "vchCodigo")]
        public string VchCodigo
        {
            get { return psVchCodigo; }
            set { psVchCodigo = value; }
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get { return psName; }
            set { psName = value; }
        }

        [DataMember(Name = "exportExt")]
        public string ExportExt
        {
            get { return psExportExt; }
            set { psExportExt = value; }
        }

        [DataMember(Name = "action")]
        public string Action
        {
            get { return psAction; }
            set { psAction = value; }
        }

        [DataMember(Name = "baseReport")]
        public int BaseReport
        {
            get { return piBaseReport; }
            set { piBaseReport = value; }
        }

        [DataMember(Name = "vchCodigoOpen")]
        public string VchCodigoOpen
        {
            get { return psVchCodigoOpen; }
            set { psVchCodigoOpen = value; }
        }

        [DataMember(Name = "modified")]
        public bool Modified
        {
            get { return pbModified; }
            set { pbModified = value; }
        }

        [DataMember(Name = "showGraph")]
        public int ShowGraph
        {
            get { return piShowGraph; }
            set { piShowGraph = value; }
        }

        [DataMember(Name = "topDir")]
        public int TopDir
        {
            get { return piTopDir; }
            set { piTopDir = value; }
        }

        [DataMember(Name = "topN")]
        public int TopN
        {
            get { return piTopN; }
            set { piTopN = value; }
        }

        [DataMember(Name = "showNumRecs")]
        public int ShowNumRecs
        {
            get { return piShowNumRecs; }
            set { piShowNumRecs = value; }
        }

        [DataMember(Name = "fields")]
        public ReporteUsuarioField[] Fields
        {
            get { return plstFields.ToArray(); }
            set
            {
                if (plstFields == null)
                    plstFields = new List<ReporteUsuarioField>();

                plstFields.Clear();

                foreach (ReporteUsuarioField loFld in value)
                    plstFields.Add(loFld);

                BindLists();
            }
        }

        [DataMember(Name = "categories")]
        public ReporteUsuarioCategory[] Categories
        {
            get { return plstCategories.ToArray(); }
            set
            {
                if (plstCategories == null)
                    plstCategories = new List<ReporteUsuarioCategory>();

                plstCategories.Clear();

                foreach (ReporteUsuarioCategory loCat in value)
                    plstCategories.Add(loCat);

                BindLists();
            }
        }

        [DataMember(Name = "criterias")]
        public ReporteUsuarioCriteria[] Criterias
        {
            get { return plstCriterias.ToArray(); }
            set
            {
                if (plstCriterias == null)
                    plstCriterias = new List<ReporteUsuarioCriteria>();

                plstCriterias.Clear();

                foreach (ReporteUsuarioCriteria loCrit in value)
                    plstCriterias.Add(loCrit);

                BindLists();
            }
        }

        [DataMember(Name = "criteriasGrp")]
        public ReporteUsuarioCriteria[] CriteriasGrp
        {
            get { return plstCriteriasGrp.ToArray(); }
            set
            {
                if (plstCriteriasGrp == null)
                    plstCriteriasGrp = new List<ReporteUsuarioCriteria>();

                plstCriteriasGrp.Clear();

                foreach (ReporteUsuarioCriteria loCrit in value)
                    plstCriteriasGrp.Add(loCrit);

                BindLists();
            }
        }

        [DataMember(Name = "startTableId")]
        public string StartTableId
        {
            get { return psStartTableId; }
            set { psStartTableId = value; }
        }

        [DataMember(Name = "fieldTableId")]
        public string FieldTableId
        {
            get { return psFieldTableId; }
            set { psFieldTableId = value; }
        }

        [DataMember(Name = "dataOrderTableId")]
        public string DataOrderTableId
        {
            get { return psDataOrderTableId; }
            set { psDataOrderTableId = value; }
        }

        [DataMember(Name = "summaryTableId")]
        public string SummaryTableId
        {
            get { return psSummaryTableId; }
            set { psSummaryTableId = value; }
        }

        [DataMember(Name = "criteriaTableId")]
        public string CriteriaTableId
        {
            get { return psCriteriaTableId; }
            set { psCriteriaTableId = value; }
        }

        [DataMember(Name = "criteriaGrpTableId")]
        public string CriteriaGrpTableId
        {
            get { return psCriteriaGrpTableId; }
            set { psCriteriaGrpTableId = value; }
        }

        [DataMember(Name = "fieldOrderTableId")]
        public string FieldOrderTableId
        {
            get { return psFieldOrderTableId; }
            set { psFieldOrderTableId = value; }
        }

        [DataMember(Name = "groupTableId")]
        public string GroupTableId
        {
            get { return psGroupTableId; }
            set { psGroupTableId = value; }
        }

        [DataMember(Name = "groupMatTableId")]
        public string GroupMatTableId
        {
            get { return psGroupMatTableId; }
            set { psGroupMatTableId = value; }
        }

        [DataMember(Name = "graphTableId")]
        public string GraphTableId
        {
            get { return psGraphTableId; }
            set { psGraphTableId = value; }
        }

        [DataMember(Name = "mailTableId")]
        public string MailTableId
        {
            get { return psMailTableId; }
            set { psMailTableId = value; }
        }

        [DataMember(Name = "accessTypeRblId")]
        public string AccessTypeRblId
        {
            get { return psAccessTypeRblId; }
            set { psAccessTypeRblId = value; }
        }

        [DataMember(Name = "saveButtonId")]
        public string SaveButtonId
        {
            get { return psSaveButtonId; }
            set { psSaveButtonId = value; }
        }

        [DataMember(Name = "deleteButtonId")]
        public string DeleteButtonId
        {
            get { return psDeleteButtonId; }
            set { psDeleteButtonId = value; }
        }

        [DataMember(Name = "generateButtonId")]
        public string GenerateButtonId
        {
            get { return psGenerateButtonId; }
            set { psGenerateButtonId = value; }
        }

        [DataMember(Name = "lastCriteriaRow")]
        public string LastCriteriaRow
        {
            get { return psLastCriteriaRow; }
            set { psLastCriteriaRow = value; }
        }

        [DataMember(Name = "lastCriteriaGrpRow")]
        public string LastCriteriaGrpRow
        {
            get { return psLastCriteriaGrpRow; }
            set { psLastCriteriaGrpRow = value; }
        }

        [DataMember(Name = "textBoxDataId")]
        public string TextBoxDataId
        {
            get { return psTextBoxDataId; }
            set { psTextBoxDataId = value; }
        }

        [DataMember(Name = "textBoxNameId")]
        public string TextBoxNameId
        {
            get { return psTextBoxNameId; }
            set { psTextBoxNameId = value; }
        }

        [DataMember(Name = "gen")]
        public int Generated
        {
            get { return piGen; }
            set { piGen = value; }
        }

        [DataMember(Name = "accessType")]
        public int AccessType
        {
            get { return piAccessType; }
            set { piAccessType = value; }
        }

        [DataMember(Name = "conditions")]
        public string Conditions
        {
            get { return psConditions; }
            set { psConditions = value; }
        }

        [DataMember(Name = "conditionsGrp")]
        public string ConditionsGrp
        {
            get { return psConditionsGrp; }
            set { psConditionsGrp = value; }
        }

        [DataMember(Name = "description")]
        public string Description
        {
            get { return psDescription; }
            set { psDescription = value; }
        }

        [DataMember(Name = "reportType")]
        public int ReportType
        {
            get { return piReportType; }
            set { piReportType = value; }
        }

        [DataMember(Name = "graphType")]
        public int GraphType
        {
            get { return piGraphType; }
            set { piGraphType = value; }
        }

        [DataMember(Name = "graphX")]
        public int GraphX
        {
            get { return piGraphX; }
            set { piGraphX = value; }
        }

        [DataMember(Name = "graphY")]
        public int GraphY
        {
            get { return piGraphY; }
            set { piGraphY = value; }
        }

        [DataMember(Name = "cru")]
        public int CreatorUser
        {
            get { return piCRU; }
            set { piCRU = value; }
        }

        [DataMember(Name = "cu")]
        public int CurrentUser
        {
            get { return piCU; }
            set { piCU = value; }
        }

        //Correo
        [DataMember(Name = "mailFreq")]
        public int MailFreq
        {
            get { return piMailFreq; }
            set { piMailFreq = value; }
        }

        [DataMember(Name = "mailFechaUnaVez")]
        public string MailFechaUnaVez
        {
            get { return psMailFechaUnaVez; }
            set { psMailFechaUnaVez = value; }
        }

        [DataMember(Name = "mailDiasHabiles")]
        public int MailDiasHabiles
        {
            get { return piMailDiasHabiles; }
            set { piMailDiasHabiles = value; }
        }

        [DataMember(Name = "mailDiaSemana")]
        public int MailDiaSemana
        {
            get { return piMailDiaSemana; }
            set { piMailDiaSemana = value; }
        }

        [DataMember(Name = "mailDiaMes")]
        public int MailDiaMes
        {
            get { return piMailDiaMes; }
            set { piMailDiaMes = value; }
        }

        [DataMember(Name = "mailSemanaMes")]
        public int MailSemanaMes
        {
            get { return piMailSemanaMes; }
            set { piMailSemanaMes = value; }
        }

        [DataMember(Name = "mailDiaSemanaMes")]
        public int MailDiaSemanaMes
        {
            get { return piMailDiaSemanaMes; }
            set { piMailDiaSemanaMes = value; }
        }



        [DataMember(Name = "mailTo")]
        public string MailTo
        {
            get { return psMailTo; }
            set { psMailTo = value; }
        }

        [DataMember(Name = "mailCC")]
        public string MailCC
        {
            get { return psMailCC; }
            set { psMailCC = value; }
        }

        [DataMember(Name = "mailBCC")]
        public string MailBCC
        {
            get { return psMailBCC; }
            set { psMailBCC = value; }
        }

        [DataMember(Name = "mailSubject")]
        public string MailSubject
        {
            get { return psMailSubject; }
            set { psMailSubject = value; }
        }

        [DataMember(Name = "mailMessage")]
        public string MailMessage
        {
            get { return psMailMessage; }
            set { psMailMessage = value; }
        }

        public List<ReporteUsuarioField> FieldsList
        {
            get { return plstFields; }
            set { plstFields = value; }
        }

        public List<ReporteUsuarioCategory> CategoriesList
        {
            get { return plstCategories; }
            set { plstCategories = value; }
        }

        public List<ReporteUsuarioCriteria> CriteriasList
        {
            get { return plstCriterias; }
            set { plstCriterias = value; }
        }

        public List<ReporteUsuarioCriteria> CriteriasGrpList
        {
            get { return plstCriteriasGrp; }
            set { plstCriteriasGrp = value; }
        }

        public void Clear()
        {
            plstFields.Clear();
            plstCategories.Clear();
        }

        public void BindLists()
        {
            if (plstFields != null && plstCategories != null)
                foreach (ReporteUsuarioField loFld in plstFields)
                    if (loFld.Category == null)
                        foreach (ReporteUsuarioCategory loCat in plstCategories)
                            if (loFld.CategoryId == loCat.Id)
                            {
                                loFld.Category = loCat;
                                break;
                            }

            if (plstFields != null && plstCriterias != null)
                foreach (ReporteUsuarioCriteria loCrit in plstCriterias)
                    if (loCrit.Field == null)
                        foreach (ReporteUsuarioField loFld in plstFields)
                            if (loCrit.FieldId == loFld.Id)
                            {
                                loCrit.Field = loFld;
                                break;
                            }

            if (plstFields != null && plstCriteriasGrp != null)
                foreach (ReporteUsuarioCriteria loCrit in plstCriteriasGrp)
                    if (loCrit.Field == null)
                        foreach (ReporteUsuarioField loFld in plstFields)
                            if (loCrit.FieldId == loFld.Id)
                            {
                                loCrit.Field = loFld;
                                break;
                            }
        }

        public void LoadBaseReport(int liCodReporte)
        {
            KDBAccess kdb = new KDBAccess();
            ReporteUsuarioCategory loCat = null;
            DataTable ldtCats = kdb.GetHisRegByRel("ReporteUsuario - Campo - Categoria", "RepUsuCatCampo", "{RepUsu} = " + liCodReporte);

            Description = (string)Util.IsDBNull(KDBUtil.SearchScalar("RepUsu", liCodReporte, "{Descripcion}"), "");
            BaseReport = liCodReporte;

            if (ldtCats != null)
            {
                foreach (DataRow ldrCat in ldtCats.Rows)
                {
                    loCat = new ReporteUsuarioCategory(
                        (int)ldrCat["iCodCatalogo"],
                        (string)ldrCat["{" + Globals.GetLanguage() + "}"],
                        "");

                    this.CategoriesList.Add(loCat);

                    //DataTable ldtFields = kdb.GetHisRegByRel("ReporteUsuario - Campo - Categoria", "RepUsuCampo", "{RepUsu} = " + liCodReporte + " and {RepUsuCatCampo} = " + ldrCat["iCodCatalogo"]);


                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" select distinct Field.iCodCatalogo, ");
                    sb.AppendLine(" 		Lang.Español as lsNombre,  ");
                    sb.AppendLine(" 		Field.DataField, ");
                    sb.AppendLine(" 		Field.TypesCod as vchCodigoType ");
                    sb.AppendLine(" 		 ");
                    sb.AppendLine(" from [visRelaciones('ReporteUsuario - Campo - Categoria','Español')] Rel ");
                    sb.AppendLine("  ");
                    sb.AppendLine(" JOIN [VisHistoricos('RepusuCampo','Campos','Español')] Field ");
                    sb.AppendLine(" 	ON Rel.RepusuCampo = Field.iCodCatalogo ");
                    sb.AppendLine(" 	and Field.dtFinvigencia>=getdate() ");
                    sb.AppendLine("  ");
                    sb.AppendLine(" JOIN [VisHistoricos('RepEstIdiomaCmp','Idioma Campos','Español')] Lang ");
                    sb.AppendLine(" 	on Lang.dtfinvigencia>=getdate() ");
                    sb.AppendLine(" 	and Lang.icodcatalogo = Field.RepEstIdiomaCmp ");
                    sb.AppendLine("  ");
                    sb.AppendLine(" where Rel.dtfinvigencia>=getdate() ");
                    sb.AppendFormat(" and Rel.RepUsu = {0} \n", liCodReporte);
                    sb.AppendFormat(" and Rel.RepUsuCatCampo = {0} ", ldrCat["iCodCatalogo"].ToString());
                    DataTable ldtFields = DSODataAccess.Execute(sb.ToString());

                    if (ldtFields != null)
                    {
                        DataView dv = ldtFields.DefaultView;
                        dv.Sort = "lsNombre";
                        ldtFields = dv.ToTable();

                        foreach (DataRow ldrField in ldtFields.Rows)
                        {
                            this.FieldsList.Add(
                                new ReporteUsuarioField(
                                    (int)ldrField["iCodCatalogo"],
                                    (string)ldrField["lsNombre"],
                                    (string)ldrField["DataField"],
                                    (string)ldrField["vchCodigoType"],
                                    loCat));
                        }
                    }
                }
            }
        }

        public void LoadUserReport()
        {
            KDBAccess kdb = new KDBAccess();
            DataTable ldt = kdb.GetHisRegByEnt("RepUsu", "Reportes Usuario",
                new string[] { "iCodCatalogo", "{RepUsu}", "{RepUsuAcceso}", "{RepUsuTpRep}", "{RepUsuUltCritId}", "{RepEst}",
                    "{RepUsuConds}", "{RepTipoGrafica}", "{RepUsuGraficaX}", "{RepUsuGraficaY}", "{Descripcion}", "iCodUsuario",
                    "{RepUsuBan2}", "{TopRegs}", "{Msg}", "vchDescripcion", "{RepUsuUltCritGrpId}", "{RepUsuCondsGrp}" },
                "iCodRegistro = " + this.Id);

            if (ldt != null && ldt.Rows.Count > 0)
            {
                this.VchCodigo = (string)ldt.Rows[0]["vchCodigo"];
                this.LastCriteriaRow = (string)Util.IsDBNull(ldt.Rows[0]["{RepUsuUltCritId}"], "");
                this.LastCriteriaGrpRow = (string)Util.IsDBNull(ldt.Rows[0]["{RepUsuUltCritGrpId}"], "");
                this.Conditions = (string)Util.IsDBNull(ldt.Rows[0]["{RepUsuConds}"], "");
                this.ConditionsGrp = (string)Util.IsDBNull(ldt.Rows[0]["{RepUsuCondsGrp}"], "");
                this.Description = (string)Util.IsDBNull(ldt.Rows[0]["{Descripcion}"], "");
                this.Name = (string)Util.IsDBNull(ldt.Rows[0]["vchDescripcion"], "");

                piAccessType = (int)Util.IsDBNull(ldt.Rows[0]["{RepUsuAcceso}"], -1);
                piReportType = (int)Util.IsDBNull(ldt.Rows[0]["{RepUsuTpRep}"], -1);

                piGraphType = (int)Util.IsDBNull(ldt.Rows[0]["{RepTipoGrafica}"], -1);
                piGraphX = (int)Util.IsDBNull(ldt.Rows[0]["{RepUsuGraficaX}"], -1);
                piGraphY = (int)Util.IsDBNull(ldt.Rows[0]["{RepUsuGraficaY}"], -1);

                piShowNumRecs = ((int)Util.IsDBNull(ldt.Rows[0]["{RepUsuBan2}"], 0) &
                    (int)KDBUtil.SearchScalar("Valores", "RepUsuBanNumRegs", "{Value}", true)) != 0 ? 1 : 0;

                piShowGraph = ((int)Util.IsDBNull(ldt.Rows[0]["{RepUsuBan2}"], 0) &
                    (int)KDBUtil.SearchScalar("Valores", "RepUsuBanAreaGrafica", "{Value}", true)) != 0 ? 1 : 0;


                piTopDir = 0;

                if (((int)Util.IsDBNull(ldt.Rows[0]["{RepUsuBan2}"], 0) &
                    (int)KDBUtil.SearchScalar("Valores", "RepUsuBanTop", "{Value}", true)) != 0)
                    piTopDir = 1;

                if (((int)Util.IsDBNull(ldt.Rows[0]["{RepUsuBan2}"], 0) &
                    (int)KDBUtil.SearchScalar("Valores", "RepUsuBanBottom", "{Value}", true)) != 0)
                    piTopDir = -1;


                piCRU = (int)Util.IsDBNull(ldt.Rows[0]["iCodUsuario"], -1);
                piTopN = (int)Util.IsDBNull(ldt.Rows[0]["{TopRegs}"], 0);
                psMailMessage = (string)Util.IsDBNull(ldt.Rows[0]["{Msg}"], "");


                //Campos
                DataTable ldtFldsSel = kdb.GetHisRegByEnt("RepUsu", "Detalle Campos",
                    new string[] { "iCodRegistro", "{RepUsuCampo}", "{RepUsuTpOrden}", "{RepUsuOrdDatos}", "{RepEstOrdenCampo}", 
                        "{RepUsuAggFn}", "{RepUsuOrdDatosGrp}", "{RepUsuTpOrdDatosGrp}", "{RepUsuOrdDatosGrpMatX}", 
                        "{RepUsuOrdDatosGrpMatY}", "{TpPeriodo}" },
                    "{RepUsu} = " + ldt.Rows[0]["iCodCatalogo"]);

                foreach (ReporteUsuarioField loFld in this.Fields)
                {
                    DataRow[] ldrs = ldtFldsSel.Select("[{RepUsuCampo}] = " + loFld.Id);

                    if (ldrs != null && ldrs.Length > 0)
                    {
                        loFld.IsSelected = true;
                        loFld.DataOrderType = (int)Util.IsDBNull(ldrs[0]["{RepUsuTpOrden}"], -1);
                        loFld.AggregateFn = (string)Util.IsDBNull(ldrs[0]["{RepUsuAggFn}"], "");

                        loFld.DataOrder = (int)Util.IsDBNull(ldrs[0]["{RepUsuOrdDatos}"], -1);
                        loFld.FieldOrder = (int)Util.IsDBNull(ldrs[0]["{RepEstOrdenCampo}"], -1);

                        loFld.Group = (int)Util.IsDBNull(ldrs[0]["{RepUsuOrdDatosGrp}"], -1);
                        loFld.GroupType = Convert.ToInt32((double)Util.IsDBNull(ldrs[0]["{RepUsuTpOrdDatosGrp}"], -1d));

                        loFld.GroupMatX = (int)Util.IsDBNull(ldrs[0]["{RepUsuOrdDatosGrpMatX}"], -1);
                        loFld.GroupMatY = (int)Util.IsDBNull(ldrs[0]["{RepUsuOrdDatosGrpMatY}"], -1);

                        loFld.TipoPeriodoGrp = int.Parse(((double)Util.IsDBNull(ldrs[0]["{TpPeriodo}"], 0d)).ToString()) & 7;
                        loFld.TipoPeriodoGrpX = (int.Parse(((double)Util.IsDBNull(ldrs[0]["{TpPeriodo}"], 0d)).ToString()) / 8) & 7;
                        loFld.TipoPeriodoGrpY = (int.Parse(((double)Util.IsDBNull(ldrs[0]["{TpPeriodo}"], 0d)).ToString()) / 64) & 7;
                    }
                }


                //Criterios
                DataTable ldtCrits = kdb.GetHisRegByEnt("RepUsu", "Detalle Criterios",
                    new string[] { "iCodRegistro", "{RepUsuFila}", "{RepUsuCampo}", "{RepUsuOper}", "{RepUsuVal}" },
                    "{RepUsu} = " + ldt.Rows[0]["iCodCatalogo"]);

                if (ldtCrits != null)
                {
                    foreach (DataRow ldrCrit in ldtCrits.Rows)
                    {
                        ReporteUsuarioCriteria loCrit = new ReporteUsuarioCriteria();

                        loCrit.Row = (string)Util.IsDBNull(ldrCrit["{RepUsuFila}"], "");
                        loCrit.FieldId = (int)Util.IsDBNull(ldrCrit["{RepUsuCampo}"], -1);
                        loCrit.Operator = (int)Util.IsDBNull(ldrCrit["{RepUsuOper}"], -1);
                        loCrit.Value = (string)Util.IsDBNull(ldrCrit["{RepUsuVal}"], "");

                        this.CriteriasList.Add(loCrit);
                    }
                }


                //Criterios Datos Agrupados
                DataTable ldtCritsGrp = kdb.GetHisRegByEnt("RepUsu", "Detalle Criterios Datos Agrupados",
                    new string[] { "iCodRegistro", "{RepUsuFila}", "{RepUsuCampo}", "{RepUsuOper}", "{RepUsuVal}" },
                    "{RepUsu} = " + ldt.Rows[0]["iCodCatalogo"]);

                if (ldtCritsGrp != null)
                {
                    foreach (DataRow ldrCrit in ldtCritsGrp.Rows)
                    {
                        ReporteUsuarioCriteria loCrit = new ReporteUsuarioCriteria();

                        loCrit.Row = (string)Util.IsDBNull(ldrCrit["{RepUsuFila}"], "");
                        loCrit.FieldId = (int)Util.IsDBNull(ldrCrit["{RepUsuCampo}"], -1);
                        loCrit.Operator = (int)Util.IsDBNull(ldrCrit["{RepUsuOper}"], -1);
                        loCrit.Value = (string)Util.IsDBNull(ldrCrit["{RepUsuVal}"], "");

                        this.CriteriasGrpList.Add(loCrit);
                    }
                }


                //Correo
                DataRow ldrAlarma = KDBUtil.SearchHistoricRow("Alarm", VchCodigo, new string[] { "iCodRegistro", "iCodMaestro" });
                DataTable ldtMae = kdb.GetMaeRegByEnt("Alarm");
                DataRow[] ldrMae;

                if (ldrAlarma != null && ldtMae != null && ldtMae.Rows.Count > 0 &&
                    (ldrMae = ldtMae.Select("iCodRegistro = " + ldrAlarma["iCodMaestro"])) != null &&
                    ldrMae.Length > 0)
                {
                    List<string> lstFields = new List<string>();
                    lstFields.Add("{Idioma}");
                    lstFields.Add("{RepEst}");
                    lstFields.Add("{Usuar}");
                    lstFields.Add("{TipoAlarma}");
                    lstFields.Add("{ExtArchivo}");
                    lstFields.Add("{HoraAlarma}");
                    lstFields.Add("{CtaDe}");
                    lstFields.Add("{NomRemitente}");
                    lstFields.Add("{CtaPara}");
                    lstFields.Add("{CtaCC}");
                    lstFields.Add("{CtaCCO}");
                    lstFields.Add("{Asunto}");
                    lstFields.Add("{Plantilla}");
                    lstFields.Add("{SigAct}");
                    lstFields.Add("dtFinVigencia");


                    if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Diaria")
                        lstFields.Add("{BanderasAlarmaDiaria}");
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Semanal")
                    {
                        lstFields.Add("{BanderasAlarmaSQ}");
                        lstFields.Add("{DiaSemana}");
                    }
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Quincenal")
                        lstFields.Add("{BanderasAlarmaSQ}");
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Mensual por Número de Día")
                    {
                        lstFields.Add("{BanderasAlarmas}");
                        lstFields.Add("{DiaEnvio}");
                    }
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Mensual por Número de Semana")
                    {
                        lstFields.Add("{BanderasAlarmas}");
                        lstFields.Add("{Semana}");
                        lstFields.Add("{DiaSemana}");
                    }

                    ldrAlarma = KDBUtil.SearchHistoricRow("Alarm", VchCodigo, lstFields.ToArray());

                    MailTo = (string)Util.IsDBNull(ldrAlarma["{CtaPara}"], "");
                    MailCC = (string)Util.IsDBNull(ldrAlarma["{CtaCC}"], "");
                    MailBCC = (string)Util.IsDBNull(ldrAlarma["{CtaCCO}"], "");
                    MailSubject = (string)Util.IsDBNull(KDBUtil.SearchScalar("Asunto", VchCodigo, "{" + Globals.GetLanguage() + "}"), "");


                    if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Diaria")
                    {
                        if (TimeSpan.FromTicks(((DateTime)ldrAlarma["dtFinVigencia"]).Ticks - DateTime.Today.Ticks).TotalDays < 2)
                        {
                            MailFreq = 1;
                            MailFechaUnaVez = ((DateTime)Util.IsDBNull(ldrAlarma["{SigAct}"], DateTime.MinValue)).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            MailFreq = 2;
                            MailDiasHabiles = ((int)Util.IsDBNull(ldrAlarma["{BanderasAlarmaDiaria}"], 0) & (int)KDBUtil.SearchScalar("Valores", "DiaHabil", "{Value}", true)) != 0 ? 1 : 0;
                        }

                    }
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Semanal")
                    {
                        MailFreq = 3;
                        MailDiaSemana = (int)Util.IsDBNull(ldrAlarma["{DiaSemana}"], 0);
                    }
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Quincenal")
                        MailFreq = 4;
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Mensual por Número de Día")
                    {
                        MailFreq = 5;
                        MailDiaMes = (int)Util.IsDBNull(ldrAlarma["{DiaEnvio}"], 0);
                        MailSemanaMes = -1;
                        MailDiaSemanaMes = -1;
                    }
                    else if ((string)ldrMae[0]["vchDescripcion"] == "Alarma Mensual por Número de Semana")
                    {
                        MailFreq = 5;
                        MailDiaMes = 0;
                        MailSemanaMes = (int)Util.IsDBNull(ldrAlarma["{Semana}"], 0);
                        MailDiaSemanaMes = (int)Util.IsDBNull(ldrAlarma["{DiaSemana}"], 9);
                    }
                }
            }

        }

        public ReporteUsuarioData Clone()
        {
            return DSOControl.DeserializeJSON<ReporteUsuarioData>(DSOControl.SerializeJSON<ReporteUsuarioData>(this));
        }
    }
}