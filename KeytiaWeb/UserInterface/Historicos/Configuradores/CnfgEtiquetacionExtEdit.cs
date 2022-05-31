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
using System.Text.RegularExpressions;

namespace KeytiaWeb.UserInterface
{
    public class CnfgEtiquetacionExtEdit: HistoricEdit
    {
        protected int pDiaEtiquetacion = 0;

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgEtiquetacionExtEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgEtiquetacionExtEdit.js") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            if (pFields != null)
            {
                KeytiaBaseField lFieldIniPer = pFields.GetByConfigName("IniPer");
                KeytiaBaseField lFieldFinPer = pFields.GetByConfigName("FinPer");
                lFieldIniPer.DSOControlDB.AddClientEvent("onChangeMonthYear", "function(year, month, inst){" + pjsObj + ".cambioPeriodoEtiquetaExt(inst,'#" + lFieldIniPer.DSOControlDB.ClientID + "_dt', '#" + lFieldFinPer.DSOControlDB.ClientID + "_dt',  1, month, year);}");
                lFieldFinPer.DSOControlDB.AddClientEvent("onChangeMonthYear", "function(year, month, inst){" + pjsObj + ".cambioPeriodoEtiquetaExt(inst,'#" + lFieldFinPer.DSOControlDB.ClientID + "_dt', '#" + lFieldIniPer.DSOControlDB.ClientID + "_dt', -1, month, year);}");
            }
        }

        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            ObtenerDiaInicioEtiquetacion();
            DateTime ldtInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, pDiaEtiquetacion); // DateTime.Today;
            if (pFields.ContainsConfigName("IniPer"))
            {
                KeytiaBaseField lFieldIniPer = pFields.GetByConfigName("IniPer");
                lFieldIniPer.DataValue = ldtInicio;
            }
            if (pFields.ContainsConfigName("FinPer"))
            {
                KeytiaBaseField lFieldFinPer = pFields.GetByConfigName("FinPer");
                DateTime ldtFin = ldtInicio.AddMonths(1);
                lFieldFinPer.DataValue = ldtFin;
            }
        }

        private void ObtenerDiaInicioEtiquetacion()
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("\r\ndeclare @iCodUsuario int");
            sbQuery.Append("\r\nset @iCodUsuario = currentUser".Replace("currentUser", Session["iCodUsuario"].ToString()));
            sbQuery.Append("\r\ndeclare @iCodEmpresa int");
            sbQuery.Append("\r\ndeclare @iCodCliente int");
            sbQuery.Append("\r\ndeclare @DiaEtiquetacion int");
            sbQuery.Append("\r\nselect @iCodEmpresa = Empre from currentSchema.[VisHistoricos('Usuar','Usuarios','Español')] where iCodCatalogo = @iCodUsuario");
            sbQuery.Append("\r\nselect @iCodCliente = Client from currentSchema.[VisHistoricos('Empre','Empresas','Español')] where iCodCatalogo = @iCodEmpresa");
            sbQuery.Append("\r\nselect @DiaEtiquetacion = DiaEtiquetacion from currentSchema.[VisHistoricos('Client','Clientes','Español')] where iCodCatalogo = @iCodCliente");
            sbQuery.Append("\r\nselect @DiaEtiquetacion");

            object loDiaEtiquetacion = DSODataAccess.ExecuteScalar(sbQuery.ToString().Replace("currentSchema", DSODataContext.Schema));

            if (loDiaEtiquetacion != null && loDiaEtiquetacion != System.DBNull.Value && (int)loDiaEtiquetacion > 0)
            {
                pDiaEtiquetacion = (int)loDiaEtiquetacion;
            }
            else
            {
                pDiaEtiquetacion = DateTime.Now.Day;
            }
        }
    }
}
