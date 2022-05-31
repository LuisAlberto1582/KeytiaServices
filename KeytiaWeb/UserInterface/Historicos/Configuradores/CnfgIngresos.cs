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
    public class CnfgIngresos : HistoricEdit
    {
        protected override void AgregarRegistro()
        {
            PrevState = State;
            SetHistoricState(HistoricState.Edicion);
            pFields.EnableFields();

            pdtIniVigencia.DataValue = DateTime.Today;
            pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            string codigo = Session["iCodUsuario"] + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            pvchCodigo.TextBox.Text = codigo;
            pvchDescripcion.TextBox.Text = codigo;
            pvchCodigo.TextBox.Enabled = false;
            pvchDescripcion.TextBox.Enabled = false;
            pdtIniVigencia.DateTimeBox.Enabled = false;
            pdtFinVigencia.DateTimeBox.Enabled = false;
        }


    }
}
