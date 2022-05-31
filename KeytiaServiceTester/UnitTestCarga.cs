using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceTester
{
    class UnitTestCargas
    {
        public void cargarCarga()
        {
            CargaMeet();
        }

        public void CargaEliminaSiana()
        {
            int icodCatUsuarDB = 128252;//icodcatalogo usardb Bat
            int icodCatCargaCDR = 242836;//icodcatalogo vistas carga

            DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaGenerica.CargaEliminaSiana.CargaEliminaSiana();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;

            Carga.IniciarCarga();
        }
        public void _ProsaFacturas()
        {

            //cide 96451
            //carga 292073
            //Prosa 79521
            int icodCatUsuarDB = 79521;//icodcatalogo usardb Bat
            int icodCatCargaCDR = 334868;//icodcatalogo vistas carga

            DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaFacturas.CargaFacturaTelmexUninetFullTIM.CargaFacturaTelmexUninetFullTIM();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;

            Carga.IniciarCarga();
        }

        public void _ProsaCargaEmpleUnid()
        {

            //cide 96451
            //carga 292073
            //Prosa 79521
            int icodCatUsuarDB = 79521;//icodcatalogo usardb Bat
            int icodCatCargaCDR = 336850;//icodcatalogo vistas carga

            DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaGenerica.CargaEmpleadoUnidad.CargaEmpleadoUnidad();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;

            Carga.IniciarCarga();
        }

        public void _ActualizaPresupuesto()
        {
            int icodCatUsuarDB = 79482;//icodcatalogo usardb Bat
            int icodCatCargaCDR = 367500;//icodcatalogo vistas carga

            DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaGenerica.ActualizaPresupuestoLineas.ActualizaPresupuestoLineas();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;

            Carga.IniciarCarga();
        }

        public void _CargaDicomtec()
        {
            int icodCatUsuarDB = 77703;//icodcatalogo usardb Bat
            int icodCatCargaCDR = 488326;//icodcatalogo vistas carga

            DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaGenerica.CargaDicomtec.CargaDicomtec
            {
                CodUsuarioDB = icodCatUsuarDB,
                CodCarga = icodCatCargaCDR
            };

            //Carga.IniciarCarga();

            Carga.EliminarCarga(icodCatCargaCDR);
        }

        #region CargaInfoC1toQlik
        // Carga 

        public void _CideCargarInfoQlik()
        {
            //cide 96451
            int icodCatUsuarDB = 96451;//icodcatalogo usardb cide
            int icodCatCargaCDR = 292077;//icodcatalogo vistas carga

            DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaGenerica.GenerarInformacionProduccion.GenerarInformacionProduccion();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;

            Carga.IniciarCarga();
        }

        public void _KioCargarInfoQlik()
        {
            //KIOc12 117469
            //218480
            int icodCatUsuarDB = 117469;//icodcatalogo usardb kio
            int icodCatCargaCDR = 218549;//icodcatalogo vistas carga

            DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaGenerica.GenerarInformacionProduccion.GenerarInformacionProduccion();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;

            Carga.IniciarCarga();
        }

        #endregion

        #region Cargas Automaticas
        public void CargaMeet()
        {
            try
            {
                string esquema = "COneEvox";
                string claveCarga = "Insertar meet Cargas";

                int icodCatUsuarDB;
                int icodCatCargaCDR;
                DSODataContext.SetContext();
                icodCatUsuarDB =
                    (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from COneEvox.[vishistoricos('UsuarDB','Usuarios DB','Español')] where dtfinvigencia>=getdate() and esquema = '{0}'", esquema));
                DSODataContext.SetContext(icodCatUsuarDB);



                icodCatCargaCDR =
                    (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from COneEvox.[vishistoricos('cargas','Cargas Videoconf','español')] where dtfinvigencia>=getdate() and vchcodigo = '{0}'", claveCarga));

                var Carga =
                    new KeytiaServiceBL.CargaVideoConf.CargaServicioVideoConf();

                DSODataContext.SetContext(icodCatUsuarDB);

                Carga.CodUsuarioDB = icodCatUsuarDB;
                Carga.CodCarga = icodCatCargaCDR;

                Carga.IniciarCarga();

            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
