using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeytiaServiceBL;
using System.Configuration;
using System.IO;
using System.Collections;

namespace KeytiaServiceTester
{
    public partial class AltaUsuarNextel : Form
    {
        public AltaUsuarNextel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable ldtEmple = new DataTable();
            StringBuilder lsbQuery = new StringBuilder();
            int liCatUsuarDB;
            int liCatEmpre;
            string psConexionConfig;
            Hashtable lhtUsuario = new Hashtable();
            Hashtable lhtTabla = new Hashtable();
            FileStream stream = new FileStream("LogUsuar.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter swLog = new StreamWriter(stream);
            string psTablaUsuarK5;
            string psTablaUsuarK3;
            string psTablaEmple;

            progressBar1.Maximum = 10;
            progressBar1.Minimum = 0;

            setMessageLabelWait("Colectando informacion previa... ");
            
            psConexionConfig = ConfigurationManager.AppSettings["appConnectionString"].ToString();
            psTablaUsuarK5 = ConfigurationManager.AppSettings["TablaUsuarK5"].ToString();
            psTablaUsuarK3 = ConfigurationManager.AppSettings["TablaUsuarK3"].ToString();
            psTablaEmple = ConfigurationManager.AppSettings["TablaEmple"].ToString();

            lsbQuery.Length = 0;
            lsbQuery.Append("select iCodCatalogo from [VisHistoricos('UsuarDB','Usuarios DB','Español')] \r");
            lsbQuery.Append("where dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("and vchCodigo like 'Nextel'");

            setMessageLabelWait("Consultando usuario DB para Nextel ");

            liCatUsuarDB = (int)Util.IsDBNull(DSODataAccess.ExecuteScalar(lsbQuery.ToString(), psConexionConfig), int.MinValue);

            progressBar1.Value = progressBar1.Maximum / 3;
            progressBar1.Update();
            System.Threading.Thread.Sleep(2000);

            if (liCatUsuarDB != int.MinValue)
            {
                DSODataContext.SetContext(liCatUsuarDB); //Nextel

                setMessageLabelWait("Consultando empresa Nextel para usuarios");

                lsbQuery.Length = 0;
                lsbQuery.Append("select iCodCatalogo from [VisHistoricos('Empre','Empresas','Español')] \r");
                lsbQuery.Append("where dtIniVigencia <> dtFinVigencia \r");
                lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
                lsbQuery.Append("and vchDescripcion like 'Nextel'");

                liCatEmpre = (int)Util.IsDBNull(DSODataAccess.ExecuteScalar(lsbQuery.ToString()), int.MinValue);

                progressBar1.Value = progressBar1.Value * 2;
                progressBar1.Update();
                System.Threading.Thread.Sleep(3000);

                if (liCatEmpre == int.MinValue)
                {
                    progressBar1.Value = progressBar1.Maximum;
                    progressBar1.Update();
                    MessageBox.Show("No se encontro la empresa Nextel para el esquema");
                    return;
                }

                setMessageLabelWait("Obteniendo usuarios a crear...");

                lsbQuery.Length = 0;
                lsbQuery.Append("select US_Cuenta, US_Nombre, dtIniVigencia = '2011-01-01 00:00:00.000', dtFinVigencia = '2079-01-01 00:00:00.000', \r");
                lsbQuery.Append("Email = EM_Nomina + '@nextel.com', EM_Nomina \r");
                lsbQuery.Append("from " + psTablaUsuarK3 + " uk3, " + psTablaEmple + " ek3 \r");
                lsbQuery.Append("where uk3.us_borrado = 'N'  \r");
                lsbQuery.Append("and uk3.us_cuenta not in (select usuario from " + psTablaUsuarK5 + ") \r");
                lsbQuery.Append("and uk3.ut_idf = 1 \r");
                lsbQuery.Append("and uk3.em_idf = ek3.em_id \r");
                lsbQuery.Append("and ek3.em_fechabaja >= GETDATE() \r");
                
                ldtEmple = DSODataAccess.Execute(lsbQuery.ToString());

                progressBar1.Value = progressBar1.Maximum;
                progressBar1.Update();
                System.Threading.Thread.Sleep(3000);

                if (ldtEmple.Rows.Count > 0)
                {
                    int cantUsuar = ldtEmple.Rows.Count;
                    int counter = 1;
                    progressBar1.Refresh();
                    progressBar1.Minimum = 0;
                    //progressBar1.Value = progressBar1.Minimum;
                    progressBar1.Maximum = cantUsuar;
                    //progressBar1.Step = 1;
                    setMessageLabelWait("Cantidad de usuarios a crear: " + cantUsuar.ToString());

                    swLog.WriteLine("Proceso para creacion de usuarios inicia: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    swLog.WriteLine(label1.Text);
                    swLog.WriteLine("Usuario | Nomina | Nombre Emple | Estatus ");
                     
                    foreach (DataRow row in ldtEmple.Rows)
                    {
                        string lsErrorUsuario = String.Empty;

                        progressBar1.Value = counter;
                        progressBar1.Update();

                        setMessageLabelWait("Creando usuario " + counter + " de " + cantUsuar.ToString());
                        //swLog.WriteLine(label1.Text);

                        lhtTabla.Clear();
                        lhtUsuario.Clear();

                        lhtTabla.Add("vchCodigoUsuario", row["US_Cuenta"].ToString());
                        lhtTabla.Add("dtIniVigencia", row["dtIniVigencia"]);
                        lhtTabla.Add("dtFinVigencia", row["dtFinVigencia"]);
                        lhtTabla.Add("{Email}", row["Email"]);
                        lhtTabla.Add("{Empre}", liCatEmpre.ToString());
                        lhtTabla.Add("vchDescripcion", row["US_Nombre"]);
                        lhtTabla.Add("{NominaA}", row["EM_Nomina"]);

                        /*RZ. Las siguientes llaves en el hash hacen lo siguiente:
                                
                         * HomePage = Si se la incluimos lo que hace es establecer como HomePage
                                del usuario a crear y no toma encuenta la que viene fija en la clase Usuar,
                                la clase esta modificada solo en codigo de desarrollo (d:/k5/Desarrollo)
                         //inicio config ~/UserInterface/Dashboard/Dashboard.aspx?Opc=OpcDshAdmin
                         //inicio emple ~/UserInterface/Dashboard/Dashboard.aspx?Opc=OpcdshEmpleado
                         
                         * Perfil = si se la incluimos al hash lo que hace es establecer como perfil 
                                del usuario a crear y no tomar el de Empleado que es el que obtiene en la 
                                clase Usuario, la clase esta modificada solo en codigo de desarrollo (d:/k5/Desarrollo)
                         // perfil Empleado 370 Config 367
                         
                         * Password = Es el password encriptado, si se lo incluimos al usuario a crear y este 
                                es valido, lo establecera como password al usuario a crear, si no es asi calcula un 
                                password generico.
                                
                         */

                        
                        //lhtTabla.Add("{HomePage}", "~/UserInterface/Dashboard/Dashboard.aspx?Opc=OpcDshAdmin");
                        
                        //lhtTabla.Add("{Perfil}", 367); 

                        string lsPwdUsuario = KeytiaServiceBL.Util.Encrypt("nextel");
                        lhtTabla.Add("{Password}", lsPwdUsuario);
                        
                        Usuarios loUsuario = new Usuarios(liCatUsuarDB);
                        int liCodCatalogoUsuario = 0;

                        swLog.Write(lhtTabla["vchCodigoUsuario"].ToString() + " | " 
                            + lhtTabla["{NominaA}"].ToString() + " | " 
                            + lhtTabla["vchDescripcion"].ToString() + " | ");

                        try
                        {
                            liCodCatalogoUsuario = loUsuario.GeneraUsuario(2, lhtTabla, out lhtUsuario, out lsErrorUsuario);
                        }
                        catch (Exception ex)
                        {

                            swLog.WriteLine(ex.ToString());
                        }

                        if (liCodCatalogoUsuario > 0)
                        {
                            swLog.Write(" es OK \n");

                            lsbQuery.Length = 0;
                            lsbQuery.Append("UPDATE [VisHistoricos('Emple','Empleados','Español')] \r");
                            lsbQuery.Append("SET Usuar = " + liCodCatalogoUsuario.ToString() + ", iCodUsuario = 340, dtFecUltAct = GETDATE() \r");
                            lsbQuery.Append("WHERE vchCodigo = " + row["EM_Nomina"].ToString());
                            lsbQuery.Append("and dtFinVigencia >= getdate()");

                            bool lbSiActualizo = DSODataAccess.ExecuteNonQuery(lsbQuery.ToString());

                            if (!lbSiActualizo)
                            {
                                swLog.WriteLine("Ocurrio un error al actualizar el usuario");
                            }

                        }
                        else
                        {
                            swLog.Write(" El usuario no pudo crearse: " + lsErrorUsuario + " \n");
                        }

                        counter++;
                    }

                    swLog.WriteLine("Fin de creacion de usuarios en: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    swLog.Close();

                    MessageBox.Show("Fin de creacion de usuarios.");
                }
                else
                {
                    progressBar1.Value = progressBar1.Maximum;
                    progressBar1.Update();
                    MessageBox.Show("No se encontraron usuarios para crear");
                    return;
                }

            }
            else
            {
                progressBar1.Value = progressBar1.Maximum;
                progressBar1.Update();
                MessageBox.Show("No se pudo encontrar el Usuario DB para Modelo");
                return;
            }

            
        }

        private void setMessageLabelWait(string message)
        { 
            label1.Text = message;
            Application.DoEvents();
            System.Threading.Thread.Sleep(100);
        }
    }
}
