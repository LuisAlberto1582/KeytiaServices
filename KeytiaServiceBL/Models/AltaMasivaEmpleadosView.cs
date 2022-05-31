using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class AltaMasivaEmpleadosView : CargaMasiva
    {
        public DateTime FechaAlta { get; set; }
        public string Nomina { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Email { get; set; }
        public int CC { get; set; }
        public string TipoEmpleado { get; set; }
        public string Puesto { get; set; }
        public string Jefe { get; set; }
        public string Ubicacion { get; set; }
        public int CrearUsuario { get; set; }
        public int OpcCrearUsuario { get; set; }


        //Datos de Apoyo
        public int ICodCatalogo { get; set; }
        public int ICodUsuar { get; set; }
        public bool SeCreo { get; set; }
    }
}
