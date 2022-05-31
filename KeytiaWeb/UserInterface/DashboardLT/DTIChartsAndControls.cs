using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;

namespace KeytiaWeb.UserInterface.DashboardLT
{
    public enum FCGpoGraf
    {
        Tabular,
        TabularLiTa,
        TabularBaTa,
        TabularLiBaCoTa,
        TabularBaCoDoTa,
        Matricial,
        MatricialConStack,
        MatricialConStack2,
        MatricialConStackLineaBase,
        MatricialLiCoBaTa,
        MatricialConStackCoBaDoTa,
        TabularCoBaTa,
        MatricialCoBaTa,
        TabularCiBaTa,
        MatricialTa,
        ColsCombinada,
        Doughnut2d
    }

    public enum FCTipoEjeSecundario
    {
        line,
        area
    }

    public class DTIChartsAndControls
    {
        public static string NombreClase
        {
            get { return "DTIChartsAndControls.cs"; }
        }

        #region Metodos para crear graficas, datatable y bordes de reportes estilo keytia

        /// <summary>
        /// Regresa una grafica de linea, pasandole como datasource una consulta con 3 columnas
        /// </summary>
        /// <param name="dataSource">DataTable con 3 columnas</param>
        /// <param name="seriesAgrupadasPor">Nombre de la columna por la cual se va agrupar</param>
        /// <param name="ejeX">Nombre de la columna que representara el ejeX</param>
        /// <param name="ejeY">Nombre de la columna que representara el ejeY</param>
        /// <param name="navegacion">Url que se asignara a cada serie. Nota: Si la propiedad lenght de este string es "0" no se agragaran Url a las series</param>
        /// <param name="alturaDeGraf">Altura de la grafica en pixeles</param>
        /// <param name="anchoDeGraf">Ancho de la grafica en pixeles</param>
        /// <param name="tituloGrafSuperior">Titulo superior de la grafica</param>
        /// <param name="tituloGrafEjeX">Titulo en ejeX de la grafica</param>
        /// <param name="tituloGrafEjeY">Titulo en ejeY de la grafica</param>
        /// <param name="axiYLabelStyleFormat">Formato de valores en eje Y Ej. "$ 0, K"  Ej2. c2</param>
        /// <param name="chartid">atributo ID de la grafica</param>
        /// <returns>Regresa un control de tipo chart si el datasource contiene filas y un control de tipo label si el datasource esta vacio</returns>
        public static Control GraficaLinea(DataTable dataSource, string seriesAgrupadasPor, string ejeX, string ejeY, string navegacion, int alturaDeGraf,
                                                     int anchoDeGraf, string tituloGrafSuperior, string tituloGrafEjeX, string tituloGrafEjeY, string axiYLabelStyleFormat, string chartid)
        {
            #region Proceso de creacion de la grafica de linea
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Se instancia la grafica y se liga con el DataSource
                    Chart chart = new Chart();
                    chart.DataBindCrossTable(dataSource.AsEnumerable(), seriesAgrupadasPor, ejeX, ejeY, "");
                    chart.AlignDataPointsByAxisLabel();
                    #endregion

                    #region Atributos de chartArea
                    chart.ChartAreas.Add("ChartArea");
                    chart.ChartAreas[0].BackColor = Color.White;
                    #endregion

                    foreach (Series s in chart.Series)
                    {
                        s.ChartType = SeriesChartType.Line;
                        s.BorderWidth = 2;
                        if (navegacion.Length > 0)
                        {
                            s.Url = navegacion + s.Name.ToString().Replace(" ", "").Trim();
                        }

                        /*RZ.20140306 Se agregan tooltips para grafica de pie*/
                        // Establecer tooltip para las porciones de la grafica
                        s.ToolTip = "#SERIESNAME : #VALY";
                    }

                    #region Leyendas de la grafica
                    chart.Legends.Add("Legend");
                    chart.Legends[0].IsTextAutoFit = true;
                    chart.Legends[0].TextWrapThreshold = 10;
                    chart.Legends[0].Docking = Docking.Bottom;
                    chart.Legends[0].Alignment = StringAlignment.Center;
                    #endregion

                    #region Atributos de AxisX y AxisY
                    //AxisX
                    chart.ChartAreas[0].AxisX.IsMarginVisible = false;
                    chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart.ChartAreas[0].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                    chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                    chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;

                    //AxisY
                    chart.ChartAreas[0].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.LabelStyle.Format = axiYLabelStyleFormat;
                    chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
                    chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
                    chart.ChartAreas[0].AxisY.Minimum = 0;
                    #endregion

                    #region Titulos de la grafica
                    //Titulo superior
                    if (tituloGrafSuperior.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafSuperior,
                                                        Docking.Top,
                                                        new Font("Verdana", 12f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeX
                    if (tituloGrafEjeX.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeX,
                                                        Docking.Bottom,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeY
                    if (tituloGrafEjeY.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeY,
                                                        Docking.Left,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    #endregion

                    #region Dimensiones de la grafica
                    chart.Height = alturaDeGraf;
                    chart.Width = anchoDeGraf;
                    #endregion

                    chart.ID = chartid;

                    return chart;
                }

                catch (Exception ex)
                {

                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaLinea() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        /// <summary>
        /// Regresa una grafica de linea, pasandole como datasource una consulta con 2 columnas
        /// </summary>
        /// <param name="dataSource">DataTable con 2 columnas</param>
        /// <param name="ejeX">Nombre de la columna que representara el ejeX</param>
        /// <param name="ejeY">Nombre de la columna que representara el ejeY</param>
        /// <param name="navegacion">Url que se asignara a cada serie. Nota: Si la propiedad lenght de este string es "0" no se agragaran Url a las series</param>
        /// <param name="alturaDeGraf">Altura de la grafica en pixeles</param>
        /// <param name="anchoDeGraf">Ancho de la grafica en pixeles</param>
        /// <param name="tituloGrafSuperior">Titulo superior de la grafica</param>
        /// <param name="tituloGrafEjeX">Titulo en ejeX de la grafica</param>
        /// <param name="tituloGrafEjeY">Titulo en ejeY de la grafica</param>
        /// <param name="axiYLabelStyleFormat">Formato de valores en eje Y Ej. "$ 0, K"  Ej2. c2</param>
        /// <param name="chartid">atributo ID de la grafica</param>
        /// <param name="mostrarLeyendaDeX">Mostrar leyenda de eje X</param>
        /// <param name="ordenPorDataTable">Toma el orden del datatable</param>
        /// <returns>Regresa un control de tipo chart si el datasource contiene filas y un control de tipo label si el datasource esta vacio</returns>
        public static Control GraficaLinea(DataTable dataSource, string ejeX, string ejeY, string navegacion, int alturaDeGraf, int anchoDeGraf,
                                                    string tituloGrafSuperior, string tituloGrafEjeX, string tituloGrafEjeY, string axiYLabelStyleFormat, string chartid,
                                                    bool mostrarLeyendaDeX, bool ordenPorDataTable)
        {
            #region Proceso de creacion de grafica
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Se instancia la grafica y se asigna datasource
                    Chart chart = new Chart();
                    chart.DataSource = dataSource;
                    #endregion

                    #region Atributos de chartArea
                    chart.ChartAreas.Add("ChartArea");
                    chart.ChartAreas[0].BackColor = Color.White;
                    #endregion

                    #region Leyendas de la grafica
                    if (mostrarLeyendaDeX)
                    {
                        chart.Legends.Add("Legend");
                        chart.Legends[0].IsTextAutoFit = true;
                        chart.Legends[0].LegendStyle = LegendStyle.Column;
                        chart.Legends[0].TextWrapThreshold = 10;
                        chart.Legends[0].Docking = Docking.Bottom;
                        chart.Legends[0].Alignment = StringAlignment.Center;
                    }
                    #endregion

                    #region Series de la grafica
                    chart.Series.Add(ejeX);
                    chart.Series[ejeX].ChartType = SeriesChartType.Line;

                    if (navegacion.Length > 0)
                    {
                        chart.Series[ejeX].Url = navegacion;
                    }

                    chart.Series[ejeX].XValueMember = ejeX;
                    chart.Series[ejeX].YValueMembers = ejeY;

                    /*RZ.20140306 Se agregan tooltips para grafica de pie*/
                    // Establecer tooltip para las porciones de la grafica
                    chart.Series[ejeX].ToolTip = "#VALX : #VALY";
                    #endregion

                    chart.DataBind();

                    #region Atributos de AxisX y AxisY
                    //AxisX
                    chart.ChartAreas[0].AxisX.IsMarginVisible = false;
                    chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart.ChartAreas[0].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                    chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                    chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
                    chart.ChartAreas[0].AxisX.IsLabelAutoFit = true;

                    //Axis Y
                    chart.ChartAreas[0].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.LabelStyle.Format = axiYLabelStyleFormat;
                    chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
                    chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
                    #endregion

                    #region Titulos de la grafica
                    //Titulo superior
                    if (tituloGrafSuperior.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafSuperior,
                                                        Docking.Top,
                                                        new Font("Verdana", 12f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeX
                    if (tituloGrafEjeX.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeX,
                                                        Docking.Bottom,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeY
                    if (tituloGrafEjeY.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeY,
                                                        Docking.Left,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    #endregion

                    #region Dimensiones de la grafica
                    chart.Height = alturaDeGraf;
                    chart.Width = anchoDeGraf;
                    #endregion

                    chart.ID = chartid;

                    return chart;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaHistorica() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        /// <summary>
        ///  Regresa una grafica de pie, pasandole como datasource una consulta con 2 columnas
        /// </summary>
        /// <param name="dataSource">DataTable con 2 columnas</param>
        /// <param name="ejeX">Nombre de la columna que representara el ejeX</param>
        /// <param name="ejeY">Nombre de la columna que representara el ejeY</param>
        /// <param name="navegacion">Url que se asignara a cada serie. Nota: Si la propiedad lenght de este string es "0" no se agragaran Url a las series</param>
        /// <param name="alturaDeGraf">Altura de la grafica en pixeles</param>
        /// <param name="anchoDeGraf">Ancho de la grafica en pixeles</param>
        /// <param name="tituloGrafSuperior">Titulo superior de la grafica</param>
        /// <param name="tituloGrafEjeX">Titulo en ejeX de la grafica</param>
        /// <param name="tituloGrafEjeY">Titulo en ejeY de la grafica</param>
        /// <param name="chartid">atributo ID de la grafica</param>
        /// <returns>Regresa un control de tipo chart si el datasource contiene filas y un control de tipo label si el datasource esta vacio</returns>
        public static Control GraficaPie(DataTable dataSource, string ejeX, string ejeY, string navegacion, int alturaDeGraf, int anchoDeGraf,
                                                  string tituloGrafSuperior, string tituloGrafEjeX, string tituloGrafEjeY, string chartid)
        {
            #region Proceso de creacion de la grafica
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Se instancia la grafica, se le agrega una area de grafica y se asigna el datasource
                    Chart chart = new Chart();
                    chart.DataSource = dataSource;
                    #endregion

                    #region Series de la grafica
                    chart.Series.Add(ejeX);
                    chart.Series[ejeX].ChartType = SeriesChartType.Pie;
                    chart.Series[ejeX].Label = "#PERCENT";
                    chart.Series[ejeX].LegendText = "#VALX";

                    if (navegacion.Length > 0)
                    {
                        chart.Series[ejeX].Url = navegacion;
                    }

                    chart.Series[ejeX].XValueMember = ejeX;
                    chart.Series[ejeX].YValueMembers = ejeY;
                    chart.Series[ejeX].IsVisibleInLegend = true;
                    chart.Series[ejeX].IsValueShownAsLabel = true;
                    chart.Series[ejeX]["PieLabelStyle"] = "Outside";
                    chart.Series[ejeX]["PieLineColor"] = "black";

                    /*RZ.20140306 Se agregan tooltips para grafica de pie*/
                    // Establecer tooltip para las porciones de la grafica
                    chart.Series[ejeX].ToolTip = "#VALX : #VALY";
                    #endregion

                    #region Atributos de chartArea
                    chart.ChartAreas.Add("ChartArea1");
                    chart.ChartAreas["ChartArea1"].BackColor = Color.White;
                    #endregion

                    #region Leyendas de la grafica
                    chart.Legends.Add("Legend");
                    chart.Legends[0].MaximumAutoSize = 20;
                    chart.Legends[0].IsTextAutoFit = true;
                    chart.Legends[0].TextWrapThreshold = 10;
                    chart.Legends[0].Docking = Docking.Bottom;
                    chart.Legends[0].Alignment = StringAlignment.Center;
                    #endregion

                    chart.DataBind();
                    chart.DataManipulator.GroupByAxisLabel("SUM", ejeX);

                    #region Atributos de AxisX y AxisY
                    //Axis X
                    chart.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisX.Name = ejeX;
                    chart.ChartAreas["ChartArea1"].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                    //Axis Y
                    chart.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Enabled = false;
                    #endregion


                    //chart.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                    //chart.ChartAreas["ChartArea1"].Area3DStyle.Inclination = 0;


                    #region Titulos de la grafica
                    //Titulo superior
                    if (tituloGrafSuperior.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafSuperior,
                                                        Docking.Top,
                                                        new Font("Verdana", 12f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeX
                    if (tituloGrafEjeX.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeX,
                                                        Docking.Bottom,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeY
                    if (tituloGrafEjeY.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeY,
                                                        Docking.Left,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    #endregion

                    #region Dimensiones de la grafica
                    chart.Height = alturaDeGraf;
                    chart.Width = anchoDeGraf;
                    #endregion

                    chart.ID = chartid;

                    return chart;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaPie() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si DataSource no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        /// <summary>
        ///  Regresa una grafica de pie, pasandole como datasource una consulta con 2 columnas
        /// </summary>
        /// <param name="dataSource">DataTable con 2 columnas</param>
        /// <param name="ejeX">Nombre de la columna que representara el ejeX</param>
        /// <param name="ejeY">Nombre de la columna que representara el ejeY</param>
        /// <param name="navegacion">Url que se asignara a cada serie. Nota: Si la propiedad lenght de este string es "0" no se agragaran Url a las series</param>
        /// <param name="mostrarEn3D">Muestra la grafica en 3D si se manda un true</param>
        /// <param name="PieLabelStyleOutSide">Muestra las etiquetas por fuera de la grafica si se manda un true</param>
        /// <param name="alturaDeGraf">Altura de la grafica en pixeles</param>
        /// <param name="anchoDeGraf">Ancho de la grafica en pixeles</param>
        /// <param name="tituloGrafSuperior">Titulo superior de la grafica</param>
        /// <param name="tituloGrafEjeX">Titulo en ejeX de la grafica</param>
        /// <param name="tituloGrafEjeY">Titulo en ejeY de la grafica</param>
        /// <param name="chartid">atributo ID de la grafica</param>
        /// <returns>Regresa un control de tipo chart si el datasource contiene filas y un control de tipo label si el datasource esta vacio</returns>
        public static Control GraficaAnillo(DataTable dataSource, string ejeX, string ejeY, string navegacion, bool mostrarEn3D, bool PieLabelStyleOutSide, int alturaDeGraf, int anchoDeGraf,
                                                  string tituloGrafSuperior, string tituloGrafEjeX, string tituloGrafEjeY, string chartid)
        {
            #region Proceso de creacion de la grafica
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Se instancia la grafica, se le agrega una area de grafica y se asigna el datasource
                    Chart chart = new Chart();
                    chart.DataSource = dataSource;
                    #endregion

                    #region Series de la grafica
                    chart.Series.Add(ejeX);
                    chart.Series[ejeX].ChartType = SeriesChartType.Doughnut;
                    chart.Series[ejeX].Label = "#PERCENT";
                    chart.Series[ejeX].LegendText = "#VALX";

                    if (navegacion.Length > 0)
                    {
                        chart.Series[ejeX].Url = navegacion;
                    }

                    chart.Series[ejeX].XValueMember = ejeX;
                    chart.Series[ejeX].YValueMembers = ejeY;
                    chart.Series[ejeX].IsVisibleInLegend = true;
                    chart.Series[ejeX].IsValueShownAsLabel = true;
                    if (PieLabelStyleOutSide)
                    {
                        chart.Series[ejeX]["PieLabelStyle"] = "Outside";
                        chart.Series[ejeX]["PieLineColor"] = "black";
                    }

                    /*RZ.20140306 Se agregan tooltips para grafica de pie*/
                    // Establecer tooltip para las porciones de la grafica
                    chart.Series[ejeX].ToolTip = "#VALX : #VALY";
                    #endregion

                    #region Atributos de chartArea
                    chart.ChartAreas.Add("ChartArea1");
                    chart.ChartAreas["ChartArea1"].BackColor = Color.White;
                    #endregion

                    #region Leyendas de la grafica
                    chart.Legends.Add("Legend");
                    chart.Legends[0].MaximumAutoSize = 20;
                    chart.Legends[0].IsTextAutoFit = true;
                    chart.Legends[0].TextWrapThreshold = 10;
                    chart.Legends[0].Docking = Docking.Bottom;
                    chart.Legends[0].Alignment = StringAlignment.Center;
                    #endregion

                    chart.DataBind();
                    chart.DataManipulator.GroupByAxisLabel("SUM", ejeX);

                    #region Atributos de AxisX y AxisY
                    //Axis X
                    chart.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisX.Name = ejeX;
                    chart.ChartAreas["ChartArea1"].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                    //Axis Y
                    chart.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Enabled = false;
                    #endregion

                    // Mostrar en 3D
                    if (mostrarEn3D)
                    {
                        chart.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                    }

                    #region Titulos de la grafica
                    //Titulo superior
                    if (tituloGrafSuperior.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafSuperior,
                                                        Docking.Top,
                                                        new Font("Verdana", 12f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeX
                    if (tituloGrafEjeX.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeX,
                                                        Docking.Bottom,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeY
                    if (tituloGrafEjeY.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeY,
                                                        Docking.Left,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    #endregion

                    #region Dimensiones de la grafica
                    chart.Height = alturaDeGraf;
                    chart.Width = anchoDeGraf;
                    #endregion

                    chart.ID = chartid;

                    return chart;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaPie() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si DataSource no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        /// <summary>
        ///  Regresa una grafica de barras, pasandole como datasource una consulta con 2 columnas
        /// </summary>
        /// <param name="dataSource">DataTable con 2 columnas</param>
        /// <param name="ejeX">Nombre de la columna que representara el ejeX</param>
        /// <param name="ejeY">Nombre de la columna que representara el ejeY</param>
        /// <param name="navegacion">Url que se asignara a cada serie. Nota: Si la propiedad lenght de este string es "0" no se agragaran Url a las series</param>
        /// <param name="alturaDeGraf">Altura de la grafica en pixeles</param>
        /// <param name="anchoDeGraf">Ancho de la grafica en pixeles</param>
        /// <param name="tituloGrafSuperior">Titulo superior de la grafica</param>
        /// <param name="tituloGrafEjeX">Titulo en ejeX de la grafica</param>
        /// <param name="tituloGrafEjeY">Titulo en ejeY de la grafica</param>
        /// <param name="axiYLabelStyleFormat">Formato de valores en eje Y Ej. "$ 0, K"  Ej2. c2</param>
        /// <param name="chartid">atributo ID de la grafica</param>
        /// <returns>Regresa un control de tipo chart si el datasource contiene filas y un control de tipo label si el datasource esta vacio</returns>
        public static Control GraficaBarras(DataTable dataSource, string ejeX, string ejeY, string navegacion, int alturaDeGraf, int anchoDeGraf,
                                                      string tituloGrafSuperior, string tituloGrafEjeX, string tituloGrafEjeY, string axiYLabelStyleFormat, string chartid)
        {
            #region Elimina columnas no necesarias en el gridview
            if (dataSource.Columns.Contains("RID"))
                dataSource.Columns.Remove("RID");
            if (dataSource.Columns.Contains("RowNumber"))
                dataSource.Columns.Remove("RowNumber");
            if (dataSource.Columns.Contains("TopRID"))
                dataSource.Columns.Remove("TopRID");
            #endregion // Elimina columnas no necesarias en el gridview

            #region Proceso de creacion de la grafica
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Se instancia la grafica y se asigna el datasource
                    Chart chart = new Chart();
                    chart.DataSource = dataSource;
                    #endregion

                    #region Atributos de chartArea
                    chart.ChartAreas.Add("ChartArea1");
                    chart.ChartAreas["ChartArea1"].BackColor = Color.White;
                    #endregion

                    #region Leyendas de la grafica
                    chart.Legends.Add("Legend");
                    chart.Legends[0].IsTextAutoFit = true;
                    chart.Legends[0].TextWrapThreshold = 10;
                    chart.Legends[0].Docking = Docking.Bottom;
                    chart.Legends[0].Alignment = StringAlignment.Center;
                    #endregion

                    #region Series de la grafica
                    chart.Series.Add(ejeX);
                    chart.Series[ejeX].ChartType = SeriesChartType.Column;

                    if (navegacion.Length > 0)
                    {
                        chart.Series[ejeX].Url = navegacion;
                    }

                    chart.Series[ejeX].XValueMember = ejeX;
                    chart.Series[ejeX].YValueMembers = ejeY;

                    /*RZ.20140306 Se agregan tooltips para grafica de pie*/
                    // Establecer tooltip para las porciones de la grafica
                    chart.Series[ejeX].ToolTip = "#VALX : #VALY";
                    #endregion

                    chart.DataBind();

                    #region Atributos de AxisX y AxisY
                    //Axis X
                    chart.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisX.Name = ejeX;
                    chart.ChartAreas["ChartArea1"].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                    //Axis Y
                    chart.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Enabled = false;
                    chart.ChartAreas["ChartArea1"].AxisY.LabelStyle.Format = axiYLabelStyleFormat;
                    chart.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
                    #endregion

                    #region Titulos de la grafica
                    //Titulo superior
                    if (tituloGrafSuperior.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafSuperior,
                                                        Docking.Top,
                                                        new Font("Verdana", 12f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeX
                    if (tituloGrafEjeX.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeX,
                                                        Docking.Bottom,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    //Titulo ejeY
                    if (tituloGrafEjeY.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        tituloGrafEjeY,
                                                        Docking.Left,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    #endregion

                    #region Dimensiones de la grafica
                    chart.Height = alturaDeGraf;
                    chart.Width = anchoDeGraf;
                    #endregion

                    // Show as 2D or 3
                    //Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = checkBoxShow3D.Checked;

                    chart.ID = chartid;

                    return chart;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaBarras() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        public static GridView GridView(string gridID, DataTable dataSource, bool agregaTotales, string tituloCeldaTotales, string[] formatStringColumnas, int numDecimales = 2, bool headerFijo = true, bool EncodHtml = true)
        {
            #region Proceso de creacion del gridview
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (dataSource.Columns.Contains("RID"))
                        dataSource.Columns.Remove("RID");
                    if (dataSource.Columns.Contains("RowNumber"))
                        dataSource.Columns.Remove("RowNumber");
                    if (dataSource.Columns.Contains("TopRID"))
                        dataSource.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.ID = gridID;
                    lgrv.CssClass = headerFijo ? "fixed_header table table-bordered tableDashboard" : "table table-bordered tableDashboard";

                    if (agregaTotales == true)
                    {
                        #region Agrega una linea al DataTable para calcular los totales

                        DataRow dr = dataSource.NewRow();

                        dr[0] = tituloCeldaTotales;

                        for (int ent = 1; ent < dataSource.Columns.Count; ent++)
                        {
                            if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String"))
                            {
                                dr[dataSource.Columns[ent].ColumnName] = dataSource.Compute("Sum([" + dataSource.Columns[ent].ColumnName + "])", "");

                                if (dr[dataSource.Columns[ent].ColumnName] != DBNull.Value)
                                    dr[dataSource.Columns[ent].ColumnName] = Math.Round(Convert.ToDecimal(dr[dataSource.Columns[ent].ColumnName]), numDecimales);
                            }
                        }

                        dataSource.Rows.Add(dr);
                        dataSource.AcceptChanges();

                        #endregion // Agrega una linea al DataTable para calcular los totales
                    }

                    #region Agrega un BoundField por cada columna del datarow

                    int i = 0;
                    foreach (DataColumn dc in dataSource.Columns)
                    {
                        BoundField bf = new BoundField();
                        bf.DataField = dc.ColumnName;
                        bf.HeaderText = dc.ColumnName;
                        bf.HtmlEncode = EncodHtml;
                        if (formatStringColumnas[i] != "")
                        {
                            bf.DataFormatString = formatStringColumnas[i]; //"{0:c}";
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        }
                        else
                        {
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        }
                        lgrv.Columns.Add(bf);
                        i++;
                        lgrv.AutoGenerateColumns = false;
                    }

                    #endregion // Agrega un BoundField por cada columna del datarow

                    lgrv.DataSource = dataSource;
                    lgrv.DataBind();

                    lgrv.UseAccessibleHeader = true;
                    lgrv.HeaderRow.TableSection = TableRowSection.TableHeader;

                    //Se vuelve a validar si se agregaron totales para agregar estilo a ultima fila pero despues de hacer el databind()
                    if (agregaTotales == true)
                    {
                        // Le da formato a la ultima fila del gridview para darle estilo keytia
                        GridViewRow row = lgrv.Rows[dataSource.Rows.Count - 1];
                        row.Cells[0].Text = tituloCeldaTotales;

                        for (int tot = 0; tot < row.Cells.Count; tot++)
                        {
                            row.Cells[tot].CssClass = "lastRowTable";
                        }
                    }

                    return lgrv;
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return RepSinDatos();
            }
            #endregion
        }

        /// <summary>
        /// Regresa un gridview, pasandole como datasource un datatable
        /// </summary>
        /// <param name="gridID">atributo ID del gridview</param>
        /// <param name="dataSource">DataTable a mostrar en gridView</param>
        /// <param name="agregaTotales">Si el DataTable es una matriz con la primer columna con algun concepto 
        /// y las demas columnas con numeros, se debe mandar un true en caso de necesitar agregar los totales en
        /// una fila complementaria al final del gridview</param>
        /// <param name="tituloCeldaTotales">Texto a mostrar en la primer celda de la fila con los totales Ej. "Totales" Ej2. "Consumo Total" etc.</param>
        /// <param name="HoriAndVertScrollBar">Se agrega un scrollbar vertical  y horizontal al contenedor del gridview</param>
        /// <param name="alturaContenedorGridView">Altura en pixeles del contenedor del gridview. Nota: Si es 0 se ajusta la propiedad height a "auto"</param>
        /// <returns></returns>  NO
        public static GridView GridView(string gridID, DataTable dataSource, bool agregaTotales, string tituloCeldaTotales, bool headerFijo = true)
        {
            #region Proceso de creacion del gridview
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (dataSource.Columns.Contains("RID"))
                        dataSource.Columns.Remove("RID");
                    if (dataSource.Columns.Contains("RowNumber"))
                        dataSource.Columns.Remove("RowNumber");
                    if (dataSource.Columns.Contains("TopRID"))
                        dataSource.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.ID = gridID;
                    lgrv.CssClass = headerFijo ? "fixed_header table table-bordered tableDashboard" : "table table-bordered tableDashboard";

                    if (agregaTotales == true)
                    {
                        #region Agrega una linea al DataTable para calcular los totales

                        DataRow dr = dataSource.NewRow();

                        dr[0] = tituloCeldaTotales;

                        for (int ent = 1; ent < dataSource.Columns.Count; ent++)
                        {
                            if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String"))
                                dr[dataSource.Columns[ent].ColumnName] = dataSource.Compute("Sum([" + dataSource.Columns[ent].ColumnName + "])", "");
                        }

                        dataSource.Rows.Add(dr);
                        dataSource.AcceptChanges();

                        #endregion // Agrega una linea al DataTable para calcular los totales

                        #region Agrega un BoundField por cada columna del datarow

                        int i = 0;
                        foreach (DataColumn dc in dataSource.Columns)
                        {
                            BoundField bf = new BoundField();
                            bf.DataField = dc.ColumnName;
                            bf.HeaderText = dc.ColumnName;
                            if (dc.DataType != System.Type.GetType("System.String"))
                            {
                                bf.DataFormatString = "{0:c}";
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                            }
                            else
                            {
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            }

                            lgrv.Columns.Add(bf);

                            i++;
                        }

                        #endregion // Agrega un BoundField por cada columna del datarow

                        lgrv.AutoGenerateColumns = false;
                    }
                    lgrv.DataSource = dataSource;
                    lgrv.DataBind();

                    lgrv.UseAccessibleHeader = true;
                    lgrv.HeaderRow.TableSection = TableRowSection.TableHeader;

                    //Se vuelve a validar si se agregaron totales para agregar estilo a ultima fila pero despues de hacer el databind()
                    if (agregaTotales == true)
                    {
                        // Le da formato a la ultima fila del gridview para darle estilo keytia
                        GridViewRow row = lgrv.Rows[dataSource.Rows.Count - 1];
                        row.Cells[0].Text = tituloCeldaTotales;

                        for (int tot = 0; tot < row.Cells.Count; tot++)
                        {
                            row.Cells[tot].CssClass = "lastRowTable";
                        }
                    }

                    return lgrv;
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return RepSinDatos();
            }
            #endregion
        }

        /// <summary>
        /// Regresa un gridview, pasandole como datasource un datatable y agregando la primer columna con un URL de navegación
        /// </summary>
        /// <param name="dataSource">DataTable a mostrar en gridView Nota: Este metodo necesita recibir un dataTable con la primer columna con valores numericos, 
        /// la segunda columna debe ser el concepto de la matriz, y las otras columnas deben ser numeros para poder agregar totales</param>
        /// <param name="HoriAndVertScrollBar">Se agrega un scrollbar vertical  y horizontal al contenedor del gridview</param>
        /// <param name="alturaContenedorGridView">Altura en pixeles del contenedor del gridview Nota: Si se manda el valor de 0 no se asigna un valor a la propiedad height</param>
        /// <param name="agregaTotales">Si el DataTable es una matriz con la primer columna con algun concepto 
        /// y las demas columnas con numeros, se debe mandar un true en caso de necesitar agregar los totales en
        /// una fila complementaria al final del gridview</param>
        /// <param name="tituloCeldaTotales">Texto a mostrar en la primer celda de la fila con los totales Ej. "Totales" Ej2. "Consumo Total" etc.</param>
        /// <param name="columnaIDNavegacion">Nombre de la primer columna del datatable que contiene el valor que mandamos en el URL del HyperLinkField
        /// Nota: Esta columna no se ve en web</param>
        /// <param name="headerTextNav">Titulo que se mostrara en la primer columna del reporte en web</param>
        /// <param name="dataNavigateUrlFormatString">El Url que se formara en el campo Ej. "www.host.com/MiPagina?Parametro1={1} donde "1" es el index del string[] que se manda de parametro"</param>
        /// <param name="urlFields">Arreglo de strings de los campos con el valor del campo con hiperlink Ej. string[] {"Campo1" , "ID"}</param>
        /// <param name="columnaAMostrar">Nombre de la primer columna que se muestra en el gridview</param>
        /// <param name="IndexCeldaTotales">Indice de la columna del DataTable en donde se incluira la palabra "Totales"</param>
        /// <param name="formatStringColumnas">Se mandan los formatos de cada columna</param>
        /// <param name="gridID">atributo ID del gridview</param>
        /// <param name="indexColumnasDatosNavegacion">Arreglo de indices de las columnas del DataTable que no deseamos mostrar en web</param>
        /// <param name="indexColumnasBoundField">Arreglo de indices de las columnas del DataTable que "NO" tienen navegacion(url)</param>
        /// <param name="indexColumnasHyperLinkField">Arreglo de indices de las columnas del DataTable que tienen navegacion(url)</param>
        /// <returns></returns>
        public static GridView GridView(string gridID, DataTable dataSource, bool agregaTotales, string tituloCeldaTotales,
                                                string[] formatStringColumnas, string dataNavigateUrlFormatString, string[] urlFields, int IndexCeldaTotales,
                                                int[] indexColumnasDatosNavegacion, int[] indexColumnasBoundField, int[] indexColumnasHyperLinkField, int numDecimales = 2, bool headerFijo = true)
        {

            #region Proceso de creacion del gridview
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (dataSource.Columns.Contains("RID"))
                        dataSource.Columns.Remove("RID");
                    if (dataSource.Columns.Contains("RowNumber"))
                        dataSource.Columns.Remove("RowNumber");
                    if (dataSource.Columns.Contains("TopRID"))
                        dataSource.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.ID = gridID;
                    lgrv.CssClass = headerFijo ? "fixed_header table table-bordered tableDashboard" : "table table-bordered tableDashboard";

                    #region Agrega una linea al DataTable para calcular los totales
                    if (agregaTotales == true)
                    {
                        DataRow dr = dataSource.NewRow();

                        dr[IndexCeldaTotales] = tituloCeldaTotales;

                        for (int ent = 0; ent < dataSource.Columns.Count; ent++)
                        {
                            if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String") && !indexColumnasDatosNavegacion.Contains(ent))
                            {
                                dr[dataSource.Columns[ent].ColumnName] = dataSource.Compute("Sum([" + dataSource.Columns[ent].ColumnName + "])", "");

                                if (dr[dataSource.Columns[ent].ColumnName] != DBNull.Value)
                                    dr[dataSource.Columns[ent].ColumnName] = Math.Round(Convert.ToDecimal(dr[dataSource.Columns[ent].ColumnName]), numDecimales);
                            }
                            else
                            {
                                if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String"))
                                {
                                    dr[ent] = 0;
                                }
                                else if (ent != IndexCeldaTotales)
                                {
                                    dr[ent] = "";
                                }
                            }
                        }

                        dataSource.Rows.Add(dr);
                        dataSource.AcceptChanges();
                    }

                    #endregion // Agrega una linea al DataTable para calcular los totales

                    #region Agrega un tipo de columna por cada columna del datarow

                    int i = 0;
                    foreach (DataColumn dc in dataSource.Columns)
                    {
                        BoundField bf = new BoundField();
                        bf.DataField = dc.ColumnName;
                        bf.HeaderText = dc.ColumnName;

                        HyperLinkField hlf = new HyperLinkField();
                        hlf.HeaderText = dc.ColumnName;

                        if (indexColumnasBoundField.Contains(i))
                        {
                            bf.DataFormatString = formatStringColumnas[i]; //"{0:c}";

                            //Si es string lo alineo a la izquierda
                            if (dc.DataType.FullName == "System.String")
                            {
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            }
                            else
                            {
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                            }
                            lgrv.Columns.Add(bf);
                        }
                        else if (indexColumnasDatosNavegacion.Contains(i))  // Columna de iCodCatalogo
                        {
                            bf.DataField = dataSource.Columns[i].ColumnName;
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            bf.Visible = false;
                            lgrv.Columns.Add(bf);
                        }
                        else if (indexColumnasHyperLinkField.Contains(i))
                        {
                            hlf.HeaderText = dataSource.Columns[i].ColumnName;
                            hlf.DataNavigateUrlFields = urlFields;
                            hlf.DataNavigateUrlFormatString = dataNavigateUrlFormatString;
                            hlf.DataTextField = dataSource.Columns[i].ColumnName;
                            hlf.NavigateUrl = dataNavigateUrlFormatString;
                            hlf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            hlf.ControlStyle.Font.Bold = true;
                            lgrv.Columns.Add(hlf);
                        }

                        i++;
                    }

                    #endregion // Agrega un BoundField por cada columna del datarow

                    lgrv.AutoGenerateColumns = false;
                    lgrv.DataSource = dataSource;
                    lgrv.DataBind();

                    lgrv.UseAccessibleHeader = true;
                    lgrv.HeaderRow.TableSection = TableRowSection.TableHeader;

                    //Se vuelve a validar si se agregaron totales para agregar estilo a ultima fila pero despues de hacer el databind()
                    if (agregaTotales == true)
                    {
                        GridViewRow row = lgrv.Rows[dataSource.Rows.Count - 1];
                        row.Cells[IndexCeldaTotales].Text = tituloCeldaTotales;

                        for (int tot = 0; tot < row.Cells.Count; tot++)
                        {
                            row.Cells[tot].CssClass = "lastRowTable";
                        }
                    }

                    return lgrv;
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion
            else
            {
                return RepSinDatos();
            }
        }

        //public static GridView GridView(string gridID, DataTable dataSource, bool agregaTotales, string tituloCeldaTotales,
        //                                       string[] formatStringColumnas, int IndexCeldaTotales, int numDecimales = 2, bool headerFijo = true)
        //{

        //    #region Proceso de creacion del gridview
        //    if (dataSource.Rows.Count > 0)
        //    {
        //        try
        //        {
        //            #region Elimina columnas no necesarias en el gridview
        //            if (dataSource.Columns.Contains("RID"))
        //                dataSource.Columns.Remove("RID");
        //            if (dataSource.Columns.Contains("RowNumber"))
        //                dataSource.Columns.Remove("RowNumber");
        //            if (dataSource.Columns.Contains("TopRID"))
        //                dataSource.Columns.Remove("TopRID");
        //            #endregion // Elimina columnas no necesarias en el gridview

        //            GridView lgrv = new GridView();
        //            lgrv.ID = gridID;
        //            lgrv.CssClass = headerFijo ? "fixed_header table table-bordered tableDashboard" : "table table-bordered tableDashboard";

        //            #region Agrega una linea al DataTable para calcular los totales
        //            if (agregaTotales == true)
        //            {
        //                DataRow dr = dataSource.NewRow();

        //                dr[IndexCeldaTotales] = tituloCeldaTotales;

        //                for (int ent = 0; ent < dataSource.Columns.Count; ent++)
        //                {
        //                    if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String"))
        //                    {
        //                        dr[dataSource.Columns[ent].ColumnName] = dataSource.Compute("Sum([" + dataSource.Columns[ent].ColumnName + "])", "");

        //                        if (dr[dataSource.Columns[ent].ColumnName] != DBNull.Value)
        //                            dr[dataSource.Columns[ent].ColumnName] = Math.Round(Convert.ToDecimal(dr[dataSource.Columns[ent].ColumnName]), numDecimales);
        //                    }
        //                    else
        //                    {
        //                        if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String"))
        //                        {
        //                            dr[ent] = 0;
        //                        }
        //                        else if (ent != IndexCeldaTotales)
        //                        {
        //                            dr[ent] = "";
        //                        }
        //                    }
        //                }

        //                dataSource.Rows.Add(dr);
        //                dataSource.AcceptChanges();
        //            }

        //            #endregion // Agrega una linea al DataTable para calcular los totales

        //            #region Agrega un tipo de columna por cada columna del datarow

        //            int i = 0;
        //            foreach (DataColumn dc in dataSource.Columns)
        //            {

        //                BoundField bf = new BoundField();
        //                bf.DataField = dc.ColumnName;
        //                bf.HeaderText = dc.ColumnName;

        //                HyperLinkField hlf = new HyperLinkField();
        //                hlf.HeaderText = dc.ColumnName;
        //                if(i==0)
        //                {

        //                }
        //                else
        //                {
        //                    hlf.HeaderText = dataSource.Columns[i].ColumnName;
        //                    hlf.DataNavigateUrlField = urlFields;
        //                    hlf.DataNavigateUrlFormatString = dataNavigateUrlFormatString;
        //                    hlf.DataTextField = dataSource.Columns[i].ColumnName;
        //                    hlf.NavigateUrl = dataNavigateUrlFormatString;
        //                    hlf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        //                    hlf.ControlStyle.Font.Bold = true;
        //                    lgrv.Columns.Add(hlf);
        //                }
                        

        //                i++;
        //            }

        //            #endregion // Agrega un BoundField por cada columna del datarow

        //            lgrv.AutoGenerateColumns = false;
        //            lgrv.DataSource = dataSource;
        //            lgrv.DataBind();

        //            lgrv.UseAccessibleHeader = true;
        //            lgrv.HeaderRow.TableSection = TableRowSection.TableHeader;

        //            //Se vuelve a validar si se agregaron totales para agregar estilo a ultima fila pero despues de hacer el databind()
        //            if (agregaTotales == true)
        //            {
        //                GridViewRow row = lgrv.Rows[dataSource.Rows.Count - 1];
        //                row.Cells[IndexCeldaTotales].Text = tituloCeldaTotales;

        //                for (int tot = 0; tot < row.Cells.Count; tot++)
        //                {
        //                    row.Cells[tot].CssClass = "lastRowTable";
        //                }
        //            }

        //            return lgrv;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new KeytiaWebException(
        //                "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
        //                + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
        //        }
        //    }
        //    #endregion
        //    else
        //    {
        //        return RepSinDatos();
        //    }
        //}

        //NZ 20160229 Se creo para Dashboard TIM.


        /// <summary>
        /// Regresa un gridview, pasandole como datasource un datatable
        /// </summary>
        /// <param name="gridID">atributo ID del gridview</param>
        /// <param name="dataSource">DataTable a mostrar en gridView</param>
        /// <param name="agregaTotales">Si el DataTable es una matriz con la primer columna con algun concepto 
        /// y las demas columnas con numeros, se debe mandar un true en caso de necesitar agregar los totales en
        /// una fila complementaria al final del gridview</param>
        /// <param name="tituloCeldaTotales">Texto a mostrar en la primer celda de la fila con los totales Ej. "Totales" Ej2. "Consumo Total" etc.</param>
        /// <param name="HoriAndVertScrollBar">Se agrega un scrollbar vertical  y horizontal al contenedor del gridview</param>
        /// <param name="alturaContenedorGridView">Altura en pixeles del contenedor del gridview. Nota: Si es 0 se ajusta la propiedad height a "auto"</param>
        /// <param name="formatStringColumnas">Se mandan los formatos de cada columna. Nota: Si el formato es = "", es para los campos de texto y la alineación se hace a la izquierda </param>

        /// <param name="indexColImage">Indices de las columnas que contendran una imagen. (Sin link)</param>
        /// <param name="urlColImage"></param>
        /// <param name="indexToolTip">Mensaje para mostrar al pasar el cursor sobre las imagenes. Puede ir Vacio</param>
        /// <returns></returns>
        public static GridView GridViewWithImage(string gridID, DataTable dataSource, bool agregaTotales, string tituloCeldaTotales,
                                                string[] formatStringColumnas, int[] indexColImage, int[] urlColImage, int[] indexToolTip, int numDecimales = 2, bool headerFijo = true)
        {
            #region Proceso de creacion del gridview
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (dataSource.Columns.Contains("RID"))
                        dataSource.Columns.Remove("RID");
                    if (dataSource.Columns.Contains("RowNumber"))
                        dataSource.Columns.Remove("RowNumber");
                    if (dataSource.Columns.Contains("TopRID"))
                        dataSource.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.ID = gridID;
                    lgrv.CssClass = headerFijo ? "fixed_header table table-bordered tableDashboard" : "table table-bordered tableDashboard";

                    if (agregaTotales == true)
                    {
                        #region Agrega una linea al DataTable para calcular los totales

                        DataRow dr = dataSource.NewRow();

                        dr[0] = tituloCeldaTotales;

                        for (int ent = 1; ent < dataSource.Columns.Count; ent++)
                        {
                            if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String"))
                            {
                                dr[dataSource.Columns[ent].ColumnName] = dataSource.Compute("Sum([" + dataSource.Columns[ent].ColumnName + "])", "");

                                if (dr[dataSource.Columns[ent].ColumnName] != DBNull.Value)
                                    dr[dataSource.Columns[ent].ColumnName] = Math.Round(Convert.ToDecimal(dr[dataSource.Columns[ent].ColumnName]), numDecimales);
                            }
                        }

                        dataSource.Rows.Add(dr);
                        dataSource.AcceptChanges();

                        #endregion // Agrega una linea al DataTable para calcular los totales
                    }

                    #region Agrega un BoundField por cada columna del datarow

                    int i = 0;
                    int x = 0;
                    foreach (DataColumn dc in dataSource.Columns)
                    {
                        BoundField bf = new BoundField();
                        bf.DataField = dc.ColumnName;
                        bf.HeaderText = dc.ColumnName;

                        ImageField imgF = new ImageField();
                        imgF.HeaderText = dc.ColumnName;

                        if (urlColImage.Contains(i) || indexToolTip.Contains(i))
                        {
                            bf.DataField = dataSource.Columns[i].ColumnName;
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            bf.Visible = false;
                            lgrv.Columns.Add(bf);
                        }
                        else if (indexColImage.Contains(i))
                        {
                            imgF.DataImageUrlField = dataSource.Columns[urlColImage[x]].ColumnName;

                            if (indexToolTip.Length > 0)
                            {
                                imgF.DataAlternateTextField = dataSource.Columns[indexToolTip[x]].ColumnName;
                            }
                            imgF.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                            imgF.ItemStyle.VerticalAlign = VerticalAlign.Middle;
                            lgrv.Columns.Add(imgF);
                            /////x++;
                        }
                        else if (formatStringColumnas[i] != "")
                        {
                            bf.DataFormatString = formatStringColumnas[i]; //"{0:c}";
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                            lgrv.Columns.Add(bf);
                        }
                        else
                        {
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            lgrv.Columns.Add(bf);
                        }
                        i++;
                    }

                    #endregion // Agrega un BoundField por cada columna del datarow

                    lgrv.AutoGenerateColumns = false;
                    lgrv.DataSource = dataSource;
                    lgrv.DataBind();
                    lgrv.UseAccessibleHeader = true;
                    lgrv.HeaderRow.TableSection = TableRowSection.TableHeader;

                    //Se vuelve a validar si se agregaron totales para agregar estilo a ultima fila pero despues de hacer el databind()
                    if (agregaTotales == true)
                    {
                        GridViewRow row = lgrv.Rows[dataSource.Rows.Count - 1];
                        row.Cells[0].Text = tituloCeldaTotales;

                        for (int tot = 0; tot < row.Cells.Count; tot++)
                        {
                            row.Cells[tot].CssClass = "lastRowTable";
                        }
                    }

                    if (indexToolTip.Length > 0)
                    {
                        for (int w = 0; w < dataSource.Rows.Count; w++)
                        {
                            lgrv.Rows[w].ToolTip = dataSource.Rows[w][indexToolTip[x]].ToString();
                        }
                    }

                    return lgrv;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return RepSinDatos();
            }
            #endregion
        }


        /// <summary>
        /// Crea un indicador horizontal que muestra el un número y una descripción.
        /// </summary>
        /// <param name="indicador">Debe ser un número</param>
        /// <param name="descripcion">Una cadena con la descripción</param>
        /// <returns>Regresa un panel con una tabla horizontal con 2 celdas, la primera con el valor de "indicador" y la segunda con el valor de "descripción"</returns>
        public static Control IndicadorHorizontal(string indicador, string descripcion)
        {
            Table ltbl = new Table();
            ltbl.Width = Unit.Percentage(100);
            ltbl.Height = Unit.Percentage(100);
            TableRow ltr = new TableRow();
            ltr.Width = Unit.Percentage(100);
            ltr.Height = Unit.Percentage(100);

            TableCell ltc1 = new TableCell();
            ltc1.Width = Unit.Percentage(30);
            ltc1.Height = Unit.Percentage(100);
            ltc1.Text = indicador;
            ltc1.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
            ltc1.Style.Add(HtmlTextWriterStyle.FontSize, "30px");
            ltc1.Style.Add(HtmlTextWriterStyle.TextAlign, "right");

            TableCell ltc2 = new TableCell();
            ltc2.Width = Unit.Percentage(70);
            ltc2.Height = Unit.Percentage(100);
            ltc2.Text = descripcion;
            ltc2.Style.Add(HtmlTextWriterStyle.FontSize, "16px");
            ltc2.Style.Add(HtmlTextWriterStyle.TextAlign, "left");

            ltbl.HorizontalAlign = HorizontalAlign.Center;

            Panel panel = new Panel();
            panel.Height = Unit.Percentage(100);
            panel.HorizontalAlign = HorizontalAlign.Center;
            panel.CssClass = "titulosReportes";

            ltr.Controls.Add(ltc1);
            ltr.Controls.Add(ltc2);
            ltbl.Controls.Add(ltr);
            panel.Controls.Add(ltbl);

            return panel;

        }

        /// <summary>
        /// Crea un indicador vertical que muestra el un número y una descripción.
        /// </summary>
        /// <param name="indicador">Debe ser un número</param>
        /// <param name="descripcion">Una cadena con la descripción</param>
        /// <returns>Regresa un panel con una tabla vertical con 2 filas, la primera con el valor de "indicador" y la segunda con el valor de "descripción"</returns>
        public static Control IndicadorVertical(string indicador, string descripcion)
        {
            Table ltbl = new Table();
            ltbl.Width = Unit.Percentage(100);
            ltbl.Height = Unit.Percentage(100);

            TableRow ltr1 = new TableRow();
            ltr1.Width = Unit.Percentage(100);
            ltr1.Height = Unit.Percentage(100);

            TableCell ltc1 = new TableCell();
            ltc1.Width = Unit.Percentage(100);
            ltc1.Height = Unit.Percentage(100);
            ltc1.Text = indicador;
            ltc1.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
            ltc1.Style.Add(HtmlTextWriterStyle.FontSize, "30px");
            ltc1.Style.Add(HtmlTextWriterStyle.TextAlign, "center");

            TableRow ltr2 = new TableRow();
            ltr1.Width = Unit.Percentage(100);
            ltr1.Height = Unit.Percentage(100);

            TableCell ltc2 = new TableCell();
            ltc2.Width = Unit.Percentage(70);
            ltc2.Height = Unit.Percentage(100);
            ltc2.Text = descripcion;
            ltc2.Style.Add(HtmlTextWriterStyle.FontSize, "16px");
            ltc2.Style.Add(HtmlTextWriterStyle.TextAlign, "center");

            ltbl.HorizontalAlign = HorizontalAlign.Center;

            Panel panel = new Panel();
            panel.Height = Unit.Percentage(100);
            panel.HorizontalAlign = HorizontalAlign.Center;
            panel.CssClass = "titulosReportes";

            ltr1.Controls.Add(ltc1);
            ltr2.Controls.Add(ltc2);
            ltbl.Controls.Add(ltr1);
            ltbl.Controls.Add(ltr2);
            panel.Controls.Add(ltbl);

            return panel;

        }

        /// <summary>
        /// Regresa un DataTable con los resultados de la consulta de SQL que recibe como parametro de entrada en forma de string.
        /// </summary>
        /// <param name="querySQL">Query en formato string</param>
        /// <returns></returns>
        public static DataTable DataTable(string querySQL)
        {
            DataTable localDT = new DataTable(null);

            try
            {
                localDT.Clear();
                localDT = DSODataAccess.Execute(querySQL);
            }

            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error en metodo creaDataTable() en KeytiaWebUserInterface.DashboardLT.DashboardLT.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return localDT;
        }

        /// <summary>
        /// Agrega un titulo con estilo de los reportes de dashboard de keytia y un pequeño borde para seccionar cada reporte.
        /// </summary>
        /// <param name="control">Control al que se desea agregar el titulo y borde</param>
        /// <param name="titulo">Titulo que se mostrara al usuario</param>
        /// <param name="height">Si el height es 0 se asigna propiedad height a "auto"</param>
        /// <param name="tituloReporte">Si el tituloReporte.Lenght es "0", no se agrega el titulo del reporte</param>
        /// <returns>Regresa un control tipo panel que contiene el control que recibe como parametro</returns>
        public static Control tituloYBordesReporte(Control control, string tituloReporte, int height)
        {
            Panel panel = new Panel();
            panel.HorizontalAlign = HorizontalAlign.Center;
            if (height > 0)
            {
                panel.CssClass = "PanelTitulosYBordeReportes maxWidth100Perc";
                panel.Height = height;
            }
            else
            {
                panel.CssClass = "PanelTitulosYBordeReportes AutoHeight maxWidth100Perc";
            }

            Panel header = new Panel();
            header.CssClass = "titulosReportes";
            header.HorizontalAlign = HorizontalAlign.Left;

            header.Height = 5;

            //Label lLabel = new Label();
            //lLabel.Text = tituloHeader;
            //header.Controls.Add(lLabel);

            if (tituloReporte.Length > 0)
            {
                ////20141003 AM. Se cambia DropDownList por Label 
                ////DropDownList ddlTitulo = new DropDownList();
                ////ddlTitulo.Items.Add(tituloReporte);
                ////ddlTitulo.Enabled = false;
                ////ddlTitulo.Width = Unit.Percentage(100);

                Table tblTituloReporte = new Table();
                tblTituloReporte.Width = Unit.Percentage(100);
                TableRow tblTituloReporteTr1 = new TableRow();
                TableCell tblTituloReporteTc1 = new TableCell();
                tblTituloReporteTc1.HorizontalAlign = HorizontalAlign.Left;
                tblTituloReporteTc1.Height = Unit.Pixel(29);

                Label lblTitulo = new Label();
                lblTitulo.Text = tituloReporte;
                lblTitulo.CssClass = "tituloHeaderReportes";
                lblTitulo.Width = Unit.Percentage(100);

                tblTituloReporteTc1.Controls.Add(lblTitulo);
                tblTituloReporteTr1.Controls.Add(tblTituloReporteTc1);
                tblTituloReporte.Controls.Add(tblTituloReporteTr1);

                panel.Controls.Add(tblTituloReporte);
            }

            panel.Controls.Add(header);
            panel.Controls.Add(control);

            return panel;
        }

        /// <summary>
        /// Agrega un titulo con estilo de los reportes de dashboard de keytia y un pequeño borde para seccionar cada reporte.
        /// </summary>
        /// <param name="control">Control al que se desea agregar el titulo y borde</param>
        /// <param name="titulo">Titulo que se mostrara al usuario</param>
        /// <param name="height">Si el height es 0 se asigna propiedad height a "auto"</param>
        /// <param name="tituloReporte">Si el tituloReporte.Lenght es "0", no se agrega el titulo del reporte</param>
        /// <returns>Regresa un control tipo panel que contiene el control que recibe como parametro</returns>
        public static Control tituloYBordesReporte(Control control, string tituloHeader, string tituloReporte, int height)
        {
            Panel panel = new Panel();
            panel.HorizontalAlign = HorizontalAlign.Center;
            if (height > 0)
            {
                panel.CssClass = "PanelTitulosYBordeReportes maxWidth100Perc";
                panel.Height = height;
            }
            else
            {
                panel.CssClass = "PanelTitulosYBordeReportes AutoHeight maxWidth100Perc";
            }

            Panel header = new Panel();
            header.CssClass = "titulosReportes";
            header.HorizontalAlign = HorizontalAlign.Left;

            header.Height = 5;

            //Label lLabel = new Label();
            //lLabel.Text = tituloHeader;
            //header.Controls.Add(lLabel);

            if (tituloReporte.Length > 0)
            {
                ////20141003 AM. Se cambia DropDownList por Label 
                ////DropDownList ddlTitulo = new DropDownList();
                ////ddlTitulo.Items.Add(tituloReporte);
                ////ddlTitulo.Enabled = false;
                ////ddlTitulo.Width = Unit.Percentage(100);

                Table tblTituloReporte = new Table();
                tblTituloReporte.Width = Unit.Percentage(100);
                TableRow tblTituloReporteTr1 = new TableRow();
                TableCell tblTituloReporteTc1 = new TableCell();
                tblTituloReporteTc1.HorizontalAlign = HorizontalAlign.Left;
                tblTituloReporteTc1.Height = Unit.Pixel(29);

                Label lblTitulo = new Label();
                lblTitulo.Text = tituloReporte;
                lblTitulo.CssClass = "tituloHeaderReportes";
                lblTitulo.Width = Unit.Percentage(100);

                tblTituloReporteTc1.Controls.Add(lblTitulo);
                tblTituloReporteTr1.Controls.Add(tblTituloReporteTc1);
                tblTituloReporte.Controls.Add(tblTituloReporteTr1);

                panel.Controls.Add(tblTituloReporte);
            }

            panel.Controls.Add(header);
            panel.Controls.Add(control);

            return panel;
        }

        /// <summary>
        /// Agrega un titulo con estilo de los reportes de dashboard de keytia y un pequeño borde para seccionar cada reporte.
        /// </summary>
        /// <param name="control">Control al que se desea agregar el titulo y borde</param>
        /// <param name="tituloHeader">Ya no se usa (Puede ser "")</param>
        /// <param name="tituloReporte">Titulo que se muestra al usuario</param>
        /// <param name="height">Altura del contenedor del reporte</param>
        /// <param name="urlNavegacion">URL al que manda al dar clic en la imagen de lupa</param>
        /// <returns>Regresa un objeto tipo control</returns>
        public static Control tituloYBordesReporte(Control control, string tituloHeader, string tituloReporte, int height, string urlNavegacion)
        {
            Panel panel = new Panel();
            panel.HorizontalAlign = HorizontalAlign.Center;
            if (height > 0)
            {
                panel.CssClass = "PanelTitulosYBordeReportes maxWidth100Perc";
                panel.Height = height;
            }
            else
            {
                panel.CssClass = "PanelTitulosYBordeReportes AutoHeight maxWidth100Perc";
            }

            Panel header = new Panel();
            header.CssClass = "titulosReportes";
            header.HorizontalAlign = HorizontalAlign.Left;

            header.Height = 5;

            //Label lLabel = new Label();
            //lLabel.Text = tituloHeader;
            //header.Controls.Add(lLabel);

            Table tblTituloReporte = new Table();
            tblTituloReporte.Width = Unit.Percentage(100);
            TableRow tblTituloReporteTr1 = new TableRow();
            TableCell tblTituloReporteTc1 = new TableCell();
            tblTituloReporteTc1.Width = Unit.Percentage(95);
            tblTituloReporteTc1.HorizontalAlign = HorizontalAlign.Left;
            TableCell tblTituloReporteTc2 = new TableCell();
            tblTituloReporteTc2.Width = Unit.Percentage(5);
            tblTituloReporteTc2.HorizontalAlign = HorizontalAlign.Center;

            //20141003 AM. Se cambia DropDownList por Label 
            //DropDownList ddlTitulo = new DropDownList();
            //ddlTitulo.Items.Add(tituloReporte);
            //ddlTitulo.Enabled = false;
            //ddlTitulo.Width = Unit.Percentage(100);

            //10-04-2015 Cambio: Nelly Zuñiga
            //Se comento el codigo donde se agregaba el Label para el titulo, por que se hizo cambio por un HyperLink
            //Label lblTitulo = new Label();
            //lblTitulo.Text = tituloReporte;
            //lblTitulo.CssClass = "tituloHeaderReportes";
            //lblTitulo.Width = Unit.Percentage(100);

            HyperLink hplTitulo = new HyperLink();
            hplTitulo.Text = tituloReporte;
            hplTitulo.CssClass = "tituloHeaderReportes";
            hplTitulo.Width = Unit.Percentage(100);
            hplTitulo.NavigateUrl = urlNavegacion;
            tblTituloReporteTc1.Controls.Add(hplTitulo);

            //NZ 20161018 Se cambia para que no sea un boton si no un HyperLink, para evitar que el momento del postback tenga que cargar todo el Dashboard.
            //Button btnNav = new Button();
            ////20140926 AM. Se cambia PostBackUrl por un event handler devido a error :
            ////"An error has occurred because a control with id 'ctl00$cphContent$ctl00' could not be located or a different control is assigned to the same ID after postback. If the ID is not assigned, explicitly set the ID property of controls that raise postback events to avoid this error."
            ////btnNav.PostBackUrl = urlNavegacion;
            //btnNav.Click += (sender, e) => btnNav_Click(sender, e, urlNavegacion);

            ////20140926 AM. Se comenta la siguiente linea para evitar error de ID duplicado (Multiple controls with the same ID 'XXX' were found. Trace requires that controls have unique IDs.)
            ////btnNav.ID = "btnNav" + DateTime.Now.Ticks.ToString();
            //btnNav.Width = 28;
            //btnNav.Height = 28;
            //hplTitulo.Width = Unit.Percentage(100);
            //btnNav.CssClass = "ui-button-icon-primary ui-icon custom-icon-search ui-widget ui-state-default ui-corner-all AutoHeight noRepeat";
            //tblTituloReporteTc2.Controls.Add(btnNav);

            HyperLink hplLupa = new HyperLink();
            hplLupa.Text = string.Empty;
            hplLupa.CssClass = "ui-button-icon-primary ui-icon custom-icon-search ui-widget ui-state-default ui-corner-all AutoHeight noRepeat";
            hplLupa.Width = 28;
            hplLupa.Height = 28;
            hplLupa.NavigateUrl = urlNavegacion;
            tblTituloReporteTc2.Controls.Add(hplLupa);

            tblTituloReporteTr1.Controls.Add(tblTituloReporteTc1);
            tblTituloReporteTr1.Controls.Add(tblTituloReporteTc2);
            tblTituloReporte.Controls.Add(tblTituloReporteTr1);

            panel.Controls.Add(tblTituloReporte);
            panel.Controls.Add(header);
            panel.Controls.Add(control);

            return panel;
        }

        protected static void btnNav_Click(object sender, EventArgs e, string url)
        {
            HttpContext.Current.Response.Redirect(url);
        }

        /// <summary>
        /// Agrega un pequeño borde alrededor del control que recibe como parametro.
        /// </summary>
        /// <param name="control">Control al que se desea agregar el titulo y borde</param>
        /// <param name="height">Si el height es 0 se asigna propiedad height a "auto"</param>
        /// <returns>Regresa un control tipo panel que contiene el control que recibe como parametro</returns>
        public static Control BordesReporte(Control control, int height)
        {
            Table ltbl = new Table();
            ltbl.Width = Unit.Percentage(100);
            ltbl.Height = Unit.Percentage(100);
            TableRow ltr = new TableRow();
            ltr.Width = Unit.Percentage(100);
            ltr.Height = Unit.Percentage(100);
            TableCell ltc = new TableCell();
            ltc.Width = Unit.Percentage(100);
            ltc.Height = Unit.Percentage(100);
            ltbl.HorizontalAlign = HorizontalAlign.Center;

            Panel panel = new Panel();
            panel.Height = Unit.Percentage(100);
            panel.HorizontalAlign = HorizontalAlign.Center;
            if (height > 0)
            {
                panel.CssClass = "PanelTitulosYBordeReportes";
                panel.Height = height;
            }
            else
            {
                panel.CssClass = "PanelTitulosYBordeReportes AutoHeight";
            }

            ltc.Controls.Add(control);
            ltr.Controls.Add(ltc);
            ltbl.Controls.Add(ltr);
            panel.Controls.Add(ltbl);

            return panel;
        }

        /// <summary>
        /// Regresa un panel con la leyenda "No existen datos de este reporte para el rango de fechas seleccionadas"
        /// </summary>
        /// <returns></returns>
        public static Control lblReporteSinDatos()
        {
            Panel lpanel = new Panel();
            lpanel.HorizontalAlign = HorizontalAlign.Center;

            Label reporteSinDatos = new Label();
            reporteSinDatos.Text = "No existen datos de este reporte para el rango de fechas seleccionadas";

            lpanel.Controls.Add(reporteSinDatos);

            return lpanel;
        }

        public static GridView RepSinDatos()
        {
            GridView grid = new GridView() { EmptyDataText = "No existen datos de este reporte para el rango de fechas seleccionadas" };
            grid.DataSource = new DataTable();
            grid.DataBind();
            return grid;
        }

        /// <summary>
        /// Ordena la tabla mandando de parametro el ordenamiento deseado.
        /// </summary>
        /// <param name="dataTable">Tabla que se desea ordenar</param>
        /// <param name="sort">Orden de los campos Ej. "[ID] Desc"   Ej2. "[ID] Asc, [Empleado] Desc"  </param>
        /// <returns>Regresa la tabla ordenada</returns>
        public static DataTable ordenaTabla(DataTable dataTable, string sort)
        {
            try
            {
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    dataTable.DefaultView.Sort = sort;
                    dataTable = dataTable.DefaultView.ToTable();
                }
                
                return dataTable;
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("No fue posible ordenar la tabla.", ex);
                return dataTable;
            }
        }

        /// <summary>
        /// Selecciona el Top N del DataTable que recibe como parametro.
        /// </summary>
        /// <param name="dataTable">Tabla de la que se quiere adquirir el Top N</param>
        /// <param name="sort">Orden de los campos Ej. "[ID] Desc"   Ej2. "[ID] Asc, [Empleado] Desc"  </param>
        /// <param name="numeroLineas">Numero de lineas que se desea tomar</param>
        /// <returns>Regresa el numero de lineas deseada</returns>
        public static DataTable selectTopNTabla(DataTable dataTable, string sort, int numeroLineas)
        {
            try
            {
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    dataTable.DefaultView.Sort = sort;
                    dataTable = dataTable.DefaultView.ToTable().AsEnumerable().Take(numeroLineas).CopyToDataTable();
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("No fue posible seleccionar el Top 10 de la tabla.", ex);
                return dataTable;
            }
        }

        public static DataTable agregaTotales(DataTable ldt, int IndexCeldaTotales, string textoEnCeldaTotales)
        {
            try
            {
                DataRow dr = ldt.NewRow();

                dr[IndexCeldaTotales] = textoEnCeldaTotales;

                for (int ent = 0; ent < ldt.Columns.Count; ent++)
                {
                    if (ldt.Columns[ent].DataType != System.Type.GetType("System.String"))
                    {
                        dr[ldt.Columns[ent].ColumnName] = ldt.Compute("Sum([" + ldt.Columns[ent].ColumnName + "])", "");
                    }
                    else
                    {
                        if (ldt.Columns[ent].DataType != System.Type.GetType("System.String"))
                        {
                            dr[ent] = 0;
                        }
                        else if (ent != IndexCeldaTotales)
                        {
                            dr[ent] = "";
                        }
                    }
                }
                ldt.Rows.Add(dr);
                ldt.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ldt;
        }


        //NZ 20150331 Para que se respete el formarto de la columna que sea Link
        public static Control GridViewConFormatosEnLink(string gridID, DataTable dataSource, bool agregaTotales, string tituloCeldaTotales, bool HoriAndVertScrollBar, int alturaContenedorGridView,
                                                string[] formatStringColumnas, string dataNavigateUrlFormatString, string[] urlFields, int IndexCeldaTotales,
                                                int[] indexColumnasDatosNavegacion, int[] indexColumnasBoundField, int[] indexColumnasHyperLinkField, int numDecimales = 2)
        {
            #region Se crea contenedor de gridview
            Panel lpnl = new Panel();
            lpnl.Width = Unit.Percentage(100);
            lpnl.CssClass = "maxWidth100Perc";

            if (HoriAndVertScrollBar)
            {
                lpnl.ScrollBars = ScrollBars.Vertical;
                lpnl.ScrollBars = ScrollBars.Horizontal;
            }

            if (alturaContenedorGridView > 0)
            {
                lpnl.Height = 400;
            }

            Table ltbl = new Table();
            ltbl.Width = Unit.Percentage(100);
            TableRow ltr = new TableRow();
            ltr.Width = Unit.Percentage(100);
            TableCell ltc = new TableCell();
            ltc.Width = Unit.Percentage(100);

            Panel lpnl1 = new Panel();
            lpnl1.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix invisible";
            //lpnl1.Height = 10; // NZ Esto se paso a la hoja de estilos.
            lpnl1.Width = Unit.Percentage(100);

            Panel lpnl2 = new Panel();
            lpnl2.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix invisible";
            //lpnl2.Height = 10;
            lpnl2.Width = Unit.Percentage(100);

            ltbl.HorizontalAlign = HorizontalAlign.Center;
            #endregion

            #region Proceso de creacion del gridview
            if (dataSource.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (dataSource.Columns.Contains("RID"))
                        dataSource.Columns.Remove("RID");
                    if (dataSource.Columns.Contains("RowNumber"))
                        dataSource.Columns.Remove("RowNumber");
                    if (dataSource.Columns.Contains("TopRID"))
                        dataSource.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.ID = gridID;
                    lgrv.CssClass = "DSOGrid";
                    lgrv.Height = Unit.Percentage(100);
                    lgrv.Width = Unit.Percentage(100);

                    lgrv.RowStyle.CssClass = "grvitemStyle";
                    lgrv.HeaderStyle.CssClass = "titulosReportes";
                    lgrv.AlternatingRowStyle.CssClass = "grvalternateItemStyle";

                    #region Agrega una linea al DataTable para calcular los totales
                    if (agregaTotales == true)
                    {
                        DataRow dr = dataSource.NewRow();

                        dr[IndexCeldaTotales] = tituloCeldaTotales;

                        for (int ent = 0; ent < dataSource.Columns.Count; ent++)
                        {
                            if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String") && !indexColumnasDatosNavegacion.Contains(ent))
                            {
                                dr[dataSource.Columns[ent].ColumnName] = dataSource.Compute("Sum([" + dataSource.Columns[ent].ColumnName + "])", "");

                                if (dr[dataSource.Columns[ent].ColumnName] != DBNull.Value)
                                    dr[dataSource.Columns[ent].ColumnName] = Math.Round(Convert.ToDecimal(dr[dataSource.Columns[ent].ColumnName]), numDecimales);
                            }
                            else
                            {
                                if (dataSource.Columns[ent].DataType != System.Type.GetType("System.String"))
                                {
                                    dr[ent] = 0;
                                }
                                else if (ent != IndexCeldaTotales)
                                {
                                    dr[ent] = "";
                                }
                            }
                        }

                        dataSource.Rows.Add(dr);
                        dataSource.AcceptChanges();
                    }

                    #endregion // Agrega una linea al DataTable para calcular los totales

                    #region Agrega un tipo de columna por cada columna del datarow

                    int i = 0;
                    foreach (DataColumn dc in dataSource.Columns)
                    {
                        BoundField bf = new BoundField();
                        bf.DataField = dc.ColumnName;
                        bf.HeaderText = dc.ColumnName;

                        HyperLinkField hlf = new HyperLinkField();
                        hlf.HeaderText = dc.ColumnName;

                        if (indexColumnasBoundField.Contains(i))
                        {
                            bf.DataFormatString = formatStringColumnas[i]; //"{0:c}";

                            //Si es string lo alineo a la izquierda
                            if (dc.DataType.FullName == "System.String")
                            {
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            }
                            else
                            {
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                            }
                            lgrv.Columns.Add(bf);
                        }
                        else if (indexColumnasDatosNavegacion.Contains(i))  // Columna de iCodCatalogo
                        {
                            bf.DataField = dataSource.Columns[i].ColumnName;
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            bf.Visible = false;
                            lgrv.Columns.Add(bf);
                        }
                        else if (indexColumnasHyperLinkField.Contains(i))
                        {
                            hlf.DataTextFormatString = formatStringColumnas[i];

                            //Si es string lo alineo a la izquierda
                            if (dc.DataType.FullName == "System.String") { hlf.ItemStyle.HorizontalAlign = HorizontalAlign.Left; }
                            else { hlf.ItemStyle.HorizontalAlign = HorizontalAlign.Right; }

                            hlf.HeaderText = dataSource.Columns[i].ColumnName;
                            hlf.DataNavigateUrlFields = urlFields;
                            hlf.DataNavigateUrlFormatString = dataNavigateUrlFormatString;
                            hlf.DataTextField = dataSource.Columns[i].ColumnName;

                            hlf.ControlStyle.Font.Bold = true;
                            lgrv.Columns.Add(hlf);
                        }

                        i++;
                    }

                    #endregion // Agrega un BoundField por cada columna del datarow

                    lgrv.AutoGenerateColumns = false;
                    lgrv.DataSource = dataSource;
                    lgrv.DataBind();

                    //Se vuelve a validar si se agregaron totales para agregar estilo a ultima fila pero despues de hacer el databind()
                    if (agregaTotales == true)
                    {
                        // Le da formato a la ultima fila del gridview para darle estilo keytia
                        lgrv.Rows[dataSource.Rows.Count - 1].CssClass = "titulosReportes totalesBnrt";
                    }

                    ltc.Controls.Add(lpnl1);
                    ltc.Controls.Add(lgrv);
                    ltc.Controls.Add(lpnl2);
                    ltr.Controls.Add(ltc);
                    ltbl.Controls.Add(ltr);

                    lpnl.Controls.Add(ltbl);

                    return lpnl;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT." + DTIChartsAndControls.NombreClase + " '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }


        //NZ 20160229 Se creo para Dashboard SIANA.
        public static EventHandler Obtener(string metodo, object target)
        {
            EventHandler handler = null;
            handler = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), target, metodo);
            return handler;
        }

        #endregion



        //NZ 20180622 Nueva forma de armar los reporte : Ademas se modificaron los metodos originales que construyen los gridview para la tabla.

        public static Dictionary<string, string> GetListaPestañasGenericas(FCGpoGraf tiposGraficas)
        {
            Dictionary<string, string> pestañas = new Dictionary<string, string>();

            switch (tiposGraficas)
            {
                case FCGpoGraf.Tabular:
                    pestañas.Add("line", "Línea");
                    pestañas.Add("column2d", "Columnas");
                    pestañas.Add("bar2d", "Barras");
                    pestañas.Add("area2d", "Área");
                    pestañas.Add("doughnut2d", "Circular");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.TabularLiTa:
                    pestañas.Add("line", "Línea");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.TabularBaTa:
                    pestañas.Add("bar2d", "Barras");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.TabularLiBaCoTa:
                    pestañas.Add("line", "Línea");
                    pestañas.Add("bar2d", "Barras");
                    pestañas.Add("column2d", "Columnas");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.TabularBaCoDoTa:
                    pestañas.Add("bar2d", "Barras");
                    pestañas.Add("column2d", "Columnas");
                    pestañas.Add("doughnut2d", "Circular");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.Matricial:
                    pestañas.Add("msline", "Línea");
                    pestañas.Add("mscolumn2d", "Columnas");
                    pestañas.Add("msbar2d", "Barras");
                    pestañas.Add("msarea", "Área");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.MatricialConStack:
                    pestañas.Add("msline", "Línea");
                    pestañas.Add("mscolumn2d", "Columnas");
                    pestañas.Add("stackedbar2d", "Stack Barras");
                    pestañas.Add("stackedcolumn2d", "Stack Columnas");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.MatricialConStack2:
                    pestañas.Add("msline", "Línea");
                    pestañas.Add("mscolumn2d", "Columnas");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.MatricialConStackLineaBase:
                    pestañas.Add("msstackedcolumn2dlinedy", "Stack Columnas Linea Base");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.MatricialLiCoBaTa:
                    pestañas.Add("msline", "Línea");
                    pestañas.Add("mscolumn2d", "Columnas");
                    pestañas.Add("msbar2d", "Barras");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.MatricialConStackCoBaDoTa:
                    pestañas.Add("mscolumn2d", "Columnas");
                    pestañas.Add("stackedbar2d", "Stack Barras");
                    pestañas.Add("stackedcolumn2d", "Stack Columnas");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.TabularCoBaTa:
                    pestañas.Add("column2d", "Columnas");
                    pestañas.Add("bar2d", "Barras");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.MatricialCoBaTa:
                    pestañas.Add("mscolumn2d", "Columnas");
                    pestañas.Add("msbar2d", "Barras");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.TabularCiBaTa:
                    pestañas.Add("doughnut2d", "Circular");
                    pestañas.Add("bar2d", "Barras");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.MatricialTa:
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.ColsCombinada:
                    pestañas.Add("mscombidy2d", "Combinada");
                    pestañas.Add("tabla", "Tabla");
                    break;
                case FCGpoGraf.Doughnut2d:
                    pestañas.Add("tabla", "Tabla");
                    break;
                default:
                    break;
            }

            return pestañas;
        }

        public static Control MapaNavegacion(List<MapNav> navegaciones)
        {
            HtmlContainerControl nav = new HtmlGenericControl("NAV");
            nav.Attributes.Add("aria-label", "breadcrumb");

            HtmlContainerControl ol = new HtmlGenericControl("OL");
            ol.Attributes.Add("class", "breadcrumb breadcrumbKeytia");

            for (int i = 0; i < navegaciones.Count; i++)
            {
                HtmlContainerControl li = new HtmlGenericControl("LI");

                if (i == navegaciones.Count - 1)
                {
                    li.Attributes.Add("class", "breadcrumb-item active");
                    li.Attributes.Add("aria-current", "page");
                    Literal literal = new Literal();
                    literal.Text = navegaciones[i].Titulo.ToUpper();
                    li.Controls.Add(literal);
                }
                else
                {
                    li.Attributes.Add("class", "breadcrumb-item");

                    LinkButton link = new LinkButton();
                    link.PostBackUrl = navegaciones[i].URL;
                    link.Text = navegaciones[i].Titulo.ToUpper() + "&nbsp;";
                    li.Controls.Add(link);
                }

                ol.Controls.Add(li);
            }

            nav.Controls.Add(ol);
            return nav;
        }

        public static Control TituloYPestañasRep1Nvl(Control control, string idContenedor, string tituloReporte, string urlNavegacion, int pestañaActiva, FCGpoGraf tiposGraficas, string info = "none", bool headerFijo = true)
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            // Crear boton para informacion modificaciones Proyecto Bimbo
            // Se agrega argumento a metodo para modificar el titulo / tooltip
            HtmlContainerControl buttonInfo = new HtmlGenericControl("BUTTON");
            buttonInfo.Attributes.Add("class", "btn btn-light");
            buttonInfo.Attributes.Add("type", "button");
            buttonInfo.Attributes.Add("title", info);
            buttonInfo.Disabled = true;
            HtmlContainerControl i_iconBtnInfo = new HtmlGenericControl("I");
            i_iconBtnInfo.Attributes.Add("class", "fas fa-info-circle");
            i_iconBtnInfo.Style.Add("margin-right", "4px");
            buttonInfo.Controls.Add(i_iconBtnInfo);
            buttonInfo.Style.Add("cursor", "default");

            if (!(info == "none") && !(info == ""))
            {
                pnlAction.Controls.Add(buttonInfo);
            }

            // ------------------------------------------ 

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (!string.IsNullOrEmpty(urlNavegacion))
            {
                HyperLink link2Pnls = new HyperLink();
                link2Pnls.CssClass = "btn btn-primary";
                link2Pnls.NavigateUrl = urlNavegacion;
                link2Pnls.Attributes.Add("role", "button");

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                link2Pnls.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fas fa-list-ul");
                link2Pnls.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(link2Pnls);
            }

            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlPestañas = new Panel();
            pnlPestañas.CssClass = "tabbable tabbable-tabdrop";

            HtmlContainerControl ulPestañas = new HtmlGenericControl("UL");
            ulPestañas.Attributes.Add("class", "nav nav-pills");

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            var dic = GetListaPestañasGenericas(tiposGraficas);
            for (int i = 0; i < dic.Count; i++)
            {
                HtmlContainerControl li = new HtmlGenericControl("LI");
                HtmlContainerControl div = new HtmlGenericControl("DIV");

                if (i == pestañaActiva)
                {
                    li.Attributes.Add("class", "active");
                    div.Attributes.Add("class", "tab-pane active");
                }
                else { div.Attributes.Add("class", "tab-pane"); }

                //Pestaña Titulo
                HyperLink tab = new HyperLink();
                tab.NavigateUrl = "#tab" + idContenedor + "_" + dic.Keys.ElementAt(i);  ///                
                if (dic.Keys.ElementAt(i) != "tabla")
                {
                    tab.CssClass = idContenedor;
                    tab.Attributes.Add("attr", "graf_" + idContenedor + "_" + dic.Keys.ElementAt(i));  //
                    tab.Attributes.Add("attrTipo", dic.Keys.ElementAt(i));
                }
                tab.Text = dic.Values.ElementAt(i);
                tab.Attributes.Add("data-toggle", "tab");

                li.Controls.Add(tab);
                ulPestañas.Controls.Add(li);
                //


                div.Attributes.Add("id", "tab" + idContenedor + "_" + dic.Keys.ElementAt(i));   ///
                if (dic.Keys.ElementAt(i) != "tabla")
                {
                    HtmlContainerControl canvas = new HtmlGenericControl("DIV");
                    canvas.Attributes.Add("id", "graf_" + idContenedor + "_" + dic.Keys.ElementAt(i));   ///
                    div.Controls.Add(canvas);
                }
                else
                {
                    Panel tabla = new Panel();
                    tabla.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
                    tabla.Controls.Add(control);
                    div.Controls.Add(tabla);
                }


                pnlContent.Controls.Add(div);
            }
            pnlPestañas.Controls.Add(ulPestañas);
            pnlPestañas.Controls.Add(pnlContent);


            divSub.Controls.Add(pnlPestañas);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);


            return panel;
        }

        public static Control TituloYPestañasRepNNvlTabla(Control control, string idContenedor, string tituloReporte, string info = "none", string urlNavegacion = "", bool headerFijo = true)
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            // Crear boton para informacion modificaciones Proyecto Bimbo
            // Se agrega argumento a metodo para modificar el titulo / tooltip
            HtmlContainerControl buttonInfo = new HtmlGenericControl("BUTTON");
            buttonInfo.Attributes.Add("class", "btn btn-light");
            buttonInfo.Attributes.Add("type", "button");
            buttonInfo.Attributes.Add("title", info);
            buttonInfo.Disabled = true;
            HtmlContainerControl i_iconBtnInfo = new HtmlGenericControl("I");
            i_iconBtnInfo.Attributes.Add("class", "fas fa-info-circle");
            i_iconBtnInfo.Style.Add("margin-right", "4px");
            buttonInfo.Controls.Add(i_iconBtnInfo);
            buttonInfo.Style.Add("cursor", "default");

            if(!(info == "none") && !(info == ""))
            {
                pnlAction.Controls.Add(buttonInfo);
            }

            // ------------------------------------------ 

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (!string.IsNullOrEmpty(urlNavegacion))
            {
                HyperLink link2Pnls = new HyperLink();
                link2Pnls.CssClass = "btn btn-primary";
                link2Pnls.NavigateUrl = urlNavegacion;
                link2Pnls.Attributes.Add("role", "button");

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                link2Pnls.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fas fa-list-ul");
                link2Pnls.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(link2Pnls);
            }

            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlPestañas = new Panel();
            pnlPestañas.CssClass = "tabbable tabbable-tabdrop";

            HtmlContainerControl ulPestañas = new HtmlGenericControl("UL");
            ulPestañas.Attributes.Add("class", "nav nav-pills");

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            #region pestaña Tabla

            HtmlContainerControl li = new HtmlGenericControl("LI");
            HtmlContainerControl div = new HtmlGenericControl("DIV");

            li.Attributes.Add("class", "active");
            div.Attributes.Add("class", "tab-pane active");


            //Pestaña Titulo
            HyperLink tab = new HyperLink();
            tab.NavigateUrl = "#tab" + idContenedor + "_tabla";

            tab.Text = "Tabla";
            tab.Attributes.Add("data-toggle", "tab");

            li.Controls.Add(tab);
            ulPestañas.Controls.Add(li);
            //

            div.Attributes.Add("id", "tab" + idContenedor + "_tabla");

            Panel tabla = new Panel();
            tabla.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
            tabla.Controls.Add(control);
            div.Controls.Add(tabla);
            pnlContent.Controls.Add(div);

            #endregion Pestaña tabla

            pnlPestañas.Controls.Add(ulPestañas);
            pnlPestañas.Controls.Add(pnlContent);

            divSub.Controls.Add(pnlPestañas);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);


            return panel;
        }

        public static Control TituloYPestañasRepNNvlGraficas(string idContenedor, string tituloReporte, int pestañaActiva, FCGpoGraf tiposGraficas, string info = "none", string urlNavegacion = "")
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            // Crear boton para informacion modificaciones Proyecto Bimbo
            // Se agrega argumento a metodo para modificar el titulo / tooltip
            HtmlContainerControl buttonInfo = new HtmlGenericControl("BUTTON");
            buttonInfo.Attributes.Add("class", "btn btn-light");
            buttonInfo.Attributes.Add("type", "button");
            buttonInfo.Attributes.Add("title", info);
            buttonInfo.Disabled = true;
            HtmlContainerControl i_iconBtnInfo = new HtmlGenericControl("I");
            i_iconBtnInfo.Attributes.Add("class", "fas fa-info-circle");
            i_iconBtnInfo.Style.Add("margin-right", "4px");
            buttonInfo.Controls.Add(i_iconBtnInfo);
            buttonInfo.Style.Add("cursor", "default");

            if (!(info == "none") && !(info == ""))
            {
                pnlAction.Controls.Add(buttonInfo);
            }

            // ------------------------------------------ 

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (!string.IsNullOrEmpty(urlNavegacion))
            {
                HyperLink link2Pnls = new HyperLink();
                link2Pnls.CssClass = "btn btn-primary";
                link2Pnls.NavigateUrl = urlNavegacion;
                link2Pnls.Attributes.Add("role", "button");

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                link2Pnls.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fas fa-list-ul");
                link2Pnls.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(link2Pnls);
            }

            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlPestañas = new Panel();
            pnlPestañas.CssClass = "tabbable tabbable-tabdrop";

            HtmlContainerControl ulPestañas = new HtmlGenericControl("UL");
            ulPestañas.Attributes.Add("class", "nav nav-pills");

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            var dic = GetListaPestañasGenericas(tiposGraficas);
            dic.Remove("tabla");

            for (int i = 0; i < dic.Count; i++)
            {
                HtmlContainerControl li = new HtmlGenericControl("LI");
                HtmlContainerControl div = new HtmlGenericControl("DIV");

                if (i == pestañaActiva)
                {
                    li.Attributes.Add("class", "active");
                    div.Attributes.Add("class", "tab-pane active");
                }
                else { div.Attributes.Add("class", "tab-pane"); }

                //Pestaña Titulo
                HyperLink tab = new HyperLink();
                tab.NavigateUrl = "#tab" + idContenedor + "_" + dic.Keys.ElementAt(i);  ///
                tab.CssClass = idContenedor;
                tab.Attributes.Add("attr", "graf_" + idContenedor + "_" + dic.Keys.ElementAt(i));  //
                tab.Attributes.Add("attrTipo", dic.Keys.ElementAt(i));
                tab.Text = dic.Values.ElementAt(i);
                tab.Attributes.Add("data-toggle", "tab");

                li.Controls.Add(tab);
                ulPestañas.Controls.Add(li);
                //

                div.Attributes.Add("id", "tab" + idContenedor + "_" + dic.Keys.ElementAt(i));   ///
                HtmlContainerControl canvas = new HtmlGenericControl("DIV");
                canvas.Attributes.Add("id", "graf_" + idContenedor + "_" + dic.Keys.ElementAt(i));   ///
                div.Controls.Add(canvas);

                pnlContent.Controls.Add(div);
            }
            pnlPestañas.Controls.Add(ulPestañas);
            pnlPestañas.Controls.Add(pnlContent);

            divSub.Controls.Add(pnlPestañas);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);
            return panel;
        }

        public static Control TituloYPestañasRepDetalleSoloTabla(string idContenedor, string tituloReporte)
        {
            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);
            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlPestañas = new Panel();
            pnlPestañas.CssClass = "tabbable tabbable-tabdrop";

            HtmlContainerControl ulPestañas = new HtmlGenericControl("UL");
            ulPestañas.Attributes.Add("class", "nav nav-pills");

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            #region pestaña Tabla

            HtmlContainerControl li = new HtmlGenericControl("LI");
            HtmlContainerControl div = new HtmlGenericControl("DIV");

            li.Attributes.Add("class", "active");
            div.Attributes.Add("class", "tab-pane active");


            //Pestaña Titulo
            HyperLink tab = new HyperLink();
            tab.NavigateUrl = "#tab" + idContenedor + "_tabla";

            tab.Text = "Tabla";
            tab.Attributes.Add("data-toggle", "tab");

            li.Controls.Add(tab);
            ulPestañas.Controls.Add(li);
            //

            div.Attributes.Add("id", "tab" + idContenedor + "_tabla");

            //La declaración de la tabla es necesaria para que se pinten los datos popsteriormente
            HtmlContainerControl tabla = new HtmlGenericControl("TABLE");
            tabla.Attributes.Add("id", idContenedor);
            tabla.Attributes.Add("class", "table tableDetail");
            tabla.Attributes.Add("width", "100%");

            div.Controls.Add(tabla);
            pnlContent.Controls.Add(div);

            #endregion Pestaña tabla

            pnlPestañas.Controls.Add(ulPestañas);
            pnlPestañas.Controls.Add(pnlContent);

            divSub.Controls.Add(pnlPestañas);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);

            return panel;
        }

        public static Control TituloYBordeRepSencilloGrafica(string idContenedor, string tituloReporte, string urlNavegacion = "")
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (!string.IsNullOrEmpty(urlNavegacion))
            {
                HyperLink link2Pnls = new HyperLink();
                link2Pnls.CssClass = "btn btn-primary";
                link2Pnls.NavigateUrl = urlNavegacion;
                link2Pnls.Attributes.Add("role", "button");

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                link2Pnls.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fas fa-list-ul");
                link2Pnls.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(link2Pnls);
            }

            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            //Collapse
            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlContenedor = new Panel();
            pnlContenedor.CssClass = "tabbable tabbable-tabdrop";

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            #region Contenedor

            HtmlContainerControl div = new HtmlGenericControl("DIV");
            div.Attributes.Add("class", "tab-pane active");

            HtmlContainerControl canvas = new HtmlGenericControl("DIV");
            canvas.Attributes.Add("id", idContenedor);
            div.Controls.Add(canvas);

            pnlContent.Controls.Add(div);

            #endregion Contenedor

            pnlContenedor.Controls.Add(pnlContent);
            divSub.Controls.Add(pnlContenedor);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);


            return panel;
        }

        public static Control TituloYBordeRepSencilloTabla(Control control, string idContenedor, string tituloReporte, string urlNavegacion = "", bool headerFijo = true)
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (!string.IsNullOrEmpty(urlNavegacion))
            {
                HyperLink link2Pnls = new HyperLink();
                link2Pnls.CssClass = "btn btn-primary";
                link2Pnls.NavigateUrl = urlNavegacion;
                link2Pnls.Attributes.Add("role", "button");

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                link2Pnls.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fas fa-list-ul");
                link2Pnls.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(link2Pnls);
            }

            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            //Collapse
            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlContenedor = new Panel();
            pnlContenedor.CssClass = "tabbable tabbable-tabdrop";

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            #region Contenedor

            HtmlContainerControl div = new HtmlGenericControl("DIV");
            div.Attributes.Add("class", "tab-pane active");
            div.Attributes.Add("id", idContenedor);

            Panel tabla = new Panel();
            tabla.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
            tabla.Controls.Add(control);
            div.Controls.Add(tabla);

            pnlContent.Controls.Add(div);

            #endregion Contenedor

            pnlContenedor.Controls.Add(pnlContent);
            divSub.Controls.Add(pnlContenedor);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);

            return panel;
        }

        public static Control TituloYBordeRepSencilloTabla(Control control, string idContenedor, string tituloReporte,
             bool iconExporta = false, string nameMetodoEventHandler = "", string nameControl = "", object target = null, bool headerFijo = true)
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (iconExporta)
            {
                LinkButton exportaExcel = new LinkButton();
                exportaExcel.ID = nameControl;
                exportaExcel.Click += DTIChartsAndControls.Obtener(nameMetodoEventHandler, target);

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                exportaExcel.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fa fa-file-excel excel");
                exportaExcel.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(exportaExcel);
            }

            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            //Collapse
            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlContenedor = new Panel();
            pnlContenedor.CssClass = "tabbable tabbable-tabdrop";

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            #region Contenedor

            HtmlContainerControl div = new HtmlGenericControl("DIV");
            div.Attributes.Add("class", "tab-pane active");
            div.Attributes.Add("id", idContenedor);

            Panel tabla = new Panel();
            tabla.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
            tabla.Controls.Add(control);
            div.Controls.Add(tabla);

            pnlContent.Controls.Add(div);

            #endregion Contenedor

            pnlContenedor.Controls.Add(pnlContent);
            divSub.Controls.Add(pnlContenedor);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);

            return panel;
        }

        public static Control TituloYPestañasRepTablasTIM(Control[] controls, string[] titulosPestañas, string idContenedor, string tituloReporte, int pestañaActiva,
            bool iconExporta = false, string nameMetodoEventHandler = "", string nameControl = "", object target = null, bool headerFijo = true)
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (iconExporta)
            {
                LinkButton exportaExcel = new LinkButton();
                exportaExcel.ID = nameControl;
                exportaExcel.Click += DTIChartsAndControls.Obtener(nameMetodoEventHandler, target);

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                exportaExcel.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fa fa-file-excel excel");
                exportaExcel.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(exportaExcel);
            }

            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlPestañas = new Panel();
            pnlPestañas.CssClass = "tabbable tabbable-tabdrop";

            HtmlContainerControl ulPestañas = new HtmlGenericControl("UL");
            ulPestañas.Attributes.Add("class", "nav nav-pills");

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            var dic = titulosPestañas;
            for (int i = 0; i < dic.Length; i++)
            {
                HtmlContainerControl li = new HtmlGenericControl("LI");
                HtmlContainerControl div = new HtmlGenericControl("DIV");

                if (i == pestañaActiva)
                {
                    li.Attributes.Add("class", "active");
                    div.Attributes.Add("class", "tab-pane active");
                }
                else { div.Attributes.Add("class", "tab-pane"); }

                //Pestaña Titulo
                HyperLink tab = new HyperLink();
                tab.NavigateUrl = "#tab" + idContenedor + "_" + i.ToString();  ///           
                tab.Text = dic[i];
                tab.Attributes.Add("data-toggle", "tab");
                li.Controls.Add(tab);
                ulPestañas.Controls.Add(li);
                //

                div.Attributes.Add("id", "tab" + idContenedor + "_" + i.ToString());   ///

                if (controls[i] == null)
                {
                    HtmlContainerControl canvas = new HtmlGenericControl("DIV");
                    canvas.Attributes.Add("id", "graf_" + idContenedor + "_" + i.ToString());   ///
                    div.Controls.Add(canvas);
                }
                else
                {
                    Panel tabla = new Panel();
                    tabla.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
                    tabla.Controls.Add(controls[i]);
                    div.Controls.Add(tabla);
                }

                pnlContent.Controls.Add(div);
            }
            pnlPestañas.Controls.Add(ulPestañas);
            pnlPestañas.Controls.Add(pnlContent);

            divSub.Controls.Add(pnlPestañas);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);

            return panel;
        }

        public static Control TituloY2RowsTablaGrafica(Control controls, string idContenedor, string tituloReporte,string contenedorGrafica, bool headerFijo = true)
        {
            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);
            if(tituloReporte != "")
            {
                Label lblTitulo = new Label();
                lblTitulo.CssClass = "caption-subject titlePortletKeytia";
                lblTitulo.Text = tituloReporte;
                pnlCaption.Controls.Add(lblTitulo);
            }


            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);


            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlPestañas = new Panel();
            pnlPestañas.CssClass = "tabbable tabbable-tabdrop";

            //HtmlContainerControl ulPestañas = new HtmlGenericControl("UL");
            //ulPestañas.Attributes.Add("class", "nav nav-pills");

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";



            HtmlContainerControl li = new HtmlGenericControl("LI");
            HtmlContainerControl div = new HtmlGenericControl("DIV");
            HtmlContainerControl div1 = new HtmlGenericControl("DIV");
            HtmlContainerControl div2 = new HtmlGenericControl("DIV");
            HtmlContainerControl div3 = new HtmlGenericControl("DIV");
            HtmlContainerControl div4 = new HtmlGenericControl("DIV");

            li.Attributes.Add("class", "active");
            div.Attributes.Add("class", "tab-pane active");
            div1.Attributes.Add("class","row");
            div3.Attributes.Add("class","col-sm-12");

            div4.Attributes.Add("class", "col-sm-12");

            div2.Attributes.Add("class", "row");

            div.Attributes.Add("id", "tab" + idContenedor);   ///

            if(contenedorGrafica != "")
            {
                /*GRAFICA*/
                HtmlContainerControl canvas = new HtmlGenericControl("DIV");
                canvas.Attributes.Add("id", contenedorGrafica);   ///
                div3.Controls.Add(canvas);
                div1.Controls.Add(div3);
                div.Controls.Add(div1);
            }

            /*TABLA*/
            //Panel tabla1 = new Panel();
            //tabla1.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
            //tabla1.Controls.Add(controls);
            //div2.Controls.Add(tabla1);
            //div.Controls.Add(div2);

            Panel tabla1 = new Panel();
            tabla1.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
            tabla1.Controls.Add(controls);
            div2.Controls.Add(tabla1);
            div4.Controls.Add(div2);
            div.Controls.Add(div4);


            pnlContent.Controls.Add(div);

            //pnlPestañas.Controls.Add(ulPestañas);
            pnlPestañas.Controls.Add(pnlContent);

            divSub.Controls.Add(pnlPestañas);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);

            return panel;
        }

        public static Control TituloYPestañas2RowsTablaGrafica(Control[] controls, string[] titulosPestañas, string idContenedor, string tituloReporte, int pestañaActiva,
    bool iconExporta = false, string nameMetodoEventHandler = "", string nameControl = "", object target = null, bool headerFijo = true)
        {

            Panel panel = new Panel();
            panel.CssClass = "portlet solid bordered";

            Panel pnlTitulo = new Panel();
            pnlTitulo.CssClass = "portlet-title";

            #region Controles dentro de pnlTitulo

            Panel pnlCaption = new Panel();
            pnlCaption.CssClass = "caption";

            HtmlContainerControl i_bar = new HtmlGenericControl("I");
            i_bar.Attributes.Add("class", "icon-bar-chart font-dark hide");
            pnlCaption.Controls.Add(i_bar);

            Label lblTitulo = new Label();
            lblTitulo.CssClass = "caption-subject titlePortletKeytia";
            lblTitulo.Text = tituloReporte;
            pnlCaption.Controls.Add(lblTitulo);

            pnlTitulo.Controls.Add(pnlCaption);
            //------------------------------------------

            Panel pnlAction = new Panel();
            pnlAction.CssClass = "actions";

            HtmlContainerControl button = new HtmlGenericControl("BUTTON");
            button.Attributes.Add("class", "btn btn-light");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-toggle", "collapse");
            button.Attributes.Add("data-target", "#" + idContenedor + "Collapse");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", idContenedor + "Collapse");

            HtmlContainerControl i_iconBtn = new HtmlGenericControl("I");
            i_iconBtn.Attributes.Add("class", "far fa-minus-square");
            button.Controls.Add(i_iconBtn);

            pnlAction.Controls.Add(button);

            if (iconExporta)
            {
                LinkButton exportaExcel = new LinkButton();
                exportaExcel.ID = nameControl;
                exportaExcel.Click += DTIChartsAndControls.Obtener(nameMetodoEventHandler, target);

                Literal literal = new Literal();
                literal.Text = "&nbsp;";
                exportaExcel.Controls.Add(literal);

                HtmlContainerControl i_iconLink = new HtmlGenericControl("I");
                i_iconLink.Attributes.Add("class", "fa fa-file-excel excel");
                exportaExcel.Controls.Add(i_iconLink);
                pnlAction.Controls.Add(exportaExcel);
            }


            pnlTitulo.Controls.Add(pnlAction);

            #endregion

            panel.Controls.Add(pnlTitulo);

            Panel pnlBody = new Panel();
            pnlBody.CssClass = "portlet-body";

            HtmlContainerControl divSub = new HtmlGenericControl("DIV");
            divSub.Attributes.Add("class", "collapse in");
            divSub.Attributes.Add("id", idContenedor + "Collapse");

            Panel pnlPestañas = new Panel();
            pnlPestañas.CssClass = "tabbable tabbable-tabdrop";

            HtmlContainerControl ulPestañas = new HtmlGenericControl("UL");
            ulPestañas.Attributes.Add("class", "nav nav-pills");

            Panel pnlContent = new Panel();
            pnlContent.CssClass = "tab-content";

            var dic = titulosPestañas;
            for (int i = 0; i < dic.Length; i++)
            {
                HtmlContainerControl li = new HtmlGenericControl("LI");
                HtmlContainerControl div = new HtmlGenericControl("DIV");
                HtmlContainerControl div1 = new HtmlGenericControl("DIV");
                HtmlContainerControl div2 = new HtmlGenericControl("DIV");
                HtmlContainerControl div3 = new HtmlGenericControl("DIV");
                HtmlContainerControl div4 = new HtmlGenericControl("DIV");

                div.Attributes.Add("id", "tab" + idContenedor);   ///

                if (i == pestañaActiva)
                {
                    li.Attributes.Add("class", "active");
                    div.Attributes.Add("class", "tab-pane active");
                }
                else { div.Attributes.Add("class", "tab-pane"); }

                div1.Attributes.Add("class", "row");
                div3.Attributes.Add("class", "col-sm-12");
                div4.Attributes.Add("class", "col-sm-12");
                div2.Attributes.Add("class", "row");


                //Pestaña Titulo
                HyperLink tab = new HyperLink();
                tab.NavigateUrl = "#tab" + idContenedor + "_" + i.ToString();  ///           
                tab.Text = dic[i];
                tab.Attributes.Add("data-toggle", "tab");
                li.Controls.Add(tab);
                ulPestañas.Controls.Add(li);
                //

                div.Attributes.Add("id", "tab" + idContenedor + "_" + i.ToString());   ///

                //if (controls[i] == null)
                //{
                //    HtmlContainerControl canvas = new HtmlGenericControl("DIV");
                //    canvas.Attributes.Add("id", "graf_" + idContenedor + "_" + i.ToString());   ///
                //    div.Controls.Add(canvas);
                //}
                //else
                //{
                //    Panel tabla = new Panel();
                //    tabla.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
                //    tabla.Controls.Add(controls[i]);
                //    div.Controls.Add(tabla);
                //}

                /*GRAFICA*/
                HtmlContainerControl canvas = new HtmlGenericControl("DIV");
                canvas.Attributes.Add("id", "graf_" + idContenedor + "_" + i.ToString());   ///
                div3.Controls.Add(canvas);
                div1.Controls.Add(div3);
                div.Controls.Add(div1);


                /*TABLA*/
                Panel tabla1 = new Panel();
                tabla1.CssClass = headerFijo ? "table-fixed-nz" : "table-responsive";
                tabla1.Controls.Add(controls[i]);
                div2.Controls.Add(tabla1);
                div4.Controls.Add(div2);
                div.Controls.Add(div4);



                pnlContent.Controls.Add(div);
            }

            pnlPestañas.Controls.Add(ulPestañas);
            pnlPestañas.Controls.Add(pnlContent);

            divSub.Controls.Add(pnlPestañas);

            pnlBody.Controls.Add(divSub);
            panel.Controls.Add(pnlBody);

            return panel;
        }
    }

    public class MapNav
    {
        public string Titulo { get; set; }
        public string URL { get; set; }
    }
}
