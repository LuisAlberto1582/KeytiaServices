using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using System.Data;
using KeytiaServiceBL;
using System.Text;
using System.Collections;
using KeytiaWeb.UserInterface.DashboardLT;
using KeytiaServiceBL.Reportes;
using System.Web.Services;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class Detallado : System.Web.UI.Page
    {

        //20190808 RM Se crea diccionario para guardar parametros  de fija y movil
        static Dictionary<string, string> param = new Dictionary<string, string>();
        static bool SoloLlamadasFueraDeHorario = false;

        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        #region Variables que guardan valores de queryString

        static DataTable RepDetallado = new DataTable();
        static DataTable RepDetalleFacturaMovil = new DataTable();
        static int maxRegistrosEnWeb = 1000; //Limite de registros que se pientaran en web
        StringBuilder consultaCbo = new StringBuilder();
        static int maxRegistrosEnExcel = 1000;
        string nomResumenCDR = string.Empty;
        static string tablaAUsuar = string.Empty;

        bool isReporteDetalleCDR = true;
        bool isReporteDetalleFactura = false;
        bool isReporteDetalleCDREntrada = false;
        bool isReporteDetalleCDREnlace = false;

        #endregion



        //Laureate
      
        static string organizacion = "0";
        static string direccionLlamada = "0";

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
                (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();

                Rep0.Visible = false;
                Rep9.Visible = false;

                if (!Page.IsPostBack)
                {
                    if (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")
                    {

                        #region Inicia los valores default de los controles de fecha y banderas de clientes y perfiles
                        try
                        {
                            CalculaFechasDeDashboard();
                            EstablecerBanderasClientePerfil();
                        }
                        catch (Exception ex)
                        {
                            throw new KeytiaWebException(
                                "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                                + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                        }
                        #endregion Inicia los valores default de los controles de fecha
                    }

                }


                //FD 18/03/2021 SOLO SE MUESTRAN LOS PANELES PARA EL ESQUEMA LAUREATE
                if (DSODataContext.Schema.ToLower() == "laureate")
                {
                 
                    rowdirLlamada.Visible = true;
                    rowOrganizacion.Visible = true;
                    rowLlamadasFueraDeHorario.Visible = true;
                   
                }

                LlenarDropDownList();

                LeeQueryString();
                AplicarOpcionTelefonia();

                ValidarTipoReporte();
                ValidarTelefoniasAMostrar();

                Rep0.Visible = true;
                pnlMapaNav.Visible = false;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path
                      + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        #region Configuracion pagina

        private void CalculaFechasDeDashboard()
        {
            #region Calcula Fechas
            if (Session["FechaInicio"] != null || Session["FechaFin"] != null)
            {
                fechaInicio = Convert.ToDateTime(Session["FechaInicio"].ToString());
                fechaFinal = Convert.ToDateTime(Session["FechaFin"].ToString());
            }
            else if (Session["FechaInicio"] == null || Session["FechaFin"] != null)
            {
                DataTable fechaMaxCDR = DSODataAccess.Execute(ConsultaFechaMaximaCDR());
                // Se valida que el datatable contenga resultados.
                if (fechaMaxCDR.Rows.Count > 0)
                {
                    int Anio;
                    int Mes;
                    int Dia;
                    // Se valida que los resultados puedan ser enteros para poder formar la fecha.
                    if (
                            int.TryParse(fechaMaxCDR.Rows[0]["Anio"].ToString(), out Anio) &&
                            int.TryParse(fechaMaxCDR.Rows[0]["Mes"].ToString(), out Mes) &&
                            int.TryParse(fechaMaxCDR.Rows[0]["Dia"].ToString(), out Dia)
                        )
                    {
                        // Se forman las fechas de inicio y fin
                        fechaInicio = new DateTime(Anio, Mes, 1);
                        fechaFinal = new DateTime(Anio, Mes, Dia);

                        // Si el dia de la fecha fin es uno, se calculan las fechas inicio y fin del mes anterior.
                        if (Dia == 1)
                        {
                            fechaInicio = fechaInicio.AddMonths(-1);
                            fechaFinal = fechaFinal.AddDays(-1);
                        }
                    }
                }
                // Si en CDR no hay informacion entonces los valores de las fechas se calculan con los
                // valores default de las variables fechaInicio y fechaFinal
                else
                {
                    if (fechaFinal.Day == 1)
                    {
                        fechaInicio = fechaInicio.AddMonths(-1);
                        fechaFinal = fechaFinal.AddDays(-1);
                    }
                }
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
            #endregion Calcula Fechas
        }

        private void EstablecerBanderasClientePerfil()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("SELECT BanderasCliente");
            consulta.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')]");
            consulta.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            consulta.AppendLine("AND UsuarDB = " + Session["iCodUsuarioDB"].ToString());
            consulta.AppendLine("AND (ISNULL(BanderasCliente,0) & 1024)/1024=1 ");

            DataTable dtConsulta = DSODataAccess.Execute(consulta.ToString());

            Session["MuestraSM"] = (dtConsulta.Rows.Count > 0) ? 1 : 0;

            consulta.Length = 0;
            consulta.AppendLine("SELECT BanderasPerfil");
            consulta.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')]");
            consulta.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            consulta.AppendLine("AND iCodCatalogo = " + Session["iCodPerfil"].ToString());
            consulta.AppendLine("AND (ISNULL(BanderasPerfil,0) & 2)/2=1 ");

            DataTable dtConsulta2 = DSODataAccess.Execute(consulta.ToString());

            Session["MuestraCostoSimulado"] = (dtConsulta2.Rows.Count > 0) ? 1 : 0;

            //Tablas independientes para DetalleCDR de Ent y Enl
            consulta.Length = 0;
            consulta.AppendLine("SELECT BanderasCliente");
            consulta.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')]");
            consulta.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            consulta.AppendLine("AND UsuarDB = " + Session["iCodUsuarioDB"].ToString());
            consulta.AppendLine("AND (ISNULL(BanderasCliente,0) & 65536)/65536=1 ");

            DataTable dtConsulta3 = DSODataAccess.Execute(consulta.ToString());

            Session["InfoCDRTablasIndependientes"] = (dtConsulta3.Rows.Count > 0) ? 1 : 0;
        }

        protected void rbtnFija_OnCheckedChanged(Object sender, EventArgs args)
        {
            ValidarTipoReporte();
            ValidarTelefoniasAMostrar();
            LlenarDropDownList();
            OcultrarMostrarControles();
        }

        private void ValidarTipoReporte()
        {
            isReporteDetalleCDR = false;
            isReporteDetalleFactura = false;
            isReporteDetalleCDREntrada = false;
            isReporteDetalleCDREnlace = false;

            if (rbtnFija.Checked) // Detallado de Fija. Toda la informacion se cuentra en una sola tabla.
            {
                isReporteDetalleCDR = true;
                tablaAUsuar = "CDR";
            }
            else if (rbtnMovil.Checked) //Detallado de Moviles
            {
                isReporteDetalleFactura = true;
                tablaAUsuar = string.Empty;
            }
            else if (rbtnFijaEntrada.Checked)   //Solo llamadas de entrada (Se guardan en otra tabla en base de datos)
            {
                isReporteDetalleCDREntrada = true;
                tablaAUsuar = "Ent";
            }
            else if (rbtnFijaEnlace.Checked)   //Solo llamadas de Enlace (Se guardan en otra tabla en base de datos)
            {
                isReporteDetalleCDREnlace = true;
                tablaAUsuar = "Enl";
            }
            else  //Si no hay alguno seleccionado se considerara Fija
            {
                isReporteDetalleCDR = true;
                tablaAUsuar = "CDR";
            }
        }

        private void ValidarTelefoniasAMostrar()
        {
            if (Convert.ToInt32(Session["InfoCDRTablasIndependientes"]) == 0)
            {
                rbtnFija.Text = "Fija";
                rbtnFijaEntrada.Visible = false;
                rbtnFijaEnlace.Visible = false;
            }
            else
            {
                rbtnFija.Text = "Llams. Salida (Fija)";
                rbtnFijaEntrada.Visible = true;
                rbtnFijaEnlace.Visible = true;
            }
        }

        private void OcultrarMostrarControles()
        {
            if (isReporteDetalleFactura) // Móvil
            {
                rowLocali.Visible = false;
                rowNumMarcado.Visible = false;
                rowExten.Visible = false;
                rowCodAuto.Visible = false;
                rowLinea.Visible = true;
                rowCatLlam.Visible = false;
                rowTDest.Visible = true;
            }
            else if (isReporteDetalleCDREntrada || isReporteDetalleCDREnlace)
            {
                rowLocali.Visible = true;
                rowNumMarcado.Visible = true;
                rowExten.Visible = true;
                rowCodAuto.Visible = true;
                rowLinea.Visible = false;
                rowCatLlam.Visible = false;
                rowTDest.Visible = false;
            }
            else  //Fija
            {
                rowLocali.Visible = true;
                rowNumMarcado.Visible = true;
                rowExten.Visible = true;
                rowCodAuto.Visible = true;
                rowLinea.Visible = false;
                rowCatLlam.Visible = true;
                rowTDest.Visible = true;
            }
        }

        private void LlenarDropDownList()
        {
            if (isReporteDetalleFactura)
            {
                LlenarDropDownListFiltrosFactura();
            }
            else //Todas las opciones de tenefonía fija ocupan los mismos combos.
            {
                LlenarDropDownListFiltrosCDR();
            }
        }

        private void LlenarDropDownListFiltrosCDR() //Fija en General
        {
            DataTable ubicacion = DSODataAccess.Execute(ConsultaObtenerSitiosCDR());
            ubicacion = DTIChartsAndControls.ordenaTabla(ubicacion, "Sitio ASC");
            DataRow rowUbicacion = ubicacion.NewRow();
            rowUbicacion["iCodCatalogo"] = 0;
            rowUbicacion["Sitio"] = "--TODOS--";

            ubicacion.Rows.InsertAt(rowUbicacion, 0);
            cboUbicacion.DataSource = ubicacion.DefaultView;
            cboUbicacion.DataValueField = "iCodCatalogo";
            cboUbicacion.DataTextField = "Sitio";
            cboUbicacion.DataBind();

            DataTable tipoLlamada = DSODataAccess.Execute(ConsultaObtenerTipoLlamada());
            tipoLlamada = DTIChartsAndControls.ordenaTabla(tipoLlamada, "TipoLlamada ASC");
            DataRow rowTipoLlamada = tipoLlamada.NewRow();
            rowTipoLlamada["GEtiqueta"] = -1;
            rowTipoLlamada["TipoLlamada"] = "--TODAS--";

            tipoLlamada.Rows.InsertAt(rowTipoLlamada, 0);
            cboTipoLlamada.DataSource = tipoLlamada.DefaultView;
            cboTipoLlamada.DataValueField = "GEtiqueta";
            cboTipoLlamada.DataTextField = "TipoLlamada";
            cboTipoLlamada.DataBind();

            DataTable carrier = DSODataAccess.Execute(ConsultaObtenerCarrierCDR());
            carrier = DTIChartsAndControls.ordenaTabla(carrier, "Carrier ASC");
            DataRow rowCarrier = carrier.NewRow();
            rowCarrier["iCodCatalogo"] = 0;
            rowCarrier["Carrier"] = "--TODOS--";

            carrier.Rows.InsertAt(rowCarrier, 0);
            cboCarrier.DataSource = carrier.DefaultView;
            cboCarrier.DataValueField = "iCodCatalogo";
            cboCarrier.DataTextField = "Carrier";
            cboCarrier.DataBind();

            DataTable tipoDestino = DSODataAccess.Execute(ConsultaObtenerTipoDestinoCDR());
            tipoDestino = DTIChartsAndControls.ordenaTabla(tipoDestino, "TipoDestino ASC");
            DataRow rowTipoDestino = tipoDestino.NewRow();
            rowTipoDestino["iCodCatalogo"] = 0;
            rowTipoDestino["TipoDestino"] = "--TODOS--";

            tipoDestino.Rows.InsertAt(rowTipoDestino, 0);
            cboTipoDestino.DataSource = tipoDestino.DefaultView;
            cboTipoDestino.DataValueField = "iCodCatalogo";
            cboTipoDestino.DataTextField = "TipoDestino";
            cboTipoDestino.DataBind();
        }

        private void LlenarDropDownListFiltrosFactura()
        {
            DataTable ubicacion = DSODataAccess.Execute(ConsultaObtenerSitiosMovil());
            ubicacion = DTIChartsAndControls.ordenaTabla(ubicacion, "Sitio ASC");
            DataRow rowUbicacion = ubicacion.NewRow();
            rowUbicacion["iCodCatalogo"] = 0;
            rowUbicacion["Sitio"] = "--TODOS--";

            ubicacion.Rows.InsertAt(rowUbicacion, 0);
            cboUbicacion.DataSource = ubicacion.DefaultView;
            cboUbicacion.DataValueField = "iCodCatalogo";
            cboUbicacion.DataTextField = "Sitio";
            cboUbicacion.DataBind();

            DataTable tipoLlamada = DSODataAccess.Execute(ConsultaObtenerTipoLlamada());
            tipoLlamada = DTIChartsAndControls.ordenaTabla(tipoLlamada, "TipoLlamada ASC");
            DataRow rowTipoLlamada = tipoLlamada.NewRow();
            rowTipoLlamada["GEtiqueta"] = -1;
            rowTipoLlamada["TipoLlamada"] = "--TODAS--";

            tipoLlamada.Rows.InsertAt(rowTipoLlamada, 0);
            cboTipoLlamada.DataSource = tipoLlamada.DefaultView;
            cboTipoLlamada.DataValueField = "GEtiqueta";
            cboTipoLlamada.DataTextField = "TipoLlamada";
            cboTipoLlamada.DataBind();

            DataTable carrier = DSODataAccess.Execute(ConsultaObtenerCarrierMovil());
            carrier = DTIChartsAndControls.ordenaTabla(carrier, "Carrier ASC");
            DataRow rowCarrier = carrier.NewRow();
            rowCarrier["iCodCatalogo"] = 0;
            rowCarrier["Carrier"] = "--TODOS--";

            carrier.Rows.InsertAt(rowCarrier, 0);
            cboCarrier.DataSource = carrier.DefaultView;
            cboCarrier.DataValueField = "iCodCatalogo";
            cboCarrier.DataTextField = "Carrier";
            cboCarrier.DataBind();

            DataTable tipoDestino = DSODataAccess.Execute(ConsultaObtenerTipoDestinoMovil());
            tipoDestino = DTIChartsAndControls.ordenaTabla(tipoDestino, "TipoDestino ASC");
            DataRow rowTipoDestino = tipoDestino.NewRow();
            rowTipoDestino["iCodCatalogo"] = 0;
            rowTipoDestino["TipoDestino"] = "--TODOS--";

            tipoDestino.Rows.InsertAt(rowTipoDestino, 0);
            cboTipoDestino.DataSource = tipoDestino.DefaultView;
            cboTipoDestino.DataValueField = "iCodCatalogo";
            cboTipoDestino.DataTextField = "TipoDestino";
            cboTipoDestino.DataBind();
        }

        private void LeeQueryString()
        {
            try
            {
                //Se inicializan todos los posibles parametros
                param.Clear();
                param.Add("opcTel", string.Empty);

                for (int i = 0; i < param.Count; i++)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString[param.Keys.ElementAt(i)]))
                        {
                            param[param.Keys.ElementAt(i)] = Request.QueryString[param.Keys.ElementAt(i)];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al leer el valor del querystring " + param.Keys.ElementAt(i) + " en " + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AplicarOpcionTelefonia()
        {
            try
            {
                //Busca la opcion de telefonia en el diccionario del QueryString

                string opcTel = string.Empty;

                if (param["opcTel"] != null && param["opcTel"].Length > 0)
                {
                    string opcion = param["opcTel"].ToString().ToLower();
                    opcTel = (opcion == "fija" || opcion == "movil") ? opcion : "fija";
                }
                else
                {
                    opcTel = "fija";
                }


                //Aplica la opcion de telefonia en el radiodutton en Front
                if (opcTel.Length > 0)
                {


                    if (opcTel == "fija")
                    {
                        rbtnFija.Checked = true;
                        rbtnMovil.Checked = false;
                        rbtnFija_OnCheckedChanged(rbtnFija, EventArgs.Empty);
                    }
                    else
                    {
                        rbtnFija.Checked = false;
                        rbtnMovil.Checked = true;
                        rbtnFija_OnCheckedChanged(rbtnMovil, EventArgs.Empty);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion Configuracion pagina

        #region Consultas a BD

        #region DetalleCDR

        private string ConsultaFechaMaximaCDR()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("");
            lsb.AppendLine(" select");
            lsb.AppendLine(" Datepart(year, Max(FechaInicio)) as Anio,");
            lsb.AppendLine(" Datepart(month, Max(FechaInicio)) as Mes,");
            lsb.AppendLine(" Datepart(day, Max(FechaInicio)) as Dia");
            lsb.AppendLine(" from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')] ");
            return lsb.ToString();
        }

        private string ConsultaObtenerSitiosCDR()
        {
            consultaCbo.Length = 0;
            consultaCbo.AppendLine("DECLARE @Perfil INT = " + Session["iCodPerfil"].ToString());
            consultaCbo.AppendLine("DECLARE @Usuario INT = " + Session["iCodUsuario"].ToString());
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("IF(@Perfil <> 367)"); //Perfil configurador.
            consultaCbo.AppendLine("BEGIN");
            consultaCbo.AppendLine("	CREATE TABLE #RestSitio ( iCodCatalogo INT )");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	INSERT INTO #RestSitio(iCodCatalogo)");
            consultaCbo.AppendLine("	SELECT Sitio FROM " + DSODataContext.Schema + ".RestSitio");
            consultaCbo.AppendLine("	WHERE FechaInicio <> FechaFin AND FechaFin >= GETDATE()");
            consultaCbo.AppendLine("		AND Usuar = @Usuario");
            consultaCbo.AppendLine("		AND Perfil = @Perfil");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	SELECT Sitio.iCodCatalogo, Sitio=UPPER(Sitio.vchDescripcion)");
            consultaCbo.AppendLine("	FROM " + DSODataContext.Schema + ".[VisHisComun('Sitio','Español')] Sitio");
            consultaCbo.AppendLine("		JOIN #RestSitio RestSitio");
            consultaCbo.AppendLine("		ON Sitio.iCodCatalogo = RestSitio.iCodCatalogo");
            consultaCbo.AppendLine("	WHERE Sitio.dtIniVigencia <> Sitio.dtFinVigencia");
            consultaCbo.AppendLine("		AND Sitio.dtFinVigencia >= GETDATE()");
            consultaCbo.AppendLine("END");
            consultaCbo.AppendLine("ELSE");
            consultaCbo.AppendLine("BEGIN ");
            consultaCbo.AppendLine("	SELECT Sitio.iCodCatalogo, Sitio=UPPER(Sitio.vchDescripcion)");
            consultaCbo.AppendLine("	FROM " + DSODataContext.Schema + ".[VisHisComun('Sitio','Español')] Sitio");
            consultaCbo.AppendLine("	WHERE Sitio.dtIniVigencia <> Sitio.dtFinVigencia");
            //RM 20190208
            consultaCbo.AppendLine("    And Sitio.dtiniVigencia <='" + Session["FechaInicio"].ToString() + " 00:00:00'");
            consultaCbo.AppendLine("    And Sitio.dtFinVigencia >= '" + Session["FechaFin"].ToString() + " 23:59:59'");
            consultaCbo.AppendLine("	--AND Sitio.dtFinVigencia >= GETDATE()");
            consultaCbo.AppendLine("END");

            return consultaCbo.ToString();
        }

        private string ConsultaObtenerTipoLlamada()
        {
            consultaCbo.Length = 0;
            consultaCbo.AppendLine("SELECT GEtiqueta, TipoLlamada=UPPER(vchDescripcion)");
            consultaCbo.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            consultaCbo.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            return consultaCbo.ToString();
        }

        private string ConsultaObtenerCarrierCDR()
        {
            if (DSODataContext.Schema.ToLower() == "k5banorte")
                nomResumenCDR = "ResumenCDR";
            else
                nomResumenCDR = "[VisAcumulados('AcumDia','ResumenCDR','Español')]";

            consultaCbo.Length = 0;
            consultaCbo.AppendLine("SELECT iCodCatalogo, Carrier=UPPER(vchDescripcion)");
            consultaCbo.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Carrier','Carriers','Español')] Carrier");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	JOIN (SELECT Carrier");
            consultaCbo.AppendLine("          FROM " + DSODataContext.Schema + "." + nomResumenCDR);
            consultaCbo.AppendLine("          GROUP BY Carrier");
            consultaCbo.AppendLine("        ) AS CDR");
            consultaCbo.AppendLine("		ON Carrier.iCodCatalogo = CDR.Carrier");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("WHERE Carrier.dtIniVigencia <> Carrier.dtFinVigencia");
            consultaCbo.AppendLine("    AND Carrier.dtFinVigencia >= GETDATE()");
            return consultaCbo.ToString();
        }

        private string ConsultaObtenerTipoDestinoCDR()
        {
            string condicionParticular = string.Empty;

            if (DSODataContext.Schema.ToLower() == "k5banorte")
                nomResumenCDR = "ResumenCDR";
            else
                nomResumenCDR = "[VisAcumulados('AcumDia','ResumenCDR','Español')]";

            if (DSODataContext.Schema.ToLower() == "fca")
                condicionParticular = " where convert(date, FechaInicio) >= '2020-12-01' "; //En FCA a partir de esta fecha están los tipos destino válidos.

            consultaCbo.Length = 0;
            consultaCbo.AppendLine("SELECT iCodCatalogo, TipoDestino=UPPER(Español)");
            consultaCbo.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')] TDest");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	JOIN (SELECT TDest");
            consultaCbo.AppendLine("          FROM " + DSODataContext.Schema + "." + nomResumenCDR);
            if(!string.IsNullOrEmpty(condicionParticular))
                consultaCbo.AppendLine(condicionParticular);
            consultaCbo.AppendLine("          GROUP BY TDest");
            consultaCbo.AppendLine("        ) AS CDR");
            consultaCbo.AppendLine("		ON TDest.iCodCatalogo = CDR.TDest");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("WHERE TDest.dtIniVigencia <> TDest.dtFinVigencia");
            consultaCbo.AppendLine("    AND TDest.dtFinVigencia >= GETDATE() AND CatTDestDesc = 'Fija'");
            return consultaCbo.ToString();
        }

        public static string ConsultaDetalleCDRAnterior()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("DECLARE @Where varchar(max)= ''   ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                if (HttpContext.Current.Session["Sitio"].ToString().ToLower().Replace(" ", "").Contains("telcel"))
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND ([Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " OR [Tipo de destino] like ''%" + HttpContext.Current.Session["Sitio"].ToString() + "%'')' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ");
                }
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [iCodEmple] = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo CenCos] = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [GEtiqueta] = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Carrier] = " + HttpContext.Current.Session["Carrier"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [iCodTDest] = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [iCodLocali] = " + HttpContext.Current.Session["IdLocali"].ToString() + "' ");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND REPLACE([Numero Marcado], '''''''', '''') = ''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "'' ' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Numero Marcado] LIKE ''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "%'' ' ");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Extensión] = ''" + HttpContext.Current.Session["Exten"].ToString() + "'' ' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Extensión] LIKE ''%" + HttpContext.Current.Session["Exten"].ToString() + "%'' ' ");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Autorizacion] LIKE ''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'' ' ");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND (CostoFac + CostoSM) " + HttpContext.Current.Session["OperadorCosto"].ToString() + HttpContext.Current.Session["Costo"].ToString() + "' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND (Costo + CostoSM) " + HttpContext.Current.Session["OperadorCosto"].ToString() + HttpContext.Current.Session["Costo"].ToString() + "' ");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Duracion] " + HttpContext.Current.Session["OperadorDuracion"].ToString() + HttpContext.Current.Session["Duracion"].ToString() + " ' ");
            }
            #endregion Filtro por Duracion

            consulta.AppendLine("exec [ConsumoDetalladoFormulario]   ");
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("[Centro de costos] , ");
            consulta.AppendLine("[Colaborador]	 , ");
            consulta.AppendLine("[Nomina],");
            consulta.AppendLine("[Extensión]	 , ");
            consulta.AppendLine("[Numero Marcado] , ");
            consulta.AppendLine("[Fecha] , ");
            consulta.AppendLine("[Hora] , ");
            consulta.AppendLine("[Fecha Fin],");
            consulta.AppendLine("[Hora Fin],");
            consulta.AppendLine("[Duracion] as [Cantidad minutos], ");
            consulta.AppendLine("[TotalSimulado] = (CostoFac+CostoSM), ");
            consulta.AppendLine("[TotalReal] = (Costo+CostoSM), ");
            consulta.AppendLine("[CostoSimulado] = (CostoFac), ");
            consulta.AppendLine("[CostoReal] = (Costo), ");
            consulta.AppendLine("[SM] =(CostoSM), ");
            consulta.AppendLine("[Nombre Localidad] as [Localidad], ");
            consulta.AppendLine("[Nombre Sitio]	as [Sitio] , ");
            consulta.AppendLine("[Codigo Autorizacion] , ");
            consulta.AppendLine("[Nombre Carrier] as [Carrier], ");
            ///////consulta.Append("[Tipo Llamada] , \r");
            consulta.AppendLine("[Tipo de destino],  ");
            consulta.AppendLine("[Categoría],  ");

            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Puesto]  ");
            }
            else if (DSODataContext.Schema.ToLower() == "kuehnenagel")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[IP] AS IVR");
            }
            else if (DSODataContext.Schema.ToLower() == "luxottica")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Circuito Entrada]  ");
            }
            else if (DSODataContext.Schema.ToLower() == "bat" || DSODataContext.Schema.ToLower() == "qualtia")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Organización]  ");
            }
            else
            {
                consulta.AppendLine("[Etiqueta]  ");
            }

            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @VistaAUsar = '" + tablaAUsuar + "',");  //Se agrega para saber de que tabla debe sair la información de fija.
            consulta.AppendLine("   @Moneda = '" + HttpContext.Current.Session["Currency"] + "', ");
            consulta.AppendLine("   @Idioma = '" + HttpContext.Current.Session["Language"] + "', ");
            consulta.AppendLine("   @FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00', ");
            consulta.AppendLine("   @FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59' ");

            return consulta.ToString();
        }

        private string ConsultaCountQueryDetalleCDRAnterior()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("DECLARE @Where varchar(max)= ''   ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                if (HttpContext.Current.Session["Sitio"].ToString().ToLower().Replace(" ", "").Contains("telcel"))
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND ([Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " OR [Tipo de destino] like ''%" + HttpContext.Current.Session["Sitio"].ToString() + "%'')' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ");
                }
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(txtNombre.Text) && !string.IsNullOrEmpty(txtEmpleId.Text))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [iCodEmple] = " + txtEmpleId.Text + "' ");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [iCodEmple] = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo CenCos] = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [GEtiqueta] = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Carrier] = " + HttpContext.Current.Session["Carrier"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [iCodTDest] = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [iCodLocali] = " + HttpContext.Current.Session["IdLocali"].ToString() + "' ");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND REPLACE([Numero Marcado], '''''''', '''') = ''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "'' ' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Numero Marcado] LIKE ''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "%'' ' ");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Extensión] = ''" + HttpContext.Current.Session["Exten"].ToString() + "'' ' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Extensión] LIKE ''%" + HttpContext.Current.Session["Exten"].ToString() + "%'' ' ");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Autorizacion] LIKE ''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'' ' ");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND (CostoFac + CostoSM) " + HttpContext.Current.Session["OperadorCosto"].ToString() + HttpContext.Current.Session["Costo"].ToString() + "' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND (Costo + CostoSM) " + HttpContext.Current.Session["OperadorCosto"].ToString() + HttpContext.Current.Session["Costo"].ToString() + "' ");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Duracion] " + HttpContext.Current.Session["OperadorDuracion"].ToString() + HttpContext.Current.Session["Duracion"].ToString() + " ' ");
            }
            #endregion Filtro por Duracion

            consulta.AppendLine("EXEC [ConsumoDetalladoFormularioCount]");
            consulta.AppendLine("   @Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @MaxCount = " + (maxRegistrosEnExcel + 1).ToString() + ", ");
            consulta.AppendLine("   @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @VistaAUsar = '" + tablaAUsuar + "',");  //Se agrega para saber de que tabla debe sair la información de fija.
            consulta.AppendLine("   @Idioma = '" + HttpContext.Current.Session["Language"] + "', ");
            consulta.AppendLine("   @FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00', ");
            consulta.AppendLine("   @FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59' ");

            return consulta.ToString();
        }

        #endregion DetalleCDR

        #region DetalleFactura

        private string ConsultaObtenerSitiosMovil()
        {
            consultaCbo.Length = 0;
            consultaCbo.AppendLine("DECLARE @Perfil INT = " + Session["iCodPerfil"].ToString());
            consultaCbo.AppendLine("DECLARE @Usuario INT = " + Session["iCodUsuario"].ToString());
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("IF(@Perfil <> 367)"); //Perfil configurador.
            consultaCbo.AppendLine("BEGIN");
            consultaCbo.AppendLine("	CREATE TABLE #RestSitio ( iCodCatalogo INT )");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	INSERT INTO #RestSitio(iCodCatalogo)");
            consultaCbo.AppendLine("	SELECT Sitio FROM " + DSODataContext.Schema + ".RestSitio");
            consultaCbo.AppendLine("	WHERE FechaInicio <> FechaFin AND FechaFin >= GETDATE()");
            consultaCbo.AppendLine("		AND Usuar = @Usuario");
            consultaCbo.AppendLine("		AND Perfil = @Perfil");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	SELECT Sitio.iCodCatalogo, Sitio=UPPER(Sitio.vchDescripcion)");
            consultaCbo.AppendLine("    FROM ( SELECT Sitio");
            consultaCbo.AppendLine("           FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles M");
            consultaCbo.AppendLine("           WHERE M.Carrier <> 374 AND Sitio IS NOT NULL");
            consultaCbo.AppendLine("           GROUP BY Sitio) AS Resumen");

            consultaCbo.AppendLine("	    JOIN " + DSODataContext.Schema + ".[VisHisComun('Sitio','Español')] Sitio");
            consultaCbo.AppendLine("            ON Resumen.Sitio = Sitio.iCodCatalogo");
            consultaCbo.AppendLine("	        AND Sitio.dtIniVigencia <> Sitio.dtFinVigencia");
            consultaCbo.AppendLine("		    AND Sitio.dtFinVigencia >= GETDATE()");

            consultaCbo.AppendLine("		JOIN #RestSitio RestSitio");
            consultaCbo.AppendLine("		ON Sitio.iCodCatalogo = RestSitio.iCodCatalogo");
            consultaCbo.AppendLine("END");
            consultaCbo.AppendLine("ELSE");
            consultaCbo.AppendLine("BEGIN ");
            consultaCbo.AppendLine("	SELECT Sitio.iCodCatalogo, Sitio=UPPER(Sitio.vchDescripcion)");
            consultaCbo.AppendLine("    FROM ( SELECT Sitio");
            consultaCbo.AppendLine("           FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles M");
            consultaCbo.AppendLine("           WHERE M.Carrier <> 374 AND Sitio IS NOT NULL");
            consultaCbo.AppendLine("           GROUP BY Sitio) AS Resumen");
            consultaCbo.AppendLine("	    JOIN " + DSODataContext.Schema + ".[VisHisComun('Sitio','Español')] Sitio");
            consultaCbo.AppendLine("            ON Resumen.Sitio = Sitio.iCodCatalogo");
            consultaCbo.AppendLine("	        AND Sitio.dtIniVigencia <> Sitio.dtFinVigencia");
            consultaCbo.AppendLine("		    AND Sitio.dtFinVigencia >= GETDATE()");
            consultaCbo.AppendLine("END");

            return consultaCbo.ToString();
        }

        private string ConsultaObtenerCarrierMovil()
        {
            consultaCbo.Length = 0;
            consultaCbo.AppendLine("SELECT iCodCatalogo, Carrier=UPPER(vchDescripcion)");
            consultaCbo.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Carrier','Carriers','Español')] Carrier");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	JOIN (SELECT Carrier");
            consultaCbo.AppendLine("          FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles");
            consultaCbo.AppendLine("          GROUP BY Carrier");
            consultaCbo.AppendLine("        ) AS Resumen");
            consultaCbo.AppendLine("		ON Carrier.iCodCatalogo = Resumen.Carrier");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("WHERE Carrier.dtIniVigencia <> Carrier.dtFinVigencia");
            consultaCbo.AppendLine("    AND Carrier.dtFinVigencia >= GETDATE()");
            return consultaCbo.ToString();
        }

        private string ConsultaObtenerTipoDestinoMovil()
        {
            consultaCbo.Length = 0;
            consultaCbo.AppendLine("SELECT iCodCatalogo, TipoDestino=UPPER(Español)");
            consultaCbo.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')] TDest");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	JOIN (SELECT TDest");
            consultaCbo.AppendLine("          FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles");
            consultaCbo.AppendLine("          GROUP BY TDest");
            consultaCbo.AppendLine("        ) AS Resumen");
            consultaCbo.AppendLine("		ON TDest.iCodCatalogo = Resumen.TDest");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("WHERE TDest.dtIniVigencia <> TDest.dtFinVigencia");
            consultaCbo.AppendLine("    AND TDest.dtFinVigencia >= GETDATE() AND CatTDestDesc = 'Móvil'");
            return consultaCbo.ToString();
        }

        public static string ConsultaDetalleFacturaMovil()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("DECLARE @Where varchar(max)= ''   ");
            consulta.AppendLine("DECLARE @OrderInv varchar(max)  ");
            consulta.Append("SELECT @Where = @Where + '[FechaInicio] >= ''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'' ");
            consulta.AppendLine(" AND [FechaInicio] <= ''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''' ");

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                if (HttpContext.Current.Session["Sitio"].ToString().ToLower().Replace(" ", "").Contains("telcel"))
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND ([Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " OR [Tipo de destino] like ''%" + HttpContext.Current.Session["Sitio"].ToString() + "%'')' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ");
                }
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Empleado] = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Centro de Costos] = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Tipo Llamada] = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Carrier] = " + HttpContext.Current.Session["Carrier"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Tipo Destino] = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Linea

            if (HttpContext.Current.Session["Linea"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Linea"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["LineaExacta"]) == true)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Linea] = ''" + HttpContext.Current.Session["Linea"].ToString() + "'' ' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Linea] LIKE ''%" + HttpContext.Current.Session["Linea"].ToString() + "%'' ' ");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND ([Costo]/[TipoCambio]) " + HttpContext.Current.Session["OperadorCosto"].ToString() + HttpContext.Current.Session["Costo"].ToString() + "' ");
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Duracion Minutos] " + HttpContext.Current.Session["OperadorDuracion"].ToString() + HttpContext.Current.Session["Duracion"].ToString() + " ' ");
            }
            #endregion Filtro por Duracion

            consulta.AppendLine("exec [RepTabDetalladosMovil]");
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("[Nombre Centro de Costos]=upper(Convert(varchar,[Numero Centro de Costos])+'' - ''+[Nombre Centro de Costos]) , ");
            consulta.AppendLine("[Codigo Centro de Costos]	 , ");
            consulta.AppendLine("[Nombre Completo]=upper([Nombre Completo]),");
            consulta.AppendLine("[Linea]	 , ");
            consulta.AppendLine("[Fecha] , ");
            consulta.AppendLine("[Hora] , ");
            consulta.AppendLine("[Duracion] = [Duracion Minutos], ");
            consulta.AppendLine("[Total]=([Costo]/[TipoCambio]), ");
            consulta.AppendLine("[Nombre Sitio]=upper([Nombre Sitio]), ");
            consulta.AppendLine("[Nombre Tipo Destino]=upper([Nombre Tipo Destino]), ");
            consulta.AppendLine("[Nombre Carrier]=upper([Nombre Carrier]), ");

            if (DSODataContext.Schema.ToLower() == "bat" || DSODataContext.Schema.ToLower() == "qualtia")
            {
                consulta.AppendLine("[Tipo Llamada]=upper([Tipo Llamada]),");
                consulta.AppendLine("[Organización]  ");
            }
            else
            {
                consulta.AppendLine("[Tipo Llamada]=upper([Tipo Llamada]) ");
            }

            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @Order = '[Total] Desc',  ");
            consulta.AppendLine("   @OrderInv = '[Total] Asc',");
            consulta.AppendLine("   @Lenght = " + maxRegistrosEnExcel + ",");
            consulta.AppendLine("   @OrderDir = 'Asc',");
            consulta.AppendLine("   @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Idioma = '" + HttpContext.Current.Session["Language"] + "', ");
            consulta.AppendLine("   @FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00', ");
            consulta.AppendLine("   @FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59' ");

            return consulta.ToString();
        }

        private string ConsultaCountQueryDetalleFacturaMovil()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("DECLARE @Where varchar(max)= ''   ");
            consulta.Append("SELECT @Where = @Where + '[FechaInicio] >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' ");
            consulta.AppendLine(" AND [FechaInicio] <= ''" + Session["FechaFin"].ToString() + " 23:59:59''' ");

            #region Filtro por Sitio
            if (Session["Sitio"] != null && !string.IsNullOrEmpty(Session["Sitio"].ToString()) && Session["Sitio"].ToString() != "0")
            {
                if (Session["Sitio"].ToString().ToLower().Replace(" ", "").Contains("telcel"))
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND ([Codigo Sitio] = " + Session["Sitio"].ToString() + " OR [Tipo de destino] like ''%" + Session["Sitio"].ToString() + "%'')' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Sitio] = " + Session["Sitio"].ToString() + " ' ");
                }
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(txtNombre.Text) && !string.IsNullOrEmpty(txtEmpleId.Text))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Empleado] = " + txtEmpleId.Text + "' ");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(txtCenCos.Text) && !string.IsNullOrEmpty(txtCenCosId.Text))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Centro de Costos] = " + txtCenCosId.Text + "' ");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (Session["TipoLlam"] != null && !string.IsNullOrEmpty(Session["TipoLlam"].ToString()) && Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Tipo Llamada] = " + Session["TipoLlam"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (Session["Carrier"] != null && !string.IsNullOrEmpty(Session["Carrier"].ToString()) && Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Carrier] = " + Session["Carrier"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (Session["TipoDest"] != null && !string.IsNullOrEmpty(Session["TipoDest"].ToString()) && Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Tipo Destino] = " + Session["TipoDest"].ToString() + "' ");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Linea

            if (!string.IsNullOrEmpty(txtLinea.Text))
            {
                if (banderaLineaExacta.Checked == true)
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Linea] = ''" + txtLinea.Text + "'' ' ");
                }
                else
                {
                    consulta.AppendLine("SELECT @Where = @Where + ' AND [Linea] LIKE ''%" + txtLinea.Text + "%'' ' ");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(txtCosto.Text))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND ([Costo]/[TipoCambio]) " + Session["OperadorCosto"].ToString() + txtCosto.Text + "' ");
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(txtDuracion.Text))
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Duracion Minutos] " + Session["OperadorDuracion"].ToString() + txtDuracion.Text + " ' ");
            }
            #endregion Filtro por Duracion

            consulta.AppendLine("exec [RepTabDetalladosMovilCount]   ");
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @MaxCount = " + maxRegistrosEnExcel + ",");
            consulta.AppendLine("   @Usuario = " + Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Idioma = '" + Session["Language"] + "', ");
            consulta.AppendLine("   @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            consulta.AppendLine("   @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59' ");

            return consulta.ToString();
        }

        #endregion DetalleFactura

        public static DataTable ObtenerCamposConNombreParticular(string nombreMetodoConsulta)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.Append("SELECT MetodoConsulta, NombreOrigCampo, NombreNuevoCampo \r ");
            consulta.Append("FROM " + DSODataContext.Schema + ".CamposConNombreParticular \r ");
            consulta.Append("WHERE dtFinVigencia <> dtIniVigencia \r ");
            consulta.Append("AND dtFinVigencia >= GETDATE() \r ");
            consulta.Append("AND MetodoConsulta = '" + nombreMetodoConsulta + "'");

            return DSODataAccess.Execute(consulta.ToString());
        }

        private static string BuscarElemento(string link, string nombreSP, string texto)
        {
            StringBuilder querySP = new StringBuilder();
            querySP.AppendLine("EXEC " + nombreSP);
            querySP.AppendLine("  	@Schema = '" + DSODataContext.Schema + "',");
            querySP.AppendLine("      @Texto = '''%" + texto.Trim().Replace(" ", "%").Replace("'", "") + "%''',");
            querySP.AppendLine("      @Link = '''''',");
            querySP.AppendLine("      @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            querySP.AppendLine("      @Perfil = " + HttpContext.Current.Session["iCodPerfil"]);

            return querySP.ToString();
        }

        #endregion Consultas a BD

        #region WebMethod

        [WebMethod]
        public static object ConsultaAutoComplateEmple(string texto)
        {
            DataTable Emple = DSODataAccess.Execute(BuscarElemento("", "BusquedaPorEmpleado", texto));
            DataView dvldt = new DataView(Emple);
            Emple = dvldt.ToTable(false, new string[] { "Codigo Empleado", "Nomina", "Nombre Completo" });
            Emple.Columns["Codigo Empleado"].ColumnName = "Id";
            Emple.Columns["Nombre Completo"].ColumnName = "Nombre";
            return FCAndControls.ConvertDataTabletoJSONString(Emple);
        }

        [WebMethod]
        public static object ConsultaAutoComplateCenCos(string texto)
        {
            DataTable CenCos = DSODataAccess.Execute(BuscarElemento("", "BusquedaPorCenCos", texto));
            DataView dvldt = new DataView(CenCos);
            CenCos = dvldt.ToTable(false, new string[] { "Codigo CenCos", "Clave CenCos", "Centro de Costos" });

            CenCos.Columns["Codigo CenCos"].ColumnName = "Id";
            CenCos.Columns["Clave CenCos"].ColumnName = "Clave";
            CenCos.Columns["Centro de Costos"].ColumnName = "Descripcion";
            return FCAndControls.ConvertDataTabletoJSONString(CenCos);
        }

        [WebMethod]
        public static object ConsultaAutoComplateLocali(string texto)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("SELECT Descripcion = vchDescripcion, Id = iCodCatalogo");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Locali','Localidades','Español')]");
            lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsb.AppendLine("    AND dtFinVigencia >= GETDATE()");
            lsb.AppendLine("    AND vchDescripcion LIKE '%" + texto.Replace("'", "") + "%'");

            DataTable Locali = DSODataAccess.Execute(lsb.ToString());
            return FCAndControls.ConvertDataTabletoJSONString(Locali);
        }

        #endregion

        #region Reporte Detallado

        private void LlenarDatosFormulario()
        {
            string nomIni = "ctl00$cphContent$";
            Session["Sitio"] = Request.Form.Get(nomIni + "cboUbicacion");
            Session["TipoLlam"] = Request.Form.Get(nomIni + "cboTipoLlamada");
            Session["TipoDest"] = Request.Form.Get(nomIni + "cboTipoDestino");
            Session["Carrier"] = Request.Form.Get(nomIni + "cboCarrier");
            Session["OperadorDuracion"] = Request.Form.Get(nomIni + "cboCriteriosDuracion");
            Session["OperadorCosto"] = Request.Form.Get(nomIni + "cboCriterioCosto");

            Session["NombreEmple"] = txtNombre.Text;
            Session["IdEmple"] = txtEmpleId.Text;
            Session["NombreCenCos"] = txtCenCos.Text;
            Session["IdCenCos"] = txtCenCosId.Text;
            Session["NombreLocali"] = txtLocali.Text;
            Session["IdLocali"] = txtLocaliId.Text;
            Session["NumMarcado"] = txtNumMarcado.Text;
            Session["NumMarcadoExacto"] = banderaNumMarcado.Checked;
            Session["CodAuto"] = txtCodigo.Text;
            Session["Exten"] = txtExtension.Text;
            Session["ExtenExacta"] = banderaExtensionExacta.Checked;
            Session["Duracion"] = txtDuracion.Text;
            Session["Costo"] = txtCosto.Text;

            Session["Linea"] = txtLinea.Text;
            Session["LineaExacta"] = banderaLineaExacta.Checked;

        }

        private void ReporteDetallado(Control contenedor, string tituloGrid)
        {
            //NZ Crea el contenedor de la tabla.
            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepDetalleSoloTabla("ReporteDetallado", tituloGrid));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionDetalleFija", "<script language=javascript> GetDatosTabla('ReporteDetallado', 'ReporteDetalladoWebM'); </script>", false);
        }

        [WebMethod]
        public static object ReporteDetalladoWebM()
        {


           
            if (DSODataContext.Schema.ToLower() == "laureate")
            {

                RepDetallado = DSODataAccess.Execute(ConsultaLaureateConsumoDetallado(SoloLlamadasFueraDeHorario));
                return FCAndControls.ConvertToJSONStringDetalle(RepDetallado);
            }
            else
            {
                RepDetallado.Clear();
                RepDetallado = DSODataAccess.Execute(ConsultaDetalleCDR());
            }
           
           

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;

            if (RepDetallado.Rows.Count > 0 && RepDetallado.Columns.Count > 0)
            {
                DataView dvldt = new DataView(RepDetallado);
                if (RepDetallado.Columns.Contains("RID"))
                    RepDetallado.Columns.Remove("RID");
                if (RepDetallado.Columns.Contains("RowNumber"))
                    RepDetallado.Columns.Remove("RowNumber");
                if (RepDetallado.Columns.Contains("TopRID"))
                    RepDetallado.Columns.Remove("TopRID");

             //   RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                #region Cambiar nombre de campos particulares
                DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                if (nombresCamposParticulares.Rows.Count > 0)
                {
                    foreach (DataRow row in nombresCamposParticulares.Rows)
                    {
                        if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                        {
                            RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                        }
                    }
                }
                #endregion Cambiar nombre de campos particulares
                //20150703 NZ

                if (RepDetallado.Columns.Contains("Numero Marcado"))
                    RepDetallado.Columns["Numero Marcado"].ColumnName = "Número Marcado";

                if (RepDetallado.Columns.Contains("Duracion"))
                    RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos"; //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"                

                if (RepDetallado.Columns.Contains("Codigo Autorizacion"))
                    RepDetallado.Columns["Codigo Autorizacion"].ColumnName = "Código Autorización";

                if (RepDetallado.Columns.Contains("Circuito Entrada"))
                    RepDetallado.Columns["Circuito Entrada"].ColumnName = "Usuario Final";

                if (RepDetallado.Columns.Contains("Total"))
                    RepDetallado.Columns["Total"].ColumnName = "Costo";


                ArrayList lista = FormatoColumRepDetallado(RepDetallado, columnasNoVisibles, columnasVisibles);
                RepDetallado = (DataTable)((object)lista[0]);
                columnasNoVisibles = (int[])((object)lista[1]);
                columnasVisibles = (int[])((object)lista[2]);

                //if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "kuehnenagel" || DSODataContext.Schema.ToLower() == "luxottica")
                //{
                int indicePuesto = Convert.ToInt32(columnasVisibles.Last());
                Array.Resize<int>(ref columnasVisibles, columnasVisibles.Count() + 1);
                columnasVisibles[columnasVisibles.Length - 1] = indicePuesto + 1;
                //}

                for (int i = columnasNoVisibles.Length - 1; i >= 0; i--)
                {
                    RepDetallado.Columns.RemoveAt(columnasNoVisibles[i]);
                }

                return FCAndControls.ConvertToJSONStringDetalle(RepDetallado);
            }
            else { return null; }
        }

        public static ArrayList FormatoColumRepDetallado(DataTable ldt, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(HttpContext.Current.Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 10, 11, 13 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 14, 15, 16, 17, 18 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 10, 11, 12 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 13, 14, 15, 16, 17, 18 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";
                }
            }
            else
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 10, 13, 14 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 15, 16, 17, 18 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 10, 12, 13, 14 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 15, 16, 17, 18 };
                    ldt.Columns["TotalReal"].ColumnName = "Total";
                }
            }

            //if (Convert.ToInt32(HttpContext.Current.Session["MuestraSM"]) == 1)
            //{
            //    if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
            //    {
            //        columnasNoVisibles = new int[] { 9, 10, 11 };
            //        columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 12, 13, 14, 15, 16, 17, 18 };
            //        ldt.Columns["CostoSimulado"].ColumnName = "Total";
            //    }
            //    else
            //    {
            //        columnasNoVisibles = new int[] { 9, 10, 11 };
            //        columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 12, 13, 14, 15, 16, 17, 18 };
            //        ldt.Columns["CostoReal"].ColumnName = "Total";
            //    }
            //}
            //else
            //{
            //    if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
            //    {
            //        columnasNoVisibles = new int[] { 9, 10, 11, 12 };
            //        columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 13, 14, 15, 16, 17, 18 };
            //        ldt.Columns["TotalSimulado"].ColumnName = "Total";
            //    }
            //    else
            //    {
            //        columnasNoVisibles = new int[] { 9, 10, 11, 12 };
            //        columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 13, 14, 15, 16, 17, 18 };
            //        ldt.Columns["TotalReal"].ColumnName = "Total";
            //    }
            //}

            #endregion Logica de las columnas a mostrar

            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);
            return valores;
        }

        private void ReporteDetalleFacturaMovil(Control contenedor, string tituloGrid)
        {
            //NZ Crea el contenedor de la tabla.
            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepDetalleSoloTabla("ReporteDetalleFacturaMovil", tituloGrid));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionDetalleMovil", "<script language=javascript> GetDatosTabla('ReporteDetalleFacturaMovil', 'ReporteDetalleFacturaMovilWebM'); </script>", false);
        }

        [WebMethod]
        public static object ReporteDetalleFacturaMovilWebM()
        {
            RepDetalleFacturaMovil.Clear();
            RepDetalleFacturaMovil = DSODataAccess.Execute(ConsultaDetalleFacturaMovil());

            if (RepDetalleFacturaMovil.Rows.Count > 0 && RepDetalleFacturaMovil.Columns.Count > 0)
            {
                DataView dvldt = new DataView(RepDetalleFacturaMovil);
                if (RepDetalleFacturaMovil.Columns.Contains("RID"))
                    RepDetalleFacturaMovil.Columns.Remove("RID");
                if (RepDetalleFacturaMovil.Columns.Contains("RowNumber"))
                    RepDetalleFacturaMovil.Columns.Remove("RowNumber");
                if (RepDetalleFacturaMovil.Columns.Contains("TopRID"))
                    RepDetalleFacturaMovil.Columns.Remove("TopRID");

                if (RepDetalleFacturaMovil.Columns.Contains("Duracion"))
                    RepDetalleFacturaMovil.Columns["Duracion"].ColumnName = "Cantidad minutos"; //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"                

                RepDetalleFacturaMovil.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                RepDetalleFacturaMovil.Columns["Nombre Completo"].ColumnName = "Colaborador";
                RepDetalleFacturaMovil.Columns["Linea"].ColumnName = "Línea";
                RepDetalleFacturaMovil.Columns["Nombre Sitio"].ColumnName = "Sitio";
                RepDetalleFacturaMovil.Columns["Nombre Carrier"].ColumnName = "Carrier";
                RepDetalleFacturaMovil.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                RepDetalleFacturaMovil.Columns["Tipo Llamada"].ColumnName = "Categoría";
                RepDetalleFacturaMovil.Columns["Total"].ColumnName = "Costo";

                int[] columnasNoVisibles = new int[] { };
                int[] columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

                for (int i = columnasNoVisibles.Length - 1; i >= 0; i--)
                {
                    RepDetalleFacturaMovil.Columns.RemoveAt(columnasNoVisibles[i]);
                }

                return FCAndControls.ConvertToJSONStringDetalle(RepDetalleFacturaMovil);
            }
            else { return null; }
        }


        #endregion

        #region Exportacion Reporte detallado

        public void ExportXLS(string tipoExtensionArchivo)
        {
            CrearXLS(tipoExtensionArchivo);
        }

        protected void CrearXLS(string lsExt)
        {
            //PruebaTFS
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                #region Exportar Reportes solo con tabla

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                if (isReporteDetalleFactura)
                {
                    if (RepDetalleFacturaMovil.Rows.Count == 0)
                    {
                        RepDetalleFacturaMovil.Clear();
                        RepDetalleFacturaMovil = DSODataAccess.Execute(ConsultaDetalleFacturaMovil());

                        #region Elimina columnas no necesarias en el gridview
                        if (RepDetalleFacturaMovil.Columns.Contains("RID"))
                            RepDetalleFacturaMovil.Columns.Remove("RID");
                        if (RepDetalleFacturaMovil.Columns.Contains("RowNumber"))
                            RepDetalleFacturaMovil.Columns.Remove("RowNumber");
                        if (RepDetalleFacturaMovil.Columns.Contains("TopRID"))
                            RepDetalleFacturaMovil.Columns.Remove("TopRID");
                        #endregion // Elimina columnas no necesarias en el gridview

                        if (RepDetalleFacturaMovil.Columns.Contains("Duracion"))
                            RepDetalleFacturaMovil.Columns["Duracion"].ColumnName = "Cantidad minutos"; //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"                

                        RepDetalleFacturaMovil.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                        RepDetalleFacturaMovil.Columns["Nombre Completo"].ColumnName = "Colaborador";
                        RepDetalleFacturaMovil.Columns["Linea"].ColumnName = "Línea";
                        RepDetalleFacturaMovil.Columns["Nombre Sitio"].ColumnName = "Sitio";
                        RepDetalleFacturaMovil.Columns["Nombre Carrier"].ColumnName = "Carrier";
                        RepDetalleFacturaMovil.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                        RepDetalleFacturaMovil.Columns["Tipo Llamada"].ColumnName = "Categoría";
                        RepDetalleFacturaMovil.Columns["Total"].ColumnName = "Costo";

                    }
                    else
                    {
                        if (Convert.ToString(RepDetalleFacturaMovil.Rows[RepDetalleFacturaMovil.Rows.Count - 1][0]) == "Totales")
                        {
                            RepDetalleFacturaMovil.Rows.RemoveAt(RepDetalleFacturaMovil.Rows.Count - 1);
                        }
                    }

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleFacturaMovil, 0, "Totales"), "Reporte", "Tabla");
                }
                else  //Fija
                {

                    RepDetallado.Clear();

                    //FD 19/03/2021 Laureate
                    if(DSODataContext.Schema.ToLower() == "laureate")
                    {
                        RepDetallado = DSODataAccess.Execute(ConsultaLaureateConsumoDetallado(chkLlamadasFueraDeHorario.Checked));
                    }
                    else
                    {
                        RepDetallado = DSODataAccess.Execute(ConsultaDetalleCDR());
                        RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                        #region Elimina columnas no necesarias en el gridview
                        if (RepDetallado.Columns.Contains("RID"))
                            RepDetallado.Columns.Remove("RID");
                        if (RepDetallado.Columns.Contains("RowNumber"))
                            RepDetallado.Columns.Remove("RowNumber");
                        if (RepDetallado.Columns.Contains("TopRID"))
                            RepDetallado.Columns.Remove("TopRID");
                        #endregion // Elimina columnas no necesarias en el gridview



                        //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                        #region Cambiar nombre de campos particulares
                        DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                        if (nombresCamposParticulares.Rows.Count > 0)
                        {
                            foreach (DataRow row in nombresCamposParticulares.Rows)
                            {
                                if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                                {
                                    RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                                }
                            }
                        }
                        #endregion Cambiar nombre de campos particulares
                        //20150703 NZ

                        if (RepDetallado.Columns.Contains("Numero Marcado"))
                        {
                            RepDetallado.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                        }
                        if (RepDetallado.Columns.Contains("Duracion"))
                        {
                            RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos"; //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                        }
                        if (RepDetallado.Columns.Contains("Codigo Autorizacion"))
                        {
                            RepDetallado.Columns["Codigo Autorizacion"].ColumnName = "Código Autorización";
                        }
                        //if (RepDetallado.Columns.Contains("Circuito Entrada"))
                        //{
                        //    RepDetallado.Columns["Circuito Entrada"].ColumnName = "Usuario Final";
                        //}

                        RepDetallado = ElimColDeAcuerdoABanClientePerfil(RepDetallado);
                        RepDetallado.Columns["Total"].ColumnName = "Costo"; //NZ El nombre de la columna Total se cambia por Costo
                    }
                    

                   

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
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
                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes" + "_" + "Consumo_Detallado" + "_");
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

        private DataTable ElimColDeAcuerdoABanClientePerfil(DataTable Tabla)
        {
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns["CostoSimulado"].ColumnName = "Total";
                    Tabla.Columns["SM"].ColumnName = "Servicio Medido";
                }
                else
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns["CostoReal"].ColumnName = "Total";
                    Tabla.Columns["SM"].ColumnName = "Servicio Medido";
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalSimulado"].ColumnName = "Total";
                }
                else
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalReal"].ColumnName = "Total";
                }
            }

            return Tabla;
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion Exportacion Reporte detallado

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            ValidarTipoReporte();
            LlenarDatosFormulario();
            RepDetallado.Clear();
            RepDetalleFacturaMovil.Clear();
            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                if (chkLlamadasFueraDeHorario.Checked)
                {
                    SoloLlamadasFueraDeHorario = true;
                }
                else
                {
                    SoloLlamadasFueraDeHorario = false;
                }
                MostrarDatosLaureate(chkLlamadasFueraDeHorario.Checked);
            }
            else
            {
                //20161109 NZ Se agrega consulta del count que resultara en el detallado para advertir al usuario si se podra exportar la información o no.
                double countConsulta = 0;
                if (isReporteDetalleFactura)
                {
                    countConsulta = Convert.ToDouble((object)DSODataAccess.ExecuteScalar(ConsultaCountQueryDetalleFacturaMovil()));
                }
                else { countConsulta = Convert.ToDouble((object)DSODataAccess.ExecuteScalar(ConsultaCountQueryDetalleCDR())); } //Fija

                if (countConsulta != 0)
                {
                   if (countConsulta <= maxRegistrosEnExcel)
                   
                    {
                        if (countConsulta <= maxRegistrosEnWeb)
                        {
                            if (isReporteDetalleFactura)
                            {
                                ReporteDetalleFacturaMovil(Rep9, "Detalle Móvil");
                            }
                            else { ReporteDetallado(Rep9, "Detalle"); }  //Fija

                            Rep0.Visible = false;
                            Rep0.Visible = false;
                            Rep9.Visible = true;
                            pnlMapaNav.Visible = true;

                            List<MapNav> listaNavegacion = new List<MapNav>();
                            listaNavegacion.Add(new MapNav() { Titulo = "Filtro", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                            listaNavegacion.Add(new MapNav() { Titulo = "Detalle", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                            pnlMapaNavegacion.Controls.Clear();
                            pnlMapaNavegacion.Controls.Add(DTIChartsAndControls.MapaNavegacion(listaNavegacion));
                        }
                        else { ExportXLS(".xlsx"); }
                    }
                    else
                    {
                        mpeEtqMail.Show();
                    }
                }
                else
                {
                    lblTituloModalMsn.Text = "No hay datos para mostrar";
                    lblBodyModalMsn.Text = "La consulta no produjo ningún resultado.";
                    mpeEtqMsn.Show();
                }
            }

          
        }

        private void MostrarDatosLaureate(bool LlamadasFuerDeHorario)
        {
            organizacion = ddlOrganizacion.SelectedValue;
            direccionLlamada = ddlDirLLam.SelectedValue;

            double countConsulta = 0;
            
            
            countConsulta = Convert.ToDouble((object)DSODataAccess.ExecuteScalar(ConsultaCountQueryLaureateRepConsumoDetallado(LlamadasFuerDeHorario)));




            if (countConsulta != 0)
            {
                if (countConsulta <= maxRegistrosEnExcel)
                //if (countConsulta <= 100000)
                {
                    if (countConsulta <= maxRegistrosEnWeb)
                   // if (countConsulta <= 100000)
                        {
                        if (isReporteDetalleFactura)
                        {
                            ReporteDetalleFacturaMovil(Rep9, "Detalle Móvil");
                        }
                        else { ReporteDetallado(Rep9, "Detalle"); }  //Fija

                        Rep0.Visible = false;
                        Rep9.Visible = true;
                        pnlMapaNav.Visible = true;

                        List<MapNav> listaNavegacion = new List<MapNav>();
                        listaNavegacion.Add(new MapNav() { Titulo = "Filtro", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                        listaNavegacion.Add(new MapNav() { Titulo = "Detalle", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                        pnlMapaNavegacion.Controls.Clear();
                        pnlMapaNavegacion.Controls.Add(DTIChartsAndControls.MapaNavegacion(listaNavegacion));
                    }
                    else { ExportXLS(".xlsx"); }
                }
                else
                {
                    mpeEtqMail.Show();
                }
            }
        }

        private string ConsultaCountQueryLaureateRepConsumoDetallado(bool llamadasfueradehorario)
        {
            string iCodUsuario = HttpContext.Current.Session["iCodUsuario"].ToString();
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("USE KEYTIA5");
            consulta.AppendLine($"declare @fechainicio varchar(20) = '{HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00"}'");
            consulta.AppendLine($"declare @fechafin varchar(20) = '{HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59"}'");
            consulta.AppendLine("declare @organizacion varchar(40)");
            consulta.AppendLine("declare @direccionLlamada varchar(40)");
            consulta.AppendLine("declare @claveUsuario varchar(40)");
            consulta.AppendLine("declare @iCodCatUsuar int");

            //Validacion De Organizacion
            if (organizacion != "0")
            {
                if (organizacion == "1") //UVM Todas las organizaciones
                {
                    consulta.AppendLine("set @claveUsuario = 'AdministracionUVM'");
                    consulta.AppendLine("set @organizacion = 'UVM'");

                }
                else
                {
                    consulta.AppendLine("set @claveUsuario = 'AdministracionUNITEC'");
                    consulta.AppendLine("set @organizacion = 'Unitec'");


                }
                consulta.AppendLine("select @iCodCatUsuar = iCodCatalogo");
                consulta.AppendLine("from Laureate.[VisHistoricos('Usuar','Usuarios','Español')]");
                consulta.AppendLine("where vchcodigo = @claveUsuario");
                consulta.AppendLine("and dtfinvigencia>=getdate()");

            }
            else
            {
                consulta.AppendLine($"SET @icodcatUsuar = {iCodUsuario}");
                consulta.AppendLine("set @organizacion = 'TODAS'");

            }

            //Validacion de Dirección de LLamdas
            if (direccionLlamada == "2")
            {
                consulta.AppendLine($"set @direccionllamada = ' = ''SALIDA'' ' ");


            }
            else if (direccionLlamada == "1")
            {
                consulta.AppendLine($"set @direccionllamada = ' = ''ENTRADA'' ' ");

            }
          
            consulta.AppendLine($"exec [LaureateRepConsumoDetalladoCount]");
            consulta.AppendLine($"@Schema = 'Laureate', ");
            consulta.AppendLine($"@Usuario = @iCodCatUsuar, ");
            consulta.AppendLine($"@Perfil = 369,  ");
            consulta.AppendLine($"@VistaAUsar = 'CDR', ");
            consulta.AppendLine($"@Moneda = 'MXP', ");
            consulta.AppendLine($"@Idioma = 'Español', ");


            #region Filtros

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = ' = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ,");
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = ' = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ,");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = ' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ,");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = ' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = '= " + HttpContext.Current.Session["Carrier"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = ' = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = ' = " + HttpContext.Current.Session["IdLocali"].ToString() + "' ,");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("@NumeroTelf = ' =  ''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "'' ',");
                }
                else
                {
                    consulta.AppendLine("@NumeroTelf = ' LIKE ''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "%'' ',");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("@Extension = ' = ''" + HttpContext.Current.Session["Exten"].ToString() + "'' ' ,");
                }
                else
                {
                    consulta.AppendLine("@Extension = ' LIKE ''%" + HttpContext.Current.Session["Exten"].ToString() + "%'' ' ,");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = ' LIKE ''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'' ' ,");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("@CostoSim = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
                else
                {
                    consulta.AppendLine("@Costo = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = ' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " ' ,");
            }
            #endregion Filtro por Duracion

            #endregion


            consulta.AppendLine($"@FechaIniRep = @fechainicio,  ");
            consulta.AppendLine($"@FechaFinRep = @fechafin  ");

            if (direccionLlamada != "0")
            {
                consulta.AppendLine($",@CategoriaLlamada = @direccionllamada ");
            }
            if (llamadasfueradehorario)
            {
                consulta.AppendLine($",@Where2 = 'AND {BuscaHorario()}'");

            }



            return consulta.ToString();
        }
        //RM 20190212 BuscaHorario
        public static string BuscaHorario()
        {
            try
            {
                string WhereHorario = string.Empty;

                StringBuilder query = new StringBuilder();

                query.AppendLine("Select ");
                query.AppendLine("    vchCodigo = vchCodigo,");
                query.AppendLine("    vchDescripcion = vchDescripcion,");
                query.AppendLine("    Horainicio = HoraInicioHorarioLaboral,");
                query.AppendLine("    MinutoInicio = MinutoInicioHorarioLaboral,");
                query.AppendLine("    HoraFin = HoraFinHorarioLaboral,");
                query.AppendLine("    MinutoFin = MinutoFinHorarioLaboral,");
                query.AppendLine("    Horario = '	DATEPART(WEEKDAY,[Fecha Inicio]) = '+convert(varchar,NumeroDia)+' And '+");
                query.AppendLine("			    '		('+");
                query.AppendLine("			    '			SUBSTRING(CONVERT(varchar,[Fecha Inicio],121),12,8) < ' +Case When HoraInicioHorarioLaboral < 10 Then '''''0'+ convert(varchar,HoraInicioHorarioLaboral) Else ''''''+convert(varchar,HoraInicioHorarioLaboral) End  +':'+ Case When MinutoInicioHorarioLaboral < 10 Then '0'+ convert(varchar,MinutoInicioHorarioLaboral)+'''''' Else convert(varchar,MinutoInicioHorarioLaboral)+'''''' End + ' OR '+");
                query.AppendLine("			    '			SUBSTRING(CONVERT(varchar,[Fecha Inicio],121),12,8) > ' +Case When HoraFinHorarioLaboral < 10 Then '''''0'+ convert(varchar,HoraFinHorarioLaboral) Else ''''''+convert(varchar,HoraFinHorarioLaboral) End  +':'+ Case When MinutoFinHorarioLaboral < 10 Then '0'+ convert(varchar,MinutoFinHorarioLaboral) +''''''Else convert(varchar,MinutoFinHorarioLaboral)+'''''' End +");
                query.AppendLine("			    '		) Or '");
                query.AppendLine("From [" + DSODataContext.Schema + "].[vishistoricos('horariolaboral','horarios laborales Diarios','español')]");
                query.AppendLine("Where  dtinivigencia <> dtFinVigencia");
                query.AppendLine("And dtiniVigencia <= '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'");
                query.AppendLine("And dtFinVigencia >= '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'");
                query.AppendLine("order by vchCodigo ");

                DataTable dtHorario = DSODataAccess.Execute(query.ToString());
                List<string> listaAnds = new List<string>();

                if (dtHorario != null && dtHorario.Rows.Count > 0 && dtHorario.Columns.Count > 0)
                {
                    foreach (DataRow row in dtHorario.Rows)
                    {
                        listaAnds.Add(row["Horario"].ToString());
                    }

                    listaAnds[listaAnds.Count - 1] = listaAnds[listaAnds.Count - 1].Substring(0, listaAnds[listaAnds.Count - 1].Length - 3);
                }


                WhereHorario = "(";
                foreach (string horario in listaAnds)
                {
                    WhereHorario = WhereHorario + " " + horario;
                }
                WhereHorario = WhereHorario + " ) ";

                if (WhereHorario == "( ) ")
                {
                    WhereHorario = "";
                }

                //WhereHorario = "And (" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 2 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 3 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 4 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 5 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 6 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''15:00'')" +
                //    ") ";

                return WhereHorario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ValidarTipoReporte();
            ExportXLS(".xlsx");
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            cboUbicacion.SelectedValue = Session["Sitio"].ToString();
            cboTipoLlamada.SelectedValue = Session["TipoLlam"].ToString();
            cboTipoDestino.SelectedValue = Session["TipoDest"].ToString();
            cboCarrier.SelectedValue = Session["Carrier"].ToString();
            cboCriteriosDuracion.SelectedValue = Session["OperadorDuracion"].ToString();
            cboCriterioCosto.SelectedValue = Session["OperadorCosto"].ToString();

            pnlMapaNav.Visible = false;
        }

        protected void btnOkMail_Click(object sender, EventArgs e)
        {
            if(Page.IsValid)
            {
                GenerarPeticionDescarga();
            }
            else
            {
                mpeEtqMail.Show();
            }
        }

        protected void btnRegMail_Click(object sender, EventArgs e)
        {
            txtMail.Text = "";
            txtNombreRep.Text = "";
        }


        public static string ConsultaLaureateConsumoDetallado(bool llamadasfueradehorario)
        {
           
            string iCodUsuario = HttpContext.Current.Session["iCodUsuario"].ToString();
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("USE KEYTIA5");
            consulta.AppendLine($"declare @fechainicio varchar(20) = '{HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00"}'");
            consulta.AppendLine($"declare @fechafin varchar(20) = '{HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59"}'");
            consulta.AppendLine("declare @organizacion varchar(40)");
            consulta.AppendLine("declare @direccionLlamada varchar(40)");
            consulta.AppendLine("declare @fields varchar(8000)");
            consulta.AppendLine("declare @claveUsuario varchar(40)");
            consulta.AppendLine("declare @iCodCatUsuar int");

            //Validacion De Organizacion
            if(organizacion!= "0")
            {
                if (organizacion == "1") //UVM Todas las organizaciones
                {
                    consulta.AppendLine("set @claveUsuario = 'AdministracionUVM'");
                    consulta.AppendLine("set @organizacion = 'UVM'");

                }
                else 
                {
                    consulta.AppendLine("set @claveUsuario = 'AdministracionUNITEC'");
                    consulta.AppendLine("set @organizacion = 'Unitec'");


                }
                consulta.AppendLine("select @iCodCatUsuar = iCodCatalogo");
                consulta.AppendLine("from Laureate.[VisHistoricos('Usuar','Usuarios','Español')]");
                consulta.AppendLine("where vchcodigo = @claveUsuario");
                consulta.AppendLine("and dtfinvigencia>=getdate()");

            }
            else
            {
                consulta.AppendLine($"SET @icodcatUsuar = {iCodUsuario}");
                consulta.AppendLine("set @organizacion = 'TODAS'");

            }

            //Validacion de Dirección de LLamdas
            if (direccionLlamada == "2")
            {
                consulta.AppendLine($"set @direccionllamada = ' = ''SALIDA'' ' ");
                

            }
            else if (direccionLlamada == "1")
            {
                consulta.AppendLine($"set @direccionllamada = ' = ''ENTRADA'' ' ");
                
            }

            consulta.AppendLine($"set @fields = ");
            consulta.AppendLine($"'");
            if (llamadasfueradehorario)
            {
                consulta.AppendLine($"[Extension]	as ''Extensión'' , ");
                consulta.AppendLine($"[Colaborador]	as [Empleado] , ");
                consulta.AppendLine($"[Numero Marcado] AS ''Número Marcado'', ");
                consulta.AppendLine($"[Fecha] , ");
                consulta.AppendLine($"[Hora] , ");
                consulta.AppendLine($"[Duracion] as [Cantidad Minutos], ");
                consulta.AppendLine($"(Costo+CostoSM) as [Total],  ");
                consulta.AppendLine($"[Centro de costos] , ");
                consulta.AppendLine($"[Nombre Sitio]	as [Sitio]     ");

            }
            else
            {
                consulta.AppendLine($"[Centro de costos] , ");
                consulta.AppendLine($"[Colaborador]	 , ");
                consulta.AppendLine($"[Nomina] AS ''Nómina'', ");
                consulta.AppendLine($"[Extension]	as ''Extensión'' , ");
                consulta.AppendLine($"[Numero Marcado] AS ''Número Marcado'', ");
                consulta.AppendLine($"[Fecha] , ");
                consulta.AppendLine($"[Hora] , ");
                consulta.AppendLine($"[Fecha Fin], ");
                consulta.AppendLine($"[Hora Fin], ");
                consulta.AppendLine($"[Duracion] as [Cantidad minutos], ");
                consulta.AppendLine($"(Costo+CostoSM) as [Costo],  ");
                consulta.AppendLine($"[Nombre Localidad] as [Localidad],   ");
                consulta.AppendLine($"[Nombre Sitio]	as [Sitio] ,    ");
                consulta.AppendLine($"[Codigo Autorizacion] as [Codigo Autorizacion],   ");
                consulta.AppendLine($"[Nombre Carrier] as [Carrier],   ");
                consulta.AppendLine($"[Tipo de destino],   ");
                consulta.AppendLine($"[CategoriaLlamada] as [Dirección Llamada],   ");
                consulta.AppendLine($"'''+@organizacion+''' as Organización,  ");
                consulta.AppendLine($"Case when {BuscaHorario()} THEN ''SI'' ELSE ''NO'' end As EsFueraDeHorario ");
            }
            consulta.AppendLine($"'");



            consulta.AppendLine($"exec [LaureateRepConsumoDetallado]");
            consulta.AppendLine($"@Schema = 'Laureate', ");
            consulta.AppendLine($"@Fields=@fields, ");
            consulta.AppendLine($"@Usuario = @iCodCatUsuar, ");
            consulta.AppendLine($"@Perfil = 369,  ");
            consulta.AppendLine($"@VistaAUsar = 'CDR', ");
            consulta.AppendLine($"@Moneda = 'MXP', ");
            consulta.AppendLine($"@Idioma = 'Español', ");


            #region Filtros

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = ' = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ,");
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = ' = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ,");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = ' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ,");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = ' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = '= " + HttpContext.Current.Session["Carrier"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = ' = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = ' = " + HttpContext.Current.Session["IdLocali"].ToString() + "' ,");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("@NumeroTelf = ' =  ''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "'' ',");
                }
                else
                {
                    consulta.AppendLine("@NumeroTelf = ' LIKE ''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "%'' ',");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("@Extension = ' = ''" + HttpContext.Current.Session["Exten"].ToString() + "'' ' ,");
                }
                else
                {
                    consulta.AppendLine("@Extension = ' LIKE ''%" + HttpContext.Current.Session["Exten"].ToString() + "%'' ' ,");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = ' LIKE ''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'' ' ,");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("@CostoSim = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
                else
                {
                    consulta.AppendLine("@Costo = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = ' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " ' ,");
            }
            #endregion Filtro por Duracion

            #endregion



            consulta.AppendLine($"@FechaIniRep = @fechainicio,  ");
            consulta.AppendLine($"@FechaFinRep = @fechafin  ");

            if(direccionLlamada != "0")
            {
                consulta.AppendLine($",@CategoriaLlamada = @direccionllamada ");
            }

            if (llamadasfueradehorario)
            {
                consulta.AppendLine($",@Where2 = 'AND {BuscaHorario()}'");

            }


            return consulta.ToString();
        }


        public static string GeneraDBDataSourceLaurate(bool llamadasfueradehorario)
        {

            string iCodUsuario = HttpContext.Current.Session["iCodUsuario"].ToString();
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("USE KEYTIA5");
            consulta.AppendLine($"declare @fechainicio varchar(20) = ''''{HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00"}''''");
            consulta.AppendLine($"declare @fechafin varchar(20) = ''''{HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59"}''''");
            consulta.AppendLine("declare @organizacion varchar(40)");
            consulta.AppendLine("declare @direccionLlamada varchar(40)");
            consulta.AppendLine("declare @fields varchar(8000)");
            consulta.AppendLine("declare @claveUsuario varchar(40)");
            consulta.AppendLine("declare @iCodCatUsuar int");

            //Validacion De Organizacion
            if (organizacion != "0")
            {
                if (organizacion == "1") //UVM 
                {
                    consulta.AppendLine("set @claveUsuario = ''''AdministracionUVM''''");
                    consulta.AppendLine("set @organizacion = ''''''''''''UVM''''''''''''");

                }
                else
                {
                    consulta.AppendLine("set @claveUsuario = ''''AdministracionUNITEC''''");
                    consulta.AppendLine("set @organizacion = ''''''''''''Unitec''''''''''''");


                }
                consulta.AppendLine("select @iCodCatUsuar = iCodCatalogo");
                consulta.AppendLine("from Laureate.[VisHistoricos(''''Usuar'''',''''Usuarios'''',''''Español'''')]");
                consulta.AppendLine("where vchcodigo = @claveUsuario");
                consulta.AppendLine("and dtfinvigencia>=getdate()");

            }
            else //Todas las organizaciones
            {
                consulta.AppendLine($"SET @icodcatUsuar = {iCodUsuario}");
                consulta.AppendLine("set @organizacion = ''''''''''''TODAS''''''''''''");

            }

            //Validacion de Dirección de LLamdas
            if (direccionLlamada == "2")
            {
                consulta.AppendLine($"set @direccionllamada = '''' = ''''''''SALIDA''''''''''''");


            }
            else if (direccionLlamada == "1")
            {
                consulta.AppendLine($"set @direccionllamada = '''' = ''''''''ENTRADA''''''''''''");

            }
            consulta.AppendLine($"set @fields = ");
            consulta.AppendLine($"''''");

         
                consulta.AppendLine($"[Centro de costos] , ");
                consulta.AppendLine($"[Colaborador]	 , ");
                consulta.AppendLine($"[Nomina] AS [Nómina], ");
                consulta.AppendLine($"[Extension]	as [Extensión] , ");
                consulta.AppendLine($"[Numero Marcado] AS [Número Marcado], ");
                consulta.AppendLine($"[Fecha] , ");
                consulta.AppendLine($"[Hora] , ");
                consulta.AppendLine($"[Fecha Fin], ");
                consulta.AppendLine($"[Hora Fin], ");
                consulta.AppendLine($"[Duracion] as [Cantidad minutos], ");
                consulta.AppendLine($"(Costo+CostoSM) as [Costo],  ");
                consulta.AppendLine($"[Nombre Localidad] as [Localidad],   ");
                consulta.AppendLine($"[Nombre Sitio]	as [Sitio] ,    ");
                consulta.AppendLine($"[Codigo Autorizacion] as [Codigo Autorizacion],   ");
                consulta.AppendLine($"[Nombre Carrier] as [Carrier],   ");
                consulta.AppendLine($"[Tipo de destino],   ");
                consulta.AppendLine($"[CategoriaLlamada] as [Dirección Llamada],   ");
                consulta.AppendLine($"''''+@organizacion+'''' as Organización, ");
            consulta.AppendLine($"Case when {BuscaHorario().Replace("'", "''''")} THEN ''''''''SI'''''''' ELSE ''''''''NO'''''''' end As EsFueraDeHorario ");

            consulta.AppendLine($"''''");
            consulta.AppendLine($"exec [LaureateRepConsumoDetallado]");
            consulta.AppendLine($"@Schema = ''''Laureate'''', ");
            consulta.AppendLine($"@Fields=@fields, ");
            consulta.AppendLine($"@Usuario = @iCodCatUsuar, ");
            consulta.AppendLine($"@Perfil = 369,  ");
            consulta.AppendLine($"@VistaAUsar = ''''CDR'''', ");
            consulta.AppendLine($"@Moneda = ''''MXP'''', ");

            #region Filtros

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = '''' = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ,");
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = '''' = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ,");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = '''' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ,");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = '''' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = ''''= " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = '''' = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ,");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = '''' = " + HttpContext.Current.Session["IdLocali"].ToString() + "'''' ,");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("@NumeroTelf = '''' =  ''''''''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "'''''''' '''',");
                }
                else
                {
                    consulta.AppendLine("@NumeroTelf = '''' LIKE ''''''''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "%'''''''' '''',");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("@Extension = '''' = ''''''''" + HttpContext.Current.Session["Exten"].ToString() + "'''''''' '''' ,");
                }
                else
                {
                    consulta.AppendLine("@Extension = '''' LIKE ''''''''%" + HttpContext.Current.Session["Exten"].ToString() + "%'''''''' '''' ,");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = '''' LIKE ''''''''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'''''''' '''' ,");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("@CostoSim = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                }
                else
                {
                    consulta.AppendLine("@Costo = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = '''' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " '''' ,");
            }
            #endregion Filtro por Duracion

            #endregion





            consulta.AppendLine($"@Idioma = ''''Español'''' ");
           
            if (direccionLlamada != "0")
            {
                consulta.AppendLine($",@CategoriaLlamada = @direccionllamada ");
            }

            if (llamadasfueradehorario)
            {
                consulta.AppendLine($",@Where2 =  ''''AND {BuscaHorario().Replace("'","''''")}''''");

            }


            return consulta.ToString();
        }
        public static string GeneraDBDataSourceCountLaureate(bool llamadasfueradehorario)
        {
            string iCodUsuario = HttpContext.Current.Session["iCodUsuario"].ToString();
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("USE KEYTIA5");
            consulta.AppendLine($"declare @fechainicio varchar(20) = ''''{HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00"}''''");
            consulta.AppendLine($"declare @fechafin varchar(20) = ''''{HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59"}''''");
            consulta.AppendLine("declare @organizacion varchar(40)");
            consulta.AppendLine("declare @direccionLlamada varchar(40)");
            consulta.AppendLine("declare @claveUsuario varchar(40)");
            consulta.AppendLine("declare @iCodCatUsuar int");

            //Validacion De Organizacion
            if (organizacion != "0")
            {
                if (organizacion == "1") //UVM 
                {
                    consulta.AppendLine("set @claveUsuario = ''''AdministracionUVM''''");
                    consulta.AppendLine("set @organizacion = ''''''''''''UVM''''''''''''");

                }
                else
                {
                    consulta.AppendLine("set @claveUsuario = ''''AdministracionUNITEC''''");
                    consulta.AppendLine("set @organizacion = ''''''''''''Unitec''''''''''''");


                }
                consulta.AppendLine("select @iCodCatUsuar = iCodCatalogo");
                consulta.AppendLine("from Laureate.[VisHistoricos(''''Usuar'''',''''Usuarios'''',''''Español'''')]");
                consulta.AppendLine("where vchcodigo = @claveUsuario");
                consulta.AppendLine("and dtfinvigencia>=getdate()");

            }
            else //Todas las organizaciones
            {
                consulta.AppendLine($"SET @icodcatUsuar = {iCodUsuario}");
                consulta.AppendLine("set @organizacion = ''''''''''''TODAS''''''''''''");

            }

            //Validacion de Dirección de LLamdas
            if (direccionLlamada == "2")
            {
                consulta.AppendLine($"set @direccionllamada = '''' = ''''''''SALIDA''''''''''''");


            }
            else if (direccionLlamada == "1")
            {
                consulta.AppendLine($"set @direccionllamada = '''' = ''''''''SALIDA''''''''''''");

            }

            consulta.AppendLine($"exec [LaureateRepConsumoDetalladoCount]");
            consulta.AppendLine($"@Schema = ''''Laureate'''', ");
            consulta.AppendLine($"@Usuario = @iCodCatUsuar, ");
            consulta.AppendLine($"@Perfil = 369,  ");
            consulta.AppendLine($"@VistaAUsar = ''''CDR'''', ");
            consulta.AppendLine($"@Moneda = ''''MXP'''', ");
            consulta.AppendLine($"@Idioma = ''''Español'''', ");

            #region Filtros

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = '''' = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ,");
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = '''' = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ,");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = '''' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ,");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = '''' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = ''''= " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = '''' = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ,");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = '''' = " + HttpContext.Current.Session["IdLocali"].ToString() + "'''' ,");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("@NumeroTelf = '''' =  ''''''''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "'''''''' '''',");
                }
                else
                {
                    consulta.AppendLine("@NumeroTelf = '''' LIKE ''''''''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "%'''''''' '''',");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("@Extension = '''' = ''''''''" + HttpContext.Current.Session["Exten"].ToString() + "'''''''' '''' ,");
                }
                else
                {
                    consulta.AppendLine("@Extension = '''' LIKE ''''''''%" + HttpContext.Current.Session["Exten"].ToString() + "%'''''''' '''' ,");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = '''' LIKE ''''''''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'''''''' '''' ,");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("@CostoSim = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                }
                else
                {
                    consulta.AppendLine("@Costo = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = '''' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " '''' ,");
            }
            #endregion Filtro por Duracion    
            #endregion



            consulta.AppendLine($"@FechaIniRep = @fechainicio,  ");
            consulta.AppendLine($"@FechaFinRep = @fechafin  ");

            if (direccionLlamada != "0")
            {
                consulta.AppendLine($",@CategoriaLlamada = @direccionllamada ");
            }
            if (llamadasfueradehorario)
            {
                consulta.AppendLine($",@Where2 = ''''AND {BuscaHorario().Replace("'", "''''")}''''");

            }



            return consulta.ToString();
        }

        public static string ConsultaExisteNombreReporte(string nombreReporte)
        {
            string iCodUsuario = HttpContext.Current.Session["iCodUsuario"].ToString();
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("SELECT count(NombreReporte)");
            consulta.AppendLine("FROM " + DSODataContext.Schema + ".[vishistoricos('AtribCargaExportaDetalleCDR','Atributos de carga exporta detalleCDR','Español')]");
            consulta.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            consulta.AppendLine("AND Usuar = " + iCodUsuario);
            consulta.AppendLine("AND NombreReporte = '" + nombreReporte + "'");

            return consulta.ToString();
        }

        public static string GeneraDBDataSource(bool isReporteDetalleFactura)
        {

            StringBuilder consulta = new StringBuilder();

            if (!isReporteDetalleFactura)
            {
                consulta.AppendLine("exec [SPRepConsumoDetallado]");
                consulta.AppendLine("@Schema = ''''" + DSODataContext.Schema + "'''',  ");
                consulta.AppendLine("@Fields='''' ");
                consulta.AppendLine("[Centro de costos] , ");
                consulta.AppendLine("[Colaborador]	 , ");
                consulta.AppendLine("[Nomina],");
                consulta.AppendLine("[Extensión]	 , ");
                consulta.AppendLine("[Numero Marcado] as [Número Marcado] , ");
                consulta.AppendLine("[Fecha] , ");
                consulta.AppendLine("[Hora] , ");
                consulta.AppendLine("[Fecha Fin],");
                consulta.AppendLine("[Hora Fin],");
                consulta.AppendLine("[Duracion] as [Cantidad minutos], ");
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraSM"]) == 1)
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                    {
                        consulta.AppendLine("[Costo] = (CostoFac), ");
                        consulta.AppendLine("[Servicio Medido] =(CostoSM), ");
                    }
                    else
                    {
                        consulta.AppendLine("[Costo] = (Costo), ");
                        consulta.AppendLine("[Servicio Medido] =(CostoSM), ");
                    }
                }
                else
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                    {
                        consulta.AppendLine("[Costo] = (CostoFac+CostoSM), ");
                    }
                    else
                    {
                        consulta.AppendLine("[Costo] = (Costo+CostoSM), ");
                    }
                }
                consulta.AppendLine("[Nombre Localidad] as [Localidad], ");
                consulta.AppendLine("[Nombre Sitio]	as [Sitio] , ");
                consulta.AppendLine("[Codigo Autorizacion] as [Código Autorización] , ");
                consulta.AppendLine("[Nombre Carrier] as [Carrier], ");
                consulta.AppendLine("[Tipo de destino],  ");
                consulta.AppendLine("[Categoría],  ");
                if (DSODataContext.Schema.ToLower() == "k5banorte")
                {
                    consulta.AppendLine("[Etiqueta],  ");
                    consulta.AppendLine("[Puesto]  ");
                }
                else if (DSODataContext.Schema.ToLower() == "kuehnenagel")
                {
                    consulta.AppendLine("[Etiqueta],  ");
                    consulta.AppendLine("[IP] AS IVR");
                }
                else if (DSODataContext.Schema.ToLower() == "luxottica")
                {
                    consulta.AppendLine("[Etiqueta],  ");
                    consulta.AppendLine("[Circuito Entrada] As [Usuario Final] ");
                }
                else if (DSODataContext.Schema.ToLower() == "bat" || DSODataContext.Schema.ToLower() == "qualtia")
                {
                    consulta.AppendLine("[Etiqueta],  ");
                    consulta.AppendLine("[Organización]  ");
                }
                else if (DSODataContext.Schema.ToLower() == "conaculta")
                {
                    consulta.AppendLine("[Etiqueta],  ");
                    consulta.AppendLine("[Nombre GpoTroncal]  ");
                }
                else if (DSODataContext.Schema.ToLower() == "femsa" || DSODataContext.Schema.ToLower() == "kioc12")
                {
                    consulta.AppendLine("[Etiqueta],  ");
                    consulta.AppendLine("[Circuito Entrada],  ");
                    consulta.AppendLine("[Circuito Salida]  ");
                }
                else
                {
                    consulta.AppendLine("[Etiqueta]  ");
                }
                if(DSODataContext.Schema.ToLower() == "laureate")
                {
                    consulta.AppendLine(", TpLlam AS [Tipo de Llamada]");
                }
                consulta.AppendLine("'''',  ");
                consulta.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
                consulta.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
                consulta.AppendLine("@VistaAUsar = ''''" + tablaAUsuar + "'''',");  //Se agrega para saber de que tabla debe sair la información de fija.
                consulta.AppendLine("@Moneda = ''''" + HttpContext.Current.Session["Currency"] + "'''', ");


                #region Filtro por Sitio
                if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
                {
                    consulta.AppendLine("@Sitio = '''' = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ,");
                }
                #endregion Filtro por Sitio

                #region Filtro por Empleado
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
                {
                    consulta.AppendLine("@Emple = '''' = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ,");
                }
                #endregion Filtro por Nombre/Nomina

                #region Filtro por Centro de Costos
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
                {
                    consulta.AppendLine("@CenCos = '''' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ,");
                }
                #endregion Filtro por Centro de Costos

                #region Filtro por Tipo Llamada
                if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
                {
                    consulta.AppendLine("@GpoEtiqueta = '''' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ,");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Carrier
                if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
                {
                    consulta.AppendLine("@Carrier = ''''= " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ,");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Tipo Destino
                if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
                {
                    consulta.AppendLine("@TDest = '''' = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ,");
                }
                #endregion Filtro por Tipo Destino

                #region Filtro por Localidad

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
                {
                    consulta.AppendLine("@Locali = '''' = " + HttpContext.Current.Session["IdLocali"].ToString() + "'''' ,");
                }

                #endregion Filtro por Localidad

                #region Filtro por Num. Marcado

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
                {
                    if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                    {
                        consulta.AppendLine("@NumeroTelf = '''' =  ''''''''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "'''''''' '''',");
                    }
                    else
                    {
                        consulta.AppendLine("@NumeroTelf = '''' LIKE ''''''''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "%'''''''' '''',");
                    }
                }

                #endregion Filtro por Num. Marcado

                #region Filtro por Extensión

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
                {
                    if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                    {
                        consulta.AppendLine("@Extension = '''' = ''''''''" + HttpContext.Current.Session["Exten"].ToString() + "'''''''' '''' ,");
                    }
                    else
                    {
                        consulta.AppendLine("@Extension = '''' LIKE ''''''''%" + HttpContext.Current.Session["Exten"].ToString() + "%'''''''' '''' ,");
                    }
                }

                #endregion Filtro por Extensión

                #region Filtro por Codigo Autorizacion
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
                {
                    consulta.AppendLine("@CodAut = '''' LIKE ''''''''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'''''''' '''' ,");
                }
                #endregion Filtro por Codigo Autorizacion

                #region Filtro por Costo
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                    {
                        consulta.AppendLine("@CostoSim = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                    }
                    else
                    {
                        consulta.AppendLine("@Costo = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                    }
                }
                #endregion Filtro por Costo

                #region Filtro por Duracion
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
                {
                    consulta.AppendLine("@Duracion = '''' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " '''' ,");
                }
                #endregion Filtro por Duracion

                consulta.AppendLine("@Idioma = ''''" + HttpContext.Current.Session["Language"] + "''''");
            }
            else
            {
                /////////////////////////////
                consulta.AppendLine("DECLARE @Where varchar(max)= '''' 1 = 1 ''''   ");
                consulta.AppendLine("DECLARE @OrderInv varchar(max)  ");
                //consulta.Append("SELECT @Where = @Where + ''''[FechaInicio] >= ''''''''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'''''''' ");
                //consulta.AppendLine(" AND [FechaInicio] <= ''''''''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'''''''''''' ");

                #region Filtro por Sitio
                if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
                {
                    if (HttpContext.Current.Session["Sitio"].ToString().ToLower().Replace(" ", "").Contains("telcel"))
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND ([Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " OR [Tipo de destino] like ''''''''%" + HttpContext.Current.Session["Sitio"].ToString() + "%'''''''')'''' ");
                    }
                    else
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ");
                    }
                }
                #endregion Filtro por Sitio

                #region Filtro por Empleado
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Empleado] = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ");
                }
                #endregion Filtro por Nombre/Nomina

                #region Filtro por Centro de Costos
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Centro de Costos] = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ");
                }
                #endregion Filtro por Centro de Costos

                #region Filtro por Tipo Llamada
                if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Tipo Llamada] = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Carrier
                if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Carrier] = " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Tipo Destino
                if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Tipo Destino] = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ");
                }
                #endregion Filtro por Tipo Destino

                #region Filtro por Linea

                if (HttpContext.Current.Session["Linea"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Linea"].ToString()))
                {
                    if (Convert.ToBoolean(HttpContext.Current.Session["LineaExacta"]) == true)
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND [Linea] = ''''''''" + HttpContext.Current.Session["Linea"].ToString() + "'''''''' '''' ");
                    }
                    else
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND [Linea] LIKE ''''''''%" + HttpContext.Current.Session["Linea"].ToString() + "%'''''''' '''' ");
                    }
                }

                #endregion Filtro por Extensión

                #region Filtro por Costo
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND ([Costo]/[TipoCambio]) " + HttpContext.Current.Session["OperadorCosto"].ToString() + HttpContext.Current.Session["Costo"].ToString() + "'''' ");
                }
                #endregion Filtro por Costo

                #region Filtro por Duracion
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Duracion Minutos] " + HttpContext.Current.Session["OperadorDuracion"].ToString() + HttpContext.Current.Session["Duracion"].ToString() + " '''' ");
                }
                #endregion Filtro por Duracion

                consulta.AppendLine("exec [RepTabDetalladosMovil]");
                consulta.AppendLine("@Schema = ''''" + DSODataContext.Schema + "'''',  ");
                consulta.AppendLine("@Fields='''' ");
                consulta.AppendLine("[Nombre Centro de Costos]=upper(Convert(varchar,[Numero Centro de Costos])+'' - ''+[Nombre Centro de Costos]) , ");
                consulta.AppendLine("[Codigo Centro de Costos]	 , ");
                consulta.AppendLine("[Nombre Completo]=upper([Nombre Completo]),");
                consulta.AppendLine("[Linea]	 , ");
                consulta.AppendLine("[Fecha] , ");
                consulta.AppendLine("[Hora] , ");
                consulta.AppendLine("[Duracion] = [Duracion Minutos], ");
                consulta.AppendLine("[Total]=([Costo]/[TipoCambio]), ");
                consulta.AppendLine("[Nombre Sitio]=upper([Nombre Sitio]), ");
                consulta.AppendLine("[Nombre Tipo Destino]=upper([Nombre Tipo Destino]), ");
                consulta.AppendLine("[Nombre Carrier]=upper([Nombre Carrier]), ");

                if (DSODataContext.Schema.ToLower() == "bat" || DSODataContext.Schema.ToLower() == "qualtia")
                {
                    consulta.AppendLine("[Tipo Llamada]=upper([Tipo Llamada]),");
                    consulta.AppendLine("[Organización]  ");
                }
                else
                {
                    consulta.AppendLine("[Tipo Llamada]=upper([Tipo Llamada]) ");
                }

                consulta.AppendLine("'''',  ");
                consulta.AppendLine("   @Where = @Where,   ");
                consulta.AppendLine("   @Order = ''''[Total] Desc'''',  ");
                consulta.AppendLine("   @OrderInv = ''''[Total] Asc'''',");
                consulta.AppendLine("   @Lenght = " + maxRegistrosEnExcel + ",");
                consulta.AppendLine("   @OrderDir = ''''Asc'''',");
                consulta.AppendLine("   @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
                consulta.AppendLine("   @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
                consulta.AppendLine("   @Idioma = ''''" + HttpContext.Current.Session["Language"] + "''''");
                //consulta.AppendLine("   @FechaIniRep = ''''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'''', ");
                //consulta.AppendLine("   @FechaFinRep = ''''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'''' ");

                ////////////////////////////
            }

            return consulta.ToString();
        }

        public static string GeneraDBDataSourceCount(bool isReporteDetalleFactura)
        {

            StringBuilder consulta = new StringBuilder();

            if (!isReporteDetalleFactura)
            {
                consulta.AppendLine("exec [SPRepConsumoDetalladoCount]");
                consulta.AppendLine("@Schema = ''''" + DSODataContext.Schema + "'''',");
                consulta.AppendLine("   @MaxCount = 500001, ");
                consulta.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
                consulta.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
                consulta.AppendLine("@VistaAUsar = ''''" + tablaAUsuar + "'''',");  //Se agrega para saber de que tabla debe sair la información de fija.
                consulta.AppendLine("@Moneda = ''''" + HttpContext.Current.Session["Currency"] + "'''',");
                consulta.AppendLine("@Idioma = ''''" + HttpContext.Current.Session["Language"] + "'''',");


                #region Filtro por Sitio
                if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
                {
                    consulta.AppendLine("@Sitio = '''' = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ,");
                }
                #endregion Filtro por Sitio

                #region Filtro por Empleado
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
                {
                    consulta.AppendLine("@Emple = '''' = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ,");
                }
                #endregion Filtro por Nombre/Nomina

                #region Filtro por Centro de Costos
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
                {
                    consulta.AppendLine("@CenCos = '''' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ,");
                }
                #endregion Filtro por Centro de Costos

                #region Filtro por Tipo Llamada
                if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
                {
                    consulta.AppendLine("@GpoEtiqueta = '''' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ,");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Carrier
                if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
                {
                    consulta.AppendLine("@Carrier = ''''= " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ,");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Tipo Destino
                if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
                {
                    consulta.AppendLine("@TDest = '''' = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ,");
                }
                #endregion Filtro por Tipo Destino

                #region Filtro por Localidad

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
                {
                    consulta.AppendLine("@Locali = '''' = " + HttpContext.Current.Session["IdLocali"].ToString() + "'''' ,");
                }

                #endregion Filtro por Localidad

                #region Filtro por Num. Marcado

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
                {
                    if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                    {
                        consulta.AppendLine("@NumeroTelf = '''' =  ''''''''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "'''''''' '''',");
                    }
                    else
                    {
                        consulta.AppendLine("@NumeroTelf = '''' LIKE ''''''''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("''''", "") + "%'''''''' '''',");
                    }
                }

                #endregion Filtro por Num. Marcado

                #region Filtro por Extensión

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
                {
                    if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                    {
                        consulta.AppendLine("@Extension = '''' = ''''''''" + HttpContext.Current.Session["Exten"].ToString() + "'''''''' '''' ,");
                    }
                    else
                    {
                        consulta.AppendLine("@Extension = '''' LIKE ''''''''%" + HttpContext.Current.Session["Exten"].ToString() + "%'''''''' '''' ,");
                    }
                }

                #endregion Filtro por Extensión

                #region Filtro por Codigo Autorizacion
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
                {
                    consulta.AppendLine("@CodAut = '''' LIKE ''''''''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'''''''' '''' ,");
                }
                #endregion Filtro por Codigo Autorizacion

                #region Filtro por Costo
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                    {
                        consulta.AppendLine("@CostoSim = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                    }
                    else
                    {
                        consulta.AppendLine("@Costo = '''' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "'''' ,");
                    }
                }
                #endregion Filtro por Costo

                #region Filtro por Duracion
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
                {
                    consulta.AppendLine("@Duracion = '''' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " '''' ,");
                }
                #endregion Filtro por Duracion    
                consulta.AppendLine("@FechaIniRep = ''''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'''', ");
                consulta.AppendLine("@FechaFinRep = ''''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'''' ");
            }
            else
            {
                consulta.AppendLine("DECLARE @Where varchar(max)= ''''''''   ");
                consulta.Append("SELECT @Where = @Where + ''''[FechaInicio] >= ''''''''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'''''''' ");
                consulta.AppendLine(" AND [FechaInicio] <= ''''''''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'''''''''''' ");

                #region Filtro por Sitio
                if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
                {
                    if (HttpContext.Current.Session["Sitio"].ToString().ToLower().Replace(" ", "").Contains("telcel"))
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND ([Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " OR [Tipo de destino] like ''''''''%" + HttpContext.Current.Session["Sitio"].ToString() + "%'''''''')'''' ");
                    }
                    else
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Sitio] = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ");
                    }
                }
                #endregion Filtro por Sitio

                #region Filtro por Empleado
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Empleado] = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ");
                }
                #endregion Filtro por Nombre/Nomina

                #region Filtro por Centro de Costos
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Centro de Costos] = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ");
                }
                #endregion Filtro por Centro de Costos

                #region Filtro por Tipo Llamada
                if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Tipo Llamada] = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Carrier
                if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Carrier] = " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ");
                }
                #endregion Filtro por Tipo Llamada

                #region Filtro por Tipo Destino
                if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Codigo Tipo Destino] = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ");
                }
                #endregion Filtro por Tipo Destino

                #region Filtro por Linea

                if (HttpContext.Current.Session["Linea"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Linea"].ToString()))
                {
                    if (Convert.ToBoolean(HttpContext.Current.Session["LineaExacta"]) == true)
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND [Linea] = ''''''''" + HttpContext.Current.Session["Linea"].ToString() + "'''''''' '''' ");
                    }
                    else
                    {
                        consulta.AppendLine("SELECT @Where = @Where + '''' AND [Linea] LIKE ''''''''%" + HttpContext.Current.Session["Linea"].ToString() + "%'''''''' '''' ");
                    }
                }

                #endregion Filtro por Extensión

                #region Filtro por Costo
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND ([Costo]/[TipoCambio]) " + HttpContext.Current.Session["OperadorCosto"].ToString() + HttpContext.Current.Session["Costo"].ToString() + "'''' ");
                }
                #endregion Filtro por Costo

                
                #region Filtro por Duracion
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
                {
                    consulta.AppendLine("SELECT @Where = @Where + '''' AND [Duracion Minutos] " + HttpContext.Current.Session["OperadorDuracion"].ToString() + HttpContext.Current.Session["Duracion"].ToString() + " '''' ");
                }
                #endregion Filtro por Duracion

                consulta.AppendLine("exec [RepTabDetalladosMovilCount]   ");
                consulta.AppendLine("@Schema = ''''" + DSODataContext.Schema + "'''',  ");
                consulta.AppendLine("   @Where = @Where,   ");
                consulta.AppendLine("   @MaxCount = " + maxRegistrosEnExcel + ",");
                consulta.AppendLine("   @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
                consulta.AppendLine("   @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
                consulta.AppendLine("   @Idioma = ''''" + HttpContext.Current.Session["Language"] + "'''', ");
                consulta.AppendLine("   @FechaIniRep = ''''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'''', ");
                consulta.AppendLine("   @FechaFinRep = ''''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'''' ");
            }
           
            return consulta.ToString();
        }

        private void GenerarPeticionDescarga()
        {
            StringBuilder Query = new StringBuilder();
            string Esquema = DSODataContext.Schema;
            string iCodUsuario = HttpContext.Current.Session["iCodUsuario"].ToString();
            string Sitio = "NULL";
            string Emple = "NULL";
            string CenCos = "NULL";
            string GpoEtiqueta = "NULL";
            string Carrier = "NULL";
            string TDest = "NULL";
            string Locali = "NULL";
            string FechaInicioRep = HttpContext.Current.Session["FechaInicio"].ToString();
            string FechaFinRep = HttpContext.Current.Session["FechaFin"].ToString();
            string DBDataSource = string.Empty;
            string DBDataSourceCount = string.Empty;
            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                DBDataSource = GeneraDBDataSourceLaurate(SoloLlamadasFueraDeHorario).TrimEnd(',');
                DBDataSourceCount = GeneraDBDataSourceCountLaureate(SoloLlamadasFueraDeHorario).TrimEnd(',');
            }
            else
            {
                DBDataSource = GeneraDBDataSource(isReporteDetalleFactura).TrimEnd(',');
                DBDataSourceCount = GeneraDBDataSourceCount(isReporteDetalleFactura).TrimEnd(',');
            }
            
             
            string Email = txtMail.Text;
            string NombreReporte = txtNombreRep.Text;
            string NumeroTelf = "NULL";
            string Extension = "NULL";
            string CodAut = "NULL";
            string FiltroCostoParaDetalle = "NULL";
            string FiltroDuracionParaDetalle = "NULL";

            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {

                Sitio = HttpContext.Current.Session["Sitio"].ToString();

            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                Emple = HttpContext.Current.Session["IdEmple"].ToString();
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                CenCos = HttpContext.Current.Session["IdCenCos"].ToString();
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                GpoEtiqueta = HttpContext.Current.Session["TipoLlam"].ToString();
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                Carrier = HttpContext.Current.Session["Carrier"].ToString();
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                TDest = HttpContext.Current.Session["TipoDest"].ToString();
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                Locali = HttpContext.Current.Session["IdLocali"].ToString();
            }
            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    NumeroTelf = "''" + HttpContext.Current.Session["NumMarcado"].ToString() + " (Búsqueda exacta)''";
                }
                else
                {
                    NumeroTelf = "''" + HttpContext.Current.Session["NumMarcado"].ToString() + "''";
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    Extension = "''" + HttpContext.Current.Session["Exten"].ToString() + " (Búsqueda exacta)''";
                }
                else
                {
                    Extension = "''" + HttpContext.Current.Session["Exten"].ToString() + "''";
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                CodAut = "''" + HttpContext.Current.Session["CodAuto"].ToString() + "''";
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                FiltroCostoParaDetalle = "''" + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "''";
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                FiltroDuracionParaDetalle = "''" + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + "''";
            }
            #endregion Filtro por Duracion

            Query.AppendLine("exec SPCargaExportaDetalleCDR @Esquema = '" + Esquema + "',");
            Query.AppendLine("@iCodUsuario = '" + iCodUsuario + "',");
            Query.AppendLine("@Sitio = '" + Sitio + "',");
            Query.AppendLine("@Emple = '" + Emple + "',");
            Query.AppendLine("@CenCos = '" + CenCos + "',");
            Query.AppendLine("@GpoEtiqueta = '" + GpoEtiqueta + "',");
            Query.AppendLine("@Carrier = '" + Carrier + "',");
            Query.AppendLine("@TDest = '" + TDest + "',");
            Query.AppendLine("@Locali = '" + Locali + "',");
            Query.AppendLine("@FechaInicioRep = '" + FechaInicioRep + " 00:00:00.000',");
            Query.AppendLine("@FechaFinRep = '" + FechaFinRep + " 23:59:59.000',");
            Query.AppendLine("@DBDataSource = '" + DBDataSource + "',");
            Query.AppendLine("@DBDataSourceCount = '" + DBDataSourceCount + "',");
            Query.AppendLine("@Email = '" + Email + "',");
            Query.AppendLine("@NumeroTelf = '" + NumeroTelf + "',");
            Query.AppendLine("@Extension = '" + Extension + "',");
            Query.AppendLine("@CodAut = '" + CodAut + "',");
            Query.AppendLine("@FiltroCostoParaDetalle = '" + FiltroCostoParaDetalle + "',");
            Query.AppendLine("@FiltroDuracionParaDetalle = '" + FiltroDuracionParaDetalle + "',");
            Query.AppendLine("@NombreReporte = '" + NombreReporte + "'");

            txtMail.Text = "";
            txtNombreRep.Text = "";

            bool val = DSODataAccess.ExecuteNonQuery(Query.ToString());

            if (val)
            {
                lblTituloModalMsn.Text = "Solicitud registrada";
                lblBodyModalMsn.Text = "La consulta está en proceso, recibiras el resultado en el correo electronico " + Email + ".";
                mpeEtqMsn.Show();
            }
            else
            {
                lblTituloModalMsn.Text = "Error al registrar la solicitud";
                lblBodyModalMsn.Text = "Ocurrio un error al registrar la solicitud, por favor intenta de nuevo.";
                mpeEtqMsn.Show();
            }
        }

        public static string ConsultaDetalleCDR()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("exec [SPRepConsumoDetallado]");
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("[Centro de costos] , ");
            consulta.AppendLine("[Colaborador]	 , ");
            consulta.AppendLine("[Nomina],");
            consulta.AppendLine("[Extensión]	 , ");
            consulta.AppendLine("[Numero Marcado] , ");
            consulta.AppendLine("[Fecha] , ");
            consulta.AppendLine("[Hora] , ");
            consulta.AppendLine("[Fecha Fin],");
            consulta.AppendLine("[Hora Fin],");
            consulta.AppendLine("[Duracion] , ");
            consulta.AppendLine("[TotalSimulado] = (CostoFac+CostoSM), ");
            consulta.AppendLine("[TotalReal] = (Costo+CostoSM), ");
            consulta.AppendLine("[CostoSimulado] = (CostoFac), ");
            consulta.AppendLine("[CostoReal] = (Costo), ");
            consulta.AppendLine("[SM] =(CostoSM), ");
            consulta.AppendLine("[Nombre Localidad] as [Localidad], ");
            consulta.AppendLine("[Nombre Sitio]	as [Sitio] , ");
            consulta.AppendLine("[Codigo Autorizacion] , ");
            consulta.AppendLine("[Nombre Carrier] as [Carrier], ");
            consulta.AppendLine("[Tipo de destino],  ");
            consulta.AppendLine("[Categoría],  ");

            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Puesto]  ");
            }
            else if (DSODataContext.Schema.ToLower() == "kuehnenagel")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[IP] AS IVR");
            }
            else if (DSODataContext.Schema.ToLower() == "luxottica")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Circuito Entrada]  ");
            }
            else if (DSODataContext.Schema.ToLower() == "bat" || DSODataContext.Schema.ToLower() == "qualtia")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Organización]  ");
            }
            else if (DSODataContext.Schema.ToLower() == "conaculta")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Nombre GpoTroncal]  ");
            }
            else if (DSODataContext.Schema.ToLower() == "femsa" || DSODataContext.Schema.ToLower() == "kioc12")
            {
                consulta.AppendLine("[Etiqueta],  ");
                consulta.AppendLine("[Circuito Entrada],  ");
                consulta.AppendLine("[Circuito Salida]  ");
            }
            else
            {
                consulta.AppendLine("[Etiqueta]  ");
            }
            consulta.AppendLine("',  ");
            consulta.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine("@VistaAUsar = '" + tablaAUsuar + "',");  //Se agrega para saber de que tabla debe sair la información de fija.
            consulta.AppendLine("@Moneda = '" + HttpContext.Current.Session["Currency"] + "', ");
            consulta.AppendLine("@Idioma = '" + HttpContext.Current.Session["Language"] + "', ");


            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = ' = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ,");
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = ' = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ,");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = ' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ,");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = ' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = '= " + HttpContext.Current.Session["Carrier"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = ' = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = ' = " + HttpContext.Current.Session["IdLocali"].ToString() + "' ,");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("@NumeroTelf = ' =  ''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "'' ',");
                }
                else
                {
                    consulta.AppendLine("@NumeroTelf = ' LIKE ''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "%'' ',");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("@Extension = ' = ''" + HttpContext.Current.Session["Exten"].ToString() + "'' ' ,");
                }
                else
                {
                    consulta.AppendLine("@Extension = ' LIKE ''%" + HttpContext.Current.Session["Exten"].ToString() + "%'' ' ,");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = ' LIKE ''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'' ' ,");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("@CostoSim = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
                else
                {
                    consulta.AppendLine("@Costo = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = ' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " ' ,");
            }
            #endregion Filtro por Duracion

            consulta.AppendLine("@FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00', ");
            consulta.AppendLine("@FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59' ");

            return consulta.ToString();
        }

        private string ConsultaCountQueryDetalleCDR()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("exec [SPRepConsumoDetalladoCount]");
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("   @MaxCount = " + (maxRegistrosEnExcel + 1).ToString() + ", ");
            consulta.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine("@VistaAUsar = '" + tablaAUsuar + "',");  //Se agrega para saber de que tabla debe sair la información de fija.
            consulta.AppendLine("@Moneda = '" + HttpContext.Current.Session["Currency"] + "', ");
            consulta.AppendLine("@Idioma = '" + HttpContext.Current.Session["Language"] + "', ");


            #region Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = ' = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ,");
            }
            #endregion Filtro por Sitio

            #region Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = ' = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ,");
            }
            #endregion Filtro por Nombre/Nomina

            #region Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = ' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ,");
            }
            #endregion Filtro por Centro de Costos

            #region Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = ' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = '= " + HttpContext.Current.Session["Carrier"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Llamada

            #region Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = ' = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ,");
            }
            #endregion Filtro por Tipo Destino

            #region Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = ' = " + HttpContext.Current.Session["IdLocali"].ToString() + "' ,");
            }

            #endregion Filtro por Localidad

            #region Filtro por Num. Marcado

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NumMarcado"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["NumMarcadoExacto"]) == true)
                {
                    consulta.AppendLine("@NumeroTelf = ' =  ''" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "'' ',");
                }
                else
                {
                    consulta.AppendLine("@NumeroTelf = ' LIKE ''%" + HttpContext.Current.Session["NumMarcado"].ToString().Replace("'", "") + "%'' ',");
                }
            }

            #endregion Filtro por Num. Marcado

            #region Filtro por Extensión

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Exten"].ToString()))
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ExtenExacta"]) == true)
                {
                    consulta.AppendLine("@Extension = ' = ''" + HttpContext.Current.Session["Exten"].ToString() + "'' ' ,");
                }
                else
                {
                    consulta.AppendLine("@Extension = ' LIKE ''%" + HttpContext.Current.Session["Exten"].ToString() + "%'' ' ,");
                }
            }

            #endregion Filtro por Extensión

            #region Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = ' LIKE ''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'' ' ,");
            }
            #endregion Filtro por Codigo Autorizacion

            #region Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    consulta.AppendLine("@CostoSim = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
                else
                {
                    consulta.AppendLine("@Costo = ' " + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "' ,");
                }
            }
            #endregion Filtro por Costo

            #region Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = ' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " ' ,");
            }
            #endregion Filtro por Duracion

            consulta.AppendLine("@FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00', ");
            

            consulta.AppendLine("@FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59' ");

            return consulta.ToString();
        }

        protected void NombreRepValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int existeNombreReporte = Convert.ToUInt16((object)DSODataAccess.ExecuteScalar(ConsultaExisteNombreReporte(txtNombreRep.Text)));

            if (existeNombreReporte != 0)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;               
            }
        }


    }
}
