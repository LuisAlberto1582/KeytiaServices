using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Collections;
using KeytiaServiceBL.CargaRecursos;

namespace KeytiaServiceTester
{
    public partial class Form7 : Form
    {
        private const string xmlError = "<mensaje></mensaje>";
        private const string xmlPruebaCE = "<mensaje><row entidad=\"Puesto\" maestro=\"Puestos Empleado\" tabla=\"Historicos\" id=\"Gerente\" regcarga=\"1\" cargas=\"79711\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"Gerente\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"2011-05-31 00:00:00\" type=\"System.DateTime\" /></row><row entidad=\"Emple\" maestro=\"Empleados\" tabla=\"Historicos\" id=\"1\" regcarga=\"1\" cargas=\"79711\" op=\"I\"><rowatt key=\"{Paterno}\" value=\"GOUYONET\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"2011-05-31 00:00:00\" type=\"System.DateTime\" /><rowatt key=\"{Puesto}\" value=\"NewGerente\" type=\"System.String\" /><rowatt key=\"{TipoEm}\" value=\"347\" type=\"System.Int32\" /><rowatt key=\"{Materno}\" value=\"MENDOZA\" type=\"System.String\" /><rowatt key=\"{Nombre}\" value=\"ALEJANDRO MANUEL\" type=\"System.String\" /><rowatt key=\"{Cos}\" value=\"76188\" type=\"System.Int32\" /><rowatt key=\"{CenCos}\" value=\"78969\" type=\"System.Int32\" /><rowatt key=\"{Nomina}\" value=\"1\" type=\"System.String\" /></row><row entidad=\"Linea\" maestro=\"Lineas\" tabla=\"Historicos\" id=\"5528997472\" regcarga=\"1\" cargas=\"79711\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"5528997472\" type=\"System.String\" /><rowatt key=\"{Sitio}\" value=\"79657\" type=\"System.Int32\" /><rowatt key=\"{Carrier}\" value=\"284\" type=\"System.Int32\" /><rowatt key=\"dtIniVigencia\" value=\"2011-05-31 00:00:00\" type=\"System.DateTime\" /><rowatt key=\"{Tel}\" value=\"3532111\" type=\"System.String\" /></row><rel nombre=\"Empleado - Linea\" id=\"1-5528997472\" dtIniVigencia=\"2011-05-31 00:00:00\" op=\"I\"><rowatt key=\"{Emple}\" value=\"New1\" type=\"System.String\" /><rowatt key=\"{Linea}\" value=\"New5528997472\" type=\"System.String\" /></rel></mensaje>";
        private const string xmlPruebaCC = "<mensaje><row entidad=\"CenCos\" maestro=\"Centro de Costos\" tabla=\"Historicos\" id=\"191\" regcarga=\"3\" cargas=\"200191\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"191\" type=\"System.String\" /><rowatt key=\"{PresupFijo}\" value=\"1000\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"02/06/2011 12:00:00 a.m.\" type=\"System.String\" /></row></mensaje>";
            //<mensaje><row entidad=\"CenCos\" maestro=\"Centro de Costos\" tabla=\"Pendientes\" id=\"338\" regcarga=\"4\" cargas=\"200191\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"338338,,1012020501,,duracion,,2011-06-02 00:00:00,2011-12-10 00:00:00[El Centro de Costo no puede ser dado de baja, no se encuentra en Sistema.][Formato Incorrecto. Id de Centro de Costo Resp.]\" type=\"System.String\" /><rowatt key=\"{PresupFijo}\" value=\"1000\" type=\"System.String\" /><rowatt key=\"{TipoPr}\" value=\"394\" type=\"System.String\" /><rowatt key=\"{CenCos}\" value=\"80531\" type=\"System.String\" /></row></mensaje><mensaje><row entidad=\"CenCos\" maestro=\"Centro de Costos\" tabla=\"Pendientes\" id=\"339\" regcarga=\"5\" cargas=\"200191\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"339[Centro de Costo Responsable: 603 no identificado.]\" type=\"System.String\" /><rowatt key=\"{PresupFijo}\" value=\"1000\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"02/06/2011 12:00:00 a.m.\" type=\"System.String\" /></row></mensaje><mensaje><row entidad=\"CenCos\" maestro=\"Centro de Costos\" tabla=\"Historicos\" id=\"477\" regcarga=\"6\" cargas=\"200191\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"477\" type=\"System.String\" /><rowatt key=\"{PresupFijo}\" value=\"100\" type=\"System.String\" /><rowatt key=\"{TipoPr}\" value=\"393\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"02/06/2011 12:00:00 a.m.\" type=\"System.String\" /></row></mensaje><mensaje><row entidad=\"CenCos\" maestro=\"Centro de Costos\" tabla=\"Pendientes\" id=\"603\" regcarga=\"7\" cargas=\"200191\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"603603,,1111111111,1000,,,2011-06-02 00:00:00,[Formato Incorrecto. Id de Centro de Costo Resp.][Centro de Costo Responsable: 1111111111 no identificado.]\" type=\"System.String\" /><rowatt key=\"{PresupFijo}\" value=\"1000\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"02/06/2011 12:00:00 a.m.\" type=\"System.String\" /></row></mensaje><mensaje><row entidad=\"CenCos\" maestro=\"Centro de Costos\" tabla=\"Historicos\" id=\"1773\" regcarga=\"8\" cargas=\"200191\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"1773\" type=\"System.String\" /><rowatt key=\"{PresupFijo}\" value=\"1000\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"02/06/2011 12:00:00 a.m.\" type=\"System.String\" /><rowatt key=\"{CenCos}\" value=\"New1914\" type=\"System.String\" /></row><row entidad=\"CenCos\" maestro=\"Centro de Costos\" tabla=\"Historicos\" id=\"1914\" regcarga=\"9\" cargas=\"200191\" op=\"I\"><rowatt key=\"vchDescripcion\" value=\"1914\" type=\"System.String\" /><rowatt key=\"{PresupFijo}\" value=\"1000\" type=\"System.String\" /><rowatt key=\"dtIniVigencia\" value=\"02/06/2011 12:00:00 a.m.\" type=\"System.String\" /></row></mensaje>";
        
        public Form7()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                try
                {
                    KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
                    cargasCOM.CargaEmpleado(xmlPruebaCE, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
                {
                    KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
                    cargasCOM.CargaCentroCosto(xmlPruebaCC, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CargaServicioEmpleado lCarga = new CargaServicioEmpleado();
            lCarga.CodCarga = 80723;
            lCarga.IniciarCarga();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CargaServicioCentroCosto lCarga = new CargaServicioCentroCosto();
            lCarga.CodCarga = 80761;
            lCarga.IniciarCarga();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int iCodRegistro = Int32.Parse(txtICodRegistro.Text);
                KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
                cargasCOM.EliminarRegistro("Historicos", iCodRegistro, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                int iCodRegistro = Int32.Parse(txtICodRegistro.Text);
                KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
                cargasCOM.EliminarRegistro("relaciones", iCodRegistro, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
                string lsEntidad = "UsuarDB";
                string lsMaestro = "Usuarios DB";
                DataTable dlUsuarios = kdb.GetHisRegByEnt(lsEntidad, lsMaestro);
                Console.WriteLine(dlUsuarios);
                if (dlUsuarios != null && dlUsuarios.Rows.Count > 0)
                {
                    foreach (DataRow ldrUsuario in dlUsuarios.Rows)
                    {
                        if (ldrUsuario["{Esquema}"].ToString().Equals("Senda"))
                        {
                            KeytiaServiceBL.DSODataContext.SetContext((int)ldrUsuario["iCodCatalogo"]);
                            KeytiaCOM.CargasCOM com = new KeytiaCOM.CargasCOM();
                            com.BajaCarga(205529, (int)ldrUsuario["iCodCatalogo"]);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.KDBAccess pKDB = new KeytiaServiceBL.KDBAccess();
            DataTable lKDBTable = null;
            string lsLang = "{Español}";
            string lsEntidad = "MsgWeb";
            string term = "";
            lKDBTable = pKDB.GetHisRegByEnt(lsEntidad, "", new string[] { "iCodCatalogo", "vchDescripcion", lsLang }, "", "vchDescripcion," + lsLang, 100, "(vchDescripcion like '%" + term.Replace("'", "''") + "%' or " + lsLang + " like '%" + term.Replace("'", "''") + "%')");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();

            //int liCodRegCarga = int.MinValue;
            int liCodRegCarga = 78775;
            int liUsuario = int.Parse(txtICodRegistro.Text);
            string lsEntidad = "CenCos";
            string lsMaestro = "Centro de Costos";
            string lsTabla = "Historicos";
            bool lbAjustaValores = false;
            bool lbReplicar = true;
            string lsDescripcion = "CC Keytia Pruebas - 013-ed";
            string lsVchCodigo = "vchCod013";
            string lsHT = "<Hashtable><item key=\"iCodMaestro\" value=\"18\" type=\"System.Int32\" /><item key=\"iCodCatalogo05\" value=\"352\" type=\"System.String\" /><item key=\"iCodCatalogo02\" value=\"null\" type=\"System.String\" /><item key=\"dtIniVigencia\" value=\"2011-01-01 00:00:00\" type=\"System.DateTime\" /><item key=\"vchDescripcion\" value=\"'" + lsDescripcion + "'\" type=\"System.String\" /><item key=\"VarChar01\" value=\"'" + lsDescripcion + "'\" type=\"System.String\" /><item key=\"Float01\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo04\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo01\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo03\" value=\"450\" type=\"System.String\" /><item key=\"vchCodigo\" value=\"'" + lsVchCodigo + "'\" type=\"System.String\" /><item key=\"dtFinVigencia\" value=\"2079-01-01 00:00:00\" type=\"System.DateTime\" /><item key=\"iCodUsuario\" value=\"341\" type=\"System.Int32\" /></Hashtable>";
            //cargasCOM.InsertaRegistro(KeytiaServiceBL.Util.Xml2Ht(lsHT), lsTabla, lsEntidad, lsMaestro, lbAjustaValores, liUsuario, lbReplicar);
            //cargasCOM.ActualizaRegistro(lsTabla, lsEntidad, lsMaestro, KeytiaServiceBL.Util.Xml2Ht(lsHT), liCodRegCarga, lbAjustaValores, liUsuario, lbReplicar);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(0);
            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();

            //StringBuilder sbQuery = new StringBuilder();
            //sbQuery.Append("\r\ndeclare @iCodRegistro int = 2");
            //sbQuery.Append("\r\ndeclare @vchCodigo varchar(50)");
            //sbQuery.Append("\r\ndeclare @vchCodigoEntidad varchar(50)");
            //sbQuery.Append("\r\ndeclare @iCodCatalogoEntidad int");
            //sbQuery.Append("\r\nselect @vchCodigo = vchCodigo, @iCodCatalogoEntidad = iCodCatalogo from Keytia.Catalogos where iCodRegistro = @iCodRegistro");
            //sbQuery.Append("\r\nselect @vchCodigoEntidad = vchCodigo from Keytia.Catalogos where iCodRegistro = @iCodCatalogoEntidad");
            //sbQuery.Append("\r\nselect @vchCodigo 'vchCodigo', @vchCodigoEntidad 'vchCodigoEntidad'");

            string lsVchCodigo = "TDest";
            string lsVchCodigoEntidad = "";
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("\r\ndeclare @vchCodigo varchar(50)");
            sbQuery.Append("\r\ndeclare @vchCodigoEntidad varchar(50)");
            sbQuery.Append("\r\ndeclare @iCodRegistroEntidad int");
            sbQuery.Append("\r\ndeclare @iCodRegistroCatalogo int");
            sbQuery.Append("\r\nset @vchCodigo = '" + lsVchCodigo + "'");
            if (lsVchCodigoEntidad.Length > 0)
                sbQuery.Append("\r\nset @vchCodigoEntidad = '" + lsVchCodigoEntidad + "'");
            sbQuery.Append("\r\nif @vchCodigoEntidad is not null");
            sbQuery.Append("\r\nselect @iCodRegistroEntidad = iCodregistro from Catalogos where vchCodigo = @vchCodigoEntidad");
            sbQuery.Append("\r\nelse");
            sbQuery.Append("\r\nselect @iCodRegistroCatalogo = iCodRegistro from Catalogos where vchCodigo = @vchCodigo and iCodCatalogo is null");
            sbQuery.Append("\r\nif @iCodRegistroEntidad is not null");
            sbQuery.Append("\r\nselect @iCodRegistroCatalogo = iCodRegistro from Catalogos where vchCodigo = @vchCodigo and iCodCatalogo = @iCodRegistroEntidad");
            sbQuery.Append("\r\nselect @iCodRegistroCatalogo 'iCodregistroCatalogo',  @iCodRegistroEntidad 'iCodregistroEntidad'");
            
            DataTable dtTest = KeytiaServiceBL.DSODataAccess.Execute(sbQuery.ToString());

            //Hashtable lhtRelaciones = kdb.CamposRel("Sitio - Plan de Marcacion");
            //string vchCodigo = dtTest.Rows[0]["vchCodigo"].ToString();
            //string vchCodigoEntidad = dtTest.Rows[0]["vchCodigoEntidad"].ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();

            //int liCodRegCarga = int.MinValue;
            int liCodRegCarga = 141;
            int liUsuario = int.Parse(txtICodRegistro.Text);
            string lsEntidad = "";
            string lsMaestro = "";
            string lsTabla = "Maestros";
            bool lbAjustaValores = false;
            bool lbReplicar = true;
            string lsDescripcion = "Pruebas Maestros 001-ed";
            //string lsHT = "<Hashtable><item key=\"iCodMaestro\" value=\"18\" type=\"System.Int32\" /><item key=\"iCodCatalogo05\" value=\"352\" type=\"System.String\" /><item key=\"iCodCatalogo02\" value=\"null\" type=\"System.String\" /><item key=\"dtIniVigencia\" value=\"2011-01-01 00:00:00\" type=\"System.DateTime\" /><item key=\"vchDescripcion\" value=\"'" + lsDescripcion + "'\" type=\"System.String\" /><item key=\"VarChar01\" value=\"'" + lsDescripcion + "'\" type=\"System.String\" /><item key=\"Float01\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo04\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo01\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo03\" value=\"450\" type=\"System.String\" /><item key=\"vchCodigo\" value=\"'" + lsVchCodigo + "'\" type=\"System.String\" /><item key=\"dtFinVigencia\" value=\"2079-01-01 00:00:00\" type=\"System.DateTime\" /><item key=\"iCodUsuario\" value=\"341\" type=\"System.Int32\" /></Hashtable>";
            string lsHT = "<Hashtable><item key=\"iCodCatalogo01\" value=\"16\" type=\"System.String\" /><item key=\"Date03Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo07Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo05Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo04\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo02Col\" value=\"1\" type=\"System.String\" /><item key=\"Float02\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion02Ren\" value=\"null\" type=\"System.String\" /><item key=\"Date04Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar10\" value=\"null\" type=\"System.String\" /><item key=\"Date02Ren\" value=\"null\" type=\"System.String\" /><item key=\"Float04Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo03Col\" value=\"null\" type=\"System.String\" /><item key=\"Float05\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion02\" value=\"null\" type=\"System.String\" /><item key=\"Integer04Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion03Ren\" value=\"null\" type=\"System.String\" /><item key=\"vchDescripcion\" value=\"'" + lsDescripcion + "'\" type=\"System.String\" /><item key=\"Integer01Ren\" value=\"null\" type=\"System.String\" /><item key=\"Integer05\" value=\"null\" type=\"System.String\" /><item key=\"Float04Col\" value=\"null\" type=\"System.String\" /><item key=\"Integer02\" value=\"null\" type=\"System.String\" /><item key=\"Integer01\" value=\"null\" type=\"System.String\" /><item key=\"dtIniVigencia\" value=\"'2011-07-18 00:00:00'\" type=\"System.String\" /><item key=\"iCodCatalogo10\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo01Ren\" value=\"1\" type=\"System.String\" /><item key=\"iCodRelacion02Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo10Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar01Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion04Ren\" value=\"null\" type=\"System.String\" /><item key=\"bActualizaHistoria\" value=\"0\" type=\"System.String\" /><item key=\"iCodCatalogo02Ren\" value=\"2\" type=\"System.String\" /><item key=\"iCodCatalogo09\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion03Col\" value=\"null\" type=\"System.String\" /><item key=\"Float01\" value=\"null\" type=\"System.String\" /><item key=\"iCodUsuario\" value=\"341\" type=\"System.String\" /><item key=\"Float03Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar03\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion01Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion05Col\" value=\"null\" type=\"System.String\" /><item key=\"Date01Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo07\" value=\"null\" type=\"System.String\" /><item key=\"Integer05Col\" value=\"null\" type=\"System.String\" /><item key=\"Integer04Col\" value=\"null\" type=\"System.String\" /><item key=\"Integer03Col\" value=\"null\" type=\"System.String\" /><item key=\"Integer02Col\" value=\"null\" type=\"System.String\" /><item key=\"Integer01Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo04Col\" value=\"null\" type=\"System.String\" /><item key=\"Float02Col\" value=\"null\" type=\"System.String\" /><item key=\"dtFecUltAct\" value=\"'2011-07-18 00:00:00'\" type=\"System.String\" /><item key=\"Date03\" value=\"null\" type=\"System.String\" /><item key=\"Float01Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion01Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion01\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo08\" value=\"null\" type=\"System.String\" /><item key=\"Date04\" value=\"null\" type=\"System.String\" /><item key=\"Date03Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo05Col\" value=\"null\" type=\"System.String\" /><item key=\"Integer05Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo03\" value=\"null\" type=\"System.String\" /><item key=\"Integer03Ren\" value=\"null\" type=\"System.String\" /><item key=\"Integer02Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar03Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar02Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar01Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo06\" value=\"null\" type=\"System.String\" /><item key=\"VarChar07Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar06Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar05Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar04Ren\" value=\"null\" type=\"System.String\" /><item key=\"Float03Ren\" value=\"null\" type=\"System.String\" /><item key=\"Float01Ren\" value=\"null\" type=\"System.String\" /><item key=\"Date02Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar08Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo06Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar10Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar10Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo07Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion05\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo08Col\" value=\"null\" type=\"System.String\" /><item key=\"Float05Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo09Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo10Col\" value=\"null\" type=\"System.String\" /><item key=\"Float04\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion05Ren\" value=\"null\" type=\"System.String\" /><item key=\"Date05Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar03Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar02Col\" value=\"null\" type=\"System.String\" /><item key=\"Date05\" value=\"null\" type=\"System.String\" /><item key=\"VarChar07Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar06Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar05Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar04Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo02\" value=\"23\" type=\"System.String\" /><item key=\"VarChar09Col\" value=\"null\" type=\"System.String\" /><item key=\"VarChar08Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion04\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion04Col\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo03Ren\" value=\"null\" type=\"System.String\" /><item key=\"Float03\" value=\"null\" type=\"System.String\" /><item key=\"Date01Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar02\" value=\"null\" type=\"System.String\" /><item key=\"VarChar01\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo05\" value=\"null\" type=\"System.String\" /><item key=\"VarChar07\" value=\"null\" type=\"System.String\" /><item key=\"VarChar06\" value=\"null\" type=\"System.String\" /><item key=\"VarChar05\" value=\"null\" type=\"System.String\" /><item key=\"VarChar04\" value=\"null\" type=\"System.String\" /><item key=\"VarChar09\" value=\"null\" type=\"System.String\" /><item key=\"VarChar08\" value=\"null\" type=\"System.String\" /><item key=\"Date04Ren\" value=\"null\" type=\"System.String\" /><item key=\"VarChar09Ren\" value=\"null\" type=\"System.String\" /><item key=\"Float05Ren\" value=\"null\" type=\"System.String\" /><item key=\"Float02Ren\" value=\"null\" type=\"System.String\" /><item key=\"dtFinVigencia\" value=\"'2079-01-01 00:00:00'\" type=\"System.String\" /><item key=\"iCodCatalogo08Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo04Ren\" value=\"null\" type=\"System.String\" /><item key=\"Date02\" value=\"null\" type=\"System.String\" /><item key=\"Date01\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo06Ren\" value=\"null\" type=\"System.String\" /><item key=\"iCodRelacion03\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo01Col\" value=\"1\" type=\"System.String\" /><item key=\"iCodEntidad\" value=\"14\" type=\"System.String\" /><item key=\"Date05Col\" value=\"null\" type=\"System.String\" /><item key=\"Integer04\" value=\"null\" type=\"System.String\" /><item key=\"Integer03\" value=\"null\" type=\"System.String\" /><item key=\"iCodCatalogo09Ren\" value=\"null\" type=\"System.String\" /></Hashtable>";

            //cargasCOM.InsertaRegistro(KeytiaServiceBL.Util.Xml2Ht(lsHT), "Maestros", false, liUsuario);

            //cargasCOM.InsertaRegistro(KeytiaServiceBL.Util.Xml2Ht(lsHT), lsTabla, lsEntidad, lsMaestro, lbAjustaValores, liUsuario, lbReplicar);
            //cargasCOM.ActualizaRegistro(lsTabla, lsEntidad, lsMaestro, KeytiaServiceBL.Util.Xml2Ht(lsHT), liCodRegCarga, lbAjustaValores, liUsuario, lbReplicar);
        }

        /// <summary>
        /// Probar ComplementaHistorico
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
            string lsEntidad = "Emple";
            string lsMaestro = "Empleados";
            string lsTabla = "Historicos";
            Hashtable lhtCamposHis = kdb.CamposHis(lsEntidad, lsMaestro);
            Hashtable lhtTabla = new Hashtable();
            lhtTabla.Add("vchDescripcion", "Carlos Alberto Longoria Leal");
            lhtTabla.Add("{Email}", "carlos.longoria@pruebas.com");
            lhtTabla.Add("{Ubica}", "Ubicación de prueba");
            lhtTabla.Add("{Nombre}", "Carlos Alberto Longoria Leal");
            lhtTabla.Add("{RFC}", "FOGE-790814");
            lhtTabla.Add("dtIniVigencia", DateTime.Today);
            int liRegistroInsertado = cargasCOM.InsertaRegistro(lhtTabla, lsTabla, lsEntidad, lsMaestro, true, 0, false);
        }


    }
}

