using System;
using System.Diagnostics;
using System.IO;

namespace KeytiaServiceBL.UtilCarga
{
    public static class BCP
    {
        public static void BulkQueryout(string outputFileName, string sentence, string server, string us, string pass)
        {
            try
            {
                //string Query = "exec keytia5..SPDetalladosCamposRespaldo 'KIOC12','2021-10-01 00:00:00','2021-10-31 23:59:59'";
                //string NombreDestino = "D:\\K5\\KeytiaService\\tempfile\\TempArchive.txt";
                string usuario = "usrDesarrollo";
                string password = "kt14$Des4";
                string servidor = "10.202.1.55";
                string bcpCMD = "bcp \"" + sentence + "\" queryout \"" + outputFileName + "\" -c -t\"|\" -r\\n  -U " + usuario + " -P " + password + " -S " + servidor;

                //ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + bcpCMD);
                //string bcp = "bcp " + bd + ".." + nombre_tabla + " in \"" + path + "\" -b 10000 -m 30 -c -t, -r\\n -o dos.out -U " + us + " -P " + pass + " -S " + server;
                //string.Format("bcp {0} queryout \"{1}\" -b 10000 -m 30 -c -C 1252 -t\"|\" -r\\n -o dos.out -U {2} -P {3} -S {4}", sentence, outputFileName, us, pass, server);
                // string bcp = string.Format("bcp {0} queryout \"{1}\" -t\"|\" -r\\n  -U {2} -P {3} -S {4}", sentence, outputFileName, us, pass, server);
                //Indicamos que deseamos inicializar el proceso cmd.exe junto a un comando de arranque. 
                //(/C, le indicamos al proceso cmd que deseamos que cuando termine la tarea asignada se cierre el proceso).
                //Para mas informacion consulte la ayuda de la consola con cmd.exe /? 
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + bcpCMD)
                {
                    // Indicamos que la salida del proceso se redireccione en un Stream
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
                proc.Close();
                proc.Dispose(); 
            }
            catch (FileNotFoundException ex)
            {
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Ocurrió un error al tratar de descargar la información.");
            }

        }

        public static void BulkUpload(string path, string nombre_tabla, string server, string bd, string us, string pass, string esquema)
        {
            try
            {
                //string bcp = "bcp " + bd + ".." + nombre_tabla + " in \"" + path + "\" -b 10000 -m 30 -c -t, -r\\n -o dos.out -U " + us + " -P " + pass + " -S " + server;
                string bcp = string.Format("bcp {0}.{1}.{2} in \"{3}\" -b 10000 -m 30 -c -C 1252 -t, -r\\n -o dos.out -U {4} -P {5} -S {6}", bd, esquema, nombre_tabla, path, us, pass, server);
                //Indicamos que deseamos inicializar el proceso cmd.exe junto a un comando de arranque. 
                //(/C, le indicamos al proceso cmd que deseamos que cuando termine la tarea asignada se cierre el proceso).
                //Para mas informacion consulte la ayuda de la consola con cmd.exe /? 
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + bcp);
                // Indicamos que la salida del proceso se redireccione en un Stream
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                //Indica que el proceso no despliegue una pantalla negra (El proceso se ejecuta en background)
                procStartInfo.CreateNoWindow = false;
                //Inicializa el proceso
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                //Consigue la salida de la Consola(Stream) y devuelve una cadena de texto
                string result = proc.StandardOutput.ReadToEnd();
            }
            catch (FileNotFoundException ex)
            {
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("ocurrio un error al realizar la carga de informacion de la tabla: " + nombre_tabla);
            }

        }
    }

}
