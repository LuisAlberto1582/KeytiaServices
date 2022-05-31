using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Text;
using System.Web.UI;
using KeytiaServiceBL;
using AjaxControlToolkit;


namespace KeytiaWeb.UserInterface.DashboardFC
{
    public class RepPorSucursalN1 : DashboardFC.ReporteFC
    {

        public RepPorSucursalN1(string FechaInicio, string FechaFin, string RequestPath, Dictionary<string, string> listadoParametros) :
            base(FechaInicio, FechaFin, RequestPath, listadoParametros)
        {

        }



        public string ConsultaPorSucursal(string linkGraf)
        {
            StringBuilder lsb = new StringBuilder();



            lsb.Append("declare @Where varchar(max) = '' \n ");
            lsb.Append("select @Where = @Where + 'FechaInicio >= ''" + RepFechaInicio + "''   \n ");
            lsb.Append("                                           and FechaInicio <= ''" + RepFechaFin + "''' \n ");
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
            lsb.Append(" 			                           @FechaIniRep = '''" + RepFechaInicio + "''', \n ");
            lsb.Append(" 			                           @FechaFinRep = '''" + RepFechaFin + "''', \n ");
            lsb.Append("									   @Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("									   @Idioma = 'Español' \n ");



            return lsb.ToString();
        
        }

        

        //Asi estaba antes: public DataTable RepTabPorSucursal(Control contenedorGrid, string tituloGrid, List<CampoReporte> listadoCamposReporte, string URLSigNivelCampoAdicional)
        //Esa era la firma del metodo, pero no se estan ocupando todos los paramentros.
        public DataTable RepTabPorSucursal(List<CampoReporte> listadoCamposReporte, string URLSigNivelCampoAdicional)
        {
            //Se obtiene el string de la consulta (DataSource en K5)
            string queryString = ConsultaPorSucursal(URLSigNivelCampoAdicional);


            //Se llena un DataTable con el resultado de la consulta
            DataTable ldtGrid = ObtieneDatosParaGrid(queryString, listadoCamposReporte);


            return ldtGrid;

        }





    }
}
