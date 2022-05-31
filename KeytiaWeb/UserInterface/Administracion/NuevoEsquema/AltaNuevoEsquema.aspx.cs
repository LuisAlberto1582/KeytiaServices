using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardFC;
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

namespace KeytiaWeb.UserInterface.Administracion.NuevoEsquema
{
    public partial class AltaNuevoEsquema : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string path = "~/images/";
        static string nameServer;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            //Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                //nameServer = Environment.MachineName;
                //txtServerCargas.Text = nameServer;
                //txtDirCargas.Text = @"\\" + nameServer + @"\Archivos\Cargas\";
                ObtieneServerCargas();
            }

        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                InsertaRegistro();
                LimpiaControles();
                lblTituloModalMsn.Text = "Crear Nuevo Cliente";
                lblBodyModalMsn.Text = "La solicitud del Nuevo Esquema, se Registro Correctamente.";
                mpeEtqMsn.Show();
            }
            catch(Exception ex)
            {

            }
            
        }
        [WebMethod]
        public static object GetEsquema(string texto)
        {
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @Existe INT");
            query.AppendLine(" IF NOT EXISTS(");
            query.AppendLine(" SELECT Esquema FROM keytia.[vishiStoricos('UsuarDB','Usuarios DB','Español')]");
            query.AppendLine(" JOIN sys.schemas ON name = vchCodigo");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Esquema = '"+ texto+ "')");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SET @Existe = 0");
            query.AppendLine(" END");
            query.AppendLine(" ELSE");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SET @Existe = 1");
            query.AppendLine(" END");
            query.AppendLine(" SELECT @Existe AS Descripcion");
            DataTable dtEsquema = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtEsquema);
        }
        private string UploadFile()
        {
            string nameLogo = "";
            string extensionesArchivos = "JPG,PNG,GIF";
            var arrayExten = extensionesArchivos.Split(',');
            try
            {
                if (fUploadLogo.HasFile)
                {
                    var extension = Path.GetExtension(fUploadLogo.PostedFile.FileName).ToLower().Replace(".", "");
                    nameLogo = fUploadLogo.PostedFile.FileName.ToString();
                    if (File.Exists(path + nameLogo)) 
                        File.Delete(path + nameLogo);
                        if (arrayExten.Contains(extension))
                        {
                            fUploadLogo.SaveAs(Server.MapPath(path) + nameLogo);
                        }
                        else
                        {

                        }                  
                }
            }
            catch
            {

            }

            return nameLogo;
        }
        private void InsertaRegistro()
        {
            string esquema = txtEsquema.Text.Trim();
            string serverServicio = txtServerCargas.Text.Trim();
            string dirCargas = txtDirCargas.Text.Trim();
            string claveUser = txtClaveUser.Text.Trim();
            string passUser = KeytiaServiceBL.Util.Encrypt(txtPasswordUser.Text.Trim());
            string claveUserOper = txtClaveOper.Text.Trim();
            string passUserOper = KeytiaServiceBL.Util.Encrypt(txtPasswordUser.Text.Trim());
            string conectionString = Util.Encrypt("uid=" + txtEsquema.Text.Trim() + ";pwd=" + txtEsquema.Text.Trim() + ";{A}");
            string nameLogo = UploadFile();
            string rutaLogo = (nameLogo != "") ? nameLogo : esquema + ".jpg";

            string email = txtEmailSyop.Text.Trim();
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine(" EXEC dbo.InsertaCargaNuevoEsquema ");
                query.AppendLine(" @Esquema = '"+esquema+"',");
                query.AppendLine(" @ServidorServicio = '"+ serverServicio + "',");
                query.AppendLine(" @SaveFolder = '"+ dirCargas + esquema + "',");
                query.AppendLine(" @ClaveUsuarioConfig = '"+ claveUser + "',");
                query.AppendLine(" @PasswordUsuarioConfig = '"+ passUser + "',");
                query.AppendLine(" @NombreLogoCliente = '" + path + rutaLogo + "',");
                query.AppendLine(" @ClaveUsuarioSyOp = '"+ claveUserOper + "',");
                query.AppendLine(" @PasswordUsuarioSyop = '"+ passUserOper + "',");
                query.AppendLine(" @ConnStr = '"+ conectionString + "',");
                query.AppendLine(" @EmailUsuarioSyop = '"+ email + "'");
                
                DSODataAccess.ExecuteNonQuery(query.ToString());
            }
            catch(Exception ex)
            {
                throw ex;
            }        
        }
        private void ObtieneServerCargas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" ServidorServicio");
            query.AppendLine(" FROM Keytia.[VisHistoricos('UsuarDB','Usuarios DB','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND dtFinvigencia >= GETDATE()");
            query.AppendLine(" AND Esquema = 'Keytia'");

            DataTable dt = DSODataAccess.Execute(query.ToString());
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                txtServerCargas.Text = dr["ServidorServicio"].ToString();
                txtDirCargas.Text = @"\\" + dr["ServidorServicio"].ToString() + @"\Archivos\Cargas\";
            }
        }
        private void LimpiaControles()
        {
            ObtieneServerCargas();
            txtEsquema.Text = string.Empty;
            txtClaveUser.Text = string.Empty;
            txtPasswordUser.Text = string.Empty;
            txtClaveOper.Text = string.Empty;
            txtPassOper.Text = string.Empty;
            txtEmailSyop.Text = string.Empty;
        }
    }
}