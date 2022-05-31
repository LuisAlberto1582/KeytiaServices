using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    public partial class ConsumoGeneralMoviles
    {
        DateTime dateNow => new DateTime(DateTime.Now.Year, 1, 1);
        public string GetConsumoPorXAnioActual()
        {
            string Carrier = string.IsNullOrEmpty(param["Carrier"]) ? "-1" : param["Carrier"];
            
            string Query = $@"exec TIMMovilesConsumoPorCarrier
                                @schema ='{DSODataContext.Schema}', 
                                @carrier = {Carrier}, --icodcatalogo del carrier, se envía cero para obtener la info de todos
                                @idioma = 'Español',    
                                @moneda = 'MXP',
                                @usuario = {Session["iCodUsuario"]} ,    
                                @FechaIniRep = '{dateNow:yyyy-MM-dd} 00:00:00',
                                @FechaFinRep = '{DateTime.Now:yyyy-MM-dd} 23:59:59'";
            
            return Query;
        }

        public string Reportehistoricogastointernet()
        {

            string Query = $@"EXEC [dbo].[HistoricoGastoInternet12Meses]
                                @Esquema ='{DSODataContext.Schema}',
                                @FechaIni ='{dateNow:yyyy-MM-dd} 00:00:00',
                                @FechaFin = '{DateTime.Now:yyyy-MM-dd} 23:59:59',
                                @iCodUsuario = {Session["iCodUsuario"]},
                                @iCodPerfil= 367,
                                @Moneda =  'MXP'";
            return Query;
        }

        public string ReporteInternetNacional()
        {
            string Query = $@"EXEC [dbo].[DesgloceConsolidadoApp]
                                @Esquema ='{DSODataContext.Schema}',
                                @FechaIni ='{dateNow:yyyy-MM-dd} 00:00:00',
                                @FechaFin = '{DateTime.Now:yyyy-MM-dd} 23:59:59',
                                @iCodUsuario = {Session["iCodUsuario"]},
                                @iCodPerfil= 367,
                                @TipoConsumo ='Nac',
                                @Moneda='MXP'";
            return Query;
        }

        public string ReporteInternetInternacional()
        {
            string Query = $@"EXEC [dbo].[DesgloceConsolidadoApp]
                                @Esquema ='{DSODataContext.Schema}',
                                @FechaIni ='{dateNow:yyyy-MM-dd} 00:00:00',
                                @FechaFin = '{DateTime.Now:yyyy-MM-dd} 23:59:59',
                                @iCodUsuario = {Session["iCodUsuario"]},
                                @iCodPerfil= 367,
                                @TipoConsumo ='Int',
                                @Moneda='MXP'";
            return Query;
        }


        public string ReporteConceptoAcumulado()
        {
            string Carrier = string.IsNullOrEmpty(param["Carrier"]) ? "-1" : param["Carrier"];
            return $@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAnioActual] @Esquema = '{DSODataContext.Schema}',
                             @iCodCatEmpre = 200002,
                             @iCodCatCarrier = {Carrier}
                             ,@usuario = {Session["iCodUsuario"]}, -- ID del usuario firmado en Keytia
                             @incluirInfoTelFija = 0,
                             @incluirInfoTelMovil = 1
                             , @NvlGlobalPorConcepto = 1";
        }

        public string reporteporconcepto()
        {
            string Carrier = string.IsNullOrEmpty(param["Carrier"]) ? "-1" : param["Carrier"];
            string Empre = string.IsNullOrEmpty("") ? "200002" : param["Carrier"];
            return $@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAnioActual] @Esquema = '{DSODataContext.Schema}',
                     @iCodCatEmpre = 200002,
                     @iCodCatCarrier = -1
                    ,@usuario = {Session["iCodUsuario"]},
                    @incluirInfoTelFija = 0,
                    @incluirInfoTelMovil = 1
                    , @NvlGlobalPorConcepto = 1";
        }

        public string Ahorrosvsmesanterior()
        {
            return $@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses]
                    @Esquema = '{DSODataContext.Schema}',
                         @iCodCatEmpre = 200002,
                         @iCodCatCarrier = -1,
                         @OpcionReporte = 'MesActualvsMesAnterior',
                         @usuario = 79483,
                         @incluirInfoTelFija = 0,
                         @incluirInfoTelMovil = 1,
                         @EnMonedaGlobal = 1
                     , @NvlGlobalPorConcepto = 1";
        }

        public string Ahorrosvsmismomesañoanterior()
        {
            return $@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses]
                    @Esquema = '{DSODataContext.Schema}',
                         @iCodCatEmpre = 200002,
                         @iCodCatCarrier = -1,
                         @OpcionReporte = 'MesActualvsMismoMesAnioAnterior',
                         @usuario = 79483,
                         @incluirInfoTelFija = 0,
                         @incluirInfoTelMovil = 1,
                         @EnMonedaGlobal = 1
                     , @NvlGlobalPorConcepto = 1";
        }


        public string ConsumoGeneralCarrier()
        {
            return $@"EXEC[TIMConsumoGeneralPorConceptoOCarrierAnioActual] @Esquema = '{DSODataContext.Schema}',
                        @iCodCatEmpre = 200002,
                        @iCodCatCarrier = -1,
                        @usuario = 79483,
                        @incluirInfoTelFija = 1,
                        @incluirInfoTelMovil = 1";
        }

        public string AhorrosvsmesanteriorCarrier()
        {
            return $@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses]
     @Esquema = '{DSODataContext.Schema}',
     @iCodCatEmpre = 200002,
     @iCodCatCarrier = -1,
     @OpcionReporte = 'MesActualvsMesAnterior',
     @usuario = 79483,
     @incluirInfoTelFija = 1,
     @incluirInfoTelMovil = 1,
     @EnMonedaGlobal = 1";
        }

        public string AhorrosvsmismomesañoanteriorCarrier()
        {
            return $@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses]
                     @Esquema = '{DSODataContext.Schema}',
                     @iCodCatEmpre = 200002,
                     @iCodCatCarrier = -1,
                     @OpcionReporte = 'MesActualvsMismoMesAnioAnterior',
                     @usuario = {Session["iCodUsuario"]},
                     @incluirInfoTelFija = 1,
                     @incluirInfoTelMovil = 1,
                     @EnMonedaGlobal = 1";
        }

        public string ConsultaRepHistoricoAnioActualVsAnteriorMoviles()
        {
            return $@"
                        exec TIMMovilesConsumoHistoricovsLineas
                        @schema = '{DSODataContext.Schema}',
                        @carrier = 0,
                        @idioma = 'Español',  
                        @moneda = 'MXP',  
                        @usuario =  {Session["iCodUsuario"]},    
                        @FechaIniRep = '{dateNow:yyyy-MM-dd} 00:00:00',
                        @FechaFinRep = '{DateTime.Now:yyyy-MM-dd} 23:59:59'
                        ";
        }

        public string ConsultaEmpleadoLigadoAlUsuario()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("select isNull(Max(iCodCatalogo), -1) as iCodCatalogo \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \n ");
            lsb.Append("and Usuar = " + Session["iCodUsuario"] + " \n ");
            return lsb.ToString();
        }



        private string ObtenerRelacion()
        {
            return $@"
                       EXEC dbo.TIMMovilesConsumoPorCategoriaYPlanTarifario @Esquema = 'bat',
                         @iCodCatEmpre = 200002,
                         @iCodCatCarrier = -1
                    ,@usuario = 79483,
                    @incluirInfoTelFija = 0,
                    @incluirInfoTelMovil = 1
                    , @NvlGlobalPorConcepto = 1
                    ,@FechaPub = '2022-02-01'
                        ";
        }

        private string ReporteConsumoPlanesFacturados()
        {
            return $@"
                       EXEC dbo.[TIMMovilesConsumoPorCategoriaYPlanTarifarioPromedio] @Esquema = 'bat',
                         @iCodCatEmpre = 200002,
                         @iCodCatCarrier = -1
                    ,@usuario = 79483,
                    @incluirInfoTelFija = 0,
                    @incluirInfoTelMovil = 1
                    , @NvlGlobalPorConcepto = 1
                    ,@FechaPub = '2022-02-01'
                        ";
        }

    }
}