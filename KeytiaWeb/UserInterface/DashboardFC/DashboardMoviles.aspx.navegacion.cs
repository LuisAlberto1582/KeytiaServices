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
using System.Web.Services;
using System.Globalization;
using KeytiaWeb.UserInterface.Indicadores;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DashboardMoviles
    {
        #region Logica de navegación

        bool ValidaConsultaFechasBD()
        {
            bool consultarBD = false;
            if (string.IsNullOrEmpty(param["Nav"]))
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
            param.Clear();
            param.Add("Nav", string.Empty);
            param.Add("Sitio", string.Empty);
            param.Add("TDest", string.Empty);
            param.Add("Emple", string.Empty);
            param.Add("CenCos", string.Empty);
            param.Add("TipoLlam", string.Empty);
            param.Add("Concepto", string.Empty);
            param.Add("Exten", string.Empty);
            param.Add("Carrier", string.Empty);
            param.Add("NumMarc", string.Empty);
            param.Add("Linea", string.Empty);
            param.Add("NumGpoTronk", string.Empty);
            param.Add("CtaMae", string.Empty);
            param.Add("EqCelular", string.Empty);
            param.Add("Tel", string.Empty);
            param.Add("MesAnio", string.Empty);
            param.Add("MiConsumo", string.Empty);
            param.Add("Level", string.Empty);
            param.Add("TipoConsumo", string.Empty);
            
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
            string planEmpleado = string.Empty;
            string tipoConsumoInternet = (!string.IsNullOrWhiteSpace(param["TipoConsumo"]) && param["TipoConsumo"].ToLower() == "nac") ? " Nacional" : " Internacional";



            lblHeaderAltReporte.Text = "";
            switch (param["Nav"])
            {
                #region Reporte Lineas Inactivas Sin Importes

                case "LineasInactivasSinImporte":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Lineas sin actividad" });
                    RepLineasInactivasCantidad(Rep1, "Total mensual de lineas inactivas", 2);
                    RepLineasInactivasSinImportes(Rep2, "Lineas con inactividad por mes", 2);
                    break;

                #endregion

                #region Reporte Lineas Por Plan Base

                case "PlanBaseLinea":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Lineas por Plan Base" });
                    RepLineasPorPlanBase(Rep2, "Lineas por Plan Base", Rep1, "Lineas por Plan Base", 1);
                    break;

                #endregion

                #region Reporte Indicador Lineas Inactivas

                case "LineasInactivas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Lineas Inactivas" });
                    RepLineasInactivas(Rep1, "Lineas inactivas", 2);
                    break;

                #endregion

                #region Navegaciones Reporte historico
                case "HistoricoN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo histórico" });
                    RepTabHistMov2Pnls(Rep2, "Consumo histórico", Rep1, "Consumo histórico", 0);
                    break;

                #endregion Navegaciones Reporte historico

                #region Navegaciones Reporte X CenCos

                case "CenCosN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Costo por área" });
                    RepTabPorCenCosMov2Pnls(Request.Path + "?Nav=CarrierCCN2&CenCos={0}",
                        "[link] = ''" + Request.Path + "?Nav=CarrierCCN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])",
                        Rep2, "Consumo por área", Rep1, "Consumo por área", 2);

                    break;

                case "CenCosN2":
                    if (lsCvePerfil == "SupervisorDet" || lsCvePerfil == "Config")
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] });
                        ReporteXEmpleado("?Nav=CenCosN4&Emple={0}&Linea={1}&CenCos=" + param["CenCos"]);
                        RepTabPorEmpleAcumMov2Pnls(Rep4, "Consumo Por Colaborador Acumulado", Rep3, "Consumo Por Colaborador Acumulado");
                    }
                    else
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] });
                        ReporteXEmpleado("?Nav=CenCosN3&Emple={0}&Linea={1}&CenCos=" + param["CenCos"] + "&Carrier=" + ((param["Carrier"] != null && param["Carrier"].Length > 0) ? param["Carrier"] : "0") + "");
                        RepTabPorEmpleAcumMov2Pnls(Rep4, "Consumo Por Colaborador Acumulado", Rep3, "Consumo Por Colaborador Acumulado");
                    }
                    break;

                case "CenCosN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"] });
                    planEmpleado = DSODataAccess.Execute(ConsultaPlanEmpleado()).Rows[0][0].ToString();
                    if (planEmpleado.Length > 0)
                    {
                        lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
                        lblHeaderAltReporte.Text = planEmpleado;
                    }
                    ReporteXConcepto(Rep1);
                    ReporteHistorico2SeriesEnRep2(2);
                    RepHistMinutosUtilizados(Rep9);
                    ReporteConsumoHistoricoInternet();

                    break;

                case "CenCosN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"] });

                    planEmpleado = DSODataAccess.Execute(ConsultaPlanEmpleado()).Rows[0][0].ToString();
                    planEmpleado = "Linea: (" + param["Linea"].ToString() + ")   " + planEmpleado;

                    if (planEmpleado.Length > 0)
                    {
                        lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
                        lblHeaderAltReporte.Text = planEmpleado;
                    }

                    ReporteXConceptoConNav(Rep1, Request.Path + "?Nav=CenCosN5&Concepto={0}" + "&CenCos=" + param["CenCos"] + "&Carrier=" + param["Carrier"] + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"], "Por concepto");

                    //RJ.20150210
                    ReporteHistorico2SeriesEnRep2(2);
                    RepHistMinutosUtilizados(Rep9);
                    ReporteConsumoHistoricoInternet();

                    break;

                case "CenCosN5":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Concepto"] });

                    planEmpleado = DSODataAccess.Execute(ConsultaPlanEmpleado()).Rows[0][0].ToString();
                    planEmpleado = "Linea: (" + param["Linea"].ToString() + ")   " + planEmpleado;

                    if (planEmpleado.Length > 0)
                    {
                        lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
                        lblHeaderAltReporte.Text = planEmpleado;
                    }

                    ReporteDetFacturaXEmpleado();
                    break;

                #endregion Navegaciones Reporte X CenCos

                #region Navegaciones Reporte X Empleado

                case "EmpleN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Costo por colaborador" });
                    RepTabPorEmpleMov2Pnls(
                        Rep2, "Consumo por colaborador", Rep1, "Consumo por colaborador", 2);
                    break;


                //20141024 AM. Se agrega el reporte por concepto con navegacion a detalle
                case "EmpleN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"], param["Tel"] });  //BG. 20150302 Agregue parametro Telefono
                    if (lsCvePerfil == "SupervisorDet" || lsCvePerfil == "Config" || lsCvePerfil == "SupervisorDetAvan" ||               //NZ 20160520 Se agrega el perfil: SupervisorDetAvan (Nemak)
                                                                                                                                         //20150113 AM. Se agrega validacion para bimbo, el perfil administrador ve el reporte por concepto con nav a detalle
                        (DSODataContext.Schema.ToLower().Trim() == "bimbo" && Session["iCodPerfil"].ToString() == "369"))
                    {
                        //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                        int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                        if (numeroDeLineasTelcel > 1)
                        {
                            ReportePorLineas(Rep0, Request.Path + "?Nav=EmpleN3&Linea=" + param["Tel"] + "&Emple=" + param["Emple"], "Consumo por línea"
                            , "373"); //Telcel
                        }
                        else
                        {
                            ConsultaLineaTelcel();
                            TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"], param["Linea"] });
                            ReporteXConceptoConNav(Rep0,
                              Request.Path + "?Nav=EmpleN4&Concepto={0}" + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"] + "&Carrier={1}", "Por concepto");
                            //ReporteXConceptoConNav(Rep0,
                            //    Request.Path + "?Nav=EmpleN4&Concepto={0}" + "&Emple=" + param["Emple"] + "&Linea=" + param["Linea"] + "&Carrier=" + ((param["Carrier"] != null && param["Carrier"].Length > 0) ? param["Carrier"] : "0"),
                            //               "Por concepto");
                        }
                    }
                    else
                    {
                        //20160908 NZ Se agrega la siguiente linea para cuando el empleado tenga más de una Linea
                        //Se necesita por que el SP que manda la consulta por esta navegacion revisa la variable Linea
                        param["Linea"] = param["Tel"];
                        ReporteXConcepto(Rep1);

                        ReporteHistoricoDelEmpleado2Series(Rep2);

                        if (lsCvePerfil == "Sup" || lsCvePerfil == "Admin")
                        {
                            RepHistMinutosUtilizados(Rep9);
                            ReporteConsumoHistoricoInternet();
                        }
                    }
                    break;

                case "EmpleN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Linea"] });
                    ReporteXConceptoConNav(Rep0, Request.Path + "?Nav=EmpleN4&Concepto={0}" + "&Emple=" + param["Emple"] + "&Carrier={1}" + "&Linea=" + param["Linea"], "Por concepto");
                    break;
                case "EmpleN4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Concepto"] });
                    ReporteDetFacturaXEmpleado();
                    break;

                #endregion Navegaciones Reporte X Empleado

                #region Navegaciones Reporte resumen por conceptos telcel

                case "ResumenPorConcepTelcelN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Resumen por conceptos" });
                    RepTabResumenPorConceptoTelcelMov2Pnls(Rep2, "Consumo por concepto", Rep1, "Consumo por concepto", 2);
                    break;

                #endregion Navegaciones Reporte resumen por conceptos telcel

                #region Navegaciones Reporte resumen por planes

                case "ResumenPorPlanesN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Resumen por planes" });
                    RepTabResumenPorPlanesMov2Pnls(Rep2, "Resumen por planes", Rep1, "Resumen por planes", 2);
                    break;

                #endregion Navegaciones Reporte resumen por planes

                #region Navegaciones Reporte top lineas mas caras
                case "TopLineasMasCarasN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Top líneas más caras" });
                    if (txtCantMaxReg.Text.Length == 0) { txtCantMaxReg.Text = "10"; pnlCantMaxReg.Visible = true; }
                    RepTabTopLinMasCarasMov2Pnls(Request.Path + "?Nav=TopLineasMasCarasN2&Linea={0}&Carrier={1}",
                        "[link] = ''" + Request.Path + "?Nav=TopLineasMasCarasN2&Linea=" + "'' + convert(varchar,[Extension]) + ''&Carrier='' + convert(varchar,[idCarrier])",
                        Rep2, "Top líneas más caras", Rep1, "Top líneas más caras", 2);

                    break;
                case "TopLineasMasCarasN2":
                    //TODO : AAAAAAAAA realizar exportacion de reporte
                    //btnExportarXLS.Visible = false;
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Linea"] });
                    StringBuilder consultaEmpleadoEnBaseALinea = new StringBuilder();
                    consultaEmpleadoEnBaseALinea.Append("select isNull(Emple, 0) as Emple  \r");
                    consultaEmpleadoEnBaseALinea.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas  \r");
                    consultaEmpleadoEnBaseALinea.Append("where dtinivigencia <> dtfinvigencia  \r");
                    consultaEmpleadoEnBaseALinea.Append("and dtfinvigencia >= getdate()  \r");
                    consultaEmpleadoEnBaseALinea.Append("and vchCodigo = '" + param["Linea"] + "' \r");
                    param["Emple"] = DSODataAccess.ExecuteScalar(consultaEmpleadoEnBaseALinea.ToString()).ToString();
                    ReporteXConceptoConNav(Rep0,
                        Request.Path + "?Nav=TopLineasMasCarasN3&Concepto={0}" + "&Linea=" + param["Linea"] + "&Emple=" + param["Emple"] + "&Carrier={1}", "Por concepto");
                    //ReporteXConceptoConNav(Rep0,
                    //    Request.Path + "?Nav=TopLineasMasCarasN3&Concepto={0}" + "&Linea=" + param["Linea"] + "&Emple=" + param["Emple"] + "&Carrier=" + (string.IsNullOrEmpty(param["Carrier"]) ? "0" : param["Carrier"]),
                    //                        "Por concepto");

                    break;
                case "TopLineasMasCarasN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Concepto"] });
                    ReporteDetFacturaXEmpleado();

                    break;
                #endregion Navegaciones Reporte top lineas mas caras

                #region Navegaciones Reporte Historico X CenCos

                case "HistXCenCosN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Gasto histórico por centro de costos" });
                    RepMatPorCenCosMov2Pnls(Rep2, "Histórico por centro de costos", Rep1, "Histórico por centro de costos");
                    break;

                #endregion Navegaciones Reporte Historico X CenCos

                #region Navegaciones Reporte Historico X Empleado

                case "HistXEmpleN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo histórico por colaborador" });
                    RepMatPorEmpleMov2Pnls(Rep2, "Histórico por colaborador", Rep1, "Histórico por colaborador");
                    break;

                #endregion Navegaciones Reporte Historico X Empleado

                #region Navegaciones Reporte resumen por equipos

                case "ResumPorEquipoN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Resumen por equipos" });
                    RepTabResumenPorEquiposMov2Pnls(Rep2, "Resumen por equipos", Rep1, "Resumen por equipos");
                    break;

                #endregion Navegaciones Reporte resumen por equipos

                #region Navegaciones Reporte historico año anterior vs actual

                case "HistoricoAnioActVsAntN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Resumen por Equipos" });
                    RepTabHistAnioAnteriorVsActualMov2Pnls(Rep2, "Consumo histórico móviles año actual vs anterior",
                        Rep1, "Consumo histórico móviles año actual vs anterior");
                    break;

                #endregion Navegaciones Reporte historico año anterior vs actual

                #region Reporte Detallado de Llamadas Telcel F4

                case "RepDetalladoLlamsTelcelF4":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detallado de Llamadas Telcel" });
                    RepTabDetalleLlamsF4Telcel1Pnl(Rep0, "Detallado de Llamadas Telcel");
                    break;


                #endregion Reporte Detallado de Llamadas Telcel F4

                #region Navegaciones Reporte Consumo Por Empleado Acumulado

                case "EmpleAcumN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo Por Colaborador Acumulado" });
                    RepTabPorEmpleAcumMov2Pnls(Rep2, "Consumo Por Colaborador Acumulado", Rep1, "Consumo Por Colaborador Acumulado");
                    break;

                #endregion Navegaciones Reporte Consumo Por Empleado Acumulado

                #region Navegaciones Reporte Detallado Factuta Telcel

                case "RepTabDetalladoFacturaTelcel":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detallado Factura Telcel" });
                    RepTabDetalladoFacturaTelcel(Rep2, "Detallado Factura Telcel", Rep0, "Detallado Factura Telcel");
                    break;

                #endregion Navegaciones Reporte Detallado Factuta Telcel

                #region Reportes Varios
                case "DireccionJerarq":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Costo por área" });
                    RepTabPorCostoAreaDireccion2Pnls(
                        Rep2, "Consumo por área (Direcciones)", Rep1, "Consumo por área (Direcciones)");

                    break;

                case "SubDireccion":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Costo por área (SubDirecciones)" });
                    RepTabPorCostoAreaSubDireccion2Pnls(Request.Path + "?Nav=EmpleadosN2&Cencos={0}",
                        "[link] = ''" + Request.Path + "?Nav=EmpleadosN2&Cencos='' + convert(varchar,[idSubDireccion])",
                        Rep2, "Consumo por área (SubDirecciones)", Rep1, "Consumo por área (SubDirecciones)");

                    break;

                case "CarrierCCN2":
                    if (lsCvePerfil == "SupervisorDet" || lsCvePerfil == "Config" || lsCvePerfil == "Supervisor")
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] });
                        RepTabConsumoPorCarrierFiltroCC2Pnls(Rep2, "Consumo por Carrier", Rep1, "Consumo por Carrier",
                        Request.Path + "?Nav=EmpleadosN2&CenCos=" + param["CenCos"] + "&Carrier={0}",
                        "[Link] = ''" + Request.Path + "?Nav=EmpleadosN2&CenCos=" + param["CenCos"] + "&Carrier='' + convert(varchar,[Codigo Carrier])", 2);
                    }
                    else if (lsCvePerfil == "Admin")
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] });
                        RepTabConsumoPorCarrierFiltroCC2Pnls(Rep2, "Consumo por Carrier", Rep1, "Consumo por Carrier",
                        Request.Path + "?Nav=EmpleN1&Carrier={0}&CenCos=" + ((param["CenCos"] != null && param["CenCos"].Length > 0) ? param["CenCos"] : "0"),
                        "[Link] = ''" + Request.Path + "?Nav=EmpleN1&Carrier= " + ((param["Carrier"] != null && param["Carrier"].Length > 0) ? param["Carrier"] : "0") + "''", 2);
                    }
                    else if (lsCvePerfil == "Sup")
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] });
                        RepTabConsumoPorCarrierFiltroCC2Pnls(Rep2, "Consumo por Carrier", Rep1, "Consumo por Carrier",
                            Request.Path + "?Nav=CenCosN2&CenCos=" + ((param["CenCos"] != null && param["CenCos"].Length > 0) ? param["CenCos"] : "0") + "&Carrier={0}",
                        "[Link] = ''" + Request.Path + "?Nav=&CenCos=" + ((param["CenCos"] != null && param["CenCos"].Length > 0) ? param["CenCos"] : "0") + "&Carrier= " + ((param["Carrier"] != null && param["Carrier"].Length > 0) ? param["Carrier"] : "0") + "''", 2);
                    }
                    break;

                case "EmpleadosN2":
                    if (lsCvePerfil == "SupervisorDet" || lsCvePerfil == "Config")
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] });
                        RepTabConsumoEmpleCC2Pnls("?Nav=CenCosN4&Emple={0}&Linea={1}&CenCos=" + param["CenCos"] + "&Carrier={2}", 1);
                    }
                    else
                    {
                        TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] });
                        RepTabConsumoEmpleCC2Pnls("?Nav=CenCosN3&Emple={0}&Linea={1}&CenCos=" + param["CenCos"] + "&Carrier={2}", 1);
                    }
                    break;

                case "ConsumoPorCtaMaestra":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes de Moviles", "Cargos Telcel" });
                    RepTabConsumoPorCtaMaestra2Pnls(Request.Path + "?Nav=LineaTelcelCtaMaeN2&CtaMae={0}",
                        "[link] = ''" + Request.Path + "?Nav=LineaTelcelCtaMaeN2&CtaMae='' + convert(varchar,[idCuentaPadre])",
                        Rep2, "Cargos Telcel", Rep1, "Cargos Telcel");
                    break;

                case "LineaTelcelCtaMaeN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo Por Linea" });
                    RepTabConsumoLineaTelcelCtaMaestra1Pnl(Rep0, "Consumo Por Linea");
                    break;

                case "ConsumoMovilPorTecno":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Moviles por Tecnologia" });
                    RepTabConsumoMovilesPorTecnologia2Pnls(Rep2, "Consumo De Moviles por Tecnologia",
                        Rep1, "Consumo De Moviles por Tecnologia");
                    break;

                case "ReporteGlobalTelMovil":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reporte Global Telefonía Móvil" });
                    ReporteGlobalTelMovil1Pnl(Rep0, "Reporte Global Telefonía Móvil");
                    break;
                #endregion

                #region Navegaciones Reporte historico (perfil emple)

                case "HistoricoPeEmN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo histórico" });
                    ReporteHistoricoPeEmple(Rep2, "Consumo histórico", Rep1, "Consumo histórico");
                    break;

                #endregion Navegaciones Reporte historico (perfil emple)

                #region Navegaciones Reporte llamadas a celulares red

                case "LlamACelRedN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Llamadas a celulares de la red" });
                    RepTabLlamACeluDeLaRedMov2Pnls(Request.Path + "?Nav=LlamACelRedN2&Exten={0}",
                        "[link] = ''" + Request.Path + "?Nav=LlamACelRedN2&Exten='' + convert(varchar,[Extension])",
                        Rep2, "Llamadas a celulares dentro de la red", Rep1, "Llamadas a celulares dentro de la red");
                    break;

                case "LlamACelRedN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Exten"] });
                    ReporteNumMarcACelDeLaRed();
                    break;

                case "LlamACelRedN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["NumMarc"] });
                    ReporteDetalladoPeEmple();
                    break;
                #endregion Navegaciones Reporte llamadas a celulares red

                #region Navegaciones Reporte costo por empleado (perfil emple)

                case "CostoXEmpleN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Inicio", "Costo por colaborador" });
                    ReporteXEmpleadoSinNav();
                    break;

                #endregion Navegaciones Reporte costo por empleado (perfil emple)

                #region Navegaciones Reporte Detalle de factura (perfil emple)

                case "DetFacPEmN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle de factura telcel" });
                    ReporteDetFacturaEnRep1();
                    ReporteHistoricoPeEmple();
                    break;

                case "DetFacPEmN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Concepto"] });
                    ReporteDetFacturaXEmpleado();
                    break;

                #endregion Navegaciones Reporte Detalle de factura (perfil emple)

                #region Navegaciones Reporte Llamadas mas largas (perfil emple)

                case "LlamMasLargN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Llamadas más largas - móviles" });
                    if (txtCantMaxReg.Text.Length == 0) { txtCantMaxReg.Text = "10"; pnlCantMaxReg.Visible = true; }
                    ReporteLlamadasMasLargas();
                    break;

                #endregion Navegaciones Reporte Llamadas mas largas (perfil emple)

                #region Navegaciones Reporte Llamadas mas costosas (perfil emple)

                case "LlamMasCostN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Llamadas más costosas" });
                    if (txtCantMaxReg.Text.Length == 0) { txtCantMaxReg.Text = "10"; pnlCantMaxReg.Visible = true; }
                    ReporteLlamadasMasCostosas();
                    break;

                #endregion Navegaciones Reporte Llamadas mas costosas (perfil emple)

                #region Navegaciones Reporte Consumo de Internet por Colaborador //NZ 20150327

                case "DetConsCol":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle del consumo de Internet", param["Exten"] });
                    ReporteConsumoHistoricoInternet();
                    break;

                case "MiConsumoInternet":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle del consumo de Internet" });
                    ReporteConsumoInternetPorColaborador(Rep6);
                    break;


                case "MiConsumoInternet2Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle del consumo de Internet" });
                    ReporteConsumoInternetPorColaborador2Pnl(Rep6);
                    break;

                #endregion

                #region Navegaciones Reporte Consumo Historico Por Area (Menu Reportes) //BG.20150330

                case "RepMat":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes", "Consumo Histórico Por Área" });
                    RepMatHistPorCenCosAnioAntMov1Pnl();  /*Año Anterior */
                    RepMatHistPorCenCosAnioActMov1Pnl();  /* Año Actual  */
                    break;

                case "RepMatPorEmple":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes", "Consumo Histórico Por Colaborador" });
                    RepMatHistPorEmpleAnioAntMov1Pnl(Rep9);   /*Año Anterior */
                    RepMatHistPorEmpleAnioActMov1Pnl(Rep0);   /* Año Actual  */
                    btnExportarXLS.Visible = false;
                    break;

                #endregion

                #region Reporte Consumo Por Empleado Acumulado

                case "RepEmpleAcum":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo Por Colaborador Acumulado" });
                    RepTabPorEmpleAcumMov2Pnls(Rep2, "Consumo Por Colaborador Acumulado", Rep1, "Consumo Por Colaborador Acumulado");

                    RepTabPorEmpleMov2Pnls(Rep4, "Consumo Por Colaborador", Rep3, "Consumo Por Colaborador");

                    RepTabPorEmplePorMesMov1Pnl(Rep9, "Consumo por colaborador mensual");

                    btnExportarXLS.Visible = true;
                    btnRegresar.Visible = false;
                    break;

                #endregion

                #region Navegaciones Reporte Minutos Utilizados (perfil emple) //BG.20150918

                case "MinsUtilizadosN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Minutos Utilizados" });
                    ReporteMinutosUtilizados(Rep1);
                    RepHistMinutosUtilizados(Rep9);
                    break;

                #endregion Navegaciones Reporte Minutos Utilizados (perfil emple)

                #region Grafica Detalle de Llamadas (Minutos Utilizados)

                case "RepDetalleLlamsMinsUtil":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detalle de Llamadas" });
                    RepDetalleLlamsMinsUtil(Rep0);
                    break;

                #endregion

                #region Navegacion Reporte Cencos Jerarquico

                case "CenCosJerarquicoN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Por centro de costos jerárquico" });
                    RepTabCenCosJerarquicoP1(new Control(), "Detalle consumo por centro de costos jerárquico", "Gráfica consumo por centro de costos jerárquico", 0);
                    break;

                case "CenCosJerarquicoN2":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { NavCenCosJer });
                    RepTabCenCosJerarquicoN2("Detalle consumo por centro de costos jerárquico", "Gráfica consumo por centro de costos jerárquico");
                    break;

                case "CenCosJerarquicoN3":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { NavCenCosJer });

                    RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=EmpleMCN2&CenCos=" + param["CenCos"] + "&Emple={0}",
                                                "[link] = ''" + Request.Path + "?Nav=EmpleMCN2&CenCos=" + param["CenCos"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
                    break;
                case "EmpleMCN2":
                    NavCenCosJer = DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacionCenCosJer(param["CenCos"])).ToString();
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"] });
                    ReporteDetallado(Rep0, "Detalle de llamadas");
                    break;

                #endregion Navegacion Reporte Cencos Jerarquico

                #region Navegaciones Reporte historico

                case "HistoricoConCantLineasN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo histórico" });
                    RepTabHistMovCantLineas2Pnls(Rep2, "Cantidad de líneas por mes", Rep1, "Comportamiento histórico móviles");
                    break;

                #endregion Navegaciones Reporte historico

                #region Navegaciones Reportes demo consumo moviles

                case "DemoConsumoMatIntPorDiaPorConc":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Linea"] });
                    DemoConsumoMatIntPorDiaPorConc();
                    break;

                case "DemoTopLineasMasConsumoDatos":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "" });
                    RepTopLineasMasConsumoDatos(Rep6, "Top Lineas Mas Consumo Datos", 2, Request.Path + "?Nav=DemoConsumoMatIntPorDiaPorConc&Linea=");
                    break;

                #endregion Navegaciones Reportes demo consumo moviles

                #region Indicador LineasNoIdent
                case "CantLineasNoIdent":
                    IndicadorLineasNoIdent();
                    break;
                #endregion

                #region Consumo Lineas No Identificadas
                case "ConsumoLineasNoIdent":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo Lineas No Identificadas" });
                    RepTabConsumoLineasNoIdent2Pnls(Rep1, "Consumo lineas no identificadas", Rep2, "Grafica consumo lineas no identificadas");
                    break;
                #endregion

                #region Consumo Lineas No Identificadas Conceptos
                case "ConsumoPorConcepto1Linea1Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Tel"] });
                    RepTabConsumoPorConcepto1Linea1Pnl(Rep0, "Detalle de Factura", param["Tel"], param["Carrier"]);
                    break;
                #endregion

                #region Consumo Lineas Nuevas
                case "ConsumoCantLineasNuevas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo Lineas Nuevas" });
                    RepTabConsumoCantLineasNuevas2Pnls(Rep1, "Consumo lineas Nuevas", Rep2, "Grafica consumo lineas Nuevas");
                    break;
                #endregion

                #region Consumo Lineas Nuevas Conceptos
                case "RepTabConsumoPorConcepto1LineaNueva1Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Tel"] });
                    RepTabConsumoPorConcepto1LineaNueva1Pnl(Rep0, "Detalle de Factura", param["Tel"], param["Carrier"]);
                    break;
                #endregion

                #region Consumo Lineas Sin Actividad
                case "RepTabConsumoLineasSinAct2Pnls":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Lineas Sin Actividad" });
                    RepTabConsumoLineasSinAct2Pnls(Rep1, "Consumo lineas Nuevas", Rep2, "Grafica consumo lineas Nuevas");
                    break;
                #endregion

                #region Indicador CantLineas En el Mes
                case "IndicadorCantLineasEnelMes":

                    IndicadorCantLineasEnElMes();
                    break;

                #endregion

                #region Consumo Lineas No Identificadas
                case "ConsumoLineasEnElMes":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo Lineas Mes Anterior" });
                    RepTabConsumoLineasEnElMes2Pnls(Rep1, "Consumo lineas mes anterior", Rep2, "Grafica consumo lineas mes anterior");
                    break;
                #endregion

                #region Consumo Lineas No Identificadas Conceptos
                case "ConsumoPorConcepto1Linea1PnlMesAnterior":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { param["Tel"] });
                    RepTabConsumoPorConcepto1Linea1PnlMesAnterior(Rep0, "Detalle de Factura", param["Tel"], param["Carrier"]);
                    break;
                #endregion

                case "ConsumoPorCentroDeCostos":
                    #region Reporte Consumo por centro de costos
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo por Centro de Costos" });
                    RepTabConsumoPorCentroDeCostos1Pnl(Rep1, "Consumo por Centro de Costos", 2);

                    #endregion
                    break;

                case "ConsumoPorCentroDeCostos2Pnl":
                    #region Reporte Consumo por centro de costos
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo por Centro de Costos" });
                    RepTabConsumoPorCentroDeCostos2Pnl(Rep1, "Consumo por Centro de Costos", Rep2, "Grafica Consumo por Centro de Costos", 2);

                    #endregion
                    break;

                /*JH*/
                #region Consumo de Datos Por Concepto
                case "RepConsumoDatosNacional2Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Consumo de Datos por Concepto" });
                    RepConsumoDatosNacional2Pnl(Rep1, "Consumo de datos por concepto", Rep2, "Grafica Consumo de Datos por Concepto", Request.Path + "?Nav=RepConsumoDatosNacionalN2&Concepto='+CONVERT(VARCHAR,ClaveConcepto)'");
                    break;
                #endregion

                #region Consumo Acumulado 12 meses por dirección Un año Y Rep de Minutos.
                case "RepMatConsumoAcumHistPorDireccion":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes", "Consumo Histórico por Dirección" });
                    RepMatConsumoAcumHistPorDireccion(Rep0, "Consumo historico de año seleccionado", 2, Convert.ToDateTime(Session["FechaInicio"].ToString()).Year);
                    RepMatConsumoAcumHistPorDireccion(Rep9, "Consumo historico de año anterior al seleccionado", 2, Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1);
                    break;
                #endregion

                #region Reporte Udo de Min

                case "RepUsoDeMinEnLineas":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes", "Consumo de Minutos" });
                    pnlCantMaxReg.Visible = false;
                    if (txtCantMaxReg.Text.Length == 0) { txtCantMaxReg.Text = "100"; pnlCantMaxReg.Visible = true; }
                    RepUsoDeMinEnLineas(Rep0, "Consumo de Minutos");
                    break;
                #endregion

                #region Consumo Acumulado 12 meses por Subdirección Un año Y Rep de Minutos.
                case "RepMatConsumoAcumHistPorSubdireccion":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes", "Consumo Histórico por Subdirección" });
                    RepMatConsumoAcumHistPorSubdireccion(Rep0, "Consumo historico de año seleccionado", 2, Convert.ToDateTime(Session["FechaInicio"].ToString()).Year);
                    //RepMatConsumoAcumHistPorDireccion(Rep9, "Consumo historico de año anterior al seleccionado", 2, Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1);
                    break;
                #endregion

                #region Reporte Uso de Min

                case "RepTabUsoDeMinEnLineas3Meses":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes", "Consumo de Minutos" });
                    pnlCantMaxReg.Visible = false;
                    if (txtCantMaxReg.Text.Length == 0) { txtCantMaxReg.Text = "100"; pnlCantMaxReg.Visible = true; }
                    RepUsoDeMinEnLineas3Meses(Rep0, "Consumo de Minutos");
                    break;


                #endregion

                #region Reporte Consumo de datos 1 Mes Todas Lineas
                case "RepTabConsumoDatosLineas1Mes":

                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reportes", "Consumo de Internet" });
                    pnlCantMaxReg.Visible = false;

                    ConsumoDatosLineas1Mes(Rep0, "Consumo de Internet");
                    break;
                #endregion
                #region Reporte de Llamadas Mas Costosas
                case "LlamadasMasCostosas1Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Llamadas Mas Costosas" });
                    LlamadasMasCostosas(Rep0, "Llamadas Mas Costosas");
                    break;
                #endregion
                #region Reporte de Llamadas mas Largas
                case "LlamMasLargas1Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Llamadas Más Largas" });
                    LlamMasLargas(Rep0, "Llamadas Más Largas");
                    break;
                #endregion
                #region Reporte Lineas Nuevas Telcel
                case "LineasNuevasTelcel2pPnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Lineas Nuevas telcel" });
                    LineasNuevasTelcel(Rep1, "Lineas Nuevas telcel", Rep2, "Lineas Nuevas telcel", "");
                    break;
                #endregion
                #region Lineas Telcel dadas de Baja
                case "LineasTelcelDadasDeBaja1pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Líneas dadas de Baja" });
                    LineasTelcelDadasDeBaja(Rep0, "Líneas dadas de Baja");
                    break;
                case "LineasConGastoMensaje1Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Líneas  con Gasto de Mensajes" });
                    LineasConGastoMensaje(Rep0, "Líneas  con Gasto de Mensajes");
                    break;
                case "DetallaFactTelcel1Pnl":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Detallado de Factura Telcel" });
                    DetallaFactTelcel(Rep0, "Detallado de Factura Telcel");
                    break;
                #endregion

                #region Reporte Consumo por Region.

                case "RepTabConsumoRegion":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Facturación consolidada" });
                    RepTabTotalPorRegion(Rep0, "Facturación consolidada");
                    break;
                #endregion

                #region Reporte Consumo por Cuenta concentradora.

                case "RepTabConsumoCuentaConcentradora":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Distribución por cuenta concentradora" });
                    RepTabImportePorCuentaConcentradora(Rep0, "Distribución por cuenta concentradora");
                    break;
                #endregion

                #region Reporte Consumo por Branch.

                case "RepTabConsumoBranch":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Distribución por Centro de Costos" });
                    RepTabImportePorBranch(Rep0, "Distribución por Centro de Costos");
                    break;
                #endregion

                #region Reporte Consumo por depto.

                case "RepTabConsumoDepto":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Distribución por departamento" });
                    RepTabImportePorDepto(Rep0, "Distribución por departamento");
                    break;
                #endregion


                #region Reporte Consumo por Linea
                case "ReporteConsumoPorLinea":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Líneas sin consumo de datos"});
                    RepTabConsumoPorLinea(Rep0, "Líneas sin consumo de datos");
                    break;
                #endregion

                #region Reporte lineas sin actividad telcel
                case "RepLineasTelcelSinActividad":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta("Reporte de lineas sin actividad Telcel");
                    RepLineasTelcelSinActividad(Rep0, "Lineas sin actividad Telcel");
                    break;
                #endregion Reporte lineas sin actividad telcel

                case "RepMiConsumoMovil":
                    ElegirDashboardMiConsumo();
                    param["Nav"] = string.Empty;
                    TituloNavegacion = string.Empty;
                    break;
                case "RepLineasSinAct3MesesN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Líneas sin actividad en los últimos 3 meses", });
                    RepTabLineasSinAct3MesesN2(Rep0, "Líneas sin actividad en los últimos 3 meses");
                    break;
                case "RepLineasExcedenteInternetN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Líneas que excedieron su límite de internet" });
                    RepLineasExcedenteInternet(Rep0, "Líneas que excedieron su límite de internet");
                    break;
                case "RepLineasExcedenPlanBaseN2":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Líneas que exceden su plan base" });
                    RepLineasExcedenPlanBase(Rep0, "Líneas que exceden su plan base");
                    break;
                case "RepDesgloseTipoExcedenteLineasN3":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Desglose por tipo de excedente" });
                    RepDesgloseTipoExcedente(Rep0, "Desglose por excedente");
                    break;
                case "RepInventarioLineasMoviles":
                    RepTabInventarioLineasMoviles(Rep0, "Inventario Lineas Moviles");
                    break;

                case "DistConsumoIntPorAppN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Distribución consumo internet por app" });
                    DistribucionConsumoInternetPorApp2Pnls(Rep2, "Consumo internet " + tipoConsumoInternet +" por app", Rep1, "Consumo internet " + tipoConsumoInternet + " por app", 0);
                    break;
                case "HistGastoInternetN1":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Histórico gasto internet" });
                    HistoricoGastoInternet12Meses2Pnl(Rep2, "Histórico gasto internet", Rep1, "Histórico gasto internet", 0);
                    break;
                // TODO : DM Paso 4 - Agregar Navegacion de reporte  


                default:
                    break;
            }
        }

        #endregion
    }
}