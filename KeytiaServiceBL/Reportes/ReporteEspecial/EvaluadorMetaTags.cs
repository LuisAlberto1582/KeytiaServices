/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Evaluador de los MetaTags de las plantillas para los Reportes Especiales 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Excel = Microsoft.Office.Interop.Excel;
using KeytiaServiceBL.Alarmas;

namespace KeytiaServiceBL.ReporteEspecial
{
    class EvaluadorMetaTags
    {
        protected ExcelAccess poExcel = null;
        protected String psPathEntrada;
        protected String psPathSalida;
        protected DataTable pdtSQL;
        protected Hashtable plstMetaTags = new Hashtable();
        protected String psHoja;
        protected int piRen;
        protected int piCol;
        protected object[,] poRange;
        protected StringBuilder psError = new StringBuilder();
        protected Hashtable phtParams = new Hashtable();
        protected String psIdioma;

        public Hashtable Params
        {
            get { return phtParams; }
            set { phtParams = value; }
        }

        public String PathEntrada
        {
            get { return psPathEntrada; }
            set { psPathEntrada = value; }
        }

        public String PathSalida
        {
            get { return psPathSalida; }
            set { psPathSalida = value; }
        }

        public String Idioma
        {
            get { return psIdioma; }
            set { psIdioma = value; }
        }

        protected ExcelAccess AbrirExcel(String lsExcelPath)
        {
            ExcelAccess loExcel = new ExcelAccess();
            loExcel.FilePath = lsExcelPath;
            loExcel.Abrir();
            return loExcel;
        }

        protected void CerrarExcel(ExcelAccess loExcel)
        {
            try
            {
                loExcel.FilePath = getFileName();
                loExcel.SalvarComo();
            }
            catch (Exception ex)
            {
                Util.LogException("Error al guardar el libro de Excel.", ex);
            }
            finally
            {
                loExcel.Cerrar();
                loExcel.Salir();
                loExcel = null;
            }
        }

        protected string getFileName()
        {
            string lsFileName;
            string lsFileDir;

            if (System.IO.Path.HasExtension(psPathSalida))
            {
                lsFileDir = System.IO.Path.GetDirectoryName(psPathSalida);
                lsFileName = psPathSalida;
            }
            else
            {
                lsFileDir = psPathSalida;
                lsFileName = System.IO.Path.Combine(psPathSalida, System.IO.Path.GetFileName(psPathEntrada));
            }
            System.IO.Directory.CreateDirectory(lsFileDir);
            return lsFileName;
        }

        public void Procesar()
        {
            try
            {
                poExcel = AbrirExcel(psPathEntrada);
                string lsMetaTag;
                lsMetaTag = poExcel.BuscarTexto("{*}", false, out psHoja, out piRen, out piCol);
                while (!string.IsNullOrEmpty(lsMetaTag))
                {
                    Eval(lsMetaTag);
                    lsMetaTag = poExcel.BuscarTexto("{*}", false, out psHoja, out piRen, out piCol);
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error al procesar el libro de Excel.", ex);
                throw ex;
            }
            finally
            {
                if (poExcel != null)
                {
                    CerrarExcel(poExcel);
                }
            }
        }

        protected void Eval(String lsMetaTag)
        {
            bool lbReturn = false;
            if (!initVars(lsMetaTag)) return;

            if (plstMetaTags.Contains("SQL"))
                pdtSQL = ExecuteSQL(plstMetaTags["SQL"].ToString(), phtParams);

            if (plstMetaTags.Contains("INSERTSHEET"))
            {
                InsertSheet();
                return;
            }

            if (plstMetaTags.Contains("INSERTRANGE"))
            {
                InsertRange();
                lbReturn = true;
            }

            if (plstMetaTags.Contains("INSERTCHARTDATA"))
            {
                InsertChart();
                return;
            }

            if (lbReturn)
            {
                return;
            }

            GetRange();

            InsertData();

            RangeFormat();

        }

        protected DataTable ExecuteSQL(string lsSQL, Hashtable Param)
        {
            StringBuilder lsbSQL = new StringBuilder(lsSQL);
            foreach (string lsKey in Param.Keys)
            {
                lsbSQL = lsbSQL.Replace("Param(" + lsKey + ")", Param[lsKey].ToString());
            }
            DataTable ldt = new DataTable();
            try
            {
                ldt = DSODataAccess.Execute(lsbSQL.ToString());
            }
            catch (Exception ex)
            {
                Util.LogException("Error al ejecutar el metatag SQL.", ex);
            }
            return ldt;
        }

        protected bool initVars(String lsMetaTag)
        {
            plstMetaTags.Clear();
            string[] lstTags = lsMetaTag.Substring(0, lsMetaTag.Length - 1)
                                        .Substring(1)
                                        .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            psError = new StringBuilder();
            foreach (string lsTag in lstTags)
            {
                string[] lstOperadores = lsTag.Split(new string[] { ":=" }, StringSplitOptions.RemoveEmptyEntries);
                if (lstOperadores.Length != 2)
                {
                    psError.AppendLine(GetLangItem("ComandoNoValido", lsTag)); // String.Format("Comando no válido ({0})", lsTag)
                    continue;
                }
                string lsOperador1 = lstOperadores[0].Trim().ToUpper();
                string lsOperador2 = lstOperadores[1].Trim().ToUpper();
                object loValor = null;
                bool lbHayError = false;

                switch (lsOperador1)
                {
                    case "SQL":                     //"Exec ZZZZ,p1,p2,p3"  SQL para obtener la información
                        loValor = lstOperadores[1].Trim();
                        break;
                    case "HEADER":                  //"Y" / "N"     Indica si se escribe la METADATA del SQL en el encabezado
                        loValor = (lsOperador2 == "Y");
                        break;
                    case "FOOTER":                  //"Y" / "N"     Indica si se escribe el ultimo renglón del SQL como pie de pagina
                        loValor = (lsOperador2 == "Y");
                        break;
                    case "ROWS":                    //0             Numero tope de renglones a vaciar, 0 indica sin tope.
                        int liRows = 0;
                        lbHayError = !int.TryParse(lsOperador2, out liRows);
                        if (lbHayError)
                            psError.AppendLine(GetLangItem("TagNoNumerica", "Rows")); //"El valor de la tag Rows no es numérico"
                        loValor = liRows;
                        break;
                    case "COLS":                    //0             Numero tope de columnas a vaciar, 0 indica sin tope.
                        int liCols = 0;
                        lbHayError = !int.TryParse(lsOperador2, out liCols);
                        if (lbHayError)
                            psError.AppendLine(GetLangItem("TagNoNumerica", "Cols")); //"El valor de la tag Cols no es numérico"
                        loValor = liCols;
                        break;
                    case "DELETECELLS":             //"A1:B2"       Indica que se eliminarán celdas después de llenar el rango
                        loValor = lsOperador2;
                        break;
                    case "INSERTCELLS":             //"H" / "V"     Indica que los datos insertan una celda Horizontal / Vertical
                        if (lsOperador2 == "H" || lsOperador2 == "V")
                            loValor = lsOperador2;
                        else
                        {
                            psError.AppendLine(GetLangItem("ValorTagNoValido", "InsertCells", "(H / V)")); //"El valor de la tag InsertCells no es válido (H / V)"
                            loValor = "";
                        }
                        break;
                    case "INSERTROW":               //"B" / "A"     Indica que los datos insertan un renglón antes / después
                        if (lsOperador2 == "A" || lsOperador2 == "B")
                            loValor = lsOperador2;
                        else
                        {
                            psError.AppendLine(GetLangItem("ValorTagNoValido", "InsertRow", "(A / B)")); //"El valor de la tag InsertRow no es válido (A / B)"
                            loValor = "";
                        }
                        break;
                    case "INSERTCOL":               //"L" / "R"     Indica que los datos insertan una columna a la Izquierda / Derecha
                        if (lsOperador2 == "L" || lsOperador2 == "R")
                            loValor = lsOperador2;
                        else
                        {
                            psError.AppendLine(GetLangItem("ValorTagNoValido", "InsertCol", "(L / R)")); //"El valor de la tag InsertCol no es válido (L / R)"
                            loValor = "";
                        }
                        break;
                    case "INSERTCHARTRANGE":             //"XXXX"        Indica que se inserta una hoja antes de la hoja “XXXX”
                        loValor = lsOperador2;
                        break;
                    case "INSERTCHARTDATA":             //"XXXX"        Indica que se inserta una hoja antes de la hoja “XXXX”
                        loValor = lsOperador2;
                        break;
                    case "INSERTCHARTTYPE":             //"XXXX"        Indica que se inserta una hoja antes de la hoja “XXXX”
                        int liChartType = 0;
                        lbHayError = !int.TryParse(lsOperador2, out liChartType);
                        if (lbHayError)
                            psError.AppendLine(GetLangItem("TagNoNumerica", lstOperadores[0].Trim())); //"El valor de la tag lstOperadores[0] no es numérico"
                        else
                        {
                            try
                            {
                                Excel.XlChartType xlChartType = (Excel.XlChartType)liChartType;
                            }
                            catch (Exception ex)
                            {
                                lbHayError = true;
                                psError.AppendLine(GetLangItem("ValorTagNoValido", lstOperadores[0].Trim(), "(XlChartType)")); //"El valor de la tag lstOperadores[0] no es válido (0 / 56)"
                            }
                        }
                        if (!lbHayError)
                        {
                            loValor = liChartType;
                        }
                        break;
                    case "INSERTSHEET":             //"XXXX"        Indica que se inserta una hoja antes de la hoja “XXXX”
                        loValor = (lsOperador2 != "N");
                        break;
                    case "INSERTRANGE":             //"A1:B2"       Indica que se copia y pega un rango de celdas
                        loValor = lsOperador2;
                        break;
                    case "CODGROUP":                //"XXXX"        Dato que indica el grupo para insertar rangos, hojas o libros
                        loValor = lstOperadores[1].Trim();
                        break;
                    case "DESGROUP":                //"XXXX"        Dato que indica la descripción para las hojas o libros.
                        loValor = lstOperadores[1].Trim();
                        break;
                    case "COLHEADERBACKCOLOR":      //"XXX"         Color para las celdas del encabezado de columnas
                    case "ROWHEADERBACKCOLOR":      //"XXX"         Color para las celdas del encabezado de renglones
                    case "DETAILBACKCOLOR":         //"XXX"         Color para las celdas del detalle
                    case "FOOTERBACKCOLOR":         //"XXX"         Color para las celdas del pie de pagina
                    case "COLHEADERFORECOLOR":      //"XXX"         Color para los textos del encabezado de columnas
                    case "ROWHEADERFORECOLOR":      //"XXX"         Color para los textos del encabezado de renglones
                    case "DETAILFORECOLOR":         //"XXX"         Color para los textos del detalle
                    case "FOOTERFORECOLOR":         //"XXX"         Color para los textos del pie de pagina
                        int liColorIndex = 0;
                        lbHayError = !int.TryParse(lsOperador2, out liColorIndex);
                        if (lbHayError)
                            psError.AppendLine(GetLangItem("TagNoNumerica", lstOperadores[0].Trim())); //"El valor de la tag lstOperadores[0] no es numérico"
                        else if(!(liColorIndex >=0 && liColorIndex <=56))
                            psError.AppendLine(GetLangItem("ValorTagNoValido", lstOperadores[0].Trim(), "(0 - 56)")); //"El valor de la tag lstOperadores[0] no es válido (0 / 56)"
                        else
                            loValor = liColorIndex;
                        break;
                    case "COLHEADERFONT":           //"XXX"         Fuente para los textos del encabezado de columnas
                    case "ROWHEADERFONT":           //"XXX"         Fuente para los textos del encabezado de renglones
                    case "DETAILFONT":              //"XXX"         Fuente para los textos del detalle
                    case "FOOTERFONT":              //"XXX"         Fuente para los textos del pie de pagina
                        loValor = lstOperadores[1].Trim();
                        break;
                    case "COLHEADERFORMAT":         //"XXX"         Formato para los textos del encabezado de columnas
                    case "ROWHEADERFORMAT":         //"XXX"         Formato para los textos del encabezado de renglones
                    case "DETAILFORMAT":            //"XXX"         Formato para los textos del detalle
                    case "FOOTERFORMAT":            //"XXX"         Formato para los textos del pie de pagina
                        loValor = lstOperadores[1].Trim();
                        break;
                    default:
                        psError.AppendLine(GetLangItem("TagNoValida", lstOperadores[0].Trim())); //"La tag lstOperadores[0] no es válida"
                        break;
                }
                if (psError.Length > 0)
                    poExcel.Actualizar(psHoja, piRen, piCol, psError.ToString());
                else
                    plstMetaTags.Add(lsOperador1, loValor);
            }
            return psError.Length == 0;
        }

        protected void InsertSheet()
        {
            if (!plstMetaTags.Contains("DESGROUP"))
            {
                psError.AppendLine(GetLangItem("TagNoEncontrada", "DesGroup")); //"Se necesita el valor de la Tag DesGroup"
            }
            if (!plstMetaTags.Contains("CODGROUP"))
            {
                psError.AppendLine(GetLangItem("TagNoEncontrada", "CodGroup")); //"Se necesita el valor de la Tag CodGroup"
            }
            if (psError.Length > 0)
            {
                poExcel.Actualizar(psHoja, piRen, piCol, psError.ToString());
                return;
            }
            string lsDesGroup = plstMetaTags["DESGROUP"].ToString();
            string lsCodGroup = plstMetaTags["CODGROUP"].ToString();
            for (int i = pdtSQL.Rows.Count - 1; i >= 0; i--)
            {
                DataRow ldr = pdtSQL.Rows[i];
                poExcel.Copiar(psHoja, ldr[lsDesGroup].ToString());
                poExcel.Actualizar(ldr[lsDesGroup].ToString(), piRen, piCol, ldr[lsCodGroup].ToString());
            }
            poExcel.Remover(psHoja);
        }

        protected void InsertRange()
        {
            string lsInsertRange = "";
            Excel.XlDirection leDirectionDel;
            Excel.XlInsertShiftDirection leDirection;
            Excel.XlInsertFormatOrigin leInsertFormat = Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow;

            if (!plstMetaTags.Contains("CODGROUP"))
            {
                psError.AppendLine(GetLangItem("TagNoEncontrada", "CodGroup")); //"Se necesita el valor de la Tag CodGroup"
            }
            if (psError.Length > 0)
            {
                poExcel.Actualizar(psHoja, piRen, piCol, psError.ToString());
                return;
            }
            if (!plstMetaTags.Contains("INSERTCELLS") || plstMetaTags["INSERTCELLS"] == "V")
            {
                leDirection = Excel.XlInsertShiftDirection.xlShiftDown;
                leDirectionDel = Excel.XlDirection.xlUp;
            }
            else
            {
                leDirection = Excel.XlInsertShiftDirection.xlShiftToRight;
                leDirectionDel = Excel.XlDirection.xlToLeft;
            }

            try
            {
                lsInsertRange = plstMetaTags["INSERTRANGE"].ToString();
                string lsCodGroup = plstMetaTags["CODGROUP"].ToString();
                for (int i = pdtSQL.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow ldr = pdtSQL.Rows[i];
                    poExcel.Actualizar(psHoja, piRen, piCol, ldr[lsCodGroup].ToString());
                    poExcel.CopiarRango(psHoja, lsInsertRange, leInsertFormat, leDirection);
                }
                poExcel.Eliminar(psHoja, lsInsertRange, leDirectionDel);
            }
            catch (Exception ex)
            {
                psError.AppendLine(GetLangItem("ComandoNoValido", "InsertRange:=" + lsInsertRange)); // String.Format("Comando no válido ({0})", lsTag)
                poExcel.Actualizar(psHoja, piRen, piCol, psError.ToString() + "\n" + ex.Message);
            }
        }

        protected void InsertChart()
        {
            Excel.XlChartType xlChartType = Excel.XlChartType.xl3DArea;
            string lsInsertChartData = "";
            string lsInsertChartRange = "";

            if (!plstMetaTags.Contains("INSERTCHARTRANGE"))
            {
                psError.AppendLine(GetLangItem("TagNoEncontrada", "InsertChartRange")); //"Se necesita el valor de la Tag InsertChartData"
            }
            if (!plstMetaTags.Contains("INSERTCHARTTYPE"))
            {
                psError.AppendLine(GetLangItem("TagNoEncontrada", "InsertChartType")); //"Se necesita el valor de la Tag InsertChartType"
            }
            if (psError.Length > 0)
            {
                poExcel.Actualizar(psHoja, piRen, piCol, psError.ToString());
                return;
            }

            try
            {
                lsInsertChartRange = plstMetaTags["INSERTCHARTRANGE"].ToString();
                lsInsertChartData = plstMetaTags["INSERTCHARTDATA"].ToString();
                xlChartType = (Excel.XlChartType)plstMetaTags["INSERTCHARTTYPE"];

                poExcel.Actualizar(psHoja, piRen, piCol, "");

                if (plstMetaTags.Contains("INSERTCELLS"))
                {
                    Excel.XlInsertShiftDirection leDirection;
                    Excel.XlInsertFormatOrigin leInsertFormat = Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow;
                    if (plstMetaTags["INSERTCELLS"].ToString() == "H")
                    {
                        leDirection = Excel.XlInsertShiftDirection.xlShiftToRight;
                    }
                    else
                    {
                        leDirection = Excel.XlInsertShiftDirection.xlShiftDown;
                    }
                    poExcel.InsertarCeldas(psHoja, lsInsertChartRange, leInsertFormat, leDirection);
                }

                poExcel.InsertarGrafica(psHoja, psHoja, "", lsInsertChartData, "", xlChartType, lsInsertChartRange);
            }
            catch (Exception ex)
            {
                psError.AppendLine(GetLangItem("ComandoNoValido", "InsertChart:=" + lsInsertChartData)); // String.Format("Comando no válido ({0})", lsTag)
                poExcel.Actualizar(psHoja, piRen, piCol, psError.ToString() + "\n" + ex.Message);
            }
        }

        protected void GetRange()
        {
            int liRenglones = pdtSQL.Rows.Count,
                liColumnas = pdtSQL.Columns.Count;
            int liInicio = 0;

            if (plstMetaTags.Contains("ROWS") && (int)plstMetaTags["ROWS"] > 0)
            {
                liRenglones = Math.Min((int)plstMetaTags["ROWS"], liRenglones);
            }

            if (plstMetaTags.Contains("COLS") && (int)plstMetaTags["COLS"] > 0)
            {
                liColumnas = Math.Min((int)plstMetaTags["COLS"], liColumnas);
            }
            if (plstMetaTags.Contains("HEADER") && (bool)plstMetaTags["HEADER"])
            {
                liInicio = 1;
                poRange = new object[++liRenglones, liColumnas];
                for (int i = 0; i < liColumnas; i++)
                {
                    poRange[0, i] = pdtSQL.Columns[i].ColumnName;
                }
            }
            else
            {
                poRange = new object[liRenglones, liColumnas];
            }

            for (int i = liInicio; i < liRenglones; i++)
            {
                for (int j = 0; j < liColumnas; j++)
                {
                    poRange[i, j] = pdtSQL.Rows[i - liInicio][j];
                }
            }
            if (liRenglones == 0)
            {
                if (plstMetaTags.Contains("COLS") && (int)plstMetaTags["COLS"] > 0)
                    liColumnas = Math.Max((int)plstMetaTags["COLS"], liColumnas);
                poRange = new object[1, Math.Max(1, liColumnas)];
                poRange[0, 0] = GetLangItem("SinRegistros");
            }

        }

        protected void InsertData()
        {
            int liRenglon = piRen,
                liColumna = piCol;
                Excel.XlInsertShiftDirection leDirection = Excel.XlInsertShiftDirection.xlShiftDown;
            if (plstMetaTags.Contains("INSERTCELLS"))
            {
                int liInsertRows = poRange.GetUpperBound(0),
                    liInsertCols = poRange.GetUpperBound(1);

                string lsRangoOrigenC1,
                       lsRangoOrigenC2,
                       lsRangoDestinoC1,
                       lsRangoDestinoC2;

                //La enumeración XlInsertFormatOrigin indica de dónde se toma el formato al insertar las celdas:
                //Si se inserta la celda Verticalmente, se puede tomar el formato de encima o de abajo
                //Si se inserta la celda Horizontalmente, se puede tomar el formato de la izquierda o derecha
                Excel.XlInsertFormatOrigin leInsertFormat = Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow;

                //La enumeración XlInsertShiftDirection indica cómo insertar las celdas: Horizontal o Verticalmente:

                if ((string)plstMetaTags["INSERTCELLS"] == "H")
                {
                    leDirection = Excel.XlInsertShiftDirection.xlShiftToRight;
                    liInsertCols--;
                    if (plstMetaTags.Contains("INSERTCOL"))
                    {
                        if (plstMetaTags["INSERTCOL"].ToString() == "R")
                        {
                            leInsertFormat = Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow;
                        }
                        else
                        {
                            leInsertFormat = Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove;
                        }
                    }
                    if (leInsertFormat == Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove)
                    {
                        lsRangoOrigenC1 = poExcel.ObtenerNombreCelda(psHoja, liRenglon, liColumna);
                        lsRangoOrigenC2 = poExcel.ObtenerNombreCelda(psHoja, liRenglon + liInsertRows, liColumna);
                    }
                    else
                    {
                        lsRangoOrigenC1 = poExcel.ObtenerNombreCelda(psHoja, liRenglon, liColumna + liInsertCols + 1);
                        lsRangoOrigenC2 = poExcel.ObtenerNombreCelda(psHoja, liRenglon + liInsertRows, liColumna + liInsertCols + 1);
                    }
                }
                else
                {
                    leDirection = Excel.XlInsertShiftDirection.xlShiftDown;
                    liInsertRows--;
                    if (plstMetaTags.Contains("INSERTROW"))
                    {
                        if (plstMetaTags["INSERTROW"].ToString() == "A")
                        {
                            leInsertFormat = Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow;
                        }
                        else
                        {
                            leInsertFormat = Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove;
                        }
                    }
                    if (leInsertFormat == Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove)
                    {
                        lsRangoOrigenC1 = poExcel.ObtenerNombreCelda(psHoja, Math.Max(liRenglon - 1, 1), liColumna);
                        lsRangoOrigenC2 = poExcel.ObtenerNombreCelda(psHoja, Math.Max(liRenglon - 1, 1), liColumna + liInsertCols);
                    }
                    else
                    {
                        lsRangoOrigenC1 = poExcel.ObtenerNombreCelda(psHoja, liRenglon + liInsertRows + 1, liColumna);
                        lsRangoOrigenC2 = poExcel.ObtenerNombreCelda(psHoja, liRenglon + liInsertRows + 1, liColumna + liInsertCols);
                    }
                }

                if (liInsertRows >= 0 && liInsertCols >= 0)
                {
                    poExcel.InsertarCeldas(psHoja, liRenglon, liColumna, liRenglon + liInsertRows, liColumna + liInsertCols, leInsertFormat, leDirection);
                    lsRangoDestinoC1 = poExcel.ObtenerNombreCelda(psHoja, liRenglon, liColumna);
                    lsRangoDestinoC2 = poExcel.ObtenerNombreCelda(psHoja, liRenglon + liInsertRows, liColumna + liInsertCols);

                    poExcel.CopiarFormato(psHoja, lsRangoOrigenC1, lsRangoOrigenC2, lsRangoDestinoC1, lsRangoDestinoC2);
                }
            }
            int liRenglon2 = liRenglon + poRange.GetUpperBound(0);
            int liColumna2 = liColumna + poRange.GetUpperBound(1);
            poExcel.Actualizar(psHoja, liRenglon, liColumna, liRenglon2, liColumna2, poRange);
            if (plstMetaTags.Contains("DELETECELLS"))
            {
                Excel.XlDirection lxlDirection = leDirection == Excel.XlInsertShiftDirection.xlShiftDown ?
                    Excel.XlDirection.xlUp :
                    Excel.XlDirection.xlToLeft;

                string lsDeleteCellsRange = plstMetaTags["DELETECELLS"].ToString();
                poExcel.Eliminar(psHoja, lsDeleteCellsRange, lxlDirection);
            }
           
        }

        protected void RangeFormat()
        {
            HeaderFormat();
            DetailFormat();
            FooterFormat();
        }

        protected void HeaderFormat()
        {
            if (plstMetaTags.Contains("HEADER") && (bool)plstMetaTags["HEADER"])
            {
                SetFormat("ROWHEADER", piRen, piCol, piRen, piCol + poRange.GetUpperBound(1));
            }
        }

        protected void DetailFormat()
        {
            int liRen1 = piRen,
                liRen2 = piRen + poRange.GetUpperBound(0),
                liCol2 = piCol + poRange.GetUpperBound(1);
            if (plstMetaTags.Contains("HEADER") && (bool)plstMetaTags["HEADER"])
            {
                liRen1++;
            }
            if (plstMetaTags.Contains("FOOTER") && (bool)plstMetaTags["FOOTER"])
            {
                liRen2--;
            }
            SetFormat("DETAIL", liRen1, piCol, liRen2, liCol2);
            SetFormat("COLHEADER", liRen1, piCol, liRen2, piCol);
        }

        protected void FooterFormat()
        {
            if (plstMetaTags.Contains("FOOTER") && (bool)plstMetaTags["FOOTER"])
            {
                int liRen = piRen + poRange.GetUpperBound(0),
                    liCol2 = piCol + poRange.GetUpperBound(1);
                SetFormat("FOOTER", liRen, piCol, liRen, liCol2);
            }
        }

        protected void SetFormat(string lsSeccion, int liRen1, int liCol1, int liRen2, int liCol2)
        {
            int liBackColorIndex;
            object lsFontName = null,
                liFontSize = null,
                liForeColorIndex = null,
                liUnderline = null,
                lbBold = null,
                lbItalic = null;
            bool lbHayError;
            if (plstMetaTags.Contains(lsSeccion + "BACKCOLOR"))
            {
                liBackColorIndex = (int)plstMetaTags[lsSeccion + "BACKCOLOR"];
                poExcel.ColorearFondo(psHoja, liRen1, liCol1, liRen2, liCol2, liBackColorIndex);
            }
            if (plstMetaTags.Contains(lsSeccion + "FORECOLOR"))
            {
                liForeColorIndex = (int)plstMetaTags[lsSeccion + "FORECOLOR"];
            }
            if (plstMetaTags.Contains(lsSeccion + "FONT"))
            {
                //Ejemplo: Size=15|Italic=Y|Name=Arial
                string lsFont = (string)plstMetaTags[lsSeccion + "FONT"];
                string[] lstFont = lsFont.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string lsComando in lstFont)
                {
                    string[] lstOperadores = lsComando.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lstOperadores.Length != 2) continue;
                    int liAux;
                    switch (lstOperadores[0].Trim().ToUpper())
                    {
                        case "SIZE":
                            lbHayError = !int.TryParse(lstOperadores[1].Trim(), out liAux);
                            if (!lbHayError && liAux >= 1 && liAux <= 409)
                            {
                                liFontSize = liAux;
                            }
                            break;
                        case "NAME":
                            lsFontName = lstOperadores[1].Trim();
                            break;
                        case "BOLD":
                            lbBold = (lstOperadores[1].Trim().ToUpper() == "Y");
                            break;
                        case "ITALIC":
                            lbItalic = (lstOperadores[1].Trim().ToUpper() == "Y");
                            break;
                        case "STYLE":
                            lbBold = (lstOperadores[1].Trim().ToUpper().IndexOf("BOLD") >= 0);
                            lbItalic = (lstOperadores[1].Trim().ToUpper().IndexOf("ITALIC") >= 0);
                            break;
                        case "UNDERLINE":
                            lbHayError = !int.TryParse(lstOperadores[1].Trim(), out liAux);
                            if (!lbHayError && liAux >= 1 && liAux <= 5)
                            {
                                liUnderline = liAux;
                            }
                            break;
                    }
                }
            }
            if (plstMetaTags.Contains(lsSeccion + "FORMAT"))
            {
                string lsFormat = plstMetaTags[lsSeccion + "FORMAT"].ToString();
                poExcel.SetNumberFormat(psHoja, liRen1, liCol1, liRen2, liCol2, lsFormat);
            }
            poExcel.SetFont(psHoja, liRen1, liCol1, liRen2, liCol2, lsFontName, liFontSize, liForeColorIndex, lbBold, lbItalic, liUnderline);
        }

        #region Idioma

        protected string GetLangItem(string lsElemento, params object[] lsParam)
        {
            return Alarma.GetLangItem(psIdioma, "MsgWeb", "Mensajes Reporte Especial", lsElemento, lsParam);
        }

        #endregion
    }
}
