using KeytiaServiceBL.CargaGenerica.CargaIFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class MarcacionLocalidad
    {
        public int ICodCatalogo { get; set; }
        public string VchCodigo { get; set; }
        public int Paises { get; set; }
        public string Clave { get; set; }
        public string Serie { get; set; }
        public string NumIni { get; set; }
        public string NumFin { get; set; }
        public int Ocupacion { get; set; }
        public int Locali { get; set; }
        public string LocaliCod { get; set; }
        public string Poblacion { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string TipoRed { get; set; }
        public int TDest { get; set; }
        public int Region { get; set; }
        public int ASL { get; set; }
        public string Modalidad { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime FechaConsolidacion { get; set; }
        public DateTime FechaMigracion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime DtIniVigencia { get; set; }
        public DateTime DtFinVigencia { get; set; }
        public DateTime DtFecUltAct { get; set; }
    }
}
