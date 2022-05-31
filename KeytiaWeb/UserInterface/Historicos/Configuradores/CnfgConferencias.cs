using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSOControls2008;
using KeytiaServiceBL;
using SeeYouOnServiceBL;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgConferenciasState : HistoricState
    {
        public static readonly HistoricState AgregandoConferencia = NewState(9, "AgregandoConferencia");
    }

    public class CnfgConferencias : HistoricEdit
    {
        protected StringBuilder psbErrores;
        protected HashSet<string> pHTOcultos;
        public CnfgConferencias()
        {
            Init += new EventHandler(CnfgConferencias_Init);
        }

        void CnfgConferencias_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgConferencias";

            pHTOcultos = new HashSet<string>();
            pHTOcultos.Add("TMSSystems");
            if (!(Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin"))
            {
                pHTOcultos.Add("Client");
            }
            pHTOcultos.Add("ConfNumericId");
            pHTOcultos.Add("FechaInicioReal");
            pHTOcultos.Add("FechaFinReal");
            pHTOcultos.Add("FechaEliminacion");
            //pHTOcultos.Add("EstConferencia");
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgConferencias.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgConferencias.js") + "'type='text/javascript'></script>\r\n", true, false);
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;

            StringBuilder lsb = new StringBuilder();
            if (s == CnfgConferenciasState.AgregandoConferencia)
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
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "AgregandoConferencia", lsb.ToString(), true, false);
            }
            else if (s == HistoricState.Baja)
            {
                pPanelSubHistoricos.Visible = (iCodRegistro != "null" && pPanelSubHistoricos.Controls.Count > 0);
            }

            foreach (string lsCampo in pHTOcultos)
            {
                OcultaCampo(lsCampo);
            }
        }

        protected override void InitFiltros()
        {
            base.InitFiltros();
            pTablaFiltros.Rows[0].Visible = false;
            pTablaFiltros.Rows[1].Visible = false;
            foreach (string lsCampo in pHTOcultos)
            {
                OcultaCampoFiltro(lsCampo);
            }
        }

        protected override void InitFiltrosFields()
        {
            DSOTextBox lDSOtxt;
            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lDSOtxt = new DSOTextBox();
                    lDSOtxt.ID = lField.Column;
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lDSOtxt.AddClientEvent("dataFilter", lField.ConfigName + "Desc");
                    }
                    else
                    {
                        lDSOtxt.AddClientEvent("dataFilter", lField.ConfigName);
                    }
                    lDSOtxt.Row = lField.Row + 2;
                    lDSOtxt.ColumnSpan = lField.ColumnSpan;
                    lDSOtxt.Table = pTablaFiltros;
                    lDSOtxt.CreateControls();

                    phtFiltros.Add(lDSOtxt.ID, lDSOtxt);
                }
            }
        }

        protected override void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pHisGrid.ClearConfig();
            pHisGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pHisGrid.Config.bAutoWidth = true;
            pHisGrid.Config.sScrollX = "100%";
            pHisGrid.Config.sScrollY = "400px";
            pHisGrid.Config.sPaginationType = "full_numbers";
            pHisGrid.Config.bJQueryUI = true;
            pHisGrid.Config.bProcessing = true;
            pHisGrid.Config.bServerSide = true;
            pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerData(this, sSource, aoData, fnCallback);}";
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetSeeYouOnConferencias");

            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "Consultar";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "50px";
            lCol.sClass = "TdConsult";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "vchCodigo";
            //lCol.aTargets.Add(lTarget++);
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "vchDescripcion";
            //lCol.aTargets.Add(lTarget++);
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            InitGridFields();

            lTarget = pHisGrid.Config.aoColumnDefs.Count;

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "dtIniVigencia";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "120px";
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "dtFinVigencia";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "120px";
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFecUltAct";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            if (pHisGrid.Config.aoColumnDefs.Count > 10)
            {
                pHisGrid.Config.sScrollXInner = (pHisGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
            }

            pHisGrid.Fill();
        }

        protected override void InitGridFields()
        {
            AgregarGridField("FechaInicioReservacion");
            AgregarGridField("FechaFinReservacion");
            AgregarGridField("AsuntoConferencia");
            AgregarGridField("Proyecto");
            AgregarGridField("TipoConferencia");
            if (Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin")
            {
                AgregarGridField("Client");
            }
            AgregarGridField("ServicioSeeYouOn");
            AgregarGridField("EstConferencia");
        }

        protected virtual void AgregarGridField(string lsConfigName)
        {
            if (pFields.ContainsConfigName(lsConfigName))
            {
                DSOGridClientColumn lCol;
                KeytiaBaseField lField = pFields.GetByConfigName(lsConfigName);
                int lTarget = pHisGrid.Config.aoColumnDefs.Count;

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

        protected override bool ValidarVigencias()
        {
            pdtIniVigencia.DataValue = DateTime.Today;
            pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            //el usuario nunca edita las vigencias por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarCampos()
        {
            bool lbRet = true; ;
            if (State != HistoricState.Baja)
            {
                lbRet = base.ValidarCampos();
            }

            return lbRet;
        }

        protected override bool ValidarClaves()
        {
            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            if (pFields.GetByConfigName("AsuntoConferencia").DSOControlDB.HasValue)
            {
                pvchDescripcion.DataValue = pFields.GetByConfigName("AsuntoConferencia").ToString().Substring(0, Math.Min(pFields.GetByConfigName("AsuntoConferencia").ToString().Length, 160));
            }
            if (!pvchDescripcion.HasValue)
            {
                pvchDescripcion.DataValue = "Default";
            }

            //el usuario nunca edita estos valores por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarDatos()
        {
            //TODO: Validar que tenga participantes
            if (State == HistoricState.Baja)
            {
                pFields.GetByConfigName("FechaEliminacion").DataValue = DateTime.Now;
                pFields.GetByConfigName("EstConferencia").DataValue = GetEstatusConferencia("Eliminando");
            }

            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();

            bool lbRet = base.ValidarDatos();
            if (State != HistoricState.Baja && lbRet)
            {
                psbErrores = new StringBuilder();

                //Validar que un empleado no sea asistente en dos equipos diferentes en el mismo horario
                ValidarTraslapesAsistentes();
                ValidarTraslapesParticipantes();
                ValidarFechasReservacion();

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

        protected void ValidarFechasReservacion()
        {
            if (pFields.ContainsConfigName("FechaInicioReservacion") && pFields.ContainsConfigName("FechaFinReservacion"))
            {
                KeytiaBaseField lFieldIni = pFields.GetByConfigName("FechaInicioReservacion");
                KeytiaBaseField lFieldFin = pFields.GetByConfigName("FechaFinReservacion");
                if (lFieldIni.DSOControlDB.HasValue && lFieldFin.DSOControlDB.HasValue)
                {
                    DateTime ldtFechaInicioReservacion = ((DSODateTimeBox)lFieldIni.DSOControlDB).Date;
                    DateTime ldtFechaFinReservacion = ((DSODateTimeBox)lFieldFin.DSOControlDB).Date;
                    if (ldtFechaInicioReservacion.CompareTo(ldtFechaFinReservacion) > 0)
                    {
                        //Hora de Inicio debe ser menor a Hora de Fin
                        string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FechasEquivocadas",
                            lFieldIni.Descripcion,
                            lFieldFin.Descripcion));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                    if (pFields.ContainsConfigName("ServicioSeeYouOn"))
                    {
                        StringBuilder lsbQuery = new StringBuilder();
                        lsbQuery.AppendLine("Select FechaInicioContrato, FechaFinContrato");
                        lsbQuery.AppendLine("from [VisHisComun('ServicioSeeYouOn','Español')] ServicioSeeYouOn,");
                        lsbQuery.AppendLine("     [VisHistoricos('ContratoSeeYouOn','Contrato','Español')] ContratoSeeYouOn");
                        lsbQuery.AppendLine("where ServicioSeeYouOn.ContratoSeeYouOn = ContratoSeeYouOn.iCodCatalogo");
                        lsbQuery.AppendLine("and ServicioSeeYouOn.iCodCatalogo = " + pFields.GetByConfigName("ServicioSeeYouOn").DataValue.ToString());
                        lsbQuery.AppendLine("and ServicioSeeYouOn.dtIniVigencia <> ServicioSeeYouOn.dtFinVigencia");
                        lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(pdtIniVigencia.Date, pdtFinVigencia.Date, false, "ServicioSeeYouOn"));
                        lsbQuery.AppendLine("and ContratoSeeYouOn.dtIniVigencia <> ContratoSeeYouOn.dtFinVigencia");
                        lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(pdtIniVigencia.Date, pdtFinVigencia.Date, false, "ContratoSeeYouOn"));

                        DataTable ldtContrato = DSODataAccess.Execute(lsbQuery.ToString());
                        if (ldtContrato.Rows.Count > 0 && !(ldtContrato.Rows[0]["FechaInicioContrato"] is DBNull) && !(ldtContrato.Rows[0]["FechaFinContrato"] is DBNull))
                        {
                            DateTime ldtFechaInicioContrato = (DateTime)Util.IsDBNull(ldtContrato.Rows[0]["FechaInicioContrato"], DateTime.Now);
                            DateTime ldtFechaFinContrato = (DateTime)Util.IsDBNull(ldtContrato.Rows[0]["FechaFinContrato"], DateTime.Now);

                            if (ldtFechaInicioReservacion.CompareTo(ldtFechaInicioContrato) < 0)
                            {
                                //Hora de Inicio de reservación debe ser mayor que fecha de inicio de contrato
                                string lsFechaInicioContrato = Globals.GetLangItem("Atrib", "Atributos", "FechaInicioContrato");
                                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FechasEquivocadas",
                                    lsFechaInicioContrato,
                                    lFieldIni.Descripcion));
                                psbErrores.Append("<li>" + lsError + "</li>");
                            }

                            if (ldtFechaFinReservacion.CompareTo(ldtFechaFinContrato) > 0)
                            {
                                //Hora de Fin de reservación debe ser menor que fecha de fin de contrato
                                string lsFechaFinContrato = Globals.GetLangItem("Atrib", "Atributos", "FechaFinContrato");
                                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FechasEquivocadas",
                                    lFieldFin.Descripcion,
                                    lsFechaFinContrato));
                                psbErrores.Append("<li>" + lsError + "</li>");
                            }
                        }
                    }
                }
            }
        }

        protected void ValidarTraslapesAsistentes()
        {
            if (iCodRegistro != "null")
            {
                StringBuilder lsbQuery = new StringBuilder();
                string lsLang = Globals.GetCurrentLanguage();
                string lsFechaInicioReservacion = null, lsFechaFinReservacion = null;

                if (pFields.ContainsConfigName("FechaInicioReservacion") && pFields.ContainsConfigName("FechaFinReservacion"))
                {
                    lsFechaInicioReservacion = pFields.GetByConfigName("FechaInicioReservacion").DataValue.ToString();
                    lsFechaFinReservacion = pFields.GetByConfigName("FechaFinReservacion").DataValue.ToString();

                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("Select TMSConf.vchDescripcion, Asistente.EmpleDesc, TMSConf.FechaInicioReservacion, TMSConf.FechaFinReservacion ");
                    lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','" + lsLang + "')] TMSConf,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','" + lsLang + "')] Participante,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('AsistenteConferencia','Asistentes','" + lsLang + "')] Asistente,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','" + lsLang + "')] TMSConf2,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','" + lsLang + "')] Participante2,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('AsistenteConferencia','Asistentes','" + lsLang + "')] Asistente2");
                    lsbQuery.AppendLine("where TMSConf.iCodCatalogo = Participante.TMSConf");
                    lsbQuery.AppendLine("and Participante.iCodCatalogo = Asistente.Participante");
                    lsbQuery.AppendLine("and TMSConf2.iCodCatalogo = Participante2.TMSConf");
                    lsbQuery.AppendLine("and Participante2.iCodCatalogo = Asistente2.Participante");
                    lsbQuery.AppendLine("and Asistente.Emple = Asistente2.Emple");
                    if (iCodCatalogo != "null")
                    {
                        lsbQuery.AppendLine("and TMSConf2.iCodCatalogo = " + iCodCatalogo);
                        lsbQuery.AppendLine("and TMSConf.iCodCatalogo <> " + iCodCatalogo);
                    }
                    lsbQuery.AppendLine("and (");
                    lsbQuery.AppendLine("		(	TMSConf.FechaInicioReservacion >= " + lsFechaInicioReservacion);
                    lsbQuery.AppendLine("		and TMSConf.FechaInicioReservacion <  " + lsFechaFinReservacion + " )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaFinReservacion >= " + lsFechaInicioReservacion);
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion <  " + lsFechaFinReservacion + " )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaInicioReservacion <= " + lsFechaInicioReservacion);
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion >= " + lsFechaFinReservacion + " )");
                    lsbQuery.AppendLine("	)");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <> TMSConf.dtFinVigencia");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and TMSConf.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <> Participante.dtFinVigencia");
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Asistente.dtIniVigencia <> Asistente.dtFinVigencia");
                    lsbQuery.AppendLine("and Asistente.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Asistente.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante2.dtIniVigencia <> Participante2.dtFinVigencia");
                    lsbQuery.AppendLine("and Participante2.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante2.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Asistente2.dtIniVigencia <> Asistente2.dtFinVigencia");
                    lsbQuery.AppendLine("and Asistente2.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Asistente2.dtFinVigencia >  " + pdtIniVigencia.DataValue);

                    DataTable ldtTMSConf = DSODataAccess.Execute(lsbQuery.ToString());

                    if (ldtTMSConf != null && ldtTMSConf.Rows.Count > 0)
                    {
                        string lsDateTimeFormat = Globals.GetLangItem("NetDateTimeFormat");
                        string lsError = ldtTMSConf.Rows[0]["EmpleDesc"].ToString() + ": "
                            + ldtTMSConf.Rows[0]["vchDescripcion"]
                            + " (" + ((DateTime)ldtTMSConf.Rows[0]["FechaInicioReservacion"]).ToString(lsDateTimeFormat)
                            + " - " + ((DateTime)ldtTMSConf.Rows[0]["FechaFinReservacion"]).ToString(lsDateTimeFormat) + ")";
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslape", lsError));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }

        protected void ValidarTraslapesParticipantes()
        {
            if (iCodRegistro != "null")
            {
                StringBuilder lsbQuery = new StringBuilder();
                string lsLang = Globals.GetCurrentLanguage();
                string lsFechaInicioReservacion = null, lsFechaFinReservacion = null;

                if (pFields.ContainsConfigName("FechaInicioReservacion") && pFields.ContainsConfigName("FechaFinReservacion"))
                {
                    lsFechaInicioReservacion = pFields.GetByConfigName("FechaInicioReservacion").DataValue.ToString();
                    lsFechaFinReservacion = pFields.GetByConfigName("FechaFinReservacion").DataValue.ToString();

                    int liEstPartEliminado = KDBUtil.SearchICodCatalogo("EstParticipante", "Eliminado", true);
                    int liEstConfEliminado = KDBUtil.SearchICodCatalogo("EstConferencia", "Eliminada", true);

                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("Select TMSConf.vchDescripcion, Participante.AddressDesc, TMSConf.FechaInicioReservacion, TMSConf.FechaFinReservacion ");
                    lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','" + lsLang + "')] TMSConf,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','" + lsLang + "')] Participante,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','" + lsLang + "')] TMSConf2,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','" + lsLang + "')] Participante2");
                    lsbQuery.AppendLine("where TMSConf.iCodCatalogo = Participante.TMSConf");
                    lsbQuery.AppendLine("and TMSConf2.iCodCatalogo = Participante2.TMSConf");
                    lsbQuery.AppendLine("and Participante.Address = Participante2.Address");
                    if (iCodCatalogo != "null")
                    {
                        lsbQuery.AppendLine("and TMSConf2.iCodCatalogo = " + iCodCatalogo);
                        lsbQuery.AppendLine("and TMSConf.iCodCatalogo <> " + iCodCatalogo);
                    }
                    lsbQuery.AppendLine("and (");
                    lsbQuery.AppendLine("		(	TMSConf.FechaInicioReservacion >= " + lsFechaInicioReservacion);
                    lsbQuery.AppendLine("		and TMSConf.FechaInicioReservacion <  " + lsFechaFinReservacion + " )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaFinReservacion >= " + lsFechaInicioReservacion);
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion <  " + lsFechaFinReservacion + " )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaInicioReservacion <= " + lsFechaInicioReservacion);
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion >= " + lsFechaFinReservacion + " )");
                    lsbQuery.AppendLine("	)");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <> TMSConf.dtFinVigencia");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and TMSConf.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <> Participante.dtFinVigencia");
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante2.dtIniVigencia <> Participante2.dtFinVigencia");
                    lsbQuery.AppendLine("and Participante2.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante2.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.EstParticipante <> " + liEstPartEliminado);
                    lsbQuery.AppendLine("and TMSConf.EstConferencia <> " + liEstConfEliminado);
                    lsbQuery.AppendLine("and Participante2.EstParticipante <> " + liEstPartEliminado);
                    lsbQuery.AppendLine("and TMSConf2.EstConferencia <> " + liEstConfEliminado);

                    DataTable ldtTMSConf = DSODataAccess.Execute(lsbQuery.ToString());

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

        protected override void pbtnCancelar_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            if (State == HistoricState.Edicion
                || State == HistoricState.Baja)
            {
                ConsultarRegistro();
            }
            else if (State == CnfgConferenciasState.AgregandoConferencia)
            {
                EliminarConferenciaNoAgregada();
                ConsultarRegistro();
            }

            FirePostCancelarClick();
        }

        public override void ConsultarRegistro()
        {
            base.ConsultarRegistro();

            if (State == HistoricState.Consulta)
            {
                int liEstFinalizada = KDBUtil.SearchICodCatalogo("EstConferencia", "Finalizada", true);
                int liEstReprogramada = KDBUtil.SearchICodCatalogo("EstConferencia", "ReProgramada", true);
                int liEstEliminada = KDBUtil.SearchICodCatalogo("EstConferencia", "Eliminada", true);
                int liEstActual = 0;
                KeytiaBaseField lField = pFields.GetByConfigName("EstConferencia");

                liEstActual = int.Parse(lField.DataValue.ToString());

                if (liEstActual == liEstFinalizada || liEstActual == liEstEliminada)
                {
                    pbtnEditar.Visible = false;
                    pbtnBaja.Visible = false;
                }
                else if (liEstActual == liEstReprogramada)
                {
                    if (Session["vchCodPerfil"].ToString() != "SeeYouOnAdmin")
                    {
                        pbtnEditar.Visible = false;
                    }
                }
            }
        }

        protected override void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            SetHistoricState(HistoricState.Baja);
            pFields.DisableFields();

            FirePostBajaClick();
        }

        /// <summary>
        /// Esta sobrecarga al metodo es disparada despues de dar click en agregar en la aplicacion de Reservaciones en SYO
        /// </summary>
        protected override void AgregarRegistro()
        {
            //Guarda el estatus actual de la aplicacion
            PrevState = State;

            //Inicializa el vchCodigo del historico en la fecha y hora actual
            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //Guarda la descripcion del historico
            pvchDescripcion.DataValue = "Default";
            
            //Inicio de vigencia hoy
            pdtIniVigencia.DataValue = DateTime.Today;
            //Fin de vigencia el 01/01/2079
            pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);

            //Extraer el icodcatalogo de los estatus "inicializada" y "finalizada"
            int liEstIniciada = (int)Util.IsDBNull(GetEstatusConferencia("Iniciada"), 0);
            int liEstFinalizada = (int)Util.IsDBNull(GetEstatusConferencia("Finalizada"), 0);
            
            //Si el estatus de conferencia tiene valor entonces...
            if (pFields.GetByConfigName("EstConferencia").DSOControlDB.HasValue)
            {
                int liCodEstatus;
                //Se realiza un parseo para convertir a int el valor del estatus de la conferencia.
                if (int.TryParse(pFields.GetByConfigName("EstConferencia").DataValue.ToString(), out liCodEstatus))
                {
                    //Si el estatus es iniciada o finalizada
                    if (liCodEstatus == liEstIniciada || liCodEstatus == liEstFinalizada)
                    {
                        //Si el historico contiene la fecha de inicio de la reservacion
                        if (pFields.ContainsConfigName("FechaInicioReservacion"))
                        {
                            //Se deshabilita el campo
                            pFields.GetByConfigName("FechaInicioReservacion").DisableField();
                        }
                    }
                    //Si el estatus es finalizada
                    if (liCodEstatus == liEstFinalizada)
                    {
                        //Si el historico contiene el valor de FechaFinReservacion
                        if (pFields.ContainsConfigName("FechaFinReservacion"))
                        {
                            //Se deshabilita el campo
                            pFields.GetByConfigName("FechaFinReservacion").DisableField();
                        }
                    }
                    //Si el estatus es diferente de inicializada y finalizada entonces
                    if (liCodEstatus != liEstIniciada && liCodEstatus != liEstFinalizada)
                    {
                        //Se establece el valor del estatus de la conferencia "ReProgramando"
                        pFields.GetByConfigName("EstConferencia").DataValue = GetEstatusConferencia("ReProgramando");
                    }
                }
            }
            else //Si no tiene valor el atributo EstConferencia del historico, entonces lo dejamos con estatus Programando
            {
                pFields.GetByConfigName("EstConferencia").DataValue = GetEstatusConferencia("Programando");
            }
            //Se extrae el iCodCatalogo del Cliente que tiene el usuario de la sesion.
            pFields.GetByConfigName("Client").DataValue = GetClienteUsuario((int)Session["iCodUsuario"], pdtIniVigencia.Date);

            //Regresar en un Hash los valores del historico actual
            phtValues = pFields.GetValues();
            //Regresar en un hash los valores de las relaciones del historico
            pdsRelValues = pFields.GetRelationValues();
            //Llamar el metodo base para grabar el registro
            base.GrabarRegistro();

            //Se guarda el estado de "Agregando Conferencia" en el Historico
            SetHistoricState(CnfgConferenciasState.AgregandoConferencia);
            //Habilitar campos en Historicos, disponibles para modo "Agregar" en un historico
            pFields.EnableFields();
        }
        /// <summary>
        /// Sobre carga al metodo Grabar Registro de Historicos.
        /// </summary>
        protected override void GrabarRegistro()
        {
            //Manda a llamar el metodo base de Historicos para grabar el registro.
            base.GrabarRegistro();

            KeytiaBaseField lField = null;
            bool lbSalaVirtual = false;
            DataTable ldtSalaVirtual = null;
            //Si el historico contiene el atributo "ServicioSeeYouOn"
            if (pFields.ContainsConfigName("ServicioSeeYouOn"))
            {
                //Se extraer el valor del atributo
                lField = pFields.GetByConfigName("ServicioSeeYouOn");

                //Extraer el vchDescripcion del Maestro donde el coincida el iCodCatalogo en la vista [VisHistoricos('ServicioSeeYouOn','Español')]
                ldtSalaVirtual = DSODataAccess.Execute("select vchDesMaestro = IsNull(vchDesMaestro, ''), TMSSystems " +
                    "from [" + DSODataContext.Schema + "].[VisHistoricos('ServicioSeeYouOn','Español')] " +
                    "where iCodCatalogo = " + lField.DataValue.ToString());

                //Si el datatable es diferente de nulo, tiene filas y el valor del campo vchDesMaestro es Sala Virtual entonces...
                if (ldtSalaVirtual != null && ldtSalaVirtual.Rows.Count > 0 && ldtSalaVirtual.Rows[0]["vchDesMaestro"].ToString() == "Sala Virtual")
                {
                    //Deja en true la variable
                    lbSalaVirtual = true;
                }
            }
            if (lbSalaVirtual) //Si es sala virtual
            {
                //Si en el estatus diferente de baja entonces
                if (State != HistoricState.Baja)
                {
                    //Creare un hash con los siguientes elementos
                    System.Collections.Hashtable lhtVal = new System.Collections.Hashtable();
                    lhtVal.Add("iCodRegistro", int.Parse(iCodRegistro));
                    lhtVal.Add("{TMSSystems}", ldtSalaVirtual.Rows[0]["TMSSystems"]);
                    //Estatus de conferencia como programada
                    lhtVal.Add("{EstConferencia}", KDBUtil.SearchICodCatalogo("EstConferencia", "Programada", true));
                    //Guarda un Historico en la entidad "TMSConf", maestro "Conferencia", 
                    KDBUtil.SaveHistoric("TMSConf", "Conferencia", vchCodigo.TextBox.Text, null, lhtVal);
                }
                else
                {
                    //Si el estatus es baja, cambia el estatus a eliminada
                    CambiarEstatus("Eliminada");
                }
            }
            else //Si no se trata de una sala virtual entonces
            {
                try
                {
                    //Se crea una instancia del Com del SYO
                    SeeYouOnCOM.SyncCOM lSyncCOM = new SeeYouOnCOM.SyncCOM();
                    //Si el estatus del historico es diferente de baja
                    if (State != HistoricState.Baja)
                    {
                        //Grabar una conferencia en MCU
                        lSyncCOM.MCUConfSave(int.Parse(iCodCatalogo), (int)Session["iCodUsuarioDB"]);
                    }
                    else
                    {
                        //Eliminar la conferencia en el MCU
                        lSyncCOM.MCUConfDelete(int.Parse(iCodCatalogo), (int)Session["iCodUsuarioDB"]);
                    }
                }
                catch (Exception ex) //En caso de cualquier error, mostrara un mensaje de alerta
                {
                    //Mandar llamar metodo adecuado de SeeYouOnCOM.SyncCOM para programar la conferencia en el MCU
                    string lsTitulo = DSOControl.JScriptEncode(this.AlertTitle);
                    //Mensaje de alerta "Error al guardar la conferencia en el MCU"
                    string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrMCUConfSave"));
                    DSOControl.jAlert(Page, pjsObj + ".ErrGuardarConferencia", lsMsg, lsTitulo);
                    //Error en log
                    Util.LogException(ex);
                }
            }
            //Si el estado es diferente de baja
            if (State != HistoricState.Baja)
            {
                //Enviar la notificación de la conferencia creada
                int liCodCatalogo;
                if (int.TryParse(iCodCatalogo, out liCodCatalogo))
                {
                    //NotificarAsistentesConferencia(liCodCatalogo, Globals.GetCurrentLanguage(), (int)Session["iCodUsuarioDB"]);
                    #region Método original
                    //Crea una instancia del SYO COM y en un mensaje de cola manda a grabar la notificacion de asistentes a la conferencia
                    SeeYouOnCOM.ISyncCOM pSyncCom = (SeeYouOnCOM.ISyncCOM)Marshal.BindToMoniker("queue:/new:SeeYouOnCOM.SyncCOM");
                    
                    //Manda a llamar al metodo que realiza la notificacion de conferencia a los asistentes.
                    pSyncCom.NotificAsistConf(liCodCatalogo, Globals.GetCurrentLanguage(), (int)Session["iCodUsuarioDB"]);
                    Marshal.ReleaseComObject(pSyncCom);
                    //Mensaje al Log, notificando que se programo la notificacion por correo a los asistentes
                    Util.LogMessage("Se ha programado el envío de correo de a los asistentes de la conferencia " + pvchDescripcion + " (" + pvchCodigo + ").");
                    pSyncCom = null;
                    #endregion
                }
            }
        }

        private void NotificarAsistentesConferencia(int liCodConferencia, string lsLang, int iCodUsuarioDB)
        {
            KeytiaServiceBL.DSODataContext.SetContext(iCodUsuarioDB);
            NotificacionAsistentes loNotif = new NotificacionAsistentes();
            loNotif.iCodConferencia = liCodConferencia;
            loNotif.vchIdioma = lsLang;
            loNotif.NotificAsistConf();
        }

        /// <summary>
        /// Se enncarga de cambiar el estatus de la conferencia y guardar un historico nuevo en TMSConf/Conferencia
        /// </summary>
        /// <param name="lvchCodEstatus">Recibe el vchCodigo el estatus de la conferencia</param>
        private void CambiarEstatus(string lvchCodEstatus)
        {
            System.Collections.Hashtable lhtVal = new System.Collections.Hashtable();
            lhtVal.Add("iCodRegistro", int.Parse(iCodRegistro));
            lhtVal.Add("{EstConferencia}", KDBUtil.SearchICodCatalogo("EstConferencia", lvchCodEstatus, true));
            KDBUtil.SaveHistoric("TMSConf", "Conferencia", vchCodigo.TextBox.Text, null, lhtVal);
        }

        public virtual void GrabarRegistroAlAgregarParticipante()
        {
            ValidarVigencias();
            ValidarClaves();
            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();

            base.GrabarRegistro();

        }

        /// <summary>
        /// Extraer el icodcatalogo del estatus de la conferencia, en base al vchCodigo
        /// </summary>
        /// <param name="lsCodEstatus">vchCodigo del Estatus de la Conferencia</param>
        /// <returns>El valor del iCodCatalogo resultado de la consulta en un tipo de dato Object</returns>
        protected object GetEstatusConferencia(string lsCodEstatus)
        {
            psbQuery.Length = 0;
            psbQuery.AppendLine("select iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('EstConferencia','Estatus','" + Globals.GetCurrentLanguage() + "')] Estatus");
            psbQuery.AppendLine("where Estatus.dtIniVigencia <> Estatus.dtFinVigencia");
            psbQuery.AppendLine("and Estatus.vchCodigo = '" + lsCodEstatus.Replace("'", "''") + "'");

            return DSODataAccess.ExecuteScalar(psbQuery.ToString());
        }

        /// <summary>
        /// Extraer el iCodCatalogo del cliente que tiene relacionado la Empresa a la cual pertenece el Usuario 
        /// </summary>
        /// <param name="liCodUsuario">iCodCatalogo del Usuario</param>
        /// <param name="ldtVigencia">Fecha de inicio de vigencia</param>
        /// <returns>Entero con valor del resultado de la consulta</returns>
        protected static int GetClienteUsuario(int liCodUsuario, DateTime ldtVigencia)
        {
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("select Empre.Client");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre,");
            lsbQuery.AppendLine(DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','" + Globals.GetCurrentLanguage() + "')] Usuar");
            lsbQuery.AppendLine("where Empre.iCodCatalogo = Usuar.Empre");
            lsbQuery.AppendLine("and Usuar.iCodCatalogo = " + liCodUsuario);
            lsbQuery.AppendLine("and Empre.dtIniVigencia <> Empre.dtFinVigencia");
            lsbQuery.AppendLine("and Empre.dtIniVigencia <= '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Empre.dtFinVigencia > '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Usuar.dtIniVigencia <> Usuar.dtFinVigencia");
            lsbQuery.AppendLine("and Usuar.dtIniVigencia <= '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Usuar.dtFinVigencia > '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            return (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString());
        }

        protected virtual void EliminarConferenciaNoAgregada()
        {
            //Eliminar todos los datos que se guardaron en la base de datos antes de que el usuario le diera clic en cancelar

            //TODO: Eliminar datos de los participantes

            //Elimino la conferencia ya que se dio clic en cancelar sin grabar la conferencia
            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            pvchDescripcion.DataValue = "Cancelar";
            pdtFinVigencia.DataValue = pdtIniVigencia.Date;
            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();
            base.GrabarRegistro();
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            if (State == CnfgConferenciasState.AgregandoConferencia
                || State == CnfgConferenciasState.Edicion)
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select NumParticipantes = count(*) from " + DSODataContext.Schema + ".[VisHistoricos('Participante','Participante','" + Globals.GetCurrentLanguage() + "')] P");
                psbQuery.AppendLine("where P.dtIniVigencia <> P.dtFinVigencia");
                psbQuery.AppendLine("and P.TMSConf = " + iCodCatalogo);

                int liNumParticipantes = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString(), 0);
                pbtnGrabar.Disabled = (liNumParticipantes < 3);
            }

            if (pFields != null)
            {
                pFields.GetByConfigName("EstConferencia").DisableField();
            }
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

        #region IPostBackEventHandler Members
        public override void RaisePostBackEvent(string eventArgument)
        {
            base.RaisePostBackEvent(eventArgument);
            if (eventArgument == "btnGrabar"
                && State == CnfgConferenciasState.AgregandoConferencia)
            {
                pbtnGrabar_ServerClick(pbtnGrabar, new EventArgs());
            }
        }
        #endregion

        #region WebMethods
        public static string SearchSeeYouOnServicioConferencia(string term, int iCodCliente, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();
                StringBuilder lsbQuery = new StringBuilder();

                if (iCodCliente == 0)
                {
                    int liCodUsuario = (int)HttpContext.Current.Session["iCodUsuario"];
                    iCodCliente = GetClienteUsuario(liCodUsuario, DSOControl.ParseDateTimeJS(iniVigencia, true));
                }

                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select iCodRegistro,iCodCatalogo,vchCodigo,vchDescripcion from " + DSODataContext.Schema + ".[VisHistoricos('ServicioSeeYouOn','Paquete','" + lsLang + "')] P");
                lsbQuery.AppendLine("where P.dtIniVigencia <> P.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "P"));
                lsbQuery.AppendLine("and P.ContratoSeeYouOn in(select C.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('ContratoSeeYouOn','Contrato','" + lsLang + "')] C");
                lsbQuery.AppendLine("   where C.Client = " + iCodCliente);
                lsbQuery.AppendLine("   and C.dtIniVigencia <> C.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "C"));
                lsbQuery.AppendLine(")");
                lsbQuery.AppendLine("and P.vchDescripcion + ' (' + P.vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'");
                lsbQuery.AppendLine("union all");
                lsbQuery.AppendLine("select iCodRegistro,iCodCatalogo,vchCodigo,vchDescripcion from " + DSODataContext.Schema + ".[VisHistoricos('ServicioSeeYouOn','Sala Virtual','" + lsLang + "')] SV");
                lsbQuery.AppendLine("where SV.dtIniVigencia <> SV.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "SV"));
                lsbQuery.AppendLine("and SV.ContratoSeeYouOn in(select C.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('ContratoSeeYouOn','Contrato','" + lsLang + "')] C");
                lsbQuery.AppendLine("   where C.Client = " + iCodCliente);
                lsbQuery.AppendLine("   and C.dtIniVigencia <> C.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "C"));
                lsbQuery.AppendLine(")");
                lsbQuery.AppendLine("and SV.vchDescripcion + ' (' + SV.vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'");

                string lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from " + DSODataContext.Schema + ".Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.Append(")");
                lsbQuery.AppendLine("order by vchDescripcion");
                DataTable ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                // Modificación para quitar los paréntesis cuando es un cliente
                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);

                if (!(HttpContext.Current.Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin"))
                {
                    RemoverParentesisDSOAutoDataSource(lDataSource);
                }

                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static string SearchSeeYouOnProyectoConferencia(string term, int iCodCliente, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();
                StringBuilder lsbQuery = new StringBuilder();

                if (iCodCliente == 0)
                {
                    int liCodUsuario = (int)HttpContext.Current.Session["iCodUsuario"];
                    iCodCliente = GetClienteUsuario(liCodUsuario, DSOControl.ParseDateTimeJS(iniVigencia, true));
                }

                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select iCodRegistro,iCodCatalogo,vchCodigo,vchDescripcion from " + DSODataContext.Schema + ".[VisHistoricos('Proyecto','Proyecto','" + lsLang + "')] P");
                lsbQuery.AppendLine("where P.dtIniVigencia <> P.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "P"));
                lsbQuery.AppendLine("and P.Client in(" + iCodCliente + ")");
                lsbQuery.AppendLine("and P.vchDescripcion + ' (' + P.vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'");

                string lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from " + DSODataContext.Schema + ".Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.Append(")");
                lsbQuery.AppendLine("order by vchDescripcion");
                DataTable ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                // Modificación para quitar los paréntesis cuando es un cliente
                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);

                if (!(HttpContext.Current.Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin"))
                {
                    RemoverParentesisDSOAutoDataSource(lDataSource);
                }

                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static string SearchSeeYouOnPhoneBookContact(string term, int iCodConferencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();
                StringBuilder lsbQuery = new StringBuilder();

                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select C.Client from " + DSODataContext.Schema + ".[VisHistoricos('TMSConf','Conferencia','" + lsLang + "')] C");
                lsbQuery.AppendLine("where C.dtIniVigencia <= C.dtFinVigencia");
                lsbQuery.AppendLine("and C.dtIniVigencia <= GetDate()");
                lsbQuery.AppendLine("and C.dtFinVigencia > GetDate()");
                lsbQuery.AppendLine("and C.iCodCatalogo = " + iCodConferencia);

                int liCodCliente = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString(), 0);


                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select top 100 PhoneBookAddress.iCodRegistro, iCodCatalogo = PhoneBookAddress.iCodCatalogo,vchCodigo = PhoneBookAddress.AddressDesc ,vchDescripcion = PhoneBookContact.vchDescripcion");
                lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PhoneBookAddress','PhoneBookAddress','" + lsLang + "')] PhoneBookAddress,");
                lsbQuery.AppendLine("   " + DSODataContext.Schema + ".[VisHistoricos('TMSPhoneBookContact','PhoneBook','" + lsLang + "')] PhoneBookContact,");
                lsbQuery.AppendLine("   " + DSODataContext.Schema + ".[VisHistoricos('TMSPhoneBookFolder','TMS Phone Book Folder','" + lsLang + "')] PhoneBookFolder");
                lsbQuery.AppendLine("where PhoneBookAddress.dtIniVigencia <> PhoneBookAddress.dtFinVigencia");
                lsbQuery.AppendLine("and PhoneBookAddress.dtIniVigencia <= GetDate()");
                lsbQuery.AppendLine("and PhoneBookAddress.dtFinVigencia > GetDate()");
                lsbQuery.AppendLine("and PhoneBookContact.dtIniVigencia <> PhoneBookContact.dtFinVigencia");
                lsbQuery.AppendLine("and PhoneBookContact.dtIniVigencia <= GetDate()");
                lsbQuery.AppendLine("and PhoneBookContact.dtFinVigencia > GetDate()");
                lsbQuery.AppendLine("and PhoneBookFolder.dtIniVigencia <> PhoneBookFolder.dtFinVigencia");
                lsbQuery.AppendLine("and PhoneBookFolder.dtIniVigencia <= GetDate()");
                lsbQuery.AppendLine("and PhoneBookFolder.dtFinVigencia > GetDate()");
                lsbQuery.AppendLine("and PhoneBookAddress.TMSPhoneBookContact = PhoneBookContact.iCodCatalogo");
                lsbQuery.AppendLine("and PhoneBookContact.TMSPhoneBookFolder = PhoneBookFolder.iCodCatalogo");
                lsbQuery.AppendLine("and PhoneBookFolder.Client = " + liCodCliente);
                //lsbQuery.AppendLine("and PhoneBookAddress.OrdenPre = 1");
                lsbQuery.AppendLine("and PhoneBookContact.vchDescripcion + ' (' + PhoneBookAddress.AddressDesc + ')'  like '%" + term.Replace("'", "''") + "%'");
                lsbQuery.AppendLine("order by PhoneBookContact.vchDescripcion, PhoneBookAddress.AddressDesc");

                DataTable ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static DSOGridServerResponse GetSeeYouOnConferencias(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                DateTime ldtVigencia = DateTime.Now;
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodEntidad, iCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();

                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();
                string lsMaestro = DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + iCodMaestro).ToString();

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                string[] larrColumns = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = larrColumns[gsRequest.iSortCol[0]];

                    if ((!lFields.ContainsConfigName(lsOrderCol)
                        || (lsOrderCol.EndsWith("Desc")
                            && !lFields.ContainsConfigName(lsOrderCol.Substring(0, lsOrderCol.Length - 4))))
                        && lsOrderCol != "vchCodigo"
                        && lsOrderCol != "vchDescripcion"
                        && lsOrderCol != "dtIniVigencia"
                        && lsOrderCol != "dtFinVigencia"
                        && lsOrderCol != "Consultar")
                    {
                        lsOrderCol = "dtFecUltAct";
                    }

                    switch (gsRequest.sSortDir[0].ToLower())
                    {
                        case "desc":
                            lsOrderDir = " desc";
                            lsOrderDirInv = " asc";
                            break;
                        default:
                            lsOrderDir = " asc";
                            lsOrderDirInv = " desc";
                            break;
                    }
                }
                else
                {
                    lsOrderCol = "dtIniVigencia";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lsbColumnas.AppendLine("iCodRegistro");
                lsbColumnas.AppendLine(",Consultar = null");
                lsbColumnas.AppendLine(",vchCodigo");
                lsbColumnas.AppendLine(",vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && larrColumns.Contains<string>(lField.ConfigName + "Desc"))
                    {
                        lsbColumnas.AppendLine(",[" + lField.ConfigName + "Desc]");
                    }
                    else if (larrColumns.Contains<string>(lField.ConfigName))
                    {
                        lsbColumnas.AppendLine(",[" + lField.ConfigName + "]");
                    }
                }

                lsbColumnas.AppendLine(",dtIniVigencia");
                lsbColumnas.AppendLine(",dtFinVigencia");
                lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from " + DSODataContext.Schema + ".[VisHistoricos('" + lsEntidad + "','" + lsMaestro + "','" + Globals.GetCurrentLanguage() + "')] a");
                lsbFrom.AppendLine("      where dtIniVigencia <> dtFinVigencia");

                if (ldtVigencia != null)
                {
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, false).ToString("yyyy-MM-dd HH:mm:ss") + "' >= dtIniVigencia");
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, false).ToString("yyyy-MM-dd HH:mm:ss") + "' < dtFinVigencia");
                }

                if (HttpContext.Current.Session["vchCodPerfil"].ToString() != "SeeYouOnAdmin")
                {
                    int liCodUsuario = (int)HttpContext.Current.Session["iCodUsuario"];
                    int liCodCliente = GetClienteUsuario(liCodUsuario, DSOControl.ParseDateTimeJS(ldtVigencia, false));
                    lsbFrom.AppendLine("      and Client = " + liCodCliente);
                    lsbFrom.AppendLine("      and EstConferencia in(select Estatus.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('EstConferencia','Estatus','" + Globals.GetCurrentLanguage() + "')] Estatus");
                    lsbFrom.AppendLine("            where Estatus.dtIniVigencia <> Estatus.dtFinVigencia");
                    lsbFrom.AppendLine("            and Estatus.vchCodigo in('Programando','Programada','Reprogramando','Reprogramada'))");
                }

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");


                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(a.vchCodigo,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(a.vchDescripcion,'')");

                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlTimeFormat + "),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlTimeFormat + "),'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (larrColumns.Contains<string>(lField.ConfigName)
                            || larrColumns.Contains<string>(lField.ConfigName + "Desc"))
                        {
                            if (lField.Column.StartsWith("Date"))
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]," + lsSqlTimeFormat + "),'')");
                            }
                            else if (lField.Column.StartsWith("iCodCatalogo"))
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "Desc]),'')");
                            }
                            else if (!lField.Column.StartsWith("VarChar"))
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]),'')");
                            }
                            else
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(a.[" + lField.ConfigName + "],'')");
                            }
                        }
                    }
                    lsbWhere.AppendLine("and " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                }

                int lidx;
                for (lidx = 0; lidx < larrColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = larrColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.ContainsConfigName(lsColumn)
                            || (lsColumn.EndsWith("Desc") && lFields.ContainsConfigName(lsColumn.Substring(0, lsColumn.Length - 4)))
                            || lsColumn == "vchCodigo"
                            || lsColumn == "vchDescripcion"
                            || lsColumn == "dtIniVigencia"
                            || lsColumn == "dtFinVigencia"))
                        {
                            lsbWhere.Append("and ");

                            if ((lFields.ContainsConfigName(lsColumn)
                                    && lFields.GetByConfigName(lsColumn) is KeytiaDateTimeField)
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a.[" + lsColumn + "], " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a.[" + lsColumn + "], " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a.[" + lsColumn + "] like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom);
                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFrom + lsWhere + lsOrderBy);

                if (!(HttpContext.Current.Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin"))
                {
                    RemoverParentesisGrid(ldt, "ProyectoDesc");
                }

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt, true);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        #endregion

        public static void RemoverParentesisDSOAutoDataSource(DataTable ldtTable)
        {
            string lsValue = "";

            foreach (DataRow ldrRow in ldtTable.Rows)
            {
                lsValue = ldrRow["value"].ToString();
                if (lsValue.IndexOf("(") >= 0)
                {
                    lsValue = lsValue.Substring(0, lsValue.IndexOf("("));
                    lsValue = lsValue.Trim();
                    ldrRow["value"] = lsValue;
                }
            }
        }

        public static void RemoverParentesisGrid(DataTable ldtTable, string lsColumna)
        {
            if (!ldtTable.Columns.Contains(lsColumna))
            {
                return;
            }

            string lsValue = "";
            foreach (DataRow ldrRow in ldtTable.Rows)
            {
                lsValue = ldrRow[lsColumna].ToString();
                if (lsValue.IndexOf("(") >= 0)
                {
                    lsValue = lsValue.Substring(0, lsValue.IndexOf("("));
                    lsValue = lsValue.Trim();
                    ldrRow[lsColumna] = lsValue;
                }
            }
        }

        public static void RemoverParentesisGrid(DataTable ldtTable, string[] lsColumnas)
        {
            foreach (string lsColumna in lsColumnas)
            {
                RemoverParentesisGrid(ldtTable, lsColumna);
            }
        }

        public static void RemoverParentesisGrid(DataTable ldtTable)
        {
            foreach (DataColumn ldColumn in ldtTable.Columns)
            {
                RemoverParentesisGrid(ldtTable, ldColumn.ColumnName);
            }
        }

    }
}
