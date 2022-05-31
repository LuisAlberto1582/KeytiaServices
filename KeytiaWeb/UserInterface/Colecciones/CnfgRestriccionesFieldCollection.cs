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
    public class CnfgRestriccionesFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadRest;

        public CnfgRestriccionesFieldCollection()
        { 
        }

        public CnfgRestriccionesFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) {}

        public CnfgRestriccionesFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }

        protected override void AgregarSubHistoricField(int liCodEntidad, int liCodMaestro, string lSubHistoricClass, string lSubCollectionClass)
        {
            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubFullHistoricField", liCodEntidad, liCodMaestro, lSubHistoricClass, lSubCollectionClass);
        }

        protected override void AgregarSubHistoricField(int liCodEntidad, int liCodMaestro)
        {
            AgregarSubHistoricField(liCodEntidad, liCodMaestro, "KeytiaWeb.UserInterface.CnfgRestriccionesEdit", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }

        protected override void AgregarSubHistoricField(string lsSubHistoricoFieldClass, int liCodEntidad, int liCodMaestro, string lSubHistoricClass, string lSubCollectionClass)
        {
            DataRow lMetaDataRow = pMetaData.NewRow();
            string lsMaestro = pdtMaestros.Select("iCodRegistro = " + liCodMaestro)[0]["vchDescripcion"].ToString();
            DataTable lKDBTable = pKDB.GetHisRegByEnt("MsgWeb", "Restricciones", " vchDescripcion = '" + lsMaestro.Replace("'","''") + "'");

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
    }
}
