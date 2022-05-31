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
    public class ReporteUsuarioTab
    {
        private int piIndex;
        private string psTitle;
        private string psContentId;
        private string psPanelId;

        public ReporteUsuarioTab(int liIndex, string lsTitle, string lsContentId, string lsPanelId)
        {
            piIndex = liIndex;
            psTitle = lsTitle;
            psContentId = lsContentId;
            psPanelId = lsPanelId;
        }

        [DataMember(Name = "index")]
        public int Id
        {
            get { return piIndex; }
            set { piIndex = value; }
        }

        [DataMember(Name = "title")]
        public string Title
        {
            get { return psTitle; }
            set { psTitle = value; }
        }

        [DataMember(Name = "contentId")]
        public string ContentId
        {
            get { return psContentId; }
            set { psContentId = value; }
        }

        [DataMember(Name = "panelId")]
        public string PanelId
        {
            get { return psPanelId; }
            set { psPanelId = value; }
        }
    }
}