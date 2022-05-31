/*
Nombre:		    DDCP
Fecha:		    20110225
Descripción:	Clase con la lógica para tasar los registros CDR de los conmutadores
Modificación:	
 * 20120123 RJ.Se modifica el método enviarNotificacion: Se cambia el método que se manda llamar, 
                anteriormente era notificacionEnviada(double ldblNivel) y cambia a: notificacionEnviada(double ldblNivel, DateTime FechaInicioPrep)
                Se crea el método notificacionEnviada(double ldblNivel, DateTime FechaInicioPrep), que es 
                tomado del original notificacionEnviada(double ldblNivel), la diferencia es que éste valida
                el campo FechaInicioPrep
 * 20130829 PT. Se modifico el metodo enviarCorreo y se agregaron metodos para generar y agregar adjunto al correo de notificacion un reporte detallado

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KeytiaServiceBL.Alarmas;
using System.Net.Mail;
using System.Collections;
using KeytiaServiceBL.Reportes;

namespace KeytiaServiceBL
{
    public class EmpleadoPpto : Empleado
    {
        protected int piCodPrepEmple;
        protected double pdblPresupuesto = 0;
        protected double pdblConsumo = 0;
        protected double pdblNivelConsumo = 0;
        protected int piCodTipoPr = 0;
        protected int piCodPeriodoPr = 0;
        protected string pvchTipoPrCod = "";
        protected string pvchPeriodoPrCod = "";
        protected Empleado poResponsableCC = null;
        protected Empleado poJefeDirecto = null;
        protected DateTime pdtFechaInicioPrep = DateTime.Now;
        protected bool pbPresupProv = false;
        protected double pdblValorConsumoBase = 0;
        protected double pdblNivelAlerta;

        public Empleado ResponsableCC
        {
            get { return poResponsableCC; }
            set { poResponsableCC = value; }
        }
        public Empleado JefeDirecto
        {
            get { return poJefeDirecto; }
            set { poJefeDirecto = value; }
        }
        public int PrepEmple
        {
            get { return piCodPrepEmple; }
            set { piCodPrepEmple = value; }
        }
        public double Presupuesto
        {
            get { return pdblPresupuesto; }
            set { pdblPresupuesto = value; }
        }
        public double Consumo
        {
            get { return pdblConsumo; }
            set { pdblConsumo = value; }
        }
        public double NivelConsumo
        {
            get { return pdblNivelConsumo; }
            set { pdblNivelConsumo = value; }
        }
        public double ValorConsumoBase
        {
            get { return pdblValorConsumoBase; }
            set { pdblValorConsumoBase = value; }
        }
        public int TipoPr
        {
            get { return piCodTipoPr; }
            set { piCodTipoPr = value; }
        }
        public int PeriodoPr
        {
            get { return piCodPeriodoPr; }
            set { piCodPeriodoPr = value; }
        }
        public string TipoPrCod
        {
            get { return pvchTipoPrCod; }
            set { pvchTipoPrCod = value; }
        }
        public string PeriodoPrCod
        {
            get { return pvchPeriodoPrCod; }
            set { pvchPeriodoPrCod = value; }
        }
        public DateTime FechaInicioPrep
        {
            get { return pdtFechaInicioPrep; }
            set { pdtFechaInicioPrep = value; }
        }
        public bool PresupProv
        {
            get { return pbPresupProv; }
            set { pbPresupProv = value; }
        }
        public double NivelAlerta
        {
            get { return pdblNivelAlerta; }
            set { pdblNivelAlerta = value; }
        }

        //NZ 20150924 Se agrega campo para guardar el numero como tal del nivel de alerta
        public int iNumAlerta { get; set; }

        public EmpleadoPpto(int liCodUsuario)
            : base(liCodUsuario)
        {
        }
        public EmpleadoPpto(int liCodEmpleado, bool getSuper)
            : base(liCodEmpleado, getSuper)
        {
            if (getSuper)
            {
                try
                {
                    KDBAccess kdb = new KDBAccess();
                    DataTable ldt = kdb.GetHisRegByEnt("Emple", "Empleados",
                        "iCodCatalogo = " + liCodEmpleado);

                    if (ldt != null && ldt.Rows.Count > 0)
                    {
                        if (ldt.Columns.Contains("{Emple}") && !(ldt.Rows[0]["{Emple}"] is DBNull) && (int)ldt.Rows[0]["{Emple}"] != liCodEmpleado)
                        {
                            poJefeDirecto = new Empleado((int)ldt.Rows[0]["{Emple}"], false);
                        }
                        if (ldt.Columns.Contains("{CenCos}") && !(ldt.Rows[0]["{CenCos}"] is DBNull))
                        {
                            piCodCenCos = (int)ldt.Rows[0]["{CenCos}"];
                            DataTable ldtCenCos = kdb.GetHisRegByEnt("CenCos", "",
                                new string[] { "{Emple}" },
                                "iCodCatalogo = " + piCodCenCos.ToString());
                            if (ldtCenCos != null && ldtCenCos.Rows.Count > 0 && !(ldtCenCos.Rows[0]["{Emple}"] is DBNull) && (int)ldtCenCos.Rows[0]["{Emple}"] != liCodEmpleado)
                            {
                                poResponsableCC = new Empleado((int)ldtCenCos.Rows[0]["{Emple}"], false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al inicializar los datos del empleado: (" + piCodEmpleado + ") / Usuario: (" + piCodUsuario + ")", ex);
                }
            }
        }

        public EmpleadoPpto(int liCodEmpleado, bool getSuper, DataRow ldrPrepEmple)
            : this(liCodEmpleado, getSuper)
        {
            piCodPrepEmple = (int)ldrPrepEmple["iCodCatalogo"];
            pdblPresupuesto = (double)ldrPrepEmple["PresupFijo"];
            piCodTipoPr = (int)ldrPrepEmple["TipoPr"];
            piCodPeriodoPr = (int)ldrPrepEmple["PeriodoPr"];
            pvchTipoPrCod = ldrPrepEmple["TipoPrCod"].ToString();
            pvchPeriodoPrCod = ldrPrepEmple["PeriodoPrCod"].ToString();
            //pdtFechaInicioPrep = (DateTime)ldrPrepEmple["FechaInicioPrep"];
        }

        public void getConsumo(DateTime ldtFecIni, DateTime ldtFecFin)
        {
            //Obtiene el importe total de consumo que ha generado el empleado a partir de las fechas recibidas como parametros
            //Los importes son tanto en pesos, en minutos y en llamadas
            DataTable ldtConsumo = DSODataAccess.Execute(
                "select Costo = IsNull(SUM(Costo), 0) + IsNull(SUM(CostoSM), 0), " +
                "       Duracion = IsNull(SUM(DuracionMin), 0), " +
                "       Llamadas = COUNT(iCodRegistro) " +
                " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','DetalleCDR','Español')] " +
                " where Emple = " + piCodEmpleado +
                " and   FechaInicio >= '" + ldtFecIni.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                " and   FechaInicio <  '" + ldtFecFin.ToString("yyyy-MM-dd HH:mm:ss") + "' ");



            //Dependiendo del tipo de presupuesto configurado (Costo, Duracion o Llamadas) en la empresa 
            //que se está procesando se asigna el valor del consumo a la variable pdblConsumo
            switch (pvchTipoPrCod)
            {
                case "Costo":
                    pdblConsumo = double.Parse(ldtConsumo.Rows[0]["Costo"].ToString());
                    break;
                case "Duracion":
                    pdblConsumo = double.Parse(ldtConsumo.Rows[0]["Duracion"].ToString());
                    break;
                case "Llamadas":
                    pdblConsumo = (int)ldtConsumo.Rows[0]["Llamadas"];
                    break;
            }
        }

        public bool notificacionEnviada(double ldblNivel)
        {
            //Leer de la bitácora un registro con la notificación, el empleado y el nivel
            DataTable ldtNotif = DSODataAccess.Execute(
                "select iCodRegistro " +
                " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Notificacion Consumos','Español')]" +
                " where Emple = " + piCodEmpleado +
                " and PeriodoPr = " + piCodPeriodoPr +
                " and TipoPr = " + piCodTipoPr +
                " and PrepEmple = " + piCodPrepEmple +
                " and ValorConsumoBase = " + pdblValorConsumoBase +
                " and NivelAlerta = " + ldblNivel +
                " and FechaReset is null" +
                " and Msg is null");
            return ldtNotif.Rows.Count > 0;
        }


        /// <summary>
        /// RJ.Se agrega este método, en donde incluye el parámetro FechaInicioPrep que no está validando en el método
        /// original y esto ocasiona que no valide el periodo actual en la consulta
        /// </summary>
        /// <param name="ldblNivel"></param>
        /// <param name="FechaInicioPrep"></param>
        /// <returns></returns>
        public bool notificacionEnviada(double ldblNivel, DateTime FechaInicioPrep)
        {
            //Leer de la bitácora un registro con la notificación, el empleado y el nivel
            DataTable ldtNotif = DSODataAccess.Execute(
                "select iCodRegistro " +
                " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Notificacion Consumos','Español')]" +
                " where Emple = " + piCodEmpleado +
                " and PeriodoPr = " + piCodPeriodoPr +
                " and TipoPr = " + piCodTipoPr +
                " and PrepEmple = " + piCodPrepEmple +
                " and ValorConsumoBase = " + pdblValorConsumoBase +
                " and NivelAlerta = " + ldblNivel +
                " and FechaInicioPrep = '" + FechaInicioPrep.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and FechaReset is null" +
                " and Msg is null");
            return ldtNotif.Rows.Count > 0;
        }

    }

    public class NotificacionPresupuestos
    {
        #region Campos

        protected KDBAccess kdb = new KDBAccess();
        protected int piCodUsuarioDB;
        protected string psTempPath;

        protected int piCodCatalogo;
        protected DataRow pdrRegistro;

        protected int piCodCarga;
        protected int piCodEmpre;
        protected int piCodTipoPr;
        protected int piCodPeriodoPr;
        protected int piCodRepEst;
        protected int piDiaInicioPeriodo;
        protected DataTable pdtBanderas;
        protected int piBanderas;
        protected bool pbEnviarJefeDirecto;
        protected bool pbEnviarARespCC;
        protected bool pbIncluirPresupuesto;
        protected bool pbIncluirConsumo;
        protected bool pbIncluirPorcentaje;
        protected bool pbIncluirDetalleEnArchAdjunto; //RJ.Para indicar si se envía archivo adjunto o no
        protected double pdblNivelAlerta1;
        protected double pdblNivelAlerta2;
        protected double pdblNivelAlerta3;
        protected double pdblNivelAlerta4;
        protected int piNivelAlerta;
        protected string psPara;
        protected string psCC;
        protected string psCCO;
        protected string psPlantilla;
        protected string psLang;
        protected int piCodAsunto;
        protected DataTable pdtExepcionesEmpleados;
        protected DataTable pdtPrepEmple;
        protected MailAccess poMail;
        protected DateTime pdtFechaInicioPrep;
        protected DateTime pdtFechaFinPrep;
        protected HashSet<int> phsEmpleadosReintentoBaja = new HashSet<int>();
        //20130814 PT: Se agrego este campo para agregar datos adjuntos al correo
        protected HashSet<string> plstAdjuntos = new HashSet<string>();


        #endregion

        #region Delegados y Eventos

        delegate void postEnvio(EmpleadoPpto loEmpleado, string lsMsg);

        #endregion

        #region Propiedades


        public int iCodUsuarioDB
        {
            get
            {
                return piCodUsuarioDB;
            }
            set
            {
                piCodUsuarioDB = value;
            }
        }
        public int iCodCarga
        {
            get
            {
                return piCodCarga;
            }
            set
            {
                piCodCarga = value;
            }
        }

        #endregion


        #region Constructores

        public NotificacionPresupuestos(DataRow ldrRegistro)
        {
            pdrRegistro = ldrRegistro;
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            initVars();
        }

        #endregion




        protected void initVars()
        {
            //Obtiene los valores configurados en el sistema para cada una de las banderas del atributo BanderasNotificacionPresupuestosEmpre
            //dichos valores se asignan a variable que son utilizadas más adelante
            getBanderas();

            piCodCatalogo = (int)getValCampo("iCodCatalogo", 0);
            piCodEmpre = (int)getValCampo("{Empre}", 0);
            piCodTipoPr = (int)getValCampo("{TipoPr}", 0);
            piCodPeriodoPr = (int)getValCampo("{PeriodoPr}", 0);
            piCodRepEst = (int)getValCampo("{RepEst}", 0);
            piDiaInicioPeriodo = (int)getValCampo("{DiaInicioPeriodo}", 0);
            pdblNivelAlerta1 = (double)getValCampo("{NivelAlerta1}", 0.0);
            pdblNivelAlerta2 = (double)getValCampo("{NivelAlerta2}", 0.0);
            pdblNivelAlerta3 = (double)getValCampo("{NivelAlerta3}", 0.0);
            pdblNivelAlerta4 = (double)getValCampo("{NivelAlerta4}", 0.0);
            psPara = (string)getValCampo("{DestPrueba}", "");
            psCC = (string)getValCampo("{CtaCC}", "");
            psCCO = (string)getValCampo("{CtaCCO}", "");
            psPlantilla = (string)getValCampo("{Plantilla}", "");

            //Obtiene el listado de empleados que se deben tomar como excepciones dentro del proceso
            //y no ser tomados en cuenta para el envío de notificaciones
            pdtExepcionesEmpleados = kdb.GetRelRegByDes("Notificaciones de Presupuestos de Empresas - Excepciones de Empleados", "{NotifPrepEmpre} = " + piCodCatalogo);

            psLang = UtilAlarma.getIdioma((int)getValCampo("{Idioma}", 0));
            piCodAsunto = (int)getValCampo("{Asunto}", 0);


            getFechasPeriodo(piDiaInicioPeriodo, piCodPeriodoPr, out pdtFechaInicioPrep, out pdtFechaFinPrep);
        }

        protected virtual void getBanderas()
        {
            //Obtiene los valores configurados en el sistema para cada una de las banderas del atributo BanderasNotificacionPresupuestosEmpre
            //dichos valores se asignan a variable que son utilizadas más adelante
            piBanderas = (int)getValCampo("{BanderasNotificacionPresupuestosEmpre}", 0);
            int liCodBandera = (int)Util.IsDBNull(kdb.GetHisRegByEnt("Atrib", "Atributos", "vchCodigo = 'BanderasNotificacionPresupuestosEmpre'").Rows[0]["iCodCatalogo"], 0);
            pdtBanderas = kdb.GetHisRegByEnt("Valores", "Valores", new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBandera);

            pbEnviarJefeDirecto = getValBandera("EnviarJefeDirecto");
            pbEnviarARespCC = getValBandera("EnviarARespCC");
            pbIncluirPresupuesto = getValBandera("IncluirPresupuesto");
            pbIncluirConsumo = getValBandera("IncluirConsumo");
            pbIncluirPorcentaje = getValBandera("IncluirPorcentaje");
            pbIncluirDetalleEnArchAdjunto = getValBandera("IncluirDetalleComoAdjunto");
        }

        protected object getValCampo(string lsCampo, object defaultValue)
        {
            if (pdrRegistro.Table.Columns.Contains(lsCampo))
                return Util.IsDBNull(pdrRegistro[lsCampo], defaultValue);
            return defaultValue;
        }

        protected bool getValBandera(string vchCodigo)
        {
            return getValBandera(pdtBanderas, piBanderas, vchCodigo);
        }

        protected bool getValBandera(DataTable ldtBanderas, int liBandera, string vchCodigo)
        {
            return getValBandera(ldtBanderas, liBandera, vchCodigo, false);
        }

        protected bool getValBandera(DataTable ldtBanderas, int liBandera, string vchCodigo, bool defaultValue)
        {
            int liValBandera = 0;
            DataRow[] rows = ldtBanderas.Select("vchCodigo = '" + vchCodigo + "'");
            if (rows.Length > 0)
            {
                liValBandera = (int)rows[0]["{Value}"];
            }
            else
            {
                return defaultValue;
            }
            return (liBandera & liValBandera) == liValBandera;
        }

        //RJ.20130607
        //Encontré una inconsistencia en el proceso que identifica los empleados a los que se debe
        //enviar una notificacion a los empleados que excedieron su presupuesto. 
        //La clase CargaServicioCDR tiene un método llamado ProcesarNotificacionesPresupuestos() 
        //mismo que identifica las empresas que tienen encendida la bandera de "Enviar Notificaciones", 
        //para después hacer un ciclo que recorre cada una de dichas empresas y dentro de él 
        //ejecuta el proceso de notificaciones para la empresa en curso.
        //Más adelante, en el método getPrepEmple() de la clase NotificacionPresupuestos
        //se obtienen todos los empleados que tienen configurardo un presupuesto 
        //y que generaron llamadas en la carga de cdr que recién finalizó. El problema radica en
        //que en dicho método (getPrepEmple()) no se valida que los empleados obtenidos correspondan
        //sólo a sitios que correspondan a la Empresa en curso, sino que ignora la Empresa y regresa
        //todos los empleados sin importar a qué sitio pertenecen.
        //No corregí el proceso debido a que se están mandando los presupuestos de GModelo y el aplicar
        //algún cambio representaría que el proceso no se ejecutara correctamente, como lo hace en este
        //momento para este cliente. 
        //Lo que se tiene que hacer para corregir este detalle es filtrar los empleados que regresa
        //el método getPrepEmple() para que tomen en cueta sólo aquellos que correspondan a sitios
        //de la empresa que está en curso en el ciclo. Pero además, por cada empleado calcular el consumo
        //total sin importar el sitio de donde generó llamadas. Esto último para prevenir casos en donde
        //un empleado haya hecho llamadas desde dos sitios de empresas diferentes, si no se incluye dicha
        //suma sin importar los sitios, se enviarán dos notificaciones al empleado, una con cada uno de los
        //consumos por Empresa.
        public void Main()
        {
            try
            {
                DSODataContext.SetContext(piCodUsuarioDB);
                Procesar();
            }
            catch (Exception ex)
            {
                Util.LogException("Error inesperado durante la ejecución de la Notificación de Presupuestos. (Código=" + piCodCatalogo + ")", ex);
            }
        }

        private void Procesar()
        {
            //Obtiene el listado de empleados a los que se deberá enviar una notificación, este listado se forma
            //al ejecutar el método getEmpleadosNotificacion()
            Hashtable lhsEmpleados = getEmpleadosNotificacion();

            //Ciclo que recorre uno a uno los empleados incluídos en el listado obtenido en el paso anterior
            //por cada uno de ellos se manda llamar el método EnviarCorreo(), que es el encargado de enviar 
            //la notificación
            foreach (EmpleadoPpto loEmpleado in lhsEmpleados.Values)
            {
                EnviarCorreo(loEmpleado);
            }


            //Si en el paso anterior se encontraron empleados a los que se les deba mandar un segundo aviso
            //de notificación, debido a que se encontró que su código sigue generando consumo, se manda llamar
            //el método enviarNotificacionReintentarBaja() que lleva a cabo esta tarea.
            if (phsEmpleadosReintentoBaja.Count > 0)
            {
                enviarNotificacionReintentarBaja();
            }
        }

        private Hashtable getEmpleadosNotificacion()
        {
            Hashtable lhtEmpleados = new Hashtable();

            //Obtiene el listado de empleados que tienen configurado un presupuesto fijo, se encuentran dentro de la
            //carga que se está procesando y además no se encuentren configurados como excepciones de envío
            pdtPrepEmple = getPrepEmple();



            //Ciclo que recorre cada empleado encontrado en el paso anterior
            foreach (DataRow ldrPrepEmple in pdtPrepEmple.Rows)
            {
                //Se crea una instancia de la clase EmpleadoPpto
                EmpleadoPpto loEmpleado = new EmpleadoPpto((int)ldrPrepEmple["Emple"], true, ldrPrepEmple);

                //RZ.20140218 Establecer la fecha de inicio de presupuesto no tomando en cuenta la que viene de getPrepEmple
                //ya que esta mal, solo la calcula de forma mensual y tomando como dia de inicio el dia 1 y no el que se tiene configurado.
                loEmpleado.FechaInicioPrep = pdtFechaInicioPrep;

                //Obtiene el importe total de consumo que ha generado el empleado a partir de las fechas recibidas como parametros
                //Los importes son tanto en pesos, en minutos y en llamadas
                getNivelConsumo(loEmpleado);


                if (enviarNotificacion(loEmpleado))
                {
                    lhtEmpleados.Add(loEmpleado.iCodEmpleado, loEmpleado);
                }
            }
            return lhtEmpleados;
        }

        protected virtual DataTable getPrepEmple()
        {
            //Obtiene el listado de Empleados que generaron información en la carga de CDR
            //que se está procesando, pero sólo incluye aquellos que ya excedieron en por lo menos
            //el 50% de su presupuesto
            StringBuilder lsbExecSp = new StringBuilder();

            //RZ.20140228 Se cambia el sp para que reciba como parametro la fecha inicio y fecha fin de presupuesto, que se calcula al instanciar el objeto de esta clase
            lsbExecSp.Append("exec ObtieneEmpleadosExcedieronPpto @esquema = '" + DSODataContext.Schema + "', \r");
            lsbExecSp.Append("@icodCatalogoCargaCDR = " + piCodCarga.ToString() + ", \r");
            lsbExecSp.Append("@icodCatalogoTipoPr = " + piCodTipoPr.ToString() + ", \r");
            lsbExecSp.Append("@icodCatalogoPeriodoPr = " + piCodPeriodoPr.ToString() + ", \r");
            lsbExecSp.Append("@fechaInicioPresupuesto = '" + pdtFechaInicioPrep.ToString("yyyy-MM-dd HH:mm:ss") + "', \r");
            lsbExecSp.Append("@fechaFinPresupuesto = '" + pdtFechaFinPrep.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            return DSODataAccess.Execute(lsbExecSp.ToString());

        }


        private bool enviarNotificacion(EmpleadoPpto loEmpleado)
        {
            //Si se exedió del presupuesto y no se ha enviado la notificación, se envía.
            piNivelAlerta = 0;
            loEmpleado.NivelAlerta = 0;


            if (pdblNivelAlerta4 > 0 && pdblNivelAlerta4 <= loEmpleado.NivelConsumo)
            {
                if (!loEmpleado.notificacionEnviada(pdblNivelAlerta4, loEmpleado.FechaInicioPrep))
                {
                    piNivelAlerta = 4;
                    loEmpleado.NivelAlerta = pdblNivelAlerta4;
                    //NZ 20150924
                    loEmpleado.iNumAlerta = 4;
                }
            }
            else if (pdblNivelAlerta3 > 0 && pdblNivelAlerta3 <= loEmpleado.NivelConsumo)
            {
                if (!loEmpleado.notificacionEnviada(pdblNivelAlerta3, loEmpleado.FechaInicioPrep))
                {
                    piNivelAlerta = 3;
                    loEmpleado.NivelAlerta = pdblNivelAlerta3;
                    //NZ 20150924
                    loEmpleado.iNumAlerta = 3;
                }
            }
            else if (pdblNivelAlerta2 > 0 && pdblNivelAlerta2 <= loEmpleado.NivelConsumo)
            {
                if (!loEmpleado.notificacionEnviada(pdblNivelAlerta2, loEmpleado.FechaInicioPrep))
                {
                    piNivelAlerta = 2;
                    loEmpleado.NivelAlerta = pdblNivelAlerta2;
                    //NZ 20150924
                    loEmpleado.iNumAlerta = 2;
                }
            }
            else if (pdblNivelAlerta1 > 0 && pdblNivelAlerta1 <= loEmpleado.NivelConsumo)
            {
                if (!loEmpleado.notificacionEnviada(pdblNivelAlerta1, loEmpleado.FechaInicioPrep))
                {
                    piNivelAlerta = 1;
                    loEmpleado.NivelAlerta = pdblNivelAlerta1;
                    //NZ 20150924
                    loEmpleado.iNumAlerta = 1;
                }
            }
            return piNivelAlerta > 0;
        }

        private void EnviarCorreo(EmpleadoPpto loEmpleado)
        {
            List<string> lstPara = new List<string>();
            List<string> lstCC = new List<string>();

            string lsWordPath = UtilAlarma.buscarPlantilla(psPlantilla, psLang);

            //20130814 PT: se agrego este campo para aduntar reporte detalado
            // al correo de notificacion de presupustos
            plstAdjuntos.Clear();


            //RJ.Se valida si en configuración se estableció algo en el campo
            //DestinatarioPrueba, de ser así los correos se envían a dicha cuenta
            //de lo contrario se enviará a la cuenta que tenga asociada el Empleado.
            if (string.IsNullOrEmpty(psPara))
            {
                lstPara.Add(getDestinatarios(loEmpleado));


            }
            else
            {
                //Si la cuenta de prueba, establecida en la configuración, no está en blanco
                //no se enviarán copias ni copias ocultas
                lstPara.Add(psPara);

                psCC = string.Empty;
                psCCO = string.Empty;
            }



            lstCC.Add(psCC);

            if (pbEnviarJefeDirecto && loEmpleado.JefeDirecto != null && !string.IsNullOrEmpty(loEmpleado.JefeDirecto.Email) && !lstPara.Contains(loEmpleado.JefeDirecto.Email))
            {
                lstCC.Add(loEmpleado.JefeDirecto.Email);
            }
            if (pbEnviarARespCC && loEmpleado.ResponsableCC != null && !string.IsNullOrEmpty(loEmpleado.ResponsableCC.Email) && !lstPara.Contains(loEmpleado.ResponsableCC.Email))
            {
                lstCC.Add(loEmpleado.ResponsableCC.Email);
            }



            //Además de tratar de enviar el correo, este método ejecuta el método GuardarBitacora() que registra en la
            //vista 'Detall','Bitacora Notificacion Consumos','Español' si el envío ha sido exitoso o no, si el campo
            //msg es igual a NULL quiere decir que sí se envió correctamente, de lo contrario en dicho campo registra
            //un mensaje especificando que no se envió el correo
            EnviarCorreo(loEmpleado, lsWordPath, piCodRepEst, piCodAsunto, string.Join(";", lstPara.ToArray()), string.Join(";", lstCC.ToArray()), psCCO, true, pbIncluirDetalleEnArchAdjunto, GuardarBitacora);


        }

        /// <summary>
        /// Envía correo de notificación
        /// </summary>
        /// <param name="loEmpleado"></param>
        /// <param name="lsWordPath"></param>
        /// <param name="liCodReporte"></param>
        /// <param name="liCodAsunto"></param>
        /// <param name="lsPara"></param>
        /// <param name="lsCC"></param>
        /// <param name="lsCCO"></param>
        /// <param name="lbReemplazarMsgWeb"></param>
        /// <param name="Bitacora"></param>
        private void EnviarCorreo(EmpleadoPpto loEmpleado, string lsWordPath, int liCodReporte,
            int liCodAsunto, string lsPara, string lsCC, string lsCCO, bool lbReemplazarMsgWeb,
            bool lbEnviarArchivoAdjunto, postEnvio Bitacora)
        {
            if (string.IsNullOrEmpty(lsPara) || string.IsNullOrEmpty(lsWordPath))
            {
                return;
            }
            WordAccess loWord = null;

            try
            {
                //NZ 20150924 cambiara la plantilla que se usara para mandar el correo de acuerdo al nivel de alerta que tenga.
                string tempPath = lsWordPath;
                int indexExtenArchvo = lsWordPath.LastIndexOf('.');
                tempPath = tempPath.Insert(indexExtenArchvo, loEmpleado.iNumAlerta.ToString());

                if (!System.IO.File.Exists(tempPath)) //Si no existe la plantilla, asigna la plantilla por default.
                {
                    tempPath = lsWordPath;
                }
                lsWordPath = tempPath;
                // NZ



                loWord = new WordAccess();
                loWord.FilePath = lsWordPath;
                loWord.Abrir(true);

                if (!GenerarReporteEstandar(loEmpleado, liCodReporte, loWord))
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord.Dispose();
                    loWord = null;
                    return;
                }

                ReemplazarMetaTags(loWord, loEmpleado, lbReemplazarMsgWeb);

                string lsFileName = getFileName(".docx");
                loWord.FilePath = lsFileName;
                loWord.SalvarComo();
                loWord.Cerrar();
                loWord.Salir();
                loWord.Dispose();
                loWord = null;

                #region reporte adjunto

                //Si no ha sido posible generar el reporte estándar configurado
                if (!GenerarReporteEstandarAdjunto(loEmpleado))
                {
                    return;
                }

                #endregion

                poMail = new MailAccess();
                poMail.NotificarSiHayError = false;
                poMail.IsHtml = true;

                poMail.De = getRemitente();
                poMail.Asunto = getAsunto(loEmpleado, liCodAsunto);
                poMail.AgregarWord(lsFileName);
                poMail.Para.Add(lsPara);
                poMail.CC.Add(lsCC);
                poMail.BCC.Add(lsCCO);



                //20150818.RJ
                if (lbEnviarArchivoAdjunto)
                {
                    //20130814 PT: Se agrega adjunto el reporte
                    //Se agregan como adjuntos cada uno de los documentos creados por el reporte estándar
                    foreach (string lsAdjunto in plstAdjuntos)
                    {
                        if (!string.IsNullOrEmpty(lsAdjunto))
                            poMail.Adjuntos.Add(new Attachment(lsAdjunto));
                    }
                }


                poMail.Enviar();

                if (!poMail.HayError)
                {
                    if (Bitacora != null)
                    {
                        Bitacora(loEmpleado, "");
                    }
                }
                else
                {
                    if (Bitacora != null)
                    {
                        string lsMsg = "No se pudo enviar el correo de Notificación de Presupuestos al empleado " + loEmpleado.iCodEmpleado + ": " + loEmpleado.vchDescripcion + ".";
                        Bitacora(loEmpleado, lsMsg);
                        Util.LogMessage(lsMsg);
                    }
                    else
                    {
                        Util.LogMessage("No se pudo enviar el correo al responsable del conmutador por la asignación de presupuesto al empleado " + loEmpleado.iCodEmpleado + ": " + loEmpleado.vchDescripcion + ".");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Bitacora != null)
                {
                    Util.LogException("Error al enviar el correo de Notificación de Presupuestos al empleado " + loEmpleado.iCodEmpleado + ": " + loEmpleado.vchDescripcion + ".", ex);
                }
                else
                {
                    Util.LogException("Error al enviar el correo al responsable del conmutador por la asignación de presupuesto al empleado " + loEmpleado.iCodEmpleado + ": " + loEmpleado.vchDescripcion + ".", ex);
                }
            }
            finally
            {
                if (loWord != null)
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord.Dispose();
                    loWord = null;
                }
            }
        }




        private void GuardarBitacora(EmpleadoPpto loEmpleado, string lsMsg)
        {
            Hashtable lhtValores = new Hashtable();
            lhtValores.Add("{Emple}", loEmpleado.iCodEmpleado);
            lhtValores.Add("{TipoPr}", loEmpleado.TipoPr);
            lhtValores.Add("{PeriodoPr}", loEmpleado.PeriodoPr);
            lhtValores.Add("{PrepEmple}", loEmpleado.PrepEmple);
            lhtValores.Add("{NivelAlerta}", loEmpleado.NivelAlerta);
            lhtValores.Add("{ValorPresupuesto}", loEmpleado.Presupuesto);
            lhtValores.Add("{ValorConsumo}", loEmpleado.Consumo);
            lhtValores.Add("{ValorConsumoBase}", loEmpleado.ValorConsumoBase);
            lhtValores.Add("{FechaInicioPrep}", loEmpleado.FechaInicioPrep);
            lhtValores.Add("{FechaNotificacionConsumo}", DateTime.Now);
            lhtValores.Add("dtFecUltAct", DateTime.Now);


            if (!string.IsNullOrEmpty(lsMsg))
            {
                lhtValores.Add("{Msg}", lsMsg);
            }

            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            cargasCOM.InsertaRegistro(lhtValores, "Detallados", "Detall", "Bitacora Notificacion Consumos", piCodUsuarioDB);

        }

        protected void ReemplazarMetaTags(WordAccess loWord, EmpleadoPpto loEmpleado, bool lbReemplazarMsgWeb)
        {
            if (lbReemplazarMsgWeb)
            {
                if (pbIncluirPresupuesto)
                {
                    string lsPresupuesto = getPresupuesto(loEmpleado);
                    if (!loWord.ReemplazarTexto("Param(Presupuesto)", lsPresupuesto))
                    {
                        loWord.NuevoParrafo();
                        loWord.InsertarTexto(lsPresupuesto);
                    }
                }
                if (pbIncluirConsumo)
                {
                    string lsConsumo = getConsumo(loEmpleado);
                    if (!loWord.ReemplazarTexto("Param(Consumo)", lsConsumo))
                    {
                        loWord.NuevoParrafo();
                        loWord.InsertarTexto(lsConsumo);
                    }
                }
                if (pbIncluirPorcentaje)
                {
                    string lsPorcentaje = getPorcentaje(loEmpleado);
                    if (!loWord.ReemplazarTexto("Param(Porcentaje)", lsPorcentaje))
                    {
                        loWord.NuevoParrafo();
                        loWord.InsertarTexto(lsPorcentaje);
                    }
                }
            }

            Hashtable lhtParamDesc = getHTParamDesc(loEmpleado);
            foreach (string lsKey in lhtParamDesc.Keys)
            {
                string lsMetaTag = string.Format("Param({0})", lsKey);
                string lsParam = lhtParamDesc[lsKey].ToString();
                lsParam = lsParam.Length > 255 ? lsParam.Substring(0, 255) : lsParam;
                loWord.ReemplazarTexto(lsMetaTag, lsParam);
            }
        }




        private string getPresupuesto(EmpleadoPpto loEmpleado)
        {
            /**********************************************************************************/

            //string lsElemento = (loEmpleado.PresupProv ? "PresupuestoProv" : "Presupuesto");
            //return UtilAlarma.GetLangItem(psLang, "MsgWeb", "Mensajes Web", lsElemento,
            //    loEmpleado.Presupuesto,
            //    UtilAlarma.GetLangItem(psLang, "TipoPr", "Tipo Presupuesto", loEmpleado.TipoPrCod));

            /**********************************************************************************/

            //20141121 AM. 
            //Se cambio el metodo para darle formato al presupuesto y evitar el #undefined-XXXXX# en el correo
            string lsElemento = (loEmpleado.PresupProv ? "PresupuestoProv" : "Presupuesto");
            string lsPresupuesto = loEmpleado.Presupuesto.ToString();
            switch (loEmpleado.TipoPrCod)
            {
                case "Costo":
                    lsPresupuesto = loEmpleado.Presupuesto.ToString("##0.00");
                    break;
                case "Duracion":
                    lsPresupuesto = loEmpleado.Presupuesto.ToString("0");
                    break;
                case "Llamadas":
                    lsPresupuesto = loEmpleado.Presupuesto.ToString("0");
                    break;
            }

            return UtilAlarma.GetLangItem(psLang, "MsgWeb", "Mensajes Web", lsElemento,
                lsPresupuesto,
                UtilAlarma.GetLangItem(psLang, "TipoPr", "Tipo Presupuesto", loEmpleado.TipoPrCod));

        }

        private string getConsumo(EmpleadoPpto loEmpleado)
        {
            string lsElemento = "Consumo";
            string lsConsumo = loEmpleado.Consumo.ToString();
            switch (loEmpleado.TipoPrCod)
            {
                case "Costo":
                    lsElemento = "ConsumoCosto";
                    lsConsumo = loEmpleado.Consumo.ToString("##0.00");
                    break;
                case "Duracion":
                    lsElemento = "ConsumoDuracion";
                    lsConsumo = loEmpleado.Consumo.ToString("0");
                    break;
                case "Llamadas":
                    lsElemento = "ConsumoLlamadas";
                    lsConsumo = loEmpleado.Consumo.ToString("0");
                    break;
            }
            return UtilAlarma.GetLangItem(psLang, "MsgWeb", "Mensajes Web", lsElemento,
                lsConsumo,
                UtilAlarma.GetLangItem(psLang, "TipoPr", "Tipo Presupuesto", loEmpleado.TipoPrCod));
        }

        private string getPorcentaje(EmpleadoPpto loEmpleado)
        {
            return UtilAlarma.GetLangItem(psLang, "MsgWeb", "Mensajes Web", "Porcentaje",
                loEmpleado.NivelAlerta);
        }

        protected string getAsunto(EmpleadoPpto loEmpleado, int liCodAsunto)
        {
            return UtilAlarma.getAsunto(loEmpleado, psLang, liCodAsunto, getHTParamDesc(loEmpleado));
        }

        private string getDestinatarios(EmpleadoPpto loEmpleado)
        {
            return loEmpleado.Email;
        }

        protected MailAddress getRemitente()
        {
            return new MailAddress(Util.AppSettings("appeMailID"));
        }

        private string getFileName(string lsExt)
        {
            string lsFileName;
            System.IO.Directory.CreateDirectory(psTempPath);
            lsFileName = Guid.NewGuid().ToString();
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(psTempPath, lsFileName + lsExt));
        }
        //20130814 PT: se agrego este metodo para el reporte adjunto 
        private string getFileName(EmpleadoPpto loEmpleado, string lsExt)
        {
            string lsFileName;

            System.IO.Directory.CreateDirectory(psTempPath);
            if (!string.IsNullOrEmpty(loEmpleado.vchCodigo))
            {
                lsFileName = loEmpleado.vchCodigo.Trim();
            }
            else
            {
                lsFileName = Guid.NewGuid().ToString();
            }
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(psTempPath, lsFileName + lsExt));
        }

        #region Notificación al responsable de sitio por presupuesto temporal
        public void getNivelConsumo(EmpleadoPpto loEmpleado)
        {
            //Obtiene el importe total de consumo que ha generado el empleado a partir de las fechas recibidas como parametros
            //Los importes son tanto en pesos, en minutos y en llamadas
            loEmpleado.getConsumo(pdtFechaInicioPrep, pdtFechaFinPrep);

            if (loEmpleado.Consumo >= loEmpleado.Presupuesto)
            {
                agregarPresupCC(loEmpleado);
                buscarPresupProv(loEmpleado);
            }
            //Se valida si el Empleado rebasó el presupuesto en base a su consumo,
            //de ser así se revisa si dicho Empleado tiene configurado un presupuesto temporal.
            //Si el Empleado tiene un presupuesto temporal, se vuelve a calcular si su consumo actual excede
            //el consumo que tenia cuando se le asignó su presupuesto temporal. 
            if (loEmpleado.Consumo >= loEmpleado.Presupuesto)
            {
                agregarBitacora100(loEmpleado);
            }
            loEmpleado.NivelConsumo = loEmpleado.Consumo / loEmpleado.Presupuesto * 100; //Porcentaje consumido del empleado
        }

        private void agregarBitacora100(EmpleadoPpto loEmpleado)
        {
            //Se revisa si hay registros en la vista 'Detall','Bitacora Consumo 100%'
            //que correspondan al empleado en curso y del periodo que se está procesando
            //en donde el consumo actual sea mayor al consumo que se tuvo en el momento en que se agregó el registro 
            //en dicha vista
            DataTable ldtBitacora = DSODataAccess.Execute(
                "Select iCodRegistro " +
                " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')]" +
                " where Emple = " + loEmpleado.iCodEmpleado +
                " and PeriodoPr = " + loEmpleado.PeriodoPr +
                " and TipoPr = " + loEmpleado.TipoPr +
                " and PrepEmple = " + loEmpleado.PrepEmple +
                " and Abs(ValorPresupuesto - " + loEmpleado.Presupuesto + ") < 0.1" +
                " and Abs(ValorConsumo - " + loEmpleado.Consumo + ") > 0.1" +
                " and Abs(ValorConsumoBase - " + loEmpleado.ValorConsumoBase + ") > 0.1" +
                " and FechaInicioPrep = '" + loEmpleado.FechaInicioPrep.ToString("yyyy-MM-dd") + "'" +
                " and FechaReset is null");

            //En caso de encontrarse registros en el paso anterior, se agrega un nuevo registro en el listado 
            //de empleados a los que se enviará un nuevo aviso
            if (ldtBitacora.Rows.Count > 0)
            {
                phsEmpleadosReintentoBaja.Add(loEmpleado.iCodEmpleado);
            }


            //Se revisa si hay registros en la vista 'Detall','Bitacora Consumo 100%'
            //que correspondan al empleado en curso, del periodo que se está procesando
            //y en donde el consumo actual sea menor o igual al consumo que se tuvo 
            //en el momento en que se agregó el registro en dicha vista.
            ldtBitacora = DSODataAccess.Execute(
                "Select iCodRegistro " +
                " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')]" +
                " where Emple = " + loEmpleado.iCodEmpleado +
                " and PeriodoPr = " + loEmpleado.PeriodoPr +
                " and TipoPr = " + loEmpleado.TipoPr +
                " and PrepEmple = " + loEmpleado.PrepEmple +
                " and Abs(ValorPresupuesto - " + loEmpleado.Presupuesto + ") < 0.1" +
                " and Abs(ValorConsumo - " + loEmpleado.Consumo + ") < 0.1" +
                " and Abs(ValorConsumoBase - " + loEmpleado.ValorConsumoBase + ") < 0.1" +
                " and FechaInicioPrep = '" + loEmpleado.FechaInicioPrep.ToString("yyyy-MM-dd") + "'" +
                " and FechaReset is null");

            //Si no se encuentran registros en el paso anterior, querrá decir que el empleado sigue generando gasto,
            //se agrega un registro en la vista 'Detall','Bitacora Consumo 100%' para que el proceso que genera los archivos
            //lo tome en cuenta y se vuelva a tomar en cuenta sus códigos para ser bloqueados
            if (ldtBitacora != null && ldtBitacora.Rows.Count == 0)
            {
                Hashtable lhtValores = new Hashtable();
                lhtValores.Add("{Emple}", loEmpleado.iCodEmpleado);
                lhtValores.Add("{TipoPr}", loEmpleado.TipoPr);
                lhtValores.Add("{PeriodoPr}", loEmpleado.PeriodoPr);
                lhtValores.Add("{PrepEmple}", loEmpleado.PrepEmple);
                lhtValores.Add("{ValorPresupuesto}", loEmpleado.Presupuesto);
                lhtValores.Add("{ValorConsumo}", loEmpleado.Consumo);
                lhtValores.Add("{ValorConsumoBase}", loEmpleado.ValorConsumoBase);
                lhtValores.Add("{FechaInicioPrep}", loEmpleado.FechaInicioPrep);
                lhtValores.Add("{FechaNotificacionConsumo}", DateTime.Now);
                lhtValores.Add("dtFecUltAct", DateTime.Now);

                KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
                cargasCOM.InsertaRegistro(lhtValores, "Detallados", "Detall", "Bitacora Consumo 100%", piCodUsuarioDB);
            }
        }

        private void agregarPresupCC(EmpleadoPpto loEmpleado)
        {
            //Revisa si el Centro de costos del empleado en cuestión tiene configurado un presupuesto temporal
            //para el periodo que se está procesando.
            DataTable ldtPrepCenCos = DSODataAccess.Execute(
                "select iCodRegistro, iCodCatalogo, PresupProv, dtIniVigencia, dtFinVigencia " +
                " from [" + DSODataContext.Schema + "].[VisHistoricos('PrepCenCos','Presupuesto Temporal','Español')]" +
                " where TipoPr = " + piCodTipoPr +
                " and PeriodoPr = " + piCodPeriodoPr +
                " and dtIniVigencia <> dtFinVigencia" +
                " and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and CenCos = " + loEmpleado.iCodCenCos +
                " order by dtIniVigencia, iCodRegistro");

            //Por cada registro encontrado en el paso anterior
            foreach (DataRow ldrPrepCenCos in ldtPrepCenCos.Rows)
            {
                int liCodPrepCenCos = (int)ldrPrepCenCos["iCodCatalogo"];

                //Revisa si el empleado en cuestión tiene configurado un presupuesto temporal que aplique para
                //el periodo que se está procesando.
                DataTable ldtPrepEmple = DSODataAccess.Execute(
                    "select iCodRegistro " +
                    " from [" + DSODataContext.Schema + "].[VisHistoricos('PrepEmple','Presupuesto Temporal','Español')]" +
                    " where Emple = " + loEmpleado.iCodEmpleado +
                    " and TipoPr = " + piCodTipoPr +
                    " and PeriodoPr = " + piCodPeriodoPr +
                    " and dtIniVigencia <> dtFinVigencia" +
                    " and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and PrepCenCos = " + liCodPrepCenCos);



                //Si sólo se encuentra un presupuesto temporal por Centro de costos, 
                //no encontrándose un presupuesto temporal para el empleado en curso
                //se inserta un registro en la vista 'PrepEmple', 'Presupuesto Temporal' con el presupuesto del CC
                //que aplicará para el empleado en curso
                if (loEmpleado.iCodCenCos > 0 && ldtPrepEmple.Rows.Count == 0)
                {
                    //insertar presupuesto temporal del centro de costos
                    loEmpleado.PresupProv = true;
                    loEmpleado.ValorConsumoBase = loEmpleado.Consumo;


                    Hashtable lhtValores = new Hashtable();
                    lhtValores.Add("vchDescripcion", loEmpleado.vchDescripcion + "");
                    lhtValores.Add("{PrepCenCos}", liCodPrepCenCos);
                    lhtValores.Add("{Emple}", loEmpleado.iCodEmpleado);
                    lhtValores.Add("{TipoPr}", piCodTipoPr);
                    lhtValores.Add("{PeriodoPr}", piCodPeriodoPr);
                    lhtValores.Add("{PresupProv}", ldrPrepCenCos["PresupProv"]);
                    lhtValores.Add("{ValorConsumoBase}", loEmpleado.ValorConsumoBase);
                    lhtValores.Add("{FechaInicioPrep}", loEmpleado.FechaInicioPrep);
                    lhtValores.Add("dtIniVigencia", DateTime.Today);
                    lhtValores.Add("dtFinVigencia", pdtFechaFinPrep);
                    lhtValores.Add("dtFecUltAct", DateTime.Now);

                    KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
                    cargasCOM.InsertaRegistro(lhtValores, "Historicos", "PrepEmple", "Presupuesto Temporal", piCodUsuarioDB);

                    //enviarNotificacionPresupProv(loEmpleado);
                    break;
                }
            }
        }

        private void enviarNotificacionReintentarBaja()
        {
            string lsEmail;
            string lsPlantilla;
            int liCodRepEst;
            int liCodAsunto;

            DataTable ldtSitios = DSODataAccess.Execute(
                "Select distinct Sitio " +
                " from   [" + DSODataContext.Schema + "].[VisDetallados('Detall','DetalleCDR','Español')] " +
                " where not Sitio is Null" +
                " and Emple in (" + getEmpleadosReintentoBaja() + ")");
            foreach (DataRow ldrSitio in ldtSitios.Rows)
            {
                int liCodSitio = (int)ldrSitio["Sitio"];
                EmpleadoPpto loEmpleado = getDatosSitio(liCodSitio, out lsEmail, out liCodAsunto, out lsPlantilla, out liCodRepEst);
                loEmpleado.iCodSitio = liCodSitio;
                EnviarCorreo(loEmpleado, lsPlantilla, liCodRepEst, liCodAsunto, lsEmail, "", "", false, false, null);
            }
        }

        private string getEmpleadosReintentoBaja()
        {
            StringBuilder lsEmpleados = new StringBuilder("0");

            foreach (int liCodEmple in phsEmpleadosReintentoBaja)
            {
                lsEmpleados.Append(",");
                lsEmpleados.Append(liCodEmple.ToString());
            }
            return lsEmpleados.ToString();
        }

        private EmpleadoPpto getDatosSitio(int liCodSitio, out string lsEmail, out int liCodAsunto, out string lsPlantilla, out int liCodRepEst)
        {
            EmpleadoPpto loEmpleado = null;
            int liBanderasEmpre = 0;
            int liValorBandera;
            lsEmail = "";
            lsPlantilla = "";
            liCodAsunto = 0;
            liCodRepEst = 0;

            liValorBandera = (int)DSODataAccess.ExecuteScalar(
                "select IsNull(Value, 0) from [" + DSODataContext.Schema + "].[VisHistoricos('Valores','Valores','Español')]" +
                " where vchCodigo = 'ActivarAltaBajaCodExtPrep'" +
                " and dtIniVigencia <> dtFinVigencia" +
                " and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'", (object)0);
            liBanderasEmpre = (int)DSODataAccess.ExecuteScalar(
                "Select IsNull(BanderasEmpre, 0) from [" + DSODataContext.Schema + "].[VisHistoricos('Empre','Empresas','Español')] " +
                " where iCodCatalogo = " + piCodEmpre +
                " and dtIniVigencia <> dtFinVigencia" +
                " and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'", (object)0);
            if ((liBanderasEmpre & liValorBandera) == liValorBandera) //Proceso activo
            {
                DataTable ldtSitio = DSODataAccess.Execute(
                    "Select PrepSitio.Asunto, PrepSitio.RepEst, PrepSitio.Plantilla, PrepSitio.CtaPara " +
                    " from  [" + DSODataContext.Schema + "].[VisHistoricos('PrepSitio','Notificación recursos dados de baja','Español')] PrepSitio," +
                    "       [" + DSODataContext.Schema + "].[VisHistoricos('NotifPrepSitio','Notificaciones de Presupuestos para Sitios','Español')] NotifPrepSitio" +
                    " where	NotifPrepSitio.Sitio = " + liCodSitio +
                    " and   PrepSitio.NotifPrepSitio = NotifPrepSitio.iCodCatalogo" +
                    " and   PrepSitio.dtIniVigencia <> PrepSitio.dtFinVigencia" +
                    " and   PrepSitio.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and   PrepSitio.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and   NotifPrepSitio.dtIniVigencia <> NotifPrepSitio.dtFinVigencia" +
                    " and   NotifPrepSitio.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and   NotifPrepSitio.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                if (ldtSitio.Rows.Count > 0)
                {
                    liCodAsunto = (int)Util.IsDBNull(ldtSitio.Rows[0]["Asunto"], 0);
                    liCodRepEst = (int)Util.IsDBNull(ldtSitio.Rows[0]["RepEst"], 0);
                    lsPlantilla = Util.IsDBNull(ldtSitio.Rows[0]["Plantilla"], "").ToString();
                    lsPlantilla = UtilAlarma.buscarPlantilla(lsPlantilla, psLang);
                    lsEmail = Util.IsDBNull(ldtSitio.Rows[0]["CtaPara"], "").ToString();
                    DataTable ldtEmpleSitio = DSODataAccess.Execute(
                        "Select Sitio.Emple, Emple.Email " +
                        " from [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')] Emple," +
                        "      [" + DSODataContext.Schema + "].[VisHisComun('Sitio','Español')] Sitio" +
                        " where Sitio.iCodCatalogo = " + liCodSitio +
                        " and   Sitio.Emple = Emple.iCodCatalogo" +
                        " and   Emple.dtIniVigencia <> Emple.dtFinVigencia" +
                        " and   Emple.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                        " and   Emple.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                        " and   Sitio.dtIniVigencia <> Sitio.dtFinVigencia" +
                        " and   Sitio.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                        " and   Sitio.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    if (ldtEmpleSitio.Rows.Count > 0)
                    {
                        if (string.IsNullOrEmpty(lsEmail))
                        {
                            lsEmail = Util.IsDBNull(ldtEmpleSitio.Rows[0]["Email"], "").ToString();
                        }
                        if (!(ldtEmpleSitio.Rows[0]["Emple"] is DBNull))
                        {
                            loEmpleado = new EmpleadoPpto((int)ldtEmpleSitio.Rows[0]["Emple"], false);
                        }
                    }
                    if (loEmpleado == null)
                    {
                        int liCodUsuar = (int)DSODataAccess.ExecuteScalar(
                            "Select IsNull(Usuar, 0)" +
                            " from [" + DSODataContext.Schema + "].[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','Español')]" +
                            " where Empre = " + piCodEmpre +
                            " and dtIniVigencia <> dtFinVigencia" +
                            " and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                            " and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'", (object)0);

                        loEmpleado = new EmpleadoPpto(liCodUsuar);
                    }
                }
            }
            return loEmpleado;
        }

        private void buscarPresupProv(EmpleadoPpto loEmpleado)
        {
            //Obtiene los valores que pudiera tener configurado el empleado en curso, los datos que obtiene son:
            //valorConsumoBase que se refiere al consumo que tenía el empleado en el momento en que fue registrado el presupuesto temporal
            //PresupProv que se refiere al presupuesto temporal que le fue asignado.
            DataTable ldtPresupProv = getPresupProv(loEmpleado);

            //Si se encuentra un presupuesto temporal configurado para el periodo en curso, 
            //se calcula el importe del presupuesto que deberá aplicar, en base al consumo que tenga actualmente
            if (ldtPresupProv.Rows.Count > 0)
            {
                loEmpleado.ValorConsumoBase = (double)ldtPresupProv.Rows[0]["ValorConsumoBase"];
                loEmpleado.Presupuesto = (double)ldtPresupProv.Rows[0]["PresupProv"];
                loEmpleado.Consumo -= loEmpleado.ValorConsumoBase;

            }
        }

        protected DataTable getPresupProv(EmpleadoPpto loEmpleado)
        {
            //Obtiene los valores que pudiera tener configurado el empleado en curso, los datos que obtiene son:
            //valorConsumoBase que se refiere al consumo que tenía el empleado en el momento en que fue registrado el presupuesto temporal
            //PresupProv que se refiere al presupuesto temporal que le fue asignado.
            return DSODataAccess.Execute(
                "select top 1 iCodRegistro, ValorConsumoBase, PresupProv, dtIniVigencia" +
                " from [" + DSODataContext.Schema + "].[VisHistoricos('PrepEmple','Presupuesto Temporal','Español')]" +
                " where Emple = " + loEmpleado.iCodEmpleado +
                " and TipoPr = " + piCodTipoPr +
                " and PeriodoPr = " + piCodPeriodoPr +
                " and dtIniVigencia <> dtFinVigencia" +
                " and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " order by dtIniVigencia desc, iCodRegistro desc");
        }

        public static void getFechasPeriodo(int liDiaInicioPeriodo, int liCodPeriodoPr, out DateTime ldtFechaInicioPrep, out DateTime ldtFechaFinPrep)
        {
            ldtFechaInicioPrep = DateTime.Today;
            ldtFechaFinPrep = DateTime.Today.AddDays(1);

            //Obtiene el vchcodigo del tipo de presupuesto, a partir del valor que recibe como parámetro (liCodPeriodoPr)
            //pudiéndo ser: Diario, Semanal, Mensual
            string lvchPeriodoPrCod = DSODataAccess.ExecuteScalar(
                "Select vchCodigo " +
                " from [" + DSODataContext.Schema + "].[VisHistoricos('PeriodoPr','Periodo Presupuesto','Español')]" +
                " where iCodCatalogo = " + liCodPeriodoPr +
                " and   dtIniVigencia <> dtFinVigencia" +
                " and   dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and   dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'").ToString();

            //Dependiendo del valor encontrado en el paso anterior, calcula el valor de las fechas inicio y fin
            //asignándo éste a las variables ldtFechaInicioPrep y ldtFechaFinPrep respectivamente
            switch (lvchPeriodoPrCod)
            {
                case "Semanal":
                    liDiaInicioPeriodo = liDiaInicioPeriodo % 7;
                    ldtFechaInicioPrep = DateTime.Today.AddDays((int)DateTime.Today.DayOfWeek * -1).AddDays(liDiaInicioPeriodo - 1);
                    if (ldtFechaInicioPrep > DateTime.Today)
                    {
                        ldtFechaInicioPrep = ldtFechaInicioPrep.AddDays(-7);
                    }
                    ldtFechaFinPrep = ldtFechaInicioPrep.AddDays(7);
                    break;
                case "Mensual":
                    DateTime ldtMes;

                    //Cuando se trate de un periodo mensual y el día inicio configurado en la empresa
                    //sea menor a 1 o mayor a 31, se tomará como día inicio de periodo el día 1
                    if (liDiaInicioPeriodo < 1 || liDiaInicioPeriodo > 31)
                    {
                        liDiaInicioPeriodo = 1;
                    }

                    //Si el día de inicio de cada periodo configurado en la empresa, es menor o igual al día actual
                    //se tomará el mes actual como el mes que se debe tomar para calcular los consumos
                    if (liDiaInicioPeriodo <= DateTime.Today.Day)
                    {
                        ldtMes = DateTime.Today;
                    }
                    else
                    {
                        //Si el día de inicio de cada periodo configurado en la empresa, es mayor al día actual
                        //quiere decir que se ha configurado un día diferente al día 1 del mes, por lo tanto
                        //se debe calcular las fechas en base al número de días transucurridos desde el último fin de periodo
                        ldtMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                        liDiaInicioPeriodo = Math.Min(liDiaInicioPeriodo, DateTime.DaysInMonth(ldtMes.Year, ldtMes.Month));
                    }

                    //Se calculan las fechas inicio y fin que se tomarán para calcular los consumos, en base
                    //a la fecha encontrada en las condiciones anteriores
                    ldtFechaInicioPrep = new DateTime(ldtMes.Year, ldtMes.Month, liDiaInicioPeriodo); //Fecha completa del día de inicio de periodo
                    ldtFechaFinPrep = ldtFechaInicioPrep.AddMonths(1); //Fecha completa 1 mes después del día de inicio de periodo
                    break;

                default:
                    ldtFechaInicioPrep = DateTime.Today;
                    ldtFechaFinPrep = DateTime.Today.AddDays(1);
                    break;
            }
        }

        #endregion

        #region ReporteEstandar
        protected bool GenerarReporteEstandar(EmpleadoPpto loEmpleado, int liCodReporte, WordAccess loWord)
        {
            bool lbRet = true;
            if (liCodReporte > 0)
            {
                try
                {
                    ReporteEstandarUtil lReporteEstandarUtil = null;
                    lReporteEstandarUtil = GetReporteEstandarUtil(loEmpleado, liCodReporte);
                    lReporteEstandarUtil.ExportDOC(loWord);
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al crear el reporte estándar.\r\n Empleado: " + loEmpleado.iCodEmpleado + "\r\n Usuario: " + loEmpleado.iCodUsuario, ex);
                    lbRet = false;



















































                }









            }
            return lbRet;
        }

        //20130814 PT: Genera reporte estandar que ira adjunto
        protected bool GenerarReporteEstandarAdjunto(EmpleadoPpto loEmpleado)
        {
            //Inicializa la variable booleana lbRet igual a true
            //Esta variable indicará si el reporte fue creado correctamente o no.
            bool lbRet = true;


            string query = "select iCodCatalogo from [VisHistoricos('RepEst','Tabular','Español')]" +
                           "where vchCodigo = 'RepTabConsumoDetalladoEnvio' and dtIniVigencia<>dtFinVigencia and dtFinVigencia>=GETDATE()";

            int liCodRepEst = (int)DSODataAccess.ExecuteScalar(query);


            if (liCodRepEst > 0)
            {

                //Se inicializan las variables lsReportePath, lExcel, lWord y lTxt
                string lsReportePath = "";
                ExcelAccess lExcel = null;

                try
                {
                    //Se crea un objeto del tipo ReporteEstandarUtil
                    ReporteEstandarUtil lReporteEstandarUtil = null;

                    lReporteEstandarUtil = GetReporteEstandarUtil(loEmpleado, liCodRepEst);
                    lsReportePath = getFileName(loEmpleado, ".xlsx");

                    lExcel = lReporteEstandarUtil.ExportXLS();
                    lExcel.FilePath = lsReportePath;
                    lExcel.SalvarComo();
                    lExcel.Cerrar(true);
                    lExcel.Dispose();

                    plstAdjuntos.Add(lsReportePath);

                }
                catch (Exception ex)
                {
                    string lsLogMsg;
                    lsLogMsg = "Error al crear el reporte estándar. Empleado: " + loEmpleado.iCodEmpleado + "\r\n" + Util.ExceptionText(ex);

                    Util.LogException(lsLogMsg, ex);
                    lbRet = false;

                }
                finally
                {
                    if (lExcel != null)
                    {
                        lExcel.Cerrar(true);
                        lExcel.Dispose();
                        lExcel = null;
                    }
                }
            }
            return lbRet;
        }

        protected ReporteEstandarUtil GetReporteEstandarUtil(EmpleadoPpto loEmpleado, int liCodReporte)
        {
            Hashtable lHTParam;
            Hashtable lHTParamDesc;
            string psStylePath = "";
            string psKeytiaWebFPath = Util.AppSettings("KeytiaWebFPath");
            DataRow ldrCte = UtilAlarma.getCliente(loEmpleado);

            if (string.IsNullOrEmpty(psKeytiaWebFPath))
            {
                Exception ex = new Exception("No se pudo obtener el path de la aplicación web de KeytiaV (App.config key: KeytiaWebFPath).");
                Util.LogException(ex);
                throw ex;
            }

            if (ldrCte == null)
            {
                Exception ex = new Exception("No se pudo obtener el cliente del empleado " + loEmpleado.vchCodigo);
                Util.LogException(ex);
                throw ex;
            }
            if (!(ldrCte["{StyleSheet}"] is DBNull))
            {
                psStylePath = System.IO.Path.Combine(psKeytiaWebFPath, ldrCte["{StyleSheet}"].ToString().Replace("~/", "").Replace("/", "\\"));
            }
            else
            {
                Exception ex = new Exception("No se pudo obtener la hoja de estilos del empleado " + loEmpleado.vchCodigo);
                Util.LogException(ex);
                throw ex;
            }
            lHTParam = getHTParam(loEmpleado);
            lHTParamDesc = getHTParamDesc(loEmpleado);
            return new ReporteEstandarUtil(liCodReporte, lHTParam, lHTParamDesc, psKeytiaWebFPath, psStylePath);
        }

        private Hashtable getHTParam(EmpleadoPpto loEmpleado)
        {
            //TODO: Validar que el empleado tenga usuario, o utilizar usuario configurado en la empresa
            Hashtable pHTParam = new Hashtable();
            pHTParam = new Hashtable();
            pHTParam.Add("iCodUsuario", loEmpleado.iCodUsuario);
            pHTParam.Add("iCodPerfil", loEmpleado.iCodPerfil);
            pHTParam.Add("vchCodIdioma", psLang);
            pHTParam.Add("vchCodMoneda", getCurrency());
            pHTParam.Add("Schema", DSODataContext.Schema);
            pHTParam.Add("FechaIniRep", "'" + pdtFechaInicioPrep.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            //20130904.PT: se cambio la fecha fin del reporte a ser el dia actual en lugar de fecha fin de presupuesto
            pHTParam.Add("FechaFinRep", "'" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            pHTParam.Add("Emple", (loEmpleado.iCodEmpleado > 0 ? loEmpleado.iCodEmpleado.ToString() : "null"));
            pHTParam.Add("Fecha", "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            pHTParam.Add("Nivel", loEmpleado.NivelAlerta);
            pHTParam.Add("Empre", piCodEmpre);
            pHTParam.Add("CenCos", (loEmpleado.iCodCenCos > 0 ? loEmpleado.iCodCenCos.ToString() : "null"));
            pHTParam.Add("Sitio", (loEmpleado.iCodSitio > 0 ? loEmpleado.iCodSitio.ToString() : "null"));
            return pHTParam;
        }

        private Hashtable getHTParamDesc(EmpleadoPpto loEmpleado)
        {
            Hashtable lHTParamDesc = new Hashtable();
            string lsDateFormat = UtilAlarma.GetLangItem(psLang, "MsgWeb", "Mensajes Web", "NetDateFormat");
            UtilAlarma.AddNotNullValue(lHTParamDesc, "FechaIniRep", pdtFechaInicioPrep.ToString(lsDateFormat));
            //20130904.PT: se cambio la fecha fin del reporte a ser el dia actual en lugar de fecha fin de presupuesto
            UtilAlarma.AddNotNullValue(lHTParamDesc, "FechaFinRep", DateTime.Today.ToString(lsDateFormat));

            UtilAlarma.AddNotNullValue(lHTParamDesc, "Emple", loEmpleado.vchDescripcion);
            UtilAlarma.AddNotNullValue(lHTParamDesc, "Fecha", DateTime.Now.ToString(lsDateFormat));
            UtilAlarma.AddNotNullValue(lHTParamDesc, "Nivel", loEmpleado.NivelAlerta.ToString());
            UtilAlarma.AddNotNullValue(lHTParamDesc, "Empre", getDescripcion("Empre", piCodEmpre, psLang));
            UtilAlarma.AddNotNullValue(lHTParamDesc, "iCodPerfil", getDescripcion("Perfil", loEmpleado.iCodPerfil, psLang));
            UtilAlarma.AddNotNullValue(lHTParamDesc, "CenCos", getDescripcion("CenCos", loEmpleado.iCodCenCos, psLang));
            UtilAlarma.AddNotNullValue(lHTParamDesc, "Sitio", getDescripcion("Sitio", loEmpleado.iCodSitio, psLang));

            UtilAlarma.AddNotNullValue(lHTParamDesc, "Presupuesto", loEmpleado.Presupuesto.ToString());
            return lHTParamDesc;
        }

        protected string getDescripcion(string lsEntidad, int liCodCatalogo, string lsLang)
        {
            string lsDescripcion = "";
            StringBuilder psbQuery = new StringBuilder();
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('" + lsEntidad + "','" + lsLang + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo);
            psbQuery.AppendLine("and   dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and   dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and   dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt.Rows.Count > 0)
            {
                DataRow lRow = ldt.Rows[0];
                if (ldt.Columns.Contains(lsLang) && lRow[lsLang] != DBNull.Value)
                {
                    lsDescripcion = lRow[lsLang].ToString();
                }
                else
                {
                    lsDescripcion = lRow["vchDescripcion"].ToString();
                }
            }
            return lsDescripcion;
        }

        protected string getCurrency()
        {
            string lsRet;

            if (Util.AppSettings("DefaultCurrency") != "")
                lsRet = Util.AppSettings("DefaultCurrency");
            else
                lsRet = "MXP";

            return lsRet;
        }
        #endregion

    }
}

