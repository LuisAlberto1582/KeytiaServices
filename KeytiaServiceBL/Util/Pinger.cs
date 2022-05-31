using System;
using System.Collections;
using System.Data;
using System.Threading;

namespace KeytiaServiceBL
{
    public class Pinger
    {
        protected string psProcess = "";
        protected int piSleepTime = 0;
        protected bool pbKeepRunning = false;
        protected bool pbFirstPing = false;

        protected KDBAccess kdb = new KDBAccess();
        protected static Pinger poPng = new Pinger();

        /// <summary>
        /// Revisa si no se encuentra corriendo un ping, para ir validar que el proceso (param) se encuentre corriendo
        /// en nuevo thread
        /// </summary>
        /// <param name="lsProcess">Nombre del Proceso</param>
        /// <param name="liSleepTime">Tiempo de reposo</param>
        public static void StartPing(string lsProcess, int liSleepTime)
        {
            //Si no se encuentra ya corriendo
            if (!poPng.KeepRunning)
            {
                //Establecer el valor de las propiedades Process y SleepTime
                poPng.Process = lsProcess;
                poPng.SleepTime = liSleepTime;

                //Crea un nuevo treah y lo lanza con el metodo Ping del objeto de la clase Pinger
                Thread ltThread = new Thread(poPng.Ping);
                ltThread.Start();

                //Si no se trata del primer ping, el thread se detiene por 100 milisegundos
                while(!poPng.FirstPing)
                    System.Threading.Thread.Sleep(100);

            }
        }
        
        /// <summary>
        /// Es llamado cuando se quiere finalizar el pinger instanciado.
        /// </summary>
        public static void StopPing()
        {
            poPng.KeepRunning = false;
        }

        public string Process
        {
            set { psProcess = value; }
            get { return psProcess; }
        }

        public int SleepTime
        {
            set { piSleepTime = value; }
            get { return piSleepTime; }
        }

        public bool KeepRunning
        {
            set { pbKeepRunning = value; }
            get { return pbKeepRunning; }
        }

        public bool FirstPing
        {
            get { return pbFirstPing; }
        }

        /// <summary>
        /// Actualiza un el valor del campo {Ping} en el historico que lleve de clave el nombre del proceso
        /// Si se encuentra actualizará un timestamp en el campo mencionado.
        /// </summary>
        public void Ping()
        {
            Hashtable lht = new Hashtable();
            int liCodRegistro = -1;
            bool lbHayError = false;
            DataTable ldt;

            pbKeepRunning = true;

            //Establecer conexion con el esquema default -Keytia
            DSODataContext.SetContext();

            Util.LogMessage("Iniciando Pinger");

            try
            {
                //Busca el registro del proceso en base al vchcodigo
                ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "iCodRegistro", "{LimpCache}", "{LimpKDB}" }, "vchCodigo = '" + psProcess + "'");

                //Si se encontro el registro, almacenamos su icodregistro
                if (ldt != null && ldt.Rows.Count > 0)
                    liCodRegistro = (int)ldt.Rows[0]["iCodRegistro"];
                else
                {
                    //Si no se encontro entonces se insertará un nuevo historico.
                    KeytiaCOM.CargasCOM lcc = new KeytiaCOM.CargasCOM();

                    //Inserta el registro
                    lht.Clear();
                    lht.Add("vchCodigo", psProcess);
                    lht.Add("vchDescripcion", psProcess);
                    lht.Add("dtFecUltAct", DateTime.Today);
                    lht.Add("dtIniVigencia", DateTime.Today);
                    lht.Add("dtFinVigencia", new DateTime(2079, 1, 1));

                    try
                    {
                        lcc.Carga(Util.Ht2Xml(lht), "Historicos", "Monitor", "Proceso", 0);
                    }
                    catch (Exception ex)
                    {
                        //Si llegara a fallar el insert, se deja en false la bandera lbHayError
                        lbHayError = true;
                        Util.LogException("No existía el registro de monitoreo y no se pudo agregar uno nuevo. El monitoreo queda deshabilitado.", ex);
                    }

                    //Si la variable de lbHayError no es falsa
                    if (!lbHayError)
                    {
                        //Busca nuevamente el registro del proceso
                        ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "iCodRegistro" }, "vchCodigo = '" + psProcess + "'");

                        //Si se encuentra se almacena en la variable liCodRegistro, si no lanza una excepción.
                        if (ldt != null && ldt.Rows.Count > 0)
                            liCodRegistro = (int)ldt.Rows[0]["iCodRegistro"];
                        else
                            throw new Exception("No se encontró registro de monitoreo.");
                    }
                }

                //Si no es false
                if (!lbHayError)
                {
                    //Limpiamos el hash y agregamos el atributo Pinger
                    lht.Clear();
                    lht.Add("{Ping}", null);

                    //Mientras siga corriendo
                    while (pbKeepRunning)
                    {
                        //Modificamos el valor del elemento {Ping} se agrega el valor de DateTime.Now
                        lht["{Ping}"] = DateTime.Now;

                        //Se realiza la actualizacion del historico
                        kdb.FechaVigencia = DateTime.Today;
                        kdb.Update("Historicos", "Monitor", "Proceso", lht, liCodRegistro);

                        try
                        {
                            //Revisar el cache de la aplicacion
                            CheckCache();
                        }
                        catch (Exception ex)
                        {
                            Util.LogException("Error al revisar caché.", ex);
                        }
                        
                        pbFirstPing = true;

                        /* Si la iteracion es menor al piSleepTime = 20 y pbKeepRunning es true, 
                         * se suspende 1000 mil milisegundos
                         */
                        for (int i = 0; i < piSleepTime && pbKeepRunning; i++)
                            System.Threading.Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error en monitoreo. El monitoreo queda deshabilitado.", ex);
                lbHayError = true;
            }

            //if (lbHayError)
            //{
            //    Util.LogMessage("Reiniciando Pinger (" + psProcess + ":" + piSleepTime + ").");
            //    Pinger.StartPing(psProcess, piSleepTime);
            //}

            pbFirstPing = true;
            Util.LogMessage("Pinger Finalizado");

            /*Se establece en false la propiedad bool 'pbKeepRunnig'*/
            pbKeepRunning = false;
        }

        /// <summary>
        /// Revisar el estatus del cache e imprimirlo en los logs
        /// </summary>
        public void CheckCache()
        {
            Hashtable lht = new Hashtable();

            string lsCache = psProcess + "." + System.Diagnostics.Process.GetCurrentProcess().Id;

            DataTable ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "{LimpCache}", "{LimpKDB}" }, "vchCodigo = '" + psProcess + "'");
            if (ldt != null && ldt.Rows.Count > 0)
            {
                //if (ldt.Rows[0]["{LimpCache}"] != System.DBNull.Value && (DateTime)ldt.Rows[0]["{LimpCache}"] >= DateTime.Now.AddMinutes(-1))
                if (ldt.Rows[0]["{LimpCache}"] != System.DBNull.Value)
                {
                    string lsCod = lsCache + "." + ((DateTime)ldt.Rows[0]["{LimpCache}"]).ToString("yyyyMMdd.HHmmss");
                    DataTable ldtCache = kdb.GetHisRegByEnt("Monitor", "Cache", new string[] { }, "vchCodigo = '" + lsCod + "'");

                    if (ldtCache == null || ldtCache.Rows.Count == 0)
                    {
                        int liCtx = DSODataContext.GetContext();

                        DSODataContext.LogCache("Cache de " + lsCache + "." + System.Threading.Thread.CurrentThread.ManagedThreadId + " (antes)");
                        DSODataContext.ClearCache();
                        DSODataContext.LogCache("Cache de " + lsCache + "." + System.Threading.Thread.CurrentThread.ManagedThreadId + " (después)");

                        DSODataContext.SetContext(liCtx);

                        lht.Clear();
                        lht.Add("vchCodigo", lsCod);
                        lht.Add("{Ping}", DateTime.Now);

                        KeytiaCOM.CargasCOM loCom = new KeytiaCOM.CargasCOM();
                        loCom.InsertaRegistro(lht, "Historicos", "Monitor", "Cache", DSODataContext.GetContext());
                    }
                }

                //if (ldt.Rows[0]["{LimpKDB}"] != System.DBNull.Value && (DateTime)ldt.Rows[0]["{LimpCache}"] >= DateTime.Now.AddMinutes(-1))
                if (ldt.Rows[0]["{LimpKDB}"] != System.DBNull.Value)
                {
                    string lsCod = lsCache + "." + ((DateTime)ldt.Rows[0]["{LimpKDB}"]).ToString("yyyyMMdd.HHmmss");
                    DataTable ldtCache = kdb.GetHisRegByEnt("Monitor", "Cache", new string[] { }, "vchCodigo = '" + lsCod + "'");

                    if (ldtCache == null || ldtCache.Rows.Count == 0)
                    {
                        DSODataContext.LogCache("Cache de " + lsCache + "." + System.Threading.Thread.CurrentThread.ManagedThreadId + " (antes)");
                        KeytiaServiceBL.KDBAccess.CleanBuffer();
                        DSODataContext.LogCache("Cache de " + lsCache + "." + System.Threading.Thread.CurrentThread.ManagedThreadId + " (después)");

                        lht.Clear();
                        lht.Add("vchCodigo", lsCod);
                        lht.Add("{Ping}", DateTime.Now);

                        KeytiaCOM.CargasCOM loCom = new KeytiaCOM.CargasCOM();
                        loCom.InsertaRegistro(lht, "Historicos", "Monitor", "Cache", DSODataContext.GetContext());
                    }
                }
            }

        }

        //RZ.20131014
        /// <summary>
        /// Se encarga de revisar el estatus del proceso
        /// </summary>
        /// <param name="lsProcess">Nombre del proceso</param>
        /// <returns>Bool que dice si el proceso se encuentra corriendo o no</returns>
        public static bool ProcessIsRunning(string lsProcess)
        {
            return ProcessIsRunning(lsProcess, 1);
        }

        /// <summary>
        /// Se encarga de revisar el estatus del proceso
        /// </summary>
        /// <param name="lsProcess">Nombre del proceso</param>
        /// <param name="liMinutes">Minutos a agregar en la fecha/hora del ping</param>
        /// <returns>Bool que dice si el proceso se encuentra corriendo o no</returns>
        public static bool ProcessIsRunning(string lsProcess, int liMinutes)
        {
            //valor de retorno incializado en false
            bool lbRet = false;

            //valor de fecha del ping inicializado en minvalue
            DateTime ldtPing = DateTime.MinValue;
            KDBAccess kdb = new KDBAccess();
            System.Data.DataTable ldt;

            //Extraer el contexto del esquema activo en la sesion de la pagina
            int liCtx = DSODataContext.GetContext();

            //Se conecta con las credenciales default del sistema. Esquema Keytia
            DSODataContext.SetContext();
            
            //Extraer el valor del ping (fecha) del proceso recibido como parametro
            ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "{Ping}" }, "vchCodigo = '" + lsProcess + "'");
            //retornar al contexto del esquema de la sesion de la pagina
            DSODataContext.SetContext(liCtx);

            /*Si el datatable no es nulo, tiene filas y el valor de {Ping} es diferente de null
             * entonces guarda en ldtPing en valor del atributo Ping del historico
             */
            if (ldt != null && ldt.Rows.Count > 0 && ldt.Rows[0]["{Ping}"] != System.DBNull.Value)
                ldtPing = (DateTime)ldt.Rows[0]["{Ping}"];

            /*Si la fecha del ping es diferente de minvalue y si la fecha del ping + 1 es mayor o 
             * igual a la fecha de ahora, entonces regresa un true.
             */
            if (ldtPing != DateTime.MinValue && ldtPing.AddMinutes(liMinutes) >= DateTime.Now)
                lbRet = true;

            return lbRet;
        }
    }
}
