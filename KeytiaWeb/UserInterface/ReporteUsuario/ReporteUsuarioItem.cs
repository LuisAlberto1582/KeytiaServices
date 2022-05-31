using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DSOControls2008;
using KeytiaServiceBL;
using KeytiaCOM;

namespace KeytiaWeb.UserInterface
{
    [DataContract]
    public class ReporteUsuarioItem
    {
        private int piId = -1;
        private string psCode = "";
        private string psName = "";

        public ReporteUsuarioItem(int liId, string lsName)
        {
            piId = liId;
            psName = lsName;
        }

        public ReporteUsuarioItem(string lsCode, string lsName)
        {
            psCode = lsCode;
            psName = lsName;
        }

        [DataMember(Name = "id")]
        public int Id
        {
            get { return piId; }
            set { piId = value; }
        }

        [DataMember(Name = "code")]
        public string Code
        {
            get { return psCode; }
            set { psCode = value; }
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get { return psName; }
            set { psName = value; }
        }
    }
}