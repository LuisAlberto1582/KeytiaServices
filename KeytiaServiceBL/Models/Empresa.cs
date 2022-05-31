using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Empresa : HistoricoBase
    {
        public int Cliente { get; set; }
        public int Pais { get; set; }
        public int FechasDefault { get; set; }
        public int DiaLimiteDefault { get; set; }
        public int BanderasEmpre { get; set; }
        public int GEtiqueta { get; set; }
        public int DiaInicioPeriodo { get; set; }
        public double PrepDefault { get; set; }
        public string RazonSocial { get; set; }
        public string MasterPage { get; set; }
        public string Logo { get; set; }
        public string StyleSheet { get; set; }
        public string HomePage { get; set; }
    }
}
