using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepConsolidado
{
    public partial class ConfEnvioRepConsolidado : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string fechaInicio = "";
        static string fechaFinal = "";
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();

            if (!Page.IsPostBack)
            {
                if (esquema == "Laureate")
                {
                    DataTable dt;

                    string connection = Util.AppSettings("appConnectionString");

                    dt = DSODataAccess.Execute(QueryTodosLosSitios(), connection);

                    rowOrganizacion.Visible = true;
                    rowSitio.Visible = true;

                    listaOrganizacion.Items.Insert(0, new ListItem("UNITEC", "205470"));
                    listaOrganizacion.Items.Insert(0, new ListItem("UVM", "205476"));
                    listaOrganizacion.Items.Insert(0, new ListItem("SIN SELECCIONAR", ""));
                    listaOrganizacion.SelectedValue = "SIN SELECCIONAR";

                    listaSitios.Items.Insert(0, new ListItem("SIN SELECCIONAR", ""));
                    listaSitios.SelectedValue = "SIN SELECCIONAR";

                    foreach (DataRow row in dt.Rows)
                    {
                        listaSitios.Items.Insert(0, new ListItem(row[0].ToString(), row[1].ToString()));
                    }
                }
            }

            if(rbtnEnvioAut.Checked == true)
            {
                rowAdjuntarDetalle.Visible = false;
                rowFecFin.Visible = false;
            }
            else
            {
                rowAdjuntarDetalle.Visible = true;
                rowFecFin.Visible = true;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int tipoEn = (rbtnEnvioAut.Checked == true)?1:0;
            int adjuntaDetall = (chkMostrar.Checked == true) ? 1 : 0;

            string fechaIni = inicioFecha.Value;
            string fechaFinal = finalFecha.Value;
            if (rowFecFin.Visible == false) { fechaFinal = DateTime.Now.ToString("yyyy-MM-dd HH:mm"); }
            if(txtEmailDestinatario.Text != "")
            {
                if(fechaIni!= "" && fechaFinal!= "")
                {
                    string fechaHoy = DateTime.Now.ToString("yyy-MM-dd");
                    string fecEnvio = (rbtnEnvioAut.Checked == true) ? Convert.ToDateTime(inicioFecha.Value).ToString("yyyy-MM-dd") +" 08:00:00" : fechaHoy;
                    try
                    {
                        string connection = Util.AppSettings("appConnectionString");
                        string sp1 = "EXEC dbo.InsertaUsuariosReporteConsolidado @Shema ='" + esquema + "',@icodUsuario =" + iCodUsuario + "";
                        DSODataAccess.Execute(sp1, connection);

                        // Validaciones para insercion de registros Organizacion y Sitio
                        // para cliente Laureate

                        string sp;

                        if(esquema != "Laureate")
                        {
                            sp = "EXEC dbo.InsertaEnvioReporteConsolidadoConf @Schema ='" + esquema + "',@fecEnvio='" + fecEnvio + "',@Destinatario='" + txtEmailDestinatario.Text + "',@CorreoCopia='" + txtEmailCC.Text + "',@CorreoCopiaOculta ='" + txtEmailCCo.Text + "',@AsuntoEmail = '" + txtAsunto.Text + "',@FecIniConsulta = '" + fechaIni + "',@FecFinConsulta  = '" + fechaFinal + "',@CantDias = 0" + txtCantEnvios.Text + ",@TipoEnvio =" + tipoEn + ",@AdjuntaDetall=" + adjuntaDetall + "";
                        }
                        else
                        {
                            var valorSitio = listaSitios.SelectedValue == "" ? "0" : listaSitios.SelectedValue;
                            var valorOrganizacion = listaOrganizacion.SelectedValue == "" ? "0" : listaOrganizacion.SelectedValue;

                            sp = "EXEC dbo.InsertaEnvioReporteConsolidadoConf @Schema ='" + esquema + "',@fecEnvio='" + fecEnvio + "',@Destinatario='" + txtEmailDestinatario.Text + "',@CorreoCopia='" + txtEmailCC.Text + "',@CorreoCopiaOculta ='" + txtEmailCCo.Text + "',@AsuntoEmail = '" + txtAsunto.Text + "',@FecIniConsulta = '" + fechaIni + "',@FecFinConsulta  = '" + fechaFinal + "',@CantDias = 0" + txtCantEnvios.Text + ",@TipoEnvio =" + tipoEn + ",@AdjuntaDetall=" + adjuntaDetall + ", @Organizacion=" + valorOrganizacion + ", @Sitio=" + valorSitio + "";
                        }
                        
                        DSODataAccess.Execute(sp, connection);

                        txtAsunto.Text = "";
                        txtEmailCC.Text = "";
                        txtEmailCCo.Text = "";
                        txtEmailDestinatario.Text = "";
                        txtCantEnvios.Text = "";
                        inicioFecha.Value = "";
                        finalFecha.Value = "";

                        lblBodyModalMsn.Text = "El registro se dio de alta correctamente";
                        mpeEtqMsn.Show();
                    }
                    catch
                    {
                        lblTituloModalMsn.Text = "¡Error!";
                        lblBodyModalMsn.Text = "Ocurrio un error al dar de alta el registro";
                        mpeEtqMsn.Show();
                    }

                }
                else
                {
                    lblTituloModalMsn.Text = "¡Atención!";
                    lblBodyModalMsn.Text = "Debe de ingresar una Fecha";
                    mpeEtqMsn.Show();
                }

            }
            else
            {
                lblTituloModalMsn.Text = "¡Atención!";
                lblBodyModalMsn.Text = "Debe de ingresar un destinatario";
                mpeEtqMsn.Show();
            }

        }

        protected void listaOrganizacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connection = Util.AppSettings("appConnectionString");
            DataTable dt;

            listaSitios.Items.Clear();
            listaSitios.Items.Insert(0, new ListItem("SIN SELECCIONAR", ""));

            switch (listaOrganizacion.SelectedValue)
            {
                // UVM
                case "205476":
                    dt = DSODataAccess.Execute(QuerySitiosUvm(), connection);
                    break;
                // UNITEC
                case "205470":
                    dt = DSODataAccess.Execute(QuerySitiosUnitec(), connection);
                    break;
                default:
                    dt = DSODataAccess.Execute(QueryTodosLosSitios(), connection);
                    break;
            }

            foreach (DataRow row in dt.Rows)
            {
                listaSitios.Items.Insert(0, new ListItem(row[0].ToString(), row[1].ToString()));
            }

            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "insertaJsPostback", "insertaJsPostback();", true);
        }

        protected string QueryTodosLosSitios()
        {
            StringBuilder qryTodosSitios = new StringBuilder();
            qryTodosSitios.AppendLine("SELECT SitioDesc, Sitio ");
            qryTodosSitios.AppendLine("FROM Laureate.[VisHistoricos('Restricciones','Sitios','Español')] ");
            qryTodosSitios.AppendLine("WHERE SitioDesc != 'Ext fuera de rango'");

            return qryTodosSitios.ToString();
        }

        protected string QuerySitiosUnitec()
        {
            StringBuilder qrySitiosUnitec = new StringBuilder();
            qrySitiosUnitec.AppendLine("SELECT SitioDesc, Sitio ");
            qrySitiosUnitec.AppendLine("FROM Laureate.[VisHistoricos('Restricciones','Sitios','Español')] ");
            qrySitiosUnitec.AppendLine("WHERE Usuar = 205470 AND SitioDesc != 'Ext fuera de rango'");

            return qrySitiosUnitec.ToString();
        }

        protected string QuerySitiosUvm()
        {
            StringBuilder qrySitiosUvm = new StringBuilder();
            qrySitiosUvm.AppendLine("SELECT SitioDesc, Sitio ");
            qrySitiosUvm.AppendLine("FROM Laureate.[VisHistoricos('Restricciones','Sitios','Español')] ");
            qrySitiosUvm.AppendLine("WHERE Usuar = 205476 AND SitioDesc != 'Ext fuera de rango'");

            return qrySitiosUvm.ToString();
        }
    }
}