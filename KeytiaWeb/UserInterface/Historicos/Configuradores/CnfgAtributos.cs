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
    public class CnfgAtributos : HistoricEdit
    {
        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgAtributos.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgAtributos.js") + "'type='text/javascript'></script>\r\n", true, false);
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            ((TableRow)pdtFinVigencia.TcCtl.Parent).Style["display"] = "none";
            ((TableRow)pdtIniVigencia.TcCtl.Parent).Style["display"] = "none";

            if (s == HistoricState.Edicion
                && iCodRegistro != "null")
            {
                pvchCodigo.TextBox.Enabled = false;
                pvchDescripcion.TextBox.Enabled = false;
            }
            else if (s == HistoricState.Baja)
            {
                pdtFinVigencia.DateTimeBox.Enabled = false;
            }
        }

        protected override void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            SetHistoricState(HistoricState.Baja);
            pFields.DisableFields();

            string lsTitutlo = DSOControl.JScriptEncode(AlertTitle);
            string lsMsgConfirmar = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarBajaAtrib"));
            DSOControl.jAlert(Page, pjsObj + ".ConfirmarBajaAtrib", lsMsgConfirmar, lsTitutlo);

            FirePostBajaClick();
        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();

            try
            {
                DataTable ldt = DSODataAccess.Execute("select top 1 * from " + DSODataContext.Schema + ".GetMaeByAtrib(" + iCodCatalogo + ")");

                if (pFields.ContainsConfigName("Types")
                    && ldt.Rows.Count > 0)
                {
                    pFields.GetByConfigName("Types").DisableField();
                }
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrInitControls", e);
            }
        }

        protected override bool ValidarVigencias()
        {
            pdtIniVigencia.DataValue = new DateTime(1900, 1, 1);
            if (State != HistoricState.Baja)
            {
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }
            else
            {
                pdtFinVigencia.DataValue = new DateTime(1900, 1, 1);
            }
            return true;
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError;
            string lsTitutlo = DSOControl.JScriptEncode(AlertTitle);
            DataTable ldt;
            StringBuilder lsbQuery = new StringBuilder();

            try
            {
                if (State == HistoricState.Baja)
                {
                    ldt = DSODataAccess.Execute("select top 1 * from " + DSODataContext.Schema + ".GetMaeByAtrib(" + iCodCatalogo + ")");

                    if (ldt.Rows.Count > 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MaeAtribConfig"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
                else
                {
                    KeytiaBaseField lFieldType = pFields.GetByConfigName("Types");
                    KeytiaBaseField lFieldControl = pFields.GetByConfigName("Controles");

                    if (lFieldControl.DSOControlDB.HasValue)
                    {
                        lsbQuery.AppendLine("select * from [VisRelaciones('Tipos de datos - Controles','" + Globals.GetCurrentLanguage() + "')]");
                        lsbQuery.AppendLine("where dtIniVigencia <> dtFinVigencia");
                        lsbQuery.AppendLine("and Types = " + lFieldType.DataValue);

                        ldt = DSODataAccess.Execute(lsbQuery.ToString());

                        if (ldt.Select("Controles = " + lFieldControl.DataValue).Length == 0)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "AtribControlConfig"));
                            lsbErrores.Append("<li><span>" + lsError + "</span><ul>");
                            foreach (DataRow lRow in ldt.Rows)
                            {
                                lsError = DSOControl.JScriptEncode(lRow["ControlesDesc"].ToString());
                                lsbErrores.Append("<li>" + lsError + "</li>");
                            }
                            lsbErrores.Append("</ul></li>");
                        }
                    }
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitutlo);
                }

                return lbret;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        #region WebMethods

        public static string SearchAtribControl(string term, int iCodType)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();

                //'Se asume que los controles y las relaciones de estos con los tipos de datos nunca expiran
                //'y que la forma de hacer que ya no aparezcan es eliminandolos 
                //'haciendo dtFinVigencia = dtIniVigencia
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("select top 100 *  from [VisHistoricos('Controles','" + lsLang + "')] ctl");
                lsbQuery.AppendLine("where ctl.dtIniVigencia <> ctl.dtFinVigencia");
                lsbQuery.AppendLine("and ctl.vchDescripcion + ' (' + ctl.vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'");
                lsbQuery.AppendLine("and ctl.iCodCatalogo in(select rel.Controles");
                lsbQuery.AppendLine("   from [VisRelaciones('Tipos de datos - Controles','" + lsLang + "')] rel");
                lsbQuery.AppendLine("   where rel.dtIniVigencia <> rel.dtFinVigencia");
                lsbQuery.AppendLine("   and rel.Types = " + iCodType + ")");
                lsbQuery.AppendLine("order by ctl.vchDescripcion");

                DataTable ldtVista = DSODataAccess.Execute(lsbQuery.ToString());
                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }


        #endregion
    }
}
