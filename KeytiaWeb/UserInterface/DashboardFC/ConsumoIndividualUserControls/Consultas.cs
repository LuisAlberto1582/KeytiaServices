using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KeytiaWeb.UserInterface.DashboardFC.ConsumoIndividualUserControls
{
    public  class Consultas
    {
        public  string RepPorCategoria(int linea, string fechaInicio)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("EXEC dbo.[ConsumoIndividualConceptos]  ");
            consulta.AppendLine("@Esquema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@icodLinea = " + linea + ",");
            consulta.AppendLine("@Fecha = '" + fechaInicio + "'");


            return consulta.ToString();
        }
        public  string ConsultaDesgloceAPP(int linea, string tipo, string fechaInicio)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("EXEC dbo.[ConsumoIndividualDesgloceAPPConsumoInternet]  ");
            consulta.AppendLine("@Esquema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@icodLinea = " + linea + ",");
            consulta.AppendLine("@Fecha = '" + fechaInicio + "', ");
            consulta.AppendLine("@TipoConsumo = '" + tipo + "'");


            return consulta.ToString();
        }
        public  string ConsultaCantidadExtensiones(int empleado, string fechaInicio)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("EXEC ConsumoIndividualCantidadExtensiones  ");
            consulta.AppendLine("@Esquema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@icodCatEmple = " + empleado + ",");
            consulta.AppendLine("@fechaInicio = '" + fechaInicio + "'");


            return consulta.ToString();
        }
        public  string ConsultaCantidadLineasMoviles(int empleado, string fechaInicio)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("EXEC ConsumoIndividualCantidadLineasMoviles  ");
            consulta.AppendLine("@Esquema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@icodCatEmple = " + empleado + ",");
            consulta.AppendLine("@fechaInicio = '" + fechaInicio + "'");


            return consulta.ToString();
        }
        public  string ConsultaCantidadClavesFac(int empleado, string fechaInicio)
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("EXEC ConsumoIndividualCantidadClavesFAC  ");
            consulta.AppendLine("@Esquema = '" + DSODataContext.Schema + "',  ");
            consulta.AppendLine("@icodCatEmple = " + empleado + ",");
            consulta.AppendLine("@fechaInicio = '" + fechaInicio + "'");


            return consulta.ToString();
        }
        public  string ObtieneLineasMoviles(int empleado, string fechaInicio)
        {
            var año = fechaInicio.Split('-').Take(1).First().ToString();
            int mes = int.Parse(fechaInicio.Split('-').Skip(1).Take(1).First().ToString());
            var fechavarchar = string.Concat(año, mes + 12);
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("select linea, LineaDesc ");
            consulta.AppendLine("from " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Linea','Español')] EmpleLinea ");
            consulta.AppendLine("where " + fechavarchar + "between convert(varchar, DATEPART(yyyy, dtIniVigencia)) + convert(varchar, DATEPART(mm, dtIniVigencia) + 12) and convert(varchar, DATEPART(yyyy, dtFinVigencia)) + convert(varchar, DATEPART(mm, dtFinVigencia) + 12)");
            consulta.AppendLine("and EmpleLinea.dtInivigencia <> EmpleLinea.dtFinVigencia");
            consulta.AppendLine("and EmpleLinea.Emple = " + empleado);


            return consulta.ToString();
        }

        public  string ConsumoTelefoniaFija(int empleado, string fechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [dbo].[ConsumoIndividualPresupConsumoFija]");
            query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = " + empleado + ",");
            query.AppendLine("@Fecha = '" + fechaInicio + "'");


            return query.ToString();

        }
        public  string ConsumoIndividualTipoDestino(int empleado, string fechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [dbo].[ConsumoIndividualTipoDestino]");
            query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = " + empleado + ",");
            query.AppendLine("@Fecha = '" + fechaInicio + "'");


            return query.ToString();

        }

        //public  string ReporteLlamadasMoviles(int linea, string fechaInicio)
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.AppendLine("EXEC dbo.[ConsumoIndividualDetallMovil]");
        //    query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
        //    query.AppendLine("@icodLinea = " + linea + ",");
        //    query.AppendLine("@Fecha = '" + fechaInicio + "'");

        //    var result = query.ToString();
        //    return result;
        //}

        public string ReporteLlamadasMoviles(int linea, string fechaInicio, string FechaFin)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("Exec sp_BAT_DetalleLlamsTelMovilv2");
            query.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Linea = '" + linea + "',");
            query.AppendLine("@FechaInicio  = '" + fechaInicio + "',");
            query.AppendLine("@FechaFin  = '" + FechaFin + "'");

            var result = query.ToString();
            return result;
        }




        public  string ReporteLlamadasFija(int emple, string fechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC dbo.[ConsumoIndividualDetalleLlamFija]");
            query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = " + emple + ",");
            query.AppendLine("@Fecha = '" + fechaInicio + "'");

            var result = query.ToString();
            return result;
        }
        public  string ReporteConsumoDeDatos(int linea, string fechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC dbo.[ConsumoIndividualDetallConsumoDatos]");
            query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@icodLinea = " + linea + ",");
            query.AppendLine("@Fecha = '" + fechaInicio + "'");

            var result = query.ToString();
            return result;
        }

        public  string ConsumoHistorico(int emple, string fechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC dbo.[ConsumoIndividualConsumoHistorico]");
            query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = " + emple + ",");
            query.AppendLine("@CantMeses= 11,");
            query.AppendLine("@Fecha = '" + fechaInicio + "'");

            return query.ToString();

        }

        public  string PresupuestoGastoTelMovil(int emple, string fechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ConsumoIndividualPresupConsumoMovil]");
            query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = " + emple + ",");
            query.AppendLine("@CantMeses = 5,");
            query.AppendLine("@Fecha = '" + fechaInicio + "'");
            return query.ToString();
        }

        public string PresupuestoGastoTelMovilPorLinea(int emple, int linea, string fechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ConsumoIndividualPresupConsumoMovilPorLinea]");
            query.AppendLine("@Esquema = '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = " + emple + ",");
            query.AppendLine("@Linea = '" +linea + "',");
            query.AppendLine("@CantMeses = 5,");
            query.AppendLine("@Fecha = '" + fechaInicio + "'");
            return query.ToString();
        }


        public string GetUsuario(string vchCodUsuario)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" Select iCodCatalogo, Nombre, Email, PuestoDesc ");
            query.AppendLine(" from " + DSODataContext.Schema + ".[vishistoricos('Emple','Empleados','Español')]");
            query.AppendLine(" where UsuarCod = '" + vchCodUsuario + "'");
            query.AppendLine(" and dtIniVigencia <> dtFinVigencia and dtFinVigencia>=getdate()");
            return query.ToString();

        }


        public string GetExtensiones(int empleado, string fechaInicio)
        {
            var año = fechaInicio.Split('-').Take(1).First().ToString();
            int mes = int.Parse(fechaInicio.Split('-').Skip(1).Take(1).First().ToString());
            var fechavarchar = string.Concat(año, mes + 12);
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("select ExtenCod ");
            consulta.AppendLine("from " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Extension','Español')] EmpleExten  ");
            consulta.AppendLine("where " + fechavarchar + "between convert(varchar, DATEPART(yyyy, dtIniVigencia)) + convert(varchar, DATEPART(mm, dtIniVigencia) + 12) and convert(varchar, DATEPART(yyyy, dtFinVigencia)) + convert(varchar, DATEPART(mm, dtFinVigencia) + 12)");
            consulta.AppendLine("and EmpleExten.dtInivigencia <> EmpleExten.dtFinVigencia");
            consulta.AppendLine("and EmpleExten.Emple = '" + empleado +"'");


            return consulta.ToString();

        }


        public string DesgloceConceptos(int empleado, int linea,  string fechaInicio, string fechaFin)
        {

            StringBuilder query = new StringBuilder();
            query.AppendLine("Exec BATConsumoTelcelPorConceptos ");
            query.AppendLine("@Schema= '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = '" + empleado + "',");
            query.AppendLine("@Linea = " + linea + ",");
            query.AppendLine("@FechaIniRep = '" + fechaInicio + "', ");
            query.AppendLine("@FechaFinRep = '" + fechaFin + "'");
            return query.ToString();

        }


        public string GetCalvesFac(int empleado, string fechaInicio)
        {
            var año = fechaInicio.Split('-').Take(1).First().ToString();
            int mes = int.Parse(fechaInicio.Split('-').Skip(1).Take(1).First().ToString());
            var fechavarchar = string.Concat(año, mes + 12);
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("select CodAutoCod ");
            consulta.AppendLine("from " + DSODataContext.Schema + ".[VisRelaciones('Empleado - CodAutorizacion','Español')] EmpleCod  ");
            consulta.AppendLine("where " + fechavarchar + "between convert(varchar, DATEPART(yyyy, dtIniVigencia)) + convert(varchar, DATEPART(mm, dtIniVigencia) + 12) and convert(varchar, DATEPART(yyyy, dtFinVigencia)) + convert(varchar, DATEPART(mm, dtFinVigencia) + 12)");
            consulta.AppendLine("and EmpleCod.dtInivigencia <> EmpleCod.dtFinVigencia");
            consulta.AppendLine("and EmpleCod.Emple = '" + empleado + "'");


            return consulta.ToString();

        }




        public string DetalleServicios(int empleado, int linea, string fechaInicio, string fechaFin, int concepto)
        {

            StringBuilder query = new StringBuilder();
            query.AppendLine("exec sp_BAT_DetalleServicios ");
            query.AppendLine("@Schema= '" + DSODataContext.Schema + "',");
            query.AppendLine("@Emple = '" + empleado + "',");
            query.AppendLine("@idConcepto = " + concepto + ",");
            query.AppendLine("@Linea = " + linea + ",");
            query.AppendLine("@FechaIniRep = '" + fechaInicio + "', ");
            query.AppendLine("@FechaFinRep = '" + fechaFin + "'");
            return query.ToString();

        }

    }
}