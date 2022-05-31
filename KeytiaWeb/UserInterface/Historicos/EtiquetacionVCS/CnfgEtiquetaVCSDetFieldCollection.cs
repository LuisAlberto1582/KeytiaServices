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
    public class CnfgEtiquetaVCSDetFieldCollection : HistoricFieldCollection
    {
        public CnfgEtiquetaVCSDetFieldCollection()
        {
        }

        public CnfgEtiquetaVCSDetFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgEtiquetaVCSDetFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }


        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);
            if (lField.ConfigName == "VCSSourceSystem" || lField.ConfigName == "VCSDestinationSystem")
            {
                lField.ConfigName = "TMSSystems";
            }

            return lField;
        }

        public override void SetValues(DataRow lDataRow)
        {
            foreach (KeytiaBaseField lField in this)
            {
                if (lField.ConfigName == "DuracionSeg")
                {
                    ((DSONumberEdit)((KeytiaNumericField)lField).DSOControlDB).NumberBox.Text =
                        KeytiaServiceBL.Reportes.ReporteEstandarUtil.TimeToString(lDataRow[lField.ConfigName], Globals.GetCurrentLanguage());
                }
                else if (!(lField is KeytiaRelationField) && lDataRow.Table.Columns.Contains(lField.ConfigName))
                {
                    lField.DataValue = lDataRow[lField.ConfigName];
                }
                else
                {
                    DataRow[] ldr = pMetaData.Select("ConfigValue = " + lField.ConfigValue);
                    if (ldr.Length > 0 && !(lField is KeytiaRelationField) && lDataRow.Table.Columns.Contains(ldr[0]["ConfigName"].ToString()))
                    {
                        lField.DataValue = lDataRow[ldr[0]["ConfigName"].ToString()];
                    }
                    else
                    {
                        lField.DataValue = lDataRow["iCodCatalogo"];
                    }
                }
            }
        }

        public override void InitLanguage()
        {
            DataRow[] lMetaData = pMetaData.Select("ConfigName = 'VCSSourceSystem' or ConfigName = 'VCSDestinationSystem'");
            foreach (DataRow ldr in lMetaData)
            {
                KeytiaBaseField lField = GetByConfigValue((int)ldr["ConfigValue"]);
                object DataValue = lField.DataValue;
                lField.ConfigName = ldr["ConfigName"].ToString();
                lField.InitLanguage();
                if (lField.DSOControlDB != null)
                {
                    lField.InitDSOControlDBLanguage();
                }
                lField.ConfigName = "TMSSystems";
                lField.DataValue = DataValue;
            }
            foreach (KeytiaBaseField lField in this)
            {
                if (lField.ConfigName != "TMSSystems")
                {
                    lField.InitLanguage();
                    if (lField.DSOControlDB != null)
                    {
                        lField.InitDSOControlDBLanguage();
                    }
                }
            }
        }

        public override Hashtable GetValues()
        {
            Hashtable lht = new Hashtable();
            KeytiaBaseField lField;
            if (ContainsConfigName("Proyecto"))
            {
                lField = GetByConfigName("Proyecto");
                lht.Add("{Proyecto}", lField.DataValue);
            }
            if (ContainsConfigName("TipoConferencia"))
            {
                lField = GetByConfigName("TipoConferencia");
                lht.Add("{TipoConferencia}", lField.DataValue);
            }
            return lht;
        }
    }
}