using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections;
using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using KeytiaWeb.UserInterface.DashboardFC;
using System.Data;
using System.Web.Services;

namespace KeytiaWeb.UserInterface.InventarioTelecom
{
    public partial class Inventario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
        }

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
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\InventarioTelecom\ReporteTabla" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                //Se quita logica de mas, puesto que solo se exportara una tabla. 
                //La plantilla que se usa para este reporte no llevara la sección de fechas no lleva fechas.
                #region Reporte Inventario de Equipo Telecom

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Inventario Telecom");

                DataTable ldt = DSODataAccess.Execute("EXEC [NCDRepInventarioEquiposDetall] @Esquema = '" + DSODataContext.Schema + "'");

                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(ldt);
                    ldt = dvldt.ToTable(false,
                        new string[] { "Cr", "Nombre", "Tipo Edificio", "Dirección IP", "Tipo Recurso", "GIG-E Ocu.", "Fa Ocu.", "GIG-E OFF", 
                                   "Fa OFF", "GIG-E Abajo", "Fa Abajo", "Total Puertos", "Admin", "Hostname", "Estatus", "Marca", "Modelo", 
                                   "No. Serie", "Versión IOS", "Domicilio", "Comentarios", "Stackeados", "Fecha Poleo"});

                    ldt.Columns["Cr"].ColumnName = "Cr Telecom";
                    ldt.Columns["GIG-E Ocu."].ColumnName = "GIG-E Ocupados";
                    ldt.Columns["Fa Ocu."].ColumnName = "Fa Ocupados";
                    ldt.Columns["Admin"].ColumnName = "Administrador";
                    ldt.AcceptChanges();
                }

                if (ldt.Rows.Count > 0)
                {
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                }

                #endregion Reporte Inventario de Equipo Telecom

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
                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reporte" + "_" + "Equipos_Telecom");
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

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion Exportacion Reporte detallado

        #region WebMethod

        //NZ 20160913 Se hace este metodo para que la fuente de la informacion de esta pagina no sea un archivo de texto si no 
        //directamente lo que se encuentra en base de datos.
        [WebMethod]
        public static object ConsultaInventarioTelecom()
        {
            DataTable dt = DSODataAccess.Execute("EXEC [NCDRepInventarioEquiposDetall] @Esquema = '" + DSODataContext.Schema + "'");
            System.Web.Script.Serialization.JavaScriptSerializer serializer =
                                new System.Web.Script.Serialization.JavaScriptSerializer();

            if (dt.Rows.Count > 0)
            {
                //Se renombran las columnas a como las necesita el archivo.
                #region Nombre Columnas
                dt.Columns["Num"].ColumnName = "recid";
                dt.Columns["Cr"].ColumnName = "CrTelecom";
                dt.Columns["Tipo Edificio"].ColumnName = "TipoEdificio";
                dt.Columns["Dirección IP"].ColumnName = "DirIP";
                dt.Columns["Tipo Recurso"].ColumnName = "Recurso";
                dt.Columns["GIG-E Ocu."].ColumnName = "GIG-E_Ocupados";
                dt.Columns["Fa Ocu."].ColumnName = "Fa_Ocupados";
                dt.Columns["GIG-E OFF"].ColumnName = "GIG-E_OFF";
                dt.Columns["Fa OFF"].ColumnName = "Fa_OFF";
                dt.Columns["GIG-E Abajo"].ColumnName = "GIG-E_Abajo";
                dt.Columns["Fa Abajo"].ColumnName = "Fa_Abajo";
                dt.Columns["Total Puertos"].ColumnName = "TotalPuertos";
                dt.Columns["No. Serie"].ColumnName = "NoSerie";
                dt.Columns["Versión IOS"].ColumnName = "Ver_IOS";
                dt.Columns["Fecha Poleo"].ColumnName = "FechaPoleo";
                #endregion Nombre Columnas

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
            else { return null; }
        }

        #endregion WebMethod

    }
}
