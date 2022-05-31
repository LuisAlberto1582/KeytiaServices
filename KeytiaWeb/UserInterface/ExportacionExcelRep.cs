using KeytiaServiceBL;
using KeytiaServiceBL.Reportes;
using KeytiaWeb.UserInterface.DashboardFC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface
{
    public static class ExportacionExcelRep
    {
        public static void CreaTablaEnExcel(ExcelAccess lExcel, DataTable ldt, string hoja, string textoBusqueda)
        {
            object[,] datos = lExcel.DataTableToArray(FCAndControls.daFormatoACeldas(ldt), true);

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

            lExcel.Actualizar(hoja, textoBusqueda, false, datos, estilo);
        }

        /// <summary>
        /// Interpreta los metatags que se encuentren en la plantilla y los cambia por los valores que corresponde.
        /// </summary>
        /// <param name="pExcel">Objeto tipo excel</param>
        /// <param name="titulo">Titulo del reporte MetaTag {TituloReporte}</param>
        public static void ProcesarTituloExcel(ExcelAccess pExcel, string titulo)
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
            string lsStylePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Session["StyleSheet"].ToString());

            //NZ 20150707 Se cambio el campo con el que se compara el esquema en la tabla de Clientes. En ves de vchcodigo se comprara con Esquema.
            DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] WITH(NOLOCK)" +
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
                    lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"], lsImg, false, false, 0, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating,60);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lfImgOffset = (float)lHTDatosImg["Ancho"]+10;
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoCliente = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
            }

            lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, @"images\KeytiaReportes.png");
            if (System.IO.File.Exists(lsImg) && lhtMeta["{LogoKeytia}"] != null)
            {
                lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"], lsImg, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating,46);
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
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).set_Value(System.Type.Missing, titulo);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }
            if (lhtMeta["{FechasReporte}"] != null)
            {
                int piRenFechas;
                int piColFechas;

                pExcel.BuscarTexto("Reporte", "{FechasReporte}", true, out piRenFechas, out piColFechas);
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas, "Inicio:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + Convert.ToDateTime(HttpContext.Current.Session["FechaInicio"].ToString()).ToString("dd/MM/yyyy"));
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 2, "Fin:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 3, "'" + Convert.ToDateTime(HttpContext.Current.Session["FechaFin"].ToString()).ToString("dd/MM/yyyy"));
            }
            if (lhtMeta["{FechasReporteMovil}"] != null)
            {
                int piRenFechas;
                int piColFechas;

                pExcel.BuscarTexto("Reporte", "{FechasReporteMovil}", true, out piRenFechas, out piColFechas);
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas, "Período:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + MonthName(Convert.ToDateTime(HttpContext.Current.Session["FechaInicio"]).Month) + " " + Convert.ToDateTime(HttpContext.Current.Session["FechaInicio"]).Year.ToString());
            }
        }
        public static void ProcesarTituloExcel(ExcelAccess pExcel, string titulo,string hoja)
        {
            Hashtable lhtMeta = BuscarTituloExcel(pExcel, hoja);
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
            string lsStylePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Session["StyleSheet"].ToString());

            //NZ 20150707 Se cambio el campo con el que se compara el esquema en la tabla de Clientes. En ves de vchcodigo se comprara con Esquema.
            DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] WITH(NOLOCK)" +
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
                    lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"], lsImg, false, false, 0, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating, 70);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lfImgOffset = (float)lHTDatosImg["Ancho"] + 10;
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoCliente = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
            }

            lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, @"images\KeytiaReportes.png");
            if (System.IO.File.Exists(lsImg) && lhtMeta["{LogoKeytia}"] != null)
            {
                lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"], lsImg, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating, 45);
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
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).set_Value(System.Type.Missing, titulo);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas(hoja, liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }
            if (lhtMeta["{FechasReporte}"] != null)
            {
                int piRenFechas;
                int piColFechas;

                pExcel.BuscarTexto(hoja, "{FechasReporte}", true, out piRenFechas, out piColFechas);
                pExcel.Actualizar(hoja, piRenFechas, piColFechas, "Inicio:");
                pExcel.Actualizar(hoja, piRenFechas, piColFechas + 1, "'" + Convert.ToDateTime(HttpContext.Current.Session["FechaInicio"].ToString()).ToString("dd/MM/yyyy"));
                pExcel.Actualizar(hoja, piRenFechas, piColFechas + 2, "Fin:");
                pExcel.Actualizar(hoja, piRenFechas, piColFechas + 3, "'" + Convert.ToDateTime(HttpContext.Current.Session["FechaFin"].ToString()).ToString("dd/MM/yyyy"));
            }
            if (lhtMeta["{FechasReporteMovil}"] != null)
            {
                int piRenFechas;
                int piColFechas;

                pExcel.BuscarTexto(hoja, "{FechasReporteMovil}", true, out piRenFechas, out piColFechas);
                pExcel.Actualizar(hoja, piRenFechas, piColFechas, "Período:");
                pExcel.Actualizar(hoja, piRenFechas, piColFechas + 1, "'" + MonthName(Convert.ToDateTime(HttpContext.Current.Session["FechaInicio"]).Month) + " " + Convert.ToDateTime(HttpContext.Current.Session["FechaInicio"]).Year.ToString());
            }
        }

        private static string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper();
        }
        
        public static Hashtable BuscarTituloExcel(ExcelAccess pExcel)
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto("Reporte", "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto("Reporte", "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto("Reporte", "{TituloReporte}", true));

            lhtRet.Add("{FechasReporte}", pExcel.BuscarTexto("Reporte", "{FechasReporte}", true));
            lhtRet.Add("{FechasReporteMovil}", pExcel.BuscarTexto("Reporte", "{FechasReporteMovil}", true));

            return lhtRet;
        }
        public static Hashtable BuscarTituloExcel(ExcelAccess pExcel,string hoja)
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto(hoja, "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto(hoja, "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto(hoja, "{TituloReporte}", true));

            lhtRet.Add("{FechasReporte}", pExcel.BuscarTexto(hoja, "{FechasReporte}", true));
            lhtRet.Add("{FechasReporteMovil}", pExcel.BuscarTexto(hoja, "{FechasReporteMovil}", true));

            return lhtRet;
        }
        #region Insertar grafico en excel

        public static void CrearGrafico(DataTable ldt, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                             string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda,
                                             Microsoft.Office.Interop.Excel.XlChartType tipoGraf, ExcelAccess lExcel, string textoPlantilla, string HojaGrafico,
                                             string HojaDatos, float anchoGraf, float alturaGraf)
        {
            ParametrosGrafica lparametrosGraf = ParametrosDeGrafica(ldt, tituloGraf, columnaDatos, leyenda, serieId, EjeX, tituloEjeX, formatoEjeX, tituloEjeY,
                                                             formatoEjeY, mostrarLeyenda);

            ProcesarGraficaExcel(HojaGrafico, HojaDatos, anchoGraf, alturaGraf, 0, 0, lparametrosGraf, lExcel, textoPlantilla, tipoGraf);
        }

        public static ParametrosGrafica ParametrosDeGrafica(DataTable lsDataSource, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                                                           string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda)
        {

            ParametrosGrafica lParametrosGrafica = new ParametrosGrafica();

            lParametrosGrafica.Datos = lsDataSource;
            lParametrosGrafica.Title = tituloGraf;
            lParametrosGrafica.DataColumns = columnaDatos;
            lParametrosGrafica.SeriesNames = leyenda;
            lParametrosGrafica.SeriesIds = serieId;
            lParametrosGrafica.XColumn = EjeX;
            lParametrosGrafica.XIdsColumn = EjeX;
            lParametrosGrafica.XTitle = tituloEjeX;
            lParametrosGrafica.XFormat = formatoEjeX;
            lParametrosGrafica.YTitle = tituloEjeY;
            lParametrosGrafica.YFormat = formatoEjeY;
            lParametrosGrafica.ShowLegend = mostrarLeyenda;
            //lParametrosGrafica.TipoGrafica = tipoGraf;

            return lParametrosGrafica;
        }

        public static Hashtable ProcesarGraficaExcel(string lsHojaGrafico, string lsHojaDatos, float lfWidth, float lfHeight, float lfOffsetX, float lfOffsetY,
                                                                ParametrosGrafica lParametrosGrafica, ExcelAccess lExcel, string cambiarTextoPorGraf, Microsoft.Office.Interop.Excel.XlChartType tipoGrafica)
        {
            FormatoGrafica lFormatoGrafica = new FormatoGrafica();
            lFormatoGrafica.Titulo = lParametrosGrafica.Title;
            lFormatoGrafica.Leyendas = lParametrosGrafica.ShowLegend;
            lFormatoGrafica.XFormat = lParametrosGrafica.XFormat;
            lFormatoGrafica.YFormat = lParametrosGrafica.YFormat;
            Microsoft.Office.Interop.Excel.XlChartType lCharType = tipoGrafica;//GetTipoGraficaExcel(lParametrosGrafica.TipoGrafica);

            return lExcel.InsertarGrafico(lsHojaGrafico, lsHojaDatos, cambiarTextoPorGraf, lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lParametrosGrafica.DataColumns,
                     lParametrosGrafica.SeriesNames, lParametrosGrafica.Datos, lCharType, lfWidth, lfHeight, lfOffsetX, lfOffsetY, lFormatoGrafica);
        }

        #endregion
    }
}