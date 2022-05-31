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
    public class CartasCustodiaGridField : KeytiaBaseField, IKeytiaFillableField
    {
        protected DSOExpandable pExpGrid;
        protected DSOGrid pPendGrid;
        protected HistoricFieldCollection pFields;

        public CartasCustodiaGridField() {}

        public override object DataValue
        {
            get
            {
                return base.DataValue;
            }
            set
            {
                pPendGrid.AddClientEvent("iCodCatalogo", value.ToString());
            }
        }

        public override int ConfigValue
        {
            get
            {
                return pConfigValue;
            }
            set
            {
                pConfigValue = value;
                if (pConfigValue > 0)
                {
                    pConfigName = (string)DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + pConfigValue);
                }
            }
        }

        public override bool ShowInGrid
        {
            get
            {
                return false;
            }
        }

        public virtual HistoricFieldCollection Fields
        {
            get
            {
                return pFields;
            }
        }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pPendGrid;
            }
        }

        public override void EnableField()
        {
            base.EnableField();
            pPendGrid.ClearEditedData();
            pPendGrid.AddClientEvent("EnableField", "1");
        }

        public override void CreateField()
        {
            pExpGrid = new DSOExpandable();
            pPendGrid = new DSOGrid();
            pjsObj = pContainer.ID;

            pFields = new HistoricFieldCollection(piCodEntidad, pConfigValue);
            InitDSOControlDB();
            InitWrapper();
            InitGrid();
        }

        protected override void InitDSOControlDB()
        {
            base.InitDSOControlDB();
            pPendGrid.Table = null;
        }

        protected void InitWrapper()
        {
            pExpGrid.ID = "wrapper" + pConfigName.Split(' ')[0];
            pExpGrid.StartOpen = false;
            pExpGrid.CreateControls();
            pPendGrid.Wrapper = pExpGrid;
        }

        protected void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget;

            pPendGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pPendGrid.Config.bAutoWidth = true;
            pPendGrid.Config.sScrollX = "100%";
            pPendGrid.Config.sScrollY = "auto";
            pPendGrid.Config.sPaginationType = "full_numbers";
            pPendGrid.Config.bJQueryUI = true;
            pPendGrid.Config.bProcessing = true;
            pPendGrid.Config.bServerSide = true;
            if (pConfigName == "Recursos Aceptados")
            {
                pPendGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetHisRecAceptados");
            }
            else if (pConfigName == "Recursos Pendientes")
            {
                pPendGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetHisRecPend");
            }
            else
            {
                pPendGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetHisRecPendLiberar");
            }
            pPendGrid.Config.fnInitComplete = "function(){" + pjsObj + ".fnSubHisInitComplete(this);}";

            lTarget = 0;
            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pPendGrid.Config.aoColumnDefs.Add(lCol);


            if (pConfigName != "Recursos Aceptados")
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "Recurso";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "100px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "TipoRecurso";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "100px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "FechaAsignacion";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "100px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);
            }
            else
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "DescRecurso";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "100px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "Recurs";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "100px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "FechaAsignacion";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "100px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "FechaAceptacion";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "100px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);
            }
            if (pPendGrid.Config.aoColumnDefs.Count > 10)
            {
                pPendGrid.Config.sScrollXInner = (pPendGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
            }
        }

        public override void InitField()
        {
            base.InitField();
            ((HistoricEdit)pContainer).PanelSubHistoricos.Controls.Add(pPendGrid);
            pPendGrid.AddClientEvent("iCodEntidad", piCodEntidad.ToString());
            pPendGrid.AddClientEvent("iCodMaestro", pConfigValue.ToString());
            pExpGrid.OnOpen = "function(){" + pjsObj + ".fnInitGrids();}";
            pExpGrid.OnClose = "function(){" + pjsObj + ".fnInitGrids();}";
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pExpGrid.Visible = true;
            pExpGrid.Title = DSOControl.JScriptEncode(pDescripcion); ;
            pExpGrid.ToolTip = DSOControl.JScriptEncode(pDescripcion);
            pPendGrid.Visible = true;
            int lTarget;
            List<Parametro> lstParams = new List<Parametro>();
            Parametro lParam = new Parametro();
            if (int.TryParse(((HistoricEdit)pContainer).iCodCatalogo, out lTarget))
            {
                lParam.Value = lTarget;
            }
            else
            {
                lParam.Value = "null";
            }
            lstParams.Add(lParam);
            pPendGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerHistoricos(this, sSource, aoData, fnCallback," + piCodEntidad + "," + pConfigValue + "," + DSOControl.SerializeJSON<List<Parametro>>(lstParams) + ");}";

            pPendGrid.Config.oLanguage = Globals.GetGridLanguage();
            pDescripcion = "";

            pFields.InitLanguage();

            foreach (DSOGridClientColumn lCol in pPendGrid.Config.aoColumnDefs)
            {
                if (lCol.sName == "Recurso")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "Recurso"));
                }
                else if (lCol.sName == "TipoRecurso")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TipoRecurso"));
                }
                else if (lCol.sName == "FechaAsignacion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FecAsignacion"));
                }
                else if (lCol.sName == "DescRecurso")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "Recurso"));
                }
                else if (lCol.sName == "Recurs")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TipoRecurso"));
                }
                else if (lCol.sName == "FechaAceptacion")
                {
                    lCol.sTitle = lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FecAceptacion"));
                }
            }
        }

        public void Fill()
        {
            pPendGrid.Fill();
        }
    }
}
