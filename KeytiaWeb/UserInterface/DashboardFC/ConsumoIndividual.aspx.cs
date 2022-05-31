using KeytiaServiceBL;
using System;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class ConsumoIndividual : Page
    {

        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int iCodUsuario;
        int perfil;
        string fechaFactura;
        protected void Page_Load(object sender, EventArgs e)
        {
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            perfil = (int)Session["iCodPerfil"];
            IniciProc();
        }
        private void IniciProc()
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

           
            if (!Page.IsPostBack)
            {
                if (perfil == 367)
                {
                    cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
                    cboAnio.DataBind();
                    cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
                    cboMes.DataBind();
                    ObtieneFechaFact();
                    rowBusqueda.Visible = true;
                    rowFechas.Visible = true;
                }
                else
                {
                    rowBusqueda.Visible = false;
                    rowFechas.Visible = false;
                    fechaFactura = ObtieneFechaMax();
                    int icodEmple = ObtieneIcodCatEmple();
                    MuestraIframe(icodEmple);
                }
            }


        }
        private void MuestraIframe(int icodEmple)
        {
            string urlServer = "https://consumoind.herokuapp.com";
            string url = urlServer + "/ConsumoIndividual/Home/" + DSODataContext.Schema + "/" + icodEmple + "/" + fechaFactura + "";
            string iframe = "<iframe class='col-sm-12' style='height:800px;border-style:none;' src=\"" + url + "\"></iframe><br/>";
            iframeDiv.Controls.Add(
                new LiteralControl(
                   iframe
                    )
                );
        }
        private string ObtieneFechaMax()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" CONVERT(VARCHAR(7),MAX(CONVERT(DATE,FechaPub)),121)");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".ResumenFacturasdemoviles WITH(NOLOCK)");
            var fecmax = DSODataAccess.ExecuteScalar(query.ToString(), connStr);
            return fecmax.ToString();
        }
        private void ObtieneFechaFact()
        {
            string query = "EXEC ObtieneFechaMaximaFactura @Schema = '" + esquema + "',@Carrier = 'TELCEL'";
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                ValidaSelectCombo(dr["Mes"].ToString(), cboMes);
            }
        }
        private int ObtieneIcodCatEmple()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" E.icodCatalogo");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','Español')] AS U");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[vishistoricos('Emple','Empleados','Español')]AS E");
            query.AppendLine(" ON U.icodCatalogo = E.Usuar");
            query.AppendLine(" AND E.dtIniVigencia<> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE U.icodCatalogo = " + iCodUsuario + "");

            var icodEmple = DSODataAccess.ExecuteScalar(query.ToString(), connStr);
            return Convert.ToInt32(icodEmple);
        }
        [WebMethod]
        public static object GetEmple(string texto)
        {
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT");
            query.AppendLine(" E.iCodCatalogo AS ID,");
            query.AppendLine(" Nomcompleto AS Descripcion");
            query.AppendLine(" FROM "+DSODataContext.Schema+".HistEmple AS E WITH(NOLOCK)");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Linea','Español')] AS REL WITH(NOLOCK)");
            query.AppendLine(" ON E.iCodCatalogo = REL.Emple");
            query.AppendLine(" AND REL.dtIniVigencia <> REL.dtFinVigencia");
            query.AppendLine(" AND REL.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".HistLinea AS L WITH(NOLOCK)");
            query.AppendLine(" ON REL.Linea = L.iCodCatalogo");
            query.AppendLine(" AND L.dtIniVigencia <> L.dtFinVigencia");
            query.AppendLine(" AND L.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Nomcompleto + L.Tel LIKE '%" + texto + "%'");
            query.AppendLine(" GROUP BY E.iCodCatalogo,Nomcompleto");

            DataTable dtJefe = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtJefe);
        }
        private DataTable GetDataDropDownList(string clave)
        {
            ObtieneAnio();
            StringBuilder query = new StringBuilder();
            bool isEstatus = false;
            query.Length = 0;
            query.AppendLine("  SELECT [CAMPOS]");
            query.AppendLine("  FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("  WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");
            query = query.Replace("[CAMPOS]", "CASE WHEN LEN(VCHCODIGO) = 1 THEN '0' + VCHCODIGO ELSE VCHCODIGO END AS vchCodigo, UPPER(Español) AS Descripcion");
            query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
            return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
        }
        private DataTable AddRowDefault(DataTable dt, bool estatus)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow rowExtra = dt.NewRow();
                rowExtra["vchCodigo"] = 0;
                rowExtra["Descripcion"] = !estatus ? "TODOS" : "Seleccionar";
                dt.Rows.InsertAt(rowExtra, 0);
            }
            return dt;
        }
        public void ObtieneAnio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo AS vchCodigo, vchDescripcion AS Descripcion FROM " + esquema + ".[VisHistoricos('Anio','Años','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion IN(DATEPART(YEAR, GETDATE()),DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())), DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())))");
            query.AppendLine(" ORDER BY vchDescripcion DESC");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                cboAnio.DataSource = dt;
                cboAnio.DataBind();
            }
        }
        private void ValidaSelectCombo(string valor, DropDownList cbo)
        {
            string itemToCompare = string.Empty;
            string itemOrigin = valor.ToUpper();
            foreach (ListItem item in cbo.Items)
            {
                itemToCompare = item.Text.ToUpper();
                if (itemOrigin == itemToCompare)
                {
                    cbo.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
            if(hdfIcodEmple.Value != null && hdfIcodEmple.Value != "" )
            {
                if (Convert.ToInt32(mes) > 0)
                {
                    DateTime F = Convert.ToDateTime((anio + "-" + mes + "-" + "01"));
                    string fechaInicio = F.ToString("yyyy-MM");
                    fechaFactura = fechaInicio;
                    int icodEmple = Convert.ToInt32(hdfIcodEmple.Value);
                    MuestraIframe(icodEmple);
                    //txtEmple.Text = "";
                }
            }            
        }
    }
}