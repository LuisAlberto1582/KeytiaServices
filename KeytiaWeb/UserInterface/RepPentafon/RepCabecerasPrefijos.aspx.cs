using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepPentafon
{
    public partial class RepCabecerasPrefijos : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string sitio = "";
        static string camp = "";
        static string fechaInicio = "";
        static string fechaFinal = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            //Response.Write("<script type='text/javascript'> sessionStorage.clear();</script>");
             sitio = "";
            if (!Page.IsPostBack)
            {
                Response.Write("<script type='text/javascript'> sessionStorage.clear();</script>");
                IniciaProceso();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionCabeceras", "Cabeceras('JSONCabeceras');", true);

        }
        #region Metodos
        private void IniciaProceso()
        {
            ObtieneSitios();
        }
        private void ObtieneInfo(int sitio)
        {
            //System.Threading.Thread.Sleep(10000);
            camp = cboCampaña.SelectedItem.ToString();
            Response.Write("<script type='text/javascript'> sessionStorage.setItem('datom'," + sitio + ");</script>");
            //Response.Write("<script type='text/javascript'> var datom = "+ claveSit.Value + "; </script>");
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionCabeceras", "Cabeceras('JSONCabeceras');", true);
            //ScriptManager.GetCurrent(Page).RegisterPostBackControl(cboCampaña);
        }
        #endregion Metodos
        #region Consultas
        private void ObtieneSitios()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo, vchDescripcion FROM "+ esquema + ".[vishiscomun('Sitio','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia>= GETDATE()");
            query.AppendLine(" AND vchDesMaestro = 'Sitio - SBC'");
            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            if( dt != null && dt.Rows.Count > 0)
            {
                cboCampaña.DataSource = dt;
                cboCampaña.DataBind();
            }

        }
        private string ObtieneCabeceras(string campaña)
        {
            string sp = "exec [ReporteLlamsPorCabecera] @Schema = '" + esquema + "',  @FechaInicio = '2019-05-01 00:00:00', @FechaFin = '2019-05-30 23:59:29', @campaña = {0}";

            string query = string.Format(sp,campaña);
            return query;
        }
        #endregion Consultas

        protected void cboCampaña_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        #region WeMethod Json
        [WebMethod]
        public static string JSONCabeceras(string sitioClave)
        {
            sitio = sitioClave;
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                DataTable dt = new DataTable();
                string esquema = DSODataContext.Schema;
                string connStr = DSODataContext.ConnectionString;

                string sp = "exec [ReporteLlamsPorCabecera] @Schema = '" + esquema + "',  @FechaInicio = '{0}', @FechaFin = '{1}', @campaña = {2}";
                string query = string.Format(sp, fechaInicio, fechaFinal, sitio);                

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

                if (dt.Columns.Contains("ID"))
                {
                    dt.Columns["ID"].ColumnName = "recid";
                }
                #endregion


                List<ListaInv> list = new List<ListaInv>();
                List<string> customers = new List<string>();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                         row.Add(col.ColumnName, dr[col]);
                    }
                    row.Add("sitio", camp);
                    rows.Add(row);

                }

                return "{\"records\":" + serializer.Serialize(rows) + "}";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            //DateTime fechaI;
            //DateTime fechaF;
            int dato = Convert.ToInt32(cboCampaña.SelectedValue.Replace("Selecciona Una Campaña", "0"));
            //hfFechaInicio1.Value = fechaI.ToString("dd/MM/yyyy");
            //hfFechaFin1.Value = fechaF.ToString("dd/MM/yyyy");
            fechaInicio = inicioFecha.Value;
            fechaFinal = finalFecha.Value;
            if (dato > 0)
            {
                string valor = cboCampaña.SelectedValue.ToString();
                claveSit.Value = valor;
                ObtieneInfo(dato);
            }
        }
    }
    public class ListaInv
    {
        public string Extension { get; set; }
        public string Cabecera { get; set; }
        public string Marcaciones { get; set; }
        public string Contestadas { get; set; }
        public string Contacto { get; set; }
    }
}