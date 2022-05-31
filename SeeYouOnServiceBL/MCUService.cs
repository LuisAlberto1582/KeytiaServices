using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KeytiaServiceBL;
using SeeYouOnCOM;

namespace SeeYouOnServiceBL
{

    public class MCUService
    {
        protected bool pbSigueCorriendo;
        public void Start()
        {
            pbSigueCorriendo = true;
            DSODataContext.SetContext();

            while (pbSigueCorriendo)
            {
                //espera N seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < Util.TiempoPausa("MCUSync") / 2 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);

                try
                {
                    ActualizaMCU();
                    ActualizaCtasMOVI();
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al sincronizar.", ex);
                }

                //espera N seg, pero si llega señal de terminar, termina la espera para salir
                for (int i = 0; i < Util.TiempoPausa("MCUSync") / 2 && pbSigueCorriendo; i++)
                    System.Threading.Thread.Sleep(1000);
            }
        }

        public void ActualizaMCU()
        {
            SyncCOM loCom = new SyncCOM();
            loCom.SyncMCUs(88235);
        }

        public void ActualizaCtasMOVI()
        {
            SyncCOM loCom = new SyncCOM();
            loCom.RemoveCtasMOVI(88235);
        }

        public void Stop()
        {
            pbSigueCorriendo = false;
        }

    }
}
