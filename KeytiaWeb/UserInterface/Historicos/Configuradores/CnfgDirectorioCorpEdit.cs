/*
Nombre:		    PGS
Fecha:		    20111017
Descripción:	Configuración específica para almacenamiento de Numeros en Directorio Corporativo.
Modificación:	
*/
using System;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using DSOControls2008;


namespace KeytiaWeb.UserInterface
{
    public class CnfgDirectorioCorpEdit : CnfgDirectoriosEdit
    {
        protected override void HistoricEdit_Init(object sender, EventArgs e)
        {
            base.HistoricEdit_Init(sender, e);
            pFieldsNoVisibles = new string[] { "Emple", "Empre" };
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgDirectorioCorpEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgDirectorioCorpEdit.js") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            if (pFields != null && pFields.ContainsConfigName("BanderasEtiqueta"))
            {
                pFields.GetByConfigName("BanderasEtiqueta").DSOControlDB.AddClientEvent(HtmlTextWriterAttribute.Onclick.ToString(), pjsObj + ".aplicaEmpleParticular($(this));");
            }            
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (State == HistoricState.Edicion)
            {
                StringBuilder lsb = new StringBuilder();
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".aplicaEmpleParticular($('#" + pFields.GetByConfigName("BanderasEtiqueta").DSOControlDB.ClientID + "_flags'))");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj, lsb.ToString(), true, false);
            }
            piCodEmpleado = null;
        }

        protected override bool ValidarRegistro()
        {
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
            return base.ValidarRegistro();
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
                    if (lField.ConfigName == "Empre" || lField.ConfigName == "Emple")
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

        protected override void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            FillAjaxControls();

            pvchDescripcion.DataValue = pvchCodigo.DataValue.ToString().Replace("'", "") + "_" + piCodEmpleado + "_Corp";

            base.pbtnGrabar_ServerClick(sender, e);
        }

        protected override void ActualizarDirectorio()
        {
            base.ActualizarDirectorio();
            try
            {
                StringBuilder lsbSQL = new StringBuilder();
                lsbSQL.Append("exec ActualizaDirectorioCorp '");
                lsbSQL.Append(DSODataContext.Schema);
                lsbSQL.Append("', 'Directorio', ");
                lsbSQL.Append(iCodCatalogo);

                DSODataAccess.ExecuteNonQuery(lsbSQL.ToString());
            }
            catch (Exception ex)
            {
                Util.LogException("Error actualizando el Directorio con iCodCatalogo " + iCodCatalogo + ".", ex);
            }
        }
    }
}
        