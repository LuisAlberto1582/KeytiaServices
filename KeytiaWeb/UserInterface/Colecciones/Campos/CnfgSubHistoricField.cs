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
    public class CnfgSubHistoricField : KeytiaBaseField, IKeytiaFillableField
    {
        protected DSOExpandable pExpGrid;
        protected DSOGrid pSubHisGrid;
        protected HtmlButton pbtnAgregar;
        protected HistoricFieldCollection pFields;

        public CnfgSubHistoricField() { }

        public override object DataValue
        {
            get
            {
                return base.DataValue;
            }
            set
            {
                pSubHisGrid.AddClientEvent("iCodCatalogo", value.ToString());
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
                return pSubHisGrid;
            }
        }

        public override void EnableField()
        {
            base.EnableField();
            pbtnAgregar.Visible = ValidarPermiso(Permiso.Agregar);
            pSubHisGrid.ClearEditedData();

            pSubHisGrid.AddClientEvent("EnableField", "1");
        }

        public override void DisableField()
        {
            //base.DisableField();
            pbtnAgregar.Visible = ValidarPermiso(Permiso.Agregar);
            pSubHisGrid.ClearEditedData();

            pSubHisGrid.AddClientEvent("EnableField", "0");
        }

        public override void CreateField()
        {
            pExpGrid = new DSOExpandable();
            pSubHisGrid = new DSOGrid();
            pbtnAgregar = new HtmlButton();
            pjsObj = pContainer.ID;

            pFields = new HistoricFieldCollection(piCodEntidad, pConfigValue);
            InitDSOControlDB();
            InitWrapper();
            InitGrid();
        }

        protected override void InitDSOControlDB()
        {
            base.InitDSOControlDB();
            pSubHisGrid.Table = null;
        }

        protected void InitWrapper()
        {
            pExpGrid.ID = "wrapper" + pConfigName.Split(' ')[0];
            pExpGrid.StartOpen = true;
            pExpGrid.CreateControls();
            pSubHisGrid.Wrapper = pExpGrid;

            pExpGrid.Panel.Controls.Add(pbtnAgregar);
            pbtnAgregar.ID = "btnAgregar" + pConfigName.Split(' ')[0];
            pbtnAgregar.Attributes["class"] = "buttonAdd";
            pbtnAgregar.ServerClick += new EventHandler(pbtnAgregar_ServerClick);
        }

        protected virtual void pbtnAgregar_ServerClick(object sender, EventArgs e)
        {
            HistoricEdit lHistorico = (HistoricEdit)pContainer;
            HistoricEdit lSubHistorico;

            lHistorico.PrevState = lHistorico.State;
            lHistorico.SubHistoricClass = this.SubHistoricClass;
            lHistorico.SubCollectionClass = this.SubCollectionClass;
            lHistorico.InitSubHistorico(pContainer.ID + this.Column + "SubHis");

            lSubHistorico = lHistorico.SubHistorico;
            lSubHistorico.SetEntidad((DSODataAccess.ExecuteScalar("select vchcodigo from Catalogos where icodregistro = " + piCodEntidad)).ToString());
            lSubHistorico.SetMaestro(pConfigName);

            DataTable lKDBTable = pKDB.GetHisRegByEnt("MsgWeb", "Mensajes Web", " vchDescripcion = '" + lSubHistorico.vchDesMaestro + "'");
            if (lKDBTable.Rows.Count > 0)
            {
                lSubHistorico.vchCodTitle = lKDBTable.Rows[0]["vchCodigo"].ToString();
            }

            lSubHistorico.EsSubHistorico = true;
            lSubHistorico.FillControls();

            lHistorico.SetHistoricState(HistoricState.CnfgSubHistoricField);
            lSubHistorico.InitMaestro();

            lSubHistorico.Fields.EnableFields();
            lSubHistorico.SetHistoricState(HistoricState.Edicion);
            if (lSubHistorico.Fields.ContainsConfigName(lHistorico.vchCodEntidad))
            {
                lSubHistorico.Fields.GetByConfigName(lHistorico.vchCodEntidad).DataValue = lHistorico.iCodCatalogo;
                lSubHistorico.Fields.GetByConfigName(lHistorico.vchCodEntidad).DisableField();
            }
            lSubHistorico.PostAgregarSubHistoricField();
        }

        protected virtual void InitGrid()
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
            pSubHisGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetSubHisData");
            pSubHisGrid.Config.fnInitComplete = "function(){" + pjsObj + ".fnSubHisInitComplete(this);}";

            lTarget = 0;
            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            if (ValidarPermiso(Permiso.Editar))
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "Editar";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "50px";
                lCol.sClass = "TdEdit";
                pSubHisGrid.Config.aoColumnDefs.Add(lCol);
            }

            if (ValidarPermiso(Permiso.Eliminar)) //@@rrh
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "Eliminar";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "50px";
                lCol.sClass = "TdEdit";
                pSubHisGrid.Config.aoColumnDefs.Add(lCol);
            }

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchCodigo";
            lCol.aTargets.Add(lTarget++);
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchDescripcion";
            lCol.aTargets.Add(lTarget++);
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            InitGridFields();

            lTarget = pSubHisGrid.Config.aoColumnDefs.Count;

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtIniVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFinVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFecUltAct";
            lCol.aTargets.Add(lTarget++);
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            if (pSubHisGrid.Config.aoColumnDefs.Count > 10)
            {
                pSubHisGrid.Config.sScrollXInner = (pSubHisGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
            }
        }

        protected virtual void InitGridFields()
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
                        lCol.sName = lField.Column + "Desc";
                    }
                    else
                    {
                        lCol.sName = lField.Column;
                    }
                    lCol.aTargets.Add(lTarget++);
                    pSubHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        public override void InitField()
        {
            base.InitField();
            ((HistoricEdit)pContainer).PanelSubHistoricos.Controls.Add(pSubHisGrid);

            pSubHisGrid.AddClientEvent("iCodEntidad", piCodEntidad.ToString());
            pSubHisGrid.AddClientEvent("iCodMaestro", pConfigValue.ToString());
            pSubHisGrid.AddClientEvent("buttonAdd", "#" + pbtnAgregar.ClientID);
            pExpGrid.OnOpen = "function(){" + pjsObj + ".fnInitGrid('#" + pSubHisGrid.Grid.ClientID + "');}";
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (((HistoricEdit)pContainer).iCodRegistro == "null")
            {
                pSubHisGrid.Visible = false;
            }
            else
            {
                InitGridLanguage();
            }
        }

        protected virtual void InitGridLanguage()
        {

            pExpGrid.Title = DSOControl.JScriptEncode(pDescripcion);
            pExpGrid.ToolTip = DSOControl.JScriptEncode(pDescripcion);
            pSubHisGrid.Visible = true;
            AgregarParametrosGrid();

            pSubHisGrid.Config.oLanguage = Globals.GetGridLanguage();
            pDescripcion = "";

            pbtnAgregar.InnerText = Globals.GetMsgWeb("btnAgregar");

            KeytiaBaseField lField;
            pFields.InitLanguage();

            foreach (DSOGridClientColumn lCol in pSubHisGrid.Config.aoColumnDefs)
            {
                if (pFields.Contains(lCol.sName))
                {
                    lField = pFields[lCol.sName];
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName.StartsWith("iCodCatalogo") && lCol.sName.EndsWith("Desc")
                    && pFields.Contains(lCol.sName.Remove(lCol.sName.LastIndexOf("Desc"))))
                {
                    lField = pFields[lCol.sName.Remove(lCol.sName.LastIndexOf("Desc"))];
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
                else if (lCol.sName == "Editar")
                {
                    string lsdoPostBack = GetPostBackEditar();
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnEditar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + pContainer.ResolveUrl("~/images/pencilsmall.png") + "\",\"" + lsdoPostBack + "\");}";
                }
                else if (lCol.sName == "Eliminar") //@@rrh
                {
                    string lsdoPostBack = GetPostBackEliminar();
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnBaja"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + pContainer.ResolveUrl("~/images/deletesmall.png") + "\",\"" + lsdoPostBack + "\");}";
                }
            }
        }

        protected virtual void AgregarParametrosGrid()
        {
            int lTarget;
            List<Parametro> lstParams = new List<Parametro>();
            Parametro lParam = new Parametro();
            lParam.Name = pFields.GetByConfigName(((HistoricEdit)pContainer).vchCodEntidad).Column;
            if (int.TryParse(((HistoricEdit)pContainer).iCodCatalogo, out lTarget))
            {
                lParam.Value = lTarget;
            }
            else
            {
                lParam.Value = "null";
            }
            lstParams.Add(lParam);
            pSubHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerHistoricos(this, sSource, aoData, fnCallback," + piCodEntidad + "," + pConfigValue + "," + DSOControl.SerializeJSON<List<Parametro>>(lstParams) + ");}";
        }

        protected virtual string GetPostBackEditar()
        {
            return DSOControl.JScriptEncode(pContainer.Page.ClientScript.GetPostBackEventReference(pContainer, "btnEditarSubHis:{0}:" + pConfigValue));
        }

        protected virtual string GetPostBackEliminar() //@@rrh
        {
            return DSOControl.JScriptEncode(pContainer.Page.ClientScript.GetPostBackEventReference(pContainer, "btnEliminarSubHis:{0}:" + pConfigValue));
        }

        public void Fill()
        {
            pSubHisGrid.Fill();
        }
    }

    public class CnfgSubFullHistoricField : CnfgSubHistoricField
    {
        protected override void InitGrid()
        {
            base.InitGrid();
            pSubHisGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetSubFullHisData");
        }
    }

    public class CnfgSubHistoricParamField : CnfgSubHistoricField
    {
        public override void InitLanguage()
        {
            base.InitLanguage();
            pSubHisGrid.Visible = true;
            if (((HistoricEdit)pContainer).iCodRegistro == "null")
            {
                InitGridLanguage();
            }
        }

        protected override void AgregarParametrosGrid()
        {
            pSubHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerHistoricosParam(this, sSource, aoData, fnCallback," + piCodEntidad + "," + pConfigValue + ");}";
        }
    }

    public class CnfgSubHistTarifaField : CnfgSubHistoricParamField
    {
        protected override void AgregarParametrosGrid()
        {
            List<Parametro> lstParams = new List<Parametro>();
            Parametro lParam = new Parametro();
            lParam.Name = "PlanServ";
            lParam.Value = Collection.GetByConfigName("PlanServ").DataValue;
            lstParams.Add(lParam);

            lParam = new Parametro();
            lParam.Name = "Region";
            lParam.Value = Collection.GetByConfigName("Region").DataValue;
            lstParams.Add(lParam);

            HistoricEdit lHistorico = (HistoricEdit)pContainer;
            HistoricEdit lHistoricoPadre;
            List<string> lstRegistros = new List<string>();
            lstRegistros.Add("-1");
            if (lHistorico.iCodRegistro != "null")
            {
                lstRegistros.Add(lHistorico.iCodRegistro);
            }

            while ((lHistoricoPadre = lHistorico.Historico) != null)
            {
                if (lHistoricoPadre.iCodRegistro != "null")
                {
                    lstRegistros.Add(lHistoricoPadre.iCodRegistro);
                }
                lHistorico = lHistoricoPadre;
            }
            lHistorico = (HistoricEdit)pContainer;

            lParam = new Parametro();
            lParam.Name = "NotiCodRegistro";
            lParam.Value = String.Join(",", lstRegistros.ToArray());
            lstParams.Add(lParam);

            pSubHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerHistoricos(this, sSource, aoData, fnCallback," + piCodEntidad + "," + pConfigValue + "," + DSOControl.SerializeJSON<List<Parametro>>(lstParams) + ");}";
        }
    }

    public class CnfgSubHistorialField : CnfgSubFullHistoricField
    {
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

        protected override void InitGrid()
        {
            base.InitGrid();
            pSubHisGrid.Config.aoColumnDefs[1].bVisible = false;
        }

        protected override void InitGridLanguage()
        {
            base.InitGridLanguage();
            pExpGrid.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "SubHistorialField"));
            pExpGrid.ToolTip = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "SubHistorialField"));
        }

        protected override void AgregarParametrosGrid()
        {
            List<Parametro> lstParams = new List<Parametro>();
            Parametro lParam = new Parametro();
            lParam.Name = "iCodCatalogo";
            lParam.Value = ((HistoricEdit)pContainer).iCodCatalogo;

            lstParams.Add(lParam);
            pSubHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerHistoricos(this, sSource, aoData, fnCallback," + piCodEntidad + "," + pConfigValue + "," + DSOControl.SerializeJSON<List<Parametro>>(lstParams) + ");}";

        }
    }

    public class CnfgSubPresupuestoField : CnfgSubHistoricField
    {
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
            pSubHisGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetSubHisData");
            pSubHisGrid.Config.fnInitComplete = "function(){" + pjsObj + ".fnSubHisInitComplete(this);}";

            lTarget = 0;
            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            if (ValidarPermiso(Permiso.Editar))
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "Editar";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "50px";
                lCol.sClass = "TdEdit";
                pSubHisGrid.Config.aoColumnDefs.Add(lCol);
            }

            if (ValidarPermiso(Permiso.Eliminar)) //@@rrh
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "Eliminar";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "50px";
                lCol.sClass = "TdEdit";
                pSubHisGrid.Config.aoColumnDefs.Add(lCol);
            }

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "vchCodigo";
            //lCol.aTargets.Add(lTarget++);
            //pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "vchDescripcion";
            //lCol.aTargets.Add(lTarget++);
            //pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            InitGridFields();

            lTarget = pSubHisGrid.Config.aoColumnDefs.Count;

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "dtIniVigencia";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "120px";
            //pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "dtFinVigencia";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "120px";
            //pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFecUltAct";
            lCol.aTargets.Add(lTarget++);
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

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
                if (lField.ShowInGrid && lField.ConfigName != "PrepCenCos")
                {
                    lCol = new DSOGridClientColumn();
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lCol.sName = lField.Column + "Desc";
                    }
                    else
                    {
                        lCol.sName = lField.Column;
                    }
                    lCol.aTargets.Add(lTarget++);
                    pSubHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }
    }

}
