using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.UtilCarga
{
    public static class claseFormatoFecha
    {
        private static int YearId;
        private static int MesId;
        public static DateTime ObtenerFecha(int pYearId,int  pMesId)
        {
            YearId = pYearId;
            MesId = pMesId;
            int MES = 0;
            int Anio = 0;

            DataTable dtResultado = DSODataAccess.Execute(ObtenerCodFechaMes());
            foreach (DataRow dr in dtResultado.Rows)
            {
                MES = Convert.ToInt32(dr["vchCodigo"]);
            }
            dtResultado = DSODataAccess.Execute(ObtenerCodFechaAnio());
            foreach (DataRow dr in dtResultado.Rows)
            {
                Anio = Convert.ToInt32(dr["vchCodigo"]);
            }
            return new DateTime(Anio, MES, 1);
        }

        public static string ObtenerCodFechaMes()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT vchCodigo");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[vishistoricos('Mes','Meses','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND iCodCatalogo =" + MesId);
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            return query.ToString();
        }

        public static string ObtenerCodFechaAnio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT vchCodigo");
            query.AppendLine(" from " + DSODataContext.Schema + ".[vishistoricos('Anio','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND iCodCatalogo =" + YearId);
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            return query.ToString();
        }
    }
}
