using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb
{
    public partial class Error : KeytiaPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void InitLanguage()
        {
            base.InitLanguage();

            string lsTit = "ErrorTit";
            string lsMsg = "ErrorGen";

            if (HttpContext.Current.Request.Params["T"] != null)
                lsTit = HttpContext.Current.Request.Params["T"];

            if (HttpContext.Current.Request.Params["M"] != null)
                lsMsg = HttpContext.Current.Request.Params["M"];

            lblErrorTitle.Text = Globals.GetLangItem(lsTit);
            lblErrorMessage.Text = Globals.GetLangItem(lsMsg);
        }

        protected override bool OptionIsValid()
        {
            return true;
        }
    }
}
