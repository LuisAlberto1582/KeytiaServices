using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace KeytiaWeb.UserInterface
{
    public class CnfgRestriccionesEdit : HistoricEdit
    {
        protected override bool ValidarClaves()
        {
            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"); ;

            if (pFields["iCodCatalogo01"].DSOControlDB.HasValue
                && pFields["iCodCatalogo02"].DSOControlDB.HasValue)
            {
                string lsQuery = "select vchCodigo from Catalogos where iCodRegistro = {0}";
                string lsVchCodigo01 = DSODataAccess.ExecuteScalar(string.Format(lsQuery, pFields["iCodCatalogo01"].DataValue)).ToString();
                string lsVchCodigo02 = DSODataAccess.ExecuteScalar(string.Format(lsQuery, pFields["iCodCatalogo02"].DataValue)).ToString();
                pvchDescripcion.DataValue = lsVchCodigo01 + " - " + lsVchCodigo02;
            }
            return base.ValidarClaves();
        }

        protected override bool ValidarDatos()
        {
            bool lbRet = true;

            //si el registro se esta eliminando entonces no es necesaria la validacion de campos obligatorios
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("declare @dtIniVigencia datetime");
            psbQuery.AppendLine("set @dtIniVigencia = '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine("declare @dtFinVigencia datetime");
            psbQuery.AppendLine("set @dtFinVigencia = '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "'");

            psbQuery.AppendLine("select * from Historicos");
            psbQuery.AppendLine("where dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and iCodMaestro = " + iCodMaestro);
            psbQuery.AppendLine("and iCodCatalogo01 = " + pFields["iCodCatalogo01"].DataValue);
            psbQuery.AppendLine("and iCodCatalogo02 = " + pFields["iCodCatalogo02"].DataValue);
            psbQuery.AppendLine("and (dtIniVigencia <= @dtIniVigencia or dtIniVigencia < @dtFinVigencia)");
            psbQuery.AppendLine("and (dtFinVigencia > @dtIniVigencia or dtFinVigencia >= @dtFinVigencia)");
            if(iCodRegistro != "null")
            {
                psbQuery.AppendLine("and iCodRegistro <> " + iCodRegistro);
            }


            DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());
            if (ldt.Rows.Count > 0)
            {
                StringBuilder lsbErrores = new StringBuilder();
                string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloHistoricos"));
                string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslapeEnt"));
                string lsErrorVig = "<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", pdtIniVigencia.Date.ToString(lsNetDateFormat), pdtIniVigencia.Date.ToString(lsNetDateFormat))) + "</li>";

                lsbErrores.Append("<li>" + lsError);
                lsbErrores.Append("<ul>" + lsErrorVig + "</ul>");
                lsbErrores.Append("</li>");

                lbRet = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }

            return lbRet;
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pvchCodigo.TextBox.Enabled = false;
            pvchDescripcion.TextBox.Enabled = false;
        }

        protected override bool ValidarAtribCatalogosVig()
        {
            //como en el caso de las restricciones lo que se establece es el rango en el cual se puede acceder al registro entonces
            //no necesariamente los catalogos relacionados deben de estar vigentes durante todo el rango del registro de restriccion
            return true;
        }
    }
}