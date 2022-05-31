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
    public class CnfgPrepCenCosFijo : CnfgPrepEmpleFijo
    {
        public CnfgPrepCenCosFijo()
        {
            Init += new EventHandler(CnfgPrepCenCosFijo_Init);
        }

        void CnfgPrepCenCosFijo_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgPrepCenCos";
        }

        protected override void ObtenerConfiguracionEmpresa()
        {
            DateTime ldtAhora = DateTime.Now;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select *");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
            psbQuery.AppendLine("where PrepEmpre.Empre = (select CenCos.Empre");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("   where CenCos.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue);
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
            pvchDescripcion.DataValue = pFields.GetByConfigName("CenCos").ToString();

            DataTable lDataTable;
            psbQuery.Length = 0;

            if (iCodCatalogo == "null")
            {
                //solo debe existir un iCodCatalogo de presupuesto fijo por CenCos por lo que configurando que el maestro guarde historia
                //y leyendo si existe un registro en historicos a la hora de agregar para reutilizarlo me aseguro de ello
                psbQuery.Length = 0;
                psbQuery.AppendLine("select top 1 iCodRegistro, iCodCatalogo");
                psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where CenCos = " + pFields.GetByConfigName("CenCos").DataValue);
                psbQuery.AppendLine("order by iCodRegistro desc");

                lDataTable = DSODataAccess.Execute(psbQuery.ToString());
                if (lDataTable.Rows.Count > 0)
                {
                    iCodRegistro = lDataTable.Rows[0]["iCodRegistro"].ToString();
                    iCodCatalogo = lDataTable.Rows[0]["iCodCatalogo"].ToString();
                }
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("declare @iCodEntCenCos int");
            psbQuery.AppendLine("declare @dtIniVigencia datetime");
            psbQuery.AppendLine("declare @dtFinVigencia datetime");

            psbQuery.AppendLine("select @dtIniVigencia = " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   ,@dtFinVigencia = " + pdtFinVigencia.DataValue);

            psbQuery.AppendLine("select @iCodEntCenCos = iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('','Entidades','" + Globals.GetCurrentLanguage() + "')] Ent");
            psbQuery.AppendLine("where Ent.dtIniVigencia <> Ent.dtFinVigencia");
            psbQuery.AppendLine("and Ent.dtIniVigencia <= @dtIniVigencia");
            psbQuery.AppendLine("and Ent.dtFinVigencia > @dtIniVigencia");
            psbQuery.AppendLine("and Ent.vchCodigo = 'CenCos'");

            psbQuery.AppendLine("select top 1 Bt.iCodRegistro");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisDetallados('Detall','Bitacora Notificacion Consumos','" + Globals.GetCurrentLanguage() + "')] Bt");
            psbQuery.AppendLine("where Bt.FechaReset is null");
            psbQuery.AppendLine("and Bt.Emple in(select Emple.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("   where Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("   and Emple.dtIniVigencia <= @dtIniVigencia");
            psbQuery.AppendLine("   and Emple.dtFinVigencia > @dtIniVigencia");

            if ((int.Parse(pFields.GetByConfigName("BanderasPrepCenCos").DataValue.ToString()) & 1) == 1) //si incluye jerarquia
            {
                psbQuery.AppendLine("   and Emple.CenCos in (select CenCos.iCodCatalogo");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".GetJerarquiaEntidadVig(@iCodEntCenCos," + pFields.GetByConfigName("CenCos").DataValue + ",@dtIniVigencia,@dtFinVigencia) CenCos)");
                psbQuery.AppendLine("   and Emple.CenCos in (select CenCos.iCodCatalogo");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                psbQuery.AppendLine("       where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                psbQuery.AppendLine("       and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.Empre = (select CenCosEmpre.Empre");
                psbQuery.AppendLine("           from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCosEmpre");
                psbQuery.AppendLine("           where CenCosEmpre.dtIniVigencia <> CenCosEmpre.dtFinVigencia");
                psbQuery.AppendLine("           and CenCosEmpre.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("           and CenCosEmpre.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("           and CenCosEmpre.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue + ")))");
            }
            else
            {
                psbQuery.AppendLine("   and Emple.CenCos = " + pFields.GetByConfigName("CenCos").DataValue);
            }
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
                //si era un registro que se habia generado por jerarquia de CenCos y se edita
                //entonces le quito la liga de haberse generado por jerarquia
                pFields.GetByConfigName("PrepCenCos").DataValue = DBNull.Value;
            }

            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();

            return true;
        }

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
            try
            {
                GrabarPrepJerarquiaCenCos();
                GrabarPrepEmpleados();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected virtual void GrabarPrepJerarquiaCenCos()
        {
            if ((int.Parse(pFields.GetByConfigName("BanderasPrepCenCos").DataValue.ToString()) & 1) != 1)
            {
                return;
            }

            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();

            //Primero actualizo los que ya se habian agregado mediante jerarquia
            psbQuery.Length = 0;
            psbQuery.AppendLine("declare @iCodEntCenCos int");
            psbQuery.AppendLine("declare @dtIniVigencia datetime");
            psbQuery.AppendLine("declare @dtFinVigencia datetime");

            psbQuery.AppendLine("select @dtIniVigencia = " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   ,@dtFinVigencia = " + pdtFinVigencia.DataValue);

            psbQuery.AppendLine("select @iCodEntCenCos = iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('','Entidades','" + Globals.GetCurrentLanguage() + "')] Ent");
            psbQuery.AppendLine("where Ent.dtIniVigencia <> Ent.dtFinVigencia");
            psbQuery.AppendLine("and Ent.dtIniVigencia <= @dtIniVigencia");
            psbQuery.AppendLine("and Ent.dtFinVigencia > @dtIniVigencia");
            psbQuery.AppendLine("and Ent.vchCodigo = 'CenCos'");

            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] PrepCenCos");
            psbQuery.AppendLine("where PrepCenCos.dtIniVigencia <> PrepCenCos.dtFinVigencia");
            psbQuery.AppendLine("and PrepCenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("and PrepCenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("and PrepCenCos.iCodCatalogo <> " + iCodCatalogo);
            psbQuery.AppendLine("and PrepCenCos.PrepCenCos = " + iCodCatalogo);
            psbQuery.AppendLine("and PrepCenCos.CenCos in (select CenCos.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".GetJerarquiaEntidadVig(@iCodEntCenCos," + pFields.GetByConfigName("CenCos").DataValue + ",@dtIniVigencia,@dtFinVigencia) CenCos)");
            psbQuery.AppendLine("and PrepCenCos.CenCos in (select CenCos.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("   where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("   and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCos.Empre = (select CenCosEmpre.Empre");
            psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCosEmpre");
            psbQuery.AppendLine("       where CenCosEmpre.dtIniVigencia <> CenCosEmpre.dtFinVigencia");
            psbQuery.AppendLine("       and CenCosEmpre.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("       and CenCosEmpre.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("       and CenCosEmpre.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue + "))");

            DataTable ltblPrepCenCos = DSODataAccess.Execute(psbQuery.ToString());
            int liCodRegistro;
            Hashtable lhtValues = (Hashtable)phtValues.Clone();
            lhtValues[pFields.GetByConfigName("PrepCenCos").Column] = iCodCatalogo;

            foreach (DataRow lDataRow in ltblPrepCenCos.Rows)
            {
                liCodRegistro = (int)lDataRow["iCodRegistro"];
                lhtValues[pFields.GetByConfigName("CenCos").Column] = lDataRow["CenCos"];
                lhtValues["vchDescripcion"] = "'" + lDataRow["vchDescripcion"].ToString().Replace("'", "''") + "'";
                if (!lCargasCOM.ActualizaRegistro("Historicos", vchCodEntidad, vchDesMaestro, lhtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked))
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
            }

            //Inserto registros de presupuestos a los centros de costos que todavia no estan configurados y que pertenecen a la jerarquia
            psbQuery.Length = 0;
            psbQuery.AppendLine("declare @iCodEntCenCos int");
            psbQuery.AppendLine("declare @dtIniVigencia datetime");
            psbQuery.AppendLine("declare @dtFinVigencia datetime");

            psbQuery.AppendLine("select @dtIniVigencia = " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   ,@dtFinVigencia = " + pdtFinVigencia.DataValue);

            psbQuery.AppendLine("select @iCodEntCenCos = iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('','Entidades','" + Globals.GetCurrentLanguage() + "')] Ent");
            psbQuery.AppendLine("where Ent.dtIniVigencia <> Ent.dtFinVigencia");
            psbQuery.AppendLine("and Ent.dtIniVigencia <= @dtIniVigencia");
            psbQuery.AppendLine("and Ent.dtFinVigencia > @dtIniVigencia");
            psbQuery.AppendLine("and Ent.vchCodigo = 'CenCos'");

            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("and CenCos.iCodCatalogo <> " + pFields.GetByConfigName("CenCos").DataValue);
            psbQuery.AppendLine("and CenCos.iCodCatalogo in (select CenCos.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".GetJerarquiaEntidadVig(@iCodEntCenCos," + pFields.GetByConfigName("CenCos").DataValue + ",@dtIniVigencia,@dtFinVigencia) CenCos)");
            psbQuery.AppendLine("and CenCos.Empre = (select CenCosEmpre.Empre");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCosEmpre");
            psbQuery.AppendLine("   where CenCosEmpre.dtIniVigencia <> CenCosEmpre.dtFinVigencia");
            psbQuery.AppendLine("   and CenCosEmpre.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCosEmpre.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and CenCosEmpre.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue + ")");
            psbQuery.AppendLine("and not exists(");
            psbQuery.AppendLine("   select PrepCenCos.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] PrepCenCos");
            psbQuery.AppendLine("   where PrepCenCos.dtIniVigencia <> PrepCenCos.dtFinVigencia");
            psbQuery.AppendLine("   and PrepCenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepCenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepCenCos.CenCos = CenCos.iCodCatalogo)");

            ltblPrepCenCos = DSODataAccess.Execute(psbQuery.ToString());

            foreach (DataRow lDataRow in ltblPrepCenCos.Rows)
            {
                lhtValues["vchDescripcion"] = "'" + lDataRow["vchDescripcion"].ToString().Replace("'", "''") + "'";
                lhtValues[pFields.GetByConfigName("CenCos").Column] = lDataRow["iCodCatalogo"];
                liCodRegistro = lCargasCOM.InsertaRegistro(lhtValues, "Historicos", vchCodEntidad, vchDesMaestro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                if (liCodRegistro < 0)
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
            }
        }

        protected virtual void GrabarPrepEmpleados()
        {
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();

            //Primero actualizo los que ya se habian agregado
            psbQuery.Length = 0;
            psbQuery.AppendLine("declare @iCodEntCenCos int");
            psbQuery.AppendLine("declare @dtIniVigencia datetime");
            psbQuery.AppendLine("declare @dtFinVigencia datetime");

            psbQuery.AppendLine("select @dtIniVigencia = " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   ,@dtFinVigencia = " + pdtFinVigencia.DataValue);

            psbQuery.AppendLine("select @iCodEntCenCos = iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('','Entidades','" + Globals.GetCurrentLanguage() + "')] Ent");
            psbQuery.AppendLine("where Ent.dtIniVigencia <> Ent.dtFinVigencia");
            psbQuery.AppendLine("and Ent.dtIniVigencia <= @dtIniVigencia");
            psbQuery.AppendLine("and Ent.dtFinVigencia > @dtIniVigencia");
            psbQuery.AppendLine("and Ent.vchCodigo = 'CenCos'");

            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] PrepEmple");
            psbQuery.AppendLine("where PrepEmple.dtIniVigencia <> PrepEmple.dtFinVigencia");
            psbQuery.AppendLine("and PrepEmple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("and PrepEmple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("and PrepEmple.PrepCenCos = " + iCodCatalogo);
            psbQuery.AppendLine("and PrepEmple.Emple in(select Emple.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("   where Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("   and Emple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and Emple.dtFinVigencia > " + pdtIniVigencia.DataValue);

            if ((int.Parse(pFields.GetByConfigName("BanderasPrepCenCos").DataValue.ToString()) & 1) == 1)
            {
                psbQuery.AppendLine("   and Emple.CenCos in (select CenCos.iCodCatalogo");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".GetJerarquiaEntidadVig(@iCodEntCenCos," + pFields.GetByConfigName("CenCos").DataValue + ",@dtIniVigencia,@dtFinVigencia) CenCos))");
            }
            else
            {
                psbQuery.AppendLine("   and Emple.CenCos = " + pFields.GetByConfigName("CenCos").DataValue + ")");
            }

            DataTable ltblPrepEmple = DSODataAccess.Execute(psbQuery.ToString());
            int liCodRegistro;

            int liCodEntPrepEmple = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where dtIniVigencia <> dtFinVigencia and iCodCatalogo is null and vchCodigo = 'PrepEmple'");
            int liCodMaePrepEmple = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where dtIniVigencia <> dtFinVigencia and iCodEntidad = " + liCodEntPrepEmple + " and vchDescripcion = 'Presupuesto Fijo'");
            HistoricFieldCollection lPrepEmpleFields = new HistoricFieldCollection(liCodEntPrepEmple, liCodMaePrepEmple);

            Hashtable lhtValues = new Hashtable();
            lhtValues["iCodMaestro"] = liCodMaePrepEmple;
            lhtValues["vchCodigo"] = pvchCodigo.DataValue;
            lhtValues["dtIniVigencia"] = pdtIniVigencia.Date;
            lhtValues["dtFinVigencia"] = pdtFinVigencia.Date;
            lhtValues["iCodUsuario"] = Session["iCodUsuario"];
            lhtValues[lPrepEmpleFields.GetByConfigName("PrepCenCos").Column] = iCodCatalogo;
            lhtValues[lPrepEmpleFields.GetByConfigName("TipoPr").Column] = phtValues[pFields.GetByConfigName("TipoPr").Column];
            lhtValues[lPrepEmpleFields.GetByConfigName("PeriodoPr").Column] = phtValues[pFields.GetByConfigName("PeriodoPr").Column];
            lhtValues[lPrepEmpleFields.GetByConfigName("PresupFijo").Column] = phtValues[pFields.GetByConfigName("PresupFijo").Column];
            lhtValues[lPrepEmpleFields.GetByConfigName("FechaInicioPrep").Column] = phtValues[pFields.GetByConfigName("FechaInicioPrep").Column];

            foreach (DataRow lDataRow in ltblPrepEmple.Rows)
            {
                liCodRegistro = (int)lDataRow["iCodRegistro"];
                lhtValues[lPrepEmpleFields.GetByConfigName("Emple").Column] = lDataRow["Emple"];
                lhtValues["vchDescripcion"] = "'" + lDataRow["vchDescripcion"].ToString().Replace("'", "''") + "'";
                if (!lCargasCOM.ActualizaRegistro("Historicos", "PrepEmple", "Presupuesto Fijo", lhtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked))
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
            }

            //Inserto registros de presupuestos a los emmpleados que todavia no estan configurados y que pertenecen a la jerarquia
            psbQuery.Length = 0;
            psbQuery.AppendLine("declare @iCodEntCenCos int");
            psbQuery.AppendLine("declare @dtIniVigencia datetime");
            psbQuery.AppendLine("declare @dtFinVigencia datetime");

            psbQuery.AppendLine("select @dtIniVigencia = " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   ,@dtFinVigencia = " + pdtFinVigencia.DataValue);

            psbQuery.AppendLine("select @iCodEntCenCos = iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('','Entidades','" + Globals.GetCurrentLanguage() + "')] Ent");
            psbQuery.AppendLine("where Ent.dtIniVigencia <> Ent.dtFinVigencia");
            psbQuery.AppendLine("and Ent.dtIniVigencia <= @dtIniVigencia");
            psbQuery.AppendLine("and Ent.dtFinVigencia > @dtIniVigencia");
            psbQuery.AppendLine("and Ent.vchCodigo = 'CenCos'");

            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("where Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("and Emple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("and Emple.dtFinVigencia > " + pdtIniVigencia.DataValue);

            if ((int.Parse(pFields.GetByConfigName("BanderasPrepCenCos").DataValue.ToString()) & 1) == 1)
            {
                psbQuery.AppendLine("and Emple.CenCos in (select CenCos.iCodCatalogo");
                psbQuery.AppendLine("   from " + DSODataContext.Schema + ".GetJerarquiaEntidadVig(@iCodEntCenCos," + pFields.GetByConfigName("CenCos").DataValue + ",@dtIniVigencia,@dtFinVigencia) CenCos)");
                psbQuery.AppendLine("and Emple.CenCos in (select CenCos.iCodCatalogo");
                psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                psbQuery.AppendLine("   where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                psbQuery.AppendLine("   and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("   and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("   and CenCos.Empre = (select CenCosEmpre.Empre");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCosEmpre");
                psbQuery.AppendLine("       where CenCosEmpre.dtIniVigencia <> CenCosEmpre.dtFinVigencia");
                psbQuery.AppendLine("       and CenCosEmpre.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCosEmpre.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCosEmpre.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue + "))");
            }
            else
            {
                psbQuery.AppendLine("and Emple.CenCos = " + pFields.GetByConfigName("CenCos").DataValue);
            }

            psbQuery.AppendLine("and not exists(");
            psbQuery.AppendLine("   select PrepEmple.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] PrepEmple");
            psbQuery.AppendLine("   where PrepEmple.dtIniVigencia <> PrepEmple.dtFinVigencia");
            psbQuery.AppendLine("   and PrepEmple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.Emple = Emple.iCodCatalogo)");

            ltblPrepEmple = DSODataAccess.Execute(psbQuery.ToString());

            foreach (DataRow lDataRow in ltblPrepEmple.Rows)
            {
                lhtValues["vchDescripcion"] = "'" + lDataRow["vchDescripcion"].ToString().Replace("'", "''") + "'";
                lhtValues[lPrepEmpleFields.GetByConfigName("Emple").Column] = lDataRow["iCodCatalogo"];
                liCodRegistro = lCargasCOM.InsertaRegistro(lhtValues, "Historicos", "PrepEmple", "Presupuesto Fijo", false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                if (liCodRegistro < 0)
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
            }
        }

        public override void PostAgregarSubHistoricField()
        {
            string lsTitulo = DSOControl.JScriptEncode(pHistorico.AlertTitle);
            string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgEditarPrepCenCos")) + "</span>";
            DSOControl.jAlert(Page, pjsObj + ".MsgEditarPrepCenCos", lsMsg, lsTitulo);

            base.PostAgregarSubHistoricField();
        }

        public override void PostEditarSubHistoricField()
        {
            string lsTitulo = DSOControl.JScriptEncode(pHistorico.AlertTitle);
            string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgEditarPrepCenCos")) + "</span>";
            DSOControl.jAlert(Page, pjsObj + ".MsgEditarPrepCenCos", lsMsg, lsTitulo);

            base.PostEditarSubHistoricField();
        }
    }
}
