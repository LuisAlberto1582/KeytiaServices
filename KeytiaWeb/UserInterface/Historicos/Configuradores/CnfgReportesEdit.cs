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
using System.Text.RegularExpressions;

namespace KeytiaWeb.UserInterface
{
    public class CnfgReportesEdit : HistoricEdit
    {
        public CnfgReportesEdit()
        {
            Init += new EventHandler(CnfgReportesEdit_Init);
        }

        void CnfgReportesEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgReportesEdit";
        }

        protected override void InitGrid()
        {
            base.InitGrid();
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetVisHistoricoParam");

            List<Parametro> lstParams = new List<Parametro>();
            Parametro lParam = new Parametro();
            lParam.Name = "Usuar";
            lParam.Value = "is null";
            lstParams.Add(lParam);

            pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerVisHistoricoParam(this, sSource, aoData, fnCallback," + DSOControl.SerializeJSON<List<Parametro>>(lstParams) + ");}";

        }

        protected override void InitGridFields()
        {
            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lCol = new DSOGridClientColumn();
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lCol.sName = lField.ConfigName + "Desc";
                    }
                    else
                    {
                        lCol.sName = lField.ConfigName;
                    }
                    lCol.aTargets.Add(lTarget++);
                    pHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                AgregarBoton("DataSourceRep");
                AgregarBoton("DataSourceRepMat");
                AgregarBoton("RepEstTitGr");
                AgregarBoton("RepEstTitGrHis");
                AgregarBoton("RepEstTitEjeX");
                AgregarBoton("RepEstTitEjeY");
                AgregarBoton("RepEstTitEjeXHis");
                AgregarBoton("RepEstTitEjeYHis");
                AgregarBoton("RepEstIdiomaCmp");
                AgregarBoton("DataSourceParam");

                if (pFields.ContainsConfigName("Controles") && pFields.ContainsConfigName("DataSourceParam"))
                {
                    int liValVisible = (int)DSODataAccess.ExecuteScalar("Select iCodCatalogo from [VisHistoricos('Controles','Controles','Español')] where vchCodigo = 'AutoCompleteFiltered' and dtIniVigencia <> dtFinVigencia", (object)0);
                    DSOAutocomplete lAuto = (DSOAutocomplete)((KeytiaAutoCompleteField)pFields.GetByConfigName("Controles")).DSOControlDB;

                    lAuto.OnSelect = "function(dateText, inst) {AutoComplete.mostrarRenglon.call(this, $('#" + pFields.GetByConfigName("DataSourceParam").DSOControlDB.ClientID + "_srch'), " + liValVisible + ");}";
                }

                if (pFields.ContainsConfigName("Usuar"))
                {
                    pTablaAtributos.Rows[pFields.GetByConfigName("Usuar").DSOControlDB.Row - 1].Visible = false;
                }
            }
        }

        protected virtual void OcultaRenglon(string lsConfigName)
        {
            if (pFields != null && pFields.ContainsConfigName(lsConfigName))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName(lsConfigName).Row - 1].Style["display"] = "none";
            }
        }

        protected override void AgregarBoton(string lConfigName)
        {
            AgregarBoton(lConfigName, "KeytiaWeb.UserInterface.CnfgReportesEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
        }

        protected override bool ValidarClaves()
        {
            bool lbRet = base.ValidarClaves();
            if (lbRet)
            {
                if (pvchCodigo.HasValue && Regex.IsMatch(pvchCodigo.DataValue.ToString(), "^[a-zA-ZñÑ]\\S[0-9a-zA-ZñÑ_]*$"))
                {
                    lbRet = false;
                    string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloHistoricos"));
                    StringBuilder lsbError = new StringBuilder();
                    lsbError.Append("<li><ul>");
                    lsbError.Append(Globals.GetLangItem("MsgWeb", "Mensajes Web", "VchCodigoReporteInvalido"));
                    lsbError.Append("</ul></li>");
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsbError.ToString(), lsTitulo);
                }
            }
            return lbRet;
        }

        protected override void InitHisGridLanguage()
        {
            KeytiaBaseField lField;
            DSOControlDB lFiltro;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                lField = null;
                if (pFields.ContainsConfigName(lCol.sName))
                {
                    lField = pFields.GetByConfigName(lCol.sName);
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName.EndsWith("Desc") && pFields.ContainsConfigName(lCol.sName.Substring(0, lCol.sName.Length - 4)))
                {
                    lField = pFields.GetByConfigName(lCol.sName.Substring(0, lCol.sName.Length - 4));
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName == "vchCodigo")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchCodigo"));
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcion"));
                }
                else if (lCol.sName == "dtIniVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtIniVigencia"));
                }
                else if (lCol.sName == "dtFinVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFinVigencia"));
                }
                else if (lCol.sName == "dtFecUltAct")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFecUltAct"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "Consultar")
                {
                    string lsdoPostBack = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "btnConsultar:{0}"));
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnConsultar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + ResolveUrl("~/images/searchsmall.png") + "\",\"" + lsdoPostBack + "\");}";
                }
                if (phtFiltros.ContainsKey(lCol.sName))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lCol.sName];
                    lFiltro.Descripcion = lCol.sTitle;
                }
                else if (lField != null && phtFiltros.ContainsKey(lField.Column))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lField.Column];
                    lFiltro.Descripcion = lCol.sTitle;
                }
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields.ContainsConfigName("Controles") && pFields.ContainsConfigName("DataSourceParam"))
            {
                int liValVisible = (int)DSODataAccess.ExecuteScalar("Select iCodCatalogo from [VisHistoricos('Controles','Controles','Español')] where vchCodigo = 'AutoCompleteFiltered' and dtIniVigencia <> dtFinVigencia", (object)0);

                if (pFields.GetByConfigName("Controles").DataValue.ToString() != liValVisible.ToString())
                {
                    OcultaRenglon("DataSourceParam");
                }
            }

        }
    }
}