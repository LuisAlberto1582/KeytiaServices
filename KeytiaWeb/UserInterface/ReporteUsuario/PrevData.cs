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
    public class PrevData
    {
        private int piShowGraph = 0;

        [DataMember(Name = "g")]
        public int ShowGraph
        {
            get { return piShowGraph; }
            set { piShowGraph = value; }
        }
    }
}