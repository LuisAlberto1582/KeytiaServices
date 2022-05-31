using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class InventarioRecursos : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionInvRecursos", "InventarioRecursos('JSONInventarioRecursos');", true);
        }
        #region WeMethod Json
        [WebMethod]
        public static string JSONInventarioRecursos()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;
                DataTable dt = new DataTable();
                string connStr = DSODataContext.ConnectionString;
                string query = ObtieneRecursos(DSODataContext.Schema);

                dt = DSODataAccess.Execute(query, connStr);
                #region elimina Nulos
                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (dataRow[column] == DBNull.Value)
                        {
                            dataRow[column] = "";
                        }
                    }
                }
                #endregion
                #region Cambia Nombre Columnas

                if (dt.Columns.Contains("Renglon"))
                {
                    dt.Columns["Renglon"].ColumnName = "recid";
                }
                if (dt.Columns.Contains("PlanTarifDesc"))
                {
                    dt.Columns["PlanTarifDesc"].ColumnName = "Plan";
                }
                if (dt.Columns.Contains("IMEI"))
                {
                    dt.Columns["IMEI"].ColumnName = "IMEI";
                }
                #endregion Cambia Nombre Columnas

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

                return "{\"records\":" + serializer.Serialize(rows) + "}";
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion WeMethod Json
        #region QUERYS
        static string ObtieneRecursos(string Schema)
        {
            string sp = "EXEC WorkFlowLineasInventarioRecursos @Esquema = '{0}'";
            string query = string.Format(sp, Schema);
            return query;
        }
        #endregion QUERYS

        #region Logica de Botones

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS(".xlsx");
        }

        #endregion Logica de Botones

        #region Exportacion Reporte

        public void ExportXLS(string tipoExtensionArchivo)
        {
            CrearXLS(tipoExtensionArchivo);
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                #region Exportar Reportes solo con tabla
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\InventarioRecursos" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                #region Reporte Inventario de Recursos

                ProcesarTituloExcel(lExcel, "Inventario de Recursos");
                string query = ObtieneRecursos(DSODataContext.Schema);
                DataTable ldt = DSODataAccess.Execute(query, connStr);

                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(ldt);
                    ldt = dvldt.ToTable(false,
                        new string[] { "Tel", "PlanTarifDesc", "Marca", "Modelo", "IMEI", "SIMCard",
                                        "Estatus", "Estado","Empleado","Puesto","Empresa"});

                    ldt.Columns["Tel"].ColumnName = "Línea";
                    ldt.Columns["PlanTarifDesc"].ColumnName = "Plan";
                    ldt.Columns["IMEI"].ColumnName = "IMEI";
                    ldt.Columns["SIMCard"].ColumnName = "SIM";
                    ldt.AcceptChanges();
                }
                if (ldt.Rows.Count > 0)
                {
                    creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, ""), "Reporte", "Tabla");
                }

                #endregion Reporte Inventario de Recursos

                #endregion Exportar Reportes solo con tabla

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;
                lExcel.FilePath = lsFileName;
                lExcel.SalvarComo();
                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reporte" + "_" + "Inventario de Recursos");
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

        private static void creaTablaEnExcel(ExcelAccess lExcel, DataTable ldt, string hoja, string textoBusqueda)
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

        protected void ProcesarTituloExcel(ExcelAccess pExcel, string titulo)
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
                //pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + Convert.ToDateTime(Session["FechaIniRep"]).ToString("dd/MM/yyyy"));
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 2, "Fin:");
                //pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 3, "'" + Convert.ToDateTime(Session["FechaFinRep"]).ToString("dd/MM/yyyy"));
            }
        }

        protected Hashtable BuscarTituloExcel(ExcelAccess pExcel)
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto("Reporte", "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto("Reporte", "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto("Reporte", "{TituloReporte}", true));
            lhtRet.Add("{FechasReporte}", pExcel.BuscarTexto("Reporte", "{FechasReporte}", true));

            return lhtRet;
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion Exportacion Reporte detallado
    }

}