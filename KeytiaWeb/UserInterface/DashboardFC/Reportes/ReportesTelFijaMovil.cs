using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KeytiaServiceBL;


namespace KeytiaWeb.UserInterface.DashboardFC.Reportes
{
    public class ReportesTelFijaMovil
    {
        static string Currency => Globals.GetCurrentCurrency();
        static DateTime dateNow => new DateTime(DateTime.Now.Year, 1, 1);
        public static DataTable TIMConsumoGeneralHistorico2Anios(string Carrier, string sessionid, string empre, int incluirInfoTelFija=0, int incluirInfoTelMovil=0)
        {
            return DSODataAccess.Execute($@"
                        EXEC[TIMConsumoGeneralHistorico2Anios]
                        @Esquema = {DSODataContext.Schema},
                        @iCodCatEmpre = {empre},
                        @iCodCatCarrier = {Carrier},
                        @usuario = {sessionid},
                        @incluirInfoTelFija = {incluirInfoTelFija},
                        @incluirInfoTelMovil = {incluirInfoTelMovil},
                        @EnMonedaGlobal = 0");
        }

        public static DataTable ConsultaRepHistoricoAnioActualVsAnteriorMoviles(string Carrier, string sessionid)
        {
            return DSODataAccess.Execute($@"
                        exec TIMMovilesConsumoHistoricovsLineas
                        @schema = '{DSODataContext.Schema}',
                        @carrier = {Carrier},
                        @idioma = 'Español',  
                        @moneda = 'MXP',  
                        @usuario =  {sessionid},    
                        @FechaIniRep = '{dateNow:yyyy-MM-dd} 00:00:00',
                        @FechaFinRep = '{DateTime.Now:yyyy-MM-dd} 23:59:59'
                        ");
        }


        public static DataTable ConsumoGeneral(string Carrier, string sessionid, string empre, int incluirInfoTelFija = 0, int incluirInfoTelMovil = 0, int PorConcepto=0)
        {
            return DSODataAccess.Execute($@"EXEC[TIMConsumoGeneralPorConceptoOCarrierAnioActual] 
                        @Esquema = '{DSODataContext.Schema}',
                        @iCodCatEmpre = {empre},
                        @iCodCatCarrier = {Carrier},
                        @usuario = {sessionid},
                        @incluirInfoTelFija = {incluirInfoTelFija},
                        @incluirInfoTelMovil = {incluirInfoTelMovil}, 
                        @NvlGlobalPorConcepto = {PorConcepto}");
        }

       
        public static DataTable TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses(string Carrier, string sessionid, string empre, string OpcionReporte, int incluirInfoTelFija = 0, int incluirInfoTelMovil = 0, int PorConcepto = 0)
        {
            return DSODataAccess.Execute($@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses]
                       @Esquema = '{DSODataContext.Schema}',
                       @iCodCatEmpre = {empre},
                       @iCodCatCarrier = {Carrier},
                       @OpcionReporte = '{OpcionReporte}',
                       @usuario = {sessionid},
                       @incluirInfoTelFija = {incluirInfoTelFija},
                       @incluirInfoTelMovil = {incluirInfoTelMovil},
                       @EnMonedaGlobal = 0,
                       @NvlGlobalPorConcepto = {PorConcepto}");
        }
        
        public static DataTable AhorrosvsmismomesañoanteriorCarrier(string Carrier, string sessionid, string empre, string OpcionReporte, int incluirInfoTelFija = 0, int incluirInfoTelMovil = 0, int PorConcepto = 0)
        {
            return DSODataAccess.Execute($@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAhorro2Meses]
                       @Esquema = '{DSODataContext.Schema}',
                       @iCodCatEmpre = {empre},
                       @iCodCatCarrier = {Carrier},
                       @OpcionReporte = '{OpcionReporte}',
                       @usuario = {sessionid},
                       @incluirInfoTelFija = {incluirInfoTelFija},
                       @incluirInfoTelMovil = {incluirInfoTelMovil},
                       @EnMonedaGlobal = 0,
                       @NvlGlobalPorConcepto = {PorConcepto}");
        }


        public static DataTable ReporteConceptoAcumulado(string Carrier, string sessionid, string empre, int incluirInfoTelFija = 0, int incluirInfoTelMovil = 0, int PorConcepto = 0)
        {
            return DSODataAccess.Execute($@"EXEC [TIMConsumoGeneralPorConceptoOCarrierAnioActual]
                       @Esquema = '{DSODataContext.Schema}',
                       @iCodCatEmpre = {empre},
                       @iCodCatCarrier = {Carrier},
                       @usuario = {sessionid},
                       @incluirInfoTelFija = {incluirInfoTelFija},
                       @incluirInfoTelMovil = {incluirInfoTelMovil},
                       @NvlGlobalPorConcepto = {PorConcepto}");
        }

         public static DataTable Reportehistoricogastointernet(string sessionid)
        {

            return DSODataAccess.Execute($@"EXEC [dbo].[HistoricoGastoInternet12Meses]
                                @Esquema ='{DSODataContext.Schema}',
                                @FechaIni ='{dateNow:yyyy-MM-dd} 00:00:00',
                                @FechaFin = '{DateTime.Now:yyyy-MM-dd} 23:59:59',
                                @iCodUsuario = {sessionid},
                                @iCodPerfil= 367,
                                @Moneda =  'MXP'");
        }

        public static DataTable ReporteInternetNacIntl(string sessionid, string tipoConsumo)
        {
            return DSODataAccess.Execute($@"EXEC [dbo].[DesgloceConsolidadoApp]
                                @Esquema ='{DSODataContext.Schema}',
                                @FechaIni ='{dateNow:yyyy-MM-dd} 00:00:00',
                                @FechaFin = '{DateTime.Now:yyyy-MM-dd} 23:59:59',
                                @iCodUsuario = {sessionid},
                                @iCodPerfil= 367,
                                @TipoConsumo ='{tipoConsumo}',
                                @Moneda='MXP'");
        }

        public static DataTable TIMMovilesConsumoPorCategoriaYPlanTarifario(string Carrier, string sessionid, string empre, int incluirInfoTelFija = 0, int incluirInfoTelMovil = 0, int PorConcepto = 1)
        {
            return DSODataAccess.Execute($@"EXEC dbo.TIMMovilesConsumoPorCategoriaYPlanTarifario 
                       @Esquema = '{DSODataContext.Schema}',
                       @iCodCatEmpre = {empre},
                       @iCodCatCarrier = {Carrier},
                       @usuario = {sessionid},
                       @incluirInfoTelFija = {incluirInfoTelFija},
                       @incluirInfoTelMovil = {incluirInfoTelMovil},
                       @NvlGlobalPorConcepto = {PorConcepto},
                       @FechaPub = '{dateNow:yyyy-MM-dd}'");
        }

        public static DataTable TIMMovilesConsumoPorCategoriaYPlanTarifarioPromedio(string Carrier, string sessionid, string empre, int incluirInfoTelFija = 0, int incluirInfoTelMovil = 0, int PorConcepto = 1)
        {
            return DSODataAccess.Execute($@"EXEC dbo.[TIMMovilesConsumoPorCategoriaYPlanTarifarioPromedio] 
                      @Esquema = '{DSODataContext.Schema}',
                      @iCodCatEmpre = {empre},
                      @iCodCatCarrier = {Carrier},
                      @usuario = {sessionid},
                      @incluirInfoTelFija = {incluirInfoTelFija},
                      @incluirInfoTelMovil = {incluirInfoTelMovil},
                      @NvlGlobalPorConcepto = {PorConcepto},
                      @FechaPub = '{dateNow:yyyy-MM-dd}'");
        }
    }
}