using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Security.Cryptography;

namespace KeytiaUtilLib
{
    public class KeytiaCrypto
    {
        #region Encriptar y Desencriptar
        //La clase TripleDESCryptoServiceProvider crea el mecanismo de encriptación 
        private static TripleDESCryptoServiceProvider objDES = new TripleDESCryptoServiceProvider();
        //Una clave Key y un vector de inicializacion iv 
        private static byte[] key = { 86, 67, 69, 68, 65, 83, 32, 69, 82, 65, 87, 84, 70, 79, 83, 32, 69, 82, 65, 87, 65, 76, 69, 68 }; //VCEDAS ERAWTFOS ERAWALED
        private static byte[] iv = { 68, 69, 76, 65, 87, 65, 82, 69 }; //DELAWARE

        public static string Encrypt(string lsTexto)
        {

            //Un objeto ICryptoTransform que encripte los datos
            ICryptoTransform objCrypto = objDES.CreateEncryptor(key, iv);

            // Create a memory stream.     
            MemoryStream ms = new MemoryStream();

            // Create a CryptoStream using the memory stream and the CSP DES key.  
            CryptoStream encStream = new CryptoStream(ms, objCrypto, CryptoStreamMode.Write);

            // Create a StreamWriter to write a string to the stream.
            StreamWriter sw = new StreamWriter(encStream);

            // Write the text to the stream.
            sw.WriteLine(lsTexto);

            // Close the StreamWriter and CryptoStream.
            sw.Close();
            encStream.Close();

            // Get an array of bytes that represents the memory stream.
            byte[] lbBuffer = ms.ToArray();

            // Close the memory stream.
            ms.Close();

            // Gonvert an array of bytes to string.
            string lsTextoEncripado = Convert.ToBase64String(lbBuffer);

            // Return the encrypted string.
            return lsTextoEncripado;

        }
        public static string Decrypt(string lsTextoEncriptado)
        {
            string lsTextoDesEncriptado = "";

            if (lsTextoEncriptado != null && lsTextoEncriptado != "")
            {
                //Un objeto ICryptoTransform que desencripte los datos
                ICryptoTransform objCrypto = objDES.CreateDecryptor(key, iv);

                // Create a memory stream to the passed buffer.
                byte[] CypherText = Convert.FromBase64String(lsTextoEncriptado);

                MemoryStream ms = new MemoryStream(CypherText);

                // Create a CryptoStream using the memory stream and the CSP DES key. 
                CryptoStream encStream = new CryptoStream(ms, objCrypto, CryptoStreamMode.Read);

                // Create a StreamReader for reading the stream.
                StreamReader sr = new StreamReader(encStream);
                // Read the stream as a string.
                lsTextoDesEncriptado = sr.ReadLine();

                // Close the streams.
                sr.Close();
                encStream.Close();
                ms.Close();
            }

            return lsTextoDesEncriptado;
        }

        public static string Encrypt(string lsTexto, bool utilizaURLEncode)
        {
            if (utilizaURLEncode)
            {
                //Codifica el texto encriptado para que se pueda utilizar como parámetro en un URL
                lsTexto = HttpUtility.UrlEncode(Encrypt(lsTexto));
            }
            else
            {
                lsTexto = Encrypt(lsTexto);
            }

            return lsTexto;
        }

        public static string Decrypt(string lsTextoEncriptado, bool utilizaURLEncode)
        {
            if (utilizaURLEncode)
            {
                lsTextoEncriptado = Decrypt(HttpUtility.UrlDecode(lsTextoEncriptado));
            }
            else
            {
                lsTextoEncriptado = Decrypt(lsTextoEncriptado);
            }

            return lsTextoEncriptado;
        }

        #endregion
    }
}
