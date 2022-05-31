using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface.Inventario
{
    public class EquipoMovil
    {

        public int iCodCatHistEquipo { set; get; }
        public string IMEI{ get; set; }
        public string NoSerie { get; set; }
        public int Marca { set; get; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string  FechaIngreso { get; set; }
        public string  FechaRetiro { get; set; }
        public string AlmacenResguardo { get; set; }
        public string MotivoRetiro { get; set; }
        public int ValorRetiroEquipo { get; set; }
        
        public int ICodCatEmple{ get; set; }
        public string  NombreEmpleado{ get; set; }
        public string  FechaAsignacion { get; set; }
    }
}