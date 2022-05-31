using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeytiaServiceBL.Models.Cargas;
using KeytiaServiceBL.DataAccess.ModelsDataAccess;

namespace KeytiaServiceBL.Handler.Cargas
{
    public class UsuariosPendienteHandler
    {
        StringBuilder query = new StringBuilder();
        MaestroViewHandler maestroHand = new MaestroViewHandler();
        public int ICodMaestro { get; set; }
        public int EntidadCat { get; set; }

        public UsuariosPendienteHandler(string conexion)
        {
            var maestro = maestroHand.GetMaestroEntidad("Detall", "Usuarios Pendiente", conexion);
            ICodMaestro = maestro.ICodRegistro;
            EntidadCat = maestro.ICodEntidad;
        }

        private string SelectUsuariosPendiente()
        {
            query.Length = 0;
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine(" ICodCatalogo, ");
            query.AppendLine(" ICodMaestro, ");
            query.AppendLine(" VchCodigo, ");
            query.AppendLine(" VchDescripcion, ");
            query.AppendLine(" Perfil, ");
            query.AppendLine(" Empre, ");
            query.AppendLine(" Idioma, ");
            query.AppendLine(" Moneda, ");
            query.AppendLine(" UsuarDB, ");
            query.AppendLine(" INumCatalogo, ");
            query.AppendLine(" UltAcc, ");
            query.AppendLine(" NominaA, ");
            query.AppendLine(" Password, ");
            query.AppendLine(" HomePage, ");
            query.AppendLine(" Email, ");
            query.AppendLine(" ConfPassword, ");
            query.AppendLine(" DtFecha, ");
            query.AppendLine(" ICodUsuario, ");
            query.AppendLine(" DtFecUltAct");
            query.AppendLine("FROM " + DiccVarConf.PendientesCargaUsuarios);

            return query.ToString();
        }

        public List<UsuariosPendiente> GetByIdCarga(int iCodCatalogo, string connStr)
        {
            try
            {
                SelectUsuariosPendiente();
                query.AppendLine(" WHERE iCodCatalogo = " + iCodCatalogo);

                return GenericDataAccess.ExecuteList<UsuariosPendiente>(query.ToString(), connStr);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(DiccMens.DL001, ex);
            }
        }

        public int InsertPendiente(UsuariosPendiente detallUsuar, string conexion)
        {
            try
            {
                detallUsuar.ICodMaestro = ICodMaestro;
                return GenericDataAccess.InsertAll(DiccVarConf.PendientesCargaUsuarios, conexion, detallUsuar, new List<string> { "ICodRegistro", "VchCodigo" }, "ICodRegistro");
            }
            catch (Exception ex)
            {
                throw new ArgumentException(DiccMens.DL001, ex);
            }
        }

        public bool EliminarRegistroByiCodReg(int iCodRegistro, string conexion)
        {
            try
            {
                query.Length = 0;
                query.AppendLine("DELETE " + DiccVarConf.PendientesCargaUsuarios);
                query.AppendLine("WHERE iCodRegistro = " + iCodRegistro);

                GenericDataAccess.ExecuteNonQuery(query.ToString(), conexion);
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(DiccMens.DL044, ex);
            }
        }

        public bool DeleteTopByiCodCarga(int iCodCarga, string conexion)
        {
            try
            {
                query.Length = 0;
                query.AppendLine("DELETE TOP(" + DiccVarConf.TopDeValoresAEliminarBD + ") " + DiccVarConf.PendientesCargaUsuarios);
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
                query.AppendLine("SELECT COUNT(*) FROM " + DiccVarConf.PendientesCargaUsuarios);
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
