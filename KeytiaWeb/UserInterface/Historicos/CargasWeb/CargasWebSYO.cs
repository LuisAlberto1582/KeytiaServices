using System;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;
using System.Collections;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CargasWebSYO : CargasWeb
    {
        protected override void LimpiaCamposUltimaCarga()
        {
            base.LimpiaCamposUltimaCarga();

            pFields.GetByConfigName("RegVCSDet").DataValue = "0";
            pFields.GetByConfigName("RegVCSPend").DataValue = "0";
            pFields.GetByConfigName("RegMCUDet").DataValue = "0";
            pFields.GetByConfigName("RegMCUPend").DataValue = "0";

            pFields.GetByConfigName("FecIniCargaVC").DataValue = DBNull.Value;
            pFields.GetByConfigName("FecFinCargaVC").DataValue = DBNull.Value;
            pFields.GetByConfigName("DurCargaVC").DataValue = DBNull.Value;

        }
    }
}
