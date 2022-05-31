using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface
{
    public partial class UploadFiles : System.Web.UI.Page
    {
        private string repositorio = string.Empty;
        private string proyecto = string.Empty;
        private string path = string.Empty;
        StringBuilder query = new StringBuilder();
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        DataTable dtUsuarios = new DataTable();
        List<EmailsEnvio> CuentaCorreos = new List<EmailsEnvio>();
        List<PathArchivos> pathArchivos = new List<PathArchivos>();
        protected string psMailRemitente = "";
        protected string psNomRemitente = "";
        private string usuario;
        int catUsuario;

        protected void Page_Load(object sender, EventArgs e)
        {
            repositorio = Request.QueryString["p"];
            proyecto = Request.QueryString["pr"];
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            usuario = Session["vchCodUsuario"].ToString();
            catUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                IniciaProceso();
            }
        }

        private void IniciaProceso()
        {
            try
            {
                CreaRepositorio();

                switch (repositorio)
                {
                    case "cliente":
                        this.CargaArchivo.Visible = false;
                        break;
                    case "consultor":
                        this.CargaArchivo.Visible = true;
                        cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
                        cboAnio.DataBind();
                        cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
                        cboMes.DataBind();
                        cboCategoria.DataSource = ObtieneCategoria(0);
                        cboCategoria.DataBind();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        protected void UploadButton_Click(object sender, EventArgs e)
        {

            try
            {
                string pathArchivo = "";

                switch (repositorio)
                {
                    case "cliente":
                        path = "~/UploadFiles//Client//" + esquema;
                        break;
                    case "consultor":
                        path = "~/UploadFiles//DtiStaff//" + esquema;
                        break;
                }

                DataTable dt = ObtieneCategoria(Convert.ToInt32(cboCategoria.SelectedValue));

                DataRow dr = dt.Rows[0];
                string tipoArchivo = dr["TipoArchivo"].ToString();
                try
                {
                    pathArchivo = UploadFile(path, ref pathArchivos, tipoArchivo);
                    if (pathArchivo != "")
                    {
                        if (File.Exists(pathArchivo))
                        {
                            ObtieneDestinatariosEmail(proyecto, repositorio, ref CuentaCorreos);
                            string descCategoria = "";
                            string descAnio = "";
                            string mesDesc = "";
                            bool envioTodosMails = true;
                            if (repositorio == "consultor")
                            {
                                string fechaCarga = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
                                string descripcion = txtDescripcion.Text;
                                int categoria = Convert.ToInt32(cboCategoria.SelectedValue);
                                int mes = Convert.ToInt32(cboMes.SelectedValue);
                                int anio = Convert.ToInt32(cboAnio.SelectedValue);

                                descCategoria = cboCategoria.SelectedItem.ToString();
                                descAnio = cboAnio.SelectedItem.ToString();
                                mesDesc = cboMes.SelectedItem.ToString();
                                envioTodosMails = EnviaEmail(CuentaCorreos, ref pathArchivos, descCategoria, descAnio, mesDesc, 1);

                                if (envioTodosMails)
                                {
                                    InsertaArchivoDescargable(ref pathArchivos, fechaCarga, descripcion, categoria, descCategoria, mes, anio);
                                    txtDescripcion.Text = "";
                                }
                                else
                                {
                                    StatusLabel.Text = "Ocurrio Un Error al Cargar El archivo intente mas tarde..";

                                    if (File.Exists(pathArchivo))
                                    {
                                        File.Delete(pathArchivo);
                                    }
                                    return;
                                }
                            }
                            else
                            {
                                if (envioTodosMails)
                                {
                                    EnviaEmail(CuentaCorreos, ref pathArchivos, descCategoria, descAnio, mesDesc, 0);
                                    txtDescripcion.Text = "";
                                }
                                else
                                {
                                    txtDescripcion.Text = "Ocurrio Un Error al enviar los correos a los destinatarios.";
                                    return;
                                }
                            }

                        }
                        else
                        {
                            txtDescripcion.Text = "Ocurrio un error al cargar el archivo intente mas tarde.";
                            return;
                        }
                    }
                }
                catch( Exception ex)
                {
                    /*Ocurrio un error al cargar el archivo intente mas tarde*/
                    txtDescripcion.Text = "Ocurrio un error al cargar el archivo intente mas tarde.";
                    KeytiaServiceBL.Util.LogException("Surgió un error en la aplicación Web.", ex);
                    return;
                }
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Surgió un error en la aplicación Web.", ex);
                throw ex;
            }
        }

        private void CreaRepositorio()
        {

            string Cliente = Server.MapPath("~/UploadFiles//Client//");
            string dtiStaff = Server.MapPath("~/UploadFiles//DtiStaff//");


            string ClienteEsquema = Cliente + esquema;
            string dtiStaffEsquema = dtiStaff + esquema;
            try
            {
                if ((!Directory.Exists(Cliente)) && (!Directory.Exists(dtiStaff)))
                {
                    Directory.CreateDirectory(Cliente);
                    Directory.CreateDirectory(dtiStaff);


                    if ((!Directory.Exists(ClienteEsquema)) && (!Directory.Exists(dtiStaffEsquema)))
                    {
                        Directory.CreateDirectory(ClienteEsquema);
                        Directory.CreateDirectory(dtiStaffEsquema);
                    }

                }
                else
                {

                    if ((!Directory.Exists(ClienteEsquema)) && (!Directory.Exists(dtiStaffEsquema)))
                    {
                        Directory.CreateDirectory(ClienteEsquema);
                        Directory.CreateDirectory(dtiStaffEsquema);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string UploadFile(string path, ref List<PathArchivos> pathArchivos, string tipoArchivo)
        {

            var allowedExtensions = new string[] { "doc", "docx", "pdf", "rar", "xls", "xlsx", "zip", "txt" };
            string extensionesArchivos = tipoArchivo;
            var arrayExten = extensionesArchivos.Split(',');

            string pathArchivo = string.Empty;

            if (FileUploadControl.HasFile)
            {
                var extension = Path.GetExtension(FileUploadControl.PostedFile.FileName).ToLower().Replace(".", "");
                try
                {
                    PathArchivos arch = new PathArchivos();
                    string filename = Path.GetFileName(FileUploadControl.FileName);
                    int valor = Convert.ToInt32(ValidaExisteArchivo(filename));
                    if (valor == 0)
                    {
                        if (arrayExten.Contains(extension))
                        {
                            if (FileUploadControl.PostedFile.ContentLength <= 11485760)//10485760
                            {
                                FileUploadControl.SaveAs(Server.MapPath(path + "//") + filename);

                                StatusLabel.Text = "Estatus de Carga: Archivo Cargado Exitosamente !";

                                arch.File = filename;
                                arch.FullPath = Server.MapPath(path + "//") + filename;
                                arch.claveArchivo = filename;
                                pathArchivos.Add(arch);

                                return pathArchivo = Server.MapPath(path + "//") + filename;
                            }
                            else
                            {
                                StatusLabel.Text = "Estatus de Carga: El Archivo debe ser Menor o Igual A 10.1 Mb";
                                return pathArchivo = "";
                            }
                        }
                        else
                        {
                            StatusLabel.Text = "Estatus de Carga: Formato de Archivo Incorrecto!";
                            return pathArchivo = "";
                        }
                    }
                    else
                    {
                        StatusLabel.Text = "El Archivo '" + filename + "', ya Existe, Favor de Cambiar el Nombre al Archivo! ";
                        return pathArchivo = "";
                    }

                }
                catch (Exception ex)
                {
                    StatusLabel.Text = "Estatus de Carga : Ocurrio un Error al Subir el Archivo: " + ex.Message;

                    return pathArchivo = "";
                }
            }
            else
            {
                StatusLabel.Text = "Debe de Selecionar un Archivo.";
                return pathArchivo = "";
            }
        }

        private DataTable GetDataDropDownList(string clave)
        {
            bool isEstatus = false;
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");

            #region Filtro
            switch (clave.ToUpper())
            {

                case "ANIO":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Anio','Años','Español')]");
                    query.AppendLine(" AND CONVERT(INT, vchDescripcion) >= 2016 AND CONVERT(INT, vchDescripcion) <= YEAR(GETDATE())");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                case "MES":
                    query = query.Replace("[CAMPOS]", "iCodCatalogo, Español AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                default:
                    return new DataTable();
            }

            #endregion
        }

        private DataTable AddRowDefault(DataTable dt, bool estatus)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow rowExtra = dt.NewRow();
                rowExtra["iCodCatalogo"] = 0;
                rowExtra["Descripcion"] = !estatus ? "TODOS" : "Seleccionar";
                dt.Rows.InsertAt(rowExtra, 0);
            }
            return dt;
        }

        private DataTable ObtieneCategoria(int categoria)
        {
            StringBuilder query = new StringBuilder();
            DataTable dt = new DataTable();
            try
            {
                query.AppendLine(" SELECT");
                query.AppendLine(" iCodCatalogo, NombreCategoria, TipoArchivo ");
                query.AppendLine(" FROM " + esquema + ".[VisHistoricos('CategoriaArchivoDescargable','Categorías Archivo Descargable','Español')] ");
                query.AppendLine(" WHERE dtinivigencia<> dtfinvigencia AND dtfinvigencia >= GETDATE() ");
                if (categoria > 0)
                {
                    query.AppendLine("AND iCodCatalogo = " + categoria + " ");
                }
                dt = DSODataAccess.Execute(query.ToString(), connStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }
        private void ObtieneDestinatariosEmail(string proyecto, string claveParametro, ref List<EmailsEnvio> CuentaCorreos)
        {
            StringBuilder query = new StringBuilder();
            DataTable dt = new DataTable();
            try
            {
                query.AppendLine(" SELECT ");
                query.AppendLine(" PROCESS.DestinatarioEmail,");
                query.AppendLine(" PROCESS.Email, ");
                query.AppendLine(" ENVIO.MensajeEnCorreo ");
                query.AppendLine(" FROM " + esquema + ".[VisRelaciones('Proceso Envío - Destinatario','Español')] AS REL");
                query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('ProcesoEnvioDestinatario','Procesos Envío Destinatarios','Español')] AS PROCESS ");
                query.AppendLine(" ON REL.ProcesoEnvioDestinatario = PROCESS.iCodCatalogo ");
                query.AppendLine(" AND PROCESS.dtIniVigencia<> PROCESS.dtFinVigencia ");
                query.AppendLine(" AND PROCESS.dtFinVigencia >= GETDATE() ");
                query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('ProcesoConEnvio','Procesos Con Envío','Español')] AS ENVIO ");
                query.AppendLine(" ON ENVIO.iCodCatalogo =  REL.ProcesoConEnvio ");
                query.AppendLine(" AND ENVIO.dtIniVigencia<> ENVIO.dtFinVigencia ");
                query.AppendLine(" AND ENVIO.dtFinVigencia >= GETDATE() ");
                query.AppendLine(" WHERE REL.dtIniVigencia<> REL.dtFinVigencia ");
                query.AppendLine(" AND REL.dtFinVigencia >= GETDATE() ");
                query.AppendLine(" AND REL.ProcesoConEnvioCod = '" + proyecto + "' AND PROCESS.CveNombre = '" + claveParametro + "'");


                dt = DSODataAccess.Execute(query.ToString(), connStr);

                if (dt.Rows.Count > 0 && dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {

                            string email = string.IsNullOrEmpty(dr["Email"].ToString()) ? "" : dr["Email"].ToString();
                            EmailsEnvio lis = new EmailsEnvio();

                            lis.Nombre = dr["DestinatarioEmail"].ToString();
                            lis.CuentaCorreo = email;
                            lis.MensajeCorreo = dr["MensajeEnCorreo"].ToString();
                            CuentaCorreos.Add(lis);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool EnviaEmail(List<EmailsEnvio> CuentaCorreos, ref List<PathArchivos> pathArchivos, string descCategoria, string descAnio, string mesDesc, int bandera)
        {
            bool enviadoCorrectamente = true;
            bool envioTodosMails = true;
            try
            {
                if (CuentaCorreos.Count > 0)
                {

                    foreach (var lista in CuentaCorreos)
                    {
                        try
                        {
                            string nombre = lista.Nombre.ToString();
                            string email = lista.CuentaCorreo.ToString();
                            string mensajeCorreo = lista.MensajeCorreo.ToString();
                            string notas = txtDescripcion.Text;
                            var list = pathArchivos[0];
                            string archivo = list.claveArchivo;
                            if (email != "")
                            {
                                enviadoCorrectamente = EnviarCorreo(nombre, email, mensajeCorreo, archivo, descCategoria, descAnio, mesDesc, bandera, notas);
                            }

                            if (!enviadoCorrectamente)
                            {
                                envioTodosMails = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Util.LogException("Surgió un error en la aplicación Web.", ex);
                            envioTodosMails = false;
                            break;
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                envioTodosMails = false;
            }
            return envioTodosMails;
        }
        private bool EnviarCorreo(string nombre, string email, string mensajeCorreo, string archivo, string descCategoria, string descAnio, string mesDesc, int bandera, string notas)
        {
            bool resultado = false;
            try
            {
                string lsEmail = email;
                string lsErrMailAsunto = Globals.GetMsgWeb("RecMailAsuntoPws");

                if (lsErrMailAsunto.StartsWith("#undefined-"))
                {
                    lsErrMailAsunto = "Carga de Nuevo Archivo";
                }

                StringBuilder body = new StringBuilder();

                body.AppendLine("<html>");
                body.AppendLine("<head>");
                body.AppendLine("   <title></title>");
                body.AppendLine("   <meta http-equiv='Content-Type' content='text/html; charset=utf-8' /> ");
                body.AppendLine("</head>");
                body.AppendLine("<body>");
                body.AppendLine("   <p>" + mensajeCorreo + "</p>");
                if (bandera == 1)
                {
                    body.AppendLine("   <p> El Archivo se cargo con las siguiente configuración:</p>");
                    body.AppendLine("   <p> Categoria: " + descCategoria + "</p>");
                    body.AppendLine("   <p> Año: " + descAnio + "</p>");
                    body.AppendLine("   <p> Mes: " + mesDesc + "</p>");
                }
                body.AppendLine("   <p> Nombre del Archivo: " + archivo + "</p>");
                body.AppendLine("   <p> Notas: " + notas + "</p> <br>");
                body.AppendLine("</body> ");
                body.AppendLine("</html>");

                MailAccess loMail = new MailAccess();
                loMail.NotificarSiHayError = false;
                loMail.IsHtml = true;
                loMail.De = getRemitente();
                loMail.Asunto = lsErrMailAsunto;
                loMail.Mensaje = body.ToString();
                loMail.Para.Add(lsEmail);

                if (loMail.ImagenesAgregadas == null)
                    loMail.ImagenesAgregadas = new System.Collections.Hashtable();

                loMail.Enviar();
                resultado = true;
                return resultado;

            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Surgió un error en la aplicación Web al enviar el correo.", ex);
                return resultado;
                throw ex;
            }
        }
        private System.Net.Mail.MailAddress getRemitente()
        {
            if (string.IsNullOrEmpty(psMailRemitente))
            {
                return new System.Net.Mail.MailAddress(Util.AppSettings("appeMailID"));

            }
            else if (string.IsNullOrEmpty(psNomRemitente))
            {
                return new System.Net.Mail.MailAddress(psMailRemitente);
            }
            else
            {
                return new System.Net.Mail.MailAddress(psMailRemitente, psNomRemitente);
            }
        }
        private void InsertaArchivoDescargable(ref List<PathArchivos> pathArchivos, string fechaCarga, string descripcion, int categoria, string descCategoria, int mes, int anio)
        {
            try
            {
                string vchCodigo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var lista = pathArchivos[0];
                string archivo = lista.claveArchivo;
                string fullPath = lista.FullPath;
                string sp = string.Empty;
                string execSp = " EXEC InsertaArchivoDescargable @esquema = '{0}', @archivo = '{1}',@mes ={2}, @anio ={3},@categoria ={4},@usuario ={5},@rutaArchivo ='{6}',@descripcion ='{7}',@fechaCarga ='{8}'";
                sp = string.Format(execSp, esquema, archivo, mes, anio, categoria, catUsuario, fullPath, descripcion, vchCodigo);
                DSODataAccess.ExecuteNonQuery(sp, connStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private object ValidaExisteArchivo(string nameFile)
        {

            StringBuilder query = new StringBuilder();
            query.AppendLine(" IF EXISTS(SELECT vchDescripcion FROM " + esquema + ".[Vishistoricos('ArchivoDescargable','Archivos Descargables','Español')]");
            query.AppendLine(" WHERE dtinivigencia <> dtfinvigencia AND dtfinvigencia >= GETDATE() ");
            query.AppendLine(" AND vchDescripcion = '" + nameFile + "') ");
            query.AppendLine(" BEGIN ");
            query.AppendLine(" SELECT 1 AS Existe ");
            query.AppendLine(" END ");
            query.AppendLine(" ELSE ");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SELECT 0 AS Existe ");
            query.AppendLine(" END ");

            var existe = DSODataAccess.ExecuteScalar(query.ToString(), connStr);

            return existe;
        }
    }
    public class EmailsEnvio
    {
        public string Nombre { get; set; }
        public string CuentaCorreo { get; set; }
        public string MensajeCorreo { get; set; }
    }
    public class PathArchivos
    {
        public string File { get; set; }
        public string FullPath { get; set; }
        public string claveArchivo { get; set; }
    }
}