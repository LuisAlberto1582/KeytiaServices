using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Reflection;
using System.Web.Services;
using System.Collections.Generic;


namespace KeytiaWeb.UserInterface
{
    public class CnfgBloqueDashboard : HistoricEdit
    {
        protected override bool ValidarDatos()
        {
            int liAplic = 0;
            //Si se esta eliminando entonces no es necesario hacer esta validacion
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            if (pFields.GetByConfigName("Aplic").DataValue.ToString() != "null")
            {
                liAplic = int.Parse(pFields.GetByConfigName("Aplic").DataValue.ToString());
            }
            string lsLang = Globals.GetCurrentLanguage();
            StringBuilder lsbQuery = new StringBuilder();
            string lsMainQuery = "select * from [esquema].[VisHistoricos('BloqueDashboard','Bloque de dashboard','lenguaje')]".Replace("esquema", DSODataContext.Schema);
            lsMainQuery = lsMainQuery.Replace("lenguaje", lsLang);
            lsMainQuery += "\r\nwhere Aplic = " + liAplic;
            lsMainQuery += DSOControl.ComplementaVigenciasJS(pdtIniVigencia.Date, pdtFinVigencia.Date);
            lsMainQuery += "\r\nand iCodRegistro <> isnull(";
            lsMainQuery += iCodRegistro;
            lsMainQuery += ",0)";
            DataTable ldtBloques = DSODataAccess.Execute(lsMainQuery);
            if (ldtBloques != null && ldtBloques.Rows.Count > 0)
            {
                string lsError = "<li><ul>";
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                //Validar que no se repitan las consultas default para los bloques del dashboard
                int liConsul = int.Parse(pFields.GetByConfigName("Consul").DataValue.ToString());
                if (ldtBloques.Select("Consul = " + liConsul).Length > 0)
                {
                    lsError += DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrBDshConsul", pFields.GetByConfigName("Consul").ToString()));
                    lsError += "</ul></li>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    return false;
                }
                //Validar que no se repita el orden de bloques
                int liOrdenBloque = int.Parse(pFields.GetByConfigName("OrdenBloque").DataValue.ToString());
                if (ldtBloques.Select("OrdenBloque = " + liOrdenBloque).Length > 0)
                {
                    lsError += DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrBDshOrden", pFields.GetByConfigName("OrdenBloque").DataValue.ToString()));
                    lsError += "</ul></li>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    return false;
                }
            }
            return base.ValidarDatos();
        }

        #region WebMethods

        public static string SearchConsultaDashboard(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();

                StringBuilder lsbQuery = new StringBuilder();
                string lsQuery = "select top (100) * from [VisHistoricos('Aplic','Consultas','currentLanguage')]";
                lsQuery = lsQuery.Replace("currentLanguage", lsLang);

                DataTable ldtVista = DSODataAccess.Execute(lsQuery + " where 1 = 2");

                lsbQuery.Append(lsQuery);
                lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\n");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia));

                string lsWhere = "\r\n  and (vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                string lsOrder = "\r\norder by vchDescripcion";
                if (ldtVista.Columns.Contains(lsLang))
                {
                    lsWhere = lsWhere + " or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                    lsOrder = "\r\norder by " + lsLang + ",vchDescripcion";
                }
                lsWhere = lsWhere + ")";

                lsbQuery.Append(lsWhere);
                lsbQuery.Append(lsOrder);

                lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo)");
                lsbQuery.AppendLine(lsOrder);
                ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        #endregion
    }
}
