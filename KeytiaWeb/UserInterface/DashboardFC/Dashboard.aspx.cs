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

    public partial class Dashboard : System.Web.UI.Page
    {
        //coment para validar cambios

        #region Variables de la pagina

        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        static string TituloNavegacion = string.Empty;
        static string WhereAdicional = string.Empty;
        string isFT = string.Empty;


        string campoOrdenamiento = "Total desc";
        int[] arrBoundFieldCols;
        string campoAGraficar = "";
        string numberPrefix = "$ ";
        string numberFormat = "$#,0.00";

        #endregion

        #region Otros campos
        //Se almacenan los parametros que llegan en el QueryString
        static Dictionary<string, string> param = new Dictionary<string, string>();

        Dictionary<string, string> listadoParametros = new Dictionary<string, string>();
        List<CampoReporte> listadoCamposReporte = new List<CampoReporte>();

        #endregion

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

                LeeQueryString();

                if (!Page.IsPostBack && ValidaConsultaFechasBD())
                {
                    Session["OcultarColumnImporte"] = ValidaOcultarColumnImporte(); //[0,1]
                    ValidaOcultarColumnImporteUsuar();

                    if (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")
                    {
                        #region Inicia los valores default de los controles de fecha
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

                #region MesAño en Parametro
                if (param["MesAnio"] != string.Empty)
                {
                    if (!Page.IsPostBack)
                    {
                        AjustaFechas();
                    }
                }

                #endregion

                #region Fechas en sesion

                //Se revisa si las fechas almacenadas en las variables de sesion son iguales a las fechas default que debe tomar el sistema.
                if (Convert.ToDateTime(Session["FechaInicio"]) == Convert.ToDateTime(Session["FechaInicioDefault"]) &&
                    Convert.ToDateTime(Session["FechaFin"]) == Convert.ToDateTime(Session["FechaFinDefault"]))
                {
                    isFT = "1";
                }

                #endregion

                #region Dashboard

                if (param["Nav"] == string.Empty || param["Nav"] == "DashboardSiana" || param["Nav"] == "DashboardDesvios"|| param["Nav"] == "DashboardNoContestadas") // El parámetro DashboardSiana viene desde el URL configurado en una aplicación del menú Dashboard Siana, implementado originalmente para SevenEleven
                {
                    //Se evalua que dashboard debe ver el usuario dependiendo el iCodCatalogo de su perfil  * 370 ---> Empleado
                    if (Session["iCodPerfil"].ToString() == "370")
                    {
                        //DashboardEmpleado();
                        DashboardMultiPerfil();
                        MostrarControlEtiquetacionEmple(); //NZ 20160719 Se agrega link hacia la pagina de etiquetación.
                    }
                    else
                    {
                        //20141217 AM. Se agrega opcion de mi consumo para perfiles diferentes a empleado.
                        if (param["MiConsumo"] == "1")
                        {
                            param["Emple"] = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();
                            if (param["Emple"] != "-1")
                            {
                                DashboardEmpleado();
                                MostrarControlEtiquetacionEmple(); //NZ 20160719 Se agrega link hacia la pagina de etiquetación.
                            }
                            else
                            {
                                Label sinEmpleadoAsignado = new Label
                                {
                                    Text = "Este usuario no cuenta con un empleado asignado."
                                };
                                Rep0.Controls.Add(sinEmpleadoAsignado);
                            }
                        }
                        else
                        {
                            DashboardMultiPerfil();
                        }
                    }

                }
                else { Navegaciones(); }

                #endregion Dashboard

                ConfiguraNavegacion(); //NZ Siempre debe ejecutarce al final
                if (string.IsNullOrEmpty(param["Nav"]))  //Solo en el primer nivel se deben mostrar los indicadores.
                {
                    BuildIndicadores.ConstruirIndicadores(ref pnlIndicadores, "Dashboard.aspx", Request.Path);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }
   
        private void CalculaFechasDeDashboard()
        {
            DataTable fechaMaxCDR = new DataTable();
            DateTime fechaInicioDefault = new DateTime();
            DateTime fechaFinDefault = new DateTime();

            isFT = "0";

            if ((DateTime.Now.Day > Convert.ToInt32(HttpContext.Current.Session["DiaLimiteMesAnt"]))
                || DSODataContext.Schema.ToLower() == "sperto")
            {
                //20141204 AM. Se cambian las fechas default por la fecha maxima de detalleCDR
                fechaMaxCDR = DSODataAccess.Execute(GeneraConsultaMaxFechaInicioCDR());
            }
            else
            {
                //RJ.20160904 Calcula y obtiene el día 1 del mes previo al actual
                DateTime ultimoDiaMesAnt = new DateTime((DateTime.Now.Year), (DateTime.Now.Month), 1).AddDays(-1); //Ultimo día del mes anterior al actual

                fechaMaxCDR.Columns.Add("Anio", typeof(int));
                fechaMaxCDR.Columns.Add("Mes", typeof(int));
                fechaMaxCDR.Columns.Add("Dia", typeof(int));

                fechaMaxCDR.Rows.Add(ultimoDiaMesAnt.Year, ultimoDiaMesAnt.Month, ultimoDiaMesAnt.Day);
            }

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
                    fechaInicio = new DateTime(Anio, Mes, 1);
                    fechaFinal = new DateTime(Anio, Mes, Dia);

                    //RJ.Se utilizan para saber si el usuario ha elegido unas fechas distintas a las default en los reportes
                    fechaInicioDefault = new DateTime(Anio, Mes, 1);
                    fechaFinDefault = new DateTime(Anio, Mes, Dia);

                    // Si el dia de la fecha fin es uno, se calculan las fechas inicio y fin del mes anterior.
                    if (Dia == 1)
                    {
                        fechaInicio = fechaInicio.AddMonths(-1);
                        fechaFinal = fechaFinal.AddDays(-1);

                        //RJ.Se utilizan para saber si el usuario ha elegido unas fechas distintas a las default en los reportes
                        fechaInicioDefault = fechaInicio.AddMonths(-1);
                        fechaFinDefault = fechaFinal.AddDays(-1);
                    }
                }
            }
            else
            {
                // Si en CDR no hay informacion entonces los valores de las fechas se calculan con los
                // valores default de las variables fechaInicio y fechaFinal

                // Si el dia de la fecha fin es uno, se calculan las fechas inicio y fin del mes anterior.
                if (fechaFinal.Day == 1)
                {
                    fechaInicio = fechaInicio.AddMonths(-1);
                    fechaFinal = fechaFinal.AddDays(-1);

                    fechaInicioDefault = fechaInicio.AddMonths(-1);
                    fechaFinDefault = fechaFinal.AddDays(-1);
                }
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");

            //Se revisa si las fechas almacenadas en las variables de sesion son iguales
            //a las fechas default que debe tomar el sistema.
            if (Convert.ToDateTime(Session["FechaInicio"]) == fechaInicioDefault &&
                Convert.ToDateTime(Session["FechaFin"]) == fechaFinDefault)
            {
                isFT = "1";
                Session["FechaInicioDefault"] = fechaInicioDefault;
                Session["FechaFinDefault"] = fechaFinDefault;
            }
        }

        #region Logica de botones de la pagina

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            if (Session["ListaNavegacion"] != null)
            {
                List<MapNav> listaNavegacion = (List<MapNav>)Session["ListaNavegacion"];
                if (listaNavegacion.Count >= 2)
                {
                    listaNavegacion.RemoveAt(listaNavegacion.Count - 1);
                    HttpContext.Current.Response.Redirect(listaNavegacion[listaNavegacion.Count - 1].URL);
                }
                else
                {
                    listaNavegacion.Clear();
                    HttpContext.Current.Response.Redirect(Request.Path);
                }
            }
            else { HttpContext.Current.Response.Redirect(Request.Path); }
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            Session["ExporDetall"] = 1;
            //20150425 RJ.Se agrega condición para exportar el reporte de Capitel en formato Excel 2003
            if (param["Nav"] != "RepTabDetalleParaFinanzas")
            {
                ExportXLS(".xlsx");
            }
            else
            {
                ExportXLS(".xls");
            }

        }

        protected void btnEtiquetacionEmple_Click(object sender, EventArgs e)
        {
            if (DSODataContext.Schema.ToString().ToLower() == "k5banorte")
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/Historicos/Etiquetacion/EtiquetacionEmple.aspx");
            }
            else
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/Historicos/Historicos.aspx?Opc=OpcMiEiqueta");
            }
        }

        private void CrearBotonMiEtiquetacion()
        {
            HtmlButton pbtnEtiquetacion = new HtmlButton();
            pbtnEtiquetacion.ID = "btnEtiquetacion";
            pbtnEtiquetacion.Attributes["class"] = "btn btn-keytia-sm";
            //pbtnEtiquetacion.Style["display"] = "none";
            pbtnEtiquetacion.ServerClick += new EventHandler(pbtnEtiquetacion_ServerClick);
            pbtnEtiquetacion.InnerText = Globals.GetMsgWeb(false, "DashboardBtnEtiquetarNumeros");
            pToolBar.Controls.Add(pbtnEtiquetacion);
        }

        protected void pbtnEtiquetacion_ServerClick(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Historicos/Historicos.aspx?Opc=OpcMiEiqueta");
        }

        private void CrearBotonExportDirectorioSpeedDial()
        {
            HtmlButton pbtnExportDirSpeedDial = new HtmlButton();
            pbtnExportDirSpeedDial.ID = "btnExportDirSpeedDial";
            pbtnExportDirSpeedDial.Attributes["class"] = "btn btn-keytia-sm";
            //pbtnExportDirSpeedDial.Style["display"] = "none";
            pbtnExportDirSpeedDial.ServerClick += new EventHandler(pbtnExportDirSpeedDial_ServerClick);
            pbtnExportDirSpeedDial.InnerText = "Exportar Directorio Speed Dial";
            pToolBar.Controls.Add(pbtnExportDirSpeedDial);
        }

        void pbtnExportDirSpeedDial_ServerClick(object sender, EventArgs e)
        {
            //Se usara la variable Nav para forzar la exportacion de este catalogo aun y cuando en realidad no se encuentre en esa navegación.
            param["Nav"] = "ExportDirSpeedDial";
            ExportXLS(".xlsx");
        }

        #endregion

        public void DashboardEmpleado()
        {
            if (DSODataContext.Schema.ToLower() == "penoles")
            {
                CrearBotonMiEtiquetacion();
                pToolBar.CssClass = "col-md-6 col-sm-6";
            }

            #region Reporte por tipo destino

            //Se obtiene el DataSource del reporte
            DataTable RepPorTDestPeEm = DSODataAccess.Execute(ConsultaPorTipoDestinoPeEm());

            if (RepPorTDestPeEm.Rows.Count > 0 && RepPorTDestPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepPorTDestPeEm);
                RepPorTDestPeEm = dvGrafConsHist.ToTable(false, new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" });
                RepPorTDestPeEm.Columns["Nombre Tipo Destino"].ColumnName = "Tipo destino";
                RepPorTDestPeEm.Columns["Total"].ColumnName = "Total";
                RepPorTDestPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                RepPorTDestPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
            }


            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepPorTDestPeEmGrid_T", RepPorTDestPeEm, true, "Totales",
                                new string[] { "", "", "{0:c}", "", "" }, Request.Path + "?Nav=TDestPeEmN2&TDest={0}",
                                new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                "RepPorTDestPeEmGridRep01_T", "Reporte por tipo de destino", Request.Path + "?Nav=TDestPeEmN1")
                );



            #endregion Reporte por tipo destino

            #region Reporte por tipo de llamada (Grafica)

            if (DSODataContext.Schema != "bimbo")
            {
                DataTable GrafPorTipoLlamPeEm = null;

                #region //20160722 NZ Se agrega esta seccion para cuando entren los esquemas de Evox, Banorte y K5Banorte
                if (DSODataContext.Schema.ToString().ToLower() == "evox"
                                                        || DSODataContext.Schema.ToString().ToLower() == "banorte"
                                                        || DSODataContext.Schema.ToString().ToLower() == "k5banorte")
                {
                    //NZ 20160921
                    GrafPorTipoLlamPeEm = DSODataAccess.Execute(consultaPorTipoLlamPeEmDetalleBanorte(
                    "[link] = ''" + Request.Path + "?Nav=PorTipoLlamN2&TipoLlam=''+convert(varchar,[Clave Tipo Llamada])"));

                    if (GrafPorTipoLlamPeEm.Rows.Count > 0)
                    {
                        string gpoNoIdentificado = DSODataAccess.ExecuteScalar("SELECT GEtiqueta FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')] WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = '0NoIdent'").ToString();
                        foreach (DataRow row in GrafPorTipoLlamPeEm.Rows)
                        {
                            if (row["Clave Tipo Llamada"].ToString() == gpoNoIdentificado)
                            {
                                row["link"] = Request.ApplicationPath + "/UserInterface/Historicos/Etiquetacion/EtiquetacionEmple.aspx";
                                break;
                            }
                        }
                    }
                } //20160722 NZ 
                else
                {
                    GrafPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaPorTipoLlamPeEm(
                    "[link] = ''" + Request.Path + "?Nav=PorTipoLlamN2&TipoLlam=''+convert(varchar,[Clave Tipo Llamada])"));

                }
                #endregion

                if (GrafPorTipoLlamPeEm.Rows.Count > 0 && GrafPorTipoLlamPeEm.Columns.Count > 0)
                {
                    DataView dvGrafConsHist = new DataView(GrafPorTipoLlamPeEm);
                    GrafPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "Tipo Llamada", "Total", "link" });
                    GrafPorTipoLlamPeEm.Columns["Tipo Llamada"].ColumnName = "label";
                    GrafPorTipoLlamPeEm.Columns["Total"].ColumnName = "value";
                    GrafPorTipoLlamPeEm.Columns["link"].ColumnName = "link";
                }

                Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafPorTipoLlamPeEm_G", "Consumo por tipo de llamada", 0, FCGpoGraf.Tabular, Request.Path + "?Nav=PorTipoLlamN1"));


                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafPorTipoLlamPeEm_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafPorTipoLlamPeEm),
                    "GrafPorTipoLlamPeEm_G", "Consumo por tipo de llamada", "", "Tipo de llamada", "Total", 0, FCGpoGraf.Tabular), false);


            }

            #endregion Reporte por tipo de llamada (Grafica)

            #region Reporte Numeros mas caros


            //Se obtiene el DataSource del reporte
            DataTable RepNumMasCarosPeEm = DSODataAccess.Execute(ConsultaNumerosMasCarosPeEm());

            if (RepNumMasCarosPeEm.Rows.Count > 0 && RepNumMasCarosPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepNumMasCarosPeEm);
                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                {
                    RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarcado", "Numero Marcado", "Etiqueta", "Tipo Llamada", "Total", "Duracion", "Numero" });
                }
                else
                {
                    RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarcado", "Numero Marcado", "Nombre Localidad",
                                                                                                               "Tipo Llamada", "Total", "Duracion","Numero" });

                    RepNumMasCarosPeEm.Columns["Nombre Localidad"].ColumnName = "Localidad";
                }

                RepNumMasCarosPeEm.Columns["Numero Marcado"].ColumnName = "Número marcado";
                RepNumMasCarosPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                RepNumMasCarosPeEm.Columns["Total"].ColumnName = "Total";
                RepNumMasCarosPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                RepNumMasCarosPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";       //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"

                if (DSODataContext.Schema.ToLower() != "bimbo")
                {
                    RepNumMasCarosPeEm.Columns["Tipo de llamada"].ColumnName = "Tipo de llamada";
                }
              }

            int [] colNoVisibles = new int[] { };
            int[] colNav = new int[] { };
            if (DSODataContext.Schema.ToLower() != "bimbo")
            {
                
                colNoVisibles = new int[] { 0 };
                colNav = new int[] { 2, 3, 4, 5, 6 };
            }
            else
            {
                colNoVisibles = new int[] { 0, 3 };
                colNav = new int[] { 2, 4, 5, 6 };
            }

            Rep3.Controls.Add(
                     DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                     DTIChartsAndControls.GridView("RepNumMasCarosPeEmGrid_T", RepNumMasCarosPeEm, true, "Totales",
                                     new string[] { "", "", "", "", "{0:c}", "", "" }, Request.Path + "?Nav=NumMasCarosN2&NumMarc={0}",
                                     new string[] { "CodNumMarcado" }, 1, colNoVisibles, colNav, new int[] { 1 }),
                                     "RepNumMasCarosPeEmGridRep03_T", "Reporte números mas caros", Request.Path + "?Nav=NumMasCarosN1")
                     );




            #endregion Reporte Numeros mas caros

            #region Reporte Historico

            //Se obtiene el DataSource del reporte
            DataTable GrafConsHist = DSODataAccess.Execute(ConsultaHistoricoPeEm());
            if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(GrafConsHist);
                GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                GrafConsHist.Columns["Nombre Mes"].ColumnName = "label";
                GrafConsHist.Columns["Total"].ColumnName = "value";
                GrafConsHist.AcceptChanges();
            }



            Rep4.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsHist_G", "Consumo histórico a  12 meses", 0, FCGpoGraf.Tabular, Request.Path + "?Nav=HistoricoPeEmN1"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsHist_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafConsHist), "GrafConsHist_G", "Consumo histórico a  12 meses", "", "Mes", "Importe", 0, FCGpoGraf.Tabular), false);




            #endregion Reporte Historico

            #region Reporte Extensiones en las que se utilizo el codigo de llamadas

            DataTable RepExtenDondeUtilizoCodAut = DSODataAccess.Execute(ConsultaExtensionesEnLasQueSeUsoElCodAuto());

            if (RepExtenDondeUtilizoCodAut.Rows.Count > 0 && RepExtenDondeUtilizoCodAut.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepExtenDondeUtilizoCodAut);
                RepExtenDondeUtilizoCodAut = dvGrafConsHist.ToTable(false, new string[] { "Codigo Autorizacion", "Extension", "Llamadas" });
                RepExtenDondeUtilizoCodAut.Columns["Codigo Autorizacion"].ColumnName = "Código de autorización";
                RepExtenDondeUtilizoCodAut.Columns["Extension"].ColumnName = "Extensión";
                RepExtenDondeUtilizoCodAut.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
            }

            Rep5.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepExtenDondeUtilizoCodAutGrid_T", RepExtenDondeUtilizoCodAut, true, "Totales",
                                new string[] { "", "", "" }), "RepExtenDondeUtilizoCodAutGridRep05_T", "Extensiones en las que se usó el código de llamadas ", Request.Path + "?Nav=ExtenUsoCodAutN1")
                );


            #endregion Reporte Extensiones en las que se utilizo el codigo de llamadas

            //PO: SOLO APLICA PARA BIMBO
            #region Reporte Consumo por tipo de llamada (Grafica)

            if (DSODataContext.Schema.ToLower() == "bimbo")
            {
                DataTable GrafConPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaConsumoPorTipoLlamadaPeEm());
                if (GrafConPorTipoLlamPeEm.Rows.Count > 0 && GrafConPorTipoLlamPeEm.Columns.Count > 0)
                {
                    DataView dvGrafConsHist = new DataView(GrafConPorTipoLlamPeEm);
                    GrafConPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "etiqueta", "Total" });
                    GrafConPorTipoLlamPeEm.Columns["etiqueta"].ColumnName = "label";
                    GrafConPorTipoLlamPeEm.Columns["Total"].ColumnName = "value";

                }

                Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConPorTipoLlamPeEm_G", "Consumo por tipo de llamada", 0, FCGpoGraf.Tabular, Request.Path + "?Nav=ExtenUsoCodAutN1"));


                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConPorTipoLlamPeEm_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafConPorTipoLlamPeEm),
                    "GrafConPorTipoLlamPeEm_G", "Consumo por tipo de llamada", "", "Tipo de llamada", "Total", 0, FCGpoGraf.Tabular), false);




            }

            #endregion Reporte Consumo por tipo de llamada (Grafica)
        }

        public void DashboardMultiPerfil()
        {
            DataTable reportes = new DataTable();

            reportes = DSODataAccess.Execute(ConsultaConfiguracionDeReportesPorPerfil("Dashboard.aspx"));


            if (param["Nav"] == "DashboardSiana" ||
                HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("dashboardsiana.aspx"))
            {
                //Substituye el texto "SoloCDR" por "SoloSiana" en el nombre de los reportes
                reportes = GetReportesDashboardSoloSiana(reportes);
                //Deja las variables en blanco para que no se muestre el mapa de navegación en la página
                param["Nav"] = string.Empty;
                TituloNavegacion = string.Empty;
            }

            if (param["Nav"] == "DashboardDesvios" ||
                HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("dashboarddesvios.aspx"))
            {
                pnlDesvios.Visible = true;
                reportes = DSODataAccess.Execute(ConsultaConfiguracionDeReportesPorPerfil("DashboardDesvios.aspx"));
                param["Dash"] = "Desvios";
                param["Nav"] = string.Empty;
                TituloNavegacion = string.Empty;
            }
            if(param["Nav"] == "DashboardNoContestadas" ||
                HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("dashboardnocontestadas.aspx"))
            {
                reportes = DSODataAccess.Execute(ConsultaConfiguracionDeReportesPorPerfil("DashboardNoContestadas.aspx"));
                param["Dash"] = "NoContestadas";
                param["Nav"] = string.Empty;
                TituloNavegacion = string.Empty;
            }
            BuscaReportes(reportes);
        }

        private DataTable GetReportesDashboardSoloSiana(DataTable rep)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Reporte");
            dt.Columns.Add("Contenedor");
            dt.Columns.Add("tipoGrafDefault");
            dt.Columns.Add("tituloGrid");
            dt.Columns.Add("tituloGrafica");
            dt.Columns.Add("pestaniaActiva");


            foreach (DataRow item in rep.Rows)
            {
                DataRow row = dt.NewRow();
                row["Reporte"] = item["Reporte"].ToString().Replace("SoloCDR", "SoloSiana");
                row["Contenedor"] = item["Contenedor"].ToString();
                row["tipoGrafDefault"] = item["tipoGrafDefault"].ToString();
                row["tituloGrid"] = item["tituloGrid"].ToString().Replace("CDR", "Siana");
                row["tituloGrafica"] = item["tituloGrafica"].ToString().Replace("CDR", "Siana"); ;
                row["pestaniaActiva"] = item["pestaniaActiva"].ToString();
                dt.Rows.Add(row);
            }

            return dt;
        }

        private static void RemoveColHerencia(ref DataTable dt)
        {
            #region Elimina columnas no necesarias
            if (dt.Columns.Contains("RID"))
                dt.Columns.Remove("RID");
            if (dt.Columns.Contains("RowNumber"))
                dt.Columns.Remove("RowNumber");
            if (dt.Columns.Contains("TopRID"))
                dt.Columns.Remove("TopRID");
            #endregion
        }

        public void GeneraGraficaUnaSerie(Control contenedorGrafica, DataTable ldtGrid, string idControl, string tipoGrafDefault, string tituloGrafica, string nombreEjeX, string nombreEjeY, string CSSStyle, string temaGrafica, int numeroElementos, bool generaLinkSiguienteNivel)
        {

            string codigoHTMLDiv = FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie(idControl, CSSStyle, tipoGrafDefault);
            LiteralControl literalControl = new LiteralControl(codigoHTMLDiv);
            Control tituloYBordes = DTIChartsAndControls.tituloYBordesReporte(literalControl, tituloGrafica, 0);

            DataTable ldtGrafica = ObtieneDatosAGraficarUnaSerie(ldtGrid, nombreEjeX, nombreEjeY, generaLinkSiguienteNivel);

            DataTable elementosAGraficar = DTIChartsAndControls.selectTopNTabla(ldtGrafica, "value desc", numeroElementos);
            string codigoJSON = FCAndControls.ConvertDataTabletoJSONString(elementosAGraficar);

            //NZ Comente el codigo por que con la nueva versión ya no funcionara.
            //string scriptCreaGrafica = FCAndControls.GeneraScriptGrafica1Serie(codigoJSON, tipoGrafDefault,
            //                                    idControl, "", "", nombreEjeX, nombreEjeY, temaGrafica,
            //                                    "98%", "330", idControl);
            contenedorGrafica.Controls.Add(tituloYBordes);

            //Page.ClientScript.RegisterStartupScript(this.GetType(), idControl, scriptCreaGrafica, false);
        }

        public Control GeneraGraficaUnaSerie(DataTable ldtGrid, string idControl, string tipoGrafDefault, string tituloGrafica, string nombreEjeX, string nombreEjeY, string CSSStyle, string temaGrafica, int numeroElementos, bool generaLinkSiguienteNivel)
        {

            string codigoHTMLDiv = FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie(idControl, CSSStyle, tipoGrafDefault);
            LiteralControl literalControl = new LiteralControl(codigoHTMLDiv);
            Control tituloYBordes = DTIChartsAndControls.tituloYBordesReporte(literalControl, tituloGrafica, 0);

            DataTable ldtGrafica = ObtieneDatosAGraficarUnaSerie(ldtGrid, nombreEjeX, nombreEjeY, generaLinkSiguienteNivel);


            DataTable elementosAGraficar = DTIChartsAndControls.selectTopNTabla(ldtGrafica, "value desc", numeroElementos);
            string codigoJSON = FCAndControls.ConvertDataTabletoJSONString(elementosAGraficar);

            //NZ Comente este codigo por que con la nueva version ya no funcionara.
            //string scriptCreaGrafica = FCAndControls.GeneraScriptGrafica1Serie(codigoJSON, tipoGrafDefault,
            //                                    idControl, "", "", nombreEjeX, nombreEjeY, temaGrafica,
            //                                    "98%", "330", idControl);

            //Page.ClientScript.RegisterStartupScript(this.GetType(), idControl, scriptCreaGrafica, false);

            return tituloYBordes;

        }

        public DataTable ObtieneDatosAGraficarUnaSerie(DataTable ldt, string nombreColumnaElementos, string nombreColumnaTotales, bool generaLinkSiguienteNivel)
        {
            string[] camposGrafica;

            if (generaLinkSiguienteNivel)
            {
                camposGrafica = new string[] { nombreColumnaElementos, nombreColumnaTotales, "link" };
            }
            else
            {
                camposGrafica = new string[] { nombreColumnaElementos, nombreColumnaTotales };
            }

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                //Se valida si el DataTable tiene fila de totales (Si es "true" entonces elimina esa fila)
                if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaElementos].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                //Se cambia el nombre de las columnas del DataTable porque asi lo necesita el metodo que genera las graficas
                ldt.Columns[nombreColumnaElementos].ColumnName = "label";
                ldt.Columns[nombreColumnaTotales].ColumnName = "value";
                ldt.AcceptChanges();
            }

            return ldt;
        }


        private string BuscarTipoLlamada()
        {
            switch (param["TipoLlam"])
            {
                case "0":
                    return "No identificada";
                case "1":
                    return "Personal";
                case "2":
                    return "Laboral";
                default:
                    return "";
            }
        }

        private void SeleccionaReportePorCenCosOPorEmple()
        {
            if (TieneCenCosHijos(param["CenCos"]))
            {
                RepTabPorCenCosJer2Pnls(Request.Path + "?Nav=CenCosJerN2&CenCos={0}",
                                                "[link] = ''" + Request.Path + "?Nav=CenCosJerN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])",
                                                    Rep2, 2, "Gráfica consumo por centro de costos jerárquico", Rep1, "Detalle consumo por centro de costos jerárquico");
            }
            else
            {
                RepTabPorEmpleMasCaros2Pnls(Request.Path + "?Nav=CenCosJerN3&CenCos=" + param["CenCos"] + "&Emple={0}",
                                                "[link] = ''" + Request.Path + "?Nav=CenCosJerN3&CenCos=" + param["CenCos"] + "&Emple='' + convert(varchar,[Codigo Empleado])",
                                        Rep2, 2, "Gráfica consumo por colaborador", Rep1, "Detalle consumo por colaborador");
            }
        }

        private bool TieneCenCosHijos(string CenCos)
        {
            bool lbool = false;
            int NumCenCosHijos = 0;
            NumCenCosHijos = Convert.ToInt32(DSODataAccess.ExecuteScalar(ConsultaNumCenCosHijos(CenCos)));
            if (NumCenCosHijos > 0)
            {
                lbool = true;
            }
            return lbool;
        }

        protected void AjustaFechas()
        {
            string[] FechaMesAnio = param["MesAnio"].Split(new char[] { '-' });
            string Mes = FechaMesAnio[0];
            string Anio = FechaMesAnio[1];
            DateTime fechaInicioRep = new DateTime(Convert.ToInt32(Anio), MesLetraANumero(Mes), 1);
            DateTime fechaFinRep = fechaInicioRep.AddMonths(1).AddDays(-1);

            Session["FechaInicio"] = fechaInicioRep.ToString("yyyy-MM-dd");  //La fecha siempre debe guardarse en este formato.
            Session["FechaFin"] = fechaFinRep.ToString("yyyy-MM-dd");
        }

        protected int MesLetraANumero(string Mes)
        {
            switch (Mes)
            {
                case "Enero":
                    return 1;
                case "Febrero":
                    return 2;
                case "Marzo":
                    return 3;
                case "Abril":
                    return 4;
                case "Mayo":
                    return 5;
                case "Junio":
                    return 6;
                case "Julio":
                    return 7;
                case "Agosto":
                    return 8;
                case "Septiembre":
                    return 9;
                case "Octubre":
                    return 10;
                case "Noviembre":
                    return 11;
                case "Diciembre":
                    return 12;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Este metodo recibe un DataTable con el listado de reportes configurados en Historicos >> Entidad: FC Configuracion de reportes  | Maestro: FC Configuracion de reportes 
        /// y por cada uno de estos elementos se genera un nuevo reporte en la pagina.
        /// </summary>
        /// <param name="RelacionReportesContenedor">Reportes configurados en historicos</param>
        protected void BuscaReportes(DataTable RelacionReportesContenedor)
        {
            foreach (DataRow dr in RelacionReportesContenedor.Rows)
            {
                try
                {
                    Panel contenedor = (Panel)pnlMainHolder.FindControl(dr["Contenedor"].ToString());
                    string reporte = dr["Reporte"].ToString();
                    string tipoGrafDefault = dr["tipoGrafDefault"].ToString();
                    string tituloGrid = dr["tituloGrid"].ToString();
                    string tituloGrafica = dr["tituloGrafica"].ToString();
                    string pestaniaActiva = dr["pestaniaActiva"].ToString();
                    CargaReporte(reporte, contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, Convert.ToInt32(pestaniaActiva));
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "No se pudo cargar el reporte ''" + dr["Reporte"].ToString() + "''" + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
        }

        protected void CargaReporte(string reporte, Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            int PestanaActiva = pestaniaActiva;

            int omitirInfoCDR = 0;
            int omitirInfoSiana = 0;

            int.TryParse(param["omitirInfoCDR"].ToString(), out omitirInfoCDR);
            int.TryParse(param["omitirInfoSiana"].ToString(), out omitirInfoSiana);

            switch (reporte)
            {
                case "RepTabHist1Pnl":
                    RepTabHist1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabHistLlamadas1Pnl":
                    RepTabHistLlamadas1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabHistSoloCDR1Pnl":
                    RepTabHist1Pnl(contenedor, tituloGrid, PestanaActiva, 0, 1);
                    break;
                case "RepTabHistSoloSiana1Pnl":
                    RepTabHist1Pnl(contenedor, tituloGrid, PestanaActiva, 1, 0);
                    break;
                case "RepTabHist1PnlPrs":
                    RepTabHist1PnlPrs(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorSitio1Pnl":
                    HttpContext.Current.Session["OmiteTDets"] = "1";                    
                    RepTabPorSitio1Pnl(contenedor, tituloGrid, PestanaActiva, omitirInfoCDR, omitirInfoSiana);
                    break;
                case "RepTabPorSitioSoloCDR1Pnl":
                    RepTabPorSitio1Pnl(contenedor, tituloGrid, PestanaActiva, 0, 1);
                    break;
                case "RepTabPorSitioSoloSiana1Pnl":
                    RepTabPorSitio1Pnl(contenedor, tituloGrid, PestanaActiva, 1, 0);
                    break;
                case "RepTabPorTDest1Pnl":
                    RepTabPorTDest1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorTDestSoloCDR1Pnl":
                    RepTabPorTDest1Pnl(contenedor, tituloGrid, PestanaActiva, 0, 1);
                    break;
                case "RepTabPorTDestSoloSiana1Pnl":
                    RepTabPorTDest1Pnl(contenedor, tituloGrid, PestanaActiva, 1, 0);
                    break;
                case "RepTabPorCenCosJer1Pnl":
                    RepTabPorCenCosJer1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorEmpleMasCaros1Pnl":
                    RepTabPorEmpleMasCaros1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorEmpleMasCarosDash1Pnl":
                    RepTabPorEmpleMasCarosDash1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorTpLlam1Pnl":
                    RepTabPorTpLlam1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepMatPorSitio1Pnl":
                    RepMatPorSitio1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorCenCos1Pnl":
                    RepTabPorCenCos1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepMatPorCarrier1Pnl":
                    RepMatPorCarrier1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorDiaDeSemana1Pnl":
                    RepTabPorDiaDeSemana1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabHist3Anios1Pnl":
                    RepTabHist3Anios1Pnl(contenedor, tituloGrid, tituloGrafica, PestanaActiva);   //NZEste metodo no lo toquen
                    break;
                case "RepTabConsColaboradores1Pnl":
                    RepTabConsColaboradores1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabTipoDestinoPrs1Pnl":
                    RepTabTipoDestinoPrs1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabPorEmpleMasCarosConSitio1Pnl":
                    RepTabPorEmpleMasCarosConSitio1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTopAreaPrs1Pnl":
                    if (Convert.ToInt32(Session["iCodUsuario"]) != 255596)  //UadminProsaCAT
                    {
                        RepTopAreaPrs1Pnl(contenedor, tituloGrid, PestanaActiva);
                    }
                    break;
                case "RepConsumoJerarquico1Pnl":   //20150618 NZ Nuevo Reporte Jerarquico 
                    RepTabCenCosJerarquicoP1(contenedor, tituloGrid, PestanaActiva, tituloGrafica);
                    break;
                case "RepConsumoJerarquicoSoloCDR1Pnl":
                    RepTabCenCosJerarquicoP1(contenedor, tituloGrid, PestanaActiva, tituloGrafica, 0, 1);  //Implementado originalmente para SevenEleven, reporte solo muestra info de CDR excluye Siana
                    break;
                case "RepConsumoJerarquicoSoloSiana1Pnl":
                    RepTabCenCosJerarquicoP1(contenedor, tituloGrid, PestanaActiva, tituloGrafica, 1, 0); //Implementado originalmente para SevenEleven, reporte solo muestra info de Siana excluye CDR
                    break;
                case "RepTabConsumoPorCampaniaPrs1Pnl":
                    if (Convert.ToInt32(Session["iCodUsuario"]) == 255596)  //UadminProsaCAT
                    {
                        RepTabConsumoPorCampaniaPrs1Pnl(contenedor, tituloGrid, PestanaActiva);
                    }
                    break;
                case "RepTabConsumoPorTDestDashboard":
                    if (DSODataContext.Schema.ToLower() == "prosa" && Convert.ToInt32(Session["iCodUsuario"]) == 255596)
                    {
                        RepTabPorTDest1Pnl(contenedor, tituloGrid, PestanaActiva);
                    }
                    else { RepTabPorTDestDashboard1Pnl(contenedor, tituloGrid, PestanaActiva); }
                    break;
                case "RepTabEmpsConLlamsACel10Digs1Pnl":
                    RepTabEmpsConLlamsACel10Digs1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabTotLlamNoContestadas":
                    HttpContext.Current.Response.Redirect("~/UserInterface/DashboardFC/Dashboard.aspx?Nav=RepLlamPerdidas2pnl", false);
                    break;
                case "RepTabPorSitioSiana1Pnl":
                    RepTabPorSitioSiana1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepSeeYouOnUtilCliente":
                    RepTabSeeYouOnUtilCliente(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepSeeYouOnUtilSistema":
                    RepTabSeeYounOnUtilSistema(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepSeeYouOnUtilSistemaHist":
                    RepTabSeeYouOnUtilSistemaHist(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabCenCosHorasHombre":
                    RepTabSeeYouOnHorasHombreCencos(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabConsumoBolsas":
                    RepConsumoBolsasDiebold(contenedor, tituloGrid, pestaniaActiva, tituloGrafica);
                    break;
                case "RepMatPerdidasPnl":
                    RepMatPerdidasPnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepDesviosTipoDestino1pnl":
                    RepDesviosPorTipoDestino(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepDesviosPorExtension1pnl":
                    string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&NumDesvios=2'' '";
                    RepDesviosPorExtension(contenedor, tituloGrid, pestaniaActiva, link);
                    break;
                case "RepDesviosPorCencosto1Pnl":
                    RepDesviosPorCencosto1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepDesviosPorSitios1Pnl":
                    RepDesviosPorSitios1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                #region Reportes desvios
                case "RepDesviosPorHora":
                    RepDesviosPorHoraDash(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepDesviosTop10LlamadasM":
                    RepDesviosTop10LlamadasDash(contenedor, tituloGrid, pestaniaActiva, "Minutos");
                    break;
                case "RepDesviosTop10LlamadasG":
                    RepDesviosTop10LlamadasDash(contenedor, tituloGrid, pestaniaActiva, "Gasto");
                    break;
                case "RepDesviosTop10Ext":
                    RepDesviosTop10ExtDash(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepDesviosPerdidosPorEmple":
                    RepDesviosPerdidosPorEmpleDash(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepDesviosPorCarrier":
                    RepDesviosPorCarrierDash(contenedor, tituloGrid, pestaniaActiva);
                    break;
                #endregion Reportes desvios
                case "ConsEmpmasLlam":
                    ConsEmpmasLlam1Dash(contenedor, tituloGrid, 0);
                    break;
                case "RepLlamadasPorDispositivo":
                    RepLlamadasPorDispositivoDash(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepLlamadasPerdidasPorTDest1Pnl":
                    RepTabPorTdestPerdidas1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepLlamadasPerdidasPorCenCos1Pnl":
                    RepTabPorCenCosPerdidas1pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepLlamadasPerdidasPorSitio1Pnl":
                    RepTabPorSitioPerdidas1Pnl(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepLlamadasPerdidasPorTopEmple1Pnl":
                    RepTabTop10EmplePerdidas(contenedor, tituloGrid, PestanaActiva);
                    break;
                #region Dashboard Empleados
                case "RepTapTipoDestinoPeEm":
                    ReptabConsultaPorTipoDestinoPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTapTipoLlamPeEmDetalle":/*es para todos los clientes excepto Bimbo*/
                    ReptabconsultaPorTipoLlamPeEmDetalleBanorte(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabNumerosMasCarosPeEm":
                    ReptabConsultaNumerosMasCarosPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabHistoricoEmpleado":
                    tabConsultaHistoricoPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabExtenLlamadas":
                    tabConsultaExtensionesEnLasQueSeUsoElCodAuto(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabTipoLlamGraf":/*solo aplica para bimbo*/
                    tabConsultaConsumoPorTipoLlamadaPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;
                #endregion Dashboard Empleados

                case "ConsultaPorTipoDestinoPeEm":
                    ReptabConsultaPorTipoDestinoPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "consultaPorTipoLlamPeEmDetalleBanorte":
                    ReptabconsultaPorTipoLlamPeEmDetalleBanorte(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "ConsultaNumerosMasCarosPeEm":
                    ReptabConsultaNumerosMasCarosPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;

                case "RepHistoricoEmpleado":
                    tabConsultaHistoricoPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepExtenLlamadas":
                    tabConsultaExtensionesEnLasQueSeUsoElCodAuto(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTipoLlamGraf":
                    tabConsultaConsumoPorTipoLlamadaPeEm(contenedor, tituloGrid, PestanaActiva);
                    break;
                case "RepTabConsLlamsMasCaras":
                    RepConsLlamsMasCaras1Pnl(contenedor, tituloGrid);
                    break;
                case "RepTabConsNumerosMasMarcadas":
                    RepConsNumerosMasMarcadas1Pnl(contenedor, tituloGrid);
                    break;
                case "RepTabConsLLamadasMasTiempo":
                    RepConsMenuConsLLamadasMasTiempo1Pnl(contenedor, tituloGrid);
                    break;
                case "RepTabPorExtensionPI":
                    RepTabPorExtensionPI1Pnl(contenedor, tituloGrid, PestanaActiva, omitirInfoCDR, omitirInfoSiana);
                    break;
                case "RepTabParticipantesvsReunionesMes":
                    //RepTabParticipantesvsReunionesMes(contenedor,  tituloGrid, PestanaActiva);
                    break;
                case "RepTabParticipantesvsHorasMes":
                    //RepTabParticipantesvsHorasMes(contenedor,  tituloGrid, PestanaActiva);
                    break;
                // TODO : DO Paso 3 - Agregar ID de reporte: Esto solo aplica cuando se vera en el Dashboard a primer nivel.
                //Control contenedor, Control contenedorGrafica, string tituloGrafica, string tituloGrid, int pestaniaActiva
                default:
                    break;
            }
        }

        public void VaciaVariables()
        {
            listadoParametros.Clear();
            listadoCamposReporte.Clear();
        }

        #region Funcionalidad del Link hacia la pagina de Etiquetacion.
        //NZ: 20160719
        public void MostrarControlEtiquetacionEmple()
        {
            if (DSODataContext.Schema.ToLower() == "penoles")
            {
                CrearBotonMiEtiquetacion();
                pToolBar.CssClass = "col-md-6 col-sm-6";
            }

            if (DSODataContext.Schema.ToString().ToLower() == "k5banorte")
            {
                #region Consulta
                StringBuilder query = new StringBuilder();
                query.AppendLine("DECLARE @gpoEtiqNoIdent INT = (SELECT GEtiqueta FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
                query.AppendLine("								 WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()	AND vchCodigo = '0NoIdent' )");
                query.AppendLine("");
                query.AppendLine("DECLARE @emple INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
                query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + ")");
                query.AppendLine("");
                query.AppendLine("DECLARE @llamsEntrada INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                query.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
                query.AppendLine("");
                query.AppendLine("DECLARE @llamsEnlace INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                query.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
                query.AppendLine("");
                query.AppendLine("SELECT COUNT(DISTINCT TelDest)");
                query.AppendLine("FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')]");
                query.AppendLine("WHERE GEtiqueta = @gpoEtiqNoIdent");
                query.AppendLine("	    AND Emple = @emple");
                query.AppendLine("	    AND TDest <> @llamsEntrada");
                query.AppendLine("	    AND TDest <> @llamsEnlace");
                #endregion

                var totalLlamsSinEtiquetar = Convert.ToInt32(DSODataAccess.ExecuteScalar(query.ToString()).ToString());
                if (totalLlamsSinEtiquetar > 0)
                {
                    btnEtiquetacionEmple.Text = "Usted tiene " + totalLlamsSinEtiquetar.ToString() + " números sin Etiquetar";
                    pnlLinkEtiquetacion.Visible = btnEtiquetacionEmple.Visible = true;
                }
                else { pnlLinkEtiquetacion.Visible = lblSinEtiqPendiente.Visible = true; }
            }
            else
            {
                var fechas = CalcularPeriodosActualEtiquetacion();
                if (fechas != null)
                {
                    var totalLlamsSinEtiquetar = Convert.ToInt32(DSODataAccess.ExecuteScalar(ConsultaGetCountNumPorEtiquetar(fechas[0], fechas[1])).ToString());
                    if (totalLlamsSinEtiquetar > 0)
                    {
                        btnEtiquetacionEmple.Text = "Usted tiene " + totalLlamsSinEtiquetar.ToString() + " números sin Etiquetar";
                        pnlLinkEtiquetacion.Visible = btnEtiquetacionEmple.Visible = true;
                    }
                    else
                    {
                        btnEtiquetacionEmple.Text = lblSinEtiqPendiente.Text;
                        btnEtiquetacionEmple.Attributes.Add("style", "color:green;");
                        pnlLinkEtiquetacion.Visible = btnEtiquetacionEmple.Visible = true;

                    }
                }
                else { pnlLinkEtiquetacion.Visible = btnEtiquetacionEmple.Visible = lblSinEtiqPendiente.Visible = false; }
            }
        }

        private int ValidarDiasMes(int liYear, int liMonth, int liDayCoL)
        {
            int liDaysInMonth = DateTime.DaysInMonth(liYear, liMonth);
            if (liDaysInMonth < liDayCoL)
            {
                return liDaysInMonth;
            }
            else if (liDayCoL == 0)
            {
                return DateTime.Today.Day;
            }
            return liDayCoL;
        }

        //NZ 20170425 Se agrega metodo para el calculo de fechas del periodo.
        private DateTime[] CalcularPeriodosActualEtiquetacion()
        {
            int liDiaCorte;
            int liDiaLimite;
            DateTime pdtActual = DateTime.Today;
            DateTime pdtCorte;
            DateTime pdtLimite;
            DateTime pdtIniPeriodoInicial;
            DateTime pdtFinPeriodoInicial;

            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT [DiaCorte] = DiaEtiquetacion, [DiaLimite] = DiaLmtEtiquetacion");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine("AND UsuarDBCod = '" + DSODataContext.Schema + "'");

            var ldrCliente = DSODataAccess.ExecuteDataRow(query.ToString());

            if (ldrCliente != null && !string.IsNullOrEmpty(ldrCliente["DiaCorte"].ToString()) && !string.IsNullOrEmpty(ldrCliente["DiaLimite"].ToString()))
            {
                int piDiaCorte = int.Parse(Util.IsDBNull(ldrCliente["DiaCorte"], 1).ToString());
                int piDiaLimite = int.Parse(Util.IsDBNull(ldrCliente["DiaLimite"], 0).ToString());

                if (1 > DateTime.Today.Day)
                {
                    liDiaCorte = ValidarDiasMes(pdtActual.AddMonths(-1).Year, pdtActual.AddMonths(-1).Month, piDiaCorte);
                    pdtCorte = new DateTime(pdtActual.AddMonths(-1).Year, pdtActual.AddMonths(-1).Month, liDiaCorte);
                }
                else
                {
                    liDiaCorte = ValidarDiasMes(pdtActual.Year, pdtActual.Month, piDiaCorte);
                    pdtCorte = new DateTime(pdtActual.Year, pdtActual.Month, liDiaCorte);
                }

                liDiaLimite = ValidarDiasMes(pdtActual.Year, pdtActual.Month, piDiaLimite);
                pdtLimite = new DateTime(pdtActual.Year, pdtActual.Month, liDiaLimite);

                int liDiaIniPeriodo = ValidarDiasMes(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, piDiaCorte);
                pdtIniPeriodoInicial = new DateTime(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, liDiaIniPeriodo);
                int liDiaFinPeriodo = ValidarDiasMes(pdtCorte.Year, pdtCorte.Month, piDiaCorte);
                if (liDiaFinPeriodo == 1)
                {
                    pdtFinPeriodoInicial = new DateTime(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, DateTime.DaysInMonth(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month));
                }
                else
                {
                    pdtFinPeriodoInicial = new DateTime(pdtCorte.Year, pdtCorte.Month, liDiaFinPeriodo - 1);
                }

                return new DateTime[2] { pdtIniPeriodoInicial, pdtFinPeriodoInicial };
            }
            else { return null; }
        }

        #endregion

        protected void btnExortDetallP_Click(object sender, EventArgs e)
        {

            if (LlamPerd.Visible == true)
            {
                Session["ExporDetall"] = 2;
            }
            //20150425 RJ.Se agrega condición para exportar el reporte de Capitel en formato Excel 2003
            if (param["Nav"] != "RepTabDetalleParaFinanzas")
            {
                ExportXLS(".xlsx");
            }
            else
            {
                ExportXLS(".xls");
            }
        }
        private string ValidaOcultarColumnImporte()
        {
            string ocultar = "0";

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select isnull((banderascliente & 67108864) / 67108864,0) as OcultarColumnaImporte ");
                sb.AppendLine("from [VisHistoricos('Client','Clientes','Español')] ");
                sb.AppendLine("where dtfinvigencia >= getdate() ");
                sb.AppendLine("and vchcodigo<> 'KeytiaC' ");
                ocultar = Convert.ToString(DSODataAccess.ExecuteScalar(sb.ToString()));
            }
            catch (Exception ex)
            {
                return "0";
            }

            return ocultar;
        }
        private void ValidaOcultarColumnImporteUsuar()
        {
            string ocultar = "0";

            try
            {
                if(Session["OcultarColumnImporte"] != null && Session["OcultarColumnImporte"].ToString() =="0")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT");
                    sb.AppendLine("ISNULL((BanderasUsuar & 16) / 16, 0) as OcultarColumnaImporte");
                    sb.AppendLine("FROM [VisHistoricos('Usuar','Usuarios','Español')]");
                    sb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    sb.AppendLine("AND iCodCatalogo =" + Session["iCodUsuario"].ToString() + " ");
                    ocultar = Convert.ToString(DSODataAccess.ExecuteScalar(sb.ToString()));
                    Session["OcultarColumnImporte"] = ocultar;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

}