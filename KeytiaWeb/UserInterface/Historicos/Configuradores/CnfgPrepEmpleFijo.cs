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

namespace KeytiaWeb.UserInterface
{
    public class CnfgPrepEmpleFijo : HistoricEdit
    {
        protected DateTime pdtIniPeriodoActual;
        protected DateTime pdtIniPeriodoSiguiente;
        protected DataRow pRowConfigEmpre = null;

        public CnfgPrepEmpleFijo()
        {
            Init += new EventHandler(CnfgPrepEmpleFijo_Init);
        }

        void CnfgPrepEmpleFijo_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgPrepEmple";
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;

            if (pFields != null && pFields.ContainsConfigName("PrepCenCos"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PrepCenCos").Row - 1].Visible = false;
            }
        }

        public virtual void CalcularDatosEmpresa()
        {
            ObtenerConfiguracionEmpresa();
            if (pRowConfigEmpre != null)
            {
                pFields.GetByConfigName("TipoPr").DataValue = pRowConfigEmpre["TipoPr"];
                pFields.GetByConfigName("PeriodoPr").DataValue = pRowConfigEmpre["PeriodoPr"];
                pFields.GetByConfigName("FechaInicioPrep").DataValue = pdtIniPeriodoActual;
            }

            pFields.GetByConfigName("TipoPr").DisableField();
            pFields.GetByConfigName("PeriodoPr").DisableField();
            pFields.GetByConfigName("FechaInicioPrep").DisableField();
        }

        protected virtual void ObtenerConfiguracionEmpresa()
        {
            DateTime ldtAhora = DateTime.Now;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select *");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
            psbQuery.AppendLine("where PrepEmpre.Empre = (select CenCos.Empre");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("   where CenCos.iCodCatalogo = (select Emple.CenCos");
            psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("       where Emple.iCodCatalogo = " + pFields.GetByConfigName("Emple").DataValue);
            psbQuery.AppendLine("       and Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("       and Emple.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("       and Emple.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            psbQuery.AppendLine("    and CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("    and CenCos.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("    and CenCos.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            psbQuery.AppendLine("and PrepEmpre.dtIniVigencia <> PrepEmpre.dtFinVigencia");
            psbQuery.AppendLine("and PrepEmpre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and PrepEmpre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(psbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                pRowConfigEmpre = lDataTable.Rows[0];

                //Calcular el inicio de periodo de presupuesto
                if (pRowConfigEmpre["PeriodoPrCod"].ToString() == "Diario")
                {
                    pdtIniPeriodoActual = new DateTime(ldtAhora.Year, ldtAhora.Month, ldtAhora.Day);
                    pdtIniPeriodoSiguiente = pdtIniPeriodoActual.AddDays(1);
                }
                else if (pRowConfigEmpre["PeriodoPrCod"].ToString() == "Semanal")
                {
                    double lDias;
                    if ((int)ldtAhora.DayOfWeek + 1 < (int)pRowConfigEmpre["DiaInicioPeriodo"])
                    {
                        lDias = (7 - (int)pRowConfigEmpre["DiaInicioPeriodo"]) + (int)ldtAhora.DayOfWeek + 1;
                    }
                    else
                    {
                        lDias = (int)ldtAhora.DayOfWeek + 1 - (int)pRowConfigEmpre["DiaInicioPeriodo"];
                    }

                    pdtIniPeriodoActual = ldtAhora.AddDays(-lDias);
                    pdtIniPeriodoSiguiente = ldtAhora.AddDays(7);

                }
                else if (pRowConfigEmpre["PeriodoPrCod"].ToString() == "Mensual")
                {
                    DateTime ldtMesActual = new DateTime(ldtAhora.Year, ldtAhora.Month, 01); //fecha de inicio de periodo para el mes actual
                    DateTime ldtMesAnterior = ldtMesActual.AddMonths(-1);   //fecha de inicio de periodo para el mes anterior
                    ldtMesActual = ldtMesActual.AddDays((int)pRowConfigEmpre["DiaInicioPeriodo"] - 1);
                    if (ldtMesActual.Month > ldtAhora.Month)
                    {
                        ldtMesActual = ldtMesActual.AddDays(-ldtMesActual.Day);
                    }

                    ldtMesAnterior = ldtMesAnterior.AddDays((int)pRowConfigEmpre["DiaInicioPeriodo"] - 1);
                    if (ldtMesAnterior.Month == ldtAhora.Month)
                    {
                        ldtMesAnterior = ldtMesAnterior.AddDays(-ldtMesAnterior.Day);
                    }

                    //Si la fecha del ahora es menor que el inicio de periodo para el mes actual
                    //entonces nos encontramos en el periodo que inicio en el mes anterior
                    if (ldtAhora < ldtMesActual)
                    {
                        pdtIniPeriodoActual = ldtMesAnterior;
                    }
                    else
                    {
                        pdtIniPeriodoActual = ldtMesActual;
                    }
                    pdtIniPeriodoSiguiente = pdtIniPeriodoActual.AddMonths(1);
                }
            }
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
            //el usuario nunca edita los catalogos por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarDatos()
        {
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            //vuelvo a llamar este metodo para asegurarme que los controles traigan la ultima configuracion de la empresa
            CalcularDatosEmpresa();

            double ldValor;
            if (pFields.GetByConfigName("PresupFijo").DSOControlDB.HasValue)
            {
                double.TryParse(pFields.GetByConfigName("PresupFijo").DataValue.ToString(), out ldValor);
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

            if (iCodCatalogo == "null")
            {
                //solo debe existir un iCodCatalogo de presupuesto fijo por empleado por lo que configurando que el maestro guarde historia
                //y leyendo si existe un registro en historicos a la hora de agregar para reutilizarlo me aseguro de ello
                psbQuery.Length = 0;
                psbQuery.AppendLine("select top 1 iCodRegistro, iCodCatalogo");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where Emple = " + pFields.GetByConfigName("Emple").DataValue);
                psbQuery.AppendLine("order by iCodRegistro desc");

                lDataTable = DSODataAccess.Execute(psbQuery.ToString());
                if (lDataTable.Rows.Count > 0)
                {
                    iCodRegistro = lDataTable.Rows[0]["iCodRegistro"].ToString();
                    iCodCatalogo = lDataTable.Rows[0]["iCodCatalogo"].ToString();
                }
            }

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
                //si ya existen notificaciones para el periodo actual entonces el cambio en el presupuesto 
                //aplica hasta el inicio del siguiente periodo
                if (State == HistoricState.Baja)
                {
                    pdtFinVigencia.DataValue = pdtIniPeriodoSiguiente;
                }
                else
                {
                    pdtIniVigencia.DataValue = pdtIniPeriodoSiguiente;
                    pFields.GetByConfigName("FechaInicioPrep").DataValue = pdtIniPeriodoSiguiente;
                }
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValPrepFijoPeriodoSig")) + "</span>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsMsg, lsTitulo);
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

        public override void PostAgregarSubHistoricField()
        {
            CalcularDatosEmpresa();
        }

        public override void PostEditarSubHistoricField()
        {
            CalcularDatosEmpresa();
        }
    }
}
