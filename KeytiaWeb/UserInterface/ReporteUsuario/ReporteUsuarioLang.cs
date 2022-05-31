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
    public class ReporteUsuarioLang
    {
        List<ReporteUsuarioItem> plstItems = new List<ReporteUsuarioItem>();

        [DataMember(Name = "items")]
        public ReporteUsuarioItem[] Items
        {
            get { return plstItems.ToArray(); }
            set
            {
                if (plstItems == null)
                    plstItems = new List<ReporteUsuarioItem>();

                plstItems.Clear();

                foreach (ReporteUsuarioItem loItem in value)
                    plstItems.Add(loItem);
            }
        }

        public List<ReporteUsuarioItem> ItemsList
        {
            get { return plstItems; }
            set { plstItems = value; }
        }
    }
}