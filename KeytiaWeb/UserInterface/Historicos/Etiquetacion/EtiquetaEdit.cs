
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
using KeytiaWeb.UserInterface.DashboardFC;
using System.Linq;

namespace KeytiaWeb.UserInterface
{
    public class EtiquetaState : HistoricState
    {
        public static readonly HistoricState EtiquetaDir = NewState(7, "EtiquetaDir");
    }

    public class EtiquetaEdit : HistoricEdit
    {
        #region Campos

        protected bool pbEnableEmpleado = false;
        protected bool pbEnablePeriodo = false;
        protected bool pbEtiquetacionCorrecta = false;
        protected bool pbEtiquetacionColaborador = true;

        //Proceso Etiquetación
        protected DSOExpandable pExpResumen;
        protected Table pTablaResumen;
        protected DSOGrid pResumenGrid;
        protected HistoricFieldCollection pFieldsResumen;

        protected string[] pFieldsNoVisibles;
        protected DataTable pdtResumenConsultado;
        protected int piNumEtiquetaPeriodo;
        protected int piProcesoEtiqueta;
        protected MailAccess poMail;

        //Exportar
        protected string psTempPath;
        protected string psFileKey;
        protected string psLogoClientePath;
        protected string psLogoKeytiaPath;

        //NZ 20150825
        protected static DateTime fechaInicioReal;
        protected static DateTime fechaFinReal;

        //Directorios
        protected EventHandler pPostDirectorioClick;
        protected HtmlButton pbtnDirPersonal;
        protected HtmlButton pbtnDirCorporativo;
        protected bool pbDirCorporativo = false;

        //Filtros
        protected KeytiaDropDownOptionField piCodGrupo;
        protected DataTable pdtGrupoCol = new DataTable();
        protected DSOCheckBox pbPersonales;
        protected DSOCheckBox pbLaborales;
        //NZ 201508112 Se incluye filtro por Tipo Destino
        protected KeytiaDropDownOptionField piCodTDest;

        //Totales
        protected DSONumberEdit pNumTotPersonal;
        protected DSONumberEdit pNumTotLaboral;
        protected DSONumberEdit pNumTotNI;
        protected DSONumberEdit pNumTotal;
        protected DSOTextBox pTxtNumTotPServer;
        protected DSOTextBox pTxtNumTotLServer;
        protected DSOTextBox pTxtNumTotNServer;
        protected DSOTextBox pTxtNumTotServer;

        //Detalles
        protected HtmlButton pbtnVerDetalle;
        protected DSOWindow pwndDetalle;
        protected EtiquetaDetalle pEtqDetalle;
        protected DSOWindow pwndDetLinea;
        protected EtiquetaDetalle pEtqDetLinea;
        protected DSOTextBox pTxtWindowVisible;

        //Configuración Cliente
        protected int piDiaCorte;
        protected int piDiaLimite;
        protected int piLongEtiqueta;
        protected bool pbColEtiqueta;
        protected int piHisPreviaEtiqueta;
        protected static bool pbEtiquetarUnaVez;
        protected static bool pbNuevaEtiquetacion;
        protected static bool pbCheckTodasLaborales;
        protected static bool pbNoEtiquetaEnBlanco;
        protected bool pbEtiquetarTodos;
        protected string psMailPara = "";
        protected string psMailCC = "";
        protected string psMailCCO = "";
        protected string psMailRemitente = "";
        protected string psNomRemitente = "";
        protected DateTime pdtIniPeriodoInicial;
        protected DateTime pdtFinPeriodoInicial;
        protected DateTime pdtActual = DateTime.Today;
        protected static DateTime pdtCorte;
        protected static DateTime pdtLimite;
        //NZ
        protected static bool pbEtiquetarIncluirLlamadasEntrada;

        #endregion


        #region Propiedades

        public override string iCodEntidad
        {
            get
            {
                return (string)ViewState["iCodEntidad"];
            }
            protected set
            {
                ViewState["iCodEntidad"] = value;

                //Cuando se borre iCodEntidad que tambien se borre vchCodEntidad
                if (String.IsNullOrEmpty(value))
                {
                    vchCodEntidad = value;
                }
            }
        }


        public override string iCodMaestro
        {
            get
            {
                return (string)ViewState["iCodMaestro"];
            }
            protected set
            {
                ViewState["iCodMaestro"] = value;

                if (String.IsNullOrEmpty(value))
                {
                    //Cuando se borre iCodMaestro que tambien se borre vchDesMaestro
                    vchDesMaestro = value;
                }
            }
        }

        protected virtual string iCodEmpleado
        {
            get
            {
                return (string)ViewState["iCodEmpleado"];
            }
            set
            {
                ViewState["iCodEmpleado"] = value;
            }
        }

        protected virtual string iCodEntidadResumen
        {
            get
            {
                return (string)ViewState["iCodEntidadResumen"];
            }
            set
            {
                ViewState["iCodEntidadResumen"] = value;
            }
        }


        protected virtual string iCodMaestroResumen
        {
            //Maestro de Grid Resumen de Etiquetacion
            get
            {
                return (string)ViewState["iCodMaestroResumen"];
            }
            set
            {
                ViewState["iCodMaestroResumen"] = value;
            }
        }

        public virtual string vchDesMaestroResumen
        {
            get
            {
                return (string)ViewState["vchDesMaestroResumen"];
            }
            protected set
            {
                ViewState["vchDesMaestroResumen"] = value;
            }
        }

        protected virtual string iCodEntidadEtq
        {
            get
            {
                return (string)ViewState["iCodEntidadEtq"];
            }
            set
            {
                ViewState["iCodEntidadEtq"] = value;
            }
        }

        protected virtual string iCodMaestroEtq
        {
            //Maestro de Datos de Etiquetacion
            get
            {
                return (string)ViewState["iCodMaestroEtq"];
            }
            set
            {
                ViewState["iCodMaestroEtq"] = value;
            }
        }

        protected DateTime IniPeriodo
        {
            //Maestro de Datos de Etiquetacion            
            get
            {
                if (pFields != null && pFields.ContainsConfigName("IniPer") && pFields.GetByConfigName("IniPer").DataValue.ToString() != "null")
                {
                    return DateTime.Parse(pFields.GetByConfigName("IniPer").DataValue.ToString().Replace("'", ""));
                }
                return DateTime.MinValue;
            }
            set
            {
                pFields.GetByConfigName("IniPer").DataValue = value;
            }
        }

        protected DateTime FinPeriodo
        {
            //Maestro de Datos de Etiquetacion
            get
            {
                if (pFields != null && pFields.ContainsConfigName("FinPer") && pFields.GetByConfigName("FinPer").DataValue.ToString() != "null")
                {
                    return DateTime.Parse(pFields.GetByConfigName("FinPer").DataValue.ToString().Replace("'", ""));
                }
                return DateTime.MinValue;
            }
            set
            {
                pFields.GetByConfigName("FinPer").DataValue = value;
            }
        }

        #endregion


        #region Contructores

        public EtiquetaEdit()
        {
            Init += new EventHandler(EtiquetaEdit_Init);

            psFileKey = Guid.NewGuid().ToString();
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), DSOUpload.EscapeFolderName(Session.SessionID));
            System.IO.Directory.CreateDirectory(psTempPath);

            vchDesMaestroResumen = "Resumen Etiquetacion Temp";
        }

        #endregion


        #region Metodos

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "EtiquetaEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Etiquetacion/EtiquetaEdit.js?V=1") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected virtual void SetEmpleado()
        {
            DataTable ldt;
            ldt = pKDB.GetHisRegByEnt("Emple", "Empleados", "{Usuar}='" + Session["iCodUsuario"].ToString() + "'");

            if (ldt == null || ldt.Rows.Count == 0)
            {
                pbEnableEmpleado = false;
                return;
            }
            iCodRegistro = ldt.Rows[0]["iCodRegistro"].ToString();
            iCodEmpleado = ldt.Rows[0]["iCodCatalogo"].ToString();
            if (iCodRegistro != null && iCodEmpleado != null && iCodRegistro != "" && iCodEmpleado != "")
            {
                pbEnableEmpleado = true;
            }
            else
            {
                pbEnableEmpleado = false;
            }
        }

        protected virtual void SetMaestros()
        {
            iCodEntidadEtq = DSODataAccess.Execute("Select iCodRegistro from Catalogos where vchCodigo='EtiquetaApp' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString();
            iCodMaestroEtq = DSODataAccess.Execute("Select iCodRegistro from Maestros where iCodEntidad = " + iCodEntidadEtq + " and vchDescripcion='Etiquetacion' and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString();

            iCodEntidadResumen = DSODataAccess.Execute("Select iCodRegistro from Catalogos where vchCodigo='Detall' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString();
            iCodMaestroResumen = DSODataAccess.Execute("Select iCodRegistro from Maestros where iCodEntidad = " + iCodEntidadResumen + " and vchDescripcion='" + vchDesMaestroResumen + "' and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString();
        }

        protected override void InitAcciones()
        {
            base.InitAcciones();
            InitAccionesToolBar();
        }

        protected virtual void InitAccionesToolBar()
        {
            pbtnDirPersonal.ID = "btnDirPersonal";
            pbtnDirCorporativo.ID = "btnDirCorporativo";
            pbtnVerDetalle.ID = "btnVerDetalle";

            pbtnDirPersonal.Attributes["class"] = "buttonEdit btnDirPersonal";
            pbtnDirCorporativo.Attributes["class"] = "buttonEdit btnDirCorporativo";
            pbtnVerDetalle.Attributes["class"] = "buttonSearch btnVerDetalle";

            pbtnDirPersonal.Style["display"] = "none";
            pbtnDirCorporativo.Style["display"] = "none";
            pbtnVerDetalle.Style["display"] = "none";

            pbtnDirPersonal.ServerClick += new EventHandler(pbtnDirPersonal_ServerClick);
            pbtnDirCorporativo.ServerClick += new EventHandler(pbtnDirCorporativo_ServerClick);

            string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "btnGrabar");
            pbtnGrabar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".grabarEtiquetas(function(){" + lsdoPostBack + "});return false;";
        }

        protected override void InitAccionesSecundarias()
        {
            InitRegistroResumen();
            CreateGridResumen();
            if (State == EtiquetaState.EtiquetaDir)
            {
                if (pPostCancelarClick == null)
                {
                    SubHistorico.PostCancelarClick += new EventHandler(pSubEtiqueta_PostCancelarClick);
                }
            }
        }

        protected virtual void FillFields()
        {
            DataTable lKDBTable = pKDB.GetHisRegByEnt(vchCodEntidad, vchDesMaestro, "iCodCatalogo = " + iCodEmpleado);
            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                DataRow lDataRow = lKDBTable.Rows[0];
                pFields.GetByConfigName("Emple").DataValue = lDataRow["iCodCatalogo"];
                pFields.GetByConfigName("CenCos").DataValue = lDataRow["{CenCos}"];
            }
            pFields.GetByConfigName("Instruc").DataValue = Globals.GetMsgWeb(false, "InstrucEtiqueta");
            if (pdtFinPeriodoInicial != DateTime.MinValue && pdtIniPeriodoInicial != DateTime.MinValue)
            {
                pFields.GetByConfigName("IniPer").DSOControlDB.AddClientEvent("monthDayValue", piDiaCorte.ToString());
                pFields.GetByConfigName("FinPer").DSOControlDB.AddClientEvent("monthDayValue", (piDiaCorte - 1).ToString());
                pFields.GetByConfigName("IniPer").DataValue = pdtIniPeriodoInicial;
                pFields.GetByConfigName("FinPer").DataValue = pdtFinPeriodoInicial;
                pbNuevaEtiquetacion = true;
            }
        }

        public override void FillAjaxControls(bool lbIncluirFechaFin)
        {
            //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
            if (State == HistoricState.Inicio)
            {
                piCodMaestro.DataSource = "select iCodRegistro, vchDescripcion from Maestros where iCodEntidad = " + piCodEntidad.DataValue.ToString() + " and dtIniVigencia <> dtFinVigencia order by vchDescripcion";
                piCodMaestro.Fill();

                if (!pbEnableMaestro)
                {
                    piCodMaestro.DataValue = iCodMaestro;
                    SetHistoricState(HistoricState.MaestroSeleccionado);
                    InitMaestro();
                }
            }

            if (pFields != null)
            {
                pFields.FillAjaxControls();
            }
        }

        protected virtual void CreateGridResumen()
        {
            pResumenGrid.ID = "ResumenGrid";
            pResumenGrid.CreateControls();
        }

        public override void InitMaestro()
        {
            PrevState = State;

            iCodRegistro = "null";
            InitFields();

            if (iCodMaestro != null)
            {
                if (iCodMaestro == iCodMaestroEtq)
                {
                    SetHistoricState(HistoricState.Edicion);
                    FillFields();
                    DisableFields();
                    if (pbEnableEmpleado)
                    {
                        CreateGridResumen();
                        InitGridResumen();
                    }
                }
                else
                {
                    SetHistoricState(HistoricState.MaestroSeleccionado);
                    pFields.FillControls();
                    pFields.DisableFields();

                }
                CreateGrid();
                InitGrid();
                InitFiltros();
            }
            else
            {
                iCodRegistro = "null";
                SetHistoricState(HistoricState.Inicio);
            }
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null && pFields.ContainsConfigName("IniPer") && pFields.ContainsConfigName("FinPer"))
            {
                pFields.GetByConfigName("IniPer").DSOControlDB.AddClientEvent(HtmlTextWriterAttribute.Onchange.ToString(), pjsObj + ".vigenciaTimeBox($(this));");
                pFields.GetByConfigName("FinPer").DSOControlDB.AddClientEvent(HtmlTextWriterAttribute.Onchange.ToString(), pjsObj + ".vigenciaTimeBox($(this));");
            }
            if (pFields != null && pFields.ContainsConfigName("Instruc"))
            {
                DSOTextBox ltxt = (DSOTextBox)pFields.GetByConfigName("Instruc").DSOControlDB;
                ltxt.TextBox.Height = 65;
            }
        }

        protected virtual void InitRegistroResumen()
        {
            int liRow = 1;
            pTablaResumen.ID = "TablaTotales";
            pTablaResumen.Width = Unit.Percentage(100);

            pExpResumen.ID = "ResumenWrapper";
            pExpResumen.StartOpen = true;
            pExpResumen.CreateControls();
            pExpResumen.Panel.Controls.Clear();
            pExpResumen.Panel.Controls.Add(pTablaResumen);

            liRow = 1;

            pNumTotPersonal.ID = "dTotPersonal";
            pNumTotPersonal.Table = pTablaResumen;
            pNumTotPersonal.Row = liRow++;
            pNumTotPersonal.ColumnSpan = 3;
            pNumTotPersonal.DataField = "dTotPersonal";
            pNumTotPersonal.CreateControls();
            pNumTotPersonal.NumberBox.ReadOnly = true;

            pNumTotLaboral.ID = "dTotLaboral";
            pNumTotLaboral.Table = pTablaResumen;
            pNumTotLaboral.Row = liRow++;
            pNumTotLaboral.ColumnSpan = 3;
            pNumTotLaboral.DataField = "dTotLaboral";
            pNumTotLaboral.CreateControls();
            pNumTotLaboral.NumberBox.ReadOnly = true;

            pNumTotNI.ID = "dTotNI";
            pNumTotNI.Table = pTablaResumen;
            pNumTotNI.Row = liRow++;
            pNumTotNI.ColumnSpan = 3;
            pNumTotNI.DataField = "dTotNI";
            pNumTotNI.CreateControls();
            pNumTotNI.NumberBox.ReadOnly = true;

            pNumTotal.ID = "dTotal";
            pNumTotal.Table = pTablaResumen;
            pNumTotal.Row = liRow++;
            pNumTotal.ColumnSpan = 3;
            pNumTotal.DataField = "dTotal";
            pNumTotal.CreateControls();
            pNumTotal.NumberBox.ReadOnly = true;

            //Controles con valores ocultos para trabajar ajax
            pTxtNumTotPServer.ID = "sTotPServer";
            pTxtNumTotPServer.CreateControls();
            pTxtNumTotPServer.TextBox.Style["display"] = "none";

            pTxtNumTotLServer.ID = "sTotLServer";
            pTxtNumTotLServer.CreateControls();
            pTxtNumTotLServer.TextBox.Style["display"] = "none";

            pTxtNumTotNServer.ID = "sTotNServer";
            pTxtNumTotNServer.CreateControls();
            pTxtNumTotNServer.TextBox.Style["display"] = "none";

            pTxtNumTotServer.ID = "sTotServer";
            pTxtNumTotServer.CreateControls();
            pTxtNumTotServer.TextBox.Style["display"] = "none";

            pTxtWindowVisible.ID = "txtWdVisible";
            pTxtWindowVisible.CreateControls();
            pTxtWindowVisible.TextBox.Style["display"] = "none";

            InitRegistroFiltros(liRow);
        }

        protected virtual void InitRegistroFiltros(int liRow)
        {
            //NZ 201508112 Se incluye filtro por Tipo Destino
            piCodTDest.Column = "iCodDestino";
            piCodTDest.Table = pTablaResumen;
            piCodTDest.Row = liRow;
            piCodTDest.ColumnSpan = 2;
            piCodTDest.Col = 1;
            piCodTDest.ConfigName = "FiltroTDest";
            piCodTDest.ConfigValue = (int)KeytiaServiceBL.Util.IsDBNull(pKDB.GetHisRegByEnt("Atrib", "Atributos", "vchCodigo = 'FiltroTDest'").Rows[0]["iCodCatalogo"], 0);
            piCodTDest.CreateField();
            piCodTDest.DSOControlDB.AddClientEvent(HtmlTextWriterAttribute.Onchange.ToString(), pjsObj + ".filtrarResumen();");
            piCodTDest.InitField();

            liRow++;
            //

            piCodGrupo.Column = "iCodGrupo";
            piCodGrupo.Table = pTablaResumen;
            piCodGrupo.Row = liRow;
            piCodGrupo.ColumnSpan = 2;
            piCodGrupo.Col = 1;
            piCodGrupo.ConfigName = "GEtiqueta";
            piCodGrupo.ConfigValue = (int)KeytiaServiceBL.Util.IsDBNull(pKDB.GetHisRegByEnt("Atrib", "Atributos", "vchCodigo = 'FiltroGEtiqueta'").Rows[0]["iCodCatalogo"], 0);
            piCodGrupo.CreateField();
            piCodGrupo.DSOControlDB.AddClientEvent(HtmlTextWriterAttribute.Onchange.ToString(), pjsObj + ".filtrarResumen();");
            piCodGrupo.InitField();
            piCodGrupo.InitDSOControlDBLanguage();

            //TODO: Establecer pdtGrupoCol con los valores de GEtiqueta                

            if (pbCheckTodasLaborales)
            {
                pbLaborales.ID = "bLaborales";
                pbLaborales.Table = pTablaResumen;
                pbLaborales.Row = liRow;
                pbLaborales.ColumnSpan = 1;
                pbLaborales.AddClientEvent(HtmlTextWriterAttribute.Onclick.ToString(), "javascript:" + pjsObj + ".fillTodasXGrupo($(this));");
                pbLaborales.DataField = "bLaborales";
                pbLaborales.CreateControls();
            }
            else
            {
                pbLaborales.ID = "bLaborales";
                pbLaborales.DataField = "bLaborales";
                pbLaborales.CreateControls();
                pbLaborales.CheckBox.Style["display"] = "none";
            }

            pbPersonales.ID = "bPersonales";
            pbPersonales.Table = pTablaResumen;
            pbPersonales.Row = liRow;
            pbPersonales.ColumnSpan = 1;
            pbPersonales.AddClientEvent(HtmlTextWriterAttribute.Onclick.ToString(), "javascript:" + pjsObj + ".fillTodasXGrupo($(this));");
            pbPersonales.DataField = "bPersonales";
            pbPersonales.CreateControls();

            pwndDetalle.ID = "wdVerDetalle";
            pwndDetalle.Width = 950;
            pwndDetalle.Height = 550;
            pwndDetalle.PositionLeft = 150;
            pwndDetalle.PositionTop = 20;
            pwndDetalle.Modal = true;
            pwndDetalle.InitOnReady = false;
            pwndDetalle.CreateControls();
            pwndDetalle.Resizeable = false;
            pwndDetalle.OnWindowClose = "function(){ " + pjsObj + ".End($('#" + pwndDetalle.Content.ClientID + "')); }";

            pwndDetLinea.ID = "wdVerDetLinea";
            pwndDetLinea.Width = 650;
            pwndDetLinea.Height = 570;
            pwndDetLinea.PositionLeft = 150;
            pwndDetLinea.PositionTop = 20;
            pwndDetLinea.Modal = true;
            pwndDetLinea.InitOnReady = false;
            pwndDetLinea.CreateControls();
            pwndDetLinea.Resizeable = false;
            pwndDetLinea.OnWindowClose = "function(){ " + pjsObj + ".End($('#" + pwndDetalle.Content.ClientID + "')); }";

            pbtnVerDetalle.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".consultaDetalle($('#" + pwndDetalle.Content.ClientID + "')); return false;";
        }

        protected override void InitGrid()
        {
            base.InitGrid();
            pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback, iCodEmpleado){" + pjsObj + ".fnServerDataEmple(this, sSource, aoData, fnCallback," + iCodEmpleado + ");}";
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisEmple");
        }

        protected virtual void InitGridResumen()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pResumenGrid.ClearConfig();
            pResumenGrid.Config.sDom = "<\"H\"lf>tr<\"F\"pi>"; //con filtro global
            pResumenGrid.Config.bAutoWidth = true;
            pResumenGrid.Config.sScrollX = "100%";
            pResumenGrid.Config.sScrollY = "400px";
            pResumenGrid.Config.sPaginationType = "full_numbers";
            pResumenGrid.Config.bJQueryUI = true;
            pResumenGrid.Config.bProcessing = true;
            pResumenGrid.Config.bServerSide = true;
            if (pbEnableEmpleado)
            {
                pResumenGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerResumen(this, sSource, aoData, fnCallback," + iCodEntidadResumen + "," + iCodMaestroResumen + ");}";
                pResumenGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetResumen");
                pResumenGrid.Config.fnDrawCallback = "function(){" + pjsObj + ".ResumenEtiquetaDrawDD();" + pjsObj + ".ResumenEtiquetaDrawTxt();}";
            }

            pFieldsResumen = new HistoricFieldCollection(int.Parse(iCodEntidadResumen), int.Parse(iCodMaestroResumen));

            if (pFieldsResumen != null)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "iCodRegistro";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pResumenGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "vchDescripcion";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pResumenGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "GEtiqueta";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pResumenGrid.Config.aoColumnDefs.Add(lCol);

                foreach (KeytiaBaseField lField in pFieldsResumen)
                {
                    if (lField.ShowInGrid)
                    {
                        if (lTarget == 4)
                        {
                            lCol = new DSOGridClientColumn();
                            lCol.sName = "vchDescLocalidad";
                            lCol.aTargets.Add(lTarget++);
                            pResumenGrid.Config.aoColumnDefs.Add(lCol);

                            lCol = new DSOGridClientColumn();
                            lCol.sName = "vchDescTpDestino";
                            lCol.aTargets.Add(lTarget++);
                            pResumenGrid.Config.aoColumnDefs.Add(lCol);
                        }
                        lCol = new DSOGridClientColumn();
                        lCol.sName = lField.Column;
                        lCol.aTargets.Add(lTarget++);
                        pResumenGrid.Config.aoColumnDefs.Add(lCol);
                    }
                }

                lCol = new DSOGridClientColumn();
                lCol.sName = "EdodeReg";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pResumenGrid.Config.aoColumnDefs.Add(lCol);

                pResumenGrid.MetaData = DSODataAccess.Execute("Select top 0 * from " + DSODataContext.Schema + ".ViewGetResumen");
                pResumenGrid.Fill();
            }

        }

        protected virtual void InitWindows()
        {
            //Window Detalle
            pEtqDetalle = new EtiquetaDetalle(iCodEmpleado, vchCodEntidad, vchDesMaestro);
            pEtqDetalle.ID = pjsObj;
            pEtqDetalle.vchDesMaestroResumen = "Detalle Etiquetacion";
            pEtqDetalle.lblTitle = lblTitle;
            pEtqDetalle.pFieldsNoVisibles = new string[] { "Instruc", "GEtiqueta", "TelDest" };
            pwndDetalle.Content.Controls.Add(pEtqDetalle);
            pwndDetalle.Modal = false;

            //Window Detalle Linea
            pEtqDetLinea = new EtiquetaDetalle(iCodEmpleado, vchCodEntidad, vchDesMaestro);
            pEtqDetLinea.ID = pjsObj;
            pEtqDetLinea.vchDesMaestroResumen = "Detalle Linea Etiquetacion";
            pEtqDetLinea.lblTitle = lblTitle;
            pEtqDetLinea.pFieldsNoVisibles = new string[] { "Instruc" };
            pEtqDetLinea.pbDetalleLinea = true;
            pwndDetLinea.Content.Controls.Add(pEtqDetLinea);
            pwndDetLinea.Modal = false;
        }

        protected virtual void InitSubHisDirectorio(string lsSubHistoricClass)
        {
            this.SubHistoricClass = lsSubHistoricClass;
            this.InitSubHistorico(this.ID + "0SubHis");
            this.SetHistoricState(EtiquetaState.EtiquetaDir);
            SubHistorico.SetHistoricState(HistoricState.Inicio);
            this.SubHistorico.SetEntidad("Directorio");
            string lsMaestro = SubHistorico.GetMaestro(int.Parse(SubHistorico.iCodEntidad));
            if (!String.IsNullOrEmpty(lsMaestro))
            {
                SubHistorico.SetMaestro(lsMaestro);
            }
            SubHistorico.EsSubHistorico = true;
            SubHistorico.FillControls();

            FirePostDirectorioClick();
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (State != EtiquetaState.EtiquetaDir)
            {
                DisableFields();
                InitLanguageEtiqueta();
            }
        }

        protected virtual void InitLanguageEtiqueta()
        {
            piCodGrupo.DSOControlDB.Descripcion = Globals.GetMsgWeb(false, "GrupoEtiqueta");
            //NZ 201508112 Se incluye filtro por Tipo Destino
            piCodTDest.DSOControlDB.Descripcion = Globals.GetMsgWeb(false, "NomColDestinoEtq");
            //

            pNumTotPersonal.Descripcion = Globals.GetMsgWeb(false, "ConsumoPerEtiqueta");
            pNumTotLaboral.Descripcion = Globals.GetMsgWeb(false, "ConsumoLabEtiqueta");
            pNumTotNI.Descripcion = Globals.GetMsgWeb(false, "ConsumoNIEtiqueta");
            pNumTotal.Descripcion = Globals.GetMsgWeb(false, "ConsumoGenEtiqueta");
            pbPersonales.Descripcion = Globals.GetMsgWeb(false, "TodasPersonalesEtiqueta");
            pbLaborales.Descripcion = Globals.GetMsgWeb(false, "TodasLaboralesEtiqueta");

            pwndDetalle.Title = Globals.GetMsgWeb(false, "WdDetalleEtiqueta");
            pwndDetLinea.Title = Globals.GetMsgWeb(false, "WdDetLineaEtiqueta");

            pbtnDirPersonal.InnerText = Globals.GetMsgWeb("btnDirPersonal");
            pbtnDirCorporativo.InnerText = Globals.GetMsgWeb("btnDirCorporativo");
            pbtnVerDetalle.InnerText = Globals.GetMsgWeb("btnVerDetalle");

            piCodGrupo.InitDSOControlDBLanguage();
            DataTable ldtFiltroGEtiqueta = (DataTable)((DSODropDownList)piCodGrupo.DSOControlDB).DataSource;
            pdtGrupoCol = ldtFiltroGEtiqueta.Clone();
            foreach (DataRow lDataRow in ldtFiltroGEtiqueta.Select("value >= 0"))
            {
                pdtGrupoCol.ImportRow(lDataRow);
            }

            pExpResumen.Title = Globals.GetMsgWeb("EtiquetaDataTitle");
            pExpResumen.ToolTip = Globals.GetMsgWeb("EtiquetaDataTitle");
            pbPersonales.CheckBox.Enabled = true;
            pbLaborales.CheckBox.Enabled = true;

            if (State == HistoricState.Edicion && pbEnableEmpleado)
            {
                StringBuilder lsb = new StringBuilder();
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".iCodEmpleado = " + iCodEmpleado + ";");
                lsb.AppendLine(pjsObj + ".iCodEntidadResum = " + iCodEntidadResumen + ";");
                lsb.AppendLine(pjsObj + ".iCodMaestroResum = " + iCodMaestroResumen + ";");
                lsb.AppendLine(pjsObj + ".dtCorte = new Date(parseInt(" + DSOControl.SerializeJSON<DateTime>(pdtCorte) + ".substr(6)));");
                lsb.AppendLine(pjsObj + ".dtLimite = new Date(parseInt(" + DSOControl.SerializeJSON<DateTime>(pdtLimite) + ".substr(6)));");
                lsb.AppendLine(pjsObj + ".liDiaCorte = " + piDiaCorte + ";");
                lsb.AppendLine(pjsObj + ".liLongEtq = " + piLongEtiqueta + ";");
                lsb.AppendLine(pjsObj + ".liHisPrevEtq = " + piHisPreviaEtiqueta + ";");
                lsb.AppendLine(pjsObj + ".$gridData = $('#" + pResumenGrid.Grid.ClientID + "__hidden');");
                lsb.AppendLine(pjsObj + ".confirmGrabar = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarGrabarEtq")) + "\";");
                lsb.AppendLine(pjsObj + ".confirmTitleGrabar = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarTitulo")) + "\";");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj, lsb.ToString(), true, false);

                int liHabil = IsPeriodoValido(int.Parse(iCodEmpleado), IniPeriodo, FinPeriodo, pdtCorte, pdtLimite);
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(" var $chkPersonal =  $('#" + pbPersonales.CheckBox.ClientID + "');");
                lsb.AppendLine(" var $chkLaboral =  $('#" + pbLaborales.CheckBox.ClientID + "');");
                lsb.AppendLine(" var $SaveBtn =  $('#" + pbtnGrabar.ClientID + "');");
                lsb.AppendLine(" $chkPersonal[0].status = false;");
                if (liHabil == 1)
                {
                    lsb.AppendLine(" $SaveBtn.removeAttr(\"disabled\");");
                    lsb.AppendLine(" $chkPersonal.removeAttr(\"disabled\");");
                    lsb.AppendLine(" $chkLaboral.removeAttr(\"disabled\");");
                }
                else
                {
                    lsb.AppendLine(" $SaveBtn.attr(\"disabled\", true);");
                    lsb.AppendLine(" $chkPersonal.attr(\"disabled\", true);");
                    lsb.AppendLine(" $chkLaboral.attr(\"disabled\", true);");
                }
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "NewValidate", lsb.ToString(), true, false);

            }


            if (pFieldsResumen != null)
            {
                InitLanguageGridResumen(pFieldsResumen, pResumenGrid);
            }

            pNumTotal.NumberBox.Text = pTxtNumTotServer.TextBox.Text;
            pNumTotPersonal.NumberBox.Text = pTxtNumTotPServer.TextBox.Text;
            pNumTotLaboral.NumberBox.Text = pTxtNumTotLServer.TextBox.Text;
            pNumTotNI.NumberBox.Text = pTxtNumTotNServer.TextBox.Text;

            IniDetalleTemporal();
            //NZ 201508112 Se incluye filtro por Tipo Destino
            LlenarComboTDest();
        }

        protected void IniDetalleTemporal()
        {
            //Inicializa Detalle temporal si es la primera vez que se accesa al periodo.
            DataTable ldtDataTable;
            ldtDataTable = HistoricFieldCollection.GetAtrib();
            int liCodIdioma = (int)ldtDataTable.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];
            iCodEmpleado = string.IsNullOrEmpty(iCodEmpleado) ? "0" : iCodEmpleado.ToString();
            psbQuery.Length = 0;

            psbQuery.Append("Select Count(iCodRegistro) ");
            psbQuery.AppendLine(" from [" + DSODataContext.Schema + "].ViewGetResumen a");
            psbQuery.AppendLine(" where iCodCatalogo01 = " + iCodEmpleado.ToString() +"");
            psbQuery.AppendLine(" and Date01 >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine(" and Date02 < dateadd(day,1,'" + FinPeriodo.ToString("yyyy-MM-dd") + "')");
            int liCount = (int)KeytiaServiceBL.Util.IsDBNull(DSODataAccess.ExecuteScalar(psbQuery.ToString()), 0);

            if ((liCount == 0 || pbNuevaEtiquetacion == true) && pbEnableEmpleado)
            {
                SetResumen(int.Parse(iCodEmpleado), pdtIniPeriodoInicial, pdtFinPeriodoInicial, piHisPreviaEtiqueta, int.Parse(iCodMaestroResumen), int.Parse(iCodEntidadResumen));
                pbNuevaEtiquetacion = false;
            }
        }

        //NZ 201508112 Se incluye filtro por Tipo Destino
        protected void LlenarComboTDest()
        {
            int icodEmple;
            if (int.TryParse(iCodEmpleado, out icodEmple) && icodEmple != 0)
            {
                StringBuilder consulta = new StringBuilder();
                consulta.AppendLine("select Value, vchDescripcion ");
                consulta.AppendLine("from ( ");
                consulta.AppendLine("		select TDest.iCodCatalogo as Value, TDest.vchDescripcion ");
                consulta.AppendLine("		from " + DSODataContext.Schema + ".HistTDest TDest ");
                consulta.AppendLine("		JOIN (select iCodCatalogo09 as Emple, iCodCatalogo05 as TDest ");
                consulta.AppendLine("				from " + DSODataContext.Schema + ".Detallados detall ");
                consulta.AppendLine("				where icodMaestro = 89 ");
                consulta.AppendLine("				and iCodCatalogo09 = " + int.Parse(iCodEmpleado).ToString());
                consulta.AppendLine("				group by iCodCatalogo09, iCodCatalogo05 ");
                consulta.AppendLine("				) as detall ");
                consulta.AppendLine("			ON TDest.iCodCatalogo = detall.TDest ");
                consulta.AppendLine("		where TDest.dtfinvigencia>=getdate() ");

                if (!pbEtiquetarIncluirLlamadasEntrada) //Si esta bandera No esta prendida...se excluyen llamadas de entrada.
                {
                    consulta.AppendLine(" AND TDest.vchCodigo <> 'Ent'");
                }

                consulta.AppendLine(" AND TDest.vchCodigo <> 'Enl'");

                if (DSODataContext.Schema.ToLower() == "cide")
                {
                    var dtTDest = GetTDest();
                    consulta.AppendLine(" AND TDest.vchCodigo <> 'Local'");
                    consulta.AppendLine(" AND TDest.vchCodigo <> 'LDNac'");
                }

                consulta.AppendLine(") as Resultado ");
                consulta.AppendLine("group by value, vchDescripcion ");

                DataTable dtResultado = DSODataAccess.Execute(consulta.ToString());

                if (dtResultado != null && dtResultado.Rows.Count > 0)
                {
                    DataView dv = dtResultado.DefaultView;
                    dv.Sort = "vchDescripcion";
                    dtResultado = dv.ToTable();

                }

                piCodTDest.InitDSOControlDBLanguageFromDataTable(dtResultado);
            }
        }


        protected virtual void InitLanguageGridResumen(HistoricFieldCollection pFieldsGrid, DSOGrid pGridDP)
        {
            pFieldsGrid.InitLanguage();
            KeytiaBaseField lField;
            string lsNomColCosto = "";

            pGridDP.Config.oLanguage = Globals.GetGridLanguage();

            foreach (DSOGridClientColumn lCol in pGridDP.Config.aoColumnDefs)
            {
                if (pFieldsGrid.Contains(lCol.sName))
                {
                    lField = pFieldsGrid[lCol.sName];
                    if (lField.ConfigName == "CostoFac")
                    {
                        lsNomColCosto = Globals.GetMsgWeb(false, "NomColConsumoEtq");
                        break;
                    }
                }
            }

            foreach (DSOGridClientColumn lCol in pGridDP.Config.aoColumnDefs)
            {

                if (pFieldsGrid.Contains(lCol.sName))
                {
                    lField = pFieldsGrid[lCol.sName];
                    lCol.sClass = lField.ConfigName;


                    if (lField.ConfigName == "Emple")
                    {
                        lCol.bVisible = false;
                    }
                    else if (lField.ConfigName == "CostoFac")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(lsNomColCosto);
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "DuracionMin")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColDuracionMinEtq"));
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "Cantidad")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColCantidadEtq"));
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "Locali")
                    {
                        lCol.sTitle = "iCodLocalidad";
                        lCol.bVisible = false;
                    }
                    else if (lField.ConfigName == "TDest")
                    {
                        lCol.sTitle = "iCodDestino";
                        lCol.bVisible = false;
                    }
                    else if (lField.ConfigName == "Etiqueta")
                    {
                        if (pbColEtiqueta)
                        {
                            lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                            lCol.bUseRendered = false;
                            lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRenderTxt(obj," + DSOControl.SerializeJSON<string>(lCol.sTitle) + " );}";
                            lCol.bVisible = true;
                        }
                        else
                        {
                            lCol.bVisible = false;
                        }
                    }
                    else if (lField.ConfigName == "TelDest")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColNumeroEtq"));
                        lCol.bUseRendered = false;
                        lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRenderLnk(obj," + DSOControl.SerializeJSON<string>(lCol.sTitle) + ",$('#" + pwndDetLinea.Content.ClientID + "'));}";
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "Gpo")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "GrupoEtiqueta"));
                        lCol.bUseRendered = false;
                        lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRenderCmb(obj," + DSOControl.SerializeJSON<DataTable>(pdtGrupoCol) + "," + DSOControl.SerializeJSON<string>(lCol.sTitle) + "," + DSOControl.SerializeJSON<string>(lsNomColCosto) + ");}";
                        lCol.bVisible = true;
                    }
                    else
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                        lCol.bVisible = false;
                    }
                }
                else if (lCol.sName == "vchDescLocalidad")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColLocalidadEtq"));
                    lCol.bVisible = true;
                }
                else if (lCol.sName == "vchDescTpDestino")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColDestinoEtq"));
                    lCol.bVisible = true;
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcion"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "GEtiqueta")
                {
                    lCol.sTitle = DSOControl.JScriptEncode("Integer03");
                    lCol.bVisible = false;
                }
            }
        }

        protected virtual void DisableFields()
        {
            if (pFields == null)
            {
                return;
            }

            pFields.DisableFields();

            //Habiliar Fields de Periodo  
            if (pbEnableEmpleado)
            {
                if (pFields.GetByConfigName("IniPer") != null)
                {
                    pFields.GetByConfigName("IniPer").EnableField();
                }
                if (pFields.GetByConfigName("FinPer") != null)
                {
                    pFields.GetByConfigName("FinPer").EnableField();
                }
            }

            //Ocultar Fields No necesarios en Etiquetación.
            foreach (string lField in pFieldsNoVisibles)
            {
                if (pFields.GetByConfigName(lField.ToString()) != null)
                {
                    pTablaAtributos.Rows[pFields.GetByConfigName(lField.ToString()).DSOControlDB.Row - 1].Visible = false;
                    pFields.GetByConfigName(lField.ToString()).DSOControlDB.Visible = false;
                }
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            pbtnDirCorporativo.Visible = false;
            pbtnDirPersonal.Visible = false;
            pbtnVerDetalle.Visible = false;
            pExpResumen.Visible = false;
            pResumenGrid.Visible = false;
            pbtnAgregar.Visible = false;
            pbtnCancelar.Visible = false;
            if (EsSubHistorico)
            {
                pbtnCancelar.Visible = true; //Regresa a SubhistoricoPrevio si existe.
            }

            if (s == HistoricState.Edicion)
            {
                if (pbEnableEmpleado)
                {
                    InitWindows();

                    pbtnGrabar.Visible = true;
                    pbtnVerDetalle.Visible = true;
                    pbtnDirCorporativo.Visible = pbDirCorporativo;
                    pbtnDirPersonal.Visible = true;
                    pExpResumen.Visible = true;
                    pExpAtributos.Visible = true;
                    pResumenGrid.Visible = true;
                    pbtnRegresar.Visible = pbEtiquetacionColaborador; //Regresa a Estado de MaestroSeleccionado
                }
                else
                {
                    pbtnGrabar.Visible = false;
                    pbtnVerDetalle.Visible = false;
                    pbtnDirCorporativo.Visible = false;
                    pbtnDirPersonal.Visible = false;
                    pResumenGrid.Visible = false;
                    pbtnRegresar.Visible = pbEtiquetacionColaborador;
                }

                pExpFiltros.Visible = false;
                pExpRegistro.Visible = false;
                pbtnConsultar.Visible = false;
                pbtnAgregar.Visible = false;
                pbtnEditar.Visible = false;
                pbtnBaja.Visible = false;
            }
            else if (s == EtiquetaState.EtiquetaDir)
            {
                this.Visible = false;
                pbtnCancelar.Visible = true;
                pbNuevaEtiquetacion = false;
            }
        }

        public override void ConsultarRegistro()
        {
            if (iCodRegistro != "null")
            {
                DataTable lKDBTable = pKDB.GetHisRegByEnt(vchCodEntidad, vchDesMaestro, "iCodRegistro = " + iCodRegistro);

                if (lKDBTable.Rows.Count > 0)
                {
                    DataRow lDataRow = lKDBTable.Rows[0];
                    iCodEmpleado = lDataRow["iCodCatalogo"].ToString();
                }

                GetClientConfig();
                GetPeriodoEtiquetaIni();

                iCodEntidad = iCodEntidadEtq;
                iCodMaestro = iCodMaestroEtq;
                InitMaestro();
            }
        }


        /// <summary>
        /// Inserta un registro en la vista visHistoricos('ProEtiqueta','Proceso Etiquetacion')
        /// con los datos del Empleado, el periodo y los totales de cada categoría
        /// </summary>
        protected virtual void GrabarProcesoEtiqueta()
        {
            try
            {
                pbEtiquetacionCorrecta = false;
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                phtValues = new Hashtable();

                phtValues.Add("vchCodigo", "'" + iCodEmpleado + "_" + IniPeriodo.Month.ToString() + "/" + IniPeriodo.Year.ToString() + "_" + piNumEtiquetaPeriodo + "'");
                phtValues.Add("vchDescripcion", "'Per: " + IniPeriodo.Month.ToString() + "/" + IniPeriodo.Year.ToString() + "_[" + piNumEtiquetaPeriodo + "]'");
                phtValues.Add("{Emple}", iCodEmpleado);
                phtValues.Add("{IniPer}", IniPeriodo);
                phtValues.Add("{FinPer}", FinPeriodo);
                phtValues.Add("dtIniVigencia", DateTime.Today);
                phtValues.Add("{TOTALSUMO}", pTxtNumTotServer.TextBox.Text);
                phtValues.Add("{ConsumoLab}", pTxtNumTotLServer.TextBox.Text);
                phtValues.Add("{ConsumoPer}", pTxtNumTotPServer.TextBox.Text);
                phtValues.Add("{ConsumoNI}", pTxtNumTotNServer.TextBox.Text);
                phtValues.Add("iCodUsuario", Session["iCodUsuario"]);


                //Mandar llamar al COM para grabar los datos del historico
                piProcesoEtiqueta = lCargasCOM.InsertaRegistro(phtValues, "Historicos", "ProEtiqueta", "Proceso Etiquetacion", false, (int)Session["iCodUsuarioDB"], false);
                if (piProcesoEtiqueta < 0)
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
                pbEtiquetacionCorrecta = true;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }

        }

        protected virtual void BajaProcesoEtiqueta()
        {
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                phtValues = new Hashtable();

                DataTable ldt = pKDB.GetHisRegByEnt("ProEtiqueta", "Proceso Etiquetacion", "iCodRegistro ='" + piProcesoEtiqueta + "'");
                if (ldt == null || ldt.Rows.Count == 0)
                {
                    return;
                }
                phtValues.Add("dtFinVigencia", ldt.Rows[0]["dtIniVigencia"]);

                //Da de baja el Proceso de Etiquetación
                lCargasCOM.ActualizaRegistro("Historicos", "ProEtiqueta", "Proceso Etiquetacion", phtValues, piProcesoEtiqueta, (int)Session["iCodUsuarioDB"]);
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }



        /// <summary>
        /// Actualiza el Directorio con los números que etiquetó el Empleado
        /// Actualiza los campos GEtiqueta y Etiqueta en DetalleCDR
        /// </summary>
        protected override void GrabarRegistro() //RJ.20160620
        {
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            DataTable ldtDirectorioEmple = new DataTable();
            phtValues = new Hashtable();
            pbEtiquetacionCorrecta = true;

            DateTime ldtFinPerido = new DateTime(2079, 1, ValidarDiasMes(2079, 1, piDiaCorte));
            if (piHisPreviaEtiqueta == 0 || pdtCorte > FinPeriodo.AddDays(1))
            {
                //La vigencia de la etiqueta del telefono sólo será para el periodo señalado si:
                //  *El cliente No tiene activa la bandera "Guardar Historia de Etiqueta" 
                //  *El periodo que se está etiquetando fue habilitado extemporáneo
                //La fecha de periodo fin muestra un día menos a la vigencia almacenada.
                ldtFinPerido = FinPeriodo.AddDays(1);
            }

            pKDB.FechaVigencia = IniPeriodo.Date;
            ldtDirectorioEmple = pKDB.GetHisRegByEnt("Directorio", "Directorio Telefonico", "{Emple} = " + iCodEmpleado);
            pKDB.FechaVigencia = DateTime.Today;
            int liCatProEtiqueta = (int)pKDB.GetHisRegByEnt("ProEtiqueta", "Proceso Etiquetacion", "iCodRegistro=" + piProcesoEtiqueta).Rows[0]["iCodCatalogo"];

            List<string> lstRegsDir = new List<string>();
            try
            {
                //Se graba registro en Directorio y en Detallados
                DataRow[] ldrArray;
                int liRegTelDirectorio = int.MinValue;
                bool lbActualizo = true;
                string lsEtiqueta = "";

                pFieldsResumen = new HistoricFieldCollection(int.Parse(iCodEntidadResumen), int.Parse(iCodMaestroResumen));


                if (pdtResumenConsultado == null || pdtResumenConsultado.Rows.Count == 0)
                {
                    goto GrabaDetalle;  //Actualiza los números en DetalleCDR
                }


                for (int liRow = 0; liRow < pdtResumenConsultado.Rows.Count; liRow++)
                {
                    liRegTelDirectorio = int.MinValue;
                    string lsNumero = (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow]["vchDescripcion"].ToString().Replace("'", ""), "");

                    if (ldtDirectorioEmple != null && ldtDirectorioEmple.Rows.Count > 0)
                    {
                        //Busca si el empleado ya ha etiquetado anteriormente el número                        
                        ldrArray = ldtDirectorioEmple.Select("vchCodigo = '" + lsNumero + "'");
                        if (ldrArray != null && ldrArray.Length > 0)
                        {
                            liRegTelDirectorio = (int)ldrArray[0]["iCodRegistro"];
                        }
                    }

                    lbActualizo = true; //inicializa variable
                    phtValues.Clear();
                    phtValues.Add("{Emple}", iCodEmpleado);
                    phtValues.Add("dtIniVigencia", IniPeriodo);
                    phtValues.Add("dtFinVigencia", ldtFinPerido);
                    phtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                    int liGpo = int.Parse(KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow]["GEtiqueta"], 0).ToString());
                    phtValues.Add("{GEtiqueta}", liGpo);
                    phtValues.Add("vchCodigo", (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow][pFieldsResumen.GetByConfigName("TelDest").Column], ""));
                    phtValues.Add("vchDescripcion", (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow][pFieldsResumen.GetByConfigName("TelDest").Column], "") + "_" + iCodEmpleado);
                    lsEtiqueta = (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow][pFieldsResumen.GetByConfigName("Etiqueta").Column], "");
                    phtValues.Add("{Etiqueta}", (liGpo == 0 ? "" : lsEtiqueta));
                    phtValues.Add("{BanderasEtiqueta}", 1); //Etiquetable

                    if (liRegTelDirectorio != int.MinValue)
                    {
                        lbActualizo = lCargasCOM.ActualizaRegistro("Historicos", "Directorio", "Directorio Telefonico", phtValues, liRegTelDirectorio, true, (int)Session["iCodUsuarioDB"]);
                    }
                    else if (liGpo == 0)
                    {
                        //Si el número fué grabado como "No Identificada", no se almacenará su directorio.
                        liRegTelDirectorio = 0;
                    }
                    else
                    {
                        liRegTelDirectorio = lCargasCOM.InsertaRegistro(phtValues, "Historicos", "Directorio", "Directorio Telefonico", true, (int)Session["iCodUsuarioDB"], false);
                    }

                    if (liRegTelDirectorio < 0 && !lbActualizo)
                    {
                        pbEtiquetacionCorrecta = false;
                        return;
                    }
                    else
                    {
                        //Almacena Directorios que se ReEtiquetarán en Llamadas:
                        lstRegsDir.Add(liRegTelDirectorio.ToString());
                    }
                }


            GrabaDetalle:

                DataTable ldtDataTable = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtDataTable.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                try
                {
                    int liMaeDetEtiquetacion = (int)DSODataAccess.ExecuteScalar("Select iCodRegistro From [" + DSODataContext.Schema + "].Maestros Where vchDescripcion = 'Detalle Proceso de Etiquetacion' and iCodEntidad = " + iCodEntidadResumen + " and dtIniVigencia <> dtFinVigencia");

                    //Almacena la imagen del Proceso de Etiquetación en Detallados:
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("Insert Into [" + DSODataContext.Schema + "].Detallados");
                    psbQuery.AppendLine("       (iCodUsuario,iCodCatalogo,iCodMaestro,{ProEtiqueta},{Locali},{TDest},{Tel},{Etiqueta},{Gpo},{CostoFac},");
                    psbQuery.AppendLine("        {Cantidad},{DuracionMin},{GEtiqueta},dtFecUltAct)");
                    psbQuery.AppendLine("Select iCodUsuario = " + Session["iCodUsuario"] + "," + "iCodCatalogo = " + iCodEntidadResumen + ",");
                    psbQuery.AppendLine("       iCodMaestro = " + liMaeDetEtiquetacion + ",");
                    psbQuery.AppendLine("       ProEtiqueta = " + liCatProEtiqueta + "," + pFieldsResumen.GetByConfigName("Locali").Column + ",");
                    psbQuery.AppendLine("       " + pFieldsResumen.GetByConfigName("TDest").Column + "," + pFieldsResumen.GetByConfigName("TelDest").Column + ",");
                    psbQuery.AppendLine("       Etiqueta = CASE WHEN GEtiqueta = 0 THEN '' ELSE " + pFieldsResumen.GetByConfigName("Etiqueta").Column + " END,");
                    psbQuery.AppendLine("       " + pFieldsResumen.GetByConfigName("Gpo").Column + "," + pFieldsResumen.GetByConfigName("CostoFac").Column + ",");
                    psbQuery.AppendLine("       " + pFieldsResumen.GetByConfigName("Cantidad").Column + "," + pFieldsResumen.GetByConfigName("DuracionMin").Column + ",");
                    psbQuery.AppendLine("       GEtiqueta,dtFecUltAct=GetDate()");
                    psbQuery.AppendLine(" from [" + DSODataContext.Schema + "].ViewGetResumen ");
                    psbQuery.AppendLine(" where iCodCatalogo01 =  " + iCodEmpleado.ToString());
                    psbQuery.AppendLine(" and Date01 >=  '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
                    psbQuery.AppendLine(" and Date02 < dateadd(day,1,'" + FinPeriodo.ToString("yyyy-MM-dd") + "')");
                    pKDB.ExecuteQuery("Detall", "Detalle Proceso de Etiquetacion", psbQuery.ToString());

                }
                catch (Exception ex)
                {
                    Util.LogException(ex);
                    pbEtiquetacionCorrecta = false;
                    return;
                }



                //Calcula Totales:
                psbQuery.Length = 0;
                psbQuery.Append("select Consumo = Sum(IsNull(G.{CostoFac},0)), Grupo = IsNull(G.{GEtiqueta},0) ");
                psbQuery.AppendLine(" from [" + DSODataContext.Schema + "].ViewGetResumen As G ");
                psbQuery.AppendLine(" where iCodCatalogo01 = " + iCodEmpleado.ToString());
                psbQuery.AppendLine(" and date01 >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
                psbQuery.AppendLine(" and date02 < dateadd(day,1,'" + FinPeriodo.ToString("yyyy-MM-dd") + "') Group By G.{GEtiqueta}");
                DataTable ldtConsumos = pKDB.ExecuteQuery("Detall", "Resumen Etiquetacion Temp", psbQuery.ToString());

                pNumTotNI.DataValue = 0;
                pNumTotPersonal.DataValue = 0;
                pNumTotLaboral.DataValue = 0;

                for (int liRow = 0; liRow < ldtConsumos.Rows.Count; liRow++)
                {
                    switch (ldtConsumos.Rows[liRow]["Grupo"].ToString())
                    {
                        case "0":
                            pNumTotNI.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        case "1":
                            pNumTotPersonal.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        case "2":
                            pNumTotLaboral.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        default:
                            break;
                    }
                }
                pNumTotal.DataValue = double.Parse(pNumTotNI.DataValue.ToString()) + double.Parse(pNumTotPersonal.DataValue.ToString()) + double.Parse(pNumTotLaboral.DataValue.ToString());



                //Obtiene el listado de registros que se requiere actualizar en DetalleCDR
                //posteriormente aplica la actualización, 
                //misma que se hace tomando como base el campo icodRegistro
                //El listado corresponde a las llamadas del periodo abierto para etiquetación
                pbEtiquetacionCorrecta = ActualizarEtiquetaEnDetalleCDR(iCodEmpleado, IniPeriodo, FinPeriodo);


            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                pbEtiquetacionCorrecta = false;
                return;
            }
        }


        public static System.Data.DataTable GetTDest()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo, vchCodigo, vchDescripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".HistTDest");
            query.AppendLine(" WHERE dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchCodigo in ('Local','LDNac','Enl')");
            return DSODataAccess.Execute(query.ToString());
        }


        /// <summary>
        /// Obtiene el listado de registros que se deben actualizar en DetalleCDR,
        /// mismos que corresponden al Empleado cuya fecha sea igual o mayor al periodo 
        /// que se está etiquetando
        /// </summary>
        /// <param name="iCodEmpleado">Emple</param>
        /// <param name="IniPeriodo">Fecha inicio del periodo</param>
        /// <param name="FinPeriodo">Fecha fin del periodo (ya no se utiliza)</param>
        /// <returns></returns>
        protected virtual bool ActualizarEtiquetaEnDetalleCDR(string iCodEmpleado, DateTime IniPeriodo, DateTime FinPeriodo) //RJ.20160620
        {
            //Obtiene el listado de registros que se requiere actualizar en Detalle
            //la actualización se hace tomando como base el campo icodRegistro
            psbQuery.Length = 0;
            psbQuery.AppendLine("select CDR.iCodRegistro, GEtiqueta = isnull(Dir.GEtiqueta,0), Etiqueta = isnull(Dir.Etiqueta,'')");
            psbQuery.AppendLine("from  " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')] CDR,");
            psbQuery.AppendLine(DSODataContext.Schema + ".[VisDirectorio('Español')] Dir");
            psbQuery.AppendLine("where Dir.Emple = " + iCodEmpleado);
            psbQuery.AppendLine("and Dir.dtIniVigencia <= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine("and Dir.dtFinVigencia > '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine("and CDR.Emple = Dir.Emple");
            psbQuery.AppendLine("and CDR.Emple = " + iCodEmpleado);
            psbQuery.AppendLine("and CDR.TelDest = Dir.vchCodigo");

            psbQuery.AppendLine("and CDR.TDestCod <> 'Enl'");  //Se excluyen llamadas de enlace.

            if (!pbEtiquetarIncluirLlamadasEntrada) //Si esta bandera No esta prendida...se excluyen llamadas de entrada.
            {
                psbQuery.AppendLine("and CDR.TpLlam <> 'Entrada'");
            }

            if (DSODataContext.Schema.ToLower() == "cide") //Prueba 
            {
                psbQuery.AppendLine("and CDR.TDestCod <> 'Local'");
                psbQuery.AppendLine("and CDR.TDestCod <> 'LDNac'");
            }

            psbQuery.AppendLine("and Dir.dtIniVigencia <= CDR.FechaInicio");
            psbQuery.AppendLine("and Dir.dtFinVigencia > CDR.FechaInicio");
            psbQuery.AppendLine("and CDR.FechaInicio >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");

            DataTable lCDR = DSODataAccess.Execute(psbQuery.ToString());


            foreach (DataRow lDataRowCDR in lCDR.Rows)
            {
                //Actualiza el campo GEtiqueta y Etiqueta en DetalleCDR
                psbQuery.Length = 0;
                psbQuery.AppendLine("Update CDR Set");
                psbQuery.AppendLine("   CDR.Integer04 = " + lDataRowCDR["GEtiqueta"] + ",");
                psbQuery.AppendLine("   CDR.Varchar10 = '" + lDataRowCDR["Etiqueta"].ToString().Replace("'", "''") + "'");
                psbQuery.AppendLine("from  " + DSODataContext.Schema + ".Detallados CDR");
                psbQuery.AppendLine("where CDR.iCodRegistro = " + lDataRowCDR["iCodRegistro"]);
                pbEtiquetacionCorrecta = DSODataAccess.ExecuteNonQuery(psbQuery.ToString());

                if (!pbEtiquetacionCorrecta)
                {
                    break;
                }
            }

            return pbEtiquetacionCorrecta;
        }


        static public int ValidarDiasMes(int liYear, int liMonth, int liDayCoL)
        {
            int liDaysInMonth = DateTime.DaysInMonth(liYear, liMonth);
            if (liDaysInMonth < liDayCoL)
            {
                return liDaysInMonth;
            }
            else if (liDayCoL == 0)
            {
                return DateTime.Today.Day;
            }
            return liDayCoL;
        }

        protected override bool ValidarRegistro()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError = "";
            string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select isnull(count(icodcatalogo),0) from" + "\r\n");
                psbQuery.AppendLine("(select * from [" + DSODataContext.Schema + "].[VisHistoricos('ProEtiqueta','Proceso Etiquetacion','" + Globals.GetCurrentLanguage() + "')]) a" + "\r\n");
                psbQuery.AppendLine("where a.Emple = " + iCodEmpleado + "\r\n");
                psbQuery.AppendLine("and a.IniPer >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                psbQuery.AppendLine("and a.FinPer <= '" + FinPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                psbQuery.AppendLine("and a.dtIniVigencia <> a.dtFinVigencia");
                piNumEtiquetaPeriodo = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                if (pbEtiquetarUnaVez)
                {
                    //Valida que no se haya ejecutado el proceso de Etiquetación para el empleado en el periodo
                    if (piNumEtiquetaPeriodo > 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjPerEtiquetado"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lbret = false;
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                        return lbret;
                    }
                }

                DataTable ldtDataTable;
                ldtDataTable = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtDataTable.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                psbQuery.Length = 0;
                psbQuery.AppendLine("select * from [" + DSODataContext.Schema + "].ViewGetResumen ");
                psbQuery.AppendLine(" where iCodCatalogo01 = " + iCodEmpleado.ToString());
                psbQuery.AppendLine(" and date01 >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
                psbQuery.AppendLine(" and date02 < dateadd(day,1,'" + FinPeriodo.ToString("yyyy-MM-dd") + "') ");
                psbQuery.AppendLine(" and EdodeReg = 2 --Numeros Modificados");
                pdtResumenConsultado = DSODataAccess.Execute(psbQuery.ToString());
                if (pdtResumenConsultado == null || pdtResumenConsultado.Rows.Count == 0)
                {
                    //No hay números por etiquetar;
                    return true;
                }

                string lsTelDest = "";
                //int liEdoDeReg = int.MinValue;
                int liGrupo = int.MinValue;
                pFieldsResumen = new HistoricFieldCollection(int.Parse(iCodEntidadResumen), int.Parse(iCodMaestroResumen));
                for (int liRow = 0; liRow < pdtResumenConsultado.Rows.Count; liRow++)
                {
                    lsTelDest = pdtResumenConsultado.Rows[liRow]["vchDescripcion"].ToString();
                    liGrupo = int.Parse(KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow]["GEtiqueta"], 0).ToString());
                    if (liGrupo < 1 && pbEtiquetarTodos)
                    {
                        //Número almacenado como No Identificado
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjEtiquetarNumero", lsTelDest));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lbret = false;
                    }
                    else if (pbColEtiqueta && pbNoEtiquetaEnBlanco && ((string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow][pFieldsResumen.GetByConfigName("Etiqueta").Column], "")).Trim() == "")
                    {
                        //Número sin Etiqueta
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjEtiquetarTxtNumero", lsTelDest));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lbret = false;
                    }
                    if (!lbret)
                    {
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected void GetClientConfig()
        {
            DataRow ldrCliente;
            DataTable ldtEmpleColaborador = new DataTable();
            string liCodCliente = "";
            pbColEtiqueta = false;
            piHisPreviaEtiqueta = 0;
            pbEtiquetarUnaVez = false;
            pbEtiquetarTodos = false;
            pbNoEtiquetaEnBlanco = false;
            pbCheckTodasLaborales = false;

            ldrCliente = KeytiaServiceBL.Alarmas.Alarma.getCliente(int.Parse(iCodEmpleado));
            if (ldrCliente == null)
            {
                if (pbEtiquetacionColaborador)
                {
                    DataTable ldt = pKDB.GetHisRegByEnt("Emple", "Empleados", "{Usuar}='" + Session["iCodUsuario"].ToString() + "'");
                    int liColaborador = (int)Util.IsDBNull(ldt.Rows[0]["iCodCatalogo"], -1);
                    ldrCliente = KeytiaServiceBL.Alarmas.Alarma.getCliente(liColaborador);
                }

                if (ldrCliente == null)
                {
                    pbEnableEmpleado = false;
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "UsuarioSinEmpleado"));
                    DSOControl.jAlert(Page, pjsObj, lsError, lsTitulo);
                    return;
                }
            }

            liCodCliente = Util.IsDBNull(ldrCliente["iCodCatalogo"], "").ToString();
            piDiaCorte = int.Parse(Util.IsDBNull(ldrCliente["{DiaEtiquetacion}"], 1).ToString());
            piDiaLimite = int.Parse(Util.IsDBNull(ldrCliente["{DiaLmtEtiquetacion}"], 0).ToString());
            piLongEtiqueta = int.Parse(Util.IsDBNull(ldrCliente["{LongEtiqueta}"], 140).ToString());

            psMailCC = Util.IsDBNull(ldrCliente["{CtaCC}"], "").ToString();
            psMailCCO = Util.IsDBNull(ldrCliente["{CtaCCO}"], "").ToString();
            psMailRemitente = Util.IsDBNull(ldrCliente["{CtaDe}"], "").ToString();
            psNomRemitente = Util.IsDBNull(ldrCliente["{NomRemitente}"], "").ToString();
            psLogoClientePath = Util.IsDBNull(ldrCliente["{Logo}"], "").ToString().Replace("/", "\\");
            psLogoClientePath = psLogoClientePath.Replace("~", HttpContext.Current.Server.MapPath("~"));
            psLogoKeytiaPath = Session["StyleSheet"].ToString().Replace("/", "\\");
            psLogoKeytiaPath = psLogoKeytiaPath.Replace("~", HttpContext.Current.Server.MapPath("~"));
            psLogoKeytiaPath = System.IO.Path.Combine(psLogoKeytiaPath, @"images\KeytiaHeader.png");

            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x01) / 0x01) == 1)
            {
                piHisPreviaEtiqueta = 1;
            }
            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x02) / 0x02) != 1)
            {
                pbEtiquetarTodos = true;
            }
            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x04) / 0x04) == 1)
            {
                pbEtiquetarUnaVez = true;
            }
            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x08) / 0x08) == 1)
            {
                string lsMailSupervisor = KeytiaServiceBL.Alarmas.Alarma.getCtaSupervisor(int.Parse(iCodEmpleado));
                if (lsMailSupervisor.Length > 0)
                {
                    psMailCC = (psMailCC.Length > 0 ? (";" + lsMailSupervisor) : lsMailSupervisor);
                }
            }
            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x10) / 0x10) == 1)
            {
                pbColEtiqueta = true;
            }
            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x20) / 0x20) == 1)
            {
                pbCheckTodasLaborales = true;
            }
            if (pbColEtiqueta && (((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x40) / 0x40) == 1)
            {
                pbNoEtiquetaEnBlanco = true;
            }
            //NZ La bandera tiene el bit 128. El 128 en hexadecimal es 80
            pbEtiquetarIncluirLlamadasEntrada = false;
            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x80) / 0x80) == 1)
            {
                pbEtiquetarIncluirLlamadasEntrada = true;
            }
        }

        protected void GetPeriodoEtiquetaIni()
        {
            int liDiaCorte;
            int liDiaLimite;

            if (piDiaCorte > pdtActual.Day)
            {
                liDiaCorte = ValidarDiasMes(pdtActual.AddMonths(-1).Year, pdtActual.AddMonths(-1).Month, piDiaCorte);
                pdtCorte = new DateTime(pdtActual.AddMonths(-1).Year, pdtActual.AddMonths(-1).Month, liDiaCorte);
            }
            else
            {
                liDiaCorte = ValidarDiasMes(pdtActual.Year, pdtActual.Month, piDiaCorte);
                pdtCorte = new DateTime(pdtActual.Year, pdtActual.Month, liDiaCorte);
            }

            liDiaLimite = ValidarDiasMes(pdtActual.Year, pdtActual.Month, piDiaLimite);
            pdtLimite = new DateTime(pdtActual.Year, pdtActual.Month, liDiaLimite);

            int liDiaIniPeriodo = ValidarDiasMes(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, piDiaCorte);
            pdtIniPeriodoInicial = new DateTime(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, liDiaIniPeriodo);
            int liDiaFinPeriodo = ValidarDiasMes(pdtCorte.Year, pdtCorte.Month, piDiaCorte);
            if (liDiaFinPeriodo == 1)
            {
                pdtFinPeriodoInicial = new DateTime(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, DateTime.DaysInMonth(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month));
            }
            else
            {
                pdtFinPeriodoInicial = new DateTime(pdtCorte.Year, pdtCorte.Month, liDiaFinPeriodo - 1);
            }
        }

        protected virtual DataTable GetExportGrid(string lsTipoDoc)
        {
            DataTable ldt = null;

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                if (State != HistoricState.Edicion || iCodEntidadResumen == null || iCodEntidadResumen == "" || iCodMaestroResumen == null || iCodMaestroResumen == "")
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                pFieldsResumen = new HistoricFieldCollection(int.Parse(iCodEntidadResumen), int.Parse(iCodMaestroResumen));

                KDBAccess lKDB = new KDBAccess();
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
                DataTable ldtAtributos;
                KeytiaBaseField lField;
                string lsConvToText = "";

                if (DSODataContext.GetObject("Atrib") == null)
                {
                    ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                    DSODataContext.SetObject("Atrib", ldtAtributos);
                }
                else
                {
                    ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
                }
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];
                if (lsTipoDoc != "WORD")
                {
                    lsConvToText = "'''' + ";
                }

                lsbSelectTop.AppendLine("select  ");

                ////////////////////////////////////////////////////////////////////////////
                foreach (DSOGridClientColumn lCol in pResumenGrid.Config.aoColumnDefs)
                {
                    if (pFieldsResumen.Contains(lCol.sName))
                    {
                        lField = pFieldsResumen[lCol.sName];
                        lCol.sClass = lField.ConfigName;

                        if (!lCol.bVisible)
                        {
                            continue;
                        }

                        if (lField.ConfigName == "TelDest")
                        {
                            lsbColumnas.AppendLine(lsConvToText + lCol.sName + " as '" + Globals.GetMsgWeb(false, "NomColNumeroEtq") + "',");
                        }
                        else if (lField.ConfigName == "CostoFac")
                        {
                            lsbColumnas.AppendLine("'$' + Convert(Char,convert(Money," + lCol.sName + ")) as '" + Globals.GetMsgWeb(false, "NomColConsumoEtq") + "',");
                        }
                        else if (lField.ConfigName == "DuracionMin")
                        {
                            lsbColumnas.AppendLine(lCol.sName + " as '" + Globals.GetMsgWeb(false, "NomColDuracionMinEtq") + "',");
                        }
                        else if (lField.ConfigName == "Cantidad")
                        {
                            lsbColumnas.AppendLine(lCol.sName + " as '" + Globals.GetMsgWeb(false, "NomColCantidadEtq") + "',");
                        }
                        else if (lField.ConfigName == "Gpo")
                        {
                            lsbColumnas.AppendLine(lCol.sName + " as '" + Globals.GetMsgWeb(false, "GrupoEtiqueta") + "',");
                        }
                        else
                        {
                            lsbColumnas.AppendLine(lCol.sName + " as '" + lCol.sTitle + "',");
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////


                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString().Remove(lsbColumnas.Length - 3, 1);
                StringBuilder lsFromGetResumen = new StringBuilder();
                lsFromGetResumen.AppendLine(" from [" + DSODataContext.Schema + "].ViewGetResumen a ");
                lsFromGetResumen.AppendLine(" where iCodCatalogo01 = " + iCodEmpleado.ToString());
                lsFromGetResumen.AppendLine(" and date01 >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
                lsFromGetResumen.AppendLine(" and date02 < dateadd(day,1,'" + FinPeriodo.ToString("yyyy-MM-dd") + "')");
                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetResumen.ToString());

                return ldt;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }

        }

        /// <summary>
        /// Genera el DataTable con la información que se imprimirá
        /// </summary>
        /// <param name="lsTipoDoc"></param>
        /// <returns></returns>
        protected virtual DataTable GetExportGridCIDE()
        {
            DataTable ldt = null;

            try
            {
                ldt = DSODataAccess.Execute(ConsultaDetalleCIDE(int.Parse(iCodEmpleado)));
                EliminarColumnasDeAcuerdoABanderas(ldt);
                ldt.Columns["Total"].ColumnName = "Importe";
                if (ldt.Columns.Contains("RID"))
                    ldt.Columns.Remove("RID");
                if (ldt.Columns.Contains("RowNumber"))
                    ldt.Columns.Remove("RowNumber");
                if (ldt.Columns.Contains("TopRID"))
                    ldt.Columns.Remove("TopRID");

                return ldt;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }

        }


        protected Hashtable GetTipoDetalle(string lsTipoDoc)
        {
            Hashtable lhtGetTipoDetalle = new Hashtable();
            DataTable ldt = null;
            bool lbIsDetLinea = false;
            string lsTxtHidden = pTxtWindowVisible.TextBox.Text;

            if (lsTxtHidden.Contains("DetLin"))
            {
                lbIsDetLinea = true;
                pEtqDetLinea.Linea = lsTxtHidden.Split('|')[1];
                pEtqDetLinea.Grupo = lsTxtHidden.Split('|')[2];
                pEtqDetLinea.Fields.GetByConfigName("IniPer").DataValue = pFields.GetByConfigName("IniPer").DataValue;
                pEtqDetLinea.Fields.GetByConfigName("FinPer").DataValue = pFields.GetByConfigName("FinPer").DataValue;
                ldt = pEtqDetLinea.GetExportGrid(lsTipoDoc);
            }
            else if (lsTxtHidden.Contains("Det"))
            {
                pEtqDetalle.Fields.GetByConfigName("IniPer").DataValue = pFields.GetByConfigName("IniPer").DataValue;
                pEtqDetalle.Fields.GetByConfigName("FinPer").DataValue = pFields.GetByConfigName("FinPer").DataValue;
                ldt = pEtqDetalle.GetExportGrid(lsTipoDoc);
            }
            else
            {
                piNumEtiquetaPeriodo = 0;
                psbQuery.Length = 0;
                psbQuery.AppendLine("select isnull(count(icodcatalogo),0) from" + "\r\n");
                psbQuery.AppendLine("(select * from [" + DSODataContext.Schema + "].[VisHistoricos('ProEtiqueta','Proceso Etiquetacion','" + Globals.GetCurrentLanguage() + "')]) a" + "\r\n");
                psbQuery.AppendLine("where a.Emple = " + iCodEmpleado + "\r\n");
                psbQuery.AppendLine("and a.IniPer >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                psbQuery.AppendLine("and a.FinPer <= '" + FinPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                psbQuery.AppendLine("and a.dtIniVigencia <> a.dtFinVigencia");
                piNumEtiquetaPeriodo = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());
                if (piNumEtiquetaPeriodo == 0)
                {
                    if (IsPeriodoValido(int.Parse(iCodEmpleado), IniPeriodo, FinPeriodo, pdtCorte, pdtLimite) == 1)
                    {
                        //El periodo aún no ha sido etiquetado, no podrá ser exportado.
                        return null;
                    }
                }
                ldt = GetExportGrid(lsTipoDoc);
            }

            if (ldt == null || ldt.Rows.Count == 0)
            {
                return null;
            }
            lhtGetTipoDetalle.Add("lbIsDetLinea", lbIsDetLinea);
            lhtGetTipoDetalle.Add("ldtDetalle", ldt);
            return lhtGetTipoDetalle;
        }


        #region ExportarDetalles

        protected string GetFileName(string lsExt)
        {
            DataTable ldtEmpleado = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + iCodEmpleado);
            string lsFileName = System.IO.Path.Combine(psTempPath, "(" + ldtEmpleado.Rows[0]["vchCodigo"].ToString().Trim() + ")_" + IniPeriodo.Date.Year + "_" + IniPeriodo.Date.Month + " - " + FinPeriodo.Date.Month + "." + psFileKey + ".temp" + lsExt);
            Session[psFileKey] = lsFileName;
            return lsFileName;
        }

        protected void ExportarArchivo(string lsExt)
        {
            DataTable ldtEmpleado = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + iCodEmpleado);
            string lsTitulo = "(" + ldtEmpleado.Rows[0]["vchCodigo"].ToString().Trim() + ")_" + IniPeriodo.Date.Year + "_" + IniPeriodo.Date.Month + " - " + FinPeriodo.Date.Month;
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        public override void ExportXLS()
        {
            Hashtable lhtGetTipoDetall = GetTipoDetalle("XLS");
            try
            {
                if (lhtGetTipoDetall == null || lhtGetTipoDetall.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjSinInfoXExprtar"));
                    DSOControl.jAlert(Page, pjsObj + ".pbtnGrabar_ServerClick", lsError, lsTitulo);
                    return;
                }
                ExportXLSGrid((DataTable)lhtGetTipoDetall["ldtDetalle"], (bool)lhtGetTipoDetall["lbIsDetLinea"]);
                ExportarArchivo(".xlsx");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".xlsx");
            }
        }

        public override void ExportPDF()
        {
            Hashtable lhtGetTipoDetall = GetTipoDetalle("WORD");
            try
            {
                if (lhtGetTipoDetall == null || lhtGetTipoDetall.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjSinInfoXExprtar"));
                    DSOControl.jAlert(Page, pjsObj + ".pbtnGrabar_ServerClick", lsError, lsTitulo);
                    return;
                }
                ExportWordGrid((DataTable)lhtGetTipoDetall["ldtDetalle"], (bool)lhtGetTipoDetall["lbIsDetLinea"], ".pdf");
                ExportarArchivo(".pdf");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".pdf");
            }
        }

        public override void ExportDOC()
        {
            Hashtable lhtGetTipoDetall = GetTipoDetalle("WORD");
            try
            {
                if (lhtGetTipoDetall == null || lhtGetTipoDetall.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjSinInfoXExprtar"));
                    DSOControl.jAlert(Page, pjsObj + ".pbtnGrabar_ServerClick", lsError, lsTitulo);
                    return;
                }
                ExportWordGrid((DataTable)lhtGetTipoDetall["ldtDetalle"], (bool)lhtGetTipoDetall["lbIsDetLinea"], ".docx");
                ExportarArchivo(".docx");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".docx");
            }
        }

        public override void ExportCSV()
        {
            Hashtable lhtGetTipoDetall = GetTipoDetalle("CSV");
            try
            {
                if (lhtGetTipoDetall == null || lhtGetTipoDetall.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjSinInfoXExprtar"));
                    DSOControl.jAlert(Page, pjsObj + ".pbtnGrabar_ServerClick", lsError, lsTitulo);
                    return;
                }
                ExportTXTGrid((DataTable)lhtGetTipoDetall["ldtDetalle"], (bool)lhtGetTipoDetall["lbIsDetLinea"]);
                ExportarArchivo(".csv");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".csv");
            }
        }

        protected virtual void ExportXLSGrid(DataTable ldt, bool lbIsDetLinea)
        {
            ExcelAccess loExcel = new ExcelAccess();
            KDBAccess lKDB = new KDBAccess();
            string lsDateFormat = Globals.GetMsgWeb(false, "NetDateTimeFormat");
            int li = 0;
            int liRow = 0;
            int liRow2 = 0;
            int liCol = 0;
            string lsHoja0;
            string lsExcelPath = buscarPlantilla(lbIsDetLinea, false, ".xlsx");
            Hashtable lHTDatosImg = null;
            string lsTopLeft = null;
            string lsBottomLeft = null;
            float lfImgOffset = 0;
            int liRenLogoCliente = -1;
            int liRenLogoKeytia = -1;

            try
            {
                object[,] loColumnas = new object[1, ldt.Columns.Count];
                object[,] loData = loExcel.DataTableToArray(ldt);

                foreach (DataColumn lCol in ldt.Columns)
                {
                    loColumnas[0, li] = lCol.ColumnName;
                    li = li + 1;
                }

                if (lsExcelPath != "")
                {
                    loExcel.FilePath = lsExcelPath;
                }
                loExcel.Abrir(true);

                //Encabezado
                lsHoja0 = loExcel.NombreHoja0();
                if (System.IO.File.Exists(psLogoClientePath) && lsExcelPath != "")
                {
                    lHTDatosImg = loExcel.ReemplazaTextoPorImagen(lsHoja0, "{LogoCliente}", false, psLogoClientePath, false, false, 0, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lfImgOffset = (float)lHTDatosImg["Ancho"];
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoCliente = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
                else if (System.IO.File.Exists(psLogoClientePath) && lsExcelPath == "")
                {
                    loExcel.InsertPicture(lsHoja0, psLogoClientePath, "A1", "B5", false, false);
                }

                if (System.IO.File.Exists(psLogoKeytiaPath) && lsExcelPath != "")
                {
                    lHTDatosImg = loExcel.ReemplazaTextoPorImagen(lsHoja0, "{LogoKeytia}", false, psLogoKeytiaPath, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoKeytia = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
                else if (System.IO.File.Exists(psLogoKeytiaPath) && lsExcelPath == "")
                {
                    loExcel.InsertPicture(lsHoja0, psLogoKeytiaPath, "D1", "S5", false, false);
                }


                loExcel.BuscarTexto(lsHoja0, "{LogoCliente}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, "");
                }
                loExcel.BuscarTexto(lsHoja0, "{LogoKeytia}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, "");
                }

                loExcel.BuscarTexto(lsHoja0, "{TituloReporte}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, Globals.GetMsgWeb(false, "TituloEtiqueta"));
                }
                else
                {
                    liRow2 = 6;
                    loExcel.Actualizar(lsHoja0, liRow2, 1, Globals.GetMsgWeb(false, "TituloEtiqueta"));
                }

                loExcel.BuscarTexto(lsHoja0, "{HeaderEmple}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, pFields.GetByConfigName("Emple").Descripcion);
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, ++liRow2, 1, pFields.GetByConfigName("Emple").Descripcion);
                }
                loExcel.BuscarTexto(lsHoja0, "{Emple}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, liRow2, 2, pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                }

                loExcel.BuscarTexto(lsHoja0, "{HeaderCenCos}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, pFields.GetByConfigName("CenCos").Descripcion);
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, ++liRow2, 1, pFields.GetByConfigName("CenCos").Descripcion);
                }
                loExcel.BuscarTexto(lsHoja0, "{CenCos}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, liRow2, 2, pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                }

                if (lbIsDetLinea)
                {
                    loExcel.BuscarTexto(lsHoja0, "{HeaderNomColNumeroEtq}", false, out liRow, out liCol);
                    if (liRow > 0 && liCol > 0)
                    {
                        loExcel.Actualizar(lsHoja0, liRow, liCol, Globals.GetMsgWeb(false, "NomColNumeroEtq"));
                    }
                    else
                    {
                        loExcel.Actualizar(lsHoja0, ++liRow2, 1, Globals.GetMsgWeb(false, "NomColNumeroEtq"));
                    }
                    loExcel.BuscarTexto(lsHoja0, "{NomColNumeroEtq}", false, out liRow, out liCol);
                    if (liRow > 0 && liCol > 0)
                    {
                        loExcel.Actualizar(lsHoja0, liRow, liCol, "'" + pEtqDetLinea.Linea);
                    }
                    else
                    {
                        loExcel.Actualizar(lsHoja0, liRow2, 2, "'" + pEtqDetLinea.Linea);
                    }

                    loExcel.BuscarTexto(lsHoja0, "{HeaderGpoEtiqueta}", false, out liRow, out liCol);
                    if (liRow > 0 && liCol > 0)
                    {
                        loExcel.Actualizar(lsHoja0, liRow, liCol, pFields.GetByConfigName("GEtiqueta").Descripcion);
                    }
                    else
                    {
                        loExcel.Actualizar(lsHoja0, ++liRow2, 1, pFields.GetByConfigName("GEtiqueta").Descripcion);
                    }
                    loExcel.BuscarTexto(lsHoja0, "{GpoEtiqueta}", false, out liRow, out liCol);
                    if (liRow > 0 && liCol > 0)
                    {
                        loExcel.Actualizar(lsHoja0, liRow, liCol, pEtqDetLinea.Grupo);
                    }
                    else
                    {
                        loExcel.Actualizar(lsHoja0, liRow2, 2, pEtqDetLinea.Grupo);
                    }
                }
                loExcel.BuscarTexto(lsHoja0, "{HeaderIniPeriodo}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, pFields.GetByConfigName("IniPer").Descripcion);
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, ++liRow2, 1, pFields.GetByConfigName("IniPer").Descripcion);
                }
                loExcel.BuscarTexto(lsHoja0, "{IniPeriodo}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, IniPeriodo.Date.ToString(lsDateFormat));
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, liRow2, 2, IniPeriodo.Date.ToString(lsDateFormat));
                }

                loExcel.BuscarTexto(lsHoja0, "{HeaderFinPeriodo}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, pFields.GetByConfigName("FinPer").Descripcion);
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, ++liRow2, 1, pFields.GetByConfigName("FinPer").Descripcion);
                }
                loExcel.BuscarTexto(lsHoja0, "{FinPeriodo}", false, out liRow, out liCol);
                if (liRow > 0 && liCol > 0)
                {
                    loExcel.Actualizar(lsHoja0, liRow, liCol, FinPeriodo.Date.ToString(lsDateFormat));
                }
                else
                {
                    loExcel.Actualizar(lsHoja0, liRow2, 2, FinPeriodo.Date.ToString(lsDateFormat));
                }

                //Detalle
                loExcel.BuscarTexto(lsHoja0, "{GridResumen}", false, out liRow, out liCol);
                bool encontroGridResumen = false;
                if (liRow > 0 && liCol > 0)
                {
                    //NZ 20150821 Se agrega codigo para el estilo de la tabla en el excel.
                    //loExcel.Actualizar(lsHoja0, liRow, 1, loColumnas.GetUpperBound(0) + liRow, loColumnas.GetUpperBound(1) + 1, loColumnas);

                    encontroGridResumen = true;
                    ////NZ 20150821 Se agrega filtro para cuando es CIDE. El Reporte a exportar es diferente a lo que se muestra en web.
                    object[,] datos;

                    if (DSODataContext.Schema.ToLower() == "cide") //aqui debe de ser CIDE, prueba 
                    {
                        DataTable dtResult = DatosDetallado(int.Parse(iCodEmpleado));
                        EliminarColumnasDeAcuerdoABanderas(dtResult);
                        dtResult.Columns["Total"].ColumnName = "Importe";
                        if (dtResult.Columns.Contains("RID"))
                            dtResult.Columns.Remove("RID");
                        if (dtResult.Columns.Contains("RowNumber"))
                            dtResult.Columns.Remove("RowNumber");
                        if (dtResult.Columns.Contains("TopRID"))
                            dtResult.Columns.Remove("TopRID");
                        datos = loExcel.DataTableToArray(FCAndControls.daFormatoACeldas(dtResult), true);
                    }
                    else
                    {
                        datos = loExcel.DataTableToArray(FCAndControls.daFormatoACeldas(ldt), true);
                    }

                    EstiloTablaExcel estilo = new EstiloTablaExcel();
                    estilo.Estilo = "KeytiaGrid";
                    estilo.FilaEncabezado = true;
                    estilo.FilasBandas = true;
                    estilo.FilaTotales = false;
                    estilo.PrimeraColumna = false;
                    estilo.UltimaColumna = true;
                    estilo.ColumnasBandas = false;
                    estilo.AutoFiltro = false;
                    estilo.AutoAjustarColumnas = true;

                    loExcel.Actualizar(lsHoja0, "{GridResumen}", false, datos, estilo);

                }
                else
                {
                    liRow2++;
                    loExcel.Actualizar(lsHoja0, ++liRow2, 1, loColumnas.GetUpperBound(0) + liRow2, loColumnas.GetUpperBound(1) + 1, loColumnas);
                    liRow = liRow2;
                }

                if (encontroGridResumen == false)
                {
                    loExcel.Actualizar(lsHoja0, ++liRow, 1, loData.GetUpperBound(0) + liRow, loData.GetUpperBound(1) + 1, loData);
                }
                loExcel.FilePath = GetFileName(".xlsx");
                loExcel.SalvarComo();
            }
            catch { }
            finally
            {
                if (loExcel != null)
                {
                    loExcel.Cerrar(true);
                    loExcel.Dispose();
                    loExcel = null;
                }
            }
        }

        protected virtual void ExportTXTGrid(DataTable ldt, bool lbIsDetLinea)
        {
            TxtFileAccess lFileTXT = new TxtFileAccess();
            KDBAccess lKDB = new KDBAccess();
            StringBuilder lsRenglon = new StringBuilder();
            string lsData = "";
            string lsDateFormat = Globals.GetMsgWeb(false, "NetDateTimeFormat");

            try
            {
                lFileTXT.FileName = GetFileName(".csv");
                lFileTXT.Abrir();
                //Encabezado
                lFileTXT.Escribir("\"" + Globals.GetMsgWeb(false, "TituloEtiqueta") + "\"");
                lsRenglon.Length = 0;
                lsRenglon.Append(pFields.GetByConfigName("Emple").Descripcion + ": ");
                lsRenglon.Append(pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                lFileTXT.Escribir("\"" + lsRenglon.ToString() + "\"");
                lsRenglon.Length = 0;
                lsRenglon.Append(pFields.GetByConfigName("CenCos").Descripcion + ": ");
                lsRenglon.Append(pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                lFileTXT.Escribir("\"" + lsRenglon.ToString() + "\"");
                if (lbIsDetLinea)
                {
                    lsRenglon.Length = 0;
                    lsRenglon.Append(Globals.GetMsgWeb(false, "NomColNumeroEtq") + ": ");
                    lsRenglon.Append(pEtqDetLinea.Linea);
                    lFileTXT.Escribir("\"" + lsRenglon.ToString() + "\"");
                    lsRenglon.Length = 0;
                    lsRenglon.Append(pFields.GetByConfigName("GEtiqueta").Descripcion + ": ");
                    lsRenglon.Append(pEtqDetLinea.Grupo);
                    lFileTXT.Escribir("\"" + lsRenglon.ToString() + "\"");
                }
                lsRenglon.Length = 0;
                lsRenglon.Append(pFields.GetByConfigName("IniPer").Descripcion + ": ");
                lsRenglon.Append(IniPeriodo.Date.ToString(lsDateFormat));
                lsRenglon.Append("        ");
                lsRenglon.Append(pFields.GetByConfigName("FinPer").Descripcion + ": ");
                lsRenglon.Append(FinPeriodo.Date.ToString(lsDateFormat));
                lFileTXT.Escribir("\"" + lsRenglon.ToString() + "\"");
                lFileTXT.Escribir("");

                //Detalle
                foreach (DataColumn lCol in ldt.Columns)
                {
                    lsData = lsData + lCol.ColumnName + ",";
                }
                lFileTXT.Escribir(lsData);

                foreach (DataRow lRow in ldt.Rows)
                {
                    lsData = "";
                    foreach (DataColumn lCol in ldt.Columns)
                    {
                        lsData = lsData + "\"" + lRow[lCol].ToString() + "\"" + ",";
                    }
                    lFileTXT.Escribir(lsData);
                }
            }
            catch { }
            finally
            {
                if (lFileTXT != null)
                {
                    lFileTXT.Cerrar();
                    lFileTXT = null;
                }
            }
        }

        protected virtual void ExportWordGrid(DataTable ldt, bool lbIsDetLinea, string lsExt)
        {
            string lsWordPath = buscarPlantilla(lbIsDetLinea, false, ".docx");
            WordAccess loWord = new WordAccess();
            string lsDateFormat = Globals.GetMsgWeb(false, "NetDateTimeFormat");

            try
            {
                //NZ 20150830 Si es CIDE la consulta del reporte a exportar es diferente.
                if (DSODataContext.Schema.ToLower() == "cide") //aqui debe de ser CIDE //Prueba 
                {
                    DataTable dtResult = DatosDetallado(int.Parse(iCodEmpleado));
                    EliminarColumnasDeAcuerdoABanderas(dtResult);
                    dtResult.Columns["Total"].ColumnName = "Importe";
                    if (dtResult.Columns.Contains("RID"))
                        dtResult.Columns.Remove("RID");
                    if (dtResult.Columns.Contains("RowNumber"))
                        dtResult.Columns.Remove("RowNumber");
                    if (dtResult.Columns.Contains("TopRID"))
                        dtResult.Columns.Remove("TopRID");
                    ldt = dtResult;
                }

                if (lsWordPath != "")
                {
                    //Con Plantilla
                    loWord.FilePath = lsWordPath;
                    loWord.Abrir(true);
                    if (System.IO.File.Exists(psLogoClientePath))
                    {
                        loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
                    }
                    else
                    {
                        loWord.ReemplazarTexto("{LogoCliente}", "");

                    }
                    if (System.IO.File.Exists(psLogoKeytiaPath))
                    {
                        loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
                    }
                    else
                    {
                        loWord.ReemplazarTexto("{LogoKeytia}", "");
                    }

                    loWord.ReemplazarTexto("{TituloReporte}", Globals.GetMsgWeb(false, "TituloEtiqueta"));
                    loWord.ReemplazarTexto("{HeaderEmple}", pFields.GetByConfigName("Emple").Descripcion);
                    loWord.ReemplazarTexto("{Emple}", pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                    loWord.ReemplazarTexto("{HeaderCenCos}", pFields.GetByConfigName("CenCos").Descripcion);
                    loWord.ReemplazarTexto("{CenCos}", pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                    loWord.ReemplazarTexto("{HeaderIniPeriodo}", pFields.GetByConfigName("IniPer").Descripcion);
                    loWord.ReemplazarTexto("{IniPeriodo}", IniPeriodo.Date.ToString(lsDateFormat));
                    loWord.ReemplazarTexto("{HeaderFinPeriodo}", pFields.GetByConfigName("FinPer").Descripcion);
                    loWord.ReemplazarTexto("{FinPeriodo}", FinPeriodo.Date.ToString(lsDateFormat));
                    if (lbIsDetLinea)
                    {
                        loWord.ReemplazarTexto("{HeaderNomColNumeroEtq}", Globals.GetMsgWeb(false, "NomColNumeroEtq"));
                        loWord.ReemplazarTexto("{NomColNumeroEtq}", pEtqDetLinea.Linea);
                        loWord.ReemplazarTexto("{HeaderGpoEtiqueta}", pFields.GetByConfigName("GEtiqueta").Descripcion);
                        loWord.ReemplazarTexto("{GpoEtiqueta}", pEtqDetLinea.Grupo);
                    }
                    //Detalle
                    loWord.ReemplazarTexto("{GridResumen}", "");
                    try
                    {
                        loWord.InsertarTabla(ldt, true, "KeytiaGrid");
                    }
                    catch
                    {
                        loWord.InsertarTabla(ldt, true);
                    }
                }
                else
                {
                    //Sin Plantilla
                    loWord.Abrir(true);
                    if (System.IO.File.Exists(psLogoClientePath))
                    {
                        loWord.InsertarImagen(psLogoClientePath);
                        loWord.NuevoParrafo();
                    }
                    if (System.IO.File.Exists(psLogoKeytiaPath))
                    {
                        loWord.InsertarImagen(psLogoKeytiaPath);
                        loWord.NuevoParrafo();
                    }

                    loWord.InsertarTexto(Globals.GetMsgWeb(false, "TituloEtiqueta"));
                    loWord.NuevoParrafo();
                    loWord.InsertarTexto(pFields.GetByConfigName("Emple").Descripcion + ": " + pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                    loWord.NuevoParrafo();
                    loWord.InsertarTexto(pFields.GetByConfigName("CenCos").Descripcion + ": " + pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                    if (lbIsDetLinea)
                    {
                        loWord.NuevoParrafo();
                        loWord.InsertarTexto(pFields.GetByConfigName("TelDest").Descripcion + ": " + pEtqDetLinea.Linea);
                        loWord.NuevoParrafo();
                        loWord.InsertarTexto(pFields.GetByConfigName("GEtiqueta").Descripcion + ": " + pEtqDetLinea.Grupo);
                    }
                    loWord.NuevoParrafo();
                    loWord.InsertarTexto(pFields.GetByConfigName("IniPer").Descripcion + ": " + IniPeriodo.Date.ToString(lsDateFormat));
                    loWord.NuevoParrafo();
                    loWord.InsertarTexto(pFields.GetByConfigName("FinPer").Descripcion + ": " + FinPeriodo.Date.ToString(lsDateFormat));
                    //Detalle
                    loWord.NuevoParrafo();
                    loWord.NuevoParrafo();
                    loWord.InsertarTabla(ldt, true);
                }

                string lsFileName = GetFileName(lsExt);
                loWord.FilePath = lsFileName;
                loWord.SalvarComo();
            }
            catch { }
            finally
            {
                if (loWord != null)
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord = null;
                }
            }
        }

        #endregion


        #region CorreoElectronico

        protected string GetFileNameCorreo(int liCodEmpleado) //RJ.20160620
        {
            return GetFileNameCorreo(liCodEmpleado, false);
        }

        protected string GetFileNameCorreo(int liCodEmpleado, bool lbNombreUnico) //RJ.20160620
        {
            DataTable ldtEmpleado = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + liCodEmpleado);
            string lsFileName = ldtEmpleado.Rows[0]["vchCodigo"].ToString().Trim() + ".docx";
            if (lbNombreUnico)
            {
                lsFileName = Guid.NewGuid().ToString() + "_" + lsFileName;
            }
            System.IO.Directory.CreateDirectory(psTempPath);
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(psTempPath, lsFileName));
        }

        protected string buscarPlantilla(bool lbIsDetLinea, bool lbIsMail, string lsExt) //RJ.20160620
        {
            string lsFilePath = "";
            string lsFile = "";
            lsFilePath = Session["StyleSheet"].ToString().Replace("/", "\\");
            lsFilePath = lsFilePath.Replace("~", HttpContext.Current.Server.MapPath("~"));
            lsFilePath = lsFilePath.Replace(@"\\", @"\");
            if (lbIsDetLinea)
            {
                lsFile = "PlantillaEtqLin" + lsExt;
            }
            else if (lbIsMail)
            {
                lsFile = "PlantillaEtqMail" + lsExt;
            }
            else
            {
                lsFile = "PlantillaEtq" + lsExt;
            }
            lsFilePath = System.IO.Path.Combine(lsFilePath, @"plantillas\Etiquetacion\" + lsFile);
            if (!System.IO.File.Exists(lsFilePath))
            {
                return "";
            }
            return lsFilePath;
        }

        /// <summary>
        /// Prepara y envía el correo de confirmación, adjuntando el archivo de word
        /// con el reporte de números etiquetados
        /// </summary>
        protected virtual void EnviarCorreo() //RJ.20160620
        {
            int liCodEmpleado = int.Parse(iCodEmpleado);
            string lsDateFormat = Globals.GetMsgWeb(false, "NetDateTimeFormat");

            DataTable ldt;

            //Obtiene el listado de números con los que generó consumo el empleado
            //con su Etiqueta, su grupo y su gasto
            if (DSODataContext.Schema.ToLower() != "cide")
            {
                ldt = GetExportGrid("WORD");
            }
            else
            {
                //RJ.20160108 Este cliente nos solicitó un reporte especial en su correo de confirmación
                ldt = GetExportGridCIDE();
            }


            //Obtiene la ruta y nombre del archivo que se utiliza como plantilla
            string lsWordPath = buscarPlantilla(false, true, ".docx");

            WordAccess loWord = new WordAccess();
            if (lsWordPath != "")
            {
                //Con Plantilla
                loWord.FilePath = lsWordPath;

                loWord.Abrir(true);  //Abre la plantilla

                //Agrega logo del cliente
                if (System.IO.File.Exists(psLogoClientePath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoCliente}", "");
                }

                //Agrega logo de Keytia
                if (System.IO.File.Exists(psLogoKeytiaPath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoKeytia}", "");
                }

                //Reemplaza los placeholders con los datos del reporte
                loWord.ReemplazarTexto("{TituloReporte}", Globals.GetMsgWeb(false, "TituloEtiqueta"));
                loWord.ReemplazarTexto("{HeaderEmple}", pFields.GetByConfigName("Emple").Descripcion);
                loWord.ReemplazarTexto("{Emple}", pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                loWord.ReemplazarTexto("{HeaderCenCos}", pFields.GetByConfigName("CenCos").Descripcion);
                loWord.ReemplazarTexto("{CenCos}", pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                loWord.ReemplazarTexto("{HeaderIniPeriodo}", pFields.GetByConfigName("IniPer").Descripcion);
                loWord.ReemplazarTexto("{IniPeriodo}", IniPeriodo.Date.ToString(lsDateFormat));
                loWord.ReemplazarTexto("{HeaderFinPeriodo}", pFields.GetByConfigName("FinPer").Descripcion);
                loWord.ReemplazarTexto("{FinPeriodo}", FinPeriodo.Date.ToString(lsDateFormat));
                loWord.ReemplazarTexto("{HeaderNumTotPersonal}", pNumTotPersonal.Descripcion);
                loWord.ReemplazarTexto("{NumTotPersonal}", (pNumTotPersonal.DataValue.ToString() == "null" || pNumTotPersonal.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotPersonal.DataValue.ToString()));
                loWord.ReemplazarTexto("{HeaderNumTotLaboral}", pNumTotLaboral.Descripcion);
                loWord.ReemplazarTexto("{NumTotLaboral}", (pNumTotLaboral.DataValue.ToString() == "null" || pNumTotLaboral.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotLaboral.DataValue.ToString()));
                loWord.ReemplazarTexto("{HeaderNumTotNI}", pNumTotNI.Descripcion);
                loWord.ReemplazarTexto("{NumTotNI}", (pNumTotNI.DataValue.ToString() == "null" || pNumTotNI.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotNI.DataValue.ToString()));
                loWord.ReemplazarTexto("{HeaderNumTotal}", pNumTotal.Descripcion);
                loWord.ReemplazarTexto("{NumTotal}", (pNumTotal.DataValue.ToString() == "null" || pNumTotal.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotal.DataValue.ToString()));



                //Inserta el detalle obtenido en el método GetExportGrid
                loWord.ReemplazarTexto("{GridResumen}", "");


                try
                {
                    loWord.InsertarTabla(ldt, true, "KeytiaGrid");
                }
                catch (Exception ex)
                {
                    loWord.InsertarTabla(ldt, true);
                }


            }
            else
            {
                //Sin Plantilla
                loWord.Abrir(true);
                if (System.IO.File.Exists(psLogoClientePath))
                {
                    loWord.InsertarImagen(psLogoClientePath);
                }
                if (System.IO.File.Exists(psLogoKeytiaPath))
                {
                    loWord.InsertarImagen(psLogoKeytiaPath);
                }
                loWord.InsertarTexto(Globals.GetMsgWeb(false, "TituloEtiqueta"));
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("Emple").Descripcion + ": " + pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("CenCos").Descripcion + ": " + pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("IniPer").Descripcion + ": " + IniPeriodo.Date.ToString(lsDateFormat));
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("FinPer").Descripcion + ": " + FinPeriodo.Date.ToString(lsDateFormat));
                //Detalle
                loWord.NuevoParrafo();
                loWord.NuevoParrafo();
                loWord.InsertarTabla(ldt, true);
            }


            //Obtiene la ruta y nombre del archivo en donde se guardará 
            //la información etiquetada (.doc). El nombre del archivo corresponde al
            //vchcodigo del Empleado
            string lsFileName = GetFileNameCorreo(liCodEmpleado);
            loWord.FilePath = lsFileName;


            try
            {
                loWord.SalvarComo(); //Guarda la plantilla como el nuevo archivo
            }
            catch (Exception ex)
            {
                lsFileName = GetFileNameCorreo(liCodEmpleado, true);
                loWord.FilePath = lsFileName;
                loWord.SalvarComo();
            }
            finally
            {
                if (loWord != null)
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord.Dispose();
                    loWord = null;
                }
            }


            //Configura y envía correo de confirmación
            poMail = new MailAccess();
            poMail.NotificarSiHayError = false;
            poMail.IsHtml = true;
            poMail.OnSendCompleted = SendCompleted;
            poMail.De = GetRemitente();
            poMail.Asunto = Globals.GetMsgWeb(false, "TituloEtiqueta") + ": " + IniPeriodo.Date.ToString(lsDateFormat) + " - " + FinPeriodo.Date.ToString(lsDateFormat);
            poMail.Adjuntos.Add(new Attachment(lsFileName));
            poMail.AgregarWord(lsFileName);
            poMail.CC.Add(psMailCC);
            poMail.BCC.Add(psMailCCO);
            poMail.Para.Add(GetMailPara());
            poMail.Enviar();
        }

        protected MailAddress GetRemitente()
        {
            if (string.IsNullOrEmpty(psMailRemitente))
            {
                return new MailAddress(Util.AppSettings("appeMailID"));
            }
            else if (string.IsNullOrEmpty(psNomRemitente))
            {
                return new MailAddress(psMailRemitente);
            }
            else
            {
                return new MailAddress(psMailRemitente, psNomRemitente);
            }
        }

        protected string GetMailPara()
        {
            DataTable lKDBTable = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + pFields.GetByConfigName("Emple").DataValue.ToString());
            psMailPara = "";

            if (lKDBTable.Rows.Count > 0)
            {
                DataRow lDataRow = lKDBTable.Rows[0];
                string lsMailEmple = (string)Util.IsDBNull(lDataRow["{Email}"], "");
                if (lsMailEmple.Length > 0 && !psMailPara.Contains(lsMailEmple))
                {
                    psMailPara = (psMailPara.Length > 0 ? (";" + lsMailEmple) : lsMailEmple);
                }

                if (pbEtiquetacionColaborador)
                {
                    DataTable ldtEmpleColaborador;
                    ldtEmpleColaborador = pKDB.GetHisRegByEnt("Emple", "Empleados", "{Usuar}='" + Session["iCodUsuario"].ToString() + "'");
                    if (ldtEmpleColaborador == null || ldtEmpleColaborador.Rows.Count == 0)
                    {
                        return psMailPara;
                    }
                    string liCodEmpleColaborador = Util.IsDBNull(ldtEmpleColaborador.Rows[0]["iCodCatalogo"], "").ToString();
                    if (liCodEmpleColaborador == iCodEmpleado)
                    {
                        return psMailPara;
                    }
                    //Si el empleado asignado al usuario de sesión no es igual al empleado que se consulta, agregar correo.
                    string lsMailColaborador = (string)Util.IsDBNull(ldtEmpleColaborador.Rows[0]["{Email}"], "");
                    if (lsMailColaborador.Length > 0 && !psMailPara.Contains(lsMailColaborador))
                    {
                        psMailPara = psMailPara + (psMailPara.Length > 0 ? (";" + lsMailColaborador) : lsMailColaborador);
                    }
                }
            }
            return psMailPara;
        }

        protected void SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) //RJ.20160620
        {
            int liCodEmpleado = (int)e.UserState;

            if (e.Error != null)
            {
                EnviarNotificacionCorreoNoValido(liCodEmpleado);
            }
            return;
        }

        protected void EnviarNotificacionCorreoNoValido(int liCodEmpleado) //RJ.20160620
        {
            DataTable ldtEmpleado = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + liCodEmpleado);
            string[] lsParam = new string[] { (string)Util.IsDBNull(ldtEmpleado.Rows[0]["{Email}"], ""), (string)Util.IsDBNull(ldtEmpleado.Rows[0]["vchDescripcion"], "") };
            string lsErrMailAsunto = Globals.GetLangItem("ErrMailAsuntoEtiqueta", null); // "Error de envío de correo automático";
            string lsMensaje = Globals.GetLangItem("ErrMailAsuntoEtiqueta", lsParam);
            if (lsErrMailAsunto.StartsWith("#undefined-"))
            {
                lsErrMailAsunto = "Error de envío de correo automático";
            }
            if (lsMensaje.StartsWith("#undefined-"))
            {
                lsMensaje = "Surgió un error durante el envío de correo automático\r\nPara: {0}\r\nEmpleado: {1}";
                lsMensaje = string.Format(lsMensaje, lsParam);
            }

            WordAccess loWord = new WordAccess();
            loWord.Abrir(true);
            KeytiaServiceBL.Alarmas.Alarma.encabezadoCorreo(loWord, liCodEmpleado);
            foreach (string lsLinea in lsMensaje.Split(new string[] { "\\r\\n" }, StringSplitOptions.None))
            {
                loWord.NuevoParrafo();
                loWord.InsertarTexto(lsLinea);
            }

            string lsFileName = GetFileNameCorreo(liCodEmpleado).Replace(".docx", "_NoValido.docx");
            loWord.FilePath = lsFileName;
            try
            {
                loWord.SalvarComo();
            }
            finally
            {
                if (loWord != null)
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord.Dispose();
                    loWord = null;
                }
            }

            string lsEmpleado = (string)KeytiaServiceBL.Util.IsDBNull(pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + liCodEmpleado).Rows[0]["vchDescripcion"], "");

            MailAccess loMail = new MailAccess();
            loMail.NotificarSiHayError = false;
            loMail.IsHtml = true;
            loMail.De = GetRemitente();
            loMail.Asunto = DSODataContext.Schema.ToString() + " " + Globals.GetMsgWeb(false, "TituloEtiqueta") + " " + lsEmpleado;
            loMail.Asunto = lsErrMailAsunto + ": " + loMail.Asunto;
            loMail.AgregarWord(lsFileName);
            loMail.Para.Add(psMailPara);
            loMail.EnviarAsincrono(liCodEmpleado);

        }

        #endregion

        #endregion


        #region Delegados

        protected virtual void EtiquetaEdit_Init(object sender, EventArgs e)
        {
            DataTable lKDBTable = pKDB.GetHisRegByEnt("Aplic", "Aplicaciones del Sistema", "iCodCatalogo = " + iCodAplicacion);

            SetMaestros();
            SetEmpleado();

            if (pbEnableEmpleado)
            {
                GetClientConfig();
                GetPeriodoEtiquetaIni();
            }
            else
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "UsuarioSinEmpleado"));
                DSOControl.jAlert(Page, pjsObj, lsError, lsTitulo);
            }

            if ((int)Util.IsDBNull(lKDBTable.Rows[0]["{ParamInteger1}"], 0) != 0 || !pbEnableEmpleado)
            {
                pbEtiquetacionColaborador = false;
                iCodEntidad = iCodEntidadEtq;
                iCodMaestro = iCodMaestroEtq;
            }

            //Valida Permiso Directorio Corporativo            
            string liCodPerfilConfig = pKDB.GetHisRegByCod("Perfil", new string[] { "Config" }, new string[] { "iCodCatalogo" }).Rows[0]["iCodCatalogo"].ToString();
            if (Session["iCodPerfil"].ToString() == liCodPerfilConfig)
            {
                pbDirCorporativo = true;
            }
            else
            {
                //Valida permisio directorio corporativo aplicable a perfil Administrador, RZ.20120607
                string liCodPerfilAdmin = pKDB.GetHisRegByCod("Perfil", new string[] { "Admin" }, new string[] { "iCodCatalogo" }).Rows[0]["iCodCatalogo"].ToString();

                if (Session["iCodPerfil"].ToString() == liCodPerfilAdmin)
                {
                    pbDirCorporativo = true;
                }
            }

            pFieldsNoVisibles = new string[] { "GEtiqueta", "TelDest" };

            // Controles para la seccion de Resumen                                    
            pbtnDirPersonal = new HtmlButton();
            pbtnDirCorporativo = new HtmlButton();
            pbtnVerDetalle = new HtmlButton();
            //plBlanco = new Label();
            pExpResumen = new DSOExpandable();
            pTablaResumen = new Table();
            pResumenGrid = new DSOGrid();
            piCodGrupo = new KeytiaDropDownOptionField();

            //NZ 201508112 Se incluye filtro por Tipo Destino  
            piCodTDest = new KeytiaDropDownOptionField();
            //

            pbPersonales = new DSOCheckBox();
            pbLaborales = new DSOCheckBox();
            pNumTotLaboral = new DSONumberEdit();
            pNumTotNI = new DSONumberEdit();
            pNumTotPersonal = new DSONumberEdit();
            pNumTotal = new DSONumberEdit();
            pTxtNumTotPServer = new DSOTextBox();
            pTxtNumTotLServer = new DSOTextBox();
            pTxtNumTotNServer = new DSOTextBox();
            pTxtNumTotServer = new DSOTextBox();
            pwndDetalle = new DSOWindow();
            pwndDetLinea = new DSOWindow();
            pTxtWindowVisible = new DSOTextBox();


            this.CssClass = "EtiquetaEdit";

            pToolBar.Controls.Add(pbtnVerDetalle);
            pToolBar.Controls.Add(pbtnDirCorporativo);
            pToolBar.Controls.Add(pbtnDirPersonal);

            Controls.Add(pExpResumen);
            Controls.Add(pResumenGrid);
            Controls.Add(pwndDetalle);
            Controls.Add(pwndDetLinea);
            Controls.Add(pTxtWindowVisible);
            Controls.Add(pTxtNumTotPServer);
            Controls.Add(pTxtNumTotLServer);
            Controls.Add(pTxtNumTotNServer);
            Controls.Add(pTxtNumTotServer);
        }

        #endregion


        #region ServerClick

        protected override void pbtnRegresar_ServerClick(object sender, EventArgs e)
        {
            SetEntidad(vchCodEntidad);
            SetMaestro(vchDesMaestro);
            SetEmpleado();
            PrevState = State;
            SetHistoricState(HistoricState.MaestroSeleccionado);
            iCodRegistro = "null";
            InitFields();
            pFields.FillControls();
            pFields.DisableFields();
            InitFiltros();
            CreateGrid();
            InitGrid();
            FirePostRegresarClick();
        }

        protected override void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            StringBuilder lsbMensajes = new StringBuilder();
            string lsMensaje = "";
            string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");

            FillAjaxControls();

            PrevState = State;

            if (State != HistoricState.Edicion)
            {
                return;
            }

            //GetPeriodo();
            if (ValidarRegistro())
            {

                //Inserta un registro en la vista visHistoricos('ProEtiqueta','Proceso Etiquetacion')
                // con los datos del Empleado, el periodo y los totales de cada categoría
                GrabarProcesoEtiqueta();


                if (pbEtiquetacionCorrecta)
                {


                    GrabarRegistro();


                    if (!pbEtiquetacionCorrecta)
                    {
                        BajaProcesoEtiqueta();
                        lsMensaje = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjEtiquetacionIncorr"));
                        lsbMensajes.Append("<li>" + lsMensaje + "</li>");
                        DSOControl.jAlert(Page, pjsObj + ".pbtnGrabar_ServerClick", lsbMensajes.ToString(), lsTitulo);
                    }
                    else
                    {
                        try
                        {
                            EnviarCorreo();
                        }
                        catch (Exception ex)
                        {
                            //No se pudo enviar el correo.
                            lsMensaje = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjCorreoEtqIncorr"));
                            lsbMensajes.Append("<li>" + lsMensaje + "</li>");
                            DSOControl.jAlert(Page, pjsObj + ".pbtnGrabar_ServerClick", lsbMensajes.ToString(), lsTitulo);
                            Util.LogException("No se pudo enviar el correo de etiquetación.", ex);
                        }



                        //Elimina de la vista visdetallados('detall','Resumen Etiquetacion Temp') 
                        //los datos del Empleado y del periodo correspondientes
                        psbQuery.Length = 0;
                        psbQuery.Append("Delete From [" + DSODataContext.Schema + "].[Detallados] ");
                        psbQuery.Append("Where iCodCatalogo = " + iCodEntidadResumen + " and {Emple} = " + iCodEmpleado + " ");
                        psbQuery.Append("and {IniPer} >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "' and {FinPer} <  '" + FinPeriodo.ToString("yyyy-MM-dd") + "' ");
                        psbQuery.Append("and iCodMaestro = " + iCodMaestroResumen);
                        pKDB.ExecuteQuery("Detall", vchDesMaestroResumen, psbQuery.ToString());
                        lsMensaje = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjEtiquetacionCorr"));
                        lsbMensajes.Append("<li>" + lsMensaje + "</li>");

                        //Sólo se corre la primera vez que se entra a la aplicación  
                        SetResumen(int.Parse(iCodEmpleado), IniPeriodo, FinPeriodo, piHisPreviaEtiqueta, int.Parse(iCodMaestroResumen), int.Parse(iCodEntidadResumen));
                        DSOControl.jAlert(Page, pjsObj + ".pbtnGrabar_ServerClick", lsbMensajes.ToString(), lsTitulo);
                    }
                }
                //ConsultarRegistro();
            }
            FirePostGrabarClick();
        }

        protected virtual void pbtnDirCorporativo_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            InitSubHisDirectorio("KeytiaWeb.UserInterface.CnfgDirectorioCorpEdit");
        }

        protected virtual void pbtnDirPersonal_ServerClick(object sender, EventArgs e)
        {

            PrevState = State;
            InitSubHisDirectorio("KeytiaWeb.UserInterface.CnfgDirectorioPersonalEdit");
        }

        protected virtual void pSubEtiqueta_PostCancelarClick(object sender, EventArgs e)
        {
            if (SubHistorico.State == HistoricState.Inicio || SubHistorico.State == HistoricState.MaestroSeleccionado)
            {
                this.RemoverSubHistorico();
                DisableFields();
            }
        }

        protected void FirePostDirectorioClick()
        {
            if (pPostDirectorioClick != null)
            {
                pPostDirectorioClick(this, new EventArgs());
            }
        }

        protected void ActualizaRegSesion()
        {
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            Hashtable phtValoresCampos = new Hashtable();

            phtValoresCampos.Add("{BanderasSesion}",
                KDBUtil.SearchScalar("Valores", KDBUtil.SearchICodCatalogo("Valores", "BanSesEtiqueto"), "{Value}"));

            try
            {
                cargasCOM.ActualizaRegistro("Historicos", "RegSesion", "RegSesion", phtValoresCampos, (int)Session["iCodRegSesion"], KeytiaServiceBL.DSODataContext.GetContext());
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Surgió un error al marcar etiquetación durante la sesión", ex);
            }
        }

        #endregion

        #region WebMethods
        public static string GetConsumoResumen(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, DateTime ldtCorte, DateTime ldtLimite)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                string lsJSON = "";
                DataTable ldt;
                StringBuilder lsQuery = new StringBuilder();
                DataTable ldtAtributos;
                if (DSODataContext.GetObject("Atrib") == null)
                {
                    ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                    DSODataContext.SetObject("Atrib", ldtAtributos);
                }
                else
                {
                    ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
                }

                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];
                StringBuilder lsbQuery = new StringBuilder();
                int libPerHabil = IsPeriodoValido(liCodEmpleado, ldtIniPeriodo, ldtFinPeriodo, ldtCorte, ldtLimite);
                string lsFuncionGet = "GetResumen";
                string lsMaestro = "Resumen Etiquetacion Temp";

                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select isnull(count(icodcatalogo),0) from" + "\r\n");
                lsbQuery.AppendLine("(select * from [" + DSODataContext.Schema + "].[VisHistoricos('ProEtiqueta','Proceso Etiquetacion','" + Globals.GetCurrentLanguage() + "')]) a" + "\r\n");
                lsbQuery.AppendLine("where a.Emple = " + liCodEmpleado.ToString() + "\r\n");
                lsbQuery.AppendLine("and a.IniPer >= '" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                lsbQuery.AppendLine("and a.FinPer <= '" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                lsbQuery.AppendLine("and a.dtIniVigencia <> a.dtFinVigencia");

                //20141119. RJ Omito la ejecución de esta consulta pues el valor de liCount siempre será cero
                //para que siempre utilice la función GetResumen
                //int liCount = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString());
                int liCount = 0;

                if (liCount > 0 && (libPerHabil == 0 || (libPerHabil == 1 && pbEtiquetarUnaVez)))
                {
                    lsFuncionGet = "GetResumenEnDet";
                    lsMaestro = "Detalle Proceso de Etiquetacion";
                }

                lsQuery.Length = 0;
                if (lsFuncionGet.ToLower() == "getresumen")
                {
                    lsQuery.Length = 0;
                    lsQuery.Append("select Consumo = Sum(IsNull(G.Float01,0)), Grupo = IsNull(G.Integer03,0) ");
                    lsQuery.Append(" from [" + DSODataContext.Schema + "].ViewGetResumen As G");
                    lsQuery.Append(" where iCodCatalogo01 = " + liCodEmpleado.ToString());
                    lsQuery.Append(" and Date01 >= '" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "'");
                    lsQuery.Append(" and Date02 < dateadd(day,1,'" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "')");
                    lsQuery.Append(" Group By G.Integer03 ");
                }
                else
                {
                    lsQuery.Append("select Consumo = Sum(IsNull(G.{CostoFac},0)), Grupo = IsNull(G.{GEtiqueta},0) ");
                    lsQuery.Append("from [" + DSODataContext.Schema + "]." + lsFuncionGet + "(" + liCodEmpleado.ToString() + "," + liCodIdioma + ",");
                    lsQuery.Append("'" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "','" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "') As G Group By G.{GEtiqueta}");
                }

                ldt = lKDB.ExecuteQuery("Detall", lsMaestro, lsQuery.ToString());
                lsJSON = DSOControl.SerializeJSON<DataTable>(ldt);
                return lsJSON;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrConsumos", e, lsTitulo);
            }
        }

        public static void SetResumen(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, int liHisPrevEtq, int liCodMaestroResum, int liCodEntidadResum)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            DataTable ldtAtributos;
            KDBAccess lKDB = new KDBAccess();
            StringBuilder lsbQuery = new StringBuilder();
            if (DSODataContext.GetObject("Atrib") == null)
            {
                ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                DSODataContext.SetObject("Atrib", ldtAtributos);
            }
            else
            {
                ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
            }
            int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

            try
            {
                int piEtiquetarUnaVez = (pbEtiquetarUnaVez == true ? 1 : 0);
                lsbQuery.Length = 0;
                lsbQuery.Append("exec spSetResumen '" + DSODataContext.Schema + "'," + liCodEmpleado + "," + liCodIdioma + ",");
                lsbQuery.Append("'" + ldtIniPeriodo.Date.ToString("yyyy-MM-dd") + "','" + ldtFinPeriodo.Date.ToString("yyyy-MM-dd") + "'");
                lsbQuery.Append("," + liHisPrevEtq + "," + liCodEntidadResum + "," + liCodMaestroResum + ",'" + pdtCorte.ToString("yyyy-MM-dd") + "','" + pdtLimite.ToString("yyyy-MM-dd") + "'");
                lsbQuery.Append("," + piEtiquetarUnaVez.ToString());
                DSODataAccess.Execute(lsbQuery.ToString());
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static int IsPeriodoValido(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, DateTime ldtCorte, DateTime ldtLimite)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                int piEtiquetarUnaVez = (pbEtiquetarUnaVez == true ? 1 : 0);
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.Append("Select bHabil = [" + DSODataContext.Schema + "].ValidaPeriodo(" + liCodEmpleado + ",'" + ldtCorte.ToString("yyyy-MM-dd") + "','" + ldtLimite.ToString("yyyy-MM-dd") + "',");
                lsbQuery.Append("'" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "','" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "'," + piEtiquetarUnaVez.ToString() + ")");
                Int16 libPerHabil = Int16.Parse(DSODataAccess.ExecuteScalar(lsbQuery.ToString()).ToString());
                return libPerHabil;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrValidatePer", e, lsTitulo);
            }
        }

        public static void SetAllGrupo(int liCodEmpleado, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, int liCodMaestroResum, int liCodEntidadResum, int liCheck, int liValor)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                StringBuilder lsbQuery = new StringBuilder();
                KDBAccess lKDB = new KDBAccess();
                DataTable ldtAtributos;
                if (DSODataContext.GetObject("Atrib") == null)
                {
                    ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                    DSODataContext.SetObject("Atrib", ldtAtributos);
                }
                else
                {
                    ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
                }
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                lsbQuery.Length = 0;
                if (liCheck == 0)
                {
                    lsbQuery.AppendLine("Update D set D.{GEtiqueta} = R.{GEtiquetaIni} From ");
                    lsbQuery.AppendLine("[" + DSODataContext.Schema + "].[Detallados] D, ");
                    lsbQuery.AppendLine("[" + DSODataContext.Schema + "].ViewGetResumen R ");
                    lsbQuery.AppendLine(" where iCodCatalogo01 = " + liCodEmpleado.ToString());
                    lsbQuery.AppendLine(" and date01 >= '" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "'");
                    lsbQuery.AppendLine(" and date02 < dateadd(day,1,'" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "')   ");
                    lsbQuery.AppendLine(" and D.iCodRegistro = R.iCodRegistro and D.iCodMaestro = " + liCodMaestroResum.ToString());
                }
                else
                {
                    lsbQuery.AppendLine("Update D set D.{GEtiqueta} = " + liValor + ",");
                    lsbQuery.AppendLine("       D.{EdodeReg} = 2--Registros Actualizados");
                    lsbQuery.AppendLine("From [" + DSODataContext.Schema + "].[Detallados] D, ");
                    lsbQuery.AppendLine("   [" + DSODataContext.Schema + "].ViewGetResumen R ");
                    lsbQuery.AppendLine(" where iCodCatalogo01 = " + liCodEmpleado.ToString());
                    lsbQuery.AppendLine(" and date01 >= '" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "'");
                    lsbQuery.AppendLine(" and date02 < dateadd(day,1,'" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "')   ");
                    lsbQuery.AppendLine(" and D.iCodRegistro = R.iCodRegistro and R.EdodeReg > 0 and D.iCodMaestro = " + liCodMaestroResum.ToString());
                }


                lKDB.ExecuteQuery("Detall", "Resumen Etiquetacion Temp", lsbQuery.ToString());
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static void UpdateRowResumen(int liCodEmpleado, int liCodMaestroResum, int liCodEntidadResum, int liRegistro, string lsCampo, string lsValor)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                StringBuilder lsbQuery = new StringBuilder();
                KDBAccess lKDB = new KDBAccess();

                lsbQuery.Length = 0;
                lsbQuery.AppendLine("Update [" + DSODataContext.Schema + "].[Detallados] set ");
                lsbQuery.AppendLine(lsCampo + " = '" + lsValor + "',");
                lsbQuery.AppendLine("{EdodeReg} = 2 --Actualizado");
                lsbQuery.AppendLine("Where iCodRegistro = " + liRegistro.ToString() + " and ");
                lsbQuery.AppendLine("{Emple} = " + liCodEmpleado.ToString() + " and ");
                lsbQuery.AppendLine("iCodMaestro = " + liCodMaestroResum.ToString() + " and ");
                lsbQuery.AppendLine("iCodCatalogo = " + liCodEntidadResum.ToString());

                lKDB.ExecuteQuery("Detall", "Resumen Etiquetacion Temp", lsbQuery.ToString());
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetResumen(DSOGridServerRequest gsRequest, int liCodEmpleado, int liCodMaestroResum, int liCodEntidad, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo, DateTime ldtCorte, DateTime ldtLimite, int liHisPrevEtq, int liSetResumen)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                DataTable ldtAtributos;
                if (DSODataContext.GetObject("Atrib") == null)
                {
                    ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                    DSODataContext.SetObject("Atrib", ldtAtributos);
                }
                else
                {
                    ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
                }
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                StringBuilder lsbQuery = new StringBuilder();
                int libPerHabil = IsPeriodoValido(liCodEmpleado, ldtIniPeriodo, ldtFinPeriodo, ldtCorte, ldtLimite);
                string lsFuncionGet = "GetResumen";

                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select isnull(count(icodcatalogo),0) from" + "\r\n");
                lsbQuery.AppendLine("(select * from [" + DSODataContext.Schema + "].[VisHistoricos('ProEtiqueta','Proceso Etiquetacion','" + Globals.GetCurrentLanguage() + "')]) a" + "\r\n");
                lsbQuery.AppendLine("where a.Emple = " + liCodEmpleado + "\r\n");
                lsbQuery.AppendLine("and a.IniPer >= '" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                lsbQuery.AppendLine("and a.FinPer <= '" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "'" + "\r\n");
                lsbQuery.AppendLine("and a.dtIniVigencia <> a.dtFinVigencia");

                //20141119. RJ Omito la ejecución de esta consulta pues el valor de liCount siempre será cero
                //para que siempre utilice la función GetResumen
                //int liCount = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString());
                int liCount = 0;

                if (liCount > 0 && (libPerHabil == 0 || (libPerHabil == 1 && pbEtiquetarUnaVez)))
                {
                    lsFuncionGet = "GetResumenEnDet";
                }
                else if (liSetResumen == 1 || pbNuevaEtiquetacion == true)
                {
                    SetResumen(liCodEmpleado, ldtIniPeriodo, ldtFinPeriodo, liHisPrevEtq, liCodMaestroResum, liCodEntidad);
                    pbNuevaEtiquetacion = false;
                }

                HistoricFieldCollection lFields = new HistoricFieldCollection(liCodEntidad, liCodMaestroResum);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbTotalRecords = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if (!lFields.Contains(lsOrderCol))
                    {
                        lsOrderCol = "vchDescripcion";
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
                    lsOrderCol = "vchDescripcion";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////
                lgsrRet.sColumns = "EdodeReg";
                if (libPerHabil == 1)
                {
                    lsbColumnas.AppendLine("EdodeReg");
                }
                else
                {
                    liHisPrevEtq = 1;  //Muestra la etiqueta previa
                    //lsbColumnas.AppendLine("EdodeReg = 1");
                    lsbColumnas.AppendLine("EdodeReg = 0");
                }

                lgsrRet.sColumns += ",iCodRegistro";
                lsbColumnas.AppendLine(",iCodRegistro");

                lgsrRet.sColumns += ",vchDescripcion";
                lsbColumnas.AppendLine(",vchDescripcion");

                lgsrRet.sColumns += ",vchDescLocalidad";
                lsbColumnas.AppendLine(",vchDescLocalidad");

                lgsrRet.sColumns += ",vchDescTpDestino";
                lsbColumnas.AppendLine(",vchDescTpDestino");

                lgsrRet.sColumns += ",GEtiqueta";
                lsbColumnas.AppendLine(",GEtiqueta");

                foreach (KeytiaBaseField lField in lFields)
                {
                    lgsrRet.sColumns += "," + lField.Column;
                    lsbColumnas.AppendLine("," + lField.Column);
                }

                //////////////////////////////////////////////////////////////////////////7

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir);

                bool lbPrimero = true;

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(a.vchDescripcion,'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (lField.Column.StartsWith("Date"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlTimeFormat + "),'')");
                        }
                        else if (!(lField.Column.StartsWith("VarChar"))
                            || lField.Column.StartsWith("iCodCatalogo"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + lField.Column + "),'')");
                        }
                        else
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(" + lField.Column + ",'')");
                        }
                    }
                    lsbWhere.AppendLine("where " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                    lbPrimero = false;
                }

                string[] lsColumns = gsRequest.sColumns.Split(',');
                int lidx;
                for (lidx = 0; lidx < lsColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = lsColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.Contains(lsColumn)
                            || lsColumn == "GEtiqueta"
                            || lsColumn == "vchDescripcion"))
                        {
                            if (lbPrimero)
                            {
                                lsbWhere.Append("where ");
                                lbPrimero = false;
                            }
                            else
                            {
                                lsbWhere.Append("and ");
                            }
                            if (lsColumn.StartsWith("Date"))
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a." + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a." + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                DataTable dtTDest = new DataTable();
                dtTDest = GetTDest();

                string lsSelectCount = "select count(iCodMaestro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFromGetResumen = "from [" + DSODataContext.Schema + "]." + lsFuncionGet + "(" + liCodEmpleado + "," + liCodIdioma + ",'" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "','" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "') a \r\n";
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();
                lsbTotalRecords.Length = 0;

                lgsrRet.sEcho = gsRequest.sEcho;
                if (lsFuncionGet == "GetResumen")
                {
                    lsbTotalRecords.AppendLine("select count(T.TelDest) From");

                    lsbTotalRecords.AppendLine("(   select Distinct varchar01 as TelDest, iCodCatalogo06 as Locali ");
                    lsbTotalRecords.AppendLine("    From [" + DSODataContext.Schema + "].Detallados ");
                    lsbTotalRecords.AppendLine("    where iCodMaestro = 89 ");
                    lsbTotalRecords.AppendLine("    and iCodCatalogo09 = " + liCodEmpleado.ToString());
                    lsbTotalRecords.AppendLine("    and Date01 >= '" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "'");
                    lsbTotalRecords.AppendLine("    and Date01 < DateAdd(Day,1,'" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "')");

                    if (!pbEtiquetarIncluirLlamadasEntrada)
                    {
                        //Si esta bandera No esta prendida...se excluyen llamadas de entrada.
                        lsbTotalRecords.AppendLine(" and varchar07 <> 'Entrada'");
                    }

                    //NZ 20170425 Se excluyen las llamadas de enlace para todos los clientes.
                    lsbTotalRecords.AppendLine(" AND iCodCatalogo05 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Enl")["iCodCatalogo"].ToString());

                    if (DSODataContext.Schema.ToLower() == "cide")
                    {
                        lsbTotalRecords.AppendLine(" AND iCodCatalogo05 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Local")["iCodCatalogo"].ToString());
                        lsbTotalRecords.AppendLine(" AND iCodCatalogo05 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "LDNac")["iCodCatalogo"].ToString());
                    }

                    lsbTotalRecords.AppendLine(" GROUP BY varchar01, iCodCatalogo06 ");
                    lsbTotalRecords.AppendLine(" ) T"); ;
                }
                else
                {
                    lsbTotalRecords.Append(lsSelectCount + lsFromGetResumen);
                }

                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsbTotalRecords.ToString());


                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFromGetResumen + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                //NZ 20170425 Se Exluyen las llamadas de enlace en todos los esquemas del reporte que se visualiza en web para etiquetar.
                if (string.IsNullOrEmpty(lsWhere))
                {
                    lsWhere = " WHERE iCodCatalogo03 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Enl")["iCodCatalogo"].ToString();
                }
                else
                {
                    lsWhere = lsWhere + " AND iCodCatalogo03 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Enl")["iCodCatalogo"].ToString();
                }
                //NZ 20170425 Para CIDE de excluyen las llamadas de Local y Local Nacional
                if (DSODataContext.Schema.ToLower() == "cide") //Prueba 
                {
                    if (string.IsNullOrEmpty(lsWhere))
                    {
                        lsWhere = " WHERE iCodCatalogo03 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Local")["iCodCatalogo"].ToString();
                        lsWhere = lsWhere + " AND iCodCatalogo03 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "LDNac")["iCodCatalogo"].ToString();
                    }
                    else
                    {
                        lsWhere = lsWhere + " AND iCodCatalogo03 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Local")["iCodCatalogo"].ToString();
                        lsWhere = lsWhere + " AND iCodCatalogo03 <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "LDNac")["iCodCatalogo"].ToString();
                    }
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetResumen + lsWhere + lsOrderBy);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt);

                //NZ 20150825 Se establecen las fechas que estan en pantalla en las variables de fechas que se usan para exportar info.
                fechaInicioReal = ldtIniPeriodo;
                fechaFinReal = ldtFinPeriodo;
                //

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetHisEmple(DSOGridServerRequest gsRequest, int liCodEntidad, int liCodMaestro, int liCodEmpleado)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(liCodEntidad, liCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
                DataTable ldtAtributos;
                if (DSODataContext.GetObject("Atrib") == null)
                {
                    ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                    DSODataContext.SetObject("Atrib", ldtAtributos);
                }
                else
                {
                    ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
                }
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
                    lsOrderCol = "dtFecUltAct";
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
                    lsbColumnas.AppendLine("," + lField.Column);
                }

                lsbColumnas.AppendLine(",dtIniVigencia");
                lsbColumnas.AppendLine(",dtFinVigencia");
                lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from Historicos a");
                lsbFrom.AppendLine("      where iCodMaestro = " + liCodMaestro);
                lsbFrom.AppendLine("      and iCodCatalogo in(select iCodRegistro from Catalogos where iCodCatalogo = " + liCodEntidad + ")");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' < dtFinVigencia");

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

                bool lbPrimero = true;

                StringBuilder lsbWhereJerarquia = new StringBuilder();
                lsbWhereJerarquia.Append("where (iCodCatalogo = '" + liCodEmpleado + "' or iCodCatalogo in ");
                lsbWhereJerarquia.Append("(select iCodCatalogo from " + DSODataContext.Schema + ".GetJerarquiaEntidad(" + liCodEntidad + ",");
                lsbWhereJerarquia.Append(liCodEmpleado + ", '" + DateTime.Today.ToString("yyyy-MM-dd") + "')))");
                lbPrimero = false;

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
                        if (lField.Column.StartsWith("Date"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlTimeFormat + "),'')");
                        }
                        else if (!(lField.Column.StartsWith("VarChar")
                            || lField.Column.StartsWith("iCodCatalogo")))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + lField.Column + "),'')");
                        }
                        else
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(" + lField.Column + ",'')");
                        }
                    }
                    lsbWhere.AppendLine("and " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                }

                string[] lsColumns = gsRequest.sColumns.Split(',');
                int lidx;
                for (lidx = 0; lidx < lsColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = lsColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.Contains(lsColumn)
                            || lsColumn == "vchCodigo"
                            || lsColumn == "vchDescripcion"
                            || lsColumn == "dtIniVigencia"
                            || lsColumn == "dtFinVigencia"))
                        {
                            if (lbPrimero)
                            {
                                lsbWhere.Append("where ");
                                lbPrimero = false;
                            }
                            else
                            {
                                lsbWhere.Append("and ");
                            }
                            if (lsColumn.StartsWith("Date")
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a." + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a." + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsFromGetHisData = "from " + DSODataContext.Schema + ".GetHisData(" + liCodEntidad + "," + liCodMaestro + "," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "') a \r\n";
                string lsWhereJerarquia = lsbWhereJerarquia.ToString();
                string lsWhere = lsWhereJerarquia + lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + lsWhereJerarquia.Replace("where", "and"));
                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFromGetHisData + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetHisData + lsWhere + lsOrderBy);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }
        #endregion


        #region Consultas
        public string ConsultaDetalle(int iCodEmpleado)
        {
            //obtiene un detalle unicamente de la tabla de detalle CDR.
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)= ''  \r");
            lsb.Append("declare @OrderInv varchar(max)  \r");
            lsb.Append("			                         select  @Where = @Where + '[Fecha Inicio] >= ''" + fechaInicioReal.ToString("yyyy-MM-dd") + " 00:00:00''  \n");
            lsb.Append("                                                and [Fecha Inicio] <= ''" + fechaFinReal.ToString("yyyy-MM-dd") + " 23:59:59'''\n ");

            lsb.Append("select @Where = @Where + ' AND [iCodEmple] = " + iCodEmpleado.ToString() + "' \n");

            lsb.Append("exec ConsumoDetalladoEtiq   \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "',  \r");
            lsb.Append("@Fields=' \r");
            lsb.Append("[Centro de costos] , \r");
            lsb.Append("[Colaborador]	 , \r");
            lsb.Append("[Extensión]	 , \r");
            lsb.Append("[Numero Marcado] , \r");
            lsb.Append("[Fecha] , \r");
            lsb.Append("[Hora] , \r");
            lsb.Append("[Duracion] , \r");
            lsb.Append("[TotalSimulado] = (CostoFac+CostoSM), \r");
            lsb.Append("[TotalReal] = (Costo+CostoSM), \r");
            lsb.Append("[CostoSimulado] = (CostoFac), \r");
            lsb.Append("[CostoReal] = (Costo), \r");
            lsb.Append("[SM] = (CostoSM), \r");
            lsb.Append("[Nombre Localidad] as [Localidad], \r");
            lsb.Append("[Nombre Sitio]	as [Sitio] , \r");
            lsb.Append("[Codigo Autorizacion] , \r");
            lsb.Append("[Nombre Carrier] as [Carrier], \r");
            lsb.Append("[Tipo Llamada] , \r");
            lsb.Append("[Tipo de destino] ,  \r");
            lsb.Append("[Etiqueta] ,  \r");
            lsb.Append("[Grupo]  \r");
            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,   \r");
            lsb.Append("@Order = '[TotalSimulado] desc',  \r");
            lsb.Append("@OrderDir = 'Asc',  \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append(" @Idioma = '" + Session["Language"] + "', \r");
            lsb.Append("@FechaIniRep = '" + fechaInicioReal.ToString("yyyy-MM-dd") + " 00:00:00',  \r");
            lsb.Append("@FechaFinRep = '" + fechaFinReal.ToString("yyyy-MM-dd") + " 23:59:59'  \r");

            return lsb.ToString();
        }

        protected DataTable DatosDetallado(int iCodEmpleado) //RJ.20160620
        {
            EstablecerBanderasClientePerfil();
            DataTable dtResult = DSODataAccess.Execute(ConsultaDetalleCIDE(iCodEmpleado));
            return dtResult;
        }

        public string ConsultaDetalleCIDE(int iCodEmpleado)
        {
            //obtiene un detalle unicamente de la tabla de detalle CDR.   
            //FALLA AL MOMENTO DE EXPORTAR EL DETALLADO. NO TOMA EN CUENTA LAS FECHAS QUE ESTA VIZUALIZANDO POR QUE NO HACE POSTBACK
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("DECLARE @llamsEnlace INT;");
            lsb.AppendLine("DECLARE @llamsLocal INT;");
            lsb.AppendLine("DECLARE @llamsLDNac INT;");
            lsb.AppendLine("");
            lsb.AppendLine("SELECT ");
            lsb.AppendLine("    @llamsEnlace	= MAX(CASE WHEN vchCodigo = 'Enl' THEN iCodCatalogo ELSE 0 END),");
            lsb.AppendLine("	@llamsLocal		= MAX(CASE WHEN vchCodigo = 'Local' THEN iCodCatalogo ELSE 0 END),");
            lsb.AppendLine("	@llamsLDNac		= MAX(CASE WHEN vchCodigo = 'LDNac' THEN iCodCatalogo ELSE 0 END)");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".HistTDest ");
            lsb.AppendLine("WHERE dtFinVigencia >= GETDATE()");
            lsb.AppendLine("");
            lsb.AppendLine("");
            lsb.AppendLine("declare @Where varchar(max)= '' ");
            lsb.AppendLine("declare @OrderInv varchar(max)");

            lsb.Append("select  @Where = @Where + '[Fecha Inicio] >= ''" + fechaInicioReal.ToString("yyyy-MM-dd") + " 00:00:00''  ");
            lsb.AppendLine("                                                and [Fecha Inicio] <= ''" + fechaFinReal.ToString("yyyy-MM-dd") + " 23:59:59'''");


            lsb.AppendLine("select @Where = @Where + ' AND [iCodEmple] = " + iCodEmpleado.ToString() + "'");
            //NZ 20170425
            lsb.AppendLine("select @Where = @Where + ' AND [iCodTDest] <> ' +  CONVERT(VARCHAR,@llamsEnlace)");
            lsb.AppendLine("select @Where = @Where + ' AND [iCodTDest] <> ' +  CONVERT(VARCHAR,@llamsLocal)");
            lsb.AppendLine("select @Where = @Where + ' AND [iCodTDest] <> ' +  CONVERT(VARCHAR,@llamsLDNac)");

            if (!pbEtiquetarIncluirLlamadasEntrada)
            {
                //Si esta bandera No esta prendida...se excluyen llamadas de entrada.
                lsb.AppendLine("select @Where = @Where + ' AND [Tipo Llamada] <> ''ENTRADA'' '");
            }

            lsb.AppendLine("exec ConsumoDetalladoEtiq");
            lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Fields=' ");
            lsb.AppendLine("[Extensión]	 ,");
            lsb.AppendLine("[Numero Marcado], ");
            lsb.AppendLine("[Fecha] , ");
            lsb.AppendLine("[Hora] , ");
            lsb.AppendLine("[Duracion] as [Total (Min)] , ");
            lsb.AppendLine("[TotalSimulado] = (CostoFac+CostoSM), ");
            lsb.AppendLine("[TotalReal] = (Costo+CostoSM), ");
            lsb.AppendLine("[CostoSimulado] = (CostoFac), ");
            lsb.AppendLine("[CostoReal] = (Costo), ");
            lsb.AppendLine("[SM] = (CostoSM), ");
            lsb.AppendLine("[Grupo],  ");
            lsb.AppendLine("[Etiqueta]  ");
            lsb.AppendLine("', ");
            lsb.AppendLine("@Where = @Where,  ");
            lsb.AppendLine("@Order = '[TotalSimulado] desc', ");
            lsb.AppendLine("@OrderDir = 'Asc', ");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ", ");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ", ");
            lsb.AppendLine(" @Idioma = '" + Session["Language"] + "', ");
            lsb.AppendLine("@FechaIniRep = '" + fechaInicioReal.ToString("yyyy-MM-dd") + " 00:00:00', ");
            lsb.AppendLine("@FechaFinRep = '" + fechaFinReal.ToString("yyyy-MM-dd") + " 23:59:59' ");

            return lsb.ToString();
        }

        protected DataTable EliminarColumnasDeAcuerdoABanderas(DataTable Tabla)
        {
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns["CostoSimulado"].ColumnName = "Total";
                    Tabla.Columns["SM"].ColumnName = "Servicio Medido";
                }
                else
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns["CostoReal"].ColumnName = "Total";
                    Tabla.Columns["SM"].ColumnName = "Servicio Medido";
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalSimulado"].ColumnName = "Total";
                }
                else
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalReal"].ColumnName = "Total";
                }
            }

            return Tabla;
        }
        protected void EstablecerBanderasClientePerfil()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("SELECT BanderasCliente");
            consulta.AppendLine("FROM " + DSODataContext.Schema + ".HistClient");
            consulta.AppendLine("WHERE dtFinVigencia >= GETDATE()");
            consulta.AppendLine("AND UsuarDB = " + Session["iCodUsuarioDB"].ToString());
            consulta.AppendLine("AND (ISNULL(BanderasCliente,0) & 1024)/1024 = 1 ");

            DataTable dtConsulta = DSODataAccess.Execute(consulta.ToString());

            Session["MuestraSM"] = (dtConsulta.Rows.Count > 0) ? 1 : 0;

            StringBuilder consulta2 = new StringBuilder();
            consulta2.AppendLine("SELECT BanderasPerfil ");
            consulta2.AppendLine("FROM " + DSODataContext.Schema + ".HistPerfil ");
            consulta2.AppendLine("WHERE dtFinVigencia >= GETDATE()");
            consulta2.AppendLine("AND iCodCatalogo = " + Session["iCodPerfil"].ToString());
            consulta2.AppendLine("AND (ISNULL(BanderasPerfil,0) & 2)/2 = 1 ");

            DataTable dtConsulta2 = DSODataAccess.Execute(consulta2.ToString());

            Session["MuestraCostoSimulado"] = (dtConsulta2.Rows.Count > 0) ? 1 : 0;
        }
        #endregion

    }
}
