using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Entregables
{
    public partial class DescargaManuales : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int contador = 0;
            DataTable ldt = GetDataTable();

            for (int i = 0; i < ldt.Rows.Count; i++)
            {
                if (contador == 0)
                {
                    botonesDescarga.Controls.Add(new LiteralControl("<div class=\"manual_estilo\">"));
                }
                else if (contador == 3)
                {
                    botonesDescarga.Controls.Add(new LiteralControl("</div><div class=\"manual_estilo\">"));
                    contador = 0;
                }

                LinkButton test = new LinkButton();
                test.CssClass = "btn_descarga";
                test.ID = "btnManual" + ldt.Rows[i][0];
                test.Text = "<span>" + ldt.Rows[i][0] + "</span>";
                test.OnClientClick = $"window.open('{ldt.Rows[i][1]}'); return false";

                botonesDescarga.Controls.Add(test);
                contador += 1;
            }

            botonesDescarga.Controls.Add(new LiteralControl("</div>"));
        }

        protected DataTable GetDataTable()
        {
            var query = new StringBuilder();

            query.Append("select (isnull(banderasEmple, 0) & 2) / 2 as EsVIP \n");
            query.Append("from [vishistoricos('Emple','Empleados','Español')] Emple \n");
            query.Append("where dtfinvigencia >= getdate() \n");
            query.Append($"and Usuar = {Session["iCodUsuario"]} \n");
            var drEsVIP = DSODataAccess.Execute(query.ToString());

            query.Length = 0;
            query.Append("SELECT Nombre, RutaArchivo \n");
            query.Append("FROM [vishistoricos('DocumentoParaDescarga','Documentos para descarga','Español')] Docto \n");
            query.Append("JOIN [visRelaciones('Perfil-DocumentoParaDescarga','Español')] Rel \n");
            query.Append("  ON Docto.icodcatalogo = Rel.DocumentoParaDescarga \n");
            query.Append("WHERE Rel.dtinivigencia <> Rel.dtfinvigencia AND Docto.dtfinvigencia >= GETDATE() \n");
            query.Append("AND Perfil = " + Session["iCodPerfil"].ToString() + "\n");

            if (drEsVIP != null && drEsVIP.Rows.Count > 0 && drEsVIP.Rows[0]["EsVIP"].ToString() == "1")
            {
                query.Append("and Docto.vchCodigo <> 'ManualDeUsuarioEstandar'");
            }

            query.Append(" UNION ALL \n");

            query.Append("SELECT Nombre, RutaArchivo \n");
            query.Append("FROM [vishistoricos('DocumentoParaDescarga','Documentos para descarga','Español')] Docto \n");
            query.Append("JOIN [visRelaciones('Usuar-DocumentoParaDescarga','Español')] Rel \n");
            query.Append("  ON Docto.icodcatalogo = Rel.DocumentoParaDescarga \n");
            query.Append("WHERE Rel.dtinivigencia <> Rel.dtfinvigencia AND Docto.dtfinvigencia >= GETDATE() \n");
            query.Append("AND Usuar = " + Session["iCodUsuario"].ToString() + "\n");

            if (drEsVIP != null && drEsVIP.Rows.Count > 0 && drEsVIP.Rows[0]["EsVIP"].ToString() == "1")
            {
                query.Append("and Docto.vchCodigo <> 'ManualDeUsuarioEstandar'");
            }

            DataTable ldt = KeytiaServiceBL.DSODataAccess.Execute(query.ToString());

            return ldt;
        }
    }
}