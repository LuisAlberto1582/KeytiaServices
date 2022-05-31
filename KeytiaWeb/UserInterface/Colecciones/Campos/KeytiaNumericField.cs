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
    public class KeytiaNumericField : KeytiaBaseField
    {
        protected DSONumberEdit pDSOnum;
        protected NumberFormatInfo pFormatInfo;
        protected string pStringFormat;
        protected DataTable pdtNumberFormats;
        protected int piNumberDigits;
        protected bool pbPadding;

        public KeytiaNumericField()
        {
            if (DSODataContext.GetObject("NumberFormats") == null)
            {
                pdtNumberFormats = pKDB.GetHisRegByEnt("NumberFormats", "Formatos numéricos");
                DSODataContext.SetObject("NumberFormats", pdtNumberFormats);
            }
            else
            {
                pdtNumberFormats = (DataTable)DSODataContext.GetObject("NumberFormats");
            }
            InitFormatInfo();
        }

        protected virtual void InitFormatInfo()
        {
            SetFormatInfo("Flotantes");
        }

        public void SetFormatInfo(string lvchCodFormat)
        {
            if (pdtNumberFormats.Select("vchCodigo = '" + lvchCodFormat + "'").Length > 0)
            {
                DataRow lRowFormat = pdtNumberFormats.Select("vchCodigo = '" + lvchCodFormat + "'")[0];
                pFormatInfo = new NumberFormatInfo();

                if (lRowFormat["{DecimalDigits}"] == DBNull.Value)
                {
                    pFormatInfo.NumberDecimalDigits = 0;
                }
                else
                {
                    pFormatInfo.NumberDecimalDigits = (int)lRowFormat["{DecimalDigits}"];
                }
                pFormatInfo.NumberDecimalSeparator = lRowFormat["{DecimalSeparator}"].ToString();
                pFormatInfo.NumberGroupSeparator = lRowFormat["{GroupSeparator}"].ToString();
                if (lRowFormat["{GroupSize}"] == DBNull.Value)
                {
                    pFormatInfo.NumberGroupSizes = new int[] { 3 };
                }
                else
                {
                    pFormatInfo.NumberGroupSizes = new int[] { (int)lRowFormat["{GroupSize}"] };
                }
                pFormatInfo.NumberNegativePattern = 1; //-n  

                if (lRowFormat["{CurrencySymbol}"] != DBNull.Value)
                {
                    pFormatInfo.CurrencyDecimalDigits = (int)lRowFormat["{DecimalDigits}"];
                    pFormatInfo.CurrencyDecimalSeparator = lRowFormat["{DecimalSeparator}"].ToString();
                    pFormatInfo.CurrencyGroupSeparator = lRowFormat["{GroupSeparator}"].ToString();
                    pFormatInfo.CurrencyGroupSizes = new int[] { (int)lRowFormat["{GroupSize}"] };

                    if (lRowFormat["{CurrencyPosition}"] == DBNull.Value || (int)lRowFormat["{CurrencyPosition}"] == (int)DSONumberEditCurrencyPosition.Prefix)
                    {
                        pFormatInfo.CurrencyNegativePattern = 1; //-$n
                        pFormatInfo.CurrencyPositivePattern = 0; //$n
                    }
                    else
                    {
                        pFormatInfo.CurrencyNegativePattern = 5; //-n$
                        pFormatInfo.CurrencyPositivePattern = 1; //n$
                    }
                    pFormatInfo.CurrencySymbol = lRowFormat["{CurrencySymbol}"].ToString();
                }
                else
                {
                    pFormatInfo.CurrencySymbol = "";
                }

                piNumberDigits = (int)lRowFormat["{NumberDigits}"];
                pbPadding = !(lRowFormat["{DecimalPadding}"] == DBNull.Value) && (int)lRowFormat["{DecimalPadding}"] != 0;

                pStringFormat = "";

                if (lRowFormat["{CurrencyPosition}"] == DBNull.Value || (int)lRowFormat["{CurrencyPosition}"] == (int)DSONumberEditCurrencyPosition.Prefix)
                {
                    pStringFormat += lRowFormat["{CurrencySymbol}"].ToString();
                }

                if (lRowFormat["{GroupSeparator}"].ToString() != "")
                {
                    pStringFormat += "#,0";
                }
                else
                {
                    pStringFormat += "0";
                }

                if ((int)lRowFormat["{DecimalDigits}"] > 0)
                {
                    pStringFormat += ".";
                    if (!(lRowFormat["{DecimalPadding}"] == DBNull.Value) && (int)lRowFormat["{DecimalPadding}"] != 0)
                    {
                        pStringFormat += new string('0', (int)lRowFormat["{DecimalDigits}"]);
                    }
                    else
                    {
                        pStringFormat += new string('#', (int)lRowFormat["{DecimalDigits}"]);
                    }
                }

                if (lRowFormat["{CurrencyPosition}"] == DBNull.Value || (int)lRowFormat["{CurrencyPosition}"] == (int)DSONumberEditCurrencyPosition.Suffix)
                {
                    pStringFormat += lRowFormat["{CurrencySymbol}"].ToString();
                }
            }
        }

        public NumberFormatInfo FormatInfo
        {
            get
            {
                return pFormatInfo;
            }
        }

        public string StringFormat
        {
            get
            {
                return pStringFormat;
            }
        }

        public int NumberDigits
        {
            get
            {
                return piNumberDigits;
            }
        }

        public bool Padding
        {
            get
            {
                return pbPadding;
            }
        }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pDSOnum;
            }
        }

        public override void CreateField()
        {
            pDSOnum = new DSONumberEdit();
            InitDSOControlDB();
        }

        protected override void InitDSOControlDB()
        {
            base.InitDSOControlDB();
            pDSOnum.FormatInfo = pFormatInfo;
            pDSOnum.NumberDigits = piNumberDigits;
            pDSOnum.Padding = pbPadding;
        }
    }

    public class KeytiaIntegerField : KeytiaNumericField
    {
        public KeytiaIntegerField() { }

        protected override void InitFormatInfo()
        {
            SetFormatInfo("Enteros");
        }
    }

    public class KeytiaIntegerFormatField : KeytiaNumericField
    {
        public KeytiaIntegerFormatField() { }

        protected override void InitFormatInfo()
        {
            SetFormatInfo("EnterosFormat");
        }
    }

    public class KeytiaCurrencyField : KeytiaNumericField
    {
        public KeytiaCurrencyField() { }

        protected override void InitFormatInfo()
        {
            SetFormatInfo("MonedaDefault");
        }
    }

}
