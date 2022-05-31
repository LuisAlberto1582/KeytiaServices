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
using System.Web.Services;
using System.Globalization;
using KeytiaWeb.UserInterface.Indicadores;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DashboardMoviles
    {
            #region Consultas a BD
        
        private int ConsultaNumeroLineas(string lsCarrier)
        {
            StringBuilder consultaNumeroDeLineas = new StringBuilder();
            consultaNumeroDeLineas.AppendLine("select count(iCodRegistro) as NumeroLineas");
            consultaNumeroDeLineas.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')]");
            consultaNumeroDeLineas.AppendLine("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()");
            consultaNumeroDeLineas.AppendLine("and SitioCod like '%" + lsCarrier + "%' ");
            consultaNumeroDeLineas.AppendLine("and Emple = " + param["Emple"]);
            int numeroDeLineasTelcel = Convert.ToInt32(DSODataAccess.ExecuteScalar(consultaNumeroDeLineas.ToString()));
            return numeroDeLineasTelcel;
        }

        private void ConsultaLineaTelcel()
        {
            //Se agrega la consulta de la linea ya que en este punto no la trae el querystring
            StringBuilder consultaLineaTelcel = new StringBuilder();
            consultaLineaTelcel.AppendLine("select vchCodigo");
            consultaLineaTelcel.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')]");
            consultaLineaTelcel.AppendLine("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()");
            //consultaLineaTelcel.AppendLine("and SitioCod like '%telcel%' ");
            consultaLineaTelcel.AppendLine("AND vchCodigo = " + param["Tel"] + "");
            consultaLineaTelcel.AppendLine("and Emple = " + param["Emple"]);
            param["Linea"] = "";
            var line = DSODataAccess.ExecuteScalar(consultaLineaTelcel.ToString());

            if (line != System.DBNull.Value && line != null)
            {
                param["Linea"] = DSODataAccess.ExecuteScalar(consultaLineaTelcel.ToString()).ToString();
            }

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
            if (param["Tel"] != string.Empty)
            {
                lsb.Append("set @ParamExtension = '" + param["Tel"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamExtension = 'null' \n ");
            }
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

            lsb.Append("			                           @Fields='[Extension], \n ");
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
            if (txtCantMaxReg.Text.Length > 0)
            {
                lsb.Append("@Lenght = " + txtCantMaxReg.Text + ", \n ");
            }
            else
            {
                lsb.Append("@Lenght = 10, \n ");
            }
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

        public string ConsultaConsumoHistorico()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) = '' \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int  \n");
            lsb.Append("declare @mes int \n");
            lsb.Append("declare @dia int \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");

            //20141027 AM. Se agrega parametro de centro de costos
            lsb.Append("declare @ParamCenCos varchar(max) \n");

            lsb.Append("set @mes = MONTH(GETDATE()) \n");
            lsb.Append("set @anio = YEAR(GETDATE()) \n");
            lsb.Append("set @dia = DAY(GETDATE()) \n");
            if (param["Emple"] != string.Empty)
            {
                lsb.Append("set @ParamEmple = '" + param["Emple"] + "' \n");
            }
            else
            {
                lsb.Append("set @ParamEmple = 'null' \n");
            }

            //20141027 AM. Se agrega parametro de centro de costos
            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("set @ParamCenCos = '" + param["CenCos"] + "' \n");
            }
            else
            {
                lsb.Append("set @ParamCenCos = 'null' \n");
            }

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("if @mes < 10 \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("else \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("set @Where = 'FechaInicio >='+ @Fechainicio +' and FechaInicio <= '+ @FechaFin +' \n");
            lsb.Append("' \n");
            lsb.Append("if @ParamEmple <> 'null' \n");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n");
            lsb.Append("' \n");

            //20141027 AM. Se agrega parametro de centro de costos
            lsb.Append("if @ParamCenCos <> 'null' \n");
            lsb.Append("      set @Where = @Where + 'And [Codigo CenCos] in('+@ParamCenCos+') \n");
            lsb.Append("' \n");

            lsb.Append("exec [ConsumoHistoricoCFM]    @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Nombre Mes], \n");
            lsb.Append("Total = convert(numeric(15,2),sum(Costo/[TipoCambio])+sum(CostoSM/[TipoCambio]))',  \n");
            lsb.Append("@Where =@Where, \n");
            lsb.Append("@Group = '[Nombre Mes]', \n");
            lsb.Append("@Order = '[Orden] Asc', \n");
            lsb.Append("@OrderInv = '[Orden] Desc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaConsumoHistoricoMov()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @fechaFin varchar(30) \n ");
            lsb.Append("declare @fechaInicio varchar(30) \n ");

            lsb.Append("set @fechaInicio = '''' + convert(varchar, dateadd(mm, -12, '" + Session["FechaInicio"].ToString() + " 00:00:00'),120) + '''' \n ");
            lsb.Append("set @fechaFin = '''' + convert(varchar, '" + Session["FechaFin"].ToString() + " 23:59:59',120) + '''' \n ");

            lsb.Append("exec [ConsumoHistoricoCFM] @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Mes Anio] as ''Nombre Mes'', \n ");
            lsb.Append("Total = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]), count(distinct Telefono) as CantidadLineas',  \n ");
            lsb.Append("@Group = '[Mes Anio]', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n ");
            lsb.Append("@FechaFinRep = @fechaFin, \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaConsumoHistoricoMoviles()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @fechaFin varchar(30) \n ");
            lsb.Append("declare @fechaInicio varchar(30) \n ");
            lsb.Append("declare @fechaInicioActual varchar(30) \n ");
            lsb.Append("declare @anio int  \n ");
            lsb.Append("declare @mes int \n ");
            lsb.Append("declare @dia int \n ");
            lsb.Append("set @mes = MONTH(GETDATE()) \n ");
            lsb.Append("set @anio = YEAR(GETDATE()) \n ");
            lsb.Append("set @dia = DAY(GETDATE()) \n ");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n ");
            lsb.Append("if @mes < 10 \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n ");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n ");
            lsb.Append("end \n ");
            lsb.Append("else \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00' \n ");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n ");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n ");
            lsb.Append("exec [ConsumoHistoricoCFM] @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Nombre Mes], \n ");
            lsb.Append("TotalSinIVA = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]), \n ");
            lsb.Append("TotalConIVA = (SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio])) + SUM(IVA), \n ");
            lsb.Append("TotalLineas=count(distinct Telefono)', \n ");
            lsb.Append("@Group = '[Nombre Mes]', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n ");
            lsb.Append("@FechaFinRep = @fechaFin, \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaCostoPorEmpleado(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("declare @Where varchar(max)");
            lsb.AppendLine("declare @ParamCenCos varchar(max)");

            lsb.AppendLine("declare @ParamEqCelular varchar(max)"); //BG. 20180201

            if (param["CenCos"] != string.Empty)
            {
                lsb.AppendLine("set @ParamCenCos = '" + param["CenCos"] + "'");
            }
            else
            {
                lsb.AppendLine("set @ParamCenCos = 'null'");
            }

            //BG. 20180201 Se agrega parametro por si viene del 
            // Reporte Consumo Moviles Por Tecnologia
            if (param["EqCelular"] != string.Empty)
            {
                lsb.AppendLine("set @ParamEqCelular = '" + param["EqCelular"] + "'");
            }
            else
            {
                lsb.AppendLine("set @ParamEqCelular = 'null'");
            }


            lsb.AppendLine("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''");
            lsb.AppendLine("and FechaInicio < ''" + Session["FechaFin"].ToString() + " 23:59:59''");
            lsb.AppendLine("' ");
            lsb.AppendLine("if @ParamCenCos <> 'null'");
            lsb.AppendLine("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            lsb.AppendLine("'");

            //BG. 20180201
            lsb.AppendLine("if @ParamEqCelular <> 'null'");
            lsb.AppendLine("      set @Where = @Where + 'And [idEquipo] in('+@ParamEqCelular+')");
            lsb.AppendLine("'");

            lsb.AppendLine("exec RepTabConsumoEmpsMasCarosCFM @Schema='" + DSODataContext.Schema + "',");
            //lsb.Append("exec zzbgRepTabConsumoEmpsMasCarosCFM @Schema='" + DSODataContext.Schema + "', \n");
            lsb.AppendLine("@Fields='[Codigo Empleado],[Nombre Completo]=Min(upper([Nombre Completo])),[Linea],");
            lsb.AppendLine("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            lsb.AppendLine("[Renta]= sum([Renta]/[TipoCambio]),");
            lsb.AppendLine("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio]),");
            lsb.AppendLine("[iCodCatCarrier] = [Codigo Carrier]");
            if (linkGrafica != "")
            {
                lsb.AppendLine("," + linkGrafica + "");
            }
            lsb.AppendLine("', ");
            lsb.AppendLine("@Where = @Where, ");
            lsb.AppendLine("@Group = '[Codigo Empleado],[Linea], [Codigo Carrier]', ");
            lsb.AppendLine("@Order = '[Total] Desc',");
            lsb.AppendLine("@OrderInv = '[Total] Asc',");
            lsb.AppendLine("@OrderDir = 'Asc',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            lsb.AppendLine("@Idioma = 'Español',");
            lsb.AppendLine("@Carrier = " + ((param["Carrier"] != null && param["Carrier"].Length > 0) ? param["Carrier"] : "0"));
            return lsb.ToString();
        }

        public string ConsultaConsumoHistoricoPorEmpleadoMat()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");
            lsb.Append("declare @mes int \n");
            lsb.Append("declare @dia int \n");

            /*BG.20150114 Se cambia el calculo del mes */
            //lsb.Append("set @mes = MONTH(GETDATE()) - 1 \n");
            lsb.Append("set @mes = MONTH(GETDATE()) \n");
            lsb.Append("set @anio = YEAR(GETDATE()) \n");
            lsb.Append("set @dia = DAY(GETDATE()) \n");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("if @mes < 10 \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("else \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("exec [ConsumoHistoricoMatricialCFM]  \n");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Completo], \n");
            lsb.Append("[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden], \n");
            lsb.Append("Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup='[Nombre Completo], \n");
            lsb.Append(" upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@OuterFields='[Empleado]=[Nombre Completo], \n");
            lsb.Append(ArmaCamposConsumoHistoricoPorEmpleadoMat());
            lsb.Append("[Total] = SUM([Total])', \n");
            lsb.Append("@OuterGroup='[Nombre Completo]', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaCamposConsumoHistoricoPorEmpleadoMat()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");
            lsb.Append("declare @mes int \n");
            lsb.Append("declare @dia int \n");

            /*BG.20150114 Se cambia el calculo del mes */
            //lsb.Append("set @mes = MONTH(GETDATE()) - 1 \n");
            lsb.Append("set @mes = MONTH(GETDATE()) \n");
            lsb.Append("set @anio = YEAR(GETDATE()) \n");
            lsb.Append("set @dia = DAY(GETDATE()) \n");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("if @mes < 10 \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("else \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("exec [ConsumoHistoricoMatricialCFM]  @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup=' upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@Order='[Orden] Asc',       \n");
            lsb.Append("@OrderInv='[Orden] Desc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ArmaCamposConsumoHistoricoPorEmpleadoMat()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoHistoricoPorEmpleadoMat());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Mes"] + "] = SUM(case when [Nombre Mes] = ''" + ldr["Nombre Mes"] + "'' AND [Orden] = " + ldr["Orden"] + " then [Total] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaDetalleFactura()
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

            //lsb.Append("exec [zzjhConsumoTelcelDetalleFacRestVerticalConsolidado] \n");
            lsb.Append("exec [ConsumoTelcelDetalleFacRestVerticalConsolidado] \n");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Concepto] = Replace([Concepto],''.'',''''), \n");
            lsb.Append("[idConcepto], \n");
            lsb.Append("[Detalle], \n");
            lsb.Append("[Total],\n");
            lsb.Append("[Carrier],\n");
            lsb.Append("[DescCarrier]',  \n");
            lsb.Append("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.Append("@Carrier = '" + param["Carrier"] + "',\n ");
            lsb.Append("@Where =@Where, \n");
            lsb.Append("@Group = '[Concepto], \n");
            lsb.Append("[idConcepto], \n");
            lsb.Append("[Detalle],\n");

            //RM 20180220 Cambios Alfa
            lsb.Append("[Carrier],\n");
            lsb.Append("[DescCarrier]',  \n");

            lsb.Append("@Order = '[Total] Desc,[Concepto] Asc', \n");
            lsb.Append("@OrderInv = '[Total] Asc,[Concepto] Desc', \n");
            lsb.Append("@OrderDir = 'Desc', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaDescripcionDePerfil()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("select vchCodigo");
            lsb.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')]");
            lsb.AppendLine("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()");
            lsb.AppendLine("and iCodCatalogo = " + Session["iCodPerfil"]);
            return lsb.ToString();
        }

        public string ConsultaConsumoPorCenCos(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @fechaini varchar(10) \n");
            lsb.Append("set @fechaini = CONVERT(varchar(10), '" + Session["FechaInicio"].ToString() + " 00:00:00',120) \n");
            lsb.Append("declare @fechafin varchar(10) \n");
            lsb.Append("set @fechafin = CONVERT(varchar(10),'" + Session["FechaFin"].ToString() + " 23:59:59',120) \n");
            lsb.Append("set @Where = 'FechaInicio >=  '''+@fechaini+''' and FechaInicio < '''+@fechafin+''' \n");
            lsb.Append("' \n");
            lsb.Append("exec [RepTabConsumoEmpsMasCarosCFM] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("			                           @Fields='[Codigo Centro de Costos],[Nombre Centro de Costos]=MIN(upper([Nombre Centro de Costos])), \n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n");
            lsb.Append("[Renta]= SUM([Renta]/[TipoCambio]), \n");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - SUM([Renta]/[TipoCambio]) \n");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n");
            lsb.Append("@Where = @Where, \n");
            lsb.Append("@Group = '[Codigo Centro de Costos]',  \n");
            lsb.Append("@Order = '[Total] Desc', \n");
            lsb.Append("@OrderInv = '[Total] Asc', \n");
            lsb.Append("@OrderDir = 'Asc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaConsumoHistPorCenCosMat()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");
            lsb.Append("declare @mes int \n");
            lsb.Append("declare @dia int \n");

            /*BG.20150114 Se cambia el calculo del mes */
            //lsb.Append("set @mes = MONTH(GETDATE()) - 1 \n");
            lsb.Append("set @mes = MONTH(GETDATE()) \n");

            lsb.Append("set @anio = YEAR(GETDATE()) \n");
            lsb.Append("set @dia = DAY(GETDATE()) \n");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("if @mes < 10 \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("else \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("set @Where = '' \n");
            lsb.Append("exec [ConsumoHistoricoMatricialCFM] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Área] = MIN(upper([Nombre Centro de Costos])), \n");
            lsb.Append("[Codigo Centro de Costos], \n");
            lsb.Append("[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden], \n");
            lsb.Append("Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup='[Codigo Centro de Costos], \n");
            lsb.Append(" upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@OuterFields='[Codigo Centro de Costos],[Área], \n");
            lsb.Append(ArmaCamposConsumoHistCenCosMat());
            lsb.Append("[Total] = SUM([Total])', \n");
            lsb.Append("@OuterGroup='[Área], \n");
            lsb.Append("[Codigo Centro de Costos]', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaCamposConsumoHistCenCosMat()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");
            lsb.Append("declare @mes int \n");
            lsb.Append("declare @dia int \n");
            /*BG.20150114 Se cambia el calculo del mes */
            lsb.Append("set @mes = MONTH(GETDATE()) \n");
            //lsb.Append("set @mes = MONTH(GETDATE()) - 1 \n");
            lsb.Append("set @anio = YEAR(GETDATE()) \n");
            lsb.Append("set @dia = DAY(GETDATE()) \n");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("if @mes < 10 \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("else \n");
            lsb.Append("begin \n");
            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd( month,1,@fechaInicioActual) -1)) + ' 23:59:59' \n");
            lsb.Append("end \n");
            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("set @Where = '' \n");
            lsb.Append("exec [ConsumoHistoricoMatricialCFM] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup=' upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@Order='[Orden] Asc',       \n");
            lsb.Append("@OrderInv='[Orden] Desc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ArmaCamposConsumoHistCenCosMat()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoHistCenCosMat());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Mes"] + "] = SUM(case when [Nombre Mes] = ''" + ldr["Nombre Mes"] + "'' AND [Orden] = " + ldr["Orden"] + " then [Total] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaPlanEmpleado()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ISNULL(MAX('Plan: '  + ISNULL(PlanT.vchDescripcion, '') + '			Vigencia: ' + ISNULL(CONVERT(VARCHAR,Linea.FechaFinPlan,105), '')),'  ') ");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Linea");
            query.AppendLine("    JOIN " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Linea','Español')] RelLinea");
            query.AppendLine("        ON RelLinea.Linea = Linea.iCodCatalogo");
            query.AppendLine("        AND RelLinea.dtIniVigencia <> RelLinea.dtFinVigencia");
            query.AppendLine("        AND '" + Session["FechaInicio"].ToString() + " 00:00:00' >= RelLinea.dtIniVigencia");
            query.AppendLine("        AND '" + Session["FechaFin"].ToString() + " 23:59:59' >= RelLinea.dtIniVigencia");
            query.AppendLine("        AND RelLinea.Emple = " + param["Emple"] + "");
            query.AppendLine("");
            query.AppendLine("    LEFT JOIN " + DSODataContext.Schema + ".[VisHistoricos('PlanTarif','Plan Tarifario','Español')] PlanT");
            query.AppendLine("        ON PlanT.iCodCatalogo = Linea.PlanTarif");
            query.AppendLine("        AND PlanT.dtIniVigencia <> PlanT.dtFinVigencia");
            query.AppendLine("        AND PlanT.dtFinVigencia >= GETDATE()");
            query.AppendLine("");
            query.AppendLine("WHERE Linea.dtIniVigencia <> Linea.dtFinVigencia");
            query.AppendLine("    AND Tel = '" + param["Linea"] + "'");
            query.AppendLine("    AND Linea.Carrier = " + param["Carrier"]);

            return query.ToString();
        }

        public string ConsultaConsumoHistoricoPerfilEmpleado()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <> dtFinVigencia \n ");
            lsb.Append("and dtFinVigencia >= GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = ' DATEPART(YEAR,[FechaInicio]) = DATEPART(YEAR,dateadd(mm,-1,GETDATE())) AND [Codigo Empleado] = '+@Empleado+' ' \n ");
            lsb.Append("exec [ConsumoHistoricoCFM] @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Nombre Mes], \n ");
            lsb.Append("Total = convert(numeric(15,2),sum(Costo/[TipoCambio])+sum(CostoSM/[TipoCambio]))',  \n ");
            lsb.Append("@Where=@Where, \n ");
            lsb.Append("@Group = '[Nombre Mes]', \n ");
            lsb.Append("@Order = '[Orden] Asc', \n ");
            lsb.Append("@OrderInv = '[Orden] Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español', \n ");
            lsb.Append("@Carrier = " + ((param["Carrier"] != null && param["Carrier"].Length > 0) ? param["Carrier"] : "0") + "");
            return lsb.ToString();
        }

        public string ConsultaConsumoHistoricoDelEmpleado(string iCodEmpleado)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Empleado varchar(max) \n ");
            //lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            //lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            //lsb.Append("and dtIniVigencia <> dtFinVigencia \n ");
            //lsb.Append("and dtFinVigencia >= GETDATE() \n ");
            lsb.Append("set @Empleado = " + iCodEmpleado + " \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");
            lsb.Append("set @Where = ' DATEPART(YEAR,[FechaInicio]) = DATEPART(YEAR,dateadd(mm,-1,GETDATE())) AND [Codigo Empleado] = '+@Empleado+' ' \n ");
            lsb.Append("exec [ConsumoHistoricoCFM] @Schema='" + DSODataContext.Schema + "', \n ");
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

        public string ConsultaCostoPorEmpleadoPerfEmpleado()
        {
            StringBuilder lsb = new StringBuilder();

            /************************************************************************************************************/
            //20141223 AM. Se agrega a la consulta el filtro del iCodCatalogo del empleado 

            lsb.Append("declare @iCodEmple int  \n ");
            lsb.Append("select @iCodEmple = max(iCodCatalogo) from \n ");
            lsb.Append(DSODataContext.Schema);
            lsb.Append(".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + "' \n ");
            lsb.Append("and Usuar = " + Session["iCodUsuario"] + " \n ");

            lsb.Append("Declare @Where varchar(max) = ''\n ");
            lsb.Append("set @Where = @Where + 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''\n ");
            lsb.Append("and FechaInicio < ''" + Session["FechaFin"].ToString() + " 23:59:59'' AND [Codigo Empleado] = '+ convert(varchar,isNull(@iCodEmple,0)) \n ");

            /************************************************************************************************************/

            lsb.Append("exec RepTabConsumoEmpsMasCarosCFM @Schema='" + DSODataContext.Schema + "', \n ");
            //lsb.Append("exec zzbgRepTabConsumoEmpsMasCarosCFM @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Nombre Completo]=Min(upper([Nombre Completo])), \n ");
            lsb.Append("[Codigo Empleado], \n ");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n ");
            lsb.Append("[Renta]= sum([Renta]), \n ");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta])  ,\n ");
            lsb.Append("[Linea], \n");
            //RM 20180220 Cambios De Carrier y linea
            lsb.Append("[Codigo Carrier], \n");
            lsb.Append("[Linea Carrier],\n");
            lsb.Append("[idConcepto]',\n");
            //>>RM 20180220 Cambios De Carrier y linea
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Codigo Empleado], [Linea],[Codigo Carrier], [Linea Carrier],[idConcepto]',  \n ");
            lsb.Append("@Order = '[Total] Desc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc', \n ");
            lsb.Append("@Lenght = 10, \n ");
            lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            //lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            //lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
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
            if (param["Carrier"] != string.Empty)
            {
                lsb.Append("And carrier = " + param["Carrier"] + "\n");
            }
            else
            {
                lsb.Append("And carrier = 0 \n");
            }
            /*20150113 AM. Se agrega filtro de linea*/
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
            if (param["Carrier"] != string.Empty)
            {
                lsb.Append("And carrier = " + param["Carrier"] + "\n");
            }
            else
            {
                lsb.Append("And carrier = 0 \n");
            }

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
            //RM 20180220 Cambios Alfa
            //lsb.Append("exec ZzJHspConsolidadoFacturasDeMovilesRest \n ");
            lsb.Append("exec spConsolidadoFacturasDeMovilesRest \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idConcepto], [Concepto] = UPPER([Concepto]),[Descripcion], \n ");
            lsb.Append("[Telefono], \n ");
            lsb.Append("Total = sum([Total])',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.Append("@Carrier = '" + param["Carrier"] + "',\n ");
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

            lsb.Append("set @NumeroMovil = 'null' \n ");

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
            //lsb.Append("@Order = '[Costo] Desc,[Duracion Minutos] Desc,[Fecha Llamada] Asc,[Hora Llamada] Asc,[Dir Llamada] Asc,[Punta A] Asc,[Punta B] Asc,[Numero Marcado] Asc', \n ");
            //lsb.Append("@OrderInv = '[Costo] Asc,[Duracion Minutos] Asc,[Fecha Llamada] Desc,[Hora Llamada] Desc,[Dir Llamada] Desc,[Punta A] Desc,[Punta B] Desc,[Numero Marcado] Desc', \n ");
            //lsb.Append("@OrderDir = 'Desc', \n ");

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

        public string ConsultaLlamadasMasLargas()
        {
            StringBuilder lsb = new StringBuilder();

            /**********************************************/
            //20150109 AM. Se agrega filtro de empleado en base a usuario
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");

            /**********************************************/

            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @ParamCenCos varchar(max) \n ");
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("set @ParamCenCos = 'null' \n ");
            lsb.Append("set @ParamEmple = 'null' \n ");
            //lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            //lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''  \n ");
            //lsb.Append("' \n ");

            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("And [Codigo Empleado] = '+@Empleado+' \n ");
            lsb.Append("' \n ");

            lsb.Append("if @ParamCenCos <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoTelcelTopNLineas  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[Extension] = '''''''' + [Extension], \n ");
            lsb.Append("[Nombre Completo], \n ");
            lsb.Append("[Numero Marcado] = '''''''' + [Numero Marcado], \n ");
            lsb.Append("[Fecha Llamada], \n ");
            lsb.Append("[Hora Llamada], \n ");
            lsb.Append("[Duracion Minutos], \n ");
            lsb.Append("[Costo]=([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Dir Llamada], \n ");
            lsb.Append("[Dir Llamada Codigo], \n ");
            lsb.Append("[Punta A], \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '',  \n ");
            lsb.Append("@Order = '[Duracion Minutos] desc', \n ");
            lsb.Append("@OrderInv = '[Duracion Minutos] asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");

            if (txtCantMaxReg.Text.Length > 0)
            {
                lsb.Append("@Lenght = " + txtCantMaxReg.Text + ", \n ");
            }
            else
            {
                lsb.Append("@Lenght = 10, \n ");
            }

            lsb.Append("@Start = 0, \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaLlamadasMasCostosas()
        {
            StringBuilder lsb = new StringBuilder();

            /**********************************************/
            //20150109 AM. Se agrega filtro de empleado en base a usuario
            lsb.Append("declare @Empleado varchar(max) \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo) from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("if @Empleado is null \n ");
            lsb.Append("begin \n ");
            lsb.Append("	set @Empleado = '-1' \n ");
            lsb.Append("end \n ");

            /**********************************************/

            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @ParamCenCos varchar(max) \n ");
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("set @ParamCenCos = 'null' \n ");
            lsb.Append("set @ParamEmple = 'null' \n ");
            //lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            //lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''  \n ");
            //lsb.Append("' \n ");

            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("And [Codigo Empleado] = '+@Empleado+' \n ");
            lsb.Append("' \n ");

            lsb.Append("if @ParamCenCos <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoTelcelTopNLineas  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[Extension] = '''''''' + [Extension], \n ");
            lsb.Append("[Nombre Completo], \n ");
            lsb.Append("[Numero Marcado] = '''''''' + [Numero Marcado], \n ");
            lsb.Append("[Fecha Llamada], \n ");
            lsb.Append("[Hora Llamada], \n ");
            lsb.Append("[Duracion Minutos], \n ");
            lsb.Append("[Costo]=([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Dir Llamada], \n ");
            lsb.Append("[Dir Llamada Codigo], \n ");
            lsb.Append("[Punta A], \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '',  \n ");
            lsb.Append("@Order = '[Costo] desc', \n ");
            lsb.Append("@OrderInv = '[Costo] asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");

            if (txtCantMaxReg.Text.Length > 0)
            {
                lsb.Append("@Lenght = " + txtCantMaxReg.Text + ", \n ");
            }
            else
            {
                lsb.Append("@Lenght = 10, \n ");
            }

            lsb.Append("@Start = 0, \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaFechaMaximaDeDetallFactCDR()
        {
            StringBuilder lsb = new StringBuilder();
            //RM 20161214 Se modifico la consulta para que regrese una fecha por default, el primero del mes actual en dado caso de no encontrar una 
            lsb.Append("select isNull(Max(FechaInicio),'" + DateTime.Now.ToString("yyyy-MM-01 00:00:00") + "') as FechaInicio \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleFacturaCDR','Español')]  \n ");
            lsb.Append("where Carrier = 373 \n ");
            return lsb.ToString();
        }

        public string ConsultaLlamACelularesDeLaRed(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @carriercod int \n ");

            /************************************************************************************************************/
            //20141223 AM. Se agrega a la consulta el filtro del iCodCatalogo del empleado 

            lsb.Append("declare @iCodEmple int  \n ");
            lsb.Append("select @iCodEmple = max(iCodCatalogo) from \n ");
            lsb.Append(DSODataContext.Schema);
            lsb.Append(".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + "' \n ");
            lsb.Append("and Usuar = " + Session["iCodUsuario"] + " \n ");

            /************************************************************************************************************/

            lsb.Append("select @carriercod = iCodCatalogo \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Carrier','Carriers','Español')] \n ");
            lsb.Append("where vchDescripcion = 'Telcel' \n ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' AND [Codigo Empleado] = '+ convert(varchar,isNull(@iCodEmple,0)) \n ");
            lsb.Append("exec ConsumoTelcelDetalleLlamRedRestOptimizado @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Codigo Empleado],[Codigo Exten] = [Extension], [Extension], \n ");
            lsb.Append("[Nombre Completo], \n ");
            lsb.Append("[Total]=SUM([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Duracion]=sum([Duracion Minutos]), \n ");
            lsb.Append("[Numero]=count(*) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Extension], \n ");
            lsb.Append("[Nombre Completo], \n ");
            lsb.Append("[Codigo Empleado]',  \n ");
            lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Extension] Desc,[Nombre Completo] Desc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Extension] Asc,[Nombre Completo] Asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaNumerosMarcadosACelRed(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @ParamExtension varchar(max) \n ");
            lsb.Append("declare @carriercod int \n ");
            lsb.Append("select @carriercod = iCodCatalogo \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Carrier','Carriers','Español')] \n ");
            lsb.Append("where vchDescripcion = 'Telcel' \n ");
            lsb.Append("set @ParamExtension = '''" + param["Exten"].Replace("'", "") + "''' \n ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamExtension <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoTelcelDetalleLlamRedRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[CodNumMarc] = [Numero Marcado],[Numero Marcado],[Etiqueta],[Total]=SUM([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Duracion]=sum([Duracion Minutos]),[Numero]=count(*) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Numero Marcado], \n ");
            lsb.Append("[Etiqueta]',  \n ");
            lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Numero Marcado] Desc,[Etiqueta] Desc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Numero Marcado] Asc,[Etiqueta] Asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaDetalleDeLlamadasPerfilEmple()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @NumeroMovil varchar(max) \n ");
            lsb.Append("declare @NumeroMarcado varchar(max) \n ");
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
            lsb.Append("set @NumeroMovil = '''" + param["Exten"].Replace("'", "") + "''' \n ");
            lsb.Append("set @NumeroMarcado = '''" + param["NumMarc"].Replace("'", "") + "''' \n ");
            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @NumeroMarcado <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Numero Marcado] in('+@NumeroMarcado+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoTelcelDetalleLlamRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[FecDeLlamada], \n ");
            lsb.Append("[Hora Llamada], \n ");
            lsb.Append("[Duracion Minutos], \n ");
            lsb.Append("[Costo]=([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Punta A], \n ");
            lsb.Append("[Dir Llamada], \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@Group = '',  \n ");
            lsb.Append("@Order = '[Costo] Desc,[Duracion Minutos] Desc,[FecDeLlamada] Asc,[Hora Llamada] Asc,[Dir Llamada] Asc,[Punta A] Asc,[Punta B] Asc', \n ");
            lsb.Append("@OrderInv = '[Costo] Asc,[Duracion Minutos] Asc,[FecDeLlamada] Desc,[Hora Llamada] Desc,[Dir Llamada] Desc,[Punta A] Desc,[Punta B] Desc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepResumenPorConceptosTelcel(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("set @Where = '[Servicio] not like ''%IMPUESTO%''' \n ");
            lsb.Append("exec RepTabDshMovilResumenPorConceptosOpt  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[fake]=''1'',[Servicio]=UPPER([Servicio]), [Descripcion], \n ");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Mes Actual]=convert(money,SUM([Mes Actual]/[TipoCambio])), \n ");
            lsb.Append("[Mes Anterior]=convert(money,SUM([Mes Anterior]/[TipoCambio])), \n ");
            lsb.Append("[Mes Anterior 2]=convert(money,SUM([Mes Anterior 2]/[TipoCambio]))',  \n ");
            lsb.Append(" \n ");
            lsb.Append("@Group = 'UPPER([Servicio]),[Descripcion]', \n ");
            lsb.Append("@Order = '[Mes Actual] Desc,[Mes Anterior] Desc,[Mes Anterior 2] Desc,[Servicio] Asc', \n ");
            lsb.Append("@OrderInv = '[Mes Actual] Asc,[Mes Anterior] Asc,[Mes Anterior 2] Asc,[Servicio] Desc', \n ");
            //lsb.Append("@Lenght = 10, \n ");
            //lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");

            //20150108 AM. Se agregan parametros Usuario y Perfil
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");

            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español', \n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''' \n ");

            return lsb.ToString();
        }

        public string ConsultaRepResumenPorPlanes(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoTelcelPorPlanesOpt @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[fake]= ''1'', [Plan], \n ");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Total]=SUM([Total]/[TipoCambio]), \n ");
            lsb.Append("Cantidad=COUNT(*)',  \n ");
            lsb.Append("@Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("      and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Group = '[Plan]',  \n ");
            lsb.Append("@Order = '[Total] Desc,[Cantidad] Desc,[Plan] Asc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Cantidad] Asc,[Plan] Desc', \n ");
            //lsb.Append("@Lenght = 10, \n ");
            //lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepTopLineasMasCaras(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @Cantidad int \n ");
            lsb.Append("declare @DisplayStart int \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @ParamExtension varchar(max) \n ");
            lsb.Append("set @ParamEmple = 'null' \n ");
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
            lsb.Append("exec RepTabDshMovilTopLineasTelcelOptPermisos @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[Extension], \n ");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("NombreCompleto = UPPER([Nombre Completo]), \n ");
            lsb.Append("[ExtensionCod]= [Extension], \n ");
            lsb.Append("[iCodCatCarrier] = [idCarrier],");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio])',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Extension], \n ");
            lsb.Append("UPPER([Nombre Completo]), [idCarrier], \n ");
            lsb.Append("[Codigo Empleado]',  \n ");
            lsb.Append("@Order = '[Total] desc', \n ");
            lsb.Append("@OrderInv = '[Total] asc', \n ");
            if (txtCantMaxReg.Text.Length > 0)
            {
                lsb.Append("@Lenght = " + txtCantMaxReg.Text + ", \n ");
            }
            else
            {
                lsb.Append("@Lenght = 10, \n ");
            }
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

        public string ConsultaRepTopLineasMasCarasDetFact()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @NumeroMovil varchar(max) \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append(" and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("set @NumeroMovil = '''" + param["Linea"] + "''' \n ");
            }
            else
            {
                lsb.Append("set @NumeroMovil = 'null' \n ");
            }
            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoTelcelDetalleFacRest  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[Tipo Detalle], \n ");
            lsb.Append("[Ver Detalle Ruta] = [Extension], \n ");
            lsb.Append("Linea = [Extension], \n ");
            lsb.Append("[Costo]=sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n ");
            lsb.Append("[Min Libres Pico]=SUM([Min Libres Pico]), \n ");
            lsb.Append("[Min Facturables Pico]=SUM([Min Facturables Pico]), \n ");
            lsb.Append("[Min Libres No Pico]=SUM([Min Libres No Pico]), \n ");
            lsb.Append("[Min Facturables No Pico]=SUM([Min Facturables No Pico]), \n ");
            lsb.Append("[Tiempo Aire Nacional]=SUM([Tiempo Aire Nacional]/[TipoCambio]), \n ");
            lsb.Append("[Tiempo Aire Roaming Nac]=SUM([Tiempo Aire Roaming Nac]/[TipoCambio]), \n ");
            lsb.Append("[Tiempo Aire Roaming Int]=SUM([Tiempo Aire Roaming Int]/[TipoCambio]), \n ");
            lsb.Append("[Larga Distancia Nac]=SUM([Larga Distancia Nac]/[TipoCambio]), \n ");
            lsb.Append("[Larga Distancia Roam Nac]=SUM([Larga Distancia Roam Nac]/[TipoCambio]), \n ");
            lsb.Append("[Larga Distancia Roam Int]=SUM([Larga Distancia Roam Int]/[TipoCambio]), \n ");
            lsb.Append("[Servicios Adicionales]=SUM([Servicios Adicionales]/[TipoCambio]) + SUM([Renta]/[TipoCambio]), \n ");
            lsb.Append("[Serv Adic] = 7577, \n ");
            lsb.Append("[Desc Tiempo Aire]=SUM([Desc Tiempo Aire]/[TipoCambio]), \n ");
            lsb.Append("[Desc Tiempo Aire Roam]=SUM([Desc Tiempo Aire Roam]/[TipoCambio]), \n ");
            lsb.Append("[Ajustes]=SUM([Ajustes]/[TipoCambio]), \n ");
            lsb.Append("[Otros Desc]=SUM([Otros Desc]/[TipoCambio]), \n ");
            lsb.Append("[Cargos Creditos]=SUM([Cargos Creditos]/[TipoCambio]), \n ");
            lsb.Append("[Otros Serv]=SUM([Otros Serv]/[TipoCambio]), \n ");
            lsb.Append("[Otros Servicios]=7574',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@Group = '[Tipo Detalle], \n ");
            lsb.Append(" [Extension], \n ");
            lsb.Append(" [Extension]',  \n ");
            lsb.Append("@Order = '[Tipo Detalle] Asc,[Costo] Asc,[Min Libres Pico] Asc,[Min Facturables Pico] Asc,[Min Libres No Pico] Asc,[Min Facturables No Pico] Asc,[Tiempo Aire Nacional] Asc,[Tiempo Aire Roaming Nac] Asc,[Tiempo Aire Roaming Int] Asc,[Larga Distancia Nac] Asc,[Larga Distancia Roam Nac] Asc,[Larga Distancia Roam Int] Asc,[Servicios Adicionales] Asc,[Desc Tiempo Aire] Asc,[Desc Tiempo Aire Roam] Asc,[Ajustes] Asc,[Otros Desc] Asc,[Cargos Creditos] Asc,[Otros Serv] Asc,[Linea] Desc', \n ");
            lsb.Append("@OrderInv = '[Tipo Detalle] Desc,[Costo] Desc,[Min Libres Pico] Desc,[Min Facturables Pico] Desc,[Min Libres No Pico] Desc,[Min Facturables No Pico] Desc,[Tiempo Aire Nacional] Desc,[Tiempo Aire Roaming Nac] Desc,[Tiempo Aire Roaming Int] Desc,[Larga Distancia Nac] Desc,[Larga Distancia Roam Nac] Desc,[Larga Distancia Roam Int] Desc,[Servicios Adicionales] Desc,[Desc Tiempo Aire] Desc,[Desc Tiempo Aire Roam] Desc,[Ajustes] Desc,[Otros Desc] Desc,[Cargos Creditos] Desc,[Otros Serv] Desc,[Linea] Asc', \n ");
            //lsb.Append("@Lenght = 10, \n ");
            //lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepPorNumeroMarcado(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @NumeroMovil varchar(max) \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append(" and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59''  \n ");
            lsb.Append("' \n ");
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("set @NumeroMovil = '''" + param["Linea"] + "''' \n ");
            }
            else
            {
                lsb.Append("set @NumeroMovil = 'null' \n ");
            }
            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoTelcelDetalleLlamRest @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[NumMarc] = [Numero Marcado], [Numero Marcado]= '''''''' +[Numero Marcado], \n ");
            if (linkGrafica != "")
            {
                lsb.Append(linkGrafica + ", \n ");
            }
            lsb.Append("[Costo]=SUM([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Duracion Minutos]=SUM([Duracion Minutos]), \n ");
            lsb.Append("LLamadas = count(*), \n ");
            lsb.Append("TpoProm = avg([Duracion Segundos]/60), \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@Group = '[Numero Marcado], \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Order = '[Costo] Desc,[Duracion Minutos] Desc,[LLamadas] Desc,[TpoProm] Desc,[Punta B] Asc,[Numero Marcado] Asc', \n ");
            lsb.Append("@OrderInv = '[Costo] Asc,[Duracion Minutos] Asc,[LLamadas] Asc,[TpoProm] Asc,[Punta B] Desc,[Numero Marcado] Desc', \n ");
            //lsb.Append("@Lenght = 10, \n ");
            //lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepDetalleLineaNumMarc()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @NumeroMovil varchar(max) \n ");
            lsb.Append("declare @NumeroMarcado varchar(max) \n ");
            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("  and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("set @NumeroMovil = '''" + param["Linea"] + "''' \n ");
            }
            else
            {
                lsb.Append("set @NumeroMovil = 'null' \n ");
            }
            if (param["NumMarc"] != string.Empty)
            {
                lsb.Append("set @NumeroMarcado = '''" + param["NumMarc"] + "''' \n ");
            }
            else
            {
                lsb.Append("set @NumeroMarcado = 'null' \n ");
            }

            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @NumeroMarcado <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Numero Marcado] in('+@NumeroMarcado+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoTelcelTopNLineas @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[Fecha Llamada], \n ");
            lsb.Append("[Hora Llamada], \n ");
            lsb.Append("[Duracion Minutos], \n ");
            lsb.Append("[Costo]=([Costo]/[TipoCambio]), \n ");
            lsb.Append("[Punta A], \n ");
            lsb.Append("[Dir Llamada], \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@Group = '',  \n ");
            lsb.Append("@Order = '[Costo] Desc,[Duracion Minutos] Desc,[Fecha Llamada] Asc,[Hora Llamada] Asc,[Dir Llamada] Asc,[Punta A] Asc,[Punta B] Asc', \n ");
            lsb.Append("@OrderInv = '[Costo] Asc,[Duracion Minutos] Asc,[Fecha Llamada] Desc,[Hora Llamada] Desc,[Dir Llamada] Desc,[Punta A] Desc,[Punta B] Desc', \n ");
            //lsb.Append("@Lenght = 10, \n ");
            //lsb.Append("@Start = 0, \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

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
            lsb.Append("and Dash.Descripcion = 'DashboardMoviles.aspx' \n ");
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
        public string ConsultaResumenEquiposTelcel(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoTelcelPorTipoEquiposOpt  @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Tipo] = [Tipo Cel], \n ");
            lsb.Append("[Cantidad]=Count (distinct [Cuenta Hija]), \n ");
            lsb.Append("[Total]=SUM([Total]/[TipoCambio]) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + ", \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append(" and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Group = '[Tipo Cel]',  \n ");
            lsb.Append("@Order = '[Total] Desc,[Cantidad] Desc,[Tipo] Asc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Cantidad] Asc,[Tipo] Desc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
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

        public string ConsultaRepHistoricoAnioActualVsAnteriorMoviles()
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString();
            if (DSODataContext.Schema.ToString().ToLower() == "sperto")
            {
                anioActual = "2017";
                anioAnterior = "2016";
                dosAnioAtras = "2015";
            }

            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");

            /*20150224 RJ. Se agrega filtro de empleado*/
            if (param["Emple"] != string.Empty)
            {
                lsb.Append("set @ParamEmple = '" + param["Emple"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamEmple = 'null' \n ");
            }

            lsb.Append("set @Where =' [Nombre Mes] is not null '");

            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("set @Where = @Where + ' And [Emple] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");

            lsb.Append("exec ConsumoHistoricoFactTelcelAnioActualvsAnioAnterior @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Numero Mes], [Mes] = [Nombre Mes], \n ");
            lsb.Append("[" + dosAnioAtras + "]=SUM([Total Dos Anio Atras]/[TipoCambio]), \n ");   /* BG20160111.Agrego esta linea a peticion de AlfaCorporativo para que el Reporte sea de 3años */
            lsb.Append("[" + anioAnterior + "]=SUM([Total Anio Atras]/[TipoCambio]), \n ");
            lsb.Append("[" + anioActual + "]=SUM([Total Anio Actual]/[TipoCambio])',  \n ");
            ////lsb.Append("[" + anioAnterior + "]=SUM([Total Anio Anterior]/[TipoCambio]), \n ");
            ////lsb.Append("[" + anioActual + "]=SUM([Total Anio Actual]/[TipoCambio])',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Nombre Mes], \n ");
            lsb.Append("[Numero Mes]',  \n ");
            lsb.Append("@Order = 'ABS([Numero Mes]) Asc', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro(string filtro)
        {

            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160112 se agrega un 3er año hacia atras.*/

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ConsumoHistFacturasAnioActualvsAnteriorFiltroCarrier  \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Numero Mes], [Mes] = [Nombre Mes], \n ");
            lsb.Append("[" + dosAnioAtras + "]=SUM([Total Dos Anio Atras]/[TipoCambio]), \n ");   /* BG20160112.Agrego esta linea a peticion de AlfaCorporativo para que el Reporte sea de 3años */
            lsb.Append("[" + anioAnterior + "]=SUM([Total Anio Atras]/[TipoCambio]), \n ");
            lsb.Append("[" + anioActual + "]=SUM([Total Anio Actual]/[TipoCambio])',  \n ");
            lsb.Append("@Where = '[Nombre Mes] is not null \n ");

            lsb.Append(filtro);

            lsb.Append("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.Append("@Carrier = '" + param["Carrier"] + "',\n ");
            lsb.Append("@Group = '[Nombre Mes], \n ");
            lsb.Append("[Numero Mes]',  \n ");
            lsb.Append("@Order = 'ABS([Numero Mes]) Asc', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaConsumoHistPorCenCosMatAnioAnt()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE())- 1 \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("set @Where = '' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Área] = MIN(upper([Nombre Centro de Costos])), \n");
            lsb.Append("[Codigo Centro de Costos], \n");
            lsb.Append("[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden], \n");
            lsb.Append("Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup='[Codigo Centro de Costos], \n");
            lsb.Append(" upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@OuterFields='[Codigo Centro de Costos],[Área], \n");
            lsb.Append(ArmaCamposConsumoHistCenCosMatAnioAnt());
            lsb.Append("[Total] = SUM([Total])', \n");
            lsb.Append("@OuterGroup='[Área], \n");
            lsb.Append("[Codigo Centro de Costos]', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaCamposConsumoHistCenCosMatAnioAnt()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE())-1 \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("set @Where = '' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup=' upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@Order='[Orden] Asc',       \n");
            lsb.Append("@OrderInv='[Orden] Desc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ArmaCamposConsumoHistCenCosMatAnioAnt()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoHistCenCosMatAnioAnt());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Mes"] + "] = SUM(case when [Nombre Mes] = ''" + ldr["Nombre Mes"] + "'' AND [Orden] = " + ldr["Orden"] + " then [Total] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaConsumoHistPorCenCosMatAnioAct()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE()) \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("set @Where = '' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Área] = MIN(upper([Nombre Centro de Costos])), \n");
            lsb.Append("[Codigo Centro de Costos], \n");
            lsb.Append("[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden], \n");
            lsb.Append("Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup='[Codigo Centro de Costos], \n");
            lsb.Append(" upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@OuterFields='[Codigo Centro de Costos],[Área], \n");
            lsb.Append(ArmaCamposConsumoHistCenCosMatAnioAct());
            lsb.Append("[Total] = SUM([Total])', \n");
            lsb.Append("@OuterGroup='[Área], \n");
            lsb.Append("[Codigo Centro de Costos]', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaCamposConsumoHistCenCosMatAnioAct()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE()) \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");
            lsb.Append("set @Where = '' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup=' upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@Order='[Orden] Asc',       \n");
            lsb.Append("@OrderInv='[Orden] Desc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ArmaCamposConsumoHistCenCosMatAnioAct()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoHistCenCosMatAnioAct());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Mes"] + "] = SUM(case when [Nombre Mes] = ''" + ldr["Nombre Mes"] + "'' AND [Orden] = " + ldr["Orden"] + " then [Total] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaConsumoHistoricoPorEmpleadoMatAnioAnt()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE())- 1 \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior]  \n");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Completo], \n");
            lsb.Append("[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden], \n");
            lsb.Append("Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup='[Nombre Completo], \n");
            lsb.Append(" upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@OuterFields='[Empleado]=[Nombre Completo], \n");
            lsb.Append(ArmaCamposConsumoHistoricoPorEmpleadoMatAnioAnt());
            lsb.Append("[Total] = SUM([Total])', \n");
            lsb.Append("@OuterGroup='[Nombre Completo]', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaCamposConsumoHistoricoPorEmpleadoMatAnioAnt()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE())-1 \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior]  @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup=' upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@Order='[Orden] Asc',       \n");
            lsb.Append("@OrderInv='[Orden] Desc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ArmaCamposConsumoHistoricoPorEmpleadoMatAnioAnt()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoHistoricoPorEmpleadoMatAnioAnt());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Mes"] + "] = SUM(case when [Nombre Mes] = ''" + ldr["Nombre Mes"] + "'' AND [Orden] = " + ldr["Orden"] + " then [Total] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaConsumoHistoricoPorEmpleadoMatAnioAct()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE()) \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior]  \n");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Completo], \n");
            lsb.Append("[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden], \n");
            lsb.Append("Total = convert(numeric(15,2),sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]))', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup='[Nombre Completo], \n");
            lsb.Append(" upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@OuterFields='[Empleado]=[Nombre Completo], \n");
            lsb.Append(ArmaCamposConsumoHistoricoPorEmpleadoMatAnioAct());
            lsb.Append("[Total] = SUM([Total])', \n");
            lsb.Append("@OuterGroup='[Nombre Completo]', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaCamposConsumoHistoricoPorEmpleadoMatAnioAct()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @ParamSitio varchar(max) \n");
            lsb.Append("declare @fechaFin varchar(30) \n");
            lsb.Append("declare @fechaInicio varchar(30) \n");
            lsb.Append("declare @fechaInicioActual varchar(30) \n");
            lsb.Append("declare @anio int \n");

            lsb.Append("set @anio = YEAR(GETDATE()) \n");

            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");

            lsb.Append("	set @fechaInicio = CONVERT(varchar,@anio) + '-01-01 00:00:00' \n");
            lsb.Append("	set @fechaFin = CONVERT(varchar,@anio) + '-12-31 23:59:59' \n");

            lsb.Append("set @fechaInicio = '''' + @fechaInicio + '''' \n");
            lsb.Append("set @fechaFin = '''' + @fechaFin + '''' \n");

            lsb.Append("exec [ConsumoHistoricoMatricialCFMAnioAnterior]  @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@InnerFields='[Nombre Mes] = upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@InnerWhere=@Where, \n");
            lsb.Append("@InnerGroup=' upper([Mes Anio]), \n");
            lsb.Append("[Orden]', \n");
            lsb.Append("@Order='[Orden] Asc',       \n");
            lsb.Append("@OrderInv='[Orden] Desc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = @fechaInicio, \n");
            lsb.Append("@FechaFinRep = @fechaFin, \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ArmaCamposConsumoHistoricoPorEmpleadoMatAnioAct()
        {
            StringBuilder lsb = new StringBuilder();

            DataTable ldt = DSODataAccess.Execute(ConsultaCamposConsumoHistoricoPorEmpleadoMatAnioAct());

            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("[" + ldr["Nombre Mes"] + "] = SUM(case when [Nombre Mes] = ''" + ldr["Nombre Mes"] + "'' AND [Orden] = " + ldr["Orden"] + " then [Total] else 0 end),  \n");
            }

            return lsb.ToString();
        }

        public string ConsultaDetalleLlamsTelcelF4()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec spDetalleLlamadasF4  \r");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            //lsb.Append("@Linea = "+ Linea +",  \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \r");
            lsb.Append("@fechaInicio = '" + Session["FechaInicio"].ToString() + " 00:00:00',\r");
            lsb.Append("@fechaFin = '" + Session["FechaFin"].ToString() + " 23:59:59' \r");


            return lsb.ToString();
        }

        public string ConsultaRepMinutosUtilizados()
        {

            //BG.20151007 SE AGREGA VARIABLE IDUSUARIO PARA PODER UTILIZAR UTILIZAR EL REPORTE DESDE MENU INICIO
            int idUsuario = 0;

            /* BG.20151007 Se valida la variable global Emple, si tiene un valor quiere decir que navegó, 
             * y ese valor se utilizara para buscar el usuario que tiene relacionado, de lo contrario, 
             * se usara el usuario con el que esta logeado.
             */

            if (string.IsNullOrEmpty(param["Emple"]))
            {
                idUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            }
            else
            {
                DataTable dtUsuar = DSODataAccess.Execute(ObtenerRelacionEmpleadosUsuarioPerfil());

                if (dtUsuar.Rows.Count > 0)
                {
                    idUsuario = Convert.ToInt32(dtUsuar.Rows[0][1]);
                }
            }

            StringBuilder lsb = new StringBuilder();
            lsb.Append("Exec RepMinutosUsados  \n");
            //lsb.Append("Exec  zzJHRepMinutosUsados  \n");  
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Linea], \n ");
            lsb.Append("[MinsDisp], \n ");
            lsb.Append("[MinsUtil]', \n ");
            lsb.Append("@Order = '[MinsUtil] Asc', \n ");
            lsb.Append("@OrderInv = '[MinsUtil] Desc', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Usuario = " + idUsuario.ToString() + ",\n ");   //BG.20151007 SE CAMBIA POR LA NUEVA VARIABLE
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();

        }

        public string ConsultaRepHistoricoMinsUtilizados(string linkGraf)
        {
            //BG.20170222 SE AGREGA VARIABLE IDUSUARIO PARA PODER UTILIZAR EL REPORTE DESDE MENU INICIO
            int idUsuario = 0;
            //NZ 20170512 
            int idEmple = 0;

            /* BG.20170222 Se valida la variable global Emple, si tiene un valor quiere decir que navegó, 
             * y ese valor se utilizara para buscar el usuario que tiene relacionado, de lo contrario, 
             * se usara el usuario con el que esta logeado.
             */

            if (string.IsNullOrEmpty(param["Emple"]))
            {
                idUsuario = Convert.ToInt32(Session["iCodUsuario"]);
                idEmple = Convert.ToInt32(DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()));
            }
            else
            {
                idEmple = Convert.ToInt32(param["Emple"]);
                DataTable dtUsuar = DSODataAccess.Execute(ObtenerRelacionEmpleadosUsuarioPerfil());
                if (dtUsuar.Rows.Count > 0)
                {
                    idUsuario = Convert.ToInt32(dtUsuar.Rows[0][1]);
                }
            }

            //NZ 20170512 Se solicita este reporte en formato matrical de 3 años. Por lo que se cambio la consulta original.

            //Se calculan los tres años anteriores a la fecha seleccionada en el combo de la pagina.
            StringBuilder fields = new StringBuilder();
            fields.AppendLine("[ClaveMes],");
            fields.AppendLine("[Mes],");
            fields.AppendLine("[" + (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 2) + "] = SUM(CASE WHEN [Mes] = [Mes] AND Anio = " + (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 2) + " THEN MinsUtil ELSE 0 END),");
            fields.AppendLine("[" + (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1) + "] = SUM(CASE WHEN [Mes] = [Mes] AND Anio = " + (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1) + " THEN MinsUtil ELSE 0 END),");
            fields.AppendLine("[" + Convert.ToDateTime(Session["FechaInicio"].ToString()).Year + "] = SUM(CASE WHEN [Mes] = [Mes] AND Anio = " + Convert.ToDateTime(Session["FechaInicio"].ToString()).Year + " THEN MinsUtil ELSE 0 END)");

            StringBuilder lsb = new StringBuilder();
            //lsb.AppendLine("Exec zzBGRepMinutosUsadosHistorico");
            lsb.AppendLine("Exec RepMinutosUsadosHistorico");
            lsb.AppendLine("@Schema='" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Fields='" + fields.ToString() + "',");
            lsb.AppendLine("@Link = '" + linkGraf + "',");
            lsb.AppendLine("@Group = '[ClaveMes],[Mes]',");
            lsb.AppendLine("@Order = '[ClaveMes] Asc',");
            lsb.AppendLine("@OrderInv = '[ClaveMes] Desc',");
            lsb.AppendLine("@OrderDir = 'Asc',");
            lsb.AppendLine("@Usuario = " + idUsuario.ToString() + ",");
            lsb.AppendLine("@Emple = " + idEmple.ToString() + ",");
            lsb.AppendLine("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.AppendLine("@Carrier = '" + param["Carrier"] + "',\n ");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            lsb.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.AppendLine("@Idioma = 'Español'");

            return lsb.ToString();
        }

        public string ConsultaDetalleLlamsMinsUtil()
        {

            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec DetalladoMinsUtilizados \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Extension] = '''''''' + [Extension], \n ");
            lsb.Append("[Nombre Completo], \n ");
            lsb.Append("[Numero Marcado] = '''''''' + [Numero Marcado], \n ");
            lsb.Append("[Fecha Llamada], \n ");
            lsb.Append("[Hora Llamada], \n ");
            lsb.Append("[Duracion Minutos], \n ");
            lsb.Append("[Dir Llamada Codigo], \n ");
            lsb.Append("[Dir Llamada Clave], \n ");
            lsb.Append("[Punta A], \n ");
            lsb.Append("[Punta B]',  \n ");
            lsb.Append("@Group = '',  \n ");
            lsb.Append("@Order = '[Duracion Minutos] desc', \n ");
            lsb.Append("@OrderInv = '[Duracion Minutos] asc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Start = 0, \n ");
            lsb.Append("@Fecha = '" + param["MesAnio"] + "', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaCostoPorEmpleAcumulado()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("set @ParamCenCos = '" + param["CenCos"] + "' \n");
            }
            else
            {
                lsb.Append("set @ParamCenCos = 'null' \n");
            }

            lsb.Append("if @ParamCenCos <> 'null' \n");
            lsb.Append("      set @Where = '[Codigo Centro de Costos] in('+@ParamCenCos+') \n");
            lsb.Append("' \n");

            lsb.Append("exec RepTabConsumoEmpsMasCarosCFMAcumulado @Schema='" + DSODataContext.Schema + "', \n");
            //lsb.Append("exec zzbgRepTabConsumoEmpsMasCarosCFMAcumulado @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Codigo Empleado],[Nombre Completo]=Min(upper([Nombre Completo])),[Linea], \n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n");
            lsb.Append("[Renta]= sum([Renta]/[TipoCambio]), \n");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio])', \n");
            //if (linkGrafica != "")
            //{
            //    lsb.Append("," + linkGrafica + " \n ");
            //}
            //lsb.Append("',  \n");
            lsb.Append("@Where = @Where, \n");
            lsb.Append("@Group = '[Codigo Empleado],[Linea]',  \n");
            lsb.Append("@Order = '[Total] Desc', \n");
            lsb.Append("@OrderInv = '[Total] Asc', \n");
            lsb.Append("@OrderDir = 'Asc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            //lsb.Append(",@Carrier = "+((Carrier != null && Carrier.Length >0 )? Carrier : "0")+"\n");
            return lsb.ToString();
        }

        public string ConsultaCostoPorEmple()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @ParamCenCos varchar(max) \n");
            if (param["CenCos"] != string.Empty)
            {
                lsb.Append("set @ParamCenCos = '" + param["CenCos"] + "' \n");
            }
            else
            {
                lsb.Append("set @ParamCenCos = 'null' \n");
            }

            lsb.Append("if @ParamCenCos <> 'null' \n");
            lsb.Append("      set @Where = '[Codigo Centro de Costos] in('+@ParamCenCos+') \n");
            lsb.Append("' \n");

            lsb.Append("exec RepTabConsumoEmpsMasCarosCFM @Schema='" + DSODataContext.Schema + "', \n");
            //lsb.Append("exec zzbgRepTabConsumoEmpsMasCarosCFMAcumulado @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Codigo Empleado],[Nombre Completo]=Min(upper([Nombre Completo])),[Linea], \n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n");
            lsb.Append("[Renta]= sum([Renta]/[TipoCambio]), \n");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio])', \n");
            //if (linkGrafica != "")
            //{
            //    lsb.Append("," + linkGrafica + " \n ");
            //}
            //lsb.Append("',  \n");
            lsb.Append("@Where = @Where, \n");
            lsb.Append("@Group = '[Codigo Empleado],[Linea]',  \n");
            lsb.Append("@Order = '[Total] Desc', \n");
            lsb.Append("@OrderInv = '[Total] Asc', \n");
            lsb.Append("@OrderDir = 'Asc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            //lsb.Append(",@Carrier = "+((Carrier != null && Carrier.Length >0 )? Carrier : "0")+"\n");
            return lsb.ToString();
        }

        public string ConsultaDetalleFacturaRoamInternVoz()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n");
            lsb.Append("declare @NumeroMovil varchar(max) \n");
            lsb.Append("declare @ParamEmple varchar(max) \n");
            lsb.Append("declare @Empleado varchar(max)\n");

            lsb.Append("set @Where = 'FechaInicio>=''" + Session["FechaInicio"].ToString() + " 00:00:00'' \n ");
            lsb.Append("and FechaInicio<=''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");

            lsb.Append("set @NumeroMovil = 'null' \n ");

            //Se agrega parametro de linea
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("set @NumeroMovil = '" + param["Linea"] + "' \n ");
            }
            else
            {
                lsb.Append("set @NumeroMovil = 'null' \n ");
            }

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

            /* Se agrega parametro de linea */
            if (param["Linea"] != string.Empty)
            {
                lsb.Append("and vchCodigo = '" + param["Linea"] + "' \n ");
            }

            lsb.Append("end  \n ");
            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");

            /* BG.20160418 AGREGO LA VALIDACION PARA SABER SI LAS VARIABLES ParamEmple, NumeroMovil TRAEN UN VALOR. */
            lsb.Append("--------------------------------------------------------------------------------------------------------------------- \n ");
            lsb.Append("/* CUANDO NO HAYA NAVEGACIÓN ESTE REPORTE DEBE DE FUNCIONAR EN BASE AL USUARIO CON EL QUE TE LOGEAS */ \n ");
            lsb.Append("if @ParamEmple = 'Null' \n ");
            lsb.Append("begin \n ");
            lsb.Append("select @Empleado = convert(varchar,iCodCatalogo)  \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] \n ");
            lsb.Append("where Usuar = " + Session["iCodUsuario"] + " \n ");
            lsb.Append("and dtIniVigencia <= GETDATE() \n ");
            lsb.Append("and dtFinVigencia > GETDATE() \n ");
            lsb.Append("select @NumeroMovil = Tel \n ");
            lsb.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas \n ");
            lsb.Append("where dtinivigencia <> dtfinvigencia \n ");
            lsb.Append("and dtfinvigencia >= getdate() \n ");
            lsb.Append("and emple = @Empleado \n ");

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
            lsb.Append("if @NumeroMovil <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \n ");
            lsb.Append("' \n ");
            lsb.Append("end \n ");

            lsb.Append("exec [ConsumoTelcelDetalleFacRestVerticalRoamInternVoz]  @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Concepto] = Replace([Concepto],''.'',''''), \n");
            lsb.Append("[idConcepto], \n");
            lsb.Append("[Detalle], \n");
            lsb.Append("[Total],  \n");
            lsb.Append("[Ruta]',  \n");
            lsb.Append("@Where =@Where, \n");
            lsb.Append("@Group = '[Concepto], \n");
            lsb.Append("[idConcepto], \n");
            lsb.Append("[Detalle],  \n");
            lsb.Append("[Ruta]',  \n");
            lsb.Append("@Order = '[Total] Desc,[Concepto] Asc', \n");
            lsb.Append("@OrderInv = '[Total] Asc,[Concepto] Desc', \n");
            lsb.Append("@OrderDir = 'Desc', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaConsumoDesgloseRoaming()
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

            lsb.Append("exec spConsolidadoFacturasDeMovilesRestRoamingVoz @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idConcepto], [Concepto] = UPPER([Concepto]),[Descripcion], \n ");
            lsb.Append("[Telefono], \n ");
            lsb.Append("Total = sum([Total])',  \n ");
            lsb.Append("@Where =@Where, \n ");
            lsb.Append("@Group = ' UPPER([Concepto]),[Descripcion],\n ");
            lsb.Append("[idConcepto], \n ");
            lsb.Append("[Telefono]',  \n ");
            lsb.Append("@Order = '[Total] Desc,[Concepto] Asc', \n ");
            lsb.Append("@OrderInv = '[Total] Asc,[Concepto] Desc', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();

        }

        public string ConsultaDetalladoFacturaTelcel()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Exec [RepDetalladoFacturaTelcel]      ");
            sb.AppendLine("@Schema = '" + DSODataContext.Schema + "',                      ");
            sb.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            sb.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'  ");

            return sb.ToString();
        }

        private string ObtenerConsumoPorCenCosJerarquico(string linkCenCos, string linkEmple, int cenCos)
        {
            if (string.IsNullOrEmpty(isFT) || param["Nav"] == "CenCosJerarquicoN2")
            {
                isFT = "0";
            }

            StringBuilder consulta = new StringBuilder();

            consulta.Append("EXEC ObtieneConsumoReporteJerarquico @Schema='" + DSODataContext.Schema + "',\n ");
            consulta.Append("@CenCos = " + cenCos.ToString() + ",\n ");
            consulta.Append("@Usuar = " + Session["iCodUsuario"] + ",\n ");
            consulta.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            consulta.Append("@Fechainicio = '" + Session["FechaInicio"].ToString() + " 00:00:00',\n ");
            consulta.Append("@Fechafin = '" + Session["FechaFin"].ToString() + " 23:59:59',\n ");
            consulta.Append("@LinkSigCenCos = '" + linkCenCos + "',\n ");
            consulta.Append("@LinkEmpleados = '" + linkEmple + "'\n ");
            consulta.Append(", @isFT = " + isFT + " \n ");

            return consulta.ToString();
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

        public string consultaPorEmpleMasCaros(string linkGrafica, int numeroRegistros)
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
                lsb.Append("exec [RepTabConsumoEmpsMasCarosSPSinLineaDashFC] @Schema='" + DSODataContext.Schema + "',\n ");
            }
            else
            {
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

            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox" */ )
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

        public string ConsultaDetalle()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine("declare @Where varchar(max)= ''");
            lsb.AppendLine("SELECT @Where = '1 = 1' "); //Inicializa la consulta para que todos los demas filtros inicien con AND y el respectivo filtro.

            if (param["Sitio"] != string.Empty)
            {
                string sitio = FCAndControls.AgregaEtiqueta(new string[] { param["Sitio"] });
                if (sitio.ToLower().Replace(" ", "").Contains("telcel"))
                {
                    lsb.AppendLine("select @Where = @Where + ' AND ([Codigo Sitio] =''" + param["Sitio"] + "'' OR [Tipo de destino] like ''%" + sitio + "%'')'");
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

            if (param["NumMarc"] != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND [Numero Marcado] = '''''''' + ''" + param["NumMarc"] + "'''");  //NZ Se le concatena la comilla con las que lo arroja el sql.
            }

            if (WhereAdicional != string.Empty)
            {
                lsb.AppendLine("select @Where = @Where + ' AND " + WhereAdicional + "'"); //NZ Se agrega esta variable con una variable que pueda contener cualquier otro tipo de filtro que un metodo especifico ocupe.
            }

            lsb.AppendLine("exec [ConsumoDetalladoDashFC]");
            lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Fields='");
            lsb.AppendLine("[Centro de costos],");
            lsb.AppendLine("[Colaborador],");
            lsb.AppendLine("[Extensión]	 ,");
            lsb.AppendLine("[Numero Marcado],");
            lsb.AppendLine("[Fecha],");
            lsb.AppendLine("[Hora],");
            lsb.AppendLine("[Duracion],");
            lsb.AppendLine("[Llamadas],");
            lsb.AppendLine("[TotalSimulado] = (CostoFac+CostoSM),");
            lsb.AppendLine("[TotalReal] = (Costo+CostoSM),");
            lsb.AppendLine("[CostoSimulado] = (CostoFac),");
            lsb.AppendLine("[CostoReal] = (Costo),");
            lsb.AppendLine("[SM] = (CostoSM),");
            lsb.AppendLine("[Nombre Sitio]	as [Sitio],");
            lsb.AppendLine("[Nombre Carrier] as [Carrier],");
            lsb.AppendLine("[Tipo Llamada],");

            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
            {
                lsb.AppendLine("[Codigo Autorizacion]  = CASE WHEN LEN([Codigo Autorizacion]) > 0 THEN ''***'' + RIGHT([Codigo Autorizacion],3) ELSE [Codigo Autorizacion] END,");
                lsb.AppendLine("[Etiqueta],");
                lsb.AppendLine("[Tipo de destino],");
                lsb.AppendLine("[Puesto]");
            }
            else
            {
                lsb.AppendLine("[Codigo Autorizacion],");
                lsb.AppendLine("[Nombre Localidad] as [Localidad],");
                lsb.AppendLine("[Tipo de destino]");
            }

            lsb.AppendLine("',");
            lsb.AppendLine("@Where = @Where,");
            ////lsb.AppendLine("@Order = '[TotalSimulado] desc',"); SE QUITA EL ORDENAMIENTO DEL LADO DEL SQL
            ////lsb.AppendLine("@OrderDir = 'Asc',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine("@Idioma = '" + Session["Language"] + "',");
            lsb.AppendLine("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            lsb.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'");

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

        private DataTable ObtenerCamposConNombreParticular(string nombreMetodoConsulta)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.Append("SELECT MetodoConsulta, NombreOrigCampo, NombreNuevoCampo \r ");
            consulta.Append("FROM " + DSODataContext.Schema + ".CamposConNombreParticular \r ");
            consulta.Append("WHERE dtFinVigencia <> dtIniVigencia \r ");
            consulta.Append("AND dtFinVigencia >= GETDATE() \r ");
            consulta.Append("AND MetodoConsulta = '" + nombreMetodoConsulta + "'");

            return DSODataAccess.Execute(consulta.ToString());
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

        public string ConsultaCostoPorAreaDireccion()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec [RepTabCostoPorAreaDirecciones] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[idDireccion],[DescDireccion],[Importe],[Renta],[MontoExc],[Link]', \n");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29' \n");
            lsb.Append(",@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + "");
            return lsb.ToString();
        }

        public string ConsultaCostoPorAreaSubDireccion(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec [RepTabCostoPorAreaSubDirecciones] @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[idSubDireccion],[DescSubDireccion],[Importe],[Renta],[MontoExc] \n");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n");

            if (param["CenCos"] != "")
            {
                lsb.Append("@CenCos = " + param["CenCos"] + ",\n ");
            }

            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29' \n");
            return lsb.ToString();
        }

        public string ConsultaConsumoEmpleFiltroCencos(string linkGrafica)
        {

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoEmpleadoFiltroCencos @Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Codigo Empleado],[Nombre Completo]=Min(upper([Nombre Completo])),[Linea], \n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n");
            lsb.Append("[Renta]= sum([Renta]/[TipoCambio]), \n");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio])', \n");
            lsb.Append("@Group = '[Codigo Empleado],[Linea]',  \n");
            lsb.Append("@Order = '[Total] Desc', \n");
            lsb.Append("@OrderInv = '[Total] Asc', \n");
            lsb.Append("@OrderDir = 'Asc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        public string ConsultaRepHistAnioActualVsAnteriorMovFiltroCC()
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString();

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ConsumoHistoricoFactTelcelAnioActualvsAnioAnteriorDirecciones @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Numero Mes], [Mes] = [Nombre Mes], \n ");
            lsb.Append("[" + dosAnioAtras + "]=SUM([Total Dos Anio Atras]/[TipoCambio]), \n ");
            lsb.Append("[" + anioAnterior + "]=SUM([Total Anio Atras]/[TipoCambio]), \n ");
            lsb.Append("[" + anioActual + "]=SUM([Total Anio Actual]/[TipoCambio])',  \n ");
            lsb.Append("@Where = 'and [Nombre Mes] is not null', \n ");
            lsb.Append("@Group = '[Nombre Mes], \n ");
            lsb.Append("[Numero Mes]',  \n ");
            lsb.Append("@Order = 'ABS([Numero Mes]) Asc', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }
        

        public string ConsultaRepConsumoPorCtaMaestra(string linkGrafica)
        {

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoPorCuentaMae @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idCuentaPadre], [CuentaPadre], [Total] = SUM([Importe]) \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n");
            lsb.Append("@Group = '[idCuentaPadre], [CuentaPadre]', \n ");
            lsb.Append("@Order = '[idCuentaPadre]', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepConsumoLineaTelcelCtaMaestra()
        {
            

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoPorLineaTelcelCtaMae @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idCuentaPadre], [CuentaPadre], [idLinea], [Linea], \n ");
            lsb.Append("[Nomina], [Empleado], [CenCos], [Centro de Costo], \n ");
            lsb.Append("[NoEmpresa], [Empresa], sum(Importe) as Importe', \n ");
            lsb.Append("@Order = '[idCuentaPadre]', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29', \n");
            lsb.Append("@CtaMae = '" + param["CtaMae"] + "',\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español', \n ");
            lsb.Append("@Group = '[idCuentaPadre], [CuentaPadre],[CenCos], [Centro de Costo], [NoEmpresa], [Empresa], [Empleado], [Nomina], [idLinea], [Linea]'\n ");
            return lsb.ToString();


        }

        public string ConsultaRepTabConsumoMovilesPorTecnologia(string linkGrafica)
        {

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoMovilesPorTecnologia  \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idTecnologia],[Tecnologia],[Total],[Renta],[Excedente] \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n");
            lsb.Append("@Order = '[Total]', \n ");
            lsb.Append("@OrderDir = 'Desc', \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepTabConsumoPorCarrierFiltroCC(string linkGrafica)
        {

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoPorCarrierFiltroCC  \n");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Codigo Carrier],[Descripcion], \n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n");
            lsb.Append("[Renta]= sum([Renta]/[TipoCambio]), \n");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio]) \n");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n");
            lsb.Append("@Group = '[Codigo Carrier],[Descripcion]',  \n");
            lsb.Append("@Order = '[Total] Desc', \n");
            lsb.Append("@OrderInv = '[Total] Asc', \n");
            lsb.Append("@OrderDir = 'Asc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            //if (lsCvePerfil != "Admin")
            //{
            lsb.Append("@CenCos = '" + param["CenCos"] + "',\n ");
            //}
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();

        }

        public string ConsultaConsumoEmpleFiltroCarrier(string linkGrafica)
        {

            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoEmpleadoFiltroCencos \n");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Codigo Empleado],[Nombre Completo]=Min(upper([Nombre Completo])),[Linea], \n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n");
            lsb.Append("[Renta]= sum([Renta]/[TipoCambio]), \n");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio]), \n");
            lsb.Append("[Codigo Carrier], [Nombre Carrier]', \n");
            lsb.Append("@Group = '[Codigo Empleado],[Linea], [Codigo Carrier], [Nombre Carrier] ',  \n");
            lsb.Append("@Order = '[Total] Desc', \n");
            lsb.Append("@OrderInv = '[Total] Asc', \n");
            lsb.Append("@OrderDir = 'Asc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.Append("@Carrier = '" + param["Carrier"] + "',\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");

            return lsb.ToString();
        }

        public string ConsultaRepHistAnioActualVsAnteriorFiltroCarrier()
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString();


            StringBuilder lsb = new StringBuilder();
            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @ParamTel varchar(max) \n ");


            /*20150224 RJ. Se agrega filtro de empleado*/
            if (param["Emple"] != string.Empty)
            {
                lsb.Append("set @ParamEmple = '" + param["Emple"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamEmple = 'null' \n ");
            }
            /*20180209 BG. Se agrega filtro de Telefono*/
            if (param["Tel"] != string.Empty)
            {
                lsb.Append("set @ParamTel = '" + param["Tel"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamTel = 'null' \n ");
            }


            lsb.Append("set @Where =' [Nombre Mes] is not null '");

            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("set @Where = @Where + ' And [Emple] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");

            lsb.Append("if @ParamTel <> 'null' \n ");
            lsb.Append("set @Where = @Where + ' And [Telefono] in('+@ParamTel+') \n ");
            lsb.Append("' \n ");

            lsb.Append("exec ConsumoHistFacturasAnioActualvsAnteriorFiltroCarrier \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[Numero Mes], [Mes] = [Nombre Mes], \n ");
            lsb.Append("[" + dosAnioAtras + "]=SUM([Total Dos Anio Atras]/[TipoCambio]), \n ");
            lsb.Append("[" + anioAnterior + "]=SUM([Total Anio Atras]/[TipoCambio]), \n ");
            lsb.Append("[" + anioActual + "]=SUM([Total Anio Actual]/[TipoCambio])',  \n ");
            lsb.Append("@Where = @Where, \n ");
            lsb.Append("@Group = '[Nombre Mes], \n ");
            lsb.Append("[Numero Mes]',  \n ");
            lsb.Append("@Order = 'ABS([Numero Mes]) Asc', \n ");
            lsb.Append("@OrderDir = 'Asc', \n ");
            lsb.Append("@Cencos = '" + param["CenCos"] + "',\n ");
            lsb.Append("@Carrier = '" + param["Carrier"] + "',\n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        public string ConsultaConsumoGlobalTelefoniaMovil()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec RepTabConsumoGlobalTelefoniaMovil \n");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsb.Append("@Fields='[Direccion] = [Nombre Direccion], \n");
            lsb.Append("[Subdireccion] = [Nombre Centro de Costos], \n");
            lsb.Append("[Codigo Empleado],[Nombre Completo]=Min(upper([Nombre Completo])), \n");
            lsb.Append("[Nombre Carrier],[Linea], \n");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n");
            lsb.Append("[Renta]= sum([Renta]/[TipoCambio]), \n");
            lsb.Append("[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio])', \n");
            lsb.Append("@Group = '[Codigo Empleado],[Linea],[Nombre Carrier], [Nombre Direccion], [Nombre Centro de Costos]',  \n");
            lsb.Append("@Order = '[Direccion] Asc, [Linea] Asc, [Total] Desc', \n");
            lsb.Append("@OrderInv = '[Direccion] Desc, [Linea] Desc, [Total] Asc', \n");
            lsb.Append("@OrderDir = 'Asc', \n");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29', \n");
            lsb.Append("@Moneda = '" + Session["Currency"] + "',\n ");
            lsb.Append("@Idioma = 'Español' \n");
            return lsb.ToString();
        }

        #region Reportes para demo de consumo de datos moviles
        public string DemoConsultaConsumoPorConecepto()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("EXEC DemoObtieneConsumoPorConcepto ");
            lsb.Append("@esquema='" + DSODataContext.Schema + "', \n");
            lsb.Append(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29' \n");

            return lsb.ToString();
        }


        public string DemoConsultaConsumoPorConeceptoUnaLinea()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("EXEC DemoObtieneConsumoPorConceptoUnaLinea ");
            lsb.Append("@esquema='" + DSODataContext.Schema + "', \n");
            lsb.Append(" @linea = " + param["Linea"] + ", ");
            lsb.Append(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29' \n");

            return lsb.ToString();
        }

        public string DemoConsultaObtieneConsumoAcumPorDiaPorConcepto()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("EXEC DemoObtieneConsumoAcumPorDiaPorConcepto ");
            lsb.Append("@esquema='" + DSODataContext.Schema + "', \n");
            lsb.Append(" @linea = " + param["Linea"] + ", ");
            lsb.Append(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsb.Append(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:29' \n");

            return lsb.ToString();
        }

        public string TopLineasMasConsumoDatos(string link)
        {
            string fechaIni = Session["FechaInicio"].ToString() + " 00:00:00";
            string fechaFin = Session["FechaFin"].ToString() + " 23:59:29";
            string spLineas = "exec DemoTopLineasMasConsumoDatos @esquema='{3}', @fechaIniRep = '{0}', @fechaFinRep = '{1}',@Link='{2}'";

            string query = string.Format(spLineas, fechaIni, fechaFin, link, DSODataContext.Schema);
            return query;
        }
        #endregion Reportes para demo de consumo de datos moviles


        public string ConsultaLineasNoIdent()
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("Exec IndicadorCantLineasNoIdent");
            query.AppendLine("	@schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("	@usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("	@perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("  @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            query.AppendLine("  @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59' ");

            return query.ToString();
        }

        public string ConsultaConsumoLineasNoIdent(string link)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamCenCos varchar(max)");
            query.AppendLine("declare @ParamEqCelular varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamCenCos = 'null'");
            query.AppendLine("set @ParamEqCelular = 'null'");
            query.AppendLine("set @Where = '");
            query.AppendLine("FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''");
            query.AppendLine("and FechaInicio < ''" + Session["FechaFin"].ToString() + " 23:59:29''");
            query.AppendLine("'");
            query.AppendLine("if @ParamCenCos <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamEqCelular <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [idEquipo] in('+@ParamEqCelular+')");
            query.AppendLine("'");
            query.AppendLine("exec ConsumoLineasNoIdent ");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields=");
            query.AppendLine("'");
            query.AppendLine("	[Linea],");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("	[Renta]= sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[iCodCatCarrier] = [Codigo Carrier],");
            query.AppendLine("	[link] = ''" + link + "''");
            query.AppendLine("', ");
            query.AppendLine("@Where = @Where, ");
            query.AppendLine("@Group = '[Linea], [Codigo Carrier]', ");
            query.AppendLine("@Order = '[Total] Desc',");
            query.AppendLine("@OrderInv = '[Total] Asc',");
            query.AppendLine("@OrderDir = 'Asc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 00:00:00''',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@Carrier = 373  ");

            return query.ToString();
        }

        public string ConsultaConsumoPorConcepto1Linea1Pnl(string tel, string carrier)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("declare @Where varchar(max)  = ''");
            query.AppendLine("declare @NumeroMovil varchar(max)");
            query.AppendLine("declare @carrier int");
            query.AppendLine("declare @ParamEmple varchar(max) ");
            query.AppendLine("declare @ParamTel varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamTel = '" + tel + "'");
            query.AppendLine("Set @carrier = " + carrier + "");
            query.AppendLine("declare @fechaini varchar(10) ");
            query.AppendLine("set @fechaini = CONVERT(varchar(10), '" + Session["FechaInicio"].ToString() + " 00:00:00',120) ");
            query.AppendLine("");
            query.AppendLine("declare @fechafin varchar(10) ");
            query.AppendLine("set @fechafin = CONVERT(varchar(10),'" + Session["FechaFin"].ToString() + " 00:00:00',120) ");
            query.AppendLine("");
            query.AppendLine(" Select  @NumeroMovil =Tel");
            query.AppendLine(" From [" + DSODataContext.Schema + "].[VisHistoricos('Linea','Lineas','Español')] lineas");
            query.AppendLine(" where dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" And dtFinVigencia >= GETDATE()");
            query.AppendLine(" And Carrier = @carrier");
            query.AppendLine(" And Tel = @ParamTel");
            query.AppendLine("");
            query.AppendLine(" set @Where = 'FechaInicio between  '''+@fechaini+''' and '''+@fechafin+''' '");
            query.AppendLine(" ");
            query.AppendLine(" if(len(@NumeroMovil) > 0 )");
            query.AppendLine(" begin");
            query.AppendLine(" Set @Where = @Where +' And [Extension] in('''+@NumeroMovil+''') '");
            query.AppendLine(" end ");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("exec [ConsumoTelcelDetalleFacRestVerticalConsolidado] ");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "', ");
            query.AppendLine("@Fields='[Concepto] = Replace([Concepto],''.'',''''), ");
            query.AppendLine("[idConcepto], ");
            query.AppendLine("[Detalle], ");
            query.AppendLine("[Total],");
            query.AppendLine("[Carrier],");
            query.AppendLine("[DescCarrier]',  ");
            query.AppendLine("@Cencos = '',");
            query.AppendLine("@Carrier = @carrier,");
            query.AppendLine("@Where =@Where, ");
            query.AppendLine("@Group = '[Concepto], ");
            query.AppendLine("[idConcepto], ");
            query.AppendLine("[Detalle],");
            query.AppendLine("[Carrier],");
            query.AppendLine("[DescCarrier]',  ");
            query.AppendLine("@Order = '[Total] Desc,[Concepto] Asc', ");
            query.AppendLine("@OrderInv = '[Total] Asc,[Concepto] Desc', ");
            query.AppendLine("@OrderDir = 'Desc', ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español' ");


            return query.ToString();
        }

        public string ConsultaCantLineasNuevas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC IndicadorCantLineasNuevas");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "', ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59' ");
            return query.ToString();
        }

        public string ConsultaConsumoCantLineasNuevas(string link)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamCenCos varchar(max)");
            query.AppendLine("declare @ParamEqCelular varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamCenCos = 'null'");
            query.AppendLine("set @ParamEqCelular = 'null'");
            query.AppendLine("set @Where = '");
            //query.AppendLine("FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''");
            //query.AppendLine("and FechaInicio < ''" + Session["FechaFin"].ToString() + " 23:59:29''");
            query.AppendLine("'");
            query.AppendLine("if @ParamCenCos <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamEqCelular <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [idEquipo] in('+@ParamEqCelular+')");
            query.AppendLine("'");
            query.AppendLine("exec ConsumoCantLineasNuevas ");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields=");
            query.AppendLine("'");
            query.AppendLine("	[Linea],");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("	[Renta]= sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[iCodCatCarrier] = [Codigo Carrier],");
            query.AppendLine("	[link] = ''" + link + "''");
            query.AppendLine("', ");
            query.AppendLine("@Where = @Where, ");
            query.AppendLine("@Group = '[Linea], [Codigo Carrier]', ");
            query.AppendLine("@Order = '[Total] Desc',");
            query.AppendLine("@OrderInv = '[Total] Asc',");
            query.AppendLine("@OrderDir = 'Asc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 00:00:00',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@Carrier = 373  ");
            return query.ToString();
        }

        public string ConsultaConsumoPorConcepto1LineaNueva1Pnl(string tel, string carrier)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("declare @Where varchar(max)  = ''");
            query.AppendLine("declare @NumeroMovil varchar(max)");
            query.AppendLine("declare @carrier int");
            query.AppendLine("declare @ParamEmple varchar(max) ");
            query.AppendLine("declare @ParamTel varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamTel = '" + tel + "'");
            query.AppendLine("Set @carrier = " + carrier + "");
            query.AppendLine("declare @fechaini varchar(10) ");
            query.AppendLine("set @fechaini = CONVERT(varchar(10), '" + Session["FechaInicio"].ToString() + " 00:00:00',120) ");
            query.AppendLine("");
            query.AppendLine("declare @fechafin varchar(10) ");
            query.AppendLine("set @fechafin = CONVERT(varchar(10),'" + Session["FechaFin"].ToString() + " 00:00:00',120) ");
            query.AppendLine("");
            query.AppendLine(" DECLARE");
            query.AppendLine(" @FechaIniRep1 varchar(10),");
            query.AppendLine(" @FechaFinRep1 varchar(10)");
            query.AppendLine(" SET @FechaFinRep1 = CONVERT(VARCHAR(10), DATEADD(dd, -(DAY(@fechaini)), @fechaini), 121)");
            query.AppendLine(" SET @FechaIniRep1 = CONVERT(VARCHAR(10), DATEADD(month, DATEDIFF(month, 0, DATEADD(month, -1, @fechaini)), 0), 121)");
            query.AppendLine(" Select  @NumeroMovil =Tel");
            query.AppendLine(" From [" + DSODataContext.Schema + "].[VisHistoricos('Linea','Lineas','Español')] lineas");
            query.AppendLine(" where dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" And dtFinVigencia >= GETDATE()");
            query.AppendLine(" And Carrier = @carrier");
            query.AppendLine(" And Tel = @ParamTel");
            query.AppendLine("");
            query.AppendLine(" set @Where = 'FechaInicio between  '''+@FechaIniRep1+''' and '''+@FechaFinRep1+''' '");
            query.AppendLine(" ");
            query.AppendLine(" if(len(@NumeroMovil) > 0 )");
            query.AppendLine(" begin");
            query.AppendLine(" Set @Where = @Where +' And [Extension] in('''+@NumeroMovil+''') '");
            query.AppendLine(" end ");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("exec [ConsumoTelcelDetalleFacRestVerticalConsolidado] ");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "', ");
            query.AppendLine("@Fields='[Concepto] = Replace([Concepto],''.'',''''), ");
            query.AppendLine("[idConcepto], ");
            query.AppendLine("[Detalle], ");
            query.AppendLine("[Total],");
            query.AppendLine("[Carrier],");
            query.AppendLine("[DescCarrier]',  ");
            query.AppendLine("@Cencos = '',");
            query.AppendLine("@Carrier = @carrier,");
            query.AppendLine("@Where =@Where, ");
            query.AppendLine("@Group = '[Concepto], ");
            query.AppendLine("[idConcepto], ");
            query.AppendLine("[Detalle],");
            query.AppendLine("[Carrier],");
            query.AppendLine("[DescCarrier]',  ");
            query.AppendLine("@Order = '[Total] Desc,[Concepto] Asc', ");
            query.AppendLine("@OrderInv = '[Total] Asc,[Concepto] Desc', ");
            query.AppendLine("@OrderDir = 'Desc', ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español' ");


            return query.ToString();
        }

        public string ConsultaLineasSinAc()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC IndicadorLineasSinActividad");
            query.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "', ");
            query.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            query.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59' ");
            return query.ToString();
        }
        public string ConsultaConsumoLineasSinAct()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamCenCos varchar(max)");
            query.AppendLine("declare @ParamEqCelular varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamCenCos = 'null'");
            query.AppendLine("set @ParamEqCelular = 'null'");
            query.AppendLine("set @Where = '");
            query.AppendLine("'");
            query.AppendLine("if @ParamCenCos <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamEqCelular <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [idEquipo] in('+@ParamEqCelular+')");
            query.AppendLine("'");
            query.AppendLine("exec [ConsumoLineasSinActividad] ");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields=");
            query.AppendLine("'");
            query.AppendLine("	[Linea],");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("	[Renta]= sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[iCodCatCarrier] = [Codigo Carrier]");
            query.AppendLine("', ");
            query.AppendLine("@Where = @Where, ");
            query.AppendLine("@Group = '[Linea], [Codigo Carrier]', ");
            query.AppendLine("@Order = '[Total] Desc',");
            query.AppendLine("@OrderInv = '[Total] Asc',");
            query.AppendLine("@OrderDir = 'Asc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 00:00:00',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@Carrier = 373  ");
            return query.ToString();
        }

        //RM20180828 
        public string ConsultaCantidadLineasEnelMes()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("Exec IndicadorCantidadLineas");
            query.AppendLine("    @schema		=	'" + DSODataContext.Schema + "',");
            query.AppendLine("    @usuario	    =	" + Session["iCodUsuario"] + ",");
            query.AppendLine("    @perfil		=	" + Session["iCodPerfil"] + ",");
            query.AppendLine("    @FechaIniRep  = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("    @FechaFinRep  = '" + Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }

        public string ConsultaConsumoLineasEnElMes(string link)
        {
            string fechaIniRep = Session["FechaInicio"].ToString() + " 00:00:00";
            string fechaFinRep = Session["FechaFin"].ToString() + " 23:59:29";
            DateTime dateFechaIniRep = new DateTime(1900, 1, 1);
            DateTime dateFechaFinRep = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(fechaIniRep, out dateFechaIniRep))
            {
                dateFechaIniRep = dateFechaIniRep.AddMonths(-1);
            }

            if (DateTime.TryParse(fechaFinRep, out dateFechaFinRep))
            {
                dateFechaFinRep = dateFechaFinRep.AddMonths(-1);
            }

            if (dateFechaIniRep > new DateTime(1900, 1, 1) && dateFechaFinRep > new DateTime(1900, 1, 1))
            {
                fechaIniRep = dateFechaIniRep.ToString("yyyy-MM-dd 00:00:00");
                fechaFinRep = dateFechaFinRep.ToString("yyyy-MM-dd 23:59:29");
            }


            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("declare @Where varchar(max)");
            query.AppendLine("declare @ParamCenCos varchar(max)");
            query.AppendLine("declare @ParamEqCelular varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamCenCos = 'null'");
            query.AppendLine("set @ParamEqCelular = 'null'");
            query.AppendLine("set @Where = '");
            query.AppendLine("FechaInicio >= ''" + fechaIniRep + "''");
            query.AppendLine("and FechaInicio < ''" + fechaFinRep + "''");
            query.AppendLine("'");
            query.AppendLine("if @ParamCenCos <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')");
            query.AppendLine("'");
            query.AppendLine("if @ParamEqCelular <> 'null'");
            query.AppendLine("set @Where = @Where + 'And [idEquipo] in('+@ParamEqCelular+')");
            query.AppendLine("'");
            query.AppendLine("exec [ConsumoLineasEnElMes] ");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("@Fields=");
            query.AppendLine("'");
            query.AppendLine("	[Linea],");
            query.AppendLine("	[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]),");
            query.AppendLine("	[Renta]= sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[Diferencia] = (sum([Costo]/[TipoCambio]) + sum([CostoSM]/[TipoCambio])) - sum([Renta]/[TipoCambio]),");
            query.AppendLine("	[iCodCatCarrier] = [Codigo Carrier],");
            query.AppendLine("	[link] = ''" + link + "''");
            query.AppendLine("', ");
            query.AppendLine("@Where = @Where, ");
            query.AppendLine("@Group = '[Linea], [Codigo Carrier]', ");
            query.AppendLine("@Order = '[Total] Desc',");
            query.AppendLine("@OrderInv = '[Total] Asc',");
            query.AppendLine("@OrderDir = 'Asc',");
            query.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + fechaIniRep + "''',");
            query.AppendLine("@FechaFinRep = '''" + fechaFinRep + "''',");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español',  ");
            query.AppendLine("@Carrier = 373  ");
            query.AppendLine("");


            return query.ToString();
        }


        public string ConsultaConsumoPorConcepto1Linea1PnlMesAnterior(string tel, string carrier)
        {


            string fechaIniRep = Session["FechaInicio"].ToString() + " 00:00:00";
            string fechaFinRep = Session["FechaFin"].ToString() + " 23:59:29";
            DateTime dateFechaIniRep = new DateTime(1900, 1, 1);
            DateTime dateFechaFinRep = new DateTime(1900, 1, 1);

            if (DateTime.TryParse(fechaIniRep, out dateFechaIniRep))
            {
                dateFechaIniRep = dateFechaIniRep.AddMonths(-1);
            }

            if (DateTime.TryParse(fechaFinRep, out dateFechaFinRep))
            {
                dateFechaFinRep = dateFechaFinRep.AddMonths(-1);
            }

            if (dateFechaIniRep > new DateTime(1900, 1, 1) && dateFechaFinRep > new DateTime(1900, 1, 1))
            {
                fechaIniRep = dateFechaIniRep.ToString("yyyy-MM-dd 00:00:00");
                fechaFinRep = dateFechaFinRep.ToString("yyyy-MM-dd 23:59:29");
            }
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("declare @Where varchar(max)  = ''");
            query.AppendLine("declare @NumeroMovil varchar(max)");
            query.AppendLine("declare @carrier int");
            query.AppendLine("declare @ParamEmple varchar(max) ");
            query.AppendLine("declare @ParamTel varchar(max)");
            query.AppendLine("");
            query.AppendLine("set @ParamTel = '" + tel + "'");
            query.AppendLine("Set @carrier = " + carrier + "");
            query.AppendLine("declare @fechaini varchar(10) ");
            query.AppendLine("set @fechaini = CONVERT(varchar(10), '" + fechaIniRep + " 00:00:00',120) ");
            query.AppendLine("");
            query.AppendLine("declare @fechafin varchar(10) ");
            query.AppendLine("set @fechafin = CONVERT(varchar(10),'" + fechaFinRep + " 00:00:00',120) ");
            query.AppendLine("");
            query.AppendLine(" Select  @NumeroMovil =Tel");
            query.AppendLine(" From [" + DSODataContext.Schema + "].[VisHistoricos('Linea','Lineas','Español')] lineas");
            query.AppendLine(" where dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" And dtFinVigencia >= GETDATE()");
            query.AppendLine(" And Carrier = @carrier");
            query.AppendLine(" And Tel = @ParamTel");
            query.AppendLine("");
            query.AppendLine(" set @Where = 'FechaInicio between  '''+@fechaini+''' and '''+@fechafin+''' '");
            query.AppendLine(" ");
            query.AppendLine(" if(len(@NumeroMovil) > 0 )");
            query.AppendLine(" begin");
            query.AppendLine(" Set @Where = @Where +' And [Extension] in('''+@NumeroMovil+''') '");
            query.AppendLine(" end ");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("exec [ConsumoTelcelDetalleFacRestVerticalConsolidado] ");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "', ");
            query.AppendLine("@Fields='[Concepto] = Replace([Concepto],''.'',''''), ");
            query.AppendLine("[idConcepto], ");
            query.AppendLine("[Detalle], ");
            query.AppendLine("[Total],");
            query.AppendLine("[Carrier],");
            query.AppendLine("[DescCarrier]',  ");
            query.AppendLine("@Cencos = '',");
            query.AppendLine("@Carrier = @carrier,");
            query.AppendLine("@Where =@Where, ");
            query.AppendLine("@Group = '[Concepto], ");
            query.AppendLine("[idConcepto], ");
            query.AppendLine("[Detalle],");
            query.AppendLine("[Carrier],");
            query.AppendLine("[DescCarrier]',  ");
            query.AppendLine("@Order = '[Total] Desc,[Concepto] Asc', ");
            query.AppendLine("@OrderInv = '[Total] Asc,[Concepto] Desc', ");
            query.AppendLine("@OrderDir = 'Desc', ");
            query.AppendLine("@Moneda = '" + Session["Currency"] + "',");
            query.AppendLine("@Idioma = 'Español' ");


            return query.ToString();
        }

        public string ConsultaConsumoDatosporDiaConcepto()
        {
            StringBuilder query = new StringBuilder();

            return query.ToString();
        }
        public string ConsultaConsumoDatosNacional(string link)
        {
            StringBuilder query = new StringBuilder();

            return query.ToString();
        }

        public string GetConsumoHistAcumPorDireccionUnAño(int año)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ConsumoHistAcumPorDireccionUnAño]");
            query.AppendLine("  @Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("  @añoHistoria = " + año + ",");
            query.AppendLine("  @Usuario = " + Session["iCodUsuario"]);
            return query.ToString();
        }

        public string GetMinLineas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("DECLARE @Where varchar(max)");
            query.AppendLine("SET @Where = '[Minutos Usados] <= " + (txtCantMaxReg.Text.Length > 0 ? txtCantMaxReg.Text : "100") + "'");
            query.AppendLine("EXEC [zz_NZ_ConsumoTelcelBajoConsumoMinRest] @Schema='" + DSODataContext.Schema + "',");
            query.AppendLine("  @Fields='[Línea],");
            query.AppendLine("      [Nombre Carrier],");
            query.AppendLine("      [Nombre Completo],");
            query.AppendLine("      [Nombre Centro de Costos],");
            query.AppendLine("      [Minutos Usados]=SUM([Minutos Usados])', ");
            query.AppendLine("   @Where = @Where,");
            query.AppendLine("   @Group = '[Línea],");
            query.AppendLine("      [Nombre Carrier],");
            query.AppendLine("      [Nombre Completo],");
            query.AppendLine("      [Nombre Centro de Costos],");
            query.AppendLine("      [FechaInicio]', ");
            query.AppendLine("   @Order = '[Minutos Usados] Desc,[Línea] Desc,[Nombre Completo] Desc,[Nombre Centro de Costos] Desc,[Nombre Carrier] Asc',");
            query.AppendLine("   @OrderInv = '[Minutos Usados] Asc,[Línea] Asc,[Nombre Completo] Asc,[Nombre Centro de Costos] Asc,[Nombre Carrier] Desc',");
            query.AppendLine("   @Lenght = 99999,");
            query.AppendLine("   @Start = 0,");
            query.AppendLine("   @OrderDir = 'Desc',");
            query.AppendLine("   @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("   @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("   @FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("   @FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("@Moneda = '" + HttpContext.Current.Session["Currency"] + "',");
            query.AppendLine("@Idioma = '" + HttpContext.Current.Session["Language"] + "'");
            return query.ToString();
        }


        public string ConsultaRepMatConsumoAcumHistPorSubdireccion(int año)
        {
            StringBuilder query = new StringBuilder();


            query.AppendLine("EXEC [ConsumoHistAcumPorSubDireccionUnAño] ");
            query.AppendLine("	@Esquema ='" + DSODataContext.Schema + "',");
            query.AppendLine("	@añoHistoria =" + año + ",");
            query.AppendLine("	@Usuario =" + Session["iCodUsuario"]);

            return query.ToString();
        }

        public string GetMinLineas3Meses()
        {
            StringBuilder query = new StringBuilder();
            // Este SP tiene la logica para generar los ultimos 3 meses (Consumo)
            query.AppendLine("declare @dtIniRep varchar(max) = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'																												");
            query.AppendLine("declare @dtFinRep varchar(max) = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'																												");
            query.AppendLine("																																										");
            query.AppendLine("declare @nomColMesActual			varchar(100) = ''																													");
            query.AppendLine("declare @nomColMesActualMenosUno	varchar(100) = ''																													");
            query.AppendLine("declare @nomColMesActualMenosDos	varchar(100) = ''																													");
            query.AppendLine("declare @campos varchar(max) =''																																		");
            query.AppendLine("declare @orderBy varchar(max) =''																																	");
            query.AppendLine("declare @orderByinv varchar(max) = ''																																");
            query.AppendLine("																																										");
            query.AppendLine("																																										");
            query.AppendLine("Select @nomColMesActual = Español + ' '+Convert(varchar,DATEPART(YEAR,Convert(date,@dtIniRep)) )																");
            query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Mes','Meses','Español')] mes																									");
            query.AppendLine("Where dtIniVigencia <> dtFinVigencia																																	");
            query.AppendLine("and dtFinVigencia >= GETDATE()																																		");
            query.AppendLine("And vchCodigo = DATEPART(MONTH,Convert(date,@dtIniRep)) 																												");
            query.AppendLine("																																										");
            query.AppendLine("Select @nomColMesActualMenosUno = Español + ' '+Convert(varchar,DATEPART(YEAR,DATEADD(Month,-1, Convert(date,@dtIniRep))) )									");
            query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Mes','Meses','Español')] mes																									");
            query.AppendLine("Where dtIniVigencia <> dtFinVigencia																																	");
            query.AppendLine("and dtFinVigencia >= GETDATE()																																		");
            query.AppendLine("And vchCodigo = DATEPART(MONTH,DATEADD(Month,-1, Convert(date,@dtIniRep))) 																							");
            query.AppendLine("																																										");
            query.AppendLine("Select @nomColMesActualMenosDos = Español + ' '+Convert(varchar,DATEPART(YEAR,DATEADD(Month,-2, Convert(date,@dtIniRep))) )									");
            query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Mes','Meses','Español')] mes																									");
            query.AppendLine("Where dtIniVigencia <> dtFinVigencia																																	");
            query.AppendLine("and dtFinVigencia >= GETDATE()																																		");
            query.AppendLine("And vchCodigo = DATEPART(MONTH,DATEADD(Month,-2, Convert(date,@dtIniRep))) 																							");
            query.AppendLine("																																										");
            query.AppendLine("																																										");
            query.AppendLine("Select @campos =																																						");
            query.AppendLine("'																																									");
            query.AppendLine("	[Línea],																																							");
            query.AppendLine("	[Nombre Carrier],																																					");
            query.AppendLine("	[Nombre Completo],																																					");
            query.AppendLine("	[Nombre Centro de Costos],																																			");
            query.AppendLine("	['+@nomColMesActualMenosDos+']=SUM([Minutos Usados Mes Menos Dos]),																									");
            query.AppendLine("	['+@nomColMesActualMenosUno+']=SUM([Minutos Usados Mes Menos Uno]),																									");
            query.AppendLine("	['+@nomColMesActual+']=SUM([Minutos Usados])																														");
            query.AppendLine("'");
            query.AppendLine("																																										");
            query.AppendLine("select @orderBy = 																																					");
            query.AppendLine(" '['+@nomColMesActual+'] Desc,[Línea] Desc,[Nombre Completo] Desc,[Nombre Centro de Costos] Desc,[Nombre Carrier] Asc'												");
            query.AppendLine("																																										");
            query.AppendLine("																																										");
            query.AppendLine(" Select @orderByInv =																																				");
            query.AppendLine(" '['+@nomColMesActual+'] Asc,[Línea] Asc,[Nombre Completo] Asc,[Nombre Centro de Costos] Asc,[Nombre Carrier] Desc'													");
            query.AppendLine("																																										");
            query.AppendLine("DECLARE @Where varchar(max)																																			");
            query.AppendLine("SET @Where = '[Minutos Usados] <= " + (txtCantMaxReg.Text.Length > 0 ? txtCantMaxReg.Text : "100") + "'");
            query.AppendLine("EXEC [ConsumoTelcelBajoConsumoMinRest3Meses]																														");
            query.AppendLine("@Schema='" + DSODataContext.Schema + "',																																");
            query.AppendLine("@Fields=@campos, 																																					    ");
            query.AppendLine("@Where = @Where,																																						");
            query.AppendLine("@Group = '[Línea],																																					");
            query.AppendLine("[Nombre Carrier],																																					    ");
            query.AppendLine("[Nombre Completo],																																					");
            query.AppendLine("[Nombre Centro de Costos],																																			");
            query.AppendLine("[FechaInicio]', 																																						");
            query.AppendLine("@Order = @orderBy,																																					");
            query.AppendLine("@OrderInv = @orderByinv,																																				");
            query.AppendLine("@Lenght = 99999,");
            query.AppendLine("@Start = 0,");
            query.AppendLine("@OrderDir = 'Desc',");
            query.AppendLine("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("@FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine("@FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine("@Moneda = '" + HttpContext.Current.Session["Currency"] + "',");
            query.AppendLine("@Idioma = '" + HttpContext.Current.Session["Language"] + "'");

            return query.ToString();

        }


        public string ConsultaConsumoDatosLineas1Mes()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("exec [ConsumoDatosLineas1Mes]				");
            query.AppendLine("	@Schema  = '" + DSODataContext.Schema + "',				");
            query.AppendLine("	@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("	@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("	@fechainiRep = '" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00' ,");
            query.AppendLine("	@fechaFinRep  = '" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'");

            return query.ToString();
        }
        public string ConsultaLLamadasMasCostosas()
        {
            StringBuilder query = new StringBuilder();
            /*LLAMADAS MAS COSTOSAS*/

            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >=''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <=''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'' '");
            query.AppendLine(" exec SPTopNLlamadasMasCarasTelcel @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Extension],");
            query.AppendLine(" [Nombre Completo],");
            query.AppendLine(" [Codigo Empleado],");
            query.AppendLine(" [Numero Marcado],");
            query.AppendLine(" [Fecha Llamada],");
            query.AppendLine(" [Hora Llamada],");
            query.AppendLine(" [Duracion Minutos],");
            query.AppendLine(" [Costo]=([Costo]/[TipoCambio]),");
            query.AppendLine(" [Dir Llamada],");
            query.AppendLine(" [Dir Llamada Codigo],");
            query.AppendLine(" [Punta A],");
            query.AppendLine(" [Punta B]', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Order = '[Costo] desc',");
            query.AppendLine(" @Lenght = 10,");
            query.AppendLine(" @OrderDir = 'Desc',");
            query.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español'");

            return query.ToString();
        }
        public string ConsultaLlamMasLargas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >=''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <=''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'' '");
            query.AppendLine(" exec SPTopNLlamadasMasLargasTelcel @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Extension],");
            query.AppendLine(" [Nombre Completo],");
            query.AppendLine(" [Numero Marcado],");
            query.AppendLine(" [Fecha Llamada],");
            query.AppendLine(" [Hora Llamada],");
            query.AppendLine(" [Duracion Minutos],");
            query.AppendLine(" [Costo]=([Costo]/[TipoCambio]),");
            query.AppendLine(" [Dir Llamada],");
            query.AppendLine(" [Dir Llamada Codigo],");
            query.AppendLine(" [Punta A],");
            query.AppendLine(" [Punta B]', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Order = '[Duracion Minutos] desc',");
            query.AppendLine(" @Lenght = 10,");
            query.AppendLine(" @OrderDir = 'Desc',");
            query.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }

        public string ConsultaLineasNuevasTelcel()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" set nocount on");

            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" IF Exists(SELECT * FROM tempdb.dbo.sysobjects WHERE[ID] = OBJECT_ID('tempdb.dbo.#TEMP'))");
            query.AppendLine(" begin ");
            query.AppendLine("      DROP TABLE #TEMP");
            query.AppendLine(" end ");

            query.AppendLine(" set @Where = 'FechaInicio >= ''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''");
            query.AppendLine(" and[Codigo Carrier] = 373 '");

            query.AppendLine(" exec ConsumoTelcelLineasNuevasV2  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Extension],");
            query.AppendLine(" [Nombre Completo],");
            query.AppendLine(" [Total]=Sum([Costo]/[TipoCambio]),");
            query.AppendLine(" [Duracion]=sum([Duracion Minutos]),");
            query.AppendLine(" [Numero]=count(*)',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Extension],");
            query.AppendLine(" [Nombre Completo]', ");
            query.AppendLine(" @Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Extension] Desc,[Nombre Completo] Desc',");
            query.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español'");

            return query.ToString();
        }




        public string ConsultaLineasTelcelBajas()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" set nocount on");

            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" IF Exists(SELECT * FROM tempdb.dbo.sysobjects WHERE[ID] = OBJECT_ID('tempdb.dbo.#TEMP'))");
            query.AppendLine(" begin ");
            query.AppendLine("      DROP TABLE #TEMP");
            query.AppendLine(" end ");

            query.AppendLine(" set @Where = ' FechaInicio >= dateadd(mm, -1, ''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'') and FechaInicio <= dateadd(mm, -1, ''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'') ");
            query.AppendLine(" and[Codigo Carrier] = 373 '");

            query.AppendLine(" exec ConsumoTelcelLineasBajaV2  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Extension],");
            query.AppendLine(" [Nombre Completo],[Nombre Centro de Costos],");
            query.AppendLine(" [Total]=Sum([Costo]/[TipoCambio]),");
            query.AppendLine(" [Duracion]=sum([Duracion Minutos]),");
            query.AppendLine(" [Numero]=count(*)',");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Extension],");
            query.AppendLine(" [Nombre Completo],[Nombre Centro de Costos]', ");
            query.AppendLine(" @Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Extension] Desc,[Nombre Completo] Desc',");
            query.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español'");

            return query.ToString();
        }
        public string ConsultaLineasConGastoMensaje()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >= ''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'' And [Costo] >0'");
            //query.AppendLine(" set @Where = 'FechaInicio >= ''2019-05-01 00:00:00'' and FechaInicio <= ''2019-05-31 23:59:59'' And [Costo] >0'");
            query.AppendLine(" exec ConsumoTelcelDetalleSMSRest @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = '[Extension],");
            query.AppendLine(" [Nombre Completo],");
            query.AppendLine(" [Codigo Empleado],");
            query.AppendLine(" [Nombre Centro de Costos],");
            query.AppendLine(" [Codigo Centro de Costos],");
            query.AppendLine(" CantMens=count(*),");
            query.AppendLine(" [Total]=Sum([Costo]/[TipoCambio])', ");
            query.AppendLine(" @Where = @Where,");
            query.AppendLine(" @Group = '[Extension],");
            query.AppendLine(" [Nombre Completo],");
            query.AppendLine(" [Codigo Empleado],");
            query.AppendLine(" [Nombre Centro de Costos],");
            query.AppendLine(" [Codigo Centro de Costos]', ");
            query.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }
        public string ConsultaDetalladoFacTelcel()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" declare @Where varchar(max)");
            query.AppendLine(" set @Where = 'FechaInicio >= ''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00'' and FechaInicio <= ''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59'' '");
            query.AppendLine(" exec ConsumoTelcelDetalleFacRest  @Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Fields = 'Linea = [Extension],");
            query.AppendLine(" [PlanLinea],");
            query.AppendLine(" [Min Libres Pico]=SUM([Min Libres Pico]),");
            query.AppendLine(" [Min Facturables Pico]=SUM([Min Facturables Pico]),");
            query.AppendLine(" [Min Libres No Pico]=SUM([Min Libres No Pico]),");
            query.AppendLine(" [Min Facturables No Pico]=SUM([Min Facturables No Pico]),");
            query.AppendLine(" [Tiempo Aire Nacional]=SUM([Tiempo Aire Nacional]/[TipoCambio]),");
            query.AppendLine(" [Tiempo Aire Roaming Nac]=SUM([Tiempo Aire Roaming Nac]/[TipoCambio]),");
            query.AppendLine(" [Tiempo Aire Roaming Int]=SUM([Tiempo Aire Roaming Int]/[TipoCambio]),");
            query.AppendLine(" [Larga Distancia Nac]=SUM([Larga Distancia Nac]/[TipoCambio]),");
            query.AppendLine(" [Larga Distancia Roam Nac]=SUM([Larga Distancia Roam Nac]/[TipoCambio]),");
            query.AppendLine(" [Larga Distancia Roam Int]=SUM([Larga Distancia Roam Int]/[TipoCambio]),");
            query.AppendLine(" [Servicios Adicionales]=SUM([Servicios Adicionales]/[TipoCambio]) + SUM([Renta]/[TipoCambio]),");
            query.AppendLine(" [Serv Adic] = 7577,");
            query.AppendLine(" [Desc Tiempo Aire]=SUM([Desc Tiempo Aire]/[TipoCambio]),");
            query.AppendLine(" [Desc Tiempo Aire Roam]=SUM([Desc Tiempo Aire Roam]/[TipoCambio]),");
            query.AppendLine(" [Otros Desc]=SUM([Otros Desc]/[TipoCambio]),");
            query.AppendLine(" [Cargos Creditos]=SUM([Cargos Creditos]/[TipoCambio]),");
            query.AppendLine(" [Otros Serv]=SUM([Otros Serv]/[TipoCambio]),");
            query.AppendLine(" [Otros Servicios]=7574,");
            query.AppendLine(" [Roaming GPRS Internacional]=SUM([Roaming GPRS Internacional]/[TipoCambio]),");
            query.AppendLine(" [Costo]=sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio])', ");
            query.AppendLine(" @Where =@Where,");
            query.AppendLine(" @Group = ' [Extension],[PlanLinea]', ");
            query.AppendLine(" @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine(" @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine(" @FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''',");
            query.AppendLine(" @FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''',");
            query.AppendLine(" @Moneda = 'MXP',");
            query.AppendLine(" @Idioma = 'Español'");
            return query.ToString();
        }
        private static string ConsultaInventarioLineasMoviles()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("declare @Where varchar(max)");
            consulta.AppendLine("declare @TodosCampos varchar(max)");
            consulta.AppendLine("set @Where = ''");
            consulta.AppendLine("set @TodosCampos = 'null'");
            consulta.AppendLine("if @TodosCampos <> 'null'");
            consulta.AppendLine("begin");
            consulta.AppendLine("set @Where = @Where + 'isnull([No Nomina],'''')+isnull([Nombre Completo],'''')+isnull([Nombre Centro de Costos],'''')+isnull([Nombre Razon Social],'''')");
            consulta.AppendLine("+isnull([Nombre Tipo Plan],'''')+isnull([Nombre Tipo Equipo],'''')+isnull([Modelo Equipo],'''')+isnull([IMEI],'''')+isnull([Plan],'''')");
            consulta.AppendLine("+isnull(convert(varchar,[Plazo Forzoso],103),'''') like ''%' + @TodosCampos + '%'' '");
            consulta.AppendLine("end");
            consulta.AppendLine("exec LineasTelcelTodosCamposRest @Schema='" + DSODataContext.Schema + "', @Fields = '[Extension],");
            consulta.AppendLine("[No Nomina],");
            consulta.AppendLine("[Nombre Completo],");
            consulta.AppendLine("[Codigo Empleado],");
            consulta.AppendLine("[Nombre Centro de Costos],");
            consulta.AppendLine("[Codigo Centro de Costos],");
            consulta.AppendLine("[Nombre Razon Social],");
            consulta.AppendLine("[Codigo Razon Social],");
            consulta.AppendLine("[Nombre Tipo Plan],");
            consulta.AppendLine("[Codigo Tipo Plan],");
            consulta.AppendLine("[Nombre Tipo Equipo],");
            consulta.AppendLine("[Codigo Tipo Equipo],");
            consulta.AppendLine("[Modelo Equipo],");
            consulta.AppendLine("[IMEI],");
            consulta.AppendLine("[Plan],");
            consulta.AppendLine("[Plazo Forzoso]',");
            consulta.AppendLine("@Where =@Where,");
            consulta.AppendLine("@Group = '',");
            consulta.AppendLine("@Order = '[Nombre Completo] Asc,[Nombre Centro de Costos] Asc,[No Nomina] Asc,[Extension] Asc,[Nombre Razon Social] Asc,[Nombre Tipo Plan] Asc,[Nombre Tipo Equipo] Asc,[Modelo Equipo] Asc,[IMEI] Asc,[Plan] Asc,[Plazo Forzoso] Asc',");
            consulta.AppendLine("@OrderInv = '[Nombre Completo] Desc,[Nombre Centro de Costos] Desc,[No Nomina] Desc,[Extension] Desc,[Nombre Razon Social] Desc,[Nombre Tipo Plan] Desc,[Nombre Tipo Equipo] Desc,[Modelo Equipo] Desc,[IMEI] Desc,[Plan] Desc,[Plazo Forzoso] Desc',");
            consulta.AppendLine("@Start = 0,");
            consulta.AppendLine("@OrderDir = 'Asc',");
            consulta.AppendLine("@Moneda = 'MXP',");
            consulta.AppendLine("@Idioma = 'Español'");
            return consulta.ToString();


        }


        public string ConsultaBuscaTotalporCuentaConcentradora(int opcion)
        {
            try
            {
                string fechainicio = Session["FechaInicio"].ToString() + " 00:00:00";
                string fechaFin = Session["FechaFin"].ToString() + " 23:59:59";

                StringBuilder query = new StringBuilder();

                query.AppendLine("exec [BuscaTotalResumenPorLinea]				");
                query.AppendLine("	@schema ='" + DSODataContext.Schema + "',					");
                query.AppendLine("	@fechaIniRep  = '" + fechainicio + "',		");
                query.AppendLine("	@fechaFinRep = '" + fechaFin + "'	,	");
                query.AppendLine("  @opcion = " + opcion + "");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        public string ConsultaRepTabPorMEplePorMes()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("exec[RepTabConsumoEmpsMasCarosCFMPorMes]");
                query.AppendLine("    @schema = '" + DSODataContext.Schema + "',");
                query.AppendLine("    @Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
                query.AppendLine("    @Perfil = " + HttpContext.Current.Session["iCodPerfil"] + "");


                return query.ToString();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string ConsultaReporteConsumoFPorLinea()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("Exec[BuscaConsumoPorLinea]    ");
            query.AppendLine("    @schema       =  '" + DSODataContext.Schema + "',          ");
            query.AppendLine("   @fechaInicio   =  '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            query.AppendLine("    @fechaFin     =  '" + Session["FechaFin"].ToString() + " 23:59:59'");


            return query.ToString();
        }

        #region Reporte lineas sin actividad telcel

        public string ConsRepLineasTelcelSinActividad()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" EXEC [dbo].[RepLineasTelcelSinActividad]");
            query.AppendLine(" @Schema = '" + DSODataContext.Schema + "',  ");
            query.AppendLine("@Idioma = '" + HttpContext.Current.Session["Language"] + "', ");
            query.AppendLine("@fechaFin     =  '" + Session["FechaFin"].ToString() + "'");
            return query.ToString();
        }

        #endregion Reporte lineas sin actividad telcel

        public string ConsultaRepLineasSinAct3MesesN2()
        {
            StringBuilder query = new StringBuilder();
            return query.ToString();
        }
        public string ConsultaRepExcendenteInternet()
        {
            StringBuilder query = new StringBuilder();
            return query.ToString();
        }
        public string ConsultaLineasExcedenPlanBase()
        {
            StringBuilder query = new StringBuilder();
            return query.ToString();
        }
        public string ConsultaDesgloseTipoExcedente()
        {
            StringBuilder query = new StringBuilder();
            return query.ToString();
        }
        #region Consultas Dasboard Internet
        public string ConsultaDistribucionGB()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [dbo].[DesgloceConsumoConceptoInternetGB]");
            query.AppendLine("@Esquema ='" + DSODataContext.Schema + "',");
            query.AppendLine("@FechaIni ='"+ Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@FechaFin = '"+ Session["FechaFin"].ToString() + "',");
            query.AppendLine("@iCodUsuario = "+ HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("@iCodPerfil= "+ HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("@TipoConsumo ='"+ param["TipoConsumo"] + "'");
            return query.ToString();

        }
        public string ConsultaDistribucionMoneda()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [dbo].[DesgloceConsumoConceptoInternetMoneda]");
            query.AppendLine("@Esquema ='" + DSODataContext.Schema + "',");
            query.AppendLine("@FechaIni ='" + Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@FechaFin = '" + Session["FechaFin"].ToString() + "',");
            query.AppendLine("@iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("@iCodPerfil= " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("@TipoConsumo ='" + param["TipoConsumo"] + "',");
            query.AppendLine("@Moneda='" + HttpContext.Current.Session["Currency"].ToString() + "'");
            return query.ToString();

        }
        public string ConsultaConsolidadoAplicacion()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [dbo].[DesgloceConsolidadoApp]");
            query.AppendLine("@Esquema ='" + DSODataContext.Schema + "',");
            query.AppendLine("@FechaIni ='" + Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@FechaFin = '" + Session["FechaFin"].ToString() + "',");
            query.AppendLine("@iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("@iCodPerfil= " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("@TipoConsumo ='" + param["TipoConsumo"] + "',");
            query.AppendLine("@Moneda='" + HttpContext.Current.Session["Currency"].ToString() + "'");
            return query.ToString();
        }
        public string ConsultaConsumoHistoricoInternet()
        {
            DateTime fechaAux1 = Convert.ToDateTime(Session["FechaInicio"].ToString());
            DateTime fechaFinal1 = new DateTime(fechaAux1.Year, fechaAux1.Month, 1);
            DateTime fechaInicio1 = fechaFinal1.AddMonths(-5);
            fechaInicio1.ToString("yyyy-MM-dd");
            fechaFinal1.ToString("yyyy-MM-dd");

            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [dbo].[ConsumoInternetConceptoHistorico] ");
            query.AppendLine("@Esquema ='" + DSODataContext.Schema + "',");
            query.AppendLine("@FechaIni ='" + fechaInicio1.ToString("yyyy-MM-dd") + "',");
            query.AppendLine("@FechaFin = '" + fechaFinal1.ToString("yyyy-MM-dd") + "',");
            query.AppendLine("@iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("@iCodPerfil= " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("@TipoConsumo ='" + param["TipoConsumo"] + "',");
            query.AppendLine("@Moneda='" + HttpContext.Current.Session["Currency"].ToString() + "'");
            return query.ToString();
        }
        public string ConsultaConsumoHistorico12Meses()
        {
            StringBuilder query = new StringBuilder();
            DateTime fechaAux1 = Convert.ToDateTime(Session["FechaInicio"].ToString());
            DateTime fechaFinal1 = new DateTime(fechaAux1.Year, fechaAux1.Month, 1);
            DateTime fechaInicio1 = fechaFinal1.AddMonths(-11);
            fechaInicio1.ToString("yyyy-MM-dd");
            fechaFinal1.ToString("yyyy-MM-dd");

            query.AppendLine("EXEC [dbo].[HistoricoGastoInternet12Meses]");
            query.AppendLine("@Esquema ='" + DSODataContext.Schema + "',");
            query.AppendLine("@FechaIni ='" + fechaInicio1.ToString("yyyy-MM-dd") + "',");
            query.AppendLine("@FechaFin = '" + fechaFinal1.ToString("yyyy-MM-dd") + "',");
            query.AppendLine("@iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"] + ",");
            query.AppendLine("@iCodPerfil= " + HttpContext.Current.Session["iCodPerfil"] + ",");
            query.AppendLine("@Moneda =  '" + Session["Currency"] + "'");
            return query.ToString();
        }
        #endregion

        private string ConsultaLineasPorPlanBase()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ObtieneCantidadDeLineasPorPlanBase]");
            query.AppendLine("@Schema =  '" + DSODataContext.Schema + "',");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "',");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "',");
            query.AppendLine("@fechaIniRep =  '" + HttpContext.Current.Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@Moneda =  '" + Session["Currency"] + "'");
            return query.ToString();
        }

        private string ConsultaTablaMesesInactivos()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ObtieneLineasSinActividadNMeses]");
            query.AppendLine("@Schema =  '" + DSODataContext.Schema + "',");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "',");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "',");
            query.AppendLine("@fechaIniRep =  '" + Session["FechaInicio"].ToString() + "',");
            query.AppendLine("@Moneda =  '" + Session["Currency"] + "'");
            return query.ToString();
        }

        private string ConsultaTablaMesesInactivosSinImporte()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ObtieneLineasSinActividadNMesesSinImportes]");
            query.AppendLine("@Schema =  '" + DSODataContext.Schema + "',");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "',");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "',");
            query.AppendLine("@fechaIniRep =  '" + Session["FechaInicio"].ToString() + "'");
            return query.ToString();
        }

        private string ConsultaTablaMesesInactivosCantidad()
        {
            string fechaDePagina = Session["FechaInicio"].ToString().Replace("-", "");
            DateTime fechaFinal = DateTime.ParseExact(fechaDePagina, "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime fechaInicio = new DateTime(fechaFinal.Year, fechaFinal.Month - 5, 1);
            fechaFinal = fechaInicio.AddMonths(6).AddSeconds(-1);
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ObtieneLineasSinActividadCantidadNMeses]");
            query.AppendLine("@Schema =  '" + DSODataContext.Schema + "',");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "',");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "',");
            query.AppendLine("@fechaIni =  '" + fechaInicio.ToString("yyyy-MM-dd") + "',");
            query.AppendLine("@fechaFin =  '" + fechaFinal.ToString("yyyy-MM-dd") + "'");
            return query.ToString();
        }
        // TODO : DM Paso 1 - Consulta del reporte
        #endregion Consultas a BD
    }
}