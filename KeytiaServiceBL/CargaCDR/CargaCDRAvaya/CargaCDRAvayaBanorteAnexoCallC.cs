using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KeytiaServiceBL.Models.Cargas;

namespace KeytiaServiceBL.CargaCDR.CargaCDRAvaya
{
    public class CargaCDRAvayaBanorteAnexoCallC : CargaCDRAvaya
    {
        public CargaCDRAvayaBanorteAnexoCallC()
        {
            piColumnas = 10;
            piDate = 0;
            piTime = 1;
            piDuration = 4;
            piCodeUsed = 7;
            piInTrkCode = int.MinValue;
            piCodeDial = int.MinValue;
            piCallingNum = 2;
            piDialedNumber = 3;
            piAuthCode = 8;
            piInCrtID = 9;
            piOutCrtID = int.MinValue;

        }

        protected override void ActualizarCampos()
        {
            ActualizarCamposSitio();
        }

        /// <summary>
        /// Calcula la duración en minutos a partir del número de segundos
        /// Banorte nos pide que cualquier duración mayor a 360 minutos sea
        /// tratada como si fuera de 3 minutos
        /// </summary>
        protected override int DuracionMin
        {
            get
            {
                return piDuracionMin;
            }
            set
            {
                piDuracionMin = (int)Math.Ceiling(value / 60.0);

                if (piDuracionMin >= 360)
                {
                    piDuracionMin = 3;
                }
            }
        }


        protected override bool ValidarRegistro()
        {
            bool lbValidaReg = true;
            DataRow[] ldrCargPrev;
            DateTime ldtFecha;
            int liAux;
            pbEsLlamPosiblementeYaTasada = false;
            

            if (psCDR == null || psCDR.Length != piColumnas)    // Formato Incorrecto 
            {
                psMensajePendiente.Append("[Formato registro incorrecto]");
                lbValidaReg = false;
                return lbValidaReg;
            }

            if (!EsRegistroNoDuplicado())
            {
                lbValidaReg = false;
                return lbValidaReg;
            }

            if (psCDR[piDuration].Trim().Length != 4) // Duracion Incorrecta
            {
                psMensajePendiente.Append("[Longitud de Duracion Incorrecta, <> 4]");
                lbValidaReg = false;
                return lbValidaReg;
            }

            if (psCDR[piDate].Trim().Length != 6) // Fecha Incorrecta
            {
                psMensajePendiente.Append("[Longitud de Fecha Incorrecta, <> 6]");
                lbValidaReg = false;
                return lbValidaReg;
            }

            ldtFecha = Util.IsDate(psCDR[piDate].Trim(), "MMddyy");

            if (ldtFecha == DateTime.MinValue)  // Fecha Incorrecta
            {
                psMensajePendiente.Append("[Formato de fecha incorrecta]");
                lbValidaReg = false;
                return lbValidaReg;
            }

            if (psCDR[piTime].Trim().Length != 4)  // Hora Incorrecta
            {
                psMensajePendiente.Append("[Longitud de hora incorrecta, <> 4]");
                lbValidaReg = false;
                return lbValidaReg;
            }

            pdtFecha = Util.IsDate(psCDR[piDate].Trim() + " " + psCDR[piTime].Trim(), "MMddyy HHmm");

            if (pdtFecha == DateTime.MinValue)  // Hora Incorrecta
            {
                psMensajePendiente.Append("[Formato de hora incorrecta]");
                lbValidaReg = false;
                return lbValidaReg;
            }

            if (piCodeUsed != int.MinValue && piInTrkCode != int.MinValue)
            {
                //if (!int.TryParse(psCDR[piCodeUsed].Trim(), out liAux) || !int.TryParse(psCDR[piInTrkCode].Trim(), out liAux)) // No se pueden identificar grupos troncales
                //{
                //    return false;
                //}
            }

            liAux = DuracionSec(psCDR[piDuration].Trim());

            //RZ.20121025 Tasa Llamadas con Duracion 0 (Configuración Nivel Sitio)
            if (liAux == 0 && pbProcesaDuracionCero == false)
            {
                psMensajePendiente.Append("[Duracion incorrecta, 0]");
                lbValidaReg = false;
                return lbValidaReg;
            }

            //Validar que la fecha no esté dentro de otro archivo
            pdtDuracion = pdtFecha.AddSeconds(liAux);

            //Validar que la fecha no esté dentro de otro archivo
            List<CargasCDR> llCargasCDRConFechasDelArchivo =
                plCargasCDRPrevias.Where(x => x.IniTasacion <= pdtFecha &&
                    x.FinTasacion >= pdtFecha &&
                    x.DurTasacion >= pdtDuracion).ToList<CargasCDR>();

            if (llCargasCDRConFechasDelArchivo != null && llCargasCDRConFechasDelArchivo.Count > 0)
            {
                pbEsLlamPosiblementeYaTasada = true;
                foreach (CargasCDR lCargaCDR in llCargasCDRConFechasDelArchivo)
                {
                    if (!plCargasCDRConFechasDelArchivo.Contains(lCargaCDR))
                    {
                        plCargasCDRConFechasDelArchivo.Add(lCargaCDR);
                    }
                }
            }

            if (!ValidarRegistroSitio())
            {
                lbValidaReg = false;
                return lbValidaReg;
            }

            return lbValidaReg;
        }
    }
}
