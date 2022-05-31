using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Text;
using KeytiaServiceBL;
using System.Data;
using System.Collections;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using KeytiaServiceBL.Reportes;

namespace KeytiaWeb.UserInterface.DashboardLT.Reportes
{
    public partial class RepTraficoTK : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddDays(-1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        /*paginaLocal se usa para mandar el nombre de la clase donde se produjo el error en el log de web*/
        string paginaLocal = "KeytiaWeb.UserInterface.DashboardLT.Reportes.RepTraficoTK.aspx.cs";
        /*nombrePagina se usa para poder cambiarle el nombre a la pagina una ves que se pasen cambios a producción*/
        string nombrePagina = "RepTraficoTK.aspx";

        string Etiqueta = string.Empty;
        string NavegacionNivel = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            #region Almacenar en variable de sesion los urls de navegacion
            List<string> list = new List<string>();
            string lsURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            if (Session["pltNavegacion"] != null) //Entonces ya tiene navegacion almacenada
            {
                list = (List<string>)Session["pltNavegacion"];
            }

            //Si el url no contiene querystring y la lista tiene urls hay que limpiar la lista
            if (!(lsURL.Contains("?")) && list.Count > 0)
            {
                //Asegurarse eliminar navegacion previa
                list.Clear();
            }

            //Si no existe entonces quiere decir que estoy en un nuevo nivel de navegacion
            if (!list.Exists(element => element == lsURL))
            {
                //Agregar el valor del url actual para almacenarlo en la lista de navegacion
                list.Add(lsURL);
            }

            //Guardar en sesion la nueva lista
            Session["pltNavegacion"] = list;

            //Ocultar boton de regresar cuando solo exista un elemento en la lista
            if (list.Count <= 1)
            {
                btnRegresar.Visible = false;
            }
            else
            {
                btnRegresar.Visible = true;
            }
            #endregion // -- Almacenar en variable de sesion los urls de navegacion

            if (!Page.IsPostBack)
            {
                #region Inicia los valores default de los controles de fecha
                try
                {
                    if (Session["FechaInicioDashLTRepTrafTK"] != null && Session["FechaFinDashLTRepTrafTK"] != null)
                    {
                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioDashLTRepTrafTK"].ToString().Substring(1, 10));
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinDashLTRepTrafTK"].ToString().Substring(1, 10));
                    }

                    else
                    {
                        //20140905 AM. Se comentan las siguientes lineas
                        //DataTable fechas = DSODataAccess.Execute(consultaFechasUltimoMesTasado());

                        //DateTime fechaInicio = (DateTime)fechas.Rows[0].ItemArray[0];
                        //DateTime fechaFinal = (DateTime)fechas.Rows[0].ItemArray[1];

                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)fechaInicio;
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)fechaFinal;
                    }

                    Session["FechaInicioDashLTRepTrafTK"] = pdtInicio.DataValue.ToString();
                    Session["FechaFinDashLTRepTrafTK"] = pdtFin.DataValue.ToString();
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al darle valores default a los campos de fecha en KeytiaWeb.UserInterface.DashboardLT.RepTraficoTK.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
                #endregion //Fin de bloque --Inicia los valores default de los controles de fecha

                FillDDLGpoTroncal();
            }

            #region Fechas en sesion

            Session["FechaInicioDashLTRepTrafTK"] = pdtInicio.DataValue.ToString();
            Session["FechaFinDashLTRepTrafTK"] = pdtFin.DataValue.ToString();

            if (Session["FechaInicioDashLTRepTrafTK"] != null && Session["FechaFinDashLTRepTrafTK"] != null)
            {
                pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioDashLTRepTrafTK"].ToString().Substring(1, 10));
                pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinDashLTRepTrafTK"].ToString().Substring(1, 10));
            }

            #endregion

            #region Etiqueta de navegacion

            if (Etiqueta.Length > 0) //Entonces ya tiene navegacion almacenada
            {
                List<string> listEtiquetaNavegacion = new List<string>();
                listEtiquetaNavegacion = (List<string>)Session["etiquetaNavegacion"];
                if (!listEtiquetaNavegacion.Contains(Etiqueta))
                {
                    listEtiquetaNavegacion.Add(Etiqueta);
                }
                lblInicio.Text = string.Join(" / ", listEtiquetaNavegacion.ToArray());
            }
            else
            {
                List<string> listEtiquetaNavegacion = new List<string>();
                listEtiquetaNavegacion.Add("Inicio");
                Session["etiquetaNavegacion"] = listEtiquetaNavegacion;
                lblInicio.Text = string.Join(" / ", listEtiquetaNavegacion.ToArray());
            }

            #endregion

            #region Lee Query String

            #region Revisar si el querystring nav contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["nav"]))
            {
                try
                {
                    NavegacionNivel = Request.QueryString["nav"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (nav) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                NavegacionNivel = "";
            }


            #endregion

            #endregion

            DataTable numCirXMin = DSODataAccess.Execute(consultaTKPorMin());

            pnlMainContainer.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(
                 DTIChartsAndControls.GraficaLinea(numCirXMin,
                 "HHMM", "CantCir", "", 500, 1300, "Análisis de tráfico", "Hora", " Circuitos", "", "grafnumCirXMin", false, true),
                     "Gráfica", "Reporte de análisis de tráfico", 0));

            if (numCirXMin.Rows.Count > 0)
            {
                DataTable gvDtnumCirXMin = numCirXMin.Select().CopyToDataTable();
                DataView gvDvnumCirXMin = new DataView(gvDtnumCirXMin);
                gvDtnumCirXMin = gvDvnumCirXMin.ToTable(false, new string[] { "HHMM", "CantCir"});
                gvDtnumCirXMin.Columns[0].ColumnName = "Hora";
                gvDtnumCirXMin.Columns[1].ColumnName = "Circuitos";

                pnlMainContainer.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(
                        DTIChartsAndControls.GridView("gvnumCirXMin", gvDtnumCirXMin,
                        false, ""), "Reporte", "Orden por fecha descendiente", 0));
            }
        }

        private void FillDDLGpoTroncal()
        {
            DataTable ldt = DTIChartsAndControls.ordenaTabla(DSODataAccess.Execute(consultaTKsVigentes()), "NumGpoTro ASC");

            ddlGpoTroncal.DataSource = ldt;
            ddlGpoTroncal.DataTextField = "NumGpoTro";
            ddlGpoTroncal.DataValueField = "NumGpoTro";
            ddlGpoTroncal.DataBind();
        }

        #region Consultas a SQL

        //20140905 AM. Se comenta siguiente metodo ya que no sera utilizado
        //protected string consultaFechasUltimoMesTasado()
        //{
        //    StringBuilder lsb = new StringBuilder();

        //    lsb.Append(" DECLARE @fechaMayorCDR SMALLDATETIME  \n");
        //    //lsb.Append(" SELECT @fechaMayorCDR = MAX(fechainicio) FROM " + DSODataContext.Schema + ".vDetalleCDR  \n");
        //    lsb.Append(" SELECT @fechaMayorCDR = MAX(fechainicio) FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleFacturaCDR','Español')]  \n");
        //    lsb.Append(" /*Primer dia del ultimo mes tasado y ultimo dia del ultimo mes tasado*/  \n");
        //    lsb.Append(" SELECT [fechaIni] = DATEADD(MONTH,DATEDIFF(MONTH,0,@fechaMayorCDR),0),  \n");
        //    lsb.Append("        [fechaFin] = DATEADD(MILLISECOND,-3,DATEADD(MONTH,DATEDIFF(MONTH,0,@fechaMayorCDR)+1,0))  \n");

        //    return lsb.ToString();
        //}

        protected string consultaTKPorMin()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneNumCircuitosPorMinuto @Esquema='" + DSODataContext.Schema + "',  \r ");
            lsb.Append("@FechaInicio='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', \r ");
            lsb.Append("@FechaFin='" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \r ");
            lsb.Append(", @Where = 'GpoTro.NumGpoTro = ''" + ddlGpoTroncal.SelectedValue + "''' \r ");
            return lsb.ToString();
        }

        protected string consultaTKsVigentes()
        {
            StringBuilder lsb = new StringBuilder();
            //20140905 AM. Se cambia query para que traiga solo las troncales con trafico en el año actual

            //lsb.Append("select NumGpoTro \r ");
            //lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('GpoTro','Español')] \r ");
            //lsb.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r ");
            //lsb.Append("and len(NumGpoTro)>0 group by NumGpoTro \r ");

            //lsb.Append("select  \r ");
            //lsb.Append("GpoTro.NumGpoTro \r ");
            //lsb.Append("from " + DSODataContext.Schema + ".TraficoTroncales Trafico \r ");
            //lsb.Append("Left join " + DSODataContext.Schema + ".[VisHistoricos('GpoTro','Español')] GpoTro \r ");
            //lsb.Append("on GpoTro.iCodCatalogo = Trafico.GpoTro and GpoTro.dtIniVigencia <> GpoTro.dtFinVigencia and GpoTro.dtFinVigencia >= GETDATE() \r ");
            //lsb.Append("where DatePart(year,FechaInicio) = DatePart(year,getdate()) \r ");
            //lsb.Append("group by GpoTro.NumGpoTro \r ");

            //20141103 AM. Se hace cambio de la consulta de troncales para que solo muestre en el combo aquellas troncales con consumo.
            //lsb.Append("select distinct GpoTro.NumGpoTro  \r");
            //lsb.Append("from " + DSODataContext.Schema + ".TraficoTroncales Trafico  \r");
            //lsb.Append("Left join " + DSODataContext.Schema + ".[VisHistoricos('GpoTro','Español')] GpoTro  \r");
            //lsb.Append("on GpoTro.iCodCatalogo = Trafico.GpoTro and GpoTro.dtIniVigencia <> GpoTro.dtFinVigencia and GpoTro.dtFinVigencia >= GETDATE()  \r");
            //lsb.Append("where FechaInicio >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r");
            //lsb.Append("and FechaInicio <= '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \r");
            //lsb.Append("and Trafico.CantidadCircuitos > 0 \r");

            //20141124 AM. Se cambia la consulta y se pasa a un stored procedure debido a que piden cambios de esta consulta 
            //muy frecuentemente
            lsb.Append("exec ObtieneTroncalesActivas @esquema='");
            lsb.Append(DSODataContext.Schema);
            lsb.Append("'"); 

            return lsb.ToString();
        }

        #endregion

        #region Logica de botones de la pagina

        protected void btnAplicar_Click(object sender, EventArgs e)
        {

        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            List<string> ltNavegacion = (List<string>)Session["pltNavegacion"];

            //obtener el numero actual de elementos de la lista
            string lsCantidadElem = ltNavegacion.Count.ToString();
            //eliminar el ultimos elemento de la lista
            ltNavegacion.RemoveAt(ltNavegacion.Count - 1);

            //obtener el ultimo elemento de la lista
            string lsLastElement = ltNavegacion[ltNavegacion.Count - 1];

            #region Etiqueta de navegacion

            //List<string> ltEtiquetaNavegacion = (List<string>)Session["estadoDeNavegacion"];
            ////eliminar los tres ultimos elementos de la listamo
            //for (int i = 0; i < 2; i++)
            //{
            //    if (ltEtiquetaNavegacion.Count > 1)
            //    {
            //        ltEtiquetaNavegacion.RemoveAt(ltEtiquetaNavegacion.Count - 1);
            //    }
            //}m

            //Session["estadoDeNavegacion"] = ltEtiquetaNavegacion;

            #endregion

            HttpContext.Current.Response.Redirect(lsLastElement);
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS();
        }

        #endregion

        #region Exportacion

        public void ExportXLS()
        {
            CrearReporteAXLS(".xlsx");
        }

        protected void CrearReporteAXLS(string lsExt)
        {
            DataTable table1 = DTIChartsAndControls.DataTable(consultaTKPorMin());

            DataTable gvDtnumCirXMin = table1.Select().CopyToDataTable();
            DataView gvDvnumCirXMin = new DataView(gvDtnumCirXMin);
            gvDtnumCirXMin = gvDvnumCirXMin.ToTable(false, new string[] { "HoraMin", "CantCir" });
            gvDtnumCirXMin.Columns[0].ColumnName = "Hora";
            gvDtnumCirXMin.Columns[1].ColumnName = "Circuitos";


            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\reportes\ReporteTraficoTK" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ProcesarTituloExcel(lExcel);

                #region Inserta grafica

                //DataTable ldtGrafica = DSODataAccess.Execute(consultaTKPorMin());

                crearGrafico(gvDtnumCirXMin, "Análisis de tráfico", new string[] { "Circuitos" }, new string[] { "Circuitos" }, new string[] { "Circuitos" }, "Hora", "", "", "Hora", "",
                                 true, TipoGrafica.Lineas, lExcel, "{GraficaPrincipal}", "Reporte", "DatosGr", 800, 400);

                #endregion

                DataTable gvDtnumCirXMinGrid = table1.Select().CopyToDataTable();
                DataView gvDvnumCirXMinGrid = new DataView(gvDtnumCirXMinGrid);
                gvDtnumCirXMinGrid = gvDvnumCirXMin.ToTable(false, new string[] { "HHMM", "CantCir" });
                gvDtnumCirXMinGrid.Columns[0].ColumnName = "Hora";
                gvDtnumCirXMinGrid.Columns[1].ColumnName = "Circuitos";

                if (gvDtnumCirXMinGrid.Rows.Count > 0)
                {
                    object[,] datos = lExcel.DataTableToArray(gvDtnumCirXMinGrid, true);
                    DataTable ldtTitulo = new DataTable();
                    ldtTitulo.Columns.Add("Análisis de tráfico");
                    object[,] titulo = lExcel.DataTableToArray(ldtTitulo, true);

                    EstiloTablaExcel estilo = new EstiloTablaExcel();
                    estilo.Estilo = "KeytiaGrid";
                    estilo.FilaEncabezado = true;
                    estilo.FilasBandas = true;
                    estilo.FilaTotales = false;
                    estilo.PrimeraColumna = false;
                    estilo.UltimaColumna = true;
                    estilo.ColumnasBandas = false;
                    estilo.AutoFiltro = false;
                    estilo.AutoAjustarColumnas = true;

                    lExcel.Actualizar("Reporte", "TituloReporte", false, titulo, estilo);
                    lExcel.Actualizar("Reporte", "RepTraficoTK", false, datos, estilo);
                }

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;


                lExcel.SalvarComo();


                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes analisis de trafico ");
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

        protected void ProcesarTituloExcel(ExcelAccess pExcel)
        {
            Hashtable lhtMeta = BuscarTituloExcel(pExcel);
            string lsImg;
            int liRenLogoCliente = -1;
            int liRenLogoKeytia = -1;
            int liRenTitulo;
            int liRenglon;
            Hashtable lHTDatosImg = null;
            string lsTopLeft = null;
            string lsBottomLeft = null;
            float lfImgOffset = 0;

            string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
            string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());

            //NZ 20150707 Se cambio el campo con el que se compara el esquema en la tabla de Clientes. En ves de vchcodigo se comprara con Esquema.
            DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] " +
                                                " where Esquema = '" + DSODataContext.Schema + "'" +
                                                " and dtinivigencia <> dtfinVigencia " +
                                                " and dtfinVigencia>getdate()");

            //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
            if (pRowCliente != null && pRowCliente["LogoExportacion"] != DBNull.Value && lhtMeta["{LogoCliente}"] != null)
            {
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                lsImg = pRowCliente["LogoExportacion"].ToString().Replace("~", "").Replace("/", "\\");
                if (lsImg.StartsWith("\\"))
                {
                    lsImg = lsImg.Substring(1);
                }
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, lsImg);
                if (System.IO.File.Exists(lsImg))
                {
                    lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"], lsImg, false, false, 0, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lfImgOffset = (float)lHTDatosImg["Ancho"];
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoCliente = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
            }

            lsImg = System.IO.Path.Combine(lsStylePath, @"images\KeytiaHeader.png");
            if (System.IO.File.Exists(lsImg) && lhtMeta["{LogoKeytia}"] != null)
            {
                lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"], lsImg, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                if ((bool)lHTDatosImg["Inserto"])
                {
                    lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                    lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                    liRenLogoKeytia = int.Parse(lsBottomLeft.Split(',')[0]);
                }
            }

            if (lhtMeta["{LogoCliente}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"]).set_Value(System.Type.Missing, String.Empty);

            if (lhtMeta["{LogoKeytia}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"]).set_Value(System.Type.Missing, String.Empty);


            if (lhtMeta["{TituloReporte}"] != null)
            {
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).set_Value(System.Type.Missing, String.Empty);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }
        }
        protected Hashtable BuscarTituloExcel(ExcelAccess pExcel)
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto("Reporte", "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto("Reporte", "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto("Reporte", "{TituloReporte}", true));

            return lhtRet;
        }

        #region Insertar grafico en excel

        protected void crearGrafico(DataTable ldt, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                             string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda, TipoGrafica tipoGraf,
                                             ExcelAccess lExcel, string textoPlantilla, string HojaGrafico, string HojaDatos, float anchoGraf, float alturaGraf)
        {
            ParametrosGrafica lparametrosGraf = parametrosDeGrafica(ldt, tituloGraf, columnaDatos, leyenda, serieId, EjeX, tituloEjeX, formatoEjeX, tituloEjeY,
                                                             formatoEjeY, mostrarLeyenda, tipoGraf);

            ProcesarGraficaExcel(HojaGrafico, HojaDatos, anchoGraf, alturaGraf, 0, 0, lparametrosGraf, lExcel, textoPlantilla);
        }

        protected ParametrosGrafica parametrosDeGrafica(DataTable lsDataSource, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                                                           string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda, TipoGrafica tipoGraf)
        {

            ParametrosGrafica lParametrosGrafica = new ParametrosGrafica();

            lParametrosGrafica.Datos = lsDataSource;
            lParametrosGrafica.Title = tituloGraf;
            lParametrosGrafica.DataColumns = columnaDatos.ToArray();
            lParametrosGrafica.SeriesNames = leyenda.ToArray();
            lParametrosGrafica.SeriesIds = serieId.ToArray();
            lParametrosGrafica.XColumn = EjeX;
            lParametrosGrafica.XIdsColumn = EjeX;
            lParametrosGrafica.XTitle = tituloEjeX;
            lParametrosGrafica.XFormat = formatoEjeX;
            lParametrosGrafica.YTitle = tituloEjeY;
            lParametrosGrafica.YFormat = formatoEjeY;
            lParametrosGrafica.ShowLegend = mostrarLeyenda;
            lParametrosGrafica.TipoGrafica = tipoGraf;

            return lParametrosGrafica;
        }

        protected Hashtable ProcesarGraficaExcel(string lsHojaGrafico, string lsHojaDatos, float lfWidth, float lfHeight, float lfOffsetX, float lfOffsetY,
                                                                ParametrosGrafica lParametrosGrafica, ExcelAccess lExcel, string cambiarTextoPorGraf)
        {
            FormatoGrafica lFormatoGrafica = new FormatoGrafica();
            lFormatoGrafica.Titulo = lParametrosGrafica.Title;
            lFormatoGrafica.Leyendas = lParametrosGrafica.ShowLegend;
            lFormatoGrafica.XFormat = lParametrosGrafica.XFormat;
            lFormatoGrafica.YFormat = lParametrosGrafica.YFormat;
            Microsoft.Office.Interop.Excel.XlChartType lCharType = GetTipoGraficaExcel(lParametrosGrafica.TipoGrafica);

            return lExcel.InsertarGrafico(lsHojaGrafico, lsHojaDatos, cambiarTextoPorGraf, lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lParametrosGrafica.DataColumns,
                     lParametrosGrafica.SeriesNames, lParametrosGrafica.Datos, lCharType, lfWidth, lfHeight, lfOffsetX, lfOffsetY, lFormatoGrafica);
        }

        protected Microsoft.Office.Interop.Excel.XlChartType GetTipoGraficaExcel(TipoGrafica lTipoGrafica)
        {
            if (lTipoGrafica == TipoGrafica.Pastel)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlPie;
            }
            else if (lTipoGrafica == TipoGrafica.Barras)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered;
            }
            else if (lTipoGrafica == TipoGrafica.Lineas)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlLine;
            }
            else
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlArea;
            }
        }

        #endregion

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {

            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        private DataTable GetDataTable(GridView dtg)
        {
            DataTable dt = new DataTable();

            // add the columns to the datatable            
            if (dtg.HeaderRow != null)
            {

                for (int i = 0; i < dtg.HeaderRow.Cells.Count; i++)
                {
                    if (dt.Columns.Contains(dtg.HeaderRow.Cells[i].Text))
                    {
                        dt.Columns.Add(dtg.HeaderRow.Cells[i].Text + i);
                    }
                    else
                    {
                        dt.Columns.Add(dtg.HeaderRow.Cells[i].Text.Replace("&#243;", "ó"));
                    }
                }
            }

            //  add each of the data rows to the table
            foreach (GridViewRow row in dtg.Rows)
            {
                DataRow dr;
                dr = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text.Replace("&nbsp;", "");
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        #endregion
    }
}
