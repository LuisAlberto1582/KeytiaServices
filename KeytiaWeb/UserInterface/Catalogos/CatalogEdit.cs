/*
Nombre:		    JCMS
Fecha:		    2011-05-22
Descripción:	Control para acutalizar la lista de entidades.
Modificación:	
*/
using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Collections;
using System.Collections.Generic;

namespace KeytiaWeb.UserInterface
{
    public class CatalogEdit : Panel, INamingContainer, IPostBackEventHandler
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected StringBuilder psbQuery = new StringBuilder();

        protected DSOWindow pWdEditor;
        protected HtmlButton pbtnAgregar;
        protected HtmlButton pbtnGrabar;
        protected HtmlButton pbtnCancelar;
        protected HtmlButton pbtnBaja;
        protected DSOGrid pCatGrid;
        protected Table pTblEditor;
        protected DSOTextBox ptxtCodigo;
        protected DSODateTimeBox pdtIniV;
        protected DSODateTimeBox pdtFinV;
        protected DSOTextBox ptxtDescripcion;
        protected DSOTextBox ptxtRegistro;

        protected DSOExpandable pExpAtributos;
        protected Table pTablaAtributos;
        protected HistoricFieldCollection pFields;

        protected string psFileKey;
        protected string psTempPath;

        protected Hashtable phtValues;

        public CatalogEdit()
        {
            psFileKey = Guid.NewGuid().ToString();
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
            System.IO.Directory.CreateDirectory(psTempPath);

            Init += new EventHandler(CatalogEdit_Init);
            Load += new EventHandler(CatalogEdit_Load);
        }

        protected void CatalogEdit_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillGrid();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            string lsConfirmarTitulo = Globals.GetMsgWeb("ConfirmarTitulo");
            string lsConfirmarCambios = Globals.GetMsgWeb("ConfirmarCambios");
            string lsScript = @"<script type=""text/javascript"">Cat.confirmarTitulo=""{0}"";Cat.confirmarCambios=""{1}""</script>";
            DSOControl.LoadControlScriptBlock(Page, typeof(CatalogEdit), "CatMsg", String.Format(lsScript, lsConfirmarTitulo, lsConfirmarCambios), true, false);
        }

        protected void FillGrid()
        {
            EnsureChildControls();

            try
            {
                pCatGrid.Fill();
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCatalogos");
                throw new KeytiaWebException("ErrGridData", e, lsTitulo);
            }
        }

        protected void CatalogEdit_Init(object sender, EventArgs e)
        {
            pWdEditor = new DSOWindow();
            pTblEditor = new Table();
            ptxtCodigo = new DSOTextBox();
            pdtIniV = new DSODateTimeBox();
            pdtFinV = new DSODateTimeBox();
            ptxtDescripcion = new DSOTextBox();
            ptxtRegistro = new DSOTextBox();
            pbtnGrabar = new HtmlButton();
            pbtnCancelar = new HtmlButton();
            pbtnBaja = new HtmlButton();
            pbtnAgregar = new HtmlButton();
            pCatGrid = new DSOGrid();

            pExpAtributos = new DSOExpandable();
            pTablaAtributos = new Table();

            this.CssClass = "CatalogEdit"; //NZ
            this.ID = "CatalogEdit";
            Controls.Add(pbtnAgregar);
            Controls.Add(pCatGrid);
            Controls.Add(pWdEditor);
        }

        protected override void CreateChildControls()
        {
            ChildControlsCreated = false;
            try
            {
                InitEditor();
                InitGrid();
                InitAcciones();
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrInitControls", e);
            }
            ChildControlsCreated = true;
        }

        protected void InitEditor()
        {
            string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "borrar");
            pWdEditor.ID = "wdEditor";
            pWdEditor.Width = 900;
            pWdEditor.PositionLeft = 100;
            pWdEditor.PositionTop = 100;
            pWdEditor.Modal = true;
            pWdEditor.InitOnReady = false;
            pWdEditor.OnWindowClose = "Cat.cancelar";
            pWdEditor.CreateControls();
            pWdEditor.Content.Controls.Add(pTblEditor);
            pWdEditor.Content.Controls.Add(pExpAtributos);

            pTblEditor.ID = "tblEdit";
            pTblEditor.Width = Unit.Percentage(100);

            pExpAtributos.ID = "AtribWrapper";
            pExpAtributos.StartOpen = true;
            pExpAtributos.CreateControls();
            pExpAtributos.Panel.Controls.Clear();
            pExpAtributos.Panel.Controls.Add(pTablaAtributos);

            pTablaAtributos.Controls.Clear();
            pTablaAtributos.ID = "Atributos";
            pTablaAtributos.Width = Unit.Percentage(100);

            pFields = new HistoricFieldCollection(this, 0, 1, pTablaAtributos, this.ValidarPermiso);
            pFields.InitFields();
            pFields.EnableFields();

            pWdEditor.Height = 100 + Math.Min(600, 30 * (pTablaAtributos.Rows.Count + 2));

            ptxtCodigo.ID = "vchCodigo";
            ptxtCodigo.DataField = "vchCodigo";
            ptxtCodigo.Table = pTblEditor;
            ptxtCodigo.Row = 1;
            ptxtCodigo.CreateControls();
            ptxtCodigo.TextBox.MaxLength = 40;
            ptxtCodigo.AddClientEvent("onchange", "Cat.setConfirmar();");

            pdtIniV.ID = "dtIniVigencia";
            pdtIniV.DataField = "dtIniVigencia";
            pdtIniV.Table = pTblEditor;
            pdtIniV.Row = 2;
            pdtIniV.ShowHour = false;
            pdtIniV.ShowMinute = false;
            pdtIniV.ShowSecond = false;
            pdtIniV.CreateControls();
            pdtIniV.AddClientEvent("onchange", "Cat.setConfirmar();");

            pdtFinV.ID = "dtFinVigencia";
            pdtFinV.DataField = "dtFinVigencia";
            pdtFinV.Table = pTblEditor;
            pdtFinV.Row = 2;
            pdtFinV.ShowHour = false;
            pdtFinV.ShowMinute = false;
            pdtFinV.ShowSecond = false;
            pdtFinV.CreateControls();
            pdtFinV.AddClientEvent("onchange", "Cat.setConfirmar();");

            ((TableRow)pdtFinV.TcCtl.Parent).Style["display"] = "none";

            ptxtDescripcion.ID = "vchDescripcion";
            ptxtDescripcion.DataField = "vchDescripcion";
            ptxtDescripcion.Table = pTblEditor;
            ptxtDescripcion.Row = 1;
            ptxtDescripcion.CreateControls();
            ptxtDescripcion.TextBox.MaxLength = 160;
            ptxtDescripcion.AddClientEvent("onchange", "Cat.setConfirmar();");

            ptxtRegistro.ID = "iCodRegistro";
            ptxtRegistro.DataField = "iCodRegistro";
            ptxtRegistro.CreateControls();
            ptxtRegistro.DataValueDelimiter = "";
            ptxtRegistro.TextBox.Style["display"] = "none";
            pWdEditor.Content.Controls.Add(ptxtRegistro);

            pbtnGrabar.ID = "btnGrabar";
            pbtnGrabar.ServerClick += new EventHandler(btnGrabar_Click);
            pbtnGrabar.Attributes["class"] = "buttonSave";
            pbtnGrabar.Style["display"] = "none";

            pbtnCancelar.ID = "btnCancelar";
            pbtnCancelar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = "Cat.finEditar();return false;";
            pbtnCancelar.Attributes["class"] = "buttonCancel";
            pbtnCancelar.Style["display"] = "none";

            pbtnBaja.ID = "btnBaja";
            pbtnBaja.Attributes["btnBaja"] = "true";
            pbtnBaja.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = "Cat.borrar('" + Globals.GetMsgWeb("ConfirmarBaja") + "','" + Globals.GetMsgWeb("ConfirmarTitulo") + "',function(){" + lsdoPostBack + "});return false;";
            pbtnBaja.Visible = ValidarPermiso(Permiso.Eliminar);
            pbtnBaja.Attributes["class"] = "buttonDelete";
            pbtnBaja.Style["display"] = "none";

            pWdEditor.Content.Controls.Add(pbtnGrabar);
            pWdEditor.Content.Controls.Add(pbtnCancelar);
            pWdEditor.Content.Controls.Add(pbtnBaja);
        }

        protected void InitGrid()
        {
            pCatGrid.ID = "CatGrid";
            pCatGrid.CreateControls();

            if (!Page.IsPostBack)
            {
                DSOGridClientColumn lCol;
                int lTarget = 0;
                bool lbEditar = ValidarPermiso(Permiso.Editar);
                pCatGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>";
                pCatGrid.Config.bAutoWidth = true;
                pCatGrid.Config.sScrollX = "100%";
                pCatGrid.Config.sScrollY = "400px";
                pCatGrid.Config.sPaginationType = "full_numbers";
                pCatGrid.Config.bJQueryUI = true;
                pCatGrid.Config.bProcessing = true;
                pCatGrid.Config.bServerSide = true;
                pCatGrid.Config.fnServerData = "Cat.fnServerData";
                pCatGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetCatData");
                pCatGrid.Config.fnInitComplete = "Cat.fnInitComplete";

                if (lbEditar)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = "iCodRegistroEditar";
                    lCol.sWidth = "50px";
                    lCol.aTargets.Add(lTarget++);
                    lCol.sClass = "TdEdit";
                    pCatGrid.Config.aoColumnDefs.Add(lCol);
                }

                lCol = new DSOGridClientColumn();
                lCol.sName = "vchCodigo";
                lCol.aTargets.Add(lTarget++);
                pCatGrid.Config.aoColumnDefs.Add(lCol);

                pCatGrid.Config.aaSorting.Add(new ArrayList());
                pCatGrid.Config.aaSorting[0].Add(lTarget);
                pCatGrid.Config.aaSorting[0].Add("asc");

                lCol = new DSOGridClientColumn();
                lCol.sName = "vchDescripcion";
                lCol.aTargets.Add(lTarget++);
                pCatGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtIniVigencia";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "150px";
                pCatGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFinVigencia";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "150px";
                pCatGrid.Config.aoColumnDefs.Add(lCol);

                if (lbEditar)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = "iCodRegistro";
                    lCol.aTargets.Add(lTarget++);
                    lCol.bVisible = false;
                    pCatGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected void InitAcciones()
        {
            pbtnAgregar.ID = "btnAgregar";
            pbtnAgregar.Attributes["class"] = "buttonAdd";
            pbtnAgregar.Style["display"] = "none";
            pbtnAgregar.Visible = ValidarPermiso(Permiso.Agregar);
        }

        protected void btnGrabar_Click(object sender, EventArgs e)
        {
            if (ValidarRegistro())
            {
                try
                {
                    int liCodRegistro;
                    KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();

                    phtValues = pFields.GetValues();
                    phtValues.Add("iCodEntidad", 0);
                    phtValues.Add("iCodMaestro", 1);
                    phtValues.Add("dtIniVigencia", pdtIniV.Date);
                    phtValues.Add("dtFinVigencia", pdtFinV.Date);
                    phtValues.Add("iCodUsuario", Session["iCodUsuario"].ToString());

                    if (ptxtRegistro.DataValue.ToString() == "null")
                    {
                        phtValues.Add("vchCodigo", ptxtCodigo.DataValue);
                        phtValues.Add("vchDescripcion", ptxtDescripcion.DataValue);

                        liCodRegistro = cargasCOM.InsertaRegistro(phtValues, "Historicos", "", "", false, (int)Session["iCodUsuarioDB"], true);

                        if (liCodRegistro < 0)
                        {
                            throw new KeytiaWebException("ErrSaveRecord");
                        }
                    }
                    else
                    {
                        bool lbActualizacionExitosa = false;
                        bool lbActualizarHistoria = int.Parse(DSODataAccess.ExecuteScalar("select isnull(bActualizaHistoria,0) from Maestros where iCodRegistro = " + 1).ToString()) == 1;

                        liCodRegistro = int.Parse(ptxtRegistro.DataValue.ToString());
                        phtValues.Add("iCodCatalogo", liCodRegistro);
                        phtValues.Add("bActualizaHistoria", lbActualizarHistoria);

                        liCodRegistro = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Historicos where iCodMaestro = 1 and iCodCatalogo = " + liCodRegistro);
                        lbActualizacionExitosa = cargasCOM.ActualizaRegistro("Historicos", "", "", phtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"], true);

                        if (!lbActualizacionExitosa)
                        {
                            throw new KeytiaWebException("ErrSaveRecord");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException("ErrSaveRecord", ex);
                }
            }
        }

        protected bool ValidarRegistro()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError = "";
            string lsCodRegistro;
            string lsScript;
            DataRow lRow = null;
            int liCount;

            try
            {
                if (ptxtRegistro.DataValue.ToString() == "null")
                {
                    lsCodRegistro = "0";
                    lsScript = "<script type='text/javascript'>$(document).ready(function() { Cat.initEditor('#" + pWdEditor.Content.ClientID + "',true);});</script>";
                    pWdEditor.Title = Globals.GetMsgWeb("WdNuevoRegistro");
                }
                else
                {
                    lsCodRegistro = ptxtRegistro.DataValue.ToString();
                    lsScript = "<script type='text/javascript'>$(document).ready(function() { Cat.initEditor('#" + pWdEditor.Content.ClientID + "',false);});</script>";
                    pWdEditor.Title = Globals.GetMsgWeb("WdEditarRegistro");
                    lRow = DSODataAccess.ExecuteDataRow("select * from Catalogos where iCodRegistro = " + lsCodRegistro);
                    ptxtCodigo.DataValue = lRow["vchCodigo"].ToString();
                    ptxtDescripcion.DataValue = lRow["vchDescripcion"].ToString();
                }

                //Validar que no existan otros catalogos con el mismo vchCodigo
                liCount = (int)DSODataAccess.ExecuteScalar("select count(iCodRegistro) from Catalogos where dtIniVigencia <> dtFinVigencia and iCodCatalogo is null and iCodRegistro <> " + lsCodRegistro + " and vchCodigo = " + ptxtCodigo.DataValue);
                if (liCount > 0)
                {
                    lsError = Globals.GetMsgWeb("CampoRepetido", ptxtCodigo.Descripcion);
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                //Validar que no existan otros catalogos con la misma descripción
                liCount = (int)DSODataAccess.ExecuteScalar("select count(iCodRegistro) from Catalogos where dtIniVigencia <> dtFinVigencia and iCodCatalogo is null and iCodRegistro <> " + lsCodRegistro + " and vchDescripcion = " + ptxtDescripcion.DataValue);
                if (liCount > 0)
                {
                    lsError = Globals.GetMsgWeb("CampoRepetido", ptxtDescripcion.Descripcion);
                    lsbErrores.Append("<li>" + lsError + "</li>");

                }

                //Validar que el vchCodigo no este vacio
                if (!ptxtCodigo.HasValue)
                {
                    lsError = Globals.GetMsgWeb("CampoRequerido", ptxtCodigo.Descripcion);
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                //Validar que vchDescripcion no este vacio
                if (!ptxtDescripcion.HasValue)
                {
                    lsError = Globals.GetMsgWeb("CampoRequerido", ptxtDescripcion.Descripcion);
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                //Validar que fin de vigencia sea mayor o igual a inicio de vigencia
                if (pdtIniV.HasValue && pdtFinV.HasValue && pdtIniV.Date > pdtFinV.Date)
                {
                    lsError = Globals.GetMsgWeb("VigenciaFin", pdtIniV.Descripcion, pdtFinV.Descripcion);
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.LoadControlScriptBlock(Page, typeof(CatalogEdit), "Cat.initEditor", lsScript, true, false);
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCatalogos");
                    DSOControl.jAlert(Page, "Cat.ValidarRegistro", lsError, lsTitulo);
                }
                else
                {
                    //Si no se proporcionaron valores para las vigencias entonces establezco los valores default
                    if (!pdtIniV.HasValue)
                    {
                        pdtIniV.DataValue = DateTime.Today;
                    }
                    if (!pdtFinV.HasValue)
                    {
                        pdtFinV.DataValue = new DateTime(2079, 1, 1);
                    }
                }

                return lbret;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrValidateRecord", e);
            }
        }

        protected void BorrarRegistro()
        {
            try
            {
                if (ValidarEliminacion())
                {
                    pdtFinV.DataValue = pdtIniV.Date;

                    bool lbActualizacionExitosa = false;

                    phtValues = new Hashtable();
                    phtValues.Add("iCodEntidad", 0);
                    phtValues.Add("iCodMaestro", 1);
                    phtValues.Add("vchCodigo", ptxtCodigo.DataValue);
                    phtValues.Add("vchDescripcion", ptxtDescripcion.DataValue);
                    phtValues.Add("dtIniVigencia", pdtIniV.Date);
                    phtValues.Add("dtFinVigencia", pdtFinV.Date);
                    phtValues.Add("iCodUsuario", Session["iCodUsuario"].ToString());

                    KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
                    int liCodRegistro = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Historicos where iCodCatalogo = " + int.Parse(ptxtRegistro.TextBox.Text));

                    //lbActualizacionExitosa = cargasCOM.ActualizaRegistro("Historicos", phtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"]);
                    lbActualizacionExitosa = cargasCOM.ActualizaRegistro("Historicos", "", "", phtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"], true);
                    if (!lbActualizacionExitosa)
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                }
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrDeleteRecord", e);
            }
        }

        protected bool ValidarEliminacion()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError = "";
            string lsCodRegistro = ptxtRegistro.DataValue.ToString();
            string lsScript = "<script type='text/javascript'>$(document).ready(function() { Cat.initEditor('#" + pWdEditor.Content.ClientID + "',false);});</script>";
            int liCount;
            DataRow lRow = DSODataAccess.ExecuteDataRow("select * from Catalogos where iCodRegistro = " + lsCodRegistro);
            ptxtCodigo.DataValue = lRow["vchCodigo"].ToString();
            ptxtDescripcion.DataValue = lRow["vchDescripcion"].ToString();

            try
            {
                liCount = (int)DSODataAccess.ExecuteScalar("select cuenta = count(*) from Historicos where dtIniVigencia <> dtFinVigencia and iCodCatalogo in(select iCodRegistro from Catalogos where iCodCatalogo = " + lsCodRegistro +")");
                if (liCount > 0)
                {
                    lsError = Globals.GetMsgWeb("CatElementosEntidad");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                liCount = (int)DSODataAccess.ExecuteScalar("select cuenta = count(*) from Maestros where dtIniVigencia <> dtFinVigencia and iCodEntidad = " + lsCodRegistro);
                if (liCount > 0)
                {
                    lsError = Globals.GetMsgWeb("MaeConfigEntidad");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                liCount = (int)DSODataAccess.ExecuteScalar("select cuenta = count(*) from " + DSODataContext.Schema + ".GetMaeByAtrib(" + lsCodRegistro + ")");
                if (liCount > 0)
                {
                    lsError = Globals.GetMsgWeb("MaeConfigColumna");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                liCount = (int)DSODataAccess.ExecuteScalar("select cuenta = count(*) from " + DSODataContext.Schema + ".GetRelByAtrib(" + lsCodRegistro + ")");
                if (liCount > 0)
                {
                    lsError = Globals.GetMsgWeb("RelConfigColumna");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.LoadControlScriptBlock(Page, typeof(CatalogEdit), "Cat.initEditor", lsScript, true, false);
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCatalogos");
                    DSOControl.jAlert(Page, "Cat.ValidarEliminacion", lsError, lsTitulo);
                }
                else
                {
                    if (!pdtIniV.HasValue)
                    {
                        pdtIniV.DataValue = DateTime.Today;
                    }
                }
                return lbret;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrValidateRecord", e);
            }
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "borrar")
            {
                BorrarRegistro();
            }
        }

        #endregion

        protected DataTable GetExportData()
        {
            DataTable ldt;
            string lsCodigo = "\"" + Globals.GetMsgWeb(false, "vchCodigo").Replace("\"", "\"\"") + "\"";
            string lsIniV = "\"" + Globals.GetMsgWeb(false, "dtIniVigencia").Replace("\"", "\"\"") + "\"";
            string lsFinV = "\"" + Globals.GetMsgWeb(false, "dtFinVigencia").Replace("\"", "\"\"") + "\"";
            string lsDesc = "\"" + Globals.GetMsgWeb(false, "vchDescripcion").Replace("\"", "\"\"") + "\"";

            psbQuery.Length = 0;
            psbQuery.AppendLine("select ");
            psbQuery.AppendLine("	" + lsCodigo + "= a.vchCodigo,");
            psbQuery.AppendLine("	" + lsIniV + "= a.dtIniVigencia,");
            psbQuery.AppendLine("	" + lsFinV + "= a.dtFinVigencia,");
            psbQuery.AppendLine("	" + lsDesc + "= a.vchDescripcion");
            psbQuery.AppendLine("from Catalogos a");
            psbQuery.AppendLine("where a.iCodCatalogo is null");
            psbQuery.AppendLine("and   a.dtIniVigencia <> a.dtFinVigencia");
            psbQuery.AppendLine("order by a.vchDescripcion");

            ldt = DSODataAccess.Execute(psbQuery.ToString());
            return ldt;
        }

        public void ExportXLS()
        {
            ExcelAccess lExcel = new ExcelAccess();

            try
            {
                DataTable ldt = GetExportData();
                string lsHoja0;
                object[,] loColumnas = new object[1, 4];
                object[,] loData = lExcel.DataTableToArray(ldt);

                loColumnas[0, 0] = ldt.Columns[0].ColumnName;
                loColumnas[0, 1] = ldt.Columns[1].ColumnName;
                loColumnas[0, 2] = ldt.Columns[2].ColumnName;
                loColumnas[0, 3] = ldt.Columns[3].ColumnName;

                lExcel.Abrir();
                lsHoja0 = lExcel.NombreHoja0();
                lExcel.Actualizar(lsHoja0, 1, 1, loColumnas.GetUpperBound(0) + 1, loColumnas.GetUpperBound(1) + 1, loColumnas);
                lExcel.Actualizar(lsHoja0, 2, 1, loData.GetUpperBound(0) + 2, loData.GetUpperBound(1) + 1, loData);
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

        public void ExportDOC()
        {
            CrearDOC(".docx");
        }

        protected void CrearDOC(string lsExt)
        {
            WordAccess lWord = new WordAccess();

            try
            {
                DataTable ldt = GetExportData();

                lWord.Abrir();
                lWord.InsertarTabla(ldt, true);
                lWord.Tabla.Columns.AutoFit();
                lWord.FilePath = GetFileName(lsExt);
                lWord.SalvarComo();

                ExportarArchivo(lsExt);
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, lsExt);
            }
            finally
            {
                if (lWord != null)
                {
                    lWord.Cerrar(true);
                    lWord = null;
                }
            }
        }

        public void ExportPDF()
        {
            CrearDOC(".pdf");
        }

        public void ExportCSV()
        {
            TxtFileAccess lTxt = new TxtFileAccess();

            try
            {
                DataTable ldt = GetExportData();
                int li;
                int lj;
                List<string> lstValores = new List<string>();
                string lsDateFormat = Globals.GetMsgWeb(false, "NetDateTimeFormat");

                lTxt.FileName = GetFileName(".csv");
                lTxt.Abrir();
                for (li = 0; li < ldt.Columns.Count; li++)
                {
                    lstValores.Add(ldt.Columns[li].ColumnName);
                }
                lTxt.Escribir("\"" + string.Join("\",\"", lstValores.ToArray()) + "\"");

                for (li = 0; li < ldt.Rows.Count; li++)
                {
                    lstValores = new List<string>();
                    for (lj = 0; lj < ldt.Columns.Count; lj++)
                    {
                        if (ldt.Rows[li][lj] is DateTime)
                        {
                            lstValores.Add(((DateTime)ldt.Rows[li][lj]).ToString(lsDateFormat));
                        }
                        else
                        {
                            lstValores.Add(ldt.Rows[li][lj].ToString());
                        }
                    }
                    lTxt.Escribir("\"" + string.Join("\",\"", lstValores.ToArray()) + "\"");
                }
                lTxt.Cerrar();
                lTxt = null;

                ExportarArchivo(".csv");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, ".csv");
            }
            finally
            {
                if (lTxt != null)
                {
                    lTxt.Cerrar();
                    lTxt = null;
                }
            }

        }

        protected void ExportarArchivo(string lsExt)
        {
            string lsTitulo = HttpUtility.UrlEncode(Globals.GetMsgWeb(false, "TituloCatalogos"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        protected string GetFileName(string lsExt)
        {
            string lsFileName = System.IO.Path.Combine(psTempPath, "cat." + psFileKey + ".temp" + lsExt);
            Session[psFileKey] = lsFileName;
            return lsFileName;
        }

        protected bool ValidarPermiso(Permiso p)
        {
            return DSONavegador.getPermiso("OpcCat", p);
        }

        public void InitLanguage()
        {
            int lTarget = 0;

            pWdEditor.Title = Globals.GetMsgWeb("WdNuevoRegistro");
            ptxtCodigo.Descripcion = Globals.GetMsgWeb(false, "vchCodigo");
            ptxtDescripcion.Descripcion = Globals.GetMsgWeb(false, "vchDescripcion");
            pdtIniV.Descripcion = Globals.GetMsgWeb(false, "dtIniVigencia");
            pdtFinV.Descripcion = Globals.GetMsgWeb(false, "dtFinVigencia");
            pdtIniV.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateFormat"));
            pdtFinV.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateFormat"));

            pbtnAgregar.InnerText = Globals.GetMsgWeb("btnAgregar");
            pbtnGrabar.InnerText = Globals.GetMsgWeb("btnGrabar");
            pbtnCancelar.InnerText = Globals.GetMsgWeb("btnCancelar");
            pbtnBaja.InnerText = Globals.GetMsgWeb("btnBaja");

            pbtnAgregar.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = "Cat.editar('#" + pWdEditor.Content.ClientID + "','" + Globals.GetMsgWeb("WdNuevoRegistro") + "');return false;";

            pExpAtributos.Title = Globals.GetMsgWeb("CatConfigTitle");
            pFields.InitLanguage();

            pCatGrid.Config.oLanguage = Globals.GetGridLanguage();

            if (ValidarPermiso(Permiso.Editar))
            {
                pCatGrid.Config.aoColumnDefs[0].sTitle = Globals.GetMsgWeb("btnEditar");
                pCatGrid.Config.aoColumnDefs[5].sTitle = Globals.GetMsgWeb("btnEditar");

                string lsfnRenderFormat = @"function(obj){{ return ""<a href=\""javascript:Cat.editar('#{0}','{1}',"" + obj.aData[5] + "");\""><img class='custom-img-edit' src='{2}'></a>""; }}";
                pCatGrid.Config.aoColumnDefs[lTarget++].fnRender = String.Format(lsfnRenderFormat, pWdEditor.Content.ClientID, Globals.GetMsgWeb("WdEditarRegistro"), ResolveUrl("~/images/pencilsmall.png"));
            }

            pCatGrid.Config.aoColumnDefs[lTarget++].sTitle = Globals.GetMsgWeb("vchCodigo");
            pCatGrid.Config.aoColumnDefs[lTarget++].sTitle = Globals.GetMsgWeb("vchDescripcion");
            pCatGrid.Config.aoColumnDefs[lTarget++].sTitle = Globals.GetMsgWeb("dtIniVigencia");
            pCatGrid.Config.aoColumnDefs[lTarget++].sTitle = Globals.GetMsgWeb("dtFinVigencia");
        }

        #region WebMethods

        public static string GetCatReg(int iCodRegistro)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCatalogos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                HistoricFieldCollection lFields = new HistoricFieldCollection(0, 1, false);
                StringBuilder lsbQuery = new StringBuilder();
                DataTable ldt;
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select");

                foreach (KeytiaBaseField lField in lFields)
                {
                    lsbQuery.AppendLine("	H." + lField.Column + ",");
                }

                lsbQuery.AppendLine("	C.iCodRegistro,");
                lsbQuery.AppendLine("	C.vchCodigo,");
                lsbQuery.AppendLine("	C.dtIniVigencia,");
                lsbQuery.AppendLine("	C.dtFinVigencia,");
                lsbQuery.AppendLine("	C.vchDescripcion");
                lsbQuery.AppendLine("from Catalogos C, Historicos H");
                lsbQuery.AppendLine("where C.iCodRegistro = " + iCodRegistro.ToString());
                lsbQuery.AppendLine("and H.iCodCatalogo = " + iCodRegistro.ToString());
                lsbQuery.AppendLine("and H.iCodMaestro = 1");
                lsbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");

                ldt = DSODataAccess.Execute(lsbQuery.ToString());

                return DSOControl.SerializeJSON<DataTable>(ldt);

            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrEditRecord", e);
            }
        }

        public static DSOGridServerResponse GetCatData(DSOGridServerRequest gsRequest)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCatalogos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];
                    switch (lsOrderCol)
                    {
                        case "vchCodigo":
                        case "dtIniVigencia":
                        case "dtFinVigencia":
                        case "vchDescripcion":
                            break;
                        default:
                            lsOrderCol = "vchDescripcion";
                            break;
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

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("      select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                lsbColumnas.AppendLine("	        a.vchCodigo,");
                lsbColumnas.AppendLine("	        a.dtIniVigencia,");
                lsbColumnas.AppendLine("	        a.dtFinVigencia,");
                lsbColumnas.AppendLine("	        a.vchDescripcion,");
                lsbColumnas.AppendLine("            iCodRegistroEditar = a.iCodRegistro,");
                lsbColumnas.AppendLine("            a.iCodRegistro");

                lsbFrom.AppendLine("      from Catalogos a");
                lsbFrom.AppendLine("      where a.iCodCatalogo is null");
                lsbFrom.AppendLine("      and a.dtIniVigencia <> a.dtFinVigencia");

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir);

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string[] lasTerms = gsRequest.sSearchGlobal.Replace("'", "''").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    lsbWhere.AppendLine("      and (");
                    bool lbPrimero = true;
                    foreach (string lsTerm in lasTerms)
                    {
                        if (lbPrimero)
                        {
                            lsbWhere.AppendLine("      (a.vchCodigo like '%" + lsTerm + "%'");
                        }
                        else
                        {
                            lsbWhere.AppendLine("      and (a.vchCodigo like '%" + lsTerm + "%'");
                        }

                        //tomar en cuenta todos los estilos de fechas
                        for (int i = 100; i <= 114; i++)
                        {
                            lsbWhere.AppendLine("          or convert(varchar, a.dtIniVigencia, " + i + ") like '%" + lsTerm + "%'");
                            lsbWhere.AppendLine("          or convert(varchar, a.dtFinVigencia, " + i + ") like '%" + lsTerm + "%'");
                        }
                        lsbWhere.AppendLine("          or a.vchDescripcion like '%" + lsTerm + "%'");
                        lsbWhere.AppendLine(")");
                        lbPrimero = false;
                    }
                    lsbWhere.AppendLine(")");
                }

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom);
                lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + lsWhere);
                lgsrRet.sColumns = "vchCodigo,dtIniVigencia,dtFinVigencia,vchDescripcion,iCodRegistroEditar,iCodRegistro";

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

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                lgsrRet.SetDataFromDataTable(ldt, lsDateFormat);
                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCatalogos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        #endregion
    }
}
