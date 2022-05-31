/*
Nombre:		    PGS
Fecha:		    20111028
Descripción:	Clase que sirve para edición/alta/baja tarifas.
Modificación:	
*/

using System;
using System.Collections;
using System.Collections.Generic;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface
{
    public class CnfgTarifaCollectionEdit : HistoricEdit
    {
        private StringBuilder psQuery = new StringBuilder();
        private StringBuilder psbErrores = new StringBuilder();
        private DataTable pdtAuxiliar;
        private string psError = "";

        public CnfgTarifaCollectionEdit()
        {
            Init += new EventHandler(CnfgTarifaCollectionEdit_Init);
        }

        void CnfgTarifaCollectionEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgTarifaCollectionEdit";
            CollectionClass = "KeytiaWeb.UserInterface.CnfgTarifaFieldCollection";
        }

        protected override void InitFields()
        {
            pExpAtributos.ID = "AtribWrapper";
            pExpAtributos.StartOpen = true;
            pExpAtributos.CreateControls();
            pExpAtributos.Panel.Controls.Clear();
            pExpAtributos.Panel.Controls.Add(pTablaAtributos);
            pExpAtributos.OnOpen = "function(){" + pjsObj + ".fnInitGrids();}";

            pTablaAtributos.Controls.Clear();
            pTablaAtributos.ID = "Atributos";
            pTablaAtributos.Width = Unit.Percentage(100);

            pPanelSubHistoricos.ID = "PanelSubHistoricos";
            pPanelSubHistoricos.CssClass = "PanelSubHistoricos";
            pPanelSubHistoricos.Controls.Clear();

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + " = new Historico('#" + this.ClientID + "','" + pjsObj + "');");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "New", lsb.ToString(), true, false);

            if (!String.IsNullOrEmpty(iCodEntidad) && !String.IsNullOrEmpty(iCodMaestro))
            {
                pFields = (HistoricFieldCollection)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricFieldCollection)).CodeBase, CollectionClass).Unwrap();
                ((CnfgTarifaFieldCollection)pFields).InitCollectionTarifa(this, int.Parse(iCodEntidad), int.Parse(iCodMaestro), pTablaAtributos, this.ValidarPermiso, "KeytiaWeb.UserInterface.CnfgTarifaCollectionEdit", "KeytiaWeb.UserInterface.CnfgSubHistTarifaField");
                pFields.InitFields();
                IniciaVigencia(false);
            }
        }

        protected override bool ValidarDatos()
        {
            bool lbRet = true;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloTarifas"));
            psError = "";
            psbErrores.Length = 0;

            try
            {
                if (pFields != null && pFields.ContainsConfigName("Horario") && pFields.GetByConfigName("Horario").DSOControlDB.HasValue &&
                     pFields.ContainsConfigName("DiasLlam") && pFields.GetByConfigName("DiasLlam").DSOControlDB.HasValue)
                {
                    psbErrores.Append(ValidarTarifaUnicaByDiaHorario());
                }
                else if (pFields != null && pFields.ContainsConfigName("Horario") && pFields.GetByConfigName("Horario").DSOControlDB.HasValue)
                {
                    psbErrores.Append(ValidarTarifaUnicaByHorario());
                }
                else if (pFields != null && pFields.ContainsConfigName("DiasLlam") && pFields.GetByConfigName("DiasLlam").DSOControlDB.HasValue)
                {
                    psbErrores.Append(ValidarTarifaUnicaByDias());
                }

                if (vchDesMaestro.Contains("Rango"))
                {
                    if (pFields != null && pFields.ContainsConfigName("ConsumoIni") && pFields.GetByConfigName("ConsumoIni").DSOControlDB.HasValue &&
                          pFields.ContainsConfigName("ConsumoFin") && pFields.GetByConfigName("ConsumoFin").DSOControlDB.HasValue)
                    {
                        psbErrores.Append(ValidarTarifaUnicaByRangos());
                    }

                    if (pFields != null && pFields.ContainsConfigName("UniCon") && pFields.GetByConfigName("UniCon").DSOControlDB.HasValue)
                    {
                        DataTable ldt = pKDB.GetHisRegByEnt("UniCon", "", "iCodCatalogo = " + pFields.GetByConfigName("UniCon").DataValue + " and vchCodigo in ('Minutos','Segundos')");
                        if (ldt == null || ldt.Rows.Count == 0)
                        {
                            // El valor de Unidad Consumo en Tarifas por Rango deben ser unidades de tiempo
                            psError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "UniConIncorrecto"));
                            psbErrores.Append("<li>" + psError + "</li>");
                        }
                    }
                }

                if (psbErrores.Length > 0)
                {
                    lbRet = false;
                    psError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", psError, lsTitulo);
                    psbErrores.Length = 0;
                }
                return lbRet;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected string ValidarFormatoHorario(string lsHoraIni, string lsHoraIni2, string lsHoraFin, string lsHoraFin2)
        {
            if (KeytiaServiceBL.Util.IsDate("1900-01-01 " + lsHoraIni, new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss.000" }) == DateTime.MinValue)
            {
                return lsHoraIni;
            }
            else if (KeytiaServiceBL.Util.IsDate("1900-01-01 " + lsHoraIni2, new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss.000" }) == DateTime.MinValue)
            {
                return lsHoraIni2;
            }
            else if (KeytiaServiceBL.Util.IsDate("1900-01-01 " + lsHoraFin, new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss.000" }) == DateTime.MinValue)
            {
                return lsHoraFin;
            }
            else if (KeytiaServiceBL.Util.IsDate("1900-01-01 " + lsHoraFin2, new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss.000" }) == DateTime.MinValue)
            {
                return lsHoraFin2;
            }
            return null;
        }

        protected virtual string ValidarTarifaUnicaByDiaHorario()
        {            
            psError = "";
            StringBuilder lsbErrores = new StringBuilder();
            List<string> lstDias = new List<string>();
            string lsDias = "";  
            string lsHoraInicio = "00:00:00";
            string lsHoraInicio2 = "00:00:00";
            string lsHoraFin = "00:00:00";
            string lsHoraFin2 = "00:00:00";
            string lsCodEntTarifa = (string)Util.IsDBNull(DSODataAccess.Execute("Select iCodRegistro from Catalogos where  vchCodigo='Tarifa' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString(), "-1");

            pdtAuxiliar = pKDB.GetHisRegByEnt("Horario", "Horarios", "iCodCatalogo =" + pFields.GetByConfigName("Horario").DataValue);
            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                lsHoraInicio = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraInicio.}"], "00:00:00");
                lsHoraFin = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraFin}"], "00:00:00");
                lsHoraInicio2 = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraInicio2.}"], lsHoraInicio);
                lsHoraFin2 = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraFin2}"], lsHoraFin);

                string lsHorarioIncorrecto = ValidarFormatoHorario(lsHoraInicio, lsHoraInicio2, lsHoraFin, lsHoraFin2);
                if (lsHorarioIncorrecto != null)
                {
                    psError = "<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValEmplFormato", pFields.GetByConfigName("Horario").Descripcion + " [" + pdtAuxiliar.Rows[0]["vchDescripcion"].ToString() + ": " + lsHorarioIncorrecto + "]")) + "</li>";
                    return psError;
                }
            }
                      
            pdtAuxiliar = pKDB.GetHisRegByRel("Dias Semana - Dias Llamada", "DiasSem", "{DiasLlam} =" + pFields.GetByConfigName("DiasLlam").DataValue);
            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                for (int iDia = 0; iDia < pdtAuxiliar.Rows.Count; iDia++)
                {
                    lstDias.Add(pdtAuxiliar.Rows[iDia]["vchCodigo"].ToString());
                }
                lsDias = string.Join(",",lstDias.ToArray());
            }
            else
            {
                lsDias = "0,1,2,3,4,5,6";
            }

            psQuery.Length = 0;
            psQuery.Append("   select distinct T.vchDescripcion \r\n");
            psQuery.Append("    from " + DSODataContext.Schema + ".[VisRelaciones('Dias Semana - Dias Llamada','Español')] DS \r\n");
            psQuery.Append("   inner Join " + DSODataContext.Schema + ".[VisHistoricos('DiasLlam','Español')] D \r\n");
            psQuery.Append("   inner join " + DSODataContext.Schema + ".[VisHistoricos('Horario','Español')] H \r\n");
            psQuery.Append("   inner join " + DSODataContext.Schema + ".[VisHistoricos('Tarifa','Español')] T \r\n");
            psQuery.Append("   on T.Horario = H.iCodCatalogo and T.iCodMaestro Not in \r\n");
            psQuery.Append("         (Select iCodRegistro From [" + DSODataContext.Schema + "].[Maestros] \r\n");
            psQuery.Append("         Where vchDescripcion like '%Destino%' and iCodEntidad = " + lsCodEntTarifa + " and dtIniVigencia <> dtFinVigencia) \r\n");
            psQuery.Append("      and T.dtIniVigencia <> T.dtFinVigencia");
            psQuery.Append("      and ((T.dtIniVigencia <= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("            T.dtFinVigencia > '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("           (T.dtIniVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("            T.dtFinVigencia > '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("           (T.dtIniVigencia >= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("            T.dtFinVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "')) \r\n");
            psQuery.Append("      and H.[HoraInicio.] is not null and H.[HoraFin] is not null \r\n");
            psQuery.Append(" on T.DiasLlam = D.iCodCatalogo \r\n");
            psQuery.Append(" on D.iCodCatalogo = DS.DiasLlam \r\n");
            psQuery.Append(" Where DS.DiasSemCod in (" + lsDias + ") \r\n");
            psQuery.Append("   and T.Region = " + pFields.GetByConfigName("Region").DataValue + "\r\n");
            psQuery.Append("   and T.PlanServ = " + pFields.GetByConfigName("PlanServ").DataValue + "\r\n");
            psQuery.Append("   and (( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraInicio + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraInicio + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraFin + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraFin + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) >= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) <= '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) >= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) <= '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraInicio2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraInicio2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraFin2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraFin2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraFin2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraFin2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) >= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) <= '2011-01-01 " + lsHoraFin2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) >= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) <= '2011-01-01 " + lsHoraFin2 + "')) \r\n");
                        if (iCodRegistro != "null")
            {
                psQuery.Append("     and T.iCodRegistro not in ( " + iCodRegistro + ") \r\n");
            }

            pdtAuxiliar = DSODataAccess.Execute(psQuery.ToString());

            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                for (int i = 0; i < pdtAuxiliar.Rows.Count; i++)
                {
                    psError = "<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TraslapeDiaHorarioTarifa",pdtAuxiliar.Rows[i]["vchDescripcion"].ToString())) + "</li>";
                    lsbErrores.Append(psError);
                }
                psError = lsbErrores.ToString();
                lsbErrores.Length = 0;
                return psError;
            }

            return "";
        }

        protected virtual string ValidarTarifaUnicaByHorario()
        {
            psError = "";
            StringBuilder lsbErrores = new StringBuilder();
            string lsCodEntTarifa = (string)Util.IsDBNull(DSODataAccess.Execute("Select iCodRegistro from Catalogos where  vchCodigo='Tarifa' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString(), "-1");
            string lsHoraInicio = "00:00:00";
            string lsHoraInicio2 = "00:00:00";
            string lsHoraFin = "00:00:00";
            string lsHoraFin2 = "00:00:00";            

            pdtAuxiliar = pKDB.GetHisRegByEnt("Horario", "Horarios", "iCodCatalogo =" + pFields.GetByConfigName("Horario").DataValue);
            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                lsHoraInicio = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraInicio.}"], "00:00:00");
                lsHoraFin = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraFin}"], "00:00:00");
                lsHoraInicio2 = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraInicio2.}"], lsHoraInicio);
                lsHoraFin2 = (string)Util.IsDBNull(pdtAuxiliar.Rows[0]["{HoraFin2}"], lsHoraFin);

                string lsHorarioIncorrecto = ValidarFormatoHorario(lsHoraInicio, lsHoraInicio2, lsHoraFin, lsHoraFin2);
                if (lsHorarioIncorrecto != null)
                {
                    psError = "<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValEmplFormato", pFields.GetByConfigName("Horario").Descripcion + " [" + pdtAuxiliar.Rows[0]["vchDescripcion"].ToString() + ": " + lsHorarioIncorrecto + "]")) + "</li>";
                    return psError;
                }
            }

            psQuery.Length = 0;
            psQuery.Append("  select distinct T.vchDescripcion \r\n");
            psQuery.Append("    from " + DSODataContext.Schema + ".[VisHistoricos('Horario','Español')] H \r\n");
            psQuery.Append("   inner join " + DSODataContext.Schema + ".[VisHistoricos('Tarifa','Español')] T \r\n");
            psQuery.Append("      on T.Horario = H.iCodCatalogo and T.iCodMaestro Not in \r\n");
            psQuery.Append("         (Select iCodRegistro From [" + DSODataContext.Schema + "].[Maestros] \r\n");
            psQuery.Append("           Where vchDescripcion like '%Destino%' and iCodEntidad = " + lsCodEntTarifa + " and dtIniVigencia <> dtFinVigencia) \r\n");
            psQuery.Append("         and T.dtIniVigencia <> T.dtFinVigencia \r\n");
            psQuery.Append("         and ((T.dtIniVigencia <= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("               T.dtFinVigencia > '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("              (T.dtIniVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("               T.dtFinVigencia > '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("              (T.dtIniVigencia >= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("               T.dtFinVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "')) \r\n");
            psQuery.Append("         and H.[HoraInicio.] is not null and H.[HoraFin] is not null \r\n");
            psQuery.Append("   Where T.Region = " + pFields.GetByConfigName("Region").DataValue + "\r\n");
            psQuery.Append("     and T.PlanServ = " + pFields.GetByConfigName("PlanServ").DataValue + "\r\n");
            psQuery.Append("     and (( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraInicio + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraInicio + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraFin + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraFin + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) >= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) <= '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) >= '2011-01-01 " + lsHoraInicio + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) <= '2011-01-01 " + lsHoraFin + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraInicio2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraInicio2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) <= '2011-01-01 " + lsHoraFin2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) > '2011-01-01 " + lsHoraFin2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) <= '2011-01-01 " + lsHoraFin2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) > '2011-01-01 " + lsHoraFin2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + H.[HoraInicio.],102) >= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + H.[HoraFin],102) <= '2011-01-01 " + lsHoraFin2 + "') or \r\n");
            psQuery.Append("        ( CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraInicio2.],H.[HoraInicio.]),102) >= '2011-01-01 " + lsHoraInicio2 + "' \r\n");
            psQuery.Append("          and  CONVERT(datetime,'2011-01-01 ' + IsNull(H.[HoraFin2],H.[HoraFin]),102) <= '2011-01-01 " + lsHoraFin2 + "')) \r\n");
            if (iCodRegistro != "null")
            {
                psQuery.Append("     and T.iCodRegistro not in ( " + iCodRegistro + ") \r\n");
            }

            pdtAuxiliar = DSODataAccess.Execute(psQuery.ToString());

            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                for (int i = 0; i < pdtAuxiliar.Rows.Count; i++)
                {
                    psError = "<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TraslapeHorarioTarifa", pdtAuxiliar.Rows[i]["vchDescripcion"].ToString())) + "</li>";
                    lsbErrores.Append(psError);
                }
                psError = lsbErrores.ToString();
                lsbErrores.Length = 0;
                return psError;
            }

            return "";
        }

        protected virtual string ValidarTarifaUnicaByDias()
        {
            psError = "";
            StringBuilder lsbErrores = new StringBuilder();
            List<string> lstDias = new List<string>();
            string lsDias = "";
            string lsCodEntTarifa = (string)Util.IsDBNull(DSODataAccess.Execute("Select iCodRegistro from Catalogos where  vchCodigo='Tarifa' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString(), "-1");
            
            pdtAuxiliar = pKDB.GetHisRegByRel("Dias Semana - Dias Llamada", "DiasSem", "{DiasLlam} =" + pFields.GetByConfigName("DiasLlam").DataValue);
            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                for (int iDia = 0; iDia < pdtAuxiliar.Rows.Count; iDia++)
                {
                    lstDias.Add(pdtAuxiliar.Rows[iDia]["vchCodigo"].ToString());
                }
                lsDias = string.Join(",", lstDias.ToArray());
            }
            else
            {
                lsDias = "0,1,2,3,4,5,6";
            }

            psQuery.Length = 0;
            psQuery.Append("   select distinct T.vchDescripcion \r\n");
            psQuery.Append("    from " + DSODataContext.Schema + ".[VisRelaciones('Dias Semana - Dias Llamada','Español')] DS \r\n");
            psQuery.Append("   inner Join " + DSODataContext.Schema + ".[VisHistoricos('DiasLlam','Español')] D \r\n");
            psQuery.Append("   inner join " + DSODataContext.Schema + ".[VisHistoricos('Tarifa','Español')] T \r\n");
            psQuery.Append("   on T.DiasLlam = D.iCodCatalogo and T.iCodMaestro Not in \r\n");
            psQuery.Append("         (Select iCodRegistro From [" + DSODataContext.Schema + "].[Maestros] \r\n");
            psQuery.Append("         Where vchDescripcion like '%Destino%' and iCodEntidad = " + lsCodEntTarifa + " and dtIniVigencia <> dtFinVigencia) \r\n");
            psQuery.Append("      and T.dtIniVigencia <> T.dtFinVigencia \r\n");
            psQuery.Append("      and ((T.dtIniVigencia <= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("            T.dtFinVigencia > '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("           (T.dtIniVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("            T.dtFinVigencia > '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("           (T.dtIniVigencia >= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("            T.dtFinVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "')) \r\n");
            psQuery.Append("      and T.[DiasLlam] is not null \r\n");
            psQuery.Append(" on D.iCodCatalogo = DS.DiasLlam \r\n");
            psQuery.Append(" Where DS.DiasSemCod in (" + lsDias + ") \r\n");
            psQuery.Append("      and T.Region = " + pFields.GetByConfigName("Region").DataValue + "\r\n");
            psQuery.Append("      and T.PlanServ = " + pFields.GetByConfigName("PlanServ").DataValue + "\r\n");
            if (iCodRegistro != "null")
            {
                psQuery.Append("     and T.iCodRegistro not in ( " + iCodRegistro + ") \r\n");
            }


            pdtAuxiliar = DSODataAccess.Execute(psQuery.ToString());

            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                for (int i = 0; i < pdtAuxiliar.Rows.Count; i++)
                {
                    psError = "<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TraslapeDiasTarifa", pdtAuxiliar.Rows[i]["vchDescripcion"].ToString())) + "</li>";
                    lsbErrores.Append(psError);
                }
                psError = lsbErrores.ToString();
                lsbErrores.Length = 0;
                return psError;
            }

            return "";
        }

        protected virtual string ValidarTarifaUnicaByRangos()
        {
            psError = "";
            StringBuilder lsbErrores = new StringBuilder();
            string lsCodEntTarifa = (string)Util.IsDBNull(DSODataAccess.Execute("Select iCodRegistro from Catalogos where  vchCodigo='Tarifa' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString(), "-1");

            psQuery.Length = 0;
            psQuery.Append("Select distinct T.vchDescripcion \r\n");
            psQuery.Append(" From " + DSODataContext.Schema + ".[VisHistoricos('Tarifa','Español')] T \r\n");
            psQuery.Append(" Where T.iCodMaestro in (Select iCodRegistro From [" + DSODataContext.Schema + "].[Maestros] \r\n");
            psQuery.Append("                          Where vchDescripcion like '%Rango%' and iCodEntidad =" + lsCodEntTarifa + " and dtIniVigencia <> dtFinVigencia)\r\n");
            psQuery.Append("  and T.[ConsumoIni] is not null and T.[ConsumoFin] is not null \r\n");
            psQuery.Append("  and T.dtIniVigencia <> T.dtFinVigencia \r\n");
            psQuery.Append("  and ((T.dtIniVigencia <= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("        T.dtFinVigencia > '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("       (T.dtIniVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("        T.dtFinVigencia > '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "') or \r\n");
            psQuery.Append("       (T.dtIniVigencia >= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' and \r\n");
            psQuery.Append("        T.dtFinVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "')) \r\n");
            psQuery.Append("  and T.Region = " + pFields.GetByConfigName("Region").DataValue + "\r\n");
            psQuery.Append("  and T.PlanServ = " + pFields.GetByConfigName("PlanServ").DataValue + "\r\n");
            if (pFields.ContainsConfigName("UniCon") && pFields.GetByConfigName("UniCon").DSOControlDB.HasValue)
            {                                
                psQuery.Append("  and T.[UniCon] = " + pFields.GetByConfigName("UniCon").DataValue + "\r\n");
            }
            psQuery.Append("  and ((T.[ConsumoIni] <= " + pFields.GetByConfigName("ConsumoIni").DataValue + "\r\n");
            psQuery.Append("        and T.[ConsumoFin] > " + pFields.GetByConfigName("ConsumoIni").DataValue + ") or \r\n");            
            psQuery.Append("       (T.[ConsumoIni] <=" + pFields.GetByConfigName("ConsumoFin").DataValue + "\r\n");
            psQuery.Append("        and T.[ConsumoFin] > " + pFields.GetByConfigName("ConsumoFin").DataValue + ") or \r\n");            
            psQuery.Append("       (T.[ConsumoIni] >= " + pFields.GetByConfigName("ConsumoIni").DataValue + "\r\n");
            psQuery.Append("        and T.[ConsumoFin] <= " + pFields.GetByConfigName("ConsumoFin").DataValue + ")) \r\n");
            if (iCodRegistro != "null")
            {
                psQuery.Append("     and T.iCodRegistro not in ( " + iCodRegistro + ") \r\n");
            }

            pdtAuxiliar = DSODataAccess.Execute(psQuery.ToString());

            if (pdtAuxiliar != null && pdtAuxiliar.Rows.Count > 0)
            {
                for (int i = 0; i < pdtAuxiliar.Rows.Count; i++)
                {
                    psError = "<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TraslapeRangosTarifa", pdtAuxiliar.Rows[i]["vchDescripcion"].ToString())) + "</li>";
                    lsbErrores.Append(psError);
                }
                psError = lsbErrores.ToString();
                lsbErrores.Length = 0;
                return psError;
            }

            return "";
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            if (s == HistoricState.Edicion
                || s == HistoricState.Baja)
            {
                pPanelSubHistoricos.Visible = true;
                if (pHistorico.Fields != null)
                {
                    pHistorico.FillAjaxControls(true);
                    pFields.GetByConfigName("PlanServ").DataValue = pHistorico.Fields.GetByConfigName("PlanServ").DataValue;
                    pFields.GetByConfigName("Region").DataValue = pHistorico.Fields.GetByConfigName("Region").DataValue;
                    pFields.GetByConfigName("PlanServ").DisableField();
                    pFields.GetByConfigName("Region").DisableField();
                }
            }
        }

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
            ExistePeriodoSinTarifa();
        }

        private void ExistePeriodoSinTarifa()
        {
            DataTable ldt;
            DataRow[] ldr;
            string lsDia;
            string lsMensaje;
            StringBuilder lsbMensaje = new StringBuilder();   
            StringBuilder lsbHorario = new StringBuilder();            
            int liSegundos = 0;
            string lsHorasFaltantes;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloTarifas"));
            
            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("Select T.[PlanServ],T.[Region],D.[DiasSemCod],H.[vchCodigo],");
                psbQuery.AppendLine("       Horas1 = H.[HoraInicio.],");
                psbQuery.AppendLine("       Horas1Fin = H.[HoraFin],");
                psbQuery.AppendLine("       --Dif1 = IsNull(DATEDIFF(ss,H.[HoraInicio.],H.[HoraFin]),0),");
                psbQuery.AppendLine("       Horas2 = H.[HoraInicio2.],");
                psbQuery.AppendLine("       Horas2Fin = H.[HoraFin2],");
                psbQuery.AppendLine("       --Dif2 = IsNull(DATEDIFF(ss,H.[HoraInicio2.],H.[HoraFin2]),0),");
                psbQuery.AppendLine("       Suma = (DATEDIFF(ss,H.[HoraInicio.],Dateadd(ss,1,H.[HoraFin])) + ISNULL(DATEDIFF(ss,H.[HoraInicio2.],dateadd(ss,1,H.[HoraFin2])),0))");
                psbQuery.AppendLine("from  " + DSODataContext.Schema + ".[VisHistoricos('Horario','Español')] H right join");
                psbQuery.AppendLine("      " + DSODataContext.Schema + ".[VisRelaciones('Dias Semana - Dias Llamada','Español')] D inner join");
                psbQuery.AppendLine("      " + DSODataContext.Schema + ".[VisHistoricos('Tarifa','Español')] T");
                psbQuery.AppendLine("   on T.[DiasLlam] = D.[DiasLlam] and T.[dtIniVigencia] <> T.[dtFinVigencia] and D.[dtIniVigencia] <> D.[dtFinVigencia]");
                psbQuery.AppendLine("   on T.[Horario] = H.[iCodCatalogo] and T.[dtIniVigencia] <> T.[dtFinVigencia] and H.[dtIniVigencia] <> H.[dtFinVigencia]");
                psbQuery.AppendLine("Where T.[PlanServ] = " + pFields.GetByConfigName("PlanServ").DataValue);
                psbQuery.AppendLine("  and T.[Region] = " + pFields.GetByConfigName("Region").DataValue);
                psbQuery.AppendLine("  and T.[dtIniVigencia] >= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("  and T.[dtIniVigencia] < " + pdtFinVigencia.DataValue);
                psbQuery.AppendLine("Group by T.[PlanServ] , T.[Region], D.[DiasSemCod],H.[vchCodigo],H.[HoraInicio.],H.[HoraFin],H.[HoraInicio2.],H.[HoraFin2]");

                ldt = DSODataAccess.Execute(psbQuery.ToString());

                if (ldt == null || ldt.Rows.Count == 0)
                {
                    for (int liDia = 0; liDia < 7; liDia++)
                    {
                        //Revisar que para cada día haya una tarifa
                        lsDia = Globals.GetLangItem("DiasSem", "Dias Semana", liDia.ToString());
                        lsbHorario.AppendLine(DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoHorariosTarifa", new string[] { lsDia, 23.ToString(), 59.ToString(), 59.ToString() })));
                        lsbHorario.Append("Día: " + lsDia + ". Tiempo por cubrir: " + 23 + " Horas," + 59 + " Minutos," + 59 + " Segundos");
                    }
                }
                else
                {
                    for (int liDia = 0; liDia < 7; liDia++)
                    {
                        //Revisar que para cada día haya una tarifa
                        lsDia = Globals.GetLangItem("DiasSem", "Dias Semana", liDia.ToString());
                        ldr = ldt.Select("DiasSemCod='" + liDia + "'");
                        if (ldr == null || ldr.Length == 0)
                        {
                            lsbHorario.Append("<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoHorariosTarifa", new string[] { lsDia, 23.ToString(), 59.ToString(), 59.ToString() })) + "</li>");                            
                            continue;
                        }

                        foreach (DataRow ldrDia in ldr)
                        {
                            //Suma Horario 1 mas Horario 2
                            liSegundos = liSegundos + (int)ldrDia["Suma"];
                        }

                        if (liSegundos < 86400)
                        {     
                            //Si el numero de segundos no corresponde a 24 horas, existen horarios sin asignar Tarifa                          
                            liSegundos = 86400 - liSegundos;
                            int liHoras = ((liSegundos / 3600) % 24);
                            int liMinutos = ((liSegundos / 60) % 60);
                            liSegundos = (liSegundos % 60);
                            lsHorasFaltantes = KeytiaServiceBL.Reportes.ReporteEstandarUtil.TimeSegToString(liSegundos, Globals.GetCurrentLanguage());
                            lsbHorario.Append("<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoHorariosTarifa", new string[] { lsDia, liHoras.ToString(), liMinutos.ToString(), liSegundos.ToString() })) + "</li>");
                        }
                    }
                }

                if (lsbHorario.Length > 0)
                {
                    string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                    lsMensaje = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "HorarioSinTarifa", new string[] {pdtIniVigencia.Date.ToString(lsNetDateFormat), pdtFinVigencia.Date.ToString(lsNetDateFormat)}));
                    lsbMensaje.Append("<li>" + lsMensaje);
                    lsbMensaje.Append("<ul>" + lsbHorario + "</ul>");
                    lsbMensaje.Append("</li>");
                    lsMensaje = "<ul>" + lsbMensaje.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsMensaje, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }
    }


}