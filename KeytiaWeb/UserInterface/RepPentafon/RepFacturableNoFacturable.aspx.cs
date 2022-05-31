using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepPentafon
{
    public partial class RepFacturableNoFacturable : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string fechaInicio = "";
        static string fechaFinal = "";
        static string fechaFac = "";
        static string colFijas = "";
        static string colFijas1 = "";
        decimal totalFacturable;
        decimal totalNoFacturable;
        static List<CarriersFijo> carriers = new List<CarriersFijo>();
        static List<MatrizImportes> matriz = new List<MatrizImportes>();
        static List<ImporteTotalVariable> colImportes = new List<ImporteTotalVariable>();
        static List<ImporteTotalCarriers> ImportesCarriers = new List<ImporteTotalCarriers>();
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
                IniciaProceso();
                ObtieneFechaFact();
                ObtieneImportesCampaña();
                LlenaCampania();
                GeneraColumnasFijosCarriers();
            }

        }
        private void ObtieneFechaFact()
        {
            DataTable dt = DSODataAccess.Execute(ObtieneFechas(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                ValidaSelectCombo(dr["Mes"].ToString(), cboMes);
                fechaInicio = dr["FechaInicio"].ToString() + " 00:00:00";
                fechaFinal = dr["FechaFin"].ToString() + " 23:59:59";
            }
        }
        private void ObtieneImportesCampaña()
        {
            matriz.Clear();

            DataTable dt = DSODataAccess.Execute(ObtieneDatosRepFacturable(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                matriz = (from DataRow dr in dt.Rows
                          select new MatrizImportes()
                          {
                              Carrier = dr["CARRIER"].ToString(),
                              Campaña = dr["CAMPAÑA"].ToString(),
                              Ibound = Convert.ToDecimal(dr["INBOUND"]),
                              Outbound = Convert.ToDecimal(dr["OUTBOUND"]),
                              Buzon = Convert.ToDecimal(dr["BUZONES"]),
                              SmsInterno = Convert.ToDecimal(dr["SMSInterno"]),
                              SmsEspecial = Convert.ToDecimal(dr["SMSEspecial"]),
                              Internet = Convert.ToDecimal(dr["INTERNET"]),
                              L2l = Convert.ToDecimal(dr["L2L"]),
                              Ho = Convert.ToDecimal(dr["HO"]),
                              Mpls = Convert.ToDecimal(dr["MPLS"]),
                              OtrosSer = Convert.ToDecimal(dr["OTROSSERV"]),
                              Mail = Convert.ToDecimal(dr["MAIL"]),
                              Total = Convert.ToDecimal(dr["TOTAL"])
                          }).ToList();
            }
        }
        private void LlenaCampania()
        {
            var campanias = matriz.GroupBy(x => x.Campaña).ToList().OrderBy(x => x.Key);
            lstBoxCampanias.Items.Clear();
            foreach (var item in campanias)
            {
                lstBoxCampanias.Items.Add(item.Key);
            }
        }
        private void IniciaProceso()
        {
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
        private DataTable GetDataDropDownList(string clave)
        {
            ObtieneAnio();
            StringBuilder query = new StringBuilder();            
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");
            query = query.Replace("[CAMPOS]", "CASE WHEN LEN(VCHCODIGO) = 1 THEN '0' + VCHCODIGO ELSE VCHCODIGO END AS vchCodigo, UPPER(Español) AS Descripcion");
            query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
            return DSODataAccess.Execute(query.ToString());
        }
        public void ObtieneAnio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo AS vchCodigo, vchDescripcion AS Descripcion FROM " + esquema + ".[VisHistoricos('Anio','Años','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion IN(DATEPART(YEAR, GETDATE()),DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())), DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())))");
            query.AppendLine(" ORDER BY vchDescripcion DESC");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                cboAnio.DataSource = dt;
                cboAnio.DataBind();
            }
        }
        private void ValidaSelectCombo(string valor, DropDownList cbo)
        {
            string itemToCompare = string.Empty;
            string itemOrigin = valor.ToUpper();
            foreach (ListItem item in cbo.Items)
            {
                itemToCompare = item.Text.ToUpper();
                if (itemOrigin == itemToCompare)
                {
                    cbo.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        private string ObtieneFechas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @FechaFin DATE");
            query.AppendLine(" SELECT");
            query.AppendLine(" @FechaFin = MAX(FechaInicio)");
            query.AppendLine(" FROM " + esquema + ".[VisAcumulados('AcumDia','ResumenCDR','Español')] AS A WITH(NOLOCK)");
            query.AppendLine(" SELECT");
            query.AppendLine(" ISNULL(CONVERT(VARCHAR, CONVERT(DATE, DATEADD(mm, DATEDIFF(mm, 0, @FechaFin), 0))), '')AS FechaInicio,");
            query.AppendLine(" ISNULL(CONVERT(VARCHAR, CONVERT(DATE, CONVERT(VARCHAR(25), DATEADD(dd, -(DAY(DATEADD(mm, 1, @FechaFin))), DATEADD(mm, 1, @FechaFin)), 101))), '') AS FechaFin,");
            query.AppendLine(" ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaFin), N'Y', 'es-MX')), '') AS PeriodoFac,");
            query.AppendLine(" ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaFin), N'MMMM', 'es-MX')), '') AS Mes,");
            query.AppendLine(" ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaFin), N'yyyy', 'es-MX')), '') AS Anio");
            return query.ToString();
        }
        protected void cboMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculaFechas();
            ObtieneImportesCampaña();
            LlenaCampania();
            GeneraColumnasFijosCarriers();
        }
        private void CalculaFechas()
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
            DateTime F = Convert.ToDateTime((anio + "-" + mes + "-" + "01"));
            DateTime fecha2 = F.AddMonths(1).AddDays(-1);
            fechaInicio = F.ToString("yyyy-MM-dd 00:00:00");
            fechaFinal = fecha2.ToString("yyyy-MM-dd 23:59:59");

        }
        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            ColFijasCarriers(colFijas, colFijas1);

            if (lstBoxCampanias.SelectedItem != null)
            {
                colImportes.Clear();/*lista con los importes por carrier para la columna de totales*/
                foreach (ListItem li in lstBoxCampanias.Items)
                {
                    ImportesCarriers.Clear();

                    if (li.Selected == true)
                    {
                        GeneraCuerpoTabla(li.Text.ToString().Trim());
                    }
                }

                GeneraColumnasTotales();
            }
        }
        private void GeneraColumnasFijosCarriers()
        {
            StringBuilder html = new StringBuilder();
            StringBuilder html1 = new StringBuilder();
            colFijas = "";
            colFijas1 = "";
            carriers.Clear();
            var listCarr = matriz.Where(x => x.Carrier != "MAIL" && x.Carrier != "ENVIOSMS" && x.Carrier != "SMARTTELCO");
            var listCarrier = matriz.GroupBy(x => x.Carrier).ToList();
            foreach (var item in listCarrier)
            {
                CarriersFijo carrier = new CarriersFijo
                {
                    Carrier = item.Key
                };
                carriers.Add(carrier);

                if (item.Key != "MAIL" && item.Key != "ENVIOSMS" && item.Key != "SMARTTELCO")
                {
                    html.AppendLine(" <tr style='border: 1px solid #CCCCCC;'>");
                    html.AppendLine(" <td> " + item.Key + " </td>"); ;
                    html.AppendLine(" </tr>");
                }
                html1.AppendLine(" <tr style='border: 1px solid #CCCCCC;'>");
                html1.AppendLine(" <td> " + item.Key + " </td>"); ;
                html1.AppendLine(" </tr>");

            }

            colFijas = html.ToString();
            colFijas1 = html1.ToString();
            ColFijasCarriers(html.ToString(), html1.ToString());

        }
        private void ColFijasCarriers(string html = "", string html1 = "")
        {
            CarriersEnlace.Controls.Add(new Literal { Text = html });
            CarriersHO.Controls.Add(new Literal { Text = html });
            CarriersServAdic.Controls.Add(new Literal { Text = html });
            CarriersTotales.Controls.Add(new Literal { Text = html1.ToString() });

        }
        private void GeneraColumnasTotales()
        {
            string totGeneral = TotGeneral();
            string totGenVoz = GeneraTotalGeneral("background-color:#EEEEEE;");
            string totSms = GeneraHtmlTot(colImportes.Where(x => x.Carrier == "ENVIOSMS" && x.Tipo == "ENVIOSMS").ToList().Sum(x => x.Importe));
            string totEmail = GeneraHtmlTot(colImportes.Where(x => x.Carrier == "MAIL" && x.Tipo == "MAIL").ToList().Sum(x => x.Importe));
            string totGenEnlace = GeneraTotalesEnlacesOtrosSerHO("ENLACE");//GeneraTotalEnlace();
            string totGenHO = GeneraTotalesEnlacesOtrosSerHO("HO");//GeneraTotalHO();
            string totGenServAdic = GeneraTotalesEnlacesOtrosSerHO("OTROSSERV");//GeneraTotaleServAdic();
            string totalGeneralCamp = GeneraTotalGeneralCamp();

            string totalcarrier = GeneraTotalCarriers();

            TotEmail.Controls.Add(new Literal { Text = totEmail });
            TotSMS.Controls.Add(new Literal { Text = totSms });
            TotGeneralVoz.Controls.Add(new Literal { Text = totGenVoz });
            TotalGeneral.Controls.Add(new Literal { Text = totGeneral });
            TotalGeneralServAdic.Controls.Add(new Literal { Text = totGenServAdic });
            TotGeneralHO.Controls.Add(new Literal { Text = totGenHO });
            TotalGeneralEnlace.Controls.Add(new Literal { Text = totGenEnlace });
            TotalGeneralCamp.Controls.Add(new Literal { Text = totalGeneralCamp });

            TotalGeneralCarriers.Controls.Add(new Literal { Text = totalcarrier });
        }
        private void GeneraCuerpoTabla(string campaña)
        {
            string campanias = "";
            string facturable = "";
            string datosFacturable = "";
            string datosSms = "";
            string datosMail = "";
            string datosEnlaces = "";
            string datosHO = "";
            string datosServAdic = "";
            string totales = "";
            string carriersTotales = "";
            string sumFactNofactCamp = "";

            campanias += GeneraHtmlEncabezadosCampañas(campaña);
            facturable += GeneraHtmlFacturableNoFacturable();
            datosFacturable += GeneraHtmlDatosFacturableNoFacturable(campaña);
            datosSms += GeneraHtmlSMS(campaña);/*SMS*/
            datosMail += GeneraHtmlMail(campaña);/*MAIL*/
            datosEnlaces += DatosEnlaces(campaña);
            datosHO += DatosHO(campaña);
            datosServAdic += DatosServAdici(campaña);
            totales += DatosTotales(totalFacturable, totalNoFacturable);/*calcular los totales por cada campaña que se selecciono*/
            carriersTotales += GeneraCarriersTotales();
            sumFactNofactCamp += GeneraSumaFacNoFacCamp();


            Campanias.Controls.Add(new Literal { Text = campanias });
            FacturableNoFacturable.Controls.Add(new Literal { Text = facturable });
            datosFacturableNoFacturable.Controls.Add(new Literal { Text = datosFacturable });
            datosFacturableNoFacturableSMS.Controls.Add(new Literal { Text = datosSms });
            datosFacturableNoFacturableMail.Controls.Add(new Literal { Text = datosMail });
            DatosFacturaEnlaces.Controls.Add(new Literal { Text = datosEnlaces });
            DatosFacturaHo.Controls.Add(new Literal { Text = datosHO });
            DatosServAdic.Controls.Add(new Literal { Text = datosServAdic });
            Datostotales.Controls.Add(new Literal { Text = totales });
            TotalCarriers.Controls.Add(new Literal { Text = carriersTotales });
            SumaFactNoFactCamp.Controls.Add(new Literal { Text = sumFactNofactCamp });
        }
        private string GeneraHtmlEncabezadosCampañas(string campaña)
        {
            StringBuilder html = new StringBuilder();

            html.AppendLine(" <td colspan='2' style='text-align:center;padding:9px;border:1px solid #CCCCCC;background-color:#EEEEEE;'> ");
            html.AppendLine(" " + campaña + " ");
            html.AppendLine(" </td> ");
            return html.ToString();
        }
        private string GeneraHtmlFacturableNoFacturable()
        {
            StringBuilder html = new StringBuilder();

            html.AppendLine(" <td style='text-align:center;padding:4px;border:1px solid #CCCCCC;background-color:#EEEEEE;'>Facturable</td> ");
            html.AppendLine(" <td style='text-align:center;padding:4px;border:1px solid #CCCCCC;background-color:#EEEEEE;'>No Facturable</td> ");
            return html.ToString();
        }
        private string GeneraHtmlDatosFacturableNoFacturable(string campaña)
        {
            List<ImportesCarriers> importes = new List<ImportesCarriers>();

            totalNoFacturable = 0;
            totalFacturable = 0;

            string html = "";
            decimal bestelInboundFacturable = 0;
            decimal marcatelInboundFacturable = 0;
            decimal protelInboundFacturable = 0;
            decimal alestraInboundFacturable = 0;

            decimal bestelInboundNoFacturable = 0;
            decimal marcatelInboundNoFacturable = 0;
            decimal protelInboundNoFacturable = 0;
            decimal alestraInboundNoFacturable = 0;

            decimal bestelOutboundFacturable = 0;
            decimal marcatelOutboundFacturable = 0;
            decimal protelOutboundFacturable = 0;
            decimal alestraOutboundFacturable = 0;

            decimal bestelOutboundNoFacturable = 0;
            decimal marcatelOutboundNoFacturable = 0;
            decimal protelOutboundNoFacturable = 0;
            decimal alestraOutboundNoFacturable = 0;

            decimal buzonNoFacturable = 0;
            decimal buzonFacturable = 0;

            var lista = matriz.Where(x => x.Campaña == campaña);
            if (lista != null && lista.Count() > 0)
            {
                #region inbound
                if (campaña != "WALMART MG")
                {
                    bestelInboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "BESTEL").Ibound : 0;
                    marcatelInboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Ibound : 0;
                    protelInboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "PROTEL").Ibound : 0;
                    alestraInboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Ibound : 0;

                    colImportes.Add(new ImporteTotalVariable { Tipo = "INBOUND", Carrier = "PROTEL", Importe = protelInboundNoFacturable });

                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "INBOUND", Carrier = "PROTEL", Importe = protelInboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                }
                else
                {
                    bestelInboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "BESTEL").Ibound : 0;
                    marcatelInboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Ibound : 0;
                    protelInboundNoFacturable = 0; //Solo aplica para Walmart FG
                    protelInboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "PROTEL").Ibound : 0;
                    alestraInboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Ibound : 0;

                    colImportes.Add(new ImporteTotalVariable { Tipo = "INBOUND", Carrier = "PROTEL", Importe = protelInboundFacturable });

                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "INBOUND", Carrier = "PROTEL", Importe = protelInboundFacturable, FactNoFact = "FACTURABLE" });
                }


                colImportes.Add(new ImporteTotalVariable { Tipo = "INBOUND", Carrier = "BESTEL", Importe = bestelInboundNoFacturable });
                colImportes.Add(new ImporteTotalVariable { Tipo = "INBOUND", Carrier = "MARCATEL", Importe = marcatelInboundNoFacturable });
                
                colImportes.Add(new ImporteTotalVariable { Tipo = "INBOUND", Carrier = "ALESTRA", Importe = alestraInboundNoFacturable });

                /*LISTA TOTALES POR CARRIER*/
                ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "INBOUND", Carrier = "BESTEL", Importe = bestelInboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "INBOUND", Carrier = "MARCATEL", Importe = marcatelInboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                
                ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "INBOUND", Carrier = "ALESTRA", Importe = alestraInboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                #endregion



                #region outbound
                if (campaña == "SUBURBIA" || campaña == "LIVERPOOL - MT")
                {
                    decimal gastoFijo = (campaña == "SUBURBIA") ? ((70 * 2) * 2800) : ((91 * 2) * 2800);
                    decimal importeTotal = 0;

                    importeTotal += (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "BESTEL").Outbound : 0;
                    importeTotal += (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Outbound : 0;
                    importeTotal += (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "PROTEL").Outbound : 0;
                    importeTotal += (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Outbound : 0;

                    if (importeTotal > gastoFijo)
                    {
                        /*se calcula el Importe facturable*/
                        bestelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? ((lista.FirstOrDefault(x => x.Carrier == "BESTEL").Outbound) / importeTotal) * gastoFijo : 0;
                        marcatelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? ((lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Outbound) / importeTotal) * gastoFijo : 0;
                        protelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? ((lista.FirstOrDefault(x => x.Carrier == "PROTEL").Outbound) / importeTotal) * gastoFijo : 0;
                        alestraOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? ((lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Outbound) / importeTotal) * gastoFijo : 0;

                        /*se calcula el Importe No Facturable*/
                        bestelOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? (lista.FirstOrDefault(x => x.Carrier == "BESTEL").Outbound - bestelOutboundFacturable) : 0;
                        marcatelOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? (lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Outbound - marcatelOutboundFacturable) : 0;
                        protelOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? (lista.FirstOrDefault(x => x.Carrier == "PROTEL").Outbound - protelOutboundFacturable) : 0;
                        alestraOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? (lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Outbound - alestraOutboundFacturable) : 0;

                    }
                    else
                    {
                        /*se calcula el Importe facturable*/
                        bestelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "BESTEL").Outbound : 0;
                        marcatelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Outbound : 0;
                        protelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "PROTEL").Outbound : 0;
                        alestraOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Outbound : 0;

                    }


                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundFacturable });

                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundNoFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundNoFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundNoFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundNoFacturable });

                    /*LISTA TOTALES POR CARRIER*/
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundFacturable, FactNoFact = "FACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundFacturable, FactNoFact = "FACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundFacturable, FactNoFact = "FACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundFacturable, FactNoFact = "FACTURABLE" });

                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                }
                else if (campaña == "AXA SEGUROS" || campaña == "SEPHORA-SEPHORA" || campaña == "METLIFE")
                {
                    bestelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "BESTEL").Outbound : 0;
                    marcatelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Outbound : 0;
                    protelOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "PROTEL").Outbound : 0;
                    alestraOutboundFacturable = (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Outbound : 0;

                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundFacturable });

                    /*LISTA TOTALES POR CARRIER*/
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundFacturable, FactNoFact = "FACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundFacturable, FactNoFact = "FACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundFacturable, FactNoFact = "FACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundFacturable, FactNoFact = "FACTURABLE" });

                }
                else
                {
                    bestelOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "BESTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "BESTEL").Outbound : 0;
                    marcatelOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "MARCATEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MARCATEL").Outbound : 0;
                    protelOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "PROTEL") != null) ? lista.FirstOrDefault(x => x.Carrier == "PROTEL").Outbound : 0;
                    alestraOutboundNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "ALESTRA") != null) ? lista.FirstOrDefault(x => x.Carrier == "ALESTRA").Outbound : 0;

                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundNoFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundNoFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundNoFacturable });
                    colImportes.Add(new ImporteTotalVariable { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundNoFacturable });

                    /*LISTA TOTALES POR CARRIER*/
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "BESTEL", Importe = bestelOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "MARCATEL", Importe = marcatelOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "PROTEL", Importe = protelOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                    ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "OUTBOUND", Carrier = "ALESTRA", Importe = alestraOutboundNoFacturable, FactNoFact = "NOFACTURABLE" });
                }

                #endregion

                
                
                buzonNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "SMARTTELCO") != null) ? lista.FirstOrDefault(x => x.Carrier == "SMARTTELCO").Buzon : 0;

                colImportes.Add(new ImporteTotalVariable { Tipo = "BUZON", Carrier = "SMARTTELCO", Importe = buzonNoFacturable });
                /*LISTA TOTALES POR CARRIER*/
                ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "BUZON", Carrier = "SMARTTELCO", Importe = buzonNoFacturable, FactNoFact = "NOFACTURABLE" });

                totalNoFacturable += bestelInboundNoFacturable;
                totalNoFacturable += marcatelInboundNoFacturable;
                totalNoFacturable += protelInboundNoFacturable;
                totalNoFacturable += alestraInboundNoFacturable;

                totalFacturable += bestelOutboundFacturable;
                totalFacturable += marcatelOutboundFacturable;
                if (campaña != "WALMART MG")
                {
                    totalFacturable += protelOutboundFacturable;
                }
                else
                {
                    totalFacturable += protelOutboundFacturable + protelInboundFacturable; //Solo Walmart FG
                }
                
                totalFacturable += alestraOutboundFacturable;

                totalNoFacturable += bestelOutboundNoFacturable;
                totalNoFacturable += marcatelOutboundNoFacturable;
                totalNoFacturable += protelOutboundNoFacturable;
                totalNoFacturable += alestraOutboundNoFacturable;

                totalNoFacturable += buzonNoFacturable;
            }

            #region FACTURABLE
            html += GenerColumnas(bestelInboundFacturable, marcatelInboundFacturable, protelInboundFacturable, alestraInboundFacturable, bestelOutboundFacturable, marcatelOutboundFacturable, protelOutboundFacturable, alestraOutboundFacturable, buzonFacturable);//Facturable
            #endregion

            #region NO FACTURABLE

            html += GenerColumnas(bestelInboundNoFacturable, marcatelInboundNoFacturable, protelInboundNoFacturable, alestraInboundNoFacturable, bestelOutboundNoFacturable, marcatelOutboundNoFacturable, protelOutboundNoFacturable, alestraOutboundNoFacturable, buzonNoFacturable);//No Facturable
            #endregion

            return html;
        }
        private string GenerColumnas(decimal bestelInbound, decimal marcatelInbound, decimal protelInbound, decimal alestraInbound, decimal bestelOutbound, decimal marcatelOutbound, decimal protelOutbound, decimal alestraOutbound, decimal buzon, string color = "")
        {
            StringBuilder html = new StringBuilder();


            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;padding:10px;'> ");

            /*inbound*/
            html.AppendLine(GeneraHtmlInboundOutbound(bestelInbound, marcatelInbound, protelInbound, alestraInbound, color));
            /**/

            /*outbound*/
            html.AppendLine(GeneraHtmlInboundOutbound(bestelOutbound, marcatelOutbound, protelOutbound, alestraOutbound, color));
            /**/

            /*buzon*/
            html.AppendLine("   <tr style='border: 1px solid #CCCCCC'> ");
            html.AppendLine("       <td style='vertical-align:middle;text-align:center;padding:4.5px;" + color + "'>" + buzon.ToString("C2") + "</td> ");
            html.AppendLine("   </tr> ");

            /**/
            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");
            return html.ToString();

        }
        private string GeneraHtmlInboundOutbound(decimal bestel, decimal marcatel, decimal protel, decimal alestra, string color = "")
        {

            StringBuilder html = new StringBuilder();
            html.AppendLine(" <tr> ");
            html.AppendLine("   <td> ");
            html.AppendLine("       <table style='border-collapse:collapse;width:120px;height:170px;text-align:center;" + color + "' cellspacing='0' cellpadding='0'> ");
            html.AppendLine("           <tr style='border: 1px solid #CCCCCC'> ");
            html.AppendLine("               <td> " + bestel.ToString("C2") + " </td> ");
            html.AppendLine("           </tr> ");
            html.AppendLine("           <tr style='border: 1px solid #CCCCCC'> ");
            html.AppendLine("               <td> " + marcatel.ToString("C2") + "</td> ");
            html.AppendLine("           </tr> ");
            html.AppendLine("           <tr style='border: 1px solid #CCCCCC'> ");
            html.AppendLine("               <td> " + protel.ToString("C2") + "</td> ");
            html.AppendLine("           </tr> ");
            html.AppendLine("           <tr style='border: 1px solid #CCCCCC'> ");
            html.AppendLine("               <td> " + alestra.ToString("C2") + "</td> ");
            html.AppendLine("           </tr> ");
            html.AppendLine("       </table> ");
            html.AppendLine("   </td> ");
            html.AppendLine(" </tr> ");

            return html.ToString();
        }
        private string GeneraHtmlSMS(string campaña)
        {
            StringBuilder html = new StringBuilder();

            var lista = matriz.Where(x => x.Campaña == campaña);
            decimal smsFacturable = 0;
            decimal smsNoFacturable = 0;
            if (lista != null && lista.Count() > 0)
            {
                smsFacturable = (lista.FirstOrDefault(x => x.Carrier == "ENVIOSMS") != null) ? lista.FirstOrDefault(x => x.Carrier == "ENVIOSMS").SmsEspecial : 0;
                smsNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "ENVIOSMS") != null) ? lista.FirstOrDefault(x => x.Carrier == "ENVIOSMS").SmsInterno : 0;

                totalFacturable += smsFacturable;
                totalNoFacturable += smsNoFacturable;

                /*SE LLENA LA LISTA PARA LA COLUMNA DE TOTALES*/
                colImportes.Add(new ImporteTotalVariable { Tipo = "ENVIOSMS", Carrier = "ENVIOSMS", Importe = smsFacturable });
                colImportes.Add(new ImporteTotalVariable { Tipo = "ENVIOSMS", Carrier = "ENVIOSMS", Importe = smsNoFacturable });
                /*LISTA TOTALES CARRIERS*/
                ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "ENVIOSMS", Carrier = "ENVIOSMS", Importe = smsFacturable, FactNoFact = "FACTURABLE" });
                ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "ENVIOSMS", Carrier = "ENVIOSMS", Importe = smsNoFacturable, FactNoFact = "NOFACTURABLE" });
            }

            html.AppendLine(DatosSmsMail(smsFacturable, smsNoFacturable));
            return html.ToString();
        }
        private string GeneraHtmlMail(string campaña)
        {
            StringBuilder html = new StringBuilder();
            var lista = matriz.Where(x => x.Campaña == campaña);
            decimal mailFacturable = 0;
            decimal mailNoFacturable = 0;

            if (campaña == "LIVERPOOL - MT" || campaña == "SUBURBIA")
            {
                mailFacturable = (lista.FirstOrDefault(x => x.Carrier == "MAIL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MAIL").Mail : 0;
            }

            if (campaña == "BANCOPPEL" || campaña == "TELEFONICA COBRANZA")
            {
                mailNoFacturable = (lista.FirstOrDefault(x => x.Carrier == "MAIL") != null) ? lista.FirstOrDefault(x => x.Carrier == "MAIL").Mail : 0;
            }

            colImportes.Add(new ImporteTotalVariable { Tipo = "MAIL", Carrier = "MAIL", Importe = mailFacturable });
            colImportes.Add(new ImporteTotalVariable { Tipo = "MAIL", Carrier = "MAIL", Importe = mailNoFacturable });
            /*LISTA TOTALES CARRIERS*/
            ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "MAIL", Carrier = "MAIL", Importe = mailFacturable, FactNoFact = "FACTURABLE" });
            ImportesCarriers.Add(new ImporteTotalCarriers { Tipo = "MAIL", Carrier = "MAIL", Importe = mailNoFacturable, FactNoFact = "NOFACTURABLE" });

            totalFacturable += mailFacturable;
            totalNoFacturable += mailNoFacturable;

            html.AppendLine(DatosSmsMail(mailFacturable, mailNoFacturable));
            return html.ToString();
        }
        private string DatosSmsMail(decimal facturable = 0, decimal noFacturable = 0)
        {

            StringBuilder html = new StringBuilder();
            //FACTURABLE
            html.AppendLine("   <td style='vertical-align:middle;text-align:center;border: 1px solid #CCCCCC;'>" + facturable.ToString("C2") + "</td> ");
            //NO FACTURABLE
            html.AppendLine("   <td style='vertical-align:middle;text-align:center;border: 1px solid #CCCCCC;'>" + noFacturable.ToString("C2") + "</td> ");

            return html.ToString();
        }
        private string DatosEnlaces(string campaña)
        {
            StringBuilder html = new StringBuilder();
            var lista = matriz.Where(x => x.Campaña == campaña);
            var listaCarrier = carriers.Where(x => x.Carrier != "MAIL" && x.Carrier != "ENVIOSMS" && x.Carrier != "SMARTTELCO").ToList();
            /*Facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            decimal importe = 0;
            foreach (var item in listaCarrier)
            {
                html.AppendLine(DatosFijos(0));
            }

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            /*No facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            foreach (var item in listaCarrier)
            {

                decimal importeCarrier = 0;
                importeCarrier += (lista.FirstOrDefault(x => x.Carrier == item.Carrier) != null) ? lista.FirstOrDefault(x => x.Carrier == item.Carrier).Internet : 0;
                importeCarrier += (lista.FirstOrDefault(x => x.Carrier == item.Carrier) != null) ? lista.FirstOrDefault(x => x.Carrier == item.Carrier).L2l : 0;
                importeCarrier += (lista.FirstOrDefault(x => x.Carrier == item.Carrier) != null) ? lista.FirstOrDefault(x => x.Carrier == item.Carrier).Mpls : 0;

                html.AppendLine(DatosFijos(importeCarrier));


                ImporteTotalVariable imp = new ImporteTotalVariable
                {
                    Tipo = "ENLACE",
                    Carrier = item.Carrier,
                    Importe = importeCarrier
                };
                colImportes.Add(imp);

                /*LISTA TOTALES CARRIERS*/
                ImporteTotalCarriers impCarriers = new ImporteTotalCarriers
                {
                    Tipo = "ENLACE",
                    Carrier = item.Carrier,
                    Importe = importeCarrier,
                    FactNoFact = "NOFACTURABLE"
                };
                ImportesCarriers.Add(impCarriers);

                importe += importeCarrier;
            }

            totalNoFacturable += importe;

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            return html.ToString();
        }
        private string DatosHO(string campaña)
        {
            StringBuilder html = new StringBuilder();
            var lista = matriz.Where(x => x.Campaña == campaña);
            var listaCarrier = carriers.Where(x => x.Carrier != "MAIL" && x.Carrier != "ENVIOSMS" && x.Carrier != "SMARTTELCO").ToList();
            /*Facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            decimal importe = 0;
            foreach (var item in listaCarrier)
            {
                html.AppendLine(DatosFijos(0));
            }

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            /*No facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            foreach (var item in listaCarrier)
            {

                decimal importeCarrier = (lista.FirstOrDefault(x => x.Carrier == item.Carrier) != null) ? lista.FirstOrDefault(x => x.Carrier == item.Carrier).Ho : 0;
                html.AppendLine(DatosFijos(importeCarrier));

                ImporteTotalVariable imp = new ImporteTotalVariable
                {
                    Tipo = "HO",
                    Carrier = item.Carrier,
                    Importe = importeCarrier
                };
                colImportes.Add(imp);

                /*LISTA TOTALES CARRIERS*/
                ImporteTotalCarriers impCarriers = new ImporteTotalCarriers
                {
                    Tipo = "HO",
                    Carrier = item.Carrier,
                    Importe = importeCarrier,
                    FactNoFact = "NOFACTURABLE"
                };
                ImportesCarriers.Add(impCarriers);

                importe += importeCarrier;
            }

            totalNoFacturable += importe;

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            return html.ToString();
        }
        private string DatosServAdici(string campaña)
        {
            StringBuilder html = new StringBuilder();
            var lista = matriz.Where(x => x.Campaña == campaña);
            var listaCarrier = carriers.Where(x => x.Carrier != "MAIL" && x.Carrier != "ENVIOSMS" && x.Carrier != "SMARTTELCO").ToList();
            /*Facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            decimal importe = 0;
            foreach (var item in listaCarrier)
            {
                html.AppendLine(DatosFijos(0));
            }

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            /*No facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            foreach (var item in listaCarrier)
            {

                decimal importeCarrier = (lista.FirstOrDefault(x => x.Carrier == item.Carrier) != null) ? lista.FirstOrDefault(x => x.Carrier == item.Carrier).OtrosSer : 0;
                html.AppendLine(DatosFijos(importeCarrier));

                ImporteTotalVariable imp = new ImporteTotalVariable();
                imp.Tipo = "OTROSSERV";
                imp.Carrier = item.Carrier;
                imp.Importe = importeCarrier;
                colImportes.Add(imp);

                /*LISTA TOTALES CARRIERS*/
                ImporteTotalCarriers impCarriers = new ImporteTotalCarriers
                {
                    Tipo = "OTROSSERV",
                    Carrier = item.Carrier,
                    Importe = importeCarrier,
                    FactNoFact = "NOFACTURABLE"
                };
                ImportesCarriers.Add(impCarriers);

                importe += importeCarrier;
            }

            totalNoFacturable += importe;

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            return html.ToString();
        }
        private string DatosFijos(decimal importe)
        {

            StringBuilder html = new StringBuilder();

            html.AppendLine("       <tr style='border: 1px solid #CCCCCC;'> ");
            html.AppendLine("           <td> " + importe.ToString("C2") + "</td> ");
            html.AppendLine("       </tr>");

            return html.ToString();
        }
        private string DatosTotales(decimal totFacturable, decimal totNoFacturable)
        {
            StringBuilder html = new StringBuilder();
            //FACTURABLE
            html.AppendLine("   <td style='vertical-align:middle;text-align:center;border:1px solid #CCCCCC;background-color:#696CAC;color:white'>" + totFacturable.ToString("C2") + "</td> ");
            //NO FACTURABLE
            html.AppendLine("   <td style='vertical-align:middle;text-align:center;border:1px solid #CCCCCC;background-color:#696CAC;color:white'>" + totNoFacturable.ToString("C2") + "</td> ");
            return html.ToString();
        }
        /*Genera Columnas totales*/
        private string TotGeneral()
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("   <td style='text-align:center;padding:4px;border:1px solid #CCCCCC;background-color:#EEEEEE;'>Total General</td> ");
            return html.ToString();
        }
        private string GeneraTotalGeneral(string color = "")
        {
            string html = "";
            decimal bestelInbound = 0;
            decimal marcatelInbound = 0;
            decimal protelInbound = 0;
            decimal alestraInbound = 0;
            decimal bestelOutbound = 0;
            decimal marcatelOutbound = 0;
            decimal protelOutbound = 0;
            decimal alestraOutbound = 0;
            decimal buzon = 0;

            bestelInbound = colImportes.Where(x => x.Carrier == "BESTEL" && x.Tipo == "INBOUND").ToList().Sum(x => x.Importe);
            marcatelInbound = colImportes.Where(x => x.Carrier == "MARCATEL" && x.Tipo == "INBOUND").ToList().Sum(x => x.Importe);
            protelInbound = colImportes.Where(x => x.Carrier == "PROTEL" && x.Tipo == "INBOUND").ToList().Sum(x => x.Importe);
            alestraInbound = colImportes.Where(x => x.Carrier == "ALESTRA" && x.Tipo == "INBOUND").ToList().Sum(x => x.Importe);

            bestelOutbound = colImportes.Where(x => x.Carrier == "BESTEL" && x.Tipo == "OUTBOUND").ToList().Sum(x => x.Importe);
            marcatelOutbound = colImportes.Where(x => x.Carrier == "MARCATEL" && x.Tipo == "OUTBOUND").ToList().Sum(x => x.Importe);
            protelOutbound = colImportes.Where(x => x.Carrier == "PROTEL" && x.Tipo == "OUTBOUND").ToList().Sum(x => x.Importe);
            alestraOutbound = colImportes.Where(x => x.Carrier == "ALESTRA" && x.Tipo == "OUTBOUND").ToList().Sum(x => x.Importe);

            buzon = colImportes.Where(x => x.Carrier == "SMARTTELCO" && x.Tipo == "BUZON").ToList().Sum(x => x.Importe);

            html += GenerColumnas(bestelInbound, marcatelInbound, protelInbound, alestraInbound, bestelOutbound, marcatelOutbound, protelOutbound, alestraOutbound, buzon, color);


            return html;
        }
        private string GeneraHtmlTot(decimal importe)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("   <td style='vertical-align:middle;text-align:center;border:1px solid #CCCCCC;background-color:#EEEEEE;'>" + importe.ToString("C2") + "</td> ");
            return html.ToString();
        }
        private string GeneraTotalesEnlacesOtrosSerHO(string tipo)
        {
            var listaCarrier = carriers.Where(x => x.Carrier != "MAIL" && x.Carrier != "ENVIOSMS" && x.Carrier != "SMARTTELCO").ToList();

            StringBuilder html = new StringBuilder();
            html.AppendLine(" <td style='background-color:#EEEEEE;'> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            foreach (var item in listaCarrier)
            {
                decimal importe = 0;
                importe = colImportes.Where(x => x.Carrier == item.Carrier && x.Tipo == tipo).ToList().Sum(x => x.Importe);

                html.AppendLine("       <tr style='border: 1px solid #CCCCCC;'> ");
                html.AppendLine("           <td> " + importe.ToString("C2") + "</td> ");
                html.AppendLine("       </tr>");
            }

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");
            return html.ToString();
        }
        private string GeneraCarriersTotales()
        {
            StringBuilder html = new StringBuilder();

            /*Facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            decimal importe = 0;
            foreach (var item in carriers)
            {
                decimal importeNoFact = 0;
                importeNoFact = ImportesCarriers.Where(x => x.Carrier == item.Carrier && x.FactNoFact == "FACTURABLE").ToList().Sum(x => x.Importe);
                html.AppendLine(DatosFijos(importeNoFact));
            }

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            /*No facturable*/
            html.AppendLine(" <td> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;' cellspacing='0' cellpadding='0'> ");

            foreach (var item in carriers)
            {
                decimal importeFact = 0;
                importeFact = ImportesCarriers.Where(x => x.Carrier == item.Carrier && x.FactNoFact == "NOFACTURABLE").ToList().Sum(x => x.Importe);
                html.AppendLine(DatosFijos(importeFact));
            }

            totalNoFacturable += importe;

            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");

            return html.ToString();
        }
        private string GeneraTotalCarriers()
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine(" <td style='background-color:#EEEEEE;'> ");
            html.AppendLine("   <table style='border-collapse:collapse;width:100%;height:170px;text-align:center;'cellspacing='0' cellpadding='0'> ");
            foreach (var item in carriers)
            {
                decimal importeTotal = 0;
                importeTotal = colImportes.Where(x => x.Carrier == item.Carrier).ToList().Sum(x => x.Importe);


                html.AppendLine("       <tr style='border: 1px solid #CCCCCC;'> ");
                html.AppendLine("           <td> " + importeTotal.ToString("C2") + "</td> ");
                html.AppendLine("       </tr>");
            }
            html.AppendLine("   </table> ");
            html.AppendLine(" </td> ");
            return html.ToString();
        }
        private string GeneraTotalGeneralCamp()
        {
            StringBuilder html = new StringBuilder();
            decimal importe = 0;
            importe = colImportes.Sum(x => x.Importe);

            html.AppendLine("   <td rowspan='2' style='border: 1px solid #CCCCCC;vertical-align:middle;text-align:center;background-color:#696CAC;color:white;'>" + importe.ToString("C2") + "</td> ");
            return html.ToString();
        }
        private string GeneraSumaFacNoFacCamp()
        {
            StringBuilder html = new StringBuilder();
            decimal importe = 0;
            importe = ImportesCarriers.Sum(x => x.Importe);

            html.AppendLine(" <td colspan='2' style='text-align:center;padding:9px;border:1px solid #CCCCCC;background-color:#696CAC;color:white;'> ");
            html.AppendLine(" " + importe.ToString("C2") + " ");
            html.AppendLine(" </td> ");
            return html.ToString();
        }
        private string ObtieneDatosRepFacturable()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC dbo.ObtieneDatosRepFacturable");
            query.AppendLine(" @FechaIni= '" + fechaInicio + "',");
            query.AppendLine(" @FechaFin = '" + fechaFinal + "',");
            query.AppendLine(" @Usuario = " + iCodUsuario + ",");
            query.AppendLine(" @Perfil = " + iCodPerfil + "");
            return query.ToString();
        }


    }
    public class CarriersFijo
    {
        public string Carrier { get; set; }
    }
    public class ImportesCarriers
    {
        public string Carrier { get; set; }
        public decimal Importe { get; set; }
    }
    public class MatrizImportes
    {
        public string Carrier { get; set; }
        public string Campaña { get; set; }
        public decimal Ibound { get; set; }
        public decimal Outbound { get; set; }
        public decimal Buzon { get; set; }
        public decimal SmsInterno { get; set; }
        public decimal SmsEspecial { get; set; }
        public decimal Internet { get; set; }
        public decimal L2l { get; set; }
        public decimal Ho { get; set; }
        public decimal Mpls { get; set; }
        public decimal OtrosSer { get; set; }
        public decimal Mail { get; set; }
        public decimal Total { get; set; }
    }
    public class ImporteTotalVariable
    {
        public string Tipo { get; set; }
        public string Carrier { get; set; }
        public decimal Importe { get; set; }
        public string FactNoFact { get; set; }
    }
    public class ImporteTotalCarriers
    {
        public string Tipo { get; set; }
        public string Carrier { get; set; }
        public decimal Importe { get; set; }
        public string FactNoFact { get; set; }
    }
}