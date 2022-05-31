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
    public static class KeytiaComparingOperators
    {
        public static DSODropDownList AddOperatorsDropDown(Control lCtl)
        {
            DSODropDownList lDSOddl = new DSODropDownList();
            lDSOddl.ID = lCtl.ID + "operddl";
            lCtl.Controls.Add(lDSOddl);

            lDSOddl.CreateControls();
            lDSOddl.DropDownList.CssClass += " DSODropDownOpers";
            lDSOddl.DataSource = GetOperators();
            lDSOddl.Fill();
            return lDSOddl;
        }

        public static DataTable GetOperators()
        {
            DataTable ldt = new DataTable();
            ldt.Columns.Add(new DataColumn("value", typeof(string)));
            ldt.Columns.Add(new DataColumn("text", typeof(string)));

            AddOperator("=", "=", ldt);
            AddOperator("<>", "<>", ldt);
            AddOperator("<", "<", ldt);
            AddOperator(">", ">", ldt);
            AddOperator("<=", "<=", ldt);
            AddOperator(">=", ">=", ldt);

            return ldt;
        }

        public static void AddOperator(string lsValue, string lsText, DataTable ldt)
        {
            DataRow ldataRow = ldt.NewRow();
            ldataRow["value"] = lsValue;
            ldataRow["text"] = lsText;
            ldt.Rows.Add(ldataRow);
        }

        public static object SetComparingValue(object lValue, DSODropDownList lDSOOperador)        
        {
            object lRet = lValue;
            if (lValue is string)
            {
                foreach (ListItem litemVal in lDSOOperador.DropDownList.Items)
                {
                    if (lValue.ToString().StartsWith(litemVal.Value + " "))
                    {
                        lDSOOperador.DataValue = litemVal.Value;
                        lRet = lValue.ToString().Substring(litemVal.Value.Length).Trim();
                        break;
                    }
                }
            }
            return lRet;
        }
    }

    #region Numericos

    public class KeytiaCompNumericField : KeytiaNumericField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);                
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    public class KeytiaCompIntegerField : KeytiaIntegerField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    public class KeytiaCompIntegerFormatField : KeytiaIntegerFormatField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    public class KeytiaCompCurrencyField : KeytiaCurrencyField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    #endregion

    #region Fecha y hora

    public class KeytiaCompDateTimeField : KeytiaDateTimeField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    public class KeytiaCompDateField : KeytiaDateField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    public class KeytiaCompMonthField : KeytiaMonthField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    public class KeytiaCompTimeField : KeytiaTimeField
    {
        DSODropDownList pDSOOperador;

        public override object DataValue
        {
            get
            {
                return pDSOOperador.DropDownList.SelectedValue + " " + base.DataValue;
            }
            set
            {
                base.DataValue = KeytiaComparingOperators.SetComparingValue(value, pDSOOperador);
            }
        }

        public override void InitField()
        {
            base.InitField();
            pDSOOperador = KeytiaComparingOperators.AddOperatorsDropDown(DSOControlDB.TcLbl);
        }

        public override void DisableField()
        {
            base.DisableField();
            ((WebControl)pDSOOperador.Control).Enabled = false;
        }

        public override void EnableField()
        {
            base.EnableField();
            ((WebControl)pDSOOperador.Control).Enabled = true;
        }
    }

    #endregion
}
