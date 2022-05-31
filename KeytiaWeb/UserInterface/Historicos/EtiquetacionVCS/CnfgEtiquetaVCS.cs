/*
Nombre:		    DMM
Fecha:		    20120621
Descripción:	Aplicación de Etiquetación de SeeYouOn.
*/
using System;
using System.Web;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.SessionState;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Net.Mail;

namespace KeytiaWeb.UserInterface
{
    public class CnfgEtiquetaVCS : HistoricEdit
    {

        protected bool pbEnableEmpleado = false;

        public CnfgEtiquetaVCS()
        {
            Init += new EventHandler(CnfgEtiquetaVCS_Init);
        }

        protected virtual void CnfgEtiquetaVCS_Init(object sender, EventArgs e)
        {
            SetEmpleado();

            if (!pbEnableEmpleado)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "UsuarioSinEmpleado"));
                DSOControl.jAlert(Page, pjsObj, lsError, lsTitulo);
            }

            this.CssClass = "CnfgEtiquetaVCS";
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgEtiquetaVCS.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/EtiquetacionVCS/CnfgEtiquetaVCS.js?V=1") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected virtual void SetEmpleado()
        {
            DataTable ldt;
            if (Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin")
            {
                pbEnableEmpleado = true;
                return;
            }

            ldt = pKDB.GetHisRegByEnt("Emple", "Empleados", "{Usuar}='" + Session["iCodUsuario"].ToString() + "'");

            if (ldt == null || ldt.Rows.Count == 0)
            {
                pbEnableEmpleado = false;
                return;
            }
            iCodRegistro = ldt.Rows[0]["iCodRegistro"].ToString();
            string iCodEmpleado = ldt.Rows[0]["iCodCatalogo"].ToString();
            if (iCodRegistro != null && iCodEmpleado != null && iCodRegistro != "" && iCodEmpleado != "")
            {
                pbEnableEmpleado = true;
            }
            else
            {
                pbEnableEmpleado = false;
            }
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields.ContainsConfigName("TMSSystems"))
            {
                DSOAutocomplete lAuto = (DSOAutocomplete)((KeytiaAutoCompleteField)pFields.GetByConfigName("TMSSystems")).DSOControlDB;
                lAuto.OnSelect = "function(dateText, inst) {" + pjsObj + ".fnInitGrids();}";
            }
            if (pFields.ContainsConfigName("FechaInicio"))
            {
                DSODateTimeBox lFechaIni = (DSODateTimeBox)((KeytiaDateTimeField)pFields.GetByConfigName("FechaInicio")).DSOControlDB;
                lFechaIni.OnSelect = "function(dateText, inst) {" + pjsObj + ".fnInitGrids();}";
            }
            if (pFields.ContainsConfigName("FechaFin"))
            {
                DSODateTimeBox lFechaFin = (DSODateTimeBox)((KeytiaDateTimeField)pFields.GetByConfigName("FechaFin")).DSOControlDB;
                lFechaFin.OnSelect = "function(dateText, inst) {" + pjsObj + ".fnInitGrids();}";
            }
        }

        public override void CreateControls()
        {
            base.CreateControls();
            ReporteEstandar.InitValoresSession();
            if (pFields.ContainsConfigName("FechaInicio"))
            {
                pFields.GetByConfigName("FechaInicio").DataValue = (DateTime)Session["FechaIniRep"];
            }
            if (pFields.ContainsConfigName("FechaFin"))
            {
                pFields.GetByConfigName("FechaFin").DataValue = ((DateTime)Session["FechaFinRep"]).AddDays(1);
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            if (s != HistoricState.CnfgSubHistoricField)
            {
                s = HistoricState.Edicion;
            }

            base.SetHistoricState(s);

            if (s == HistoricState.Edicion)
            {
                pbtnGrabar.Visible = false;
                pbtnCancelar.Visible = false;
                pExpFiltros.Visible = false;
                pExpRegistro.Visible = false;
                pbtnConsultar.Visible = false;
                pbtnAgregar.Visible = false;
                pbtnEditar.Visible = false;
                pbtnBaja.Visible = false;
                if (pbEnableEmpleado)
                {
                    pExpAtributos.Visible = true;
                    pPanelSubHistoricos.Visible = true;
                }
                else
                {
                    pExpAtributos.Visible = false;
                    pPanelSubHistoricos.Visible = false;
                }
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pExpAtributos.Title = Globals.GetMsgWeb("HistoricFilterTitle");
            pExpAtributos.ToolTip = Globals.GetMsgWeb("HistoricFilterTitle");
        }

        #region IPostBackEventHandler Members

        public override void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "btnGrabar"
                && (State == HistoricState.Edicion
                || State == HistoricState.Baja))
            {
                pbtnGrabar_ServerClick(pbtnGrabar, new EventArgs());
            }
            else if (eventArgument == "btnCancelar")
            {
                pbtnCancelar_ServerClick(pbtnCancelar, new EventArgs());
            }
            else if (eventArgument.StartsWith("btnConsultar"))
            {
                int liCodRegistro;
                if (eventArgument.Split(':').Length == 2
                    && int.TryParse(eventArgument.Split(':')[1], out liCodRegistro))
                {
                    iCodRegistro = liCodRegistro.ToString();
                    pbtnConsultar_ServerClick(pbtnConsultar, new EventArgs());
                }
            }
            else if (eventArgument.StartsWith("btnEditarSubHis"))
            {
                int liCodRegistro;
                int liConfigValue;
                if (eventArgument.Split(':').Length == 3
                    && int.TryParse(eventArgument.Split(':')[1], out liCodRegistro)
                    && int.TryParse(eventArgument.Split(':')[2], out liConfigValue))
                {
                    KeytiaBaseField lField = pFields.GetByConfigValue(liConfigValue);
                    PrevState = State;
                    SubHistoricClass = lField.SubHistoricClass;
                    SubCollectionClass = lField.SubCollectionClass;

                    InitSubHistorico(this.ID + "EditarEnt" + iCodRegistro);

                    pSubHistorico.SetEntidad("EtiquetaVCS");
                    pSubHistorico.SetMaestro("DetalleVCS");

                    DataTable lKDBTable = pKDB.GetHisRegByEnt("MsgWeb", "Mensajes Web", " vchDescripcion = '" + pSubHistorico.vchDesMaestro + "'");
                    if (lKDBTable.Rows.Count > 0)
                    {
                        pSubHistorico.vchCodTitle = lKDBTable.Rows[0]["vchCodigo"].ToString();
                    }

                    pSubHistorico.EsSubHistorico = true;
                    pSubHistorico.FillControls();

                    SetHistoricState(HistoricState.CnfgSubHistoricField);
                    pSubHistorico.InitMaestro();

                    pSubHistorico.iCodRegistro = liCodRegistro.ToString();
                    pSubHistorico.ConsultarRegistro();

                    pSubHistorico.Fields.EnableFields();
                    pSubHistorico.SetHistoricState(HistoricState.Edicion);
                    if (pSubHistorico.Fields.ContainsConfigName(vchCodEntidad))
                    {
                        pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DataValue = iCodCatalogo;
                        pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DisableField();
                    }
                    pSubHistorico.PostEditarSubHistoricField();
                }
            }
            else if (eventArgument.StartsWith("btnEliminarSubHis")) //@@rrh
            {
                int liCodRegistro;
                int liConfigValue;
                if (eventArgument.Split(':').Length == 3
                    && int.TryParse(eventArgument.Split(':')[1], out liCodRegistro)
                    && int.TryParse(eventArgument.Split(':')[2], out liConfigValue))
                {
                    KeytiaBaseField lField = pFields.GetByConfigValue(liConfigValue);
                    DataTable dtMaestro = DSODataAccess.Execute(@"select  c.vchcodigo,
	                                                                      m.vchdescripcion 
                                                                  from historicos h
                                                                  join maestros m
                                                                  on h.icodmaestro = m.icodregistro
                                                                  and m.dtIniVigencia <> m.dtFinVigencia
                                                                  join catalogos c
                                                                  on c.icodregistro = m.icodentidad
                                                                  where h.icodregistro = " + liCodRegistro);

                    PrevState = State;
                    SubHistoricClass = lField.SubHistoricClass;
                    SubCollectionClass = lField.SubCollectionClass;

                    InitSubHistorico(this.ID + "EditarEnt" + iCodRegistro);

                    pSubHistorico.SetEntidad((dtMaestro.Rows[0]["vchcodigo"]).ToString());
                    pSubHistorico.SetMaestro((dtMaestro.Rows[0]["vchdescripcion"]).ToString());

                    DataTable lKDBTable = pKDB.GetHisRegByEnt("MsgWeb", "Mensajes Web", " vchDescripcion = '" + pSubHistorico.vchDesMaestro + "'");
                    if (lKDBTable.Rows.Count > 0)
                    {
                        pSubHistorico.vchCodTitle = lKDBTable.Rows[0]["vchCodigo"].ToString();
                    }

                    pSubHistorico.EsSubHistorico = true;
                    pSubHistorico.FillControls();

                    SetHistoricState(HistoricState.CnfgSubHistoricField);
                    pSubHistorico.InitMaestro();

                    pSubHistorico.iCodRegistro = liCodRegistro.ToString();
                    pSubHistorico.ConsultarRegistro();

                    pSubHistorico.Fields.DisableFields();
                    pSubHistorico.SetHistoricState(HistoricState.Baja);
                    pSubHistorico.dtFinVigencia.DataValue = DateTime.Today;

                    if (pSubHistorico.Fields.ContainsConfigName(vchCodEntidad))
                    {
                        pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DataValue = iCodCatalogo;
                        pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DisableField();
                    }
                    pSubHistorico.PostEliminarSubHistoricField();
                }
            }
        }

        #endregion

        #region WebMethods
        public static DSOGridServerResponse GetSubHisDataEtiquetaVCS(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return GetSubHisDataEtiquetaVCS(gsRequest, iCodEntidad, iCodMaestro, DateTime.Now, parametros);
        }

        public static DSOGridServerResponse GetSubHisDataEtiquetaVCS(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, object ldtVigencia, List<Parametro> parametros)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodEntidad, iCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();

                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if (!lFields.Contains(lsOrderCol)
                        && lsOrderCol != "Editar")
                    {
                        lsOrderCol = "FechaInicio";
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
                    lsOrderCol = "FechaInicio";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lsbColumnas.AppendLine("DetalleVCSSystem.iCodRegistro");
                lsbColumnas.AppendLine(",Editar = null");
                lsbColumnas.AppendLine(",FechaInicio = Convert(varchar,'')");
                lsbColumnas.AppendLine(",FechaInicioDate = DetalleVCS.FechaInicio");
                lsbColumnas.AppendLine(",DuracionSeg = Convert(varchar,'')");
                lsbColumnas.AppendLine(",DuracionSegInt = DetalleVCS.DuracionSeg");
                lsbColumnas.AppendLine(",VCSSourceSystemDesc      = IsNull(DetalleVCS.VCSSourceSystemDesc,      '') + ' (' + IsNull(DetalleVCS.VCSSourceNumberDesc,      '') + ')'");
                lsbColumnas.AppendLine(",VCSDestinationSystemDesc = IsNull(DetalleVCS.VCSDestinationSystemDesc, '') + ' (' + IsNull(DetalleVCS.VCSDestinationNumberDesc, '') + ')'");
                lsbColumnas.AppendLine(",DetalleVCSSystem.ProyectoDesc");
                lsbColumnas.AppendLine(",DetalleVCSSystem.TipoConferenciaDesc");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from [VisDetallados('Detall','DetalleVCS','Español')] DetalleVCS,");
                lsbFrom.AppendLine("           [VisDetallados('Detall','DetalleVCSSystem','Español')] DetalleVCSSystem");
                lsbFrom.AppendLine("      where DetalleVCS.iCodCatalogo = DetalleVCSSystem.iCodCatalogo");
                lsbFrom.AppendLine("      and DetalleVCS.RegCarga = DetalleVCSSystem.RegCarga");

                if (parametros != null)
                {
                    foreach (Parametro lParam in parametros)
                    {
                        if (lParam.Value == null)
                        {
                            lParam.Value = "null";
                        }
                        string lsFiltro = lParam.Value.ToString().Replace("'", "''").Trim();

                        string lsColumna = lParam.Name;
                        if (lsColumna != null)
                        {
                            if (lsColumna == "FechaInicio")
                            {
                                DateTime aux;
                                if (DateTime.TryParseExact(lParam.Value.ToString(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out aux))
                                {
                                    lsbFrom.AppendLine("and DetalleVCS.FechaInicio >= '" + DateTime.SpecifyKind(aux, DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                }
                                else if (DateTime.TryParseExact(lParam.Value.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out aux))
                                {
                                    lsbFrom.AppendLine("and DetalleVCS.FechaInicio >= '" + aux.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                }
                            }
                            else if (lsColumna == "FechaFin")
                            {
                                DateTime aux;
                                if (DateTime.TryParseExact(lParam.Value.ToString(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out aux))
                                {
                                    lsbFrom.AppendLine("and DetalleVCS.FechaInicio <  '" + DateTime.SpecifyKind(aux, DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                }
                                else if (DateTime.TryParseExact(lParam.Value.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out aux))
                                {
                                    lsbFrom.AppendLine("and DetalleVCS.FechaInicio <  '" + aux.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                }
                            }
                            else if (lsColumna == "TMSSystems")
                            {
                                lsbFrom.AppendLine("and DetalleVCSSystem.VCSSourceSystem = " + lsFiltro);
                                lsbFrom.AppendLine("and " + lsFiltro + " in(DetalleVCS.VCSSourceSystem, DetalleVCS.VCSDestinationSystem)");
                            }
                        }
                    }
                }

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();

                    lsbColTodas.AppendLine("isnull(convert(varchar,DetalleVCS.FechaInicio," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,DetalleVCS.FechaInicio," + lsSqlTimeFormat + "),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,DetalleVCS.DuracionSeg),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(DetalleVCS.VCSSourceSystemDesc,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(DetalleVCS.VCSSourceNumberCod,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(DetalleVCS.VCSDestinationSystemDesc,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(DetalleVCS.VCSDestinationNumberCod,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(DetalleVCSSystem.ProyectoDesc,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(DetalleVCSSystem.TipoConferenciaDesc,'')");

                    lsbWhere.AppendLine("and " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                }

                if (lsOrderCol == "Proyecto" ||
                    lsOrderCol == "TipoConferencia")
                {
                    lsbOrderBy.AppendLine("       order by DetalleVCSSystem." + lsOrderCol + lsOrderDir + ", DetalleVCSSystem.iCodRegistro" + lsOrderDir);
                }
                else
                {
                    lsbOrderBy.AppendLine("       order by DetalleVCS." + lsOrderCol + lsOrderDir + ", DetalleVCSSystem.iCodRegistro" + lsOrderDir);
                }
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);


                string lsSelectCount = "select count(DetalleVCSSystem.iCodRegistro) ";
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

                string lsDateTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateTimeFormat");                
                foreach (DataRow ldr in ldt.Rows)
                {
                    ldr["FechaInicio"] = ((DateTime)ldr["FechaInicioDate"]).ToString(lsDateTimeFormat);
                    ldr["DuracionSeg"] = KeytiaServiceBL.Reportes.ReporteEstandarUtil.TimeToString(ldr["DuracionSegInt"],Globals.GetCurrentLanguage());
                }

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }
        #endregion

    }
}
