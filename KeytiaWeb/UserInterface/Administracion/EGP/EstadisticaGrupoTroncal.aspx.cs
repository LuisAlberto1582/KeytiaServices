using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardFC;
using KeytiaWeb.UserInterface.DashboardLT;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Administracion.EGP
{
    public partial class EstadisticaGrupoTroncal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");

            if (!IsPostBack)
            {
                System.Data.DataTable troncales = DSODataAccess.Execute(ConsultaTroncales());
                System.Data.DataTable tipollamada = DSODataAccess.Execute(ConsultaTiposLlamada());
                System.Data.DataTable sitio = DSODataAccess.Execute(ConsultaSitio());
                var date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var lastday = date1.AddDays(7);
                FillDropDownListTroncales(troncales);
                FillDropDownListTipoLlamada(tipollamada);
                FillDropDownListSitio(sitio);
                //LlenarTabla(GrafConsHist);
               
                Date1.Text = date1.ToShortDateString();
                Date2.Text = lastday.ToShortDateString();
            }
        }

        private string ConsultaTroncales()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select distinct GrupoTroncal  ");
            query.AppendLine(" From " + DSODataContext.Schema + ".[DisponibilidadTraficoTroncales]  ");
            query.AppendLine("Where  GrupoTroncal <> ''");

           
            return query.ToString();
        }

        private string ConsultaTiposLlamada()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select distinct TipoLlamada  ");
            query.AppendLine(" From " + DSODataContext.Schema + ".[DisponibilidadTraficoTroncales]  ");
            query.AppendLine("Where  TipoLlamada <> ''");


            return query.ToString();
        }

        private string ConsultaSitio()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select distinct SitioDesc  ");
            query.AppendLine(" From " + DSODataContext.Schema + ".[DisponibilidadTraficoTroncales]  ");
            query.AppendLine("Where  SitioDesc <> ''");


            return query.ToString();
        }

        private void LlenarTabla(System.Data.DataTable grafConsHist)
        {

            if (grafConsHist.Rows.Count > 0)
            {
                //grafConsHist.AsEnumerable().OrderBy(x => x.Field<string>("Fecha"));
                //grafConsHist.Columns[0].ColumnName = "Fecha";
                //grafConsHist.Columns[1].ColumnName = "Grupo Troncal";

                //grafConsHist.Columns[2].ColumnName = "Hora Inicio";
                //grafConsHist.Columns[3].ColumnName = "Hora Fin";
                //grafConsHist.Columns[4].ColumnName = "Tipo de Llamada";
                //grafConsHist.Columns[5].ColumnName = "Circuitos";


                //Rep1.Controls.Add(
                //         DTIChartsAndControls.TituloY2RowsTablaGrafica(
                //                         DTIChartsAndControls.GridView("RepNumMasCarosPeEmGrid_T", grafConsHist, false, "Totales"),
                //                         "RepNumMasCarosPeEmGridRep03_T", "Uso de Troncales", Request.Path + "?Nav=NumMasCarosN1")
                //         );

                foreach (ListItem item in lstBoxTroncales.Items)
                {

                    if (item.Selected)
                    {
                        if (grafConsHist.AsEnumerable().Where(x => x.Field<string>("GrupoTroncal") == item.Text).Count() > 0)
                        {
                            paneles.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas($"GrafPorTroncal_{Regex.Replace(item.Text, @"[.\s-]+", "_", RegexOptions.Compiled)}", $"Gráfica del Uso de la troncal {item.Text}", 0, FCGpoGraf.TabularLiTa, Request.Path + "?Nav=PorTipoLlamN1"));

                            System.Data.DataTable newdt;
                            if (grafConsHist.Columns.Contains("Hora"))
                            {
                                newdt = grafConsHist.AsEnumerable().Where(x => x.Field<string>("GrupoTroncal") == item.Text).GroupBy(y => new { Fecha = y.Field<string>("Fecha"), Hora = y.Field<string>("Hora"), GrupoTroncal = y.Field<string>("GrupoTroncal"), TipoLlamada = y.Field<string>("TipoLlamada") }).Select(

                                g =>
                                {
                                    int val = int.Parse(g.Key.Hora);
                                    TimeSpan result = TimeSpan.FromHours(val);
                                    string fromTimeString = result.ToString("hh':'mm");
                                    var row = grafConsHist.NewRow();
                                    row["Fecha"] = g.Key.Fecha;
                                    row["Circuitos"] = g.Sum(r => r.Field<int>("Circuitos"));

                                    return row;
                                }
                                ).OrderBy(x => x.Field<string>("Fecha")).CopyToDataTable();
                            }
                            else
                            {
                                newdt = grafConsHist.AsEnumerable().Where(x => x.Field<string>("GrupoTroncal") == item.Text).GroupBy(y => new { Fecha = y.Field<string>("Fecha"),  GrupoTroncal = y.Field<string>("GrupoTroncal"), TipoLlamada = y.Field<string>("TipoLlamada") }).Select(

                               g =>
                               {
                                   var row = grafConsHist.NewRow();
                                   row["Fecha"] = g.Key.Fecha ;
                                   row["Circuitos"] = g.Sum(r => r.Field<int>("Circuitos"));

                                   return row;
                               }
                               ).OrderBy(x => x.Field<string>("Fecha")).CopyToDataTable();
                            }

                            

                            DataView dt = new DataView(newdt);
                            var tabla = dt.ToTable(false, new string[] { "Fecha", "Circuitos" });
                            tabla.Columns[0].ColumnName = "label";
                            tabla.Columns[1].ColumnName = "value";

                            Page.ClientScript.RegisterStartupScript(this.GetType(), $"GrafPorTroncal{Regex.Replace(item.Text, @"[.\s-]+", "_", RegexOptions.Compiled)}",
                                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(tabla),
                                $"GrafPorTroncal_{Regex.Replace(item.Text, @"[.\s-]+", "_", RegexOptions.Compiled)}", "", "", "Fecha", 
                                "Circuitos", 0, FCGpoGraf.TabularLiTa, "",""), false);
                        }
                    }
                   
                   
                }

                
            }




        }

        private void FillDropDownListTroncales(System.Data.DataTable grafConsHist)
        {
            if (grafConsHist.Rows.Count > 0)
            {

                lstBoxTroncales.DataSource = null;
                lstBoxTroncales.DataSource = grafConsHist.AsEnumerable().Where(y => y.Field<string>("GrupoTroncal") != null).Select(x => x.Field<string>("GrupoTroncal")).ToList();
                lstBoxTroncales.DataBind();

            }


        }



        private void FillDropDownListTipoLlamada(System.Data.DataTable grafConsHist)
        {
            if (grafConsHist.Rows.Count > 0)
            {

                lstTipo.DataSource = null;
                lstTipo.DataSource = grafConsHist.AsEnumerable().Where(y => y.Field<string>("TipoLlamada") != null).Select(x => x.Field<string>("TipoLlamada")).ToList();
                lstTipo.DataBind();


            }


        }

        private void FillDropDownListSitio(System.Data.DataTable grafConsHist)
        {
            if (grafConsHist.Rows.Count > 0)
            {

                lstSitio.DataSource = null;
                lstSitio.DataSource = grafConsHist.AsEnumerable().Where(y => y.Field<string>("SitioDesc") != null).Select(x => x.Field<string>("SitioDesc")).ToList();
                lstSitio.DataBind();


            }


        }

        private string ConsultaEstadisticasGruposTroncales(string where)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select  convert(varchar,Fecha,20) as Fecha, GrupoTroncal, HoraInicio, TipoLlamada, , SitioDesc as Sitio, sum(Cantidad) as Circuitos ");
            query.AppendLine(" From " + DSODataContext.Schema + ".[DisponibilidadTraficoTroncales]  ");
            query.AppendLine("Where ");

            if (Date1.Text == "" && Date2.Text == "")
            {
                var date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var lastday = date1.AddDays(7);
                query.AppendLine(" Fecha  >= '" + date1.ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and  ");
                query.AppendLine(" Fecha  <= '" + lastday.ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            else
            {
                query.AppendLine(" Fecha  >= '" + DateTime.Parse(Date1.Text).ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and ");
                query.AppendLine(" Fecha  <= '" + DateTime.Parse(Date2.Text).ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            if(where != null && where != "")
            query.AppendLine(" and " + where + "");
            query.AppendLine(" Group By Fecha, GrupoTroncal,HoraInicio, TipoLlamada, SitioDesc");
            query.AppendLine(" Order By Fecha");
            return query.ToString();
        }


        private string ConsultaEstadisticasGruposTroncalesPorHora(string where)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT Convert(varchar,Fecha,11) AS Fecha, GrupoTroncal, TipoLlamada,  SitioDesc as Sitio,");
            query.AppendLine(" convert(varchar,DATEPART(hour,Fecha)) AS Hora, ");
            query.AppendLine("  Max(Cantidad) AS Circuitos ");
            query.AppendLine(" From " + DSODataContext.Schema + ".[DisponibilidadTraficoTroncales]  ");
            query.AppendLine("Where ");

            if (Date1.Text == "" && Date2.Text == "")
            {
                var date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var lastday = date1.AddDays(7);
                query.AppendLine(" Fecha  >= '" + date1.ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and  ");
                query.AppendLine(" Fecha  <= '" + lastday.ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            else
            {
                query.AppendLine(" Fecha  >= '" + DateTime.Parse(Date1.Text).ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and ");
                query.AppendLine(" Fecha  <= '" + DateTime.Parse(Date2.Text).ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            if (where != null && where != "")
                query.AppendLine(" and " + where + "");
            query.AppendLine(" GROUP BY Convert(varchar,Fecha,11), DATEPART(hour,Fecha), GrupoTroncal, TipoLlamada, SitioDesc");
            query.AppendLine(" order by Fecha, Hora");
            return query.ToString();
        }

        private string ConsultaEstadisticasGruposTroncalesPorMinuto(string where)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT convert(varchar,Fecha,21) as Fecha,convert(varchar,DATEPART(hour,Fecha)) as Hora, GrupoTroncal, TipoLlamada, SitioDesc as Sitio,");
            query.AppendLine("  Cantidad AS Circuitos");
            query.AppendLine(" From " + DSODataContext.Schema + ".[DisponibilidadTraficoTroncales]  ");
            query.AppendLine("Where ");

            if (Date1.Text == "" && Date2.Text == "")
            {
                var date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var lastday = date1.AddDays(7);
                query.AppendLine(" Fecha  >= '" + date1.ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and  ");
                query.AppendLine(" Fecha  <= '" + lastday.ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            else
            {
                query.AppendLine(" Fecha  >= '" + DateTime.Parse(Date1.Text).ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and ");
                query.AppendLine(" Fecha  <= '" + DateTime.Parse(Date2.Text).ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            if (where != null && where != "")
                query.AppendLine(" and " + where + "");
            query.AppendLine(" order by Fecha");
            return query.ToString();
        }

        private string ConsultaEstadisticasGruposTroncalesPorMediaHora(string where)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" select convert(varchar,dateadd(minute,datediff(minute,0,Fecha)/30*30,0),21) as Fecha,convert(varchar,DATEPART(hour,Fecha)) AS Hora, GrupoTroncal, TipoLlamada, SitioDesc ");
            query.AppendLine("  , max(cantidad) as Circuitos");
            query.AppendLine(" From " + DSODataContext.Schema + ".[DisponibilidadTraficoTroncales]  ");
            query.AppendLine("Where ");

            if (Date1.Text == "" && Date2.Text == "")
            {
                var date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var lastday = date1.AddDays(7);
                query.AppendLine(" Fecha  >= '" + date1.ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and  ");
                query.AppendLine(" Fecha  <= '" + lastday.ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            else
            {
                query.AppendLine(" Fecha  >= '" + DateTime.Parse(Date1.Text).ToString("yyyy-MM-dd 00:00:00") + "'");
                query.AppendLine(" and ");
                query.AppendLine(" Fecha  <= '" + DateTime.Parse(Date2.Text).ToString("yyyy-MM-dd 23:59:00") + "'");
            }
            if (where != null && where != "")
                query.AppendLine(" and " + where + "");
            query.AppendLine(" group by convert(varchar,dateadd(minute,datediff(minute,0,Fecha)/30*30,0),21),GrupoTroncal, TipoLlamada,convert(varchar,DATEPART(hour,Fecha)), SitioDesc ");
            query.AppendLine(" order by Fecha");
            return query.ToString();
        }


        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {

            System.Data.DataTable ldt = DSODataAccess.Execute(Query());

            DataView dt = new DataView(ldt);
            var table = dt.ToTable(false, new string[] { "Fecha", "Hora", "GrupoTroncal", "TipoLlamada", "Circuitos" });
            table.Columns[0].ColumnName = "Fecha";
            table.Columns[1].ColumnName = "Hora";
            table.Columns[2].ColumnName = "TRK";
            table.Columns[3].ColumnName = "Tipo";
            table.Columns[4].ColumnName = "Circuitos";
            ExcelAccess lExcel = new ExcelAccess();
            try
            {

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\reportes\Plantilla Reporte Estandar" + ".xlsx");
                lExcel.Abrir();
                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                var troncales = ldt.AsEnumerable().Where(x=> x.Field<string>("GrupoTroncal") != "").Select(x => x.Field<string>("GrupoTroncal")).Distinct().ToList();
                foreach (var item in troncales)
                {
                    var nombre = Regex.Replace(item, @"[.\s]+", "_", RegexOptions.Compiled);
                    if (nombre.Length > 30)
                        nombre = nombre.Substring(0, 29);

                    lExcel.CreaHojaExcel(nombre);
                    lExcel.SetXlSheet(nombre);
                    var datos = ldt.AsEnumerable().Where(x => x.Field<string>("GrupoTroncal") == item).CopyToDataTable();
                    
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, datos, nombre, "Datos");

                    var grafdt = new DataView(datos).ToTable(false,new string[] {"Fecha", "Circuitos" });
                    
                    ExportacionExcelRep.CrearGrafico(grafdt, $"Uso de la troncal {item}", new string[] { "Fecha","Circuitos" },
                        new string[] { "" }, new string[] { "Circuitos" },
                        "Fecha", "", "", "Circuitos", "",
                                       false, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", nombre, nombre, 900, 300);
                   

                }

                //lExcel.Remover("Reporte");
                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + ".xlsx");
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;
                lExcel.xlBook.RefreshAll();
                lExcel.SalvarComo();

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reporte" + "_" + "Uso de Troncales");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrExportTo", ex, ".xlsx");
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

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            var where = ConsultaFiltrada();
            

            var fechainicio = DateTime.Parse(Date1.Text);
            var fechafin = DateTime.Parse(Date2.Text);
            System.Data.DataTable datos = DSODataAccess.Execute(Query());
            //FillDropDownLists(datos);
            LlenarTabla(datos);

        }

        private string Query()
        {
            var where = ConsultaFiltrada();


            var fechainicio = DateTime.Parse(Date1.Text);
            var fechafin = DateTime.Parse(Date2.Text);
            if (fechafin - fechainicio <= TimeSpan.FromDays(3))
            {
                return ConsultaEstadisticasGruposTroncalesPorMinuto(where);
            }
            else if (fechafin - fechainicio > TimeSpan.FromDays(3) && fechafin - fechainicio <= TimeSpan.FromDays(20))
            {
                return ConsultaEstadisticasGruposTroncalesPorMediaHora(where);
            }
            else
            {
                return ConsultaEstadisticasGruposTroncalesPorHora(where);
            }
        }
        private string ConsultaFiltrada()
        {
            List<string> selectedItems = new List<string>();
            StringBuilder query = new StringBuilder(); ;

            bool todosTroncales = true;
            bool todosTipo = true;
            if (lstBoxTroncales.SelectedItem != null)
            {

                foreach (ListItem item in lstBoxTroncales.Items)
                {
                    if (!item.Selected)
                    {
                        todosTroncales = false;
                        break;
                    }

                }

                if (!todosTroncales)
                {
                    query.AppendLine("GrupoTroncal IN(");
                    foreach (ListItem item in lstBoxTroncales.Items)
                    {
                        if (item.Selected)
                        {
                            selectedItems.Add("'" + item.Value + "'");
                        }
                    }
                    query.AppendLine(string.Join(",", selectedItems));
                    query.AppendLine(")");
                }
            }
            selectedItems.Clear();
            if (lstTipo.SelectedItem != null)
            {
                foreach (ListItem item in lstTipo.Items)
                {
                    if (!item.Selected)
                    {
                        todosTipo = false;
                        break;
                    }
                }
                if (!todosTipo)
                {
                    if (!todosTroncales)
                        query.AppendLine("AND TipoLlamada IN(");
                    else
                        query.AppendLine("TipoLlamada IN(");

                    foreach (ListItem item in lstTipo.Items)
                    {
                        if (item.Selected)
                        {
                            selectedItems.Add("'" + item.Value + "'");
                        }

                    }
                    query.AppendLine(string.Join(",", selectedItems));
                    query.AppendLine(")");
                }
            }

            selectedItems.Clear();
            if (lstSitio.SelectedItem != null)
            {
                foreach (ListItem item in lstSitio.Items)
                {
                    if (!item.Selected)
                    {
                        todosTipo = false;
                        break;
                    }
                }
                if (!todosTipo)
                {
                    if (!todosTroncales)
                        query.AppendLine("AND SitioDesc IN(");
                    else
                        query.AppendLine("SitioDesc IN(");

                    foreach (ListItem item in lstSitio.Items)
                    {
                        if (item.Selected)
                        {
                            selectedItems.Add("'" + item.Value + "'");
                        }

                    }
                    query.AppendLine(string.Join(",", selectedItems));
                    query.AppendLine(")");
                }
            }

            return query.ToString();
           // return ConsultaEstadisticasGruposTroncales(query.ToString());
        }

        private void CrearRow(int rowIni, int colIni, System.Data.DataTable dt, Worksheet worksheet, bool totales)
        {
            int numeroColum = dt.Columns.Count;
            int rowFin = dt.Rows.Count + rowIni;
            int rowDatos = rowIni + 1;
            int indiceColumna = colIni;
            Range formatRange;

            /*GENERA ENCABEZADOS*/
            foreach (DataColumn col in dt.Columns)  //Columnas
            {
                indiceColumna++;
                worksheet.Cells[rowIni, indiceColumna] = col.ColumnName;
            }

            /*VACIA LA INFORMACION EN LA HOJA*/
            foreach (DataRow row in dt.Rows)//Filas
            {
                indiceColumna = colIni;

                foreach (DataColumn col in dt.Columns)  //Columnas
                {
                    indiceColumna++;
                    worksheet.Cells[rowDatos, indiceColumna] = row[col.ColumnName];
                }

                formatRange = worksheet.Range[worksheet.Cells[rowDatos, colIni + 1], worksheet.Cells[rowDatos, indiceColumna]];
                formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                rowDatos++;
            }


            worksheet.Columns.AutoFit();

        }


        private Worksheet CrearHojaExcel(Workbook workbook, string nombreHoja)
        {
            Worksheet worksheet;
            worksheet = (Worksheet)workbook.Worksheets.Add();
            worksheet.Name = nombreHoja;
            worksheet = (Worksheet)workbook.Worksheets[nombreHoja];
            worksheet.Select(Type.Missing);

            return worksheet;
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}