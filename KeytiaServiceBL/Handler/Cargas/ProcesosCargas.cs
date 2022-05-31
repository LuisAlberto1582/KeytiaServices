using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeytiaServiceBL.DataAccess.ModelsDataAccess;
namespace KeytiaServiceBL.Handler.Cargas
{
    public static class ProcesosCargas
    {
        private static bool _finalizoCorrectamente = false;

        public static bool RegeneraInfoReporteHistorico(string esquema)
        {
            _finalizoCorrectamente = false;

            try
            {
                GenericDataAccess.ExecuteNonQuery("EXEC dbo.RegeneraInfoRepHistorico @Schema = '" + esquema + "'");
                _finalizoCorrectamente = true;
            }
            catch(Exception ex)
            {
                Util.LogException(ex);
            }


            return _finalizoCorrectamente;
        }

        public static bool RegeneraInfoIndicadoresDashboard(string esquema)
        {
            _finalizoCorrectamente = false;

            try
            {
                GenericDataAccess.ExecuteNonQuery("dbo.RegeneraInfoIndicadores @Eschema= '" + esquema + "'");
                _finalizoCorrectamente = true;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }

            return _finalizoCorrectamente; ;
        }

        public static bool RegeneraInfoReporteCenCosJerarq(string esquema)
        {
            _finalizoCorrectamente = false;

            try
            {
                GenericDataAccess.ExecuteNonQuery("EXEC dbo.EjecutaJerarquiaCliente @Schema = '" + esquema + "'");
                _finalizoCorrectamente = true;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }

            return _finalizoCorrectamente; ;
        }
    }
}
