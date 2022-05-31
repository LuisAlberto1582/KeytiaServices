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
using System.Globalization;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class WorkflowAutorizarBajaExtenAbiertas : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();
        string iCodCatPeriodo = string.Empty;
        List<ConfigCliente> configCli = new List<ConfigCliente>();

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

                LeeQueryString();
                configCli = GetConfigEsquema();

                if (!Page.IsPostBack)
                {
                    ReporteMovimientosPendientes();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path
                      + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void LeeQueryString()
        {
            #region Revisar si el querystring iCodCatPeriodo contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["p"]))
            {
                try
                {
                    iCodCatPeriodo = Util.Decrypt(Request.QueryString["p"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (Nav) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                iCodCatPeriodo = string.Empty;
            }

            #endregion Revisar si el iCodCatPeriodo Nav contiene un valor
        }

        private void ReporteMovimientosPendientes()
        {
            if (configCli.Count > 0)
            {
                DateTime fechIniP = configCli.First().FechaInicioPeriodoExten;
                DateTime fechFinP = configCli.First().FechaFinPeriodoExten;
                var dtResult = DSODataAccess.Execute(GetExtensionesBaja());
                if (dtResult.Rows.Count > 0)
                {
                    var fechaMax = Convert.ToDateTime(dtResult.Rows[0]["FechaMaxEdit"]);
                    if (DateTime.Now >= fechaMax)
                    {
                        gridExtenAbiertas.Enabled = false;
                        btnGuardar.Visible = false;
                    }
                    else
                    {
                        gridExtenAbiertas.Enabled = true;
                        btnGuardar.Enabled = true;
                    }
                }
                gridExtenAbiertas.DataSource = dtResult;
                gridExtenAbiertas.DataBind();
                if (configCli.First().IdPeriodoExtenActivo > 0)
                {
                    lblInicio.Text = "Extensiones abiertas del periodo: " + fechIniP.Day.ToString() + " de " + MonthName(fechIniP.Month).ToUpper() +
                                " del " + fechIniP.Year.ToString() + " al " + fechFinP.Day.ToString() + " de " + MonthName(fechFinP.Month).ToUpper() +
                                " del " + fechFinP.Year.ToString();
                }
            }
        }

        #region Consulta

        public List<ConfigCliente> GetConfigEsquema()
        {
            query.Length = 0;
            query.AppendLine("SELECT");
            query.AppendLine("   EsJefeResponsableCenCos					= (ISNULL(BanderasConfigProcesoWorkflow,0) & 2)/2,");
            query.AppendLine("   EsActivarProcesoBajaExtenAbierta			= (ISNULL(BanderasConfigProcesoWorkflow,0) & 128)/128,");
            query.AppendLine("   ConfigPeriodo.iCodCatalogo AS iCodCatPeriodoExtenActivo,");
            query.AppendLine("   ConfigPeriodo.FechaInicioPeriodo AS FechaInicioPeriodoExten,");
            query.AppendLine("   ConfigPeriodo.FechaFinPeriodo AS FechaFinPeriodoExten ");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')] Cliente, ");
            query.AppendLine("	" + DSODataContext.Schema + ".[VisHistoricos('ConfigProcesoWorkflow','Configuracion Proceso Workflow','Español')] Config");
            query.AppendLine("	LEFT JOIN (");
            query.AppendLine("			SELECT TOP(1) *");
            query.AppendLine("			FROM " + DSODataContext.Schema + ".[VisHistoricos('WorkflowPeriodoExten','Workflow Periodos Extensiones','Español')]");
            query.AppendLine("			WHERE GETDATE() BETWEEN FechaInicioPeriodo AND FechaFinPeriodo	--PARA SABER CUAL ES EL PERIODO VIGENTE");
            query.AppendLine("				AND dtIniVigencia <> dtFinVigencia");
            query.AppendLine("				AND dtFinVigencia >= GETDATE()");
            query.AppendLine("			ORDER BY FechaInicioPeriodo");
            query.AppendLine("		 ) AS ConfigPeriodo");
            query.AppendLine("		ON GETDATE() BETWEEN ConfigPeriodo.FechaInicioPeriodo AND ConfigPeriodo.FechaFinPeriodo");
            query.AppendLine("WHERE Cliente.dtIniVigencia <> Cliente.dtFinVigencia");
            query.AppendLine("	AND Cliente.dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND Config.dtIniVigencia <> Config.dtFinVigencia");
            query.AppendLine("	AND Config.dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND (ISNULL(BanderasCliente,0) & 131072)/131072=1  -- Bandera que indica si esta encendida la bandera de Habilitar Workflow");

            return VaciarDatosConfig(DSODataAccess.Execute(query.ToString()));
        }

        private List<ConfigCliente> VaciarDatosConfig(DataTable dt)
        {
            List<ConfigCliente> clientes = new List<ConfigCliente>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    #region Vaciar info
                    ConfigCliente cliente = new ConfigCliente();
                    cliente.EsJefeResponsableCenCos = Convert.ToInt32(row["EsJefeResponsableCenCos"]);
                    cliente.EsActivarProcesoBajaExtenAbierta = Convert.ToInt32(row["EsActivarProcesoBajaExtenAbierta"]);
                    cliente.IdPeriodoExtenActivo = row["iCodCatPeriodoExtenActivo"] != DBNull.Value ? Convert.ToInt32(row["iCodCatPeriodoExtenActivo"]) : 0;
                    cliente.FechaInicioPeriodoExten = row["FechaInicioPeriodoExten"] != DBNull.Value ? Convert.ToDateTime(row["FechaInicioPeriodoExten"]) : DateTime.MinValue;
                    cliente.FechaFinPeriodoExten = row["FechaFinPeriodoExten"] != DBNull.Value ? Convert.ToDateTime(row["FechaFinPeriodoExten"]) : DateTime.MinValue;
                    clientes.Add(cliente);
                    #endregion
                }
            }
            if (!string.IsNullOrEmpty(iCodCatPeriodo) && iCodCatPeriodo != "0" && clientes.Count > 0 &&
                iCodCatPeriodo != clientes.First().IdPeriodoExtenActivo.ToString())
            {
                InfoPeriodo();
            }

            return clientes;
        }

        private void InfoPeriodo()
        {
            query.Length = 0;
            query.AppendLine("SELECT iCodCatalogo, FechaInicioPeriodo, FechaFinPeriodo");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('WorkflowPeriodoExten','Workflow Periodos Extensiones','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine("AND dtFinVigencia >= GETDATE()");
            query.AppendLine("AND iCodCatalogo = " + iCodCatPeriodo);

            var dtResult = DSODataAccess.Execute(query.ToString());
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                configCli.First().IdPeriodoExtenActivo = dtResult.Rows[0]["iCodCatalogo"] != DBNull.Value ? Convert.ToInt32(dtResult.Rows[0]["iCodCatalogo"]) : 0;
                configCli.First().FechaInicioPeriodoExten = dtResult.Rows[0]["FechaInicioPeriodo"] != DBNull.Value ? Convert.ToDateTime(dtResult.Rows[0]["FechaInicioPeriodo"]) : DateTime.MinValue;
                configCli.First().FechaFinPeriodoExten = dtResult.Rows[0]["FechaFinPeriodo"] != DBNull.Value ? Convert.ToDateTime(dtResult.Rows[0]["FechaFinPeriodo"]) : DateTime.MinValue;
            }
        }

        private string GetExtensionesBaja()
        {
            query.Length = 0;

            query.AppendLine("EXEC [WorkflowV2GetExtenParaBajaMasiva]");
            query.AppendLine("  @Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@iCodPeriodoWorkflow = " + configCli.First().IdPeriodoExtenActivo + ",");
            query.AppendLine("	@JefeIsResposCenCos = " + configCli.First().EsJefeResponsableCenCos + ",");
            query.AppendLine("	@iCodUsuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@iCodPerfil = " + Session["iCodPerfil"]);

            return query.ToString();
        }

        private void UpdateRegistros(List<int> iCodReg, bool isAutorizado)
        {
            if (iCodReg.Count > 0)
            {
                query.Length = 0;
                for (int i = 0; i < iCodReg.Count; i++)
                {
                    query.Append(iCodReg[i] + ",");
                }
                query.Remove(query.Length - 1, 1);
                var iCods = query.ToString();

                //Valor de bandera 1 indica que el registro fue autorizado, y el valor de bandera 2 indica que fue rechazado.
                string valorBandera = isAutorizado ? "1" : "2";
                query.Length = 0;
                query.AppendLine("UPDATE " + DSODataContext.Schema + ".[VisPendientes('Detall','Workflow Bitacora Baja Exten Abiertas','Español')]");
                query.AppendLine("SET WorkflowBanderasBitacoBajaExtenAbiertas = ISNULL(WorkflowBanderasBitacoBajaExtenAbiertas,0) + " + valorBandera + ",");
                query.AppendLine("  Usuar = " + Session["iCodUsuario"] + ",");
                query.AppendLine("	dtFecUltAct = GETDATE()");
                query.AppendLine("WHERE WorkflowPeriodoExten = " + configCli.First().IdPeriodoExtenActivo + "");
                query.AppendLine("	AND FechaMaxCambios >= GETDATE()"); //Tiene que ser un periodo vigente. Los anteriores ya no se pueden mover.
                query.AppendLine("	AND (ISNULL(WorkflowBanderasBitacoBajaExtenAbiertas,0) & " + valorBandera + ") / " + valorBandera + " = 0");
                query.AppendLine("	AND iCodRegistro IN (" + iCods + ")");

                DSODataAccess.ExecuteNonQuery(query.ToString());
            }
        }

        #endregion Consulta

        public string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> iCodRegAutorizados = new List<int>();
                List<int> iCodRegRechazados = new List<int>();

                ObtenerValoresAActualizar(ref iCodRegAutorizados, ref iCodRegRechazados);
                if (iCodRegAutorizados.Count == 0 && iCodRegRechazados.Count == 0)
                {
                    throw new ArgumentException("No hay cambios pendientes por guardar.");
                }

                UpdateRegistros(iCodRegAutorizados, true);
                UpdateRegistros(iCodRegRechazados, false);

                Response.Redirect(Request.RawUrl);
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

        private void ObtenerValoresAActualizar(ref List<int> iCodRegAutorizados, ref List<int> iCodRegRechazados)
        {
            for (int i = 0; i < gridExtenAbiertas.Rows.Count; i++)
            {
                if (Convert.ToInt32(gridExtenAbiertas.DataKeys[i]["IsEnable"]) == 1)
                {
                    RadioButton rbAutorizar = (gridExtenAbiertas.Rows[i].FindControl("rdbAutorizar")) as RadioButton;
                    RadioButton rbRechazar = (gridExtenAbiertas.Rows[i].FindControl("rdbRechazar")) as RadioButton;
                    if (rbAutorizar.Checked)
                    {
                        iCodRegAutorizados.Add(Convert.ToInt32(gridExtenAbiertas.DataKeys[i]["iCodRegistro"]));
                    }
                    else if (rbRechazar.Checked)
                    {
                        iCodRegRechazados.Add(Convert.ToInt32(gridExtenAbiertas.DataKeys[i]["iCodRegistro"]));
                    }
                }
            }
        }
    }

    public class ConfigCliente
    {
        public int EsJefeResponsableCenCos { get; set; }
        public int EsActivarProcesoBajaExtenAbierta { get; set; }
        public int IdPeriodoExtenActivo { get; set; }
        public DateTime FechaInicioPeriodoExten { get; set; }
        public DateTime FechaFinPeriodoExten { get; set; }

    }
}
