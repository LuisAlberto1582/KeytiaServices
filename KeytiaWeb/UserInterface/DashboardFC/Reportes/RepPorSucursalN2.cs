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
    public class RepPorSucursalN2 : DashboardFC.ReporteFC
    {

        public RepPorSucursalN2(string FechaInicio, string FechaFin, string RequestPath, Dictionary<string, string> listadoParametros) :
            base(FechaInicio, FechaFin, RequestPath, listadoParametros)
        {

        }

        public string ConsultaPorSucursalN2(string linkGraf, string Sitio)
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

            //if (linkGraf != "")
            //{
            //    lsb.Append("," + linkGraf);
            //}

            //lsb.Append("',  \r");
            lsb.Append("@Sitio = " + Sitio.ToString() + ",\r ");   //NEXTEL
            lsb.Append("@OrderDir = 'Asc',\r ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
            lsb.Append("@FechaIniRep = '''" + RepFechaInicio + "''',\r ");
            lsb.Append("@FechaFinRep = '''" + RepFechaFin + "''',\r ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r ");
            lsb.Append("@Idioma = 'Español'\r ");

            return lsb.ToString();
        }



        public DataTable RepTabPorSucursalN2(Control contenedorGrid, string tituloGrid, List<CampoReporte> listadoCamposReporte, string URLSigNivelCampoAdicional, string Sitio)
        {
            //Se obtiene el string de la consulta (DataSource en K5)
            string queryString = ConsultaPorSucursalN2(URLSigNivelCampoAdicional, Sitio);


            //Se llena un DataTable con el resultado de la consulta
            DataTable ldtGrid = ObtieneDatosParaGrid(queryString, listadoCamposReporte);


            return ldtGrid;

        }





    }
}
