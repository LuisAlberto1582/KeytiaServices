using System;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CargasWebEdit : HistoricEdit
    {
        protected DSOExpandable pExpGridPend;
        protected Table pTablaPendientes;
        protected DSOGrid pPendGrid;
        protected DSODropDownList piCodMaestroPend;
        protected HistoricFieldCollection pFieldsPend;
        protected HtmlButton pbtnExpArchPend;

        protected DSOExpandable pExpPendFiltros;
        protected Table pTblPendFiltros;
        protected HtmlButton pbtnPendFiltro;
        protected HtmlButton pbtnLimpiaPendFiltro;
        protected Hashtable phtPendFiltros;

        protected StringBuilder psbErrores;
        protected string psFileKey;
        protected string psTempPath;

        protected int piCodCargaInicializada;
        protected int piCodCargaEnEspera;

        protected virtual string iCodCarga
        {
            get
            {
                return (string)ViewState["iCodCarga"];
            }
            set
            {
                ViewState["iCodCarga"] = value;
            }
        }

        protected virtual string iCodMaestroPend
        {
            get
            {
                return (string)ViewState["iCodMaestroPend"];
            }
            set
            {
                ViewState["iCodMaestroPend"] = value;
            }
        }

        public CargasWebEdit()
        {
            psFileKey = Guid.NewGuid().ToString();
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
            System.IO.Directory.CreateDirectory(psTempPath);
            Init += new EventHandler(CargasWebEdit_Init);

        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CargasWebEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/CargasWeb/CargasWebEdit.js") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected virtual void CargasWebEdit_Init(object sender, EventArgs e)
        {
            // Controles para la seccion de Pendientes
            pPendGrid = new DSOGrid();
            pExpGridPend = new DSOExpandable();
            pTablaPendientes = new Table();
            pExpPendFiltros = new DSOExpandable();
            pTblPendFiltros = new Table();
            pbtnPendFiltro = new HtmlButton();
            pbtnLimpiaPendFiltro = new HtmlButton();
            pbtnExpArchPend = new HtmlButton();

            Controls.Add(pPendGrid);
            pToolBar.Controls.Add(pbtnExpArchPend);

        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            pbtnExpArchPend.Attributes["class"] = "buttonXLS";
            pbtnExpArchPend.Style["display"] = "none";

            pbtnExpArchPend.ServerClick += new EventHandler(pbtnExpArchPend_ServerClick);

            InitRegistroPend();
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            pbtnAgregar.InnerText = Globals.GetMsgWeb("btnNueva");
            pbtnExpArchPend.InnerText = Globals.GetMsgWeb("btnExpPend");

            piCodMaestroPend.Descripcion = Globals.GetMsgWeb(false, "Maestro");
            pExpGridPend.Title = Globals.GetMsgWeb("CargasPendTitle");
            pExpGridPend.ToolTip = Globals.GetMsgWeb("CargasPendTitle");
            pPendGrid.Config.oLanguage = Globals.GetGridLanguage();
            pbtnPendFiltro.InnerText = Globals.GetMsgWeb("btnFiltro");
            pbtnLimpiaPendFiltro.InnerText = Globals.GetMsgWeb("btnLimpiarFiltro");
            pExpPendFiltros.Title = Globals.GetMsgWeb("HistoricFilterTitle");
            pExpPendFiltros.ToolTip = Globals.GetMsgWeb("HistoricFilterTitle");

            if (pFields != null)
            {
                // Cambia el nombre de columna Inicio Vigencia x "Fecha de Carga".
                CambiaHisGridLanguage();
            }


            if (pFieldsPend != null)
            {
                InitLanguageGrid(pFieldsPend, pPendGrid, phtPendFiltros, piCodMaestroPend.ToString());
            }

        }

        protected void CambiaHisGridLanguage()
        {
            DSOControlDB lFiltro;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                if (lCol.sName == "dtIniVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtCarga"));
                }
                if (phtFiltros.ContainsKey(lCol.sName))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lCol.sName];
                    lFiltro.Descripcion = lCol.sTitle;
                }
            }
        }
        public virtual void InitLanguageGrid(HistoricFieldCollection pFieldsGrid, DSOGrid pGridDP, Hashtable phtFiltrosDP)
        {
            InitLanguageGrid(pFieldsGrid, pGridDP, phtFiltrosDP, "");
        }
        public virtual void InitLanguageGrid(HistoricFieldCollection pFieldsGrid, DSOGrid pGridDP, Hashtable phtFiltrosDP, string liCodMaestroDeta)
        {
            pFieldsGrid.InitLanguage();
            KeytiaBaseField lField;
            DSOControlDB lFiltro;
            string lsClave;
            string vchDesMaestroGrid;

            if (liCodMaestroDeta == "")
            {
                vchDesMaestroGrid = "Clave.";
            }
            else
            {
                vchDesMaestroGrid = "Clave." + liCodMaestroDeta;
                vchDesMaestroGrid = vchDesMaestroGrid.Replace("Pendiente", "");
                vchDesMaestroGrid = vchDesMaestroGrid.Replace("Detalle ", "");
            }

            foreach (DSOGridClientColumn lCol in pGridDP.Config.aoColumnDefs)
            {
                if (pFieldsGrid.Contains(lCol.sName))
                {
                    lField = pFieldsGrid[lCol.sName];
                    if (lField.ConfigName == "Clave.")
                    {
                        lsClave = Globals.GetMsgWeb(false, vchDesMaestroGrid);
                        if (lsClave.StartsWith("#undefined"))
                        {
                            lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                        }
                        else
                        {
                            lCol.sTitle = DSOControl.JScriptEncode(lsClave);
                        }
                    }
                    else
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                    }

                }
                else if (lCol.sName == "vchCodigo")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchCodigo"));
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcionPend"));
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
                if (phtFiltrosDP.ContainsKey(lCol.sName))
                {
                    lFiltro = (DSOControlDB)phtFiltrosDP[lCol.sName];
                    lFiltro.Descripcion = lCol.sTitle;
                }
            }
        }

        protected virtual void InitRegistroPend()
        {
            pExpGridPend.ID = "GridPendWrapper";
            pExpGridPend.StartOpen = true;
            pExpGridPend.CreateControls();
            pExpGridPend.Panel.Controls.Clear();
            pExpGridPend.Panel.Controls.Add(pTablaPendientes);
            pExpGridPend.Panel.Controls.Add(pExpPendFiltros);
            pExpGridPend.OnOpen = "function(){" + pjsObj + ".fnInitGrids();}";

            pTablaPendientes.Controls.Clear();
            pTablaPendientes.ID = "TablaPendientes";
            pTablaPendientes.Width = Unit.Percentage(100);

            pPendGrid.ID = "PendGrid";
            pPendGrid.Wrapper = pExpGridPend;
            pPendGrid.CreateControls();

            int liRow = 1;

            piCodMaestroPend = new DSODropDownList();
            piCodMaestroPend.ID = "iCodMaestroPend";
            piCodMaestroPend.Table = pTablaPendientes;
            piCodMaestroPend.Row = liRow++;
            piCodMaestroPend.DataField = "iCodMaestroPend";
            piCodMaestroPend.SelectItemValue = "";
            piCodMaestroPend.ColumnSpan = 3;
            piCodMaestroPend.CreateControls();
            piCodMaestroPend.AutoPostBack = true;
            piCodMaestroPend.DropDownListChange += new EventHandler(piCodMaestroPend_SelectedIndexChanged);

            pExpPendFiltros.ID = "PendFiltrosWrapper";
            pExpPendFiltros.StartOpen = false;
            pExpPendFiltros.CreateControls();
            pExpPendFiltros.Panel.Controls.Clear();

            pExpPendFiltros.Panel.Controls.Add(pTblPendFiltros);
            pExpPendFiltros.Panel.Controls.Add(pbtnPendFiltro);
            pExpPendFiltros.Panel.Controls.Add(pbtnLimpiaPendFiltro);

            pbtnPendFiltro.ID = "btnPendFiltro";
            pbtnLimpiaPendFiltro.ID = "btnLimpiaPendFiltro";

            pbtnPendFiltro.Attributes["class"] = "buttonSearch";
            pbtnLimpiaPendFiltro.Attributes["class"] = "buttonCancel";

            pbtnPendFiltro.Style["display"] = "none";
            pbtnLimpiaPendFiltro.Style["display"] = "none";

            pbtnPendFiltro.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".filtrarPend();return false;";
            pbtnLimpiaPendFiltro.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".limpiarFiltroPend();return false;";

        }

        public override void ConsultarRegistro()
        {
            base.ConsultarRegistro();

            if (iCodRegistro != "null")
            {
                ConsultarDetalles();
            }
        }

        protected virtual void ConsultarDetalles()
        {
            IniMaestrosCargas();
            if (iCodMaestroPend != null)
            {
                InitGridPend();
                InitPendFiltros();
                ClearFiltros(phtPendFiltros);
            }
            else
            {
                pPendGrid.Grid.Visible = false;
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
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisData");

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

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtIniVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchCodigo";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchDescripcion";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            InitGridFields();

            lTarget = pHisGrid.Config.aoColumnDefs.Count;

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFinVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);

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

        protected virtual void InitGridPend()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pPendGrid.ClearConfig();
            pPendGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pPendGrid.Config.bAutoWidth = true;
            pPendGrid.Config.sScrollX = "100%";
            pPendGrid.Config.sScrollY = "400px";
            pPendGrid.Config.sPaginationType = "full_numbers";
            pPendGrid.Config.bJQueryUI = true;
            pPendGrid.Config.bProcessing = true;
            pPendGrid.Config.bServerSide = true;
            pPendGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerPendientes(this, sSource, aoData, fnCallback);}";
            pPendGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisDetallados");

            if (iCodMaestroPend != null)
            {
                pFieldsPend = new HistoricFieldCollection(int.Parse(iCodCarga), int.Parse(iCodMaestroPend));
            }
            else
            {
                pFieldsPend = null;
            }

            if (pFieldsPend != null)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "iCodRegistro";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "vchDescripcion";
                lCol.aTargets.Add(lTarget++);
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                foreach (KeytiaBaseField lField in pFieldsPend)
                {
                    if (lField.ShowInGrid)
                    {
                        lCol = new DSOGridClientColumn();
                        lCol.sName = lField.Column;
                        lCol.aTargets.Add(lTarget++);
                        pPendGrid.Config.aoColumnDefs.Add(lCol);
                    }
                }

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecha";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "120px";
                lCol.bVisible = false;
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecUltAct";
                lCol.aTargets.Add(lTarget++);
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                if (pPendGrid.Config.aoColumnDefs.Count > 10)
                {
                    pPendGrid.Config.sScrollXInner = (pPendGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
                }

                pPendGrid.Grid.Visible = true;
                pPendGrid.Fill();
            }
        }

        protected virtual void InitPendFiltros()
        {
            pTblPendFiltros.Controls.Clear();
            pTblPendFiltros.ID = "PendFiltros";
            pTblPendFiltros.Width = Unit.Percentage(100);

            DSOTextBox lDSOtxt;
            if (pFieldsPend != null)
            {
                phtPendFiltros = new Hashtable();

                foreach (KeytiaBaseField lField in pFieldsPend)
                {
                    if (lField.ShowInGrid)
                    {
                        lDSOtxt = new DSOTextBox();
                        lDSOtxt.ID = lField.Column;
                        lDSOtxt.AddClientEvent("dataFilterPend", lField.Column);
                        lDSOtxt.Row = lField.Row;
                        lDSOtxt.ColumnSpan = lField.ColumnSpan;
                        lDSOtxt.Table = pTblPendFiltros;
                        lDSOtxt.CreateControls();

                        phtPendFiltros.Add(lDSOtxt.ID, lDSOtxt);
                    }
                }
            }
        }

        protected virtual void ClearFiltros(Hashtable phtFiltrosDP)
        {
            foreach (DSOControlDB lFiltro in phtFiltrosDP.Values)
            {
                lFiltro.DataValue = DBNull.Value;
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            pbtnEditar.Visible = false;

            StringBuilder lsb = new StringBuilder();
            pPendGrid.Visible = false;
            pExpPendFiltros.Visible = false;
            pbtnExpArchPend.Visible = false;
            if (s == HistoricState.Consulta)
            {
                pPendGrid.Visible = true;
                pExpPendFiltros.Visible = true;
                pbtnExpArchPend.Visible = true;
                if (!CargaActiva())
                {
                    pbtnBaja.Visible = false;
                }
            }
            else if (s == HistoricState.MaestroSeleccionado)
            {
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".refreshGrid = 10000;");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "refreshGrid", lsb.ToString(), true, false);

            }
            //pdtIniVigencia.Visible = false;
            //pdtFinVigencia.Visible = false;
            ((TableRow)pdtFinVigencia.TcCtl.Parent).Style["display"] = "none";
            ((TableRow)pdtIniVigencia.TcCtl.Parent).Style["display"] = "none";

            State = s;
        }

        protected virtual void piCodMaestroPend_SelectedIndexChanged(object sender, EventArgs e)
        {
            iCodMaestroPend = piCodMaestroPend.HasValue ? piCodMaestroPend.DataValue.ToString() : null;

            if (iCodRegistro != null)
            {
                SetHistoricState(HistoricState.Consulta);
                if (iCodMaestroPend != null)
                {
                    pPendGrid.TxtState.Text = "";
                    InitGridPend();
                    InitPendFiltros();
                    ClearFiltros(phtPendFiltros);
                }
            }
            else
            {
                SetHistoricState(HistoricState.Inicio);
            }
        }

        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            IniCarga();
            BloqueaCampos();
        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            BloqueaCampos();
        }

        protected override void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            SetHistoricState(HistoricState.Baja);
            pFields.DisableFields();
            //            pdtFinVigencia.DataValue = DateTime.Today;

            FirePostBajaClick();
            ActualizaCarga();
        }

        protected override void GrabarRegistro()
        {
            if (State == HistoricState.Baja)
            {
                if (!ValidaCargaEmpleRecurs()) 
                { return; } //No realiza ninguna actualización de registro.

                base.GrabarRegistro(); //Esta Linea, entre otras cosas pone el estatus de CarEsperaElimina
                BorrarRegDetallados();
            }
            else
            {
                base.GrabarRegistro();
            }
        }

        protected virtual void IniMaestrosCargas()
        {
            if (iCodRegistro != "null")
            {
                InitAccionesSecundarias();

                iCodCarga = iCodCatalogo;

                psbQuery.Length = 0;
                psbQuery.AppendLine("Select Top 1 convert(varchar(20),iCodRegistro) from Maestros ");
                psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Pendientes ");
                psbQuery.AppendLine("                          Where iCodCatalogo = " + iCodCarga.ToString() + ") ");
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
                psbQuery.AppendLine("Order by vchDescripcion");

                iCodMaestroPend = (string)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                psbQuery.Length = 0;
                psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
                psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Pendientes ");
                psbQuery.AppendLine("                       Where iCodCatalogo = " + iCodCarga.ToString() + ") ");
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
                psbQuery.AppendLine("Order by vchDescripcion ");

                piCodMaestroPend.DataSource = psbQuery.ToString();
                piCodMaestroPend.Fill();
            }
        }

        protected virtual void IniCarga()
        {
            int liCount = 0;
            KDBAccess lKDB = new KDBAccess();

            // Verificar si existen registros para el maestro
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select isnull(count(iCodRegistro),0) from Historicos ");
            psbQuery.AppendLine("where iCodMaestro = " + iCodMaestro);

            liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());
            if (liCount > 0)
            {
                // Obten el ultimo registros para el maestro
                psbQuery.Length = 0;
                psbQuery.AppendLine("Select Max(iCodRegistro) from Historicos ");
                psbQuery.AppendLine("where iCodMaestro = " + iCodMaestro);

                iCodRegistro = DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString();

                // Consulta la informacion para inicializar los controles
                psbQuery.Length = 0;
                psbQuery.AppendLine("select H.*,C.vchCodigo from Historicos H, Catalogos C");
                psbQuery.AppendLine("where H.iCodRegistro = " + iCodRegistro);
                psbQuery.AppendLine("and C.iCodRegistro = H.iCodCatalogo");

                DataRow lDataRow = DSODataAccess.ExecuteDataRow(psbQuery.ToString());
                iCodCatalogo = lDataRow["iCodCatalogo"].ToString();
                pFields.SetValues(lDataRow);
                LimpiaCamposUltimaCarga();
            }

            // Inicializa el Estatus de la carga
            DataTable lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarEspera'");

            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                int liCodEstatus = (int)lKDBTable.Rows[0]["iCodCatalogo"];

                if (pFields.ContainsConfigName("EstCarga"))
                {
                    pFields.GetByConfigName("EstCarga").DataValue = liCodEstatus;
                }
            }
            // Inicializa la Fecha de incio
            pdtIniVigencia.DataValue = DateTime.Today;

            iCodRegistro = "null";
        }

        protected virtual void LimpiaCamposUltimaCarga()
        {
            if (pFields.ContainsConfigName("EstCarga"))
            {
                pFields.GetByConfigName("EstCarga").DataValue = DBNull.Value;
            }
            if (pFields.ContainsConfigName("Registros"))
            {
                pFields.GetByConfigName("Registros").DataValue = DBNull.Value;
            }
            if (pFields.ContainsConfigName("RegP"))
            {
                pFields.GetByConfigName("RegP").DataValue = DBNull.Value;
            }
            if (pFields.ContainsConfigName("RegD"))
            {
                pFields.GetByConfigName("RegD").DataValue = DBNull.Value;
            }
            if (pFields.ContainsConfigName("FechaInicio"))
            {
                pFields.GetByConfigName("FechaInicio").DataValue = DBNull.Value;
            }
            if (pFields.ContainsConfigName("FechaFin"))
            {
                pFields.GetByConfigName("FechaFin").DataValue = DBNull.Value;
            }
            if (pFields.ContainsConfigName("IniTasacion"))
            {
                pFields.GetByConfigName("IniTasacion").DataValue = DBNull.Value;
            }
            if (pFields.ContainsConfigName("FinTasacion"))
            {
                pFields.GetByConfigName("FinTasacion").DataValue = DBNull.Value;
            }

            // Si es un campo de UploadField se limpia
            if (pFields != null)
            {
                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField is KeytiaUploadField)
                    {
                        lField.DataValue = DBNull.Value;
                    }
                }
            }

            //if (pFields.ContainsConfigName("Archivo01"))
            //{
            //    pFields.GetByConfigName("Archivo01").DataValue = DBNull.Value;
            //}
            //if (pFields.ContainsConfigName("Archivo02"))
            //{
            //    pFields.GetByConfigName("Archivo02").DataValue = DBNull.Value;
            //}
            //if (pFields.ContainsConfigName("Archivo03"))
            //{
            //    pFields.GetByConfigName("Archivo03").DataValue = DBNull.Value;
            //}
            //if (pFields.ContainsConfigName("Archivo04"))
            //{
            //    pFields.GetByConfigName("Archivo04").DataValue = DBNull.Value;
            //}
            //if (pFields.ContainsConfigName("Archivo05"))
            //{
            //    pFields.GetByConfigName("Archivo05").DataValue = DBNull.Value;
            //}
            if (pFields.ContainsConfigName("OpcCreaUsuar"))
            {
                pFields.GetByConfigName("OpcCreaUsuar").DataValue = "0";
            }
            if (pFields.ContainsConfigName("TipoCenCost"))
            {
                (pFields.GetByConfigName("TipoCenCost")).DataValue = "0";
            }

        }

        protected virtual void BorrarRegDetallados()
        {
            try
            {
                KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");

                int liCodUsuario = (int)Session["iCodUsuarioDB"];
                pCargaCom.BajaCarga(int.Parse(iCodRegistro), liCodUsuario);
                Marshal.ReleaseComObject(pCargaCom);
                pCargaCom = null;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrDeleteRecord", e);
            }
        }

        protected virtual void BloqueaCampos()
        {
            if (pFields.ContainsConfigName("EstCarga"))
            {
                pFields.GetByConfigName("EstCarga").DisableField();
            }
            if (pFields.ContainsConfigName("Registros"))
            {
                pFields.GetByConfigName("Registros").DisableField();
            }
            if (pFields.ContainsConfigName("RegP"))
            {
                pFields.GetByConfigName("RegP").DisableField();
            }
            if (pFields.ContainsConfigName("RegD"))
            {
                pFields.GetByConfigName("RegD").DisableField();
            }

        }

        protected virtual void ActualizaCarga()
        {
            try
            {
                //Obtener el codigo de estatus de cargas eliminada
                DataTable lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarEsperaElimina'");
                int liCodRegistro = (int)lKDBTable.Rows[0]["iCodCatalogo"];

                if (pFields.ContainsConfigName("EstCarga"))
                {
                    pFields.GetByConfigName("EstCarga").DataValue = liCodRegistro;
                    //pdtFinVigencia.DataValue = pdtIniVigencia.Date;
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected virtual bool ValidarCamposConexion()
        {
            bool lbRet = true;
            string lsError = "";
            KeytiaBaseField lField;

            //Validar el String de conexion no este vacio  si la carga contiene el campo
            if (pFields.ContainsConfigName("ExtConnStr") &&
                pFields.GetByConfigName("ExtConnStr").DataValue == "null")
            {
                lField = pFields.GetByConfigName("ExtConnStr");
                lsError = Globals.GetMsgWeb("CampoRequerido", lField.Descripcion);
                psbErrores.Append("<li>" + lsError + "</li>");
                lbRet = false;
            }

            //Validar el Data Source no este vacio  si la carga contiene el campo
            if (pFields.ContainsConfigName("DBDataSource") &&
                pFields.GetByConfigName("DBDataSource").DataValue == "null")
            {
                lField = pFields.GetByConfigName("DBDataSource");
                lsError = Globals.GetMsgWeb("CampoRequerido", lField.Descripcion);
                psbErrores.Append("<li>" + lsError + "</li>");
                lbRet = false;
            }

            return lbRet;

        }

        protected virtual bool CargaActiva()
        {
            bool lbRet = true;

            //Estatus de cargas 
            int liCodCargaInicializada = 0;
            int liCodCargaEnEspera = 0;
            int liCodCargaEnEsperaElim = 0;
            int liCodCargaEliminada = 0;
            int liCodEstatus = 0;
            DataTable lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarInicial'");
            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                liCodCargaInicializada = (int)lKDBTable.Rows[0]["iCodCatalogo"];
            }
            // Estatus de la carga en espera de servicio

            lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarEspera'");
            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                liCodCargaEnEspera = (int)lKDBTable.Rows[0]["iCodCatalogo"];
            }
            lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarEsperaElimina'");
            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                liCodCargaEnEsperaElim = (int)lKDBTable.Rows[0]["iCodCatalogo"];
            }

            lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarElimina'");
            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                liCodCargaEliminada = (int)lKDBTable.Rows[0]["iCodCatalogo"];
            }

            if (pFields.ContainsConfigName("EstCarga") &&
                pFields.GetByConfigName("EstCarga").DataValue != "null")
            {
                string lsEstatus = pFields.GetByConfigName("EstCarga").DataValue.ToString();
                liCodEstatus = int.Parse(lsEstatus);
            }

            if (liCodEstatus == liCodCargaEnEspera || liCodEstatus == liCodCargaInicializada
                || liCodEstatus == liCodCargaEnEsperaElim || liCodEstatus == liCodCargaEliminada)
            {
                lbRet = false;
            }
            return lbRet;

        }

        protected virtual bool GeneraArchivoCarga()
        {
            bool lbRet = true;
            string lsConexion;
            string lsConexionDes = "";
            String lsDataSource = "";
            string lsError = "";
            DataTable ldt;
            try
            {
                lsConexion = pKDB.GetQueryHis(pKDB.CamposHis("ExtConnStr", "Coneccion Externo"), new string[] { "iCodCatalogo", "{ConnStrExt}" }, "", "", "");

                ldt = DSODataAccess.Execute(lsConexion + " where iCodCatalogo = " + pFields.GetByConfigName("ExtConnStr").DataValue.ToString());

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    lsConexionDes = Util.Decrypt((string)Util.IsDBNull(ldt.Rows[0]["{ConnStrExt}"], ""));
                }
                else
                {
                    lbRet = false;
                    lsError = Globals.GetMsgWeb("ErrValDataSource");
                    psbErrores.Append("<li>" + lsError + "</li>");
                    return lbRet;
                }

                DSOTextBox ltxtContenido = (DSOTextBox)pFields.GetByConfigName("DBDataSource").DSOControlDB;

                lsDataSource = ltxtContenido.TextBox.Text;

                ldt = DSODataAccess.Execute(lsDataSource, lsConexionDes);

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    lbRet = ExportXLSCarga(ldt);
                }
                else
                {
                    lbRet = false;
                    lsError = Globals.GetMsgWeb("ErrValDataSource");
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
            }
            catch
            {
                lbRet = false;
                lsError = Globals.GetMsgWeb("ErrValDataSource");
                psbErrores.Append("<li>" + lsError + "</li>");
            }
            return lbRet;

        }

        protected override bool ValidarRegistro()
        {

            bool lbRet = true;
            psbErrores = new StringBuilder();

            //si el registro se esta eliminando entonces no es necesaria la validacion de campos obligatorios
            if (State == HistoricState.Baja)
            {
                return true;
            }

            lbRet = base.ValidarRegistro();

            return lbRet;

        }
        protected override bool ValidarAtribCatalogosVig()
        {
            return true;
        }

        protected virtual bool ExportXLSCarga(DataTable dtDataSource)
        {
            bool lbRet = true;
            string lsError = "";
            ExcelAccess lExcel = new ExcelAccess();

            try
            {
                int li = 0;
                string lsHoja0;

                object[,] loColumnas = new object[1, dtDataSource.Columns.Count];
                object[,] loData = lExcel.DataTableToArray(dtDataSource);

                foreach (DataColumn lCol in dtDataSource.Columns)
                {
                    loColumnas[0, li] = lCol.ColumnName;
                    li = li + 1;
                }

                lExcel.Abrir();

                lsHoja0 = lExcel.NombreHoja0();
                lExcel.SetNumberFormat(lsHoja0, 1, 1, dtDataSource.Rows.Count + 1, dtDataSource.Columns.Count, "@");

                lExcel.Actualizar(lsHoja0, 1, 1, loColumnas.GetUpperBound(0) + 1, loColumnas.GetUpperBound(1) + 1, loColumnas);
                lExcel.Actualizar(lsHoja0, 2, 1, loData.GetUpperBound(0) + 2, loData.GetUpperBound(1) + 1, loData);
                lExcel.FilePath = GetFileNameCarga(".xlsx");
                lExcel.SalvarComo();

            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                lbRet = false;
                //                throw new KeytiaWebException("ErrExportTo", e, ".xlsx");
                lsError = Globals.GetMsgWeb("ErrExportTo");
                psbErrores.Append("<li>" + lsError + "</li>");
            }
            finally
            {

                if (lExcel != null)
                {
                    lExcel.Cerrar(true);
                    lExcel.Dispose();
                    lExcel = null;
                }
            }
            return lbRet;

        }

        protected virtual string GetFileNameCarga(string lsExt)
        {
            string lsFolder = GetSaveFolder();
            string lsFileName = System.IO.Path.Combine(lsFolder, "CargaBD" + DateTime.Now.ToFileTime() + lsExt);
            System.IO.Directory.CreateDirectory(lsFolder);
            Session[psFileKey] = lsFileName;
            return lsFileName;
        }

        protected virtual void pbtnExpArchPend_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            if (iCodMaestroPend != null)
            {
                ExportXLSDetllados(1, int.Parse(iCodCarga), int.Parse(iCodMaestroPend));
            }
            FirePostConsultarClick();
        }

        protected virtual void ExportXLSDetllados(int liTipoDeta, int liCodCarga, int liCodMaestro)
        {
            ExcelAccess lExcel = new ExcelAccess();
            KDBAccess lKDB = new KDBAccess();
            DataTable ldtMaestros;
            string lsHoja0;
            string lsMaestro;
            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
                if (liTipoDeta == 0 || liTipoDeta == 3)
                {
                    psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Detallados ");
                }
                else
                {
                    psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Pendientes ");
                }

                psbQuery.AppendLine("                       Where iCodCatalogo = " + iCodCarga.ToString() + ") ");
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
                psbQuery.AppendLine("Order by vchDescripcion ");

                ldtMaestros = DSODataAccess.Execute(psbQuery.ToString());
                // Obtener los nombres de empleados que tiene el recurso

                lExcel.Abrir();
                lsHoja0 = lExcel.NombreHoja0();

                foreach (DataRow lRowMaestros in ldtMaestros.Rows)
                {
                    liCodMaestro = (int)lRowMaestros["iCodRegistro"];
                    lsMaestro = lRowMaestros["vchDescripcion"].ToString();

                    DataTable ldt = GetExportData(liTipoDeta, liCodCarga, liCodMaestro);
                    int li = 0;

                    object[,] loColumnas = new object[1, ldt.Columns.Count];
                    object[,] loData = lExcel.DataTableToArray(ldt);

                    foreach (DataColumn lCol in ldt.Columns)
                    {
                        loColumnas[0, li] = lCol.ColumnName;
                        li = li + 1;
                    }

                    lsMaestro = lsMaestro.Substring(0, Math.Min(31, lsMaestro.Length));
                    lExcel.Copiar(lsHoja0, lsMaestro);

                    lExcel.SetNumberFormat(lsMaestro, 1, 1, ldt.Rows.Count + 1, ldt.Columns.Count, "@");

                    lExcel.Actualizar(lsMaestro, 1, 1, loColumnas.GetUpperBound(0) + 1, loColumnas.GetUpperBound(1) + 1, loColumnas);
                    lExcel.Actualizar(lsMaestro, 2, 1, loData.GetUpperBound(0) + 2, loData.GetUpperBound(1) + 1, loData);

                }

                lExcel.Remover(lsHoja0);

                lExcel.FilePath = GetFileName(".xlsx");
                lExcel.SalvarComo();

                ExportarArchivo(".xlsx");

            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".xlsx");
            }
            finally
            {
                if (lExcel != null)
                {
                    lExcel.Cerrar(true);
                    lExcel.Dispose();
                    lExcel = null;
                }
            }
        }

        protected void ExportarArchivo(string lsExt)
        {
            string lsTitulo = HttpUtility.UrlEncode(Globals.GetMsgWeb(false, "TituloCargasWeb"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        protected string GetFileName(string lsExt)
        {
            string lsFileName = System.IO.Path.Combine(psTempPath, "carga." + psFileKey + ".temp" + lsExt);
            Session[psFileKey] = lsFileName;
            return lsFileName;
        }

        protected void AjustaColumna(int liRow)
        {
            DSOControlDB lField;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                if (pFields.Contains(lCol.sName))
                {
                    lField = pFields[lCol.sName].DSOControlDB;
                    if (lField.Row == liRow)
                    {
                        pFields[lCol.sName].DSOControlDB.TcCtl.ColumnSpan = 3;
                        pFields[lCol.sName].DSOControlDB.TcCtl.CssClass = "DSOTcCtl ColSpan3";
                    }
                }
            }
        }

        protected virtual void OcultaCampos()
        {

            if (pFields != null && pTablaAtributos.Rows.Count > 0)
            {

                if (pFields.ContainsConfigName("FechaInicio"))
                {
                    pFields.GetByConfigName("FechaInicio").DSOControlDB.TcLbl.Visible = false;
                    pFields.GetByConfigName("FechaInicio").DSOControlDB.TcCtl.Visible = false;
                    int liCodRow = pFields.GetByConfigName("FechaInicio").DSOControlDB.Row;
                    AjustaColumna(liCodRow);

                }
                if (pFields.ContainsConfigName("FechaFin"))
                {
                    pFields.GetByConfigName("FechaFin").DSOControlDB.TcLbl.Visible = false;
                    pFields.GetByConfigName("FechaFin").DSOControlDB.TcCtl.Visible = false;
                    int liCodRow = pFields.GetByConfigName("FechaFin").DSOControlDB.Row;
                    AjustaColumna(liCodRow);
                }

            }

        }

        protected DataTable GetExportData(int liTipoDeta, int liCodCarga, int liCodMaestro)
        {
            DataTable ldt;
            // liTipoDeta Tipo de Detalle a presentar (0-Detallados,1-Pendientes,2-Pendientes de Facturas) 

            String lsArchivo = "Detallados";
            String lsFuncion = ".GetHisDetallados";
            string lsClave = "Clave.";
            lsClave += DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + liCodMaestro);
            lsClave = lsClave.Replace("Detalle ", "");
            lsClave = lsClave.Replace("Pendiente", "");
            lsClave = Globals.GetMsgWeb(false, lsClave);

            if (liTipoDeta == 1 || liTipoDeta == 2)
            {
                lsArchivo = "Pendientes";
                lsFuncion = ".GetHisPendientes";
            }

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(liCodCarga, liCodMaestro);
                lFields.InitLanguage();
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
                DataTable ldtAtributos;
                string lsDesc;
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

                lsbSelectTop.AppendLine("       select  ");

                ////////////////////////////////////////////////////////////////////////////

                //lsbColumnas.AppendLine("iCodRegistro");

                //if (liTipoDeta == 3)
                //{
                //    lsbColumnas.AppendLine(",vchCodigo");

                //}
                //lsbColumnas.AppendLine(",vchDescripcion");

                if (liTipoDeta == 1 || liTipoDeta == 2)
                {
                    lsDesc = "\"" + Globals.GetMsgWeb(false, "vchDescripcionPend").Replace("\"", "\"\"") + "\"";
                    //lsbColumnas.AppendLine("vchDescripcion as " + Globals.GetMsgWeb(false, "vchDescripcionPend"));
                    lsbColumnas.AppendLine("	" + lsDesc + "= vchDescripcion");
                }
                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField.ConfigName == "iNumCatalogo")
                    {
                        continue;
                    }
                    lsDesc = "\"" + lField.Descripcion.Replace("\"", "\"\"") + "\"";
                    if (lField.ConfigName == "Clave." && !lsClave.StartsWith("#undefined"))
                    {
                        lsDesc = "\"" + lsClave + "\"";
                    }
                    if (lField.ConfigName == "CenCos")
                    {
                        lsClave = Globals.GetMsgWeb(false, "CenCosPadre");
                        if (lsClave.StartsWith("#undefined"))
                        {
                            lsDesc = "\"" + lField.Descripcion.Replace("\"", "\"\"") + "\"";
                        }
                        lsDesc = "\"" + lsClave + "\"";
                    }
                    if (lsbColumnas.Length > 0)
                    {
                        //lsbColumnas.AppendLine("," + lField.Column + " as '" + lField.Descripcion + "'");
                        lsbColumnas.AppendLine("   ," + lsDesc + " = " + lField.Column);
                    }
                    else
                    {
                        //lsbColumnas.AppendLine(lField.Column + " as '" + lField.Descripcion + "'");
                        lsbColumnas.AppendLine("   " + lsDesc + " = " + lField.Column);
                    }
                }

                //lsbColumnas.AppendLine(",dtFecha");

                //lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from " + lsArchivo + " a");
                lsbFrom.AppendLine("      where iCodMaestro = " + liCodMaestro);
                lsbFrom.AppendLine("      and iCodCatalogo = " + liCodCarga + " ");

                lsbOrderBy.AppendLine("       order by iCodRegistro");

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");


                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsFromGetHisData = "from " + DSODataContext.Schema + lsFuncion + "(" + liCodCarga + "," + liCodMaestro + "," + liCodIdioma + ") a \r\n";
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();


                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetHisData + lsWhere + lsOrderBy);

                if (ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                return ldt;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }

        }

        protected bool ValidaCargaEmpleRecurs()
        {
            bool respuesta = false;
            if (vchDesMaestro.ToLower() == "cargas empleado")
            {
                StringBuilder lsbErrores = new StringBuilder();
                lsbErrores.AppendLine("SELECT COUNT(iCodRegistro) FROM " + DSODataContext.Schema + ".[VisHistoricos('Cargas','Cargas Empleado','Español')]");
                lsbErrores.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                lsbErrores.AppendLine("     AND iCodCatalogo = " + iCodCatalogo);
                lsbErrores.AppendLine("     AND Clase LIKE '%CargasEmpleRecurs.CargaMasivaEmpleRecurs'");
                lsbErrores.AppendLine("     AND (");
                lsbErrores.AppendLine("             EstCargaCod = 'ErrGenerandoBackup'");
                lsbErrores.AppendLine("			    OR iCodCatalogo NOT IN(SELECT DISTINCT iCodCatCarga FROM " + DSODataContext.Schema + ".HistoricosBackupCargas)");
                lsbErrores.AppendLine("		    )");

                int count = Convert.ToInt32(DSODataAccess.ExecuteScalar(lsbErrores.ToString()));
                if (count > 0)
                {
                    lsbErrores.Length = 0;
                    string error = string.Empty;
                    error = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValEliminarCarEmpleRecurs"));
                    lsbErrores.Append("<li>" + error + "</li>");

                    error = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", error, "Eliminar Carga");
                    respuesta = false;
                }
                else { respuesta = true; }
            }
            else { respuesta = true; }

            return respuesta;
        }

        #region WebMethods

        public static DSOGridServerResponse GetHisDetallados(DSOGridServerRequest gsRequest, int liTipoDeta, int liCodCarga, int liCodMaestro)
        {
            // liTipoDeta Tipo de Detalle a presentar (0-Detallados,1-Pendientes,2-Pendientes de Facturas) 

            String lsArchivo = "Detallados";
            String lsFuncion = ".GetHisDetallados";
            String lsBoton = "";

            if (liTipoDeta == 1 || liTipoDeta == 2)
            {
                lsArchivo = "Pendientes";
                lsFuncion = ".GetHisPendientes";
            }

            if (liTipoDeta == 2)
            {
                lsBoton = ",vchBoton";
            }


            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(liCodCarga, liCodMaestro);
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
                        && lsOrderCol != "vchDescripcion")
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

                if (lsBoton != "")
                {
                    lsbSelectTop.AppendLine("select ");
                    lsbSelectTop.AppendLine("vchBoton = " + DSODataContext.Schema + ".GetExiCatDesc(");
                    lsbSelectTop.AppendLine(liCodMaestro.ToString() + ",a.vchDescripcion),*  from (");
                }
                else
                {
                    lsbSelectTop.AppendLine("select * from (");
                }
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                if (lsBoton != "")
                {
                    lgsrRet.sColumns += lsBoton;
                }

                lgsrRet.sColumns = "iCodRegistro";
                lsbColumnas.AppendLine("iCodRegistro");

                if (liTipoDeta == 3)
                {
                    lgsrRet.sColumns += ",vchCodigo";
                    lsbColumnas.AppendLine(",vchCodigo");

                }
                lgsrRet.sColumns += ",vchDescripcion";
                lsbColumnas.AppendLine(",vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    lgsrRet.sColumns += "," + lField.Column;
                    lsbColumnas.AppendLine("," + lField.Column);
                }
                lgsrRet.sColumns += ",dtFecha";
                lsbColumnas.AppendLine(",dtFecha");

                lgsrRet.sColumns += ",dtFecUltAct";
                lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from " + lsArchivo + " a");
                lsbFrom.AppendLine("      where iCodMaestro = " + liCodMaestro);
                lsbFrom.AppendLine("      and iCodCatalogo = " + liCodCarga + " ");

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

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
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlDateFormat + "),'')");
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlTimeFormat + "),'')");
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

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsFromGetHisData = "from " + DSODataContext.Schema + lsFuncion + "(" + liCodCarga + "," + liCodMaestro + "," + liCodIdioma + ") a \r\n";
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom);
                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFromGetHisData + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetHisData + lsWhere + lsOrderBy);

                //elimino las filas repetidas de la ultima pagina
                int idxDelRows = 0;
                if (gsRequest.iDisplayStart != 0 && gsRequest.iDisplayStart + gsRequest.iDisplayLength > lgsrRet.iTotalDisplayRecords)
                {
                    idxDelRows = gsRequest.iDisplayStart + gsRequest.iDisplayLength - lgsrRet.iTotalDisplayRecords;
                }
                while (idxDelRows-- > 0)
                {
                    ldt.Rows.Remove(ldt.Rows[0]);
                }

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                string lsDateTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateTimeFormat");
                Dictionary<string, string> lColStringFormat = new Dictionary<string, string>();
                Dictionary<string, IFormatProvider> lColFormatter = new Dictionary<string, IFormatProvider>();

                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField is KeytiaDateField)
                    {
                        lColStringFormat.Add(lField.Column, lsDateFormat);
                    }
                    else if (lField is KeytiaDateTimeField)
                    {
                        lColStringFormat.Add(lField.Column, lsDateTimeFormat);
                    }
                    else if (lField is KeytiaNumericField)
                    {
                        lColStringFormat.Add(lField.Column, ((KeytiaNumericField)lField).StringFormat);
                        lColFormatter.Add(lField.Column, ((KeytiaNumericField)lField).FormatInfo);
                    }
                }

                lgsrRet.SetDataFromDataTable(ldt, lColStringFormat, lColFormatter);
                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetHisPendientes(DSOGridServerRequest gsRequest, int iCodCarga, int iCodMaestroPend)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodCarga, iCodMaestroPend);
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

                    if (!lFields.Contains(lsOrderCol))
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

                lgsrRet.sColumns = "iCodRegistro";
                lsbColumnas.AppendLine("iCodRegistro");


                foreach (KeytiaBaseField lField in lFields)
                {
                    lgsrRet.sColumns += "," + lField.Column;
                    lsbColumnas.AppendLine("," + lField.Column);
                }
                lgsrRet.sColumns += ",dtFecha";
                lsbColumnas.AppendLine(",dtFecha");

                lgsrRet.sColumns += ",dtFecUltAct";
                lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from Pendientes a");
                lsbFrom.AppendLine("      where iCodMaestro = " + iCodMaestroPend);
                lsbFrom.AppendLine("      and iCodCatalogo = " + iCodCarga + " ");

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

                bool lbPrimero = true;

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();

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
                            && (lFields.Contains(lsColumn)))
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

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsFromGetHisData = "from " + DSODataContext.Schema + ".GetHisPendientes(" + iCodCarga + "," + iCodMaestroPend + "," + liCodIdioma + ") a \r\n";
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom);
                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFromGetHisData + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetHisData + lsWhere + lsOrderBy);

                //elimino las filas repetidas de la ultima pagina
                int idxDelRows = 0;
                if (gsRequest.iDisplayStart != 0 && gsRequest.iDisplayStart + gsRequest.iDisplayLength > lgsrRet.iTotalDisplayRecords)
                {
                    idxDelRows = gsRequest.iDisplayStart + gsRequest.iDisplayLength - lgsrRet.iTotalDisplayRecords;
                }
                while (idxDelRows-- > 0)
                {
                    ldt.Rows.Remove(ldt.Rows[0]);
                }

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                string lsDateTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateTimeFormat");
                Dictionary<string, string> ldateColumnsFormat = new Dictionary<string, string>();

                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField is KeytiaDateField)
                    {
                        ldateColumnsFormat.Add(lField.Column, lsDateFormat);
                    }
                    else if (lField is KeytiaDateTimeField)
                    {
                        ldateColumnsFormat.Add(lField.Column, lsDateTimeFormat);
                    }
                }

                lgsrRet.SetDataFromDataTable(ldt, ldateColumnsFormat);
                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWeb");
                throw new KeytiaWebException(true, "ErrGridPend", e, lsTitulo);
            }
        }

        #endregion
    }
}
