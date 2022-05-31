using KeytiaWeb.InvContratos.App_Code.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaWeb.WSSS;
using System.IO;
using KeytiaServiceBL;

namespace KeytiaWeb.InvContratos.Pantallas
{
    public partial class Pantallas_CargaArchivos : System.Web.UI.Page
    {
        string procedimiento = String.Empty;
        string folioAnexo, folio, folioConvenio, url;
        int Anexo, Contrato, Convenio;

        protected void Page_Load(object sender, EventArgs e)
        {
            folioAnexo = String.IsNullOrEmpty(Request.QueryString["folioAnexo"]) ? "" : Request.QueryString["folioAnexo"].ToString();
            Anexo = Int32.Parse(String.IsNullOrEmpty(Request.QueryString["Anexo"]) ? "" : Request.QueryString["Anexo"].ToString());
            folio = String.IsNullOrEmpty(Request.QueryString["folio"]) ? "" : Request.QueryString["folio"].ToString();
            Contrato = Int32.Parse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? "" : Request.QueryString["Id"].ToString());
            folioConvenio = String.IsNullOrEmpty(Request.QueryString["folioConvenio"]) ? "" : Request.QueryString["folioConvenio"].ToString();
            Convenio = Int32.Parse(String.IsNullOrEmpty(Request.QueryString["Convenio"]) ? "" : Request.QueryString["Convenio"].ToString());
            url = String.IsNullOrEmpty(Request.QueryString["Url"]) ? "" : Request.QueryString["Url"].ToString();
            Session["Url"] = url;
            if (!String.IsNullOrEmpty(folioAnexo))
            {
                procedimiento = "Anexo";
                lblFolio.Text = "Detalle Anexo: ";
                txtFolio.Text = folioAnexo;
            }
            else
            {
                if (!String.IsNullOrEmpty(folioConvenio))
                {
                    procedimiento = "Convenio";
                    lblFolio.Text = "Detalle Convenio: ";
                    txtFolio.Text = folioConvenio;
                }
                else
                {
                    procedimiento = "Contrato";
                    lblFolio.Text = "Detalle Contrato: ";
                    txtFolio.Text = folio;
                }
            }
        }

        public void CargarArchivos(object sender, EventArgs e)
        {
            string archivo = FileUploadControl.FileName.ToString();
            int tam_var = archivo.Length;
            string extension = archivo.Substring((tam_var - 4), 4);
            int IdTipoDocumento = 0;
            WebServiceSoapClient servicio = new WebServiceSoapClient();
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String result;

            using (WebClient client = new WebClient())
            {
                List<InvContTipoDocumento> tipo = new List<InvContTipoDocumento>();
                string JSON = servicio.DevuelveTipoDocumentoJSON(extension, Util.Encrypt(DSODataContext.ConnectionString));
                tipo = (new JavaScriptSerializer()).Deserialize<List<InvContTipoDocumento>>(JSON);
                if (tipo.Count > 0)
                {
                    IdTipoDocumento = tipo[0].Id;
                }
            }
            if (IdTipoDocumento > 0)
            {
                if (procedimiento == "Anexo")
                {
                    String path = @"D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";
                    if (Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path + "/Anexo " + folioAnexo + "/");
                        String path2 = "Anexo " + folioAnexo + "/" + FileUploadControl.FileName;

                        FileUploadControl.PostedFile.SaveAs(path + path2);

                        //Para tomar la ruta de archivo y guardarlo en la tabla
                        string path4 = path + path2;
                        Session["Archivo"] = path4;
                    }
                    else
                    {
                        Directory.CreateDirectory("D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Anexo " + folioAnexo + "/");
                        String path3 = "D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Anexo " + folioAnexo + "/" + FileUploadControl.FileName;

                        FileUploadControl.PostedFile.SaveAs(path3);

                        Session["Archivo2"] = path3;
                    }

                    string Archivo = (String)Session["Archivo"];
                    string Archivo2 = (String)Session["Archivo2"];
                    bool EsVigente;

                    InvContAnexo anexo = new InvContAnexo();
                    anexo.Id = Anexo;
                    InvContContrato contrato = new InvContContrato();
                    contrato.Id = 0;
                    InvContConvModificatorio conv = new InvContConvModificatorio();
                    conv.Id = 0;

                    WSSS.InvContUploadedFile file = new WSSS.InvContUploadedFile();
                    file.TipoDocumentoId = IdTipoDocumento;
                    file.InvContAnexo = anexo;
                    file.InvContContrato = contrato;
                    file.InvContConvModificatorio = conv;
                    file.Nombre = FileUploadControl.FileName.ToString();
                    file.Comentarios = txtComentario.Text;
                    if (rbSi.Checked)
                    {
                        EsVigente = true;
                    }
                    else
                    {
                        EsVigente = false;
                    }
                    file.Vigente = EsVigente;
                    if (Archivo != null)
                    {
                        file.RutaArchivo = Archivo;
                    }
                    else
                    {
                        file.RutaArchivo = Archivo2;
                    }
                    file.Usuar = 0;
                    file.UsuarioUltAct = 0;

                    WebServiceSoapClient service = new WebServiceSoapClient();
                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            result = service.InsertarArchivos(file,
                                Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    if (procedimiento == "Convenio")
                    {
                        String path = @"D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";
                        //String path = @"C:/K6/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";

                        if (Directory.Exists(path))
                        {


                            Directory.CreateDirectory(path + "/Convenio " + folioConvenio + "/");
                            String path2 = "Convenio " + folioConvenio + "/" + FileUploadControl.FileName;

                            FileUploadControl.PostedFile.SaveAs(path + path2);

                            //Para tomar la ruta de archivo y guardarlo en la tabla
                            string path4 = path + path2;
                            Session["Archivo"] = path4;
                        }
                        else
                        {
                            Directory.CreateDirectory("D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/");
                            String path3 = "D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/" + FileUploadControl.FileName;

                            //Directory.CreateDirectory("C:/K6/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/");
                            //String path3 = "C:/K6/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/Convenio " + folioConvenio + "/" + FileUploadControl.FileName;

                            FileUploadControl.PostedFile.SaveAs(path3);

                            Session["Archivo2"] = path3;

                        }

                        string Archivo = (String)Session["Archivo"];
                        string Archivo2 = (String)Session["Archivo2"];
                        bool EsVigente;

                        InvContAnexo anexo = new InvContAnexo();
                        anexo.Id = 0;
                        InvContContrato contrato = new InvContContrato();
                        contrato.Id = 0;
                        InvContConvModificatorio conv = new InvContConvModificatorio();
                        conv.Id = Convenio;

                        WSSS.InvContUploadedFile file = new WSSS.InvContUploadedFile();
                        file.TipoDocumentoId = IdTipoDocumento;
                        file.InvContAnexo = anexo;
                        file.InvContContrato = contrato;
                        file.InvContConvModificatorio = conv;
                        file.Nombre = FileUploadControl.FileName.ToString();
                        file.Comentarios = txtComentario.Text;
                        if (rbSi.Checked)
                        {
                            EsVigente = true;
                        }
                        else
                        {
                            EsVigente = false;
                        }
                        file.Vigente = EsVigente;
                        if (Archivo != null)
                        {
                            file.RutaArchivo = Archivo;
                        }
                        else
                        {
                            file.RutaArchivo = Archivo2;
                        }
                        file.Usuar = 0;
                        file.UsuarioUltAct = 0;

                        WebServiceSoapClient service = new WebServiceSoapClient();
                        using (WebClient client = new WebClient())
                        {
                            try
                            {
                                result = service.InsertarArchivos(file,
                                    Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);
                            }
                            catch (Exception ex)
                            {
                                result = ex.Message;
                            }
                        }

                    }
                    else
                    {
                        String path = @"D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/";
                        if (Directory.Exists(path))
                        {

                            FileUploadControl.PostedFile.SaveAs(path + FileUploadControl.FileName);
                            Session["Archivo"] = path + FileUploadControl.FileName; ;
                        }
                        else
                        {
                            Directory.CreateDirectory("D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/");
                            String path2 = "D:/K5/Archivos/UploadedFiles/K5Banorte/Contratos/Contratos " + folio + "/" + FileUploadControl.FileName;

                            FileUploadControl.PostedFile.SaveAs(path2);

                            Session["Archivo2"] = path2;
                        }
                        string Archivo = (String)Session["Archivo"];
                        string Archivo2 = (String)Session["Archivo2"];


                        bool EsVigente;

                        InvContContrato contrato = new InvContContrato();
                        contrato.Id = Contrato;
                        InvContAnexo anexo = new InvContAnexo();
                        anexo.Id = 0;
                        InvContConvModificatorio conv = new InvContConvModificatorio();
                        conv.Id = 0;
                        WSSS.InvContUploadedFile file = new WSSS.InvContUploadedFile();
                        file.TipoDocumentoId = IdTipoDocumento;
                        file.TipoDocumentoId = IdTipoDocumento;
                        file.InvContAnexo = anexo;
                        file.InvContContrato = contrato;
                        file.InvContConvModificatorio = conv;
                        file.Nombre = FileUploadControl.FileName.ToString();
                        file.Comentarios = txtComentario.Text;
                        if (rbSi.Checked)
                        {
                            EsVigente = true;
                        }
                        else
                        {
                            EsVigente = false;
                        }
                        file.Vigente = EsVigente;
                        if (Archivo != null)
                        {
                            file.RutaArchivo = Archivo;
                        }
                        else
                        {
                            file.RutaArchivo = Archivo2;
                        }
                        file.Usuar = 0;
                        file.UsuarioUltAct = 0;
                        WebServiceSoapClient service = new WebServiceSoapClient();
                        using (WebClient client = new WebClient())
                        {
                            try
                            {
                                result = service.InsertarArchivos(file,
                                    Util.Encrypt(DSODataContext.ConnectionString), DSODataContext.Schema);
                            }
                            catch (Exception ex)
                            {
                                result = ex.Message;
                            }
                        }

                    }
                }
            }
            else
            {
                result = "El tipo de documento no es permitido";
            }
            String cstext = "alert('" + result + "'); opener.window.location.href='" + url + "'; window.close();";
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }
    }
}