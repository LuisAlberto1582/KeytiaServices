using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DownloadGuiaEtiq : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            var path = Util.AppSettings("FolderTemp");
            string filePath = "";
            filePath = Path.Combine(path, "Guía rápida para proceso de etiquetación 27ago2019.pdf");
            if (File.Exists(filePath))
            {
                var buffer = File.ReadAllBytes(filePath);
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("Content-Type", "application/octet-stream");
                Response.AddHeader("Content-disposition", "attachment; filename=\"" + Path.GetFileName(filePath) + "\"");
                Response.BinaryWrite(buffer);
                Response.ContentType = "application/octet-stream";
                Response.Flush();
                buffer = null;
            }
        }
    }
}