using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Data;
using System.Text;
using DSOControls2008;
using System.Collections;

namespace KeytiaWeb.UserInterface
{
    public class CnfgParticipantesState : HistoricState
    {
        public static readonly HistoricState AgregandoParticipante = NewState(9, "AgregandoParticipante");
    }

    public class CnfgParticipantes : HistoricEdit
    {
        protected StringBuilder psbErrores;
        protected bool lbValidarAsistentes = true;
        public CnfgParticipantes()
        {
            Init += new EventHandler(CnfgParticipantes_Init);
        }

        void CnfgParticipantes_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgParticipante";
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;

            StringBuilder lsb = new StringBuilder();
            if (s == CnfgParticipantesState.AgregandoParticipante)
            {
                pbtnGrabar.Visible = true;
                pbtnCancelar.Visible = true;
                pExpAtributos.Visible = true;

                pPanelSubHistoricos.Visible = (iCodRegistro != "null" && pPanelSubHistoricos.Controls.Count > 0);

                piCodEntidad.DropDownList.Enabled = false;
                piCodMaestro.DropDownList.Enabled = false;

                pvchCodigo.TextBox.Enabled = true;
                pvchDescripcion.TextBox.Enabled = true;

                pTablaRegistro.Rows[pvchDescripcion.Row - 1].Visible = true;
                pTablaRegistro.Rows[pdtIniVigencia.Row - 1].Visible = true;

                pdtIniVigencia.DateTimeBox.Enabled = false;
                pdtFinVigencia.DateTimeBox.Enabled = false;
                if (ValidarPermiso(Permiso.Administrar))
                {
                    pdtIniVigencia.DateTimeBox.Enabled = true;
                    pdtFinVigencia.DateTimeBox.Enabled = true;
                }

                if (ValidarPermiso(Permiso.Replicar))
                {
                    pTablaRegistro.Rows[pbReplicarClientes.Row - 1].Visible = true;
                    pbReplicarClientes.DataValue = DBNull.Value;
                }

                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".editing = true;");
                lsb.AppendLine(pjsObj + ".iCodRegistro = " + iCodRegistro + ";");
                lsb.AppendLine(pjsObj + ".iCodCatalogo = " + iCodCatalogo + ";");
                lsb.AppendLine(pjsObj + ".iCodMaestro = " + iCodMaestro + ";");
                lsb.AppendLine(pjsObj + ".iCodEntidad = " + iCodEntidad + ";");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "AgregandoParticipante", lsb.ToString(), true, false);
            }
            else if (s == HistoricState.Baja)
            {
                pPanelSubHistoricos.Visible = (iCodRegistro != "null" && pPanelSubHistoricos.Controls.Count > 0);
            }

            OcultaCampo("Address");
            OcultaCampo("TMSPhoneBookContact");
        }

        protected override bool ValidarVigencias()
        {
            if (!pdtIniVigencia.HasValue)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
            }
            if (!pdtFinVigencia.HasValue)
            {
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

            //el usuario nunca edita las vigencias por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarClaves()
        {
            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            if (pFields.GetByConfigName("TMSPhoneBookContact").DSOControlDB.HasValue)
            {
                pvchDescripcion.DataValue = pFields.GetByConfigName("TMSPhoneBookContact").ToString().Substring(0, Math.Min(pFields.GetByConfigName("TMSPhoneBookContact").ToString().Length, 160));
            }
            if (!pvchDescripcion.HasValue)
            {
                pvchDescripcion.DataValue = "Default";
            }

            //el usuario nunca edita estos valores por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarAtribCatalogosVig()
        {
            return true;
        }

        protected override bool ValidarDatos()
        {
            //Establecer como fecha de fin de vigencia del participante la misma fecha de la conferencia
            psbErrores = new StringBuilder();
            if (State == HistoricState.Baja)
            {
                pdtFinVigencia.DataValue = pdtIniVigencia.DataValue;
            }
            else
            {
                DataRow ldrPhoneBookAddress = null;

                psbQuery.Length = 0;
                psbQuery.AppendLine("select Top 1 Ph.Address, Ph.TMSPhoneBookContact from " + DSODataContext.Schema + ".[VisHistoricos('PhoneBookAddress','PhoneBookAddress','" + Globals.GetCurrentLanguage() + "')] Ph");
                psbQuery.AppendLine("where Ph.iCodCatalogo = " + pFields.GetByConfigName("PhoneBookAddress").DataValue);
                ldrPhoneBookAddress = DSODataAccess.ExecuteDataRow(psbQuery.ToString());

                pFields.GetByConfigName("TMSPhoneBookContact").DataValue = (int)ldrPhoneBookAddress["TMSPhoneBookContact"];
                pFields.GetByConfigName("Address").DataValue = (int)ldrPhoneBookAddress["Address"];

                ValidarTraslapes();
            }
            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();


            bool lbRet = base.ValidarDatos();
            if (State != HistoricState.Baja && lbRet)
            {
                //Validar que tenga asistentes
                ValidarAsistentes();

                if (psbErrores.Length > 0)
                {
                    lbRet = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbRet;
        }

        protected void ValidarTraslapes()
        {
            if (pFields.ContainsConfigName("Address") && pFields.ContainsConfigName("TMSConf"))
            {
                KeytiaBaseField lFieldAddress = pFields.GetByConfigName("Address");
                string lsLang = Globals.GetCurrentLanguage();
                string lsFechaInicioReservacion = null, lsFechaFinReservacion = null;

                int liEstConfEliminado = KDBUtil.SearchICodCatalogo("EstConferencia", "Eliminada", true);

                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("Select TMSConf.FechaInicioReservacion, TMSConf.FechaFinReservacion");
                lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','Español')] TMSConf");
                lsbQuery.AppendLine("where TMSConf.iCodCatalogo = " + pFields.GetByConfigName("TMSConf").DataValue.ToString());
                lsbQuery.AppendLine("and TMSConf.dtIniVigencia <> TMSConf.dtFinVigencia");
                lsbQuery.AppendLine("and TMSConf.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                lsbQuery.AppendLine("and TMSConf.dtFinVigencia > " + pdtIniVigencia.DataValue);
                lsbQuery.AppendLine("and TMSConf.EstConferencia <> " + liEstConfEliminado);

                DataTable ldtTMSConf = DSODataAccess.Execute(lsbQuery.ToString());

                if (ldtTMSConf != null && ldtTMSConf.Rows.Count > 0)
                {
                    lsFechaInicioReservacion = ((DateTime)ldtTMSConf.Rows[0]["FechaInicioReservacion"]).ToString("yyyy-MM-dd HH:mm:ss");
                    lsFechaFinReservacion = ((DateTime)ldtTMSConf.Rows[0]["FechaFinReservacion"]).ToString("yyyy-MM-dd HH:mm:ss");

                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("Select TMSConf.vchDescripcion, Participante.AddressDesc, TMSConf.FechaInicioReservacion, TMSConf.FechaFinReservacion ");
                    lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','" + lsLang + "')] TMSConf,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','" + lsLang + "')] Participante");
                    lsbQuery.AppendLine("where TMSConf.iCodCatalogo = Participante.TMSConf");
                    lsbQuery.AppendLine("and Participante.Address = " + lFieldAddress.DataValue.ToString());
                    if (iCodCatalogo != "null")
                    {
                        lsbQuery.AppendLine("and Participante.iCodCatalogo <> " + iCodCatalogo);
                    }
                    lsbQuery.AppendLine("and (");
                    lsbQuery.AppendLine("		(	TMSConf.FechaInicioReservacion >= '" + lsFechaInicioReservacion + "'");
                    lsbQuery.AppendLine("		and TMSConf.FechaInicioReservacion <  '" + lsFechaFinReservacion + "' )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaFinReservacion >= '" + lsFechaInicioReservacion + "'");
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion <  '" + lsFechaFinReservacion + "' )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaInicioReservacion <= '" + lsFechaInicioReservacion + "'");
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion >= '" + lsFechaFinReservacion + "' )");
                    lsbQuery.AppendLine("	)");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <> TMSConf.dtFinVigencia");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and TMSConf.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <> Participante.dtFinVigencia");
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtFinVigencia >  " + pdtIniVigencia.DataValue);

                    ldtTMSConf = DSODataAccess.Execute(lsbQuery.ToString());

                    if (ldtTMSConf != null && ldtTMSConf.Rows.Count > 0)
                    {
                        string lsDateTimeFormat = Globals.GetLangItem("NetDateTimeFormat");
                        string lsError = ldtTMSConf.Rows[0]["AddressDesc"].ToString() + ": "
                            + ldtTMSConf.Rows[0]["vchDescripcion"]
                            + " (" + ((DateTime)ldtTMSConf.Rows[0]["FechaInicioReservacion"]).ToString(lsDateTimeFormat)
                            + " - " + ((DateTime)ldtTMSConf.Rows[0]["FechaFinReservacion"]).ToString(lsDateTimeFormat) + ")";
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslape", lsError));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }

        protected void ValidarAsistentes()
        {
            if (!lbValidarAsistentes) return;

            if (pFields.ContainsConfigName("Address"))
            {
                KeytiaBaseField lFieldAddress = pFields.GetByConfigName("Address");
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("select iCodRegistro");
                lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('AsistenteConferencia','Asistentes','Español')]");
                lsbQuery.AppendLine("where Participante = " + iCodCatalogo);
                lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("and dtIniVigencia <= " + dtIniVigencia.DataValue);
                lsbQuery.AppendLine("and dtFinVigencia >  " + dtIniVigencia.DataValue);

                DataTable ldt = DSODataAccess.Execute(lsbQuery.ToString());
                if (ldt == null || ldt.Rows.Count == 0)
                {
                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("select TMSSystems.Emple, EmpleDesc = TMSSystems.EmpleDesc + ' (' + TMSSystems.EmpleCod + ')'");
                    lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('TMSSystemAddress','Español')] TMSSystemAddress,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHisComun('TMSSystems','Español')] TMSSystems");
                    lsbQuery.AppendLine("where TMSSystemAddress.Address = " + lFieldAddress.DataValue.ToString());
                    lsbQuery.AppendLine("and TMSSystemAddress.TMSSystems = TMSSystems.iCodCatalogo");
                    lsbQuery.AppendLine("and TMSSystems.vchDesMaestro = 'Cuenta Movi'");

                    ldt = DSODataAccess.Execute(lsbQuery.ToString());
                    if (ldt != null && ldt.Rows.Count > 0)
                    {
                        AgregarAsistente((int)ldt.Rows[0]["Emple"], ldt.Rows[0]["EmpleDesc"].ToString());
                    }
                    else
                    {
                        string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NoAsistentes"));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }

        protected void AgregarAsistente(int liCodEmple, string lvchEmpleDesc)
        {
            Hashtable lhtVal = new Hashtable();
            lhtVal.Add("{Participante}", iCodCatalogo);
            lhtVal.Add("{Emple}", liCodEmple);
            lhtVal.Add("{EstAsistente}", KDBUtil.SearchICodCatalogo("EstAsistente", "Por notificar", true));
            lhtVal.Add("dtIniVigencia", pdtIniVigencia.DataValue.ToString());
            lhtVal.Add("dtFinVigencia", pdtFinVigencia.DataValue.ToString());
            KDBUtil.SaveHistoric("AsistenteConferencia", "Asistentes",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                lvchEmpleDesc.Substring(0, Math.Min(lvchEmpleDesc.Length, 160)), lhtVal);
        }

        protected override void GrabarRegistro()
        {
            //SeeYouOnCOM.SyncCOM lSyncCOM = new SeeYouOnCOM.SyncCOM();

            //psbQuery.Length = 0;
            //psbQuery.AppendLine("select ConfNumericId from " + DSODataContext.Schema + ".[VisHistoricos('TMSConf','Conferencia','"+Globals.GetCurrentLanguage()+"')] Conf");
            //psbQuery.AppendLine("where Conf.dtIniVigencia <> Conf.dtFinVigencia");
            //psbQuery.AppendLine("and Conf.iCodCatalogo = " + pFields.GetByConfigName("TMSConf").DataValue);

            //string lsNumericId = DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString();
            //int liCodSystem = int.Parse(pFields.GetByConfigName("TMSSystems").DataValue.ToString());

            //if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            //{
            //    lSyncCOM.MCUPartDelete(lsNumericId, liCodSystem, (int)Session["iCodUsuarioDB"]);
            //}
            //else
            //{
            //    lSyncCOM.MCUPartSave(lsNumericId, liCodSystem, (int)Session["iCodUsuarioDB"]);
            //}


            base.GrabarRegistro();

            KeytiaBaseField lField = null;
            bool lbSalaVirtual = false;
            DataTable ldtSalaVirtual = null;
            if (Historico != null)
            {
                if (Historico.Fields.ContainsConfigName("ServicioSeeYouOn"))
                {
                    lField = Historico.Fields.GetByConfigName("ServicioSeeYouOn");
                    ldtSalaVirtual = DSODataAccess.Execute("select vchDesMaestro = IsNull(vchDesMaestro, ''), TMSSystems " +
                        "from [" + DSODataContext.Schema + "].[VisHistoricos('ServicioSeeYouOn','Español')] " +
                        "where iCodCatalogo = " + lField.DataValue.ToString());
                    if (ldtSalaVirtual != null && ldtSalaVirtual.Rows.Count > 0 && ldtSalaVirtual.Rows[0]["vchDesMaestro"].ToString() == "Sala Virtual")
                    {
                        lbSalaVirtual = true;
                    }
                }
                if (lbSalaVirtual)
                {
                    if (State == HistoricState.Baja)
                    {
                        CambiarEstatus("Eliminado");
                    }
                    else
                    {
                        CambiarEstatus("PorAgregar");
                    }
                }
                else
                {
                    if (State == HistoricState.Baja)
                    {
                        CambiarEstatus("Eliminando");
                    }
                    else
                    {
                        CambiarEstatus("Agregando");
                    }
                }
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            OcultaCampo("EstParticipante");
            if (Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin" &&
                pFields != null && pFields.ContainsConfigName("Address"))
            {
                pFields.GetByConfigName("Address").DisableField();
            }

            if (pFields != null && pFields.ContainsConfigName("PhoneBookAddress")
             && pFields.GetByConfigName("PhoneBookAddress").DSOControlDB.HasValue)
            {
                KeytiaAutoCompleteField lCampo = (KeytiaAutoCompleteField)pFields.GetByConfigName("PhoneBookAddress");
                if ((State == CnfgParticipantesState.AgregandoParticipante) && lCampo != null)
                {
                    string lsValor = lCampo.DataValue.ToString();
                    if (lsValor != "null")
                    {
                        string lsLang = Globals.GetCurrentLanguage();
                        StringBuilder lsbQuery = new StringBuilder();
                        lsbQuery.AppendLine("select id = iCodCatalogo, value = TMSPhoneBookContactDesc + ' (' + AddressDesc + ')' ");
                        lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PhoneBookAddress','PhoneBookAddress','" + lsLang + "')]");
                        lsbQuery.AppendLine("where iCodCatalogo = " + lsValor);
                        DataTable lDataSource = DSODataAccess.Execute(lsbQuery.ToString());
                        ((DSOAutocomplete)lCampo.DSOControlDB).DataSource = lDataSource;
                        ((DSOAutocomplete)lCampo.DSOControlDB).Fill();
                    }
                }
            }
        }

        private void CambiarEstatus(string lvchCodEstatus)
        {
            System.Collections.Hashtable lhtVal = new System.Collections.Hashtable();
            lhtVal.Add("iCodRegistro", int.Parse(iCodRegistro));
            lhtVal.Add("{EstParticipante}", KDBUtil.SearchICodCatalogo("EstParticipante", lvchCodEstatus, true));
            KDBUtil.SaveHistoric("Participante", "Participante", vchCodigo.TextBox.Text, null, lhtVal);
        }

        public virtual void AgregarRegistroSubHistorico()
        {
            PrevState = State;

            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            pvchDescripcion.DataValue = "Default";
            pdtIniVigencia.DataValue = DateTime.Today;
            pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();
            GrabarRegistro();

            SetHistoricState(CnfgParticipantesState.AgregandoParticipante);
            pFields.EnableFields();
        }

        protected override void pbtnCancelar_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            if (State == HistoricState.Edicion
                || State == HistoricState.Baja)
            {
                ConsultarRegistro();
            }
            else if (State == CnfgParticipantesState.AgregandoParticipante)
            {
                EliminarParticipanteNoAgregado();
                ConsultarRegistro();
            }

            FirePostCancelarClick();
        }

        protected virtual void EliminarParticipanteNoAgregado()
        {
            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            pvchDescripcion.DataValue = "Cancelar";
            pdtFinVigencia.DataValue = pdtIniVigencia.Date;
            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();
            GrabarRegistro();
        }

        #region IPostBackEventHandler Members
        public override void RaisePostBackEvent(string eventArgument)
        {
            base.RaisePostBackEvent(eventArgument);
            if (eventArgument == "btnGrabar"
                && State == CnfgParticipantesState.AgregandoParticipante)
            {
                pbtnGrabar_ServerClick(pbtnGrabar, new EventArgs());
            }
        }
        #endregion

    }
}
