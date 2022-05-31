using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KeytiaServiceBL;
using SeeYouOnCOM;

namespace SeeYouOnServiceBL
{
    public class TMSService
    {
        protected bool pbSigueCorriendo;

        public void Start()
        {
            pbSigueCorriendo = true;
            DSODataContext.SetContext();

            while (pbSigueCorriendo)
            {
                //espera N seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < Util.TiempoPausa("TMSSync") / 2 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);

                try
                {
                    SyncCOM loCom = new SyncCOM();
                    loCom.SyncTMSs(88235);
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al sincronizar.", ex);
                }

                //espera N seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < Util.TiempoPausa("TMSSync") / 2 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            pbSigueCorriendo = false;
        }
    }
}
