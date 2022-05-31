using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface
{
    public class CnfgConsultasEdit : HistoricEdit
    {
        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            pFields.GetByConfigName("URL").DataValue = "~/UserInterface/Consultas/Consultas.aspx";
            pFields.GetByConfigName("URL").DisableField();
        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            pFields.GetByConfigName("URL").DataValue = "~/UserInterface/Consultas/Consultas.aspx";
            pFields.GetByConfigName("URL").DisableField();
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControls2008.DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgValorConsultasEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgValorConsultasEdit.js") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            if (pSubHistorico != null && pSubHistorico.Fields != null && pSubHistorico is CnfgValorConsultasEdit)
            {
                KeytiaBaseField lFieldAtributo = pSubHistorico.Fields.GetByConfigName("Atrib");
                KeytiaBaseField lFieldAtributoRel = ((KeytiaRelationField)pFields.GetByConfigName("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte")).Fields.GetByConfigName("Atrib");
                lFieldAtributo.DataValue = lFieldAtributoRel.DataValue;
            }
        }

        protected override bool ValidarRelCatBlancos(string lsRelacion)
        {
            if (lsRelacion == "Aplicación - Estado - Perfil - Atributo - Consulta - Reporte")
            {
                return true;
            }
            else
            {
                return base.ValidarRelCatBlancos(lsRelacion);
            }
        }
    }
}
