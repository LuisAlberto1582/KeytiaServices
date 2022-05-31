using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface
{
    public class CnfgConsultasCartasCustodia : HistoricEdit
    {
        protected string psCodEmpleado = "";

        public CnfgConsultasCartasCustodia()
        {
            Init += new EventHandler(CnfgConsultasCartasCustodia_Init);
        }

        protected virtual void CnfgConsultasCartasCustodia_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgConsultasCartasCustodia";
        }

        protected override void pbtnRegresar_ServerClick(object sender, EventArgs e)
        {
            base.pbtnRegresar_ServerClick(sender, e);
            Session["iCodEmpleado"] = null;
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            if (State == HistoricState.Consulta)
            {
                pPanelSubHistoricos.Visible = true;
            }
        }

        public override void ConsultarRegistro()
        {
            base.ConsultarRegistro();
            if (pFields.ContainsConfigName("Emple"))
            {
                KeytiaBaseField lField = pFields.GetByConfigName("Emple");
                if (lField.DataValue.ToString() != null && lField.DataValue.ToString() != "null")
                {
                    Session["iCodEmpleado"] = lField.DataValue.ToString().Replace("'", "");
                }
            }
        }
    }
}
