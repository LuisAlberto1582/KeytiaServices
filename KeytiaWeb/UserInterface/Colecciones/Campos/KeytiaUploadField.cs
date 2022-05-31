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
    public class KeytiaUploadField : KeytiaBaseField
    {
        protected DSOUpload pDSOupl;

        public override bool ShowInGrid
        {
            get
            {
                return true;
            }
        }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pDSOupl;
            }
        }

        public override void CreateField()
        {
            pDSOupl = new DSOUpload();
            InitDSOControlDB();
        }
    }

}
