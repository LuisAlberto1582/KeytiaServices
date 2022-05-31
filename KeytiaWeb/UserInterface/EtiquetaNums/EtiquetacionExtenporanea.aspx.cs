using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.EtiquetaNums
{
    public partial class EtiquetacionExtenporanea : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int iCodUsuario;
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Now.Day);
        protected void Page_Load(object sender, EventArgs e)
        {          
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            if (!Page.IsPostBack)
            {
                if (Session["Language"].ToString() == "Español")
                {
                    pdtInicio.setRegion("es");
                }

                //pdtInicio.MaxDateTime = DateTime.Today;
                pdtInicio.CreateControls();
                pdtInicio.DataValue = (object)fechaInicio;
                fechaInicio = Convert.ToDateTime(pdtInicio.Date);
                //IniciaProceso();
            }
        }
        [WebMethod]
        public static object GetEmploye(string texto)
        {
            DataTable Emple = new DataTable();
            string connStr = DSODataContext.ConnectionString;
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 100 iCodCatalogo,NominaA +' - '+ NomCompleto AS Nomcompleto FROM FCA.HistEmple WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND NomCompleto LIKE '%" + texto + "%' OR NominaA LIKE '%" + texto + "%'");
            Emple = DSODataAccess.Execute(query.ToString(), connStr);
            DataView dvldt = new DataView(Emple);
            Emple = dvldt.ToTable(false, new string[] { "NomCompleto", "iCodCatalogo" });
            Emple.Columns["NomCompleto"].ColumnName = "Nombre";
            Emple.Columns["iCodCatalogo"].ColumnName = "Clave";
            return FCAndControls.ConvertDataTabletoJSONString(Emple);
        }
        private void ObtieneDatosEmpleado(int emId)
        {
            DataTable dt = ObtieneDatosEmple(emId);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                var numDepto = dr["vchCodigo"].ToString().Substring(4).Replace("-", "").Trim();
                txtNomEmple.Text = dr["NomCompleto"].ToString();//dr["em_apPaterno"].ToString() + " " + dr["em_apMaterno"].ToString() + " " + dr["em_nombre"].ToString();
                txtNumDepto.Text = numDepto;
                txtDepartamento.Text = dr["Descripcion"].ToString().ToUpper();//dr["cr_departamento"].ToString();
                var externo = dr["TipoEmCod"].ToString();//dr["em_recursos"].ToString();
                //Si el empleado es Externo Buscamos su Centro de Costo
                //Los empleados solo tienen departamemto
                if (externo == "X")
                {
                    txtNumEmpleado.Text = "00000";
                    rowCencos.Visible = true;
                    rowIdCencos.Visible = true;
                    //rowLocalidad.Visible = false;
                    //rowNumLocali.Visible = false;
                    ObtieneDatosExterno(emId);
                }
                else
                {
                    rowCencos.Visible = false;
                    rowIdCencos.Visible = false;
                    rowLocalidad.Visible = true;
                    rowNumLocali.Visible = true;
                    ObtieneLocalidadEmple(dr["vchCodigo"].ToString().Substring(0, 4));
                    txtNumEmpleado.Text = dr["NominaA"].ToString();
                }
                ObtieneLocalidadEmple(dr["vchCodigo"].ToString().Substring(0, 4));
            }

        }
        private DataTable ObtieneDatosEmple(int emId)
        {
            /*MODFICAR LA CONSULTA PARA QUE VALIDE CON EL HISTORICO DE USUARIOS*/
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" ISNULL(RFC,'') AS RFC,");
            query.AppendLine(" TE.vchCodigo AS TipoEmCod,");
            query.AppendLine(" NominaA,");
            query.AppendLine(" NomCompleto,");
            query.AppendLine(" C.vchCodigo,");
            query.AppendLine(" C.Descripcion");
            query.AppendLine(" FROM " + esquema + ".HistEmple AS E WITH(NOLOCK)");/*VARIABLE ESQUEMA*/
            query.AppendLine(" JOIN " + esquema + ".HistCenCos AS C WITH(NOLOCK)");
            //query.AppendLine(" FROM " + esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E WITH(NOLOCK)");/*VARIABLE ESQUEMA*/
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] AS C WITH(NOLOCK)");
            query.AppendLine(" ON E.CenCos = C.iCodCatalogo");
            query.AppendLine(" AND C.dtIniVigencia <> C.dtFinVigencia");
            query.AppendLine(" AND C.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('TipoEm','Tipo Empleado','Español')]AS TE");
            query.AppendLine(" ON E.TipoEm = TE.iCodCatalogo");
            query.AppendLine(" AND TE.dtIniVigencia<> TE.dtFinVigencia");
            query.AppendLine(" AND TE.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND E.iCodCatalogo = " + emId + "");/*VARIABLE EMPLEADO*/

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;
        }
        private void ObtieneDatosExterno(int emId)
        {

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 1");
            query.AppendLine(" CENCOSTO.vchCodigo,");
            query.AppendLine(" CENCOSTO.Descripcion");
            query.AppendLine(" FROM " + esquema + ".[visRelaciones('FCA CentroCosto-Externo','Español')] AS EXTERN WITH(NOLOCK)");
            query.AppendLine(" JOIN " + esquema + ".HistCenCos AS CENCOSTO WITH(NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] AS CENCOSTO WITH(NOLOCK)");
            query.AppendLine(" ON EXTERN.CenCos = CENCOSTO.iCodCatalogo");
            query.AppendLine(" AND CENCOSTO.dtIniVigencia <> CENCOSTO.dtFinVigencia");
            query.AppendLine(" AND CENCOSTO.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE EXTERN.dtIniVigencia <> EXTERN.dtFinVigencia");
            query.AppendLine(" AND EXTERN.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND EXTERN.Emple=" + emId + " ");
            //query.AppendLine(" ORDER BY FCANumeroLocalidad ASC ");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                txtCencos.Text = dr["Descripcion"].ToString();
                txtNumCencos.Text = dr["vchCodigo"].ToString();
            }
            else
            {
                txtCencos.Text = "N/A";
                txtNumCencos.Text = "N/A";
            }
        }
        private void ObtieneLocalidadEmple(string numDepto)
        {
            //SE OBTIENE LA LOCALIDAD CON EL ID MENOR QUE COINCIDA CON LOS PRIMEROS 4 DIGITOS DEL CENTRO DE COSTOS
            //SE HACE DE ESTA FORMA PUES PUEDE HABER VARIAS LOCALIDADES QUE CUMPLAN CON ESTA CONDICION

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo,");
            query.AppendLine(" FCANumeroLocalidad,");
            query.AppendLine(" FCANombreLocalidad");
            query.AppendLine(" FROM " + esquema + ".[VisHistoricos('FCALocalidad','Localidades FCA','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND SUBSTRING(FCANumeroLocalidad,0,5)= '" + numDepto + "'");
            query.AppendLine("ORDER BY FCANumeroLocalidad ASC");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                hfLocaliId.Value = dr["iCodCatalogo"].ToString();
                txtNumLocali.Text = dr["FCANumeroLocalidad"].ToString().Substring(0, 4);
                txtLocalidad.Text = dr["FCANombreLocalidad"].ToString();
            }
            else
            {
                hfLocaliId.Value = "0";
                txtLocalidad.Text = "N/A";
                txtNumLocali.Text = "N/A";
            }

        }
        private void AltaEmpleado(int claveEmple,string nomEmple, string fecha, int usuario)
        {
            try
            {
                string sp = "EXEC InsertaEmpleTiempoEngraciaFCA @icodEmpleado = {0},@Nombre = '{1}',@InicioVigencia = '{2}',@icodUsuario = {3}";
                string query = string.Format(sp, claveEmple, nomEmple, fecha, usuario);
                DSODataAccess.ExecuteNonQuery(query.ToString(), connStr);
            }
            catch
            {
                return;
            }
        }
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtBusqueda.Text != "")
            {
                detalleEmpleados.Visible = true;
                int icodEmple = Convert.ToInt32(txtEmpleId.Value);
                ObtieneDatosEmpleado(icodEmple);
                rowBotones.Visible = true;
            }
            else
            {
                lblTituloModalMsn.Text = "¡Mensaje!";
                lblBodyModalMsn.Text = "Debe de Seleccionar un empleado.";
                mpeEtqMsn.Show();
                return;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {               
                    int icodEmple = Convert.ToInt32(txtEmpleId.Value);
                    string nomEmple = txtNomEmple.Text;
                    int claveUser = iCodUsuario;
                    fechaInicio = Convert.ToDateTime(pdtInicio.Date);
                    string fecha = fechaInicio.ToString("yyyy-MM-dd");

                AltaEmpleado(icodEmple, nomEmple, fecha, claveUser);

                    rowBotones.Visible = false;
                    detalleEmpleados.Visible = false;
                    txtBusqueda.Text = "";

                lblTituloModalMsn.Text = "!Mensaje¡";
                lblBodyModalMsn.Text = "El Empleado Se Registro Correctamente.";
                mpeEtqMsn.Show();
            }
            catch
            {
                lblTituloModalMsn.Text = "Error!!";
                lblBodyModalMsn.Text = "Ocurrio Un Error al Guardar el empleado, intentelo mas tarde.";
                mpeEtqMsn.Show();
                rowBotones.Visible = false;
                detalleEmpleados.Visible = false;
                return;
            }
            

        }
    }
}