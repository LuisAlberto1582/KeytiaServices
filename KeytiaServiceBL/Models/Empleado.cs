using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Empleado : HistoricoBase
    {
        private string _nombre = string.Empty;
        private string _paterno = string.Empty;
        private string _materno = string.Empty;
        private string _nomCompleto = string.Empty;

        public int CenCos { get; set; }
        public int TipoEm { get; set; }
        public int Puesto { get; set; }
        public int Emple { get; set; }
        public int Usuar { get; set; }
        public int TipoPr { get; set; }
        public int PeriodoPr { get; set; }
        public int Organizacion { get; set; }
        public int OpcCreaUsuar { get; set; }
        public int BanderasEmple { get; set; }

        public double PresupFijo { get; set; }
        public double PresupProv { get; set; }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
            set
            {
                _nombre = value.Trim().Replace("  ", " ").ToUpper();
            }
        }
        public string Paterno
        {
            get
            {
                return _paterno;
            }
            set
            {
                _paterno = (string.IsNullOrEmpty(value)) ? string.Empty : value.Trim().Replace("  ", " ").ToUpper();

            }
        }
        public string Materno
        {
            get
            {
                return _materno;
            }
            set
            {
                _materno = (string.IsNullOrEmpty(value)) ? string.Empty : value.Trim().Replace("  ", " ").ToUpper();
            }
        }
        public string RFC { get; set; }
        public string Email { get; set; }
        public string Ubica { get; set; }
        public string NominaA { get; set; }
        public string NomCompleto
        {
            get
            {
                return _nomCompleto;
            }

            set
            {
                _nomCompleto = value.Trim().Replace("  ", " "); //Se eliminan los espacios dobles y espacios en blanco
            }
        }

    }
}
