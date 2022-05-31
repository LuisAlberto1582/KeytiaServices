using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeYouOnServiceBL
{
    public class Service
    {
        SeeYouOnServiceBL.TMSService poTMSService;
        System.Threading.Thread poThreadTMSService;

        SeeYouOnServiceBL.MCUService poMCUService;
        System.Threading.Thread poThreadMCUService;

        //RZ.20131030 
        SeeYouOnServiceBL.LanzadorCargasSYO poLanzadorCargas;
        System.Threading.Thread poThreadCargasSYO;

        public void Start()
        {
            poTMSService = new SeeYouOnServiceBL.TMSService();
            poThreadTMSService = new System.Threading.Thread(poTMSService.Start);
            poThreadTMSService.Start();

            // Monitorea las conferencias de las salas virtuales
            poMCUService = new SeeYouOnServiceBL.MCUService();
            poThreadMCUService = new System.Threading.Thread(poMCUService.Start);
            poThreadMCUService.Start();

            //RZ.20131030 Lanzador para cargas en SYO
            poLanzadorCargas = new SeeYouOnServiceBL.LanzadorCargasSYO();
            poThreadCargasSYO = new System.Threading.Thread(poLanzadorCargas.Start);
            poThreadCargasSYO.Start();
        }

        public void Stop()
        {
            int liCount;

            if (poTMSService != null)
                poTMSService.Stop();

            if (poThreadMCUService != null)
            {
                liCount = 0;

                while (poThreadMCUService.IsAlive && liCount < 60)
                {
                    System.Threading.Thread.Sleep(1000);
                    liCount++;
                }

                if (poThreadMCUService.IsAlive)
                    poThreadMCUService.Interrupt();
            }

            poTMSService = null;
            poThreadMCUService = null;


            // Detener el Monitoreo de MCU para salas virtuales
            if (poMCUService != null)
                poMCUService.Stop();

            if (poThreadMCUService != null)
            {
                liCount = 0;

                while (poThreadMCUService.IsAlive && liCount < 60)
                {
                    System.Threading.Thread.Sleep(1000);
                    liCount++;
                }

                if (poThreadMCUService.IsAlive)
                    poThreadMCUService.Interrupt();
            }

            poMCUService = null;
            poThreadMCUService = null;

            //RZ.20131030 Detener el lanzador de Cargas en SYO
            if (poLanzadorCargas != null)
            {
                poLanzadorCargas.Stop();
            }

            if (poThreadCargasSYO != null)
            {
                liCount = 0;

                while (poThreadCargasSYO.IsAlive && liCount < 60)
                {
                    System.Threading.Thread.Sleep(1000);
                    liCount++;
                }

                if (poThreadCargasSYO.IsAlive)
                {
                    poThreadCargasSYO.Interrupt();
                }
            }

            poLanzadorCargas = null;
            poThreadCargasSYO = null;
        }
    
    }
}
