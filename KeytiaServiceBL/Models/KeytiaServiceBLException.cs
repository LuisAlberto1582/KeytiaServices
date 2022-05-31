using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class KeytiaServiceBLException : ArgumentException
    {
        private string _claveEstatusCarga = DiccMens.LL109;

        public string ClaveEstatusCarga
        {
            get
            {
                return _claveEstatusCarga;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _claveEstatusCarga = value;
            }
        }

        public KeytiaServiceBLException(string message, Exception innerException)
            : this(DiccMens.LL109, message, innerException)
        {
        }

        public KeytiaServiceBLException(string claveEstatusCarga, string message, Exception innerException)
        {
            this.ClaveEstatusCarga = claveEstatusCarga;
        }
    }
}
