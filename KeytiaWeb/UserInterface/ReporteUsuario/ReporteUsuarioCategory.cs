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
    public class ReporteUsuarioCategory
    {
        private int piId;
        private string psName = "";
        private string psControlId = "";

        public ReporteUsuarioCategory(int liId, string lsName, string lsControlId)
        {
            piId = liId;
            psName = lsName;
            psControlId = lsControlId;
        }

        [DataMember(Name = "id")]
        public int Id
        {
            get { return piId; }
            set { piId = value; }
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get { return psName; }
            set { psName = value; }
        }

        [DataMember(Name = "controlId")]
        public string ControId
        {
            get { return psControlId; }
            set { psControlId = value; }
        }
    }
}