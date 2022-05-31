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
using KeytiaServiceBL.Reportes;
 
namespace KeytiaWeb.UserInterface
{
    public class KeytiaAutoCompleteField : KeytiaBaseField, IKeytiaFillableAjaxField
    {
        protected DSOAutocomplete pDSOauto;

        public KeytiaAutoCompleteField() { }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pDSOauto;
            }
        }

        public override object DataValue
        {
            get
            {
                object lret;
                pDSOauto.IsDropDown = true;
                lret = pDSOauto.DataValue;
                pDSOauto.IsDropDown = false;
                return lret;
            }
            set
            {
                if (value != DBNull.Value)
                {
                    FillDSOAuto(GetDataValueSource(value.ToString()));
                }

                pDSOauto.DataValue = value;
            }
        }

        public override void CreateField()
        {
            pDSOauto = new DSOAutocomplete();
            InitDSOControlDB();

            pDSOauto.IsDropDown = false;
            pDSOauto.DataValueDelimiter = "";
            pDSOauto.MinLength = 0;
            pDSOauto.Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchCatReg");
            pDSOauto.FnSearch = "function(request, response){" + pjsObj + ".fnSearchCatReg(this, request, response); }";
            pDSOauto.AddClientEvent("iCodEntidad", pConfigValue.ToString());
        }

        public virtual void FillAjax()
        {
            psLang = "{" + Globals.GetCurrentLanguage() + "}";
            pDSOauto.IsDropDown = true;
            FillDSOAuto(GetFillAjaxSource());
            pDSOauto.IsDropDown = false;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pDescripcion == pConfigName)
            {
                pDescripcion = Globals.GetLangItem("", "Entidades", pConfigName);
            }
        }

        public override void InitField()
        {
            base.InitField();
            CargaScriptValidaVigencias();
        }

        public override void InitDSOControlDBLanguage()
        {
            base.InitDSOControlDBLanguage();

            pDSOauto.IsDropDown = true;
            DataValue = pDSOauto.DataValue;
            pDSOauto.IsDropDown = false;
        }

        protected virtual DataTable GetDataValueSource(string lsValue)
        {
            string lsLang = Globals.GetCurrentLanguage();
            //string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

            StringBuilder lsbQuery = new StringBuilder();
            string lsQuery = "select * from currentSchema.[VisHistoricos('currentlsEntidad','currentLanguage')]";
            lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
            lsQuery = lsQuery.Replace("currentlsEntidad", pConfigName);
            lsQuery = lsQuery.Replace("currentLanguage", lsLang);
            lsbQuery.Append(lsQuery);
            lsbQuery.Append("\r\nwhere iCodCatalogo = ");
            lsbQuery.Append(lsValue);
            lsbQuery.Append(DSOControl.ComplementaVigenciasJS(pIniVigencia, pFinVigencia, false));

            return DSODataAccess.Execute(lsbQuery.ToString());
            //return pKDB.GetHisRegByEnt(pConfigName, "", "iCodCatalogo = " + lsValue);
        }

        protected virtual DataTable GetFillAjaxSource()
        {
            string lsLang = Globals.GetCurrentLanguage();

            StringBuilder lsbQuery = new StringBuilder();
            string lsQuery = "select * from currentSchema.[VisHistoricos('currentlsEntidad','currentLanguage')]";
            lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
            lsQuery = lsQuery.Replace("currentlsEntidad", pConfigName);
            lsQuery = lsQuery.Replace("currentLanguage", lsLang);
            lsbQuery.Append(lsQuery);
            lsbQuery.Append("\r\nwhere iCodCatalogo = ");
            lsbQuery.Append(pDSOauto.Value);
            lsbQuery.Append(DSOControl.ComplementaVigenciasJS(pIniVigencia, pFinVigencia, false));
            lsbQuery.Append("\r\norder by vchDescripcion");
            return DSODataAccess.Execute(lsbQuery.ToString());
        }

        protected virtual void FillDSOAuto(DataTable lKDBTable)
        {
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("id", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("value", typeof(string)));
            string lsLang = Globals.GetCurrentLanguage();
            string lsKDBLang = "{" + lsLang + "}";

            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                DataRow lDataRow = lDataTable.NewRow();
                lDataRow["id"] = lKDBRow["iCodCatalogo"];
                if (lKDBTable.Columns.Contains(lsLang) && lKDBRow[lsLang] != DBNull.Value)
                {
                    lDataRow["value"] = lKDBRow[lsLang] + " (" + lKDBRow["vchCodigo"] + ")";
                }
                else if (lKDBTable.Columns.Contains(lsKDBLang) && lKDBRow[lsKDBLang] != DBNull.Value)
                {
                    lDataRow["value"] = lKDBRow[lsKDBLang] + " (" + lKDBRow["vchCodigo"] + ")";
                }
                else
                {
                    lDataRow["value"] = lKDBRow["vchDescripcion"] + " (" + lKDBRow["vchCodigo"] + ")";
                }
                lDataTable.Rows.Add(lDataRow);
            }

            DataView lViewData = lDataTable.DefaultView;
            DataTable lDataSource = lDataTable.Clone();
            lViewData.Sort = "value ASC, id ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                lDataSource.ImportRow(lViewRow.Row);
            }

            pDSOauto.DataSource = lDataSource;
            pDSOauto.Fill();
        }

        protected virtual void CargaScriptValidaVigencias()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + ".fnVigArray.push(function(){AutoComplete.validaVigencias.call(" + pjsObj + ", $('#" + pDSOauto.Search.ClientID + "', false))})");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricFieldCollection), pDSOauto.Search.ClientID + ".validaVigencias", lsb.ToString(), true, false);
        }

        public virtual string RestrictedValues
        {
            get
            {
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.Append("select distinct(iCodCatalogo) from ");
                //20170614 NZ Se cambia funcion
                //lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestriccionVigencia(" + Session["iCodUsuario"] + ", " + Session["iCodPerfil"] + ", '" + pConfigName.Replace("'", "''") + "', '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(pFinVigencia, false).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestricPorEntidad(" + Session["iCodUsuario"] + ", " + Session["iCodPerfil"] + ", '" + pConfigName.Replace("'", "''") + "', '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(pFinVigencia, false).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                return lsbQuery.ToString();
            }
        }

        #region WebMethods
        public static bool ValidaVigenciasAutoComplete(int iCodCatalogo, int iCodEntidad, int iRestrictedValues, object iniVigencia, object finVigencia)
        {
            bool lbRet = false;
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {

                string lsLang = Globals.GetCurrentLanguage();
                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

                StringBuilder lsbQuery = new StringBuilder();
                string lsQuery = "select * from currentSchema.[VisHistoricos('currentlsEntidad','currentLanguage')]";
                lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
                lsQuery = lsQuery.Replace("currentlsEntidad", lsEntidad);
                lsQuery = lsQuery.Replace("currentLanguage", lsLang);
                lsbQuery.Append(lsQuery);
                lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\n");
                lsbQuery.Append("\r\n  and iCodCatalogo = ");
                lsbQuery.Append(iCodCatalogo);
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia));

                if (iRestrictedValues == 1)
                {
                    lsbQuery.Append("\r\n  and iCodCatalogo in (");
                    lsbQuery.Append("select distinct(iCodCatalogo) from ");
                    //20170614 NZ Se cambia funcion
                    //lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestriccionVigencia(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                    lsbQuery.AppendLine(DSODataContext.Schema + ".GetRestricPorEntidad(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                    lsbQuery.Append(")");
                }

                DataTable ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                if (ldtVista != null && ldtVista.Rows.Count > 0)
                    lbRet = true;

                return lbRet;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static bool ValidaVigenciasAtribAutoComplete(int iCodCatalogo, object iniVigencia, object finVigencia)
        {
            bool lbRet = false;
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();

                StringBuilder lsbQuery = new StringBuilder();
                string lsQuery = "select * from currentSchema.[VisHistoricos('','Entidades','currentLanguage')]";
                lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
                lsQuery = lsQuery.Replace("currentLanguage", lsLang);
                lsbQuery.Append(lsQuery);
                lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\n");
                lsbQuery.Append("\r\n  and iCodCatalogo = ");
                lsbQuery.Append(iCodCatalogo);
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia));

                DataTable ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                if (ldtVista != null && ldtVista.Rows.Count > 0)
                    lbRet = true;
                else
                {
                    lsbQuery.Length = 0;
                    lsQuery = "select * from currentSchema.[VisHistoricos('Atrib','currentLanguage')]";
                    lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
                    lsQuery = lsQuery.Replace("currentLanguage", lsLang);
                    lsbQuery.Append(lsQuery);
                    lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\n");
                    lsbQuery.Append("\r\n  and iCodCatalogo = ");
                    lsbQuery.Append(iCodCatalogo);
                    lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia));
                    ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                    if (ldtVista != null && ldtVista.Rows.Count > 0)
                        lbRet = true;
                }

                return lbRet;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }
        #endregion
    }

    public class KeytiaAutoHierarchyField : KeytiaAutoCompleteField
    {
        protected string psValues;

        public KeytiaAutoHierarchyField() { }

        public override object DataValue
        {
            get
            {
                int liCodPadre;
                if (int.TryParse(base.DataValue.ToString(), out liCodPadre))
                {
                    if (String.IsNullOrEmpty(psValues))
                    {
                        psValues = DSODataAccess.ExecuteScalar("select valores = " + DSODataContext.Schema + ".GetJerarquiaEntidadV(" + pConfigValue + "," + liCodPadre + ",'" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "')").ToString();
                    }

                    return psValues;
                }
                else
                {
                    return "null";
                }
            }
            set
            {
                psValues = null;
                base.DataValue = value;
            }
        }

    }

    public class KeytiaAutoRestrictedField : KeytiaAutoCompleteField
    {
        public override void CreateField()
        {
            base.CreateField();
            pDSOauto.Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchCatRestricted");
            pDSOauto.FnSearch = "function(request, response){" + pjsObj + ".fnSearchCatRestricted(this, request, response); }";
        }

        protected override DataTable GetDataValueSource(string lsValue)
        {
            string lsLang = Globals.GetCurrentLanguage();
            //string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();

            StringBuilder lsbQuery = new StringBuilder();
            string lsQuery = "select * from currentSchema.[VisHistoricos('currentlsEntidad','currentLanguage')]";
            lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
            lsQuery = lsQuery.Replace("currentlsEntidad", pConfigName);
            lsQuery = lsQuery.Replace("currentLanguage", lsLang);
            lsbQuery.Append(lsQuery);
            lsbQuery.Append("\r\nwhere iCodCatalogo = ");
            lsbQuery.Append(lsValue);
            lsbQuery.Append("\r\n  and iCodCatalogo in(");
            lsbQuery.Append(RestrictedValues);
            lsbQuery.Append(")");
            lsbQuery.Append(DSOControl.ComplementaVigenciasJS(pIniVigencia, pFinVigencia, false));

            return DSODataAccess.Execute(lsbQuery.ToString());
            //return pKDB.GetHisRegByEnt(pConfigName, "", "iCodCatalogo in(" + RestrictedValues + ") and iCodCatalogo = " + lsValue);
        }

        protected override DataTable GetFillAjaxSource()
        {

            string lsLang = Globals.GetCurrentLanguage();

            StringBuilder lsbQuery = new StringBuilder();

            //20140605 AM. Cuando la entidad sea sitio se cambia VisHistoricos por VisHisComun
            string lsQuery = string.Empty;
            if (pConfigName == "Sitio")
            {
                lsQuery = "select * from currentSchema.[VisHisComun('currentlsEntidad','currentLanguage')]";
            }
            else
            {
                lsQuery = "select * from currentSchema.[VisHistoricos('currentlsEntidad','currentLanguage')]";
            }
            
            lsQuery = lsQuery.Replace("currentSchema", DSODataContext.Schema);
            lsQuery = lsQuery.Replace("currentlsEntidad", pConfigName);
            lsQuery = lsQuery.Replace("currentLanguage", lsLang);
            lsbQuery.Append(lsQuery);
            lsbQuery.Append("\r\nwhere iCodCatalogo = ");
            lsbQuery.Append(pDSOauto.Value);
            lsbQuery.Append("\r\n  and iCodCatalogo in(");
            lsbQuery.Append(RestrictedValues);
            lsbQuery.Append(")");
            lsbQuery.Append(DSOControl.ComplementaVigenciasJS(pIniVigencia, pFinVigencia, false));
            lsbQuery.Append("\r\norder by vchDescripcion");
            return DSODataAccess.Execute(lsbQuery.ToString());

            //return pKDB.GetHisRegByEnt(pConfigName, "", new string[] { "iCodCatalogo", "vchDescripcion", psLang }, "", "vchDescripcion," + psLang, 100, "iCodCatalogo in(" + RestrictedValues + ") and iCodCatalogo = " + pDSOauto.Value);
        }

        protected override void CargaScriptValidaVigencias()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + ".fnVigArray.push(function(){AutoComplete.validaVigencias.call(" + pjsObj + ", $('#" + pDSOauto.Search.ClientID + "', true))})");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricFieldCollection), pDSOauto.Search.ClientID + ".validaVigencias", lsb.ToString(), true, false);
        }

    }

    public class KeytiaAutoRestrictedHierarchyField : KeytiaAutoRestrictedField
    {
        protected string psValues;

        public KeytiaAutoRestrictedHierarchyField() { }

        public override object DataValue
        {
            get
            {
                int liCodPadre;
                if (int.TryParse(base.DataValue.ToString(), out liCodPadre))
                {
                    if (String.IsNullOrEmpty(psValues))
                    {
                        DataTable ldt = DSODataAccess.Execute("select iCodCatalogo from " + DSODataContext.Schema + ".GetJerarquiaEntidad(" + pConfigValue + "," + liCodPadre + ",'" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "') where iCodCatalogo in(" + RestrictedValues + ")");
                        List<string> lstValues = new List<string>();
                        foreach (DataRow lDataRow in ldt.Rows)
                        {
                            lstValues.Add(lDataRow["iCodCatalogo"].ToString());
                        }
                        psValues = String.Join(",", lstValues.ToArray());
                    }

                    return psValues;
                }
                else
                {
                    return "null";
                }
            }
            set
            {
                psValues = null;
                base.DataValue = value;
            }
        }

    }

    public class KeytiaAutoFilteredField : KeytiaAutoCompleteField
    {
        protected int piCodDataSourceParam;
        protected string psDataSourceControlValue;

        public int iCodDataSourceParam
        {
            get { return piCodDataSourceParam; }
            set 
            { 
                piCodDataSourceParam = value;
            }
        }

        public static DataTable getDataSource(int liCodDataSourceParam, string term)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            DataTable ldtDataSource = null;
            if (liCodDataSourceParam > 0)
            {
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select DataSourceControl");
                lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('DataSourceParam','DataSource Parametro','" + Globals.GetCurrentLanguage() + "')]");
                lsbQuery.AppendLine("where dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("and dtIniVigencia <= GetDate()");
                lsbQuery.AppendLine("and dtFinVigencia > GetDate()");
                lsbQuery.AppendLine("and iCodCatalogo = " + liCodDataSourceParam);

                string lsDataSource = DSODataAccess.ExecuteScalar(lsbQuery.ToString()).ToString();

                if (lsDataSource.Length > 0)
                {
                    Hashtable lHTParam = GetHTParamSession();
                    lHTParam["term"] = term;
                    lsDataSource = ReporteEstandarUtil.ParseDataSource(lsDataSource, lHTParam, true);

                    ldtDataSource = DSODataAccess.Execute(lsDataSource);
                    if (ldtDataSource.Columns.Count < 2)
                    {
                        throw new KeytiaWebException(true, "ErrorGen");
                    }

                    ldtDataSource.Columns[0].ColumnName = "id";
                    ldtDataSource.Columns[1].ColumnName = "value";
                }
            }
            return ldtDataSource;
        }

        protected static Hashtable GetHTParamSession()
        {
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select isnull(Empre.Client,0)");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','"+Globals.GetCurrentLanguage()+"')] Usuar,");
            lsbQuery.AppendLine("   " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre");
            lsbQuery.AppendLine("where Usuar.iCodCatalogo = " + HttpContext.Current.Session["iCodUsuario"]);
            lsbQuery.AppendLine("and Usuar.Empre = Empre.iCodCatalogo");
            lsbQuery.AppendLine("and Usuar.dtIniVigencia <> Usuar.dtFinVigencia");
            lsbQuery.AppendLine("and Usuar.dtIniVigencia < '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Usuar.dtFinVigencia >= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Empre.dtIniVigencia <> Empre.dtFinVigencia");
            lsbQuery.AppendLine("and Empre.dtIniVigencia < '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Empre.dtFinVigencia >= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            
            int liCodClient = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString(), (object)0);

            Hashtable lHTParam = new Hashtable();
            lHTParam["vchCodUsuario"] = HttpContext.Current.Session["vchCodUsuario"];
            lHTParam["iCodUsuario"] = HttpContext.Current.Session["iCodUsuario"];
            lHTParam["iCodPerfil"] = HttpContext.Current.Session["iCodPerfil"];
            lHTParam["vchCodIdioma"] = Globals.GetCurrentLanguage();
            lHTParam["vchCodMoneda"] = Globals.GetCurrentCurrency();
            lHTParam["Client"] = liCodClient;
            lHTParam["Schema"] = DSODataContext.Schema;

            return lHTParam;
        }

        public override void CreateField()
        {
            base.CreateField();
            pDSOauto.Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchCatFiltered");
            pDSOauto.FnSearch = "function(request, response){" + pjsObj + ".fnSearchCatFiltered(this, request, response); }";
        }

        protected override void FillDSOAuto(DataTable lKDBTable)
        {
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("id", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("value", typeof(string)));

            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                DataRow lDataRow = lDataTable.NewRow();
                lDataRow["id"] = lKDBRow[0];
                lDataRow["value"] = lKDBRow[1];
                lDataTable.Rows.Add(lDataRow);
            }

            pDSOauto.DataSource = lDataTable;
            pDSOauto.Fill();
        }

        protected override DataTable GetDataValueSource(string lsValue)
        {
            DataTable ldtDataSource = null;
            if (piCodDataSourceParam > 0)
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select DataSourceControlValue");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('DataSourceParam','DataSource Parametro','"+Globals.GetCurrentLanguage()+"')]");
                psbQuery.AppendLine("where dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("and dtIniVigencia <= GetDate()");
                psbQuery.AppendLine("and dtFinVigencia > GetDate()");
                psbQuery.AppendLine("and iCodCatalogo = " + piCodDataSourceParam);

                string lsDataSource = DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString();

                if (lsDataSource.Length > 0)
                {
                    Hashtable lHTParam = GetHTParamSession();
                    lHTParam["DataValue"] = lsValue;
                    lsDataSource = ReporteEstandarUtil.ParseDataSource(lsDataSource, lHTParam, true);

                    ldtDataSource = DSODataAccess.Execute(lsDataSource.ToString());
                    if (ldtDataSource.Columns.Count < 2)
                    {
                        throw new KeytiaWebException(true, "ErrorGen");
                    }

                    ldtDataSource.Columns[0].ColumnName = "id";
                    ldtDataSource.Columns[1].ColumnName = "value";
                }
            }

            return ldtDataSource;
        }

        protected override DataTable GetFillAjaxSource()
        {
            return GetDataValueSource(pDSOauto.Value.ToString());
        }

        public override void InitDSOControlDBLanguage()
        {
            DSOControlDB.AddClientEvent("keytiaFilter", piCodDataSourceParam.ToString());
            base.InitDSOControlDBLanguage();
        }
    }

    public class KeytiaEntityField : KeytiaAutoCompleteField
    {
        public KeytiaEntityField() { }

        public override void CreateField()
        {
            base.CreateField();
            pDSOauto.Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchEntity");
        }

        protected override DataTable GetDataValueSource(string lsValue)
        {
            DataTable lKDBTable;
            DataTable lRetTable = new DataTable();
            DataRow lRetRow;

            lRetTable.Columns.Add(new DataColumn("iCodCatalogo", typeof(int)));
            lRetTable.Columns.Add(new DataColumn("vchCodigo", typeof(string)));
            lRetTable.Columns.Add(new DataColumn("vchDescripcion", typeof(string)));
            lRetTable.Columns.Add(new DataColumn(psLang, typeof(string)));

            //Agrego las entidades
            lKDBTable = pKDB.GetHisRegByEnt("", "Entidades", "iCodCatalogo = " + lsValue);
            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                lRetRow = lRetTable.NewRow();
                foreach (DataColumn lRetCol in lRetTable.Columns)
                {
                    if (lKDBTable.Columns.Contains(lRetCol.ColumnName))
                    {
                        lRetRow[lRetCol] = lKDBRow[lRetCol.ColumnName];
                    }
                }
                lRetTable.Rows.Add(lRetRow);
            }

            return lRetTable;
        }

        protected override DataTable GetFillAjaxSource()
        {
            DataTable lKDBTable;
            DataTable lRetTable = new DataTable();
            DataRow lRetRow;

            lRetTable.Columns.Add(new DataColumn("iCodCatalogo", typeof(int)));
            lRetTable.Columns.Add(new DataColumn("vchCodigo", typeof(string)));
            lRetTable.Columns.Add(new DataColumn("vchDescripcion", typeof(string)));
            lRetTable.Columns.Add(new DataColumn(psLang, typeof(string)));

            //Agrego las entidades
            pDSOauto.IsDropDown = true;
            lKDBTable = pKDB.GetHisRegByEnt("", "Entidades", new string[] { "iCodCatalogo", "vchDescripcion", psLang }, "", "vchDescripcion," + psLang, 100, "iCodCatalogo = " + pDSOauto.Value);
            pDSOauto.IsDropDown = false;
            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                lRetRow = lRetTable.NewRow();
                foreach (DataColumn lRetCol in lRetTable.Columns)
                {
                    lRetRow[lRetCol] = lKDBRow[lRetCol.ColumnName];
                }
                lRetTable.Rows.Add(lRetRow);
            }

            return lRetTable;
        }
    }

    public class KeytiaAttibuteField : KeytiaAutoCompleteField
    {
        public KeytiaAttibuteField() { }

        public override void CreateField()
        {
            base.CreateField();
            pDSOauto.Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchAttribute");
        }

        protected override DataTable GetDataValueSource(string lsValue)
        {
            DataTable lKDBTable;
            DataTable lRetTable = new DataTable();
            DataRow lRetRow;

            lRetTable.Columns.Add(new DataColumn("iCodCatalogo", typeof(int)));
            lRetTable.Columns.Add(new DataColumn("vchCodigo", typeof(string)));
            lRetTable.Columns.Add(new DataColumn("vchDescripcion", typeof(string)));
            lRetTable.Columns.Add(new DataColumn(psLang, typeof(string)));

            //Agrego las entidades
            lKDBTable = pKDB.GetHisRegByEnt("", "Entidades", "iCodCatalogo = " + lsValue);
            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                lRetRow = lRetTable.NewRow();
                foreach (DataColumn lRetCol in lRetTable.Columns)
                {
                    if (lKDBTable.Columns.Contains(lRetCol.ColumnName))
                    {
                        lRetRow[lRetCol] = lKDBRow[lRetCol.ColumnName];
                    }
                }
                lRetTable.Rows.Add(lRetRow);
            }

            //Agrego los atributos
            lKDBTable = pKDB.GetHisRegByEnt("Atrib", "", "iCodCatalogo = " + lsValue);
            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                lRetRow = lRetTable.NewRow();
                foreach (DataColumn lRetCol in lRetTable.Columns)
                {
                    if (lKDBTable.Columns.Contains(lRetCol.ColumnName))
                    {
                        lRetRow[lRetCol] = lKDBRow[lRetCol.ColumnName];
                    }
                }
                lRetTable.Rows.Add(lRetRow);
            }

            return lRetTable;
        }

        protected override DataTable GetFillAjaxSource()
        {
            DataTable lKDBTable;
            DataTable lRetTable = new DataTable();
            DataRow lRetRow;

            lRetTable.Columns.Add(new DataColumn("iCodCatalogo", typeof(int)));
            lRetTable.Columns.Add(new DataColumn("vchCodigo", typeof(string)));
            lRetTable.Columns.Add(new DataColumn("vchDescripcion", typeof(string)));
            lRetTable.Columns.Add(new DataColumn(psLang, typeof(string)));

            //Agrego las entidades
            pDSOauto.IsDropDown = true;
            lKDBTable = pKDB.GetHisRegByEnt("", "Entidades", new string[] { "iCodCatalogo", "vchDescripcion", psLang }, "", "vchDescripcion," + psLang, 100, "iCodCatalogo = " + pDSOauto.Value);
            pDSOauto.IsDropDown = false;
            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                lRetRow = lRetTable.NewRow();
                foreach (DataColumn lRetCol in lRetTable.Columns)
                {
                    lRetRow[lRetCol] = lKDBRow[lRetCol.ColumnName];
                }
                lRetTable.Rows.Add(lRetRow);
            }

            //Agrego los atributos
            pDSOauto.IsDropDown = true;
            lKDBTable = pKDB.GetHisRegByEnt("Atrib", "", new string[] { "iCodCatalogo", "vchDescripcion", psLang }, "", "vchDescripcion," + psLang, 100, "iCodCatalogo = " + pDSOauto.Value);
            pDSOauto.IsDropDown = false;
            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                lRetRow = lRetTable.NewRow();
                foreach (DataColumn lRetCol in lRetTable.Columns)
                {
                    lRetRow[lRetCol] = lKDBRow[lRetCol.ColumnName];
                }
                lRetTable.Rows.Add(lRetRow);
            }

            return lRetTable;
        }

        protected override void CargaScriptValidaVigencias()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + ".fnVigArray.push(function(){AutoComplete.validaVigenciasAtrib.call(" + pjsObj + ", $('#" + pDSOauto.Search.ClientID + "', false))})");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricFieldCollection), pDSOauto.Search.ClientID + ".validaVigenciasAtrib", lsb.ToString(), true, false);
        }
    }

    public class KeytiaAttibuteValueField : KeytiaAutoCompleteField
    {
        public KeytiaAttibuteValueField() { }

        public override object DataValue
        {
            get
            {
                return base.DataValue;
            }
            set
            {
                string lConfigName = pConfigName;
                if (value != DBNull.Value)
                {
                    KeytiaAttibuteField lField = (KeytiaAttibuteField)this.Collection.GetByConfigName("Atrib");
                    DataTable ldt = DSODataAccess.Execute("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + lField.DataValue);
                    if (ldt.Rows.Count > 0)
                    {
                        pConfigName = ldt.Rows[0]["vchCodigo"].ToString();
                    }
                    else
                    {
                        value = DBNull.Value;
                    }
                }

                base.DataValue = value;
                pConfigName = lConfigName;
            }
        }

        public override void CreateField()
        {
            base.CreateField();
            pDSOauto.AddClientEvent("entidadField", "Atrib");
        }

        public override void FillAjax()
        {
            string lConfigName = pConfigName;
            KeytiaAttibuteField lField = (KeytiaAttibuteField)this.Collection.GetByConfigName("Atrib");
            DataTable ldt = DSODataAccess.Execute("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + lField.DataValue);
            if (ldt.Rows.Count > 0)
            {
                pConfigName = ldt.Rows[0]["vchCodigo"].ToString();
            }
            else
            {
                DataValue = DBNull.Value;
            }

            base.FillAjax();
            pConfigName = lConfigName;
        }
    }

    public class RelationAutoCompleteField : KeytiaAutoCompleteField
    {
        protected override DataTable GetDataValueSource(string lsValue)
        {
            DataTable lRetTable = new DataTable();
            lRetTable.Columns.Add(new DataColumn("iCodCatalogo", typeof(int)));
            lRetTable.Columns.Add(new DataColumn("vchCodigo", typeof(string)));
            lRetTable.Columns.Add(new DataColumn("vchDescripcion", typeof(string)));
            lRetTable.Columns.Add(new DataColumn(psLang, typeof(string)));
            DataRow lRetRow = lRetTable.NewRow();

            DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
            int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

            DataTable ldtHis = DSODataAccess.Execute("select top 1 H.*,C.vchCodigo from Historicos H, Catalogos C where H.iCodCatalogo = C.iCodRegistro and H.iCodCatalogo = " + lsValue + " and H.dtIniVigencia <> H.dtFinVigencia order by H.dtFecUltAct desc");
            if (ldtHis.Rows.Count > 0)
            {
                int liCodMaestro = (int)ldtHis.Rows[0]["iCodMaestro"];

                DataTable ldtCampoIdioma = DSODataAccess.Execute("select CampoIdioma = [" + DSODataContext.Schema + "].GetCampoMaestro(" + liCodMaestro + "," + liCodIdioma + ")");

                lRetRow["iCodCatalogo"] = ldtHis.Rows[0]["iCodCatalogo"];
                lRetRow["vchCodigo"] = ldtHis.Rows[0]["vchCodigo"];
                lRetRow["vchDescripcion"] = ldtHis.Rows[0]["vchDescripcion"];
                if (ldtCampoIdioma.Rows[0]["CampoIdioma"] != DBNull.Value)
                {
                    lRetRow[psLang] = ldtHis.Rows[0][ldtCampoIdioma.Rows[0]["CampoIdioma"].ToString()];
                }

                lRetTable.Rows.Add(lRetRow);
            }

            return lRetTable;
        }

        protected override DataTable GetFillAjaxSource()
        {
            DataTable lRetTable = new DataTable();
            lRetTable.Columns.Add(new DataColumn("iCodCatalogo", typeof(int)));
            lRetTable.Columns.Add(new DataColumn("vchCodigo", typeof(string)));
            lRetTable.Columns.Add(new DataColumn("vchDescripcion", typeof(string)));
            lRetTable.Columns.Add(new DataColumn(psLang, typeof(string)));
            DataRow lRetRow = lRetTable.NewRow();

            DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
            int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

            pDSOauto.IsDropDown = true;
            DataTable ldtHis = DSODataAccess.Execute("select top 1 H.*,C.vchCodigo from Historicos H, Catalogos C where H.iCodCatalogo = C.iCodRegistro and H.iCodCatalogo = " + pDSOauto.Value + " and H.dtIniVigencia <> H.dtFinVigencia order by H.dtFecUltAct desc");
            pDSOauto.IsDropDown = false;
            if (ldtHis.Rows.Count > 0)
            {
                int liCodMaestro = (int)ldtHis.Rows[0]["iCodMaestro"];

                DataTable ldtCampoIdioma = DSODataAccess.Execute("select CampoIdioma = " + DSODataContext.Schema + ".GetCampoMaestro(" + liCodMaestro + "," + liCodIdioma + ")");

                lRetRow["iCodCatalogo"] = ldtHis.Rows[0]["iCodCatalogo"];
                lRetRow["vchCodigo"] = ldtHis.Rows[0]["vchCodigo"];
                lRetRow["vchDescripcion"] = ldtHis.Rows[0]["vchDescripcion"];
                if (ldtCampoIdioma.Rows[0]["CampoIdioma"] != DBNull.Value)
                {
                    lRetRow[psLang] = ldtHis.Rows[0][ldtCampoIdioma.Rows[0]["CampoIdioma"].ToString()];
                }

                lRetTable.Rows.Add(lRetRow);
            }

            return lRetTable;
        }
    }

    public class KeytiaConsultaDashboardField : KeytiaAutoCompleteField
    {
        public KeytiaConsultaDashboardField() { }

        public override void CreateField()
        {
            base.CreateField();
            int liCodEntidad = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Aplic'");
            pDSOauto.AddClientEvent("iCodEntidad", liCodEntidad.ToString());
            pDSOauto.Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchConsultaDashboard");
        }

        public override object DataValue
        {
            get
            {
                return base.DataValue;
            }
            set
            {
                string lConfigName = pConfigName;
                pConfigName = "Aplic";
                base.DataValue = value;
                pConfigName = lConfigName;
            }
        }

        public override void FillAjax()
        {
            string lConfigName = pConfigName;
            pConfigName = "Aplic";
            base.FillAjax();
            pConfigName = lConfigName;
        }
    }
}
