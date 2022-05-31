using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{
    public partial class ConsumoGeneralMoviles
    {
        static Dictionary<string, string> param = new Dictionary<string, string>();
        List<InfoCarrierGlobal> dtCarriers = new List<InfoCarrierGlobal>();
        public void Navegaciones()
        {
            LeeQueryString();
            dtCarriers = GetCarriersTIM(0, 0, "ConsumoGeneral");
            ddlCarrier.Visible = btnAplicar.Visible = true;
            if (!Page.IsPostBack)
            {
                var carriers = dtCarriers.
                                GroupBy(g => new { g.iCodCatalogo, g.vchDescripcion }).
                                Select(x => x.First()).
                                Select(x => new { x.iCodCatalogo, EmpreDesc = x.vchDescripcion + " - " + x.EmpreDesc }).ToList();

                ddlCarrier.DataSource = carriers;

                ddlCarrier.DataValueField = "iCodCatalogo";
                ddlCarrier.DataTextField = "EmpreDesc";
                ddlCarrier.DataBind();
            }

            //Datos de año y mes
            var rowCarrier = dtCarriers.OrderByDescending(x => x.MaxFecha).First();

            ///RM 20190529 Se modifica la forma en base a a que se calcula la maxFecha para mostrar en Dsh
            var carrier = 0;
            int.TryParse(ddlCarrier.SelectedValue, out carrier);

            if (carrier == 0 && int.Parse(carrierSelected) > 0)
            {
                int.TryParse(carrierSelected, out carrier);
            }

            if (carrier > 0)
            {
                rowCarrier = dtCarriers.Where(x => (x.IdsCarrierEmpre.ToString().Split('-'))[0] == carrier.ToString()).OrderByDescending(x => x.MaxFecha).First();
            }

            
            /*
             * Linea 1
             * A1 .-
             * B1
             * C1
             */
            RepAcumuladoCarrier(Rep1, GroupBy.Carrier);
            RepConsumoHistorico(Rep2, "Consumo histórico", 0);
            RepLCarriersMoviles(Rep3);

            /*
            * Linea 2
            * A2 .-
            * B2
            * C2 .- Reporte por concepto acumulado
            */
            
            ReporteConsumoGlobalPorXTabsComparativos(Rep4, GroupBy.Carrier);
            ReporteConsumoGlobalPorXTabsComparativos(Rep5, GroupBy.Concepto);
            ReportePorConcepGrafTotalizado(Rep6, GroupBy.Concepto, "bar2d",  null);


            /*
            * Linea 3
            * A3 .-
            * B3
            * C3 .- 
            */
            RepHistoricoGastoInternet(Rep7);
            UsoInternetPorTabs("Datos Nacionales");
            UsoInternetPorTabs("Datos Internacionales");

            /*
           * Linea 4
           * A4 .-
           * B4
           */
            //Activar validacion cuando carrier sea telcel
            bool TodosOtelcel = carrier == 373 || carrier == -1;
            if (TodosOtelcel)
            {
                ReporteConsumoPlanesFacturados(Rep10);
                ReporteConsumoPlanesFacturadosExcedentes(Rep11);
            }
        }
    }
}