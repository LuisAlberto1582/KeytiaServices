using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Collections;
using KeytiaServiceBL.Reportes;
using AjaxControlToolkit;
using DSOControls2008;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Web.Services;
using KeytiaWeb.UserInterface.Indicadores;
using KeytiaWeb.Resources;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class Dashboard
    {

        #region Consultas a BD

        private void ConsultaLineaPorCarrier(string carrier)
        {
            //RM 20180918 Se modifica el parametro carrier para que de prioridad al parametro del querystring "param["Carrier"]" y despues al parametro del método "Carrier"
            string Carrier = (param["Carrier"] != string.Empty) ? param["Carrier"] : carrier;

            //Se agrega la consulta de la linea ya que en este punto no la trae el querystring
            StringBuilder consultaLinea = new StringBuilder();
            //if (DSODataContext.Schema.ToUpper() != "K5IPAB") //cambiar por    K5IPAB
            //{
            //    consultaLinea.Append("select vchCodigo\r");
            //    consultaLinea.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] \r");
            //    consultaLinea.Append("where dtIniVigencia <> dtFinVigencia ");
            //    consultaLinea.Append("and convert(varchar,datepart(yyyy,'" + Session["FechaInicio"].ToString() + " 00:00:00')) + convert(varchar,datepart(mm,'" + Session["FechaInicio"].ToString() + " 00:00:00')+12) between convert(varchar,datepart(yyyy,dtinivigencia)) + convert(varchar,datepart(mm,dtinivigencia)+12) and convert(varchar,datepart(yyyy,dtfinvigencia)) + convert(varchar,datepart(mm,dtfinvigencia)+12) \r");
            //    consultaLinea.Append("and SitioCod like '%" + Carrier + "%' \r");
            //    //consultaLinea.Append("and SitioCod like '%" + param["Carrier"] + "%' \r");
            //    consultaLinea.Append("and Emple = " + param["Emple"] + " \r");
            //}
            //else
            //{
            //    consultaLinea.Append(" SELECT LineaCod AS vchCodigo FROM " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Linea','Español')]");
            //    consultaLinea.Append(" WHERE dtIniVigencia <> dtFinVigencia ");
            //    consultaLinea.Append(" AND convert(varchar, datepart(yyyy,'2019-05-01 00:00:00')) + convert(varchar, datepart(mm,'2019-05-31 00:00:00')+12) between convert(varchar, datepart(yyyy, dtinivigencia)) + convert(varchar, datepart(mm, dtinivigencia)+12)");
            //    consultaLinea.Append(" AND convert(varchar, datepart(yyyy, dtfinvigencia)) + convert(varchar, datepart(mm, dtfinvigencia)+12)");
            //    consultaLinea.Append(" AND Emple = " + param["Emple"] + " ");
            //}
            consultaLinea.Append(" SELECT LineaCod AS vchCodigo FROM " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Linea','Español')]");
            consultaLinea.Append(" WHERE dtIniVigencia <> dtFinVigencia ");
            consultaLinea.Append(" AND convert(varchar, datepart(yyyy,'2019-05-01 00:00:00')) + convert(varchar, datepart(mm,'2019-05-31 00:00:00')+12) between convert(varchar, datepart(yyyy, dtinivigencia)) + convert(varchar, datepart(mm, dtinivigencia)+12)");
            consultaLinea.Append(" AND convert(varchar, datepart(yyyy, dtfinvigencia)) + convert(varchar, datepart(mm, dtfinvigencia)+12)");
            consultaLinea.Append(" AND Emple = " + param["Emple"] + " ");
            if(param["Carrier"] != string.Empty)
            {
                consultaLinea.Append("and SitioCod like '%" + Carrier + "%' \r");

            }
            if (param["Linea"] != string.Empty)
            {
                consultaLinea.Append("and vchCodigo = '" + param["Linea"].Replace(" ", "") + "' \r");
            }

            param["Linea"] = DSODataAccess.ExecuteScalar(consultaLinea.ToString()).ToString();
        }

        /// <summary>
        /// Se calcula la fecha máxima que se tomará para las consultas de los reportes
        /// Si la fecha máxima encontrada en el detalle de CDR es mayor a la fecha actual, se tomará
        /// como fecha máxima el día previo al actual
        /// </summary>
        /// <returns>Consulta que regresará la fecha máxima</returns>
        public static string GeneraConsultaMaxFechaInicioCDR()
        {
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine(" \r");
            lsbQuery.AppendLine(" declare @fechaMaximaEnDetall date ");
            lsbQuery.AppendLine("select @fechaMaximaEnDetall = isnull(Max(Fecha), convert(date, getdate())) from FechaMaximaProcesada "); //Fecha máxima sin importar si se trata de telefonía fija o móvil
            lsbQuery.AppendLine("if @fechaMaximaEnDetall > convert(date,getdate()) ");
            lsbQuery.AppendLine("begin ");
            lsbQuery.AppendLine("	select Datepart(year, dateadd(dd,-1,getdate())) as Anio, ");
            lsbQuery.AppendLine("			Datepart(month, dateadd(dd,-1,getdate())) as Mes, ");
            lsbQuery.AppendLine("			Datepart(day, dateadd(dd,-1,getdate())) as Dia ");
            lsbQuery.AppendLine("end ");
            lsbQuery.AppendLine("else ");
            lsbQuery.AppendLine("begin ");
            lsbQuery.AppendLine("	select Datepart(year, @fechaMaximaEnDetall) as Anio, ");
            lsbQuery.AppendLine("			Datepart(month, @fechaMaximaEnDetall) as Mes, ");
            lsbQuery.AppendLine("			Datepart(day, @fechaMaximaEnDetall) as Dia ");
            lsbQuery.AppendLine("end ");

            return lsbQuery.ToString();
        }

        private int ConsultaNumeroLineas(string lsCarrier)
        {
            StringBuilder consultaNumeroDeLineas = new StringBuilder();
            consultaNumeroDeLineas.Append("select count(iCodRegistro) as NumeroLineas \r");
            consultaNumeroDeLineas.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] \r");
            consultaNumeroDeLineas.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
            consultaNumeroDeLineas.Append("and SitioCod like '%" + lsCarrier + "%' \r");
            consultaNumeroDeLineas.Append("and Emple = " + param["Emple"] + " \r");
            int numeroDeLineasTelcel = Convert.ToInt32(DSODataAccess.ExecuteScalar(consultaNumeroDeLineas.ToString()));
            return numeroDeLineasTelcel;
        }

        public string ConsultaConsumoHistorico(string linkGrafica, int omitirInfoCDR = 0, int omitirInfoSiana = 0, int soloLlamadas = 0)
        {
            if (string.IsNullOrEmpty(isFT))
            {
                isFT = "0";
            }

            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @fechaFin varchar(30)\n ");
            lsb.Append("declare @fechaInicio varchar(30)\n ");
            lsb.Append("declare @fechaInicioActual varchar(30)\n ");
            lsb.Append("declare @anio int \n ");
            lsb.Append("declare @mes int\n ");
            lsb.Append("declare @dia int\n ");
            lsb.Append("declare @Where varchar(max) = ''\n");

            if (param["MesAnio"] != string.Empty)
            {
                AjustaFechas();
                lsb.Append("select @Where = @Where + '[Mes Anio] =''" + param["MesAnio"].Replace("-", " ") + "''' \n");
            }

            lsb.Append("set @mes = MONTH(GETDATE())\n ");
            lsb.Append("set @anio = YEAR(GETDATE())\n ");
            lsb.Append("set @dia = DAY(GETDATE())\n ");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 00:00:00'\n ");
            lsb.Append("if @mes < 10\n ");
            lsb.Append("begin\n ");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 00:00:00'\n ");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'\n ");
            lsb.Append("end\n ");
            lsb.Append("else\n ");
            lsb.Append("begin\n ");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 00:00:00'\n ");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'\n ");
            lsb.Append("end\n ");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + ''''\n ");
            lsb.Append("set @fechaFin = '''' + @fechaFin + ''''\n ");
            lsb.Append("exec ConsumoHistoricoOptDashFC    @Schema='" + DSODataContext.Schema + "',\n ");
            if (linkGrafica != "")
            {
                lsb.Append("@Link= '" + linkGrafica + "',\n");
            }
            lsb.Append("@Fields='[Nombre Mes],[Mes Anio] = replace([Mes Anio],'' '',''-''),\n ");
            if (linkGrafica != "")
            {
                /*"[link] = ''" + Request.Path + "?Nav=HistoricoN2&MesAnio='' + replace([Mes Anio],'' '',''-'')"*/
                lsb.Append("[link] = " + linkGrafica + "+ replace([Mes Anio],'' '',''-''), \n ");
            }
            if (soloLlamadas == 1)
            {
                lsb.Append("[Total] = SUM([TotalLlamadas])', \n ");
            }
            else
            {
                lsb.Append("[Total] = ROUND(SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]),2)', \n ");
            }

            lsb.Append("@Where = @Where,\n");
            lsb.Append("@Group = '[Nombre Mes],[Mes Anio]',\n ");/*Se agrega esta columna para el agrupado*/
            lsb.Append("@Order = '[Orden] Asc',\n ");
            lsb.Append("@OrderInv = '[Orden] Desc',\n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            if (soloLlamadas == 1)
            {
                lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
                lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            }
            else
            {
                lsb.Append("@FechaIniRep = @fechaInicio,\n ");
                lsb.Append("@FechaFinRep = @fechaFin,\n ");
            }
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español', \n ");
            lsb.Append("@omitirInfoCDR = " + omitirInfoCDR.ToString() + ", \n ");
            lsb.Append("@omitirInfoSiana = " + omitirInfoSiana.ToString() + ", \n ");
            lsb.Append("@soloLlamadas = " + soloLlamadas.ToString() + ", \n ");
            lsb.Append("@isFT = " + isFT + " \n ");

            return lsb.ToString();
        }

        private void EstablecerBanderasClientePerfil()
        {
            int liBanderasPerfil = 0;

            StringBuilder consulta = new StringBuilder();
            consulta.Append("SELECT BanderasCliente \r");
            consulta.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')] \r");
            consulta.Append("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() \r");
            consulta.Append("AND UsuarDB = " + Session["iCodUsuarioDB"].ToString() + " \r");
            consulta.Append("AND (ISNULL(BanderasCliente,0) & 1024)/1024=1 ");
            DataTable dtConsulta = DSODataAccess.Execute(consulta.ToString());

            Session["MuestraSM"] = (dtConsulta.Rows.Count > 0) ? 1 : 0;

            consulta.Length = 0;
            consulta.Append("SELECT isnull(BanderasPerfil,0) as BanderasPerfil \r");
            consulta.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] \r");
            consulta.Append("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() \r");
            consulta.Append("AND iCodCatalogo = " + Session["iCodPerfil"].ToString() + " \r");
            object loResult = DSODataAccess.ExecuteScalar(consulta.ToString());

            liBanderasPerfil = (loResult != null && loResult != DBNull.Value) ? (int)loResult : 0;

            Session["MuestraCostoSimulado"] = (liBanderasPerfil & 2) == 2 ? 1 : 0;
            Session["TomaDatosDeTablaFija"] = (liBanderasPerfil & 4) == 4 ? 1 : 0;
        }

        public string ConsultaConsumoHistoricoPrs(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @fechaFin varchar(30)\n ");
            lsb.Append("declare @fechaInicio varchar(30)\n ");
            lsb.Append("declare @fechaInicioActual varchar(30)\n ");
            lsb.Append("declare @anio int \n ");
            lsb.Append("declare @mes int\n ");
            lsb.Append("declare @dia int\n ");
            lsb.Append("declare @Where varchar(max) = ''\n");

            if (param["MesAnio"] != string.Empty)
            {
                AjustaFechas();
                lsb.Append("select @Where = @Where + '[Mes Anio] =''" + param["MesAnio"].Replace("-", " ") + "''' \n");
            }

            lsb.Append("set @mes = MONTH(GETDATE())\n ");
            lsb.Append("set @anio = YEAR(GETDATE())\n ");
            lsb.Append("set @dia = DAY(GETDATE())\n ");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'\n ");
            lsb.Append("if @mes < 10\n ");
            lsb.Append("begin\n ");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00'\n ");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'\n ");
            lsb.Append("end\n ");
            lsb.Append("else\n ");
            lsb.Append("begin\n ");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00'\n ");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59'\n ");
            lsb.Append("end\n ");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + ''''\n ");
            lsb.Append("set @fechaFin = '''' + @fechaFin + ''''\n ");
            lsb.Append("exec ConsumoHistoricoOptDashFCPrs    @Schema='" + DSODataContext.Schema + "',\n ");
            lsb.Append("@Fields='[Nombre Mes],[Mes Anio] = replace([Mes Anio],'' '',''-''),\n ");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[TotalSimulado] = ROUND(SUM([CostoFac]) + SUM([CostoSM]),2), \n ");
            lsb.Append("[TotalReal] = ROUND(SUM([Costo]) + SUM([CostoSM]),2), \n ");
            lsb.Append("[CostoSimulado] = ROUND(SUM([CostoFac]),2), \n ");
            lsb.Append("[CostoReal] = ROUND(SUM([Costo]),2), \n ");
            lsb.Append("[SM] = ROUND(SUM([CostoSM]),2)', \n ");
            lsb.Append("@Where = @Where,\n");
            lsb.Append("@Group = '[Nombre Mes],[Mes Anio]',\n ");
            lsb.Append("@Order = '[Orden] Asc',\n ");
            lsb.Append("@OrderInv = '[Orden] Desc',\n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio,\n ");
            lsb.Append("@FechaFinRep = @fechaFin,\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\n ");

            return lsb.ToString();
        }

        public string Consulta12MesesVariacionRenta()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec [dbo].[ObtieneVariacionDeRenta] @Esquema = '"+DSODataContext.Schema+"' , @Fechafin = '" + Session["FechaInicio"].ToString() + "' ");
            return lsb.ToString();
        }

        public string ConsultaConsumoPorSitio(string linkGrafica, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            if (string.IsNullOrEmpty(isFT))
            {
                isFT = "0";
            }

            string sp = (@omitirInfoCDR == 1) ? "[RepTabConsumoPorSitioSianaSeven]" : "RepTabConsumoPorSitioOptDashFC";

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec " + sp + "    @Schema='" + DSODataContext.Schema + "',\n ");
            lsb.Append("			                           @Fields='[Nombre Sitio]=Min(upper([Nombre Sitio])),\n");
            lsb.Append("[Codigo Sitio],\n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),\n");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Numero]=sum([TotalLlamadas]),\n");
            lsb.Append("[Duracion]=sum([Duracion Minutos])', \n");
            lsb.Append("@Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            if(DSODataContext.Schema.ToUpper()=="FCA"&& param["TDest"] != string.Empty)
            {
                lsb.Append("AND [Codigo Tipo Destino] = "+ param["TDest"] + "");
            }
            lsb.Append(" ', @Group = '[Codigo Sitio]', \n");
            lsb.Append("@Order = '[Total] Desc,[Numero] Desc,[Duracion] Desc,[Nombre Sitio] Asc',\n");
            lsb.Append("@OrderInv = '[Total] Asc,[Numero] Asc,[Duracion] Asc,[Nombre Sitio] Desc',\n");
            lsb.Append("@OrderDir = 'Desc',\n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español', \n");
            lsb.Append("@omitirInfoCDR = " + omitirInfoCDR.ToString() + ", \n");
            lsb.Append("@omitirInfoSiana = " + omitirInfoSiana.ToString() + " \n");
            lsb.Append(", @isFT = " + isFT + " \n");


            return lsb.ToString();
        }

        //RJ.20161019 Si se invoca el método desde Dashboard (sin parámetros), se
        //ejecutará un sp más ágil pero que no se puede ejecutar si se requiere recibir parámetros
        public string ConsultaConsumoPorTipoDestino(string linkGrafica, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            int mpleConJer = 0;

            if (param["EmpleConJer"] != null && param["EmpleConJer"] != "")
            {
                mpleConJer = 1;
                Session["ReportePorEmpleConJer"] = 1;
                param["EmpleConJer"] = "1";
            }
            else
            {
                Session["ReportePorEmpleConJer"] = 0;
            }

            if (string.IsNullOrEmpty(isFT))
            {
                isFT = "0";
            }

            /*Obtiene Valor de bandera para saber si desglosara el costo de la llamada o no*/
            ValidaDesgloceCosto();
            int desglosaCosto = 0;
            if (Session["DesgloseCosto"] != null)
            {
                desglosaCosto = Convert.ToInt32(Session["DesgloseCosto"]);
            }


            StringBuilder lsb = new StringBuilder();
            bool utilizarSPConParams = false;


            //NZ 20160921
            if ((DSODataContext.Schema.ToLower() == "k5banorte" ||
                DSODataContext.Schema.ToLower() == "evox") && param["Nav"] == "TpLlamN2")
            {
                lsb.AppendLine("DECLARE @llamsEntrada VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
                lsb.AppendLine("");
                lsb.AppendLine("DECLARE @llamsEnlace VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
            }

            lsb.Append("declare @Where varchar(max) = ''\n");
            lsb.Append(" select @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''\n ");

            //BG.20161129 SE AGREGA FILTRO DE SITIO.
            if (param["Sitio"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "' \n");
                utilizarSPConParams = true;
            }

            if (param["Emple"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Empleado] = " + param["Emple"] + "' \n");
                utilizarSPConParams = true;
            }
            if (param["Carrier"] != string.Empty && DSODataContext.Schema.ToUpper() == "PENTAFON")
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Carrier] = " + param["Carrier"] + "' \n");
                utilizarSPConParams = true;
            }

            if (param["TipoLlam"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Etiqueta] = " + param["TipoLlam"] + "' \n");
                utilizarSPConParams = true;
            }

            if ((DSODataContext.Schema.ToLower() == "k5banorte" ||
                DSODataContext.Schema.ToLower() == "evox") && param["Nav"] == "TpLlamN2")
            {
                lsb.AppendLine("set @Where = @Where + ' AND [Codigo Tipo Destino] <> ' + @llamsEntrada + '");
                lsb.AppendLine("                    AND [Codigo Tipo Destino] <> ' + @llamsEnlace + ''");
            }


            if (DSODataContext.Schema.ToLower() != "k5banorte" && DSODataContext.Schema.ToLower() != "evox")
            {
                lsb.Append("exec RepTabConsumoPorTipoDestinoOptDashFC @Schema='" + DSODataContext.Schema + "',\n ");
            }
            else
            {
                if (utilizarSPConParams)
                {
                    lsb.Append("exec RepTabConsumoPorTipoDestinoOptDashFC @Schema='" + DSODataContext.Schema + "',\n ");
                }
                else
                {
                    lsb.Append("exec RepTabConsumoPoTDestOptDashFC @Schema='" + DSODataContext.Schema + "',\n ");
                }
            }

            if ((DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox"))
            {
                lsb.Append("@Group = '[Codigo Tipo Destino], [Nombre Tipo Destino]', \n");
                lsb.Append("@Fields='[Nombre Tipo Destino]=upper([Nombre Tipo Destino]),\n");
            }
            else
            {
                lsb.Append("@Group = '[Codigo Tipo Destino]', \n");
                lsb.Append("@Fields='[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),\n");
            }

            lsb.Append("[Codigo Tipo Destino],\n");
            /*Validacion cuando un cliente tenga encendida la bandera de desgloso el costo*/
            if (desglosaCosto == 1)
            {
                lsb.Append("[Costo] = convert(money,sum([Costo]/[TipoCambio])),\n");
                lsb.Append("[CostoSM] = sum([CostoSM] /[TipoCambio]),\n");
            }
            /**/

            lsb.Append("[Total]= convert(money,sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio])),\n");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Numero]=SUM([TotalLlamadas]),\n");
            lsb.Append("[Duracion]=sum([Duracion Minutos])', \n");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc',\n");
            lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc',\n");
            lsb.Append("@OrderDir = 'Desc',\n");

            /*Validar si se manda a llamar del reporte ReportePorEmpleConJer */
            if (mpleConJer == 0)
            {
                lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
                lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            }

            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");


            if (omitirInfoCDR != 0 || omitirInfoSiana != 0)
            {
                lsb.Append("@omitirInfoCDR = " + omitirInfoCDR.ToString() + ",\n ");
                lsb.Append("@omitirInfoSiana = " + omitirInfoSiana.ToString() + ",\n ");
            }

            lsb.Append("@Idioma = 'Español'\n");

            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                lsb.Append(", @isFT = " + isFT + " \n");
            }

            if (param["Indicador"] == "1")
            {
                lsb.AppendLine(", @esIndicador = 1");
            }
            return lsb.ToString();
        }
        private int ValidaDesgloceCosto()
        {
            int desgloceCosto = 0;

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine(" DECLARE @Empre INT");
            lsb.AppendLine(" SELECT @Empre = Empre FROM " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','Español')]");
            lsb.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            lsb.AppendLine(" AND iCodCatalogo = " + Session["iCodUsuario"] + "");
            lsb.AppendLine(" select((IsNull(BanderasEmpre,0) & 4) / 4) AS DesglosarCosto");
            lsb.AppendLine(" from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','Español')]");
            lsb.AppendLine(" where dtinivigencia<> dtfinvigencia");
            lsb.AppendLine(" and dtfinvigencia >= getdate()");
            lsb.AppendLine(" and icodcatalogo = @Empre");
            DataTable dt = DSODataAccess.Execute(lsb.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                desgloceCosto = Convert.ToInt32(dr["DesglosarCosto"]);
                Session["DesgloseCosto"] = desgloceCosto;
            }

            return desgloceCosto;
        }
        public string ConsultaConsumoPorCenCosJerarquico(string linkGrafica)
        {
            string iCodCenCosPadre = string.Empty;
            if (param["CenCos"] != string.Empty)
            {
                iCodCenCosPadre = param["CenCos"];
            }
            else
            {
                iCodCenCosPadre = "0";
            }

            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = ''\n");
            lsb.Append("			                         select  @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append("                                                and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''\n ");
            lsb.Append("exec spAcumuladoXCenCosRestDashFC @Schema='" + DSODataContext.Schema + "',\n ");
            lsb.Append("			                           @Fields='[Nombre Centro de Costos] = Min(upper([Nombre Centro de Costos])),\n");
            lsb.Append("[Codigo Centro de Costos],\n");
            lsb.Append("[TipoDetalleRuta] = Min([TipoDetalle]),\n");
            lsb.Append("[Total] = sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),\n");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("TotLlamadas = sum([TotalLlamadas]),\n");
            lsb.Append("[Duracion Minutos] = sum([Duracion Minutos])', \n");
            lsb.Append("@Where = @Where,\n");
            lsb.Append("@Group = '[Codigo Centro de Costos]', \n");
            lsb.Append("@Order = '[Total] Desc,[TotLlamadas] Desc,[Duracion Minutos] Desc,[Nombre Centro de Costos] Asc',\n");
            lsb.Append("@OrderInv = '[Total] Asc,[TotLlamadas] Asc,[Duracion Minutos] Asc,[Nombre Centro de Costos] Desc',\n");
            lsb.Append("@OrderDir = 'Desc',\n");
            lsb.Append("@iCodCenCosPadre = " + iCodCenCosPadre + ",\n");
            lsb.Append("@Jerarquia = 1,\n");
            lsb.Append("@IncluirPadre = 1,\n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\n");
            return lsb.ToString();
        }

        public string ConsultaPorEmpleMasCaros(string linkGrafica, int numeroRegistros, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            bool utilizarSPConParams = false;
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = ''\n");
            lsb.Append(" select @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''\n ");

            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Centro de Costos] = " + param["CenCos"] + "' \n");
                utilizarSPConParams = true;
            }

            if (param["Sitio"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "' \n");
                utilizarSPConParams = true;
            }

            if (param["TDest"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Tipo Destino] =''" + param["TDest"] + "''' \n");
                utilizarSPConParams = true;
            }

            if (param["Carrier"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Carrier] =''" + param["Carrier"] + "''' \n");
                utilizarSPConParams = true;
            }


            if (DSODataContext.Schema.ToLower() != "k5banorte")
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN" && param["Nav"] == "CenCosJerarquicoN3")
                {
                    lsb.Append("exec dbo.RepTabConsumoEmpsMasCarosSPSinLineaDashFCSEVEN @Schema='" + DSODataContext.Schema + "',\n ");
                }
                else
                {
                    lsb.Append("exec [RepTabConsumoEmpsMasCarosSPSinLineaDashFC] @Schema='" + DSODataContext.Schema + "',\n ");
                }


                lsb.Append("@omitirInfoCDR = " + omitirInfoCDR.ToString() + ",\n");
                lsb.Append("@omitirInfoSiana = " + omitirInfoSiana.ToString() + ",\n");

            }
            else
            {
                //RJ.Dependiendo de si el sp recibirá parámtros se utiliza uno u otro. El del Dashboard es más rápido
                //pues solo contiene los campos y agrupacion que utiliza el primer nivel
                if (utilizarSPConParams)
                {
                    lsb.Append("exec [BnrtRepTabConsumoEmpsMasCarosSPSinLineaDashFC] @Schema='" + DSODataContext.Schema + "',\n ");
                }
                else
                {
                    lsb.Append("exec [BnrtRepTabConsumoEmpsMasCarosDashboardFC] @Schema='" + DSODataContext.Schema + "',\n ");
                }
            }



            /*BG.20161124 se agrega validacion de esquema */
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox" */ )
            {
                lsb.Append("@Fields='[No Nomina], \n");
                lsb.Append("[Nombre Completo]=Min(upper([Nombre Completo])), \n");
                lsb.Append("[Codigo Empleado],\n");
                lsb.Append("[Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])),\n");
                lsb.Append("[Codigo Centro de Costos],\n");
            }
            else
            {
                lsb.AppendLine("@Fields =' ");
                if (DSODataContext.Schema.ToLower() == "institutomora")
                {
                    lsb.Append("[Extension],");
                }
                else
                {
                    lsb.Append("[Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])),\n");
                    lsb.Append("[Codigo Centro de Costos],\n");
                }

                lsb.Append("[Nombre Completo]=Min(upper([Nombre Completo])), \n");
                lsb.Append("[Codigo Empleado],\n");
            }


            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("            [Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),\n");
            lsb.Append("            [Numero]=SUM([TotalLlamadas]),\n");



            //NZ 20160823 
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
            {
                lsb.Append("        [Duracion]=sum([Duracion Minutos]), \n");
                lsb.Append("        [Puesto]',\n");
            }
            else
            {
                lsb.Append("        [Duracion]=sum([Duracion Minutos])', \n");
            }

            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Codigo Empleado], \n");


            //NZ 20160823 
            /*BG.20161113 se agrega Nomina */
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox" */ )
            {
                lsb.Append("        [No Nomina], \n");
                lsb.Append("        [Codigo Centro de Costos], \n");
                lsb.Append("        [Puesto]', \n");
            }
            else
            {


                if (DSODataContext.Schema.ToLower() == "institutomora")
                {
                    lsb.Append("[Codigo Centro de Costos],[Extension] ',\n");
                }
                else
                {
                    lsb.Append("        [Codigo Centro de Costos]', \n");
                }

            }

            lsb.Append("@Order = '[Total] Desc',\n");
            lsb.Append("@OrderInv = '[Total] Asc',\n");
            lsb.Append("@OrderDir = 'Asc',\n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español',\n");
            lsb.Append("@Lenght = " + numeroRegistros.ToString() + "\n");

            if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN" && param["Nav"] == "CenCosJerarquicoN3")
            {
                lsb.Append(",@Cencos = " + param["CenCos"] + "\n");
            }
            return lsb.ToString();
        }

        public string ConsultaPorEmpleMasCarosConSitio(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = ''\n");
            lsb.Append("			                           select @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append("                                                and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''\n ");

            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Centro de Costos] = " + param["CenCos"] + "' \n");
            }

            if (param["Sitio"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "' \n");
            }

            if (param["TDest"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Tipo Destino] =''" + param["TDest"] + "''' \n");
            }

            if (param["Carrier"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Carrier] =''" + param["Carrier"] + "''' \n");
            }

            lsb.Append("exec RepTabConsumoEmpsMasCarosSPSinLineaDashFC @Schema='" + DSODataContext.Schema + "',\n ");
            lsb.Append("@Fields='[Nombre Completo]=Min(upper([Nombre Completo])), [ExtensionEmple], Sitio, \n");
            lsb.Append("[Codigo Empleado],\n");
            lsb.Append("[Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])),\n");
            lsb.Append("[Codigo Centro de Costos],\n");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),\n");
            lsb.Append("[Numero]=SUM([TotalLlamadas]),\n");

            //NZ 20160823 
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
            {
                lsb.Append("[Duracion]=sum([Duracion Minutos]), \n");
                lsb.Append("[Puesto]',\n");
            }
            else
            {
                lsb.Append("[Duracion]=sum([Duracion Minutos])', \n");
            }

            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Codigo Empleado], [ExtensionEmple], Sitio, \n");

            //NZ 20160823 
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
            {
                lsb.Append("[Codigo Centro de Costos], \n");
                lsb.Append("[Puesto]', \n");
            }
            else
            {
                lsb.Append("[Codigo Centro de Costos]', \n");
            }

            lsb.Append("@Order = '[Total] Desc',\n");
            lsb.Append("@OrderInv = '[Total] Asc',\n");
            lsb.Append("@OrderDir = 'Asc',\n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\n");
            return lsb.ToString();
        }

        public string ConsultaPorCenCos(string linkGrafica)
        {
            bool tieneParametros = false;

            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = ''\n");
            lsb.Append("    select  @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append("    and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n");
            lsb.Append("   '\n ");

            if (param["Sitio"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Sitio] =''" + param["Sitio"] + "''' \n");
                tieneParametros = true;
            }

            if (param["TDest"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Tipo Destino] =''" + param["TDest"] + "''' \n");
                tieneParametros = true;
            }

            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Centro de Costos] = " + param["CenCos"] + "' \n");
                tieneParametros = true;
            }

            if (param["Carrier"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Carrier] = " + param["Carrier"] + "' \n");
                tieneParametros = true;
            }

            //RJ.20161013 En Banorte se hizo un cambio para que muestre todos los CenCos
            //independientemente de que tengan consumo o no.
            if (DSODataContext.Schema.ToLower() != "banorte" &&
                DSODataContext.Schema.ToLower() != "k5banorte")
            {
                lsb.Append("exec ConsumoAcumuladoTodosCamposRestDashFC \r");
            }
            else
            {
                if (!tieneParametros)
                {
                    lsb.Append("exec ConsumoAcumuladoPorCenCosSinJerarqFC \r"); //Sp optimizado
                }
                else
                {
                    lsb.Append("exec ConsumoAcumuladoTodosCamposRestDashFC \r");
                }
            }


            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields=' [Codigo Centro de Costos],[Centro de Costos]=Min(upper([Nombre Centro de Costos])), \r");
            lsb.Append("[Total] = convert(numeric(15,2),sum(Costo/[TipoCambio])+sum(CostoSM/[TipoCambio])), \r");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("LLamadas = sum([TotalLlamadas]), \r");
            lsb.Append("[Minutos] = SUM([Duracion Minutos])',  \r");
            lsb.Append("@Where = @Where, \r");
            lsb.Append("@Group = '[Codigo Centro de Costos]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[LLamadas] Desc,[Centro de Costos] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[LLamadas] Asc,[Centro de Costos] Desc', \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }
        public string ConsultaPorSitio(string linkGrafica)
        {
            bool tieneParametros = false;

            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = ''\n");
            lsb.Append("    select  @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append("    and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n");
            lsb.Append("   '\n ");

            if (param["Sitio"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Sitio] =''" + param["Sitio"] + "''' \n");
                tieneParametros = true;
            }

            if (param["TDest"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Tipo Destino] =''" + param["TDest"] + "''' \n");
                tieneParametros = true;
            }

            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Centro de Costos] = " + param["CenCos"] + "' \n");
                tieneParametros = true;
            }

            if (param["Carrier"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Carrier] = " + param["Carrier"] + "' \n");
                tieneParametros = true;
            }

            //RJ.20161013 En Banorte se hizo un cambio para que muestre todos los CenCos
            //independientemente de que tengan consumo o no.
            if (DSODataContext.Schema.ToLower() != "banorte" &&
                DSODataContext.Schema.ToLower() != "k5banorte")
            {
                lsb.Append("exec ConsumoAcumuladoTodosCamposRestDashFC \r");
            }
            else
            {
                if (!tieneParametros)
                {
                    lsb.Append("exec ConsumoAcumuladoPorCenCosSinJerarqFC \r"); //Sp optimizado
                }
                else
                {
                    lsb.Append("exec ConsumoAcumuladoTodosCamposRestDashFC \r");
                }
            }


            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Codigo Sitio],[Sitio]=Min(upper([Nombre Sitio])), \r");
            lsb.Append("[Total] = convert(numeric(15,2),sum(Costo/[TipoCambio])+sum(CostoSM/[TipoCambio])), \r");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("LLamadas = sum([TotalLlamadas]), \r");
            lsb.Append("[Minutos] = SUM([Duracion Minutos])',  \r");
            lsb.Append("@Where = @Where, \r");
            lsb.Append("@Group = '[Codigo Sitio]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[LLamadas] Desc,[Codigo Sitio] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[LLamadas] Asc,[Codigo Sitio] Desc', \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        public static string ConsultaDetalle()
        {
            string nombreSP = "ConsumoDetalladoDashFC";
            int mpleConJer = 0;
            if (HttpContext.Current.Session["ReportePorEmpleConJer"] != null)
            {
                mpleConJer = Convert.ToInt32(HttpContext.Current.Session["ReportePorEmpleConJer"]);
                HttpContext.Current.Session["ReportePorEmpleConJer"] = 0;
            }

            //Se utiliza para comparar el tdest actual con call y ld para seven
            int tdestSevenEleven = 0;
            int omitirInfoCDR = 0;
            int omitirInfoSiana = 0;
            if (param["omitirInfoCDR"] != string.Empty || param["omitirInfoSiana"] != string.Empty)
            {
                omitirInfoCDR = Convert.ToInt32(param["omitirInfoCDR"]);
                omitirInfoSiana = Convert.ToInt32(param["omitirInfoSiana"]);
                HttpContext.Current.Session["omitirInfoCDR"] = omitirInfoCDR;
            }


            int.TryParse(param["TDest"].ToString(), out tdestSevenEleven);

            if
            (
                //Se comenta validacion porque deberia de servir para todos los clientes
                //DSODataContext.Schema.ToLower() == "seveneleven" &&
                (
                    tdestSevenEleven == 387 || //CelNac
                    tdestSevenEleven == 386 || //CelLoc
                    tdestSevenEleven == 389 || //LDM
                    tdestSevenEleven == 388 || //LDInt
                    tdestSevenEleven == 383    //800E

                )
            )
            {
                nombreSP = "ConsultaDetalleSevenElevenCellLD";
            }

            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine("declare @Where varchar(max)= ''");
            lsb.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.

            if (param["Sitio"] != string.Empty)
            {
                string sitio = FCAndControls.AgregaEtiqueta(new string[] { param["Sitio"] });
                if (sitio.ToLower().Replace(" ", "").Contains("telcel"))
                {
                    lsb.AppendLine("select @Where = @Where + ' AND ([Codigo Sitio] =''" + param["Sitio"] + "'' OR [Tipo de destino] like ''%" + param["Sitio"] + "%'')'");
                }
                else
                {
                    lsb.AppendLine("select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "'");
                }
            }

            if (param["CenCos"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [Codigo CenCos] = " + param["CenCos"] + "'");
            }

            if (param["Emple"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [iCodEmple] = " + param["Emple"] + "'");
            }

            if (param["TDest"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [iCodTDest] = " + param["TDest"] + "'");
            }

            if (param["TipoLlam"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [GEtiqueta] = " + param["TipoLlam"] + "'");
            }

            if (param["Carrier"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [Codigo Carrier] = " + param["Carrier"] + "'");
            }

            if (param["NumGpoTronk"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [Codigo GpoTroncal] = " + param["NumGpoTronk"] + "'");
            }

            if (param["Locali"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [iCodLocali] = " + param["Locali"] + "'");
            }

            if (param["NumMarc"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [Numero Marcado] = '''''''' + ''" + param["NumMarc"] + "'''");  //NZ Se le concatena la comilla con las que lo arroja el sql.
            }

            if (param["Nav"] == "RepLlamadasPerdidasPorTDestN2" || param["Nav"] == "RepLlamadasPerdidasPorSitioN2" || param["Nav"] == "RepLlamadasPerdidasPorTopEmpleN2" || param["Nav"] == "RepLlamadasPerdidasPorCencosN3")
            {
                lsb.AppendLine("select @Where = @Where + ' AND [Duracion] = 0'");
            }
            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                lsb.AppendLine(@"select @Where = @Where + ' and [Codigo Tipo Destino] IN (select iCodCatalogo from " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')] where dtfinvigencia>=getdate() and vchcodigo in (''Ent'',''EntPorDesvio'',''Enl'',''EnlPorDesvio'',''ExtExt'', ''ExtExtPorDesvio'')) '");
            }
            if (WhereAdicional != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND " + WhereAdicional + "'"); //NZ Se agrega esta variable con una variable que pueda contener cualquier otro tipo de filtro que un metodo especifico ocupe.

            }

            lsb.AppendLine("exec [" + nombreSP + "]");
            lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Fields='");
            lsb.AppendLine("[Centro de costos],");
            lsb.AppendLine("[Colaborador],");
            lsb.AppendLine("[Nomina],");
            lsb.AppendLine("[Extensión]	 ,");
            lsb.AppendLine("[Fecha],");

            if (DSODataContext.Schema.ToUpper() != "SEVENELEVEN" && omitirInfoCDR != 1)
            {
                lsb.AppendLine("[Numero Marcado],");
                lsb.AppendLine("[Hora],");
                lsb.AppendLine("[Fecha Fin],");
                lsb.AppendLine("[Hora Fin],");
                lsb.AppendLine("[SM] = (CostoSM),");
                lsb.AppendLine("[Tipo Llamada],");
            }
            lsb.AppendLine("[Duracion],");
            lsb.AppendLine("[Llamadas],");
            lsb.AppendLine("[TotalSimulado] = CONVERT(MONEY,Round((CostoFac+CostoSM),2)),");
            lsb.AppendLine("[TotalReal] = CONVERT(MONEY,Round((Costo+CostoSM),2)),");
            lsb.AppendLine("[CostoSimulado] = CONVERT(MONEY,ROUND((CostoFac),2)),");
            lsb.AppendLine("[CostoReal] = CONVERT(MONEY,ROUND((Costo),2)),");
            lsb.AppendLine("[Nombre Sitio]	as [Sitio],");
            lsb.AppendLine("[Nombre Carrier] as [Carrier]");


            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                lsb.AppendLine(",[Codigo Autorizacion]  = CASE WHEN LEN([Codigo Autorizacion]) > 0 THEN ''***'' + RIGHT([Codigo Autorizacion],3) ELSE [Codigo Autorizacion] END,");
                lsb.AppendLine("[Etiqueta],");
                lsb.AppendLine("[Tipo de destino],");
                lsb.AppendLine("[Puesto]");
            }
            else
            {
                if (DSODataContext.Schema.ToUpper() != "SEVENELEVEN")
                {
                    lsb.AppendLine(",[Codigo Autorizacion],");
                    lsb.AppendLine("[Nombre Localidad] as [Localidad],");
                    lsb.AppendLine("[Tipo de destino]");
                }
                else
                {
                    if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN" && omitirInfoCDR == 1)
                    {
                        lsb.AppendLine(",[Tipo de destino] AS [Tipo de Servicio]");
                    }
                    else
                    {
                        lsb.AppendLine(",[Tipo de destino]");
                    }
                }
            }


            lsb.AppendLine("',");
            lsb.AppendLine("@Where = @Where,");
            ////lsb.AppendLine("@Order = '[TotalSimulado] desc',"); SE QUITA EL ORDENAMIENTO DEL LADO DEL SQL
            ////lsb.AppendLine("@OrderDir = 'Asc',");
            //if (mpleConJer == 0)
            //{
            //    lsb.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            //}

            lsb.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            lsb.AppendLine("@Idioma = '" + HttpContext.Current.Session["Language"] + "',");
            lsb.AppendLine("@Moneda = '" + HttpContext.Current.Session["Currency"] + "',\n ");
            //20181210 RM Cambio para que se respeten las fechas del mes actual si viene de un indicador            

            lsb.AppendLine("@FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00',");
            lsb.AppendLine("@FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'");

            if (param["Indicador"] != null && param["Indicador"].ToString().Length > 0)
            {
                lsb.AppendLine(", @esIndicador = 1");
            }

            if (omitirInfoCDR != 0 || omitirInfoSiana != 0)
            {
                lsb.Append(",@omitirInfoCDR = " + omitirInfoCDR.ToString() + ",\n ");
                lsb.Append("@omitirInfoSiana = " + omitirInfoSiana.ToString() + "\n ");
            }

            WhereAdicional = string.Empty;  //NZ Se pone en esta sección, para limpiar la variable exactamente despues de ser usada. //Se limpia de nuevo para que no afecte a otros reportes

            return lsb.ToString();
        }

        public string ConsultaNumCenCosHijos(string CenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("select count(iCodRegistro) as NumCenCosHijos from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','Español')]\n ");
            lsb.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()\n ");
            lsb.Append("and CenCos = " + param["CenCos"] + "\n ");

            return lsb.ToString();
        }

        public string ConsultaDescripcionDeNavegacionCenCosJer(string iCodCatalogo)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Query nvarchar(max)\n ");
            lsb.Append("declare @QueryInsert varchar(max)\n ");
            lsb.Append("declare @iCodCenCos varchar(100) = '" + iCodCatalogo + "'\n ");
            lsb.Append("declare @Esquema varchar(200) = '" + DSODataContext.Schema + "'\n ");
            lsb.Append("declare @iCodCatalogo varchar(200) = '0'\n ");
            lsb.Append("declare @veces int\n ");
            lsb.Append("declare @Descripcion varchar(max)\n ");
            lsb.Append("declare @stringFinal varchar(max) = ''\n ");
            lsb.Append("create table #temp\n ");
            lsb.Append("(\n ");
            lsb.Append("	ID int identity not null,\n ");
            lsb.Append("	Descripcion varchar(max)\n ");
            lsb.Append(")\n ");
            lsb.Append("set @QueryInsert = '\n ");

            lsb.Append("select Descripcion  = Case When '''+@Esquema+''' <> ''Bimbo'' Then Descripcion Else Descripcion + '' (''+vchCodigo+'') ''end\n ");
            lsb.Append("from '+@Esquema+'.[VisHistoricos(''CenCos'',''Centro de Costos'',''Español'')]\n ");
            lsb.Append("where dtIniVigencia <> dtFinVigencia \n ");
            lsb.Append("and dtFinVigencia >= getdate() \n ");
            lsb.Append("and iCodCatalogo = ' + @iCodCenCos \n ");
            lsb.Append("\n ");
            lsb.Append("insert into #temp\n ");
            lsb.Append("exec (@QueryInsert)\n ");
            lsb.Append("\n ");
            lsb.Append("while(@iCodCatalogo is not null)\n ");
            lsb.Append("begin \n ");
            lsb.Append("		set @Query = '\n ");
            lsb.Append("				select @iCodCatalogo = CenCos \n ");
            lsb.Append("                from '+@Esquema+'.[VisHistoricos(''CenCos'',''Centro de Costos'',''Español'')] \n ");
            lsb.Append("				where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \n ");
            lsb.Append("				and iCodCatalogo = ' + @iCodCenCos \n ");
            lsb.Append("\n ");
            lsb.Append("		EXEC SP_EXECUTESQL @Query, \n ");
            lsb.Append("						   N'@iCodCatalogo int OUTPUT',\n ");
            lsb.Append("						   @iCodCatalogo OUTPUT				   	  \n ");
            lsb.Append("		set @QueryInsert = '\n ");
            lsb.Append("				select Descripcion = Case When '''+@Esquema+''' <> ''Bimbo'' Then Descripcion Else Descripcion + '' (''+vchCodigo+'') ''end \n ");
            lsb.Append("                from '+@Esquema+'.[VisHistoricos(''CenCos'',''Centro de Costos'',''Español'')] \n ");
            lsb.Append("                where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \n ");
            lsb.Append("				and iCodCatalogo = ' + @iCodCatalogo 			   	   \n ");
            lsb.Append("		insert into #temp\n ");
            lsb.Append("		exec (@QueryInsert)\n ");
            lsb.Append("		select @iCodCenCos = @iCodCatalogo\n ");
            lsb.Append("end \n ");
            lsb.Append("select @veces = max(ID) from #temp\n ");
            lsb.Append("\n ");
            lsb.Append("while(@veces <> 0)\n ");
            lsb.Append("begin \n ");
            lsb.Append("	select @Descripcion = Descripcion \n ");
            lsb.Append("	from #temp where ID = @veces\n ");
            lsb.Append("\n ");
            lsb.Append("	if(@Descripcion is not null)\n ");
            lsb.Append("	begin\n ");
            lsb.Append("		set @stringFinal = @stringFinal + @Descripcion + ' / '\n ");
            lsb.Append("	end\n ");
            lsb.Append("\n ");
            lsb.Append("	select @veces = @veces - 1\n ");
            lsb.Append("\n ");
            lsb.Append("end\n ");
            lsb.Append("select substring(@stringFinal,1,len(@stringFinal)-1)\n ");
            lsb.Append("drop table #temp\n ");
            lsb.Append("	   \n ");
            lsb.Append("\n ");
            return lsb.ToString();
        }

        /// <summary>
        /// RJ.Este método sólo se debe invocar desde el reporte de 1 panel, es decir, desde el Dashboard
        /// ya que no está preparado para recibir parámetros
        /// </summary>
        /// <param name="linkGrafica"></param>
        /// <returns></returns>
        public string GeneraConsultaPorTpLlamada1Pnl(string linkGrafica)
        {
            if (string.IsNullOrEmpty(isFT))
            {
                isFT = "0";
            }

            StringBuilder lsb = new StringBuilder();

            //NZ 20160921
            if ((DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox"))
            {
                lsb.AppendLine("DECLARE @llamsEntrada VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
                lsb.AppendLine("");
                lsb.AppendLine("DECLARE @llamsEnlace VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
            }

            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @ParamSitio varchar(max) \n ");

            lsb.Append("set @ParamEmple = 'null' \n ");
            lsb.Append("set @ParamSitio = 'null' \n ");

            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamSitio <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+') \n ");
            lsb.Append("' \n ");

            if ((DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox"))
            {
                lsb.AppendLine("set @Where = @Where + ' AND [Codigo Tipo Destino] <> ' + @llamsEntrada + '");
                lsb.AppendLine("                    AND [Codigo Tipo Destino] <> ' + @llamsEnlace + ''");
            }

            lsb.Append("exec RepTabConsumoPorTipoLlamadaFC     @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Clave Tipo Llamada],[Tipo Llamada]=Min([Tipo Llamada]), \n ");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))',  \n ");
            lsb.Append("@Orderdir='desc', \n ");
            lsb.Append("@Order='[Total] desc', \n ");
            lsb.Append("@OrderInv='[Total] desc', \n ");
            lsb.Append("@Group = '[Clave Tipo Llamada]', \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            lsb.Append(", @isFT = " + isFT + " \n ");

            return lsb.ToString();
        }

        public string ConsultaConsumoPorSitioMat(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec spAcumuladoMatrizRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@InnerFields='[Nombre Sitio]=MIN(upper([Nombre Sitio])), \n ");
            lsb.Append("[Codigo Sitio], \n ");
            lsb.Append("[Nombre Tipo Destino]=MIN(upper([Nombre Tipo Destino])), \n ");
            lsb.Append("[Codigo Tipo Destino], \n ");
            lsb.Append("TotalCosto = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio])',  \n ");
            lsb.Append("@InnerWhere= 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            if (param["Carrier"] != string.Empty)
            {
                lsb.Append(" and [Codigo Carrier] = " + param["Carrier"] + "  \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@InnerGroup='[Codigo Sitio], \n ");
            lsb.Append("[Codigo Tipo Destino]',  \n ");
            lsb.Append("@OuterFields='[Codigo Sitio],[Nombre Sitio], \n ");
            lsb.Append(ArmaCamposConsumoPorSitioMat());

            //lsb.Append("[TotalCosto] = SUM([TotalCosto])',  \n ");
            lsb.Append("[TotalCosto] = SUM([TotalCosto]) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@OuterGroup='[Nombre Sitio], \n ");
            lsb.Append("[Codigo Sitio]', \n ");
            lsb.Append("@Order='[Nombre Sitio] Asc,[TotalCosto] Desc', \n ");
            lsb.Append("@OrderInv='[Nombre Sitio] Desc,[TotalCosto] Asc', \n ");
            lsb.Append("@OrderDir='Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");

            return lsb.ToString();
        }

        public string ConsultaCamposConsumoPorSitioMat()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec spAcumuladoMatrizRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@InnerFields='[Nombre Tipo Destino]=MIN(upper([Nombre Tipo Destino])), \n ");
            lsb.Append("[Codigo Tipo Destino]',       \n ");
            lsb.Append("@InnerWhere= 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append(" and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''',  \n ");
            lsb.Append("@InnerGroup='[Codigo Tipo Destino]',       \n ");
            lsb.Append("@Order='[Nombre Tipo Destino] Asc',       \n ");
            lsb.Append("@OrderInv='[Nombre Tipo Destino] Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");

            return lsb.ToString();
        }

        public string ArmaCamposConsumoPorSitioMat()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoPorSitioMat());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Tipo Destino"] + "] = SUM(case when [Nombre Tipo Destino] = ''" + ldr["Nombre Tipo Destino"] + "'' AND [Codigo Tipo Destino] = " + ldr["Codigo Tipo Destino"] + " then [TotalCosto] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string GeneraStringCamposConsumoPorSitioMat()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaObtieneCamposConsumoPorSitioMat());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Tipo Destino"] + "] = SUM(case when [Nombre Tipo Destino] = ''" + ldr["Nombre Tipo Destino"] + "'' AND [Codigo Tipo Destino] = " + ldr["Codigo Tipo Destino"] + " then [TotalCosto] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaObtieneCamposConsumoPorSitioMat()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec ObtieneTiposDestino @esquema = '" + DSODataContext.Schema + "' \n ");


            return lsb.ToString();
        }

        public string ConsultaConsumoPorCarrierMat(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec spAcumuladoPorCarrierMatrizRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@InnerFields='[Nombre Carrier]=MIN(upper([Nombre Carrier])), \n ");
            lsb.Append("[Codigo Carrier], \n ");
            lsb.Append("[Nombre Tipo Destino]=MIN(upper([Nombre Tipo Destino])), \n ");
            lsb.Append("[Codigo Tipo Destino], \n ");
            lsb.Append("TotalCosto = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio])',  \n ");
            lsb.Append("@InnerWhere= 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''',  \n ");
            lsb.Append("@InnerGroup='[Codigo Carrier], \n ");
            lsb.Append("[Codigo Tipo Destino]',  \n ");
            lsb.Append("@OuterFields='[Codigo Carrier], [Nombre Carrier], \n ");
            lsb.Append(ArmaCamposConsumoPorCarrierMat());
            lsb.Append("[TotalCosto] = SUM([TotalCosto]) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@OuterGroup='[Nombre Carrier], \n ");
            lsb.Append("[Codigo Carrier]', \n ");
            lsb.Append("@Order='[Nombre Carrier] Asc,[TotalCosto] Desc', \n ");
            lsb.Append("@OrderInv='[Nombre Carrier] Desc,[TotalCosto] Asc', \n ");
            lsb.Append("@OrderDir='Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaCamposConsumoPorCarrierMat()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec spAcumuladoPorCarrierMatrizRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@InnerFields='[Nombre Tipo Destino]=MIN(upper([Nombre Tipo Destino])), \n ");
            lsb.Append("[Codigo Tipo Destino]',       \n ");
            lsb.Append("@InnerWhere= 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''',  \n ");
            lsb.Append("@InnerGroup='[Codigo Tipo Destino]',       \n ");
            lsb.Append("@Order='[Nombre Tipo Destino] Asc',       \n ");
            lsb.Append("@OrderInv='[Nombre Tipo Destino] Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");

            return lsb.ToString();
        }

        public string ArmaCamposConsumoPorCarrierMat()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoPorCarrierMat());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Tipo Destino"] + "] = SUM(case when [Nombre Tipo Destino] = ''" + ldr["Nombre Tipo Destino"] + "'' AND [Codigo Tipo Destino] = " + ldr["Codigo Tipo Destino"] + " then [TotalCosto] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaRepHistPorDiaDeLaSemana()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ProsaConsumoPorDiaDeLaSemana   @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[DiaSemana], \n ");
            lsb.Append("[Total]',  \n ");
            lsb.Append("@Orderdir='desc', \n ");
            lsb.Append("@Order='[Total] desc', \n ");
            lsb.Append("@OrderInv='[Total] desc', \n ");
            lsb.Append("@Group = '[DiaSemana]', \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepConsumoSimuladoSitiosDestino(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ConsumoSimuladoSitiosDestino @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Codigo Sitio Origen],[Nombre Sitio Origen], \n ");
            lsb.Append("Total2=Sum([Total]/[TipoCambio]), \n ");
            lsb.Append("Duracion=sum([Duracion Minutos]), \n ");
            lsb.Append("Llamadas=sum([TotLlamadas]) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("', \n ");
            lsb.Append("@Where =  '', \n ");
            lsb.Append("@Group = '[Nombre Sitio Origen], \n ");
            lsb.Append("[Codigo Sitio Origen]', \n ");
            lsb.Append("@Order = '[Total2] Desc,[Duracion] Desc,[Llamadas] Desc,[Nombre Sitio Origen] Desc', \n ");
            lsb.Append("@OrderInv = '[Total2] Asc,[Duracion] Asc,[Llamadas] Asc,[Nombre Sitio Origen] Asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            lsb.Append(" \n ");
            return lsb.ToString();
        }

        public string ConsultaRepConsumoSimuladoDestino(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ConsumoSimuladoSitiosDestino @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Codigo Sitio Destino],[Nombre Sitio Destino], \n ");
            lsb.Append("[Total]=([Total]/[TipoCambio]), \n ");
            lsb.Append("[Duracion Minutos], \n ");
            lsb.Append("[TotLlamadas] \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Where = '', \n ");
            lsb.Append("@Order = '[Total] Desc,[Duracion Minutos] Desc,[TotLlamadas] Desc,[Nombre Sitio Destino] Desc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Duracion Minutos] Asc,[TotLlamadas] Asc,[Nombre Sitio Destino] Asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@SitioOrigen='" + param["Sitio"] + "', \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepConsumoSimuladoDestinoDetalle()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ConsumoSimuladoSitiosDestinoDetall @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Extensión origen] = [Extension Origen], \n ");
            lsb.Append("[Extensión destino] = [Extension Destino], \n ");
            lsb.Append("[Fecha], \n ");
            lsb.Append("[Hora], \n ");
            lsb.Append("[Minutos] = [Duracion Minutos], \n ");
            lsb.Append("[Total]=([Total]/[TipoCambio]), \n ");
            lsb.Append("[Localidad] = [Etiqueta]',  \n ");
            lsb.Append("@Where =''  , \n ");
            lsb.Append("@Group = '',  \n ");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[Localidad] Asc,[Hora] Asc,[Fecha] Desc,[Extensión destino] Asc,[Extensión origen] Asc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[Localidad] Desc,[Hora] Desc,[Fecha] Asc,[Extensión destino] Desc,[Extensión origen] Desc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@SitioDestino='''" + param["SitioDest"] + "''', \n ");
            lsb.Append("@SitioOrigen='" + param["Sitio"] + "', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepHistorico3Anios()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @fechaFin varchar(30) \n ");
            lsb.Append("declare @fechaInicio varchar(30) \n ");
            lsb.Append("set @fechaInicio = '''' + CONVERT(varchar,YEAR(GETDATE())-1) + '-01-01 12:00:00' + '''' \n ");
            lsb.Append("set @fechaFin = '''' + convert(varchar,GETDATE(),120) + '''' \n ");
            lsb.Append("exec ConsumoHistoricoActualvs2Anteriores @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Numero Mes], [Mes] = [Nombre Mes], \n ");
            lsb.Append("[2 Años atras]=SUM([Total 2 Anios Atras]/[TipoCambio]), \n ");
            lsb.Append("[Año anterior]=SUM([Total Anio Anterior]/[TipoCambio]), \n ");
            lsb.Append("[Año actual]=SUM([Total Anio Actual]/[TipoCambio])',  \n ");
            lsb.Append("@Where = '[Nombre Mes] is not null', \n ");
            lsb.Append("@Group = '[Nombre Mes], \n ");
            lsb.Append("[Numero Mes]',  \n ");
            lsb.Append("@Order = 'ABS([Numero Mes]) Asc', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n ");
            lsb.Append("@FechaFinRep = @fechaFin, \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaConsumoPorTipoLlamada()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepTipoLlamadaAdmin '" + Session["FechaInicio"].ToString() + " 00:00:00', \n ");
            lsb.Append("'" + Session["FechaFin"].ToString() + " 23:59:59','" + DSODataContext.Schema + "', \n ");
            lsb.Append(Session["iCodUsuario"] + ", \n ");
            lsb.Append(Session["iCodPerfil"] + " \n ");
            return lsb.ToString();
        }

        #region Consultas Perfil Empleado

        public string ConsultaPorTipoDestinoPeEm()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("And [Codigo Empleado] = '+@Empleado+' \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoAcumuladoPorTDestTodosParamRest     @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Codigo Tipo Destino],[Nombre Tipo Destino]=MIN(upper([Nombre Tipo Destino])), \n ");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n ");
            lsb.Append("[Numero]=sum([TotalLlamadas]), \n ");
            lsb.Append("[Duracion]=sum([Duracion Minutos])',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Codigo Tipo Destino]',  \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc', \n ");
            lsb.Append("@OrderDir = 'Desc' \n ");
            return lsb.ToString();
        }

        public string ConsultaPorNumMarcPeEm()
        {
            StringBuilder lsb = new StringBuilder();

            //NZ 20160921
            if ((DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox") && param["Nav"] == "PorTipoLlamN2")
            {
                lsb.AppendLine("DECLARE @llamsEntrada VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
                lsb.AppendLine("");
                lsb.AppendLine("DECLARE @llamsEnlace VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
            }

            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @ParamGEtiqueta varchar(max) \n ");
            lsb.Append("declare @ParamTDest varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("And [Codigo Empleado] = '+@Empleado+' \n ");
            lsb.Append("' \n ");
            if (param["TipoLlam"] != string.Empty)
            {
                lsb.Append("set @ParamGEtiqueta= '" + param["TipoLlam"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamGEtiqueta= 'null' \n ");
            }
            if (param["TDest"] != string.Empty)
            {
                lsb.Append("set @ParamTDest= '" + param["TDest"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamTDest= 'null' \n ");
            }
            lsb.Append("if @ParamGEtiqueta <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamTDest <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+') \n ");
            lsb.Append("' \n ");

            if ((DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox") && param["Nav"] == "PorTipoLlamN2")
            {
                lsb.AppendLine("set @Where = @Where + ' AND [Codigo Tipo Destino] <> ' + @llamsEntrada + '");
                lsb.AppendLine("                    AND [Codigo Tipo Destino] <> ' + @llamsEnlace + '");
                lsb.AppendLine("                    AND (LEN([Numero Marcado]) = 3 OR LEN([Numero Marcado]) >= 7)'");
            }

            lsb.Append("exec ConsumoAcumuladoPorNumMarcadoTodosParamRest   @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[CodNumMarc] = [Numero Marcado], [Numero Marcado] = '''''''' + isnull([Numero Marcado],''[Detalle]''), \n ");

            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
            {
                lsb.Append("[Etiqueta]=MIN(upper([Etiqueta])), \n ");
            }
            else
            {
                lsb.Append("[Nombre Localidad]=MIN(upper([Nombre Localidad])), \n ");
                lsb.Append("[Codigo Localidad], \n ");
            }

            lsb.Append("[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])), \n ");
            lsb.Append("[Codigo Tipo Destino], \n ");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n ");
            lsb.Append("[Duracion]=sum([Duracion Minutos]), \n ");
            lsb.Append("[Numero]=count(*), \n ");
            lsb.Append("[Tipo Llamada]=Min(upper([Tipo Llamada])), \n ");
            lsb.Append("[Clave Tipo Llamada]',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Numero Marcado], \n ");

            if (DSODataContext.Schema.ToLower() != "k5banorte" && DSODataContext.Schema.ToLower() != "evox")
            {
                lsb.Append("[Codigo Localidad], \n ");
            }

            lsb.Append("[Codigo Tipo Destino], \n ");
            lsb.Append("[Clave Tipo Llamada]',  \n ");

            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
            {
                lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Etiqueta] Asc,[Numero Marcado] Asc,[Nombre Tipo Destino] Asc,[Tipo Llamada] Asc', \n ");
                lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Etiqueta] Desc,[Numero Marcado] Desc,[Nombre Tipo Destino] Desc,[Tipo Llamada] Desc', \n ");
            }
            else
            {
                lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Localidad] Asc,[Numero Marcado] Asc,[Nombre Tipo Destino] Asc,[Tipo Llamada] Asc', \n ");
                lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Localidad] Desc,[Numero Marcado] Desc,[Nombre Tipo Destino] Desc,[Tipo Llamada] Desc', \n ");
            }

            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaDetallePeEm()
        {
            StringBuilder lsb = new StringBuilder();
            //NZ 20160921
            if ((DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox") && param["Nav"] == "PorTipoLlamN3")
            {
                lsb.AppendLine("DECLARE @llamsEntrada VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
                lsb.AppendLine("");
                lsb.AppendLine("DECLARE @llamsEnlace VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
                lsb.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
            }

            lsb.Append("declare @ParamTelDest varchar(max) \n ");
            lsb.Append("declare @ParamGEtiqueta varchar(max) \n ");
            lsb.Append("declare @ParamTDest varchar(max) \n ");
            lsb.Append("declare @Where varchar(max) \n ");
            if (param["NumMarc"] != string.Empty)
            {
                lsb.Append("set @ParamTelDest = '''" + param["NumMarc"] + "''' \n ");
            }
            else
            {
                lsb.Append("set @ParamTelDest = 'null' \n ");
            }
            if (param["TipoLlam"] != string.Empty)
            {
                lsb.Append("set @ParamGEtiqueta = '" + param["TipoLlam"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamGEtiqueta = 'null' \n ");
            }
            if (param["TDest"] != string.Empty)
            {
                lsb.Append("set @ParamTDest = '" + param["TDest"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamTDest = 'null' \n ");
            }
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("And [Codigo Empleado] = '+@Empleado+' \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamTelDest <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamGEtiqueta <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamTDest <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+') \n ");
            lsb.Append("' \n ");

            if ((DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox") && param["Nav"] == "PorTipoLlamN3")
            {
                lsb.Append("set @Where = @Where + ' AND [Codigo Tipo Destino] <> ' + @llamsEntrada + '");
                lsb.AppendLine("                    AND [Codigo Tipo Destino] <> ' + @llamsEnlace + '");
                lsb.AppendLine("                    AND (LEN([Numero Marcado]) = 3 OR LEN([Numero Marcado]) >= 7)'");
            }

            lsb.Append("exec ConsumoDetalladoTodosCamposRestSinLinea     @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Extension], \n ");
            lsb.Append("[Fecha], \n ");
            lsb.Append("[Hora], \n ");
            lsb.Append("[Duracion]=[Duracion Minutos], \n ");
            lsb.Append("[Total]= (([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])), \n ");
            lsb.Append("[Nombre Localidad]=upper([Nombre Localidad]), \n ");
            lsb.Append("[Numero Marcado], \n ");
            lsb.Append("[Tipo Llamada]=upper([Tipo Llamada]), \n ");
            lsb.Append("[Clave Tipo Llamada], \n ");
            lsb.Append("[Etiqueta]',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Order = '[Fecha] Asc,[Hora] Asc,[Duracion] Asc,[Extension] Asc,[Numero Marcado] Asc,[Nombre Localidad] Asc,[Tipo Llamada] Asc,[Etiqueta] Asc,[Total] Desc', \n ");
            lsb.Append("@OrderInv = '[Fecha] Desc,[Hora] Desc,[Duracion] Desc,[Extension] Desc,[Numero Marcado] Desc,[Nombre Localidad] Desc,[Tipo Llamada] Desc,[Etiqueta] Desc,[Total] Asc', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaPorTipoLlamPeEm(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("And [Codigo Empleado] = '+@Empleado+' And [Nombre Tipo Destino] <> ''Telcel'' \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoAcumuladoPorTipoLlamadaTodosParamRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Clave Tipo Llamada],[Tipo Llamada]=MIN([Tipo Llamada]), \n ");
            lsb.Append("Total = convert(numeric(15,2),sum(Costo/[TipoCambio])+sum(CostoSM/[TipoCambio])) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + "', \n ");
            }
            else
            {
                lsb.Append("',");
            }
            lsb.Append("@Where =  @Where, \n ");
            lsb.Append("@Group = '[Clave Tipo Llamada]', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaNumerosMasCarosPeEm()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("And [Codigo Empleado] = '+@Empleado+' \n ");
            lsb.Append("And [numero marcado] <> '''' \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoAcumuladoPorNumMarcadoTodosParamRest   @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[CodNumMarcado] = [Numero Marcado], [Numero Marcado] = '''''''' + [Numero Marcado], \n ");
            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
            {
                lsb.Append("[Etiqueta], \n ");
            }
            else
            {
                lsb.Append("[Nombre Localidad]=upper([Nombre Localidad]), \n ");
                lsb.Append("[Codigo Localidad], \n ");
            }

            lsb.Append("[Tipo Llamada]=upper([Tipo Llamada]), \n ");
            lsb.Append("[Clave Tipo Llamada], \n ");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n ");
            lsb.Append("[Duracion]=sum([Duracion Minutos]), \n ");
            lsb.Append("[Numero]=count(*)',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Numero Marcado], \n ");
            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
            {
                lsb.Append("[Etiqueta], \n ");
            }
            else
            {
                lsb.Append("upper([Nombre Localidad]), \n ");
                lsb.Append("[Codigo Localidad], \n ");
            }

            lsb.Append("upper([Tipo Llamada]), \n ");
            lsb.Append("[Clave Tipo Llamada]',  \n ");
            lsb.Append("@Order = '[Total] desc', \n ");
            lsb.Append("@OrderInv ='[Total] Asc', \n ");
            lsb.Append("@Lenght = 10, \n ");
            lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaHistoricoPeEm()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = '[Codigo Empleado] = '+@Empleado+' And [Nombre Mes] is not null ' \n ");
            lsb.Append("exec ConsumoHistoricoEmpSinLinea @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Nombre Mes], \n ");
            lsb.Append("Total = convert(numeric(15,2),sum(Costo/[TipoCambio])+sum(CostoSM/[TipoCambio]))',  \n ");
            lsb.Append("@Where=@Where, \n ");
            lsb.Append("@Group = '[Nombre Mes]', \n ");
            lsb.Append("@Order = '[Orden] Asc', \n ");
            lsb.Append("@OrderInv = '[Orden] Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaExtensionesEnLasQueSeUsoElCodAuto()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Empleado varchar (max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia<=GETDATE() \n ");
            lsb.Append("and dtFinVigencia>GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("set @Empleado='-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("and [Codigo Autorizacion] <> '''' and [Codigo Empleado] = '+@Empleado+'  \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoDetalladoTodosCamposRest     @Schema='" + DSODataContext.Schema + "', \n ");

            //NZ 20160906 Se agrega que para el cliente banorte no se visualizen los Codigos mas que los ultimos 3 digitos.
            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                lsb.Append("@Fields='[Codigo Autorizacion]  = ''***'' + RIGHT([Codigo Autorizacion],3), \n ");
            }
            else
            {
                lsb.Append("@Fields='[Codigo Autorizacion], \n ");
            }

            lsb.Append("[Extension], \n ");
            lsb.Append("[Llamadas] = count(*)',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Order = '[Codigo Autorizacion] Desc,[Llamadas] Desc,[Extension] Desc', \n ");
            lsb.Append("@Group = '[Codigo Autorizacion], \n ");
            lsb.Append("[Extension]',  \n ");
            lsb.Append("@OrderInv = '[Codigo Autorizacion] Asc,[Llamadas] Asc,[Extension] Asc', \n ");
            lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaConsumoPorTipoLlamadaPeEm()
        {
            StringBuilder lsb = new StringBuilder();

            /***************************************************************/
            //Busca empleado en base a Usuario

            lsb.Append("declare @Empleado int \n ");
            lsb.Append("select @Empleado = iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            /***************************************************************/

            lsb.Append("exec RepTipoLlamadaEmple '" + Session["FechaInicio"].ToString() + " 00:00:00', \n ");
            lsb.Append("'" + Session["FechaFin"].ToString() + " 23:59:59','" + DSODataContext.Schema + "', \n ");
            lsb.Append(Session["iCodUsuario"] + ", \n ");
            lsb.Append(Session["iCodPerfil"] + ", \n ");
            lsb.Append("@Empleado \n ");

            return lsb.ToString();
        }

        #endregion Consultas Perfil Empleado

        public string ConsultaConfiguracionDeReportesPorPerfil()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("select  \n ");
            lsb.Append("[Reporte] = CveRep.Descripcion, \n ");
            lsb.Append("[Contenedor] = IDCtrl.Descripcion, \n ");
            lsb.Append("[tipoGrafDefault] = TpGraf.Descripcion, \n ");
            lsb.Append("[tituloGrid] = Cnfg.FCTituloTabla, \n ");
            lsb.Append("[tituloGrafica] = Cnfg.FCTituloGrafica, \n ");
            lsb.Append("[pestaniaActiva] = Cnfg.FCPestanaActiva \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('FCConfReportes','FC Configuracion de reportes','Español')] Cnfg \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCCveRep','FC Clave de reporte','Español')] CveRep \n ");
            lsb.Append("on CveRep.iCodCatalogo = Cnfg.FCCveRep \n ");
            lsb.Append("and CveRep.dtIniVigencia <> CveRep.dtFinVigencia  \n ");
            lsb.Append("and CveRep.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCIDControl','FC ID de control','Español')] IDCtrl \n ");
            lsb.Append("on IDCtrl.iCodCatalogo = Cnfg.FCIDControl \n ");
            lsb.Append("and IDCtrl.dtIniVigencia <> IDCtrl.dtFinVigencia  \n ");
            lsb.Append("and IDCtrl.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCTipoGraf','FC Tipo de Grafica','Español')] TpGraf \n ");
            lsb.Append("on TpGraf.iCodCatalogo = Cnfg.FCTipoGraf \n ");
            lsb.Append("and TpGraf.dtIniVigencia <> TpGraf.dtFinVigencia  \n ");
            lsb.Append("and TpGraf.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCDashboard','FC Dashboard','Español')] Dash \n ");
            lsb.Append("on Dash.iCodCatalogo = Cnfg.FCDashboard \n ");
            lsb.Append("and Dash.dtIniVigencia <> Dash.dtFinVigencia  \n ");
            lsb.Append("and Dash.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("where Cnfg.dtIniVigencia <> Cnfg.dtFinVigencia  \n ");
            lsb.Append("and Cnfg.dtFinVigencia >= GETDATE() \n ");
            lsb.Append("and Cnfg.Perfil = " + Session["iCodPerfil"] + " \n ");
            lsb.Append("and Dash.Descripcion = 'Dashboard.aspx' \n ");
            return lsb.ToString();
        }


        public string ConsultaConfiguracionDeReportesPorPerfil(string nombrePagina)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("select  \n ");
            lsb.Append("[Reporte] = CveRep.Descripcion, \n ");
            lsb.Append("[Contenedor] = IDCtrl.Descripcion, \n ");
            lsb.Append("[tipoGrafDefault] = TpGraf.Descripcion, \n ");
            lsb.Append("[tituloGrid] = Cnfg.FCTituloTabla, \n ");
            lsb.Append("[tituloGrafica] = Cnfg.FCTituloGrafica, \n ");
            lsb.Append("[pestaniaActiva] = Cnfg.FCPestanaActiva \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('FCConfReportes','FC Configuracion de reportes','Español')] Cnfg \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCCveRep','FC Clave de reporte','Español')] CveRep \n ");
            lsb.Append("on CveRep.iCodCatalogo = Cnfg.FCCveRep \n ");
            lsb.Append("and CveRep.dtIniVigencia <> CveRep.dtFinVigencia  \n ");
            lsb.Append("and CveRep.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCIDControl','FC ID de control','Español')] IDCtrl \n ");
            lsb.Append("on IDCtrl.iCodCatalogo = Cnfg.FCIDControl \n ");
            lsb.Append("and IDCtrl.dtIniVigencia <> IDCtrl.dtFinVigencia  \n ");
            lsb.Append("and IDCtrl.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCTipoGraf','FC Tipo de Grafica','Español')] TpGraf \n ");
            lsb.Append("on TpGraf.iCodCatalogo = Cnfg.FCTipoGraf \n ");
            lsb.Append("and TpGraf.dtIniVigencia <> TpGraf.dtFinVigencia  \n ");
            lsb.Append("and TpGraf.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCDashboard','FC Dashboard','Español')] Dash \n ");
            lsb.Append("on Dash.iCodCatalogo = Cnfg.FCDashboard \n ");
            lsb.Append("and Dash.dtIniVigencia <> Dash.dtFinVigencia  \n ");
            lsb.Append("and Dash.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("where Cnfg.dtIniVigencia <> Cnfg.dtFinVigencia  \n ");
            lsb.Append("and Cnfg.dtFinVigencia >= GETDATE() \n ");
            lsb.Append("and Cnfg.Perfil = " + Session["iCodPerfil"] + " \n ");
            lsb.Append("and Dash.Descripcion = '" + nombrePagina + "' \n ");
            return lsb.ToString();
        }

        public string ConsultaReporteColaboradores()  //20141216 PO.  <-- Reporte Bimbo
        {
            StringBuilder lsb = new StringBuilder();
            // BG.20150323 Se agrega validacion de Sitio
            lsb.Append("declare @Sitio int \n ");

            if (string.IsNullOrEmpty(param["Sitio"]))
            {
                param["Sitio"] = "''";
            }

            lsb.Append("set @Sitio = " + param["Sitio"] + " \n ");
            lsb.Append("if @Sitio <> '' \n ");
            lsb.Append("begin \n ");
            lsb.Append("set @Sitio = " + param["Sitio"] + " \n ");
            lsb.Append("end \n ");
            lsb.Append("else \n ");
            lsb.Append("begin \n ");
            lsb.Append("set  @Sitio = '' \n ");
            lsb.Append("end \n ");
            lsb.Append("exec RepColoboradoresBimbo  @Esquema ='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Sitio = @Sitio, \n ");
            //lsb.Append("@Sitio = " + param["Sitio"] + ",\n ");   // BG.20150304 Se agrega filtro de Sitio
            lsb.Append("@FechaIniRep = " + "'" + Session["FechaInicio"].ToString() + " 00:00:00', \n ");
            lsb.Append("@FechaFinRep = " + "'" + Session["FechaFin"].ToString() + " 23:59:59' \n ");

            return lsb.ToString();
        }

        public string ConsultaPorTDestPrs(string linkGraf)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamCarrier varchar(max)\r ");
            lsb.Append("set @ParamCarrier = '" + param["Carrier"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''\r ");
            lsb.Append("'\r ");
            lsb.Append("if @ParamCarrier <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')\r ");
            lsb.Append("'\r ");

            lsb.Append("exec ProsaRepTabConsumoPorTipoDestinoSP @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Codigo Tipo Destino],[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])), \r");
            lsb.Append("[TotalSimulado]= sum([CostoFac]) + SUM([CostoSM]), \r");
            lsb.Append("[TotalReal]= sum([Costo]) + SUM([CostoSM]), \r");
            lsb.Append("[CostoSimulado]= sum([CostoFac]), \r");
            lsb.Append("[CostoReal]= sum([Costo]), \r");
            lsb.Append("[SM]= sum([CostoSM]), \r");
            lsb.Append("[Numero]=SUM([TotalLlamadas]), \r");
            lsb.Append("[Duracion]=sum([Duracion Minutos])  \r");

            if (linkGraf != "")
            {
                lsb.Append("," + linkGraf);
            }

            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,\r ");
            lsb.Append("@Carrier = " + param["Carrier"] + ",\r ");
            lsb.Append("@Group = '[Codigo Tipo Destino]',  \r");
            lsb.Append("@Order = '[TotalSimulado] Desc,[TotalReal] Desc,[CostoSimulado] Desc,[CostoReal] Desc,[SM] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc', \r");
            lsb.Append("@OrderInv = '[TotalSimulado] Asc,[TotalReal] Asc,[CostoSimulado] Asc,[CostoReal] Asc,[SM] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc', \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \r");
            return lsb.ToString();
        }

        public string ConsultaPorTDestPrsN2(string linkGraf)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamEmple varchar(max)\r ");
            lsb.Append("declare @ParamSitio varchar(max)\r ");
            lsb.Append("declare @ParamCenCos varchar(max)\r ");
            lsb.Append("declare @ParamCarrier varchar(max)\r ");
            lsb.Append("declare @ParamExtension varchar(max)\r ");
            lsb.Append("declare @ParamCodAut varchar(max)\r ");
            lsb.Append("declare @ParamLocali varchar(max)\r ");
            lsb.Append("declare @ParamTelDest varchar(max)\r ");
            lsb.Append("declare @ParamGEtiqueta varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("\r ");
            lsb.Append("set @ParamEmple = 'null'\r ");
            lsb.Append("set @ParamSitio = 'null'\r ");
            lsb.Append("set @ParamCenCos = 'null'\r ");
            lsb.Append("set @ParamCarrier = 'null'\r ");
            lsb.Append("set @ParamExtension = 'null'\r ");
            lsb.Append("set @ParamCodAut = 'null'\r ");
            lsb.Append("set @ParamLocali = 'null'\r ");
            lsb.Append("set @ParamTelDest = 'null'\r ");
            lsb.Append("set @ParamGEtiqueta = 'null'\r ");
            lsb.Append("set @ParamTDest = null \r ");
            //lsb.Append("set @ParamTDest = '386'\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamEmple <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamSitio <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamCenCos <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamCarrier <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamExtension <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamCodAut <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamLocali <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTelDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamGEtiqueta <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");

            if (param["TDest"] != string.Empty)
            {
                lsb.Append("set @ParamTDest = " + param["TDest"] + "  \r ");
            }

            lsb.Append("if @ParamTDest <> 'null'  \r ");
            lsb.Append("Begin\r ");
            lsb.Append("           if @ParamTDest <> 238135\r ");
            lsb.Append("           Begin   \r ");
            lsb.Append("                     if @ParamTDest <> 238134\r ");
            lsb.Append("                     Begin\r ");
            lsb.Append("                            set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')'\r ");
            lsb.Append("                     End\r ");
            lsb.Append("            End\r ");
            lsb.Append("End\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest = 238135\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in(385,388,389)\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest = 238134\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in(381,382,383,384,385,386,387,388,389,390,391,392,393,394,395,82851,83619,83620,87680,217620,238135)\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("exec ProsaConsumoAcumuladoPorCarrierTodosParamRest    @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Codigo Carrier],[Nombre Carrier]=Min(upper([Nombre Carrier])),\r ");
            lsb.Append("[TotalSimulado]= sum([CostoFac]) + SUM([CostoSM]), \r");
            lsb.Append("[TotalReal]= sum([Costo]) + SUM([CostoSM]), \r");
            lsb.Append("[CostoSimulado]= sum([CostoFac]), \r");
            lsb.Append("[CostoReal]= sum([Costo]), \r");
            lsb.Append("[SM]= sum([CostoSM]), \r");
            lsb.Append("[Numero]=sum([TotalLlamadas]),\r ");
            lsb.Append("[Duracion]=sum([Duracion Minutos]), \r ");
            lsb.Append("[link]= [link]', \r ");

            lsb.Append("@Where = @Where,\r ");
            lsb.Append("@Link = '" + linkGraf + "',\r ");
            lsb.Append("@Group = '[Codigo Carrier],[Link]', \r ");
            lsb.Append("@Order = '[TotalSimulado] Desc,[TotalReal] Desc,[CostoSimulado] Desc,[CostoReal] Desc,[SM] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Carrier] Asc',\r ");
            lsb.Append("@OrderInv = '[TotalSimulado] Asc,[TotalReal] Asc,[CostoSimulado] Asc,[CostoReal] Asc,[SM] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Carrier] Desc',\r ");
            lsb.Append("@OrderDir = 'Desc',\r ");
            lsb.Append("@TipoDest = @ParamTDest,\r ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:29'''\r ");
            lsb.Append(",\r ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\r ");


            return lsb.ToString();
        }

        public string ConsultaPorTDestLDPrsN2(string linkGraf)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)\r ");

            lsb.Append("declare @ParamCarrier varchar(max)\r ");
            lsb.Append("set @ParamCarrier = '" + param["Carrier"] + "'\r ");
            lsb.Append("\r ");

            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @ParamTDest = '238135'\r "); //Es el icodcatalogo de TDest LD
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''\r ");
            lsb.Append("'\r ");
            lsb.Append("if @ParamTDest = 238135\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in(385,388,389)\r ");
            lsb.Append("'\r ");

            lsb.Append("if @ParamCarrier <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')\r ");
            lsb.Append("'\r ");

            lsb.Append("exec ProsaRepTabConsumoPorTipoDestinoLDSP    @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Codigo Tipo Destino],[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),\r ");
            lsb.Append("[TotalSimulado]= sum([CostoFac]) + SUM([CostoSM]), \r");
            lsb.Append("[TotalReal]= sum([Costo]) + SUM([CostoSM]), \r");
            lsb.Append("[CostoSimulado]= sum([CostoFac]), \r");
            lsb.Append("[CostoReal]= sum([Costo]), \r");
            lsb.Append("[SM]= sum([CostoSM]), \r");
            lsb.Append("[Numero]=SUM([TotalLlamadas]),\r ");
            lsb.Append("[Duracion]=sum([Duracion Minutos]) \r ");

            if (linkGraf != "")
            {
                lsb.Append("," + linkGraf);
            }

            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,\r ");
            lsb.Append("@Group = '[Codigo Tipo Destino]', \r ");
            lsb.Append("@Order = '[TotalSimulado] Desc,[TotalReal] Desc,[CostoSimulado] Desc,[CostoReal] Desc,[SM] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc',\r ");
            lsb.Append("@OrderInv = '[TotalSimulado] Asc,[TotalReal] Asc,[CostoSimulado] Asc,[CostoReal] Asc,[SM] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc',\r ");
            lsb.Append("@OrderDir = 'Desc',\r ");
            lsb.Append("@TipoDest = @ParamTDest,\r ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''\r ");
            lsb.Append(",\r ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\r ");

            return lsb.ToString();
        }

        public string ConsultaPorTDestCencosPrsN3(string linkGraf)
        {

            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamEmple varchar(max)\r ");
            lsb.Append("declare @ParamSitio varchar(max)\r ");
            lsb.Append("declare @ParamCenCos varchar(max)\r ");
            lsb.Append("declare @ParamCarrier varchar(max)\r ");
            lsb.Append("declare @ParamExtension varchar(max)\r ");
            lsb.Append("declare @ParamCodAut varchar(max)\r ");
            lsb.Append("declare @ParamLocali varchar(max)\r ");
            lsb.Append("declare @ParamTelDest varchar(max)\r ");
            lsb.Append("declare @ParamGEtiqueta varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @ParamEmple = 'null'\r ");
            lsb.Append("set @ParamSitio = 'null'\r ");
            lsb.Append("set @ParamCenCos = 'null'\r ");
            lsb.Append("set @ParamCarrier = '" + param["Carrier"] + "'\r ");
            lsb.Append("set @ParamExtension = 'null'\r ");
            lsb.Append("set @ParamCodAut = 'null'\r ");
            lsb.Append("set @ParamLocali = 'null'\r ");
            lsb.Append("set @ParamTelDest = 'null'\r ");
            lsb.Append("set @ParamGEtiqueta = 'null'\r ");
            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamEmple <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamSitio <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamCenCos <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamCarrier <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamExtension <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamCodAut <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamLocali <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTelDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamGEtiqueta <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'  \r ");
            lsb.Append("Begin\r ");
            lsb.Append("           if @ParamTDest <> 238135\r ");
            lsb.Append("           Begin   \r ");
            lsb.Append("                     if @ParamTDest <> 238134\r ");
            lsb.Append("                     Begin\r ");
            lsb.Append("                            set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')'\r ");
            lsb.Append("                     End\r ");
            lsb.Append("            End\r ");
            lsb.Append("End\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest = 238135 \r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in(385,388,389)\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest = 238134\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in(381,382,383,384,385,386,387,388,389,390,391,392,393,394,395,82851,83619,83620,87680,217620,238135)\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("exec ProsaConsumoAcumuladoTodosCamposRest       \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Codigo Centro de Costos],[Nombre Centro de Costos]=Min(upper([Nombre Centro de Costos])),\r ");
            lsb.Append("TotImporte = SUM([Costo] + [CostoSM]),\r ");
            lsb.Append("LLamadas = sum([TotalLlamadas]),\r ");
            lsb.Append("TotMin = SUM([Duracion Minutos])\r ");

            if (linkGraf != "")
            {
                lsb.Append("," + linkGraf);
            }

            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,\r ");
            lsb.Append("@Group = '[Codigo Centro de Costos]', \r ");
            lsb.Append("@Order = '[TotImporte] Desc,[TotMin] Desc,[LLamadas] Desc,[Nombre Centro de Costos] Asc',\r ");
            lsb.Append("@OrderInv = '[TotImporte] Asc,[TotMin] Asc,[LLamadas] Asc,[Nombre Centro de Costos] Desc',\r ");
            lsb.Append("@OrderDir = 'Desc',\r ");
            lsb.Append("@TipoDest = @ParamTDest,\r ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\r ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\r ");

            return lsb.ToString();

        }

        public string ConsultaDetalleParaFinanzas()
        {

            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' and [Codigo Tipo Destino] not in (381,382) ' \r ");
            lsb.Append("\r ");
            lsb.Append("exec RepTabDetallados @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[FechaInicio],\r ");
            lsb.Append("[Numero Marcado],\r ");
            lsb.Append("[Nombre Tipo Destino] = [Region],\r ");
            lsb.Append("[Extension],\r ");
            lsb.Append("[Duracion]=[Duracion Minutos] \r ");
            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,\r ");
            lsb.Append("@Order = '[Duracion] Desc,[Nombre Tipo Destino] Asc,[Numero Marcado] Asc,[Extension] Asc,[FechaInicio] Asc',\r ");             //BG.20151026
            lsb.Append("@OrderInv = '[Duracion] Asc,[Nombre Tipo Destino] Desc,[Numero Marcado] Desc,[Extension] Desc,[FechaInicio] Desc',\r ");       //BG.20151026            
            lsb.Append("@OrderDir = 'Asc',\r ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \r ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\r ");

            return lsb.ToString();
        }

        public string ConsultaAreasMayorConsumo(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ProsaRepTabAreasMayorConsumo \r");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='idArea	= [idArea], \r");
            lsb.Append("Area = [Area], \r");
            lsb.Append("Importe = Sum([Importe]), \r");
            lsb.Append("Porcentaje = Sum([Porcentaje]) \r");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Group = '[idArea],[Area]', \r");
            lsb.Append("@Order = '[Porcentaje] Desc,[Importe] Desc, Area Asc',\r");
            lsb.Append("@OrderInv = '[Porcentaje] Asc,[Importe] Asc, Area Desc',\r");
            lsb.Append("@OrderDir = 'Desc',\r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \r");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\r");

            return lsb.ToString();

        }

        public string ConsultaDetalleLlamsTelcelF4()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec spDetalleLlamadasF4  \r");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \r");
            lsb.Append("@fechaInicio = '" + Session["FechaInicio"].ToString() + " 00:00:00',\r");
            lsb.Append("@fechaFin = '" + Session["FechaFin"].ToString() + " 23:59:59' \r");
            return lsb.ToString();
        }

        public string ConsultaRepPrsConsumoPorCampania()   //string linkGrafica
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ProsaConsumoPorCampania  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Campaña] = [Campaña], \n ");
            lsb.Append("[Importe]= isnull([Importe],0), \n ");
            lsb.Append("[Llamadas]= isnull([Llamadas],0), \n ");
            lsb.Append("[Minutos]= isnull([Duracion],0)', \n ");

            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepDetalladoEnlacesTelmex()   //string linkGrafica
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec DetalladoEnlacesTelmex  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Folio], \n ");
            lsb.Append("[PuntaA], \n ");
            lsb.Append("[PuntaB], \n ");
            lsb.Append("[Total]= ([Importe])', \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' ,\n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
            return lsb.ToString();
        }

        public string ConsultaEmpleadoLigadoAlUsuario()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("SELECT isNull(Max(iCodCatalogo), -1) as iCodCatalogo");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
            lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()");
            lsb.AppendLine("    AND Usuar = " + Session["iCodUsuario"] + "");
            return lsb.ToString();
        }

        public string ConsultaRepTopLineasMasCaras(string linkGrafica, string lsCarrier)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Cantidad int \n ");
            lsb.Append("declare @DisplayStart int \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @ParamExtension varchar(max) \n ");

            /*20140106 AM. Se agrega filtro de empleado*/
            if (param["Emple"] != string.Empty)
            {
                lsb.Append("set @ParamEmple = '" + param["Emple"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamEmple = 'null' \n ");
            }
            lsb.Append("set @ParamExtension = 'null' \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append(" and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''  \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamExtension <> 'null' \n ");
            lsb.Append("set @Where = @Where + 'And [Extension] in('+@ParamExtension+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ReporteLineasPorCarrierDashFC @Schema='" + DSODataContext.Schema + "', \n ");

            /*20140107 AM. Se agrega parametro de carrier*/
            lsb.Append(" @Carrier=" + lsCarrier + ", \n ");

            lsb.Append("@Fields='[Extension], \n ");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("NombreCompleto = UPPER([Nombre Completo]), \n ");
            lsb.Append("[ExtensionCod]= [Extension], \n ");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio])',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Extension], \n ");
            lsb.Append(" UPPER([Nombre Completo]), \n ");
            lsb.Append("[Codigo Empleado]',  \n ");
            lsb.Append("@Order = '[Total] desc', \n ");
            lsb.Append("@OrderInv = '[Total] asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaDetalleFacturaTelcel()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @NumeroMovil varchar(max) \n");
            lsb.Append("declare @Empleado varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @fechaini varchar(10) \n");
            lsb.Append("set @fechaini = CONVERT(varchar(10), '" + Session["FechaInicio"].ToString() + " 00:00:00',120) \n");
            lsb.Append("declare @fechafin varchar(10) \n");
            lsb.Append("set @fechafin = CONVERT(varchar(10),'" + Session["FechaFin"].ToString() + " 23:59:59',120) \n");
            lsb.Append("/* CUANDO LLEVE  NAVEGACIÓN EL REPORTE DEBE DE FILTRAR EL EMPLEADO AL QUE SE LE DA CLIC */ \n");
            if (param["Emple"] != string.Empty)
            {
                lsb.Append("set @ParamEmple = '" + param["Emple"] + "' \n");
            }
            else
            {
                lsb.Append("set @ParamEmple = 'null' \n");
            }
            lsb.Append("set @Where = 'FechaInicio between  '''+@fechaini+''' and '''+@fechafin+''' \n");
            lsb.Append("' \n");
            lsb.Append("if @ParamEmple <> 'null' \n");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n");
            lsb.Append("' \n");
            lsb.Append("if @ParamEmple <> 'null' \n");
            lsb.Append("begin \n");
            lsb.Append("select @NumeroMovil = tel \n");
            lsb.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas \n");
            lsb.Append("where dtinivigencia <> dtfinvigencia \n");
            lsb.Append("and dtfinvigencia >= getdate() \n");
            lsb.Append("and emple = @ParamEmple \n");
            //20150113 AM. Se agrega filtro de linea 
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("and vchCodigo = '" + param["Linea"] + "' \n");
            }
            lsb.Append("if @NumeroMovil <> 'null' \n");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n");
            lsb.Append("' \n");
            lsb.Append("end  \n");
            lsb.Append("------------------------------------------------------------------------------------------------------------------------ \n");
            lsb.Append("/* CUANDO NO HAYA NAVEGACIÓN ESTE REPORTE DEBE DE FUNCIONAR EN BASE AL USUARIO CON EL QUE TE LOGEAS */ \n");
            lsb.Append("if @ParamEmple = 'Null' \n");
            lsb.Append("begin \n");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo)  \n");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n");
            lsb.Append("and dtIniVigencia <= GETDATE() \n");
            lsb.Append("and dtFinVigencia > GETDATE() \n");
            lsb.Append("select @NumeroMovil = tel \n");
            lsb.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas \n");
            lsb.Append("where dtinivigencia <> dtfinvigencia \n");
            lsb.Append("and dtfinvigencia >= getdate() \n");
            lsb.Append("and emple = @Empleado \n");
            //20150119 AM. Se agrega filtro de linea 
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("and vchCodigo = '" + param["Linea"] + "' \n");
            }
            lsb.Append("if @Empleado is null \n");
            lsb.Append("begin \n");
            lsb.Append("	set @Empleado = '-1' \n");
            lsb.Append("end \n");
            lsb.Append("set @Where = @Where + 'And [Codigo Empleado] = '+@Empleado+' \n");
            lsb.Append("' \n");
            lsb.Append("--set @NumeroMovil = 'null' \n");
            lsb.Append("if @NumeroMovil <> 'null' \n");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n");
            lsb.Append("' \n");
            lsb.Append("end \n");
            lsb.Append("exec [ConsumoTelcelDetalleFacRestVerticalConsolidado]  @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Concepto] = Replace([Concepto],''.'',''''), \n");
            lsb.Append("[idConcepto], \n");
            lsb.Append("[Detalle], \n");
            lsb.Append("[Total]',  \n");
            lsb.Append("@Where =@Where, \n");
            lsb.Append("@Group = '[Concepto], \n");
            lsb.Append("[idConcepto], \n");
            lsb.Append("[Detalle]',  \n");
            lsb.Append("@Order = '[Total] Desc,[Concepto] Asc', \n");
            lsb.Append("@OrderInv = '[Total] Asc,[Concepto] Desc', \n");
            lsb.Append("@OrderDir = 'Desc', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaConsumoDesglosadoPorConcepto()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @NumeroMovil varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @idConcepto varchar(max) \n ");
            lsb.Append("declare @fechaini varchar(10) \n ");
            lsb.Append("set @fechaini = CONVERT(varchar(10), '" + Session["FechaInicio"].ToString() + " 00:00:00',120) \n ");
            lsb.Append("declare @fechafin varchar(10) \n ");
            lsb.Append("set @fechafin = CONVERT(varchar(10),'" + Session["FechaFin"].ToString() + " 23:59:59',120) \n ");
            lsb.Append("set @idConcepto = '" + param["Concepto"] + "' \n ");
            lsb.Append("/* CUANDO LLEVE  NAVEGACIÓN EL REPORTE DEBE DE FILTRAR EL EMPLEADO AL QUE SE LE DA CLIC */ \n ");
            if (param["Emple"] != string.Empty)
            {
                lsb.Append("set @ParamEmple = " + param["Emple"] + " \n ");
            }
            else
            {
                lsb.Append("set @ParamEmple = 'null' \n ");
            }
            lsb.Append("set @Where = 'FechaInicio between  '''+@fechaini+''' and '''+@fechafin+''' \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("begin \n ");
            lsb.Append("select @NumeroMovil = icodcatalogo \n ");
            lsb.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas \n ");
            lsb.Append("where dtinivigencia <> dtfinvigencia \n ");
            lsb.Append("and dtfinvigencia >= getdate() \n ");
            lsb.Append("and emple = @ParamEmple \n ");

            /*20150112 AM. Se agrega filtro de linea*/
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("and vchCodigo = '" + param["Linea"] + "' \n ");
            }

            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Linea] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @idConcepto <> 'null' \n ");
            lsb.Append("     set @Where = @Where + 'And [idConcepto] in('+@idConcepto+') \n ");
            lsb.Append("' \n ");
            lsb.Append("end  \n ");
            lsb.Append("--------------------------------------------------------------------------------------------------------------------- \n ");
            lsb.Append("/* CUANDO NO HAYA NAVEGACIÓN ESTE REPORTE DEBE DE FUNCIONAR EN BASE AL USUARIO CON EL QUE TE LOGEAS */ \n ");
            lsb.Append("if @ParamEmple = 'Null' \n ");
            lsb.Append("begin \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo)  \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("select @NumeroMovil = icodcatalogo \n ");
            lsb.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas \n ");
            lsb.Append("where dtinivigencia <> dtfinvigencia \n ");
            lsb.Append("and dtfinvigencia >= getdate() \n ");
            lsb.Append("and emple = @Empleado \n ");
            /*20150119 AM. Se agrega filtro de linea*/
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("and vchCodigo = '" + param["Linea"] + "' \n ");
            }
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = @Where + 'And [Codigo Empleado] = '+@Empleado+' \n ");
            lsb.Append("' \n ");
            lsb.Append("--set @NumeroMovil = 'Param(Extension)' \n ");
            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Linea] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("end \n ");
            lsb.Append("if @idConcepto <> 'null' \n ");
            lsb.Append("     set @Where = @Where + 'And [idConcepto] in('+@idConcepto+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec spConsolidadoFacturasDeMovilesRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idConcepto], [Concepto] = UPPER([Concepto]),[Descripcion], \n ");
            lsb.Append("[Telefono], \n ");
            lsb.Append("Total = sum([Total])',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@Group = ' UPPER([Concepto]),[Descripcion], \n ");
            lsb.Append("[idConcepto], \n ");
            lsb.Append("[Telefono]',  \n ");
            lsb.Append("@Order = '[Total] Desc,[Concepto] Asc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Concepto] Desc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaConsumoDesglosadoPorLlamadas()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @NumeroMovil varchar(max) \n ");
            lsb.Append("declare @NumeroMarcado varchar(max) \n ");
            lsb.Append("declare @CampoTotal varchar(40) \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");

            /************************************************************************************************************/
            //20141119 AM. Se agrega a la consulta el filtro del iCodCatalogo del empleado para optimizar la consulta
            if (Session["iCodPerfil"].ToString() == "370")
            {

                lsb.Append("declare @iCodEmple int  \n ");
                lsb.Append("select @iCodEmple = max(iCodCatalogo) from \n ");
                lsb.Append(DSODataContext.Schema);
                lsb.Append(".[VisHistoricos('Emple','Empleados','Español')] \n ");
                lsb.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + "' \n ");
                lsb.Append("and Usuar = " + Session["iCodUsuario"] + " \n ");
                lsb.Append("if @@rowcount = 1 \n ");
                lsb.Append("begin \n ");
                lsb.Append("   set @Where = @Where +' and [Codigo Empleado] = ' + convert(varchar,@iCodEmple) \n ");
                lsb.Append("end \n ");
                lsb.Append("else \n ");
                lsb.Append("begin \n ");
                lsb.Append("   set @Where = @Where +' and [Codigo Empleado] = 0' \n ");
                lsb.Append("end \n ");
            }
            /************************************************************************************************************/

            //20150119 AM. Se agrega parametro de linea
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("set @NumeroMovil = '" + param["Linea"] + "' \n ");
            }
            else
            {
                lsb.Append("set @NumeroMovil = 'null' \n ");
            }

            lsb.Append("set @NumeroMarcado = 'null' \n ");

            if (param["Emple"] != string.Empty)
            {
                lsb.Append("set @ParamEmple = '" + param["Emple"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamEmple = 'null' \n ");
            }


            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("begin \n ");
            lsb.Append("select @NumeroMovil = vchCodigo \n ");
            lsb.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas \n ");
            lsb.Append("where dtinivigencia <> dtfinvigencia \n ");
            lsb.Append("and dtfinvigencia >= getdate() \n ");
            lsb.Append("and emple = @ParamEmple \n ");

            /*20150114 AM. Se agrega parametro de linea*/
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("and vchCodigo = '" + param["Linea"] + "' \n ");
            }

            lsb.Append("end  \n ");
            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @NumeroMarcado <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Numero Marcado] in('+@NumeroMarcado+') \n ");
            lsb.Append("' \n ");
            lsb.Append("select @CampoTotal = vchcodigo \n ");
            lsb.Append("from " + DSODataContext.Schema + ".catalogos \n ");
            lsb.Append("where iCodRegistro = replace('''" + param["Concepto"] + "''',char(39),'') \n ");
            lsb.Append("exec ConsumoTelcelDetalleLlamRestSinClaveCar  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Fecha Llamada], \n ");
            lsb.Append("[Hora Llamada], \n ");
            lsb.Append("[Numero Marcado] = '''''''' + [Numero Marcado], \n ");
            lsb.Append("[Duracion Minutos], \n ");
            lsb.Append("[Costo]=([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Punta A], \n ");
            lsb.Append("[Dir Llamada], \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@CampoTotal = @CampoTotal, \n ");
            lsb.Append("@Group = '',  \n ");

            if (Session["iCodPerfil"].ToString() != "370")
            {
                lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
                lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            }
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaDetalleFacturaNextel()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec DetalleDeFacturaDeNextelDashFC \r");
            lsb.Append("@Esquema='" + DSODataContext.Schema + "', \r");
            //20150113 AM. Se agrega condicion para consultar la linea nextel en caso de que solo tenga una sola linea 
            //(Esto se hizo por un requerimiento en bimbo)
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("@Linea='" + param["Linea"] + "', \r");
            }
            else
            {
                StringBuilder consultaLinea = new StringBuilder();
                consultaLinea.Append("select vchCodigo \r");
                consultaLinea.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] \r");
                consultaLinea.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
                consultaLinea.Append("and SitioCod like '%nextel%' \r");
                consultaLinea.Append("and Emple = " + param["Emple"] + " \r");
                param["Linea"] = DSODataAccess.ExecuteScalar(consultaLinea.ToString()).ToString();
                lsb.Append("@Linea='" + param["Linea"] + "', \r");
            }
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \r");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59' \n ");
            return lsb.ToString();
        }

        private string ObtenerConsumoPorCenCosJerarquico(string linkCenCos, string linkEmple, int cenCos,
            int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            string sp = "ObtieneConsumoReporteJerarquico";
            if (string.IsNullOrEmpty(isFT) || param["Nav"] == "CenCosJerarquicoN2")
            {
                isFT = "0";
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    sp = (@omitirInfoCDR == 1) ? "ObtieneConsumoReporteJerarquicoSianaSeven" : "ObtieneConsumoReporteJerarquico";
                }

            }



            StringBuilder consulta = new StringBuilder();
            consulta.Append("EXEC " + sp + " @Schema='" + DSODataContext.Schema + "',\n ");
            consulta.Append("@CenCos = " + cenCos.ToString() + ",\n ");
            consulta.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            consulta.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            consulta.Append("@Fechainicio = '" + Session["FechaInicio"].ToString() + " 00:00:00',\n ");
            consulta.Append("@Fechafin = '" + Session["FechaFin"].ToString() + " 23:59:59',\n ");
            consulta.Append("@LinkSigCenCos = '" + linkCenCos + "',\n ");
            consulta.Append("@LinkEmpleados = '" + linkEmple + "',\n ");
            consulta.Append("@omitirInfoCDR = " + omitirInfoCDR.ToString() + ",\n ");
            consulta.Append("@omitirInfoSiana = " + omitirInfoSiana.ToString() + "\n ");
            consulta.Append(", @isFT = " + isFT + " \n ");

            return consulta.ToString();
        }

        private static DataTable ObtenerCamposConNombreParticular(string nombreMetodoConsulta)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.Append("SELECT MetodoConsulta, NombreOrigCampo, NombreNuevoCampo \r ");
            consulta.Append("FROM " + DSODataContext.Schema + ".CamposConNombreParticular \r ");
            consulta.Append("WHERE dtFinVigencia <> dtIniVigencia \r ");
            consulta.Append("AND dtFinVigencia >= GETDATE() \r ");
            consulta.Append("AND MetodoConsulta = '" + nombreMetodoConsulta + "'");

            return DSODataAccess.Execute(consulta.ToString());
        }

        //20160908 BG Reporte Detalle Rentas Telmex Prs
        public string ConsultaDetalleRentasPrs()
        {
            StringBuilder lsb = new StringBuilder();



            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");

            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [TDest] in('+@ParamTDest+')'\r ");
            lsb.Append("\r ");

            lsb.Append("exec [RepTabConsumoTDestRentasPrs] \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[CtaTelmex],[CenCos],[Descripcion],[Folio],[Tipo],[Costo]', \r ");
            lsb.Append("@Where = @Where \r ");

            return lsb.ToString();
        }

        //20160908 BG Reporte Detalle Enlaces Telmex Prs
        public string ConsultaDetalleEnlacesPrs()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");

            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [TDest] in('+@ParamTDest+')'\r ");
            lsb.Append("\r ");

            lsb.Append("exec [RepTabConsumoTDestEnlacesPrs] \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[CtaTelmex],[CenCos],[Descripcion],[Folio],[Comercio],[Costo],[Importe]', \r ");

            lsb.Append("@Where = @Where \r ");

            return lsb.ToString();
        }

        //20160908 BG Reporte Detalle Uninet Telmex Prs
        public string ConsultaDetalleUninetPrs()
        {
            StringBuilder lsb = new StringBuilder();



            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");

            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [TDest] in('+@ParamTDest+')'\r ");
            lsb.Append("\r ");

            lsb.Append("exec [RepTabConsumoTDestUninetPrs] \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Identificacion de Servicios],[LeyendaCobro],[Importe],[Comercio]', \r ");
            lsb.Append("@Where = @Where \r ");

            return lsb.ToString();
        }

        //20160908 BG Reporte Detalle 800 de Entrada Telmex Prs
        public string ConsultaDetalle800EntPrs()
        {
            StringBuilder lsb = new StringBuilder();


            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");
            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [TDest] in('+@ParamTDest+')'\r ");
            lsb.Append("\r ");

            lsb.Append("exec [RepTabConsumoTDest800EntPrs] \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Cuenta],[LadaTelefono],[No800],[Comercio],[Cencos], \r ");
            lsb.Append("[Llamadas],[Minutos],[Costo]', \r ");

            lsb.Append("@Where = @Where \r ");

            return lsb.ToString();
        }

        //20161003 BG Reporte Detalle Enlaces Axtel Prs
        public string ConsultaDetalleEnlAxtelPrs()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");

            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [TDest] in('+@ParamTDest+')'\r ");
            lsb.Append("\r ");

            lsb.Append("exec [RepTabConsumoTDestEnlacesAxtelPrs] \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Banco],[Referencia],[Descripcion],[Costo]', \r ");
            lsb.Append("@Where = @Where \r ");

            return lsb.ToString();
        }

        //20161003 BG Reporte Detalle Enlaces Avantel Prs
        public string ConsultaDetalleEnlAvantelPrs()
        {
            StringBuilder lsb = new StringBuilder();


            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");

            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [TDest] in('+@ParamTDest+')'\r ");
            lsb.Append("\r ");

            lsb.Append("exec [RepTabConsumoTDestEnlacesAvantelPrs] \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Banco],[Referencia],[CostoFact],[CC]', \r ");
            lsb.Append("@Where = @Where \r ");

            return lsb.ToString();
        }

        //20170116 BG Reporte Detalle Llamadas 800E Avantel Prs
        public string ConsultaDetalle800EAvantelPrs()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)\r ");
            lsb.Append("declare @ParamTDest varchar(max)\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaCorte >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaCorte <= ''" + Session["FechaFin"].ToString() + " 23:59:29''\r ");
            lsb.Append("'\r ");

            lsb.Append("set @ParamTDest = '" + param["TDest"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest <> 'null'\r ");
            lsb.Append("      set @Where = @Where + 'And [TDest] in('+@ParamTDest+')'\r ");
            lsb.Append("\r ");

            lsb.Append("exec [RepTabConsumoTDest800EAvantelPrs] \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Cuenta],[NumDestino],[NumOrigen],[FechaInicio],[DuracionMin],[Importe],[CdDestino]', \r ");
            lsb.Append("@Where = @Where \r ");

            return lsb.ToString();
        }

        //20170216 BG Reporte Detalle Rentas Telnor Prs
        public string ConsultaDetalleRentasTelnorPrs()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @ParamCarrier varchar(max)\r ");

            lsb.Append("set @ParamCarrier = '" + param["Carrier"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("exec [RepDetTelnorRyOCPrs] \r ");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[Descripcion],[CenCos],[Importe]', \r ");
            lsb.Append("@Carrier = @ParamCarrier, \r ");
            lsb.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:29'''\r ");

            return lsb.ToString();
        }

        //20170216 BG Reporte Detalle SM Telnor Prs
        public string ConsultaDetalleSMTelnorPrs()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @ParamCarrier varchar(max)\r ");
            lsb.Append("set @ParamCarrier = '" + param["Carrier"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("exec [RepDetTelnorSMPrs] \r ");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[Descripcion],[CenCos],[Importe]', \r ");
            lsb.Append("@Carrier = @ParamCarrier, \r ");
            lsb.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:29'''\r ");
            return lsb.ToString();
        }

        //20170216 BG Reporte Detalle Enlaces Telnor Prs
        public string ConsultaDetalleEnlTelnorPrs()
        {
            StringBuilder lsb = new StringBuilder();


            lsb.Append("declare @ParamCarrier varchar(max)\r ");
            lsb.Append("set @ParamCarrier = '" + param["Carrier"] + "'\r ");
            lsb.Append("\r ");
            lsb.Append("exec [RepDetTelnorEnlPrs] \r ");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[Descripcion],[CenCos],[Importe]', \r ");
            lsb.Append("@Carrier = @ParamCarrier, \r ");
            lsb.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:29'''\r ");

            return lsb.ToString();
        }

        //20170216 BG Reporte Detalle Enlaces Orange Prs
        public string ConsultaDetalleEnlOrangePrs()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec [RepDetOrangeEnlPrs] \r ");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[Empleado],[CenCos],[Importe]', \r ");
            lsb.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:29'''\r ");

            return lsb.ToString();
        }

        //20170216 BG Reporte Detalle Enlaces Uninet COL Prs
        public string ConsultaDetalleEnlUninetCOLPrs()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec [RepDetUninetCOLEnlPrs] \r ");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[Cia],[Cuenta],[C.C],[Proyecto],[Producto],[Temporal],[Debito Valor],[Credito Valor],[Descripcion]', \r ");
            lsb.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:29'''\r ");

            return lsb.ToString();
        }

        //20170216 BG Reporte Detalle Enlaces Bestel Prs
        public string ConsultaDetalleEnlBestelPrs()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec [RepDetBestelEnlPrs] \r ");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[Concepto],[Importe]', \r ");
            lsb.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\r ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:29'''\r ");

            return lsb.ToString();
        }

        //NZ 20160921 Reporte por tipo de llamada. Banorte.
        public string consultaPorTipoLlamPeEmDetalleBanorte(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("DECLARE @llamsEntrada VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            lsb.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
            lsb.AppendLine("");
            lsb.AppendLine("DECLARE @llamsEnlace VARCHAR(10) = (SELECT CONVERT(VARCHAR,MAX(iCodCatalogo)) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            lsb.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");

            lsb.AppendLine("DECLARE @Where VARCHAR(max)");
            lsb.AppendLine("DECLARE @Empleado VARCHAR(max)");

            lsb.AppendLine("SELECT @Empleado = CONVERT(VARCHAR,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
            lsb.AppendLine("WHERE Usuar = " + Session["iCodUsuario"]);
            lsb.AppendLine("AND dtIniVigencia <= GETDATE()");
            lsb.AppendLine("AND dtFinVigencia > GETDATE()");
            lsb.AppendLine("IF @Empleado is null");
            lsb.AppendLine("BEGIN");
            lsb.AppendLine("	SET @Empleado = '-1'");
            lsb.AppendLine("END");
            lsb.AppendLine("SET @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00''");
            lsb.AppendLine("AND FechaInicio <=''" + Session["FechaFin"].ToString() + " 23:59:59''");
            lsb.AppendLine("AND [Codigo Empleado] = '+@Empleado+' And [Nombre Tipo Destino] <> ''Telcel''");

            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
            {
                lsb.AppendLine("AND [Codigo Tipo Destino] <> ' + @llamsEntrada + '");
                lsb.AppendLine("AND [Codigo Tipo Destino] <> ' + @llamsEnlace + '");
                lsb.AppendLine("AND (LEN([Numero Marcado]) = 3 OR LEN([Numero Marcado]) >= 7)");
            }

            lsb.AppendLine("'");
            lsb.AppendLine("EXEC [ConsumoDetalladoPorTipoLlamadaTodosParamRest] ");
            lsb.AppendLine("@Schema='" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Fields='[Clave Tipo Llamada],");
            lsb.AppendLine("[Tipo Llamada]=MIN([Tipo Llamada]),");
            lsb.AppendLine("[Total] = convert(numeric(15,2),sum(Costo/[TipoCambio])+sum(CostoSM/[TipoCambio]))");
            if (linkGrafica != "")
            {
                lsb.AppendLine("," + linkGrafica + "',");
            }
            else
            {
                lsb.AppendLine("',");
            }
            lsb.AppendLine("@Where =  @Where,");
            lsb.AppendLine("@Group = '[Clave Tipo Llamada]',");
            lsb.AppendLine("@Order = '[Total] DESC',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.AppendLine("@Idioma = 'Español'");
            return lsb.ToString();
        }

        public string ConsultaGetCountNumPorEtiquetar(DateTime fechaInicio, DateTime fechaFin)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("DECLARE @gpoEtiqNoIdent INT = (SELECT GEtiqueta FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            query.AppendLine("							WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()	AND vchCodigo = '0NoIdent' )");
            query.AppendLine("");
            query.AppendLine("DECLARE @emple INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
            query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + ")");
            query.AppendLine("");
            query.AppendLine("DECLARE @llamsEntrada INT;");
            query.AppendLine("DECLARE @llamsEnlace INT;");
            query.AppendLine("DECLARE @llamsLocal INT;");
            query.AppendLine("DECLARE @llamsLDNac INT;");
            query.AppendLine("");
            query.AppendLine("SELECT ");
            query.AppendLine("		@llamsEntrada	= MAX(CASE WHEN vchCodigo = 'Ent' THEN iCodCatalogo ELSE 0 END),");
            query.AppendLine("		@llamsEnlace	= MAX(CASE WHEN vchCodigo = 'Enl' THEN iCodCatalogo ELSE 0 END),");
            query.AppendLine("		@llamsLocal		= MAX(CASE WHEN vchCodigo = 'Local' THEN iCodCatalogo ELSE 0 END),");
            query.AppendLine("		@llamsLDNac		= MAX(CASE WHEN vchCodigo = 'LDNac' THEN iCodCatalogo ELSE 0 END)");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine("");
            query.AppendLine("SELECT COUNT(DISTINCT TelDest)");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')]");
            query.AppendLine("WHERE Emple = @emple");
            query.AppendLine("	  AND GEtiqueta = @gpoEtiqNoIdent");
            query.AppendLine("	  AND FechaInicio >= '" + fechaInicio.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            query.AppendLine("    AND FechaInicio < '" + fechaFin.ToString("yyyy-MM-dd") + " 23:59:59'");
            query.AppendLine("    AND TDest <> @llamsEntrada");
            query.AppendLine("    AND TDest <> @llamsEnlace");

            if (DSODataContext.Schema.ToLower() == "cide")
            {
                query.AppendLine("    AND TDest <> @llamsLocal");
                query.AppendLine("    AND TDest <> @llamsLDNac");
            }

            return query.ToString();
        }

        public string ConsultaReportePorTDestDashboard(string linkGrid, string linkGrafica)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec RepTabConsumoPorTDestOptFC ");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@Link = '" + linkGrid + "',");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string ConsultaReporteTopNEmpleDashboard(string linkGrid, string linkGrafica, int numeroRegistros)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec RepTabTopNEmpleadosOptFC ");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("  @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("  @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("  @Link = '" + linkGrid + "',");
            query.AppendLine("  @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("  @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("  @Lenght = " + numeroRegistros.ToString() + "\n");

            return query.ToString();
        }

        public string ConsultaConsumoHistoricoDashboard(string linkGrid, string linkGrafica)
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("exec ConsumoHistoricoOptFC ");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("  @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("  @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("  @Link = '" + linkGrid + "',");
            query.AppendLine("  @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("  @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string ConsultaAccesosAgrupado()
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("Declare @fechaInicioRep varchar(19)= '" + Session["FechaInicio"].ToString() + " 00:00:00'");
            query.AppendLine("Declare @fechaFinRep varchar(19) ='" + Session["FechaFin"].ToString() + " 23:59:59'");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("SELECT *");
            query.AppendLine("FROM");
            query.AppendLine("	(SELECT ");
            query.AppendLine("");
            query.AppendLine("		[Codigo Usuario]				= Detall.[Usuar],");
            query.AppendLine("		[Usuario]						= Usuar.[vchCodigo],");
            query.AppendLine("		[Descripcion Usuario]			= Usuar.[vchDescripcion],	");
            query.AppendLine("		[Cantidad Accesos]              = count(*)");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("	From [" + DSODataContext.Schema + "].RegSesion Detall");
            query.AppendLine("	INNER JOIN [" + DSODataContext.Schema + "].[vishistoricos('usuar','usuarios','español')] usuar");
            query.AppendLine("		ON Detall.Usuar = usuar.iCodCatalogo");
            query.AppendLine("		AND usuar.dtIniVigencia <> usuar.dtFinVigencia ");
            query.AppendLine("		AND usuar.dtFinVigencia >= GETDATE()");
            query.AppendLine("");
            query.AppendLine("		Where Detall.[FecAcc] between  @fechaInicioRep and  @fechaFinRep");
            query.AppendLine("		group by Detall.[Usuar],");
            query.AppendLine("				 Usuar.[vchCodigo],");
            query.AppendLine("				 Usuar.[vchDescripcion]");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("				 ) As Rep");
            query.AppendLine("order by [Cantidad Accesos] Desc");

            return query.ToString();
        }

        public string ConsultaAccesosAgrupadoN2(string Usuario)
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("Declare @fechaInicioRep varchar(19)= '" + Session["FechaInicio"].ToString() + " 00:00:00'");
            query.AppendLine("Declare @fechaFinRep varchar(19) ='" + Session["FechaFin"].ToString() + " 23:59:59'");
            query.AppendLine("Declare @Usuario varchar(max) = '" + Usuario + "'");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("SELECT *");
            query.AppendLine("FROM");
            query.AppendLine("	(SELECT ");
            query.AppendLine("");
            query.AppendLine("		[Codigo Usuario]				= Detall.[Usuar],");
            query.AppendLine("		[Usuario]						= Usuar.[vchCodigo],");
            query.AppendLine("		[Descripcion Usuario]			= Usuar.[vchDescripcion],	");
            query.AppendLine("      [Fecha Acceso]                  = convert(varchar,Detall.FecAcc,120)");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("	From [" + DSODataContext.Schema + "].RegSesion Detall");
            query.AppendLine("	INNER JOIN [" + DSODataContext.Schema + "].[vishistoricos('usuar','usuarios','español')] usuar");
            query.AppendLine("		ON Detall.Usuar = usuar.iCodCatalogo");
            query.AppendLine("		AND usuar.dtIniVigencia <> usuar.dtFinVigencia ");
            query.AppendLine("		AND usuar.dtFinVigencia >= GETDATE()");
            query.AppendLine("");
            query.AppendLine("		Where Detall.[FecAcc] between  @fechaInicioRep and  @fechaFinRep");
            query.AppendLine("      and usuar.vchCodigo = @Usuario");
            query.AppendLine("				 ) As Rep");
            query.AppendLine("      order by [Fecha Acceso]");

            return query.ToString();
        }

        public string ConsultaRepTabResumenSpeedDial(string linkGrid)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("DECLARE @Where varchar(max)");
            query.AppendLine("SET @Where = '([Codigo Autorizacion] IS NULL OR [Codigo Autorizacion] = '''')");
            query.AppendLine("AND ([Costo] + [CostoSM]) > 0");
            query.AppendLine("AND [Extension] <> ''''");
            query.AppendLine("'");
            query.AppendLine("");
            query.AppendLine("EXEC ReporteDetalladoSpeedDial ");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields='[Sitio],");
            query.AppendLine("[SitioDesc],");
            query.AppendLine("[Numero Marcado],");
            query.AppendLine("[Llamadas] = COUNT(*),");
            query.AppendLine("[Minutos] = SUM([Duracion]),");
            query.AppendLine("[Costo]= convert(money,sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio])),");
            //query.AppendLine("[Costo]=SUM([Costo]) + SUM([CostoSM]), ");
            query.AppendLine("link=''" + linkGrid + "', ");
            query.AppendLine("@Where = @Where,");
            query.AppendLine("@Group = '[Sitio],[SitioDesc], [Numero Marcado]',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',\n ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }

        private string ConsultaGetDirectorioSpeedDials()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ");
            query.AppendLine("	[Número Marcado]		= '''' + NumMarcadoReal,");
            query.AppendLine("	[Speed Dial]			= ISNULL(SpeedDial,''),");
            query.AppendLine("	[Inicio de Vigencia]	= CONVERT(VARCHAR,dtIniVigencia,105),");
            query.AppendLine("	[Fin de Vigencia]		= CASE WHEN DATEPART(YEAR,dtFinVigencia) = 2079 THEN '' ELSE CONVERT(VARCHAR,dtFinVigencia,105) END	");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('SpeedDial','Speed Dials','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("ORDER BY iCodCatalogo");

            return query.ToString();
        }

        public string ConsultaRepMatConsumoPorCampañaTipoDest()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC RepConsumoPorCampañaYTipoDest ");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + "");
            return query.ToString();
        }

        public string ConsultaReporteTraficoPorHora()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("exec ObtieneTraficoPorHora");
                query.AppendLine("		@esquema = '" + DSODataContext.Schema + "',");
                query.AppendLine("		@sitio = " + ((param["Sitio"] != null && param["Sitio"].Length > 0) ? param["Sitio"] : "0") + ",");
                query.AppendLine("		@fechaInicio = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
                query.AppendLine("		@fechaFin = '" + Session["FechaFin"].ToString() + " 23:59:59'");


                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaBuscaSitios()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("Select ");
                query.AppendLine("	nivel =0,");
                query.AppendLine("	iCodCatalogo	=	0,");
                query.AppendLine("	vchCodigo		=	'TODOS',");
                query.AppendLine("	vchDescripcion = 'TODOS'");
                query.AppendLine("");
                query.AppendLine("Union ");
                query.AppendLine("");
                query.AppendLine("Select ");
                query.AppendLine("	nivel =1,");
                query.AppendLine("	iCodCatalogo,");
                query.AppendLine("	vchCodigo,");
                query.AppendLine("	vchDescripcion");
                query.AppendLine("From [" + DSODataContext.Schema + "].[visHisComun('sitio','Español')]");
                query.AppendLine("Where dtinivigencia <> dtFinVigencia ");
                query.AppendLine("And dtFinVigencia >= GETDATE()");
                query.AppendLine("And vchCodigo <> '99999999'");
                query.AppendLine("");
                query.AppendLine("order by ");
                query.AppendLine("	nivel,vchCodigo");

                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string RepTabLlamadasBuzonDeVoz()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC RepTabLlamadasBuzonDeVoz");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields = '[Fecha],[Segundos],[Minutos],[LlamsBuzon]', \n ");
            query.AppendLine("@Order = '[Fecha] asc, [Segundos] Desc', \n ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',\n ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }

        public string RepTabDetalleLlamsBuzonDeVoz()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC RepTabDetalleLlamsBuzonDeVoz");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields = '[Fecha],[Hora],[Duracion],[NumMarcado],[Extension],[Buzon],[TipoLlam],[ExtensionIntermedia],[ExtensionInicial]', \n ");
            query.AppendLine("@Order = '[Fecha] asc, [Hora] asc', \n ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',\n ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }

        public string RepEstTabCodEnMasDeNExtensiones()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Group varchar(max)");
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamSitio varchar(max)");
            query.AppendLine("declare @ParamExtension varchar(max)");
            query.AppendLine("declare @ParamTDest varchar(max)");
            query.AppendLine("declare @ParamCantExt varchar(max)");

            query.AppendLine("");
            query.AppendLine("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' and [Codigo Autorizacion] is not null and [Codigo Autorizacion] <> ''''");
            query.AppendLine("'");

            query.AppendLine("set @ParamCantExt = 4");
            query.AppendLine("");
            query.AppendLine("set @Group = '[Codigo Autorizacion],[Nombre Completo],[Codigo Empleado],[No Nomina],[Nombre Centro de Costos],[Codigo Centro de Costos],[Nombre Sitio],[Codigo Sitio]  Having count(distinct [Extension]) >=  ' + @ParamCantExt");
            query.AppendLine("");
            query.AppendLine("exec ConsumoDetalladoRespCodAutoRest @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("			                           @Fields='''''''''+[Codigo Autorizacion] as [Codigo Autorizacion],[Nombre Completo],[Codigo Empleado],[No Nomina],[Total]=SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]),[Llamadas] = count(*),[Duracion Minutos] = SUM([Duracion Minutos]),CantCod=count(distinct [Extension]),[Nombre Centro de Costos],[Codigo Centro de Costos],[Nombre Sitio],[Codigo Sitio]', ");
            query.AppendLine("			                           @Where = @Where,");
            query.AppendLine("			                           @Group = @Group, ");
            query.AppendLine("			                           @Order = '[CantCod] Desc,[Total] Desc,[Duracion Minutos] Desc,[Llamadas] Desc,[Nombre Centro de Costos] Asc,[Nombre Sitio] Asc,[Codigo Autorizacion] Asc,[Nombre Completo] Asc,[No Nomina] Asc',");
            query.AppendLine("			                           @OrderInv = '[CantCod] Asc,[Total] Asc,[Duracion Minutos] Asc,[Llamadas] Asc,[Nombre Centro de Costos] Desc,[Nombre Sitio] Desc,[Codigo Autorizacion] Desc,[Nombre Completo] Desc,[No Nomina] Desc',");
            query.AppendLine("			                           @Start = 0,");
            query.AppendLine("			                           @OrderDir = 'Desc',");
            query.AppendLine("			                           @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("			                           @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("  @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("  @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'");

            return query.ToString();
        }

        public string RepEstTabCodEnMasDeNExtensionesN2()
        {
            StringBuilder query = new StringBuilder();

            var sitio = param["Sitio"] != null ? param["Sitio"].Trim('\'') : "0";
            var codigo = param["Codigo"] != null ? param["Codigo"].Trim('\'') : "0";


            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamSitio varchar(max)");
            query.AppendLine("declare @ParamCodAut varchar(max)");

            query.AppendLine("set @ParamSitio = '" + sitio + "'");
            query.AppendLine("set @ParamCodAut = '" + codigo + "'");

            query.AppendLine("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''");
            query.AppendLine("'");

            query.AppendLine("if @ParamSitio <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')");
            query.AppendLine("'");
            query.AppendLine("");
            query.AppendLine("if @ParamCodAut <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')");
            query.AppendLine("'");

            query.AppendLine("exec ConsumoDetalladoTodosCamposRest     @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Extension],[Fecha],[Hora],[Duracion]=[Duracion Minutos],[Total]= (([Costo]+[CostoSM])/[TipoCambio]),[Nombre Localidad],''''''''+[Numero Marcado] as [Numero Marcado],[Tipo Llamada],[Clave Tipo Llamada]', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Order = '[Total] Desc,[Duracion] Desc,[Nombre Localidad] Asc,[Hora] Asc,[Fecha] Asc,[Extension] Asc,[Numero Marcado] Asc,[Tipo Llamada] Asc',");
            query.AppendLine(" @OrderInv = '[Total] Asc,[Duracion] Asc,[Nombre Localidad] Desc,[Hora] Desc,[Fecha] Desc,[Extension] Desc,[Numero Marcado] Desc,[Tipo Llamada] Desc',");
            query.AppendLine(" @Start = 0,");
            query.AppendLine(" @OrderDir = 'Desc',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");

            return query.ToString();
        }

        public string RepMatDiasProcesados()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec RepMatDiasProcesados @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("										@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("										@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("										@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("										@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("										@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("										@Idioma = 'Español'");

            return query.ToString();
        }

        public string RepTabCodigosNoAsignados()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");

            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'");

            query.AppendLine("exec CodigosPorIdentificar @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("      @Fields='[Codigo Autorizacion],[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("              [Numero]=count(*),[Duracion]=sum([Duracion Minutos]),[Nombre Sitio],[Codigo Sitio],");

            query.AppendLine(" Link = ''" + Request.Path + "?Nav=RepTabCodigosNoAsignadosN2&Codigo='' +[Codigo Autorizacion] + ''&Sitio='' + convert(varchar,[Codigo Sitio])',");

            query.AppendLine("		@Where = @Where,");
            query.AppendLine("		@Group = '[Codigo Autorizacion],[Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine("		@Start = 0,");
            query.AppendLine("		@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("		@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("		@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("		@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("      @Idioma = 'Español'");

            return query.ToString();
        }

        public string RepTabCodigosNoAsignadosN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamSitio varchar(max)");
            query.AppendLine("declare @ParamCodAut varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamSitio = '" + param["Sitio"] + "'");
            query.AppendLine("set @ParamCodAut = '''" + param["Codigo"] + "'''");
            query.AppendLine("");
            query.AppendLine("set @Where = 'FechaInicio >=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' and [Clave Empleado] like ''%identificar%'''");
            query.AppendLine("");
            query.AppendLine("if @ParamSitio <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')'");
            query.AppendLine("");
            query.AppendLine("if @ParamCodAut <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')'");
            query.AppendLine("");
            query.AppendLine("exec ConsumoDetalladoTodosCamposRest @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("		@Fields='[Extension],'''''''' + convert(varchar,[Codigo Autorizacion]) AS [Codigo Autorizacion],[Fecha],[Hora],[Duracion]=[Duracion Minutos],");
            query.AppendLine("				[Total]= (([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),[Nombre Localidad]=upper([Nombre Localidad]),'''''''' + convert(varchar, [Numero Marcado]) AS [Numero Marcado],");
            query.AppendLine("				[Tipo Llamada]=upper([Tipo Llamada]),[Clave Tipo Llamada]', ");
            query.AppendLine("		@Where = @Where,");
            query.AppendLine("		@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("		@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("		@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("		@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("      @Idioma = 'Español'");

            return query.ToString();
        }

        public string RepTabExtensionesNoAsignadas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");

            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");

            query.AppendLine("exec ExtensionesPorIdentificar @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("      @Fields='[Extension],[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("              [Numero]=count(*),[Duracion]=sum([Duracion Minutos]),[Nombre Sitio],[Codigo Sitio], Link=''" + Request.Path + "?Nav=RepTabExtensionesNoAsignadasN2&Extension=''+[Extension]+''&Sitio=''+convert(varchar,[Codigo Sitio])',");
            query.AppendLine("		@Where = @Where,");
            query.AppendLine("		@Group = '[Extension],[Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine("		@Start = 0,");
            query.AppendLine("		@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("		@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("		@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("		@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("      @Idioma = 'Español'");

            return query.ToString();
        }

        public string RepTabExtensionesNoAsignadasN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamSitio varchar(max)");
            query.AppendLine("declare @ParamExtension varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamSitio = '" + param["Sitio"] + "'");
            query.AppendLine("set @ParamExtension = '''" + param["Extension"] + "'''");
            query.AppendLine("");
            query.AppendLine("set @Where = 'FechaInicio >=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' and [Clave Empleado] like ''%identificar%'''");
            query.AppendLine("");
            query.AppendLine("if @ParamSitio <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')'");
            query.AppendLine("");
            query.AppendLine("if @ParamExtension <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')'");
            query.AppendLine("");
            query.AppendLine("exec ConsumoDetalladoTodosCamposRest @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("		@Fields='[Extension],'''''''' + convert(varchar,[Codigo Autorizacion]) AS [Codigo Autorizacion],[Fecha],[Hora],[Duracion]=[Duracion Minutos],");
            query.AppendLine("				[Total]= (([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),[Nombre Localidad]=upper([Nombre Localidad]),'''''''' + convert(varchar, [Numero Marcado]) AS [Numero Marcado],");
            query.AppendLine("				[Tipo Llamada]=upper([Tipo Llamada]),[Clave Tipo Llamada]', ");
            query.AppendLine("		@Where = @Where,");
            query.AppendLine("		@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("		@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("		@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("		@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("      @Idioma = 'Español'");

            return query.ToString();
        }

        public string RepMatConsumoPorCarrier()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");

            query.AppendLine("exec [RepMatConsumoPorCarrier]");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorCarrierN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorSitio]");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Carrier] = " + param["Carrier"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorCarrierN3()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorCenCos] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Carrier] = " + param["Carrier"] + " AND [Codigo Sitio] = " + param["Sitio"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorCarrierN4()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorEmple] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            if(DSODataContext.Schema.ToLower() =="fca")
            {
                query.AppendLine("	@where = ' [Codigo Carrier] = " + param["Carrier"] + " AND [Codigo Sitio] = " + param["Sitio"] + " ',");
            }
            else
            {
                query.AppendLine("	@where = ' [Codigo Carrier] = " + param["Carrier"] + " AND [Codigo Sitio] = " + param["Sitio"] + "  AND [Codigo Centro de Costos] = " + param["CenCos"] + " ',");
            }
            
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorEmple()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorEmple] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorEmpleN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorSitio] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Emple] = " + param["Emple"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorSitio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorSitio] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorSitioN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorCenCos] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Sitio] = " + param["Sitio"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorSitioN3()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorEmple] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            if(DSODataContext.Schema.ToLower()=="fca")
            {
                query.AppendLine("	@where = ' [Codigo Sitio] = " + param["Sitio"] + " ',");
            }
            else
            {
                query.AppendLine("	@where = ' [Codigo Sitio] = " + param["Sitio"] + " AND [Codigo Centro de Costos] = " + param["CenCos"] + " ',");
            }
            

            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorCenCosSJ()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorCenCos] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorCenCosSJN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorEmple] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Centro de Costos] = " + param["CenCos"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatConsumoPorCenCosSJN3()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorSitio] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Centro de Costos] = " + param["CenCos"] + " AND [Codigo Emple] = " + param["Emple"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string ConsEmpsMasCarosN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorSitio] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Emple] = " + param["Emple"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string ConsumoEmpsMasTiempoN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorSitio] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Emple] = " + param["Emple"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }


        #region Consultas JH y RM

        public string ConsumoEmpsMasTiempo(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)");

            query.AppendLine("set @Where = 'FechaInicio>= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine(" '");
            query.AppendLine("  exec TopEmpleadosMasDuracionAlTel     @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("  @Fields='[Nombre Completo]=Min(upper([Nombre Completo])),[Codigo Empleado],[Duracion]=sum([Duracion Minutos]) ");
            if (linkGrafica != string.Empty)
            {
                query.AppendLine("  ,[link] = ''/UserInterface/DashboardFC/Dashboard.aspx?Nav=ConsumoEmpsMasTiempoN2&Emple='' + convert(varchar,[Codigo Empleado])");
            }
            query.AppendLine("', \n");
            query.AppendLine("			                           @Where = @Where,");
            query.AppendLine("			                           @Lenght = 10,");
            query.AppendLine("			                           @Group = '[Codigo Empleado]', ");
            query.AppendLine("			                           @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("			                           @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("			                           @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("			                           @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("      @Idioma = 'Español',");
            query.AppendLine("      @Order = 'Duracion desc'");


            return query.ToString();
        }

        public string RepTabMenuConsLLamadasMasTiempo()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("and [Codigo Tipo Destino] not in (381, 382) --ent, enl");
            query.AppendLine("'");
            query.AppendLine("  exec ConsumoDetalladoTodosCamposCDRRest @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("  @Fields='[Extension],[Nombre Completo]=upper([Nombre Completo]),");
            query.AppendLine("[Codigo Empleado],[Numero Marcado],[Fecha],[Duracion Minutos],");
            query.AppendLine("[Costo]=([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio]),");
            query.AppendLine("[Nombre Localidad]=upper([Nombre Localidad]),[Codigo Localidad],");
            query.AppendLine("[Tipo Llamada]=upper([Tipo Llamada]),[Clave Tipo Llamada],''''''''+[Numero Marcado] AS NumMarcado',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Lenght = 10,");
            query.AppendLine(" @Order = '[Duracion Minutos] DESC', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepEstTabExtAbiertas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine(" and ([Codigo Autorizacion] is null or [Codigo Autorizacion] = '''')  ");
            query.AppendLine(" and ([Costo] > 0)");
            query.AppendLine(" and ([Extension] <> '''')");
            query.AppendLine("'");
            query.AppendLine(" exec ConsumoDetalladoTodosCamposRest  @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Extension],[Llamadas] = count(*),");
            query.AppendLine(" [Costo]=SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]), [ExtSitio]= [Extension] + '' ''+ [Nombre Sitio],");
            query.AppendLine(" [Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Extension],[Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPorEmpleado()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            query.AppendLine(" exec ConsumoAcumuladoPorEmpleadoRest @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields ='[Nombre Centro de Costos],[Codigo Centro de Costos],");
            query.AppendLine(" [Nombre Completo] = UPPER([Nombre Completo]), [Codigo Empleado],");
            query.AppendLine(" [CostoFija] = ROUND(Sum(Case When [Nombre Categoria Tipo Destino] in (''Fija'') then (([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])) else 0 end),2),");
            query.AppendLine(" [Tel Fija] = ''85763'',");
            query.AppendLine(" [CostoMovil] = ROUND(Sum(Case When [Nombre Categoria Tipo Destino] in (''Movil'') then ([Costo]/[TipoCambio]) +( [CostoSM]/[TipoCambio]) else 0 end),2),");
            query.AppendLine(" [Tel Movil] = ''85764'',");
            query.AppendLine(" [Total] = ROUND(Sum(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),2)', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Nombre Centro de Costos],[Codigo Centro de Costos],[Nombre Completo],[Codigo Empleado]', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPorEmpleadoN2(string linkGrafica)
        {
            string group = "";
            string fields = "";
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" declare @ParamEmple varchar(max)");
            query.AppendLine(" declare @ParamCC varchar(max)");
            query.AppendLine(" set @ParamEmple = '" + param["Emple"] + "' ");
            query.AppendLine(" set @ParamCC = '" + param["CenCos"] + "' ");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            if (DSODataContext.Schema.ToLower() =="fca")
            {                
                query.AppendLine(" set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')'");
                group = "[Codigo Tipo Destino],[Codigo Empleado]";
                fields = "[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),[Codigo Tipo Destino]";
            }
            else
            {                
                query.AppendLine(" set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')'+'And [Codigo Centro de Costos] in('+@ParamCC+')'");
                group = "[Codigo Tipo Destino],[Codigo Centro de Costos],[Codigo Empleado]";
                fields = "[Codigo Centro de Costos],[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])), [Codigo Tipo Destino]";
            }

            query.AppendLine(" exec ConsumoAcumuladoPorTDestTodosParamRest     @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '"+ fields + ",");
            query.AppendLine(" [Total] = sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), ");
            query.AppendLine(" [Numero]= sum([TotalLlamadas]),[Duracion]=sum([Duracion Minutos]),[Codigo Empleado] ");
            if (linkGrafica != "")
            {
                query.AppendLine("," + linkGrafica + " \n ");
            }
            query.AppendLine(" ,@Where = @Where,");
            query.AppendLine(" @Group = '"+ group + "', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPorEmpleadoN3(string linkGrafica)
        {
            string group = "";
            string fields = "";
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" declare @ParamEmple varchar(max)");
            query.AppendLine(" declare @ParamCenCos varchar(max)");
            query.AppendLine(" declare @ParamTDest varchar(max)");
            query.AppendLine(" set @ParamEmple = '" + param["Emple"] + "' ");
            query.AppendLine(" set @ParamCenCos = '" + param["CenCos"] + "' ");
            query.AppendLine(" set @ParamTDest = '" + param["TDest"] + "'");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");

            if (DSODataContext.Schema.ToLower() == "fca")
            {
                query.AppendLine(" set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') ' + 'And [Codigo Tipo Destino] in('+@ParamTDest+')'");
                group = "[Numero Marcado],[Clave Tipo Llamada],[Codigo Empleado],[Codigo Tipo Destino]";
                fields = "[Tipo Llamada]=Min(upper([Tipo Llamada])),[Clave Tipo Llamada],[Codigo Empleado],[Codigo Tipo Destino], ''''''''+[Numero Marcado] AS NumMarcado, ";
            }
            else
            {
                query.AppendLine(" set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') ' + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')' + 'And [Codigo Tipo Destino] in('+@ParamTDest+')'");
                group = "[Numero Marcado],[Clave Tipo Llamada],[Codigo Centro de Costos],[Codigo Empleado],[Codigo Tipo Destino]";
                fields = "[Tipo Llamada]=Min(upper([Tipo Llamada])),[Clave Tipo Llamada],[Codigo Centro de Costos],[Codigo Empleado],[Codigo Tipo Destino], ''''''''+[Numero Marcado] AS NumMarcado, ";
            }            
           
            query.AppendLine(" exec ConsumoAcumuladoPorNumMarcado    @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Numero Marcado],");
            query.AppendLine(" [Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine(" [Numero]=count(*),[Duracion]=sum([Duracion Minutos]),");
            query.AppendLine(fields);

            if (linkGrafica != "")
            {
                query.AppendLine(linkGrafica + "," + " \n ");
            }

            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '"+ group + "', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPorEmpleadoN4()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" declare @ParamEmple varchar(max)");
            query.AppendLine(" declare @ParamCenCos varchar(max)");
            query.AppendLine(" declare @ParamTelDest varchar(max)");
            query.AppendLine(" declare @ParamGEtiqueta varchar(max)");
            query.AppendLine(" declare @ParamTDest varchar(max)");

            query.AppendLine(" set @ParamEmple = '" + param["Emple"] + "' ");
            query.AppendLine(" set @ParamCenCos = '" + param["CenCos"] + "'");
            query.AppendLine(" set @ParamTelDest = '''" + param["NumMarc"] + "'''");
            query.AppendLine(" set @ParamGEtiqueta = '" + param["TipoLlam"] + "'");
            query.AppendLine(" set @ParamTDest = '" + param["TDest"] + "'");

            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");

            query.AppendLine(" if @ParamEmple is not null and @ParamEmple <> '' begin set @Where = @Where + ' And [Codigo Empleado] in('+@ParamEmple+') ' end ");
            query.AppendLine(" if @ParamCenCos is not null and @ParamCenCos <> '' begin set @Where = @Where + ' And [Codigo Centro de Costos] in('+@ParamCenCos+') ' end ");
            query.AppendLine(" if @ParamTelDest is not null and @ParamTelDest <> '' begin set @Where = @Where + ' And [Numero Marcado] in('+@ParamTelDest+') ' end ");
            query.AppendLine(" if @ParamGEtiqueta is not null and @ParamGEtiqueta <> '' begin set @Where = @Where + ' And [Clave Tipo Llamada] in('+@ParamGEtiqueta+') ' end ");
            query.AppendLine(" if @ParamTDest is not null and @ParamTDest <> '' begin set @Where = @Where + ' And [Codigo Tipo Destino] in('+@ParamTDest+') ' end ");


            query.AppendLine(" exec ConsumoDetalladoTodosCamposRestSinLinea  @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Extension],");
            query.AppendLine(" [Fecha],[Hora],[Duracion]=[Duracion Minutos],");
            query.AppendLine(" [Total]=(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),");
            query.AppendLine(" [Nombre Localidad]=upper([Nombre Localidad]),");
            query.AppendLine(" [Numero Marcado],[Tipo Llamada]=upper([Tipo Llamada]),");
            query.AppendLine(" [Clave Tipo Llamada],[Etiqueta],''''''''+[Numero Marcado] AS NumMarcado', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPorEmpleadoGraf()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            query.AppendLine(" exec ConsumoAcumuladoPorEmpleadoRest @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='ISNULL([Nombre Categoria Tipo Destino],'''') AS [Nombre Categoria Tipo Destino],");
            query.AppendLine(" Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', ");
            query.AppendLine(" @Group = 'ISNULL([Nombre Categoria Tipo Destino],'''')',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPobmasMindeConv()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            query.AppendLine(" exec ConsumoAcumuladoTodosCamposRest   @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Nombre Localidad]=Min(upper([Nombre Localidad])),");
            query.AppendLine(" [Codigo Localidad],");
            query.AppendLine(" [Costo]=Sum([Costo]/[TipoCambio])+Sum([CostoSM]/[TipoCambio]),");
            query.AppendLine(" [Duracion]=sum([Duracion Minutos]),");
            query.AppendLine(" TotLlam = sum([TotalLlamadas])', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Codigo Localidad]', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPobmasMindeConvN2(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" declare @ParamLocali varchar(max)");
            query.AppendLine(" set @ParamLocali = '" + param["Locali"] + "'");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            query.AppendLine(" set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')'");
            query.AppendLine(" exec ConsumoAcumuladoPorEmpleadoTodosParamRest  @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Nombre Completo]=Min(upper([Nombre Completo])),[Codigo Empleado],");
            query.AppendLine(" TotImporte = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]),");
            query.AppendLine(" LLamadas = sum([TotalLlamadas]),TotMin = SUM([Duracion Minutos]),[Codigo Localidad], ");
            if (linkGrafica != "")
            {
                query.AppendLine(linkGrafica + "," + " \n ");
            }
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Codigo Empleado],[Codigo Localidad]',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPobmasMindeConvN3(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" declare @ParamEmple varchar(max)");
            query.AppendLine(" declare @ParamLocali varchar(max)");
            query.AppendLine(" set @ParamEmple = '" + param["Emple"] + "'");
            query.AppendLine(" set @ParamLocali = '" + param["Locali"] + "'");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            query.AppendLine(" set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')'");
            query.AppendLine(" set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')'");
            query.AppendLine(" exec ConsumoAcumuladoTodosCamposRest  @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Tipo Llamada]=Min(upper([Tipo Llamada])),");
            query.AppendLine(" [Clave Tipo Llamada],");
            query.AppendLine(" TotImporte = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]),");
            query.AppendLine(" TotLlam = sum([TotalLlamadas]),");
            query.AppendLine(" TotMin = SUM([Duracion Minutos]),[Codigo Empleado],[Codigo Localidad], ");
            if (linkGrafica != "")
            {
                query.AppendLine(linkGrafica + "," + " \n ");
            }
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Clave Tipo Llamada],[Codigo Empleado],[Codigo Localidad]', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabConsPobmasMindeConvN4()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamEmple varchar(max)");
            query.AppendLine("declare @ParamLocali varchar(max)");
            query.AppendLine(" declare @ParamGEtiqueta varchar(max)");
            query.AppendLine(" set @ParamEmple = '" + param["Emple"] + "'");
            query.AppendLine(" set @ParamLocali = '" + param["Locali"] + "'");
            query.AppendLine(" set @ParamGEtiqueta = '" + param["TipoLlam"] + "'");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            query.AppendLine(" set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')'");
            query.AppendLine(" set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')'");
            query.AppendLine(" set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')'");
            query.AppendLine(" exec ConsumoDetalladoTodosCamposRestSinLinea  @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields='[Extension],[Fecha],[Hora],[Duracion]=[Duracion Minutos],");
            query.AppendLine(" [Total]=(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),");
            query.AppendLine(" [Nombre Localidad]=upper([Nombre Localidad]),");
            query.AppendLine(" [Numero Marcado],[Tipo Llamada]=upper([Tipo Llamada]),");
            query.AppendLine(" [Clave Tipo Llamada],[Etiqueta],''''''''+[Numero Marcado] AS NumMarcado', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string RepTabLLamsFueraHoraLaboral(string WhereHorario)
        {
            StringBuilder query = new StringBuilder();

            WhereHorario = WhereHorario.Length > 0 ? WhereHorario : "And (Hora < ''08:30:00'' or Hora > ''18:30:00'')";


            query.AppendLine("declare @ParamSitio varchar(max)																																								");
            query.AppendLine("declare @ParamCenCos varchar(max)																																								");
            query.AppendLine("declare @ParamTDest varchar(max)																																								");
            query.AppendLine("declare @Where varchar(max)																																									");
            query.AppendLine("declare @HoraInicio varchar(max)																																								");
            query.AppendLine("declare @HoraFin varchar(max)																																									");
            query.AppendLine("																																																");
            query.AppendLine("set @ParamCenCos = 'null'																																										");
            query.AppendLine("set @ParamSitio = 'null'																																										");
            query.AppendLine("set @ParamTDest = 'null'																																										");
            query.AppendLine("set @HoraInicio = 'null'																																										");
            query.AppendLine("set @HoraFin = 'null' 																																										");
            query.AppendLine("																																																");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' 																									");
            query.AppendLine("'																																																");
            query.AppendLine("																																																");
            query.AppendLine("if @HoraInicio = 'null' and @HoraFin = 'null'																																					");
            query.AppendLine("begin																																															");
            query.AppendLine("set @Where = @Where + '" + WhereHorario + "' 																														");
            query.AppendLine("end																																															");
            query.AppendLine("else																																															");
            query.AppendLine("set @Where = @Where + 'And (Hora < ' + @HoraInicio + ' or Hora > ' + @HoraFin + ')																											");
            query.AppendLine("'																																																");
            query.AppendLine("																																																");
            query.AppendLine("if @ParamCenCos <> 'null'																																										");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')																												");
            query.AppendLine("'																																																");
            query.AppendLine("if @ParamSitio <> 'null'																																										");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')																															");
            query.AppendLine("'																																																");
            query.AppendLine("																																																");
            query.AppendLine("if @ParamTDest <> 'null'																																										");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')																													");
            query.AppendLine("'																																																");
            query.AppendLine("		exec ConsumoDetalladoTodosCamposRest    																																				");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',																																								");
            query.AppendLine("		@Fields='																																												");
            query.AppendLine("			[Extension],																																										");
            query.AppendLine("			[Nombre Completo]=upper([Nombre Completo]),																																			");
            query.AppendLine("			[Codigo Empleado],																																									");
            query.AppendLine("			[Numero Marcado],																																									");
            query.AppendLine("			[Fecha],																																											");
            query.AppendLine("			[Hora],																																												");
            query.AppendLine("			[Duracion Minutos],																																									");
            query.AppendLine("			[Costo]=([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio]),																															");
            query.AppendLine("			[Nombre Centro de Costos]=upper([Nombre Centro de Costos]),																															");
            query.AppendLine("			[Codigo Centro de Costos]', 																																						");
            query.AppendLine("		@Where = @Where,																																										");
            query.AppendLine("		@Group = '', 																																											");
            query.AppendLine("		@Order = '[Extension] Asc,[Nombre Completo] Asc,[Numero Marcado] Asc,[Fecha] Asc,[Hora] Asc,[Nombre Centro de Costos] Asc,[Duracion Minutos] Desc,[Costo] Desc',						");
            query.AppendLine("		@OrderInv = '[Extension] Desc,[Nombre Completo] Desc,[Numero Marcado] Desc,[Fecha] Desc,[Hora] Desc,[Nombre Centro de Costos] Desc,[Duracion Minutos] Asc,[Costo] Asc',					");
            query.AppendLine("		@Start = 0,																																												");
            query.AppendLine("		@OrderDir = 'Asc',																																										");
            query.AppendLine("		@Usuario = " + Session["iCodUsuario"] + ",															");
            query.AppendLine("		@Perfil = " + Session["iCodPerfil"] + ",															");
            query.AppendLine("		@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',						");
            query.AppendLine("		@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',							");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("		@Idioma = 'Español'																																										");


            return query.ToString();

        }

        public string ConsLugmasCost(int ConsultaDetallFactura = 0)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' '");
            query.AppendLine("exec ConsumoAcumuladoTodosCamposRest");
            query.AppendLine("	@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("  @Fields='");
            query.AppendLine("	[Nombre Localidad]=Min(upper([Nombre Localidad])),");
            query.AppendLine("	[Codigo Localidad],");
            query.AppendLine("	[Costo]=Sum([Costo]/[TipoCambio])+Sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos]),");
            query.AppendLine("	TotLlam = sum([TotalLlamadas]), ");
            query.AppendLine("	Link=''/UserInterface/DashboardFC/Dashboard.aspx?Nav=ConsLugmasCostN2&Locali=''+convert(varchar,[Codigo Localidad])',");
            query.AppendLine("@Where = @Where,");
            query.AppendLine("@Lenght = 10,");
            query.AppendLine("@Group = '[Codigo Localidad]',");
            query.AppendLine("@Order = '[Costo] desc',");
            query.AppendLine("@OrderInv = '[Costo] asc',");
            query.AppendLine("@Start = 0,");
            query.AppendLine("@OrderDir = 'Desc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español',");
            query.AppendLine(" @ConsultaDetallFactura = " + ConsultaDetallFactura + "");
            return query.ToString();

        }

        public string ConsLocalidMasMarcadas(int ConsultaDetallFactura = 0)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @ParamSitio varchar(max)");
            query.AppendLine("declare @ParamCenCos varchar(max)");
            query.AppendLine("declare @ParamTDest varchar(max)");
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @Cantidad int");
            query.AppendLine("declare @DisplayStart int");
            query.AppendLine("set @ParamCenCos = 'null'");
            query.AppendLine("set @ParamSitio = 'null'");
            query.AppendLine("set @ParamTDest = 'null'");
            query.AppendLine("set @DisplayStart = 0");
            query.AppendLine("set @Cantidad = case when '10' <> 'null' then '10' else 10 end");
            query.AppendLine("if @Cantidad - @DisplayStart < 10 and @Cantidad - @DisplayStart > 0");
            query.AppendLine("begin");
            query.AppendLine("set @Cantidad = @Cantidad - @DisplayStart");
            query.AppendLine("end");
            query.AppendLine("else");
            query.AppendLine("begin");
            query.AppendLine("set @Cantidad = 10");
            query.AppendLine("end");
            query.AppendLine("");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''");
            query.AppendLine(" '");
            query.AppendLine("");
            query.AppendLine("if @ParamCenCos <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamSitio <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamTDest <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')");
            query.AppendLine("'");
            query.AppendLine("");
            query.AppendLine("exec ConsumoAcumuladoTodosCamposRest");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields='");
            query.AppendLine("	[Nombre Localidad]=Min(upper([Nombre Localidad])),");
            query.AppendLine("	[Codigo Localidad],");
            query.AppendLine("	[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),");
            query.AppendLine("	[Codigo Tipo Destino],");
            query.AppendLine("	[Numero]=sum([TotalLlamadas]),");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos]),");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio])',");
            query.AppendLine("@Where = @Where,");
            query.AppendLine("@Group = '");
            query.AppendLine("	[Codigo Localidad],");
            query.AppendLine("	[Codigo Tipo Destino]',");
            query.AppendLine("@Order = '[Numero] desc',");
            query.AppendLine("@OrderInv = '[Numero] asc',");
            query.AppendLine("@Start = 0,");
            query.AppendLine("@OrderDir = 'Desc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español',");
            query.AppendLine(" @ConsultaDetallFactura = " + ConsultaDetallFactura + "");

            return query.ToString();

        }

        public string ConsEmpmasLlam()
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @ParamSitio varchar(max)                                                              ");
            query.AppendLine("declare @ParamCenCos varchar(max)                                                             ");
            query.AppendLine("declare @ParamTDest varchar(max)                                                              ");
            query.AppendLine("declare @Where varchar(max)                                                                   ");
            query.AppendLine("                                                                                              ");
            query.AppendLine("set @ParamCenCos = 'null'                                                                     ");
            query.AppendLine("set @ParamSitio = 'null'                                                                      ");
            query.AppendLine("set @ParamTDest = 'null'                                                                      ");
            query.AppendLine("                                                                                              ");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine(" '                                                                                            ");
            query.AppendLine("                                                                                              ");
            query.AppendLine("if @ParamCenCos <> 'null'                                                                     ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')               ");
            query.AppendLine("'                                                                                             ");
            query.AppendLine("if @ParamSitio <> 'null'                                                                      ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')                           ");
            query.AppendLine("'                                                                                             ");
            query.AppendLine("if @ParamTDest <> 'null'                                                                      ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')                    ");
            query.AppendLine("'                                                                                             ");
            query.AppendLine("                                                                                              ");
            query.AppendLine("exec ConsumoAcumuladoTodosCamposRest                                                          ");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='                                                                                     ");
            query.AppendLine("	[Nombre Completo]=Min(upper([Nombre Completo])),                                            ");
            query.AppendLine("	[Codigo Empleado],                                                                          ");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),                             ");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos]),                                                         ");
            query.AppendLine("	[Numero]=sum([TotalLlamadas])',                                                             ");
            query.AppendLine("@Where = @Where,                                                                              ");
            query.AppendLine("@Group = '[Codigo Empleado]',                                                                 ");
            query.AppendLine("@Order = '[Numero] desc',                                                                     ");
            query.AppendLine("@OrderInv = '[Numero] asc',                                                                   ");
            query.AppendLine("@Start = 0,                                                                                   ");
            query.AppendLine("@OrderDir = 'Desc',                                                                           ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");


            return query.ToString();

        }

        public string ConsEmpmasLlamN2(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @Where varchar(max)																					");
            query.AppendLine("declare @ParamCarrier varchar(max)                                                                           ");
            query.AppendLine("declare @ParamEmple varchar(max)                                                                             ");
            query.AppendLine("declare @ParamCC varchar(max)                                                                                ");
            query.AppendLine("declare @ParamSitio varchar(max)                                                                             ");
            query.AppendLine("declare @ParamTipoLlam varchar(max)                                                                          ");
            query.AppendLine("declare @ParamTDest varchar(max)                                                                             ");
            query.AppendLine("declare @ParamLocali varchar(max)                                                                            ");
            query.AppendLine("declare @ParamCatTDest varchar(max)                                                                          ");
            query.AppendLine("                                                                                                             ");
            query.AppendLine("set @ParamCarrier = 'null'                                                                                   ");
            query.AppendLine("set @ParamEmple = '" + param["Emple"] + "'                                                                                   ");
            query.AppendLine("set @ParamCC = 'null'                                                                                        ");
            query.AppendLine("set @ParamSitio = 'null'                                                                                     ");
            query.AppendLine("set @ParamTipoLlam = 'null'                                                                                  ");
            query.AppendLine("set @ParamTDest = 'null'                                                                                     ");
            query.AppendLine("set @ParamLocali = 'null'                                                                                    ");
            query.AppendLine("set @ParamCatTDest = 'null'                                                                                  ");
            query.AppendLine("                                                                                                             ");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("                                                                                                             ");
            query.AppendLine("if @ParamCarrier <> 'null'                                                                                   ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')                                      ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("if @ParamEmple <> 'null'                                                                                     ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')                                       ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("if @ParamCC <> 'null'                                                                                        ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCC+')                                  ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("if @ParamSitio <> 'null'                                                                                     ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')                                          ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("if @ParamTipoLlam <> 'null'                                                                                  ");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamTipoLlam+')                                 ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("if @ParamTDest <> 'null'                                                                                     ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')                                   ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("if @ParamLocali <> 'null'                                                                                    ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')                                     ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("if @ParamCatTDest <> 'null'                                                                                  ");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Categoria Tipo Destino] in('+@ParamCatTDest+')                      ");
            query.AppendLine("'                                                                                                            ");
            query.AppendLine("                                                                                                             ");
            query.AppendLine("                                                                                                             ");
            query.AppendLine("                                                                                                             ");
            query.AppendLine("exec ConsumoAcumuladoPorTDestTodosParamRest                                                                  ");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='                                                                                                    ");
            query.AppendLine("	[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),                                                    ");
            query.AppendLine("	[Codigo Tipo Destino],                                                                                      ");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),                                             ");
            query.AppendLine("	[Numero]=sum([TotalLlamadas]),                                                                              ");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos])                                                                     ");
            if (linkGrafica != "")
            {
                query.AppendLine(", " + linkGrafica + "',");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine("@Where = @Where,                                                                                             ");
            query.AppendLine("@Group = '[Codigo Tipo Destino],  [Nombre Tipo Destino]',                                                                            ");
            query.AppendLine("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc',                             ");
            query.AppendLine("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc',                            ");
            query.AppendLine("@Start = 0,                                                                                                  ");
            query.AppendLine("@OrderDir = 'Desc',                                                                                          ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");


            return query.ToString();

        }

        public string ConsEmpmasLlamN3(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @Where varchar(max)																																				");
            query.AppendLine("declare @ParamEmple varchar(max)																																			");
            query.AppendLine("declare @ParamSitio varchar(max)																																			");
            query.AppendLine("declare @ParamCenCos varchar(max)																																			");
            query.AppendLine("declare @ParamCarrier varchar(max)																																		");
            query.AppendLine("declare @ParamExtension varchar(max)																																		");
            query.AppendLine("declare @ParamCodAut varchar(max)																																			");
            query.AppendLine("declare @ParamLocali varchar(max)																																			");
            query.AppendLine("declare @ParamTelDest varchar(max)																																		");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																																		");
            query.AppendLine("declare @ParamTDest varchar(max)																																			");
            query.AppendLine("																																											");
            query.AppendLine("set @ParamEmple = '" + param["Emple"] + "'																																				");
            query.AppendLine("set @ParamSitio = 'null'																																					");
            query.AppendLine("set @ParamCenCos = 'null'																																					");
            query.AppendLine("set @ParamCarrier = 'null'																																				");
            query.AppendLine("set @ParamExtension = 'null'																																				");
            query.AppendLine("set @ParamCodAut = 'null'																																					");
            query.AppendLine("set @ParamLocali = 'null'																																					");
            query.AppendLine("set @ParamTelDest = 'null'																																				");
            query.AppendLine("set @ParamGEtiqueta = 'null'																																				");
            query.AppendLine("set @ParamTDest = '" + param["TDest"] + "'																																					");
            query.AppendLine("																																											");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamEmple <> 'null'																																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')																									");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamSitio <> 'null'																																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')																										");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamCenCos <> 'null'																																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')																							");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamCarrier <> 'null'																																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')																									");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamExtension <> 'null'																																				");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')																										");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamCodAut <> 'null'																																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')																								");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamLocali <> 'null'																																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')																									");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamTelDest <> 'null'																																				");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')																									");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																																				");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')																								");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("if @ParamTDest <> 'null'																																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')																								");
            query.AppendLine("'																																											");
            query.AppendLine("																																											");
            query.AppendLine("exec ConsumoDetalladoTodosCamposRestSinLinea 																																");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																																									");
            query.AppendLine("	[Extension],																																							");
            query.AppendLine("	[Fecha],																																								");
            query.AppendLine("	[Hora],																																									");
            query.AppendLine("	[Duracion]=[Duracion Minutos],																																			");
            query.AppendLine("	[Total]=(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),																												");
            query.AppendLine("	[Nombre Localidad]=upper([Nombre Localidad]),																															");
            query.AppendLine("	[Numero Marcado],																																						");
            query.AppendLine("	[Tipo Llamada]=upper([Tipo Llamada]),																																	");
            query.AppendLine("	[Clave Tipo Llamada],																																					");
            query.AppendLine("	[Etiqueta]', 																																							");
            query.AppendLine("@Where = @Where,																																							");
            query.AppendLine("@Order = '[Etiqueta] Asc,[Total] Desc,[Duracion] Desc,[Nombre Localidad] Asc,[Hora] Asc,[Fecha] Asc,[Extension] Asc,[Numero Marcado] Asc,[Tipo Llamada] Asc',				");
            query.AppendLine("@OrderInv = '[Etiqueta] Desc,[Total] Asc,[Duracion] Asc,[Nombre Localidad] Desc,[Hora] Desc,[Fecha] Desc,[Extension] Desc,[Numero Marcado] Desc,[Tipo Llamada] Desc',		");
            query.AppendLine("@Start = 0,																																								");
            query.AppendLine("@OrderDir = 'Asc',																																						");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");

            return query.ToString();

        }

        public string ConsEmpsMasCaros(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @ParamSitio varchar(max)																		");
            query.AppendLine("declare @ParamCenCos varchar(max)																		");
            query.AppendLine("declare @ParamTDest varchar(max)																		");
            query.AppendLine("declare @Where varchar(max)																			");
            query.AppendLine("declare @Cantidad int																					");
            query.AppendLine("declare @DisplayStart int																				");
            query.AppendLine("set @ParamCenCos = 'null'																				");
            query.AppendLine("set @ParamSitio = 'null'																				");
            query.AppendLine("set @ParamTDest = 'null'																				");
            query.AppendLine("set @DisplayStart = 0																					");
            query.AppendLine("set @Cantidad = case when '10' <> 'null' then '10' else 10 end										");
            query.AppendLine("if @Cantidad - @DisplayStart < 10 and @Cantidad - @DisplayStart > 0									");
            query.AppendLine("begin																									");
            query.AppendLine("set @Cantidad = @Cantidad - @DisplayStart																");
            query.AppendLine("end 																									");
            query.AppendLine("else 																									");
            query.AppendLine("begin																									");
            query.AppendLine("set @Cantidad = 10 																					");
            query.AppendLine("end																									");
            query.AppendLine("																										");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine(" '																									");
            query.AppendLine("																										");
            query.AppendLine("if @ParamCenCos <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')						");
            query.AppendLine("'																										");
            query.AppendLine("if @ParamSitio <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')									");
            query.AppendLine("'																										");
            query.AppendLine("if @ParamTDest <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')							");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("exec ConsumoAcumuladoTodosCamposRest     																");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																								");
            query.AppendLine("	[Nombre Completo]=Min(upper([Nombre Completo])),													");
            query.AppendLine("	[Codigo Empleado],																					");
            query.AppendLine("	[Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])),									");
            query.AppendLine("	[Codigo Centro de Costos],																			");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),										");
            query.AppendLine("	[Numero]=sum([TotalLlamadas]),																		");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos]) 																");
            if (linkGrafica != "")
            {
                query.AppendLine(", " + linkGrafica + "',");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine("@Where = @Where,																						");
            query.AppendLine("@Group = '[Codigo Empleado],																			");
            query.AppendLine("[Codigo Centro de Costos]', 																			");
            query.AppendLine("@Order = '[Total] desc',																				");
            query.AppendLine("@OrderInv = '[Total] asc',																			");
            query.AppendLine("@Lenght = 20,             																			");
            query.AppendLine("@Start = 0,																							");
            query.AppendLine("@OrderDir = 'Desc',																					");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");

            return query.ToString();

        }

        public string RepTabConsumosPorNumMarcTipoLlamada()
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @Where varchar(max)																															");
            query.AppendLine("declare @ParamEmple varchar(max)																														");
            query.AppendLine("declare @ParamSitio varchar(max)																														");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																													");
            query.AppendLine("declare @ParamTDest varchar(max)																														");
            query.AppendLine("																																						");
            query.AppendLine("set @ParamEmple = 'null'																																");
            query.AppendLine("set @ParamSitio = 'null'																																");
            query.AppendLine("set @ParamGEtiqueta = 'null'																															");
            query.AppendLine("set @ParamTDest = 'null'																																");
            query.AppendLine("																																						");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																																						");
            query.AppendLine("																																						");
            query.AppendLine("if @ParamEmple <> 'null'																																");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')																				");
            query.AppendLine("'																																						");
            query.AppendLine("																																						");
            query.AppendLine("if @ParamSitio <> 'null'																																");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')																					");
            query.AppendLine("'																																						");
            query.AppendLine("																																						");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																															");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')																			");
            query.AppendLine("'																																						");
            query.AppendLine("																																						");
            query.AppendLine("if @ParamTDest <> 'null'																																");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')																			");
            query.AppendLine("'																																						");
            query.AppendLine("																																						");
            query.AppendLine("exec ConsumoDetalladoTodosCamposRest    																												");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																																				");
            query.AppendLine("	[Numero Marcado],																																	");
            query.AppendLine("	[Llamadas] = count(*),																																");
            query.AppendLine("	[Duracion Minutos] = SUM([Duracion Minutos]),																										");
            query.AppendLine("	[Total]=SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]),																					");
            query.AppendLine("	[Tipo Llamada]=Min(upper([Tipo Llamada])),																											");
            query.AppendLine("	[Clave Tipo Llamada],																																");
            query.AppendLine("	[Etiqueta]', 																																		");
            query.AppendLine("@Where = @Where,																																		");
            query.AppendLine("@Group = '																																			");
            query.AppendLine("	[Numero Marcado],																																	");
            query.AppendLine("	[Clave Tipo Llamada],																																");
            query.AppendLine("	[Etiqueta]', 																																		");
            query.AppendLine("@Order = '[Tipo Llamada] Asc,[Total] Desc,[Duracion Minutos] Desc,[Llamadas] Desc,[Numero Marcado] Desc,[Etiqueta] Asc',								");
            query.AppendLine("@OrderInv = '[Tipo Llamada] Desc,[Total] Asc,[Duracion Minutos] Asc,[Llamadas] Asc,[Numero Marcado] Asc,[Etiqueta] Desc',								");
            query.AppendLine("@Start = 0,																																			");
            query.AppendLine("@OrderDir = 'Asc',																																	");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");

            return query.ToString();

        }

        public string RepTabConsumosPorNumMarcTipoLlamadaN2()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)																																								");
            query.AppendLine("declare @ParamEmple varchar(max)																																							");
            query.AppendLine("declare @ParamSitio varchar(max)																																							");
            query.AppendLine("declare @ParamCenCos varchar(max)																																							");
            query.AppendLine("declare @ParamCarrier varchar(max)																																						");
            query.AppendLine("declare @ParamExtension varchar(max)																																						");
            query.AppendLine("declare @ParamCodAut varchar(max)																																							");
            query.AppendLine("declare @ParamLocali varchar(max)																																							");
            query.AppendLine("declare @ParamTelDest varchar(max)																																						");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																																						");
            query.AppendLine("declare @ParamTDest varchar(max)																																							");
            query.AppendLine("																																															");
            query.AppendLine("set @ParamEmple = 'null'																																									");
            query.AppendLine("set @ParamSitio = 'null'																																									");
            query.AppendLine("set @ParamCenCos = 'null'																																									");
            query.AppendLine("set @ParamCarrier = 'null'																																								");
            query.AppendLine("set @ParamExtension = 'null'																																								");
            query.AppendLine("set @ParamCodAut = 'null'																																									");
            query.AppendLine("set @ParamLocali = 'null'																																									");
            query.AppendLine("set @ParamTelDest = '''" + param["NumMarc"] + "'''																																					");
            query.AppendLine("set @ParamGEtiqueta = " + param["TipoLlam"] + "/*2*/																																									");
            query.AppendLine("set @ParamTDest = 'null'																																									");
            query.AppendLine("																																															");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamEmple <> 'null'																																									");
            query.AppendLine("set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')																															");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamSitio <> 'null'																																									");
            query.AppendLine("set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')																																");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamCenCos <> 'null'																																									");
            query.AppendLine("set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')																													");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamCarrier <> 'null'																																								");
            query.AppendLine("set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')																															");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamExtension <> 'null'																																								");
            query.AppendLine("set @Where = @Where + 'And [Extension] in('+@ParamExtension+')																															");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamCodAut <> 'null'																																									");
            query.AppendLine("set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')																														");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamLocali <> 'null'																																									");
            query.AppendLine("set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')																														");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamTelDest <> 'null'																																								");
            query.AppendLine("set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')																															");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																																								");
            query.AppendLine("set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')																													");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("if @ParamTDest <> 'null'																																									");
            query.AppendLine("set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')																														");
            query.AppendLine("'																																															");
            query.AppendLine("																																															");
            query.AppendLine("exec ConsumoDetalladoTodosCamposRestSinLinea  																																			");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																																													");
            query.AppendLine("	[Extension],																																											");
            query.AppendLine("	[Fecha],																																												");
            query.AppendLine("	[Hora],																																													");
            query.AppendLine("	[Duracion]=[Duracion Minutos],																																							");
            query.AppendLine("	[Total]=(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),																																");
            query.AppendLine("	[Nombre Localidad]=upper([Nombre Localidad]),																																			");
            query.AppendLine("	[Numero Marcado],																																										");
            query.AppendLine("	[Tipo Llamada]=upper([Tipo Llamada]),																																					");
            query.AppendLine("	[Clave Tipo Llamada],																																									");
            query.AppendLine("	[Etiqueta]', 																																											");
            query.AppendLine("@Where = @Where,																																											");
            query.AppendLine("@Order = '[Etiqueta] Asc,[Total] Desc,[Duracion] Desc,[Nombre Localidad] Asc,[Hora] Asc,[Fecha] Asc,[Extension] Asc,[Numero Marcado] Asc,[Tipo Llamada] Asc',								");
            query.AppendLine("@OrderInv = '[Etiqueta] Desc,[Total] Asc,[Duracion] Asc,[Nombre Localidad] Desc,[Hora] Desc,[Fecha] Desc,[Extension] Desc,[Numero Marcado] Desc,[Tipo Llamada] Desc',						");
            query.AppendLine("@Start = 0,																																												");
            query.AppendLine("@OrderDir = 'Asc',																																										");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");

            return query.ToString();

        }

        public string ConsEmpsMasCarosN4(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)																							");
            query.AppendLine("declare @ParamCarrier varchar(max)																					");
            query.AppendLine("declare @ParamEmple varchar(max)																						");
            query.AppendLine("declare @ParamCC varchar(max)																							");
            query.AppendLine("declare @ParamSitio varchar(max)																						");
            query.AppendLine("declare @ParamTipoLlam varchar(max)																					");
            query.AppendLine("declare @ParamTDest varchar(max)																						");
            query.AppendLine("declare @ParamLocali varchar(max)																						");
            query.AppendLine("declare @ParamCatTDest varchar(max)																					");
            query.AppendLine("																														");
            query.AppendLine("set @ParamCarrier = 'null'																							");
            query.AppendLine("set @ParamEmple = '" + param["Emple"] + "'																							");
            query.AppendLine("set @ParamCC = '" + param["CenCos"] + "'																								");
            query.AppendLine("set @ParamSitio = '" + param["Sitio"] + "'																							");
            query.AppendLine("set @ParamTipoLlam = '" + param["TipoLlam"] + "'																								");
            query.AppendLine("set @ParamTDest = 'null'																								");
            query.AppendLine("set @ParamLocali = 'null'																								");
            query.AppendLine("set @ParamCatTDest = 'null'																							");
            query.AppendLine("																														");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																														");
            query.AppendLine("																														");
            query.AppendLine("if @ParamCarrier <> 'null'																							");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')												");
            query.AppendLine("'																														");
            query.AppendLine("if @ParamEmple <> 'null'																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')												");
            query.AppendLine("'																														");
            query.AppendLine("if @ParamCC <> 'null'																									");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCC+')											");
            query.AppendLine("'																														");
            query.AppendLine("if @ParamSitio <> 'null'																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')													");
            query.AppendLine("'																														");
            query.AppendLine("if @ParamTipoLlam <> 'null'																							");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamTipoLlam+')											");
            query.AppendLine("'																														");
            query.AppendLine("if @ParamTDest <> 'null'																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')											");
            query.AppendLine("'																														");
            query.AppendLine("if @ParamLocali <> 'null'																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')												");
            query.AppendLine("'																														");
            query.AppendLine("if @ParamCatTDest <> 'null'																							");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Categoria Tipo Destino] in('+@ParamCatTDest+')								");
            query.AppendLine("'																														");
            query.AppendLine("																														");
            query.AppendLine("																														");
            query.AppendLine("																														");
            query.AppendLine("exec ConsumoAcumuladoPorTDestTodosParamRest    ");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),				");
            query.AppendLine("[Codigo Tipo Destino],																								");
            query.AppendLine("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),														");
            query.AppendLine("[Numero]=sum([TotalLlamadas]),																						");
            query.AppendLine("[Duracion]=sum([Duracion Minutos]) 																					");
            if (linkGrafica != "")
            {
                query.AppendLine(", " + linkGrafica + "',");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine("@Where = @Where,																										");
            query.AppendLine("@Group = '[Codigo Tipo Destino]', 																					");
            query.AppendLine("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc',										");
            query.AppendLine("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc',										");
            query.AppendLine("@Start = 0,																											");
            query.AppendLine("@OrderDir = 'Desc',																									");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");

            return query.ToString();

        }

        public string ConsEmpsMasCarosN5(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)																				");
            query.AppendLine("declare @ParamEmple varchar(max)																			");
            query.AppendLine("declare @ParamSitio varchar(max)																			");
            query.AppendLine("declare @ParamCenCos varchar(max)																			");
            query.AppendLine("declare @ParamCarrier varchar(max)																		");
            query.AppendLine("declare @ParamExtension varchar(max)																		");
            query.AppendLine("declare @ParamCodAut varchar(max)																			");
            query.AppendLine("declare @ParamLocali varchar(max)																			");
            query.AppendLine("declare @ParamTelDest varchar(max)																		");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																		");
            query.AppendLine("declare @ParamTDest varchar(max)																			");
            query.AppendLine("																											");
            query.AppendLine("set @ParamEmple = '" + param["Emple"] + "'        ");
            query.AppendLine("set @ParamSitio = '" + param["Sitio"] + "'        ");
            query.AppendLine("set @ParamCenCos = '" + param["CenCos"] + "'       ");
            query.AppendLine("set @ParamCarrier = 'null'        ");
            query.AppendLine("set @ParamExtension = 'null'      ");
            query.AppendLine("set @ParamCodAut = 'null'         ");
            query.AppendLine("set @ParamLocali = 'null'         ");
            query.AppendLine("set @ParamTelDest = 'null'        ");
            query.AppendLine("set @ParamGEtiqueta = '" + param["TipoLlam"] + "'         ");
            query.AppendLine("set @ParamTDest = '" + param["TDest"] + "'         ");
            query.AppendLine("																											");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamEmple <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')									");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamSitio <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')										");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamCenCos <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')							");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamCarrier <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')									");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamExtension <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')										");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamCodAut <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')								");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamLocali <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')									");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamTelDest <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')									");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')								");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamTDest <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')								");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("exec ConsumoAcumuladoPorCarrierTodosParamRest 															");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																									");
            query.AppendLine("	[Nombre Carrier]=Min(upper([Nombre Carrier])),															");
            query.AppendLine("	[Codigo Carrier],																						");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),											");
            query.AppendLine("	[Numero]=sum([TotalLlamadas]),																			");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos]) 																	");
            if (linkGrafica != "")
            {
                query.AppendLine(", " + linkGrafica + "',");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine("@Where = @Where,																							");
            query.AppendLine("@Group = '[Codigo Carrier]', 																				");
            query.AppendLine("@Order = '[Total] Desc,[Numero] Desc,[Duracion] Desc,[Nombre Carrier] Asc',								");
            query.AppendLine("@OrderInv = '[Total] Asc,[Numero] Asc,[Duracion] Asc,[Nombre Carrier] Desc',								");
            query.AppendLine("@OrderDir = 'Desc',																						");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");


            return query.ToString();

        }

        public string ConsEmpsMasCarosN6(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)																					");
            query.AppendLine("declare @ParamEmple varchar(max)																				");
            query.AppendLine("declare @ParamSitio varchar(max)																				");
            query.AppendLine("declare @ParamCenCos varchar(max)																				");
            query.AppendLine("declare @ParamCarrier varchar(max)																			");
            query.AppendLine("declare @ParamExtension varchar(max)																			");
            query.AppendLine("declare @ParamCodAut varchar(max)																				");
            query.AppendLine("declare @ParamLocali varchar(max)																				");
            query.AppendLine("declare @ParamTelDest varchar(max)																			");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																			");
            query.AppendLine("declare @ParamTDest varchar(max)																				");
            query.AppendLine("																												");
            query.AppendLine("set @ParamEmple = '" + param["Emple"] + "'																					");
            query.AppendLine("set @ParamSitio = '" + param["Sitio"] + "'																					");
            query.AppendLine("set @ParamCenCos = '" + param["CenCos"] + "'																					");
            query.AppendLine("set @ParamCarrier = '" + param["Carrier"] + "'																						");
            query.AppendLine("set @ParamExtension = 'null'																					");
            query.AppendLine("set @ParamCodAut = 'null'																						");
            query.AppendLine("set @ParamLocali = 'null'																						");
            query.AppendLine("set @ParamTelDest = 'null'																					");
            query.AppendLine("set @ParamGEtiqueta = '" + param["TipoLlam"] + "'																						");
            query.AppendLine("set @ParamTDest = '" + param["TDest"] + "'																						");
            query.AppendLine("																												");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamEmple <> 'null'																						");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')										");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamSitio <> 'null'																						");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')											");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamCenCos <> 'null'																						");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')								");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamCarrier <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')										");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamExtension <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')											");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamCodAut <> 'null'																						");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')									");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamLocali <> 'null'																						");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')										");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamTelDest <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')										");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')									");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("if @ParamTDest <> 'null'																						");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')									");
            query.AppendLine("'																												");
            query.AppendLine("																												");
            query.AppendLine("exec ConsumoAcumuladoPorNumMarcado    																		");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																										");
            query.AppendLine("	[Numero Marcado],																							");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),												");
            query.AppendLine("	[Numero]=count(*),																							");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos]),																			");
            query.AppendLine("	[Tipo Llamada]=Min(upper([Tipo Llamada])),																	");
            query.AppendLine("	[Clave Tipo Llamada] 																						");
            if (linkGrafica != "")
            {
                query.AppendLine(", " + linkGrafica + "',");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine("@Where = @Where,																								");
            query.AppendLine("@Group = '																									");
            query.AppendLine("	[Numero Marcado],																							");
            query.AppendLine("	[Clave Tipo Llamada]', 																						");
            query.AppendLine("@Order = '[Tipo Llamada] Asc,[Total] Desc,[Duracion] Desc,[Numero] Desc,[Numero Marcado] Asc',				");
            query.AppendLine("@OrderInv = '[Tipo Llamada] Desc,[Total] Asc,[Duracion] Asc,[Numero] Asc,[Numero Marcado] Desc',				");
            query.AppendLine("@OrderDir = 'Asc',																							");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");

            return query.ToString();

        }

        public string ConsEmpsMasCarosN7(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @Where varchar(max)																																							");
            query.AppendLine("declare @ParamEmple varchar(max)																																						");
            query.AppendLine("declare @ParamSitio varchar(max)																																						");
            query.AppendLine("declare @ParamCenCos varchar(max)																																						");
            query.AppendLine("declare @ParamCarrier varchar(max)																																					");
            query.AppendLine("declare @ParamExtension varchar(max)																																					");
            query.AppendLine("declare @ParamCodAut varchar(max)																																						");
            query.AppendLine("declare @ParamLocali varchar(max)																																						");
            query.AppendLine("declare @ParamTelDest varchar(max)																																					");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																																					");
            query.AppendLine("declare @ParamTDest varchar(max)																																						");
            query.AppendLine("																																														");
            query.AppendLine("set @ParamEmple = '" + param["Emple"] + "'																																							");
            query.AppendLine("set @ParamSitio = '" + param["Sitio"] + "'																																							");
            query.AppendLine("set @ParamCenCos = '" + param["CenCos"] + "'																																							");
            query.AppendLine("set @ParamCarrier = '" + param["Carrier"] + "'																																								");
            query.AppendLine("set @ParamExtension = 'null'																																							");
            query.AppendLine("set @ParamCodAut = 'null'																																								");
            query.AppendLine("set @ParamLocali = 'null'																																								");
            query.AppendLine("set @ParamTelDest = '''" + param["NumMarc"] + "'''																																				");
            query.AppendLine("set @ParamGEtiqueta = '" + param["TipoLlam"] + "'																																								");
            query.AppendLine("set @ParamTDest = '" + param["TDest"] + "'																																								");
            query.AppendLine("																																														");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamEmple <> 'null'																																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')																												");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamSitio <> 'null'																																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')																													");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamCenCos <> 'null'																																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')																										");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamCarrier <> 'null'																																							");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')																												");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamExtension <> 'null'																																							");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')																													");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamCodAut <> 'null'																																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')																											");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamLocali <> 'null'																																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')																												");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamTelDest <> 'null'																																							");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')																												");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																																							");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')																											");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("if @ParamTDest <> 'null'																																								");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')																											");
            query.AppendLine("'																																														");
            query.AppendLine("																																														");
            query.AppendLine("exec ConsumoDetalladoTodosCamposRestSinLinea																																			");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																																												");
            query.AppendLine("	[Extension],																																										");
            query.AppendLine("	[Fecha],																																											");
            query.AppendLine("	[Hora],																																												");
            query.AppendLine("	[Duracion]=[Duracion Minutos],																																						");
            query.AppendLine("	[Total]=(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),																															");
            query.AppendLine("	[Nombre Localidad]=upper([Nombre Localidad]),																																		");
            query.AppendLine("	[Numero Marcado],																																									");
            query.AppendLine("	[Tipo Llamada]=upper([Tipo Llamada]),																																				");
            query.AppendLine("	[Clave Tipo Llamada],																																								");
            query.AppendLine("	[Etiqueta]																																										");
            if (linkGrafica != "")
            {
                query.AppendLine(", " + linkGrafica + "',");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine("@Where = @Where,																																										");
            query.AppendLine("@Order = '[Etiqueta] Asc,[Total] Desc,[Duracion] Desc,[Nombre Localidad] Asc,[Hora] Asc,[Fecha] Asc,[Extension] Asc,[Numero Marcado] Asc,[Tipo Llamada] Asc',							");
            query.AppendLine("@OrderInv = '[Etiqueta] Desc,[Total] Asc,[Duracion] Asc,[Nombre Localidad] Desc,[Hora] Desc,[Fecha] Desc,[Extension] Desc,[Numero Marcado] Desc,[Tipo Llamada] Desc',					");
            query.AppendLine("@OrderDir = 'Asc',																																									");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");

            return query.ToString();

        }

        public string ConsLlamsMasCaras()
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @ParamSitio varchar(max)																			");
            query.AppendLine("declare @ParamCenCos varchar(max)																			");
            query.AppendLine("declare @ParamTDest varchar(max)																			");
            query.AppendLine("declare @Where varchar(max)																				");
            query.AppendLine("declare @Cantidad int																						");
            query.AppendLine("declare @DisplayStart int																					");
            query.AppendLine("set @ParamCenCos = 'null'																					");
            query.AppendLine("set @ParamSitio = 'null'																					");
            query.AppendLine("set @ParamTDest = 'null'																					");
            query.AppendLine("																											");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("and [Codigo Tipo Destino] not in (381, 382) --ent, enl														");
            query.AppendLine(" '																											");
            query.AppendLine("																											");
            query.AppendLine("if @ParamCenCos <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')							");
            query.AppendLine("'																											");
            query.AppendLine("if @ParamSitio <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')										");
            query.AppendLine("'																											");
            query.AppendLine("if @ParamTDest <> 'null'																					");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')									");
            query.AppendLine("'																											");
            query.AppendLine("																											");
            query.AppendLine("exec ConsumoDetalladoTodosCamposCDRRest																	");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																									");
            query.AppendLine("	[Extension],																							");
            query.AppendLine("	[Nombre Completo]=upper([Nombre Completo]),																");
            query.AppendLine("	[Codigo Empleado],																						");
            query.AppendLine("	[Numero Marcado],																						");
            query.AppendLine("	[Fecha],																								");
            query.AppendLine("	[Hora],																									");
            query.AppendLine("	[Duracion Minutos],																						");
            query.AppendLine("	[Costo]=(([Costo]/[TipoCambio]))+(([CostoSM]/[TipoCambio])),											");
            query.AppendLine("	[Nombre Sitio]=upper([Nombre Sitio]),																	");
            query.AppendLine("	[Codigo Sitio],																							");
            query.AppendLine("	[Nombre Localidad]=upper([Nombre Localidad]),															");
            query.AppendLine("	[Codigo Localidad]', 																					");
            query.AppendLine("@Where = @Where,																							");
            query.AppendLine("@Lenght = 10,																							");
            query.AppendLine("@Group = '', 																								");
            query.AppendLine("@Order = '[Costo] desc',																					");
            query.AppendLine("@OrderInv = '[Costo] asc',																					");
            query.AppendLine("@OrderDir = 'Desc',																						");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");


            return query.ToString();

        }

        public string ConsNumerosMasMarcadas()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @ParamSitio varchar(max)																	");
            query.AppendLine("declare @ParamCenCos varchar(max)																	");
            query.AppendLine("declare @ParamTDest varchar(max)																	");
            query.AppendLine("declare @Where varchar(max)																		");
            query.AppendLine("declare @Cantidad int																				");
            query.AppendLine("declare @DisplayStart int																			");
            query.AppendLine("set @ParamCenCos = 'null'																			");
            query.AppendLine("set @ParamSitio = 'null'																			");
            query.AppendLine("set @ParamTDest = 'null'																			");
            query.AppendLine("																									");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("and [Codigo Tipo Destino] not in (381, 382) --ent, enl											");
            query.AppendLine("'																									");
            query.AppendLine("																									");
            query.AppendLine("if @ParamCenCos <> 'null'																			");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')					");
            query.AppendLine("'																									");
            query.AppendLine("if @ParamSitio <> 'null'																			");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')								");
            query.AppendLine("'																									");
            query.AppendLine("if @ParamTDest <> 'null'																			");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')						");
            query.AppendLine("'																									");
            query.AppendLine("	   																								");
            query.AppendLine("exec TopNumerosMasMarcados 															");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																							");
            query.AppendLine("	[Numero Marcado],																				");
            query.AppendLine("	[Llamadas]=count(*),																			");
            query.AppendLine("	[Nombre Localidad]=MIN(upper([Nombre Localidad])),												");
            query.AppendLine("	[Codigo Localidad],																				");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),									");
            query.AppendLine("	[Cant Emp] = count(distinct [Codigo Empleado]),													");
            query.AppendLine("	[Duracion]=sum([Duracion Minutos])', 															");
            query.AppendLine("@Where = @Where,																					");
            query.AppendLine("@Lenght = 10,																					");
            query.AppendLine("@Group = '[Numero Marcado],																		");
            query.AppendLine("[Codigo Localidad]', 																				");
            query.AppendLine("@Order = '[Llamadas] desc',																		");
            query.AppendLine("@OrderInv = '[Llamadas] asc',																		");
            query.AppendLine("@OrderDir = 'Desc',																				");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");



            return query.ToString();

        }

        public string ConsNumerosMasMarcadasN2(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamEmple varchar(max)");
            query.AppendLine("declare @ParamSitio varchar(max)");
            query.AppendLine("declare @ParamCenCos varchar(max)");
            query.AppendLine("declare @ParamCarrier varchar(max)");
            query.AppendLine("declare @ParamExtension varchar(max)");
            query.AppendLine("declare @ParamCodAut varchar(max)");
            query.AppendLine("declare @ParamLocali varchar(max)");
            query.AppendLine("declare @ParamTelDest varchar(max)");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)");
            query.AppendLine("declare @ParamTDest varchar(max)");            
            query.AppendLine("set @ParamEmple = 'null'");
            query.AppendLine("set @ParamSitio = 'null'");
            query.AppendLine("set @ParamCenCos = 'null'");
            query.AppendLine("set @ParamCarrier = 'null'");
            query.AppendLine("set @ParamExtension = 'null'");
            query.AppendLine("set @ParamCodAut = 'null'");
            query.AppendLine("set @ParamLocali = '" + param["Locali"] + "'");
            query.AppendLine("set @ParamTelDest = '''" + param["NumMarc"] + "'''");
            query.AppendLine("set @ParamGEtiqueta = 'null'");
            query.AppendLine("set @ParamTDest = 'null'");            
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'");            
            query.AppendLine("if @ParamEmple <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamSitio <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamCenCos <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamCarrier <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamExtension <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamCodAut <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamLocali <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamTelDest <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamGEtiqueta <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')");
            query.AppendLine("'");            
            query.AppendLine("if @ParamTDest <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')");
            query.AppendLine("'");            
            query.AppendLine("exec ConsumoDetalladoTodosCamposCDRRest");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields='");
            query.AppendLine("[Nombre Completo]=Min(upper([Nombre Completo])),");
            query.AppendLine("[Codigo Empleado],");
            if(DSODataContext.Schema.ToUpper() != "FCA")
            {
                query.AppendLine("[Nombre Centro de Costos]=Min(upper([Nombre Centro de Costos])),");
                query.AppendLine("[Codigo Centro de Costos],");

            }
            query.AppendLine("[Nombre Sitio]=Min(upper([Nombre Sitio])),");
            query.AppendLine("[Codigo Sitio],");
            query.AppendLine("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("[Numero]=count(*),");
            query.AppendLine("[Duracion]=sum([Duracion Minutos])	,");
            query.AppendLine("[Localidad] = Max([Nombre Localidad])+''(''+Max([Clave Localidad])+'')'',");
            query.AppendLine("[Numero Marcado] = ''''''''+Max([Numero Marcado])");
            if (linkGrafica != "")
            {
                query.AppendLine(", " + linkGrafica + "',");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine("@Where = @Where,");
            query.AppendLine("@Group = '[Codigo Empleado],");
            if(DSODataContext.Schema.ToUpper()!= "FCA")
            {
                query.AppendLine("[Codigo Centro de Costos],");
            }

            query.AppendLine("[Codigo Sitio]',");
            //query.AppendLine("@Order = '[Nombre Completo] Asc,[Nombre Centro de Costos] Asc,[Nombre Sitio] Asc,[Total] Asc,[Duracion] Asc,[Numero] Asc',");
            //query.AppendLine("@OrderInv = '[Nombre Completo] Desc,[Nombre Centro de Costos] Desc,[Nombre Sitio] Desc,[Total] Desc,[Duracion] Desc,[Numero] Desc',");
            //query.AppendLine("@OrderDir = 'Asc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'");


            return query.ToString();

        }

        public string ConsNumerosMasMarcadasN2Graf()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)																																										");
            query.AppendLine("declare @ParamEmple varchar(max)																																									");
            query.AppendLine("declare @ParamSitio varchar(max)																																									");
            query.AppendLine("declare @ParamCenCos varchar(max)																																									");
            query.AppendLine("declare @ParamCarrier varchar(max)																																								");
            query.AppendLine("declare @ParamExtension varchar(max)																																								");
            query.AppendLine("declare @ParamCodAut varchar(max)																																									");
            query.AppendLine("declare @ParamLocali varchar(max)																																									");
            query.AppendLine("declare @ParamTelDest varchar(max)																																								");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																																								");
            query.AppendLine("declare @ParamTDest varchar(max)																																									");
            query.AppendLine("																																																	");
            query.AppendLine("set @ParamEmple = 'null'																																											");
            query.AppendLine("set @ParamSitio = 'null'																																											");
            query.AppendLine("set @ParamCenCos = 'null'																																											");
            query.AppendLine("set @ParamCarrier = 'null'																																										");
            query.AppendLine("set @ParamExtension = 'null'																																										");
            query.AppendLine("set @ParamCodAut = 'null'																																											");
            query.AppendLine("set @ParamLocali = '" + param["Locali"] + "'																																											");
            query.AppendLine("set @ParamTelDest = '''" + param["NumMarc"] + "'''																																							");
            query.AppendLine("set @ParamGEtiqueta = 'null'																																										");
            query.AppendLine("set @ParamTDest = 'null'																																											");
            query.AppendLine("																																																	");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamEmple <> 'null'																																											");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')																															");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamSitio <> 'null'																																											");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')																																");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamCenCos <> 'null'																																											");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')																													");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamCarrier <> 'null'																																										");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')																															");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamExtension <> 'null'																																										");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')																																");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamCodAut <> 'null'																																											");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')																														");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamLocali <> 'null'																																											");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')																															");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamTelDest <> 'null'																																										");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')																															");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																																										");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')																														");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("if @ParamTDest <> 'null'																																											");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')																														");
            query.AppendLine("'																																																	");
            query.AppendLine("																																																	");
            query.AppendLine("exec ConsumoDetalladoTodosCamposCDRRest   																																						");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																																															");
            query.AppendLine("	[Nombre Sitio]=Min([Nombre Sitio]),																																								");
            query.AppendLine("	[Codigo Sitio],																																													");
            query.AppendLine("	Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', 																											");
            query.AppendLine("@Lenght =10,																																														");
            query.AppendLine("@Orderdir='desc',																																													");
            query.AppendLine("@Order='[Total] desc',																																											");
            query.AppendLine("@OrderInv='[Total] desc',																																											");
            query.AppendLine("@Start = 0,																																														");
            query.AppendLine("@Group = '[Codigo Sitio]',																																										");
            query.AppendLine("@Where = @Where,																																													");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");



            return query.ToString();

        }

        public string ConsNumerosMasMarcadasN3()
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamEmple varchar(max)");
            query.AppendLine("declare @ParamSitio varchar(max)");
            query.AppendLine("declare @ParamCenCos varchar(max)");
            query.AppendLine("declare @ParamCarrier varchar(max)");
            query.AppendLine("declare @ParamExtension varchar(max)");
            query.AppendLine("declare @ParamCodAut varchar(max)");
            query.AppendLine("declare @ParamLocali varchar(max)");
            query.AppendLine("declare @ParamTelDest varchar(max)");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)");
            query.AppendLine("declare @ParamTDest varchar(max)");
            query.AppendLine("set @ParamEmple = '" + param["Emple"] + "'");
            query.AppendLine("set @ParamSitio = '" + param["Sitio"] + "'");
            if (DSODataContext.Schema.ToUpper()!="FCA")
            {
                query.AppendLine("set @ParamCenCos = '" + param["CenCos"] + "'");
            }            
            query.AppendLine("set @ParamCarrier = 'null'");
            query.AppendLine("set @ParamExtension = 'null'");
            query.AppendLine("set @ParamCodAut = 'null'");
            query.AppendLine("set @ParamLocali = '" + param["Locali"] + "'");
            query.AppendLine("set @ParamTelDest = '''" + param["NumMarc"] + "'''");
            query.AppendLine("set @ParamGEtiqueta = 'null'");
            query.AppendLine("set @ParamTDest = 'null'");            
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'");
            query.AppendLine("if @ParamEmple <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamSitio <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamCenCos <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamCarrier <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')");
            query.AppendLine("'");
            query.AppendLine("");
            query.AppendLine("if @ParamExtension <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamCodAut <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamLocali <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamTelDest <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamGEtiqueta <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamTDest <> 'null'");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')");
            query.AppendLine("'");
            query.AppendLine("exec ConsumoDetalladoTodosCamposRestSinLinea");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields='");
            query.AppendLine("	[Extension],");
            query.AppendLine("	[Fecha],");
            query.AppendLine("	[Hora],");
            query.AppendLine("	[Duracion]=[Duracion Minutos],");
            query.AppendLine("	[Total]=(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),");
            query.AppendLine("	[Nombre Localidad]=upper([Nombre Localidad]),");
            query.AppendLine("	[Numero Marcado],");
            query.AppendLine("	[Tipo Llamada]=upper([Tipo Llamada]),");
            query.AppendLine("	[Clave Tipo Llamada],");
            query.AppendLine("	[Etiqueta],");
            query.AppendLine("[Nombre Empleado] = [Nombre Completo]+''(''+[Clave Empleado]+'')'',  ");
            if (DSODataContext.Schema.ToUpper() != "FCA")
            {
                query.AppendLine("[Centro de Costos] = [Nombre Centro de Costos]+''(''+[Numero Centro de Costos]+'')'',");
            }            
            query.AppendLine("[Nombre del Sitio] = [Nombre Sitio]+''(''+[Clave Localidad]+ '')'',");
            query.AppendLine("[Localidad] = [Nombre Localidad]+''(''+[Clave Localidad]+'')'',");
            query.AppendLine("[Telefono Destino] = [Numero Marcado]',");
            query.AppendLine("@Where = @Where,");
            //query.AppendLine("@Order = '[Etiqueta] Asc,[Total] Desc,[Duracion] Desc,[Nombre Localidad] Asc,[Hora] Asc,[Fecha] Asc,[Extension] Asc,[Numero Marcado] Asc,[Tipo Llamada] Asc',					");
            //query.AppendLine("@OrderInv = '[Etiqueta] Desc,[Total] Asc,[Duracion] Asc,[Nombre Localidad] Desc,[Hora] Desc,[Fecha] Desc,[Extension] Desc,[Numero Marcado] Desc,[Tipo Llamada] Desc',			");
            //query.AppendLine("@OrderDir = 'Asc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'");


            return query.ToString();

        }

        public string ConsLocalidMasMarcadasN2()
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("declare @Where varchar(max)																			");
            query.AppendLine("declare @ParamEmple varchar(max)																		");
            query.AppendLine("declare @ParamSitio varchar(max)																		");
            query.AppendLine("declare @ParamCenCos varchar(max)																		");
            query.AppendLine("declare @ParamCarrier varchar(max)																	");
            query.AppendLine("declare @ParamExtension varchar(max)																	");
            query.AppendLine("declare @ParamCodAut varchar(max)																		");
            query.AppendLine("declare @ParamLocali varchar(max)																		");
            query.AppendLine("declare @ParamTelDest varchar(max)																	");
            query.AppendLine("declare @ParamGEtiqueta varchar(max)																	");
            query.AppendLine("declare @ParamTDest varchar(max)																		");
            query.AppendLine("																										");
            query.AppendLine("set @ParamEmple = 'null'																				");
            query.AppendLine("set @ParamSitio = 'null'																				");
            query.AppendLine("set @ParamCenCos = 'null'																				");
            query.AppendLine("set @ParamCarrier = 'null'																			");
            query.AppendLine("set @ParamExtension = 'null'																			");
            query.AppendLine("set @ParamCodAut = 'null'																				");
            query.AppendLine("set @ParamLocali = '" + param["Locali"] + "'																				");
            query.AppendLine("set @ParamTelDest = 'null'																			");
            query.AppendLine("set @ParamGEtiqueta = 'null'																			");
            query.AppendLine("set @ParamTDest = '" + param["TDest"] + "'																				");
            query.AppendLine("																										");
            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamEmple <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')								");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamSitio <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+')									");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamCenCos <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')						");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamCarrier <> 'null'																			");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+')								");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamExtension <> 'null'																			");
            query.AppendLine("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+')									");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamCodAut <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+')							");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamLocali <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+')								");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamTelDest <> 'null'																			");
            query.AppendLine("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+')								");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamGEtiqueta <> 'null'																			");
            query.AppendLine("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+')							");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("if @ParamTDest <> 'null'																				");
            query.AppendLine("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+')							");
            query.AppendLine("'																										");
            query.AppendLine("																										");
            query.AppendLine("exec ConsumoAcumuladoPorEmpleadoTodosParamRest  														");
            query.AppendLine("		@Schema='" + DSODataContext.Schema + "',												");
            query.AppendLine("@Fields='																								");
            query.AppendLine("	[Nombre Completo]=Min(upper([Nombre Completo])),													");
            query.AppendLine("	[Codigo Empleado],																					");
            query.AppendLine("	TotImporte = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]),								");
            query.AppendLine("	LLamadas = sum([TotalLlamadas]),																	");
            query.AppendLine("	TotMin = SUM([Duracion Minutos])', 																	");
            query.AppendLine("@Where = @Where,																						");
            query.AppendLine("@Group = '[Codigo Empleado]', 																		");
            query.AppendLine("@Order = '[TotImporte] Desc,[LLamadas] Desc,[TotMin] Desc,[Nombre Completo] Asc',						");
            query.AppendLine("@OrderInv = '[TotImporte] Asc,[LLamadas] Asc,[TotMin] Asc,[Nombre Completo] Desc',					");
            query.AppendLine("@Start = 0,																							");
            query.AppendLine("@OrderDir = 'Desc',																					");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",													");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",														");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',					");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',						");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español'																			");


            return query.ToString();

        }

        #endregion Consultas JH y RM


        public string RepObtieneLlamEntradasEnlace(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" EXEC ObtieneLlamEntradasEnlace @Schema='" + DSODataContext.Schema + "',");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            lsb.AppendLine("@Idioma = 'Español',");
            lsb.AppendLine("@Link = '" + linkGrafica + "' ");

            return lsb.ToString();
        }
        public string RepTabLlamadasEntreSitiosN2(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" EXEC ObtieneLlamEntradasEnlace @Schema='" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            lsb.AppendLine("@Idioma = 'Español',");
            lsb.AppendLine("@Sitio = " + param["Sitio"] + ",");
            lsb.AppendLine("@Link = '" + linkGrafica + "' ");

            return lsb.ToString();
        }
        public string RepTabLlamadasEntreSitiosN3(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" EXEC ObtieneExtenSitiosLlam @Schema='" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ", ");
            lsb.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            lsb.AppendLine("@Idioma = 'Español',");
            lsb.AppendLine("@SitioOrigen = " + param["SitioDest"] + ",");
            lsb.AppendLine("@SitioDestino = " + param["Sitio"] + ",");
            lsb.AppendLine("@Link = '" + linkGrafica + "' ");

            return lsb.ToString();
        }
        public string RepTabLlamadasEntreSitiosN4()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" EXEC ObtieneDetallExtenLlamEnlaceSitio @Schema='" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ", ");
            lsb.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            lsb.AppendLine("@Idioma = 'Español',");
            lsb.AppendLine("@Extension = " + param["Extension"] + ",");
            lsb.AppendLine("@Sitio = " + param["Sitio"] + "");

            return lsb.ToString();
        }

        public string ConsLugmasCostN2(string linkGrafica)
        {
            string lsWhere = string.Empty;

            lsWhere = "'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ";

            if (!string.IsNullOrEmpty(param["Locali"]))
            {
                lsWhere += " and [Codigo Localidad] = " + param["Locali"];
            }

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ConsumoPorSitio    @Schema='" + DSODataContext.Schema + "',\n ");
            lsb.Append("			                           @Fields='[Nombre Sitio]=Min(upper([Nombre Sitio])),\n");
            lsb.Append("[Codigo Sitio],\n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),\n");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Numero]=sum([TotalLlamadas]),\n");
            lsb.Append("[Duracion]=sum([Duracion Minutos])', \n");
            lsb.Append("@Where = " + lsWhere + "', \n");
            lsb.Append("@Group = '[Codigo Sitio]', \n");
            lsb.Append("@Order = '[Total] Desc,[Numero] Desc,[Duracion] Desc,[Nombre Sitio] Asc',\n");
            lsb.Append("@OrderInv = '[Total] Asc,[Numero] Asc,[Duracion] Asc,[Nombre Sitio] Desc',\n");
            lsb.Append("@OrderDir = 'Desc',\n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\n");
            lsb.Append(", @isFT = 0" + isFT + " \n");


            return lsb.ToString();
        }

        public string ConsultaConsLugmasCostN3()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorCenCos] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Localidad] = " + param["Locali"] + " and [Codigo Sitio] = " + param["Sitio"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string ConsLugmasCostN4()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatConsumoPorEmple] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            if(DSODataContext.Schema.ToUpper()== "FCA")
            {
                query.AppendLine("	@where = ' [Codigo Localidad] = " + param["Locali"] + " AND [Codigo Sitio] = " + param["Sitio"] + "',");
            }
            else
            {
                query.AppendLine("	@where = ' [Codigo Localidad] = " + param["Locali"] + " AND [Codigo Sitio] = " + param["Sitio"] + "  AND [Codigo Centro de Costos] = " + param["CenCos"] + " ',");
            }
           
            
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string ConsultaConsumoPorSucursalBimbo(string linkGraf)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = '' \n ");
            lsb.Append("select @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''   \n ");
            lsb.Append("                                           and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''' \n ");
            lsb.Append(" \n ");
            lsb.Append("exec RepConsumoPorLineasDirectasN1TDest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append(" 			                           @Fields='[Codigo Sitio], \n ");
            lsb.Append("												[Nombre Sitio]=Min(upper([Nombre Sitio])), \n ");
            lsb.Append("												 \n ");
            lsb.Append("												[Importe Telmex]= sum([ImporteTelmex]/[TipoCambio]), \n ");
            lsb.Append("												[Total Llams Telmex]=SUM([TotalLlamadasTelmex]), \n ");
            lsb.Append("\n ");
            lsb.Append("												[Importe Axtel]= sum([ImporteAxtel]/[TipoCambio]), \n ");
            lsb.Append("												[Total Llams Axtel]=SUM([TotalLlamadasAxtel]), \n ");
            lsb.Append("\n ");
            lsb.Append("												[Total Gral]= sum([Costo]/[TipoCambio])\n ");
            if (linkGraf != "")
            {
                lsb.Append("," + linkGraf);
            }
            lsb.Append("',  \n ");

            lsb.Append("			                           @Where = @Where,  \n ");
            lsb.Append(" 			                           @Group = '[Codigo Sitio]',  \n ");
            lsb.Append("			                           @Order = '[Total Gral] Desc,[Nombre Sitio] Asc', \n ");
            lsb.Append("			                           @OrderInv = '[Total] Asc,[Nombre Sitio] Desc', \n ");
            lsb.Append("			                           @OrderDir = 'Desc', \n ");
            lsb.Append("                                       @Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("                                       @Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.AppendLine("	                                @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine("	                                @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.Append("									   @Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("									   @Idioma = 'Español' \n ");



            return lsb.ToString();

        }

        public string ConsultaConsumoPorSucursalBimboN2(string linkGraf)
        {

            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepConsumoPorLineasDirectasN2  \r ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Telefono], \r ");

            lsb.Append("[Total Llams Telmex], \r ");
            lsb.Append("[Importe Telmex],\r ");
            lsb.Append("[Total Llams Axtel],\r ");
            lsb.Append("[Importe Axtel],\r ");

            lsb.Append("[Total Gral]', \r ");
            lsb.Append("                                        @Sitio = " + param["Sitio"] + ",\r ");
            lsb.Append("                                        @OrderDir = 'Asc',\r ");
            lsb.Append("                                        @Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("                                        @Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.AppendLine("	                                @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine("	                                @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r ");
            lsb.Append("@Idioma = 'Español'\r ");

            return lsb.ToString();
        }

        private string ConsultaRepIndConcentracionGasto2Nvl()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [IndicadorConcentracionDelGasto2Nvl]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            query.AppendLine("  @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59', ");
            query.AppendLine("  @Usuario = " + Session["iCodUsuario"].ToString() + ",");
            query.AppendLine("  @Perfil = " + Session["iCodPerfil"].ToString());

            if (param["Indicador"] != null && param["Indicador"].Length > 0)
            {
                query.AppendLine(",@esIndicador = 1");
            }

            return query.ToString();
        }

        private string ConsultaRepIndCodAutoNuevos2Nvl()
        {
            var tabla = BuildIndicadores.GetIndicares("Dashboard.aspx");

            string numMesValor = string.Empty;
            //Para poder obtner el numero de meses que se debe mandar como parametro.
            if (tabla != null && tabla.Rows.Count >= 0)
            {
                var row = tabla.AsEnumerable().FirstOrDefault(x => x.Field<string>("IndicadorClave") == "IndCantCodigosNuevos");
                if (row != null)
                {
                    var array = row["Consulta"].ToString().Split(',');
                    for (int i = 0; i < array.Count(); i++)
                    {
                        if (array[i].ToLower().Contains("@nmeses"))
                        {
                            numMesValor = Convert.ToInt32(array[i].ToLower().Replace("@nmeses", "").Replace("=", "").Replace("'", "").Replace(" ", "")).ToString();
                        }
                    }
                }
            }

            numMesValor = numMesValor != string.Empty ? numMesValor : "1";

            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [IndicadorCodAutoNuevo2Nvl]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            query.AppendLine("  @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59', ");
            query.AppendLine("  @Usuario = " + Session["iCodUsuario"].ToString() + ",");
            query.AppendLine("  @Perfil = " + Session["iCodPerfil"].ToString() + ",");
            query.AppendLine("  @nMeses = " + numMesValor);

            if (param["Indicador"] != null && param["Indicador"].Length > 0)
            {
                query.AppendLine(",@esIndicador = 1");
            }

            return query.ToString();
        }

        private string ConsultaRepIndExtenNuevas2Nvl()
        {
            var tabla = BuildIndicadores.GetIndicares("Dashboard.aspx");

            string numMesValor = string.Empty;
            //Para poder obtner el numero de meses que se debe mandar como parametro.
            if (tabla != null && tabla.Rows.Count >= 0)
            {
                var row = tabla.AsEnumerable().FirstOrDefault(x => x.Field<string>("IndicadorClave") == "IndCantidadExtenNuevas");
                if (row != null)
                {
                    var array = row["Consulta"].ToString().Split(',');
                    for (int i = 0; i < array.Count(); i++)
                    {
                        if (array[i].ToLower().Contains("@nmeses"))
                        {
                            numMesValor = Convert.ToInt32(array[i].ToLower().Replace("@nmeses", "").Replace("=", "").Replace("'", "").Replace(" ", "")).ToString();
                        }
                    }
                }
            }

            numMesValor = numMesValor != string.Empty ? numMesValor : "1";

            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [IndicadorExtenNueva2Nvl]");
            query.AppendLine("  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            query.AppendLine("  @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59', ");
            query.AppendLine("  @Usuario = " + Session["iCodUsuario"].ToString() + ",");
            query.AppendLine("  @Perfil = " + Session["iCodPerfil"].ToString() + ",");
            query.AppendLine("  @nMeses = " + numMesValor);

            if (param["Indicador"] != null && param["Indicador"].Length > 0)
            {
                query.AppendLine(",  @esindicador = 1");
            }

            return query.ToString();
        }

        private string ConsultaRepIndLlamMayorDuracion2Nvl(int opc = 0)
        {
            var tabla = BuildIndicadores.GetIndicares("Dashboard.aspx");
            if (tabla != null && tabla.Rows.Count >= 0)
            {
                if (opc == 0)
                {
                    tabla.AsEnumerable().Where(x => x.Field<string>("IndicadorClave") != "IndMinutosLlamConMasDuracion").ToList()
                    .ForEach(x => tabla.Rows.Remove(x));
                }
                else
                {
                    tabla.AsEnumerable().Where(x => x.Field<string>("IndicadorClave") != "IndMinutosLlamConMasDuracionLDM").ToList()
                    .ForEach(x => tabla.Rows.Remove(x));
                }

                //if (DSODataContext.Schema.ToUpper() != "SCHINDLER" && DSODataContext.Schema.ToUpper() != "K5SCHINDLER" && DSODataContext.Schema.ToUpper() != "KIONETWORKSDRP" && DSODataContext.Schema.ToUpper() != "KIOC12" && DSODataContext.Schema.ToUpper() != "K5REDIT")
                //{
                //    tabla.AsEnumerable().Where(x => x.Field<string>("IndicadorClave") != "IndMinutosLlamConMasDuracion").ToList()
                //    .ForEach(x => tabla.Rows.Remove(x));
                //}
                //else if (DSODataContext.Schema.ToUpper() == "SCHINDLER" || DSODataContext.Schema.ToUpper() == "K5SCHINDLER" || DSODataContext.Schema.ToUpper() == "KIONETWORKSDRP" || DSODataContext.Schema.ToUpper() == "KIOC12" || DSODataContext.Schema.ToUpper() == "K5REDIT")
                //{
                //    tabla.AsEnumerable().Where(x => x.Field<string>("IndicadorClave") != "IndMinutosLlamConMasDuracionLDM").ToList()
                //    .ForEach(x => tabla.Rows.Remove(x));
                //}


                if (tabla.Rows.Count == 1)
                {
                    BuildIndicadores.EjecutarQueyIndicadores(ref tabla);
                    return tabla.Rows[0]["Valor"].ToString();
                }
            }

            return string.Empty;
        }

        public String ConsultaBuscaLineasAVencer(int mesesParaVencimiento)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("Exec BuscaLineasAVencer           ");
            query.AppendLine("    @esquema = '" + DSODataContext.Schema + "',   ");
            query.AppendLine("    @mesesParaVencimiento = " + mesesParaVencimiento + "     ");

            return query.ToString();
        }


        public string ConsultaConRepTabConsHistAnioActualVsAnterior2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("declare @fechaFin varchar(30)");
            query.AppendLine("declare @fechaInicio varchar(30)");
            query.AppendLine("");
            query.AppendLine("set @fechaInicio = '''' + CONVERT(varchar,YEAR(GETDATE())-1) + '-01-01 12:00:00' + ''''");
            query.AppendLine("set @fechaFin = '''' + convert(varchar,GETDATE(),120) + ''''");
            query.AppendLine("");
            query.AppendLine("exec ConsumoHistoricoActualVsAnterior ");
            query.AppendLine("	@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("	@Fields='");
            query.AppendLine("		[Nombre Mes],");
            query.AppendLine("		[FechaKeytia],");
            query.AppendLine("		[Total Anterior]=SUM([Total Anio Anterior]/[TipoCambio]),");
            query.AppendLine("		[Total Actual]=SUM([Total Anio Actual]/[TipoCambio])', ");
            query.AppendLine("	@Group = '");
            query.AppendLine("		[Nombre Mes],");
            query.AppendLine("		[FechaKeytia]', ");
            query.AppendLine("	@Order = '[FechaKeytia] Asc',");
            query.AppendLine("	@OrderDir = 'Asc',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = @fechaInicio,");
            query.AppendLine("	@FechaFinRep = @fechaFin,");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Idioma = 'Español'");
            return query.ToString();
        }


        public string ConsultaReporteMatricialDiasProcesados()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("Exec ReporteMatricialDiasProcesados      ");
            query.AppendLine("    @Schema  ='" + DSODataContext.Schema + "',                   ");
            query.AppendLine("    @FechaIniRep  ='" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("    @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59',");
            query.AppendLine("    @usuario = " + Session["iCodUsuario"] + ",   ");
            query.AppendLine("    @perfil =" + Session["iCodPerfil"] + " ,      ");
            query.AppendLine("    @idioma = 'Español' ");

            return query.ToString();
        }



        public string ConsultaReporteLlamadasEntradasFiltroExten()
        {
            /*este reporte busca la cantidad de minutos y la cantidad de numeros distintos llamados para extensiones fijas en codigo del SP*/

            StringBuilder query = new StringBuilder();

            query.AppendLine("Exec [BuscaLlamsEntradaPorExtension]      ");
            query.AppendLine("  @schema		 = '" + DSODataContext.Schema + "',                   ");
            query.AppendLine("  @fechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("  @fechaFinRep =  '" + Session["FechaFin"].ToString() + " 23:59:59'");


            return query.ToString();

        }
        public string RepIndicadorCodigosNoAsignados()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");

            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' ");
            query.AppendLine("'");

            query.AppendLine("exec CodigosPorIdentificar @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("      @Fields='[Codigo Autorizacion],[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("              [Numero]=count(*),[Duracion]=sum([Duracion Minutos]),[Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine("		@Where = @Where,");
            query.AppendLine("		@Group = '[Codigo Autorizacion],[Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine("		@Start = 0,");
            query.AppendLine("		@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("		@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("		@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("		@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("      @Idioma = 'Español',");
            query.AppendLine(" @isGet = 1");

            return query.ToString();
        }
        public string RepIndicadorExtensionesNoAsignadas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");

            query.AppendLine("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''' ");

            query.AppendLine("exec ExtensionesPorIdentificar @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("      @Fields='[Extension],[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("              [Numero]=count(*),[Duracion]=sum([Duracion Minutos]),[Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine("		@Where = @Where,");
            query.AppendLine("		@Group = '[Extension],[Nombre Sitio],[Codigo Sitio]',");
            query.AppendLine("		@Start = 0,");
            query.AppendLine("		@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("		@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("		@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("		@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("      @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("      @Idioma = 'Español',");
            query.AppendLine(" @isGet = 1");
            return query.ToString();
        }
        public string ConsultaExcedenteLineasTelcel(string link)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @Where VARCHAR(MAX) = ''");
            query.AppendLine(" SELECT @Where = '1 = 1'");
            query.AppendLine(" EXEC[RepTabConsumoMovilPorLineaExcedente]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Fields = ' ");
            query.AppendLine(" [Codigo Linea],");
            query.AppendLine(" [Linea],");
            query.AppendLine(" [Codigo Carrier],");
            query.AppendLine(" [Carrier],");
            query.AppendLine(" [Nombre Completo] = MIN(UPPER([Nombre Completo])),");
            query.AppendLine(" [Nombre Centro de Costos] = MIN(UPPER([Nombre Centro de Costos])),");
            query.AppendLine(" [Renta] = ROUND(SUM([Renta]/[TipoCambio]),2),");
            query.AppendLine(" [Excedente] = ROUND((SUM([Costo]/[TipoCambio])) - SUM([Renta]/[TipoCambio]),2),");
            query.AppendLine(" [Porcentaje] = ( (ROUND((SUM([Costo]/[TipoCambio])) - SUM([Renta]/[TipoCambio]),2)) / (CASE WHEN ROUND(SUM([Renta]/[TipoCambio]),2) = 0 THEN 1 ELSE ROUND(SUM([Renta]/[TipoCambio]),2) END) * 100),");
            query.AppendLine(" [Total] = ROUND(SUM([Costo]/[TipoCambio]),2),");
            query.AppendLine(" [Plan] = MIN(UPPER([Plan])),");
            query.AppendLine(" [Fecha Fin Plan] = MIN(UPPER([Fecha Fin Plan]))");
            if (link != "")
            {
                query.AppendLine(" ," + link + "''' ,");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine(" @Where = @Where,   ");
            query.AppendLine(" @Group = '[Codigo Linea],[Linea],[Codigo Carrier],[Carrier]',");
            query.AppendLine(" @Order = '[Total] Desc', ");
            query.AppendLine(" @OrderInv = '[Total] Asc', ");
            query.AppendLine(" @OrderDir = 'Asc',");
            query.AppendLine(" @Carrier = 373,");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ", ");
            query.AppendLine(" @Moneda = 'MXP', ");
            query.AppendLine(" @Idioma = 'Español', ");
            query.AppendLine(" @FechaIniRep  = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine(" @FechaFinRep  = '" + Session["FechaFin"].ToString() + " 23:59:59'");
            return query.ToString();
        }
        public string ConsultaRepPorCategoria(string link)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @Where VARCHAR(MAX) = ''");
            query.AppendLine(" SELECT @Where = '1 = 1'");
            query.AppendLine(" SELECT @Where = @Where + ' AND [Codigo Linea] = " + param["Linea"] + " '");
            query.AppendLine(" EXEC[ConsumoMovilPorCategoria]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Fields = ' ");
            query.AppendLine(" [id] = MAX([id]),");
            query.AppendLine(" [Concepto],");
            query.AppendLine(" [Detalle],");
            query.AppendLine(" [idConcepto],");
            query.AppendLine(" [Total] = ROUND(SUM([Total]),2),");
            query.AppendLine(" [Codigo Carrier],");
            query.AppendLine(" [Carrier],");
            query.AppendLine(" [Codigo Linea],");
            query.AppendLine(" [Linea]");
            if (link != "")
            {
                query.AppendLine(" ," + link + "''' ,");
            }
            else
            {
                query.AppendLine("',");
            }
            query.AppendLine(" @Where = @Where,   ");
            query.AppendLine(" @Group = '[Concepto],[idConcepto],[Codigo Carrier],[Carrier],[Detalle],[Codigo Linea],[Linea]',");
            query.AppendLine(" @Order = '[Total] Desc', ");
            query.AppendLine(" @OrderInv = '[Total] Asc', ");
            query.AppendLine(" @OrderDir = 'Asc',");
            query.AppendLine(" @Carrier = 373,");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ", ");
            query.AppendLine(" @Moneda = 'MXP', ");
            query.AppendLine(" @Idioma = 'Español', ");
            query.AppendLine(" @FechaIniRep  = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine(" @FechaFinRep  = '" + Session["FechaFin"].ToString() + " 23:59:59'");
            return query.ToString();
        }
        private string ConsultaRepPorConceptoDesglose()
        {
            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("DECLARE @Where VARCHAR(MAX)= ''   ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.
            consulta.AppendLine("SELECT @Where = 'FechaInicio BETWEEN ''" + Session["FechaInicio"].ToString() + " 00:00:00''" + " AND ''" + Session["FechaFin"].ToString() + " 23:59:59''" + "'");

            #region Filtro por Linea
            if (!string.IsNullOrEmpty(param["Linea"]) && param["Linea"] != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Linea] = " + param["Linea"] + " ' ");
            }
            #endregion Filtro por Linea

            #region Filtro por Concepto
            if (!string.IsNullOrEmpty(param["Concepto"]) && param["Concepto"] != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [idConcepto] = " + param["Concepto"] + " ' ");
            }
            #endregion Filtro por Concepto

            consulta.AppendLine("EXEC [spConsolidadoFacturasDeMovilesRest]  ");  //Este es el que se usa en el Dashboard de moviles
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("   [idConcepto],");
            consulta.AppendLine("   [Concepto] = UPPER([Concepto]),");
            consulta.AppendLine("   [Descripcion],");
            consulta.AppendLine("   [Telefono],");
            consulta.AppendLine("   [Total] = ROUND(SUM([Total]),2)");
            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,   ");
            consulta.AppendLine("   @Group = 'UPPER([Concepto]),[Descripcion],[idConcepto],[Telefono]',");
            consulta.AppendLine("   @Order = '[Total] Desc,[Concepto] Asc', ");
            consulta.AppendLine("   @OrderInv = '[Total] Asc,[Concepto] Desc', ");
            consulta.AppendLine("   @OrderDir = 'Desc',");
            consulta.AppendLine("   @Carrier = 373,");
            consulta.AppendLine("   @Usuario = " + Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Moneda = 'MXP', ");
            consulta.AppendLine("   @Idioma = 'Español', ");
            consulta.AppendLine("   @FechaIniRep  = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            consulta.AppendLine("   @FechaFinRep  = '" + Session["FechaFin"].ToString() + " 23:59:59'");
            return consulta.ToString();
        }
        private string ConsultaRepPorConceptoDetalleLamadas()
        {
            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("DECLARE @Where VARCHAR(MAX)= '' ");
            consulta.AppendLine("DECLARE @CampoTotal VARCHAR(40) = '' ");
            consulta.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.
            consulta.AppendLine("SELECT @Where = 'FechaInicio BETWEEN ''" + Session["FechaInicio"].ToString() + " 00:00:00''" + " AND ''" + Session["FechaFin"].ToString() + " 23:59:59''" + "'");

            #region Filtro por Linea
            if (!string.IsNullOrEmpty(param["Linea"]) && param["Linea"] != "0")
            {
                consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Linea] = " + param["Linea"] + " ' ");
            }
            #endregion Filtro por Linea
            consulta.AppendLine("SELECT @Where = @Where + ' AND [Codigo Carrier] = 373' ");

            #region Filtro por Concepto
            if (!string.IsNullOrEmpty(param["Concepto"]) && param["Concepto"] != "0")
            {
                consulta.AppendLine("");
                consulta.AppendLine("SELECT @CampoTotal = (SELECT vchCodigo FROM " + DSODataContext.Schema + ".Catalogos WHERE iCodRegistro = " + param["Concepto"] + ")");
            }
            #endregion Filtro por Concepto

            consulta.AppendLine("");
            consulta.AppendLine("EXEC [ConsumoTelcelDetalleLlamRestSinClaveCar]  ");  //Este es el que se usa en el Dashboard de moviles
            consulta.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@Fields=' ");
            consulta.AppendLine("   [Fecha Llamada],");
            consulta.AppendLine("   [Hora Llamada],");
            consulta.AppendLine("   [Numero Marcado] = '''''''' + [Numero Marcado],");
            consulta.AppendLine("   [Duracion Minutos],");
            consulta.AppendLine("   [Costo]=([Costo]/[TipoCambio]),");
            consulta.AppendLine("   [Punta A],");
            consulta.AppendLine("   [Dir Llamada],");
            consulta.AppendLine("   [Punta B]");
            consulta.AppendLine("',  ");
            consulta.AppendLine("   @Where = @Where,");
            consulta.AppendLine("   @CampoTotal = @CampoTotal,");
            consulta.AppendLine("   @Group = '',");
            consulta.AppendLine("   @Usuario = " + Session["iCodUsuario"] + ", ");
            consulta.AppendLine("   @Perfil = " + Session["iCodPerfil"] + ", ");
            consulta.AppendLine("   @Moneda = 'MXP', ");
            consulta.AppendLine("   @Idioma = 'Español', ");
            consulta.AppendLine(" @FechaIniRep  = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            consulta.AppendLine(" @FechaFinRep  = '" + Session["FechaFin"].ToString() + " 23:59:59'");
            return consulta.ToString();
        }
        private string ConsultaRepEtiquetacion()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' ");
            query.AppendLine(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''");
            query.AppendLine(" and[Codigo Tipo Destino]  not in(''381'',''382'',''384'') ");
            query.AppendLine(" and[Clave Tipo Llamada] in (''1'',''0'',''2'')'");
            query.AppendLine(" exec ConsumoDetalladoTodosCamposSinMovAccEtqV2 @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Nombre Centro de Costos2],");
            query.AppendLine(" [Codigo Centro de Costos],");
            query.AppendLine(" [Nombre Completo],");
            query.AppendLine(" [Gasto Laboral] = SUM(Case[Clave Tipo Llamada] When ''2'' Then  (([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])) else 0  END),");
            query.AppendLine(" [Gasto Personal] = SUM(Case[Clave Tipo Llamada] When ''1'' Then  (([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])) else 0  END),");
            query.AppendLine(" [Gasto Por Identificar]=SUM(Case When  [Clave Tipo Llamada] IS NULL   Then  (([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])) When[Clave Tipo Llamada] = ''0''   Then(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])) else 0 END),");
            query.AppendLine(" [Gasto Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine(" [Fecha Ultima Etiqueta],");
            query.AppendLine(" [Fecha Ultimo Acceso]',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Nombre Centro de Costos2],");
            query.AppendLine(" [Codigo Centro de Costos],");
            query.AppendLine(" [Nombre Completo],");
            query.AppendLine(" [Fecha Ultima Etiqueta],");
            query.AppendLine(" [Fecha Ultimo Acceso]', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string ConsultaRepMinutosPorCarrier(string link)
        {
            try
            {

                StringBuilder query = new StringBuilder();

                query.AppendLine("exec ConsumoAcumuladoTodosCamposRest															");
                query.AppendLine("@Schema='" + DSODataContext.Schema + "',																			");
                query.AppendLine("@Fields='																						");
                query.AppendLine("	[Nombre Carrier]=Min(upper([Nombre Carrier])),												");
                query.AppendLine("	[Codigo Carrier],																			");
                query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),								");
                query.AppendLine("	[Numero]=sum([TotalLlamadas]),																");
                query.AppendLine("	[Duracion]=sum([Duracion Minutos]), " + link + "', 														");
                query.AppendLine("@Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''',	");
                query.AppendLine("@Group = '[Codigo Carrier]', 																	");
                query.AppendLine("@Order = '[Total] Desc,[Numero] Desc,[Duracion] Desc,[Nombre Carrier] Asc',					");
                query.AppendLine("@OrderInv = '[Total] Asc,[Numero] Asc,[Duracion] Asc,[Nombre Carrier] Desc',					");
                query.AppendLine("@Lenght = 99,																					");
                query.AppendLine("@Start = 0,																					");
                query.AppendLine("@OrderDir = 'Desc',																			");
                query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",																			");
                query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",																				");
                query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',														");
                query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',														");
                query.AppendLine("@Moneda = 'MXP',																				");
                query.AppendLine("@Idioma = 'Español'																			");

                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaEmpsConLlamsACel10Digs(string linkGrafica, int numeroRegistros)
        {
            bool utilizarSPConParams = false;
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = ''\n");
            lsb.Append(" select @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''\n ");

            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Centro de Costos] = " + param["CenCos"] + "' \n");
                utilizarSPConParams = true;
            }

            if (param["Sitio"] != string.Empty && param["Sitio"] != "''")
            {
                lsb.Append("select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "' \n");
                utilizarSPConParams = true;
            }

            if (param["TDest"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Tipo Destino] =''" + param["TDest"] + "''' \n");
                utilizarSPConParams = true;
            }

            if (param["Carrier"] != string.Empty)
            {
                lsb.Append("select @Where = @Where + 'AND [Codigo Carrier] =''" + param["Carrier"] + "''' \n");
                utilizarSPConParams = true;
            }

            lsb.Append("exec [RepTabEmpsConsumoCel10Digs] @Schema='" + DSODataContext.Schema + "',\n ");

            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                lsb.Append("@Fields='[No Nomina], \n");
                lsb.Append("[Nombre Completo]=Min(upper([Nombre Completo])), \n");
                lsb.Append("[Codigo Empleado],\n");
                lsb.Append("[Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])),\n");
                lsb.Append("[Codigo Centro de Costos],\n");
            }
            else
            {
                lsb.Append("@Fields='[Nombre Completo]=Min(upper([Nombre Completo])), \n");
                lsb.Append("[Codigo Empleado],\n");
                lsb.Append("[Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])),\n");
                lsb.Append("[Codigo Centro de Costos],\n");
            }


            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("            [Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),\n");
            lsb.Append("            [Numero]=SUM([TotalLlamadas]),\n");



            //NZ 20160823 
            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                lsb.Append("        [Duracion]=sum([Duracion Minutos]), \n");
                lsb.Append("        [Puesto]',\n");
            }
            else
            {
                lsb.Append("        [Duracion]=sum([Duracion Minutos])', \n");
            }

            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Codigo Empleado], \n");

            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                lsb.Append("        [No Nomina], \n");
                lsb.Append("        [Codigo Centro de Costos], \n");
                lsb.Append("        [Puesto]', \n");
            }
            else
            {
                lsb.Append("        [Codigo Centro de Costos]', \n");
            }

            lsb.Append("@Order = '[Total] Desc',\n");
            lsb.Append("@OrderInv = '[Total] Asc',\n");
            lsb.Append("@OrderDir = 'Asc',\n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español',\n");
            lsb.Append("@Lenght = " + numeroRegistros.ToString() + "\n");

            return lsb.ToString();
        }
        public string RepTabConsPorEmpleado2(int numeroRegistros)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            query.AppendLine(" exec ConsumoAcumuladoPorEmpleadoRest @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Nombre Centro de Costos],[Codigo Centro de Costos],");
            query.AppendLine(" [Nombre Completo] = UPPER([Nombre Completo]), [Codigo Empleado],");
            query.AppendLine(" [CostoFija] = ROUND(Sum(Case When [Nombre Categoria Tipo Destino] in (''Fija'') then(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])) else 0 end),2),");
            query.AppendLine(" [CostoMovil] = ROUND(Sum(Case When [Nombre Categoria Tipo Destino] in (''Movil'') then([Costo]/[TipoCambio]) +( [CostoSM]/[TipoCambio]) else 0 end),2),");
            query.AppendLine(" [Total] = ROUND(Sum(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),2), ");
            query.AppendLine(" [Numero] = SUM([TotalLlamadas]),");
            query.AppendLine(" [Duracion] = SUM([Duracion Minutos]),");
            query.AppendLine(" [Link] =''/UserInterface/DashboardFC/Dashboard.aspx?Nav=EmpleMCN2&Emple=''+CONVERT(VARCHAR,[Codigo Empleado])+''  '' ', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Nombre Centro de Costos],[Codigo Centro de Costos],[Nombre Completo],[Codigo Empleado]', ");
            query.AppendLine(" @Order = '[Total] DESC',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español',");
            query.AppendLine(" @Lenght = " + numeroRegistros.ToString() + "");

            return query.ToString();
        }

        public string RepTabTopEmpleMasCaros(int numeroRegistros)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            query.AppendLine(" exec ConsumoTopNEmpleMasCaros @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Nombre Centro de Costos],[Codigo Centro de Costos],");
            query.AppendLine(" [Nombre Completo] = UPPER([Nombre Completo]), [Codigo Empleado],");
            query.AppendLine(" [CostoFija] = ROUND(Sum(Case When [Nombre Categoria Tipo Destino] in (''Fija'') then(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])) else 0 end),2),");
            query.AppendLine(" [CostoMovil] = ROUND(Sum(Case When [Nombre Categoria Tipo Destino] in (''Movil'') then([Costo]/[TipoCambio]) +( [CostoSM]/[TipoCambio]) else 0 end),2),");
            query.AppendLine(" [Total] = ROUND(Sum(([Costo]/[TipoCambio])+([CostoSM]/[TipoCambio])),2), ");
            query.AppendLine(" [Numero] = SUM([TotalLlamadas]),");
            query.AppendLine(" [Duracion] = SUM([Duracion Minutos]),");
            query.AppendLine(" [Link] =''/UserInterface/DashboardFC/Dashboard.aspx?Nav=EmpleMCN2&Emple=''+CONVERT(VARCHAR,[Codigo Empleado])+''  '' ', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Nombre Centro de Costos],[Codigo Centro de Costos],[Nombre Completo],[Codigo Empleado]', ");
            query.AppendLine(" @Order = '[Total] DESC',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español',");
            query.AppendLine(" @Lenght = " + numeroRegistros.ToString() + "");

            return query.ToString();
        }


        public string ConsultaReportePorEmpleConJer()
        {
            StringBuilder query = new StringBuilder();

            try
            {
                query.AppendLine(" Exec BuscaConJerEmple ");
                query.AppendLine("  @schema = '" + DSODataContext.Schema + "', ");
                query.AppendLine("	@dtIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
                query.AppendLine("	@dtFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59', ");
                query.AppendLine("	@usuar = " + Session["iCodUsuario"] + "  ");
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return query.ToString();
        }
        public string ConsultaReporteLlamNoContestadas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC[RepTabLlamPerdidas]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");
            return query.ToString();
        }
        public string ConsutaReporteLlamNoContestadasN2()
        {
            string sp = "EXEC [RepTabLlamPerdidasEmples] @Schema = '{0}',@Usuario = {1},@Perfil = {2},@Sitio = {3},@FechaIniRep = '{4}',@FechaFinRep = '{5}'";
            string query = string.Format(sp, DSODataContext.Schema, Session["iCodUsuario"], Session["iCodPerfil"], param["Sitio"], Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59");
            return query.ToString();
        }
        public string ConsultaReporteLlamNoContestadasN3(int opcion)
        {
            string sp = " EXEC DetallLlAmPerdidasEmple @Schema='{0}',@Usuario = {1},@Perfil = {2}, @Emple = {3},@FechaIniRep = '{4}',@FechaFinRep = '{5}',@Opcion = {6}";
            string query = string.Format(sp, DSODataContext.Schema, Session["iCodUsuario"], Session["iCodPerfil"], param["Emple"], Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59", opcion);

            return query.ToString();
        }

        public string ConsultaConsumoPorSitioSiana(string linkGrafica)
        {
            if (string.IsNullOrEmpty(isFT))
            {
                isFT = "0";
            }

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoSoloSianaPorSitioOptDashFC    @Schema='" + DSODataContext.Schema + "',\n ");
            lsb.Append("			                           @Fields='[Nombre Sitio]=Min(upper([Nombre Sitio])),\n");
            lsb.Append("[Codigo Sitio],\n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),\n");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Numero]=sum([TotalLlamadas]),\n");
            lsb.Append("[Duracion]=sum([Duracion Minutos])', \n");
            lsb.Append("@Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Group = '[Codigo Sitio]', \n");
            lsb.Append("@Order = '[Total] Desc,[Numero] Desc,[Duracion] Desc,[Nombre Sitio] Asc',\n");
            lsb.Append("@OrderInv = '[Total] Asc,[Numero] Asc,[Duracion] Asc,[Nombre Sitio] Desc',\n");
            lsb.Append("@OrderDir = 'Desc',\n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',\n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español'\n");
            lsb.Append(", @isFT = " + isFT + " \n");


            return lsb.ToString();
        }
        public string ConsultaConsumoPorTipoDestinoSiana(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" declare @Where varchar(max) = ''");
            lsb.AppendLine(" select @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  ");
            lsb.AppendLine(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' '");
            lsb.AppendLine(" select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "'");
            lsb.AppendLine(" select @Where = @Where + ' AND [Codigo Empleado] = " + param["Emple"] + "'");
            lsb.AppendLine(" exec RepTabConsumoPorTipoDestinoSoloSianaOptDashFC @Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine(" @Group = '[Codigo Tipo Destino]', ");
            lsb.AppendLine(" @Fields = '[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),");
            lsb.AppendLine(" [Codigo Tipo Destino],");
            lsb.AppendLine(" [Total]= convert(money, sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio])),");
            lsb.Append(linkGrafica + ", \n ");
            lsb.AppendLine(" [Numero] = SUM([TotalLlamadas]),");
            lsb.AppendLine(" [Duracion] = sum([Duracion Minutos])', ");
            lsb.AppendLine(" @Where = @Where, ");
            lsb.AppendLine(" @Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc',");
            lsb.AppendLine(" @OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc',");
            lsb.AppendLine(" @OrderDir = 'Desc',");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine(" @Moneda = 'MXP',");
            lsb.AppendLine(" @Idioma = 'Español'");

            return lsb.ToString();
        }
        public static string ConsultaDetalleSiana()
        {

            string nombreSP = "ConsumoDetalladoSoloSianaDashFC";

            //Se utiliza para comparar el tdest actual con call y ld para seven
            int tdestSevenEleven = 0;

            int.TryParse(param["TDest"].ToString(), out tdestSevenEleven);

            if
            (
                //Se comenta validacion porque deberia de servir para todos los clientes
                //DSODataContext.Schema.ToLower() == "seveneleven" &&
                (
                    tdestSevenEleven == 387 || //CelNac
                    tdestSevenEleven == 386 || //CelLoc
                    tdestSevenEleven == 389 || //LDM
                    tdestSevenEleven == 388 || //LDInt
                    tdestSevenEleven == 383    //800E

                )
            )
            {
                nombreSP = "ConsultaDetalleSevenElevenCellLD";
            }


            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine(" declare @Where varchar(max) = ''");
            lsb.AppendLine(" SELECT @Where = '1 = 1'");
            lsb.AppendLine(" select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "'");
            lsb.AppendLine(" select @Where = @Where + ' AND [iCodEmple] = " + param["Emple"] + "'");
            lsb.AppendLine(" select @Where = @Where + ' AND [iCodTDest] = " + param["TDest"] + "'");
            lsb.AppendLine(" exec[" + nombreSP + "]");
            lsb.AppendLine(" @Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine(" @Fields = '");
            lsb.AppendLine(" [Centro de costos],");
            lsb.AppendLine(" [Colaborador],");
            lsb.AppendLine(" [Nomina],");
            lsb.AppendLine(" [Extensión]	 ,");
            lsb.AppendLine(" [Numero Marcado],");
            lsb.AppendLine(" [Fecha],");
            lsb.AppendLine(" [Hora],");
            lsb.AppendLine(" [Fecha Fin],");
            lsb.AppendLine(" [Hora Fin],");
            lsb.AppendLine(" [Duracion],");
            lsb.AppendLine(" [Llamadas],");
            lsb.AppendLine(" [TotalSimulado] = CONVERT(MONEY, Round((CostoFac+CostoSM),2)),");
            lsb.AppendLine(" [TotalReal] = CONVERT(MONEY, Round((Costo+CostoSM),2)),");
            lsb.AppendLine(" [CostoSimulado] = CONVERT(MONEY, ROUND((CostoFac),2)),");
            lsb.AppendLine(" [CostoReal] = CONVERT(MONEY, ROUND((Costo),2)),");
            lsb.AppendLine(" [SM] = (CostoSM),");
            lsb.AppendLine(" [Nombre Sitio]	as [Sitio],");
            lsb.AppendLine(" [Nombre Carrier] as [Carrier],");
            lsb.AppendLine(" [Tipo Llamada],");
            lsb.AppendLine(" [Codigo Autorizacion],");
            lsb.AppendLine(" [Nombre Localidad] as [Localidad],");
            lsb.AppendLine(" [Tipo de destino]',");
            lsb.AppendLine(" @Where = @Where,");
            lsb.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @Idioma = 'Español',");
            lsb.AppendLine(" @Moneda = 'MXP',");
            lsb.AppendLine("@FechaIniRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00',");
            lsb.AppendLine("@FechaFinRep = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'");
            WhereAdicional = string.Empty;  //NZ Se pone en esta sección, para limpiar la variable exactamente despues de ser usada. //Se limpia de nuevo para que no afecte a otros reportes

            return lsb.ToString();
        }
        public string ConsultaPorEmpleMasCarosSiana(string linkGrafica, int numeroRegistros)
        {

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine(" declare @Where varchar(max) = ''");
            lsb.AppendLine(" select @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + "  00:00:00''  ");
            lsb.AppendLine(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            lsb.AppendLine(" select @Where = @Where + ' AND [Codigo Sitio] = " + param["Sitio"] + "'");
            lsb.AppendLine(" exec[RepTabConsumoEmpsMasCarosSoloSianaSPSinLineaDashFC] @Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine(" @Fields = '[Nombre Completo]=Min(upper([Nombre Completo])), ");
            lsb.AppendLine(" [Codigo Empleado],");
            lsb.AppendLine(" [Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])),");
            lsb.AppendLine(" [Codigo Centro de Costos],");
            lsb.Append(linkGrafica + ", \n ");
            lsb.AppendLine(" [Total] = sum([Costo] /[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            lsb.AppendLine(" [Numero]=SUM([TotalLlamadas]),");
            lsb.AppendLine(" [Duracion]=sum([Duracion Minutos])', ");
            lsb.AppendLine(" @Where = @Where, ");
            lsb.AppendLine(" @Group = '[Codigo Empleado], ");
            lsb.AppendLine(" [Codigo Centro de Costos]', ");
            lsb.AppendLine(" @Order = '[Total] Desc',");
            lsb.AppendLine(" @OrderInv = '[Total] Asc',");
            lsb.AppendLine(" @OrderDir = 'Asc',");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine(" @Moneda = 'MXP',");
            lsb.AppendLine(" @Idioma = 'Español',");
            lsb.AppendLine(" @Lenght = 2147483647");
            return lsb.ToString();
        }
        #region BOOKING&MANAGMENT SEEYOUONE
        public string ConsultaUtilSistemasCliente()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" declare @Where varchar(max)");
            lsb.AppendLine(" set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' '");
            lsb.AppendLine(" exec RepSeeYouOnUtilCliente @Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine(" @Fields = 'Client,");
            lsb.AppendLine(" Cliente = Max(ClientDesc),");
            lsb.AppendLine(" Utilizacion = CONVERT(VARCHAR,dateadd(SS,SUM(DuracionSeg),''19000101''),108),");
            lsb.AppendLine(" Cantidad = Count(*),");
            lsb.AppendLine(" [link] = ''/UserInterface/DashboardFC/Dashboard.aspx?Nav=RepTabSeeYouOnUtilClienteN2&Client=''+convert(varchar,Client),");
            lsb.AppendLine(" UtilizacionGraf = convert(numeric(15,2),SUM(DuracionSeg)/3600.0)',");
            lsb.AppendLine(" @Where = @Where,");
            lsb.AppendLine(" @Group = 'Client', ");
            lsb.AppendLine(" @Idioma = 'Español',");
            lsb.AppendLine(" @Moneda = 'MXP',");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ", ");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ", ");
            lsb.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', ");
            lsb.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''' ");

            return lsb.ToString();
        }
        public string ConsultaUtilSistemasClienteN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' '");
            query.AppendLine(" set @Where = @Where + 'And Client = " + param["Client"] + " '");
            query.AppendLine(" exec RepDetalleVCSSystem    @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = 'Cliente = Max(ClientDesc),");
            query.AppendLine(" Client,");
            query.AppendLine(" Sistema = Max(VCSSourceSystemDesc),");
            query.AppendLine(" VCSSourceSystem,");
            query.AppendLine(" Utilizacion = CONVERT(VARCHAR,dateadd(SS, SUM(DuracionSeg),''19000101''),108),");
            query.AppendLine(" Cantidad = Count(*),");
            query.AppendLine(" [link] = ''/UserInterface/DashboardFC/Dashboard.aspx?Nav=RepTabSeeYouOnUtilClienteN3&Sistema=''+convert(varchar,VCSSourceSystem)',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = 'Client,");
            query.AppendLine(" VCSSourceSystem', ");
            query.AppendLine(" @Idioma = 'Español',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            return query.ToString();
        }
        public string ConsultaUtilSistemasClienteN3()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" declare @ParamSistema varchar(max)");
            query.AppendLine(" set @ParamSistema = '" + param["Sistema"] + "'");
            query.AppendLine(" set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' '");
            query.AppendLine(" if @ParamSistema <> 'null'");
            query.AppendLine(" set @Where = @Where + 'And (VCS in(' + @ParamSistema + ')");
            query.AppendLine(" Or VCSSourceSystem in('+@ParamSistema+')");
            query.AppendLine(" Or VCSDestinationSystem in('+@ParamSistema+'))'");
            query.AppendLine(" exec RepDetalleVCS  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = 'FechaInicio,VCSName,NetworkAddress,DuracionSeg = CONVERT(VARCHAR,dateadd(SS,DuracionSeg,''19000101''),108),");
            query.AppendLine(" SourceNumber,DestinationNumber,Bandwidth', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Idioma = 'Español',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', ");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            return query.ToString();
        }
        public string ConsultaUtilSistema()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' '");
            query.AppendLine(" exec RepDetalleVCSSystem    @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = 'Cliente = Max(ClientDesc),");
            query.AppendLine(" Client,");
            query.AppendLine(" Sistema = Max(VCSSourceSystemDesc),");
            query.AppendLine(" VCSSourceSystem,");
            query.AppendLine(" Utilizacion = CONVERT(VARCHAR,dateadd(SS, SUM(DuracionSeg),''19000101''),108),");
            query.AppendLine(" Cantidad = Count(*),");
            query.AppendLine(" [link] = ''/UserInterface/DashboardFC/Dashboard.aspx?Nav=RepTabSeeYouOnUtilClienteN3&Sistema=''+convert(varchar,VCSSourceSystem)',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = 'Client,");
            query.AppendLine(" VCSSourceSystem', ");
            query.AppendLine(" @Idioma = 'Español',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            return query.ToString();
        }
        public string ConsultaUtilSistemaHistorico()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @fechaFin varchar(30)");
            query.AppendLine(" declare @fechaInicio varchar(30)");
            query.AppendLine(" declare @fechaInicioActual varchar(30)");
            query.AppendLine(" declare @anio int");
            query.AppendLine(" declare @mes int");
            query.AppendLine(" declare @dia int");
            query.AppendLine(" set @mes = MONTH(GETDATE())");
            query.AppendLine(" set @anio = YEAR(GETDATE())");
            query.AppendLine(" set @dia = DAY(GETDATE())");
            query.AppendLine(" set @fechaInicioActual = CONVERT(varchar, @anio) + '-0' + CONVERT(varchar, @mes) + '-01 12:00:00'");
            query.AppendLine(" if @mes < 10");
            query.AppendLine(" begin");
            query.AppendLine(" set @fechaInicio = CONVERT(varchar, @anio - 1) + '-0' + CONVERT(varchar, @mes) + '-01 12:00:00'");
            query.AppendLine(" set @fechaFin = CONVERT(varchar, @anio) + '-0' + CONVERT(varchar, @mes) + '-' + convert(varchar, day(dateadd(month, 1, @fechaInicioActual) - 1)) + ' 23:59:59'");
            query.AppendLine(" end");
            query.AppendLine(" else");
            query.AppendLine(" begin");
            query.AppendLine(" set @fechaInicio = CONVERT(varchar, @anio - 1) + '-' + CONVERT(varchar, @mes) + '-01 12:00:00'");
            query.AppendLine(" set @fechaFin = CONVERT(varchar, @anio) + '-' + CONVERT(varchar, @mes) + '-' + convert(varchar, day(dateadd(month, 1, @fechaInicioActual) - 1)) + ' 23:59:59'");
            query.AppendLine(" end");
            query.AppendLine(" set @fechaInicio = '''' + @fechaInicio + ''''");
            query.AppendLine(" set @fechaFin = '''' + @fechaFin + ''''");
            query.AppendLine(" exec RepSeeYouOnUtilSistemaHist @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Nombre Mes],Utilizacion = CONVERT(VARCHAR,dateadd(SS,SUM(DuracionSeg),''19000101''),108)', ");
            query.AppendLine(" @Group = '[Nombre Mes]',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = @fechaInicio,");
            query.AppendLine(" @FechaFinRep = @fechaFin");
            return query.ToString();
        }
        public string ConsultaCencosHorasHombre()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            query.AppendLine(" exec RepTabCenCosHorasHombre @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = 'Cencosdesc,");
            query.AppendLine(" TiempoTotal = CONVERT(VARCHAR, dateadd(SS, SUM(DuracionSeg), ''19000101''), 108),");
            query.AppendLine(" TiempoTotalOrig = Sum(DuracionSeg),");
            query.AppendLine(" Cantidad = Count(*),");
            query.AppendLine(" Promedio = CONVERT(VARCHAR, dateadd(SS, Avg(DuracionSeg), ''19000101''), 108) ', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = 'Cencosdesc', ");
            query.AppendLine(" @Idioma = 'Español',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ", ");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', ");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''' ");
            return query.ToString();
        }
        #endregion
        public string ConsultaLlamPorTipoDestinoN2()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine(" exec[ImporteEmplePorTpLlam]   @Schema = '" + DSODataContext.Schema + "', ");
            lsb.AppendLine(" @Fields = '[iCodCatEmple],[nominaEmple], [nombreEmple],[cencos],[cencosCod],[cencosDesc],");
            lsb.AppendLine(" Total = convert(numeric(15, 2), sum([Costo] /[TipoCambio]) + sum([CostoSM] /[TipoCambio]))', ");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', ");
            lsb.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', ");
            lsb.AppendLine(" @Moneda = 'MXP',");
            lsb.AppendLine(" @Idioma = 'Español' ,");
            lsb.AppendLine(" @isFT = " + param["TipoLlam"] + " ");
            return lsb.ToString();
        }
        public string ConsultaRepMarketing()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine(" declare @Where varchar(max)");
            lsb.AppendLine(" set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' And [Codigo Centro de Costos]=''240723'''");
            lsb.AppendLine(" exec spDetalleMatrizRest @Schema = 'Caintra',");
            lsb.AppendLine(" @InnerFields = '[Nombre Completo],");
            lsb.AppendLine(" [Codigo Empleado],");
            lsb.AppendLine(" [FechaInicio] = convert(varchar(10), [FechaInicio], 120),");
            lsb.AppendLine(" [Llamadas]=count(*)', ");
            lsb.AppendLine(" @InnerWhere=@Where, ");
            lsb.AppendLine(" @InnerGroup='[Nombre Completo],");
            lsb.AppendLine(" [Codigo Empleado],");
            lsb.AppendLine(" convert(varchar(10), [FechaInicio], 120)', ");
            lsb.AppendLine(" @OuterFields='[Nombre Completo],");
            lsb.AppendLine(" [Codigo Empleado],");
            lsb.AppendLine(" [Llamadas] = SUM([Llamadas])', ");
            lsb.AppendLine(" @OuterGroup='[Nombre Completo],");
            lsb.AppendLine(" [Codigo Empleado]',");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine(" @Moneda = 'MXP',");
            lsb.AppendLine(" @Idioma = 'Español'");
            return lsb.ToString();
        }
        public string ConsultaConsumoBolsa()
        {
            string execSp = "EXEC dbo.ObtieneConsumoBolsa @FechaIni = '{0}', @FechaFin = '{1}'";
            string query = string.Format(execSp, Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59");
            return query;
        }
        public string ConsultaConsumoBolsaDiaria()
        {
            string execSp = "EXEC dbo.ObtieneBolsaDiaria @FechaIni = '{0}', @FechaFin = '{1}'";
            string query = string.Format(execSp, Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59");
            return query;
        }
        public string ConsultaObtienFiltroDetalle()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" FiltroQuery AS Filtro");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('ConfAlarmCantMinLDM','Configuracion Alarma Cant Min LDM','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchCodigo = '" + param["FiltroNav"] + "'");

            return query.ToString();
        }

        public string ConsultaDetalleConsumo(string filtro)
        {
            StringBuilder fields = new StringBuilder();

            fields.AppendLine(" [Centro de costos],");
            fields.AppendLine(" [Colaborador],");
            fields.AppendLine(" [Extensión],");
            fields.AppendLine(" [Numero Marcado],");
            fields.AppendLine(" [Fecha],[Hora],");
            fields.AppendLine(" [Duracion],[Costo],");
            fields.AppendLine(" [Nombre Localidad] as [Localidad],");
            fields.AppendLine(" [Nombre Sitio] as [Sitio],");
            fields.AppendLine(" [Nombre Carrier] as [Carrier],");
            fields.AppendLine(" [Tipo de destino],[Categoría],[Etiqueta]");
            string DetallLllamadas = " Exec [RepDetalladoLlamadasLDIyLDM] @Schema = '{0}', @Fields ='{1}',@Filtro = '{2}', @FechaIniRep = '{3}', @FechaFinRep = '{4}', @duracionMin = {5}";
            string sp_Detall = string.Format(DetallLllamadas, DSODataContext.Schema, fields.ToString(), filtro.Replace("'", "''"), Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59", 0) + ",@agregaUnidMedida = 1";
            return sp_Detall;
        }

        #region Reporte Qualtia por Centro de costos
        // EV Para este reporte se toman los datos de la Organización como Centro de costos a petición de eflores
        // Esta consideración se hizo desde los SP para no tener que modificar el reporte en caso de cambios
        public string RepMatQualtiaPorCenCos()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatQualtiaPorCenCos] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepMatQualtiaPorEmple()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatQualtiaPorEmple] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@where = ' [Codigo Centro de Costos] = " + param["CenCos"] + " ',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("	@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return query.ToString();
        }

        public string RepDetalladoEmple()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec [SPRepConsumoDetallado]");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine("@Fields=' ");
            query.AppendLine("[Colaborador]	 , ");
            query.AppendLine("[Nomina],");
            query.AppendLine("[Extensión]	 , ");
            query.AppendLine("[Numero Marcado] as [Número Marcado] , ");
            query.AppendLine("[Fecha] , ");
            query.AppendLine("[Hora] , ");
            query.AppendLine("[Fecha Fin],");
            query.AppendLine("[Hora Fin],");
            query.AppendLine("[Duracion] as [Cantidad minutos], ");


            if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
            {
                query.AppendLine("[Costo] = (CostoFac+CostoSM), ");
            }
            else
            {
                query.AppendLine("[Costo] = (Costo+CostoSM), ");
            }

            query.AppendLine("[Nombre Localidad] as [Localidad], ");
            query.AppendLine("[Nombre Sitio]	as [Sitio] , ");
            query.AppendLine("[Codigo Autorizacion] as [Código Autorización] , ");
            query.AppendLine("[Nombre Carrier] as [Carrier], ");
            query.AppendLine("[Tipo de destino],  ");
            query.AppendLine("[Categoría],  ");
            query.AppendLine("[Etiqueta]'  ,  ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine("@VistaAUsar = 'CDR',  ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',  ");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@Emple = ' = " + param["Emple"] + "' ,  ");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',  ");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }
        #endregion Reporte Qualtia por Centro de costos

        #region Reporte llamadas perdidas por extension

        public string RepTabPerdidasPorExt()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepTabPerdidasPorExt] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("	@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }

        public string RepMatPerdidasPorExtPorDia()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatPerdidasPorExtPorDia] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("	@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }

        public string RepMatPerdidasPorExtPorDiaPnl()
        {
            string strFecha = Session["FechaFin"].ToString();
            string[] arrFecha = strFecha.Split('-');

            int anio = Convert.ToInt32(arrFecha[0]);
            int mes = Convert.ToInt32(arrFecha[1]);
            int dia = Convert.ToInt32(arrFecha[2]);

            DateTime fechaF = new DateTime(anio, mes, dia);
            DateTime fechaI = fechaF.AddDays(-4);

            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [RepMatPerdidasPorExtPorDia] ");
            query.AppendLine("	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@Idioma ='Español', ");
            query.AppendLine("	@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("	@FechaIniRep = '" + fechaI.ToString("yyyy-MM-dd 00:00:00") + "',");
            query.AppendLine("	@FechaFinRep = '" + fechaF.ToString("yyyy-MM-dd 23:59:59") + "'");

            return query.ToString();
        }

        public string RepDetalladoPorExt()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec [SPRepConsumoDetallado]");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine("@Fields=' ");
            query.AppendLine("[Centro de costos] , ");
            query.AppendLine("[Colaborador]	 , ");
            query.AppendLine("[Nomina],");
            query.AppendLine("[Extensión]	 , ");
            query.AppendLine("[Numero Marcado] as [Número Marcado] , ");
            query.AppendLine("[Fecha] , ");
            query.AppendLine("[Hora] , ");
            query.AppendLine("[Fecha Fin],");
            query.AppendLine("[Hora Fin],");
            query.AppendLine("[Duracion] as [Cantidad minutos], ");
            query.AppendLine("[Costo] = (Costo+CostoSM), ");
            query.AppendLine("[Nombre Localidad] as [Localidad], ");
            query.AppendLine("[Nombre Sitio]	as [Sitio] , ");
            query.AppendLine("[Codigo Autorizacion] as [Código Autorización] , ");
            query.AppendLine("[Nombre Carrier] as [Carrier], ");
            query.AppendLine("[Tipo de destino],  ");
            query.AppendLine("[Categoría],  ");
            query.AppendLine("[Etiqueta]'  ,  ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine("@VistaAUsar = 'CDR',  ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',  ");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@Extension = ' = ''" + param["Extension"] + "'' ' ,");
            query.AppendLine("@TDest = ' = 381' ,");
            query.AppendLine("@Duracion = ' = 0' ,");
            query.AppendLine("@FechaIniRep = '" + param["Dia"] + " 00:00:00',  ");
            query.AppendLine("@FechaFinRep = '" + param["Dia"] + " 23:59:59'");


            return query.ToString();
        }

        public string RepDetalladoPerdidas()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec [SPRepConsumoDetallado]");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine("@Fields=' ");
            query.AppendLine("[Centro de costos] , ");
            query.AppendLine("[Colaborador]	 , ");
            query.AppendLine("[Nomina],");
            query.AppendLine("[Extensión]	 , ");
            query.AppendLine("[Numero Marcado] as [Número Marcado] , ");
            query.AppendLine("[Fecha] , ");
            query.AppendLine("[Hora] , ");
            query.AppendLine("[Fecha Fin],");
            query.AppendLine("[Hora Fin],");
            query.AppendLine("[Duracion] as [Cantidad minutos], ");
            query.AppendLine("[Costo] = (Costo+CostoSM), ");
            query.AppendLine("[Nombre Localidad] as [Localidad], ");
            query.AppendLine("[Nombre Sitio]	as [Sitio] , ");
            query.AppendLine("[Codigo Autorizacion] as [Código Autorización] , ");
            query.AppendLine("[Nombre Carrier] as [Carrier], ");
            query.AppendLine("[Tipo de destino],  ");
            query.AppendLine("[Categoría],  ");
            query.AppendLine("[Etiqueta]'  ,  ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine("@VistaAUsar = 'CDR',  ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',  ");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@TDest = ' = 381' ,");
            query.AppendLine("@Duracion = ' = 0' ,");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',  ");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }
        #endregion Reporte llamadas perdidas por extension

        /*Reportes de Desvio*/
        public string RepDetalladoDesvios(int desvios = 0)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec [SPRepConsumoDetallado]");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine("@Fields=' ");
            query.AppendLine("[Centro de costos] , ");
            query.AppendLine("[Colaborador]	 , ");
            query.AppendLine("[Nomina],");
            query.AppendLine("[Extensión]	 , ");
            query.AppendLine("[Numero Marcado] as [Número Marcado] , ");
            query.AppendLine("[Fecha] , ");
            query.AppendLine("[Hora] , ");
            query.AppendLine("[Fecha Fin],");
            query.AppendLine("[Hora Fin],");
            query.AppendLine("[Duracion] as [Cantidad minutos], ");
            query.AppendLine("[Costo] = (Costo+CostoSM), ");
            query.AppendLine("[Nombre Localidad] as [Localidad], ");
            query.AppendLine("[Nombre Sitio]	as [Sitio] , ");
            query.AppendLine("[Codigo Autorizacion] as [Código Autorización] , ");
            query.AppendLine("[Nombre Carrier] as [Carrier], ");
            query.AppendLine("[Tipo de destino],  ");
            query.AppendLine("[Categoría],  ");
            query.AppendLine("[Etiqueta]'  ,  ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine("@VistaAUsar = 'CDR',  ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',  ");
            query.AppendLine("@Idioma = 'Español',  ");
            switch (desvios)
            {
                case 0:
                    query.AppendLine(" @TDest = ' in (SELECT iCodCatalogo FROM " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')]");
                    query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    query.AppendLine(" AND vchCodigo LIKE ''%PorDesvio'')' ,");
                    break;
                case 1:
                    query.AppendLine(" @Extension = '= ''" + param["Extension"] + "''',");
                    query.AppendLine(" @Emple = '= " + param["Emple"] + "',");
                    query.AppendLine(" @TDest = ' in (SELECT iCodCatalogo FROM " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')]");
                    query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    query.AppendLine(" AND (UPPER(vchDescripcion) = ''" + param["TDest"] + " POR DESVIO'' OR UPPER(vchDescripcion) = ''" + param["TDest"] + " POR DESVÍO''))' ,");

                    break;
                case 2:
                    query.AppendLine(" @Extension = '= ''" + param["Extension"] + "''',");
                    query.AppendLine(" @Emple = '= " + param["Emple"] + "',");
                    query.AppendLine(" @TDest = ' in (SELECT iCodCatalogo FROM " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')]");
                    query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    query.AppendLine(" AND vchCodigo LIKE ''%PorDesvio'')',");
                    break;
                case 3:
                    query.AppendLine(" @Emple = '= " + param["Emple"] + "',");
                    query.AppendLine(" @CenCos= '=" + param["CenCos"] + "',");
                    query.AppendLine(" @TDest = ' in (SELECT iCodCatalogo FROM " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')]");
                    query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    query.AppendLine(" AND vchCodigo LIKE ''%PorDesvio'')',");
                    break;
            }
            if (param["Nav"] == "RepDesviosTop10ExtN2")
            {
                query.AppendLine(" @Extension = '= ''" + param["Extension"] + "''',");
            }
            if (param["Nav"] == "RepDesviosPerdidosPorEmpleN2")
            {
                query.AppendLine(" @Extension = '= ''" + param["Extension"] + "''',");
                query.AppendLine(" @Duracion = '= ''0''',");
                query.AppendLine(" @TpLlam = 'IN(''Salida'' ,''Entrada'')',");
            }
            if (param["Nav"] == "RepDesviosPorHoraN2")
            {
                query.AppendLine("@Hora = '" + param["Hora"] + "',  ");
            }

            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',  ");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");



            return query.ToString();
        }
        public string RepDesviosTipoDestino()
        {
            StringBuilder query = new StringBuilder();
            string link = "" + Request.Path + "?Nav=RepDesviosTipoDestinoN2&TDest=";
            query.AppendLine(" EXEC [dbo].[RepDesviosTipoDestino]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            query.AppendLine(" ,@link = '" + link + "'");
            return query.ToString();
        }
        public string RepDesviosTipoDestinoN2(string link)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" EXEC [dbo].[RepDesviosTipoDestinoN2]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            query.AppendLine(" ,@link = " + link + "");
            if (param["TDest"] != null && param["TDest"] != "")
            {
                query.AppendLine(" ,@TpDestino = '" + param["TDest"] + "'");
            }
            if (param["Sitio"] != null && param["Sitio"] != "")
            {
                query.AppendLine(" ,@Sitio = '" + param["Sitio"] + "'");
            }

            return query.ToString();
        }
        public string RepDesviosPorCencos()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" EXEC [dbo].[RepDesviosCenCosto]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            if (param["CenCos"] != null && param["CenCos"] != "")
            {
                query.AppendLine(" ,@link = '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&NumDesvios=3&CenCos=''+CONVERT(VARCHAR,A.icodCatalogo)+''&Emple=''+CONVERT(VARCHAR,A.Emple)'");
                query.AppendLine(" ,@Cencosto=" + param["CenCos"] + "");
            }
            else
            {
                query.AppendLine(" ,@link = '''" + Request.Path + "?Nav=RepDesviosCenCostoN2&CenCos=''+CONVERT(VARCHAR,A.icodCatalogo)'");
            }

            return query.ToString();
        }
        public string RepDesviosPorSitio()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" EXEC [dbo].[RepDesviosSitio]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            query.AppendLine(" ,@link = '''" + Request.Path + "?Nav=RepDesviosSitioN2&CenCos=''+CONVERT(VARCHAR,A.Sitio)'");
            return query.ToString();
        }

        #region Reportes desvios


        public string RepDesviosPorHora()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [SPRepDesviosPorHora]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();
        }

        public string RepDesviosTop10Llamadas(string by)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [SPRepDesviosTop10Llamadas]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "' ,");
            query.AppendLine(" @By = '" + by + "'");

            return query.ToString();
        }

        public string RepDesviosTop10Ext()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [SPRepDesviosTop10Ext]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();
        }

        public string RepDesviosPerdidosPorEmple()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [SPRepDesviosPerdidosPorEmple]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();
        }

        public string RepDesviosPorCarrier()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [SPRepDesviosPorCarrier]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();
        }
        public string RepExtensionessinActVSBD()
        {
            string sp = "EXEC dbo.ExtenSinActividaVSBD @Schema='{0}',@Usuario={1},@Perfil={2},@FechaIniRep='{3}',@FechaFinRep='{4}'";
            string query = string.Format(sp, DSODataContext.Schema, Session["iCodUsuario"], Session["iCodPerfil"], Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59");
            return query;
        }
        public string RepTraficoExtensiones()
        {
            string sp = "EXEC [dbo].[TraficoExtensiones] @Schema='{0}',@Usuario={1},@Perfil={2},@FechaIniRep='{3}',@FechaFinRep='{4}'";
            string query = string.Format(sp, DSODataContext.Schema, Session["iCodUsuario"], Session["iCodPerfil"], Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59");
            return query;
        }
        #endregion Reportes desvios

        #region Reporte por dispositivo IKUSI
        public string RepLlamadasPorDispositivo()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec ObtieneCantidadLlamadasPorDispositivo ");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "', ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + "', ");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + "', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ", ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ", ");
            query.AppendLine("@isGet = 0 ");
            return query.ToString();
        }

        public string RepLlamadasPorDispositivoExt()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec ObtieneCantidadLlamadasPorExtUnDispositivo ");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "', ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + "', ");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + "', ");
            query.AppendLine(" @iCodCatTipoDispositivo = " + param["Dispositivo"] + ", ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ", ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ", ");
            query.AppendLine("@isGet = 0 ");

            return query.ToString();
        }

        public string RepLlamadasPorDispositivoDet()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [SPRepConsumoDetallado]");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine("@Fields=' ");
            query.AppendLine("[Centro de costos] , ");
            query.AppendLine("[Colaborador]	 , ");
            query.AppendLine("[Nomina],");
            query.AppendLine("[Extensión]	 , ");
            query.AppendLine("[Numero Marcado] as [Número Marcado] , ");
            query.AppendLine("[Fecha] , ");
            query.AppendLine("[Hora] , ");
            query.AppendLine("[Fecha Fin],");
            query.AppendLine("[Hora Fin],");
            query.AppendLine("[Duracion] as [Cantidad minutos], ");
            query.AppendLine("[Costo] = (Costo+CostoSM), ");
            query.AppendLine("[Nombre Localidad] as [Localidad], ");
            query.AppendLine("[Nombre Sitio]	as [Sitio] , ");
            query.AppendLine("[Codigo Autorizacion] as [Código Autorización] , ");
            query.AppendLine("[Nombre Carrier] as [Carrier], ");
            query.AppendLine("[Tipo de destino],  ");
            query.AppendLine("[Codec] = '''',  ");
            query.AppendLine("[Dispositivo]',  ");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine("@VistaAUsar = 'CDR',  ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',  ");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@Extension = ' = ''" + param["Extension"] + "'' ' ,");
            query.AppendLine("@Dispositivo = '" + param["Dispositivo"] + "' ,");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',  ");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");
            return query.ToString();
        }
        #endregion Reporte por dispositivo IKUSI
        #region LlamadasPerdidas
        public string RepLlamadasPerdidasTdest(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max) = ''");
            query.AppendLine("select @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''  ");
            query.AppendLine("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59" + "''");

            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                query.AppendLine("and [Codigo Tipo Destino] IN (select iCodCatalogo from " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')] where dtfinvigencia>=getdate() and vchcodigo in (''Ent'',''EntPorDesvio'',''Enl'',''EnlPorDesvio'',''ExtExt'', ''ExtExtPorDesvio'')) ");

            }

            query.AppendLine("AND[Duracion Minutos] = 0 AND upper([Nombre Tipo Destino]) <> ''TELCEL'''");
            query.AppendLine("exec RepTabCantLlamadasPerdidas @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Group = '[Codigo Tipo Destino]', ");
            query.AppendLine("@Fields = '[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),");
            query.AppendLine("[Codigo Tipo Destino],");
            query.Append(linkGrafica + ", \n ");
            query.AppendLine("[Numero] = SUM([TotalLlamadas])', ");
            query.AppendLine("@Where = @Where, ");
            query.AppendLine("@OrderDir = 'Desc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59" + "''',");
            query.AppendLine("@Moneda = 'MXP',");
            query.AppendLine("@Idioma = 'Español'");
            return query.ToString();
        }
        public string RepLlamadasPerdidasSitio(string linkGrafica, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            StringBuilder query = new StringBuilder();
            if (string.IsNullOrEmpty(isFT))
            {
                isFT = "0";
            }
            query.AppendLine(" exec RepTabLlamadasPerdidasPorSitio");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Nombre Sitio]=Min(upper([Nombre Sitio])),");
            query.AppendLine(" [Codigo Sitio],");
            query.Append(linkGrafica + ", \n ");
            query.AppendLine(" [Numero] = sum([TotalLlamadas])', ");
            query.AppendLine(" @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''");
            query.AppendLine(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59" + "''");

            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                query.AppendLine("and [Codigo Tipo Destino] IN (select iCodCatalogo from " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')] where dtfinvigencia>=getdate() and vchcodigo in (''Ent'',''EntPorDesvio'',''Enl'',''EnlPorDesvio'',''ExtExt'', ''ExtExtPorDesvio'')) ");
            }

            query.AppendLine(" AND[Duracion Minutos] = 0 ',");

            query.AppendLine(" @Group = '[Codigo Sitio]', ");
            query.AppendLine(" @OrderDir = 'Desc',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59" + "''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español', ");
            query.AppendLine(" @omitirInfoCDR = 0, ");
            query.AppendLine(" @omitirInfoSiana = 0 ");
            query.AppendLine(" , @isFT = " + isFT + "");
            return query.ToString();
        }
        public string RepLlamadasPerdidasCencos(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [RepCenCosLlamPerdidas]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Idioma = 'Español', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59" + "'''");
            query.AppendLine(" , @link = " + linkGrafica + "");

            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                query.AppendLine(", @where = ' and [TDest] IN (select iCodCatalogo from [vishistoricos(''TDest'',''Tipo de destino'',''Español'')] where dtfinvigencia>=getdate() and vchcodigo in (''Ent'',''EntPorDesvio'',''Enl'',''EnlPorDesvio'',''ExtExt'', ''ExtExtPorDesvio''))' ");

            }

            return query.ToString();
        }
        public string RepLlamadasPerdidasCencosN2(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [RepCenCosLlamPerdidasPorEmple]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Idioma = 'Español', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59" + "'''");
            query.AppendLine(" ,@link = " + linkGrafica);

            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                query.AppendLine(" ,@where = ' and Detall.TDest IN (select iCodCatalogo from " + DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')] where dtfinvigencia>=getdate() and vchcodigo in (''Ent'',''EntPorDesvio'',''Enl'',''EnlPorDesvio'',''ExtExt'', ''ExtExtPorDesvio'')) '");
            }
            
            query.AppendLine(" ,@Cencos = " + param["CenCos"] + "");
            return query.ToString();
        }
        public string RepLlamadasPerdidasTopEmple(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max) = ''");
            query.AppendLine(" select @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00" + "'' ");
            query.AppendLine(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59" + "'' AND[Duracion Minutos] = 0'");


            if (DSODataContext.Schema.ToLower() == "laureate")
            {
                query.Append("select @Where = @Where + 'and [Codigo Tipo Destino] IN (select iCodCatalogo from "+ DSODataContext.Schema + ".[vishistoricos(''TDest'',''Tipo de destino'',''Español'')] where dtfinvigencia>=getdate() and vchcodigo in (''Ent'',''EntPorDesvio'',''Enl'',''EnlPorDesvio'',''ExtExt'', ''ExtExtPorDesvio''))' \n");
            }


            query.AppendLine(" exec[RepTabLlamadasPerdidasTopNEmple] @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @omitirInfoCDR = 0,");
            query.AppendLine(" @omitirInfoSiana = 0,");
            query.AppendLine(" @Fields = ' ");
            query.AppendLine(" [Nombre Completo] = Min(upper([Nombre Completo])), ");
            query.AppendLine(" [Codigo Empleado],");
            query.Append(linkGrafica + ", \n ");
            query.AppendLine(" [Numero] = SUM([TotalLlamadas])', ");
            query.AppendLine(" @Where = @Where, ");
            query.AppendLine(" @Group = '[Codigo Empleado]', ");
            query.AppendLine(" @Order = '[Numero] Desc',");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''',");
            query.AppendLine(" @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59" + "''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español',");
            query.AppendLine(" @Lenght = 10");
            return query.ToString();
        }
        #endregion LlamadasPerdidas

        public string RepLlamadasAgrupEmpleN1(string linkGrafica)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max) = ''");
            query.AppendLine("select @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:0" + "'' ");
            query.AppendLine(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59" + "'' AND[Duracion Minutos] = 0'");
            query.AppendLine("exec[RepTabConsumoEmpsMasCarosSPSinLineaDashFC] @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@omitirInfoCDR = 0,");
            query.AppendLine("@omitirInfoSiana = 0,");
            query.AppendLine("@Fields = ' ");
            query.AppendLine("[Nombre Completo] = Min(upper([Nombre Completo])),");
            query.AppendLine("[Codigo Empleado],");
            query.AppendLine(linkGrafica + " \n ");
            query.AppendLine("[Total] = sum([Costo] /[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("[Numero]=SUM([TotalLlamadas]),");
            query.AppendLine("[Duracion]=sum([Duracion Minutos])', ");
            query.AppendLine("@Where = @Where,");
            query.AppendLine("@Group = '[Codigo Empleado]',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00" + "''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59" + "''',");
            query.AppendLine("@Moneda = 'MXP',");
            query.AppendLine("@Idioma = 'Español'");

            return query.ToString();

        }

        public string RepLlamadasAgrupEmpleN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("exec [SPRepConsumoDetallado]");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields = '");
            query.AppendLine("[Centro de costos] ,");
            query.AppendLine("[Colaborador],");
            query.AppendLine("[Nomina],");
            query.AppendLine("[Extensión],");
            query.AppendLine("[Numero Marcado] as [Número Marcado] ,");
            query.AppendLine("[Fecha] ,");
            query.AppendLine("[Hora] ,");
            query.AppendLine("[Fecha Fin],");
            query.AppendLine("[Hora Fin],");
            query.AppendLine("[Duracion] as [Cantidad minutos],");
            query.AppendLine("[Costo] = (Costo+CostoSM),");
            query.AppendLine("[Nombre Localidad] as [Localidad],");
            query.AppendLine("[Nombre Sitio]	as [Sitio] ,");
            query.AppendLine("[Codigo Autorizacion] as [Código Autorización] ,");
            query.AppendLine("[Nombre Carrier] as [Carrier],");
            query.AppendLine("[Tipo de destino],");
            query.AppendLine("[Categoría],");
            query.AppendLine("[Etiqueta]' ,");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@VistaAUsar = 'CDR',");
            query.AppendLine("@Moneda ='MX',");
            query.AppendLine("@Idioma ='Español',");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00 " + "',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "',");
            query.AppendLine("@Emple = ' = " + param["Emple"] + "'   ");

            return query.ToString();
        }

        public string RepConsumoColabDirectos()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [dbo].[BuscaColabDirectos]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Usuar = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @dtIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @dtFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();
        }

        public string RepConsumoporDeptoN1()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [dbo].[ConsumoPorDepto]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Usuar = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @dtIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @dtFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();

        }

        public string RepConsumoporDeptoN2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [dbo].[ConsumoPorColaborador]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Cencos = " + param["CenCos"] + ",  ");
            query.AppendLine(" @dtIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @dtFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();

        }

        public string RepCantLineasContestadasYNoContestadas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [CantidadLlamadasPerdidasYContestadas]");
            query.AppendLine("@Schema =  '" + DSODataContext.Schema + "',");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "',");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "',");
            query.AppendLine(" @dtIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00" + "',");
            query.AppendLine(" @dtFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59" + "'");
            return query.ToString();
        }

        public string RepTraficoPorExtensionPI()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC ObtieneTraficoLlamadasPI ");
            query.AppendLine("@esquema =  '" + DSODataContext.Schema + "', ");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "', ");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "', ");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + "'");
            return query.ToString();
        }

        public string ObtieneTraficoLlamadasPorExtension()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC ObtieneTraficoLlamadasPorExtension ");
            query.AppendLine("@esquema =  '" + DSODataContext.Schema + "', ");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "', ");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "', ");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + "'");
            return query.ToString();
        }

        public string RepDetalladoDesdePorExtension()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec Keytia5..SPRepConsumoDetalladoITESM ");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Fields = '[Fecha] , [Hora] ,[Extensión], [Numero Marcado], [Nombre Localidad] as [Localidad], [Duracion], [Total] = (Costo+CostoSM), Timbrado, [Tipo de destino], Resultado,OrigReason, DestReason, [Centro de costos] , [Colaborador], [Nombre Sitio]as [Sitio]', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @VistaAUsar = 'CDR',  ");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',  ");
            query.AppendLine(" @Idioma = 'Español',  ");
            query.AppendLine(" @Extension = '= ''" + param["Extension"] + "''',");
            query.AppendLine(" @Sitio = '= " + param["Sitio"] + "',");
            if (param["CenCos"] != null && param["CenCos"] != "")
            {
                query.AppendLine(" @CenCos= '=" + param["CenCos"] + "',");
            }
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',  ");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }

        public string RepDetalladoHospitales()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec Keytia5..SPRepConsumoDetalladoITESM ");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine(" @Fields = '[Fecha] , [Hora] ,[Extensión], [Numero Marcado], [Nombre Localidad] as [Localidad], [Duracion], [Total] = (Costo+CostoSM), Timbrado, [Tipo de destino], Resultado,OrigReason, DestReason, [Centro de costos] , [Colaborador], [Nombre Sitio]as [Sitio]', ");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",  ");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",  ");
            query.AppendLine(" @VistaAUsar = 'CDR',  ");
            query.AppendLine(" @Moneda = '" + Session["Currency"] + "',  ");
            query.AppendLine(" @Idioma = 'Español',  ");
            query.AppendLine(" @Extension = ' like ''" + param["FiltroNav"] + "%''', ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',  ");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }


        public string RepTabCantLlamsContestadasYNoPorSitio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC ObtieneCantidadLlamadasContestadasyNoPorSitio ");
            query.AppendLine("@esquema =  '" + DSODataContext.Schema + "', ");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "', ");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "', ");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + "'");
            return query.ToString();
        }

        public string ObtieneCantidadLlamadasContestadasyNoUnSitioPorExt()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC ObtieneCantidadLlamadasContestadasyNoUnSitioPorExt ");
            query.AppendLine("@esquema =  '" + DSODataContext.Schema + "', ");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "', ");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "', ");
            query.AppendLine(" @Sitio = ' = " + param["Sitio"] + "',");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + "'");
            return query.ToString();
        }

        //// TODO : DO Paso 1 - Consulta del reporte 
        public string FechaInicio => Session["FechaInicio"].ToString() + " 00:00:00";
        public string FechaFinal => Session["FechaFin"].ToString() + " 23:59:59";
        public string ObtieneConsultaMes()
        {
            return $"EXEC ObtenerParticipantesvsReunionesporMes '{DSODataContext.Schema}', '{FechaInicio}', '{FechaFinal}'";
        }

        public string ObtieneParticipantesHoras()
        {
            return $"EXEC ObtenerParticipantesHorasporMes '{DSODataContext.Schema}', '{FechaInicio}', '{FechaFinal}'";
        }

        public string ObtieneNumeroReuniones()
        {
            return $"EXEC ObtenerHorasPicoSemana '{DSODataContext.Schema}', '{FechaInicio}', '{FechaFinal}'";
        }

        public string ObtieneNoParticipantes()
        {
            return $@"
               SELECT  CASE 
			                WHEN CHARINDEX(' ', nomcompleto) = 0 THEN nomcompleto
			                ELSE LEFT(nomcompleto, CHARINDEX(' ', nomcompleto) - 1)
		                END AS Nombre ,* 
                from {DSODataContext.Schema}.[vishistoricos('Emple','Empleados','Español')] 
                where dtFinVigencia >= getdate() AND
	                  NominaA <> ('999999999') AND
                iCodCatalogo not in (select ParticipantiCodCatEmple 
					                 from {DSODataContext.Schema}.DetalleEventosVideoConferencia 
					                 where DateEvent >= '{FechaInicio}' and  DateEvent <= '{FechaFinal}')";
        }

        public string Plataformameet()
        {
            return $@"
               select ct.ClientTypeName as Nombre, count(*) AS [No. Reuniones], Sum(DurationMins) as [Minutos totales] 
                from {DSODataContext.Schema}.DetalleEventosVideoConferencia dv
                inner join {DSODataContext.Schema}.ClientTypes ct on ct.ClientTypeID =  dv.ClientTypeID
				where DateEvent >= '{FechaInicio}' and  DateEvent <= '{FechaFinal}'
                GROUP BY ct.ClientTypeName
                order by [Minutos totales]  desc";
        }

        

        public string Plataformameetdetail(string plataforma)
        {
            return $@"
                SELECT CASE 
			            WHEN CHARINDEX(' ', nomcompleto) = 0 THEN nomcompleto
			            ELSE LEFT(nomcompleto, CHARINDEX(' ', nomcompleto) - 1)
		            END AS Nombre , count(*) AS [No. Reuniones], Sum(DurationMins) as [Minutos totales] 
                            from {DSODataContext.Schema}.DetalleEventosVideoConferencia dv
                            inner join {DSODataContext.Schema}.[vishistoricos('Emple','Empleados','Español')] ct on ct.iCodCatalogo =  dv.ParticipantiCodCatEmple
				            where DateEvent >= '{FechaInicio}' and  DateEvent <= '{FechaFinal}' and
                            ClientTypeID = 1
                            GROUP BY ct.nomcompleto
                            order by [Minutos totales]  desc  ";
        }
        #endregion Consultas a BD

    }
}