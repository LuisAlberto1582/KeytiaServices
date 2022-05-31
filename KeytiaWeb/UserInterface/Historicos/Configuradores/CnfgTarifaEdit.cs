/*
Nombre:		    PGS
Fecha:		    20111028
Descripción:	Clase que sirve para consultar tarifas desde un filtro de Región y Plan de Servicios.
Modificación:	
*/

using System;
using System.Text;
using DSOControls2008;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface
{
    public class CnfgTarifaEdit : HistoricEdit
    {
        protected CnfgTarifaFieldCollection pFieldsTarifas;

        public CnfgTarifaEdit()
        {
            Init += new EventHandler(CnfgTarifaEdit_Init);
        }

        void CnfgTarifaEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgTarifaEdit";
            CollectionClass = "KeytiaWeb.UserInterface.CnfgTarifaFieldCollection";
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgTarifaEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgTarifaEdit.js") + "'type='text/javascript'></script>\r\n", true, false);
        }

        public override void SetHistoricState(HistoricState s)
        {
            if (s == HistoricState.Inicio || s == HistoricState.MaestroSeleccionado)
            {
                s = HistoricState.Edicion;
            }

            base.SetHistoricState(s);

            if (s == HistoricState.Edicion)
            {
                pPanelSubHistoricos.Visible = true;
                pbtnGrabar.Visible = false;
                pbtnCancelar.Visible = false;
                pExpRegistro.StartOpen = false;                

                //Si actual Historico es un SubHistorico se muestra botón Cancelar.
                if ( this.pHistorico != null)
                {
                    pbtnCancelar.Visible = true;
                }
            }
        }

        protected override void InitFields()
        {
            pExpAtributos.ID = "AtribWrapper";
            pExpAtributos.StartOpen = true;
            pExpAtributos.CreateControls();
            pExpAtributos.Panel.Controls.Clear();
            pExpAtributos.Panel.Controls.Add(pTablaAtributos);
            pExpAtributos.OnOpen = "function(){" + pjsObj + ".fnInitGrids();}";

            pTablaAtributos.Controls.Clear();
            pTablaAtributos.ID = "Atributos";
            pTablaAtributos.Width = Unit.Percentage(100);

            pPanelSubHistoricos.ID = "PanelSubHistoricos";
            pPanelSubHistoricos.CssClass = "PanelSubHistoricos";
            pPanelSubHistoricos.Controls.Clear();

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + " = new Historico('#" + this.ClientID + "','" + pjsObj + "');");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "New", lsb.ToString(), true, false);

            if (!String.IsNullOrEmpty(iCodEntidad) && !String.IsNullOrEmpty(iCodMaestro))
            {
                pFields = (HistoricFieldCollection)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricFieldCollection)).CodeBase, CollectionClass).Unwrap();
                ((CnfgTarifaFieldCollection)pFields).InitCollectionTarifa(this, int.Parse(iCodEntidad), int.Parse(iCodMaestro), pTablaAtributos, this.ValidarPermiso, "KeytiaWeb.UserInterface.CnfgTarifaCollectionEdit", "KeytiaWeb.UserInterface.CnfgSubHistoricParamField");
                pFields.InitFields();
                IniciaVigencia(false);
            }

            if (pFields != null && pFields.ContainsConfigName("PlanServ") && pFields.ContainsConfigName("Region"))
            {
                pFields.GetByConfigName("PlanServ").DSOControlDB.AddClientEvent(HtmlTextWriterAttribute.Onchange.ToString(), pjsObj + ".filtrarTarifas();");
                pFields.GetByConfigName("Region").DSOControlDB.AddClientEvent(HtmlTextWriterAttribute.Onchange.ToString(), pjsObj + ".filtrarTarifas();");
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            if (State != HistoricState.CnfgSubHistoricField)
            {
                StringBuilder lsb = new StringBuilder();
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".bPermisoAgregar = \"" + ValidarPermiso(Permiso.Agregar).ToString() + "\";");
                lsb.AppendLine(pjsObj + ".deshabilitarRegistro();");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj, lsb.ToString(), true, false);
                lsb.Length = 0;
            }
        }
    }
}