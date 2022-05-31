using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Reflection;
using System.Web.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KeytiaServiceBL.Alarmas;

namespace KeytiaWeb.UserInterface
{
    public class CnfgPrepSitioEnvio : HistoricEdit
    {
        protected StringBuilder psbErrores;

        public CnfgPrepSitioEnvio()
        {
            Init += new EventHandler(CnfgPrepSitioEnvio_Init);
        }

        void CnfgPrepSitioEnvio_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgPrepSitioEnvio";
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                AgregarBoton("Asunto");
            }
        }

        protected override bool ValidarVigencias()
        {
            if (State == HistoricState.Edicion)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

            //el usuario nunca edita las vigencias por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarClaves()
        {
            //el usuario nunca edita las claves por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarAtribCatalogosVig()
        {
            //el usuario solamente puede seleccionar catalogos de entidades que estan vigentes todo el tiempo
            return true;
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                psbErrores = new StringBuilder();

                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                pvchDescripcion.DataValue = pFields.GetByConfigName("NotifPrepSitio").ToString();

                ValidarEmail("CtaPara");
                ValidarPlantilla("Plantilla");
                ValidarPlantillaReporte("Plantilla");

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected void ValidarEmail(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {

                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string email = lField.DSOControlDB.ToString().Trim();
                string lsError;
                Regex regexp = new Regex("^[a-zA-Z]{1}[a-zA-Z0-9_-]{2,}([.][a-zA-Z0-9_-]+)*[@][a-zA-Z0-9_-]{3,}([.][a-zA-Z0-9_-]+)*[.][a-zA-Z]{2,4}$");

                if (!String.IsNullOrEmpty(email))
                {
                    foreach (string mail in email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!regexp.IsMatch(mail))
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmailFormat", mail, lField.Descripcion));
                            psbErrores.Append("<li>" + lsError + "</li>");
                        }
                    }
                }
            }
        }

        protected void ValidarPlantilla(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string lsPlantilla = lField.DSOControlDB.ToString();
                string lsError;
                if (!String.IsNullOrEmpty(lsPlantilla))
                {
                    string lsFilePath = lsPlantilla;
                    if (System.IO.File.Exists(lsFilePath))
                        return;

                    lsFilePath = System.IO.Path.Combine(lsFilePath, Globals.GetCurrentLanguage());
                    if (System.IO.Directory.Exists(lsFilePath))
                    {
                        foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                        {
                            if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                            {
                                return;
                            }
                        }
                    }
                    lsFilePath = lsPlantilla;
                    if (System.IO.Directory.Exists(lsFilePath))
                    {
                        foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                        {
                            if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                            {
                                return;
                            }
                        }
                    }
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "PlantPrepSitioEnvio"));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
            }
        }

        protected void ValidarPlantillaReporte(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string lsPlantilla = lField.DSOControlDB.ToString();
                bool lbHayError = false;
                string lsError;

                if (pFields.GetByConfigName("RepEst").DSOControlDB.HasValue)
                {
                    //RZ.20140117 Se retira validacion del catalogo Idioma
                    if (String.IsNullOrEmpty(lsPlantilla)) //|| !pFields.GetByConfigName("Idioma").DSOControlDB.HasValue
                    {
                        lbHayError = true;
                    }
                    else
                    {
                        DataTable loTabla = new DataTable();
                        loTabla.Columns.Add("c1");
                        DataRow ldr = loTabla.NewRow();
                        ldr["c1"] = "c1";
                        loTabla.Rows.Add(ldr);
                        //RZ.20140117 Se retira busqueda del campo idioma, ahora se mandara por default "Español"
                        //Alarma.getIdioma(int.Parse(pFields.GetByConfigName("Idioma").DSOControlDB.DataValue.ToString()))
                        string lsFilePath = UtilAlarma.buscarPlantilla(lsPlantilla, "Español");
                        if (!string.IsNullOrEmpty(lsFilePath))
                        {
                            WordAccess loWord = new WordAccess();
                            loWord.FilePath = lsFilePath;
                            try
                            {
                                loWord.Abrir();

                                EstiloTablaWord pEstiloTablaWord = new EstiloTablaWord();
                                pEstiloTablaWord.Estilo = "KeytiaGrid";
                                pEstiloTablaWord.FilaEncabezado = true;
                                pEstiloTablaWord.FilasBandas = true;
                                pEstiloTablaWord.PrimeraColumna = false;
                                pEstiloTablaWord.UltimaColumna = false;
                                pEstiloTablaWord.ColumnasBandas = false;
                                loWord.InsertarTabla(loTabla, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);
                                pEstiloTablaWord.Estilo = "KeytiaHeaderRep";
                                loWord.InsertarTabla(loTabla, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);
                                loWord.InsertarTexto("Titulo");
                                loWord.SetStyle("TituloReporte");
                            }
                            catch (Exception ex)
                            {
                                lbHayError = true;
                            }
                            finally
                            {
                                loWord.Cerrar();
                                loWord.Salir();
                                loWord = null;
                            }
                        }
                    }
                    if (lbHayError)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "PlantRptPrepSitioEnvio"));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }

    }
}
