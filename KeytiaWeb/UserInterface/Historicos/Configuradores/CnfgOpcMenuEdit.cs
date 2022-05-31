using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;

namespace KeytiaWeb.UserInterface
{
    public class CnfgOpcMenuEdit : HistoricEdit
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
                    psbQuery.AppendLine("select Menus = isnull(count(H.iCodRegistro),0)");
                    psbQuery.AppendLine("from [VisHistoricos('OpcMnu','Opciones de Menu','" + Globals.GetCurrentLanguage() + "')] H");
                    psbQuery.AppendLine("where H.OpcMnu = " + iCodCatalogo);
                    psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                    psbQuery.AppendLine("and H.dtFinVigencia > " + pdtFinVigencia.DataValue);

                    liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                    if (liCount > 0)
                    {
                        lbret = false;
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "OpcMnuAsoc", pvchDescripcion.ToString()));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    }
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException("ErrValidateRecord", ex);
                }
            }
            return lbret;
        }
    }
}
