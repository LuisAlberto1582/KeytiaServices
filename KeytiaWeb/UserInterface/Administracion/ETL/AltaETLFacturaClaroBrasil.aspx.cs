using KeytiaServiceBL;
using KeytiaServiceBL.Handler;
using KeytiaServiceBL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Administracion.ETL
{
    public partial class AltaETLFacturaClaroBrasil : System.Web.UI.Page
    {
        const string CLASE_CARGA = "Keytia.AppCluster.DataLoaderLogic.ETL.Facturas.ClaroBrasilCel.ETLFacturaClaroBrasilCelService";
        const string ENTIDAD = "Cargas";
        const string DESC_MAESTRO = "ETL Factura ClaroBrasilCel v3";
        const string EST_CARGA = "ETLEsperandoAtencion";
        static int icodCatEditaCarga;
        private string saveFolder;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                FillDropDownLists();
                string icodCarga = Request.QueryString["param"];
                if (icodCarga != null && icodCarga != "")
                {
                    ObtieneDatosCarga(Convert.ToInt32(icodCarga));
                }
            }
        }
        private string GetSaveFolderByEsquema()
        {
            return new UsuarioDBHandler().GetByEsquema(DSODataContext.Schema, DSODataContext.ConnectionString).SaveFolder + @"\Cargas\ETL Factura ClaroBrasilCel v3";
            //return @"Y:\K5\Archivos\Factura";  //TODO: Reemplazar
        }
        private void FillDropDownLists()
        {
            var lstAnios = AnioHandler.GetAll(DSODataContext.ConnectionString).Where(x => x.NumeroAnio >= (DateTime.Now.Year - 1));
            var lstMeses = MesHandler.GetAll(DSODataContext.ConnectionString);
            var lstEmpresas = new EmpresaHandler().GetAll(DSODataContext.ConnectionString);
            var lstMonedas = new MonedaHandler().GetAll(DSODataContext.ConnectionString);

            var anioDefault = lstAnios.FirstOrDefault(x => x.NumeroAnio == DateTime.Now.Year).ICodCatalogo;
            var mesDefault = lstMeses.FirstOrDefault(x => x.NumeroMes == DateTime.Now.Month).ICodCatalogo;
            var empreDefault = lstEmpresas.FirstOrDefault(x => x.VchCodigo.ToUpper() == "TERNIUMBRASIL").ICodCatalogo;
            var monedaDefault = lstMonedas.FirstOrDefault(x => x.VchCodigo == "BRL").ICodCatalogo;

            ddlAnio.DataValueField = "iCodCatalogo";
            ddlAnio.DataTextField = "NumeroAnio";
            ddlAnio.DataSource = lstAnios;
            ddlAnio.DataBind();
            ddlAnio.SelectedValue = ddlAnio.Items.FindByValue(anioDefault.ToString()).Value;

            ddlMes.DataValueField = "iCodCatalogo";
            ddlMes.DataTextField = "Español";
            ddlMes.DataSource = lstMeses;
            ddlMes.DataBind();
            ddlMes.SelectedValue = ddlMes.Items.FindByValue(mesDefault.ToString()).Value;

            ddlEmpresa.DataValueField = "iCodCatalogo";
            ddlEmpresa.DataTextField = "vchDescripcion";
            ddlEmpresa.DataSource = lstEmpresas;
            ddlEmpresa.DataBind();
            ddlEmpresa.SelectedValue = ddlEmpresa.Items.FindByValue(empreDefault.ToString()).Value;

            ddlMoneda.DataValueField = "iCodCatalogo";
            ddlMoneda.DataTextField = "Español";
            ddlMoneda.DataSource = lstMonedas;
            ddlMoneda.DataBind();
            ddlMoneda.SelectedValue = ddlMoneda.Items.FindByValue(monedaDefault.ToString()).Value;
        }
        private int ObtieneValorEnteroBanderas()
        {
            int valorBanderas = 0;

            if (chkCargaLineasNoReg.Checked)
                valorBanderas += 1;

            if (chkSubirInfo.Checked)
                valorBanderas += 32;

            if (chkActualizaLineas.Checked)
                valorBanderas += 64;

            if (chkGeneraDetalle.Checked)
                valorBanderas += 128;

            if (chkGeneraResumenes.Checked)
                valorBanderas += 256;

            return valorBanderas;
        }
        private bool RegistraHistProcesoETL()
        {
            bool exitoso = false;

            try
            {

                var iCodMaestro = new MaestroViewHandler().GetMaestroEntidad(ENTIDAD, DESC_MAESTRO, DSODataContext.ConnectionString).ICodRegistro;
                var estCarga = new EstatusCargaHandler().GetByVchCodigo(EST_CARGA, DSODataContext.ConnectionString).ICodCatalogo;

                ETLFacturaClaroBrasil etl = new ETLFacturaClaroBrasil
                {
                    ICodMaestro = iCodMaestro,
                    VchCodigo = txtClave.Text,
                    VchDescripcion = txtClave.Text,
                    Empre = Convert.ToInt32(ddlEmpresa.SelectedValue),
                    Anio = Convert.ToInt32(ddlAnio.SelectedValue),
                    Mes = Convert.ToInt32(ddlMes.SelectedValue),
                    EstCarga = estCarga,
                    Moneda = Convert.ToInt32(ddlMoneda.SelectedValue),
                    BanderasFacClaroBrasilCelv3 = ObtieneValorEnteroBanderas(),
                    Clase = CLASE_CARGA,
                    Archivo01 = fuClaroFile1.HasFile ? saveFolder + "\\" + fuClaroFile1.FileName : "",
                    DtIniVigencia = DateTime.Now,
                    DtFinVigencia = new DateTime(2079, 1, 1, 0, 0, 0)
                };

                if (!ValidaExisteCargaMismmaClave(etl.VchCodigo))
                {
                    exitoso = ETLFacturaClaroBrasilHandler.CreaNuevoRegistroETL(etl);
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }

            return exitoso;
        }
        private bool ActualizaEstatusCargaETL(int icodCarga)
        {
            bool exitoso = false;
            try
            {
                int banderas = ObtieneValorEnteroBanderas();
                exitoso = ETLFacturaClaroBrasilHandler.ActualizaCargaETL(icodCarga, banderas);
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }
            return exitoso;

        }
        private bool ValidaExisteCargaMismmaClave(string claveCarga)
        {
            return ETLFacturaClaroBrasilHandler.ValidaExisteCargaMismaClave(claveCarga);
        }
        private bool ValidaExisteCargaMismaFecha(int anio, int mes)
        {
            return ETLFacturaClaroBrasilHandler.ValidaExisteCargaMismaFecha(anio, mes);
        }
        protected void TransfiereArchivo(ref FileUpload fileupload, ref Label label)
        {
            if (fileupload.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    btnAceptar.Visible = false;

                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);

                    fileupload.SaveAs(saveFolder + "\\" + filename);

                    label.Text = "Transferencia finalizada!";

                    System.Threading.Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    label.Text = "Ha ocurrido un error al intentar transferir el archivo.";
                }
            }
        }
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            bool exitoso = false;
            if (icodCarga.Value != null && icodCarga.Value != "")
            {
                int icodCatEditaCarga = Convert.ToInt32(icodCarga.Value);
                exitoso = ActualizaEstatusCargaETL(icodCatEditaCarga);
                if (exitoso)
                {
                    lblMensajeConfirma.Text = "Se ha registrado correctamente la instrucción.";
                }
                else
                {
                    lblMensajeConfirma.Text = "Se ha generado un error al intentar registrar el proceso.";
                    lblMensajeConfirma.CssClass = "alert alert-danger";
                    pnlAlerta.CssClass = "alert alert-danger";
                }
                lblMensajeConfirma.Visible = true;
                pnlAlerta.Visible = true;
            }
            else
            {
                saveFolder = GetSaveFolderByEsquema();

                if (txtClave.Text != "")
                {
                    var claveCar = txtClave.Text.ToString();
                    int longClave = claveCar.Length;
                    if (longClave <= 40)
                    {
                        ddlAnio.Enabled = false;
                        ddlMes.Enabled = false;
                        ddlEmpresa.Enabled = false;
                        ddlMoneda.Enabled = false;
                        txtClave.Enabled = false;
                        fuClaroFile1.Enabled = false;
                        if (!string.IsNullOrWhiteSpace(txtClave.Text)
                            && !string.IsNullOrEmpty(ddlAnio.SelectedValue)
                            && !string.IsNullOrEmpty(ddlMes.SelectedValue)
                            && !string.IsNullOrEmpty(ddlEmpresa.SelectedValue)
                            && !string.IsNullOrEmpty(ddlMoneda.SelectedValue)
                            && !string.IsNullOrEmpty(saveFolder)
                            )
                        {
                            SetDisponibilidadControles(false);

                            if (ValidaExisteCargaMismmaClave(txtClave.Text))
                            {
                                EstableceMensajeAlertDanger("La clave ingresada ya existe en el sistema.");
                                SetDisponibilidadControles(true);
                            }
                            else
                            {
                                if (TransfiereArchivos())
                                {
                                    if (RegistraHistProcesoETL())
                                    {
                                        lblMensajeConfirma.Text = "Instrucción registrada correctamente en el sistema.";
                                    }
                                    else
                                    {
                                        EstableceMensajeAlertDanger("Se ha generado un error al intentar registrar el proceso.");
                                    }
                                }
                                else
                                {
                                    EstableceMensajeAlertDanger("Ha ocurrido un error al tratar de transferir los archivos.");
                                }
                            }

                            lblMensajeConfirma.Visible = true;
                            pnlAlerta.Visible = true;
                        }
                    }
                    else
                    {
                        lblTituloModalMsn.Text = "¡Atención!";
                        lblBodyModalMsn.Text = "La clave de carga debe ser de 40 Caracteres.";
                        mpeEtqMsn.Show();
                    }
                }
                else
                {
                    lblTituloModalMsn.Text = "¡Atención!";
                    lblBodyModalMsn.Text = "Debe de ingresar una clave de carga.";
                    mpeEtqMsn.Show();

                }

                
            }
            btnBack.Enabled = true;
        }
        private void SetDisponibilidadControles(bool disponibilidad)
        {
            ddlAnio.Enabled = disponibilidad;
            ddlMes.Enabled = disponibilidad;
            ddlEmpresa.Enabled = disponibilidad;
            ddlMoneda.Enabled = disponibilidad;
            txtClave.Enabled = disponibilidad;
            fuClaroFile1.Enabled = disponibilidad;
            btnAceptar.Enabled = disponibilidad;
            btnAceptar.Visible = disponibilidad;
        }
        private void EstableceMensajeAlertDanger(string mensaje)
        {
            lblMensajeConfirma.Text = mensaje;
            lblMensajeConfirma.CssClass = "alert alert-danger";
            pnlAlerta.CssClass = "alert alert-danger";
            lblMensajeConfirma.Visible = true;
            pnlAlerta.Visible = true;
        }
        private bool TransfiereArchivos()
        {
            bool exitoso = false;

            try
            {
                TransfiereArchivo(ref fuClaroFile1, ref lblEstatusArchivo01);

                exitoso = true;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }

            return exitoso;
        }
        private void ObtieneDatosCarga(int icodCarga)
        {
            if (icodCarga > 0)
            {
                ActivaCheckbox(ETLFacturaClaroBrasilHandler.ObtieneValorBandera(icodCarga));
            }
        }
        private void ActivaCheckbox(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                if (Convert.ToInt32(dr["LineasNoReg"]) == 1) { chkCargaLineasNoReg.Checked = true; } else { chkCargaLineasNoReg.Checked = false; }

                if (Convert.ToInt32(dr["SubirInfo"]) == 32) { chkSubirInfo.Checked = true; } else { chkSubirInfo.Checked = false; }

                if (Convert.ToInt32(dr["ActualizaLin"]) == 64) { chkActualizaLineas.Checked = true; } else { chkActualizaLineas.Checked = false; }

                if (Convert.ToInt32(dr["GenDetall"]) == 128) { chkGeneraDetalle.Checked = true; } else { chkGeneraDetalle.Checked = false; }

                if (Convert.ToInt32(dr["GenRes"]) == 256) { chkGeneraResumenes.Checked = true; } else { chkGeneraResumenes.Checked = false; };
            }
        }
    }
}