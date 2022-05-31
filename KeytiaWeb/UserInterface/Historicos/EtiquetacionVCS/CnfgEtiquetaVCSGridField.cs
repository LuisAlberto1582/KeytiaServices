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

namespace KeytiaWeb.UserInterface
{
    public class CnfgEtiquetaVCSGridField : CnfgSubHistoricParamField
    {
        public CnfgEtiquetaVCSGridField() { }

        public override void EnableField()
        {
            base.EnableField();
            pbtnAgregar.Visible = false;
        }

        public override void DisableField()
        {
            base.DisableField();
            pbtnAgregar.Visible = false;
        }

        public override void CreateField()
        {
            base.CreateField();
            pbtnAgregar.Visible = false;
        }

        protected override void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget;

            pSubHisGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pSubHisGrid.Config.bAutoWidth = true;
            pSubHisGrid.Config.sScrollX = "100%";
            pSubHisGrid.Config.sPaginationType = "full_numbers";
            pSubHisGrid.Config.bJQueryUI = true;
            pSubHisGrid.Config.bProcessing = true;
            pSubHisGrid.Config.bServerSide = true;
            pSubHisGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetSubHisDataEtiquetaVCS");
            pSubHisGrid.Config.fnInitComplete = "function(){" + pjsObj + ".fnSubHisInitComplete(this);}";

            lTarget = 0;
            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "Editar";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "50px";
            lCol.sClass = "TdEdit";
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            InitGridFields();

            if (pSubHisGrid.Config.aoColumnDefs.Count > 10)
            {
                pSubHisGrid.Config.sScrollXInner = (pSubHisGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
            }
        }
       
        protected override void InitGridFields()
        {
            DSOGridClientColumn lCol;
            int lTarget = pSubHisGrid.Config.aoColumnDefs.Count;

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
                    pSubHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected override void InitGridLanguage()
        {
            base.InitGridLanguage();

            KeytiaBaseField lField;
            foreach (DSOGridClientColumn lCol in pSubHisGrid.Config.aoColumnDefs)
            {
                if (pFields.ContainsConfigName(lCol.sName))
                {
                    lField = pFields.GetByConfigName(lCol.sName);
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName.EndsWith("Desc")
                    && pFields.ContainsConfigName(lCol.sName.Remove(lCol.sName.LastIndexOf("Desc"))))
                {
                    lField = pFields.GetByConfigName(lCol.sName.Remove(lCol.sName.LastIndexOf("Desc")));
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
            }
        }
        
        protected override void AgregarParametrosGrid()
        {
            List<Parametro> lstParams = new List<Parametro>();
            Parametro lParam;
            HistoricFieldCollection lFields = ((HistoricEdit)pContainer).Fields;
            if (lFields.ContainsConfigName("TMSSystems"))
            {
                lParam = new Parametro();
                lParam.Name = "TMSSystems";
                lParam.Value = "#" + ((DSOControl)lFields.GetByConfigName("TMSSystems").DSOControlDB).ClientID + "_txt";
                lstParams.Add(lParam);
            }
            if (lFields.ContainsConfigName("FechaInicio"))
            {
                lParam = new Parametro();
                lParam.Name = "FechaInicio";
                lParam.Value = "#" + ((DSOControl)lFields.GetByConfigName("FechaInicio").DSOControlDB).ClientID + "_txt";
                lstParams.Add(lParam);
            }
            if (lFields.ContainsConfigName("FechaFin"))
            {
                lParam = new Parametro();
                lParam.Name = "FechaFin";
                lParam.Value = "#" + ((DSOControl)lFields.GetByConfigName("FechaFin").DSOControlDB).ClientID + "_txt";
                lstParams.Add(lParam);
            }
            pSubHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerHistoricosVCS(this, sSource, aoData, fnCallback," + piCodEntidad + "," + pConfigValue + "," + DSOControl.SerializeJSON<List<Parametro>>(lstParams) + ");}";
        }

    }
}
