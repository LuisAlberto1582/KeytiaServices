using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ABCEmpleadosViewEmpleRecurs : CargaMasiva
    {
        public DateTime Fecha { get; set; }
        public string Nomina { get; set; }
        public string Nombres { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Email { get; set; }
        public int CenCos { get; set; }
        public string CveDescCenCos { get; set; }
        public int TipoEmpleado { get; set; }
        public string CveDescTipoEmpleado { get; set; }
        public string Puesto { get; set; }
        public string Jefe { get; set; }
        public string Ubicacion { get; set; }
        public string RFC { get; set; }
        public int Organizacion { get; set; }
        public string CveDescOrganizacion { get; set; }
        public int CrearUsuario { get; set; }
        public int OpcCrearUsuario { get; set; }
        public string TipoMovimiento { get; set; }

        //Datos de Apoyo
        public int ICodCatalogo { get; set; }
        public int ICodUsuar { get; set; }
        public bool Exitoso { get; set; }
        public int ICodPuesto { get; set; }
    }
}
