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
    public class KeytiaMultiSelectField : KeytiaBaseField, IKeytiaFillableField
    {
        protected Panel pWrapper;
        protected DSOGrid pMultiSelectGrid;
        protected DSOCheckBox pChkTodos;
        protected object pDataValue = null;

        public KeytiaMultiSelectField() { }

        public override object DataValue
        {
            get
            {
                if (pDataValue != null)
                {
                    return pDataValue;
                }

                if (!pMultiSelectGrid.HasValue)
                {
                    pDataValue = "null";
                }
                else
                {
                    List<string> lstValues = new List<string>();
                    foreach (DataRow ldataRow in pMultiSelectGrid.EditedData.Rows)
                    {
                        lstValues.Add(ldataRow["iCodCatalogo"].ToString());
                    }

                    string lsCatalogos = "(" + String.Join(",", lstValues.ToArray()) + ")";

                    StringBuilder lsbQuery = new StringBuilder();
                    lsbQuery.AppendLine("select distinct(H.iCodCatalogo)");
                    lsbQuery.AppendLine("from Historicos H, Maestros M, Catalogos C");
                    lsbQuery.AppendLine("where M.dtIniVigencia <> M.dtFinVigencia");
                    lsbQuery.AppendLine("and H.iCodMaestro = M.iCodRegistro");
                    lsbQuery.AppendLine("and H.iCodCatalogo = C.iCodRegistro");
                    lsbQuery.AppendLine("and C.iCodCatalogo = " + pConfigValue);
                    lsbQuery.AppendLine("and H.iCodCatalogo in " + lsCatalogos);
                    if (this is KeytiaMultiSelectRestrictedField)
                    {
                        string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + pConfigValue).ToString();
                        lsbQuery.AppendLine("and H.iCodCatalogo in (select distinct(iCodCatalogo) from ");
                        //20170614 NZ Se cambia funcion
                        //lsbQuery.AppendLine("[" + DSODataContext.Schema + "].GetRestriccionVigencia(" + Session["iCodUsuario"] + ", " + Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ((DateTime)pFinVigencia).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                        lsbQuery.AppendLine("[" + DSODataContext.Schema + "].GetRestricPorEntidad(" + Session["iCodUsuario"] + ", " + Session["iCodPerfil"] + ", '" + lsEntidad.Replace("'", "''") + "', '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ((DateTime)pFinVigencia).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                        lsbQuery.AppendLine(")");
                    }
                    lsbQuery.Append(DSOControl.ComplementaVigenciasJS(pIniVigencia, pFinVigencia, false, "H"));

                    DataTable ldtResults = DSODataAccess.Execute(lsbQuery.ToString());
                    if (ldtResults != null && ldtResults.Rows.Count > 0)
                    {
                        if (ldtResults.Rows.Count == lstValues.Count)
                        {
                            pDataValue = pMultiSelectGrid.DataValueDelimiter + String.Join(",", lstValues.ToArray()) + pMultiSelectGrid.DataValueDelimiter;
                        }
                        else
                        {
                            DataRow ldataRow;
                            pMultiSelectGrid.ClearEditedData();
                            lstValues = new List<string>();
                            foreach (DataRow ldrRow in ldtResults.Rows)
                            {
                                ldataRow = pMultiSelectGrid.EditedData.NewRow();
                                ldataRow["iCodCatalogo"] = ldrRow["iCodCatalogo"];
                                lstValues.Add(ldataRow["iCodCatalogo"].ToString());
                                pMultiSelectGrid.EditedData.Rows.Add(ldataRow);
                            }
                            pDataValue = pMultiSelectGrid.DataValueDelimiter + String.Join(",", lstValues.ToArray()) + pMultiSelectGrid.DataValueDelimiter;
                        }
                    }
                    else
                    {
                        pMultiSelectGrid.ClearEditedData();
                        pDataValue = "null";
                    }
                }
                return pDataValue;
            }
            set
            {
                pChkTodos.DataValue = false;
                if (value == null || value == DBNull.Value || value.ToString() == "null")
                {
                    pDataValue = "null";
                    pMultiSelectGrid.DataValue = value;
                }
                else if (pDataValue == null || value.ToString() != pDataValue.ToString())
                {
                    pDataValue = null;
                    string[] lstValues = value.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    DataRow ldataRow;
                    int liValor;
                    pMultiSelectGrid.ClearEditedData();
                    foreach (string lsValue in lstValues)
                    {
                        if (int.TryParse(lsValue.ToString(), out liValor))
                        {
                            ldataRow = pMultiSelectGrid.EditedData.NewRow();
                            ldataRow["iCodCatalogo"] = liValor;
                            pMultiSelectGrid.EditedData.Rows.Add(ldataRow);
                        }
                    }
                    pMultiSelectGrid.SaveEditedData();
                }
            }
        }

        public override bool ShowInGrid
        {
            get
            {
                return false;
            }
        }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pMultiSelectGrid;
            }
        }

        public override void EnableField()
        {
            pChkTodos.Visible = true;
            pMultiSelectGrid.AddClientEvent("enableField", "1");
        }

        public override void DisableField()
        {
            pChkTodos.Visible = false;
            pMultiSelectGrid.AddClientEvent("enableField", "0");
        }

        public override void CreateField()
        {
            pWrapper = new Panel();
            pMultiSelectGrid = new DSOGrid();
            pChkTodos = new DSOCheckBox();
            pjsObj = pContainer.ID + "." + pColumn + "MultiSelect";
            pMultiSelectGrid.DataValueDelimiter = "'";
            EnableField();

            InitDSOControlDB();
            InitWrapper();
            InitGrid();
        }

        protected virtual void InitWrapper()
        {
            pWrapper.ID = "wrapper";
            pWrapper.CssClass = "MultiSelectWrapper";
            pWrapper.Controls.Add(pChkTodos);
            pMultiSelectGrid.Wrapper = pWrapper;

            pChkTodos.ID = "todos";
        }

        protected virtual void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget;

            pMultiSelectGrid.Config.sDom = "<lf>tr<pi>"; //con filtro global
            pMultiSelectGrid.Config.bAutoWidth = true;
            pMultiSelectGrid.Config.sScrollX = "100%";
            pMultiSelectGrid.Config.sPaginationType = "full_numbers";
            pMultiSelectGrid.Config.bJQueryUI = true;
            pMultiSelectGrid.Config.bProcessing = true;
            pMultiSelectGrid.Config.bServerSide = true;
            pMultiSelectGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetMultiSelectData");
            pMultiSelectGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerData(this, sSource, aoData, fnCallback);}";
            pMultiSelectGrid.Config.fnInitComplete = "function(){" + pjsObj + ".fnInitComplete(this);}";

            lTarget = 0;
            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodCatalogo";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pMultiSelectGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "bSeleccionado";
            lCol.sClass = "TdSelect";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = true;
            lCol.sWidth = "50px";
            pMultiSelectGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchDescripcion";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = true;
            pMultiSelectGrid.Config.aoColumnDefs.Add(lCol);

            pMultiSelectGrid.Config.aaSorting.Add(new ArrayList());
            pMultiSelectGrid.Config.aaSorting[0].Add(1);
            pMultiSelectGrid.Config.aaSorting[0].Add("asc");

            pMultiSelectGrid.MetaData = DSODataAccess.Execute("select iCodCatalogo from Historicos where 1=2");
        }

        public override void InitField()
        {
            base.InitField();
            pChkTodos.CreateControls();
            pChkTodos.AddClientEvent("onclick", pjsObj + ".chkTodosOnclick(this.checked);");

            pMultiSelectGrid.AddClientEvent("iCodEntidad", pConfigValue.ToString());

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){" + pjsObj + " = new MultiSelect(" + pContainer.ID + ",\"#" + pMultiSelectGrid.Grid.ClientID + "\",\"#" + pChkTodos.CheckBox.ClientID + "\");});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(KeytiaMultiSelectField), pjsObj + "New", lsb.ToString(), true, false);
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            pMultiSelectGrid.Config.oLanguage = Globals.GetGridLanguage();
            pChkTodos.CheckBox.Text = Globals.GetMsgWeb(false, "bSelTodos");

            if (pDescripcion == pConfigName)
            {
                pDescripcion = Globals.GetLangItem("", "Entidades", pConfigName);
            }
            foreach (DSOGridClientColumn lCol in pMultiSelectGrid.Config.aoColumnDefs)
            {
                if (lCol.sName == "bSeleccionado")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "bSeleccionado"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRenderCheck(obj,\"" + pjsObj + ".chkOnClick({0}, this.checked);\"); }";
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcion"));
                }
            }
        }

        public virtual void Fill()
        {
            pMultiSelectGrid.Fill();
        }

        public override string ToString()
        {
            if (pMultiSelectGrid != null)
            {
                if (pMultiSelectGrid.HasValue)
                {
                    string lsLang = Globals.GetCurrentLanguage();
                    string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + pConfigValue).ToString();
                    List<string> lstValores = new List<string>();

                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('" + lsEntidad + "','" + lsLang + "')]");
                    psbQuery.AppendLine("where iCodCatalogo in(" + DataValue + ")");
                    psbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(pIniVigencia, pFinVigencia, false));
                    DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());

                    foreach (DataRow lRow in ldt.Rows)
                    {
                        if (ldt.Columns.Contains(lsLang) && lRow[lsLang] != DBNull.Value)
                        {
                            lstValores.Add(lRow[lsLang].ToString());
                        }
                        else
                        {
                            lstValores.Add(lRow["vchDescripcion"].ToString());
                        }
                    }
                    lstValores.Sort();
                    return "\"" + string.Join("\",\n \"", lstValores.ToArray()) + "\"";
                }
                else
                {
                    return "";
                }
            }
            return base.ToString();
        }
    }

    public class KeytiaMultiSelectRestrictedField : KeytiaMultiSelectField
    {
        public KeytiaMultiSelectRestrictedField() { }

        protected override void InitGrid()
        {
            base.InitGrid();
            pMultiSelectGrid.Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetMultiSelectRestData");
        }
    }
}