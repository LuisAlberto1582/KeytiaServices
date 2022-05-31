using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgPerfilesEdit : HistoricEdit
    {
        protected override bool ValidarDatos()
        {
            bool lbret = true;
            int liCount;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            if (State == HistoricState.Baja)
            {
                try
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select Usuarios = isnull(count(H.iCodRegistro),0)");
                    psbQuery.AppendLine("from [VisHistoricos('Usuar','Usuarios','" + Globals.GetCurrentLanguage() + "')] H");
                    psbQuery.AppendLine("where H.Perfil = " + iCodCatalogo);
                    psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                    psbQuery.AppendLine("and H.dtFinVigencia > " + pdtFinVigencia.DataValue);

                    liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                    if (liCount > 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "UsuAsoc", pvchDescripcion.DataValue.ToString()));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                        lbret = false;
                    }
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException("ErrValidateRecord", ex);
                }
            }
            return lbret;
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
                    pCargaCom.ActualizaRestPerfil(iCodCatalogo, "CenCos", "RestCenCos", liCodUsuario);
                    break;
                case "Empleados":
                    pCargaCom.ActualizaRestPerfil(iCodCatalogo, "Emple", "RestEmple", liCodUsuario);
                    break;
                case "Sitios":
                    pCargaCom.ActualizaRestPerfil(iCodCatalogo, "Sitio", "RestSitio", liCodUsuario);
                    break;
            }
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }
      
    }
}