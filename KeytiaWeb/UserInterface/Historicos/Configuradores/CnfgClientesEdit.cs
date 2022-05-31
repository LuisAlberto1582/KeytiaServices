using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgClientesEdit : HistoricEdit
    {
        protected string psError;
        protected StringBuilder psbErrores = new StringBuilder();
        protected string psTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloClientes"));

        public CnfgClientesEdit()
        {
            Init += new EventHandler(CnfgClientesEdit_Init);
        }

        void CnfgClientesEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgReportesEdit";
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            ValidarPlantilla();
            ValidarEmail();
            ValidarExistencia();
            if (psbErrores.Length > 0)
            {
                lbret = false;
                psError = "<ul>" + psbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", psError, psTitulo);
            }
            return lbret;
        }

        protected bool ValidarPlantilla()
        {
            bool lbret = true;
            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ConfigName == "PathPlantillaCartCustProcesadas"
                       || lField.ConfigName == "PathPlantillaCartCustAceptadas"
                       || lField.ConfigName == "PathPlantillaCartCustRechazadas")
                {
                    KeytiaUploadField lFieldUpload = (KeytiaUploadField)pFields.GetByConfigName(lField.ConfigName);
                    DSOUpload lfuField = (DSOUpload)lFieldUpload.DSOControlDB;

                    string lsFieldDataValue = lfuField.FileName;

                    if (lsFieldDataValue != ""
                        && !(lsFieldDataValue.EndsWith(".doc")
                           || lsFieldDataValue.EndsWith(".docx")))
                    {
                        psError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "PlantillaNoValida", " \".doc\" , \".docx\" " , lField.Descripcion));
                        psbErrores.Append("<li>" + psError + "</li>");
                    }
                }
            }
            if (psbErrores.Length > 0)
            {
                lbret = false;
                psError = "<ul>" + psbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", psError, psTitulo);
            }

            return lbret;
        }

        protected void ValidarEmail()
        {
            string lsRegExp = "^[a-zA-Z]{1}[a-zA-Z0-9_-]{2,}([.][a-zA-Z0-9_-]+)*[@][a-zA-Z0-9_-]{3,}([.][a-zA-Z0-9_-]+)*[.][a-zA-Z]{2,4}$";
            string[] lsCampos = { "CtaCC", "CtaCCO", "CtaDe" };

            foreach (string lsCampo in lsCampos)
            {
                if (pFields.ContainsConfigName(lsCampo))
                {
                    string lsEmail = pFields.GetByConfigName(lsCampo).DataValue.ToString().Replace("'", "");
                    if (lsEmail != "null" && !System.Text.RegularExpressions.Regex.IsMatch(lsEmail, lsRegExp))
                    {
                        psError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmailFormat", lsEmail, lsCampo));
                        psbErrores.Append("<li>" + psError + "</li>");
                    }
                }
            }
        }

        protected void ValidarExistencia()
        {
            string lsFilePath = "";
            string lsPath = "";
            string[] lsFileFields = { "MasterPage", "Logo", "StyleSheet" };

            foreach (string lsField in lsFileFields)
            {
                if (pFields.ContainsConfigName(lsField))
                {
                    KeytiaBaseField lField = pFields.GetByConfigName(lsField);
                    lsPath = lField.DataValue.ToString().Replace("'", "");
                    lsFilePath = lsPath;
                    lsFilePath = lsFilePath.Replace("/", "\\");
                    lsFilePath = lsFilePath.Replace("~", HttpContext.Current.Server.MapPath("~"));

                    if (lsPath != "null")
                    {
                        if (!System.IO.Directory.Exists(lsFilePath)
                            && !System.IO.File.Exists(lsFilePath))
                        {
                            psError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrExistPath", lsPath));
                            psbErrores.Append("<li>" + psError + "</li>");
                        }
                        else
                        {
                            if (lsField == "MasterPage" && !lsPath.EndsWith("master"))
                            {
                                psError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrFormato", lsPath, lField.Descripcion));
                                psbErrores.Append("<li>" + psError + "</li>");
                            }
                            else if (lsField == "Logo" &&
                                     !(lsPath.EndsWith("jpg") || lsPath.EndsWith("png") || lsPath.EndsWith("gif")))
                            {
                                psError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrFormato", lsPath, lField.Descripcion));
                                psbErrores.Append("<li>" + psError + "</li>");
                            }
                        }
                    }
                }
            }
        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            if (pFields.ContainsConfigName("CartaCust"))
            {
                KeytiaBaseField lField = pFields.GetByConfigName("CartaCust");
                lField.DisableField();
            }
        }

        protected override void SubHistorico_PostGrabarClick(object sender, EventArgs e)
        {
            base.SubHistorico_PostGrabarClick(sender, e);
            if (pFields.ContainsConfigName("CartaCust"))
            {
                KeytiaBaseField lField = pFields.GetByConfigName("CartaCust");
                lField.DisableField();
            }
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                AgregarBoton("CartaCust");
            } 
        }

        protected override void AgregarBoton(string lConfigName)
        {
            AgregarBoton(lConfigName, "KeytiaWeb.UserInterface.CnfgClientesEdit", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }

        protected override void btnSubHistorico_ServerClick(object sender, EventArgs e)
        {
            FillAjaxControls();
            KeytiaBaseField lField = pFields[((HtmlButton)sender).Attributes["DataField"]];

            //las entidades que se utilicen con los botones que se agregan deben de tener un solo maestro
            string lsMaestro = DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodEntidad = " + lField.ConfigValue + " and dtIniVigencia <> dtFinVigencia order by iCodRegistro").ToString();

            PrevState = State;
            SubHistoricClass = lField.SubHistoricClass; // "KeytiaWeb.UserInterface.CnfgReportesEdit";
            SubCollectionClass = lField.SubCollectionClass; // "KeytiaWeb.UserInterface.CnfgRepFieldCollection";

            InitSubHistorico(this.ID + "Ent" + lField.ConfigValue);

            pSubHistorico.SetEntidad(lField.ConfigName);
            pSubHistorico.SetMaestro("Configuracion Cartas Custodia");

            pSubHistorico.EsSubHistorico = true;
            pSubHistorico.FillControls();

            SetHistoricState(HistoricState.SubHistorico);
            pSubHistorico.InitMaestro();

            if (lField.DSOControlDB.HasValue)
            {
                KDBAccess kdb = new KDBAccess();
                DataTable dtReg = kdb.GetHisRegByEnt((lField.ConfigName).ToString(), "", "iCodCatalogo = " + (lField.DataValue).ToString());
                pSubHistorico.iCodRegistro = (dtReg.Rows[0]["iCodRegistro"]).ToString();
                pSubHistorico.ConsultarRegistro();
            }
            pSubHistorico.Fields.EnableFields();
            pSubHistorico.SetHistoricState(HistoricState.Edicion);
        }

        protected override void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            if (ValidarPlantilla())
            {
                base.pbtnGrabar_ServerClick(sender, e);
            }
        }

        protected override void CnfgSubHistoricField_PostGrabarClick(object sender, EventArgs e)
        {
            ActualizarRestricciones();
            base.CnfgSubHistoricField_PostGrabarClick(sender, e);
        }

        protected void ActualizarRestricciones()
        {
            KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");
            int liCodUsuario = (int)Session["iCodUsuarioDB"];

            switch (pSubHistorico.vchDesMaestro)
            {
                case "Centro de Costos":
                    pCargaCom.ActualizaRestCliente(iCodCatalogo, "CenCos", "RestCenCos", liCodUsuario);
                    break;
                case "Empleados":
                    pCargaCom.ActualizaRestCliente(iCodCatalogo, "Emple", "RestEmple", liCodUsuario);
                    break;
                case "Sitios":
                    pCargaCom.ActualizaRestCliente(iCodCatalogo, "Sitio", "RestSitio", liCodUsuario);
                    break;
            }
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

    }
}
