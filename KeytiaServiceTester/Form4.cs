using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using KeytiaServiceBL;
using System.Runtime.InteropServices;

namespace KeytiaServiceTester
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            //System.IO.DirectoryInfo ldiDir = new System.IO.DirectoryInfo("path");

            //System.Activator.CreateInstanceFrom("", "");

            //System.IO.StreamReader tr = new System.IO.StreamReader("arch");
            //tr.Close();
            //tr.ReadLine();

            ////System.Collections.Arraylis
            //string[] lsValores;
            ////lsValores = Array.CreateInstance(System.Type.GetType("System.String"), 1);

            ////Microsoft.VisualBasic.info

            ////dt.rows;
            ////ldiDir.file;

            //System.Collections.ArrayList arr;

            //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();//
            //System.Xml.XmlNodeList nl;//
            //doc.Load("xxx");//
            //nl = doc.SelectNodes("xxx");//
            ////nl;

            ////arr.Add(

            //System.Collections.Hashtable ht;
            //ht.keys();
            //ht.

            //string s;
            ////s.len

            //System.Data.DataTable dt; //
            //DataColumn[] v2;
            //DataColumn[] v = { dt.Columns["col"] };
            //dt.PrimaryKey = v;

            //System.Threading.Thread.sle ep;
            //DateTime dti;
            //dti.DayOfWeek = DayOfWeek.Friday;

            //MessageBox.Show(DayOfWeek.Friday.ToString());

            //int[] arr2;
            //arr2 = new int[0];
            //arr2[0] = 0;

            //StringBuilder sb = new StringBuilder();
            //sb.Append("");
            //sb.Replace 

            //string s;
            //DateTime dtime;
            //dtime = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KDBAccess kdb = new KDBAccess();
            DataTable dt;
            Hashtable ht;
            string s;

            //System.Collections.ArrayList a;

            //dt = kdb.GetHisRegByCod(new string[] { "0010" });

            //DSODataContext.SetContext(0);
            //dt = kdb.GetHisRegByEnt("RepEstIdiomaCmp", "Tipos de data source");

            DSODataContext.SetContext(77215);
            dt = kdb.GetHisRegByEnt("Restricciones", "Centro Costos");

            //dt = kdb.GetHisRegByEnt("Directorio", "Directorio Telefonico");
            //dt = kdb.GetHisRegByEnt("Region", "Regiones", new string[] { "iCodRegistro", "{Español}", "{Ingles}", "{Frances}", "{Aleman}" });
            //dt = kdb.GetHisRegByEnt("Cat.Emp", "", new string[] { "iCodRegistro", "{Nom.}", "{Ape}" });
            //dt = kdb.GetHisRegByEnt("Cat.Emp", "", new string[] { "{Nom.}", "{Ape}" }, "{Puesto} like '%Jefe%'", "{Ape}");
            //dt = kdb.GetHisRegByEnt("Cat.Emp", "", "{Puesto} like '%Jefe%'", "{Ape}");

            //dt = kdb.GetHisRegByRel("Empleado - Centro Costos", "Cat.Emp");
            //dt = kdb.GetHisRegByRel("Empleado - Centro Costos", "Cat.Emp", "isnull({FlagCat.Emp}, 0) & 1 <> 0");

            //ht = new Hashtable();
            ////ht.Add("Cat.Cos", 10);
            //ht.Add("Cat.Emp", 8);
            //kdb.FechaVigencia = DateTime.Today.AddYears(-1);
            //dt = kdb.GetHisRegByRel("Empleado - Centro Costos", "Cat.Emp", "isnull({FlagCat.Emp}, 0) & 1 <> 0", ht);

            //dt = kdb.ExecuteQuery("Cat.Emp", "Empleados M", "select * from historicos where {Nom.} = 'Rodrigo'");

            //ht = new Hashtable();
            //ht.Add("{Nom.}", "Rolando");
            //ht.Add("{Ape}", "Ramirez");
            //ht.Add("{Puesto}", "Developer");
            ////kdb.Insert("historicos", "Cat.Emp", "Empleados M", ht);

            //ht = new Hashtable();
            //ht.Add("vchDescripcion", "Rolando Ramirez");
            //kdb.Update("historicos", "Cat.Emp", "Empleados M", ht, 1);

            //dt = kdb.GetHisRegByEnt("xx", "yy");
            //dt = kdb.GetHisRegByEnt("Sitio", "Sitio - Cisco", "iCodCatalogo = 72218");
            //dt = kdb.GetHisRegByEnt("Detall", "DetalleFacturaATelmexRyOC");

            //ht = new Hashtable();
            //ht.Add("{ClaveCar}", 1);
            //ht.Add("{Linea}", 1);
            //ht.Add("{CtaMae}", 1);
            //ht.Add("{Ident}", 1);
            //ht.Add("{CveCargo}", 1);
            //ht.Add("{CustID}", 1);
            //ht.Add("{TpLlam}", 1);
            //ht.Add("{CdOrig}", 1);
            //ht.Add("{CdDest}", 1);
            //ht.Add("{TelDest}", 1);
            //ht.Add("{Tarifa}", 1);
            //ht.Add("{Jurisd}", 1);
            //ht.Add("{RegCarga}", 1);
            //ht.Add("{Cantidad}", 1);
            //ht.Add("{Ciclo}", 1);
            //ht.Add("{Importe}", 1);
            //ht.Add("{DuracionSeg}", 1 * 60.0);
            //ht.Add("{FechaFactura}", 1);
            //ht.Add("{FechaInicio}", 1);
            //ht.Add("{HoraInicio}", 1);

            //dt = kdb.GetHisRegByRel("Carrier-ExcepcionLinea", "Linea", "", ht);
            //dt = kdb.GetHisRegByEnt("MarLoc", "Marcacion Localidades", new string[] {"Paises"} );
            //dt = kdb.GetHisRegByEnt("Detall", "DetalleFacturaAAlestraDet");
            //kdb.Insert("detallados", "Detall", "DetalleFacturaAAlestraDet", ht);

            //ht = new Hashtable();
            //ht.Add("{CostoSM}", (double)1);
            //ht.Add("{IP}", (float)1);
            //ht.Add("{Contrato}", (int)1);
            //ht.Add("{CveEtiquet}", true);
            //ht.Add("{CodAuto}", (byte)1);
            //ht.Add("{TDest}", (char)1);
            //ht.Add("{Exten}", (decimal)1);
            //ht.Add("{Tarifa}", (long)1);
            //ht.Add("{Costo}", (short)1);
            //ht.Add("{DuracionMin}", "1");
            //ht.Add("{GpoTroSal}", 1);
            //ht.Add("{CircuitoSal}", 1);
            //ht.Add("{GpoTroEnt}", 1);
            //ht.Add("{TpLlam}", 1);
            //ht.Add("{CostoFact}", 1);
            //ht.Add("{DuracionSeg}", 1);
            //ht.Add("{Locali}", 1);
            //ht.Add("{CodAut}", 1);
            //ht.Add("{CodAcc}", 1);
            //ht.Add("{CircuitoEnt}",1); 
            //ht.Add("{Emple}", 1);
            //ht.Add("{FecHrIni}", 1);
            //ht.Add("{TelDest}", 1);
            //ht.Add("{Cargas}", 1);
            //ht.Add("{FechaHoraFin}", 1);

            ////dt = kdb.GetHisRegByEnt("Detall", "DetalleCDR", new string[] { "{CostoSM}", "{IP}", "{Contrato}", "{CveEtiquet}", "{CodAuto}", "{TDest}", "{Exten}", "{Tarifa}", "{Costo}", "{DuracionMin}", "{GpoTroSal}", "{CircuitoSal}", "{GpoTroEnt}", "{TpLlam}", "{CostoFact}", "{DuracionSeg}", "{Locali}", "{CodAut}", "{CodAcc}", "{CircuitoEnt}", "{Emple}", "{FecHrIni}", "{TelDest}", "{Cargas}", "{FechaHoraFin}" });
            //kdb.Insert("detallados", "Detall", "DetalleCDR", ht);

            //dt = kdb.GetRelRegByDes("Region - Tipo Destino");

            //dt = kdb.GetHisRegByEnt("Cargas", "Cargas");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //KeytiaCOM.CargasCOM ccargaCom = (KeytiaCOM.CargasCOM)Activator.CreateInstance(Type.GetTypeFromProgID("KeytiaCOM.CargasCOM"));
            KeytiaCOM.ICargasCOM ccargaComLoc;
            object o;
            //KeytiaCOM.ICargasCOM ccargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:ComputerName=dsoserver/new:KeytiaCOM.CargasCOM");
            //KeytiaCOM.ICargasCOM ccargaComLoc = (KeytiaCOM.ICargasCOM) Marshal.BindToMoniker("queue:FormatName=DIRECT=OS:dsoserver\\PRIVATE$\\KeytiaCOM/new:KeytiaCOM.CargasCOM");

            Hashtable lht = new Hashtable();

            lht.Add("{Español}", "Region prueba");
            lht.Add("{Ingles}", "Test Zone");
            lht.Add("dtFecUltAct", null);

            o = Activator.CreateInstance(Type.GetTypeFromProgID("KeytiaCOM.CargasCOM"));
            ccargaComLoc = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");
            //ccargaComLoc = new KeytiaCOM.CargasCOM();

            for (int i = 1; i <= 1; i++)
            {
                lht["dtFecUltAct"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //ccargaCom.CargaFacturas(Util.Ht2Xml(lht), "historicos", "Cat.Emp", "Empleados M");

                //ccargaComLoc = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:FormatName=DIRECT=OS:dsoserver\\private$\\KeytiaCOM/new:KeytiaCOM.CargasCOM");
                //ccargaComLoc.CargaFacturas(Util.Ht2Xml(lht), "historicos", "Region", "Regiones");
                //ccargaComLoc.CargaFacturas("<Hashtable><item key=\"{Descripcion}\" value=\"SERV.DIRE.EXT\" /><item key=\"{TpRegFac}\" value=\"6351\" /><item key=\"{RegCarga}\" value=\"50\" /><item key=\"{Troncal}\" value=\"5554202306\" /><item key=\"{Cargas}\" value=\"72238\" /></Hashtable>", "detallados", "Cargas", "DetalleFacturaBTelmexRyOC");

                if (i % 5 != 0)
                {
                    lht = new Hashtable();
                    //lht.Add("{RegCarga}", i);
                    lht.Add("{CostoSM}", (double)1);
                    //lht.Add("{IP}", (float)1);
                    //lht.Add("{Contrato}", (int)1);
                    //lht.Add("{CveEtiquet}", true);
                    //lht.Add("{CodAuto}", (byte)1);
                    //lht.Add("{TDest}", 1);
                    //lht.Add("{Exten}", (decimal)1);
                    //lht.Add("{Tarifa}", (long)1);
                    //lht.Add("{Costo}", (short)1);
                    //lht.Add("{DuracionMin}", "1");
                    //lht.Add("{GpoTroSal}", 1);
                    //lht.Add("{CircuitoSal}", 1);
                    //lht.Add("{GpoTroEnt}", 1);
                    //lht.Add("{TpLlam}", 1);
                    //lht.Add("{CostoFac}", 1);
                    //lht.Add("{DuracionSeg}", 1);
                    //lht.Add("{Locali}", 1);
                    ////lht.Add("{CodAut}", 1);
                    //lht.Add("{CodAcc}", 1);
                    //lht.Add("{CircuitoEnt}", 1);
                    //lht.Add("{Emple}", 1);
                    //lht.Add("{FechaInicio}", DateTime.Now);
                    //lht.Add("{TelDest}", 1);
                    //lht.Add("{Cargas}", 1);
                    lht.Add("{FechaFin}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    ccargaComLoc.CargaFacturas(Util.Ht2Xml(lht), "Pendientes", "Detall", "DetalleCDR", 0);
                }
                else
                {
                    lht = new Hashtable();
                    lht.Add("{RegCarga}", i);
                    lht.Add("{CostoSM}", (double)2);
                    //lht.Add("{IP}", (float)2);
                    //lht.Add("{Contrato}", (int)2);
                    //lht.Add("{CveEtiquet}", false);
                    //lht.Add("{CodAuto}", (byte)2);
                    //lht.Add("{TDest}", 2);
                    //lht.Add("{Exten}", (decimal)2);
                    //lht.Add("{Tarifa}", (long)2);
                    //lht.Add("{Costo}", (short)2);
                    //lht.Add("{DuracionMin}", "2");
                    //lht.Add("{GpoTroSal}", 2);
                    //lht.Add("{CircuitoSal}", 2);
                    //lht.Add("{GpoTroEnt}", 2);
                    //lht.Add("{TpLlam}", 2);
                    //lht.Add("{CostoFac}", 2);
                    //lht.Add("{DuracionSeg}", 2);
                    //lht.Add("{Locali}", 2);
                    ////lht.Add("{CodAut}", 1);
                    //lht.Add("{CodAcc}", 2);
                    //lht.Add("{CircuitoEnt}", 2);
                    //lht.Add("{Emple}", 2);
                    //lht.Add("{FechaInicio}", DateTime.Now.AddYears(-100));
                    //lht.Add("{TelDest}", 2);
                    //lht.Add("{Cargas}", 2);
                    //lht.Add("{FechaFic}", DateTime.Now.AddYears(-100));

                    ccargaComLoc.CargaFacturas(Util.Ht2Xml(lht), "Pendientes", "Detall", "DetalleCDR", 1);
                }

                if (i % 5 == 0)
                {
                    Marshal.ReleaseComObject(ccargaComLoc);
                    ccargaComLoc = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");
                    //ccargaComLoc = new KeytiaCOM.CargasCOM();
                }
            }

            Marshal.ReleaseComObject(ccargaComLoc);

            //KeytiaServiceBL.CargaFacturas.CargaFacturaServicio k = new KeytiaServiceBL.CargaFacturas.CargaFacturaServicio();
            //k.EnviarMensaje(lht, "historicos", "Cat.Emp", "Empleados M");

            MessageBox.Show("Fin");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(textBox2.Text);
            label1.Text = System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, textBox2.Text) ? "1" : "0";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileReaderCSV frcsv = new FileReaderCSV();
            string[] valores;

            frcsv.Abrir("c:\\temp\\K5\\testcsv.txt");

            while ((valores = frcsv.SiguienteRegistro()) != null)
            {
                Console.WriteLine(valores[0]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //FileReaderXML frxml = new FileReaderXML();
            //string[] valores;

            //frxml.Abrir("c:\\temp\\K5\\20110322.XML");

            //while ((valores = frxml.SiguienteRegistro("/CDR_COLLECTION/CDR")) != null)
            //{
            //    foreach (string s in valores)
            //        Console.WriteLine(s);
            //}

            FileReaderXML frxml = new FileReaderXML();
            System.Xml.XmlNamespaceManager xmlns;
            string[] valores;

            frxml.Abrir("c:\\temp\\K5\\Iusacell.XML");

            xmlns = new System.Xml.XmlNamespaceManager(frxml.NameTable);
            xmlns.AddNamespace("ns", "http://www.sat.gob.mx/cfd/2");
            frxml.XmlNS = xmlns;

            while ((valores = frxml.SiguienteRegistro("/ns:Comprobante/ns:Conceptos/ns:Concepto")) != null)
            {
                foreach (string s in valores)
                    Console.WriteLine(s);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FileReaderXLS frxls = new FileReaderXLS();
            string[] valores;

            if (frxls.Abrir("c:\\temp\\K5\\Movistar detalle.xlsx"))
            {
                for (int i = 0; i < 100 && (valores = frxls.SiguienteRegistro()) != null; i++)
                    Console.WriteLine(valores[0]);

                frxls.Cerrar();
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Util.IsDate(textBox1.Text, textBox2.Text).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            KDBAccess kdb = new KDBAccess();
            DataTable dtEnt;
            DataTable dtMae;
            StringBuilder lsbQuery;

            DSODataContext.SetContext(0);
            DataTable ldtUsuarDB = kdb.GetHisRegByEnt("UsuarDB", "Usuarios DB");

            if (ldtUsuarDB != null)
            {
                foreach (DataRow ldrUsuarDB in ldtUsuarDB.Rows)
                {
                    DSODataContext.SetContext((int)ldrUsuarDB["iCodCatalogo"]);

                    dtEnt = DSODataAccess.Execute("select * from catalogos where iCodCatalogo is null");
                    int c = 0;

                    Hashtable htTable = new Hashtable();

                    htTable.Add("historicos", "his");
                    htTable.Add("detallados", "det");
                    htTable.Add("pendientes", "pend");

                    dtEnt = DSODataAccess.Execute("select 'drop view ' + name from sysobjects where name like 'vw_kdb_%'");

                    if (dtEnt != null)
                        foreach (DataRow drEnt in dtEnt.Rows)
                            DSODataAccess.Execute((string)drEnt[0]);

                    dtEnt = DSODataAccess.Execute("select * from catalogos where iCodCatalogo is null");

                    if (dtEnt != null)
                        foreach (DataRow drEnt in dtEnt.Rows)
                        {
                            c = 0;
                            dtMae = DSODataAccess.Execute("select * from maestros where iCodEntidad = " + (int)drEnt["iCodRegistro"]);

                            if (dtMae != null)
                                foreach (string tabla in htTable.Keys)
                                {
                                    c = 0;
                                    foreach (DataRow drMae in dtMae.Rows)
                                    {
                                        c++;

                                        if (c == 1)
                                        {
                                            try
                                            {
                                                lsbQuery = new StringBuilder();
                                                lsbQuery.Append("create view [vw_kdb_e_" + ((string)drEnt["vchCodigo"]).Replace(" ", "").Replace("-", "") + "_" + htTable[tabla] + "] as\r\n");
                                                lsbQuery.Append(kdb.GetQueryHis(
                                                    tabla,
                                                    kdb.CamposHis((string)drEnt["vchCodigo"], ""),
                                                    null, "", "", "") + "\r\n");

                                                //if (tabla != "historicos")
                                                //{
                                                //    lsbQuery.Replace("and '" + DateTime.Today.ToString("yyyy-MM-dd") + "' >= a.dtIniVigencia", "");
                                                //    lsbQuery.Replace("and '" + DateTime.Today.ToString("yyyy-MM-dd") + "' < a.dtFinVigencia", "");
                                                //}

                                                lsbQuery.Replace("'" + DateTime.Today.ToString("yyyy-MM-dd") + "'", "convert(varchar(10), getDate(), 120)");

                                                if (tabla == "detallados")
                                                    lsbQuery.Replace("a.*", "a.iCodRegistro, a.iCodCatalogo, a.iCodMaestro");
                                                else
                                                    lsbQuery.Replace("a.*", "a.iCodRegistro, a.iCodCatalogo, a.iCodMaestro, a.vchDescripcion");

                                                //lsbQuery.Replace("historicos his", tabla + " his");

                                                DSODataAccess.ExecuteNonQuery(lsbQuery.ToString());
                                            }
                                            catch { }
                                        }

                                        try
                                        {
                                            lsbQuery = new StringBuilder();
                                            lsbQuery.Append("create view [vw_kdb_e_" + ((string)drEnt["vchCodigo"]).Replace(" ", "").Replace("-", "") + "_m_" + ((string)drMae["vchDescripcion"]).Replace(" ", "").Replace("-", "") + "_" + htTable[tabla] + "] as\r\n");
                                            lsbQuery.Append(kdb.GetQueryHis(
                                                tabla,
                                                kdb.CamposHis((string)drEnt["vchCodigo"],
                                                (string)drMae["vchDescripcion"]),
                                                null, "", "", "") + "\r\n");

                                            //if (tabla != "historicos")
                                            //{
                                            //    lsbQuery.Replace("and '" + DateTime.Today.ToString("yyyy-MM-dd") + "' >= a.dtIniVigencia", "");
                                            //    lsbQuery.Replace("and '" + DateTime.Today.ToString("yyyy-MM-dd") + "' < a.dtFinVigencia", "");
                                            //}

                                            lsbQuery.Replace("'" + DateTime.Today.ToString("yyyy-MM-dd") + "'", "convert(varchar(10), getDate(), 120)");

                                            if (tabla == "detallados")
                                                lsbQuery.Replace("a.*", "a.iCodRegistro, a.iCodCatalogo, a.iCodMaestro");
                                            else
                                                lsbQuery.Replace("a.*", "a.iCodRegistro, a.iCodCatalogo, a.iCodMaestro, a.vchDescripcion");

                                            //lsbQuery.Replace("historicos his", tabla + " his");

                                            DSODataAccess.ExecuteNonQuery(lsbQuery.ToString());
                                        }
                                        catch { }
                                    }
                                }
                        }
                }
            }

            MessageBox.Show("Vistas creadas");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            KeytiaCOM.ICargasCOM ccargaComLoc;

            Hashtable lht = new Hashtable();

            lht.Add("{Español}", "Region prueba");
            lht.Add("{Ingles}", "Test Zone");
            lht.Add("dtFecUltAct", null);

            for (int i = 1; i <= 10000; i++)
            {
                ccargaComLoc = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:Priority=7/new:KeytiaCOM.CargasCOM,Priority=7");
                lht = new Hashtable();
                lht["dtFecUltAct"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                lht.Add("{CostoSM}", (double)i);
                lht.Add("{FechaFin}", DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"));
                ccargaComLoc.CargaFacturas(Util.Ht2Xml(lht), "Pendientes", "Detall", "DetalleCDR", 0);
                Marshal.ReleaseComObject(ccargaComLoc);
            }

            ccargaComLoc = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:Priority=7/new:KeytiaCOM.CargasCOM,Priority=7");
            lht = new Hashtable();
            lht["dtFecUltAct"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lht.Add("{CostoSM}", (double)11111);
            lht.Add("{FechaFin}", DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"));
            ccargaComLoc.CargaFacturas(Util.Ht2Xml(lht), "Pendientes", "Detall", "DetalleCDR", 0);

            Marshal.ReleaseComObject(ccargaComLoc);
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Fin");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            KeytiaCOM.ICargasCOM ccargaComLoc;

            Hashtable lht = new Hashtable();

            // Inserción de 10000 registros
            for (int i = 1; i <= 100; i++)
            {
                ccargaComLoc = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:Priority=7/new:KeytiaCOM.CargasCOM,Priority=7");
                lht = new Hashtable();
                lht.Add("{Español}", "Region prueba");
                lht.Add("{Ingles}", "Test Zone");
                lht.Add("dtFinVigencia", DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
                ccargaComLoc.CargaFacturas(Util.Ht2Xml(lht), "Historicos", "MsgWeb", "Test", 0);
                Marshal.ReleaseComObject(ccargaComLoc);
            }


            MessageBox.Show("Pulsa aceptar cuando la cola de mensajes esté vacía.");

            KDBAccess kdb = new KDBAccess();
            DataTable ldtRegistros = kdb.GetHisRegByEnt("MsgWeb", "Test");

            if (ldtRegistros != null && ldtRegistros.Rows.Count > 0)
            {
                MessageBox.Show("Se encontraron " + ldtRegistros.Rows.Count + " registros.");
                foreach (DataRow ldrRow in ldtRegistros.Rows)
                {
                    ccargaComLoc = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:Priority=7/new:KeytiaCOM.CargasCOM,Priority=7");
                    lht.Clear();
                    lht.Add("{Español}", "Region prueba");
                    lht.Add("{Ingles}", "Test Zone");
                    lht.Add("dtFinVigencia", DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"));
                    ccargaComLoc.CargaFacturas(Util.Ht2Xml(lht), "Historicos", "MsgWeb", "Test", (int)ldrRow["iCodRegistro"]);
                    Marshal.ReleaseComObject(ccargaComLoc);
                    break;
                }
            }
            else
            {
                MessageBox.Show("No se encontraron registros.");
            }

            Cursor.Current = Cursors.Default;
            MessageBox.Show("Fin");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.CargaCDR.CargaServicioCDR c = new KeytiaServiceBL.CargaCDR.CargaServicioCDR();
            c.GetExtensiones();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.LanzadorCargas oLC = new KeytiaServiceBL.LanzadorCargas();

            oLC.Start();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Hashtable lht = new Hashtable();

            do
            {
                lht.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            } while (true);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            KeytiaWeb.TripleDESWrapper enc = new KeytiaWeb.TripleDESWrapper();

            textBox2.Text =  enc.Encrypt(textBox1.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            KeytiaWeb.TripleDESWrapper dec = new KeytiaWeb.TripleDESWrapper();

            textBox2.Text = dec.Decrypt(textBox1.Text);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Hashtable lht = new Hashtable();
            lht.Add("algo", @"algo "" con comillas");

            string xml = Util.Ht2Xml(lht);

            xml = "<mensaje><row entidad=\"Emple\" maestro=\"Empleados\" tabla=\"Pendientes\" copiardet=\"trueº\" id=\"0010010\" regcarga=\"20\" cargas=\"205759\" op=\"I\"><rowatt key=\"{NominaA}\" value=\"0010010\" type=\"System.String\" /><rowatt key=\"vchDescripcion\" value=\"0010010,F&quot;OGE-790814,BLANCA LAURA BELTRAN JIMENEZ,,,blanca.beltran@gruposenda.com,E,,0010,DIRECCION GENERAL_TAMAULIPAS FEDERAL,2010-01-01 00:00:\"/></row></mensaje>";
            xml = "<Hashtable><item key=\"{NominaA}\" value=\"0010010\" type=\"System.String\" /><item key=\"vchDescripcion\" value=\"0010010,F&quot;OGE-790814,BLANCA LAURA BELTRAN JIMENEZ,,,blanca.beltran@gruposenda.com,E,,0010,DIRECCION GENERAL_TAMAULIPAS FEDERAL,2010-01-01 00:00:\"/></Hashtable>";
            xml = "<Hashtable><item key=\"{NominaA}\" value=\"0010010\" type=\"System.String\" /><item key=\"vchDescripcion\" value=\"0010010,F\"OGE-790814,BLANCA LAURA BELTRAN JIMENEZ,,,blanca.beltran@gruposenda.com,E,,0010,DIRECCION GENERAL_TAMAULIPAS FEDERAL,2010-01-01 00:00:\"/></Hashtable>";
            Hashtable lht2 = Util.Xml2Ht(xml);
        }
    }
}
