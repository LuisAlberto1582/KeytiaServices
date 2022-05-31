using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Reflection;
using System.Web.Services;
using System.Collections.Generic;

namespace KeytiaWeb.UserInterface
{
    [Serializable]
    public class HistoricState
    {
        //Puesto que los enum no se pueden heredar se cambio la enumeracion de estados por una clase
        //con propiedades estaticas de tal forma que se pueda heredar para agregar más estados.
        public static readonly HistoricState Inicio = new HistoricState(1, "Inicio");
        public static readonly HistoricState MaestroSeleccionado = new HistoricState(2, "MaestroSeleccionado");
        public static readonly HistoricState Consulta = new HistoricState(3, "Consulta");
        public static readonly HistoricState Edicion = new HistoricState(4, "Edicion");
        public static readonly HistoricState Baja = new HistoricState(5, "Baja");
        public static readonly HistoricState SubHistorico = new HistoricState(6, "SubHistorico");
        public static readonly HistoricState SubHistoricoRel = new HistoricState(7, "SubHistoricoRel");
        public static readonly HistoricState CnfgSubHistoricField = new HistoricState(8, "CnfgSubHistoricField");

        protected int pIntValue;
        protected string pStringValue;

        public int IntValue
        {
            get
            {
                return pIntValue;
            }
        }

        protected HistoricState() { }

        protected HistoricState(int lIntValue, string lStringValue)
        {
            this.pIntValue = lIntValue;
            this.pStringValue = lStringValue;
        }

        protected static HistoricState NewState(int lIntValue, string lStringValue)
        {
            return new HistoricState(lIntValue, lStringValue);
        }

        public override string ToString()
        {
            return pStringValue;
        }

        public static bool operator ==(HistoricState a, HistoricState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.IntValue == b.IntValue && a.ToString() == b.ToString();
        }

        public static bool operator !=(HistoricState a, HistoricState b)
        {
            return !(a == b);
        }
    }

    public class HistoricEdit : Panel, INamingContainer, IPostBackEventHandler
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected StringBuilder psbQuery = new StringBuilder();
        protected KDBAccess pKDB = new KDBAccess();

        protected bool pbEnableEntidad = true;
        protected bool pbEnableMaestro = true;

        protected Panel pToolBar;
        protected HtmlButton pbtnConsultar;
        protected HtmlButton pbtnAgregar;
        protected HtmlButton pbtnEditar;
        protected HtmlButton pbtnGrabar;
        protected HtmlButton pbtnCancelar;
        protected HtmlButton pbtnRegresar;
        protected HtmlButton pbtnBaja;

        protected DSOExpandable pExpRegistro;
        protected Table pTablaRegistro;
        protected DSODropDownList piCodEntidad;
        protected DSODropDownList piCodMaestro;
        protected DSOTextBox pvchCodigo;
        protected DSOTextBox pvchDescripcion;
        protected DSODateTimeBox pdtIniVigencia;
        protected DSODateTimeBox pdtFinVigencia;
        protected DSOCheckBox pbReplicarClientes;
        protected DSOGrid pHisGrid;
        protected DSOExpandable pExpFiltros;
        protected Table pTablaFiltros;
        protected HtmlButton pbtnFiltro;
        protected HtmlButton pbtnLimpiaFiltro;
        protected Hashtable phtFiltros;

        protected Panel pPanelSubHistoricos;

        protected DSOExpandable pExpAtributos;
        protected Table pTablaAtributos;
        protected HistoricFieldCollection pFields;
        protected Label plblTitle;

        protected Hashtable phtValues;
        protected DataSet pdsRelValues;

        protected string pjsObj;

        protected EventHandler pPostMaestroSelectedIndexChanged;
        protected EventHandler pPostConsultarClick;
        protected EventHandler pPostAgregarClick;
        protected EventHandler pPostEditarClick;
        protected EventHandler pPostGrabarClick;
        protected EventHandler pPostCancelarClick;
        protected EventHandler pPostRegresarClick;
        protected EventHandler pPostBajaClick;

        protected bool pbControlsCreated = false;

        protected HistoricEdit pSubHistorico;
        protected HistoricEdit pHistorico = null;

        public HistoricEdit()
        {
            Init += new EventHandler(HistoricEdit_Init);
            Load += new EventHandler(HistoricEdit_Load);
        }

        public Panel PanelSubHistoricos
        {
            get
            {
                return pPanelSubHistoricos;
            }
        }

        public string OpcMnu
        {
            get
            {
                return (string)ViewState["OpcMnu"];
            }
            set
            {
                ViewState["OpcMnu"] = value;
            }
        }

        public int iCodAplicacion
        {
            get
            {
                return (int)ViewState["iCodAplicacion"];
            }
            set
            {
                ViewState["iCodAplicacion"] = value;
            }
        }

        public string CollectionClass
        {
            get
            {
                if (ViewState["CollectionClass"] == null)
                {
                    ViewState["CollectionClass"] = "KeytiaWeb.UserInterface.HistoricFieldCollection";
                }
                return ViewState["CollectionClass"].ToString();
            }
            set
            {
                ViewState["CollectionClass"] = value;
            }
        }

        public virtual string vchCodEntidad
        {
            get
            {
                return (string)ViewState["vchCodEntidad"];
            }
            protected set
            {
                ViewState["vchCodEntidad"] = value;
            }
        }

        public virtual string iCodEntidad
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
                else
                {
                    vchCodEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodRegistro = " + value).ToString();
                }
            }
        }

        public virtual string vchDesMaestro
        {
            get
            {
                return (string)ViewState["vchDesMaestro"];
            }
            protected set
            {
                ViewState["vchDesMaestro"] = value;
            }
        }

        public virtual string iCodMaestro
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
                else
                {
                    //Cuando se inicialice iCodMaestro que tambien se inicialice vchDesMaestro
                    vchDesMaestro = DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + value).ToString();
                }
            }
        }

        public virtual string iCodRegistro
        {
            get
            {
                if (ViewState["iCodRegistro"] == null)
                {
                    ViewState["iCodRegistro"] = "null";
                }
                return (string)ViewState["iCodRegistro"];
            }
            set
            {
                ViewState["iCodRegistro"] = value;
                if (value == null || value == "null")
                {
                    iCodCatalogo = "null";
                }
            }
        }

        public virtual string iCodCatalogo
        {
            get
            {
                if (ViewState["iCodCatalogo"] == null)
                {
                    ViewState["iCodCatalogo"] = "null";
                }
                return (string)ViewState["iCodCatalogo"];
            }
            protected set
            {
                ViewState["iCodCatalogo"] = value;
            }
        }

        public virtual HistoricState State
        {
            get
            {
                if (ViewState["HistoricState"] == null)
                {
                    ViewState["HistoricState"] = HistoricState.Inicio;
                }
                return (HistoricState)ViewState["HistoricState"];
            }
            protected set
            {
                ViewState["HistoricState"] = value;
            }
        }

        public virtual HistoricState PrevState
        {
            get
            {
                if (ViewState["HistoricPrevState"] == null)
                {
                    ViewState["HistoricPrevState"] = HistoricState.Inicio;
                }
                return (HistoricState)ViewState["HistoricPrevState"];
            }
            set
            {
                ViewState["HistoricPrevState"] = value;
            }
        }

        public string Title
        {
            get
            {
                return (string)ViewState["Title"];
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        public string AlertTitle
        {
            get
            {
                if (ViewState["AlertTitle"] == null)
                {
                    return Title;
                }
                else
                {
                    return (string)ViewState["AlertTitle"];
                }
            }
            set
            {
                ViewState["AlertTitle"] = value;
            }
        }

        public string vchCodTitle
        {
            get
            {
                return (string)ViewState["vchCodTitle"];
            }
            set
            {
                ViewState["vchCodTitle"] = value;
            }
        }

        public Label lblTitle
        {
            get
            {
                return plblTitle;
            }
            set
            {
                plblTitle = value;
            }
        }

        public DSOTextBox vchCodigo
        {
            get
            {
                return pvchCodigo;
            }
        }

        public DSOTextBox vchDescripcion
        {
            get
            {
                return pvchDescripcion;
            }
        }

        public DSODateTimeBox dtIniVigencia
        {
            get
            {
                return pdtIniVigencia;
            }
        }

        public DSODateTimeBox dtFinVigencia
        {
            get
            {
                return pdtFinVigencia;
            }
        }

        public HistoricFieldCollection Fields
        {
            get
            {
                return pFields;
            }
        }

        public virtual void CleanEntidad()
        {
            iCodEntidad = null;
            iCodMaestro = null;
            piCodEntidad.DataValue = DBNull.Value;
            piCodMaestro.DataValue = DBNull.Value;
        }

        public virtual void CleanMaestro()
        {
            iCodMaestro = null;
            piCodMaestro.DataValue = DBNull.Value;
        }

        public virtual void SetEntidad(string lsEntidad)
        {
            vchCodEntidad = lsEntidad;
            iCodEntidad = DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where dtIniVigencia <> dtFinVigencia and iCodCatalogo is null and vchCodigo = '" + lsEntidad.Replace("'", "''") + "'").ToString();
            pbEnableEntidad = false;
        }

        public virtual void SetMaestro(string lsMaestro)
        {
            if (!pbEnableEntidad)
            {
                vchDesMaestro = lsMaestro;
                iCodMaestro = DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where dtIniVigencia <> dtFinVigencia and iCodEntidad = " + iCodEntidad + " and vchDescripcion = '" + lsMaestro.Replace("'", "''") + "'").ToString();
                pbEnableMaestro = false;
            }
        }

        public virtual string GetMaestro(int liCodEntidad)
        {
            DataTable ldtMaestros = DSODataAccess.Execute("select * from Maestros where dtIniVigencia <> dtFinVigencia and iCodEntidad = " + liCodEntidad);
            if (ldtMaestros.Rows.Count == 1)
            {
                return ldtMaestros.Rows[0]["vchDescripcion"].ToString();
            }
            else
            {
                return null;
            }
        }

        public virtual void LoadScripts()
        {
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "HistoricEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/HistoricEdit.js?V=1") + "' type='text/javascript'></script>\r\n", true, false);
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "KeytiaFields.js", "<script src='" + ResolveClientUrl("~/UserInterface/Colecciones/Campos/KeytiaFields.js?V=2") + "' type='text/javascript'></script>\r\n", true, false);
        }

        #region PostEvents

        public event EventHandler PostMaestroSelectedIndexChanged
        {
            add
            {
                pPostMaestroSelectedIndexChanged += value;
            }
            remove
            {
                pPostMaestroSelectedIndexChanged -= value;
            }
        }

        public event EventHandler PostConsultarClick
        {
            add
            {
                pPostConsultarClick += value;
            }
            remove
            {
                pPostConsultarClick -= value;
            }
        }

        public event EventHandler PostAgregarClick
        {
            add
            {
                pPostAgregarClick += value;
            }
            remove
            {
                pPostAgregarClick -= value;
            }
        }

        public event EventHandler PostEditarClick
        {
            add
            {
                pPostEditarClick += value;
            }
            remove
            {
                pPostEditarClick -= value;
            }
        }

        public event EventHandler PostGrabarClick
        {
            add
            {
                pPostGrabarClick += value;
            }
            remove
            {
                pPostGrabarClick -= value;
            }
        }

        public event EventHandler PostCancelarClick
        {
            add
            {
                pPostCancelarClick += value;
            }
            remove
            {
                pPostCancelarClick -= value;
            }
        }

        public event EventHandler PostRegresarClick
        {
            add
            {
                pPostRegresarClick += value;
            }
            remove
            {
                pPostRegresarClick -= value;
            }
        }

        public event EventHandler PostBajaClick
        {
            add
            {
                pPostBajaClick += value;
            }
            remove
            {
                pPostBajaClick -= value;
            }
        }

        protected void FirePostMaestroSelectedIndexChanged()
        {
            if (pPostMaestroSelectedIndexChanged != null)
            {
                pPostMaestroSelectedIndexChanged(this, new EventArgs());
            }
        }

        protected void FirePostConsultarClick()
        {
            if (pPostConsultarClick != null)
            {
                pPostConsultarClick(this, new EventArgs());
            }
        }

        protected void FirePostAgregarClick()
        {
            if (pPostAgregarClick != null)
            {
                pPostAgregarClick(this, new EventArgs());
            }
        }

        protected void FirePostEditarClick()
        {
            if (pPostEditarClick != null)
            {
                pPostEditarClick(this, new EventArgs());
            }
        }

        protected void FirePostGrabarClick()
        {
            if (pPostGrabarClick != null)
            {
                pPostGrabarClick(this, new EventArgs());
            }
        }

        protected void FirePostCancelarClick()
        {
            if (pPostCancelarClick != null)
            {
                pPostCancelarClick(this, new EventArgs());
            }
        }

        protected void FirePostRegresarClick()
        {
            if (pPostRegresarClick != null)
            {
                pPostRegresarClick(this, new EventArgs());
            }
        }

        protected void FirePostBajaClick()
        {
            if (pPostBajaClick != null)
            {
                pPostBajaClick(this, new EventArgs());
            }
        }

        #endregion

        protected virtual void HistoricEdit_Init(object sender, EventArgs e)
        {
            pToolBar = new Panel();
            pbtnConsultar = new HtmlButton();
            pbtnAgregar = new HtmlButton();
            pbtnEditar = new HtmlButton();
            pbtnGrabar = new HtmlButton();
            pbtnCancelar = new HtmlButton();
            pbtnRegresar = new HtmlButton();
            pbtnBaja = new HtmlButton();

            pExpRegistro = new DSOExpandable();
            pTablaRegistro = new Table();
            piCodEntidad = new DSODropDownList();
            piCodMaestro = new DSODropDownList();
            pvchCodigo = new DSOTextBox();
            pvchDescripcion = new DSOTextBox();
            pdtIniVigencia = new DSODateTimeBox();
            pdtFinVigencia = new DSODateTimeBox();
            pbReplicarClientes = new DSOCheckBox();

            pHisGrid = new DSOGrid();
            pExpFiltros = new DSOExpandable();
            pTablaFiltros = new Table();
            pbtnFiltro = new HtmlButton();
            pbtnLimpiaFiltro = new HtmlButton();

            pExpAtributos = new DSOExpandable();
            pTablaAtributos = new Table();

            pPanelSubHistoricos = new Panel();

            this.CssClass = "HistoricEdit";
            Controls.Add(pToolBar);
            pToolBar.Controls.Add(pbtnConsultar);
            pToolBar.Controls.Add(pbtnAgregar);
            pToolBar.Controls.Add(pbtnEditar);
            pToolBar.Controls.Add(pbtnGrabar);
            pToolBar.Controls.Add(pbtnCancelar);
            pToolBar.Controls.Add(pbtnRegresar);
            pToolBar.Controls.Add(pbtnBaja);

            Controls.Add(pExpRegistro);
            Controls.Add(pExpFiltros);
            Controls.Add(pHisGrid);
            Controls.Add(pExpAtributos);
            Controls.Add(pPanelSubHistoricos);

            pjsObj = this.ID;
        }

        protected virtual void HistoricEdit_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || piCodEntidad.DataSource == null)
            {
                FillControls();
            }
        }

        public virtual void FillControls()
        {
            //Metodo para llenar los controles que solo es necesario llenar una vez
            CreateControls();

            piCodEntidad.DataSource = "select iCodRegistro, vchDescripcion from Catalogos where dtIniVigencia <> dtFinVigencia and iCodCatalogo is null order by vchDescripcion";
            piCodEntidad.Fill();

            if (!pbEnableEntidad)
            {
                piCodEntidad.DataValue = iCodEntidad;
            }
        }

        protected virtual void FillAjaxControls()
        {
            FillAjaxControls(true);
        }

        public virtual void FillAjaxControls(bool lbIncluirFechaFin)
        {
            //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
            piCodMaestro.DataSource = "select iCodRegistro, vchDescripcion from Maestros where dtIniVigencia <> dtFinVigencia and iCodEntidad = " + piCodEntidad.DataValue.ToString() + " order by vchDescripcion";
            piCodMaestro.Fill();

            if (!pbEnableMaestro)
            {
                piCodMaestro.DataValue = iCodMaestro;
                if (State == HistoricState.Inicio)
                {
                    SetHistoricState(HistoricState.MaestroSeleccionado);
                    InitMaestro();
                }
            }

            if (pFields != null)
            {
                IniciaVigencia(lbIncluirFechaFin);
                pFields.FillAjaxControls();
            }
        }

        public virtual void CreateControls()
        {
            try
            {
                if (pbControlsCreated)
                {
                    return;
                }
                InitAcciones();
                InitRegistro();
                CreateGrid();
                InitSubHistorico(SubHistoricoID);
                InitFields();
                InitFiltros();
                InitAccionesSecundarias();
                SetHistoricState(State);

                pbControlsCreated = true;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrInitControls", e);
            }
        }

        protected virtual void InitAcciones()
        {
            pToolBar.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";

            pbtnConsultar.ID = "btnConsultar";
            pbtnAgregar.ID = "btnAgregar";
            pbtnEditar.ID = "btnEditar";
            pbtnGrabar.ID = "btnGrabar";
            pbtnCancelar.ID = "btnCancelar";
            pbtnRegresar.ID = "btnRegresar";
            pbtnBaja.ID = "btnBaja";

            pbtnConsultar.Attributes["class"] = "buttonSearch";
            pbtnAgregar.Attributes["class"] = "buttonAdd";
            pbtnEditar.Attributes["class"] = "buttonEdit";
            pbtnGrabar.Attributes["class"] = "buttonSave";
            pbtnCancelar.Attributes["class"] = "buttonCancel";
            pbtnRegresar.Attributes["class"] = "buttonBack";
            pbtnBaja.Attributes["class"] = "buttonDelete";

            pbtnConsultar.Style["display"] = "none";
            pbtnAgregar.Style["display"] = "none";
            pbtnEditar.Style["display"] = "none";
            pbtnGrabar.Style["display"] = "none";
            pbtnCancelar.Style["display"] = "none";
            pbtnRegresar.Style["display"] = "none";
            pbtnBaja.Style["display"] = "none";

            pbtnConsultar.ServerClick += new EventHandler(pbtnConsultar_ServerClick);
            pbtnAgregar.ServerClick += new EventHandler(pbtnAgregar_ServerClick);
            pbtnEditar.ServerClick += new EventHandler(pbtnEditar_ServerClick);
            pbtnRegresar.ServerClick += new EventHandler(pbtnRegresar_ServerClick);
            pbtnBaja.ServerClick += new EventHandler(pbtnBaja_ServerClick);

            string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "btnGrabar");
            pbtnGrabar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = "$(this).button(\"disable\");$('#" + pbtnCancelar.ClientID + "').button(\"disable\");" + lsdoPostBack;

            lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "btnCancelar");
            pbtnCancelar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".cancel(function(){" + lsdoPostBack + "});return false;";
        }

        protected virtual void InitRegistro()
        {
            pExpRegistro.ID = "RegWrapper";
            pExpRegistro.StartOpen = true;
            pExpRegistro.CreateControls();
            pExpRegistro.Panel.Controls.Clear();
            pExpRegistro.Panel.Controls.Add(pTablaRegistro);

            pTablaRegistro.ID = "TablaRegistro";
            pTablaRegistro.Width = Unit.Percentage(100);

            int liRow = 1;
            piCodEntidad.ID = "iCodEntidad";
            piCodEntidad.Table = pTablaRegistro;
            piCodEntidad.Row = liRow++;
            piCodEntidad.ColumnSpan = 3;
            piCodEntidad.AddClientEvent(HtmlTextWriterAttribute.Onchange.ToString(), pjsObj + ".catChange($(this));");
            piCodEntidad.SelectItemText = " ";
            piCodEntidad.DataField = "iCodEntidad";
            piCodEntidad.CreateControls();

            piCodMaestro.ID = "iCodMaestro";
            piCodMaestro.Table = pTablaRegistro;
            piCodMaestro.Row = liRow++;
            piCodMaestro.ColumnSpan = 3;
            piCodMaestro.SelectItemText = " ";
            piCodMaestro.DataField = "iCodMaestro";
            piCodMaestro.CreateControls();
            //piCodMaestro.DropDownList.AutoPostBack = true;
            //piCodMaestro.DropDownList.SelectedIndexChanged += new EventHandler(piCodMaestro_SelectedIndexChanged);
            piCodMaestro.AutoPostBack = true;
            piCodMaestro.DropDownListChange += new EventHandler(piCodMaestro_SelectedIndexChanged);

            pvchCodigo.ID = "vchCodigo";
            pvchCodigo.Table = pTablaRegistro;
            pvchCodigo.Row = liRow;
            pvchCodigo.DataField = "vchCodigo";
            pvchCodigo.CreateControls();
            pvchCodigo.TextBox.MaxLength = 40;

            pvchDescripcion.ID = "vchDescripcion";
            pvchDescripcion.Table = pTablaRegistro;
            pvchDescripcion.Row = liRow++;
            pvchDescripcion.DataField = "vchDescripcion";
            pvchDescripcion.CreateControls();
            pvchDescripcion.TextBox.MaxLength = 160;

            pdtIniVigencia.ID = "dtIniVigencia";
            pdtIniVigencia.Row = liRow;
            pdtIniVigencia.Table = pTablaRegistro;
            pdtIniVigencia.DataField = "dtIniVigencia";
            pdtIniVigencia.ShowHour = false;
            pdtIniVigencia.ShowMinute = false;
            pdtIniVigencia.ShowSecond = false;
            pdtIniVigencia.CreateControls();

            pdtFinVigencia.ID = "dtFinVigencia";
            pdtFinVigencia.Row = liRow++;
            pdtFinVigencia.Table = pTablaRegistro;
            pdtFinVigencia.DataField = "dtFinVigencia";
            pdtFinVigencia.ShowHour = false;
            pdtFinVigencia.ShowMinute = false;
            pdtFinVigencia.ShowSecond = false;
            pdtFinVigencia.CreateControls();

            pbReplicarClientes.ID = "bReplicarClientes";
            pbReplicarClientes.Row = liRow++;
            pbReplicarClientes.ColumnSpan = 3;
            pbReplicarClientes.Table = pTablaRegistro;
            pbReplicarClientes.CreateControls();
        }

        protected virtual void CreateGrid()
        {
            pHisGrid.ID = "HisGrid";
            pHisGrid.CreateControls();
        }

        protected virtual void InitGrid()
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
            lCol.sName = "dtIniVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFinVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
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

        protected virtual void InitGridFields()
        {
            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = lField.Column;
                    lCol.aTargets.Add(lTarget++);
                    pHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected virtual void InitFields()
        {
            pExpAtributos.ID = "AtribWrapper";
            pExpAtributos.StartOpen = true;
            pExpAtributos.CreateControls();
            pExpAtributos.Panel.Controls.Clear();
            pExpAtributos.Panel.Controls.Add(pTablaAtributos);
            pExpAtributos.OnOpen = "function(){" + pjsObj + ".fnInitGrids();}";

            pTablaAtributos.Controls.Clear();
            pTablaAtributos.ID = "Atributos";
            pTablaAtributos.Width = Unit.Percentage(100);

            pPanelSubHistoricos.ID = "PanelSubHistoricos";
            pPanelSubHistoricos.CssClass = "PanelSubHistoricos";
            pPanelSubHistoricos.Controls.Clear();

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + " = new Historico('#" + this.ClientID + "','" + pjsObj + "');");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "New", lsb.ToString(), true, false);

            if (!String.IsNullOrEmpty(iCodEntidad) && !String.IsNullOrEmpty(iCodMaestro))
            {
                pFields = (HistoricFieldCollection)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricFieldCollection)).CodeBase, CollectionClass).Unwrap();
                pFields.InitCollection(this, int.Parse(iCodEntidad), int.Parse(iCodMaestro), pTablaAtributos, this.ValidarPermiso);
                pFields.InitFields();
                IniciaVigencia(false);
            }

        }

        protected virtual void InitFiltros()
        {
            pExpFiltros.ID = "FiltrosWrapper";
            pExpFiltros.StartOpen = false;
            pExpFiltros.CreateControls();
            pExpFiltros.Panel.Controls.Clear();

            pExpFiltros.Panel.Controls.Add(pTablaFiltros);
            pExpFiltros.Panel.Controls.Add(pbtnFiltro);
            pExpFiltros.Panel.Controls.Add(pbtnLimpiaFiltro);

            pbtnFiltro.ID = "btnFiltro";
            pbtnLimpiaFiltro.ID = "btnLimpiaFiltro";

            pbtnFiltro.Attributes["class"] = "buttonSearch";
            pbtnLimpiaFiltro.Attributes["class"] = "buttonCancel";

            pbtnFiltro.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".filtrar();return false;";
            pbtnLimpiaFiltro.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".limpiarFiltro();return false;";

            pTablaFiltros.Controls.Clear();
            pTablaFiltros.ID = "Filtros";
            pTablaFiltros.Width = Unit.Percentage(100);

            DSOTextBox lDSOtxt;
            if (pFields != null)
            {
                phtFiltros = new Hashtable();

                lDSOtxt = new DSOTextBox();
                lDSOtxt.ID = "vchCodigo";
                lDSOtxt.AddClientEvent("dataFilter", "vchCodigo");
                lDSOtxt.Row = 1;
                lDSOtxt.ColumnSpan = 3;
                lDSOtxt.Table = pTablaFiltros;
                lDSOtxt.CreateControls();
                phtFiltros.Add(lDSOtxt.ID, lDSOtxt);

                lDSOtxt = new DSOTextBox();
                lDSOtxt.ID = "vchDescripcion";
                lDSOtxt.AddClientEvent("dataFilter", "vchDescripcion");
                lDSOtxt.Row = 2;
                lDSOtxt.ColumnSpan = 3;
                lDSOtxt.Table = pTablaFiltros;
                lDSOtxt.CreateControls();
                phtFiltros.Add(lDSOtxt.ID, lDSOtxt);

                InitFiltrosFields();
            }
        }

        protected virtual void InitFiltrosFields()
        {
            DSOTextBox lDSOtxt;
            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lDSOtxt = new DSOTextBox();
                    lDSOtxt.ID = lField.Column;
                    lDSOtxt.AddClientEvent("dataFilter", lField.Column);
                    lDSOtxt.Row = lField.Row + 2;
                    lDSOtxt.ColumnSpan = lField.ColumnSpan;
                    lDSOtxt.Table = pTablaFiltros;
                    lDSOtxt.CreateControls();

                    phtFiltros.Add(lDSOtxt.ID, lDSOtxt);
                }
            }
        }

        protected void ClearFiltros()
        {
            foreach (DSOControlDB lFiltro in phtFiltros.Values)
            {
                lFiltro.DataValue = DBNull.Value;
            }
        }

        protected virtual void InitAccionesSecundarias()
        {
            if (State == HistoricState.CnfgSubHistoricField)
            {
                pSubHistorico.PostGrabarClick += new EventHandler(CnfgSubHistoricField_PostGrabarClick);
                pSubHistorico.PostCancelarClick += new EventHandler(CnfgSubHistorico_PostCancelarClick);
            }
            else if (State == HistoricState.SubHistorico)
            {
                pSubHistorico.PostGrabarClick += new EventHandler(SubHistorico_PostGrabarClick);
                pSubHistorico.PostCancelarClick += new EventHandler(SubHistorico_PostCancelarClick);
            }
        }

        public virtual void InitLanguage()
        {
            FillAjaxControls(false);

            plblTitle.Text = Title;

            pbtnConsultar.InnerText = Globals.GetMsgWeb(false, "btnConsultar");
            pbtnAgregar.InnerText = Globals.GetMsgWeb(false, "btnAgregar");
            pbtnEditar.InnerText = Globals.GetMsgWeb(false, "btnEditar");
            pbtnGrabar.InnerText = Globals.GetMsgWeb(false, "btnGrabar");
            pbtnCancelar.InnerText = Globals.GetMsgWeb(false, "btnCancelar");
            pbtnRegresar.InnerText = Globals.GetMsgWeb(false, "btnRegresar");
            pbtnBaja.InnerText = Globals.GetMsgWeb(false, "btnBaja");

            pbtnFiltro.InnerText = Globals.GetMsgWeb(false, "btnFiltro");
            pbtnLimpiaFiltro.InnerText = Globals.GetMsgWeb(false, "btnLimpiarFiltro");

            piCodEntidad.Descripcion = Globals.GetMsgWeb(false, "Entidad");
            piCodMaestro.Descripcion = Globals.GetMsgWeb(false, "Maestro");
            pdtIniVigencia.Descripcion = Globals.GetMsgWeb(false, "dtIniVigencia");
            pdtFinVigencia.Descripcion = Globals.GetMsgWeb(false, "dtFinVigencia");
            pdtIniVigencia.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateFormat"));
            pdtFinVigencia.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateFormat"));
            pvchCodigo.Descripcion = Globals.GetMsgWeb(false, "vchCodigo");
            pvchDescripcion.Descripcion = Globals.GetMsgWeb(false, "vchDescripcion");
            pbReplicarClientes.Descripcion = Globals.GetMsgWeb(false, "bReplicarClientes");

            pvchCodigo.RequiredMessage = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pvchCodigo.Descripcion));
            pvchDescripcion.RequiredMessage = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pvchDescripcion.Descripcion));

            pExpRegistro.Title = Globals.GetMsgWeb("HistoricRegTitle");
            pExpRegistro.ToolTip = Globals.GetMsgWeb("HistoricRegTitle");

            pExpFiltros.Title = Globals.GetMsgWeb("HistoricFilterTitle");
            pExpFiltros.ToolTip = Globals.GetMsgWeb("HistoricFilterTitle");

            pExpAtributos.Title = Globals.GetMsgWeb("HistoricDataTitle");
            pExpAtributos.ToolTip = Globals.GetMsgWeb("HistoricDataTitle");

            pHisGrid.Config.oLanguage = Globals.GetGridLanguage();


            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine("HisMsgs.confirm = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarCambios")) + "\";");
            lsb.AppendLine("HisMsgs.confirmTitle = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarTitulo")) + "\";");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "HisMsgs", lsb.ToString(), true, false);


            if (pFields != null)
            {
                pFields.InitLanguage();
                InitHisGridLanguage();
            }

            if (!String.IsNullOrEmpty(SubHistoricoID))
            {
                if (String.IsNullOrEmpty(pSubHistorico.vchCodTitle))
                {
                    pSubHistorico.Title = this.Title + " / " + Globals.GetLangItem("", "Entidades", pSubHistorico.vchCodEntidad);
                    pSubHistorico.AlertTitle = this.AlertTitle + " / " + Globals.GetLangItem("", "Entidades", pSubHistorico.vchCodEntidad);
                }
                else
                {
                    pSubHistorico.Title = this.Title + " / " + Globals.GetLangItem("MsgWeb", "Mensajes Web", pSubHistorico.vchCodTitle);
                    pSubHistorico.AlertTitle = this.AlertTitle + " / " + Globals.GetLangItem("MsgWeb", "Mensajes Web", pSubHistorico.vchCodTitle);
                }
                pSubHistorico.InitLanguage();
            }
        }

        protected virtual void InitHisGridLanguage()
        {
            KeytiaBaseField lField;
            DSOControlDB lFiltro;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                if (pFields.Contains(lCol.sName))
                {
                    lField = pFields[lCol.sName];
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
            }
        }

        public virtual void SetHistoricState(HistoricState s)
        {
            StringBuilder lsb = new StringBuilder();

            //Configuracion default
            this.Visible = true;
            pbtnConsultar.Visible = false;
            pbtnAgregar.Visible = false;
            pbtnEditar.Visible = false;
            pbtnGrabar.Visible = false;
            pbtnCancelar.Visible = false;
            pbtnRegresar.Visible = false;
            pbtnBaja.Visible = false;
            pExpAtributos.Visible = false;
            pHisGrid.Visible = false;
            pExpFiltros.Visible = false;
            pExpRegistro.Visible = false;
            pPanelSubHistoricos.Visible = false;

            piCodEntidad.DropDownList.Enabled = pbEnableEntidad;
            piCodMaestro.DropDownList.Enabled = pbEnableMaestro;

            if (pbEnableEntidad)
            {
                ((TableRow)piCodEntidad.TcCtl.Parent).Style.Remove("display");
            }
            else
            {
                ((TableRow)piCodEntidad.TcCtl.Parent).Style["display"] = "none";
            }

            if (pbEnableMaestro)
            {
                ((TableRow)piCodMaestro.TcCtl.Parent).Style.Remove("display");
            }
            else
            {
                ((TableRow)piCodMaestro.TcCtl.Parent).Style["display"] = "none";
            }

            pTablaRegistro.Rows[pvchDescripcion.Row - 1].Visible = false;
            pTablaRegistro.Rows[pdtIniVigencia.Row - 1].Visible = false;
            pTablaRegistro.Rows[pbReplicarClientes.Row - 1].Visible = false;

            //Configuracion especifica del estado
            if (s == HistoricState.Inicio)
            {
                pExpRegistro.Visible = true;
                pbtnCancelar.Visible = EsSubHistorico;
            }
            else if (s == HistoricState.MaestroSeleccionado)
            {
                pbtnAgregar.Visible = ValidarPermiso(Permiso.Agregar);
                pHisGrid.Visible = true;
                pExpFiltros.Visible = true;

                if (!pbEnableEntidad && !pbEnableMaestro)
                {
                    pExpRegistro.Visible = false;
                }
                else
                {
                    pExpRegistro.Visible = true;
                }

                pbtnCancelar.Visible = EsSubHistorico;

                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".iCodRegistro = " + iCodRegistro + ";");
                lsb.AppendLine(pjsObj + ".iCodCatalogo = " + iCodCatalogo + ";");
                lsb.AppendLine(pjsObj + ".iCodMaestro = " + iCodMaestro + ";");
                lsb.AppendLine(pjsObj + ".iCodEntidad = " + iCodEntidad + ";");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "RegistrosMS", lsb.ToString(), true, false);
            }
            else if (s == HistoricState.Consulta)
            {
                pbtnConsultar.Visible = ValidarPermiso(Permiso.Consultar);
                pbtnEditar.Visible = ValidarPermiso(Permiso.Editar);
                pbtnRegresar.Visible = true;
                pbtnBaja.Visible = ValidarPermiso(Permiso.Eliminar);
                pExpAtributos.Visible = true;
                pExpRegistro.Visible = true;
                pPanelSubHistoricos.Visible = true;

                pTablaRegistro.Rows[pvchDescripcion.Row - 1].Visible = true;
                pvchCodigo.TextBox.Enabled = false;
                pvchDescripcion.TextBox.Enabled = false;

                pTablaRegistro.Rows[pdtIniVigencia.Row - 1].Visible = true;
                pdtIniVigencia.DateTimeBox.Enabled = false;
                pdtFinVigencia.DateTimeBox.Enabled = false;

                pTablaRegistro.Rows[pbReplicarClientes.Row - 1].Visible = false;

                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".iCodRegistro = " + iCodRegistro + ";");
                lsb.AppendLine(pjsObj + ".iCodCatalogo = " + iCodCatalogo + ";");
                lsb.AppendLine(pjsObj + ".iCodMaestro = " + iCodMaestro + ";");
                lsb.AppendLine(pjsObj + ".iCodEntidad = " + iCodEntidad + ";");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "RegistrosC", lsb.ToString(), true, false);
            }
            else if (s == HistoricState.Edicion)
            {
                pbtnGrabar.Visible = true;
                pbtnCancelar.Visible = true;
                pExpAtributos.Visible = true;
                pExpRegistro.Visible = true;

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
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "Edit", lsb.ToString(), true, false);
            }
            else if (s == HistoricState.Baja)
            {
                pbtnGrabar.Visible = ValidarPermiso(Permiso.Eliminar);
                pbtnCancelar.Visible = true;
                pExpAtributos.Visible = true;
                pExpRegistro.Visible = true;

                piCodEntidad.DropDownList.Enabled = false;
                piCodMaestro.DropDownList.Enabled = false;

                pvchCodigo.TextBox.Enabled = false;
                pvchDescripcion.TextBox.Enabled = false;

                pTablaRegistro.Rows[pvchDescripcion.Row - 1].Visible = true;
                pTablaRegistro.Rows[pdtIniVigencia.Row - 1].Visible = true;

                pdtIniVigencia.DateTimeBox.Enabled = false;
                pdtFinVigencia.DateTimeBox.Enabled = true;

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
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "Del", lsb.ToString(), true, false);
            }
            else if (s == HistoricState.SubHistorico)
            {
                this.Visible = false;
                pSubHistorico.Visible = true;

                pSubHistorico.SetEntidad(pSubHistorico.vchCodEntidad);
                pSubHistorico.SetMaestro(pSubHistorico.vchDesMaestro);
                pSubHistorico.SetHistoricState(pSubHistorico.State);
            }
            else if (s == HistoricState.SubHistoricoRel)
            {
                this.Visible = false;
                pSubHistorico.Visible = true;
            }
            else if (s == HistoricState.CnfgSubHistoricField)
            {
                this.Visible = false;
                pSubHistorico.Visible = true;

                pSubHistorico.SetEntidad(pSubHistorico.vchCodEntidad);
                pSubHistorico.SetMaestro(pSubHistorico.vchDesMaestro);
                pSubHistorico.SetHistoricState(pSubHistorico.State);
            }

            State = s;
        }

        protected virtual void OcultaCampo(string lsConfigName)
        {
            if (pFields != null && pFields.ContainsConfigName(lsConfigName))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName(lsConfigName).Row - 1].Visible = false;
            }
        }

        protected virtual void OcultaCampoFiltro(string lsConfigName)
        {
            if (pFields != null && pFields.ContainsConfigName(lsConfigName)
                && phtFiltros.ContainsKey(pFields.GetByConfigName(lsConfigName).Column))
            {
                pTablaFiltros.Rows[((DSOTextBox)phtFiltros[pFields.GetByConfigName(lsConfigName).Column]).Row - 1].Visible = false;
            }
        }

        protected virtual void piCodMaestro_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAjaxControls();
            iCodEntidad = piCodEntidad.HasValue ? piCodEntidad.DataValue.ToString() : null;
            iCodMaestro = piCodMaestro.HasValue ? piCodMaestro.DataValue.ToString() : null;

            PrevState = State;
            InitMaestro();

            FirePostMaestroSelectedIndexChanged();
        }

        public virtual void InitMaestro()
        {
            iCodRegistro = "null";
            InitFields();
            pFields.FillControls();
            pFields.DisableFields();

            if (iCodMaestro != null)
            {
                SetHistoricState(HistoricState.MaestroSeleccionado);
                CreateGrid();
                InitGrid();
                InitFiltros();
                ClearFiltros();
            }
            else
            {
                SetHistoricState(HistoricState.Inicio);
                pTablaFiltros.Controls.Clear();
            }
        }

        protected virtual void pbtnConsultar_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            ConsultarRegistro();

            FirePostConsultarClick();
        }

        protected virtual void pbtnAgregar_ServerClick(object sender, EventArgs e)
        {
            pFields.IniVigencia = DateTime.Today;
            AgregarRegistro();

            FirePostAgregarClick();
        }

        protected virtual void pbtnEditar_ServerClick(object sender, EventArgs e)
        {
            EditarRegistro();

            FirePostEditarClick();
        }

        protected virtual void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            if (State != HistoricState.Baja)
                FillAjaxControls();
            else
                FillAjaxControls(false);

            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();

            PrevState = State;

            if (ValidarRegistro())
            {
                if (GrabarArchivos())
                {
                    GrabarRegistro();
                    ConsultarRegistro();
                }
            }
           
            FirePostGrabarClick();
        }

        protected virtual void pbtnCancelar_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            if (State == HistoricState.Edicion
                || State == HistoricState.Baja)
            {
                ConsultarRegistro();
            }

            FirePostCancelarClick();
        }

        protected virtual void pbtnRegresar_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            SetHistoricState(HistoricState.MaestroSeleccionado);
            iCodRegistro = "null";
            InitFields();
            pFields.FillControls();
            pFields.DisableFields();

            FirePostRegresarClick();
        }

        protected virtual void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            SetHistoricState(HistoricState.Baja);
            pFields.DisableFields();
            pdtFinVigencia.DataValue = DateTime.Today;

            FirePostBajaClick();
        }

        public virtual void ConsultarRegistro()
        {
            if (iCodRegistro != "null")
            {
                DataTable lDataTable = GetDatosRegistro();

                if (lDataTable.Rows.Count > 0)
                {
                    DataRow lDataRow = lDataTable.Rows[0];
                    pFields.SetValues(lDataRow);

                    pvchCodigo.DataValue = lDataRow["vchCodigo"];
                    pvchDescripcion.DataValue = lDataRow["vchDescripcion"];
                    pdtIniVigencia.DataValue = lDataRow["dtIniVigencia"];
                    pdtFinVigencia.DataValue = lDataRow["dtFinVigencia"];

                    SetHistoricState(HistoricState.Consulta);
                }
                else
                {
                    iCodRegistro = "null";
                    SetHistoricState(HistoricState.MaestroSeleccionado);
                    pHisGrid.TxtState.Text = "";

                    InitFields();
                    pFields.FillControls();
                }
            }
            else
            {
                SetHistoricState(HistoricState.MaestroSeleccionado);

                InitFields();
                pFields.FillControls();
            }
            pFields.DisableFields();
        }

        protected virtual DataTable GetDatosRegistro()
        {
            DataTable lDataTable = pKDB.GetHisRegByEnt(vchCodEntidad, vchDesMaestro, "iCodRegistro = " + iCodRegistro);
            if (lDataTable.Rows.Count > 0)
            {
                iCodCatalogo = lDataTable.Rows[0]["iCodCatalogo"].ToString();
            }
            else
            {
                iCodCatalogo = null;
            }
            return lDataTable;
        }

        protected virtual void AgregarRegistro()
        {
            PrevState = State;
            SetHistoricState(HistoricState.Edicion);
            pFields.EnableFields();

            pvchCodigo.DataValue = DBNull.Value;
            pvchDescripcion.DataValue = DBNull.Value;

            pdtIniVigencia.DataValue = DBNull.Value;
            pdtFinVigencia.DataValue = DBNull.Value;
        }

        protected virtual void EditarRegistro()
        {
            PrevState = State;
            SetHistoricState(HistoricState.Edicion);
            pFields.EnableFields();
        }

        protected virtual bool ValidarRegistro()
        {
            return ValidarVigencias()
                && ValidarCampos()
                && ValidarClaves()
                && ValidarRelaciones()
                && ValidarRelCatBlancos()
                && ValidarRelCatVig()
                && ValidarAtribCatalogosVig()
                && ValidarDatos();
        }

        protected virtual bool ValidarClaves()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            DataTable ldt;

            try
            {
                if (pvchCodigo.HasValue)
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, H.iCodMaestro, H.dtIniVigencia, H.dtFinVigencia");
                    psbQuery.AppendLine("from Historicos H, Catalogos C");
                    psbQuery.AppendLine("where H.iCodCatalogo = C.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodRegistro + ",-1)");
                    psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ",-1)");
                    psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
                    psbQuery.AppendLine("and C.vchCodigo = " + pvchCodigo.DataValue);
                    psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                    psbQuery.AppendLine("and ((H.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia <= " + pdtFinVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia >= " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");
                    psbQuery.AppendLine("order by H.dtIniVigencia, H.dtFinVigencia, H.iCodRegistro");

                    ldt = DSODataAccess.Execute(psbQuery.ToString());

                    string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                    foreach (DataRow ldataRow in ldt.Rows)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat), ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat)));
                        lsbErroresCodigos.Append("<li>" + lsError + "</li>");
                    }

                    if (lsbErroresCodigos.Length > 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidaVchCodigo", pvchCodigo.ToString()));
                        lsError = "<span>" + lsError + "</span>";
                        lsbErrores.Append("<li>" + lsError);
                        lsbErrores.Append("<ul>" + lsbErroresCodigos.ToString() + "</ul>");
                        lsbErrores.Append("</li>");
                    }
                }
                else
                {
                    lsbErrores.Append("<li>" + pvchCodigo.RequiredMessage + "</li>");
                }

                if (!pvchDescripcion.HasValue)
                {
                    lsbErrores.Append("<li>" + pvchDescripcion.RequiredMessage + "</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarVigencias()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                if (!pdtIniVigencia.HasValue)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pdtIniVigencia.Descripcion));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                //Si no se proporciono valor para la fecha de fin entonces se establece el valor default
                if (!pdtFinVigencia.HasValue)
                {
                    pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
                }

                //Validar que fin de vigencia sea mayor o igual a inicio de vigencia
                if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date > pdtFinVigencia.Date)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "VigenciaFin", pdtIniVigencia.Descripcion, pdtFinVigencia.Descripcion));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarCampos()
        {
            //si el registro se esta eliminando entonces no es necesaria la validacion de campos obligatorios
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            DataRow lRowMaestro = DSODataAccess.ExecuteDataRow("select * from Maestros where iCodRegistro = " + iCodMaestro);

            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            bool lbRequerido;

            try
            {
                //Validar Campos Obligatorios
                foreach (KeytiaBaseField lField in pFields)
                {
                    if (!lRowMaestro.Table.Columns.Contains(lField.Column))
                    {
                        continue;
                    }

                    if (lRowMaestro[lField.Column + "Req"] == DBNull.Value
                        || int.Parse(lRowMaestro[lField.Column + "Req"].ToString()) == 0)
                    {
                        lbRequerido = false;
                    }
                    else
                    {
                        lbRequerido = true;
                    }

                    if (lbRequerido && !lField.DSOControlDB.HasValue)
                    {
                        if (lField is KeytiaRelationField)
                        {
                            int liCount = 0;
                            KeytiaBaseField lFieldEntidad = ((KeytiaRelationField)lField).Fields.GetByConfigValue(int.Parse(iCodEntidad));
                            psbQuery.Length = 0;
                            psbQuery.AppendLine("select count(iCodRegistro) from Relaciones");
                            psbQuery.AppendLine("where iCodRelacion = " + lField.ConfigValue);
                            psbQuery.AppendLine("and " + lFieldEntidad.Column + " = isnull(" + iCodCatalogo + ",0)");

                            liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());
                            if (liCount == 0)
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelacionRequerida", lField.Descripcion));
                                lsbErrores.Append("<li>" + lsError + "</li>");
                            }
                        }
                        else
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", lField.Descripcion));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                        }
                    }
                }
                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
                return lbret;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected virtual bool ValidarRelaciones()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresRelacion = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                //Validar relaciones
                KeytiaRelationField lRelField;
                StringBuilder lsbErrorRegistroRel = new StringBuilder();
                foreach (DataTable lDataTable in pdsRelValues.Tables)
                {
                    lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lDataTable.TableName));
                    lsbErroresRelacion.Length = 0;
                    foreach (DataRow lDataRow in lDataTable.Rows)
                    {
                        lsbErrorRegistroRel.Length = 0;

                        //Validar que fecha de inicio de vigencia tenga valor
                        if (lDataRow["dtIniVigencia"] == DBNull.Value)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pdtIniVigencia.Descripcion));
                            lsbErrorRegistroRel.Append("<li>" + lsError + "</li>");
                        }

                        //Si no se proporciono valor para la fecha de fin entonces se establece el valor default
                        if (lDataRow["dtFinVigencia"] == DBNull.Value)
                        {
                            lDataRow["dtFinVigencia"] = new DateTime(2079, 1, 1);
                        }

                        //Validar que fin de vigencia sea mayor o igual a inicio de vigencia
                        if (lDataRow["dtIniVigencia"] != DBNull.Value && lDataRow["dtFinVigencia"] != DBNull.Value
                            && (DateTime)lDataRow["dtIniVigencia"] > (DateTime)lDataRow["dtFinVigencia"])
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "VigenciaFin", pdtIniVigencia.Descripcion, pdtFinVigencia.Descripcion));
                            lsbErrorRegistroRel.Append("<li>" + lsError + "</li>");
                        }

                        if (lsbErrorRegistroRel.Length > 0)
                        {
                            lsError = Globals.GetMsgWeb(false, "RegistroRel");
                            lsError += "(" + lDataRow["iCodRegistro"] + ")";
                            lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                            lsbErroresRelacion.Append("<li>" + lsError);
                            lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                            lsbErroresRelacion.Append("</li>");
                        }
                    }

                    if (lsbErroresRelacion.Length > 0)
                    {
                        lsError = DSOControl.JScriptEncode(lRelField.DSOControlDB.Descripcion);
                        lsError = "<span>" + lsError + "</span>";
                        lsbErrores.Append("<li>" + lsError);
                        lsbErrores.Append("<ul>" + lsbErroresRelacion.ToString() + "</ul>");
                        lsbErrores.Append("</li>");
                    }
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarDatos()
        {
            return true;
        }

        protected virtual bool ValidarRelCatBlancos()
        {
            bool lbret = true;
            foreach (KeytiaBaseField lRelField in pFields)
            {
                if (lRelField is KeytiaRelationField)
                {
                    lbret = ValidarRelCatBlancos(lRelField.ConfigName);
                    if (!lbret)
                    {
                        break;
                    }
                }
            }

            return lbret;
        }

        protected virtual bool ValidarRelCatBlancos(string lsRelacion)
        {
            //Valida que los registros de la relacion no tengan campos en blanco
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresRelacion = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                //Validar relaciones
                KeytiaBaseField lField;
                KeytiaRelationField lRelField;
                StringBuilder lsbErrorRegistroRel = new StringBuilder();
                DataTable lDataTable = pdsRelValues.Tables[lsRelacion];

                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lDataTable.TableName));
                lsbErrores.Length = 0;
                lsbErroresRelacion.Length = 0;
                foreach (DataRow lDataRow in lDataTable.Rows)
                {
                    //Si se esta eliminando el registro de la relacion entonces no se valida
                    if ((DateTime)lDataRow["dtIniVigencia"] == (DateTime)lDataRow["dtFinVigencia"])
                    {
                        continue;
                    }

                    lsbErrorRegistroRel.Length = 0;

                    foreach (DataColumn lDataCol in lDataTable.Columns)
                    {
                        //Validar que las columnas de catalogos tengan valor
                        if (lDataCol.ColumnName.StartsWith("iCodCatalogo")
                            && !lDataCol.ColumnName.Contains("Display")
                            && lRelField.Fields.Contains(lDataCol.ColumnName)
                            && (lField = lRelField.Fields[lDataCol.ColumnName]).ConfigValue.ToString() != iCodEntidad
                            && lDataRow[lDataCol] == DBNull.Value)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", lField.Descripcion));
                            lsbErrorRegistroRel.Append("<li>" + lsError + "</li>");
                        }
                    }

                    if (lsbErrorRegistroRel.Length > 0)
                    {
                        lsError = Globals.GetMsgWeb(false, "RegistroRel");
                        lsError += "(" + lDataRow["iCodRegistro"] + ")";
                        lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                        lsbErroresRelacion.Append("<li>" + lsError);
                        lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                        lsbErroresRelacion.Append("</li>");
                    }
                }

                if (lsbErroresRelacion.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(lRelField.DSOControlDB.Descripcion);
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresRelacion.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
                }


                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarRelCatVig()
        {
            bool lbret = true;
            foreach (KeytiaBaseField lRelField in pFields)
            {
                if (lRelField is KeytiaRelationField)
                {
                    lbret = ValidarRelCatVig(lRelField.ConfigName);
                    if (!lbret)
                    {
                        break;
                    }
                }
            }

            return lbret;
        }

        protected virtual bool ValidarRelCatVig(string lsRelacion)
        {
            //Valida la existencia de todos los catalogos para la vigencia de los registros de relacion
            //se asume que ya se mando llamar ValidarRelaciones
            //se asume que todos los registros de las relacion traen valores de vigencias
            //no se validan los campos de los catalogos que esten en null en el registro de la relacion
            //se asume que no hay empalmes de vigencias para los registros de historicos con un mismo iCodCatalogo
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresRelacion = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                KeytiaBaseField lField;
                KeytiaRelationField lRelField;
                StringBuilder lsbErrorRegistroRel = new StringBuilder();
                StringBuilder lsbErrorCampoRel = new StringBuilder();
                DataTable lDataTableHis;
                StringBuilder lsbQueryHis = new StringBuilder();
                DataRow[] ladrHis;
                string lsFiltro;
                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                DataTable lDataTable = pdsRelValues.Tables[lsRelacion];

                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lDataTable.TableName));
                lsbErroresRelacion.Length = 0;
                foreach (DataRow lDataRow in lDataTable.Rows)
                {
                    //Si se esta eliminando el registro de la relacion entonces no se valida
                    if ((DateTime)lDataRow["dtIniVigencia"] == (DateTime)lDataRow["dtFinVigencia"])
                    {
                        continue;
                    }
                    lsbErrorRegistroRel.Length = 0;
                    lsbQueryHis.Length = 0;
                    lsbQueryHis.AppendLine("select");
                    lsbQueryHis.AppendLine("    H.iCodCatalogo,");
                    lsbQueryHis.AppendLine("    H.dtIniVigencia,");
                    lsbQueryHis.AppendLine("    H.dtFinVigencia");
                    lsbQueryHis.AppendLine("from Historicos H");
                    lsbQueryHis.AppendLine("where H.dtIniVigencia <> H.dtFinVigencia");
                    lsbQueryHis.Append("and H.iCodCatalogo in(0");

                    foreach (DataColumn lDataCol in lDataTable.Columns)
                    {
                        //Validar las columnas de catalogos que tengan valor
                        if (lDataCol.ColumnName.StartsWith("iCodCatalogo")
                            && !lDataCol.ColumnName.Contains("Display")
                            && lRelField.Fields.Contains(lDataCol.ColumnName)
                            && lRelField.Fields[lDataCol.ColumnName].ConfigValue.ToString() != iCodEntidad
                            && lDataRow[lDataCol] != DBNull.Value)
                        {
                            lsbQueryHis.Append("," + lDataRow[lDataCol].ToString());
                        }
                    }
                    lsbQueryHis.AppendLine(")");
                    lsbQueryHis.AppendLine(DSOControl.ComplementaVigenciasJS(lDataRow["dtIniVigencia"], lDataRow["dtFinVigencia"], false, "H."));
                    lsbQueryHis.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ", 0)");
                    if (iCodRegistro != "null")
                    {
                        lsbQueryHis.AppendLine("union");
                        lsbQueryHis.AppendLine("select");
                        lsbQueryHis.AppendLine("    H.iCodCatalogo,");
                        lsbQueryHis.AppendLine("    dtIniVigencia = case when H.dtIniVigencia >= " + pdtIniVigencia.DataValue + " then " + pdtFinVigencia.DataValue + " else H.dtIniVigencia end,");
                        lsbQueryHis.AppendLine("    dtFinVigencia = case when H.dtFinVigencia <= " + pdtFinVigencia.DataValue + " then " + pdtIniVigencia.DataValue + " else H.dtFinVigencia end");
                        lsbQueryHis.AppendLine("from Historicos H");
                        lsbQueryHis.AppendLine("where H.dtIniVigencia <> H.dtFinVigencia");
                        lsbQueryHis.AppendLine("and H.iCodRegistro <> " + iCodRegistro);
                        lsbQueryHis.AppendLine("and H.iCodCatalogo = " + iCodCatalogo);
                        lsbQueryHis.AppendLine("and Not (H.dtIniVigencia >= " + pdtIniVigencia.DataValue + " and H.dtFinVigencia <= " + pdtFinVigencia.DataValue + ")");
                        lsbQueryHis.AppendLine(DSOControl.ComplementaVigenciasJS(lDataRow["dtIniVigencia"], lDataRow["dtFinVigencia"], false, "H."));
                    }

                    lsbQueryHis.AppendLine("union");
                    lsbQueryHis.AppendLine("select");
                    lsbQueryHis.AppendLine("    iCodCatalogo = isnull(" + iCodCatalogo + ", 0),");
                    lsbQueryHis.AppendLine("    dtIniVigencia = " + pdtIniVigencia.DataValue + ",");
                    lsbQueryHis.AppendLine("    dtFinVigencia = " + pdtFinVigencia.DataValue + " ");

                    lsbQueryHis.AppendLine("order by iCodCatalogo, dtIniVigencia");

                    lDataTableHis = DSODataAccess.Execute(lsbQueryHis.ToString());

                    foreach (DataColumn lDataCol in lDataTable.Columns)
                    {
                        lsbErrorCampoRel.Length = 0;
                        if (lDataCol.ColumnName.StartsWith("iCodCatalogo")
                            && !lDataCol.ColumnName.Contains("Display")
                            && lRelField.Fields.Contains(lDataCol.ColumnName))
                        {
                            lField = lRelField.Fields[lDataCol.ColumnName];
                            if (lField.ConfigValue.ToString() != iCodEntidad
                                && lDataRow[lDataCol] == DBNull.Value)
                            {
                                //Si no es el campo de la entidad del historico y tiene valor nulo entonces no se valida
                                //ya que no se pueden obtener datos de historicos que validar
                                continue;
                            }
                            else if (lField.ConfigValue.ToString() == iCodEntidad)
                            {
                                //Si es el campo de la entidad del historico y se esta agregando entonces se toma su valor como cero
                                lsFiltro = iCodCatalogo == "null" ? "0" : iCodCatalogo;
                            }
                            else
                            {
                                //es cualquier otro campo de la relacion que no es el campo de la entidad del historico
                                lsFiltro = lDataRow[lDataCol].ToString();
                            }

                            ladrHis = lDataTableHis.Select("iCodCatalogo = " + lsFiltro, "dtIniVigencia ASC");
                            if (ladrHis.Length == 0)
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)lDataRow["dtIniVigencia"]).ToString(lsDateFormat), ((DateTime)lDataRow["dtFinVigencia"]).ToString(lsDateFormat)));
                                lsbErrorCampoRel.Append("<li>" + lsError + "</li>");
                            }
                            else if ((DateTime)ladrHis[0]["dtIniVigencia"] > (DateTime)lDataRow["dtIniVigencia"])
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)lDataRow["dtIniVigencia"]).ToString(lsDateFormat), ((DateTime)ladrHis[0]["dtIniVigencia"]).ToString(lsDateFormat)));
                                lsbErrorCampoRel.Append("<li>" + lsError + "</li>");
                            }

                            for (int lidx = 0; lidx < ladrHis.Length; lidx++)
                            {
                                if (lidx > 0 && (DateTime)ladrHis[lidx]["dtIniVigencia"] > (DateTime)ladrHis[lidx - 1]["dtFinVigencia"])
                                {
                                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ladrHis[lidx - 1]["dtFinVigencia"]).ToString(lsDateFormat), ((DateTime)ladrHis[lidx]["dtIniVigencia"]).ToString(lsDateFormat)));
                                    lsbErrorCampoRel.Append("<li>" + lsError + "</li>");
                                }
                            }

                            if (ladrHis.Length > 0 && (DateTime)lDataRow["dtFinVigencia"] > (DateTime)ladrHis[ladrHis.Length - 1]["dtFinVigencia"])
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ladrHis[ladrHis.Length - 1]["dtFinVigencia"]).ToString(lsDateFormat), ((DateTime)lDataRow["dtFinVigencia"]).ToString(lsDateFormat)));
                                lsbErrorCampoRel.Append("<li>" + lsError + "</li>");
                            }
                        }

                        if (lsbErrorCampoRel.Length > 0)
                        {
                            lField = lRelField.Fields[lDataCol.ColumnName];
                            lsError = Globals.GetMsgWeb(false, "ValidarRelCatVig", lField.Descripcion);
                            lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                            lsbErrorRegistroRel.Append("<li>" + lsError);
                            lsbErrorRegistroRel.Append("<ul>" + lsbErrorCampoRel.ToString() + "</ul>");
                            lsbErrorRegistroRel.Append("</li>");
                        }
                    }

                    if (lsbErrorRegistroRel.Length > 0)
                    {
                        lsError = Globals.GetMsgWeb(false, "RegistroRel");
                        lsError += "(" + lDataRow["iCodRegistro"] + ")";
                        lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                        lsbErroresRelacion.Append("<li>" + lsError);
                        lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                        lsbErroresRelacion.Append("</li>");
                    }
                }

                if (lsbErroresRelacion.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(lRelField.DSOControlDB.Descripcion);
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresRelacion.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarRelTraslapeVig(string lsRelacion)
        {
            //Valida que no se tralapen las vigencias de los registros de la relacion para los valores de la entidad
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresRelacion = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                KeytiaBaseField lFieldEntHis = null;
                KeytiaRelationField lRelField;
                StringBuilder lsbErrorRegistroRel = new StringBuilder();
                DataTable lDataTable;
                StringBuilder lsbQuery = new StringBuilder();
                string lsFiltro;
                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                DataTable lEditedData = pdsRelValues.Tables[lsRelacion];

                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lEditedData.TableName));

                lsbQuery.AppendLine("select iCodRegistro");
                foreach (KeytiaBaseField lFieldAux in lRelField.Fields)
                {
                    if (lFieldAux.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbQuery.AppendLine("   ," + lFieldAux.Column);
                        if (lFieldAux.ConfigValue.ToString() == iCodEntidad)
                        {
                            lFieldEntHis = lFieldAux;
                        }
                    }
                }
                lsbQuery.AppendLine("   ,dtIniVigencia");
                lsbQuery.AppendLine("   ,dtFinVigencia");
                lsbQuery.AppendLine("from Relaciones");
                lsbQuery.AppendLine("where iCodRelacion  = " + lRelField.ConfigValue);
                lsbQuery.AppendLine("and " + lFieldEntHis.Column + " = isnull(" + iCodCatalogo + ",0)");

                List<string> lstRegistros = new List<string>();
                foreach (DataRow lEditedRow in lEditedData.Rows)
                {
                    //se convierte a entero para validar que efectivamente traiga un entero ya que estos valores
                    //se agregan con js del lado del cliente
                    lstRegistros.Add(int.Parse(lEditedRow["iCodRegistro"].ToString()).ToString());
                }
                if (lstRegistros.Count > 0)
                {
                    lsFiltro = String.Join(",", lstRegistros.ToArray());
                    lsbQuery.AppendLine("and iCodRegistro not in(" + lsFiltro + ")");
                }

                lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("order by iCodRegistro");

                lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

                //Agrego los datos editados a la tabla que contiene los datos guardados en la base de datos
                //para despues validar con una sola tabla
                if (lEditedData.Rows.Count > 0)
                {
                    foreach (DataRow lEditedRow in lEditedData.Select("dtIniVigencia <> dtFinVigencia")) //Se excluyen los registros que se están eliminando
                    {
                        DataRow lDataRow = lDataTable.NewRow();
                        foreach (DataColumn lDataCol in lDataTable.Columns)
                        {
                            if (lEditedData.Columns.Contains(lDataCol.ColumnName))
                            {
                                lDataRow[lDataCol] = lEditedRow[lDataCol.ColumnName];
                            }
                        }
                        lDataTable.Rows.Add(lDataRow);
                    }
                }
                DataView lDataView = lDataTable.DefaultView;
                lDataView.Sort = "iCodRegistro ASC";

                lsbErroresRelacion.Length = 0;
                DataRow[] larTraslape;
                foreach (DataRowView lViewRow in lDataView)
                {
                    lsbErrorRegistroRel.Length = 0;
                    lsFiltro = "iCodRegistro <> " + lViewRow["iCodRegistro"];
                    if (lViewRow[lFieldEntHis.Column] == DBNull.Value)
                    {
                        lsFiltro += " And " + lFieldEntHis.Column + " is null";
                    }
                    else
                    {
                        lsFiltro += " And " + lFieldEntHis.Column + " = " + lViewRow[lFieldEntHis.Column];
                    }

                    lsFiltro += DSOControl.ComplementaVigenciasJS(lViewRow["dtIniVigencia"], lViewRow["dtFinVigencia"], false);
                    larTraslape = lDataTable.Select(lsFiltro);
                    if (larTraslape.Length > 0)
                    {

                        lstRegistros = new List<string>();
                        foreach (DataRow lDataRow in larTraslape)
                        {
                            lstRegistros.Add(lDataRow["iCodRegistro"].ToString());
                        }
                        lsError = String.Join(", ", lstRegistros.ToArray());
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslape", lsError));
                        lsbErrorRegistroRel.Append("<li>" + lsError + "</li>");
                    }

                    if (lsbErrorRegistroRel.Length > 0)
                    {
                        lsError = Globals.GetMsgWeb(false, "RegistroRel");
                        lsError += "(" + lViewRow["iCodRegistro"] + ")";
                        lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                        lsbErroresRelacion.Append("<li>" + lsError);
                        lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                        lsbErroresRelacion.Append("</li>");
                    }
                }

                if (lsbErroresRelacion.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(lRelField.DSOControlDB.Descripcion);
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresRelacion.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarRelTraslapeVig(string lsRelacion, string lsEntidad)
        {
            //Valida que no se tralapen las vigencias de los registros de la relacion para los valores de la entidad
            //la entidad que recibe debe de ser diferente a la del historico ya que si es la misma se debe de llamar
            //el metodo que no recibe el parametro de entidad
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresRelacion = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                KeytiaBaseField lFieldEntHis = null;
                KeytiaBaseField lFieldEntidad = null;
                KeytiaRelationField lRelField;
                StringBuilder lsbErrorRegistroRel = new StringBuilder();
                StringBuilder lsbErrorCampoRel = new StringBuilder();
                DataTable lDataTable;
                StringBuilder lsbQuery = new StringBuilder();
                string lsFiltro;
                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                DataTable lEditedData = pdsRelValues.Tables[lsRelacion];

                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lEditedData.TableName));

                lsbQuery.AppendLine("select iCodRegistro");
                foreach (KeytiaBaseField lFieldAux in lRelField.Fields)
                {
                    if (lFieldAux.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbQuery.AppendLine("   ," + lFieldAux.Column);
                        if (lFieldAux.ConfigValue.ToString() == iCodEntidad)
                        {
                            lFieldEntHis = lFieldAux;
                        }
                        if (lFieldAux.ConfigName == lsEntidad)
                        {
                            lFieldEntidad = lFieldAux;
                        }
                    }
                }
                lsbQuery.AppendLine("," + lFieldEntHis.Column + "Display = " + DSODataContext.Schema + ".GetCatDesc(" + lFieldEntHis.Column + "," + liCodIdioma + ",dtIniVigencia)");

                lsbQuery.AppendLine("   ,dtIniVigencia");
                lsbQuery.AppendLine("   ,dtFinVigencia");
                lsbQuery.AppendLine("from Relaciones");
                lsbQuery.AppendLine("where iCodRelacion  = " + lRelField.ConfigValue);

                List<string> lstRegistros = new List<string>();
                List<string> lstValoresEnt = new List<string>();
                foreach (DataRow lEditedRow in lEditedData.Rows)
                {
                    //se convierte a entero para validar que efectivamente traiga un entero ya que estos valores
                    //se agregan con js del lado del cliente
                    lstRegistros.Add(int.Parse(lEditedRow["iCodRegistro"].ToString()).ToString());
                    if (lEditedRow[lFieldEntidad.Column] != DBNull.Value)
                    {
                        lstValoresEnt.Add(int.Parse(lEditedRow[lFieldEntidad.Column].ToString()).ToString());
                    }
                    else
                    {
                        lstValoresEnt.Add("0");
                    }
                }
                if (lstRegistros.Count > 0)
                {
                    lsFiltro = String.Join(",", lstRegistros.ToArray());
                    lsbQuery.AppendLine("and iCodRegistro not in(" + lsFiltro + ")");
                }
                if (lstValoresEnt.Count > 0)
                {
                    lsFiltro = String.Join(",", lstValoresEnt.ToArray());
                    lsbQuery.AppendLine("and isnull(" + lFieldEntidad.Column + ",0) in(" + lsFiltro + ")");
                }

                lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("order by iCodRegistro");

                lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

                //Agrego los datos editados a la tabla que contiene los datos guardados en la base de datos
                //para despues validar con una sola tabla
                foreach (DataRow lEditedRow in lEditedData.Rows)
                {
                    DataRow lDataRow = lDataTable.NewRow();
                    foreach (DataColumn lDataCol in lDataTable.Columns)
                    {
                        if (iCodCatalogo == "null" && lDataCol.ColumnName == lFieldEntHis.Column)
                        {
                            lDataRow[lDataCol] = 0;
                        }
                        else if (lEditedData.Columns.Contains(lDataCol.ColumnName))
                        {
                            lDataRow[lDataCol] = lEditedRow[lDataCol.ColumnName];
                        }
                    }
                    lDataTable.Rows.Add(lDataRow);
                }

                DataView lDataView = lDataTable.DefaultView;
                lDataView.Sort = "iCodRegistro ASC";

                lsbErroresRelacion.Length = 0;
                DataRow[] larTraslape;
                foreach (DataRowView lViewRow in lDataView)
                {
                    if ((iCodCatalogo != "null" && lViewRow[lFieldEntHis.Column].ToString() == iCodCatalogo)
                        || (iCodCatalogo == "null" && lViewRow[lFieldEntHis.Column].ToString() == "0"))
                    {
                        lsbErrorRegistroRel.Length = 0;
                        lsFiltro = "iCodRegistro <> " + lViewRow["iCodRegistro"];
                        if (lViewRow[lFieldEntidad.Column] == DBNull.Value)
                        {
                            lsFiltro += " And " + lFieldEntidad.Column + " is null";
                        }
                        else
                        {
                            lsFiltro += " And " + lFieldEntidad.Column + " = " + lViewRow[lFieldEntidad.Column];
                        }

                        lsFiltro += DSOControl.ComplementaVigenciasJS(lViewRow["dtIniVigencia"], lViewRow["dtFinVigencia"], false);
                        larTraslape = lDataTable.Select(lsFiltro);
                        if (larTraslape.Length > 0)
                        {
                            lsbErrorCampoRel.Length = 0;
                            foreach (DataRow lDataRow in larTraslape)
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslapeEntReg", lDataRow["iCodRegistro"].ToString(), lFieldEntHis.Descripcion, lDataRow[lFieldEntHis.Column + "Display"].ToString()));
                                lsbErrorCampoRel.Append("<li>" + lsError + "</li>");
                            }
                            if (lsbErrorCampoRel.Length > 0)
                            {
                                lsError = Globals.GetMsgWeb(false, "RelTraslapeEnt");
                                lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                                lsbErrorRegistroRel.Append("<li>" + lsError);
                                lsbErrorRegistroRel.Append("<ul>" + lsbErrorCampoRel.ToString() + "</ul>");
                                lsbErrorRegistroRel.Append("</li>");
                            }
                        }

                        if (lsbErrorRegistroRel.Length > 0)
                        {
                            lsError = Globals.GetMsgWeb(false, "RegistroRel");
                            lsError += "(" + lViewRow["iCodRegistro"] + ")";
                            lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                            lsbErroresRelacion.Append("<li>" + lsError);
                            lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                            lsbErroresRelacion.Append("</li>");
                        }
                    }
                }

                if (lsbErroresRelacion.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(lRelField.DSOControlDB.Descripcion);
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresRelacion.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarAtribCatalogosVig()
        {
            //Valida la existencia de todos los catalogos para la vigencia del registro historico
            //se asume que ya se mando llamar ValidarCampos, ValidarVigencias
            //no se validan los campos de los catalogos que esten en null
            //se asume que no hay empalmes de vigencias para los registros de historicos con un mismo iCodCatalogo


            //si el registro se esta eliminando entonces no es necesaria la validacion
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErrorCampo = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {

                DataTable lDataTableHis;
                StringBuilder lsbQueryHis = new StringBuilder();
                DataRow[] ladrHis;
                string lsFiltro;
                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                lsbQueryHis.Length = 0;
                lsbQueryHis.AppendLine("select");
                lsbQueryHis.AppendLine("    H.iCodCatalogo,");
                lsbQueryHis.AppendLine("    H.dtIniVigencia,");
                lsbQueryHis.AppendLine("    H.dtFinVigencia");
                lsbQueryHis.AppendLine("from Historicos H");
                lsbQueryHis.AppendLine("where H.dtIniVigencia <> H.dtFinVigencia");
                lsbQueryHis.Append("and H.iCodCatalogo in(0");

                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && lField.DSOControlDB.HasValue
                        && (lField.ConfigValue.ToString() != iCodEntidad
                        || lField.DataValue.ToString() != iCodCatalogo))
                    {
                        lsbQueryHis.Append("," + lField.DataValue);
                    }
                }

                lsbQueryHis.AppendLine(")");
                lsbQueryHis.AppendLine(DSOControl.ComplementaVigenciasJS(pdtIniVigencia.Date, pdtFinVigencia.Date, false, "H."));
                lsbQueryHis.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ", 0)");
                lsbQueryHis.AppendLine("order by iCodCatalogo, dtIniVigencia");

                lDataTableHis = DSODataAccess.Execute(lsbQueryHis.ToString());


                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && lField.DSOControlDB.HasValue
                        && (lField.ConfigValue.ToString() != iCodEntidad
                        || lField.DataValue.ToString() != iCodCatalogo))
                    {

                        lsbErrorCampo.Length = 0;
                        lsFiltro = lField.DataValue.ToString();

                        ladrHis = lDataTableHis.Select("iCodCatalogo = " + lsFiltro, "dtIniVigencia ASC");
                        if (ladrHis.Length == 0)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", pdtIniVigencia.Date.ToString(lsDateFormat), pdtFinVigencia.Date.ToString(lsDateFormat)));
                            lsbErrorCampo.Append("<li>" + lsError + "</li>");
                        }
                        else if ((DateTime)ladrHis[0]["dtIniVigencia"] > pdtIniVigencia.Date)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", pdtIniVigencia.Date.ToString(lsDateFormat), ((DateTime)ladrHis[0]["dtIniVigencia"]).ToString(lsDateFormat)));
                            lsbErrorCampo.Append("<li>" + lsError + "</li>");
                        }

                        for (int lidx = 0; lidx < ladrHis.Length; lidx++)
                        {
                            if (lidx > 0 && (DateTime)ladrHis[lidx]["dtIniVigencia"] > (DateTime)ladrHis[lidx - 1]["dtFinVigencia"])
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ladrHis[lidx - 1]["dtFinVigencia"]).ToString(lsDateFormat), ((DateTime)ladrHis[lidx]["dtIniVigencia"]).ToString(lsDateFormat)));
                                lsbErrorCampo.Append("<li>" + lsError + "</li>");
                            }
                        }

                        if (ladrHis.Length > 0 && pdtFinVigencia.Date > (DateTime)ladrHis[ladrHis.Length - 1]["dtFinVigencia"])
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ladrHis[ladrHis.Length - 1]["dtFinVigencia"]).ToString(lsDateFormat), pdtFinVigencia.Date.ToString(lsDateFormat)));
                            lsbErrorCampo.Append("<li>" + lsError + "</li>");
                        }

                        if (lsbErrorCampo.Length > 0)
                        {
                            lsError = Globals.GetMsgWeb(false, "ValidarHisCatVig", lField.Descripcion);
                            lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                            lsbErrores.Append("<li>" + lsError);
                            lsbErrores.Append("<ul>" + lsbErrorCampo.ToString() + "</ul>");
                            lsbErrores.Append("</li>");
                        }
                    }
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual string GetSaveFolder()
        {
            //El directorio se calcula como \DirEsquema\vchCodEntidad\vchDesMaestro\Año\Mes\
            try
            {
                string lsSaveFolder = "";
                string lsDirEntidad = DSOUpload.EscapeFolderName(vchCodEntidad);
                string lsDirMaestro = DSOUpload.EscapeFolderName(vchDesMaestro);
                int liCodUsuarioDB;

                DataTable lKDBTable = pKDB.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo = " + Session["iCodUsuario"]);
                liCodUsuarioDB = (int)lKDBTable.Rows[0]["{UsuarDB}"];
                lKDBTable = pKDB.GetHisRegByEnt("UsuarDB", "Usuarios DB", "iCodCatalogo = " + liCodUsuarioDB);

                if (lKDBTable != null
                    && lKDBTable.Rows.Count > 0
                    && lKDBTable.Rows[0]["{SaveFolder}"] != DBNull.Value)
                {
                    lsSaveFolder = lKDBTable.Rows[0]["{SaveFolder}"].ToString().Trim();
                    lsSaveFolder = System.IO.Path.Combine(lsSaveFolder, lsDirEntidad);
                    lsSaveFolder = System.IO.Path.Combine(lsSaveFolder, lsDirMaestro);
                    lsSaveFolder = System.IO.Path.Combine(lsSaveFolder, DateTime.Now.Year.ToString());
                    lsSaveFolder = System.IO.Path.Combine(lsSaveFolder, DateTime.Now.Month.ToString("00"));
                }

                return lsSaveFolder.Trim();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrInitSaveFolder", ex);
            }
        }

        protected virtual bool GrabarArchivos()
        {
            StringBuilder lsbErrores = new StringBuilder();
            string lsError;
            bool lbret = true;

            try
            {
                string lsSaveFolder = "";
                var opc = Session["OpcMenu"].ToString();
                if(opc == "OpcCte")
                {
                    lsSaveFolder = Util.AppSettings("PathLogoCliente");
                }
                else
                {
                    lsSaveFolder = GetSaveFolder();
                }
                 

                if (lsSaveFolder != "")
                {
                    DSOUpload lDSOUpl;

                    foreach (KeytiaBaseField lField in pFields)
                    {
                        if (lField is KeytiaUploadField)
                        {
                            lDSOUpl = (DSOUpload)lField.DSOControlDB;
                            lDSOUpl.SaveFolder = lsSaveFolder;
                            if(lDSOUpl.HasValue)
                            {
                                //Valida que el tamaño del archivo no sea mayor al permitido
                                if (!lDSOUpl.FileExceedsLength)
                                {
                                    if (lDSOUpl.SaveFile())
                                    {
                                        if (opc == "OpcCte")
                                        {
                                            var dataValue = lField.DataValue;
                                            dataValue = dataValue.ToString().Replace(lsSaveFolder, "~/images");
                                            phtValues[lField.Column] = dataValue;
                                        }
                                        else
                                        {
                                            phtValues[lField.Column] = lField.DataValue;
                                        }

                                    }
                                    else
                                    {
                                        throw new KeytiaWebException("ErrSaveFiles");
                                    }
                                }
                                else
                                {
                                    lbret = false;
                                    lsError = DSOControl.JScriptEncode(lDSOUpl.ResultMessage);
                                    lsbErrores.Append("<li>" + lsError + "</li>");
                                }
                            }

                        
                        }
                    }

                    if (lsbErrores.Length > 0)
                    {
                        lbret = false;
                        lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, "Carga archivos");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveFiles", ex);
            }

            return lbret;
        }

        protected virtual void GrabarRegistro()
        {
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                if (State != HistoricState.Baja)
                {
                    int liCodRegistro;

                    phtValues.Add("iCodMaestro", int.Parse(iCodMaestro));
                    phtValues.Add("vchCodigo", pvchCodigo.DataValue);
                    phtValues.Add("vchDescripcion", pvchDescripcion.DataValue);
                    phtValues.Add("dtIniVigencia", pdtIniVigencia.Date);
                    phtValues.Add("dtFinVigencia", pdtFinVigencia.Date);
                    phtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                    //Mandar llamar al COM para grabar los datos del historico
                    if (iCodRegistro == "null")
                    {
                        liCodRegistro = lCargasCOM.InsertaRegistro(phtValues, "Historicos", vchCodEntidad, vchDesMaestro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                        if (liCodRegistro < 0)
                        {
                            throw new KeytiaWebException("ErrSaveRecord");
                        }
                        iCodRegistro = liCodRegistro.ToString();
                        iCodCatalogo = DSODataAccess.ExecuteScalar("select iCodCatalogo from Historicos where iCodRegistro = " + iCodRegistro).ToString();
                        GrabarRelaciones();
                    }
                    else
                    {
                        GrabarRelaciones();
                        liCodRegistro = int.Parse(iCodRegistro);
                        if (!lCargasCOM.ActualizaRegistro("Historicos", vchCodEntidad, vchDesMaestro, phtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked))
                        {
                            throw new KeytiaWebException("ErrSaveRecord");
                        }
                        //Obtener el iCodRegistro actualizado ya que cuando el historico tiene la bandera de actualizar la historia encendida se genera un nuevo registro
                        iCodRegistro = DSODataAccess.ExecuteScalar("select top 1 iCodRegistro from Historicos where iCodCatalogo = " + iCodCatalogo + " and iCodMaestro = " + iCodMaestro + " order by dtFecUltAct desc, iCodRegistro desc").ToString();
                    }

                }
                else
                {
                    phtValues.Add("iCodMaestro", int.Parse(iCodMaestro));
                    phtValues.Add("vchCodigo", pvchCodigo.DataValue);
                    phtValues.Add("vchDescripcion", pvchDescripcion.DataValue);
                    phtValues.Add("dtIniVigencia", pdtIniVigencia.Date);
                    phtValues.Add("dtFinVigencia", pdtFinVigencia.Date);
                    phtValues.Add("iCodUsuario", Session["iCodUsuario"]);
                    lCargasCOM.BajaHistorico(int.Parse(iCodRegistro), phtValues, int.Parse(Session["iCodUsuarioDB"].ToString()), false, pbReplicarClientes.CheckBox.Checked);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected virtual void GrabarRelaciones()
        {
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            int liCodRegistroRel;

            KeytiaRelationField lRelField;
            KeytiaBaseField lFieldEntidad;
            Hashtable lhtRel;
            int liCodEntidad = int.Parse(iCodEntidad);
            foreach (DataTable lDataTable in pdsRelValues.Tables)
            {
                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lDataTable.TableName));
                lFieldEntidad = lRelField.Fields.GetByConfigValue(liCodEntidad);
                foreach (DataRow lDataRow in lDataTable.Rows)
                {
                    lhtRel = new Hashtable();
                    lhtRel.Add("iCodRelacion", lRelField.ConfigValue);
                    lhtRel.Add("iCodUsuario", Session["iCodUsuario"]);
                    lhtRel.Add(lFieldEntidad.Column, iCodCatalogo);

                    foreach (DataColumn lDataCol in lDataTable.Columns)
                    {
                        if (lDataCol.ColumnName == "iCodRegistro")
                        {
                            if (lDataRow[lDataCol] != DBNull.Value && (int)lDataRow[lDataCol] > 0)
                            {
                                lhtRel.Add("iCodRegistro", lDataRow[lDataCol]);
                            }
                        }
                        else if (lDataCol.ColumnName != "iCodRelacion"
                            && lDataCol.ColumnName != "iCodUsuario"
                            && lDataCol.ColumnName != "dtFecUltAct"
                            && lDataCol.ColumnName != lFieldEntidad.Column
                            && lDataRow[lDataCol] != DBNull.Value
                            && lRelField.Fields.Contains(lDataCol.ColumnName))
                        {
                            lhtRel.Add(lDataCol.ColumnName, lDataRow[lDataCol]);
                        }
                        else if (lDataCol.ColumnName == "dtIniVigencia"
                            && lDataRow[lDataCol] == DBNull.Value)
                        {
                            lhtRel.Add(lDataCol.ColumnName, DateTime.Today);
                        }
                        else if (lDataCol.ColumnName == "dtFinVigencia"
                            && lDataRow[lDataCol] == DBNull.Value)
                        {
                            lhtRel.Add(lDataCol.ColumnName, new DateTime(2079, 1, 1));
                        }
                    }
                    //Mandar llamar al COM para grabar los datos de la relacion
                    //liCodRegistroRel = lCargasCOM.GuardaRelacion(lhtRel, lRelField.ConfigName, (int)Session["iCodUsuarioDB"]);
                    liCodRegistroRel = lCargasCOM.GuardaRelacion(lhtRel, lRelField.ConfigName, true, (int)Session["iCodusuarioDB"], pbReplicarClientes.CheckBox.Checked);

                    if (liCodRegistroRel < 0)
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                }
            }
        }

        protected virtual bool ValidarPermiso(Permiso p)
        {
            return DSONavegador.getPermiso(OpcMnu, p);
        }

        protected virtual void IniciaVigencia(bool lbIncluirFechaFin)
        {
            if (dtIniVigencia.HasValue)
            {
                pFields.IniVigencia = dtIniVigencia.Date;
            }
            else
            {
                pFields.IniVigencia = DateTime.Today;
            }
            if (lbIncluirFechaFin && dtFinVigencia.HasValue)
            {
                pFields.FinVigencia = dtFinVigencia.Date;
            }
            else
                pFields.ClearFinVigencia();
        }

        #region SubHistorico

        public HistoricEdit Historico
        {
            get
            {
                return pHistorico;
            }
            set
            {
                pHistorico = value;
            }
        }

        public HistoricEdit SubHistorico
        {
            get
            {
                return pSubHistorico;
            }
        }

        public string SubHistoricClass
        {
            get
            {
                if (ViewState["SubHistoricClass"] == null)
                {
                    ViewState["SubHistoricClass"] = "KeytiaWeb.UserInterface.HistoricEdit";
                }
                return ViewState["SubHistoricClass"].ToString();
            }
            set
            {
                ViewState["SubHistoricClass"] = value;
            }
        }

        public string SubCollectionClass
        {
            get
            {
                if (ViewState["SubCollectionClass"] == null)
                {
                    ViewState["SubCollectionClass"] = "KeytiaWeb.UserInterface.HistoricFieldCollection";
                }
                return ViewState["SubCollectionClass"].ToString();
            }
            set
            {
                ViewState["SubCollectionClass"] = value;
            }
        }

        public bool EsSubHistorico
        {
            get
            {
                if (ViewState["EsSubHistorico"] == null)
                {
                    ViewState["EsSubHistorico"] = false;
                }
                return (bool)ViewState["EsSubHistorico"];
            }
            set
            {
                ViewState["EsSubHistorico"] = value;
            }
        }

        public string SubHistoricoID
        {
            get
            {
                return (string)ViewState["SubHistoricoID"];
            }
            set
            {
                ViewState["SubHistoricoID"] = value;
            }
        }

        public virtual void InitSubHistorico(string lSubHistoricoID)
        {
            if (!String.IsNullOrEmpty(lSubHistoricoID))
            {
                this.SubHistoricoID = lSubHistoricoID;
                pSubHistorico = (HistoricEdit)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricEdit)).CodeBase, SubHistoricClass).Unwrap();
                pSubHistorico.ID = lSubHistoricoID;
                pSubHistorico.CollectionClass = this.SubCollectionClass;
                pSubHistorico.OpcMnu = this.OpcMnu;
                pSubHistorico.lblTitle = this.lblTitle;
                pSubHistorico.Historico = this;
                this.Parent.Controls.Add(pSubHistorico);
                pSubHistorico.LoadScripts();
                pSubHistorico.CreateControls();
            }
        }

        public virtual void RemoverSubHistorico()
        {
            if (SubHistoricoID != null)
            {
                pSubHistorico.RemoverSubHistorico();
                this.SubHistoricoID = null;
                pSubHistorico.CleanEntidad();
                pSubHistorico.SetHistoricState(HistoricState.Inicio);
                SetHistoricState(this.PrevState);
                this.Parent.Controls.Remove(pSubHistorico);
                this.SubHistoricClass = "KeytiaWeb.UserInterface.HistoricEdit";
                this.SubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";
                pSubHistorico = null;
            }
        }

        protected virtual void CnfgSubHistoricField_PostGrabarClick(object sender, EventArgs e)
        {
            if (pSubHistorico.State != HistoricState.Edicion
                && pSubHistorico.State != HistoricState.Baja)
            {
                RemoverSubHistorico();
            }
        }

        protected virtual void CnfgSubHistorico_PostCancelarClick(object sender, EventArgs e)
        {
            RemoverSubHistorico();
        }

        protected virtual void AgregarBoton(string lConfigName)
        {
            AgregarBoton(lConfigName, "KeytiaWeb.UserInterface.HistoricEdit", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }

        protected virtual void AgregarBoton(string lConfigName, string lSubHistoricClass, string lSubCollectionClass)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                lField.SubHistoricClass = lSubHistoricClass;
                lField.SubCollectionClass = lSubCollectionClass;

                HtmlButton lbtnSubHistorico = new HtmlButton();
                lbtnSubHistorico.ID = "btnAddField" + lField.Column;
                lbtnSubHistorico.Attributes["class"] = "buttonEditImg";
                lbtnSubHistorico.Attributes["DataField"] = lField.Column;
                lbtnSubHistorico.InnerText = "...";
                lbtnSubHistorico.ServerClick += new EventHandler(btnSubHistorico_ServerClick);

                lField.DSOControlDB.TcCtl.Controls.Add(lbtnSubHistorico);
            }
        }

        protected virtual void btnSubHistorico_ServerClick(object sender, EventArgs e)
        {
            FillAjaxControls();
            KeytiaBaseField lField = pFields[((HtmlButton)sender).Attributes["DataField"]];

            //las entidades que se utilicen con los botones que se agregan deben de tener un solo maestro
            string lsMaestro = DSODataAccess.ExecuteScalar("select top 1 vchDescripcion from Maestros where iCodEntidad = " + lField.ConfigValue + " and dtIniVigencia <> dtFinVigencia order by iCodRegistro").ToString();

            PrevState = State;
            SubHistoricClass = lField.SubHistoricClass; // "KeytiaWeb.UserInterface.CnfgReportesEdit";
            SubCollectionClass = lField.SubCollectionClass; // "KeytiaWeb.UserInterface.CnfgRepFieldCollection";

            InitSubHistorico(this.ID + "Ent" + lField.ConfigValue);

            pSubHistorico.SetEntidad(lField.ConfigName);
            pSubHistorico.SetMaestro(lsMaestro);

            pSubHistorico.EsSubHistorico = true;
            pSubHistorico.FillControls();

            SetHistoricState(HistoricState.SubHistorico);
            pSubHistorico.InitMaestro();

            if (lField.DSOControlDB.HasValue)
            {
                KDBAccess kdb = new KDBAccess();
                DataTable dtReg = kdb.GetHisRegByEnt((lField.ConfigName).ToString(), "", "iCodCatalogo = " + (lField.DataValue).ToString());
                pSubHistorico.iCodRegistro = (dtReg.Rows[0]["iCodRegistro"]).ToString();
                pSubHistorico.ConsultarRegistro();
            }
            pSubHistorico.Fields.EnableFields();
            pSubHistorico.SetHistoricState(HistoricState.Edicion);
        }

        protected virtual void SubHistorico_PostCancelarClick(object sender, EventArgs e)
        {
            RemoverSubHistorico();
        }

        protected virtual void SubHistorico_PostGrabarClick(object sender, EventArgs e)
        {
            FillAjaxControls();
            if (pSubHistorico.State != HistoricState.Edicion)
            {
                if (pSubHistorico.iCodCatalogo == "null")
                {
                    pFields.GetByConfigName(pSubHistorico.vchCodEntidad).DataValue = DBNull.Value;
                }
                else
                {
                    pFields.GetByConfigName(pSubHistorico.vchCodEntidad).DataValue = pSubHistorico.iCodCatalogo;
                }
                RemoverSubHistorico();
                if (State != HistoricState.Edicion)
                {
                    SetHistoricState(HistoricState.Edicion);
                    pFields.EnableFields();
                }
            }
        }

        public virtual void PostAgregarSubHistoricField()
        {

        }

        public virtual void PostEditarSubHistoricField()
        {

        }

        public virtual void PostEliminarSubHistoricField()
        {

        }

        #endregion

        #region IPostBackEventHandler Members

        public virtual void RaisePostBackEvent(string eventArgument)
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

        #region Exportar

        public virtual void ExportXLS()
        {
            DSOControls2008.DSOControl.LoadControlScriptBlock(Page, typeof(KeytiaPage), "HisExportDefaultXLS",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { jAlert('" + Globals.GetLangItem("ExpNoDisp") + "'); });" +
                "</script>\r\n",
                false, false);
        }

        public virtual void ExportDOC()
        {
            DSOControls2008.DSOControl.LoadControlScriptBlock(Page, typeof(KeytiaPage), "HisExportDefaultDOC",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { jAlert('" + Globals.GetLangItem("ExpNoDisp") + "'); });" +
                "</script>\r\n",
                false, false);
        }

        public virtual void ExportPDF()
        {
            DSOControls2008.DSOControl.LoadControlScriptBlock(Page, typeof(KeytiaPage), "HisExportDefaultPDF",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { jAlert('" + Globals.GetLangItem("ExpNoDisp") + "'); });" +
                "</script>\r\n",
                false, false);
        }

        public virtual void ExportCSV()
        {
            DSOControls2008.DSOControl.LoadControlScriptBlock(Page, typeof(KeytiaPage), "HisExportDefaultCSV",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { jAlert('" + Globals.GetLangItem("ExpNoDisp") + "'); });" +
                "</script>\r\n",
                false, false);
        }

        #endregion

        #region WebMethods

        public static string GetHisMaestros(string iCodCatalogo)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsJSON = "";
                DataTable ldt;
                ldt = DSODataAccess.Execute("select value = iCodRegistro, text = vchDescripcion from Maestros where dtIniVigencia <> dtFinVigencia and iCodEntidad = " + iCodCatalogo.Replace("'", "''"));
                lsJSON = DSOControl.SerializeJSON<DataTable>(ldt);
                return lsJSON;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static string SearchCatReg(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();
                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

                StringBuilder lsbQuery = new StringBuilder();
                string lsQuery = "select top (100) * from [VisHistoricos('currentlsEntidad','currentLanguage')]";
                //lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
                lsQuery = lsQuery.Replace("currentlsEntidad", lsEntidad);
                lsQuery = lsQuery.Replace("currentLanguage", lsLang);

                DataTable ldtVista = DSODataAccess.Execute(lsQuery + " where 1 = 2");

                lsbQuery.Append(lsQuery);
                lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\n");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia));

                string lsWhere = "\r\n  and (vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                string lsOrder = "\r\norder by vchDescripcion";
                if (ldtVista.Columns.Contains(lsLang))
                {
                    lsWhere = lsWhere + " or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                    lsOrder = "\r\norder by " + lsLang + ",vchDescripcion";
                }
                lsWhere = lsWhere + ")";

                lsbQuery.Append(lsWhere);
                lsbQuery.Append(lsOrder);

                lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.Append(")");
                lsbQuery.AppendLine(lsOrder);
                ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static string SearchDataSourceRep(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();
                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

                StringBuilder lsbQuery = new StringBuilder();
                string lsQuery = "select top (100) * from [VisHistoricos('currentlsEntidad','currentLanguage')]";
                //lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
                lsQuery = lsQuery.Replace("currentlsEntidad", lsEntidad);
                lsQuery = lsQuery.Replace("currentLanguage", lsLang);

                DataTable ldtVista = DSODataAccess.Execute(lsQuery + " where 1 = 2");

                lsbQuery.Append(lsQuery);
                lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\nand Usuar is null\r\n");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia));

                string lsWhere = "\r\n  and (vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                string lsOrder = "\r\norder by vchDescripcion";
                if (ldtVista.Columns.Contains(lsLang))
                {
                    lsWhere = lsWhere + " or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                    lsOrder = "\r\norder by " + lsLang + ",vchDescripcion";
                }
                lsWhere = lsWhere + ")";

                lsbQuery.Append(lsWhere);
                lsbQuery.Append(lsOrder);

                lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.Append(")");
                lsbQuery.AppendLine(lsOrder);
                ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static string SearchCatRestricted(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                //restrictedValues = Util.Decrypt(restrictedValues).Replace("'", "''");

                string lsLang = Globals.GetCurrentLanguage();
                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

                StringBuilder lsbQuery = new StringBuilder();
                string lsQuery = "select top (100) * from [VisHistoricos('currentlsEntidad','currentLanguage')]";
                //lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
                lsQuery = lsQuery.Replace("currentlsEntidad", lsEntidad);
                lsQuery = lsQuery.Replace("currentLanguage", lsLang);

                DataTable ldtVista = DSODataAccess.Execute(lsQuery + " where 1 = 2");

                lsbQuery.Append(lsQuery);
                lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\n");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia));

                string lsWhere = "\r\n  and (vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                string lsOrder = "\r\norder by vchDescripcion";
                if (ldtVista.Columns.Contains(lsLang))
                {
                    lsWhere = lsWhere + " or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                    lsOrder = "\r\norder by " + lsLang + ",vchDescripcion";
                }
                lsWhere = lsWhere + ")";

                lsbQuery.Append(lsWhere);

                lsbQuery.Append("\r\n  and iCodCatalogo in(");
                if (iniVigencia == null || iniVigencia.ToString() == "null")
                {
                    iniVigencia = DateTime.Today;
                }

                if (finVigencia == null || finVigencia.ToString() == "null")
                {
                    finVigencia = new DateTime(2079, 01, 01);
                }

                lsbQuery.Append("select distinct(iCodCatalogo) from ");
                //20170614 NZ Se cambia funcion
                //lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestriccionVigencia(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestricPorEntidad(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                lsbQuery.Append(")");
                lsbQuery.Append(lsOrder);

                lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.Append(")");
                lsbQuery.AppendLine(lsOrder);

                ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static string SearchCatFiltered(string term, int iCodEntidad, string keytiaFilter, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                int liDataSourceParam;
                DataTable ldtDataSource = null;
                if (int.TryParse(keytiaFilter, out liDataSourceParam))
                {
                    ldtDataSource = KeytiaAutoFilteredField.getDataSource(liDataSourceParam, term);
                }
                else
                {
                    return SearchCatReg(term, iCodEntidad, iniVigencia, finVigencia);
                }

                string json = DSOControl.SerializeJSON<DataTable>(ldtDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static DataTable FillDSOAutoDataSource(DataTable lKDBTable)
        {
            string lsLang = Globals.GetCurrentLanguage();

            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("id", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("value", typeof(string)));

            DataTable lDataSource = lDataTable.Clone();

            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                DataRow lDataRow = lDataTable.NewRow();
                lDataRow["id"] = lKDBRow["iCodCatalogo"];
                if (lKDBTable.Columns.Contains(lsLang) && lKDBRow[lsLang] != DBNull.Value)
                {
                    lDataRow["value"] = lKDBRow[lsLang] + " (" + lKDBRow["vchCodigo"] + ")";
                }
                else
                {
                    lDataRow["value"] = lKDBRow["vchDescripcion"] + " (" + lKDBRow["vchCodigo"] + ")";
                }
                lDataTable.Rows.Add(lDataRow);
            }

            DataView lViewData = lDataTable.DefaultView;
            lViewData.Sort = "value ASC, id ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                lDataSource.ImportRow(lViewRow.Row);
            }
            return lDataSource;
        }

        public static string SearchAttribute(string term, int iCodEntidad)
        {
            KDBAccess lKDB = new KDBAccess();

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = "{" + Globals.GetCurrentLanguage() + "}";
                string lsEntidad;
                DataTable lKDBTable;
                DataTable lDataTable = new DataTable();

                lsEntidad = "Atrib"; //(string)DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodRegistro = " + iCodEntidad);

                lDataTable.Columns.Add(new DataColumn("id", typeof(int)));
                lDataTable.Columns.Add(new DataColumn("value", typeof(string)));

                //Agrego las entidades
                lKDBTable = lKDB.GetHisRegByEnt("", "Entidades", new string[] { "iCodCatalogo", "vchDescripcion", lsLang }, "", "vchDescripcion," + lsLang, 100, "(vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%' or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%')");
                foreach (DataRow lKDBRow in lKDBTable.Rows)
                {
                    DataRow lDataRow = lDataTable.NewRow();
                    lDataRow["id"] = lKDBRow["iCodCatalogo"];
                    if (lKDBTable.Columns.Contains(lsLang) && lKDBRow[lsLang] != DBNull.Value)
                    {
                        lDataRow["value"] = lKDBRow[lsLang] + " (" + lKDBRow["vchCodigo"] + ")";
                    }
                    else
                    {
                        lDataRow["value"] = lKDBRow["vchDescripcion"] + " (" + lKDBRow["vchCodigo"] + ")";
                    }
                    lDataTable.Rows.Add(lDataRow);
                }

                //Agrego los atributos
                lKDBTable = lKDB.GetHisRegByEnt(lsEntidad, "", new string[] { "iCodCatalogo", "vchDescripcion", lsLang }, "", "vchDescripcion," + lsLang, 100, "(vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%' or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%')");
                foreach (DataRow lKDBRow in lKDBTable.Rows)
                {
                    DataRow lDataRow = lDataTable.NewRow();
                    lDataRow["id"] = lKDBRow["iCodCatalogo"];
                    if (lKDBTable.Columns.Contains(lsLang) && lKDBRow[lsLang] != DBNull.Value)
                    {
                        lDataRow["value"] = lKDBRow[lsLang] + " (" + lKDBRow["vchCodigo"] + ")";
                    }
                    else
                    {
                        lDataRow["value"] = lKDBRow["vchDescripcion"] + " (" + lKDBRow["vchCodigo"] + ")";
                    }
                    lDataTable.Rows.Add(lDataRow);
                }

                DataView lViewData = lDataTable.DefaultView;
                DataTable lDataSource = lDataTable.Clone();
                lViewData.Sort = "value ASC, id ASC";
                foreach (DataRowView lViewRow in lViewData)
                {
                    lDataSource.ImportRow(lViewRow.Row);
                }
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static string SearchEntity(string term)
        {
            KDBAccess lKDB = new KDBAccess();

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = "{" + Globals.GetCurrentLanguage() + "}";
                DataTable lKDBTable = lKDB.GetHisRegByEnt("", "Entidades", new string[] { "iCodCatalogo", "vchDescripcion", lsLang }, "", "vchDescripcion," + lsLang, 100, "(vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%' or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%')");
                DataTable lDataSource = FillDSOAutoDataSource(lKDBTable);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static DSOGridServerResponse GetMultiSelectData(DSOGridServerRequest gsRequest, int iCodEntidad, int bSelTodos, string jsonSeleccionados, int enableField, bool bRestriccion, object iniVigencia, object finVigencia)
        {
            string lvchCodEntidad = null;
            string lsTitulo = null;
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }
            try
            {
                lvchCodEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodRegistro = " + iCodEntidad).ToString();

                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;
                DataTable ldtSeleccionados = new DataTable();
                ldtSeleccionados.Columns.Add(new DataColumn("iCodCatalogo", typeof(int)));
                if (!String.IsNullOrEmpty(jsonSeleccionados))
                {
                    ldtSeleccionados = DSOControl.DeserializeDataTableJSON(jsonSeleccionados, ldtSeleccionados.Columns);
                }
                List<string> lstSeleccionados = new List<string>();
                foreach (DataRow ldataRow in ldtSeleccionados.Rows)
                {
                    lstSeleccionados.Add(ldataRow["iCodCatalogo"].ToString());
                }
                if (lstSeleccionados.Count == 0)
                {
                    lstSeleccionados.Add("0");
                }
                string lsSeleccionados = String.Join(",", lstSeleccionados.ToArray());
                int lbSeleccionado = 0;
                if (bSelTodos == 1 || enableField == 0)
                {
                    lbSeleccionado = 1;
                }

                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                string lsOrderCol = "vchDescripcion asc";
                string lsOrderColInv = "vchDescripcion desc";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
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

                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];
                    switch (lsOrderCol)
                    {
                        case "bSeleccionado":
                            lsOrderCol = "bSeleccionado" + lsOrderDirInv + ",vchDescripcion" + lsOrderDir;
                            lsOrderColInv = "bSeleccionado" + lsOrderDir + ",vchDescripcion" + lsOrderDirInv;
                            break;
                        default:
                            lsOrderCol = "vchDescripcion" + lsOrderDir;
                            lsOrderColInv = "vchDescripcion" + lsOrderDirInv;
                            break;
                    }
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("      select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                lsbColumnas.AppendLine("	        H.iCodCatalogo");
                lsbColumnas.AppendLine("	        ,bSeleccionado = case");
                lsbColumnas.AppendLine("	                            when " + lbSeleccionado + " = 1 or H.iCodCatalogo in(" + lsSeleccionados + ") then 1");
                lsbColumnas.AppendLine("	                            else 0");
                lsbColumnas.AppendLine("                            end");
                lsbColumnas.AppendLine("	        ,vchDescripcion = case");
                lsbColumnas.AppendLine("	            when M.VarChar01 = " + liCodIdioma + " then H.VarChar01");
                lsbColumnas.AppendLine("	            when M.VarChar02 = " + liCodIdioma + " then H.VarChar02");
                lsbColumnas.AppendLine("	            when M.VarChar03 = " + liCodIdioma + " then H.VarChar03");
                lsbColumnas.AppendLine("	            when M.VarChar04 = " + liCodIdioma + " then H.VarChar04");
                lsbColumnas.AppendLine("	            when M.VarChar05 = " + liCodIdioma + " then H.VarChar05");
                lsbColumnas.AppendLine("	            when M.VarChar06 = " + liCodIdioma + " then H.VarChar06");
                lsbColumnas.AppendLine("	            when M.VarChar07 = " + liCodIdioma + " then H.VarChar07");
                lsbColumnas.AppendLine("	            when M.VarChar08 = " + liCodIdioma + " then H.VarChar08");
                lsbColumnas.AppendLine("	            when M.VarChar09 = " + liCodIdioma + " then H.VarChar09");
                lsbColumnas.AppendLine("	            when M.VarChar10 = " + liCodIdioma + " then H.VarChar10");
                lsbColumnas.AppendLine("	            else H.vchDescripcion");
                lsbColumnas.AppendLine("	         end + ' (' + C.vchCodigo + ')'");
                lsbColumnas.AppendLine("            ,H.iCodRegistro");

                lsbFrom.AppendLine("      from Historicos H, Maestros M, Catalogos C");
                lsbFrom.AppendLine("      where H.iCodMaestro = M.iCodRegistro");
                lsbFrom.AppendLine("      and H.iCodCatalogo = C.iCodRegistro");
                lsbFrom.AppendLine("      and C.iCodCatalogo = " + iCodEntidad);
                lsbFrom.AppendLine("      and M.dtIniVigencia <> M.dtFinVigencia");

                string lsRestriccion = "";
                if (enableField == 0)
                {
                    lsbFrom.AppendLine("      and H.iCodCatalogo in(" + lsSeleccionados + ")");
                }
                else if (bRestriccion)
                {
                    //20170614 NZ Se cambia funcion
                    //DataTable ldtRestriccion = DSODataAccess.Execute("select distinct iCodCatalogo from " + DSODataContext.Schema + ".GetRestriccionVigencia(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lvchCodEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                    DataTable ldtRestriccion = DSODataAccess.Execute("select distinct iCodCatalogo from " + DSODataContext.Schema + ".GetRestricPorEntidad(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lvchCodEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                    List<string> lstValues = new List<string>();
                    foreach (DataRow ldataRow in ldtRestriccion.Rows)
                    {
                        lstValues.Add(ldataRow["iCodCatalogo"].ToString());
                    }
                    if (lstValues.Count == 0)
                    {
                        lstValues.Add("0");
                    }
                    lsRestriccion = String.Join(",", lstValues.ToArray());

                    if (iniVigencia == null || iniVigencia.ToString() == "null")
                    {
                        iniVigencia = DateTime.Today;
                    }

                    if (finVigencia == null || finVigencia.ToString() == "null")
                    {
                        finVigencia = new DateTime(2079, 01, 01);
                    }
                    lsbFrom.AppendLine("      and (H.iCodCatalogo in(" + lsSeleccionados + ") or H.iCodCatalogo in(" + lsRestriccion + "))");
                }
                lsbFrom.AppendLine("      and H.iCodRegistro = ((select Max(His.iCodRegistro) from Historicos His");
                lsbFrom.AppendLine("            where His.iCodCatalogo = H.iCodCatalogo");
                if (enableField != 0 && bRestriccion)
                {
                    lsbFrom.AppendLine("        and (His.iCodCatalogo in(" + lsSeleccionados + ") or His.iCodCatalogo in(" + lsRestriccion + "))");
                }
                lsbFrom.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbFrom.AppendLine("))");

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + ", H.iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as H order by " + lsOrderColInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as H order by " + lsOrderCol + ", iCodRegistro" + lsOrderDir);


                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    lsbWhere.AppendLine("and case");
                    lsbWhere.AppendLine("   when M.VarChar01 = " + liCodIdioma + " then H.VarChar01");
                    lsbWhere.AppendLine("   when M.VarChar02 = " + liCodIdioma + " then H.VarChar02");
                    lsbWhere.AppendLine("   when M.VarChar03 = " + liCodIdioma + " then H.VarChar03");
                    lsbWhere.AppendLine("   when M.VarChar04 = " + liCodIdioma + " then H.VarChar04");
                    lsbWhere.AppendLine("   when M.VarChar05 = " + liCodIdioma + " then H.VarChar05");
                    lsbWhere.AppendLine("   when M.VarChar06 = " + liCodIdioma + " then H.VarChar06");
                    lsbWhere.AppendLine("   when M.VarChar07 = " + liCodIdioma + " then H.VarChar07");
                    lsbWhere.AppendLine("   when M.VarChar08 = " + liCodIdioma + " then H.VarChar08");
                    lsbWhere.AppendLine("   when M.VarChar09 = " + liCodIdioma + " then H.VarChar09");
                    lsbWhere.AppendLine("   when M.VarChar10 = " + liCodIdioma + " then H.VarChar10");
                    lsbWhere.AppendLine("   else H.vchDescripcion");
                    lsbWhere.AppendLine("end + ' (' + C.vchCodigo + ')'  like '%" + lsTerm + "%'");
                }

                string lsSelectCount = "select count(H.iCodRegistro) ";
                string lsSelectAll = "select distinct H.iCodCatalogo ";
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

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    lsTitulo = Globals.GetLangItem("", "Entidades", lvchCodEntidad);
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                if (bSelTodos == 1 && enableField == 1)
                {
                    ldtSeleccionados = DSODataAccess.Execute(lsSelectAll + lsFrom);
                    lgsrRet.sDSOTag = DSOControl.SerializeJSON<DataTable>(ldtSeleccionados);
                }
                else if (ldtSeleccionados.Rows.Count > 0 && enableField == 1)
                {
                    foreach (DataRow ldataRow in ldt.Rows)
                    {
                        if (ldtSeleccionados.Select("iCodCatalogo = " + ldataRow["iCodCatalogo"]).Length > 0)
                        {
                            ldataRow["bSeleccionado"] = 1;
                        }
                    }
                }

                lgsrRet.SetDataFromDataTable(ldt, lsSqlDateFormat);
                return lgsrRet;
            }
            catch (Exception e)
            {
                lsTitulo = Globals.GetLangItem("", "Entidades", lvchCodEntidad);
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetRelData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodRelacion, int iCodCatalogo, string jsonData, string vchDescripcion)
        {
            KDBAccess pKDB = new KDBAccess();

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                RelationFieldCollection lFields = new RelationFieldCollection(iCodEntidad, iCodRelacion);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                string lsColEntidad = "";
                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if (!lFields.Contains(lsOrderCol.Replace("Display", "")))
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
                lsbSelectTop.AppendLine("      select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lgsrRet.sColumns = "iCodRegistro";
                lsbColumnas.AppendLine("iCodRegistro");
                //Columnas para guardar los valores
                foreach (KeytiaBaseField lField in lFields)
                {
                    lgsrRet.sColumns += "," + lField.Column;
                    lsbColumnas.AppendLine("," + lField.Column);
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && lField.ConfigValue == iCodEntidad)
                    {
                        lsColEntidad = lField.Column;
                    }
                }
                lgsrRet.sColumns += ",dtFecUltAct";
                lsbColumnas.AppendLine(",dtFecUltAct");

                //Columnas para mostrar los valores
                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lgsrRet.sColumns += "," + lField.Column + "Display";
                        lsbColumnas.AppendLine("," + lField.Column + "Display = " + DSODataContext.Schema + ".GetCatDesc(a." + lField.Column + "," + liCodIdioma + ",a.dtIniVigencia)");
                    }
                    else if (lField.Column.EndsWith("Vigencia"))
                    {
                        lgsrRet.sColumns += "," + lField.Column + "Display";
                        lsbColumnas.AppendLine("," + lField.Column + "Display = " + lField.Column);
                    }
                }
                lgsrRet.sColumns += ",Editar";
                lsbColumnas.AppendLine(",Editar = 1");

                lgsrRet.sColumns += ",Eliminar";
                lsbColumnas.AppendLine(",Eliminar = 1");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from Relaciones a");

                lsbWhere.AppendLine("      where iCodRelacion = " + iCodRelacion);
                lsbWhere.AppendLine("      and a.dtIniVigencia <> a.dtFinVigencia");
                lsbWhere.AppendLine("      and " + lsColEntidad + " = " + iCodCatalogo);

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir);

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + lsWhere);
                lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFrom + lsWhere + lsOrderBy);

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

                DataTable lData = new DataTable();
                if (!String.IsNullOrEmpty(jsonData))
                {
                    lData = DSOControl.DeserializeJSON<DataTable>(jsonData);
                }
                foreach (DataRow lDataRow in lData.Rows)
                {
                    DataRow ldr;
                    if (ldt.Select("iCodRegistro = " + lDataRow["iCodRegistro"]).Length == 0)
                    {
                        ldr = ldt.NewRow();
                        ldt.Rows.Add(ldr);
                    }
                    else
                    {
                        ldr = ldt.Select("iCodRegistro = " + lDataRow["iCodRegistro"])[0];
                    }
                    foreach (DataColumn lDataCol in lData.Columns)
                    {
                        if (lDataCol.ColumnName != "Editar"
                            && lDataCol.ColumnName != "Eliminar"
                            && ldt.Columns.Contains(lDataCol.ColumnName))
                        {
                            ldr[lDataCol.ColumnName] = lDataRow[lDataCol];
                        }
                    }
                    ldr["Editar"] = lDataRow["Editar"];
                    ldr["Eliminar"] = lDataRow["Eliminar"];
                }
                KeytiaBaseField lFieldEntidad = lFields.GetByConfigValue(iCodEntidad);
                foreach (DataRow ldr in ldt.Rows)
                {
                    ldr[lFieldEntidad.Column + "Display"] = vchDescripcion;
                }

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                lgsrRet.SetDataFromDataTable(ldt, lsDateFormat, "dtIniVigenciaDisplay", "dtFinVigenciaDisplay");
                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetHisData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
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
                DateTime ldtVigencia = DateTime.Now;

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
                lsbFrom.AppendLine("      where iCodMaestro = " + iCodMaestro);
                lsbFrom.AppendLine("      and iCodCatalogo in(select iCodRegistro from Catalogos where iCodCatalogo = " + iCodEntidad + ")");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' < dtFinVigencia");

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
                string lsFromGetHisData = "from " + DSODataContext.Schema + ".GetHisData(" + iCodEntidad + "," + iCodMaestro + "," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "') a \r\n";
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

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lgsrRet.ProcesarDatos(gsRequest, ldt);


                lFields.FormatGridData(lgsrRet, ldt);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetHisDataEmple(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
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
                DateTime ldtVigencia = DateTime.Now;

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
                lsbFrom.AppendLine("      where iCodMaestro = " + iCodMaestro);
                lsbFrom.AppendLine("      and iCodCatalogo in(select iCodRegistro from Catalogos where iCodCatalogo = " + iCodEntidad + ")");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' < dtFinVigencia");

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                //                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                //                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a ");
                lsbOrderBy.AppendLine(") as a ");

                bool lbPrimero = true;

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(a.vchCodigo,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(a.vchDescripcion,'')");

                    //lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlTimeFormat + "),'')");
                    //lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlTimeFormat + "),'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (lField.Column.StartsWith("VarChar") )
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
                string lsFromGetHisData = "from " + DSODataContext.Schema + ".GetHisDataEmple(" + iCodEntidad + "," + iCodMaestro + "," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "') a \r\n";
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

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lgsrRet.ProcesarDatos(gsRequest, ldt);


                lFields.FormatGridData(lgsrRet, ldt);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetHisRestData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                int liCodUsuario = (int)HttpContext.Current.Session["iCodUsuario"];
                int liCodPerfil = (int)HttpContext.Current.Session["iCodPerfil"];

                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodEntidad, iCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

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
                lsbColumnas.AppendLine(",iCodCatalogo");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from Historicos a");
                lsbFrom.AppendLine("      where iCodMaestro = " + iCodMaestro);
                lsbFrom.AppendLine("      and iCodCatalogo in(select iCodRegistro from Catalogos where iCodCatalogo = " + iCodEntidad + ")");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' < dtFinVigencia");
                //20170614 NZ Se cambia funcion
                //lsbFrom.AppendLine("      and iCodCatalogo in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestriccionVigencia(" + liCodUsuario + "," + liCodPerfil + ",'" + lsEntidad.Replace("'", "''") + "','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");
                lsbFrom.AppendLine("      and iCodCatalogo in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestricPorEntidad(" + liCodUsuario + "," + liCodPerfil + ",'" + lsEntidad.Replace("'", "''") + "','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");

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

                if (lsbWhere.Length == 0)
                {
                    //20170614 NZ Se cambia funcion
                    //lsbWhere.AppendLine("where iCodCatalogo in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestriccionVigencia(" + liCodUsuario + "," + liCodPerfil + ",'" + lsEntidad.Replace("'", "''") + "','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");
                    lsbWhere.AppendLine("where iCodCatalogo in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestricPorEntidad(" + liCodUsuario + "," + liCodPerfil + ",'" + lsEntidad.Replace("'", "''") + "','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");
                }
                else
                {
                    //20170614 NZ Se cambia funcion
                    //lsbWhere.AppendLine("and iCodCatalogo in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestriccionVigencia(" + liCodUsuario + "," + liCodPerfil + ",'" + lsEntidad.Replace("'", "''") + "','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");
                    lsbWhere.AppendLine("and iCodCatalogo in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestricPorEntidad(" + liCodUsuario + "," + liCodPerfil + ",'" + lsEntidad.Replace("'", "''") + "','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");
                }

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsFromGetHisData = "from " + DSODataContext.Schema + ".GetHisData(" + iCodEntidad + "," + iCodMaestro + "," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "') a \r\n";
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

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lgsrRet.ProcesarDatos(gsRequest, ldt);


                lFields.FormatGridData(lgsrRet, ldt);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static DSOGridServerResponse GetVisHistorico(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            return HistoricEdit.GetVisHistorico(gsRequest, iCodEntidad, iCodMaestro, null, null);
        }

        public static DSOGridServerResponse GetVisHistorico(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, object ldtVigencia)
        {
            return HistoricEdit.GetVisHistorico(gsRequest, iCodEntidad, iCodMaestro, ldtVigencia, null);
        }

        public static DSOGridServerResponse GetVisHistorico(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, object ldtVigencia, List<Parametro> parametros)
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

                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();
                string lsMaestro = DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + iCodMaestro).ToString();

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

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
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbColumnas.AppendLine(",[" + lField.ConfigName + "Desc]");
                    }
                    else
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
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "' >= dtIniVigencia");
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "' < dtFinVigencia");
                }

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (parametros != null)
                {
                    foreach (Parametro lParam in parametros)
                    {
                        if (lParam.Value == null)
                        {
                            lParam.Value = "null";
                        }
                        string lsFiltro = lParam.Value.ToString().Replace("'", "''").Trim();
                        string lsColumnaVista = null;
                        string lsColumnaTabla = null;

                        if (lFields.Contains(lParam.Name))
                        {
                            lsColumnaTabla = lParam.Name;
                            lsColumnaVista = lFields[lParam.Name].ConfigName;
                        }
                        else if (lFields.ContainsConfigName(lParam.Name))
                        {
                            lsColumnaTabla = lFields.GetByConfigName(lParam.Name).Column;
                            lsColumnaVista = lParam.Name;
                        }
                        else if (lParam.Name == "vchCodigo"
                            || lParam.Name == "vchDescripcion"
                            || lParam.Name == "dtIniVigencia"
                            || lParam.Name == "dtFinVigencia"
                            || lParam.Name == "iCodRegistro"
                            || lParam.Name == "NotiCodRegistro"
                            || lParam.Name == "iCodCatalogo"
                            || lParam.Name == "NotiCodCatalogo")
                        {
                            lsColumnaTabla = lParam.Name;
                            lsColumnaVista = lParam.Name;
                        }

                        if (lsColumnaVista != null)
                        {
                            lsbFrom.Append("and ");

                            if (lsColumnaVista == "iCodRegistro")
                            {
                                lsbFrom.AppendLine("a.iCodRegistro in(" + lsFiltro + ")");
                            }
                            else if (lsColumnaVista == "NotiCodRegistro")
                            {
                                lsbFrom.AppendLine("a.iCodRegistro not in(" + lsFiltro + ")");
                            }
                            else if (lsColumnaVista == "iCodCatalogo")
                            {
                                lsbFrom.AppendLine("a.iCodCatalogo in(" + lsFiltro + ")");
                            }
                            else if (lsColumnaVista == "NotiCodCatalogo")
                            {
                                lsbFrom.AppendLine("a.iCodCatalogo not in(" + lsFiltro + ")");
                            }
                            else if (lsColumnaTabla.StartsWith("Date")
                                || lsColumnaTabla == "dtIniVigencia"
                                || lsColumnaTabla == "dtFinVigencia")
                            {
                                if (lParam.Value is DateTime)
                                {
                                    lsbFrom.AppendLine("a." + lsColumnaVista + " = '" + ((DateTime)lParam.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                }
                                else
                                {
                                    lsbFrom.Append("(");
                                    lsbFrom.AppendLine("convert(varchar, a." + lsColumnaVista + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                    lsbFrom.AppendLine("or convert(varchar, a." + lsColumnaVista + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                    lsbFrom.AppendLine(")");
                                }
                            }
                            else if (lsColumnaTabla.StartsWith("VarChar"))
                            {
                                lsbFrom.AppendLine("a." + lsColumnaVista + " = '" + lsFiltro + "'");
                            }
                            else if (lsColumnaTabla == "vchCodigo")
                            {
                                lsbFrom.AppendLine("C." + lsColumnaVista + " = '" + lsFiltro + "'");
                            }
                            else if (lsColumnaTabla == "vchDescripcion")
                            {
                                lsbFrom.AppendLine("a." + lsColumnaVista + " = '" + lsFiltro + "'");
                            }
                            else if (lsFiltro == "is null")
                            {
                                lsbFrom.AppendLine("a." + lsColumnaVista + " is null");
                            }
                            else if (lsColumnaTabla.StartsWith("iCodCatalogo"))
                            {
                                lsbFrom.AppendLine("a." + lsColumnaVista + " in(" + lsFiltro + ")");
                            }
                            else
                            {
                                lsbFrom.AppendLine("a." + lsColumnaVista + " = " + lsFiltro);
                            }
                        }
                    }
                }

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

        public static DSOGridServerResponse GetSubHisData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return HistoricEdit.GetSubHisData(gsRequest, iCodEntidad, iCodMaestro, DateTime.Now, false, parametros);
        }

        public static DSOGridServerResponse GetSubHisData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, object ldtVigencia, List<Parametro> parametros)
        {
            return HistoricEdit.GetSubHisData(gsRequest, iCodEntidad, iCodMaestro, ldtVigencia, true, parametros);
        }

        public static DSOGridServerResponse GetSubHisData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, object ldtVigencia, bool lbToLocalTime, List<Parametro> parametros)
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

                lsbColumnas.AppendLine("a.iCodRegistro");
                lsbColumnas.AppendLine(",Editar = null");
                lsbColumnas.AppendLine(",Eliminar = null");
                lsbColumnas.AppendLine(",C.vchCodigo");
                lsbColumnas.AppendLine(",a.vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    lsbColumnas.AppendLine(",a." + lField.Column);
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbColumnas.AppendLine("," + lField.Column + "Desc = " + DSODataContext.Schema + ".GetCatDesc(a." + lField.Column + ", " + liCodIdioma + " , a.dtIniVigencia)");
                    }
                }

                lsbColumnas.AppendLine(",a.dtIniVigencia");
                lsbColumnas.AppendLine(",a.dtFinVigencia");
                lsbColumnas.AppendLine(",a.dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from Historicos a, Catalogos C");
                lsbFrom.AppendLine("      where a.iCodMaestro = " + iCodMaestro);
                lsbFrom.AppendLine("      and a.iCodCatalogo = C.iCodRegistro");
                lsbFrom.AppendLine("      and a.iCodCatalogo in(select E.iCodRegistro from Catalogos E where E.iCodCatalogo = " + iCodEntidad + ")");
                lsbFrom.AppendLine("      and a.dtIniVigencia <> a.dtFinVigencia");
                if (ldtVigencia != null)
                {
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, lbToLocalTime).ToString("yyyy-MM-dd HH:mm:ss") + "' >= a.dtIniVigencia");
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, lbToLocalTime).ToString("yyyy-MM-dd HH:mm:ss") + "' < a.dtFinVigencia");
                }

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (parametros != null)
                {
                    foreach (Parametro lParam in parametros)
                    {
                        if (lParam.Value == null)
                        {
                            lParam.Value = "null";
                        }
                        string lsFiltro = lParam.Value.ToString().Replace("'", "''").Trim();
                        string lsColumna = null;
                        if (lFields.Contains(lParam.Name))
                        {
                            lsColumna = lParam.Name;
                        }
                        else if (lFields.ContainsConfigName(lParam.Name))
                        {
                            lsColumna = lFields.GetByConfigName(lParam.Name).Column;
                        }
                        else if (lParam.Name == "vchCodigo"
                            || lParam.Name == "vchDescripcion"
                            || lParam.Name == "dtIniVigencia"
                            || lParam.Name == "dtFinVigencia"
                            || lParam.Name == "iCodRegistro"
                            || lParam.Name == "NotiCodRegistro"
                            || lParam.Name == "iCodCatalogo"
                            || lParam.Name == "NotiCodCatalogo")
                        {
                            lsColumna = lParam.Name;
                        }

                        if (lsColumna != null)
                        {
                            lsbFrom.Append("and ");

                            if (lsColumna == "iCodRegistro")
                            {
                                lsbFrom.AppendLine("a.iCodRegistro in(" + lsFiltro + ")");
                            }
                            else if (lsColumna == "NotiCodRegistro")
                            {
                                lsbFrom.AppendLine("a.iCodRegistro not in(" + lsFiltro + ")");
                            }
                            else if (lsColumna == "iCodCatalogo")
                            {
                                lsbFrom.AppendLine("a.iCodCatalogo in(" + lsFiltro + ")");
                            }
                            else if (lsColumna == "NotiCodCatalogo")
                            {
                                lsbFrom.AppendLine("a.iCodCatalogo not in(" + lsFiltro + ")");
                            }
                            else if (lsColumna.StartsWith("Date")
                                || lsColumna == "dtIniVigencia"
                                || lsColumna == "dtFinVigencia")
                            {
                                if (lParam.Value is DateTime)
                                {
                                    lsbFrom.AppendLine("a." + lsColumna + " = '" + ((DateTime)lParam.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                }
                                else
                                {
                                    lsbFrom.Append("(");
                                    lsbFrom.AppendLine("convert(varchar, a." + lsColumna + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                    lsbFrom.AppendLine("or convert(varchar, a." + lsColumna + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                    lsbFrom.AppendLine(")");
                                }
                            }
                            else if (lsColumna.StartsWith("VarChar"))
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " = '" + lsFiltro + "'");
                            }
                            else if (lsColumna == "vchCodigo")
                            {
                                lsbFrom.AppendLine("C." + lsColumna + " = '" + lsFiltro + "'");
                            }
                            else if (lParam.Name == "vchDescripcion")
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " = '" + lsFiltro + "'");
                            }
                            else if (lsColumna.StartsWith("iCodCatalogo"))
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " in(" + lsFiltro + ")");
                            }
                            else
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " = " + lsFiltro);
                            }
                        }
                    }
                }

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(C.vchCodigo,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(a.vchDescripcion,'')");

                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlTimeFormat + "),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlTimeFormat + "),'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (lField.Column.StartsWith("Date"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlTimeFormat + "),'')");
                        }
                        else if (!lField.Column.StartsWith("VarChar")
                            && !lField.Column.StartsWith("iCodCatalogo"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + lField.Column + "),'')");
                        }
                        else if (lField.Column.StartsWith("iCodCatalogo"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + DSODataContext.Schema + ".GetCatDesc(a." + lField.Column + ", " + liCodIdioma + " , a.dtFecUltAct)" + "),'')");
                        }
                        else
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(a." + lField.Column + ",'')");
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
                            lsbWhere.Append("and ");

                            if (lsColumn.StartsWith("Date")
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a." + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a." + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else if (lsColumn.StartsWith("iCodCatalogo") && lsColumn.EndsWith("Desc"))
                            {
                                lsbWhere.AppendLine(DSODataContext.Schema + ".GetCatDesc(a." + lsColumn + ", " + liCodIdioma + " , a.dtFecUltAct) like '%" + lsFiltro + "%'");
                            }
                            else if (lsColumn == "vchCodigo")
                            {
                                lsbWhere.AppendLine("C." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                if (lsOrderCol == "vchCodigo")
                {
                    lsbOrderBy.AppendLine("       order by C." + lsOrderCol + lsOrderDir + ", a.iCodRegistro" + lsOrderDir);
                }
                else
                {
                    lsbOrderBy.AppendLine("       order by a." + lsOrderCol + lsOrderDir + ", a.iCodRegistro" + lsOrderDir);
                }
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);


                string lsSelectCount = "select count(a.iCodRegistro) ";
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
