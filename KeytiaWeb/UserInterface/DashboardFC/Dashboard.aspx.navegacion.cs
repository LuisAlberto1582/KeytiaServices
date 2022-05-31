using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Collections;
using KeytiaServiceBL.Reportes;
using AjaxControlToolkit;
using DSOControls2008;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Web.Services;
using KeytiaWeb.UserInterface.Indicadores;
using KeytiaWeb.Resources;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class Dashboard
    {

        #region Logica de navegación

        bool ValidaConsultaFechasBD()
        {
            bool consultarBD = false;
            if (string.IsNullOrEmpty(param["Nav"]) ||
                    (param["Nav"].ToLower() == "dashboardsiana" && Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == ""))
            {
                if (Session["ListaNavegacion"] != null &&
                    (((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) != null)
                {
                    consultarBD = false;
                }
                else { consultarBD = true; }
            }
            else if (param["Nav"] != string.Empty && !string.IsNullOrEmpty(param["Level"]))
            {
                if (Session["ListaNavegacion"] == null)
                {
                    consultarBD = true;
                }
                else if (((List<MapNav>)Session["ListaNavegacion"]).Count == 0)
                {
                    consultarBD = false;
                }
                else if ((((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) != null)
                {
                    consultarBD = false;
                }
                else if ((((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) == null)
                {
                    consultarBD = true;
                }
                else
                {
                    consultarBD = true;
                }
            }
            else
            {
                consultarBD = false;
            }

            return consultarBD;
        }

        void ConfiguraNavegacion()
        {
            if (param["Nav"] == string.Empty)
            {
                pnlMapaNav.Visible = false;
                pnlIndicadores.Visible = true;

                if (Session["ListaNavegacion"] != null) //Entonces ya tiene navegacion almacenada
                {
                    List<MapNav> listaNavegacion = (List<MapNav>)Session["ListaNavegacion"];
                    listaNavegacion.Clear();
                    Session["ListaNavegacion"] = listaNavegacion;
                }
            }
            else
            {
                pnlMapaNav.Visible = true;
                pnlIndicadores.Visible = false;

                List<MapNav> listaNavegacion = new List<MapNav>();

                //Entonces ya tiene navegacion almacenada. NZ: Se agrego el && string.IsNullOrEmpty(param["Level"]) ya independientemente de si ya trae navegación sí este
                //parametro trae un valor entonces debe eliminar toda la navegación y solo considerar ese nivel. (Por los reportes colgados de un Menú)
                if (Session["ListaNavegacion"] != null && string.IsNullOrEmpty(param["Level"]))
                {
                    listaNavegacion = (List<MapNav>)Session["ListaNavegacion"];

                    if (listaNavegacion.Count == 0)
                    {
                        listaNavegacion.Add(new MapNav() { Titulo = "Inicio", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                    }

                    if (listaNavegacion.Exists(x => x.URL == HttpContext.Current.Request.Url.AbsoluteUri.ToString()))
                    {
                        int index = listaNavegacion.FindIndex(x => x.URL == HttpContext.Current.Request.Url.AbsoluteUri.ToString());
                        if (index == listaNavegacion.Count - 1)
                        {
                            listaNavegacion.RemoveAt(index);
                            listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() });
                        }
                        else { listaNavegacion.RemoveRange(index + 1, (listaNavegacion.Count - 1) - index); }
                    }
                    else { listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() }); }
                }
                else
                {
                    listaNavegacion.Add(new MapNav() { Titulo = "Inicio", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                    listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() });
                }

                Session["ListaNavegacion"] = listaNavegacion;

                pnlMapaNavegacion.Controls.Clear();
                pnlMapaNavegacion.Controls.Add(DTIChartsAndControls.MapaNavegacion(listaNavegacion));
            }


            //Panels
            List<Panel> Reps = new List<Panel>() { Rep0, Rep1, Rep2, Rep3, Rep4, Rep5, Rep6, Rep7, Rep8, Rep9 };
            foreach (Panel item in Reps)
            {
                if (item.Controls.Count == 0)
                {
                    item.Attributes.Add("style", "none");
                }
                else { item.Attributes.Remove("style"); }
            }

        }

        private void LeeQueryString()
        {
            //Se inicializan todos los posibles parametros
            param.Clear();
            param.Add("Nav", string.Empty);
            param.Add("Dash", string.Empty);
            param.Add("Sitio", string.Empty);
            param.Add("TDest", string.Empty);
            param.Add("CenCos", string.Empty);
            param.Add("Emple", string.Empty);
            param.Add("TipoLlam", string.Empty);
            param.Add("MesAnio", string.Empty);
            param.Add("NumMarc", string.Empty);
            param.Add("Carrier", string.Empty);
            param.Add("Concepto", string.Empty);
            param.Add("SitioDest", string.Empty);
            param.Add("Linea", string.Empty);
            param.Add("MiConsumo", string.Empty);
            param.Add("Usuario", string.Empty);
            param.Add("NumGpoTronk", string.Empty);
            param.Add("Locali", string.Empty);
            param.Add("Codigo", string.Empty);
            param.Add("Extension", string.Empty);
            param.Add("Level", string.Empty);
            param.Add("Indicador", string.Empty);
            param.Add("DesglosarCosto", string.Empty);
            param.Add("Client", string.Empty);
            param.Add("Sistema", string.Empty);
            param.Add("EmpleConJer", string.Empty);
            param.Add("omitirInfoCDR", string.Empty); //20200122.RJ.Se utiliza para limitar la información en los reportes implementados originalmente para SevenEleven
            param.Add("omitirInfoSiana", string.Empty); //20200122.RJ.Se utiliza para limitar la información en los reportes implementados originalmente para SevenEleven
            param.Add("FiltroNav", string.Empty);
            param.Add("Dia", string.Empty);
            param.Add("NumDesvios", string.Empty);
            param.Add("Hora", string.Empty);
            param.Add("Dispositivo", string.Empty);
            param.Add("TipoDisp", string.Empty);
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

        public void Navegaciones()
        {
            string NavCenCosJer = string.Empty;
            string descTipoDestino = string.Empty;
            string descSitio = string.Empty;

            string tituloReporteYGrafica = string.Empty;
            string URLSigNivelConsulta = string.Empty;
            string URLSigNivelCampoAdicional = string.Empty;

            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();
            string tituloReportePrevio = string.Empty;

            int omitirInfoCDR = 0;
            int omitirInfoSiana = 0;

            int.TryParse(param["omitirInfoCDR"], out omitirInfoCDR);
            int.TryParse(param["omitirInfoSiana"], out omitirInfoSiana);

            switch (param["Nav"])
            {

                #region Navegaciones Reporte historico

                case "HistoricoDashN1":
                    TituloNavegacion = "Histórico";
                    RepTabHistDash2Pnls(Request.Path + "?Nav=HistoricoN2&MesAnio={0}", "[link] = ''" + Request.Path + "?Nav=HistoricoN2''",
                                                       Rep2, 2, "Gráfica consumo histórico", Rep1, "Consumo histórico");
                    break;
                case "HistoricoN1":
                    TituloNavegacion = "Histórico";
                    RepTabHist2Pnls(Request.Path + "?Nav=HistoricoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&MesAnio={0}",
                                    "''" + Request.Path + "?Nav=HistoricoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "''",
                                                       Rep2, 2, "Gráfica consumo histórico", Rep1, "Consumo histórico", omitirInfoCDR, omitirInfoSiana);

                    break;
                case "HistoricoLlamadasN1":
                    TituloNavegacion = "Histórico";
                    RepTabHistLlamadasPnls(Request.Path + "?Nav=HistoricoLlamadasN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&MesAnio={0}",
                                    "''" + Request.Path + "?Nav=HistoricoLlamadasN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "''",
                                                       Rep2, 2, "Gráfica consumo histórico", Rep1, "Consumo histórico", omitirInfoCDR, omitirInfoSiana);

                    break;
                case "HistoricoN1Prs":
                    TituloNavegacion = "Histórico";
                    RepTabHist2PnlsPrs(Request.Path + "?Nav=HistoricoN2Prs&MesAnio={0}", "[link] = ''" + Request.Path + "?Nav=HistoricoN2Prs''",
                                                       Rep2, 2, "Gráfica consumo histórico", Rep1, "Consumo histórico");
                    break;

                case "HistoricoN2":
                    if (DSODataContext.Schema.ToUpper() == "FCA")
                    {
                        TituloNavegacion = "Sitios";
                        RepTabPorSitio2Pnls(Request.Path + "?Nav=HistoricoN3&Sitio={0}",
                                                                  "[link] = ''" + Request.Path + "?Nav=HistoricoN3&Sitio='' + convert(varchar,[Codigo Sitio])",
                                                                  Rep2, 2, "Gráfica consumo por sitio", Rep1, "Detalle consumo por sitio");
                    }
                    else
                    {
                        TituloNavegacion = "Centros de costos";
                        RepTabPorCenCos2Pnls(Request.Path + "?Nav=HistoricoN3&CenCos={0}",
                                                                  "[link] = ''" + Request.Path + "?Nav=HistoricoN3&CenCos='' + convert(varchar,[Codigo Centro de Costos])",
                                                                  Rep2, 2, "Gráfica consumo por centro de costos", Rep1, "Detalle consumo por centro de costos");
                    }
                    break;
                case "HistoricoLlamadasN2":
                    TituloNavegacion = "Centros de costos";
                    RepTabPorCenCosLlamadasPnls(Request.Path + "?Nav=HistoricoLlamadasN3&CenCos={0}",
                                                              "[link] = ''" + Request.Path + "?Nav=HistoricoLlamadasN3&CenCos='' + convert(varchar,[Codigo Centro de Costos])",
                                                              Rep2, 2, "Gráfica consumo por centro de costos", Rep1, "Detalle consumo por centro de costos");
                    break;
                case "HistoricoN3":
                    string linkGrid1;
                    string linkGraf1;
                    string etiqueta;

                    if (DSODataContext.Schema.ToUpper() == "FCA")
                    {
                        etiqueta = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                        linkGrid1 = Request.Path + "?Nav=HistoricoN4&Sitio=" + param["Sitio"] + "&Emple={0}";
                        linkGraf1 = "[link] = ''" + Request.Path + "?Nav=HistoricoN4&Sitio=" + param["Sitio"] + "&Emple='' + convert(varchar,[Codigo Empleado])";
                    }
                    else
                    {
                        etiqueta = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                        linkGrid1 = Request.Path + "?Nav=HistoricoN4&CenCos=" + param["CenCos"] + "&Emple={0}";
                        linkGraf1 = "[link] = ''" + Request.Path + "?Nav=HistoricoN4&CenCos=" + param["CenCos"] + "&Emple='' + convert(varchar,[Codigo Empleado])";

                    }

                    TituloNavegacion = etiqueta;
                    RepTabPorEmpleMasCaros2Pnls(linkGrid1, linkGraf1,
                                        Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    break;
                case "HistoricoLlamadasN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepTabPorEmpleMasCarosLlamadasPnls(Request.Path + "?Nav=HistoricoLlamadasN4&CenCos=" + param["CenCos"] + "&Emple={0}",
                                        "[link] = ''" + Request.Path + "?Nav=HistoricoLlamadasN4&CenCos=" + param["CenCos"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    break;
                case "HistoricoN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);

                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {

                        RepTabPorTDest2Pnls("", "", Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino");
                    }
                    else
                    {
                        RepTabPorTDest2Pnls(Request.Path + "?Nav=HistoricoN5&CenCos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest={0}",
                                           "[link] = ''" + Request.Path + "?Nav=HistoricoN5&CenCos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                           Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino");
                    }

                    break;

                case "HistoricoLlamadasN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);


                    RepTabPorTDestLlamadasPnls(Request.Path + "?Nav=HistoricoLlamadasN5&CenCos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest={0}",
                                       "[link] = ''" + Request.Path + "?Nav=HistoricoLlamadasN5&CenCos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                       Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino");


                    break;

                case "HistoricoN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0,
                            Request.Path + "?Nav=HistoricoN6&Linea={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&CenCos=" + param["CenCos"],
                                "Consumo por línea", "373"); //Telcel
                        }
                        else
                        {
                            ConsultaLineaPorCarrier("telcel");
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                            ReporteXConceptoConNav(Rep0,
                            Request.Path + "?Nav=HistoricoN7&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&CenCos=" + param["CenCos"] +
                            "&Linea=" + param["Linea"], "Por concepto");
                        }
                    }
                    else if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0,
                                Request.Path + "?Nav=HistoricoN6&Linea={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&CenCos=" + param["CenCos"],
                                "Consumo por línea", "78019"); //Nextel
                        }
                        else
                        {
                            ConsultaLineaPorCarrier("nextel");
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                            ReporteDetalleFacturaNextel();
                        }
                    }
                    else
                    {
                        ReporteDetallado(Rep0, "Detalle de llamadas");
                    }
                    break;
                case "HistoricoLlamadasN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                case "HistoricoN6":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        ReporteXConceptoConNav(Rep0,
                            Request.Path + "?Nav=HistoricoN7&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&CenCos=" + param["CenCos"] +
                            "&Linea=" + param["Linea"],
                                            "Por concepto");
                    }
                    if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        ReporteDetalleFacturaNextel();
                    }
                    break;
                case "HistoricoN7":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Concepto"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        ReporteDetFacturaXEmpleado();
                    }
                    break;
                #endregion Navegaciones Reporte historico

                #region Navegaciones Reporte por Sitio

                //20150106 AM. Navegacion nueva
                case "SitioN1":
                    TituloNavegacion = "Consumo por sitio";
                    Session["OmiteTDets"] = "1";
                    RepTabPorSitio2Pnls(Request.Path + "?Nav=SitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio={0}",
                       "[link] = ''" + Request.Path + "?Nav=SitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio='' + convert(varchar,[Codigo Sitio])",
                       Rep2, 2, "Consumo por sitio", Rep1, "Consumo por sitio", omitirInfoCDR, omitirInfoSiana);
                    break;
                case "SitioN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        RepTabPorEmpleMasCaros2Pnls("", "", Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    }
                    else
                    {
                        string linkGrid2;
                        string linkGraf2;

                        if (DSODataContext.Schema.ToUpper() == "FCA" && HttpContext.Current.Session["OmiteTDets"].ToString() == "0")
                        {
                            linkGrid2 = Request.Path + "?Nav=EmpleMCN3&Sitio=" + param["Sitio"] + "&Emple={0}&TDest=" + param["TDest"] + "";
                            linkGraf2 = "[link] = ''" + Request.Path + "?Nav=EmpleMCN3&Sitio=" + param["Sitio"] + "&TDest=" + param["TDest"] + "&Emple='' + convert(varchar,[Codigo Empleado])";
                        }
                        else
                        {
                            linkGrid2 = Request.Path + "?Nav=EmpleMCN2&Sitio=" + param["Sitio"] + "&Emple={0}";
                            linkGraf2 = "[link] = ''" + Request.Path + "?Nav=SitioN3&Sitio=" + param["Sitio"] + "&Emple='' + convert(varchar,[Codigo Empleado])";
                        }

                        RepTabPorEmpleMasCaros2Pnls(linkGrid2, linkGraf2,
                                            Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    }

                    break;
                case "SitioN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    descSitio = FCAndControls.AgregaEtiqueta(new string[] { param["Sitio"] });
                    if (descSitio.ToLower().Contains("telcel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0, Request.Path + "?Nav=SitioN4&Linea={0}" + "&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"], "Consumo por línea", "373"); //Telcel
                        }
                        else
                        {
                            ConsultaLineaPorCarrier("telcel");
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"], param["Linea"] });
                            ReporteXConceptoConNav(Rep0,
                                            Request.Path + "?Nav=SitioN5&Concepto={0}" + "&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"],
                                            "Por concepto");
                        }
                    }
                    else if (descSitio.ToLower().Contains("nextel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0, Request.Path + "?Nav=SitioN4&Linea={0}" + "&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"], "Consumo por línea"
                             , "78019"); //Nextel
                        }
                        else
                        {
                            ConsultaLineaPorCarrier("nextel");
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"], param["Linea"] });
                            ReporteDetalleFacturaNextel();
                        }
                    }
                    else if (descSitio.ToLower().Contains("enlaces telmex")) //BG.20151103. Se agrega navegacion a Detalle enlaces Telmex
                    {
                        RepTabDetalladoEnlacesTelmex2Pnls("", "", Rep2, 2, "Gráfica Consumo Enlaces Telmex", Rep1, "Detallado Enlaces Telmex");
                    }
                    else { ReporteDetallado(Rep0, "Detalle de llamadas"); }
                    break;
                case "SitioN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                    descSitio = FCAndControls.AgregaEtiqueta(new string[] { param["Sitio"] });
                    if (descSitio.ToLower().Contains("telcel"))
                    {
                        ReporteXConceptoConNav(Rep0,
                                            Request.Path + "?Nav=SitioN5&Concepto={0}" + "&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"],
                                            "Por concepto");
                    }
                    if (descSitio.ToLower().Contains("nextel"))
                    {
                        ReporteDetalleFacturaNextel();
                    }
                    break;
                case "SitioN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Concepto"]);
                    descSitio = FCAndControls.AgregaEtiqueta(new string[] { param["Sitio"] });
                    if (descSitio.ToLower().Contains("telcel"))
                    {
                        ReporteDetFacturaXEmpleado();
                    }
                    break;

                #endregion Navegaciones Reporte por Sitio

                #region Navegaciones Reporte por Tipo Destino

                //RJ.20170502 Se agrega primer nivel llegando desde Dashboard
                case "TDestDashN1":
                    TituloNavegacion = "Por tipo de destino";
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {

                        RepTabPorTDest2Pnls("",
                                                "",
                                                Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino");
                    }
                    else
                    {
                        RepTabPorTDestDashboard2Pnls(Request.Path + "?Nav=TDestN2&TDest={0}",
                                                       "[link] = ''" + Request.Path + "?Nav=TDestN2&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                                        Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino");
                    }

                    break;

                //20150106 AM. Navegacion nueva
                case "TDestN1":
                    TituloNavegacion = "Por tipo de destino";
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {

                        RepTabPorTDest2Pnls("", "", Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino");
                    }
                    else
                    {
                        RepTabPorTDest2Pnls(Request.Path + "?Nav=TDestN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest={0}",
                            "[link] = ''" + Request.Path + "?Nav=TDestN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                                  Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino", omitirInfoCDR, omitirInfoSiana);
                    }

                    break;
                case "TDestN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    HttpContext.Current.Session["OmiteTDets"] = "0";

                    if (DSODataContext.Schema.ToUpper() == "FCA")
                    {
                        RepTabPorSitio2Pnls(Request.Path + "?Nav=SitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio={0}&TDest=" + param["TDest"] + "",
                      "[link] = ''" + Request.Path + "?Nav=SitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest=" + param["TDest"] + "&Sitio='' + convert(varchar,[Codigo Sitio])",
                      Rep2, 2, "Consumo por sitio", Rep1, "Consumo por sitio", omitirInfoCDR, omitirInfoSiana);
                    }
                    else
                    {
                        RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=TDestN3&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest=" + param["TDest"] + "&Emple={0}",
                                "[link] = ''" + Request.Path + "?Nav=TDestN3&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest=" + param["TDest"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                            Rep2, 2, "Consumo por colaborador", Rep1, "Consumo por colaborador", omitirInfoCDR, omitirInfoSiana);
                    }

                    break;
                case "TDestN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0, Request.Path + "?Nav=TDestN4&Linea={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"], "Consumo por línea"
                            , "373"); //Telcel
                        }
                        else
                        {
                            ConsultaLineaPorCarrier("telcel");
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                            ReporteXConceptoConNav(Rep0,
                                            Request.Path + "?Nav=TDestN5&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"],
                                            "Por concepto");
                        }
                    }
                    else if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0, Request.Path + "?Nav=TDestN4&Linea={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"], "Consumo por línea"
                            , "78019"); //Nextel
                        }
                        else
                        {
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                            ConsultaLineaPorCarrier("nextel");
                            ReporteDetalleFacturaNextel();
                        }
                    }
                    else if (descTipoDestino.ToLower().Contains("enlaces telmex")) //BG.20151104. Se agrega navegacion a Detalle enlaces Telmex
                    {
                        RepTabDetalladoEnlacesTelmex2Pnls("", "", Rep2, 2, "Gráfica Consumo Enlaces Telmex", Rep1, "Detallado Enlaces Telmex");
                    }
                    else { ReporteDetallado(Rep0, "Detalle de llamadas"); }
                    break;
                case "TDestN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        ReporteXConceptoConNav(Rep0, Request.Path + "?Nav=TDestN5&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"],
                                            "Por concepto");
                    }
                    if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        ReporteDetalleFacturaNextel();
                    }
                    break;
                case "TDestN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Concepto"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        ReporteDetFacturaXEmpleado();
                    }
                    break;

                #endregion Navegaciones Reporte por Tipo Destino

                #region Navegaciones Reporte por CenCos Jerarquico

                case "CenCosJerN1":
                    TituloNavegacion = "Por centro de costos jerárquico";
                    RepTabPorCenCosJer2Pnls(Request.Path + "?Nav=CenCosJerN2&CenCos={0}",
                                                    "[link] = ''" + Request.Path + "?Nav=CenCosJerN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])",
                                                    Rep2, 2, "Gráfica consumo por centro de costos jerárquico", Rep1, "Detalle consumo por centro de costos jerárquico");
                    break;
                case "CenCosJerN2":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(NavCenCosJer);
                    SeleccionaReportePorCenCosOPorEmple();
                    break;

                case "CenCosJerN3":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    if (NavCenCosJer.ToLower().Contains("telmex"))
                    {
                        RepTabDetalladoEnlacesTelmex2Pnls("", "", Rep2, 2, "Gráfica Consumo Enlaces Telmex", Rep1, "Detallado Enlaces Telmex");
                    }
                    else
                    {
                        ReporteDetallado(Rep0, "Detalle de llamadas");
                    }

                    break;
                case "CenCosJerarquicoN1":
                    TituloNavegacion = "Por centro de costos jerárquico";
                    RepTabCenCosJerarquicoP1(new Control(), "Detalle consumo por centro de costos jerárquico", 3, "Gráfica consumo por centro de costos jerárquico");
                    break;
                case "CenCosJerarquicoSoloCDRN1":
                    TituloNavegacion = "Por centro de costos jerárquico";

                    RepTabCenCosJerarquicoP1(new Control(),
                        "Detalle consumo por centro de costos jerárquico", 3, "Gráfica consumo por centro de costos jerárquico",
                        omitirInfoCDR, omitirInfoSiana);
                    break;
                case "CenCosJerarquicoN2":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(NavCenCosJer);

                    RepTabCenCosJerarquicoN2(2, "Detalle consumo por centro de costos jerárquico", "Gráfica consumo por centro de costos jerárquico",
                        omitirInfoCDR, omitirInfoSiana);
                    break;
                case "CenCosJerarquicoN3":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(NavCenCosJer);

                    RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=EmpleMCN2&CenCos=" + param["CenCos"] + "&Emple={0}" + "&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(),
                                                "[link] = ''" + Request.Path + "?Nav=EmpleMCN2&" + "omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos =" + param["CenCos"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador",
                                        omitirInfoCDR, omitirInfoSiana);
                    break;
                case "CenCosJerarquicoN4":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegaciones Reporte por CenCos Jerarquico

                #region Navegaciones Reporte por empleados mas caros

                case "EmpleMCDashN1":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    TituloNavegacion = tituloReportePrevio;
                    RepTabPorEmpleMasCarosDash2Pnls(Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                                           Request.Path + "?Nav=EmpleMCN2&Emple=",
                                        Rep2, 2, "Gráfica consumo " + tituloReportePrevio, Rep1, "Detalle consumo " + tituloReportePrevio);
                    break;

                case "EmpleCel10DigsN1":
                    tituloReportePrevio = "Por colaborador";

                    TituloNavegacion = tituloReportePrevio;
                    RepTabEmpsConLlamsACel10Digs2Pnls(Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                                           "[link] = ''" + Request.Path + "?Nav=EmpleMCN2&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo " + tituloReportePrevio, Rep1, "Detalle consumo " + tituloReportePrevio);
                    break;

                case "EmpleMCN1":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    TituloNavegacion = tituloReportePrevio;
                    RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                                           "[link] = ''" + Request.Path + "?Nav=EmpleMCN2&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo " + tituloReportePrevio, Rep1, "Detalle consumo " + tituloReportePrevio);
                    break;

                case "EmpleMCN2":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                        //tituloReportePrevio = "Por colaborador";
                    }

                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        RepTabPorTDest2Pnls("", "", Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino", omitirInfoCDR, omitirInfoSiana);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(param["Sitio"]))
                        {
                            RepTabPorTDest2Pnls(Request.Path + "?Nav=EmpleMCN3&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&TDest={0}" + "&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(),
                                                                           "[link] = ''" + Request.Path + "?Nav=EmpleMCN3&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest='' + convert(varchar,[Codigo Tipo Destino])", //"+ TDest +",
                                                                           Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino", new int[] { 2, 3, 4 }, omitirInfoCDR, omitirInfoSiana);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(param["Carrier"]) && DSODataContext.Schema.ToUpper() == "PENTAFON")
                            {
                                TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Carrier"]);
                                RepTabPorTDest2Pnls(Request.Path + "?Nav=EmpleMCN3&Emple=" + param["Emple"] + "&TDest={0}" + "&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), "[link]= ''''",
                                                                           Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino", new int[] { 1, 2, 3, 4 }, omitirInfoCDR, omitirInfoSiana);
                            }
                            else
                            {
                                TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                                RepTabPorTDest2Pnls(Request.Path + "?Nav=EmpleMCN3&Emple=" + param["Emple"] + "&TDest={0}" + "&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(),
                                                                           "[link] = ''" + Request.Path + "?Nav=EmpleMCN3&Emple=" + param["Emple"] + "&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest='' + convert(varchar,[Codigo Tipo Destino])", //"+ TDest +",
                                                                           Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino", new int[] { 2, 3, 4 }, omitirInfoCDR, omitirInfoSiana);
                            }

                        }
                    }

                    break;
                case "EmpleMCN3":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    string titulo = FCAndControls.AgregaEtiqueta(param["TDest"]);

                    if (DSODataContext.Schema.ToUpper() == "FCA" && param["Emple"] != string.Empty && HttpContext.Current.Session["OmiteTDets"].ToString() == "0")
                    {

                        titulo = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    }

                    TituloNavegacion = titulo;

                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0, Request.Path + "?Nav=EmpleMCN4&Linea={0}" + "&Emple=" + param["Emple"] + "&TDest=" + param["TDest"], "Consumo por línea", "373"); //Telcel
                        }
                        else
                        {
                            ConsultaLineaPorCarrier("telcel");
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                            ReporteXConceptoConNav(Rep0, Request.Path + "?Nav=EmpleMCN5&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"], "Por concepto");
                        }
                    }
                    else if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0, Request.Path + "?Nav=EmpleMCN4&Linea={0}" + "&Emple=" + param["Emple"] + "&TDest=" + param["TDest"], "Consumo por línea", "78019"); //Nextel
                        }
                        else
                        {
                            ConsultaLineaPorCarrier("nextel");
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                            ReporteDetalleFacturaNextel();
                        }
                    }
                    else if (descTipoDestino.ToLower().Contains("enlaces telmex")) //BG.20151104. Se agrega navegacion a Detalle enlaces Telmex
                    {
                        RepTabDetalladoEnlacesTelmex2Pnls("", "", Rep2, 2, "Gráfica Consumo Enlaces Telmex", Rep1, "Detallado Enlaces Telmex");
                    }
                    else { ReporteDetallado(Rep0, "Detalle de llamadas", omitirInfoCDR, omitirInfoSiana); }
                    break;
                case "EmpleMCN4":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);

                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        ReporteXConceptoConNav(Rep0,
                                            Request.Path + "?Nav=EmpleMCN5&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"],
                                            "Por concepto");
                    }
                    if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        ReporteDetalleFacturaNextel();
                    }
                    break;

                case "EmpleMCN5":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Concepto"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        ReporteDetFacturaXEmpleado();
                    }
                    break;
                #endregion Navegaciones Reporte por empleados mas caros

                #region Navegaciones Reporte por tipo de llamada

                case "TpLlamN1":
                    TituloNavegacion = "Por tipo de llamada";
                    RepTabPorTpLlam2Pnls(Request.Path + "?Nav=TpLlamN2&TipoLlam={0}",
                                                                  "[link] = ''" + Request.Path + "?Nav=TpLlamN2&TipoLlam=" + "'' + convert(varchar,[Clave Tipo Llamada])",
                                                                  Rep2, 2, "Gráfica consumo por tipo de llamada", Rep1, "Consumo por tipo de llamada");
                    break;
                case "TpLlamN2":
                    TituloNavegacion = BuscarTipoLlamada();
                    RepTabPorTDest2Pnls(Request.Path + "?Nav=TpLlamN3&TipoLlam=" + param["TipoLlam"] + "&TDest={0}",
                                                "[link] = ''" + Request.Path + "?Nav=TpLlamN3&TipoLlam=" + param["TipoLlam"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                                Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino");
                    break;
                case "TpLlamN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegaciones Reporte por tipo de llamada

                #region Navegaciones Reporte matricial por sitio

                case "SitioMatN1":
                    TituloNavegacion = "Consumo por sitio";
                    RepMatPorSitio2Pnls(Request.Path + "?Nav=SitioN2&Sitio={0}",
                                                                  "[link] = ''" + Request.Path + "?Nav=SitioN2&Sitio=" + "'' + convert(varchar,[Codigo Sitio])",
                                                                  Rep2, 2, "Gráfica consumo matricial sitio X tipo de destino", Rep1, "Consumo matricial sitio X tipo de destino");
                    break;

                #endregion Navegaciones Reporte matricial por sitio

                #region Navegaciones Reporte por centro de costos

                case "CenCosN1":
                    TituloNavegacion = "Por centro de costos";
                    RepTabPorCenCos2Pnls(Request.Path + "?Nav=CenCosN2&CenCos={0}",
                                                              "[link] = ''" + Request.Path + "?Nav=CenCosN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])",
                                                              Rep2, 2, "Gráfica consumo por centro de costos", Rep1, "Detalle consumo por centro de costos");
                    break;
                case "CenCosN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=CenCosN3&CenCos=" + param["CenCos"] + "&Emple={0}",
                                        "[link] = ''" + Request.Path + "?Nav=CenCosN3&CenCos=" + param["CenCos"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    break;
                case "CenCosN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegaciones Reporte por centro de costos

                #region Navegaciones Reporte por carrier

                case "RepPorCarrierN1":
                    TituloNavegacion = "Por carrier";
                    RepMatPorCarrier2Pnls(Request.Path + "?Nav=RepPorCarrierN2&Carrier={0}",
                                        "[link] = ''" + Request.Path + "?Nav=RepPorCarrierN2&Carrier=" + "'' + convert(varchar,[Codigo Carrier])",
                                        Rep2, 2, "Gráfica por carrier", Rep1, "Por carrier");
                    break;

                case "RepPorCarrierN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Carrier"]);
                    RepMatPorSitio2Pnls(Request.Path + "?Nav=RepPorCarrierN3&Carrier=" + param["Carrier"] + "&Sitio={0}",
                                                                  "[link] = ''" + Request.Path + "?Nav=RepPorCarrierN3&Carrier=" + param["Carrier"] + "&Sitio=" + "'' + convert(varchar,[Codigo Sitio])",
                                                                  Rep2, 2, "Gráfica consumo matricial sitio X tipo de destino", Rep1, "Consumo matricial sitio X tipo de destino");
                    break;
                case "RepPorCarrierN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepTabPorCenCos2Pnls(Request.Path + "?Nav=RepPorCarrierN4&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&CenCos={0}",
                                                              "[link] = ''" + Request.Path + "?Nav=RepPorCarrierN4&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&CenCos='' + convert(varchar,[Codigo Centro de Costos])",
                                                              Rep2, 2, "Gráfica consumo por centro de costos", Rep1, "Detalle consumo por centro de costos");
                    break;
                case "RepPorCarrierN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=RepPorCarrierN5&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&CenCos=" + param["CenCos"] + "&Emple={0}",
                                                    "[link] = ''" + Request.Path + "?Nav=RepPorCarrierN5&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&CenCos=" + param["CenCos"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    break;
                case "RepPorCarrierN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegaciones Reporte por carrier

                #region Navegaciones Reporte historico por dia de la semana

                case "RepHistPorDiaSemN1":
                    TituloNavegacion = "Por día de la semana";
                    RepTabPorDiaDeSemana2Pnls(Rep2, 0, "Gráfica consumo por día de la semana", Rep1, "Detalle consumo por día de la semana");
                    break;

                #endregion Navegaciones Reporte historico por dia de la semana

                #region Navegaciones historico 3 años

                case "Hist3AniosN1":
                    TituloNavegacion = "Llamadas por la red Q";
                    RepTabHist3Anios2Pnls("", "", Rep2, 0, "Histórico 3 años", Rep1, "Histórico 3 años");
                    break;

                #endregion Navegaciones historico 3 años

                #region Navegaciones reporte Colaboradores

                case "ConColaboradoresN1":
                    TituloNavegacion = "Colaboradores Directos";
                    RepTabConsColaboradores2Pnls(Rep2, 0, "Colaboradores Directos", Rep1, "Colaboradores Directos");
                    break;

                #endregion Navegaciones reporte Colaboradores

                #region Navegaciones reporte por tipo de destino Prs

                case "RepTDestPrsN1":
                    TituloNavegacion = "Consumo por Carrier";
                    RepTabTipoDestinoPrs2PnlsNiv2(Rep2, 2, "Consumo por Carrier", Rep1, "Consumo por Carrier");
                    break;

                case "RepTDestPrsN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Carrier"]);
                    RepTabTipoDestinoPrs2Pnls(Rep2, 2, "Consumo Por Tipo de Destino", Rep1, "Consumo Por Tipo de Destino");
                    break;

                case "RepTDestPrsN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (param["TDest"] == "238135")
                    {
                        RepTabTipoDestinoPrsLD2PnlsNiv2(Rep2, 2, "Consumo Por LD", Rep1, "Consumo Por LD");
                    }
                    else if (param["TDest"] == "83283" && param["Carrier"] == "374") //BG.20160908
                    {
                        RepTabTipoDestinoPrsDetallRenta1Pnl(Rep0, "Consumo Rentas Telmex");
                    }
                    else if (param["TDest"] == "255748" && param["Carrier"] == "374") //BG.20160912
                    {
                        RepTabTipoDestinoPrsDetallEnlaces1Pnl(Rep0, "Consumo Enlaces Telmex");
                    }
                    else if (param["TDest"] == "83492" && param["Carrier"] == "374") //BG.20160912
                    {
                        RepTabTipoDestinoPrsDetallUninet1Pnl(Rep0, "Consumo Servicios Uninet Telmex");
                    }
                    else if (param["TDest"] == "383" && param["Carrier"] == "374") //BG.20160912
                    {
                        RepTabTipoDestinoPrsDetall800E1Pnl(Rep0, "Consumo 800 Entrada Telmex");
                    }
                    else if (param["TDest"] == "279390") //BG.20161004
                    {
                        RepTabTipoDestinoPrsDetallEnlAxtel1Pnl(Rep0, "Consumo Enlaces Axtel");
                    }
                    else if (param["TDest"] == "279391") //BG.20161004
                    {
                        RepTabTipoDestinoPrsDetallEnlAvantel1Pnl(Rep0, "Consumo Enlaces Avantel");
                    }
                    else if (param["TDest"] == "383" && param["Carrier"] == "200413") //BG.20160912
                    {
                        RepTabTipoDestinoPrsDetall800EAvantel1Pnl(Rep0, "Consumo 800 Entrada Avantel");
                    }
                    else if (param["TDest"] == "83283" && param["Carrier"] == "291794") //BG.20170216 TELNOR RENTAS
                    {
                        RepTabTipoDestinoPrsDetallRentaTelnor1Pnl(Rep0, "Consumo Rentas Telnor");
                    }
                    else if (param["TDest"] == "238134" && param["Carrier"] == "291794") //BG.20170216 TELNOR SM
                    {
                        RepTabTipoDestinoPrsDetallSMTelnor1Pnl(Rep0, "Consumo SM Telnor");
                    }
                    else if (param["TDest"] == "292895" && param["Carrier"] == "291794") //BG.20170216 TELNOR ENLACES
                    {
                        RepTabTipoDestinoPrsDetallEnlTelnor1Pnl(Rep0, "Consumo Enlaces Telnor");
                    }
                    else
                    {
                        RepTabTipoDestinoPrs2PnlsNiv3(Rep2, 2, "Consumo Por Centro de Costos", Rep1, "Consumo Por Centro de Costos");
                    }
                    break;

                case "RepTDestPrsN4":

                    tituloReporteYGrafica = "Consumo Por Colaborador";


                    //PARAMETROS
                    listadoParametros.Add("TDest", param["TDest"]);
                    listadoParametros.Add("Carrier", param["Carrier"]);
                    listadoParametros.Add("CenCos", param["CenCos"]);
                    //CAMPOS
                    listadoCamposReporte.Add(new CampoReporte("Codigo Colaborador", "Codigo Colaborador", "", true, true, false, false));
                    listadoCamposReporte.Add(new CampoReporte("NombreEmple", "Colaborador", "", false, false, false, true));
                    listadoCamposReporte.Add(new CampoReporte("TotImporte", "Total", "{0:c}", false, false, true, false));
                    listadoCamposReporte.Add(new CampoReporte("LLamadas", "Cantidad llamadas", "{0:0,0}", false, false, true, false));
                    listadoCamposReporte.Add(new CampoReporte("TotMin", "Cantidad minutos", "{0:0,0}", false, false, true, false));
                    listadoCamposReporte.Add(new CampoReporte("link", "link", "", false, true, false, false));


                    //URL strings para siguiente nivel de navegación
                    URLSigNivelConsulta = Request.Path + "?Nav=RepTDestPrsN5&TDest=" + param["TDest"] + "&Carrier=" + param["Carrier"] + "&Cencos=" + param["CenCos"] + "&Emple={0}";
                    URLSigNivelCampoAdicional = "link = ''" + Request.Path + "?Nav=RepTDestPrsN5&TDest=" + param["TDest"] + "&Carrier=" + param["Carrier"] + "&Cencos=" + param["CenCos"] + "&Emple=" + "'' + convert(varchar,[Codigo Empleado])";

                    //Mapa de navegación
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);

                    //Instancia el nuevo reporte y obtiene los datos necesarios para mostrarse
                    RepTDestPrsN4 rep = new RepTDestPrsN4(Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59", Request.Path, listadoParametros);
                    DataTable ldt = rep.RepTabTipoDestinoPrs2PnlsNiv4(Rep1, tituloReporteYGrafica, listadoCamposReporte, URLSigNivelCampoAdicional);

                    //Genera grid en página
                    rep.GeneraReporte2PnlsEnPagina(Rep1, ldt, "RepTabTipoDestinoPrsGrid", listadoCamposReporte, tituloReporteYGrafica, URLSigNivelConsulta, 1);

                    //Genera gráfica en página
                    GeneraGraficaUnaSerie(Rep2, ldt, "RepTabTipoDestinoPrsGraf", "bar2d", tituloReporteYGrafica, "Colaborador", "Total", "ControlesAlCentro", "dti", 10, true);

                    break;

                case "RepTDestPrsN5":
                    TituloNavegacion = "Detalle";
                    ReporteDetallado(Rep0, "Detalle de llamadas");

                    break;
                case "RepTDestPrsLDN3":
                    TituloNavegacion = "Consumo por Centro de Costos";
                    RepTabTipoDestinoPrs2PnlsNiv3(Rep2, 2, "Consumo Por Centro de Costos", Rep1, "Consumo Por Centro de Costos");
                    break;

                case "RepTDestPrsOrange":
                    TituloNavegacion = "Detalle Orange";
                    RepTabTipoDestinoPrsOrange1Pnl(Rep0, "Detallado Factura Orange");
                    break;

                case "RepTDestPrsUninet":
                    TituloNavegacion = "Detalle Uninet";
                    RepTabTipoDestinoPrsUninet1Pnl(Rep0, "Detallado Factura Uninet Colombia");
                    break;

                case "RepTDestPrsBestel":
                    TituloNavegacion = "Detalle Bestel";
                    RepTabTipoDestinoPrsBestel1Pnl(Rep0, "Detallado Factura Bestel");
                    break;

                #endregion Navegaciones reporte por tipo de destino Prs

                #region Reporte Top 5 Areas de Mayor Consumo

                case "TopAreasN1":
                    TituloNavegacion = "Áreas de Mayor Consumo";
                    RepTopAreaPrs2Pnls(Request.Path + "?Nav=TopAreasN2&CenCos={0}",
                                                    "[link] = ''" + Request.Path + "?Nav=TopAreasN2&CenCos='' + convert(varchar,[idArea])",
                                                    Rep2, 2, "Gráfica Áreas de Mayor Consumo", Rep1, "Áreas de Mayor Consumo");
                    break;


                case "TopAreasN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=TopAreasN3&CenCos=" + param["CenCos"] + "&Emple={0}",
                                           "[link] = ''" + Request.Path + "?Nav=TopAreasN3&CenCos=" + param["CenCos"] + "&Emple=" + "'' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica Áreas de Mayor Consumo", Rep1, "Áreas de Mayor Consumo");
                    break;
                case "TopAreasN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Reporte Top 5 Areas de Mayor Consumo

                #region Reporte Detallado de Llamadas Telcel F4

                case "RepDetalladoLlamsTelcelF4":
                    TituloNavegacion = "Detalle de llamadas Telcel";
                    RepTabDetalleLlamsF4Telcel1Pnl(Rep0, "Detallado de Llamadas Telcel");
                    break;

                #endregion Reporte Detallado de Llamadas Telcel F4

                #region Navegaciones Reporte Consumo Por Campaña Prosa
                /* BG.20151026 Reporte Consumo Por Campaña Prosa */
                case "RepPrsConsumosPorCampaniaN1":
                    TituloNavegacion = "Por Campaña";
                    RepTabConsumoPorCampaniaPrs2Pnls("", "", Rep2, 1, "Gráfica Consumo Por Campaña", Rep1, "Consumo Por Campaña");
                    break;

                #endregion Navegaciones Reporte Consumo Por Campaña Prosa

                #region Navegaciones Reporte por tipo destino (perfil emple)
                case "TDestPeEmN1":
                    TituloNavegacion = "Colaborador por tipo de servicio";
                    ReportePorTipoDestinoPeEmple();
                    break;

                case "TDestPeEmN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        param["Emple"] = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();
                        if (param["Emple"] != "-1")
                        {
                            //20150115 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                            int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                            if (numeroDeLineasTelcel > 1)
                            {
                                ReportePorLineas(Rep0, Request.Path + "?Nav=TDestPeEmN3&Linea={0}" + "&TDest=" + param["TDest"] + "&Emple" + param["Emple"],
                                                "Consumo por línea", "373"); //Telcel
                            }
                            else
                            {
                                ConsultaLineaPorCarrier("telcel");
                                TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                                ReporteXConceptoConNav(Rep0,
                                            Request.Path + "?Nav=TDestPeEmN4&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple" + param["Emple"] + "&Linea=" + param["Linea"],
                                            "Por concepto");
                            }
                        }
                        else
                        {
                            Label sinEmpleadoAsignado = new Label();
                            sinEmpleadoAsignado.Text = "Este usuario no cuenta con un empleado asignado.";
                            Rep0.Controls.Add(sinEmpleadoAsignado);
                        }
                    }
                    else if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        param["Emple"] = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();
                        if (param["Emple"] != "-1")
                        {
                            //20150115 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                            int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                            if (numeroDeLineasTelcel > 1)
                            {
                                ReportePorLineas(Rep0, Request.Path + "?Nav=TDestPeEmN3&Linea={0}" + "&TDest=" + param["TDest"] + "&Emple" + param["Emple"],
                                                "Consumo por línea", "78019"); //Nextel
                            }
                            else
                            {
                                ConsultaLineaPorCarrier("nextel");
                                TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Linea"]);
                                ReporteDetalleFacturaNextel();
                            }

                        }
                        else
                        {
                            Label sinEmpleadoAsignado = new Label();
                            sinEmpleadoAsignado.Text = "Este usuario no cuenta con un empleado asignado.";
                            Rep0.Controls.Add(sinEmpleadoAsignado);
                        }

                    }
                    else
                    {
                        ReportePorNumMarcadoPeEmple(Request.Path + "?Nav=TDestPeEmN3&TDest=" + param["TDest"] + "&NumMarc={0}&TipoLlam={1}");
                    }
                    break;

                case "TDestPeEmN3":
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                        ReporteXConceptoConNav(Rep0,
                                            Request.Path + "?Nav=TDestPeEmN4&Concepto={0}" + "&TDest=" + param["TDest"] + "&Emple" + param["Emple"] + "&Linea=" + param["Linea"],
                                            "Por concepto");
                    }
                    else if (descTipoDestino.ToLower().Contains("nextel"))
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                        ReporteDetalleFacturaNextel();
                    }
                    else
                    {
                        TituloNavegacion = "'" + param["NumMarc"];
                        ReporteDetalladoPeEm();
                    }
                    break;
                case "TDestPeEmN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Concepto"]);
                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    if (descTipoDestino.ToLower().Contains("telcel"))
                    {
                        ReporteDetFacturaXEmpleado();
                    }
                    break;
                #endregion Navegaciones Reporte por tipo destino (perfil emple)

                #region Navegaciones Reporte por tipo de llamada (perfil emple)

                case "PorTipoLlamN1":
                    TituloNavegacion = "Por tipo de llamada";
                    ReportePorTipoLlamadaPeEm();
                    break;

                case "PorTipoLlamN2":
                    TituloNavegacion = BuscarTipoLlamada();
                    ReportePorNumMarcadoPeEmple(Request.Path + "?Nav=PorTipoLlamN3&NumMarc={0}&TipoLlam={1}");
                    break;

                case "PorTipoLlamN3":
                    TituloNavegacion = "'" + param["NumMarc"];
                    ReporteDetalladoPeEm();
                    break;

                #endregion Navegaciones Reporte por tipo de llamada (perfil emple)

                #region Navegaciones Reporte por numero marcado (perfil emple)

                case "NumMasCarosN1":
                    TituloNavegacion = "Números mas caros";
                    ReportePorNumMasCarosPeEm();
                    break;

                case "NumMasCarosN2":
                    TituloNavegacion = "'" + param["NumMarc"];
                    ReporteDetalladoPeEm();
                    break;

                #endregion Navegaciones Reporte por numero marcado (perfil emple)

                #region Navegaciones Reporte historico (perfil emple)

                case "HistoricoPeEmN1":
                    TituloNavegacion = "Consumo histórico a  12 meses";
                    ReporteHistoricoPeEm();
                    break;

                #endregion Navegaciones Reporte historico (perfil emple)

                #region Navegaciones Reporte extensiones en las que se uso el codauto (perfil emple)

                case "ExtenUsoCodAutN1":
                    TituloNavegacion = "Extensiones en las que se usó el código de llamadas";
                    ReporteExtenDondeSeUsoCodAutoPeEm();
                    break;

                #endregion Navegaciones Reporte extensiones en las que se uso el codauto (perfil emple)

                #region Navegaciones Reporte Consumo por tipo de llamada (perfil emple)

                case "ConPorTipoLlamN1":
                    TituloNavegacion = "Por tipo de llamada";
                    ReporteConPorTipoLlamadaPeEm();
                    break;

                #endregion Navegaciones Reporte Consum por tipo de llamada (perfil emple)

                #region Navegacion reporte top empleados con sitio
                case "EmpleMCConSitioN1":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    TituloNavegacion = tituloReportePrevio;
                    RepTabPorEmpleMasCarosConSitio2Pnls(Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                                           "[link] = ''" + Request.Path + "?Nav=EmpleMCN2&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo " + tituloReportePrevio, Rep1, "Detalle consumo " + tituloReportePrevio);
                    break;
                #endregion

                #region Navegaciones para reporte AccesosAgrupado
                case "RepTabAccesosAgrupadoN1":
                    TituloNavegacion = "Accesos por Usuario";
                    RepTabAccesosAgrupado(Request.Path + "?Nav=RepTabAccesosAgrupadoN2&Usuario={0}", Rep2, 2, "Accesos", Rep0, "Accesos por Usuario");
                    break;

                case "RepTabAccesosAgrupadoN2":
                    TituloNavegacion = "Bítacora de Accesos";
                    RepTabAccesosAgrupadoN2("", Rep2, 2, "Accesos", Rep0, "Bítacora Accesos");
                    break;
                #endregion

                #region Navegaciones Reporte Speed Dials

                case "RepTabResumenSpeedDialN1":
                    TituloNavegacion = "Resumen Speed Dials";

                    RepTabResumenSpeedDials2PnlsN1(Request.Path + "?Nav=RepTabResumenSpeedDialN2&NumMarc={0}&Sitio={1}",
                                            Request.Path + "?Nav=RepTabResumenSpeedDialN2&NumMarc='' + convert(varchar,[Numero Marcado])+''&Sitio='' + convert(varchar,[Sitio])",
                                            Rep2, 2, "Gráfica Resumen Speed Dial", Rep1, "Reporte Resumen Speed Dial");
                    break;

                case "RepTabResumenSpeedDialN2":
                    param["NumMarc"] = param["NumMarc"].Replace("'", "");
                    TituloNavegacion = "Detalle Speed Dial " + param["NumMarc"] + FCAndControls.AgregaEtiqueta(param["Sitio"]);

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("([Codigo Autorizacion] IS NULL OR [Codigo Autorizacion] = '''')");
                    query.AppendLine("  AND ([Costo] + [CostoSM]) > 0");
                    query.AppendLine("  AND [Extension] <> ''''");
                    query.AppendLine("  AND [Numero Marcado] IN (");
                    query.AppendLine("							SELECT '''''''' + NumMarcadoReal");
                    query.AppendLine("						    FROM " + DSODataContext.Schema + ".[VisHistoricos(''SpeedDial'',''Speed Dials'',''Español'')]");
                    query.AppendLine("							WHERE dtIniVigencia <> dtFinVigencia");
                    query.AppendLine("								AND (dtIniVigencia <= ''" + Session["FechaInicio"].ToString() + " 00:00:00" + "'' OR dtIniVigencia < ''" + Session["FechaFin"].ToString() + " 23:59:59'')");
                    query.AppendLine("								AND (dtFinVigencia > ''" + Session["FechaInicio"].ToString() + " 00:00:00" + "'' OR dtFinVigencia >= ''" + Session["FechaFin"].ToString() + " 23:59:59'')");
                    query.AppendLine("                              AND NumMarcadoReal = ''" + param["NumMarc"] + "''");
                    query.AppendLine("						)");

                    WhereAdicional = query.ToString();
                    ReporteDetallado(Rep0, "Reporte Detalle Speed Dial");
                    break;

                #endregion Navegaciones Reporte Speed Dials Yazaki

                #region Reporte Trafico Por Hora

                case "RepTabTraficoPorHora2Pnl":
                    TituloNavegacion = "Trafíco por Hora";
                    RepTabTraficoProHora2Pnl(Rep0, Rep2, 1, "Gráfica", Rep1, "Trafíco por Hora");
                    break;
                #endregion

                #region Reporte Consumo Por Campaña y Tipo de Destino

                case "RepMatConsumoCampaniaTDest":
                    TituloNavegacion = "Consumo Por Campaña";
                    RepMatConsumoPorCampañaTipoDest1Pnl(Rep0, "Consumo Por Campaña");
                    break;
                #endregion Reporte Consumo Por Campaña y Tipo de Destino

                #region Navegacion Reporte Llamadas Buzon de Voz

                /* BG.20180402 Reporte Llamadas Buzon de Voz */
                case "RepLlamsBuzonVoz":
                    TituloNavegacion = "Llamadas en Buzón de Voz";
                    ReporteLlamadasBuzonDeVoz1Pnl(Rep0, "Llamadas en Buzón de Voz");
                    break;


                /* BG.20180409 Reporte Detalle de Llamadas en Buzon de Voz */
                case "RepDetalleLlamsBuzonVoz":
                    TituloNavegacion = "Detalle de Llamadas en Buzón de Voz";
                    ReporteDetalleLlamsBuzonDeVoz1Pnl(Rep0, "Detalle de Llamadas en Buzón de Voz");
                    break;

                #endregion Navegacion Reporte Llamadas Buzon de Voz

                #region Navegaciones Reporte llamadas por la red Q
                ///20180702 RM Falta Probar los Reportes de Red Q
                case "RepLlamRedQN1":
                    TituloNavegacion = "Llamadas por la red Q";
                    RepTabPorConsSimuSitiosTDest2Pnls(Request.Path + "?Nav=RepLlamRedQN2&Sitio={0}",
                        "[link] = ''" + Request.Path + "?Nav=RepLlamRedQN2&Sitio=" + "'' + convert(varchar,[Codigo Sitio Origen])",
                        Rep2, 4, "Llamadas por la red Q", Rep1, "Llamadas por la red Q");

                    // "doughnut3d"
                    break;
                case "RepLlamRedQN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepTabPorConsSimuTDest2Pnls(Request.Path + "?Nav=RepLlamRedQN3&Sitio=" + param["Sitio"] + "&SitioDest={0}",
                        "[link] = ''" + Request.Path + "?Nav=RepLlamRedQN3&Sitio=" + param["Sitio"] + "&SitioDest=" + "'' + convert(varchar,[Codigo Sitio Destino])",
                        Rep2, 4, "Llamadas por la red Q", Rep1, "Llamadas por la red Q");
                    // "doughnut3d"
                    break;
                case "RepLlamRedQN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["SitioDest"]);
                    ReporteDetalladoConsumoSimulado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegaciones Reporte llamadas por la red Q

                #region Reporte Detalle para Finanzas
                //BG. 20150422  Navegacion Reporte Detalle para Finanzas

                case "RepTabDetalleParaFinanzas":
                    TituloNavegacion = "Detalle Para Finanzas";
                    RepTabDetalleParaFinanzasPrs1Pnls(Rep0, "Detalle Para Finanzas");
                    break;

                #endregion Reporte Detalle para Finanzas

                #region Navegacion Reporte Códigos en más de N extensiones
                case "RepEstTabCodEnMasDeNExtensiones":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Códigos en más de N extensiones");
                    RepEstTabCodEnMasDeNExtensiones1Pnl(Rep0, "Códigos en más de N extensiones");
                    break;
                case "RepEstTabCodEnMasDeNExtensionesN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Codigo"]);
                    RepEstTabCodEnMasDeNExtensionesN21Pnl(Rep0, "Códigos en más de N extensiones");
                    break;
                #endregion Navegacion Reporte Códigos en más de N extensiones

                #region Navegacion Reporte Días procesados
                case "RepMatDiasProcesados":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Días procesados");
                    RepMatDiasProcesados1Pnl(Rep0, "Días procesados");
                    break;
                #endregion Navegacion Reporte Días procesados

                #region Navegacion Reporte códigos No asignados
                case "RepTabCodigosNoAsignados":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Códigos no asignados");
                    RepTabCodigosNoAsignados2Pnls(Request.Path + "?Nav=RepTabCodigosNoAsignadosN2",
                                                    "[link] = ''" + Request.Path + "?Nav=RepTabCodigosNoAsignadosN2&Codigo='' + convert(varchar,[Codigo Autorización])",
                                                    Rep2, 2, "Códigos no asignados", Rep1, "Códigos no asignados");
                    break;
                case "RepTabCodigosNoAsignadosN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Codigo"]);
                    RepTabCodigosNoAsignadosN21Pnl(Rep0, "Códigos no asignados");
                    break;
                #endregion Navegacion Reporte códigos No asignados

                #region Navegacion Reporte extensiones no asignadas
                case "RepTabExtensionesNoAsignadas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Extensiones no asignadas");
                    RepTabExtensionesNoAsignadas2Pnls(Request.Path + "?Nav=RepTabExtensionesNoAsignadasN2",
                                                    "[link] = ''" + Request.Path + "?Nav=RepTabExtensionesNoAsignadasN2&Extension='' + convert(varchar,[Extensión])",
                                                    Rep2, 2, "Extensiones no asignadas", Rep1, "Extensiones no asignadas");
                    break;
                case "RepTabExtensionesNoAsignadasN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Extension"]);
                    RepTabExtensionesNoAsignadasN21Pnl(Rep0, "Extensiones no asignadas");
                    break;
                #endregion Navegacion Reporte extensiones no asignadas

                #region Navegacion Reporte matricial por Carrier
                case "RepMatConsumoPorCarrier":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Consumo por carrier");
                    RepMatConsumoPorCarrier1Pnl(Rep0, "Consumo por carrier");
                    break;
                case "RepMatConsumoPorCarrierN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Carrier"]);
                    RepMatConsumoPorCarrierN21Pnl(Rep0, "Consumo por sitio");
                    break;
                case "RepMatConsumoPorCarrierN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepMatConsumoPorCarrierN31Pnl(Rep0, "Consumo por centro de costos");
                    break;
                case "RepMatConsumoPorCarrierN4":
                    TituloNavegacion = (DSODataContext.Schema.ToLower() == "fca") ? FCAndControls.AgregaEtiqueta(param["Sitio"]) : FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepMatConsumoPorCarrierN41Pnl(Rep0, "Consumo por empleado");
                    break;
                case "RepMatConsumoPorCarrierN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    string gridLink = (DSODataContext.Schema.ToLower() == "fca") ? Request.Path + "?Nav=RepMatConsumoPorCarrierN6&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&TDest={0}" : Request.Path + "?Nav=RepMatConsumoPorCarrierN6&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&Cencos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest={0}";
                    string gridGraf = (DSODataContext.Schema.ToLower() == "fca") ? "[link] = ''" + Request.Path + "?Nav=RepMatConsumoPorCarrierN6&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])" : "[link] = ''" + Request.Path + "?Nav=RepMatConsumoPorCarrierN6&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&Cencos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])";
                    RepTabPorTDest2Pnls(gridLink, gridGraf, Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino", new int[] { 2, 3, 4 });
                    break;
                case "RepMatConsumoPorCarrierN6":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegacion Reporte matricial por Carrier

                #region Navegacion Reporte matricial por Empleado
                case "RepMatConsumoPorEmple":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Consumo por colaborador");
                    RepMatConsumoPorEmple1Pnl(Rep0, "Consumo por colaborador");
                    break;
                case "RepMatConsumoPorEmpleN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    RepMatConsumoPorEmpleN21Pnl(Rep0, "Consumo por sitio");
                    break;
                case "RepMatConsumoPorEmpleN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepTabPorTDest2Pnls(Request.Path + "?Nav=RepMatConsumoPorEmpleN4&Emple=" + param["Emple"] + "&Sitio=" + param["Sitio"] + "&TDest={0}",
                                                                   "[link] = ''" + Request.Path + "?Nav=RepMatConsumoPorEmpleN4&Emple=" + param["Emple"] + "&Sitio=" + param["Sitio"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                                                   Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino", new int[] { 2, 3, 4 });
                    break;
                case "RepMatConsumoPorEmpleN4":

                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegacion Reporte matricial por Empleado

                #region Navegacion Reporte matricial por Sitio
                case "RepMatConsumoPorSitio":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Consumo por sitio");
                    RepMatConsumoPorSitio1Pnl(Rep0, "Consumo por sitio");
                    break;
                case "RepMatConsumoPorSitioN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepMatConsumoPorSitioN21Pnl(Rep0, "Consumo por Centro de costos");
                    break;
                case "RepMatConsumoPorSitioN3":
                    TituloNavegacion = (DSODataContext.Schema.ToLower() == "fca") ? FCAndControls.AgregaEtiqueta(param["Sitio"]) : FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepMatConsumoPorSitioN31Pnl(Rep0, "Consumo por empleado");
                    break;
                case "RepMatConsumoPorSitioN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    string lGrid = (DSODataContext.Schema.ToLower() == "fca") ? Request.Path + "?Nav=RepMatConsumoPorSitioN5&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&TDest={0}" : Request.Path + "?Nav=RepMatConsumoPorSitioN5&Sitio=" + param["Sitio"] + "&Cencos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest={0}";
                    string lGraf = (DSODataContext.Schema.ToLower() == "fca") ? "[link] = ''" + Request.Path + "?Nav=RepMatConsumoPorSitioN5&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])" : "[link] = ''" + Request.Path + "?Nav=RepMatConsumoPorSitioN5&Sitio=" + param["Sitio"] + "&Cencos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])";
                    RepTabPorTDest2Pnls(lGrid, lGraf, Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino", new int[] { 2, 3, 4 });
                    break;
                case "RepMatConsumoPorSitioN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegacion Reporte matricial por Sitio

                #region Navegacion Reporte matricial por CenCos Sin Jerarquia
                case "RepMatConsumoPorCenCosSJ":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Consumo por centro de costos");
                    RepMatConsumoPorCenCosSJ1Pnl(Rep0, "Consumo por centro de costos");
                    break;
                case "RepMatConsumoPorCenCosSJN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepMatConsumoPorCenCosSJN21Pnl(Rep0, "Consumo por empleado");
                    break;
                case "RepMatConsumoPorCenCosSJN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    RepMatConsumoPorCenCosSJN31Pnl(Rep0, "Consumo por sitio");
                    break;
                case "RepMatConsumoPorCenCosSJN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepTabPorTDest2Pnls(Request.Path + "?Nav=RepMatConsumoPorCenCosSJN5&Cencos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&Sitio=" + param["Sitio"] + "&TDest={0}",
                                                                   "[link] = ''" + Request.Path + "?Nav=RepMatConsumoPorCenCosSJN5&Cencos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&Sitio=" + param["Sitio"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                                                   Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino", new int[] { 2, 3, 4 });
                    break;
                case "RepMatConsumoPorCenCosSJN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegacion Reporte matricial por CenCos Sin Jerarquia

                #region Navegacion Reporte Detallado para Cinvestav
                case "ReporteDetalladoCinvestav":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Detalle de llamadas");
                    ReporteDetalladoCinvestav1Pnl(Rep0, "Detalle de llamadas");
                    break;
                #endregion Navegacion Reporte Detallado para Cinvestav

                #region EmpleadosMasCaros RJ
                case "ConsEmpsMasCarosN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ConsEmpsMasCarosN21Pnl(Rep0, "Top empleados más caros");
                    break;
                case "ConsEmpsMasCarosN3":
                    string gridNav = "";
                    string grafNav = "";
                    if (param["Sitio"] != "")
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);

                        gridNav = Request.Path + "?Nav=ConsEmpsMasCarosN4&Emple=" + param["Emple"] + "&Sitio=" + param["Sitio"] + "&TDest={0}";
                        grafNav = "[link] = ''" + Request.Path + "?Nav=ConsEmpsMasCarosN4&Emple=" + param["Emple"] + "&Sitio=" + param["Sitio"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])";
                    }
                    else
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                        gridNav = Request.Path + "?Nav=ConsEmpsMasCarosN4&Emple=" + param["Emple"] + "&TDest={0}";
                        grafNav = "[link] = ''" + Request.Path + "?Nav=ConsEmpsMasCarosN4&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])";
                    }

                    RepTabPorTDest2Pnls(gridNav, grafNav, Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino", new int[] { 2, 3, 4 });
                    break;
                case "ConsEmpsMasCarosN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                #endregion  EmpleadosMasCaros RJ

                #region Empleados mas duracion RJ

                case "ConsumoEmpsMasTiempoN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    RepTabPorTDest2Pnls(Request.Path + "?Nav=ConsumoEmpsMasTiempoN4&Emple=" + param["Emple"] + "&TDest={0}",
                                                                   "[link] = ''" + Request.Path + "?Nav=ConsumoEmpsMasTiempoN4&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])",
                                                                   Rep2, 2, "Consumo por tipo de destino", Rep1, "Consumo por tipo de destino", new int[] { 2, 3, 4 });
                    break;
                case "ConsumoEmpsMasTiempoN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"], param["TDest"] });
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion

                #region Generados por JH y RM
                #region Navegacion Reporte Top Empleados
                case "ConsumoEmpsMasTiempo":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Top colaboradores más tiempo al teléfono");
                    ConsumoEmpsMasTiempo2Pnls(Request.Path + "?Nav=ConsumoEmpsMasTiempoN2&Emple={0}",
                                                                   "[link] = ''" + Request.Path + "?Nav=ConsumoEmpsMasTiempoN2&Emple='' + convert(varchar,[Codigo Empleado])",
                                                                   Rep2, 2, "Top colaboradores", Rep1, "Top colaboradores");
                    break;
                #endregion  Navegacion Reporte Top Empleados


                #region Navegacion Reporte Top Llamadas
                case "RepTabMenuConsLLamadasMasTiempo":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Top llamadas con mayor duración");
                    RepTabMenuConsLLamadasMasTiempo1Pnl(Rep0, "Top llamadas con mayor duración");
                    break;
                #endregion Navegacion Reporte Top Llamadas

                #region Navegacion Reporte Extensiones Abiertas
                case "RepEstTabExtAbiertas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Extensiones Abiertas");
                    RepEstTabExtAbiertas2Pnls(Rep2, 2, "Cantidad de Llamadas", Rep1, "Cantidad de Llamadas");
                    break;
                #endregion Navegacion Reporte Extensiones Abiertas

                #region Navegacion Reporte Empleado Consolidado
                case "RepTabConsPorEmpleado":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Empleado (Consolidado)");
                    RepTabConsPorEmpleado2Pnls(Rep2, 2, "Consumo Por Categoria de Llamadas", Rep1, "Empleados");
                    break;
                case "RepTabConsPorEmpleadoN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    string gridLnk = (DSODataContext.Schema.ToLower() == "fca") ? "[link] = ''" + Request.Path + "?Nav=RepTabConsPorEmpleadoN3&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])'" : "[link] = ''" + Request.Path + "?Nav=RepTabConsPorEmpleadoN3&Emple=" + param["Emple"] + "&CenCos=" + param["CenCos"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])'";

                    RepTabConsPorEmpleadoN22Pnls(gridLnk, Rep2, 2, "Consumo Por Tipo Destino", Rep1, "Consumo Por Tipo Destino");

                    break;
                case "RepTabConsPorEmpleadoN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    string lnkGrid = (DSODataContext.Schema.ToLower() == "fca") ? "[link] = ''" + Request.Path + "?Nav=RepTabConsPorEmpleadoN4&Emple=" + param["Emple"] + "&NumMarc=''+ Convert(varchar,[Numero Marcado]) + ''&TipoEtiqueta='' + convert(varchar,[Clave Tipo Llamada]) + ''&TDest=''+ convert(varchar,[Codigo Tipo Destino])'" : "[link] = ''" + Request.Path + "?Nav=RepTabConsPorEmpleadoN4&Emple=" + param["Emple"] + "&CenCos=" + param["CenCos"] + "&NumMarc=''+ Convert(varchar,[Numero Marcado]) + ''&TipoEtiqueta='' + convert(varchar,[Clave Tipo Llamada]) + ''&TDest=''+ convert(varchar,[Codigo Tipo Destino])'";
                    RepTabConsPorEmpleadoN32Pnls(lnkGrid, Rep2, 2, "Consumo Tabular Por Número Marcado", Rep1, "Consumo Por Número Marcado");
                    break;
                case "RepTabConsPorEmpleadoN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["NumMarc"]);
                    RepTabConsPorEmpleadoN41Pnl(Rep0, " Llamadas");
                    break;
                #endregion Navegacion Reporte Empleado Consolidado

                #region Navegacion Reporte Top Destinos
                case "RepTabConsPobmasMindeConv":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Top lugares");
                    RepTabConsPobmasMindeConv1Pnl(Rep0, "Top lugares");
                    break;
                case "RepTabConsPobmasMindeConvN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Locali"]);
                    string navGraf = (DSODataContext.Schema.ToUpper() == "FCA") ? "[link] = ''" + Request.Path + "?Nav=RepTabConsPobmasMindeConvN3&Emple=''+ Convert(varchar,[Codigo Empleado]) '" : "[link] = ''" + Request.Path + "?Nav=RepTabConsPobmasMindeConvN3&Emple=''+ Convert(varchar,[Codigo Empleado]) + ''&Locali=''+Convert(varchar,[Codigo Localidad])'";
                    RepTabConsPobmasMindeConvN22Pnls(navGraf, Rep2, 2, "Consumo por Empleado", Rep1, "Consumo por Empleado");
                    break;
                case "RepTabConsPobmasMindeConvN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    RepTabConsPobmasMindeConvN32Pnls("[link] = ''" + Request.Path + "?Nav=RepTabConsPobmasMindeConvN4&Emple=''+ Convert(varchar,[Codigo Empleado]) + ''&Locali=''+Convert(varchar,[Codigo Localidad]) +''&TipoEtiqueta=''+Convert(varchar,[Clave Tipo Llamada])'",
                        Rep2, 2, "Consumo por Tipo Llamada", Rep1, "Consumo por Tipo Llamada");
                    break;
                case "RepTabConsPobmasMindeConvN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TipoLlam"]);
                    RepTabConsPobmasMindeConv1N4Pnl(Rep0, " Llamadas");
                    break;
                #endregion Navegacion Reporte Top Destinos



                #region Navegacion Reporte Llamadas fuera de Horario laboral
                case "RepTabLLamsFueraHoraLaboral":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Llamadas Fuera del Horario Laboral");
                    RepTabLLamsFueraHoraLaboral1Pnl(Rep0, "Llamadas Fuera del Horario Laboral");
                    break;
                #endregion Navegacion Reporte Llamadas fuera de Horario laboral

                #region Navegacion Reporte Importe Top Destinos


                case "ConsLugmasCost":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Destinos más costosos");
                    ConsLugmasCost2Pnls(Request.Path + "?Nav=ConsLugmasCostN2&Locali={0}",
                                                                   "[link] = ''" + Request.Path + "?Nav=ConsLugmasCostN2&Locali='' + convert(varchar,[Codigo Localidad])",
                                                                   Rep2, 2, "Top localidades", Rep1, "Top localidades");
                    break;
                case "ConsLugmasCostN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Locali"]);
                    string lnkGr = (DSODataContext.Schema.ToUpper() == "FCA") ? Request.Path + "?Nav=ConsLugmasCostN4&Locali=" + param["Locali"] + "&Sitio={0}" : Request.Path + "?Nav=ConsLugmasCostN3&Locali=" + param["Locali"] + "&Sitio={0}";
                    string lnkGra = (DSODataContext.Schema.ToUpper() == "FCA") ? "[link] = ''" + Request.Path + "?Nav=ConsLugmasCostN4&Locali=" + param["Locali"] + "&Sitio='' + convert(varchar,[Codigo Sitio])" : "[link] = ''" + Request.Path + "?Nav=ConsLugmasCostN3&Locali=" + param["Locali"] + "&Sitio='' + convert(varchar,[Codigo Sitio])";
                    ConsLugmasCostN22Pnls(lnkGr, lnkGra, Rep2, 2, "Consumo por sitio", Rep1, "Consumo por sitio");
                    break;
                case "ConsLugmasCostN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    ConsLugmasCostN32Pnls(Rep0, "Consumo por centro de costos");
                    break;
                case "ConsLugmasCostN4":
                    TituloNavegacion = (DSODataContext.Schema.ToUpper() == "FCA") ? FCAndControls.AgregaEtiqueta(param["Sitio"]) : FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    ConsLugmasCostN42Pnls(Rep0, "Consumo por empleado");
                    break;
                case "ConsLugmasCostN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                #endregion Navegacion Reporte Importe Top Destinos


                #region Navegacion Reporte Llamadas Top Destinos
                case "ConsLocalidMasMarcadas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Destinos con más llamadas");
                    ConsLocalidMasMarcadas1Pnl(Rep0, "Top Destinos");
                    break;
                #endregion Navegacion Reporte Llamadas Top Destinos

                #region Navegacion Reporte Llamadas Top Empledos
                case "ConsEmpmasLlam":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Colaboradores con más llamadas");
                    ConsEmpmasLlam1Pnl(Rep0, "Colaboradores con más llamadas");
                    break;
                #endregion Navegacion Reporte Llamadas Top Empledos



                #region Navegacion Reporte Llamadas Top Empledos N2

                case "ConsEmpmasLlamN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ConsEmpmasLlamN22Pnl(Request.Path + "?Nav=ConsEmpmasLlamN3&Emple=" + param["Emple"] + "&TDest={0}",
                                            "link = ''" + Request.Path + "?Nav=ConsEmpmasLlamN3&Emple=" + param["Emple"] + "&TDest=" + "'' + convert(varchar,[Nombre Tipo Destino])",
                                            Rep2,
                                            2,
                                            "Top Empleados",
                                            Rep1,
                                            "Top Empleados");
                    break;

                #endregion Navegacion Reporte Llamadas Top Empledos N2

                #region Navegacion Reporte Llamadas Top Empledos N3

                case "ConsEmpmasLlamN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ConsEmpmasLlamN31Pnl(Rep0, "Top Empleados");
                    break;

                #endregion Navegacion Reporte Llamadas Top Empledos N3

                #region Navegacion Reporte Llamadas Top Empleados

                case "ConsEmpsMasCaros":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Top colaboradores más caros");
                    ConsEmpsMasCaros1Pnl(Rep0, "Top colaboradores más caros");
                    break;
                #endregion Navegacion Reporte Llamadas Top Empleados

                #region Navegacion Reporte Consumo por Num Marc
                case "RepTabConsumosPorNumMarcTipoLlamada":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Tipo de llamada");
                    RepTabConsumosPorNumMarcTipoLlamada1Pnl(Rep0, "Tipo de llamada");
                    break;
                #endregion Navegacion Reporte Consumo por Num Marc

                #region Navegacion Reporte Consumo por Num Marc N2
                case "RepTabConsumosPorNumMarcTipoLlamadaN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["NumMarc"]);
                    RepTabConsumosPorNumMarcTipoLlamadaN21Pnl(Rep0, "Tipo de Llamada");
                    break;
                #endregion Navegacion Reporte Consumo por Num Marc N2


                #region Navegacion ConsLlamsMasCaras
                case "ConsLlamsMasCaras":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Top llamadas más caras");
                    ConsLlamsMasCaras1Pnl(Rep0, "Top llamadas más caras");
                    break;
                #endregion Navegacion ConsLlamsMasCaras


                #region Navegacion ConsNumerosMasMarcados
                case "ConsNumerosMasMarcadas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Top números más marcados");
                    ConsNumerosMasMarcadas1Pnl(Rep0, "Top Números Marcados");
                    break;
                case "ConsNumerosMasMarcadasN2":
                    TituloNavegacion = param["NumMarc"];
                    string grdLnk = (DSODataContext.Schema.ToUpper() == "FCA") ? Request.Path + "?Nav=ConsNumerosMasMarcadasN3&NumMarc=" + param["NumMarc"] + "&locali=" + param["Locali"] + "&Emple={0}&Sitio={1}" : Request.Path + "?Nav=ConsNumerosMasMarcadasN3&NumMarc=" + param["NumMarc"] + "&locali=" + param["Locali"] + "&Emple={0}&CenCos={1}&Sitio={2}";
                    string grfLnk = (DSODataContext.Schema.ToUpper() == "FCA") ? "link = ''" + Request.Path + "?Nav=ConsNumerosMasMarcadasN3&NumMarc=" + param["NumMarc"] + "&locali=" + param["Locali"] + "&Emple='' + convert(varchar,[Codigo Empleado] )+''&Sitio=''+convert(varchar,[Codigo Sitio] )" : "link = ''" + Request.Path + "?Nav=ConsNumerosMasMarcadasN3&NumMarc=" + param["NumMarc"] + "&locali=" + param["Locali"] + "&Emple='' + convert(varchar,[Codigo Empleado] )+''&CenCos='' + convert(varchar,[Codigo Centro de Costos])+''&Sitio=''+convert(varchar,[Codigo Sitio] )";
                    ConsNumerosMasMarcadasN22Pnl(grdLnk, grfLnk, Rep2, 2, "", Rep1, "");
                    break;
                case "ConsNumerosMasMarcadasN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ConsNumerosMasMarcadasN31Pnl(Rep0, "Detalle de llamadas");
                    break;
                #endregion Navegacion ConsNumerosMasMarcados



                #region Navegacion ConsLocalidMasMarcadasN2

                case "ConsLocalidMasMarcadasN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Top destinos");

                    ConsLocalidMasMarcadasN22Pnl(
                                               "",
                                               "",
                                               Rep2,
                                               2,
                                               "",
                                               Rep1,
                                               "");
                    break;
                #endregion Navegacion ConsLocalidMasMarcadasN2


                #endregion Generados por JH y RM

                #region Reporte entre sitios INBAL
                case "RepTabLlamadasEntreSitios":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Llamadas entre sitios");
                    RepTabLlamadasEntreSitios2Pnls(Request.Path + "?Nav=RepTabLlamadasEntreSitiosN2&Sitio=",
                                                              Rep2, 2, "Gráfica Llamadas de Entrada", Rep1, "Llamadas de Entrada");
                    break;
                case "RepTabLlamadasEntreSitiosN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepTabLlamadasEntreSitiosN22Pnls(Request.Path + "?Nav=RepTabLlamadasEntreSitiosN3&Sitio=" + param["Sitio"] + "&SitioDest=",
                                                              Rep2, 2, "Gráfica Llamadas de Entrada por Sitio", Rep1, "Llamadas de Entrada");
                    break;
                case "RepTabLlamadasEntreSitiosN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["SitioDest"]);
                    RepTabLlamadasEntreSitiosN32Pnls(Request.Path + "?Nav=RepTabLlamadasEntreSitiosN4&SitioDest=" + param["SitioDest"] + "&Extension=''+CONVERT(VARCHAR,Teldest)+''" + "&SitioDest=" + param["SitioDest"],

                                                              Rep2, 2, "Gráfica Llamadas de Entrada por Extensión", Rep1, "Llamadas de Entrada");
                    break;
                case "RepTabLlamadasEntreSitiosN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Extension"]);
                    RepTabLlamadasEntreSitiosN41Pnl(Rep0, "Detalle de Llamadas de Entrada");
                    break;
                #endregion Reporte entre sitios INBAL

                #region Navegación Reporte Por sucursales Bimbo

                case "RepPorSucursalv2N1":
                    TituloNavegacion = "Consumo por sucursal";
                    RepConsumoPorSucursalesv2Bimbo2Pnls(Request.Path + "?Nav=RepPorSucursalv2N2&Sitio={0}",
                                                        "[link] = ''" + Request.Path + "?Nav=RepPorSucursalv2N2&Sitio='' + convert(varchar,[Codigo Sitio])",
                                            Rep2, 2, "Consumo por sucursal", Rep1, "Consumo por sucursal");
                    break;

                case "RepPorSucursalv2N2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    RepConsumoPorSucursalesv2BimboN22Pnls("", "",
                                            Rep2, 2, "Consumo por sucursal", Rep1, "Consumo por sucursal");
                    break;
                #endregion

                #region Reportes Indicadores Niveles

                case "RepIndConcentracionGastoN2":
                    TituloNavegacion = "Empleados que concentran el 15% del gasto total";
                    RepIndConcentracionGasto2Nvl();
                    break;
                case "RepIndConcentracionGastoN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    RepTabPorTDest2Pnls(Request.Path + "?Nav=ConsEmpsMasCarosN4&Emple=" + param["Emple"] + "&TDest={0}&Indicador=1",
                                      "[link] = ''" + Request.Path + "?Nav=ConsEmpsMasCarosN4&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])+''&Indicador=1''",
                                      Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino");
                    break;

                case "RepIndCodAutoNuevosN2":
                    TituloNavegacion = "Códigos nuevos";
                    RepIndCodAutoNuevos2Nvl();
                    break;

                case "RepIndExtenNuevasN2":
                    TituloNavegacion = "Extensiones nuevas";
                    RepIndExtenNuevas2Nvl();
                    break;

                case "RepIndLlamMayorDuracionN2":
                    TituloNavegacion = "Llamada de mayor duración a celular";
                    RepIndLlamMayorDuracion2Nvl(0);
                    break;

                case "RepIndLlamMayorDuracionN2LDM":
                    TituloNavegacion = "Llamada de mayor duración, larga distancia mundial";
                    RepIndLlamMayorDuracion2Nvl(1);
                    break;
                case "RepTabIndicadorCodigosNoAsignadosN2":
                    TituloNavegacion = "Códigos por Identificar";
                    RepTabIndicadorCodigosNoAsignados2Pnls(Rep2, 2, TituloNavegacion, Rep1, TituloNavegacion);
                    break;
                case "RepTabIndicadorExtensionesNoAsignadasN2":
                    TituloNavegacion = "Extensiones por Identificar";
                    RepTabIndicadorExtensionesNoAsignadas2Pnls(Rep2, 2, TituloNavegacion, Rep1, TituloNavegacion);
                    break;
                #endregion

                #region Reporte Busca lineas proximas a vencer
                case "RepTepBuscaLineasAVencer":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Renovación de planes");
                    RepTabBuscaLineasAVencer(Rep0, "Renovación de planes", 4);
                    break;
                #endregion

                #region Reporte Año Acual vs Anterior
                case "ConRepTabConsHistAnioActualVsAnterior2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Año actual vs año anterior", });
                    ConRepTabConsHistAnioActualVsAnterior22Pnl(Rep1, "Año actual vs año anterior", Rep2, "Grafica año actual vs año anterior");
                    break;
                #endregion

                #region Reporte Matricial Dias Procesados
                case "ReporteMatricialDiasProcesados":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reporte Dias Procesados", });
                    ReporteMatricialDiasProcesados(Rep0, "Reporte Dias Proecsados");
                    break;
                #endregion

                #region Reporte Llams entrada filtro por etension
                case "RepTabReporteLlamadasEntradasFiltroExten":
                    string ulrSigNivel = Request.Path + "?Nav=RepTabReporteLlamadasEntradasFiltroExtenN2&Tdest={0}&Extension={1}";
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reporte llamadas de entrada CC", });
                    RepTabReporteLlamadasEntradasFiltroExten(Rep0, "Reporte llamadas de entrada CC", ulrSigNivel);
                    break;
                #endregion

                #region Reporte Llams entrada filtro por etension
                case "RepTabReporteLlamadasEntradasFiltroExtenN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle de llamadas", });

                    WhereAdicional = WhereAdicional + " Extension = ''" + param["Extension"].ToString() + "''";
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                #endregion

                #region Reporte Lineas Telcel que exceden Renta
                case "RepLineasExcedentesTelcel2pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Líneas Telcel que Exceden Renta" });
                    RepLineasExcedentesTelcel2pnl(Rep1, "Excedente Líneas Telcel", Rep2, "Excedente Líneas Telcel", "[link] = ''" + Request.Path + "?Nav=PorConceptoN2&Linea=''+CONVERT(VARCHAR,[Codigo Linea])+''&NumMarc=''+CONVERT(VARCHAR,[Linea])+''");
                    break;
                #endregion
                #region ReporteLineasTelcelExcedenteN2.

                case "PorConceptoN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Por Concepto [" + param["NumMarc"] + "]" });
                    RepPorCategoriaExcedente2pnl(Rep1, "[link] = ''" + Request.Path + "?Nav=PorConceptoN3&Concepto=''+CONVERT(VARCHAR,[idConcepto])+''&Linea=''+CONVERT(VARCHAR,[Codigo Linea])+''&NumMarc=''+CONVERT(VARCHAR,[Linea])+''&Level=''+[Concepto]+''", Rep2);
                    break;
                case "PorConceptoN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Desgloce de Concepto - [" + param["Level"] + "]" });
                    RepPorConcepto(Rep0, "");
                    break;
                #endregion


                #region Reporte Minutos por Carrier.

                case "RepMinPorCarrier":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Minutos por carrier" });
                    RepMinutosPorCarrier(Rep1, Rep2);
                    break;
                #endregion

                #region Reporte Acceso Etiquetacion
                case "RepAccesEtiquetacion":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Accesos Etiquetación" });
                    RepAccesEtiquetacion(Rep0, "Accesos Etiquetación");
                    break;
                #endregion

                #region Reporte Por Empleados Con Jerarquia
                case "ReportePorEmpleConJer":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Comsumo por colaborador jerarquico" });
                    ReportePorEmpleConJer(Rep0, "Comsumo por colaborador jerarquico");
                    break;
                #endregion
                #region ReportePochtecaLlamNoContestadas
                case "RepLlamPerdidas2pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Total de Llamadas Perdidas por Sitio" });
                    RepTabTotLlamNoContestadas2Pnl("", "", Rep2, 2, "Total de Llamadas Perdidas por Sitio", Rep1, "Total de Llamadas Perdidas por Sitio");
                    break;
                case "RepLlamPerdidasN22Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle por colaborador" });
                    RepTabTotLlamNoContestadasN22Pnl("", "", Rep2, 2, "Detalle por Colaborador", Rep1, "Detalle por Colaborador");
                    break;
                case "RepLlamPerdidasN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle de llamadas" });
                    RepTabTotLlamNoContestadasN3(Rep0, "Detalle de Llamadas Perdidas");
                    break;
                #endregion
                #region Navegaciones Reporte por SitioSiana

                //20150106 AM. Navegacion nueva
                case "SitioSianaN1":
                    TituloNavegacion = "Consumo por Sitio Solo Siana";
                    RepTabPorSitioSiana2Pnls(Request.Path + "?Nav=SitioN2&Sitio={0}",
                                         "[link] = ''" + Request.Path + "?Nav=SitioSianaN2&Sitio='' + convert(varchar,[Codigo Sitio])", Rep2, 2, "Consumo por Sitio Solo Siana", Rep1, "Consumo por Sitio Solo Siana");
                    break;
                case "SitioSianaN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);

                    RepTabPorEmpleMasCarosSiana2Pnls(Request.Path + "?Nav=EmpleMCNSiana2&Sitio=" + param["Sitio"] + "&Emple={0}",
                                                    "[link] = ''" + Request.Path + "?Nav=EmpleMCNSiana2&Sitio=" + param["Sitio"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    break;
                case "EmpleMCNSiana2":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    //BG.20161129 SE AGREGA COMO PARAMETRO EL SITIO
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);

                    if (!string.IsNullOrEmpty(param["Sitio"]))
                    {
                        RepTabPorSianaTDest2Pnls(Request.Path + "?Nav=EmpleMCNSiana3&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&TDest={0}",
                                                                       "[link] = ''" + Request.Path + "?Nav=EmpleMCNSiana3&Sitio=" + param["Sitio"] + "&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])", //"+ TDest +",
                                                                       Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino", new int[] { 2, 3, 4 });
                    }
                    else
                    {
                        RepTabPorSianaTDest2Pnls(Request.Path + "?Nav=EmpleMCNSiana3&Emple=" + param["Emple"] + "&TDest={0}",
                                                                       "[link] = ''" + Request.Path + "?Nav=EmpleMCNSiana3&Emple=" + param["Emple"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])", //"+ TDest +",
                                                                       Rep2, 2, "Gráfica consumo por tipo de destino", Rep1, "Detalle consumo por tipo de destino", new int[] { 2, 3, 4 });
                    }

                    break;
                case "EmpleMCNSiana3":
                    if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
                    {
                        tituloReportePrevio = "Por Línea";
                    }
                    else { tituloReportePrevio = "Por colaborador"; }

                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);

                    descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);

                    ReporteDetalladoSiana(Rep0, "Detalle de llamadas");
                    break;
                #endregion Navegaciones Reporte por Sitio
                case "RepTabSeeYouOnUtilCliente2pnl":
                    TituloNavegacion = "Utilización Sistemas Cliente";
                    RepTabSeeYouOnUtilCliente2pnl("", "", Rep2, 2, "Utilización Sistemas Cliente", Rep1, "Utilización Sistemas Cliente");
                    break;
                case "RepTabSeeYouOnUtilClienteN2":
                    TituloNavegacion = "Utilización Sistemas Cliente - " + FCAndControls.AgregaEtiqueta(param["Client"]);
                    RepTabSeeyouOnUtilClienteN2(Rep0, "");
                    break;
                case "RepTabSeeYouOnUtilClienteN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Client"]) + FCAndControls.AgregaEtiqueta(param["Sistema"]);
                    RepTabSeeyouOnUtilClienteN3(Rep0, "");
                    break;
                case "RepTabSeeYouOnUtilSistema2pnl":
                    TituloNavegacion = "Utilización Sistema";
                    RepTabSeeYounOnUtilSistema2Pnl("", "", Rep2, 2, "Utilización Sistema", Rep1, "Utilización Sistema");
                    break;
                case "RepTabSeeYouOnUtilSistemaHist2pnl":
                    TituloNavegacion = "Histórico de Utilización de Sistema";
                    RepTabSeeYouOnUtilSistemaHist2Pnl(Rep2, 2, "Histórico de Utilización de Sistema", Rep1, "Histórico de Utilización de Sistema");
                    break;
                case "RepTabSeeYouOnCencosHoras2Pnl":
                    TituloNavegacion = "Horas Hombre por Centro de Costos";
                    RepTabSeeYouOnHorasHombreCencos2Pnl(Rep2, 2, "Horas Hombre por Centro de Costos", Rep1, "Horas Hombre por Centro de Costos");
                    break;
                case "RepTabImporteEmplePorTpLlam":
                    TituloNavegacion = "Tipo LLamada";
                    RepTabImportePorEmpleyTipoLlamada(Rep0, "Reporte Por Tipo de Llamada");
                    break;
                case "RepTeleMarketing":
                    TituloNavegacion = "Consumo MKT";
                    RepTeleMarketing(Rep0, "Consumo MKT");
                    break;
                case "RepBolsaConsumoN1":
                    TituloNavegacion = "Consumo de Bolsas";
                    RepConsumoBolsasDiebold2pnl("", Rep2, 1, "Consumo Bolsa Diaria", Rep1, "Consumo de Bolsas");
                    break;
                case "RepTabBolsaDiariaN2":
                    TituloNavegacion = "Detalle de Consumo";
                    RepConsumoBolsasDieboldN2(Rep0, "Detalle de Consumo de Bolsa");
                    break;
                #region Reporte Qualtia por Centro de costos
                case "RepMatQualtiaPorCenCosN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Consumo por centro de costos");
                    RepMatQualtiaPorCenCosNv1(Rep0, "Consumo por centro de costos");
                    break;
                case "RepMatQualtiaPorCenCosN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepMatQualtiaPorCenCosNv2(Rep0, "Consumo por empleado");
                    break;
                case "RepMatQualtiaPorCenCosN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    RepMatQualtiaPorCenCosNv3(Rep0, "Detallado");
                    break;
                #endregion Reporte Qualtia por Centro de costos

                #region Reporte llamadas perdidas por extension
                case "RepPerdidasPorExtN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Llamadas perdidas por extensión");
                    RepPerdidasPorExtensionNv2(Rep0, "Llamadas perdidas por extensión");
                    break;
                case "RepPerdidasPorExtN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Detalle extensión");
                    RepPerdidasPorExtensionNv3(Rep0, "Detalle");
                    break;
                #endregion Reporte llamadas perdidas por extension
                case "RepDesviosTipoDestinoN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Desvíos por tipo de destino");
                    RepDesviosPorTipoDestino2pnl(Rep2, 1, "Desvíos por tipo de destino", Rep1, "Desvíos por tipo de destino");
                    break;
                case "RepDesviosTipoDestinoN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    string linkGraf = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extensión) +''&TDest=" + param["TDest"] + "''+''&NumDesvios=1'' '";
                    string linkGrid = Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&TDest=" + param["TDest"] + "&NumDesvios=1";
                    RepDesviosPorTipoDestinoN22pnl(Rep2, 1, "Desvíos por Empleado", Rep1, "Desvíos por Empleado", linkGraf, linkGrid);
                    break;
                case "RepDesviosTipoDestinoN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Detalle por Desvíos");
                    RepDesviosPorTipoDestinoN3(Rep0, "Detalle por Desvíos");
                    break;
                case "RepDesviosPorExtensionN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Desvíos por Extensión");
                    string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&NumDesvios=2'' '";
                    RepDesviosPorExtension2Pnl(Rep2, 1, "Desvíos por Extensión", Rep1, "Desvíos por Extensión", link);
                    break;
                case "RepDesviosCenCostoN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Desvíos por Centro de Costos");
                    RepDesviosPorCencosto2Pnl(Rep2, 1, "Desvíos por Centro de Costos", Rep1, "Desvíos por Centro de Costos");
                    break;
                case "RepDesviosCenCostoN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepDesviosPorCencostoN2(Rep2, 1, "Desvíos por Empleado", Rep1, "Desvíos por Empleado");
                    break;
                case "RepDesviosSitioN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    string linkGra = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension)+''&NumDesvios=2'' '";
                    string linkGri = Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&NumDesvios=2";
                    RepDesviosPorTipoDestinoN22pnl(Rep2, 1, "Desvíos por Empleado", Rep1, "Desvíos por Empleado", linkGra, linkGri);
                    break;
                case "RepDesviosSitioN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Desvíos por Sitio");
                    RepDesviosPorSitios2Pnl(Rep2, 1, "Desvíos por Sitio", Rep1, "Desvíos por Sitio");
                    break;
                #region Reportes desvios
                case "RepDesviosPorHoraN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Reporte de desvíos por hora");
                    RepDesviosPorHoraN1(Rep2, 0, "Desvíos por hora", Rep1, "Desvíos por hora");
                    break;
                case "RepDesviosPorHoraN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Detalle " + param["Hora"] + ":00 - " + param["Hora"] + ":59");
                    RepDesviosPorHoraN2(Rep0, "Detalle de llamadas de desvío por hora");
                    break;
                case "RepDesviosTop10LlamadasMN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Llamadas de desvío con mayor duración");
                    RepDesviosTop10LlamadasN1(Rep2, 0, "Top 10", Rep1, "Top 10", "Minutos");
                    break;
                case "RepDesviosTop10LlamadasGN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Llamadas de desvío con mayor gasto");
                    RepDesviosTop10LlamadasN1(Rep2, 0, "Top 10", Rep1, "Top 10", "Gasto");
                    break;
                case "RepDesviosTop10ExtN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Extensiones con mayor cantidad de llamadas de desvío");
                    RepDesviosTop10ExtN1(Rep2, 0, "Top 10", Rep1, "Top 10");
                    break;
                case "RepDesviosTop10ExtN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Extensión " + param["Extension"]);
                    RepDesviosTop10ExtN2(Rep0, "Detalle de llamadas de desvío por extensión");
                    break;
                case "RepDesviosPerdidosPorEmpleN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Extensiones con llamadas de desvío no contestadas");
                    RepDesviosPerdidosPorEmpleN1(Rep2, 0, "Llamadas de desvío no contestadas", Rep1, "Llamadas de desvío no contestadas");
                    break;
                case "RepDesviosPerdidosPorEmpleN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Extensión " + param["Extension"]);
                    RepDesviosPerdidosPorEmpleN2(Rep0, "Detalle de llamadas de desvío no contestadas por extensión");
                    break;
                case "RepDesviosPorCarrierN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Reporte de desvíos por carrier");
                    RepDesviosPorCarrierN1(Rep2, 0, "Desvíos por carrier", Rep1, "Desvíos por carrier");
                    break;
                case "RepExtensionessinActBD":
                    TituloNavegacion = "Extensiones sin Actividad";
                    RepExtensionessinActVSBD1pnl(Rep0, "Extensiones sin Actividad");
                    break;
                case "RepTraficoExtensiones1pnl":
                    TituloNavegacion = "Tráfico de Extensiones";
                    RepTraficoExtensiones1pnl(Rep0, "Tráfico de Extensiones");
                    RepExtensionessinActVSBD1pnl(Rep9, "Extensiones sin Actividad");
                    break;
                #endregion Reporte desvios
                #region Reporte por dispositivo IKUSI
                case "RepLlamadasPorDispositivoN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Reporte de llamadas por dispositivo");
                    RepLlamadasPorDispositivoN1(Rep2, 4, "Llamadas por dispositivo", Rep1, "Llamadas por dispositivo");
                    break;
                case "RepLlamadasPorDispositivoN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Dispositivo: " + param["TipoDisp"]);
                    RepLlamadasPorDispositivoN2(Rep2, 2, "Llamadas por dispositivo", Rep1, "Llamadas por dispositivo");
                    break;
                case "RepLlamadasPorDispositivoN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Extensión: " + param["Extension"]);
                    RepLlamadasPorDispositivoN3(Rep0, "Detalle de llamadas por extensión");
                    break;
                #endregion Reporte por dispositivo IKUSI
                case "RepLlamadasPerdidasPorTDestN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("LLamadas Perdidas Por Tipo Destino");
                    RepTabPorTdestPerdidas2Pnl(Rep2, 1, "LLamadas Perdidas Por Tipo Destino", Rep1, "LLamadas Perdidas Por Tipo Destino");
                    break;
                case "RepLlamadasPerdidasPorTDestN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["TDest"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                case "RepLlamadasPerdidasPorSitioN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("LLamadas Perdidas Por Sitio");
                    RepTabPorSitioPerdidas2Pnl(Rep2, 1, "LLamadas Perdidas Por Sitio", Rep1, "LLamadas Perdidas Por Sitio");
                    break;
                case "RepLlamadasPerdidasPorSitioN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                case "RepLlamadasPerdidasPorTopEmpleN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Empleado con mas Llamadas");
                    RepTabTop10EmplePerdidas2pnl(Rep2, 1, "Empleado con mas Llamadas", Rep1, "Empleado con mas Llamadas");
                    break;
                case "RepLlamadasPerdidasPorTopEmpleN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                case "RepLlamadasPerdidasPorCencosN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Llamadas por Centro de Costos");
                    RepTabPorCenCosPerdidas2pnlN1(Rep2, 1, "Empleado con mas Llamadas", Rep1, "Empleado con mas Llamadas");
                    break;
                case "RepLlamadasPerdidasPorCencosN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepTabPorCenCosPerdidas2pnl(Rep2, 1, "Llamadas Perdidas Por Empleado", Rep1, "Llamadas Perdidas Por Empleado");
                    break;
                case "RepLlamadasPerdidasPorCencosN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"] + param["Emple"]);
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;
                case "RepLlamadasAgrupadasEmpleN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("llamadas por Empleado");
                    ReptabLlamadasAgrupEmpleN1(Rep2, 2, "LLamadas por Empleado", Rep1, "LLamadas por Empleado");
                    break;
                case "RepLlamadasAgrupadasEmpleN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Emple"]);
                    ReptabLlamadasAgrupEmpleN2(Rep0, "LLamadas por Empleado");
                    break;
                case "RepColabDirectos":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Consumo colaboradores directos");
                    RepTabEmpleporCencos(Rep0, "Consolidado por Empleado");
                    break;
                case "RepConsumoporDeptosN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Consumo por Departamento");
                    RepTabConsumoporDeptosN1(Rep0, "Consolidado por departamento");
                    break;
                case "RepConsumoporDeptosN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["CenCos"]);
                    RepTabConsumoporDeptosN2(Rep0, "Consolidado por Empleado");
                    break;
                case "RepTabVariacionRentaLineas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("VariacionRenta(Lineas)");
                    RepTabVariacionRentaLineas(Rep0, "Variacion en Renta de Lineas");
                    break;
                case "RepCantContestadasYNoContestadas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Cantidad llamadas recibidas y no contestadas" });
                    RepCantContestadasYNoContestadas(Rep0, "Llamadas que fueron recibidas, no fueron contestadas, o fueron devueltas", 2);
                    break;
                case "RepTabPorExtensionPI":
                    TituloNavegacion = "Tráfico por extensión por identificar";
                    //linkGrid = Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&TDest=" + param["TDest"] + "&NumDesvios=1";
                    //linkGraf = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extensión) +''&TDest=" + param["TDest"] + "''+''&NumDesvios=1'' '";

                    linkGrid = "";
                    linkGraf = "";

                    //RepTabPorExtensionPI1Pnl(Rep0, "Tráfico por extensión por identificar", 1);

                    RepTabPorExtensionPI2Pnls(linkGrid, linkGraf, Rep2, 1, "Tráfico por extensión por identificar", Rep1, "Tráfico por extensión por identificar");
                    break;
                case "DetallePorExtPI":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Extension"]);
                    RepDetalleDesdePorExtension(Rep0, "Detalle de llamadas");
                    break;
                case "RepTabPorExtension":
                    TituloNavegacion = "Consumo por extensión";
                    RepTabPorExtension2Pnls(Rep2, 0, "Consumo por extensión", Rep1, "Consumo por extensión");
                    break;
                case "DetalleHospitales":
                    TituloNavegacion = "Detalle de llamadas";
                    RepDetalleHospitales(Rep0, "Detalle de llamadas");
                    break;
                case "RepTabContestadasYNoPorSitio":
                    TituloNavegacion = "Tráfico por sitio";

                    //linkGrid = Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&TDest=" + param["TDest"] + "&NumDesvios=1";
                    //linkGraf = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extensión) +''&TDest=" + param["TDest"] + "''+''&NumDesvios=1'' '";

                    linkGrid = "";
                    linkGraf = "";

                    //RepTabPorExtensionPI1Pnl(Rep0, "Tráfico por extensión por identificar", 1);

                    RepTabTraficoContestadasYNoPorSitio2Pnls(linkGrid, linkGraf, Rep2, 1, "Tráfico por sitio", Rep1, "Tráfico por sitio");
                    break;
                case "RepTabContestadasYNoUnSitioPorExten":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(param["Sitio"]);

                    //linkGrid = Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&TDest=" + param["TDest"] + "&NumDesvios=1";
                    //linkGraf = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extensión) +''&TDest=" + param["TDest"] + "''+''&NumDesvios=1'' '";

                    linkGrid = "";
                    linkGraf = "";

                    //RepTabPorExtensionPI1Pnl(Rep0, "Tráfico por extensión por identificar", 1);

                    RepTabTraficoContestadasYNoUnSitioPorExt2Pnls(linkGrid, linkGraf, Rep2, 1, "Tráfico por extensión", Rep1, "Tráfico por extensión");
                    break;
                case "RepTabParticipantesvsReunionesMes":
                    TituloNavegacion = "Participantes / reunión";

                    linkGrid = "";
                    linkGraf = "";

                    RepTabParticipantesvsReunionesMes(Rep1 , TituloNavegacion, Rep2, TituloNavegacion, 0);
                    break;
                case "RepTabParticipantesvsReunionesMes1":
                    TituloNavegacion = "Participantes / reunión";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabParticipantesvsReunionesMes1(Rep1, TituloNavegacion, 0);
                    break;
                case "RepTabParticipantesvsHorasMes":
                    TituloNavegacion = "Tiempo dedicado a reuniones";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabParticipantesvsHorasMes(Rep1, TituloNavegacion, Rep2, TituloNavegacion, 0);
                    break;

                case "RepTabParticipantesvsHorasMes1":
                    TituloNavegacion = "Tiempo dedicado a reuniones";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabParticipantesvsHorasMes1(Rep1, TituloNavegacion, 0);
                    break;
                case "RepTabReunionesvsSemana":
                    TituloNavegacion = "Hora pico";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabReunionesvsSemana(Rep1, TituloNavegacion, Rep2, TituloNavegacion, 0);
                    break;
                case "RepTabReunionesvsSemana1":
                    TituloNavegacion = "Hora pico";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabReunionesvsSemana1(Rep1, TituloNavegacion, 0);
                    break;
                case "RepTabNoParticipantes":
                    TituloNavegacion = "Usuarios sin usar google meet";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabUsuariossinutilizacion(Rep1, TituloNavegacion, Rep2, TituloNavegacion, 0);
                    break;
                case "RepTabNoParticipantes1":
                    TituloNavegacion = "Usuarios sin usar google meet";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabUsuariossinutilizacion1(Rep1, TituloNavegacion, 0);
                    break;
                case "RepTabPlataformaMeet":
                    TituloNavegacion = "Uso de plataforma";

                    linkGrid = "";
                    linkGraf = "";
                    RepTabPlataformaMeet(Rep1, TituloNavegacion, Rep2, TituloNavegacion, 0);
                    break;
                case "RepTabPlataformaMeetPersonas":
                    TituloNavegacion = "Uso de plataforma";
                    param["Extension"].ToString();
                    linkGrid = "";
                    linkGraf = "";
                    RepTabPlataformaMeetPersonas(Rep1, TituloNavegacion, Rep2, TituloNavegacion, 0);
                    break;
                    
                // TODO : DO Paso 4 - Agregar Navegacion de reporte  
                default:
                    break;
            }
        }

        #endregion

    }
}