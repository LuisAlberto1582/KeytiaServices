using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;

namespace KeytiaWeb.UserInterface
{
    public class CnfgAsistentes : HistoricEdit
    {
        protected StringBuilder psbErrores;
        public CnfgAsistentes()
        {
            Init += new EventHandler(CnfgAsistentes_Init);
        }

        void CnfgAsistentes_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgAsistentes";
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgAsistentes.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgAsistentes.js") + "'type='text/javascript'></script>\r\n", true, false);
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;

        }

        protected override bool ValidarVigencias()
        {
            if (!pdtIniVigencia.HasValue)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
            }
            if (!pdtFinVigencia.HasValue)
            {
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

            //el usuario nunca edita las vigencias por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarClaves()
        {
            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            if (pFields.GetByConfigName("Emple").DSOControlDB.HasValue)
            {
                pvchDescripcion.DataValue = pFields.GetByConfigName("Emple").ToString().Substring(0, Math.Min(pFields.GetByConfigName("Emple").ToString().Length, 160));
            }
            if (!pvchDescripcion.HasValue)
            {
                pvchDescripcion.DataValue = "Default";
            }

            //el usuario nunca edita estos valores por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarDatos()
        {
            bool lbRet = base.ValidarDatos();
            if (lbRet)
            {
                psbErrores = new StringBuilder();

                //Validar que un empleado no sea asistente en dos equipos diferentes en el mismo horario
                ValidarTraslapes();

                if (psbErrores.Length > 0)
                {
                    lbRet = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbRet;
        }

        protected void ValidarTraslapes()
        {
            if (pFields.ContainsConfigName("Emple") && pFields.ContainsConfigName("Participante"))
            {
                KeytiaBaseField lFieldEmple = pFields.GetByConfigName("Emple");
                string lsLang = Globals.GetCurrentLanguage();
                string lsFechaInicioReservacion = null, lsFechaFinReservacion = null;

                int liEstPartEliminado = KDBUtil.SearchICodCatalogo("EstParticipante", "Eliminado", true);
                int liEstConfEliminado = KDBUtil.SearchICodCatalogo("EstConferencia", "Eliminada", true);

                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("Select TMSConf.FechaInicioReservacion, TMSConf.FechaFinReservacion");
                lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','Español')] TMSConf,");
                lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','Español')] Participante");
                lsbQuery.AppendLine("where TMSConf.iCodCatalogo = Participante.TMSConf");
                lsbQuery.AppendLine("and Participante.iCodCatalogo = " + pFields.GetByConfigName("Participante").DataValue.ToString());
                lsbQuery.AppendLine("and TMSConf.dtIniVigencia <> TMSConf.dtFinVigencia");
                lsbQuery.AppendLine("and TMSConf.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                lsbQuery.AppendLine("and TMSConf.dtFinVigencia > " + pdtIniVigencia.DataValue);
                lsbQuery.AppendLine("and Participante.dtIniVigencia <> Participante.dtFinVigencia");
                lsbQuery.AppendLine("and Participante.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                lsbQuery.AppendLine("and Participante.dtFinVigencia > " + pdtIniVigencia.DataValue);
                lsbQuery.AppendLine("and Participante.EstParticipante <> " + liEstPartEliminado);
                lsbQuery.AppendLine("and TMSConf.EstConferencia <> " + liEstConfEliminado);

                DataTable ldtTMSConf = DSODataAccess.Execute(lsbQuery.ToString());

                if (ldtTMSConf != null && ldtTMSConf.Rows.Count > 0)
                {
                    lsFechaInicioReservacion = ((DateTime)ldtTMSConf.Rows[0]["FechaInicioReservacion"]).ToString("yyyy-MM-dd HH:mm:ss");
                    lsFechaFinReservacion = ((DateTime)ldtTMSConf.Rows[0]["FechaFinReservacion"]).ToString("yyyy-MM-dd HH:mm:ss");

                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("Select TMSConf.vchDescripcion, Asistente.EmpleDesc, TMSConf.FechaInicioReservacion, TMSConf.FechaFinReservacion ");
                    lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','" + lsLang + "')] TMSConf,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','" + lsLang + "')] Participante,");
                    lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('AsistenteConferencia','Asistentes','" + lsLang + "')] Asistente");
                    lsbQuery.AppendLine("where TMSConf.iCodCatalogo = Participante.TMSConf");
                    lsbQuery.AppendLine("and Participante.iCodCatalogo = Asistente.Participante");
                    lsbQuery.AppendLine("and Asistente.Emple = " + lFieldEmple.DataValue.ToString());
                    if (iCodCatalogo != "null")
                    {
                        lsbQuery.AppendLine("and Asistente.iCodCatalogo <> " + iCodCatalogo);
                    }
                    lsbQuery.AppendLine("and (");
                    lsbQuery.AppendLine("		(	TMSConf.FechaInicioReservacion >= '" + lsFechaInicioReservacion + "'");
                    lsbQuery.AppendLine("		and TMSConf.FechaInicioReservacion <  '" + lsFechaFinReservacion + "' )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaFinReservacion >= '" + lsFechaInicioReservacion + "'");
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion <  '" + lsFechaFinReservacion + "' )");
                    lsbQuery.AppendLine("	or	(	TMSConf.FechaInicioReservacion <= '" + lsFechaInicioReservacion + "'");
                    lsbQuery.AppendLine("		and TMSConf.FechaFinReservacion >= '" + lsFechaFinReservacion + "' )");
                    lsbQuery.AppendLine("	)");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <> TMSConf.dtFinVigencia");
                    lsbQuery.AppendLine("and TMSConf.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and TMSConf.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <> Participante.dtFinVigencia");
                    lsbQuery.AppendLine("and Participante.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Participante.dtFinVigencia >  " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Asistente.dtIniVigencia <> Asistente.dtFinVigencia");
                    lsbQuery.AppendLine("and Asistente.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    lsbQuery.AppendLine("and Asistente.dtFinVigencia >  " + pdtIniVigencia.DataValue);

                    ldtTMSConf = DSODataAccess.Execute(lsbQuery.ToString());

                    if (ldtTMSConf != null && ldtTMSConf.Rows.Count > 0)
                    {
                        string lsDateTimeFormat = Globals.GetLangItem("NetDateTimeFormat");
                        string lsError = ldtTMSConf.Rows[0]["EmpleDesc"].ToString() + ": "
                            + ldtTMSConf.Rows[0]["vchDescripcion"]
                            + " (" + ((DateTime)ldtTMSConf.Rows[0]["FechaInicioReservacion"]).ToString(lsDateTimeFormat)
                            + " - " + ((DateTime)ldtTMSConf.Rows[0]["FechaFinReservacion"]).ToString(lsDateTimeFormat) + ")";
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslape", lsError));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            OcultaCampo("EstAsistente");
        }

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
            if (State != HistoricState.Baja)
            {
                CambiarEstatus("PorNotificar");
            }
            else
            {
                CambiarEstatus("Eliminado");
            }
        }

        private void CambiarEstatus(string lvchCodEstatus)
        {
            System.Collections.Hashtable lhtVal = new System.Collections.Hashtable();
            lhtVal.Add("iCodRegistro", int.Parse(iCodRegistro));
            lhtVal.Add("{EstAsistente}", KDBUtil.SearchICodCatalogo("EstAsistente", lvchCodEstatus, true));
            KDBUtil.SaveHistoric("AsistenteConferencia", "Asistentes", vchCodigo.TextBox.Text, null, lhtVal);
        }

        #region WebMethods
        public static string SearchEmpleByClient(string term, string iCodCliente, object iniVigencia, object finVigencia)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                string lsLang = Globals.GetCurrentLanguage();

                string lsQuery = "select top (100) * from [VisHistoricos('Emple','" + lsLang + "')]";

                DataTable ldtVista = DSODataAccess.Execute(lsQuery + " where 1 = 2");

                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("select Emple.iCodRegistro, Emple.iCodCatalogo, Emple.vchCodigo, Emple.vchDescripcion, Emple.NomCompleto");
                lsbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')] Emple,");
                lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('CenCos','Centro de Costos','Español')] CenCos,");
                lsbQuery.AppendLine("     [" + DSODataContext.Schema + "].[VisHistoricos('Empre','Empresas','Español')] Empre");
                lsbQuery.AppendLine("where Emple.CenCos = CenCos.iCodCatalogo");
                lsbQuery.AppendLine("and CenCos.Empre = Empre.iCodCatalogo");
                lsbQuery.AppendLine("and Empre.Client = " + iCodCliente);
                lsbQuery.AppendLine("and Emple.dtIniVigencia <> Emple.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "Emple"));
                lsbQuery.AppendLine("and CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "CenCos"));
                lsbQuery.AppendLine("and Empre.dtIniVigencia <> Empre.dtFinVigencia");
                lsbQuery.AppendLine(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "Empre"));

                #region Se quitaron las restricciones por centro de costo, para que que se puedan ver todos los empleados de un mismo cliente
                //lsbQuery.Append("and Emple.iCodCatalogo in(");
                //if (iniVigencia == null || iniVigencia.ToString() == "null")
                //{
                //    iniVigencia = DateTime.Today;
                //}

                //if (finVigencia == null || finVigencia.ToString() == "null")
                //{
                //    finVigencia = new DateTime(2079, 01, 01);
                //}

                //lsbQuery.Append("select distinct(iCodCatalogo) from ");
                //lsbQuery.Append(DSODataContext.Schema + ".GetRestriccionVigencia(" + HttpContext.Current.Session["iCodUsuario"] + ", " + HttpContext.Current.Session["iCodPerfil"] + ", 'Emple', '" + DSOControl.ParseDateTimeJS(iniVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DSOControl.ParseDateTimeJS(finVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "', default)");
                //lsbQuery.AppendLine(")");
                #endregion

                lsbQuery.AppendLine("and (Emple.vchDescripcion + ' (' + Emple.vchCodigo + ')' like '%" + term.Replace("'", "''") + "%')");

                lsQuery = lsbQuery.ToString();
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine(lsQuery + ") Emple");
                lsbQuery.AppendLine("where Emple.iCodRegistro = (select Max(His.iCodRegistro) from Historicos His");
                lsbQuery.AppendLine("   where His.iCodCatalogo = Emple.iCodCatalogo");
                lsbQuery.AppendLine("   and His.dtIniVigencia <> His.dtFinVigencia");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(iniVigencia, finVigencia, true, "His"));
                lsbQuery.AppendLine(")");
                lsbQuery.AppendLine("order by Emple.vchDescripcion");

                ldtVista = DSODataAccess.Execute(lsbQuery.ToString());

                DataTable lDataSource = HistoricEdit.FillDSOAutoDataSource(ldtVista);
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
