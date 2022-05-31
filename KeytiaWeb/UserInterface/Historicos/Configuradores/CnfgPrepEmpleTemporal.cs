using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using KeytiaServiceBL.Alarmas;
using KeytiaServiceBL.Reportes;

namespace KeytiaWeb.UserInterface
{
    public class CnfgPrepEmpleTemporal : CnfgPrepEmpleFijo
    {
        protected DataRow pRowConsumoBase = null;
        protected DataRow pRowEmple = null;
        protected DataRow pRowUsuarEmple = null;
        protected DataRow pRowNotifPrepEmple = null;
        protected DataRow pRowEmpre = null;
        protected DataRow pRowClient = null;
        protected DataRow pRowTipoPr = null;
        protected DataRow pRowPeriodoPr = null;
        protected DataRow pRowUsuarDB = null;
        protected DataRow pRowSitio = null;
        protected DataRow pRowPrepSitioEnvio = null;
        protected DataRow pRowNotifPrepSitio = null;
        protected DataRow pRowEmpleRespSitio = null;
        protected DataRow pRowUsuarRespSitio = null;
        protected int piMaxMinutosSitio = 0;
        protected DataTable ptblNotifPrepSitio = null;
        protected string psIdioma;
        protected WordAccess pWord;
        protected MailAccess pMail;
        protected StringBuilder psbErrores;
        protected string psDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

        protected override bool ValidarVigencias()
        {
            if (State == HistoricState.Edicion)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
                pdtFinVigencia.DataValue = pdtIniPeriodoSiguiente;
            }

            //el usuario nunca edita las vigencias por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarDatos()
        {
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            //vuelvo a llamar este metodo para asegurarme que los controles traigan la ultima configuracion de la empresa
            CalcularDatosEmpresa();
            pdtFinVigencia.DataValue = pdtIniPeriodoSiguiente;

            double ldValor;
            if (pFields.GetByConfigName("PresupProv").DSOControlDB.HasValue)
            {
                double.TryParse(pFields.GetByConfigName("PresupProv").DataValue.ToString(), out ldValor);
                if (ldValor < 0)
                {
                    string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValPrepMenorCero")) + "</span>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsMsg, lsTitulo);

                    return false;
                }
            }

            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            pvchDescripcion.DataValue = pFields.GetByConfigName("Emple").ToString();

            DataTable lDataTable;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select Bt.iCodRegistro");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisDetallados('Detall','Bitacora Notificacion Consumos','" + Globals.GetCurrentLanguage() + "')] Bt");
            psbQuery.AppendLine("where Bt.FechaReset is null");
            psbQuery.AppendLine("and Bt.Emple = " + pFields.GetByConfigName("Emple").DataValue);
            psbQuery.AppendLine("and Bt.PrepEmple = " + iCodCatalogo);
            psbQuery.AppendLine("and Bt.FechaInicioPrep = " + pFields.GetByConfigName("FechaInicioPrep").DataValue);

            lDataTable = DSODataAccess.Execute(psbQuery.ToString());
            if (lDataTable.Rows.Count > 0)
            {
                //si ya existen notificaciones para el periodo actual entonces no se permite hacer cambios al presupuesto
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValPrepTempNotificaciones")) + "</span>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsMsg, lsTitulo);

                return false;
            }

            if (pFields.GetByConfigName("PrepCenCos").DSOControlDB.HasValue)
            {
                //si era un registro que se habia generado desde CenCos y se edita en Empleados
                //entonces le quito la liga a CenCos
                pFields.GetByConfigName("PrepCenCos").DataValue = DBNull.Value;
            }

            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();

            return true;
        }

        public override void CalcularDatosEmpresa()
        {
            base.CalcularDatosEmpresa();
            CalcularConsumoBase();
        }

        protected virtual void CalcularConsumoBase()
        {
            psbQuery.Length = 0;
            psbQuery.AppendLine("select Emple,");
            if (pRowConfigEmpre["TipoPrCod"].ToString() == "Llamadas")
            {
                psbQuery.AppendLine("   Consumo = isnull(COUNT(*),0)");
            }
            else if (pRowConfigEmpre["TipoPrCod"].ToString() == "Duracion")
            {
                psbQuery.AppendLine("   Consumo = isnull(SUM(DuracionMin),0)");
            }
            else if (pRowConfigEmpre["TipoPrCod"].ToString() == "Costo")
            {
                psbQuery.AppendLine("   Consumo = isnull(SUM(Costo),0) + isnull(SUM(CostoSM),0)");
            }
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','" + Globals.GetCurrentLanguage() + "')] Detall");
            psbQuery.AppendLine("where Emple = " + pFields.GetByConfigName("Emple").DataValue);
            psbQuery.AppendLine("and FechaInicio >= '" + pdtIniPeriodoActual.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and FechaInicio < '" + pdtIniPeriodoSiguiente.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("group by Emple");

            DataTable lDataTable = DSODataAccess.Execute(psbQuery.ToString());
            if (lDataTable.Rows.Count > 0)
            {
                pRowConsumoBase = lDataTable.Rows[0];
                pFields.GetByConfigName("ValorConsumoBase").DataValue = pRowConsumoBase["Consumo"];
            }
            else
            {
                pFields.GetByConfigName("ValorConsumoBase").DataValue = 0;
            }
            pFields.GetByConfigName("ValorConsumoBase").DisableField();
        }

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
            EnviarCorreos();
        }

        protected virtual void EnviarCorreos()
        {
            //Enviar correos en base a configuracion de baja y alta de codigos/extensiones de sitios
            psbErrores = new StringBuilder();

            DateTime ldtAhora = DateTime.Now;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre");
            psbQuery.AppendLine("where Empre.dtIniVigencia <> Empre.dtFinVigencia");
            psbQuery.AppendLine("and Empre.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Empre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Empre.iCodCatalogo = " + pRowConfigEmpre["Empre"].ToString());

            pRowEmpre = DSODataAccess.ExecuteDataRow(psbQuery.ToString());

            if ((int.Parse(pRowEmpre["BanderasEmpre"].ToString()) & 2) != 2)
            {
                //Si no esta encendida la bandera de dar de baja/alta los codigos/extensiones entonces no se necesita enviar ningun correo
                return;
            }

            if (pRowEmpre["Client"] == DBNull.Value)
            {
                //Si la empresa no tiene configurado el cliente entonces esta mal configurada y me detengo
                return;
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','" + Globals.GetCurrentLanguage() + "')] Client");
            psbQuery.AppendLine("where Client.dtIniVigencia <> Client.dtFinVigencia");
            psbQuery.AppendLine("and Client.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Client.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Client.iCodCatalogo = " + Util.IsDBNull(pRowEmpre["Client"], 0));

            pRowClient = DSODataAccess.ExecuteDataRow(psbQuery.ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepSitio','Notificaciones de Presupuestos para Sitios','" + Globals.GetCurrentLanguage() + "')] NotifPrepSitio");
            psbQuery.AppendLine("where NotifPrepSitio.dtIniVigencia <> NotifPrepSitio.dtFinVigencia");
            psbQuery.AppendLine("and NotifPrepSitio.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and NotifPrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and NotifPrepSitio.TipoBloqueo = 1");
            psbQuery.AppendLine("and NotifPrepSitio.Sitio in(select CodAuto.Sitio");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CodAuto','" + Globals.GetCurrentLanguage() + "')] CodAuto");
            psbQuery.AppendLine("   where CodAuto.dtIniVigencia <> CodAuto.dtFinVigencia");
            psbQuery.AppendLine("   and CodAuto.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("   and CodAuto.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("   and CodAuto.iCodCatalogo in(select Rel.CodAuto from " + DSODataContext.Schema + ".[VisRelaciones('Empleado - CodAutorizacion','" + Globals.GetCurrentLanguage() + "')] Rel");
            psbQuery.AppendLine("       where Rel.dtIniVigencia <> Rel.dtFinVigencia");
            psbQuery.AppendLine("       and Rel.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("       and Rel.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("       and Rel.Emple = " + pFields.GetByConfigName("Emple").DataValue + "))");
            //psbQuery.AppendLine("and NotifPrepSitio.Sitio in(select Sitio.iCodCatalogo");
            //psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHisComun('Sitio','" + Globals.GetCurrentLanguage() + "')] Sitio");
            //psbQuery.AppendLine("   where Sitio.dtIniVigencia <> Sitio.dtFinVigencia");
            //psbQuery.AppendLine("   and Sitio.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            //psbQuery.AppendLine("   and Sitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            //psbQuery.AppendLine("   and Sitio.Empre = " + pRowConfigEmpre["Empre"].ToString() + ")");
            psbQuery.AppendLine("union all");
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepSitio','Notificaciones de Presupuestos para Sitios','" + Globals.GetCurrentLanguage() + "')] NotifPrepSitio");
            psbQuery.AppendLine("where NotifPrepSitio.dtIniVigencia <> NotifPrepSitio.dtFinVigencia");
            psbQuery.AppendLine("and NotifPrepSitio.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and NotifPrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and NotifPrepSitio.TipoBloqueo = 2");
            psbQuery.AppendLine("and NotifPrepSitio.Sitio in(select Exten.Sitio");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Exten','" + Globals.GetCurrentLanguage() + "')] Exten");
            psbQuery.AppendLine("   where Exten.dtIniVigencia <> Exten.dtFinVigencia");
            psbQuery.AppendLine("   and Exten.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("   and Exten.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("   and Exten.iCodCatalogo in(select Rel.Exten from " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Extension','" + Globals.GetCurrentLanguage() + "')] Rel");
            psbQuery.AppendLine("       where Rel.dtIniVigencia <> Rel.dtFinVigencia");
            psbQuery.AppendLine("       and Rel.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("       and Rel.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("       and Rel.Emple = " + pFields.GetByConfigName("Emple").DataValue + "))");
            //psbQuery.AppendLine("and NotifPrepSitio.Sitio in(select Sitio.iCodCatalogo");
            //psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHisComun('Sitio','" + Globals.GetCurrentLanguage() + "')] Sitio");
            //psbQuery.AppendLine("   where Sitio.dtIniVigencia <> Sitio.dtFinVigencia");
            //psbQuery.AppendLine("   and Sitio.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            //psbQuery.AppendLine("   and Sitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            //psbQuery.AppendLine("   and Sitio.Empre = " + pRowConfigEmpre["Empre"].ToString() + ")");

            ptblNotifPrepSitio = DSODataAccess.Execute(psbQuery.ToString());
            if (ptblNotifPrepSitio.Rows.Count == 0)
            {
                //Si el empleado no tiene asignado ningun codigo/extension en algun sitio que tenga configurado la generacion de arhivos
                //de bajas/altas de codigos entonces no se puede enviar ningun correo
                return;
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('TipoPr','Tipo Presupuesto','" + Globals.GetCurrentLanguage() + "')] TipoPr");
            psbQuery.AppendLine("where TipoPr.dtIniVigencia <> TipoPr.dtFinVigencia");
            psbQuery.AppendLine("and TipoPr.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and TipoPr.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and TipoPr.iCodCatalogo = " + pRowConfigEmpre["TipoPr"].ToString());

            pRowTipoPr = DSODataAccess.ExecuteDataRow(psbQuery.ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PeriodoPr','Periodo Presupuesto','" + Globals.GetCurrentLanguage() + "')] PeriodoPr");
            psbQuery.AppendLine("where PeriodoPr.dtIniVigencia <> PeriodoPr.dtFinVigencia");
            psbQuery.AppendLine("and PeriodoPr.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and PeriodoPr.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and PeriodoPr.iCodCatalogo = " + pRowConfigEmpre["PeriodoPr"].ToString());

            pRowPeriodoPr = DSODataAccess.ExecuteDataRow(psbQuery.ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('UsuarDB','Usuarios DB','" + Globals.GetCurrentLanguage() + "')] UsuarDB");
            psbQuery.AppendLine("where UsuarDB.dtIniVigencia <> UsuarDB.dtFinVigencia");
            psbQuery.AppendLine("and UsuarDB.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and UsuarDB.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and UsuarDB.iCodCatalogo = " + Session["iCodUsuarioDB"]);

            pRowUsuarDB = DSODataAccess.ExecuteDataRow(psbQuery.ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("where Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("and Emple.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Emple.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Emple.iCodCatalogo = " + pFields.GetByConfigName("Emple").DataValue);

            pRowEmple = DSODataAccess.ExecuteDataRow(psbQuery.ToString());

            psIdioma = Globals.GetCurrentLanguage();
            if (pRowEmple["Usuar"] != DBNull.Value)
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select * ");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','" + Globals.GetCurrentLanguage() + "')] Usuar");
                psbQuery.AppendLine("where Usuar.dtIniVigencia <> Usuar.dtFinVigencia");
                psbQuery.AppendLine("and Usuar.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and Usuar.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and Usuar.iCodCatalogo = " + Util.IsDBNull(pRowEmple["Usuar"],0));

                pRowUsuarEmple = DSODataAccess.ExecuteDataRow(psbQuery.ToString());
                if (pRowUsuarEmple["IdiomaCod"] != DBNull.Value)
                {
                    psIdioma = pRowUsuarEmple["IdiomaCod"].ToString();
                }
            }

            EnviarCorreoEmpleado();
            EnviarCorreoRespSitio();

            if (psbErrores.Length > 0)
            {
                string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
                string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".EnvioCorreos", lsError, lsTitulo);
            }
        }

        protected virtual void EnviarCorreoEmpleado()
        {
            DateTime ldtAhora = DateTime.Now;

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select * ");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmple','Notificación Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] NotifPrepEmple");
                psbQuery.AppendLine("where NotifPrepEmple.dtIniVigencia <> NotifPrepEmple.dtFinVigencia");
                psbQuery.AppendLine("and NotifPrepEmple.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and NotifPrepEmple.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and NotifPrepEmple.iCodCatalogo = " + Util.IsDBNull(pRowConfigEmpre["NotifPrepEmple"], 0).ToString());

                DataTable ltblNotifPrepEmple = DSODataAccess.Execute(psbQuery.ToString());

                if (ltblNotifPrepEmple.Rows.Count == 0)
                {
                    //Si la empresa no tiene configurado el envio de correo al empleado al asignar presupuesto temporal
                    //entonces no se puede enviar el correo
                    return;
                }
                pRowNotifPrepEmple = ltblNotifPrepEmple.Rows[0];

                DataTable ltblNotifPrepSitio = ptblNotifPrepSitio.Clone();
                piMaxMinutosSitio = 0;
                foreach (DataRow lDataRow in ptblNotifPrepSitio.Rows)
                {
                    if (((int)lDataRow["BanderasPrepSitios"] & 1) == 1) //Generar archivo de altas por presupuestos temporales
                    {
                        piMaxMinutosSitio = Math.Max(piMaxMinutosSitio, (int)lDataRow["Minutos"]);
                        ltblNotifPrepSitio.ImportRow(lDataRow);
                    }
                }
                if (ltblNotifPrepSitio.Rows.Count == 0)
                {
                    //Si ningun sitio esta configurado para generar el archivo de altas por presupuestos temporales
                    //entonces no se puede enviar el correo ya que se tiene que avisar al empleado que en cierto lapso de tiempo sus
                    //codigos/extensiones seran dados de alta
                    return;
                }

                string lsFileName = GetFileNameCorreo();

                PrepararMensajeCorreoEmpleado();

                pWord.FilePath = lsFileName;
                pWord.SalvarComo();
                pWord.Cerrar(true);
                pWord.Salir();
                pWord = null;

                pMail = new MailAccess();
                pMail.NotificarSiHayError = false;
                pMail.IsHtml = true;
                pMail.De = GetRemitente();
                pMail.Asunto = GetAsuntoCorreoEmpleado();
                pMail.AgregarWord(lsFileName);

                pMail.Para.Add(pRowEmple["Email"].ToString());

                if (pRowNotifPrepEmple["CtaCC"] != DBNull.Value && !String.IsNullOrEmpty(pRowNotifPrepEmple["CtaCC"].ToString()))
                {
                    pMail.CC.Add(pRowNotifPrepEmple["CtaCC"].ToString());
                }
                if (pRowNotifPrepEmple["CtaCCO"] != DBNull.Value && !String.IsNullOrEmpty(pRowNotifPrepEmple["CtaCCO"].ToString()))
                {
                    pMail.BCC.Add(pRowNotifPrepEmple["CtaCCO"].ToString());
                }


                pMail.Enviar();
            }
            catch (Exception Ex)
            {
                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgErrEnvioCorreoPrepTemp"));
                psbErrores.Append("<li>" + lsError + "</li>");
            }
            finally
            {
                if (pWord != null)
                {
                    pWord.Cerrar(true);
                    pWord = null;
                }
            }
        }

        protected MailAddress GetRemitente()
        {
            //Obtener remitente para el correo en base a la configuracion del cliente
            if (string.IsNullOrEmpty(pRowClient["CtaDe"].ToString())
                || string.IsNullOrEmpty(pRowClient["NomRemitente"].ToString()))
            {
                return new MailAddress(Util.AppSettings("appeMailID"));
            }
            else
            {
                return new MailAddress(pRowClient["CtaDe"].ToString(), pRowClient["NomRemitente"].ToString());
            }
        }

        protected string GetAsuntoCorreoEmpleado()
        {
            //Procesar el asunto del correo que se le envia al empleado
            string lsAsunto = "";
            DateTime ldtAhora = DateTime.Now;

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Asunto','Asunto de Correo Electrónico','" + Globals.GetCurrentLanguage() + "')] Asunto");
            psbQuery.AppendLine("where Asunto.dtIniVigencia <> Asunto.dtFinVigencia");
            psbQuery.AppendLine("and Asunto.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Asunto.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Asunto.iCodCatalogo = " + Util.IsDBNull(pRowNotifPrepEmple["Asunto"], 0));

            DataTable lDataTable = DSODataAccess.Execute(psbQuery.ToString());
            if (lDataTable.Rows.Count > 0)
            {
                lsAsunto = lDataTable.Rows[0][psIdioma].ToString();
            }
            if (!String.IsNullOrEmpty(lsAsunto))
            {
                lsAsunto = lsAsunto.Replace("Param(Client)", pRowClient["vchDescripcion"].ToString());
                lsAsunto = lsAsunto.Replace("Param(Empre)", pRowEmpre["vchDescripcion"].ToString());
                lsAsunto = lsAsunto.Replace("Param(Emple)", pRowEmple["NomCompleto"].ToString());
                lsAsunto = lsAsunto.Replace("Param(FechaInicioPrep)", pdtIniPeriodoActual.ToString(psDateFormat));
                lsAsunto = lsAsunto.Replace("Param(FechaFinPrep)", pdtIniPeriodoSiguiente.AddDays(-1).ToString(psDateFormat));
                lsAsunto = lsAsunto.Replace("Param(FechaActual)", DateTime.Now.ToString(psDateFormat));
                lsAsunto = lsAsunto.Replace("Param(TipoPr)", pRowTipoPr[psIdioma].ToString());
                lsAsunto = lsAsunto.Replace("Param(PeriodoPr)", pRowPeriodoPr[psIdioma].ToString());
                lsAsunto = lsAsunto.Replace("Param(ValorConsumoBase)", pFields.GetByConfigName("ValorConsumoBase").DataValue.ToString());
                lsAsunto = lsAsunto.Replace("Param(PresupProv)", pFields.GetByConfigName("PresupProv").DataValue.ToString());
                lsAsunto = lsAsunto.Replace("Param(Minutos)", piMaxMinutosSitio.ToString());
            }

            return lsAsunto;
        }

        protected string GetFileNameCorreo()
        {
            string lsFileKey = Guid.NewGuid().ToString();
            string lsTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), DSOUpload.EscapeFolderName(Session.SessionID));
            System.IO.Directory.CreateDirectory(lsTempPath);

            return System.IO.Path.Combine(lsTempPath, "msg." + lsFileKey + ".docx");
        }

        protected virtual void PrepararMensajeCorreoEmpleado()
        {
            pWord = new WordAccess();
            pWord.FilePath = Alarma.buscarPlantilla(pRowNotifPrepEmple["Plantilla"].ToString(), psIdioma);
            pWord.Abrir(true);

            pWord.ReemplazarTexto("Param(Client)", pRowClient["vchDescripcion"].ToString());
            pWord.ReemplazarTexto("Param(Empre)", pRowEmpre["vchDescripcion"].ToString());
            pWord.ReemplazarTexto("Param(Emple)", pRowEmple["NomCompleto"].ToString());
            pWord.ReemplazarTexto("Param(FechaInicioPrep)", pdtIniPeriodoActual.ToString(psDateFormat));
            pWord.ReemplazarTexto("Param(FechaFinPrep)", pdtIniPeriodoSiguiente.AddDays(-1).ToString(psDateFormat));
            pWord.ReemplazarTexto("Param(FechaActual)", DateTime.Now.ToString(psDateFormat));
            pWord.ReemplazarTexto("Param(TipoPr)", pRowTipoPr[psIdioma].ToString());
            pWord.ReemplazarTexto("Param(PeriodoPr)", pRowPeriodoPr[psIdioma].ToString());
            pWord.ReemplazarTexto("Param(ValorConsumoBase)", pFields.GetByConfigName("ValorConsumoBase").DataValue.ToString());
            pWord.ReemplazarTexto("Param(PresupProv)", pFields.GetByConfigName("PresupProv").DataValue.ToString());
            pWord.ReemplazarTexto("Param(Minutos)", piMaxMinutosSitio.ToString());
            
            if (pRowNotifPrepEmple["RepEst"] != DBNull.Value)
            {
                ProcesaReporteCorreoEmpleado();
            }
        }

        protected virtual void ProcesaReporteCorreoEmpleado()
        {
            string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
            string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
            Hashtable lHTParam = GetHTParam();
            Hashtable lHTParamDesc = GetHTParamDesc();

            ReporteEstandarUtil lReporteEstandarUtil = new ReporteEstandarUtil((int)pRowNotifPrepEmple["RepEst"], lHTParam, lHTParamDesc, lsKeytiaWebFPath, lsStylePath);
            lReporteEstandarUtil.ExportDOC(pWord);
        }

        protected virtual Hashtable GetHTParam()
        {
            Hashtable lHTParam = new Hashtable();

            lHTParam.Add("Schema", DSODataContext.Schema);
            lHTParam.Add("FechaIniRep", "'" + pdtIniPeriodoActual.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lHTParam.Add("FechaFinRep", "'" + pdtIniPeriodoSiguiente.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lHTParam.Add("vchCodIdioma", psIdioma);
            lHTParam.Add("vchCodMoneda", pRowUsuarDB["MonedaCod"]);
            if (pRowUsuarEmple != null)
            {
                lHTParam.Add("iCodUsuario", pRowUsuarEmple["iCodCatalogo"]);
                lHTParam.Add("iCodPerfil", pRowUsuarEmple["Perfil"]);
            }
            else
            {
                lHTParam.Add("iCodUsuario", Session["iCodUsuario"]);
                lHTParam.Add("iCodPerfil", Session["iCodPerfil"]);
            }

            lHTParam.Add("Client", pRowClient["iCodCatalogo"]);
            lHTParam.Add("Empre", pRowEmpre["iCodCatalogo"]);
            lHTParam.Add("Emple", pRowEmple["iCodCatalogo"].ToString());
            lHTParam.Add("FechaInicioPrep", "'" + pdtIniPeriodoActual.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lHTParam.Add("FechaFinPrep", "'" + pdtIniPeriodoSiguiente.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lHTParam.Add("FechaActual", "'" + DateTime.Today.ToString("yyyy-MM-dd") + "'");
            lHTParam.Add("TipoPr", pRowTipoPr["iCodCatalogo"]);
            lHTParam.Add("PeriodoPr", pRowPeriodoPr["iCodCatalogo"]);
            lHTParam.Add("ValorConsumoBase", pFields.GetByConfigName("ValorConsumoBase").DataValue);
            lHTParam.Add("PresupProv", pFields.GetByConfigName("PresupProv").DataValue);

            return lHTParam;
        }

        protected virtual Hashtable GetHTParamDesc()
        {
            Hashtable lHTParamDesc = new Hashtable();

            lHTParamDesc.Add("FechaIniRep", pdtIniPeriodoActual.ToString(psDateFormat));
            lHTParamDesc.Add("FechaFinRep", pdtIniPeriodoSiguiente.AddDays(-1).ToString(psDateFormat));

            lHTParamDesc.Add("Client", pRowClient["vchDescripcion"]);
            lHTParamDesc.Add("Empre", pRowEmpre["vchDescripcion"]);
            lHTParamDesc.Add("Emple", pRowEmple["NomCompleto"]);
            lHTParamDesc.Add("FechaInicioPrep", pdtIniPeriodoActual.ToString(psDateFormat));
            lHTParamDesc.Add("FechaFinPrep", pdtIniPeriodoSiguiente.AddDays(-1).ToString(psDateFormat));
            lHTParamDesc.Add("FechaActual", DateTime.Now.ToString(psDateFormat));
            lHTParamDesc.Add("TipoPr", pRowTipoPr[psIdioma]);
            lHTParamDesc.Add("PeriodoPr", pRowPeriodoPr[psIdioma]);
            lHTParamDesc.Add("ValorConsumoBase", pFields.GetByConfigName("ValorConsumoBase").ToString());
            lHTParamDesc.Add("PresupProv", pFields.GetByConfigName("PresupProv").ToString());

            return lHTParamDesc;
        }

        protected virtual void EnviarCorreoRespSitio()
        {
            DateTime ldtAhora = DateTime.Now;
            DataTable ltblNotifPrepSitio = ptblNotifPrepSitio.Clone();
            foreach (DataRow lDataRow in ptblNotifPrepSitio.Rows)
            {
                if (((int)lDataRow["BanderasPrepSitios"] & 2) == 2) //Enviar correo al asignar presupuesto temporal
                {
                    ltblNotifPrepSitio.ImportRow(lDataRow);
                }
            }
            if (ltblNotifPrepSitio.Rows.Count == 0)
            {
                //Si ningun sitio esta configurado para enviar correo al asignar el presupuesto temporal entonces no hago nada
                return;
            }

            List<string> lstNotifPrepSitio = new List<string>();
            List<string> lstSitio = new List<string>();
            foreach (DataRow lRowNotifPrepSitio in ltblNotifPrepSitio.Rows)
            {
                lstNotifPrepSitio.Add(lRowNotifPrepSitio["iCodCatalogo"].ToString());
                lstSitio.Add(lRowNotifPrepSitio["Sitio"].ToString());
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepSitio','Notificación por Presupuesto Temporal a Empleado','" + Globals.GetCurrentLanguage() + "')] PrepSitio");
            psbQuery.AppendLine("where PrepSitio.dtIniVigencia <> PrepSitio.dtFinVigencia");
            psbQuery.AppendLine("and PrepSitio.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and PrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and PrepSitio.NotifPrepSitio in(" + String.Join(",",lstNotifPrepSitio.ToArray())+")");

            DataTable ltblPrepSitioEnvio = DSODataAccess.Execute(psbQuery.ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Sitio','" + Globals.GetCurrentLanguage() + "')] Sitio");
            psbQuery.AppendLine("where Sitio.dtIniVigencia <> Sitio.dtFinVigencia");
            psbQuery.AppendLine("and Sitio.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Sitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Sitio.iCodCatalogo in(" + String.Join(",", lstSitio.ToArray()) + ")");

            DataTable ltblSitio = DSODataAccess.Execute(psbQuery.ToString());
            DataTable lDataTable;
            foreach (DataRow lRowNotifPrepSitio in ltblNotifPrepSitio.Rows)
            {
                pRowNotifPrepSitio = lRowNotifPrepSitio;
                pRowSitio = ltblSitio.Select("iCodCatalogo = " + pRowNotifPrepSitio["Sitio"].ToString())[0];
                if (ltblPrepSitioEnvio.Select("NotifPrepSitio = " + pRowNotifPrepSitio["iCodCatalogo"].ToString()).Length > 0)
                {
                    pRowPrepSitioEnvio = ltblPrepSitioEnvio.Select("NotifPrepSitio = " + pRowNotifPrepSitio["iCodCatalogo"].ToString())[0];

                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select * ");
                    psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
                    psbQuery.AppendLine("where Emple.dtIniVigencia <> Emple.dtFinVigencia");
                    psbQuery.AppendLine("and Emple.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    psbQuery.AppendLine("and Emple.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    psbQuery.AppendLine("and Emple.iCodCatalogo = " + Util.IsDBNull(pRowSitio["Emple"],0));

                    lDataTable = DSODataAccess.Execute(psbQuery.ToString());
                    pRowEmpleRespSitio = null;
                    pRowUsuarRespSitio = null;
                    psIdioma = Globals.GetCurrentLanguage();
                    if (lDataTable.Rows.Count > 0)
                    {
                        pRowEmpleRespSitio = lDataTable.Rows[0];

                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select * ");
                        psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','" + Globals.GetCurrentLanguage() + "')] Usuar");
                        psbQuery.AppendLine("where Usuar.dtIniVigencia <> Usuar.dtFinVigencia");
                        psbQuery.AppendLine("and Usuar.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        psbQuery.AppendLine("and Usuar.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        psbQuery.AppendLine("and Usuar.iCodCatalogo = " + Util.IsDBNull(pRowEmpleRespSitio["Usuar"],0));

                        lDataTable = DSODataAccess.Execute(psbQuery.ToString());
                        if (lDataTable.Rows.Count > 0)
                        {
                            pRowUsuarRespSitio = DSODataAccess.ExecuteDataRow(psbQuery.ToString());
                            if (pRowUsuarRespSitio["IdiomaCod"] != DBNull.Value)
                            {
                                psIdioma = pRowUsuarRespSitio["IdiomaCod"].ToString();
                            }
                        }
                    }

                    ProcesaCorreoRespSitio();
                }
            }
        }

        protected virtual void ProcesaCorreoRespSitio()
        {
            DateTime ldtAhora = DateTime.Now;

            try
            {
                string lsCtaPara = GetParaRespSitio();
                string lsFileName = GetFileNameCorreo();

                if (String.IsNullOrEmpty(lsCtaPara))
                {
                    //Si no tengo a quien enviar el correo entonces no hago nada
                    return;
                }

                PrepararMensajeCorreoRespSitio();

                pWord.FilePath = lsFileName;
                pWord.SalvarComo();
                pWord.Cerrar(true);
                pWord.Salir();
                pWord = null;

                pMail = new MailAccess();
                pMail.NotificarSiHayError = false;
                pMail.IsHtml = true;
                pMail.De = GetRemitente();
                pMail.Asunto = GetAsuntoCorreoRespSitio();
                pMail.AgregarWord(lsFileName);
                pMail.Para.Add(lsCtaPara);

                pMail.Enviar();
            }
            catch (Exception Ex)
            {
                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgErrEnvioCorreoPrepTempSitio", pRowSitio["vchDescripcion"].ToString()));
                psbErrores.Append("<li>" + lsError + "</li>");
            }
            finally
            {
                if (pWord != null)
                {
                    pWord.Cerrar(true);
                    pWord = null;
                }
            }
        }

        protected string GetParaRespSitio()
        {
            //Obtener la direccion del responsable del sitio
            if (pRowPrepSitioEnvio["CtaPara"] != DBNull.Value)
            {
                return pRowPrepSitioEnvio["CtaPara"].ToString();
            }
            else if (pRowEmpleRespSitio != null && pRowEmpleRespSitio["Email"] != DBNull.Value)
            {
                return pRowEmpleRespSitio["Email"].ToString();
            }

            return "";
        }

        protected string GetAsuntoCorreoRespSitio()
        {
            //Procesar el asunto del correo que se le envia al empleado
            string lsAsunto = "";
            DateTime ldtAhora = DateTime.Now;

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Asunto','Asunto de Correo Electrónico','" + Globals.GetCurrentLanguage() + "')] Asunto");
            psbQuery.AppendLine("where Asunto.dtIniVigencia <> Asunto.dtFinVigencia");
            psbQuery.AppendLine("and Asunto.dtIniVigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Asunto.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Asunto.iCodCatalogo = " + Util.IsDBNull(pRowPrepSitioEnvio["Asunto"], 0));

            DataTable lDataTable = DSODataAccess.Execute(psbQuery.ToString());
            if (lDataTable.Rows.Count > 0)
            {
                lsAsunto = lDataTable.Rows[0][psIdioma].ToString();
            }
            if (!String.IsNullOrEmpty(lsAsunto))
            {
                lsAsunto = lsAsunto.Replace("Param(Client)", pRowClient["vchDescripcion"].ToString());
                lsAsunto = lsAsunto.Replace("Param(Empre)", pRowEmpre["vchDescripcion"].ToString());
                lsAsunto = lsAsunto.Replace("Param(Emple)", pRowEmple["NomCompleto"].ToString());
                if (pRowEmpleRespSitio != null)
                {
                    lsAsunto = lsAsunto.Replace("Param(EmpleRespSitio)", pRowEmpleRespSitio["NomCompleto"].ToString());
                }
                else
                {
                    lsAsunto = lsAsunto.Replace("Param(EmpleRespSitio)", "");
                }
                lsAsunto = lsAsunto.Replace("Param(FechaInicioPrep)", pdtIniPeriodoActual.ToString(psDateFormat));
                lsAsunto = lsAsunto.Replace("Param(FechaFinPrep)", pdtIniPeriodoSiguiente.AddDays(-1).ToString(psDateFormat));
                lsAsunto = lsAsunto.Replace("Param(FechaActual)", DateTime.Now.ToString(psDateFormat));
                lsAsunto = lsAsunto.Replace("Param(TipoPr)", pRowTipoPr[psIdioma].ToString());
                lsAsunto = lsAsunto.Replace("Param(PeriodoPr)", pRowPeriodoPr[psIdioma].ToString());
                lsAsunto = lsAsunto.Replace("Param(ValorConsumoBase)", pFields.GetByConfigName("ValorConsumoBase").DataValue.ToString());
                lsAsunto = lsAsunto.Replace("Param(PresupProv)", pFields.GetByConfigName("PresupProv").DataValue.ToString());
                lsAsunto = lsAsunto.Replace("Param(Minutos)", pRowNotifPrepSitio["Minutos"].ToString());
            }

            return lsAsunto;
        }
        
        protected virtual void PrepararMensajeCorreoRespSitio()
        {
            pWord = new WordAccess();
            pWord.FilePath = Alarma.buscarPlantilla(pRowPrepSitioEnvio["Plantilla"].ToString(), psIdioma);
            pWord.Abrir(true);

            pWord.ReemplazarTexto("Param(Client)", pRowClient["vchDescripcion"].ToString());
            pWord.ReemplazarTexto("Param(Empre)", pRowEmpre["vchDescripcion"].ToString());
            pWord.ReemplazarTexto("Param(Emple)", pRowEmple["NomCompleto"].ToString());
            if (pRowEmpleRespSitio != null)
            {
                pWord.ReemplazarTexto("Param(EmpleRespSitio)", pRowEmpleRespSitio["NomCompleto"].ToString());
            }
            else
            {
                pWord.ReemplazarTexto("Param(EmpleRespSitio)", "");
            }
            pWord.ReemplazarTexto("Param(FechaInicioPrep)", pdtIniPeriodoActual.ToString(psDateFormat));
            pWord.ReemplazarTexto("Param(FechaFinPrep)", pdtIniPeriodoSiguiente.AddDays(-1).ToString(psDateFormat));
            pWord.ReemplazarTexto("Param(FechaActual)", DateTime.Now.ToString(psDateFormat));
            pWord.ReemplazarTexto("Param(TipoPr)", pRowTipoPr[psIdioma].ToString());
            pWord.ReemplazarTexto("Param(PeriodoPr)", pRowPeriodoPr[psIdioma].ToString());
            pWord.ReemplazarTexto("Param(ValorConsumoBase)", pFields.GetByConfigName("ValorConsumoBase").DataValue.ToString());
            pWord.ReemplazarTexto("Param(PresupProv)", pFields.GetByConfigName("PresupProv").DataValue.ToString());
            pWord.ReemplazarTexto("Param(Minutos)", pRowNotifPrepSitio["Minutos"].ToString());

            if (pRowPrepSitioEnvio["RepEst"] != DBNull.Value)
            {
                ProcesaReporteCorreoRespSitio();
            }
        }
        
        protected virtual void ProcesaReporteCorreoRespSitio()
        {
            string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
            string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
            Hashtable lHTParam = GetHTParamRespSitio();
            Hashtable lHTParamDesc = GetHTParamDescRespSitio();

            ReporteEstandarUtil lReporteEstandarUtil = new ReporteEstandarUtil((int)pRowPrepSitioEnvio["RepEst"], lHTParam, lHTParamDesc, lsKeytiaWebFPath, lsStylePath);
            lReporteEstandarUtil.ExportDOC(pWord);
        }

        protected virtual Hashtable GetHTParamRespSitio()
        {
            Hashtable lHTParam = GetHTParam();

            if (pRowUsuarRespSitio != null)
            {
                lHTParam["iCodUsuario"] = pRowUsuarRespSitio["iCodCatalogo"];
                lHTParam["iCodPerfil"] = pRowUsuarRespSitio["Perfil"];
            }
            else
            {
                lHTParam["iCodUsuario"] =  Session["iCodUsuario"];
                lHTParam["iCodPerfil"] =  Session["iCodPerfil"];
            }

            lHTParam.Add("Sitio", pRowSitio["iCodCatalogo"]);
            lHTParam.Add("EmpleRespSitio", pRowSitio["Emple"]);

            return lHTParam;
        }

        protected virtual Hashtable GetHTParamDescRespSitio()
        {
            Hashtable lHTParamDesc = GetHTParamDesc();

            lHTParamDesc.Add("Sitio", pRowSitio["vchDescripcion"].ToString());
            if (pRowEmpleRespSitio != null)
            {
                lHTParamDesc.Add("EmpleRespSitio", pRowEmpleRespSitio["NomCompleto"].ToString());
            }
            else
            {
                lHTParamDesc.Add("EmpleRespSitio", "");
            }

            return lHTParamDesc;
        }

        public override void PostAgregarSubHistoricField()
        {
            string lsTitulo = DSOControl.JScriptEncode(this.Historico.AlertTitle);

            base.PostAgregarSubHistoricField();
            DateTime ldtAhora = DateTime.Now;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select *");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] PrepFijo");
            psbQuery.AppendLine("where PrepFijo.Emple = " + pFields.GetByConfigName("Emple").DataValue);
            psbQuery.AppendLine("and PrepFijo.dtIniVigencia <> PrepFijo.dtFinVigencia");
            psbQuery.AppendLine("and PrepFijo.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and PrepFijo.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable ltblPrepFijo = DSODataAccess.Execute(psbQuery.ToString());
            if (ltblPrepFijo.Rows.Count == 0)
            {
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValPrepFijoRequerido")) + "</span>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsMsg, lsTitulo);
                this.Historico.RemoverSubHistorico();
            }
            else
            {
                double ldConsumoBase = 0;
                double ldPresupuesto = 0;

                if (pRowConsumoBase != null)
                {
                    ldConsumoBase = (double)pRowConsumoBase["Consumo"];
                }

                psbQuery.Length = 0;
                psbQuery.AppendLine("select top 1 *");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] PrepTemporal");
                psbQuery.AppendLine("where PrepTemporal.Emple = " + pFields.GetByConfigName("Emple").DataValue);
                psbQuery.AppendLine("and PrepTemporal.dtIniVigencia <> PrepTemporal.dtFinVigencia");
                psbQuery.AppendLine("and PrepTemporal.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and PrepTemporal.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("order by ValorConsumoBase Desc");

                DataTable ltblPrepTemporal = DSODataAccess.Execute(psbQuery.ToString());
                if (ltblPrepTemporal.Rows.Count > 0)
                {
                    ldPresupuesto = (double)ltblPrepTemporal.Rows[0]["ValorConsumoBase"] + (double)ltblPrepTemporal.Rows[0]["PresupProv"];
                }
                else
                {
                    ldPresupuesto = (double)ltblPrepFijo.Rows[0]["PresupFijo"];
                }

                if (ldConsumoBase < ldPresupuesto)
                {
                    string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValPrepNoConsumido")) + "</span>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsMsg, lsTitulo);
                    this.Historico.RemoverSubHistorico();
                }
            }
        }
    }
}
