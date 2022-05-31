using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.ModuloKeytiaManagement
{
    public partial class DescargaDeRecursos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            // Se deshabilita la funcionalidad del boton
            linkBtn.Enabled = false;

            DataSet reportesConvertir = new DataSet();
            string timestamp = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss.fff");
            string filename = "Recursos_Activos_" + timestamp + ".xlsx";
            string path = Server.MapPath(@"~\ArchivosDescargaKM\" + filename);

            byte[] archivoRespuesta;

            foreach (ListItem li in CheckBoxListRecursos.Items)
            {
                DataTable repoDt = new DataTable();

                if (li.Selected)
                {
                    switch (li.Text)
                    {
                        case "Cencos":
                            repoDt = ConsultasDb.CencosData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Empleados":
                            repoDt = ConsultasDb.EmpleActivosData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Codigos de Autorizacion":
                            repoDt = ConsultasDb.CodAutData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Extensiones Activas":
                            repoDt = ConsultasDb.ExtensionesData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Lineas Activas":
                            repoDt = ConsultasDb.LineasData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Sitios":
                            repoDt = ConsultasDb.SitioData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Tipo de Empleado":
                            repoDt = ConsultasDb.TipoEmData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Puestos":
                            repoDt = ConsultasDb.PuestoData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Cos":
                            repoDt = ConsultasDb.CosData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Carriers":
                            repoDt = ConsultasDb.CarrierData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Cuentas Maestras":
                            repoDt = ConsultasDb.CuentaMaestraData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Tipo de Plan":
                            repoDt = ConsultasDb.TipoPlanData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Equipo de Celular":
                            repoDt = ConsultasDb.EquipoCelularData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Plan Tarifario":
                            repoDt = ConsultasDb.PlanTarifarioData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "CodAut sin Empleado":
                            repoDt = ConsultasDb.CodAutoSinEmpleData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Extensiones sin Empleado":
                            repoDt = ConsultasDb.ExtenSinEmpleData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Lineas sin Empleado":
                            repoDt = ConsultasDb.LineasSinEmpleData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                        case "Organizaciones":
                            repoDt = ConsultasDb.OrganizacionData();
                            repoDt.TableName = li.Text;
                            reportesConvertir.Tables.Add(repoDt);
                            break;
                    }
                }
            }

            // Se crea el excel utilizando ClosedXML
            if (reportesConvertir.Tables.Count > 0)
            {
                using (var workbook = new XLWorkbook())
                {
                    foreach (DataTable table in reportesConvertir.Tables)
                    {
                        var worksheet = workbook.Worksheets.Add(table, table.TableName);
                    }

                    workbook.SaveAs(path);
                }

                // Se lee el archivo a un array de bytes para la respuesta y descarga al cliente
                archivoRespuesta = File.ReadAllBytes(path);

                // Eliminamos el archivo creado en la carpeta DownloadFiles
                File.Delete(path);

                Response.Buffer = true;
                Response.Clear();
                Response.ClearHeaders();
                Response.ContentType = "application/vnd.ms-excel";
                Response.CacheControl = "public";
                Response.AddHeader("Pragma", "public");
                Response.AddHeader("Expires", "0");
                Response.AddHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                Response.AddHeader("Content-Description", "Excel File Download");
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                Response.BinaryWrite(archivoRespuesta);
                Response.Flush();
                Response.End();
            }

            // Rehabilitamos la funcionalidad del boton
            linkBtn.Enabled = true;
        }
    }
}