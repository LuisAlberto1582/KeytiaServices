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
    public class CnfgPrepCenCosTemporal : CnfgPrepCenCosFijo
    {
        protected DataTable ptblConsumoBase = null;
        protected DataTable ptblPrepEmple = null;

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
            pvchDescripcion.DataValue = pFields.GetByConfigName("CenCos").ToString();

            DataTable lDataTable;
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
            psbQuery.AppendLine("   and Emple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and Emple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            if ((int.Parse(pFields.GetByConfigName("BanderasPrepCenCos").DataValue.ToString()) & 1) == 1)
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
                psbQuery.AppendLine("       and Emple.CenCos = " + pFields.GetByConfigName("CenCos").DataValue + ")");
            }
            psbQuery.AppendLine("and Bt.PrepEmple in(select PrepEmple.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] PrepEmple");
            psbQuery.AppendLine("   where PrepEmple.dtIniVigencia <> PrepEmple.dtFinVigencia");
            psbQuery.AppendLine("   and PrepEmple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.PrepCenCos = " + iCodCatalogo);
            psbQuery.AppendLine("   and PrepEmple.Emple = Bt.Emple)");
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

        protected override void GrabarPrepJerarquiaCenCos()
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

            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] PrepCenCos");
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
            psbQuery.AppendLine("   select PrepCenCos.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] PrepCenCos");
            psbQuery.AppendLine("   where PrepCenCos.PrepCenCos = " + iCodCatalogo);
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
            psbQuery.AppendLine("where Detall.Emple in(select Emple.iCodCatalogo");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("   where Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("   and Emple.dtIniVigencia <= Detall.FechaInicio");
            psbQuery.AppendLine("   and Emple.dtFinVigencia > Detall.FechaInicio");
            if ((int.Parse(pFields.GetByConfigName("BanderasPrepCenCos").DataValue.ToString()) & 1) == 1)
            {
                psbQuery.AppendLine("   and Emple.CenCos in (select CenCos.iCodCatalogo");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".GetJerarquiaEntidadVig(@iCodEntCenCos," + pFields.GetByConfigName("CenCos").DataValue + ",'" + pdtIniPeriodoActual.ToString("yyyy-MM-dd HH:mm:ss") + "','" + pdtIniPeriodoSiguiente.ToString("yyyy-MM-dd HH:mm:ss") + "') CenCos))");
            }
            else
            {
                psbQuery.AppendLine("   and Emple.CenCos = " + pFields.GetByConfigName("CenCos").DataValue + ")");
            }
            psbQuery.AppendLine("and Detall.FechaInicio >= '" + pdtIniPeriodoActual.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and Detall.FechaInicio < '" + pdtIniPeriodoSiguiente.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("group by Emple");

            ptblConsumoBase = DSODataAccess.Execute(psbQuery.ToString());
        }

        protected override void GrabarPrepEmpleados()
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

            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] PrepEmple");
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
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".GetJerarquiaEntidadVig(@iCodEntCenCos," + pFields.GetByConfigName("CenCos").DataValue + ",@dtIniVigencia,@dtFinVigencia) CenCos)");
                psbQuery.AppendLine("   and Emple.CenCos in (select CenCos.iCodCatalogo");
                psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                psbQuery.AppendLine("       where CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                psbQuery.AppendLine("       and CenCos.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and CenCos.Empre = (select CenCosEmpre.Empre");
                psbQuery.AppendLine("           from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCosEmpre");
                psbQuery.AppendLine("           where CenCosEmpre.dtIniVigencia <> CenCos.dtFinVigencia");
                psbQuery.AppendLine("           and CenCosEmpre.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("           and CenCosEmpre.dtFinVigencia > " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("           and CenCosEmpre.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue + ")))");
            }
            else
            {
                psbQuery.AppendLine("   and Emple.CenCos = " + pFields.GetByConfigName("CenCos").DataValue + ")");
            }

            DataTable ltblPrepEmple = DSODataAccess.Execute(psbQuery.ToString());
            int liCodRegistro;

            int liCodEntPrepEmple = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where dtIniVigencia <> dtFinVigencia and iCodCatalogo is null and vchCodigo = 'PrepEmple'");
            int liCodMaePrepEmple = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where dtIniVigencia <> dtFinVigencia and iCodEntidad = " + liCodEntPrepEmple + " and vchDescripcion = 'Presupuesto Temporal'");
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
            lhtValues[lPrepEmpleFields.GetByConfigName("PresupProv").Column] = phtValues[pFields.GetByConfigName("PresupProv").Column];
            lhtValues[lPrepEmpleFields.GetByConfigName("FechaInicioPrep").Column] = phtValues[pFields.GetByConfigName("FechaInicioPrep").Column];

            foreach (DataRow lDataRow in ltblPrepEmple.Rows)
            {
                liCodRegistro = (int)lDataRow["iCodRegistro"];
                lhtValues[lPrepEmpleFields.GetByConfigName("Emple").Column] = lDataRow["Emple"];
                lhtValues["vchDescripcion"] = "'" + lDataRow["vchDescripcion"].ToString().Replace("'", "''") + "'";
                if (!lCargasCOM.ActualizaRegistro("Historicos", "PrepEmple", "Presupuesto Temporal", lhtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked))
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
            psbQuery.AppendLine("   select PrepEmple.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Temporal','" + Globals.GetCurrentLanguage() + "')] PrepEmple");
            psbQuery.AppendLine("   where PrepEmple.PrepCenCos = " + iCodCatalogo);
            psbQuery.AppendLine("   and PrepEmple.Emple = Emple.iCodCatalogo)");
            psbQuery.AppendLine("and exists("); //el empleado debe tener configurado un presupuesto fijo para poder configurar uno temporal
            psbQuery.AppendLine("   select PrepEmple.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] PrepEmple");
            psbQuery.AppendLine("   where PrepEmple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("   and PrepEmple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.Emple = Emple.iCodCatalogo)");
            psbQuery.AppendLine("and not exists("); //no debe existir un registro de presupuesto del empleado para el cual no se haya consumido el 100%
            psbQuery.AppendLine("   select PrepEmple.iCodCatalogo from " + DSODataContext.Schema + ".[VisHistoricos('PrepEmple','" + Globals.GetCurrentLanguage() + "')] PrepEmple");
            psbQuery.AppendLine("   where PrepEmple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("   and PrepEmple.dtIniVigencia <= " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.dtFinVigencia > " + pdtIniVigencia.DataValue);
            psbQuery.AppendLine("   and PrepEmple.Emple = Emple.iCodCatalogo");
            psbQuery.AppendLine("   and not exists(");
            psbQuery.AppendLine("       select Bt.PrepEmple from " + DSODataContext.Schema + ".[VisDetallados('Detall','Bitacora Consumo 100%','" + Globals.GetCurrentLanguage() + "')] Bt");
            psbQuery.AppendLine("       where Bt.PrepEmple = PrepEmple.iCodCatalogo");
            psbQuery.AppendLine("       and Bt.FechaInicioPrep = " + pFields.GetByConfigName("FechaInicioPrep").DataValue);
            psbQuery.AppendLine("       and Bt.FechaReset is null");
            psbQuery.AppendLine("       and Bt.Emple = Emple.iCodCatalogo))");

            ltblPrepEmple = DSODataAccess.Execute(psbQuery.ToString());

            if (ltblPrepEmple.Rows.Count > 0)
            {
                CalcularConsumoBase();
            }

            DataRow lRowConsumoBase;
            foreach (DataRow lDataRow in ltblPrepEmple.Rows)
            {
                if (ptblConsumoBase.Select("Emple = " + lDataRow["iCodCatalogo"].ToString()).Length > 0)
                {
                    lRowConsumoBase = ptblConsumoBase.Select("Emple = " + lDataRow["iCodCatalogo"].ToString())[0];
                    lhtValues["vchDescripcion"] = "'" + lDataRow["vchDescripcion"].ToString().Replace("'", "''") + "'";
                    lhtValues[lPrepEmpleFields.GetByConfigName("Emple").Column] = lDataRow["iCodCatalogo"];
                    lhtValues[lPrepEmpleFields.GetByConfigName("ValorConsumoBase").Column] = lRowConsumoBase["Consumo"];
                    liCodRegistro = lCargasCOM.InsertaRegistro(lhtValues, "Historicos", "PrepEmple", "Presupuesto Temporal", false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                    if (liCodRegistro < 0)
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                }
            }
        }

        public override void PostAgregarSubHistoricField()
        {
            string lsTitulo = DSOControl.JScriptEncode(this.Historico.AlertTitle);

            base.PostAgregarSubHistoricField();
            DateTime ldtAhora = DateTime.Now;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select *");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepCenCos','Presupuesto Fijo','" + Globals.GetCurrentLanguage() + "')] PrepFijo");
            psbQuery.AppendLine("where PrepFijo.CenCos = " + pFields.GetByConfigName("CenCos").DataValue);
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
        }
    }
}
