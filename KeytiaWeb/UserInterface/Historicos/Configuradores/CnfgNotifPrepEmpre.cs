using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Reflection;
using System.Web.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KeytiaServiceBL.Alarmas;

namespace KeytiaWeb.UserInterface
{
    public class CnfgNotifPrepEmpre : HistoricEdit
    {
        protected StringBuilder psbErrores;

        public CnfgNotifPrepEmpre()
        {
            Init += new EventHandler(CnfgNotifPrepEmpre_Init);
        }

        void CnfgNotifPrepEmpre_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgNotifPrepEmpre";
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgNotifPrepEmpre.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgNotifPrepEmpre.js") + "'type='text/javascript'></script>\r\n", true, false);
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                AgregarBoton("Asunto");
                AgregarBoton("NotifPrepEmple", "KeytiaWeb.UserInterface.CnfgNotifPrepTemporal", "KeytiaWeb.UserInterface.HistoricFieldCollection");
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;
        }

        protected override bool ValidarVigencias()
        {
            if (State == HistoricState.Edicion)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

            //el usuario nunca edita las vigencias por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarClaves()
        {
            //el usuario nunca edita las claves por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarAtribCatalogosVig()
        {
            //el usuario solamente puede seleccionar catalogos de entidades que estan vigentes todo el tiempo
            return true;
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                psbErrores = new StringBuilder();

                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                pvchDescripcion.DataValue = pFields.GetByConfigName("Empre").ToString();

                ValidarNivelAlertas();
                ValidarEmail("CtaCC");
                ValidarEmail("CtaCCO");
                ValidarPlantilla("Plantilla");
                ValidarPlantillaReporte("Plantilla");
                ValidarDiaInicioPeriodo();
                ValidarEliminacionRequerida();

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected void ValidarNivelAlertas()
        {
            string lsError;

            //el primer nivel de alerta no debe de ser cero
            if (!pFields.GetByConfigName("NivelAlerta1").DSOControlDB.HasValue)
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("NivelAlerta1").Descripcion));
                psbErrores.Append("<li>" + lsError + "</li>");
            }
            else
            {
                double ldNivel1;
                double.TryParse(pFields.GetByConfigName("NivelAlerta1").DataValue.ToString(), out ldNivel1);
                if (ldNivel1 <= 0 || ldNivel1 > 100)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValRangoNivelAlerta1", pFields.GetByConfigName("NivelAlerta1").Descripcion));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
            }

            ValidarNivelAlerta("NivelAlerta2", "NivelAlerta1");
            ValidarNivelAlerta("NivelAlerta3", "NivelAlerta2");
            ValidarNivelAlerta("NivelAlerta4", "NivelAlerta3");
        }

        protected void ValidarNivelAlerta(string lsNivelAlerta, string lsNivelAnterior)
        {
            if (pFields.ContainsConfigName(lsNivelAlerta) && pFields.ContainsConfigName(lsNivelAnterior))
            {
                string lsError;
                double ldNivelAnterior = 0;
                double ldNivelAlerta = 0;
                if (pFields.GetByConfigName(lsNivelAlerta).DSOControlDB.HasValue)
                {
                    double.TryParse(pFields.GetByConfigName(lsNivelAlerta).DataValue.ToString(), out ldNivelAlerta);
                    if (ldNivelAlerta < 0 || ldNivelAlerta > 100)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValRangoNivelAlerta", pFields.GetByConfigName(lsNivelAlerta).Descripcion));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
                if (pFields.GetByConfigName(lsNivelAnterior).DSOControlDB.HasValue)
                {
                    double.TryParse(pFields.GetByConfigName(lsNivelAnterior).DataValue.ToString(), out ldNivelAnterior);
                }

                if (ldNivelAlerta > 0 && ldNivelAlerta <= ldNivelAnterior)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValNivelAlertaAnterior", pFields.GetByConfigName(lsNivelAlerta).Descripcion, pFields.GetByConfigName(lsNivelAnterior).Descripcion));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
            }
        }

        protected void ValidarEmail(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {

                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string email = lField.DSOControlDB.ToString().Trim();
                string lsError;
                Regex regexp = new Regex("^[a-zA-Z]{1}[a-zA-Z0-9_-]{2,}([.][a-zA-Z0-9_-]+)*[@][a-zA-Z0-9_-]{3,}([.][a-zA-Z0-9_-]+)*[.][a-zA-Z]{2,4}$");

                if (!String.IsNullOrEmpty(email))
                {
                    foreach (string mail in email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!regexp.IsMatch(mail))
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmailFormat", mail, lField.Descripcion));
                            psbErrores.Append("<li>" + lsError + "</li>");
                        }
                    }
                }
            }
        }

        protected void ValidarPlantilla(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string lsPlantilla = lField.DSOControlDB.ToString();
                string lsError;
                if (!String.IsNullOrEmpty(lsPlantilla))
                {
                    string lsFilePath = lsPlantilla;
                    if (System.IO.File.Exists(lsFilePath))
                        return;

                    lsFilePath = System.IO.Path.Combine(lsFilePath, Globals.GetCurrentLanguage());
                    if (System.IO.Directory.Exists(lsFilePath))
                    {
                        foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                        {
                            if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                            {
                                return;
                            }
                        }
                    }
                    lsFilePath = lsPlantilla;
                    if (System.IO.Directory.Exists(lsFilePath))
                    {
                        foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                        {
                            if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                            {
                                return;
                            }
                        }
                    }
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "PlantNotifPrepEmpre"));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
            }
        }

        protected void ValidarPlantillaReporte(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string lsPlantilla = lField.DSOControlDB.ToString();
                bool lbHayError = false;
                string lsError;

                if (pFields.GetByConfigName("RepEst").DSOControlDB.HasValue)
                {
                    if (String.IsNullOrEmpty(lsPlantilla) || !pFields.GetByConfigName("Idioma").DSOControlDB.HasValue)
                    {
                        lbHayError = true;
                    }
                    else
                    {
                        DataTable loTabla = new DataTable();
                        loTabla.Columns.Add("c1");
                        DataRow ldr = loTabla.NewRow();
                        ldr["c1"] = "c1";
                        loTabla.Rows.Add(ldr);
                        string lsFilePath = UtilAlarma.buscarPlantilla(lsPlantilla,
                            Alarma.getIdioma(int.Parse(pFields.GetByConfigName("Idioma").DSOControlDB.DataValue.ToString())));
                        if (!string.IsNullOrEmpty(lsFilePath))
                        {
                            WordAccess loWord = new WordAccess();
                            loWord.FilePath = lsFilePath;
                            try
                            {
                                loWord.Abrir();

                                EstiloTablaWord pEstiloTablaWord = new EstiloTablaWord();
                                pEstiloTablaWord.Estilo = "KeytiaGrid";
                                pEstiloTablaWord.FilaEncabezado = true;
                                pEstiloTablaWord.FilasBandas = true;
                                pEstiloTablaWord.PrimeraColumna = false;
                                pEstiloTablaWord.UltimaColumna = false;
                                pEstiloTablaWord.ColumnasBandas = false;
                                loWord.InsertarTabla(loTabla, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);
                                pEstiloTablaWord.Estilo = "KeytiaHeaderRep";
                                loWord.InsertarTabla(loTabla, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);
                                loWord.InsertarTexto("Titulo");
                                loWord.SetStyle("TituloReporte");
                            }
                            catch (Exception ex)
                            {
                                lbHayError = true;
                            }
                            finally
                            {
                                loWord.Cerrar();
                                loWord.Salir();
                                loWord = null;
                            }
                        }
                    }
                    if (lbHayError)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "PlantRptNotifPrepEmpre"));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }

        protected void ValidarDiaInicioPeriodo()
        {
            string lsError;

            if (!pFields.GetByConfigName("PeriodoPr").DSOControlDB.HasValue)
            {
                return;
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("select *");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PeriodoPr','Periodo Presupuesto','" + Globals.GetCurrentLanguage() + "')] PeriodoPr");
            psbQuery.AppendLine("where PeriodoPr.dtIniVigencia <> PeriodoPr.dtFinVigencia");
            psbQuery.AppendLine("and PeriodoPr.iCodCatalogo = " + pFields.GetByConfigName("PeriodoPr").DataValue);

            DataTable ltblPeriodoPr = DSODataAccess.Execute(psbQuery.ToString());
            if (ltblPeriodoPr.Rows.Count > 0)
            {
                if (!pFields.GetByConfigName("DiaInicioPeriodo").DSOControlDB.HasValue
                    && (ltblPeriodoPr.Rows[0]["vchCodigo"].ToString() == "Semanal"
                    || ltblPeriodoPr.Rows[0]["vchCodigo"].ToString() == "Mensual"))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("DiaInicioPeriodo").Descripcion));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
                else if (ltblPeriodoPr.Rows[0]["vchCodigo"].ToString() == "Semanal"
                    && (int.Parse(pFields.GetByConfigName("DiaInicioPeriodo").DataValue.ToString()) < 1
                    || int.Parse(pFields.GetByConfigName("DiaInicioPeriodo").DataValue.ToString()) > 7))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValDiaInicioPrep", pFields.GetByConfigName("DiaInicioPeriodo").Descripcion, "1", "7"));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
                else if (ltblPeriodoPr.Rows[0]["vchCodigo"].ToString() == "Mensual"
                    && (int.Parse(pFields.GetByConfigName("DiaInicioPeriodo").DataValue.ToString()) < 1
                    || int.Parse(pFields.GetByConfigName("DiaInicioPeriodo").DataValue.ToString()) > 31))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValDiaInicioPrep", pFields.GetByConfigName("DiaInicioPeriodo").Descripcion, "1", "31"));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
            }
        }

        protected void ValidarEliminacionRequerida()
        {
            DataTable lDataTable;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
            psbQuery.AppendLine("where PrepEmpre.iCodRegistro = " + iCodRegistro);

            lDataTable = DSODataAccess.Execute(psbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                if (pFields.GetByConfigName("EliminarConfigPresupuesto").DataValue.ToString() == "0"
                    && (pFields.GetByConfigName("TipoPr").DataValue.ToString() != lDataTable.Rows[0]["TipoPr"].ToString()
                    || pFields.GetByConfigName("PeriodoPr").DataValue.ToString() != lDataTable.Rows[0]["PeriodoPr"].ToString()
                    || ((lDataTable.Rows[0]["PeriodoPrCod"].ToString() == "Semanal" || lDataTable.Rows[0]["PeriodoPrCod"].ToString() == "Mensual")
                    && pFields.GetByConfigName("DiaInicioPeriodo").DataValue.ToString() != lDataTable.Rows[0]["DiaInicioPeriodo"].ToString())))
                {
                    string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValEliminarConfigPresupuesto"));
                    psbErrores.Append("<li>" + lsError + "</li>");
                }
            }
        }

        protected override void GrabarRegistro()
        {
            try
            {
                if (pFields.GetByConfigName("EliminarConfigPresupuesto").DataValue.ToString() == "1")
                {
                    EliminarConfigPresupuesto();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }

            base.GrabarRegistro();
        }

        protected void EliminarConfigPresupuesto()
        {
            //elimino los datos del presupuesto fijo de empleados
            psbQuery.Length = 0;
            psbQuery.AppendLine("update His set His.dtFinVigencia = His.dtIniVigencia");
            psbQuery.AppendLine("   ,His.dtFecUltAct = GetDate()");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] His");
            psbQuery.AppendLine("where His.Emple in(select Emple.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("   where Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("   and Emple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and Emple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and Emple.CenCos in(select CenCos.iCodCatalogo");
            psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("       where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("       and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("       and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("       and CenCos.Empre = " + pFields.GetByConfigName("Empre").DataValue + "))");
            psbQuery.AppendLine("and His.dtIniVigencia <> His.dtFinVigencia");
            psbQuery.AppendLine("and His.dtFinVigencia > " + pdtIniVigencia.DataValue);

            DSODataAccess.ExecuteNonQuery(psbQuery.ToString());

            //elimino los datos del presupuesto temporal de empleados
            psbQuery.Length = 0;
            psbQuery.AppendLine("update His set His.dtFinVigencia = His.dtIniVigencia");
            psbQuery.AppendLine("   ,His.dtFecUltAct = GetDate()");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] His");
            psbQuery.AppendLine("where His.Emple in(select Emple.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("   where Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("   and Emple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and Emple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and Emple.CenCos in(select CenCos.iCodCatalogo");
            psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("       where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("       and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("       and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("       and CenCos.Empre = " + pFields.GetByConfigName("Empre").DataValue + "))");
            psbQuery.AppendLine("and His.dtIniVigencia <> His.dtFinVigencia");
            psbQuery.AppendLine("and His.dtFinVigencia > " + pdtIniVigencia.DataValue);

            DSODataAccess.ExecuteNonQuery(psbQuery.ToString());


            //elimino los datos del presupuesto fijo de centro de costos
            psbQuery.Length = 0;
            psbQuery.AppendLine("update His set His.dtFinVigencia = His.dtIniVigencia");
            psbQuery.AppendLine("   ,His.dtFecUltAct = GetDate()");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] His");
            psbQuery.AppendLine("where His.CenCos in(select CenCos.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("   where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("   and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCos.Empre = " + pFields.GetByConfigName("Empre").DataValue + ")");
            psbQuery.AppendLine("and His.dtIniVigencia <> His.dtFinVigencia");
            psbQuery.AppendLine("and His.dtFinVigencia > " + pdtIniVigencia.DataValue);

            DSODataAccess.ExecuteNonQuery(psbQuery.ToString());

            //elimino los datos del presupuesto temporal de centro de costos
            psbQuery.Length = 0;
            psbQuery.AppendLine("update His set His.dtFinVigencia = His.dtIniVigencia");
            psbQuery.AppendLine("   ,His.dtFecUltAct = GetDate()");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] His");
            psbQuery.AppendLine("where His.CenCos in(select CenCos.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("   where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("   and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCos.Empre = " + pFields.GetByConfigName("Empre").DataValue + ")");
            psbQuery.AppendLine("and His.dtIniVigencia <> His.dtFinVigencia");
            psbQuery.AppendLine("and His.dtFinVigencia > " + pdtIniVigencia.DataValue);

            DSODataAccess.ExecuteNonQuery(psbQuery.ToString());

            ResetBitacora();
        }

        protected void ResetBitacora()
        {
            DateTime ldtAhora = DateTime.Now;
            DateTime ldtIniPeriodoActual;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
            psbQuery.AppendLine("where PrepEmpre.iCodRegistro = " + iCodRegistro);

            DataTable lDataTable = DSODataAccess.Execute(psbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                //Calcular el inicio de periodo de presupuesto
                if (lDataTable.Rows[0]["PeriodoPrCod"].ToString() == "Diario")
                {
                    ldtIniPeriodoActual = new DateTime(ldtAhora.Year, ldtAhora.Month, ldtAhora.Day);
                }
                else if (lDataTable.Rows[0]["PeriodoPrCod"].ToString() == "Semanal")
                {
                    double lDias;
                    if ((int)ldtAhora.DayOfWeek + 1 < (int)lDataTable.Rows[0]["DiaInicioPeriodo"])
                    {
                        lDias = (7 - (int)lDataTable.Rows[0]["DiaInicioPeriodo"]) + (int)ldtAhora.DayOfWeek + 1;
                    }
                    else
                    {
                        lDias = (int)ldtAhora.DayOfWeek + 1 - (int)lDataTable.Rows[0]["DiaInicioPeriodo"];
                    }

                    ldtIniPeriodoActual = ldtAhora.AddDays(-lDias);

                }
                else //Mensual
                {
                    DateTime ldtMesActual = new DateTime(ldtAhora.Year, ldtAhora.Month, 01); //fecha de inicio de periodo para el mes actual
                    DateTime ldtMesAnterior = ldtMesActual.AddMonths(-1);   //fecha de inicio de periodo para el mes anterior
                    ldtMesActual = ldtMesActual.AddDays((int)lDataTable.Rows[0]["DiaInicioPeriodo"] - 1);
                    if (ldtMesActual.Month > ldtAhora.Month)
                    {
                        ldtMesActual = ldtMesActual.AddDays(-ldtMesActual.Day);
                    }

                    ldtMesAnterior = ldtMesAnterior.AddDays((int)lDataTable.Rows[0]["DiaInicioPeriodo"] - 1);
                    if (ldtMesAnterior.Month == ldtAhora.Month)
                    {
                        ldtMesAnterior = ldtMesAnterior.AddDays(-ldtMesAnterior.Day);
                    }

                    //Si la fecha del ahora es menor que el inicio de periodo para el mes actual
                    //entonces nos encontramos en el periodo que inicio en el mes anterior
                    if (ldtAhora < ldtMesActual)
                    {
                        ldtIniPeriodoActual = ldtMesAnterior;
                    }
                    else
                    {
                        ldtIniPeriodoActual = ldtMesActual;
                    }
                }

                //reseteo los datos de la bitacora
                psbQuery.Length = 0;
                psbQuery.AppendLine("update Det set Det.FechaReset = GetDate()");
                psbQuery.AppendLine("   ,Det.dtFecUltAct = GetDate()");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisDetallados('Detall','Bitacora Notificacion Consumos','" + Globals.GetCurrentLanguage() + "')] Det");
                psbQuery.AppendLine("where Det.Emple in(select Emple.iCodCatalogo");
                psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
                psbQuery.AppendLine("   where Emple.dtIniVigencia <> Emple.dtFinVigencia");
                psbQuery.AppendLine("   and Emple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("   and Emple.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("   and Emple.CenCos in(select CenCos.iCodCatalogo");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                psbQuery.AppendLine("       where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                psbQuery.AppendLine("       and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.Empre = " + pFields.GetByConfigName("Empre").DataValue + "))");
                psbQuery.AppendLine("and Det.FechaReset is null");
                psbQuery.AppendLine("and Det.FechaInicioPrep >= '" + ldtIniPeriodoActual.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                DSODataAccess.ExecuteNonQuery(psbQuery.ToString());

                //reseteo los datos de la bitacora de consumo 100%
                psbQuery.Length = 0;
                psbQuery.AppendLine("update Det set Det.FechaReset = GetDate()");
                psbQuery.AppendLine("   ,Det.dtFecUltAct = GetDate()");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisDetallados('Detall','Bitacora Consumo 100%','" + Globals.GetCurrentLanguage() + "')] Det");
                psbQuery.AppendLine("where Det.Emple in(select Emple.iCodCatalogo");
                psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
                psbQuery.AppendLine("   where Emple.dtIniVigencia <> Emple.dtFinVigencia");
                psbQuery.AppendLine("   and Emple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("   and Emple.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("   and Emple.CenCos in(select CenCos.iCodCatalogo");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                psbQuery.AppendLine("       where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                psbQuery.AppendLine("       and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.Empre = " + pFields.GetByConfigName("Empre").DataValue + "))");
                psbQuery.AppendLine("and Det.FechaReset is null");
                psbQuery.AppendLine("and Det.FechaInicioPrep >= '" + ldtIniPeriodoActual.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                DSODataAccess.ExecuteNonQuery(psbQuery.ToString());
            }
        }

        #region WebMethods

        public static string SearchEmpreEmple(string term, int iCodEmpre, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("select top 100 *");
                lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
                lsbQuery.AppendLine("where Emple.CenCos in (select CenCos.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                lsbQuery.AppendLine("       where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                lsbQuery.AppendLine("       " + DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "CenCos"));
                lsbQuery.AppendLine("       and CenCos.Empre = " + iCodEmpre + ")");
                lsbQuery.AppendLine("and Emple.dtIniVigencia <> Emple.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "Emple"));
                lsbQuery.AppendLine("and Emple.vchDescripcion + ' (' + Emple.vchCodigo + ')' like '%" + term.Replace("'", "''") + "%'");
                lsbQuery.AppendLine("order by Emple.vchDescripcion");

                string lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Vista");
                lsbQuery.AppendLine("where Vista.iCodRegistro = (select Max(His.iCodRegistro) from Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Vista.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.Append(")");
                lsbQuery.AppendLine("order by vchDescripcion");
                DataTable ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = FillDSOAutoDataSource(ldtVista);
                string json = DSOControl.SerializeJSON<DataTable>(lDataSource);
                return json;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException(true, "ErrorGen", e);
            }
        }

        #endregion
    }
}
