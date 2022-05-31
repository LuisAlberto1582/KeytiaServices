using KeytiaServiceBL.Models;
using System;
using System.Diagnostics;

namespace KeytiaServiceBL.CargaCDR.CargaCDRSBC
{

    public class CargaCDRSBCPentafonPortabilidad : CargaCDRSBCPentafon
    {
        public event NuevoRegistroEventHandler NuevoRegistro;
        public CargaCDRSBCPentafonPortabilidad()
        {
            piColumnas = 12;

            piFecha = 0;
            piDuracion = 1;
            piTroncal = 4;
            piCallerId = 2;
            piTipo = 5;
            piDigitos = 3;
            piCodigo = 6;
            piFechaOrigen = 0;

            piSrcURI_2 = 7;
            piSrcURI = 8;
            piDstURI = 9;
            piTrmReason = 10;
            piTrmReasonCategory = 11;

            psFormatoDuracionCero = "0";
        }

        protected override void AbrirArchivo()
        {

            //RJ.20160908 Se valida si se tiene encendida la bandera de que toda llamada de Enlace o Entrada se asigne al
            //empleado 'Enlace y Entrada' y algunos de los datos nesearios no se hayan encontrado en BD
            if (pbAsignaLlamsEntYEnlAEmpSist && (piCodCatEmpleEnlYEnt == 0 || piCodCatTDestEnl == 0 || piCodCatTDestEnt == 0 || piCodCatTDestExtExt == 0))
            {
                ActualizarEstCarga("ErrCarNoExisteEmpEnlYEnt", "Cargas CDRs");
                return;
            }


            if (!pfrCSV.Abrir(psArchivo1))
            {
                ActualizarEstCarga("ArchNoVal1", "Cargas CDRs");
                return;
            }

            lsSeccion = "AbrirArchivo_001";
            stopwatch.Reset();
            stopwatch.Start();
            if (!ValidarArchivo())
            {
                if (pbRegistroCargado)
                {
                    ActualizarEstCarga("ArchEnSis1", "Cargas CDRs");
                }
                else
                {
                    ActualizarEstCarga("Arch1NoFrmt", "Cargas CDRs");
                }
                return;
            }
            stopwatch.Stop();
            Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));


            piRegistro = 0;
            piDetalle = 0;
            piPendiente = 0;

            pfrCSV.Abrir(psArchivo1);

            kdb.FechaVigencia = Util.IsDate(pdtFecIniTasacion.ToString("yyyyMMdd"), "yyyyMMdd");

            lsSeccion = "AbrirArchivo_002";
            stopwatch.Reset();
            stopwatch.Start();
            CargaAcumulados(ObtieneListadoSitiosComun<SitioSBC>(plstSitiosEmpre));
            stopwatch.Stop();
            Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));


            palRegistrosNoDuplicados.Clear();

            do
            {
                try
                {
                    psCDR = pfrCSV.SiguienteRegistro();
                    piRegistro++;
                    psMensajePendiente.Length = 0;
                    pGpoTro = new GpoTroComun();
                    piGpoTro = 0;
                    psGpoTroEntCDR = string.Empty;
                    psGpoTroSalCDR = string.Empty;
                    pscSitioLlamada = null;
                    pscSitioDestino = null;
                    pscSitioLlamada = null;
                    pscSitioDestino = null;

                    if (NuevoRegistro != null)
                    {
                        NuevoRegistro(this,
                            new NuevoRegistroEventArgs(piRegistro, pdtFecIniCarga, DateTime.Now, "Nombre_Archivo", 0));
                    }


                    lsSeccion = "AbrirArchivo_003";
                    stopwatch.Reset();
                    stopwatch.Start();
                    bool esValido = ValidarRegistro();
                    stopwatch.Stop();
                    Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));

                    if (esValido)
                    {
                        //2012.12.19 - DDCP - Toma como vigencia la fecha de la llamada cuando es valida y diferente a
                        // la fecha de de inicio del archivo
                        lsSeccion = "AbrirArchivo_004";
                        stopwatch.Reset();
                        stopwatch.Start();

                        if (pdtFecha != DateTime.MinValue && pdtFecha.ToString("yyyyMMdd") != kdb.FechaVigencia.ToString("yyyyMMdd"))
                        {
                            kdb.FechaVigencia = Util.IsDate(pdtFecha.ToString("yyyyMMdd"), "yyyyMMdd");
                            GetExtensiones();
                            GetCodigosAutorizacion();
                        }
                        stopwatch.Stop();
                        Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));

                        lsSeccion = "AbrirArchivo_005";
                        stopwatch.Reset();
                        stopwatch.Start();
                        ActualizarCampos();
                        stopwatch.Stop();
                        Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));

                        lsSeccion = "AbrirArchivo_006";
                        stopwatch.Reset();
                        stopwatch.Start();
                        GetCriterios();
                        stopwatch.Stop();
                        Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));

                        lsSeccion = "AbrirArchivo_007";
                        stopwatch.Reset();
                        stopwatch.Start();
                        ProcesarRegistro();
                        stopwatch.Stop();
                        Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));

                        lsSeccion = "AbrirArchivo_008";
                        stopwatch.Reset();
                        stopwatch.Start();
                        TasarRegistro();
                        stopwatch.Stop();
                        Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));


                        lsSeccion = "AbrirArchivo_009";
                        stopwatch.Reset();
                        stopwatch.Start();
                        if (pbEnviarDetalle == true)
                        {
                            //RJ. Se valida si se encontró el sitio de la llamada en base a la extensión
                            //de no ser así, se asignará el sitio 'Ext fuera de rango'
                            if (pbEsExtFueraDeRango)
                            {
                                phCDR["{Sitio}"] = piCodCatSitioExtFueraRang;
                            }

                            //RJ.20170109 Cambio para validar bandera de cliente 
                            if (!pbEnviaEntYEnlATablasIndep)
                            {
                                //EnviarMensaje(phCDR, "Detallados", "Detall", "DetalleCDR");
                                psNombreTablaIns = "Detallados";
                                InsertarRegistroCDR(CrearRegistroCDR());
                            }
                            else
                            {
                                if (phCDR["{TDest}"].ToString() == piCodCatTDestEnt.ToString() || phCDR["{TDest}"].ToString() == piCodCatTDestEntPorDesvio.ToString())
                                {
                                    psNombreTablaIns = "DetalleCDREnt";
                                    InsertarRegistroCDREntYEnl(CrearRegistroCDR());
                                }
                                else if (phCDR["{TDest}"].ToString() == piCodCatTDestEnl.ToString() || phCDR["{TDest}"].ToString() == piCodCatTDestExtExt.ToString() ||
                                            phCDR["{TDest}"].ToString() == piCodCatTDestEnlPorDesvio.ToString() || phCDR["{TDest}"].ToString() == piCodCatTDestExtExtPorDesvio.ToString())
                                {
                                    psNombreTablaIns = "DetalleCDREnl";
                                    InsertarRegistroCDREntYEnl(CrearRegistroCDR());
                                }
                                else
                                {
                                    //EnviarMensaje(phCDR, "Detallados", "Detall", "DetalleCDR");
                                    psNombreTablaIns = "Detallados";
                                    InsertarRegistroCDR(CrearRegistroCDR());
                                }
                            }

                            if (pbCDRConCamposAdic && psCDR != null && psCDR.Length > 0)
                            {
                                FillCDRComplemento();
                                InsertarRegistroCDRComplemento(CrearRegistroCDRComplemento(), "DetalleCDRComplemento");
                            }

                            piDetalle++;
                            continue;
                        }
                        else
                        {
                            //ProcesaPendientes();
                            psNombreTablaIns = "Pendientes";
                            InsertarRegistroCDRPendientes(CrearRegistroCDR());

                            if (pbCDRConCamposAdic && psCDR != null && psCDR.Length > 0)
                            {
                                FillCDRComplemento();
                                InsertarRegistroCDRComplemento(CrearRegistroCDRComplemento(), "PendientesCDRComplemento");
                            }

                            piPendiente++;
                        }
                        stopwatch.Stop();
                        Debug.WriteLine(string.Format("Método: {0}, Sección: {1}, Tiempo: {2}", "AbrirArchivo()", lsSeccion, stopwatch.Elapsed));

                    }
                    else
                    {
                        /*RZ.20130308 Se manda a llamar GetCriterios() y ProcesaRegistro() metodo para que establezca las propiedades que llenaran el hashtable que envia pendientes
                        desde este metodo se invoca el metodo FillCDR() que es quien prepara el hashtable del registro a CDR de pendientes o detallados */
                        //GetCriterios(); RZ.20130404 Se retira llamada metodo y se reemplaza por CargaServicioCDR.ProcesaRegistroPte()
                        ProcesarRegistroPte();
                        //ProcesarRegistro();
                        //ProcesaPendientes();
                        psNombreTablaIns = "Pendientes";
                        InsertarRegistroCDRPendientes(CrearRegistroCDR());

                        if (pbCDRConCamposAdic && psCDR != null && psCDR.Length > 0)
                        {
                            FillCDRComplemento();
                            InsertarRegistroCDRComplemento(CrearRegistroCDRComplemento(), "PendientesCDRComplemento");
                        }

                        piPendiente++;
                    }
                }
                catch (Exception e)
                {
                    Util.LogException("Error Inesperado Registro: " + piRegistro.ToString(), e);
                    psMensajePendiente = psMensajePendiente.Append(" [Error Inesperado Registro: " + piRegistro.ToString() + "]");
                    //ProcesaPendientes();
                    psNombreTablaIns = "Pendientes";
                    InsertarRegistroCDRPendientes(CrearRegistroCDR());

                    if (pbCDRConCamposAdic && psCDR != null && psCDR.Length > 0)
                    {
                        FillCDRComplemento();
                        InsertarRegistroCDRComplemento(CrearRegistroCDRComplemento(), "PendientesCDRComplemento");
                    }

                    piPendiente++;
                }

            } while (psCDR != null);

          

            //LAAV 202200518: Agregando que se genere la informacion despues de tazar
			Step1RegeneraInfoPentafon();
            Step2RegeneraInfoPentafon();
            Step3RegeneraInfoPentafon();
            Step4RegeneraInfoPentafon();
            Step5RegeneraInfoPentafon();
            Step6RegeneraInfoPentafon();
            Step7RegeneraInfoPentafon();
            Step8RegeneraInfoPentafon();
            Step9RegeneraInfoPentafon();
            Step10RegeneraInfoPentafon();
            ActualizarEstCarga("CarFinal", "Cargas CDRs");
            pfrCSV.Cerrar();
        }


        public void Step1RegeneraInfoPentafon()
        {
            string query = $@"
                --Regenera ResumenCDR

                --RJ.20190607 Solo revisa los últimos 2 meses (el actual y el anterior)
                --porque de lo contrario, elimina la información de Resumen pues
                --ésta no coincide contra lo que está en detalle, ya que el detalle 
                --se depura constantemente
                exec [RegeneraResumenCDR] 'Pentafon'

                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step2RegeneraInfoPentafon()
        {
            string query = $@"
                declare @fechaInicio varchar(10)
                declare @fechaFin varchar(10)

                select @fechaInicio = convert(varchar,dateadd(dd, -(datepart(dd, isnull(MIN(convert(date,date01)), '2011-01-01')))+1, isnull(MIN(convert(date,date01)), '2011-01-01')), 120),
		                @fechaFin = convert(varchar,isnull(Max(convert(date,date01)), '2011-01-01'), 120)
                from Keytia5.Pentafon.Detallados
                where iCodMaestro = 89
                and CONVERT(date,dtFecUltAct) >= CONVERT(date, dateadd(dd,-1,getdate()))
	
                exec GeneraConsumoJerarquicoyAcumulado 
		                @Schema = 'Pentafon', 
		                @FechaIniRep = @fechaInicio, 
		        @FechaFinRep =  @fechaFin
                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step3RegeneraInfoPentafon()
        {
            string query = @"
              use Keytia5
                declare @fechaFin varchar(30)
                declare @fechaInicio varchar(30)
                declare @fechaInicioActual varchar(30)
                declare @anio int 
                declare @mes int
                declare @dia int
                declare @Where varchar(max) = ''

                declare @usuar int
                declare @usuarios int
                declare @iCodCatUsuario int
                declare @iCodCatPerfil int
                declare @veces int


                CREATE TABLE #Usuarios
                (
                Id int not null identity primary key,
                iCodCatUsuario int not null,
                iCodCatPerfil int not null
                )

                insert into #Usuarios
                select distinct iCodCatalogo, perfil
                from [Pentafon].[vishistoricos('Usuar','Usuarios','Español')]
                where dtinivigencia <> dtfinvigencia
                and dtfinvigencia >= getdate()
                and icodcatalogo in (select usuar from [Pentafon].RegSesion where fecacc >= '2019-02-20 15:29:09')

                select @usuarios = @@rowcount
                select @veces = 1

                TRUNCATE TABLE [Pentafon].ReporteHistorico

                while @veces <= @usuarios
                begin

	                select @iCodCatUsuario = iCodCatUsuario,
			                @iCodCatPerfil = iCodCatPerfil
	                from #Usuarios
	                where id = @veces

	                set @mes = MONTH(GETDATE())
	                set @anio = YEAR(GETDATE())
	                set @dia = DAY(GETDATE())
	                set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'

	                if @mes < 10
	                begin
 		                set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                end
	                else
		                begin
 		                set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                end

	                set @fechaInicio = '''' + @fechaInicio + ''''
	                set @fechaFin = '''' + @fechaFin + ''''

	                insert into [Pentafon].ReporteHistorico
	                exec ConsumoHistoricoOptDashFC    
	                @Schema =		'Pentafon',
	                @Fields =		'[Nombre Mes],[Mes Anio] = replace([Mes Anio],'' '',''-''),
					                [link] = ''/keytia5/UserInterface/DashboardFC/Dashboard.aspx?Nav=HistoricoN2&MesAnio='' + replace([Mes Anio],'' '',''-''), 
					                [Total] = ROUND(SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]),2)', 
	                @Where =		@Where,
	                @Group =		'[Nombre Mes],[Mes Anio]',
	                @Order =		'[Orden] Asc',
	                @OrderInv =	'[Orden] Desc',
	                @Usuario =		@iCodCatUsuario,
	                @Perfil =		@iCodCatPerfil,
	                @FechaIniRep = @fechaInicio,
	                @FechaFinRep = @fechaFin,
	                @Moneda =		'MXP',
	                @Idioma =		'Español',
	                @isFT =		0,
	                @FillTable =	1


	                select @veces = @veces + 1
                end


                ALTER INDEX ALL on [Pentafon].ReporteHistorico REBUILD

            ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step4RegeneraInfoPentafon()
        {
            string query = @"
                    use Keytia5
                    declare @fechaIni DateTime
                    declare @fechaInicio varchar(20)
                    declare @fechaFin varchar(20)


                    set @fechaIni = dateadd(mm, -5, getdate())

                    select @fechainicio = 
	                    CONVERT(varchar, 
	                    case 
		                    when len(datepart(mm,@fechaIni))=1 then 
			                    convert(varchar,datepart(yyyy,@fechaIni))+'-'+convert(varchar,'0')+convert(varchar,datepart(mm,@fechaIni))+'-'+convert(varchar,'01')+ ' 00:00:00'
		                    when len(datepart(mm,@fechaIni))=2 then 
			                    convert(varchar,datepart(yyyy,@fechaIni))+'-'+convert(varchar,datepart(mm,@fechaIni))+'-'+convert(varchar,'01')+ ' 00:00:00'
	                    end
	                    , 120)

                    select @fechaFin = 
	                    CONVERT(varchar,
	                    case 
			                    when len(datepart(mm,getdate()))=1 and len(datepart(dd,getdate()))=1 then 
				                    convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
			                    when len(datepart(mm,getdate()))=1 and len(datepart(dd,getdate()))=2 then 
				                    convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
			                    when len(datepart(mm,getdate()))=2 and len(datepart(dd,getdate()))=1 then 
				                    convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
			                    when len(datepart(mm,getdate()))=2 and len(datepart(dd,getdate()))=2 then 
				                    convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
		                    end
	                    , 120)

                    select @fechainicio, @fechaFin



                    Exec ReporteTipoDeCampaniasFillCarriers 
	                    @esquema			= 'Pentafon' ,     
	                    @fechaInicioReporte = @fechainicio, 
	                    @fechaFinReporte	= @fechaFin, 
	                    @campaña            = 0,             
	                    @formatoFecha		= 'Case When Month >= 10 Then Convert(varchar,Month) Else (''0''+ convert(varchar,Month)) End  + ''/'' + convert(varchar,year)',  
	                    @formatoFechaInt	= 'convert(varchar,year)+ Case When month >= 10 Then (convert(varchar,month)) Else (''0''+Convert(varchar,Month)) End '

                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step5RegeneraInfoPentafon()
        {
            string query = @"
                use Keytia5
                declare @fechaIni DateTime
                declare @fechaInicio varchar(20)
                declare @fechaFin varchar(20)


                set @fechaIni = dateadd(mm, -5, getdate())

                select @fechainicio = 
	                CONVERT(varchar, 
	                case 
		                when len(datepart(mm,@fechaIni))=1 then 
			                convert(varchar,datepart(yyyy,@fechaIni))+'-'+convert(varchar,'0')+convert(varchar,datepart(mm,@fechaIni))+'-'+convert(varchar,'01')+ ' 00:00:00'
		                when len(datepart(mm,@fechaIni))=2 then 
			                convert(varchar,datepart(yyyy,@fechaIni))+'-'+convert(varchar,datepart(mm,@fechaIni))+'-'+convert(varchar,'01')+ ' 00:00:00'
	                end
	                , 120)

                select @fechaFin = 
	                CONVERT(varchar,
	                case 
			                when len(datepart(mm,getdate()))=1 and len(datepart(dd,getdate()))=1 then 
				                convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
			                when len(datepart(mm,getdate()))=1 and len(datepart(dd,getdate()))=2 then 
				                convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
			                when len(datepart(mm,getdate()))=2 and len(datepart(dd,getdate()))=1 then 
				                convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,'0')+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
			                when len(datepart(mm,getdate()))=2 and len(datepart(dd,getdate()))=2 then 
				                convert(varchar,datepart(yyyy,getdate()))+'-'+convert(varchar,datepart(mm,getdate()))+'-'+convert(varchar,datepart(dd,getdate()))+ ' 23:59:29'
		                end
	                , 120)

                select @fechainicio, @fechaFin


                TRUNCATE TABLE Pentafon.ReporteTiposDeCamapaniasResumenPorDia


                insert into Pentafon.ReporteTiposDeCamapaniasResumenPorDia
                select     dia     = Datepart(Day, FechaInicio),
			                mes     = Datepart(Month, FechaInicio),
			                año                 = Datepart(Year, FechaInicio)  ,
			                fecha    = Convert(varchar,FechaInicio,103),
			                Sitio, 
			                tDest    = tdest.iCodCatalogo,
			                tdestCod            = tdest.vchCodigo,
			                carrier    =   Carrier,
			                carrierCod   =   Carrier.vchCodigo,
			                carrierDesc        = carrier.vchDescripcion,
			                Costo    =   Sum(Costo + CostoSM)  ,
			                Emple = detall.emple 
                from [Pentafon].[VisAcumulados('AcumDia','ResumenCDR','Español')]  detall  
			  
                JOIN [Pentafon].Carrier  Carrier   
	                ON detall.Carrier = Carrier.iCodCatalogo   
	                and Carrier.dtInivigencia <> Carrier.dtFinVigencia   
	                and Carrier.dtFinvigencia >= getdate()    
			
                JOIN [Pentafon].TDest TDest   
	                ON detall.TDest = TDest.iCodCatalogo   
	                and TDest.dtInivigencia <> TDest.dtFinVigencia   
	                and TDest.dtFinvigencia >= getdate()    
				
                where FechaInicio >= @fechaInicio
                and FechaInicio <= @fechaFin                                       
                group by datepart(Day, FechaInicio),                
		                datepart(Month, FechaInicio),              
		                datepart(Year, FechaInicio)  ,             
		                convert(varchar,FechaInicio,103),  
		                Sitio,        
		                tdest.iCodCatalogo,   
		                tdest.vchCodigo,   
		                Carrier,                                   
		                Carrier.vchCodigo,   
		                carrier.vchDescripcion ,
		                detall.Emple

                ALTER INDEX ALL ON Pentafon.ReporteTiposDeCamapaniasResumenPorDia REBUILD
            ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step6RegeneraInfoPentafon()
        {
            string query = $@"
                        --Re-genera la tabla ResumenCDRExtensionCodigo, 
                        --que sirve para obtener la información para ciertos indicadores del Dashboard nuevo L&F

                        EXEC ReGeneraResumenPorExtensionYCodigo @esquema = 'Pentafon'
                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step7RegeneraInfoPentafon()
        {
            string query = $@"
               
                    --HISTORICO TELEFONIA MOVIL

                    SET NOCOUNT ON

                    declare @fechaFin varchar(30)
                    declare @fechaInicio varchar(30)
                    declare @fechaInicioActual varchar(30)
                    declare @anio int 
                    declare @mes int
                    declare @dia int
                    declare @Where varchar(max) = ''

                    declare @usuar int
                    declare @usuarios int
                    declare @iCodCatUsuario int
                    declare @iCodCatPerfil int
                    declare @veces int

                    if OBJECT_ID('tempdb..#Usuarios') is not null
	                    drop table #Usuarios


                    CREATE TABLE #Usuarios
                    (
                    Id int not null identity primary key,
                    iCodCatUsuario int not null,
                    iCodCatPerfil int not null
                    )

                    insert into #Usuarios
                    select distinct iCodCatalogo, perfil
                    from [Pentafon].[vishistoricos('Usuar','Usuarios','Español')]
                    where dtinivigencia <> dtfinvigencia
                    and dtfinvigencia >= getdate()
                    --and icodcatalogo in (select usuar from [Pentafon].RegSesion where fecacc >= '2021-04-24 12:14:08')

                    select @usuarios = @@rowcount
                    select @veces = 1

                    TRUNCATE TABLE [Pentafon].ReporteHistoricoMoviles

                    while @veces <= @usuarios
                    begin

	                    select @iCodCatUsuario = iCodCatUsuario,
			                    @iCodCatPerfil = iCodCatPerfil
	                    from #Usuarios
	                    where id = @veces

	                    set @mes = MONTH(GETDATE())
	                    set @anio = YEAR(GETDATE())
	                    set @dia = DAY(GETDATE())
	                    set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'

	                    if @mes < 10
	                    begin
 		                    set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                    set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                    end
	                    else
		                    begin
 		                    set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                    set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                    end

	                    --set @fechaInicio = CHAR(39) + @fechaInicio + CHAR(39)
	                    --set @fechaFin = CHAR(39) + @fechaFin + CHAR(39)

	                    insert into [Pentafon].ReporteHistoricoMoviles
	                    EXEC [dbo].[HistoricoGastoInternet12Meses]
		                    @Esquema		=	'Pentafon',
		                    @FechaIni		=	@fechaInicio,
		                    @FechaFin		=	@fechaFin,
		                    @iCodUsuario	=	@iCodCatUsuario,
		                    @iCodPerfil		=	@iCodCatPerfil,
		                    @Moneda			=	'MXP',
		                    @isFT			=	0,
		                    @FillTable		=	1


	                    select @veces = @veces + 1
                    end


                    ALTER INDEX ALL on [Pentafon].ReporteHistoricoMoviles REBUILD




                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step8RegeneraInfoPentafon()
        {
            string query = $@"
               
                    SET NOCOUNT ON

                    declare @fechaFin varchar(30)
                    declare @fechaInicio varchar(30)
                    declare @fechaInicioActual varchar(30)
                    declare @anio int 
                    declare @mes int
                    declare @dia int
                    declare @Where varchar(max) = ''

                    declare @usuar int
                    declare @usuarios int
                    declare @iCodCatUsuario int
                    declare @iCodCatPerfil int
                    declare @veces int

                    if OBJECT_ID('tempdb..#Usuarios') is not null
	                    drop table #Usuarios


                    CREATE TABLE #Usuarios
                    (
                    Id int not null identity primary key,
                    iCodCatUsuario int not null,
                    iCodCatPerfil int not null
                    )

                    insert into #Usuarios
                    select distinct iCodCatalogo, perfil
                    from [Pentafon].[vishistoricos('Usuar','Usuarios','Español')]
                    where dtinivigencia <> dtfinvigencia
                    and dtfinvigencia >= getdate()
                    and icodcatalogo in (select usuar from [Pentafon].RegSesion where fecacc >= '2021-04-25 12:33:31')

                    select @usuarios = @@rowcount
                    select @veces = 1

                    TRUNCATE TABLE [Pentafon].CantLineasExcedenLimiteInternetNal

                    while @veces <= @usuarios
                    begin

	                    select @iCodCatUsuario = iCodCatUsuario,
			                    @iCodCatPerfil = iCodCatPerfil
	                    from #Usuarios
	                    where id = @veces

	                    set @mes = MONTH(GETDATE())
	                    set @anio = YEAR(GETDATE())
	                    set @dia = DAY(GETDATE())
	                    set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'

	                    if @mes < 10
	                    begin
 		                    set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                    set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                    end
	                    else
		                    begin
 		                    set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                    set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                    end


	                    insert into [Pentafon].CantLineasExcedenLimiteInternetNal
	                    exec IndicadoresMovilCantLineasExcedenLimiteInternetNal 
	                    @Schema = 'Pentafon', 
	                    @fechaIniRep = @fechaInicio, 
	                    @Usuario = @iCodCatUsuario, 
	                    @Perfil = @iCodCatPerfil,
	                    @isFT =  0,
	                    @FillTable = 1


	                    select @veces = @veces + 1
                    end


                    ALTER INDEX ALL on [Pentafon].CantLineasExcedenLimiteInternetNal REBUILD



                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step9RegeneraInfoPentafon()
        {
            string query = $@"
               
                        SET NOCOUNT ON

                        declare @fechaFin varchar(30)
                        declare @fechaInicio varchar(30)
                        declare @fechaInicioActual varchar(30)
                        declare @anio int 
                        declare @mes int
                        declare @dia int
                        declare @Where varchar(max) = ''

                        declare @usuar int
                        declare @usuarios int
                        declare @iCodCatUsuario int
                        declare @iCodCatPerfil int
                        declare @veces int

                        if OBJECT_ID('tempdb..#Usuarios') is not null
	                        drop table #Usuarios


                        CREATE TABLE #Usuarios
                        (
                        Id int not null identity primary key,
                        iCodCatUsuario int not null,
                        iCodCatPerfil int not null
                        )

                        insert into #Usuarios
                        select distinct iCodCatalogo, perfil
                        from [Pentafon].[vishistoricos('Usuar','Usuarios','Español')]
                        where dtinivigencia <> dtfinvigencia
                        and dtfinvigencia >= getdate()
                        and icodcatalogo in (select usuar from [Pentafon].RegSesion where fecacc >= '2021-04-25 12:33:31')

                        select @usuarios = @@rowcount
                        select @veces = 1

                        TRUNCATE TABLE [Pentafon].CantLineasConConsumoInternetInt

                        while @veces <= @usuarios
                        begin

	                        select @iCodCatUsuario = iCodCatUsuario,
			                        @iCodCatPerfil = iCodCatPerfil
	                        from #Usuarios
	                        where id = @veces

	                        set @mes = MONTH(GETDATE())
	                        set @anio = YEAR(GETDATE())
	                        set @dia = DAY(GETDATE())
	                        set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'

	                        if @mes < 10
	                        begin
 		                        set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                        set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                        end
	                        else
		                        begin
 		                        set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00'
 		                        set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'
	                        end


	                        insert into [Pentafon].CantLineasConConsumoInternetInt
	                        exec IndicadoresMovilCantLineasConConsumoInternetInt 
	                        @Schema = 'Pentafon', 
	                        @fechaIniRep = @fechaInicio, 
	                        @Usuario = @iCodCatUsuario, 
	                        @Perfil = @iCodCatPerfil,
	                        @isFT =  0,
	                        @FillTable = 1


	                        select @veces = @veces + 1
                        end


                        ALTER INDEX ALL on [Pentafon].CantLineasConConsumoInternetInt REBUILD



                        GO

                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
        public void Step10RegeneraInfoPentafon()
        {

            string query = $@"
               
					set nocount on

					declare @fechaYHoraActual DateTime


					delete Keytia.BitacoraEjecucionLlenaIndicadores
					where Fecha < DATEADD(dd, -7, convert(date, getdate()))


					if(object_id('tempdb..#TempIndicadores') is not null)
					Begin
						Drop table #TempIndicadores
					End 

					create table #TempIndicadores
					(
						id int primary key identity not null,
						NombreIndicador varchar(100)
					)

					insert into #TempIndicadores
					Values
					('IndDuracionPromedioLlamadas'),
					('IndGastoPerCapita'),
					('IndMinutosLlamConMasDuracion'),
					('IndConcentracionDelGasto'),
					('IndCantCodigosNuevos'),
					('IndCantidadExtenNuevas'),
					('IndCantidadLineasNuevas'),
					('IndMinutosLlamConMasDuracionLDM'),
					('IndiExtensionesPorIdentificar'),
					('IndiCodigosPorIdentificar')



					If(object_id('tempdb..#TempEsquemas') is not null)
					Begin 
						Drop table #TempEsquemas
					End 

					create table #TempEsquemas
					(
						id int primary key identity not null,
						iCodCatEsquema int,
						Esquema varchar(100),
						fechaIniIndicador datetime,
						fechaFinIndicador datetime
					)

					Insert into #tempEsquemas
					Select 
						iCodCatalogo,
						Esquema,
						null,
						null
					From Keytia.[VisHistoricos('UsuarDB','Usuarios DB','Español')] usuardb
					Inner Join sys.schemas esquemas
					On esquemas.name = usuardb.esquema
					Where usuardb.dtIniVigencia <> usuardb.dtFinVigencia
					And usuardb.dtFinVigencia >= GETDATE()
					And usuardb.Esquema = 'Pentafon'


					Declare @nReg int =0
					Declare @contador int =1

					Select @nReg = count(*)
					From #TempEsquemas



					While(@contador <= @nReg)
					Begin

						Declare @MaxFechaDetalle smalldatetime
						Declare @FechaIniIndicador datetime
						Declare @FechaFinIndicador datetime

						Declare @EsquemaActual varchar(100) = ''


						Select @EsquemaActual  = Esquema
						from #TempEsquemas
						Where id = @contador

						--Select @esquemaActual = 'AlfaCorporativo'

						Declare @nQuery nvarchar(max) = ''

						Set @nQuery = '
							Select @MaxFechaDetalle =  Max(FechaInicio)
							From 
							(
								Select FechaInicio = Max(FechaInicio)
								From  ['+@EsquemaActual+'].[VisDetallados(''Detall'',''DetalleCDR'',''Español'')]
								Where FechaInicio <= GETDATE()

								Union 

								Select FechaInicio = Max(FechaInicio)
								From ['+@EsquemaActual+'].[visDetallados(''Detall'',''DetalleFacturaCDR'',''Español'')]
								Where FechaInicio <= GETDATE()
							) Cons

							if(Convert(int,Datepart(day,@MaxFechaDetalle)) < 5)
							Begin
								Set @MaxFechaDetalle  = Dateadd(MONTH,-1,@MaxFechaDetalle)
							End 
						'
						Execute sp_executesql 
							@nquery,
							N'@MaxFechaDetalle varchar(30) OUTPUT',
							 @MaxFechaDetalle =  @MaxFechaDetalle OUTPUT

						Select @FechaIniIndicador = convert(varchar,DATEPART(YEAR,@maxfechadetalle)) +'-' +Case When Len(DATEPART(Month,@maxfechadetalle))  < 2  then '0'+ convert(varchar,DATEPART(Month,@maxfechadetalle)) Else convert(varchar,DATEPART(Month,@maxfechadetalle))End  + '-01 00:00:00.000'
						Select @fechaFinIndicador  = convert(varchar,Dateadd(second,-31,Dateadd(Month,1,@fechaIniIndicador)),121)

						if(@FechaIniIndicador <> '' And @FechaFinIndicador <> '')
						Begin
							Update TempEsquemas
							Set 
								fechaIniIndicador = @FechaIniIndicador,
								fechaFinIndicador = @FechaFinIndicador
							From #TempEsquemas TempEsquemas
							Where id = @contador
							And Esquema = @esquemaActual
						End

						Set @contador = @contador + 1 

					End 


					Set @contador = 1
					Select  @nReg = COUNT(*)
					From #TempEsquemas tempEsquemas

					While(@contador <= @nReg)
					Begin
						Set @EsquemaActual = ''
						Set @fechaIniIndicador = ''
						Set @fechaFinIndicador = ''

						Select 
							@EsquemaActual = Esquema,
							@fechaIniIndicador = FechaIniIndicador,
							@fechaFinIndicador = FechaFinIndicador
						From #TempEsquemas
						Where id = @contador


						if(@fechaIniIndicador <> '' And @fechaFinIndicador <> '' And @esquemaActual <> '')
						Begin

							If(Object_Id('Tempdb..#TempUsuarios') is not null)
							Begin
								Drop table #TempUsuarios
							End 

							Create table #TempUsuarios
							(
								id int not null identity,
								Usuar int not null,
								Perfil int not null
							)

							Set @nQuery = 
							'
								Select iCodCatalogo, Perfil
								From ['+@EsquemaActual+'].[VisHistoricos(''Usuar'',''Usuarios'',''Español'')] usuar
								Where dtinivigencia <> dtfinvigencia 
								And dtfinvigencia >= GETDATE()
								AND icodcatalogo in (select usuar from [' + @EsquemaActual + '].RegSesion where convert(date, FecAcc) >= dateadd(mm, -6, getdate()))
							'

							Insert into #TempUsuarios
							Exec(@nQuery)

							Declare @contadorUsuarios int =1
							Declare @nRegUsuarios int = 0

							Select @nRegUsuarios= Count(*)
							From #TempUsuarios

							Declare @usuarActual int =0
							Declare @perfilUsuarActual int = 0


							while(@contadorUsuarios  <= @nRegUsuarios)
							Begin
								Set @usuarActual			=0
								Set @perfilUsuarActual		=0

								Select 
									@usuarActual			 = Usuar,
									@perfilUsuarActual		 = Perfil
								From #TempUsuarios
								Where id = @contadorUsuarios

								Declare @contadorIndicadores  int = 1
								Declare @nRegIndicadores int = 0
								Declare @NombreIndicadorActual varchar(100)

								Select @nRegIndicadores = count(*)
								From #TempIndicadores

								While(@contadorIndicadores <= @nRegIndicadores)
								Begin
				
									select @fechaYHoraActual = GETDATE()

									Select @NombreIndicadorActual = NombreIndicador
									From #TempIndicadores
									Where  id = @contadorIndicadores

									Declare @vFechaIni varchar(30) =  convert(varchar,@FechaIniIndicador,121)
									Declare @vFechaFin varchar(30) = convert(varchar,@FechaFinIndicador,121)

									Exec IndicadorRegeneraIndicadorValue
										@Schema = @EsquemaActual,  
										@FechaIniRep = @vFechaini,  
										@FechaFinRep = @vFechaFin, 
										@Usuario = @usuarActual,
										@Perfil = @perfilUsuarActual ,
										@NombreIndicador  = @NombreIndicadorActual

									insert into Keytia.BitacoraEjecucionLlenaIndicadores
									(
									Esquema,
									Usuar, 
									Indicador,
									Fecha,
									DuracionEjecucionMinutos
									)
									values(
									@EsquemaActual,--Esquema,
									@usuarActual, --Usuar
									@NombreIndicadorActual,--Indicador,
									GETDATE(),--Fecha,
									(select DATEDIFF(mi, @fechaYHoraActual, getdate()))--DuracionEjecucionMinutos
									)

			
									Set @contadorIndicadores  = @contadorIndicadores +1
				
								End 
								Set @contadorUsuarios  = @contadorUsuarios  + 1
							End 


						End 

						Set @contador = @contador + 1
					End 


                ";
            DSODataAccess.ExecuteNonQuery(query);
        }
    }
}
