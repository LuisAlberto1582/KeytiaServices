using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;

namespace KeytiaWeb.UserInterface
{
    public class CnfgContratoSeeYouOnFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadServicioSeeYouOn;

        public CnfgContratoSeeYouOnFieldCollection()
        {
        }

        public CnfgContratoSeeYouOnFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgContratoSeeYouOnFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadServicioSeeYouOn = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'ServicioSeeYouOn'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadServicioSeeYouOn + " and dtIniVigencia <> dtFinVigencia");

            foreach (DataRow lDataRow in pdtMaestros.Rows)
            {
                AgregarSubHistoricField(piCodEntidadServicioSeeYouOn, (int)lDataRow["iCodRegistro"]);
            }
        }
    }
}
