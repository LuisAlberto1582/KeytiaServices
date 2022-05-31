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
using System.Reflection;

namespace KeytiaWeb.UserInterface
{
    public class KeytiaRelationField : KeytiaBaseField, IKeytiaFillableField, IKeytiaFillableAjaxField
    {
        protected DSOExpandable pExpGrid;
        protected DSOGrid pRelGrid;
        protected DSOWindow pWdAgregar;
        protected DSOWindow pWdEditor;
        protected DSOWindow pWdEliminador;
        protected Table pTablaAgregar;
        protected Table pTablaEditor;
        protected Table pTablaEliminador;
        protected HtmlButton pbtnAgregar;
        protected HtmlButton pbtnAceptar;
        protected HtmlButton pbtnAceptarAgregar;
        protected HtmlButton pbtnAceptarEliminar;
        protected HtmlButton pbtnCancelar;
        protected HtmlButton pbtnCancelarAgregar;
        protected HtmlButton pbtnCancelarEliminar;
        protected RelationFieldCollection pFieldsEditar;
        protected RelationFieldCollection pFieldsAdd;
        protected RelationFieldCollection pFieldsEliminador;
        protected string psColEntidad;
        protected string psRelationCollectionClass = "KeytiaWeb.UserInterface.RelationFieldCollection";

        protected DSOTextBox pDSOTxtRegAdd;

        public KeytiaRelationField() { }

        public virtual string RelationCollectionClass
        {
            get
            {
                return psRelationCollectionClass;
            }
            set
            {
                psRelationCollectionClass = value;
            }
        }

        public override object DataValue
        {
            get
            {
                return base.DataValue;
            }
            set
            {
                pRelGrid.AddClientEvent("iCodCatalogo", value.ToString());
            }
        }

        public override int ConfigValue
        {
            get
            {
                return pConfigValue;
            }
            set
            {
                pConfigValue = value;
                if (pConfigValue > 0)
                {
                    pConfigName = (string)DSODataAccess.ExecuteScalar("select vchDescripcion from Relaciones where iCodRegistro = " + pConfigValue);
                }
            }
        }

        public override bool ShowInGrid
        {
            get
            {
                return false;
            }
        }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pRelGrid;
            }
        }

        public virtual RelationFieldCollection Fields
        {
            get
            {
                return pFieldsAdd;
            }
        }

        public override void EnableField()
        {
            base.EnableField();
            pbtnAgregar.Visible = ValidarPermiso(Permiso.Agregar);
            pRelGrid.ClearEditedData();

            pRelGrid.AddClientEvent("EnableField", "1");
        }

        public override void DisableField()
        {
            //base.DisableField();
            pbtnAgregar.Visible = false;
            pRelGrid.ClearEditedData();

            pRelGrid.AddClientEvent("EnableField", "0");
        }

        public override void CreateField()
        {
            pExpGrid = new DSOExpandable();
            pRelGrid = new DSOGrid();
            pWdAgregar = new DSOWindow();
            pWdEditor = new DSOWindow();
            pWdEliminador = new DSOWindow();
            pTablaAgregar = new Table();
            pTablaEditor = new Table();
            pTablaEliminador = new Table();
            pbtnAgregar = new HtmlButton();
            pbtnAceptar = new HtmlButton();
            pbtnAceptarAgregar = new HtmlButton();
            pbtnAceptarEliminar = new HtmlButton();
            pbtnCancelar = new HtmlButton();
            pbtnCancelarAgregar = new HtmlButton();
            pbtnCancelarEliminar = new HtmlButton();
            pDSOTxtRegAdd = new DSOTextBox();

            pjsObj = pContainer.ID + "." + this.pColumn;

            InitDSOControlDB();
            InitWrapper();
            InitAgregar();
            InitEditor();
            InitEliminador();
            InitGrid();
        }

        protected void InitWrapper()
        {
            pExpGrid.ID = "wrapper";
            pExpGrid.StartOpen = true;
            pExpGrid.CreateControls();
            pRelGrid.Wrapper = pExpGrid;
            pExpGrid.Panel.Controls.Add(pWdAgregar);
            pExpGrid.Panel.Controls.Add(pWdEditor);
            pExpGrid.Panel.Controls.Add(pWdEliminador);
            pExpGrid.Panel.Controls.Add(pbtnAgregar);
            pbtnAgregar.ID = "btnAgregar";
            pbtnAgregar.Attributes["class"] = "buttonAdd";
        }

        protected RelationFieldCollection GetRelFields()
        {
            RelationFieldCollection lFields = (RelationFieldCollection)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(RelationFieldCollection)).CodeBase, psRelationCollectionClass).Unwrap();
            return lFields;
        }

        protected void InitAgregar()
        {
            pWdAgregar.ID = "wdAgregar";
            pWdAgregar.Width = 600;
            pWdAgregar.PositionLeft = 250;
            pWdAgregar.PositionTop = 200;
            pWdAgregar.Modal = true;
            pWdAgregar.InitOnReady = false;
            pWdAgregar.Visible = ValidarPermiso(Permiso.Agregar);

            pWdAgregar.CreateControls();
            pWdAgregar.Content.Controls.Add(pTablaAgregar);
            pWdAgregar.Content.Controls.Add(pbtnAceptarAgregar);
            pWdAgregar.Content.Controls.Add(pbtnCancelarAgregar);

            pbtnAceptarAgregar.ID = "btnAceptar";
            pbtnAceptarAgregar.Attributes["class"] = "buttonOK";

            pbtnCancelarAgregar.ID = "btnCancelar";
            pbtnCancelarAgregar.Attributes["class"] = "buttonCancel";

            pDSOTxtRegAdd.ID = "iCodRegistro";
            pDSOTxtRegAdd.DataField = "iCodRegistro";
            pWdAgregar.Content.Controls.Add(pDSOTxtRegAdd);
            pDSOTxtRegAdd.CreateControls();
            pDSOTxtRegAdd.TextBox.Style["display"] = "none";

            pTablaAgregar.ID = "TblAgregar";
            pTablaAgregar.CssClass = "RelEdit";
            pTablaAgregar.Width = Unit.Percentage(100);

            pFieldsAdd = GetRelFields();
            pFieldsAdd.InitCollection(pContainer, piCodEntidad, pConfigValue, pTablaAgregar, this.ValidarPermiso);
        }

        protected void InitEditor()
        {
            pWdEditor.ID = "wdEditor";
            pWdEditor.Width = 600;
            pWdEditor.PositionLeft = 250;
            pWdEditor.PositionTop = 200;
            pWdEditor.Modal = true;
            pWdEditor.InitOnReady = false;
            pWdEditor.Visible = ValidarPermiso(Permiso.Editar);

            pWdEditor.CreateControls();
            pWdEditor.Content.Controls.Add(pTablaEditor);
            pWdEditor.Content.Controls.Add(pbtnAceptar);
            pWdEditor.Content.Controls.Add(pbtnCancelar);

            pbtnAceptar.ID = "btnAceptar";
            pbtnAceptar.Attributes["class"] = "buttonOK";

            pbtnCancelar.ID = "btnCancelar";
            pbtnCancelar.Attributes["class"] = "buttonCancel";

            pTablaEditor.ID = "TblEdit";
            pTablaEditor.CssClass = "RelEdit";
            pTablaEditor.Width = Unit.Percentage(100);

            pFieldsEditar = GetRelFields();
            pFieldsEditar.InitCollection(pContainer, piCodEntidad, pConfigValue, pTablaEditor, this.ValidarPermiso);
        }

        protected void InitEliminador()
        {
            pWdEliminador.ID = "wdEliminador";
            pWdEliminador.Width = 600;
            pWdEliminador.PositionLeft = 250;
            pWdEliminador.PositionTop = 200;
            pWdEliminador.Modal = true;
            pWdEliminador.InitOnReady = false;
            pWdEliminador.Visible = ValidarPermiso(Permiso.Eliminar);

            pWdEliminador.CreateControls();
            pWdEliminador.Content.Controls.Add(pTablaEliminador);
            pWdEliminador.Content.Controls.Add(pbtnAceptarEliminar);
            pWdEliminador.Content.Controls.Add(pbtnCancelarEliminar);

            pbtnAceptarEliminar.ID = "btnAceptar";
            pbtnAceptarEliminar.Attributes["class"] = "buttonOK";

            pbtnCancelarEliminar.ID = "btnCancelar";
            pbtnCancelarEliminar.Attributes["class"] = "buttonCancel";

            pTablaEliminador.ID = "TblDelete";
            pTablaEliminador.CssClass = "RelEdit";
            pTablaEliminador.Width = Unit.Percentage(100);

            pFieldsEliminador = GetRelFields();
            pFieldsEliminador.InitCollection(pContainer, piCodEntidad, pConfigValue, pTablaEliminador, this.ValidarPermiso);
        }

        protected void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pRelGrid.Config.sDom = "<\"H\"l>tr<\"F\"p>"; ;
            pRelGrid.Config.bAutoWidth = true;
            pRelGrid.Config.sScrollX = "100%";
            pRelGrid.Config.sScrollY = "300px";
            pRelGrid.Config.sPaginationType = "full_numbers";
            pRelGrid.Config.bJQueryUI = true;
            pRelGrid.Config.bProcessing = true;
            pRelGrid.Config.bServerSide = true;
            pRelGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerData(this, sSource, aoData, fnCallback);}";
            pRelGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetRelData");
            pRelGrid.Config.fnInitComplete = "function(){" + pjsObj + ".fnInitComplete(this);}";

            lCol = new DSOGridClientColumn();
            lCol.sName = "Editar";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "50px";
            lCol.sClass = "TdEdit";
            lCol.bVisible = false;
            pRelGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "Eliminar";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "50px";
            lCol.sClass = "TdEdit";
            lCol.bVisible = false;
            pRelGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            pRelGrid.Config.aoColumnDefs.Add(lCol);

            //Columnas para guardar los valores
            foreach (KeytiaBaseField lField in pFieldsEditar)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = lField.Column;
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pRelGrid.Config.aoColumnDefs.Add(lCol);

                if (lField.Column.StartsWith("iCodCatalogo")
                    && lField.ConfigValue == piCodEntidad)
                {
                    psColEntidad = lField.Column;
                }
            }

            pRelGrid.Config.aaSorting.Add(new ArrayList());
            pRelGrid.Config.aaSorting[0].Add(lTarget);
            pRelGrid.Config.aaSorting[0].Add("asc");

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFecUltAct";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            lCol.bUseRendered = false;
            pRelGrid.Config.aoColumnDefs.Add(lCol);

            //Columnas para mostrar los valores
            foreach (KeytiaBaseField lField in pFieldsEditar)
            {
                if (lField.Column.StartsWith("iCodCatalogo")
                    || lField.Column.EndsWith("Vigencia"))
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = lField.Column + "Display";
                    lCol.aTargets.Add(lTarget++);
                    lCol.bVisible = true;

                    if (lField is KeytiaDateTimeField)
                    {
                        lCol.sWidth = "160px";
                    }

                    pRelGrid.Config.aoColumnDefs.Add(lCol);
                }
            }

            DataTable lMetaData = DSODataAccess.GetSchema("Relaciones");
            lMetaData.Columns.Add("Editar", typeof(int));
            lMetaData.Columns.Add("Eliminar", typeof(int));
            pRelGrid.MetaData = lMetaData;
        }

        public override void InitField()
        {
            base.InitField();

            pRelGrid.AddClientEvent("iCodEntidad", piCodEntidad.ToString());
            pRelGrid.AddClientEvent("iCodRelacion", pConfigValue.ToString());
            pRelGrid.AddClientEvent("buttonAdd", "#" + pbtnAgregar.ClientID);
            pRelGrid.AddClientEvent("colEntidad", psColEntidad);
            pExpGrid.OnOpen = "function(){" + pjsObj + ".fnAdjustColumnSizing();}";

            pFieldsAdd.InitFields();
            pFieldsAdd[psColEntidad].DisableField();

            pFieldsEditar.InitFields();
            pFieldsEditar[psColEntidad].DisableField();

            foreach (KeytiaBaseField lField in pFieldsAdd)
            {
                if (lField is KeytiaAutoCompleteField && lField.ConfigValue != this.iCodEntidad)
                {
                    HtmlButton lbtnSubHistorico = new HtmlButton();
                    lbtnSubHistorico.ID = "btnSubHistorico" + lField.Column;
                    lbtnSubHistorico.Attributes["class"] = "buttonAddImg";
                    lbtnSubHistorico.Attributes["DataField"] = lField.Column;
                    lbtnSubHistorico.InnerText = "...";
                    lbtnSubHistorico.ServerClick += new EventHandler(lbtnSubHistorico_ServerClick);
                    //RZ.20140131 Se oculta boton para agregar un subhistorico a petición de caso 491956000001670009
                    //RZ.20140409 Se vuelve a habilitar este boton ya que impacta en la definicion de rutas de consultas en reportes
                    lbtnSubHistorico.Visible = true;

                    lField.DSOControlDB.TcCtl.Controls.Add(lbtnSubHistorico);
                }
            }

            if (ValidarPermiso(Permiso.Administrar))
            {
                pFieldsEditar["dtIniVigencia"].EnableField();
                pFieldsEditar["dtFinVigencia"].EnableField();
                pFieldsAdd["dtIniVigencia"].EnableField();
                pFieldsAdd["dtFinVigencia"].EnableField();
            }
            else
            {
                pFieldsEditar["dtIniVigencia"].DisableField();
                pFieldsEditar["dtFinVigencia"].DisableField();
                pFieldsAdd["dtIniVigencia"].DisableField();
                pFieldsAdd["dtFinVigencia"].DisableField();
            }

            pFieldsEliminador.InitFields();
            pFieldsEliminador.DisableFields();
            pFieldsEliminador["dtFinVigencia"].EnableField();

            pWdEditor.Height = Math.Min(600, 30 * (pTablaEditor.Rows.Count + 2));
            pWdEliminador.Height = pWdEditor.Height;

            pWdAgregar.Height = pWdEditor.Height;

            pWdEditor.OnWindowClose = "function(){ " + pjsObj + ".cancel($('#" + pWdEditor.Content.ClientID + "')); }";
            pWdEliminador.OnWindowClose = "function(){ " + pjsObj + ".cancel($('#" + pWdEliminador.Content.ClientID + "')); }";

            pWdAgregar.OnWindowClose = "function(){ " + pjsObj + ".cancel($('#" + pWdAgregar.Content.ClientID + "')); }";

            if (((HistoricEdit)pContainer).State == HistoricState.SubHistoricoRel
                && pContainer.ID + this.Column + "SubHis" == ((HistoricEdit)pContainer).SubHistoricoID)
            {
                HistoricEdit lSubHistorico = ((HistoricEdit)pContainer).SubHistorico;
                KeytiaBaseField lField = pFieldsAdd.GetByConfigValue(int.Parse(lSubHistorico.iCodEntidad));
                lSubHistorico.SetEntidad(lField.ConfigName);
                string lsMaestro = lSubHistorico.GetMaestro(lField.ConfigValue);
                if (!String.IsNullOrEmpty(lsMaestro))
                {
                    lSubHistorico.SetMaestro(lsMaestro);
                }

                lSubHistorico.PostConsultarClick += new EventHandler(lSubHistorico_PostConsultarClick);
                lSubHistorico.PostGrabarClick += new EventHandler(lSubHistorico_PostGrabarClick);
                lSubHistorico.PostCancelarClick += new EventHandler(lSubHistorico_PostCancelarClick);
                lSubHistorico.SetHistoricState(lSubHistorico.State);
            }

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){" + pjsObj + " = new Relacion(\"#" + pContainer.ClientID + "\",\"#" + pRelGrid.Grid.ClientID + "\",\"#" + pWdAgregar.Content.ClientID + "\",\"#" + pWdEditor.Content.ClientID + "\",\"#" + pWdEliminador.Content.ClientID + "\");});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaRelationField), pjsObj + "New", lsb.ToString(), true, false);
        }

        protected virtual void lSubHistorico_PostCancelarClick(object sender, EventArgs e)
        {
            HistoricEdit lHistorico = (HistoricEdit)pContainer;
            HistoricEdit lSubHistorico = lHistorico.SubHistorico;

            if ((lSubHistorico.PrevState == HistoricState.Inicio || lSubHistorico.PrevState == HistoricState.MaestroSeleccionado)
                && (lSubHistorico.State == HistoricState.Inicio || lSubHistorico.State == HistoricState.MaestroSeleccionado))
            {
                lHistorico.RemoverSubHistorico();

                StringBuilder lsb = new StringBuilder();
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".InitAdd();");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaRelationField), pjsObj + ".InitAdd", lsb.ToString(), false, false);
            }
        }

        protected virtual void lSubHistorico_PostGrabarClick(object sender, EventArgs e)
        {
            HistoricEdit lHistorico = (HistoricEdit)pContainer;
            HistoricEdit lSubHistorico = lHistorico.SubHistorico;
            if (lSubHistorico.State != HistoricState.Edicion)
            {
                lSubHistorico.PrevState = lSubHistorico.State;
                lSubHistorico.SetHistoricState(HistoricState.Consulta);
                lSubHistorico_PostConsultarClick(sender, e);
            }
        }

        protected virtual void lSubHistorico_PostConsultarClick(object sender, EventArgs e)
        {
            HistoricEdit lHistorico = (HistoricEdit)pContainer;
            HistoricEdit lSubHistorico = lHistorico.SubHistorico;
            KeytiaBaseField lField = pFieldsAdd.GetByConfigValue(int.Parse(lSubHistorico.iCodEntidad));
            if (lSubHistorico.State == HistoricState.Consulta)
            {
                lField.DataValue = lSubHistorico.iCodCatalogo;
            }
            else
            {
                lField.DataValue = DBNull.Value;
            }
            lHistorico.RemoverSubHistorico();

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + ".InitAdd();");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaRelationField), pjsObj + ".InitAdd", lsb.ToString(), false, false);
        }

        protected virtual void lbtnSubHistorico_ServerClick(object sender, EventArgs e)
        {
            KeytiaBaseField lField = pFieldsAdd[((HtmlButton)sender).Attributes["DataField"]];
            HistoricEdit lHistorico = (HistoricEdit)pContainer;
            HistoricEdit lSubHistorico;
            lHistorico.PrevState = lHistorico.State;
            if (lField.SubHistoricClass != "KeytiaWeb.UserInterface.HistoricEdit")
            {
                lHistorico.SubHistoricClass = lField.SubHistoricClass;
            }
            else
            {
                lHistorico.SubHistoricClass = this.SubHistoricClass;
            }
            if (lField.SubCollectionClass != "KeytiaWeb.UserInterface.HistoricFieldCollection")
            {
                lHistorico.SubCollectionClass = lField.SubCollectionClass;
            }
            else
            {
                lHistorico.SubCollectionClass = this.SubCollectionClass;
            }
            lHistorico.InitSubHistorico(pContainer.ID + this.Column + "SubHis");
            lHistorico.SetHistoricState(HistoricState.SubHistoricoRel);
            lSubHistorico = lHistorico.SubHistorico;
            lSubHistorico.SetEntidad(lField.ConfigName);
            string lsMaestro = lSubHistorico.GetMaestro(lField.ConfigValue);
            if (!String.IsNullOrEmpty(lsMaestro))
            {
                lSubHistorico.SetMaestro(lsMaestro);
            }
            lSubHistorico.EsSubHistorico = true;
            lSubHistorico.FillControls();
            lSubHistorico.SetHistoricState(HistoricState.Inicio);
        }

        public override void InitLanguage()
        {
            pRelGrid.Config.oLanguage = Globals.GetGridLanguage();
            pDescripcion = "";

            pWdAgregar.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "WdNuevoRegistro"));
            pWdEditor.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "WdEditarRegistro"));
            pWdEliminador.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarBaja"));

            pbtnAgregar.InnerText = Globals.GetMsgWeb("btnAgregar");
            pbtnAgregar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".edit();return false;";

            pbtnAceptar.InnerText = Globals.GetMsgWeb("btnAceptar");
            pbtnAceptar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".saveEdit();return false;";

            pbtnCancelar.InnerText = Globals.GetMsgWeb("btnCancelar");
            pbtnCancelar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".End(" + pjsObj + ".$wdEdit);return false;";

            pbtnAceptarAgregar.InnerText = Globals.GetMsgWeb("btnAceptar");
            pbtnAceptarAgregar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".save();return false;";

            pbtnCancelarAgregar.InnerText = Globals.GetMsgWeb("btnCancelar");
            pbtnCancelarAgregar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".End(" + pjsObj + ".$wdAdd);return false;";

            pbtnAceptarEliminar.InnerText = Globals.GetMsgWeb("btnAceptar");
            pbtnAceptarEliminar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".saveDelete();return false;";

            pbtnCancelarEliminar.InnerText = Globals.GetMsgWeb("btnCancelar");
            pbtnCancelarEliminar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".End(" + pjsObj + ".$wdDel);return false;";

            KeytiaBaseField lField;
            pFieldsEditar.InitLanguage();
            pFieldsAdd.InitLanguage();
            pFieldsEliminador.InitLanguage();

            foreach (DSOGridClientColumn lCol in pRelGrid.Config.aoColumnDefs)
            {
                if (lCol.sName.EndsWith("Display") && pFieldsEditar.Contains(lCol.sName.Replace("Display", "")))
                {
                    lField = pFieldsEditar[lCol.sName.Replace("Display", "")];
                    pDescripcion += lField.Column.StartsWith("iCodCatalogo") ? (pDescripcion == "" ? lField.Descripcion : " - " + lField.Descripcion) : "";
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName == "iCodRegistro")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RegistroRel"));
                }
                else if (lCol.sName == "Editar")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnEditar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"Editar\",\"" + pjsObj + ".edit\",\"custom-img-edit\",\"" + pContainer.ResolveUrl("~/images/pencilsmall.png") + "\");}";
                }
                else if (lCol.sName == "Eliminar")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnBaja"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"Eliminar\",\"" + pjsObj + ".deleteEdit\",\"custom-img-delete\",\"" + pContainer.ResolveUrl("~/images/deletesmall.png") + "\");}";
                }
            }

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine("RelMsgs.confirm = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarCambios")) + "\";");
            lsb.AppendLine("RelMsgs.confirmTitle = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarTitulo")) + "\";");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricFieldCollection), "RelMsgs", lsb.ToString(), true, false);

            lsb.Length = 0;
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + ".name = \"" + DSOControl.JScriptEncode(pDescripcion) + "\";");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricFieldCollection), pjsObj + ".name", lsb.ToString(), true, false);


            if (pRelGrid.GetClientEvent("EnableField") == "1")
            {
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){ try { $('#" + pRelGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "0" + "," + ValidarPermiso(Permiso.Editar).ToString().ToLower() + "); } catch(e){ } });");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaRelationField), pjsObj + "Editar", lsb.ToString(), false, false);

                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){ try{ $('#" + pRelGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "1" + "," + ValidarPermiso(Permiso.Eliminar).ToString().ToLower() + "); } catch(e){ } });");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaRelationField), pjsObj + "Eliminar", lsb.ToString(), false, false);
            }
            else
            {
                lsb.Length = 0;
                lsb.Append("<script type='text/javascript'>");
                lsb.Append("jQuery(function($){ try{ $('#" + pRelGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "0" + ",false); } catch(e){ } });");
                lsb.Append("</script>");

                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaRelationField), pjsObj + "Editar", lsb.ToString(), false, false);

                lsb.Length = 0;
                lsb.Append("<script type='text/javascript'>");
                lsb.Append("jQuery(function($){ try{ $('#" + pRelGrid.Grid.ClientID + "').dataTable({bRetrieve:true}).fnSetColumnVis(" + "1" + ",false); } catch(e){ } });");
                lsb.Append("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaRelationField), pjsObj + "Eliminar", lsb.ToString(), false, false);
            }
        }

        public void Fill()
        {
            pRelGrid.Fill();
            pFieldsEditar.FillControls();
            pFieldsAdd.FillControls();
            pFieldsEliminador.FillControls();
        }

        public void FillAjax()
        {
            psLang = "{" + Globals.GetCurrentLanguage() + "}";
            pFieldsEditar.FillAjaxControls();
            pFieldsAdd.FillAjaxControls();
            pFieldsEliminador.FillAjaxControls();
        }

        public static DataTable GetRelByAtrib(string lsEntidad)
        {
            int liCodEntidad = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = '" + lsEntidad.Replace("'", "''") + "'");
            return DSODataAccess.Execute("select iCodRelacion from [" + DSODataContext.Schema + "].GetRelByAtrib(" + liCodEntidad + ")");
        }
    }
}
