using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSOControls2008;
using System.Web.SessionState;
using KeytiaServiceBL;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text;
using System.Reflection;
using System.Web.UI;
using System.Collections;

namespace KeytiaWeb.UserInterface
{
    public enum BusquedaState
    {
        Default,
        SubReporte,
        SubHistorico
    }

    public class DSOBusqueda : Panel, INamingContainer, IPostBackEventHandler
    {
        protected Panel pnlBusquedas;
        protected Panel pnlConsultas;
        protected Table Table1;
        protected Table Table2;
        protected Panel pnlToolBar;
        protected DSOTextBox Search;
        protected HtmlButton btnConsultar;
        protected DSOExpandable pnlRecursos;
        protected DSOExpandable pnlConsumos;
        protected DSOExpandable pnlEmpleados;
        protected DSOGrid pGridRecursos;
        protected DSOGrid pGridConsumos;
        protected DSOGrid pGridEmpleados;
        protected List<string> plstCamposEmpleados;
        protected List<string> plstCamposRecursos = new List<string>();
        protected List<string> plstCamposConsumos = new List<string>();
        protected bool pbPermisoEmpleados;
        protected bool pbPermisoRecursos;
        protected bool pbPermisoConsumos;
        protected int piCodPerfil;
        protected string psID;
        protected HtmlInputHidden phSessionID;
        protected bool pbControlsCreated = false;
        protected Control pParentContainer;
        protected Label plblTitle;

        protected HistoricEdit pSubHistorico;
        protected ReporteEstandar pSubReporte;
        protected KDBAccess pKDB = new KDBAccess();

        public BusquedaState State
        {
            get
            {
                if (ViewState["BusquedaState"] == null)
                {
                    ViewState["BusquedaState"] = BusquedaState.Default;
                }
                return (BusquedaState)ViewState["BusquedaState"];
            }
            set
            {
                ViewState["BusquedaState"] = value;
            }
        }

        public int iCodSubConsulta
        {
            get
            {
                if (ViewState["iCodSubConsulta"] == null)
                {
                    ViewState["iCodSubConsulta"] = 0;
                }
                return (int)ViewState["iCodSubConsulta"];
            }
            set
            {
                ViewState["iCodSubConsulta"] = value;
            }
        }

        public int iCodSubReporte
        {
            get
            {
                if (ViewState["iCodSubReporte"] == null)
                {
                    ViewState["iCodSubReporte"] = 0;
                }
                return (int)ViewState["iCodSubReporte"];
            }
            set
            {
                ViewState["iCodSubReporte"] = value;
            }
        }

        public int iSubEstadoConsulta
        {
            get
            {
                if (ViewState["iSubEstadoConsulta"] == null)
                {
                    ViewState["iSubEstadoConsulta"] = 0;
                }
                return (int)ViewState["iSubEstadoConsulta"];
            }
            set
            {
                ViewState["iSubEstadoConsulta"] = value;
            }
        }

        public int iCodSubAplicacion
        {
            get
            {
                if (ViewState["iCodSubAplicacion"] == null)
                {
                    ViewState["iCodSubAplicacion"] = 0;
                }
                return (int)ViewState["iCodSubAplicacion"];
            }
            set
            {
                ViewState["iCodSubAplicacion"] = value;
            }
        }

        public int iNumSubReporte
        {
            get
            {
                if (ViewState["iNumSubReporte"] == null)
                {
                    ViewState["iNumSubReporte"] = 0;
                }
                return (int)ViewState["iNumSubReporte"];
            }
            set
            {
                ViewState["iNumSubReporte"] = value;
            }
        }

        public int iCodRutaConsulta
        {
            get
            {
                if (ViewState["iCodRutaConsulta"] == null)
                {
                    ViewState["iCodRutaConsulta"] = 0;
                }
                return (int)ViewState["iCodRutaConsulta"];
            }
            set
            {
                ViewState["iCodRutaConsulta"] = value;
            }
        }

        public int iCodSubRutaConsulta
        {
            get
            {
                if (ViewState["iCodSubRutaConsulta"] == null)
                {
                    ViewState["iCodSubRutaConsulta"] = 0;
                }
                return (int)ViewState["iCodSubRutaConsulta"];
            }
            set
            {
                ViewState["iCodSubRutaConsulta"] = value;
            }
        }

        public string OpcMnu
        {
            get
            {
                if (ViewState["OpcMnu"] == null)
                {
                    ViewState["OpcMnu"] = "";
                }
                return ViewState["OpcMnu"].ToString();
            }
            set
            {
                ViewState["OpcMnu"] = value;
            }
        }

        protected string Titulo
        {
            get
            {
                if (ViewState["Titulo"] == null)
                {
                    ViewState["Titulo"] = "";
                }
                return ViewState["Titulo"].ToString();
            }
            set
            {
                ViewState["Titulo"] = value;
            }
        }

        public Control ParentContainer
        {
            get
            {
                return pParentContainer;
            }
            set
            {
                pParentContainer = value;
            }
        }

        public Label lblTitle
        {
            get { return plblTitle; }
            set { plblTitle = value; }
        }

        protected string Entidad
        {
            get
            {
                return ViewState["Entidad"] == null ? "" : ViewState["Entidad"].ToString();
            }
            set
            {
                ViewState["Entidad"] = value;
            }
        }

        protected string Maestro
        {
            get
            {
                return ViewState["Maestro"] == null ? "" : ViewState["Maestro"].ToString();
            }
            set
            {
                ViewState["Maestro"] = value;
            }
        }

        protected string Registro
        {
            get
            {
                return ViewState["Registro"] == null ? "" : ViewState["Registro"].ToString();
            }
            set
            {
                ViewState["Registro"] = value;
            }
        }

        public DSOBusqueda()
        {
            psID = Guid.NewGuid().ToString();
        }

        public void SetSearch()
        {
            if (!this.ParentContainer.Page.IsPostBack && this.ParentContainer.Page.Request.Params["Search"] != null)
            {
                Search.TextBox.Text = this.ParentContainer.Page.Request.Params["Search"];
            }
            FillGrids();

        }

        public void CreateControls()
        {
            try
            {
                if (pbControlsCreated)
                {
                    return;
                }

                InitBusquedas();
                InitSubReporte();
                InitSubHistorico();
                SetBusquedaState(State);

                pbControlsCreated = true;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrInitControls", e);
            }
        }

        protected void InitBusquedas()
        {
            pnlBusquedas = new Panel();
            pnlConsultas = new Panel();
            Table1 = new Table();
            Table2 = new Table();
            pnlToolBar = new Panel();
            Search = new DSOTextBox();
            btnConsultar = new HtmlButton();
            phSessionID = new HtmlInputHidden();

            phSessionID.ID = "SessionID";
            phSessionID.Value = psID;
            ParentContainer.Controls.Add(phSessionID);

            pnlToolBar.CssClass = "pnlToolBar fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";

            Search.ID = "Search";
            Search.CreateControls();
            Search.TextBox.AutoPostBack = false;

            btnConsultar.ID = "btnConsultar";
            btnConsultar.Attributes["class"] = "buttonSearchImg";
            btnConsultar.Attributes.Add(HtmlTextWriterAttribute.Onclick.ToString(),
                "Busqueda.fnSeachText(); return false;");

            piCodPerfil = (int)HttpContext.Current.Session["iCodPerfil"];
            pbPermisoRecursos = getPermiso(BusquedaGenerica.Seccion.SecRecurs, Permiso.Consultar);
            pbPermisoConsumos = getPermiso(BusquedaGenerica.Seccion.SecConsumos, Permiso.Consultar);
            pbPermisoEmpleados = getPermiso(BusquedaGenerica.Seccion.SecEmple, Permiso.Consultar);

            if (pbPermisoRecursos)
            {
                pnlRecursos = new DSOExpandable();
                pGridRecursos = new DSOGrid();
                plstCamposRecursos = new List<string>();
                plstCamposRecursos.Add("Recurs");
                plstCamposRecursos.Add("Descripcion");
                plstCamposRecursos.Add("Sitio");
                plstCamposRecursos.Add("Emple");
                plstCamposRecursos.Add("Responsable");
                plstCamposRecursos.Add("EmpleResp");
                plstCamposRecursos.Add("Email");
                plstCamposRecursos.Add("CenCos");

            }

            if (pbPermisoConsumos)
            {
                pnlConsumos = new DSOExpandable();
                pGridConsumos = new DSOGrid();
                plstCamposConsumos = new List<string>();
                plstCamposConsumos.Add("Entidad");
                plstCamposConsumos.Add("Descripcion");
                plstCamposConsumos.Add("Costo");
                plstCamposConsumos.Add("CostoFac");
                plstCamposConsumos.Add("Duracion");
                plstCamposConsumos.Add("Llamadas");
            }

            if (pbPermisoEmpleados)
            {
                pnlEmpleados = new DSOExpandable();
                pGridEmpleados = new DSOGrid();
                plstCamposEmpleados = new List<string>();
                plstCamposEmpleados.Add("Nomina");
                plstCamposEmpleados.Add("Descripcion");
                plstCamposEmpleados.Add("Email");
                plstCamposEmpleados.Add("Recurs");
                plstCamposEmpleados.Add("Sitio");
                plstCamposEmpleados.Add("CenCos");
                plstCamposEmpleados.Add("Ubica");
            }

            CreatePanels();
            CreateGrids();

            TableRow tr;
            TableCell tc;

            //20140812 AM. Se hace cambio para que se agregue en la pagina el grid de consumos, despues el de recursos y al final empleados.
            //Antes era empleados, recursos, consumos
            #region Antes de cambio

            //if (pbPermisoEmpleados)
            //{
            //    pnlEmpleados.Panel.Controls.Add(pGridEmpleados);

            //    tr = new TableRow();
            //    tc = new TableCell();
            //    tc.Controls.Add(pnlEmpleados);
            //    tr.Cells.Add(tc);
            //    Table2.Rows.Add(tr);
            //}

            //if (pbPermisoRecursos)
            //{
            //    pnlRecursos.Panel.Controls.Add(pGridRecursos);

            //    tr = new TableRow();
            //    tc = new TableCell();
            //    tc.Controls.Add(pnlRecursos);
            //    tr.Cells.Add(tc);
            //    Table2.Rows.Add(tr);
            //}

            //if (pbPermisoConsumos)
            //{
            //    pnlConsumos.Panel.Controls.Add(pGridConsumos);

            //    tr = new TableRow();
            //    tc = new TableCell();
            //    tc.Controls.Add(pnlConsumos);
            //    tr.Cells.Add(tc);
            //    Table2.Rows.Add(tr);
            //}

            #endregion Antes de cambio

            if (pbPermisoConsumos)
            {
                pnlConsumos.Panel.Controls.Add(pGridConsumos);

                tr = new TableRow();
                tc = new TableCell();
                tc.Controls.Add(pnlConsumos);
                tr.Cells.Add(tc);
                Table2.Rows.Add(tr);
            }

            if (pbPermisoRecursos)
            {
                pnlRecursos.Panel.Controls.Add(pGridRecursos);

                tr = new TableRow();
                tc = new TableCell();
                tc.Controls.Add(pnlRecursos);
                tr.Cells.Add(tc);
                Table2.Rows.Add(tr);
            }

            if (pbPermisoEmpleados)
            {
                pnlEmpleados.Panel.Controls.Add(pGridEmpleados);

                tr = new TableRow();
                tc = new TableCell();
                tc.Controls.Add(pnlEmpleados);
                tr.Cells.Add(tc);
                Table2.Rows.Add(tr);
            }



            Table1.Controls.Add(new TableRow());
            Table1.Rows[0].Controls.Add(new TableCell());
            Table1.Rows[0].Controls.Add(new TableCell());

            Table1.Style.Add("Width", "60%");
            Table1.Style.Add("float", "left");
            Table2.Style.Add("Width", "100%");

            Search.TextBox.Style.Add("Width", "100%");
            Table1.Rows[0].Cells[1].Style.Add("Width", "100%");
            Table1.Rows[0].Cells[0].Controls.Add(btnConsultar);
            Table1.Rows[0].Cells[1].Controls.Add(Search);

            pnlToolBar.Controls.Add(Table1);
            pnlBusquedas.Controls.Add(pnlToolBar);
            pnlBusquedas.Controls.Add(Table2);
            ParentContainer.Controls.Add(pnlBusquedas);
            ParentContainer.Controls.Add(pnlConsultas);
        }

        protected bool getPermiso(BusquedaGenerica.Seccion Sec, Permiso lpPermiso)
        {
            DataTable ldtSeccion = pKDB.GetHisRegByEnt("Seccion", "Secciones de Búsqueda", new string[] { "iCodCatalogo" }, "vchCodigo = '" + Sec.ToString() + "'");
            if (ldtSeccion.Rows.Count == 0) return false;
            DataTable ldtSeccionPermiso = pKDB.GetRelRegByDes("Perfil - Seccion - Permiso", "[{Perfil}] = " + piCodPerfil + " and [{Seccion}] = " + ldtSeccion.Rows[0]["iCodCatalogo"].ToString());
            DataTable ldtPermisos;
            if (DSODataContext.GetObject("Permiso") == null)
            {
                try
                {
                    ldtPermisos = pKDB.GetHisRegByEnt("Permiso", "Permisos", new string[] { "iCodCatalogo", "vchDescripcion", "{OpcionesPermisos}" });
                    DSODataContext.SetObject("Permiso", ldtPermisos);
                }
                catch (Exception e)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "Permiso");
                    throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
                }
            }
            else
            {
                ldtPermisos = (DataTable)DSODataContext.GetObject("Permiso");
            }
            int liPermiso = (int)ldtPermisos.Select("vchDescripcion = '" + lpPermiso.ToString() + "'")[0]["{OpcionesPermisos}"];
            int liPermisoSeccion;
            if (ldtSeccionPermiso.Rows.Count == 0)
            {
                liPermisoSeccion = 0;
            }
            else
            {
                int liCodPermiso = (int)ldtSeccionPermiso.Rows[0]["{Permiso}"];
                liPermisoSeccion = (int)ldtPermisos.Select("iCodCatalogo = " + liCodPermiso)[0]["{OpcionesPermisos}"];
            }
            return (liPermiso <= liPermisoSeccion);
        }

        #region Panel Busquedas

        private void CreatePanels()
        {
            //20140812 AM. Se hace cambio para que se agregue en la pagina el grid de consumos, despues el de recursos y al final empleados.
            //Antes era empleados, recursos, consumos
            #region Antes de cambio
            //if (pbPermisoEmpleados)
            //{
            //    pnlEmpleados.ID = "pnlEmpleados";
            //    pnlEmpleados.Title = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecEmple");
            //    pnlEmpleados.ToolTip = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecEmple");
            //    pnlEmpleados.StartOpen = true;
            //    pnlEmpleados.CreateControls();
            //}

            //if (pbPermisoRecursos)
            //{
            //    pnlRecursos.ID = "pnlRecursos";
            //    pnlRecursos.Title = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecRecurs");
            //    pnlRecursos.ToolTip = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecRecurs");
            //    pnlRecursos.StartOpen = true;
            //    pnlRecursos.CreateControls();
            //}

            //if (pbPermisoConsumos)
            //{
            //    pnlConsumos.ID = "pnlConsumos";
            //    pnlConsumos.Title = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecConsumos");
            //    pnlConsumos.ToolTip = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecConsumos");
            //    pnlConsumos.StartOpen = true;
            //    pnlConsumos.CreateControls();
            //}
            #endregion Antes de cambio

            if (pbPermisoConsumos)
            {
                pnlConsumos.ID = "pnlConsumos";
                pnlConsumos.Title = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecConsumos");
                pnlConsumos.ToolTip = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecConsumos");
                pnlConsumos.StartOpen = true;
                pnlConsumos.CreateControls();
            }

            if (pbPermisoRecursos)
            {
                pnlRecursos.ID = "pnlRecursos";
                pnlRecursos.Title = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecRecurs");
                pnlRecursos.ToolTip = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecRecurs");
                pnlRecursos.StartOpen = true;
                pnlRecursos.CreateControls();
            }

            if (pbPermisoEmpleados)
            {
                pnlEmpleados.ID = "pnlEmpleados";
                pnlEmpleados.Title = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecEmple");
                pnlEmpleados.ToolTip = Globals.GetLangItem("Seccion", "Secciones de Búsqueda", "SecEmple");
                pnlEmpleados.StartOpen = true;
                pnlEmpleados.CreateControls();
            }
        }

        private void CreateGrids()
        {
            int Index = 1;

            //20140812 AM. Se hace cambio para que se agregue en la pagina el grid de consumos, despues el de recursos y al final empleados.
            //Antes era empleados, recursos, consumos
            #region Antes de cambio

            //if (pbPermisoEmpleados)
            //{
            //    CreateGrid(pGridEmpleados, BusquedaGenerica.Seccion.SecEmple, Index++, plstCamposEmpleados, "~/WebMethods.aspx/BusquedaEmpleados", getPermiso(BusquedaGenerica.Seccion.SecEmple, Permiso.Editar));
            //}
            //if (pbPermisoRecursos)
            //{
            //    CreateGrid(pGridRecursos, BusquedaGenerica.Seccion.SecRecurs, Index++, plstCamposRecursos, "~/WebMethods.aspx/BusquedaRecursos", getPermiso(BusquedaGenerica.Seccion.SecRecurs, Permiso.Editar));
            //}
            //if (pbPermisoConsumos)
            //{
            //    CreateGrid(pGridConsumos, BusquedaGenerica.Seccion.SecConsumos, Index++, plstCamposConsumos, "~/WebMethods.aspx/BusquedaConsumos", getPermiso(BusquedaGenerica.Seccion.SecConsumos, Permiso.Editar));
            //}

            #endregion Antes de cambio

            if (pbPermisoConsumos)
            {
                CreateGrid(pGridConsumos, BusquedaGenerica.Seccion.SecConsumos, Index++, plstCamposConsumos, "~/WebMethods.aspx/BusquedaConsumos", getPermiso(BusquedaGenerica.Seccion.SecConsumos, Permiso.Editar));
            }
            if (pbPermisoRecursos)
            {
                CreateGrid(pGridRecursos, BusquedaGenerica.Seccion.SecRecurs, Index++, plstCamposRecursos, "~/WebMethods.aspx/BusquedaRecursos", getPermiso(BusquedaGenerica.Seccion.SecRecurs, Permiso.Editar));
            }
            if (pbPermisoEmpleados)
            {
                CreateGrid(pGridEmpleados, BusquedaGenerica.Seccion.SecEmple, Index++, plstCamposEmpleados, "~/WebMethods.aspx/BusquedaEmpleados", getPermiso(BusquedaGenerica.Seccion.SecEmple, Permiso.Editar));
            }
        }

        private void CreateGrid(DSOGrid grdGrid, BusquedaGenerica.Seccion Seccion, int Index, List<string> lstCampos, string sAjaxSource, bool lbPermisoConsultar)
        {
            grdGrid.ID = "grd" + Seccion.ToString();
            grdGrid.CreateControls();
            grdGrid.AddClientEvent("Index", Index.ToString());

            if (!this.ParentContainer.Page.IsPostBack)
            {
                grdGrid.Config.sDom = "<\"H\"Rl>tr<\"F\"pi>";
                grdGrid.Config.bAutoWidth = true;
                grdGrid.Config.sScrollX = "100%";
                grdGrid.Config.sScrollY = "400px";
                grdGrid.Config.sPaginationType = "full_numbers";
                grdGrid.Config.bJQueryUI = true;
                grdGrid.Config.bProcessing = true;

                grdGrid.Config.bServerSide = true;
                grdGrid.Config.fnServerData = "Busqueda.fnServerData";
                grdGrid.Config.sAjaxSource = ResolveUrl(sAjaxSource);

                grdGrid.Config.aaSorting.Add(new ArrayList());
                grdGrid.Config.aaSorting[0].Add(4);
                grdGrid.Config.aaSorting[0].Add("asc");

                grdGrid.Config.aoColumnDefs.Add(new DSOGridClientColumn());
                grdGrid.Config.aoColumnDefs[0].sName = "Entidad";
                grdGrid.Config.aoColumnDefs[0].aTargets.Add(0);
                grdGrid.Config.aoColumnDefs[0].sWidth = "0px";
                grdGrid.Config.aoColumnDefs[0].bVisible = false;

                grdGrid.Config.aoColumnDefs.Add(new DSOGridClientColumn());
                grdGrid.Config.aoColumnDefs[1].sName = "Maestro";
                grdGrid.Config.aoColumnDefs[1].aTargets.Add(1);
                grdGrid.Config.aoColumnDefs[1].sWidth = "0px";
                grdGrid.Config.aoColumnDefs[1].bVisible = false;

                grdGrid.Config.aoColumnDefs.Add(new DSOGridClientColumn());
                grdGrid.Config.aoColumnDefs[2].sName = "Registro";
                grdGrid.Config.aoColumnDefs[2].aTargets.Add(2);
                grdGrid.Config.aoColumnDefs[2].sWidth = "0px";
                grdGrid.Config.aoColumnDefs[2].bVisible = false;

                grdGrid.Config.aoColumnDefs.Add(new DSOGridClientColumn());
                grdGrid.Config.aoColumnDefs[3].sName = "Consultar";
                grdGrid.Config.aoColumnDefs[3].aTargets.Add(3);
                grdGrid.Config.aoColumnDefs[3].sWidth = "50px";
                grdGrid.Config.aoColumnDefs[3].sClass = "TdConsult";
                grdGrid.Config.aoColumnDefs[3].bVisible = lbPermisoConsultar;

                foreach (string lsCampo in lstCampos)
                {
                    grdGrid.Config.aoColumnDefs.Add(new DSOGridClientColumn());
                    grdGrid.Config.aoColumnDefs[grdGrid.Config.aoColumnDefs.Count - 1].sName = lsCampo;
                    grdGrid.Config.aoColumnDefs[grdGrid.Config.aoColumnDefs.Count - 1].aTargets.Add(grdGrid.Config.aoColumnDefs.Count - 1);
                    if (Seccion == BusquedaGenerica.Seccion.SecEmple)
                    {
                        if (lsCampo == "Recurs" || lsCampo == "Sitio")
                        {
                            grdGrid.Config.aoColumnDefs[grdGrid.Config.aoColumnDefs.Count - 1].bVisible = false;
                        }
                    }
                }
            }
        }

        private void FillGrids()
        {
            //20140812 AM. Se cambia orden en que se llenan los grids de busquedas
            #region Antes de cambio
            //if (pbPermisoRecursos)
            //    pGridRecursos.Fill();

            //if (pbPermisoConsumos)
            //    pGridConsumos.Fill();

            //if (pbPermisoEmpleados)
            //    pGridEmpleados.Fill();
            #endregion Antes de cambio

            if (pbPermisoConsumos)
                pGridConsumos.Fill();

            if (pbPermisoRecursos)
                pGridRecursos.Fill();

            if (pbPermisoEmpleados)
                pGridEmpleados.Fill();
        }

        #endregion

        #region Reporte

        protected void InitSubReporte()
        {
            if (iCodSubConsulta > 0)
            {

                pSubReporte = new ReporteEstandar();
                pSubReporte.ID = "SubR" + iCodSubReporte + "_" + iNumSubReporte;
                pSubReporte.lblTitle = lblTitle;
                pSubReporte.iCodConsulta = iCodSubConsulta;
                pSubReporte.iCodReporte = iCodSubReporte;
                pSubReporte.iCodPerfil = piCodPerfil;
                pSubReporte.iEstadoConsulta = iSubEstadoConsulta;
                pSubReporte.iNumReporte = iNumSubReporte;
                pSubReporte.iCodRutaConsulta = iCodRutaConsulta;
                pSubReporte.iCodSubRutaConsulta = iCodSubRutaConsulta;
                pSubReporte.ParentContainer = pnlConsultas;

                pnlConsultas.Controls.Add(pSubReporte);

                pSubReporte.PostRegresarClick += new EventHandler(pSubReporte_PostRegresarClick);

                pSubReporte.CreateControls();
                pSubReporte.LoadScripts();

                if (!string.IsNullOrEmpty(Entidad))
                {
                    if (pSubReporte.Fields != null && pSubReporte.Fields.ContainsConfigName(Entidad))
                    {
                        int liCodCatalogo = 0;
                        int.TryParse(Registro, out liCodCatalogo);
                        pSubReporte.Fields.GetByConfigName(Entidad).DataValue = liCodCatalogo;
                    }
                }
            }
        }

        protected void SetSubReporte()
        {
            try
            {
                int liCodAplicacion = 0;
                StringBuilder lsb = new StringBuilder();
                DataTable ldt = 
                    DSODataAccess.Execute(string.Format(
                        @"select Aplic = Aplic.iCodCatalogo
                    from [{0}].[VisHistoricos('Seccion','{1}')] Seccion
	                    left join [{0}].[VisHistoricos('Aplic','{1}')] Aplic
		                    on	Seccion.Aplic = Aplic.iCodCatalogo
                    where Seccion.vchCodigo = 'SecConsumos'
                    and Seccion.dtIniVigencia <= GETDATE()
                    and Seccion.dtFinVigencia >GETDATE()
                    and Aplic.dtIniVigencia <= GETDATE()
                    and Aplic.dtFinVigencia >GETDATE()",
                    DSODataContext.Schema,
                    Globals.GetCurrentLanguage()));

                if (ldt.Rows.Count > 0)
                {
                    liCodAplicacion = (int)ldt.Rows[0]["Aplic"];
                }
                lsb.AppendLine("{Aplic} = " + liCodAplicacion);
                lsb.AppendLine("and {EstadoConsulta} is null");
                lsb.AppendLine("and {Perfil} = " + piCodPerfil);
                lsb.AppendLine("and {Atrib} is null");
                lsb.AppendLine("and {Consul} is null");
                lsb.AppendLine("and {RepEst} is not null");
                lsb.AppendLine("and {Ruta} is null");

                DataTable lKDBTable = pKDB.GetRelRegByDes("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte", lsb.ToString());

                iCodSubConsulta = liCodAplicacion;
                iCodSubReporte = (int)lKDBTable.Rows[0]["{RepEst}"];

                InitSubReporte();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrInitControls", ex);
            }
        }

        protected void pSubReporte_PostRegresarClick(object sender, EventArgs e)
        {
            if (iCodSubReporte > 0)
            {
                pSubReporte.RemoverSubReporte();

                iCodSubConsulta = 0;
                iCodSubReporte = 0;
                iSubEstadoConsulta = 0;
                pnlConsultas.Controls.Remove(pSubReporte);
                SetBusquedaState(BusquedaState.Default);
                pSubReporte = null;
            }
        }

        #endregion

        #region Historico

        protected void InitSubHistorico()
        {
            if (iCodSubAplicacion > 0)
            {
                DataRow lRowAplic = pKDB.GetHisRegByEnt("Aplic", "", "iCodCatalogo = " + iCodSubAplicacion).Rows[0];
                if (lRowAplic["{ParamVarChar3}"] != DBNull.Value)
                {
                    pSubHistorico = (HistoricEdit)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricEdit)).CodeBase, lRowAplic["{ParamVarChar3}"].ToString()).Unwrap();
                }
                else
                {
                    pSubHistorico = new HistoricEdit();
                }
                if (lRowAplic["{ParamVarChar4}"] != DBNull.Value)
                {
                    pSubHistorico.CollectionClass = lRowAplic["{ParamVarChar4}"].ToString();
                }
                pSubHistorico.SetEntidad(Entidad);
                pSubHistorico.SetMaestro(Maestro);

                pSubHistorico.ID = this.ID + "SubH" + iCodSubAplicacion;
                pSubHistorico.OpcMnu = this.OpcMnu;
                pSubHistorico.iCodAplicacion = iCodSubAplicacion;
                pSubHistorico.lblTitle = this.lblTitle;
                pSubHistorico.EsSubHistorico = true;
                pnlConsultas.Controls.Add(pSubHistorico);

                pSubHistorico.LoadScripts();
                pSubHistorico.CreateControls();

                if (!string.IsNullOrEmpty(Registro))
                {
                    pSubHistorico.FillControls();

                    pSubHistorico.InitMaestro();

                    pSubHistorico.iCodRegistro = Registro;
                    pSubHistorico.ConsultarRegistro();
                }

                pSubHistorico.PostRegresarClick += new EventHandler(pSubHistorico_PostRegresarClick);
            }
        }

        protected void SetSubHistorico(string Seccion)
        {
            StringBuilder lsbQuery = new StringBuilder();
            DataTable ldt = null;
            if (Seccion == "SecRecurs")
            {
                string liCodTipoRecurs = Maestro.Split('|')[1];
                Titulo = Maestro.Split('|')[2];
                Maestro = Maestro.Split('|')[0];
                ldt = pKDB.GetHisRegByEnt("Recurs", "", new string[] { "{Aplic}" }, "iCodCatalogo = " + liCodTipoRecurs);
            }
            else if (!string.IsNullOrEmpty(Seccion))
            {
                Titulo = Maestro;
                ldt = pKDB.GetHisRegByCod("Seccion", new string[] { Seccion }, new string[] { "{Aplic}" });
            }

            if (ldt != null && ldt.Rows.Count > 0)
            {
                iCodSubAplicacion = (int)ldt.Rows[0]["{Aplic}"];
            }
            ldt = pKDB.GetHisRegByEnt("OpcMnu", "Opciones de Menu", "[{Aplic}] = " + iCodSubAplicacion);
            if (ldt != null && ldt.Rows.Count > 0)
            {
                OpcMnu = ldt.Rows[0]["vchCodigo"].ToString();
            }
            InitSubHistorico();
        }

        protected void pSubHistorico_PostRegresarClick(object sender, EventArgs e)
        {
            if (iCodSubAplicacion > 0)
            {
                pSubHistorico.RemoverSubHistorico();

                iCodSubAplicacion = 0;
                pnlConsultas.Controls.Remove(pSubHistorico);
                SetBusquedaState(BusquedaState.Default);
                pSubHistorico = null;
            }
        }

        #endregion

        protected void SetBusquedaState(BusquedaState s)
        {
            pnlConsultas.Visible = true;
            pnlBusquedas.Visible = true;

            if (s == BusquedaState.Default)
            {
                pnlConsultas.Visible = false;
            }
            else
            {
                pnlBusquedas.Visible = false;
            }

            State = s;
        }

        public void Export(KeytiaExportFormat lkeFormat)
        {
            if (State == BusquedaState.SubReporte)
            {
                switch (lkeFormat)
                {
                    case KeytiaExportFormat.xlsx:
                        pSubReporte.ExportXLS();
                        break;
                    case KeytiaExportFormat.docx:
                        pSubReporte.ExportDOC();
                        break;
                    case KeytiaExportFormat.pdf:
                        pSubReporte.ExportPDF();
                        break;
                    case KeytiaExportFormat.csv:
                        pSubReporte.ExportCSV();
                        break;
                }
            }
            else if (State == BusquedaState.SubHistorico)
            {
                switch (lkeFormat)
                {
                    case KeytiaExportFormat.xlsx:
                        pSubHistorico.ExportXLS();
                        break;
                    case KeytiaExportFormat.docx:
                        pSubHistorico.ExportDOC();
                        break;
                    case KeytiaExportFormat.pdf:
                        pSubHistorico.ExportPDF();
                        break;
                    case KeytiaExportFormat.csv:
                        pSubHistorico.ExportCSV();
                        break;
                }
            }
            else
            {
                DSOControl.jAlert(this.ParentContainer.Page, this.ID + ".ExpNoDisp", DSOControl.JScriptEncode(Globals.GetLangItem("ExpNoDisp")), Globals.GetMsgWeb("TituloBusquedas"));
            }
        }

        #region Language

        public void InitLanguage()
        {
            btnConsultar.InnerText = Globals.GetMsgWeb("btnConsultar");
            if (State == BusquedaState.Default)
            {
                lblTitle.Text = Globals.GetMsgWeb("TituloBusquedas");

                if (pbPermisoRecursos)
                    InitGridLanguage(pGridRecursos, plstCamposRecursos);

                if (pbPermisoConsumos)
                    InitGridLanguage(pGridConsumos, plstCamposConsumos);

                if (pbPermisoEmpleados)
                    InitGridLanguage(pGridEmpleados, plstCamposEmpleados);
            }
            else if (State == BusquedaState.SubReporte)
            {
                DataTable ldt = DSODataAccess.Execute(string.Format(
                    "select Titulo = IsNull({1}, '') from [{0}].[VisHistoricos('RepEst','{1}')] where iCodCatalogo = {2} and dtIniVigencia <= GETDATE() and dtFinVigencia > GETDATE()",
                    DSODataContext.Schema,
                    Globals.GetCurrentLanguage(),
                    pSubReporte.iCodReporte));

                if (ldt.Rows.Count > 0)
                {
                    pSubReporte.Title = ldt.Rows[0]["Titulo"].ToString();
                }
                pSubReporte.InitLanguage();
            }
            else if (State == BusquedaState.SubHistorico)
            {
                if (pSubHistorico.OpcMnu != null)
                {
                    pSubHistorico.Title = Globals.GetLangItem("OpcMnu", "Opciones de Menu", pSubHistorico.OpcMnu.Replace("'", "''"));
                }
                else
                {
                    pSubHistorico.Title = Titulo;
                }
                pSubHistorico.InitLanguage();
            }
        }

        protected void InitGridLanguage(DSOGrid grdGrid, List<string> lstCampos)
        {
            int liColumn = 3;
            string lsdoPostBack = DSOControl.JScriptEncode(this.ParentContainer.Page.ClientScript.GetPostBackEventReference(this, grdGrid.ID.Substring(3) + "///{0}///{1}///{2}"));

            grdGrid.Config.oLanguage = Globals.GetGridLanguage();

            grdGrid.Config.aoColumnDefs[liColumn].bUseRendered = false;
            grdGrid.Config.aoColumnDefs[liColumn].fnRender = "function(obj){ return Busqueda.fnRenderFormat(obj,\"custom-img-search\",\"" + ResolveUrl("~/images/searchsmall.png") + "\",\"" + lsdoPostBack + "\");}";
            grdGrid.Config.aoColumnDefs[liColumn++].sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnConsultar"));

            foreach (string lsAtrib in lstCampos)
            {
                string lsTitle = HttpUtility.HtmlEncode(Globals.GetLangItem("Atrib", "Atributos", lsAtrib));
                if (lsTitle.StartsWith("#undefined-"))
                {
                    lsTitle = HttpUtility.HtmlEncode(Globals.GetLangItem("", "Entidades", lsAtrib));
                }
                if (lsTitle.StartsWith("#undefined-"))
                {
                    lsTitle = Globals.GetMsgWeb(lsAtrib);
                }
                grdGrid.Config.aoColumnDefs[liColumn++].sTitle = lsTitle;
            }
        }

        #endregion

        #region IPostBackEventHandler Members

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            string[] arguments = eventArgument.Split(new string[] { "///" }, StringSplitOptions.RemoveEmptyEntries);
            string pSeccion = arguments[0];
            Entidad = arguments[1];
            Maestro = arguments[2];
            Registro = arguments[3];
            this.pnlConsultas.Controls.Clear();

            if (pSeccion == "SecConsumos")
            {
                SetSubReporte();
                SetBusquedaState(BusquedaState.SubReporte);
            }
            else
            {
                SetSubHistorico(pSeccion);
                SetBusquedaState(BusquedaState.SubHistorico);
            }
            
        }

        #endregion
    }
}
