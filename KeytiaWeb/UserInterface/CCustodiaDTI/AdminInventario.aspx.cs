using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class AdminInventario : System.Web.UI.Page
    {
        protected DataTable dtTipoInventario = new DataTable();
        private string iCodCatalogoDisp;
        private string state;
        private string iCodMarca;
        private string iCodModelo;
        private string iCodTipoDisp;
        private string psNSerie;
        private string psMacAddress;
        private DateTime dtFechaCompra;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            iCodCatalogoDisp = Request.QueryString["iCodCatDisp"];
            state = KeytiaServiceBL.Util.Decrypt(Request.QueryString["st"]);
            chbPendiente.Attributes.Add("onclick", "actualizaMacAddress()");

            if (!Page.IsPostBack)
            {
                FillDropDowns();

                if ((state == "delete" || state == "edit") && !String.IsNullOrEmpty(iCodCatalogoDisp))
                {
                    FillControls();

                    switch (state)
                    {
                        case "delete":
                            DeshabilitarParaBorrar();
                            break;

                        case "edit":
                            DeshabilitarParaEdicion();
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    //Preparar estatus de alta.
                    lbtnRegresarPaginaBusq.Text = "Ir a la búsqueda de resultados";
                    lblMensajeTipoEstado.Text = "Alta de nuevo inventario en sistema";
                    lblMensajeTipoEstado.ForeColor = System.Drawing.Color.Green;
                    lblMensajeTipoEstado.Visible = true;
                }

            }

        }

        protected void DeshabilitarParaBorrar()
        {
            //drpMarca.Enabled = false;
            //ccdMarca.Enabled = false;
            //drpModelo.Enabled = false;
            //ccdModelo.Enabled = false;

            drpTipoAparato.Enabled = false;
            txtNoSerie.Enabled = false;
            txtMacAddress.Enabled = false;
            txtFechaCompra.Enabled = false;
            chbPendiente.Enabled = false;

            btnGuardar.Text = "Borrar";
            lblMensajeTipoEstado.Text = "Para confirmar la eliminación del inventario de este equipo de click en Borrar";
            lblMensajeTipoEstado.ForeColor = System.Drawing.Color.Red;
            lblMensajeTipoEstado.Visible = true;
        }

        protected void DeshabilitarParaEdicion()
        {
            //txtNoSerie.Enabled = false;

            btnGuardar.Text = "Guardar cambios";
            lblMensajeTipoEstado.Text = "Para guardar los cambios en la edición de inventario es necesario dar click en Guardar cambios";
            lblMensajeTipoEstado.ForeColor = System.Drawing.Color.Blue;
            lblMensajeTipoEstado.Visible = true;
        }

        protected void FillControls()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT Marca = MarcaDisp, MarcaDispDesc, Modelo = ModeloDisp, ModeloDispDesc, TipoDispositivo, \r");
            lsbQuery.Append("NSerie, MacAddress, FechaCompra \r");
            lsbQuery.Append("FROM [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("and iCodCatalogo = " + iCodCatalogoDisp);

            DataTable dtDispositivo = DSODataAccess.Execute(lsbQuery.ToString());

            if (dtDispositivo.Rows.Count == 1)
            {
                if (state == "delete")
                {
                    drpMarca.Visible = false;
                    drpMarca.Enabled = false;
                    ccdMarca.Enabled = false;

                    drpModelo.Visible = false;
                    drpModelo.Enabled = false;
                    ccdModelo.Enabled = false;

                    txtMarca.Visible = true;
                    txtModelo.Visible = true;

                    txtMarca.Text = dtDispositivo.Rows[0]["MarcaDispDesc"].ToString();
                    txtModelo.Text = dtDispositivo.Rows[0]["ModeloDispDesc"].ToString();
                }
                else
                {
                    ccdMarca.SelectedValue = dtDispositivo.Rows[0]["Marca"].ToString();

                    ccdModelo.SelectedValue = dtDispositivo.Rows[0]["Modelo"].ToString();
                }

                drpTipoAparato.SelectedValue = dtDispositivo.Rows[0]["TipoDispositivo"].ToString();

                txtNoSerie.Text = dtDispositivo.Rows[0]["NSerie"].ToString();
                txtMacAddress.Text = dtDispositivo.Rows[0]["MacAddress"].ToString();

                if (dtDispositivo.Rows[0]["MacAddress"].ToString() == "PENDIENTE")
                {
                    chbPendiente.Checked = true;
                }

                DateTime ldtFechaInicio = new DateTime();
                ldtFechaInicio = Convert.ToDateTime(dtDispositivo.Rows[0]["FechaCompra"].ToString());

                txtFechaCompra.Text = ldtFechaInicio.ToString("dd/MM/yyyy");

            }
        }

        protected void FillDropDowns()
        {
            FillDDLTipoInventario();
        }

        protected void FillDDLTipoInventario()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT vchDescripcion = Español, iCodCatalogo \r");
            lsbQuery.Append("FROM [VisHistoricos('TipoDispositivo','Tipos de Dispositivo','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("order by vchDescripcion");

            dtTipoInventario = DSODataAccess.Execute(lsbQuery.ToString());

            drpTipoAparato.DataSource = dtTipoInventario;
            drpTipoAparato.DataValueField = "iCodCatalogo";
            drpTipoAparato.DataTextField = "vchDescripcion";
            drpTipoAparato.DataBind();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            bool result = false;
            string lsMensajeAlert = String.Empty;

            switch (state)
            {
                case "delete":
                    result = bajaInventario();
                    break;
                case "edit":
                    result = editaInventario(out lsMensajeAlert);
                    break;
                case "alta":
                    result = altaInventario(out lsMensajeAlert);
                    break;
                default:
                    result = altaInventario(out lsMensajeAlert);
                    break;
            }

            if (result)
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/CCustodiaDTI/BusquedaInventarioDisp.aspx");
            }
            else
            {
                string script;

                if (lsMensajeAlert != String.Empty)
                {
                    script = @"<script type='text/javascript'>alerta('{0}');</script>";

                    script = string.Format(script, lsMensajeAlert);
                }
                else
                {
                    //Mostrar un mensaje notificando que el dispositivo fue actualizado correctamente.
                    script = @"<script type='text/javascript'>alerta(""La operación no logró realizarse exitosamente, vuelva a intentar."");</script>";

                }

                ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);

            }


        }

        protected bool bajaInventario()
        {
            StringBuilder lsbUpdate = new StringBuilder();

            lsbUpdate.Append("update [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] \r");
            lsbUpdate.Append("set dtFinVigencia = " + "'" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', \r");
            lsbUpdate.Append("dtFecUltAct = getdate(), ");
            lsbUpdate.Append("iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"].ToString() + "\r");
            lsbUpdate.Append("where iCodCatalogo = " + iCodCatalogoDisp + "\r");
            lsbUpdate.Append("and dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()");

            return DSODataAccess.ExecuteNonQuery(lsbUpdate.ToString());
        }

        protected bool editaInventario(out string lsMensaje)
        {
            //20140310 AM. Se agrega un bool para validar si los campos requeridos contienen información
            //leerDatosInventario();
            bool validaCampos = leerDatosInventario();

            StringBuilder lsbUpdateHist = new StringBuilder();
            StringBuilder lsbUpdateCat = new StringBuilder();
            lsMensaje = String.Empty;

            //20140310 AM. Se agrega la validacion del bool validaCampos
            if (dtFechaCompra != DateTime.MinValue
                    && validarNoExisteNSerie(psNSerie, dtFechaCompra.ToString("yyyy-MM-dd HH:mm:ss.fff"), out lsMensaje)
                    && validarNoExisteMacAddress(psMacAddress, dtFechaCompra.ToString("yyyy-MM-dd HH:mm:ss.fff"), ref lsMensaje) && validaCampos)
            {
                /*Update en Historicos*/
                lsbUpdateHist.Append("UPDATE [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] \r");
                lsbUpdateHist.Append("SET TipoDispositivo = " + iCodTipoDisp + ", \r");
                lsbUpdateHist.Append("MarcaDisp = " + iCodMarca + ", \r");
                lsbUpdateHist.Append("ModeloDisp = " + iCodModelo + ", \r");
                lsbUpdateHist.Append("FechaCompra = '" + dtFechaCompra.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', \r");
                //RZ.20130715 Se retira en la edicion el update a la fecha inicio de vigencia.
                //lsbUpdateHist.Append("dtIniVigencia = '" + dtFechaCompra.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', \r");
                lsbUpdateHist.Append("NSerie = '" + psNSerie + "', \r");
                lsbUpdateHist.Append("macAddress = '" + psMacAddress + "', \r");
                lsbUpdateHist.Append("dtFecUltAct = GETDATE(), ");
                lsbUpdateHist.Append("iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"].ToString() + "\r");
                lsbUpdateHist.Append("where iCodCatalogo = " + iCodCatalogoDisp + "\r");
                lsbUpdateHist.Append("and dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()");

                /*Update en catalogos para el campo vchCodigo*/
                lsbUpdateCat.Append("UPDATE catalogos \r");
                lsbUpdateCat.Append("SET vchCodigo = '" + psNSerie + "', \r");
                lsbUpdateCat.Append("vchDescripcion = '" + psNSerie + " (" + iCodMarca + ")' \r");
                lsbUpdateCat.Append("where iCodRegistro = " + iCodCatalogoDisp + "\r");
                lsbUpdateCat.Append("and dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()");


                bool lbEditaH = DSODataAccess.ExecuteNonQuery(lsbUpdateHist.ToString());
                bool lbEditaC = DSODataAccess.ExecuteNonQuery(lsbUpdateCat.ToString());

                return lbEditaH && lbEditaC;
            }
            else
            {
                return false;
            }
        }

        protected bool altaInventario(out string lsMensaje)
        {
            DALCCustodia grabarDatos = new DALCCustodia();
            //20140310 AM. Se agrega un bool para validar si los campos requeridos contienen información
            //leerDatosInventario();
            bool validaCampos = leerDatosInventario();
            lsMensaje = String.Empty;

            //20140310 AM. Se agrega la validacion del bool validaCampos
            if (dtFechaCompra != DateTime.MinValue
                && validarNoExisteNSerie(psNSerie, dtFechaCompra.ToString("yyyy-MM-dd HH:mm:ss.fff"), out lsMensaje)
                && validarNoExisteMacAddress(psMacAddress, dtFechaCompra.ToString("yyyy-MM-dd HH:mm:ss.fff"), ref lsMensaje) && validaCampos)
            {
                return grabarDatos.altaInventario(iCodMarca, iCodModelo, iCodTipoDisp, psNSerie, psMacAddress, dtFechaCompra);
            }
            else
            {
                return false;
            }
        }

        /*RZ.20130715 Se agrega metodo que sirve para validar si el nserie existe o no.
         En caso de existir no lo dara de alta y notificará al usuario que ya se encuentra
         dado de alta un no. de serie*/
        protected bool validarNoExisteNSerie(string lsNSerie, string lsFechaInicioVig, out string lsMensaje)
        {
            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.Append("SELECT iCodCatalogo, dtIniVigencia, dtFinVigencia \r");
            lsbConsulta.Append("FROM [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] \r");
            lsbConsulta.Append("WHERE NSerie = '" + lsNSerie + "' \r");
            lsbConsulta.Append("AND dtIniVigencia <> dtFinVigencia \r");
            lsbConsulta.Append("AND ('" + lsFechaInicioVig + "' between dtIniVigencia and dtFinVigencia \r");
            //20150722 NZ Se agrega validacion de la fecha de fin de vigecia.
            lsbConsulta.Append("OR '2079-01-01 00:00:00' between dtIniVigencia and dtFinVigencia )"); 

            //Si se trata de edicion entonces excluir el icodcatalogo del nserie que se esta modificando
            if (state == "edit")
            {
                lsbConsulta.Append("\r and iCodCatalogo <> " + iCodCatalogoDisp);
            }

            DataTable dtNoSerieDuplicados = DSODataAccess.Execute(lsbConsulta.ToString());

            if (dtNoSerieDuplicados.Rows.Count > 0)
            {
                //Se encontraron registros con vigencia y mismo numero de serie, notificar a usuario
                StringBuilder lsbMensaje = new StringBuilder();

                lsbMensaje.Append("Se encontraron No. de series con vigencia en los siguientes periodos: ");
                foreach (DataRow dr in dtNoSerieDuplicados.Rows)
                {
                    DateTime dtIniVigencia = (DateTime)dr["dtIniVigencia"];
                    DateTime dtFinVigencia = (DateTime)dr["dtFinVigencia"];

                    lsbMensaje.Append(dtIniVigencia.ToString("dd-MM-yyyy") + " - " + dtFinVigencia.ToString("dd-MM-yyyy") + ",");
                }

                //retirar la ultima (,)
                lsbMensaje.Remove(lsbMensaje.Length - 1, 1);
                lsbMensaje.Append(" Es necesario dar de baja los no. de serie de esos periodos ");
                lsbMensaje.Append("ó modificar la fecha de compra del nuevo registro.");

                lsMensaje = lsbMensaje.ToString();
                return false;
            }
            else
            {
                lsMensaje = String.Empty;
                return true;
            }
        }

        protected bool validarNoExisteMacAddress(string lsMacAddress, string lsFechaInicioVig, ref string lsMensaje)
        {
            if (lsMacAddress != "PENDIENTE")
            {
                StringBuilder lsbConsulta = new StringBuilder();

                lsbConsulta.Append("SELECT iCodCatalogo, dtIniVigencia, dtFinVigencia \r");
                lsbConsulta.Append("FROM [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] \r");
                lsbConsulta.Append("WHERE macAddress = '" + lsMacAddress + "' \r");
                lsbConsulta.Append("AND dtIniVigencia <> dtFinVigencia \r");
                lsbConsulta.Append("AND ('" + lsFechaInicioVig + "' between dtIniVigencia and dtFinVigencia \r");
                //20150722 NZ Se agrega validacion de la fecha de fin de vigecia.
                lsbConsulta.Append("OR '2079-01-01 00:00:00' between dtIniVigencia and dtFinVigencia )"); 

                //Si se trata de edicion entonces excluir el icodcatalogo del nserie que se esta modificando
                if (state == "edit")
                {
                    lsbConsulta.Append("\r and iCodCatalogo <> " + iCodCatalogoDisp);
                }

                DataTable dtMacAddressDuplicadas = DSODataAccess.Execute(lsbConsulta.ToString());

                if (dtMacAddressDuplicadas.Rows.Count > 0)
                {
                    //Se encontraron registros con vigencia y misma macAddress, notificar a usuario
                    StringBuilder lsbMensaje = new StringBuilder();

                    lsbMensaje.Append("Se encontraron MAC Address con vigencia en los siguientes periodos: ");
                    foreach (DataRow dr in dtMacAddressDuplicadas.Rows)
                    {
                        DateTime dtIniVigencia = (DateTime)dr["dtIniVigencia"];
                        DateTime dtFinVigencia = (DateTime)dr["dtFinVigencia"];

                        lsbMensaje.Append(dtIniVigencia.ToString("dd-MM-yyyy") + " - " + dtFinVigencia.ToString("dd-MM-yyyy") + ",");
                    }

                    //retirar la ultima (,)
                    lsbMensaje.Remove(lsbMensaje.Length - 1, 1);
                    lsbMensaje.Append(" Es necesario dar de baja las MAC Address de esos periodos ");
                    lsbMensaje.Append("ó modificar la fecha de compra del nuevo registro.");

                    lsMensaje = lsMensaje + lsbMensaje.ToString();
                    return false;
                }
                else
                {
                    lsMensaje = lsMensaje + String.Empty;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        //20140310 AM. Se modifica el metodo leerDatosInventario() para que regrese un dato booleano y saber si los campos requeridos contienen información.
        protected bool leerDatosInventario()
        {
            iCodMarca = drpMarca.SelectedValue;
            iCodModelo = drpModelo.SelectedValue;
            iCodTipoDisp = drpTipoAparato.SelectedValue;
            psNSerie = txtNoSerie.Text;
            psMacAddress = txtMacAddress.Text;
            dtFechaCompra = DateTime.MinValue;

            if (txtFechaCompra.Text != String.Empty)
            {
                dtFechaCompra = Convert.ToDateTime(txtFechaCompra.Text.ToString());
            }

            if (String.IsNullOrEmpty(iCodMarca) || iCodMarca == "0" || String.IsNullOrEmpty(iCodModelo) || iCodModelo == "0"
                || String.IsNullOrEmpty(txtNoSerie.Text))
            {
                //Mostrar un mensaje
                string script = @"<script type='text/javascript'>alerta(""Los campos Marca, Modelo y No. de Serie son requeridos"");</script>";
                ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);
                return false;
            }
            else
            {
                return true;
            }
        }

        protected void lbtnRegresarPaginaBusq_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/CCustodiaDTI/BusquedaInventarioDisp.aspx?stAI=2");

        }

    }
}
