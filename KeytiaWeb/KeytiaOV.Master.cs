using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DSOControls2008;
using System.Text;
using System.Web.UI.HtmlControls;

namespace KeytiaWeb
{
    public partial class KeytiaOV : KeytiaMasterPage
    {
        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EnsureChildControls();

                //**20131105.PT: se ocultan estos botones si el usuardb es nextel 79124
                if ((int)Session["iCodUsuarioDB"] == 79124)
                {
                    imgToolUsrCurrency.Visible = false;
                    imgToolUsrLanguage.Visible = false;
                    imgToolUsrPassword.Visible = false;
                }

                //Sperto y el usuario CemexUSA
                if ((int)Session["iCodUsuarioDB"] == 94427 && (int)Session["iCodUsuario"] == 243580)
                {
                    imgToolUsrCurrency.Visible = false;
                    imgToolUsrLanguage.Visible = false;
                    imgToolUsrPassword.Visible = false;
                }

                //RJ.20170809 Esta funcionalidad no está implementada con FusionCharts, por lo tanto se desactiva
                imgToolUsrLanguage.Visible = false;

                cboCurrency.DataSource = Globals.GetLangHT("Moneda", "Monedas");
                cboCurrency.DataValueField = "key";
                cboCurrency.DataTextField = "value";
                cboCurrency.DataBind();
                cboCurrency.SelectedValue = Globals.GetCurrentCurrency();

                cboLanguage.DataSource = Globals.GetLangHT("Idioma", "Idioma");
                cboLanguage.DataValueField = "key";
                cboLanguage.DataTextField = "value";
                cboLanguage.DataBind();
                cboLanguage.SelectedValue = Globals.GetCurrentLanguage();

                //RM20190226 Busca bandera menu colapsado
                GetCollapseOption();
            }

            PublicarFechasAlFront();
        }

        protected virtual void btnLanguageOk_Click(object sender, EventArgs e)
        {
            Globals.SetLanguage(cboLanguage.SelectedValue);
            Response.Redirect(Request.RawUrl);
        }

        protected virtual void btnLanguageCancel_Click(object sender, EventArgs e)
        {
            cboLanguage.SelectedValue = Globals.GetCurrentLanguage();
        }

        protected virtual void btnCurrencyOk_Click(object sender, EventArgs e)
        {
            Globals.SetCurrency(cboCurrency.SelectedValue);

            //20140217 PT: Se actualiza la moneda en el registro del usuario
            KeytiaServiceBL.DSODataAccess.ExecuteNonQuery(
                " update " + KeytiaServiceBL.DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','Español')]" +
                " set Moneda = (select iCodCatalogo " +
                "               from " + KeytiaServiceBL.DSODataContext.Schema + ".[VisHistoricos('Moneda','Monedas','Español')]" +
                "               where vchCodigo = '" + cboCurrency.SelectedValue + "' " +
                "               and dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE()), " +
                " dtFecUltAct = GETDATE()" +
                " where iCodCatalogo = " + Session["iCodUsuario"]);
            Response.Redirect(Request.RawUrl);
        }

        protected virtual void btnCurrencyCancel_Click(object sender, EventArgs e)
        {
            cboCurrency.SelectedValue = Globals.GetCurrentCurrency();
        }

        protected virtual void btnPasswordCancel_Click(object sender, EventArgs e)
        {
            txtCurrentPassword.Text = "";
            txtNewPassword.Text = "";
            txtNewPassword2.Text = "";

            wndPassword.Display = false;
            wndPassword.InitOnReady = false;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            DSONavegador1.CreateControls();

            wndLanguage.CreateControls();
            wndLanguage.Content.Controls.Add(wndLanguageContent);

            wndCurrency.CreateControls();
            wndCurrency.Content.Controls.Add(wndCurrencyContent);

            wndPassword.CreateControls();
            wndPassword.Content.Controls.Add(wndPasswordContent);


            //Ventana de cambio de password
            DSOControl.LoadControlScriptBlock(Page, this.GetType(), wndPassword.ID,
                "<script language='javascript'>\r\n" +
                "var wndpw_" + wndPassword.ClientID + ";\r\n" +
                "$(document).ready(function() {\r\n" +
                "   wndpw_" + wndPassword.ClientID + " = new KeytiaMaster.Password(\"" + wndPassword.Content.ClientID + "\");\r\n" +
                "});\r\n" +
                "</script>\r\n",
                false, false);

            btnPasswordOk.Attributes.Add(
                "onclick",
                "wndpw_" + wndPassword.ClientID + ".change(" +
                    "'" + txtCurrentPassword.ClientID + "'," +
                    "'" + txtNewPassword.ClientID + "'," +
                    "'" + txtNewPassword2.ClientID + "');" +
                    "return false;");

            btnPasswordCancel.Attributes.Add(
                "onclick",
                "wndpw_" + wndPassword.ClientID + ".cancel(" +
                    "'" + txtCurrentPassword.ClientID + "'," +
                    "'" + txtNewPassword.ClientID + "'," +
                    "'" + txtNewPassword2.ClientID + "');" +
                "return false;");

            ChildControlsCreated = true;
        }

        public override void InitMasterControls()
        {
            base.InitMasterControls();
            AddJsFechas();           
        }

        public virtual void AddJsFechas() 
        {
            //Calendario en Rango
            DSOControl.LoadControlScriptBlock(Page, this.GetType(), "scriptsFechas",
                "<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/DateRangePicker/datePickerHeader.js?v=1") + "'></script>",
                false, false);
        }

        protected override void InitLanguage()
        {
            wndLanguage.Title = Globals.GetLangItem("SelIdiomaTit");
            lblLanguageMsg.Text = Globals.GetLangItem("SelIdioma");
            lblLanguage.Text = Globals.GetLangItem("Idioma");
            btnLanguageOk.Text = Globals.GetLangItem("btnAceptar");
            btnLanguageCancel.Text = Globals.GetLangItem("btnCancelar");

            wndCurrency.Title = Globals.GetLangItem("SelMonedaTit");
            lblCurrencyMsg.Text = Globals.GetLangItem("SelMoneda");
            lblCurrency.Text = Globals.GetLangItem("Moneda");
            btnCurrencyOk.Text = Globals.GetLangItem("btnAceptar");
            btnCurrencyCancel.Text = Globals.GetLangItem("btnCancelar");

            wndPassword.Title = Globals.GetLangItem("ChgPasswordTit");
            lblPasswordMsg.Text = Globals.GetLangItem("ChgPassword");
            lblCurrentPassword.Text = Globals.GetLangItem("PwdActual");
            lblNewPassword.Text = Globals.GetLangItem("PwdNueva");
            lblNewPassword2.Text = Globals.GetLangItem("PwdNuevaConf");
            btnPasswordOk.Text = Globals.GetLangItem("btnAceptar");
            btnPasswordCancel.Text = Globals.GetLangItem("btnCancelar");

            Globals.ChangeComboLanguage(cboLanguage, "Idioma", "Idioma");
            Globals.ChangeComboLanguage(cboCurrency, "Moneda", "Monedas");

            InitLanguageMessages();
        }

        protected virtual void InitLanguageMessages()
        {
            List<string> llsMsgs = new List<string>();

            foreach (string s in Globals.GetUserMessages())
                llsMsgs.Add(HttpUtility.HtmlEncode(s));

            if (llsMsgs.Count == 0)
                llsMsgs.Add("");
        }

        protected virtual void btnAplicarFecha_Click(object sender, EventArgs e)
        {
        }

        private void PublicarFechasAlFront()
        {
            DateTime fechaI;
            DateTime fechaF;
                if (DateTime.TryParse(HttpContext.Current.Session["FechaInicio"].ToString(), out fechaI) &&
                DateTime.TryParse(HttpContext.Current.Session["FechaFin"].ToString(), out fechaF))
                {
                    hfFechaInicio.Value = fechaI.ToString("dd/MM/yyyy");
                    hfFechaFin.Value = fechaF.ToString("dd/MM/yyyy");
                }
        }

        public void ChangeCalendar(bool twoCalendar) 
        {
            txtFechas.Attributes.Add("twoCalendar", twoCalendar ? "true" : "false");
        }

        public void GetCollapseOption()
        {
            hdOpcCollapse.Value = HttpContext.Current.Session["CollapseValue"].ToString();
        }
        

    }
}
