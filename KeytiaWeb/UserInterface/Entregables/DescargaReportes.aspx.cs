using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;
using System.Text;
using System.Linq.Expressions;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Configuration;

namespace KeytiaWeb.UserInterface.Entregables
{
    class ReporteDescargable
    {
        public int ICodCatalogo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public partial class DescargaReportes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            DataTable ldtReportesDisponibles = new DataTable();

            if (!IsPostBack)
            {
                ldtReportesDisponibles = ObtieneReportes(0);

                DataTable ldtAnios = ObtieneAnios();
                DataTable ldtMeses = ObtieneMeses();
                DataTable ldtTiposReporte = ObtieneTiposReporte();

                int liCodCatMesAnterior = 0;
                int liMesAnterior = DateTime.Now.AddMonths(-1).Month;

                if (ldtMeses != null && ldtMeses.Rows.Count > 0)
                {
                    DataRow drMes = ldtMeses.Select(" Mes = '" + liMesAnterior.ToString() + "'").FirstOrDefault();
                    liCodCatMesAnterior = (int)drMes["icodCatMes"];
                }

                cmbAnio.DataSource = ldtAnios;
                cmbAnio.DataTextField = "Anio";
                cmbAnio.DataValueField = "icodCatAnio";
                if (liMesAnterior != 12)
                {
                    cmbAnio.SelectedIndex = 1;
                }
                else { cmbAnio.SelectedIndex = 0; }
                cmbAnio.DataBind();

                cmbMes.DataSource = ldtMeses;
                cmbMes.DataTextField = "Nombre";
                cmbMes.DataValueField = "icodCatMes";
                cmbMes.SelectedValue = liCodCatMesAnterior.ToString();
                cmbMes.DataBind();

                cmbTipoReporte.DataSource = ldtTiposReporte;
                cmbTipoReporte.DataTextField = "TipoReporteDesc";
                cmbTipoReporte.DataValueField = "icodCatTipoReporte";
                cmbTipoReporte.DataBind();
            }
            else
            {
                ldtReportesDisponibles = ObtieneReportes(Convert.ToInt32(cmbTipoReporte.SelectedValue)); //Obtiene los reportes de todos los tipos de reporte
            }

            pnlReportesDisp.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("gvReportesDisp", ldtReportesDisponibles, false, "", new string[] { "", "", "", "" }),
                                      "gvReportesDisp_T", "Reportes disponibles")
                      );
        }

        private DataTable ObtieneTiposReporte()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select 'A' as ordenamiento, 0 as icodCatTipoReporte,  ");
            sb.AppendLine("				'------------------------------' asTipoReporteCod, ");
            sb.AppendLine("				'------------------------------' as TipoReporteDesc  ");
            sb.AppendLine("UNION ");
            sb.AppendLine("select distinct 'B' as ordenamiento, TipoReporte.iCodCatalogo as icodCatTipoReporte,  ");
            sb.AppendLine("				TipoReporte.vchCodigo as TipoReporteCod, ");
            sb.AppendLine("				TipoReporte.Descripcion as TipoReporteDesc  ");
            sb.AppendLine("from [vishistoricos('TipoReporteDescargable','Tipos Reporte Descargable','Español')] TipoReporte ");
            sb.AppendLine("join [vishistoricos('ReporteDescargable','Reportes Descargables','Español')] Reporte ");
            sb.AppendLine("	on TipoReporte.iCodCatalogo = Reporte.TipoReporteDescargable ");
            sb.AppendLine("	and TipoReporte.dtinivigencia<>TipoReporte.dtfinvigencia ");
            sb.AppendLine("	and TipoReporte.dtfinvigencia>=getdate() ");
            sb.AppendLine("join [visRelaciones('Reporte Descargable - Usuario','Español')] ReporteUsuar ");
            sb.AppendLine("	on Reporte.icodcatalogo = ReporteUsuar.ReporteDescargable ");
            sb.AppendLine("	and ReporteUsuar.dtinivigencia <> ReporteUsuar.dtfinvigencia ");
            sb.AppendLine("	and ReporteUsuar.dtfinvigencia >= getdate() ");
            sb.AppendLine("where Reporte.dtinivigencia <> Reporte.dtfinvigencia ");
            sb.AppendLine("and Reporte.dtfinvigencia >= getdate() ");
            sb.AppendLine("and ReporteUsuar.Usuar = " + Session["iCodUsuario"]);
            sb.AppendLine("order by Ordenamiento, TipoReporteDesc ");

            return DSODataAccess.Execute(sb.ToString());
        }

        private DataTable ObtieneMeses()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select icodCatalogo as icodCatMes, vchcodigo as Mes, [Español] as Nombre ");
            sb.AppendLine(" from [vishistoricos('Mes','Meses','español')] ");
            sb.AppendLine(" where dtinivigencia<>dtfinvigencia ");
            sb.AppendLine(" and dtfinvigencia>=getdate() ");
            sb.AppendLine(" order by convert(int,vchcodigo )");

            return DSODataAccess.Execute(sb.ToString());
        }

        private DataTable ObtieneAnios()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select icodCatalogo as icodCatAnio, vchcodigo as Anio  ");
            sb.AppendLine(" from [vishistoricos('anio','Años','español')] ");
            sb.AppendLine(" where dtinivigencia<>dtfinvigencia ");
            sb.AppendLine(" and dtfinvigencia>=getdate() ");
            sb.AppendLine(" and convert(int, vchcodigo)>= datepart(yyyy, getdate()) - 1 ");
            sb.AppendLine(" and convert(int, vchcodigo)<= datepart(yyyy, getdate()) ");
            sb.AppendLine(" order by convert(int,vchcodigo) ");

            return DSODataAccess.Execute(sb.ToString());
        }

        private ReporteDescargable ObtieneNombreArchivoADescargar(int iCodCatAnio, int iCodCatMes, int iCodCatTipoReporte)
        {
            ReporteDescargable reporte = new ReporteDescargable();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select icodcatalogo as iCodCatReporte, Descripcion, NomArchivo ");
            sb.AppendLine(" from [vishistoricos('ReporteDescargable','Reportes Descargables','español')] ");
            sb.AppendLine(" where dtinivigencia<>dtfinvigencia ");
            sb.AppendLine(" and dtfinvigencia>=getdate() ");
            sb.AppendLine(" and anio = " + iCodCatAnio.ToString());
            sb.AppendLine(" and mes = " + iCodCatMes.ToString());
            sb.AppendLine(" and TipoReporteDescargable = " + iCodCatTipoReporte.ToString());

            DataTable ldtReporte = DSODataAccess.Execute(sb.ToString());

            if (ldtReporte != null && ldtReporte.Rows.Count > 0)
            {
                DataRow ldr = ldtReporte.Rows[0]; //primer registro que regresa la consulta

                reporte.ICodCatalogo = int.Parse(ldr["iCodCatReporte"].ToString());
                reporte.Nombre = ldr["NomArchivo"].ToString();
                reporte.Descripcion = ldr["Descripcion"].ToString();

            }

            return reporte;
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            int iCodCatAnio = int.Parse(cmbAnio.SelectedValue);
            int iCodCatMes = int.Parse(cmbMes.SelectedValue);
            int iCodCatTipoReporte = int.Parse(cmbTipoReporte.SelectedValue);

            ReporteDescargable reporte = ObtieneNombreArchivoADescargar(iCodCatAnio, iCodCatMes, iCodCatTipoReporte);

            if (reporte.ICodCatalogo > 0)
            {
                //lblnombreArchivo.Text = "";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("content-disposition", "attachment; filename=" + reporte.Nombre);
                Response.WriteFile(@ConfigurationManager.AppSettings["carpetaDescargas"].ToString() + reporte.Nombre);
                Response.End();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append("No se ha encontrado un archivo que coincida con la búsqueda seleccionada.");
                sb.Append("')};");
                sb.Append("</script>");

                ClientScript.RegisterClientScriptBlock(this.GetType(), "Aviso", sb.ToString());
            }
        }

        public DataTable ObtieneReportes(int iCodCatTipoRep)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("select Reporte.AnioCod as Anio, Reporte.MesDesc as Mes, TipoReporte.Descripcion, Reporte.NomArchivo  ");
            sb.AppendLine("from [vishistoricos('TipoReporteDescargable','Tipos Reporte Descargable','Español')] TipoReporte ");
            sb.AppendLine("join [vishistoricos('ReporteDescargable','Reportes Descargables','Español')] Reporte ");
            sb.AppendLine("	on TipoReporte.iCodCatalogo = Reporte.TipoReporteDescargable ");
            sb.AppendLine("	and TipoReporte.dtinivigencia<>TipoReporte.dtfinvigencia ");
            sb.AppendLine("	and TipoReporte.dtfinvigencia>=getdate() ");
            sb.AppendLine("join [visRelaciones('Reporte Descargable - Usuario','Español')] ReporteUsuar ");
            sb.AppendLine("	on Reporte.icodcatalogo = ReporteUsuar.ReporteDescargable ");
            sb.AppendLine("	and ReporteUsuar.dtinivigencia <> ReporteUsuar.dtfinvigencia ");
            sb.AppendLine("	and ReporteUsuar.dtfinvigencia >= getdate() ");
            sb.AppendLine("where Reporte.dtinivigencia <> Reporte.dtfinvigencia ");
            sb.AppendLine("and Reporte.dtfinvigencia >= getdate() ");
            sb.AppendLine("and ReporteUsuar.Usuar = " + Session["iCodUsuario"]);

            if (iCodCatTipoRep > 0)
            {
                sb.AppendLine(" and TipoReporte.icodcatalogo = " + iCodCatTipoRep.ToString());
            }

            sb.AppendLine("order by TipoReporte.Descripcion, Reporte.Anio, Reporte.Mes ");

            return DSODataAccess.Execute(sb.ToString());
        }

        protected void cmbTipoReporte_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
