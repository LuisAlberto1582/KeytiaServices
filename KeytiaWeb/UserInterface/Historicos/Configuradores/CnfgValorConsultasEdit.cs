using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Data;
using DSOControls2008;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public class CnfgValorConsultasEdit : HistoricEdit
    {
        protected object poAtrib = null;

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            pFields.GetByConfigName("Atrib").DisableField();
        }

        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            pFields.GetByConfigName("Atrib").DisableField();
        }

        public static string SearchConsulta(string term, int iCodAtributo)
        {
            KDBAccess lKDB = new KDBAccess();

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = "{" + Globals.GetCurrentLanguage() + "}";
                //string lsEntidad;
                DataTable lKDBTable;
                DataTable lDataTable = new DataTable();

                //lsEntidad = (string)DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodRegistro = " + iCodAtributo);

                lDataTable.Columns.Add(new DataColumn("id", typeof(int)));
                lDataTable.Columns.Add(new DataColumn("value", typeof(string)));

                //Agrego las consultas
                lKDBTable = lKDB.GetHisRegByEnt("Consul", "", new string[] { "iCodCatalogo", "vchDescripcion", lsLang }, "{Atrib} = " + iCodAtributo, "vchDescripcion," + lsLang, 100, "(vchDescripcion + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%' or " + lsLang + " + ' (' + vchCodigo + ')' like '%" + term.Replace("'", "''") + "%')");
                foreach (DataRow lKDBRow in lKDBTable.Rows)
                {
                    DataRow lDataRow = lDataTable.NewRow();
                    lDataRow["id"] = lKDBRow["iCodCatalogo"];
                    if (lKDBTable.Columns.Contains(lsLang) && lKDBRow[lsLang] != DBNull.Value)
                    {
                        lDataRow["value"] = lKDBRow[lsLang] + " (" + lKDBRow["vchCodigo"] + ")";
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
                string json = DSOControls2008.DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        public static DSOGridServerResponse GetValorConsultasData(DSOGridServerRequest gsRequest, int iCodAtributo, int iCodEntidad, int iCodMaestro)
        {
            object ldtVigencia = null;
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodEntidad, iCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();

                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();
                string lsMaestro = DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + iCodMaestro).ToString();

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if ((!lFields.ContainsConfigName(lsOrderCol)
                        || (lsOrderCol.EndsWith("Desc")
                            && !lFields.ContainsConfigName(lsOrderCol.Substring(0, lsOrderCol.Length - 4))))
                        && lsOrderCol != "vchCodigo"
                        && lsOrderCol != "vchDescripcion"
                        && lsOrderCol != "dtIniVigencia"
                        && lsOrderCol != "dtFinVigencia"
                        && lsOrderCol != "Consultar")
                    {
                        lsOrderCol = "dtFecUltAct";
                    }

                    switch (gsRequest.sSortDir[0].ToLower())
                    {
                        case "desc":
                            lsOrderDir = " desc";
                            lsOrderDirInv = " asc";
                            break;
                        default:
                            lsOrderDir = " asc";
                            lsOrderDirInv = " desc";
                            break;
                    }
                }
                else
                {
                    lsOrderCol = "dtIniVigencia";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lsbColumnas.AppendLine("iCodRegistro");
                lsbColumnas.AppendLine(",Consultar = null");
                lsbColumnas.AppendLine(",vchCodigo");
                lsbColumnas.AppendLine(",vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbColumnas.AppendLine(",[" + lField.ConfigName + "Desc]");
                    }
                    else
                    {
                        lsbColumnas.AppendLine(",[" + lField.ConfigName + "]");
                    }
                }

                lsbColumnas.AppendLine(",dtIniVigencia");
                lsbColumnas.AppendLine(",dtFinVigencia");
                lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from " + DSODataContext.Schema + ".[VisHistoricos('" + lsEntidad + "','" + lsMaestro + "','" + Globals.GetCurrentLanguage() + "')] a");
                lsbFrom.AppendLine("      where dtIniVigencia <> dtFinVigencia");
                lsbFrom.AppendLine("      and Atrib = " + iCodAtributo);

                if (ldtVigencia != null)
                {
                    lsbFrom.AppendLine("      and '" + ((DateTime)ldtVigencia).ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
                    lsbFrom.AppendLine("      and '" + ((DateTime)ldtVigencia).ToString("yyyy-MM-dd") + "' < dtFinVigencia");
                }

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(a.vchCodigo,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(a.vchDescripcion,'')");

                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlTimeFormat + "),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlTimeFormat + "),'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (lField.Column.StartsWith("Date"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]," + lsSqlTimeFormat + "),'')");
                        }
                        else if (lField.Column.StartsWith("iCodCatalogo"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "Desc]),'')");
                        }
                        else if (!lField.Column.StartsWith("VarChar"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]),'')");
                        }
                        else
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(a.[" + lField.ConfigName + "],'')");
                        }
                    }
                    lsbWhere.AppendLine("and " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                }

                string[] lsColumns = gsRequest.sColumns.Split(',');
                int lidx;
                for (lidx = 0; lidx < lsColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = lsColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.ContainsConfigName(lsColumn)
                            || (lsColumn.EndsWith("Desc") && lFields.ContainsConfigName(lsColumn.Substring(0, lsColumn.Length - 4)))
                            || lsColumn == "vchCodigo"
                            || lsColumn == "vchDescripcion"
                            || lsColumn == "dtIniVigencia"
                            || lsColumn == "dtFinVigencia"))
                        {
                            lsbWhere.Append("and ");

                            if ((lFields.ContainsConfigName(lsColumn)
                                    && lFields.GetByConfigName(lsColumn) is KeytiaDateTimeField)
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a.[" + lsColumn + "], " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a.[" + lsColumn + "], " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a.[" + lsColumn + "] like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom);
                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFrom + lsWhere + lsOrderBy);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt, true);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        protected override void InitGrid()
        {
            base.InitGrid();
            if (pFields != null)
            {
                KeytiaBaseField lFieldAtributo = pFields.GetByConfigName("Atrib");
                KeytiaBaseField lFieldAtributoRel = ((KeytiaRelationField)this.Historico.Fields.GetByConfigName("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte")).Fields.GetByConfigName("Atrib");
                lFieldAtributo.DataValue = lFieldAtributoRel.DataValue;
                if (lFieldAtributo.DSOControlDB.HasValue)
                    poAtrib = lFieldAtributo.DataValue;
                else
                    poAtrib = -1;
                pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerDataValorConsulta(this, sSource, aoData, fnCallback, " + poAtrib.ToString() + ");}";
                pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetValorConsultasData");
            }
        }
    }
}
