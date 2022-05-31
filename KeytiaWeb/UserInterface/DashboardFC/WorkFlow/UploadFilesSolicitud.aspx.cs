using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class UploadFilesSolicitud : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int perfil;
        int iCodUsuario;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            perfil = (int)Session["iCodPerfil"];
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                CreaRepositorio();
            }


            //RM 20190312 se oculta el mensaje del domicilio de afirme para cualquier otro cliente
            if (DSODataContext.Schema.ToUpper() != "K5AFIRME")
            {
                pnlInfo.Visible = false;
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtFolio.Text != "")
            {
                int solicitud = Convert.ToInt32(txtFolio.Text);
                ValidaSolicitud(perfil, iCodUsuario, solicitud);
            }
            else
            {
                MuestraMensaje("Mensaje!", "Ingrese Un Numero de Folio");
            }
        }
        #region METODOS
        public void ValidaSolicitud(int perfil, int usuario, int solicitud)
        {
            try
            {
                DataTable dt = ValidaSolicitudes(perfil, iCodUsuario, solicitud);
                if (dt != null && dt.Rows.Count > 0)
                {
                    rowUploadFiles.Visible = true;
                    DataRow dr = dt.Rows[0];
                    int banderaIne = Convert.ToInt32(dr["Ine"]);
                    int banderaCarta = Convert.ToInt32(dr["Carta"]);


                    if (banderaCarta == 0)
                    {
                        rowCarta.Visible = true;
                    }
                    if (banderaIne == 0)
                    {
                        rowIne.Visible = true;
                    }
                }
                else
                {
                    MuestraMensaje("Solicitud!", "No Existen Solicitudes con el Número de Folio : " + solicitud);
                    txtFolio.Text = "";
                }
            }
            catch
            {
                MuestraMensaje("Error!", "Ocurrio un Error!");
                throw;
            }
        }
        public void MuestraMensaje(string mensajeHeader, string mensajeBody)
        {
            lblTituloModalMsn.Text = mensajeHeader;
            lblBodyModalMsn.Text = mensajeBody;
            mpeEtqMsn.Show();
        }
        
        public void UploadFiles()
        {
            string pathClienteIne = Diccionario.ine + esquema + "//INE";
            string pathClienteCarta = Diccionario.cartaResponsiva + esquema + "//CartaResponsiva";
            int solicitud = Convert.ToInt32(txtFolio.Text);
            int cargaIne = 0;
            int cargaCarta = 0;
            string mensajeArchivos = "";
            /*cuando los dos controles estan activos*/
            if (rowIne.Visible == true && rowCarta.Visible == true)
            {
                /*se muestra un mensaje cuando no se a elegido algun archivo*/
                if (!file.HasFile && !FileCarta.HasFile)
                {
                    MuestraMensaje("Error!", "Debe Elegir los Dos Documentos.");
                    return;
                }
                else if (!file.HasFile || !FileCarta.HasFile)
                {
                    MuestraMensaje("Error!", "Debe Elegir los Dos Documentos");
                   
                    return;
                }

                if (file.HasFile && FileCarta.HasFile)
                {
                    if (file.HasFile)
                    {
                        string pathIne = CargaIne(pathClienteIne);
                        if (pathIne != "")
                        {
                            int resultado = GuardaArchivos(solicitud, pathIne, "INE");
                            if (resultado == 0)
                            {
                                cargaIne = 0;
                                mensajeArchivos = "al Cargar la INE";

                            }
                            else
                            {
                                cargaIne = 1;
                            }

                        }
                    }

                    if (FileCarta.HasFile)
                    {
                        string pathCarta = CargaCartaResponsiva(pathClienteCarta);
                        if (pathCarta != "")
                        {
                            int res = GuardaArchivos(solicitud, pathCarta, "CartaResponsiva");
                            if (res == 0)
                            {
                                cargaCarta = 0;
                                mensajeArchivos +=  " al Cargar la CartaResponsiva";
                            }
                            else
                            {
                                cargaCarta = 1;
                            }
                        }
                    }
                }

                
                if(cargaCarta == 1 && cargaIne == 1)
                {
                    MuestraMensaje("Mensaje!", "Los Archivos se Cargaron Correctamente");
                    rowUploadFiles.Visible = false;
                    rowIne.Visible = false;
                    rowCarta.Visible = false;
                    txtFolio.Text = "";
                }
                else
                {
                    MuestraMensaje("Error!","Ocurrio un Error " + mensajeArchivos);
                    return;
                }
            }
            //else
            //{
            //    if (rowIne.Visible == true)
            //    {
            //        string pathIne = CargaIne(pathClienteIne);
            //        int resultado = GuardaArchivos(solicitud, pathIne, "INE");
            //        if (resultado > 0)
            //        {
            //            MuestraMensaje("Mensaje!", "El INE se Cargo Correctamente");
            //            rowUploadFiles.Visible = false;
            //            rowIne.Visible = false;
            //            txtFolio.Text = "";
            //        }
            //        else
            //        {
            //            MuestraMensaje("Error!", "Ocurrio Un Error al Cargar la INE");
            //        }
            //    }

            //    if (rowCarta.Visible == true)
            //    {
            //        string pathCarta = CargaCartaResponsiva(pathClienteCarta);
            //        int res = GuardaArchivos(solicitud, pathCarta, "CartaResponsiva");
            //        if (res > 0)
            //        {
            //            MuestraMensaje("Mensaje!", "La Carta Responsiva Se Cargo Correctamente");
            //            rowUploadFiles.Visible = false;
            //            rowCarta.Visible = false;
            //            txtFolio.Text = "";
            //        }
            //        else
            //        {
                        
            //            MuestraMensaje("Error!", "Ocurrio Un Error al Cargar la CartaResponsiva");
            //        }
            //    }
            //}
        }
        public string CargaIne(string pathIne)
        {
            var allowedExtensions = new string[] { "jpg", "png", "pdf" };
            string pathArchivo = "";

            if(file.HasFile)
            {
                var extension = Path.GetExtension(file.PostedFile.FileName).ToLower().Replace(".", "");
                try
                {
                    string filename = Path.GetFileName(file.FileName);
                    if (allowedExtensions.Contains(extension))
                    {
                        if (file.PostedFile.ContentLength <= 10485760)
                        {
                            file.SaveAs(Server.MapPath(pathIne + "//") + filename);
                            /*Retornar el path del Archivo para guardarlo en la BD*/
                            pathArchivo = Server.MapPath(pathIne + "//") + filename;
                        }
                        else
                        {
                            MuestraMensaje(Diccionario.ErrorTamañoArchivo,Diccionario.MensajeTamañoArchivo);
                            pathArchivo = "";
                        }
                    }
                    else
                    {
                        MuestraMensaje(Diccionario.ErrorTipodeArchivo,Diccionario.MensajeTipoArch);
                        pathArchivo = "";
                    }
                }
                catch
                {
                    MuestraMensaje(Diccionario.MEN001, Diccionario.MEN003);
                    pathArchivo = "";
                }
            }
            else
            {
                MuestraMensaje(Diccionario.MEN001, Diccionario.MEN002);
                pathArchivo = "";
            }

            return pathArchivo;

        }
        public string CargaCartaResponsiva(string pathCarta)
        {
            var allowedExtensions = new string[] { "jpg", "png", "pdf" };
            string pathArchivo = "";
            if (FileCarta.HasFile)
            {
                var extension = Path.GetExtension(FileCarta.PostedFile.FileName).ToLower().Replace(".", "");
                try
                {
                    string filename = Path.GetFileName(FileCarta.FileName);
                    if (allowedExtensions.Contains(extension))
                    {
                        if (FileCarta.PostedFile.ContentLength <= 10485760)
                        {
                            FileCarta.SaveAs(Server.MapPath(pathCarta + "//") + filename);
                            /*Retornar el path del Archivo para guardarlo en la BD*/
                            pathArchivo = Server.MapPath(pathCarta + "//") + filename;
                        }
                        else
                        {
                            MuestraMensaje(Diccionario.ErrorTamañoArchivo, Diccionario.MensajeTamañoArchivo);
                            pathArchivo = "";
                        }
                    }
                    else
                    {
                        MuestraMensaje(Diccionario.ErrorTipodeArchivo, Diccionario.MensajeTipoArch);
                        pathArchivo = "";
                    }
                }
                catch
                {
                    MuestraMensaje(Diccionario.MEN001, Diccionario.MEN003);
                    pathArchivo = "";
                }
            }
            else
            {
                MuestraMensaje(Diccionario.MEN001, Diccionario.MEN002);
                pathArchivo = "";
            }
            return pathArchivo;

        }
        private void CreaRepositorio()
        {

            string ine = Server.MapPath(Diccionario.ine.ToString());
            string cartaResponsiva = Server.MapPath(Diccionario.cartaResponsiva.ToString());

            //CartaResponsiva
            //INE
            string ClienteIne = ine + esquema + "//INE";
            string ClienteCarta = cartaResponsiva + esquema + "//CartaResponsiva";
            try
            {
                if ((!Directory.Exists(ine)) && (!Directory.Exists(cartaResponsiva)))
                {
                    Directory.CreateDirectory(ine);
                    Directory.CreateDirectory(cartaResponsiva);


                    if ((!Directory.Exists(ClienteIne)) && (!Directory.Exists(ClienteCarta)))
                    {
                        Directory.CreateDirectory(ClienteIne);
                        Directory.CreateDirectory(ClienteCarta);
                    }

                }
                else
                {

                    if ((!Directory.Exists(ClienteIne)) && (!Directory.Exists(ClienteCarta)))
                    {
                        Directory.CreateDirectory(ClienteIne);
                        Directory.CreateDirectory(ClienteCarta);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion METODOS
        #region QUERYS
        private int  GuardaArchivos(int solicitud, string path,string ClaveArchivo)
        {
            DataTable dt = new DataTable();
            int resultado = 0;
            string sp = " EXEC WorkFlowLineasInsertaArchivosSolicitud @Esquema = '{0}', @IdSolicitud = {1}, @RutaARchivo = '{2}', @ClaveArchivo = '{3}'";
            string query = string.Format(sp, DSODataContext.Schema, solicitud, path, ClaveArchivo);
            dt = DSODataAccess.Execute(query, connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                resultado = Convert.ToInt32(dr["Correcto"]);
            }
            return resultado =  1;
        }
        public DataTable ValidaSolicitudes(int perfil, int usuario, int solicitud)
        {
            DataTable dt = new DataTable();
            string sp = "EXEC WorkFlowLineasGetSolicitud @Esquema = '{0}', @ClaveSolicitud = {1}, @Usuario = {2},@Perfil = {3}";
            string query = string.Format(sp, DSODataContext.Schema, solicitud, usuario, perfil);
            dt = DSODataAccess.Execute(query, connStr);

            return dt;
        }
        #endregion QUERYS

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (txtFolio.Text != "")
            {
                UploadFiles();
            }
            else
            {
                MuestraMensaje("Mensaje!", "Ingrese Un Numero de Folio");
            }
        }
    }
}