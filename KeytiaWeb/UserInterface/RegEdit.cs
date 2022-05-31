/*
 * Nombre:		    DMM
 * Fecha:		    20110607
 * Descripción:	    Clase que implementa métodos comunes entre Maestros y Relaciones
 * Modificación:	20110621.DMM.Cambio para que acepte fechas de vigencia anteriores a la fecha actual
 */

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI.HtmlControls;

namespace KeytiaWeb.UserInterface
{
    public abstract class RegEdit : PlaceHolder, INamingContainer, IPostBackEventHandler
    {
        #region Propiedades

        protected string psNombreTabla;
        protected int piMaxColumnSpan;
        protected Table pTablaEdit;
        protected DSOAutocomplete piCodRegistro;
        protected DSOTextBox pvchDescripcion;
        protected DSODateTimeBox pdtIniVigencia;
        protected DSODateTimeBox pdtFinVigencia;
        protected DSODateTimeBox pdtFecUltAct;
        protected DSOTextBox piCodUsuario;
        protected Panel pnlToolBar;
        protected Panel pnlContenido;
        protected HtmlButton btnEditar;
        protected HtmlButton btnAgregar;
        protected HtmlButton btnConsultar;
        protected HtmlButton btnCancelar;
        protected HtmlButton btnGrabar;
        protected HtmlButton btnBaja;
        protected HtmlButton btnConfirmarBaja;

        protected Hashtable pHTControls = new Hashtable();
        protected KDBAccess kdb = new KDBAccess();
        protected string pFileKey;
        protected string pTempPath;
        protected string psFiltroDescripcion;
        protected System.Text.StringBuilder psbErrores;
        public Table TablaEdit
        {
            get
            {
                return pTablaEdit;
            }
        }
        public DSOAutocomplete iCodRegistro
        {
            get
            {
                return piCodRegistro;
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
        public DSODateTimeBox dtFecUltAct
        {
            get
            {
                return pdtFecUltAct;
            }
        }
        public DSOTextBox vchDescripcion
        {
            get
            {
                return pvchDescripcion;
            }
        }
        #endregion

        public RegEdit()
        {
            pFileKey = Guid.NewGuid().ToString();
            pTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), HttpContext.Current.Session.SessionID);
            System.IO.Directory.CreateDirectory(pTempPath);
            Init += new EventHandler(RegEdit_Init);
            Load += new EventHandler(RegEdit_Load);
        }

        protected abstract void InitData();

        protected abstract void InitTablaEdit();

        protected abstract void ValidarRepetidos();

        protected abstract bool getPermiso(Permiso lpPermiso);

        protected abstract void DeshabilitarCatalogos();

        protected abstract void ValidarDatos();

        protected abstract DataTable getDataSource(string lsDataField, object Value);

        protected void RegEdit_Init(System.Object sender, EventArgs e)
        {
            pTablaEdit = new Table();
            pnlToolBar = new Panel();
            pnlContenido = new Panel();
            btnEditar = new HtmlButton();
            btnAgregar = new HtmlButton();
            btnConsultar = new HtmlButton();
            btnCancelar = new HtmlButton();
            btnGrabar = new HtmlButton();
            btnConfirmarBaja = new HtmlButton();
            btnBaja = new HtmlButton();
            piCodRegistro = new DSOAutocomplete();
            piCodUsuario = new DSOTextBox();
            pdtFecUltAct = new DSODateTimeBox();
            pvchDescripcion = new DSOTextBox();
            pdtIniVigencia = new DSODateTimeBox();
            pdtFinVigencia = new DSODateTimeBox();

            pnlToolBar.Controls.Add(btnConsultar);
            pnlToolBar.Controls.Add(btnEditar);
            pnlToolBar.Controls.Add(btnAgregar);
            pnlToolBar.Controls.Add(btnGrabar);
            pnlToolBar.Controls.Add(btnBaja);
            pnlToolBar.Controls.Add(btnConfirmarBaja);
            pnlToolBar.Controls.Add(btnCancelar);
            pnlContenido.Controls.Add(pTablaEdit);
            Controls.Add(pnlToolBar);
            Controls.Add(pnlContenido);
        }

        protected void RegEdit_Load(System.Object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                EnsureChildControls();
                try
                {
                    InitData();
                    InitFiltros();
                    InitTablaEdit();
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException("ErrInitControls", ex);
                }
            }
            setDataSource();
        }

        protected DataTable getDataSource(string lsQuery, bool isAtrib)
        {
            try
            {

                DataTable dataSource = null;
                if (isAtrib)
                {
                    dataSource = kdb.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{" + HttpContext.Current.Session["Language"] + "}" }, lsQuery, "{" + HttpContext.Current.Session["Language"] + "}");
                }
                else
                {
                    dataSource = DSODataAccess.Execute(lsQuery + " order by vchDescripcion");
                }
                return dataSource;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
        }

        protected void setDataSource()
        {
            //Como los dropdowns se llenan con Ajax, siempre es necesario volver a llenarlos en el page load
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                if (ctl is IDSOFillableInput)
                {
                    if (IsAtrib(ctl.DataField))
                    {
                        DataTable dataSource = getDataSource(ctl.DataField, ((IDSOFillableInput)ctl).TextValue.Text);
                        if (dataSource != null)
                        {
                            ((IDSOFillableInput)ctl).DataSource = dataSource;
                            ((IDSOFillableInput)ctl).Fill();
                        }
                    }
                    else if (IsFlag(ctl.DataField))
                    {
                        DataTable dataSource = getDataSource(ctl.DataField, null);
                        if (dataSource != null)
                        {
                            ((IDSOFillableInput)ctl).DataSource = dataSource;
                            ((IDSOFillableInput)ctl).Fill();
                        }
                    }
                }
            }
            piCodRegistro.IsDropDown = true;
            piCodRegistro.DataSource = "Select id = iCodRegistro, value = vchDescripcion from " + psNombreTabla + " where iCodRegistro = " + piCodRegistro.Value;
            piCodRegistro.Fill();
            piCodRegistro.IsDropDown = false;

        }

        public void CreateControls()
        {
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            pTablaEdit.ID = "TablaEdit";
            pTablaEdit.CssClass = "TablaEdit";

            CreateToolBar();
            CreateFiltros();
            CreateTablaEdit();
            CreateVigencias();
            setEstado(ViewState["Estado"] != null ? ViewState["Estado"].ToString() : "Inicio");

            ChildControlsCreated = true;
        }

        protected virtual void CreateToolBar()
        {
            pnlToolBar.ID = "toolBar";
            pnlContenido.ID = "contenido";
            btnEditar.ID = "btnEditar";
            btnAgregar.ID = "btnAgregar";
            btnConsultar.ID = "btnConsultar";
            btnCancelar.ID = "btnCancelar";
            btnGrabar.ID = "btnGrabar";
            btnConfirmarBaja.ID = "btnConfirmarBaja";
            btnBaja.ID = "btnBaja";

            pnlToolBar.CssClass = "pnlToolBar fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";
            pnlContenido.CssClass = "contenidoTabla";
            btnEditar.Attributes["class"] = "buttonEdit";
            btnAgregar.Attributes["class"] = "buttonAdd";
            btnConsultar.Attributes["class"] = "buttonSearch";
            btnCancelar.Attributes["class"] = "buttonCancel";
            btnGrabar.Attributes["class"] = "buttonSave";
            btnConfirmarBaja.Attributes["class"] = "buttonSave";
            btnBaja.Attributes["class"] = "buttonDelete";

            btnEditar.Style["display"] = "none";
            btnAgregar.Style["display"] = "none";
            btnConsultar.Style["display"] = "none";
            btnCancelar.Style["display"] = "none";
            btnGrabar.Style["display"] = "none";
            btnConfirmarBaja.Style["display"] = "none";
            btnBaja.Style["display"] = "none";

            btnEditar.ServerClick += btnEditar_Click;
            btnAgregar.ServerClick += btnAgregar_Click;
            btnConsultar.ServerClick += btnConsultar_Click;
            btnBaja.ServerClick += btnBaja_Click;
            btnGrabar.Attributes.Add(HtmlTextWriterAttribute.Onclick.ToString(),
                "$(this).button(\"disable\"); " + Page.ClientScript.GetPostBackEventReference(this, "btnGrabar") + ";");

            btnConfirmarBaja.Attributes.Add(HtmlTextWriterAttribute.Onclick.ToString(),
                "return RegEdit.Botones.btnBaja_Click('" + Globals.GetMsgWeb("ConfirmarBaja") + "', '"
                                                         + Globals.GetMsgWeb("Titulo" + psNombreTabla) + "', "
                                                         + "function(){ " + Page.ClientScript.GetPostBackEventReference(this, "btnConfirmarBaja") + "});");

            btnCancelar.Attributes.Add(HtmlTextWriterAttribute.Onclick.ToString(),
                "return RegEdit.Botones.btnCancelar_Click('" + Globals.GetMsgWeb("ConfirmarCambios") + "', '"
                                                             + Globals.GetMsgWeb("Titulo" + psNombreTabla) + "', "
                                                             + "function(){ " + Page.ClientScript.GetPostBackEventReference(this, "btnCancelar") + " });");

            btnConsultar.Attributes["disabled"] = "disabled";
            btnAgregar.Attributes["disabled"] = "disabled";
        }

        protected virtual void CreateFiltros()
        {
            piCodRegistro.ID = "iCodRegistro";
            piCodRegistro.Table = pTablaEdit;
            piCodRegistro.Row = pTablaEdit.Rows.Count + 1;
            piCodRegistro.ColumnSpan = piMaxColumnSpan;
            piCodRegistro.AutoCompleteChange += new EventHandler(iCodRegistro_Change);
            piCodRegistro.CreateControls();
            piCodRegistro.DataValueDelimiter = "";

            pvchDescripcion.ID = "vchDescripcion";
            pvchDescripcion.DataField = "vchDescripcion";
            pvchDescripcion.Row = pTablaEdit.Rows.Count + 1;
            pvchDescripcion.ColumnSpan = piMaxColumnSpan;
            pvchDescripcion.Table = pTablaEdit;
            pvchDescripcion.CreateControls();

            pHTControls.Add(pvchDescripcion.ID, pvchDescripcion);
        }

        protected virtual void CreateTablaEdit()
        {
            piCodUsuario.ID = "iCodUsuario";
            piCodUsuario.Table = pTablaEdit;
            piCodUsuario.Row = pTablaEdit.Rows.Count + 1;
            piCodUsuario.ColumnSpan = piMaxColumnSpan;
            piCodUsuario.CreateControls();
            ((WebControl)piCodUsuario.Control).Enabled = false;

            pdtFecUltAct.ID = "dtFecUltAct";
            pdtFecUltAct.ColumnSpan = piMaxColumnSpan;
            pdtFecUltAct.Row = pTablaEdit.Rows.Count + 1;
            pdtFecUltAct.Table = pTablaEdit;
            pdtFecUltAct.ShowHour = false;
            pdtFecUltAct.ShowMinute = false;
            pdtFecUltAct.ShowSecond = false;
            pdtFecUltAct.CreateControls();
            ((WebControl)pdtFecUltAct.Control).Enabled = false;
        }

        protected void CreateVigencias()
        {
            pdtIniVigencia.ID = "dtIniVigencia";
            pdtIniVigencia.ColumnSpan = piMaxColumnSpan;
            pdtIniVigencia.Row = pTablaEdit.Rows.Count + 1;
            pdtIniVigencia.Table = pTablaEdit;
            pdtIniVigencia.DataField = "dtIniVigencia";
            pdtIniVigencia.ShowHour = false;
            pdtIniVigencia.ShowMinute = false;
            pdtIniVigencia.ShowSecond = false;
            pdtIniVigencia.CreateControls();
            pHTControls.Add(pdtIniVigencia.ID, pdtIniVigencia);

            pdtFinVigencia.ID = "dtFinVigencia";
            pdtFinVigencia.ColumnSpan = piMaxColumnSpan;
            pdtFinVigencia.Row = pTablaEdit.Rows.Count + 1;
            pdtFinVigencia.Table = pTablaEdit;
            pdtFinVigencia.DataField = "dtFinVigencia";
            pdtFinVigencia.ShowHour = false;
            pdtFinVigencia.ShowMinute = false;
            pdtFinVigencia.ShowSecond = false;
            pdtFinVigencia.CreateControls();
            pHTControls.Add(pdtFinVigencia.ID, pdtFinVigencia);

        }

        protected virtual void InitFiltros()
        {
            piCodRegistro.DataField = "iCodRegistro";
            piCodRegistro.AutoPostBack = true;
            piCodRegistro.IsDropDown = false;
            piCodRegistro.MinLength = 0;
            piCodRegistro.OnSelect = "RegEdit.Botones.setEstado";
        }

        protected void Fill()
        {
            if (piCodRegistro.DataValue.ToString() == "null")
            {
                LimpiarControles();
                return;
            }
            try
            {
                DataTable dtEdit = DSODataAccess.Execute("Select * from " + psNombreTabla + " where iCodRegistro = " + piCodRegistro.DataValue.ToString());
                if (dtEdit.Rows.Count == 0) return;
                foreach (DataColumn dc in dtEdit.Columns)
                {
                    if (pHTControls.Contains(dc.ColumnName))
                    {
                        DSOControlDB ctl = (DSOControlDB)pHTControls[dc.ColumnName];
                        if (ctl is DSODropDownList && !(dtEdit.Rows[0][dc] is DBNull))
                        {
                            DSODropDownList ddl = (DSODropDownList)ctl;
                            string lsFiltro = "";
                            if (dtEdit.Rows[0][dc] is DBNull)
                            {
                                lsFiltro = ddl.TextValue.Text;
                            }
                            else if (ddl.TextValue.Text == "null")
                            {
                                lsFiltro = dtEdit.Rows[0][dc].ToString();
                            }
                            else
                            {
                                lsFiltro = dtEdit.Rows[0][dc].ToString() + "," + ddl.TextValue.Text;
                            }
                            DataTable dataSource = getDataSource(ddl.DataField, lsFiltro);
                            if (dataSource != null)
                            {
                                ddl.DataSource = dataSource;
                                ddl.Fill();
                            }
                        }
                        ctl.DataValue = dtEdit.Rows[0][dc];
                    }
                    else if (dc.ColumnName == "iCodUsuario")
                    {
                        if (dtEdit.Rows[0][dc] is DBNull)
                        {
                            piCodUsuario.DataValue = DBNull.Value;
                        }
                        else
                        {
                            piCodUsuario.DataValue = kdb.ExecuteScalar("Usuar", "Usuarios",
                            "Select vchDescripcion from Catalogos where iCodRegistro = " + dtEdit.Rows[0][dc].ToString());
                        }
                    }
                    else if (dc.ColumnName == "dtFecUltAct")
                    {
                        pdtFecUltAct.DataValue = dtEdit.Rows[0][dc];
                    }

                }
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
        }

        protected void LimpiarControles()
        {
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                ctl.DataValue = System.DBNull.Value;
            }
            piCodUsuario.DataValue = DBNull.Value;
            pdtFecUltAct.DataValue = DBNull.Value;
        }

        protected void HabilitarControles(bool value)
        {
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                ((WebControl)ctl.Control).Enabled = value;
                if (IsAtrib(ctl.DataField))
                {
                    pTablaEdit.Rows[ctl.Row - 1].Visible = value || ctl.DataValue.ToString() != "null";
                }
            }
            ((WebControl)pdtIniVigencia.Control).Enabled = false;
            ((WebControl)pdtFinVigencia.Control).Enabled = false;
            if (ViewState["Estado"].ToString() == "Edicion")
            {
                ((WebControl)pvchDescripcion.Control).Enabled = false;
            }
        }

        protected virtual void DeshabilitarOpciones(bool value)
        {

        }

        protected virtual void setRequiredMessages()
        {
            pvchDescripcion.RequiredMessage = Globals.GetMsgWeb("CampoRequerido", HttpUtility.HtmlDecode(pvchDescripcion.Descripcion));
        }

        protected virtual void setEstado(string Estado)
        {
            bool bTablaEdit = false, 
                 bDescripcion = false, 
                 bConsultar = false, 
                 bEditar = false, 
                 bAgregar = false, 
                 bCancelar = false, 
                 bGrabar = false, 
                 bBaja = false, 
                 bConfirmarBaja = false;

            ((TableRow) pdtIniVigencia.TcCtl.Parent).Style.Remove("display");
            ((TableRow)pdtFinVigencia.TcCtl.Parent).Style.Remove("display");

            ViewState["Estado"] = Estado;
            
            switch (Estado)
            {
                case "Inicio":
                    bConsultar = true;
                    bAgregar = true;
                    break;
                case "Consulta":
                    bTablaEdit = true;
                    bConsultar = true;
                    bEditar = true;
                    bAgregar = true;
                    bBaja = true;

                    break;
                case "Edicion":
                    bTablaEdit = true;
                    bDescripcion = true;
                    bCancelar = true;
                    bGrabar = true;
                    ((TableRow)pdtIniVigencia.TcCtl.Parent).Style["display"] = "none";
                    ((TableRow)pdtFinVigencia.TcCtl.Parent).Style["display"] = "none";

                    break;
                case "ConfirmarBaja":
                    bTablaEdit = true;
                    bDescripcion = true;
                    bCancelar = true;
                    bConfirmarBaja = true;

                    break;
            }
            setEstado(bTablaEdit, bDescripcion, bConsultar, bEditar, bAgregar, bCancelar, bGrabar, bBaja, bConfirmarBaja);
        }

        protected void setEstado(bool bTablaEdit, bool bDescripcion, bool bConsultar, bool bEditar, bool bAgregar, bool bCancelar, bool bGrabar, bool bBaja, bool bConfirmarBaja)
        {
            bAgregar = bAgregar && getPermiso(Permiso.Agregar);
            bEditar = bEditar && getPermiso(Permiso.Editar);
            bBaja = bBaja && getPermiso(Permiso.Eliminar);
            bConfirmarBaja = bConfirmarBaja && getPermiso(Permiso.Eliminar);
            bConsultar = bConsultar && getPermiso(Permiso.Consultar);

            btnConsultar.Visible = bConsultar;
            btnEditar.Visible = bEditar;
            btnAgregar.Visible = bAgregar;
            btnCancelar.Visible = bCancelar;
            btnGrabar.Visible = bGrabar;
            btnConfirmarBaja.Visible = bConfirmarBaja;
            btnBaja.Visible = bBaja;
            pTablaEdit.Rows[piCodRegistro.Row - 1].Style["display"] = (!bDescripcion ? "" : "none");
            pTablaEdit.Rows[pvchDescripcion.Row - 1].Style["display"] = (bDescripcion ? "" : "none");
            for (int i = pvchDescripcion.Row; i < pTablaEdit.Rows.Count; i++)
            {
                pTablaEdit.Rows[i].Style["display"] = (bTablaEdit ? "" : "none");
            }
        }

        protected DataTable DataSourceNumbers(int Count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("id", typeof(int)));
            dt.Columns.Add(new DataColumn("descripcion", typeof(string)));
            for (int i = 1; i <= Count; i++)
                dt.Rows.Add(new object[] { i, i.ToString() });
            return dt;
        }

        public virtual void InitLanguage()
        {
            btnEditar.InnerText = Globals.GetMsgWeb("btnEditar");
            btnAgregar.InnerText = Globals.GetMsgWeb("btnAgregar");
            btnConsultar.InnerText = Globals.GetMsgWeb("btnConsultar");
            btnCancelar.InnerText = Globals.GetMsgWeb("btnCancelar");
            btnGrabar.InnerText = Globals.GetMsgWeb("btnGrabar");
            btnConfirmarBaja.InnerText = Globals.GetMsgWeb("btnGrabar");
            btnBaja.InnerText = Globals.GetMsgWeb("btnBaja");
            pdtIniVigencia.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateFormat"));
            pdtFinVigencia.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateFormat"));

            piCodRegistro.Descripcion = Globals.GetMsgWeb("vchDescripcion");
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                string lsDescripcion;
                if (!IsAtrib(ctl.DataField))
                {
                    lsDescripcion = Globals.GetMsgWeb(ctl.DataField);
                }
                else
                {
                    lsDescripcion = ctl.DataField;
                }
                ctl.Descripcion = lsDescripcion;
            }
        }

        protected bool IsAtrib(string lsDataField)
        {
            int iCol;
            return int.TryParse(lsDataField.Substring(lsDataField.Length - 2), out iCol) && !lsDataField.StartsWith("iFlags");
        }

        protected bool IsFlag(string lsDataField)
        {
            return lsDataField.StartsWith("iFlags");
        }

        #region Eventos Botones

        protected bool RegistroCambiado()
        {
            if (piCodRegistro.DataValue.ToString() == "null") return true;
            DataTable dtEdit = new DataTable();
            try
            {
                dtEdit = DSODataAccess.Execute("Select * from " + psNombreTabla + " where iCodRegistro = " + piCodRegistro.DataValue.ToString());
            }
            catch (Exception ex)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException("ErrorConsulta", ex, lsTitulo);
            }
            foreach (DataColumn dc in dtEdit.Columns)
            {
                if (pHTControls.Contains(dc.ColumnName))
                {
                    string lsDataValueDelimiter = ((DSOControlDB)pHTControls[dc.ColumnName]).DataValueDelimiter;
                    string ctlValue = ((DSOControlDB)pHTControls[dc.ColumnName]).DataValue.ToString();
                    string DBValue;

                    if (lsDataValueDelimiter == null)
                        lsDataValueDelimiter = "";

                    if ((dtEdit.Rows[0][dc] is DBNull))
                        DBValue = "null";
                    else if ((dtEdit.Rows[0][dc] is DateTime))
                        DBValue = lsDataValueDelimiter + ((DateTime)dtEdit.Rows[0][dc]).ToString("yyyy-MM-dd HH:mm:ss") + lsDataValueDelimiter;
                    else
                        DBValue = lsDataValueDelimiter + dtEdit.Rows[0][dc].ToString() + lsDataValueDelimiter;

                    if (ctlValue != DBValue)
                        return true;
                }
            }
            return false;
        }

        protected void btnEditar_Click(System.Object sender, System.EventArgs e)
        {
            if (piCodRegistro.DataValue.ToString() == "null") return;

            setEstado("Edicion");
            Fill();
            HabilitarControles(true);
            DeshabilitarOpciones(false);
            DeshabilitarCatalogos();
        }

        protected void iCodRegistro_Change(System.Object sender, System.EventArgs e)
        {
            if (piCodRegistro.DataValue.ToString() != "null")
            {
                btnConsultar_Click(sender, e);
            }
        }

        protected void btnConsultar_Click(System.Object sender, System.EventArgs e)
        {
            if (piCodRegistro.DataValue.ToString() != "null")
            {
                setEstado("Consulta");
                Fill();
                HabilitarControles(false);
            }
            else
            {
                DSOControl.jAlert(Page, "AlertNoCoincidencias", Globals.GetMsgWeb("AlertNoCoincidencias"), Globals.GetMsgWeb("Titulo" + psNombreTabla));
            }
        }

        protected void btnCancelar_Click()
        {
            if (piCodRegistro.DataValue.ToString() != "null")
            {
                setEstado("Consulta");
                Fill();
                HabilitarControles(false);
            }
            else
            {
                setEstado("Inicio");
            }
        }

        protected void btnAgregar_Click(System.Object sender, System.EventArgs e)
        {
            if (piCodRegistro.DataValue.ToString() != "null") return;

            setEstado("Edicion");
            Fill();
            HabilitarControles(true);

            pvchDescripcion.DataValue = piCodRegistro.Search.Text;
        }

        protected void btnBaja_Click(object sender, EventArgs e)
        {
            if (piCodRegistro.DataValue.ToString() != "null")
            {
                setEstado("ConfirmarBaja");
                HabilitarControles(false);
                pdtFinVigencia.DataValue = pdtIniVigencia.Date;
            }
            else
            {
                setEstado("Inicio");
            }
        }

        protected void btnConfirmarBaja_Click()
        {
            if (piCodRegistro.DataValue.ToString() != "null")
            {
                btnGrabar_Click();
            }
            else
            {
                setEstado("Inicio");
            }
        }

        protected void btnGrabar_Click()
        {
            if (ViewState["Estado"].ToString() == "ConfirmarBaja" || RegistroCambiado())
            {
                //Si el registro no es válido, se regresa a la pantalla de edición, mostrando los errores encontrados
                if (!ValidarRegistro()) //La función de validar se encarga de mostrar los mensajes de error, si los hay
                {
                    setEstado(ViewState["Estado"].ToString() == "ConfirmarBaja" ? "ConfirmarBaja" : "Edicion");
                    return;
                }

                int liCodRegistro;
                if (piCodRegistro.DataValue.ToString() == "null")
                {
                    liCodRegistro = AgregarRegistro();
                    if (liCodRegistro < 0)
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                }
                else
                {
                    if (!ActualizarRegistro())
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                    piCodRegistro.IsDropDown = true;
                    liCodRegistro = (int)piCodRegistro.Value;
                    piCodRegistro.IsDropDown = false;
                }

                piCodRegistro.IsDropDown = true;
                piCodRegistro.DataSource = "Select id = iCodRegistro, value = vchDescripcion from " + psNombreTabla + " where iCodRegistro = " + liCodRegistro;
                piCodRegistro.Fill();
                piCodRegistro.IsDropDown = false;

                if (ViewState["Estado"].ToString() == "ConfirmarBaja")
                {
                    piCodRegistro.DataValue = DBNull.Value;
                }
                else
                {
                    piCodRegistro.DataValue = liCodRegistro;
                }
            }
            btnCancelar_Click();
        }

        protected int AgregarRegistro()
        {
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            return cargasCOM.InsertaRegistro(getValoresCampos(), psNombreTabla, "", "", false, (int)System.Web.HttpContext.Current.Session["iCodUsuarioDB"], true);
        }

        protected bool ActualizarRegistro()
        {
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            return cargasCOM.ActualizaRegistro(psNombreTabla, "", "", getValoresCampos(), int.Parse(piCodRegistro.DataValue.ToString()), false, (int)System.Web.HttpContext.Current.Session["iCodUsuarioDB"], true);
        }

        protected virtual Hashtable getValoresCampos()
        {
            Hashtable phtValoresCampos = new Hashtable();
            try
            {
                piCodUsuario.DataValue = kdb.ExecuteScalar("Usuar", "Usuarios",
                "Select vchDescripcion from Catalogos where iCodRegistro = " + ((int)HttpContext.Current.Session["iCodUsuario"]).ToString());
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
            DateTime ldtNow = DateTime.Now;
            pdtFecUltAct.DataValue = ldtNow;

            phtValoresCampos.Clear();
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                phtValoresCampos.Add(ctl.DataField, ctl.DataValue.ToString());
            }
            phtValoresCampos.Add("iCodUsuario", ((int)HttpContext.Current.Session["iCodUsuario"]).ToString());
            phtValoresCampos.Add("dtFecUltAct", ldtNow);
            return phtValoresCampos;
        }

        protected bool ValidarRegistro()
        {
            try
            {
                Object dtDateIniVigencia;
                Object dtDateFinVigencia;

                dtDateIniVigencia = pdtIniVigencia.Date;
                dtDateFinVigencia = pdtFinVigencia.Date;

                if (pdtIniVigencia.Date == DateTime.MinValue)
                {
                    dtDateIniVigencia = System.DBNull.Value;
                }
                if (pdtFinVigencia.Date == DateTime.MinValue)
                {
                    dtDateFinVigencia = System.DBNull.Value;
                }

                ValidarCampos();
                ValidarDatos();
                if (psbErrores.Length > 0)
                {
                    string msg;
                    msg = Globals.GetMsgWeb("ErroresEncontrados") + "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, "msgError", msg, Globals.GetMsgWeb("Titulo" + psNombreTabla));
                    pdtIniVigencia.DataValue = dtDateIniVigencia;
                    pdtFinVigencia.DataValue = dtDateFinVigencia;
                    return false;
                }

                if (ViewState["Estado"].ToString() == "ConfirmarBaja")
                {
                    pdtFinVigencia.DataValue = pdtIniVigencia.Date;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected virtual void ValidarCampos()
        {
            psbErrores = new System.Text.StringBuilder();
            string lFiltroDescripcion = psFiltroDescripcion;
            DateTime ldtIniVigencia, ldtFinVigencia;
            int liRepetidos = 0;
            DateTime Today = DateTime.Today;

            if (pdtIniVigencia.DataValue.ToString() == "null")
            {
                pdtIniVigencia.DataValue = Today;
            }

            if (pdtFinVigencia.DataValue.ToString() == "null")
            {
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

            ldtIniVigencia = pdtIniVigencia.Date;
            ldtFinVigencia = pdtFinVigencia.Date;

            setRequiredMessages();
            ValidarRepetidos();
            //Validar que no se repita la Descripción
            if (piCodRegistro.DataValue.ToString() != "null")//Edicion
            {
                lFiltroDescripcion += " and iCodRegistro <> " + piCodRegistro.DataValue;
            }
            try
            {
                liRepetidos = (int)DSODataAccess.ExecuteScalar("Select Count(*) from " + psNombreTabla + " where dtIniVigencia <> dtFinVigencia and vchDescripcion = " + pvchDescripcion.DataValue + lFiltroDescripcion);
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
            if (liRepetidos > 0)
            {
                psbErrores.Append("<li>" + Globals.GetMsgWeb("DescripcionRepetida") + "</li>");
            }

            if (ldtFinVigencia.CompareTo(ldtIniVigencia) < 0)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("VigenciaFin", pdtIniVigencia.Descripcion, pdtFinVigencia.Descripcion) + "</li>");

        }

        protected static string getColCatRel()
        {
            return getColsTable("iCodCatalogo", "Relaciones");
        }

        public static string getColsTable(string suffix, string tableName)
        {
            return getColsTable(suffix, tableName, false);
        }

        public static string getColsTable(string suffix, string tableName, bool endsWithNumber)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + tableName);
                throw new KeytiaWebSessionException(true, lsTitulo);
            }
            try
            {
                int iCol;
                ArrayList lsValores = new ArrayList();
                DataTable dt = DSODataAccess.Execute("Select * from " + tableName + " where 1 = 2");
                foreach (DataColumn dc in dt.Columns)
                {
                    bool lbNumber = true;
                    if (endsWithNumber)
                    {
                        lbNumber = int.TryParse(dc.ColumnName.Substring(dc.ColumnName.Length - 2), out iCol);
                    }
                    if ((string.IsNullOrEmpty(suffix) || dc.ColumnName.StartsWith(suffix)) && lbNumber)
                    {
                        lsValores.Add(dc.ColumnName);
                    }
                }
                return string.Join(",", (string[])lsValores.ToArray(typeof(string)));
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + tableName);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
        }
        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            switch (eventArgument)
            {
                case "btnCancelar":
                    btnCancelar_Click();
                    break;
                case "btnConfirmarBaja":
                    btnConfirmarBaja_Click();
                    break;
                case "btnGrabar":
                    if (ViewState["Estado"].ToString() == "Edicion" || ViewState["Estado"].ToString() == "ConfirmarBaja")
                    {
                        btnGrabar_Click();
                    }
                    break;
            }
        }

        #endregion

        #region Exportar Archivo

        public void ExportarExcel()
        {
            btnConsultar_Click(this, EventArgs.Empty);

            ExcelAccess oExcel = new ExcelAccess();
            try
            {

                string lsStylePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Session["StyleSheet"].ToString());
                object[,] oArreglo = oExcel.DataTableToArray(GetExportData());
                string lsHoja = Globals.GetMsgWeb("Titulo" + psNombreTabla);
                oExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\reportes\Plantilla Reporte Estandar.xlsx");
                oExcel.Abrir();
                oExcel.Renombrar(oExcel.NombreHoja0(), lsHoja);
                oExcel.xlSheet.get_Range("A1", "A20").Clear();

                EstiloTablaExcel lEstiloTablaExcel = new EstiloTablaExcel();
                lEstiloTablaExcel.Estilo = "KeytiaParamRep";
                lEstiloTablaExcel.FilaEncabezado = false;
                lEstiloTablaExcel.FilaTotales = false;
                lEstiloTablaExcel.FilasBandas = false;
                lEstiloTablaExcel.PrimeraColumna = false;
                lEstiloTablaExcel.UltimaColumna = false;
                lEstiloTablaExcel.ColumnasBandas = true;
                lEstiloTablaExcel.AutoFiltro = false;

                oExcel.Actualizar(lsHoja, 1, 1, oArreglo, lEstiloTablaExcel);
                oExcel.xlSheet.get_Range("A1", "H1").EntireColumn.AutoFit();
                oExcel.Remover("DatosGr");
                oExcel.Remover("DatosGrHis");
                oExcel.FilePath = getFileName("xlsx");
                oExcel.SalvarComo();
                ExportarArchivo("xlsx");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".xlsx");
            }
            finally
            {
                if (oExcel != null)
                {
                    oExcel.Cerrar(true);
                    oExcel.Dispose();
                    oExcel = null;
                }
            }
        }

        public void ExportarWord()
        {
            btnConsultar_Click(this, EventArgs.Empty);
            CrearWord("docx");
        }

        public void ExportarPDF()
        {
            btnConsultar_Click(this, EventArgs.Empty);
            CrearWord("pdf");
        }

        public void ExportarCSV()
        {
            btnConsultar_Click(this, EventArgs.Empty);
            TxtFileAccess oCSV = new TxtFileAccess();

            try
            {
                oCSV.FileName = getFileName("csv");
                oCSV.Abrir();
                DataTable dt = GetExportData();
                foreach (DataRow dr in dt.Rows)
                {
                    string[] oRenglon = new string[dt.Columns.Count];
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        if (dr[col] != null)
                        {
                            oRenglon[col] = dr[col].ToString();
                        }
                    }
                    oCSV.Escribir("\"" + string.Join("\",\"", oRenglon) + "\"");
                }
                ExportarArchivo("csv");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".csv");
            }
            finally
            {
                oCSV.Cerrar();
                oCSV = null;
            }
        }

        protected void CrearWord(string ext)
        {
            WordAccess oWord = new WordAccess();

            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Session["StyleSheet"].ToString());
                object oMissing = System.Type.Missing;
                DataTable dt = GetExportData();
                oWord.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\reportes\Plantilla Reporte Estandar.docx");
                oWord.Abrir();
                oWord.Doc.Select();
                oWord.App.Selection.Delete(ref oMissing, ref oMissing);
                EstiloTablaWord lEstiloTablaWord = new EstiloTablaWord();
                lEstiloTablaWord.Estilo = "KeytiaParamRep";
                lEstiloTablaWord.FilaEncabezado = false;
                lEstiloTablaWord.FilaTotales = false;
                lEstiloTablaWord.FilasBandas = false;
                lEstiloTablaWord.PrimeraColumna = false;
                lEstiloTablaWord.UltimaColumna = false;
                lEstiloTablaWord.ColumnasBandas = true;
                oWord.InsertarTabla(dt, lEstiloTablaWord.FilaEncabezado, lEstiloTablaWord.Estilo, lEstiloTablaWord);
                oWord.Tabla.AllowAutoFit = true;
                oWord.Tabla.Columns.AutoFit();
                CombinarCeldasNulosWord(oWord);
                oWord.FilePath = getFileName(ext);
                oWord.SalvarComo();
                ExportarArchivo(ext);
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, "." + ext);
            }
            finally
            {
                oWord.Cerrar(true);
                oWord = null;
            }
        }

        protected void CombinarCeldasNulosWord(WordAccess pWord)
        {
            int liColIni;
            for (int li = 1; li <= pWord.Tabla.Rows.Count; li++)
            {
                int lj = 2;
                while (lj <= pWord.Tabla.Rows[li].Cells.Count)
                {
                    liColIni = lj;
                    while (lj < pWord.Tabla.Rows[li].Cells.Count &&
                        string.IsNullOrEmpty(pWord.Tabla.Cell(li, lj + 1).Range.Text.Trim().Replace("\n", "").Replace("\a", "")))
                    {
                        lj++;
                    }
                    if (lj <= pWord.Tabla.Rows[li].Cells.Count)
                    {
                        pWord.CombinarCeldas(li, liColIni, li, lj);
                        lj = liColIni + 1;
                        while (lj <= pWord.Tabla.Rows[li].Cells.Count &&
                            !string.IsNullOrEmpty(pWord.Tabla.Cell(li, lj).Range.Text.Trim().Replace("\n", "").Replace("\a", "")))
                        {
                            lj++;
                        }

                    }
                }
            }
        }

        protected void ExportarArchivo(string ext)
        {
            string lsTitulo = HttpUtility.UrlEncode(Globals.GetMsgWeb(false, "Titulo" + psNombreTabla));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + pFileKey + "&fn=" + lsTitulo + "." + ext);
        }

        protected DataTable GetExportData()
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < piMaxColumnSpan + 1; i++)
            {
                dt.Columns.Add(new DataColumn("Columna" + (i + 1).ToString()));
            }

            foreach (TableRow tr in pTablaEdit.Rows)
            {
                if (tr.Visible && tr.Style["display"] != "none")
                {
                    DataRow dr = dt.NewRow();
                    int col = 0;
                    foreach (TableCell tc in tr.Cells)
                    {
                        if (tc.Controls.Count > 0 && tc.Controls[0] is Label)
                        {
                            Label ctl = (Label)tc.Controls[0];
                            dr[col++] = HttpUtility.HtmlDecode(ctl.Text);
                        }
                        else if (tc.Controls.Count > 0 && tc.Controls[0] is DSOControlDB)
                        {
                            DSOControlDB ctl = (DSOControlDB)tc.Controls[0];
                            dr[col++] = ctl.ToString();
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        protected string getFileName(string ext)
        {
            string lsFileName = System.IO.Path.Combine(pTempPath, "upl." + pFileKey + ".temp." + ext);
            HttpContext.Current.Session[pFileKey] = lsFileName;
            return lsFileName;
        }

        #endregion

        #region WebMethods

        protected static string GetRegistros(string lsNombreTabla, string lsFiltro, string vchDescripcion)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + lsNombreTabla);
                throw new KeytiaWebSessionException(true, lsTitulo);
            }
            try
            {
                if (string.IsNullOrEmpty(lsFiltro))
                {
                    lsFiltro = "1=1";
                }
                DataTable dt;
                dt = DSODataAccess.Execute("select id = iCodRegistro, value = vchDescripcion from " + lsNombreTabla.Replace("'", "''")
                    + " where " + lsFiltro.Replace("'", "''")
                    + " and vchDescripcion like '%" + vchDescripcion.Replace("'", "''") + "%'"
                    + " and dtIniVigencia <> dtFinVigencia"
                    + " order by vchDescripcion");
                string json = DSOControl.SerializeJSON<DataTable>(dt);
                return json;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + lsNombreTabla);
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        public static string GetMaestros(string term, int iCodEntidad)
        {
            return GetRegistros("Maestros", "iCodEntidad = " + iCodEntidad.ToString(), term);
        }

        public static string GetRelaciones(string term)
        {
            return GetRegistros("Relaciones", "iCodRelacion is null", term);
        }

        public static string GetDataSource(string lsNombreTabla, string tipoCampo, string iCodEntidad, string Excluir)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + lsNombreTabla);
                throw new KeytiaWebSessionException(true, lsTitulo);
            }
            try
            {
                DataTable dt = new DataTable();
                if (string.IsNullOrEmpty(Excluir))
                {
                    Excluir = "0";
                }
                switch (tipoCampo)
                {
                    case "iCodRelacion":
                    case "iCodCatalogo":
                        dt = DSODataAccess.Execute(KeytiaWeb.UserInterface.MaestroEdit.getFiltroAtrib(tipoCampo, iCodEntidad) +
                                                " and not iCodRegistro in (" + Excluir + ")" +
                                                " order by vchDescripcion");
                        dt.Columns[0].ColumnName = "value";
                        dt.Columns[1].ColumnName = "text";
                        break;
                    case "Integer":
                    case "Float":
                    case "Date":
                    case "VarChar":
                        KDBAccess kdb = new KDBAccess();
                        dt = kdb.GetHisRegByEnt("Atrib", "Atributos",
                            new string[2] { "value = iCodCatalogo", "{" + HttpContext.Current.Session["Language"] + "}" },
                            KeytiaWeb.UserInterface.MaestroEdit.getFiltroAtrib(tipoCampo) +
                            " and not iCodCatalogo in (" + Excluir + ") ",
                            "{" + HttpContext.Current.Session["Language"] + "}");
                        dt.Columns[1].ColumnName = "text";
                        break;
                }
                string json = DSOControl.SerializeJSON<DataTable>(dt);
                return json;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + lsNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsultar", e, lsTitulo);
            }
        }

        #endregion

    }
}
