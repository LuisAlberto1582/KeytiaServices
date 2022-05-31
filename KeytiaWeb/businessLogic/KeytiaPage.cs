using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

using KeytiaServiceBL;
using KeytiaWeb.UserInterface;

namespace KeytiaWeb
{
    public class KeytiaPage : System.Web.UI.Page
    {
        protected KDBAccess kdb = new KDBAccess();
        private HtmlLink pCSS;

        protected KeytiaPage()
        {
            PreInit += new EventHandler(KeytiaPage_PreInit);
            PreRender += new EventHandler(KeytiaPage_PreRender);
            PreRenderComplete += new EventHandler(KeytiaPage_PreRenderComplete);
        }

        protected void KeytiaPage_PreInit(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
                Response.Redirect("~", true);

            if (HttpContext.Current.Session["OpcionesUsuario"] != null && !OptionIsValid())
                AccessError();

            if (KMaster != null)
            {
                this.MasterPageFile = (string)HttpContext.Current.Session["MasterPage"];
                this.KMaster.Export += new KeytiaExportEventHandler(Page_Export);
            }
        }

        protected void KeytiaPage_PreRender(object sender, EventArgs e)
        {
            if (!OptionIsValid())
                AccessError();

            InitLanguage();
        }

        protected void KeytiaPage_PreRenderComplete(object sender, EventArgs e)
        {
            DSOControls2008.DSODateTimeBox.setRegion(Page, Globals.GetDateTimeLanguage());
        }

        public KeytiaMasterPage KMaster
        {
            get
            {
                KeytiaMasterPage lkmpMaster = null;

                if (this.Master != null && this.Master is KeytiaMasterPage)
                    lkmpMaster = (KeytiaMasterPage)Master;

                return lkmpMaster;
            }
        }

        public virtual void SetStyleSheet(string lsStyleSheet)
        {
            if (pCSS == null)
            {
                pCSS = new HtmlLink();
                pCSS.Attributes.Add("rel", "stylesheet");
                pCSS.Attributes.Add("type", "text/css");
                Page.Header.Controls.Add(pCSS);
            }

            pCSS.Href = lsStyleSheet;
        }

        protected virtual void Page_Export(KeytiaExportFormat lkeFormat)
        {
            DSOControls2008.DSOControl.LoadControlScriptBlock(Page, typeof(KeytiaPage), "KPExportDefault",
                "<script language=\"javascript\">" +
                "$(document).ready(function() { jAlert('" + Globals.GetLangItem("ExpNoDisp") + "'); });" +
                "</script>\r\n",
                false, false);
        }

        protected virtual void InitLanguage()
        {

        }

        protected virtual bool OptionIsValid()
        {
            bool lbRet = false;

            if (DSONavegador.getPermiso(HttpContext.Current.Request.Params["Opc"]) != "Restringir")
            {
                //Valida que la url corresponda a la aplicación de la opción
                int liCodAplic = (int)Util.IsDBNull(KDBUtil.SearchScalar("OpcMnu", (string)HttpContext.Current.Request.Params["Opc"], "{Aplic}"), -1);
                string lsUrl = (string)Util.IsDBNull(KDBUtil.SearchScalar("Aplic", liCodAplic, "{URL}"), "");

                if (Request.MapPath(lsUrl).Equals(Request.PhysicalPath, StringComparison.InvariantCultureIgnoreCase))
                    lbRet = true;
            }

            return lbRet;
        }

        protected void AccessError()
        {
            Util.LogMessage(
                "El usuario intentó entrar ilegalmente a la opción '" + HttpContext.Current.Request.Params["Opc"] + "' " +
                "a través de la URL '" + Request.RawUrl + "'\r\n");

            Response.Redirect("~/Error.aspx?T=ErrAcc&M=AccRestr");
        }
    }
}
