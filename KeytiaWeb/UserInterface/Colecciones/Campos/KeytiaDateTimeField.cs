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
    public class KeytiaDateTimeField : KeytiaBaseField
    {
        protected DSODateTimeBox pDSOdate;

        public KeytiaDateTimeField() { }

        public override int ConfigValue
        {
            get
            {
                return base.ConfigValue;
            }
            set
            {
                base.ConfigValue = value;
                if (pColumn == "dtIniVigencia")
                {
                    pConfigName = "dtIniVigencia";
                }
                else if (pColumn == "dtFinVigencia")
                {
                    pConfigName = "dtFinVigencia";
                }
                else if (pColumn == "dtFecUltAct")
                {
                    pConfigName = "dtFecUltAct";
                }
            }
        }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pDSOdate;
            }
        }

        public override void CreateField()
        {
            pDSOdate = new DSODateTimeBox();
            InitDSOControlDB();
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            if (pColumn == "dtIniVigencia")
            {
                pDescripcion = Globals.GetMsgWeb(false, "dtIniVigencia");
            }
            else if (pColumn == "dtFinVigencia")
            {
                pDescripcion = Globals.GetMsgWeb(false, "dtFinVigencia");
            }
            else if (pColumn == "dtFecUltAct")
            {
                pDescripcion = Globals.GetMsgWeb(false, "dtFecUltAct");
            }
        }

        protected override void InitDSOControlDB()
        {
            base.InitDSOControlDB();
            pDSOdate.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateTimeFormat"));
        }
    }

    public class KeytiaDateField : KeytiaDateTimeField
    {
        public KeytiaDateField() { }

        public override void CreateField()
        {
            base.CreateField();
            pDSOdate.ShowHour = false;
            pDSOdate.ShowMinute = false;
            pDSOdate.ShowSecond = false;
        }

        protected override void InitDSOControlDB()
        {
            base.InitDSOControlDB();
            pDSOdate.AlertFormat = Globals.GetMsgWeb(false, "DateAlertFormat", Globals.GetMsgWeb(false, "NetDateFormat"));
        }
    }

    public class KeytiaMonthField : KeytiaDateField
    {
        public KeytiaMonthField() { }

        public override object DataValue
        {
            get
            {
                return base.DataValue;
            }
            set
            {
                base.DataValue = value;
                pDSOdate.AddClientEvent("monthDayValue", pDSOdate.Date.Day.ToString());
            }
        }

        public override void CreateField()
        {
            base.CreateField();
            pDSOdate.ShowCalendar = false;
            pDSOdate.ShowCurrent = true;
            //pDSOdate.DateFormat = "MM/yyyy";
            //pDSOdate.DateFormatJS = "M/yy" + new string(' ', 100) + "/dd";
            pDSOdate.AddClientEvent("onChangeMonthYear", "DSOControls.DateTimeBox.setMonthDay");
        }

        public override void InitField()
        {
            base.InitField();
            if(String.IsNullOrEmpty(pDSOdate.GetClientEvent("monthDayValue")))
            {
                pDSOdate.AddClientEvent("monthDayValue","1");
            }
        }
    }

    public class KeytiaTimeField : KeytiaDateTimeField
    {
        public KeytiaTimeField() { }

        public override object DataValue
        {
            get
            {
                if (pDSOdate.HasValue)
                {
                    return pDSOdate.DataValueDelimiter + pDSOdate.Date.ToString("HH:mm:ss") + pDSOdate.DataValueDelimiter;
                }
                else
                {
                    return base.DataValue;
                }
            }
            set
            {
                base.DataValue = value;
            }
        }

        public override void CreateField()
        {
            base.CreateField();
            pDSOdate.TimeOnly = true;
            pDSOdate.ShowHour = true;
            pDSOdate.ShowMinute = true;
            pDSOdate.ShowSecond = true;
            pDSOdate.TimeFormat = "HH:mm:ss tt";
        }
    }
}
