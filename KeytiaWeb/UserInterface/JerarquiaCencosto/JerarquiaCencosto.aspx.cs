using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardFC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.JerarquiaCencosto
{
    public partial class JerarquiaCencosto : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema; 
        private string connStr = DSODataContext.ConnectionString;
        public static List<CenCosMod> lstCenCos = new List<CenCosMod>();
        public static List<CencosList> lstCenCosto = new List<CencosList>();
        static List<CenCosMod> lstCenCosWChildren = new List<CenCosMod>();
        static string folder;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            folder = Server.MapPath("~/UserInterface/JerarquiaCencosto/");
            if (!Page.IsPostBack)
            {
                IniciaProc();
            }
            
        }
        private void IniciaProc()
        { 
            lstCenCos.Clear();
            lstCenCosto.Clear();
            lstCenCosWChildren.Clear();

            string cencos = (txtCencosId.Text != "") ? txtCencosId.Text.ToString() : "";
            string nomCencos = txtCencosto.Text.ToString();
            string ruta;
            GenerarDatos(cencos, nomCencos);
            //Lanza la funcion recursiva, iniciando con el CenCos superior
            lstCenCosWChildren.Clear();
            if (cencos != "")
            {
                GetJerarquia(lstCenCos.FirstOrDefault(x => x.parent == "0" && x.name == cencos));
            }
            else
            {
                GetJerarquia(lstCenCos.FirstOrDefault(x => x.parent == "0"));
            }


            //
            //Serializa los datos, en formato json

            ruta = SerializaDatos();
            if (lstCenCosWChildren.Count > 1)
            {
                if (File.Exists(ruta))
                {
                    iframeDiv.Controls.Add(new LiteralControl("<iframe class='col-sm-12' id='CencosJer' style='height:800px;border-style:none;' src=cencos.html></iframe><br/>"));
                    hdfPath.Value = esquema.ToUpper() + ".json";
                }

            }
            else
            {
                //ObieneJer(cencos);
                //ruta = SerializaDatos();
                //if (lstCenCosWChildren.Count >= 1)
                //{
                //    if (File.Exists(ruta))
                //    {
                //        iframeDiv.Controls.Add(new LiteralControl("<iframe class='col-sm-12' id='CencosJer' style='height:800px;border-style:none;' src=cencos.html></iframe><br/>"));
                //        hdfPath.Value = esquema + ".json";
                //    }

                //}

                lblTituloModalMsn.Text = "Atención";
                lblBodyModalMsn.Text = "El centro de Costos: " + txtCencosto.Text + " no tiene jerarquia";
                mpeEtqMsn.Show();
                txtCencosId.Text = "";
                txtCencosto.Text = "";
                IniciaProc();
            }

        }
        [WebMethod]
        public static object GetCencos(string texto)
        {
            DataTable CenCosto = new DataTable();
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo, vchCodigo+'-'+Descripcion AS vchDescripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".HistCencos WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion + Descripcion LIKE '%" + texto + "%'");

            CenCosto = DSODataAccess.Execute(query.ToString(), connStr);
            DataView dvldt = new DataView(CenCosto);
            CenCosto = dvldt.ToTable(false, new string[] { "iCodCatalogo", "vchDescripcion" });
            CenCosto.Columns["iCodCatalogo"].ColumnName = "idCencos";
            CenCosto.Columns["vchDescripcion"].ColumnName = "Descripcion";
            return FCAndControls.ConvertDataTabletoJSONString(CenCosto);
        }
        private static void GenerarDatos(string cencos, string name)
        {
            lstCenCos.Clear();

            string conn_string = DSODataContext.ConnectionString;
            string query = "SELECT iCodCatalogo AS Id,isnull(CenCos, 0) as IdPadre,vchCodigo+'-'+ISNULL(UPPER(Descripcion),'') AS Descripcion FROM " + DSODataContext.Schema + ".HistCenCos WITH(NOLOCK) WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()";

            if (cencos != "")
            {
                var centro1 = new CenCosMod
                {
                    name = cencos,
                    Descripcion = name,
                    parent = "0"
                };
                lstCenCos.Add(centro1);
            }
            
            if(lstCenCosto.Count > 0)
            {
                foreach (var item in lstCenCosto)
                {
                    var centro = new CenCosMod
                    {
                        name = item.name,
                        parent =item.parent,
                        Descripcion = item.Descripcion
                    };
                    lstCenCos.Add(centro);
                }
            }
            else
            {
                lstCenCosto.Clear();

                DataTable dt = DSODataAccess.Execute(query, conn_string);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var centro = new CenCosMod
                        {
                            name = dr["Id"].ToString(),
                            parent = dr["IdPadre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        };
                        lstCenCos.Add(centro);

                        var cencosto = new CencosList
                        {
                            name = dr["Id"].ToString(),
                            parent = dr["IdPadre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        };
                        lstCenCosto.Add(cencosto);
                    }
                }
            }
           

        }
        private static void GetJerarquia(CenCosMod cc)
        {
            var hijos = lstCenCos.Where(x => x.parent == cc.name).ToList();

            if (hijos.Count == 0)
            {
                cc.children = null;
            }  

            else
            {
                cc.children = hijos;

                foreach (var hijo in cc.children)
                {
                    GetJerarquia(hijo);
                }
            }

            lstCenCosWChildren.Add(cc);
        }
        private static string SerializaDatos()
        {
            string path = "";
            try
            {
                path = folder + DSODataContext.Schema.ToUpper() + ".json";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                JsonSerializer serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                using (StreamWriter sw = new StreamWriter(path))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, lstCenCosWChildren.FirstOrDefault(x => x.parent == "0"));
                    }
                }
            }
            catch
            {

            }

            return path;
        }
        protected void btnDescargar_Click(object sender, EventArgs e)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string sp = "EXEC ObtieneHijosDeUnCC '" + DSODataContext.Schema + "'";
                DataTable ldt = DSODataAccess.Execute(sp, connStr);
                string nomFile = "Jerarquia_" + DSODataContext.Schema;

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    ldt.Columns.Remove("iCodCatN1");
                    ldt.Columns.Remove("iCodCatN2");
                    ldt.Columns.Remove("iCodCatN3");
                    ldt.Columns.Remove("iCodCatN4");
                    ldt.Columns.Remove("iCodCatN5");
                    ldt.Columns.Remove("iCodCatN6");
                    ldt.Columns.Remove("iCodCatN7");
                    ldt.Columns.Remove("iCodCatN8");
                    ldt.Columns.Remove("iCodCatN9");
                    ldt.Columns.Remove("iCodCatN10");

                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + ".xlsx");
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Tipo Destino");
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");

                    string psFileKey;
                    string psTempPath;

                    psFileKey = Guid.NewGuid().ToString();
                    psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                    System.IO.Directory.CreateDirectory(psTempPath);

                    string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + ".xlsx");
                    Session[psFileKey] = lsFileName;

                    lExcel.FilePath = lsFileName;
                    lExcel.SalvarComo();

                    ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reportes" + "_" + nomFile);
                }
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
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtCencosId.Text = "";
            txtCencosto.Text = "";
            IniciaProc();
        }
        private static void ObieneJer(string cenc)
        {
            lstCenCosWChildren.Clear();
            Child(cenc);
            var lista = lstCenCosWChildren;
            if(lista.Count >1)
            {
                foreach (var item in lista)
                {
                    var parent= item.parent;
                }
            }
        }
        private static void Child(string cenc)
        {
            string padre;
            var r = lstCenCosto.FirstOrDefault(x => x.name == cenc);

            padre = r.parent;
            if (padre !="0")
            {
                var t = lstCenCosto.FirstOrDefault(x => x.name == padre);
                CenCosMod c = new CenCosMod();
                c.name = t.name;
                c.parent = t.parent;
                c.Descripcion = t.Descripcion;
                c.children = hijos(r.name, r.parent, r.Descripcion);

                lstCenCosWChildren.Add(c);

                Child(padre);
            }

        }
        private static List<CenCosMod> hijos(string name, string parent, string descripcion)
        {
            List<CenCosMod> l = new List<CenCosMod>();
            CenCosMod t = new CenCosMod
            {
                name = name,
                parent = parent,
                Descripcion = descripcion
            };
            l.Add(t);

            return l;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            IniciaProc();
        }
    }
    public class CenCosMod
    {
        // El JSON solicita puros valores string
        public string name { get; set; }
        public string parent { get; set; }
        public string Descripcion { get; set; }
        public List<CenCosMod> children { get; set; } = null;
    }
    public class CencosList
    {
        public string name { get; set; }
        public string parent { get; set; }
        public string Descripcion { get; set; }
    }
    
}