using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeytiaServiceBL.DataAccess.ModelsDataAccess;
using KeytiaServiceBL.Models.Cargas;

namespace KeytiaServiceBL.Handler.Cargas
{
    public class LineasPendienteHandler
    {
        StringBuilder query = new StringBuilder();
        MaestroViewHandler maestroHand = new MaestroViewHandler();
        public int ICodMaestro { get; set; }
        public int EntidadCat { get; set; }

        public LineasPendienteHandler(string conexion)
        {
            var maestro = maestroHand.GetMaestroEntidad("Detall", "LineasPendiente", conexion);
            ICodMaestro = maestro.ICodRegistro;
            EntidadCat = maestro.ICodEntidad;
        }

        private string SelectLineasPendiente()
        {
            query.Length = 0;
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine(" ICodCatalogo, ");
            query.AppendLine(" ICodMaestro, ");
            query.AppendLine(" VchCodigo, ");
            query.AppendLine(" VchDescripcion, ");
            query.AppendLine(" Cargas, ");
            query.AppendLine(" Carrier, ");
            query.AppendLine(" Sitio, ");
            query.AppendLine(" Empre, ");
            query.AppendLine(" CenCos, ");
            query.AppendLine(" Recurs, ");
            query.AppendLine(" Emple, ");
            query.AppendLine(" CtaMaestra, ");
            query.AppendLine(" RegCarga, ");
            query.AppendLine(" BanderasLinea, ");
            query.AppendLine(" CargoFijo, ");
            query.AppendLine(" FechaInicio, ");
            query.AppendLine(" FechaFin, ");
            query.AppendLine(" [Clave.] AS Clave, ");
            query.AppendLine(" Tel, ");
            query.AppendLine(" IMEI, ");
            query.AppendLine(" ModeloCel,");
            query.AppendLine(" Filler, ");
            query.AppendLine(" DtFecha, ");
            query.AppendLine(" ICodUsuario, ");
            query.AppendLine(" DtFecUltAct");
            query.AppendLine("FROM " + DiccVarConf.PendientesCargaLineas);

            return query.ToString();
        }

        public List<LineasPendiente> GetByIdCarga(int iCodCatalogo, string connStr)
        {
            try
            {
                SelectLineasPendiente();
                query.AppendLine(" WHERE iCodCatalogo = " + iCodCatalogo.ToString());

                return GenericDataAccess.ExecuteList<LineasPendiente>(query.ToString(), connStr);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(DiccMens.DL001, ex);
            }
        }

        public int InsertPendiente(LineasPendiente detallCod, string conexion)
        {
            try
            {
                detallCod.ICodMaestro = ICodMaestro;
                return GenericDataAccess.InsertAll(DiccVarConf.PendientesCargaLineas, conexion, detallCod, new List<string> { "ICodRegistro", "VchCodigo", "Clave" }, "ICodRegistro");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public bool UpdateClave(string where, string clave, string conexion)
        {
            try
            {
                if (where.ToUpper().Contains("WHERE"))
                {
                    query.Length = 0;
                    query.AppendLine("UPDATE " + DiccVarConf.PendientesCargaLineas);
                    query.AppendLine("SET [Clave.] = '" + clave + "'");
                    query.AppendLine(where);

                    GenericDataAccess.ExecuteNonQuery(query.ToString(), conexion);
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(DiccMens.DL001, ex);
            }
        }

        public bool DeleteTopByiCodCarga(int iCodCarga, string conexion)
        {
            try
            {
                query.Length = 0;
                query.AppendLine("DELETE TOP(" + DiccVarConf.TopDeValoresAEliminarBD + ") " + DiccVarConf.PendientesCargaLineas);
                query.AppendLine("WHERE iCodCatalogo = " + iCodCarga);

                GenericDataAccess.ExecuteNonQuery(query.ToString(), conexion);
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(DiccMens.DL044, ex);
            }
        }

        public int GetCountByiCodCarga(int iCodCarga, string conexion)
        {
            try
            {
                query.Length = 0;
                query.AppendLine("SELECT COUNT(*) FROM " + DiccVarConf.PendientesCargaLineas);
                query.AppendLine("WHERE iCodCatalogo = " + iCodCarga);

                return (int)((object)GenericDataAccess.ExecuteScalar(query.ToString(), conexion));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(DiccMens.DL001, ex);
            }
        }

    }
}
