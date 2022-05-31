/*
 * Nombre:		    DMM
 * Fecha:		    20111019
 * Descripción:	    Clase para agregar subhistóricos de tipos de Recursos
 * Modificación:	
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using KeytiaServiceBL;
using DSOControls2008;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface
{
    public class CnfgRecursosFieldCollection : HistoricFieldCollection
    {
        protected override void AgregarSubHistoricos()
        {
            StringBuilder lsb = new StringBuilder();

            int liCodRegistro;
            int liCodEntidad = 0;
            int liCodAplic = 0;
            DataTable lDataTable;

            if (int.TryParse(((HistoricEdit)this.pContainer).iCodRegistro, out liCodRegistro) && liCodRegistro != 0)
            {
                lsb.Length = 0;
                lsb.Append("select * from " + DSODataContext.Schema);
                lsb.Append(".[VisHistoricos('Recurs','" + Globals.GetCurrentLanguage() + "')] ");
                lsb.Append("where iCodRegistro = " + liCodRegistro);
                lDataTable = DSODataAccess.Execute(lsb.ToString());

                if (lDataTable.Rows.Count == 0) return;

                if (lDataTable.Columns.Contains("Entidad"))
                {
                    liCodEntidad = (int)Util.IsDBNull(lDataTable.Rows[0]["Entidad"], 0);
                }
                if (lDataTable.Columns.Contains("Aplic"))
                {
                    liCodAplic = (int)Util.IsDBNull(lDataTable.Rows[0]["Aplic"], 0);
                }
                lsb.Length = 0;
                lsb.AppendLine("select iCodRegistro, iCodEntidad, vchDescripcion,");
                lsb.AppendLine("vchCodigo = (Select vchCodigo from Catalogos where iCodRegistro = iCodEntidad)");
                lsb.AppendLine("from Maestros");
                lsb.AppendLine("where iCodEntidad = " + liCodEntidad);
                lsb.AppendLine("and dtIniVigencia <> dtFinVigencia");
                pdtMaestros = DSODataAccess.Execute(lsb.ToString());

                foreach (DataRow ldr in pdtMaestros.Rows)
                {
                    string lsSubHistoricClass, lsSubCollectionClass;
                    lDataTable = pKDB.GetHisRegByEnt("Aplic", "", "iCodCatalogo = " + liCodAplic);
                    if (lDataTable.Rows.Count > 0)
                    {
                        lsSubHistoricClass = (string)Util.IsDBNull(lDataTable.Rows[0]["{ParamVarChar3}"], "KeytiaWeb.UserInterface.CnfgRecursosEdit");
                        lsSubCollectionClass = (string)Util.IsDBNull(lDataTable.Rows[0]["{ParamVarChar4}"], "KeytiaWeb.UserInterface.HistoricFieldCollection");
                    }
                    else
                    {
                        lsSubHistoricClass = "KeytiaWeb.UserInterface.CnfgRecursosEdit";
                        lsSubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";
                    }
                    AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubRecursosField", (int)ldr["iCodEntidad"], (int)ldr["iCodRegistro"], lsSubHistoricClass, lsSubCollectionClass);
                }
            }
        }

        protected override void CreateFields(bool lbOnlyGridFields)
        {
            base.CreateFields(lbOnlyGridFields);

            foreach (KeytiaBaseField lField in this)
            {
                if (lField is CnfgSubHistoricField)
                {
                    ((DSOGrid)((CnfgSubHistoricField)lField).DSOControlDB).Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetSubRecursosData");
                }
            }
        }

        public static DSOGridServerResponse GetSubRecursosData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            DateTime ldtIniVigencia, ldtFinVigencia;
            ldtIniVigencia = DateTime.Today;
            ldtFinVigencia = DateTime.Today.AddDays(1);

            DataTable ldtSitios = DSODataAccess.Execute(string.Format(
                //"Select iCodCatalogo from [{0}].GetRestriccionVigencia({1}, {2},'Sitio','{3}','{4}',1)", //20170614 NZ Se cambia funcion
                "Select iCodCatalogo from [{0}].GetRestricPorEntidad({1}, {2},'Sitio','{3}','{4}',default)", 
                DSODataContext.Schema,
                System.Web.HttpContext.Current.Session["iCodUsuario"].ToString(),
                System.Web.HttpContext.Current.Session["iCodPerfil"].ToString(),
                ldtIniVigencia.ToString("yyyy-MM-dd"),
                ldtFinVigencia.ToString("yyyy-MM-dd")));

            if (parametros == null)
            {
                parametros = new List<Parametro>();
            }

            Parametro lParam = new Parametro();
            lParam.Name = "Sitio";
            lParam.Value = CnfgHisRecursosEdit.DataTableToString(ldtSitios, "iCodCatalogo");
            parametros.Add(lParam);

            return HistoricEdit.GetSubHisData(gsRequest, iCodEntidad, iCodMaestro, parametros);
        }

    }

    public class CnfgSubRecursosField : CnfgSubHistoricField
    {
        protected override void InitGridLanguage()
        {
            base.InitGridLanguage();
            KeytiaBaseField lFieldEmple = null;
            if (pFields.ContainsConfigName("Emple"))
            {
                lFieldEmple = pFields.GetByConfigName("Emple");
                foreach (DSOGridClientColumn lCol in pSubHisGrid.Config.aoColumnDefs)
                {
                    if (lFieldEmple != null && lCol.sName == lFieldEmple.Column + "Desc")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "Responsable"));
                    }
                }
            }
        }

    }

    public class CnfgSubRecursosFieldCollection : HistoricFieldCollection
    {
        protected DataTable pTablaRelSitio; //Relaciones que tienen la entidad de Sitios en alguno de sus campos

        public CnfgSubRecursosFieldCollection()
        {
        }

        public CnfgSubRecursosFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) {}

        public CnfgSubRecursosFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void FillMetaData()
        {
            pTablaRelSitio = KeytiaRelationField.GetRelByAtrib("Sitio");
            base.FillMetaData();

            int liCodEntSitio = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Sitio'");
            if (pMetaData.Select("ConfigValue = " + liCodEntSitio).Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigValue = " + liCodEntSitio)[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAutoRestrictedField";
            }
        }

        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);
            lField.SubCollectionClass = "KeytiaWeb.UserInterface.CnfgSubRecursosFieldCollection";
            if (lField is KeytiaRelationField && pTablaRelSitio.Select("iCodRelacion = " + lField.ConfigValue).Length > 0)
            {
                ((KeytiaRelationField)lField).RelationCollectionClass = "KeytiaWeb.UserInterface.RecursosRelationFieldCollection";
            }
            return lField;
        }

        protected override void CreateFields(bool lbOnlyGridFields)
        {
            base.CreateFields(lbOnlyGridFields);

            foreach (KeytiaBaseField lField in this)
            {
                if (lField is KeytiaRelationField
                    && EsRelacionEmpleado((KeytiaRelationField)lField)
                    && CnfgEmpleadosFieldCollection.EsRelacionRecurso((KeytiaRelationField)lField))
                {
                    ((DSOGrid)((KeytiaRelationField)lField).DSOControlDB).Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetEmpleRelData");
                }
                else if (lField.ConfigName == "Sitio")
                {
                    KeytiaAutoCompleteField lacSitio = (KeytiaAutoCompleteField)lField;
                    if (((HistoricEdit)pContainer).vchCodEntidad != "Linea")
                    {
                        ((DSOAutocomplete)lacSitio.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/GetSitiosConmutadores");
                    }
                    //else
                    //{
                    //    ((DSOAutocomplete)lacSitio.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/GetSitiosNoConmutadores");
                    //}
                }
            }
        }

        public static bool EsRelacionEmpleado(KeytiaRelationField lRelField)
        {
            bool lbRet = false;
            if (lRelField.Fields.ContainsConfigName("Emple"))
            {
                lbRet = true;
            }
            return lbRet;
        }

        public static string GetSitiosConmutadores(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return GetConmutadores(term, iCodEntidad, iniVigencia, finVigencia, "=");
        }

        public static string GetSitiosNoConmutadores(string term, int iCodEntidad, object iniVigencia, object finVigencia)
        {
            return GetConmutadores(term, iCodEntidad, iniVigencia, finVigencia, "<>");
        }

        public static string GetConmutadores(string term, int iCodEntidad, object iniVigencia, object finVigencia, string operador)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloRecursos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                //restrictedValues = Util.Decrypt(restrictedValues).Replace("'", "''");

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

                string lsWhere;
                lsWhere = "\r\n  and IsNull(TipoSitioCod, 'SitPBX') " + operador + " 'SitPBX'" +
                          "\r\n  and (vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                string lsOrder = "\r\norder by vchDescripcion";
                if (ldtVista.Columns.Contains(lsLang))
                {
                    lsWhere = lsWhere + " or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'";
                    lsOrder = "\r\norder by " + lsLang + ",vchDescripcion";
                }
                lsWhere = lsWhere + ")";

                lsbQuery.Append(lsWhere);

                lsbQuery.Append("\r\n  and iCodCatalogo in(");
                if (iniVigencia == null || iniVigencia.ToString() == "null")
                {
                    iniVigencia = DateTime.Today;
                }

                if (finVigencia == null || finVigencia.ToString() == "null")
                {
                    finVigencia = new DateTime(2079, 01, 01);
                }

                lsbQuery.Append("select distinct(iCodCatalogo) from ");
                //20170614 NZ Se cambia funcion
                //lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestriccionVigencia(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestricPorEntidad(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                lsbQuery.Append(")");
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

    public class RecursosRelationFieldCollection : RelationFieldCollection
    {
        public RecursosRelationFieldCollection()
        {
        }

        public RecursosRelationFieldCollection(int liCodEntidad, int liCodRelacion)
            : base(liCodEntidad, liCodRelacion) { }

        public RecursosRelationFieldCollection(WebControl lContainer, int liCodEntidad, int liCodRelacion, Table lTablaAtributos, ValidacionPermisos lValidarPermisos)
            : base(lContainer, liCodEntidad, liCodRelacion, lTablaAtributos, lValidarPermisos) { }

        public override void InitFields()
        {
            base.InitFields();
            if (ContainsConfigName("Sitio"))
            {
                RelationAutoCompleteField lField = (RelationAutoCompleteField)GetByConfigName("Sitio");
                ((DSOAutocomplete)lField.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchCatRestricted");

                lField.SubHistoricClass = "KeytiaWeb.UserInterface.CnfgSitiosRestEdit";
            }
        }
    }
}
