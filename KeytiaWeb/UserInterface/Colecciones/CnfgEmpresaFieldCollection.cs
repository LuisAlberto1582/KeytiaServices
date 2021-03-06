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
    public class CnfgEmpresaFieldCollection : CnfgRestriccionesFieldCollection
    {

        public CnfgEmpresaFieldCollection()
        {
        }

        public CnfgEmpresaFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgEmpresaFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadRest = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'RestriccionesEmpresa'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadRest + " and dtIniVigencia <> dtFinVigencia order by vchDescripcion");

            foreach (DataRow ldataRow in pdtMaestros.Rows)
            {
                AgregarSubHistoricField("RestriccionesEmpresa", ldataRow["vchDescripcion"].ToString());
            }
        }
    }
}