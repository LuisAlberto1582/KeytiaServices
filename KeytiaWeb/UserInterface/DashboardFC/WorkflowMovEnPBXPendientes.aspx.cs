using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text;
using DSOControls2008;
using System.Data;
using KeytiaServiceBL;
using System.IO;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class WorkflowMovEnPBXPendientes : System.Web.UI.Page
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
                    ReporteMovimientosPendientes();
                    LimpiarValores();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path
                      + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        #region Metodos Reporte

        private void ReporteMovimientosPendientes()
        {
            var dtResult = DSODataAccess.Execute(GetMovPendientesEnPBX());
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                gridMovEnPBX.DataSource = dtResult;
                gridMovEnPBX.DataBind();
                LlenarDropDownList(dtResult);
            }
            else
            {
                lblTituloModalMsn.Text = "No hay registros para mostrar";
                lblBodyModalMsn.Text = "No se encontraron registros que mostrar.";
                mpeEtqMsn.Show();
            }
        }

        #endregion Metodos Reporte

        #region Consultas

        private string GetMovPendientesEnPBX()
        {
            query.Length = 0;
            query.AppendLine("DECLARE @Where VARCHAR(MAX) = ''");
            query.AppendLine("SET @Where = @Where + '[iCodRegistro] > 0'");

            #region Filtro por Proveedor
            if (iCodProveedor.Value != null && Convert.ToInt32(iCodProveedor.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [ProveedorID] = " + iCodProveedor.Value + "' ");
            }
            #endregion Filtro por Proveedor

            #region Filtro por Solicitud
            if (iCodSolicitud.Value != null && Convert.ToInt32(iCodSolicitud.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [SolicitudID] = " + iCodSolicitud.Value + "' ");
            }
            #endregion Filtro por Solicitud

            #region Filtro por Sitio
            if (iCodSitio.Value != null && Convert.ToInt32(iCodSitio.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [SitioID] = " + iCodSitio.Value + "' ");
            }
            #endregion Filtro por Sitio

            #region Filtro por Tecnologia
            if (iCodTecnologia.Value != null && Convert.ToInt32(iCodTecnologia.Value) > 0)
            {
                query.AppendLine("SET @Where = @Where + ' AND [TecnologiaID] = " + iCodTecnologia.Value + "' ");
            }
            #endregion Filtro por Tecnologia

            query.AppendLine("");
            query.AppendLine("EXEC [WorkflowV2GetMovPendientesEnPBX]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Fields='*',");
            query.AppendLine("	@Where = @Where,");
            query.AppendLine("	@Order = '[FechaRegistro] Asc',");
            query.AppendLine("	@OrderInv = '[FechaRegistro] Desc',");
            query.AppendLine("	@OrderDir = 'Desc',");
            query.AppendLine("	@iCodUsuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@iCodPerfil = " + Session["iCodPerfil"]);

            return query.ToString();
        }

        private string ConsultaGetRegBitacoraById(string idRegBit)
        {
            query.Length = 0;
            query.AppendLine("DECLARE @Where VARCHAR(MAX) = ''");
            query.AppendLine("SET @Where = @Where + '[iCodRegistro] > 0'");
            query.AppendLine("SET @Where = @Where + ' AND [iCodRegistro] = " + idRegBit + "' ");
            query.AppendLine("");
            query.AppendLine("EXEC [WorkflowV2GetMovPendientesEnPBX]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Fields='*',");
            query.AppendLine("	@Where = @Where,");
            query.AppendLine("	@Order = '[FechaRegistro] Asc',");
            query.AppendLine("	@OrderInv = '[FechaRegistro] Desc',");
            query.AppendLine("	@OrderDir = 'Desc',");
            query.AppendLine("	@iCodUsuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@iCodPerfil = " + Session["iCodPerfil"]);

            return query.ToString();
        }

        #endregion Consultas

        private void LlenarDropDownList(DataTable dtResult)
        {
            var lista = dtResult.AsEnumerable().ToList();
            var proveedor = lista.GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("ProveedorID"),
                Descripcion = x.Field<string>("ProveedorNom")
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            proveedor.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            var solicitud = lista.GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("SolicitudID"),
                Descripcion = x.Field<int>("SolicitudID").ToString()
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            solicitud.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            var sitio = lista.GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("SitioID"),
                Descripcion = x.Field<string>("SitioNom")
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            sitio.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            var tecnologia = lista.GroupBy(x => new
            {
                iCodCatalogo = x.Field<int>("TecnologiaID"),
                Descripcion = x.Field<string>("TecnologiaDesc")
            }).Select(w => new { w.Key.iCodCatalogo, w.Key.Descripcion }).OrderBy(y => y.Descripcion).ToList();
            tecnologia.Insert(0, new { iCodCatalogo = 0, Descripcion = "TODOS" });

            cboProveedor.DataSource = proveedor;
            cboProveedor.DataBind();

            cboSolicitud.DataSource = solicitud;
            cboSolicitud.DataBind();

            cboSitio.DataSource = sitio;
            cboSitio.DataBind();

            cboTecnologia.DataSource = tecnologia;
            cboTecnologia.DataBind();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            iCodProveedor.Value = cboProveedor.SelectedValue;
            iCodSolicitud.Value = cboSolicitud.SelectedValue;
            iCodSitio.Value = cboSitio.SelectedValue;
            iCodTecnologia.Value = cboTecnologia.SelectedValue;

            var dtResult = DSODataAccess.Execute(GetMovPendientesEnPBX());
            gridMovEnPBX.DataSource = dtResult;
            gridMovEnPBX.DataBind();
        }

        protected void gridMovEnPBX_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);
            GridViewRow selectedRow = (GridViewRow)gridMovEnPBX.Rows[rowIndex];
            LimpiarControlesModal();
            int iCodRegEvidencia = (int)gridMovEnPBX.DataKeys[rowIndex]["iCodRegistro"];

            var dtResult = DSODataAccess.Execute(ConsultaGetRegBitacoraById(iCodRegEvidencia.ToString()));
            if (dtResult.Rows.Count > 0)
            {
                txtRecursoModal.Text = dtResult.Rows[0]["Recurso"].ToString();
                txtSolicitudModal.Text = dtResult.Rows[0]["SolicitudID"].ToString();
                txtTelefonoModal.Text = dtResult.Rows[0]["Telefono"].ToString();
                txtRegistroModal.Text = dtResult.Rows[0]["FechaRegistro"].ToString();
                txtSitioModal.Text = dtResult.Rows[0]["SitioNom"].ToString();
                txtTecnologiaModal.Text = dtResult.Rows[0]["TecnologiaDesc"].ToString();
                txtIPModal.Text = dtResult.Rows[0]["IPSitio"].ToString();
                txtMovimientoModal.Text = dtResult.Rows[0]["MovimientoDesc"].ToString();
                txtPermisoModal.Text = dtResult.Rows[0]["CosDesc"].ToString();
                txtEmpleadoModal.Text = dtResult.Rows[0]["EmpleadoNom"].ToString();
                txtComentarioModal.Text = dtResult.Rows[0]["Comentarios"].ToString();
                hfICodRegBitacora.Value = iCodRegEvidencia.ToString();
                hfRutaImgEvidencia.Value = dtResult.Rows[0]["RutaImgEvidencia"].ToString();
                hfProveedorID.Value = dtResult.Rows[0]["ProveedorID"].ToString();
                hfSolicitudID.Value = dtResult.Rows[0]["SolicitudID"].ToString();
                hfSitioID.Value = dtResult.Rows[0]["SitioID"].ToString();
            }

            lblTituloEditMovEnPBX.Text = "Editar Movimiento en Conmutador";
            StyleFiltros();
            mpeEditMovEnPBX.Show();
        }

        private void StyleFiltros()
        {
            List<int> rowIndexVisibles = new List<int>();
            for (int i = 1; i < tablaCaptura.Rows.Count; i++)
            {
                if (tablaCaptura.Rows[i].Visible)
                    rowIndexVisibles.Add(i);
            }
            if (rowIndexVisibles.Count > 0)
            {
                for (int i = 1; i < rowIndexVisibles.Count; i++)
                {
                    tablaCaptura.Rows[rowIndexVisibles[i]].CssClass = (i % 2) == 0 ? "grvalternateItemStyle" : "grvitemStyle";
                }
            }
            else
            {
                for (int i = 1; i < tablaCaptura.Rows.Count; i++)
                {
                    tablaCaptura.Rows[i].CssClass = (i % 2) == 0 ? "grvalternateItemStyle" : "grvitemStyle";
                }
            }
        }

        protected void btnGuardarCambiosBitacoraMovPBX_Click(object sender, EventArgs e)
        {
            try
            {
                lblTituloModalMsn.Text = "Guardar cambios";
                string idBitacora = hfICodRegBitacora.Value;
                string imgEvidencia = string.Empty;
                string nuevoNombre = string.Empty;
                if (FileUploadEvidencia.HasFile)
                {
                    if (!string.IsNullOrEmpty(idBitacora) && idBitacora != "0")
                    {
                        if (FileUploadEvidencia.PostedFile.ContentType == "image/jpeg" ||
                                        FileUploadEvidencia.PostedFile.ContentType == "image/png")
                        {
                            if (FileUploadEvidencia.PostedFile.ContentLength > 0 &&
                                (FileUploadEvidencia.PostedFile.ContentLength / 1024) <= 1024)  //ContentLength Propiedad en kb
                            {
                                string extension = Path.GetExtension(FileUploadEvidencia.FileName);

                                //Nomenclatura del nuevo nombre:
                                //vchCodigoEsquema_B_iCodRegistroBitacora_P_iCodCatProveedor_S_iCodCatSolicitud_SB_iCodCatSitio
                                nuevoNombre = DSODataContext.Schema + "_B_" + idBitacora + "_P_" + hfProveedorID.Value + "_S_"
                                            + hfSolicitudID.Value + "_SB_" + hfSitioID.Value + extension;

                                if (!(Directory.Exists(hfRutaImgEvidencia.Value)))
                                {
                                    Directory.CreateDirectory(hfRutaImgEvidencia.Value);
                                } 

                                imgEvidencia = hfRutaImgEvidencia.Value + nuevoNombre;
                                FileUploadEvidencia.SaveAs(imgEvidencia);
                            }
                            else { throw new ArgumentException("La evidencia no puede tener un peso mayor a 1MB"); }
                        }
                        else { throw new ArgumentException("Solamente archivos JPEG o PNG son aceptados."); }
                    }
                    else { throw new ArgumentException("No se cuenta con los datos suficientes para la actualización."); }
                }
                else { throw new ArgumentException("El campo de envidencia es requerido para continuar"); }

                //Proceder al Insert
                if (!string.IsNullOrEmpty(imgEvidencia))
                {
                    string comentario = txtComentarioModal.Text.ToUpper().Trim().Replace("'", "").Replace(";", "").Replace("INSERT", "")
                                        .Replace("DELETE", "").Replace("DROP", "").Replace("UPDATE", "").Replace("SELECT", "").Replace("TRUNCATE", "");

                    var valor = ActualizarRegistroBitacora(idBitacora, imgEvidencia, txtComentarioModal.Text.Trim(), hfSolicitudID.Value);

                    if (valor == 0) { throw new ArgumentException("No fue posible guardar los datos."); }
                }

                //Se actualiza el Grid
                ReporteMovimientosPendientes();
                Response.Redirect(Request.RawUrl);

                lblBodyModalMsn.Text = "Los cambios se guardaron correctamente.";
                mpeEtqMsn.Show();
            }
            catch (ArgumentException ex)
            {
                lblBodyModalMsn.Text = ex.Message;
                mpeEtqMsn.Show();
            }
            catch (Exception)
            {
                lblBodyModalMsn.Text = "Ocurrió un error al intentar grabar los cambios.";
                mpeEtqMsn.Show();
            }
        }

        private int ActualizarRegistroBitacora(string idRegBitacora, string imgEvidencia, string comentarios, string iCodCatSolicitud)
        {
            query.Length = 0;
            query.AppendLine("EXEC [WorkflowV2InsertEvidenciaUpdateBitacora]");
            query.AppendLine("  @Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @iCodCatSolicitud = " + iCodCatSolicitud + ",");
            query.AppendLine("  @iCodRegistroBitacora = " + idRegBitacora + ",");
            query.AppendLine("  @RutaEvidencia = '" + imgEvidencia + "',");
            query.AppendLine("  @Comentarios = '" + comentarios + "'");

            return Convert.ToInt32(DSODataAccess.ExecuteScalar(query.ToString()));
        }

        private void LimpiarControlesModal()
        {
            txtRecursoModal.Text = string.Empty;
            txtSolicitudModal.Text = string.Empty;
            txtTelefonoModal.Text = string.Empty;
            txtRegistroModal.Text = string.Empty;
            txtSitioModal.Text = string.Empty;
            txtTecnologiaModal.Text = string.Empty;
            txtIPModal.Text = string.Empty;
            txtMovimientoModal.Text = string.Empty;
            txtPermisoModal.Text = string.Empty;
            txtEmpleadoModal.Text = string.Empty;
            txtComentarioModal.Text = string.Empty;
            hfICodRegBitacora.Value = string.Empty;
            hfRutaImgEvidencia.Value = string.Empty;
            hfProveedorID.Value = string.Empty;
            hfSolicitudID.Value = string.Empty;
            hfSitioID.Value = string.Empty;           
        }

        private void LimpiarValores()
        {
            iCodProveedor.Value = "0";
            iCodSolicitud.Value = "0";
            iCodSitio.Value = "0";
            iCodTecnologia.Value = "0";
        }


    }
}
