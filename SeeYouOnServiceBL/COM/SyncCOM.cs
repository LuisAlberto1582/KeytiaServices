using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

using System.Configuration;
using System.EnterpriseServices;
using System.Runtime.InteropServices;

using KeytiaServiceBL;
using SeeYouOnServiceBL;

[assembly: ApplicationActivation(ActivationOption.Server)]
[assembly: ApplicationQueuing(Enabled = true, QueueListenerEnabled = true)]

namespace SeeYouOnCOM
{
    [ComVisible(true)]
    [Guid("E2493B80-4E32-450a-BAA8-2A1641CA835C")]
    [ProgId("SeeYouOnCOM.SyncCOM")]
    [ClassInterface(ClassInterfaceType.None)]
    [Transaction(TransactionOption.Disabled)]
    [InterfaceQueuing(Enabled = true, Interface = "ISyncCOM")]
    [ConstructionEnabled(true)]

    public class SyncCOM : ServicedComponent, ISyncCOM
    {
        protected KDBAccess kdb = null;
        protected object poLock = new object();

        #region COM Init
        protected override void Construct(string s)
        {
            ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = s;
            Util.SetConfig(ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None));

            kdb = new KDBAccess();

            //RZ.20140602 Se retira llamado a clase Pinger
            //Pinger.StartPing("SeeYouOnCOM", 20);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            //RZ.20140602 Se retira llamado a clase Pinger
            //if (disposing)
            //    Pinger.StopPing();
        }

        /// <summary>
        /// Sirve para establecer la conexion con el esquema indicado
        /// </summary>
        /// <param name="liDataContext">iCodCatalogo del UsuarDB del Esquema</param>
        protected void IndicarEsquema(int liDataContext)
        {
            //Establece el contexto del esquema 
            DSODataContext.SetContext(liDataContext);
            //Se establece la fecha en la fecha de hoy
            kdb.FechaVigencia = DateTime.Today;
        }

        public void Ping()
        {
            Util.LogMessage("Ping");
            //throw new Exception("xxx");
            //return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        #endregion


        public void SyncTMSs(int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                DataTable ldt = kdb.GetHisRegByEnt("ServidorTMS", "Servidor TMS",
                    new string[] { "iCodCatalogo" });

                if (ldt != null)
                    foreach (DataRow ldr in ldt.Rows)
                    {
                        SyncSystems((int)ldr["iCodCatalogo"], liDataContext);
                        SyncPhoneBooks((int)ldr["iCodCatalogo"], liDataContext);
                        SyncProvGroups((int)ldr["iCodCatalogo"], liDataContext);
                        SyncMoviAccounts((int)ldr["iCodCatalogo"], liDataContext);
                        //SyncConferences((int)ldr["iCodCatalogo"], liDataContext);
                    }
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de servidores TMS y SeeYouOn.", ex);
                throw ex;
            }
        }

        public void SyncSystems(int liCodTMS, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                TMS loTMSSync = new TMS();
                loTMSSync.SyncSystems(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de sistemas TMS y SeeYouOn.", ex);
                throw ex;
            }
        }

        public void SyncMoviAccounts(int liCodTMS, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                TMS loTMSSync = new TMS();
                loTMSSync.SyncMoviAccounts(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de cuentas movi TMS y SeeYouOn.", ex);
                throw ex;
            }
        }

        public void SyncPhoneBooks(int liCodTMS, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                TMS loTMSSync = new TMS();
                loTMSSync.SyncPhoneBooks(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de PhoneBooks TMS y SeeYouOn.", ex);
                throw ex;
            }
        }

        public void SyncConferences(int liCodTMS, int liDataContext)
        {
            IndicarEsquema(liDataContext);

            try
            {
                TMS loTMSSync = new TMS();
                loTMSSync.SyncConferences(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de conferencias TMS y SeeYouOn.", ex);
                throw ex;
            }
        }

        public void SyncProvGroups(int liCodTMS, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                TMS loTMSSync = new TMS();
                loTMSSync.SyncProvGroups(liCodTMS);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de grupos de provisioning TMS y SeeYouOn.", ex);
                throw ex;
            }
        }

        public void TMSMoviSave(int liCodMoviAccount, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                TMS loTMSSync = new TMS();
                loTMSSync.TMSMoviSave(liCodMoviAccount);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al grabar la cuenta movi.", ex);
                throw ex;
            }
        }

        public void TMSMoviDelete(int liCodMoviAccount, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                TMS loTMSSync = new TMS();
                loTMSSync.TMSMoviDelete(liCodMoviAccount);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al borrar la cuenta movi.", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Guarda una conferencia en el MCU de SYO
        /// </summary>
        /// <param name="liCodConf">iCodCatalogo de la Conferencia</param>
        /// <param name="liDataContext">iCodCatalogo del Usuario DB del Esquema</param>
        public void MCUConfSave(int liCodConf, int liDataContext)
        {
            try
            {
                //Se establece la conexcion con el usuardb enviado como parametro
                IndicarEsquema(liDataContext);

                //Se crea un objeto de la clase MCU.
                MCU loMCU = new MCU();
                //Guarda la conferencia del SYO hacia el MCU
                loMCU.SaveConferenceSYO2MCU(liCodConf);
            }
            catch (Exception ex)
            {
                //Se produce un error se guarda en log el error
                Util.LogException("Surgió un error al grabar la conferencia en el MCU.", ex);
                throw ex;
            }
        }

        public int MCUAvailable(int liCodConf, int liDataContext)
        {
            int liRet = 0;
            try
            {
                IndicarEsquema(liDataContext);

                MCU loMCU = new MCU();
                liRet = loMCU.GetAvailablePorts(liCodConf);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al grabar la conferencia en el MCU.", ex);
                throw ex;
            }

            return liRet;
        }

        public void MCUConfDelete(int liCodConf, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);

                MCU loMCU = new MCU();
                loMCU.DeleteConferenceSYO2MCU(liCodConf);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error al borrar la conferencia en el MCU.", ex);
                throw ex;
            }
        }

        public void SyncMCUs(int liDataContext)
        {
            try
            {

                IndicarEsquema(liDataContext);

                MCU loMCUSync = new MCU();
                
                loMCUSync.SyncSYOConferencesMCU();

            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de Conferencias de SeeYouOn.", ex);
                throw ex;
            }
        }
        public void NotificAsistConf(int liCodConferencia, string lsLang, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);
                NotificacionAsistentes loNotif = new NotificacionAsistentes();
                loNotif.iCodConferencia = liCodConferencia;
                loNotif.vchIdioma = lsLang;
                loNotif.NotificAsistConf();
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la notificación a los asistentes de la conferencia.", ex);
                throw ex;
            }
        }
        public void EnviaCtaMOVI(int liCodCuentaMovi, string lsLang, string lsStyle, int liDataContext)
        {
            try
            {
                IndicarEsquema(liDataContext);
                EnvioCuentasMOVI loEnviaCta = new EnvioCuentasMOVI();
                loEnviaCta.EnvioCuentaServidorTMS(lsLang, lsStyle, liCodCuentaMovi);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la notificación a los asistentes de la conferencia.", ex);
                throw ex;
            }
        }
        public void RemoveCtasMOVI(int liDataContext)
        {
            try
            {

                IndicarEsquema(liDataContext);

                TMS loTMSSync = new TMS();
                loTMSSync.RemoveCtasMOVI();

            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de Cuentas MOVI de SeeYouOn.", ex);
                throw ex;
            }
        }

    }

}
