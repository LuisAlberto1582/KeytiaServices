using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net;
using KeytiaWeb.UserInterface.DashboardLT;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class AdministracionDeEnlaces : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RepDetalleDeUbicacion();

            //OcultarFormAgregado();

            if (!IsPostBack)
            {
                btnEliminarEquiposDeSeguridad.Attributes.CssStyle.Add("display", "none");
                BtnEliminarEnlace.Attributes.CssStyle.Add("display", "none");
                BtnEditarEnlace.Attributes.CssStyle.Add("display", "none");
                BtnGuardarEnlace.Attributes.CssStyle.Add("display", "none");
                BtnGuardarEdicion.Attributes.CssStyle.Add("display", "none");
                BtnCancelarEdicion.Attributes.CssStyle.Add("display", "none");
                BtnCancelarIngresoRegistro.Attributes.CssStyle.Add("display", "none");
                AjustarControlesReadOnly();
                OcultarCajeros();
                AgregaInformacionUnRegistro();
            }

            //OcultarEquiposSeguridad();

            // Agregamos valores a los controles si el parametro enlace se encuentra disponible
            if (Request.QueryString["enlace"] != null)
            {
                if (!IsPostBack)
                {
                    AgregarInformacionControles(Request.QueryString["enlace"]);
                    BtnEliminarEnlace.Attributes.CssStyle.Add("display", "");
                    BtnEditarEnlace.Attributes.CssStyle.Add("display", "");
                    AjustarControlesReadOnly();
                    MostrarCajeros();
                    MostrarEquiposSeguridad();
                }
                //AjustarControlesReadOnly();
                //BtnEliminarEnlace.Attributes.CssStyle.Add("display", "");
                //BtnEditarEnlace.Attributes.CssStyle.Add("display", "");
            }

            DataTable dummy = new DataTable();
            dummy.Columns.Add("NombreCajero");
            dummy.Columns.Add("IdCajero");
            dummy.Columns.Add("TipoCajero");
            dummy.Columns.Add("FechaActivacion");
            dummy.Columns.Add("CantidadCajeros");
            dummy.Columns.Add("IpLookback");
            dummy.Columns.Add("IpGateway");
            dummy.Columns.Add("IpMasc");
            dummy.Columns.Add("IpWan");
            dummy.Columns.Add("IpAlarma");
            dummy.Columns.Add("IpCamara");
            dummy.Columns.Add("IpTunel");
            dummy.Rows.Add();

            GridView1.DataSource = dummy;
            GridView1.DataBind();

            GridView1.UseAccessibleHeader = true;
            GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

        }

        private void RepDetalleDeUbicacion()
        {

            //string tituloReporte = "Detalle de Ubicación";
            string tituloReporte = "";

            DataTable dt = DSODataAccess.Execute(ConsultaDetalleDeUbicacion());

            if (dt != null && dt.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

                GridView grid = DTIChartsAndControls.GridView("ReportePrincipal", dt, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dt.AcceptChanges();
                Rep1.Controls.Clear();
                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));
            }
        }

        protected void AgregaInformacionUnRegistro()
        {
            // Si la tabla tiene solo un solo registro se ingresan los datos en la interfaz para mostrar al cliente
            DataTable dtConsulta;

            dtConsulta = DSODataAccess.Execute(BusquedaCajeros());

            if (dtConsulta.Rows.Count == 1)
            {
                // Id de cajero = dtConsulta.Rows[0][1].ToString()
                AgregarInformacionControles(dtConsulta.Rows[0][1].ToString());
                MostrarEquiposSeguridad();
                MostrarCajeros();
            }
        }

        protected void AgregarEquipoSeguridad_Click(object sender, EventArgs e)
        {
            MostrarEquiposSeguridad();
            AjustarControlesEdicion();

            btnEliminarEquiposDeSeguridad.Attributes.CssStyle.Add("display", "");
            btnAgregarEquiposDeSeguridad.Attributes.CssStyle.Add("display", "none");

            // Estos campos solo se agregaran automáticamente si se da click en bandera de seguridad
            string ip = ObtenerRangoIp();
            string nuevaIpSinCero = ip.Remove(ip.Length - 1, 1);

            IpCajAlarma.Text = nuevaIpSinCero + "121";
            txtBoxCajIpLanAlarma.Text = nuevaIpSinCero + "122";
            IpCajCamara.Text = nuevaIpSinCero + "125";
            txtBoxCajIpLanCamara.Text = nuevaIpSinCero + "126";

        }

        protected void MostrarEquiposSeguridad()
        {
            lblIpCajAlarma.Attributes.CssStyle.Add("display", "");
            IpCajAlarma.Attributes.CssStyle.Add("display", "");
            lblIpLanAlarma.Attributes.CssStyle.Add("display", "");
            txtBoxCajIpLanAlarma.Attributes.CssStyle.Add("display", "");

            lblIpCctv.Attributes.CssStyle.Add("display", "");
            IpCajCamara.Attributes.CssStyle.Add("display", "");
            lblIpCajLanCctv.Attributes.CssStyle.Add("display", "");
            txtBoxCajIpLanCamara.Attributes.CssStyle.Add("display", "");
        }

        protected void OcultarEquiposSeguridad()
        {
            lblIpCajAlarma.Attributes.CssStyle.Add("display", "none");
            IpCajAlarma.Attributes.CssStyle.Add("display", "none");
            lblIpLanAlarma.Attributes.CssStyle.Add("display", "none");
            txtBoxCajIpLanAlarma.Attributes.CssStyle.Add("display", "none");

            lblIpCctv.Attributes.CssStyle.Add("display", "none");
            IpCajCamara.Attributes.CssStyle.Add("display", "none");
            lblIpCajLanCctv.Attributes.CssStyle.Add("display", "none");
            txtBoxCajIpLanCamara.Attributes.CssStyle.Add("display", "none");
        }

        protected void LimpiarCamposSeguridad()
        {
            IpCajAlarma.Text = "";
            txtBoxCajIpLanAlarma.Text = "";
            IpCajCamara.Text = "";
            txtBoxCajIpLanCamara.Text = "";
        }

        protected void EliminarBotonEquiposSeguridad_Click(object sender, EventArgs e)
        {
            OcultarEquiposSeguridad();
            LimpiarCamposSeguridad();

            btnAgregarEquiposDeSeguridad.Attributes.CssStyle.Add("display", "");
            btnEliminarEquiposDeSeguridad.Attributes.CssStyle.Add("display", "none");
        }

        protected void MostrarFormAgregado()
        {
            contenedorFormAgregar.Attributes.CssStyle.Add("display", "");
        }

        protected void OcultarFormAgregado()
        {
            contenedorFormAgregar.Attributes.CssStyle.Add("display", "none");
        }

        protected void AgregarEnlace_Click(object sender, EventArgs e)
        {
            //MostrarFormAgregado();

            OcultarCajeros();
            OcultarEquiposSeguridad();
            MostrarBotonesCajeros();
            AjustarControlesEdicion();
            LimpiarFormulario();

            btnAgregarCajero1div.Attributes.CssStyle.Add("display", "");
            btnAgregarEquiposDeSeguridad.Attributes.CssStyle.Add("display", "");
            BtnCancelarIngresoRegistro.Attributes.CssStyle.Add("display", "");
            BtnGuardarEnlace.Attributes.CssStyle.Add("display", "");
            BtnEliminarEnlace.Attributes.CssStyle.Add("display", "none");
            BtnEditarEnlace.Attributes.CssStyle.Add("display", "none");

            // Agregar campos de IP's automáticamente
            // Ejecutar consulta y obtener rango disponible. Posteriormente asignar a cajero y Lan

            // Asignamos todas las IP's automáticamente
            string ip = ObtenerRangoIp();
            string nuevaIpSinCero = ip.Remove(ip.Length - 1, 1);

            IpCajLookback.Text = nuevaIpSinCero + "1";
            IpCajGateway.Text = nuevaIpSinCero + "2";
            IpCajMasc.Text = "255.255.255.252";

            cajeroIp1.Text = nuevaIpSinCero + "10";
            txtBoxCajIpLan1.Text = nuevaIpSinCero + "9";


        }

        protected void AjustarControlesReadOnly()
        {
            TipoDeEnlace.Enabled = false;
            NombreDeCajero.ReadOnly = true;
            IdDeCajero.ReadOnly = true;
            FechaActCajero.ReadOnly = true;
            IpCajLookback.ReadOnly = true;
            IpCajGateway.ReadOnly = true;
            IpCajMasc.ReadOnly = true;
            IpCajWan.ReadOnly = true;
            IpCajAlarma.ReadOnly = true;
            txtBoxCajIpLanAlarma.ReadOnly = true;
            IpCajCamara.ReadOnly = true;
            txtBoxCajIpLanCamara.ReadOnly = true;
            IpCajTunel.ReadOnly = true;
            cajeroIp1.ReadOnly = true;
            cajeroIp2.ReadOnly = true;
            cajeroIp3.ReadOnly = true;
            cajeroIp4.ReadOnly = true;
            cajeroIp5.ReadOnly = true;
            TipoDeCajero1.Enabled = false;
            TipoDeCajero2.Enabled = false;
            TipoDeCajero3.Enabled = false;
            TipoDeCajero4.Enabled = false;
            TipoDeCajero5.Enabled = false;
            txtBoxCajIpLan1.ReadOnly = true;
            txtBoxCajIpLan2.ReadOnly = true;
            txtBoxCajIpLan3.ReadOnly = true;
            txtBoxCajIpLan4.ReadOnly = true;
            txtBoxCajIpLan5.ReadOnly = true;

        }

        protected void AjustarControlesEdicion()
        {
            TipoDeEnlace.Enabled = true;
            NombreDeCajero.ReadOnly = false;
            IdDeCajero.ReadOnly = false;
            FechaActCajero.ReadOnly = false;
            //IpCajLookback.ReadOnly = false;
            //IpCajGateway.ReadOnly = false;
            //IpCajMasc.ReadOnly = false;
            IpCajWan.ReadOnly = false;
            //IpCajAlarma.ReadOnly = false;
            //IpCajCamara.ReadOnly = false;
            //IpCajTunel.ReadOnly = false;
            //cajeroIp1.ReadOnly = false;
            //cajeroIp2.ReadOnly = false;
            //cajeroIp3.ReadOnly = false;
            //cajeroIp4.ReadOnly = false;
            //cajeroIp5.ReadOnly = false;
            TipoDeCajero1.Enabled = true;
            TipoDeCajero2.Enabled = true;
            TipoDeCajero3.Enabled = true;
            TipoDeCajero4.Enabled = true;
            TipoDeCajero5.Enabled = true;
            //BtnAsignarIpCaj1.Enabled = true;
            //BtnAsignarIpCaj2.Enabled = true;
            //BtnAsignarIpCaj3.Enabled = true;
            //BtnAsignarIpCaj4.Enabled = true;
            //BtnAsignarIpCaj5.Enabled = true;
        }

        protected void LimpiarFormulario()
        {
            NombreDeCajero.Text = "";
            IdDeCajero.Text = "";
            FechaActCajero.Text = "";
            cajeroIp1.Text = "";
            cajeroIp2.Text = "";
            cajeroIp3.Text = "";
            cajeroIp4.Text = "";
            cajeroIp5.Text = "";
            IpCajLookback.Text = "";
            IpCajGateway.Text = "";
            IpCajMasc.Text = "";
            IpCajWan.Text = "";
            IpCajAlarma.Text = "";
            IpCajCamara.Text = "";
            IpCajTunel.Text = "";
        }

        public static string ObtieneFecha(string date)
        {
            var fechaObj = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            //return fechaObj.ToString("dd-MM-yyyy");
            return fechaObj.ToString("yyyy-MM-dd");
        }

        protected void AgregarInformacionControles(string id)
        {
            var dt = BusquedaIdCajero(id);
            var fechaDt = dt.Rows[0][3].ToString().Split(' ');

            var test = ObtieneFecha(fechaDt[0]);

            NombreDeCajero.Text = dt.Rows[0][0].ToString();
            IdDeCajero.Text = dt.Rows[0][1].ToString();
            TipoDeEnlace.SelectedValue = dt.Rows[0][2].ToString();
            FechaActCajero.Text = ObtieneFecha(fechaDt[0]);
            IpCajLookback.Text = dt.Rows[0][4].ToString();
            IpCajGateway.Text = dt.Rows[0][5].ToString();
            IpCajMasc.Text = dt.Rows[0][6].ToString();
            IpCajWan.Text = dt.Rows[0][7].ToString();
            IpCajAlarma.Text = dt.Rows[0][8].ToString();
            IpCajCamara.Text = dt.Rows[0][9].ToString();
            IpCajTunel.Text = dt.Rows[0][10].ToString();

            if (!string.IsNullOrEmpty(dt.Rows[0][11].ToString()))
            {
                cajeroIp1.Text = dt.Rows[0][11].ToString();
                TipoDeCajero1.SelectedValue = dt.Rows[0][12].ToString();
            }
            else
            {
                cajeroIp1.Text = "";
            }

            if (!string.IsNullOrEmpty(dt.Rows[0][13].ToString()))
            {
                cajeroIp2.Text = dt.Rows[0][13].ToString();
                TipoDeCajero2.SelectedValue = dt.Rows[0][14].ToString();
            }
            else
            {
                cajeroIp2.Text = "";
            }

            if (!string.IsNullOrEmpty(dt.Rows[0][15].ToString()))
            {
                cajeroIp3.Text = dt.Rows[0][15].ToString();
                TipoDeCajero3.SelectedValue = dt.Rows[0][16].ToString();
            }
            else
            {
                cajeroIp3.Text = "";
            }

            if (!string.IsNullOrEmpty(dt.Rows[0][17].ToString()))
            {
                cajeroIp4.Text = dt.Rows[0][17].ToString();
                TipoDeCajero4.SelectedValue = dt.Rows[0][18].ToString();
            }
            else
            {
                cajeroIp4.Text = "";
            }

            if (!string.IsNullOrEmpty(dt.Rows[0][19].ToString()))
            {
                cajeroIp5.Text = dt.Rows[0][19].ToString();
                TipoDeCajero5.SelectedValue = dt.Rows[0][20].ToString();
            }
            else
            {
                cajeroIp5.Text = "";
            }
        }

        protected void BtnEliminar_Click(object sender, EventArgs e)
        {
            string nuevaIpSinCero = IpCajLookback.Text.Remove(IpCajLookback.Text.Length - 1, 1);
            string ipConCeroCuartoOcteto = nuevaIpSinCero + "0";

            // Eliminar registro de DB
            var qry = DeleteEnlaceCajero(IdDeCajero.Text);
            DSODataAccess.Execute(qry);

            // Damos de baja la asignacion de ubicacion y fecha de asignacion para el rango de IP
            DSODataAccess.Execute(UpdateTablaRangoIpsBaja(ipConCeroCuartoOcteto));

            // Limpiar datos de txtboxes
            LimpiarFormulario();
            // Quitar enlace o redireccionar a URL anterior a enlace
            string sample = "AdministracionDeEnlaces.aspx?qry=" + Request.QueryString["qry"];
            Response.Redirect("AdministracionDeEnlaces.aspx?qry=" + Request.QueryString["qry"]);
        }

        protected void BtnCancelarEdicion_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdministracionDeEnlaces.aspx?qry=" + Request.QueryString["qry"] + "&enlace=" + Request.QueryString["enlace"]);
        }

        protected void BtnCancelarIngresoRegistro_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdministracionDeEnlaces.aspx?qry=" + Request.QueryString["qry"]);
        }

        protected void BtnEditar_Click(object sender, EventArgs e)
        {
            AjustarControlesEdicion();

            // Desaparecen botones Guardar, Editar y Eliminar
            BtnEliminarEnlace.Attributes.CssStyle.Add("display", "none");
            BtnEditarEnlace.Attributes.CssStyle.Add("display", "none");
            BtnGuardarEnlace.Attributes.CssStyle.Add("display", "none");
            BtnGuardarEdicion.Attributes.CssStyle.Add("display", "");
            BtnCancelarEdicion.Attributes.CssStyle.Add("display", "");
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            string ipCajLookback;
            string ipCajGateway;
            string ipCajMasc;
            string ipCajWan;
            string ipCajAlarma;
            string ipCajCamara;
            string ipCajTunel;

            string ipCajero1;
            string ipCajero2;
            string ipCajero3;
            string ipCajero4;
            string ipCajero5;

            string tipoCajero1;
            string tipoCajero2;
            string tipoCajero3;
            string tipoCajero4;
            string tipoCajero5;

            string fechaCajero;

            string qry;

            string mensajeIps = "";

            #region Asignacion y validacion de IPs Lookback - Tunel

            if (IpCajLookback.Text != "")
            {
                if (ValidaDireccionIp(IpCajLookback.Text))
                {
                    ipCajLookback = "'" + IpCajLookback.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Lookback\\n";
                    ipCajLookback = "NULL";
                }
            }
            else
            {
                ipCajLookback = "NULL";
            }

            if (IpCajGateway.Text != "")
            {
                if (ValidaDireccionIp(IpCajGateway.Text))
                {
                    ipCajGateway = "'" + IpCajGateway.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Gateway\\n";
                    ipCajGateway = "NULL";
                }
            }
            else
            {
                ipCajGateway = "NULL";
            }

            if (IpCajMasc.Text != "")
            {
                if (ValidaDireccionIp(IpCajMasc.Text))
                {
                    ipCajMasc = "'" + IpCajMasc.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Masc\\n";
                    ipCajMasc = "NULL";
                }
            }
            else
            {
                ipCajMasc = "NULL";
            }

            if (IpCajWan.Text != "")
            {
                if (ValidaDireccionIp(IpCajWan.Text))
                {
                    ipCajWan = "'" + IpCajWan.Text + "'";
                }
                else
                {
                    mensajeIps += "IP WAN\\n";
                    ipCajWan = "NULL";
                }
            }
            else
            {
                ipCajWan = "NULL";
            }

            if (IpCajAlarma.Text != "")
            {
                if (ValidaDireccionIp(IpCajAlarma.Text))
                {
                    ipCajAlarma = "'" + IpCajAlarma.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Alarma\\n";
                    ipCajAlarma = "NULL";
                }
            }
            else
            {
                ipCajAlarma = "NULL";
            }

            if (IpCajCamara.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp1.Text))
                {
                    ipCajCamara = "'" + IpCajCamara.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Camara\\n";
                    ipCajCamara = "NULL";
                }
            }
            else
            {
                ipCajCamara = "NULL";
            }

            if (IpCajTunel.Text != "")
            {
                if (ValidaDireccionIp(IpCajTunel.Text))
                {
                    ipCajTunel = "'" + IpCajTunel.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Tunel\\n";
                    ipCajTunel = "NULL";
                }
            }
            else
            {
                ipCajTunel = "NULL";
            }

            #endregion

            #region Asignacion de valores a cajeros

            if (cajeroIp1.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp1.Text))
                {
                    ipCajero1 = "'" + cajeroIp1.Text + "'";
                    tipoCajero1 = TipoDeCajero1.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 1\\n";
                    ipCajero1 = "NULL";
                    tipoCajero1 = "NULL";
                }
            }
            else
            {
                ipCajero1 = "NULL";
                tipoCajero1 = "NULL";
            }

            if (cajeroIp2.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp2.Text))
                {
                    ipCajero2 = "'" + cajeroIp2.Text + "'";
                    tipoCajero2 = TipoDeCajero2.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 2\\n";
                    ipCajero2 = "NULL";
                    tipoCajero2 = "NULL";
                }
            }
            else
            {
                ipCajero2 = "NULL";
                tipoCajero2 = "NULL";
            }

            if (cajeroIp3.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp3.Text))
                {
                    ipCajero3 = "'" + cajeroIp3.Text + "'";
                    tipoCajero3 = TipoDeCajero3.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 3\\n";
                    ipCajero3 = "NULL";
                    tipoCajero3 = "NULL";
                }
            }
            else
            {
                ipCajero3 = "NULL";
                tipoCajero3 = "NULL";
            }

            if (cajeroIp4.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp4.Text))
                {
                    ipCajero4 = "'" + cajeroIp4.Text + "'";
                    tipoCajero4 = TipoDeCajero4.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 4\\n";
                    ipCajero4 = "NULL";
                    tipoCajero4 = "NULL";
                }
            }
            else
            {
                ipCajero4 = "NULL";
                tipoCajero4 = "NULL";
            }

            if (cajeroIp5.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp5.Text))
                {
                    ipCajero5 = "'" + cajeroIp5.Text + "'";
                    tipoCajero5 = TipoDeCajero5.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 5\\n";
                    ipCajero5 = "NULL";
                    tipoCajero5 = "NULL";
                }
            }
            else
            {
                ipCajero5 = "NULL";
                tipoCajero5 = "NULL";
            }

            #endregion

            if (mensajeIps != "")
            {
                string script = "AlertIpsErrones('" + mensajeIps + "');";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
            }


            // Arreglado de fecha a formato MM-dd-yyyy
            if (!string.IsNullOrEmpty(FechaActCajero.Text))
            {
                try
                {
                    fechaCajero = DateTime.ParseExact(FechaActCajero.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                 .ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    fechaCajero = DateTime.ParseExact(FechaActCajero.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                                 .ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
            }
            else
            {
                fechaCajero = DateTime.Now.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
            }

            if (Page.IsValid)
            {
                qry = InsertEnlaceCajero(TipoDeEnlace.Text, IdDeCajero.Text, fechaCajero, ipCajLookback, ipCajGateway,
                               ipCajMasc, ipCajWan, ipCajAlarma, ipCajCamara, ipCajTunel, ipCajero1,
                               tipoCajero1, ipCajero2, tipoCajero2, ipCajero3, tipoCajero3, ipCajero4, tipoCajero4, ipCajero5, tipoCajero5, NombreDeCajero.Text);

                DSODataAccess.Execute(qry);

                // Query actualizacion de Tabla Rangos
                string ip = ObtenerRangoIp();

                qry = UpdateTablaRangoIps(ip, NombreDeCajero.Text);

                DSODataAccess.Execute(qry);

                LimpiarFormulario();
                AgregaInformacionUnRegistro();
                AjustarControlesReadOnly();

                // Desaparece Boton Guardar y Cancelar
                BtnCancelarIngresoRegistro.Attributes.CssStyle.Add("display", "none");
                BtnGuardarEnlace.Attributes.CssStyle.Add("display", "none");

                OcultarEquiposSeguridad();
                OcultarCajeros();
            }
            else
            {
                string mensaje = ValidaCamposForm();
                string script = "MostrarMensajesDeError('" + mensaje + "');";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
            }
        }

        protected void BtnGuardarEdicion_Click(object sender, EventArgs e)
        {
            string ipCajLookback;
            string ipCajGateway;
            string ipCajMasc;
            string ipCajWan;
            string ipCajAlarma;
            string ipCajCamara;
            string ipCajTunel;

            string ipCajero1;
            string ipCajero2;
            string ipCajero3;
            string ipCajero4;
            string ipCajero5;

            string tipoCajero1;
            string tipoCajero2;
            string tipoCajero3;
            string tipoCajero4;
            string tipoCajero5;

            string fechaCajero;

            string qry;

            string mensajeIps = "";

            #region Asignacion y validacion de IPs Lookback - Tunel

            if (IpCajLookback.Text != "")
            {
                if (ValidaDireccionIp(IpCajLookback.Text))
                {
                    ipCajLookback = "'" + IpCajLookback.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Lookback\\n";
                    ipCajLookback = "NULL";
                }
            }
            else
            {
                ipCajLookback = "NULL";
            }

            if (IpCajGateway.Text != "")
            {
                if (ValidaDireccionIp(IpCajGateway.Text))
                {
                    ipCajGateway = "'" + IpCajGateway.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Gateway\\n";
                    ipCajGateway = "NULL";
                }
            }
            else
            {
                ipCajGateway = "NULL";
            }

            if (IpCajMasc.Text != "")
            {
                if (ValidaDireccionIp(IpCajMasc.Text))
                {
                    ipCajMasc = "'" + IpCajMasc.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Masc\\n";
                    ipCajMasc = "NULL";
                }
            }
            else
            {
                ipCajMasc = "NULL";
            }

            if (IpCajWan.Text != "")
            {
                if (ValidaDireccionIp(IpCajWan.Text))
                {
                    ipCajWan = "'" + IpCajWan.Text + "'";
                }
                else
                {
                    mensajeIps += "IP WAN\\n";
                    ipCajWan = "NULL";
                }
            }
            else
            {
                ipCajWan = "NULL";
            }

            if (IpCajAlarma.Text != "")
            {
                if (ValidaDireccionIp(IpCajAlarma.Text))
                {
                    ipCajAlarma = "'" + IpCajAlarma.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Alarma\\n";
                    ipCajAlarma = "NULL";
                }
            }
            else
            {
                ipCajAlarma = "NULL";
            }

            if (IpCajCamara.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp1.Text))
                {
                    ipCajCamara = "'" + IpCajCamara.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Camara\\n";
                    ipCajCamara = "NULL";
                }
            }
            else
            {
                ipCajCamara = "NULL";
            }

            if (IpCajTunel.Text != "")
            {
                if (ValidaDireccionIp(IpCajTunel.Text))
                {
                    ipCajTunel = "'" + IpCajTunel.Text + "'";
                }
                else
                {
                    mensajeIps += "IP Tunel\\n";
                    ipCajTunel = "NULL";
                }
            }
            else
            {
                ipCajTunel = "NULL";
            }

            #endregion

            #region Asignacion de valores a cajeros

            if (cajeroIp1.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp1.Text))
                {
                    ipCajero1 = "'" + cajeroIp1.Text + "'";
                    tipoCajero1 = TipoDeCajero1.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 1\\n";
                    ipCajero1 = "NULL";
                    tipoCajero1 = "NULL";
                }
            }
            else
            {
                ipCajero1 = "NULL";
                tipoCajero1 = "NULL";
            }

            if (cajeroIp2.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp2.Text))
                {
                    ipCajero2 = "'" + cajeroIp2.Text + "'";
                    tipoCajero2 = TipoDeCajero2.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 2\\n";
                    ipCajero2 = "NULL";
                    tipoCajero2 = "NULL";
                }
            }
            else
            {
                ipCajero2 = "NULL";
                tipoCajero2 = "NULL";
            }

            if (cajeroIp3.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp3.Text))
                {
                    ipCajero3 = "'" + cajeroIp3.Text + "'";
                    tipoCajero3 = TipoDeCajero3.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 3\\n";
                    ipCajero3 = "NULL";
                    tipoCajero3 = "NULL";
                }
            }
            else
            {
                ipCajero3 = "NULL";
                tipoCajero3 = "NULL";
            }

            if (cajeroIp4.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp4.Text))
                {
                    ipCajero4 = "'" + cajeroIp4.Text + "'";
                    tipoCajero4 = TipoDeCajero4.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 4\\n";
                    ipCajero4 = "NULL";
                    tipoCajero4 = "NULL";
                }
            }
            else
            {
                ipCajero4 = "NULL";
                tipoCajero4 = "NULL";
            }

            if (cajeroIp5.Text != "")
            {
                if (ValidaDireccionIp(cajeroIp5.Text))
                {
                    ipCajero5 = "'" + cajeroIp5.Text + "'";
                    tipoCajero5 = TipoDeCajero5.Text;
                }
                else
                {
                    mensajeIps += "IP Cajero 5\\n";
                    ipCajero5 = "NULL";
                    tipoCajero5 = "NULL";
                }
            }
            else
            {
                ipCajero5 = "NULL";
                tipoCajero5 = "NULL";
            }

            #endregion

            if (mensajeIps != "")
            {
                string script = "AlertIpsErrones('" + mensajeIps + "');";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
            }

            // Arreglado de fecha a formato MM-dd-yyyy
            if (!string.IsNullOrEmpty(FechaActCajero.Text))
            {
                try
                {
                    fechaCajero = DateTime.ParseExact(FechaActCajero.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                 .ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    fechaCajero = DateTime.ParseExact(FechaActCajero.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                                 .ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
            }
            else
            {
                fechaCajero = DateTime.Now.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
            }

            if (Page.IsValid)
            {
                qry = EditarEnlaceCajero(IdDeCajero.Text, TipoDeEnlace.Text, IdDeCajero.Text, fechaCajero, ipCajLookback, ipCajGateway,
                               ipCajMasc, ipCajWan, ipCajAlarma, ipCajCamara, ipCajTunel, ipCajero1,
                               tipoCajero1, ipCajero2, tipoCajero2, ipCajero3, tipoCajero3, ipCajero4, tipoCajero4, ipCajero5, tipoCajero5, NombreDeCajero.Text);

                DSODataAccess.Execute(qry);

                BtnGuardarEdicion.Attributes.CssStyle.Add("display", "none");
                BtnCancelarEdicion.Attributes.CssStyle.Add("display", "none");

            }
            else
            {
                string mensaje = ValidaCamposForm();
                string script = "MostrarMensajesDeError('" + mensaje + "');";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
            }

            AgregarInformacionControles(Request.QueryString["enlace"]);
        }

        public static int DeterminaCantidadCajeros(DataRow row)
        {
            int cantidadCajeros = 0;

            if (!string.IsNullOrEmpty(row[11].ToString()))
            {
                cantidadCajeros++;
            }

            if (!string.IsNullOrEmpty(row[13].ToString()))
            {
                cantidadCajeros++;
            }

            if (!string.IsNullOrEmpty(row[15].ToString()))
            {
                cantidadCajeros++;
            }

            if (!string.IsNullOrEmpty(row[17].ToString()))
            {
                cantidadCajeros++;
            }

            if (!string.IsNullOrEmpty(row[19].ToString()))
            {
                cantidadCajeros++;
            }

            return cantidadCajeros;
        }

        protected static bool ValidaDireccionIp(string IpAddress)
        {
            bool flag;

            try
            {
                IPAddress IP;

                if (IpAddress.Count(c => c == '.') == 3)
                {
                    flag = IPAddress.TryParse(IpAddress, out IP);
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception)
            {
                flag = false;
            }

            return flag;
        }

        protected static string ObtenerIpsUnicas(List<string> muestra, List<string> todasLasIp)
        {
            foreach (var ip in muestra)
            {
                if (todasLasIp.Contains(ip))
                {
                    todasLasIp.Remove(ip);
                }
            }

            return todasLasIp[0];
        }

        #region Botones

        protected void BtnAgregarCajero1_Click(object sender, EventArgs e)
        {
            tipoCajero2div.Attributes.CssStyle.Add("display", "");
            ipCajero2div.Attributes.CssStyle.Add("display", "");
            ipLanCajero2div.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero2div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero2div.Attributes.CssStyle.Add("display", "");

            // Agregar informacion a control

            string ip = ObtenerRangoIp();
            string nuevaIpSinCero = ip.Remove(ip.Length - 1, 1);

            cajeroIp2.Text = nuevaIpSinCero + "6";
            txtBoxCajIpLan2.Text = nuevaIpSinCero + "7";
        }

        protected void BtnAgregarCajero2_Click(object sender, EventArgs e)
        {
            tipoCajero3div.Attributes.CssStyle.Add("display", "");
            ipCajero3div.Attributes.CssStyle.Add("display", "");
            ipLanCajero3div.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero3div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero3div.Attributes.CssStyle.Add("display", "");

            string ip = ObtenerRangoIp();
            string nuevaIpSinCero = ip.Remove(ip.Length - 1, 1);

            cajeroIp3.Text = nuevaIpSinCero + "1";
            txtBoxCajIpLan3.Text = nuevaIpSinCero + "2";
        }

        protected void BtnAgregarCajero3_Click(object sender, EventArgs e)
        {
            tipoCajero4div.Attributes.CssStyle.Add("display", "");
            ipCajero4div.Attributes.CssStyle.Add("display", "");
            ipLanCajero4div.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero4div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero4div.Attributes.CssStyle.Add("display", "");

            string ip = ObtenerRangoIp();
            string nuevaIpSinCero = ip.Remove(ip.Length - 1, 1);

            cajeroIp4.Text = nuevaIpSinCero + "3";
            txtBoxCajIpLan4.Text = nuevaIpSinCero + "4";
        }

        protected void BtnAgregarCajero4_Click(object sender, EventArgs e)
        {
            tipoCajero5div.Attributes.CssStyle.Add("display", "");
            ipCajero5div.Attributes.CssStyle.Add("display", "");
            ipLanCajero5div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero5div.Attributes.CssStyle.Add("display", "");

            string ip = ObtenerRangoIp();
            string nuevaIpSinCero = ip.Remove(ip.Length - 1, 1);

            cajeroIp5.Text = nuevaIpSinCero + "5";
            txtBoxCajIpLan5.Text = nuevaIpSinCero + "6";
        }

        protected void BtnQuitarCajero2_Click(object sender, EventArgs e)
        {
            tipoCajero2div.Attributes.CssStyle.Add("display", "none");
            ipCajero2div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero2div.Attributes.CssStyle.Add("display", "none");
            btnAgregarCajero2div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero2div.Attributes.CssStyle.Add("display", "none");

            cajeroIp2.Text = "";
            txtBoxCajIpLan2.Text = "";
        }

        protected void BtnQuitarCajero3_Click(object sender, EventArgs e)
        {
            tipoCajero3div.Attributes.CssStyle.Add("display", "none");
            ipCajero3div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero3div.Attributes.CssStyle.Add("display", "none");
            btnAgregarCajero3div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero3div.Attributes.CssStyle.Add("display", "none");

            cajeroIp3.Text = "";
            txtBoxCajIpLan3.Text = "";
        }

        protected void BtnQuitarCajero4_Click(object sender, EventArgs e)
        {
            tipoCajero4div.Attributes.CssStyle.Add("display", "none");
            ipCajero4div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero4div.Attributes.CssStyle.Add("display", "none");
            btnAgregarCajero4div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero4div.Attributes.CssStyle.Add("display", "none");

            cajeroIp4.Text = "";
            txtBoxCajIpLan4.Text = "";
        }

        protected void BtnQuitarCajero5_Click(object sender, EventArgs e)
        {
            tipoCajero5div.Attributes.CssStyle.Add("display", "none");
            ipCajero5div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero5div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero5div.Attributes.CssStyle.Add("display", "none");

            cajeroIp5.Text = "";
            txtBoxCajIpLan5.Text = "";
        }

        //protected void BtnAgregarCajero5_Click(object sender, EventArgs e)
        //{
        //    tipoCajero5div.Attributes.CssStyle.Add("display", "");
        //    ipCajero5div.Attributes.CssStyle.Add("display", "");
        //    ipLanCajero2div.Attributes.CssStyle.Add("display", "");
        //    btnAgregarCajero2div.Attributes.CssStyle.Add("display", "");
        //    btnQuitarCajero2div.Attributes.CssStyle.Add("display", "");
        //}

        //protected void BtnAsignarIpCaj1_Click(object sender, EventArgs e)
        //{
        //    List<string> IpsEjemplo = new List<string>();
        //    IpsEjemplo.Add("255.255.255.255");
        //    IpsEjemplo.Add("192.158.1.38");
        //    IpsEjemplo.Add("192.158.1.29");
        //    IpsEjemplo.Add("192.158.2.38");
        //    IpsEjemplo.Add("192.158.5.20");
        //    IpsEjemplo.Add("192.158.1.25");

        //    string resultadoIps = ObtenerIpsUnicas(new List<string> { cajeroIp2.Text, cajeroIp3.Text, cajeroIp4.Text, cajeroIp5.Text }, IpsEjemplo);

        //    cajeroIp1.Text = resultadoIps;
        //}

        //protected void BtnAsignarIpCaj2_Click(object sender, EventArgs e)
        //{
        //    List<string> IpsEjemplo = new List<string>();

        //    IpsEjemplo.Add("255.255.255.255");
        //    IpsEjemplo.Add("192.158.1.38");
        //    IpsEjemplo.Add("192.158.1.29");
        //    IpsEjemplo.Add("192.158.2.38");
        //    IpsEjemplo.Add("192.158.5.20");
        //    IpsEjemplo.Add("192.158.1.25");

        //    string resultadoIps = ObtenerIpsUnicas(new List<string> { cajeroIp1.Text, cajeroIp3.Text, cajeroIp4.Text, cajeroIp5.Text }, IpsEjemplo);

        //    cajeroIp2.Text = resultadoIps;
        //}

        //protected void BtnAsignarIpCaj3_Click(object sender, EventArgs e)
        //{
        //    List<string> IpsEjemplo = new List<string>();

        //    IpsEjemplo.Add("255.255.255.255");
        //    IpsEjemplo.Add("192.158.1.38");
        //    IpsEjemplo.Add("192.158.1.29");
        //    IpsEjemplo.Add("192.158.2.38");
        //    IpsEjemplo.Add("192.158.5.20");
        //    IpsEjemplo.Add("192.158.1.25");

        //    string resultadoIps = ObtenerIpsUnicas(new List<string> { cajeroIp1.Text, cajeroIp2.Text, cajeroIp4.Text, cajeroIp5.Text }, IpsEjemplo);

        //    cajeroIp3.Text = resultadoIps;
        //}

        //protected void BtnAsignarIpCaj4_Click(object sender, EventArgs e)
        //{
        //    List<string> IpsEjemplo = new List<string>();

        //    IpsEjemplo.Add("255.255.255.255");
        //    IpsEjemplo.Add("192.158.1.38");
        //    IpsEjemplo.Add("192.158.1.29");
        //    IpsEjemplo.Add("192.158.2.38");
        //    IpsEjemplo.Add("192.158.5.20");
        //    IpsEjemplo.Add("192.158.1.25");

        //    string resultadoIps = ObtenerIpsUnicas(new List<string> { cajeroIp1.Text, cajeroIp2.Text, cajeroIp3.Text, cajeroIp5.Text }, IpsEjemplo);

        //    cajeroIp4.Text = resultadoIps;
        //}

        //protected void BtnAsignarIpCaj5_Click(object sender, EventArgs e)
        //{
        //    List<string> IpsEjemplo = new List<string>();

        //    IpsEjemplo.Add("255.255.255.255");
        //    IpsEjemplo.Add("192.158.1.38");
        //    IpsEjemplo.Add("192.158.1.29");
        //    IpsEjemplo.Add("192.158.2.38");
        //    IpsEjemplo.Add("192.158.5.20");
        //    IpsEjemplo.Add("192.158.1.25");

        //    string resultadoIps = ObtenerIpsUnicas(new List<string> { cajeroIp1.Text, cajeroIp2.Text, cajeroIp3.Text, cajeroIp4.Text }, IpsEjemplo);

        //    cajeroIp5.Text = resultadoIps;
        //}

        #endregion

        #region Consultas

        protected string InsertEnlaceCajero(string tipoEnlaceId, string instalacionId, string fechaActivacion, string ipLookback, string ipGateway, string ipMasc,
                                          string ipWan, string ipAlarma, string ipCamara, string ipTunel, string ipCajero1, string posCajero1, string ipCajero2,
                                          string posCajero2, string ipCajero3, string posCajero3, string ipCajero4, string posCajero4, string ipCajero5,
                                          string posCajero5, string nombreCajero)
        {
            StringBuilder qry = new StringBuilder();
            qry.AppendLine("INSERT INTO Keytia5.k5banorte.CajeroEnlace (");
            qry.AppendLine("CajeroTipoEnlaceID,");
            qry.AppendLine("CajeroInstalacionID,");
            qry.AppendLine("FechaActivacion,");
            qry.AppendLine("IPLookBack,");
            qry.AppendLine("IPGatewayLAN,");
            qry.AppendLine("IPMascara,");
            qry.AppendLine("IPWAN,");
            qry.AppendLine("IPAlarma,");
            qry.AppendLine("IPCamaraVideo,");
            qry.AppendLine("IPTunel,");
            qry.AppendLine("IPCajero01,");
            qry.AppendLine("CajeroPosicion01ID,");
            qry.AppendLine("IPCajero02,");
            qry.AppendLine("CajeroPosicion02ID,");
            qry.AppendLine("IPCajero03,");
            qry.AppendLine("CajeroPosicion03ID,");
            qry.AppendLine("IPCajero04,");
            qry.AppendLine("CajeroPosicion04ID,");
            qry.AppendLine("IPCajero05,");
            qry.AppendLine("CajeroPosicion05ID,");
            qry.AppendLine("EsTemporal,");
            qry.AppendLine("dtIniVigencia,");
            qry.AppendLine("dtFinVigencia,");
            qry.AppendLine("dtFecUltAct,");
            qry.AppendLine("NombreCajero,");
            qry.AppendLine("FechaBaja");
            qry.AppendLine(")");
            qry.AppendLine("VALUES");
            qry.AppendLine("(");
            qry.AppendLine("valor,").Replace("valor", tipoEnlaceId);
            qry.AppendLine("valor,").Replace("valor", instalacionId);
            qry.AppendLine("'valor',").Replace("valor", fechaActivacion);
            qry.AppendLine("valor,").Replace("valor", ipLookback);
            qry.AppendLine("valor,").Replace("valor", ipGateway);
            qry.AppendLine("valor,").Replace("valor", ipMasc);
            qry.AppendLine("valor,").Replace("valor", ipWan);
            qry.AppendLine("valor,").Replace("valor", ipAlarma);
            qry.AppendLine("valor,").Replace("valor", ipCamara);
            qry.AppendLine("valor,").Replace("valor", ipTunel);
            qry.AppendLine("valor,").Replace("valor", ipCajero1);
            qry.AppendLine("valor,").Replace("valor", posCajero1);
            qry.AppendLine("valor,").Replace("valor", ipCajero2);
            qry.AppendLine("valor,").Replace("valor", posCajero2);
            qry.AppendLine("valor,").Replace("valor", ipCajero3);
            qry.AppendLine("valor,").Replace("valor", posCajero3);
            qry.AppendLine("valor,").Replace("valor", ipCajero4);
            qry.AppendLine("valor,").Replace("valor", posCajero4);
            qry.AppendLine("valor,").Replace("valor", ipCajero5);
            qry.AppendLine("valor,").Replace("valor", posCajero5);
            qry.AppendLine("NULL,");
            qry.AppendLine("CONVERT(VARCHAR(10),GETDATE(),121),");
            qry.AppendLine("'2079-01-01',");
            qry.AppendLine("GETDATE(),");
            qry.AppendLine("'valor',").Replace("valor", nombreCajero);
            qry.AppendLine("NULL");
            qry.AppendLine(");");

            return qry.ToString();
        }

        protected string EditarEnlaceCajero(string cajeroId, string tipoEnlaceId, string instalacionId, string fechaActivacion, string ipLookback, string ipGateway, string ipMasc,
                                          string ipWan, string ipAlarma, string ipCamara, string ipTunel, string ipCajero1, string posCajero1, string ipCajero2,
                                          string posCajero2, string ipCajero3, string posCajero3, string ipCajero4, string posCajero4, string ipCajero5,
                                          string posCajero5, string nombreCajero)
        {
            StringBuilder qry = new StringBuilder();
            qry.AppendLine("UPDATE Keytia5.k5banorte.CajeroEnlace ");
            qry.AppendLine("SET ");
            qry.AppendLine("    CajeroTipoEnlaceID = valor,").Replace("valor", tipoEnlaceId);
            qry.AppendLine("    CajeroInstalacionID = valor,").Replace("valor", instalacionId);
            qry.AppendLine("    FechaActivacion = 'valor',").Replace("valor", fechaActivacion);
            qry.AppendLine("    IPLookBack = valor,").Replace("valor", ipLookback);
            qry.AppendLine("    IPGatewayLAN = valor,").Replace("valor", ipGateway);
            qry.AppendLine("    IPMascara = valor,").Replace("valor", ipMasc);
            qry.AppendLine("    IPWAN = valor,").Replace("valor", ipWan);
            qry.AppendLine("    IPAlarma = valor,").Replace("valor", ipAlarma);
            qry.AppendLine("    IPCamaraVideo = valor,").Replace("valor", ipCamara);
            qry.AppendLine("    IPTunel = valor,").Replace("valor", ipTunel);
            qry.AppendLine("    IPCajero01 = valor,").Replace("valor", ipCajero1);
            qry.AppendLine("    CajeroPosicion01ID = valor,").Replace("valor", posCajero1);
            qry.AppendLine("    IPCajero02 = valor,").Replace("valor", ipCajero2);
            qry.AppendLine("    CajeroPosicion02ID = valor,").Replace("valor", posCajero2);
            qry.AppendLine("    IPCajero03 = valor,").Replace("valor", ipCajero3);
            qry.AppendLine("    CajeroPosicion03ID = valor,").Replace("valor", posCajero3);
            qry.AppendLine("    IPCajero04 = valor,").Replace("valor", ipCajero4);
            qry.AppendLine("    CajeroPosicion04ID = valor,").Replace("valor", posCajero4);
            qry.AppendLine("    IPCajero05 = valor,").Replace("valor", ipCajero5);
            qry.AppendLine("    CajeroPosicion05ID = valor,").Replace("valor", posCajero5);
            qry.AppendLine("    EsTemporal = NULL,");
            qry.AppendLine("    dtIniVigencia = CONVERT(VARCHAR(10),GETDATE(),121),");
            qry.AppendLine("    dtFinVigencia = '2079-01-01',");
            qry.AppendLine("    dtFecUltAct = '2079-01-01',");
            qry.AppendLine("    NombreCajero = 'valor'").Replace("valor", nombreCajero);
            qry.AppendLine("WHERE CajeroInstalacionID = idCajero").Replace("idCajero", cajeroId);

            return qry.ToString();
        }

        protected string DeleteEnlaceCajero(string cajeroId)
        {
            StringBuilder qry = new StringBuilder();
            qry.AppendLine("UPDATE Keytia5.k5banorte.CajeroEnlace ");
            qry.AppendLine("SET FechaBaja = GETDATE() ");
            qry.AppendLine("WHERE CajeroInstalacionID = idCajero").Replace("idCajero", cajeroId);

            return qry.ToString();
        }

        protected DataTable BusquedaIdCajero(string id)
        {
            DataTable dtConsulta;
            StringBuilder qry = new StringBuilder();

            qry.Append("SELECT ");
            qry.Append("    NombreCajero, ");
            qry.Append("    CajeroInstalacionID, ");
            qry.Append("    CajeroTipoEnlaceID, ");
            qry.Append("    FechaActivacion, ");
            qry.Append("    IpLookback, ");
            qry.Append("    IPGatewayLAN, ");
            qry.Append("    IPMascara, ");
            qry.Append("    IPWan, ");
            qry.Append("    IPAlarma, ");
            qry.Append("    IPCamaraVideo, ");
            qry.Append("    IPTunel, ");
            qry.Append("    IPCajero01, ");
            qry.Append("    CajeroPosicion01ID, ");
            qry.Append("    IPCajero02, ");
            qry.Append("    CajeroPosicion02ID, ");
            qry.Append("    IPCajero03, ");
            qry.Append("    CajeroPosicion03ID, ");
            qry.Append("    IPCajero04, ");
            qry.Append("    CajeroPosicion04ID, ");
            qry.Append("    IPCajero05, ");
            qry.Append("    CajeroPosicion05ID ");
            qry.Append("FROM Keytia5.k5banorte.CajeroEnlace ");
            qry.Append("WHERE CajeroInstalacionID = 'idCajero' ").Replace("idCajero", id);

            dtConsulta = DSODataAccess.Execute(qry.ToString());

            return dtConsulta;
        }

        protected static string BusquedaCajeros()
        {
            StringBuilder qry = new StringBuilder();

            qry.Append("SELECT ");
            qry.Append("    NombreCajero, ");
            qry.Append("    CajeroInstalacionID, ");
            qry.Append("    CajeroTipoEnlaceID, ");
            qry.Append("    FechaActivacion, ");
            qry.Append("    IpLookback, ");
            qry.Append("    IPGatewayLAN, ");
            qry.Append("    IPMascara, ");
            qry.Append("    IPWan, ");
            qry.Append("    IPAlarma, ");
            qry.Append("    IPCamaraVideo, ");
            qry.Append("    IPTunel, ");
            qry.Append("    IPCajero01, ");
            qry.Append("    CajeroPosicion01ID, ");
            qry.Append("    IPCajero02, ");
            qry.Append("    CajeroPosicion02ID, ");
            qry.Append("    IPCajero03, ");
            qry.Append("    CajeroPosicion03ID, ");
            qry.Append("    IPCajero04, ");
            qry.Append("    CajeroPosicion04ID, ");
            qry.Append("    IPCajero05, ");
            qry.Append("    CajeroPosicion05ID ");
            qry.Append("FROM Keytia5.k5banorte.CajeroEnlace ");
            qry.Append("WHERE FechaBaja >= GETDATE() OR FechaBaja IS NULL ");

            return qry.ToString();
        }

        protected string ValidaCamposForm()
        {
            List<string> mensajeResultado = new List<string>();
            string resultado = "";

            if (string.IsNullOrEmpty(NombreDeCajero.Text))
            {
                mensajeResultado.Add("Nombre de cajero");
            }

            if (string.IsNullOrEmpty(IdDeCajero.Text))
            {
                mensajeResultado.Add("Id de Cajero");
            }

            if (string.IsNullOrEmpty(FechaActCajero.Text))
            {
                mensajeResultado.Add("Fecha de Cajero");
            }

            if (mensajeResultado.Count == 1)
            {
                resultado += mensajeResultado[0];
            }
            else
            {
                foreach (var cadena in mensajeResultado)
                {
                    if (mensajeResultado.IndexOf(cadena) == mensajeResultado.Count - 1)
                    {
                        resultado += cadena;
                    }
                    else
                    {
                        resultado += (cadena + ",");
                    }
                }
            }

            return resultado;
        }

        [WebMethod]
        public static List<SampleModel> GetInfo2()
        {
            DataTable dtConsulta;
            List<SampleModel> repo = new List<SampleModel>();

            dtConsulta = DSODataAccess.Execute(BusquedaCajeros());

            foreach (DataRow row in dtConsulta.Rows)
            {
                var fechaDt = row[3].ToString().Split(' ');

                repo.Add(new SampleModel()
                {
                    NombreCajero = row[0].ToString(),
                    IdCajero = row[1].ToString(),
                    TipoCajero = row[2].ToString(),
                    FechaActivacion = ObtieneFecha(fechaDt[0]),
                    CantidadCajeros = DeterminaCantidadCajeros(row).ToString(),
                    IpLookback = row[4].ToString(),
                    IpGateway = row[5].ToString(),
                    IpMasc = row[6].ToString(),
                    IpWan = row[7].ToString(),
                    IpAlarma = row[8].ToString(),
                    IpCamara = row[9].ToString(),
                    IpTunel = row[10].ToString()
                }
                );
            }
            return repo;
        }

        private string ConsultaDetalleDeUbicacion()
        {
            string datoBusqueda = Request.QueryString["qry"].Replace('+', ' ');

            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ");
            query.AppendLine("Nombre,");
            query.AppendLine("Domicilio,");
            query.AppendLine("Domicilio2 AS [Entre Calles],");
            query.AppendLine("Colonia,");
            query.AppendLine("Ciudad,");
            query.AppendLine("Municipio,");
            query.AppendLine("Estado,");
            query.AppendLine("CodigoPostal AS [Código Postal],");
            query.AppendLine("Latitud,");
            query.AppendLine("Longitud,");
            query.AppendLine("Teléfono,");
            query.AppendLine("Contacto,");
            query.AppendLine("ContactoRegional AS [Contacto Regional]");
            query.AppendLine("FROM Keytia5.k5banorte.CajeroInstalacion");
            query.AppendLine("WHERE Nombre = '" + datoBusqueda + "'");

            return query.ToString();
        }

        private string ObtenerRangoIp()
        {
            DataTable dt;

            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ");
            query.AppendLine("RangoIP");
            query.AppendLine("FROM Keytia5.k5banorte.RangoIpDeCajeros");
            query.AppendLine("WHERE FechaAsignacionRango IS NULL");

            dt = DSODataAccess.Execute(query.ToString());

            return dt.Rows[0][0].ToString();
        }

        private string UpdateTablaRangoIps(string ip, string ubicacion)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE Keytia5.k5banorte.RangoIpDeCajeros ");
            query.AppendLine("SET UbicacionAsignacion = '" + ubicacion + "', FechaAsignacionRango = GETDATE()");
            query.AppendLine("WHERE RangoIP = '" + ip + "'");

            return query.ToString();
        }

        private string UpdateTablaRangoIpsBaja(string ip)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE Keytia5.k5banorte.RangoIpDeCajeros ");
            query.AppendLine("SET UbicacionAsignacion = NULL, FechaAsignacionRango = NULL");
            query.AppendLine("WHERE RangoIP = '" + ip + "'");

            return query.ToString();
        }

        #endregion

        #region Modelos

        public class SampleModel
        {
            public string NombreCajero { get; set; }
            public string IdCajero { get; set; }
            public string TipoCajero { get; set; }
            public string FechaActivacion { get; set; }
            public string CantidadCajeros { get; set; }
            public string IpLookback { get; set; }
            public string IpGateway { get; set; }
            public string IpMasc { get; set; }
            public string IpWan { get; set; }
            public string IpAlarma { get; set; }
            public string IpCamara { get; set; }
            public string IpTunel { get; set; }
        }

        #endregion

        #region Metodos Auxiliares y Estilos CSS

        protected void OcultarCajeros()
        {
            btnAgregarCajero1div.Attributes.CssStyle.Add("display", "none");
            tipoCajero2div.Attributes.CssStyle.Add("display", "none");
            ipCajero2div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero2div.Attributes.CssStyle.Add("display", "none");

            btnAgregarCajero2div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero2div.Attributes.CssStyle.Add("display", "none");
            tipoCajero3div.Attributes.CssStyle.Add("display", "none");
            ipCajero3div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero3div.Attributes.CssStyle.Add("display", "none");

            btnAgregarCajero3div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero3div.Attributes.CssStyle.Add("display", "none");
            tipoCajero4div.Attributes.CssStyle.Add("display", "none");
            ipCajero4div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero4div.Attributes.CssStyle.Add("display", "none");

            btnAgregarCajero4div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero4div.Attributes.CssStyle.Add("display", "none");
            tipoCajero5div.Attributes.CssStyle.Add("display", "none");
            ipCajero5div.Attributes.CssStyle.Add("display", "none");
            ipLanCajero5div.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero5div.Attributes.CssStyle.Add("display", "none");
        }

        protected void MostrarCajeros()
        {
            // Ejecutado al dar click en un enlace para su muestreo en la informacion inferior

            btnAgregarCajero1div.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero1.Attributes.CssStyle.Add("display", "none");

            tipoCajero2div.Attributes.CssStyle.Add("display", "");
            ipCajero2div.Attributes.CssStyle.Add("display", "");
            ipLanCajero2div.Attributes.CssStyle.Add("display", "");

            btnAgregarCajero2div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero2div.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero2.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero2.Attributes.CssStyle.Add("display", "none");
            tipoCajero3div.Attributes.CssStyle.Add("display", "");
            ipCajero3div.Attributes.CssStyle.Add("display", "");
            ipLanCajero3div.Attributes.CssStyle.Add("display", "");

            btnAgregarCajero3div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero3div.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero3.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero3.Attributes.CssStyle.Add("display", "none");
            tipoCajero4div.Attributes.CssStyle.Add("display", "");
            ipCajero4div.Attributes.CssStyle.Add("display", "");
            ipLanCajero4div.Attributes.CssStyle.Add("display", "");

            btnAgregarCajero4div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero4div.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero4.Attributes.CssStyle.Add("display", "none");
            btnQuitarCajero4.Attributes.CssStyle.Add("display", "none");

            btnQuitarCajero5div.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero5.Attributes.CssStyle.Add("display", "none");
            tipoCajero5div.Attributes.CssStyle.Add("display", "");
            ipCajero5div.Attributes.CssStyle.Add("display", "");
            ipLanCajero5div.Attributes.CssStyle.Add("display", "");
        }

        protected void MostrarBotonesCajeros()
        {
            // Ejecutado al dar click en un enlace para su muestreo en la informacion inferior

            btnAgregarCajero1.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero2.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero2.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero3.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero3.Attributes.CssStyle.Add("display", "");
            btnAgregarCajero4.Attributes.CssStyle.Add("display", "");
            btnQuitarCajero4.Attributes.CssStyle.Add("display", "");

            btnQuitarCajero5.Attributes.CssStyle.Add("display", "");
        }

        #endregion
    }
}