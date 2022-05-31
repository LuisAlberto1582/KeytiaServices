using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using KeytiaServiceBL;
using System.Text.RegularExpressions;
using KeytiaWeb.UserInterface.DashboardLT;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public class FCAndControls
    {
        public static string cargaScriptsFusionCharts()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("    <script src=\"scripts/fusioncharts.js\" type=\"text/javascript\"  id=\"JSMain\"></script>\n");

            //Los temas deben de ir despues del fusioncharts.js
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.carbon.js\" type=\"text/javascript\"></script>\n ");
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.fint.js\" type=\"text/javascript\"></script>\n ");
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.ocean.js\" type=\"text/javascript\"></script>\n ");
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.zune.js\" type=\"text/javascript\"></script>\n ");
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.verdeAzul.js\" type=\"text/javascript\"></script>\n ");
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.rojoCafe.js\" type=\"text/javascript\"></script>\n ");
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.verdeCafe.js\" type=\"text/javascript\"></script>\n ");
            lsb.Append("    <script src=\"scripts/themes/fusioncharts.theme.ColorsRandom.js\" type=\"text/javascript\"></script>\n ");


            lsb.Append("    <script src=\"scripts/fusioncharts.charts.js\" type=\"text/javascript\" id=\"JScharts\"></script>\n");

            //lsb.Append("    <script src=\"scripts/jqueryCombo.charts.js\" type=\"text/javascript\" id=\"JScharts\"></script>\n");
            //lsb.Append("    <script src=\"scripts/fusioncharts.gantt.js\" type=\"text/javascript\"  id=\"JSgantt\"></script>\n");
            //lsb.Append("    <script src=\"scripts/fusioncharts.maps.js\" type=\"text/javascript\"  id=\"JSmaps\"></script>\n");
            //lsb.Append("    <script src=\"scripts/fusioncharts.powercharts.js\" type=\"text/javascript\"  id=\"JSpowercharts\"></script>\n");
            //lsb.Append("    <script src=\"scripts/fusioncharts.widgets.js\" type=\"text/javascript\"  id=\"JSwidgets\"></script>\n");

            //NZ Se elimina esta css. Los estilos que contenia esta css se mueven a la css de Keytia.css Para el mejor manejo de los estilos de la pagina.
            //lsb.Append("    <link href=\"styles/FCStyles.css\" rel=\"stylesheet\" type=\"text/css\" />\n");

            return lsb.ToString();
        }

        public static string grafica1Serie(string DataSourceJSon, string tipoGrafica, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY,
                                                   string nombreTemaGraf, string ancho, string alto, string classNameRadioBtns)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("<script type=\"text/javascript\">\n ");
            lsb.Append("  FusionCharts.ready(function(){\n ");
            lsb.Append("        var radio = [], radElem, val, " + idContenedor + " = new FusionCharts({\n ");
            lsb.Append("        \"type\": \"" + tipoGrafica + "\",\n ");
            lsb.Append("        \"renderAt\": \"" + idContenedor + "\",\n ");
            lsb.Append("        \"width\": \"" + ancho + "\",\n ");
            lsb.Append("        \"height\": \"" + alto + "\",\n ");
            lsb.Append("        \"dataFormat\": \"json\",\n ");
            lsb.Append("        \"dataSource\":  {\n ");
            lsb.Append("          \"chart\": {\n ");
            lsb.Append("            \"caption\": \"" + titulo + "\",\n ");
            lsb.Append("            \"subCaption\": \"" + segundoTitulo + "\",\n ");
            lsb.Append("            \"xAxisName\": \"" + ejeX + "\",\n ");
            lsb.Append("            \"yAxisName\": \"" + ejeY + "\",\n ");
            lsb.Append("            \"formatNumberScale\": \"0\",\n ");
            lsb.Append("            \"numberPrefix\": \"$\",\n ");
            lsb.Append("            \"showlabels\": \"1\",\n ");
            lsb.Append("            \"showvalues\": \"0\",\n ");
            lsb.Append("            \"decimals\": \"2\",\n ");
            lsb.Append("            \"decimalSeparator\": \".\",\n ");
            lsb.Append("            \"thousandSeparator\": \",\",\n ");
            //lsb.Append("            \"exportEnabled\": \"1\",\n ");
            lsb.Append("            \"theme\": \"" + nombreTemaGraf + "\",\n ");
            lsb.Append("            \"labelDisplay\": \"rotate\",\n ");
            lsb.Append("            \"slantLabels\": \"1\",\n ");
            lsb.Append("         },\n ");
            lsb.Append("         \"data\": " + DataSourceJSon);
            lsb.Append("      }\n ");
            lsb.Append("  });\n ");
            lsb.Append(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte para el rango de fechas seleccionadas.\"); \n ");
            lsb.Append(idContenedor + ".render();\n ");
            lsb.Append("    radio = document.getElementsByClassName('" + classNameRadioBtns + "');\n ");
            lsb.Append("    for (i = 0; i < radio.length; i++) {\n ");
            lsb.Append("        radElem = radio[i];\n ");
            lsb.Append("        if (radElem.type === 'radio') {\n ");
            lsb.Append("            radElem.onclick = function(){\n ");
            lsb.Append("                val = this.getAttribute('value');\n ");
            lsb.Append("                val && " + idContenedor + ".chartType(val);\n ");
            lsb.Append("            };\n ");
            lsb.Append("        }\n ");
            lsb.Append("    }\n ");
            lsb.Append("})\n ");
            lsb.Append("</script>\n ");
            return lsb.ToString();
        }


        public static string grafica1Serie(string DataSourceJSon, string tipoGrafica, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY,
                                                  string nombreTemaGraf, string ancho, string alto, string classNameRadioBtns, bool incluirSignoPesos)
        {
            string lsb = grafica1Serie(DataSourceJSon, tipoGrafica, idContenedor, titulo, segundoTitulo, ejeX, ejeY,
                                                  nombreTemaGraf, ancho, alto, classNameRadioBtns);


            if (!incluirSignoPesos)
            {
                lsb = lsb.Replace("\"numberPrefix\": \"$\",", string.Empty);

            }

            return lsb;
        }


        public static string graficaMultiSeries(string[] DataSourceJSon, string[] nombreSeries, string tipoGrafica, string idContenedor, string titulo, string segundoTitulo,
                                                          string ejeX, string ejeY, string nombreTemaGraf, string ancho, string alto, string classNameRadioBtns, bool scriptTags)
        {
            List<string> DataSeries = new List<string>();

            StringBuilder sbSerie = new StringBuilder();
            for (int i = 1; i < DataSourceJSon.Length; i++)
            {
                sbSerie.Length = 0;
                sbSerie.Append("\"seriesname\": \"");
                sbSerie.Append(nombreSeries[i]);
                sbSerie.Append("\",\"data\":");
                sbSerie.Append(DataSourceJSon[i]);
                DataSeries.Add(sbSerie.ToString());
            }

            StringBuilder lsb = new StringBuilder();
            if (scriptTags)
            {
                lsb.Append("<script type=\"text/javascript\"> \r");
            }
            lsb.Append("FusionCharts.ready(function () { \r");
            lsb.Append("    var radio = [], radElem, val, " + idContenedor + " = new FusionCharts({ \r");
            lsb.Append("        \"type\": \"" + tipoGrafica + "\", \r");
            lsb.Append("        \"renderAt\": \"" + idContenedor + "\", \r");
            lsb.Append("        \"width\": \"" + ancho + "\", \r");
            lsb.Append("        \"height\": \"" + alto + "\", \r");
            lsb.Append("        \"dataFormat\": \"json\", \r");
            lsb.Append("        \"dataSource\": { \r");
            lsb.Append("           \"chart\": { \r");
            lsb.Append("              \"caption\": \"" + titulo + "\", \r");
            lsb.Append("              \"subCaption\": \"" + segundoTitulo + "\", \r");
            lsb.AppendLine("            \"setAdaptiveYMin\": \"1\", ");
            lsb.Append("              \"xAxisname\": \"" + ejeX + "\", \r");
            lsb.Append("              \"yAxisName\": \"" + ejeY + "\", \r");
            lsb.Append("            \"numberPrefix\": \"$\",\n ");
            lsb.Append("            \"showlabels\": \"1\",\n ");
            lsb.Append("            \"showvalues\": \"0\",\n ");
            //lsb.Append("            \"exportEnabled\": \"1\",\n ");
            lsb.Append("              \"theme\": \"" + nombreTemaGraf + "\", \r");
            lsb.Append("            \"labelDisplay\": \"rotate\",\n ");
            lsb.Append("            \"slantLabels\": \"1\",\n ");
            lsb.Append("           }, \r");
            lsb.Append("           \"categories\": [ \r");
            lsb.Append("              { \r");
            lsb.Append("                 \"category\": " + DataSourceJSon[0] + " \r");
            lsb.Append("              } \r");
            lsb.Append("           ], \r");
            lsb.Append("           \"dataset\": [ \r");
            lsb.Append("              { \r");
            lsb.Append(String.Join("},{", DataSeries.ToArray()));
            lsb.Append("              } \r");
            lsb.Append("           ] \r");
            lsb.Append("        } \r");
            lsb.Append("    }); \r");
            lsb.Append(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte para el rango de fechas seleccionadas.\"); \n ");
            lsb.Append(idContenedor + ".render(); \r");

            lsb.Append("    radio = document.getElementsByClassName('" + classNameRadioBtns + "');\n ");
            lsb.Append("    for (i = 0; i < radio.length; i++) {\n ");
            lsb.Append("        radElem = radio[i];\n ");
            lsb.Append("        if (radElem.type === 'radio') {\n ");
            lsb.Append("            radElem.onclick = function(){\n ");
            lsb.Append("                val = this.getAttribute('value');\n ");
            lsb.Append("                val && " + idContenedor + ".chartType(val);\n ");
            lsb.Append("            };\n ");
            lsb.Append("        }\n ");
            lsb.Append("    }\n ");

            lsb.Append("}); \r");
            if (scriptTags)
            {
                lsb.Append("</script> \r");
            }

            return lsb.ToString();
        }


        /// <summary>
        /// Convierte los valores de un DataTable en un string en formato de JSON. Se usa para crear las graficas de una sola serie de FusionCharts
        /// </summary>
        /// <param name="dt">DataTable a convertir</param>
        /// <returns></returns>
        public static string ConvertDataTabletoJSONString(DataTable dt)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer =
                    new System.Web.Script.Serialization.JavaScriptSerializer();
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }

                return serializer.Serialize(rows);
            }
            catch (Exception Ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Convierte un DataTable[] en un string[] con formato de JSON. Se usa para crear las graficas multiserie de FusionCharts
        /// </summary>
        /// <param name="dta">Arreglo de DataTable que se desea serializar</param>
        /// <returns></returns>
        public static string[] ConvertDataTabletoJSONString(DataTable[] dta)
        {
            List<string> lsa = new List<string>();
            foreach (DataTable ldt in dta)
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in ldt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in ldt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                lsa.Add(serializer.Serialize(rows));
            }

            return lsa.ToArray<string>();
        }

        /// <summary>
        /// Te regresa los nombres de las columnas de un DataTable en un string[]
        /// </summary>
        /// <param name="dt">DataTable del cual se requiere el nombre de las columnas</param>
        /// <returns></returns>
        public static string[] extraeNombreColumnas(DataTable dt)
        {
            List<string> lsa = new List<string>();
            try
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    lsa.Add(dc.ColumnName);
                }
                return lsa.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Convierte cada una de las columnas de un DataTable en un DataTable nuevo y lo regresa en un DataTable[]. Se usa para crear las graficas multiserie de FusionCharts
        /// </summary>
        /// <param name="dt">DataTable a convertir</param>
        /// <returns></returns>
        public static DataTable[] ConvertDataTabletoDataTableArray(DataTable dt)
        {
            List<DataTable> ldta = new List<DataTable>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataTable ldt = new DataTable();
                DataView dv = new DataView(dt);
                ldt = dv.ToTable(false, new string[] { dt.Columns[i].ColumnName });
                if (i == 0)
                {
                    ldt.Columns[0].ColumnName = "label";
                }
                else
                {
                    ldt.Columns[0].ColumnName = "value";
                }
                ldta.Add(ldt);

            }
            return ldta.ToArray<DataTable>();
        }

        //NZ 20151124 Agrega links a graficas multiserie (Matriciales) Se usa actualmente en Dashboar Ternium con FC
        /// <summary>
        /// Convierte cada una de las columnas de un DataTable en un DataTable nuevo y lo regresa en un DataTable[]. Se usa para crear las graficas multiserie de FusionCharts 
        /// </summary>
        /// <param name="dt">El DataTable con los datos a Graficar (Un reporte Matricial)</param>
        /// <param name="indexColumLabel">El indices en el que se encuentran los datos grafican en el eje X</param>
        /// <param name="indexColumIngore">Columnas del DataTable que se deben ignorar en la graficación</param>
        /// <param name="formatoLinks">El formato que se usarara el armado del Link. Entre llaves los datos que aplicaran igual a nivel renglon, datos que pueden ser obtenidos de una columna. 
        /// y en [] los que se sacaran de otra tabla para los datos en Y</param>
        /// <param name="indexColumInfoLinkX">Indices de columnas con informacion del link a nivel renglon</param>
        /// <param name="infoIndexLinkY">Tabla con una sola columna que es proporcional a los datos que datos que se tienen que sacar para el eje Y y sustituirlos en el Link. []</param>
        /// <returns></returns>
        public static DataTable[] ConvertDataTabletoDataTableArrayLink(DataTable dt, int indexColumLabel, int[] indexColumIngore, string formatoLinks, int[] indexColumInfoLinkX, DataTable infoIndexLinkY)
        {
            List<DataTable> ldta = new List<DataTable>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataTable ldt = new DataTable();
                DataView dv = new DataView(dt);
                ldt = dv.ToTable(false, new string[] { dt.Columns[i].ColumnName });
                if (i == indexColumLabel)
                {
                    ldt.Columns[0].ColumnName = "label";
                    ldta.Add(ldt);
                }
                else if (indexColumIngore == null || !indexColumIngore.Contains(i))
                {
                    ldt.Columns[0].ColumnName = "value";
                    ldt.Columns.Add("link");

                    string copiaformatoLinks = string.Empty;
                    for (int x = 0; x < ldt.Rows.Count; x++)
                    {
                        if (ldt.Rows[x][0] != DBNull.Value && Convert.ToDecimal(ldt.Rows[x][0]) > 0)
                        {
                            copiaformatoLinks = formatoLinks;
                            int contador = 0;
                            while (copiaformatoLinks.Contains("{" + contador.ToString() + "}"))
                            {
                                copiaformatoLinks = copiaformatoLinks.Replace("{" + contador.ToString() + "}", dt.Rows[x][indexColumInfoLinkX[contador]].ToString());
                                ldt.Rows[x][1] = copiaformatoLinks;
                                contador++;
                            }

                            //Agrega la concatenacion a cada valor del stack.
                            if (copiaformatoLinks.Contains("[0]"))
                            {
                                if (infoIndexLinkY != null)
                                {
                                    copiaformatoLinks = copiaformatoLinks.Replace("[0]", infoIndexLinkY.Rows[ldta.Count - 1][0].ToString());
                                    ldt.Rows[x][1] = copiaformatoLinks;
                                }
                            }
                        }
                        else { ldt.Rows[x][0] = DBNull.Value; }
                    }
                    ldta.Add(ldt);
                }
            }
            return ldta.ToArray<DataTable>();
        }

        public static string CreaContenedorGraficaYRadioButtonsGraf1Serie(string idContenedorGrafica, string CssStyle, string tipoGrafSeleccionada)
        {
            /***********************************************************************************
             Estos son algunos otros tipos de grafica:
             
             Column3D, Column2D, Line, Area2D, Bar2D, Pie2D, Pie3D, Doughnut2D, Doughnut3D, Pareto2D, Pareto3D, SSGrid, pie3d          
              
             ***********************************************************************************/
            StringBuilder lsb = new StringBuilder();
            lsb.Append("<div class=\"" + CssStyle + "\">\n ");
            lsb.Append("<div id=\"" + idContenedorGrafica + "\" style='margin-top:2px'></div>\n ");
            lsb.Append("<br/>\n ");
            lsb.Append("			<div>\n ");

            tipoGrafSeleccionada = tipoGrafSeleccionada.ToLower();

            if (tipoGrafSeleccionada == "line")
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='line'  checked='true' class=\"" + idContenedorGrafica + "\" /> Linea</label>\n ");
            }
            else
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='line'  class=\"" + idContenedorGrafica + "\" /> Linea</label>\n ");
            }
            if (tipoGrafSeleccionada == "column2d")
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='column2d'  checked='true' class=\"" + idContenedorGrafica + "\" /> Columnas</label>\n ");
            }
            else
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='column2d'  class=\"" + idContenedorGrafica + "\" /> Columnas</label>\n ");
            }
            if (tipoGrafSeleccionada == "bar2d")
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='bar2d'  checked='true' class=\"" + idContenedorGrafica + "\" /> Barras</label>\n ");
            }
            else
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='bar2d'  class=\"" + idContenedorGrafica + "\" /> Barras</label>\n ");
            }
            if (tipoGrafSeleccionada == "area2d")
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='area2d'  checked='true' class=\"" + idContenedorGrafica + "\" /> Area</label>\n ");
            }
            else
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='area2d'  class=\"" + idContenedorGrafica + "\" /> Area</label>\n ");
            }
            if (tipoGrafSeleccionada == "doughnut2d")
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='doughnut2d'  checked='true' class=\"" + idContenedorGrafica + "\" /> Anillo</label>\n ");
            }
            else
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='doughnut2d'  class=\"" + idContenedorGrafica + "\" /> Anillo</label>\n ");
            }
            if (tipoGrafSeleccionada == "doughnut3d")
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='doughnut3d'  checked='true' class=\"" + idContenedorGrafica + "\" /> Anillo 3D</label>\n ");
            }
            else
            {
                lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='doughnut3d'  class=\"" + idContenedorGrafica + "\" /> Anillo 3D</label>\n ");
            }
            lsb.Append("			</div>\n ");
            lsb.Append("<br/>\n ");
            lsb.Append("</div>\n ");
            return lsb.ToString();
        }

        //RJ.Método modificado para que tome las opciones sencillas y no las stacked.
        public static string CreaContenedorGraficaYRadioButtonsGrafMultiSerie(string idContenedorGrafica, string CssStyle, string tipoGrafSeleccionada)
        {
            /***********************************************************************************
             Estos son algunos otros tipos de grafica:
             
             MSColumn2D, MSColumn3D, MSLine, MSBar2D, MSBar3D, MSArea, Marimekko, ZoomLine, StackedColumn2D, StackedColumn3D,
             StackedBar2D, StackedBar3D, StackedArea2D
              
             ***********************************************************************************/

            string msLineChecked = string.Empty;
            string msColumn2DChecked = string.Empty;
            string msBar2DChecked = string.Empty;
            string msAreaChecked = string.Empty;

            StringBuilder lsb = new StringBuilder();
            lsb.Append("<div class=\"" + CssStyle + "\">\n ");
            lsb.Append("<div id=\"" + idContenedorGrafica + "\"></div>\n ");
            lsb.Append("<br/>\n ");
            lsb.Append("			<div>\n ");

            //tipoGrafSeleccionada = tipoGrafSeleccionada.ToLower();

            //Se verifica la opción seleccionada en el radiobutton
            //a la opción que se encuentre se le agrega el atributo "checked" al momento de crear el control
            switch (tipoGrafSeleccionada.ToLower())
            {
                case "msline":
                    msLineChecked = " checked='true' ";
                    break;
                case "mscolumn2d":
                    msColumn2DChecked = " checked='true' ";
                    break;
                case "msbar2d":
                    msBar2DChecked = " checked='true' ";
                    break;
                case "msarea":
                    msAreaChecked = " checked='true' ";
                    break;
                default:
                    break;
            }

            lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='msline'  " + msLineChecked + " class=\"" + idContenedorGrafica + "\" /> Linea</label>\n ");
            lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='mscolumn2d' " + msColumn2DChecked + " class=\"" + idContenedorGrafica + "\" /> Columnas</label>\n ");
            lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='msbar2d' " + msBar2DChecked + " class=\"" + idContenedorGrafica + "\" /> Barras</label>\n ");
            lsb.Append("				<label><input name=" + idContenedorGrafica + " type='radio' value='msarea' " + msAreaChecked + " class=\"" + idContenedorGrafica + "\"/> Area</label>\n ");



            lsb.Append("			</div>\n ");
            lsb.Append("<br/>\n ");
            lsb.Append("</div>\n ");
            return lsb.ToString();
        }


        /// <summary>
        /// Busca descripcion en catagos de cada elemento del array 
        /// </summary>
        /// <param name="iCodCatalogos">iCodCatalogos en formato string[]  Nota: Si en lugar de un catalogo se manda un string que no se puede convertir en numero se toma la cadena como descripcion.
        /// Ej. new string[] {"239475","235345","Cadena1"} regresaria :  " Descripcion de catalogo239475 / Descripcion de catalogo235345 / Cadena1 " </param>
        /// <returns>regresa un string con las descripciones de los catalogos separados por un "/" </returns>
        public static string AgregaEtiqueta(string[] iCodCatalogos)
        {
            try
            {
                if (ValidaAgregarNavegacion())
                {
                    List<string> listEtiquetaNavegacion = new List<string>();
                    string etiqueta = string.Empty;
                    int numero = 0;
                    foreach (string catalogo in iCodCatalogos)
                    {
                        if (int.TryParse(catalogo, out numero))
                        {
                            etiqueta = "";
                            etiqueta = Util.IsDBNull(DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacion(catalogo)), "").ToString();

                            //BG.20151103 Se cortan los parentesis que llegue a tener la variable Etiqueta.
                            etiqueta = etiqueta.Split(new char[] { '(' })[0].Trim();

                            if (etiqueta.Length > 0)
                            {
                                listEtiquetaNavegacion.Add(etiqueta);
                            }
                        }
                        else
                        {
                            listEtiquetaNavegacion.Add(catalogo);
                        }
                    }

                    return string.Join(" - ", listEtiquetaNavegacion.ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                        "Ocurrio un error en metodo AgregaEtiqueta "
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public static string AgregaEtiqueta(string iCodCatalogo)
        {
            try
            {
                if (ValidaAgregarNavegacion())
                {
                    int numero = 0;
                    string etiqueta = "";
                    if (int.TryParse(iCodCatalogo, out numero))
                    {
                        etiqueta = Util.IsDBNull(DSODataAccess.ExecuteScalar(ConsultaDescripcionDeNavegacion(iCodCatalogo)), "").ToString();
                        etiqueta = etiqueta.Split(new char[] { '(' })[0].Trim();
                    }
                    else { etiqueta = iCodCatalogo; }

                    return etiqueta;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                        "Ocurrio un error en metodo AgregaEtiqueta "
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public static string ConsultaDescripcionDeNavegacion(string iCodCatalogo)
        {
            //BG.20151103. Se deja como originalmente estaba, mas adelante se hace un split para quitar parentesis.
            StringBuilder lsb = new StringBuilder();
            lsb.Append("select vchDescripcion from " + DSODataContext.Schema + ".catalogos \n ");
            lsb.Append("where iCodRegistro = " + iCodCatalogo + "\n ");
            return lsb.ToString();


            ////BG.20150819. Se agrega el charindex para solo considerar la descripcion sin el parentesis.
            //StringBuilder lsb = new StringBuilder();
            //lsb.Append("select isnull(SUBSTRING(vchDescripcion, 0,charindex('(',vchDescripcion,1)),'') as vchDescripcion from " + DSODataContext.Schema + ".catalogos \n ");
            //lsb.Append("where iCodRegistro = " + iCodCatalogo + "\n ");
            //return lsb.ToString();
        }

        private static bool ValidaAgregarNavegacion()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("DECLARE @valorBandera INT = 0;");
            query.AppendLine("SELECT @valorBandera = ISNULL(Value, 0)");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Valores','Valores','Español')]");
            query.AppendLine("WHERE dtInivigencia <> dtFinVigencia AND dtFinVigencia >= getdate()");
            query.AppendLine("		AND AtribCod = 'BanderasCliente'");
            query.AppendLine("		AND vchCodigo = 'OcultarMapaDeNavegacion'");
            query.AppendLine("");
            query.AppendLine("SELECT TOP(1) CASE WHEN (ISNULL(BanderasCliente,0) & @valorBandera)/@valorBandera = 1 THEN 1 ELSE 0 END");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')]");
            query.AppendLine("WHERE dtInivigencia <> dtFinVigencia AND dtFinVigencia >= getdate()");
            query.AppendLine("		AND vchCodigo <> 'KeytiaC'");

            int valorBanderaOcultarNavegacion = (int)((object)(DSODataAccess.ExecuteScalar(query.ToString())));

            if (valorBanderaOcultarNavegacion == 1)//Si la bandera esta encendida, quiere decir que no debemos mostrar la navegación.
            {
                return false;
            }
            else { return true; }
        }

        public static string javaScriptDrillDown()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("");
            return lsb.ToString();
        }

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



        //NZ 20180622 Nueva Forma de trabajar las graficas
        public static string Grafica1Serie(string DataSourceJSon, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY, int pestañaActiva, FCGpoGraf tiposGraficas,
                                                   string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385")
        {
            var tipoDefault = DTIChartsAndControls.GetListaPestañasGenericas(tiposGraficas);

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\">");
            lsb.AppendLine("  FusionCharts.ready(function(){");
            lsb.AppendLine("        var FC_" + idContenedor + " = new FusionCharts({ ");
            lsb.AppendLine("        \"type\": \"" + tipoDefault.Keys.ElementAt(pestañaActiva) + "\", ");
            ////lsb.AppendLine("        \"renderAt\": \"" + idContenedor + "\", ");
            lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
            lsb.AppendLine("        \"height\": \"" + alto + "\", ");
            lsb.AppendLine("        \"dataFormat\": \"json\", ");
            lsb.AppendLine("        \"dataSource\":  { ");
            lsb.AppendLine("          \"chart\": { ");
            lsb.AppendLine("            \"caption\": \"" + titulo + "\", ");
            lsb.AppendLine("            \"subCaption\": \"" + segundoTitulo + "\", ");
            lsb.AppendLine("            \"setAdaptiveYMin\": \"1\", ");
            lsb.AppendLine("            \"xAxisName\": \"" + ejeX + "\", ");
            lsb.AppendLine("            \"yAxisName\": \"" + ejeY + "\", ");
            lsb.AppendLine("            \"formatNumberScale\": \"0\", ");
            lsb.AppendLine("            \"numberPrefix\": \"" + numberprefix + "\", ");
            lsb.AppendLine("            \"numberSuffix\": \"" + numberSuffix + "\",");
            lsb.AppendLine("            \"showlabels\": \"1\", ");
            lsb.AppendLine("            \"showvalues\": \"0\", ");
            lsb.AppendLine("            \"decimals\": \"2\", ");
            lsb.AppendLine("            \"decimalSeparator\": \".\", ");
            lsb.AppendLine("            \"thousandSeparator\": \",\", ");
            if (DSODataContext.Schema.ToLower() == "bat")
            {
                lsb.AppendLine("            \"theme\": \"" + "Line" + "\", ");
            }
            else
            {
                lsb.AppendLine("            \"theme\": \"" + nombreTemaGraf + "\", ");
            }

            lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
            lsb.AppendLine("            \"slantLabels\": \"1\", ");
            lsb.AppendLine("         }, ");
            lsb.AppendLine("         \"data\": " + DataSourceJSon);
            lsb.AppendLine("      } ");
            lsb.AppendLine("  }); ");
            lsb.AppendLine("");
            lsb.AppendLine("    radio = document.getElementsByClassName('" + idContenedor + "');");
            lsb.AppendLine("    for (i = 0; i < radio.length; i++) { ");
            lsb.AppendLine("        radElem = radio[i];");
            lsb.AppendLine("        if (radElem.localName === 'a') { ");
            lsb.AppendLine("            radElem.onclick = function(){ ");
            lsb.AppendLine("                val = this.getAttribute('attr');");
            lsb.AppendLine("                tipo = this.getAttribute('attrTipo');");
            lsb.AppendLine("                FC_" + idContenedor + ".chartType(tipo);");
            lsb.AppendLine("                FC_" + idContenedor + ".render(val, undefined, undefined); ");
            lsb.AppendLine("            };");
            lsb.AppendLine("        }");
            lsb.AppendLine("    }");
            lsb.AppendLine("");
            lsb.AppendLine("    if(radio.length > 0 && " + pestañaActiva + " < radio.length){");
            lsb.AppendLine("        radio[" + pestañaActiva + "].click();");
            lsb.AppendLine("        FC_" + idContenedor + ".render();");
            lsb.AppendLine("    }");
            lsb.AppendLine("});");
            lsb.AppendLine("</script> ");
            return lsb.ToString();
        }

        public static string Grafica1Serie(string DataSourceJSon, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY, string tipoGrafica,
                                                    string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385", bool agregarTagScript = true)
        {
            StringBuilder lsb = new StringBuilder();
            if (agregarTagScript)
            {
                lsb.AppendLine("<script type=\"text/javascript\">");
            }
            lsb.AppendLine("  FusionCharts.ready(function(){");
            lsb.AppendLine("        var " + idContenedor + " = new FusionCharts({ ");
            lsb.AppendLine("        \"type\": \"" + tipoGrafica + "\", ");
            lsb.AppendLine("        \"renderAt\": \"" + idContenedor + "\", ");
            lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
            lsb.AppendLine("        \"height\": \"" + alto + "\", ");
            lsb.AppendLine("        \"dataFormat\": \"json\", ");
            lsb.AppendLine("        \"dataSource\":  { ");
            lsb.AppendLine("          \"chart\": { ");
            lsb.AppendLine("            \"caption\": \"" + titulo + "\", ");
            lsb.AppendLine("            \"subCaption\": \"" + segundoTitulo + "\", ");
            lsb.AppendLine("            \"setAdaptiveYMin\": \"1\", ");
            lsb.AppendLine("            \"xAxisName\": \"" + ejeX + "\", ");
            lsb.AppendLine("            \"yAxisName\": \"" + ejeY + "\", ");
            lsb.AppendLine("            \"formatNumberScale\": \"0\", ");
            lsb.AppendLine("            \"numberPrefix\": \"" + numberprefix + "\", ");
            lsb.AppendLine("            \"numberSuffix\": \"" + numberSuffix + "\",");
            lsb.AppendLine("            \"showlabels\": \"1\", ");
            lsb.AppendLine("            \"showvalues\": \"0\", ");
            lsb.AppendLine("            \"decimals\": \"2\", ");
            lsb.AppendLine("            \"decimalSeparator\": \".\", ");
            lsb.AppendLine("            \"thousandSeparator\": \",\", ");
            lsb.AppendLine("            \"theme\": \"" + nombreTemaGraf + "\", ");
            lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
            lsb.AppendLine("            \"slantLabels\": \"1\", ");
            lsb.AppendLine("         }, ");
            lsb.AppendLine("         \"data\": " + DataSourceJSon);
            lsb.AppendLine("      } ");
            lsb.AppendLine("  }); ");
            lsb.AppendLine("");
            lsb.AppendLine(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte.\"); ");
            lsb.AppendLine(idContenedor + ".render();");
            lsb.AppendLine("});");
            if (agregarTagScript)
            {
                lsb.AppendLine("</script> ");
            }
            return lsb.ToString();
        }

        public static string GraficaMultiSeries(string[] DataSourceJSon, string[] nombreSeries, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY, int pestañaActiva, FCGpoGraf tiposGraficas,
                                                   string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385", bool showValues = false, bool showPercentage = false)
        {

            var tipoDefault = DTIChartsAndControls.GetListaPestañasGenericas(tiposGraficas);

            StringBuilder lsb = new StringBuilder();
            if (tiposGraficas == FCGpoGraf.MatricialConStackLineaBase)
            {
                List<string> DataSeries = new List<string>();

                StringBuilder sbSerie = new StringBuilder();
                for (int i = 1; i < DataSourceJSon.Length - 1; i++)
                {
                    sbSerie.Length = 0;
                    sbSerie.Append("\"seriesname\": \"");
                    sbSerie.Append(nombreSeries[i]);
                    sbSerie.Append("\",\"data\":");
                    sbSerie.Append(DataSourceJSon[i]);
                    DataSeries.Add(sbSerie.ToString());
                }


                List<string> DataLineSet = new List<string>();

                StringBuilder sbLineSet = new StringBuilder();

                for (int i = DataSourceJSon.Length - 1; i < DataSourceJSon.Length; i++)
                {
                    sbLineSet.Length = 0;
                    sbLineSet.Append("\"seriesname\": \"");
                    sbLineSet.Append(nombreSeries[i]);
                    sbLineSet.Append("\",\"data\":");
                    sbLineSet.Append(DataSourceJSon[i]);
                    DataLineSet.Add(sbLineSet.ToString());
                }
                lsb.AppendLine("<script type=\"text/javascript\"> ");
                lsb.AppendLine("FusionCharts.ready(function(){										    ");
                lsb.AppendLine("       var  " + idContenedor + " = new FusionCharts({                   ");
                lsb.AppendLine("               \"type\": \"" + tipoDefault.Keys.ElementAt(pestañaActiva) + "\",                       ");
                lsb.AppendLine("               \"renderAt\": \"" + idContenedor + "\",                  ");
                lsb.AppendLine("               \"width\": \"" + ancho + "\",                            ");
                lsb.AppendLine("               \"height\": \"" + alto + "\",                            ");
                lsb.AppendLine("               \"dataFormat\": \"json\",                                ");
                lsb.AppendLine("               \"dataSource\": {                                        ");
                lsb.AppendLine("                   \"chart\":  {                                        ");
                lsb.AppendLine("                       \"caption\": \"" + titulo + "\",                 ");
                lsb.AppendLine("                       \"subcaption\": \"" + segundoTitulo + "\",       ");
                lsb.AppendLine("                       \"setAdaptiveYMin\": \"1\", ");
                lsb.AppendLine("                       \"pYAxisName\": \"" + ejeY + "\",                ");
                lsb.AppendLine("                       \"sYAxisName\": \"Presupuesto\",                 ");
                lsb.AppendLine("                       \"NumberPrefix\":\"" + numberprefix + "\",           ");
                lsb.AppendLine("                       \"sNumberPrefix\": \"" + numberprefix + "\",         ");
                lsb.AppendLine("                       \"NumberSuffix\":\"" + numberSuffix + "\",           ");
                lsb.AppendLine("                       \"sNumberSuffix\": \"" + numberSuffix + "\",         ");
                lsb.AppendLine("                        \"formatNumberScale\":\"0\",                    ");
                if (showValues)
                {
                    lsb.Append("            \"showsum\": \"1\",\n ");
                }
                if (showPercentage)
                {
                    //lsb.Append("            \"showvalues\": \"1\",\n ");
                    //lsb.Append("            \"showPercentValues\": \"1\",\n ");
                    lsb.Append("            \"showPercentInTooltip\": \"1\",\n ");
                }
                lsb.Append("            \"decimals\": \"1\",\n ");
                lsb.AppendLine("                       \"theme\": \"" + nombreTemaGraf + "\",           ");

                lsb.AppendLine("                       },                                               ");
                lsb.AppendLine("                   \"categories\": [  {                                 ");
                lsb.AppendLine("                           \"category\": " + DataSourceJSon[0] + "}   ],");
                lsb.AppendLine("                   \"dataset\": [  {                                    ");
                lsb.AppendLine("                       \"dataset\":  [  {                               ");
                lsb.AppendLine(String.Join("},{", DataSeries.ToArray()));
                lsb.AppendLine("                            }  ],                                       ");
                lsb.AppendLine("                       }  ],                                            ");
                lsb.AppendLine("                       \"lineset\": [  {                                ");
                lsb.AppendLine(String.Join("},{", DataLineSet.ToArray()));
                lsb.AppendLine("                                    } ] }                               ");
                lsb.AppendLine("                   });                                                  ");
                lsb.AppendLine(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte para el rango de fechas seleccionadas.\"); ");
                lsb.AppendLine(idContenedor + ".render();                                               ");
                lsb.AppendLine("   });                                                                  ");
                lsb.AppendLine("</script> \r");
            }
            else
            {

                List<string> DataSeries = new List<string>();

                StringBuilder sbSerie = new StringBuilder();
                for (int i = 1; i < DataSourceJSon.Length; i++)
                {
                    sbSerie.Length = 0;
                    sbSerie.Append("\"seriesname\": \"");
                    sbSerie.Append(nombreSeries[i]);
                    sbSerie.Append("\",\"data\":");
                    sbSerie.Append(DataSourceJSon[i]);
                    DataSeries.Add(sbSerie.ToString());
                }

                lsb.AppendLine("<script type=\"text/javascript\"> ");
                lsb.AppendLine("FusionCharts.ready(function () { ");
                lsb.AppendLine("    var radio = [], radElem, val, FC_" + idContenedor + " = new FusionCharts({ ");
                lsb.AppendLine("        \"type\": \"" + tipoDefault.Keys.ElementAt(pestañaActiva) + "\", ");
                //lsb.AppendLine("        \"renderAt\": \"" + idContenedor + "\", ");
                lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
                lsb.AppendLine("        \"height\": \"" + alto + "\", ");
                lsb.AppendLine("        \"dataFormat\": \"json\", ");
                lsb.AppendLine("        \"dataSource\": { ");
                lsb.AppendLine("           \"chart\": { ");
                lsb.AppendLine("              \"caption\": \"" + titulo + "\", ");
                lsb.AppendLine("              \"subCaption\": \"" + segundoTitulo + "\", ");
                lsb.AppendLine("              \"xAxisname\": \"" + ejeX + "\", ");
                lsb.AppendLine("              \"yAxisName\": \"" + ejeY + "\", ");
                lsb.AppendLine("              \"setAdaptiveYMin\": \"1\", ");
                lsb.AppendLine("              \"formatNumberScale\": \"0\", ");
                lsb.AppendLine("              \"numberPrefix\": \"" + numberprefix + "\", ");
                lsb.AppendLine("              \"numberSuffix\": \"" + numberSuffix + "\",");
                lsb.AppendLine("              \"showlabels\": \"1\",");
                lsb.AppendLine("              \"showvalues\": \"0\",");
                if (showValues)
                {
                    lsb.Append("            \"showsum\": \"1\",\n ");
                }
                if (showPercentage)
                {
                    //lsb.Append("            \"showvalues\": \"1\",\n ");
                    //lsb.Append("            \"showPercentValues\": \"1\",\n ");
                    lsb.Append("            \"showPercentInTooltip\": \"1\",\n ");
                }
                lsb.AppendLine("              \"decimals\": \"2\", ");
                lsb.AppendLine("              \"decimalSeparator\": \".\", ");
                lsb.AppendLine("              \"thousandSeparator\": \",\", ");
                lsb.AppendLine("              \"theme\": \"" + nombreTemaGraf + "\", ");
                lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
                lsb.AppendLine("            \"slantLabels\": \"1\", ");
                lsb.AppendLine("           }, ");
                if(DataSourceJSon.Length > 0)
                {
                    lsb.AppendLine("           \"categories\": [ ");
                    lsb.AppendLine("              { ");
                    lsb.AppendLine("                 \"category\": " + DataSourceJSon[0] + " ");
                    lsb.AppendLine("              } ");
                    lsb.AppendLine("           ], ");
                }
                
                lsb.AppendLine("           \"dataset\": [ ");
                lsb.AppendLine("              { ");
                lsb.AppendLine(String.Join("},{", DataSeries.ToArray()));
                lsb.AppendLine("              } ");
                lsb.AppendLine("           ] ");
                lsb.AppendLine("        } ");
                lsb.AppendLine("    }); ");
                lsb.AppendLine("");
                lsb.AppendLine("    radio = document.getElementsByClassName('" + idContenedor + "');");
                lsb.AppendLine("    for (i = 0; i < radio.length; i++) { ");
                lsb.AppendLine("        radElem = radio[i];");
                lsb.AppendLine("        if (radElem.localName === 'a') { ");
                lsb.AppendLine("            radElem.onclick = function(){ ");
                lsb.AppendLine("                val = this.getAttribute('attr');");
                lsb.AppendLine("                tipo = this.getAttribute('attrTipo');");
                lsb.AppendLine("                FC_" + idContenedor + ".chartType(tipo);");
                lsb.AppendLine("                FC_" + idContenedor + ".render(val, undefined, undefined); ");
                lsb.AppendLine("            };");
                lsb.AppendLine("        }");
                lsb.AppendLine("    }");
                lsb.AppendLine("");
                lsb.AppendLine("    if(radio.length > 0 && " + pestañaActiva + " < radio.length){");
                lsb.AppendLine("        radio[" + pestañaActiva + "].click();");
                lsb.AppendLine("        FC_" + idContenedor + ".render();");
                lsb.AppendLine("    }");
                lsb.AppendLine("});");
                lsb.AppendLine("</script> ");
            }



            return lsb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSourceJSon"></param>
        /// <param name="nombreSeries"></param>
        /// <param name="idContenedor"></param>
        /// <param name="titulo"></param>
        /// <param name="segundoTitulo"></param>
        /// <param name="ejeX"></param>
        /// <param name="ejeY"></param>
        /// <param name="pestañaActiva"></param>
        /// <param name="tiposGraficas"></param>
        /// <param name="yMaxVal"></param>Valor maximo que se le asigna a la  grafica
        /// <param name="ejeSY"></param>
        /// <param name="numberprefix"></param>
        /// <param name="numberSuffix"></param>
        /// <param name="nombreTemaGraf"></param>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <returns></returns>
        public static string GraficaMultiSeries(string[] DataSourceJSon, string[] nombreSeries, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY, int pestañaActiva, FCGpoGraf tiposGraficas, int yMaxVal, string ejeSY,
                                                string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385")
        {

            var tipoDefault = DTIChartsAndControls.GetListaPestañasGenericas(tiposGraficas);

            StringBuilder lsb = new StringBuilder();
            if (tiposGraficas == FCGpoGraf.MatricialConStackLineaBase)
            {
                List<string> DataSeries = new List<string>();

                StringBuilder sbSerie = new StringBuilder();
                for (int i = 1; i < DataSourceJSon.Length - 1; i++)
                {
                    sbSerie.Length = 0;
                    sbSerie.Append("\"seriesname\": \"");
                    sbSerie.Append(nombreSeries[i]);
                    sbSerie.Append("\",\"data\":");
                    sbSerie.Append(DataSourceJSon[i]);
                    DataSeries.Add(sbSerie.ToString());
                }


                List<string> DataLineSet = new List<string>();

                StringBuilder sbLineSet = new StringBuilder();
                //
                for (int i = DataSourceJSon.Length - 1; i < DataSourceJSon.Length; i++)
                {
                    sbLineSet.Length = 0;
                    sbLineSet.Append("\"seriesname\": \"");
                    sbLineSet.Append(nombreSeries[i]);
                    sbLineSet.Append("\",\"data\":");
                    sbLineSet.Append(DataSourceJSon[i]);
                    DataLineSet.Add(sbLineSet.ToString());
                }
                lsb.AppendLine("<script type=\"text/javascript\"> ");
                lsb.AppendLine("FusionCharts.ready(function(){										    ");
                lsb.AppendLine("       var  " + idContenedor + " = new FusionCharts({                   ");
                lsb.AppendLine("               \"type\": \"" + tipoDefault.Keys.ElementAt(pestañaActiva) + "\",                       ");
                lsb.AppendLine("               \"renderAt\": \"" + idContenedor + "\",                  ");
                lsb.AppendLine("               \"width\": \"" + ancho + "\",                            ");
                lsb.AppendLine("               \"height\": \"" + alto + "\",                            ");
                lsb.AppendLine("               \"dataFormat\": \"json\",                                ");
                lsb.AppendLine("               \"dataSource\": {                                        ");
                lsb.AppendLine("                   \"chart\":  {                                        ");
                lsb.AppendLine("                       \"caption\": \"" + titulo + "\",                 ");
                lsb.AppendLine("                       \"subcaption\": \"" + segundoTitulo + "\",       ");
                lsb.AppendLine("                       \"setAdaptiveYMin\": \"1\", ");
                lsb.AppendLine("                       \"pYAxisName\": \"" + ejeY + "\",                ");
                lsb.AppendLine("                       \"sYAxisName\": \"Presupuesto\",                 ");
                lsb.AppendLine("                       \"NumberPrefix\":\"" + numberprefix + "\",           ");
                lsb.AppendLine("                       \"sNumberPrefix\": \"" + numberprefix + "\",         ");
                lsb.AppendLine("                       \"NumberSuffix\":\"" + numberSuffix + "\",           ");
                lsb.AppendLine("                       \"sNumberSuffix\": \"" + numberSuffix + "\",         ");
                lsb.AppendLine("                        \"formatNumberScale\":\"0\",                    ");
                if (yMaxVal > 0)
                {
                    lsb.AppendLine("                       \"PYAxisMaxValue\" :\"" + yMaxVal + "\",     ");
                    lsb.AppendLine("                       \"PYAxisMinValue\": \"0\",                   ");
                    lsb.AppendLine("                       \"SYAxisMaxValue\": \"" + yMaxVal + "\",     ");
                    lsb.AppendLine("                       \"SYAxisMinValue\": \"0\",                   ");
                }
                lsb.AppendLine("                       \"theme\": \"" + nombreTemaGraf + "\",           ");

                lsb.AppendLine("                       },                                               ");
                lsb.AppendLine("                   \"categories\": [  {                                 ");
                lsb.AppendLine("                           \"category\": " + DataSourceJSon[0] + "}   ],");
                lsb.AppendLine("                   \"dataset\": [  {                                    ");
                lsb.AppendLine("                       \"dataset\":  [  {                               ");
                lsb.AppendLine(String.Join("},{", DataSeries.ToArray()));
                lsb.AppendLine("                            }  ],                                       ");
                lsb.AppendLine("                       }  ],                                            ");
                lsb.AppendLine("                       \"lineset\": [  {                                ");
                lsb.AppendLine(String.Join("},{", DataLineSet.ToArray()));
                lsb.AppendLine("                                    } ] }                               ");
                lsb.AppendLine("                   });                                                  ");
                lsb.AppendLine(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte para el rango de fechas seleccionadas.\"); ");
                lsb.AppendLine(idContenedor + ".render();                                               ");
                lsb.AppendLine("   });                                                                  ");
                lsb.AppendLine("</script> \r");
            }
            else
            {

                List<string> DataSeries = new List<string>();

                StringBuilder sbSerie = new StringBuilder();
                for (int i = 1; i < DataSourceJSon.Length; i++)
                {
                    sbSerie.Length = 0;
                    sbSerie.Append("\"seriesname\": \"");
                    sbSerie.Append(nombreSeries[i]);
                    sbSerie.Append("\",\"data\":");
                    sbSerie.Append(DataSourceJSon[i]);
                    DataSeries.Add(sbSerie.ToString());
                }

                lsb.AppendLine("<script type=\"text/javascript\"> ");
                lsb.AppendLine("FusionCharts.ready(function () { ");
                lsb.AppendLine("    var radio = [], radElem, val, FC_" + idContenedor + " = new FusionCharts({ ");
                lsb.AppendLine("        \"type\": \"" + tipoDefault.Keys.ElementAt(pestañaActiva) + "\", ");
                //lsb.AppendLine("        \"renderAt\": \"" + idContenedor + "\", ");
                lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
                lsb.AppendLine("        \"height\": \"" + alto + "\", ");
                lsb.AppendLine("        \"dataFormat\": \"json\", ");
                lsb.AppendLine("        \"dataSource\": { ");
                lsb.AppendLine("           \"chart\": { ");
                lsb.AppendLine("              \"caption\": \"" + titulo + "\", ");
                lsb.AppendLine("              \"subCaption\": \"" + segundoTitulo + "\", ");
                lsb.AppendLine("              \"xAxisname\": \"" + ejeX + "\", ");
                lsb.AppendLine("              \"yAxisName\": \"" + ejeY + "\", ");
                lsb.AppendLine("              \"formatNumberScale\": \"0\", ");
                lsb.AppendLine("              \"numberPrefix\": \"" + numberprefix + "\", ");
                lsb.AppendLine("              \"numberSuffix\": \"" + numberSuffix + "\",");
                lsb.AppendLine("              \"showlabels\": \"1\",");
                lsb.AppendLine("              \"showvalues\": \"0\",");
                lsb.AppendLine("              \"decimals\": \"2\", ");
                lsb.AppendLine("              \"decimalSeparator\": \".\", ");
                lsb.AppendLine("              \"thousandSeparator\": \",\", ");
                lsb.AppendLine("              \"theme\": \"" + nombreTemaGraf + "\", ");
                lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
                lsb.AppendLine("            \"slantLabels\": \"1\", ");
                lsb.AppendLine("           }, ");
                lsb.AppendLine("           \"categories\": [ ");
                lsb.AppendLine("              { ");
                lsb.AppendLine("                 \"category\": " + DataSourceJSon[0] + " ");
                lsb.AppendLine("              } ");
                lsb.AppendLine("           ], ");
                lsb.AppendLine("           \"dataset\": [ ");
                lsb.AppendLine("              { ");
                lsb.AppendLine(String.Join("},{", DataSeries.ToArray()));
                lsb.AppendLine("              } ");
                lsb.AppendLine("           ] ");
                lsb.AppendLine("        } ");
                lsb.AppendLine("    }); ");
                lsb.AppendLine("");
                lsb.AppendLine("    radio = document.getElementsByClassName('" + idContenedor + "');");
                lsb.AppendLine("    for (i = 0; i < radio.length; i++) { ");
                lsb.AppendLine("        radElem = radio[i];");
                lsb.AppendLine("        if (radElem.localName === 'a') { ");
                lsb.AppendLine("            radElem.onclick = function(){ ");
                lsb.AppendLine("                val = this.getAttribute('attr');");
                lsb.AppendLine("                tipo = this.getAttribute('attrTipo');");
                lsb.AppendLine("                FC_" + idContenedor + ".chartType(tipo);");
                lsb.AppendLine("                FC_" + idContenedor + ".render(val, undefined, undefined); ");
                lsb.AppendLine("            };");
                lsb.AppendLine("        }");
                lsb.AppendLine("    }");
                lsb.AppendLine("");
                lsb.AppendLine("    if(radio.length > 0 && " + pestañaActiva + " < radio.length){");
                lsb.AppendLine("        radio[" + pestañaActiva + "].click();");
                lsb.AppendLine("        FC_" + idContenedor + ".render();");
                lsb.AppendLine("    }");
                lsb.AppendLine("});");
                lsb.AppendLine("</script> ");
            }



            return lsb.ToString();
        }

        public static string GraficaMultiSeries(string[] DataSourceJSon, string[] nombreSeries, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY, string tipoGrafica,
                                                   string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385", bool agregarTagScript = true)
        {
            List<string> DataSeries = new List<string>();

            StringBuilder sbSerie = new StringBuilder();
            for (int i = 1; i < DataSourceJSon.Length; i++)
            {
                sbSerie.Length = 0;
                sbSerie.Append("\"seriesname\": \"");
                sbSerie.Append(nombreSeries[i]);
                sbSerie.Append("\",\"data\":");
                sbSerie.Append(DataSourceJSon[i]);
                DataSeries.Add(sbSerie.ToString());
            }

            StringBuilder lsb = new StringBuilder();
            if (agregarTagScript)
            {
                lsb.AppendLine("<script type=\"text/javascript\"> ");
            }
            lsb.AppendLine("FusionCharts.ready(function () { ");
            lsb.AppendLine("    var " + idContenedor + " = new FusionCharts({ ");
            lsb.AppendLine("        \"type\": \"" + tipoGrafica + "\", ");
            lsb.AppendLine("        \"renderAt\": \"" + idContenedor + "\", ");
            lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
            lsb.AppendLine("        \"height\": \"" + alto + "\", ");
            lsb.AppendLine("        \"dataFormat\": \"json\", ");
            lsb.AppendLine("        \"dataSource\": { ");
            lsb.AppendLine("           \"chart\": { ");
            lsb.AppendLine("              \"caption\": \"" + titulo + "\", ");
            lsb.AppendLine("              \"subCaption\": \"" + segundoTitulo + "\", ");
            lsb.AppendLine("              \"setAdaptiveYMin\": \"1\", ");
            lsb.AppendLine("              \"xAxisname\": \"" + ejeX + "\", ");
            lsb.AppendLine("              \"yAxisName\": \"" + ejeY + "\", ");
            lsb.AppendLine("              \"formatNumberScale\": \"0\", ");
            lsb.AppendLine("              \"numberPrefix\": \"" + numberprefix + "\", ");
            lsb.AppendLine("              \"numberSuffix\": \"" + numberSuffix + "\",");
            lsb.AppendLine("              \"showlabels\": \"1\",");
            lsb.AppendLine("              \"showvalues\": \"0\", ");
            lsb.AppendLine("              \"decimals\": \"2\", ");
            lsb.AppendLine("              \"decimalSeparator\": \".\", ");
            lsb.AppendLine("              \"thousandSeparator\": \",\", ");
            lsb.AppendLine("              \"theme\": \"" + nombreTemaGraf + "\", ");
            lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
            lsb.AppendLine("            \"slantLabels\": \"1\", ");
            lsb.AppendLine("           }, ");
            lsb.AppendLine("           \"categories\": [ ");
            lsb.AppendLine("              { ");
            lsb.AppendLine("                 \"category\": " + DataSourceJSon[0] + " ");
            lsb.AppendLine("              } ");
            lsb.AppendLine("           ], ");
            lsb.AppendLine("           \"dataset\": [ ");
            lsb.AppendLine("              { ");
            lsb.AppendLine(String.Join("},{", DataSeries.ToArray()));
            lsb.AppendLine("              } ");
            lsb.AppendLine("           ] ");
            lsb.AppendLine("        } ");
            lsb.AppendLine("    }); ");
            lsb.AppendLine("");
            lsb.AppendLine(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte.\"); ");
            lsb.AppendLine(idContenedor + ".render();");
            lsb.AppendLine("});");
            if (agregarTagScript)
            {
                lsb.AppendLine("</script> ");
            }

            return lsb.ToString();
        }

        public static string ConvertToJSONStringDetalle(DataTable dt)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DTIChartsAndControls.agregaTotales(dt, 0, "Totales");

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                return "{\"data\":" + serializer.Serialize(rows) + "}";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gráfica con dos escalas en el eje de las Y
        /// el ejemplo lo tomé de esta página: http://jsfiddle.net/fusioncharts/pfsytcca/
        /// </summary>
        /// <returns></returns>
        public static string GraficaCombinada(string[] DataSourceJSon, string[] columnNames, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeYPrinc, string ejeYSec, int pestañaActiva, FCTipoEjeSecundario tipoEjeSec, FCGpoGraf tiposGraficas,
                                           string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385")
        {
            var tipoDefault = DTIChartsAndControls.GetListaPestañasGenericas(tiposGraficas);

            List<string> DataSeries = new List<string>();
            StringBuilder sbSerie = new StringBuilder();

            for (int i = 1; i < DataSourceJSon.Length; i++)
            {
                sbSerie.Length = 0;
                sbSerie.Append("\"seriesname\": \"");
                sbSerie.Append(columnNames[i]);

                if (i == 1)
                    sbSerie.Append("\",\"data\":");
                else
                {
                    sbSerie.Append("\",\"parentYAxis\": \"S\",\"renderas\": \"" + tipoEjeSec + "\",\"data\":");
                }
                sbSerie.Append(DataSourceJSon[i]);
                DataSeries.Add(sbSerie.ToString());
            }

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\">");
            lsb.AppendLine("  FusionCharts.ready(function(){");
            lsb.AppendLine("        var FC_" + idContenedor + " = new FusionCharts({ ");
            lsb.AppendLine("        \"type\": \"mscombidy2d\", ");
            lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
            lsb.AppendLine("        \"height\": \"" + alto + "\", ");
            lsb.AppendLine("        \"dataFormat\": \"json\", ");
            lsb.AppendLine("        \"dataSource\":  { ");
            lsb.AppendLine("          \"chart\": { ");
            lsb.AppendLine("            \"caption\": \"" + titulo + "\", ");
            lsb.AppendLine("            \"subCaption\": \"" + segundoTitulo + "\", ");
            lsb.AppendLine("            \"setAdaptiveYMin\": \"1\", ");
            lsb.AppendLine("            \"xAxisName\": \"" + ejeX + "\", ");
            lsb.AppendLine("            \"pYAxisName\": \"" + ejeYPrinc + "\", ");
            lsb.AppendLine("            \"sYAxisName\": \"" + ejeYSec + "\", ");
            lsb.AppendLine("            \"formatNumberScale\": \"0\", ");

            lsb.AppendLine("            \"numberPrefix\": \"" + numberprefix + "\", ");
            lsb.AppendLine("            \"sNumberSuffix\": \"" + numberSuffix + "\",");
            //lsb.AppendLine("            \"sYAxisMaxValue\": \"3000\",");

            lsb.AppendLine("            \"showlabels\": \"1\", ");
            lsb.AppendLine("            \"showvalues\": \"0\", ");
            lsb.AppendLine("            \"decimals\": \"2\", ");
            lsb.AppendLine("            \"decimalSeparator\": \".\", ");
            lsb.AppendLine("            \"thousandSeparator\": \",\", ");
            lsb.AppendLine("            \"theme\": \"" + nombreTemaGraf + "\", ");
            lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
            lsb.AppendLine("            \"slantLabels\": \"1\", ");
            lsb.AppendLine("         }, ");


            lsb.AppendLine("           \"categories\": [ ");
            lsb.AppendLine("              { ");
            lsb.AppendLine("                 \"category\": " + DataSourceJSon[0] + " ");
            lsb.AppendLine("              } ");
            lsb.AppendLine("           ], ");


            lsb.AppendLine("           \"dataset\": [ ");
            lsb.AppendLine("              { ");
            lsb.AppendLine(String.Join("},{", DataSeries.ToArray()));
            lsb.AppendLine("              } ");
            lsb.AppendLine("           ] ");


            lsb.AppendLine("      } ");
            lsb.AppendLine("  }); ");
            lsb.AppendLine("");
            lsb.AppendLine("    radio = document.getElementsByClassName('" + idContenedor + "');");
            lsb.AppendLine("    for (i = 0; i < radio.length; i++) { ");
            lsb.AppendLine("        radElem = radio[i];");
            lsb.AppendLine("        if (radElem.localName === 'a') { ");
            lsb.AppendLine("            radElem.onclick = function(){ ");
            lsb.AppendLine("                val = this.getAttribute('attr');");
            lsb.AppendLine("                tipo = this.getAttribute('attrTipo');");
            lsb.AppendLine("                FC_" + idContenedor + ".chartType(tipo);");
            lsb.AppendLine("                FC_" + idContenedor + ".render(val, undefined, undefined); ");
            lsb.AppendLine("            };");
            lsb.AppendLine("        }");
            lsb.AppendLine("    }");
            lsb.AppendLine("");
            lsb.AppendLine("    if(radio.length > 0 && " + pestañaActiva + " < radio.length){");
            lsb.AppendLine("        radio[" + pestañaActiva + "].click();");
            lsb.AppendLine("        FC_" + idContenedor + ".render();");
            lsb.AppendLine("    }");
            lsb.AppendLine("});");
            lsb.AppendLine("</script> ");
            return lsb.ToString();
        }

        public static string GraficaCombinadaSencilla(string[] DataSourceJSon, string[] columnNames, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeYPrinc, string ejeYSec,FCTipoEjeSecundario tipoEjeSec,
                                          string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385")
        {

            List<string> DataSeries = new List<string>();
            StringBuilder sbSerie = new StringBuilder();

            for (int i = 1; i < DataSourceJSon.Length; i++)
            {
                sbSerie.Length = 0;
                sbSerie.Append("\"seriesname\": \"");
                sbSerie.Append(columnNames[i]);

                if (i == 1)
                    sbSerie.Append("\",\"data\":");
                else
                {
                    sbSerie.Append("\",\"parentYAxis\": \"S\",\"renderas\": \"" + tipoEjeSec + "\",\"data\":");
                }
                sbSerie.Append(DataSourceJSon[i]);
                DataSeries.Add(sbSerie.ToString());
            }

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\">");
            lsb.AppendLine("  FusionCharts.ready(function(){");
            lsb.AppendLine("        var " + idContenedor + " = new FusionCharts({ ");
            lsb.AppendLine("        \"type\": \"mscombidy2d\", ");
            lsb.AppendLine("        \"renderAt\": \"" + idContenedor + "\", ");
            lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
            lsb.AppendLine("        \"height\": \"" + alto + "\", ");
            lsb.AppendLine("        \"dataFormat\": \"json\", ");
            lsb.AppendLine("        \"dataSource\":  { ");
            lsb.AppendLine("          \"chart\": { ");
            lsb.AppendLine("            \"caption\": \"" + titulo + "\", ");
            lsb.AppendLine("            \"subCaption\": \"" + segundoTitulo + "\", ");
            lsb.AppendLine("            \"setAdaptiveYMin\": \"1\", ");
            lsb.AppendLine("            \"xAxisName\": \"" + ejeX + "\", ");
            lsb.AppendLine("            \"pYAxisName\": \"" + ejeYPrinc + "\", ");
            lsb.AppendLine("            \"sYAxisName\": \"" + ejeYSec + "\", ");
            lsb.AppendLine("            \"formatNumberScale\": \"0\", ");

            lsb.AppendLine("            \"numberPrefix\": \"" + numberprefix + "\", ");
            lsb.AppendLine("            \"sNumberSuffix\": \"" + numberSuffix + "\",");
            //lsb.AppendLine("            \"sYAxisMaxValue\": \"3000\",");

            lsb.AppendLine("            \"showlabels\": \"1\", ");
            lsb.AppendLine("            \"showvalues\": \"0\", ");
            lsb.AppendLine("            \"decimals\": \"2\", ");
            lsb.AppendLine("            \"decimalSeparator\": \".\", ");
            lsb.AppendLine("            \"thousandSeparator\": \",\", ");
            lsb.AppendLine("            \"theme\": \"" + nombreTemaGraf + "\", ");
            lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
            lsb.AppendLine("            \"slantLabels\": \"1\", ");
            lsb.AppendLine("         }, ");


            lsb.AppendLine("           \"categories\": [ ");
            lsb.AppendLine("              { ");
            lsb.AppendLine("                 \"category\": " + DataSourceJSon[0] + " ");
            lsb.AppendLine("              } ");
            lsb.AppendLine("           ], ");


            lsb.AppendLine("           \"dataset\": [ ");
            lsb.AppendLine("              { ");
            lsb.AppendLine(String.Join("},{", DataSeries.ToArray()));
            lsb.AppendLine("              } ");
            lsb.AppendLine("           ] ");


            lsb.AppendLine("      } ");
            lsb.AppendLine("  }); ");
            lsb.AppendLine("");
            lsb.AppendLine(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte.\"); ");
            lsb.AppendLine(idContenedor + ".render();");
            lsb.AppendLine("});");
            lsb.AppendLine("</script> ");
            return lsb.ToString();
        }
    }
}
