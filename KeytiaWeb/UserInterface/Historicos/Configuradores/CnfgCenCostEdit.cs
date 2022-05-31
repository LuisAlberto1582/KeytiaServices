using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgCenCostEdit : HistoricEdit
    {
        protected string lsDescripcion;

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            if (s == HistoricState.Consulta)
            {
                InitConfigPresupuestos();
            }
            else if (s == HistoricState.Edicion)
            {
                InitConfigPresupuestos();
            }

            if (pFields != null && pFields.ContainsConfigName("TipoPr"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("TipoPr").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PeriodoPr"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PeriodoPr").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PresupFijo"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PresupFijo").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PresupProv"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PresupProv").Row - 1].Visible = false;
            }
        }

        protected virtual void InitConfigPresupuestos()
        {
            //Revisa si la empresa a la que pertenece el empleado tiene prendida la bandera de Activar notificaciones de presupuestos
            //para mostrar el panel de subhistoricos en el cual se encuentran los configuradores de presupuesto fijo y presupuesto
            //temporal del empleado

            pPanelSubHistoricos.Visible = false;

            if (iCodCatalogo == "null")
            {
                return;
            }

            DateTime ldtAhora = DateTime.Now;

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select BanderasEmpre = isnull(Empre.BanderasEmpre,0)");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre");
            lsbQuery.AppendLine("where Empre.iCodCatalogo = (select CenCos.Empre");
            lsbQuery.AppendLine("    from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            lsbQuery.AppendLine("    where CenCos.iCodCatalogo = " + iCodCatalogo);
            lsbQuery.AppendLine("    and CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            lsbQuery.AppendLine("    and CenCos.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("    and CenCos.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            lsbQuery.AppendLine("and Empre.dtIniVigencia <> Empre.dtFinVigencia");
            lsbQuery.AppendLine("and Empre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Empre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

            if (lDataTable.Rows.Count > 0
                && ((int)lDataTable.Rows[0]["BanderasEmpre"] & 1) == 1)
            {
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select PrepEmpre.iCodCatalogo");
                lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
                lsbQuery.AppendLine("where PrepEmpre.Empre = (select CenCos.Empre");
                lsbQuery.AppendLine("    from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                lsbQuery.AppendLine("    where CenCos.iCodCatalogo = " + iCodCatalogo);
                lsbQuery.AppendLine("    and CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                lsbQuery.AppendLine("    and CenCos.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("    and CenCos.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                lsbQuery.AppendLine("and PrepEmpre.dtIniVigencia <> PrepEmpre.dtFinVigencia");
                lsbQuery.AppendLine("and PrepEmpre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("and PrepEmpre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

                if (lDataTable.Rows.Count > 0)
                {
                    pPanelSubHistoricos.Visible = true;
                }
            }
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();

            vchDescripcion.TcLbl.Visible = false;
            vchDescripcion.TcCtl.Visible = false;
            if (pFields.ContainsConfigName("TipoCenCost") && (pFields.GetByConfigName("TipoCenCost")).DataValue.ToString() == "null")
            {
                (pFields.GetByConfigName("TipoCenCost")).DataValue = "0";
            }
        }

        protected override void pbtnConsultar_ServerClick(object sender, EventArgs e)
        {
            base.pbtnConsultar_ServerClick(sender, e);
            if (pvchDescripcion.DataValue.ToString() != "null" && pFields.ContainsConfigName("Descripcion"))
            {
                KeytiaBaseField lField = pFields.GetByConfigName("Descripcion");
                if (lField.DataValue.ToString() == "null")
                {
                    lField.DataValue = vchDescripcion.DataValue.ToString().Replace("'", "");
                }
            }
        }

        protected override void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            ActualizaHistoria();
            base.pbtnGrabar_ServerClick(sender, e);
            ActualizaJerarquiaRest();
        }


        protected void ActualizaJerarquiaRest()
        {
            string iCodPadre = "";
            if (pFields.ContainsConfigName(vchCodEntidad))
            {
                iCodPadre = pFields.GetByConfigName(vchCodEntidad).DataValue.ToString();
            }

            KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");

            int liCodUsuario = (int)Session["iCodUsuarioDB"];
            pCargaCom.ActualizaJerarquiaRestCenCos(iCodCatalogo, iCodPadre, liCodUsuario);
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

        protected override bool ValidarRegistro()
        {
            return ValidarVigencias()
                && ValidarCampos()
                && ValidarAtribCatalogosVig()
                && ValidarClaves()
                && ValidarRelaciones()
                && ValidarRelCatBlancos()
                && ValidarRelCatVig()
                && ValidarDatos();
        }

        protected override bool ValidarClaves()
        {
            bool lbret = true;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));
            lsDescripcion = (pFields.GetByConfigName("Descripcion")).DataValue.ToString().Replace("'", "");

            try
            {
                if (State != HistoricState.Baja)
                {
                    if (pvchCodigo.HasValue)
                    {
                        lbret = ValidarCodigo();
                    }
                    else
                    {
                        lsbErrores.Append("<li>" + pvchCodigo.RequiredMessage + "</li>");
                    }

                    if (lsDescripcion != "null")
                    {
                        if (pFields.ContainsConfigName("CenCos"))
                        {
                            string lsCatCenCos = (pFields.GetByConfigName("CenCos")).DataValue.ToString().Replace("'", "");
                            if (!pFields.GetByConfigName("CenCos").DSOControlDB.HasValue)
                            {
                                //Centro de Costo Raiz
                                string lsClaveEmpresa = "";
                                if (pFields.ContainsConfigName("Empre") && pFields.GetByConfigName("Empre").DSOControlDB.HasValue)
                                {
                                    psbQuery.Length = 0;
                                    psbQuery.AppendLine("select vchCodigo");
                                    psbQuery.AppendLine("from [VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')]");
                                    psbQuery.AppendLine("where iCodCatalogo = " + pFields.GetByConfigName("Empre").DataValue);
                                    psbQuery.AppendLine("and dtIniVigencia <= " + pdtIniVigencia.DataValue);
                                    psbQuery.AppendLine("and dtFinVigencia > " + pdtIniVigencia.DataValue);
                                    lsClaveEmpresa = (DSODataAccess.ExecuteScalar(psbQuery.ToString())).ToString();

                                    pvchDescripcion.DataValue = lsDescripcion + " (" + lsClaveEmpresa + ")";
                                }
                                else
                                {
                                    pvchDescripcion.DataValue = lsDescripcion;
                                }
                            }
                            else if (iCodCatalogo != "null" && lsCatCenCos == iCodCatalogo)
                            {
                                //Error para cuando se asigna el mismo centro de costos como padre
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrCenCostPadre"));
                                lsbErrores.Append("<li>" + lsError + "</li>");
                            }
                            else
                            {
                                string lsClaveCenCosPadre = "";
                                psbQuery.Length = 0;
                                psbQuery.AppendLine("select vchCodigo");
                                psbQuery.AppendLine("from [VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')]");
                                psbQuery.AppendLine("where iCodCatalogo = " + lsCatCenCos);
                                psbQuery.AppendLine("and dtInivigencia <= " + pdtIniVigencia.DataValue);
                                psbQuery.AppendLine("and dtFinvigencia > " + pdtIniVigencia.DataValue);
                                lsClaveCenCosPadre = (DSODataAccess.ExecuteScalar(psbQuery.ToString())).ToString();

                                pvchDescripcion.DataValue = lsDescripcion + " (" + lsClaveCenCosPadre + ")";
                            }
                        }
                        else
                        {
                            pvchDescripcion.DataValue = lsDescripcion;
                        }
                    }
                    else
                    {
                        lsbErrores.Append("<li>" + pvchDescripcion.RequiredMessage + "</li>");
                    }

                    if (lsbErrores.Length > 0)
                    {
                        lbret = false;
                        lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));

            try
            {
                if (State == HistoricState.Edicion)
                {
                    lbret = CenCostPadre();
                    if (lbret && pFields.ContainsConfigName("Emple") && pFields.GetByConfigName("Emple").DSOControlDB.HasValue)
                    {
                        DataRow drEmple = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + pFields.GetByConfigName("Emple").DataValue).Rows[0];
                        DataRow drTipoEm = pKDB.GetHisRegByEnt("TipoEm", "Tipo Empleado", "iCodCatalogo = " + drEmple["{TipoEm}"]).Rows[0];

                        string lsTipoEmpleado = drTipoEm["vchCodigo"].ToString();
                        //RZ.20140129 Se omite validacion para cuando el empleado es tipo externo o empleado
                        if (lsTipoEmpleado != "E" && lsTipoEmpleado != "X")
                        {
                            lbret = false;
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValEmplResponsable"));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                            lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                            DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                        }
                    }
                }
                else if (State == HistoricState.Baja)
                {
                    lbret = CenCostHijos() && EmpleadosAsignados() && LineasAsignadas();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected bool CenCostPadre()
        {
            bool lbret = true;
            DataTable ldt;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            //StringBuilder lsbErrRangos = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));

            try
            {
                if (pFields.ContainsConfigName("CenCos") && pFields.ContainsConfigName("Empre"))
                {
                    KeytiaBaseField lField = pFields.GetByConfigName("CenCos");
                    if (lField.DSOControlDB.DataValue.ToString() == "null")
                    {
                        string lsEmpresa = pFields.GetByConfigName("Empre").DataValue.ToString();
                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select C.iCodCatalogo, C.iCodMaestro, C.dtIniVigencia, C.dtFinVigencia");
                        psbQuery.AppendLine("from [VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] C");
                        psbQuery.AppendLine("where C.CenCos is null");
                        psbQuery.AppendLine("and C.dtIniVigencia <> C.dtFinVigencia");
                        psbQuery.AppendLine("and ((dtInivigencia <= " + pdtIniVigencia.DataValue + " and");
                        psbQuery.AppendLine("      dtFinvigencia > " + pdtIniVigencia.DataValue + ")");
                        psbQuery.AppendLine(" or  (dtInivigencia <= " + pdtFinVigencia.DataValue + " and");
                        psbQuery.AppendLine("      dtFinvigencia > " + pdtFinVigencia.DataValue + ")");
                        psbQuery.AppendLine(" or  (dtInivigencia >= " + pdtIniVigencia.DataValue + " and");
                        psbQuery.AppendLine("      dtFinvigencia <= " + pdtFinVigencia.DataValue + "))");
                        psbQuery.AppendLine("and Empre = " + lsEmpresa);

                        if (iCodCatalogo != "null")
                        {
                            psbQuery.AppendLine("and iCodCatalogo <>" + iCodCatalogo);
                        }

                        ldt = DSODataAccess.Execute(psbQuery.ToString());

                        if (ldt != null && ldt.Rows.Count > 0)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgCenCosResp"))));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                        }

                        if (lsbErrores.Length > 0)
                        {
                            lbret = false;
                            lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                            DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                        }

                        //string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                        //foreach (DataRow ldataRow in ldt.Rows)
                        //{
                        //    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat), ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat)));
                        //    lsbErrRangos.Append("<li>" + lsError + "</li>");
                        //}

                        //if (lsbErrRangos.Length > 0)
                        //{
                        //    lbret = false;
                        //    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidaCenCosRaiz", pFields.GetByConfigName("Empre").ToString()));
                        //    lsError = "<span>" + lsError + "</span>";
                        //    lsbErrores.Append("<li>" + lsError);
                        //    lsbErrores.Append("<ul>" + lsbErrRangos.ToString() + "</ul>");
                        //    lsbErrores.Append("</li>");                                                      
                        //    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                        //    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);                            
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected bool CenCostHijos()
        {
            bool lbret = true;
            int liCount;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));

            try
            {
                if (pFields.ContainsConfigName("CenCos"))
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select CenCost = isnull(count(C.iCodRegistro),0)");
                    psbQuery.AppendLine("from [VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] C");
                    psbQuery.AppendLine("where C.CenCos = " + iCodCatalogo);
                    psbQuery.AppendLine("and C.dtIniVigencia <> C.dtFinVigencia");
                    psbQuery.AppendLine("and (C.dtIniVigencia < " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("or C.dtFinVigencia > " + pdtFinVigencia.DataValue + ")");

                    liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                    if (liCount > 0)
                    {
                        if (State != HistoricState.Baja)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValCenCosHijosAct"));
                        }
                        else
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValCenCosHijos"));
                        }
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected bool ValidarCodigo()
        {
            bool lbret = true;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));
            Regex lRegExp = new Regex("^[^,]+$");

            if (!lRegExp.IsMatch(pvchCodigo.DataValue.ToString()))
            {
                lbret = false;
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValClaveCenCos", pvchCodigo.Descripcion.ToString()));
                lsbErrores.Append("<li>" + lsError + "</li>");
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }
            else
            {
                if (State != HistoricState.Baja)
                {
                    lbret = ValidarRepetidos();
                }
            }

            return lbret;
        }

        protected bool ValidarRepetidos()
        {
            bool lbret = true;
            int liCount;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));
            string lsValidacion = (pFields.GetByConfigName("TipoCenCost")).DataValue.ToString();

            try
            {
                string lsCenCost = (pFields.GetByConfigName("CenCos")).DSOControlDB.DataValue.ToString();

                psbQuery.Length = 0;
                psbQuery.AppendLine("select CenCost = isnull(count(icodregistro),0)");
                psbQuery.AppendLine("from [VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where");

                if (iCodCatalogo != "null")
                {
                    psbQuery.AppendLine("   iCodCatalogo <> " + iCodCatalogo + " and");
                }

                if (lsValidacion == "0")
                {
                    //Validacion para los centros de costos que no son duplicados en clave o descipcion
                    psbQuery.AppendLine("(vchCodigo = " + pvchCodigo.DataValue.ToString() + " or Descripcion = '" + lsDescripcion + "')");
                }
                else if (lsValidacion == "1")
                {
                    //Validacion donde no puede tener la misma descripcion                   
                    psbQuery.AppendLine("Descripcion = '" + lsDescripcion + "'");
                }
                else if (lsValidacion == "2")
                {
                    //Validacion donde no puede tener la misma clave                      
                    psbQuery.AppendLine("vchCodigo = " + pvchCodigo.DataValue.ToString());
                }
                else if (lsValidacion == "4")
                {
                    //Validacion donde puede tener la misma descripcion y la misma clave
                    //pero no depende del mismo centro de costos
                    psbQuery.AppendLine("vchCodigo = " + pvchCodigo.DataValue.ToString() + " and Descripcion = '" + lsDescripcion + "'");
                    psbQuery.AppendLine("and (CenCos is null or CenCos = " + lsCenCost + ")");
                }
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("and ((dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                psbQuery.AppendLine("   or (dtIniVigencia <= " + pdtFinVigencia.DataValue);
                psbQuery.AppendLine("       and dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                psbQuery.AppendLine("   or (dtIniVigencia >= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");

                liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                if (liCount > 0)
                {
                    lbret = false;
                    if (lsValidacion == "0")
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CenCostDuplicado"));
                    }
                    else if (lsValidacion == "1")
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CenCosDescDuplicada"));
                    }
                    else if (lsValidacion == "2")
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CenCosCveDuplicada"));
                    }
                    else
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CenCostValDuplicado"));
                    }
                    lsbErrores.Append("<li>" + lsError + "</li>");
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    return lbret;
                }

                psbQuery.Length = 0;
                psbQuery.AppendLine("select CenCost = isnull(count(icodregistro),0)");
                psbQuery.AppendLine("from [VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where");
                psbQuery.AppendLine("   (    ((vchCodigo = " + pvchCodigo.DataValue.ToString() + " or Descripcion = '" + lsDescripcion + "') and tipoCenCost = 0)");
                psbQuery.AppendLine("     or (vchCodigo = " + pvchCodigo.DataValue.ToString() + " and tipoCenCost = 2)");
                psbQuery.AppendLine("     or (Descripcion = '" + lsDescripcion + "' and tipoCenCost = 1))");
                if (iCodCatalogo != "null")
                {
                    psbQuery.AppendLine("and  iCodCatalogo <> " + iCodCatalogo);
                }
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("and ((dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                psbQuery.AppendLine("   or (dtIniVigencia <= " + pdtFinVigencia.DataValue);
                psbQuery.AppendLine("       and dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                psbQuery.AppendLine("   or (dtIniVigencia >= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("       and dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");

                liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                if (liCount > 0)
                {
                    lbret = false;
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CenCosDuplicado"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }

                return lbret;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected bool EmpleadosAsignados()
        {
            bool lbret = true;
            string lsError;
            int liCount;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select Empleados = isnull(count(R.iCodRegistro),0)");
                psbQuery.AppendLine("from [VisRelaciones('CentroCosto-Empleado','" + Globals.GetCurrentLanguage() + "')] R,");
                psbQuery.AppendLine("     [VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] E");
                psbQuery.AppendLine("where R.CenCos = " + iCodCatalogo);
                psbQuery.AppendLine("and R.dtIniVigencia <> R.dtFinVigencia");
                psbQuery.AppendLine("and (R.dtIniVigencia < '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("  or R.dtFinVigencia > '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                psbQuery.AppendLine("and R.Emple = E.iCodCatalogo");
                psbQuery.AppendLine("and E.dtIniVigencia <> E.dtFinVigencia");
                psbQuery.AppendLine("and ((E.dtIniVigencia <= R.dtIniVigencia and E.dtFinVigencia > R.dtIniVigencia)");
                psbQuery.AppendLine("  or (E.dtIniVigencia <= R.dtFinVigencia and E.dtFinVigencia > R.dtFinVigencia)");
                psbQuery.AppendLine("  or (E.dtIniVigencia >= R.dtFinVigencia and E.dtFinVigencia <= R.dtIniVigencia))");

                liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                if (liCount > 0)
                {
                    lbret = false;
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmpleAsoc"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected bool LineasAsignadas()
        {
            bool lbret = true;
            string lsError;
            int liCount;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCentrosdeCostos"));

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select Lineas = isnull(count(R.iCodRegistro),0)");
                psbQuery.AppendLine("from [VisRelaciones('CentroCosto-Lineas','" + Globals.GetCurrentLanguage() + "')] R,");
                psbQuery.AppendLine("     [VisHistoricos('Linea','Lineas','" + Globals.GetCurrentLanguage() + "')] L");
                psbQuery.AppendLine("where R.CenCos = " + iCodCatalogo);
                psbQuery.AppendLine("and R.dtIniVigencia <> R.dtFinVigencia");
                psbQuery.AppendLine("and (R.dtIniVigencia < '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("  or R.dtFinVigencia > '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                psbQuery.AppendLine("and R.Linea = L.iCodCatalogo");
                psbQuery.AppendLine("and L.dtIniVigencia <> L.dtFinVigencia");
                psbQuery.AppendLine("and ((L.dtIniVigencia <= R.dtIniVigencia and L.dtFinVigencia > R.dtIniVigencia)");
                psbQuery.AppendLine("  or (L.dtIniVigencia <= R.dtFinVigencia and L.dtFinVigencia > R.dtFinVigencia)");
                psbQuery.AppendLine("  or (L.dtIniVigencia >= R.dtFinVigencia and L.dtFinVigencia <= R.dtIniVigencia))");

                liCount = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                if (liCount > 0)
                {
                    lbret = false;
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "LineaAsoc"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            return lbret;
        }

        protected void ActualizaHistoria()
        {
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            Hashtable phtValues = new Hashtable();
            string liMaestro = (DSODataAccess.ExecuteScalar("select bActualizaHistoria from maestros where iCodRegistro = " + iCodMaestro + " and iCodEntidad = " + iCodEntidad)).ToString();

            if (liMaestro == "0")
            {
                phtValues.Add("bActualizaHistoria", 1);

                if (!lCargasCOM.ActualizaRegistro("Maestros", vchCodEntidad, vchDesMaestro, phtValues, int.Parse(iCodMaestro), false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked))
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields != null)
            {
                if (pFields.ContainsConfigName("CenCos"))
                {
                    KeytiaBaseField lField = pFields.GetByConfigName("CenCos");
                    lField.DSOControlDB.Descripcion = Globals.GetMsgWeb(false, "MsgCenCosResp");
                    //lField.DSOControlDB.TcLbl.Text = Globals.GetMsgWeb(false, "MsgCenCosResp"); 
                }
                if (pFields.ContainsConfigName("Emple"))
                {
                    KeytiaBaseField lField = pFields.GetByConfigName("Emple");
                    lField.DSOControlDB.Descripcion = Globals.GetMsgWeb(false, "MsgEmpleResp");
                    //lField.DSOControlDB.TcLbl.Text = Globals.GetMsgWeb(false, "MsgEmpleResp");
                }
            }
        }

        protected override void InitHisGridLanguage()
        {
            base.InitHisGridLanguage();
            KeytiaBaseField lField;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                if (pFields.Contains(lCol.sName))
                {
                    lField = pFields[lCol.sName];

                    if (lField.ConfigName == "Emple")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgEmpleResp"));
                    }
                    else if (lField.ConfigName == "CenCos")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgCenCosResp"));
                    }
                }
            }
        }
    }
}
