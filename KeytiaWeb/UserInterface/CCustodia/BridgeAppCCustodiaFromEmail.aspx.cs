using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.CCustodia
{
    public partial class BridgeAppCCustodiaFromEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sbiCodEmple = new StringBuilder();
            sbiCodEmple.AppendLine("select iCodCatalogo from [VisHistoricos('Emple','Empleados','Español')]");
            sbiCodEmple.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbiCodEmple.AppendLine("and dtfinvigencia >= getdate()");
            sbiCodEmple.AppendLine("and Usuar = " + HttpContext.Current.Session["iCodUsuario"].ToString());
            DataRow driCodEmple = DSODataAccess.ExecuteDataRow(sbiCodEmple.ToString());

            string iCodEmple = driCodEmple["iCodCatalogo"].ToString();

            HttpContext.Current.Response.Redirect("~/UserInterface/CCustodia/AppCCustodia.aspx?Opc=OpcAppCCustodia&st=QX8U9CM550Q%3D&iCodEmple=" + iCodEmple);
        }
    }
}
