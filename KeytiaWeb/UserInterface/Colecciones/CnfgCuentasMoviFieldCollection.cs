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
    public class CnfgCuentasMoviFieldCollection : HistoricFieldCollection
    {       
        public CnfgCuentasMoviFieldCollection()
        { 
        }

        public CnfgCuentasMoviFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgCuentasMoviFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }

        protected override void FillMetaData()
        {
            base.FillMetaData();

            DataRow[] ldr = pMetaData.Select("ConfigName = 'TMSGroup'");
            if (ldr.Length > 0)
            {
                DataRow lDataRow = ldr[0];
                lDataRow["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAutoFilteredField";
            }
        }

        protected override void CreateFields()
        {
            base.CreateFields();
            if (ContainsConfigName("TMSGroup"))
            {
                KeytiaAutoFilteredField lField = (KeytiaAutoFilteredField)GetByConfigName("TMSGroup");
                lField.iCodDataSourceParam = (int)DSODataAccess.ExecuteScalar(
                    "select IsNull(iCodCatalogo, 0) " + "\r\n" +
                    "from [VisHistoricos('DataSourceParam','DataSource Parametro','Español')] " + "\r\n" +
                    "where dtIniVigencia <> dtFinVigencia " + "\r\n" +
                    "and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " + "\r\n" +
                    "and dtFinVigencia > '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " + "\r\n" +
                    "and vchCodigo = 'DSTMSGroupProvFilter'", (object)0);
            }
        }
    }
}
