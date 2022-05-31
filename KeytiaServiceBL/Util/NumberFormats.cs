using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;

namespace KeytiaServiceBL
{
    public enum CurrencyPosition
    {
        Prefix,
        Suffix
    }

    public class KeytiaNumberFormats
    {
        protected KDBAccess pKDB = new KDBAccess();

        protected NumberFormatInfo pFormatInfo;
        protected string pStringFormat;
        protected string pOfficeStringFormat; // Formato para gráficas
        protected DataTable pdtNumberFormats;
        protected int piNumberDigits;
        protected bool pbPadding;

        public KeytiaNumberFormats()
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

                    if (lRowFormat["{CurrencyPosition}"] == DBNull.Value || (int)lRowFormat["{CurrencyPosition}"] == (int)CurrencyPosition.Prefix)
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
                pOfficeStringFormat = "";

                if (lRowFormat["{CurrencyPosition}"] == DBNull.Value || (int)lRowFormat["{CurrencyPosition}"] == (int)CurrencyPosition.Prefix)
                {
                    pStringFormat += lRowFormat["{CurrencySymbol}"].ToString();
                    pOfficeStringFormat += "\"" + lRowFormat["{CurrencySymbol}"].ToString().Replace("\"", "\"\\\"\"") + "\"";
                }

                if (lRowFormat["{GroupSeparator}"].ToString() != "")
                {
                    pStringFormat += "#,0";
                    pOfficeStringFormat += "#,0";
                }
                else
                {
                    pStringFormat += "0";
                    pOfficeStringFormat += "0";
                }

                if ((int)lRowFormat["{DecimalDigits}"] > 0)
                {
                    pStringFormat += ".";
                    pOfficeStringFormat += ".";
                    if (!(lRowFormat["{DecimalPadding}"] == DBNull.Value) && (int)lRowFormat["{DecimalPadding}"] != 0)
                    {
                        pStringFormat += new string('0', (int)lRowFormat["{DecimalDigits}"]);
                        pOfficeStringFormat += new string('0', (int)lRowFormat["{DecimalDigits}"]);
                    }
                    else
                    {
                        pStringFormat += new string('#', (int)lRowFormat["{DecimalDigits}"]);
                        pOfficeStringFormat += new string('#', (int)lRowFormat["{DecimalDigits}"]);
                    }
                }

                if (lRowFormat["{CurrencyPosition}"] == DBNull.Value || (int)lRowFormat["{CurrencyPosition}"] == (int)CurrencyPosition.Suffix)
                {
                    pStringFormat += lRowFormat["{CurrencySymbol}"].ToString();
                    pOfficeStringFormat += lRowFormat["{CurrencySymbol}"].ToString();
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

        public string OfficeStringFormat
        {
            get
            {
                return pOfficeStringFormat;
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

    }
}
