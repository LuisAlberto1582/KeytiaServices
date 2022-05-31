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

namespace KeytiaWeb.UserInterface
{
    public class CnfgReportesCamposEdit : CnfgReportesEdit
    {
        protected override bool ValidarClaves()
        {
            bool lbRet = base.ValidarClaves();
            //si el registro se esta eliminando entonces no es necesaria la validacion
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }
            if (lbRet)
            {
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("declare @dtIniVigencia datetime");
                lsbQuery.AppendLine("set @dtIniVigencia = '" + dtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
                lsbQuery.AppendLine("declare @dtFinVigencia datetime");
                lsbQuery.AppendLine("set @dtFinVigencia = '" + dtFinVigencia.Date.ToString("yyyy-MM-dd") + "'");
                lsbQuery.AppendLine("select iCodRegistro, dtIniVigencia, dtFinVigencia from " + DSODataContext.Schema + ".[VisHistoricos('RepEstCampo','" + Globals.GetCurrentLanguage() + "')]");
                lsbQuery.AppendLine("where vchDesMaestro in ('" + vchDesMaestro.Replace("'", "''") + "')");
                lsbQuery.AppendLine("and iCodRegistro <> isnull(" + iCodRegistro + ",0)");
                lsbQuery.AppendLine("and (Atrib = " + pFields.GetByConfigName("Atrib").DataValue.ToString());
                lsbQuery.AppendLine(" or AtribAd = " + pFields.GetByConfigName("Atrib").DataValue.ToString() + ")");
                lsbQuery.AppendLine("and RepEst = " + pFields.GetByConfigName("RepEst").DataValue.ToString());
                lsbQuery.AppendLine("and (dtIniVigencia <= @dtIniVigencia or dtIniVigencia < @dtFinVigencia)");
                lsbQuery.AppendLine("and (dtFinVigencia > @dtIniVigencia or dtFinVigencia >= @dtFinVigencia)");
                lsbQuery.AppendLine("and dtFinVigencia > GETDATE()");
                lsbQuery.AppendLine("order by dtIniVigencia");

                DataTable ldtColisiones = DSODataAccess.Execute(lsbQuery.ToString());
                if (ldtColisiones != null && ldtColisiones.Rows.Count > 0)
                {
                    lbRet = false;
                    DataRow ldataRow = ldtColisiones.Rows[0];
                    string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                    string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
                    StringBuilder lsbError = new StringBuilder();
                    object[] laoParams = new object[2];
                    laoParams[0] = ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat);
                    laoParams[1] = ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat);
                    lsbError.Append("<li><ul>");
                    lsbError.Append(Globals.GetLangItem("MsgWeb", "Mensajes Web", "AtributoReporteEstandarTraslapado", laoParams));
                    lsbError.Append("</ul></li>");
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsbError.ToString(), lsTitulo);
                }
            }

            return lbRet;
        }

        protected override bool ValidarDatos()
        {
            bool lbRet = base.ValidarDatos();
            //si el registro se esta eliminando entonces no es necesaria la validacion
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }
            if (lbRet && pFields.ContainsConfigName("AtribAd") && pFields.ContainsConfigName("DataFieldRutaAd"))
            {
                if ((pFields.GetByConfigName("AtribAd").DSOControlDB.HasValue && !pFields.GetByConfigName("DataFieldRutaAd").DSOControlDB.HasValue)
                    || (!pFields.GetByConfigName("AtribAd").DSOControlDB.HasValue && pFields.GetByConfigName("DataFieldRutaAd").DSOControlDB.HasValue))
                {
                    lbRet = false;
                    string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
                    StringBuilder lsbError = new StringBuilder();
                    lsbError.Append("<li><ul>");
                    lsbError.Append(Globals.GetLangItem("MsgWeb", "Mensajes Web", "RepEstValAtributoAdDataFieldAd"));
                    lsbError.Append("</ul></li>");
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsbError.ToString(), lsTitulo);
                }
                else if (pFields.GetByConfigName("Atrib").DataValue.ToString() == pFields.GetByConfigName("AtribAd").DataValue.ToString())
                {
                    lbRet = false;
                    string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
                    StringBuilder lsbError = new StringBuilder();
                    lsbError.Append("<li><ul>");
                    lsbError.Append(Globals.GetLangItem("MsgWeb", "Mensajes Web", "RepEstValAtributoAd"));
                    lsbError.Append("</ul></li>");
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsbError.ToString(), lsTitulo);
                }
                else if (pFields.GetByConfigName("AtribAd").DSOControlDB.HasValue && pFields.GetByConfigName("DataFieldRutaAd").DSOControlDB.HasValue)
                {
                    StringBuilder lsbQuery = new StringBuilder();
                    lsbQuery.AppendLine("declare @dtIniVigencia datetime");
                    lsbQuery.AppendLine("set @dtIniVigencia = '" + dtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
                    lsbQuery.AppendLine("declare @dtFinVigencia datetime");
                    lsbQuery.AppendLine("set @dtFinVigencia = '" + dtFinVigencia.Date.ToString("yyyy-MM-dd") + "'");
                    lsbQuery.AppendLine("select iCodRegistro, dtIniVigencia, dtFinVigencia from " + DSODataContext.Schema + ".[VisHistoricos('RepEstCampo','" + Globals.GetCurrentLanguage() + "')]");
                    lsbQuery.AppendLine("where vchDesMaestro in ('" + vchDesMaestro.Replace("'","''") + "')");
                    lsbQuery.AppendLine("and iCodRegistro <> isnull(" + iCodRegistro + ",0)");
                    lsbQuery.AppendLine("and (Atrib = " + pFields.GetByConfigName("AtribAd").DataValue.ToString());
                    lsbQuery.AppendLine(" or AtribAd = " + pFields.GetByConfigName("AtribAd").DataValue.ToString() + ")");
                    lsbQuery.AppendLine("and RepEst = "+pFields.GetByConfigName("RepEst").DataValue.ToString());
                    lsbQuery.AppendLine("and (dtIniVigencia <= @dtIniVigencia or dtIniVigencia < @dtFinVigencia)");
                    lsbQuery.AppendLine("and (dtFinVigencia > @dtIniVigencia or dtFinVigencia >= @dtFinVigencia)");
                    lsbQuery.AppendLine("and dtFinVigencia > GETDATE()");
                    lsbQuery.AppendLine("order by dtIniVigencia");
                    DataTable ldtColisiones = DSODataAccess.Execute(lsbQuery.ToString());
                    if (ldtColisiones != null && ldtColisiones.Rows.Count > 0)
                    {
                        lbRet = false;
                        DataRow ldataRow = ldtColisiones.Rows[0];
                        string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                        string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
                        StringBuilder lsbError = new StringBuilder();
                        object[] laoParams = new object[2];
                        laoParams[0] = ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat);
                        laoParams[1] = ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat);
                        lsbError.Append("<li><ul>");
                        lsbError.Append(Globals.GetLangItem("MsgWeb", "Mensajes Web", "AtributoAdReporteEstandarTraslapado", laoParams));
                        lsbError.Append("</ul></li>");
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsbError.ToString(), lsTitulo);
                    }
                }
            }
            return lbRet;
        }
    }
}
