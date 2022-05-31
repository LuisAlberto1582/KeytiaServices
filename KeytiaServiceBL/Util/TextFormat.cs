using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL
{
    public static class TextFormat
    {
        /// <summary>
        /// Toma los últimos N caracteres de la derecha, del texto recibido
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="numeroCaracteres"></param>
        /// <returns></returns>
        public static string Right(string lsTextoOriginal, int lsNumeroCaracteres)
        {
            string lsRight = lsTextoOriginal;

            try
            {
                if (lsTextoOriginal.Length >= lsNumeroCaracteres)
                {
                    lsRight = lsTextoOriginal.Substring(lsTextoOriginal.Length - lsNumeroCaracteres, lsNumeroCaracteres);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lsRight;
        }
    }
}
