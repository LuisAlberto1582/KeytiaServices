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
    public class CnfgParticipantesFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadAsistente;

        public CnfgParticipantesFieldCollection()
        {
        }

        public CnfgParticipantesFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgParticipantesFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadAsistente = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'AsistenteConferencia'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadAsistente + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubAsistenteField", "AsistenteConferencia", "Asistentes", "KeytiaWeb.UserInterface.CnfgAsistentes", "KeytiaWeb.UserInterface.CnfgAsistentesFieldCollection");
            //AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubHistoricField", "AsistenteConferencia", "Asistentes", "KeytiaWeb.UserInterface.CnfgAsistentes", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }

        public override void InitFields()
        {
            base.InitFields();
            KeytiaAutoCompleteField lFieldConferencia = (KeytiaAutoCompleteField)GetByConfigName("TMSConf");
            KeytiaAutoCompleteField lFieldContacto = (KeytiaAutoCompleteField)GetByConfigName("PhoneBookAddress");
            ((DSOAutocomplete)lFieldContacto.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchSeeYouOnPhoneBookContact");
            ((DSOAutocomplete)lFieldContacto.DSOControlDB).FnSearch = "function(request, response){" + lFieldContacto.JsObj + ".fnSearchPhoneBookContact(this, request, response); }";
            lFieldContacto.DSOControlDB.AddClientEvent("TMSConfId", "#" + ((DSOAutocomplete)lFieldConferencia.DSOControlDB).TextValue.ClientID);
        }

    }

    public class CnfgSubAsistenteField : CnfgSubHistoricField
    {
        public override void DisableField()
        {
            base.DisableField();
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

            if (ValidarPermiso(Permiso.Eliminar))
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "Eliminar";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "50px";
                lCol.sClass = "TdEdit";
                pSubHisGrid.Config.aoColumnDefs.Add(lCol);
            }

            InitGridFields();

            lTarget = pSubHisGrid.Config.aoColumnDefs.Count;

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

            KeytiaBaseField lField = pFields.GetByConfigName("Emple");
            lCol = new DSOGridClientColumn();
            lCol.sName = lField.Column + "Desc";
            lCol.aTargets.Add(lTarget++);
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);
        }

        protected override void InitGridLanguage()
        {
            base.InitGridLanguage();

            StringBuilder lsb = new StringBuilder();
            if (pSubHisGrid.GetClientEvent("EnableField") == "1")
            {
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){ try { $('#" + pSubHisGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "1" + "," + ValidarPermiso(Permiso.Editar).ToString().ToLower() + "); } catch(e){ } });");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(CnfgSubParticipanteField), pjsObj + "Editar", lsb.ToString(), false, false);

                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){ try{ $('#" + pSubHisGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "2" + "," + ValidarPermiso(Permiso.Eliminar).ToString().ToLower() + "); } catch(e){ } });");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(CnfgSubParticipanteField), pjsObj + "Eliminar", lsb.ToString(), false, false);
            }
            else
            {
                lsb.Length = 0;
                lsb.Append("<script type='text/javascript'>");
                lsb.Append("jQuery(function($){ try{ $('#" + pSubHisGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "1" + ",false); } catch(e){ } });");
                lsb.Append("</script>");

                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(CnfgSubParticipanteField), pjsObj + "Editar", lsb.ToString(), false, false);

                lsb.Length = 0;
                lsb.Append("<script type='text/javascript'>");
                lsb.Append("jQuery(function($){ try{ $('#" + pSubHisGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "2" + ",false); } catch(e){ } });");
                lsb.Append("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(CnfgSubParticipanteField), pjsObj + "Eliminar", lsb.ToString(), false, false);
            }
        }
    }
}
