using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;

namespace KeytiaServiceBL
{
    public enum TipoBloqueo
    {
        Codigos = 1,
        Extensiones = 2
    }

    public enum TipoArchivo
    {
        Alta,
        Baja,
        //RZ.20140214 Nuevo Tipo de Archivo
        AltaPrepProv
    }

    public class PresupuestosSitios
    {
        #region Propiedades

        protected int piCodUsuarioDB = -1;
        public int iCodUsuarioDB
        {
            get { return piCodUsuarioDB; }
            set { piCodUsuarioDB = value; }
        }

        protected int piCodSitio;
        public int Sitio
        {
            get { return piCodSitio; }
            set { piCodSitio = value; }
        }

        //RZ.20140217 Esta propiedad ha quedado obsoleta ya que se usaba para extraer solo los sitios de la carga de CDR
        //Ahora se traen todos aquellos sitios.
        protected int piCodCarga;
        public int iCodCarga
        {
            get { return piCodCarga; }
            set { piCodCarga = value; }
        }

        protected int piDiaInicioPeriodo;
        public int DiaInicioPeriodo
        {
            get { return piDiaInicioPeriodo; }
            set { piDiaInicioPeriodo = value; }
        }

        protected int piCodPeriodoPr;
        public int PeriodoPr
        {
            get { return piCodPeriodoPr; }
            set { piCodPeriodoPr = value; }
        }

        //RZ.20140213 Se agrega nueva propiedad int para guardar el icodcatalogo de la empresa en curso
        protected int piCodEmpre;
        public int Empre
        {
            get { return piCodEmpre; }
            set { piCodEmpre = value; }
        }

        #endregion


        /// <summary>
        /// Obtiene un listado de los sitios cuya empresa tenga configurada la baja de recursos
        /// </summary>
        /// <returns></returns>
        protected DataTable getSitios()
        {

            //20130208.RJ. El query original tomaba en cuenta sólo aquellos sitios que estuvieran configurados para aplicar bajas y altas
            //y que estuvieran en la carga de CDR que se está procesando. Le quité esta ultima condición debido a que se deben
            //bloquear todos los códigos que tenga el empleado, independientemente de que el sitio de éstos se encuentre en la carga o no
            return DSODataAccess.Execute(
                "Select Sitio.iCodCatalogo" +
                " from [" + DSODataContext.Schema + "].[VisHisComun('Sitio','Español')] Sitio," +
                "      [" + DSODataContext.Schema + "].[VisHistoricos('Empre','Empresas','Español')] Empre" +
                " where /*Sitio.iCodCatalogo in (" +
                " 	select distinct Sitio from [" + DSODataContext.Schema + "].[VisDetallados('Detall','DetalleCDR','Español')] Detall" +
                " 	where Detall.iCodCatalogo = " + piCodCarga + ")" +
                " and*/ Sitio.Empre = Empre.iCodCatalogo" +
                " and Empre.BanderasEmpre & 2 = 2" +
                " and Sitio.dtIniVigencia <> Sitio.dtFinVigencia" +
                " and Sitio.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and Sitio.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and Empre.dtIniVigencia <> Empre.dtFinVigencia" +
                " and Empre.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " and Empre.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
        }


        public void procesarAltasInicioPeriodo()
        {
            DateTime ldtFechaInicioPrep;
            DateTime ldtFechaFinPrep;


            //Obtiene las fechas inicio y fin del periodo en curso
            NotificacionPresupuestos.getFechasPeriodo(piDiaInicioPeriodo, piCodPeriodoPr, out ldtFechaInicioPrep, out ldtFechaFinPrep);


            //Valida si el día actual es igual al día configurado para dar de Alta los recursos
            //y además que no existan registros en la vista [VisDetallados('Detall','Bitacora Alta Códigos-Extensiones','Español')]
            //para el periodo actual, pues de haberlos querría decir que ya se corrieron las altas
            //RZ.20140213 Se agrega en el query filtro para que solo cuente los registros de de aquellos sitios que tengan la empresa establecida en la propiedad Empree
            if (DateTime.Today == ldtFechaInicioPrep &&
                (int)DSODataAccess.ExecuteScalar(
                    "Select COUNT(BitacoraAlta.iCodRegistro) " +
                    " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Alta Códigos-Extensiones','Español')] BitacoraAlta," +
                    "      [" + DSODataContext.Schema + "].[VisHistoricos('PrepEmple','Presupuesto Fijo','Español')] PrepEmple, " +
                    "      [" + DSODataContext.Schema + "].[VisHisComun('Sitio','Español')] Sitio " +
                    " where BitacoraAlta.FechaInicioPrep = '" + ldtFechaInicioPrep.ToString("yyyy-MM-dd") + "'" +
                    " and   BitacoraAlta.PrepEmple = PrepEmple.iCodCatalogo" +
                    " and   PrepEmple.dtIniVigencia <> PrepEmple.dtFinVigencia" +
                    " and   PrepEmple.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and   PrepEmple.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and BitacoraAlta.Sitio =  Sitio.iCodCatalogo" +
                    " and Sitio.Empre = " + Empre.ToString() +
                    " and Sitio.dtIniVigencia <> Sitio.dtFinVigencia " +
                    " and Sitio.dtFinVigencia >=  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'", (object)0) == 0)
            {


                //Obtiene la fecha del periodo actual, tomando la fecha máxima del campo FechaInicioPrep
                //de la vista [VisDetallados('Detall','Bitacora Consumo 100%','Español')]
                DateTime ldtFechaInicioPrepAnt = (DateTime)DSODataAccess.ExecuteScalar(
                    "Select IsNull(Max(FechaInicioPrep), GETDATE()) " +
                    " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')]" +
                    " where FechaReset is null", DateTime.Now);


                //Obtiene el listado de registros que coincidan con la fecha de periodo actual
                //y cuyo valor del campo BanderasBitacora100 sea igual a 1, que quiere decir que excedieron del 100%
                /* 20130808 PT: Se modifico el query para que desde aqui ya no trajera empleados duplicados
                 y evitar el ciclo que elimina duplicados*/
                //RZ.20140210 En el select se habia omitio el campo ValorConsumo, que es necesario en el insert de la Bitacora Alta Códigos-Extensiones
                //RZ.20140303 Se retira distinct y order by, ya que no estaba quitando empleados duplicados, ni tampoco se requiere un orden para los inserts de empleados
                DataTable ldtBitacoraEmple100 = DSODataAccess.Execute(
                    " Select Emple, TipoPr, PeriodoPr, PrepEmple, ValorPresupuesto =  MAX(ValorPresupuesto), ValorConsumo = MAX(ValorConsumo), FechaInicioPrep " +
                    " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')] Bitacora100" +
                    " where FechaInicioPrep = '" + ldtFechaInicioPrepAnt.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and IsNull(BanderasBitacora100, 0) = 1" +
                    " and FechaReset is null" +
                    " group by Emple, TipoPr, PeriodoPr, PrepEmple, FechaInicioPrep ");

                #region comments
                /* 20130808 PT: este ciclo ya no es necesario 
                //Ciclo para recorrer todos los registros encontrados en el paso anterior y
                //eliminar empleados duplicados
                int i = 1;
                while (i < ldtBitacoraEmple100.Rows.Count)
                {
                    if (ldtBitacoraEmple100.Rows[i]["Emple"] == ldtBitacoraEmple100.Rows[i - 1]["Emple"])
                    {
                        ldtBitacoraEmple100.Rows.Remove(ldtBitacoraEmple100.Rows[i]);
                    }
                    else
                    {
                        i++;
                    }
                }*/
                #endregion

                //Obtiene un listado de los sitios cuya empresa tenga configurada la baja de recursos
                DataTable ldtSitios = getSitios();

                
                //Ciclo que recorre cada uno de los sitios encontrados en el paso anterior
                foreach (DataRow ldrSitio in ldtSitios.Rows)
                {
                    //Se crea una instancia del objeto ArchivosSitio, 
                    //al hacerlo se obtienen los valores de configuracion del sitio en cuestión
                    ArchivosSitio loArchivosSitios = new ArchivosSitio((int)ldrSitio["iCodCatalogo"]);


                    //Se valida si el sitio que se está procesando tiene configurado las bajas de recursos
                    if (loArchivosSitios.SitioConfigurado)
                    {
                        loArchivosSitios.iCodUsuarioDB = piCodUsuarioDB;


                        //Borra físicamente, de la ruta establecida, el archivo indicado
                        loArchivosSitios.borrarArchivo(TipoArchivo.Alta);


                        //Por cada registro encontrado en la vista [VisDetallados('Detall','Bitacora Consumo 100%','Español')]
                        //y cuyo campo BanderasBitacora100 sea igual a 1
                        foreach (DataRow ldrEmple in ldtBitacoraEmple100.Rows)
                        {
                            loArchivosSitios.insertarEmpleado(ldrEmple, TipoArchivo.Alta);
                        }
                    }
                }
            }
        }



        public void procesarAltasPresupuestoTemporal()
        {
            DSODataContext.SetContext(piCodUsuarioDB);

            try
            {
                DateTime ldtFechaInicioPrep;
                DateTime ldtFechaFinPrep;
                NotificacionPresupuestos.getFechasPeriodo(piDiaInicioPeriodo, piCodPeriodoPr,
                    out ldtFechaInicioPrep, out ldtFechaFinPrep);

                DataTable ldtPrepEmple = DSODataAccess.Execute(
                    "Select iCodCatalogo, Emple " +
                    " from [" + DSODataContext.Schema + "].[VisHistoricos('PrepEmple','Presupuesto Temporal','Español')] PrepEmple" +
                    " where FechaInicioPrep = '" + ldtFechaInicioPrep.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and   Emple in (" +
                    "    Select Emple " +
                    "     from  [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')] Bitacora100" +
                    "     where Bitacora100.FechaInicioPrep = PrepEmple.FechaInicioPrep" +
                    "     and   IsNull(Bitacora100.BanderasBitacora100, 0) = 1" +
                    "     and   Bitacora100.PrepEmple <> PrepEmple.iCodCatalogo" +
                    "     and   Bitacora100.FechaReset is null)" +
                    " and not Emple in (" +
                    "    Select Emple" +
                    "     from  [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Alta Códigos-Extensiones','Español')] BitacoraAlta" +
                    "     where BitacoraAlta.PrepEmple = PrepEmple.iCodCatalogo" +
                    "     and   BitacoraAlta.FechaInicioPrep = PrepEmple.FechaInicioPrep" +
                    "     and   BitacoraAlta.Sitio = " + piCodSitio + ")" +
                    " and dtIniVigencia <> dtFinVigencia" +
                    " and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                ArchivosSitio loArchivosSitios = new ArchivosSitio(piCodSitio);
                if (loArchivosSitios.SitioConfigurado)
                {
                    //RZ.20140214 Se cambia tipo de archivo a presupuesto provisional
                    loArchivosSitios.borrarArchivo(TipoArchivo.AltaPrepProv);
                    foreach (DataRow ldrEmple in ldtPrepEmple.Rows)
                    {
                        try
                        {
                            DataTable ldtBitacoraEmple100 = DSODataAccess.Execute(
                                "Select top 1 iCodRegistro, Emple, TipoPr, PeriodoPr, PrepEmple, ValorPresupuesto, ValorConsumo, FechaInicioPrep" +
                                " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')] Bitacora100" +
                                " where Bitacora100.FechaInicioPrep = '" + ldtFechaInicioPrep.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                                " and Bitacora100.Emple = " + ldrEmple["Emple"].ToString() +
                                " and IsNull(BanderasBitacora100, 0) = 1" +
                                " and FechaReset is null" +
                                " order by FechaInicioPrep desc, ValorConsumoBase desc");

                            if (ldtBitacoraEmple100.Rows.Count > 0)
                            {
                                ldtBitacoraEmple100.Rows[0]["PrepEmple"] = ldrEmple["iCodCatalogo"];
                                //RZ.20140214 Se cambia tipo de archivo a presupuesto provisional
                                loArchivosSitios.insertarEmpleado(ldtBitacoraEmple100.Rows[0], TipoArchivo.AltaPrepProv);
                            }
                        }
                        catch (Exception ex)
                        {
                            Util.LogException("Error al procesar altas de presupuesto temporal en ciclo de empleados:", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException("Error al procesar altas de presupuesto temporal", ex);
            }
        }

        
        /// <summary>
        /// Proceso que genera los archivos con los recursos que corresponden a empleados que han excedido su presupuesto
        /// </summary>
        public void procesarBajas()
        {
            //Obtiene los registros que se encuentren en la vista [VisDetallados('Detall','Bitacora Consumo 100%','Español')]
            //y cuyo campo BanderasBitacora100 sea igual a NULL
            DataTable ldtBitacoraEmple100 = getBitacora100();


            //Obtiene un listado de los sitios cuya empresa tenga configurada la baja de recursos
            DataTable ldtSitios = getSitios();


            //Ciclo que recorre cada uno de los sitios encontrados en el paso anterior
            foreach (DataRow ldrSitio in ldtSitios.Rows)
            {

                //Se crea una instancia del objeto ArchivosSitio, 
                //al hacerlo se obtienen los valores de configuracion del sitio en cuestión
                ArchivosSitio loArchivosSitios = new ArchivosSitio((int)ldrSitio["iCodCatalogo"]);


                //Se valida si el sitio que se está procesando tiene configurado las bajas de recursos
                if (loArchivosSitios.SitioConfigurado)
                {

                    loArchivosSitios.iCodUsuarioDB = piCodUsuarioDB;

                    //Borra físicamente, de la ruta establecida, el archivo indicado
                    loArchivosSitios.borrarArchivo(TipoArchivo.Baja);


                    //Por cada registro encontrado en la vista [VisDetallados('Detall','Bitacora Consumo 100%','Español')]
                    //y cuyo campo BanderasBitacora100 sea igual a NULL
                    foreach (DataRow ldrEmple in ldtBitacoraEmple100.Rows)
                    {

                        loArchivosSitios.insertarEmpleado(ldrEmple, TipoArchivo.Baja);
                    }
                }
            }


            //Actualiza el estatus del registro, marcando con un 1 el campo BanderasBitacora100
            //que sirve para identificar los registros que aún no han sido procesados
            ActualizarBitacora100(ldtBitacoraEmple100);
        }


        /// <summary>
        /// Actualiza el estatus del registro, marcando con un 1 el campo BanderasBitacora100
        /// que sirve para identificar los registros que aún no han sido procesados
        /// </summary>
        /// <param name="ldtRegistros"></param>
        private void ActualizarBitacora100(DataTable ldtRegistros)
        {
            DSODataAccess.ExecuteNonQuery(
                "Update [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')]" +
                " set BanderasBitacora100 = 1" +
                " where iCodRegistro in ( " + KeytiaServiceBL.Alarmas.UtilAlarma.DataTableToString(ldtRegistros, "iCodRegistro") + ")" +
                " and FechaReset is null");
        }


        /// <summary>
        /// Obtiene el listado de empleados que se encuentren en la vista [VisDetallados('Detall','Bitacora Consumo 100%','Español')]
        /// y cuyo campo BanderasBitacora100 sea igual a NULL
        /// </summary>
        /// <returns></returns>
        protected DataTable getBitacora100()
        {
            return DSODataAccess.Execute(
                "Select iCodRegistro, Emple, TipoPr, PeriodoPr, PrepEmple, ValorPresupuesto, ValorConsumo, FechaInicioPrep " +
                " from [" + DSODataContext.Schema + "].[VisDetallados('Detall','Bitacora Consumo 100%','Español')]" +
                " where IsNull(BanderasBitacora100, 0) = 0" +
                " and FechaReset is null");
        }

    }


    class ArchivosSitio
    {

        #region Atributos

        protected int piTipoBloqueo;
        protected int piBanderasPrepSitios;
        protected string psNombreArchivoAlta = "";
        protected string psRutaArchivoAlta = "";
        protected string psFormatoArchivoAlta = "";
        protected string psNombreArchivoBaja = "";
        protected string psRutaArchivoBaja = "";
        protected string psFormatoArchivoBaja = "";
        //RZ.20140228 Se agrega atritbuto para guardar la ruta del archivo de alta de presupuesto temporal
        protected string psRutaArchivoAltaPresupProv = "";
        protected TipoBloqueo peTipoBloqueo;

        #endregion

        
        #region Constructores

        public ArchivosSitio(int liCodSitio)
        {
            piCodSitio = liCodSitio;
            initVars();
        }

        #endregion


        #region Propiedades

        protected bool pbSitioConfigurado = false;
        protected int piCodUsuarioDB;
        protected int piCodSitio;


        public bool SitioConfigurado
        {
            get { return pbSitioConfigurado; }
        }

        public int iCodUsuarioDB
        {
            get { return piCodUsuarioDB; }
            set { piCodUsuarioDB = value; }
        }

        public int Sitio
        {
            get { return piCodSitio; }
            set { piCodSitio = value; }
        }

        #endregion


        #region Métodos


        /// <summary>
        /// Obtiene los valores de configuración de bajas, del sitio que se está procesando
        /// asigna dichos valores a los atributos correspondientes
        /// </summary>
        private void initVars()
        {
            KDBAccess kdb = new KDBAccess();

            //Obtiene los datos de configuración de baja de recursos del sitio que se está procesando
            DataTable ldtSitio = kdb.GetHisRegByEnt(
                "NotifPrepSitio", "Notificaciones de Presupuestos para Sitios", "[{Sitio}] = " + piCodSitio);


            //Valida si encontró algún registro en el paso anterior y de ser así asigna los valores a las respectivas variables
            if (ldtSitio != null && ldtSitio.Rows.Count > 0)
            {
                pbSitioConfigurado = true;
                piTipoBloqueo = (int)Util.IsDBNull(ldtSitio.Rows[0]["{TipoBloqueo}"], 0); //1. Códigos - 2. Extensiones
                peTipoBloqueo = (TipoBloqueo)piTipoBloqueo;
                piBanderasPrepSitios = (int)Util.IsDBNull(ldtSitio.Rows[0]["{BanderasPrepSitios}"], 0);
                psNombreArchivoAlta = ldtSitio.Rows[0]["{NombreArchivoAlta}"].ToString();
                psRutaArchivoAlta = ldtSitio.Rows[0]["{RutaArchivoAlta}"].ToString();
                psFormatoArchivoAlta = ldtSitio.Rows[0]["{FormatoArchivoAlta}"].ToString();
                psNombreArchivoBaja = ldtSitio.Rows[0]["{NombreArchivoBaja}"].ToString();
                psRutaArchivoBaja = ldtSitio.Rows[0]["{RutaArchivoBaja}"].ToString();
                psFormatoArchivoBaja = ldtSitio.Rows[0]["{FormatoArchivoBaja}"].ToString();
                //RZ.20140214 Leer de la configuracion la ruta del archivo de alta temporal
                psRutaArchivoAltaPresupProv = ldtSitio.Rows[0]["{RutaArchivoAltaTemp}"].ToString();
            }
        }

        
        /// <summary>
        /// Borra físicamente, de la ruta establecida, el archivo indicado
        /// se utiliza el mismo método para borrar el archivo de Altas y de Bajas
        /// </summary>
        /// <param name="leTipoArchivo"></param>
        public void borrarArchivo(TipoArchivo leTipoArchivo)
        {
            string lsPathArchivo = "";

            try
            {
                //RZ.20140214 Se cambian if's por switch para incluir el tipo de archivo Alta Prep Prov
                switch (leTipoArchivo)
                {
                    case TipoArchivo.Alta:
                        if (!string.IsNullOrEmpty(psRutaArchivoAlta) && !string.IsNullOrEmpty(psNombreArchivoAlta))
                        {
                            lsPathArchivo = System.IO.Path.Combine(psRutaArchivoAlta, psNombreArchivoAlta);
                        }
                        break;
                    case TipoArchivo.Baja:
                        if (!string.IsNullOrEmpty(psRutaArchivoBaja) && !string.IsNullOrEmpty(psNombreArchivoBaja))
                        {
                            lsPathArchivo = System.IO.Path.Combine(psRutaArchivoBaja, psNombreArchivoBaja);
                        }
                        break;
                    case TipoArchivo.AltaPrepProv:
                        if (!string.IsNullOrEmpty(psRutaArchivoAltaPresupProv) && !string.IsNullOrEmpty(psNombreArchivoAlta))
                        {
                            lsPathArchivo = System.IO.Path.Combine(psRutaArchivoAltaPresupProv, psNombreArchivoAlta);
                        }
                        break;
                    default:
                        lsPathArchivo = String.Empty;
                        break;
                }

                if (!string.IsNullOrEmpty(lsPathArchivo) && File.Exists(lsPathArchivo))
                {
                    //File.Delete(lsPathArchivo); //No se borrara el archivo lo que se hara será moverlo a backup
                    FileInfo lfiArchivoBackup = new FileInfo(lsPathArchivo);

                    MoverArchivoBackUp(lfiArchivoBackup);
                }
            }
            catch (Exception ex)
            {
                //borrar ---> mover
                Util.LogException("No se pudo mover el archivo " + lsPathArchivo + ".", ex);
            }
        }

        //RZ.20131226 Se agrega metodo que movera el archivo para una carpeta backup
        public string MoverArchivoBackUp(FileInfo lfiArch)
        {
            string lsFile = lfiArch.FullName;
            string lsPathDest = System.IO.Path.Combine(lfiArch.DirectoryName, "backup");
            string lsArchDest = string.Empty;

            if (lfiArch.Name.LastIndexOf(".") >= 1)
            {
                lsArchDest = System.IO.Path.Combine(lsPathDest, lfiArch.Name.Substring(0, lfiArch.Name.LastIndexOf("."))
                    + "." + DateTime.Now.ToString("yyyyMMdd.HHmmss") +
                    lfiArch.Name.Substring(lfiArch.Name.LastIndexOf(".")));
            }
            else
            {
                lsArchDest = System.IO.Path.Combine(lsPathDest, 
                    lfiArch.Name + "." + DateTime.Now.ToString("yyyyMMdd.HHmmss"));
            }

            try
            {
                Util.EnsureFolderExists(lsPathDest);
                lfiArch.MoveTo(lsArchDest);
                Util.LogMessage("Se movio el archivo de codigos " + lfiArch.Name + " a " + lsArchDest);
            }
            catch (Exception ex)
            {
                Util.LogException("Surgio un error al mover el archivo " + lfiArch.Name, ex);
                lsArchDest = "";
            }

            return lsArchDest;
        }

        /// <summary>
        /// Obtiene las extensiones o códigos, según sea el caso, del empleado que se está procesando
        /// se baja esa información al archivo de texto y se guarda un registro por cada recurso procesado
        /// en la vista [VisDetallados('detall','Bitacora Baja Códigos-Extensiones','español')] o [VisDetallados('detall','Bitacora Alta Códigos-Extensiones','español')]
        /// </summary>
        /// <param name="ldrEmpleado"></param>
        /// <param name="leTipoArchivo"></param>
        public void insertarEmpleado(DataRow ldrEmpleado, TipoArchivo leTipoArchivo)
        {
            string lsPathArchivo;
            string lsFormatoArchivo;
            string lsLinea;
            DataTable ldtRegistros;



            if (leTipoArchivo == TipoArchivo.Alta)
            {
                //Se valida que ni el nombre ni la ruta del archivo sean vacías o nulas,
                //de ser así se marca como error en el log
                if (string.IsNullOrEmpty(psRutaArchivoAlta) || string.IsNullOrEmpty(psNombreArchivoAlta))
                {
                    Util.LogMessage("No se pudo insertar el empleado " + ldrEmpleado["Emple"].ToString() + " porque no se tiene la ruta completa del archivo de alta. Favor de configurarlo en el Sitio.");
                    return;
                }


                Directory.CreateDirectory(psRutaArchivoAlta); //Se crea el archivo en donde se almacenará el archivo de altas
                lsPathArchivo = System.IO.Path.Combine(psRutaArchivoAlta, psNombreArchivoAlta); //Se forma el nombre del archivo de altas
                lsFormatoArchivo = psFormatoArchivoAlta;  //El formato configurado desde sistema
            }
            else
            {
                //Se valida que ni el nombre ni la ruta del archivo sean vacías o nulas,
                //de ser así se marca como error en el log
                if (string.IsNullOrEmpty(psRutaArchivoBaja) || string.IsNullOrEmpty(psNombreArchivoBaja))
                {
                    Util.LogMessage("No se pudo insertar el empleado " + ldrEmpleado["Emple"].ToString() + " porque no se tiene la ruta completa del archivo de baja. Favor de configurarlo en el Sitio.");
                    return;
                }


                Directory.CreateDirectory(psRutaArchivoBaja); //Se crea el archivo en donde se almacenará el archivo de bajas
                lsPathArchivo = System.IO.Path.Combine(psRutaArchivoBaja, psNombreArchivoBaja); //Se forma el nombre del archivo de bajas
                lsFormatoArchivo = psFormatoArchivoBaja; //El formato configurado desde sistema
            }
            

            //Valida si el tipo de baja configurado es por Codigos
            if (peTipoBloqueo == TipoBloqueo.Codigos)
            {
                
                //Obtiene el listado de códigos que tenga el empleado actualmente en el sitio que se está procesando, 
                //esto se obtiene mediante las relaciones "Empleado - CodAutorizacion" que sigan vigentes
                ldtRegistros = DSODataAccess.Execute(
                    "select Exten = null, ExtenCod = '',TNExten = '', ModeloExten = '', CosExten = ''," +
                    "       CodAuto = CodAuto.iCodCatalogo, CodAutoCod = CodAuto.vchCodigo, CodAuto.Cos, CodAuto.CosCod" +
                    " from [" + DSODataContext.Schema + "].[VisRelaciones('Empleado - CodAutorizacion','Español')] RelCodAuto," +
                    "      [" + DSODataContext.Schema + "].[VisHistoricos('CodAuto','Codigo Autorizacion','Español')] CodAuto," +
                    "      [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')] Emple" +
                    " where Emple.iCodCatalogo = " + ldrEmpleado["Emple"].ToString() +
                    " and CodAuto.Sitio = " + piCodSitio +
                    " and Emple.iCodCatalogo = RelCodAuto.Emple" +
                    " and RelCodAuto.CodAuto = CodAuto.iCodCatalogo" +
                    " and CodAuto.dtIniVigencia <> CodAuto.dtFinVigencia" +
                    " and CodAuto.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and CodAuto.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and RelCodAuto.dtIniVigencia <> RelCodAuto.dtFinVigencia" +
                    " and RelCodAuto.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and RelCodAuto.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and Emple.dtIniVigencia <> Emple.dtFinVigencia" +
                    " and Emple.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and Emple.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            }
            else  //Si el tipo de bloqueo no es por código, entonces es tipo de bloqueo por Extension
            {
                StringBuilder lsCampos = new StringBuilder();
                List<string> lstCampos = new List<string>();

                ldtRegistros = DSODataAccess.Execute("select top 0 * " +
                                                        " from [" + DSODataContext.Schema + "].[VisHistoricos('Exten','Español')] " +
                                                        " where 1=2");

                lstCampos.Add("TNExten");
                lstCampos.Add("ModeloExten");
                lstCampos.Add("CosExten");
                foreach (string lsCampo in lstCampos)
                {
                    if (ldtRegistros.Columns.Contains(lsCampo + "Cod"))
                    {
                        lsCampos.Append(lsCampo + " = " + lsCampo + "Cod, ");
                    }
                    else if (ldtRegistros.Columns.Contains(lsCampo))
                    {
                        lsCampos.Append(lsCampo + ", ");
                    }
                    else
                    {
                        lsCampos.Append(lsCampo + " = '', ");
                    }
                }


                //Obtiene el listado de extensiones que tenga el empleado actualmente en el sitio que se está procesando, 
                //esto se obtiene mediante las relaciones "Empleado - Extension" que sigan vigentes
                ldtRegistros = DSODataAccess.Execute(
                    "select Exten = Exten.iCodCatalogo, ExtenCod = Exten.vchCodigo, " + lsCampos.ToString() +
                    "       CodAuto = null, CodAutoCod = '', Cos = null, CosCod = ''" +
                    " from [" + DSODataContext.Schema + "].[VisRelaciones('Empleado - Extension','Español')] RelExten," +
                    "      [" + DSODataContext.Schema + "].[VisHistoricos('Exten','Español')] Exten," +
                    "      [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')] Emple" +
                    " where Emple.iCodCatalogo = " + ldrEmpleado["Emple"].ToString() +
                    " and Exten.Sitio = " + piCodSitio +
                    " and Emple.iCodCatalogo = RelExten.Emple" +
                    " and RelExten.Exten = Exten.iCodCatalogo" +
                    " and Exten.dtIniVigencia <> Exten.dtFinVigencia" +
                    " and Exten.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and Exten.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and RelExten.dtIniVigencia <> RelExten.dtFinVigencia" +
                    " and RelExten.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and RelExten.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and Emple.dtIniVigencia <> Emple.dtFinVigencia" +
                    " and Emple.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    " and Emple.dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            }


            //Para cada registro encontrado en el paso anterior (ya sean códigos o extensiones)
            foreach (DataRow ldrRegistro in ldtRegistros.Rows)
            {
                //Se forma la línea que se escribirá en el archivo, sustituyendo el metatag por el valor correspondiente
                lsLinea = lsFormatoArchivo
                            .Replace("Param(TNExten)", ldrRegistro["TNExten"].ToString())
                            .Replace("Param(ModeloExten)", ldrRegistro["ModeloExten"].ToString())
                            .Replace("Param(CosExten)", ldrRegistro["CosExten"].ToString())
                            .Replace("Param(Exten)", ldrRegistro["ExtenCod"].ToString())
                            .Replace("Param(CodAuto)", ldrRegistro["CodAutoCod"].ToString())
                            .Replace("Param(Cos)", ldrRegistro["CosCod"].ToString());


                //RZ.20140213 Si logro insertar la linea de texto, mandará a bitacora lo que inserto.
                //Escribe en un archivo de texto la línea que recibe como parámetro
                if (escribirArchivo(lsPathArchivo, lsLinea))
                {
                    //Agrega un registro en la vista 'Bitacora Alta Códigos-Extensiones' o 'Bitacora Baja Códigos-Extensiones' según aplique
                    insertarBitacora(ldrEmpleado, ldrRegistro, leTipoArchivo);
                }
                else
                {
                    //Hacer algo ya que no se inserto la linea.. 
                }
            }

        }


        //RZ.20140213 Se modifica metodo ahora regresara bool
        /// <summary>
        /// Escribe en un archivo de texto la línea que recibe como parámetro
        /// </summary>
        /// <param name="lsPathArchivo"></param>
        /// <param name="lsLinea"></param>
        /// <returns></returns>
        private bool escribirArchivo(string lsPathArchivo, string lsLinea)
        {
            try
            {
                if (!File.Exists(lsPathArchivo))
                {
                    using (StreamWriter sw = File.CreateText(lsPathArchivo))
                    {
                        sw.WriteLine(lsLinea);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(lsPathArchivo))
                    {
                        sw.WriteLine(lsLinea);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException("No se pudo escribir en el archivo " + lsPathArchivo + ".", ex);
                return false;
            }
            return true;
        }

        
        
        /// <summary>
        /// Agrega un registro en la vista 'Bitacora Alta Códigos-Extensiones' o 'Bitacora Baja Códigos-Extensiones' según aplique
        /// </summary>
        /// <param name="ldrEmpleado"></param>
        /// <param name="ldrRegistro"></param>
        /// <param name="leTipoArchivo"></param>
        private void insertarBitacora(DataRow ldrEmpleado, DataRow ldrRegistro, TipoArchivo leTipoArchivo)
        {
            string lsMaestro = "Bitacora " + (leTipoArchivo == TipoArchivo.Alta ? "Alta" : "Baja") + " Códigos-Extensiones";

            Hashtable lhtValores = new Hashtable();
            lhtValores.Add("{Sitio}", piCodSitio);
            lhtValores.Add("{Emple}", ldrEmpleado["Emple"]);
            lhtValores.Add("{TipoPr}", ldrEmpleado["TipoPr"]);
            lhtValores.Add("{PeriodoPr}", ldrEmpleado["PeriodoPr"]);
            lhtValores.Add("{PrepEmple}", ldrEmpleado["PrepEmple"]);
            lhtValores.Add("{ValorPresupuesto}", ldrEmpleado["ValorPresupuesto"]);
            lhtValores.Add("{ValorConsumo}", ldrEmpleado["ValorConsumo"]);
            lhtValores.Add("{FechaNotificacionConsumo}", DateTime.Now);
            lhtValores.Add("{FechaInicioPrep}", ldrEmpleado["FechaInicioPrep"]);
            if (!(ldrRegistro["CodAuto"] is DBNull))
            {
                lhtValores.Add("{CodAuto}", ldrRegistro["CodAuto"]);
            }
            if (!(ldrRegistro["Exten"] is DBNull))
            {
                lhtValores.Add("{Exten}", ldrRegistro["Exten"]);
            }
            if (!(ldrRegistro["Cos"] is DBNull))
            {
                lhtValores.Add("{Cos}", ldrRegistro["Cos"]);
            }
            if (leTipoArchivo == TipoArchivo.Alta)
            {
                lhtValores.Add("{FormatoArchivoAlta}", psFormatoArchivoAlta);
            }
            else
            {
                lhtValores.Add("{FormatoArchivoBaja}", psFormatoArchivoBaja);
            }
            lhtValores.Add("dtFecUltAct", DateTime.Now);

            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            cargasCOM.InsertaRegistro(lhtValores, "Detallados", "Detall", lsMaestro, piCodUsuarioDB);

        }


        #endregion

    }
}
