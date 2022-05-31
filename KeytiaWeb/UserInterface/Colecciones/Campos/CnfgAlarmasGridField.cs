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
    public class CnfgAlarmasGridField : CnfgSubHistoricField
    {
        public CnfgAlarmasGridField() { }

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
            pSubHisGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetSubHisDataAlarmas");
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
            else if (ValidarPermiso(Permiso.Consultar))
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "Consultar";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "50px";
                lCol.sClass = "TdConsult";
                pSubHisGrid.Config.aoColumnDefs.Add(lCol);
            }

            if (ValidarPermiso(Permiso.Eliminar))
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

        protected override void InitGridLanguage()
        {
            base.InitGridLanguage();

            foreach (DSOGridClientColumn lCol in pSubHisGrid.Config.aoColumnDefs)
            {
                if (lCol.sName == "Consultar")
                {
                    string lsdoPostBack = GetPostBackConsultar();
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnConsultar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + pContainer.ResolveUrl("~/images/searchsmall.png") + "\",\"" + lsdoPostBack + "\");}";
                }
            }
        }

        protected virtual string GetPostBackConsultar()
        {
            return DSOControl.JScriptEncode(pContainer.Page.ClientScript.GetPostBackEventReference(pContainer, "btnConsultarSubHis:{0}:" + pConfigValue));
        }
    }
}
