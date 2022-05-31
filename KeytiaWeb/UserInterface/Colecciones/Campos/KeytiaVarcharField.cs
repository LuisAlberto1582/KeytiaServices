using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Globalization;
using System.Web.SessionState;

namespace KeytiaWeb.UserInterface
{
    public class KeytiaVarcharField : KeytiaBaseField
    {
        protected DSOTextBox pDSOtxt;

        public KeytiaVarcharField() { }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pDSOtxt;
            }
        }

        public override void CreateField()
        {
            pDSOtxt = new DSOTextBox();
            InitDSOControlDB();
        }
    }

    public class KeytiaPasswordField : KeytiaVarcharField
    {
        //protected TripleDESWrapper pDesPSW;

        public KeytiaPasswordField() { }

        public override void CreateField()
        {
            base.CreateField();
            //pDesPSW = new TripleDESWrapper();
        }

        public override void InitField()
        {
            base.InitField();
            pDSOtxt.TextBox.TextMode = TextBoxMode.Password;
        }

        public override object DataValue
        {
            get
            {
                string lsRet;
                if (pDSOtxt.HasValue)
                {
                    //lsRet = pDesPSW.Encrypt(pDSOtxt.TextBox.Text);
                    lsRet = Util.Encrypt(pDSOtxt.TextBox.Text);
                }
                else
                {
                    lsRet = pDSOtxt.GetClientEvent("txtval");
                }
                if (String.IsNullOrEmpty(lsRet))
                {
                    lsRet = "null";
                }
                else
                {
                    lsRet = pDSOtxt.DataValueDelimiter + lsRet.Replace("'", "''") + pDSOtxt.DataValueDelimiter;
                }
                return lsRet;
            }
            set
            {
                //string lsValor = pDesPSW.Decrypt(value.ToString());
                string lsValor = Util.Decrypt(value.ToString());
                pDSOtxt.AddClientEvent("txtval", value.ToString());
                base.DataValue = lsValor;
            }
        }

        public override bool ShowInGrid
        {
            get
            {
                return false;
                //return true;
            }
        }
    }

    public class KeytiaMultiLineField : KeytiaVarcharField
    {
        public KeytiaMultiLineField() { }

        public override bool ShowInGrid
        {
            get
            {
                return false;
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOtxt.TextBox.TextMode = TextBoxMode.MultiLine;
        }
    }

}
