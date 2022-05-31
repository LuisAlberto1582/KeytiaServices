using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public class EmpleRelationFieldCollection : RelationFieldCollection
    {
        public EmpleRelationFieldCollection()
        {
        }

        public EmpleRelationFieldCollection(int liCodEntidad, int liCodRelacion)
            : base(liCodEntidad, liCodRelacion) { }

        public EmpleRelationFieldCollection(WebControl lContainer, int liCodEntidad, int liCodRelacion, Table lTablaAtributos, ValidacionPermisos lValidarPermisos)
            : base(lContainer, liCodEntidad, liCodRelacion, lTablaAtributos, lValidarPermisos) { }

        public override void InitFields()
        {
            base.InitFields();
            if (ContainsConfigName("Exten"))
            {
                RelationAutoCompleteField lField = (RelationAutoCompleteField)GetByConfigName("Exten");
                lField.SubHistoricClass = "KeytiaWeb.UserInterface.CnfgExtenEdit";
                ((DSOAutocomplete)lField.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchRecursoRest");
            }
            if (ContainsConfigName("CodAuto"))
            {
                RelationAutoCompleteField lField = (RelationAutoCompleteField)GetByConfigName("CodAuto");
                lField.SubHistoricClass = "KeytiaWeb.UserInterface.CnfgCodigosAutorizacionEdit";
                ((DSOAutocomplete)lField.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchRecursoRest");
            }
            if (ContainsConfigName("Linea"))
            {
                RelationAutoCompleteField lField = (RelationAutoCompleteField)GetByConfigName("Linea");
                lField.SubHistoricClass = "KeytiaWeb.UserInterface.CnfgLineasRelEdit";
                ((DSOAutocomplete)lField.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchRecursoRest");
            }
        }

        public static string SearchRecursoRest(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                int liCodUsuario = (int)HttpContext.Current.Session["iCodUsuario"];
                int liCodPerfil = (int)HttpContext.Current.Session["iCodPerfil"];
                DateTime ldtVigenciaRest = DateTime.Today;

                string lsLang = Globals.GetCurrentLanguage();
                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

                StringBuilder lsbQuery = new StringBuilder();
                string lsQuery = "select top (100) * from [VisHistoricos('currentlsEntidad','currentLanguage')]";
                //lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
                lsQuery = lsQuery.Replace("currentlsEntidad", lsEntidad);
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

                if (ldtVista.Columns.Contains("Sitio"))
                {
                    //20170614 NZ Se cambia funcion
                    //lsWhere = lsWhere + "\r\nand Sitio in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestriccionVigencia(" + liCodUsuario + "," + liCodPerfil + ",'Sitio','" + ldtVigenciaRest.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigenciaRest.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))";
                    lsWhere = lsWhere + "\r\nand Sitio in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestricPorEntidad(" + liCodUsuario + "," + liCodPerfil + ",'Sitio','" + ldtVigenciaRest.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigenciaRest.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))";
                }

                lsbQuery.Append(lsWhere);
                lsbQuery.Append(lsOrder);

                lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.Append(")");
                lsbQuery.AppendLine(lsOrder);
                ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = HistoricEdit.FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

    }
}
