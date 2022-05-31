/*
Nombre:		    PGS
Fecha:		    20120105
Descripción:	Configuración específica para almacenamiento de Cuentas Maestras.
Modificación:	
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using KeytiaServiceBL;
using DSOControls2008;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public class CnfgCtaMaestraEdit : HistoricEdit
    {
        protected override void InitAccionesSecundarias()
        {
            pvchDescripcion.Visible = false;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pvchDescripcion.Descripcion = "";            
        }

        protected override bool ValidarCampos()
        {
            if (!base.ValidarCampos())
            {                
                return false;
            }

            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCtaMaestra"));
            bool lbret = true;
            try
            {
                if (pFields != null && pFields.ContainsConfigName("Carrier") && !pFields.GetByConfigName("Carrier").DSOControlDB.HasValue)
                {
                    lbret = false;
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("Carrier").Descripcion));
                    lsError = "<ul>" + "<li>" + lsError + "</li>" + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
                return lbret;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected override bool ValidarClaves()
        {
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCtaMaestra"));
            DataTable ldt;
            bool lbret = true;

            try
            {
                if (!pvchCodigo.HasValue)
                {
                    lbret = false;
                    lsError = "<ul>" + "<li>" + pvchCodigo.RequiredMessage + "</li>" + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    return lbret;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(vchCodigo.DataValue.ToString().Replace("'", ""), "^([0-9]*[a-z]*[A-Z]*)*$"))
                {
                    lbret = false;
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValClaveCenCos", pvchCodigo.Descripcion.ToString()));
                    lsError = "<ul>" + "<li>" + lsError + "</li>" + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    return lbret;
                }
                else if (pFields != null && pFields.ContainsConfigName("Carrier"))
                {
                    string lsCatCarrier = pFields.GetByConfigName("Carrier").DataValue.ToString();
                    string lsCodCarrier = pKDB.GetHisRegByEnt("Carrier", "Carriers", "iCodCatalogo = " + lsCatCarrier).Rows[0]["vchCodigo"].ToString();
                    pvchDescripcion.DataValue = pvchCodigo.DataValue.ToString().Replace("'", "") + " (" + lsCodCarrier + ")";
                }
                else
                {
                    pvchDescripcion.DataValue = pvchCodigo.DataValue.ToString().Replace("'", "");
                }

                psbQuery.Length = 0;
                psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, H.iCodMaestro, H.dtIniVigencia, H.dtFinVigencia");
                psbQuery.AppendLine("from Historicos H, Catalogos C");
                psbQuery.AppendLine("where H.iCodCatalogo = C.iCodRegistro");
                psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodRegistro + ",-1)");
                psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ",-1)");
                psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
                psbQuery.AppendLine("and C.vchCodigo = " + pvchCodigo.DataValue);
                psbQuery.AppendLine("and C.vchDescripcion = " + pvchDescripcion.DataValue);
                psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                psbQuery.AppendLine("and ((H.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                psbQuery.AppendLine("   or (H.dtIniVigencia <= " + pdtFinVigencia.DataValue);
                psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                psbQuery.AppendLine("   or (H.dtIniVigencia >= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and H.dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");
                psbQuery.AppendLine("order by H.dtIniVigencia, H.dtFinVigencia, H.iCodRegistro");

                ldt = DSODataAccess.Execute(psbQuery.ToString());

                string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                foreach (DataRow ldataRow in ldt.Rows)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat), ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat)));
                    lsbErroresCodigos.Append("<li>" + lsError + "</li>");
                }

                if (lsbErroresCodigos.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidaCtaMaestra", pFields.GetByConfigName("Carrier").ToString()));
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresCodigos.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
                return lbret;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }
   
        protected override bool ValidarDatos()
        {
            bool lbret = true;

            if (State != HistoricState.Baja)
            {
                return lbret;
            }

            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            DataTable ldt;

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select iCodRegistro, iCodCatalogo, iCodMaestro, dtIniVigencia, dtFinVigencia");
                psbQuery.AppendLine("from [VisHistoricos('Linea','Lineas','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where [CtaMaestra] = " + iCodCatalogo);
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("and ((dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("      and dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                psbQuery.AppendLine("  or (dtIniVigencia <= " + pdtFinVigencia.DataValue);
                psbQuery.AppendLine("      and dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                psbQuery.AppendLine("  or (dtIniVigencia >= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("      and dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");

                ldt = DSODataAccess.Execute(psbQuery.ToString());

                string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                foreach (DataRow ldataRow in ldt.Rows)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat), ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat)));
                    lsbErroresCodigos.Append("<li>" + lsError + "</li>");
                }
                
                if (lsbErroresCodigos.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "LineaAsocCtaMae", pvchDescripcion.DataValue.ToString().Replace("'", "")));
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresCodigos.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            if (lsbErrores.Length > 0)
            {
                lbret = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }

            return lbret;

        }
    }
}
