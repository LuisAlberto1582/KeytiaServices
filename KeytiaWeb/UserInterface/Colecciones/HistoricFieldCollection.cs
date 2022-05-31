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
using System.Web.SessionState;
namespace KeytiaWeb.UserInterface
{
    public class HistoricFieldCollection : KeytiaFieldCollection
    {
        protected DataTable pdtTypes;
        protected DataTable pdtAtrib;
        protected DataTable pdtControles;
        protected KDBAccess pKDB = new KDBAccess();
        protected DataTable pdtMaestros;

        public HistoricFieldCollection()
        {
        }

        public HistoricFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public HistoricFieldCollection(int liCodEntidad, int liCodMaestro)
            : this(liCodEntidad, liCodMaestro, true) { }

        public HistoricFieldCollection(int liCodEntidad, int liCodMaestro, bool lbOnlyGridFields)
        {
            this.piCodEntidad = liCodEntidad;
            this.piCodConfig = liCodMaestro;

            InitMetaData();
            FillMetaData();
            CreateFields(lbOnlyGridFields);
        }

        protected override void InitMetaData()
        {
            base.InitMetaData();
            pMetaData.Columns.Add(new DataColumn("KeytiaField", typeof(string)));
            pMetaData.Columns.Add(new DataColumn("LangEntity", typeof(string)));
            pMetaData.Columns.Add(new DataColumn("LangValue", typeof(int)));
            pMetaData.Columns.Add(new DataColumn("SubHistoricClass", typeof(string)));
            pMetaData.Columns.Add(new DataColumn("SubCollectionClass", typeof(string)));

            pdtTypes = GetTypes();
            pdtAtrib = GetAtrib();
            pdtControles = GetControles();
        }

        protected override void FillMetaData()
        {
            DataTable ldtConfig = DSODataAccess.Execute("select * from " + DSODataContext.Schema + ".GetCamposMaestro(" + piCodConfig + ")");
            DataTable lMetaData = pMetaData.Clone();
            DataRow lDataRow;
            DataRow lRowTypes;
            DataRow lRowAtrib;
            DataRow lRowControles;
            int liMaxRow = 0;
            int liMaxCol = 1;


            foreach (DataRow lConfigRow in ldtConfig.Rows)
            {
                lDataRow = lMetaData.NewRow();
                lDataRow["Column"] = lConfigRow["Columna"];
                lDataRow["iCodEntidad"] = piCodEntidad;
                lDataRow["ConfigValue"] = lConfigRow["ConfigValue"];
                lDataRow["ConfigName"] = lConfigRow["ConfigName"];
                lDataRow["Row"] = lConfigRow["ConfigRen"];
                lDataRow["Col"] = lConfigRow["ConfigCol"];
                if (lDataRow["Row"] != DBNull.Value)
                {
                    liMaxRow = Math.Max(liMaxRow, (int)lDataRow["Row"]);
                }
                if (lDataRow["Col"] != DBNull.Value)
                {
                    liMaxCol = Math.Max(liMaxCol, (int)lDataRow["Col"]);
                }

                if (lConfigRow["Columna"].ToString().StartsWith("iCodRelacion"))
                {
                    lDataRow["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaRelationField";
                }
                else if (lConfigRow["Columna"].ToString().StartsWith("iCodCatalogo"))
                {
                    if (lConfigRow["ConfigName"].ToString() == "Entidad")
                    {
                        lDataRow["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaEntityField";
                    }
                    else
                    {
                        lDataRow["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAutoCompleteField";
                    }
                }
                else
                {
                    lRowControles = null;
                    lRowAtrib = pdtAtrib.Select("iCodCatalogo = " + lDataRow["ConfigValue"])[0];
                    if (lRowAtrib["{Controles}"] != DBNull.Value)
                    {
                        lRowControles = pdtControles.Select("iCodCatalogo = " + lRowAtrib["{Controles}"])[0];
                    }
                    else if (lRowAtrib["{Types}"] != DBNull.Value)
                    {
                        lRowTypes = pdtTypes.Select("iCodCatalogo = " + lRowAtrib["{Types}"])[0];
                        lRowControles = pdtControles.Select("iCodCatalogo = " + lRowTypes["{Controles}"])[0];
                    }
                    if (lRowControles != null)
                    {
                        lDataRow["KeytiaField"] = lRowControles["{Clase}"];
                    }
                    if (lDataRow["KeytiaField"] != DBNull.Value
                        && (lDataRow["KeytiaField"].ToString() == "KeytiaWeb.UserInterface.KeytiaMultiSelectField"
                            || lDataRow["KeytiaField"].ToString() == "KeytiaWeb.UserInterface.KeytiaMultiSelectRestrictedField")
                        )
                    {
                        lDataRow["ConfigValue"] = lRowAtrib["{Entidad}"];
                    }
                }

                lMetaData.Rows.Add(lDataRow);
            }

            DataView lViewData = lMetaData.DefaultView;
            lViewData.Sort = "Col ASC, Row ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                if ((lViewRow["Col"] is DBNull || lViewRow["Row"] is DBNull)
                    && !lViewRow["Column"].ToString().StartsWith("iCodRelacion"))
                {
                    lViewRow["Row"] = ++liMaxRow;
                    lViewRow["Col"] = 1;
                }
            }
            foreach (DataRowView lViewRow in lViewData)
            {
                if ((lViewRow["Col"] is DBNull || lViewRow["Row"] is DBNull)
                    && lViewRow["Column"].ToString().StartsWith("iCodRelacion"))
                {
                    lViewRow["Row"] = ++liMaxRow;
                    lViewRow["Col"] = 1;
                }
            }
            int liRow;
            int liColCount;
            int liColSpan;
            DataRow[] laDataRow;
            for (liRow = 1; liRow <= liMaxRow; liRow++)
            {
                laDataRow = lViewData.Table.Select("Row = " + liRow, "Col ASC");
                liColCount = laDataRow.Length;
                if (liColCount > 0)
                {
                    if (liColCount == 1)
                    {
                        liColSpan = 3;
                    }
                    else
                    {
                        liColSpan = 1;
                    }
                    for (int li = 0; li < liColCount; li++)
                    {
                        laDataRow[li]["ColumnSpan"] = liColSpan;
                    }
                }
            }
            foreach (DataRowView lViewRow in lViewData)
            {
                pMetaData.ImportRow(lViewRow.Row);
            }

            AgregarSubHistoricos();
        }

        protected virtual void AgregarSubHistoricos()
        {
            //Sobrescribirlo en las clases hijas para llenar pdtMaestros y agregar campos de tipo CnfgSubHistoricField a pMetaData
        }

        protected virtual void AgregarSubHistoricField(string lsEntidad, string lsMaestro)
        {
            int liCodEntidad = (int)DSODataAccess.ExecuteScalar("Select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = '" + lsEntidad.Replace("'", "''") + "'");
            int liCodMaestro = (int)pdtMaestros.Select("iCodEntidad = " + liCodEntidad + " and vchDescripcion = '" + lsMaestro.Replace("'", "''") + "'")[0]["iCodRegistro"];

            AgregarSubHistoricField(liCodEntidad, liCodMaestro);
        }

        protected virtual void AgregarSubHistoricField(string lsEntidad, string lsMaestro, string lSubHistoricClass, string lSubCollectionClass)
        {
            int liCodEntidad = (int)DSODataAccess.ExecuteScalar("Select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = '" + lsEntidad.Replace("'", "''") + "'");
            int liCodMaestro = (int)pdtMaestros.Select("iCodEntidad = " + liCodEntidad + " and vchDescripcion = '" + lsMaestro.Replace("'", "''") + "'")[0]["iCodRegistro"];

            AgregarSubHistoricField(liCodEntidad, liCodMaestro, lSubHistoricClass, lSubCollectionClass);
        }

        protected virtual void AgregarSubHistoricField(string lsSubHistoricoFieldClass, string lsEntidad, string lsMaestro, string lSubHistoricClass, string lSubCollectionClass)
        {
            int liCodEntidad = (int)DSODataAccess.ExecuteScalar("Select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = '" + lsEntidad.Replace("'", "''") + "'");
            int liCodMaestro = (int)pdtMaestros.Select("iCodEntidad = " + liCodEntidad + " and vchDescripcion = '" + lsMaestro.Replace("'", "''") + "'")[0]["iCodRegistro"];

            AgregarSubHistoricField(lsSubHistoricoFieldClass, liCodEntidad, liCodMaestro, lSubHistoricClass, lSubCollectionClass);
        }

        protected virtual void AgregarSubHistoricField(int liCodEntidad, int liCodMaestro)
        {
            AgregarSubHistoricField(liCodEntidad, liCodMaestro, "KeytiaWeb.UserInterface.HistoricEdit", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }

        protected virtual void AgregarSubHistoricField(int liCodEntidad, int liCodMaestro, string lSubHistoricClass, string lSubCollectionClass)
        {
            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubHistoricField", liCodEntidad, liCodMaestro, lSubHistoricClass, lSubCollectionClass);
        }

        protected virtual void AgregarSubHistoricField(string lsSubHistoricoFieldClass, int liCodEntidad, int liCodMaestro, string lSubHistoricClass, string lSubCollectionClass)
        {
            DataRow lMetaDataRow = pMetaData.NewRow();
            string lsMaestro = pdtMaestros.Select("iCodRegistro = " + liCodMaestro)[0]["vchDescripcion"].ToString();
            DataTable lKDBTable = pKDB.GetHisRegByEnt("MsgWeb", "Mensajes Web", " vchDescripcion = '" + lsMaestro.Replace("'", "''") + "'");

            lMetaDataRow["Column"] = "Mae" + liCodMaestro;
            lMetaDataRow["iCodEntidad"] = liCodEntidad;
            lMetaDataRow["ConfigValue"] = liCodMaestro;
            lMetaDataRow["ConfigName"] = lsMaestro;
            lMetaDataRow["Row"] = pMetaData.Rows.Count + 1;
            lMetaDataRow["Col"] = 1;
            lMetaDataRow["ColumnSpan"] = 4;
            lMetaDataRow["KeytiaField"] = lsSubHistoricoFieldClass;
            if (lKDBTable.Rows.Count > 0)
            {
                lMetaDataRow["LangEntity"] = "MsgWeb";
                lMetaDataRow["LangValue"] = lKDBTable.Rows[0]["iCodCatalogo"];
            }
            lMetaDataRow["SubHistoricClass"] = lSubHistoricClass;
            lMetaDataRow["SubCollectionClass"] = lSubCollectionClass;

            pMetaData.Rows.Add(lMetaDataRow);
        }

        protected virtual void CreateFields(bool lbOnlyGridFields)
        {
            KeytiaBaseField lField = null;

            foreach (DataRow ldr in pMetaData.Rows)
            {
                lField = CreateField(ldr);

                if (lbOnlyGridFields && lField.ShowInGrid)
                {
                    //Cuando unicamente quiero la configuracion es para mostrar los valores en un grid
                    //por lo que solo agrego los campos que pasan la validacion de mostrarse en un grid
                    Add(lField);
                }
                else if (!lbOnlyGridFields)
                {
                    lField.Table = pTablaAtritubos;
                    lField.CreateField();
                    Add(lField);
                }
            }
        }

        protected virtual KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = null;
            if (lMetaDataRow["KeytiaField"] != DBNull.Value)
            {
                lField = (KeytiaBaseField)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(KeytiaBaseField)).CodeBase, lMetaDataRow["KeytiaField"].ToString()).Unwrap();
            }

            if (lField == null)
            {
                if (((string)lMetaDataRow["Column"]).StartsWith("iCodRelacion"))
                {
                    lField = new KeytiaRelationField();
                }
                else if (((string)lMetaDataRow["Column"]).StartsWith("iCodCatalogo"))
                {
                    lField = new KeytiaAutoCompleteField();
                }
                else if (((string)lMetaDataRow["Column"]).StartsWith("Integer"))
                {
                    lField = new KeytiaIntegerField();
                }
                else if (((string)lMetaDataRow["Column"]).StartsWith("Float"))
                {
                    lField = new KeytiaNumericField();
                }
                else if (((string)lMetaDataRow["Column"]).StartsWith("Date"))
                {
                    lField = new KeytiaDateTimeField();
                }
                else if (((string)lMetaDataRow["Column"]).StartsWith("VarChar"))
                {
                    lField = new KeytiaVarcharField();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            lField.Container = pContainer;
            lField.ValidarPermiso = ValidarPermiso;
            lField.Column = (string)lMetaDataRow["Column"];
            lField.iCodEntidad = (int)lMetaDataRow["iCodEntidad"];
            lField.ConfigValue = (int)lMetaDataRow["ConfigValue"];
            if (lMetaDataRow["ConfigName"] != DBNull.Value && !String.IsNullOrEmpty(lMetaDataRow["ConfigName"].ToString()))
            {
                lField.ConfigName = (string)lMetaDataRow["ConfigName"];
            }
            lField.Row = (int)lMetaDataRow["Row"];
            lField.Col = (int)lMetaDataRow["Col"];
            lField.ColumnSpan = (int)lMetaDataRow["ColumnSpan"];
            lField.Collection = this;
            if (lMetaDataRow["LangEntity"] != DBNull.Value && lMetaDataRow["LangValue"] != DBNull.Value)
            {
                lField.LangEntity = lMetaDataRow["LangEntity"].ToString();
                lField.LangValue = (int)lMetaDataRow["LangValue"];
            }
            if (lMetaDataRow["SubHistoricClass"] != DBNull.Value)
            {
                lField.SubHistoricClass = lMetaDataRow["SubHistoricClass"].ToString();
            }
            if (lMetaDataRow["SubCollectionClass"] != DBNull.Value)
            {
                lField.SubCollectionClass = lMetaDataRow["SubCollectionClass"].ToString();
            }

            return lField;
        }

        protected override void CreateFields()
        {
            CreateFields(false);
        }

        public virtual void SetValues(int liCodRegistro)
        {
            DataRow lDataRow = DSODataAccess.ExecuteDataRow("select * from Historicos where iCodRegistro = " + liCodRegistro);
            SetValues(lDataRow);
        }

        public virtual Hashtable GetValues()
        {
            Hashtable lht = new Hashtable();
            string opc = (HttpContext.Current.Session["OpcMenu"]!= null)? HttpContext.Current.Session["OpcMenu"].ToString() : "" ;
            
            foreach (KeytiaBaseField lField in this)
            {
                if (!(lField is KeytiaRelationField) && !(lField is CnfgSubHistoricField))
                {

                    if (opc == "OpcCte")
                    {
                        if (lField.DataValue != null && lField.DataValue.ToString() != "" && lField.DataValue.ToString() != "null")
                        {
                            lht.Add(lField.Column, lField.DataValue);
                        }
                    }
                    else
                    {
                        lht.Add(lField.Column, lField.DataValue);
                    }

                }
            }
            //foreach (KeytiaBaseField lField in this)
            //{
            //    if (!(lField is KeytiaRelationField) && !(lField is CnfgSubHistoricField))
            //    {
            //        lht.Add(lField.Column, lField.DataValue);
            //    }
            //}
            return lht;
        }

        public virtual DataSet GetRelationValues()
        {
            DataSet lDataSet = new DataSet();
            foreach (KeytiaBaseField lField in this)
            {
                if (lField is KeytiaRelationField && lField.DataValue != null)
                {
                    lDataSet.Tables.Add((DataTable)lField.DataValue);
                    lDataSet.Tables[lDataSet.Tables.Count - 1].TableName = lField.ConfigName;
                }
            }
            return lDataSet;
        }

        public static DataTable GetTypes()
        {
            DataTable ldtTypes;
            KDBAccess lKDB = new KDBAccess();
            if (DSODataContext.GetObject("Types") == null)
            {
                ldtTypes = lKDB.GetHisRegByEnt("Types", "Tipos de Datos", new string[] { "iCodCatalogo", "{Controles}" });
                DSODataContext.SetObject("Types", ldtTypes);
            }
            else
            {
                ldtTypes = (DataTable)DSODataContext.GetObject("Types");
            }
            return ldtTypes;
        }

        public static DataTable GetAtrib()
        {
            DataTable ldtAtrib;
            KDBAccess lKDB = new KDBAccess();
            if (DSODataContext.GetObject("Atrib") == null)
            {
                ldtAtrib = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}", "{Entidad}" });
                DSODataContext.SetObject("Atrib", ldtAtrib);
            }
            else
            {
                ldtAtrib = (DataTable)DSODataContext.GetObject("Atrib");
            }
            return ldtAtrib;
        }

        public static DataTable GetControles()
        {
            DataTable ldtControles;
            KDBAccess lKDB = new KDBAccess();
            if (DSODataContext.GetObject("Controles") == null)
            {
                ldtControles = lKDB.GetHisRegByEnt("Controles", "Controles", new string[] { "iCodCatalogo", "{Clase}" });
                DSODataContext.SetObject("Controles", ldtControles);
            }
            else
            {
                ldtControles = (DataTable)DSODataContext.GetObject("Controles");
            }
            return ldtControles;
        }
    }
}
