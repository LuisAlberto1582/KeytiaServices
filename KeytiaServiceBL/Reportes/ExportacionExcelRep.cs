using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace KeytiaServiceBL.Reportes
{
    class ExportacionExcelRep
    {
        public static void CreaTablaEnExcel(ExcelAccess lExcel, DataTable ldt, string hoja, string textoBusqueda)
        {
            object[,] datos = lExcel.DataTableToArray(daFormatoACeldas(ldt), true);

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

        public static void ProcesarTituloExcel(ExcelAccess pExcel, string titulo, string lsStylePath, string lsKeytiaWebFPath, string logoExportacion, DateTime fechaInicio, DateTime fechaFin)
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

            //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
            if (!String.IsNullOrEmpty(logoExportacion))
            {
                lsImg = logoExportacion;
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
                lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"], lsImg, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating, 70);
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
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + fechaInicio.ToString("dd/MM/yyyy"));
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 2, "Fin:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 3, "'" + fechaFin.ToString("dd/MM/yyyy"));
            }
            if (lhtMeta["{FechasReporteMovil}"] != null)
            {
                int piRenFechas;
                int piColFechas;

                pExcel.BuscarTexto("Reporte", "{FechasReporteMovil}", true, out piRenFechas, out piColFechas);
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas, "Período:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + MonthName(fechaInicio.Month) + " " + fechaInicio.Year.ToString());
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

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static DataTable daFormatoACeldas(DataTable dataTable)
        {
            DataTable ldt = new DataTable();
            int numeroCol = dataTable.Columns.Count;
            List<object> valores = new List<object>();
            try
            {
                foreach (DataColumn ldc in dataTable.Columns)
                {
                    ldt.Columns.Add(new DataColumn(ldc.ColumnName, typeof(string)));
                }
                foreach (DataRow ldr in dataTable.Rows)
                {
                    foreach (object obj in ldr.ItemArray)
                    {
                        valores.Add(FormatearValor(obj));
                    }
                    ldt.Rows.Add(valores.ToArray());
                    valores.Clear();
                }

            }
            catch (Exception ex)
            {
                string lsEx = ex.Message.ToString();
            }
            return ldt;
        }

        public static string FormatearValor(object lValor)
        {
            string lsRet = "";

            if (lValor is int)
            {
                lsRet = ((int)lValor).ToString("#,0");
            }
            else if (lValor is double && lValor.ToString().Contains("."))
            {
                lsRet = ((double)lValor).ToString("$#,0.00");
            }
            else if (lValor is decimal && lValor.ToString().Contains("."))
            {
                lsRet = ((decimal)lValor).ToString("$#,0.00");
            }
            else if (lValor is DateTime)
            {
                //20140425 RJ.Le quité la concatenación de comillas sencillas al inicio y al final de la fecha
                //por requierimiento del cliente Capitel
                lsRet = "" + ((DateTime)lValor).ToString("yyyy/MM/dd HH:mm:ss") + "";
            }
            else
            {
                lsRet = lValor.ToString();
            }
            return lsRet;
        }
    }
}
