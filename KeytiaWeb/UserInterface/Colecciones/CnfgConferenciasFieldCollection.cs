using System;
using System.Data;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public class CnfgConferenciasFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadParticipante;

        public CnfgConferenciasFieldCollection()
        {
        }

        public CnfgConferenciasFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgConferenciasFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadParticipante = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Participante'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadParticipante + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubParticipanteField", "Participante", "Participante", "KeytiaWeb.UserInterface.CnfgParticipantes", "KeytiaWeb.UserInterface.CnfgParticipantesFieldCollection");
        }

        public override void InitFields()
        {
            base.InitFields();
            KeytiaAutoCompleteField lFieldCliente = (KeytiaAutoCompleteField)GetByConfigName("Client");
            KeytiaAutoCompleteField lFieldServicio = (KeytiaAutoCompleteField)GetByConfigName("ServicioSeeYouOn");
            KeytiaAutoCompleteField lFieldProyecto = (KeytiaAutoCompleteField)GetByConfigName("Proyecto");
            ((DSOAutocomplete)lFieldServicio.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchSeeYouOnServicioConferencia");
            ((DSOAutocomplete)lFieldServicio.DSOControlDB).FnSearch = "function(request, response){" + lFieldServicio.JsObj + ".fnSearchSeeYouOnServicioConferencia(this, request, response); }";
            ((DSOAutocomplete)lFieldProyecto.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchSeeYouOnProyectoConferencia");
            ((DSOAutocomplete)lFieldProyecto.DSOControlDB).FnSearch = "function(request, response){" + lFieldServicio.JsObj + ".fnSearchSeeYouOnServicioConferencia(this, request, response); }";
            lFieldServicio.DSOControlDB.AddClientEvent("clienteId", "#" + ((DSOAutocomplete)lFieldCliente.DSOControlDB).TextValue.ClientID);
            lFieldProyecto.DSOControlDB.AddClientEvent("clienteId", "#" + ((DSOAutocomplete)lFieldCliente.DSOControlDB).TextValue.ClientID);
            StringBuilder lsbScript = new StringBuilder();
            lsbScript.AppendLine("<script type='text/javascript'>");
            lsbScript.AppendLine("jQuery(function($){");
            lsbScript.AppendLine(lFieldCliente.JsObj + ".InitValorClienteChange('#" + ((DSOAutocomplete)lFieldCliente.DSOControlDB).Search.ClientID + "', '#" + ((DSOAutocomplete)lFieldServicio.DSOControlDB).Search.ClientID + "');");
            lsbScript.AppendLine(lFieldCliente.JsObj + ".InitValorClienteChange('#" + ((DSOAutocomplete)lFieldCliente.DSOControlDB).Search.ClientID + "', '#" + ((DSOAutocomplete)lFieldProyecto.DSOControlDB).Search.ClientID + "');");
            lsbScript.AppendLine("});");
            lsbScript.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricEdit), "CnfgConferenciasFieldCollection_InitFields" + pContainer.ClientID, lsbScript.ToString(), true, false);
        }
    }

    public class CnfgSubParticipanteField : CnfgSubHistoricField
    {
        public override void DisableField()
        {
            base.DisableField();
            pbtnAgregar.Visible = false;
        }

        protected override void pbtnAgregar_ServerClick(object sender, EventArgs e)
        {
            if (ValidarAgregarParticipante())
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
                ((CnfgParticipantes)lSubHistorico).AgregarRegistroSubHistorico();
                if (lSubHistorico.Fields.ContainsConfigName(lHistorico.vchCodEntidad))
                {
                    lSubHistorico.Fields.GetByConfigName(lHistorico.vchCodEntidad).DataValue = lHistorico.iCodCatalogo;
                    lSubHistorico.Fields.GetByConfigName(lHistorico.vchCodEntidad).DisableField();
                }
                lSubHistorico.PostAgregarSubHistoricField();
            }
        }

        protected bool ValidarDisponiblidadMCU()
        {
            CnfgConferencias lCnfgConferencias = (CnfgConferencias)this.Container;
            try
            {
                if (lCnfgConferencias.Fields.ContainsConfigName("TMSSystems") 
                    && lCnfgConferencias.Fields.GetByConfigName("TMSSystems").DSOControlDB.HasValue)
                {
                    SeeYouOnCOM.SyncCOM lSyncCOM = new SeeYouOnCOM.SyncCOM();
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select NumParticipantes = count(*) from " + DSODataContext.Schema + ".[VisHistoricos('Participante','Participante','" + Globals.GetCurrentLanguage() + "')] P");
                    psbQuery.AppendLine("where P.dtIniVigencia <> P.dtFinVigencia");
                    psbQuery.AppendLine("and P.TMSConf = " + lCnfgConferencias.iCodCatalogo);

                    int liNumParticipantes = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString(), 0);
                    int liAvailablePorts = lSyncCOM.MCUAvailable(int.Parse(lCnfgConferencias.iCodCatalogo), (int)Session["iCodUsuarioDB"]);
                    if (liNumParticipantes > liAvailablePorts)
                    {
                        //Mandar llamar metodo adecuado de SeeYouOnCOM.SyncCOM para determinar la disponibilidad en el MCU
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                //Mandar llamar metodo adecuado de SeeYouOnCOM.SyncCOM para programar la conferencia en el MCU
                Util.LogException(ex);
                return false;
            }

            return true;
        }

        protected virtual bool ValidarAgregarParticipante()
        {
            CnfgConferencias lCnfgConferencias = (CnfgConferencias)this.Container;
            lCnfgConferencias.FillAjaxControls(false);

            string lsTitulo = DSOControl.JScriptEncode(lCnfgConferencias.AlertTitle);

            //Revisar si ya tengo el MCU y de ser asi mandar llamar metodo generico para validar disponibilidad
            if (!ValidarDisponiblidadMCU())
            {
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrMCUConfSave"));
                DSOControl.jAlert(lCnfgConferencias.Page, pjsObj + ".ErrGuardarConferencia", lsMsg, lsTitulo);
                return false;
            }

            //Validar que tenga fecha de inicio y fin de reservacion
            if (!Collection.GetByConfigName("FechaInicioReservacion").DSOControlDB.HasValue)
            {
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", Collection.GetByConfigName("FechaInicioReservacion").Descripcion)) + "</span>";
                DSOControl.jAlert(lCnfgConferencias.Page, pjsObj + ".ValidarFechaInicioReservacion", lsMsg, lsTitulo);
                return false;
            }
            if (!Collection.GetByConfigName("FechaFinReservacion").DSOControlDB.HasValue)
            {
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", Collection.GetByConfigName("FechaFinReservacion").Descripcion)) + "</span>";
                DSOControl.jAlert(lCnfgConferencias.Page, pjsObj + ".ValidarFechaFinReservacion", lsMsg, lsTitulo);
                return false;
            }


            //Validar el paquete seleccionado
            if (!Collection.GetByConfigName("ServicioSeeYouOn").DSOControlDB.HasValue)
            {
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", Collection.GetByConfigName("ServicioSeeYouOn").Descripcion)) + "</span>";
                DSOControl.jAlert(lCnfgConferencias.Page, pjsObj + ".ValidarServicioRequerido", lsMsg, lsTitulo);
                return false;
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("select iNumParticipantes = count(*) from " + DSODataContext.Schema + ".[VisHistoricos('Participante','Participante','" + Globals.GetCurrentLanguage() + "')] P");
            psbQuery.AppendLine("where P.dtIniVigencia <> P.dtFinVigencia");
            psbQuery.AppendLine("and P.TMSConf = " + lCnfgConferencias.iCodCatalogo);

            int liNumParticipantes = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString(), 0);

            psbQuery.Length = 0;
            psbQuery.AppendLine("select P.NumPuertos from " + DSODataContext.Schema + ".[VisHistoricos('ServicioSeeYouOn','Paquete','" + Globals.GetCurrentLanguage() + "')] P");
            psbQuery.AppendLine("where P.dtIniVigencia <> P.dtFinVigencia");
            psbQuery.AppendLine("and P.dtIniVigencia <= '" + Collection.IniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and P.dtFinVigencia > '" + Collection.IniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and P.iCodCatalogo = " + Collection.GetByConfigName("ServicioSeeYouOn").DataValue);
            psbQuery.AppendLine("union all");
            psbQuery.AppendLine("select SV.NumPuertos from " + DSODataContext.Schema + ".[VisHistoricos('ServicioSeeYouOn','Sala Virtual','" + Globals.GetCurrentLanguage() + "')] SV");
            psbQuery.AppendLine("where SV.dtIniVigencia <> SV.dtFinVigencia");
            psbQuery.AppendLine("and SV.dtIniVigencia <= '" + Collection.IniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and SV.dtFinVigencia > '" + Collection.IniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and SV.iCodCatalogo = " + Collection.GetByConfigName("ServicioSeeYouOn").DataValue);

            int liNumPuertos = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString(), 0);

            if (liNumParticipantes + 1 > liNumPuertos)
            {
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidarNumParticipantes", liNumPuertos.ToString())) + "</span>";
                DSOControl.jAlert(lCnfgConferencias.Page, pjsObj + ".ValidarNumParticipantes", lsMsg, lsTitulo);
                return false;
            }

            lCnfgConferencias.GrabarRegistroAlAgregarParticipante();
            return true;
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

            KeytiaBaseField lField = pFields.GetByConfigName("TMSPhoneBookContact");
            lCol = new DSOGridClientColumn();
            lCol.sName = lField.Column + "Desc";
            lCol.aTargets.Add(lTarget++);
            pSubHisGrid.Config.aoColumnDefs.Add(lCol);

            lField = pFields.GetByConfigName("Address");
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
