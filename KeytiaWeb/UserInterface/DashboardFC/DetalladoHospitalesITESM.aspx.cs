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
    public partial class DetalladoHospitalesITESM : System.Web.UI.Page
    {
        #region Campos
        //Laureate
        static string organizacion = "0";
        static string direccionLlamada = "0";

        //20190808 RM Se crea diccionario para guardar parametros  de fija y movil
        static Dictionary<string, string> param = new Dictionary<string, string>();
        static bool SoloLlamadasFueraDeHorario = false;

        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        //Variables que guardan valores de queryString

        static DataTable RepDetallado = new DataTable();

        static int maxRegistrosEnWeb = 1000; //Limite de registros que se presentarán en web
        StringBuilder consultaCbo = new StringBuilder();
        static int maxRegistrosEnExcel = 1000;
        string nomResumenCDR = string.Empty;
        static string tablaAUsuar = tablaAUsuar = "CDR";

        #endregion

        #region Events
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
                        try
                        {
                            //Inicia los valores default de los controles
                            //de fecha y banderas de clientes y perfiles
                            CalculaFechasDeDashboard();

                            EstablecerBanderasClientePerfil();
                        }
                        catch (Exception ex)
                        {
                            throw new KeytiaWebException(
                                "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                                + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                        }
                    }

                }


                //FD 18/03/2021 SOLO SE MUESTRAN LOS PANELES PARA EL ESQUEMA LAUREATE
                if (DSODataContext.Schema.ToLower() == "laureate")
                {
                    rowdirLlamada.Visible = true;
                    rowOrganizacion.Visible = true;
                    rowLlamadasFueraDeHorario.Visible = true;
                }

                //Oculta controles que no se ocupan en formulario
                OcultaControles();

                //LlenarDropDownList(); //ITESM

                LeeQueryString();

                AplicarOpcionTelefonia();

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

        protected void rbtnFija_OnCheckedChanged(Object sender, EventArgs args)
        {
            ValidarTelefoniasAMostrar();
            LlenarDropDownList();
            OcultrarMostrarControles();
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS(".xlsx");
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            LlenarDatosFormulario();

            RepDetallado.Clear();

            
            {
                //20161109 NZ Se agrega consulta del count que resultara en el detallado
                //para advertir al usuario si se podra exportar la información o no.
                //double countConsulta = 
                //    Convert.ToDouble((object)DSODataAccess.ExecuteScalar(ConsultaCountQueryDetalleCDR()));


                //if (countConsulta != 0)
                {
                    //if (countConsulta <= maxRegistrosEnExcel)
                    //{
                    //    if (countConsulta <= maxRegistrosEnWeb)
                    //    {
                    //        ReporteDetallado(Rep9, "Detalle");

                    //        Rep0.Visible = false;
                    //        Rep9.Visible = true;
                    //        pnlMapaNav.Visible = true;

                    //        List<MapNav> listaNavegacion = new List<MapNav>();
                    //        listaNavegacion.Add(new MapNav() { Titulo = "Filtro", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                    //        listaNavegacion.Add(new MapNav() { Titulo = "Detalle", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                    //        pnlMapaNavegacion.Controls.Clear();
                    //        pnlMapaNavegacion.Controls.Add(DTIChartsAndControls.MapaNavegacion(listaNavegacion));
                    //    }
                    //    else { ExportXLS(".xlsx"); }
                    //}
                    //else
                    {
                        mpeEtqMail.Show();
                    }
                }
                //else
                //{
                //    lblTituloModalMsn.Text = "No hay datos para mostrar";
                //    lblBodyModalMsn.Text = "La consulta no produjo ningún resultado.";
                //    mpeEtqMsn.Show();
                //}
            }

        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            cboHospital.SelectedValue = Session["Hospital"].ToString();

            //cboUbicacion.SelectedValue = Session["Sitio"].ToString();
            //cboTipoLlamada.SelectedValue = Session["TipoLlam"].ToString();
            //cboTipoDestino.SelectedValue = Session["TipoDest"].ToString();
            //cboCarrier.SelectedValue = Session["Carrier"].ToString();
            //cboCriteriosDuracion.SelectedValue = Session["OperadorDuracion"].ToString();
            //cboCriterioCosto.SelectedValue = Session["OperadorCosto"].ToString();

            pnlMapaNav.Visible = false;
        }

        protected void btnOkMail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
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

        #endregion


        #region Configuracion pagina

        private void OcultaControles()
        {
            lblUbicacion.Visible = false;
            cboUbicacion.Visible = false;

            lblEmpleado.Visible = false;
            txtNombre.Visible = false;

            lblCentroCostos.Visible = false;
            txtCenCos.Visible = false;

            lblTipoLlamada.Visible = false;
            cboTipoLlamada.Visible = false;

            lblCarrier.Visible = false;
            cboCarrier.Visible = false;

            lblTipoDestino.Visible = false;
            cboTipoDestino.Visible = false;

            lblLocalidad.Visible = false;
            txtLocali.Visible = false;

            lblNumMarcado.Visible = false;
            txtNumMarcado.Visible = false;
            banderaNumMarcado.Visible = false;

            lblExtension.Visible = false;
            txtExtension.Visible = false;
            banderaExtensionExacta.Visible = false;

            lblCodigo.Visible = false;
            txtCodigo.Visible = false;

            lblCosto.Visible = false;
            cboCriterioCosto.Visible = false;
            txtCosto.Visible = false;

            lblDuracion.Visible = false;
            cboCriteriosDuracion.Visible = false;
            txtDuracion.Visible = false;

        }
        private void CalculaFechasDeDashboard()
        {
            
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
                else
                {
                    // Si en CDR no hay informacion entonces los valores de las fechas se calculan con los
                    // valores default de las variables fechaInicio y fechaFinal
                    if (fechaFinal.Day == 1)
                    {
                        fechaInicio = fechaInicio.AddMonths(-1);
                        fechaFinal = fechaFinal.AddDays(-1);
                    }
                }
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
        }



        private void EstablecerBanderasClientePerfil()
        {
            //Fijas para ITESM
            Session["MuestraSM"] = 0;
            Session["MuestraCostoSimulado"] = 0;
            Session["InfoCDRTablasIndependientes"] = 0;
        }

        private void ValidarTelefoniasAMostrar()
        {
            //Fijo para ITESM
            rbtnFija.Text = "Fija";
            rbtnFijaEntrada.Visible = false;
            rbtnFijaEnlace.Visible = false;
        }

        private void OcultrarMostrarControles()
        {
            //Fijo para ITESM
            rowLocali.Visible = true;
            rowNumMarcado.Visible = true;
            rowExten.Visible = true;
            rowCodAuto.Visible = true;
            rowLinea.Visible = false;
            rowCatLlam.Visible = true;
            rowTDest.Visible = true;
        }

        private void LlenarDropDownList()
        {
            //Fijo para ITESM
            LlenarDropDownListFiltrosCDR();
        }

        private void LlenarDropDownListFiltrosCDR() //Fija en General
        {
            DataTable hospitales = DSODataAccess.Execute(ConsultaObtenerHospitalesITESMCDR());
            hospitales = DTIChartsAndControls.ordenaTabla(hospitales, "Hospital ASC");
            /*DataRow rowHospital = hospitales.NewRow();
            rowHospital["iCodCatalogo"] = 0;
            rowHospital["Hospital"] = "--TODOS--";
            rowHospital["Condicion"] = string.Empty;
            hospitales.Rows.InsertAt(rowHospital, 0);*/

            cboHospital.DataSource = hospitales.DefaultView;
            cboHospital.DataValueField = "Condicion";
            cboHospital.DataTextField = "Hospital";
            cboHospital.DataBind();
            

            //DataTable ubicacion = DSODataAccess.Execute(ConsultaObtenerSitiosCDR());
            //ubicacion = DTIChartsAndControls.ordenaTabla(ubicacion, "Sitio ASC");
            //DataRow rowUbicacion = ubicacion.NewRow();
            //rowUbicacion["iCodCatalogo"] = 0;
            //rowUbicacion["Sitio"] = "--TODOS--";

            //ubicacion.Rows.InsertAt(rowUbicacion, 0);
            //cboUbicacion.DataSource = ubicacion.DefaultView;
            //cboUbicacion.DataValueField = "iCodCatalogo";
            //cboUbicacion.DataTextField = "Sitio";
            //cboUbicacion.DataBind();


            //DataTable tipoLlamada = DSODataAccess.Execute(ConsultaObtenerTipoLlamada());
            //tipoLlamada = DTIChartsAndControls.ordenaTabla(tipoLlamada, "TipoLlamada ASC");
            //DataRow rowTipoLlamada = tipoLlamada.NewRow();
            //rowTipoLlamada["GEtiqueta"] = -1;
            //rowTipoLlamada["TipoLlamada"] = "--TODAS--";

            //tipoLlamada.Rows.InsertAt(rowTipoLlamada, 0);
            //cboTipoLlamada.DataSource = tipoLlamada.DefaultView;
            //cboTipoLlamada.DataValueField = "GEtiqueta";
            //cboTipoLlamada.DataTextField = "TipoLlamada";
            //cboTipoLlamada.DataBind();

            //DataTable carrier = DSODataAccess.Execute(ConsultaObtenerCarrierCDR());
            //carrier = DTIChartsAndControls.ordenaTabla(carrier, "Carrier ASC");
            //DataRow rowCarrier = carrier.NewRow();
            //rowCarrier["iCodCatalogo"] = 0;
            //rowCarrier["Carrier"] = "--TODOS--";

            //carrier.Rows.InsertAt(rowCarrier, 0);
            //cboCarrier.DataSource = carrier.DefaultView;
            //cboCarrier.DataValueField = "iCodCatalogo";
            //cboCarrier.DataTextField = "Carrier";
            //cboCarrier.DataBind();


            //DataTable tipoDestino = DSODataAccess.Execute(ConsultaObtenerTipoDestinoCDR());
            //tipoDestino = DTIChartsAndControls.ordenaTabla(tipoDestino, "TipoDestino ASC");
            //DataRow rowTipoDestino = tipoDestino.NewRow();
            //rowTipoDestino["iCodCatalogo"] = 0;
            //rowTipoDestino["TipoDestino"] = "--TODOS--";

            //tipoDestino.Rows.InsertAt(rowTipoDestino, 0);
            //cboTipoDestino.DataSource = tipoDestino.DefaultView;
            //cboTipoDestino.DataValueField = "iCodCatalogo";
            //cboTipoDestino.DataTextField = "TipoDestino";
            //cboTipoDestino.DataBind();
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
                //Fijo para ITESM
                rbtnFija.Checked = true;
                rbtnMovil.Checked = false;
                rbtnFija_OnCheckedChanged(rbtnFija, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Configuracion pagina


        #region Consultas a BD

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

        private string ConsultaObtenerHospitalesITESMCDR()
        {
            consultaCbo.Length = 0;
            consultaCbo.AppendLine("	SELECT Hosp.iCodCatalogo, Hospital=UPPER(Hosp.Descripcion), RepUsuVal as Condicion ");
            consultaCbo.AppendLine("	FROM " + DSODataContext.Schema + ".[vishistoricos('HospitalITESM','Hospitales ITESM','Español')] Hosp");
            consultaCbo.AppendLine("	WHERE Hosp.dtIniVigencia <> Hosp.dtFinVigencia");
            consultaCbo.AppendLine("    And Hosp.dtFinVigencia >= getdate() ");

            return consultaCbo.ToString();
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
            consultaCbo.Length = 0;
            consultaCbo.AppendLine("SELECT iCodCatalogo, Carrier=UPPER(vchDescripcion)");
            consultaCbo.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Carrier','Carriers','Español')] Carrier");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	JOIN (SELECT Carrier");
            consultaCbo.AppendLine("          FROM " + DSODataContext.Schema + "." + "[VisAcumulados('AcumDia','ResumenCDR','Español')]");
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

            if (DSODataContext.Schema.ToLower() == "fca")
                condicionParticular = " where convert(date, FechaInicio) >= '2020-12-01' "; //En FCA a partir de esta fecha están los tipos destino válidos.

            consultaCbo.Length = 0;
            consultaCbo.AppendLine("SELECT iCodCatalogo, TipoDestino=UPPER(Español)");
            consultaCbo.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')] TDest");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("	JOIN (SELECT TDest");
            consultaCbo.AppendLine("          FROM " + DSODataContext.Schema + "." + "[VisAcumulados('AcumDia','ResumenCDR','Español')]");
            if (!string.IsNullOrEmpty(condicionParticular))
                consultaCbo.AppendLine(condicionParticular);
            consultaCbo.AppendLine("          GROUP BY TDest");
            consultaCbo.AppendLine("        ) AS CDR");
            consultaCbo.AppendLine("		ON TDest.iCodCatalogo = CDR.TDest");
            consultaCbo.AppendLine("");
            consultaCbo.AppendLine("WHERE TDest.dtIniVigencia <> TDest.dtFinVigencia");
            consultaCbo.AppendLine("    AND TDest.dtFinVigencia >= GETDATE() AND CatTDestDesc = 'Fija'");
            return consultaCbo.ToString();
        }


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

                return WhereHorario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public static string GeneraDBDataSource()
        {

            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("exec Keytia5..SPRepConsumoDetalladoITESM  ");
            consulta.AppendLine(" @Schema = ''''" + DSODataContext.Schema + "'''',  ");
            consulta.AppendLine(" @Fields = ''''[Fecha] , [Hora] ,[Extensión], [Numero Marcado], [Nombre Localidad] as [Localidad], [Duracion], [Total] = (Costo+CostoSM), Timbrado, [Tipo de destino], Resultado,OrigReason, DestReason, [Centro de costos] , [Colaborador], [Nombre Sitio]as [Sitio]'''',  ");
            consulta.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine(" @VistaAUsar = ''''CDR'''',");
            consulta.AppendLine(" @Moneda = ''''" + HttpContext.Current.Session["Currency"] + "'''', ");

            //Filtro por Hospital
            if (HttpContext.Current.Session["Hospital"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Hospital"].ToString()) && HttpContext.Current.Session["Hospital"].ToString() != "0")
            {
                consulta.AppendLine(" @LikeSearch = '''' " + HttpContext.Current.Session["Hospital"].ToString() + " '''',  ");
            }


            /*
            //Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = '''' = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ,");
            }

            //Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = '''' = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ,");
            }

            //Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = '''' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ,");
            }

            //Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = '''' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ,");
            }

            //Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = ''''= " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ,");
            }

            //Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = '''' = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ,");
            }

            //Filtro por Localidad
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = '''' = " + HttpContext.Current.Session["IdLocali"].ToString() + "'''' ,");
            }

            //Filtro por Num. Marcado
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

            //Filtro por Extensión
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

            //Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = '''' LIKE ''''''''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'''''''' '''' ,");
            }

            //Filtro por Costo
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

            //Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = '''' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " '''' ,");
            }
            */
            consulta.AppendLine("@Idioma = ''''Español''''");

            return consulta.ToString();
        }

        public static string GeneraDBDataSourceCount()
        {

            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("exec Keytia5..SPRepConsumoDetalladoCountITESM  ");
            consulta.AppendLine(" @Schema = ''''" + DSODataContext.Schema + "'''',  ");
            consulta.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine(" @VistaAUsar = ''''CDR'''',   ");
            consulta.AppendLine(" @Moneda = ''''" + HttpContext.Current.Session["Currency"] + "'''', ");
            consulta.AppendLine(" @Idioma = ''''" + HttpContext.Current.Session["Language"] + "'''',");

            //Filtro por Hospital
            if (HttpContext.Current.Session["Hospital"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Hospital"].ToString()) && HttpContext.Current.Session["Hospital"].ToString() != "0")
            {
                consulta.AppendLine(" @LikeSearch = '''' " + HttpContext.Current.Session["Hospital"].ToString() + " '''',  ");
            }

            /*
            //Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = '''' = " + HttpContext.Current.Session["Sitio"].ToString() + " '''' ,");
            }

            //Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = '''' = " + HttpContext.Current.Session["IdEmple"].ToString() + "'''' ,");
            }

            //Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = '''' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "'''' ,");
            }

            //Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = '''' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "'''' ,");
            }

            //Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = ''''= " + HttpContext.Current.Session["Carrier"].ToString() + "'''' ,");
            }

            //Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = '''' = " + HttpContext.Current.Session["TipoDest"].ToString() + "'''' ,");
            }

            //Filtro por Localidad
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = '''' = " + HttpContext.Current.Session["IdLocali"].ToString() + "'''' ,");
            }

            //Filtro por Num. Marcado
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

            //Filtro por Extensión
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

            //Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = '''' LIKE ''''''''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'''''''' '''' ,");
            }

            //Filtro por Costo
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

            //Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = '''' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " '''' ,");
            }
            */

            consulta.AppendLine("@FechaIniRep = ''''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'''', ");
            consulta.AppendLine("@FechaFinRep = ''''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'''' ");

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


            DBDataSource = GeneraDBDataSource().TrimEnd(',');
            DBDataSourceCount = GeneraDBDataSourceCount().TrimEnd(',');



            string Email = txtMail.Text;
            string NombreReporte = txtNombreRep.Text;
            string NumeroTelf = "NULL";
            string Extension = "NULL";
            string CodAut = "NULL";
            string FiltroCostoParaDetalle = "NULL";
            string FiltroDuracionParaDetalle = "NULL";

            //Filtro por Hospital
            //if (HttpContext.Current.Session["Hospital"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Hospital"].ToString()) && HttpContext.Current.Session["Hospital"].ToString() != "0")
            //{

            //    Extension = HttpContext.Current.Session["Hospital"].ToString();

            //}

            /*
            //Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {

                Sitio = HttpContext.Current.Session["Sitio"].ToString();

            }

            //Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                Emple = HttpContext.Current.Session["IdEmple"].ToString();
            }

            //Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                CenCos = HttpContext.Current.Session["IdCenCos"].ToString();
            }

            //Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                GpoEtiqueta = HttpContext.Current.Session["TipoLlam"].ToString();
            }

            //Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                Carrier = HttpContext.Current.Session["Carrier"].ToString();
            }

            //Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                TDest = HttpContext.Current.Session["TipoDest"].ToString();
            }

            //Filtro por Localidad

            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                Locali = HttpContext.Current.Session["IdLocali"].ToString();
            }

            //Filtro por Num. Marcado
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

            //Filtro por Extensión
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

            //Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                CodAut = "''" + HttpContext.Current.Session["CodAuto"].ToString() + "''";
            }

            //Filtro por Costo
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Costo"].ToString()))
            {
                FiltroCostoParaDetalle = "''" + HttpContext.Current.Session["OperadorCosto"].ToString() + " " + HttpContext.Current.Session["Costo"].ToString() + "''";
            }

            //Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                FiltroDuracionParaDetalle = "''" + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + "''";
            }
            */
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
            consulta.AppendLine("[Etiqueta]  ");
            consulta.AppendLine("',  ");
            consulta.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ", ");
            consulta.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ", ");
            consulta.AppendLine("@VistaAUsar = '" + tablaAUsuar + "',");  //Se agrega para saber de que tabla debe sair la información de fija.
            consulta.AppendLine("@Moneda = '" + HttpContext.Current.Session["Currency"] + "', ");
            consulta.AppendLine("@Idioma = '" + HttpContext.Current.Session["Language"] + "', ");

            //Filtro por Hospital
            if (HttpContext.Current.Session["Hospital"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Hospital"].ToString()) && HttpContext.Current.Session["Hospital"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = ' = " + HttpContext.Current.Session["Hospital"].ToString() + " ' ,");
            }

            /*
            //Filtro por Sitio
            if (HttpContext.Current.Session["Sitio"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Sitio"].ToString()) && HttpContext.Current.Session["Sitio"].ToString() != "0")
            {
                consulta.AppendLine("@Sitio = ' = " + HttpContext.Current.Session["Sitio"].ToString() + " ' ,");
            }

            //Filtro por Empleado
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreEmple"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdEmple"].ToString()))
            {
                consulta.AppendLine("@Emple = ' = " + HttpContext.Current.Session["IdEmple"].ToString() + "' ,");
            }

            //Filtro por Centro de Costos
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreCenCos"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdCenCos"].ToString()))
            {
                consulta.AppendLine("@CenCos = ' = " + HttpContext.Current.Session["IdCenCos"].ToString() + "' ,");
            }

            //Filtro por Tipo Llamada
            if (HttpContext.Current.Session["TipoLlam"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoLlam"].ToString()) && HttpContext.Current.Session["TipoLlam"].ToString() != "-1")
            {
                consulta.AppendLine("@GpoEtiqueta = ' = " + HttpContext.Current.Session["TipoLlam"].ToString() + "' ,");
            }

            //Filtro por Carrier
            if (HttpContext.Current.Session["Carrier"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Carrier"].ToString()) && HttpContext.Current.Session["Carrier"].ToString() != "0")
            {
                consulta.AppendLine("@Carrier = '= " + HttpContext.Current.Session["Carrier"].ToString() + "' ,");
            }

            //Filtro por Tipo Destino
            if (HttpContext.Current.Session["TipoDest"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["TipoDest"].ToString()) && HttpContext.Current.Session["TipoDest"].ToString() != "0")
            {
                consulta.AppendLine("@TDest = ' = " + HttpContext.Current.Session["TipoDest"].ToString() + "' ,");
            }

            //Filtro por Localidad
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["NombreLocali"].ToString()) && !string.IsNullOrEmpty(HttpContext.Current.Session["IdLocali"].ToString()))
            {
                consulta.AppendLine("@Locali = ' = " + HttpContext.Current.Session["IdLocali"].ToString() + "' ,");
            }

            //Filtro por Num. Marcado
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

            //Filtro por Extensión
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

            //Filtro por Codigo Autorizacion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["CodAuto"].ToString()))
            {
                consulta.AppendLine("@CodAut = ' LIKE ''%" + HttpContext.Current.Session["CodAuto"].ToString() + "%'' ' ,");
            }

            //Filtro por Costo
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

            //Filtro por Duracion
            if (!string.IsNullOrEmpty(HttpContext.Current.Session["Duracion"].ToString()))
            {
                consulta.AppendLine("@Duracion = ' " + HttpContext.Current.Session["OperadorDuracion"].ToString() + " " + HttpContext.Current.Session["Duracion"].ToString() + " ' ,");
            }
            */

            consulta.AppendLine("@FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00', ");
            consulta.AppendLine("@FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59' ");

            return consulta.ToString();
        }

        protected void NombreRepValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int existeNombreReporte =
                Convert.ToUInt16((object)DSODataAccess.ExecuteScalar(ConsultaExisteNombreReporte(txtNombreRep.Text)));

            if (existeNombreReporte != 0)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        #endregion Consultas a BD


        #region WebMethods

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

        [WebMethod]
        public static object ReporteDetalladoWebM()
        {
            RepDetallado.Clear();
            RepDetallado = DSODataAccess.Execute(ConsultaDetalleCDR());

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

                int indicePuesto = Convert.ToInt32(columnasVisibles.Last());
                Array.Resize<int>(ref columnasVisibles, columnasVisibles.Count() + 1);
                columnasVisibles[columnasVisibles.Length - 1] = indicePuesto + 1;

                for (int i = columnasNoVisibles.Length - 1; i >= 0; i--)
                {
                    RepDetallado.Columns.RemoveAt(columnasNoVisibles[i]);
                }

                return FCAndControls.ConvertToJSONStringDetalle(RepDetallado);
            }
            else { return null; }
        }

        #endregion


        #region Reporte Detallado

        private void LlenarDatosFormulario()
        {
            string nomIni = "ctl00$cphContent$";
            Session["Hospital"] = Request.Form.Get(nomIni + "cboHospital");

            //Session["Sitio"] = Request.Form.Get(nomIni + "cboUbicacion");
            //Session["TipoLlam"] = Request.Form.Get(nomIni + "cboTipoLlamada");
            //Session["TipoDest"] = Request.Form.Get(nomIni + "cboTipoDestino");
            //Session["Carrier"] = Request.Form.Get(nomIni + "cboCarrier");
            //Session["OperadorDuracion"] = Request.Form.Get(nomIni + "cboCriteriosDuracion");
            //Session["OperadorCosto"] = Request.Form.Get(nomIni + "cboCriterioCosto");

            //Session["NombreEmple"] = txtNombre.Text;
            //Session["IdEmple"] = txtEmpleId.Text;
            //Session["NombreCenCos"] = txtCenCos.Text;
            //Session["IdCenCos"] = txtCenCosId.Text;
            //Session["NombreLocali"] = txtLocali.Text;
            //Session["IdLocali"] = txtLocaliId.Text;
            //Session["NumMarcado"] = txtNumMarcado.Text;
            //Session["NumMarcadoExacto"] = banderaNumMarcado.Checked;
            //Session["CodAuto"] = txtCodigo.Text;
            //Session["Exten"] = txtExtension.Text;
            //Session["ExtenExacta"] = banderaExtensionExacta.Checked;
            //Session["Duracion"] = txtDuracion.Text;
            //Session["Costo"] = txtCosto.Text;
            //Session["Linea"] = txtLinea.Text;
            //Session["LineaExacta"] = banderaLineaExacta.Checked;
        }

        private void ReporteDetallado(Control contenedor, string tituloGrid)
        {
            //NZ Crea el contenedor de la tabla.
            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepDetalleSoloTabla("ReporteDetallado", tituloGrid));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionDetalleFija", "<script language=javascript> GetDatosTabla('ReporteDetallado', 'ReporteDetalladoWebM'); </script>", false);
        }

        public static ArrayList FormatoColumRepDetallado(DataTable ldt, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            //Columnas a mostrar
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

            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);

            return valores;
        }
        
        #endregion


        #region Exportacion

        public void ExportXLS(string tipoExtensionArchivo)
        {
            CrearXLS(tipoExtensionArchivo);
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                RepDetallado.Clear();


                RepDetallado = DSODataAccess.Execute(ConsultaDetalleCDR());
                RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                //Elimina columnas no necesarias en el gridview
                if (RepDetallado.Columns.Contains("RID"))
                    RepDetallado.Columns.Remove("RID");
                if (RepDetallado.Columns.Contains("RowNumber"))
                    RepDetallado.Columns.Remove("RowNumber");
                if (RepDetallado.Columns.Contains("TopRID"))
                    RepDetallado.Columns.Remove("TopRID");


                //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                //Cambiar nombre de campos particulares
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

                RepDetallado = ElimColDeAcuerdoABanClientePerfil(RepDetallado);
                RepDetallado.Columns["Total"].ColumnName = "Costo"; //NZ El nombre de la columna Total se cambia por Costo
                

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");

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
                Tabla.Columns.Remove("TotalSimulado");
                Tabla.Columns.Remove("TotalReal");
                Tabla.Columns.Remove("CostoSimulado");
                Tabla.Columns["CostoReal"].ColumnName = "Total";
                Tabla.Columns["SM"].ColumnName = "Servicio Medido";
            }
            else
            {
                Tabla.Columns.Remove("TotalSimulado");
                Tabla.Columns.Remove("CostoSimulado");
                Tabla.Columns.Remove("CostoReal");
                Tabla.Columns.Remove("SM");
                Tabla.Columns["TotalReal"].ColumnName = "Total";
            }

            return Tabla;
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion

    }
}