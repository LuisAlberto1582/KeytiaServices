using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Threading;

namespace KeytiaServiceBL
{
    public class LanzadorPrepProv
    {
        protected KDBAccess kdb;
        protected bool pbSigueCorriendo;
        protected int piCodUsuarioDB = -1;
        public int iCodUsuarioDB
        {
            get { return piCodUsuarioDB; }
            set { piCodUsuarioDB = value; }
        }


        //RJ.20190901. Substituí el método original, por éste que no hace nada, pues las consultas del original
        //consumían muchos recursos del servidor y ningún cliente tiene habilitado este proceso
        public void Start()
        {
            pbSigueCorriendo = true;

            while (pbSigueCorriendo)
            {
                //espera 60 seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < 30 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);

                if (!pbSigueCorriendo)
                    break;

                try
                {
                    pbSigueCorriendo = true; //RJ 20190901. Solo para que haga algo
                }
                catch (Exception ex)
                {
                    Util.LogException(ex);
                }

                //espera 10 seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < 30 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);
            }
        }


        //RJ.Método Start original, lo omití porque las consultas consumen recursos del servidor y ningun
        //cliente tiene habilitado este proceso.
        //public void Start()
        //{
        //    pbSigueCorriendo = true;

        //    if (kdb == null)
        //    {
        //        kdb = new KDBAccess();
        //    }

        //    while (pbSigueCorriendo)
        //    {
        //        //espera 60 seg, pero si llega señal de terminar, termina la espera para salir
        //        for (int i = 0; i < 30 && pbSigueCorriendo; i++)
        //            System.Threading.Thread.Sleep(1000);

        //        if (!pbSigueCorriendo)
        //            break;

        //        try
        //        {
        //            if (kdb == null)
        //            {
        //                kdb = new KDBAccess();
        //            }
        //            DSODataContext.SetContext(0);
        //            string lsEntidad = "UsuarDB";
        //            string lsMaestro = "Usuarios DB";
        //            //RZ.20130820 Leer el valor de la ip del servidor
        //            string lsServidorServicio = Util.AppSettings("ServidorServicio");

        //            /*RZ.20130402 Agregar en el ldtUsuarDB un filtro para que solo los esquemas en los que la bandera 
        //              "Activar Presupuesto" esta encendida, el valor integer del atributo es 1 (Entidad y Maestro: Valores)
        //              Esto se encuentra fijo en el argumento lsInnerWhere, por lo que se espera que nunca cambie su valor en el histórico. 
        //             *RZ.20130820 Filtrar solo aquellos servicios donde el servidor debe correr los presupuestos {ServidorServicio}
        //             */

        //            DataTable ldtUsuarDB = kdb.GetHisRegByEnt(lsEntidad, lsMaestro, "((isnull({BanderasUsuarDB},0)) & 1) / 1 = 1 and {ServidorServicio} = '" + lsServidorServicio + "'");
        //            if (ldtUsuarDB != null && ldtUsuarDB.Rows.Count > 0)
        //            {
        //                foreach (DataRow ldrUsuarDB in ldtUsuarDB.Rows)
        //                {
        //                    piCodUsuarioDB = (int)ldrUsuarDB["iCodCatalogo"];
        //                    DSODataContext.SetContext(piCodUsuarioDB);
        //                    Calcular();
        //                }
        //            }
        //            else
        //            {
        //                piCodUsuarioDB = 0;
        //                DSODataContext.SetContext(piCodUsuarioDB);
        //                Calcular();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Util.LogException(ex);
        //        }

        //        //espera 10 seg, pero si llega señal de terminar, termina la espera para salir
        //        for (int i = 0; i < 30 && pbSigueCorriendo; i++)
        //            System.Threading.Thread.Sleep(1000);
        //    }
        //}

        public void Stop()
        {
            pbSigueCorriendo = false;
        }

        public void Calcular()
        {
            DataTable ldtSitios = getSitios();
            foreach (DataRow ldrSitio in ldtSitios.Rows)
            {
                int liMinutos = (int)Util.IsDBNull(ldrSitio["Minutos"], 0);
                DateTime ldtSigAct = (DateTime)Util.IsDBNull(ldrSitio["SigAct"], DateTime.MinValue);

                if (ldtSigAct < DateTime.Now)
                {
                    try
                    {
                        ldtSigAct = getSigAct(ldtSigAct, liMinutos);
                        ActualizarSigAct((int)ldrSitio["iCodRegistro"], ldtSigAct);
                        PresupuestosSitios loPresupuestos = new PresupuestosSitios();
                        loPresupuestos.iCodUsuarioDB = piCodUsuarioDB;
                        loPresupuestos.DiaInicioPeriodo = (int)ldrSitio["DiaInicioPeriodo"];
                        loPresupuestos.PeriodoPr = (int)ldrSitio["PeriodoPr"];
                        loPresupuestos.Sitio = (int)ldrSitio["Sitio"];
                        Thread ltThread = new Thread(loPresupuestos.procesarAltasPresupuestoTemporal);
                        ltThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Util.LogException("No se pudo insertar el presupuesto temporal en el archivo.", ex);
                    }
                }
            }

            //RZ.20140217
            #region Altas en inicio de periodo.
            try
            {
                //Busca el valor de la bandera que se utiliza para activar la generación de archivos para bloquear códigos.
                int liValorBandera = (int)DSODataAccess.ExecuteScalar(
                    "select IsNull(Value, 0) from [" + DSODataContext.Schema + "].[VisHistoricos('Valores','Valores','Español')]" +
                    " where vchCodigo = 'ActivarAltaBajaCodExtPrep'", (object)0);
                int liBanderasEmpre;

                //Obtiene el listado de empresas que tienen habilitado la creación de archivos para altas y bajas de recursos
                DataTable ldtEmpre = DSODataAccess.Execute(
                    "Select distinct NotifPrepEmpre.*, BanderasEmpre " + //, Empre = Empre.iCodCatalogo
                    " from [" + DSODataContext.Schema + "].[VisHistoricos('Empre','Empresas','Español')] Empre," +
                    "      [" + DSODataContext.Schema + "].[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','Español')] NotifPrepEmpre," +
                    "      [" + DSODataContext.Schema + "].[VisHisComun('Sitio','Español')] Sitio" +
                    " where Sitio.Empre = Empre.iCodCatalogo" +
                    " and NotifPrepEmpre.Empre = Empre.iCodCatalogo" +
                    " and Empre.dtIniVigencia <> Empre.dtFinVigencia" +
                    " and Empre.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and Empre.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and NotifPrepEmpre.dtIniVigencia <> NotifPrepEmpre.dtFinVigencia" +
                    " and NotifPrepEmpre.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and NotifPrepEmpre.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                //Recorrer cada empresa para procesar sus sitios y los archivos de alta a crear
                foreach (DataRow ldr in ldtEmpre.Rows)
                {
                    liBanderasEmpre = (int)Util.IsDBNull(ldr["BanderasEmpre"], 0);
                    int liDiaInicioPeriodo = (int)Util.IsDBNull(ldr["DiaInicioPeriodo"], 0);

                    //Se lee el dia de inicio de periodo para saber si es el dia de hoy y si es asi entonces ver si para la empresa
                    //hay archivos de alta para procesar, de no ser asi recorrera la siguiente empresa.
                    if (((liBanderasEmpre & liValorBandera) == liValorBandera) && liDiaInicioPeriodo == DateTime.Today.Day) //Proceso activo
                    {
                        PresupuestosSitios loPresupuestos = new PresupuestosSitios();
                        loPresupuestos.iCodUsuarioDB = piCodUsuarioDB;
                        loPresupuestos.iCodCarga = int.MinValue; //Esta propiedad no se usa en el proceso
                        loPresupuestos.DiaInicioPeriodo = liDiaInicioPeriodo;
                        loPresupuestos.PeriodoPr = (int)ldr["PeriodoPr"];
                        loPresupuestos.Empre = (int)ldr["Empre"];

                        //Procesa los archivos para Altas de un inicio de periodo
                        loPresupuestos.procesarAltasInicioPeriodo();
                    }
                }
            }
            catch (Exception e)
            {
                Util.LogException("No se ha podido procesar las altas en inicio de periodo", e);
            }
            #endregion
        }

        private DateTime getSigAct(DateTime ldtSigAct, int liMinutos)
        {
            while (ldtSigAct < DateTime.Now)
            {
                ldtSigAct = ldtSigAct.AddMinutes(liMinutos);
            }
            return ldtSigAct;
        }

        private void ActualizarSigAct(int liCodNotifPrepSitio, DateTime ldtSigAct)
        {
            Hashtable lhtValores = new Hashtable();
            lhtValores.Add("{SigAct}", ldtSigAct);

            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            cargasCOM.ActualizaRegistro("Historicos", "NotifPrepSitio", "Notificaciones de Presupuestos para Sitios", lhtValores, liCodNotifPrepSitio, piCodUsuarioDB);

        }

        protected DataTable getSitios()
        {
            return DSODataAccess.Execute(
                "Select NotifPrepSitio.iCodRegistro, NotifPrepSitio.Sitio," +
                "       NotifPrepSitio.Minutos, NotifPrepSitio.SigAct," +
                "       NotifPrepEmpre.DiaInicioPeriodo, NotifPrepEmpre.PeriodoPr" +
                " from [" + DSODataContext.Schema + "].[VisHistoricos('NotifPrepSitio','Notificaciones de Presupuestos para Sitios','Español')] NotifPrepSitio," +
                "      [" + DSODataContext.Schema + "].[VisHisComun('Sitio','Español')] Sitio," +
                "      [" + DSODataContext.Schema + "].[VisHistoricos('Empre','Empresas','Español')] Empre," +
                "      [" + DSODataContext.Schema + "].[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','Español')] NotifPrepEmpre" +
                " where NotifPrepSitio.Sitio = Sitio.iCodCatalogo" +
                " and Sitio.Empre = Empre.iCodCatalogo" +
                " and Empre.BanderasEmpre & 2 = 2" +
                " and NotifPrepSitio.BanderasPrepSitios & 1 = 1" +
                " and Empre.iCodCatalogo = NotifPrepEmpre.Empre" +
                " and Empre.dtIniVigencia <> Empre.dtFinVigencia" +
                " and Empre.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and Empre.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and Sitio.dtIniVigencia <> Sitio.dtFinVigencia" +
                " and Sitio.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and Sitio.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and NotifPrepEmpre.dtIniVigencia <> NotifPrepEmpre.dtFinVigencia" +
                " and NotifPrepEmpre.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and NotifPrepEmpre.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and NotifPrepSitio.dtIniVigencia <> NotifPrepSitio.dtFinVigencia" +
                " and NotifPrepSitio.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and NotifPrepSitio.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
        }

    }
}
