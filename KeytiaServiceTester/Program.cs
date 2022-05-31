using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;


namespace KeytiaServiceTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        //static void Main(string[] args)
        //{
        //    try
        //    {
        //        //int icodCatUsuarDB = 97909;
        //        //int icodCatCargaCDR = 1441687;

        //        int icodCatUsuarDB = Convert.ToInt32(ConfigurationManager.AppSettings["iCodCatUsuarDB"]);
        //        int icodCatCargaCDR = Convert.ToInt32(ConfigurationManager.AppSettings["iCodCatCargaCDR"]);

        //        KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);

        //        var Carga =
        //            new KeytiaServiceBL.CargaCDR.CargaCDRIPOffice.CargaCDRIPOfficeEagleburgmann();


        //        //Carga.NuevoRegistro += ImprimeEnConsola;
        //        Carga.CodUsuarioDB = icodCatUsuarDB;
        //        Carga.CodCarga = icodCatCargaCDR;

        //        Carga.IniciarCarga();

        //        Console.WriteLine("Carga finalizada.");

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.Read();
        //        throw ex;
        //    }

        //    Console.WriteLine("Presione <enter> para terminar...");
        //    Console.Read();
        //}




        static void ImprimeEnConsola(object sender, KeytiaServiceBL.CargaCDR.NuevoRegistroEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Registro: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(string.Format("{0}", e.RegCarga.ToString()));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" procesado. ");

        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new UnitTestCargas().cargarCarga();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form5());
            //Application.Run(new CargasTIMTester());

            //Application.Run(new CargaArchivoXML());
            //Application.Run(new CargaMasivaArchivosXML());

            //Application.Run(new CargaMasivaArchivosDatosFactura());
        }
    }
}
