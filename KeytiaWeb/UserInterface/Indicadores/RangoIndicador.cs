using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface.Indicadores
{
    public class RangoIndicador
    {
        public int Indicador { get; set; }
        public string IndicadorCod { get; set; }
        public int RangoId { get; set; }
        public string RangoCod { get; set; }
        public decimal LimiteInferior { get; set; }
        public decimal LimiteSuperior { get; set; }
        public int RangoColorId { get; set; }
        public string RangoColor { get; set; }
    }
}