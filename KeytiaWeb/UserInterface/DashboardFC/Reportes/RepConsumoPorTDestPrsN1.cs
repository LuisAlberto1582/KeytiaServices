using System.Text;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using KeytiaServiceBL;


namespace KeytiaWeb.UserInterface.DashboardFC
{
    public class RepConsumoPorTDestPrsN1 : DashboardFC.ReporteFC
    {

        public RepConsumoPorTDestPrsN1(string FechaInicio, string FechaFin, string RequestPath, Dictionary<string, string> listadoParametros) :
            base(FechaInicio, FechaFin, RequestPath, listadoParametros)
        {

        }


        public string ConsultaConsumoTDestPrsN1(string linkGraf)
        {

            int perfil = (int)((object)Session["iCodPerfil"]);


            StringBuilder lsb = new StringBuilder();


            /* BG.20150405 SE VALIDA SI EL USUARIO ES DE PERFIL CONFIGURADOR O ADMINISTRADOR, 
             * SI ES ASI MOSTRARA EL COSTO REAL, SINO EL COSTO INFLADO 
             * CostoReal = [CostoFac], CostoInflado = [Costo]
            */

            if (perfil == 367 || perfil == 369)
            {
                lsb.Append("declare @Where varchar(max)\r ");
                lsb.Append("set @Where = 'FechaInicio >= ''" + RepFechaInicio + "'' and FechaInicio <= ''" + RepFechaFin + "''\r ");

                lsb.Append("exec ProsaRepTabConsumoPorTipoDestinoSP      @Schema='" + KeytiaServiceBL.DSODataContext.Schema + "', \r");
                lsb.Append("@Fields='[Codigo Tipo Destino], \r ");
                lsb.Append("[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])),\r ");
                lsb.Append("[Total]= sum([CostoFac]),\r ");
                lsb.Append("[SM]= sum([CostoSM]),\r ");
                lsb.Append("[Numero]=SUM([TotalLlamadas]),\r ");
                lsb.Append("[Duracion]=sum([Duracion Minutos])'\r ");
                if (linkGraf != "")
                {
                    lsb.Append("," + linkGraf);
                }
                lsb.Append("',  \r");
                lsb.Append("@Where = @Where,\r ");
                lsb.Append("@Group = '[Codigo Tipo Destino]', \r ");
                lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc',\r ");
                lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc',\r ");
                lsb.Append("@OrderDir = 'Desc',\r ");
                lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
                lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
                lsb.Append("@FechaIniRep = '''" + RepFechaInicio + "''',\r ");
                lsb.Append("@FechaFinRep = '''" + RepFechaFin + "''',\r ");
                lsb.Append("@Moneda = '" + Session["Currency"] + "', \r ");
                lsb.Append("@Idioma = 'Español'\r ");

                return lsb.ToString();
            }
            else
            {

                lsb.Append("declare @Where varchar(max)\r ");
                lsb.Append("set @Where = 'FechaInicio >= ''" + RepFechaInicio + "'' and FechaInicio <= ''" + RepFechaFin + "''\r ");

                lsb.Append("exec ProsaRepTabConsumoPorTipoDestinoSP      @Schema='" + KeytiaServiceBL.DSODataContext.Schema + "', \r");
                lsb.Append("@Fields='[Nombre Tipo Destino]=Min(upper([Nombre Tipo Destino])), \r ");
                lsb.Append("[Codigo Tipo Destino],\r ");
                lsb.Append("[Total]= sum([Costo]),\r ");
                lsb.Append("[SM]= sum([CostoSM]),\r ");
                lsb.Append("[Numero]=SUM([TotalLlamadas]),\r ");
                lsb.Append("[Duracion]=sum([Duracion Minutos])'\r ");
                if (linkGraf != "")
                {
                    lsb.Append("," + linkGraf);
                }
                lsb.Append("',  \r");
                lsb.Append("@Where = @Where,\r ");
                lsb.Append("@Group = '[Codigo Tipo Destino]', \r ");
                lsb.Append("@Order = '[Total] Desc,[Duracion] Desc,[Numero] Desc,[Nombre Tipo Destino] Asc',\r ");
                lsb.Append("@OrderInv = '[Total] Asc,[Duracion] Asc,[Numero] Asc,[Nombre Tipo Destino] Desc',\r ");
                lsb.Append("@OrderDir = 'Desc',\r ");
                lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",   \n");
                lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",   \n");
                lsb.Append("@FechaIniRep = '''" + RepFechaInicio + "''',\r ");
                lsb.Append("@FechaFinRep = '''" + RepFechaFin + "''',\r ");
                lsb.Append("@Moneda = '" + Session["Currency"] + "', \r ");
                lsb.Append("@Idioma = 'Español'\r ");

                return lsb.ToString();

            }

        }


        public DataTable RepTabConsumoTDestPrs2PnlsNiv1(Control contenedorGrid, string tituloGrid, List<CampoReporte> listadoCamposReporte, string URLSigNivelCampoAdicional)
        {
            //Se obtiene el string de la consulta (DataSource en K5)
            string queryString = ConsultaConsumoTDestPrsN1(URLSigNivelCampoAdicional);


            //Se llena un DataTable con el resultado de la consulta
            DataTable ldtGrid = ObtieneDatosParaGrid(queryString, listadoCamposReporte);


            return ldtGrid;

        }

    }
}
