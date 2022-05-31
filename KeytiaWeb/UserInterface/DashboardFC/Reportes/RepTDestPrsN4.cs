using System.Text;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Web.UI;
using KeytiaServiceBL;
using System.Collections.Generic;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public class RepTDestPrsN4 : DashboardFC.ReporteFC
    {


        public RepTDestPrsN4(string FechaInicio, string FechaFin, string RequestPath, Dictionary<string,string> listadoParametros) :
            base(FechaInicio, FechaFin, RequestPath, listadoParametros)
        {

        }

       
        public string ConsultaPorTDestEmplePrsN4(string linkGraf)
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
            lsb.Append("set @ParamCenCos = '" + RepCenCos + "'\r ");
            lsb.Append("set @ParamCarrier = '"+ RepCarrier +"'\r ");
            lsb.Append("set @ParamExtension = 'null'\r ");
            lsb.Append("set @ParamCodAut = 'null'\r ");
            lsb.Append("set @ParamLocali = 'null'\r ");
            lsb.Append("set @ParamTelDest = 'null'\r ");
            lsb.Append("set @ParamGEtiqueta = 'null'\r ");
            lsb.Append("set @ParamTDest = '" + RepTDest + "'\r ");
            lsb.Append("\r ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + RepFechaInicio+ "'' and FechaInicio <= ''" + RepFechaFin +"''\r ");
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
            lsb.Append("if @ParamTDest = 238135\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in(385,388,389)\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("if @ParamTDest = 238134\r ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in(381,382,383,384,385,386,387,388,389,390,391,392,393,394,395,82851,83619,83620,87680,217620,238135)\r ");
            lsb.Append("'\r ");
            lsb.Append("\r ");
            lsb.Append("exec ProsaConsumoAcumuladoPorEmpleadoTodosParamRest     @Schema='" + KeytiaServiceBL.DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Codigo Colaborador] = [Codigo Empleado], NombreEmple = [Nombre Completo],\r ");
            lsb.Append("TotImporte = SUM([Costo]+[CostoSM]),\r ");
            lsb.Append("LLamadas = sum([TotalLlamadas]),\r ");
            lsb.Append("TotMin = SUM([Duracion Minutos]) \r ");

            if (linkGraf != "")
            {
                lsb.Append("," + linkGraf);
            }

            lsb.Append("',  \r");

            lsb.Append("			                                          @Where = @Where,\r ");
            lsb.Append("			                                          @Group = '[Codigo Empleado],[Nombre Completo]', \r ");
            lsb.Append("			                                          @Order = '[TotImporte] Desc,[LLamadas] Desc,[TotMin] Desc, NombreEmple Asc',\r ");
            lsb.Append("			                                          @OrderInv = '[TotImporte] Asc,[LLamadas] Asc,[TotMin] Asc, NombreEmple Desc',\r ");
            lsb.Append("			                                          @OrderDir = 'Desc',\r ");
            lsb.Append("                                                                                                   @TipoDest = @ParamTDest,\r ");
            lsb.Append("                                       @Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("                                       @Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.Append("			                           @FechaIniRep = '''" + RepFechaInicio +"''',\r ");
            lsb.Append("			                           @FechaFinRep = '''" + RepFechaFin +"''',\r ");
            lsb.Append("                                                                                                   @Moneda = '" + Session["Currency"] + "', \r ");
            lsb.Append("                                                                                                   @Idioma = 'Español'\r ");


            return lsb.ToString();


        }


        public DataTable RepTabTipoDestinoPrs2PnlsNiv4(Control contenedorGrid, string tituloGrid, List<CampoReporte> listadoCamposReporte, string URLSigNivelCampoAdicional)
        {
            //Se obtiene el string de la consulta (DataSource en K5)
            string queryString = ConsultaPorTDestEmplePrsN4(URLSigNivelCampoAdicional);


            //Se llena un DataTable con el resultado de la consulta
            DataTable ldtGrid = ObtieneDatosParaGrid(queryString, listadoCamposReporte);


            return ldtGrid;

        }

    }
}
