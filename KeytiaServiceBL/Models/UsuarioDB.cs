using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class UsuarioDB
    {
        private string _connStr;
        private string _servidor;
        private string _baseDatos;

        public string Servidor
        {
            get
            {
                return _servidor;
            }
            set
            {
                _servidor = value;
                _servidor = _servidor.Replace("DTIMTYKEYTIA01", "192.168.2.159")
                                     .Replace("DTIMTYKEYTIA02", "192.168.2.155")
                                     .Replace("DTIMTYDEV", "192.168.1.157")
                                     .Replace("KEYTIA-DB", "10.202.1.55")
                                     .Replace("DTIBANORTES01", "15.128.19.221");
            }
        }
        public string BaseDatos
        {
            get
            {
                return _baseDatos;
            }
            set
            {
                _baseDatos = value;
            }
        }

        //Construye la propiedad ConnStr a partir de lo que se tiene configurado en la base de datos 
        //en el campo ConnStr de la vista de UsuarDB, y el nombre de servidor y base de datos 
        //se obtienen desde la consulta por SQL a usuarioDB (campos añadidos en la consulta)
        public string ConnStr
        {
            get
            {
                return _connStr;
            }
            set
            {
                try
                {
                    //Las propiedades Servidor y BaseDatos se obtienen por comando al ejecutar la consulta de SQL
                    _connStr = ("Data Source=" + this.Servidor + ";" + Util.Decrypt(value) + ";").Replace("{A};", "Initial Catalog=" + this.BaseDatos);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(DiccMens.DA001, ex);
                }
            }
        }

        public int ICodRegistro { get; set; }
        public int ICodCatalogo { get; set; }
        public int ICodMaestro { get; set; }
        public string VchCodigo { get; set; }
        public string VchDescripcion { get; set; }
        public int Moneda { get; set; }
        public int BanderasUsuarDB { get; set; }

        public string Esquema { get; set; }
        public string SaveFolder { get; set; }
        public string ServidorServicio { get; set; }
        public DateTime DtIniVigencia { get; set; }
        public DateTime DtFinVigencia { get; set; }
        public int ICodUsuario { get; set; }
        public DateTime DtFecUltAct { get; set; }


    }
}
