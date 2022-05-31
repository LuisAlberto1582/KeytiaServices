using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface
{
    public class CnfgDashboard : HistoricEdit
    {
        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            pFields.GetByConfigName("URL").DataValue = "~/UserInterface/DashboardFC/Dashboard.aspx";
            pFields.GetByConfigName("URL").DisableField();
        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            pFields.GetByConfigName("URL").DataValue = "~/UserInterface/DashboardFC/Dashboard.aspx";
            pFields.GetByConfigName("URL").DisableField();
        }
    }
}
