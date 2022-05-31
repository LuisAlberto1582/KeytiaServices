using System;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web;
using System.Collections.Generic;
using System.Reflection;

namespace KeytiaWeb.UserInterface
{
    public partial class Consultas : KeytiaPage
    {
        protected ReporteEstandar Reporte;
        protected KDBAccess pKDB = new KDBAccess();

        public Consultas()
        {
            Init += new EventHandler(Consultas_Init);
        }

        protected void Consultas_Init(object sender, EventArgs e)
        {
            try
            {
                int liCodPerfil = (int)Session["iCodPerfil"]; //pKDB.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo = " + Session["iCodUsuario"]).Rows[0]["{Perfil}"];
                int liCodAplicacion = (int)pKDB.GetHisRegByCod("OpcMnu", new string[] { Request.Params["Opc"].Replace("'", "''") }).Rows[0]["{Aplic}"];
                StringBuilder lsb = new StringBuilder();
                lsb.AppendLine("{Aplic} = " + liCodAplicacion);
                lsb.AppendLine("and {EstadoConsulta} is null");
                lsb.AppendLine("and {Perfil} = " + liCodPerfil);
                lsb.AppendLine("and {Atrib} is null");
                lsb.AppendLine("and {Consul} is null");
                lsb.AppendLine("and {RepEst} is not null");
                lsb.AppendLine("and {Ruta} is null");

                DataTable lKDBTable = pKDB.GetRelRegByDes("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte", lsb.ToString());

                Reporte = new ReporteEstandar();
                Reporte.ID = "Reporte1";
                Reporte.OpcMnu = Request.Params["Opc"];
                Reporte.lblTitle = lblTitle;
                Reporte.iCodConsulta = liCodAplicacion;
                Reporte.iCodReporte = (int)lKDBTable.Rows[0]["{RepEst}"];
                //Reporte.iCodPerfil = liCodPerfil;
                Reporte.iEstadoConsulta = 0;
                Reporte.ParentContainer = Content;

                Content.Controls.Add(Reporte);
                Reporte.LoadScripts();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrInitControls", ex);
            }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            Reporte.CreateControls();

            //NZ
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            switch (lkeFormat)
            {
                case KeytiaExportFormat.xlsx:
                    Reporte.ExportXLS();
                    break;
                case KeytiaExportFormat.docx:
                    Reporte.ExportDOC();
                    break;
                case KeytiaExportFormat.pdf:
                    Reporte.ExportPDF();
                    break;
                case KeytiaExportFormat.csv:
                    Reporte.ExportCSV();
                    break;
            }
        }

        protected override void InitLanguage()
        {
            base.InitLanguage();
            Reporte.Title = Globals.GetLangItem("OpcMnu", "Opciones de Menu", Reporte.OpcMnu.Replace("'", "''"));
            Reporte.InitLanguage();
        }

    }
}
