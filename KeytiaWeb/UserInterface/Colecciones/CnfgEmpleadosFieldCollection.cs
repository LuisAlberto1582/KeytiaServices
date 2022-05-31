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
using System.Reflection;

namespace KeytiaWeb.UserInterface
{
    public class CnfgEmpleadosFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadPrepEmple;

        public CnfgEmpleadosFieldCollection()
        {
        }
        public CnfgEmpleadosFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgEmpleadosFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void FillMetaData()
        {
            base.FillMetaData();

            int liCodAtributos = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'");
            if (pMetaData.Select("ConfigValue = " + liCodAtributos).Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigValue = " + liCodAtributos)[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAttibuteField";

                int liCodValores = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Valores'");
                if (pMetaData.Select("ConfigValue = " + liCodValores).Length > 0)
                {
                    lRowAtrib = pMetaData.Select("ConfigValue = " + liCodValores)[0];
                    lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAttibuteValueField";
                }
            }
        }


        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);
            if (lField.SubCollectionClass == "KeytiaWeb.UserInterface.HistoricFieldCollection")
            {
                lField.SubCollectionClass = "KeytiaWeb.UserInterface.CnfgEmpleadosFieldCollection";
            }

            if (lField is KeytiaRelationField)
            {
                if ((lField.ConfigName == "Empleado - CodAutorizacion") ||
                    (lField.ConfigName == "Empleado - Extension") ||
                    (lField.ConfigName == "Empleado - Linea"))
                {
                    ((KeytiaRelationField)lField).RelationCollectionClass = "KeytiaWeb.UserInterface.EmpleRelationFieldCollection";
                }
            }
            else if (lField is CnfgSubPresupuestoField)
            {
                lField.SubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";
            }

            return lField;
        }

        protected override void CreateFields(bool lbOnlyGridFields)
        {
            base.CreateFields(lbOnlyGridFields);

            foreach (KeytiaBaseField lField in this)
            {
                if (lField is KeytiaRelationField && EsRelacionRecurso((KeytiaRelationField)lField))
                {
                    ((DSOGrid)((KeytiaRelationField)lField).DSOControlDB).Config.sAjaxSource = pContainer.ResolveUrl("~/WebMethods.aspx/GetEmpleRelData");
                }
            }
        }

        public static DataTable GetEntidadesRecursos()
        {
            DataTable lDataTable;
            StringBuilder lsb = new StringBuilder();
            int liCodEntidadRecurso = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Recurs'");

            lsb.AppendLine("select M.iCodEntidad from Maestros M");
            lsb.AppendLine("where M.dtIniVigencia <> M.dtFinVigencia and M.iCodRegistro in(select iCodMaestro from [" + DSODataContext.Schema + "].GetMaeByAtrib(" + liCodEntidadRecurso + "))");
            lsb.AppendLine("and M.iCodEntidad not in(select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo in('Detall','CartaCust'))");
            lDataTable = DSODataAccess.Execute(lsb.ToString());

            return lDataTable;
        }

        public static bool EsRelacionRecurso(KeytiaRelationField lRelField)
        {
            bool lbRet = false;
            foreach (DataRow lDataRow in GetEntidadesRecursos().Rows)
            {
                if (lRelField.Fields.ContainsConfigValue((int)lDataRow["iCodEntidad"]))
                {
                    lbRet = true;
                    break;
                }
            }
            return lbRet;
        }

        public static DSOGridServerResponse GetEmpleRelData(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodRelacion, int iCodCatalogo, string jsonData, string vchDescripcion)
        {
            KDBAccess pKDB = new KDBAccess();

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                int liCodUsuario = (int)HttpContext.Current.Session["iCodUsuario"];
                int liCodPerfil = (int)HttpContext.Current.Session["iCodPerfil"];

                RelationFieldCollection lFields = new RelationFieldCollection(iCodEntidad, iCodRelacion);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();

                DateTime ldtVigencia = DateTime.Today;

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                string lsColEntidad = "";
                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if (!lFields.Contains(lsOrderCol.Replace("Display", "")))
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
                    lsOrderCol = "dtFecUltAct";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("      select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lgsrRet.sColumns = "iCodRegistro";
                lsbColumnas.AppendLine("a.iCodRegistro");
                //Columnas para guardar los valores
                foreach (KeytiaBaseField lField in lFields)
                {
                    lgsrRet.sColumns += "," + lField.Column;
                    lsbColumnas.AppendLine(",a." + lField.Column);
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && lField.ConfigValue == iCodEntidad)
                    {
                        lsColEntidad = lField.Column;
                    }
                }
                lgsrRet.sColumns += ",dtFecUltAct";
                lsbColumnas.AppendLine(",a.dtFecUltAct");

                //Columnas para mostrar los valores
                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lgsrRet.sColumns += "," + lField.Column + "Display";
                        lsbColumnas.AppendLine("," + lField.Column + "Display = " + DSODataContext.Schema + ".GetCatDesc(a." + lField.Column + "," + liCodIdioma + ",a.dtIniVigencia)");
                    }
                    else if (lField.Column.EndsWith("Vigencia"))
                    {
                        lgsrRet.sColumns += "," + lField.Column + "Display";
                        lsbColumnas.AppendLine("," + lField.Column + "Display = a." + lField.Column);
                    }
                }
                lgsrRet.sColumns += ",Editar";
                lsbColumnas.AppendLine(",Editar = case when Vis.iCodCatalogo is null then 0 else 1 end");

                lgsrRet.sColumns += ",Eliminar";
                lsbColumnas.AppendLine(",Eliminar = case when Vis.iCodCatalogo is null then 0 else 1 end");

                //////////////////////////////////////////////////////////////////////////7

                string lsColumnaRecurso = null;
                string lsEntidadRecurso = null;
                KeytiaBaseField lRecursoField;
                foreach (DataRow lDataRow in GetEntidadesRecursos().Rows)
                {
                    if (lFields.ContainsConfigValue((int)lDataRow["iCodEntidad"]))
                    {
                        lRecursoField = lFields.GetByConfigValue((int)lDataRow["iCodEntidad"]);
                        lsColumnaRecurso = lRecursoField.Column;
                        lsEntidadRecurso = lRecursoField.ConfigName;
                        break;
                    }
                }

                lsbFrom.AppendLine("      from Relaciones a");
                lsbFrom.AppendLine("      Left Outer Join " + DSODataContext.Schema + ".[VisHistoricos('" + lsEntidadRecurso + "','" + Globals.GetCurrentLanguage() + "')] Vis");
                lsbFrom.AppendLine("      on a." + lsColumnaRecurso + " = Vis.iCodCatalogo");
                //20170614 NZ Se cambia funcion
                //lsbFrom.AppendLine("      and Vis.Sitio in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestriccionVigencia(" + liCodUsuario + "," + liCodPerfil + ",'Sitio','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");
                lsbFrom.AppendLine("      and Vis.Sitio in(select iCodCatalogo from " + DSODataContext.Schema + ".GetRestricPorEntidad(" + liCodUsuario + "," + liCodPerfil + ",'Sitio','" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ldtVigencia.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "',default))");
                lsbFrom.AppendLine("      and Vis.iCodRegistro = (select MAX(iCodRegistro) from " + DSODataContext.Schema + ".[VisHistoricos('" + lsEntidadRecurso + "','" + Globals.GetCurrentLanguage() + "')] VisH");
                lsbFrom.AppendLine("            where VisH.iCodCatalogo = Vis.iCodCatalogo");
                lsbFrom.AppendLine("            and VisH.dtIniVigencia <> VisH.dtFinVigencia)");

                lsbWhere.AppendLine("      where iCodRelacion = " + iCodRelacion);
                lsbWhere.AppendLine("      and a.dtIniVigencia <> a.dtFinVigencia");
                lsbWhere.AppendLine("      and " + lsColEntidad + " = " + iCodCatalogo);

                lsbOrderBy.AppendLine("       order by a." + lsOrderCol + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by a." + lsOrderCol + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by a." + lsOrderCol + lsOrderDir);

                string lsSelectCount = "select count(a.iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + lsWhere);
                lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFrom + lsWhere + lsOrderBy);

                //elimino las filas repetidas de la ultima pagina
                int idxDelRows = 0;
                if (gsRequest.iDisplayStart != 0 && gsRequest.iDisplayStart + gsRequest.iDisplayLength > lgsrRet.iTotalDisplayRecords)
                {
                    idxDelRows = gsRequest.iDisplayStart + gsRequest.iDisplayLength - lgsrRet.iTotalDisplayRecords;
                }
                while (idxDelRows-- > 0)
                {
                    ldt.Rows.Remove(ldt.Rows[0]);
                }

                DataTable lData = new DataTable();
                if (!String.IsNullOrEmpty(jsonData))
                {
                    lData = DSOControl.DeserializeJSON<DataTable>(jsonData);
                }
                foreach (DataRow lDataRow in lData.Rows)
                {
                    DataRow ldr;
                    if (ldt.Select("iCodRegistro = " + lDataRow["iCodRegistro"]).Length == 0)
                    {
                        ldr = ldt.NewRow();
                        ldt.Rows.Add(ldr);
                    }
                    else
                    {
                        ldr = ldt.Select("iCodRegistro = " + lDataRow["iCodRegistro"])[0];
                    }
                    foreach (DataColumn lDataCol in lData.Columns)
                    {
                        if (lDataCol.ColumnName != "Editar"
                            && lDataCol.ColumnName != "Eliminar"
                            && ldt.Columns.Contains(lDataCol.ColumnName))
                        {
                            ldr[lDataCol.ColumnName] = lDataRow[lDataCol];
                        }
                    }
                    ldr["Editar"] = lDataRow["Editar"];
                    ldr["Eliminar"] = lDataRow["Eliminar"];
                }
                KeytiaBaseField lFieldEntidad = lFields.GetByConfigValue(iCodEntidad);
                foreach (DataRow ldr in ldt.Rows)
                {
                    ldr[lFieldEntidad.Column + "Display"] = vchDescripcion;
                }

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                lgsrRet.SetDataFromDataTable(ldt, lsDateFormat, "dtIniVigenciaDisplay", "dtFinVigenciaDisplay");
                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadPrepEmple = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'PrepEmple'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadPrepEmple + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubPresupuestoField", "PrepEmple", "Presupuesto Fijo", "KeytiaWeb.UserInterface.CnfgPrepEmpleFijo", "KeytiaWeb.UserInterface.HistoricFieldCollection");
            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubPresupuestoField", "PrepEmple", "Presupuesto Temporal", "KeytiaWeb.UserInterface.CnfgPrepEmpleTemporal", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }
    }
}
