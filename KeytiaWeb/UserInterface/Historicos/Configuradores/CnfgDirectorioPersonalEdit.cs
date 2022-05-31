/*
Nombre:		    PGS
Fecha:		    20111017
Descripción:	Configuración específica para almacenamiento de Numeros en Directorio Personal.
Modificación:	
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using DSOControls2008;

namespace KeytiaWeb.UserInterface
{
    public class CnfgDirectorioPersonalEdit : CnfgDirectoriosEdit
    {
        protected override void HistoricEdit_Init(object sender, EventArgs e)
        {
            base.HistoricEdit_Init(sender, e);
            pFieldsNoVisibles = new string[] { "EmpleParticulares", "EmpleExcepcion", "BanderasEtiqueta", "Empre", "Emple" };
        }

        protected override void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            FillAjaxControls();

            pvchDescripcion.DataValue = pvchCodigo.DataValue.ToString().Replace("'", "") + "_" + piCodEmpleado;

            base.pbtnGrabar_ServerClick(sender, e);
        }

        protected override bool ValidarRegistro()
        {
            if (!pdtIniVigencia.HasValue)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
            }
            int liDiaIniVig = EtiquetaEdit.ValidarDiasMes(pdtIniVigencia.Date.Year, pdtIniVigencia.Date.Month, piDiaCorte);
            DateTime ldtIniVigencia = new DateTime(pdtIniVigencia.Date.Year, pdtIniVigencia.Date.Month, liDiaIniVig);
            if (ldtIniVigencia > pdtIniVigencia.Date)
            {
                liDiaIniVig = EtiquetaEdit.ValidarDiasMes(pdtIniVigencia.Date.AddMonths(-1).Year, pdtIniVigencia.Date.AddMonths(-1).Month, piDiaCorte);
                pdtIniVigencia.DataValue = new DateTime(pdtIniVigencia.Date.AddMonths(-1).Year, pdtIniVigencia.Date.AddMonths(-1).Month, liDiaIniVig);
            }
            else
            {
                pdtIniVigencia.DataValue = ldtIniVigencia;
            }
            if (piHisPreviaEtiqueta == 0 && State != HistoricState.Baja)
            {
                //Si no guarda Historia, el periodo vigente del número será de un mes
                int liDiaFinVig = EtiquetaEdit.ValidarDiasMes(pdtIniVigencia.Date.AddMonths(1).Year, pdtIniVigencia.Date.AddMonths(1).Month, piDiaCorte);
                pdtFinVigencia.DataValue = new DateTime(pdtIniVigencia.Date.AddMonths(1).Year, pdtIniVigencia.Date.AddMonths(1).Month, liDiaFinVig).AddDays(-1);
            }
            return base.ValidarRegistro();
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields == null)
            {
                return;
            }
            if (pFields.ContainsConfigName("BanderasEtiqueta"))
            {
                pFields.GetByConfigName("BanderasEtiqueta").DataValue = 1;
            }
            if (pbEnableEmpleado && pFields.ContainsConfigName("Emple"))
            {
                pFields.GetByConfigName("Emple").DataValue = piCodEmpleado;
            }
        }

        protected override void InitHisGridLanguage()
        {
            KeytiaBaseField lField;
            DSOControlDB lFiltro;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {

                if (pFields.Contains(lCol.sName))
                {
                    lField = pFields[lCol.sName];
                    if (lField.ConfigName == "Empre")
                    {
                        lCol.bVisible = false;
                    }
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName == "vchCodigo")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NumeroTel"));
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcion"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "Gpo")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "GrupoEtiqueta"));
                }
                else if (lCol.sName == "dtIniVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtIniVigencia"));
                }
                else if (lCol.sName == "dtFinVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFinVigencia"));
                }
                else if (lCol.sName == "dtFecUltAct")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFecUltAct"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "Consultar")
                {
                    string lsdoPostBack = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "btnConsultar:{0}"));
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnConsultar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + ResolveUrl("~/images/searchsmall.png") + "\",\"" + lsdoPostBack + "\");}";
                }
                if (phtFiltros.ContainsKey(lCol.sName))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lCol.sName];
                    lFiltro.Descripcion = lCol.sTitle;
                }
            }
        }

    }
}
