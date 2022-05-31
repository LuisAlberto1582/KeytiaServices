using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public class CampoReporte : DashboardFC.Dashboard
    {
        #region Campos
        private string _nombreEnConsulta = string.Empty;
        private string _nombreEnWeb = string.Empty;
        private string _formato = string.Empty;
        private bool _esURLField = false;
        private bool _esDatoDeNavegacion = false;
        private bool _esBoundField = false;
        private bool _esHyperlinkField = false;

      
        #endregion


        #region Propiedades
        public bool EsURLField
        {
            get { return _esURLField; }
            set { _esURLField = value; }
        }

        public string NombreEnConsulta
        {
            get { return _nombreEnConsulta; }
            set { _nombreEnConsulta = value; }
        }

        public string NombreEnWeb
        {
            get { return _nombreEnWeb; }
            set { _nombreEnWeb = value; }
        }
        
        public string Formato
        {
            get { return _formato; }
            set { _formato = value; }
        }

        public bool EsDatoDeNavegacion
        {
            get { return _esDatoDeNavegacion; }
            set { _esDatoDeNavegacion = value; }
        }

        public bool EsBoundField
        {
            get { return _esBoundField; }
            set { _esBoundField = value; }
        }

        public bool EsHyperlinkField
        {
            get { return _esHyperlinkField; }
            set { _esHyperlinkField = value; }
        }
        #endregion

        #region Constructores
        public CampoReporte(string nombreEnConsulta, string nombreEnWeb, string formato, bool esURLField, bool esDatoDeNavegacion, bool esBoundField, bool esHyperlinkField)
        {
            _nombreEnConsulta = nombreEnConsulta;
            _nombreEnWeb = nombreEnWeb;
            _formato = formato;
            _esURLField = esURLField;
            _esDatoDeNavegacion = esDatoDeNavegacion;
            _esBoundField = esBoundField;
            _esHyperlinkField = esHyperlinkField;
        }

        #endregion

        #region Metodos

        public override string ToString()
        {
            return _nombreEnWeb;
        }

        #endregion
    }
}
