using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;
using System.Web.Services;
using KeytiaServiceBL.Reportes;
using System.Collections;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class HallazgosGA : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion
                               
                if (!Page.IsPostBack)
                {
                    LlenarDropDownListPrimerFiltro();
                    LimpiarValores();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path
                      + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void LlenarDropDownListPrimerFiltro()
        {
            cboCarrier.DataSource = GetDataDropDownList("CARRIER").DefaultView;
            cboCarrier.DataBind();

            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();

            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();

            pnlRep0.Visible = true;
            OcultarControles();
        }

        private DataTable GetDataDropDownList(string clave)
        {
            bool isEstatus = false;
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");

            #region Filtro
            switch (clave.ToUpper())
            {
                case "CARRIER":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Carrier','Carriers','Español')]");
                    query.AppendLine("  ORDER BY vchDescripcion");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                case "ANIO":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Anio','Años','Español')]");
                    query.AppendLine(" AND CONVERT(INT, vchDescripcion) >= 2016 AND CONVERT(INT, vchDescripcion) <= YEAR(GETDATE())");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                case "MES":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Español AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                case "ESTATUS":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Descripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('EstatusGestionAdmitiva','Estatus Gestion Admitiva','Español')]");
                    isEstatus = true;
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                default:
                    return new DataTable();
            }

            #endregion
        }

        private DataTable AddRowDefault(DataTable dt, bool estatus)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow rowExtra = dt.NewRow();
                rowExtra["iCodCatalogo"] = 0;
                rowExtra["Descripcion"] = !estatus ? "TODOS" : "Seleccionar";
                dt.Rows.InsertAt(rowExtra, 0);
            }
            return dt;
        }

        protected void cboCarrierIndex_Changed(Object sender, EventArgs e)
        {
            CountPreviewResultados();
        }

        #region Consultas

        private string ConsultaCOUNTPrimerFiltro()
        {
            query.Length = 0;
            query.AppendLine("DECLARE @Where VARCHAR(MAX) = ''");
            query.AppendLine("SET @Where = @Where + '[iCodRegistro] > 0'");

            #region Filtro por Carrier
            if (iCodCarrier.Value != null && Convert.ToInt32(iCodCarrier.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [CarrierID] = " + iCodCarrier.Value + "' ");
            }
            #endregion Filtro por Carrier

            #region Filtro por CtaMaestra
            if (iCodCtaMaestra.Value != null && Convert.ToInt32(iCodCtaMaestra.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [CtaMaestraID] = " + iCodCtaMaestra.Value + "' ");
            }
            #endregion Filtro por CtaMaestra

            #region Filtro por Anio
            if (iCodAnio.Value != null && Convert.ToInt32(iCodAnio.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [AnioID] = " + iCodAnio.Value + "' ");
            }
            #endregion Filtro por Anio

            #region Filtro por Mes
            if (iCodMes.Value != null && Convert.ToInt32(iCodMes.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [MesID] = " + iCodMes.Value + "' ");
            }
            #endregion Filtro por Mes

            #region Filtro por TDest
            if (iCodTDest.Value != null && Convert.ToInt32(iCodTDest.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [TDestID] = " + iCodTDest.Value + "' ");
            }
            #endregion Filtro por TDest

            #region Filtro por Variacion
            if (iCodVariacion.Value != null && Convert.ToInt32(iCodVariacion.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [VariacionID] = " + iCodVariacion.Value + "' ");
            }
            #endregion Filtro por Variacion

            #region Filtro por Estatus
            if (iCodEstatus.Value != null && Convert.ToInt32(iCodEstatus.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [EstatusID] = " + iCodEstatus.Value + "' ");
            }
            #endregion Filtro por Estatus

            query.AppendLine("");
            query.AppendLine("EXEC [GestionAdmitivaGetCOUNTHallazgos]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Where = @Where,");
            query.AppendLine("	@Start = 0");

            return query.ToString();
        }

        private string ConsultaGetHallazgos()
        {
            query.Length = 0;
            query.AppendLine("DECLARE @Where VARCHAR(MAX) = ''");
            query.AppendLine("SET @Where = @Where + '[iCodRegistro] > 0'");

            #region Filtro por Carrier
            if (iCodCarrier.Value != null && Convert.ToInt32(iCodCarrier.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [CarrierID] = " + iCodCarrier.Value + "' ");
            }
            #endregion Filtro por Carrier

            #region Filtro por CtaMaestra
            if (iCodCtaMaestra.Value != null && Convert.ToInt32(iCodCtaMaestra.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [CtaMaestraID] = " + iCodCtaMaestra.Value + "' ");
            }
            #endregion Filtro por CtaMaestra

            #region Filtro por Anio
            if (iCodAnio.Value != null && Convert.ToInt32(iCodAnio.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [AnioID] = " + iCodAnio.Value + "' ");
            }
            #endregion Filtro por Anio

            #region Filtro por Mes
            if (iCodMes.Value != null && Convert.ToInt32(iCodMes.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [MesID] = " + iCodMes.Value + "' ");
            }
            #endregion Filtro por Mes

            #region Filtro por TDest
            if (iCodTDest.Value != null && Convert.ToInt32(iCodTDest.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [TDestID] = " + iCodTDest.Value + "' ");
            }
            #endregion Filtro por TDest

            #region Filtro por Variacion
            if (iCodVariacion.Value != null && Convert.ToInt32(iCodVariacion.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [VariacionID] = " + iCodVariacion.Value + "' ");
            }
            #endregion Filtro por Variacion

            #region Filtro por Estatus
            if (iCodEstatus.Value != null && Convert.ToInt32(iCodEstatus.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [EstatusID] = " + iCodEstatus.Value + "' ");
            }
            #endregion Filtro por Estatus

            query.AppendLine("");
            query.AppendLine("EXEC [GestionAdmitivaGetHallazgos]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Fields='*',");
            query.AppendLine("	@Where = @Where,");
            query.AppendLine("	@Order = '[iCodRegistro] Desc, [Importe] Desc,[Folio] Desc,[Anio] Asc,[Mes] Asc',");
            query.AppendLine("	@OrderInv = '[iCodRegistro] Desc, [Importe] Desc,[Folio] Desc,[Anio] Asc,[Mes] Asc',");
            query.AppendLine("	@OrderDir = 'Desc',");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "'");

            return query.ToString();
        }

        private string ConsultaGetHallazgosById(string idHallazgo)
        {
            query.Length = 0;
            query.AppendLine("DECLARE @Where VARCHAR(MAX) = ''");
            query.AppendLine("SET @Where = @Where + '[iCodRegistro] > 0'");
            query.AppendLine("SET @Where = @Where + ' AND [HallazgoID] = " + idHallazgo + "' ");
            query.AppendLine("");
            query.AppendLine("EXEC [GestionAdmitivaGetHallazgos]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Fields='*',");
            query.AppendLine("	@Where = @Where,");
            query.AppendLine("	@Order = '[Importe] Desc,[Folio] Desc,[Anio] Asc,[Mes] Asc',");
            query.AppendLine("	@OrderInv = '[Importe] Desc,[Folio] Desc,[Anio] Asc,[Mes] Asc',");
            query.AppendLine("	@OrderDir = 'Desc',");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "'");

            return query.ToString();
        }

        private string ConsultaGetComentarios(int idHallazgo)
        {
            query.Length = 0;
            query.AppendLine("SELECT ");
            query.AppendLine("	Usuario			= UsuarCod,");
            query.AppendLine("	Fecha			= FechaReg,");
            query.AppendLine("	Comentario		= Comentarios,");
            query.AppendLine("  HallazgoID		= Rel.HallazgosGestionAdmitiva");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('ComentarioGestionAdmitiva','Comentarios Gestion Admitiva','Español')] Com");
            query.AppendLine("	JOIN " + DSODataContext.Schema + ".[VisRelaciones('Gestion Admitiva - Comentarios','Español')] Rel");
            query.AppendLine("		ON Rel.ComentarioGestionAdmitiva = Com.iCodCatalogo");
            query.AppendLine("		AND Rel.dtIniVigencia <> Rel.dtFinVigencia");
            query.AppendLine("		AND Rel.dtFinVigencia >= GETDATE()");
            query.AppendLine("WHERE Com.dtIniVigencia <> Com.dtFinVigencia");
            query.AppendLine("	AND Com.dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND Rel.HallazgosGestionAdmitiva = " + idHallazgo);
            query.AppendLine("ORDER BY FechaReg DESC");

            return query.ToString();
        }

        private int CountPreviewResultados()
        {
            iCodCarrier.Value = cboCarrier.SelectedValue;
            iCodAnio.Value = cboAnio.SelectedValue;
            iCodMes.Value = cboMes.SelectedValue;
            iCodCtaMaestra.Value = "0";
            iCodTDest.Value = "0";
            iCodVariacion.Value = "0";
            iCodEstatus.Value = "0";
            int result = Convert.ToInt32(DSODataAccess.ExecuteScalar(ConsultaCOUNTPrimerFiltro()));
            lblTitulo.Text = "Búsqueda: " + result + " resultado(s)";
            return result;
        }

        #endregion Consultas

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            var dtResult = DSODataAccess.Execute(ConsultaGetHallazgos());
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                pnlRep0.Visible = false;
                gridHallazgos.DataSource = dtResult;
                gridHallazgos.DataBind();
                gridHallazgos.HeaderRow.TableSection = TableRowSection.TableHeader;
                LlenarDropDownListSegundoFiltro(dtResult);
                MostrarPanelHallazgos();
            }
            else
            {
                lblTituloModalMsn.Text = "No hay registros para mostrar";
                lblBodyModalMsn.Text = "No se encontraron registros que mostrar.";
                mpeEtqMsn.Show();
                pnlRep0.Visible = true;
                OcultarControles();
            }
        }

        private void MostrarPanelHallazgos()
        {
            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
            {
                gridHallazgos.Columns[20].Visible = false;
            }
            OcultarControles();
        }

        private void LlenarDropDownListSegundoFiltro(DataTable dtResult)
        {
            var lista = dtResult.AsEnumerable().ToList();
            var ctaMaestra = lista.GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("CtaMaestraID"),
                Descripcion = x.Field<string>("Cuenta")
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            ctaMaestra.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            var TDest = lista.GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("TDestID"),
                Descripcion = x.Field<string>("Servicio")
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            TDest.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            var variacion = lista.GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("VariacionID"),
                Descripcion = x.Field<string>("Variacion")
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            variacion.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            var estatus = lista.Where(w => !w.IsNull("EstatusID")).GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("EstatusID"),
                Descripcion = x.Field<string>("Estatus")
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            estatus.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            cboCtaMestra.DataSource = ctaMaestra;
            cboCtaMestra.DataBind();

            cboTDest.DataSource = TDest;
            cboTDest.DataBind();

            cboVariacion.DataSource = variacion;
            cboVariacion.DataBind();

            cboEstatus.DataSource = estatus;
            cboEstatus.DataBind();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            iCodCtaMaestra.Value = cboCtaMestra.SelectedValue;
            iCodTDest.Value = cboTDest.SelectedValue;
            iCodVariacion.Value = cboVariacion.SelectedValue;
            iCodEstatus.Value = cboEstatus.SelectedValue;

            var dtResult = DSODataAccess.Execute(ConsultaGetHallazgos());
            gridHallazgos.DataSource = dtResult;
            gridHallazgos.DataBind();
            gridHallazgos.HeaderRow.TableSection = TableRowSection.TableHeader;
            MostrarPanelHallazgos();
        }

        protected void btnLinkFolio_Click(object sender, EventArgs e)
        {
            LinkButton lnkView = sender as LinkButton;
            string numFolio = lnkView.Text;
            string idHallazgo = lnkView.CommandArgument;
            txtAddComentarioModal.Text = string.Empty;
            txtIdHallazoModal.Text = idHallazgo.ToString();

            var dtResult = DSODataAccess.Execute(ConsultaGetComentarios(Convert.ToInt32(idHallazgo)));
            gridComentarios.DataSource = dtResult;
            gridComentarios.DataBind();
            lblTituloComentarios.Text = "COMENTARIOS " + numFolio;
            mpeComentarios.Show();
        }

        protected void gridHallazgos_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);
            GridViewRow selectedRow = (GridViewRow)gridHallazgos.Rows[rowIndex];
            LimpiarControlesModal();
            int iCodHallago = (int)gridHallazgos.DataKeys[rowIndex]["HallazgoID"];
            var dtResult = DSODataAccess.Execute(ConsultaGetHallazgosById(iCodHallago.ToString()));
            if (dtResult.Rows.Count > 0)
            {
                txtFolioModal.Text = dtResult.Rows[0]["Folio"].ToString();
                txtHallazgoModal.Text = dtResult.Rows[0]["Hallazgo"].ToString();
                txtCarrierModal.Text = dtResult.Rows[0]["Carrier"].ToString();
                txtCtaMaestraModal.Text = dtResult.Rows[0]["Cuenta"].ToString();
                txtAnioModal.Text = dtResult.Rows[0]["Anio"].ToString();
                txtMesModal.Text = dtResult.Rows[0]["Mes"].ToString();
                txtTDestModal.Text = dtResult.Rows[0]["Servicio"].ToString();
                txtVariacionModal.Text = dtResult.Rows[0]["Variacion"].ToString();
                txtImporteModal.Text = dtResult.Rows[0]["Importe"].ToString();
                txtDescripcionModal.Text = dtResult.Rows[0]["Descripcion"].ToString();
                txtIdHallazoModal.Text = iCodHallago.ToString();
                cboEstatusModal.DataSource = GetDataDropDownList("ESTATUS");
                cboEstatusModal.DataBind();

                if (dtResult.Rows[0]["EstatusID"] != DBNull.Value)
                {
                    cboEstatusModal.SelectedValue = dtResult.Rows[0]["EstatusID"].ToString();
                }
            }

            lblTituloEditHallazo.Text = "Editar Hallazgo";         
            mpeEditHallazo.Show();
        }
      

        protected void btnGuardarCambiosHallazgoModal_Click(object sender, EventArgs e)
        {
            string idHallazgo = txtIdHallazoModal.Text;
            string idEstatus = cboEstatusModal.SelectedValue;

            if (string.IsNullOrEmpty(idEstatus) || idEstatus == "0")
            {
                idEstatus = "NULL";
            }
            ActualizarEstatusHallazgo(idHallazgo, idEstatus);
            mpeEditHallazo.Hide();

            //Se actualiza el Grid
            var dtResult = DSODataAccess.Execute(ConsultaGetHallazgos());
            gridHallazgos.DataSource = dtResult;
            gridHallazgos.DataBind();
            gridHallazgos.HeaderRow.TableSection = TableRowSection.TableHeader;
            LlenarDropDownListSegundoFiltro(dtResult);
            MostrarPanelHallazgos();
        }

        private void ActualizarEstatusHallazgo(string idHallazgo, string idEstatus)
        {
            query.Length = 0;
            query.AppendLine("UPDATE " + DSODataContext.Schema + ".[VisHistoricos('HallazgosGestionAdmitiva','Hallazgos Gestion Admitiva','Español')]");
            query.AppendLine("SET EstatusGestionAdmitiva = " + idEstatus + ", dtFecUltAct = GETDATE()");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND iCodCatalogo = " + idHallazgo);
            DSODataAccess.ExecuteNonQuery(query.ToString());
        }

        private void InsertarComentarioHallazgo(string idHallazgo, string comentario)
        {
            query.Length = 0;
            query.AppendLine("EXEC [AltaComentatioGestionAdmitiva]");
            query.AppendLine("  @Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @Comentario = '" + comentario + "',");
            query.AppendLine("  @iCodHallazgo = " + idHallazgo + ",");
            query.AppendLine("  @iCodUsuario = " + Session["iCodUsuario"]);
            DSODataAccess.Execute(query.ToString());
        }

        private void LimpiarControlesModal()
        {
            txtFolioModal.Text = string.Empty;
            txtHallazgoModal.Text = string.Empty;
            txtCarrierModal.Text = string.Empty;
            txtCtaMaestraModal.Text = string.Empty;
            txtAnioModal.Text = string.Empty;
            txtMesModal.Text = string.Empty;
            txtTDestModal.Text = string.Empty;
            txtVariacionModal.Text = string.Empty;
            txtImporteModal.Text = string.Empty;
            txtDescripcionModal.Text = string.Empty;
            txtIdHallazoModal.Text = string.Empty;
            txtAddComentarioModal.Text = string.Empty;
        }

        protected void btnAgregarComentario_Click(object sender, EventArgs e)
        {
            string comentarioHallazgo = txtAddComentarioModal.Text.Trim();
            if (!string.IsNullOrEmpty(comentarioHallazgo))
            {
                InsertarComentarioHallazgo(txtIdHallazoModal.Text, comentarioHallazgo);
                mpeEditHallazo.Hide();

                lblTituloModalMsn.Text = "Agregar comentario";
                lblBodyModalMsn.Text = "El comentario se inserto correctamente";
                mpeEtqMsn.Show();
            }
        }
       
        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS(".xlsx");
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            pnlRep0.Visible = true;
            OcultarControles();

            //Setear todas las variables en sesion para que no se quede ningun filtro guardado.
            LimpiarValores();
        }

        private void LimpiarValores()
        {
            iCodCarrier.Value = "0";
            iCodAnio.Value = "0";
            iCodMes.Value = "0";

            iCodCtaMaestra.Value = "0";
            iCodTDest.Value = "0";
            iCodVariacion.Value = "0";
            iCodEstatus.Value = "0";
        }

        private void OcultarControles()
        {
            if (pnlRep0.Visible)
            {
                pnlRep1.Visible = pnlRep2.Visible = btnRegresar.Visible = btnExportarXLS.Visible = false;
            }
            else
            {
                pnlRep1.Visible = pnlRep2.Visible = btnRegresar.Visible = btnExportarXLS.Visible = true;
            }
        }


        #region Exportacion Reporte detallado

        public void ExportXLS(string tipoExtensionArchivo)
        {
            CrearXLS(tipoExtensionArchivo);
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {

                #region Exportar Reportes solo con tabla

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaSinFechas" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                var dtResult = DSODataAccess.Execute(ConsultaGetHallazgos());

                if (dtResult.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dtResult);
                    dtResult = dvldt.ToTable(false, new string[] { "Folio", "Hallazgo", "Carrier", "Cuenta", "Anio", "Mes", "Variacion", "Importe", "Estatus", "Descripcion", "Categoria" });

                    dtResult.Columns["Anio"].ColumnName = "Año";
                    dtResult.Columns["Variacion"].ColumnName = "Variación";
                    dtResult.Columns["Descripcion"].ColumnName = "Descripción";
                    dtResult.Columns["Categoria"].ColumnName = "Categoría";

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Gestion Administrativa");
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtResult, "Reporte", "Tabla");
                }

                #endregion Exportar Reportes solo con tabla

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;
                lExcel.SalvarComo();
                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reporte" + "_" + "GestionAdministrativa" + "_");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, lsExt);
            }
            finally
            {
                if (lExcel != null)
                {
                    lExcel.Cerrar(true);
                    lExcel.Dispose();

                }
            }
        }
                
        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion Exportacion Reporte detallado
    }
}
